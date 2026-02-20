using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_GERAL_CIOT", EntityName = "ConfiguracaoGeralCIOT", Name = "Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT", NameType = typeof(ConfiguracaoGeralCIOT))]
    public class ConfiguracaoGeralCIOT : EntidadeBase, IEquatable<ConfiguracaoGeralCIOT>
    {
        public ConfiguracaoGeralCIOT() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_FAVORECIDO_CIOT", TypeType = typeof(TipoFavorecidoCIOT), NotNull = false)]
        public virtual TipoFavorecidoCIOT? TipoFavorecidoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_TIPO_GERACAO_CIOT", TypeType = typeof(TipoGeracaoCIOT), NotNull = false)]
        public virtual TipoGeracaoCIOT? TipoGeracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_TIPO_QUITACAO_CIOT", TypeType = typeof(TipoQuitacaoCIOT), NotNull = false)]
        public virtual TipoQuitacaoCIOT? TipoQuitacaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_TIPO_ADIANTAMENTO_CIOT", TypeType = typeof(TipoQuitacaoCIOT), NotNull = false)]
        public virtual TipoQuitacaoCIOT? TipoAdiantamentoCIOT { get; set; }

        public virtual bool Equals(ConfiguracaoGeralCIOT other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
