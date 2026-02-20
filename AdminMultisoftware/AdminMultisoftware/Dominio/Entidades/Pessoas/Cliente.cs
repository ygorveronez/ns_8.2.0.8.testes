using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE", EntityName = "Cliente", Name = "AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente", NameType = typeof(Cliente))]
    public class Cliente : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "CLI_CNPJ", TypeType = typeof(string), Length = 15, NotNull = true)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoEstadual", Column = "CLI_IE", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string InscricaoEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RazaoSocial", Column = "CLI_RAZAO_SOCIAL", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string RazaoSocial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperadora", Column = "CLI_TIPO_OPERADORA", TypeType = typeof(Dominio.Enumeradores.TipoOperadora), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoOperadora TipoOperadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "CLI_NOME_FANTASIA", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string NomeFantasia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DDDTelefone", Column = "CLI_DDD_TELEFONE", TypeType = typeof(string), Length = 2, NotNull = true)]
        public virtual string DDDTelefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CLI_TELEFONE", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "CLI_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CLI_EMAIL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Logradouro", Column = "CLI_LOGRADOURO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Logradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "CLI_BAIRRO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteConfiguracao", Column = "CCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.ClienteConfiguracao ClienteConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteConfiguracao", Column = "CCF_CODIGO_REPORT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.ClienteConfiguracao ClienteConfiguracaoReport { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteConfiguracao", Column = "CCF_CODIGO_HOMOLOGACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.ClienteConfiguracao ClienteConfiguracaoHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteConfiguracao", Column = "CCF_CODIGO_HOMOLOGACAO_REPORT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.ClienteConfiguracao ClienteConfiguracaoHomologacaoReport { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "LOC_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "LOC_COMPLEMENTO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "EMP_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CRL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlMobile", Column = "CLI_MOBILE_URL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UrlMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlMobileHomologacao", Column = "CLI_MOBILE_URL_HOMOLOGACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UrlMobileHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Logo", Column = "CLI_LOGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Logo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogoLight", Column = "CLI_LOGO_LIGHT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LogoLight { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HeightLogo", Column = "CLI_HEIGHT_LOGO", TypeType = typeof(int), NotNull = false)]
        public virtual int HeightLogo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Site", Column = "CLI_SITE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Site { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearLoginVersaoAntigaAPP", Column = "CRL_BLOQUEAR_LOGIN_VERSAO_ANTIGA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearLoginVersaoAntigaAPP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticadaViaCodigoDeIntegracaoDoUsuarioParaPortalMultiClifor", Column = "CLI_URL_AUTENTICADA_VIA_CODIGO_DE_INTEGRACAO_DO_USUARIO_PARA_PORTAL_MULTI_CLIFOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool URLAutenticadaViaCodigoDeIntegracaoDoUsuarioParaPortalMultiClifor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ClienteURLsAcesso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_URL_ACESSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteURLAcesso", Column = "CRL_CODIGO")]
        public virtual IList<Dominio.Entidades.Pessoas.ClienteURLAcesso> ClienteURLsAcesso { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "NsAppsClientId", Column = "CLI_NSAPPS_CLIENT_ID", TypeType = typeof(string), NotNull = false)]
        public virtual string NsAppsClientId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NsAppsClientSecret", Column = "CLI_NSAPPS_CLIENT_SECRET", TypeType = typeof(string), NotNull = false)]
        public virtual string NsAppsClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NsAppsAppPublicKey", Column = "CLI_NSAPPS_APP_PUB_KEY", TypeType = typeof(string), NotNull = false)]
        public virtual string NsAppsAppPublicKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NsAppsCompanyId", Column = "CLI_NSAPPS_COMPANY_ID", TypeType = typeof(string), NotNull = false)]
        public virtual string NsAppsCompanyId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cabotagem", Column = "CLI_CABOTAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cabotagem { get; set; }



        public virtual string Descricao
        {
            get { return !string.IsNullOrWhiteSpace(NomeFantasia) ? NomeFantasia : string.Empty; }
        }

        /// <summary>
        /// Retorna o CNPJ no formato 99.999.999/9999-99
        /// </summary>
        public virtual string CNPJFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CNPJ) ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ)) : string.Empty; }
        }

        /// <summary>
        /// Seta o CNPJ no padrão 99999999999999 para ser salvo no Banco de dados
        /// </summary>
        public virtual string CNPJSemFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CNPJ) ? string.Format(@"{0:00000000000000}", long.Parse(this.CNPJ)) : string.Empty; }
        }

        public virtual string AtivoFormatado
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual string TelefoneFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DDDTelefone) || string.IsNullOrWhiteSpace(Telefone))
                    return string.Empty;

                string telefone = DDDTelefone + Telefone;

                if (telefone.Length == 11)
                    return String.Format(@"{0:(00) 00000-0000}", long.Parse(telefone));
                return String.Format(@"{0:(00) 0000-0000}", long.Parse(telefone));
            }
        }

        public virtual string CEPFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CEP) ? string.Format(@"{0:00\.000\-000}", long.Parse(this.CEP)) : string.Empty; }
        }

        public virtual bool Equals(Cliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
