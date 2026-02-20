using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ImpressaoProdutos)]
public class ImpressaoProdutosReport : ReportBase
{
    public ImpressaoProdutosReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        // Instancia repositorios
        Repositorio.Embarcador.WMS.Separacao repSeparacaoMercadorias =
            new Repositorio.Embarcador.WMS.Separacao(_unitOfWork);

        // Parametros
        int codigo = extraData.GetValue<int>("codigo");

        // Busca informacoes
        Dominio.Entidades.Embarcador.WMS.Separacao separacao = repSeparacaoMercadorias.BuscarPorCodigo(codigo);

        // Valida
        if (separacao == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        List<Dominio.Relatorios.Embarcador.DataSource.WMS.ImpressaoProdutos> dsProdutosSerparacao =
            (from o in separacao.Produtos
                select new Dominio.Relatorios.Embarcador.DataSource.WMS.ImpressaoProdutos()
                {
                    Produto = o.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao,
                    CodigoBarras = o.ProdutoEmbarcadorLote.CodigoBarras,
                    DepositoPosicao = o.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao,
                    QuantidadeSeparar = o.Quantidade,
                    QuantidadeSeparada = o.QuantidadeSeparada
                }).ToList();


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsProdutosSerparacao
            };

        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "crystal.png");

        // Gera pdf
        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\WMS\ImpressaoProdutos.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}