using System;

namespace Dominio.Entidades.Embarcador.Pallets.Transferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_TRANSFERENCIA_RECEBIMENTO", EntityName = "TransferenciaPalletRecebimento", Name = "Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletRecebimento", NameType = typeof(TransferenciaPalletRecebimento))]
    public class TransferenciaPalletRecebimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PTR_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PTR_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Recebedor", Column = "PTR_RECEBEDOR", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Recebedor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

        public virtual bool Equals(TransferenciaPalletRecebimento other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
