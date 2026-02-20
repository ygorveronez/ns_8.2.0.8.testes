using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_ATRASO_CARREGAMENTO", EntityName = "MotivoAtrasoCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento", NameType = typeof(MotivoAtrasoCarregamento))]
    public class MotivoAtrasoCarregamento : EntidadeBase, IEquatable<MotivoAtrasoCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MAC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MAC_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Status ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoAtrasoCarregamento other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
