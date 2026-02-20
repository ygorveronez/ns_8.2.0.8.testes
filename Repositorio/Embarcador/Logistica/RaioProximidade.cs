using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class RaioProximidade : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RaioProximidade>
    {
        #region Métodos públicos
        public RaioProximidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.RaioProximidade BuscarPorCodigo(int codigo)
        {
            var raioProximidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RaioProximidade>();


            raioProximidade = raioProximidade.Where(ent => ent.Codigo == codigo);

            return raioProximidade.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> BuscarPorLocal(int codigoLocal)
        {
            var buscaRaioProximidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RaioProximidade>();


            buscaRaioProximidade = buscaRaioProximidade.Where(raioProximidade => raioProximidade.Local.Codigo == codigoLocal);

            return buscaRaioProximidade.OrderBy(r => r.Raio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> BuscarPorCodigosLocais(List<int> codigos)
        {
            var buscaRaiosProximidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RaioProximidade>();

            buscaRaiosProximidade = buscaRaiosProximidade.Where(raioProximidade => codigos.Contains(raioProximidade.Local.Codigo));

            return buscaRaiosProximidade.OrderBy(x => x.Raio).ToList();

        }


        private IQueryable<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocaisRaioProximidade filtroPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RaioProximidade>();

            if (!string.IsNullOrEmpty(filtroPesquisa.Descricao))
                consulta = consulta.Where(o => o.Identificacao.Contains(filtroPesquisa.Descricao) || o.Local.Descricao.Contains(filtroPesquisa.Descricao));

            return consulta.OrderByDescending(x => x.Local.Codigo).ThenBy(x => x.Raio);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocaisRaioProximidade filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtroPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaLocaisRaioProximidade filtroPesquisa)
        {
            var consulta = Consultar(filtroPesquisa);

            return consulta.Count();
        }
        #endregion

    }

}
