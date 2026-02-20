using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_RETORNO_ARQUIVO", EntityName = "BoletoRetornoArquivo", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo", NameType = typeof(BoletoRetornoArquivo))]
    public class BoletoRetornoArquivo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BRA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "BRA_ARQUIVO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "BRA_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoletoStatusTitulo", Column = "BRA_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo BoletoStatusTitulo { get; set; }

        public virtual bool Equals(BoletoRetornoArquivo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
