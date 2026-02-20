namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaTermoQuitacao
    {
        public int CodigoTransportador { get; set; }

        public System.DateTime? DataBaseInicial { get; set; }

        public System.DateTime? DataBaseLimite { get; set; }

        public int Numero { get; set; }

        public Enumeradores.SituacaoTermoQuitacao? Situacao { get; set; }
    }
}
