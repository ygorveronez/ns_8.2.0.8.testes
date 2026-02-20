using System;
using System.Linq;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_PARTICIPANTE", EntityName = "ParticipanteCTe", Name = "Dominio.Entidades.ParticipanteCTe", NameType = typeof(ParticipanteCTe))]
    public class ParticipanteCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF_CNPJ", Column = "PCT_CPF_CNPJ", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPF_CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PCT_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPessoa Tipo { get; set; }

        /// <summary>
        /// Se vazio utilizar ISENTO.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IE_RG", Column = "PCT_IERG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IE_RG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PCT_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "PCT_NOMEFANTASIA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string NomeFantasia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "PCT_ENDERECO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PCT_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "PCT_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "PCT_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cidade", Column = "PCT_CIDADE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "PCT_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone1", Column = "PCT_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone2", Column = "PCT_FAX", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "PCT_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailStatus", Column = "PCT_EMAIL_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContato", Column = "PCT_EMAILCONTATO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContatoStatus", Column = "PCT_EMAILCONTATO_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailContatoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContador", Column = "PCT_EMAILCONTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContadorStatus", Column = "PCT_EMAILCONTADOR_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailContadorStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoSuframa", Column = "PCT_INSCRICAO_SUFRAMA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoSuframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoMunicipal", Column = "PCT_INSCRICAO_MUNICIPAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Exterior", Column = "PCT_EXTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Exterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SalvarEndereco", Column = "PCT_SALVAR_ENDERECO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoST", Column = "PCT_INSCRICAO_ST", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailTransportador", Column = "PCT_EMAILTRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailTransportadorStatus", Column = "PCT_EMAILTRANSPORTADOR_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailTransportadorStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PCT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEnderecoIntegracao", Column = "PCT_CODIGO_ENDERECO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEnderecoIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                string descricao = "";
                descricao += this.Nome;
                descricao += " (" + this.CPF_CNPJ_Formatado + ")";
                return descricao;
            }
        }

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

        public virtual string CPF_CNPJ_Formatado_AX
        {
            get
            {
                long cpf_cnpj = 0L;

                if (long.TryParse(this.CPF_CNPJ, out cpf_cnpj))
                {
                    return this.Tipo == Enumeradores.TipoPessoa.Juridica ? string.Format(@"{0:000000000000\-00}", cpf_cnpj) : string.Format(@"{0:000000000\-00}", cpf_cnpj);
                }
                else
                {
                    return this.CPF_CNPJ;
                }
            }
        }

        public virtual string CPF_CNPJ_SemFormato
        {
            get
            {
                return this.CPF_CNPJ;
            }
        }

        public virtual string CEP_SemFormato
        {
            get
            {
                return new string(this.CEP?.Where(Char.IsDigit).ToArray());
            }
        }

        public virtual int InscricaoSTEDI
        {
            get
            {
                int inscricao = 0;
                int.TryParse(InscricaoST, out inscricao);
                return inscricao;
            }
        }

        public virtual int InscricaoSTClienteEDI
        {
            get
            {
                int inscricao = 0;
                if (!string.IsNullOrWhiteSpace(InscricaoST))
                    int.TryParse(InscricaoST, out inscricao);
                else if (Cliente != null)
                    int.TryParse(Cliente.InscricaoST, out inscricao);
                return inscricao;
            }
        }

        public virtual int InscricaoClienteEDI
        {
            get
            {
                int inscricao = 0;
                if (!string.IsNullOrWhiteSpace(IE_RG))
                    int.TryParse(IE_RG, out inscricao);
                else if (Cliente != null)
                    int.TryParse(Cliente.IE_RG, out inscricao);
                return inscricao;
            }
        }

        public virtual ParticipanteCTe Clonar()
        {
            return (ParticipanteCTe)this.MemberwiseClone();
        }
    }
}
