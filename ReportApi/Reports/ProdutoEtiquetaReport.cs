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

[UseReportType(ReportType.ProdutoEtiqueta)]
public class ProdutoEtiquetaReport : ReportBase
{
    public ProdutoEtiquetaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(extraData.GetValue<string>("Produtos"));
        List<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEtiqueta> dataSourceProdutosEtiquetas = (
            from produto in produtos
            select new Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEtiqueta()
            {
                QRCode = Utilidades.QRcode.Gerar(produto.Codigo.ToString()),
                Descripcao = produto?.Descricao ?? string.Empty,
                Codigo = produto.Codigo,
                LocalArmazenamento = produto.LocalArmazenamentoProduto != null ? produto.LocalArmazenamentoProduto.Descricao : string.Empty
            }).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dataSourceProdutosEtiquetas,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Produtos\ProdutosEtiqueta.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}