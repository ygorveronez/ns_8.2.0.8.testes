using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface SeguroCTE: Base<Dominio.Entidades.SeguroCTE>
    {
        List<Dominio.Entidades.SeguroCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe);
        Dominio.Entidades.SeguroCTE BuscarPorCTeECodigo(int codigoCTe, int codigo);
    }
}
