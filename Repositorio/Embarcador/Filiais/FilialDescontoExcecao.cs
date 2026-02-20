using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public class FilialDescontoExcecao : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao>
    {
        public FilialDescontoExcecao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consulta = Consultar(descricao);

            return consulta.Count();
        }


    }

}

