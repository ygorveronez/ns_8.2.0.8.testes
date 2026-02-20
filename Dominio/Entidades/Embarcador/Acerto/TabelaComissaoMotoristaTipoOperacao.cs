namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_COMISSAO_MOTORISTA_TIPO_OPERACAO", EntityName = "TabelaComissaoMotoristaTipoOperacao", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao", NameType = typeof(TabelaComissaoMotoristaTipoOperacao))]
    public class TabelaComissaoMotoristaTipoOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotorista", Column = "TCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotorista TabelaComissaoMotorista { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
