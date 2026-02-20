namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_ROTA_TIPO_CARGA_MODELO_VEICULAR_CARGA", EntityName = "TabelaFreteRotaTipoCargaModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga", NameType = typeof(TabelaFreteRotaTipoCargaModeloVeicularCarga))]
    public class TabelaFreteRotaTipoCargaModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteRotaTipoCarga", Column = "TTC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga TabelaFreteRotaTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "TTM_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "TTM_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPedagio { get; set; }

        //todo: Remover essa coluna, adicinada apenas para registrar a importação sem icms na Tirol
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteComICMS", Column = "TTM_VALOR_FRETE_COM_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteComICMS { get; set; }

    }
}
