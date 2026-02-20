using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_TEMPORARIO", EntityName = "CargaPedidoXMLNotaFiscalTemporario", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario", NameType = typeof(CargaPedidoXMLNotaFiscalTemporario))]
    public class CargaPedidoXMLNotaFiscalTemporario : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CPT_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "CPT_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        public virtual string Descricao => $"Pedido: {Pedido.CodigoCargaEmbarcador} - Número: {Numero}";

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CPT_STATUS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPT_NUMERO_FATURA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NumeroFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPT_TIPO_NOTA_FISCAL_INTEGRADA", TypeType = typeof(TipoNotaFiscalIntegrada), NotNull = false)]
        public virtual TipoNotaFiscalIntegrada TipoNotaFiscalIntegrada { get; set; }

        public virtual bool Equals(CargaPedidoXMLNotaFiscalTemporario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}