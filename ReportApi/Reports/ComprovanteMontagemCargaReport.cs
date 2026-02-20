using System;
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

[UseReportType(ReportType.ComprovanteMontagemCarga)]
public class ComprovanteMontagemCargaReport : ReportBase
{
    public ComprovanteMontagemCargaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("codigo");
        Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio repositorioMontagemCargaPatio =
            new Repositorio.Embarcador.GestaoPatio.MontagemCargaPatio(_unitOfWork);
        Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio montagemCargaPatio =
            repositorioMontagemCargaPatio.BuscarPorCodigo(codigo);

        if (montagemCargaPatio == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento =
            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao =
            new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(_unitOfWork);

        Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao =
            servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();
        Dominio.Entidades.Embarcador.Cargas.Carga carga = montagemCargaPatio.Carga;
        Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados = carga.DadosSumarizados;
        Dominio.Entidades.Veiculo reboque = carga.VeiculosVinculados?.FirstOrDefault();
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento =
            repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
        List<string> numerosPedidos = repositorioCargaPedido.BuscarNumeroPedidosPorCarga(carga.Codigo);

        Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteMontagemCarga dsComprovante =
            new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteMontagemCarga()
            {
                DataCarregamento = carga.DataCarregamentoCarga?.ToDateTimeString() ?? string.Empty,
                CodigoControleCarregamento = configuracao.MontagemCargaCodigoControle,
                CodigoIntegracaoFilial = carga.Filial.CodigoFilialEmbarcador,
                TipoOperacao = carga.TipoOperacao.Descricao,
                NumeroCarga = carga.CodigoCargaEmbarcador,
                NumeroPedido = string.Join(", ", numerosPedidos),
                CodigoIntegracaoDestinatarios = dadosSumarizados.CodigoIntegracaoDestinatarios,
                Destinatarios = dadosSumarizados.Destinatarios,
                Destinos = dadosSumarizados.Destinos,
                HorarioJanelaCarregamento = janelaCarregamento?.InicioCarregamento.ToTimeString() ?? string.Empty,
                Transportadora = carga.Empresa.Descricao,
                PlacaVeiculo = carga.Veiculo?.Placa ?? string.Empty,
                UFVeiculo = carga.Veiculo?.Estado.Sigla ?? string.Empty,
                PlacaCarreta = reboque?.Placa ?? string.Empty,
                UFCarreta = reboque?.Estado.Sigla ?? string.Empty
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteMontagemCarga>()
                    { dsComprovante }
            };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\GestaoPatio\ComprovanteMontagemCarga.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        if (pdfContent == null)
            throw new ServicoException("Não foi possível gerar o comprovante da montagem de carga.");

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}