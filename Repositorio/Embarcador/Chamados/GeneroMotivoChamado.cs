using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public  class GeneroMotivoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado>
	{

		#region Construtores

		public GeneroMotivoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }
		
		#endregion

		#region Métodos Públicos

		public List<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGeneroMotivoChamado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
		{
			var result = Consultar(filtrosPesquisa);

			return ObterLista(result, parametroConsulta);
		}

		public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGeneroMotivoChamado filtrosPesquisa)
		{
			var result = Consultar(filtrosPesquisa);

			return result.Count();
		}

		public bool ExisteGeneroCadastrado()
		{
			return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado>().Any();
		}

		#endregion

		#region Métodos Privados

		private IQueryable<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGeneroMotivoChamado filtrosPesquisa)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado>();

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
