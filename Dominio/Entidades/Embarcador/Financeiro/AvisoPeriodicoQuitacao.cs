using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AVISO_PERIODICO_QUITACAO", EntityName = "AvisoPeriodicoQuitacao", Name = "Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao", NameType = typeof(Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao))]
    public class AvisoPeriodicoQuitacao : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "TAP_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "TAP_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "TAP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "TAP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAvisoPeriodico", Column = "TAP_SITUACAO_AVISO_PERIODICO", TypeType = typeof(SituacaoAvisoPeriodicoQuitacao), NotNull = false)]
        public virtual SituacaoAvisoPeriodicoQuitacao SituacaoAvisoPeriodico { get; set; }

        //Pagamentos
        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPagamentoEDescontosViaCreditoConta", Column = "TAP_TOTAL_PAGAMENTO_DESCONTOS_CREDITO_CONTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPagamentoEDescontosViaCreditoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPagamentoEDescontosViaConfirming", Column = "TAP_TOTAL_PAGAMENTO_DESCONTOS_CONFIRMING", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPagamentoEDescontosViaConfirming { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPagamentoEDescontosEmConta", Column = "TAP_TOTAL_PAGAMENTO_DESCONTOS_CONTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPagamentoEDescontosEmConta { get; set; }

        //Adiantamentos
        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalAdiantamento", Column = "TAP_TOTAL_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalNotasCompensadasAdiantamento", Column = "TAP_TOTAL_NOTAS_COMPENSADAS_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalNotasCompensadasAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSaldoAdiantamentoEmAberto", Column = "TAP_TOTAL_SALDO_ADIANTAMENTO_EM_ABERTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalSaldoAdiantamentoEmAberto { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalGeralPagamentos", Column = "TAP_TOTAL_GERAL_PAGAMENTOS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalGeralPagamentos { get; set; }

        //Pendencias
        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasVencidoTransportador", Column = "TAP_TOTAL_PENDENCIAS_VENCIDO_TRANSPORTADOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasVencidoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasAVencerTransportador", Column = "TAP_TOTAL_PENDENCIAS_A_VENCER_TRANSPORTADOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasAVencerTransportador { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasVencidoDesbloqueado", Column = "TAP_TOTAL_PENDENCIAS_VENCIDO_DESBLOQUEADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasVencidoDesbloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasAVencerDesbloqueado", Column = "TAP_TOTAL_PENDENCIAS_A_VENCER_DESBLOQUEADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasAVencerDesbloqueado { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasVencidoUnilever", Column = "TAP_TOTAL_PENDENCIAS_VENCIDO_UNILEVER", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasVencidoUnilever { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasAVencerUnilever", Column = "TAP_TOTAL_PENDENCIAS_A_VENCER_UNILEVER", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasAVencerUnilever { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasVencidoBloqueioPOD", Column = "TAP_TOTAL_PENDENCIAS_VENCIDO_BLOQUEIO_POD", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasVencidoBloqueioPOD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendenciasAVencerBloqueioPOD", Column = "TAP_TOTAL_PENDENCIAS_A_VENCER_BLOQUEIO_POD", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendenciasAVencerBloqueioPOD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendentesAVencerBloqueioIrregularidade", Column = "TAP_TOTAL_PENDENCIAS_A_VENCER_BLOQUEIO_IRREGULARIDADE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendentesAVencerBloqueioIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendentesVencidaBloqueioIrregularidade", Column = "TAP_TOTAL_PENDENCIAS_VENCIDO_BLOQUEIO_IRREGULARIDADE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendentesVencidaBloqueioIrregularidade { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPendencias", Column = "TAP_TOTAL_PENDENCIAS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalPendencias { get; set; }


        //Avarias
        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalAvariasEmAberto", Column = "TAP_TOTAL_AVARIAS_EM_ABERTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalAvariasEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalDebitoBaixa", Column = "TAP_TOTAL_DEBITO_BAIXA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalDebitoBaixa { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalProjecoesRecebimento", Column = "TAP_TOTAL_PROJECOES_RECEBIMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalProjecoesRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaRejeicao", Column = "TAP_JUSTIFICATIVA_REJEICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string JustificativaRejeicao { get; set; }

    }
}
