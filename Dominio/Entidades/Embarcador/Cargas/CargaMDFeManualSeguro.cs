namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_SEGURO", EntityName = "CargaMDFeManualSeguro", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro", NameType = typeof(CargaMDFeManualSeguro))]
    public class CargaMDFeManualSeguro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoResponsavel", Column = "CMS_TIPO_RESPONSAVEL", TypeType = typeof(Dominio.Enumeradores.TipoResponsavelSeguroMDFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoResponsavelSeguroMDFe TipoResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "CMS_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJSeguradora", Column = "CMS_CNPJ_SEGURADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "CMS_NOME_SEGURADORA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "CMS_NUMERO_APOLICE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "CMS_NUMERO_AVERBACAO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string NumeroAverbacao { get; set; }
    }
}
