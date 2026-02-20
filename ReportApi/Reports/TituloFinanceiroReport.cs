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

[UseReportType(ReportType.TituloFinanceiro)]
public class TituloFinanceiroReport : ReportBase
{
    public TituloFinanceiroReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var rptTitulo = extraData.GetValue<string>("RptTitulo");
        var dadosTitulo = extraData.GetValue<string>("DadosTitulo")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Dominio.Entidades.Empresa empresaRelatorio = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarRelatorioAutorizacaoPagamento(nomeEmpresa, dadosTitulo, empresaRelatorio?.CaminhoLogoDacte, rptTitulo);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Financeiros/TituloFinanceiro", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public static CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioAutorizacaoPagamento(
        string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo> dadosTitulo,
        string caminhoLogo, string rptTitulo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosTitulo,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\" + rptTitulo, Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, true, caminhoLogo);
    }
}