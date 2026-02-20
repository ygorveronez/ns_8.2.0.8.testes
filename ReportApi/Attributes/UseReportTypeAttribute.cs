using System;
using Dominio.ObjetosDeValor.Relatorios;

namespace ReportApi.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]

public class UseReportTypeAttribute : Attribute
{
    public ReportType ReportType { get; }
    public UseReportTypeAttribute(ReportType reportType)
    {
        ReportType = reportType;
    }
}