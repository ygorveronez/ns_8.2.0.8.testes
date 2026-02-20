namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_VEICULO_PROPRIETARIO", EntityName = "ProprietarioVeiculoCTe", Name = "Dominio.Entidades.ProprietarioVeiculoCTe", NameType = typeof(ProprietarioVeiculoCTe))]
    public class ProprietarioVeiculoCTe: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF_CNPJ", Column = "PVE_CPF_CNPJ", TypeType = typeof(string), Length = 14, NotNull = true)]
        public virtual string CPF_CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IE", Column = "PVE_IE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string IE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PVE_NOME", TypeType = typeof(string), Length = 60, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "PVE_RNTRC", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PVE_TIPO", TypeType = typeof(Enumeradores.TipoProprietarioVeiculo), NotNull = true)]
        public virtual Enumeradores.TipoProprietarioVeiculo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Estado", Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        public virtual ProprietarioVeiculoCTe Clonar()
        {
            return (ProprietarioVeiculoCTe)this.MemberwiseClone();
        }
    }
}
