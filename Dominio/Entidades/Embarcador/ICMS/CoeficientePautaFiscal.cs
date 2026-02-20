using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.ICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COEFICIENTE_PAUTA_FISCAL", EntityName = "CoeficientePautaFiscal", Name = "Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal", NameType = typeof(CoeficientePautaFiscal))]
    public class CoeficientePautaFiscal : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_MES", TypeType = typeof(int), NotNull = true)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_ANO", TypeType = typeof(int), NotNull = true)]
        public virtual int Ano { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_VALOR", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_PERCENTUAL_COEFICIENTE", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCoeficiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_OBSERVACAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CPF_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoMesAno => $"{(Mes < 10 ? "0" : "")}{Mes}/{Ano}";

        public virtual string Descricao => $"{(Estado?.Descricao ?? string.Empty)} - {DescricaoMesAno}";

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
