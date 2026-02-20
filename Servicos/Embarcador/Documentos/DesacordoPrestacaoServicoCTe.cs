using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Servicos.Embarcador.Documentos
{
    public static class DesacordoPrestacaoServicoCTe
    {

        /// <summary>
        /// Evento de desacordo CT-e Versão 4.0
        /// </summary>
        /// <param name="codigoEmpresa"></param>
        /// <param name="chaveCTe"></param>
        /// <param name="justificativa"></param>
        /// <param name="urlSefaz"></param>
        /// <param name="mensagemRetorno"></param>
        /// <param name="unidadeTrabalho"></param>
        /// <returns></returns>
        public static bool EmitirDesacordoServicoVersao4(int codigoEmpresa, string chaveCTe, string justificativa, string urlSefaz, ref string mensagemRetorno, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.EventoDesacordoServico repEventoDesacordoServico = new Repositorio.Embarcador.Documentos.EventoDesacordoServico(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
                throw new Exception("Certificado da empresa é inválido ou inexistente.");

            MultiSoftware.CTe.v400.Eventos.TAmb tipoAmbiente = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? MultiSoftware.CTe.v400.Eventos.TAmb.Item1 : MultiSoftware.CTe.v400.Eventos.TAmb.Item2;

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            MultiSoftware.CTe.v400.Eventos.TCOrgaoIBGE ufIBGE = (MultiSoftware.CTe.v400.Eventos.TCOrgaoIBGE)int.Parse(chaveCTe.Substring(0, 2));//  empresa.Localidade.Estado.CodigoIBGE;

            string xmlEvento = "";
            MultiSoftware.CTe.v400.Eventos.Desacordo.TRetEvento retorno = EnviarDesacordoPrestacaoServico(empresa.CNPJ, ufIBGE, tipoAmbiente, chaveCTe, dataFuso, repEventoDesacordoServico.BuscarUltimoIdLote(empresa.Codigo) + 1, justificativa, empresa.NomeCertificado, empresa.SenhaCertificado, urlSefaz, ref xmlEvento, unidadeTrabalho);

            Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico eventoDesacordo = new Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico();
            eventoDesacordo.Empresa = empresa;
            eventoDesacordo.Ambiente = empresa.TipoAmbiente;
            eventoDesacordo.ChaveCTe = chaveCTe;
            eventoDesacordo.CodigoStatusResposta = retorno.infEvento.cStat;
            DateTime.TryParseExact(retorno.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTime dataEvento);
            eventoDesacordo.DataAutorizacao = dataEvento > DateTime.MinValue ? dataEvento : DateTime.Now;
            eventoDesacordo.DataEmissao = dataFuso;
            eventoDesacordo.DescricaoStatusResposta = retorno.infEvento.xMotivo;
            eventoDesacordo.Justificativa = justificativa;
            eventoDesacordo.NumeroSequencialEvento = retorno.infEvento.nSeqEvento != null ? int.Parse(retorno.infEvento.nSeqEvento) : 0;
            eventoDesacordo.Protocolo = !string.IsNullOrWhiteSpace(retorno.infEvento.nProt) ? retorno.infEvento.nProt : string.Empty;
            eventoDesacordo.VersaoAplicacao = retorno.infEvento.verAplic;

            if (eventoDesacordo.CodigoStatusResposta == "135" || eventoDesacordo.CodigoStatusResposta == "136")
            {
                eventoDesacordo.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Autorizado;
                eventoDesacordo.XML = xmlEvento;
            }
            else
            {
                eventoDesacordo.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Rejeitado;
                mensagemRetorno = eventoDesacordo.DescricaoStatusResposta;
                eventoDesacordo.XML = xmlEvento;
            }

            repEventoDesacordoServico.Inserir(eventoDesacordo);

            return string.IsNullOrWhiteSpace(mensagemRetorno);
        }

        public static bool EmitirDesacordoServico(int codigoEmpresa, string chaveCTe, string justificativa, string urlSefaz, ref string mensagemRetorno, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.EventoDesacordoServico repEventoDesacordoServico = new Repositorio.Embarcador.Documentos.EventoDesacordoServico(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
                throw new Exception("Certificado da empresa é inválido ou inexistente.");


            MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio.TAmb tipoAmbiente = MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio.TAmb.Item2; //homologação
            if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                tipoAmbiente = MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio.TAmb.Item1; //producao

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio.TCOrgaoIBGE ufIBGE = (MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio.TCOrgaoIBGE)int.Parse(chaveCTe.Substring(0, 2));//  empresa.Localidade.Estado.CodigoIBGE;

            string xmlEvento = "";
            MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Retorno.TRetEvento retorno = MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Servicos.PrestacaoServicoDesacordo.EnviarDesacordoPrestacaoServico(empresa.CNPJ, ufIBGE, tipoAmbiente, chaveCTe, dataFuso, repEventoDesacordoServico.BuscarUltimoIdLote(empresa.Codigo) + 1, justificativa, empresa.NomeCertificado, empresa.SenhaCertificado, urlSefaz, ref xmlEvento);

            Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico eventoDesacordo = new Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico();
            eventoDesacordo.Empresa = empresa;
            eventoDesacordo.Ambiente = empresa.TipoAmbiente;
            eventoDesacordo.ChaveCTe = chaveCTe;
            eventoDesacordo.CodigoStatusResposta = retorno.infEvento.cStat;
            DateTime.TryParseExact(retorno.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTime dataEvento);
            eventoDesacordo.DataAutorizacao = dataEvento > DateTime.MinValue ? dataEvento : DateTime.Now;
            eventoDesacordo.DataEmissao = dataFuso;
            eventoDesacordo.DescricaoStatusResposta = retorno.infEvento.xMotivo;
            eventoDesacordo.Justificativa = justificativa;
            eventoDesacordo.NumeroSequencialEvento = retorno.infEvento.nSeqEvento != null ? int.Parse(retorno.infEvento.nSeqEvento) : 0;
            eventoDesacordo.Protocolo = !string.IsNullOrWhiteSpace(retorno.infEvento.nProt) ? retorno.infEvento.nProt : string.Empty;
            eventoDesacordo.VersaoAplicacao = retorno.infEvento.verAplic;

            if (eventoDesacordo.CodigoStatusResposta == "135" || eventoDesacordo.CodigoStatusResposta == "136")
            {
                eventoDesacordo.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Autorizado;
                eventoDesacordo.XML = xmlEvento;
            }
            else
            {
                eventoDesacordo.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Rejeitado;
                mensagemRetorno = eventoDesacordo.DescricaoStatusResposta;
                eventoDesacordo.XML = xmlEvento;
            }

            repEventoDesacordoServico.Inserir(eventoDesacordo);

            return string.IsNullOrWhiteSpace(mensagemRetorno);
        }

        public static Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo EnviarParaAprovacao(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivoDesacordo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool tipoUnilever = repTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever);
            Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo retorno = new Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo() { status = null, mensagem = string.Empty };

            if (!tipoUnilever)
                return retorno;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Servicos.Embarcador.Documentos.ControleDocumento svcControleDocumento = new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPrimeiroQualquerPorChaveCTe(documento.Chave);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(documento.Chave);

            if (documento.DataEmissao == null || (documento.DataEmissao <= DateTime.Today.AddDays(-45)))
            {
                retorno.status = false;
                retorno.mensagem = "O documento foi emitido há mais de 45 dias";
                return retorno;
            }

            if (documentoFaturamento != null && documentoFaturamento?.DataMiro != null && documentoFaturamento.Situacao == SituacaoDocumentoFaturamento.Cancelado)
            {
                retorno.status = false;
                retorno.mensagem = "A MIRO precisa estar estornada para realizar a operação";
                return retorno;
            }

            if (!(cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal || cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento))
            {
                retorno.status = false;
                retorno.mensagem = "O documento deve ser do tipo Normal ou Complementar";
                return retorno;
            }

            unitOfWork.Start();


            Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo retornoControleDocumento = svcControleDocumento.ControleEmissaoDesacordo(cte, motivoDesacordo);
            if (retornoControleDocumento.status != true)
            {
                retorno.status = false;
                retorno.mensagem = retornoControleDocumento.mensagem;
                return retorno;
            }

            documento.SituacaoManifestacaoDestinatario = SituacaoManifestacaoDestinatario.AgAprovacaoDesacordoServico;

            Servicos.Auditoria.Auditoria.Auditar(auditado, documento, null, "Desacordo Enviado para aprovação.", unitOfWork);
            repDocumentoDestinadoEmpresa.Atualizar(documento);

            unitOfWork.CommitChanges();


            retorno.status = true;
            retorno.mensagem = "Documento enviado para aprovação";
            return retorno;
        }

        private static MultiSoftware.CTe.v400.Eventos.Desacordo.TRetEvento EnviarDesacordoPrestacaoServico(string cnpj, MultiSoftware.CTe.v400.Eventos.TCOrgaoIBGE uf, MultiSoftware.CTe.v400.Eventos.TAmb tpAmb, string chaveCTe, DateTime dataEmissao, long idLote, string justificativa, string caminhoCertificado, string senhaCertificado, string urlSefaz, ref string xmlEvento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

            //#if DEBUG
            //            chaveCTe = "52241013252764000116570010000065801000192002";
            //#else
            //            chaveCTe = chaveCTe;
            //#endif

            string id = "ID610110" + chaveCTe + "001";

            MultiSoftware.CTe.v400.Eventos.Desacordo.TEvento eventoCTe = new MultiSoftware.CTe.v400.Eventos.Desacordo.TEvento()
            {
                versao = "4.00",
                infEvento = new MultiSoftware.CTe.v400.Eventos.Desacordo.TEventoInfEvento()
                {
                    Id = id,
                    cOrgao = uf,
                    tpAmb = tpAmb,
                    Item = cnpj,
                    ItemElementName = MultiSoftware.CTe.v400.Eventos.ItemChoiceType.CNPJ,
                    chCTe = chaveCTe,
                    dhEvento = dataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    tpEvento = "610110",
                    nSeqEvento = "1",
                    detEvento = new MultiSoftware.CTe.v400.Eventos.Desacordo.TEventoInfEventoDetEvento()
                    {
                        versaoEvento = "4.00",
                        evPrestDesacordo = new MultiSoftware.CTe.v400.Eventos.PrestacaoServicoDesacordo.evPrestDesacordo()
                        {
                            descEvento = MultiSoftware.CTe.v400.Eventos.PrestacaoServicoDesacordo.evPrestDesacordoDescEvento.PrestaçãodoServiçoemDesacordo,
                            xObs = justificativa,
                            indDesacordoOper = MultiSoftware.CTe.v400.Eventos.PrestacaoServicoDesacordo.evPrestDesacordoIndDesacordoOper.Item1
                        }
                    },

                }
            };

            MultiSoftware.CTe.ServicoRecepcaoEvento.cteCabecMsg dadosCabecalho = new MultiSoftware.CTe.ServicoRecepcaoEvento.cteCabecMsg()
            {
                cUF = uf.ToString("d"),
                versaoDados = "4.00"
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(MultiSoftware.CTe.v400.Eventos.Desacordo.TEvento));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://www.portalfiscal.inf.br/cte");

            XmlDocument doc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, eventoCTe, namespaces);

                memoryStream.Position = 0;

                doc.Load(memoryStream);
            }

            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
                xmlDeclaration.Encoding = "UTF-8";
            }

            XmlDocument xmlElementManual = MultiSoftware.CTe.Servicos.Assinatura.AssinarXmlManual(doc, certificado, id);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificado);
            XmlNode dadosRetorno = null;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("SOAPAction", "\"http://www.portalfiscal.inf.br/cte/wsdl/CTeRecepcaoEventoV4/cteRecepcaoEvento\"");

                string soapMessage = @"<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope""><s:Header><Action mustUnderstand=""1"" xmlns=""http://www.w3.org/2005/08/addressing"">http://www.portalfiscal.inf.br/cte/wsdl/CTeRecepcaoEventoV4/cteRecepcaoEvento</Action><VsDebuggerCausalityData xmlns=""http://schemas.microsoft.com/vstudio/diagnostics/servicemodelsink"">uIDPoxKdZCeOGvpNopDw+y8mNLUAAAAAoCLNGCjDPUmxwt7WCIXmB8i286nc769DrmMbPcT3UzkACQAA</VsDebuggerCausalityData></s:Header><s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><cteDadosMsg xmlns=""http://www.portalfiscal.inf.br/cte/wsdl/CTeRecepcaoEventoV4"">" + doc.GetElementsByTagName("eventoCTe")[0].OuterXml + "</cteDadosMsg></s:Body></s:Envelope>";

                var content = new StringContent(soapMessage, Encoding.UTF8, "application/soap+xml");
                var url = "https://cte.svrs.rs.gov.br/ws/CTeRecepcaoEventoV4/CTeRecepcaoEventoV4.asmx";

                try
                {
                    string xmlEnvio = soapMessage;
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();

                    string xmlRetorno = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    dadosRetorno = Utilidades.XML.StringParaXmlNode(xmlRetorno);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                    else
                    {
                        string errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Log.TratarErro("Falha no desacordo do CT-e : " + errorContent);
                    }

                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("Falha ao realizar comunicação com a SEFAZ " + ex.Message);
                }
            }

            if (dadosRetorno == null)
            {
                Log.TratarErro("Falha no desacordo do CT-, não teve retorno");
                throw new Exception("Falha ao realizar comunicação com a SEFAZ");                
            }

            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v400.Eventos.Desacordo.TRetEvento));

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            nsmgr.AddNamespace("pf", "http://www.portalfiscal.inf.br/cte/wsdl/CTeRecepcaoEventoV4");
            nsmgr.AddNamespace("cte", "http://www.portalfiscal.inf.br/cte");

            // Seleciona o nó infEvento usando o caminho completo com namespaces
            XmlNode infEventoNode = dadosRetorno.SelectSingleNode("//soap:Envelope/soap:Body/pf:cteRecepcaoEventoResult/cte:retEventoCTe", nsmgr);

            using (TextReader reader = new StringReader(infEventoNode.OuterXml))
            {
                MultiSoftware.CTe.v400.Eventos.Desacordo.TRetEvento result = (MultiSoftware.CTe.v400.Eventos.Desacordo.TRetEvento)ser.Deserialize(reader);

                if (result.infEvento.cStat == "135" || result.infEvento.cStat == "136" || result.infEvento.cStat == "631")
                {
                    if (result.infEvento.cStat == "631")
                    {
                        string dataEvento = string.Empty;
                        string protocoloEvento = string.Empty;

                        string pattern = @"(?:\[nProt:([0-9]+)\])(?:\[dhRegEvento:(.{19})\])";  //(?:\[dhRegEvento:(.{19}))

                        MatchCollection matches = Regex.Matches(result.infEvento.xMotivo, pattern);

                        if (matches.Count > 0)
                        {
                            protocoloEvento = matches[0].Groups[1].Value;
                            dataEvento = matches[0].Groups[2].Value;
                        }

                        result.infEvento.cStat = "135";
                        result.infEvento.xMotivo = "Evento registrado e vinculado a CT-e";
                        result.infEvento.chCTe = chaveCTe;
                        result.infEvento.tpEvento = "610110";
                        result.infEvento.xEvento = "Prestação Serviço em Desacordo";
                        result.infEvento.nSeqEvento = "1";
                        result.infEvento.nProt = protocoloEvento;
                        result.infEvento.dhRegEvento = dataEvento;
                    }


                    //TextReader readerEvento = new StringReader(doc.OuterXml);                        

                    MultiSoftware.CTe.v400.Eventos.Desacordo.procEventoCTe procEventoCTe = new MultiSoftware.CTe.v400.Eventos.Desacordo.procEventoCTe()
                    {
                        eventoCTe = eventoCTe, //(Envio.TRetEvento)ser.Deserialize(readerEvento),// 
                        retEventoCTe = result,
                        versao = "4.00"
                    };

                    XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(MultiSoftware.CTe.v400.Eventos.Desacordo.procEventoCTe));
                    XmlDocument docRetorno = new XmlDocument();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        xmlSerializerRetorno.Serialize(memoryStream, procEventoCTe, namespaces);

                        memoryStream.Position = 0;

                        docRetorno.Load(memoryStream);
                    }
                    xmlEvento = docRetorno.OuterXml;
                }

                return result;
            }

        }
    }
}
