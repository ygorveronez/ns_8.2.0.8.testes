namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_GERAL_EFRETE", EntityName = "IntegracaoGeralEFrete", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete", NameType = typeof(IntegracaoGeralEFrete))]
    public class IntegracaoGeralEFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoRecebivel", Column = "CGE_POSSUI_INTEGRACAO_RECEBIVEL", TypeType = typeof(bool), Length = 200, NotNull = false)]
        public virtual bool PossuiIntegracaoRecebivel { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLRecebivel", Column = "CGE_URL_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLRecebivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CGE_URL_AUTENTICACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLCancelamentoRecebivel", Column = "CGE_URL_CANCELAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLCancelamentoRecebivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPagamentoRecebivel", Column = "CGE_URL_PAGAMENTO_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLPagamentoRecebivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "APIKey", Column = "CGE_API_KEY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string APIKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioRecebivel", Column = "CGE_USUARIO_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioRecebivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaRecebivel", Column = "CGE_SENHA_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaRecebivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoRecebivel", Column = "CGE_CODIGO_INTEGRACAO_RECEBIVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracaoRecebivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_ENVIAR_IMPOSTOS_INTEGRACAO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarImpostosNaIntegracaoDoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_DEDUZIR_IMPOSTOS_VALOR_TOTAL_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeduzirImpostosValorTotalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoEFrete", Column = "CGE_VERSAO_EFRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoEFreteEnum), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoEFreteEnum? VersaoEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_ENVIAR_DADOS_REGULATORIO_ANTT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosRegulatorioANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_CONSULTAR_TAG_AO_INCLUIR_VEICULO_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarTagAoIncluirVeiculoNaCarga { get; set; }
    }
}
