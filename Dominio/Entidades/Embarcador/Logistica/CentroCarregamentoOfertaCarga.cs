using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_OFERTA_CARGA", EntityName = "CentroCarregamentoOfertaCarga", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga", NameType = typeof(CentroCarregamentoOfertaCarga))]
    public class CentroCarregamentoOfertaCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Regra", Column = "CCO_REGRA", TypeType = typeof(RegraOfertaCarga), NotNull = false)]
        public virtual RegraOfertaCarga Regra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "CCO_PRIORIDADE", TypeType = typeof(PrioridadeOfertaCarga), NotNull = false)]
        public virtual PrioridadeOfertaCarga Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoDiferenciadoShare", Column = "CED_PERIODO_DIFERENCIADO_SHARE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PeriodoDiferenciadoShare { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialPeriodoDiferenciadoShare", Column = "CED_DATA_INICIAL_PERIODO_SHARE", TypeType = typeof(System.DateTime), NotNull = false)]
        public virtual System.DateTime? DataInicialPeriodoDiferenciadoShare { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalPeriodoDiferenciadoShare", Column = "CED_DATA_FINAL_PERIODO_SHARE", TypeType = typeof(System.DateTime), NotNull = false)]
        public virtual System.DateTime? DataFinalPeriodoDiferenciadoShare { get; set; }
        
        public virtual string Descricao { get { return $"Regra de Oferta de Carga do Centro de Carrgamento: {this.CentroCarregamento.Descricao}"; } }

    }
}
