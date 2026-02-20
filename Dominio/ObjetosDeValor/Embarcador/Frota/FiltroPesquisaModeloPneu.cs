namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaModeloPneu
    {
        public int CodigoDimensao { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoMarca { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }
    }
}
