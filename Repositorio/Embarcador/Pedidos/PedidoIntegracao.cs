using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>
    {
        public PedidoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao BuscarLayoutPedidoPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query where obj.SituacaoIntegracao == situacao select obj;
            return result.ToList();
        }


        public List<int> BuscarPendentesIntegracao(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query
                         where !obj.IntegracaoCancelamento && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.TipoIntegracao.Tentativas > 0 && obj.Tentativas < obj.TipoIntegracao.Tentativas && (obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Correios ? obj.DataEnvio <= DateTime.Now.AddMinutes(-15) : obj.DataEnvio <= DateTime.Now.AddDays(-1)))
                         select obj;
            return result.Skip(0).Take(numeroRegistrosPorVez).Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query
                         where !obj.IntegracaoCancelamento && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.TipoIntegracao.Tentativas > 0 && obj.Tentativas < obj.TipoIntegracao.Tentativas && (obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Correios ? obj.DataEnvio <= DateTime.Now.AddMinutes(-15) : obj.DataEnvio <= DateTime.Now.AddDays(-1)))
                         select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>> BuscarPendentesIntegracaoPorPedidoAsync(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query
                         where
                         codigosPedidos.Contains(obj.Pedido.Codigo) &&
                         (!obj.IntegracaoCancelamento && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.TipoIntegracao.Tentativas > 0 && obj.Tentativas < obj.TipoIntegracao.Tentativas && (obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Correios ? obj.DataEnvio <= DateTime.Now.AddMinutes(-15) : obj.DataEnvio <= DateTime.Now.AddDays(-1))))
                         select obj;
            return result.ToListAsync();
        }

        public List<int> BuscarIntegracoesPendentesRetorno(int numeroRegistrosPorVez, int tempoMinutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => !obj.IntegracaoCancelamento && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && obj.DataEnvio <= DateTime.Now.AddMinutes(-tempoMinutos));

            return query.Skip(0).Take(numeroRegistrosPorVez).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarIntegracoesEmCancelmantoAguardandoIntegracao(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query
                         where obj.IntegracaoCancelamento && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.TipoIntegracao.Tentativas > 0 && obj.Tentativas < obj.TipoIntegracao.Tentativas && (obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Correios ? obj.DataEnvio <= DateTime.Now.AddMinutes(-15) : obj.DataEnvio <= DateTime.Now.AddDays(-1)))
                         select obj;
            return result.Skip(0).Take(numeroRegistrosPorVez).Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo));

            return query.FirstOrDefault();
        }

        public int ContarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return query.Count();
        }

        public int TotalArquivos(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.Count();
        }

        public int TotalArquivosStatus(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.SituacaoIntegracao == status select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.Pedido.Codigo == codigoPedido);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarPorPedidos(List<int> codigosPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query select obj;
            result = result.Where(obj => codigosPedido.Contains(obj.Pedido.Codigo));
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarPorPedido(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query select obj;

            if ((int)situacao >= 0)
                result = result.Where(obj => obj.Pedido.Codigo == codigoPedido && obj.SituacaoIntegracao == situacao);
            else
                result = result.Where(obj => obj.Pedido.Codigo == codigoPedido);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorPedido(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();
            var result = from obj in query select obj;
            if ((int)situacao >= 0)
                result = result.Where(obj => obj.Pedido.Codigo == codigoPedido && obj.SituacaoIntegracao == situacao);
            else
                result = result.Where(obj => obj.Pedido.Codigo == codigoPedido);
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarIntegracoesPendentesEnvio(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.DataEnvio == null)
                                ||
                                (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao && obj.DataEnvio.Value <= DateTime.Now)
                                ||
                                (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                 obj.Tentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataEnvio <= DateTime.Now.AddMinutes(-minutosACadaTentativa)));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao BuscarIntegracoesPorCargaEnvioEmail(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => obj.Pedido.Codigo == codigoPedido);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarIntegracoesPendentesEnvioEmail(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                 obj.Tentativas < 4 && obj.DataEnvio <= DateTime.Now.AddMinutes(-minutosACadaTentativa))));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarIntegracoesPendentesRetorno(string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> BuscarIntegracoesPorPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>();

            query = query.Where(obj => codigosPedidos.Contains(obj.Pedido.Codigo));

            return query.ToList();
        }
    }
}
