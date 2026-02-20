namespace Dominio.Interfaces.Repositorios
{
    public interface ConhecimentoDeTransporteEletronico: Base<Dominio.Entidades.ConhecimentoDeTransporteEletronico>
    {
        Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorId(int codigoCTe, int codigoEmpresa);
        int BuscarUltimoNumero(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoAmbiente ambiente, int modeloDocumento = 0);
    }
}
