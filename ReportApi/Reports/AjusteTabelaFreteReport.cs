using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.AjusteTabelaFrete)]
public class AjusteTabelaFreteReport : ReportBase
{
    public AjusteTabelaFreteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var filtrosPesquisa = extraData.GetValue<string>("FiltrosPesquisa")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente>();
        var propriedades = extraData.GetValue<string>("Propriedades")
            .FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>>();
        var parametrosConsulta = extraData.GetValue<string>("ParametrosConsulta")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var relatorioTemporario = extraData.GetValue<string>("RelatorioTemporario").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        var parametros = extraData.GetValue<string>("Parametros").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>>();

        Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> listaConsultaTabelaFrete = repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta);

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemporario, listaConsultaTabelaFrete, _unitOfWork);

        _servicoRelatorioReportService.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao, relatorioTemporario, parametros);
        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Fretes/AjusteTabelaFrete", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}