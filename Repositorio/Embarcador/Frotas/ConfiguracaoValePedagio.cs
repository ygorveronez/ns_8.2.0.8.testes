using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frotas
{
    public class ConfiguracaoValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>
    {
        #region Construtores
        public ConfiguracaoValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoValePedagio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> BuscarPorSituacaoAtiva()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();

            var result = query.Where(o => o.Situacao).DistinctBy(o => o.TipoIntegracao);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio BuscarPrimeiraConfiguracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();

            var result = from obj in query
                         where
                             obj.TipoIntegracao == tipoIntegracao
                             && obj.Situacao == true
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> BuscarPorTipoIntegracaoTipoOperacaoEFilial(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int codigoTipoOperacao, int codigoFilial, string cnpjTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>()
                .Where(obj =>
                    obj.Situacao &&
                    obj.TipoIntegracao == tipoIntegracao &&
                    (obj.TipoOperacao.Codigo == codigoTipoOperacao || obj.TipoOperacao == null));

            if (!string.IsNullOrWhiteSpace(cnpjTransportador))
                query = query.Where(o => o.Filial.CNPJ == cnpjTransportador || o.Filial == null);
            else
                query = query.Where(o => o.Filial.Codigo == codigoFilial || o.Filial == null);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> BuscarPorTipoIntegracaoTipoOperacaoEGrupoPessoas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int codigoTipoOperacao, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();

            var result = from obj in query
                         where
                             obj.TipoIntegracao == tipoIntegracao
                             && obj.Situacao
                             && (obj.TipoOperacao.Codigo == codigoTipoOperacao || obj.TipoOperacao == null)
                             && (obj.GrupoPessoas.Codigo == codigoGrupoPessoas || obj.GrupoPessoas == null)
                         select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio BuscarConfiguracaoDuplicada(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();

            var result = from obj in query where obj.TipoIntegracao == configuracaoValePedagio.TipoIntegracao && obj.Situacao select obj;

            if (configuracaoValePedagio.Codigo > 0)
                result = result.Where(o => o.Codigo != configuracaoValePedagio.Codigo);

            if (configuracaoValePedagio.Filial != null)
                result = result.Where(o => o.Filial.Codigo == configuracaoValePedagio.Filial.Codigo);
            else
                result = result.Where(o => o.Filial == null);

            if (configuracaoValePedagio.GrupoPessoas != null)
                result = result.Where(o => o.GrupoPessoas.Codigo == configuracaoValePedagio.GrupoPessoas.Codigo);
            else
                result = result.Where(o => o.GrupoPessoas == null);

            if (configuracaoValePedagio.TipoOperacao != null)
                result = result.Where(o => o.TipoOperacao.Codigo == configuracaoValePedagio.TipoOperacao.Codigo);
            else
                result = result.Where(o => o.TipoOperacao == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaConfiguracaoValePedagio filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> result = Consultar(filtrosPesquisa);

            result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                .Fetch(o => o.TipoOperacao)
                .Fetch(o => o.Filial)
                .Fetch(o => o.GrupoPessoas)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaConfiguracaoValePedagio filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public bool PossuiIntegracaoRepomRest()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();

            var result = from obj in query where obj.Situacao && obj.IntegracaoRepom.TipoIntegracaoRepom == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoRepom.REsT select obj;

            return result.Any();
        }

        public async Task<bool> PossuiIntegracaoRepomRestAsync()
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>()
                .Where(x => x.Situacao && x.IntegracaoRepom.TipoIntegracaoRepom == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoRepom.REsT)
                .AnyAsync();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaConfiguracaoValePedagio filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoFilial > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                result = result.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Situacao == (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            if (filtrosPesquisa.TipoIntegracao.HasValue)
                result = result.Where(o => o.TipoIntegracao == filtrosPesquisa.TipoIntegracao);

            return result;
        }

        #endregion
    }
}
