using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Cargas.Ofertas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETROS_OFERTAS_FILIAL", EntityName = "ParametrosOfertasFilial", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.ParametrosOfertasFilial", NameType = typeof(ParametrosOfertasFilial))]
    public class ParametrosOfertasFilial : EntidadeBase, Interfaces.Embarcador.Cargas.Ofertas.IRelacionamentoParametrosOfertas
    {
        public virtual string Descricao
        {
            get { return Filial.Descricao; }
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametrosOfertas", Column = "POF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametrosOfertas ParametrosOfertas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }
    }
}
