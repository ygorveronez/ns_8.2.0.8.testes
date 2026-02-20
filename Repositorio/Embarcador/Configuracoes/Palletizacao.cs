using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class Palletizacao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao>
    {
        public Palletizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.Palletizacao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaPalletizacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaPalletizacao filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaPalletizacao filtrosPesquisa)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                consulta = consulta.Where(o => o.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Ativo.HasValue)
                consulta = consulta.Where(o => o.Ativo == filtrosPesquisa.Ativo.Value);

            return consulta;
        }

        #endregion
    }
}
