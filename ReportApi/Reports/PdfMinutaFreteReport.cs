using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;
[UseReportType(ReportType.PdfMinutaFrete)]
public class PdfMinutaFreteReport : ReportBase
{
    public PdfMinutaFreteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dataSourceMinutaFreteBovinoString =
            extraData.GetValue<string>("DataSourceMinutaFreteBovino"); 
        Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.MinutaFreteBovino dataSourceMinutaFreteBovino =
            Newtonsoft.Json.JsonConvert
                .DeserializeObject<
                    Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.MinutaFreteBovino>(
                    dataSourceMinutaFreteBovinoString); //TODO: Validar serializacao   
        
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.MinutaFreteBovino>() { dataSourceMinutaFreteBovino },
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\DiarioBordoMinutaFreteBovino.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);
        
        return PrepareReportResult(FileType.PDF, pdf);
    }
}