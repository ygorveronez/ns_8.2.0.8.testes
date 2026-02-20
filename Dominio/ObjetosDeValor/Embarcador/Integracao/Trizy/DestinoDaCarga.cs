using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class DestinoDaCarga
    {
        public string operation { get; set; }
        public string expectedAt { get; set; }
        public PontoCoordenadas point { get; set; }
        public Cliente client { get; set; }
        public Eventos events { get; set; }
        public List<Evidencia> evidences { get; set; }
        public List<Documento> documents { get; set; }
        public List<Produto> products { get; set; }
    }
}
