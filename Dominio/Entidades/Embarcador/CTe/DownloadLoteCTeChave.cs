using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOWNLOAD_LOTE_CTE_CHAVE", EntityName = "DownloadLoteCTeChave", Name = "Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave", NameType = typeof(DownloadLoteCTeChave))]
    public class DownloadLoteCTeChave : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DownloadLoteCTe", Column = "DLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DownloadLoteCTe DownloadLoteCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "DCC_CHAVE", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string Chave { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemFalha", Column = "DCC_MSG_FALHA", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string MensagemFalha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "DCC_SITUACAO", TypeType = typeof(SituacaoDownloadLoteCTe), NotNull = true)]
        public virtual SituacaoDownloadLoteCTe Situacao { get; set; }

    }
}
