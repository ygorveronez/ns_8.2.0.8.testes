using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>
    {
        #region Construtores

        public ContratoFreteTransportadorIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> Consultar(int codigoIntegracao, SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>()
                .Where(o => o.LBC.Codigo == codigoIntegracao);

            if (situacao.HasValue)
                consultaIntegracao = consultaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao BuscarPorCodigo(int codigo)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return integracao;
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> Consultar(int codigo, SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigo, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigo, SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigo, situacao);

            return consultaIntegracoes.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao BuscarAguardandoRetornoPorTipoIntegracao(int codigoContratoFreteTransportador, int codigoTipoIntegracao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>()
                .Where(obj =>
                    obj.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador &&
                    obj.TipoIntegracao.Codigo == codigoTipoIntegracao &&
                    obj.SituacaoIntegracao == SituacaoIntegracao.AgRetorno
                );

            return consultaIntegracoes.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> BuscarIntegracoesComFalha()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>();

            var result = from obj in query where obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> BuscarIntegracoesPorContrato(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>();

            var result = from obj in query where obj.ContratoFreteTransportador.Codigo == codigoContrato select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, bool tipoAnexo = false, bool custoFixo = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && obj.TipoIntegracao.Ativo && obj.IntegrarAnexo == tipoAnexo && obj.IntegrarCustoFixo == custoFixo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> BuscarPorLBC(int codigoCargaMDFeManual, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>();

            var result = from obj in query where obj.LBC.Codigo == codigoCargaMDFeManual select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        #endregion
    }
}
