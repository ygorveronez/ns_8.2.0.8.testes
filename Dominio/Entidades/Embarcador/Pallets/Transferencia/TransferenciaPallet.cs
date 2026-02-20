using System;

namespace Dominio.Entidades.Embarcador.Pallets.Transferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_TRANSFERENCIA", EntityName = "TransferenciaPallet", Name = "Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet", NameType = typeof(TransferenciaPallet))]
    public class TransferenciaPallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public TransferenciaPallet() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TransferenciaPalletEnvio", Column = "PTE_CODIGO", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TransferenciaPalletEnvio Envio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TransferenciaPalletRecebimento", Column = "PTR_CODIGO", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TransferenciaPalletRecebimento Recebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TransferenciaPalletSolicitacao", Column = "PTS_CODIGO", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TransferenciaPalletSolicitacao Solicitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_DATA_TRANSFERENCIA", TypeType = typeof(System.DateTime), NotNull = false)]
        public virtual DateTime? DataTransferencia { get; set; }

        public virtual string Descricao {
            get {
                return Numero.ToString();
            }
        }

        public virtual bool Equals(TransferenciaPallet other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
