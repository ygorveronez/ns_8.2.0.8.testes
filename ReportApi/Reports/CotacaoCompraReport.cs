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

[UseReportType(ReportType.CotacaoCompra)]
public class CotacaoCompraReport : ReportBase
{
    public CotacaoCompraReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("Codigo");
        string nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosCotacaoCompra = extraData.GetValue<string>("DadosCotacaoCompraDs").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioCotacaoCompraFornecedor(codigo, nomeEmpresa, dadosCotacaoCompra);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Compras/CotacaoCompra",_unitOfWork);

        return PrepareReportResult(FileType.PDF, relatorioControleGeracao.GuidArquivo);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioCotacaoCompraFornecedor(
        int codigo, string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor>
            dadosCotacaoCompraFornecedor)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosCotacaoCompraFornecedor,
                Parameters = parametros
            };
        
        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Compras\CotacaoCompraFornecedor.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return report;
    }
}