using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface Estado: Base<Dominio.Entidades.Estado>
    {

        List<Dominio.Entidades.Estado> BuscarTodos();
        Dominio.Entidades.Estado BuscarPorSigla(string sigla);

    }
}
