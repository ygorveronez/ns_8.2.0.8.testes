using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParticipantesSumarizadosCalculoFrete : IEquatable<ParticipantesSumarizadosCalculoFrete>
    {
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? Modalidade { get; set; }

        public bool Equals(ParticipantesSumarizadosCalculoFrete other)
        {
            if(((Remetente == null && other.Remetente == null) || (Remetente != null && Remetente.Equals(other.Remetente))) &&
               ((Destinatario == null && other.Destinatario == null) || (Destinatario != null && Destinatario.Equals(other.Destinatario))) &&
               Modalidade == other.Modalidade)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashRemetente = Remetente == null ? 0 : Remetente.GetHashCode();
            int hashDestinatario = Destinatario == null ? 0 : Destinatario.GetHashCode();
            int hashModalidade = Modalidade == null ? 0 : Modalidade.GetHashCode();

            return hashRemetente ^ hashDestinatario ^ hashModalidade;
        }
    }
}
