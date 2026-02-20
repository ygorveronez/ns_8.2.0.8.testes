using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.GestaoPallet
{
    public class AgendamentoColetaPallet : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet>
    {

        #region Construtores

        public AgendamentoColetaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public int ContarConsultaAgendamentoColetaPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet filtrosPesquisa)
        {
            var sql = QueryConsultaAgendamento(filtrosPesquisa, true);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.AgendamentoColetaPalletPesquisa> ConsultarAgendamentoColetaPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var sql = QueryConsultaAgendamento(filtrosPesquisa, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.AgendamentoColetaPalletPesquisa)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.AgendamentoColetaPalletPesquisa>();
        }

        public int ObterProximoNumeroSequencial()
        {
            var consultaAgendamentoPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet>();
            int? ultimoNumeroSequencial = consultaAgendamentoPallet.Max(o => (int?)o.NumeroOrdem);

            return ultimoNumeroSequencial.HasValue ? (ultimoNumeroSequencial.Value + 1) : 1;
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet BuscarPorCarga(int codigoCarga)
        {
            var consultaAgendamentoPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet>();

            consultaAgendamentoPallet = consultaAgendamentoPallet.Where(obj => obj.Carga.Codigo == codigoCarga);

            return consultaAgendamentoPallet.FirstOrDefault();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string QueryConsultaAgendamento(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet filtrosPesquisa, bool somenteContarNumeroRegistros, string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append($@"SELECT
                                agendamento.ACP_CODIGO                          AS Codigo,
	                            carga.CAR_CODIGO_CARGA_EMBARCADOR	            AS NumeroCarga,
	                            agendamento.ACP_QUANTIDADE_PALLETS	            AS QuantidadePallets,
	                            agendamento.ACP_DATA_ORDEM	        	        AS DataOrdem,
	                            agendamento.ACP_SITUACAO			            AS Situacao,
	                            agendamento.ACP_RESPONSAVEL_AGENDAMENTO_PALLET	AS Responsavel,
	                            agendamento.ACP_NUMERO_ORDEM                	AS NumeroOrdem,
		                        
	                            filialCarga.FIL_DESCRICAO			            AS Filial,
	                            transportador.EMP_RAZAO			                AS Transportador,
	                            cliente.CLI_NOME			                    AS Cliente,
	                            veiculo.VEI_PLACA			                    AS Veiculo,
	                            solicitante.FUN_NOME			                AS Solicitante,
                                motorista.FUN_NOME			                    AS Motorista
                ");

            sql.Append(" from T_AGENDAMENTO_COLETA_PALLET agendamento ");

            sql.Append(ObterJoinsAgendamento);
            sql.Append(ObterFiltrosControleSaldo(filtrosPesquisa));

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(propOrdenacao))
            {
                sql.Append($" ORDER BY {propOrdenacao} {dirOrdenacao}");

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    sql.Append($" OFFSET {inicioRegistros} ROWS FETCH NEXT {maximoRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        private string ObterJoinsAgendamento => @"
                    left join T_EMPRESA transportador ON transportador.EMP_CODIGO = agendamento.EMP_CODIGO
                    left join T_CLIENTE cliente ON cliente.CLI_CGCCPF = agendamento.CLI_CGCCPF
                    left join T_CARGA carga ON carga.CAR_CODIGO = agendamento.CAR_CODIGO
		            left join T_FILIAL filialCarga ON filialCarga.FIL_CODIGO = carga.FIL_CODIGO
		            left join T_VEICULO veiculo ON veiculo.VEI_CODIGO = agendamento.VEI_CODIGO
                    left join T_FUNCIONARIO motorista ON agendamento.FUM_CODIGO_MOTORISTA = motorista.FUN_CODIGO
		            left join T_FUNCIONARIO Solicitante ON Solicitante.FUN_CODIGO = agendamento.FUN_CODIGO";

        private string ObterFiltrosControleSaldo(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta.FiltroPesquisaAgendamentoColetaPallet filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" WHERE 1 = 1 ");

            if (filtrosPesquisa.ResponsavelAgendamento.HasValue)
            {
                sql.Append($" AND agendamento.ACP_RESPONSAVEL_AGENDAMENTO_PALLET = {(int)filtrosPesquisa.ResponsavelAgendamento}");

                if (filtrosPesquisa.ResponsavelAgendamento == ResponsavelPallet.Transportador)
                    sql.Append($" AND agendamento.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

                if (filtrosPesquisa.ResponsavelAgendamento == ResponsavelPallet.Cliente)
                    sql.Append($" AND agendamento.CLI_CGCCPF = {filtrosPesquisa.CodigoCliente}");
            }

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                sql.Append($" AND (filialCarga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))");

            if (filtrosPesquisa.DataOrdem != default)
                sql.Append($" AND CAST(agendamento.ACP_DATA_ORDEM AS DATE) = '{filtrosPesquisa.DataOrdem:yyyy-MM-dd}'");

            if (filtrosPesquisa.CodigoCarga.Count > 0)
                sql.Append($" AND carga.CAR_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigoCarga)})");

            if (filtrosPesquisa.CodigoCliente > 0)
                sql.Append($" AND agendamento.CLI_CGCCPF = {filtrosPesquisa.CodigoCliente}");

            if (filtrosPesquisa.NumeroOrdem > 0)
                sql.Append($" AND agendamento.ACP_NUMERO_ORDEM = {filtrosPesquisa.NumeroOrdem}");

            if (filtrosPesquisa.StatusAgendamento != null)
                sql.Append($" AND agendamento.ACP_SITUACAO = {(int)filtrosPesquisa.StatusAgendamento.Value}");

            return sql.ToString();
        }

        #endregion Métodos Privados
    }
}
