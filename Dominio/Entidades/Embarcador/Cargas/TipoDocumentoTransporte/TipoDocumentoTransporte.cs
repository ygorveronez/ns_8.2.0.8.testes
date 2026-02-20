namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DOCUMENTO_TRANSPORTE", EntityName = "TipoDocumentoTransporte", Name = "Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte", NameType = typeof(TipoDocumentoTransporte))]
    public class TipoDocumentoTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TDT_DESCRICAO", TypeType = typeof(string), NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TDT_CODIGO_MOEDA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "TDT_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }

        public virtual bool Equals(TipoDocumentoTransporte other)
        {
            return other.Codigo == this.Codigo ? true : false;
        }
    }
}
