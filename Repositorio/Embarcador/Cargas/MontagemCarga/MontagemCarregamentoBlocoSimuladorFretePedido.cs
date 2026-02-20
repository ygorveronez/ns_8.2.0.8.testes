using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class MontagemCarregamentoBlocoSimuladorFretePedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>
    {
        public MontagemCarregamentoBlocoSimuladorFretePedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MontagemCarregamentoBlocoSimuladorFretePedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> BuscarPorBlocoSimuladorFrete(int codigoBlocoSimuladorFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

            var result = from obj in query
                         where obj.SimuladorFrete.Codigo == codigoBlocoSimuladorFrete
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> BuscarPorBlocosSimuladoresFretes(List<int> codigosBlocosSimuladoresFretes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

            var result = from obj in query
                         where codigosBlocosSimuladoresFretes.Contains(obj.SimuladorFrete.Codigo)
                         select obj;

            return result.Fetch(x => x.Pedido)
                         .ThenFetch(x => x.Destino)
                         .ThenFetch(x => x.Estado)
                         .Fetch(x => x.Pedido)
                         .ThenFetch(x => x.Destinatario)
                         .Fetch(x => x.SimuladorFrete).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

            var result = from obj in query
                         where obj.Pedido.Codigo == codigoPedido
                         select obj;

            return result.Fetch(x => x.SimuladorFrete)
                         .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>> BuscarPorPedidosAsync(List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();
            int take = 1000;
            int start = 0;

            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();

                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(await filter.Fetch(x => x.SimuladorFrete)
                                      .ToListAsync(CancellationToken));
                start += take;
            }

            return result;
        }

        public void InserirSQL(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> itens)
        {
            if (itens != null && itens.Count > 0)
            {
                int take = 150;
                int start = 0;

                while (start < itens.Count)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> itensTmp = itens.Skip(start).Take(take).ToList();

                    string parameros = "( :MSP_PESO_TOTAL_[X], :MSP_QUANTIDADE_PALLET_[X], :MSP_METRO_CUBICO_[X], :MSP_VOLUMES_[X], :MSF_CODIGO_[X], :PED_CODIGO_[X] )";
                    string sql = @"
INSERT INTO T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO (MSP_PESO_TOTAL, MSP_QUANTIDADE_PALLET, MSP_METRO_CUBICO, MSP_VOLUMES, MSF_CODIGO, PED_CODIGO) VALUES " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < itensTmp.Count; i++)
                        sql += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                    for (int i = 0; i < itensTmp.Count; i++)
                    {
                        query.SetParameter("MSP_PESO_TOTAL_" + i.ToString(), itensTmp[i].Peso);
                        query.SetParameter("MSP_QUANTIDADE_PALLET_" + i.ToString(), itensTmp[i].QuantidadePallet);
                        query.SetParameter("MSP_METRO_CUBICO_" + i.ToString(), itensTmp[i].MetroCubico);
                        query.SetParameter("MSP_VOLUMES_" + i.ToString(), itensTmp[i].Volumes);
                        query.SetParameter("MSF_CODIGO_" + i.ToString(), itensTmp[i].SimuladorFrete.Codigo);
                        query.SetParameter("PED_CODIGO_" + i.ToString(), itensTmp[i].Pedido.Codigo);
                    }

                    query.ExecuteUpdate();
                    start += take;
                }
            }
        }
    }
}
