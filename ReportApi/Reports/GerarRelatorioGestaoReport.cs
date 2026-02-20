using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.GerarRelatorioGestao)]
public class GerarRelatorioGestaoReport : ReportBase
{
    public GerarRelatorioGestaoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var listaParametros = extraData.GetValue<string>("ListaParametros").FromJson<List<Parametro>>();
        var filtrosPesquisa = extraData.GetValue<string>("FiltrosPesquisa")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga>();
        
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        
        var listaReport = repCarga.ConsultarRelatorioGestaoCargaDetalhado(filtrosPesquisa);
        
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = listaReport,
            Parameters = listaParametros
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\GerarRelatorioGestao.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, possuiLogo: true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}