using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VEICULO_VINCULO_CENTRO_RESULTADO", EntityName = "HistoricoVeiculoVinculoCentroResultado", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado", NameType = typeof(HistoricoVeiculoVinculoCentroResultado))]
    public class HistoricoVeiculoVinculoCentroResultado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HVM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "HistoricoVeiculoVinculo", Column = "HVV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo HistoricoVeiculoVinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HVM_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHora { get; set; }

        public virtual string Descricao
        {
            get { return CentroResultado?.Descricao ?? string.Empty; }
        }
    }
}