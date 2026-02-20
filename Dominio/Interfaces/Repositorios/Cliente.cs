namespace Dominio.Interfaces.Repositorios
{
    public interface Cliente: Base<Dominio.Entidades.Cliente>
    {
        Dominio.Entidades.Cliente BuscarPorCPFCNPJ(double cpfCnpj); 
    }
}
