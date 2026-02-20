using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MOVIMENTACAO_PNEU_DADOS_ADICIONAIS", EntityName = "Frota.MovimentacaoPneuVeiculoDadosAdicionais", Name = "Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculoDadosAdicionais", NameType = typeof(MovimentacaoPneuVeiculoDadosAdicionais))]
    public class MovimentacaoPneuVeiculoDadosAdicionais : EntidadeBase, IEquatable<MovimentacaoPneuVeiculoDadosAdicionais>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MilimetragemUm", Column = "MDA_MILIMETRAGEM_UM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal MilimetragemUm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MilimetragemDois", Column = "MDA_MILIMETRAGEM_DOIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal MilimetragemDois { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MilimetragemTres", Column = "MDA_MILIMETRAGEM_TRES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal MilimetragemTres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MilimetragemQuatro", Column = "MDA_MILIMETRAGEM_QUATRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal MilimetragemQuatro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MilimetragemMedia", Column = "MDA_MILIMETRAGEM_MEDIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal MilimetragemMedia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDA_CALIBRAGEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Calibragem { get; set; }

        public virtual bool Equals(MovimentacaoPneuVeiculoDadosAdicionais other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
