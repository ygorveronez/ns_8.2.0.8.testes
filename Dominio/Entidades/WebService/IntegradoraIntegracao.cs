using System;

namespace Dominio.Entidades.WebService
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRADORA_INTEGRACAO", EntityName = "IntegradoraIntegracao", Name = "Dominio.Entidades.WebService.IntegradoraIntegracao", NameType = typeof(IntegradoraIntegracao))]
    public class IntegradoraIntegracao : EntidadeBase, IEquatable<IntegradoraIntegracao>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "II_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "II_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Tipo { get; set; }

        #endregion Propriedades

        #region Métodos com Regras

        public virtual bool Equals(IntegradoraIntegracao other)
        {
            return other.Codigo == this.Codigo;
        }

        #endregion Métodos com Regras
    }
}



