namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_EIXO", EntityName = "EixoVeiculo", Name = "Dominio.Entidades.EixoVeiculo", NameType = typeof(EixoVeiculo))]
    public class EixoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dianteiro", Column = "VTE_DIANTEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Dianteiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemEixo", Column = "VTE_ORDEM_EIXO", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemEixo { get; set; }

        /// <summary>
        /// S - SIMPLES
        /// D - DUPLO
        /// E - ESTEPE
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "VTE_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VTE_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VTE_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        /// <summary>
        /// I - INTERNO
        /// E - EXTERNO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Interno_Externo", Column = "VTE_INT_EXT", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Interno_Externo { get; set; }

        /// <summary>
        /// D - DIREITO
        /// E - ESQUERDO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "VTE_POSICAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Posicao { get; set; }

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
                    case "S":
                        return "Simples";
                    case "D":
                        return "Duplo";
                    case "E":
                        return "Estepe";
                    default:
                        return "";
                }
            }
        }
    }
}
