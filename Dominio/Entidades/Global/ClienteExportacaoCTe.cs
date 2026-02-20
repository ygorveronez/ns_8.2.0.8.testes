namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_EXP", EntityName = "ClienteExportacaoCTe", Name = "Dominio.Entidades.ClienteExportacaoCTe", NameType = typeof(ClienteExportacaoCTe))]
    public class ClienteExportacaoCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CLIE_NOME", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "CLIE_ENDERECO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CLIE_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "CLIE_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "CLIE_BAIRRO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cidade", Column = "CLIE_CIDADE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CLIE_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CLIE_EMAIL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Email { get; set; }
    }
}
