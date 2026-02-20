using Dominio.Entidades.Embarcador.Logistica;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_PATIO_CANCELAMENTO", EntityName = "FluxoPatioCancelamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento", NameType = typeof(FluxoPatioCancelamento))]
    public class FluxoPatioCancelamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "FPC_MOTIVO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Motivo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RemoverVeiculoFilaCarregamento", Column = "FPC_REMOVER_VEICULO_FILA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverVeiculoFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo VeiculoBloqueado { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRetiradaFilaCarregamento", Column = "FMR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRetiradaFilaCarregamento MotivoRetiradaFilaCarregamento { get; set; }
    }
}
