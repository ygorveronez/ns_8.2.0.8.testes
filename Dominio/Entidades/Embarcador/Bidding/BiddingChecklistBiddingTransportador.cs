using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_CHECKLIST_TRANSPORTADOR_BIDDING", EntityName = "BiddingChecklistBiddingTransportador", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador", NameType = typeof(BiddingChecklistBiddingTransportador))]
    public class BiddingChecklistBiddingTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingConvite", Column = "BCC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingConvite BiddingConvite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "BCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTB_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRetorno { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTB_ACEITAMENTO", TypeType = typeof(string), NotNull = true)]
        public virtual string Aceitamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTB_ACEITAMENTO_DESEJAVEL", TypeType = typeof(string), NotNull = true)]
        public virtual string AceitamentoDesejavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTB_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 300)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTB_SITUACAO", TypeType = typeof(StatusBiddingConviteTransportadorRespostas), NotNull = true)]
        public virtual StatusBiddingConviteTransportadorRespostas Situacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{this.Transportador.NomeFantasia} com {this.BiddingConvite.Descricao}";
            }
        }
    }
}
