using System;
using System.Collections.Generic;
using System.Linq;
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

[UseReportType(ReportType.CaixaFuncionario)]
public class CaixaFuncionarioReport : ReportBase
{
    public CaixaFuncionarioReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario =
            new Repositorio.Embarcador.Usuarios.CaixaFuncionario(_unitOfWork);
        Repositorio.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro repCaixaFuncionarioMovimentoFinanceiro =
            new Repositorio.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro(_unitOfWork);
        Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa =
            new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(_unitOfWork);

        int codigo = extraData.GetValue<int>("CodigoCaixa");
        bool detalhado = extraData.GetValue<bool>("Detalhado");

        // Busca informacoes
        Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = repCaixaFuncionario.BuscarPorCodigo(codigo);
        List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro> movimentacoes =
            repCaixaFuncionarioMovimentoFinanceiro.BuscarPorCaixa(codigo);
        List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa> valoresCaixa =
            repCaixaFuncionarioValorCaixa.BuscarPorCaixa(codigo);

        // Valida
        if (caixa == null || caixa.SituacaoCaixa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto)
            throw new ServicoException("Caixa não localizado, ou não está com a sua situação Fechada.");
        if (movimentacoes == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario> dsCaixaFuncionario =
            (from o in movimentacoes
                select new Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario()
                {
                    Operador = o.CaixaFuncionario.Usuario.Nome,
                    PlanoConta = o.CaixaFuncionario.PlanoConta.BuscarDescricao,
                    DataAbertura = o.CaixaFuncionario.DataAbertura.Value,
                    SaldoInicial = o.CaixaFuncionario.SaldoInicial,
                    Codigo = o.CaixaFuncionario.Codigo,
                    CodigoMovimento = o.MovimentoFinanceiroDebitoCredito.MovimentoFinanceiro.Codigo,
                    DataMovimento = o.MovimentoFinanceiroDebitoCredito.DataMovimento,
                    NumeroDocumento = o.MovimentoFinanceiroDebitoCredito.MovimentoFinanceiro.Documento,
                    ObservacaoMovimento = o.MovimentoFinanceiroDebitoCredito.MovimentoFinanceiro.Observacao,
                    ValorMovimento = o.MovimentoFinanceiroDebitoCredito.Valor,
                    DebitoCreditoMovimento = (int)o.MovimentoFinanceiroDebitoCredito.DebitoCredito,
                    Entradas = o.CaixaFuncionario.TotalEntradas,
                    Saidas = o.CaixaFuncionario.TotalSaidas,
                    SaldoFinal = o.CaixaFuncionario.SaldoFinal,
                    ValoresNoCaixa = o.CaixaFuncionario.SaldoNoCaixa
                }).ToList();

        if (dsCaixaFuncionario.Count == 0)
        {
            dsCaixaFuncionario.Add(new Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario()
            {
                Operador = caixa.Usuario.Nome,
                PlanoConta = caixa.PlanoConta.BuscarDescricao,
                DataAbertura = caixa.DataAbertura.Value,
                SaldoInicial = caixa.SaldoInicial,
                Codigo = 0,
                CodigoMovimento = 0,
                DataMovimento = caixa.DataAbertura.Value,
                NumeroDocumento = "",
                ObservacaoMovimento = "",
                ValorMovimento = 0,
                DebitoCreditoMovimento = 0,
                Entradas = caixa.TotalEntradas,
                Saidas = caixa.TotalSaidas,
                SaldoFinal = caixa.SaldoFinal,
                ValoresNoCaixa = caixa.SaldoNoCaixa
            });
        }

        if (!detalhado)
        {
            Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario movimentoDiariaDebito =
                new Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario()
                {
                    Codigo = caixa.Codigo,
                    CodigoMovimento = -1,
                    Operador = caixa.Usuario.Nome,
                    PlanoConta = caixa.PlanoConta.BuscarDescricao,
                    DataAbertura = caixa.DataAbertura.Value,
                    SaldoInicial = caixa.SaldoInicial,
                    DataMovimento = DateTime.Now.Date,
                    NumeroDocumento = "",
                    ObservacaoMovimento = "",
                    ValorMovimento = 0,
                    DebitoCreditoMovimento = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito,
                    Entradas = caixa.TotalEntradas,
                    Saidas = caixa.TotalSaidas,
                    SaldoFinal = caixa.SaldoFinal,
                    ValoresNoCaixa = caixa.SaldoNoCaixa
                };
            Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario movimentoDiariaCredito =
                new Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario()
                {
                    Codigo = caixa.Codigo,
                    CodigoMovimento = -2,
                    Operador = caixa.Usuario.Nome,
                    PlanoConta = caixa.PlanoConta.BuscarDescricao,
                    DataAbertura = caixa.DataAbertura.Value,
                    SaldoInicial = caixa.SaldoInicial,
                    DataMovimento = DateTime.Now.Date,
                    NumeroDocumento = "",
                    ObservacaoMovimento = "",
                    ValorMovimento = 0,
                    DebitoCreditoMovimento = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito,
                    Entradas = caixa.TotalEntradas,
                    Saidas = caixa.TotalSaidas,
                    SaldoFinal = caixa.SaldoFinal,
                    ValoresNoCaixa = caixa.SaldoNoCaixa
                };
            List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario> listaRemover =
                new List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionario>();
            foreach (var movimento in dsCaixaFuncionario)
            {
                if ((movimento.ObservacaoMovimento.Contains("DIARIA") ||
                     movimento.ObservacaoMovimento.Contains("DIÁRIA")) && movimento.DebitoCreditoMovimento ==
                    (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                {
                    movimentoDiariaDebito.DataMovimento = movimento.DataMovimento;
                    movimentoDiariaDebito.ObservacaoMovimento += " " + movimento.ObservacaoMovimento;
                    movimentoDiariaDebito.NumeroDocumento += " " + movimento.NumeroDocumento;
                    movimentoDiariaDebito.ValorMovimento += movimento.ValorMovimento;

                    listaRemover.Add(movimento);
                }
                else if ((movimento.ObservacaoMovimento.Contains("DIARIA") ||
                          movimento.ObservacaoMovimento.Contains("DIÁRIA")) && movimento.DebitoCreditoMovimento ==
                         (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                {
                    movimentoDiariaCredito.DataMovimento = movimento.DataMovimento;
                    movimentoDiariaCredito.ObservacaoMovimento += " " + movimento.ObservacaoMovimento;
                    movimentoDiariaCredito.NumeroDocumento += " " + movimento.NumeroDocumento;
                    movimentoDiariaCredito.ValorMovimento += movimento.ValorMovimento;

                    listaRemover.Add(movimento);
                }
            }

            foreach (var remover in listaRemover)
                dsCaixaFuncionario.Remove(remover);

            if (movimentoDiariaDebito.ValorMovimento > 0)
                dsCaixaFuncionario.Add(movimentoDiariaDebito);
            if (movimentoDiariaCredito.ValorMovimento > 0)
                dsCaixaFuncionario.Add(movimentoDiariaCredito);
        }

        List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionarioValorCaixa>
            dsCaixaFuncionarioValorCaixa = (from o in valoresCaixa
                select new Dominio.Relatorios.Embarcador.DataSource.Usuarios.CaixaFuncionarioValorCaixa()
                {
                    Codigo = o.Codigo,
                    Descricao = o.Descricao,
                    Valor = o.Valor
                }).ToList();


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsCaixaFuncionario,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "CaixaFuncionario_ValorCaixa.rpt",
                        DataSet = dsCaixaFuncionarioValorCaixa
                    }
                }
            };

        // Gera pdf
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Usuarios\CaixaFuncionario.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}