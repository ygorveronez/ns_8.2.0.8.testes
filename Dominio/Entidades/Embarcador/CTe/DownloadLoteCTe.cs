using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOWNLOAD_LOTE_CTE", EntityName = "DownloadLoteCTe", Name = "Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe", NameType = typeof(DownloadLoteCTe))]
    public class DownloadLoteCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSolicitacao", Column = "DLC_DATA_SOLICITACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTermino", Column = "DLC_DATA_TERMINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTermino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "DLC_SITUACAO", TypeType = typeof(SituacaoDownloadLoteCTe), NotNull = true)]
        public virtual SituacaoDownloadLoteCTe Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemFalha", Column = "DLC_MSG_FALHA", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string MensagemFalha { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Lote N° {this.Codigo}";
            }
        }
    }
}
