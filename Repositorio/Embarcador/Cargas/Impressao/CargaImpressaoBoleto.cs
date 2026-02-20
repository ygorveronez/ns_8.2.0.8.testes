using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.Impressao
{
    public class CargaImpressaoBoleto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto>
    {
        public CargaImpressaoBoleto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> BuscarPorCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto>();

            var result = from obj in query
                         where
                         obj.CargaPedido.Carga.Codigo == carga
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var consultaCargaImpressaoBoleto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return consultaCargaImpressaoBoleto.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> Buscar(int numeroUnidade, string numeroCarga, List<int> codigosPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto>();

            var result = from obj in query select obj;

            if (codigosPedidos != null && codigosPedidos.Count > 0)
                result = result.Where(o => codigosPedidos.Contains(o.CargaPedido.Pedido.Codigo));
            else
            {
                result = result.Where(o => o.SituacaoImpressao == situacao);

                if (numeroUnidade > 0)
                    result = result.Where(o => o.CargaPedido.Carga.Filial.NumeroUnidadeImpressao == numeroUnidade);

                if (!string.IsNullOrWhiteSpace(numeroCarga) && numeroCarga != "0")
                    result = result.Where(o => o.CargaPedido.Carga.CodigoCargaEmbarcador == numeroCarga);
            }

            return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> BuscarPorPedidos(List<int> codigosPedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto>();

            var result = from obj in query where codigosPedidos.Contains(obj.CargaPedido.Pedido.Codigo) select obj;

            return result.ToList();
        }

    }
}
