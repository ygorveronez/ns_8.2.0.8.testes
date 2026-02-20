using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_DESCARREGAMENTO_PEDIDO", EntityName = "CargaJanelaDescarregamentoPedido", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido", NameType = typeof(CargaJanelaDescarregamentoPedido))]
    public class CargaJanelaDescarregamentoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaDescarregamento", Column = "CJD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaDescarregamento CargaJanelaDescarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcaoParcial", Column = "JDP_TIPO_ACAO_PARCIAL", TypeType = typeof(TipoAcaoParcial), NotNull = true)]
        public virtual TipoAcaoParcial TipoAcaoParcial { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "JDP_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }
    }
}
