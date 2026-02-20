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

[UseReportType(ReportType.FluxoHorario)]
public class FluxoHorarioReport : ReportBase
{
    public FluxoHorarioReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var relatorioTemp = extraData.GetValue<string>("relatorioTemp").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> listaReport = extraData.GetValue<string>("listaReport").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz>>();
        string CaminhoLogoDacte = extraData.GetValue<string>("CaminhoLogoDacte");
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = extraData.GetValue<string>("parametros").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>>();
        string caminho = extraData.GetValue<string>("caminho");

        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(_unitOfWork);
        Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT()
            {
                ContadorRegistrosGrupo = null,
                ContadorRegistrosTotal = null,
                GroupFooterSection = null
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaReport, _unitOfWork,
                identificacaoCamposRPT, null, true, extraData.GetInfo().TipoServico, CaminhoLogoDacte);

        report.DataDefinition.FormulaFields["QuantidadeBaixa"].Text =
            configuracaoGestaoPatio.RelatorioFluxoHorarioQuantidadeBaixa.ToString();
        report.DataDefinition.FormulaFields["QuantidadeNormal"].Text =
            configuracaoGestaoPatio.RelatorioFluxoHorarioQuantidadeNormal.ToString();
        report.DataDefinition.FormulaFields["QuantidadeAlta"].Text =
            configuracaoGestaoPatio.RelatorioFluxoHorarioQuantidadeAlta.ToString();

        _servicoRelatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, caminho, _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}