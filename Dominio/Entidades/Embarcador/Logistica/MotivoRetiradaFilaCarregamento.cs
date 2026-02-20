using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_MOTIVO_RETIRADA", EntityName = "MotivoRetiradaFilaCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento", NameType = typeof(MotivoRetiradaFilaCarregamento))]
    public class MotivoRetiradaFilaCarregamento : EntidadeBase, IEquatable<MotivoRetiradaFilaCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FMR_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FMR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FMR_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mobile", Column = "FMR_MOBILE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Mobile { get; set; }

        public virtual string DescricaoAtivo
        {
            get  { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoRetiradaFilaCarregamento other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
