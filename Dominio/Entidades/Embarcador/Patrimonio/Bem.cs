using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM", EntityName = "Bem", Name = "Dominio.Entidades.Embarcador.Patrimonio.Bem", NameType = typeof(Bem))]
    public class Bem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.Bem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BEM_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoNota", Column = "BEM_DESCRICAO_NOTA", TypeType = typeof(string), Length = 120, NotNull = false)]
        public virtual string DescricaoNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSerie", Column = "BEM_NUMERO_SERIE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NumeroSerie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAquisicao", Column = "BEM_DATA_AQUISICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAquisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimGarantia", Column = "BEM_DATA_FIM_GARANTIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimGarantia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "BEM_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBem", Column = "BEM_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorBem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualResidual", Column = "BEM_PERCENTUAL_RESIDUAL", TypeType = typeof(decimal), Scale = 2, Precision = 7, NotNull = false)]
        public virtual decimal PercentualResidual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorResidual", Column = "BEM_VALOR_RESIDUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorResidual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDepreciar", Column = "BEM_VALOR_DEPRECIAR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDepreciar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DepreciacaoAcumulada", Column = "BEM_DEPRECIACAO_ACUMULADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal DepreciacaoAcumulada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImplantacao", Column = "BEM_DATA_IMPLANTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataImplantacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntradaTransferencia", Column = "BEM_DATA_ENTRADA_TRANSFERENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntradaTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaixa", Column = "BEM_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDepreciacao", Column = "BEM_PERCENTUAL_DEPRECIACAO", TypeType = typeof(decimal), Scale = 2, Precision = 7, NotNull = false)]
        public virtual decimal PercentualDepreciacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VidaUtil", Column = "BEM_VIDA_UTIL", TypeType = typeof(int), NotNull = false)]
        public virtual int VidaUtil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VidaUtilTaxaDepreciacao", Column = "BEM_VIDA_UTIL_TAXA_DEPRECIACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal VidaUtilTaxaDepreciacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProdutoTMS", Column = "GPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.GrupoProdutoTMS GrupoProdutoTMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Almoxarifado", Column = "AMX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.Almoxarifado Almoxarifado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaItem", Column = "TDI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.DocumentoEntradaItem DocumentoEntradaItem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlocado", Column = "BEM_DATA_ALOCADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlocado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ALOCADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioAlocado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BEM_COMPONENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BEM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BemComponente", Column = "BEC_CODIGO")]
        public virtual IList<BemComponente> Componentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BEM_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BEM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BemAnexo", Column = "BEA_CODIGO")]
        public virtual IList<BemAnexo> Anexos { get; set; }

        public virtual bool Equals(Bem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
