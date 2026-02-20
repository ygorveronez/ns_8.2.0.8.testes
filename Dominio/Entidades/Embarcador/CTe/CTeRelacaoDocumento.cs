namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_RELACAO_DOCUMENTO", EntityName = "CTeRelacaoDocumento", Name = "Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento", NameType = typeof(CTeRelacaoDocumento))]

    public class CTeRelacaoDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// CT-e original
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO_ORIGINAL", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTeOriginal { get; set; }

        /// <summary>
        /// CT-e gerado
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO_GERADO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTeGerado { get; set; }

        /// <summary>
        /// Tipo do CT-e gerado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTeGerado", Column = "CRD_TIPO_CTE_GERADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado TipoCTeGerado { get; set; }
    }
}
