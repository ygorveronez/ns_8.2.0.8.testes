using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoPedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>
    {
        public CarregamentoPedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoPedidoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.CarregamentoPedido.Carregamento.Codigo == carregamento &&
                               obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                         select obj;

            return result
                .Fetch(obj => obj.PedidoProduto)
                .ThenFetch(obj => obj.Produto)
                .Fetch(obj => obj.CarregamentoPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.CanalEntrega)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCarregamentos(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where carregamentos.Contains(obj.CarregamentoPedido.Carregamento.Codigo) &&
                               obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                         select obj;

            return result
                .Fetch(obj => obj.PedidoProduto)
                .ThenFetch(obj => obj.Produto)
                .Fetch(obj => obj.CarregamentoPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.CanalEntrega)
                .ToList();
        }

        /// <summary>
        /// Procedimento para consultar todos os pedidos/produtos com carregamento
        /// </summary>
        /// <param name="pedido">Código do pedido</param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.CarregamentoPedido.Pedido.Codigo == pedido &&
                               obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                         select obj;

            return result.ToList();
        }

        /// <summary>
        /// Procedimento para consultar todos os produtos de pedidos contidos em algum carregamento.
        /// </summary>
        /// <param name="pedidos">Lista de código de pedidos</param>
        /// <returns>Lista de carregamento pedido produto de carregamentos não cancelados.</returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorPedidos(List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                //Códigos dos pedidos take...
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                var filter = from obj in query
                             where tmp.Contains(obj.CarregamentoPedido.Pedido.Codigo) &&
                                   obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                             select obj;

                result.AddRange(filter.Fetch(x => x.PedidoProduto)
                                      .Fetch(x => x.CarregamentoPedido)
                                      .ToList());

                start += take;
            }

            return result;
        }

        /// <summary>
        /// Procedimento para consultar todos os produtos de pedidos contidos em algum carregamento.
        /// </summary>
        /// <param name="pedidos">Lista de código de pedidos</param>
        /// <returns>Lista de carregamento pedido produto de carregamentos não cancelados.</returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCarregamentoEPedidos(int codigoCarregamento, List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                //Códigos dos pedidos take...
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>().Where(obj => obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento &&
                                                                                                                                                     obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
                var filter = from obj in query
                             where tmp.Contains(obj.CarregamentoPedido.Pedido.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(x => x.PedidoProduto).ThenFetch(x => x.Produto)
                                      .Fetch(x => x.CarregamentoPedido)
                                      .ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorCarregamentoEPedidoProdutosAsync(int codigoCarregamento, List<int> codigosPedidosProdutos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedidosProdutos?.Count)
            {
                List<int> tmp = codigosPedidosProdutos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>().Where(obj => obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento &&
                                                                                                                                                     obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
                var filter = from obj in query
                             where tmp.Contains(obj.PedidoProduto.Codigo)
                             select obj;

                result.AddRange(await filter.Fetch(x => x.PedidoProduto).ThenFetch(x => x.Produto)
                                      .Fetch(x => x.CarregamentoPedido).ThenFetch(x => x.Pedido)
                                      .ToListAsync());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorPedidos(int codigoCarregamento, List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            int take = 1000;
            int start = 0;

            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                query = query.Where(o => tmp.Contains(o.CarregamentoPedido.Pedido.Codigo) && o.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);
                query = query.Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .ThenFetch(x => x.Destinatario)
                             .Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .ThenFetch(x => x.CanalEntrega);
                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorPedidosAsync(int codigoCarregamento, List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            int take = 1000;
            int start = 0;

            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                query = query.Where(o => tmp.Contains(o.CarregamentoPedido.Pedido.Codigo) && o.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);
                query = query.Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .ThenFetch(x => x.Destinatario)
                             .Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .ThenFetch(x => x.CanalEntrega);
                result.AddRange(await query.ToListAsync(CancellationToken));
                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorPedidosAsync(List<int> carregamentos, List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> lista = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                //Códigos dos pedidos take...
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
                var result = from obj in query
                             where tmp.Contains(obj.CarregamentoPedido.Pedido.Codigo) && carregamentos.Contains(obj.CarregamentoPedido.Carregamento.Codigo)
                             select obj;

                result = result.Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .ThenFetch(x => x.Destinatario)
                             .Fetch(x => x.PedidoProduto)
                             .ThenFetch(x => x.Pedido)
                             .ThenFetch(x => x.CanalEntrega)
                             .Fetch(x => x.CarregamentoPedido);

                lista.AddRange(await result.ToListAsync(CancellationToken));

                start += take;
            }

            return lista;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCarregamentoPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.CarregamentoPedido.Codigo == codigo
                         select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorCarregamentoEDestinatarioAsync(int codigoCarregamento, long codigoDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento
                         select obj;

            if (codigoDestinatario > 0)
                result = result.Where(obj => obj.CarregamentoPedido.Pedido.Destinatario.CPF_CNPJ == codigoDestinatario);

            return result.Fetch(x => x.CarregamentoPedido)
                         .ThenFetch(x => x.Pedido)
                         .ThenFetch(x => x.Destinatario)
                         .Fetch(x => x.CarregamentoPedido)
                         .ThenFetch(x => x.Pedido)
                         .ThenFetch(x => x.CanalEntrega)
                         .Fetch(x => x.PedidoProduto)
                         .ThenFetch(x => x.Produto)
                         .Fetch(x => x.PedidoProduto)
                         .ThenFetch(x => x.LinhaSeparacao)
                         .ToListAsync(CancellationToken);
        }

        /// <summary>
        /// Retorna todos os produtos com carregamento.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorPedidoProduto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.PedidoProduto.Codigo == codigo && obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                         select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorSessaoRoteirizadorAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            query = query.Where(obj => obj.CarregamentoPedido.Carregamento.SessaoRoteirizador.Codigo == codigo)
                         .Fetch(x => x.CarregamentoPedido)
                         .ThenFetch(x => x.Carregamento)
                         .Fetch(x => x.PedidoProduto)
                         .ThenFetch(x => x.Produto);
            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorSessaoRoteirizadorEPedidos(int codigoSessaoRoteirizador, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            query = query.Where(obj => obj.CarregamentoPedido.Carregamento.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador && codigosPedidos.Contains(obj.CarregamentoPedido.Pedido.Codigo))
                         .Fetch(x => x.CarregamentoPedido)
                         .ThenFetch(x => x.Carregamento)
                         .Fetch(x => x.PedidoProduto)
                         .ThenFetch(x => x.Produto);
            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorSessaoRoteirizadorEPedidosAsync(int codigoSessaoRoteirizador, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            query = query.Where(obj => obj.CarregamentoPedido.Carregamento.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador && codigosPedidos.Contains(obj.CarregamentoPedido.Pedido.Codigo))
                         .Fetch(x => x.CarregamentoPedido).ThenFetch(x => x.Carregamento)
                         .Fetch(x => x.PedidoProduto).ThenFetch(x => x.Produto);

            return query.ToListAsync(CancellationToken);
        }

        /// <summary>
        /// Procedimento para consultar todos os produtos de um pedido contidos em um carregamento
        /// </summary>
        /// <param name="codigoCarregamento">Código do carregamento</param>
        /// <param name="codigoPedido">Código do pedido</param>
        /// <returns>Lista de produtos de pedido contidos no carregamento</returns>
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCarregamentoPedidoProdutos(int codigoCarregamento, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento &&
                               obj.CarregamentoPedido.Pedido.Codigo == codigoPedido
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCarregamentoPedidoProdutos(int codigoCarregamento, List<int> codigosPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento &&
                               codigosPedido.Contains(obj.CarregamentoPedido.Pedido.Codigo)
                         select obj;

            return result.Fetch(x => x.CarregamentoPedido)
                         .ThenFetch(x => x.Carregamento)
                         .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorPedidoProdutos(List<int> codigoPedidoProdutos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> lista = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            int take = 1000;
            int start = 0;
            while (start < codigoPedidoProdutos?.Count)
            {
                List<int> tmp = codigoPedidoProdutos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

                var filter = from obj in query
                             where tmp.Contains(obj.PedidoProduto.Codigo) && obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                             select obj;

                lista.AddRange(filter.Fetch(x => x.PedidoProduto).ToList());

                start += take;
            }
            return lista;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>> BuscarPorPedidoProdutosAsync(List<int> codigoPedidoProdutos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> lista = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();
            int take = 1000;
            int start = 0;
            while (start < codigoPedidoProdutos?.Count)
            {
                List<int> tmp = codigoPedidoProdutos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

                var filter = from obj in query
                             where tmp.Contains(obj.PedidoProduto.Codigo) && obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                             select obj;

                lista.AddRange(await filter.Fetch(x => x.PedidoProduto).ToListAsync(CancellationToken));

                start += take;
            }
            return lista;
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto BuscaPorCarregamentoPedidoProduto(int codigoCarregamento, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where
                           obj.CarregamentoPedido.Codigo == codigoCarregamento &&
                           obj.PedidoProduto.Codigo == codigoPedido
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> BuscarPorCodigoDiferenteECarregamentoPedido(List<int> codigos, int carregamentoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query where obj.CarregamentoPedido.Codigo == carregamentoPedido && !codigos.Contains(obj.Codigo) select obj;

            return result
                .ToList();
        }

        public void InserirTodosProdutosPedido(int codigoPedido, int codigoCarregamentoPedido)
        {
            string sql = @"
INSERT INTO T_CARREGAMENTO_PEDIDO_PRODUTO ( CPP_QUANTIDADE 
										  , CRP_CODIGO
										  , PRP_CODIGO
										  , CPP_PESO
										  , CPP_QUANTIDADE_PALLET
										  , CPP_METRO_CUBICO )
SELECT PRP_QUANTIDADE
	 , :CRP_CODIGO
	 , t.PRP_CODIGO
	 , (PRP_QUANTIDADE * PRP_PESO_UNITARIO) + PRP_PESO_TOTAL_EMBALAGEM as PESO
	 , PRP_QUANTIDADE_PALET
     , PRP_METRO_CUBICO 
  FROM T_PEDIDO_PRODUTO T
 WHERE T.PED_CODIGO = :PED_CODIGO; ";

            // Limpando os produtos do pedido carregamento....
            var delete = this.SessionNHiBernate.CreateSQLQuery("DELETE FROM T_CARREGAMENTO_PEDIDO_PRODUTO WHERE CRP_CODIGO = :CRP_CODIGO");
            delete.SetParameter("CRP_CODIGO", codigoCarregamentoPedido);
            delete.ExecuteUpdate();
            // Relacionado os produtos
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("CRP_CODIGO", codigoCarregamentoPedido);
            query.SetParameter("PED_CODIGO", codigoPedido);
            query.ExecuteUpdate();
        }

        public void InserirSQL(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> itens)
        {
            if (itens != null && itens.Count > 0)
            {
                int take = 100;
                int start = 0;
                if (itens[0].CarregamentoPedido.Codigo > 0)
                {
                    while (start < itens.Count)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> itensTmp = itens.Skip(start).Take(take).ToList();

                        string parameros = "( :CPP_QUANTIDADE_[X], :CRP_CODIGO_[X], :PRP_CODIGO_[X], :CPP_PESO_[X], :CPP_QUANTIDADE_PALLET_[X], :CPP_METRO_CUBICO_[X], :CPP_QUANTIDADE_ORIG_[X], :CPP_QUANTIDADE_PALLET_ORIG_[X], :CPP_METRO_CUBICO_ORIG_[X] )";
                        string sql = @"
INSERT INTO T_CARREGAMENTO_PEDIDO_PRODUTO (CPP_QUANTIDADE, CRP_CODIGO, PRP_CODIGO, CPP_PESO, CPP_QUANTIDADE_PALLET, CPP_METRO_CUBICO, CPP_QUANTIDADE_ORIG, CPP_QUANTIDADE_PALLET_ORIG, CPP_METRO_CUBICO_ORIG) VALUES " + parameros.Replace("[X]", "0");

                        for (int i = 1; i < itensTmp.Count; i++)
                            sql += ", " + parameros.Replace("[X]", i.ToString());

                        var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                        for (int i = 0; i < itensTmp.Count; i++)
                        {
                            query.SetParameter("CPP_QUANTIDADE_" + i.ToString(), itensTmp[i].Quantidade);
                            query.SetParameter("CRP_CODIGO_" + i.ToString(), itensTmp[i].CarregamentoPedido.Codigo);
                            query.SetParameter("PRP_CODIGO_" + i.ToString(), itensTmp[i].PedidoProduto.Codigo);
                            query.SetParameter("CPP_PESO_" + i.ToString(), itensTmp[i].Peso);
                            query.SetParameter("CPP_QUANTIDADE_PALLET_" + i.ToString(), itensTmp[i].QuantidadePallet);
                            query.SetParameter("CPP_METRO_CUBICO_" + i.ToString(), itensTmp[i].MetroCubico);
                            query.SetParameter("CPP_QUANTIDADE_ORIG_" + i.ToString(), itensTmp[i].Quantidade);
                            query.SetParameter("CPP_QUANTIDADE_PALLET_ORIG_" + i.ToString(), itensTmp[i].QuantidadePallet);
                            query.SetParameter("CPP_METRO_CUBICO_ORIG_" + i.ToString(), itensTmp[i].MetroCubico);
                        }

                        query.ExecuteUpdate();
                        start += take;
                    }
                }
                else
                {
                    while (start < itens.Count)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> itensTmp = itens.Skip(start).Take(take).ToList();

                        string parameros = "( :CPP_QUANTIDADE_[X], (SELECT CRP_CODIGO FROM T_CARREGAMENTO_PEDIDO CPE WHERE CPE.CRG_CODIGO = :CRG_CODIGO_[X] AND CPE.PED_CODIGO = :PED_CODIGO_[X]), :PRP_CODIGO_[X], :CPP_PESO_[X], :CPP_QUANTIDADE_PALLET_[X], :CPP_METRO_CUBICO_[X], :CPP_QUANTIDADE_ORIG_[X], :CPP_QUANTIDADE_PALLET_ORIG_[X], :CPP_METRO_CUBICO_ORIG_[X] )";
                        string sql = @"
INSERT INTO T_CARREGAMENTO_PEDIDO_PRODUTO (CPP_QUANTIDADE, CRP_CODIGO, PRP_CODIGO, CPP_PESO, CPP_QUANTIDADE_PALLET, CPP_METRO_CUBICO, CPP_QUANTIDADE_ORIG, CPP_QUANTIDADE_PALLET_ORIG, CPP_METRO_CUBICO_ORIG) VALUES " + parameros.Replace("[X]", "0");

                        for (int i = 1; i < itensTmp.Count; i++)
                            sql += ", " + parameros.Replace("[X]", i.ToString());

                        var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                        for (int i = 0; i < itensTmp.Count; i++)
                        {
                            query.SetParameter("CPP_QUANTIDADE_" + i.ToString(), itensTmp[i].Quantidade);
                            query.SetParameter("CRG_CODIGO_" + i.ToString(), itensTmp[i].CarregamentoPedido.Carregamento.Codigo);
                            query.SetParameter("PED_CODIGO_" + i.ToString(), itensTmp[i].CarregamentoPedido.Pedido.Codigo);
                            query.SetParameter("PRP_CODIGO_" + i.ToString(), itensTmp[i].PedidoProduto.Codigo);
                            query.SetParameter("CPP_PESO_" + i.ToString(), itensTmp[i].Peso);
                            query.SetParameter("CPP_QUANTIDADE_PALLET_" + i.ToString(), itensTmp[i].QuantidadePallet);
                            query.SetParameter("CPP_METRO_CUBICO_" + i.ToString(), itensTmp[i].MetroCubico);
                            query.SetParameter("CPP_QUANTIDADE_ORIG_" + i.ToString(), itensTmp[i].Quantidade);
                            query.SetParameter("CPP_QUANTIDADE_PALLET_ORIG_" + i.ToString(), itensTmp[i].QuantidadePallet);
                            query.SetParameter("CPP_METRO_CUBICO_ORIG_" + i.ToString(), itensTmp[i].MetroCubico);
                        }

                        query.ExecuteUpdate();
                        start += take;
                    }
                }
            }
        }

        public decimal PesoTotalProdutosCarregado(int codigoCarregamentoPedido)
        {
            string sql = @"
SELECT SUM(CPP_PESO)
  FROM T_CARREGAMENTO_PEDIDO_PRODUTO
 WHERE CRP_CODIGO = :codigo ";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("codigo", codigoCarregamentoPedido);

            return query.UniqueResult<decimal>();
        }

        public void DeletarCarregamentoPedidoProdutoPorCodigosCarregamentoPedidoProdutoViaQuery(List<int> codigosCarregamentoPedidoProduto)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARREGAMENTO_PEDIDO_PRODUTO WHERE CPP_CODIGO in ({string.Join(", ", codigosCarregamentoPedidoProduto)})").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARREGAMENTO_PEDIDO_PRODUTO WHERE CPP_CODIGO in ({string.Join(", ", codigosCarregamentoPedidoProduto)})").ExecuteUpdate(); // SQL-INJECTION-SAFE

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecaoSql);
                }

                throw;
            }
        }

        public void DeletarCarregamentoPedidoProdutoPorCodigoPedidoProdutoViaQuery(int codigoPedidoProduto)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                { 
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARREGAMENTO_PEDIDO_PRODUTO WHERE PRP_CODIGO = {codigoPedidoProduto}").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARREGAMENTO_PEDIDO_PRODUTO WHERE PRP_CODIGO = {codigoPedidoProduto}").ExecuteUpdate(); // SQL-INJECTION-SAFE

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecaoSql);
                }

                throw;
            }
        }

        public List<int> CodigosCarregamentoPedidoProdutoPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>().Where(obj => obj.PedidoProduto.Pedido.Codigo == codigoPedido
            && obj.CarregamentoPedido.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);

            return query.Select(obj => obj.Codigo).ToList();
        }
    }
}
