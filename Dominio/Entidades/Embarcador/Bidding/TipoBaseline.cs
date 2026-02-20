namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_BASELINE", EntityName = "TipoBaseline", Name = "Dominio.Entidades.Embarcador.Bidding.TipoBaseline", NameType = typeof(TipoBaseline))]
    public class TipoBaseline : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TBS_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TBS_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "TBS_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }

        public virtual string DescricaoStatus
        {
            get { return Status ? "Ativo" : "Inativo"; }
        }
    }
}
