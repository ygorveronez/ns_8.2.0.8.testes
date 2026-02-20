namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_REAJUSTE_FRETE_TIPO_OPERACAO", EntityName = "AlcadaReajusteFreteTipoOperacao", Name = "Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao", NameType = typeof(AlcadaReajusteFreteTipoOperacao))]
    public class AlcadaReajusteFreteTipoOperacao : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ART_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraControleReajusteFretePlanilha", Column = "RRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraControleReajusteFretePlanilha RegraControleReajusteFretePlanilha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TipoOperacao?.Descricao ?? string.Empty;
            }
        }

        public virtual Pedidos.TipoOperacao PropriedadeAlcada
        {
            get
            {
                return this.TipoOperacao;
            }
            set
            {
                this.TipoOperacao = value;
            }
        }
    }
}