namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_NOTA_FISCAL_CHAMADO", EntityName = "CargaEntregaNotaFiscalChamado", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado", NameType = typeof(CargaEntregaNotaFiscalChamado))]
    public class CargaEntregaNotaFiscalChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaNotaFiscal", Column = "CEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal CargaEntregaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDevolucaoEntrega", Column = "MDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega MotivoDaDevolucao { get; set; }

        /// <summary>
        /// Situação de entrega da nota fiscal que foi marcada no chamado dessa tabela, pois após confirmação da entrega, a situação da nota em si muda e não exibe mais marcado no chamado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CNC_SITUACAO_ENTREGA_NOTA_FISCAL_CHAMADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal? SituacaoEntregaNotaFiscalChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNC_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNC_DEVOLUCAO_TOTAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoTotal { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}