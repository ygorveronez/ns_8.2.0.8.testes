using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ResumoAgendamentoColetaSams)]
public class ResumoAgendamentoColetaSamsReport : ReportBase
{
    public ResumoAgendamentoColetaSamsReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

        var agendamento = repAgendamentoColeta.BuscarPorCodigo(extraData.GetValue<int>("CodigoAgendamento"));
        var cargaJanelaDescarregamento = repCargaJanelaDescarregamento.BuscarPorCodigo(extraData.GetValue<int>("CodigoCargaJanelaDescarregamento"), false);

        Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaSams DSresumo =
            new Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaSams
            {
                Situacao = cargaJanelaDescarregamento.Situacao.ObterDescricao() ?? string.Empty,
                SenhaAgendamento = agendamento.Senha,
                DataAgendamento = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") ??
                                  string.Empty,
                VolumesAgendados = agendamento.Volumes.ToString(),
                Observacao = agendamento.Observacao,
                NumeroCarga = agendamento.Carga.Numero,
            };

        Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto =
            new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet pedidos =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ResumoAgendamentoColetaPedidosSams_.rpt",
                DataSet = (from agendamentoPedido in agendamento.Pedidos
                           select new Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaPedidosSams
                           {
                               Remetente = agendamentoPedido.Pedido.Remetente?.NomeCNPJ ?? string.Empty,
                               NumeroPedido = agendamentoPedido.Pedido.NumeroPedidoEmbarcador,
                               QuantidadeCaixas = agendamentoPedido.VolumesEnviar,
                               QuantidadeItens = agendamentoPedido.SKU,
                               Valor = agendamentoPedido.Pedido.ValorTotalNotasFiscais,
                               TipoOperacao = agendamentoPedido.Pedido.TipoOperacao?.Descricao ?? string.Empty,
                               TipoCarga = agendamentoPedido.Pedido.TipoDeCarga?.Descricao ?? string.Empty,
                               Setor = agendamentoPedido.Pedido.ProdutoPrincipal?.GrupoProduto != null
                                   ? agendamentoPedido.Pedido.ProdutoPrincipal.GrupoProduto.Descricao
                                   : agendamentoPedido.Pedido.Produtos?.FirstOrDefault().Produto?.GrupoProduto?.Descricao ??
                                     string.Empty,
                               DescricaoFilial = cargaJanelaDescarregamento?.CentroDescarregamento?.Descricao ?? string.Empty,
                           }).ToList()
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaSams>()
                    { DSresumo },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    pedidos
                }
            };

        Dominio.Enumeradores.TipoArquivoRelatorio tipo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
        string arquivoRPT = @"Areas\Relatorios\Reports\Default\Logistica\ResumoAgendamentoColetaSams.rpt";

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(arquivoRPT, tipo, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}