using System;
using System.Collections.Generic;

namespace ReportApi.ReportService;

public class RelatorioSemPadraoReportService
{
      #region Métodos Privados

        private static void SetarDataSetSubReport(CrystalDecisions.CrystalReports.Engine.ReportDocument report, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> dataSets)
        {
            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet in dataSets)
                report.Subreports[dataSet.Key].SetDataSource(dataSet.DataSet);
        }

        private static void SetarTarja(CrystalDecisions.CrystalReports.Engine.ReportDocument report, string mensagemTarja)
        {
            CrystalDecisions.CrystalReports.Engine.ReportObject tarja = null;

            try
            {
                tarja = report.ReportDefinition.ReportObjects["TarjaMensagem"];
            }
            catch
            {
            }

            if (tarja == null)
                return;

            if (!string.IsNullOrWhiteSpace(mensagemTarja))
            {
                tarja.ObjectFormat.EnableSuppress = false;
                var tarjaTextBox = tarja as CrystalDecisions.CrystalReports.Engine.TextObject;

                tarjaTextBox.Text = mensagemTarja;
                tarjaTextBox.ObjectFormat.HorizontalAlignment = CrystalDecisions.Shared.Alignment.HorizontalCenterAlign;
                tarjaTextBox.Left = 0;
                tarjaTextBox.Top = 0;
                tarjaTextBox.Width = report.PrintOptions.PageContentWidth;
                tarjaTextBox.ObjectFormat.EnableCanGrow = true;
                tarjaTextBox.ApplyFont(new System.Drawing.Font("Arial", mensagemTarja.Length > 10 ? 90f : 120f));
                tarjaTextBox.Color = System.Drawing.Color.Gainsboro;
            }
            else
            {
                tarja.ObjectFormat.EnableSuppress = true;
            }
        }

        #endregion

        #region Métodos Públicos

        public static byte[] GerarRelatorio(string caminhoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF, Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = null, bool possuiLogo = false, string caminhoLogo = null, string tarja = null)
        {
            CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarCrystalReport(caminhoRelatorio, tipoArquivo, dataSet, possuiLogo, caminhoLogo, tarja);
            return ObterBufferReport(report, tipoArquivo);
        }

        public static CrystalDecisions.CrystalReports.Engine.ReportDocument GerarCrystalReport(string caminhoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF, Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = null, bool possuiLogo = false, string caminhoLogo = null, string tarja = null)
        {
            CrystalDecisions.CrystalReports.Engine.ReportDocument report = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            string diretorioBase = System.Configuration.ConfigurationManager.AppSettings["CaminhoRelatoriosCrystal"].ToString();

#if DEBUG
            diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
#endif

            try
            {
                report.Load(Utilidades.IO.FileStorageService.Storage.Combine(diretorioBase, caminhoRelatorio));

                if (dataSet != null)
                {
                    if (dataSet.SubReports != null && dataSet.SubReports.Count > 0)
                        SetarDataSetSubReport(report, dataSet.SubReports);

                    report.SetDataSource(dataSet.DataSet);

                    if (dataSet.Parameters != null)
                    {
                        foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametro in dataSet.Parameters)
                        {
                            report.SetParameterValue(parametro.NomeParametro, parametro.ValorParametro);
                        }
                    }
                }

                if (possuiLogo)
                {
                    string caminhoPadrao = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"], "crystal.png");

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoLogo))
                    {
                        report.SetParameterValue("CaminhoImagem", caminhoLogo);
                    }
                    else if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPadrao))
                    {
                        report.SetParameterValue("CaminhoImagem", caminhoPadrao);
                    }
                    else
                    {
                        report.ReportDefinition.ReportObjects["Logo"].ObjectFormat.EnableSuppress = true;
                        report.SetParameterValue("CaminhoImagem", "");
                    }
                }

                SetarTarja(report, tarja);

                return report;
            }
            catch (Exception ex)
            {
                report.Dispose();
                throw;
            }
        }

        public static byte[] ObterBufferReport(CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo)
        {
            using (report)
            {

                System.IO.Stream streamRelatorio = null;

                switch (tipoArquivo)
                {
                    case Dominio.Enumeradores.TipoArquivoRelatorio.DOC:
                        streamRelatorio = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                        break;
                    case Dominio.Enumeradores.TipoArquivoRelatorio.RTF:
                        streamRelatorio = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.RichText);
                        break;
                    case Dominio.Enumeradores.TipoArquivoRelatorio.XLS:
                        streamRelatorio = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                        break;
                    case Dominio.Enumeradores.TipoArquivoRelatorio.PDF:
                    default:
                        streamRelatorio = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        break;
                }

                using (System.IO.MemoryStream msRelatorio = new System.IO.MemoryStream())
                {
                    streamRelatorio.CopyTo(msRelatorio);
                    streamRelatorio.Close();
                    return msRelatorio.ToArray();
                }
            }
        }

        #endregion
}