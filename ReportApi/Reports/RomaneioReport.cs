using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.Romaneio)]
public class RomaneioReport : ReportBase
{
    public RomaneioReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral =
            new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRomaneio tipoRomaneio =
            repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao().TipoRomaneio;

        Repositorio.Embarcador.Cargas.Carga repositorioPosicao = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioPosicao.BuscarPorCodigo(codigoCarga);

        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador =
            new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador =
            repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal =
            new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal
            .BuscarPorCarga(carga.Codigo).OrderBy(o => o.CargaPedido.OrdemEntrega)
            .ThenBy(o => o.CargaPedido.Pedido.Destinatario.CPF_CNPJ).ToList();
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos =
            new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

        if (notasFiscais.Count == 0)
            cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet;
        string caminhoRelatorio = "";

        switch (tipoRomaneio)
        {
            case TipoRomaneio.Obramax:
                dataSet = ObterDataSetObramax(carga, notasFiscais, cargaPedidos, _unitOfWork);
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\Cargas\RomaneioViagemObramax.rpt";
                break;
            default:
                dataSet = ObterDataSet(carga, notasFiscais, cargaPedidos, configuracaoEmbarcador);
                caminhoRelatorio = @"Areas\Relatorios\Reports\Default\Cargas\RomaneioViagem.rpt";
                break;
        }

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(caminhoRelatorio,
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSet(
        Dominio.Entidades.Embarcador.Cargas.Carga carga,
        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais,
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos,
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
    {
        Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

        List<Dominio.Entidades.Cliente> remetentes;

        if (notasFiscais.Count == 0)
            remetentes = cargaPedidos.Select(o => o.Pedido.Remetente).Distinct().ToList();
        else
            remetentes = (from notas in notasFiscais select notas?.CargaPedido?.Pedido?.Remetente).Distinct().ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagem>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagem()
                    {
                        NumeroCarga = servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador),
                        Motorista = carga?.Motoristas?.FirstOrDefault()?.Nome ?? "-",
                        Telefone = carga?.Motoristas?.FirstOrDefault()?.Telefone ?? "-",
                        RG = carga?.Motoristas?.FirstOrDefault()?.RG ?? "-",
                        Placa = !string.IsNullOrEmpty(carga?.PlacasVeiculos)
                            ? carga.PlacasVeiculos
                            : (carga?.Veiculo?.Placa) ?? "-",
                        LocalPartida =
                            string.Join(", ", (from remetente in remetentes select remetente?.Nome).ToList()),
                        HorarioPartida = carga?.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                        Endereco = string.Join(", ",
                            (from remetente in remetentes select remetente?.EnderecoCompleto).ToList()),
                        OcupacaoPeso =  servicoCarga.BuscarPorcentagemOcupacaoVeiculo(carga, _unitOfWork).ToString("n2") + "%",
                        OcupacaoVolume = carga?.DadosSumarizados?.CubagemTotal.ToString("n2") + "%" ?? "-",
                        TempoRota = "-",
                        DistanciaEstimada = "-",
                        Combustivel = "-",
                        Pedagio = "-",
                        QuantidadeNotasFiscais = notasFiscais.Count == 0
                            ? cargaPedidos.Count.ToString()
                            : notasFiscais.Count.ToString(),
                        Peso = carga?.DadosSumarizados?.PesoTotal.ToString() ?? "-",
                        Volume = carga?.DadosSumarizados?.VolumesTotal.ToString("n0") ?? "-",
                    }
                },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "RomaneioViagemCargaPedido",
                        DataSet = ObterDataSetCargaPedido(carga, notasFiscais, cargaPedidos)
                    }
                },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>(),
            };

        return dataSet;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemCargaPedido>
        ObterDataSetCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga,
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemCargaPedido>
            dataSourceRomaneioViagemCargaPedido;

        if (notasFiscais.Count == 0)
            dataSourceRomaneioViagemCargaPedido = (from cargaPedido in cargaPedidos
                                                   select new Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemCargaPedido()
                                                   {
                                                       Codigo = carga?.Codigo.ToString() ?? "-",
                                                       Cliente = cargaPedido.Pedido.Destinatario?.Nome ?? "-",
                                                       HorarioProgramado = "-",
                                                       Telefone = cargaPedido.Pedido.Destinatario?.Telefone1 ?? "-",
                                                       Endereco = cargaPedido.Pedido.Destinatario?.Endereco ?? "-",
                                                       Complemento = cargaPedido.Pedido.Destinatario?.Complemento ?? "-",
                                                       NrPedido = cargaPedido.Pedido.Numero.ToString() ?? "-",
                                                       NrNF = cargaPedido.Pedido.NumeroNotaCliente.ToString() ?? "-",
                                                       Peso = "-",
                                                       Volume = "-",
                                                   }).ToList();
        else
            dataSourceRomaneioViagemCargaPedido = (from notaFiscal in notasFiscais
                                                   select new Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemCargaPedido()
                                                   {
                                                       Codigo = carga?.Codigo.ToString() ?? "-",
                                                       Cliente = notaFiscal?.CargaPedido?.Pedido?.Destinatario?.Nome ?? "-",
                                                       HorarioProgramado = "-",
                                                       Telefone = notaFiscal?.CargaPedido?.Pedido?.Destinatario?.Telefone1 ?? "-",
                                                       Endereco = notaFiscal.CargaPedido?.Pedido?.Destinatario?.Endereco ?? "-",
                                                       Complemento = notaFiscal?.CargaPedido?.Pedido?.Destinatario?.Complemento ?? "-",
                                                       NrPedido = notaFiscal?.CargaPedido?.Pedido?.Numero.ToString() ?? "-",
                                                       NrNF = notaFiscal?.XMLNotaFiscal?.Numero.ToString() ?? "-",
                                                       Peso = notaFiscal?.XMLNotaFiscal?.Peso.ToString() ?? "-",
                                                       Volume = notaFiscal?.XMLNotaFiscal?.Volumes.ToString() ?? "-",
                                                   }).ToList();

        return dataSourceRomaneioViagemCargaPedido;
    }

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterDataSetObramax(
        Dominio.Entidades.Embarcador.Cargas.Carga carga,
        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais,
        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

        double.TryParse(carga?.Filial?.CNPJ ?? "0", out double filialCNPJ);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet =
                    new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemObramax>()
                    {
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemObramax()
                        {
                            NumeroCarga = carga?.CodigoCargaEmbarcador ?? "-",
                            Motorista = carga?.Motoristas?.FirstOrDefault()?.Nome ?? "-",
                            Filial = carga?.Filial?.Descricao ?? "-",
                            EnderecoFilial = repCliente.BuscarPorCPFCNPJ(filialCNPJ)?.EnderecoCompleto ?? "-",
                            Placa = carga?.Veiculo?.Placa ?? "-",
                            HoraPartida = carga?.DataFinalizacaoEmissao?.ToString("HH:mm") ?? "-",
                            DataPartida = carga?.DataFinalizacaoEmissao?.ToString("dd/MM/yyyy") ?? "-",
                            Peso = carga?.DadosSumarizados?.PesoTotal.ToString("n2") ?? "-",
                            Ajudante = carga?.Ajudantes?.FirstOrDefault()?.Nome ?? "-",
                            Romaneio = carga?.Protocolo.ToString() ?? "-",
                        }
                    },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "RomaneioViagemCargaPedidoObramax.rpt",
                        DataSet = ObterDataSetCargaPedidoObramax(notasFiscais, cargaPedidos)
                    }
                },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>(),
            };

        return dataSet;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemCargaPedidoObramax>
        ObterDataSetCargaPedidoObramax(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.RomaneioViagemCargaPedidoObramax>
            dataSourceRomaneioViagemCargaPedido;

        if (notasFiscais.Count == 0)
            dataSourceRomaneioViagemCargaPedido = (from cargaPedido in cargaPedidos
                                                   select new Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.
                                                       RomaneioViagemCargaPedidoObramax()
                                                   {
                                                       NotaFiscal = cargaPedido?.Pedido?.NumeroNotaCliente.ToString() ?? "-",
                                                       Pedido = cargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? "-",
                                                       Regiao = cargaPedido?.Pedido?.LocalidadeEnderecoDestinoAtual?.DescricaoCidadeEstado ?? "-",
                                                       DataEntrega = cargaPedido?.Pedido?.PrevisaoEntrega?.ToString("dd/MM/yyyy") ?? "-",
                                                       HoraEntrega = cargaPedido?.Pedido?.PrevisaoEntrega?.ToString("HH:mm") ?? "-",
                                                       VolumeCarga = cargaPedido?.QtVolumes.ToString() ?? "-",
                                                       DestinatarioCEP = cargaPedido?.Pedido?.CEPEnderecoDestinoAtual ?? "-",
                                                       Zona = "",
                                                       Cliente = cargaPedido.ObterDestinatario()?.Nome ?? "-",
                                                   }).ToList();
        else
            dataSourceRomaneioViagemCargaPedido = (from notaFiscal in notasFiscais
                                                   select new Dominio.Relatorios.Embarcador.DataSource.Cargas.RomaneioViagem.
                                                       RomaneioViagemCargaPedidoObramax()
                                                   {
                                                       NotaFiscal = notaFiscal?.XMLNotaFiscal?.Numero.ToString() ?? "-",
                                                       Pedido = notaFiscal?.CargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? "-",
                                                       Regiao =
                                                               notaFiscal?.CargaPedido?.Pedido?.LocalidadeEnderecoDestinoAtual?.DescricaoCidadeEstado ??
                                                               "-",
                                                       DataEntrega = notaFiscal?.CargaPedido?.Pedido?.PrevisaoEntrega?.ToString("dd/MM/yyyy") ?? "-",
                                                       HoraEntrega = notaFiscal?.CargaPedido?.Pedido?.PrevisaoEntrega?.ToString("HH:mm") ?? "-",
                                                       VolumeCarga = notaFiscal?.XMLNotaFiscal?.Volumes.ToString() ?? "-",
                                                       DestinatarioCEP = notaFiscal?.CargaPedido?.Pedido?.CEPEnderecoDestinoAtual ?? "-",
                                                       Zona = "",
                                                       Cliente = notaFiscal?.CargaPedido?.ObterDestinatario()?.Nome ?? "-",
                                                   }).ToList();

        return dataSourceRomaneioViagemCargaPedido;
    }
}