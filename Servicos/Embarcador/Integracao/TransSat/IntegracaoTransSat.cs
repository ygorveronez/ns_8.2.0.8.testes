using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus;
using Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Servicos.Embarcador.Integracao.TransSat
{
    public partial class IntegracaoTransSat
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoTransSat _configuracaoIntegracaoRepositorio;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat _configuracaoIntegracao;
        string tokenAutenticacao = null;

        #endregion Atributos Globais

        #region Construtores
        public IntegracaoTransSat(Repositorio.UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
            _configuracaoIntegracaoRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoTransSat(unitOfWork);
            _configuracaoIntegracao = _configuracaoIntegracaoRepositorio.Buscar();

        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.NumeroTentativas++;

                string token = _configuracaoIntegracao.TokenIntegracaoTransSat;

                var request = SolicitacaoMonitoramentoDeViagem(cargaIntegracao.Carga);

                var retWS = Transmitir(request, "integracao/json_sm", token, _configuracaoIntegracao.URLWebServiceIntegracaoTransSat);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.RetornoSolicitacaoMonitoramentoSucesso retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.RetornoSolicitacaoMonitoramentoSucesso>(retWS.jsonRetorno);

                    cargaIntegracao.CodigoExternoRetornoIntegracao = retorno?.CDSOLICITACAO ?? "0";
                }

                cargaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public retornoWebService Transmitir(object objEnvio, string metodo, string token, string uri, enumTipoWS tipoWS = enumTipoWS.POST)
        {
            var retornoWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebService();

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(uri))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (uri.EndsWith("/"))
                    url = uri;
                else
                    url = uri + "/";
                url += metodo;

                HttpClient requisicao = CriarRequisicao(url, token);

                retornoWS.jsonRequisicao = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);

                HttpResponseMessage retornoRequisicao;

                StringContent conteudoRequisicao = new StringContent(retornoWS.jsonRequisicao, Encoding.UTF8, "application/json");
                retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;

                retornoWS.jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.RetornoSolicitacaoMonitoramentoFalha retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.RetornoSolicitacaoMonitoramentoFalha>(retornoWS.jsonRetorno);

                    if ((retorno?.STATUS == false) && (retorno?.ERRO != null || retorno?.ERRO?.Count > 0))
                        throw new ServicoException(@$"Erro: {string.Join(", ", retorno.ERRO)}");

                    retornoWS.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    retornoWS.ProblemaIntegracao = "Registro integrado com sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.RetornoSolicitacaoMonitoramentoFalha retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.RetornoSolicitacaoMonitoramentoFalha>(retornoWS.jsonRetorno);

                    if ((retorno?.STATUS == false) && (retorno?.ERRO != null || retorno?.ERRO?.Count > 0))
                        throw new ServicoException(@$"Erro: {string.Join(", ", retorno.ERRO)}");

                }
            }
            catch (ServicoException ex)
            {
                retornoWS.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retornoWS.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                retornoWS.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retornoWS.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da TransSat";
            }

            if (retornoWS?.ProblemaIntegracao.Length > 300)
                retornoWS.ProblemaIntegracao = retornoWS.ProblemaIntegracao.Substring(0, 300);

            return retornoWS;
        }

        public HttpClient CriarRequisicao(string url, string accessToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTransSat));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (accessToken != null)
            {
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return requisicao;
        }

        #endregion

        #region Métodos Privados

        private SolicitacaoMonitoramentoEnvio SolicitacaoMonitoramentoDeViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            SolicitacaoMonitoramento solicitacaoMonitoramento = new SolicitacaoMonitoramento();

            solicitacaoMonitoramento.CLIENTE = ObterCliente(carga.Empresa);
            solicitacaoMonitoramento.VIAGEM = ObterViagem(carga);
            solicitacaoMonitoramento.VEICULO = ObterVeiculo(carga);
            solicitacaoMonitoramento.CARGA = ObterCarga(carga);
            solicitacaoMonitoramento.PLANEJAMENTO = ObterPlanejamento(carga);
            solicitacaoMonitoramento.OBS = carga.ObservacaoTransportador ?? "";
            solicitacaoMonitoramento.MANTER_SINAL = carga.FreteDeTerceiro ? "N" : "S";

            SolicitacaoMonitoramentoEnvio solicitacaoMonitoramentoEnvio = new SolicitacaoMonitoramentoEnvio { sm = solicitacaoMonitoramento };

            return solicitacaoMonitoramentoEnvio;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Veiculo ObterVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Veiculo == null)
                return null;

            List<string> placasCarreta = new List<string>();
            var contatosMotorista = carga.Motoristas.SelectMany(motorista => motorista.Contatos).Select(contato => contato.Nome).ToList();
            var contatosAjudante = carga.Ajudantes.SelectMany(ajudante => ajudante.Contatos).Select(contato => contato.Nome).ToList();

            foreach (Dominio.Entidades.Veiculo carreta in carga.VeiculosVinculados)
            {
                if (carreta.DescricaoTipoVeiculo == "Reboque")
                {
                    placasCarreta.Add(carreta.Placa);
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Telefone motoristaTelefone = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Telefone()
            {
                TELEFONES = new List<string> { carga.Motoristas.Select(motorista => motorista.Telefone_Formatado).FirstOrDefault(), carga.Motoristas.Select(motorista => motorista.Celular_Formatado).FirstOrDefault() },
                TIPO = new List<string> { "Telefone Fixo", "Telefone Celular" },
                CONTATO = contatosMotorista ?? null,
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Telefone ajudanteTelefone = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Telefone()
            {
                TELEFONES = new List<string> { carga.Ajudantes.Select(ajudante => ajudante.Telefone_Formatado).FirstOrDefault(), carga.Ajudantes.Select(ajudante => ajudante.Celular_Formatado).FirstOrDefault() },
                TIPO = new List<string> { "Telefone Fixo", "Telefone Celular" },
                CONTATO = contatosAjudante ?? null,
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Veiculo()
            {
                PLACA = carga.Veiculo?.Placa,
                PLACA_CARRETA = placasCarreta ?? null,
                MOTORISTA_CPF = carga.Motoristas.Select(motorista => motorista.CPF).FirstOrDefault() ?? "O motorista não foi encontrado!",
                AJUDANTE_CPF = carga.Ajudantes.Select(ajudante => ajudante.CPF).FirstOrDefault() ?? null,
                MOTORISTA_TELEFONE = motoristaTelefone ?? null,
                AJUDANTE_TELEFONE = ajudanteTelefone ?? null
            };

            return veiculo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Cliente ObterCliente(Dominio.Entidades.Empresa empresa)
        {
            if (empresa == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Cliente
            {
                SOLICITANTE = empresa.RazaoSocial,
                EMAIL_RETORNO = _configuracaoIntegracao.EmailParaReceberRetornoDaGR != null ? _configuracaoIntegracao.EmailParaReceberRetornoDaGR : "Não foi cadastrado um e-mail para retorno."
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Viagem ObterViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return null;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = carga.Pedidos?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido detalhesCarga = repCargaPedido.BuscarPorCodigoDetalhes(cargaPedidoPrimeiro.Codigo);

            List<string> locaisDestino = new List<string>();
            List<string> enderecosDestino = new List<string>();
            List<int> numerosNotasFiscais = new List<int>();
            DateTime? dataInicio = carga.Pedidos.Min(o => o.Pedido.DataInicialColeta);
            DateTime? dataFim = carga.Pedidos.Max(o => o.Pedido.PrevisaoEntrega);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoInformacoes in carga.Pedidos)
            {
                string localDestino = cargaPedidoInformacoes.Pedido?.Destino?.DescricaoCidadeEstado ?? "Descrição do local de destino não encontrada!";
                string enderecoDestino = cargaPedidoInformacoes.Pedido?.Destinatario?.EnderecoCompletoCidadeeEstado ?? "Endereço do destinário não foi encontrado!";

                locaisDestino.Add(localDestino);
                enderecosDestino.Add(enderecoDestino);
                
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaFiscal in cargaPedidoInformacoes.NotasFiscais)
                {
                    numerosNotasFiscais.Add(notaFiscal.XMLNotaFiscal.Numero);
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Viagem()
            {
                LOCAL_ORIGEM = carga.DadosSumarizados.Origens,
                LOCAL_DESTINO = locaisDestino,
                ENDERECO_ORIGEM = detalhesCarga.Pedido?.Remetente?.EnderecoCompletoCidadeeEstado,
                ENDERECO_DESTINO = enderecosDestino,
                DATA_INICIO = dataInicio?.ToString("dd/MM/yyyy") ?? "",
                HORA_INICIO = dataInicio?.ToString("HH:mm") ?? "",
                DATA_FIM = dataFim?.ToString("dd/MM/yyyy") ?? "",
                HORA_FIM = dataFim?.ToString("HH:mm") ?? "",
                NF = String.Join(", ", numerosNotasFiscais)
            };

            return viagem;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Carga ObterCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return null;

            DateTime? dataInicio = carga.Pedidos.Min(o => o.Pedido.DataInicialColeta);

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Carga cargaInformacoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Carga()
            {
                DATA = dataInicio?.ToString("dd/MM/yyyy") ?? "",
                VALOR = carga.ValorFrete,
                TIPO = carga.TipoDeCarga?.Descricao ?? "",
                COD_TIPO = carga.TipoDeCarga?.Codigo.ToString() ?? "",
                PESO = (int)Math.Round(carga.DadosSumarizados.PesoTotal),
                TEMPERATURA = carga.FaixaTemperatura?.DescricaoVariancia
            };

            return cargaInformacoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Planejamento ObterPlanejamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return null;

            int.TryParse(carga.Rota?.CodigoIntegracao, out int codigoRotaIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Planejamento planejamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat.Planejamento()
            {
                ROTEIRO = new List<string> { carga.Rota.Descricao ?? "Rota sem descrição!" },
                ROTA = codigoRotaIntegracao,
            };

            return planejamento;
        }

        #endregion Métodos Privados
    }
}
