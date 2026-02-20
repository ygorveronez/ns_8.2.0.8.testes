namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CANCELAMENTO_SOLICITACAO", EntityName = "CargaCancelamentoSolicitacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao", NameType = typeof(CargaCancelamentoSolicitacao))]
    public class CargaCancelamentoSolicitacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCS_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CCS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCancelamentoSolicitacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCancelamentoSolicitacao Situacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Solicitação nº {Numero} de cancelamento da carga {CargaCancelamento.Carga.CodigoCargaEmbarcador}";
            }
        }
    }
}
