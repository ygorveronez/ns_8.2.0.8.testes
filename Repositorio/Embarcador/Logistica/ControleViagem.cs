using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ControleViagem : RepositorioBase<Dominio.Entidades.EntidadeBase>
    {
        #region Atributos privados

        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        #endregion

        #region Atributos públicos

        public ControleViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.ControleViagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {

            string sql = GetSQLSelect() + GetSQLFrom();
            sql += GetFiltroMonitoramento(filtrosPesquisa);

            sql = sql + $" order by {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}";
            if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                sql = sql + $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.ControleViagem)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.ControleViagem>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem filtrosPesquisa)
        {
            string sqlContar = $"SELECT COUNT(*) CONTADOR " + GetSQLFrom(); // SQL-INJECTION-SAFE
            sqlContar += GetFiltroMonitoramento(filtrosPesquisa);

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlContar);
            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private string GetSQLSelect()
        {
            return $@"
                SELECT 
                    Monitoramento.MON_CODIGO MonitoramentoCodigo,
                    Carga.CAR_CODIGO CargaCodigo,
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR CargaCodigoEmbarcador,
                    Carga.CAR_DATA_CRIACAO DataCriacaoCarga,
                    Carga.CAR_SITUACAO CargaSituacao,
                    Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM DataPrevisaoChegadaPlanta,
					Filial.FIL_CODIGO FilialCodigo,
					Filial.FIL_CNPJ FilialCNPJ,
                    Filial.FIL_DESCRICAO FilialDescricao,
                    Empresa.EMP_CODIGO TransportadorCodigo,
                    Empresa.EMP_RAZAO TransportadorRazaoSocial,
                    Empresa.EMP_FANTASIA TransportadorNomeFantasia,
                    VeiculoTracao.VEI_PLACA VeiculoTracaoPlaca,
					Operacao.TOP_DESCRICAO as Operacao,
					Rastreador.TRA_DESCRICAO TecnologiaRastreadorDescricao,
					MonitoramentoStatus.MSV_DESCRICAO StatusViagem,
                    Monitoramento.MON_DISTANCIA_PREVISTA DistanciaPrevista,
                    Monitoramento.MON_DISTANCIA_REALIZADA DistanciaRealizada,
                    Monitoramento.MON_DISTANCIA_ATE_ORIGEM DistanciaAteOrigem,
                    Monitoramento.MON_DISTANCIA_ATE_DESTINO DistanciaAteDestino,
                    Posicao.POS_DESCRICAO PosicaoDescricao,
                    Posicao.POS_DATA_VEICULO PosicaoDataVeiculo,
                    Posicao.POS_EM_ALVO EmAlvo,
					CASE 
						WHEN Posicao.POS_EM_ALVO = 1 THEN
							SUBSTRING((SELECT ',' + convert(varchar, convert(bigint, CLI_CGCCPF)) AS [text()]
									FROM T_POSICAO_ALVO PosicaoAlvo
									WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO
									FOR XML PATH ('')), 2, 2000) 
					    ELSE null
					END CodigosClientesAlvos,
                    SUBSTRING((SELECT ', '+ Reboque.VEI_PLACA  AS [text()]
                                               FROM T_MONITORAMENTO_REBOQUES MonitoramentoReboques
                                               JOIN T_VEICULO Reboque ON Reboque.VEI_CODIGO = MonitoramentoReboques.VEI_CODIGO
                                               WHERE MonitoramentoReboques.MON_CODIGO = Monitoramento.MON_CODIGO
                                              ORDER BY MonitoramentoReboques.MON_CODIGO
                                              FOR XML PATH ('')), 3, 1000
					) VeiculoReboquePlaca,
                    CASE
                        WHEN (
							SELECT top 1 CargaEntrega1.CEN_COLETA
							FROM T_CARGA_ENTREGA as CargaEntrega1 
							WHERE CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
							ORDER BY CargaEntrega1.CEN_ORDEM desc
						) = 1 then 'Coleta'
                        ELSE 'Entrega'
                    END Processo,
					(
                        SELECT top 1 Pedido.PED_NUMERO_PEDIDO_EMBARCADOR
			            FROM t_carga_pedido CargaPedido 
			            JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
						JOIN T_CARGA_ENTREGA as CargaEntrega1 ON CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
			            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
						and Pedido.CLI_CODIGO = CargaEntrega1.CLI_CODIGO_ENTREGA
			            ORDER BY CargaEntrega1.CEN_ORDEM desc, Pedido.PED_ORDEM
                    ) Pedido,
                    (
                        SELECT top 1 Pedido.CAR_DATA_CARREGAMENTO_PEDIDO
			            FROM t_carga_pedido CargaPedido 
			            JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
						JOIN T_CARGA_ENTREGA as CargaEntrega1 ON CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
			            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
						and Pedido.CLI_CODIGO = CargaEntrega1.CLI_CODIGO_ENTREGA
			            ORDER BY CargaEntrega1.CEN_ORDEM desc, Pedido.PED_ORDEM
                    ) DataCarregamentoPedido,
                    (
						SELECT top 1 PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM
			            FROM t_carga_pedido CargaPedido 
			            JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
						JOIN T_CARGA_ENTREGA as CargaEntrega1 ON CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
						JOIN T_PRE_CARGA as PreCarga on PreCarga.PCA_CODIGO = Pedido.PCA_CODIGO
			            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
						and Pedido.CLI_CODIGO = CargaEntrega1.CLI_CODIGO_ENTREGA
			            ORDER BY CargaEntrega1.CEN_ORDEM desc, Pedido.PED_ORDEM
					) DataPrevisaoInicioViagem,
					(
						SELECT top 1 CargaEntrega1.CEN_DATA_ENTREGA_REPROGRAMADA
						FROM T_CARGA_ENTREGA as CargaEntrega1 
						WHERE CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
						ORDER BY CargaEntrega1.CEN_ORDEM desc
					) DataPrevista,
					(
						SELECT top 1 Cliente.CLI_CGCCPF
						FROM T_CARGA_ENTREGA as CargaEntrega1 
						JOIN T_CLIENTE as Cliente ON Cliente.CLI_CGCCPF = CargaEntrega1.CLI_CODIGO_ENTREGA
						WHERE CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
						ORDER BY CargaEntrega1.CEN_ORDEM desc
					) ClienteDestinoCodigo,
					(
						SELECT top 1 Cliente.CLI_NOME
						FROM T_CARGA_ENTREGA as CargaEntrega1 
						JOIN T_CLIENTE as Cliente ON Cliente.CLI_CGCCPF = CargaEntrega1.CLI_CODIGO_ENTREGA
						WHERE CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
						ORDER BY CargaEntrega1.CEN_ORDEM desc
					) ClienteDestinoNome,
                    (
						SELECT top 1 Localidades.LOC_DESCRICAO + '/' + Localidades.UF_SIGLA
						FROM T_CARGA_ENTREGA as CargaEntrega1 
						JOIN T_CLIENTE as Cliente ON Cliente.CLI_CGCCPF = CargaEntrega1.CLI_CODIGO_ENTREGA
                        JOIN T_LOCALIDADES as Localidades ON Localidades.LOC_CODIGO = Cliente.LOC_CODIGO
						WHERE CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
						ORDER BY CargaEntrega1.CEN_ORDEM desc
					) ClienteDestinoLocalidade
                    ";
        }

        private string GetSQLFrom()
        {
            return $@"
                from T_MONITORAMENTO as Monitoramento
                join T_CARGA as Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO
                join T_VEICULO as VeiculoTracao on VeiculoTracao.VEI_CODIGO =  Carga.CAR_VEICULO
                join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO
                join T_EMPRESA as Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                join T_TIPO_OPERACAO as Operacao on Operacao.TOP_CODIGO = Carga.TOP_CODIGO
                left join T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatus on MonitoramentoStatus.MSV_CODIGO = Monitoramento.MSV_CODIGO
                left join T_RASTREADOR_TECNOLOGIA Rastreador on Rastreador.TRA_CODIGO = VeiculoTracao.TRA_CODIGO
                left join T_POSICAO Posicao on Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO
                ";
        }

        private string GetFiltroMonitoramento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem filtrosPesquisa)
        {
            string filtro = $@"
                WHERE Monitoramento.MON_STATUS = 1 and Carga.CAR_CARGA_DE_PRE_CARGA = 1 and CAR_CARGA_FECHADA = 1
                ";

            if (filtrosPesquisa.DataInicialCarga != null)
                filtro += $" AND Carga.CAR_DATA_CRIACAO >= convert(datetime, '{filtrosPesquisa.DataInicialCarga.Value.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.DataFinalCarga != null)
                filtro += $" AND Carga.CAR_DATA_CRIACAO <= convert(datetime, '{filtrosPesquisa.DataFinalCarga.Value.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                filtro = filtro + $" and Filial.FIL_CODIGO in ({ string.Join(", ", filtrosPesquisa.CodigosFilial)}) ";

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
                filtro = filtro + $" and T_EMPRESA.EMP_CODIGO in ({ string.Join(", ", filtrosPesquisa.CodigosTransportador)}) ";

            if (filtrosPesquisa.CodigosVeiculos.Count > 0)
                filtro = filtro + $" and VeiculoTracao.VEI_CODIGO in ({ string.Join(", ", filtrosPesquisa.CodigosVeiculos)}) ";

            if (filtrosPesquisa.CodigoClienteDestino > 0)
                filtro = filtro + $@" and (
                                              SELECT top 1 Cliente.CLI_CGCCPF
                                              FROM T_CARGA_ENTREGA as CargaEntrega1
                                              JOIN T_CLIENTE as Cliente ON Cliente.CLI_CGCCPF = CargaEntrega1.CLI_CODIGO_ENTREGA
                                              WHERE CargaEntrega1.CAR_CODIGO = Carga.CAR_CODIGO
                                              ORDER BY CargaEntrega1.CEN_ORDEM desc
						                  ) = {filtrosPesquisa.CodigoClienteDestino} ";

            if (filtrosPesquisa.CodigosStatusViagem != null && filtrosPesquisa.CodigosStatusViagem.Count > 0)
            {
                filtro = filtro + " and (";
                if (filtrosPesquisa.CodigosStatusViagem.Contains(-1))
                {
                    filtro = filtro + $" Monitoramento.MSV_CODIGO is null or ";
                }
                filtro = filtro + $" Monitoramento.MSV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosStatusViagem)}) )";
            }

            return filtro;
        }

        #endregion

        #region Relatorios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.ControleTempoViagem> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaControleTempoViagem().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.ControleTempoViagem)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ControleTempoViagem>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaControleTempoViagem().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
