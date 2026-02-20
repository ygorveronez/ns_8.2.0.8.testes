using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_DESPESAS", EntityName = "DespesaDoAcertoDeViagem", Name = "Dominio.Entidades.DespesaDoAcertoDeViagem", NameType = typeof(DespesaDoAcertoDeViagem))]
    public class DespesaDoAcertoDeViagem : EntidadeBase, IEquatable<Dominio.Entidades.DespesaDoAcertoDeViagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesa", Column = "ACT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDespesa TipoDespesa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoDeViagem", Column = "ACE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcertoDeViagem AcertoDeViagem { get; set; }

        public virtual int NumeroAcerto
        {
            get
            {
                return AcertoDeViagem?.Numero ?? 0;
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ACD_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "ACD_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "ACD_VALORUNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ACD_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ACD_OBS", TypeType = typeof(string), NotNull = false, Length = 3000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Paga", Column = "ACD_PAGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Paga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFornecedor", Column = "ACD_NOME_FORNECEDOR", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string NomeFornecedor { get; set; }

        public virtual string DescricaoFornecedor
        {
            get
            {
                if (this.Fornecedor != null)
                {
                    return string.Concat(this.Fornecedor.CPF_CNPJ_Formatado, " - ", this.Fornecedor.Nome);
                }
                else
                {
                    return this.NomeFornecedor;
                }
            }
        }

        public virtual string DescricaoDespesa
        {
            get
            {
                if (this.TipoDespesa != null)
                {
                    return this.TipoDespesa.Descricao;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                return this.Quantidade * this.ValorUnitario;
            }
        }


        public virtual bool Equals(DespesaDoAcertoDeViagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
