using System;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MOTIVO_REMOCAO_ESCALA", EntityName = "MotivoRemocaoVeiculoEscala", Name = "Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala", NameType = typeof(MotivoRemocaoVeiculoEscala))]
    public class MotivoRemocaoVeiculoEscala : EntidadeBase, IEquatable<MotivoRemocaoVeiculoEscala>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MRE_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MRE_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MRE_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoRemocaoVeiculoEscala other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
