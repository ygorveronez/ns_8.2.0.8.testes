using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoSaidaAlvo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoSaidaAlvo>
    {
        public MonitoramentoSaidaAlvo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> BuscarSaidasDeAlvoPendentes(List<int> codigosMonitoramento, int quantidadeRegistros)
        {
            string sql = $@"
				select top {quantidadeRegistros}
	                SaidaAlvo.MSA_CODIGO CodigoSaidaAlvo,
	                Monitoramento.MON_CODIGO CodigoMonitoramento,
                    Monitoramento.POS_ULTIMA_POSICAO CodigoUltimaPosicaoMonitoramento,
                    Carga.CAR_CODIGO CodigoCarga,
                    TipoOperacao.TOP_CODIGO CodigoTipoOperacao,
					TipoOperacao.TOP_NAO_PROCESSAR_TROCA_ALVO_VIA_MONITORAMENTO NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao,
                    SaidaAlvo.POS_CODIGO CodigoPosicao,
                    Posicao.POS_DATA_VEICULO DataVeiculoPosicao,
                    Posicao.POS_LATITUDE LatitudePosicao,
                    Posicao.POS_LONGITUDE LongitudePosicao,
	                SaidaAlvo.CEN_CODIGO CodigoCargaEntrega,
	                SaidaAlvo.CLI_CGCCPF CodigoCliente,
					(
						SELECT ',' + convert(varchar, convert(bigint, CargaEntrega.CLI_CODIGO_ENTREGA)) AS [text()]
						FROM T_CARGA_ENTREGA CargaEntrega
						WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
						FOR XML PATH ('')
					) CodigosClientesEntregas
                from 
	                t_monitoramento_saida_alvo SaidaAlvo
                join
					t_monitoramento Monitoramento on Monitoramento.MON_CODIGO = SaidaAlvo.MON_CODIGO
				join
					t_carga Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO
				join
					t_tipo_operacao TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                join
                    t_posicao Posicao on Posicao.POS_CODIGO = SaidaAlvo.POS_CODIGO
                where
                    Monitoramento.MON_CODIGO in ({string.Join(",", codigosMonitoramento)})
                order by
					SaidaAlvo.MSA_CODIGO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo)));
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> monitoramentoSaidasDeAlvo = query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo>();
            return monitoramentoSaidasDeAlvo;
        }

        public void ExcluiPorCodigos(List<int> codigos, DbConnection connection, DbTransaction transaction)
        {
            if (codigos != null && codigos.Count > 0)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandTimeout = 300;
                command.Transaction = transaction;

                command.CommandText = "DELETE t_monitoramento_saida_alvo WHERE msa_codigo in (" + string.Join(",", codigos) + ")"; // SQL-INJECTION-SAFE
                command.ExecuteNonQuery();
            }
        }
    }
}
