using System;
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

[UseReportType(ReportType.DREGerencial)]
public class DREGerencialReport : ReportBase
{
    public DREGerencialReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var info = extraData.GetInfo();

        DateTime dataInicial = extraData.GetValue<DateTime>("DataInicial");
        DateTime dataFinal = extraData.GetValue<DateTime>("DataFinal");
        var dadosDREGerencial = extraData.GetValue<string>("DadosDREGerencial")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Dominio.Entidades.Empresa empresaRelatorio = null;
        if (info.TipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            empresaRelatorio = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioDREGerencial(dataInicial,
            dataFinal, dadosDREGerencial, empresaRelatorio?.CaminhoLogoDacte);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
            "Relatorios/Financeiros/DREGerencial",
            _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioDREGerencial(DateTime dataInicial,
        DateTime dataFinal, List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial> dadosDREGerencial,
        string caminhoLogo)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametro =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "DataInicial";
        parametro.ValorParametro = dataInicial.ToString("dd/MM/yyyy");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "DataFinal";
        parametro.ValorParametro = dataFinal.ToString("dd/MM/yyyy");
        parametros.Add(parametro);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosDREGerencial,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\DREGerencial.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
    }
}