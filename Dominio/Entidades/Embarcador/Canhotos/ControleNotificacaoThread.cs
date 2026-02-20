using System;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_NOTIFICACAO_CANHOTO_THREAD", EntityName = "ControleNotificacaoThread", Name = "Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread", NameType = typeof(ControleNotificacaoThread))]
    public class ControleNotificacaoThread : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNT_DATA_ULTIMO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataUltimoProcessamento { get; set; }
    }
}
