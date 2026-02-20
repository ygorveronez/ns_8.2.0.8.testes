using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>
    {
        public ChamadoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ChamadoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao BuscarPorProtocolo(string protocolo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>();
            var result = query.Where(obj => obj.ProtocoloDevolucao == protocolo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao BuscarPrimeiroPorChamado(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao BuscarPrimeiroPorChamadoECargaEntregaNotaFiscal(int codigoChamado, int cargaEntregaNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado && obj.CargaEntregaNotaFiscal.Codigo == cargaEntregaNotaFiscal);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> BuscarPorChamado(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> Consultar(int codigoChamado, SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoChamado, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoChamado, SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoChamado, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> BuscarIntegracaoPendente(int limiteRegistros)
        {
            DateTime dataLimiteProximaTentativa = DateTime.Now.AddMinutes(-5d);
            int numeroTentativasLimite = 3;

            var consultaChamadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>()
                .Where(o =>
                (o.Chamado.Situacao == SituacaoChamado.AgIntegracao || o.Chamado.Situacao == SituacaoChamado.FalhaIntegracao || o.Chamado.Situacao == SituacaoChamado.Finalizado) &&
                o.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Individual &&
                    (o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                    (
                        o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= dataLimiteProximaTentativa
                    ))
                );

            return consultaChamadoIntegracao
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarArquivoHistoricoPorCodigo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var result = from obj in query where obj.Codigo == codigoArquivo select obj;

            return result.FirstOrDefault();
        }

        public Task<bool> PossuiIntegracaoAsync(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado);
            return result.AnyAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> BuscarPorCodigoChamadoSituacao(int codigoChamado, SituacaoIntegracao situacao)
        {
            var consultaChamadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>()
                .Where(o => o.Chamado.Codigo == codigoChamado)
                .Where(o => o.SituacaoIntegracao == situacao);

            return consultaChamadoIntegracao.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> Consultar(int codigoChamado, SituacaoIntegracao? situacao)
        {
            var consultaChamadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao>()
                .Where(o => o.Chamado.Codigo == codigoChamado);

            if (situacao.HasValue)
                consultaChamadoIntegracao = consultaChamadoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaChamadoIntegracao;
        }

        #endregion
    }
}
