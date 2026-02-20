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

[UseReportType(ReportType.CotacaoPedido)]
public class CotacaoPedidoReport : ReportBase
{
    public CotacaoPedidoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var info = extraData.GetInfo();

        var dadosCotacaoPedido = extraData.GetValue<string>("DadosCotacaoPedidoDs")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Dominio.Entidades.Empresa empresaRelatorio = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarRelatorioCotacaoPedido(dadosCotacaoPedido, empresaRelatorio.CaminhoLogoDacte);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Cotacoes/CotacaoPedido",
            _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioCotacaoPedido(
        List<Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido> dadosCotacaoPedido, string caminhoLogo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosCotacaoPedido,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Cotacoes\CotacaoPedido.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
    }
}