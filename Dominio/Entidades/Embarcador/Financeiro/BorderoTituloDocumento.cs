namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BORDERO_TITULO_DOCUMENTO", EntityName = "BorderoTituloDocumento", Name = "Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento", NameType = typeof(BorderoTituloDocumento))]
    public class BorderoTituloDocumento: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BorderoTitulo", Column = "BOT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BorderoTitulo BorderoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloDocumento", Column = "TDO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TituloDocumento TituloDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BTD_VALOR_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorACobrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BTD_VALOR_TOTAL_ACRESCIMO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BTD_VALOR_TOTAL_DESCONTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalDesconto { get; set; }

        /// <summary>
        /// Valor a Cobrar + Valor Total Acréscimo - Valor Total Desconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "BTD_VALOR_TOTAL_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalACobrar { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TituloDocumento?.Descricao ?? string.Empty;
            }
        }
    }
}
