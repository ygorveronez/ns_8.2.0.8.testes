using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO", EntityName = "Pagamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.Pagamento", NameType = typeof(Pagamento))]
    public class Pagamento : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PAG_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDocsPagamento", Column = "PAG_QUANTIDADE_DOCUMENTOS_PAGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDocsPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPagamento", Column = "PAG_VALOR_PAGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_DATA_INICIO_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_DATA_FIM_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalEmissao { get; set; }

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

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerandoMovimentoFinanceiro", Column = "PAG_GERANDO_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoMovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoAutomaticamente", Column = "PAG_GERADO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LotePagamentoLiberado", Column = "PAG_LOTE_PAGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LotePagamentoLiberado { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "PAG_CARGA_EM_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool CargaEmCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CargasLiberadas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_CARGA_LIBERADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.Carga> CargasLiberadas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_ULTIMA_CARGA_EM_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga UltimaCargaEmCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_AUTORIZADOR_CARGA_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario AutorizadorCargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_DATA_AUTORIZACAO_CARGA_EM_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoCargaEmCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoFechamentoPagamento", Column = "PAG_MOTIVO_REJEICAO_CANCELAMENTO_FECHAMENTO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string MotivoRejeicaoFechamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAG_SITUACAO", TypeType = typeof(SituacaoPagamento), NotNull = false)]
        public virtual SituacaoPagamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosFaturamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoFaturamento", Column = "DFA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> DocumentosFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosFaturamentoLiberado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAG_CODIGO_LIBERACAO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoFaturamento", Column = "DFA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> DocumentosFaturamentoLiberado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoIntegracao", Column = "PIN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> Integracoes { get; set; }

        public virtual string Descricao
        {
            get {  return this.Numero.ToString(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

    }
}
