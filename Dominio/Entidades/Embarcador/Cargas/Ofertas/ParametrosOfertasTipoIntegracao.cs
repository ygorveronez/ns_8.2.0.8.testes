using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Cargas.Ofertas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETROS_OFERTAS_TIPO_INTEGRACAO", EntityName = "ParametrosOfertasTipoIntegracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.ParametrosOfertasTipoIntegracao", NameType = typeof(ParametrosOfertasTipoIntegracao))]
    public class ParametrosOfertasTipoIntegracao : EntidadeBase, Interfaces.Embarcador.Cargas.Ofertas.IRelacionamentoParametrosOfertas
    {
        public virtual string Descricao
        {
            get { return TipoIntegracao.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametrosOfertas", Column = "POF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametrosOfertas ParametrosOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTI_TIPO_INTEGRACAO", TypeType = typeof(int), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }
    }
}
