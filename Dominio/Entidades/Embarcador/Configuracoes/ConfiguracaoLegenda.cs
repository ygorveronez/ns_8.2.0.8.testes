namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_LEGENDA", EntityName = "ConfiguracaoLegenda", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda", NameType = typeof(ConfiguracaoLegenda))]
    public class ConfiguracaoLegenda : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLG_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLG_CODIGO_CONTROLE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CodigoControleLegenda), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CodigoControleLegenda CodigoControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Exibir", Column = "CLG_EXIBIR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Exibir { get; set; }
    }
}
