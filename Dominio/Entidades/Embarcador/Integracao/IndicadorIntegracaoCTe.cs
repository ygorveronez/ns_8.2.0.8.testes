using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INDICADOR_INTEGRACAO_CTE", EntityName = "IndicadorIntegracaoCTe", Name = "Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe", NameType = typeof(IndicadorIntegracaoCTe))]
    public class IndicadorIntegracaoCTe: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "IIC_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaConsultaIntegracao", Column = "IIC_DATA_ULTIMA_CONSULTA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsultaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual WebService.Integradora Integradora { get; set; }

        public virtual string Descricao
        {
            get { return $"Integração do CT-e {CargaCTe.CTe.Numero} pela integradora {Integradora.Descricao}";  }
        }
    }
}
