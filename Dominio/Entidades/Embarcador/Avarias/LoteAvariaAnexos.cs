namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_AVARIA_ANEXOS", EntityName = "LoteAvariaAnexos", Name = "Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos", NameType = typeof(LoteAvariaAnexos))]
    public class LoteAvariaAnexos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Lote", Column = "LAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.Lote Lote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }


        public virtual bool Equals(LoteAvariaAnexos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
