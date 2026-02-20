using System;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_INCONSISTENCIA_DIGITACAO_CANHOTO", EntityName = "MotivoInconsistenciaDigitacao", Name = "Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao", NameType = typeof(MotivoInconsistenciaDigitacao))]
    public class MotivoInconsistenciaDigitacao : EntidadeBase, IEquatable<MotivoInconsistenciaDigitacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CMI_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CMI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CMI_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeObservacao", Column = "CMI_EXIGE_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeObservacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoInconsistenciaDigitacao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}