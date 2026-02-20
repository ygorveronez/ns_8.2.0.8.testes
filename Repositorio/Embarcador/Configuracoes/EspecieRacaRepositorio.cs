using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
	public class EspecieRacaRepositorio : RepositorioBase<EspecieRaca>
	{
		public EspecieRacaRepositorio(UnitOfWork workUnit) : base(workUnit) { }

		#region Métodos Públicos

		public bool VerificarExistenciaPorCodigo(int codigo)
		{
			var query = this.SessionNHiBernate.Query<EspecieRaca>();
			var result = from obj in query where obj.Codigo == codigo select obj;
			return result.Any();
		}

		public List<EspecieRaca> Consultar(int codigoEmpresa, int codigoEspecie, string descricaoEspecie, string descricaoRaca, SituacaoAtivoPesquisa ativo, string propOrdena, string dirOrdena, int inicio, int limite)
		{
			IQueryable<EspecieRaca> query = ObterConsulta(codigoEmpresa, codigoEspecie, descricaoEspecie, descricaoRaca, ativo);

			return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
		}

		public int ContarConsulta(int codigoEmpresa, int codigoEspecie, string descricaoEspecie, string descricaoRaca, SituacaoAtivoPesquisa ativo)
		{
			IQueryable<EspecieRaca> query = ObterConsulta(codigoEmpresa, codigoEspecie, descricaoEspecie, descricaoRaca, ativo);

			return query.Count();
		}

		#endregion

		#region Métodos Privados

		private IQueryable<EspecieRaca> ObterConsulta(int codigoEmpresa, int codigoEspecie, string descricaoEspecie, string descricaoRaca, SituacaoAtivoPesquisa ativo)
		{
			var query = this.SessionNHiBernate.Query<EspecieRaca>();

			if (codigoEmpresa > 0)
				query = query.Where(o => o.Especie.Empresa.Codigo == codigoEmpresa);

			if (codigoEspecie > 0)
				query = query.Where(o => o.Especie.Codigo == codigoEspecie);

			if (!string.IsNullOrWhiteSpace(descricaoEspecie))
				query = query.Where(o => o.Especie.Descricao.Contains(descricaoEspecie));

			if (!string.IsNullOrWhiteSpace(descricaoRaca))
				query.Where(o => o.Descricao.Contains(descricaoRaca));

			if (ativo != SituacaoAtivoPesquisa.Todos)
			{
				if (ativo == SituacaoAtivoPesquisa.Ativo)
					query = query.Where(o => o.Ativo);
				else
					query = query.Where(o => !o.Ativo);
			}

			return query;
		}

		#endregion
	}
}
