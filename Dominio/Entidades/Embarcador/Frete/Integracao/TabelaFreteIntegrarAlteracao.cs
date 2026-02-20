using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_INTEGRAR_ALTERACAO", EntityName = "TabelaFreteIntegrarAlteracao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao", NameType = typeof(TabelaFreteIntegrarAlteracao))]
    public class TabelaFreteIntegrarAlteracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TIA_SITUACAO", TypeType = typeof(SituacaoTabelaFreteIntegrarAlteracao), NotNull = true)]
        public virtual SituacaoTabelaFreteIntegrarAlteracao Situacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Alteração {Numero} da tabela de frete {TabelaFrete.Descricao}";
            }
        }
    }
}
