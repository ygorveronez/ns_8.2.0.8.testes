using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_FUNCIONARIO", EntityName = "PessoaFuncionario", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario", NameType = typeof(PessoaFuncionario))]
    public class PessoaFuncionario : EntidadeBase, IEquatable<PessoaFuncionario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "PFU_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), Scale = 5, Precision = 15, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "PFU_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigencia", Column = "PFU_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        public virtual bool Equals(PessoaFuncionario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
