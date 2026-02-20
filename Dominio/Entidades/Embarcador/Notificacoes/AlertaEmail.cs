using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ALERTA_EMAIL", EntityName = "AlertaEmail", Name = "Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail", NameType = typeof(AlertaEmail))]
    public class AlertaEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CAE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraInicio", Column = "CAE_DATA_HORA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraFim", Column = "CAE_DATA_HORA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRepeticoes", Column = "CAE_NUMERO_REPETICOES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroRepeticoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoNotificacoes", Column = "CAE_PERIODO_NOTIFICACOES", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumIntervaloAlertaEmail), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumIntervaloAlertaEmail PeriodoNotificacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Setor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ALERTA_EMAIL_SETOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Setor", Column = "SET_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Setor> Setor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Portfolio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ALERTA_EMAIL_PORTFOLIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PortfolioModuloControle", Column = "PMC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle> Portfolio { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Irregularidade", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ALERTA_EMAIL_IRREGULARIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Irregularidade", Column = "IRR_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> Irregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Usuarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ALERTA_EMAIL_USUARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Usuarios { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoEnvio", Column = "CAE_DATA_ULTIMO_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoEnvio { get; set; }
    }    
}
