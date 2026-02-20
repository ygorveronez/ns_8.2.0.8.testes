using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;

namespace ReportApi.Interfaces
{
    public interface IReport
    {
        ReportResult Process(Dictionary<string, string> extraData);
    }
}