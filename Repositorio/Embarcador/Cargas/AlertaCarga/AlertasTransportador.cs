using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class AlertasTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCarregamento>
    {
        public AlertasTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContarNumeroRegistros, int codigoTransportador)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, somenteContarNumeroRegistros, parametrosConsulta, codigoTransportador));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta)));

            return consulta.SetTimeout(120).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta>();

        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa,int codigoTransportador)
        {

            var countConsulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, true, null, codigoTransportador));

            return countConsulta.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, int codigoTransportador)
        {
            string sql;
            string pattern = "yyyy-MM-dd";

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0)) ";
            else
                sql = @"select
                            Alerta.ALE_CODIGO Codigo,
                            MonitoramentoEvento.MEV_DESCRICAO as NomeAlerta,
                            Alerta.ALE_DESCRICAO as Descricao,
                            Alerta.ALE_STATUS as Status,
                            Alerta.ALE_LATITUDE as Latitude,
                            Alerta.ALE_LONGITUDE as Longitude,
                            Alerta.ALE_DATA as Data,
                            Alerta.ALE_DATA_FIM as DataFim,
                            Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador,
                            TipoOperacao.TOP_DESCRICAO as TipoOperacao,
                            Veiculo.VEI_PLACA as Placa,
                            Motorista.FUN_NOME as Motorista,
                            Empresa.EMP_RAZAO as Transportador,
                            Alerta.ALE_DATA_FIM as DataTratativa,
                            IsNull(Responsavel.FUN_NOME, '-') as Responsavel,
                            AlertaTratativaAcao.ATC_DESCRICAO as Acao ";

            sql += @"  from
                        T_ALERTA_MONITOR as Alerta 
                    left join
                        T_MONITORAMENTO_EVENTO MonitoramentoEvento 
                            on MonitoramentoEvento.MEV_CODIGO = Alerta.MEV_CODIGO 
                    inner join
                        T_CARGA Carga 
                            on Carga.CAR_CODIGO = Alerta.CAR_CODIGO 
                    left join
                        T_TIPO_OPERACAO TipoOperacao 
                            on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO 
                    join
                        T_VEICULO Veiculo 
                            on Veiculo.VEI_CODIGO = Alerta.VEI_CODIGO 
                    left join
                        T_CARGA_MOTORISTA CargaMotorista 
                            on CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO 
                    left join
                        T_FUNCIONARIO Motorista 
                            on Motorista.FUN_CODIGO = Alerta.FUN_CODIGO_MOTORISTA
                    left join
                        T_EMPRESA Empresa 
                            on Empresa.EMP_CODIGO = Carga.EMP_CODIGO 
                    left join
                        T_ALERTA_TRATATIVA AlertaTratativa 
                            on AlertaTratativa.ALE_CODIGO = Alerta.ALE_CODIGO  
                    left join
                        T_ALERTA_TRATATIVA_ACAO AlertaTratativaAcao 
                            on AlertaTratativaAcao.ATC_CODIGO = AlertaTratativa.ATC_CODIGO
                    left join
                        T_FUNCIONARIO Responsavel 
                            on Responsavel.FUN_CODIGO = Alerta.FUN_CODIGO  ";

            sql += $"where 1=1 and Empresa.EMP_CODIGO = {codigoTransportador}";

            if (filtrosPesquisa.AlertaMonitorStatus != Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos)
                sql += $" and Alerta.ALE_STATUS = {(int)filtrosPesquisa.AlertaMonitorStatus}";

            if (filtrosPesquisa.ApenasComPosicaoTardia)
                sql += $" and Alerta.ALE_POSICAO_RETROATIVA = 1";

            if (filtrosPesquisa.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta)
                sql += $" and Alerta.ALE_TIPO = {(int)filtrosPesquisa.TipoAlerta}";

            if (filtrosPesquisa.DataInicial != null)
                sql += $" and Alerta.ALE_DATA >= convert(datetime, '{filtrosPesquisa.DataInicial.Value.ToString("yyyy-MM-dd HH:mm:ss")}', 102) ";

            if (filtrosPesquisa.DataFinal != null)
                sql += $" and Alerta.ALE_DATA <= convert(datetime, '{filtrosPesquisa.DataFinal.Value.ToString("yyyy-MM-dd HH:mm:ss")}', 102) ";

            if (filtrosPesquisa.CodigoCargaEmbarcador != "")
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    sql += $" and Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.CodigoCargaEmbarcador}%'";
                else
                    sql += $" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'";
            }

            if (filtrosPesquisa.Transportador > 0)
                sql += $" and Empresa.EMP_CODIGO = {filtrosPesquisa.Transportador}";

            if (filtrosPesquisa.Placa != "")
                sql += $" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.Placa}%'";

            if (filtrosPesquisa.Motorista > 0)
                sql += $" and Motorista.FUN_CODIGO = {filtrosPesquisa.Motorista}";


            if (filtrosPesquisa.Filiais.Any(codigo => codigo == -1))
            {
                sql += $@" AND ( Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )";
            }
            else if (filtrosPesquisa.Filiais.Count > 0)
                sql += $@" AND Carga.FIL_CODIGO in ({string.Join(",", filtrosPesquisa.Filiais)})";

            if (parametrosConsulta != null && !somenteContarNumeroRegistros)
            {

                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ASC" : "DESC ")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }
        #endregion
    }
}

