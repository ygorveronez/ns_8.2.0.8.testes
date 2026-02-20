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

[UseReportType(ReportType.ReciboFinanceiro)]
public class ReciboFinanceiroReport : ReportBase
{
    public ReciboFinanceiroReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var pagamentoMotorista = extraData.GetValue<bool>("PagamentoMotorista");
        var movimentoFinanceiro = extraData.GetValue<bool>("MovimentoFinanceiro");
        var baixaPagar = extraData.GetValue<bool>("BaixaPagar");
        var carga = extraData.GetValue<bool>("Carga");
        var infracao = extraData.GetValue<bool>("Infracao");

        var dadosRecibo = extraData.GetValue<string>("DadosRecibo")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
        Dominio.Entidades.Empresa empresaRelatorio = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));
        if (empresaRelatorio == null && !infracao)
            empresaRelatorio = repEmpresa.BuscarEmpresaPadraoEmissao();

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarRelatorioReciboFinanceiro(nomeEmpresa, dadosRecibo,
                empresaRelatorio?.CaminhoLogoDacte ?? string.Empty);

        GerarRegistroRelatorio(report, relatorioControleGeracao, pagamentoMotorista, movimentoFinanceiro, baixaPagar,
            carga, infracao);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioReciboFinanceiro(string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> dadosRecibo, string caminhoLogo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro()
            {
                NomeParametro = "Empresa",
                ValorParametro = nomeEmpresa
            };

        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro fornecedorCount =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro()
            {
                NomeParametro = "TotalFornecedores",
                ValorParametro = (dadosRecibo[0]?.FornecedoresCount > 0) ? $"{dadosRecibo[0].FornecedoresCount}" : ""
            };

        parametros.Add(fornecedorCount);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosRecibo,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\ReciboFinanceiro.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
    }

    public void GerarRegistroRelatorio(CrystalDecisions.CrystalReports.Engine.ReportDocument report,
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
        bool pagamentoMotorista, bool movimentoFinanceiro, bool baixaPagar, bool carga, bool infracao)
    {
        if (infracao)
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Frota/Infracao",
                _unitOfWork);
        else if (pagamentoMotorista)
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
                "PagamentosMotoristas/PagamentoMotoristaTMS", _unitOfWork);
        else if (movimentoFinanceiro)
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
                "Financeiros/MovimentoFinanceiro", _unitOfWork);
        else if (baixaPagar)
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
                "Financeiros/BaixaTituloPagar", _unitOfWork);
        else if (carga)
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Cargas/Carga", _unitOfWork);
        else
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
                "Financeiros/BaixaTituloReceber", _unitOfWork);
    }
}