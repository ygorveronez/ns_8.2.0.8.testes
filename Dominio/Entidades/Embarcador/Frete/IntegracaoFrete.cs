using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_FRETE", EntityName = "IntegracaoFrete", Name = "<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete", NameType = typeof(IntegracaoFrete))]
    public class IntegracaoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoIntegracao", Column = "IFR_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetornoIntegracao", Column = "IFR_CODIGO_RETORNO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoRetornoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "IFR_TIPO", TypeType = typeof(TipoIntegracaoFrete), NotNull = true)]
        public virtual TipoIntegracaoFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoFrete", Column = "IFR_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual IntegracaoFrete IntegracaoFreteOrigem { get; set; }
    }
}
