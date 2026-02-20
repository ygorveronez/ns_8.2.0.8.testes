using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FOLHA_LANCAMENTO", EntityName = "FolhaLancamento", Name = "Dominio.Entidades.Embarcador.RH.FolhaLancamento", NameType = typeof(FolhaLancamento))]
    public class FolhaLancamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.FolhaLancamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FOL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEvento", Column = "FOL_NUMERO_EVENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "FOL_NUMERO_CONTRATO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "FOL_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "FOL_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FOL_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Base", Column = "FOL_BASE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Base { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Referencia", Column = "FOL_REFERENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Referencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FOL_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FolhaInformacao", Column = "FOI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FolhaInformacao FolhaInformacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompetencia", Column = "FOL_DATA_COMPETENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouMovimentoFinanceiro", Column = "FOL_GEROU_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouMovimentoFinanceiro { get; set; }

        public virtual bool Equals(FolhaLancamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
