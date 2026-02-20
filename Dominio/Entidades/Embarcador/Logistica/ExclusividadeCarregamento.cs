using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_EXCLUSIVIDADE", EntityName = "ExclusividadeCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento", NameType = typeof(ExclusividadeCarregamento))]
    public class ExclusividadeCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ECC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_SEGUNDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_TERCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_QUARTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_QUINTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_SEXTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_SABADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_DISPONIVEL_DOMINGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelDomingo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PeriodosCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ECC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoCarregamento", Column = "PEC_CODIGO")]
        public virtual ICollection<PeriodoCarregamento> PeriodosCarregamento { get; set; }

        public virtual string DescricaoData => $"{DataInicial.ToDateString()} - {DataFinal.ToDateString()}";

        public virtual string DescricaoExclusividade => (Transportador != null ? $"Transportador {Transportador.Descricao}" : "") + (Cliente != null ? $"{(Transportador != null ? " - " : "")}Cliente {Cliente.Descricao}" : "");

        public virtual string DescricaoDiaSemana
        {
            get
            {
                List<string> diasHabilitados = new List<string>();
                if (DisponivelSegunda) diasHabilitados.Add("Seg");
                if (DisponivelTerca) diasHabilitados.Add("Ter");
                if (DisponivelQuarta) diasHabilitados.Add("Qua");
                if (DisponivelQuinta) diasHabilitados.Add("Qui");
                if (DisponivelSexta) diasHabilitados.Add("Sex");
                if (DisponivelSabado) diasHabilitados.Add("Sab");
                if (DisponivelDomingo) diasHabilitados.Add("Dom");

                return string.Join("; ", diasHabilitados);
            }
        }
    }
}
