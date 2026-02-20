namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_REGISTRO_ENCERRAMENTO_INTEGRACAO", EntityName = "CargaRegistroEncerramentoIntegracao ", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao ", NameType = typeof(CargaRegistroEncerramentoIntegracao))]
    public class CargaRegistroEncerramentoIntegracao : EntidadeBase
    {
        public CargaRegistroEncerramentoIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaRegistroEncerramento", Column = "CRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento CargaRegistroEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao)this.MemberwiseClone();
        }
    }
}
