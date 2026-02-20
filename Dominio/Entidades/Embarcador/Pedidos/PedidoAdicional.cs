using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_ADICIONAL", EntityName = "PedidoAdicional", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional", NameType = typeof(PedidoAdicional))]
    public class PedidoAdicional : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDetalhe", Column = "TDE_CODIGO_PROC_ESPEC", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe ProcessamentoEspecial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDetalhe", Column = "TDE_CODIGO_HOR_ENTR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe HorarioEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoICT", Column = "PAD_NUMERO_PEDIDO_ICT", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoICT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CondicaoExpedicao", Column = "PAD_CONDICAO_ESPECIAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CondicaoExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoFreteMaterial", Column = "PAD_GRUPO_FRETE_MATERIAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string GrupoFreteMaterial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RestricaoEntrega", Column = "PAD_RESTRICAO_ENTREGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string RestricaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacaoRemessa", Column = "PAD_DATA_CRIACAO_REMESSA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacaoVenda", Column = "PAD_DATA_CRIACAO_VENDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorPOF", Column = "PAD_INDICADOR_POF", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IndicadorPOF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ISISReturn", Column = "PAD_ISIS_RETURN", TypeType = typeof(int), NotNull = false)]
        public virtual int ISISReturn { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDetalhe", Column = "TDE_CODIGO_ZONA_TRANSPORTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe ZonaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDetalhe", Column = "TDE_CODIGO_PERIODO_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe PeriodoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDetalhe", Column = "TDE_CODIGO_DETALHE_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe DetalheEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoVolumoso", Column = "PAD_PRODUTO_VOLUMOSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProdutoVolumoso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicativoColetaEntrega", Column = "PAD_INDICATIVO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.IndicativoColetaEntrega), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.IndicativoColetaEntrega IndicativoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "PAD_TIPO_SERVICO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAutorizacaoColetaEntrega", Column = "PAD_NUMERO_AUTORIZACAO_COLETA_ENTREGA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroAutorizacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_CLIENTE_PROPOSTA_COMERCIAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClientePropostaComercial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSeguro", Column = "PAD_TIPO_SEGURO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TipoSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOSMae", Column = "PAD_NUMERO_OS_MAE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroOSMae { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AjudanteCarga", Column = "PAD_AJUDANTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjudanteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdAjudantesCarga", Column = "PAD_QUANTIDADE_AJUDANTE_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdAjudantesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AjudanteDescarga", Column = "PAD_AJUDANTE_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjudanteDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdAjudantesDescarga", Column = "PAD_QUANTIDADE_AJUDANTE_DESCARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdAjudantesDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExecaoCab", Column = "PAD_EXECAO_CAP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExecaoCab { get; set; }

        [Obsolete("Migrado para uma entidade.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoOrigem", Column = "PAD_NUMERO_PEDIDO_ORIGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroPedidoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_PEDIDO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido PedidoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EssePedidopossuiPedidoBonificacao", Column = "PAD_ESSE_PEDIDO_POSSUI_PEDIDO_BONIFICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EssePedidopossuiPedidoBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EssePedidopossuiPedidoVenda", Column = "PAD_ESSE_PEDIDO_POSSUI_PEDIDO_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EssePedidopossuiPedidoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoVinculado", Column = "PAD_NUMERO_PEDIDO_VINCULADO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroPedidoVinculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoPaletizado", Column = "PAD_PEDIDO_PALETIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PedidoPaletizado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoEstoquePedido", Column = "SEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SituacaoEstoquePedido SituacaoEstoquePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Incoterm", Column = "PAD_INCOTERM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumIncotermPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumIncotermPedido? Incoterm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TransitoAduaneiro", Column = "PAD_TRANSITO_ADUANEIRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TransitoAduaneiro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TransitoAduaneiro? TransitoAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_NOTIFICACAO_CRT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente NotificacaoCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DtaRotaPrazoTransporte", Column = "PAD_DTA_ROTA_PRAZO_TRANSPORTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string DtaRotaPrazoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoEmbalagem", Column = "MRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem TipoEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalheMercadoria", Column = "PAD_DETALHE_MERCADORIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DetalheMercadoria { get; set; }

        public virtual string Descricao => Pedido.Descricao;

        public virtual PedidoAdicional Clonar()
        {
            return (PedidoAdicional)this.MemberwiseClone();
        }
    }
}