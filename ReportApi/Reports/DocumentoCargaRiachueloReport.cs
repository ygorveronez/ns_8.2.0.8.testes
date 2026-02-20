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
using Zen.Barcode;

namespace ReportApi.Reports;

[UseReportType(ReportType.DocumentoCargaRiachuelo)]
public class DocumentoCargaRiachueloReport : ReportBase
{
    public DocumentoCargaRiachueloReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

        var carga = repCarga.BuscarPorCodigo(extraData.GetValue<int>("CodigoCarga"));

        byte[] codigoBarrasPlaca = null;
        string placaCaminhoes = !string.IsNullOrEmpty(carga.PlacasVeiculos)
            ? carga.PlacasVeiculos.PadLeft(carga.PlacasVeiculos.Length, '0')
            : (carga.Veiculo?.Placa.PadLeft(carga.Veiculo.Placa.Length, '0') ?? "0");
        if (!string.IsNullOrEmpty(placaCaminhoes))
        {
            codigoBarrasPlaca = Utilidades.Barcode.Gerar(placaCaminhoes, ZXing.BarcodeFormat.CODE_128,
                new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);
        }

        Dominio.Relatorios.Embarcador.DataSource.Cargas.DocumentoImpressaoCarga.DocumentoImpressaoCarga
            dataSourceDocumentoImpressaoCarga
                = new Dominio.Relatorios.Embarcador.DataSource.Cargas.DocumentoImpressaoCarga.DocumentoImpressaoCarga()
                {
                    Emissao = DateTime.Now,
                    Criacao = carga?.DataCriacaoCarga ?? DateTime.MinValue,
                    Operador = carga?.Operador?.Nome ?? "-",
                    Filial = carga?.Filial?.Descricao ?? "-",
                    TipoDeOperacao = carga?.TipoOperacao?.Descricao ?? "-",
                    PlacaCaminhaoCavalo = !string.IsNullOrEmpty(carga.PlacasVeiculos)
                        ? carga.PlacasVeiculos
                        : (carga.Veiculo?.Placa ?? "-"),
                    Frota = carga?.Veiculo?.NumeroFrota.ToString() ?? "-",
                    Empresa = carga?.Empresa?.Descricao ?? "-",
                    ModeloVeicular = carga?.ModeloVeicularCarga?.Descricao ?? "-",
                    Motorista = carga?.Motoristas?.FirstOrDefault()?.Nome ?? "-",
                    CPF = carga?.Motoristas?.FirstOrDefault()?.CPF_Formatado.ToString() ?? "-",
                    CargaTMS = carga?.CodigoCargaEmbarcador.ToString() ?? "-",
                    DataPrevisaoSaida = carga?.Pedidos.Select(p => p.Pedido.DataPrevisaoSaida).FirstOrDefault() ??
                                        DateTime.MinValue,
                    CodigoBarrasPlaca = codigoBarrasPlaca
                };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet =
                    new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DocumentoImpressaoCarga.
                        DocumentoImpressaoCarga>() { dataSourceDocumentoImpressaoCarga },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            };

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DocumentoImpressaoCarga.DocumentoImpressaoCargaPedido>
            dataSourceDocumentoImpressaoCargaPedido;
        dataSourceDocumentoImpressaoCargaPedido = (from pedidos in carga.Pedidos.OrderByDescending(p => p.OrdemEntrega)
            select new Dominio.Relatorios.Embarcador.DataSource.Cargas.DocumentoImpressaoCarga.
                DocumentoImpressaoCargaPedido()
                {
                    Loja = pedidos?.ClienteEntrega?.CodigoIntegracao ?? "-",
                    DataHoraEntrega = pedidos.Pedido.PrevisaoEntrega.ToString() ?? "-",
                }).ToList();

        dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "DocumentoCargaPedidoRiachuelo",
                DataSet = dataSourceDocumentoImpressaoCargaPedido,
            }
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\DocumentoCargaRiachuelo.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}