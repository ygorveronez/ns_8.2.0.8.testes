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

[UseReportType(ReportType.RelacaoPedidoPacoteCarga)]
public class RelacaoPedidoPacoteCarga : ReportBase
{
    public RelacaoPedidoPacoteCarga(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote.RelacaoPedidoPacote> listaDadosCarga = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote.RelacaoPedidoPacote>();

        Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote.RelacaoPedidoPacote dadosCarga = repositorioCarga.ConsultarRelatorioRelacaoPedidoPacote(codigoCarga);

        listaDadosCarga.Add(dadosCarga);
        subReports.Add(ObterSubReportPacotes(codigoCarga));

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = listaDadosCarga,
            Parameters = parametros,
            SubReports = subReports
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\RelacaoPedidoPacoteCarga.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet,
            true
        );

        return PrepareReportResult(FileType.PDF, pdf);
    }

    #region Métodos Privados

    private Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ObterSubReportPacotes(int codigoCarga)
    {
        Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
        Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = repCargaPedidoPacote.BuscarCargaPedidoPacotePorCarga(codigoCarga);
        IList<Dominio.ObjetosDeValor.Embarcador.CTe.NumeroCTeAnteriorPacote> cteTerceiros = repositorioCTeTerceiro.BuscarCTeAnteriorIndentificacaoPacote(cargaPedidoPacotes.Select(o => o.Pacote.Codigo).ToList());
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote.RelacaoPedidoPacoteCargaPacotes> dadosPacotes = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote.RelacaoPedidoPacoteCargaPacotes>();

        cargaPedidoPacotes.ForEach(cargaPedidoPacote =>
        {
            dadosPacotes.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote.RelacaoPedidoPacoteCargaPacotes
            {
                NumeroPacote = !string.IsNullOrWhiteSpace(cargaPedidoPacote.Pacote.LogKey) ? cargaPedidoPacote.Pacote.LogKey : string.Empty,
                NumeroPedido = cargaPedidoPacote.CargaPedido?.Pedido?.Numero ?? 0,
                Destino = cargaPedidoPacote.Pacote.Destino.Descricao,
                Cte = string.Join(" ,", cteTerceiros.Where(cteTerceiro => cteTerceiro.NumeroPacote == cargaPedidoPacote.Pacote.LogKey).Select(cteAnteriorPacote => cteAnteriorPacote.NumeroCTeAnterior).ToList()),
            });
        });

        return new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Key = "RelacaoPedidoPacoteCargaPacotes",
            DataSet = dadosPacotes
        };
    }

    #endregion Métodos Privados
}