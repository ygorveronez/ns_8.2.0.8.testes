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

[UseReportType(ReportType.AuditoriaDeOs)]
public class AuditoriaDeOsReport : ReportBase
{
    public AuditoriaDeOsReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(
        unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico> listaReport = extraData.GetValue<string>("ListaReport").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico>>();        
        
        List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico> listaServicos = extraData.GetValue<string>("ListaServicos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico>>();

        List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto> listaProdutos = extraData.GetValue<string>("ListaProdutos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto>>();

        if (listaProdutos.Count == 0)
        {
            Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto item = new Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto()
            {
                Codigo =  0,
                CodigoOrdemServico = 0
            };
            listaProdutos.Add(item);
        }        

        List<Parametro> listaParametros = extraData.GetValue<string>("ListaParametros").FromJson<List<Parametro>>();            

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AuditoriaDeOsServicos.rpt",
                DataSet = listaServicos
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "AuditoriaDeOsProdutos.rpt",
                DataSet = listaProdutos
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = listaReport,
                Parameters = listaParametros,
                SubReports = subReports
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Frotas\AuditoriaDeOs.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, possuiLogo: true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}