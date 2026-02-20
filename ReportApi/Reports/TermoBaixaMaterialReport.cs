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

[UseReportType(ReportType.TermoBaixaMaterial)]
public class TermoBaixaMaterialReport : ReportBase
{
    private readonly BemReportService _bemReportService;
    public TermoBaixaMaterialReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage, BemReportService bemReportService) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
        _bemReportService = bemReportService;
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Dominio.Entidades.Empresa empresaRelatorio = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));

        var dadosBem = extraData.GetValue<string>("DadosBem").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = _bemReportService.GerarRelatorioTermoBaixaMaterial(_unitOfWork, dadosBem, empresaRelatorio.CaminhoLogoDacte);
        
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Patrimonio/BaixaBem", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}