using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>
    {
        public TipoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TipoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public bool ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> lstTiposIntegracao = null)
        {
            return ExistePorTipo(new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { tipo }, lstTiposIntegracao);
        }

        public async Task<bool> ExistePorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> lstTiposIntegracao = null)
        {
            return await ExistePorTipoAsync(new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { tipo }, lstTiposIntegracao);
        }

        public bool ExistePorTipo(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> lstTiposIntegracao = null)
        {
            if (lstTiposIntegracao != null)
            {
                if (tipos != null && tipos.Count > 0)
                    return lstTiposIntegracao.Where(o => o.Ativo && tipos.Contains(o.Tipo)).Any();
                return lstTiposIntegracao.Where(o => o.Ativo).Any();
            }
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            query = query.Where(o => o.Ativo);
            if (tipos != null && tipos.Count > 0)
                query = query.Where(o => tipos.Contains(o.Tipo));
            return query.Any();
        }

        public async Task<bool> ExistePorTipoAsync(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> lstTiposIntegracao = null)
        {
            if (lstTiposIntegracao != null)
            {
                if (tipos != null && tipos.Count > 0)
                    return lstTiposIntegracao.Where(o => o.Ativo && tipos.Contains(o.Tipo)).Any();
                return lstTiposIntegracao.Where(o => o.Ativo).Any();
            }
            var queryAsync = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>().ToListAsync();
            var query = queryAsync.AsQueryable();
            query = query.Where(o => o.Ativo);
            if (tipos != null && tipos.Count > 0)
                query = query.Where(o => tipos.Contains(o.Tipo));
            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarPorTipos(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos, bool? integracaoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            query = query.Where(o => o.Ativo);

            if (tipos != null && tipos.Count > 0)
                query = query.Where(o => tipos.Contains(o.Tipo));

            if (integracaoTransportador.HasValue)
                query = query.Where(o => o.IntegracaoTransportador == integracaoTransportador.Value);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>> BuscarPorTiposAsync(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos, bool? integracaoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            query = query.Where(o => o.Ativo);

            if (tipos != null && tipos.Count > 0)
                query = query.Where(o => tipos.Contains(o.Tipo));

            if (integracaoTransportador.HasValue)
                query = query.Where(o => o.IntegracaoTransportador == integracaoTransportador.Value);

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarPorTipos(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            query = query.Where(o => o.Ativo && tipos.Contains(o.Tipo));

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>> BuscarPorTiposAsync(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>()
                .Where(o => o.Ativo && tipos.Contains(o.Tipo));

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoQueGeraIntegracaoPedido()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            query = query.Where(o => o.Ativo);
            query = query.Where(o => o.IntegrarPedidos == true);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracao BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, bool integracaoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            query = query.Where(o => o.Ativo && o.Tipo == tipo && o.IntegracaoTransportador == integracaoTransportador);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracao BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            var result = from obj in query where obj.Ativo && obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>()
                .Where(obj => obj.Ativo && obj.Tipo == tipo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            var result = from obj in query where obj.Ativo select obj.Tipo;

            return result.Distinct().ToList();
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarTiposAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            var result = from obj in query where obj.Ativo select obj.Tipo;

            return result.Distinct().ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao? grupoTipoIntegracao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = Consultar(descricao, situacaoAtivo, grupoTipoIntegracao);
            return ObterLista(query, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao? grupoTipoIntegracao)
        {
            var query = Consultar(descricao, situacaoAtivo, grupoTipoIntegracao);
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarComIntegracaoVeiculoTrocaMotorista()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            query = query.Where(o => o.IntegrarVeiculoTrocaMotorista);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            var result = query.Where(o => o.Ativo);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>> BuscarTodosAtivosAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>()
                .Where(o => o.Ativo && o.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada && o.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao);

            return query.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracao ExisteTabelaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            var result = from obj in query where obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public void Desativar(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo)
        {
            tipo.Ativo = false;
            Atualizar(tipo);
        }

        public void Ativar(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo)
        {
            var existe = ExisteTabelaPorTipo(tipo.Tipo);

            if (existe != null)
            {
                existe.Tipo = tipo.Tipo;
                existe.Descricao = tipo.Descricao;
                existe.TipoEnvio = tipo.TipoEnvio;
                existe.QuantidadeMaximaEnvioLote = tipo.QuantidadeMaximaEnvioLote;
                existe.IntegrarCargaTransbordo = tipo.IntegrarCargaTransbordo;
                existe.GerarIntegracaoNasOcorrencias = tipo.GerarIntegracaoNasOcorrencias;
                existe.PermitirReenvioExcecao = tipo.PermitirReenvioExcecao;
                existe.ControlePorLote = tipo.ControlePorLote;
                existe.GerarIntegracaoFechamentoCarga = tipo.GerarIntegracaoFechamentoCarga;
                existe.IntegrarVeiculoTrocaMotorista = tipo.IntegrarVeiculoTrocaMotorista;
                existe.GerarIntegracaoDadosTransporteCarga = tipo.GerarIntegracaoDadosTransporteCarga;
                existe.IntegracaoTransportador = tipo.IntegracaoTransportador;
                existe.IntegrarComPlataformaNstech = tipo.IntegrarComPlataformaNstech;
                existe.IntegrarPedidos = tipo.IntegrarPedidos;
                existe.Grupo = tipo.Grupo;
                existe.Ativo = true;
                Atualizar(existe);
            }
            else
            {
                var novoTipoIntegracao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracao
                {
                    Tipo = tipo.Tipo,
                    Descricao = tipo.Descricao,
                    TipoEnvio = tipo.TipoEnvio,
                    QuantidadeMaximaEnvioLote = tipo.QuantidadeMaximaEnvioLote,
                    IntegrarCargaTransbordo = tipo.IntegrarCargaTransbordo,
                    GerarIntegracaoNasOcorrencias = tipo.GerarIntegracaoNasOcorrencias,
                    PermitirReenvioExcecao = tipo.PermitirReenvioExcecao,
                    ControlePorLote = tipo.ControlePorLote,
                    GerarIntegracaoFechamentoCarga = tipo.GerarIntegracaoFechamentoCarga,
                    IntegrarVeiculoTrocaMotorista = tipo.IntegrarVeiculoTrocaMotorista,
                    GerarIntegracaoDadosTransporteCarga = tipo.GerarIntegracaoDadosTransporteCarga,
                    IntegracaoTransportador = tipo.IntegracaoTransportador,
                    IntegrarComPlataformaNstech = tipo.IntegrarComPlataformaNstech,
                    IntegrarPedidos = tipo.IntegrarPedidos,
                    Grupo = tipo.Grupo,
                    Ativo = true
                };
                Inserir(novoTipoIntegracao);
            }
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao? grupoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;
                query = query.Where(o => o.Ativo == ativo);
            }

            if (grupoTipoIntegracao != null)
                query = query.Where(obj => obj.Grupo == grupoTipoIntegracao);

            var result = from obj in query select obj;

            return result;
        }

        #endregion
    }
}
