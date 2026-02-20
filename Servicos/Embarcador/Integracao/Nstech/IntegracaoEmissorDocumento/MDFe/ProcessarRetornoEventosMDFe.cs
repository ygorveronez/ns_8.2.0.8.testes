using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool ProcessarMDFe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, dynamic objetoRetorno, RetornoEventoCTe retornoEventoMDFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            string idDocumento = (string)objetoRetorno.id;

            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                if (retornoEventoMDFe != null)
                    Log.TratarErro($"ReceberMDFe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string externalId = ((string)objetoRetorno.data?.externalId ?? (string)objetoRetorno.externalId).ObterSomenteNumeros();

            int codigoMDFe = 0;
            if (!int.TryParse(externalId, out codigoMDFe))
            {
                if (retornoEventoMDFe != null)
                    Log.TratarErro($"ReceberMDFe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string chaveAcessoMDFe = string.Empty;
            string qrCodeMDFe = string.Empty;
            string protocoloAutorizacao = string.Empty;
            DateTime dataProtocolo = DateTime.Now;
            string codStatusProtocolo = string.Empty;
            string descricaoProtocolo = string.Empty;
            string statusIntegrador = string.Empty;
            string urlDownloadXML = string.Empty;

            switch (status)
            {
                case "authorized":
                case "authorized_with_divergent_digval":
                    statusIntegrador = "M";

                    if (objetoRetorno.data?.sefazData == null && objetoRetorno.sefazData == null)
                    {
                        if (retornoEventoMDFe != null)
                            Log.TratarErro($"ReceberMDFe - Mensagem: Não foi possível obter o objeto data.sefazData, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                        mensagemErro = "Não foi possível obter o externalID.";
                        return false;
                    }

                    chaveAcessoMDFe = (string)objetoRetorno.data?.mdfeKey ?? (string)objetoRetorno.mdfeKey;
                    qrCodeMDFe = (string)objetoRetorno.data?.qrCode ?? (string)objetoRetorno.qrCode;
                    urlDownloadXML = (string)objetoRetorno.data?.xml ?? (string)objetoRetorno.xml;
                    protocoloAutorizacao = (string)objetoRetorno.data?.sefazData.authorizationProtocol ?? (string)objetoRetorno.sefazData.authorizationProtocol;
                    codStatusProtocolo = (string)objetoRetorno.data?.sefazData.status ?? (string)objetoRetorno.sefazData.status;
                    descricaoProtocolo = (string)objetoRetorno.data?.sefazData.reason ?? (string)objetoRetorno.sefazData.reason;
                    dataProtocolo = (DateTime?)objetoRetorno.data?.sefazData.authorizationDate ?? (DateTime)objetoRetorno.sefazData.authorizationDate;
                    break;

                case "rejected":
                    statusIntegrador = "R";
                    codStatusProtocolo = (string)objetoRetorno.data?.sefazData.status ?? (string)objetoRetorno.sefazData.status;
                    descricaoProtocolo = (string)objetoRetorno.data?.sefazData.reason ?? (string)objetoRetorno.sefazData.reason;
                    break;

                case "error":
                    statusIntegrador = "R";
                    descricaoProtocolo = (string)(objetoRetorno.data?.error?.message ?? objetoRetorno.error?.message);
                    break;

                default:
                    break;
            }

            #region Localizar Documento

            if (!string.IsNullOrEmpty(descricaoProtocolo) && descricaoProtocolo.Length > 1900)
                descricaoProtocolo = descricaoProtocolo.Substring(0, Math.Min(descricaoProtocolo.Length, 1900));

            if (mdfe == null)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null && !string.IsNullOrWhiteSpace(chaveAcessoMDFe))
                    mdfe = repMDFe.BuscarPorChave(chaveAcessoMDFe);

                if (mdfe == null)
                {
                    mensagemErro = "MDFe " + (string.IsNullOrEmpty(chaveAcessoMDFe) ? "código " + codigoMDFe.ToString() : chaveAcessoMDFe) + " não localizado na base SqlServer";
                    return false;
                }
            }

            #endregion

            Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle = new Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle();
            mdfeOracle.DataRecibo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
            mdfeOracle.CodStatusEnvio = codStatusProtocolo;
            mdfeOracle.DescricaoEnvio = descricaoProtocolo;
            mdfeOracle.NumeroRecibo = null;
            mdfeOracle.DataProtocolo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
            mdfeOracle.CodStatusProtocolo = codStatusProtocolo;
            mdfeOracle.DescricaoProtocolo = descricaoProtocolo;
            mdfeOracle.NumeroProtocolo = protocoloAutorizacao;
            mdfeOracle.ChaveMDFe = chaveAcessoMDFe;
            mdfeOracle.DigVerificador = qrCodeMDFe;
            mdfeOracle.StatusIntegrador = statusIntegrador;
            mdfeOracle.DescricaoStatusIntegrador = mdfeOracle.StatusIntegrador == "M" ? "MDFe processado" : "MDFe não integrado";
            mdfeOracle.CodigoMDFeAutorizacao = 0;
            mdfeOracle.XMLAutorizacao = this.ObterXMLEmissorAsync(urlDownloadXML).GetAwaiter().GetResult();
            mdfeOracle.PDFDAMDFE = null;

            mdfeOracle.Info = new Dominio.ObjetosDeValor.WebService.CTe.Resultado()
            {
                Tipo = "OK",
                Mensagem = mdfeOracle.StatusIntegrador == "M" ? "MDFe processado" : "MDFe não integrado"
            };

            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            if (!serMDFe.ProcessarRetornoMDFeAutorizado(out mensagemErro, mdfeOracle, mdfe, Auditado, tipoServicoMultisoftware, unitOfWork, "RetornoEmissao"))
                return false;

            return true;
        }

        public bool ProcessarEventoMDFe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, dynamic objetoRetorno, RetornoEventoCTe retornoEventoMDFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            string idDocumento = (string)objetoRetorno.id;

            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                Log.TratarErro($"ReceberEventoMDFe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}", "ReceberEventoMDFe");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string externalId = (string)(objetoRetorno.data?.externalId ?? objetoRetorno.externalId);

            if (!string.IsNullOrWhiteSpace(externalId) && externalId.Contains("_external_close"))
            {
                Log.GravarInfo($"ReceberEventoMDFe - Mensagem: Retorno encerramento MDF-e externo, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}", "ReceberEventoMDFe");
                return true;
            }

            externalId = externalId.ObterSomenteNumeros();

            int codigoMDFe = 0;
            if (!int.TryParse(externalId, out codigoMDFe))
            {
                Log.TratarErro($"ReceberEventoMDFe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}", "ReceberEventoMDFe");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string eventName = (string)objetoRetorno.data?.eventName ?? (string)objetoRetorno.eventName;
            string chaveAcessoMDFe = string.Empty;
            string protocoloAutorizacao = string.Empty;
            DateTime dataProtocolo = DateTime.Now;
            string codStatusProtocolo = string.Empty;
            string descricaoProtocolo = string.Empty;
            string statusIntegrador = string.Empty;
            string urlDownloadXML = string.Empty;

            switch (status)
            {
                case "authorized":
                case "authorized_with_divergent_digval":
                    if (eventName == "cancel")
                        statusIntegrador = "C";
                    else if (eventName == "close")
                        statusIntegrador = "E";
                    else if (eventName == "include_driver")
                        statusIntegrador = "E";

                    if ((objetoRetorno.data?.sefazData ?? objetoRetorno.sefazData) == null)
                    {
                        Log.TratarErro($"ReceberEventoMDFe - Mensagem: Não foi possível obter o objeto data.sefazData, Evento: {eventName}, Data: {retornoEventoMDFe.objeto.ToString()}", "ReceberEventoMDFe");
                        mensagemErro = "Não foi possível obter o externalID.";
                        return false;
                    }

                    chaveAcessoMDFe = (string)(objetoRetorno.data?.mdfeKey ?? objetoRetorno.mdfeKey);
                    urlDownloadXML = (string)(objetoRetorno.data?.xml ?? objetoRetorno.xml);
                    protocoloAutorizacao = (string)(objetoRetorno.data?.sefazData.authorizationProtocol ?? objetoRetorno.sefazData.authorizationProtocol);
                    codStatusProtocolo = eventName == "cancel" ? "101" : (string)(objetoRetorno.data?.sefazData.status ?? objetoRetorno.sefazData.status);
                    descricaoProtocolo = (string)(objetoRetorno.data?.sefazData.reason ?? objetoRetorno.sefazData.reason);
                    dataProtocolo = (DateTime)(objetoRetorno.data?.sefazData.authorizationDate ?? objetoRetorno.sefazData.authorizationDate);
                    break;

                case "rejected":
                    statusIntegrador = "R";
                    codStatusProtocolo = (string)(objetoRetorno.data?.sefazData.status ?? objetoRetorno.sefazData.status);
                    descricaoProtocolo = (string)(objetoRetorno.data?.sefazData.reason ?? objetoRetorno.sefazData.reason);
                    break;

                case "error":
                    statusIntegrador = "R";
                    codStatusProtocolo = "225";
                    descricaoProtocolo = (string)(objetoRetorno.data?.error?.message ?? objetoRetorno.error?.message);
                    break;

                default:
                    break;
            }

            if (eventName == "cancel" || eventName == "close" || eventName == "include_driver")
            {
                #region Localizar Documento

                if (mdfe == null)
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                    mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                    if (mdfe == null && !string.IsNullOrWhiteSpace(chaveAcessoMDFe))
                        mdfe = repMDFe.BuscarPorChave(chaveAcessoMDFe);

                    if (mdfe == null)
                    {
                        mensagemErro = "MDFe " + (string.IsNullOrEmpty(chaveAcessoMDFe) ? "código " + codigoMDFe.ToString() : chaveAcessoMDFe) + " não localizado na base SqlServer";
                        return false;
                    }
                }

                #endregion

                Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle = new Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle();
                mdfeOracle.DataRecibo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
                mdfeOracle.CodStatusEnvio = codStatusProtocolo;
                mdfeOracle.DescricaoEnvio = descricaoProtocolo;
                mdfeOracle.NumeroRecibo = null;
                mdfeOracle.DataProtocolo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
                mdfeOracle.CodStatusProtocolo = codStatusProtocolo;
                mdfeOracle.DescricaoProtocolo = descricaoProtocolo;
                mdfeOracle.NumeroProtocolo = protocoloAutorizacao;
                mdfeOracle.ChaveMDFe = chaveAcessoMDFe;
                mdfeOracle.StatusIntegrador = statusIntegrador;
                mdfeOracle.DescricaoStatusIntegrador = (mdfeOracle.StatusIntegrador == "C" || mdfeOracle.StatusIntegrador == "E") ? "MDFe evento processado" : "MDFe evento não integrado";
                mdfeOracle.CodigoMDFeCancelamento = 0;
                string xml = this.ObterXMLEmissorAsync(urlDownloadXML).GetAwaiter().GetResult();
                mdfeOracle.XMLCancelamento = eventName == "cancel" ? xml : null;
                mdfeOracle.XMLEncerramento = eventName == "close" || eventName == "include_driver" ? xml : null;
                mdfeOracle.PDFDAMDFE = null;

                mdfeOracle.Info = new Dominio.ObjetosDeValor.WebService.CTe.Resultado()
                {
                    Tipo = "OK",
                    Mensagem = mdfeOracle.StatusIntegrador == "M" ? "MDFe processado" : "MDFe não integrado"
                };

                string acaoProcessamento = string.Empty;
                if (eventName == "cancel")
                    acaoProcessamento = "RetornoCancelamento";
                else if (eventName == "close")
                    acaoProcessamento = "RetornoEncerramento";
                else if (eventName == "include_driver")
                    acaoProcessamento = "RetornoInclusaoMotorista";

                Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
                if (!serMDFe.ProcessarRetornoMDFeAutorizado(out mensagemErro, mdfeOracle, mdfe, Auditado, tipoServicoMultisoftware, unitOfWork, acaoProcessamento))
                    return false;
            }
            else
            {
                Log.TratarErro($"ReceberEventoMDFe - Mensagem: Evento não homologado, Evento: {eventName}, Data: {retornoEventoMDFe.objeto.ToString()}", "ReceberEventoMDFe");
                mensagemErro = "Evento não homologado.";
                return false;
            }

            return true;
        }

        public bool ProcessarRecebimentoDamdfe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, dynamic objetoRetorno, RetornoEventoCTe retornoEventoMDFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            string idDocumento = (string)objetoRetorno.id;

            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                if (retornoEventoMDFe != null)
                    Log.TratarErro($"retornoEventoMDFe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            int codigoMDFe = 0;
            if (!int.TryParse((string)objetoRetorno.data?.externalId ?? (string)objetoRetorno.externalId, out codigoMDFe))
            {
                if (retornoEventoMDFe != null)
                    Log.TratarErro($"ReceberMDFe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string chaveAcessoMDFe = string.Empty;
            string urlDownloadDamdfe = string.Empty;

            if (status == "authorized")
            {
                chaveAcessoMDFe = (string)objetoRetorno.data?.mdfeKey ?? (string)objetoRetorno.mdfeKey;
                urlDownloadDamdfe = (string)objetoRetorno.data?.damdfe ?? (string)objetoRetorno.damdfe;
            }

            #region Localizar Documento

            if (mdfe == null)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                mdfe = repMDFe.BuscarPorCodigoComFetchEmpresa(codigoMDFe);

                if (mdfe == null && !string.IsNullOrWhiteSpace(chaveAcessoMDFe))
                    mdfe = repMDFe.BuscarPorChave(chaveAcessoMDFe);

                if (mdfe == null)
                {
                    mensagemErro = "MDFe " + (string.IsNullOrEmpty(chaveAcessoMDFe) ? "código " + codigoMDFe.ToString() : chaveAcessoMDFe) + " não localizado na base SqlServer";
                    return false;
                }
            }

            mdfe.Chave ??= chaveAcessoMDFe;

            #endregion

            Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle = new Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle();
            mdfeOracle.PDFDAMDFE = null;
            byte[] damdfe = this.obterDacte(urlDownloadDamdfe);

            if (damdfe == null)
            {
                mensagemErro = "MDFe " + (string.IsNullOrEmpty(chaveAcessoMDFe) ? "código " + codigoMDFe.ToString() : chaveAcessoMDFe) + " não foi possível obter a damdfe";
                return false;
            }

            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            serMDFe.ObterESalvarDAMDFEOracle(mdfe, mdfeOracle, unitOfWork, damdfe);

            return true;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        public string ProcessarXMLMDFe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, dynamic objetoRetorno, RetornoEventoCTe retornoEventoMDFe, Repositorio.UnitOfWork unitOfWork, bool retornarXMLCancelamento = false)
        {
            mensagemErro = string.Empty;
            int codigoMDFe = 0;
            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                if (retornoEventoMDFe != null)
                    Log.TratarErro($"ReceberMDFe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                throw new Exception(mensagemErro);
            }

            if (!int.TryParse((string)objetoRetorno.data?.externalId ?? (string)objetoRetorno.externalId, out codigoMDFe))
            {
                if (retornoEventoMDFe != null)
                    Log.TratarErro($"ReceberMDFe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                throw new Exception(mensagemErro);
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string chaveAcessoMDFe = string.Empty;
            string urlXmlAutorizacao = string.Empty;
            string urlXmlCancelamento = string.Empty;

            if (status == "authorized" || status == "canceled")
            {
                chaveAcessoMDFe = (string)objetoRetorno.data?.mdfeKey ?? (string)objetoRetorno.cteKey;
                urlXmlAutorizacao = (string)(objetoRetorno.data?.xml ?? objetoRetorno.xml);
            }

            #region Localizar Documento

            if (mdfe == null)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null && !string.IsNullOrWhiteSpace(chaveAcessoMDFe))
                    mdfe = repMDFe.BuscarPorChave(chaveAcessoMDFe);

                if (mdfe == null)
                {
                    mensagemErro = "MDFe " + (string.IsNullOrEmpty(chaveAcessoMDFe) ? "código " + codigoMDFe.ToString() : chaveAcessoMDFe) + " não localizado na base SqlServer";
                    throw new Exception(mensagemErro);
                }
            }

            #endregion
            ServicoMDFe.RetornoMDFe mdfeOracle = new ServicoMDFe.RetornoMDFe();
            ServicoMDFe.RetornoEventoMDFe mdfeRetornoEvento = new ServicoMDFe.RetornoEventoMDFe();

            mdfeOracle.XML = this.ObterXMLEmissorAsync(urlXmlAutorizacao).GetAwaiter().GetResult();

            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            serMDFe.ObterESalvarXMLAutorizacao(mdfe, mdfe.Empresa.Codigo, unitOfWork, mdfeOracle);


            if (objetoRetorno?.events != null)
            {
                // Filtrando eventos com eventName "cancel"
                dynamic eventoCancelado = ((JArray)objetoRetorno["events"])
                    .FirstOrDefault(e => (string)e["eventName"] == "cancel");

                if (eventoCancelado != null)
                {
                    urlXmlCancelamento = (string)(eventoCancelado.data?.xml ?? eventoCancelado.xml);
                    mdfeRetornoEvento.XML = this.ObterXMLEmissorAsync(urlXmlCancelamento).GetAwaiter().GetResult();

                    serMDFe.ObterESalvarXMLCancelamento(mdfe.Codigo, codigoEmpresa, mdfeRetornoEvento, unitOfWork);
                }
            }

            return retornarXMLCancelamento ? mdfeRetornoEvento.XML : mdfeOracle.XML;
        }

        #endregion Métodos Privados
    }
}