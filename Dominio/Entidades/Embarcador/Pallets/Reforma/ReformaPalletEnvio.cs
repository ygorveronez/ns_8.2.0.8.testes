using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets.Reforma
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_REFORMA_ENVIO", EntityName = "ReformaPalletEnvio", Name = "Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvio", NameType = typeof(ReformaPalletEnvio))]
    public class ReformaPalletEnvio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PRE_DATA", TypeType = typeof(System.DateTime), NotNull = true)]
        public virtual System.DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "QuantidadesReforma", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_REFORMA_ENVIO_QUANTIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ReformaPalletEnvioQuantidade", Column = "PRQ_CODIGO")]
        public virtual IList<ReformaPalletEnvioQuantidade> QuantidadesReforma { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(ReformaPalletEnvio other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
