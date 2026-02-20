namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_NOTA_FISCAL", EntityName = "CargaEntregaNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal", NameType = typeof(CargaEntregaNotaFiscal))]
    public class CargaEntregaNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDevolucaoEntrega", Column = "MDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega MotivoDaDevolucao { get; set; }

        /// <summary>
        /// Situação de entrega da nota fiscal que foi marcada no chamado dessa tabela, pois após confirmação da entrega, a situação da nota em si muda e não exibe mais marcado no chamado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEF_SITUACAO_ENTREGA_NOTA_FISCAL_CHAMADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal? SituacaoEntregaNotaFiscalChamado { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}