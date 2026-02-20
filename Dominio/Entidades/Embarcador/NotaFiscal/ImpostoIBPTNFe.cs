using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPOSTO_IBPT_NFE", EntityName = "ImpostoIBPTNFe", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe", NameType = typeof(ImpostoIBPTNFe))]
    public class ImpostoIBPTNFe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IBT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "IBT_NCM", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Extensao", Column = "IBT_EXTENSAO", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string Extensao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "IBT_TIPO", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "IBT_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NacionalFederal", Column = "IBT_NACIONAL_FEDERAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal NacionalFederal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportadosFederal", Column = "IBT_IMPORTADOS_FEDERAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ImportadosFederal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Estadual", Column = "IBT_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Estadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Municipal", Column = "IBT_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Municipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VigenciaInicio", Column = "IBT_VIGENCIA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime VigenciaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VigenciaFim", Column = "IBT_VIGENCIA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime VigenciaFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "IBT_VERSAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Fonte", Column = "IBT_FONTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Fonte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual bool Equals(ImpostoIBPTNFe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe Clonar()
        {
            return (Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe)this.MemberwiseClone();
        }
    }
}
