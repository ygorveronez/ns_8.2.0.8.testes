using Dominio.Entidades.Embarcador.Acerto;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANCELAMENTO_CTE", EntityName = "CancelamentoCTe", Name = "Dominio.Entidades.Embarcador.CTe.CancelamentoCTe", NameType = typeof(CancelamentoCTe))]
    public class CancelamentoCTe : EntidadeBase, IEquatable<CancelamentoCTe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoCTeSemCarga", Column = "CCS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga CancelamentoCTeSemCarga{ get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }


        public virtual bool Equals(CancelamentoCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

    
}
