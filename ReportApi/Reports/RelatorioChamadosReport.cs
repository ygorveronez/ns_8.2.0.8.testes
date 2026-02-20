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

[UseReportType(ReportType.RelatorioChamados)]
public class RelatorioChamadosReport : ReportBase
{
    public RelatorioChamadosReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoChamado = extraData.GetValue<int>("CodigoChamado");
        
        Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
        Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise =
            new Repositorio.Embarcador.Chamados.ChamadoAnalise(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);

        Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado);

        List<Dominio.Relatorios.Embarcador.DataSource.Chamados.Analise.Analise> dsAnaliseChamados =
            new List<Dominio.Relatorios.Embarcador.DataSource.Chamados.Analise.Analise>();
        List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> analises =
            repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
        string informacoesBancarias = "";

        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais =
            new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
        if (chamado.CargaEntrega != null)
            notasFiscais = repCargaEntregaNotaFiscal.BuscarNotaFiscalPorCargaEntrega(chamado.CargaEntrega.Codigo);

        List<string> tomadores = new List<string>();
        if (chamado.Tomador != null)
            tomadores.Add(chamado.Tomador.Descricao);

        Dominio.Entidades.Embarcador.Cargas.Carga carga = chamado.Carga;
        if (carga != null && carga.Pedidos != null && carga.Pedidos.Count > 0)
        {
            foreach (var cargaPedido in carga.Pedidos)
            {
                if (cargaPedido.Pedido != null && cargaPedido.Pedido.Destinatario != null)
                {
                    informacoesBancarias += "";
                    if (cargaPedido.Pedido.Destinatario.Banco != null)
                        informacoesBancarias += " Banco: " + cargaPedido.Pedido.Destinatario.Banco.Descricao;
                    if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.Destinatario.Agencia))
                        informacoesBancarias += " Ag: " + cargaPedido.Pedido.Destinatario.Agencia;
                    if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.Destinatario.NumeroConta))
                        informacoesBancarias += " Conta: " + cargaPedido.Pedido.Destinatario.NumeroConta;
                    if (string.IsNullOrWhiteSpace(informacoesBancarias))
                    {
                        if (cargaPedido.Pedido.Destinatario.TipoContaBanco ==
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente)
                            informacoesBancarias += " - Conta corrente";
                        else if (cargaPedido.Pedido.Destinatario.TipoContaBanco ==
                                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança)
                            informacoesBancarias += " - Conta poupança";
                        else if (cargaPedido.Pedido.Destinatario.TipoContaBanco ==
                                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Salario)
                            informacoesBancarias += " - Conta salário";
                    }

                    if (chamado.Tomador == null && cargaPedido.ObterTomador() != null &&
                        !tomadores.Contains(cargaPedido.ObterTomador()?.Descricao))
                        tomadores.Add(cargaPedido.ObterTomador()?.Descricao);
                }
            }
        }

        for (int i = analises.Count - 1; i >= 0; i--)
        {
            Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise = analises[i];
            dsAnaliseChamados.Add(new Dominio.Relatorios.Embarcador.DataSource.Chamados.Analise.Analise
            {
                Anotacao = analise.Observacao,
                Autor = analise.Autor.Nome,
                DataRegistro = analise.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                DataRetorno = analise.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Valor = chamado.Valor,
                Remetente = carga?.DadosSumarizados?.Remetentes ?? "",
                Destinatario = chamado.Destinatario?.Descricao ??
                               chamado.Cliente?.Descricao ?? carga?.DadosSumarizados?.Destinatarios ?? "",
                Tomador = string.Join(", ", tomadores),
                InformacoesBancarias = informacoesBancarias,
                Veiculos = carga?.RetornarPlacasComModelo,
                Motoristas = carga?.RetornarMotoristas
            });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsAnaliseChamados
            };

        // Calcula tempo total do chamado
        DateTime abertura = chamado.DataCriacao;
        DateTime dataFim = DateTime.Now;

        if (chamado.DataFinalizacao.HasValue)
            dataFim = chamado.DataFinalizacao.Value;

        // Remove Segundos
        abertura = abertura.AddSeconds(-abertura.Second);
        dataFim = dataFim.AddSeconds(-dataFim.Second);

        TimeSpan tempoAberto = (dataFim - abertura);
        List<string> splitTempoTotal = new List<string>();

        if (tempoAberto.Days > 0)
            splitTempoTotal.Add((tempoAberto.Days > 1 ? tempoAberto.Days + " Dias" : "1 Dia"));

        if (tempoAberto.Hours > 0)
            splitTempoTotal.Add((tempoAberto.Hours > 1 ? tempoAberto.Hours + " Horas" : "1 Hora"));

        if (tempoAberto.Minutes > 1)
            splitTempoTotal.Add(tempoAberto.Minutes + " Minutos");

        string tempoTotal = string.Join(" e ", splitTempoTotal);

        dataSet.Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TempoTotal", tempoTotal, true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Chamado", chamado.Numero.ToString(), true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga",
                carga?.CodigoCargaEmbarcador ?? string.Empty, true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador",
                carga?.Empresa?.Descricao ?? string.Empty, true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataChamado",
                chamado.DataCriacao.ToString("dd/MM/yyyy HH:mm"), true),
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NotasFiscais",
                notasFiscais.Select(o => o.XMLNotaFiscal.Numero).ToList())
        };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Chamados\Analises.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, true);
        
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}