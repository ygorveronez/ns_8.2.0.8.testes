namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaLeilaoAprovacao
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoUsuario { get; set; }

        public Enumeradores.SituacaoCargaJanelaCarregamentoCotacao? SituacaoCotacao { get; set; }
    }
}
