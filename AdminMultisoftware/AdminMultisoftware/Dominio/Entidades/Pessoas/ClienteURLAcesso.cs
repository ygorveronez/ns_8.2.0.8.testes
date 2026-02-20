using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_URL_ACESSO", EntityName = "ClienteURLAcesso", Name = "AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso", NameType = typeof(ClienteURLAcesso))]
    public class ClienteURLAcesso : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultisoftware", Column = "CRL_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAcesso", Column = "CRL_URL_ACESSO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string URLAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UriSistemaEmissaoCTe", Column = "CRL_URI_SISTEMA_EMISSAO_CTE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string UriSistemaEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WebServiceConsultaCTe", Column = "CRL_URL_WEBSERVICE_CONSULTA_CTE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string WebServiceConsultaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WebServiceOracle", Column = "CRL_URL_WEBSERVICE_ORACLE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string WebServiceOracle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CRL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLHomologacao", Column = "CRL_URL_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool URLHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiFila", Column = "CRL_POSSUI_FILA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiFila { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Layout", Column = "CRL_LAYOUT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Layout { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Logo", Column = "CRL_LOGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Logo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorFundoUsuario", Column = "CRL_COR_FUNCIONARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CorFundoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Favicon", Column = "CRL_ICONE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Favicon { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LayoutLogin", Column = "CRL_LAYOUT_LOGIN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LayoutLogin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogoLogin", Column = "CRL_LOGO_LOGIN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LogoLogin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorFundoUsuarioLogin", Column = "CRL_COR_FUNCIONARIO_LOGIN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CorFundoUsuarioLogin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaviconLogin", Column = "CRL_ICONE_LOGIN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FaviconLogin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LayoutLoginPersonalizado", Column = "CRL_LAYOUT_LOGIN_PERSONALIZADO", TypeType = typeof(string), NotNull = false)]
        public virtual string LayoutLoginPersonalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LayoutLoginPersonalizadoTransportador", Column = "CRL_LAYOUT_LOGIN_PERSONALIZADO_TRANSPORTADOR", TypeType = typeof(string), NotNull = false)]
        public virtual string LayoutLoginPersonalizadoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemBoasVindasPersonalizada", Column = "CRL_MENSAGEM_BOAS_VINDAS_PERSONALIZADA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MensagemBoasVindasPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemBoasVindasPersonalizadaTransportador", Column = "CRL_MENSAGEM_BOAS_VINDAS_PERSONALIZADA_TRANSPORTADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MensagemBoasVindasPersonalizadaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogoClienteTerceiroLight", Column = "CRL_LOGO_CLIENTE_TERCEIRO_LIGHT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LogoClienteTerceiroLight { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogoClienteTerceiroDark", Column = "CRL_LOGO_CLIENTE_TERCEIRO_DARK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LogoClienteTerceiroDark { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoExecucao", Column = "CRL_TIPO_EXECUCAO", TypeType = typeof(Dominio.Enumeradores.TipoExecucao), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoExecucao TipoExecucao { get; set; }
      
  


        #endregion Propriedades

        #region Métodos Públicos

        public virtual string Descricao
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual string AtivoFormatado
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual string DescricaoTipoServicoMultisoftware
        {
            get
            {
                switch (this.TipoServicoMultisoftware)
                {
                    case Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador:
                        return "Multi Embarcador";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS:
                        return "Multi TMS";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe:
                        return "Multi CT-e";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.CallCenter:
                        return "Call Center";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros:
                        return "Terceiros";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe:
                        return "Multi NF-e";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin:
                        return "Multi NF-e Admin";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor:
                        return "Fornecedor";
                    case Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile:
                        return "Multi Mobile";
                    default:
                        return string.Empty;

                }
            }
        }

        #endregion Métodos Públicos
    }
}
