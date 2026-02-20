using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.OrdemColeta)]
public class OrdemColeta : ReportBase
{
    public OrdemColeta(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        var cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCodigo(extraData.GetValue<int>("CodigoJanelaCarregamento"));

        Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(_unitOfWork);

        Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta dsOrdemColeta = null;
        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta> dsOrdensColeta =
            new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta>();
        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaPedidoProduto> dsProdutosPedido =
            new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaPedidoProduto>();
        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaRemessa> dsOrdemColetaRemessa =
            new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaRemessa>();
        decimal pesoTotal = 0m;

        string caminhoRelatorio = string.Empty;

        LayoutImpressaoOrdemColeta layoutImpressaoOrdemColeta =
            cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.LayoutImpressaoOrdemColeta ??
            LayoutImpressaoOrdemColeta.LayoutPadrao;

        if (layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto =
                new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtos =
                repositorioCargaPedidoProduto.BuscarPorCarga(codigoCarga);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos =
                produtos.Select(x => x.CargaPedido).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaJanelaCarregamento.Carga
                         .Pedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres =
                    repositorioCargaLacre.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                dsOrdensColeta.Add(ObterDataSetOrdemColetaPedido(cargaJanelaCarregamento, cargaPedido, cargaLacres));

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtosDoPedido =
                    (from p in produtos where p.CargaPedido.Codigo == cargaPedido.Codigo select p).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto produtoDoPedido in produtosDoPedido)
                    dsProdutosPedido.Add(ObterDataSetOrdemColetaPedidoProduto(produtoDoPedido));
            }
        }
        else if (layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar)
        {
            dsOrdemColeta = ObterDataSetOrdemColeta(cargaJanelaCarregamento, _unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido =
                new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos =
                repositorioCargaPedido.BuscarPorCarga(codigoCarga);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaRemessa ordemColetaRemessa =
                    new Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaRemessa()
                    {
                        Codigo = cargaPedido.Pedido?.Codigo ?? 0,
                        NumeroCarga = cargaPedido.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        NumeroPedido = cargaPedido.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                        Cliente = cargaPedido.Pedido?.Destinatario?.Descricao ?? string.Empty,
                        PesoBruto = cargaPedido.Peso.ToString("n2") ?? string.Empty,
                        UnidadeMedida = "KG",
                        ObservacaoExpedicao = "",
                        CidadeCliente = cargaPedido.Pedido.Destinatario?.Localidade?.Descricao ?? string.Empty,
                        EstadoCliente = cargaPedido.Pedido.Destinatario?.Localidade?.Estado?.Descricao ?? string.Empty
                    };

                pesoTotal += cargaPedido.Peso;
                dsOrdemColetaRemessa.Add(ordemColetaRemessa);
            }
        }
        else
            dsOrdemColeta = ObterDataSetOrdemColeta(cargaJanelaCarregamento, _unitOfWork);

        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "crystal.png");
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagem", caminhoLogo, true)
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet();

        if (layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar)
        {
            caminhoRelatorio = @"Areas\Relatorios\Reports\Default\Logistica\DocumentoAuxiliarOrdemColeta.rpt";

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PesoTotalPedidos",
                $"{pesoTotal.ToString("n2")} KG"));

            dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta>() { dsOrdemColeta },
                Parameters = parametros,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "Remessa",
                        DataSet = dsOrdemColetaRemessa
                    }
                }
            };
        }
        else
        {
            if (layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutColetaContainer)
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\Logistica\OrdemColetaContainer.rpt";
            else if (layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento)
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\Logistica\OrdemColetaCarregamento.rpt";
            else
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\Logistica\OrdemColeta.rpt";

            dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento
                    ? dsOrdensColeta
                    : new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta>() { dsOrdemColeta },
                Parameters = parametros,
                SubReports = layoutImpressaoOrdemColeta == LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento
                    ? new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                    {
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                        {
                            Key = "OrdemColetaCarregamentoPedido.rpt",
                            DataSet = dsProdutosPedido
                        }
                    }
                    : null
            };
        }

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(caminhoRelatorio, TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta ObterDataSetOrdemColeta(
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento,
        Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer =
            new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
        Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre =
            new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);

        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer =
            repositorioColetaContainer.BuscarPorCargaAtual(cargaJanelaCarregamento.Carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres =
            repositorioCargaLacre.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

        if (coletaContainer == null)
            coletaContainer = repositorioColetaContainer.BuscarPorCargaDeColeta(cargaJanelaCarregamento.Carga.Codigo);

        Dominio.Entidades.Usuario motorista = cargaJanelaCarregamento.Carga.Motoristas?.FirstOrDefault();

        ICollection<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos =
            cargaJanelaCarregamento.Carga.Pedidos;
        DateTime? dataPrevisaoEntrega =
            (from o in cargaPedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega)
            .FirstOrDefault();

        string destino = cargaJanelaCarregamento.Carga.DadosSumarizados?.Destinos ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(cargaJanelaCarregamento.Carga.ObservacaoLocalEntrega))
            destino += " (" + cargaJanelaCarregamento.Carga.ObservacaoLocalEntrega + ")";

        return new Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta(cargaJanelaCarregamento,
            coletaContainer, cargaLacres);
    }

    private Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta ObterDataSetOrdemColetaPedido(
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento,
        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido,
        List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres)
    {
        return new Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColeta(cargaJanelaCarregamento, cargaPedido,
            cargaLacres);
    }

    private Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaPedidoProduto
        ObterDataSetOrdemColetaPedidoProduto(Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto)
    {
        return new Dominio.Relatorios.Embarcador.DataSource.Logistica.OrdemColetaPedidoProduto()
        {
            CodigoPedido = cargaPedidoProduto.CargaPedido.Pedido.Codigo,
            PesoLiquido = cargaPedidoProduto.PesoLiquidoProduto.ToString("n2"),
            Quantidade = cargaPedidoProduto.Quantidade.ToString("n2"),
            Produto = cargaPedidoProduto.Produto.Descricao
        };
    }
}