using System.Collections.Generic;

namespace ReportApi.ReportService;

public class BemReportService
{
    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioTermoResponsabilidade(Repositorio.UnitOfWork unidadeTrabalho, List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade> dadosBem, string caminhoLogo, out string mensagemErro)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosBem,
            Parameters = parametros
        };

        mensagemErro = null;

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Patrimonio\TermoResponsabilidade.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioTermoRecolhimentoMaterial(Repositorio.UnitOfWork unidadeTrabalho, List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoRecolhimentoMaterial> dadosBem, string caminhoLogo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosBem,
            Parameters = parametros
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Patrimonio\TermoRecolhimentoMaterial.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioTermoBaixaMaterial(Repositorio.UnitOfWork unidadeTrabalho, List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial> dadosBem, string caminhoLogo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosBem,
            Parameters = parametros
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Patrimonio\TermoBaixaMaterial.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }
}