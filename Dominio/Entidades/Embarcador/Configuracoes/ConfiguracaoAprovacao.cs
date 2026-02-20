namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_APROVACAO", EntityName = "ConfiguracaoAprovacao", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao", NameType = typeof(ConfiguracaoAprovacao))]
    public class ConfiguracaoAprovacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_PERMITIR_DELEGAR_PARA_USUARIO_COM_TODAS_ALCADAS_REJEITADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_UTILIZAR_ALCADA_APROVACAO_TABELA_FRETE_POR_TABELA_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_CRIAR_APROVACAO_CARGA_AO_CONFIRMAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarAprovacaoCargaAoConfirmarDocumentos { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para aprovações"; }
        }
    }
}
