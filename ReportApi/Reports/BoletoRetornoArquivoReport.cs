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

[UseReportType(ReportType.BoletoRetornoArquivo)]
public class BoletoRetornoArquivoReport:ReportBase
{
    public BoletoRetornoArquivoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> listaRetornoBoleto = extraData.GetValue<string>("listaRetornoBoleto").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto>>();
        string CaminhoLogoDacte = extraData.GetValue<string>("CaminhoLogoDacte");

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioRetornoBoleto(listaRetornoBoleto, CaminhoLogoDacte);
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Financeiros/BoletoImportarRetorno", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }
    
    
    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioRetornoBoleto(List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> listaRetornoBoleto, string caminhoLogo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = listaRetornoBoleto,
            Parameters = parametros
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Financeiros\BoletoRetornoArquivo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }
}