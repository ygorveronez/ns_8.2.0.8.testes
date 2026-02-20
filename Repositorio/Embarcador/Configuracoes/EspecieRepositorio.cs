using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
	public class EspecieRepositorio : RepositorioBase<Especie>
	{
        public EspecieRepositorio(UnitOfWork workUnit) : base(workUnit) { }

		#region Métodos Públicos

		public Especie BuscarRacasPorEspecieCodigo(int codigo)
		{
			var query = this.SessionNHiBernate.Query<Especie>();
			var result = from obj in query where obj.Codigo.Equals(codigo) select obj;
			return result
				.Fetch(obj => obj.Racas)
				.FirstOrDefault();
		}

		public ICollection<EspecieRaca> BuscarRacaPorEspecie(string descricaoEspecie)
		{
			var query = this.SessionNHiBernate.Query<Especie>();
			var result = from obj in query where obj.Descricao.Contains(descricaoEspecie) select obj.Racas;
			return result.FirstOrDefault();
		}

		public bool VerificarExistenciaPorCodigo(int codigo)
		{
			var query = this.SessionNHiBernate.Query<Especie>();
			var result = from obj in query where obj.Codigo == codigo select obj;
			return result.Any();
		}

		public List<Especie> Consultar(int codigoEmpresa, string descricaoEspecie, string descricaoRaca, int codigoRaca, SituacaoAtivoPesquisa ativo, string propOrdena, string dirOrdena, int inicio, int limite)
		{
			IQueryable<Especie> query = ObterConsulta(codigoEmpresa, descricaoEspecie, descricaoRaca, codigoRaca, ativo);

			return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
		}

		public int ContarConsulta(int codigoEmpresa, string descricaoEspecie, string descricaoRaca, int codigoRaca, SituacaoAtivoPesquisa ativo)
		{
			IQueryable<Especie> query = ObterConsulta(codigoEmpresa, descricaoEspecie, descricaoRaca, codigoRaca, ativo);

			return query.Count();
		}

		#endregion

		#region Métodos Privados

		private IQueryable<Especie> ObterConsulta(int codigoEmpresa, string descricaoEspecie, string descricaoRaca, int codigoRaca, SituacaoAtivoPesquisa ativo)
		{
			var query = this.SessionNHiBernate.Query<Especie>();

			if (codigoEmpresa > 0)
				query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

			if (!string.IsNullOrWhiteSpace(descricaoEspecie))
				query = query.Where(o => o.Descricao.Contains(descricaoEspecie));

			if (codigoRaca > 0)
				query = query.Where(o => o.Racas.Any(raca => raca.Codigo == codigoRaca));

			//if (!string.IsNullOrWhiteSpace(descricaoRaca))
			//	query.Select(s => s.Racas.Where(w => w.Descricao.Contains(descricaoRaca)));

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
