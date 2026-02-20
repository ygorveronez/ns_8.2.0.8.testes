using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RESPONSAVEL_VEICULO", EntityName = "ResponsavelVeiculo", Name = "Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo", NameType = typeof(ResponsavelVeiculo))]
    public class ResponsavelVeiculo : EntidadeBase, IEquatable<ResponsavelVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "REV_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "REV_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_LANCAMENTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioResponsavel { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Placa_Formatada + " - " + this.FuncionarioResponsavel?.Nome;
            }
        }

        public virtual bool Equals(ResponsavelVeiculo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
