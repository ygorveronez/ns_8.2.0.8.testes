namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PONTUACAO_POR_TIPO_OPERACAO", EntityName = "PontuacaoPorTipoOperacao", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao", NameType = typeof(PontuacaoPorTipoOperacao))]
    public class PontuacaoPorTipoOperacao : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "PTA_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        public override string Descricao
        {
            get { return $"Tipo de operação {this.TipoOperacao.Descricao}"; }
        }
    }
}
