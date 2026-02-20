using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class ComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>
    {
        private CancellationToken _cancellationToken;
        public ComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ComponenteFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._cancellationToken = cancellationToken; }


        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();
            query = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return query
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoCompomenteFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();
            var result = from obj in query where obj.TipoComponenteFrete == tipoCompomenteFrete && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete buscarPorCodigoEmbarcador(string codigoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();
            var result = from obj in query where obj.CodigoIntegracao == codigoEmbarcador && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public bool ExistePorDescricao(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            query = query.Where(o => o.Descricao == descricao && o.Ativo);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete BuscarPorDescricao(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            query = query.Where(o => o.Descricao == descricao && o.Ativo);

            return query.FirstOrDefault();
        }



        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            query = query.Where(obj => obj.CodigoIntegracao == codigoIntegracao && obj.Ativo);

            return query.FirstOrDefault();
        }



        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> BuscarTodosAtivos(int limite = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            query = query.Where(o => o.Ativo);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();

        }

        public async Task<List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>> BuscarTodosAtivosAsync(int limite = 0)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>().Where(o => o.Ativo).ToListAsync(_cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> BuscarComponentesDePara()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado>();

            return query
                .Select(obj => obj.ComponenteFrete)
                .Distinct()
                .ToList();

        }

        public List<string> BuscarDescricaoPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj.Descricao;

            return result.ToList();
        }
        public async Task<List<string>> BuscarDescricaoPorCodigoAsync(int[] codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>().
                Where(x => codigos.Contains(x.Codigo)).Select(x => x.Descricao)
                .ToListAsync(_cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoCompomenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            var result = from obj in query
                         where !obj.ComponentePertenceComposicaoFreteValor
                         select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(mvc => mvc.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (tipoCompomenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.TODOS)
                result = result.Where(obj => obj.TipoComponenteFrete == tipoCompomenteFrete);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoCompomenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            var result = from obj in query
                         where !obj.ComponentePertenceComposicaoFreteValor
                         select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(mvc => mvc.Descricao.Contains(descricao));

            if (tipoCompomenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.TODOS)
                result = result.Where(obj => obj.TipoComponenteFrete == tipoCompomenteFrete);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

    }
}
