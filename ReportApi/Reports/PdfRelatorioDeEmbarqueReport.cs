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

[UseReportType(ReportType.PdfRelatorioDeEmbarque)]
public class PdfRelatorioDeEmbarqueReport : ReportBase
{

    public PdfRelatorioDeEmbarqueReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCarga = extraData.GetValue<int>("CodigoCarga");
        
        Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelatorioEmbarque.RelatorioEmbarque> dadosRelatorioEmbarque = repositorioCargaPedidoXMLNotaFiscalCTe.RelatorioRelacaoEmbarque(codigoCarga);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosRelatorioEmbarque
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\RelatorioEmbarque.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o relatório de embarque.");

        return PrepareReportResult(FileType.PDF, pdf);
    }
}