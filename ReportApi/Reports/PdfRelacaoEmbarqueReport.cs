using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.PdfRelacaoEmbarque)]
public class PdfRelacaoEmbarqueReport : ReportBase
{
    public PdfRelacaoEmbarqueReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        
        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

        IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEmbarque.RelacaoEmbarque> dadosRelacaoEmbarque = repCargaPedidoDocumentoCTe.RelatorioRelacaoEmbarque(codigoCarga);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro tomador = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        tomador.NomeParametro = "Tomador";
        tomador.ValorParametro = cargaPedidos[0].ObterTomador().Descricao;
        parametros.Add(tomador);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosRelacaoEmbarque,
            Parameters = parametros,
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\RelacaoEmbarque.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}