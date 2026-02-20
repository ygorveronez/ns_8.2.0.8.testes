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
[UseReportType(ReportType.RomaneioDetalhadoResumido)]
public class RomaneioDetalhadoResumidoReport : ReportBase
{
    public RomaneioDetalhadoResumidoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("codigoCarga");

        Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioDetalhadoResumido> romaneioDetalhadoResumido = repositorioCargaPedidoProduto.RomaneioDetalhadoResumido(codigoCarga);
        IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhadoResumido> produtosRomaneioDetalhadoResumido = repositorioCargaPedidoProduto.ProdutosRomaneioDetalhadoResumido(codigoCarga);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "Produtos",
            DataSet = produtosRomaneioDetalhadoResumido
        };

        subReports.Add(ds1);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = romaneioDetalhadoResumido,
            SubReports = subReports,
        };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio($@"Areas\Relatorios\Reports\Default\GestaoPatio\RomaneioDetalhadoResumido.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        if (pdfContent == null)
            throw new ServicoException(Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}