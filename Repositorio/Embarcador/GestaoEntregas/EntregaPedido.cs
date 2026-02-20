using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class EntregaPedido : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido>
    {
        public EntregaPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido>();
            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido BuscarPorToken(string token)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido>();
            var result = from obj in query where obj.Token == token select obj;
            return result.FirstOrDefault();
        }
    }
}