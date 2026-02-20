using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoColeta : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta>
    {
        public PeriodoColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PeriodoColeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta> BuscarPorJanelaColeta(int codigoJanela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.JanelaColeta.Codigo == codigoJanela);

            return result.ToList();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoColeta> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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
