namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_CONTA", EntityName = "PlanoDeConta", Name = "Dominio.Entidades.PlanoDeConta", NameType = typeof(PlanoDeConta))]
    public class PlanoDeConta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PLA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// A - ANALÍTICO
        /// S - SINTÉTICO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PLA_TIPO", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Conta", Column = "PLA_CONTA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Conta { get; set; }

        /// <summary>
        /// R - RECEITA
        /// D - DESPESA
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDeConta", Column = "PLA_TIPO_CONTA", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string TipoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContaContabil", Column = "PLA_CONTA_CONTABIL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PLA_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExibirDRE", Column = "PLA_NAO_EXIBIR_DRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirDRE { get; set; }

        public virtual string DescricaoTipoDeConta
        {
            get
            {
                switch (this.TipoDeConta)
                {
                    case "R":
                        return "Receita";
                    case "D":
                        return "Despesa";
                    case "O":
                        return "Outras";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case "A":
                        return "Analítico";
                    case "S":
                        return "Sintético";
                    default:
                        return string.Empty;
                }
            }
        }

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
