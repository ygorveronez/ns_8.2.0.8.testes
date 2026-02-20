namespace Dominio.Interfaces.Repositorios
{
    public interface Setor: Base<Dominio.Entidades.Setor>
    {
        Dominio.Entidades.Setor BuscarPorCodigo(int codigo);
    }
}
