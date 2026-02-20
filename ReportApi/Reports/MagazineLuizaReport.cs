using System;
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

[UseReportType(ReportType.MagazineLuiza)]
public class MagazineLuizaReport : ReportBase
{
    public MagazineLuizaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        DateTime dataInicial = extraData.GetValue<DateTime>("DataInicial");
        DateTime dataFinal = extraData.GetValue<DateTime>("DataFinal");
        
        Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe> ctes = repCTe.ConsultarRelatorioCTesMagazineLuiza(dataInicial, dataFinal);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataset = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = ctes
        };

        byte[] relatorio = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\CTe\MagazineLuiza.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.XLS, dataset);

        return PrepareReportResult(FileType.EXCEL, relatorio);
    }
}