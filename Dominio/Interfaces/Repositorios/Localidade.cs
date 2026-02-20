using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface Localidade: Base<Dominio.Entidades.Localidade>
    {
        List<Dominio.Entidades.Localidade> Consulta(string cidade, int codigoIBGE, int inicioRegistros, int maximoRegistros, string propOrdena = "Descricao", string dirOrdena = "asc", string uf = "");
        int ContarConsulta(string cidade, int codigoIBGE, string uf = "");
        Dominio.Entidades.Localidade BuscarPorCodigo(int codigo);
        List<Dominio.Entidades.Localidade> BuscarPorUF(string uf);
    }
}
