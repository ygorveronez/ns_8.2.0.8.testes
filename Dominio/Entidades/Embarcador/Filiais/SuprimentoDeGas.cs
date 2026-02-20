using System;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SUPRIMENTO_DE_GAS", EntityName = "SuprimentoDeGas", Name = "Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas", NameType = typeof(SuprimentoDeGas))]
    public class SuprimentoDeGas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SDG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteSolicitacao", Column = "SDG_HORA_LIMITE_SOLICITACAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraLimiteSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteGerente", Column = "SDG_HORA_LIMITE_GERENTE", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraLimiteGerente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraBloqueioSolicitacao", Column = "SDG_HORA_BLOQUEIO_SOLICITACAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraBloqueioSolicitacao { get; set; }
        
        [Obsolete("Campo deixou de ser utilizado.", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteSolicitacaoComJustificativa", Column = "SDG_HORA_LIMITE_SOLICITACAO_COM_JUSTIFICATIVA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraLimiteSolicitacaoComJustificativa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCargaPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacaoPadrao { get; set; }

        [Obsolete("Passou a ser por Cliente", true)]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial UnidadeSupridoraPadrao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente SupridorPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDG_EMAIL_LIMITE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NotificarPorEmailLimite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDG_EMAIL_GERENTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NotificarPorEmailGerente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDG_EMAIL_BLOQUEIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NotificarPorEmailBloqueio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaNotificacaoLimite", Column = "SDG_DATA_ULTIMA_NOTIFICACAO_LIMITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaNotificacaoLimite { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaNotificacaoGerente", Column = "SDG_DATA_ULTIMA_NOTIFICACAO_GERENTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaNotificacaoGerente { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaNotificacaoBloqueio", Column = "SDG_DATA_ULTIMA_NOTIFICACAO_BLOQUEIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaNotificacaoBloqueio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Capacidade", Column = "SDG_CAPACIDADE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Capacidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lastro", Column = "SDG_LASTRO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Lastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstoqueMinimo", Column = "SDG_ESTOQUE_MINIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal EstoqueMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstoqueMaximo", Column = "SDG_ESTOQUE_MAXIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal EstoqueMaximo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Suprimento de Gás {this.Codigo}";
            }
        }
    }
}
