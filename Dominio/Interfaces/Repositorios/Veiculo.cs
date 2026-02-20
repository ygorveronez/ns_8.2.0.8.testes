namespace Dominio.Interfaces.Repositorios
{
    public interface Veiculo: Base<Dominio.Entidades.Veiculo>
    {
        Dominio.Entidades.Veiculo BuscarPorCodigo(int codigoEmpresa, int codigoVeiculo);
        Dominio.Entidades.Veiculo BuscarPorPlaca(int codigoEmpresa, string placa);
    }
}
