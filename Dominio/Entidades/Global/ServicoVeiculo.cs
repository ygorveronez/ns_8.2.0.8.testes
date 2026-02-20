namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SERVICO_VEICULO", EntityName = "ServicoVeiculo", Name = "Dominio.Entidades.ServicoVeiculo", NameType = typeof(ServicoVeiculo))]
    public class ServicoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoDeConta PlanoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SER_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SER_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTroca", Column = "SER_KM_TROCA", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTroca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasTroca", Column = "SER_DIAS_TROCA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasTroca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAvisoManutencao", Column = "SER_AVISODIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAvisoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMAvisoManutencao", Column = "SER_AVISOKM", TypeType = typeof(int), NotNull = false)]
        public virtual int KMAvisoManutencao { get; set; }

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
