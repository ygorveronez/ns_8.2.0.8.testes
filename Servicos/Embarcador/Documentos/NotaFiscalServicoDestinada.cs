using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Repositorio;

namespace Servicos.Embarcador.Documentos
{
    public class NotaFiscalServicoDestinada : ServicoBase
    {

        public NotaFiscalServicoDestinada(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Metodos publicos

        public bool ConsultarNFSeDestinadaSaoPauloSP(DateTime dataInicial, DateTime DataFinal, int codigoEmpresa, string stringConexao)
        {

            try
            {
                Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);                
                if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
                {
                    Servicos.Log.TratarErro("Certificado vencido ou inexistente na empresa " + empresa.NomeCNPJ, "Servico NFSEs Destinadas Sao Paulo SP");
                    return false;
                }
                X509Certificate2 certificado = new X509Certificate2(empresa.NomeCertificado, empresa.SenhaCertificado);
                int numeroPagina = 1;
                Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.PedidoConsultaNFePeriodo pedido = new Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.PedidoConsultaNFePeriodo();                

                List<Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpNFe> notasRecebidas = new List<Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpNFe>();
                
                while(numeroPagina > 0)
                {
                    pedido.Cabecalho = new Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.PedidoConsultaNFePeriodoCabecalho()
                    {
                        CPFCNPJRemetente = new Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpCPFCNPJ() { Item = empresa.CNPJ_SemFormato, ItemElementName = Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.ItemChoiceType.CNPJ },
                        CPFCNPJ = new Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpCPFCNPJ() { Item = empresa.CNPJ_SemFormato, ItemElementName = Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.ItemChoiceType.CNPJ },
                        dtInicio = dataInicial,
                        dtFim = DataFinal,
                        Versao = 1,
                        NumeroPagina = numeroPagina
                    };
                    Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.RetornoConsulta retornoConsulta = EfetuarConsultaWS(pedido, certificado);

                    bool possuiNotasRecebidas = retornoConsulta.Cabecalho.Sucesso && retornoConsulta.NFe != null && retornoConsulta.NFe.Length > 0;

                    if (possuiNotasRecebidas)
                        notasRecebidas.AddRange(retornoConsulta.NFe);

                    if (possuiNotasRecebidas && retornoConsulta.NFe.Length >= 50)
                    {
                        numeroPagina = 2;
                        Thread.Sleep(5000);
                    }                        
                    else {                        
                        break;
                    }                        
                }

                return GerarDocumentoDestinadoNFSeSP(notasRecebidas, empresa ,unidadeTrabalho);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex.ToString(), "Servico NFSEs Destinadas Sao Paulo SP");
                return false;
            }

        }

        private bool GerarDocumentoDestinadoNFSeSP(List<Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpNFe> notas, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {            
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            var caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;           

            //gravar xml pasta - preparar diretorio base
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFSe");

            foreach (var nota in notas)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = null;
                    int numeroNota = 0;
                    int.TryParse(nota.ChaveNFe.NumeroNFe.ToString(), out numeroNota);
                    documento = repDocumentoDestinadoEmpresa.BuscarNFSePorCPFCNPJEmitenteNumeroNota(nota.CPFCNPJPrestador.Item, numeroNota, nota.DataEmissaoNFe);

                    if(documento == null)
                    {
                        documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                        documento.Empresa = empresa;
                        documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                        documento.DataAutorizacao = nota.DataEmissaoNFe;
                        documento.DataEmissao = nota.DataEmissaoNFe;
                        documento.NomeTomador = nota.RazaoSocialTomador;
                        documento.CPFCNPJTomador = nota.CPFCNPJTomador.Item;
                        documento.DataIntegracao = DateTime.Now;
                        documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;
                        documento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFSeDestinada;
                        documento.Chave = nota.ChaveNFe.InscricaoPrestador.ToString() + documento.DataEmissao.Value.Year + documento.DataEmissao.Value.Month + nota.ChaveNFe.NumeroNFe.ToString() + nota.ChaveNFe.CodigoVerificacao;
                        documento.CPFCNPJEmitente = nota.CPFCNPJPrestador.Item;
                        documento.NomeEmitente = nota.RazaoSocialPrestador;
                        documento.Numero = numeroNota;
                        documento.Serie = nota.ChaveRPS?.SerieRPS?.ToInt() ?? 0;
                        documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
                        documento.Valor = nota.ValorServicos;
                        documento.Observacao = nota.Discriminacao;
                        documento.CPFCNPJDestinatario = nota.CPFCNPJTomador.Item;
                        documento.Cancelado = nota.StatusNFe == Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpStatusNFe.C;
                        documento.NumeroSequencialUnico = 1;                        

                        if (nota.DataCancelamento.Year > 1990)
                            documento.DataCancelamento = nota.DataCancelamento;

                        repDocumentoDestinadoEmpresa.Inserir(documento);
                        
                        GerarControleDocumento(nota, empresa, unidadeTrabalho);
                    }
                    else if (nota.StatusNFe == Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpStatusNFe.C)
                    {
                        documento.Cancelado = true;
                        documento.DataCancelamento = nota.DataCancelamento;
                        repDocumentoDestinadoEmpresa.Atualizar(documento);
                    }
                    
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFSe", documento.Chave + "_NFSe.xml");

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpNFe));
                    var xmlString = "";

                    var settings = new XmlWriterSettings();
                    settings.Indent = false;
                    settings.OmitXmlDeclaration = true;
                    settings.Encoding = System.Text.Encoding.UTF8;

                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww, settings))
                        {
                            serializer.Serialize(writer, nota);
                            xmlString = sww.ToString(); // XML
                        }
                    }
                    //tratar xml para ficar igual ao fornecido pela prefeitura =/
                    xmlString = "<NFe xmlns=\"\">" + xmlString.Substring(106);
                    xmlString = xmlString.Replace("</tpNFe>", "</NFe>");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, xmlString);

                }
                catch(Exception ex)
                {
                    Servicos.Log.TratarErro("Falha ao gerar documento destinado: " + nota.ChaveNFe.InscricaoPrestador.ToString() + nota.ChaveRPS.SerieRPS + nota.ChaveNFe.NumeroNFe.ToString() + nota.ChaveNFe.CodigoVerificacao + "- EMPRESA:" + empresa.NomeCNPJ + " EX: " +ex.Message,"NFSeSP");                    
                }
            }
            return true;
        }

        private void GerarControleDocumento(Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpNFe nota, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.NFSe servicoNFSe = new Servicos.NFSe();
            Servicos.Embarcador.Documentos.ControleDocumento servicoControle = new Servicos.Embarcador.Documentos.ControleDocumento(unidadeTrabalho);    
            
            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoNFSe.ConverterNFSeSaoPauloSPEmCte(nota, empresa, unidadeTrabalho);
                servicoControle.GeracaoControleDocumento(cte);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao Gerar o ControleDocumento a partir do CTe de NFSe São Paulo/SP: " + ex, "NFSeSP");                    
            }
        }

        private Servicos.NFSeSaoPauloSP.LoteNFeSoapClient ObterClient()
        {
            string url = $"https://nfe.prefeitura.sp.gov.br/ws/lotenfe.asmx";

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Certificate;
            

            return new Servicos.NFSeSaoPauloSP.LoteNFeSoapClient(binding, endpointAddress);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.RetornoConsulta EfetuarConsultaWS(Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.PedidoConsultaNFePeriodo pedido, X509Certificate2 certificado)
        {
            Servicos.NFSeSaoPauloSP.LoteNFeSoapClient client = ObterClient();
            client.ClientCredentials.ClientCertificate.Certificate = certificado;

            var xmlString = "";
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.PedidoConsultaNFePeriodo));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    xsSubmit.Serialize(writer, pedido, ns);
                    xmlString = sww.ToString();
                }
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            MultiSoftware.CTe.Servicos.Assinatura.AssinarXML(xmlDoc.GetElementsByTagName("q1:PedidoConsultaNFePeriodo")[0], xmlDoc.GetElementsByTagName("Cabecalho")[0], certificado, true);

            var response = client.ConsultaNFeRecebidas(1, xmlDoc.OuterXml);
            
            Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.RetornoConsulta retConsulta = null;
            using (MemoryStream resultStream = GenerateStreamFromString(response))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.RetornoConsulta));
                retConsulta = (ser.Deserialize(resultStream) as Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.RetornoConsulta);
            }
            return retConsulta;
        }

        private static MemoryStream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion

    }
}
