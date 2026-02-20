using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;

namespace ReportApi.Reports;

[UseReportType(ReportType.PdfCRT)]
public class PdfCRTReport : ReportBase
{
    public PdfCRTReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dataSourceConhecimentoTransporteInternacionalRodovia = extraData.GetValue<string>("DataSourceCrt");
        Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia dataSourceCrt =
            Newtonsoft.Json.JsonConvert
                .DeserializeObject<
                    Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia>(
                    dataSourceConhecimentoTransporteInternacionalRodovia); //TODO: Validar serializacao


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet =
                    new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.
                        ConhecimentoTransporteInternacionalRodovia>() { dataSourceCrt },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\ConhecimentoTransporteInternacionalRodovia.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: false);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}