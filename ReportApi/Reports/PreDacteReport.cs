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

[UseReportType(ReportType.PreDacte)]
public class PreDacteReport : ReportBase
{
    public PreDacteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {

        int codigoCTe = extraData.GetValue<int>("CodigoCte");
        
        var dadosCTe = extraData.GetValue<string>("DadosCTe")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE>>();
        
        var dadosCarga = extraData.GetValue<string>("DadosCarga")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga>>();
        
        var dadosComponente = extraData.GetValue<string>("DadosComponente")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioPreDACTE(codigoCTe, dadosCTe, dadosCarga, dadosComponente);
        
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "CTe/RelatorioPreDACTE", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
    
    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioPreDACTE(int codigoCTe, List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE> dadosCTe, List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga> dadosCarga, List<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente> dadosComponente)
    {
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosCTe
        };
        dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "PreDACTECarga",
                DataSet = dadosCarga
            },
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "PreDACTEComponente",
                DataSet = dadosComponente
            }
        };

        return RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\CTe\PreDACTE.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}