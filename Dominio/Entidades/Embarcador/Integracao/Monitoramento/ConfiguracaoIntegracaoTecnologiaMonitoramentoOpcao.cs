using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Integracao.Monitoramento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_TECNOLOGIA_MONITORAMENTO_OPCAO", EntityName = "ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao", Name = "Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao", NameType = typeof(ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao))]
    public class ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoIntegracaoTecnologiaMonitoramento", Column = "CIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_KEY", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Key { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_VALUE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Value { get; set; }

        public virtual string Descricao { get { return Key; } }
    }
}
