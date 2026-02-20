
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParticipante
    {
        Remetente = 1,
        Destinatario = 2,
        Expedidor = 3,
        Recebedor = 4,
        Tomador = 5
    }

    public static class TipoParticipanteHelper
    {
        public static string ObterDescricao(this TipoParticipante tipo)
        {
            switch (tipo)
            {
                case TipoParticipante.Remetente: return "Remetente";
                case TipoParticipante.Destinatario: return "Destinatário";
                case TipoParticipante.Expedidor: return "Expedidor";
                case TipoParticipante.Recebedor: return "Recebedor";
                case TipoParticipante.Tomador: return "Tomador";
                default: return string.Empty;
            }
        }
    }


}
