namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_TARGET", EntityName = "IntegracaoTarget", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget", NameType = typeof(IntegracaoTarget))]
    public class IntegracaoTarget : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIT_USUARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIT_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIT_TOKEN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FORNECEDOR_VP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_DIAS_PRAZO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasPrazo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_CADASTRAR_ROTA_POR_IBGE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CadastrarRotaPorIBGE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_CADASTRAR_ROTA_POR_COORDENADAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CadastrarRotaPorCoordenadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_NAO_BUSCAR_CARTAO_MOTORISTA_TARGET", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoBuscarCartaoMotoristaTarget { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_CODIGO_CENTRO_CUSTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoCentroCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_PREENCHER_LAT_LONG_DA_ROTA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherLatLongDaRotaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_PREENCHER_PONTOS_PASSAGEM_MODIFICADO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherPontosPassagemModificadoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_NOTIFICAR_TRANSPORTADOR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorPorEmail { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Target";
            }
        }

    }
}
