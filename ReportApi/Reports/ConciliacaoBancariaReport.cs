using System;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ConciliacaoBancaria)]
public class ConciliacaoBancariaReport : ReportBase
{
    public ConciliacaoBancariaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("CodigoConciliacaoBancaria");
        
        Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria =
            new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(_unitOfWork);
        Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro =
            new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(_unitOfWork);
        Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario =
            new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(_unitOfWork);
        
        Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria =
            repConciliacaoBancaria.BuscarPorCodigo(codigo);
        IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> listaMovimentosConcolidados =
            repMovimentoFinanceiro.ConsultarMovimentoConcolidados(codigo);
        IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> listaExtratosConcolidados =
            repExtratoBancario.ConsultarExtratoConcolidados(codigo);

        // Valida
        if (conciliacaoBancaria == null)
            throw new ServicoException("Não foi possível encontrar o registro da conciliação bancária");

        List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConciliacaoBancaria> listaConciliacaoBancaria =
            new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConciliacaoBancaria>();
        listaConciliacaoBancaria.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ConciliacaoBancaria()
        {
            Codigo = conciliacaoBancaria.Codigo,
            Colaborador = conciliacaoBancaria.Colaborador.Descricao,
            DataFinal = conciliacaoBancaria.DataFinal.HasValue
                ? conciliacaoBancaria.DataFinal.Value.ToString("dd/MM/yyyy")
                : "",
            DataInicial = conciliacaoBancaria.DataInicial.HasValue
                ? conciliacaoBancaria.DataInicial.Value.ToString("dd/MM/yyyy")
                : "",
            DataGeracaoMovimento = conciliacaoBancaria.DataGeracaoMovimento > DateTime.MinValue
                ? conciliacaoBancaria.DataGeracaoMovimento.ToString("dd/MM/yyyy")
                : "",
            PlanoConta = conciliacaoBancaria.PlanoConta?.BuscarDescricao ??
                         conciliacaoBancaria.PlanoContaSintetico?.BuscarDescricao ?? "",
            RealizarConciliacaoAutomatica = conciliacaoBancaria.RealizarConciliacaoAutomatica ? "Sim" : "Não",
            SituacaoConciliacaoBancaria = conciliacaoBancaria.DescricaoSituacaoConciliacaoBancaria,
            ValorTotalCreditoExtrato = conciliacaoBancaria.ValorTotalCreditoExtrato,
            ValorTotalCreditoMovimento = conciliacaoBancaria.ValorTotalCreditoMovimento,
            ValorTotalDebitoExtrato = conciliacaoBancaria.ValorTotalDebitoExtrato,
            ValorTotalDebitoMovimento = conciliacaoBancaria.ValorTotalDebitoMovimento,
            ValorTotalExtrato = conciliacaoBancaria.ValorTotalExtrato,
            ValorTotalMovimento = conciliacaoBancaria.ValorTotalMovimento
        });

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = listaConciliacaoBancaria,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ConciliacaoBancaria_Extrato",
                        DataSet = listaExtratosConcolidados
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ConciliacaoBancaria_Movimento",
                        DataSet = listaMovimentosConcolidados
                    }
                }
            };

        // Gera pdf
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Financeiros\ConciliacaoBancaria.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}