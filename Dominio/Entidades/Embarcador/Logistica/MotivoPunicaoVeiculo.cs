using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MOTIVO_PUNICAO", EntityName = "MotivoPunicaoVeiculo", Name = "Dominio.Entidades.Embarcador.Logistica.MotivoPunicaoVeiculo", NameType = typeof(MotivoPunicaoVeiculo))]
    public class MotivoPunicaoVeiculo : EntidadeBase, IEquatable<MotivoPunicaoVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VMP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "VMP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "VMP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get  { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoPunicaoVeiculo other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
