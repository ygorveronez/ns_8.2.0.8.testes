namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TIPO_DE_CARGA", EntityName = "RegraTipoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga", NameType = typeof(RegraTipoCarga))]
    public class RegraTipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoasDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RTC_UF_ORIGEM_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UFOrigemDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RTC_UF_ORIGEM", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RTC_UF_DESTINO_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UFDestinoDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RTC_UF_DESTINO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoCarga { get; set; }
    }
}
