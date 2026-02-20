using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.DataSource.WMS;
using Newtonsoft.Json;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.GerarEtiquetasVolume)]
public class GerarEtiquetasVolumeReport : ReportBase
{
    public GerarEtiquetasVolumeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        #region Parametros
        int codigo = extraData.GetValue<int>("codigo");
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");


        List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster> dadosEtiquetaVolume  = extraData.GetValue<string>("dadosEtiquetaVolume").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster>>();
        string nomeEtiqueta = extraData.GetValue<string>("nomeEtiqueta");

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);
        #endregion

        #region MetodosInternos
        var report = GerarEtiquetasVolume(codigo, _unitOfWork, nomeEmpresa, dadosEtiquetaVolume, nomeEtiqueta);
        #endregion

        #region MetodosExternos
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "WMS/RecebimentoMercadoria", _unitOfWork);
        #endregion

        return PrepareReportResult(FileType.PDF);
    }
    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarEtiquetasVolume(int codigo, Repositorio.UnitOfWork unidadeTrabalho, string nomeEmpresa, List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster> dadosEtiquetaVolume, string nomeEtiqueta)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dadosEtiquetaVolume,
            Parameters = parametros
        };
        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\WMS\" + nomeEtiqueta, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return report;
    }

}