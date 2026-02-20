namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_LICENCA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoLicenca", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoLicenca", NameType = typeof(ConfiguracaoTipoOperacaoLicenca))]
    public class ConfiguracaoTipoOperacaoLicenca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarLicencaMotorista", Column = "COL_VALIDAR_LICENCA_MOTORISTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarLicencaMotorista { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações da Licença."; }
        }
    }
}