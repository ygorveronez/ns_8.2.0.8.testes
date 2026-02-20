using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_DOCUMENTO", EntityName = "CargaOcorrenciaDocumento", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento", NameType = typeof(CargaOcorrenciaDocumento))]
    public class CargaOcorrenciaDocumento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO_COMPLEMENTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManualComplementado { get; set; }

        /// <summary>
        /// Armazena o Ct-e importado quando encontra para uma ocorrência
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTeImportado { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaDeCTe", Column = "OOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.OcorrenciaDeCTe OcorrenciaDeCTe { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "XMLNotaFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_OCORRENCIA_DOCUMENTO_XML_NOTAS_FISCAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> XMLNotaFiscais { get; set; }

        public virtual bool Equals(CargaOcorrenciaDocumento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
