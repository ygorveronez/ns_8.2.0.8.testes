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
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.PlanejamentoPedido)]
public class PlanejamentoPedidoReport:ReportBase
{
    public PlanejamentoPedidoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {        
        List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido> dataSourcePlanejamentoPedido = extraData.GetValue<string>("dataSet").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido>>();
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dataSourcePlanejamentoPedido,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };        

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Pedidos\PlanejamentoPedido.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, false);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }   
}