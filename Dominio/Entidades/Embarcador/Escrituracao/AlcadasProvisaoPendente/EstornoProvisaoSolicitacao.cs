namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESTORNO_PROVISAO_SOLICITACAO", EntityName = "EstornoProvisaoSolicitacao", Name = "Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao", NameType = typeof(EstornoProvisaoSolicitacao))]
    public class EstornoProvisaoSolicitacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EPS_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoProvisao", Column = "CPV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CancelamentoProvisao EstornoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "EPS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoEstornoProvisaoSolicitacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEstornoProvisaoSolicitacao Situacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Solicitação nº {Numero} de cancelamento da provisão {EstornoProvisao.Numero}";
            }
        }
    }
}
