namespace Dominio.Entidades.Embarcador.Frete
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_ROTA_TIPO_CARGA", EntityName = "TabelaFreteRotaTipoCarga", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga", NameType = typeof(TabelaFreteRotaTipoCarga))]
    public class TabelaFreteRotaTipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteRota", Column = "TFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteRota TabelaFreteRota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TFR_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }
    }
}
