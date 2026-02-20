using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface InformacaoCargaCTE: Base<Dominio.Entidades.InformacaoCargaCTE>
    {
        List<Dominio.Entidades.InformacaoCargaCTE> BuscarPorCTe(int codigoEmpresa, int CodigoCTe);
        Dominio.Entidades.InformacaoCargaCTE BuscarPorCTeECodigo(int codigoCTe, int codigo);
    }
}
