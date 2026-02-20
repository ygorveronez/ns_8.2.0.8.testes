using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PNEU_HISTORICO", EntityName = "HistoricoPneu", Name = "Dominio.Entidades.HistoricoPneu", NameType = typeof(HistoricoPneu))]
    public class HistoricoPneu : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EixoVeiculo", Column = "VTE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EixoVeiculo Eixo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pneu", Column = "PNS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PNH_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        /// <summary>
        /// E - ENTRADAS
        /// S - SAIDAS
        /// R - RODIZIO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PNH_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Kilometragem", Column = "PNH_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int Kilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PNH_OBS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Calibragem", Column = "PNH_CALIBRAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Calibragem { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case "E":
                        return "Entrada";
                    case "S":
                        return "Saída";
                    case "R":
                        return "Rodízios";
                    default:
                        return "";
                }
            }
        }
    }
}
