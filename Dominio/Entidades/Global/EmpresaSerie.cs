namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_SERIE", EntityName = "EmpresaSerie", Name = "Dominio.Entidades.EmpresaSerie", NameType = typeof(EmpresaSerie))]
    public class EmpresaSerie : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ESE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ESE_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ESE_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ESE_TIPO", TypeType = typeof(Enumeradores.TipoSerie), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoSerie Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProximoNumeroDocumento", Column = "ESE_PROXIMO_NUMERO_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int ProximoNumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ESE_NAO_GERAR_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCargaAutomaticamente { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoSerie.CTe:
                        return "CT-e";
                    case Enumeradores.TipoSerie.MDFe:
                        return "MDF-e";
                    case Enumeradores.TipoSerie.NFSe:
                        return "NFS-e";
                    case Enumeradores.TipoSerie.NFe:
                        return "NF-e";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
