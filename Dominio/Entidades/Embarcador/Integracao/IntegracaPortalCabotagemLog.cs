using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_PORTAL_CABOTAGEM_LOG", EntityName = "IntegracaPortalCabotagemLog", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaPortalCabotagemLog", NameType = typeof(IntegracaPortalCabotagemLog))]
    public class IntegracaPortalCabotagemLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "IEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoEnvio", Column = "IEM_ARQUIVO_ENVIO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ArquivoEnvio { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoRetorno", Column = "IEM_ARQUIVO_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ArquivoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "IEM_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Topic", Column = "IEM_TOPIC", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Topic { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensageRetorno", Column = "IEM_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensageRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusIntegracaoEMP", Column = "IEM_STATUS_INTEGRACAO", TypeType = typeof(StatusIntegracaoEMP), NotNull = false)]
        public virtual StatusIntegracaoEMP StatusIntegracaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "IEM_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracaoEMP), NotNull = false)]
        public virtual TipoIntegracaoEMP TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "IEM_JUSTIFICATIVA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "IEM_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracaoEMP), NotNull = false)]
        public virtual SituacaoIntegracaoEMP SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBooking", Column = "IEM_NUMERO_BOOKING", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NumeroBooking { get; set; }
    }
}
