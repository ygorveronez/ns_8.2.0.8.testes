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

[UseReportType(ReportType.RomaneioTotalizador)]
public class RomaneioTotalizadorReport : ReportBase
{
    public RomaneioTotalizadorReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioTotalizador> dadosRelatorioRomaneioTotalizador = repositorioCargaPedidoProduto.RelatorioRomanaeioTotalizador(codigoCarga);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosRelatorioRomaneioTotalizador
        };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio($@"Areas\Relatorios\Reports\Default\GestaoPatio\RomaneioTotalizador.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        if (pdfContent == null)
            throw new ServicoException(Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}