using Dominio.Entidades.Embarcador.Cargas;
using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_LICITACAO", EntityName = "SolicitacaoLicitacao", Name = "Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao", NameType = typeof(SolicitacaoLicitacao))]
    public class SolicitacaoLicitacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoLicitacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoLicitacao Situacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_TIPO_COTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoSolicitacaoLicitacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoSolicitacaoLicitacao TipoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeDestino { get; set; }

        [Obsolete("Campo migrado para uma lista de Produtos Embarcador")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_PRODUTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Produto { get; set; }

        [Obsolete("Campo migrado para tipo entidade (TipoDeCarga)")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_ACONDICIONAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Acondicionamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida UnidadeMedida { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_LARGURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_ALTURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_COMPRIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalNotaFiscal", Column = "SLI_VALOR_TOTAL_NOTA_FISCAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalNotaFiscal { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoCotacao", Column = "SLI_DESCRICAO_COTACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string DescricaoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_DATA_INICIO_EMBARQUE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_DATA_FIM_EMBARQUE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_DATA_PRAZO_RESPOSTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPrazoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SLI_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_VALOR_TRECHO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_VALOR_TONELADA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTonelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SLI_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_COTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioCotacao { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }
    }
}
