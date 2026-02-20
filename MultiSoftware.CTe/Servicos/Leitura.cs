using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MultiSoftware.CTe.Servicos
{
    public static class Leitura
    {
        public static object Ler(System.IO.Stream stream)
        {
            stream.Position = 0;

            XDocument doc = null;

            // Definio inicio do XML, que pode ser 0, ou o tamanho do so cabecalho
            int inicioXML = 0;

            try
            {
                // as vezes, basta remover o cabecalho "<?xml version="1.0" encoding="UTF-8"?>"
                doc = XDocument.Load(stream);
            }
            catch (System.Exception e)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao carregar XML CTe - tentando sem cabeçalho: {e}", "CatchNoAction");
            }

            try
            {
                if (doc == null)
                {
                    // Remove Cabecalho
                    inicioXML = ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>".Length * 2) - 2; // Multiplicado por 2 pois a string do strim possui "\0" para cada caracter
                    stream.Position = inicioXML;
                    doc = XDocument.Load(stream);
                }
            }
            catch (System.Exception e)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao carregar XML CTe removendo cabeçalho: {e}", "CatchNoAction");
            }


            if (doc == null)
            {
                //Remover caracteres após a tag </cteProc>
                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string value = reader.ReadToEnd();
                    int posicaoFinal = value.IndexOf("</cteProc>");
                    if (posicaoFinal >= 0)
                    {
                        posicaoFinal += 10;
                        if (value.Length > posicaoFinal)
                        {
                            string xmlNew = value.Substring(0, posicaoFinal);

                            stream = Utilidades.String.ToStream(xmlNew);
                            doc = XDocument.Load(stream);

                        }
                    }
                }
            }             
            

            XNamespace ns = doc.Root.Name.Namespace;

            stream.Position = 0;

            if (doc.Root.Descendants(ns + "infCte").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "infCte") select ele.Attribute("versao").Value).FirstOrDefault();

                if (doc.Descendants(ns + "cteProc").Count() > 0)
                {
                    if (versao == "1.04")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporteProcessado.cteProc));
                        return (MultiSoftware.CTe.v104.ConhecimentoDeTransporteProcessado.cteProc)serializer.Deserialize(stream);
                    }

                    if (versao == "2.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc));
                        return (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)serializer.Deserialize(stream);
                    }

                    if (versao == "3.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc));
                        return (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)serializer.Deserialize(stream);
                    }

                    if(versao == "4.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc));
                        return (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)serializer.Deserialize(stream);
                    }
                }

                if (doc.Descendants(ns + "cteSimpProc").Count() > 0)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc));
                    return (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc)serializer.Deserialize(stream);
                }

                if (doc.Descendants(ns + "CTe").Count() > 0)
                {
                    if (versao == "1.04")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTe));
                        return (MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTe)serializer.Deserialize(stream);
                    }

                    if (versao == "2.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTe));
                        return (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTe)serializer.Deserialize(stream);
                    }

                    if (versao == "3.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTe));
                        return (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTe)serializer.Deserialize(stream);
                    }

                    if (versao == "4.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe));
                        return (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe)serializer.Deserialize(stream);
                    }
                }
            }

            if (doc.Descendants(ns + "procCancCTe").Count() > 0)
            {
                string versao = (from ele in doc.Root.Descendants(ns + "procCancCTe") select ele.Attribute("versao").Value).FirstOrDefault();

                if (versao == "1.04")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TProcCancCTe));
                    return (MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TProcCancCTe)serializer.Deserialize(stream);
                }
            }

            if (doc.Descendants(ns + "procEventoCTe").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "procEventoCTe") select ele.Attribute("versao")?.Value).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(versao))
                    versao = (from ele in doc.Descendants(ns + "eventoCTe") select ele.Attribute("versao")?.Value).FirstOrDefault();

                if (versao == "2.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v200.Eventos.TProcEvento));
                    return (MultiSoftware.CTe.v200.Eventos.TProcEvento)serializer.Deserialize(stream);
                }
                
                if (versao == "3.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v300.Eventos.TProcEvento));
                    return (MultiSoftware.CTe.v300.Eventos.TProcEvento)serializer.Deserialize(stream);
                }

                if(versao == "4.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v400.Eventos.TProcEvento));
                    return (MultiSoftware.CTe.v400.Eventos.TProcEvento)serializer.Deserialize(stream);
                }
            }

            if (doc.Descendants(ns + "procInutCTe").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "procInutCTe") select ele.Attribute("versao").Value).FirstOrDefault();

                if (versao == "1.04")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporteInutilizado.TProcInutCTe));
                    return (MultiSoftware.CTe.v104.ConhecimentoDeTransporteInutilizado.TProcInutCTe)serializer.Deserialize(stream);
                }

                if (versao == "2.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteInutilizado.TProcInutCTe));
                    return (MultiSoftware.CTe.v200.ConhecimentoDeTransporteInutilizado.TProcInutCTe)serializer.Deserialize(stream);
                }

                if (versao == "3.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteInutilizado.TProcInutCTe));
                    return (MultiSoftware.CTe.v300.ConhecimentoDeTransporteInutilizado.TProcInutCTe)serializer.Deserialize(stream);
                }
            }

            if (doc.Descendants(ns + "cancCTe").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "cancCTe") select ele.Attribute("versao").Value).FirstOrDefault();

                if (versao == "1.04")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TCancCTe));
                    return (MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TCancCTe)serializer.Deserialize(stream);
                }
            }

            if (doc.Descendants(ns + "cteOSProc").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "infCte") select ele.Attribute("versao").Value).FirstOrDefault();

                if (versao == "3.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc));
                    return (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc)serializer.Deserialize(stream);
                }

                if (versao == "4.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc));
                    return (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc)serializer.Deserialize(stream);
                }
            }

            return null;

        }

        public static object LerXMLEnvio(System.IO.Stream stream)
        {
            XDocument doc = XDocument.Load(stream);
            XNamespace ns = doc.Root.Name.Namespace;

            stream.Position = 0;

            XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.CTe.v200.Envio.TEnviCTe));
            return (MultiSoftware.CTe.v200.Envio.TEnviCTe)serializer.Deserialize(stream);
        }
    }
}

