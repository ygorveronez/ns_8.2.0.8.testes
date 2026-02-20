using System.Collections.Generic;
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

[UseReportType(ReportType.VolumeFaltante)]
public class VolumeFaltanteReport : ReportBase
{
    public VolumeFaltanteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");
        bool recebimentoVolume = extraData.GetValue<bool>("recebimentoVolume");
        List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> dadosVolumeFaltante = extraData.GetValue<string>("dadosEtiquetaVolume").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante>>();
        
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =  GerarVolumeFaltante(nomeEmpresa, dadosVolumeFaltante);

        if (recebimentoVolume)
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "WMS/RecebimentoMercadoria", _unitOfWork);
        else
            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Cargas/CargaControleExpedicao", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarVolumeFaltante(string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> dadosVolumeFaltante)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);


        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosVolumeFaltante,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\WMS\VolumeFaltante.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return report;
    }
}