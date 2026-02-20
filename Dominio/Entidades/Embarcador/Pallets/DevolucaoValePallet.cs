namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_VALE_PALLET_DEVOLUCAO", EntityName = "DevolucaoValePallet", Name = "Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet", NameType = typeof(DevolucaoValePallet))]
    public class DevolucaoValePallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DVP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DVP_DATA", TypeType = typeof(System.DateTime), NotNull = true)]
        public virtual System.DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallets", Column = "DVP_QUANTIDADE_PALLETS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "DVP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValePallet", Column = "VLP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ValePallet ValePallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DVP_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual bool Equals(DevolucaoValePallet other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
