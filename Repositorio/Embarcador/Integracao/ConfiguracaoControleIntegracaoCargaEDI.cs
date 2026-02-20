using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class ConfiguracaoControleIntegracaoCargaEDI : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI>
    {

        public ConfiguracaoControleIntegracaoCargaEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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
