namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_PARTICIPANTE", EntityName = "ParticipanteNFSe", Name = "Dominio.Entidades.ParticipanteNFSe", NameType = typeof(ParticipanteNFSe))]
    public class ParticipanteNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF_CNPJ", Column = "PNF_CPF_CNPJ", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPF_CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoExterior", Column = "PNF_NUM_DOC_EXTERIOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDocumentoExterior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PNF_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPessoa Tipo { get; set; }

        /// <summary>
        /// Se vazio utilizar ISENTO.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IE_RG", Column = "PNF_IERG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IE_RG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PNF_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "PNF_NOME_FANTASIA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string NomeFantasia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "PNF_ENDERECO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PNF_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "PNF_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "PNF_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cidade", Column = "PNF_CIDADE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "PNF_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone1", Column = "PNF_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone2", Column = "PNF_FAX", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "PNF_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailStatus", Column = "PNF_EMAIL_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContato", Column = "PNF_EMAIL_CONTATO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContatoStatus", Column = "PNF_EMAIL_CONTATO_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailContatoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContador", Column = "PNF_EMAIL_CONTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContadorStatus", Column = "PNF_EMAIL_CONTADOR_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailContadorStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoSuframa", Column = "PNF_INSCRICAO_SUFRAMA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoSuframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoMunicipal", Column = "PNF_INSCRICAO_MUNICIPAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Exterior", Column = "PNF_EXTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Exterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SalvarEndereco", Column = "PNF_SALVAR_ENDERECO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PNF_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual string CPF_CNPJ_Formatado
        {
            get
            {
                long cpf_cnpj = 0L;

                if (long.TryParse(this.CPF_CNPJ, out cpf_cnpj))
                {
                    return this.Tipo == Enumeradores.TipoPessoa.Juridica ? string.Format(@"{0:00\.000\.000\/0000\-00}", cpf_cnpj) : string.Format(@"{0:000\.000\.000\-00}", cpf_cnpj);
                }
                else
                {
                    return this.CPF_CNPJ;
                }
            }
        }
    }
}
