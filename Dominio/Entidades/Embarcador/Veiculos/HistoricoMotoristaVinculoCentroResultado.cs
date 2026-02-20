using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_MOTORISTA_VINCULO_CENTRO_RESULTADO", EntityName = "HistoricoMotoristaVinculoCentroResultado", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado", NameType = typeof(HistoricoMotoristaVinculoCentroResultado))]
    public class HistoricoMotoristaVinculoCentroResultado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "HistoricoMotoristaVinculo", Column = "HMV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo HistoricoMotoristaVinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HMM_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHora { get; set; }

        public virtual string Descricao
        {
            get { return CentroResultado?.Descricao ?? string.Empty; }
        }
    }
}