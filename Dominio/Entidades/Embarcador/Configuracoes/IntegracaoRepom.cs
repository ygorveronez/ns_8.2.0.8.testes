using Dominio.Entidades.Embarcador.Filiais;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_REPOM", EntityName = "IntegracaoRepom", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom", NameType = typeof(IntegracaoRepom))]
    public class IntegracaoRepom : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        #region SOAP

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCliente", Column = "CIR_CODIGO_CLIENTE", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string CodigoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFilial", Column = "CIR_CODIGO_FILIAL", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string CodigoFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssinaturaDigital", Column = "CIR_ASSINATURA_DIGITAL", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string AssinaturaDigital { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoRepom", Column = "CIR_TIPO_INTEGRACAO_REPOM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoRepom), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoRepom TipoIntegracaoRepom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRotaFreteRepom", Column = "CIR_TIPO_ROTA_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFreteRepom), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFreteRepom TipoRotaFreteRepom { get; set; }

        #region REST

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacaoRota", Column = "CIR_URL_AUTENTICACAO_ROTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLAutenticacaoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLRota", Column = "CIR_URL_ROTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIR_CLIENTE_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIR_CLIENTE_SECRET", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLViagem", Column = "CIR_URL_VIAGEM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIR_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIR_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial FilialCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIR_CONSIDERAR_EIXOS_SUSPENSOS_NA_CONSULTA_DO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarEixosSuspensosNaConsultaDoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIR_CONSIDERAR_ROTA_FRETE_DA_CARGA_NO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarRotaFreteDaCargaNoValePedagio { get; set; }

        #endregion REST

        public virtual string Descricao
        {
            get { return "Configuração Integração Repom"; }
        }
    }
}
