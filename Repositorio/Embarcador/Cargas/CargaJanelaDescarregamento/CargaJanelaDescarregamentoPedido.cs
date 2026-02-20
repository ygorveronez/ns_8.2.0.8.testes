using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaDescarregamentoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido>
    {
        #region Construtores

        public CargaJanelaDescarregamentoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public bool VerificarExistenciaPorPedidoCargaJanelaDescarregamento(int codigoPedido, int codigoJanela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido>()
                .Where(obj => obj.Pedido.Codigo == codigoPedido && obj.CargaJanelaDescarregamento.Codigo == codigoJanela);

            return query.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido BuscarPorPedidoCargaJanelaDescarregamento(int codigoPedido, int codigoJanela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido>()
                   .Where(obj => obj.Pedido.Codigo == codigoPedido && obj.CargaJanelaDescarregamento.Codigo == codigoJanela);

            return query.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido>()
                   .Where(obj => obj.CargaJanelaDescarregamento.Carga.Codigo == codigoCarga);

            return query.ToList();
        }
        
        public List<int> BuscarCodigosPedidosPorCargaJanelaDescarregamento(int codigoJanela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido>()
                   .Where(obj => obj.CargaJanelaDescarregamento.Codigo == codigoJanela);

            return query
                .Select(obj => obj.Pedido.Codigo)
                .ToList();
        }

        #endregion
    }
}
