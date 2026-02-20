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

[UseReportType(ReportType.RetornoPagamento)]
public class RetornoPagamentoReport : ReportBase
{
    public RetornoPagamentoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno repPagamentoEletronicoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno(_unitOfWork);

        string codigosRetornos = extraData.GetValue<string>("CodigosRetornos");

        IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoPagamento> dsRetornoPagamento = repPagamentoEletronicoRetorno.RelatorioRetornoPagamento(new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento() { CodigosRetornos = codigosRetornos }, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta());

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsRetornoPagamento
        };
        
        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Financeiros\RetornoPagamentoDigital.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}