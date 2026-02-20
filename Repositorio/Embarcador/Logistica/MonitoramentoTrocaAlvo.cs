using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoTrocaAlvo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoTrocaAlvo>
    {
        public MonitoramentoTrocaAlvo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo BuscarTrocasDeAlvoPendentes(int codigoMonitoramento, long codigoPosicaoAtual, long codigoPosicaoAnterior)
        {
            string sql = $@"
                select 
					Monitoramento.MON_CODIGO CodigoMonitoramento,
					Monitoramento.MON_DATA_CRIACAO DataCriacaoMonitoramento,
					Monitoramento.MON_DATA_INICIO DataInicioMonitoramento,
					Monitoramento.MON_DATA_FIM DataFimMonitoramento,
					Monitoramento.CAR_CODIGO CodigoCarga,
					Monitoramento.VEI_CODIGO CodigoVeiculo,
					TipoOperacao.TOP_CODIGO CodigoTipoOperacao,
					case
						when TipoOperacao.TOP_DESLOCAMENTO_VAZIO = 1 then TipoOperacao.TOP_DESLOCAMENTO_VAZIO
						else TipoOperacao.TOP_REALIZAR_BAIXA_ENTRADA_NO_RAIO
					end RealizarBaixaEntradaNoRaio,
					TipoOperacao.TOP_NAO_PROCESSAR_TROCA_ALVO_VIA_MONITORAMENTO NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao,
					PosicaoAtual.POS_CODIGO CodigoPosicaoAtual,
					PosicaoAtual.POS_DATA_VEICULO DataVeiculoPosicaoAtual,
					PosicaoAtual.POS_LATITUDE LatitudePosicaoAtual,
					PosicaoAtual.POS_LONGITUDE LongitudePosicaoAtual,
					PosicaoAtual.POS_EM_ALVO EmAlvoPosicaoAtual,
					PosicaoAtual.POS_EM_LOCAL EmLocalPosicaoAtual,
					case
						when PosicaoAtual.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]  
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvoPosicaoAtual,
					case
						when PosicaoAtual.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, PosicaoAlvoSubarea.SAC_CODIGO) + '-' + convert(varchar, convert(bigint, Subarea.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							JOIN T_POSICAO_ALVO_SUBAREA PosicaoAlvoSubarea on PosicaoAlvoSubarea.POA_CODIGO = PosicaoAlvo.POA_CODIGO
							JOIN T_SUBAREA_CLIENTE Subarea on Subarea.SAC_CODIGO = PosicaoAlvoSubarea.SAC_CODIGO
							WHERE PosicaoAlvo.POS_CODIGO = PosicaoAtual.POS_CODIGO
							FOR XML PATH ('')
						)
						else null
					end CodigosSubareasAlvoPosicaoAtual,
					case
						when PosicaoAtual.POS_EM_LOCAL = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoLocal.LOC_CODIGO)) AS [text()]  
							FROM T_POSICAO_LOCAL PosicaoLocal
							WHERE PosicaoLocal.POS_CODIGO = PosicaoAtual.POS_CODIGO
							FOR XML PATH ('')
						)
						else null
					end CodigosLocaisPosicaoAtual,
					PosicaoAnterior.POS_CODIGO CodigoPosicaoAnterior,
					PosicaoAnterior.POS_DATA_VEICULO DataVeiculoPosicaoAnterior,
					PosicaoAnterior.POS_LATITUDE LatitudePosicaoAnterior,
					PosicaoAnterior.POS_LONGITUDE LongitudePosicaoAnterior,
					PosicaoAnterior.POS_EM_ALVO EmAlvoPosicaoAnterior,
					PosicaoAnterior.POS_EM_LOCAL EmLocalPosicaoAnterior,
					case
						when PosicaoAnterior.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = PosicaoAnterior.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvoPosicaoAnterior,
					case
						when PosicaoAnterior.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, PosicaoAlvoSubarea.SAC_CODIGO) + '-' + convert(varchar, convert(bigint, Subarea.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							JOIN T_POSICAO_ALVO_SUBAREA PosicaoAlvoSubarea on PosicaoAlvoSubarea.POA_CODIGO = PosicaoAlvo.POA_CODIGO
							JOIN T_SUBAREA_CLIENTE Subarea on Subarea.SAC_CODIGO = PosicaoAlvoSubarea.SAC_CODIGO
							WHERE PosicaoAlvo.POS_CODIGO = PosicaoAnterior.POS_CODIGO
							FOR XML PATH ('')
						)
						else null
					end CodigosSubareasAlvoPosicaoAnterior,
					case
						when PosicaoAnterior.POS_EM_LOCAL = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoLocal.LOC_CODIGO)) AS [text()]
							FROM T_POSICAO_LOCAL PosicaoLocal
							WHERE PosicaoLocal.POS_CODIGO = PosicaoAnterior.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosLocaisPosicaoAnterior,
					(
						SELECT ',' + convert(varchar, convert(bigint, CargaEntrega.CLI_CODIGO_ENTREGA))  + ',' + ISNULL(convert(varchar, convert(bigint, areaRedex.CLI_CGCCPF_AREA_REDEX)), '')  AS [text()]
						FROM T_CARGA_ENTREGA CargaEntrega
						left outer join 
						T_CLIENTE_AREA_REDEX areaRedex on areaRedex.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
						WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO 
						FOR XML PATH ('')
					) CodigosClientesEntregas
				from
					t_monitoramento Monitoramento 
				left join
					t_carga Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO
				left join
					t_tipo_operacao TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
				left join
					t_posicao PosicaoAtual on PosicaoAtual.POS_CODIGO = {codigoPosicaoAtual}
				left join
					t_posicao PosicaoAnterior on PosicaoAnterior.POS_CODIGO = {codigoPosicaoAnterior}
				where
					Monitoramento.MON_CODIGO = {codigoMonitoramento}";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo)));
            Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo monitoramentoTrocaDeAlvo = query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo>();
            return monitoramentoTrocaDeAlvo;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado> BuscarTrocasDeAlvoPendentesMonitoramentosPorCodigos(List<int> codigoMonitoramento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado> result = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado>();

            int take = 600;
            int start = 0;
            while (start < codigoMonitoramento?.Count)
            {
                List<int> tmpMonitoramentos = codigoMonitoramento.Skip(start).Take(take).ToList();

                string sql = $@"
                		select 
					Monitoramento.MON_CODIGO CodigoMonitoramento,
					Monitoramento.MON_DATA_CRIACAO DataCriacaoMonitoramento,
					Monitoramento.MON_DATA_INICIO DataInicioMonitoramento,
					Monitoramento.MON_DATA_FIM DataFimMonitoramento,
					Monitoramento.CAR_CODIGO CodigoCarga,
					Monitoramento.VEI_CODIGO CodigoVeiculo,
					TipoOperacao.TOP_CODIGO CodigoTipoOperacao,
					case
						when TipoOperacao.TOP_DESLOCAMENTO_VAZIO = 1 then TipoOperacao.TOP_DESLOCAMENTO_VAZIO
						else TipoOperacao.TOP_REALIZAR_BAIXA_ENTRADA_NO_RAIO
					end RealizarBaixaEntradaNoRaio,
					TipoOperacao.TOP_NAO_PROCESSAR_TROCA_ALVO_VIA_MONITORAMENTO NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao,
					(
						SELECT ',' + convert(varchar, convert(bigint, CargaEntrega.CLI_CODIGO_ENTREGA))  + ',' + ISNULL(convert(varchar, convert(bigint, areaRedex.CLI_CGCCPF_AREA_REDEX)), '')  AS [text()]
						FROM T_CARGA_ENTREGA CargaEntrega
						left outer join 
						T_CLIENTE_AREA_REDEX areaRedex on areaRedex.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
						WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO 
						FOR XML PATH ('')
					) CodigosClientesEntregas
				from
					t_monitoramento Monitoramento 
				left join
					t_carga Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO
				left join
					t_tipo_operacao TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
				where
					Monitoramento.MON_CODIGO in ({string.Join(",", tmpMonitoramentos)})";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado)));
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado> monitoramentoTrocaDeAlvo = (List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado>)query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentosTrocaAlvoSimplificado>();
                result.AddRange(monitoramentoTrocaDeAlvo);

                start += take;
            }

            return result;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes> BuscarPosicoesTrocaAlvoPorCodigos(List<long> codigoPosicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes> result = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes>();

            int take = 600;
            int start = 0;
            while (start < codigoPosicoes?.Count)
            {
                List<long> tmpPosicoes = codigoPosicoes.Skip(start).Take(take).ToList();

                string sql = $@"
                select
					posicao.POS_CODIGO CodigoPosicao,
					posicao.POS_DATA_VEICULO DataVeiculoPosicao,
					posicao.POS_LATITUDE LatitudePosicao,
					posicao.POS_LONGITUDE LongitudePosicao,
					posicao.POS_EM_ALVO EmAlvoPosicao,
					posicao.POS_EM_LOCAL EmLocalPosicao,
					case
						when posicao.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = posicao.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvoPosicao,
					case
						when posicao.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, PosicaoAlvoSubarea.SAC_CODIGO) + '-' + convert(varchar, convert(bigint, Subarea.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							JOIN T_POSICAO_ALVO_SUBAREA PosicaoAlvoSubarea on PosicaoAlvoSubarea.POA_CODIGO = PosicaoAlvo.POA_CODIGO
							JOIN T_SUBAREA_CLIENTE Subarea on Subarea.SAC_CODIGO = PosicaoAlvoSubarea.SAC_CODIGO
							WHERE PosicaoAlvo.POS_CODIGO = posicao.POS_CODIGO
							FOR XML PATH ('')
						)
						else null
					end CodigosSubareasAlvoPosicao,
					case
						when posicao.POS_EM_LOCAL = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoLocal.LOC_CODIGO)) AS [text()]
							FROM T_POSICAO_LOCAL PosicaoLocal
							WHERE PosicaoLocal.POS_CODIGO = posicao.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosLocaisPosicao
					from T_POSICAO posicao
					where posicao.POS_CODIGO in ({string.Join(",", tmpPosicoes)})";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes)));
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes> posicoestrocaAlvo = (List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes>)query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaAlvoPosicoes>();
                result.AddRange(posicoestrocaAlvo);

                start += take;
            }

            return result;
        }


        public void ExcluiPorCodigos(List<int> codigos, DbConnection connection, DbTransaction transaction)
        {
            if (codigos != null && codigos.Count > 0)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandTimeout = 300;
                command.Transaction = transaction;

                command.CommandText = "DELETE t_monitoramento_troca_alvo WHERE mta_codigo in (" + string.Join(",", codigos) + ")"; // SQL-INJECTION-SAFE
                command.ExecuteNonQuery();
            }
        }

    }
}
