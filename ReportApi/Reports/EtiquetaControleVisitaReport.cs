using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;
using Zen.Barcode;

namespace ReportApi.Reports;

[UseReportType(ReportType.EtiquetaControleVisita)]
public class EtiquetaControleVisitaReport : ReportBase
{
    public EtiquetaControleVisitaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        BarcodeMetrics1d metricas = new BarcodeMetrics1d();
        metricas.Scale = 5;

        var etiquetaControleVisita = extraData.GetValue<string>("EtiquetaControleVisitaDs")
            .FromJson<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.EtiquetaControleVisita>();

        byte[] codigoBarrasPNJ = Utilidades.Barcode.Gerar(etiquetaControleVisita.CPF, ZXing.BarcodeFormat.CODE_128,
            new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);
        etiquetaControleVisita.CodigoBarras = codigoBarrasPNJ;

        List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.EtiquetaControleVisita> dados =
            new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.EtiquetaControleVisita>();
        dados.Add(etiquetaControleVisita);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dados
            };
        
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\GestaoPatio\EtiquetaControleVisita.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}