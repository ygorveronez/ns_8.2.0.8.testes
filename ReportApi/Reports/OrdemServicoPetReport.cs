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
[UseReportType(ReportType.OrdemServicoPet)]
public class OrdemServicoPetReport : ReportBase
{
    public OrdemServicoPetReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        
        int codigoOrdemServicoPet = extraData.GetValue<int>("codigoOrdemServicoPet");
        string nomeEmpresa =extraData.GetValue<string>("nomeEmpresa");
        List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet> dadosOrdemServicoPet = extraData.GetValue<string>("dadosOrdemServicoPet").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet>>();
        
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);
        
        string CaminhoLogoDacte =extraData.GetValue<string>("CaminhoLogoDacte");
           
        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioOrdemServicoPet(codigoOrdemServicoPet, nomeEmpresa, dadosOrdemServicoPet, CaminhoLogoDacte);

        // Substituir PedidoVenda pelo controller novo
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "PedidosVendas/OrdemServicoPet", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioOrdemServicoPet(int codigoOrdemServicoPet, string nomeEmpresa, List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet> dadosOrdemServicoPet, string caminhoLogo)
    {
        Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new(_unitOfWork);
        Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigoOrdemServicoPet);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosOrdemServicoPet,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\PedidoVenda\OrdemServicoPet.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }
}