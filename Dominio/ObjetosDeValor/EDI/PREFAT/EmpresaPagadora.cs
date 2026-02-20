using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class EmpresaPagadora
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string Filler { get; set; }
        public List<PreFatura> PreFaturas { get; set; }
    }
}
