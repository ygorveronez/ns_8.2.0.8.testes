using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHEQUE", EntityName = "Cheque", Name = "Dominio.Entidades.Embarcador.Financeiro.Cheque", NameType = typeof(Cheque))]
    public class Cheque : EntidadeBase, IEquatable<Cheque>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHQ_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "CHQ_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompensacao", Column = "CHQ_DATA_COMPENSACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompensacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTransacao", Column = "CHQ_DATA_TRANSACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "CHQ_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "CHQ_DIGITO_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAgencia", Column = "CHQ_NUMERO_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCheque", Column = "CHQ_NUMERO_CHEQUE", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string NumeroCheque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "CHQ_NUMERO_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CHQ_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHQ_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusCheque), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusCheque Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHQ_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCheque), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCheque Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CHQ_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoCompensacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHEQUE_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHQ_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChequeAnexo", Column = "ANX_CODIGO")]
        public virtual IList<ChequeAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_REPASSE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente PessoaRepasse { get; set; }

        public virtual string Descricao
        {
            get { return NumeroCheque; }
        }

        public virtual bool Equals(Cheque other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
