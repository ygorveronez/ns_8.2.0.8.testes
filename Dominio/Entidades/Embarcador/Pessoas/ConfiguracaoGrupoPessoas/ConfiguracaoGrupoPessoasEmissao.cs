namespace Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_GRUPO_PESSOAS_EMISSAO", DynamicUpdate = true, EntityName = "ConfiguracaoGrupoPessoasEmissao", Name = "Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao", NameType = typeof(ConfiguracaoGrupoPessoasEmissao))]
    public class ConfiguracaoGrupoPessoasEmissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_UTILIZAR_PRIMEIRA_UNIDADE_MEDIDA_PESO_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Emissão"; }
        }
    }
}
