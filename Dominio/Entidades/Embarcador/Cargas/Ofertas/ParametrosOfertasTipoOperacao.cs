using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Cargas.Ofertas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETROS_OFERTAS_TIPO_OPERACAO", EntityName = "ParametrosOfertasTipoOperacao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.ParametrosOfertasTipoOperacao", NameType = typeof(ParametrosOfertasTipoOperacao))]
    public class ParametrosOfertasTipoOperacao : EntidadeBase, Interfaces.Embarcador.Cargas.Ofertas.IRelacionamentoParametrosOfertas
    {
        public virtual string Descricao
        {
            get { return TipoOperacao.Descricao; }
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametrosOfertas", Column = "POF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametrosOfertas ParametrosOfertas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }
    }
}
