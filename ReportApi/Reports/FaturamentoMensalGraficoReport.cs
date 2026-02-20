using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
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

[UseReportType(ReportType.FaturamentoMensalGrafico)]
public class FaturamentoMensalGraficoReport : ReportBase
{
    public FaturamentoMensalGraficoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoEmpresa = extraData.GetValue<int>("CodigoEmpresa");
        var tipoData = extraData.GetValue<string>("TipoData").ToEnum<TipoDataFaturamentoMensal>();
        var dataInicial = extraData.GetValue<DateTime>("DataInicial");
        var dataFinal = extraData.GetValue<DateTime>("DataFinal");
        var nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosFaturamentoMensal = extraData.GetValue<string>("DadosFaturamentoMensal")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FaturamentoMensal>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioFaturamentoMensalGrafico(codigoEmpresa, tipoData, dataInicial, dataFinal, nomeEmpresa,dadosFaturamentoMensal);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,"Relatorios/Financeiros/FaturamentoMensal", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioFaturamentoMensalGrafico(
        int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataFaturamentoMensal tipoData,
        DateTime dataInicial, DateTime dataFinal, string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FaturamentoMensal> dadosFaturamentoMensal)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro tipo =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        tipo.NomeParametro = "TipoData";
        tipo.ValorParametro = tipoData.ObterDescricao();
        parametros.Add(tipo);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro dataInicio =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        dataInicio.NomeParametro = "DataInicial";
        if (dataInicial > DateTime.MinValue)
            dataInicio.ValorParametro = dataInicial.ToString();
        else
            dataInicio.ValorParametro = "Todas";
        parametros.Add(dataInicio);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro dataFim =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        dataFim.NomeParametro = "DataFinal";
        if (dataFinal > DateTime.MinValue)
            dataFim.ValorParametro = dataFinal.ToString();
        else
            dataFim.ValorParametro = "Todas";
        parametros.Add(dataFim);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosFaturamentoMensal,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\FaturamentoMensalGrafico.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}