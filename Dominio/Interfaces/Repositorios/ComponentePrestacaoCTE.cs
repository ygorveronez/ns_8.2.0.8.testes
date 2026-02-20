using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface ComponentePrestacaoCTE: Base<Dominio.Entidades.ComponentePrestacaoCTE>
    {
        List<Dominio.Entidades.ComponentePrestacaoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe);
        Dominio.Entidades.ComponentePrestacaoCTE BuscarPorCodigoECTe(int codigoCTe, int codigo);
    }
}
