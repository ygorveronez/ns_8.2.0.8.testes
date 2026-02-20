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

[UseReportType(ReportType.EmpresasFaturamento)]
public class EmpresasFaturamentoReport : ReportBase
{
    public EmpresasFaturamentoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var listaRelatorio = extraData.GetValue<string>("ListaRelatorio")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento>>();
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
                DataSet = listaRelatorio,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\FaturamentoMensal\EmpresasFaturamento.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao,
            "FaturamentoMensal/ValidacaoFaturamentoMensal", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}