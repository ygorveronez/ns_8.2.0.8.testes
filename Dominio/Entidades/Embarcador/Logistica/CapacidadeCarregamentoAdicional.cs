using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_CAPACIDADE_CARREGAMENTO_ADICIONAL", EntityName = "CapacidadeCarregamentoAdicional", Name = "Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional", NameType = typeof(CapacidadeCarregamentoAdicional))]
    public class CapacidadeCarregamentoAdicional : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CCA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoInicio", Column = "CCA_PERIODO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoTermino", Column = "CCA_PERIODO_TERMINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoTermino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_CAPACIDADE_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CCA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_CAPACIDADE_CARREGAMENTO_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_CAPACIDADE_CARREGAMENTO_CUBAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagem { get; set; }

        public virtual string Descricao
        {
            get { return $"Capacidade adicional do centro de carregamento {CentroCarregamento.Descricao}";  }
        }
    }
}
