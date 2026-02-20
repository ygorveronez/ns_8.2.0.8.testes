using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Extensions;

public static class ReportRequestExtensions
{
    public static ReportResult CallReport(this ReportRequest reportRequest, bool retornarFalha = true)
    {
        string serverUrl = Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().UrlReportAPI;

#if DEBUG
        serverUrl = "http://localhost:5000/";
#endif
        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new ServicoException("URL Serviço de Relatórios não configurada.");

        var client = new RestClient
            {
                BaseUrl = new Uri(serverUrl)
            };

        var request = new RestRequest("/report", DataFormat.Json)
            .AddHeader("x-apikey", reportRequest.ApiKey)
            .AddJsonBody(reportRequest);

        var response = client.Post<ReportResult>(request);

        string arquivo = nameof(ReportRequestExtensions);

        Log.TratarErro($"{nameof(response.StatusCode)} - {response.StatusCode}", arquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);
        Log.TratarErro($"{nameof(response.ErrorMessage)} - {response.ErrorMessage?.ToString()}", arquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);
        Log.TratarErro($"{nameof(response.ErrorException)} - {response.ErrorException?.ToString()}", arquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);
        Log.TratarErro($"{nameof(response.Content)} - {response.Content?.ToString()}", arquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);

        ReportResult result = response.Data;

        if (result == null && retornarFalha)
            throw new ServicoException("Falha ao realizar a requisição do Relatório.");

        return result;
    }

    public static async Task<ReportResult> CallReportAsync(
        this ReportRequest reportRequest,
        bool retornarFalha = true,
        CancellationToken cancellationToken = default)
    {
        string serverUrl = Embarcador.Configuracoes.ConfigurationInstance
            .GetInstance()?
            .ObterConfiguracaoAmbiente()
            .UrlReportAPI;

        #if DEBUG
        serverUrl = "http://localhost:5000/";
        #endif

        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new ServicoException("URL Serviço de Relatórios não configurada.");

        var client = new RestClient(serverUrl);

        var request = new RestRequest("/report")
            .AddHeader("x-apikey", reportRequest.ApiKey)
            .AddJsonBody(reportRequest);

        var response = await client.ExecutePostAsync<ReportResult>(request, cancellationToken);

        string arquivo = nameof(ReportRequestExtensions);

        Log.TratarErro($"{nameof(response.StatusCode)} - {response.StatusCode}", arquivo,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);

        Log.TratarErro($"{nameof(response.ErrorMessage)} - {response.ErrorMessage}", arquivo,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);

        Log.TratarErro($"{nameof(response.ErrorException)} - {response.ErrorException}", arquivo,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);

        Log.TratarErro($"{nameof(response.Content)} - {response.Content}", arquivo,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Debug);

        ReportResult result = response.Data;

        if (result == null && retornarFalha)
            throw new ServicoException("Falha ao realizar a requisição do Relatório.");

        return result;
    }
}