using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    /// <summary>
    /// Define com quais sistemas a carga deve ser integrada; Cada ponto da integração deve ser tratado individualmente confirme integração, aqui serve apenas para identificar com quem essa carga deve ser integrada.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO", EntityName = "CargaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracao", NameType = typeof(CargaIntegracao))]
    public class CargaIntegracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>
    {
        public CargaIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIN_BLOQUEAR_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCancelamentoCarga { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaIntegracao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao)this.MemberwiseClone();
        }
        public virtual bool Equals(CargaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
