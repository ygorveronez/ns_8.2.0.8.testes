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

[UseReportType(ReportType.ReciboInfracao)]
public class ReciboInfracaoReport : ReportBase
{
    public ReciboInfracaoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("CodigoRecibo");

        Repositorio.Embarcador.Configuracoes.ConfiguracaoInfracoes repConfiguraoInfracoes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoInfracoes(_unitOfWork);

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes configuraoInfracoes = repConfiguraoInfracoes.BuscarConfiguracaoPadrao();

        Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
        Repositorio.Embarcador.Frota.InfracaoParcela repositorioParcela =
            new Repositorio.Embarcador.Frota.InfracaoParcela(_unitOfWork);
        
        Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

        if (infracao == null)
            throw new ServicoException("Não foi possível encontrar a infração.");

        string configInfracaoTextoPadrao = string.Empty;
        if (configuraoInfracoes.FormulaInfracaoPadrao != null)
        {
            configInfracaoTextoPadrao = configuraoInfracoes.FormulaInfracaoPadrao
                                        .Replace("#MotoristaInfracao", $"{infracao.Motorista.Nome}")
                                        .Replace("#CpfInfracao", $"{infracao.Motorista.CPF}")
                                        .Replace("#CnhInfracao", $"{infracao.Motorista.NumeroHabilitacao}")
                                        .Replace("#DataEmissaoInfracao", $"{infracao.DataEmissaoInfracao}")
                                        .Replace("#DataInfracao", $"{infracao.DataLancamento}")
                                        .Replace("#NumeroInfracao", $"{infracao.Numero}")
                                        .Replace("#NumeroAutuacao", $"{infracao.NumeroAtuacao}")
                                        .Replace("#TipoInfracao", $"{infracao.TipoInfracao.Descricao}")
                                        .Replace("#LocalInfracao", $"{infracao.Local}")
                                        .Replace("#CidadeInfracao", $"{infracao.Cidade.Descricao}")
                                        .Replace("#VeiculoInfracao", $"{infracao.Veiculo.Descricao}")
                                        .Replace("#ObservacaoInfracao", $"{infracao.Observacao}");
        }


        List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> infracaoParcelas =
            repositorioParcela.BuscarPorInfracao(codigo);

        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiroCompleto> dadosRecibo =
            new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiroCompleto>();

        dadosRecibo.Add(new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiroCompleto()
            {
                Nome = infracao.Motorista.Nome,
                CPF = infracao.Motorista.CPF_Formatado,
                AIT = infracao.NumeroAtuacao,
                DataAutuacao = infracao.Data,
                CNH = infracao.Motorista.NumeroHabilitacao,
                Motivo = infracao.TipoInfracao?.Descricao ?? "",
                ConfigInfracaoTextoPadrao = configInfracaoTextoPadrao ?? ""
        });

        List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiroCompletoParcela> dadosSubReport =
            new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiroCompletoParcela>();

        foreach (Dominio.Entidades.Embarcador.Frota.InfracaoParcela parcela in infracaoParcelas)
        {
            dadosSubReport.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiroCompletoParcela()
                {
                    Motivo =
                        $"{(String.IsNullOrWhiteSpace(infracao.NumeroAtuacao) ? "" : infracao.NumeroAtuacao + " - ")}{infracao.Data.ToString("dd/MM/yyyy HH:mm")} - {(infracao.Veiculo != null ? infracao.Veiculo.Placa_Formatada : "")}",
                    Valor = parcela.Valor,
                    DataVencimento = parcela.DataVencimento.ToString("dd/MM/yyyy") ?? "",
                    Nome = infracao.Motorista?.Nome ?? ""
                }
            );
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "ReciboInfracaoParcela.rpt",
                DataSet = dadosSubReport
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosRecibo,
                Parameters = null,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> { ds1 }
            };

        byte[] arquivo = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Financeiros\ReciboInfracao.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        return PrepareReportResult(FileType.PDF, arquivo);
    }
}