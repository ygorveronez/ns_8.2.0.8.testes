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

[UseReportType(ReportType.RelacaoSeparacaoVolume)]
public class RelacaoSeparacaoVolumeReport : ReportBase
{
    public RelacaoSeparacaoVolumeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosRelacaoSeparacaoVolume = extraData.GetValue<string>("DadosRelacaoSeparacaoVolume")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioRelacaoSeparacaoVolume(nomeEmpresa, dadosRelacaoSeparacaoVolume);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Cargas/Carga", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioRelacaoSeparacaoVolume(
        string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume>
            dadosRelacaoSeparacaoVolume)
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
                DataSet = dadosRelacaoSeparacaoVolume,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Pedidos\RelacaoSeparacaoVolume.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}