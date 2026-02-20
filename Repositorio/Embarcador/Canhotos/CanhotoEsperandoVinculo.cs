using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Canhotos
{
    public class CanhotoEsperandoVinculo : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>
    {
        #region Construtores

        public CanhotoEsperandoVinculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> BuscarAguardandoLeituraNumeroDocumento(int limiteRegistros)
        {
            var consultaCanhotoEsperandoVinculo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .Where(o => o.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoLeituraNumeroDocumento);

            return consultaCanhotoEsperandoVinculo
                .OrderBy(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> BuscarAguardandoVinculoPorCargaEntrega(int codigoCargaEntrega)
        {
            var consultaCanhotoEsperandoVinculo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .Where(o => o.CargaEntrega.Codigo == codigoCargaEntrega && o.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoVinculo);

            return consultaCanhotoEsperandoVinculo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo> BuscarAguardandoVinculoPorCargaEPedido(int codigoCarga, int codigoPedido)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(cargaEntregaPedido => cargaEntregaPedido.CargaPedido.Pedido.Codigo == codigoPedido);

            var consultaCanhotoEsperandoVinculo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .Where(canhoto =>
                    canhoto.CargaEntrega.Carga.Codigo == codigoCarga &&
                    canhoto.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoVinculo &&
                    consultaCargaEntregaPedido.Any(cargaEntregaPedido => cargaEntregaPedido.CargaEntrega.Codigo == canhoto.CargaEntrega.Codigo)
                );

            return consultaCanhotoEsperandoVinculo.ToList();
        }   
        
        public Task<List<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>> BuscarAguardandoVinculoPorCargaEPedidoAsync(int codigoCarga, int codigoPedido)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(cargaEntregaPedido => cargaEntregaPedido.CargaPedido.Pedido.Codigo == codigoPedido);

            var consultaCanhotoEsperandoVinculo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .Where(canhoto =>
                    canhoto.CargaEntrega.Carga.Codigo == codigoCarga &&
                    canhoto.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoVinculo &&
                    consultaCargaEntregaPedido.Any(cargaEntregaPedido => cargaEntregaPedido.CargaEntrega.Codigo == canhoto.CargaEntrega.Codigo)
                );

            return consultaCanhotoEsperandoVinculo.ToListAsync();
        }   
        
        public Task<List<string>> BuscarNumeroDocumentoAguardandoVinculoPorCargaEPedidoAsync(int codigoCarga, int codigoPedido)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(cargaEntregaPedido => cargaEntregaPedido.CargaPedido.Pedido.Codigo == codigoPedido);

            var consultaCanhotoEsperandoVinculo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .Where(canhoto =>
                    canhoto.CargaEntrega.Carga.Codigo == codigoCarga &&
                    canhoto.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoVinculo &&
                    consultaCargaEntregaPedido.Any(cargaEntregaPedido => cargaEntregaPedido.CargaEntrega.Codigo == canhoto.CargaEntrega.Codigo)
                );

            return consultaCanhotoEsperandoVinculo.Select(x=>x.NumeroDocumento).ToListAsync();
        }       

        public Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo BuscarPorCodigo(int codigo)
        {
            var consultaCanhotoEsperandoVinculo = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .Where(o => o.Codigo == codigo);

            return consultaCanhotoEsperandoVinculo.FirstOrDefault();
        }
        
        public Task<bool> ExistePendenteAsync()
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo>()
                .AnyAsync(x=>x.Situacao == SituacaoCanhotoEsperandoVinculo.AguardandoVinculo);
        }

        #endregion Métodos públicos
    }
}
