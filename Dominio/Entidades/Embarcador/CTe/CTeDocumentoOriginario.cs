using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_DOCUMENTO_ORIGINARIO", EntityName = "CTeDocumentoOriginario", Name = "Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario", NameType = typeof(CTeDocumentoOriginario))]
    public class CTeDocumentoOriginario: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// CT-e gerado
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        /// <summary>
        /// Chave do CT-e que originou o CT-e gerado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDO_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        /// <summary>
        /// Número do CT-e que originou o CT-e gerado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        /// <summary>
        /// Série do CT-e que originou o CT-e gerado
        /// </summary>        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDO_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        /// <summary>
        /// Número Operacional do Conhecimento Aéreo que originou o CT-e gerado
        /// </summary>        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDO_NUMERO_OPERACIONAL_CONHECIMENTO_AEREO", TypeType = typeof(long), NotNull = false)]
        public virtual long? NumeroOperacionalConhecimentoAereo { get; set; }

        /// <summary>
        /// Número da Minuta do Conhecimento Aéreo que originou o CT-e gerado
        /// </summary>        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDO_NUMERO_MINUTA", TypeType = typeof(long), NotNull = false)]
        public virtual long? NumeroMinuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDO_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }
    }
}
