using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets.Reforma
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_REFORMA", EntityName = "ReformaPallet", Name = "Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet", NameType = typeof(ReformaPallet))]
    public class ReformaPallet : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAR_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "PAR_DATA_RETORNO", TypeType = typeof(System.DateTime), NotNull = false)]
        public virtual System.DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoReformaPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoReformaPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ReformaPalletEnvio", Column = "PRE_CODIGO", NotNull = true, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ReformaPalletEnvio Envio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscaisSaida", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_REFORMA_NFE_SAIDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ReformaPalletNfeSaida", Column = "PNF_CODIGO")]
        public virtual IList<ReformaPalletNfeSaida> NotasFiscaisSaida { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscaisRetorno", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_REFORMA_NFE_RETORNO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ReformaPalletNfeRetorno", Column = "PNR_CODIGO")]
        public virtual IList<ReformaPalletNfeRetorno> NotasFiscaisRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasServicoRetorno", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_REFORMA_NFS_RETORNO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ReformaPalletNfsRetorno", Column = "PNS_CODIGO")]
        public virtual IList<ReformaPalletNfsRetorno> NotasServicoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAR_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        public virtual string Descricao
        {
            get {  return Numero.ToString(); }
        }

        public virtual bool Equals(ReformaPallet other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
