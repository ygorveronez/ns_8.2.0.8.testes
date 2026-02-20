using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public class FilialDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialDesconto>
    {
        public FilialDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Filiais.FilialDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialDesconto>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.FilialDesconto> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialDesconto>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialDesconto> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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

