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

[UseReportType(ReportType.FaturaPagamentoAgregado)]
public class FaturaPagamentoAgregadoReport:ReportBase
{
    public FaturaPagamentoAgregadoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("codigo");
        byte[] pdfContent = ObterPdfFaturaPagamentoAgregado(codigo);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
    public byte[] ObterPdfFaturaPagamentoAgregado(int codigoPagamentoAgregado)
    {
        Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(_unitOfWork);

        IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregado> dadosPagamentoAgregado = repPagamentoAgregado.RelatorioFaturaPagamentoAgregado(codigoPagamentoAgregado);
        IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregadoDocumento> dadosPagamentoAgregadoDocumento = repPagamentoAgregado.RelatorioFaturaPagamentoAgregadoDocumento(codigoPagamentoAgregado);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "Documentos",
            DataSet = dadosPagamentoAgregadoDocumento
        };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosPagamentoAgregado,
            SubReports = subReports
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\PagamentosAgregados\FaturaPagamentoAgregado.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar a fatura do pagamento agregado.");

        return pdf;
    }

}