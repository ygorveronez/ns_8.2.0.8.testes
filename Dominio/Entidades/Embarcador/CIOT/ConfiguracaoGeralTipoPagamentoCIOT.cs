using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACOES_TIPO_PAGAMENTO_CIOT", EntityName = "ConfiguracaoGeralTipoPagamentoCIOT", Name = "Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT", NameType = typeof(ConfiguracaoGeralTipoPagamentoCIOT))]
    public class ConfiguracaoGeralTipoPagamentoCIOT : EntidadeBase, IEquatable<ConfiguracaoGeralTipoPagamentoCIOT>
    {
        public ConfiguracaoGeralTipoPagamentoCIOT() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPC_PAGAMENTO_CIOT", TypeType = typeof(TipoPagamentoCIOT), NotNull = false)]
        public virtual TipoPagamentoCIOT TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Operadora", Column = "TPC_OPERADORA", TypeType = typeof(OperadoraCIOT), NotNull = false)]
        public virtual OperadoraCIOT Operadora { get; set; }

        public virtual bool Equals(ConfiguracaoGeralTipoPagamentoCIOT other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
