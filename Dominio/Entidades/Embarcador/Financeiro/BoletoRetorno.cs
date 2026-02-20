using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_RETORNO", EntityName = "BoletoRetorno", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno", NameType = typeof(BoletoRetorno))]
    public class BoletoRetorno : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoRetorno>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comando", Column = "BRT_COMANDO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string Comando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumero", Column = "BRT_NOSSO_NUMERO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NossoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBanco", Column = "BRT_CODIGO_BANCO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "BRT_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiquidacao", Column = "BRT_DATA_LIQUIDACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataLiquidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetorno", Column = "BRT_VALOR_RETORNO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTitulo", Column = "BRT_VALOR_TITULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juros", Column = "BRT_JUROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Juros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Outros", Column = "BRT_OUTROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Outros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tarifa", Column = "BRT_TARIFA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Tarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRecebido", Column = "BRT_VALOR_RECEBIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRecebido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCredito", Column = "BRT_DATA_CREDITO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRejeicao", Column = "BRT_CODIGO_REJEICAO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaixa", Column = "BRT_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataArquivo", Column = "BRT_DATA_ARQUIVO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Registrada", Column = "BRT_REGISTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Registrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteTarifa", Column = "BRT_SOMENTE_TARIFA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteTarifa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoRetornoComando", Column = "BRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando BoletoRetornoComando { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoRetornoArquivo", Column = "BRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo BoletoRetornoArquivo { get; set; }

        public virtual bool Equals(BoletoRetorno other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
