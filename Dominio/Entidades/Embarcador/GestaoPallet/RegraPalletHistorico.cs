using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PALLET_HISTORICO", EntityName = "RegraPalletHistorico", Name = "Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico", NameType = typeof(RegraPalletHistorico))]
    public class RegraPalletHistorico : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPallet", Column = "RPH_REGRA_PALLET", TypeType = typeof(RegraPallet), NotNull = true)]
        public virtual RegraPallet RegraPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "RPH_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "RPH_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }
    }
}
