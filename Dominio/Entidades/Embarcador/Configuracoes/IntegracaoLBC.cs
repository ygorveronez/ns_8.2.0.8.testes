namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_LBC", EntityName = "IntegracaoLBC", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC", NameType = typeof(IntegracaoLBC))]
    public class IntegracaoLBC : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_POSSUI_INTEGRACAO_LBC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoLBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_UTILIZAR_VALOR_PADRAO_PARA_CAMPOS_NULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorPadraoParaCamposNulos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoLBC", Column = "CIL_URL_INTEGRACAO_LBC", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLIntegracaoLBC { get; set; }     
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoLBCAnexo", Column = "CIL_URL_INTEGRACAO_LBC_ANEXO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLIntegracaoLBCAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioLBC", Column = "CIL_USUARIO_LBC", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioLBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaLBC", Column = "CIL_SENHA_LBC", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaLBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoLBCCustoFixo", Column = "CIL_URL_INTEGRACAO_LBC_CUSTO_FIXO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLIntegracaoLBCCustoFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoLBCTabelaFreteCliente", Column = "CIL_URL_INTEGRACAO_LBC_TABELA_FRETE_CLIENTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLIntegracaoLBCTabelaFreteCliente { get; set; }
    }
}
