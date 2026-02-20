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

namespace ReportApi.Reports;

[UseReportType(ReportType.GerarRelatorioEmbarcador)]
public class GerarRelatorioEmbarcadorReport : ReportBase
{
    public GerarRelatorioEmbarcadorReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoPedido = extraData.GetValue<int>("codigoPedido");
        bool planoViagem = extraData.GetValue<bool>("planoViagem");
        int codigoCarga = extraData.GetValue<int>("codigoCarga");

        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;

        if (codigoPedido < 0)
            pedido = repCargaPedido.BuscarPrimeiraPorCarga(codigoCarga)?.Pedido ?? null;
        else
            pedido = repPedido.BuscarPorCodigo(codigoPedido);

        Dominio.Entidades.Embarcador.Cargas.Carga carga =
            repCargaPedido.BuscarCargaPedidoPorPedido(pedido.Codigo)?.Carga ?? null;
        Dominio.Entidades.Usuario motorista = carga?.Motoristas?.FirstOrDefault();
        Dominio.Entidades.Veiculo veiculo = carga?.Veiculo;

        List<Dominio.Entidades.Veiculo> veiculoVinculados = new List<Dominio.Entidades.Veiculo>();
        if ((carga?.VeiculosVinculados?.Count() ?? 0) > 0)
            veiculoVinculados = carga.VeiculosVinculados.ToList();

        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas =
            new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
        if ((pedido?.NotasFiscais?.Count() ?? 0) > 0)
            notas = pedido.NotasFiscais.ToList();

        string numeroSM = "";
        List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS_Senhas> senhas =
            new List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS_Senhas>();

        if (carga != null)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao =
                new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao =
                new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao =
                repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);
            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao =
                    repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo);
                if (cargaCargaIntegracao != null)
                {
                    numeroSM = cargaCargaIntegracao.Protocolo;
                }
            }
        }

        Dominio.Entidades.Embarcador.Cargas.Carga cargaPedidos = pedido.CargasPedido?.ToList().FirstOrDefault();
        senhas = (from o in cargaPedidos?.Pedidos
            select new Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS_Senhas
            {
                SenhaAgendamento = o.Pedido.SenhaAgendamento,
                CodigoCD = o.Pedido.Filial?.CodigoFilialEmbarcador ?? "",
                DataEntrega = o.Pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
            }).ToList();

        int qtdEntregas = pedido?.CargasPedido?.Sum(p => p.Pedidos.Sum(o => o.Pedido.QtdEntregas)) ?? 0;

        if (senhas.Count == 0)
        {
            senhas.Add(new Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS_Senhas()
            {
                SenhaAgendamento = pedido.SenhaAgendamento,
                CodigoCD = pedido.Filial?.CodigoFilialEmbarcador ?? "",
                DataEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
            });
        }

        List<string> numeroPedidos = (from o in cargaPedidos.Pedidos select o.Pedido.NumeroPedidoEmbarcador).ToList();
        if (numeroPedidos.Count == 0)
            numeroPedidos.Add(pedido.NumeroPedidoEmbarcador);

        string observacao =
            Servicos.Embarcador.Pedido.Pedido.RetornarObservacaoCTeDoPedidoFormatado(pedido, carga, _unitOfWork);

        Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS ds =
            new Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS()
            {
                Numero = string.Join(", ", pedido.CargasPedido.Select(o => o.CodigoCargaEmbarcador).Distinct()),
                SenhaCarregamento = pedido.SenhaAgendamentoCliente,
                RemessaOTM = "",
                Data = pedido.DataInicialColeta ?? DateTime.Now,

                Remetente = planoViagem ? pedido.Remetente?.Descricao :
                    pedido.Filial != null ? (pedido.Filial.Descricao + " - " + pedido.Filial.CodigoFilialEmbarcador) :
                    "",
                CodigoFornecedor = pedido.Remetente?.CodigoIntegracao ?? "",
                Destinatario = planoViagem ? pedido.Destinatario?.Descricao : pedido.Remetente?.Nome ?? "",
                Endereco = pedido.Remetente?.Endereco ?? "",
                Bairro = pedido.Remetente?.Bairro ?? "",
                Cidade = planoViagem
                    ? pedido.Destino?.DescricaoCidadeEstado
                    : pedido.Remetente?.Localidade.Descricao ?? "",
                Contato = pedido.Remetente?.Email ?? "",
                CodigoCD = planoViagem
                    ? pedido.Origem?.DescricaoCidadeEstado
                    : pedido.Filial?.CodigoFilialEmbarcador ?? "",

                Transportadora = carga?.EmpresaFilialEmissora?.RazaoSocial ??
                                 (carga?.Empresa?.RazaoSocial ?? string.Empty),

                Cavalo = veiculo?.Placa ?? "",
                Carreta = string.Join(", ",
                    (from o in veiculoVinculados where !string.IsNullOrWhiteSpace(o.Placa) select o.Placa)),
                Motorista = motorista?.Nome ?? "",
                Operacao = pedido.TipoOperacao?.Descricao ?? "",
                Peso = pedido.PesoTotal,
                Valor = pedido.ValorFreteNegociado,
                QuantidadePallet = (int)pedido.NumeroPaletesFracionado + pedido.NumeroPaletes,
                NumeroLacre = "",
                Pedido = pedido.Remetente?.GrupoPessoas?.Descricao ?? "",
                NumeroPedido = string.Join(", ", numeroPedidos.Distinct()),
                TemperaturaVeiculo = pedido.Temperatura,
                NotaFiscal = planoViagem
                    ? qtdEntregas.ToString("D")
                    : string.Join(", ", (from o in notas select o.Numero)),

                Observacoes = observacao,
                SenhaAgendamento = pedido.SenhaAgendamento,
                DataEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataColeta = pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Orientacao = Localization.Resources.Pedidos.Pedido.NecessarioUtilizarEPI,
                ContatosGTS = "", //"54*31469",
                Buonny = numeroSM,
                Emitente = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario")).Nome,
                OrdemGTS = "", //"2018060511687",
                Normatizacao = "F-DLG-357 -08 -0",
                Impressao = DateTime.Now,
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaGTS>() { ds },
                SubReports = planoViagem
                    ? null
                    : (new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                    {
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
                        {
                            DataSet = senhas,
                            Key = "OrdemColeta_Senhas.rpt"
                        }
                    })
            };
        byte[] pdfContent;
        if (planoViagem)
            pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\Pedidos\PlanoViagemTMS.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        else
            pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\Pedidos\OrdemColeta.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}