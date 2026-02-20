using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_CAPACIDADE_DESCARREGAMENTO_ADICIONAL", EntityName = "CapacidadeDescarregamentoAdicional", Name = "Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional", NameType = typeof(CapacidadeDescarregamentoAdicional))]
    public class CapacidadeDescarregamentoAdicional : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CDA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoInicio", Column = "CDA_PERIODO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoTermino", Column = "CDA_PERIODO_TERMINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoTermino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_CAPACIDADE_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CDA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CanaisVenda", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_CAPACIDADE_DESCARREGAMENTO_ADICIONAL_CANAL_VENDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CanalVenda", Column = "CNV_CODIGO")]
        public virtual ICollection<Pedidos.CanalVenda> CanaisVenda { get; set; }

        public virtual string Descricao
        {
            get { return $"Capacidade adicional do centro de descarregamento {CentroDescarregamento.Descricao}"; }
        }
    }
}
