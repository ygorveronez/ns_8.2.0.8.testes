using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleColeta
{
    [Obsolete("O controle de coleta não existe mais na aplicação")]
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_COLETA_NOTA_FISCAL", EntityName = "CargaColetaNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.ControleColeta.CargaColetaNotaFiscal", NameType = typeof(CargaColetaNotaFiscal))]
    public class CargaColetaNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaColeta", Column = "CCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaColeta CargaColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
