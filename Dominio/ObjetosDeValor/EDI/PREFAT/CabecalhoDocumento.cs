using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class CabecalhoDocumento
    {
        public string IdentificacaoDocumento { get; set; }
        public string Filler { get; set; }
        public List<EmpresaPagadora> EmpresasPagadoras { get; set; }
    }
}
