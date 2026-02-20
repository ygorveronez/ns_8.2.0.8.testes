using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoVtex : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex>
    {
        #region Construtores

        public ConfiguracaoVtex(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex BuscarPrimeiraConfiguracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex>();

            var result = from obj in query
                         where obj.Situacao == true
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex BuscarPorFilial(int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex>();

            var result = from obj in query
                         where obj.Situacao == true
                             && (obj.Filial.Codigo == filial || obj.Filial == null)
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex BuscarConfiguracaoDuplicada(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex configuracaoValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex>();

            var result = from obj in query where obj.Situacao == true select obj;

            if (configuracaoValePedagio.Codigo > 0)
                result = result.Where(o => o.Codigo != configuracaoValePedagio.Codigo);

            if (configuracaoValePedagio.Filial != null)
                result = result.Where(o => o.Filial.Codigo == configuracaoValePedagio.Filial.Codigo);
            else
                result = result.Where(o => o.Filial == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex> result = Consultar(filtrosPesquisa);

            result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex> BuscarAtivos()
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex>().Where(x=>x.Situacao == true).ToList();
        }


        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex> result = Consultar(filtrosPesquisa);
            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.Filial > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.Filial);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Situacao == (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            return result;

        }
        #endregion
    }
}
