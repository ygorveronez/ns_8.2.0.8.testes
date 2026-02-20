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

[UseReportType(ReportType.FluxoCaixa)]
public class FluxoCaixaReport : ReportBase
{
    public FluxoCaixaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dataVencimentoInicial = extraData.GetValue<DateTime>("DataVencimentoInicial");
        var dataVencimentoFinal = extraData.GetValue<DateTime>("DataVencimentoFinal");
        var codigoTipoPagamentoRecebimento = extraData.GetValue<int>("CodigoTipoPagamentoRecebimento");
        var tituloPendente = extraData.GetValue<bool>("TituloPendente");
        var limiteConta = extraData.GetValue<bool>("LimiteConta");
        var provisaoPesquisaTitulo = extraData.GetValue<string>("ProvisaoPesquisaTitulo")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo>();
        var analiticoSintetico = extraData.GetValue<string>("AnaliticoSintetico")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico>();
        var cnpjEmpresa = extraData.GetValue<string>("CnpjEmpresa");
        var nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosFluxoCaixa = extraData.GetValue<string>("DadosFluxoCaixa")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa>>();
        var dadosFluxoCaixaConta = extraData.GetValue<string>("DadosFluxoCaixaConta")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta>>();
        var codigoEmpresa = extraData.GetValue<int>("CodigoEmpresa");
        var tipoAmbiente = extraData.GetValue<string>("TipoAmbiente").ToEnum<Dominio.Enumeradores.TipoAmbiente>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioFluxoCaixa(
            dataVencimentoInicial, dataVencimentoFinal, codigoTipoPagamentoRecebimento, tituloPendente,
            limiteConta, provisaoPesquisaTitulo, analiticoSintetico, cnpjEmpresa, nomeEmpresa, dadosFluxoCaixa,
            dadosFluxoCaixaConta, codigoEmpresa, tipoAmbiente);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
            "Relatorios/Financeiros/FluxoCaixa", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioFluxoCaixa(
        DateTime dataVencimentoInicial, DateTime dataVencimentoFinal,
        int codigoTipoPagamentoRecebimento, bool tituloPendente, bool limiteConta,
        Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo provisaoPesquisaTitulo,
        Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico analiticoSintetico, string cnpjEmpresa,
        string nomeEmpresa, List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa> dadosFluxoCaixa,
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta> dadosFluxoCaixaConta,
        int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento =
            new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(_unitOfWork);
        Repositorio.Embarcador.Financeiro.Titulo repTitulo =
            new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);

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
        parametro.NomeParametro = "DataVencimentoInicial";
        parametro.ValorParametro = dataVencimentoInicial.ToString("dd/MM/yyyy");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "DataVencimentoFinal";
        parametro.ValorParametro = dataVencimentoFinal.ToString("dd/MM/yyyy");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "TituloPendente";
        if (tituloPendente)
            parametro.ValorParametro = "Sim";
        else
            parametro.ValorParametro = "Não";
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "LimiteConta";
        if (limiteConta)
            parametro.ValorParametro = "Sim";
        else
            parametro.ValorParametro = "Não";
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "TipoConta";
        if (analiticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
            parametro.ValorParametro = "Sintético";
        else
            parametro.ValorParametro = "Analítico";
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "Provisao";
        parametro.ValorParametro = provisaoPesquisaTitulo.ToString();
        if (provisaoPesquisaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo.SemProvisao)
            parametro.ValorParametro = "Sem Provisão";
        else if (provisaoPesquisaTitulo ==
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo.SomenteProvisao)
            parametro.ValorParametro = "Somente Provisão";
        else
            parametro.ValorParametro = "Com Provisão";
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "TipoPagamentoRecebimento";
        if (codigoTipoPagamentoRecebimento > 0)
            parametro.ValorParametro =
                repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoPagamentoRecebimento).Descricao;
        else
            parametro.ValorParametro = "Todos";
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "AReceberEmAtraso";
        parametro.ValorParametro = repTitulo
            .TituloAReceberEmAtraso(codigoEmpresa, tipoAmbiente, dataVencimentoInicial, provisaoPesquisaTitulo)
            .ToString("n2");
        parametro.ValorParametro = parametro.ValorParametro.Replace(".", "");
        Servicos.Log.TratarErro(parametro.ValorParametro, "FluxoCaixa");
        parametros.Add(parametro);

        parametro = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametro.NomeParametro = "APagarEmAtraso";
        parametro.ValorParametro = repTitulo
            .TituloAPagarEmAtraso(codigoEmpresa, tipoAmbiente, dataVencimentoInicial, provisaoPesquisaTitulo)
            .ToString("n2");
        parametro.ValorParametro = parametro.ValorParametro.Replace(".", "");
        Servicos.Log.TratarErro(parametro.ValorParametro, "FluxoCaixa");
        parametros.Add(parametro);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosFluxoCaixa,
                Parameters = parametros
            };

        dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "FluxoCaixaConta",
                DataSet = dadosFluxoCaixaConta
            },
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "FluxoCaixa",
                DataSet = dadosFluxoCaixa
            },
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "FluxoCaixaData",
                DataSet = dadosFluxoCaixa
            }
        };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Financeiros\FluxoCaixa.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}