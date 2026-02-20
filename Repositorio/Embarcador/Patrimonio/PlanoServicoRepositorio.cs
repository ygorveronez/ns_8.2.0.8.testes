using Dominio.Entidades.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
	public class PlanoServicoRepositorio : RepositorioBase<PlanoServico>
	{
        public PlanoServicoRepositorio(UnitOfWork workUnit) : base(workUnit) { }

		#region Métodos Públicos

		public PlanoServico BuscarPorDescricao(string descricao)
		{
			var query = this.SessionNHiBernate.Query<PlanoServico>();
			var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
			return result.FirstOrDefault();
		}

		public bool VerificarExistenciaPorCodigo(int codigo)
		{
			var query = this.SessionNHiBernate.Query<PlanoServico>();
			var result = from obj in query where obj.Codigo == codigo select obj;
			return result.Any();
		}

		public List<PlanoServico> Consultar(int codigoEmpresa, SituacaoAtivoPesquisa ativo, string propOrdena, string dirOrdena, int inicio, int limite)
		{
			IQueryable<PlanoServico> query = ObterConsulta(codigoEmpresa, string.Empty, ativo);

			return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
		}

		public List<PlanoServico> Consultar(int codigoEmpresa, string descricao, SituacaoAtivoPesquisa ativo, string propOrdena, string dirOrdena, int inicio, int limite)
		{
			IQueryable<PlanoServico> query = ObterConsulta(codigoEmpresa, descricao, ativo);

			return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
		}

		public int ContarConsulta(int codigoEmpresa, string descricao, SituacaoAtivoPesquisa ativo)
		{
			IQueryable<PlanoServico> query = ObterConsulta(codigoEmpresa, descricao, ativo);

			return query.Count();
		}

		#endregion

		#region Métodos Privados

		private IQueryable<PlanoServico> ObterConsulta(int codigoEmpresa, string descricao, SituacaoAtivoPesquisa ativo)
		{
			var query = this.SessionNHiBernate.Query<PlanoServico>();

			if (codigoEmpresa > 0)
				query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

			if (!string.IsNullOrWhiteSpace(descricao))
				query = query.Where(o => o.Descricao.Contains(descricao));		

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
