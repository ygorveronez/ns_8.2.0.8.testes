
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ELECTROLUX", EntityName = "IntegracaoElectrolux", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux", NameType = typeof(IntegracaoElectrolux))]
    public class IntegracaoElectrolux : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIE_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIE_USUARIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIE_SENHA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentCarrierSp", Column = "CIE_IDENT_CARRIER_SP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IdentCarrierSp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentCarrierPr", Column = "CIE_IDENT_CARRIER_PR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IdentCarrierPr { get; set; }

        /// URL OCOREN
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlOcorenService", Column = "CIE_URL_OCOREN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlOcorenService { get; set; }

        /// <summary>
        /// URL NOTFIS - Busca lista de pendentes
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlNotfisLista", Column = "CIE_URL_NOTFIS_LISTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlNotfisLista { get; set; }

        /// <summary>
        /// URL NOTFIS - Busca lista detalhada
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlNotfisDetalhada", Column = "CIE_URL_NOTFIS_DETALHADA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlNotfisDetalhada { get; set; }

        #region OBSOLETO

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLCONEMB", Column = "CIE_URL_CONEMB", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCONEMB { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO_CONEMB", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDICONEMB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLOCORREN", Column = "CIE_URL_OCORREN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLOCORREN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLNOTFIS", Column = "CIE_URL_NOTFIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLNOTFIS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO_OCORREN", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDIOCORREN { get; set; }

        #endregion

        /// <summary>
        /// Valida se possui integração para electrolux
        /// </summary>
        public virtual bool ElectroluxPossuiIntegracao
        {
            get { return (PossuiIntegracao || !string.IsNullOrWhiteSpace(Usuario) || !string.IsNullOrWhiteSpace(Senha)); }
        }

        public virtual string DefinirIdentCarrier(string uFOrigemTransportador)
        {
            var result = "PR";

            switch (uFOrigemTransportador)
            {
                case "SP": result = IdentCarrierSp; break;
                case "PR": result = IdentCarrierPr; break;
                default: break;
            }

            return result;
        }

    }
}