using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_EXCECAO_CAPACIDADE", EntityName = "ExcecaoCapacidadeDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento", NameType = typeof(ExcecaoCapacidadeDescarregamento))]
    public class ExcecaoCapacidadeDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CEX_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_CAPACIDADE_DESCARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CEX_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CEX_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }
        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "PeriodosDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamento", Column = "PED_CODIGO")]
        public virtual ICollection<PeriodoDescarregamento> PeriodosDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PrevisoesDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PREVISAO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PrevisaoDescarregamento", Column = "PRD_CODIGO")]
        public virtual ICollection<PrevisaoDescarregamento> PrevisoesDescarregamento { get; set; }
    }
}
