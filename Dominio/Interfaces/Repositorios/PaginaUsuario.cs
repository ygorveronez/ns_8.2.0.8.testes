using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface PaginaUsuario : Base<Dominio.Entidades.PaginaUsuario>
    {
        Dominio.Entidades.PaginaUsuario BuscarPorPaginaEUsuario(int codigoPagina, int codigoUsuario);
        List<Dominio.Entidades.PaginaUsuario> BuscarPorUsuario(int codigoUsuario);
    }
}
