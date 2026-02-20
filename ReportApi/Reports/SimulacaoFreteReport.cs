using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.SimulacaoFrete)]
public class SimulacaoFreteReport : ReportBase
{
    public SimulacaoFreteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacao repAjusteTabelaFreteSimulacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacao(_unitOfWork);

        int codigoSimulacao = extraData.GetValue<int>("CodigoSimulacao");

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem repItemSimulacao =
            new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem(_unitOfWork);

        Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao simulacao = repAjusteTabelaFreteSimulacao.BuscarPorCodigo(codigoSimulacao);

        List<Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFrete> simulacoes =
            repItemSimulacao.ConsultarRelatorioSimulacaoFrete(codigoSimulacao);
        List<Dominio.Relatorios.Embarcador.DataSource.Fretes.SimulacaoFreteComponente> componentes =
            repItemSimulacao.ConsultarComponenteRelatorioSimulacaoFreteItem(codigoSimulacao);
        componentes.AddRange(repItemSimulacao.ConsultarComponenteRelatorioSimulacaoFreteCarga(codigoSimulacao));

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet();
        dataSet.DataSet = simulacoes;
        dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "SimulacaoFreteComponente.rpt",
                DataSet = componentes
            },
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "SimulacaoFreteComponente.rpt - 01",
                DataSet = componentes
            }
        };
        dataSet.Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroAjuste",
                simulacao.Ajuste.Numero.ToString(), true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacaoAjuste",
                simulacao.Ajuste.DataCriacao.ToString("dd/MM/yyyy"), true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TabelaFrete",
                simulacao.Ajuste.TabelaFrete.Descricao, true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo",
                simulacao.DataInicial.ToString("dd/MM/yyyy") + " à " + simulacao.DataFinal.ToString("dd/MM/yyyy"), true)
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Fretes\SimulacaoFrete.rpt", TipoArquivoRelatorio.PDF, dataSet, true);

        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Fretes/AjusteTabelaFrete", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}