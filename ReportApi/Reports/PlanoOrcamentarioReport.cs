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

[UseReportType(ReportType.PlanoOrcamentario)]
public class PlanoOrcamentarioReport : ReportBase
{
    public PlanoOrcamentarioReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dataInicial = extraData.GetValue<DateTime>("DataInicial");
        var dataFinal = extraData.GetValue<DateTime>("DataFinal");
        var codigoCentroResultado = extraData.GetValue<int>("CodigoCentroResultado");
        var cnpjEmpresa = extraData.GetValue<string>("CnpjEmpresa");
        var nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosPlanoOrcamentario = extraData.GetValue<string>("DadosPlanoOrcamentario")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioPlanoOrcamentario(dataInicial, dataFinal, codigoCentroResultado, cnpjEmpresa, nomeEmpresa, dadosPlanoOrcamentario);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Financeiros/PlanoOrcamentario", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioPlanoOrcamentario(DateTime dataInicial,
        DateTime dataFinal, int codigoCentroResultado, string cnpjEmpresa, string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario> dadosPlanoOrcamentario)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario =
            new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(_unitOfWork);
        Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado =
            new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametro =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "Empresa";
        parametro.ValorParametro = nomeEmpresa;
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "CNPJEmpresa";
        parametro.ValorParametro = cnpjEmpresa;
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "DataInicial";
        parametro.ValorParametro = dataInicial.ToString("dd/MM/yyyy");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "DataFinal";
        parametro.ValorParametro = dataFinal.ToString("dd/MM/yyyy");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "CentroResultado";
        if (codigoCentroResultado > 0)
            parametro.ValorParametro = repCentroResultado.BuscarPorCodigo(codigoCentroResultado).BuscarDescricao;
        else
            parametro.ValorParametro = "Todos";
        parametros.Add(parametro);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosPlanoOrcamentario,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\PlanoOrcamentario.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}