using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.NovoApp.Cargas;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SGT.Mobile.MTrack.V1
{
    /// <summary>
    /// Essa classe cuida apenas da requisição /IniciarViagem e /ObterCargas pro app. Ela obtém os dados das cargas do motorista e os formata.
    /// Não adicione novas funcionalidades aqui que não sejam em relação a isso.
    /// </summary>
    public class Cargas : BaseControllerNovoApp, ICargas
    {
        #region Métodos Públicos

        public ResponseBool RecusarCarga(RequestRecusarCarga request)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(request.codigoCarga);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                // Validação
                if (carga == null)
                    throw new WebServiceException("Carga não encontrada");

                if (motorista == null)
                    throw new WebServiceException("Motorista não encontrado");

                AtualizarAceitePedidosCarga(EnumAceiteMotorista.Recusou, request.codigoCarga, motorista, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = TipoAuditado.Usuario,
                    OrigemAuditado = OrigemAuditado.WebServiceMobile
                };

                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Carga rejeitada pelo motorista {motorista.Descricao} via app", unitOfWork);

                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);
                //deletar a mensagem de carga aguardando confirmacao
                servicoMensagemAlerta.Confirmar(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista);
                //criar nova mensagem de carga recusada pelo motorista (essa mensagem vai servir como bloqueio la na carga..)
                servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.CargaRecusadaMotorista, $"Carga recusada pelo motorista {motorista.Nome} em {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}");

                return new ResponseBool { Sucesso = true };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao recusar a carga", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool { Sucesso = false };
        }

        public ResponseBool IniciarViagem(RequestIniciarViagem request)
        {
            Servicos.Log.TratarErro($"NovoApp - Iniciando IniciarViagem - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            var usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out var unitOfWork);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(request.codigoCarga);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);


                // Validação
                if (carga == null) throw new WebServiceException("Carga não encontrada");
                if (carga.DataInicioViagem != null)
                {
                    Servicos.Log.TratarErro("Carga já foi iniciada");
                    return new ResponseBool
                    {
                        Sucesso = true,
                    };
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
                if (!(carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                {
                    wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                    {
                        Latitude = request.coordenada.latitude,
                        Longitude = request.coordenada.longitude
                    };
                }

                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                var configuracaoEmbarcador = ObterConfiguracaoTMS(unitOfWork);
                var motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = TipoAuditado.Usuario;
                auditado.OrigemAuditado = OrigemAuditado.WebServiceMobile;

                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, Utilidades.DateTime.FromUnixSeconds(request.coordenada.dataCoordenada) ?? DateTime.MinValue, OrigemSituacaoEntrega.App, wayPoint, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, auditado, unitOfWork))
                {
                    if (motorista != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Início de viagem informado pelo motorista {motorista.Descricao} via app", unitOfWork);

                    Servicos.Log.TratarErro($"NovoAppYandehTarefa66332FluxoPatio - Iniciando IniciarViagemNoControleDePatioAoIniciarViagemNoApp - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
                    if (carga.TipoOperacao?.ConfiguracaoMobile?.IniciarViagemNoControleDePatioAoIniciarViagemNoApp ?? false)
                    {
                        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

                        servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioViagem, DateTime.Now);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, fluxoGestaoPatio, $"Etapa do Fluxo de pátio confirmada por {motorista.Descricao} via app", unitOfWork);
                        Servicos.Log.TratarErro($"NovoAppYandehTarefa66332FluxoPatio - Alterações executadas dentro do if IniciarViagemNoControleDePatioAoIniciarViagemNoApp - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
                    }
                    Servicos.Log.TratarErro($"NovoAppYandehTarefa66332FluxoPatio - Alterações executadas finalizadas após o if IniciarViagemNoControleDePatioAoIniciarViagemNoApp - request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");

                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listMonitoramento = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(carga.Veiculo.Placa);

                    if (listMonitoramento.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoCarga = listMonitoramento.Find(p => p.Carga.CodigoCargaEmbarcador != carga.CodigoCargaEmbarcador);
                        if (monitoramentoCarga != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaComMonitoramentoAtivo = repCarga.BuscarPorCodigo(monitoramentoCarga.Carga.Codigo);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaComMonitoramentoAtivo, $"Monitoramento encerrado na carga {cargaComMonitoramentoAtivo.CodigoCargaEmbarcador}, visto que foi iniciada viagem na carga {carga.CodigoCargaEmbarcador} pelo motorista {motorista.Descricao} via app.", unitOfWork);
                        }
                    }

                }


                NotificarMotoristasInicioViagem(motorista, carga, unitOfWork);

                //forçar mostrar alerta na tela de acompanhamento carga
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargasAcompanamentoMSMQ(carga, this.Cliente.Codigo);

                Servicos.Log.TratarErro($"NovoApp - IniciarViagem - Finalizado");

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao recusar a carga", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public List<ResponseObterCargas> ObterCargas(int clienteMultisoftware)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile repConfiguracaoMobile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();

                int quantidadeCargas = 0;
                DateTime? dataLimite = null;
                DateTime? dataLimiteInicio = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataReferenciaBusca? dataReferenciaBusca = null;

                if (configuracaoMobile.RetornarMultiplasCargasApp)
                {
                    dataReferenciaBusca = DataReferenciaBusca.DataCarregamentoCarga;

                    if (configuracaoMobile.DataReferenciaBusca.HasValue && configuracaoMobile.IntervaloInicial > 0 && configuracaoMobile.IntervaloFinal > 0)
                    {
                        dataReferenciaBusca = configuracaoMobile.DataReferenciaBusca.Value;
                        dataLimiteInicio = DateTime.Now.AddDays(-configuracaoMobile.IntervaloInicial);
                        dataLimite = DateTime.Now.AddDays(configuracaoMobile.IntervaloFinal);
                    }
                    else if (configuracaoMobile.DiasLimiteRetornarMultiplasCargas > 0)
                        dataLimite = DateTime.Now.AddDays(configuracaoMobile.DiasLimiteRetornarMultiplasCargas);
                    else
                        dataLimite = DateTime.Now;
                }
                else
                    quantidadeCargas = 1;

                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigoMobile(usuarioMobileCliente.UsuarioMobile.Codigo);

                bool orderCrescente = configuracao.OrdenarCargasMobileCrescente;
                bool naoOrdenarPorData = false;
                if (!orderCrescente && motorista != null)
                {
                    orderCrescente = motorista.Empresa?.OrdenarCargasMobileCrescente ?? false;
                    if (!orderCrescente)
                        orderCrescente = motorista.OrdenarCargasMobileCrescente;

                    if (orderCrescente)
                        naoOrdenarPorData = true;
                }

                List<int> cargas = repCarga.BuscarCodigosCargaPorMotorista(usuarioMobileCliente.UsuarioMobile.Codigo, quantidadeCargas, !orderCrescente, naoOrdenarPorData, configuracao.ExibirEntregaAntesEtapaTransporte, configuracao.HorasCargaExibidaNoApp, dataLimite, dataLimiteInicio, dataReferenciaBusca);

                return ConverterCargasEmListaResponseObterCargas(cargas, unitOfWork);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                List<int> cargas = new List<int>();
                return ConverterCargasEmListaResponseObterCargas(cargas, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Não foi possível obter as cargas, sincronize e tente novamente.", System.Net.HttpStatusCode.InternalServerError);
                List<int> cargas = new List<int>();
                return ConverterCargasEmListaResponseObterCargas(cargas, unitOfWork);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }
        }

        public string BaixarDocumentosDeTransporteCarga(RequestDownloadDocumentoTransporte request)
        {
            ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS(unitOfWork);
                string pdf = "";
                if (request.tipoDocumentoTransporte == Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte.CTe
                    || request.tipoDocumentoTransporte == Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte.NFSe
                    || request.tipoDocumentoTransporte == Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte.Outros)
                {
                    Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(request.codigoDocumento);
                    pdf = serCTe.ObterRetornoPDF(cte, configuracao.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                }
                else if (request.tipoDocumentoTransporte == Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte.MDFe)
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                    Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(request.codigoDocumento);
                    pdf = serMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unitOfWork, configuracao.UtilizarCodificacaoUTF8ConversaoPDF);
                }
                else if (request.tipoDocumentoTransporte == Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte.ValePedagio)
                {
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaIntegracaoValePedagio.BuscarPorCodigo(request.codigoDocumento);

                    byte[] arquivo = null;
                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);
                    servicoValePedagio.ObterArquivoValePedagio(cargaIntegracaoValePedagio, ref arquivo, TipoServicoMultisoftware);
                    if (arquivo != null)
                    {
                        if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                            pdf = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                        else
                            pdf = Convert.ToBase64String(arquivo);
                    }
                }
                else if (request.tipoDocumentoTransporte == Dominio.ObjetosDeValor.NovoApp.Enumeradores.TipoDocumentoTransporte.CIOT)
                {
                    Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                    Servicos.CIOT serCIOT = new Servicos.CIOT(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCodigo(request.codigoDocumento, false);
                    string mensagemErro;
                    byte[] arquivo = null;
                    if (cargaCIOT.CIOT.Operadora == OperadoraCIOT.eFrete)
                        arquivo = new Servicos.Embarcador.CIOT.EFrete().ObterOperacaoTransportePdf(cargaCIOT, unitOfWork, out mensagemErro);
                    else
                        arquivo = new Servicos.Embarcador.CIOT.CIOT().GerarContratoFrete(cargaCIOT.CIOT.Codigo, unitOfWork, out mensagemErro);

                    if (!string.IsNullOrWhiteSpace(mensagemErro) && arquivo != null)
                    {
                        if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                            pdf = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, arquivo));
                        else
                            pdf = Convert.ToBase64String(arquivo);
                    }
                }
                return pdf;
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                return "";
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao buscar documentos", System.Net.HttpStatusCode.InternalServerError);
                return "";
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }
        }

        public ResponseDocumentosTransporteCarga ObterDocumentosDeTransporteCarga(int clienteMultisoftware, int codigoCarga)
        {
            var usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out var unitOfWork);

            try
            {
                ResponseDocumentosTransporteCarga responseDocumentosTransporteCarga = new ResponseDocumentosTransporteCarga();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCte.BuscarPorCarga(codigoCarga, true, false);

                responseDocumentosTransporteCarga.CTes = new List<DocumentoFiscal>();
                responseDocumentosTransporteCarga.NFSe = new List<DocumentoFiscal>();
                responseDocumentosTransporteCarga.Outros = new List<DocumentoFiscal>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico doc = cargaCTe.CTe;
                    DocumentoFiscal documentoFiscal = new DocumentoFiscal();
                    documentoFiscal.Codigo = doc.Codigo;
                    documentoFiscal.Chave = "";
                    documentoFiscal.Numero = doc.Numero;
                    documentoFiscal.Serie = doc.Serie.Numero;

                    if (doc.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        documentoFiscal.Chave = doc.Chave;
                        responseDocumentosTransporteCarga.CTes.Add(documentoFiscal);
                    }
                    else if (doc.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || doc.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        responseDocumentosTransporteCarga.NFSe.Add(documentoFiscal);
                    else if (doc.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                        responseDocumentosTransporteCarga.Outros.Add(documentoFiscal);
                }


                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarAutorizadosPorProtocoloCarga(codigoCarga);

                responseDocumentosTransporteCarga.MDFes = new List<DocumentoFiscal>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                {
                    DocumentoFiscal documentoFiscal = new DocumentoFiscal();
                    documentoFiscal.Codigo = cargaMDFe.MDFe.Codigo;
                    documentoFiscal.Chave = cargaMDFe.MDFe.Chave;
                    documentoFiscal.Numero = cargaMDFe.MDFe.Numero;
                    documentoFiscal.Serie = cargaMDFe.MDFe.Serie.Numero;
                    responseDocumentosTransporteCarga.MDFes.Add(documentoFiscal);
                }

                responseDocumentosTransporteCarga.ValePedagios = new List<ValePedagio>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(codigoCarga);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio in cargaValePedagios)
                {
                    if (cargaValePedagio.CargaIntegracaoValePedagio != null)
                    {
                        ValePedagio valePedagio = new ValePedagio();
                        valePedagio.Codigo = cargaValePedagio.CargaIntegracaoValePedagio.Codigo;
                        valePedagio.Numero = cargaValePedagio.NumeroComprovante;
                        valePedagio.Integradora = cargaValePedagio.CargaIntegracaoValePedagio.TipoIntegracao.DescricaoTipo;
                        valePedagio.Valor = cargaValePedagio.Valor;
                        responseDocumentosTransporteCarga.ValePedagios.Add(valePedagio);
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(codigoCarga);

                responseDocumentosTransporteCarga.CIOTs = new List<CIOT>();
                if (cargaCIOT != null)
                {
                    CIOT ciot = new CIOT();
                    ciot.Codigo = cargaCIOT.Codigo;
                    ciot.Numero = cargaCIOT.CIOT.Numero;
                    ciot.Operadora = cargaCIOT.CIOT.Operadora.ObterDescricao();
                    responseDocumentosTransporteCarga.CIOTs.Add(ciot);
                }

                return responseDocumentosTransporteCarga;
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao obter documentos de transporte da carga", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

        }

        public ResponseObterExtratoViagem ObterExtratoViagem(int clienteMultisoftware)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                ResponseObterExtratoViagem responseObterExtratoViagem = new ResponseObterExtratoViagem();

                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Fatura.Justificativa> justificativas = repJustificativa.BuscarTodasAtivas();
                responseObterExtratoViagem.MotivosDespesa = new List<MotivoDespesa>();
                foreach (var justificativa in justificativas)
                    responseObterExtratoViagem.MotivosDespesa.Add(new MotivoDespesa() { Codigo = justificativa.Codigo, Descricao = justificativa.Descricao });

                responseObterExtratoViagem.MensagemRetorno = "Sucesso";
                responseObterExtratoViagem.StatusRetorno = true;
                if (motorista == null)
                {
                    responseObterExtratoViagem.MensagemRetorno = "Motorista não localizado.";
                    responseObterExtratoViagem.StatusRetorno = false;
                    return responseObterExtratoViagem;
                }

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarAcertoAberto(motorista.Codigo);
                if (acertoViagem == null)
                {
                    responseObterExtratoViagem.MensagemRetorno = "Motorista não possui acerto de viagem em aberto.";
                    responseObterExtratoViagem.StatusRetorno = false;
                    return responseObterExtratoViagem;
                }

                List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita = servAcertoViagem.RetornaObjetoReceitaViagem(acertoViagem.Codigo, unitOfWork, configuracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, configuracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, configuracaoEmbarcador.GerarTituloFolhaPagamento, configuracaoEmbarcador.GerarReciboAcertoViagemDetalhado, configuracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

                responseObterExtratoViagem.PrevisaoDiaria = servAcertoViagem.RetornarPreviaValorDiaria(acertoViagem, unitOfWork);
                responseObterExtratoViagem.SaldoDespesa = receita[0]?.TotalDespesaMotorista ?? 0m;
                responseObterExtratoViagem.SaldoReceita = receita[0]?.TotalReceitaMotorista ?? 0m;

                return responseObterExtratoViagem;
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao obter extrato da viagem", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

        }

        /// <summary>
        /// Esse método é usado apenas para testes. Retorna a mesma coisa que /ObterCargas,
        /// mas é possível escolher especificamente qual carga quer que seja retornada, arbitrariamente.
        /// </summary>
        public List<ResponseObterCargas> ObterCargaEspecifica(int clienteMultisoftware, int codigoCarga, string codigoCargaEmbarcador)
        {
            var usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out var unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var configuracao = ObterConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga;
                if (codigoCarga > 0)
                {
                    carga = repCarga.BuscarPorCodigo(codigoCarga);
                }
                else
                {
                    carga = repCarga.BuscarCargaPorCodigoEmbarcador(codigoCargaEmbarcador);
                }

                if (carga == null)
                {
                    return new List<ResponseObterCargas> { };
                }

                var cargas = new List<int> { carga.Codigo };
                return ConverterCargasEmListaResponseObterCargas(cargas, unitOfWork);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao obter a carga", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

        }

        /// <summary>
        /// Obtém apenas uma parada de uma carga. Usado pelo app para obter a nova parada quando uma
        /// nova é adicionada enquanto o motorista já está com a carga ativa
        /// </summary>
        public Parada ObterParada(int clienteMultisoftware, int codigoCargaEntrega)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                if (cargaEntrega == null)
                    throw new Exception("Parada não encontrada");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile configuracaoMobile = cargaEntrega.Carga.TipoOperacao?.ConfiguracaoMobile;
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork); Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>() { cargaEntrega };

                List<Parada> paradas = ObterParadas(cargaEntrega.Carga, listaCargaEntrega, ObterVeiculo(cargaEntrega.Carga), unitOfWork, configuracaoMobile, configuracaoEmbarcador);
                if (paradas.Count > 0)
                    return paradas[0];

                throw new Exception("Parada não encontrada");
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao obter a parada.", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }
        }

        public ResponseBool AceitarCargaMotorista(RequestAceitarCargaMotorista request)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(request.codigoCarga);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaMobilePorCPF(usuarioMobileCliente.UsuarioMobile.CPF);

                if (carga == null)
                    throw new WebServiceException("Carga não encontrada");

                if (motorista == null)
                    throw new WebServiceException("Motorista não encontrado");

                if (!(carga.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false))
                    throw new WebServiceException("Tipo de Operação não permite confirmação do motorista");

                if (!carga.DataLimiteConfirmacaoMotorista.HasValue)
                    throw new WebServiceException("Carga não possui data limite para confirmação do motorista");

                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);

                if (!servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista))
                    throw new WebServiceException("Carga já está confirmada");

                if (carga.DataLimiteConfirmacaoMotorista.Value < DateTime.Now)
                    throw new WebServiceException("O tempo para aceitar esta carga já expirou");

                servicoMensagemAlerta.Confirmar(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista);

                AtualizarAceitePedidosCarga(EnumAceiteMotorista.Aceite, request.codigoCarga, motorista, unitOfWork);

                carga.DataLimiteConfirmacaoMotorista = null;
                repCarga.Atualizar(carga);

                if (((carga.Veiculo != null && carga.Motoristas.Count > 0) || carga.TipoOperacao.NaoExigeVeiculoParaEmissao) && carga.Empresa != null)
                {
                    if (carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        carga.SituacaoCarga = SituacaoCarga.AgNFe;

                        if (!(carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && repCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                        {
                            bool exigeConfirmacaoFreteAntesEmissao = carga.Filial?.ExigeConfirmacaoFreteAntesEmissao ?? false;
                            if (!exigeConfirmacaoFreteAntesEmissao)
                                exigeConfirmacaoFreteAntesEmissao = carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? true;

                            if (!exigeConfirmacaoFreteAntesEmissao)
                            {
                                carga.DataEnvioUltimaNFe = DateTime.Now;
                                carga.DataRecebimentoUltimaNFe = DateTime.Now;
                                carga.DataInicioEmissaoDocumentos = DateTime.Now;
                            }

                            carga.ProcessandoDocumentosFiscais = true;
                            carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = TipoAuditado.Usuario,
                    OrigemAuditado = OrigemAuditado.WebServiceMobile
                };
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Carga aceita pelo motorista {motorista.Descricao} via app", unitOfWork);

                return new ResponseBool { Sucesso = true };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu um erro ao aceitar a carga", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool { Sucesso = false };
        }

        public ResponseBool AdicionarAnexosCarga(RequestSalvarAnexo request)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.ClienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

                if (usuarioMobileCliente == null)
                    return new ResponseBool { Sucesso = false };

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(request.CodigoCarga);

                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "ColetaContainerAnexo" });

                if (coletaContainer == null)
                    throw new WebServiceException($"Não se achou carga com o codigo {request.CodigoCarga}");

                string guid = Guid.NewGuid().ToString();

                byte[] dadosAsBytes = System.Convert.FromBase64String(request.ArquivoBase64);

                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guid}{request.Extensao}"), dadosAsBytes);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo cargaAnexo = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo()
                {
                    ColetaContainer = coletaContainer,
                    Descricao = "Arquivo anexado por webservise",
                    NomeArquivo = $"Arquivo_{guid}{request.Extensao}",
                    GuidArquivo = guid
                };

                repositorioColetaContainerAnexo.Inserir(cargaAnexo);
                return new ResponseBool { Sucesso = true };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
                return new ResponseBool { Sucesso = false };
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                RetornarErro("Ocorreu um erro ao adicionar anexos na carga", System.Net.HttpStatusCode.InternalServerError);
                return new ResponseBool { Sucesso = false };
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void IniciarMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime? dataInicio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
            auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMonitoriamento;

            Servicos.Embarcador.Monitoramento.Monitoramento.IniciarMonitoramento(
                carga,
                dataInicio,
                configuracaoEmbarcador,
                auditado,
                unitOfWork
            );
        }

        private void NotificarMotoristasInicioViagem(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (motorista == null)
                return;

            var serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
            serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(carga, null, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.InicioViagem);
        }

        private List<ResponseObterCargas> ConverterCargasEmListaResponseObterCargas(List<int> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            var configuracao = ObterConfiguracaoTMS(unitOfWork);
            return (from o in cargas select ConverterCargaEmResponseObterCargas(o, configuracao, unitOfWork)).ToList();
        }

        private ResponseObterCargas ConverterCargaEmResponseObterCargas(int codigoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile configuracaoTipoOperacaoMobile = carga.TipoOperacao?.ConfiguracaoMobile;
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile repConfiguracaoMobile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> paradas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            if (configuracaoTipoOperacaoMobile?.NaoRetornarColetas ?? false)
                paradas = repCargaEntrega.BuscarEntregasPorCarga(carga.Codigo);
            else
                paradas = repCargaEntrega.BuscarPorCarga(carga.Codigo);

            var veiculo = ObterVeiculo(carga);

            string key = Environment.MachineName + "!Mu!tis*ftw@r3#";
            string token = Servicos.Criptografia.Criptografar(carga.Codigo.ToString() + "-" + DateTime.Now.ToDateTimeString(), key, true);

            return new ResponseObterCargas
            {
                Codigo = carga.Codigo,
                ObservacaoTransportador = carga.ObservacaoTransportador,
                Paradas = ObterParadas(carga, paradas, veiculo, unitOfWork, configuracaoTipoOperacaoMobile, configuracaoEmbarcador),
                Configuracoes = ObterConfiguracoes(carga, configuracaoEmbarcador, configuracaoTipoOperacaoMobile, repCargaIntegracao, configuracaoMobile),
                Filial = new Filial
                {
                    Codigo = carga.Filial?.Codigo ?? 0,
                    Nome = carga.Filial?.Descricao ?? ""
                },
                NumeroCargaEmbarcador = carga.CodigoCargaEmbarcador,
                Origens = carga?.DadosSumarizados?.Origens ?? string.Empty,
                Destinos = carga?.DadosSumarizados?.Destinos ?? string.Empty,
                Polilinha = cargaRotaFrete?.PolilinhaRota ?? string.Empty,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaColetaEntega.AguarandoAceita,
                TipoCarga = new TipoCarga
                {
                    Codigo = carga?.TipoDeCarga?.Codigo ?? 0,
                    Descricao = carga?.TipoDeCarga?.Descricao ?? string.Empty
                },
                TipoOperacao = new Dominio.ObjetosDeValor.NovoApp.Cargas.TipoOperacao
                {
                    Codigo = carga?.TipoOperacao?.Codigo ?? 0,
                    Descricao = carga?.TipoOperacao?.Descricao ?? string.Empty
                },
                Veiculo = veiculo,
                Motoristas = (from m in carga.Motoristas
                              select new Dominio.ObjetosDeValor.NovoApp.Cargas.Motorista
                              {
                                  Codigo = m.Codigo,
                                  Nome = m.Nome,
                                  Transportadora = new Dominio.ObjetosDeValor.NovoApp.Cargas.Transportadora
                                  {
                                      Codigo = m.Empresa?.Codigo ?? carga.Empresa.Codigo,
                                      Nome = m.Empresa?.NomeFantasia ?? carga.Empresa.NomeFantasia,
                                  }
                              }).ToList(),
                ViagemIniciada = carga.DataInicioViagem.HasValue,
                JustificativasTemperatura = ObterJustificativasTemperaturas(unitOfWork),
                MotivosRejeicaoEntrega = ObterMotivosRejeicaoEntrega(unitOfWork),
                MotivosRejeicaoColeta = ObterMotivosRejeicaoColeta(unitOfWork),
                MotivosRetificacaoColeta = ObterMotivosRetificacaoColeta(unitOfWork),
                TiposEventos = ObterTiposEventos(unitOfWork),
                MotivosFalhaGta = ObterMotivosFalhaGta(unitOfWork),
                MotivosFalhaNotaFiscal = ObterMotivosFalhaNotaFiscal(unitOfWork),
                RegrasReconhecimentoCanhoto = ObterRegrasReconhecimentoCanhoto(unitOfWork),
                DataCriacao = carga.DataCriacaoCarga.ToUnixSeconds(),
                DataPrevisaoInicioViagem = carga.DataInicioViagemPrevista?.ToUnixSeconds(),
                BloquearRastreamento = carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false,
                NecessarioConfirmacaoMotorista = carga.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false,
                DataLimiteConfirmacaoMotorista = carga.DataLimiteConfirmacaoMotorista?.ToUnixSeconds(),
                // Detalhes a mais da carga
                Temperatura = carga.FaixaTemperatura?.DescricaoVariancia,
                DistanciaTotal = carga.DadosSumarizados?.Distancia ?? 0,
                Duracao = $"{String.Format("{0:0.#}", carga.TempoPrevistoEmHoras)} horas",
                VolumeTotal = $"{carga.DadosSumarizados?.VolumesTotal} unidades",
                Peso = carga?.DadosSumarizados?.PesoTotal ?? 0,
                DataCarregamento = carga.DataCarregamentoCarga?.ToUnixSeconds(),
                DataAgendamento = carga.Pedidos?.FirstOrDefault().Pedido.DataAgendamento?.ToUnixSeconds(),
                URLAcessoDocumentosCarga = $"https://{configuracaoTMS.LinkUrlAcessoCliente}/download-documentos-carga/{token}"
            };
        }

        private Configuracoes ObterConfiguracoes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, ConfiguracaoTipoOperacaoMobile configuracaoTipoOperacaoMobile, Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile)
        {
            return new Configuracoes
            {
                ObrigarInformarRIC = carga.TipoOperacao?.ObrigarInformarRICnaColetaDeConteiner ?? false,
                ObrigarFotoNaDevolucao = configuracaoEmbarcador.ObrigarFotoNaDevolucao,
                PermiteQrCodeMobile = configuracaoEmbarcador.PermiteQRCodeMobile,
                PermiteImpressaoMobile = carga.TipoOperacao?.PermiteImpressaoMobile ?? false,
                ExibirCalculadoraMobile = carga.TipoOperacao?.ExibirCalculadoraMobile ?? false,
                PermiteRetificarMobile = carga.TipoOperacao?.PermiteRetificarMobile ?? false,
                ExigeSenhaClienteRecebimento = carga.TipoOperacao?.ExigeSenhaConfirmacaoEntrega ?? false,
                NumeroTentativasSenhaClientePermitidas = carga.TipoOperacao?.NumeroTentativaSenhaConfirmacaoEntrega ?? 0,
                ServerChatUrl = configuracaoEmbarcador.ServerChatURL,
                NaoPermiteRejeitarEntrega = carga.TipoOperacao?.NaoPermiteRejeitarEntrega ?? false,
                HabilitarControleFluxoNFeDevolucaoChamado = configuracaoEmbarcador.HabilitarControleFluxoNFeDevolucaoChamado,
                ObrigarJustificativaSolicitacoesForaAreaCliente = configuracaoEmbarcador.JustificarEntregaForaDoRaio || (configuracaoTipoOperacaoMobile?.SolicitarJustificativaRegistroForaRaio ?? false),
                ObrigarFotoCanhoto = configuracaoTipoOperacaoMobile?.ObrigarFotoCanhoto ?? false,
                ObrigarAssinaturaEntrega = configuracaoTipoOperacaoMobile?.ObrigarAssinaturaEntrega ?? false,
                ObrigarDadosRecebedor = configuracaoTipoOperacaoMobile?.ObrigarDadosRecebedor ?? false,
                ForcarPreenchimentoSequencialMobile = configuracaoTipoOperacaoMobile?.ForcarPreenchimentoSequencial ?? false,
                PermiteFotosEntrega = configuracaoTipoOperacaoMobile?.PermiteFotosEntrega ?? false,
                QuantidadeMinimasFotosEntrega = configuracaoTipoOperacaoMobile?.QuantidadeMinimasFotosEntrega ?? 0,
                PermiteFotosColeta = configuracaoTipoOperacaoMobile?.PermiteFotosColeta ?? false,
                QuantidadeMinimasFotosColeta = configuracaoTipoOperacaoMobile?.QuantidadeMinimasFotosColeta ?? 0,
                PermiteConfirmarChegadaEntrega = configuracaoTipoOperacaoMobile?.PermiteConfirmarChegadaEntrega ?? false,
                PermiteConfirmarChegadaColeta = configuracaoTipoOperacaoMobile?.PermiteConfirmarChegadaColeta ?? false,
                ControlarTempoColeta = configuracaoTipoOperacaoMobile?.ControlarTempoColeta ?? false,
                NaoUtilizarProdutosNaColeta = configuracaoTipoOperacaoMobile?.NaoUtilizarProdutosNaColeta ?? false,
                PermitirVisualisarProgramacaoAntesViagem = configuracaoTipoOperacaoMobile?.PermitirVisualizarProgramacaoAntesViagem ?? false,
                PermiteEventos = configuracaoTipoOperacaoMobile?.PermiteEventos ?? false,
                PermiteChat = configuracaoTipoOperacaoMobile?.PermiteChat ?? false,
                PermiteSac = configuracaoTipoOperacaoMobile?.PermiteSAC ?? false,
                ObrigarAssinaturaProdutor = configuracaoTipoOperacaoMobile?.ObrigarAssinaturaProdutor ?? false,
                AguardarAnaliseNaoConformidadesNFsCheckin = configuracaoTipoOperacaoMobile?.AguardarAnaliseNaoConformidadesNFsCheckin ?? false,
                ExibirAvaliacaoNaAssinatura = configuracaoTipoOperacaoMobile?.ExibirAvaliacaoNaAssintura ?? false,
                PermiteBaixarOsDocumentosDeTransporte = configuracaoTipoOperacaoMobile?.PermiteBaixarOsDocumentosDeTransporte ?? false,
                PermiteConfirmarEntrega = configuracaoTipoOperacaoMobile?.PermiteConfirmarEntrega ?? true,
                BloquearRastreamento = configuracaoTipoOperacaoMobile?.BloquearRastreamento ?? false,
                PermiteCanhotoModoManual = configuracaoTipoOperacaoMobile?.PermiteCanhotoModoManual ?? false,
                PermiteEntregaParcial = configuracaoTipoOperacaoMobile?.PermiteEntregaParcial ?? false,
                ControlarTempoEntrega = configuracaoTipoOperacaoMobile?.ControlarTempoEntrega ?? false,
                ExibirRelatorio = configuracaoTipoOperacaoMobile?.ExibirRelatorio ?? false,
                NaoRetornarColetas = configuracaoTipoOperacaoMobile?.NaoRetornarColetas ?? false,
                DevolucaoProdutosPorPeso = carga?.TipoOperacao?.DevolucaoProdutosPorPeso ?? false,
                ObrigarHandlingUnit = repCargaIntegracao.ExistePorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre),
                ValidarCapacidadeMaximaNoApp = carga?.Veiculo?.ModeloVeicularCarga?.ValidarCapacidadeMaximaNoApp ?? false,
                PermitirEscanearChavesNfe = carga?.TipoOperacao?.ConfiguracaoMobile?.PermitirEscanearChavesNfe ?? false,
                ObrigarEscanearChavesNfe = carga?.TipoOperacao?.ConfiguracaoMobile?.ObrigarEscanearChavesNfe ?? false,
                SolicitarReconhecimentoFacialDoRecebedor = carga?.TipoOperacao?.ConfiguracaoMobile?.SolicitarReconhecimentoFacialDoRecebedor ?? false,
                PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte = carga?.TipoOperacao?.PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte ?? false,
                NaoListarProdutosColetaEntrega = carga?.TipoOperacao?.ConfiguracaoMobile?.NaoListarProdutosColetaEntrega ?? false,
                RetornarMultiplasCargasApp = configuracaoMobile?.RetornarMultiplasCargasApp ?? false,
                NaoApresentarDataInicioViagem = carga?.TipoOperacao?.ConfiguracaoMobile?.NaoApresentarDataInicioViagem ?? false
            };
        }

        private List<Parada> ObterParadas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> paradas, Veiculo veiculo, Repositorio.UnitOfWork unitOfWork, ConfiguracaoTipoOperacaoMobile configuracaoMobile, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repCargaEntregaProduto.BuscarPorCargaPaginado(carga.Codigo, 0, 1000); //DECATHLON - Feito retornar  1000 pois ao retornar muitos produtos APP Mobile não funciona
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaSemFetch(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPaginado(carga.Codigo, 0, 1000); //DECATHLON - Feito retornar  1000 pois ao retornar muitos produtos APP Mobile não funciona
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade = repCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCarga(carga.Codigo);

            return (from cargaEntrega in paradas
                    select ConverterParada(
                        cargaEntrega,
                        cargaEntregaNotasFiscais,
                        cargaEntregaPedidos,
                        cargaEntregaProdutos,
                        cargaPedidoProdutos,
                        cargaCargaPedidoProdutoDivisaoCapacidade,
                        canhotos,
                        veiculo,
                        configuracaoEmbarcador,
                        unitOfWork
                    )).ToList();
        }

        private Parada ConverterParada(
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade,
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> todosCanhotos,
            Veiculo veiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Repositorio.UnitOfWork unitOfWork
        )
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);

            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhotos = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCheckList = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscaisParada = (from obj in cargaEntregaNotasFiscais where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaPedidosParada = (from obj in cargaEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = cargaPedidosParada.FirstOrDefault()?.CargaPedido?.Pedido;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutosParada = (from obj in cargaEntregaProdutos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();

            var tipoChecklist = cargaEntrega.Coleta ? TipoCheckList.Coleta : TipoCheckList.Entrega;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntasChecklist = repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo, tipoChecklist);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> chavesNfe = repCargaEntregaChaveNfe.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            TimeSpan? tempoProgramadaColeta = (pedidoBase?.DataPrevisaoSaida - pedidoBase?.DataInicialColeta);

            GetNotasECanhotos(cargaEntrega, cargaEntregaNotasFiscaisParada, todosCanhotos, out List<Nota> notas, out List<Canhoto> canhotos);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = ObterModalidadeFornecedorPessoas(cargaEntrega, unitOfWork);

            var clienteDescarga = repClienteDescarga.BuscarPorPessoa(cargaEntrega.Cliente.CPF_CNPJ);

            var parada = new Parada
            {
                Codigo = cargaEntrega.Codigo,
                Coleta = cargaEntrega.Coleta,
                ColetaEquipamento = cargaEntrega.ColetaEquipamento,
                Tipo = cargaEntrega.TipoCargaEntrega,
                dataInicio = cargaEntrega.DataInicio?.ToUnixSeconds(),
                dataFim = cargaEntrega.DataFim?.ToUnixSeconds(),
                dataConfirmacao = cargaEntrega.DataConfirmacao?.ToUnixSeconds(),
                DataProgramada = cargaEntrega.Coleta ? pedidoBase?.DataInicialColeta?.ToUnixSeconds() : cargaEntrega.DataPrevista?.ToUnixSeconds(),
                dataConfirmacaoChegada = cargaEntrega.DataEntradaRaio?.ToUnixSeconds(),
                EstouIndo = cargaEntrega.MotoristaACaminho,
                DiferencaDevolucao = cargaEntrega.NotificarDiferencaDevolucao,
                Endereco = cargaEntrega.ClienteOutroEndereco != null ? cargaEntrega.ClienteOutroEndereco.EnderecoCompletoCidadeeEstado : (cargaEntrega.Cliente?.EnderecoCompletoCidadeeEstado ?? string.Empty),
                DataEntradaPropriedade = cargaEntrega.DataEntradaRaio?.ToUnixSeconds(),
                JanelaDescarga = GetJanelaDescarga(cargaEntrega, unitOfWork),
                Ordem = cargaEntrega.Ordem,
                MotivoDevolucao = (!cargaEntrega.Coleta && cargaEntrega.MotivoRejeicao != null) ? new MotivoDevolucao()
                {
                    Codigo = cargaEntrega.MotivoRejeicao.Codigo,
                    Motivo = cargaEntrega.MotivoRejeicao.Descricao
                } : null,
                MotivoRejeicaoColeta = (cargaEntrega.Coleta && cargaEntrega.MotivoRejeicao != null) ? new MotivoRejeicaoColeta
                {
                    Codigo = cargaEntrega.MotivoRejeicao.Codigo,
                    Descricao = cargaEntrega.MotivoRejeicao.Descricao
                } : null,
                MotivoRetificacaoColeta = (cargaEntrega.Coleta && cargaEntrega.MotivoRetificacaoColeta != null) ? new MotivoRetificacaoColeta
                {
                    Codigo = cargaEntrega.MotivoRetificacaoColeta.Codigo,
                    Descricao = cargaEntrega.MotivoRetificacaoColeta.Descricao
                } : null,
                Canhotos = canhotos,
                Notas = notas,
                Questionario = perguntasChecklist.Count() > 0 ? servicoCheckList.ObterObjetoMobileCheckList(perguntasChecklist) : null,
                Cliente = new Dominio.ObjetosDeValor.NovoApp.Cargas.Cliente
                {
                    CodigoIntegracao = cargaEntrega.Cliente.CodigoIntegracao,
                    RaioEmMetros = (cargaEntrega.Cliente.RaioEmMetros.HasValue && cargaEntrega.Cliente.RaioEmMetros.Value > 0) ? cargaEntrega.Cliente.RaioEmMetros.Value : configuracao.RaioPadrao,
                    RazaoSocial = cargaEntrega.Cliente.Descricao,
                    Endereco = GetEnderecoCliente(cargaEntrega),
                    Observacao = cargaEntrega.Cliente.Observacao,
                    SenhaConfirmacaoColetaEntrega = cargaEntrega.Cliente.SenhaConfirmacaoColetaEntrega,
                    NaoEObrigatorioInformarNfeNaColeta = modalidadeFornecedorPessoas?.NaoEObrigatorioInformarNfeNaColeta ?? false,
                    NaoExigePreenchimentoDeChecklistEntrega = clienteDescarga?.NaoExigePreenchimentoDeChecklistEntrega ?? false,
                    NomeFantasia = cargaEntrega.Cliente.NomeFantasia,
                    Telefone = new Telefone
                    {
                        CodigoPais = cargaEntrega.Cliente?.Localidade?.Pais?.CodigoTelefonico.ToString() ?? string.Empty,
                        Numero = cargaEntrega.Cliente?.Telefone1 ?? string.Empty,
                    },
                    NaoAplicarChecklist = cargaEntrega.Cliente.NaoAplicarChecklistMultiMobile
                },
                Pedidos = GetPedidos(cargaEntrega, cargaEntregaProdutosParada, cargaPedidosParada, cargaPedidoProdutos, cargaCargaPedidoProdutoDivisaoCapacidade, veiculo),
                Peso = cargaPedidosParada != null && cargaPedidosParada.Count > 0 ? (from obj in cargaPedidosParada select obj.CargaPedido.Peso).Sum() : 0,
                PossuiReentrega = cargaEntrega.Reentrega,
                Situacao = cargaEntrega.SituacaoParaMobile,
                Ocorrencias = ObterOcorrencias(cargaEntrega, unitOfWork),

                ObservacoesPedidos = cargaPedidosParada != null && cargaPedidosParada.Count > 0 ? string.Join(" / ", (from o in cargaPedidosParada where !string.IsNullOrEmpty(o.CargaPedido.Pedido.Observacao) select o.CargaPedido.Pedido.Observacao)) : string.Empty,

                // Detalhes do agendamento                
                DataAgendamento = cargaPedidosParada.FirstOrDefault()?.CargaPedido?.Pedido?.DataAgendamento?.ToUnixSeconds(),
                ObservacaoAgendamento = cargaPedidosParada.FirstOrDefault()?.CargaPedido?.Pedido?.ObservacaoAdicional,
                ResponsavelAgendamento = cargaEntrega.ResponsavelAgendamento?.Nome,
                chavesNFe = (from o in chavesNfe select o.ChaveNfe).ToList(),
                NaoExigeDigitalizacaoDoCanhotoNaConfirmacaoDaEntrega = cargaEntrega.Cliente.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente
            };

            return parada;
        }

        private List<Dominio.ObjetosDeValor.NovoApp.Cargas.Pedido> GetPedidos(
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutosParada,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaPedidosParada,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade,
            Veiculo veiculo
        )
        {
            List<Dominio.ObjetosDeValor.NovoApp.Cargas.Pedido> pedidos = new List<Dominio.ObjetosDeValor.NovoApp.Cargas.Pedido>();

            // Cada CargaEntrega tem uma lista de pedidos (foreach abaixo)
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaPedidoParada in cargaPedidosParada)
            {
                Dominio.ObjetosDeValor.NovoApp.Cargas.Pedido pedidoParada = new Dominio.ObjetosDeValor.NovoApp.Cargas.Pedido();
                pedidoParada.Codigo = cargaPedidoParada.CargaPedido.Codigo;
                pedidoParada.NumeroPedido = cargaPedidoParada.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                pedidoParada.Observacao = cargaPedidoParada.CargaPedido.Pedido.Observacao;
                pedidoParada.QuantidadeVolumesDestino = cargaPedidoParada.CargaPedido.Pedido.QtVolumes;

                if (cargaPedidoParada.CargaPedido.Pedido.CanalEntrega != null)
                {
                    pedidoParada.CanalEntrega = new Dominio.ObjetosDeValor.NovoApp.Cargas.CanalEntrega();
                    pedidoParada.CanalEntrega.Codigo = cargaPedidoParada.CargaPedido.Pedido.CanalEntrega.Codigo.ToString();
                    pedidoParada.CanalEntrega.Descricao = cargaPedidoParada.CargaPedido.Pedido.CanalEntrega.Descricao;
                }

                pedidoParada.Produtos = new List<Dominio.ObjetosDeValor.NovoApp.Cargas.Produto>();

                // Cada Pedido da CargaEntrega tem vários produtos atrelada à ela (atributo abaixo)
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtosPedido = (from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == cargaPedidoParada.CargaPedido.Codigo select obj).ToList();
                foreach (var produtoPedido in produtosPedido)
                {
                    // Cada produto desse pedido, tem que ter um correspondente, que é o CargaEntregaProduto. É o mesmo produto, mas com relacionamento à CargaEntrega
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto =
                        (from o in cargaEntregaProdutosParada where o.Produto.Codigo == produtoPedido.Produto.Codigo select o).FirstOrDefault();

                    var divisoes = ObterDivisoes(cargaCargaPedidoProdutoDivisaoCapacidade, produtoPedido, veiculo, cargaEntrega, out int numeroAndares, out int numeroLinhas, out int numeroColunas);

                    Dominio.ObjetosDeValor.NovoApp.Cargas.Produto produto = new Dominio.ObjetosDeValor.NovoApp.Cargas.Produto
                    {
                        Codigo = produtoPedido.Produto.CodigoProdutoEmbarcador,
                        Descricao = produtoPedido.Produto.Descricao,
                        ImunoPlanejado = produtoPedido.ImunoPlanejado,
                        ImunoRealizado = produtoPedido.ImunoRealizado,
                        InformarDadosColeta = produtoPedido.Produto.PossuiIntegracaoColetaMobile,
                        InformarTemperatura = produtoPedido.Produto.ObrigatorioInformarTemperatura,
                        ExigeInformarImunos = produtoPedido.Produto.ExigeInformarImunos,
                        ExigeInformarCaixas = produtoPedido.Produto.ExigeInformarCaixas,
                        ObrigatorioGuiaTransporteAnimal = produtoPedido.Produto.ObrigatorioGuiaTransporteAnimal,
                        ObrigatorioNfProdutor = produtoPedido.Produto.ObrigatorioNFProdutor,
                        Observacao = produtoPedido.ObservacaoCarga ?? produtoPedido.Produto.Observacao,
                        PesoUnitario = cargaEntregaProduto?.PesoUnitario ?? 0,
                        NumeroAndares = numeroAndares,
                        NumeroLinhas = numeroLinhas,
                        NumeroColunas = numeroColunas,
                        Divisoes = divisoes,
                        Protocolo = produtoPedido.Codigo,
                        ProtocoloCargaEntregaProduto = cargaEntregaProduto?.Codigo ?? 0,
                        Quantidade = produtoPedido.Quantidade,
                        QuantidadeCaixa = produtoPedido.QuantidadeCaixa,
                        QuantidadePorCaixaRealizada = produtoPedido.QuantidadePorCaixaRealizada,
                        QuantidadeCaixasVazias = produtoPedido.QuantidadeCaixasVazias,
                        QuantidadeCaixasVaziasRealizada = produtoPedido.QuantidadeCaixasVaziasRealizada,
                        QuantidadeDevolucao = cargaEntregaProduto?.QuantidadeDevolucao ?? 0,
                        QuantidadePlanejada = produtoPedido.QuantidadePlanejada,
                        Temperatura = produtoPedido.Temperatura,
                        Nota = cargaEntregaProduto?.XMLNotaFiscal?.Numero.ToString(),
                        UnidadeDeMedida = new Dominio.ObjetosDeValor.NovoApp.Cargas.UnidadeDeMedida()
                        {
                            Codigo = produtoPedido.Produto.Unidade?.Codigo ?? 0,
                            Descricao = produtoPedido.Produto.Unidade?.Descricao ?? "",
                        },
                        MotivoTemperatura = produtoPedido.JustificativaTemperatura?.Codigo,
                    };


                    pedidoParada.Produtos.Add(produto);
                }
                pedidos.Add(pedidoParada);
            }

            return pedidos;
        }

        /// <summary>
        /// Obtém a divisão dos produtos. Divisões é uma forma de dividir a quantidade de produtos no veículo. Por exemplo, no transporte de suínos, eles ficam
        /// divididos em compartimentos na carreta. No transporte de leite, ele fica dividido em vários tonéis.
        /// </summary>
        private List<Divisao> ObterDivisoes(
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade,
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto produtoPedido,
            Veiculo veiculo,
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
            out int numeroAndares,
            out int numeroLinhas,
            out int numeroColunas
        )
        {
            numeroAndares = 1;
            numeroColunas = 1;
            numeroLinhas = 1;

            List<Divisao> divisoes = new List<Divisao>();

            if (!cargaEntrega.Coleta) return divisoes;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> pedidoProdutosDivisaoCapacidade = (from obj in cargaCargaPedidoProdutoDivisaoCapacidade where obj.CargaPedidoProduto.Codigo == produtoPedido.Codigo select obj).ToList();

            // Esse dicionário guarda quantas linhas tem em cada coluna
            Dictionary<string, int?> linhasNaColuna = new Dictionary<string, int?>();

            foreach (var divisaoDoVeiculo in veiculo?.Divisoes)
            {
                var pedidoProdutoDivisaoCapacidade = pedidoProdutosDivisaoCapacidade.Find(p => p.ModeloVeicularCargaDivisaoCapacidade.Codigo == divisaoDoVeiculo.Codigo);

                string chaveDicionario = (divisaoDoVeiculo.Coluna ?? 0) + "," + (divisaoDoVeiculo.Andar ?? 0);

                if (!linhasNaColuna.ContainsKey(chaveDicionario))
                {
                    linhasNaColuna[chaveDicionario] = 1;
                }

                var divisao = new Divisao
                {
                    Codigo = divisaoDoVeiculo.Codigo,
                    Andar = divisaoDoVeiculo.Andar ?? 1,
                    Linha = linhasNaColuna[chaveDicionario] ?? 1,
                    Coluna = divisaoDoVeiculo.Coluna ?? 1,
                    Quantidade = pedidoProdutoDivisaoCapacidade?.Quantidade ?? 0,
                    QuantidadePlanejada = pedidoProdutoDivisaoCapacidade?.QuantidadePlanejada ?? 0,
                    Capacidade = divisaoDoVeiculo.Capacidade,
                    //UnidadeDeMedida = new Dominio.ObjetosDeValor.NovoApp.Cargas.UnidadeDeMedida
                    //{
                    //    Codigo = divisaoDoVeiculo.UnidadeDeMedida?.Codigo ?? 0,
                    //    UnidadeMedida = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.UnidadeMedida ?? Dominio.Enumeradores.UnidadeMedida.UN,
                    //    Descricao = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Descricao ?? ""
                    //}
                };

                // Validando a quantidade de andares, colunas e linhas
                if ((divisaoDoVeiculo.Andar ?? 1) > numeroAndares)
                {
                    numeroAndares = divisaoDoVeiculo.Andar ?? 1;
                }

                if ((divisaoDoVeiculo.Coluna ?? 1) > numeroColunas)
                {
                    numeroColunas = divisaoDoVeiculo.Coluna ?? 1;
                }

                if (linhasNaColuna[chaveDicionario] > numeroLinhas)
                {
                    numeroLinhas = linhasNaColuna[chaveDicionario] ?? 1;
                }

                divisoes.Add(divisao);
                linhasNaColuna[chaveDicionario] += 1;
            }

            return divisoes;
        }

        private string GetJanelaDescarga(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataEntrega = cargaEntrega.DataReprogramada ?? cargaEntrega.DataPrevista ?? DateTime.Now;
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> janelasDescarga = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterJanelaDescarregamento(cargaEntrega, dataEntrega, unitOfWork);
            return janelasDescarga != null ? string.Join(", ", (from janelaDescarga in janelasDescarga select janelaDescarga?.HoraInicio.ToString(@"hh\:mm") + " - " + janelaDescarga?.HoraTermino.ToString(@"hh\:mm"))) : string.Empty;
        }

        private Endereco GetEnderecoCliente(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (cargaEntrega.ClienteOutroEndereco != null)
            {
                // Localidade de entrega. Por algum motivo, se chama "ClienteOutroEndereco", mas é a localidade da entrega
                return new Endereco
                {
                    Latitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(cargaEntrega.ClienteOutroEndereco?.Latitude.Replace(",", "")),
                    Longitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(cargaEntrega.ClienteOutroEndereco?.Longitude.Replace(",", "")),
                    Cidade = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Descricao,
                    Uf = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Estado.Descricao,
                };
            }

            // Endereço da localidade normal do cliente
            return new Endereco
            {
                Latitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(cargaEntrega.Cliente?.Latitude.Replace(",", "")),
                Longitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(cargaEntrega.Cliente?.Longitude.Replace(",", "")),
                Cidade = cargaEntrega?.Cliente?.Localidade?.Descricao,
                Uf = cargaEntrega?.Cliente?.Localidade?.Estado.Descricao,
            };
        }

        private void GetNotasECanhotos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscaisParada, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> todosCanhotos, out List<Nota> notas, out List<Canhoto> canhotos)
        {
            notas = new List<Nota>();
            canhotos = new List<Canhoto>();
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntrega = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            bool contemAvulso = false;

            if (cargaEntrega.Coleta)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscaisParada)
            {
                notas.Add(new Nota
                {
                    Codigo = cargaEntregaNotaFiscal.Codigo,
                    Chave = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                    NumeroNota = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                    Devolvida = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida,
                    DevolvidaParcial = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial,
                });

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntregaFitrados = (from obj in todosCanhotos
                                                                                               where
                                                                          (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)
                                                                          || (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo))
                                                                          || (obj.TipoCanhoto == TipoCanhoto.CTe && cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.CTes.Any(c => c.CargaCTe.CTe.Codigo == obj.CargaCTe.CTe.Codigo))
                                                                                               select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosEntregaFitrados)
                {
                    if (canhoto != null && !canhotosEntrega.Contains(canhoto))
                    {
                        canhotosEntrega.Add(canhoto);
                        if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                            contemAvulso = true;
                    }
                }
            }

            if (contemAvulso)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosAjustados = (from obj in canhotosEntrega where obj.TipoCanhoto == TipoCanhoto.Avulso select obj).ToList();
                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> canhotosAvulsos = (from obj in canhotosEntrega where obj.TipoCanhoto == TipoCanhoto.Avulso select obj.CanhotoAvulso).ToList();
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosNota = (from obj in canhotosEntrega where obj.TipoCanhoto == TipoCanhoto.NFe select obj).ToList();
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosCTe = (from obj in canhotosEntrega where obj.TipoCanhoto == TipoCanhoto.CTe select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoNota in canhotosNota)
                {
                    bool adicionar = true;
                    foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso in canhotosAvulsos)
                    {
                        if (canhotoAvulso.PedidosXMLNotasFiscais.Any(obj => obj.XMLNotaFiscal.Codigo == canhotoNota.XMLNotaFiscal?.Codigo))
                        {
                            adicionar = false;
                            continue;
                        }
                    }
                    if (adicionar)
                        canhotosAjustados.Add(canhotoNota);
                }
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoCTe in canhotosCTe)
                {
                    bool adicionar = true;
                    foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso in canhotosAvulsos)
                    {
                        if (canhotoAvulso.PedidosXMLNotasFiscais.Any(obj => obj.CTes.Any(c => c.CargaCTe.CTe?.Codigo == canhotoCTe.CargaCTe?.CTe?.Codigo)))
                        {
                            adicionar = false;
                            continue;
                        }
                    }
                    if (adicionar)
                        canhotosAjustados.Add(canhotoCTe);
                }

                canhotosEntrega = canhotosAjustados;
            }

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoEntrega in canhotosEntrega)
            {
                canhotos.Add(new Canhoto
                {
                    Codigo = canhotoEntrega.Codigo,
                    Numero = canhotoEntrega.Numero,
                    TipoCanhoto = canhotoEntrega.TipoCanhoto,
                    DigitalizacaoCanhotoInteiro = canhotoEntrega.XMLNotaFiscal?.Emitente?.DigitalizacaoCanhotoInteiro ?? false,
                    Identificacao = canhotoEntrega.Identificacao,
                });
            }
        }

        private List<JustificativaTemperatura> ObterJustificativasTemperaturas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura> listaJustificativasTemperatura = repJustificativaTemperatura.BuscarAtivos();

            return (from o in listaJustificativasTemperatura
                    select new JustificativaTemperatura
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                    }).ToList();
        }

        private List<MotivoRejeicaoEntrega> ObterMotivosRejeicaoEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarOcorrenciasMobile(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega, true);

            return (from o in tiposDeOcorrenciaDeCTe
                    select new MotivoRejeicaoEntrega
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                        AguardarTratativa = o.MotivoChamado != null ? true : false,
                        ObrigarFoto = o.AnexoObrigatorio,
                        Devolucao = o.MotivoChamado != null ? o.MotivoChamado.TipoMotivoAtendimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Devolucao : false,
                        CodigoTipoOperacaoEntrega = o.TipoOperacaoColeta?.Codigo ?? 0,
                        NaoAlterarSituacaoColetaEntrega = o.NaoAlterarSituacaoColetaEntrega,
                        Canais = o.CanaisDeEntrega != null ? o.CanaisDeEntrega.Select(x => new Dominio.ObjetosDeValor.NovoApp.Cargas.CanalDeEntrega { Codigo = x.Codigo, Descricao = x.Descricao }).ToList() : new List<CanalDeEntrega>(),
                        ObrigarInformarValorNaLiberacao = o.MotivoChamado?.ExigeValorNaLiberacao ?? false,
                        ObrigarMotoristaInformarMultiMobile = o.MotivoChamado?.ObrigarMotoristaInformarMultiMobile ?? false,
                    }).ToList();
        }

        private List<MotivoRejeicaoColeta> ObterMotivosRejeicaoColeta(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarOcorrenciasMobile(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta, true);

            return (from o in tiposDeOcorrenciaDeCTe
                    select new MotivoRejeicaoColeta
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                        CodigoTipoOperacaoColeta = o.TipoOperacaoColeta?.Codigo ?? 0,
                        NaoAlterarSituacaoColetaEntrega = o.NaoAlterarSituacaoColetaEntrega,
                        AguardarTratativa = o.MotivoChamado != null ? true : false
                    }).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta> ObterMotivosRetificacaoColeta(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> listaMotivosRetificacaoColeta = repMotivoRetificacaoColeta.BuscarAtivos(TipoAplicacaoColetaEntrega.Coleta);

            return (from o in listaMotivosRetificacaoColeta
                    select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                        TipoOperacao = o.TipoOperacao != null ?
                             (
                                 new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoOperacao()
                                 {
                                     Codigo = o.TipoOperacao.Codigo,
                                     Descricao = o.TipoOperacao.Descricao
                                 }
                             ) : null,
                    }).ToList();
        }

        private List<TipoEvento> ObterTiposEventos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentoEventos = repMonitoramentoEvento.BuscarTodosMobile();

            return (from o in monitoramentoEventos
                    select new TipoEvento
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                    }).ToList();
        }

        private List<MotivoFalhaGta> ObterMotivosFalhaGta(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);
            List<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> listaMotivosFalhaGTA = repMotivoFalhaGTA.BuscarAtivos();

            return (from o in listaMotivosFalhaGTA
                    select new MotivoFalhaGta
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                        ExigirFotoGTA = o.ExigirFotoGTA,
                    }).ToList();
        }

        private List<MotivoFalhaNotaFiscal> ObterMotivosFalhaNotaFiscal(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);
            var listaMotivosFalhaNotaFiscal = repMotivoFalhaNotaFiscal.BuscarAtivos();

            return (from o in listaMotivosFalhaNotaFiscal
                    select new MotivoFalhaNotaFiscal
                    {
                        Codigo = o.Codigo,
                        Descricao = o.Descricao,
                    }).ToList();
        }

        private List<RegraReconhecimentoCanhoto> ObterRegrasReconhecimentoCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> configuracoes = repConfiguracaoReconhecimentoCanhoto.BuscarAtivos();

            return (from o in configuracoes
                    select new RegraReconhecimentoCanhoto
                    {
                        Codigo = o.Codigo,
                        PalavrasChaves = o.PalavrasChaves.Split(';').Where(p => p.Length > 0).ToList()
                    }).ToList();
        }

        private Veiculo ObterVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            // Se tem algum reboque, então o ModeloVeicular que queremos é o dele. Se não, é o modelo normal do veículo padrão da carga mesmo
            var modeloVeicularCarga = carga.VeiculosVinculados.Count > 0 ? carga.VeiculosVinculados.ToList()[0].ModeloVeicularCarga : carga.Veiculo?.ModeloVeicularCarga ?? null;

            var divisoes = new List<DivisaoCapacidadeModeloVeicular>();

            // Esse dicionário guarda quantas linhas tem em cada coluna
            Dictionary<string, int?> linhasNaColuna = new Dictionary<string, int?>();

            int numeroAndares = 0;
            int numeroColunas = 0;
            int numeroLinhas = 1;

            if (modeloVeicularCarga != null && modeloVeicularCarga.DivisoesCapacidade.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade modeloVeicularCargaDivisaoCapacidade in modeloVeicularCarga.DivisoesCapacidade.ToList())
                {
                    string chaveDicionario = (modeloVeicularCargaDivisaoCapacidade.Coluna ?? 0) + "," + (modeloVeicularCargaDivisaoCapacidade.Piso ?? 0);

                    if (!linhasNaColuna.ContainsKey(chaveDicionario))
                    {
                        linhasNaColuna[chaveDicionario] = 1;
                    }

                    DivisaoCapacidadeModeloVeicular divisaoCapacidadeModelo = new DivisaoCapacidadeModeloVeicular();
                    divisaoCapacidadeModelo.Capacidade = modeloVeicularCargaDivisaoCapacidade.Quantidade;
                    divisaoCapacidadeModelo.Codigo = modeloVeicularCargaDivisaoCapacidade.Codigo;
                    divisaoCapacidadeModelo.Descricao = modeloVeicularCargaDivisaoCapacidade.Descricao;
                    divisaoCapacidadeModelo.Coluna = modeloVeicularCargaDivisaoCapacidade.Coluna ?? 0;
                    divisaoCapacidadeModelo.Linha = linhasNaColuna[chaveDicionario] ?? 1;
                    divisaoCapacidadeModelo.Andar = modeloVeicularCargaDivisaoCapacidade.Piso ?? 0;

                    divisoes.Add(divisaoCapacidadeModelo);

                    // Validando a quantidade de andares, colunas e linhas
                    if ((modeloVeicularCargaDivisaoCapacidade.Piso ?? 0) > numeroAndares)
                    {
                        numeroAndares = modeloVeicularCargaDivisaoCapacidade.Piso ?? 0;
                    }

                    if ((modeloVeicularCargaDivisaoCapacidade.Coluna ?? 0) > numeroColunas)
                    {
                        numeroColunas = modeloVeicularCargaDivisaoCapacidade.Coluna ?? 0;
                    }

                    if (linhasNaColuna[chaveDicionario] > numeroLinhas)
                    {
                        numeroLinhas = linhasNaColuna[chaveDicionario] ?? 1;
                    }

                    linhasNaColuna[chaveDicionario] += 1;
                }
            }

            return new Veiculo
            {
                Placa = carga.Veiculo?.Placa ?? "",
                NumeroAndares = numeroAndares,
                NumeroLinhas = numeroLinhas,
                NumeroColunas = numeroColunas,
                Divisoes = divisoes,
            };
        }

        private List<Ocorrencia> ObterOcorrencias(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarListaPorCargaEntrega(cargaEntrega.Codigo);

            return (from o in chamados
                    select new Ocorrencia
                    {
                        Codigo = o.Codigo,
                        Numero = o.Numero,
                        DevolucaoParcial = o.DevolucaoParcial,
                        Observacao = o.Observacao,
                        data = o.DataRegistroMotorista?.ToUnixSeconds() ?? o.DataCriacao.ToUnixSeconds(),
                        Situacao = o.Situacao,
                        TratativaDevolucao = o.TratativaDevolucao.ObterDescricaoTratativaDevolucao(),
                    }).ToList();
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas ObterModalidadeFornecedorPessoas(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, cargaEntrega.Cliente.CPF_CNPJ);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = null;

            if (modalidadePessoas != null)
                modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            return modalidadeFornecedorPessoas;
        }

        private void AtualizarAceitePedidosCarga(EnumAceiteMotorista statusAceite, int codigoCarga, Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCarga(codigoCarga);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = TipoAuditado.Usuario,
                OrigemAuditado = OrigemAuditado.WebServiceMobile
            };

            if (pedidos != null)
            {
                foreach (var pedido in pedidos)
                {
                    pedido.AceiteMotorista = statusAceite;
                    repPedido.Atualizar(pedido);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, $"Escala do pedido atualizada pelo motorista {motorista.Descricao} via app", unitOfWork);
                }
            }
        }

        #endregion
    }
}
