using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_EMILENIUM", EntityName = "ConfiguracaoEmilenium", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium", NameType = typeof(ConfiguracaoEmilenium))]
    public class ConfiguracaoEmilenium : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_TRANSID_MASSIVO_EMILLENIUM", TypeType = typeof(int), NotNull = false)]
        public virtual int TransIdInicioBuscaMassiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COE_TRANSID_MASSIVO_FIM_EMILLENIUM", TypeType = typeof(int), NotNull = false)]
        public virtual int TransIdFimBuscaMassiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacaoBuscaMassiva", Column = "COE_DATA_FINALIZACAO_INTEGRACAO_MASSIVA_EMILLENIUM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacaoBuscaMassiva { get; set; }
    }
}
