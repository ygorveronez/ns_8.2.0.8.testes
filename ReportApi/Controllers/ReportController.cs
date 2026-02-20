using System;
using System.Web.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReportApi.Common;
using ReportApi.Common.Authorization;
using ReportApi.Interfaces;
using Utilidades.Extensions;

namespace ReportApi.Controllers;

public class ReportController : Controller
{

    private readonly IServiceProvider _serviceProvider;
    private readonly TypeFinder _typeFinder;

    public ReportController(IServiceProvider serviceProvider, TypeFinder typeFinder)
    {
        _serviceProvider = serviceProvider;
        _typeFinder = typeFinder;
    }


    [System.Web.Http.HttpPost]
    [Route("report")]
    [ApiKeyAuthorizationRequirement]
    public ActionResult Process(ReportRequest reportRequest)
    {
        string identificadorChamada = Guid.NewGuid().ToString();
        try
        {
            Servicos.Log.TratarErro($"Identificador {identificadorChamada}: {reportRequest.ToJson(new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }, Formatting.Indented)}", "Requests-Responses");

            var report = GetRequestByType(reportRequest.ReportType);
            ReportResult retorno = report.Process(reportRequest.ExtraData);

            Servicos.Log.TratarErro($"Identificador {identificadorChamada}: {retorno.ToJson(new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }, Formatting.Indented)}", "Requests-Responses");

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            Servicos.Log.TratarErro(ex);

            var errorResult = new ReportResult
            {
                Id = Guid.NewGuid(),
                ErrorMessage = ex.Message,
                ExecutionStatus = ExecutionStatus.Fail
            };

            Servicos.Log.TratarErro($"Identificador {identificadorChamada}: {errorResult.ToJson(new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }, Formatting.Indented)}", "Requests-Responses");

            return Json(errorResult, JsonRequestBehavior.AllowGet);
        }

    }

    public IReport GetRequestByType(ReportType reportType)
    {
        var reportTypeItem = _typeFinder.GetRequestByType(reportType);
        return (IReport)_serviceProvider.GetRequiredService(reportTypeItem.Type);
    }



}