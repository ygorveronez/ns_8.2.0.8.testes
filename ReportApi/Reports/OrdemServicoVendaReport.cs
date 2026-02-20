using System.Collections.Generic;
using System.Linq;
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

[UseReportType(ReportType.OrdemServicoVenda)]
public class OrdemServicoVendaReport : ReportBase
{
    public OrdemServicoVendaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(_unitOfWork);

        int codigoPedidoVenda = extraData.GetValue<int>("codigoPedidoVenda");
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");
        IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoVenda> dadosPedidoVenda = repPedidoVenda.RelatorioOrdemServicoVenda(codigoPedidoVenda);

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        string CaminhoLogoDacte = extraData.GetValue<string>("CaminhoLogoDacte");

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioOrdemServicoVenda(codigoPedidoVenda, _unitOfWork, nomeEmpresa, dadosPedidoVenda, CaminhoLogoDacte);
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "PedidosVendas/OrdemServicoVenda", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioOrdemServicoVenda(int codigoPedidoVenda, Repositorio.UnitOfWork unidadeTrabalho, string nomeEmpresa, IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoVenda> dadosPedidoVenda, string caminhoLogo)
    {
        Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unidadeTrabalho);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();

        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        string dirArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Assinaturas", "PedidoVenda" });
        string guidAssinatura = dadosPedidoVenda.Select(o => o.GuidArquivoAssinatura).FirstOrDefault();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroCaminhoAssinatura = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroCaminhoAssinatura.NomeParametro = "CaminhoAssinatura";
        parametroCaminhoAssinatura.ValorParametro = !string.IsNullOrWhiteSpace(guidAssinatura) ? Utilidades.IO.FileStorageService.Storage.GetFiles(dirArquivo, $"{guidAssinatura}.jpg").FirstOrDefault() ?? string.Empty: string.Empty;
        parametros.Add(parametroCaminhoAssinatura);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosPedidoVenda,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\PedidoVenda\OrdemServicoVenda.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        
        return report;
    }
}