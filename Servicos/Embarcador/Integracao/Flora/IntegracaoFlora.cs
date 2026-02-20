using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Flora;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Flora
{
    public class IntegracaoFlora
    {

        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora _configuracaoIntegracaoFlora;

        #endregion Atributos Privados

        #region Construtores

        public IntegracaoFlora(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoFlora();

                string token = Login();
                HttpClient requisicao = CriarRequisicao(RemoverBearer(token));

                ContratacaoVeiculo objetoContratacaoVeiculo = ObterContratacaoVeiculo(cargaDadosTransporteIntegracao);

                jsonRequisicao = JsonConvert.SerializeObject(objetoContratacaoVeiculo, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracaoFlora.EnvioCarga, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Bearer token inválido ou expirado.");

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    RetornoContratacaoVeiculo retornoContratacao = JsonConvert.DeserializeObject<RetornoContratacaoVeiculo>(jsonRetorno);

                    if (retornoContratacao.Sucesso == "False")
                        throw new ServicoException(retornoContratacao.CodigoRetorno + " - " + retornoContratacao.Mensagem);

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso.";

                }
                else
                    throw new ServicoException($"Falha ao conectar na API Flora: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Flora";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private ContratacaoVeiculo ObterContratacaoVeiculo(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            ContratacaoVeiculoDetalhes requisicaoVeiculoDetalhes = ObterVeiculoDetalhes(cargaDadosTransporteIntegracao);

            ContratacaoVeiculo requisicaoVeiculo = new ContratacaoVeiculo()
            {
                ContratacaoVeiculoDetalhes = requisicaoVeiculoDetalhes
            };

            return requisicaoVeiculo;
        }

        private ContratacaoVeiculoDetalhes ObterVeiculoDetalhes(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaDadosTransporteIntegracao.Carga;
            Dominio.Entidades.Usuario motorista = repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCarga(carga.Codigo);

            ContratacaoVeiculoDetalhes veiculoDetalhes = new ContratacaoVeiculoDetalhes()
            {
                ProtocoloCarregamento = carga.Protocolo.ToString(),
                MidiaOrigem = 45,
                Veiculos = new ListaVeiculo()
                {
                    ListaVeiculos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Flora.Veiculo()
                    {
                        TemVeiculo = 1,
                        Placa = !string.IsNullOrWhiteSpace(carga.Veiculo.CodigoIntegracao) ? carga.Veiculo.CodigoIntegracao : carga.Veiculo.Placa,
                        PlacaPrincipal = carga.Veiculo.Placa,
                        CodigoIntegracaoTransportadora = carga.Empresa.CodigoIntegracao,
                        TransportadoraSubContratada = "0",
                        CodigoIntegracaoMotorista = motorista.CodigoIntegracao,
                        PrevisaoColeta = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToDateTimeString() : string.Empty,
                        KmTotal = carga.DadosSumarizados.Distancia,
                        VeiculoAtrelado = ObterVeiculosAtrelados(carga.VeiculosVinculados.ToList()),
                        VeiculoEvento = ObterVeiculosEvento(carga),
                        VeiculoInformacao = ObterVeiculosInformacao(carga)
                    }

                }

            };

            return veiculoDetalhes;
        }

        private List<VeiculoAtrelado> ObterVeiculosAtrelados(List<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            List<VeiculoAtrelado> veiculosAtreladosList = new List<VeiculoAtrelado>();

            foreach (Dominio.Entidades.Veiculo item in veiculosVinculados)
            {
                veiculosAtreladosList.Add(new VeiculoAtrelado() { CodigoIntegracaoVeiculoAtrelado = !string.IsNullOrWhiteSpace(item.CodigoIntegracao) ? item.CodigoIntegracao : item.Placa });
            }

            return veiculosAtreladosList;
        }

        private List<VeiculoEvento> ObterVeiculosEvento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Operador)
                return null;

            List<VeiculoEvento> veiculosEventoList = new List<VeiculoEvento>
            {
                new VeiculoEvento() { CodigoEvento = 798, Valor = carga.ValorFrete, BloqueadoAlteracao = "1" }
            };

            return veiculosEventoList;
        }

        private List<VeiculoInformacao> ObterVeiculosInformacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (string.IsNullOrWhiteSpace(carga.ObservacaoTransportador))
                return null;

            List<VeiculoInformacao> veiculosInformacaoList = new List<VeiculoInformacao>
            {
                new VeiculoInformacao() { Item = 1, CodigoEvento = 600, Conteudo = carga.ObservacaoTransportador }
            };

            return veiculosInformacaoList;
        }

        private string Login()
        {
            HttpClient requisicao = CriarRequisicao();

            dynamic dadosLogin = new
            {
                username = _configuracaoIntegracaoFlora.Usuario,
                password = _configuracaoIntegracaoFlora.Senha
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(dadosLogin), Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracaoFlora.URL, content).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                throw new ServicoException("Não foi possível fazer login.");            

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

            if (obj?.accessToken == "invalid_client")
                throw new ServicoException("Usuário ou senha incorretos.");

            return obj?.accessToken;
        }

        private string RemoverBearer(string token)
        {
            const string prefixoBearer = "Bearer ";

            if (token.StartsWith(prefixoBearer))
            {
                return token.Substring(prefixoBearer.Length);
            }

            return token;
        }

        private HttpClient CriarRequisicao(string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoFlora));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(3);

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        private void ObterConfiguracaoIntegracaoFlora()
        {
            if (_configuracaoIntegracaoFlora != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFlora repositorioIntegracaoFlora = new Repositorio.Embarcador.Configuracoes.IntegracaoFlora(_unitOfWork);
            _configuracaoIntegracaoFlora = repositorioIntegracaoFlora.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracaoFlora == null || !_configuracaoIntegracaoFlora.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Flora.");
        }

        #endregion Métodos Privados

    }
}
