namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaInfracaoAprovacao
    {
        public int CodigoTipoInfracao { get; set; }

        public int CodigoUsuario { get; set; }

        public System.DateTime? DataInicio { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public int Numero { get; set; }

        public Enumeradores.SituacaoInfracao? Situacao { get; set; }
    }
}
