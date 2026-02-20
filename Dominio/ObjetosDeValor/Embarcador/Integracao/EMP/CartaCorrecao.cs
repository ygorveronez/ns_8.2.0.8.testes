using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EMP
{
    public class CartaCorrecao
    {
        public string Evento { get; set; }
        public string NumeroBooking { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public List<CartaCorrecaoCampo> CamposCCe { get; set; }
    }
}
