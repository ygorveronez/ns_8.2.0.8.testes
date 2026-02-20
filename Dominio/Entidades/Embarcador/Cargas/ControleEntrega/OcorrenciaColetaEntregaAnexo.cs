namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_COLETA_ENTREGA_ANEXO", EntityName = "OcorrenciaColetaEntregaAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.OcorrenciaColetaEntregaAnexo", NameType = typeof(OcorrenciaColetaEntregaAnexo))]
    public class OcorrenciaColetaEntregaAnexo : Anexo.Anexo<Cargas.ControleEntrega.OcorrenciaColetaEntrega>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaColetaEntrega", Column = "OCE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.ControleEntrega.OcorrenciaColetaEntrega EntidadeAnexo { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ANX_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

    }
}
