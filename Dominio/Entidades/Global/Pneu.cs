using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PNEU", EntityName = "Pneu", Name = "Dominio.Entidades.Pneu", NameType = typeof(Pneu))]
    public class Pneu : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaPneu", Column = "PMA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaPneu MarcaPneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloPneu", Column = "PMO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloPneu ModeloPneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DimensaoPneu", Column = "PDI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DimensaoPneu DimensaoPneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StatusPneu", Column = "PST_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual StatusPneu StatusPneu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "PNS_SERIE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompra", Column = "PNS_DATACOMPRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVenda", Column = "PNS_DATAVENDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCompra", Column = "PNS_VALORCOMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Custos", Column = "PNS_CUSTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Custos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Receitas", Column = "PNS_RECEITA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Receitas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PNS_OBS", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PNS_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EixoVeiculo", Column = "VTE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EixoVeiculo Eixo { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }
    }
}
