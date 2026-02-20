using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos.NotaDeDebito
{
    public class NotaDeDebitoLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna>
    {
        public NotaDeDebitoLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna> BuscarPorNotaPendentesGeracaoPedido(int codigoNotaDebito)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna>();

            query = query.Where(o => o.Linha.NotaDeDebito.Codigo == codigoNotaDebito && o.Linha.Pedido == null);

            return query.Fetch(o => o.Linha).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }
    }
}
