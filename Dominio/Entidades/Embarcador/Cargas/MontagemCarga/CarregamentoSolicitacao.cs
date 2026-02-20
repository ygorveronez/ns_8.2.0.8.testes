namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_SOLICITACAO", EntityName = "CarregamentoSolicitacao", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao", NameType = typeof(CarregamentoSolicitacao))]
    public class CarregamentoSolicitacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRS_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CRS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamentoSolicitacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamentoSolicitacao Situacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Solicitação nº {Numero} de gerar carga do carregamento {Carregamento.NumeroCarregamento}";
            }
        }
    }
}
