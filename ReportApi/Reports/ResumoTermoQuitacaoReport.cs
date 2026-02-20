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

namespace ReportApi.Reports;

[UseReportType(ReportType.ResumoTermoQuitacao)]
public class ResumoTermoQuitacaoReport : ReportBase
{
    public ResumoTermoQuitacaoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Financeiro.TermoQuitacao repTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);

        var termo = repTermoQuitacao.BuscarPorCodigo(extraData.GetValue<int>("CodigoTermo"));

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = ObterDataSetResumo(termo);

        var pdfDocument = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Financeiros\ResumoTermoQuitacao.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.XLS, dataSet);

        return PrepareReportResult(FileType.PDF, pdfDocument);
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetResumo(
        Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo)
    {
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoTermoQuitacao>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoTermoQuitacao()
                    {
                        NumeroTermo = termo?.Codigo ?? 0,
                        DataInicial = termo?.DataInicial?.ToDateString() ?? string.Empty,
                        DataFinal = termo?.DataFinal?.ToDateString() ?? string.Empty,
                        NomeTransportador = termo?.Transportador.Descricao,
                        CredEmConta = termo?.CreditoEmConta ?? 0m,
                        NotasCompContraAdiant = termo?.NotasCompensadasAdiantamentos ?? 0m,
                        PagEDescViaConfirming = termo?.PagamentosEDescontosViaConfiming ?? 0m,
                        PagEDescViaCredEmConta = termo?.PagamentosEDescontosViaCreditoEmConta ?? 0m,
                        SaldoDeAdiantEmAberto = termo?.TotalSaldoEmAberto ?? 0m,
                        TotalDeAdiant = termo?.TotalAdiantamento ?? 0m,
                        TotalGeralDosPag = termo?.TotalGeralPagamento ?? 0m
                    }
                },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ResumoTermoQuitacaoFiliais.rpt",
                        DataSet = ObterDataSetsResumoFiliais(termo)
                    }
                }
            };

        return dataSet;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoTermoQuitacaoFiliais>
        ObterDataSetsResumoFiliais(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoTermoQuitacaoFiliais> dataSetFiliais =
            new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoTermoQuitacaoFiliais>();

        if (termo?.Transportador.Filiais != null && termo?.Transportador.Filiais.Count > 0)
        {
            foreach (var filial in termo.Transportador.Filiais)
            {
                dataSetFiliais.Add(new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoTermoQuitacaoFiliais
                {
                    CodigoIntegracao = filial?.CodigoIntegracao ?? string.Empty,
                    CNPJ = filial.CNPJ_Formatado,
                    Cidade = filial.Localidade.Descricao ?? string.Empty,
                    UF = filial.LocalidadeUF ?? string.Empty
                });
            }
        }

        return dataSetFiliais;
    }
}