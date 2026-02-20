using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_CTE_COMPLEMENTAR_GLOBALIZADO_NOTA_FISCAL", EntityName = "CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal", NameType = typeof(CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal))]
    public class CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        public virtual bool Equals(CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
