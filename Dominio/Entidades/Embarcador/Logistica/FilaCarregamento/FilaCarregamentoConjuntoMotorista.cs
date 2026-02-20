using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_CONJUNTO_MOTORISTA", EntityName = "FilaCarregamentoConjuntoMotorista", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista", NameType = typeof(FilaCarregamentoConjuntoMotorista))]
    public class FilaCarregamentoConjuntoMotorista : EntidadeBase, IEquatable<FilaCarregamentoConjuntoMotorista>
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Exclusivo", Column = "FCM_EXCLUSIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Exclusivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FCM_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoMotorista", Column = "FLM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoMotorista FilaCarregamentoMotorista { get; set; }

        #endregion

        #region Métodos Públicos

        public virtual bool IsCompleto()
        {
            return Motorista != null;
        }

        public virtual bool Equals(FilaCarregamentoConjuntoMotorista other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion
    }
}
