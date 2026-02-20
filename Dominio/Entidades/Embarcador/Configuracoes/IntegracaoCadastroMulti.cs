namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CADASTRO_MULTI", EntityName = "IntegracaoCadastroMulti", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti", NameType = typeof(IntegracaoCadastroMulti))]
    public class IntegracaoCadastroMulti : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICM_ENVIAR_DOCUMENTACAO_CTE_AVERBACAO_INSTANCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentacaoCTeAverbacaoInstancia { get; set; }

        public virtual string Descricao 
        { 
            get { return "Configuração integração - Cadastros Multi"; }
        }
    }
}
