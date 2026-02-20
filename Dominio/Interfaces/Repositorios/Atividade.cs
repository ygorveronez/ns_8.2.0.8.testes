using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface Atividade: Base<Dominio.Entidades.Atividade>
    {
        IList<Dominio.Entidades.Atividade> Consulta(string descricao, int inicioRegistros, int maximoRegistros);
        int ContarConsulta(string descricao);
        Dominio.Entidades.Atividade BuscarPorCodigo(int codigo);
    }
}
