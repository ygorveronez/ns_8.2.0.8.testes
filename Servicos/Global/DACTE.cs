using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos
{
    public class DACTE : ServicoBase
    {
        #region Propriedades

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo _configuracaoArquivo;

        #endregion

        #region Construtores        

        public DACTE(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public byte[] GerarPorProcesso(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho = null, bool gerarPDFCTeCancelado = false)
        {
            try
            {
                if (unidadeDeTrabalho == null)
                    unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte != null)
                {
                    string nomeArquivo = cte.Chave;

                    if (cte.ModeloDocumentoFiscal.Numero != "57")
                        nomeArquivo = cte.ModeloDocumentoFiscal.Numero + "_" + cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao + "_" + cte.Codigo;

                    if (gerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.Numero == "57")
                        nomeArquivo = nomeArquivo + "_Canc";

                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ObterConfiguracaoArquivo(unidadeDeTrabalho).CaminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    else
                        GerarArquivoPDFNaPasta(cte, unidadeDeTrabalho, caminhoPDF);

                    Relatorio svcRelatorio = new Relatorio(unidadeDeTrabalho);

                    if (svcRelatorio.GerarRelatorioPorProcesso(ObterConfiguracaoArquivo(unidadeDeTrabalho).CaminhoGeradorRelatorios, "DACTE" + " " + cte.Codigo.ToString() + " " + System.IO.Path.GetDirectoryName(caminhoPDF) + " " + System.IO.Path.GetFileName(caminhoPDF)))
                    {
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                return null;
            }
        }

        private void GerarArquivoPDFNaPasta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho, string caminhoPDF)
        {

            Repositorio.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao repConfiguracaoWebService = new Repositorio.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao configuracaoWebService = repConfiguracaoWebService.BuscarPorTipo(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            if (configuracaoWebService == null || cte.SistemaEmissor != TipoEmissorDocumento.Integrador)
                return;

            ServicoCTe.uCteServiceTSSoapClient servicoCTeSoapClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);
            ServicoCTe.RetornoCTE retornoCTe = servicoCTeSoapClient.ConsultaProtocoloCte(cte.CodigoCTeIntegrador);

            if (retornoCTe != null && !string.IsNullOrWhiteSpace(retornoCTe.PDFDacte))
            {
                byte[] decodedBytePDF = System.Text.Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(retornoCTe.PDFDacte))).ToArray();

                if (!string.IsNullOrWhiteSpace(caminhoPDF))
                {
                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedBytePDF);
                }

            }
        }

        public void GerarPorProcessoAsync(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte != null)
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";

                    Task task = new Task(() =>
                    {
                        try
                        {
                            Relatorio svcRelatorio = new Relatorio(unidadeDeTrabalho);

                            svcRelatorio.GerarRelatorioPorProcesso(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios, "DACTE" + " " + codigoCTe + " " + System.IO.Path.GetDirectoryName(caminhoPDF) + " " + System.IO.Path.GetFileName(caminhoPDF));
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao gerar relatório DACTE: {ex.ToString()}", "CatchNoAction");
                        }
                    });

                    task.Start();
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
        }

        public byte[] Gerar(int codigoCTe, Dominio.ObjetosDeValor.Relatorios.DACTE dacte, List<Dominio.Entidades.DocumentosCTE> documentos, List<Dominio.Entidades.VeiculoCTE> veiculos, List<Dominio.Entidades.MotoristaCTE> motoristas)
        {
            if (dacte == null || (dacte.Status != "A" && dacte.Status != "C" && dacte.Status != "K"))
                return null;

            List<Dominio.ObjetosDeValor.Relatorios.DACTE> dactes = new List<Dominio.ObjetosDeValor.Relatorios.DACTE>();

            dactes.Add(dacte);

            List<ReportDataSource> dataSources = new List<ReportDataSource>();

            string reportPath = string.Empty;

            if (dacte.TipoImpressao == Dominio.Enumeradores.TipoImpressao.Retrato)
            {
                reportPath = "Relatorios\\DACTERetrato.rdlc";

                ReportDataSource dataSourceDocumentosFiscais1 = new ReportDataSource();
                dataSourceDocumentosFiscais1.Name = "DocumentosFiscais1";
                dataSourceDocumentosFiscais1.Value = (from obj in documentos select obj).Take(12).ToList();
                dataSources.Add(dataSourceDocumentosFiscais1);

                ReportDataSource dataSourceDocumentosFiscais2 = new ReportDataSource();
                dataSourceDocumentosFiscais2.Name = "DocumentosFiscais2";
                dataSourceDocumentosFiscais2.Value = (from obj in documentos select obj).Skip(12).Take(12).ToList();
                dataSources.Add(dataSourceDocumentosFiscais2);

                ReportDataSource dataSourceDocumentosFiscais3 = new ReportDataSource();
                dataSourceDocumentosFiscais3.Name = "DocumentosFiscais3";
                dataSourceDocumentosFiscais3.Value = (from obj in documentos select obj).Skip(24).Take(70).ToList();
                dataSources.Add(dataSourceDocumentosFiscais3);

                ReportDataSource dataSourceDocumentosFiscais4 = new ReportDataSource();
                dataSourceDocumentosFiscais4.Name = "DocumentosFiscais4";
                dataSourceDocumentosFiscais4.Value = (from obj in documentos select obj).Skip(94).Take(70).ToList();
                dataSources.Add(dataSourceDocumentosFiscais4);
            }
            else
            {
                reportPath = "Relatorios\\DACTEPaisagem.rdlc";

                ReportDataSource dataSourceDocumentosFiscais1 = new ReportDataSource();
                dataSourceDocumentosFiscais1.Name = "DocumentosFiscais1";
                dataSourceDocumentosFiscais1.Value = (from obj in documentos select obj).Take(7).ToList();
                dataSources.Add(dataSourceDocumentosFiscais1);

                ReportDataSource dataSourceDocumentosFiscais2 = new ReportDataSource();
                dataSourceDocumentosFiscais2.Name = "DocumentosFiscais2";
                dataSourceDocumentosFiscais2.Value = (from obj in documentos select obj).Skip(7).Take(7).ToList();
                dataSources.Add(dataSourceDocumentosFiscais2);

                ReportDataSource dataSourceDocumentosFiscais3 = new ReportDataSource();
                dataSourceDocumentosFiscais3.Name = "DocumentosFiscais3";
                dataSourceDocumentosFiscais3.Value = (from obj in documentos select obj).Skip(14).Take(60).ToList();
                dataSources.Add(dataSourceDocumentosFiscais3);

                ReportDataSource dataSourceDocumentosFiscais4 = new ReportDataSource();
                dataSourceDocumentosFiscais4.Name = "DocumentosFiscais4";
                dataSourceDocumentosFiscais4.Value = (from obj in documentos select obj).Skip(74).Take(60).ToList();
                dataSources.Add(dataSourceDocumentosFiscais4);
            }

            if (dacte.Modelo != "57")
                reportPath = "Relatorios\\OutrosRetrato.rdlc";

            ReportDataSource dataSourceDACTE = new ReportDataSource();
            dataSourceDACTE.Name = "DACTE";
            dataSourceDACTE.Value = dactes;
            dataSources.Add(dataSourceDACTE);

            ReportDataSource dataSourceVeiculos = new ReportDataSource();
            dataSourceVeiculos.Name = "Veiculos";
            dataSourceVeiculos.Value = (from obj in veiculos select obj).Take(4).ToList();
            dataSources.Add(dataSourceVeiculos);

            Relatorio svcRelatorio = new Relatorio();

            Dominio.ObjetosDeValor.Relatorios.Relatorio relatorio = svcRelatorio.GerarDesktop(reportPath, "PDF", null, dataSources);

            svcRelatorio = null;
            dacte = null;
            dactes = null;

            return relatorio.Arquivo;
        }

        public byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            return Utilidades.File.MergeFiles(sourceFiles);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ObterConfiguracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoArquivo == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                _configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
                return _configuracaoArquivo;
            }

            return _configuracaoArquivo;
        }

        #endregion
    }
}

