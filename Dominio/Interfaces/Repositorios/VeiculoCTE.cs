using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface VeiculoCTE: Base<Dominio.Entidades.VeiculoCTE>
    {
        Dominio.Entidades.VeiculoCTE BuscarPorCodigoECTe(int codigo, int codigoCTe);
        List<Dominio.Entidades.VeiculoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe, string tipoVeiculo = "");
    }
}
