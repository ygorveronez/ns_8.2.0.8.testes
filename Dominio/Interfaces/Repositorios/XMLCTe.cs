namespace Dominio.Interfaces.Repositorios
{
    public interface XMLCTe: Base<Dominio.Entidades.XMLCTe>
    {
        Dominio.Entidades.XMLCTe BuscarPorCTe(int codigoCTe, Dominio.Enumeradores.TipoXMLCTe tipo);
    }
}
