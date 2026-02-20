using Dominio.Entidades.Embarcador.Cargas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_TRANSPORTADOR_ROTA_OFERTA", EntityName = "BiddingTransportadorRotaOferta", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta", NameType = typeof(BiddingTransportadorRotaOferta))]

    public class BiddingTransportadorRotaOferta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_RODADA", TypeType = typeof(Int32), NotNull = true)]
        public virtual int Rodada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_FIXO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_QUILOMETRAGEM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_FRANQUIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_FIXO_EQUIPAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFixoEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_FIXO_MENSAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFixoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_KM_RODADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorKmRodado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_PORCENTAGEM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Porcentagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_VIAGEM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_VALOR_ENTREGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_ICMS_PORCENTAGEM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ICMSPorcentagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_FRETE_TONELADA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal FreteTonelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_PEDAGIO_PARA_EIXO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PedagioParaEixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_AJUDANTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Ajudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_ADICIONAL_POR_ENTREGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AdicionalPorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "TRO_MODELO_VEICULAR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingTransportadorRota", Column = "TRO_TRANSPORTADOR_ROTA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingTransportadorRota TransportadorRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_STATUS_ROTA", TypeType = typeof(StatusBiddingRota), NotNull = true)]
        public virtual StatusBiddingRota Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_TIPO_OFERTA", TypeType = typeof(TipoLanceBidding), NotNull = true)]
        public virtual TipoLanceBidding TipoOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_TIPO_TRANSPORTADOR", TypeType = typeof(TipoTransportadorBidding), NotNull = false)]
        public virtual TipoTransportadorBidding? TipoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_ACEITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Aceito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoEstimado", Column = "TRO_CUSTO_ESTIMADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal CustoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_REPLICAR_ICMS_DESTE_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarICMSDesteModeloVeicular { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_NAO_OFERTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoOfertar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_CODIGO_TABELA_FRETE_PAI", TypeType = typeof(Int32), NotNull = false)]
        public virtual int CodigoTabelaFretePai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRO_INFORMAR_VEICULOS_VERDES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarVeiculosVerdes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculosVerdes", Column = "TRO_VEICULOS_VERDES", TypeType = typeof(int), NotNull = false)]
        public virtual int VeiculosVerdes { get; set; }

        public virtual string Descricao
        {
            get
            {
                switch (this.TipoOferta)
                {
                    case TipoLanceBidding.LancePorEquipamento:
                        return $"{this.ValorFixoEquipamento} Equipamento";
                    case TipoLanceBidding.LanceFrotaFixaFranquia:
                        return $"{this.ValorFixo} + {this.ValorFranquia} a cada {this.Quilometragem} km";
                    case TipoLanceBidding.LancePorcentagemNota:
                        return $"{this.Porcentagem}% sobre nota";
                    case TipoLanceBidding.LanceViagemAdicional:
                        return $"{this.ValorViagem} viagem + {this.ValorEntrega} por entrega";
                    case TipoLanceBidding.LanceFrotaFixaKmRodado:
                        return $"{this.ValorFixoMensal} Mês + {this.ValorKmRodado} por km";
                    default:
                        return "Sem descrição";
                }
            }
        }

        public virtual string DescricaoOferta
        {
            get
            {
                switch (this.TipoOferta)
                {
                    case TipoLanceBidding.LancePorEquipamento:
                        return this.ValorFixoEquipamento.ToString();
                    case TipoLanceBidding.LanceFrotaFixaFranquia:
                        return (this.ValorFixo + this.ValorFranquia).ToString();
                    case TipoLanceBidding.LancePorcentagemNota:
                        return this.Porcentagem.ToString();
                    case TipoLanceBidding.LanceViagemAdicional:
                        return this.ValorViagem.ToString();
                    case TipoLanceBidding.LanceFrotaFixaKmRodado:
                        return this.ValorFixoMensal.ToString();
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoOferta
        {
            get
            {
                switch (this.TipoOferta)
                {
                    case TipoLanceBidding.LancePorEquipamento:
                        return $"Equipamento";
                    case TipoLanceBidding.LanceFrotaFixaFranquia:
                        return $"Valor Fixo + Valor da Franquia a cada {this.Quilometragem} km";
                    case TipoLanceBidding.LancePorcentagemNota:
                        return $"% sobre nota";
                    case TipoLanceBidding.LanceViagemAdicional:
                        return $"Valor da viagem + {this.ValorEntrega} por entrega";
                    case TipoLanceBidding.LanceFrotaFixaKmRodado:
                        return $"Valor fixo mensal + {this.ValorKmRodado} por km";
                    case TipoLanceBidding.LancePorPeso:
                        return $"Peso";
                    default:
                        return "Sem descrição";
                }
            }
        }
    }
}
