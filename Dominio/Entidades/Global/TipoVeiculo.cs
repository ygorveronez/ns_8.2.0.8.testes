using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_TIPO", EntityName = "TipoVeiculo", Name = "Dominio.Entidades.TipoVeiculo", NameType = typeof(TipoVeiculo))]
    public class TipoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VTI_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "VTI_PESO_LIQUIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "VTI_PESO_BRUTO", TypeType = typeof(int), NotNull = false)]
        public virtual int PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEixos", Column = "VTI_NUMERO_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VTI_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "VTI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EixosDoVeiculo", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_VEICULO_EIXO_TIPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VTI_CODIGO", ForeignKey = "VTI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EixoVeiculo", ForeignKey = "VTE_CODIGO", Column = "VTE_CODIGO", NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual IList<Dominio.Entidades.EixoVeiculo> EixosDoVeiculo { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }
    }
}
