using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_RETORNO_SEFAZ", EntityName = "MDFeRetornoSefaz", Name = "Dominio.Entidades.MDFeRetornoSefaz", NameType = typeof(MDFeRetornoSefaz))]
    public class MDFeRetornoSefaz : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        /// <summary>
        /// A - AUTORIZAÇÃO
        /// C - CANCELAMENTO
        /// E - ENCERRAMENTO
        /// O - CONTINGÊNCIA
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MRS_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "MRS_DATAHORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "MRS_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMDFeIntegrador", Column = "MRS_CODIGO_MDFE_INTEGRADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoMDFeIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ErroSefaz", Column = "ERR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ErroSefaz ErroSefaz { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case "A":
                        return "Autorização";
                    case "C":
                        return "Cancelamento";
                    case "E":
                        return "Encerramento";
                    case "I":
                        return "Inclusão Motorista";
                    case "O":
                        return "Contingência";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
