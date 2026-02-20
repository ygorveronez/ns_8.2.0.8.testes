using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.AreaVeiculoPosicoesQrCode)]
public class AreaVeiculoPosicoesQrCodeReport:ReportBase
{
    public AreaVeiculoPosicoesQrCodeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = extraData.GetValue<List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode>>("dataSourceAreaVeiculoPosicaoQrCodeList"),
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Logistica\AreaVeiculoPosicoesQrCode.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        if (pdfContent == null)
            throw new ServicoException("Não foi possível gerar o QR Code.");
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}