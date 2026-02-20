using System;
using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface XMLNotaFiscalEletronica : Base<Dominio.Entidades.XMLNotaFiscalEletronica>
    {
        IList<Dominio.Entidades.XMLNotaFiscalEletronica> Consultar(int codigoEmpresa, string numeroNota, double cnpjEmitente, DateTime dataEmissao, int inicioRegistros, int maximoRegistros);
        int ContarConsulta(int codigoEmpresa, string numeroNota, double cnpjEmitente, DateTime dataEmissao);
    }
}
