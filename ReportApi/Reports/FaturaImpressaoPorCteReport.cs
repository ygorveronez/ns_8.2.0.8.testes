using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.FaturaImpressaoPorCte)]
public class FaturaImpressaoPorCteReport : ReportBase
{
    public FaturaImpressaoPorCteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = extraData
            .GetValue<string>("parametros").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var relatorioTemporario = extraData.GetValue<string>("RelatorioTemporario").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura tipoImpressaoFatura =
            extraData.GetValue<string>("tipoImpressaoFatura")
                .FromJson<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura>();

        int codigofatura = extraData.GetValue<int>("codigofatura");
        Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigofatura);


        Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPorCte> listaRelatorioPorCte =
            repositorioFatura.BuscarRelatorioPorCte(fatura);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCampos =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT
            {
                ContadorRegistrosGrupo = "",
                ContadorRegistrosTotal = "",
                CorFundoNomeFormula = ""
            };

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio =
            _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemporario,
                listaRelatorioPorCte, _unitOfWork, identificacaoCampos, null, false);

        _servicoRelatorioReportService.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao,
            relatorioTemporario, parametros, "", "");
        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Faturas/Fatura",
            _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }
}