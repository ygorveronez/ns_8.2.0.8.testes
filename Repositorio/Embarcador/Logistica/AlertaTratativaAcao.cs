using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Logistica
{

    public class AlertaTratativaAcao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao>
    {
        public AlertaTratativaAcao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao>();

            var result = from obj in query select obj;

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> BuscarPorTipoDeAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, bool somenteAtivos = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao>();

            var result = from obj in query where obj.TipoAlerta == tipoAlerta && (!somenteAtivos || obj.Ativo) select obj;

            return result.ToList();

        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> Consultar(string descricao, SituacaoAtivoPesquisa status)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));

            if (status == SituacaoAtivoPesquisa.Ativo)
                consulta = consulta.Where(o => o.Ativo == true);
            else if (status == SituacaoAtivoPesquisa.Inativo)
                consulta = consulta.Where(o => o.Ativo == false);

            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> Consultar(string descricao, SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao, status);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa status)
        {
            var consulta = Consultar(descricao, status);

            return consulta.Count();
        }

    }

}
