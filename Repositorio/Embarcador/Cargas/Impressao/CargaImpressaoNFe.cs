using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.Impressao
{
    public class CargaImpressaoNFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe>
    {
        public CargaImpressaoNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe> Buscar(int numeroUnidade, string numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe>();

            var result = from obj in query where obj.SituacaoImpressao == situacao select obj;

            if (numeroUnidade > 0)
                result = result.Where(o => o.CargaPedido.Carga.Filial.NumeroUnidadeImpressao  == numeroUnidade);

            if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                result = result.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador == numeroCarga);

            return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe BuscarPorCargaPedidoENumero(int cargaPedido, int numero)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe> BuscarPorPedidos(List<int> codigosPedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe>();

            var result = from obj in query where codigosPedidos.Contains(obj.CargaPedido.Pedido.Codigo) select obj;

            return result.ToList();
        }
    }
}
