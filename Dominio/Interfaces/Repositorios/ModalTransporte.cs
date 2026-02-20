namespace Dominio.Interfaces.Repositorios
{
    public interface ModalTransporte : Base<Dominio.Entidades.ModalTransporte>
    {
        Dominio.Entidades.ModalTransporte BuscarPorNumero(string numeroModal);
    }
}
