using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPallet
{
	public class ManutencaoPallet : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.ManutencaoPallet>
	{

		#region Construtores

		public ManutencaoPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

		#endregion Construtores

		#region Métodos Públicos

		#region Métodos Públicos Consulta

		public async Task<List<ConsultaManutencaoPallet>> ObterManutencaoPalletAsync(FiltroPesquisaManutencaoPallet filtrosPesquisaManutencaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
		{
			var consulta = QueryConsultaControlePallet(filtrosPesquisaManutencaoPallet, parametrosConsulta, somenteContar: false);

			consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ConsultaManutencaoPallet)));

			return (await consulta.SetTimeout(600).ListAsync<ConsultaManutencaoPallet>()).ToList();
		}

		public async Task<int> ContarManutencaoPalletAsync(FiltroPesquisaManutencaoPallet filtrosPesquisaManutencaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
		{
			var consulta = QueryConsultaControlePallet(filtrosPesquisaManutencaoPallet, parametrosConsulta, somenteContar: true);

			return await consulta.SetTimeout(600).UniqueResultAsync<int>();
		}

		public async Task<TotalizadorManutencaoPallet> ObterTotalizadorManutencaoPalletAsync(FiltroPesquisaManutencaoPallet filtrosPesquisaManutencaoPallet)
		{
			var consulta = QueryConsultaTotalizadorManutencaoPallet(filtrosPesquisaManutencaoPallet);

			consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(TotalizadorManutencaoPallet)));

			return await consulta.SetTimeout(600).UniqueResultAsync<TotalizadorManutencaoPallet>();
		}

		#endregion Métodos Públicos Consulta

		#endregion Métodos Públicos

		#region Métodos Privados

		#region Métodos Privados Consulta

		private NHibernate.IQuery QueryConsultaControlePallet(FiltroPesquisaManutencaoPallet filtrosPesquisaManutencaoPallet, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContar)
		{
			StringBuilder sql = new StringBuilder();

			if (somenteContar)
				sql.Append(@"select distinct(count(0) over ())");
			else
			{
				sql.Append(@"
                select
                       ManutencaoPallet.MNP_CODIGO as Codigo,
                       XmlNotaFiscal.NF_NUMERO as NumeroNotaFiscal,
	                   Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga,
                       ManutencaoPallet.MNP_QUANTIDADE_PALLET as QuantidadePallet,
                       ManutencaoPallet.MNP_DATA_CRIACAO as DataCriacao,
	                   Filial.FIL_DESCRICAO as Filial, 
	                   Filial.FIL_CNPJ as FilialCNPJ, 
                       ManutencaoPallet.MNP_TIPO_MANUTENCAO_PALLET as TipoManutencaoPallet,
	                   ManutencaoPallet.MNP_TIPO_MOVIMENTACAO as TipoMovimentacao,
                       ManutencaoPallet.MNP_OBSERVACAO as Observacao ");
			}

			ObterJoinsManutencaoPallet(sql);

			sql.Append(ObterFiltrosManutencaoPallet(filtrosPesquisaManutencaoPallet));

			if (!somenteContar)
			{
				sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

				if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
					sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
			}

			return SessionNHiBernate.CreateSQLQuery(sql.ToString());
		}

		private void ObterJoinsManutencaoPallet(StringBuilder from)
		{
			from.Append(@" from T_MANUTENCAO_PALLET ManutencaoPallet");
			from.Append(@" left join T_XML_NOTA_FISCAL XmlNotaFiscal on ManutencaoPallet.NFX_CODIGO = XmlNotaFiscal.NFX_CODIGO");
			from.Append(@" left join T_FILIAL Filial on ManutencaoPallet.FIL_CODIGO = Filial.FIL_CODIGO");
			from.Append(@" left join T_CARGA Carga on ManutencaoPallet.CAR_CODIGO = Carga.CAR_CODIGO");
		}

		private StringBuilder ObterFiltrosManutencaoPallet(FiltroPesquisaManutencaoPallet filtrosPesquisaManutencaoPallet)
		{
			StringBuilder clausulaWhere = new StringBuilder(" WHERE 1 = 1");
			string pattern = "yyyy-MM-dd";

			if (!string.IsNullOrEmpty(filtrosPesquisaManutencaoPallet.Carga))
				clausulaWhere.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisaManutencaoPallet.Carga}'");

			if (filtrosPesquisaManutencaoPallet.Filial > 0)
				clausulaWhere.Append($" and ManutencaoPallet.FIL_CODIGO = {filtrosPesquisaManutencaoPallet.Filial}");

			if (filtrosPesquisaManutencaoPallet.Transportador > 0)
				clausulaWhere.Append($" and Carga.EMP_CODIGO = {filtrosPesquisaManutencaoPallet.Transportador}");

			if (filtrosPesquisaManutencaoPallet.Cliente > 0)
				clausulaWhere.Append($" and XmlNotaFiscal.CLI_CODIGO_DESTINATARIO = {filtrosPesquisaManutencaoPallet.Cliente}");

			if (filtrosPesquisaManutencaoPallet.DataInicialMovimentacao != DateTime.MinValue)
				clausulaWhere.Append($" and FORMAT(ManutencaoPallet.MNP_DATA_CRIACAO, '{pattern}') >= '{filtrosPesquisaManutencaoPallet.DataInicialMovimentacao.ToString(pattern)}'");

			if (filtrosPesquisaManutencaoPallet.DataFinalMovimentacao != DateTime.MinValue)
				clausulaWhere.Append($" and FORMAT(ManutencaoPallet.MNP_DATA_CRIACAO, '{pattern}') <= '{filtrosPesquisaManutencaoPallet.DataFinalMovimentacao.ToString(pattern)}'");

			if (filtrosPesquisaManutencaoPallet.TipoManutencaoPallet.HasValue)
				clausulaWhere.Append($" and ManutencaoPallet.MNP_TIPO_MANUTENCAO_PALLET = {(int)filtrosPesquisaManutencaoPallet.TipoManutencaoPallet.Value}");

			if (filtrosPesquisaManutencaoPallet.TipoMovimentacao != TipoEntradaSaida.Todos)
				clausulaWhere.Append($" and ManutencaoPallet.MNP_TIPO_MOVIMENTACAO = {(int)filtrosPesquisaManutencaoPallet.TipoMovimentacao}");

			return clausulaWhere;
		}

		private NHibernate.IQuery QueryConsultaTotalizadorManutencaoPallet(FiltroPesquisaManutencaoPallet filtrosPesquisaManutencaoPallet)
		{
			StringBuilder sql = new StringBuilder();

			sql.Append(@$"
                    select 
                        ({ObterSqlTotalManutentcao(filtrosPesquisaManutencaoPallet)}) as EmManutencao,
                        sum(case when ManutencaoPallet.MNP_TIPO_MANUTENCAO_PALLET = {(int)TipoManutencaoPallet.Disponivel} then ManutencaoPallet.MNP_QUANTIDADE_PALLET else 0 end) as Disponivel,
                        sum(case when ManutencaoPallet.MNP_TIPO_MANUTENCAO_PALLET = {(int)TipoManutencaoPallet.Descarte} then ManutencaoPallet.MNP_QUANTIDADE_PALLET else 0 end) as Descarte,
                        sum(case when ManutencaoPallet.MNP_TIPO_MANUTENCAO_PALLET = {(int)TipoManutencaoPallet.Sucata} then ManutencaoPallet.MNP_QUANTIDADE_PALLET else 0 end) as Sucata "); // SQL-INJECTION-SAFE

            ObterJoinsManutencaoPallet(sql);

			sql.Append(ObterFiltrosManutencaoPallet(filtrosPesquisaManutencaoPallet));

			return this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
		}

		private string ObterSqlTotalManutentcao(FiltroPesquisaManutencaoPallet filtrosPesquisa)
		{
			StringBuilder sql = new StringBuilder("SELECT SUM(CPT_QUANTIDADE_TOTAL_PALLETS) FROM T_CONTROLE_ESTOQUE_PALLET");

			sql.Append($" where CPT_TIPO_ESTOQUE_PALLET = {(int)TipoEstoquePallet.Manutencao}");

			if (filtrosPesquisa.Filial > 0)
				sql.Append($" and Fil_CODIGO = {filtrosPesquisa.Filial}");

			return sql.ToString();
		}

		#endregion Métodos Privados Consulta

		#endregion Métodos Privados
	}
}
