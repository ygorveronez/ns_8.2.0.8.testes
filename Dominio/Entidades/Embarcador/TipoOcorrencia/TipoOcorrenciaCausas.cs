
namespace Dominio.Entidades.Embarcador.TipoOcorrencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OCORRENCIA_CAUSAS", EntityName = "TipoOcorrenciaCausas", Name = "Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas", NameType = typeof(TipoOcorrenciaCausas))]
    public class TipoOcorrenciaCausas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTOC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTOC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }
    }    
}
