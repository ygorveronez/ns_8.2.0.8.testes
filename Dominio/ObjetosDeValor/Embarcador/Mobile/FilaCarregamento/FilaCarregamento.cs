
namespace Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento
{
    public sealed class FilaCarregamento
    {
        public int Codigo { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public string DescricaoCentroCarregamento { get; set; }
        
        public string Doca { get; set; }

        public int Posicao { get; set; }

        public Enumeradores.SituacaoFilaCarregamento Situacao { get; set; }

        public Enumeradores.SituacaoMotoristaFilaCarregamento SituacaoMotorista { get; set; }

        public Enumeradores.TipoFilaCarregamento Tipo { get; set; }
    }
}
