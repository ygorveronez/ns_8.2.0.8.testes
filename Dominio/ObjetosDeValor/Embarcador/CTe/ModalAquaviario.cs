using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalAquaviario
    {
        public string NumeroViagem { get; set; }
        public decimal? ValorPrestacaoAfrmm { get; set; }
        public decimal? ValorAdicionalAfrmm { get; set; }
        public string Direcao { get; set; }
        public int Navio { get; set; }
        public List<ModalAquaviarioBalsa> Balsas { get; set; }
    }
}
