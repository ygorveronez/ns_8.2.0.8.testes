namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TIPO_ANEXO", EntityName = "TipoAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.TipoAnexo", NameType = typeof(TipoAnexo))]
    public class TipoAnexo : EntidadeBase 
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_DESCRICAO", TypeType = typeof(string),Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        public virtual string DescricaoAtivo { get { return this.Status ? "Ativo" : "Inativo"; } }
    }
}
