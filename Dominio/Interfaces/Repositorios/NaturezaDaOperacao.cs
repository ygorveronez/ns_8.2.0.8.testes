using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface NaturezaDaOperacao: Base<Dominio.Entidades.NaturezaDaOperacao>
    {
        List<Dominio.Entidades.NaturezaDaOperacao> BuscarTodos();
        Dominio.Entidades.NaturezaDaOperacao BuscarPorId(int idNatureza);
        IList<Dominio.Entidades.NaturezaDaOperacao> Consultar(string descricao, int inicioRegistros, int maximoRegistros);
        int ContarConsulta(string descricao);
    }
}
