namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_ENDERECO_CLIENTE", EntityName = "EnderecoParticipanteCTe", Name = "Dominio.Entidades.EnderecoParticipanteCTe", NameType = typeof(EnderecoParticipanteCTe))]
    public class EnderecoParticipanteCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "CEC_ENDERECO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CEC_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "CEC_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "CEC_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "CEC_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CEC_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        public virtual EnderecoParticipanteCTe Clonar()
        {
            return (EnderecoParticipanteCTe)this.MemberwiseClone();
        }

    }
}
