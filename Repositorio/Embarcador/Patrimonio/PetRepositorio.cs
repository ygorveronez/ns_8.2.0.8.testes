using Dominio.Entidades.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Enumerador;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
	public class PetRepositorio : RepositorioBase<Pet>
	{
		public PetRepositorio(UnitOfWork workUnit) : base(workUnit) { }

		public List<Pet> Consultar(FiltroPesquisaPet filtroPesquisaPet, string propOrdena, string dirOrdena, int inicio, int limite)
		{
			IQueryable<Pet> query = ObterConsulta(filtroPesquisaPet);

			return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
		}

		private IQueryable<Pet> ObterConsulta(FiltroPesquisaPet filtroPesquisaPet)
		{
			var query = this.SessionNHiBernate.Query<Pet>();

			query = query.Where(o => o.Empresa.Codigo == filtroPesquisaPet.EmpresaCodigo);

			if (filtroPesquisaPet.TutorCodigo > 0)
				query = query.Where(o => o.Tutor.CPF_CNPJ == filtroPesquisaPet.TutorCodigo);

			if (filtroPesquisaPet.EspecieCodigo > 0)
				query = query.Where(o => o.Especie.Codigo == filtroPesquisaPet.EspecieCodigo);

			if (filtroPesquisaPet.RacaCodigo > 0)
				query = query.Where(o => o.Raca.Codigo == filtroPesquisaPet.RacaCodigo);

			if (filtroPesquisaPet.CorCodigo > 0)
				query = query.Where(o => o.Cor.Codigo == filtroPesquisaPet.CorCodigo);

			if (!string.IsNullOrWhiteSpace(filtroPesquisaPet.Nome))
				query = query.Where(o => o.Nome.Contains(filtroPesquisaPet.Nome));

			if (filtroPesquisaPet.SituacaoAtivo != SituacaoAtivoPesquisa.Todos)
			{
				if (filtroPesquisaPet.SituacaoAtivo == SituacaoAtivoPesquisa.Ativo)
					query = query.Where(o => o.Ativo);
				else
					query = query.Where(o => !o.Ativo);
			}

			if (filtroPesquisaPet.Sexo != Sexo.NaoInformado)
				query = query.Where(o => o.Sexo == filtroPesquisaPet.Sexo);

			if (filtroPesquisaPet.Porte != Porte.Todos)
				query = query.Where(o => o.Porte == filtroPesquisaPet.Porte);

			if (filtroPesquisaPet.Pelagem != Pelagem.Todas)
				query = query.Where(o => o.Pelagem == filtroPesquisaPet.Pelagem);

			return query;
		}
	}
}
