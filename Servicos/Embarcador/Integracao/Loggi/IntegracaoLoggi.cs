using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Loggi
{
    public class IntegracaoLoggi
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion Atributo

        #region Construtores

        public IntegracaoLoggi(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaCargaIntegracao.NumeroTentativas++;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                bool atualizar = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.Protocolo);

                Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracao = ObterConfiguracaoIntegracao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = repositorioCargaPedidoPacote.BuscarCargaPedidoPacotePorCarga(cargaCargaIntegracao.Carga.Codigo);

                if (cargaPedidoPacotes.Count > 0)
                {
                    foreach (var cargaPedidoPacote in cargaPedidoPacotes)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.RequestIntegracao dadosRequisicao = PreencherRequisicao(cargaPedidoPacote, atualizar);

                        HttpClient requisicao = CriarRequisicao(configuracaoIntegracao, cargaCargaIntegracao.Protocolo, atualizar);
                        jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                        StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                        HttpResponseMessage retornoRequisicao = requisicao.PutAsync(configuracaoIntegracao.UrlIntegracaoCTe, conteudoRequisicao).Result;
                        jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(jsonRetorno);

                        if (!retornoRequisicao.IsSuccessStatusCode)
                            throw new ServicoException(resposta.Detail);

                        servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integrado com sucesso");
                    }
                }

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";

            }
            catch (ServicoException excecao)
            {
                cargaCargaIntegracao.ProblemaIntegracao = excecao.Message;
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public string ObterToken(string urlToken, string clienteId, string clienteSecret, string scope)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(urlToken);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clienteId);
            request.AddParameter("client_secret", clienteSecret);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", scope);

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            return retorno.access_token;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracao = ObterConfiguracaoIntegracao();

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao, string.Empty, false);
                StringContent conteudoRequisicao = new StringContent(httpRequisicaoResposta.conteudoRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLIntegracao, conteudoRequisicao).Result;
                httpRequisicaoResposta.conteudoResposta = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(httpRequisicaoResposta.conteudoResposta);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);

                    httpRequisicaoResposta.sucesso = true;
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso";
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException(resposta.Detail);
                else
                    throw new ServicoException($"Problema ao integrar com Loggi (Status: {retornoRequisicao.StatusCode})");
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com Loggi.";
            }

            return httpRequisicaoResposta;
        }

        public void ConsultarPacotesCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes cargaPedidoIntegracaoPacote, bool processoViaThread = false)
        {
            Repositorio.Embarcador.Cargas.Pacote repositorioPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repositorioIntegracaoToken = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes repositorioCargaPedidoIntegracaoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracaoLoggi = repositorioIntegracaoToken.Buscar();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidoIntegracaoPacote.CargaPedido;

            Servicos.Embarcador.Pacote.Pacote servicoPacote = new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            cargaPedidoIntegracaoPacote.DataIntegracao = DateTime.Now;
            cargaPedidoIntegracaoPacote.NumeroTentativas++;
            string request = "";
            string response = "";
            string erroIntegracao = "";

            try
            {
                HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggi));

                requisicao.BaseAddress = new Uri(configuracaoIntegracaoLoggi.URLIntegracao);
                requisicao.DefaultRequestHeaders.Accept.Clear();
                requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requisicao.DefaultRequestHeaders.Add("Authorization", $"Bearer {ObterToken(configuracaoIntegracaoLoggi.URLIntegracao, configuracaoIntegracaoLoggi.ClientID, configuracaoIntegracaoLoggi.ClientSecret, configuracaoIntegracaoLoggi.Scope)}");

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCargaEtapaETipo(cargaPedido.Carga.Codigo, TipoFluxoGestaoPatio.Origem, EtapaFluxoGestaoPatio.FimCarregamento);

                DateTime dataCarregamento = fluxoGestaoPatio?.DataFimCarregamento ?? DateTime.Now;

                string destino = (cargaPedido.Recebedor != null && cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Cliente?.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ &&
                    (cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false)) ? cargaPedido.Pedido.EnderecoDestino.ClienteOutroEndereco.CodigoEmbarcador : cargaPedido.Pedido.Destinatario.CodigoIntegracao;

                string objeto = $"?origem={cargaPedido.Pedido.Remetente.CodigoIntegracao}&destino={destino}&data_carregamento={dataCarregamento.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'")}&placa={(cargaPedido.Carga.Veiculo?.Placa ?? string.Empty).ToUpper()}";
                HttpResponseMessage respostaMensagem = requisicao.GetAsync(configuracaoIntegracaoLoggi.URLConsultaPacotes + objeto).Result;
                request = configuracaoIntegracaoLoggi.URLConsultaPacotes + objeto;
                response = respostaMensagem.Content.ReadAsStringAsync().Result;

                dynamic resposta = JsonConvert.DeserializeObject<dynamic>(respostaMensagem.Content.ReadAsStringAsync().Result);

                List<string> codigosPacotes = resposta.lista_pacotes != null ? resposta.lista_pacotes.ToObject<List<string>>() : new List<string>() { };

                if (cargaPedido.Carga.DadosSumarizados.CodigoIntegracaoRemetentes == null)
                    erroIntegracao = "Código de Integração do Remetente deve ser preenchido.";

                if (cargaPedido.Carga.DadosSumarizados.CodigoIntegracaoDestinatarios == null)
                    erroIntegracao = "Código de Integração do Destinatário deve ser preenchido.";

                if (cargaPedido.Carga.Veiculo?.Placa == null)
                    erroIntegracao = "Veículo deve ser preenchido.";

                if (codigosPacotes.Count == 0)
                    erroIntegracao = "Não foram localizados pacotes para essa transferência. Favor verificar internamente.";

                if (!respostaMensagem.IsSuccessStatusCode && respostaMensagem.StatusCode == HttpStatusCode.InternalServerError)
                    erroIntegracao = "WebService de consulta de pacotes indisponível. Acionar TI da Loggi para verificação.";

                if (!string.IsNullOrWhiteSpace(erroIntegracao) && !processoViaThread)
                    throw new ServicoException(erroIntegracao);

                if (!string.IsNullOrWhiteSpace(erroIntegracao) && processoViaThread)
                {
                    cargaPedidoIntegracaoPacote.ProblemaIntegracao = erroIntegracao;
                    cargaPedidoIntegracaoPacote.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(cargaPedidoIntegracaoPacote, request, response, "json", erroIntegracao);

                    repositorioCargaPedidoIntegracaoPacote.Atualizar(cargaPedidoIntegracaoPacote);
                }
                else
                {

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = cargaPedido == null ? new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>() : repositorioCargaPedidoPacote.BuscarCargaPedidoPacoteLoggiKey(codigosPacotes, cargaPedido);
                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros = repCTeTerceiro.BuscarPorIdentificacaoPacote(codigosPacotes);
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCTeSubContratacaoECargaPedido(ctesTerceiros, cargaPedido);

                    bool validarCTe = true;

                    _unitOfWork.Start();

                    for (int i = 0; i < codigosPacotes.Count; i++)
                    {
                        string codigoPacote = codigosPacotes[i];

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote = cargaPedidoPacotes.Where(o => o.Pacote.LogKey == codigoPacote).FirstOrDefault();

                        if (cargaPedidoPacote == null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Pacote pacote = new Dominio.Entidades.Embarcador.Cargas.Pacote
                            {
                                DataRecebimento = DateTime.Now,
                                LogKey = codigoPacote,
                                Destino = cargaPedido.ObterDestinatario(),
                                Origem = cargaPedido.Pedido.Remetente,
                                Contratante = cargaPedido.Pedido.Tomador != null ? cargaPedido.Pedido.Tomador : cargaPedido.ObterDestinatario(),
                                Peso = 0,
                                Cubagem = 0
                            };

                            repositorioPacote.Inserir(pacote);

                            cargaPedidoPacote = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote
                            {
                                CargaPedido = cargaPedido,
                                Pacote = pacote
                            };

                            repositorioCargaPedidoPacote.Inserir(cargaPedidoPacote);
                        }
                        else
                        {
                            if (cargaPedidoPacote.CargaPedido == null)
                                cargaPedidoPacote.CargaPedido = cargaPedido;
                        }

                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceirosExistentes = ctesTerceiros.Where(o => o.IdentifacaoPacote == codigoPacote).ToList();
                        foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceirosExistentes)
                        {
                            string retorno = new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware).VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro, pedidoCTesParaSubContratacao, configuracao, validarCTe).GetAwaiter().GetResult();

                            validarCTe = false;

                            if (!string.IsNullOrWhiteSpace(retorno))
                                continue;
                        }
                    }

                    servicoPacote.VerificarQuantidadePacotesCtesAvancaAutomaticoAsync(cargaPedido.Carga, _auditado).GetAwaiter().GetResult();

                    cargaPedidoIntegracaoPacote.ProblemaIntegracao = "";
                    cargaPedidoIntegracaoPacote.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                    repositorioCargaPedidoIntegracaoPacote.Atualizar(cargaPedidoIntegracaoPacote);

                    servicoArquivoTransacao.Adicionar(cargaPedidoIntegracaoPacote, request, response, "json", $"Consulta realizada com sucesso com o pedido {cargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty}.");
                }

                _unitOfWork.CommitChanges();
            }
            catch (ServicoException ex)
            {
                _unitOfWork.Rollback();

                cargaPedidoIntegracaoPacote.ProblemaIntegracao = ex.Message;
                cargaPedidoIntegracaoPacote.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaPedidoIntegracaoPacote, request, response, "json", ex.Message);

                repositorioCargaPedidoIntegracaoPacote.Atualizar(cargaPedidoIntegracaoPacote);
                throw new ServicoException(ex.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);
                servicoArquivoTransacao.Adicionar(cargaPedidoIntegracaoPacote, request, response, "json");
                cargaPedidoIntegracaoPacote.ProblemaIntegracao = "Falha ao Consultar Pacotes.";
                cargaPedidoIntegracaoPacote.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaPedidoIntegracaoPacote.Atualizar(cargaPedidoIntegracaoPacote);
                throw;
            }
        }

        public void GerarRegistroIntegracoesCargaPedidoPacote(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Loggi);

            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes repositorioCargaPedidoIntegracaoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(codigoCarga);

            foreach (var cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes cargaPedidoIntegracaoPacote = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes();
                cargaPedidoIntegracaoPacote.DataIntegracao = DateTime.Now;
                cargaPedidoIntegracaoPacote.NumeroTentativas = 0;
                cargaPedidoIntegracaoPacote.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaPedidoIntegracaoPacote.ProblemaIntegracao = string.Empty;
                cargaPedidoIntegracaoPacote.TipoIntegracao = tipoIntegracao;
                cargaPedidoIntegracaoPacote.CargaPedido = cargaPedido;

                repositorioCargaPedidoIntegracaoPacote.Inserir(cargaPedidoIntegracaoPacote);
            }
        }

        public void ProcessarIntegracoesPendentes(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes repositorioCargaPedidoIntegracaoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> integracoesPendentes = repositorioCargaPedidoIntegracaoPacote.BuscarPorCargaPedidoIntegracaoPacotes(codigoCarga);

            foreach (var integracaoPendente in integracoesPendentes)
                ConsultarPacotesCarga(integracaoPendente);
        }

        public void IntegrarCargaCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;

            try
            {
                IntegrarCTe(cargaCTeIntegracao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Loggi";
            }
        }

        public void IntegrarCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);

            cargaCTeManualIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeManualIntegracao.NumeroTentativas++;

            try
            {
                IntegrarCTeManual(cargaCTeManualIntegracao);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Loggi";
            }

            repCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
        }

        public void IntegrarOcorrenciaCTe(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracao.NumeroTentativas++;

            try
            {
                IntegrarCTeOcorrencias(ocorrenciaCTeIntegracao);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Loggi";
            }
        }

        public void IntegrarCargaValoresCTeLoggi(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;

            try
            {
                IntegrarValoresCTe(cargaCTeIntegracao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Loggi";
            }
        }

        public void IntegrarCargaCTeAnterioresLoggi(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi configuracaoIntegracao = ObterConfiguracaoIntegracaoCTeAnterioresLoggi();
                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggi corpoRequisicao = PreencherCorpoRequisicaoCTeAnteriores(integracaoPendente);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PutAsync(configuracaoIntegracao.URLEnvioDocumentos, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado CT-e Anteriores.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(jsonRetorno);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.ProblemaIntegracao = retornoIntegracao.Detail ?? "Integração realizada com sucesso!";
                }
                else if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException(retornoIntegracao.Detail ?? "Problema ao realizar integração");
                else
                    throw new ServicoException("Retorno de status não tratado, verificar a comunicação com a Loggi CT-e Anteriores!");
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoLoggi");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Loggi CT-e Anteriores.";
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracao, string protocolo, bool atualizar)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggi));

            requisicao.BaseAddress = atualizar ? new Uri(configuracaoIntegracao.URLIntegracao + $"/{protocolo.ToInt()}") : new Uri(configuracaoIntegracao.UrlIntegracaoCTe);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + ObterToken(configuracaoIntegracao.URLIntegracao, configuracaoIntegracao.ClientID, configuracaoIntegracao.ClientSecret, configuracaoIntegracao.Scope));

            return requisicao;
        }

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggi));

            string token = ObterToken(configuracaoIntegracao.URLAutenticacao, configuracaoIntegracao.ClientID, configuracaoIntegracao.ClientSecret, configuracaoIntegracao.Scope);

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLEnvioDocumentos);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return requisicao;
        }

        private bool IsRetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repositorioIntegracaoToken = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracaoLoggi = repositorioIntegracaoToken.Buscar();

            if ((configuracaoIntegracaoLoggi == null) || !configuracaoIntegracaoLoggi.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLoggi.URLIntegracao))
                throw new ServicoException("A URL deve estar preenchido corretamente na configuração de integração da Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLoggi.ClientID))
                throw new ServicoException("Cliente ID deve estar preenchidos corretamente na configuração de integração da Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLoggi.ClientSecret))
                throw new ServicoException("Cliente Secret deve estar preenchidos corretamente na configuração de integração da Loggi.");

            return configuracaoIntegracaoLoggi;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi ObterConfiguracaoIntegracaoCTePagamento()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi repositorioIntegracaoToken = new Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi configuracaoIntegracaoCTePagamentoLoggi = repositorioIntegracaoToken.Buscar();

            if ((configuracaoIntegracaoCTePagamentoLoggi == null) || !configuracaoIntegracaoCTePagamentoLoggi.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para CT-e Pagamento Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoCTePagamentoLoggi.URLAutenticacao))
                throw new ServicoException("A URL de autenticação deve estar preenchido corretamente na configuração de integração de CT-e Pagamento Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoCTePagamentoLoggi.ClientID))
                throw new ServicoException("Cliente ID deve estar preenchidos corretamente na configuração de integração de CT-e Pagamento Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoCTePagamentoLoggi.ClientSecret))
                throw new ServicoException("Cliente Secret deve estar preenchidos corretamente na configuração de integração de CT-e Pagamento Loggi.");

            return configuracaoIntegracaoCTePagamentoLoggi;
        }


        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi ObterConfiguracaoIntegracaoValoresCTeLoggi()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi repIntegracaoValoresCTeLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi configuracaoIntegracaoValoresCTeLoggi = repIntegracaoValoresCTeLoggi.Buscar();

            if ((configuracaoIntegracaoValoresCTeLoggi == null) || !configuracaoIntegracaoValoresCTeLoggi.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para CT-e Pagamento Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoValoresCTeLoggi.URLAutenticacao))
                throw new ServicoException("A URL de autenticação deve estar preenchido corretamente na configuração de integração de CT-e Pagamento Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoValoresCTeLoggi.ClientID))
                throw new ServicoException("Cliente ID deve estar preenchidos corretamente na configuração de integração de CT-e Pagamento Loggi.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoValoresCTeLoggi.ClientSecret))
                throw new ServicoException("Cliente Secret deve estar preenchidos corretamente na configuração de integração de CT-e Pagamento Loggi.");

            return configuracaoIntegracaoValoresCTeLoggi;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi ObterConfiguracaoIntegracaoCTeAnterioresLoggi()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi repositorioIntegracaoCTeAnterioresLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi configuracaoIntegracaoCTeAnterioresLoggi = repositorioIntegracaoCTeAnterioresLoggi.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoCTeAnterioresLoggi == null || !configuracaoIntegracaoCTeAnterioresLoggi.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para CT-e Anteriores Loggi.");

            return configuracaoIntegracaoCTeAnterioresLoggi;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.RequestIntegracao PreencherRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote, bool atualizar)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.RequestIntegracao request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.RequestIntegracao();

            request.LogKeyPacote = cargaPedidoPacote.Pacote.LogKey;
            request.ProtocoloCarga = cargaPedidoPacote.CargaPedido?.Carga?.Protocolo ?? 0;
            request.ProtocoloPedido = cargaPedidoPacote.CargaPedido?.Carga?.Pedidos.Select(o => o.Pedido.Protocolo).FirstOrDefault().ToString();
            request.CodigoIntegracaoTipoOperacao = cargaPedidoPacote.CargaPedido?.Carga?.TipoOperacao?.CodigoIntegracao ?? string.Empty;

            return request;
        }

        private void IntegrarCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe dadosRequisicao = ObterDadosIntegracaoCTe(cargaCTeIntegracao.CargaCTe);
            (HttpResponseMessage Requisicao, string jsonRequest, string jsonResponse) respostaRequisicao = EnviarIntegracaoCTe(dadosRequisicao);

            AdicionarArquivoTransacao(cargaCTeIntegracao, respostaRequisicao.jsonRequest, respostaRequisicao.jsonResponse);

            if (!respostaRequisicao.Requisicao.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(respostaRequisicao.jsonResponse);
                throw new ServicoException(resposta.Detail);
            }
        }

        private void IntegrarCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe dadosRequisicao = ObterDadosIntegracaoCTeManual(cargaCTeManualIntegracao);
            (HttpResponseMessage Requisicao, string jsonRequest, string jsonResponse) respostaRequisicao = EnviarIntegracaoCTe(dadosRequisicao);

            AdicionarArquivoCteManualTransacao(cargaCTeManualIntegracao, respostaRequisicao.jsonRequest, respostaRequisicao.jsonResponse);

            if (!respostaRequisicao.Requisicao.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(respostaRequisicao.jsonResponse);
                throw new ServicoException(resposta.Detail);
            }
        }

        private void IntegrarCTeOcorrencias(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe dadosRequisicao = ObterDadosIntegracaoCTe(ocorrenciaCTeIntegracao.CargaCTe);
            (HttpResponseMessage Requisicao, string jsonRequest, string jsonResponse) respostaRequisicao = EnviarIntegracaoCTe(dadosRequisicao);

            AdicionarArquivoTransacaoOcorrencias(ocorrenciaCTeIntegracao, respostaRequisicao.jsonRequest, respostaRequisicao.jsonResponse);

            if (!respostaRequisicao.Requisicao.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(respostaRequisicao.jsonResponse);
                throw new ServicoException(resposta.Detail);
            }
        }

        private (HttpResponseMessage Requisicao, string jsonRequest, string jsonResponse) EnviarIntegracaoCTe(Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe dadosRequisicao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi configuracaoIntegracao = ObterConfiguracaoIntegracaoCTePagamento();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEnvioDocumentos))
                throw new ServicoException("A URL para envio dos documentos não está configurada nas configurações de integração da Loggi.");

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None);

            string token = ObterToken(configuracaoIntegracao.URLAutenticacao, configuracaoIntegracao.ClientID, configuracaoIntegracao.ClientSecret, configuracaoIntegracao.Scope);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggi));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLEnvioDocumentos);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            StringContent conteudoRequisicao = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PutAsync(configuracaoIntegracao.URLEnvioDocumentos, conteudoRequisicao).Result;
            jsonResponse = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                throw new ServicoException("Erro interno de servidor.");

            return (retornoRequisicao, jsonRequest, jsonResponse);
        }

        private void IntegrarValoresCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi configuracaoIntegracao = ObterConfiguracaoIntegracaoValoresCTeLoggi();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEnvioDocumentos))
                throw new ServicoException("A URL para envio dos documentos não está configurada nas configurações de integração da Loggi.");

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ValoresCTeLoggi dadosRequisicao = ObterValoresCTe(cargaCTeIntegracao.CargaCTe);

            jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None);

            string token = ObterToken(configuracaoIntegracao.URLAutenticacao, configuracaoIntegracao.ClientID, configuracaoIntegracao.ClientSecret, configuracaoIntegracao.Scope);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggi));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLEnvioDocumentos);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            StringContent conteudoRequisicao = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PutAsync(configuracaoIntegracao.URLEnvioDocumentos, conteudoRequisicao).Result;
            jsonResponse = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                throw new ServicoException("Erro interno de servidor.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ResponseLoggi>(jsonResponse);

            AdicionarArquivoTransacao(cargaCTeIntegracao, jsonRequest, jsonResponse);

            if (!retornoRequisicao.IsSuccessStatusCode)
                throw new ServicoException(resposta.Detail);

        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe ObterDadosIntegracaoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTe.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi integracaoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(_unitOfWork).Buscar();

            var integracaoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe()
            {
                NumeroCTe = cte.Numero,
                SerieCTe = cte.Serie?.Numero ?? 0,
                DataEmissao = cte.DataEmissao?.ToString("dd.MM.yyyy") ?? string.Empty,
                CodigoCFOP = cte.CFOP?.CodigoCFOP ?? 0,
                ValorTotal = cte.ValorAReceber,
                AliquotaICMS = cte.AliquotaICMS,
                BaseCalculoICMS = cte.BaseCalculoICMS,
                ValorICMS = cte.ValorICMS,
                CNPJEmissor = cte.Empresa?.CNPJ_SemFormato ?? string.Empty,
                NomeEmitente = cte.Empresa?.NomeFantasia ?? string.Empty,
                CPFCNPJTomador = cte.Tomador?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeTomador = cte.Tomador?.Nome ?? string.Empty,
                CPFCNPJRemetente = cte.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeRemetente = cte.Remetente?.Nome ?? string.Empty,
                CPFCNPJDestinatario = cte.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeDestinatario = cte.Destinatario?.Nome ?? string.Empty,
                CPFCNPJExpedidor = cte.Expedidor?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeExpedidor = cte.Expedidor?.Nome ?? string.Empty,
                CPFCNPJRecebedor = cte.Recebedor?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeRecebedor = cte.Recebedor?.Nome ?? string.Empty,
                ChaveCTe = cte.Chave,
                TomadorCTe = (int)cte.TipoTomador,
                TipoDocumento = cte.TipoEmissao.ObterSomenteNumeros().ToInt(),
                RegiaoEmissorDocumento = cte.Empresa?.Localidade?.Estado?.CodigoIBGE ?? 0,
                UFInicioPrestacao = cte.LocalidadeInicioPrestacao?.Estado?.Sigla ?? string.Empty,
                CodigoDomicilioOrigem = cte.LocalidadeInicioPrestacao?.CodigoIBGE.ToString() ?? string.Empty,
                NomeDomicilioOrigem = cte.LocalidadeInicioPrestacao?.Descricao ?? string.Empty,
                UFFimPrestacao = cte.LocalidadeTerminoPrestacao?.Estado?.Sigla ?? string.Empty,
                CodigoDomicilioDestino = cte.LocalidadeTerminoPrestacao?.CodigoIBGE.ToString() ?? string.Empty,
                NomeDomicilioDestino = cte.LocalidadeTerminoPrestacao?.Descricao ?? string.Empty,
                UFEmitente = cte.Empresa?.Localidade?.Estado?.Sigla ?? string.Empty,
                ModoTransporte = !string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal?.Numero ?? string.Empty) ? cte.ModeloDocumentoFiscal.Numero.ObterSomenteNumeros().ToInt() : 0,
                StatusCTeSefaz = cte.MensagemStatus?.MensagemDoErro ?? string.Empty,
                CentroCusto = carga.TipoOperacao?.ConfiguracaoPagamentos?.CentroResultado?.Plano.ObterSomenteNumeros().ToInt() ?? 0,
            };

            return integracaoCTe;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe ObterDadosIntegracaoCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTe)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTe.Carga;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;

            var integracaoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.IntegracaoCTe()
            {
                NumeroCTe = cte.Numero,
                SerieCTe = cte.Serie?.Numero ?? 0,
                DataEmissao = cte.DataEmissao?.ToString("dd.MM.yyyy") ?? string.Empty,
                CodigoCFOP = cte.CFOP?.CodigoCFOP ?? 0,
                ValorTotal = cte.ValorAReceber,
                AliquotaICMS = cte.AliquotaICMS,
                BaseCalculoICMS = cte.BaseCalculoICMS,
                ValorICMS = cte.ValorICMS,
                CNPJEmissor = cte.Empresa?.CNPJ_SemFormato ?? string.Empty,
                NomeEmitente = cte.Empresa?.NomeFantasia ?? string.Empty,
                CPFCNPJTomador = cte.Tomador?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeTomador = cte.Tomador?.Nome ?? string.Empty,
                CPFCNPJRemetente = cte.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeRemetente = cte.Remetente?.Nome ?? string.Empty,
                CPFCNPJDestinatario = cte.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeDestinatario = cte.Destinatario?.Nome ?? string.Empty,
                CPFCNPJExpedidor = cte.Expedidor?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeExpedidor = cte.Expedidor?.Nome ?? string.Empty,
                CPFCNPJRecebedor = cte.Recebedor?.CPF_CNPJ_SemFormato ?? string.Empty,
                NomeRecebedor = cte.Recebedor?.Nome ?? string.Empty,
                ChaveCTe = cte.Chave ?? string.Empty,
                TomadorCTe = (int)cte.TipoTomador,
                TipoDocumento = cte.TipoEmissao.ObterSomenteNumeros().ToInt(),
                RegiaoEmissorDocumento = cte.Empresa?.Localidade?.Estado?.CodigoIBGE ?? 0,
                UFInicioPrestacao = cte.LocalidadeInicioPrestacao?.Estado?.Sigla ?? string.Empty,
                CodigoDomicilioOrigem = cte.LocalidadeInicioPrestacao?.CodigoIBGE.ToString() ?? string.Empty,
                NomeDomicilioOrigem = cte.LocalidadeInicioPrestacao?.Descricao ?? string.Empty,
                UFFimPrestacao = cte.LocalidadeTerminoPrestacao?.Estado?.Sigla ?? string.Empty,
                CodigoDomicilioDestino = cte.LocalidadeTerminoPrestacao?.CodigoIBGE.ToString() ?? string.Empty,
                NomeDomicilioDestino = cte.LocalidadeTerminoPrestacao?.Descricao ?? string.Empty,
                UFEmitente = cte.Empresa?.Localidade?.Estado?.Sigla ?? string.Empty,
                ModoTransporte = !string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal?.Numero ?? string.Empty) ? cte.ModeloDocumentoFiscal.Numero.ObterSomenteNumeros().ToInt() : 0,
                StatusCTeSefaz = cte.MensagemStatus?.MensagemDoErro ?? string.Empty,
                CentroCusto = carga.TipoOperacao?.ConfiguracaoPagamentos?.CentroResultado?.Plano.ObterSomenteNumeros().ToInt() ?? 0,
            };

            return integracaoCTe;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ValoresCTeLoggi ObterValoresCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.XMLCTe repositorioXMLCTe = new Repositorio.XMLCTe(_unitOfWork);
            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
            Dominio.Entidades.XMLCTe xmlCTE = repositorioXMLCTe.BuscarPorCTe(cargaCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

            float Gris = (float)cargaCTe.CTe.ComponentesPrestacao.Where(x => x.NomeCTe == "GRIS").Sum(x => x.Valor);
            float Advalorem = (float)cargaCTe.CTe.ComponentesPrestacao.Where(x => x.NomeCTe == "AD VALOREM").Sum(x => x.Valor);
            string XML = "";

            if (!xmlCTE.XMLArmazenadoEmArquivo)
                XML = xmlCTE.XML;
            else
            {
                string caminho = servicoCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCTE.CTe, "A", _unitOfWork);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(caminho);
                string stringXMLCTe = doc.InnerXml;
                stringXMLCTe = stringXMLCTe.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");
                XML = stringXMLCTe;
            }



            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(XML);
            string XMLBase64 = Convert.ToBase64String(bytes);


            var integracaoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.ValoresCTeLoggi()
            {
                valor_total = (float)cte.ValorAReceber,
                pis = (float)cte.ValorPIS,
                cofins = (float)cte.ValorCOFINS,
                gris = Gris,
                advalorem = Advalorem,
                icms = (float)cte.ValorICMS,
                data_emissao = cte.DataEmissao?.ToString("dd.MM.yyyy") ?? string.Empty,
                cnpj_emissor = cte.Empresa?.CNPJ_SemFormato ?? string.Empty,
                chave_cte = cte.Chave,
                xml = XMLBase64
            };
            return integracaoCTe;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggi PreencherCorpoRequisicaoCTeAnteriores(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.CTe.CTeTerceiroXML repositorioCTeTerceiroXML = new Repositorio.Embarcador.CTe.CTeTerceiroXML(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete repositorioCteTerceiroComponenteFrete = new Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggiDados> listaDadosCteAnterioresLoggi = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggiDados>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCteParaSubContratacao = repositorioPedidoCTeParaSubContratacao.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);

            List<int> codigosPedidosCteTerceirosAnteriores = pedidosCteParaSubContratacao.Select(pedidoCteParaSubContratacao => pedidoCteParaSubContratacao.CTeTerceiro.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> ctesTerceiroComponentesFretes = repositorioCteTerceiroComponenteFrete.BuscarPorCodigosCTeParaSubContratacao(codigosPedidosCteTerceirosAnteriores);
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML> xmlsCTEsTerceiros = repositorioCTeTerceiroXML.BuscarPorCodigosCTesTerceiros(codigosPedidosCteTerceirosAnteriores);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCteParaSubContratacao in pedidosCteParaSubContratacao)
            {
                decimal Gris = ctesTerceiroComponentesFretes.Where(cteTerceiroComponenteFrete => cteTerceiroComponenteFrete.Codigo == pedidoCteParaSubContratacao.CTeTerceiro.Codigo && cteTerceiroComponenteFrete.Descricao == "GRIS").Sum(x => x.Valor);
                decimal Advalorem = ctesTerceiroComponentesFretes.Where(cteTerceiroComponenteFrete => cteTerceiroComponenteFrete.Codigo == pedidoCteParaSubContratacao.CTeTerceiro.Codigo && cteTerceiroComponenteFrete.Descricao == "AD VALOREM").Sum(x => x.Valor);
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML xmlCTETerceiro = xmlsCTEsTerceiros.Find(xmlCteTerceiro => xmlCteTerceiro.CTeTerceiro.Codigo == pedidoCteParaSubContratacao.CTeTerceiro.Codigo);

                string XMLBase64 = string.Empty;

                if (xmlCTETerceiro != null)
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xmlCTETerceiro.XML);
                    XMLBase64 = Convert.ToBase64String(bytes);
                }

                listaDadosCteAnterioresLoggi.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggiDados()
                {
                    ValorTotal = pedidoCteParaSubContratacao.CTeTerceiro.ValorAReceber,
                    Pis = pedidoCteParaSubContratacao.ValorPis,
                    Cofins = pedidoCteParaSubContratacao.ValorCofins,
                    Gris = Gris,
                    Advalorem = Advalorem,
                    Icms = pedidoCteParaSubContratacao.ValorICMS,
                    DataEmissao = pedidoCteParaSubContratacao.CTeTerceiro.DataEmissao.ToString("dd.MM.yyyy") ?? string.Empty,
                    CnpjEmissor = pedidoCteParaSubContratacao.CTeTerceiro.TransportadorTerceiro?.CPF_CNPJ_SemFormato ?? string.Empty,
                    ChaveCte = pedidoCteParaSubContratacao.CTeTerceiro.ChaveAcesso,
                    Xml = XMLBase64
                });
            }

            return ObterCTeAnterior(listaDadosCteAnterioresLoggi);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggi ObterCTeAnterior(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggiDados> listaDadosCteAnterioresLoggi)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi.CTeAnteriorLoggi
            {
                ListaCteAnterior = listaDadosCteAnterioresLoggi
            };
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = cargaCTeIntegracao.DataIntegracao,
                Mensagem = cargaCTeIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaCTeIntegracao.ArquivosTransacao == null)
                cargaCTeIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoCteManualTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = cargaCTeManualIntegracao.DataIntegracao,
                Mensagem = cargaCTeManualIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaCTeManualIntegracao.ArquivosTransacao == null)
                cargaCTeManualIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCTeManualIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacaoOcorrencias(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = ocorrenciaCTeIntegracao.DataIntegracao,
                Mensagem = ocorrenciaCTeIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioOcorrenciaCTeIntegracao.Inserir(arquivoIntegracao);

            if (ocorrenciaCTeIntegracao.ArquivosTransacao == null)
                ocorrenciaCTeIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

            ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion Métodos Privados
    }
}
