using System;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaAlertaEvento
    {

        #region Métodos Publicos

        public string ObterSql(Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.FiltroPesquisaCargasAlertaEvento filtroPesquisa)
        {
            StringBuilder joins = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarSelect(select, filtroPesquisa);
            SetarWhere(filtroPesquisa, where, joins);

            sql.Append($"select {select.ToString().Trim().Substring(0, select.Length - 1)} "); 
            sql.Append($" from T_CARGA Carga ");
            sql.Append(joins.ToString());
            sql.Append($" where {where.ToString().Trim().Substring(0)} ");

            return sql.ToString();
        }

        #endregion

        #region Métodos Privados


        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" LEFT JOIN T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento ON Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO and CargaJanelaCarregamento.CEC_CODIGO is not null ");
        }

        private void SetarJoinsCentroCarregamentoLimite(StringBuilder joins)
        {
            SetarJoinsCargaJanelaCarregamento(joins);

            if (!joins.Contains(" LimiteCarregamento "))
                joins.Append(" left join T_CENTRO_CARREGAMENTO_LIMITE_CARREGAMENTO LimiteCarregamento on LimiteCarregamento.CEC_CODIGO = CargaJanelaCarregamento.CEC_CODIGO and DATEPART(WEEKDAY, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO) = LimiteCarregamento.CLC_DIA and Carga.TCG_CODIGO = LimiteCarregamento.TCG_CODIGO ");
        }

        private void SetarJoinsCargaIndicador(StringBuilder joins)
        {
            if (!joins.Contains(" CargaIndicador "))
                joins.Append(" INNER JOIN T_CARGA_INDICADOR CargaIndicador ON Carga.CAR_CODIGO = CargaIndicador.CAR_CODIGO ");

        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append(" inner JOIN T_MONITORAMENTO Monitoramento ON Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO ");
        }


        private void SetarSelect(StringBuilder select, Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.FiltroPesquisaCargasAlertaEvento filtrosPesquisa)
        {

            select.Append("Carga.CAR_CODIGO CodigoCarga, ");
            select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR CargaEmbarcador,");
            select.Append("Carga.CAR_DATA_CRIACAO DataCriacaoCarga,");
            select.Append("Carga.CAR_DATA_TERMINO_CARGA DataPrevisaoTerminoCarga,");
            select.Append("Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem,");
            select.Append("Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA DataInicioViagemPrevista,");
            select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamentoCarga,");
            select.Append("(SELECT top 1 CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO FROM T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento WHERE CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO and CargaJanelaCarregamento.CEC_CODIGO is not null) DataInicioCarregamentoJanela,");
            select.Append("Carga.CAR_DATA_FIM_VIAGEM_PREVISTA DataPrevisaoChegada,");
            select.Append("Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM DataPrevisaoChegadaPlanta,");

            if (filtrosPesquisa.TipoAlertaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AntecedenciaGrade)
                select.Append("CAST(DATEADD(dd, - LimiteCarregamento.CLC_DIAS_ANTECEDENCIA, CONVERT(date, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO)) AS DATETIME) + CAST(LimiteCarregamento.CLC_HORA_LIMITE AS DATETIME) DataLimiteCarregamento,");

        }


        private void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.FiltroPesquisaCargasAlertaEvento filtrosPesquisa, StringBuilder where, StringBuilder joins)
        {
            string patternDate = "yyyy-MM-dd HH:mm:sss";

            where.Append($" Carga.CAR_CARGA_FECHADA = 1 and Carga.CAR_DATA_CRIACAO > '2022-01-03' and (Carga.CAR_SITUACAO <> 13 or Carga.CAR_SITUACAO is null) and (Carga.CAR_SITUACAO <> 18 or Carga.CAR_SITUACAO is null) ");

            switch (filtrosPesquisa.TipoAlertaCarga)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.CagraSemVeiculo:
                    where.Append($" and (Carga.CAR_SITUACAO <> 11 or Carga.CAR_SITUACAO is null) and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO is not null and DATEDIFF(minute, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, '" + DateTime.Now.ToString(patternDate) + "') <= " + filtrosPesquisa.TempoEvento);
                    where.Append($" and Carga.CAR_VEICULO is null");

                    SetarJoinsCargaJanelaCarregamento(joins);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.CargaSemTransportador:
                    where.Append($" and (Carga.CAR_SITUACAO <> 11 or Carga.CAR_SITUACAO is null)  and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO is not null and DATEDIFF(minute, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, '" + DateTime.Now.ToString(patternDate) + "') <= " + filtrosPesquisa.TempoEvento);
                    where.Append($" and Carga.EMP_CODIGO IS NULL");

                    SetarJoinsCargaJanelaCarregamento(joins);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.VeiculoNaoMonitorado:
                    //se o veiculo da carga, esta recebendo sinal, porem o monitoramento da carga nao existe ou nao esta recendo sinal (veiculo enviando sinal para outro monitoramento) temos q guardar a data da informacao do veiculo, para comparar com o tempo do alerta

                    //SetarJoinsMonioramento(joins);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AntecedenciaGrade:
                    //Deverá confrontar(quando a carga for confirmada pelo transportador) a data de criação da carga com a data de agendamento e verificar se esta diferença respeitou a antecedência mínima existente no centro de carregamento, nos períodos de carregamento;

                    where.Append($" and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO is not null and Carga.CAR_DATA_CRIACAO < CAST(DATEADD(dd, - LimiteCarregamento.CLC_DIAS_ANTECEDENCIA, CONVERT(date, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO)) AS DATETIME) + CAST(LimiteCarregamento.CLC_HORA_LIMITE AS DATETIME) and CargaIndicador.CIC_INDICADOR_TRANSPORTADOR != 0  and Monitoramento.MON_STATUS IN (0,1) ");

                    SetarJoinsCentroCarregamentoLimite(joins);
                    SetarJoinsMonitoramento(joins);
                    SetarJoinsCargaIndicador(joins);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda:
                    where.Append($" and (Carga.CAR_SITUACAO <> 11 or Carga.CAR_SITUACAO is null) and Monitoramento.MON_STATUS IN (0,1) and Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM > CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO ");
                    
                    SetarJoinsMonitoramento(joins);
                    SetarJoinsCargaJanelaCarregamento(joins);
                    break;


                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.VeiculoComInsumos:
                    //where.Append($" and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO is not null and (DATEDIFF(minute, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, '" + DateTime.Now.ToString(patternDate) + "') <= " + tempoMinutosLimite);
                    //where.Append($" and Transportador.EMP_CODIGO is null)");

                    //SetarJoinsCargaJanelaCarregamento(joins);
                    //SetarJoinsTransportador(joins);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AtrasoInicioViagem:
                    where.Append($" and Carga.CAR_DATA_INICIO_VIAGEM is null and Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA is not null and Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA <= '" + DateTime.Now.AddMinutes(filtrosPesquisa.TempoEvento).ToString(patternDate) + "'");
                    where.Append($" and Carga.CAR_VEICULO is null");

                    break;

                default:
                    break;

            }

        }


        #endregion


    }
}
