using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.GestaoViagem;
using Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Servicos.Embarcador.Integracao.AngelLira
{
    public class IntegrarCargaAngelLira : IntegracaoClientBase<WSAngelLiraStatus.WSStatusSoapClient, WSAngelLiraStatus.WSStatusSoap>
    {
        /// <summary>
        /// Documentação
        /// </summary>
        /// <param name="link">http://www.angellira.com.br:443/webservices/app_def/</param>

        #region Métodos Públicos Estáticos


        public static Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma ObterComprovanteAgendamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma retornoRotograma = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma();
            retornoRotograma.status = false;
            string url = "";
            WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader = ObterCabecalhoStatus(unitOfWork, out url);

            if (validationSoapHeader != null)
            {
                WSAngelLiraStatus.WSStatusSoapClient wsStatusSoapClient = ObterWSStatusClient(url);
                InspectorBehavior inspector = new InspectorBehavior();
                wsStatusSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

                WSAngelLiraStatus.IdentificadorViagem viagem = new WSAngelLiraStatus.IdentificadorViagem();

                viagem.V_Carga = ObterVCarga(false, carga, null, tipoServicoMultisoftware, unitOfWork);

                if (carga.ProcedimentoEmbarque != null)
                    viagem.V_ProcedimentoEmbarque = carga.ProcedimentoEmbarque.IntegracaoProcedimentoEmbarque;
                else
                    viagem.V_ProcedimentoEmbarque = ObterProcedimentoEmbarque(carga, 0, unitOfWork);

                WSAngelLiraStatus.RetornoItem retornoItem = wsStatusSoapClient.GetComprovanteAgendamento(validationSoapHeader, viagem);

                if (retornoItem.status == 0)
                {
                    retornoRotograma.status = true;
                    retornoRotograma.link = retornoItem.descricao;
                }
                else
                {
                    Servicos.Log.TratarErro(inspector.LastRequestXML);
                    Servicos.Log.TratarErro(inspector.LastResponseXML);
                }
                retornoRotograma.mensagemErro = retornoItem.motivo;
            }
            else
            {
                retornoRotograma.mensagemErro = "Não foram configurados os dados de integração com a AngelLira";
            }
            return retornoRotograma;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma ObterRotograma(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma retornoRotograma = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma();
            retornoRotograma.status = false;
            string url = "";
            WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader = ObterCabecalhoStatus(unitOfWork, out url);

            if (validationSoapHeader != null)
            {
                WSAngelLiraStatus.WSStatusSoapClient wsStatusSoapClient = ObterWSStatusClient(url);
                InspectorBehavior inspector = new InspectorBehavior();
                wsStatusSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

                WSAngelLiraStatus.IdentificadorViagem viagem = new WSAngelLiraStatus.IdentificadorViagem();

                viagem.V_Carga = ObterVCarga(false, carga, null, tipoServicoMultisoftware, unitOfWork);

                if (carga.ProcedimentoEmbarque != null)
                    viagem.V_ProcedimentoEmbarque = carga.ProcedimentoEmbarque.IntegracaoProcedimentoEmbarque;
                else
                    viagem.V_ProcedimentoEmbarque = ObterProcedimentoEmbarque(carga, 0, unitOfWork);

                WSAngelLiraStatus.RetornoItem retornoItem = wsStatusSoapClient.GetRotograma(validationSoapHeader, viagem);

                if (retornoItem.status == 0)
                {
                    retornoRotograma.status = true;
                    retornoRotograma.link = retornoItem.descricao;
                }
                else
                {
                    Servicos.Log.TratarErro(inspector.LastRequestXML);
                    Servicos.Log.TratarErro(inspector.LastResponseXML);
                }
                retornoRotograma.mensagemErro = retornoItem.motivo;
            }
            else
            {
                retornoRotograma.mensagemErro = "Não foram configurados os dados de integração com a AngelLira";
            }
            return retornoRotograma;
        }

        public static void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = "";
            WSAngelLira.ValidationSoapHeader validationSoapHeader = ObterCabecalho(unitOfWork, out url);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            if (validationSoapHeader != null)
            {
                WSAngelLira.WSImportSoapClient wSImportSoapClient = ObterWSImportClient(url);
                InspectorBehavior inspector = new InspectorBehavior();
                wSImportSoapClient.Endpoint.EndpointBehaviors.Add(inspector);
                WSAngelLira.Cancelamento cancelamento = new WSAngelLira.Cancelamento();

                var repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                var cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCarga(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo)
                    .Where(x => x.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).FirstOrDefault();

                bool bOrigemTransporteDaCarga = false;
                if (cargaDadosTransporteIntegracao != null)
                    bOrigemTransporteDaCarga = true;

                cancelamento.V_Carga = ObterVCarga(bOrigemTransporteDaCarga, cargaCancelamentoIntegracao.CargaCancelamento.Carga, null, tipoServicoMultisoftware, unitOfWork);

                if (cargaCancelamentoIntegracao.CargaCancelamento.Carga.ProcedimentoEmbarque != null)
                    cancelamento.V_ProcedimentoEmbarque = cargaCancelamentoIntegracao.CargaCancelamento.Carga.ProcedimentoEmbarque.IntegracaoProcedimentoEmbarque;
                else
                    cancelamento.V_ProcedimentoEmbarque = ObterProcedimentoEmbarque(cargaCancelamentoIntegracao.CargaCancelamento.Carga, 0, unitOfWork);

                List<WSAngelLira.Cancelamento> cancelamentos = new List<WSAngelLira.Cancelamento>();
                cancelamentos.Add(cancelamento);

                try
                {

                    WSAngelLira.RetornoViagem[] retornos = wSImportSoapClient.CancelarViagem(validationSoapHeader, cancelamentos.ToArray());

                    for (int i = 0; i < retornos.Length; i++)
                    {
                        WSAngelLira.RetornoViagem retorno = retornos[i];

                        cargaCancelamentoIntegracao.ProblemaIntegracao = retorno.status.ToString() + " - " + retorno.motivo;
                        cargaCancelamentoIntegracao.NumeroTentativas++;

                        if (retorno.status == 0)
                        {
                            cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaCancelamentoIntegracao.Protocolo = retorno.codigo.ToString();
                        }
                        else
                            cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                catch (Exception ex)
                {
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a AngelLira.";
                    cargaCancelamentoIntegracao.NumeroTentativas++;
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    Servicos.Log.TratarErro(ex);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    Mensagem = cargaCancelamentoIntegracao.ProblemaIntegracao,
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }
            else
            {
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não foram configurados os dados de integração com a AngelLira.";
                cargaCancelamentoIntegracao.NumeroTentativas++;
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            }

            repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = "";
            WSAngelLira.ValidationSoapHeader validationSoapHeader = ObterCabecalho(unitOfWork, out url);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            if (validationSoapHeader != null)
            {
                try
                {
                    string mensagem = "";
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento agendamento = ObterViagem(cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                    if (agendamento != null)
                    {
                        WSAngelLira.WSImportSoapClient wSImportSoapClient = ObterWSImportClient(url);

                        string xml = Servicos.XML.ConvertObjectToXMLString(agendamento);
                        byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                        MemoryStream stream = new MemoryStream(byteArray);
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(stream);

                        InspectorBehavior inspector = new InspectorBehavior();
                        wSImportSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

                        WSAngelLira.RetornoViagem[] retornos = wSImportSoapClient.AgendarViagem(validationSoapHeader, xmlDocument);

                        for (int i = 0; i < retornos.Length; i++)
                        {

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

                            WSAngelLira.RetornoViagem retorno = retornos[i];

                            cargaIntegracao.ProblemaIntegracao = retorno.status.ToString() + " - " + retorno.motivo;
                            cargaIntegracao.NumeroTentativas++;
                            if (retorno.status == 0 || retorno.status == 999)
                            {
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                cargaIntegracao.Protocolo = retorno.codigo.ToString();
                            }
                            else
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                            arquivoIntegracao.Data = DateTime.Now;

                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        }

                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = mensagem.Left(300);
                        cargaIntegracao.NumeroTentativas++;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                catch (Exception ex)
                {
                    cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a AngelLira.";
                    cargaIntegracao.NumeroTentativas++;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    Servicos.Log.TratarErro(ex);
                }
            }
            else
            {
                cargaIntegracao.ProblemaIntegracao = "Não foram configurados os dados de integração com a AngelLira";
                cargaIntegracao.NumeroTentativas++;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            }
            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            WSAngelLira.ValidationSoapHeader validationSoapHeader = ObterCabecalho(unitOfWork, out string url);

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            if (validationSoapHeader == null)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Não foram configurados os dados de integração com a AngelLira";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

                return;
            }

            string mensagem = "";

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento agendamento = ObterViagem(cargaDadosTransporteIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);

            if (agendamento == null)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                WSAngelLira.WSImportSoapClient wSImportSoapClient = ObterWSImportClient(url);

                string xml = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento>(agendamento);
                byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                MemoryStream stream = new MemoryStream(byteArray);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);

                wSImportSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

                WSAngelLira.RetornoViagem[] retornos = wSImportSoapClient.AgendarViagem(validationSoapHeader, xmlDocument);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = string.Join(", ", retornos.Select(retorno => retorno.status.ToString() + " - " + retorno.motivo));

                if (retornos.Any(o => o.status == 0 || o.status == 999))
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.Protocolo = retornos.Where(o => o.codigo > 0).Select(o => o.codigo.ToString()).FirstOrDefault();
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a AngelLira.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                Servicos.Log.TratarErro(ex);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                Mensagem = cargaDadosTransporteIntegracao.ProblemaIntegracao,
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = cargaDadosTransporteIntegracao.DataIntegracao,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        private static bool ExisteIntegracaoTemperaturaAngelira(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            var integracaoAngelLira = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira);

            if (integracaoAngelLira != null)
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoTemperaturaAngelLira = repIntegracaoAngelLira.Buscar();

                return integracaoTemperaturaAngelLira?.IntegracaoTemperatura ?? false;
            }

            return false;
        }

        private static decimal ObterUltimaTemperaturaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            return 0;

            //Habilitar apos criacao da V_UltimaTemperatura pela angellira 
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = "";
            Servicos.WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader = ObterCabecalhoStatus(unitOfWork, out url);

            //Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
                return 0;


            if (validationSoapHeader != null)
            {
                WSAngelLiraStatus.WSStatusSoapClient wSImportSoapClient = ObterWSStatusClient(url);

                Servicos.WSAngelLiraStatus.StatusViagem status = wSImportSoapClient.GetStatusViagem(validationSoapHeader, carga.CodigoCargaEmbarcador);


                //'V_UltimaTemperatura
                return -3;

            }

            return -0;
        }

        public static void IntegrarUltimaTemperatura(Repositorio.UnitOfWork unitOfWork)
        {
            if (!ExisteIntegracaoTemperaturaAngelira(unitOfWork))
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosPatio = repFluxoGestaoPatio.BuscarIntegracaoTemperatura();

            foreach (var fluxoPatio in fluxosPatio)
            {
                try
                {
                    fluxoPatio.Temperatura = ObterUltimaTemperaturaCarga(fluxoPatio.Carga, unitOfWork);

                    repFluxoGestaoPatio.Atualizar(fluxoPatio);
                }
                catch
                {

                }

            }

        }

        //public static void IntegrarFaixaTemperatura(Repositorio.UnitOfWork unitOfWork)
        //{
        //    if (!ExisteIntegracaoTemperaturaAngelira(unitOfWork))
        //        return;


        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    string url = "";
        //    WSAngelLira.ValidationSoapHeader validationSoapHeader = ObterCabecalho(unitOfWork, out url);

        //    Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

        //    Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
        //    List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repFaixaTemperatura.BuscarTodos();

        //    List<int> procedimentosEmbarque = repTipoOperacao.BuscarCodigosProcedimentoEmbarque();


        //    if (validationSoapHeader != null)
        //    {
        //        WSAngelLira.WSImportSoapClient wSImportSoapClient = ObterWSImportClient(url);

        //        foreach (var procedimentoEmbarque in procedimentosEmbarque)
        //        {
        //            try
        //            {
        //                Servicos.WSAngelLira.RetornoItem[] retornos = wSImportSoapClient.GetFaixaTemperatura(validationSoapHeader, procedimentoEmbarque);

        //                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = repTipoOperacao.BuscarPorProcedimentoEmbarque(procedimentoEmbarque);

        //                foreach (var retorno in retornos)
        //                {

        //                    foreach (var tipoOperacao in tiposOperacao)
        //                    {

        //                        Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = faixasTemperatura
        //                            .Where(f => f.CodigoIntegracao == retorno.codigo.ToString() && f.TipoOperacao.Codigo == tipoOperacao.Codigo).FirstOrDefault();

        //                        if (faixaTemperatura == null)
        //                        {
        //                            var novaFaixaTemperatura = new Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura
        //                            {
        //                                CodigoIntegracao = retorno.codigo.ToString(),
        //                                Descricao = retorno.descricao,
        //                                TipoOperacao = tipoOperacao
        //                            };

        //                            repFaixaTemperatura.Inserir(novaFaixaTemperatura);
        //                        }
        //                        else
        //                        {
        //                            faixaTemperatura.Descricao = retorno.descricao;
        //                            repFaixaTemperatura.Atualizar(faixaTemperatura);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //                Servicos.Log.TratarErro(ex);
        //            }
        //        }

        //    }
        //    else
        //    {
        //        Servicos.Log.TratarErro("Não foram configurados os dados de integração com a AngelLira");

        //    }

        //}

        public static void IntegrarFaixaTemperatura(Repositorio.UnitOfWork unitOfWork)
        {
            if (!ExisteIntegracaoTemperaturaAngelira(unitOfWork))
                return;


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = "";
            WSAngelLira.ValidationSoapHeader validationSoapHeader = ObterCabecalho(unitOfWork, out url);

            //Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);

            Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repFaixaTemperatura.BuscarTodos();

            List<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> procedimentoEmbarques = repProcedimentoEmbarque.BuscarTodosAtivos();

            List<int> procedimentosEmbarque = (from obj in procedimentoEmbarques select obj.IntegracaoProcedimentoEmbarque).Distinct().ToList(); // repTipoOperacao.BuscarCodigosProcedimentoEmbarque();


            if (validationSoapHeader != null)
            {
                WSAngelLira.WSImportSoapClient wSImportSoapClient = ObterWSImportClient(url);

                foreach (var procedimentoEmbarque in procedimentosEmbarque)
                {
                    try
                    {
                        Servicos.WSAngelLira.RetornoItem[] retornos = wSImportSoapClient.GetFaixaTemperatura(validationSoapHeader, procedimentoEmbarque);

                        //List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = repTipoOperacao.BuscarPorProcedimentoEmbarque(procedimentoEmbarque);

                        List<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> procedimentos = (from obj in procedimentoEmbarques where obj.IntegracaoProcedimentoEmbarque == procedimentoEmbarque select obj).ToList();

                        foreach (var retorno in retornos)
                        {

                            foreach (var procedimento in procedimentos)
                            {

                                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = faixasTemperatura
                                    .Where(f => f.CodigoIntegracao == retorno.codigo.ToString() && f.ProcedimentoEmbarque.Codigo == procedimento.Codigo).FirstOrDefault();

                                if (faixaTemperatura == null)
                                {
                                    var novaFaixaTemperatura = new Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura
                                    {
                                        CodigoIntegracao = retorno.codigo.ToString(),
                                        Descricao = retorno.descricao,
                                        ProcedimentoEmbarque = procedimento,
                                        DataUltimaModificacao = DateTime.Now
                                    };

                                    repFaixaTemperatura.Inserir(novaFaixaTemperatura);
                                }
                                else
                                {
                                    if (faixaTemperatura.Descricao != retorno.descricao)
                                    {
                                        faixaTemperatura.Descricao = retorno.descricao;
                                        faixaTemperatura.DataUltimaModificacao = DateTime.Now;
                                        repFaixaTemperatura.Atualizar(faixaTemperatura);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Servicos.Log.TratarErro(ex);
                    }
                }

            }
            else
            {
                Servicos.Log.TratarErro("Não foram configurados os dados de integração com a AngelLira");

            }

        }

        #endregion

        #region Métodos Privados Estáticos

        public static WSAngelLiraStatus.WSStatusSoapClient ObterWSStatusClient(string url)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "WSStatus.asmx";

            WSAngelLiraStatus.WSStatusSoapClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                client = new WSAngelLiraStatus.WSStatusSoapClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                client = new WSAngelLiraStatus.WSStatusSoapClient(binding, endpointAddress);
            }

            return client;
        }

        public static WSAngelLira.WSImportSoapClient ObterWSImportClient(string url)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "WSImport.asmx";

            WSAngelLira.WSImportSoapClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                client = new WSAngelLira.WSImportSoapClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                client = new WSAngelLira.WSImportSoapClient(binding, endpointAddress);
            }

            return client;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento ObterViagem(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao.Carga;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = repIntegracaoAngelLira.Buscar();

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento agendamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento();
            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarEntregasPorCarga(carga.Codigo);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (integracaoAngelLira.UtilizarDataAgendamentoPedido)
                    cargaPedidos = cargaPedidos.OrderBy(o => o.Pedido.DataAgendamento).ToList();
                else
                    cargaPedidos = cargaPedidos.OrderBy(o => o.Pedido.DataPrevisaoSaida).ToList();
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = cargaPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoUltimo = cargaPedidos.LastOrDefault();

            if (carga.CargaAgrupada)
            {
                cargaPedidoPrimeiro = (from obj in cargaPedidos where obj.CargaOrigem.FaixaTemperatura?.Codigo == carga.FaixaTemperatura?.Codigo select obj).FirstOrDefault();
                if (cargaPedidoPrimeiro == null)
                    cargaPedidoPrimeiro = cargaPedidos.FirstOrDefault();

                cargaPedidoUltimo = (from obj in cargaPedidos where obj.CargaOrigem.FaixaTemperatura?.Codigo == carga.FaixaTemperatura?.Codigo select obj).LastOrDefault();
                if (cargaPedidoUltimo == null)
                    cargaPedidoUltimo = cargaPedidos.LastOrDefault();
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            viagem.V_Tipo = 2;

            viagem.V_Carga = ObterVCarga(false, carga, integracaoAngelLira, tipoServicoMultisoftware, unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (carga?.FaixaTemperatura != null)
                {
                    int.TryParse(carga?.FaixaTemperatura?.CodigoIntegracao, out int codigoFaixaTemperatura);

                    if (codigoFaixaTemperatura > 0)
                        viagem.V_FaixaTemperatura = codigoFaixaTemperatura;
                }

            }
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = carga.FaixaTemperatura ?? carga.TipoDeCarga?.FaixaDeTemperatura;

                if (faixaTemperatura != null &&
                    int.TryParse(faixaTemperatura.CodigoIntegracao, out int codigoFaixaTemperatura) &&
                    codigoFaixaTemperatura > 0)
                    viagem.V_FaixaTemperatura = codigoFaixaTemperatura;
            }

            var observacoes = cargaPedidos.Select(obj => obj.Pedido.Observacao).ToList();
            viagem.V_Observacao = observacoes.Any() ? string.Join("/", observacoes) : string.Empty;

            int codigoRota = 0;
            //string polilinha = "";

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete == null && carga.Rota == null)
            {
                Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorRemetenteDestino(cargaPedidoPrimeiro.Pedido.Remetente.CPF_CNPJ, cargaPedidoPrimeiro.Pedido.Destinatario.CPF_CNPJ);

                if (rota != null && !string.IsNullOrWhiteSpace(rota.DescricaoRotaSemParar))
                    int.TryParse(Utilidades.String.OnlyNumbers(rota.DescricaoRotaSemParar), out codigoRota);
            }
            else
            {
                int.TryParse(Utilidades.String.OnlyNumbers(carga.Rota?.CodigoIntegracao), out codigoRota);

                //carrefour solicitou o cancelamento do envio da polilinha para a angellira #11716
                //if (cargaRotaFrete != null)
                //    polilinha = cargaRotaFrete.PolilinhaRota;
                //else
                //    polilinha = carga.Rota?.PolilinhaRota;
            }

            //if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !string.IsNullOrWhiteSpace(polilinha))
            //    viagem.V_Polilinha = polilinha;
            //else if (codigoRota > 0)
            //    viagem.V_Rota = codigoRota;

            if (codigoRota > 0 && !integracaoAngelLira.NaoEnviarRotaViagem)
                viagem.V_Rota = codigoRota;

            if (carga.ProcedimentoEmbarque != null)
                viagem.V_ProcedimentoEmbarque = carga.ProcedimentoEmbarque.IntegracaoProcedimentoEmbarque;
            else
                viagem.V_ProcedimentoEmbarque = ObterProcedimentoEmbarque(carga, 1, unitOfWork);

            if (carga.ProcedimentoEmbarque != null)
                viagem.V_Modelo_Contratacao = carga.ProcedimentoEmbarque.CodigoModeloContratacao;
            else
                viagem.V_Modelo_Contratacao = carga.TipoOperacao?.CodigoModeloContratacao ?? 0;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if ((cargaPedidoPrimeiro.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor ||
                    cargaPedidoPrimeiro.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedidoPrimeiro.Expedidor != null)
                    viagem.V_Origem = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoPrimeiro.Expedidor, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                else
                    viagem.V_Origem = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoPrimeiro.Pedido.Remetente, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
            }
            else
            {
                viagem.V_Origem = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoPrimeiro.Pedido.Remetente, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
            }

            if (!string.IsNullOrWhiteSpace(mensagem))
                return null;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if ((cargaPedidoUltimo.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                    cargaPedidoUltimo.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedidoUltimo.Recebedor != null)
                    viagem.V_Destino = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoUltimo.Recebedor, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                else
                    viagem.V_Destino = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoUltimo.Pedido.Destinatario, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;
            }
            else
            {
                int codigoTipoCirucuito = 1;
                if (viagem.V_Modelo_Contratacao == codigoTipoCirucuito)
                {
                    viagem.V_Destino = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoPrimeiro.Pedido.Destinatario, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                    if (!string.IsNullOrWhiteSpace(mensagem))
                        return null;
                }
                else
                {
                    viagem.V_Destino = viagem.V_Origem;
                }

                if (carga.FaixaTemperatura != null)//regra fixa carrefour se necessário tratar outro embarcador criar configuração.
                {
                    viagem.V_Origem += "-" + carga.FaixaTemperatura.Descricao;

                    if (viagem.V_Modelo_Contratacao != codigoTipoCirucuito)
                        viagem.V_Destino += "-" + carga.FaixaTemperatura.Descricao;
                }
            }
            VerificaDestino(cargaPedidoPrimeiro, ref viagem, integracaoAngelLira.AplicarRegraLocalPalletizacao);
            viagem.V_Valor = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
          
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCarga(carga.Codigo);

            DateTime? dataPrevisaoInicioViagem = null, dataPrevisaoFimViagem = null;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (integracaoAngelLira.UtilizarDataAtualETempoRotaParaInicioEFimViagem)
                {
                    dataPrevisaoInicioViagem = DateTime.Now;
                    dataPrevisaoFimViagem = dataPrevisaoInicioViagem.Value.AddMinutes(cargaIntegracao.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0);

                    viagem.V_PrevInicio = dataPrevisaoInicioViagem.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                    viagem.V_PrevFim = dataPrevisaoFimViagem.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                else
                {
                    dataPrevisaoInicioViagem = cargaPedidos.Min(o => o.Pedido.DataPrevisaoSaida) ?? DateTime.Now.AddMinutes(35);
                    dataPrevisaoFimViagem = cargaPedidos.Max(o => o.Pedido.PrevisaoEntrega);

                    viagem.V_PrevInicio = dataPrevisaoInicioViagem?.ToString("yyyy-MM-ddTHH:mm:ss");
                    viagem.V_PrevFim = dataPrevisaoFimViagem?.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }
            else
            {
                viagem.V_PrevInicio = DateTime.Now.AddMinutes(35).ToString("yyyy-MM-ddTHH:mm:ss");
            }

            viagem.V_DataFat = carga.DataFinalizacaoEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            viagem.V_EnviarEmail = "N";
            viagem.V_EnviarSMS = "N";

            if (carga.Motoristas != null && carga.Motoristas.Count > 0)
                viagem.V_Motorista = carga.Motoristas?.FirstOrDefault().CPF_Formatado;

            //viagem.V_Transportador = carga.Empresa.CNPJ_Formatado;
            viagem.V_Codigo_Transportador = carga.Empresa.CNPJ_Formatado;

            if (carga.Veiculo != null)
            {
                if (integracaoAngelLira.EnviarDadosFormatados)
                    viagem.V_Veiculo = carga.Veiculo.Placa.ObterPlacaFormatada();
                else
                    viagem.V_Veiculo = carga.Veiculo.Placa.Insert(3, "-");

                for (int i = 0; i < carga.VeiculosVinculados.Count; i++)
                {
                    Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados.ToList()[i];

                    string placa = string.Empty;

                    if (integracaoAngelLira.EnviarDadosFormatados)
                        placa = reboque.Placa.ObterPlacaFormatada();
                    else
                        placa = reboque.Placa.Insert(3, "-");

                    if (i == 0)
                        viagem.V_Carreta1 = placa;
                    else if (i == 1)
                        viagem.V_Carreta2 = placa;
                    else if (i == 2)
                        viagem.V_Carreta3 = placa;
                    else
                        break;
                }
            }

            if (preCarga != null)
                viagem.V_Vinculo = string.Join(", ", (from obj in preCarga.Pedidos select obj.NumeroPedidoEmbarcador).ToList());

            viagem.V_Peso = repCargaPedido.BuscarPesoTotalPorCarga(carga.Codigo);

            DateTime? dataAgendamento = null;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega> entregas;
            if (integracaoAngelLira.GerarViagensPorPedido || cargaEntregas.Count == 0)
                entregas = ObterEntregasPorCargaPedido(cargaPedidos, viagem, ref mensagem, ref dataAgendamento, preCarga, integracaoAngelLira, cargaIntegracao, tipoServicoMultisoftware, unitOfWork, dataPrevisaoInicioViagem, dataPrevisaoFimViagem);
            else
                entregas = ObterEntregasPorCargaEntrega(carga, cargaEntregas, viagem, ref mensagem, preCarga, integracaoAngelLira, cargaIntegracao, tipoServicoMultisoftware, unitOfWork);

            if (entregas == null)
                return null;

            if (integracaoAngelLira.UtilizarDataAgendamentoPedido && dataAgendamento.HasValue)
                viagem.V_PrevFim = dataAgendamento.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            viagem.Entregas = entregas.ToArray();

            agendamento.Versao = "1.2";
            agendamento.Viagem = viagem;

            return agendamento;
        }

        private static int ObterTempoEntregaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int tempoEntrega = carga?.TipoOperacao?.TempoEntregaAngelLira ?? 0;

            if (carga?.ProcedimentoEmbarque != null)
                tempoEntrega = carga.ProcedimentoEmbarque.TempoEntregaAngelLira;

            if (tempoEntrega <= 0)
                tempoEntrega = 120;

            return tempoEntrega;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega> ObterEntregasPorCargaEntrega(
            Dominio.Entidades.Embarcador.Cargas.Carga carga,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem viagem,
            ref string mensagem,
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga,
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira,
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            Repositorio.UnitOfWork unitOfWork
            )
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega> entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            for (int i = 0; i < cargaEntregas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregas[i];

                Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega
                {
                    E_SeqEntrega = cargaEntrega.Ordem,
                    E_Cliente = CodigoClienteAngelLira(integracaoAngelLira, cargaEntrega.Cliente, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware),
                    E_PrevInicio = viagem.V_PrevInicio,
                    E_PrevFim = viagem.V_PrevFim
                };

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = (from obj in cargaEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).FirstOrDefault();

                if (preCarga != null && cargaEntregaPedido?.CargaPedido.Pedido.DataPrevisaoChegadaDestinatario != null) //regra fixa carrefour se necessário tratar outro embarcador criar configuração.
                    entrega.E_PrevInicio = cargaEntregaPedido.CargaPedido.Pedido.DataPrevisaoChegadaDestinatario.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                else if (cargaEntrega.DataPrevista.HasValue)
                {
                    if (cargaEntrega.DataPrevista.Value > DateTime.Now.AddMinutes(35))
                        entrega.E_PrevInicio = cargaEntrega.DataPrevista.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto> docs = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto>();
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntrega.NotasFiscais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto docto = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto
                    {
                        D_Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                        D_Serie = !string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Serie) ? pedidoXMLNotaFiscal.XMLNotaFiscal.Serie : "1",
                        D_Valor = pedidoXMLNotaFiscal.XMLNotaFiscal.Valor
                    };

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        docto.Produtos = new[]{
                                                    new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Produto
                                                    {
                                                        P_Codigo = carga.TipoDeCarga.NCM,
                                                        P_Valor = pedidoXMLNotaFiscal.XMLNotaFiscal.Valor
                                                    }
                                                };
                    }

                    docs.Add(docto);
                }

                if (docs.Count > 0)
                    entrega.Doctos = docs.ToArray();

                entregas.Add(entrega);
            }

            return entregas;
        }


        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega> ObterEntregasPorCargaPedido(
             List<CargaPedido> cargaPedidos,
             Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem viagem,
             ref string mensagem,
             ref DateTime? dataAgendamento,
             Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga,
             Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira,
             Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao,
             AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
             Repositorio.UnitOfWork unitOfWork,
             DateTime? dataPrevisaoInicialViagem,
             DateTime? dataPrevisaoFinalViagem
             )
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega> entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega>();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = cargaPedidos.FirstOrDefault();
            int tempoEntrega = ObterTempoEntregaPorCarga(cargaPedidoPrimeiro?.Carga);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                CargaPedido cargaPedido = cargaPedidos[i];

                Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega();
                entrega.E_SeqEntrega = i + 1;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                   (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                    cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null)
                    entrega.E_Cliente = CodigoClienteAngelLira(integracaoAngelLira, cargaPedido.Recebedor, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                else
                    entrega.E_Cliente = CodigoClienteAngelLira(integracaoAngelLira, cargaPedido.Pedido.Destinatario, cargaIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (integracaoAngelLira.UtilizarDataAgendamentoPedido)
                    {
                        DateTime? dataAgendamentoPedido = cargaPedido.Pedido.DataAgendamento?.AddMinutes(tempoEntrega);

                        if (dataAgendamentoPedido.HasValue && (!dataAgendamento.HasValue || dataAgendamento < dataAgendamentoPedido))
                            dataAgendamento = dataAgendamentoPedido;

                        entrega.E_PrevInicio = cargaPedido.Pedido.DataAgendamento?.ToString("yyyy-MM-ddTHH:mm:ss");
                        entrega.E_PrevFim = dataAgendamentoPedido?.ToString("yyyy-MM-ddTHH:mm:ss");
                    }
                    else
                    {
                        if (configuracaoTMS.InformarDataViagemExecutadaPedido)
                            entrega.E_PrevInicio = cargaPedido.Pedido.DataInicialViagemExecutada?.ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevInicio;
                        else
                            entrega.E_PrevInicio = cargaPedidoPrimeiro.Pedido.PrevisaoEntrega?.AddMinutes(-tempoEntrega).ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevInicio;

                        if (!dataPrevisaoFinalViagem.HasValue || !cargaPedido.Pedido.PrevisaoEntrega.HasValue || dataPrevisaoFinalViagem >= cargaPedido.Pedido.PrevisaoEntrega)
                            entrega.E_PrevFim = cargaPedido.Pedido.PrevisaoEntrega?.ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevFim;
                        else
                        {
                            entrega.E_PrevInicio = viagem.V_PrevInicio;
                            entrega.E_PrevFim = viagem.V_PrevFim;
                        }
                    }
                }
                else
                {
                    if (preCarga == null)//regra fixa carrefour se necessário tratar outro embarcador criar configuração.
                        entrega.E_PrevInicio = viagem.V_PrevInicio;
                    else
                        entrega.E_PrevInicio = cargaPedido.Pedido.DataPrevisaoChegadaDestinatario?.ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevInicio;
                }

                entrega.E_Volume = cargaPedido.QtVolumes;
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto> docs = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in cargaPedido.NotasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto docto = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto
                    {
                        D_Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                        D_Serie = !string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Serie) ? pedidoXMLNotaFiscal.XMLNotaFiscal.Serie : "1",
                        D_Valor = pedidoXMLNotaFiscal.XMLNotaFiscal.Valor
                    };

                    /*
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        docto.Produtos = new[]{
                                                    new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Produto
                                                    {
                                                        P_Codigo = cargaPedido.Carga.TipoDeCarga.NCM,
                                                        P_Valor = pedidoXMLNotaFiscal.XMLNotaFiscal.Valor
                                                    }
                                                };
                    }
                    */

                    docs.Add(docto);
                }

                if (docs.Count > 0)
                    entrega.Doctos = docs.ToArray();

                entregas.Add(entrega);
            }

            return entregas;
        }


        private static Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento ObterViagem(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaDadosTransporteIntegracao.Carga;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = repIntegracaoAngelLira.Buscar();

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento agendamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Agendamento();
            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = carga.Pedidos.FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = carga?.Pedidos.ToList() ?? new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Cliente clienteOrigem = null;
            Dominio.Entidades.Cliente clienteDestino = null;

            if (cargaPedidoPrimeiro.PontoPartida != null)
            {
                clienteOrigem = cargaPedidoPrimeiro.PontoPartida;
                clienteDestino = cargaPedidoPrimeiro.Expedidor ?? cargaPedidoPrimeiro.Pedido.Expedidor ?? cargaPedidoPrimeiro.Pedido.Remetente;
            }
            else
            {
                clienteOrigem = cargaPedidoPrimeiro.Expedidor ?? cargaPedidoPrimeiro.Pedido.Expedidor ?? cargaPedidoPrimeiro.Pedido.Remetente;
                clienteDestino = cargaPedidoPrimeiro.Recebedor ?? cargaPedidoPrimeiro.Pedido.Recebedor ?? cargaPedidoPrimeiro.Pedido.Destinatario;
            }

            viagem.V_Tipo = 2;

            viagem.V_Carga = ObterVCarga(true, carga, integracaoAngelLira, tipoServicoMultisoftware, unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (carga?.FaixaTemperatura != null)
                {
                    int.TryParse(carga?.FaixaTemperatura?.CodigoIntegracao, out int codigoFaixaTemperatura);

                    if (codigoFaixaTemperatura > 0)
                        viagem.V_FaixaTemperatura = codigoFaixaTemperatura;
                }

                viagem.V_Observacao = string.Join("/", (from obj in cargaPedidos select obj.Pedido.NumeroPedidoEmbarcador).ToList());
            }
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = carga.FaixaTemperatura ?? carga.TipoDeCarga?.FaixaDeTemperatura;

                if (faixaTemperatura != null &&
                    int.TryParse(faixaTemperatura.CodigoIntegracao, out int codigoFaixaTemperatura) &&
                    codigoFaixaTemperatura > 0)
                    viagem.V_FaixaTemperatura = codigoFaixaTemperatura;
            }

            int codigoRota = 0;
            string polilinha = "";

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete == null && carga.Rota == null)
            {
                Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorRemetenteDestino(clienteOrigem.CPF_CNPJ, clienteDestino.CPF_CNPJ);

                if (rota != null && !string.IsNullOrWhiteSpace(rota.DescricaoRotaSemParar))
                    int.TryParse(Utilidades.String.OnlyNumbers(rota.DescricaoRotaSemParar), out codigoRota);
            }
            else
            {
                int.TryParse(Utilidades.String.OnlyNumbers(carga.Rota?.CodigoIntegracao), out codigoRota);

                if (cargaRotaFrete != null)
                    polilinha = cargaRotaFrete.PolilinhaRota;
                else
                    polilinha = carga.Rota?.PolilinhaRota;
            }

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !string.IsNullOrWhiteSpace(polilinha))
                viagem.V_Polilinha = polilinha;
            else if (codigoRota > 0 && !integracaoAngelLira.NaoEnviarRotaViagem)
                viagem.V_Rota = codigoRota;

            if (carga.ProcedimentoEmbarque != null)
                viagem.V_ProcedimentoEmbarque = carga.ProcedimentoEmbarque.IntegracaoProcedimentoEmbarque;
            else
                viagem.V_ProcedimentoEmbarque = ObterProcedimentoEmbarque(carga, 1, unitOfWork);


            if (viagem.V_ProcedimentoEmbarque <= 0)
                viagem.V_ProcedimentoEmbarque = 1;


            if (carga.ProcedimentoEmbarque != null)
                viagem.V_Modelo_Contratacao = carga.ProcedimentoEmbarque.CodigoModeloContratacao;
            else
                viagem.V_Modelo_Contratacao = carga.TipoOperacao?.CodigoModeloContratacao ?? 0;


            viagem.V_Origem = CodigoClienteAngelLira(integracaoAngelLira, clienteOrigem, cargaDadosTransporteIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
            if (!string.IsNullOrWhiteSpace(mensagem))
                return null;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                viagem.V_Destino = CodigoClienteAngelLira(integracaoAngelLira, clienteDestino, cargaDadosTransporteIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;
            }
            else
            {
                int codigoTipoCirucuito = 1;
                if (viagem.V_Modelo_Contratacao == codigoTipoCirucuito)
                {
                    viagem.V_Destino = CodigoClienteAngelLira(integracaoAngelLira, clienteDestino, cargaDadosTransporteIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                    if (!string.IsNullOrWhiteSpace(mensagem))
                        return null;
                }
                else
                {
                    viagem.V_Destino = viagem.V_Origem;
                }
            }

            viagem.V_Valor = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCarga(carga.Codigo);

            DateTime? dataPrevisaoInicioViagem = null, dataPrevisaoFimViagem = null;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (integracaoAngelLira.UtilizarDataAgendamentoPedido)
                    cargaPedidos = cargaPedidos.OrderBy(o => o.Pedido.DataAgendamento).ToList();
                else
                    cargaPedidos = cargaPedidos.OrderBy(o => o.Pedido.DataPrevisaoSaida).ToList();

                if (integracaoAngelLira.UtilizarDataAtualETempoRotaParaInicioEFimViagem)
                {
                    dataPrevisaoInicioViagem = DateTime.Now;
                    dataPrevisaoFimViagem = dataPrevisaoInicioViagem.Value.AddMinutes(cargaDadosTransporteIntegracao.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0);

                    viagem.V_PrevInicio = dataPrevisaoInicioViagem.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                    viagem.V_PrevFim = dataPrevisaoFimViagem.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                else
                {
                    dataPrevisaoInicioViagem = cargaPedidos.Min(o => o.Pedido.DataPrevisaoSaida) ?? DateTime.Now.AddMinutes(35);
                    dataPrevisaoFimViagem = cargaPedidos.Max(o => o.Pedido.PrevisaoEntrega);

                    viagem.V_PrevInicio = dataPrevisaoInicioViagem?.ToString("yyyy-MM-ddTHH:mm:ss");
                    viagem.V_PrevFim = dataPrevisaoFimViagem?.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }
            else
            {
                viagem.V_PrevInicio = DateTime.Now.AddMinutes(35).ToString("yyyy-MM-ddTHH:mm:ss");
            }

            //viagem.V_DataFat = carga.DataFinalizacaoEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss");

            viagem.V_EnviarEmail = "N";
            viagem.V_EnviarSMS = "N";

            if (carga.Motoristas != null && carga.Motoristas.Count > 0)
                viagem.V_Motorista = carga.Motoristas?.FirstOrDefault().CPF_Formatado;

            //viagem.V_Transportador = carga.Empresa.CNPJ_Formatado;
            viagem.V_Codigo_Transportador = carga.Empresa?.CNPJ_Formatado;

            if (carga.Veiculo != null)
            {
                if (integracaoAngelLira.EnviarDadosFormatados)
                    viagem.V_Veiculo = carga.Veiculo.Placa.ObterPlacaFormatada();
                else
                    viagem.V_Veiculo = carga.Veiculo.Placa.Insert(3, "-");

                for (int i = 0; i < carga.VeiculosVinculados.Count; i++)
                {
                    Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados.ToList()[i];

                    string placa = string.Empty;

                    if (integracaoAngelLira.EnviarDadosFormatados)
                        placa = carga.Veiculo.Placa.ObterPlacaFormatada();
                    else
                        placa = reboque.Placa.Insert(3, "-");

                    if (i == 0)
                        viagem.V_Carreta1 = placa;
                    else if (i == 1)
                        viagem.V_Carreta2 = placa;
                    else if (i == 2)
                        viagem.V_Carreta3 = placa;
                    else
                        break;
                }
            }

            if (preCarga != null)
                viagem.V_Vinculo = string.Join(", ", (from obj in preCarga.Pedidos select obj.NumeroPedidoEmbarcador).ToList());

            int tempoEntrega = carga.TipoOperacao?.TempoEntregaAngelLira ?? 0;

            if (carga.ProcedimentoEmbarque != null)
                tempoEntrega = carga.ProcedimentoEmbarque.TempoEntregaAngelLira;

            if (tempoEntrega <= 0)
                tempoEntrega = 120;

            DateTime? dataAgendamento = null;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega> entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega>();
            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                Dominio.Entidades.Cliente cargaPedidoClienteDestino = cargaPedido.PontoPartida != null ? cargaPedido.Pedido.Remetente : cargaPedido.Pedido.Destinatario;

                if (cargaPedido.PontoPartida != null)
                    cargaPedidoClienteDestino = cargaPedido.Expedidor ?? cargaPedido.Pedido.Expedidor ?? cargaPedido.Pedido.Remetente;
                else
                    cargaPedidoClienteDestino = cargaPedido.Recebedor ?? cargaPedido.Pedido.Recebedor ?? cargaPedido.Pedido.Destinatario;

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCargaPedido = repPreCarga.BuscarPorPedido(cargaPedido.Pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Entrega();
                entrega.E_SeqEntrega = i + 1;
                entrega.E_Cliente = CodigoClienteAngelLira(integracaoAngelLira, cargaPedidoClienteDestino, cargaDadosTransporteIntegracao, ref mensagem, unitOfWork, tipoServicoMultisoftware);
                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (integracaoAngelLira.UtilizarDataAgendamentoPedido)
                    {
                        DateTime? dataAgendamentoPedido = cargaPedido.Pedido.DataAgendamento?.AddMinutes(tempoEntrega);

                        if (dataAgendamentoPedido.HasValue && (!dataAgendamento.HasValue || dataAgendamento < dataAgendamentoPedido))
                            dataAgendamento = dataAgendamentoPedido;

                        entrega.E_PrevInicio = cargaPedido.Pedido.DataAgendamento?.ToString("yyyy-MM-ddTHH:mm:ss");
                        entrega.E_PrevFim = dataAgendamentoPedido?.ToString("yyyy-MM-ddTHH:mm:ss");
                    }
                    else
                    {
                        if (configuracaoTMS.InformarDataViagemExecutadaPedido)
                            entrega.E_PrevInicio = cargaPedido.Pedido.DataInicialViagemExecutada?.ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevInicio;
                        else
                            entrega.E_PrevInicio = cargaPedidoPrimeiro.Pedido.PrevisaoEntrega?.AddMinutes(-tempoEntrega).ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevInicio;

                        if (!dataPrevisaoFimViagem.HasValue || !cargaPedido.Pedido.PrevisaoEntrega.HasValue || dataPrevisaoFimViagem >= cargaPedido.Pedido.PrevisaoEntrega)
                            entrega.E_PrevFim = cargaPedido.Pedido.PrevisaoEntrega?.ToString("yyyy-MM-ddTHH:mm:ss") ?? viagem.V_PrevFim;
                        else
                        {
                            entrega.E_PrevInicio = viagem.V_PrevInicio;
                            entrega.E_PrevFim = viagem.V_PrevFim;
                        }
                    }
                }
                else
                {
                    entrega.E_PrevInicio = viagem.V_PrevInicio;
                }

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto> docs = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in cargaPedido.NotasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto docto = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Docto();
                    docto.D_Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero;
                    docto.D_Serie = !string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Serie) ? pedidoXMLNotaFiscal.XMLNotaFiscal.Serie : "1";

                    docs.Add(docto);
                }
                if (docs.Count > 0)
                    entrega.Doctos = docs.ToArray();


                VerificaDestino(cargaPedido, ref viagem, integracaoAngelLira.AplicarRegraLocalPalletizacao);

                entregas.Add(entrega);
            }

            if (integracaoAngelLira.UtilizarDataAgendamentoPedido && dataAgendamento.HasValue)
                viagem.V_PrevFim = dataAgendamento.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            
            viagem.Entregas = entregas.ToArray();

            agendamento.Versao = "1.2";
            agendamento.Viagem = viagem;

            return agendamento;
        }

        private static void VerificaDestino(CargaPedido cargaPedido,ref Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem.Viagem viagem,bool AplicarRegraLocalPalletizacao)
        {
            if (AplicarRegraLocalPalletizacao && cargaPedido?.Pedido != null && cargaPedido.Pedido.LocalPaletizacao != null && !string.IsNullOrEmpty(cargaPedido.Pedido.LocalPaletizacao.CPF_CNPJ_SemFormato))
            {
                viagem.V_Destino = cargaPedido.Pedido.LocalPaletizacao.CPF_CNPJ_SemFormato;
            }
        }


        private static string CodigoClienteAngelLira(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = CodigoClienteAngelLira(out string codigoIntegracao, out string mensagemIntegracao, integracaoAngelLira, cliente, unitOfWork, tipoServicoMultisoftware);

            if (arquivoIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            if (!string.IsNullOrWhiteSpace(mensagemIntegracao))
                mensagem = mensagemIntegracao;

            return codigoIntegracao;
        }

        private static string CodigoClienteAngelLira(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cliente == null)
                return null;

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = CodigoClienteAngelLira(out string codigoIntegracao, out string mensagemIntegracao, integracaoAngelLira, cliente, unitOfWork, tipoServicoMultisoftware);

            if (arquivoIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            if (!string.IsNullOrWhiteSpace(mensagemIntegracao))
                mensagem = mensagemIntegracao;

            return codigoIntegracao;
        }

        private static Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo CodigoClienteAngelLira(out string codigoIntegracao, out string mensagem, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira, Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            mensagem = null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.ClienteDadosIntegracao repClienteDadosIntegracao = new Repositorio.ClienteDadosIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.ClienteDadosIntegracao clienteDadosIntegracao = repClienteDadosIntegracao.BuscaPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira, cliente.CPF_CNPJ);

            if (clienteDadosIntegracao != null)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.Tipo != "E")
                    codigoIntegracao = cliente.CPF_CNPJ_SemFormato;
                else
                    codigoIntegracao = cliente.CodigoIntegracao;

                return null;
            }

            string url = "";
            string mensagemRetornoIntegracao = "";
            InspectorBehavior inspector = new InspectorBehavior();
            try
            {
                WSAngelLira.ValidationSoapHeader validationSoapHeader = ObterCabecalho(unitOfWork, out url);
                WSAngelLira.WSImportSoapClient wSImportSoapClient = ObterWSImportClient(url);

                wSImportSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

                List<WSAngelLira.Cliente> clientes = new List<WSAngelLira.Cliente>();

                WSAngelLira.Cliente clienteAnge = new WSAngelLira.Cliente
                {
                    bairro = cliente.Bairro,
                    cep = Utilidades.String.OnlyNumbers(cliente.CEP),
                    cidade = cliente.Localidade.Descricao,
                    complemento = cliente.Bairro,
                    endereco = cliente.Endereco,
                    fantasia = !string.IsNullOrWhiteSpace(cliente.NomeFantasia) ? cliente.NomeFantasia : cliente.Nome,
                    nome = cliente.Nome,
                    numero = cliente.Numero,
                    pais = cliente.Tipo == "E" ? "" : "BRA",
                    uf = cliente.Localidade.Estado.Sigla
                };

                if (integracaoAngelLira.EnviarDadosFormatados)
                {
                    if (cliente.Tipo == "F")
                        clienteAnge.cpf = cliente.CPF_CNPJ_Formatado;
                    else
                        clienteAnge.cnpj = cliente.CPF_CNPJ_Formatado;
                }
                else
                {
                    if (cliente.Tipo == "F")
                        clienteAnge.cpf = cliente.CPF_CNPJ_SemFormato;
                    else
                        clienteAnge.cnpj = cliente.CPF_CNPJ_SemFormato;
                }

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.Tipo != "E")
                    clienteAnge.id = cliente.CPF_CNPJ_SemFormato;
                else
                    clienteAnge.id = cliente.CodigoIntegracao;

                clientes.Add(clienteAnge);

                WSAngelLira.RetornoCliente[] retornos = wSImportSoapClient.CadastrarCliente(validationSoapHeader, clientes.ToArray());

                mensagemRetornoIntegracao = string.Join(", ", retornos.Select(o => o.status.ToString() + " - " + o.motivo)) + " (Criar cliente AngelLira)";

                if (retornos.Any(o => o.status == 0 || o.status == 999))
                {
                    clienteDadosIntegracao = new Dominio.Entidades.ClienteDadosIntegracao
                    {
                        Cliente = cliente,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira,
                        CodigoIntegracao = retornos.Select(o => o.codigo.ToString()).FirstOrDefault()
                    };

                    repClienteDadosIntegracao.Inserir(clienteDadosIntegracao);
                }
                else
                {
                    mensagem = mensagemRetornoIntegracao;
                }

            }
            catch (Exception ex)
            {
                mensagem = string.Concat("Ocorreu uma falha ao integrar com a AngelLira: ", ex.Message);
                Servicos.Log.TratarErro(ex);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                Mensagem = mensagemRetornoIntegracao,
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (clienteDadosIntegracao != null)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    codigoIntegracao = cliente.CPF_CNPJ_SemFormato;
                else
                    codigoIntegracao = cliente.CodigoIntegracao;
            }
            else
                codigoIntegracao = "";

            return arquivoIntegracao;
        }

        public static WSAngelLira.ValidationSoapHeader ObterCabecalho(Repositorio.UnitOfWork unidadeTrabalho, out string url)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = repIntegracaoAngelLira.Buscar();
            url = "";
            if (integracaoAngelLira != null)
            {
                WSAngelLira.ValidationSoapHeader validationSoapHeader = new WSAngelLira.ValidationSoapHeader();
                validationSoapHeader.homologacao = integracaoAngelLira.Homologacao;
                validationSoapHeader.userCod = integracaoAngelLira.Usuario;
                validationSoapHeader.userPwd = integracaoAngelLira.Senha;
                url = integracaoAngelLira.URLAcesso;
                if (string.IsNullOrEmpty(url))
                    url = "https://api.angellira.com.br/ws-soap/";

                return validationSoapHeader;
            }
            else
                return null;
        }

        public static WSAngelLiraStatus.ValidationSoapHeader ObterCabecalhoStatus(Repositorio.UnitOfWork unidadeTrabalho, out string url)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = repIntegracaoAngelLira.Buscar();
            url = "";
            if (integracaoAngelLira != null)
            {
                WSAngelLiraStatus.ValidationSoapHeader validationSoapHeader = new WSAngelLiraStatus.ValidationSoapHeader();
                validationSoapHeader.homologacao = integracaoAngelLira.Homologacao;
                validationSoapHeader.userCod = integracaoAngelLira.Usuario;
                validationSoapHeader.userPwd = integracaoAngelLira.Senha;
                url = integracaoAngelLira.URLAcesso;
                if (string.IsNullOrEmpty(url))
                    url = "https://api.angellira.com.br/ws-soap/";

                return validationSoapHeader;
            }
            else
                return null;
        }

        private static int ObterProcedimentoEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga, int valorPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao == null)
                return valorPadrao;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira repositorioExcecoesAngelLira = new Repositorio.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            decimal valorNotasFiscais = repositorioXMLNotaFiscal.BuscarValorTotalPorCarga(carga.Codigo);
            List<CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            List<string> estados = cargaPedidos.Where(obj => obj.Destino?.Estado != null).Select(obj => obj.Destino.Estado.Sigla).Distinct().ToList();

            return repositorioExcecoesAngelLira.BuscarPorTipoOperacaoDestinoValorMinimo(carga.TipoOperacao.Codigo, valorNotasFiscais, estados)?.ProcedimentoEmbarque ?? carga.TipoOperacao.IntegracaoProcedimentoEmbarque;
        }

        private static string ObterVCarga(bool origemTransporteDaCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);

            if (integracaoAngelLira == null)
                integracaoAngelLira = repIntegracaoAngelLira.Buscar();

            if (origemTransporteDaCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = carga.Pedidos.FirstOrDefault();

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.TipoOperacao?.DeslocamentoVazio ?? false))
                {
                    return cargaPedidoPrimeiro.Pedido.Codigo.ToString() + "VAZIO";
                }
                else
                {
                    if (integracaoAngelLira.RegraCodigoIdentificacaoViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AngelLiraRegraCodigoIdentificacaoViagem.NumeroPedidoEmbarcadorMaisPlaca)
                        return cargaPedidoPrimeiro.Pedido.NumeroPedidoEmbarcador + (carga.Veiculo?.Placa ?? string.Empty);
                    else
                        return cargaPedidoPrimeiro.Codigo.ToString() + cargaPedidoPrimeiro.Pedido.NumeroPedidoEmbarcador;
                }
            }
            else
            {
                if ((!carga.CargaAgrupada) && (!integracaoAngelLira.IgnorarValidacaoCargaAgrupadaRegraCodigoViagem))
                {
                    return carga.CodigoCargaEmbarcador;
                }
                else
                {
                    if (integracaoAngelLira.RegraCodigoIdentificacaoViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AngelLiraRegraCodigoIdentificacaoViagem.NumeroPedidoEmbarcadorMaisPlaca)
                    {
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = repCarga.BuscarCargasOriginais(carga.Codigo);
                        return string.Join("/", (from obj in cargasOrigem select obj.CodigoCargaEmbarcador).ToList());
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = carga.Pedidos.FirstOrDefault();

                        return cargaPedidoPrimeiro.Codigo.ToString() + cargaPedidoPrimeiro.Pedido.NumeroPedidoEmbarcador;
                    }
                }
            }
        }

        #endregion
    }
}
