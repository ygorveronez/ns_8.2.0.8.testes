using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.FS;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.FS
{
    public class IntegracaoFS
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFS _configuracaoIntegracaoFS;

        #endregion Atributos Privados

        #region Construtores

        public IntegracaoFS(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;

            if (cargaDadosTransporteIntegracao.Carga.CalculandoFrete || cargaDadosTransporteIntegracao.Carga.ValorFrete <= 0)
            {
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now.AddMinutes(2);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "O valor do frete ainda não foi calculado.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Requisicao.InputRequisicao corpoRequisicao = ObterDadosCargaIntegracao(cargaDadosTransporteIntegracao.Carga);

                respostaHttp = ExecutarRequisicao(corpoRequisicao, "/BuscarAgendamentoEtanol_Multi");

                if (respostaHttp.httpStatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Ocorreu um erro interno no servidor requisitado FS.");

                Response<Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Retorno.DadosRetorno> retorno = JsonConvert.DeserializeObject<Response<Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Retorno.DadosRetorno>>(respostaHttp.conteudoResposta);

                if (!respostaHttp.sucesso || respostaHttp.httpStatusCode != HttpStatusCode.Created || string.IsNullOrWhiteSpace(retorno.Dados.Agendamento))
                    throw new ServicoException(retorno.Dados.Erro ?? "Ocorreu um erro ao realizar a integração com a FS.");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = $"Integração realizada com sucesso! Agendamento {retorno.Dados.Agendamento}";
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoFS");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu um erro ao realizar a integração com a FS.";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCancelamentoCargaIntegracao.NumeroTentativas++;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao.InputRequisicao corpoRequisicao = ObterDadosCancelamentoCargaIntegracao(cargaCancelamentoCargaIntegracao.CargaCancelamento);

                respostaHttp = ExecutarRequisicao(corpoRequisicao, "/CancelaCargaEtanol_Multi");

                if (respostaHttp.httpStatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Ocorreu um erro interno no servidor requisitado FS.");

                Response<Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Retorno.DadosRetorno> retorno = JsonConvert.DeserializeObject<Response<Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Retorno.DadosRetorno>>(respostaHttp.conteudoResposta);

                if (!respostaHttp.sucesso || respostaHttp.httpStatusCode != HttpStatusCode.Created || !string.IsNullOrWhiteSpace(retorno.Dados.Erro))
                    throw new ServicoException(retorno.Dados.Erro ?? "Ocorreu um erro ao realizar a integração com a FS.");

                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso!";
            }
            catch (ServicoException excecao)
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoFS");

                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Problema ao tentar integrar com FS.";
            }

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados de Requisição

        private void ObterConfiguracaoIntegracaoFS()
        {
            if (_configuracaoIntegracaoFS != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFS repositorioConfiguracaoIntegracaoFS = new Repositorio.Embarcador.Configuracoes.IntegracaoFS(_unitOfWork);
            _configuracaoIntegracaoFS = repositorioConfiguracaoIntegracaoFS.BuscarPrimeiroRegistro();

            if ((_configuracaoIntegracaoFS == null) || !_configuracaoIntegracaoFS.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a FS.");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoFS.URLAutenticacao) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoFS.URLIntegracao))
                throw new ServicoException("O URL Autenticação e URL Integração devem estar preenchidos na configuração de integração da FS.");
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicaoFS, string urlIntegracao)
        {
            string token = ObterToken();
            HttpClient client = CriarRequisicao(token);

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicaoFS, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(_configuracaoIntegracaoFS.URLIntegracao + urlIntegracao, content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = result.IsSuccessStatusCode,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        private string ObterToken()
        {
            ObterConfiguracaoIntegracaoFS();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoFS));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _configuracaoIntegracaoFS.ClientID },
                { "client_secret", _configuracaoIntegracaoFS.ClientSecret },
                { "grant_type", "client_credentials" }
            });

            HttpResponseMessage result = client.PostAsync(_configuracaoIntegracaoFS.URLAutenticacao, content).Result;

            if (!result.IsSuccessStatusCode)
                throw new ServicoException("Não foi possível obter o Token");

            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            RetornoToken retorno = JsonConvert.DeserializeObject<RetornoToken>(jsonResponse);

            return retorno.Token;
        }

        private HttpClient CriarRequisicao(string accessToken = null)
        {
            ObterConfiguracaoIntegracaoFS();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoFS));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Requisicao.InputRequisicao ObterDadosCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repositorioCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioProdutoEmbarcador = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = carga.Pedidos.FirstOrDefault()?.Pedido;

            if (primeiroPedido == null)
                throw new ServicoException("A carga não possui um pedido.");

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = repositorioProdutoEmbarcador.BuscarProdutosPorPedidos(new List<int> { primeiroPedido.Codigo });
            List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes = repositorioCargaComposicaoFrete.BuscarPorCarga(carga.Codigo, composicaoFreteFilialEmissora: false);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Requisicao.InputRequisicao
            {
                Carga = new Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Requisicao.Carga
                {
                    TipoOperacao = carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco ?? string.Empty,
                    TipoCarga = carga.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? string.Empty,
                    DataCarregamento = carga.DataCarregamentoCarga?.ToString("MMddyyyyHHmm") ?? string.Empty,
                    CodigoContrato = primeiroPedido.NumeroOrdem ?? string.Empty,
                    Origem = primeiroPedido.Remetente.CPF_CNPJ_SemFormato ?? string.Empty,
                    Destinatario = primeiroPedido.Destinatario.CPF_CNPJ_SemFormato ?? string.Empty,
                    DestinoEntrega = primeiroPedido.Destino.Descricao ?? string.Empty,
                    CodigoProduto = produtos?.FirstOrDefault()?.CodigoProdutoEmbarcador ?? string.Empty,
                    Volume = primeiroPedido.QtVolumes.ToString() ?? string.Empty,
                    Transportadora = carga.Empresa?.CNPJ_SemFormato ?? string.Empty,
                    PlacaCavalo = carga.Veiculo?.Placa ?? string.Empty,
                    PlacaCarreta = carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                    Placa3 = carga.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa ?? string.Empty,
                    Motorista = carga.Motoristas.FirstOrDefault()?.CPF ?? string.Empty,
                    UfPlaca = carga.Veiculo?.Estado.Descricao ?? string.Empty,
                    Tarifa = cargaComposicaoFretes.FirstOrDefault()?.Valor.ToString("F6", CultureInfo.InvariantCulture) ?? string.Empty,
                    ProtocoloCarga = carga.Protocolo.ToString(),
                    ProtocoloPedido = primeiroPedido.Protocolo.ToString() ?? string.Empty,
                    NumeroCargaEtanol = carga.CodigoCargaEmbarcador,
                    CnpjEce = primeiroPedido.CodigoPedidoCliente ?? string.Empty,
                    DescricaoFilial = carga.Filial?.Descricao ?? string.Empty,
                    Ibge = primeiroPedido.Origem?.CodigoIBGE.ToString() ?? string.Empty,
                    Uf = primeiroPedido.Origem?.Estado.Sigla ?? string.Empty,
                    CNPJDescarga = primeiroPedido.EnderecoDestino?.ClienteOutroEndereco?.CodigoEmbarcador ?? string.Empty,
                }
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao.InputRequisicao ObterDadosCancelamentoCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao.InputRequisicao
            {
                CancelamentoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCancelamentoCarga.Requisicao.CancelamentoCarga
                {
                    Motivo = cargaCancelamento.MotivoCancelamento,
                    Protocolo = cargaCancelamento.Carga.Protocolo.ToString(),
                }
            };
        }

        #endregion Métodos Privados de Requisição
    }
}
