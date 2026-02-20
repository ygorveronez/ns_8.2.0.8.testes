using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_PUNICAO", EntityName = "PunicaoVeiculo", Name = "Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo", NameType = typeof(PunicaoVeiculo))]
    public class PunicaoVeiculo : EntidadeBase, IEquatable<PunicaoVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "VPU_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioPunicao", Column = "VPU_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioPunicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasPunicao", Column = "VPU_DIAS_PUNICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasPunicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "VPU_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoPunicaoVeiculo", Column = "VMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoPunicaoVeiculo Motivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        public virtual string Descricao {
            get { return Codigo.ToString(); }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(PunicaoVeiculo other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
