using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_CONFIGURACAO_MENSAL", EntityName = "ConfiguracaoGeracaoCreditoMensal", Name = "Dominio.Entidades.Embarcador.Creditos.ConfiguracaoGeracaoCreditoMensal", NameType = typeof(ConfiguracaoGeracaoCreditoMensal))]
    public class ConfiguracaoGeracaoCreditoMensal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.ConfiguracaoGeracaoCreditoMensal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RECEBEDOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Creditor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCreditoMensal", Column = "CCM_VALOR_CREDITO_MENSAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCreditoMensal { get; set; }

        public virtual bool Equals(ConfiguracaoGeracaoCreditoMensal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }

}
