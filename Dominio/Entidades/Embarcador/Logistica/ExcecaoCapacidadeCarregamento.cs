using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_EXCECAO_CAPACIDADE", EntityName = "ExcecaoCapacidadeCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento", NameType = typeof(ExcecaoCapacidadeCarregamento))]
    public class ExcecaoCapacidadeCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CEX_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CEX_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_TIPO_ABRANGENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAbrangenciaExcecaoCapacidadeCarregamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAbrangenciaExcecaoCapacidadeCarregamento TipoAbrangencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_CAPACIDADE_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_SEGUNDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_TERCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_QUARTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_QUINTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_SEXTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_SABADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_DISPONIVEL_DOMINGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelDomingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_CAPACIDADE_CARREGAMENTO_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_CAPACIDADE_CARREGAMENTO_CUBAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeCarregamentoCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PeriodosCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoCarregamento", Column = "PEC_CODIGO")]
        public virtual ICollection<PeriodoCarregamento> PeriodosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PrevisoesCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PREVISAO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PrevisaoCarregamento", Column = "PRC_CODIGO")]
        public virtual ICollection<PrevisaoCarregamento> PrevisoesCarregamento { get; set; }

        public virtual string DescricaoData => $"{Data.ToDateString()}{(DataFinal.HasValue ? $" - {DataFinal?.ToDateString()}" : "")}";

        public virtual string DescricaoDiaSemana
        {
            get
            {
                List<string> diasHabilitados = new List<string>();
                if (DisponivelSegunda) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Seg);
                if (DisponivelTerca) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Ter);
                if (DisponivelQuarta) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Qua);
                if (DisponivelQuinta) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Qui);
                if (DisponivelSexta) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Sex);
                if (DisponivelSabado) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Sab);
                if (DisponivelDomingo) diasHabilitados.Add(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Dom);

                return string.Join("; ", diasHabilitados);
            }
        }
    }
}
