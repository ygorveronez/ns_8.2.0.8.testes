using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PACOTE_WEBHOOK", EntityName = "PacoteWebHook", Name = "Dominio.Entidades.Embarcador.Cargas.PacoteWebHook", NameType = typeof(PacoteWebHook))]
    public class PacoteWebHook : EntidadeBase
    {
        public PacoteWebHook() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PWT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "PWT_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogKey", Column = "PWT_LOG_KEY", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string LogKey { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiroXML", Column = "CTX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML CTeTerceiroXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "PWT_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemIntegracao", Column = "PWT_MENSAGEM_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MensagemIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.LogKey) ? this.LogKey : string.Empty;
            }
        }
    }
}
