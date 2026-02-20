using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.HistoricoParada)]
public class HistoricoParadasReport : ReportBase
{
    public HistoricoParadasReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var dadosHistoricoParadas = extraData.GetValue<string>("DadosHistoricoParadas")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo>>();
        var filtrosPesquisa = extraData.GetValue<string>("FiltrosPesquisa")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioHistoricoParadas(dadosHistoricoParadas, filtrosPesquisa);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Logistica/HistoricoParadas", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioHistoricoParadas(
        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> dadosHistoricoParadas,
        Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa)
    {
        Repositorio.Embarcador.Filiais.Filial
            repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
        Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);
        Repositorio.Veiculo repositorioVeiculos = new Repositorio.Veiculo(_unitOfWork);

        Dominio.Entidades.Embarcador.Filiais.Filial filial =
            repositorioFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial);
        Dominio.Entidades.Empresa transportador =
            repositorioTransportador.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);
        List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculos.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculo);

        List<Parametro> parametros = new List<Parametro>();

        parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
        parametros.Add(new Parametro("Filial", filial?.Descricao));
        parametros.Add(new Parametro("Transportador", transportador?.NomeFantasia));
        parametros.Add(new Parametro("Veiculo", veiculos?.Select(o => o.Placa)));

        ReportDataSet dataSet = new ReportDataSet()
        {
            DataSet = dadosHistoricoParadas,
            Parameters = parametros
        };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Logistica\HistoricoParadas.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}