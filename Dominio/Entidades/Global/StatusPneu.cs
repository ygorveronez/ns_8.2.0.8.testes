using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PNEU_STATUS", EntityName = "StatusPneu", Name = "Dominio.Entidades.StatusPneu", NameType = typeof(StatusPneu))]
    public class StatusPneu : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PST_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PST_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PST_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        /// <summary>
        /// E - ENTRADA
        /// S - SERVICO
        /// F - FINAL
        /// A - SAÍDA
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PST_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PST_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

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

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case "E":
                        return "Entrada";
                    case "S":
                        return "Serviço";
                    case "A":
                        return "Saída";
                    case "F":
                        return "Final";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
