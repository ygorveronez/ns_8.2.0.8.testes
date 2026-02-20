namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_INTEGRACAO_DIAGEO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoIntegracaoDiageo", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoDiageo", NameType = typeof(ConfiguracaoTipoOperacaoIntegracaoDiageo))]
    public class ConfiguracaoTipoOperacaoIntegracaoDiageo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoDiageo", Column = "CID_POSSUI_INTEGRACAO_DIAGEO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoDiageo { get; set; }
        public virtual string Descricao
        {
            get { return "Configurações tipo operação da integração com a Diageo."; }
        }
    }
}