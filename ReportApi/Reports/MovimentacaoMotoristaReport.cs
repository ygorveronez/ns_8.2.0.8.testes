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

[UseReportType(ReportType.MovimentacaoMotorista)]
public class MovimentacaoMotoristaReport : ReportBase
{
    public MovimentacaoMotoristaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var relatorioMovimentosMotorista = extraData.GetValue<string>("RelatorioMovimentosMotorista")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroOperador =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroOperador.NomeParametro = "Operador";
        parametroOperador.ValorParametro = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario")).Nome;
        parametros.Add(parametroOperador);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = relatorioMovimentosMotorista,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Financeiros\MovimentoMotorista.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
            "Financeiros/GeracaoMovimentoLote", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}