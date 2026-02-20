namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_MOTORISTA", EntityName = "MotoristaMDFe", Name = "Dominio.Entidades.MotoristaMDFe", NameType = typeof(MotoristaMDFe))]
    public class MotoristaMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "MDM_CPF", TypeType = typeof(string), Length = 11, NotNull = true)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "MDM_NOME", TypeType = typeof(string), Length = 60, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MDM_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoMotoristaMDFe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoMotoristaMDFe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloEventoInclusao", Column = "MDM_PROTOCOLO_EVENTO_INCLUSAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloEventoInclusao { get; set; }
    }
}
