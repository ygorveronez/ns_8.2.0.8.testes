namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INFRACOES", EntityName = "ConfiguracaoInfracoes", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes", NameType = typeof(ConfiguracaoInfracoes))]
    public class ConfiguracaoInfracoes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CI_FORMULA_INFRACAO_PADRAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string FormulaInfracaoPadrao { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para infrações"; }
        }
    }
}
