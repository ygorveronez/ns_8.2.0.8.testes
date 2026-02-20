using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class AreaEnvolvidaMotivoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado>
	{

		#region Construtores

		public AreaEnvolvidaMotivoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

		#endregion

		#region Métodos Públicos

		public List<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAreaEnvolvidaMotivoChamado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
		{
			var result = Consultar(filtrosPesquisa);

			return ObterLista(result, parametroConsulta);
		}

		public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAreaEnvolvidaMotivoChamado filtrosPesquisa)
		{
			var result = Consultar(filtrosPesquisa);

			return result.Count();
		}

		public bool ExisteAreaEnvolvidaCadastrada()
		{
			return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado>().Any();
		}

		#endregion

		#region Métodos Privados

		private IQueryable<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAreaEnvolvidaMotivoChamado filtrosPesquisa)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado>();

			if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
				query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

			if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
				query = query.Where(o => o.Status);
			else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
				query = query.Where(o => !o.Status);

			return query;
		}

		#endregion
	}
}
