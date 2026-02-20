using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>
    {
        public CarregamentoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoPedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public void ExcluirCarregamentoPedido(int codigo)
        {
            {
                try
                {
                    if (UnitOfWork.IsActiveTransaction())
                    {
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoNotaFiscal WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoNotaFiscal c WHERE c.CarregamentoPedido.Codigo = :codigoCarregamentoPedido )").SetParameter("codigoCarregamentoPedido", codigo).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Codigo = :codigoCarregamentoPedido )").SetParameter("codigoCarregamentoPedido", codigo).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Codigo = :codigoCarregamentoPedido").SetParameter("codigoCarregamentoPedido", codigo).ExecuteUpdate();
                    }
                    else
                    {
                        try
                        {
                            UnitOfWork.Start();

                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoNotaFiscal WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoNotaFiscal c WHERE c.CarregamentoPedido.Codigo = :codigoCarregamentoPedido )").SetParameter("codigoCarregamentoPedido", codigo).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Codigo = :codigoCarregamentoPedido )").SetParameter("codigoCarregamentoPedido", codigo).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Codigo = :codigoCarregamentoPedido").SetParameter("codigoCarregamentoPedido", codigo).ExecuteUpdate();

                            UnitOfWork.CommitChanges();
                        }
                        catch
                        {
                            UnitOfWork.Rollback();
                            throw;
                        }
                    }
                }
                catch (NHibernate.Exceptions.GenericADOException ex)
                {
                    if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                    {
                        System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                        if (excecao.Number == 547)
                        {
                            throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                        }
                    }
                    throw;
                }

            }
        }

        public void ExcluirPorPedido(int codigoPedido)
        {
            {
                try
                {
                    if (UnitOfWork.IsActiveTransaction())
                    {
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Pedido.Codigo = :codigoPedido )").SetParameter("codigoPedido", codigoPedido).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Pedido.Codigo = :codigoPedido").SetParameter("codigoPedido", codigoPedido).ExecuteUpdate();
                    }
                    else
                    {
                        try
                        {
                            UnitOfWork.Start();

                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedidoProduto WHERE Codigo IN (SELECT c.Codigo FROM CarregamentoPedidoProduto c WHERE c.CarregamentoPedido.Pedido.Codigo = :codigoPedido )").SetParameter("codigoPedido", codigoPedido).ExecuteUpdate();
                            UnitOfWork.Sessao.CreateQuery("DELETE FROM CarregamentoPedido WHERE Pedido.Codigo = :codigoPedido").SetParameter("codigoPedido", codigoPedido).ExecuteUpdate();

                            UnitOfWork.CommitChanges();
                        }
                        catch
                        {
                            UnitOfWork.Rollback();
                            throw;
                        }
                    }
                }
                catch (NHibernate.Exceptions.GenericADOException ex)
                {
                    if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                    {
                        System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                        if (excecao.Number == 547)
                        {
                            throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                        }
                    }
                    throw;
                }

            }
        }

        public bool ExisteCarregamentoAtivoPorPedido(int codigoPedido)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(o => (o.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem ||
                o.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                && o.Pedido.Codigo == codigoPedido);

            return consultaCarregamentoPedido.Any();
        }

        public Task<bool> ExisteCarregamentoAtivoPorPedidoAsync(int codigoPedido)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(o => (o.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem ||
                o.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                && o.Pedido.Codigo == codigoPedido);

            return consultaCarregamentoPedido.AnyAsync(CancellationToken);
        }

        public bool ExisteCarregamentoAtivoPorPedidoPorProtocolo(int protocoloPedido)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(o => o.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado && o.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem && o.Pedido.Protocolo == protocoloPedido);

            return consultaCarregamentoPedido.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Pedido.Codigo == pedido select obj;

            return result.Fetch(x => x.Carregamento).ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>> BuscarPorPedidosAsync(List<int> pedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(await filter.Fetch(obj => obj.Carregamento)
                                      .ToListAsync(CancellationToken));
                start += take;
            }
            return result;
        }

        public int BuscarIDPropostaTrizyPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento && obj.Carregamento.IDPropostaTrizy > 0 select obj;

            return result.Select(c => c.Carregamento.IDPropostaTrizy)?.FirstOrDefault() ?? 0;
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido BuscarOutroCarregamentoPorPedidos(List<int> pedidos, bool redespacho, int codigo, bool carregamentoDeColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            List<int> pedidosFiltro = new List<int>();

            if (pedidos.Count > 2000)
                pedidosFiltro = pedidos.Skip(0).Take(2000).ToList();
            else
                pedidosFiltro = pedidos;

            var result = from obj in query
                         where
                            pedidosFiltro.Contains(obj.Pedido.Codigo)
                            && obj.Carregamento.CarregamentoRedespacho == redespacho
                            && obj.Carregamento.Codigo != codigo
                            && obj.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                            && obj.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado
                            && obj.Carregamento.CarregamentoColeta == carregamentoDeColeta
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido BuscarCarregamentoPorPedidos(List<int> pedidos, bool redespacho, int codigo, bool carregamentoDeColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query
                         where
                            pedidos.Contains(obj.Pedido.Codigo)
                            && obj.Carregamento.CarregamentoRedespacho == redespacho
                            && obj.Carregamento.Codigo == codigo
                            && obj.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                            && obj.Carregamento.CarregamentoColeta == carregamentoDeColeta
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarTodosCarregamentosPorPedidos(List<int> pedidos, bool redespacho, bool carregamentoDeColeta)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                                    && obj.Carregamento.CarregamentoRedespacho == redespacho
                                    && obj.Carregamento.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado
                                    && obj.Carregamento.CarregamentoColeta == carregamentoDeColeta
                             select obj;

                result.AddRange(filter.Fetch(obj => obj.Carregamento)
                                      .Fetch(obj => obj.Pedido)
                                      .ToList());
                start += take;
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorPedidosEmMontagem(int cod_sessao_roteirizador, List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                                && obj.Carregamento.SessaoRoteirizador.Codigo == cod_sessao_roteirizador
                                && obj.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem
                             select obj;

                result.AddRange(filter.Fetch(obj => obj.Carregamento)
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
                                      .ToList());
                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>> BuscarPorPedidosEmMontagemAsync(int cod_sessao_roteirizador, List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                                && obj.Carregamento.SessaoRoteirizador.Codigo == cod_sessao_roteirizador
                                && obj.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem
                             select obj;

                var partialResult = await filter
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
                    .ToListAsync(CancellationToken);

                result.AddRange(partialResult);
                start += take;
            }
            return result;
        }
        public int ContarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentoNotasFiscaisPedidos(int codigoCarregamento, List<int> codigosNotas, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            query = query.Where(obj => obj.Carregamento.Codigo == codigoCarregamento);
            query = query.Where(obj => codigosPedidos.Contains(obj.Pedido.Codigo));
            query = query.Where(obj => obj.Pedido.NotasFiscais.Any(x => codigosNotas.Contains(x.Codigo)));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentoPorOrdem(int codigoCarregamento)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            return consultaCarregamentoPedido
                .Fetch(o => o.Pedido).ThenFetch(o => o.Destinatario)
                .Fetch(o => o.Pedido).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Pedido).ThenFetch(o => o.Remetente)
                .OrderBy("Ordem")
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamento(int carregamento)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(carregamentoPedido => carregamentoPedido.Carregamento.Codigo == carregamento);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosParticaoUm = consultaCarregamentoPedido
                .Fetch(carregamentoPedido => carregamentoPedido.Carregamento).ThenFetch(x => x.ModeloVeicularCarga)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Destinatario).ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Remetente).ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Origem).ThenFetch(obj => obj.Estado)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Destino).ThenFetch(obj => obj.Estado)
                .ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosParticaoDois = consultaCarregamentoPedido
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Filial)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.TipoOperacao)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Recebedor)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Expedidor)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.CanalEntrega)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.RotaFrete)
                .ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoParticaoUm in carregamentoPedidosParticaoUm)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoParticaoDois = carregamentoPedidosParticaoDois.First(carregamentoPedido => carregamentoPedido.Codigo == carregamentoPedidoParticaoUm.Codigo);

                carregamentoPedidoParticaoUm.Pedido.Filial = carregamentoPedidoParticaoDois.Pedido.Filial;
                carregamentoPedidoParticaoUm.Pedido.TipoOperacao = carregamentoPedidoParticaoDois.Pedido.TipoOperacao;
                carregamentoPedidoParticaoUm.Pedido.Recebedor = carregamentoPedidoParticaoDois.Pedido.Recebedor;
                carregamentoPedidoParticaoUm.Pedido.Expedidor = carregamentoPedidoParticaoDois.Pedido.Expedidor;
                carregamentoPedidoParticaoUm.Pedido.CanalEntrega = carregamentoPedidoParticaoDois.Pedido.CanalEntrega;
                carregamentoPedidoParticaoUm.Pedido.RotaFrete = carregamentoPedidoParticaoDois.Pedido.RotaFrete;
            }

            return carregamentoPedidosParticaoUm;
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>> BuscarPorCarregamentoAsync(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == codigoCarregamento select obj;

            return result.ToListAsync(CancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>> BuscarPorCarregamentoPalletAsync(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return await result
                .Fetch(obj => obj.Pedido)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentoSemNotaParcial(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento && obj.Pedido.PedidoNotasParciais.Count == 0 select obj;

            return result
                .Fetch(obj => obj.Carregamento)
                .ThenFetch(x => x.ModeloVeicularCarga)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
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
                .Fetch(obj => obj.Pedido)
                .ThenFetch(x => x.Recebedor)
                .Fetch(x => x.Pedido)
                .ThenFetch(x => x.Expedidor)
                .Fetch(x => x.Pedido)
                .ThenFetch(x => x.CanalEntrega)
                .Fetch(x => x.Pedido)
                .ThenFetch(x => x.CTesTerceiro)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorSessaoRoteirizacaoEPedidos(int codigoSessaoRoteirizador, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            query = query.Where(obj => obj.Carregamento.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador && codigosPedidos.Contains(obj.Pedido.Codigo));

            return query.Fetch(o => o.Pedido)
                        .Fetch(o => o.Carregamento)
                        .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>> BuscarPorSessaoRoteirizacaoEPedidosAsync(int codigoSessaoRoteirizador, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            query = query.Where(obj => obj.Carregamento.SessaoRoteirizador.Codigo == codigoSessaoRoteirizador && codigosPedidos.Contains(obj.Pedido.Codigo));

            return query.Fetch(o => o.Pedido)
                        .Fetch(o => o.Carregamento)
                        .ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentoEPedidos(int codigoCarregamento, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            query = query.Where(obj => obj.Carregamento.Codigo == codigoCarregamento && codigosPedidos.Contains(obj.Pedido.Codigo));

            return query
                .Fetch(o => o.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentoFetchPedido(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;
            return result
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido BuscarPrimeiroPorCarregamentoFetchPedido(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result
                .Fetch(obj => obj.Pedido)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentoSemProdutos(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var queryProdutos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

            var result = from obj in query
                         where obj.Carregamento.Codigo == carregamento && !queryProdutos.Any(cpp => cpp.CarregamentoPedido.Codigo == obj.Codigo)
                         select obj;

            return result
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentos(List<int> carregamentos)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(carregamentoPedido => carregamentos.Contains(carregamentoPedido.Carregamento.Codigo));

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosParticaoUm = consultaCarregamentoPedido
                .Fetch(carregamentoPedido => carregamentoPedido.Carregamento).ThenFetch(x => x.ModeloVeicularCarga)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Destinatario)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Origem).ThenFetch(obj => obj.Estado)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Destino).ThenFetch(obj => obj.Estado)
                .ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosParticaoDois = consultaCarregamentoPedido
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Filial)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.TipoOperacao)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Recebedor)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Expedidor)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.RotaFrete)
                .ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoParticaoUm in carregamentoPedidosParticaoUm)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoParticaoDois = carregamentoPedidosParticaoDois.First(carregamentoPedido => carregamentoPedido.Codigo == carregamentoPedidoParticaoUm.Codigo);

                carregamentoPedidoParticaoUm.Pedido.Filial = carregamentoPedidoParticaoDois.Pedido.Filial;
                carregamentoPedidoParticaoUm.Pedido.TipoOperacao = carregamentoPedidoParticaoDois.Pedido.TipoOperacao;
                carregamentoPedidoParticaoUm.Pedido.Recebedor = carregamentoPedidoParticaoDois.Pedido.Recebedor;
                carregamentoPedidoParticaoUm.Pedido.Expedidor = carregamentoPedidoParticaoDois.Pedido.Expedidor;
                carregamentoPedidoParticaoUm.Pedido.RotaFrete = carregamentoPedidoParticaoDois.Pedido.RotaFrete;
            }

            return carregamentoPedidosParticaoUm;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>> BuscarPorCarregamentosAsync(List<int> carregamentos)
        {
            var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(carregamentoPedido => carregamentos.Contains(carregamentoPedido.Carregamento.Codigo));

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosParticaoUm = await consultaCarregamentoPedido
                .Fetch(carregamentoPedido => carregamentoPedido.Carregamento).ThenFetch(x => x.ModeloVeicularCarga)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Destinatario)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Remetente)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Origem).ThenFetch(obj => obj.Estado)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Destino).ThenFetch(obj => obj.Estado)
                .ToListAsync(CancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosParticaoDois = await consultaCarregamentoPedido
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Filial)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.TipoOperacao)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Recebedor)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.Expedidor)
                .Fetch(carregamentoPedido => carregamentoPedido.Pedido).ThenFetch(obj => obj.RotaFrete)
                .ToListAsync(CancellationToken);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoParticaoUm in carregamentoPedidosParticaoUm)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoParticaoDois = carregamentoPedidosParticaoDois.First(carregamentoPedido => carregamentoPedido.Codigo == carregamentoPedidoParticaoUm.Codigo);

                carregamentoPedidoParticaoUm.Pedido.Filial = carregamentoPedidoParticaoDois.Pedido.Filial;
                carregamentoPedidoParticaoUm.Pedido.TipoOperacao = carregamentoPedidoParticaoDois.Pedido.TipoOperacao;
                carregamentoPedidoParticaoUm.Pedido.Recebedor = carregamentoPedidoParticaoDois.Pedido.Recebedor;
                carregamentoPedidoParticaoUm.Pedido.Expedidor = carregamentoPedidoParticaoDois.Pedido.Expedidor;
                carregamentoPedidoParticaoUm.Pedido.RotaFrete = carregamentoPedidoParticaoDois.Pedido.RotaFrete;
            }

            return carregamentoPedidosParticaoUm;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCodigoDiferenteECarregamento(List<int> codigos, int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento && !codigos.Contains(obj.Codigo) select obj;

            return result
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido BuscarPorCarregamentoEPedido(int codigoPedido, int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == codigoCarregamento && obj.Pedido.Codigo == codigoPedido select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPrimeiroDestinatarioPedido(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            var result = from obj in query where obj.Carregamento.Codigo == codigoCarregamento select obj;

            return result.Select(o => o.Pedido.Destinatario)
                .Fetch(o => o.Localidade)
                .FirstOrDefault();
        }

        public void InserirSQL(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> itens)
        {
            if (itens != null && itens.Count > 0)
            {
                int take = 150;
                int start = 0;

                while (start < itens.Count)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> itensTmp = itens.Skip(start).Take(take).ToList();

                    string parameros = "( :CRG_CODIGO_[X], :PED_CODIGO_[X], :CRP_NUMERO_REBOQUE_[X], :CRP_PESO_[X], :CRP_VOLUME_BIPADO_[X], :CRP_VOLUME_TOTAL_[X], :CRP_PALLET_[X], :CRP_PESO_PALLET_[X] )";
                    string sql = @"
INSERT INTO T_CARREGAMENTO_PEDIDO (CRG_CODIGO, PED_CODIGO, CRP_NUMERO_REBOQUE, CRP_PESO, CRP_VOLUME_BIPADO, CRP_VOLUME_TOTAL, CRP_PALLET, CRP_PESO_PALLET) VALUES " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < itensTmp.Count; i++)
                        sql += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                    for (int i = 0; i < itensTmp.Count; i++)
                    {
                        query.SetParameter("CRG_CODIGO_" + i.ToString(), itensTmp[i].Carregamento.Codigo);
                        query.SetParameter("PED_CODIGO_" + i.ToString(), itensTmp[i].Pedido.Codigo);
                        query.SetParameter("CRP_NUMERO_REBOQUE_" + i.ToString(), itensTmp[i].NumeroReboque);
                        query.SetParameter("CRP_PESO_" + i.ToString(), itensTmp[i].Peso);
                        query.SetParameter("CRP_VOLUME_BIPADO_" + i.ToString(), itensTmp[i].VolumeBipado);
                        query.SetParameter("CRP_VOLUME_TOTAL_" + i.ToString(), itensTmp[i].VolumeTotal);
                        query.SetParameter("CRP_PALLET_" + i.ToString(), itensTmp[i].Pallet);
                        query.SetParameter("CRP_PESO_PALLET_" + i.ToString(), itensTmp[i].PesoPallet);
                    }

                    query.ExecuteUpdate();
                    start += take;
                }
            }
        }

        public List<int> PedidosCarregamentoTipoOperacaoPedidoColetaEntrega(List<int> pedidos)
        {
            string sql = @"
 SELECT CPE.PED_CODIGO
  FROM T_CARREGAMENTO_PEDIDO CPE
     , T_CARREGAMENTO		 CRG
	 , T_TIPO_OPERACAO		 TPE
 WHERE TPE.TOP_CODIGO		= CRG.TOP_CODIGO
   AND CRG.CRG_CODIGO		= CPE.CRG_CODIGO
   AND TPE.TOP_PEDIDO_COLETA_ENTREGA = 1
   AND CPE.PED_CODIGO IN ( :codigos )";

            List<int> result = new List<int>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetParameterList("codigos", tmp);
                result.AddRange(query.List<int>());

                start += take;
            }

            return result;
        }

        public IList<double> DestinatariosCarregamento(int codigoCarregamento)
        {
            string sql = @"
                        select distinct CLI_CGCCPF
                          from t_cliente		cli
	                         , t_pedido			ped
	                         , t_carregamento_pedido cpe
                         where cli.CLI_CGCCPF	= ped.CLI_CODIGO
                           and ped.PED_CODIGO	= cpe.PED_CODIGO
                           and cpe.CRG_CODIGO   = :codigo 
                         order by 1";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("codigo", codigoCarregamento);
            return query.List<double>();
        }

        public void AtualizarTipoOperacaoPedidosDoCarregamento(int codigoCarregamento, int codigoTipoOperacao)
        {
            if (codigoTipoOperacao == 0 || codigoCarregamento == 0)
                return;

            var querySql = $@"UPDATE Pedido
                    SET Pedido.TOP_CODIGO = Carregamento.TOP_CODIGO
                    FROM T_CARREGAMENTO Carregamento
                    JOIN T_CARREGAMENTO_PEDIDO Pedidos on Carregamento.CRG_CODIGO = Pedidos.CRG_CODIGO
                    JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = Pedidos.PED_CODIGO
                    WHERE Carregamento.CRG_CODIGO = {codigoCarregamento}";

            var query = this.SessionNHiBernate.CreateSQLQuery(querySql);

            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> BuscarPorCarregamentosSemFetch(List<int> codigosCarregamentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(o => codigosCarregamentos.Contains(o.Carregamento.Codigo));

            return query
                .ToList();
        }

        public decimal BuscarValorTotalMercadoriaPorCarregamento(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            query = query.Where(obj => obj.Carregamento.Codigo == codigoCarregamento && obj.Pedido != null);

            return query.Sum(o => (decimal?)o.Pedido.ValorTotalNotasFiscais) ?? 0m;
        }

        public void DeletarCarregamentoPedidoPendente(int codigoCarregamento)
        {
            var querySql = $@"delete from T_CARREGAMENTO_PEDIDO where CRG_CODIGO = {codigoCarregamento}";

            var query = this.SessionNHiBernate.CreateSQLQuery(querySql);

            query.ExecuteUpdate();
        }

        public void DeletarCarregamentoPendente(int codigoCarregamento)
        {
            var querySql = $@"delete from T_CARREGAMENTO where CRG_CODIGO = {codigoCarregamento}";

            var query = this.SessionNHiBernate.CreateSQLQuery(querySql);

            query.ExecuteUpdate();
        }
    }
}
