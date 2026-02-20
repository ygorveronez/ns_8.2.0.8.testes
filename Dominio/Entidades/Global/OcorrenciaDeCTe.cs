using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_OCORRENCIA", EntityName = "OcorrenciaDeCTe", Name = "Dominio.Entidades.OcorrenciaDeCTe", NameType = typeof(OcorrenciaDeCTe))]
    public class OcorrenciaDeCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeCadastro", Column = "COC_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDeCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDaOcorrencia", Column = "COC_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "COF_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InclusaoPorNota", Column = "COC_INCLUSAO_POR_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InclusaoPorNota { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "XMLNotaFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_OCORRENCIA_XML_NOTAS_FISCAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> XMLNotaFiscais { get; set; }
    }
}
