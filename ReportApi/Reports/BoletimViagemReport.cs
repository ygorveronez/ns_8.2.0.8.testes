using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.Entidades.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Reports;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

[UseReportType(ReportType.BoletimViagem)]
public class BoletimViagemReport : ReportBase
{
    public BoletimViagemReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCarga = extraData.GetValue<int>("CodigoCarga");
        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
        
        Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal =
            new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador =
            new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodosDescarga =
            new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(_unitOfWork);

        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador =
            repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal
            .BuscarPorCarga(carga.Codigo).OrderBy(o => o.CargaPedido.OrdemEntrega)
            .ThenBy(o => o.CargaPedido.Pedido.Destinatario.CPF_CNPJ).ToList();

        List<Dominio.Entidades.Cliente> remetentes =
            (from notas in notasFiscais select notas?.CargaPedido?.Pedido?.Remetente).Distinct().ToList();

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos =
            (from cargapedido in carga.Pedidos select cargapedido)?.OrderBy(o => o.OrdemEntrega).ToList();

        List<double> cpfCnpjDestinatarios =
            (from obj in cargaPedidos where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario.CPF_CNPJ)
            .Distinct().ToList();
        List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarga =
            repositorioPeriodosDescarga.BuscarPorDestinatariosTipoCarga(cpfCnpjDestinatarios,
                carga?.TipoDeCarga?.Codigo ?? 0);

        Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagem dataSourceBoletimViagem =
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagem()
            {
                DataDaCarga = carga.DataCriacaoCarga,
                Lacre = string.Join(", ", (from lacre in carga?.Lacres select lacre.Descricao).ToList()),
                NumeroCarga = servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador),
                Motorista = carga?.Motoristas?.FirstOrDefault()?.Nome ?? "",
                Placa = !string.IsNullOrEmpty(carga.PlacasVeiculos)
                    ? carga.PlacasVeiculos
                    : (carga.Veiculo?.Placa ?? ""),
                Remetente = string.Join(", ",
                    (from remetente in remetentes select remetente?.CPF_CNPJ + " " + remetente?.Nome).ToList()),
                EnderecoRemetente = string.Join(", ",
                    (from remetente in remetentes
                        select remetente?.CodigoIntegracao + " " + remetente?.EnderecoCompleto).ToList()),
                Observacao = "",
                Transportador = carga?.Empresa.Descricao,
                Carimbo = carga?.TipoDeCarga?.ControlaTemperatura == true
                    ? (carga?.TipoDeCarga?.FaixaDeTemperatura?.Carimbo ?? 0)
                    : 0,
                CarimboDescricao = carga?.TipoDeCarga?.ControlaTemperatura == true
                    ? (carga?.TipoDeCarga?.FaixaDeTemperatura?.CarimboDescricao ?? "")
                    : "",
                FaixaDeTemperaturaDescricao = carga?.TipoDeCarga?.ControlaTemperatura == true
                    ? (carga?.TipoDeCarga?.FaixaDeTemperatura?.Descricao ?? "")
                    : "",
                TipoDeCarga = carga?.TipoDeCarga?.Descricao,
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagem>()
                    { dataSourceBoletimViagem },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            };

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemDestinatario>
            dataSourceBoletimViagemDestinatario;
        dataSourceBoletimViagemDestinatario = (from notaFiscal in notasFiscais
            where notaFiscal.CargaPedido.Pedido.Destinatario != null
            select new Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem.BoletimViagemDestinatario()
            {
                CodigoCarga = carga?.Codigo ?? 0,
                CodigoIntegracao = notaFiscal.CargaPedido.Pedido.Destinatario.CodigoIntegracao,
                CPF_CNPJ = notaFiscal.CargaPedido.Pedido.Destinatario.CPF_CNPJ,
                Endereco = notaFiscal.CargaPedido.Pedido.Destinatario.Endereco,
                Destinatario = notaFiscal.CargaPedido.Pedido.Destinatario.Nome,
                Valor = notaFiscal.XMLNotaFiscal?.Valor ?? 0,
                Numero = notaFiscal.XMLNotaFiscal?.Numero ?? 0,
                HorariosDescarga = ObterPeriodosDescarga(periodosDescarga)
            }).ToList();

        dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "BoletimViagemDestinatario",
                DataSet = dataSourceBoletimViagemDestinatario,
            },
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\BoletimViagem.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }
    
    private string ObterPeriodosDescarga(List<PeriodoDescarregamento> periodosDescarga)
    {
        StringBuilder periodos = new StringBuilder();

        foreach (PeriodoDescarregamento periodo in periodosDescarga)
            periodos.AppendLine(periodo.Descricao);

        return periodos.ToString();
    }
}