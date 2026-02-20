namespace Dominio.Entidades.Embarcador.CRM
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROSPECCAO_ANEXO", EntityName = "ProspeccaoAnexo", Name = "Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo", NameType = typeof(ProspeccaoAnexo))]
    public class ProspeccaoAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Prospeccao", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CRM.Prospeccao Prospeccao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }


        public virtual bool Equals(ProspeccaoAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
