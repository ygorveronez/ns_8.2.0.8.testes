using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface ModeloDocumentoFiscal: Base<Dominio.Entidades.ModeloDocumentoFiscal>
    {
        List<Dominio.Entidades.ModeloDocumentoFiscal> BuscarTodos();
        Dominio.Entidades.ModeloDocumentoFiscal BuscarPorId(int codigo);
    }
}
