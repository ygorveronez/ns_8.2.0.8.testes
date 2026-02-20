using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANCELAMENTO_CTE_SEM_CARGA", EntityName = "CancelamentoCTeSemCarga", Name = "Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga", NameType = typeof(CancelamentoCTeSemCarga))]
    public class CancelamentoCTeSemCarga : EntidadeBase, IEquatable<CancelamentoCTeSemCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCS_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusao", Column = "CCS_DATA_INCLUSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "CCS_DATA_ATUALIZACA0", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacao { get; set; }

        //[NHibernate.Mapping.Attributes.Bag(0, Name = "CancelamentoCTe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANCELAMENTO_CTE")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "CCS_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CancelamentoCTe", Column = "CC_CODIGO")]
        //public virtual IList<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe> CancelamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicao", Column = "CCS_MOTIVO_REJEICAO", TypeType = typeof(string), NotNull = false)]
        public virtual string MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "CCS_MOTIVO_CANCELAMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        public virtual bool Equals(CancelamentoCTeSemCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                if (MotivoCancelamento != null)
                    return MotivoCancelamento;
                else
                    return "";
            }
        }
    }
 
}
