using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface DocumentosCTE: Base<Dominio.Entidades.DocumentosCTE>
    {
        List<Dominio.Entidades.DocumentosCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe);
        Dominio.Entidades.DocumentosCTE BuscarPorCTeENumero(int codigoCTe, string numero);
    }
}
