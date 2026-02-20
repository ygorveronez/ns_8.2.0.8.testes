using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GATILHO_GERACAO_AUTOMATICA_OCORRENCIA", EntityName = "GatilhoGeracaoAutomaticaOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia", NameType = typeof(GatilhoGeracaoAutomaticaOcorrencia))]
    public class GatilhoGeracaoAutomaticaOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GGO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega", Column = "GGO_DEFINIR_AUTOMATICAMENTE_TEMPO_ESTADIA_POR_TEMPO_PARADA_NO_LOCAL_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GGO_GERAR_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GGO_GATILHO_FINAL_FLUXO_PATIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio? GatilhoFinalFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GGO_GATILHO_INICIAL_FLUXO_PATIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrenciaInicialFluxoPatio), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrenciaInicialFluxoPatio? GatilhoInicialFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GATILHO_FINAL_TRAKING", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking? GatilhoFinalTraking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GATILHO_INICIAL_TRAKING", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GatilhoInicialTraking), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GatilhoInicialTraking? GatilhoInicialTraking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasMinimas", Column = "GGO_HORAS_MINIMAS", TypeType = typeof(int), NotNull = true)]
        public virtual int HorasMinimas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GGO_UTILIZAR_TEMPO_CARREGAMENTO_COMO_HORA_MINIMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTempoCarregamentoComoHoraMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "GGO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrencia Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametroOcorrencia", Column = "POC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametroOcorrencia Parametro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAplicacaoGatilhoTracking", Column = "GGO_TIPO_APLICACAO_GATILHO_TRACKING", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking TipoAplicacaoGatilhoTracking { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GATILHO_OCORRENCIA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GGO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TiposOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GATILHO_OCORRENCIA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GGO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filiais.Filial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GATILHO_OCORRENCIA_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GGO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobrancaMultimodal", Column = "GGO_TIPO_COBRANCA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarDataAgendadaEntrega", Column = "GGO_VALIDAR_DATA_AGENDADA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDataAgendadaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDataAlteracaoGatilho", Column = "GGO_TIPO_DATA_ALTERACAO_GATILHO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDataAlteracaoGatilho), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDataAlteracaoGatilho TipoDataAlteracaoGatilho { get; set; }


		[NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteDuplicarOcorrencia", Column = "GGO_NAO_PERMITE_DUPLICAR_OCORRENCIA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
		public virtual bool NaoPermiteDuplicarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtribuirDataOcorrenciaNaDataAgendamentoTransportador", Column = "GGO_ATRIBUIR_DATA_OCORRENCIA_NA_DATA_AGENDAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtribuirDataOcorrenciaNaDataAgendamentoTransportador { get; set; }

        public virtual string ObterObservacaoOcorrencia()
        {
            switch (Tipo)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrencia.FluxoPatio:
                    return "Ocorrência gerada automaticamente por finalização de etapa do fluxo de pátio";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrencia.Tracking:
                    return "Ocorrência gerada automaticamente por monitoramento";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrencia.AlteracaoData:
                    return $"Ocorrência gerada automaticamente por alteração da data de {TipoDataAlteracaoGatilho.ObterDescricao()}";
                case ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoOcorrencia.AtingirData:
                    return "Ocorrência gerada automaticamente por atingir data";
                default:
                    return "";
            }
        }
    }
}
