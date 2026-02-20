using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/CancelamentoSaldoReserva")]
    public class CancelamentoSaldoReservaController : BaseController
    {
        #region Construtores

        public CancelamentoSaldoReservaController(Conexao conexao) : base(conexao) { }

        #endregion Construtores

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "Codigo"
                };

                int quantidade = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);

                if (quantidade == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoExistemPedidosComOsFiltrosInformados);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = quantidade > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                List<int> codigosPedidos = (from pedido in pedidos select pedido.Codigo).Distinct().ToList();

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(codigosPedidos, filtrosPesquisa.CodigosProdutos, filtrosPesquisa.CodigosLinhaSeparacao, unitOfWork);

                // Validar se algum dos pedidos selecionados não estão em alguma sessão de roteirização em andamento.
                //Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork, cancellationToken);

                //List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosSessao = repositorioSessaoRoteirizadorPedido.BuscarSessaoRoteirizadorPorPedidos(codigosPedidos);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> pedidosProdutosSessao = repositorioSessaoRoteirizadorPedidoProduto.BuscarSessaoRoteirizadorPorPedidos(codigosPedidos);

                //// Filtrar somente pedidos em sessões em andamento.
                //pedidosSessao = (from obj in pedidosSessao
                //                 where obj.SessaoRoteirizador.SituacaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Iniciada
                //                 select obj).ToList();

                //saldoProdutos = (from obj in saldoProdutos
                //                 where obj.SaldoQtde > 0 && !pedidosSessao.Any(x => x.Pedido.Codigo == obj.CodigoPedido)
                //                 select obj).ToList();

                // Filtrar somente pedidos em sessões em andamento.
                pedidosProdutosSessao = (from obj in pedidosProdutosSessao
                                         where obj.SessaoRoteirizadorPedido.SessaoRoteirizador.SituacaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Iniciada
                                         select obj).ToList();

                saldoProdutos = (from obj in saldoProdutos
                                 where obj.SaldoQtde > 0 && !pedidosProdutosSessao.Any(x => x.PedidoProduto.Codigo == obj.CodigoPedidoProduto)
                                 select obj).ToList();

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                List<string> codigosCarga = repositorioCargaPedido.BuscarNumerosCargasPorPedidos(codigosPedidos);

                var retorno = (from obj in saldoProdutos
                               join ped in pedidos on obj.CodigoPedido equals ped.Codigo
                               where (obj.IdDemanda == filtrosPesquisa.IdDemanda || filtrosPesquisa.IdDemanda == "")
                               select new
                               {
                                   obj.CodigoPedido,
                                   Codigo = obj.CodigoPedidoProduto,
                                   obj.NumeroPedidoEmbarcador,
                                   Emitente = ped.Remetente?.CodigoIntegracao ?? string.Empty,
                                   Destinatario = ped.Destinatario?.CodigoIntegracao ?? string.Empty,
                                   CanalEntrega = ped.CanalEntrega?.Descricao ?? string.Empty,
                                   LinhaSeparacao = obj.LinhaSeparacao,
                                   GrupoProduto = obj.GrupoProduto,
                                   TipoDeCarga = ped.TipoDeCarga?.Descricao ?? string.Empty,
                                   Produto = obj.CodigoProdutoEmbarcador + " - " + obj.Produto,
                                   Situacao = (obj.QtdeCarregado > 0 ? "Atendido Parcial" : "Em Aberto"),
                                   obj.Qtde,
                                   obj.QtdeCarregado,
                                   obj.SaldoQtde,
                                   IdDemanda = obj.IdDemanda,
                                   PalletFechado = obj.PalletFechado ? "Sim" : "Não",
                                   CodigosCarga = string.Join(", ", codigosCarga)
                               }).ToList();

                //List<string> pedidosRemovidosRetorno = (from obj in pedidosSessao
                //                                        where !saldoProdutos.Any(x => x.CodigoPedido == obj.Pedido.Codigo)
                //                                        select obj.Pedido.NumeroPedidoEmbarcador).ToList();

                //return new JsonpResult(retorno, true, (pedidosRemovidosRetorno.Count == 0 ? "" : pedidosRemovidosRetorno.Count + " pedidos removidos da visualização por estarem em uma sessão de roteirização em andamento."));
                List<string> pedidosProdutosRemovidosRetorno = (from obj in pedidosProdutosSessao
                                                                where !saldoProdutos.Any(x => x.CodigoPedidoProduto == obj.PedidoProduto.Codigo)
                                                                select obj.SessaoRoteirizadorPedido.Pedido.NumeroPedidoEmbarcador).ToList();

                return new JsonpResult(retorno, true, (pedidosProdutosRemovidosRetorno.Count == 0 ? "" : pedidosProdutosRemovidosRetorno.Count + " produtos removidos da visualização por estarem em uma sessão de roteirização em andamento."));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> CancelarReserva()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutosCancelarReserva = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto>("Saldos");

                if (saldoProdutosCancelarReserva.Count == 0)
                    return new JsonpResult(false, true, "Nenhum pedido/produto informado para cancelar reserva de saldo.");

                List<int> codigosPedidos = (from saldo in saldoProdutosCancelarReserva
                                            select saldo.CodigoPedido).Distinct().ToList();

                // Validar se algum dos pedidos selecionados não estão em alguma sessão de roteirização em andamento.
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosSessao = repositorioSessaoRoteirizadorPedido.BuscarSessaoRoteirizadorPorPedidos(codigosPedidos);

                // Filtrar somente pedidos em sessões em andamento.
                pedidosSessao = (from obj in pedidosSessao
                                 where obj.SessaoRoteirizador.SituacaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Iniciada
                                 select obj).ToList();

                if (pedidosSessao.Count > 0)
                {
                    List<int> codigosSessoesDistintas = (from obj in pedidosSessao select obj.Codigo).Distinct().ToList();
                    string sessoesDistintas = string.Join(", ", codigosSessoesDistintas);
                    string pedidosDistintos = string.Join(",", (from obj in pedidosSessao select obj.Pedido.NumeroPedidoEmbarcador).Distinct().ToList());
                    if (pedidosDistintos.Length > 30)
                        pedidosDistintos = pedidosDistintos.Substring(0, 30) + "...";
                    return new JsonpResult(false, true, $"Os pedido {pedidosDistintos} estão relacionados {(codigosSessoesDistintas.Count > 1 ? "as sessões" : "à sessão")} de roteirização {sessoesDistintas} e não podem ser cancelado seu saldo.");
                }

                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(codigosPedidos);

                //11602
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosQuebra = pedidos.FindAll(x => x.QuebraMultiplosCarregamentos == false);
                if (pedidosQuebra?.Count > 0)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoCancelarReservaDosPedidosPoisOsMesmosNaoPermitemQuebraNaRoteirizacao, string.Join(", ", (from nro in pedidosQuebra
                                                                                                                                                                                                                           select nro.NumeroPedidoEmbarcador).ToArray())));

                int erro = 0;
                List<int> pedidosComCarregamento = repositorioPedido.BuscarPorCodigosComCarregamento(codigosPedidos);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidosNaoAtendido = repositorioPedidoProduto.BuscarPorCodigos((from saldo in saldoProdutosCancelarReserva
                                                                                                                                                 select saldo.CodigoPedidoProduto).Distinct().ToList()); //.ProdutosPedidosNaoAtendidosTotalmente(codigosPedidos);

                //Consulta todos os carregamentos pedido produto;;
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosCarregadoPedidos = repositorioCarregamentoPedidoProduto.BuscarPorPedidos(codigosPedidos);

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> listaTiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte
                };

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(listaTiposIntegracao, null);

                foreach (int codigoPedido in codigosPedidos)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);

                        // 1 - Vamos obter o saldo dos produtos do pedido....
                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutosPedido = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, unitOfWork);

                        List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> produtosNaoContidosSelecaoCancelamento = (from obj in saldoProdutosPedido
                                                                                                                                                 where !produtosPedidosNaoAtendido.Any(x => x.Pedido.Codigo == obj.CodigoPedido && x.Codigo == obj.CodigoPedidoProduto)
                                                                                                                                                 select obj).ToList();

                        bool todosProdutosPedidoSelecionadosCancelar = (produtosNaoContidosSelecaoCancelamento.Count == 0);

                        // 2 - Os pedidos que foram cancelados integralmente, ou seja, que não foram incluídos em nenhum carregamento devem ficar com o status de cancelado ?
                        bool existeCarregamentoDoPedido = pedidosComCarregamento?.Exists(p => p == pedido.Codigo) ?? false;
                        if (!existeCarregamentoDoPedido && todosProdutosPedidoSelecionadosCancelar)
                        {
                            pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                            repositorioPedido.Atualizar(pedido);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Cancelado saldo de reserva do pedido.", unitOfWork);
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidoNaoAtendido = (from produto in produtosPedidosNaoAtendido
                                                                                                                  where produto.Pedido.Codigo == pedido.Codigo
                                                                                                                  select produto).ToList();

                            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                                Servicos.Embarcador.Integracao.IntegracaoPedido.GerarIntegracaoPedidoCancelamentoReserva(tipoIntegracao, pedido, produtosPedidoNaoAtendido, this.Usuario, unitOfWork);

                        }
                        else
                        {
                            List<int> codigosProdutosPedidoNaoAtendido = (from prod in produtosPedidosNaoAtendido
                                                                          where prod.Pedido.Codigo == pedido.Codigo
                                                                          select prod.Codigo).ToList();
                            if (codigosProdutosPedidoNaoAtendido?.Count > 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosCarregadoPedido = (from prod in produtosCarregadoPedidos
                                                                                                                                             where prod.PedidoProduto.Pedido.Codigo == pedido.Codigo
                                                                                                                                             select prod).ToList();

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosCancelar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                                foreach (int codigoPedidoProduto in codigosProdutosPedidoNaoAtendido)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoAtualizar = repositorioPedidoProduto.BuscarPorCodigo(codigoPedidoProduto);
                                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtoCarregado = produtosCarregadoPedido?.FindAll(x => x.PedidoProduto.Codigo == codigoPedidoProduto);
                                    decimal qtde = 0;
                                    decimal peso = 0;
                                    decimal pallet = 0;
                                    decimal metro = 0;
                                    if (produtoCarregado != null)
                                    {
                                        qtde = produtoCarregado.Sum(x => x.Quantidade);
                                        peso = produtoCarregado.Sum(x => x.Peso);
                                        pallet = produtoCarregado.Sum(x => x.QuantidadePallet);
                                        metro = produtoCarregado.Sum(x => x.MetroCubico);
                                    }

                                    if (qtde < pedidoProdutoAtualizar.Quantidade || peso < pedidoProdutoAtualizar.PesoTotal || pallet < pedidoProdutoAtualizar.QuantidadePalet || metro < pedidoProdutoAtualizar.MetroCubico)
                                    {
                                        produtosCancelar.Add(new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                                        {
                                            Codigo = pedidoProdutoAtualizar.Codigo,
                                            MetroCubico = pedidoProdutoAtualizar.MetroCubico - metro,
                                            Quantidade = pedidoProdutoAtualizar.Quantidade - qtde,
                                            QuantidadePalet = pedidoProdutoAtualizar.QuantidadePalet - pallet,
                                            PesoUnitario = pedidoProdutoAtualizar.PesoUnitario,
                                            PesoTotalEmbalagem = pedidoProdutoAtualizar.PesoTotalEmbalagem
                                        });

                                        //Comentado, pois não estava cancelando o saldo.. quando o produto não possuia nenhum carregamento.. 
                                        ////Se teve algum carregamento do produto.. vamos atualizar as qtdes do pedido/produto.
                                        //if (qtde + peso + pallet + metro > 0)
                                        //{
                                        pedidoProdutoAtualizar.Quantidade = qtde;
                                        pedidoProdutoAtualizar.QuantidadePalet = pallet;
                                        pedidoProdutoAtualizar.MetroCubico = metro;
                                        repositorioPedidoProduto.Atualizar(pedidoProdutoAtualizar);
                                        //}
                                    }
                                }

                                foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                                    Servicos.Embarcador.Integracao.IntegracaoPedido.GerarIntegracaoPedidoCancelamentoReserva(tipoIntegracao, pedido, produtosCancelar, this.Usuario, unitOfWork);

                                // Aqui, vamos descontar do peso total do pedido... o peso que foi cancelado a reserva...
                                decimal pesoTotalCancelado = (from obj in produtosCancelar select obj.PesoTotal).Sum();
                                if (pesoTotalCancelado > 0)
                                {
                                    pedido.PesoTotal -= pesoTotalCancelado;
                                    pedido.PesoSaldoRestante -= pesoTotalCancelado;
                                    pedido.PedidoTotalmenteCarregado = (pedido.PesoSaldoRestante <= (decimal)0.5);
                                    repositorioPedido.Atualizar(pedido);
                                    //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                                    Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. CancelamentoSaldoReservaController.CancelarReserva", "SaldoPedido");
                                }

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Cancelado saldo de reserva parcial do pedido.", unitOfWork);
                            }

                            //#49551 - Verificar se todos os produtos do pedido foram cancelados.. e vamos "Cancelar" ele, caso não tenha mais saldo.. vamos "Finalizar" ele.
                            if (pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado && pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado)
                            {
                                saldoProdutosPedido = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, unitOfWork);

                                bool existeQuantidadeProduto = saldoProdutosPedido.Any(x => x.Qtde > 0);
                                if (!existeQuantidadeProduto)
                                {
                                    pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                                    repositorioPedido.Atualizar(pedido);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Cancelado saldo de reserva do pedido.", unitOfWork);
                                }
                                else
                                {
                                    bool existeSaldoQuantidadeProduto = saldoProdutosPedido.Any(x => x.SaldoQtde > 0);
                                    if (!existeSaldoQuantidadeProduto)
                                    {
                                        pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado;
                                        repositorioPedido.Atualizar(pedido);
                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Finalizado por saldo de reserva do pedido.", unitOfWork);
                                    }
                                }
                            }
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        erro++;
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                    }
                }

                if (erro == codigosPedidos.Count)
                    return new JsonpResult(false, "Não foi possível cancelar reserva dos pedidos.");
                else if (erro > 0)
                    return new JsonpResult(erro, false, $"Não foi possível cancelar reserva de {erro.ToString()} pedidos.");
                else
                    return new JsonpResult(true, "Cancelamento de Reserva realizado com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoRemoverOsPedidosDaSessaoDeRoteirizacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            double codigoDestinatario = Request.GetDoubleParam("Destinatario");
            string estadoOrigem = Request.GetStringParam("EstadoOrigem");
            string estadoDestino = Request.GetStringParam("EstadoDestino");

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosCanalEntrega = Request.GetListParam<int>("CodigosCanalEntrega");
            List<int> codigosTipoDeCarga = Request.GetListParam<int>("CodigosTipoDeCarga");

            List<double> codigosDestinatario = Request.GetListParam<double>("Destinatario");

            List<string> estadosOrigem = Request.GetListParam<string>("EstadosOrigem");
            List<string> estadosDestino = Request.GetListParam<string>("EstadosDestino");

            if ((codigoFilial > 0) && (codigosFilial.Count == 0))
                codigosFilial.Add(codigoFilial);

            if (codigoCanalEntrega > 0 && codigosCanalEntrega.Count == 0)
                codigosCanalEntrega.Add(codigoCanalEntrega);

            if ((codigoDestinatario > 0) && (codigosDestinatario.Count == 0))
                codigosDestinatario.Add(codigoDestinatario);

            if ((!string.IsNullOrEmpty(estadoOrigem) && estadoOrigem != "0") && (estadosOrigem.Count == 0))
                estadosOrigem.Add(estadoOrigem);

            if ((!string.IsNullOrEmpty(estadoDestino) && estadoDestino != "0") && (estadosDestino.Count == 0))
                estadosDestino.Add(estadoDestino);

            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.NaoInformado;

            List<Dominio.Enumeradores.TipoTomador> tiposTomador = Request.GetListEnumParam<Dominio.Enumeradores.TipoTomador>("TiposTomador");

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial,
                CodigosTipoCarga = codigosTipoDeCarga.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTipoDeCarga,
                CodigosProdutos = Request.GetListParam<int>("CodigosProdutos"),
                Destinatarios = codigosDestinatario,
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                EstadosOrigem = estadosOrigem,
                EstadosDestino = estadosDestino,
                ExibirPedidosExpedidor = ConfiguracaoEmbarcador.NaoGerarCarregamentoRedespacho,
                FiltrarPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                GrupoPessoa = Request.GetIntParam("GrupoPessoaRemetente"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                NumeroPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                OrdernarPorPrioridade = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS),
                PedidoSemCarregamento = ConfiguracaoEmbarcador.FiltrarPorPedidoSemCarregamentoNaMontagemCarga,
                PedidoSemCarga = true,
                OcultarPedidosRetiradaProdutos = true,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                Transbordo = false,
                CodigosCanalEntrega = codigosCanalEntrega,
                GrupoPessoaDestinatario = Request.GetIntParam("GrupoPessoaDestinatario"),
                ProgramaComSessaoRoteirizador = Request.GetBoolParam("ComSessaoRoteirizador"),
                OpcaoSessaoRoteirizador = Request.GetEnumParam("OpcaoSessaoRoteirizador", Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcaoSessaoRoteirizador.NENHUM),
                CodigoSessaoRoteirizador = Request.GetIntParam("SessaoRoteirizador"),
                SituacaoSessaoRoteirizador = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador>("SituacaoSessaoRoteirizador", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Todas),
                TipoFiltroData = ConfiguracaoEmbarcador.TipoFiltroDataMontagemCarga,
                TipoCarga = codigoTipoCarga,
                TipoDeCarga = Request.GetIntParam("TipoDeCarga"),
                Deposito = Request.GetIntParam("Deposito"),
                CodigosLinhaSeparacao = Request.GetListParam<int>("CodigosLinhaSeparacao"),
                CodigosGrupoProdutos = Request.GetListParam<int>("CodigosGrupoProdutos"),
                CodigosCategoriaClientes = Request.GetListParam<int>("CodigosCategoriaClientes"),
                CodigosPedido = Request.GetListParam<int>("CodigosPedidos"),
                TipoTomador = tipoTomador,
                TiposTomador = tiposTomador,
                IdDemanda = Request.GetStringParam("IdDemanda"),
                CodigosCarga = Request.GetListParam<int>("CodigosCarga")
            };

            filtrosPesquisa.CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Codigo;
            }

            return filtrosPesquisa;
        }

        #endregion Métodos Privados
    }
}
