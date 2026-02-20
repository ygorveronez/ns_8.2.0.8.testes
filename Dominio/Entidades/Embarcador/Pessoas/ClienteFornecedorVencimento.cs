namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CLIENTE_FORNECEDOR_VENCIMENTO", EntityName = "ClienteFornecedorVencimento", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento", NameType = typeof(Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento))]
    public class ClienteFornecedorVencimento : EntidadeBase
    {
        #region Propriedades
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaEmissaoInicial", Column = "CFV_DIA_EMISSAO_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaEmissaoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaEmissaoFinal", Column = "CFV_DIA_EMISSAO_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaEmissaoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vencimento", Column = "CFV_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int Vencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }
        #endregion

        #region Propriedades Virtuais
        public virtual string DataEmissao
        {
            get
            {
                return this.DiaEmissaoInicial + " até " + this.DiaEmissaoFinal; 
            }
        }
        #endregion

    }
}
