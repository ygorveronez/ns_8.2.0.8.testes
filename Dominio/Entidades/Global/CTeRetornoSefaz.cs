using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_RETORNO_SEFAZ", EntityName = "CTeRetornoSefaz", Name = "Dominio.Entidades.CTeRetornoSefaz", NameType = typeof(CTeRetornoSefaz))]
    public class CTeRetornoSefaz : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        /// <summary>
        /// A - AUTORIZAÇÃO
        /// C - CANCELAMENTO
        /// I - INUTILIZACAO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CRS_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "CRS_DATAHORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CRS_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodStatusProtocolo", Column = "CRS_CODSTATUS_PROTOCOLO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodStatusProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCTeIntegrador", Column = "CRS_CODIGO_CTE_INTEGRADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoCTeIntegrador { get; set; }

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
                    case "I":
                        return "Inutilização";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
