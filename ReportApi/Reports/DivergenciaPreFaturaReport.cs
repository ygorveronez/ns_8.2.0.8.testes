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

[UseReportType(ReportType.DivergenciaPreFatura)]
public class DivergenciaPreFaturaReport : ReportBase
{
    public DivergenciaPreFaturaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int codigoFatura = extraData.GetValue<int>("CodigoFatura");
        string nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosPreFatura = extraData.GetValue<string>("DadosPreFatura")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report =
            GerarRelatorioDivergenciaPreFatura(codigoFatura, nomeEmpresa, dadosPreFatura);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Fatura/Fatura", _unitOfWork);

        return PrepareReportResult(FileType.EXCEL);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioDivergenciaPreFatura(int codigoFatura,
        string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioDivergenciaPreFatura> dadosPreFatura)
    {
        Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroMensagemBC =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroMensagemBC.NomeParametro = "NumeroFatura";
        parametroMensagemBC.ValorParametro = fatura.Numero.ToString();
        parametros.Add(parametroMensagemBC);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametroLocalidade =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        parametroLocalidade.NomeParametro = "PessoaGrupo";
        parametroLocalidade.ValorParametro = fatura.GrupoPessoas != null ? fatura.GrupoPessoas.Descricao :
            fatura.Cliente != null ? fatura.Cliente.Nome : "";
        parametros.Add(parametroLocalidade);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosPreFatura,
                Parameters = parametros
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Fatura\DivergenciaPreFatura.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.XLS, dataSet, true);
    }
}