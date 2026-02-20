using System;
using System.Collections.Generic;
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

[UseReportType(ReportType.ResumoAgendamentoColeta)]
public class ResumoAgendamentoColetaReport : ReportBase
{
    public ResumoAgendamentoColetaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento repAlteracaoPedidoProdutoAgendamento = new Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento(_unitOfWork);
        Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
        Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(_unitOfWork);
        Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargasCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

        int codigoAgendamento = extraData.GetValue<int>("CodigoAgendamento");
        int codigoCargaJanelaDescarregamento = extraData.GetValue<int>("CodigoCargaJanelaDescarregamento");

        var agendamento = repAgendamentoColeta.BuscarPorCodigo(codigoAgendamento);
        var cargaJanelaDescarregamento = repCargasCargaJanelaDescarregamento.BuscarPorCodigo(codigoCargaJanelaDescarregamento, false);
        Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
        Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repositorioAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(_unitOfWork);

        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaPedidos> dsResumoAgendamentoColetaPedidos = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaPedidos>();
        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColeta> dsResumoAgendamentoColeta = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColeta>();
        List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> agendamentosColetaPedidoProdutos = repositorioAgendamentoColetaPedidoProduto.BuscarPorCodigoAgendamentoColeta(agendamento.Codigo);

        dsResumoAgendamentoColeta.Add(new Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColeta
        {
            Situacao = cargaJanelaDescarregamento?.Situacao.ObterDescricao() ?? string.Empty,
            SenhaAgendamento = agendamento.Senha,
            DataAgendamento = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
            VolumesAgendados = agendamento.Volumes.ToString(),
            Observacao = agendamento.Observacao,
            NumeroCarga = agendamento.Carga.Numero,
            DescricaoFilial = cargaJanelaDescarregamento?.CentroDescarregamento?.Descricao ?? string.Empty,
            TipoDeCarga = agendamento.TipoCarga?.Descricao ?? string.Empty,
        });

        foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto agendamentoColetaPedidoProduto in agendamentosColetaPedidoProdutos)
        {
            decimal valorTotal = (agendamentoColetaPedidoProduto.PedidoProduto.PrecoUnitario * agendamentoColetaPedidoProduto.Quantidade);

            dsResumoAgendamentoColetaPedidos.Add(new Dominio.Relatorios.Embarcador.DataSource.Logistica.ResumoAgendamentoColetaPedidos()
            {
                Remetente = agendamentoColetaPedidoProduto.PedidoProduto.Pedido.Remetente?.NomeCNPJ ?? string.Empty,
                NumeroPedido = agendamentoColetaPedidoProduto.PedidoProduto.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                Valor = valorTotal,
                Setor = agendamentoColetaPedidoProduto.PedidoProduto.Produto.GrupoProduto?.Descricao ?? string.Empty,
                TipoOperacao = agendamentoColetaPedidoProduto.PedidoProduto.Pedido.TipoOperacao?.Descricao ?? string.Empty,
                TipoCarga = agendamentoColetaPedidoProduto.PedidoProduto.Pedido.TipoDeCarga?.Descricao ?? string.Empty,
                QuantidadeProduto = agendamentoColetaPedidoProduto.Quantidade,
                PrecoUnitarioProduto = agendamentoColetaPedidoProduto.PedidoProduto.PrecoUnitario,
                CodigoIntegracaoEDescricaoProduto = agendamentoColetaPedidoProduto.PedidoProduto.Produto.CodigoProdutoEmbarcador.ToString() + " - " + agendamentoColetaPedidoProduto.PedidoProduto.Produto.Descricao,
                QuantidadeDeCaixas = agendamentoColetaPedidoProduto.QuantidadeDeCaixas,
            });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "_ResumoAgendamentoColetaPedidos_.rpt",
                DataSet = dsResumoAgendamentoColetaPedidos,
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>() { };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsResumoAgendamentoColeta,
                SubReports = subReports,
                Parameters = parametros
            };

        Dominio.Enumeradores.TipoArquivoRelatorio tipo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
        string arquivoRPT = @"Areas\Relatorios\Reports\Default\Logistica\ResumoAgendamentoColeta.rpt";

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(arquivoRPT, tipo, dataSet, true);
        var pdfcontent = RelatorioSemPadraoReportService.ObterBufferReport(report, tipo);

        return PrepareReportResult(FileType.PDF, pdfcontent);
    }
}