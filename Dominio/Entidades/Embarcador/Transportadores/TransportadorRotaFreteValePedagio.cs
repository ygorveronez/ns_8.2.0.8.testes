namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_ROTA_FRETE_VALE_PEDAGIO", EntityName = "TransportadorRotaFreteValePedagio", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio", NameType = typeof(TransportadorRotaFreteValePedagio))]
    public class TransportadorRotaFreteValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ERF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRotaFrete", Column = "ERF_TIPO_ROTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete TipoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
    }
}
