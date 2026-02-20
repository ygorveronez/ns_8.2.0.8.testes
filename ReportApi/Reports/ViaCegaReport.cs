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

namespace ReportApi.Reports;

[UseReportType(ReportType.ViaCega)]
public class ViaCegaReport : ReportBase
{
    public ViaCegaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.Carga cargaViaCega = repCarga.BuscarPorCodigo(extraData.GetValue<int>("CodigoCarga"));

        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedido =
            new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscal =
            repositorioPedido.BuscarPorCargaFetchPedido(cargaViaCega.Codigo);
        List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ViaCega> dsViaCega =
            new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ViaCega>();
        List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ViaCegaNotaFiscal> dsViaCegaNotaFiscal =
            new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ViaCegaNotaFiscal>();

        int quantidadePedidos = repositorioCargaPedido.QuantidadeCargaPedido(cargaViaCega.Codigo);
        string caminhoLogo =
            Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"],
                "Crystal.png");

        int contador = 0;

        dsViaCega.Add(new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ViaCega()
        {
            LocalOrganizacao = cargaViaCega.Filial?.Descricao ?? string.Empty,
            LocalExpedicao = cargaViaCega.Filial?.CodigoFilialEmbarcador ?? string.Empty,
            NumeroDT = cargaViaCega.CodigoCargaEmbarcador ?? string.Empty,
            RazaoSocial = cargaViaCega.Empresa?.RazaoSocial ?? string.Empty,
            CNPJCPF = cargaViaCega.Empresa?.CNPJ_Formatado ?? string.Empty,
            Endereco = cargaViaCega.Empresa?.Endereco ?? string.Empty,
            Telefone = cargaViaCega.Empresa?.Telefone ?? string.Empty,
            InscEstadual = cargaViaCega.Empresa?.InscricaoEstadual ?? string.Empty,
            Bairro = cargaViaCega.Empresa?.Bairro ?? string.Empty,
            Cidade = cargaViaCega.Empresa?.Localidade.DescricaoCidadeEstado ?? string.Empty,
            ValorTotalNF = pedidosXmlNotaFiscal.Sum(o => o.XMLNotaFiscal.ValorTotalProdutos),
            PesoTotalNF = cargaViaCega.DadosSumarizados?.PesoTotal ?? 0,
            TotalNFs = pedidosXmlNotaFiscal?.Count() ?? 0,
            TotalVolumes = cargaViaCega?.DadosSumarizados?.VolumesTotal ?? 0,
            TotalCubado = cargaViaCega.DadosSumarizados?.CubagemTotal ?? 0,
            Cubagem = pedidosXmlNotaFiscal.Sum(o => o.XMLNotaFiscal.PesoCubado),
            PlacaCavalo = cargaViaCega.Veiculo?.Placa_Formatada ?? string.Empty,
            PlacaCarreta = string.Join(", ",
                (from r in cargaViaCega.VeiculosVinculados select r.Placa_Formatada).ToList()),
            TotalRemessas = quantidadePedidos.ToString(),
            TipoExpedicao = cargaViaCega.ModeloVeicularCarga?.Descricao ?? string.Empty,
            Nome = cargaViaCega.NomePrimeiroMotorista,
            CPF = cargaViaCega.CPFPrimeiroMotorista,
            RG = cargaViaCega.RGMotoristas,
        });

        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlNotaFiscal in pedidosXmlNotaFiscal)
        {
            if (contador % 2 == 0)
            {
                dsViaCegaNotaFiscal.Add(new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ViaCegaNotaFiscal()
                {
                    NumeroNotaFiscal = xmlNotaFiscal.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                    SerieNotaFiscal = xmlNotaFiscal.XMLNotaFiscal?.Serie ?? string.Empty,
                    NumeroRemessa = xmlNotaFiscal.CargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                    Volume = xmlNotaFiscal.CargaPedido?.Pedido?.QtVolumes.ToString() ?? string.Empty,
                });
            }
            else
            {
                int contadorSegundaColuna = dsViaCegaNotaFiscal.Count();

                dsViaCegaNotaFiscal[contadorSegundaColuna - 1].NumeroNotaFiscalSegundaColuna =
                    xmlNotaFiscal.XMLNotaFiscal.Numero.ToString();
                dsViaCegaNotaFiscal[contadorSegundaColuna - 1].SerieNotaFiscalSegundaColuna =
                    xmlNotaFiscal.XMLNotaFiscal.Serie;
                dsViaCegaNotaFiscal[contadorSegundaColuna - 1].NumeroRemessaSegundaColuna =
                    xmlNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                dsViaCegaNotaFiscal[contadorSegundaColuna - 1].VolumeSegundaColuna =
                    xmlNotaFiscal.CargaPedido.Pedido.QtVolumes.ToString();
            }

            contador++;
        }

        ;

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "ViaCegaNotaFiscal",
                DataSet = dsViaCegaNotaFiscal,
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CaminhoImagem", caminhoLogo, true)
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsViaCega,
                SubReports = subReports,
                Parameters = parametros
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\GestaoPatio\ViaCega.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}