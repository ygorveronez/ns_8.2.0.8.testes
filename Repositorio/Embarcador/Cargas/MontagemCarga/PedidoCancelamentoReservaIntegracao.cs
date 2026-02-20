using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class PedidoCancelamentoReservaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao>
    {
        #region Construtores

        public PedidoCancelamentoReservaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao> BuscarCarregamentoIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros)
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }
               
        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao> _Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroPedidoEmbarcador, int codigoEmpresa, double emitente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao <= dataFim);

            if (emitente > 0)
                result = result.Where(o => o.Pedido.Recebedor.CPF_CNPJ == emitente);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Pedido.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrEmpty(numeroPedidoEmbarcador))
                result = result.Where(o => o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador);

            return result.Fetch(obj => obj.Pedido).ThenFetch(obj => obj.Recebedor).ThenFetch(obj => obj.Localidade).Fetch(obj => obj.TipoIntegracao);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao> Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, double emitente, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, emitente);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, double emitente)
        {
            var result = _Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, emitente);

            return result.Count();
        }

    }
}
