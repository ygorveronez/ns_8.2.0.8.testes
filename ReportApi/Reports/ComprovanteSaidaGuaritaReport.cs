using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.ComprovanteSaidaGuarita)]
public class ComprovanteSaidaGuaritaReport : ReportBase
{
    public ComprovanteSaidaGuaritaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio repositorioConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoFluxoPatio = repositorioConfiguracaoFluxoPatio.BuscarConfiguracaoPadrao();
        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);

        int codigoGuarita = extraData.GetValue<int>("codigoGuarita");
        int codigoFluxoGestaoPatio = extraData.GetValue<int>("codigoFluxoGestaoPatio");

        Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = null;

        if (codigoGuarita > 0)
            cargaGuarita = repositorioCargaGuarita.BuscarPorCodigo(codigoGuarita);
        else if (codigoFluxoGestaoPatio > 0)
            cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

        if (cargaGuarita == null)
            throw new ServicoException("Carga não encontrada.");

        if (cargaGuarita.FluxoGestaoPatio == null)
            throw new ServicoException("Carga não encontrada.");

        Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaGuarita.FluxoGestaoPatio);

        bool informarDataSaida = sequenciaGestaoPatio?.ChegadaVeiculoPreencherDataSaida ?? false;

        byte[] qrCode = Utilidades.QRcode.Gerar($"{cargaGuarita.Carga.Codigo}");
        string caminhoRelatorio = string.Empty;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = null;

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        if (configuracaoFluxoPatio.TipoComprovanteSaida == TipoComprovanteSaida.RomaneioCarregamento)
        {
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TituloRelatorio", "ROMANEIO DE CARREGAMENTO", true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RodapeTexto", "DESENVOLVIDO POR MULTISOFTWARE", true));

            caminhoRelatorio = @"Areas\Relatorios\Reports\Default\GestaoPatio\RomaneioCarregamento.rpt";

            List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioCarregamento> dsRomaneioCarregamento = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioCarregamento>();

            foreach (var cargaPedido in cargaGuarita.Carga.Pedidos)
            {
                dsRomaneioCarregamento.Add(
                    new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioCarregamento()
                    {
                        NumeroCarga = cargaPedido.Carga.CodigoCargaEmbarcador,
                        Transportador = cargaPedido.Carga.Empresa?.Descricao ?? string.Empty,
                        NomeMotorista = cargaPedido.Carga.Motoristas.FirstOrDefault()?.Nome,
                        PlacaCavalo = cargaPedido.Carga.Veiculo?.Placa ?? "",
                        PlacaCarreta = cargaPedido.Carga.VeiculosVinculados != null
                            ? string.Join(", ", (from o in cargaPedido.Carga.VeiculosVinculados select o.Placa))
                            : "",
                        Cliente = cargaPedido.Pedido.Remetente?.CodigoIntegracao ?? string.Empty,
                        Destinatario = cargaPedido.Pedido.Destinatario?.Descricao ?? string.Empty,
                        GrupoDestinatario = cargaPedido.Carga.GrupoPessoaPrincipal?.Descricao ?? string.Empty,
                        TipoCarga = cargaPedido.Pedido.TipoDeCarga.Descricao,
                        Volume = cargaPedido.QtVolumes,
                        Destino = cargaPedido.Pedido.Destino?.Descricao + " - " + cargaPedido.Pedido.Destino?.Estado?.Descricao ?? string.Empty,
                        Peso = cargaPedido.Peso,
                        Valor = cargaPedido.Pedido.ValorCarga ?? 0,
                        NotasFiscais = string.Join(";",
                            cargaPedido.NotasFiscais.Select(cenf => cenf.XMLNotaFiscal?.Numero.ToString())
                                .DefaultIfEmpty("0")),
                        CPF = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.CPF,
                        RG = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.RG
                    });
            }

            dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsRomaneioCarregamento,
                Parameters = parametros
            };
        }
        else
        {
            Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteSaida dsComprovanteSaida = new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteSaida();

            if (sequenciaGestaoPatio?.ChegadaVeiculoImprimirComprovanteModeloColetaOutbound ?? false)
            {
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\GestaoPatio\ComprovanteColetaOutbound.rpt";
                dsComprovanteSaida = new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteSaida()
                {
                    NumeroCarga = cargaGuarita.Carga.CodigoCargaEmbarcador,
                    Doca = cargaGuarita.DocaChegadaGuarita ?? string.Empty,
                    PrevisaoEntrega = (cargaGuarita.Carga.DadosSumarizados?.DataPrevisaoEntrega.HasValue ?? false)
                        ? cargaGuarita.Carga.DadosSumarizados.DataPrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm")
                        : string.Empty,
                    QRCode = qrCode,
                    Senha = cargaGuarita.SenhaChegadaGuarita ?? string.Empty,
                    NomeMotorista = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.Nome,
                    Transportador = cargaGuarita.Carga.Empresa?.Descricao ?? string.Empty,
                    PlacaCavalo = cargaGuarita.Carga.Veiculo?.Placa ?? "",
                    PlacaCarreta = cargaGuarita.Carga.VeiculosVinculados != null
                        ? string.Join(", ", (from o in cargaGuarita.Carga.VeiculosVinculados select o.Placa))
                        : "",
                    NomeCliente = cargaGuarita.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                    Fornecedor = cargaGuarita.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                    NotasFiscais = string.Join(";",
                        cargaGuarita.Carga.Entregas?.SelectMany(obj =>
                            obj.NotasFiscais?.Select(cenf =>
                                cenf.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero.ToString())).DefaultIfEmpty("0"))
                };
            }
            else
            {
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\GestaoPatio\ComprovanteSaida.rpt";
                dsComprovanteSaida = new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteSaida()
                {
                    DataAgenda = cargaGuarita.Carga.DataCarregamentoCarga.HasValue
                        ? cargaGuarita.Carga.DataCarregamentoCarga.Value
                        : DateTime.Now,
                    Senha = cargaGuarita.Carga.CodigoCargaEmbarcador,
                    Fornecedor = cargaGuarita.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                    CodigoIntegracaoDestinatario = cargaGuarita.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                    NomeMotorista = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.Nome,
                    PlacaCavalo = cargaGuarita.Carga.Veiculo?.Placa ?? "",
                    PlacaCarreta = cargaGuarita.Carga.VeiculosVinculados != null
                        ? String.Join(", ", (from o in cargaGuarita.Carga.VeiculosVinculados select o.Placa))
                        : "",
                    Telefone = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.Telefone,
                    DataChegada = cargaGuarita.DataChegadaVeiculo ?? DateTime.MinValue,
                    DataSaida = informarDataSaida
                        ? ""
                        : cargaGuarita.FluxoGestaoPatio.DataDeslocamentoPatio?.ToString("dd/MM/yyyy HH:mm"),
                    QRCode = qrCode,
                    CPF = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.CPF,
                    RG = cargaGuarita.Carga.Motoristas.FirstOrDefault()?.RG
                };
            }

            dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ComprovanteSaida>()
                    { dsComprovanteSaida },
                Parameters = parametros
            };
        }

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(caminhoRelatorio,
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o comprovante de saída.");

        return PrepareReportResult(FileType.PDF, pdf);
    }
}