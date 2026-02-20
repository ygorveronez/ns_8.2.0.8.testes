using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.RFI
{
    public class RFIConvidado
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public StatusRFIConviteConvidado Status { get; set; }
    }
}
