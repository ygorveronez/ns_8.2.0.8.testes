namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_CODIGO_INTEGRACAO", EntityName = "TipoDeCargaCodigoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao", NameType = typeof(TipoDeCargaCodigoIntegracao))]
    public class TipoDeCargaCodigoIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TCI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaCarga", Column = "TCI_ETAPA_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga EtapaCarga { get; set; }
    }
}