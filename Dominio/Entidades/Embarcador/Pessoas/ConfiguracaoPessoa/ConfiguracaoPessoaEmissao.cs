namespace Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_PESSOA_EMISSAO", DynamicUpdate = true, EntityName = "ConfiguracaoPessoaEmissao", Name = "Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao", NameType = typeof(ConfiguracaoPessoaEmissao))]
    public class ConfiguracaoPessoaEmissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPE_UTILIZAR_PRIMEIRA_UNIDADE_MEDIDA_PESO_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Emissão"; }
        }
    }
}
