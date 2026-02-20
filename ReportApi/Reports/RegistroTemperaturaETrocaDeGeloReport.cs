using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
namespace ReportApi.Reports;

[UseReportType(ReportType.RegistroTemperaturaETrocaDeGelo)]
public class RegistroTemperaturaETrocaDeGeloReport:ReportBase
{
    public RegistroTemperaturaETrocaDeGeloReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\CTe\RegistroTemperaturaETrocaDeGelo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF);  
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}