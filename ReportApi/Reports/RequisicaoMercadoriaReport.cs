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

[UseReportType(ReportType.RequisicaoMercadoria)]
public class RequisicaoMercadoriaReport : ReportBase
{
    public RequisicaoMercadoriaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoRequisicaoMercadoria = extraData.GetValue<int>("CodigoRequisicaoMercadoria");
        var nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var dadosRequisicaoMercadoria = extraData.GetValue<string>("DadosRequisicaoMercadoria")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria>>();

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioRequisicaoMercadoria(codigoRequisicaoMercadoria, nomeEmpresa, dadosRequisicaoMercadoria);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Compras/RequisicaoMercadoria", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioRequisicaoMercadoria(
        int codigoRequisicaoMercadoria, string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria> dadosRequisicaoMercadoria)
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
                DataSet = dadosRequisicaoMercadoria,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Compras\RequisicaoMercadoria.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}