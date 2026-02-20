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

namespace ReportApi.Reports;

[UseReportType(ReportType.EtiquetaControleTacografo)]
public class EtiquetaControleTacografoReport : ReportBase
{
    public EtiquetaControleTacografoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dadosEtiqueta = extraData.GetValue<string>("DadosEtiqueta").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Frota.EtiquetaControleTacografo>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosEtiqueta,
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Frota\EtiquetaControleTacografo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, false);
        
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Frota/ControleTacografo", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}