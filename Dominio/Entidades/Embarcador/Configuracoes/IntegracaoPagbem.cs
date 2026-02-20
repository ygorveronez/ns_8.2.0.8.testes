namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_PAGBEM", EntityName = "IntegracaoPagbem", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem", NameType = typeof(IntegracaoPagbem))]
    public class IntegracaoPagbem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPagbem", Column = "CIP_URL_PAGBEM", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioPagbem", Column = "CIP_USUARIO_PAGBEM", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string UsuarioPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaPagbem", Column = "CIP_SENHA_PAGBEM", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SenhaPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJEmpresaContratante", Column = "CIP_CNPJ_EMPRESA_CONTRATANTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CNPJEmpresaContratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_INTEGRAR_NUMERO_RPS_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarNumeroRPSNFSE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_LIBERAR_VIAGEM_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarViagemManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixosPadraoValePedagio", Column = "CIP_QUANTIDADE_EIXO_PADRAO_VALE_PEDAGIO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixosPadraoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_CONSULTAR_VEICULO_SEM_PARAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarVeiculoSemParar { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Pagbem";
            }
        }

    }
}
