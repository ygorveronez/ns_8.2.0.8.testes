using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_CONVITE", EntityName = "BiddingConvite", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingConvite", NameType = typeof(BiddingConvite))]
    public class BiddingConvite : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_EXIGE_PREENCHIMENTO_CHECKLIST_NO_CONVITE_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirPreenchimentoChecklistConvitePeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DATA_LIMITE", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLimite { get; set; }
       
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DATA_FINAL_ACEITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrazoAceiteConvite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_STATUS_CONVITE", TypeType = typeof(StatusBiddingConvite), NotNull = true)]
        public virtual StatusBiddingConvite Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DESCRITIVO_CONVITE", Type = "StringClob", NotNull = true)]
        public virtual string DescritivoConvite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DESCRITIVO_TRANSPORTADOR", Type = "StringClob", NotNull = false)]
        public virtual string DescritivoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBC_TIPO_FRETE", TypeType = typeof(TipoFrete), NotNull = false)]
        public virtual TipoFrete? TipoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoBidding", Column = "TBI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoBidding TipoBidding { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_CONVITE_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BiddingConviteAnexo", Column = "ANX_CODIGO")]
        public virtual IList<BiddingConviteAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Convidados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_CONVITE_CONVIDADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BiddingConviteConvidado", Column = "BCC_CODIGO")]
        public virtual IList<BiddingConviteConvidado> Convidados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Checklist", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BIDDING_CHECKLIST")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BiddingChecklist", Column = "TCL_CODIGO")]
        public virtual IList<BiddingChecklist> Checklist { get; set; }

        public virtual double TempoRestante
        {
            get
            {
                DateTime? data = DataPrazoAceiteConvite.HasValue ? DataPrazoAceiteConvite.Value : null;
                TimeSpan? diferenca = data.HasValue ? (data.Value - DateTime.Now) : null;
                return diferenca.HasValue ? (diferenca.Value.TotalHours) : 0;
            }
        }
    }
}
