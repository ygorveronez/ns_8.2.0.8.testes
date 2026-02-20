namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO", EntityName = "ApoliceSeguroAverbacao", Name = "Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao", NameType = typeof(ApoliceSeguroAverbacao))]
    public class ApoliceSeguroAverbacao : EntidadeBase
    {
        public ApoliceSeguroAverbacao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APS_SEGURO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SeguroFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_DESCONTO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? Desconto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

        public virtual ApoliceSeguroAverbacao Clonar()
        {
            return (ApoliceSeguroAverbacao)this.MemberwiseClone();
        }
    }
}
