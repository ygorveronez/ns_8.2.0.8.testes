namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaBandaRodagemPneu
    {
        public int CodigoEmpresa { get; set; }

        public int CodigoMarca { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoBandaRodagemPneu? Tipo { get; set; }
    }
}
