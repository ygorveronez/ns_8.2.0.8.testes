using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CONVITE", EntityName = "RFIConvite", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite", NameType = typeof(RFIConvite))]
    public class RFIConvite : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_EXIGE_PREENCHIMENTO_CHECKLIST_NO_CONVITE_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirPreenchimentoChecklistConvitePeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_DATA_LIMITE", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLimite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_DATA_FINAL_ACEITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPrazoAceiteConvite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_STATUS_CONVITE", TypeType = typeof(StatusRFIConvite), NotNull = true)]
        public virtual StatusRFIConvite Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRC_DESCRITIVO_CONVITE", Type = "StringClob", NotNull = true)]
        public virtual string DescritivoConvite { get; set; }   

        public virtual double TempoRestante
        {
            get
            {
                TimeSpan diferenca = DataPrazoAceiteConvite - DateTime.Now;
                return diferenca.TotalHours;
            }
        }
    }
}
