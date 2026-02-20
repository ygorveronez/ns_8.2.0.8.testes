using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoRotas : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas>
    {
        public CargaPedidoRotas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> BuscarPorCargaPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas BuscarPorCodigoIdentificacao(int cargaPedido, string identificacaoRota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.IdenticacaoRota == identificacaoRota select obj;
            return result.FirstOrDefault();
        }

    }
}
