namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CANCELAMENTO_INTEGRACAO", EntityName = "CargaCancelamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao", NameType = typeof(CargaCancelamentoIntegracao))]
    public class CargaCancelamentoIntegracao : EntidadeBase
    {
        public CargaCancelamentoIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao)this.MemberwiseClone();
        }
    }
}
