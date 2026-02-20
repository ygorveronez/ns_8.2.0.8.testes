using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CALCULO_PREVISAO", EntityName = "ConfiguracaoCalculoPrevisao", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoCalculoPrevisao", NameType = typeof(ConfiguracaoCalculoPrevisao))]
    public class ConfiguracaoCalculoPrevisao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesconsiderarSabadosCalculoPrevisao", Column = "CCP_DESCONSIDERAR_SABADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarSabadosCalculoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesconsiderarDomingosCalculoPrevisao", Column = "CCP_DESCONSIDERAR_DOMINGOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarDomingosCalculoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesconsiderarFeriadosCalculoPrevisao", Column = "CCP_DESCONSIDERAR_FERIADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarFeriadosCalculoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarJornadaMotorita", Column = "CCP_CONSIDERAR_JORNADA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarJornadaMotorita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioInicialAlmoco", Column = "CCP_HORARIO_INICIAL_ALMOCO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HorarioInicialAlmoco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MinutosIntervalo", Column = "CCP_MINUTOS_INTERVALO", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosIntervalo { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para calculo Previsao"; }
        }
    }
}
