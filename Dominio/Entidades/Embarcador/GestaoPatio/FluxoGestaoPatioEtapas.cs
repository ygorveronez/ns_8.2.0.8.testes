using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_GESTAO_PATIO_ETAPAS", EntityName = "FluxoGestaoPatioEtapas", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas", NameType = typeof(FluxoGestaoPatioEtapas))]
    public class FluxoGestaoPatioEtapas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "FGE_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ETAPA_FLUXO_GESTAO", TypeType = typeof(EtapaFluxoGestaoPatio), NotNull = true)]
        public virtual EtapaFluxoGestaoPatio EtapaFluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ETAPA_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_ETAPA_VISUALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaVisualizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_EXIBIR_ALERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_ALERTA_TEMPO_EXCEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlertaTempoExcedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "FGE_DATA_ALERTA_TEMPO_FALTANTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlertaTempoFaltante { get; set; }

        public virtual string Descricao
        {
            get
            { 
                return $"{EtapaFluxoGestaoPatio.ObterDescricao()} - {FluxoGestaoPatio?.Descricao}";
            }
        }
    }
}
