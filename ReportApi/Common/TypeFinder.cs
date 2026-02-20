using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReportApi.Common;

public struct ReportTypeItem
{
    public Type Type { get; set; }
    public UseReportTypeAttribute Attribute { get; set; }
}

public class TypeFinder
{

    private readonly List<ReportTypeItem> _reportTypeItems;

    public TypeFinder()
    {
        _reportTypeItems = GetType().Assembly
                .GetTypes()
                .Where(t => typeof(IReport).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => new ReportTypeItem { Type = t, Attribute = t.GetCustomAttribute<UseReportTypeAttribute>() }).ToList();
    }

    public ReportTypeItem GetRequestByType(ReportType reportType)
    {
        var result = _reportTypeItems.FirstOrDefault(t => t.Attribute != null && t.Attribute.ReportType == reportType);
        return result;
    }
}

