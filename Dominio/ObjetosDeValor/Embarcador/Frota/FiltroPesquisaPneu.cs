namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaPneu
    {
        public int Codigo { get; set; }

        public int CodigoAlmoxarifado { get; set; }

        public int CodigoBandaRodagem { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoModelo { get; set; }

        public System.DateTime? DataEntradaInicio { get; set; }

        public System.DateTime? DataEntradaLimite { get; set; }

        public string DTO { get; set; }

        public string NumeroFogo { get; set; }

        public Enumeradores.SituacaoPneu? Situacao { get; set; }

        public Enumeradores.TipoAquisicaoPneu? TipoAquisicao { get; set; }

        public Enumeradores.VidaPneu? VidaAtual { get; set; }

        public Enumeradores.SituacaoCadastroPneu? SituacaoCadastroPneu { get; set; }

        public int NumeroNota { get; set; }
    }

}
