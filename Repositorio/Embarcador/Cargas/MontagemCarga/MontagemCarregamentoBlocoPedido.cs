using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class MontagemCarregamentoBlocoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido>
    {
        public MontagemCarregamentoBlocoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido> BuscarPorBloco(int codigoMontagemCarregamentoBloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido>();

            var result = from obj in query
                         where obj.Bloco.Codigo == codigoMontagemCarregamentoBloco
                         select obj;

            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(x => x.RotaFrete)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido> BuscarPorBlocos(List<int> codigosMontagemCarregamentoBloco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido>();

            var result = from obj in query
                         where codigosMontagemCarregamentoBloco.Contains(obj.Bloco.Codigo)
                         select obj;

            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(x => x.RotaFrete)
                .ToList();
        }

        public void InserirSQL(int codigoMontagemCarregamentoBloco, List<int> codigosPedidos)
        {
            string sqlQuery = @"
INSERT INTO T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO ( MCB_CODIGO, PED_CODIGO ) 
SELECT :MCB_CODIGO, PED_CODIGO
  FROM T_PEDIDO
 WHERE PED_CODIGO IN ( :codigos );";

            int take = 1000;
            int start = 0;

            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameter("MCB_CODIGO", codigoMontagemCarregamentoBloco);
                query.SetParameterList("codigos", tmp);
                query.ExecuteUpdate();
                start += take;
            }
        }
    }
}
