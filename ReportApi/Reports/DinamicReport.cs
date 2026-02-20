using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System.Collections.Generic;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.Dinamic)]
public class DinamicReport : ReportBase
{
    public DinamicReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        bool envioPorArquivo = extraData.GetValue<bool>("ReenvioPorArquivo");
        if (envioPorArquivo)
        {
            string jsonRequest = Utilidades.IO.FileStorageService.Storage.ReadAllText(extraData.GetValue<string>("CaminhoArquivo"));

            extraData = jsonRequest.FromJson<Dictionary<string, string>>();
        }

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("ControleGeracao");
        var relatorioTemporario = extraData.GetValue<string>("RelatorioTemp").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();
        var dataSource = GetDataSource(extraData.GetValue<string>("DataSource"), extraData.GetValue<string>("Type"));
        var parametros = extraData.GetValue<string>("Parametros").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>>();
        var identificacaoCamposRPT = extraData.GetValue<string>("IdentificacaoCamposRPT").FromJson<Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT>();
        var subReportDataSources = extraData.GetValue<string>("SubReportDataSources").FromJson<List<KeyValuePair<string, dynamic>>>();
        var caminhoPagniaRelatorio = extraData.GetValue<string>("CaminhoPagniaRelatorio");
        var ajustarLinhasAutomaticamente = extraData.GetValue<bool>("AjustarLinhasAutomaticamente");
        var caminhoLogo = extraData.GetValue<string>("CaminhoLogo");

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var info = extraData.GetInfo();

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemporario, dataSource, _unitOfWork, identificacaoCamposRPT, subReportDataSources, ajustarLinhasAutomaticamente, info.TipoServico, caminhoLogo);
        _servicoRelatorioReportService.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao, relatorioTemporario, parametros);
        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, caminhoPagniaRelatorio, _unitOfWork);

        return PrepareReportResult(DTO.FileType.PDF, relatorioControleGeracao.GuidArquivo);
    }

    public dynamic GetDataSource(string jsonDataSource, string typeName)
    {
        System.Type type = System.Type.GetType(typeName);

        return Newtonsoft.Json.JsonConvert.DeserializeObject(jsonDataSource, type);
    }
}