using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_GESTAO_ENTREGA", EntityName = "FluxoGestaoEntrega", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega", NameType = typeof(FluxoGestaoEntrega))]
    public class FluxoGestaoEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ETAPA_ATUAL_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaAtualLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_INDEX_ETAPA", TypeType = typeof(int), NotNull = true)]
        public virtual int IndexEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ETAPA_ATUAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio EtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Etapas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_GESTAO_ENTREGA_ETAPAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FGE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FluxoGestaoEntregaEtapas", Column = "GEE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntregaEtapas> Etapas { get; set; }

        #region TemposEtapas

        private int? DiffTimeMinutes(DateTime? previsto, DateTime? realizado)
        {
            if (!previsto.HasValue || !realizado.HasValue)
                return null;

            return (int)(previsto.Value - realizado.Value).TotalMinutes;
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_INICIO_VIAGEM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_INICIO_VIAGEM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemReprogramada { get; set; }

        public virtual int? DiferencaInicioViagem
        {
            get
            {
                return DiffTimeMinutes(this.DataInicioViagemPrevista, this.DataInicioViagem);
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_POSICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_FIM_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagem { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_FIM_VIAGEM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_FIM_VIAGEM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagemReprogramada { get; set; }

        public virtual int? DiferencaFimViagem
        {
            get
            {
                return DiffTimeMinutes(this.DataFimViagemPrevista, this.DataFimViagem);
            }
        }
        
        #endregion

        public virtual List<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntregaEtapas> GetEtapas()
        {
            if (this.Etapas == null) return new List<FluxoGestaoEntregaEtapas>();

            return (from o in this.Etapas orderby o.Ordem ascending select o).ToList();
        }

        public virtual string Descricao
        {
            get
            {
                return this.Carga?.CodigoCargaEmbarcador ?? string.Empty;
            }
        }
    }
}
