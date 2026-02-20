namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VEICULO_VINCULO_MOTORISTA", EntityName = "HistoricoVeiculoVinculoMotorista", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista", NameType = typeof(HistoricoVeiculoVinculoMotorista))]
    public class HistoricoVeiculoVinculoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HVM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "HistoricoVeiculoVinculo", Column = "HVV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo HistoricoVeiculoVinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

    }
}