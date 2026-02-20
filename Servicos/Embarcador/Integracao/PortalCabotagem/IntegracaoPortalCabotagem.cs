using Azure.Storage.Blobs;
using Confluent.Kafka;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Servicos.Embarcador.Integracao.PortalCabotagem
{
    public class IntegracaoPortalCabotagem
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private DeliveryReport<Null, string> _deliveryHandler;
        private string _numeroBookingAtual;

        #endregion Atributos

        #region Construtores

        public IntegracaoPortalCabotagem(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void Integrar(int codigoCte, bool integrarXML = true)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repConfiguracaoIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem configuracaoIntegracaoPortalCabotagem = repConfiguracaoIntegracaoPortalCabotagem.BuscarTodos().FirstOrDefault();

                if (integrarXML)
                {
                    var xml = ObterXml(codigoCte);
                    EfetuarIntegracaoPortalCabotagem(configuracaoIntegracaoPortalCabotagem, xml.Item1, xml.Item2);
                }

                var dacte = ObterDacte(codigoCte);
                EfetuarIntegracaoPortalCabotagem(configuracaoIntegracaoPortalCabotagem, dacte.Item1, dacte.Item2);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Tuple<byte[], string> ObterXml(int codigoCte)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCte);

            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);

            byte[] data = svcCTe.ObterXMLAutorizacao(cte, _unitOfWork);

            string nomeArquivoDownload = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "xml");

            if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                nomeArquivoDownload = string.Concat(cte.Chave, ".xml");

            return new Tuple<byte[], string>(data, nomeArquivoDownload);
        }

        private Tuple<byte[], string> ObterDacte(int codigoCte)
        {
            string nomeDocumento = "DACTE";

            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCarga = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Servicos.DACTE svcDACTE = new Servicos.DACTE(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCte);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorCTe(codigoCte);

            List<int> CodigosCte = new List<int> { codigoCte };
            // TODO: ToList cast
            List<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)> ctesImprimirTabelaTemperatura = repCTe.BuscarInformacaoImpressaoTabelaTemperaturaVersoCTe(CodigosCte).ToList();
            bool imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cte.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);
            string nomeArquivoFisico = cte.Chave;
            byte[] pdf = null;

            if (cte.ModeloDocumentoFiscal.DocumentoTipoCRT)
            {                
                pdf = new Servicos.Embarcador.Carga.Impressao(_unitOfWork).ObterPDFCRT(cte, carga);
                nomeArquivoFisico = cte.NumeroCRT;

                return new Tuple<byte[], string>(pdf, nomeArquivoFisico + ".pdf");

            }
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
            {
                nomeDocumento = cte.ModeloDocumentoFiscal.Abreviacao;
                nomeArquivoFisico = cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao;

                if (!string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                {
                    byte[] arquivo = new Servicos.Embarcador.Relatorios.OutrosDocumentos(_unitOfWork).ObterPdf(cte);

                    return new Tuple<byte[], string>(arquivo, nomeArquivoFisico + ".pdf");
                }
            }

            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;


            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                nomeArquivoFisico = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString();

            if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                nomeArquivoFisico = nomeArquivoFisico + "_Canc";

            if (cte.Status == "F")
                nomeArquivoFisico = nomeArquivoFisico + "_FSDA";

            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
            {
                Servicos.NFSe svcNFSe = new Servicos.NFSe(_unitOfWork);
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    pdf = svcNFSe.ObterDANFSECTe(cte.Codigo, null, true);
                else
                    pdf = svcNFSe.ObterDANFSECTe(cte.Codigo);
            }
            else
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {

                    pdf = svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoTMS.GerarPDFCTeCancelado);
                }
                else
                {
                    pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }
            }

            byte[] arquivoCCe = null;

            if (configuracaoTMS.ImprimirDACTEeCartaCorrecaoJunto)
            {
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);

                if (cce != null)
                {
                    var resultReportApi = ReportRequest.WithType(ReportType.CCe)
                                                .WithExecutionType(ExecutionType.Sync)
                                                .AddExtraData("codigoCCe", cce.Codigo)
                                                .CallReport();

                    if (resultReportApi == null)
                        throw new Exception();

                    arquivoCCe = resultReportApi.GetContentFile();
                }
            }

            byte[] versoCTe = null;

            if (imprimirTabelaTemperaturaNoVersoCTe)
            {
                versoCTe = ReportRequest.WithType(ReportType.RegistroTemperaturaETrocaDeGelo)
                    .WithExecutionType(ExecutionType.Sync)
                    .CallReport()
                    .GetContentFile();
            }


            string nomeArquivoDownload = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "pdf");

            if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                nomeArquivoDownload = System.IO.Path.GetFileName(caminhoPDF);

            List<byte[]> sourceFiles = new List<byte[]>();
            sourceFiles.Add(pdf);
            if (arquivoCCe != null)
                sourceFiles.Add(arquivoCCe);
            if (versoCTe != null)
                sourceFiles.Add(versoCTe);

            byte[] pdfAgrupado = svcDACTE.MergeFiles(sourceFiles);


            return new Tuple<byte[], string>(pdfAgrupado, nomeArquivoDownload);
        }

        private void EfetuarIntegracaoPortalCabotagem(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem configuracaoIntegracaoPortalCabotagem, byte[] arquivo, string nomeArquivo)
        {
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={configuracaoIntegracaoPortalCabotagem.StorageAccount};AccountKey={configuracaoIntegracaoPortalCabotagem.Secret};EndpointSuffix=core.windows.net";
            string containerName = configuracaoIntegracaoPortalCabotagem.Container;

            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                 containerClient.CreateIfNotExists();

                BlobClient blobClient = containerClient.GetBlobClient(nomeArquivo);

                Stream stream = new MemoryStream(arquivo);

                blobClient.Upload(stream);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                throw new ServicoException(ex.Message);
            }
        }

        #endregion Métodos Privados
    }
}