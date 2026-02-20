using System;
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
using Zen.Barcode;

namespace ReportApi.Reports;

[UseReportType(ReportType.ProdutoEtiquetaCodigoBarra)]
public class ProdutoEtiquetaCodigoBarrasReport : ReportBase
{
    public ProdutoEtiquetaCodigoBarrasReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        BarcodeMetrics1d metricas = new BarcodeMetrics1d();
        var produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(extraData.GetValue<string>("Produtos"));
        metricas.Scale = 5;

        List<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEtiquetaCodigoBarras> dataSourceProdutosEtiquetasCodigoBarras = (
            from produto in produtos
            select new Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEtiquetaCodigoBarras()
            {
                Codigo = produto?.CodigoProduto ?? string.Empty,
                Descricao = produto?.Descricao ?? string.Empty,
                CodigoBarrasExtenso = ConverterCodigoBarras(produto?.CodigoProduto.ToString()),
                CodigoBarrasImagem = Utilidades.Barcode.Gerar((produto?.CodigoProduto.ToString()), ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png),
                DirArquivoFotoProduto = produto?.Codigo != null ? ObterCaminhoFotoProduto(Convert.ToInt32(produto.Codigo)) : string.Empty
            }).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dataSourceProdutosEtiquetasCodigoBarras,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Produtos\ProdutoEtiquetaCodigoBarras.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private string ConverterCodigoBarras(string codigoProduto)
    {
        if (string.IsNullOrWhiteSpace(codigoProduto) || !ChecarSeSomenteNumero(codigoProduto))
            return codigoProduto;

        string codProdutoOriginal = codigoProduto;

        int CodigoBarrasEAN13 = 13 - codigoProduto.Length;
        codigoProduto = "99" + codigoProduto.PadLeft(CodigoBarrasEAN13, '0');

        if (codigoProduto.Length != 12)
            return codProdutoOriginal;

        int digitoVerificador = CalcularDigitoVerificarCodigoBarras(codigoProduto);

        return codigoProduto + digitoVerificador.ToString();
    }

    private int CalcularDigitoVerificarCodigoBarras(string codigoBarras)
    {
        int[] array = new int[]
        {
            int.Parse(codigoBarras[0].ToString()),
            int.Parse(codigoBarras[1].ToString()) * 3,
            int.Parse(codigoBarras[2].ToString()),
            int.Parse(codigoBarras[3].ToString()) * 3,
            int.Parse(codigoBarras[4].ToString()),
            int.Parse(codigoBarras[5].ToString()) * 3,
            int.Parse(codigoBarras[6].ToString()),
            int.Parse(codigoBarras[7].ToString()) * 3,
            int.Parse(codigoBarras[8].ToString()),
            int.Parse(codigoBarras[9].ToString())* 3,
            int.Parse(codigoBarras[10].ToString()),
            int.Parse(codigoBarras[11].ToString()) * 3
        };

        int sum = (array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6] + array[7] + array[8] + array[9] + array[10] + array[11]);
        int checkDigit = (((sum / 10) + 1) * 10) - sum;

        if (checkDigit % 10 == 0) checkDigit = 0;

        return checkDigit;
    }

    private bool ChecarSeSomenteNumero(string codigoProduto)
    {
        int n;
        bool isNumeric = int.TryParse(codigoProduto, out n);
        return isNumeric;
    }

    private string ObterCaminhoFotoProduto(int codigoProduto)
    {
        Repositorio.ProdutoFoto repFotoProduto = new Repositorio.ProdutoFoto(_unitOfWork);
        Dominio.Entidades.ProdutoFoto fotoProduto = repFotoProduto.BuscarPorProduto(codigoProduto);
        if (fotoProduto != null)
            return fotoProduto.CaminhoArquivo;

        return string.Empty;
    }
}