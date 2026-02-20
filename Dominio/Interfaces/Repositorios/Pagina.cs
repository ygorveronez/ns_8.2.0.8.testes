namespace Dominio.Interfaces.Repositorios
{
    public interface Pagina : Base<Dominio.Entidades.Pagina>
    {
        Dominio.Entidades.Pagina BuscarPorCodigo(int codigo);
    }
}
