using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido
{
    public class LoteLiberacaoComercialPedidoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>
    {
        public LoteLiberacaoComercialPedidoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao> Consultar(int codigoLoteLiberacaoComercialPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIntegracoes = Consultar(codigoLoteLiberacaoComercialPedido, situacao);

            return ObterLista(consultaIntegracoes, parametrosConsulta);
        }

        public int ContarConsulta(int codigoLoteLiberacaoComercialPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoLoteLiberacaoComercialPedido, situacao);

            return consultaIntegracoes.Count();
        }

        public Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao BuscarPorLote(int lote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>();

            var result = from obj in query where obj.LoteLiberacaoComercialPedido.Codigo == lote select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarArquivosPorCodigo(int codigo)
        {
            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var resultCargaCTeIntegracaoArquivo = queryCargaCTeIntegracaoArquivo
                .Where(obj => obj.Codigo == codigo)
                .OrderByDescending(obj => obj.Data)
                .FirstOrDefault();

            return resultCargaCTeIntegracaoArquivo;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var queryLoteLiberacaoComercialPedidoIntegracao = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>()
                .Where(obj => obj.Codigo == codigo);

            var arquivosTransacao = queryLoteLiberacaoComercialPedidoIntegracao
                .SelectMany(p => p.ArquivosTransacao)
                .ToList();

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var resultCargaCTeIntegracaoArquivo = queryCargaCTeIntegracaoArquivo
                .Where(obj => arquivosTransacao.Contains(obj))
                .OrderByDescending(obj => obj.Data)
                .Skip(inicio)
                .Take(limite)
                .ToList();

            return resultCargaCTeIntegracaoArquivo;
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryLoteLiberacaoComercialPedidoIntegracao = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>()
                .Where(obj => obj.Codigo == codigo);

            var arquivosTransacao = queryLoteLiberacaoComercialPedidoIntegracao
                .SelectMany(p => p.ArquivosTransacao)
                .ToList();

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            var resultCargaCTeIntegracaoArquivoCount = queryCargaCTeIntegracaoArquivo
                .Count(obj => arquivosTransacao.Contains(obj));

            return resultCargaCTeIntegracaoArquivoCount;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao> BuscarPendentesIntegracaoPorTipo(int numeroTentativas, double minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public bool ExisteIntegracoesPendentesPorTipo(int numeroTentativas, double minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo
                         select obj;

            return result.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao> Consultar(int codigoLoteLiberacaoComercialPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaLoteLiberacaoComercialPedidoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao>()
                .Where(o => o.LoteLiberacaoComercialPedido.Codigo == codigoLoteLiberacaoComercialPedido);

            if (situacao.HasValue)
                consultaLoteLiberacaoComercialPedidoIntegracao = consultaLoteLiberacaoComercialPedidoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaLoteLiberacaoComercialPedidoIntegracao;
        }

        #endregion
    }
}
