namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_ALTERACAO", EntityName = "TabelaFreteAlteracao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao", NameType = typeof(TabelaFreteAlteracao))]
    public class TabelaFreteAlteracao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAlteracao", Column = "TFA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete SituacaoAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Alteração nº {Numero} da tabela de frete {TabelaFrete.Descricao}";
            }
        }
    }
}
