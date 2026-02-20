using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PracaPedagioTarifaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao>
    {
        #region Construtores

        public PracaPedagioTarifaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao BuscarProximaIntegracaoAgIntgracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao>()
                .Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                .OrderBy(o => o.DataIntegracao);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPracaPedagioTarifaIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);
            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPracaPedagioTarifaIntegracao filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPracaPedagioTarifaIntegracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao>();

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Where(o => o.DataIntegracao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                query = query.Where(o => o.DataIntegracao <= filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return query;
        }

        #endregion
    }
}
