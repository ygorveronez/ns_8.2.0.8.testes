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

[UseReportType(ReportType.ExtratoPagamentoAgregado)]
public class ExtratoPagamentoAgregadoReport : ReportBase
{
    public ExtratoPagamentoAgregadoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado> extratoPagamentoAgregado = extraData.GetValue<string>("extratoPagamentoAgregado").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes> extratoPagamentoAgregadoFretes = extraData.GetValue<string>("extratoPagamentoAgregadoFretes").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos> extratoPagamentoAgregadoAcrescimos = extraData.GetValue<string>("extratoPagamentoAgregadoAcrescimos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos> extratoPagamentoAgregadoAdiantamentos = extraData.GetValue<string>("extratoPagamentoAgregadoAdiantamentos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos> extratoPagamentoAgregadoDescontos = extraData.GetValue<string>("extratoPagamentoAgregadoDescontos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos> extratoPagamentoAgregadoTributos = extraData.GetValue<string>("extratoPagamentoAgregadoTributos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos>>();
        List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos> extratoPagamentoAgregadoAbastecimentos = extraData.GetValue<string>("extratoPagamentoAgregadoAbastecimentos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioExtratoPagamentoAgregado(nomeEmpresa, extratoPagamentoAgregado,
            extratoPagamentoAgregadoFretes,
            extratoPagamentoAgregadoAcrescimos,
            extratoPagamentoAgregadoAdiantamentos,
            extratoPagamentoAgregadoDescontos,
            extratoPagamentoAgregadoTributos,
            extratoPagamentoAgregadoAbastecimentos);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "PagamentosAgregados/PagamentoAgregado", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }


    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioExtratoPagamentoAgregado(string nomeEmpresa,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregado> extratoPagamentoAgregado,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoFretes> extratoPagamentoAgregadoFretes,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAcrescimos> extratoPagamentoAgregadoAcrescimos,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAdiantamentos> extratoPagamentoAgregadoAdiantamentos,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoDescontos> extratoPagamentoAgregadoDescontos,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoTributos> extratoPagamentoAgregadoTributos,
      List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.ExtratoPagamentoAgregadoAbastecimentos> extratoPagamentoAgregadoAbastecimentos)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ExtratoPagamentoAgregadoAcrescimos",
            DataSet = extratoPagamentoAgregadoAcrescimos
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ExtratoPagamentoAgregadoFretes",
            DataSet = extratoPagamentoAgregadoFretes
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ExtratoPagamentoAgregadoAbastecimentos",
            DataSet = extratoPagamentoAgregadoAbastecimentos
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds4 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ExtratoPagamentoAgregadoDescontos",
            DataSet = extratoPagamentoAgregadoDescontos
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds5 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ExtratoPagamentoAgregadoAdiantamentos",
            DataSet = extratoPagamentoAgregadoAdiantamentos
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds6 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "ExtratoPagamentoAgregadoTributos",
            DataSet = extratoPagamentoAgregadoTributos
        };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);
        subReports.Add(ds4);
        subReports.Add(ds5);
        subReports.Add(ds6);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = extratoPagamentoAgregado,
            Parameters = parametros,
            SubReports = subReports
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\PagamentosAgregados\ExtratoPagamentoAgregado.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return report;
    }
}