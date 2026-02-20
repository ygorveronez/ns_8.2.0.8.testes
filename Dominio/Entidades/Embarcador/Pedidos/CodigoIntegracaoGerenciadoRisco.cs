namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CODIGO_INTEGRACAO_GERENCIADORA_RISCO", EntityName = "CodigoIntegracaoGerenciadoraRisco", Name = "Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco", NameType = typeof(CodigoIntegracaoGerenciadoraRisco))]
    public class CodigoIntegracaoGerenciadoraRisco : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CGR_CODIGO_INTEGRACAO", TypeType = typeof(string), NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaCarga", Column = "CGR_ETAPA_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga EtapaCarga { get; set; }
    }
}