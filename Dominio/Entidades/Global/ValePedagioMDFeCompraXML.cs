using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_VALE_PEDAGIO_COMPRA_XML", EntityName = "ValePedagioMDFeCompraXML", Name = "Dominio.Entidades.ValePedagioMDFeCompraXML", NameType = typeof(ValePedagioMDFeCompraXML))]
    public class ValePedagioMDFeCompraXML : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MVX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValePedagioMDFeCompra", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ValePedagioMDFeCompra ValePedagioMDFeCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVX_DATAHORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVX_REQUISICAO", Type = "StringClob", NotNull = false)]
        public virtual string Requisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVX_RESPOSTA", Type = "StringClob", NotNull = false)]
        public virtual string Resposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MVX_TIPO", TypeType = typeof(Enumeradores.TipoXMLValePedagio), NotNull = false)]
        public virtual Enumeradores.TipoXMLValePedagio Tipo { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoXMLValePedagio.BuscarCustoRota :
                        return "Buscar Custo Rota";
                    case Enumeradores.TipoXMLValePedagio.BuscarRotaIBGE :
                        return "Buscar Rota";
                    case Enumeradores.TipoXMLValePedagio.CancelarCompraValePedagio:
                        return "Cancelar Compra";
                    case Enumeradores.TipoXMLValePedagio.ComprarValePedagio:
                        return "Compra Vale Pedágio";
                    case Enumeradores.TipoXMLValePedagio.BuscarCartaoMotorista:
                        return "Buscar Cartão Motorista";
                    case Enumeradores.TipoXMLValePedagio.BuscarDocumento:
                        return "Buscar Documento";
                    case Enumeradores.TipoXMLValePedagio.ConfirmacaoPedagioTAG:
                        return "Confirmação TAG";
                    default:
                        return string.Empty;
                }
            }
        }

    }
}
