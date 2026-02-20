using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ROTA_FRETE", EntityName = "ConfiguracaoRotaFrete", Name = "Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete", NameType = typeof(ConfiguracaoRotaFrete))]
    public class ConfiguracaoRotaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CRF_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CRF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiLocalidadesDestino", Column = "CRF_POSSUI_LOCALIDADES_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiLocalidadesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiTiposCarga", Column = "CRF_POSSUI_TIPOS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiTiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiModelosVeicularesCarga", Column = "CRF_POSSUI_MODELOS_VEICULARES_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiModelosVeicularesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiGrupoTransportadoresHUBOfertas", Column = "CRF_POSSUI_GRUPO_TRANSPORTADORES_HUB_OFERTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiGrupoTransportadoresHUBOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraEnvioTransportadorRota", Column = "CRF_HORA_ENVIO_TRANSPORTADOR_ROTA", TypeType = typeof(TimeSpan), Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraEnvioTransportadorRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_DOMINGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaDomingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_SEGUNDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_TERCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_QUARTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_QUINTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_SEXTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_SABADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarSpotAbertoAposTempoLimiteGrupos", Column = "CRF_LIBERAR_SPOT_ABERTO_APOS_TEMPO_LIMITE_GRUPOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarSpotAbertoAposTempoLimiteGrupos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOferta", Column = "CRF_TIPO_OFERTA", TypeType = typeof(int), NotNull = false)]
        public virtual int TipoOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAntecedenciaHUBOfertas", Column = "CRF_DIAS_ANTECEDENCIA_HUB_OFERTAS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAntecedenciaHUBOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraEnvioOfertaHUBOfertas", Column = "CRF_HORA_ENVIO_OFERTA", TypeType = typeof(TimeSpan), Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraEnvioOfertaHUBOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_DOMINGO_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaDomingoHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_SEGUNDA_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaSegundaHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_TERCA_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaTercaHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_QUARTA_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaQuartaHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_QUINTA_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaQuintaHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_SEXTA_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaSextaHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_TRANSPORTADOR_ROTA_SABADO_HUB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTransportadorRotaSabadoHUB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAntecedenciaEnvioTransportadorRota", Column = "CRF_DIAS_ANTECEDENCIA_ENVIO_TRANSPORTADOR_ROTA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAntecedenciaEnvioTransportadorRota { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LocalidadesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ROTA_FRETE_LOCALIDADE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> LocalidadesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LocalidadesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ROTA_FRETE_LOCALIDADE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> LocalidadesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Estados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ROTA_FRETE_ESTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> Estados { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool EnviarTransportadorRota(DateTime data)
        {
            bool[] diasSemana = new bool[7];

            diasSemana[(int)DayOfWeek.Sunday] = EnviarTransportadorRotaDomingo;
            diasSemana[(int)DayOfWeek.Monday] = EnviarTransportadorRotaSegunda;
            diasSemana[(int)DayOfWeek.Tuesday] = EnviarTransportadorRotaTerca;
            diasSemana[(int)DayOfWeek.Thursday] = EnviarTransportadorRotaQuarta;
            diasSemana[(int)DayOfWeek.Wednesday] = EnviarTransportadorRotaQuinta;
            diasSemana[(int)DayOfWeek.Friday] = EnviarTransportadorRotaSexta;
            diasSemana[(int)DayOfWeek.Saturday] = EnviarTransportadorRotaSabado;

            return diasSemana[(int)data.DayOfWeek];
        }

        public virtual bool EnviarTransportadorRotaConfigurado()
        {
            return (
                EnviarTransportadorRotaDomingo ||
                EnviarTransportadorRotaSegunda ||
                EnviarTransportadorRotaTerca ||
                EnviarTransportadorRotaQuarta ||
                EnviarTransportadorRotaQuinta ||
                EnviarTransportadorRotaSexta ||
                EnviarTransportadorRotaSabado
            );
        }
    }
}
