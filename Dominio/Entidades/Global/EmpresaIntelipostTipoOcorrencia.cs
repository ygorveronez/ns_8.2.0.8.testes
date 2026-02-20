namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_INTELIPOST_TIPO_OCORRENCIA", EntityName = "EmpresaIntelipostTipoOcorrencia", Name = "Dominio.Entidades.EmpresaIntelipostTipoOcorrencia", NameType = typeof(EmpresaIntelipostTipoOcorrencia))]
    public class EmpresaIntelipostTipoOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EIO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MicroStatus", Column = "EIO_MICRO_STATUS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MicroStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MacroStatus", Column = "EIO_MACRO_STATUS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MacroStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "EIO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }


    }
}
