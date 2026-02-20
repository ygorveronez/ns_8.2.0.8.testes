using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_EMISSAO_CTE", EntityName = "PlanoEmissaoCTe", Name = "Dominio.Entidades.PlanoEmissaoCTe", NameType = typeof(PlanoEmissaoCTe))]
    public class PlanoEmissaoCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PEC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoFaixas", Column = "PEC_DESCRICAO_FAIXAS", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string DescricaoFaixas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PEC_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FaixaEmissaoCTe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FAIXA_EMISSAO_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaixaEmissaoCTe", Column = "FEC_CODIGO")]
        public virtual IList<Dominio.Entidades.FaixaEmissaoCTe> FaixaEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ValoresPorDocumentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALORES_POR_DOCUMENTOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValoresPorDocumentos", Column = "VPD_CODIGO")]
        public virtual IList<Dominio.Entidades.ValoresPorDocumentos> ValoresPorDocumentos { get; set; }



        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    default:
                        return "Inativo";
                }
            }
        }
    }
}
