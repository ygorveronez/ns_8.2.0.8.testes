using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_ENTRADA_DOCUMENTO", EntityName = "RegraEntradaDocumento", Name = "Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento", NameType = typeof(RegraEntradaDocumento))]
    public class RegraEntradaDocumento : EntidadeBase, IEquatable<RegraEntradaDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RED_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RED_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_DENTRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPDentro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_FORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarInformarVeiculo", Column = "RED_OBRIGA_INFORMAR_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarFaturarNotaAutomaticamente", Column = "RED_FINALIZAR_FATURAR_NOTA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarFaturarNotaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoTributarICMS", Column = "RED_TRIBUTAR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoTributarICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MultiplosFornecedores", Column = "RED_MULTIPLOS_FORNECEDORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplosFornecedores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorPagamento", Column = "RED_INDICADOR_PAGAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada IndicadorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento", Column = "RED_NAO_FINALIZAR_QUANDO_ARLA_ESTIVER_ASSOCIADA_REBOQUE_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoFinalizarQuandoArlaTiverQuantidadeSuperior", Column = "RED_NAO_FINALIZAR_QUANDO_ARLA_TIVER_QUANTIDADE_SUPERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarQuandoArlaTiverQuantidadeSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoFinalizarDocumentoSemProdutoPreCadastrado", Column = "RED_NAO_FINALIZAR_DOCUMENTO_SEM_PRODUTO_PRE_CADASTRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarDocumentoSemProdutoPreCadastrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSuperiorArla", Column = "RED_QUANTIDADE_SUPERIOR_ARLA", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeSuperiorArla { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NCMs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ENTRADA_DOCUMENTO_NCM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegraEntradaDocumentoNCM", Column = "REN_CODIGO")]
        public virtual IList<RegraEntradaDocumentoNCM> NCMs { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Fornecedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ENTRADA_DOCUMENTO_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegraEntradaDocumentoFornecedor", Column = "REF_CODIGO")]
        public virtual IList<RegraEntradaDocumentoFornecedor> Fornecedores { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(RegraEntradaDocumento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
