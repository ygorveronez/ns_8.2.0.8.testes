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
[UseReportType(ReportType.PedidoVendaContrato)]
public class PedidoVendaContratoReport : ReportBase
{
    public PedidoVendaContratoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoPedidoVenda = extraData.GetValue<int>("codigoPedidoVenda");
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");
        List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato> dadosPedidoVenda = extraData.GetValue<string>("dadosPedidoVenda").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato>>();
        
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        string CaminhoLogoDacte = extraData.GetValue<string>("CaminhoLogoDacte");

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarRelatorioPedidoVendaContrato(codigoPedidoVenda, nomeEmpresa, dadosPedidoVenda, CaminhoLogoDacte);
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "PedidosVendas/PedidoVenda", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }
    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioPedidoVendaContrato(int codigoPedidoVenda, string nomeEmpresa, List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato> dadosPedidoVenda, string caminhoLogo)
    {
        Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(_unitOfWork);
        Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigoPedidoVenda);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosPedidoVenda,
            Parameters = parametros
        };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\PedidoVenda\PedidoVendaContrato.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }
}