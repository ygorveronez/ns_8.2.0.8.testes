using MongoDB.Driver;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Integracao
{
    public class ConfiguracaoFilialIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao>
    {
        #region Construtores
        public ConfiguracaoFilialIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoFilialIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos  

        public Task<List<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaConfiguracaoFilialIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao> result = Consultar(filtrosPesquisa);

            result = result.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.LimiteRegistros > parametrosConsulta.InicioRegistros)
                result = result.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            return result
                .Fetch(o => o.TiposIntegracao)
                .Fetch(o => o.Filial)
                .Fetch(o => o.TipoOperacao)
                .ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaConfiguracaoFilialIntegracao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao> result = Consultar(filtrosPesquisa);

            return result.CountAsync(CancellationToken);
        }


        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarPorFilialETipoOperacaoAsync(int codigoFilial, int codigoTipoOperacao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao>()
                .Where(x => x.TipoOperacao.Codigo == codigoTipoOperacao && x.Filial.Codigo == codigoFilial)
                .SelectMany(x => x.TiposIntegracao)
                .Select(ti => ti.Tipo)
                .ToListAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao> BuscarConfiguracaoDuplicada(Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao>();

            var result = from obj in query where obj.TipoOperacao == configuracaoIntegracao.TipoOperacao && obj.Ativo select obj;

            if (configuracaoIntegracao.Codigo > 0)
                result = result.Where(o => o.Codigo != configuracaoIntegracao.Codigo);

            if (configuracaoIntegracao.TipoOperacao != null)
                result = result.Where(o => o.TipoOperacao.Codigo == configuracaoIntegracao.TipoOperacao.Codigo);
            else
                result = result.Where(o => o.TipoOperacao == null);

            if (configuracaoIntegracao.Filial != null)
                result = result.Where(o => o.Filial.Codigo == configuracaoIntegracao.Filial.Codigo);
            else
                result = result.Where(o => o.Filial == null);

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaConfiguracaoFilialIntegracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoFilial > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Ativo == (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            return result;
        }

        #endregion
    }
}
