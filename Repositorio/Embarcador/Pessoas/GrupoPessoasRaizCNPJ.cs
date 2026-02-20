using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasRaizCNPJ : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>
    {
        public GrupoPessoasRaizCNPJ(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<(int CodigoGrupoPessoas, string RaizCnpj)> BuscarDadosPorGruposPessoas(List<int> codigos)
        {
            var consultaGrupoPessoasRaizCNPJ = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>()
                .Where(o => codigos.Contains(o.GrupoPessoas.Codigo));

            return consultaGrupoPessoasRaizCNPJ
                .Select(o => ValueTuple.Create(o.GrupoPessoas.Codigo, o.RaizCNPJ))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ> BuscarPorGrupoPessoas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ> BuscarPorGrupoPessoas(int codigo, List<string> raizesCNPJ)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigo && !raizesCNPJ.Contains(obj.RaizCNPJ) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
    }
}