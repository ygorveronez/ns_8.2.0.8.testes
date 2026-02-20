using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Embarcador.TorreControle.Consulta
{
    public class ConsultaPermanencias : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias>
    {
        #region Construtores

        public ConsultaPermanencias() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        protected override SQLDinamico ObterSql(FiltroPesquisaRelatorioPermanencias filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            var parametros = new List<ParametroSQL>();

            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                SetarSelect(propriedade.Propriedade, propriedade.CodigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

            if (!somenteContarNumeroRegistros || parametrosConsulta != null)
                orderBy.Append(" Carga.Car_codigo, Monitoramento.MON_CODIGO, MonitoramentoHistoricoStatusViagem.MHS_DATA_INICIO, CargaEntrega.CEN_CODIGO, PermanenciaCliente.PCL_CODIGO, PermanenciaSubarea.PSA_CODIGO ");

            SetarWhere(filtrosPesquisa, where, joins, groupBy, parametros);

            string campos = select.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (somenteContarNumeroRegistros)
                sql.Append("select distinct(count(0) over ()) ");
            else
                sql.Append($"select {(_somenteRegistrosDistintos ? "distinct " : "")}{(campos.Length > 0 ? campos.Substring(0, campos.Length - 1) : "")} ");

            sql.Append($" from {_tabela} ");
            sql.Append(joins.ToString());

            if (condicoes.Length > 0)
                sql.Append($" where {condicoes.ToString().Substring(3)} ");

            if (!somenteContarNumeroRegistros)
            {
                sql.Append($" order by {(orderBy.Length > 0 ? orderBy.ToString() : "1 asc")}");

                if ((parametrosConsulta != null) && ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0)))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), parametros);
        }

        #region Joins

        private void SetarTodosJoins(StringBuilder joins)
        {
            if (!joins.ToString().Contains("JOIN T_MONITORAMENTO Monitoramento"))
                joins.Append(" JOIN T_MONITORAMENTO Monitoramento ON Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem ON MonitoramentoStatusViagem.MSV_CODIGO = Monitoramento.MSV_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM MonitoramentoHistoricoStatusViagem "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM MonitoramentoHistoricoStatusViagem ON MonitoramentoHistoricoStatusViagem.MON_CODIGO = Monitoramento.MON_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem_Historico "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem_Historico ON MonitoramentoStatusViagem_Historico.MSV_CODIGO = MonitoramentoHistoricoStatusViagem.MSV_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM_PERMANENCIA_CLIENTE MonitoramentoHistoricoStatusViagemPermanenciaCliente "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM_PERMANENCIA_CLIENTE MonitoramentoHistoricoStatusViagemPermanenciaCliente ON MonitoramentoHistoricoStatusViagemPermanenciaCliente.MHS_CODIGO = MonitoramentoHistoricoStatusViagem.MHS_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_PERMANENCIA_CLIENTE PermanenciaCliente "))
                joins.Append(" LEFT JOIN T_PERMANENCIA_CLIENTE PermanenciaCliente ON PermanenciaCliente.PCL_CODIGO = MonitoramentoHistoricoStatusViagemPermanenciaCliente.PCL_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM_PERMANENCIA_SUBAREA MonitoramentoHistoricoStatusViagemPermanenciaSubArea "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM_PERMANENCIA_SUBAREA MonitoramentoHistoricoStatusViagemPermanenciaSubArea ON MonitoramentoHistoricoStatusViagemPermanenciaSubArea.MHS_CODIGO = MonitoramentoHistoricoStatusViagem.MHS_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_PERMANENCIA_SUBAREA PermanenciaSubarea "))
                joins.Append(" LEFT JOIN T_PERMANENCIA_SUBAREA PermanenciaSubarea ON PermanenciaSubarea.PSA_CODIGO = MonitoramentoHistoricoStatusViagemPermanenciaSubArea.PSA_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_SUBAREA_CLIENTE SubareaCliente "))
                joins.Append(" LEFT JOIN T_SUBAREA_CLIENTE SubareaCliente ON SubareaCliente.SAC_CODIGO = PermanenciaSubarea.SAC_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_TIPO_SUBAREA_CLIENTE TipoSubAreaCliente "))
                joins.Append(" LEFT JOIN T_TIPO_SUBAREA_CLIENTE TipoSubAreaCliente on TipoSubAreaCliente.TSA_CODIGO = SubareaCliente.TSA_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_CARGA_ENTREGA CargaEntrega "))
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CEN_CODIGO = COALESCE(PermanenciaCliente.CEN_CODIGO, PermanenciaSubarea.CEN_CODIGO) ");

            if (!joins.ToString().Contains("LEFT JOIN T_CLIENTE Cliente "))
                joins.Append(" LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA ");

            if (!joins.ToString().Contains("LEFT JOIN T_VEICULO Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo ON Monitoramento.VEI_CODIGO = Veiculo.VEI_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_EMPRESA Transportador "))
                joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_TIPO_OPERACAO TipoOperacao "))
                joins.Append(" LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_FILIAL Filial "))
                joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_GRUPO_PESSOAS GrupoPessoas "))
                joins.Append(" LEFT JOIN T_GRUPO_PESSOAS GrupoPessoas ON Cliente.GRP_CODIGO = GrupoPessoas.GRP_CODIGO ");

            if (!joins.ToString().Contains("LEFT JOIN T_POSICAO Posicao "))
                joins.Append(" LEFT JOIN T_POSICAO Posicao ON Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO ");

            if (!joins.ToString().Contains("LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados "))
                joins.Append(" LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados on DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }


        #endregion

        #region Select
        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPermanencias filtrosPesquisa)
        {
            SetarTodosJoins(joins);

            switch (propriedade)
            {
                case "Carga":
                    if (!select.Contains(" Carga, "))
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                        select.Append("Veiculo.VEI_PLACA Placa, ");
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                        select.Append("CASE WHEN Carga.EMP_CODIGO IS NULL THEN '' ELSE CONCAT(Transportador.EMP_RAZAO, ' - (', Transportador.EMP_CNPJ, ')') END as Transportador, ");
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                        select.Append(@"SUBSTRING((
                                            SELECT DISTINCT 
                                                ', ' + CAST(
                                                    SUBSTRING(_motorista.FUN_CPF, 1, 3) + '.' +
                                                    SUBSTRING(_motorista.FUN_CPF, 4, 3) + '.' +
                                                    SUBSTRING(_motorista.FUN_CPF, 7, 3) + '-' +
                                                    SUBSTRING(_motorista.FUN_CPF, 10, 3) + ' - ' +
                                                    _motorista.FUN_NOME AS NVARCHAR(4000)
                                                )
                                            FROM T_CARGA_MOTORISTA _cargaMotorista
                                            JOIN T_FUNCIONARIO _motorista 
                                                ON _cargaMotorista.CAR_MOTORISTA = _motorista.FUN_CODIGO
                                            WHERE _cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                                            FOR XML PATH('')
                                        ), 3, 4000) Motoristas, ");
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                        select.Append("Cliente.CLI_NOME Cliente, ");
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas, "))
                        select.Append("GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ");
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                    break;

                case "OrigemCarga":
                    if (!select.Contains(" OrigemCarga, "))
                        select.Append("DadosSumarizados.CDS_ORIGENS OrigemCarga, ");
                    break;

                case "DestinoCarga":
                    if (!select.Contains(" DestinoCarga, "))
                        select.Append("DadosSumarizados.CDS_DESTINATARIOS DestinoCarga, ");
                    break;

                case "SituacaoMonitoramento":
                case "DescricaoSituacaoMonitoramento":
                    if (!select.Contains(" SituacaoMonitoramento, "))
                        select.Append("Monitoramento.MON_STATUS SituacaoMonitoramento, ");
                    break;

                case "EtapaMonitoramento":
                    if (!select.Contains(" EtapaMonitoramento, "))
                        select.Append("MonitoramentoStatusViagem_Historico.MSV_DESCRICAO EtapaMonitoramento, ");
                    break;

                case "TempoTotalEtapaMonitoramento":
                case "DescricaoTempoTotalEtapaMonitoramento":
                    if (!select.Contains(" TempoTotalEtapaMonitoramento, "))
                        select.Append("MonitoramentoHistoricoStatusViagem.MHS_TEMPO_SEGUNDOS TempoTotalEtapaMonitoramento, ");
                    break;

                case "EntregaForaDoRaio":
                case "DescricaoEntregaForaDoRaio":
                    if (!select.Contains(" EntregaForaDoRaio, "))
                        select.Append("CargaEntrega.CEN_ENTREGA_NO_RAIO EntregaForaDoRaio, ");
                    break;

                case "DataCarregamento":
                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamento, "))
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ");
                    break;

                case "DataCriacaoCarga":
                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga, "))
                        select.Append("Carga.CAR_DATA_CRIACAO DataCriacaoCarga, ");
                    break;

                case "DataAgendamento":
                case "DataAgendamentoFormatada":
                    if (!select.Contains(" DataAgendamento, "))
                        select.Append("CargaEntrega.CEN_DATA_AGENDAMENTO DataAgendamento, ");
                    break;

                case "DataConfirmacao":
                case "DataConfirmacaoFormatada":
                    if (!select.Contains(" DataConfirmacao, "))
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA DataConfirmacao, ");
                    break;

                case "DataEntregaAtualizada":
                case "DataEntregaAtualizadaFormatada":
                    if (!select.Contains(" DataEntregaAtualizada, "))
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA DataEntregaAtualizada, ");
                    break;

                case "TempoAguardoNFE":
                case "DescricaoTempoAguardoNFE":
                    if (!select.Contains(" TempoAguardoNFE, "))
                        select.Append("DATEDIFF(MINUTE, Carga.CAR_DATA_CRIACAO, Carga.CAR_DATA_INICIO_EMISSAO_DOCUMENTOS) TempoAguardoNFE, ");
                    break;

                case "DataPrimeiroEspelhamento":
                case "DataPrimeiroEspelhamentoFormatada":
                    if (!select.Contains(" DataPrimeiroEspelhamento, "))
                        select.Append(@"(
                            SELECT TOP 1 POS.POS_DATA_VEICULO
                            FROM T_MONITORAMENTO_VEICULO MONVEI
                            LEFT JOIN T_MONITORAMENTO_VEICULO_POSICAO MONVEIPOS ON MONVEI.MOV_CODIGO = MONVEIPOS.MOV_CODIGO
                            LEFT JOIN T_POSICAO POS ON MONVEIPOS.POS_CODIGO = POS.POS_CODIGO
                            WHERE Monitoramento.MON_CODIGO = MONVEI.MON_CODIGO
                            ORDER BY POS.POS_DATA_VEICULO ASC ) DataPrimeiroEspelhamento, "
                        );
                    break;

                case "DataUltimoEspelhamento":
                case "DataUltimoEspelhamentoFormatada":
                    if (!select.Contains(" DataUltimoEspelhamento, "))
                        select.Append("Posicao.POS_DATA DataUltimoEspelhamento, ");
                    break;

                case "TipoParada":
                case "DescricaoTipoParada":
                    if (!select.Contains(" TipoParada, "))
                        select.Append("CargaEntrega.CEN_COLETA TipoParada, ");
                    break;

                case "DataEntradaArea":
                case "DataEntradaAreaFormatada":
                    if (!select.Contains(" DataEntradaArea, "))
                        select.Append("PermanenciaCliente.PCL_DATA_INICIO DataEntradaArea, ");
                    break;

                case "DataSaidaArea":
                case "DataSaidaAreaFormatada":
                    if (!select.Contains(" DataSaidaArea, "))
                        select.Append("PermanenciaCliente.PCL_DATA_FIM DataSaidaArea, ");
                    break;

                case "TempoArea":
                case "DescricaoTempoArea":
                    if (!select.Contains(" TempoArea, "))
                        select.Append("PermanenciaCliente.PCL_TEMPO_SEGUNDOS TempoArea, ");
                    break;

                case "SubArea":
                    if (!select.Contains(" SubArea, "))
                        select.Append("SubareaCliente.SAC_DESCRICAO SubArea, ");
                    break;

                case "TipoSubArea":
                    if (!select.Contains(" TipoSubArea, "))
                        select.Append("TipoSubAreaCliente.TSA_DESCRICAO TipoSubArea, ");
                    break;

                case "DataEntradaSubArea":
                case "DataEntradaSubAreaFormatada":
                    if (!select.Contains(" DataEntradaSubArea, "))
                        select.Append("PermanenciaSubarea.PSA_DATA_INICIO DataEntradaSubArea, ");
                    break;

                case "DataSaidaSubArea":
                case "DataSaidaSubAreaFormatada":
                    if (!select.Contains(" DataSaidaSubArea, "))
                        select.Append("PermanenciaSubarea.PSA_DATA_FIM DataSaidaSubArea, ");
                    break;

                case "TempoSubArea":
                case "DescricaoTempoSubArea":
                    if (!select.Contains(" TempoSubArea, "))
                        select.Append("PermanenciaSubarea.PSA_TEMPO_SEGUNDOS TempoSubArea, ");
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPermanencias filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<ParametroSQL> parametros)
        {
            string utcDateTimePattern = "yyyy-MM-ddTHH:mm:ss.fffZ";

            if (filtrosPesquisa.Carga != string.Empty)
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}' ");

            if (filtrosPesquisa.DataCarregamentoInicial.HasValue && filtrosPesquisa.DataCarregamentoInicial.Value != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CARREGAMENTO >= '{filtrosPesquisa.DataCarregamentoInicial.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataCarregamentoFinal.HasValue && filtrosPesquisa.DataCarregamentoFinal.Value != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CARREGAMENTO <= '{filtrosPesquisa.DataCarregamentoFinal.Value.ToString(utcDateTimePattern)}' ");

            if (!string.IsNullOrEmpty(filtrosPesquisa.Placa))
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.Placa}%' ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador.Value} ");

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                where.Append($" and GrupoPessoas.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas.Value} ");

            if (filtrosPesquisa.DataCriacaoCargaInicial.HasValue && filtrosPesquisa.DataCriacaoCargaInicial.Value != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCriacaoCargaInicial.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataCriacaoCargaFinal.HasValue && filtrosPesquisa.DataCriacaoCargaFinal.Value != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataCriacaoCargaFinal.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataAgendamentoColetaInicial.HasValue && filtrosPesquisa.DataAgendamentoColetaInicial.Value != DateTime.MinValue)
                where.Append($" and CargaEntrega.CEN_COLETA = 1 and CargaEntrega.CEN_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoColetaInicial.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataAgendamentoColetaFinal.HasValue && filtrosPesquisa.DataAgendamentoColetaFinal.Value != DateTime.MinValue)
                where.Append($" and CargaEntrega.CEN_COLETA = 1 and CargaEntrega.CEN_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoColetaFinal.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataAgendamentoEntregaInicial.HasValue && filtrosPesquisa.DataAgendamentoEntregaInicial.Value != DateTime.MinValue)
                where.Append($" and CargaEntrega.CEN_COLETA = 0 and CargaEntrega.CEN_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoEntregaInicial.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataAgendamentoEntregaFinal.HasValue && filtrosPesquisa.DataAgendamentoEntregaFinal.Value != DateTime.MinValue)
                where.Append($" and CargaEntrega.CEN_COLETA = 0 and CargaEntrega.CEN_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoEntregaFinal.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.CodigoFilial > 0)
                where.Append($" and Carga.FIL_CODIGO = {filtrosPesquisa.CodigoFilial.Value} ");

            if (filtrosPesquisa.CodigoCliente > 0)
                where.Append($" and Cliente.CLI_CODIGO = {filtrosPesquisa.CodigoCliente.Value} ");

            if (filtrosPesquisa.CodigoTipoParada != null)
                where.Append($" and CargaEntrega.CEN_COLETA = {filtrosPesquisa.CodigoTipoParada.Value} ");
        }

        #endregion
    }
}
