using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool ProcessarCTe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, dynamic objetoRetorno, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool obterXML = true)
        {
            mensagemErro = string.Empty;

            string idDocumento = (string)objetoRetorno.id;

            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                if (retornoEventoCTe != null)
                    Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            int codigoCTe = 0;
            if (!int.TryParse((string)objetoRetorno.data?.externalId ?? (string)objetoRetorno.externalId, out codigoCTe))
            {
                if (retornoEventoCTe != null)
                    Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string chaveAcessoCTe = string.Empty;
            string qrCodeCTe = string.Empty;
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
                        if (retornoEventoCTe != null)
                            Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o objeto data.sefazData, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                        mensagemErro = "Não foi possível obter o externalID.";
                        return false;
                    }

                    chaveAcessoCTe = (string)objetoRetorno.data?.cteKey ?? (string)objetoRetorno.cteKey;
                    qrCodeCTe = (string)objetoRetorno.data?.qrCode ?? (string)objetoRetorno.qrCode;
                    urlDownloadXML = (string)objetoRetorno.data?.xml ?? (string)objetoRetorno.xml;
                    protocoloAutorizacao = (string)objetoRetorno.data?.sefazData.authorizationProtocol ?? (string)objetoRetorno.sefazData.authorizationProtocol;
                    codStatusProtocolo = (string)objetoRetorno.data?.sefazData.status ?? (string)objetoRetorno.sefazData.status;
                    descricaoProtocolo = (string)objetoRetorno.data?.sefazData.reason ?? (string)objetoRetorno.sefazData.reason;
                    dataProtocolo = (DateTime?)objetoRetorno.data?.sefazData.authorizationDate ?? (DateTime)objetoRetorno.sefazData.authorizationDate;
                    break;

                case "rejected":
                    statusIntegrador = "E";
                    codStatusProtocolo = (string)objetoRetorno.data?.sefazData.status ?? (string)objetoRetorno.sefazData.status;
                    descricaoProtocolo = (string)objetoRetorno.data?.sefazData.reason ?? (string)objetoRetorno.sefazData.reason;
                    break;

                case "error":
                    statusIntegrador = "E";
                    descricaoProtocolo = (string)(objetoRetorno.data?.error?.message ?? objetoRetorno.error?.message);
                    break;

                default:
                    break;
            }

            #region Localizar Documento

            if (cte == null)
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null && !string.IsNullOrWhiteSpace(chaveAcessoCTe))
                    cte = repCTe.BuscarPorChave(chaveAcessoCTe);

                if (cte == null)
                {
                    mensagemErro = "CTe " + (string.IsNullOrEmpty(chaveAcessoCTe) ? "código " + codigoCTe.ToString() : chaveAcessoCTe) + " não localizado na base SqlServer";
                    return false;
                }
            }

            #endregion

            Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle = new Dominio.ObjetosDeValor.WebService.CTe.CTeOracle();
            cteOracle.DataRecibo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
            cteOracle.CodStatusEnvio = codStatusProtocolo;
            cteOracle.DescricaoEnvio = descricaoProtocolo;
            cteOracle.NumeroRecibo = null;
            cteOracle.DataProtocolo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
            cteOracle.CodStatusProtocolo = codStatusProtocolo;
            cteOracle.DescricaoProtocolo = descricaoProtocolo;
            cteOracle.NumeroProtocolo = protocoloAutorizacao;
            cteOracle.ChaveCTE = chaveAcessoCTe;
            cteOracle.DigVerificador = qrCodeCTe;
            cteOracle.StatusIntegrador = statusIntegrador;
            cteOracle.DescricaoStatusIntegrador = cteOracle.StatusIntegrador == "M" ? "CTe processado" : "CTe não integrado";
            cteOracle.CodigoCTeInterno = 0;

            if (obterXML)
                cteOracle.XML = this.ObterXMLEmissorAsync(urlDownloadXML).GetAwaiter().GetResult();
            else
            {
                cteOracle.XML = null;
                cte.UrlDownloadXml = urlDownloadXML;
            }

            cteOracle.PDFDacte = null;

            cteOracle.Info = new Dominio.ObjetosDeValor.WebService.CTe.Resultado()
            {
                Tipo = "OK",
                Mensagem = cteOracle.StatusIntegrador == "M" ? "CTe processado" : "CTe não integrado"
            };

            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            serCTe.ProcessarRetornoCTeAutorizado(cteOracle, cte, Auditado, tipoServicoMultisoftware, unitOfWork);

            return true;
        }

        public bool ProcessarEventoCTe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ref Dominio.Entidades.CartaDeCorrecaoEletronica cce, dynamic objetoRetorno, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            string idDocumento = (string)objetoRetorno.id;

            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                Log.TratarErro($"ReceberEventoCTe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            int codigoCTe = 0;
            if (!int.TryParse((string)(objetoRetorno.data?.externalId ?? objetoRetorno.externalId), out codigoCTe))
            {
                Log.TratarErro($"ReceberEventoCTe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string eventName = (string)objetoRetorno.data?.eventName ?? (string)objetoRetorno.eventName;
            string chaveAcessoCTe = string.Empty;
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

                    if ((objetoRetorno.data?.sefazData ?? objetoRetorno.sefazData) == null)
                    {
                        Log.TratarErro($"ReceberEventoCTe - Mensagem: Não foi possível obter o objeto data.sefazData, Evento: {eventName}, Data: {retornoEventoCTe.objeto.ToString()}");
                        mensagemErro = "Não foi possível obter o externalID.";
                        return false;
                    }

                    chaveAcessoCTe = (string)(objetoRetorno.data?.cteKey ?? objetoRetorno.cteKey);
                    urlDownloadXML = (string)(objetoRetorno.data?.xml ?? objetoRetorno.xml);
                    protocoloAutorizacao = (string)(objetoRetorno.data?.sefazData.authorizationProtocol ?? objetoRetorno.sefazData.authorizationProtocol);
                    //codStatusProtocolo = (string)(objetoRetorno.data?.sefazData.status ?? objetoRetorno.sefazData.status);
                    codStatusProtocolo = eventName == "cancel" ? "101" : (string)(objetoRetorno.data?.sefazData.status ?? objetoRetorno.sefazData.status);
                    descricaoProtocolo = (string)(objetoRetorno.data?.sefazData.reason ?? objetoRetorno.sefazData.reason);
                    dataProtocolo = (DateTime)(objetoRetorno.data?.sefazData.authorizationDate ?? objetoRetorno.sefazData.authorizationDate);
                    break;

                case "rejected":
                    statusIntegrador = "E";
                    codStatusProtocolo = (string)(objetoRetorno.data?.sefazData.status ?? objetoRetorno.sefazData.status);
                    descricaoProtocolo = (string)(objetoRetorno.data?.sefazData.reason ?? objetoRetorno.sefazData.reason);
                    break;

                case "error":
                    statusIntegrador = "E";
                    codStatusProtocolo = "225";
                    descricaoProtocolo = (string)(objetoRetorno.data?.error?.message ?? objetoRetorno.error?.message);
                    break;

                default:
                    break;
            }

            if (eventName == "cancel")
            {
                #region Localizar Documento

                if (cte == null)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    cte = repCTe.BuscarPorCodigo(codigoCTe);

                    if (cte == null && !string.IsNullOrWhiteSpace(chaveAcessoCTe))
                        cte = repCTe.BuscarPorChave(chaveAcessoCTe);

                    if (cte == null)
                    {
                        mensagemErro = "CTe " + (string.IsNullOrEmpty(chaveAcessoCTe) ? "código " + codigoCTe.ToString() : chaveAcessoCTe) + " não localizado na base SqlServer";
                        return false;
                    }
                }

                #endregion

                Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle = new Dominio.ObjetosDeValor.WebService.CTe.CTeOracle();
                cteOracle.DataRecibo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
                cteOracle.CodStatusEnvio = codStatusProtocolo;
                cteOracle.DescricaoEnvio = descricaoProtocolo;
                cteOracle.NumeroRecibo = null;
                cteOracle.DataProtocolo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
                cteOracle.CodStatusProtocolo = codStatusProtocolo;
                cteOracle.DescricaoProtocolo = descricaoProtocolo;
                cteOracle.NumeroProtocolo = protocoloAutorizacao;
                cteOracle.ChaveCTE = chaveAcessoCTe;
                cteOracle.StatusIntegrador = statusIntegrador;
                cteOracle.DescricaoStatusIntegrador = cteOracle.StatusIntegrador == "M" ? "CTe processado" : "CTe não integrado";
                cteOracle.CodigoCTeInterno = 0;
                cteOracle.XMLCancelamento = this.ObterXMLEmissorAsync(urlDownloadXML).GetAwaiter().GetResult();
                cteOracle.PDFDacte = null;

                cteOracle.Info = new Dominio.ObjetosDeValor.WebService.CTe.Resultado()
                {
                    Tipo = "OK",
                    Mensagem = cteOracle.StatusIntegrador == "M" ? "CTe processado" : "CTe não integrado"
                };

                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                serCTe.ProcessarRetornoCTeAutorizado(cteOracle, cte, Auditado, tipoServicoMultisoftware, unitOfWork);
            }
            else if (eventName == "correction_letter")
            {
                #region Localizar Documento

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                if (cce == null)
                    cce = repCCe.BuscarPorCodigo(codigoCTe);

                if (cce == null)
                {
                    mensagemErro = "CCe código " + codigoCTe.ToString() + " não localizado na base SqlServer";
                    return false;
                }

                #endregion

                Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle = new Dominio.ObjetosDeValor.WebService.CTe.CTeOracle();
                cceOracle.StatusIntegrador = status == "authorized" ? "M" : "R";
                cceOracle.CodStatusEnvio = codStatusProtocolo;
                cceOracle.DescricaoEnvio = descricaoProtocolo;
                cceOracle.NumeroProtocolo = protocoloAutorizacao;
                cceOracle.DataProtocolo = dataProtocolo.ToString("dd/MM/yyyy HH:mm:ss");
                cceOracle.CodStatusProtocolo = codStatusProtocolo;
                cceOracle.DescricaoProtocolo = descricaoProtocolo;
                cceOracle.XML = this.ObterXMLEmissorAsync(urlDownloadXML).GetAwaiter().GetResult();

                Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);
                return svcCCe.ReceberEventoCCe(out mensagemErro, out Exception exception, cceOracle, Auditado, unitOfWork, ref cce);
            }
            else
            {
                Log.TratarErro($"ReceberEventoCTe - Mensagem: Evento não homologado, Evento: {eventName}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Evento não homologado.";
                return false;
            }

            return true;
        }

        public bool ProcessarRecebimentoDacte(out string mensagemErro, string tipoEvento, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, dynamic objetoRetorno, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            string idDocumento = (string)objetoRetorno.id;

            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                if (retornoEventoCTe != null)
                    Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            int codigoCTe = 0;
            if (!int.TryParse((string)objetoRetorno.data?.externalId ?? (string)objetoRetorno.externalId, out codigoCTe))
            {
                if (retornoEventoCTe != null)
                    Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                return false;
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string chaveAcessoCTe = string.Empty;
            string urlDownloadDacte = string.Empty;

            if (status == "authorized")
            {
                chaveAcessoCTe = (string)objetoRetorno.data?.cteKey ?? (string)objetoRetorno.cteKey;
                urlDownloadDacte = (string)objetoRetorno.data?.dacte ?? (string)objetoRetorno.dacte;
            }

            #region Localizar Documento

            if (cte == null)
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                cte = repCTe.BuscarPorCodigoComFetchEmpresa(codigoCTe);

                if (cte == null && !string.IsNullOrWhiteSpace(chaveAcessoCTe))
                    cte = repCTe.BuscarPorChaveComFetchEmpresa(chaveAcessoCTe);

                if (cte == null)
                {
                    mensagemErro = "CTe " + (string.IsNullOrEmpty(chaveAcessoCTe) ? "código " + codigoCTe.ToString() : chaveAcessoCTe) + " não localizado na base SqlServer";
                    return false;
                }
            }

            #endregion

            Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle = new Dominio.ObjetosDeValor.WebService.CTe.CTeOracle();
            cteOracle.PDFDacte = null;
            byte[] dacte = this.obterDacte(urlDownloadDacte);

            if (dacte == null)
            {
                mensagemErro = "CTe " + (string.IsNullOrEmpty(chaveAcessoCTe) ? "código " + codigoCTe.ToString() : chaveAcessoCTe) + " não foi possível obter a dacte";
                return false;
            }

            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            serCTe.ObterESalvarDACTEOracle(cte, cteOracle, "", unitOfWork, dacte);

            return true;
        }

        public string ProcessarXMLCTe(out string mensagemErro, string tipoEvento, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, dynamic objetoRetorno, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool retornarXMLCancelamento = false)
        {
            mensagemErro = string.Empty;
            int codigoCTe = 0;
            if (((string)objetoRetorno.data?.status ?? (string)objetoRetorno.status) == null)
            {
                if (retornoEventoCTe != null)
                    Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o objeto data, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                throw new Exception(mensagemErro);
            }

            if (!int.TryParse((string)objetoRetorno.data?.externalId ?? (string)objetoRetorno.externalId, out codigoCTe))
            {
                if (retornoEventoCTe != null)
                    Log.TratarErro($"ReceberCTe - Mensagem: Não foi possível obter o externalID, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Não foi possível obter o externalID.";
                throw new Exception(mensagemErro);
            }

            string status = (string)objetoRetorno.data?.status ?? (string)objetoRetorno.status;
            string chaveAcessoCTe = string.Empty;
            string urlXmlAutorizacao = string.Empty;
            string urlXmlCancelamento = string.Empty;

            if (status == "authorized" || status == "canceled")
            {
                chaveAcessoCTe = (string)objetoRetorno.data?.cteKey ?? (string)objetoRetorno.cteKey;
                urlXmlAutorizacao = (string)(objetoRetorno.data?.xml ?? objetoRetorno.xml);
            }

            #region Localizar Documento

            if (cte == null)
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null && !string.IsNullOrWhiteSpace(chaveAcessoCTe))
                    cte = repCTe.BuscarPorChave(chaveAcessoCTe);

                if (cte == null)
                {
                    mensagemErro = "CTe " + (string.IsNullOrEmpty(chaveAcessoCTe) ? "código " + codigoCTe.ToString() : chaveAcessoCTe) + " não localizado na base SqlServer";
                    throw new Exception(mensagemErro);
                }
            }

            #endregion

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoSGT = repConfiguracaoSGT.BuscarConfiguracaoPadrao();
            bool armazenarEmArquivo = configuracaoSGT?.ArmazenarXMLCTeEmArquivo ?? false;

            cte.UrlDownloadXml = urlXmlAutorizacao;

            Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle = new Dominio.ObjetosDeValor.WebService.CTe.CTeOracle();
            cteOracle.XML = ObterESalvarXmlUrlEmissor(cte, cteOracle, armazenarEmArquivo, unitOfWork);

            if (objetoRetorno?.events != null)
            {
                // Filtrando eventos com eventName "cancel"
                dynamic eventoCancelado = ((JArray)objetoRetorno["events"])
                    .FirstOrDefault(e => (string)e["eventName"] == "cancel");

                if (eventoCancelado != null)
                {
                    urlXmlCancelamento = (string)(eventoCancelado.data?.xml ?? eventoCancelado.xml);
                    cteOracle.XMLCancelamento = this.ObterXMLEmissorAsync(urlXmlCancelamento).GetAwaiter().GetResult();

                    Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                    serCTe.ObterESalvarXMLCancelamentoInutilizacao(ref cte, cteOracle, armazenarEmArquivo, unitOfWork);
                }
            }

            return retornarXMLCancelamento ? cteOracle.XMLCancelamento : cteOracle.XML;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private string ObterESalvarXmlUrlEmissor(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle, bool armazenarEmArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            cteOracle ??= new Dominio.ObjetosDeValor.WebService.CTe.CTeOracle();

            cteOracle.XML = this.ObterXMLEmissorAsync(cte.UrlDownloadXml).GetAwaiter().GetResult();

            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            serCTe.ObterESalvarXMLAutorizacaoOracle(cte, armazenarEmArquivo, cteOracle, unitOfWork);

            return cteOracle.XML;
        }

        private async Task<string> ObterXMLEmissorAsync(string urlDownloadXML)
        {
            if (string.IsNullOrWhiteSpace(urlDownloadXML))
                return null;

            try
            {
                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoNSTech));
                var response = await client.GetAsync(urlDownloadXML).ConfigureAwait(false);

                // Lança exceção para códigos HTTP de erro
                response.EnsureSuccessStatusCode();

                string conteudo = await response.Content.ReadAsStringAsync();
                return conteudo;
            }
            catch (HttpRequestException ex)
            {
                Servicos.Log.TratarErro($"Erro de requisição HTTP: {ex.Message}");
                Servicos.Log.TratarErro(ex);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro inesperado: {ex.Message}");
                Servicos.Log.TratarErro(ex);
            }

            return null;
        }

        private byte[] obterDacte(string urlDownloadDacte)
        {
            byte[] retorno = null;

            if (string.IsNullOrEmpty(urlDownloadDacte))
                return null;

            try
            {
                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoNSTech));
                retorno = client.GetByteArrayAsync(urlDownloadDacte).Result;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao fazer download DACTE via HttpClient: {ex.ToString()}", "CatchNoAction");
            }

            return retorno;
        }

        #endregion Métodos Privados
    }
}

