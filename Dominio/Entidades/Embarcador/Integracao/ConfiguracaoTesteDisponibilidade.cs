using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TESTE_DISPONIBILIDADE", EntityName = "ConfiguracaoTesteDisponibilidade", Name = "Dominio.Entidades.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade", NameType = typeof(ConfiguracaoTesteDisponibilidade))]
    public class ConfiguracaoTesteDisponibilidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoTeste", Column = "CTD_DATA_ULTIMO_TESTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoTeste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTD_EMAIL", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTD_TEMPO_AGUARDAR_EXECUTAR_TESTE", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoAguardarExecutarTeste { get; set; }
    }
}
