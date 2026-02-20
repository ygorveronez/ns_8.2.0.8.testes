using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_VIGENCIA", EntityName = "VigenciaTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete", NameType = typeof(VigenciaTabelaFrete))]
    public class VigenciaTabelaFrete : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "TFV_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "TFV_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_VIGENCIA_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VigenciaTabelaFreteAnexo", Column = "ANX_CODIGO")]
        public virtual IList<VigenciaTabelaFreteAnexo> Anexos { get; set; }

        public virtual string Descricao
        {
            get { return $"De {DataInicial.ToString("dd/MM/yyyy")}{ (DataFinal.HasValue ? $" até {DataFinal.Value.ToString("dd/MM/yyyy")}" : "")}"; }
        }
    }
}
