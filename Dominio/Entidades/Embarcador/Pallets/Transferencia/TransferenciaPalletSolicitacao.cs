using System;

namespace Dominio.Entidades.Embarcador.Pallets.Transferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_TRANSFERENCIA_SOLICITACAO", EntityName = "TransferenciaPalletSolicitacao", Name = "Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletSolicitacao", NameType = typeof(TransferenciaPalletSolicitacao))]
    public class TransferenciaPalletSolicitacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PTS_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PTS_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Solicitante", Column = "PTS_SOLICITANTE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Turno", Column = "TUR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Turno Turno { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

        public virtual bool Equals(TransferenciaPalletSolicitacao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
