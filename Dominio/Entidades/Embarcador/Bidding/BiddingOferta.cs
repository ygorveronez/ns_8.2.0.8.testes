using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_OFERTA", EntityName = "BiddingOferta", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingOferta", NameType = typeof(BiddingOferta))]
    public class BiddingOferta : EntidadeBase
    {
        private const string MensagemObsolete = "Migrado para TipoLance";

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingConvite", Column = "TBC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Bidding.BiddingConvite BiddingConvite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_PRAZO_OFERTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrazoOferta { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_EQUIPAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LancePorEquipamento { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_FROTA_FIXA_KM_RODADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LanceFrotaFixaKmRodado { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_PORCENTAGEM_NOTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LancePorcentagemNota { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_VIAGEM_ADICIONAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LanceViagemAdicional { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancePorPeso { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_CAPACIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancePorCapacidade { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_FRETE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancePorFreteViagem { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_FROTA_FIXA_FRANQUIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LanceFrotaFixaFranquia { get; set; }

        [Obsolete(MensagemObsolete)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_LANCE_VIAGEM_ENTREGA_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancePorViagemEntregaAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_TIPO_LANCE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLanceBidding), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLanceBidding TipoLance { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_DATA_LIMITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_PERMITIR_INFORMAR_VEICULOS_VERDES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarVeiculosVerdes { get; set; }

        [Obsolete("Campo foi migrado para BiddingTransportadorRotaOferta")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBO_INFORMAR_VEICULOS_VERDES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarVeiculosVerdes { get; set; }

        [Obsolete("Campo foi migrado para BiddingTransportadorRotaOferta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculosVerdes", Column = "TBO_VEICULOS_VERDES", TypeType = typeof(int), NotNull = false)]
        public virtual int VeiculosVerdes { get; set; }
    }
}
