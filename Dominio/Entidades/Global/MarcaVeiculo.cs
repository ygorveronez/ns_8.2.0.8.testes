using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MARCA", EntityName = "MarcaVeiculo", Name = "Dominio.Entidades.MarcaVeiculo", NameType = typeof(MarcaVeiculo))]
    public class MarcaVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "VMA_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VMA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VMA_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "VMA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo? TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Modelos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_MODELO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeiculo", Column = "VMO_CODIGO")]
        public virtual IList<ModeloVeiculo> Modelos { get; set; }

        public virtual string DescricaoTipoVeiculo
        {
            get
            {
                switch (this.TipoVeiculo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque:
                        return "Reboque";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao:
                        return "Tração";
                    default:
                        return "";
                }
            }
        }

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
