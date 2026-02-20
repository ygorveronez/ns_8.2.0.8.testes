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

[UseReportType(ReportType.Francesinha)]
public class FrancesinhaReport : ReportBase
{
    public FrancesinhaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosFrancesinha = extraData.GetValue<string>("DadosFrancesinhaDs")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioFrancesinha(nomeEmpresa, dadosFrancesinha);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Financeiros/RelatorioFrancesinha", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioFrancesinha(string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha> dadosFrancesinha)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosFrancesinha,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\Francesinha.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}