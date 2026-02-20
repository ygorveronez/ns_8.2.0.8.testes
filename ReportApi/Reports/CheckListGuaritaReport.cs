using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.CheckListGuarita)]
public class CheckListGuaritaReport : ReportBase
{
    public CheckListGuaritaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(_unitOfWork);
        int codigo=extraData.GetValue<int>("codigo");

        // Busca informacoes
        Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList checkList =
            repGuaritaCheckList.BuscarPorCodigo(codigo);

        // Valida
        if (checkList == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListGuarita> dsCheckListGuarita =
            repGuaritaCheckList.RelatorioCheckListGuarita(codigo);
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro tipoServicoMultisoftware =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        tipoServicoMultisoftware.NomeParametro = "TipoServicoMultisoftware";
        tipoServicoMultisoftware.ValorParametro =  extraData.GetInfo().TipoServico.ToString();
        parametros.Add(tipoServicoMultisoftware);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsCheckListGuarita,
                Parameters = parametros
            };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\GestaoPatio\CheckListGuarita.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
        
    }
}