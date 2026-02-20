namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APOLICE_SEGURO_DESCONTO", EntityName = "ApoliceSeguroDesconto", Name = "Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto", NameType = typeof(ApoliceSeguroDesconto))]
    public class ApoliceSeguroDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "APD_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "APD_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
