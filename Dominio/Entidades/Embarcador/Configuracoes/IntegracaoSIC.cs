namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SIC", EntityName = "IntegracaoSIC", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC", NameType = typeof(IntegracaoSIC))]
    public class IntegracaoSIC : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoSIC", Column = "CIS_POSSUI_INTEGRACAO_SIC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarIntegracaoNovosCadastrosPessoaSIC", Column = "CIS_REALIZA_INTEGRACAO_NOVOS_CAD_PESSOA_SIC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoNovosCadastrosPessoaSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoSIC", Column = "CIS_URL", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string URLIntegracaoSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LoginSIC", Column = "CIS_LOGIN", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string LoginSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSIC", Column = "CIS_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string SenhaSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCadastroVeiculoSIC", Column = "CIS_TIPO_CADASTRO_VEICULO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoCadastroVeiculoSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCadastroMotoristaSIC", Column = "CIS_TIPO_CADASTRO_MOTORISTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoCadastroMotoristaSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCadastroClientesSIC", Column = "CIS_TIPO_CADASTRO_CLIENTES", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoCadastroClientesSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCadastroTransportadoresTerceirosSIC", Column = "CIS_TIPO_CADASTRO_TRANSPORTADORES_TERCEIROS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoCadastroTransportadoresTerceirosSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmpresaSIC", Column = "CIS_EMPRESA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EmpresaSIC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCadastroClientesTerceirosSIC", Column = "CIS_TIPO_CADASTRO_CLIENTES_TERCEIROS", TypeType = typeof(bool), Length = 200, NotNull = false)]
        public virtual bool TipoCadastroClientesTerceirosSIC { get; set; }


    }
}
