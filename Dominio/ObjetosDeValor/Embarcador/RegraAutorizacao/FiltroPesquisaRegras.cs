namespace Dominio.ObjetosDeValor.Embarcador.RegraAutorizacao
{
    public sealed class FiltroPesquisaRegraAutorizacaoPadrao
    {
        public Entidades.Usuario Aprovador { get; set; }

        public System.DateTime? DataInicial { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
    }
}
