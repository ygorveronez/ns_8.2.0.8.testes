using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANCELAMENTO_PAGAMENTO", EntityName = "CancelamentoPagamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento", NameType = typeof(CancelamentoPagamento))]
    public class CancelamentoPagamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_QUANTIDADE_DOCUMENTOS_PAGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDocsCancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_VALOR_PAGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCancelamentoPagamento", Column = "MCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento MotivoCancelamento { get; set; }

        [Obsolete("Campo descontinuado pois foi transformado em um set.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_GERANDO_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoMovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_MOTIVO_REJEICAO_CANCELAMENTO_FECHAMENTO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string MotivoRejeicaoFechamentoCancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPG_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosFaturamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoFaturamento", Column = "DFA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> DocumentosFaturamento { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANCELAMENTO_PAGAMENTO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CancelamentoPagamentoIntegracao", Column = "CPI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANCELAMENTO_PAGAMENTO_PAGAMENTOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pagamento", Column = "PAG_CODIGO")]
        public virtual ICollection<Pagamento> Pagamentos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmCancelamento:
                        return "Em Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.PendenciaCancelamento:
                        return "Pendência no Fechamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.AgIntegracao:
                        return "Ag. Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmIntegracao:
                        return "Em Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.FalhaIntegracao:
                        return "Falha na integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

    }
}
