namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_INTEGRACAO_TRANSSAT", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoIntegracaoTransSat", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoTransSat", NameType = typeof(ConfiguracaoTipoOperacaoIntegracaoTransSat))]
    public class ConfiguracaoTipoOperacaoIntegracaoTransSat : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoTransSat", Column = "CIT_POSSUI_INTEGRACAO_TRANSSAT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoTransSat { get; set; }
        public virtual string Descricao
        {
            get { return "Configurações tipo operação da integração com a TransSat."; }
        }
    }
}