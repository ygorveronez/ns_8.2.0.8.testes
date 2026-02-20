using System;

namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_USUARIO_MONITORAMENTO_MAPA", EntityName = "ConfiguracaoUsuarioMonitoramentoMapa", Name = "Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa", NameType = typeof(ConfiguracaoUsuarioMonitoramentoMapa))]
    public class ConfiguracaoUsuarioMonitoramentoMapa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CUM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CUM_CONFIGURACAO_EXIBICAO_INDICADORES", Type = "StringClob", NotNull = false)]
        public virtual string ConfiguracaoExibicaoIndicadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CUM_CONFIGURACAO_EXIBICAO_LEGENDA_MAPA", Type = "StringClob", NotNull = false)]
        public virtual string ConfiguracaoExibicaoLegendaMapa { get; set; }
    }
}
