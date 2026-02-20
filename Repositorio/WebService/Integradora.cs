using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.WebService
{
    public class Integradora : RepositorioBase<Dominio.Entidades.WebService.Integradora>
    {
        #region Construtores

        public Integradora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Integradora(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.WebService.Integradora BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.WebService.Integradora> BuscarPorIndicarIntegracao()
        {
            var consultaIntegradora = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>()
                .Where(o => (o.TipoIndicadorIntegracao == TipoIndicadorIntegracao.Sistema || o.TipoIndicadorIntegracao == TipoIndicadorIntegracao.WebService) && o.Ativo);

            return consultaIntegradora.OrderBy(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.WebService.Integradora BuscarPorToken(string token)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>();
            var result = from obj in query where obj.Ativo && ((obj.Token == token && obj.TipoAutenticacao == TipoAutenticacao.Token) || (obj.TokenTemporario == token && obj.DataExpiracao >= DateTime.Now && obj.TipoAutenticacao == TipoAutenticacao.UsuarioESenha)) select obj;
            return result
                .Fetch(obj => obj.Empresa)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.WebService.Integradora> BuscarPorTokenAsync(string token)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>();
            var result = from obj in query where obj.Ativo && ((obj.Token == token && obj.TipoAutenticacao == TipoAutenticacao.Token) || (obj.TokenTemporario == token && obj.DataExpiracao >= DateTime.Now && obj.TipoAutenticacao == TipoAutenticacao.UsuarioESenha)) select obj;
            return result
                .Fetch(obj => obj.Empresa)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.WebService.Integradora BuscarPorTipoIntegracao(TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>();
            var result = from obj in query where obj.Ativo && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.WebService.Integradora>> ConsultarAsync(Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = QueryConsulta(filtrosPesquisa);

            return ObterListaAsync(query, parametroConsulta);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora filtrosPesquisa)
        {
            var query = QueryConsulta(filtrosPesquisa);

            return query.CountAsync(CancellationToken);
        }

        public Dominio.Entidades.WebService.Integradora BuscarPorTokenIntegracao(string token)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>();
            var result = from obj in query where obj.Ativo && obj.Token != string.Empty && token.Contains(obj.Token) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.WebService.Integradora BuscarPorUsuarioESenha(string usuario, string senha)
        {
            var consultaIntegradora = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>().
                Where(obj => obj.TipoAutenticacao == TipoAutenticacao.UsuarioESenha && obj.Usuario == usuario && obj.Senha == senha && obj.Ativo);

            return consultaIntegradora.FirstOrDefault();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.WebService.Integradora> QueryConsulta(Dominio.ObjetosDeValor.WebService.Integradora.FiltroPesquisaIntegradora filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.Integradora>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo != SituacaoAtivoPesquisa.Todos)
                query = query.Where(obj => obj.Ativo == (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Ativo));

            return query;
        }

        #endregion Métodos Privados
    }
}
