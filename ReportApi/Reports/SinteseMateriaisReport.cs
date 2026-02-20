using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.SinteseMateriais)]
public class SinteseMateriaisReport : ReportBase
{
    public SinteseMateriaisReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.GestaoPatio.FimCarregamento repFimCarrgemanto = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);

        var fimCarregamento = repFimCarrgemanto.BuscarPorCodigo(extraData.GetValue<int>("CodigoFimCarregamento"));
        Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto =
            new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto =
            new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaProdutos =
            repositorioCargaPedidoProduto.BuscarPorCargaParaImpressaoSintese(fimCarregamento.Carga.Codigo);
        List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FimCarregamentoSinteseMateriais>
            dataSetSinteseMateriais =
                new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FimCarregamentoSinteseMateriais>();

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto produto in listaProdutos)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto =
                repositorioPedidoProduto.BuscarPorPedidoProduto(produto.CargaPedido.Pedido.Codigo,
                    produto.Produto.Codigo);

            dataSetSinteseMateriais.Add(
                new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FimCarregamentoSinteseMateriais()
                {
                    Data = produto.CargaPedido.Pedido.DataTerminoCarregamento?.ToString("dd/MM/yyyy") ?? "",
                    Transporte = fimCarregamento.Carga.CodigoCargaEmbarcador,
                    Placa = fimCarregamento.Carga.Veiculo?.Placa ?? "",
                    Remessa = produto.CargaPedido.Pedido.NumeroPedidoEmbarcador ?? "",
                    Ordem = fimCarregamento.Carga.DadosSumarizados?.NumeroOrdem ?? "",
                    Cliente = produto.CargaPedido.Pedido.Destinatario?.NomeFantasia ?? "",
                    Material = produto.Produto?.Descricao ?? "",
                    Quantidade = pedidoProduto?.Quantidade.ToString("n2") ?? produto.Quantidade.ToString("n2"),
                    Observacao = pedidoProduto?.Observacao ?? produto.Produto?.Observacao ?? ""
                });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dataSetSinteseMateriais
            };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\GestaoPatio\SinteseMateriais.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}