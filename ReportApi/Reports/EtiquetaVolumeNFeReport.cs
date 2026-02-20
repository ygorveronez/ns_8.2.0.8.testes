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

[UseReportType(ReportType.EtiquetaVolumeNFe)]
public class EtiquetaVolumeNFeReport : ReportBase
{
    public EtiquetaVolumeNFeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigo = extraData.GetValue<int>("codigo");
        string nomeEmpresa = extraData.GetValue<string>("nomeEmpresa");
        List<Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe> dadosEtiquetaVolume =
            extraData.GetValue<string>("dadosEtiquetaVolume")
                .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe>>();
        string caminhoLogo = extraData.GetValue<string>("caminhoLogo");

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);


        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarEtiquetasVolume(codigo, nomeEmpresa, dadosEtiquetaVolume, caminhoLogo);
        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "NotasFiscais/NotaFiscalEletronica", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarEtiquetasVolume(int codigo, string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.NFe.EtiquetaNFe> dadosEtiquetaVolume, string caminhoLogo)
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
                DataSet = dadosEtiquetaVolume,
                Parameters = parametros
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\NFe\EtiquetaVolumeNFe.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true, caminhoLogo);
        return report;
    }
}