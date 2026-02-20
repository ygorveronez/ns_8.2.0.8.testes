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

[UseReportType(ReportType.PdfDiarioBordo)]
public class PdfDiarioBordoReport : ReportBase
{
    public PdfDiarioBordoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dataSourceDiarioBordoString =
            extraData.GetValue<string>("DataSourceDiarioBordo"); 
        Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo dataSourceDiarioBordo =
            Newtonsoft.Json.JsonConvert
                .DeserializeObject<
                    Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo>(
                    dataSourceDiarioBordoString); //TODO: Validar serializacao        
        
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo>() { dataSourceDiarioBordo },
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\DiarioBordo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}