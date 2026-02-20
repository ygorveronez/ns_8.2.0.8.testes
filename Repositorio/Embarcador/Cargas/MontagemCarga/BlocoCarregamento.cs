using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class BlocoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>
    {
        #region Construtores

        public BlocoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho> BuscarBlocoCarregamentoSegundoTrechoPorCarregamento(int codigoCarregamento)
        {
            System.Text.StringBuilder hql = new System.Text.StringBuilder();

            hql.Append("select CodigoCarregamentoSegundoTrecho, ");
            hql.Append("       CodigoPedido, ");
            hql.Append("       CpfCnpjExpedidor, ");
            hql.Append("       OrdemCarregamento, ");
            hql.Append("       cast(row_number() over(Partition by CodigoCarregamentoSegundoTrecho order by OrdemCarregamento desc) as int) as OrdemEntrega ");
            hql.Append("  from ( ");
            hql.Append("           select Carregamento.CRG_CODIGO as CodigoCarregamentoSegundoTrecho, ");
            hql.Append("                  Pedido.PED_CODIGO as CodigoPedido, ");
            hql.Append("                  Pedido.CLI_CODIGO_EXPEDIDOR as CpfCnpjExpedidor, ");
            hql.Append("                  cast(row_number() over(Partition by Carregamento.CRG_CODIGO order by RoterizacaoClienteRota.CTC_ORDEM desc, CarregamentoPedido.CRP_CODIGO) as int) as OrdemCarregamento ");
            hql.Append("             from T_CARREGAMENTO Carregamento ");
            hql.Append("             join T_CARREGAMENTO_ROTEIRIZACAO Roterizacao ");
            hql.Append("               on Carregamento.CRG_CODIGO = Roterizacao.CRG_CODIGO ");
            hql.Append("             join T_CARREGAMENTO_ROTEIRIZACAO_CLIENTES_ROTA RoterizacaoClienteRota ");
            hql.Append("               on Roterizacao.CRT_CODIGO = RoterizacaoClienteRota.CRT_CODIGO ");
            hql.Append("             join T_CARREGAMENTO_PEDIDO CarregamentoPedido ");
            hql.Append("               on Carregamento.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO ");
            hql.Append("             join T_PEDIDO Pedido ");
            hql.Append("               on CarregamentoPedido.PED_CODIGO = Pedido.PED_CODIGO ");
            hql.Append("              and RoterizacaoClienteRota.CLI_CGCCPF = Pedido.CLI_CODIGO ");
            hql.Append("            where Carregamento.CRG_CARREGAMENTO_REDESPACHO = 1 ");
            hql.Append($"             and Carregamento.CRG_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado} ");
            hql.Append("              and RoterizacaoClienteRota.CTC_ORDEM > 1 ");
            hql.Append("              and exists ( ");
            hql.Append("                      select top(1) 1 ");
            hql.Append("                        from T_CARREGAMENTO_PEDIDO PedidoCarregamentoMae ");
            hql.Append($"                      where PedidoCarregamentoMae.CRG_CODIGO = {codigoCarregamento} ");
            hql.Append("                         and PedidoCarregamentoMae.PED_CODIGO = Pedido.PED_CODIGO ");
            hql.Append("                  ) ");
            hql.Append("       ) as PedidoSegundoTrecho ");
            hql.Append(" order by CodigoCarregamentoSegundoTrecho, OrdemCarregamento ");

            var consultaBlocoCarregamentoSegundoTrecho = this.SessionNHiBernate.CreateSQLQuery(hql.ToString());

            consultaBlocoCarregamentoSegundoTrecho.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho)));

            return consultaBlocoCarregamentoSegundoTrecho.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamentoSegundoTrecho>();
        }

        public int ContarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> BuscarPorCarregamento(int carregamento)
        {
            var blocos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            var query = from obj in blocos
                        where obj.Carregamento.Codigo == carregamento
                        orderby obj.OrdemCarregamento
                        select obj;

            return query
                .Fetch(obj => obj.BlocoCarregamentoSegundoTrecho)
                .ThenFetch(obj => obj.Carregamento)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.BlocoCarregamentoSegundoTrecho)
                .ThenFetch(obj => obj.Carregamento)
                .ThenFetch(obj => obj.Veiculo)
                .Fetch(obj => obj.Carregamento)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Origem)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destino)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Pedido).ToList();

        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento BuscarPorCarregamentoECodigo(int carregamento, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento && obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> BuscarPorCarregamentoEPedido(int carregamento, int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento && obj.Pedido.Codigo == pedido select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> BuscarPorCarregamentos(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            var result = from obj in query where carregamentos.Contains(obj.Carregamento.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento> BuscarPorCarregamentosEOrdenarPorDescarregamento(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();
            var result = from obj in query where carregamentos.Contains(obj.Carregamento.Codigo) select obj;
            result = result.Fetch(x => x.Pedido);

            var blocos = result.ToList();
            int protocoloPedido = -1;
            Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento blocosPedidoCarregamento = new Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento> lstBlocosPedidosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento>();

            foreach (var bloco in blocos.OrderBy(x => x.Carregamento.Codigo).ThenBy(x => x.OrdemCarregamento))
            {
                int protocoloBlocoPedido = bloco.Pedido?.Protocolo ?? -1;
                if (protocoloPedido != protocoloBlocoPedido)
                {
                    blocosPedidoCarregamento = new Dominio.ObjetosDeValor.Embarcador.Pedido.BlocosPedidosCarregamento()
                    {
                        ProtocoloPedido = bloco.Pedido.Protocolo,
                        Carregamento = bloco.Carregamento.Codigo,
                        BlocosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoCarregamento>()
                    };
                    lstBlocosPedidosCarregamento.Add(blocosPedidoCarregamento);
                }
                blocosPedidoCarregamento.BlocosCarregamento.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoCarregamento()
                {
                    Codigo = bloco.Codigo,
                    Bloco = bloco.Bloco
                });
                protocoloPedido = protocoloBlocoPedido;
            }
            return lstBlocosPedidosCarregamento;
        }

        public void RemoverPorCodigosCarregamento(List<int> codigosCarregamento)
        {
            UnitOfWork.Sessao.CreateQuery("delete from BlocoCarregamento bloco where bloco.Carregamento.Codigo in (:CodigosCarregamento)")
                .SetParameterList("CodigosCarregamento", codigosCarregamento)
                .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> BuscarPorPedidos(List<int> codigosPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();

            query.Select(o => codigosPedido.Contains(o.Pedido.Codigo));

            return query.ToList();
        }

        #endregion
    }
}
