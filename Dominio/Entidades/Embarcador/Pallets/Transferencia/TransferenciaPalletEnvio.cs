namespace Dominio.Entidades.Embarcador.Pallets.Transferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_TRANSFERENCIA_ENVIO", EntityName = "TransferenciaPalletEnvio", Name = "Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletEnvio", NameType = typeof(TransferenciaPalletEnvio))]
    public class TransferenciaPalletEnvio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PTE_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Remetente", Column = "PTE_REMETENTE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Remetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "PTE_RESPONSAVEL", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Responsavel { get; set; }

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

        public virtual bool Equals(TransferenciaPalletEnvio other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
