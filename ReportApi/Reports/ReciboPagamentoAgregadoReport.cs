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

[UseReportType(ReportType.ReciboPagamentoAgregado)]
public class ReciboPagamentoAgregadoReport : ReportBase
{
    public ReciboPagamentoAgregadoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {

    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado> dadosPagamentoAgregado = extraData.GetValue<string>("dadosPagamentoAgregado").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento> dadosPagamentoAgregadoAdiantamento = extraData.GetValue<string>("dadosPagamentoAgregadoAdiantamento").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo> dadosPagamentoAgregadoDescontoAcrescimo = extraData.GetValue<string>("dadosPagamentoAgregadoDescontoAcrescimo").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento> dadosPagamentoAgregadoDocumento = extraData.GetValue<string>("dadosPagamentoAgregadoDocumento").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento>>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioReciboPagamentoAgregado(nomeEmpresa, dadosPagamentoAgregado, dadosPagamentoAgregadoAdiantamento, dadosPagamentoAgregadoDescontoAcrescimo, dadosPagamentoAgregadoDocumento);
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "PagamentosAgregados/PagamentoAgregado", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioReciboPagamentoAgregado(string nomeEmpresa,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado> dadosPagamentoAgregado,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento> dadosPagamentoAgregadoAdiantamento,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo> dadosPagamentoAgregadoDescontoAcrescimo,
            List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento> dadosPagamentoAgregadoDocumento)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ReciboPagamentoAgregadoAdiantamento.rpt",
            DataSet = dadosPagamentoAgregadoAdiantamento
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ReciboPagamentoAgregadoAcrescimoDesconto.rpt",
            DataSet = dadosPagamentoAgregadoDescontoAcrescimo
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ReciboPagamentoAgregadoDocumento.rpt",
            DataSet = dadosPagamentoAgregadoDocumento
        };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosPagamentoAgregado,
            Parameters = parametros,
            SubReports = subReports
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\PagamentosAgregados\ReciboPagamentoAgregado.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return report;
    }
}