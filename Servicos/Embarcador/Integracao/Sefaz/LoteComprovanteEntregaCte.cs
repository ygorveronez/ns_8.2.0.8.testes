using Dominio.ObjetosDeValor.Embarcador.Integracao.Sefaz;
using MultiSoftware.CTe.Servicos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Sefaz
{
    public class LoteComprovanteEntregaCte
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        EventoComprovanteEntregaCte.TEvento _eventoCte;

        #endregion

        #region Construtores

        public LoteComprovanteEntregaCte(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Metodos Públicos

        public static void VerificarIntegracoesComprovanteEntrega(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao> loteIntegracoes = repLoteComprovanteEntrega.BuscarLoteComprovanteEntregaPendente(5, 5, "Codigo", "asc", 20, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual);

            for (int i = 0; i < loteIntegracoes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao LoteXMLIntegracao = loteIntegracoes[i];
                if (LoteXMLIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Sefaz)
                    new Integracao.Sefaz.LoteComprovanteEntregaCte(unidadeTrabalho, tipoServicoMultisoftware).IntegracarComprovanteEntregaLoteCte(LoteXMLIntegracao);
                else
                {
                    LoteXMLIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    LoteXMLIntegracao.ProblemaIntegracao = "Tipo de integração não implementada";
                    repLoteComprovanteEntrega.Atualizar(LoteXMLIntegracao);
                }
            }
        }

        public void IntegracarComprovanteEntregaLoteCte(Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao loteIntegracao)
        {
            Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(_unitOfWork);
            try
            {

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Sefaz repSefaz = new Repositorio.Sefaz(_unitOfWork);
                Hubs.ComprovanteEntrega svcHubComprovanteEntrega = new Hubs.ComprovanteEntrega();

                if (loteIntegracao.XMLNotaFiscalComprovanteEntrega?.Cte?.CargaCTe == null)
                {
                    loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    loteIntegracao.ProblemaIntegracao = "Dados inválidos";
                    loteIntegracao.NumeroTentativas = +1;
                    repLoteComprovanteEntrega.Atualizar(loteIntegracao);

                    return;
                }

                MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb tipoAmbiente = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb.Item2; //homologação
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(loteIntegracao.XMLNotaFiscalComprovanteEntrega.Cte.CargaCTe.CTe.Empresa.CNPJ);
                Dominio.Entidades.Estado EstadoEmpresa = repEstado.BuscarPorSigla(empresa.Localidade.Estado.Sigla);
                Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega notaFiscal = loteIntegracao.XMLNotaFiscalComprovanteEntrega;
                string URLSefaz = string.Empty;


                //string base64Imagem = srvLoteComprovanteEntrega.ObterMiniaturaNotaFiscal(notaFiscal);
                //string base64Imagem = loteIntegracao.XMLNotaFiscalComprovanteEntrega.NomeArquivoImagem;
                string base64Imagem = ObterImagem(loteIntegracao.XMLNotaFiscalComprovanteEntrega.NomeArquivoImagem);

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                    tipoAmbiente = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb.Item1; //producao

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && EstadoEmpresa.SefazCTeHomologacao == null)
                {
                    loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    loteIntegracao.ProblemaIntegracao = "Empresa não possui configuração para emissao Sefaz CTe no ambiente Homologacao";
                    loteIntegracao.NumeroTentativas = +1;
                    repLoteComprovanteEntrega.Atualizar(loteIntegracao);
                    return;
                }
                else if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && EstadoEmpresa.SefazCTe == null)
                {
                    loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    loteIntegracao.ProblemaIntegracao = "Empresa não possui configuração para emissao Sefaz CTe no ambiente Produção";
                    loteIntegracao.NumeroTentativas = +1;
                    repLoteComprovanteEntrega.Atualizar(loteIntegracao);
                    return;
                }

                if (tipoAmbiente == MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb.Item1)
                    URLSefaz = EstadoEmpresa.SefazCTe.UrlRecepcaoEvento;
                else
                    URLSefaz = EstadoEmpresa.SefazCTeHomologacao.UrlRecepcaoEvento;

                loteIntegracao.DataIntegracao = DateTime.Now;
                loteIntegracao.NumeroTentativas++;

                MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TCOrgaoIBGE ufIBGE = (MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TCOrgaoIBGE)EstadoEmpresa.CodigoIBGE;

                _eventoCte = null;
                EventoComprovanteEntregaCte.TRetEvento retorno = EnviarComprovanteEntregaCte(ref loteIntegracao, tipoAmbiente, empresa.NomeCertificado, empresa.SenhaCertificado, ufIBGE, URLSefaz, base64Imagem);
                if (retorno != null)
                {
                    XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(EventoComprovanteEntregaCte.TEvento));
                    XmlDocument docEnvio = new XmlDocument();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        xmlSerializerEnvio.Serialize(memoryStream, _eventoCte);
                        memoryStream.Position = 0;
                        docEnvio.Load(memoryStream);
                    }

                    if (retorno.infEvento.cStat == "135" || retorno.infEvento.cStat == "136")
                    {
                        //autorizado.
                        EventoComprovanteEntregaCte.procEventoCTe eventoProcessado = new EventoComprovanteEntregaCte.procEventoCTe();
                        eventoProcessado.versao = "3.00";
                        eventoProcessado.retEventoCTe = retorno;
                        eventoProcessado.eventoCTe = _eventoCte;

                        XmlSerializer xmlSerializerProtocolado = new XmlSerializer(typeof(EventoComprovanteEntregaCte.procEventoCTe));
                        XmlDocument docProtocolado = new XmlDocument();
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            xmlSerializerProtocolado.Serialize(memoryStream, eventoProcessado);
                            memoryStream.Position = 0;
                            docProtocolado.Load(memoryStream);
                        }

                        loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        loteIntegracao.ProblemaIntegracao = "";
                        SalvarArquivosIntegracao(loteIntegracao, docEnvio, docProtocolado, _unitOfWork);
                    }
                    else
                    {
                        loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        loteIntegracao.ProblemaIntegracao = retorno.infEvento.xMotivo;
                        SalvarArquivosIntegracao(loteIntegracao, docEnvio, null, _unitOfWork);
                    }

                    svcHubComprovanteEntrega.InformarComprovantesEntregaStatus(loteIntegracao.XMLNotaFiscalComprovanteEntrega.LoteComprovanteEntrega.Codigo, loteIntegracao.SituacaoIntegracao);
                }

                repLoteComprovanteEntrega.Atualizar(loteIntegracao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                loteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar" + ex.Message;
                repLoteComprovanteEntrega.Atualizar(loteIntegracao);
            }
        }

        #endregion

        #region Metodos Privados

        private string ObterImagem(string nomeArquivo)
        {
            if (Path.GetExtension(nomeArquivo).ToLower() == ".tif" || Path.GetExtension(nomeArquivo).ToLower() == ".jpg")
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                    return null;

                using (System.Drawing.Image image = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeArquivo)))
                {
                    if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
                    {
                        string tmp = Path.GetTempFileName();

                        Utilidades.IO.FileStorageService.Storage.SaveImage(tmp, image, System.Drawing.Imaging.ImageFormat.Png);
                        
                        nomeArquivo = tmp;
                    }
                }
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return null;

            string newTemp = Path.GetTempFileName();

            using (Image imgPhoto = Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeArquivo)))
            {
                Bitmap newImage = ResizeImage(imgPhoto, 600);

                Utilidades.IO.FileStorageService.Storage.SaveImage(newTemp, newImage);
            }

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(newTemp);

            return Convert.ToBase64String(imageArray);
        }

        private Bitmap ResizeImage(Image image, int newWidth)
        {
            int newHeight = (image.Height * newWidth) / image.Width;
            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        private EventoComprovanteEntregaCte.TRetEvento EnviarComprovanteEntregaCte(ref Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao loteIntegracao, MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb ambienteIntegracao, string caminhoCertificado, string senhaCertificado, MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TCOrgaoIBGE ufEmpresa, string urlSefaz, string base64Imagem)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback +=
                    new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);
                Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega XMLNotaFiscalComprovanteEntrega = loteIntegracao.XMLNotaFiscalComprovanteEntrega;

                string chaveCte = "";
                string protCte = "";
                if (XMLNotaFiscalComprovanteEntrega.Cte != null)
                {
                    chaveCte = XMLNotaFiscalComprovanteEntrega.Cte.CargaCTe.CTe.Chave;
                    protCte = XMLNotaFiscalComprovanteEntrega.Cte.CargaCTe.CTe.Protocolo;
                }

                string CnpjEmpresa = XMLNotaFiscalComprovanteEntrega.Cte.CargaCTe.CTe.Empresa.CNPJ; //loteIntegracao.Carga.Empresa.CNPJ;
                string id = "ID110180" + chaveCte + "01";
                string hashEntrega = GetBase64EncodedSHA1Hash(chaveCte + base64Imagem);

                List<EventoComprovanteEntregaCte.TInfEntrega> chavesNF = new List<EventoComprovanteEntregaCte.TInfEntrega>();
                EventoComprovanteEntregaCte.TInfEntrega chave = new EventoComprovanteEntregaCte.TInfEntrega
                {
                    chNFe = loteIntegracao.XMLNotaFiscalComprovanteEntrega?.PedidoXMLNotaFiscal?.XMLNotaFiscal.Chave
                };
                chavesNF.Add(chave);
                string datahora = DateTime.Now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:sszzz");

                List<EventoComprovanteEntregaCte.TevCECTe> ListEvento = new List<EventoComprovanteEntregaCte.TevCECTe>();
                EventoComprovanteEntregaCte.TevCECTe evCECTe = new EventoComprovanteEntregaCte.TevCECTe
                {
                    descEvento = "Comprovante de Entrega do CT-e",
                    nProt = protCte,
                    dhEntrega = datahora,
                    nDoc = XMLNotaFiscalComprovanteEntrega.XMLNotaFiscal.Numero.ToString(),
                    xNome = XMLNotaFiscalComprovanteEntrega.DadosRecebedor.Nome.Trim(),
                    latitude = XMLNotaFiscalComprovanteEntrega.Latitude.Length > 9 ? XMLNotaFiscalComprovanteEntrega.Latitude.Substring(0, 10) : XMLNotaFiscalComprovanteEntrega.Latitude,
                    longitude = XMLNotaFiscalComprovanteEntrega.Longitude.Length > 9 ? XMLNotaFiscalComprovanteEntrega.Longitude.Substring(0, 10) : XMLNotaFiscalComprovanteEntrega.Longitude,
                    hashEntrega = hashEntrega,
                    dhHashEntrega = datahora,
                    infEntrega = chavesNF
                };
                ListEvento.Add(evCECTe);

                EventoComprovanteEntregaCte.TEvento comprovanteEntregaEvento = new EventoComprovanteEntregaCte.TEvento()
                {

                    versao = "3.00",
                    infEvento = new EventoComprovanteEntregaCte.TEventoInfEvento()
                    {
                        Id = id,
                        cOrgao = ufEmpresa,
                        tpAmb = ambienteIntegracao,
                        CNPJ = CnpjEmpresa,
                        chCTe = chaveCte,
                        dhEvento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        tpEvento = "110180",
                        nSeqEvento = "1",
                        detEvento = new EventoComprovanteEntregaCte.TEventoTEventoInfEventoTDetEvento()
                        {
                            versaoEvento = "3.00",
                            evCECTe = ListEvento
                        }
                    }
                };

                MultiSoftware.CTe.ServicoRecepcaoEvento.cteCabecMsg dadosCabecalho = new MultiSoftware.CTe.ServicoRecepcaoEvento.cteCabecMsg()
                {
                    cUF = ufEmpresa.ToString("d"),
                    versaoDados = "3.00"
                };

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(EventoComprovanteEntregaCte.TEvento));
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "http://www.portalfiscal.inf.br/cte");
                XmlDocument doc = new XmlDocument();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(memoryStream, comprovanteEntregaEvento, namespaces);
                    memoryStream.Position = 0;
                    doc.Load(memoryStream);
                }

                if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
                    xmlDeclaration.Encoding = "UTF-8";
                }

                XmlElement xmlElement = Assinatura.AssinarXML(doc.GetElementsByTagName("eventoCTe")[0], doc.GetElementsByTagName("infEvento")[0], certificado);
                _eventoCte = Deserialize<EventoComprovanteEntregaCte.TEvento>(xmlElement.OuterXml);

                MultiSoftware.CTe.ServicoRecepcaoEvento.CteRecepcaoEventoSoap12Client svcRecepcaoEvento = new MultiSoftware.CTe.ServicoRecepcaoEvento.CteRecepcaoEventoSoap12Client();
                svcRecepcaoEvento.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlSefaz);
                svcRecepcaoEvento.ClientCredentials.ClientCertificate.Certificate = certificado;

                InspectorBehavior inspector = new InspectorBehavior();
                svcRecepcaoEvento.Endpoint.EndpointBehaviors.Add(inspector);

                XmlNode dadosRetorno = svcRecepcaoEvento.cteRecepcaoEvento(ref dadosCabecalho, doc.DocumentElement);

                using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(EventoComprovanteEntregaCte.TRetEvento));

                    EventoComprovanteEntregaCte.TRetEvento result = (EventoComprovanteEntregaCte.TRetEvento)ser.Deserialize(reader);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                loteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar" + ex.Message;

                return null;
            }
        }

        private static void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao loteIntegracao, XmlDocument XmlEnvio, XmlDocument xmlRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                Data = loteIntegracao.DataIntegracao,
                Mensagem = loteIntegracao.ProblemaIntegracao ?? "",
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XmlEnvio.OuterXml, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno != null ? xmlRetorno.OuterXml : "", "xml", unitOfWork),
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            loteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static T Deserialize<T>(string data) where T : class, new()
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var ser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(data))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        string GetBase64EncodedSHA1Hash(string texto)
        {
            byte[] buffer = Encoding.Default.GetBytes(texto);
            System.Security.Cryptography.SHA1CryptoServiceProvider cryptoTransformSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            return System.Convert.ToBase64String((cryptoTransformSHA1.ComputeHash(buffer)));
        }
        #endregion

    }
}
