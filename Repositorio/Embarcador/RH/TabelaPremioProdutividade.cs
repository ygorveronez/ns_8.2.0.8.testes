using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RH
{
    public class TabelaPremioProdutividade : RepositorioBase<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>
    {
        public TabelaPremioProdutividade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade BuscarPorGrupoEVigencia(int codigoGrupo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();
            var result = from obj in query where obj.GrupoPessoas.Any(c => c.Codigo == codigoGrupo) && obj.DataInicio <= data && obj.DataFim >= data select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade BuscarSemGrupoEVigencia(DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();
            var result = from obj in query where (obj.GrupoPessoas.Count == 0) && obj.DataInicio <= data && obj.DataFim >= data select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade> Consultar(int grupoPessoas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();

            var result = from obj in query select obj;

            if (grupoPessoas > 0)
                result = result.Where(obj => obj.GrupoPessoas.Any(c => c.Codigo == grupoPessoas));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int grupoPessoas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();

            var result = from obj in query select obj;

            if (grupoPessoas > 0)
                result = result.Where(obj => obj.GrupoPessoas.Any(c => c.Codigo == grupoPessoas));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

        public List<int> BuscarItensNaoPesentesNaLista(int parametro, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();
            var result = from obj in query
                         where
                            obj.Codigo == parametro
                            && !obj.GrupoPessoas.Any(c => codigos.Contains(c.Codigo)) //!codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public bool ContemGrupoNaLista(int codigoTabela, int codigoGrupo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>();
            query = query.Where(c => c.Codigo == codigoTabela && c.GrupoPessoas.Any(a => a.Codigo == codigoGrupo));
            return query.Any();
        }
    }
}
