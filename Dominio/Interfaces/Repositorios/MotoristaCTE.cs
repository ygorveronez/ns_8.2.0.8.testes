using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface MotoristaCTE: Base<Dominio.Entidades.MotoristaCTE>
    {
        Dominio.Entidades.MotoristaCTE BuscarPorCodigoECTe(int codigo, int codigoCTe);
        List<Dominio.Entidades.MotoristaCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe);
    }
}
