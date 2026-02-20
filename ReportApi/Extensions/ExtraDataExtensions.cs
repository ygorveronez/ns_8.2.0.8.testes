using System.Collections.Generic;
using ReportApi.DTO;

namespace ReportApi.Extensions;

public static class ExtraDataExtensions
{
    public static RequestInformation GetInfo(this Dictionary<string, string> dictionary)
    {
        //int.TryParse(dictionary["EmpresaCodigo"], out int empresaCodigo);
        //int.TryParse(dictionary["Usuario"], out int usuario);
        //Enum.TryParse(dictionary["TipoServicoMultisoftware"], out TipoServicoMultisoftware tipoServicoMultisoftware);
        int empresaCodigo = 17048;
        int usuario = 1;
        return new RequestInformation
        {
            CodigoEmpresa = empresaCodigo,
            CodigoUsuario = usuario,
            TipoServico = ConnectionFactory.TipoServicoMultisoftware
        };
    }
}