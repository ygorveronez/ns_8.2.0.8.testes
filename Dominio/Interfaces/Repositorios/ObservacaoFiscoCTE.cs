using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface ObservacaoFiscoCTE : Base<Dominio.Entidades.ObservacaoFiscoCTE>
    {
        Dominio.Entidades.ObservacaoFiscoCTE BuscarPorCodigoECTe(int codigo, int codigoCTe);
        List<Dominio.Entidades.ObservacaoFiscoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe);
    }
}
