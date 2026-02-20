using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoStage : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>
    {
        public PedidoStage(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoStage BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Pedido.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorPedidoECargaStage(int codigo, int cargaDt)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Pedido.Codigo == codigo && obj.Stage.CargaDT.Codigo == cargaDt select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorPedidoSECargaStage(List<int> codigos, int cargaDt)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigos.Contains(obj.Pedido.Codigo) && obj.Stage.CargaDT.Codigo == cargaDt select obj;

            return result.Fetch(x => x.Pedido).Fetch(x => x.Stage).ThenFetch(x => x.StageAgrupamento).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarStagesPorPedidoECargaStage(List<int> codigos, int cargaDt)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigos.Contains(obj.Pedido.Codigo) && obj.Stage.CargaDT.Codigo == cargaDt select obj;

            return result.Select(x => x.Stage).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorStage(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Stage.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorStageFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query
                         where obj.Stage.Codigo == codigo
                         select obj;

            return result.Fetch(x => x.Pedido).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoStage BuscarPorStageEPedido(int codigoStage, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Stage.Codigo == codigoStage && obj.Pedido.Codigo == codigoPedido select obj;

            return result
                .Fetch(x => x.Stage).ThenFetch(obj => obj.StageAgrupamento).FirstOrDefault();
        }

        public bool ExisteStagePorPedido(int codigoStage, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();
            var result = from obj in query where obj.Stage.Codigo == codigoStage && obj.Pedido.Codigo == codigoPedido select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorListaStages(List<int> codigoStages)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigoStages.Contains(obj.Stage.Codigo) select obj.Pedido;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPedidoStages(List<int> codigoStages)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigoStages.Contains(obj.Stage.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarPorListaPedidos(List<int> codigoPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigoPedidos.Contains(obj.Pedido.Codigo) select obj.Stage;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarPorListaPedidosECargaDt(List<int> codigoPedidos, int cargaDt)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigoPedidos.Contains(obj.Pedido.Codigo) && obj.Stage.CargaDT.Codigo == cargaDt select obj.Stage;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorListaCodigoPedidos(List<int> codigoPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigoPedidos.Contains(obj.Pedido.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosRelevanteParaCusto(List<int> codigoPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where codigoPedidos.Contains(obj.Pedido.Codigo) && obj.Stage.RelevanciaCusto select obj;

            return result.Select(x => x.Pedido).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarPorCargaDT(int codigoCargaDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Stage.CargaDT.Codigo == codigoCargaDT select obj;

            return result
                .Fetch(obj => obj.Stage)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarRelevantesCustoPorCargaDT(int codigoCargaDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Stage.CargaDT.Codigo == codigoCargaDT && obj.Stage.RelevanciaCusto select obj;

            return result
                .Fetch(obj => obj.Stage)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Stage BuscarRelevantesCustoPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            var result = from obj in query where obj.Stage.RelevanciaCusto && obj.Pedido.Codigo == pedido select obj;

            return result
                .Select(obj => obj.Stage).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Stage BuscarRelevantesCustoPorPedidoECargaPai(int pedido, int cargaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from obj in query where obj.Pedido.Codigo == pedido && obj.Carga.Codigo == cargaPai select obj;

            return result
                .Select(obj => obj.StageRelevanteCusto).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> BuscarComRelevanciaCustoPorCargaDT(int codigoCargaDT)
        {
            var consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => pedidoStage.Stage.CargaDT.Codigo == codigoCargaDT && pedidoStage.Stage.RelevanciaCusto == true);

            return consultaPedidoStage.ToList();
        }

        public bool ExisteStageEmOutroPedido(int codigoStage, int codigoPedido)
        {
            var consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => pedidoStage.Stage.Codigo == codigoStage && pedidoStage.Pedido.Codigo != codigoPedido);

            return consultaPedidoStage.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Stage BuscarStagePorCargaPedido(int codigoCargaPedido, bool considerarEtapaRetorno = true)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.Codigo == codigoCargaPedido);

            var consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => consultaCargaPedido.Any(cargaPedido => cargaPedido.Carga.Codigo == pedidoStage.Stage.CargaDT.Codigo && cargaPedido.Pedido.Codigo == pedidoStage.Pedido.Codigo));

            if (considerarEtapaRetorno == false)
                consultaPedidoStage = consultaPedidoStage.Where(x => x.Stage.TipoPercurso != Vazio.PercursoRegreso);

            return consultaPedidoStage
                .Select(pedidoStage => pedidoStage.Stage).OrderByDescending(stage => stage.RelevanciaCusto)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Stage> BuscarStagesPorCargaPedido(List<int> codigoCargaPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => codigoCargaPedido.Contains(cargaPedido.Codigo));

            var consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => consultaCargaPedido.Any(cargaPedido => cargaPedido.Carga.Codigo == pedidoStage.Stage.CargaDT.Codigo && cargaPedido.Pedido.Codigo == pedidoStage.Pedido.Codigo));

            return consultaPedidoStage
                .Select(pedidoStage => pedidoStage.Stage)
                .ToList();
        }

        public void ExecuteDeletarPorStagePedido(int codigoStage, int codPedido)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_STAGE WHERE STA_CODIGO = :codigoStage and PED_CODIGO = :codPedido").SetInt32("codigoStage", codigoStage).SetInt32("codPedido", codPedido).ExecuteUpdate();
        }

        public void ExecuteDeletarPorPedido(int codPedido)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_STAGE WHERE PED_CODIGO = :codPedido").SetInt32("codPedido", codPedido).ExecuteUpdate();
        }
    }
}
