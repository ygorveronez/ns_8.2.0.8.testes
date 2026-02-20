using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_ELETRONICO_RETORNO", EntityName = "PagamentoEletronicoRetorno", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno", NameType = typeof(PagamentoEletronicoRetorno))]
    public class PagamentoEletronicoRetorno : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comando", Column = "PER_COMANDO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string Comando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumero", Column = "PER_NOSSO_NUMERO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NossoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBanco", Column = "PER_CODIGO_BANCO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "PER_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "PER_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiquidacao", Column = "PER_DATA_LIQUIDACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiquidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetorno", Column = "PER_VALOR_RETORNO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTitulo", Column = "PER_VALOR_TITULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juros", Column = "PER_JUROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Juros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Outros", Column = "PER_OUTROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Outros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tarifa", Column = "PER_TARIFA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Tarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRecebido", Column = "PER_VALOR_RECEBIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRecebido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCredito", Column = "PER_DATA_CREDITO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRejeicao", Column = "PER_CODIGO_REJEICAO", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaixa", Column = "PER_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataArquivo", Column = "PER_DATA_ARQUIVO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Registrada", Column = "PER_REGISTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Registrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteTarifa", Column = "PER_SOMENTE_TARIFA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteTarifa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agendamento", Column = "PER_AGENDAMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Agendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "PER_NOME_ARQUIVO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(PagamentoEletronicoRetorno other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
