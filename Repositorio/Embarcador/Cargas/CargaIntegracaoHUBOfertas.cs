using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoHUBOfertas : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>
    {
        #region Construtores

        public CargaIntegracaoHUBOfertas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaIntegracaoHUBOfertas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> BuscarPorVinculoDemanda(string idVinculoDemanda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>();

            var result = from obj in query where obj.IdVinculoDemanda == idVinculoDemanda select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarArquivoHistoricoPorCodigo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var result = from obj in query where obj.Codigo == codigoArquivo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> ConsultarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.SituacaoIntegracao == situacaoIntegracao);

            return query.ToList();
        }

        public int ContarConsultaPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.SituacaoIntegracao == situacaoIntegracao);

            return query.Count();
        }

        public bool ExistePorTipoIntegracao(int codigoCarga, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.Any();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> ConsultarIntegracaoTransportador(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.Empresa.Codigo == codigoEmpresa && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB);

            return await query.FirstOrDefaultAsync(CancellationToken);
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> ConsultarIntegracaoCargaEnviadaHUB(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB
                && o.TipoEnvioHUBOfertas == TipoEnvioHUBOfertas.EnvioDemandaOferta);

            return await query.FirstOrDefaultAsync(CancellationToken);
        }

        public bool ConsultaCargaFoiEnviadoAoHUB(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB
                        && o.TipoEnvioHUBOfertas != TipoEnvioHUBOfertas.FinalizacaoDemandaOferta && o.TipoEnvioHUBOfertas != TipoEnvioHUBOfertas.CancelamentoDemandaOferta);

            return query.Any();
        }

        public bool ConsultarIntegracaoCargaComFalha(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao);

            return query.Any();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>> BuscarIntegracoesEmpresaHubPendente(int minutosLimite = 5, int numeroTentativasLimite = 3)
        {
            DateTime dataLimiteProximaTentativa = DateTime.Now.AddMinutes(-minutosLimite);

            var consultaEmpresaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o => o.Empresa.Status == "A" &&
                    (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                    (
                        o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= dataLimiteProximaTentativa
                    ))
                    && o.TipoEnvioHUBOfertas == TipoEnvioHUBOfertas.EnvioTransportador
                );

            return await consultaEmpresaIntegracao.OrderBy(o => o.Codigo)
                .Fetch(x => x.Empresa)
                .ThenFetch(x => x.Localidade)
                .ThenFetch(x => x.Estado)
                .ThenFetch(x => x.Pais)
                .ToListAsync(CancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>> BuscarIntegracoesHubPendente(int minutosLimite = 5, int numeroTentativasLimite = 5)
        {
            DateTime dataLimiteProximaTentativa = DateTime.Now.AddMinutes(-minutosLimite);

            var consultaEmpresaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>()
                .Where(o =>
                    (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) && o.NumeroTentativas < numeroTentativasLimite
                );

            return await consultaEmpresaIntegracao.OrderBy(o => o.Codigo)
                .Fetch(x => x.Carga)
                .ToListAsync(CancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoHUBOfertas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            consulta = consulta
                .Fetch(o => o.TipoIntegracao);

            return await ObterListaAsync<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>(consulta, parametroConsulta);
        }

        public Task<int> ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoHUBOfertas filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.CountAsync(CancellationToken);
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> BuscarPorProtocolo(string protocolo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>();

            query = query.Where(o => o.Protocolo == protocolo);

            return await query.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoHUBOfertas filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao.Date <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.TipoEnvioHUBOfertas.HasValue)
                query = query.Where(o => o.TipoEnvioHUBOfertas == filtrosPesquisa.TipoEnvioHUBOfertas.Value);

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.Carga > 0)
                query = query.Where(o => o.Carga.Codigo == filtrosPesquisa.Carga);

            return query;
        }
        #endregion
    }
}
