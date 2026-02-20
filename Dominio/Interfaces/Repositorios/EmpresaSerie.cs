using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface EmpresaSerie : Base<Dominio.Entidades.Empresa>
    {
        Dominio.Entidades.EmpresaSerie BuscarPorCodigo(int codigo);
        List<Dominio.Entidades.EmpresaSerie> BuscarTodosPorEmpresa(int codigoEmpresa, Dominio.Enumeradores.TipoSerie tipo, string status = "");
        Dominio.Entidades.EmpresaSerie BuscarPorSerie(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoSerie tipo);
        List<Dominio.Entidades.EmpresaSerie> BuscarTodos(int codigoEmpresa, int inicioRegistros, int maximoRegistros);
        int ContarTodos(int codigoEmpresa);
    }
}
