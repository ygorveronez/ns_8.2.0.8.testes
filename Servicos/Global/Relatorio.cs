using Microsoft.Reporting.WebForms;
using Repositorio;
using System;
using System.Collections.Generic;

namespace Servicos
{
    public class Relatorio : ServicoBase
    {
        public Relatorio() : base() { }
        public Relatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool GerarRelatorioPorProcesso(string caminhoProcesso, string parametros)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = caminhoProcesso;
                p.StartInfo.Arguments = parametros;

                p.Start();
                p.WaitForExit();

                int exitCode = p.ExitCode;

                p.Dispose();
                p = null;

                if (exitCode <= 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                return false;
            }
        }

        public Dominio.ObjetosDeValor.Relatorios.Relatorio GerarWeb(string path, string format, List<Microsoft.Reporting.WebForms.ReportParameter> parameters, List<Microsoft.Reporting.WebForms.ReportDataSource> dataSources, Microsoft.Reporting.WebForms.SubreportProcessingEventHandler subreportHandler = null)
        {
            Microsoft.Reporting.WebForms.LocalReport reportViewer = new Microsoft.Reporting.WebForms.LocalReport();

            reportViewer.ReportPath = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), path);

            reportViewer.EnableExternalImages = true;

            if (parameters != null)
                reportViewer.SetParameters(parameters);

            foreach (Microsoft.Reporting.WebForms.ReportDataSource dataSource in dataSources)
                reportViewer.DataSources.Add(dataSource);

            if (subreportHandler != null)
                reportViewer.SubreportProcessing += subreportHandler;

            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = reportViewer.Render(format, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            try
            {
                reportViewer.Dispose();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao fazer dispose do ReportViewer: {ex.ToString()}", "CatchNoAction");
            }

            reportViewer = null;
            path = null;
            parameters = null;
            dataSources = null;
            subreportHandler = null;

            GC.Collect();

            return new Dominio.ObjetosDeValor.Relatorios.Relatorio(bytes, mimeType, filenameExtension);
        }

        public Dominio.ObjetosDeValor.Relatorios.Relatorio GerarDesktop(string path, string format, List<ReportParameter> parameters, List<ReportDataSource> dataSources, SubreportProcessingEventHandler subreportHandler = null)
        {
            var reportViewer = new LocalReport();

            reportViewer.ReportPath = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), path);

            if (parameters != null)
                reportViewer.SetParameters(parameters);

            foreach (ReportDataSource dataSource in dataSources)
                reportViewer.DataSources.Add(dataSource);

            if (subreportHandler != null)
                reportViewer.SubreportProcessing += subreportHandler;

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = reportViewer.Render(format, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            try
            {
                reportViewer.Dispose();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao fazer dispose do ReportViewer (segunda ocorrência): {ex.ToString()}", "CatchNoAction");
            }

            reportViewer = null;
            path = null;
            parameters = null;
            dataSources = null;
            subreportHandler = null;

            GC.Collect();

            return new Dominio.ObjetosDeValor.Relatorios.Relatorio(bytes, mimeType, filenameExtension);
        }
    }
}

