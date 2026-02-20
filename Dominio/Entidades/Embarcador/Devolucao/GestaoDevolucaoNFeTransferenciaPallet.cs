namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_NFE_TRANSFERENCIA_PALLET", EntityName = "GestaoDevolucaoNFeTransferenciaPallet", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet", NameType = typeof(GestaoDevolucaoNFeTransferenciaPallet))]

    public class GestaoDevolucaoNFeTransferenciaPallet : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }
        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNF", Column = "GTP_CHAVE_NOTA_FISCAL", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string ChaveNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFeTransferencia", Column = "GTP_NOTA_FISCAL_DE_TRANSFERENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NFeTransferencia { get; set; }
    }
}
