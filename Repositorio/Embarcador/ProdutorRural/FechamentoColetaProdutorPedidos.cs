using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.ProdutorRural
{
    public class FechamentoColetaProdutorPedidos : RepositorioBase<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos>
    {
        public FechamentoColetaProdutorPedidos(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> Consultar(int codigoFechamentoColeta, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos>();
            var result = (from obj in query where obj.FechamentoColetaProdutor.Codigo == codigoFechamentoColeta select obj.PedidoColetaProdutor);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.PreCarga)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Origem)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoOrigem)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destino)
                .ToList();
        }

        public int ContarConsulta(int codigoFechamentoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos>();
            var result = (from obj in query where obj.FechamentoColetaProdutor.Codigo == codigoFechamentoColeta select obj.PedidoColetaProdutor);

            return result.Count();
        }
    }
}
