using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface CFOP: Base<Dominio.Entidades.CFOP>
    {
        List<Dominio.Entidades.CFOP> BuscarPorNaturezaDaOperacao(int idNaturezaOperacao);
        Dominio.Entidades.CFOP BuscarPorId(int idCFOP);
    }
}
