using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_FREQUENCIA_ENTREGA", EntityName = "RotaFreteFrequenciaEntrega", Name = "Dominio.Entidades.RotaFreteFrequenciaEntrega", NameType = typeof(RotaFreteFrequenciaEntrega))]
    public class RotaFreteFrequenciaEntrega : EntidadeBase, IEquatable<RotaFreteFrequenciaEntrega>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFE_DIA_SEMANA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemana { get; set; }


        public virtual bool Equals(RotaFreteFrequenciaEntrega other)
        {
            if (other.Codigo == this.Codigo)
                return true;

            return false;
        }
    }
}
