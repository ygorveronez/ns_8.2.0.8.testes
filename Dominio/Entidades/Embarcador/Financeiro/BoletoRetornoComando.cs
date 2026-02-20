using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_RETORNO_COMANDO", EntityName = "BoletoRetornoComando", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando", NameType = typeof(BoletoRetornoComando))]
    public class BoletoRetornoComando : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comando", Column = "BRC_COMANDO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string Comando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BRC_DESCRICAO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Liquidacao", Column = "BRC_LIQUIDACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Liquidacao { get; set; }

        public virtual bool Equals(BoletoRetornoComando other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
