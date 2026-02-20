using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface ObservacaoContribuinteCTE : Base<Dominio.Entidades.ObservacaoContribuinteCTE>
    {
        Dominio.Entidades.ObservacaoContribuinteCTE BuscarPorCodigoECTe(int codigo, int codigoCTe);
        List<Dominio.Entidades.ObservacaoContribuinteCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe);
    }
}
