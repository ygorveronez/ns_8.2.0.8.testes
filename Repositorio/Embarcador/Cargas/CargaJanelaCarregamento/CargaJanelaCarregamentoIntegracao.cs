using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>
	{

		#region Construtores

		public CargaJanelaCarregamentoIntegracao(UnitOfWork unitOfWork) :base(unitOfWork) { }

		#endregion

		#region Métodos Públicos

		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao BuscarPorJanelaCarregamento(int codigoJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoRecebimento? tipoRetornoRecebimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEventoIntegracaoJanelaCarregamento? tipoEvento)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>()
				.Where(o => o.CargaJanelaCarregamento.Codigo == codigoJanelaCarregamento);

			if (tipoRetornoRecebimento.HasValue)
				query = query.Where(o => o.TipoRetornoRecebimento == tipoRetornoRecebimento.Value);

			if (tipoEvento.HasValue)
				query = query.Where(o => o.TipoEvento == tipoEvento.Value);

			return query.FirstOrDefault();
		}

		public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> BuscarIntegracoesLeilaoPendentesDeEnvio()
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>()
				.Where(o => 
					o.CargaJanelaCarregamentoViagem != null &&
					o.TipoRetornoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoRecebimento.Retorno &&
					o.TipoEvento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEventoIntegracaoJanelaCarregamento.ResultadoLeilao &&
					o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
				);

			return query.ToList();
		}

		public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> BuscarIntegracoesPendentesDeEnvio()
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>()
				.Where(o =>
					o.CargaJanelaCarregamento != null && o.TipoRetornoRecebimento == 0 && o.TipoEvento == 0 &&
					o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
				);

			return query.ToList();
		}

		public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> BuscarIntegracoesAguardandoRetorno()
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>()
				.Where(o =>
					o.CargaJanelaCarregamento != null && o.TipoRetornoRecebimento == 0 && o.TipoEvento == 0 &&
					o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno
				);

			return query.ToList();
		}

		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao BuscarPorCarga(int codigoCarga)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>()
				.Where(o => o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga);

			return query.FirstOrDefault();
		}

		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao BuscarPorJanelaCarregamentoViagem(int codigoJanelaCarregamentoViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoRecebimento? tipoRetornoRecebimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEventoIntegracaoJanelaCarregamento? tipoEvento)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>()
				.Where(o => o.CargaJanelaCarregamentoViagem.Codigo == codigoJanelaCarregamentoViagem);

			if (tipoRetornoRecebimento.HasValue)
				query = query.Where(o => o.TipoRetornoRecebimento == tipoRetornoRecebimento.Value);

			if (tipoEvento.HasValue)
				query = query.Where(o => o.TipoEvento == tipoEvento.Value);

			return query.FirstOrDefault();
		}

		public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoIntegracao filtrosPesquisa)
		{
			var result = Consultar(filtrosPesquisa);

			return result.Count();
		}

		public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
		{
			var result = Consultar(filtrosPesquisa);

			return ObterLista(result, parametrosConsulta);
		}

		public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaCarregamentoIntegracao> ConsultarRelatorioJanelaCarregamentoIntegracao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
		{
			var consultaJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Logistica.ConsultaJanelaCarregamentoIntegracao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

			consultaJanelaCarregamentoIntegracao.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaCarregamentoIntegracao)));

			return consultaJanelaCarregamentoIntegracao.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaCarregamentoIntegracao>();
		}

		public int ContarConsultaRelatorioJanelaCarregamentoIntegracao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
		{
			var consultaJanelaCarregamentIntegracao = new Repositorio.Embarcador.Logistica.ConsultaJanelaCarregamentoIntegracao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

			return consultaJanelaCarregamentIntegracao.SetTimeout(1200).UniqueResult<int>();
		}

		#endregion

		#region Métodos Privados

		private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamentoIntegracao filtrosPesquisa)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao>();

			var result = from obj in query select obj;

			if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCargaEmbarcador))
				result = result.Where(o => (o.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCargaEmbarcador || o.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento.Any(obj => obj.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCargaEmbarcador)));

			if (filtrosPesquisa.DataCargaInicial.HasValue)
				result = result.Where(o => (o.CargaJanelaCarregamento.Carga.DataCriacaoCarga >= filtrosPesquisa.DataCargaInicial || o.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento.Any(obj => obj.Carga.DataCriacaoCarga >= filtrosPesquisa.DataCargaInicial)));

			if (filtrosPesquisa.DataCargaFinal.HasValue)
				result = result.Where(o => (o.CargaJanelaCarregamento.Carga.DataCriacaoCarga <= filtrosPesquisa.DataCargaFinal || o.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento.Any(obj => obj.Carga.DataCriacaoCarga <= filtrosPesquisa.DataCargaFinal)));

			if (filtrosPesquisa.SituacaoIntegracao.HasValue)
				result = result.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);

			if (filtrosPesquisa.CodigoFilial > 0)
				result = result.Where(o => (o.CargaJanelaCarregamento.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial || o.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento.Any(obj => obj.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial)));

			if (filtrosPesquisa.CodigoCentroCarregamento > 0)
				result = result.Where(o => (o.CargaJanelaCarregamento.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento || o.CargaJanelaCarregamentoViagem.CargasJanelaCarregamento.Any(obj => obj.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento)));

			if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroViagem))
				result = result.Where(o => o.CargaJanelaCarregamentoViagem.NumeroViagem == filtrosPesquisa.NumeroViagem);

			return result;
		}

		#endregion
	}
}
