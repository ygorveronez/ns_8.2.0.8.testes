using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos.ImportacaoTakePay
{
    public class ImportacaoTakePayLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna>
    {
        public ImportacaoTakePayLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> BuscarPorImportacaoPendentesGeracaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoTakePay.Codigo == codigoImportacaoPedido && o.Linha.Pedido == null);

            return query.Fetch(o => o.Linha).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }
    }
}
