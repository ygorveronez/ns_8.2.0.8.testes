using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.OCOREN
{
    public class Transportador
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string Filler { get; set; }
        public List<NotaFiscalOcorrencia> NotasFiscais { get; set; }
    }
}
