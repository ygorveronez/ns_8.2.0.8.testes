namespace Dominio.Interfaces.Repositorios
{
    public interface DocumentosAnulacaoCTE : Base<Dominio.Entidades.DocumentosAnulacaoCTE>
    {
        Dominio.Entidades.DocumentosAnulacaoCTE BuscarPorCTe(int codigoEmpresa, int codigoCTe);
    }
}
