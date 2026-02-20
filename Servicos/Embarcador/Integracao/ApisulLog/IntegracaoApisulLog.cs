using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ApisulLog
{
    public class IntegracaoApisulLog
    {
        #region Propriedades Privados

        readonly private Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoApisulLog _configuracaoIntegracao;

        #endregion Propriedades Privados

        #region Construtores

        public IntegracaoApisulLog(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public async Task IntegrarSMP(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaCargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaCargaCargaIntegracao.NumeroTentativas++;
            integracaCargaCargaIntegracao.DataIntegracao = DateTime.Now;

            InspectorBehavior inspector = new InspectorBehavior(false);

            try
            {
                ObterConfiguracaoIntegracaoApisulLog();

                int.TryParse(_configuracaoIntegracao.Token, out int token);
                Servico.IntegracaoSMP.SMPModeloIntegracao objetoRequisicao = await ObterObjetoRequestIntegracaoApisul(integracaCargaCargaIntegracao.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao);

                Servico.IntegracaoSMP.SMPClient sMPClient = ObterClient<Servico.IntegracaoSMP.SMPClient>(_configuracaoIntegracao.URLIntegracaoApisulLog);
                sMPClient.Endpoint.EndpointBehaviors.Add(inspector);
                Servico.IntegracaoSMP.RetornoInsereSMP retorno = await sMPClient.InsereSMPAsync(token, objetoRequisicao);

                integracaCargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                integracaCargaCargaIntegracao.ProblemaIntegracao = "";
                integracaCargaCargaIntegracao.Protocolo = retorno?.NumeroSMP.ToString() ?? string.Empty;

                if (!retorno.TransacaoOk)
                {
                    int maxsubstring = 0;

                    foreach (var item in retorno.MensagensErro)
                        maxsubstring += item.Mensagem.Length;

                    if (maxsubstring > 299)
                        maxsubstring = 299;

                    integracaCargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaCargaCargaIntegracao.ProblemaIntegracao = string.Join(", ", retorno.MensagensErro.Select(m => m.Mensagem).ToList()).Substring(0, maxsubstring).Replace("'", "");
                }
                servicoArquivoTransacao.Adicionar(integracaCargaCargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException ex)
            {
                integracaCargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaCargaCargaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaCargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaCargaCargaIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracaCargaCargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            await repositorioCargaCargaIntegracao.AtualizarAsync(integracaCargaCargaIntegracao);
        }

        public async Task BuscarSMPAsync(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCargaIntegracao.NumeroTentativas++;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            InspectorBehavior inspector = new InspectorBehavior(false);

            try
            {
                ObterConfiguracaoIntegracaoApisulLog();

                int.TryParse(_configuracaoIntegracao.Token, out int token);
                Servico.IntegracaoSMP.BuscaSMPModeloIntegracao objetoRequisicao = ObterObjetoRequestBuscarSMP(cargaCargaIntegracao);

                Servico.IntegracaoSMP.SMPClient sMPClient = ObterClient<Servico.IntegracaoSMP.SMPClient>(_configuracaoIntegracao.URLIntegracaoApisulLog);
                sMPClient.Endpoint.EndpointBehaviors.Add(inspector);
                Servico.IntegracaoSMP.RetornoBuscaSMP retorno = sMPClient.BuscaSMP(token, objetoRequisicao);

                List<string> statusSucesso = new List<string>() { "Em Viagem", "Em Monitoramento", "Logística", "Encerrada", "Finalizada" };
                List<string> statusError = new List<string>() { "Cancelada", "Não Aprovada", "Recusada" };

                if (statusSucesso.Contains(retorno.SMP.Status))
                {
                    cargaCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso " + retorno.SMP.Status;
                    cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
                else if (statusError.Contains(retorno.SMP.Status))
                {
                    cargaCargaIntegracao.ProblemaIntegracao = "Problema ao integrar " + retorno.SMP.Status;
                    cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                    SetMotivoErro(token, cargaCargaIntegracao);
                }
                else
                {
                    cargaCargaIntegracao.ProblemaIntegracao = retorno.SMP.Status;
                    cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                }

                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException ex)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";

                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarCancelaSMP(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                ObterConfiguracaoIntegracaoApisulLog();

                int.TryParse(_configuracaoIntegracao.Token, out int token);
                Servico.IntegracaoSMP.CancelaSMPModeloIntegracao objetoRequisicao = ObterObjetoRequestCancelaSMP(integracaoPendente);

                xmlRequest = Utilidades.XML.Serializar(objetoRequisicao);

                Servico.IntegracaoSMP.SMPClient sMPClient = ObterClient<Servico.IntegracaoSMP.SMPClient>(_configuracaoIntegracao.URLIntegracaoApisulLog);
                sMPClient.Endpoint.EndpointBehaviors.Add(inspector);
                Servico.IntegracaoSMP.RetornoCancelaSMP retorno = sMPClient.CancelaSMP(token, objetoRequisicao);

                xmlResponse = Utilidades.XML.Serializar(retorno);

                if (retorno.TransacaoOk)
                {
                    if (retorno.MensagensErro.Select(m => m.Mensagem).ToList().Count > 0)
                    {
                        integracaoPendente.ProblemaIntegracao = string.Join(", ", retorno.MensagensErro.Select(m => m.Mensagem).ToList()).Substring(0, 299).Replace("'", "");
                        integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        integracaoPendente.ProblemaIntegracao = "SMP Cancelada com sucesso";
                        integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    }
                }
                else
                {
                    integracaoPendente.ProblemaIntegracao = string.Join(", ", retorno.MensagensErro.Select(m => m.Mensagem).ToList()).Substring(0, 299).Replace("'", "");
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                }

                servicoArquivoTransacao.Adicionar(integracaoPendente, xmlRequest, xmlResponse, "xml");
            }
            catch (ServicoException ex)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message.Substring(0, 299).Replace("'", "");
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracaoPendente, xmlRequest, xmlResponse, "xml");
            }

            repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);
        }

        public async Task IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            InspectorBehavior inspector = new InspectorBehavior(false);

            try
            {
                ObterConfiguracaoIntegracaoApisulLog();

                Servico.IntegracaoSMP.SMPModeloIntegracao conteudoRequisicao = await ObterObjetoRequestIntegracaoApisul(cargaDadosTransporteIntegracao.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova);

                Servico.IntegracaoSMP.SMPClient smpClient = ObterClient<Servico.IntegracaoSMP.SMPClient>(_configuracaoIntegracao.URLIntegracaoApisulLog);
                smpClient.Endpoint.EndpointBehaviors.Add(inspector);
                Servico.IntegracaoSMP.RetornoInsereSMP retorno = await smpClient.InsereSMPAsync(_configuracaoIntegracao.Token.ToInt(), conteudoRequisicao);

                if (!retorno.TransacaoOk)
                {
                    int maxsubstring = 0;

                    foreach (var item in retorno.MensagensErro)
                        maxsubstring += item.Mensagem.Length;

                    if (maxsubstring > 299)
                        maxsubstring = 299;

                    string mensagem = string.Join(", ", retorno.MensagensErro.Select(m => m.Mensagem).ToList()).Substring(0, maxsubstring).Replace("'", "");

                    throw new ServicoException(mensagem);
                }

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                cargaDadosTransporteIntegracao.Protocolo = retorno.NumeroSMP.ToString() ?? string.Empty;
            }
            catch (BaseException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoApisulLog");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            await repositorioCargaDadosTransporteIntegracao.AtualizarAsync(cargaDadosTransporteIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        public void ObterConfiguracaoIntegracaoApisulLog()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoApisulLog repositorioApisulLog = new Repositorio.Embarcador.Configuracoes.IntegracaoApisulLog(_unitOfWork);
            _configuracaoIntegracao = repositorioApisulLog.BuscarPrimeiroRegistro();

            if ((_configuracaoIntegracao == null) || !_configuracaoIntegracao.PossuiIntegracaoApisulLog)
                throw new ServicoException("Não existe configuração de integração disponível para a ApisulLog");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLIntegracaoApisulLog))
                throw new ServicoException("Não existe URL de integração para apisullog configurada.");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.Token))
                throw new ServicoException("Não existe token configurado para realizar a integração.");
        }

        private void SetMotivoErro(int token, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLIntegracaoApisulLogEventos))
                return;

            // chama BuscaEventoSMP para saber os motivos do problema 
            ServicoAPISULLOG.EventoClient eventoSMPClient = ObterClient<ServicoAPISULLOG.EventoClient>(_configuracaoIntegracao.URLIntegracaoApisulLogEventos);
            ServicoAPISULLOG.BuscaEventoSMPModeloIntegracao modeloIntegracao = ObterEventoIntegracaoSMP(int.Parse(cargaCargaIntegracao.Protocolo));
            ServicoAPISULLOG.RetornoBuscaEventos retornoBuscaEventos = eventoSMPClient.BuscaEventoSMP(token, modeloIntegracao);

            cargaCargaIntegracao.ProblemaIntegracao = "";

            foreach (ServicoAPISULLOG.RetornoMensagem item in retornoBuscaEventos.MensagensErro)
                cargaCargaIntegracao.ProblemaIntegracao += " " + item.Mensagem;

            cargaCargaIntegracao.ProblemaIntegracao = cargaCargaIntegracao.ProblemaIntegracao.Substring(0, 299);
        }

        private T ObterClient<T>(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = new TimeSpan(0, 20, 0),
                SendTimeout = new TimeSpan(0, 20, 0)
            };

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return (T)Activator.CreateInstance(typeof(T), binding, endpointAddress);
        }

        private short GetCoresApisullogs(string cor)
        {
            switch (cor.ToUpper())
            {
                case "AMARELO":
                    return 1;
                case "AZUL":
                    return 2;
                case "BEGE":
                    return 3;
                case "BORDO":
                    return 4;
                case "BRANCO":
                    return 5;
                case "BRANCA":
                    return 5;
                case "CINZA":
                    return 6;
                case "DOURADO":
                    return 7;
                case "LARANJA":
                    return 8;
                case "MARROM":
                    return 9;
                case "PRETO":
                    return 10;
                case "PURPURA":
                    return 11;
                case "ROSA":
                    return 12;
                case "VERDE":
                    return 13;
                case "VERMELHO":
                    return 14;
                case "VIOLETA":
                    return 15;
                case "GRENÁ":
                    return 16;
                case "GRENA":
                    return 16;
                case "PRATA":
                    return 17;
                case "ROXO":
                    return 18;
                case "ROXA":
                    return 18;
                default:
                    return 999;
            }
        }

        private async Task<Servico.IntegracaoSMP.SMPModeloIntegracao> ObterObjetoRequestIntegracaoApisul(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga etapaIntegracao)
        {
            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.CNPJEmbarcador))
                throw new ServicoException("CNPJ do embarcador não configurado.");

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repostiorioMdfe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Servico.IntegracaoSMP.SMPModeloIntegracao dadosRequisicao = new Servico.IntegracaoSMP.SMPModeloIntegracao();

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdf = repostiorioMdfe.BuscarPrimeiroPorCarga(carga.Codigo);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> cte = repositorioCargaCte.BuscarCTePorCarga(carga.Codigo);

            if (_configuracaoIntegracao.ConcatenarCodigoIntegracaoTransporteOridemEDestino)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPrimeiraPorCargaAsync(carga.Codigo);

                dadosRequisicao.NomeRota = $"{carga.Empresa?.CodigoIntegracao ?? string.Empty}{cargaPedido.ClienteColeta?.CodigoIntegracao ?? string.Empty}{cargaPedido.ClienteEntrega?.CodigoIntegracao ?? string.Empty}";
            }
            else
                dadosRequisicao.NomeRota = carga?.Rota?.Descricao ?? string.Empty;

            switch (_configuracaoIntegracao.OrigemDataInicioViagem)
            {
                case OrigemDataInicioViagem.DataEnvioIntegracao:
                    dadosRequisicao.DataInicioViagem = DateTime.Now;
                    break;

                case OrigemDataInicioViagem.DataCarregamentoCarga:
                    if (carga.DataCarregamentoCarga == null)
                        throw new ServicoException("Para essa integração é necessário informar a Data de carregamento.");

                    dadosRequisicao.DataInicioViagem = carga.DataCarregamentoCarga.Value;
                    break;
                default:
                    dadosRequisicao.DataInicioViagem = DateTime.Now;
                    break;
            }

            Int16.TryParse(await ObterCodigoIntegracaoTipoOperacao(carga, etapaIntegracao), out short idTipoOperacao);

            dadosRequisicao.IdTipoOperacao = idTipoOperacao;
            dadosRequisicao.CNPJEmbarcador = _configuracaoIntegracao.CNPJEmbarcador;
            dadosRequisicao.CNPJTransportadora = carga.Empresa?.CNPJ ?? string.Empty;
            dadosRequisicao.AnoSMP = (short)DateTime.Now.Year;
            dadosRequisicao.DocumentoControle = carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador;

            bool naoUtilizarRastreadores = _configuracaoIntegracao.NaoUtilizarRastreadores;

            if (_configuracaoIntegracao.EtapaCarga != null && _configuracaoIntegracao.EtapaCarga != etapaIntegracao && _configuracaoIntegracao.EtapaCarga != SituacaoCarga.Todas)
            {
                naoUtilizarRastreadores = false;
            }

            ObterMotoristas(carga, ref dadosRequisicao);
            ObterVeiculos(carga, ref dadosRequisicao, naoUtilizarRastreadores);
            await ObterPontosPassagemCarga(carga, dadosRequisicao, etapaIntegracao);

            return dadosRequisicao;
        }

        private Servico.IntegracaoSMP.BuscaSMPModeloIntegracao ObterObjetoRequestBuscarSMP(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao carga)
        {
            Servico.IntegracaoSMP.BuscaSMPModeloIntegracao dadosRequisicao = new Servico.IntegracaoSMP.BuscaSMPModeloIntegracao();

            int.TryParse(carga.Protocolo, out int codigoSMP);
            dadosRequisicao.AnoSMP = (short)DateTime.Now.Year;
            dadosRequisicao.NumeroSMP = codigoSMP;

            return dadosRequisicao;
        }

        private Servico.IntegracaoSMP.CancelaSMPModeloIntegracao ObterObjetoRequestCancelaSMP(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao carga)
        {
            Servico.IntegracaoSMP.CancelaSMPModeloIntegracao dadosRequisicao = new Servico.IntegracaoSMP.CancelaSMPModeloIntegracao();

            int.TryParse(carga.Protocolo, out int codigoSMP);
            dadosRequisicao.AnoSMP = (short)DateTime.Now.Year;
            dadosRequisicao.NumeroSMP = codigoSMP;

            return dadosRequisicao;
        }

        private ServicoAPISULLOG.BuscaEventoSMPModeloIntegracao ObterEventoIntegracaoSMP(int numeroSMP)
        {
            Servicos.ServicoAPISULLOG.BuscaEventoSMPModeloIntegracao retorno = new Servicos.ServicoAPISULLOG.BuscaEventoSMPModeloIntegracao();
            retorno.AnoSMP = Convert.ToInt16(DateTime.Now.Year);
            retorno.NumeroSMP = numeroSMP;
            return retorno;
        }

        private async Task ObterPontosPassagemCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Servico.IntegracaoSMP.SMPModeloIntegracao dadosRequisicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga etapaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioEmbarcadorPontoPasagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            DateTime? PrevisaoChegada_ = repositorioPedido.BuscarMaiorDataPrevisaoEntregaEntrePedidosPorCarga(carga.Codigo);

            PrevisaoChegada_ = PrevisaoChegada_ > dadosRequisicao.DataInicioViagem ? PrevisaoChegada_ : dadosRequisicao.DataInicioViagem;

            dadosRequisicao.Pontos = new List<Servico.IntegracaoSMP.SMPPontoModeloIntegracao>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontoPasagem = await repositorioEmbarcadorPontoPasagem.BuscarPorCargaAsync(carga.Codigo);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaComValorNF> cargaEntregaComValorNF = repositorioCargaEntrega.BuscaCargaEntregaComValorNF(carga.Codigo);

            if (_configuracaoIntegracao.IdentificadorUnicoViagem == null)
                throw new ServicoException("Identificador único da viagem não está configurado.");

            for (int i = 0; i < pontoPasagem.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoAtual = pontoPasagem[i];
                decimal valorTotal = 0;
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaComValorNF objValorCargaEntrega = cargaEntregaComValorNF.Where(o => o.Remetente == pontoAtual.Cliente?.Codigo).FirstOrDefault();

                List<Servico.IntegracaoSMP.SMPPontoDocumentoSMPModeloIntegracao> documentos = new List<Servico.IntegracaoSMP.SMPPontoDocumentoSMPModeloIntegracao>();

                if (carga.Alocacao != null)
                {
                    decimal valor = carga.CargaCTes?.SelectMany(cte => cte.NotasFiscais).Where(pedidoNota => pedidoNota.PedidoXMLNotaFiscal?.XMLNotaFiscal != null).Sum(nota => nota?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Valor) ?? 0;

                    Servico.IntegracaoSMP.SMPPontoDocumentoSMPModeloIntegracao documento = new Servico.IntegracaoSMP.SMPPontoDocumentoSMPModeloIntegracao
                    {
                        Documento = carga.Alocacao,
                        IdTipoDocumentoSMP = 6,
                        Valor = valor != 0 ? valor : 0.1m,
                    };

                    documentos.Add(documento);
                }

                if (objValorCargaEntrega == null)
                {
                    if (cargaEntregaComValorNF.Count > 0)
                    {
                        valorTotal = (decimal)cargaEntregaComValorNF.Sum(x => x.ValorNf);
                    }
                }
                else
                {
                    valorTotal = (decimal)objValorCargaEntrega.ValorNf;
                }

                byte idTipoPontoSMPDeterminado = (byte)(pontoAtual.TipoPontoPassagem == TipoPontoPassagem.Coleta ? 1 : 2);
                int raioPadrao = 100;
                Dominio.Entidades.Cliente cliente = pontoPasagem[i].Cliente;

                if (cliente == null)
                    continue;

                string identificador = string.Empty;

                switch (_configuracaoIntegracao.IdentificadorUnicoViagem)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Integracao.ApiSulLog.IdentificadorUnicoViagem.CodIntegracaoNomeCliente:
                        identificador = $"{cliente.CodigoIntegracao}-{cliente.Nome}";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Integracao.ApiSulLog.IdentificadorUnicoViagem.CodIntegracaoCidadeUF:
                        identificador = $"{cliente.CodigoIntegracao}-{cliente.Localidade?.Descricao}-{cliente.Localidade?.Estado.Sigla}";
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Valor inesperado para IdentificadorUnicoViagem: {_configuracaoIntegracao.IdentificadorUnicoViagem}");
                }

                if (dadosRequisicao.Pontos.Any(x => x.Identificador == identificador))
                    continue;

                if (string.IsNullOrEmpty(_configuracaoIntegracao.TipoCarga))
                    throw new ServicoException("Tipo de carga não configurado para integração com ApisulLog.");

                dadosRequisicao.Pontos.Add(new Servico.IntegracaoSMP.SMPPontoModeloIntegracao()
                {
                    CEP = cliente?.Localidade?.CEP ?? string.Empty,
                    Identificador = identificador,
                    Endereco = cliente?.Endereco ?? string.Empty,
                    Cidade = cliente?.Localidade?.Descricao ?? string.Empty,
                    Estado = cliente?.Localidade?.Estado?.Descricao ?? string.Empty,
                    Pais = cliente?.Pais?.Abreviacao ?? "BR",
                    TempoPermanencia = 120,
                    PrevisaoChegada = PrevisaoChegada_,
                    Raio = raioPadrao,
                    IdTipoPontoSMP = idTipoPontoSMPDeterminado,
                    Cargas = new List<Servico.IntegracaoSMP.SMPPontoCargaModeloIntegracao>() {
                        new Servico.IntegracaoSMP.SMPPontoCargaModeloIntegracao()
                        {
                            Projeto = await ObterProjeto(carga, etapaIntegracao),
                            ValorCarga = pontoAtual.TipoPontoPassagem == TipoPontoPassagem.Coleta || valorTotal == 0 ? _configuracaoIntegracao.ValorCargaOrigem : valorTotal,
                            TipoCarga = _configuracaoIntegracao.TipoCarga
                        }
                    },
                    Documentos = documentos
                });
            }
        }

        private void ObterVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref Servico.IntegracaoSMP.SMPModeloIntegracao dadosRequisicao, bool naoUtilizarRastreamento = false)
        {
            if (carga.Veiculo == null)
                return;

            dadosRequisicao.Veiculos = new List<Servico.IntegracaoSMP.VeiculoModeloIntegracao>();

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            IList<int> codigosVeiculos = repositorioVeiculo.BuscarVeiculosVinculadoACarga(carga.Codigo);
            codigosVeiculos.Insert(0, carga.Veiculo.Codigo);

            List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarPorCodigos(codigosVeiculos, false);

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                byte idFaturamentoFixo = 2;
                short.TryParse(veiculo?.ModeloVeicularCarga?.CodigoIntegracaoGerenciadoraRisco, out short codigoGeranciadorRisco);

                int.TryParse(veiculo.NumeroEquipamentoRastreador, out int IdRastreador_);
                byte.TryParse(veiculo.TipoComunicacaoRastreador?.CodigoIntegracao, out byte IdTipoComunicacao_);
                int.TryParse(veiculo.TecnologiaRastreador?.CodigoIntegracao, out int IdFabricante_);

                Servico.IntegracaoSMP.VeiculoModeloIntegracao veiculoModeloIntegracao = new Servico.IntegracaoSMP.VeiculoModeloIntegracao()
                {
                    Autonomo = veiculo.Tipo != "P",
                    Chassi = veiculo.Chassi,
                    EmitenteCNPJ = carga.Empresa.CNPJ,
                    Frota = veiculo.NumeroFrota,
                    IdCor = GetCoresApisullogs(veiculo.CorVeiculo?.Descricao ?? string.Empty),
                    IdPerfil = (short?)(veiculo.Tipo.Equals("P") ? 1 : 2),
                    IdTipoClassificacao = codigoGeranciadorRisco,
                    IdTipoFaturamento = idFaturamentoFixo,
                    Placa = veiculo.Placa,
                    Renavam = veiculo.Renavam,
                    TipoVeiculo = veiculo?.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty
                };

                if (!naoUtilizarRastreamento && veiculo.PossuiRastreador)
                {
                    veiculoModeloIntegracao.Rastreadores = new List<Servico.IntegracaoSMP.RastreadorModeloIntegracao> {
                        new Servico.IntegracaoSMP.RastreadorModeloIntegracao()
                        {
                            EmitenteCNPJ =  dadosRequisicao.CNPJEmbarcador,
                            IdFabricante = IdFabricante_,
                            IdRastreador = IdRastreador_,
                            EquipamentoMovel = true,
                            IdTipoComunicacao  = IdTipoComunicacao_,
                            Numero = veiculo.NumeroEquipamentoRastreador
                        }
                    };
                }

                dadosRequisicao.Veiculos.Add(veiculoModeloIntegracao);
            }
        }

        private void ObterMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref Servico.IntegracaoSMP.SMPModeloIntegracao dadosRequisicao)
        {
            dadosRequisicao.Motoristas = new List<Servico.IntegracaoSMP.MotoristaModeloIntegracao>();

            for (int i = 0; i < carga.Motoristas.Count; i++)
            {
                Dominio.Entidades.Usuario motorista = carga.Motoristas[i];

                dadosRequisicao.Motoristas.Add(new Servico.IntegracaoSMP.MotoristaModeloIntegracao()
                {
                    Ativo = motorista.Status == "A" ? true : false,
                    Nome = motorista.Nome,
                    IdTipoMotorista = (short)motorista.TipoMotorista,
                    NumeroDocumento = motorista.CPF_Formatado,
                    IdTipoDocumento = 1,
                    IdPais = 3
                });
            }
        }

        private async Task<string> ObterProjeto(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga etapaIntegracao)
        {
            if (!_configuracaoIntegracao.EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCarga)
                return carga.TipoDeCarga.CodigoTipoCargaEmbarcador;

            Repositorio.Embarcador.Cargas.TipoDeCargaCodigoIntegracao repositorioTipoDeCargaCodigoIntegracao = new Repositorio.Embarcador.Cargas.TipoDeCargaCodigoIntegracao(_unitOfWork);

            List<string> codigosIntegracoes = await repositorioTipoDeCargaCodigoIntegracao.BuscarCodigosIntegracaoPorTipoDeCargaEtapaAsync(carga.TipoDeCarga.Codigo, etapaIntegracao);

            return codigosIntegracoes.FirstOrDefault() ?? string.Empty;
        }

        private async Task<string> ObterCodigoIntegracaoTipoOperacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga etapaIntegracao)
        {
            Repositorio.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco repositorioCodigoIntegracaoGerenciadoraRisco = new Repositorio.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco(_unitOfWork);

            List<string> codigosIntegracoes = await repositorioCodigoIntegracaoGerenciadoraRisco.BuscarCodigosIntegracaoPorTipoOperacaoEtapaAsync(carga.TipoOperacao.Codigo, etapaIntegracao);

            return codigosIntegracoes.FirstOrDefault() ?? carga.TipoOperacao.CodigoIntegracaoGerenciadoraRisco;
        }

        #endregion Métodos Privados
    }
}
