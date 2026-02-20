
using Dominio.Entidades.Embarcador.Filiais;
using System;

namespace Dominio.Entidades.Embarcador.Produtos
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA_DOCUMENTO", DynamicUpdate = true, EntityName = "OrdemDeCompraDocumento", Name = "Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraDocumento", NameType = typeof(OrdemDeCompraDocumento))]
    public class OrdemDeCompraDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "OCP_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "OCP_TIPO_DOCUMENTO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaDocumento", Column = "OCP_CATEGORIA_DOCUMENTO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CategoriaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusDocumento", Column = "OCP_STATUS_DOCUMENTO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string StatusDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoCancelado", Column = "OCP_DOCUMENTO_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoCancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "OCP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CondicaoPagamento", Column = "OCP_CONDICAO_PAGAMENTO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "OCP_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "OCP_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoCompra", Column = "OCP_GRUPO_COMPRA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string GrupoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TaxaCambio", Column = "OCP_TAXA_CAMBIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TaxaCambio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Moeda", Column = "OCP_MOEDA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorTaxaCambio", Column = "OCP_INDICADOR_TAXA_CAMBIO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string IndicadorTaxaCambio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDocumento", Column = "OCP_DATA_DOCUMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioValidade", Column = "OCP_INICIO_VALIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? InicioValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinValidade", Column = "OCP_FIN_VALIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? FinValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CondicaoDocumento", Column = "OCP_CONDICAO_DOCUMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CondicaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemDeCompraPrincipal", Column = "OPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemDeCompraPrincipal OrdemDeCompraPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeNegocio", Column = "UNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }
    }
}
