namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_EMP", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoEMP", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEMP", NameType = typeof(ConfiguracaoTipoOperacaoEMP))]
    public class ConfiguracaoTipoOperacaoEMP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTE_ATIVAR_INTEGRACAO_COM_SIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoComSIL { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações EMP"; }
        }
    }
}