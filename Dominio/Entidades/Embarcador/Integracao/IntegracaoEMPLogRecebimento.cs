using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_EMP_LOG_RECEBIMENTO", EntityName = "IntegracaoEMPLogRecebimento", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento", NameType = typeof(IntegracaoEMPLogRecebimento))]
    public class IntegracaoEMPLogRecebimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "IER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoRecebimento", Column = "IER_ARQUIVO_RECEBIMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ArquivoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "IER_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Topic", Column = "IER_TOPIC", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Topic { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensageRetorno", Column = "IER_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensageRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IER_SUCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Sucesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "IER_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracaoEMP), NotNull = false)]
        public virtual TipoIntegracaoEMP TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "IER_JUSTIFICATIVA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "IER_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracaoEMP), NotNull = false)]
        public virtual SituacaoIntegracaoEMP SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBooking", Column = "IER_NUMERO_BOOKING", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustomerCode", Column = "IER_CUSTOMER_CODE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string CustomerCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ScheduleViagemNavio", Column = "IER_VIAGEM_NAVIO_SCHEDULE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ScheduleViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Fatura", Column = "IER_FATURA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Boleto", Column = "IER_BOLETO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Boleto { get; set; }
    }
}
