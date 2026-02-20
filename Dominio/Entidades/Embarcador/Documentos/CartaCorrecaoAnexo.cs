namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARTA_CORRECAO_ANEXO", EntityName = "CartaCorrecaoAnexo", Name = "Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo", NameType = typeof(CartaCorrecaoAnexo))]
    public class CartaCorrecaoAnexo : Anexo.Anexo<Documentos.ControleDocumento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleDocumento", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Documentos.ControleDocumento EntidadeAnexo { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_CODIGO_IRREGULARIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIrregularidade { get; set; }
    }
}
