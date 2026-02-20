namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BORDERO_TITULO", EntityName = "BorderoTitulo", Name = "Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo", NameType = typeof(BorderoTitulo))]
    public class BorderoTitulo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bordero", Column = "BOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bordero Bordero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOT_VALOR_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorACobrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOT_VALOR_TOTAL_ACRESCIMO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BOT_VALOR_TOTAL_DESCONTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalDesconto { get; set; }

        /// <summary>
        /// Valor a Cobrar + Valor Total Acréscimo - Valor Total Desconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "BOT_VALOR_TOTAL_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalACobrar { get; set; }
        public virtual string Descricao
        {
            get
            {
                return this.Titulo?.Descricao ?? string.Empty;
            }
        }
    }
}
