using Dominio.Entidades.Embarcador.Pessoas;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_VENDEDORES", EntityName = "TipoOperacaoVendedores", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores", NameType = typeof(TipoOperacaoVendedores))]
    public class TipoOperacaoVendedores : EntidadeBase, IEquatable<TipoOperacaoVendedores>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "TOV_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), Scale = 5, Precision = 15, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "TOV_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigencia", Column = "TOV_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimVigencia { get; set; }


        public virtual bool Equals(TipoOperacaoVendedores other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }

}
