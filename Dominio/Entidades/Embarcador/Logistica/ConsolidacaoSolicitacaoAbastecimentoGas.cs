using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONSOLIDACAO_SOLICITACAO_ABASTECIMENTO_GAS", EntityName = "ConsolidacaoSolicitacaoAbastecimentoGas", Name = "Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas", NameType = typeof(ConsolidacaoSolicitacaoAbastecimentoGas))]
    public class ConsolidacaoSolicitacaoAbastecimentoGas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAbastecimentoGas", Column = "SAG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SolicitacaoAbastecimentoGas SolicitacaoAbastecimentoGas { get; set; }

        [Obsolete("Alterado para ser por Cliente", true)]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial BaseSupridora { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteBaseSupridora { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoDeCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCarga", Column = "CSA_QUANTIDADE_CARGA", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal QuantidadeCarga { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                return $"Consolidação ({SolicitacaoAbastecimentoGas.Descricao})";
            }
        }
    }
}
