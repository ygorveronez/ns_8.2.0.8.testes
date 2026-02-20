using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Integracao.Monitoramento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_TECNOLOGIA_MONITORAMENTO", EntityName = "ConfiguracaoIntegracaoTecnologiaMonitoramento", Name = "Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento", NameType = typeof(ConfiguracaoIntegracaoTecnologiaMonitoramento))]
    public class ConfiguracaoIntegracaoTecnologiaMonitoramento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_HABILITADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Habilitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_PROCESSAR_SENSORES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ProcessarSensores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_TEMPO_SLEEP_ENTRE_CONTAS", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoSleepEntreContas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_MINUTOS_DIFERENCA_MINIMA_ENTRE_POSICOES", TypeType = typeof(int), NotNull = true)]
        public virtual int MinutosDiferencaMinimaEntrePosicoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual string Descricao { get { return Tipo.ObterDescricao(); } }
    }
}
