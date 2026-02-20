namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_MOTORISTA", EntityName = "MotoristaCTE", Name = "Dominio.Entidades.MotoristaCTE", NameType = typeof(MotoristaCTE))]
    public class MotoristaCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMotoristaCTe", Column = "CMO_NOME_MOTORISTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeMotoristaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFMotorista", Column = "CMO_CPF_MOTORISTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CPFMotorista { get; set; }

        public virtual string NomeMotorista
        {
            get
            {
                return NomeMotoristaCTe != null ? NomeMotoristaCTe.ToUpper() : string.Empty;
            }
            set
            {
                NomeMotoristaCTe = value != null ? value.ToUpper() : value;
            }
        }

        public virtual MotoristaCTE Clonar()
        {
            return (MotoristaCTE)this.MemberwiseClone();
        }
    }
}
