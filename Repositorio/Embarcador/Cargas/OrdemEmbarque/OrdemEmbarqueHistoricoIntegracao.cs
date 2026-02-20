using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.OrdemEmbarque
{
    public class OrdemEmbarqueHistoricoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>
    {
        #region Construtores

        public OrdemEmbarqueHistoricoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OrdemEmbarqueHistoricoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        public Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaOrdemEmbarqueHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaOrdemEmbarqueHistoricoIntegracao.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao> BuscarPorCodigoArquivoAsync(int codigoArquivo)
        {
            var consultaOrdemEmbarqueHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return await consultaOrdemEmbarqueHistoricoIntegracao.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var consultaOrdemEmbarqueHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>()
                .Where(o => o.OrdemEmbarque.Carga.Codigo == codigoCarga || o.OrdemEmbarque.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return consultaOrdemEmbarqueHistoricoIntegracao.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>> BuscarPorCargaAsync(int codigoCarga)
        {
            var consultaOrdemEmbarqueHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>()
                .Where(o => o.OrdemEmbarque.Carga.Codigo == codigoCarga || o.OrdemEmbarque.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return await consultaOrdemEmbarqueHistoricoIntegracao.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao BuscarUltimoPorTrocaPedido(int codigoCarga, int codigoPedido, string numeroOrdemEmbarque)
        {
            var consultaOrdemEmbarqueHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>()
                .Where(o =>
                    (o.OrdemEmbarque.Carga.Codigo == codigoCarga || o.OrdemEmbarque.Carga.CargaAgrupamento.Codigo == codigoCarga) &&
                    o.OrdemEmbarque.Numero == numeroOrdemEmbarque &&
                    o.Tipo == TipoOrdemEmbarqueHistoricoIntegracao.TrocaPedido &&
                    (o.PedidoAdicionado.Codigo == codigoPedido || o.PedidoRemovido.Codigo == codigoPedido)
                );

            return consultaOrdemEmbarqueHistoricoIntegracao
                .OrderByDescending(o => o.DataIntegracao)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao BuscarUltimoPorTrocaPedido(int codigoCarga, int codigoPedido, NumeroReboque numeroReboque)
        {
            var consultaOrdemEmbarqueHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>()
                .Where(o =>
                    (o.OrdemEmbarque.Carga.Codigo == codigoCarga || o.OrdemEmbarque.Carga.CargaAgrupamento.Codigo == codigoCarga) &&
                    o.OrdemEmbarque.NumeroReboque == numeroReboque &&
                    o.Tipo == TipoOrdemEmbarqueHistoricoIntegracao.TrocaPedido &&
                    (o.PedidoAdicionado.Codigo == codigoPedido || o.PedidoRemovido.Codigo == codigoPedido)
                );

            return consultaOrdemEmbarqueHistoricoIntegracao
                .OrderByDescending(o => o.DataIntegracao)
                .FirstOrDefault();
        }

        #endregion
    }
}
