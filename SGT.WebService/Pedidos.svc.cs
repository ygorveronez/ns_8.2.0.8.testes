using CoreWCF;
using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Carga;
using Dominio.ObjetosDeValor.WebService.Rest.Pedidos;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Pedidos(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IPedidos
    {
        public Retorno<bool> AdicionarPedidoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocolo.protocoloIntegracaoPedido <= 0)
                    throw new WebServiceException("O protocolo do pedido não foi informado.");

                if (protocolo.protocoloIntegracaoCarga <= 0)
                    throw new WebServiceException("O protocolo da carga não foi informado.");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocolo.protocoloIntegracaoPedido);

                if (pedido == null)
                    throw new WebServiceException("O pedido não foi encontrado.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("A carga não foi encontrada.");

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargaPedido != null)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Pedido já adicionado a carga.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                cargaPedido = Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(carga, pedido, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, false);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Adicionou o pedido {pedido.NumeroPedidoEmbarcador}", unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                //servicoCarga.ValidarCapacidadeModeloVeicularCarga(carga, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionados: new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, cargaPedidosRemovidos: null);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao Adicionar o Pedido na Carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<Retorno<Dominio.ObjetosDeValor.ProcessadorTarefas.RetornoAdicionarRequestAssincrono>> AdicionarPedidoEmLote(List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.Interfaces.ProcessadorTarefas.IAdicionarRequestAssincrono servicoAdicionarRequestAssincrono = _serviceProvider.GetRequiredService<Dominio.Interfaces.ProcessadorTarefas.IAdicionarRequestAssincrono>();

                List<TipoEtapaTarefa> etapas = new List<TipoEtapaTarefa>
                {
                    TipoEtapaTarefa.QuebrarRequest,
                    TipoEtapaTarefa.AdicionarPedido,
                    TipoEtapaTarefa.RetornarIntegracao
                };

                Dominio.ObjetosDeValor.ProcessadorTarefas.RetornoAdicionarRequestAssincrono retornoAdicionarRequestAssincrono = await servicoAdicionarRequestAssincrono.SalvarLoteAsync(cargasIntegracao, TipoRequest.AdicionarPedidoEmLote, etapas, default, integradora.Codigo);

                return Retorno<Dominio.ObjetosDeValor.ProcessadorTarefas.RetornoAdicionarRequestAssincrono>.CriarRetornoSucesso(retornoAdicionarRequestAssincrono, "Lote adicionado para processamento.");
            });
        }

        public Retorno<bool> AlterarSaldoProdutoPedido(int protocoloIntegracaoPedido, List<Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoSaldoProdutoPedido> Alteracoes, string motivoAlteracao)
        {
            ValidarToken();

            Servicos.Log.TratarErro($"AlterarSaldoProdutoPedido: {protocoloIntegracaoPedido} - {(Alteracoes != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Alteracoes) : string.Empty)}", "Request");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocoloIntegracaoPedido);

                if (pedido == null)
                    throw new WebServiceException("Esse pedido não existe");

                if (Alteracoes.Count == 0)
                    throw new WebServiceException("Pelo menos uma alteração deve ser enviada");

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, unitOfWork);

                unitOfWork.Start();

                foreach (Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoSaldoProdutoPedido alteracao in Alteracoes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repositorioPedidoProduto.BuscarPorPedidoProdutoEmbarcador(pedido.Codigo, alteracao.ProtocoloProduto);

                    if (pedidoProduto == null)
                        throw new WebServiceException($"O produto com protocolo {alteracao.ProtocoloProduto} não existe nesse pedido");

                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto saldoPedidoProduto = (from saldo in saldoProdutos
                                                                                                                   where saldo.CodigoPedidoProduto == pedidoProduto.Codigo
                                                                                                                   select saldo).FirstOrDefault();

                    // Ex: Pedido está com 100 sacos de cimento.
                    //     Gerado carregamento com 80 sacos (Saldo 20)
                    //     Se atualizado novo saldo para 10, deverá atualizar (80 + 10) ficando com a qtde de 90
                    decimal qtdeCarregado = saldoPedidoProduto?.QtdeCarregado ?? 0; // (saldoPedidoProduto?.Qtde - saldoPedidoProduto?.SaldoQtde) ?? 0;
                    decimal novoSaldo = (qtdeCarregado + alteracao.NovoSaldo);
                    decimal qtdeAtual = pedidoProduto.Quantidade;

                    pedidoProduto.Quantidade = novoSaldo;
                    repositorioPedidoProduto.Atualizar(pedidoProduto);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Alterou o saldo do produto {pedidoProduto.Produto.CodigoProdutoEmbarcador} para {alteracao.NovoSaldo} sendo a quantidade atual {qtdeAtual} para {novoSaldo} através do WS de integração. Motivo: {motivoAlteracao}", unitOfWork);
                }

                //Aki vamos atualizar o PesoTotal do Pedido
                // Obter todos os produtos do pedido... 
                // Calcular o peso total dos produtos..
                // Analisar o peso carregado
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido = repositorioPedidoProduto.BuscarPorPedido(pedido.Codigo);
                decimal pesoTotalProdutos = produtosPedido.Sum(x => x.PesoTotal);
                decimal cubagemTotalProdutos = produtosPedido.Sum(x => x.MetroCubico);
                decimal quantidade = produtosPedido.Sum(x => x.Quantidade);
                // Atualizando o saldo dos produtos novamente.. para calcularmos o peso do saldo restante...
                saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, unitOfWork);
                // Filtrando apenas os produtos que possuem saldo.
                saldoProdutos = (from obj in saldoProdutos where obj.SaldoQtde > 0 select obj).ToList();

                decimal saldoRestate = (from sp in saldoProdutos join pp in produtosPedido on sp.CodigoPedidoProduto equals pp.Codigo select (sp.SaldoQtde * pp.PesoUnitario) + pp.PesoTotalEmbalagem).Sum();

                pedido.PesoTotal = pesoTotalProdutos;
                pedido.PesoSaldoRestante = saldoRestate; // ((pesoTotalProdutos - pesoCarregado) > 0 ? (pesoTotalProdutos - pesoCarregado) : 0);
                pedido.PedidoTotalmenteCarregado = (pedido.PesoSaldoRestante <= (decimal)0.5); // pedido.PesoSaldoRestante == 0;
                pedido.CubagemTotal = cubagemTotalProdutos;
                pedido.QtVolumes = (int)quantidade;

                //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. PedidosSVC.AlterarSaldoProdutoPedido", "SaldoPedido");

                repositorioPedido.Atualizar(pedido);

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"AlterarSaldoProdutoPedido: {protocoloIntegracaoPedido} - Sucesso.");

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar a baixa do saldo do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarPedidoAsync(cargaIntegracao, true, default).Result);
            });
        }

        public async Task<Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> AdicionarPedidoNovoAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CreateFrom(await new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarPedidoAsync(cargaIntegracao, true, default));
            });
        }

        public Retorno<bool> ConfirmarIntegracaoCarregamento(int protocolo)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            try
            {

                //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocolo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo);
                if (carga != null && carga.Carregamento != null)
                {
                    if (!carga.CarregamentoIntegradoERP)
                    {
                        carga.CarregamentoIntegradoERP = true;
                        repCarregamento.Atualizar(carga.Carregamento);
                        retorno.Objeto = true;
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Confirmou integração do carregamento.", unitOfWork);
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Status = true;
                        retorno.Mensagem = "A confirmação da integração já foi realizada anteriormente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado um carregamento para o protocolo informado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracao(int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarCarregamentosPendentesIntegracao(new RequestCarregamentosPendentesIntegracao() { Inicio = inicio ?? 0, Limite = limite ?? 0 }, integradora, true));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ExcluirNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, int protocoloNotaFiscal)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(protocoloNotaFiscal);

                        if (xMLNotaFiscal != null)
                        {
                            unitOfWork.Start();
                            string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.ExcluirNotaFiscal(xMLNotaFiscal, cargaPedido, Auditado, TipoServicoMultisoftware, unitOfWork);
                            if (string.IsNullOrWhiteSpace(retornoIntegracao))
                            {
                                retorno.Objeto = true;
                                retorno.Status = true;
                                unitOfWork.CommitChanges();
                            }
                            else
                            {
                                retorno.Objeto = true;
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.Mensagem = "Não foi localizada a nota fiscal. ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "Protocolos informados são inválidos. ";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Não foi localizado o pedido. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao excluir a Nota Fiscal";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public Retorno<bool> InformarSeparacaoPedido(Dominio.ObjetosDeValor.WebService.Pedido.SeparacaoPedido protocoloSeparacaoPedido)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Servicos.Log.TratarErro("InformarSeparacaoPedido - protocoloSeparacaoPedido: " + (protocoloSeparacaoPedido != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocoloSeparacaoPedido) : string.Empty), "Request");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (protocoloSeparacaoPedido.ProtocoloIntegracaoPedido <= 0)
                    throw new WebServiceException("É obrigatório informar o protocolo de integração do pedido.");

                if (protocoloSeparacaoPedido.PercentualSeparacao < 0 || protocoloSeparacaoPedido.PercentualSeparacao > 100)
                    throw new WebServiceException("O percentual de separação do pedido deve ser entre 0% e 100%.");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocoloSeparacaoPedido.ProtocoloIntegracaoPedido);

                if (pedido == null)
                    throw new WebServiceException("O pedido não foi encontrado.");

                new Servicos.Embarcador.Pedido.Pedido().InformarSeparacaoPedido(pedido, protocoloSeparacaoPedido, unitOfWork);

                Servicos.Log.TratarErro("InformarSeparacaoPedido - Retorno: Sucesso");

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);

            }
            catch (BaseException ws)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ws);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ws.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu um falha ao informar a separação do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarDadosNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            try
            {
                if (notasFiscais.Count <= 0)
                {
                    retorno.Status = false;
                    retorno.Mensagem = "As notas fiscais não foram enviadas.";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
                else
                {

                    if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                    {

                        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                        Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                        Servicos.Embarcador.Pedido.Produto serProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);
                        Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                        Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                        Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                        //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                        if (cargaPedido != null)
                        {
                            if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe || (cargaPedido.Carga.CargaEmitidaParcialmente && (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos))))
                            {
                                unitOfWork.Start();
                                string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(cargaPedido, notasFiscais, null, null, configuracao, TipoServicoMultisoftware, Auditado, integradora, unitOfWork);
                                if (string.IsNullOrWhiteSpace(retornoIntegracao))
                                {
                                    retorno.Objeto = true;
                                    retorno.Status = true;
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, "Integrou dados de notas fiscais", unitOfWork);
                                    unitOfWork.CommitChanges();
                                }
                                else
                                {
                                    retorno.Objeto = true;
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    unitOfWork.Rollback();
                                }
                            }
                            else
                            {
                                //logs para arcelor Mittal
                                if (configuracao.IncluirCargaCanceladaProcessarDT)
                                {
                                    Servicos.Log.TratarErro($"IntegrarDadosNotasFiscais - protocoloIntegracaoCarga: {protocolo.protocoloIntegracaoCarga} | protocoloIntegracaoPedido: {protocolo.protocoloIntegracaoPedido}");
                                    Servicos.Log.TratarErro($"Situacao Carga: {cargaPedido.Carga.DescricaoSituacaoCarga}");
                                }

                                if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                                    || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador
                                    || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                                    || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                    || (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete))
                                {
                                    retorno.Status = false;
                                    if (cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    else
                                        retorno.CodigoMensagem = 313;
                                    retorno.Mensagem += "Não é possível enviar as notas fiscais para a carga em sua atual situação (" + cargaPedido.Carga.DescricaoSituacaoCarga + "). ";
                                }
                                else
                                {
                                    if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && cargaPedido.Carga.CargaEmitidaParcialmente)
                                    {
                                        retorno.Mensagem += "Para enviar as demais notas a carga precisa estar na situação em transporte.";
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    }
                                    else
                                    {
                                        retorno.Mensagem += "As notas físcais já foram enviadas para essa carga.";
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                    }
                                    retorno.Status = false;
                                }
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.Mensagem = "Protocolos informados são inválidos. ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            //string objetoJson = Newtonsoft.Json.JsonConvert.SerializeObject(TokensXMLNotasFiscais);
                            //ArmazenarLogParametros(objetoJson);
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "É obrigatório informar os protocolos de integração. ";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    }
                    if (retorno.Status)
                    {
                        foreach (string caminho in caminhosXMLTemp)
                            Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                    }
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) NF-e(s)";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (retorno.CodigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
            {
                //falar com rafael da pelissari aqui está gerando muito log, fica tentando mandar sem parar para viagens de armazens
                //Servicos.Log.TratarErro(retorno.Mensagem);
                //Servicos.Log.TratarErro("Protocolo Pedido: " + protocolo != null ? protocolo.protocoloIntegracaoPedido.ToString() : "nulo" + "Protocolo Carga: " + protocolo != null ? protocolo.protocoloIntegracaoCarga.ToString() : "nulo" + " Retorno: " + retorno.Mensagem + " Status:" + retorno.Status.ToString());
            }

            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento)
        {
            Servicos.Log.TratarErro("SolicitarCancelamentoDoPedido - protocoloIntegracaoPedido: " + protocoloIntegracaoPedido.ToString());

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            string erro = string.Empty;

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocoloIntegracaoPedido);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (configuracao.TrocarPreCargaPorCarga)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    if (repositorioCargaPedido.ExistePorPedidoPorProtocolo(protocoloIntegracaoPedido, cargasAtivas: false))
                        throw new WebServiceException("O pedido já está vinculado à uma carga, não sendo possível realizar a exclusão do mesmo.");

                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

                    if (repositorioCarregamentoPedido.ExisteCarregamentoAtivoPorPedidoPorProtocolo(protocoloIntegracaoPedido))
                        throw new WebServiceException("O pedido já está vinculado à um carregamento, não sendo possível realizar a exclusão do mesmo.");
                }
                else
                    Servicos.Embarcador.Pedido.Pedido.RemoverPedidoCancelado(pedido, unitOfWork, TipoServicoMultisoftware, Auditado, configuracao, configuracaoGeralCarga);

                if (!Servicos.Embarcador.Pedido.Pedido.CancelarPedido(out erro, pedido, TipoPedidoCancelamento.Cancelamento, null, motivoDoCancelamento, unitOfWork, TipoServicoMultisoftware, Auditado, configuracao, Cliente))
                    throw new WebServiceException(erro);

                unitOfWork.CommitChanges();

                return Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoSucesso(RetornoSolicitacaoCancelamento.Cancelada);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(excecao.Message, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoExcecao(!string.IsNullOrWhiteSpace(erro) ? erro : "Ocorreu uma falha ao solicitar o cancelamento do pedido", RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> LiberarEmissaoSemNFe(int protocoloIntegracaoCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("A carga informada não existe no Multi Embarcador");

                if (carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    throw new WebServiceException("A carga informada não esta pendente de NF-e no MultiEmbarcador");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, Auditado);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar a liberação da carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> EnviarArquivoAnexoPDF(Stream arquivo)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Carga" });

                string nomeArquivo = Servicos.Arquivo.SalvarArquivoPDF(arquivo, caminho);

                Servicos.Log.TratarErro("EnviarArquivo: " + nomeArquivo, "EnviarArquivoAnexoPDF");

                return Retorno<string>.CriarRetornoSucesso(nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarArquivoAnexoPDF");
                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao salvar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarArquivoAnexoPDF(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.WebService.Carga.TokenArquivoAnexo> tokensArquivoAnexo)
        {
            Servicos.Log.TratarErro("IntegrarArquivoAnexoPDF - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "IntegrarArquivoAnexoPDF");
            Servicos.Log.TratarErro("IntegrarArquivoAnexoPDF - TokensArquivosAnexo: " + (tokensArquivoAnexo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(tokensArquivoAnexo) : string.Empty), "IntegrarArquivoAnexoPDF");

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                VincularArquivoAnexo(protocolo, integradora, tokensArquivoAnexo, unitOfWork);

                Servicos.Log.TratarErro("IntegrarArquivoAnexoPDF - Retorno: Sucesso", "IntegrarArquivoAnexoPDF");

                return Retorno<bool>.CriarRetornoSucesso(true);

            }
            catch (BaseException ws)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ws, "IntegrarArquivoAnexoPDF");
                ArmazenarLogIntegracao(tokensArquivoAnexo, unitOfWork);

                return Retorno<bool>.CriarRetornoDadosInvalidos(ws.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "IntegrarArquivoAnexoPDF");
                ArmazenarLogIntegracao(tokensArquivoAnexo, unitOfWork);

                return Retorno<bool>.CriarRetornoExcecao("Ocorreu um falha ao integrar o arquivo anexo a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void VincularArquivoAnexo(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.Entidades.WebService.Integradora integradora, List<Dominio.ObjetosDeValor.WebService.Carga.TokenArquivoAnexo> tokensArquivosAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaAnexo repCargaAnexo = new Repositorio.Embarcador.Cargas.CargaAnexo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

            if (protocolo.protocoloIntegracaoCarga > 0 && carga == null)
                throw new WebServiceException($"Protocolo {protocolo.protocoloIntegracaoCarga} da carga informados inválido.");

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
            Repositorio.Embarcador.Pedidos.PedidoAnexo repPedidoAnexo = new Repositorio.Embarcador.Pedidos.PedidoAnexo(unitOfWork);

            if (protocolo.protocoloIntegracaoCarga == 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                cargasPedido = repCargaPedido.BuscarPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);
            }

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            pedido = repPedido.BuscarPorProtocolo(protocolo.protocoloIntegracaoPedido);

            if (protocolo.protocoloIntegracaoPedido > 0 && pedido == null && cargasPedido.Count == 0)
                throw new WebServiceException($"Protocolo {protocolo.protocoloIntegracaoPedido} do pedido informado inválido.");

            int qtdeAnexo = 0;

            //string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;
            string path = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Carga" });

            List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
            foreach (Dominio.ObjetosDeValor.WebService.Carga.TokenArquivoAnexo tokenArquivo in tokensArquivosAnexo)
            {
                qtdeAnexo += 1;
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(tokenArquivo.Token, ".pdf"));

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    throw new WebServiceException($"O Token {tokenArquivo.Token} não existe mais físicamente no servidor, por favor, envie o PDF da carga novamente e receba um novo token.");

                if (cargasPedido.Count > 0)
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaAnexo cargaAnexo = new Dominio.Entidades.Embarcador.Cargas.CargaAnexo()
                        {
                            EntidadeAnexo = cargaPedido.Carga,
                            Descricao = tokenArquivo.Descricao,
                            NomeArquivo = tokenArquivo.NomeArquivo ?? Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(tokenArquivo.Token)),
                            GuidArquivo = tokenArquivo.Token
                        };

                        repCargaAnexo.Inserir(cargaAnexo, Auditado);
                    }
                if (cargasPedido.Count == 0 && carga != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaAnexo cargaAnexo = new Dominio.Entidades.Embarcador.Cargas.CargaAnexo()
                    {
                        EntidadeAnexo = carga,
                        Descricao = tokenArquivo.Descricao,
                        NomeArquivo = tokenArquivo.NomeArquivo ?? Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(tokenArquivo.Token)),
                        GuidArquivo = tokenArquivo.Token
                    };

                    repCargaAnexo.Inserir(cargaAnexo, Auditado);
                }

                if (pedido != null && protocolo.protocoloIntegracaoCarga == 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo pedidoAnexo = new Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo()
                    {
                        EntidadeAnexo = pedido,
                        Descricao = tokenArquivo.Descricao,
                        NomeArquivo = tokenArquivo.NomeArquivo ?? Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(tokenArquivo.Token)),
                        GuidArquivo = tokenArquivo.Token
                    };

                    repPedidoAnexo.Inserir(pedidoAnexo, Auditado);
                }
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>> BuscarPedidosPendentesIntegracao(int? inicio, int? limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.Pedido serPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPedidosAgIntegracao(integradora.Empresa?.Codigo ?? 0, (int)inicio, (int)limite);
                    Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido> paginacao = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>();
                    paginacao.Itens = serPedido.ConverterPedidos(pedidos);
                    paginacao.NumeroTotalDeRegistro = repPedido.ContarPedidosAgIntegracao(integradora.Empresa?.Codigo ?? 0);
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoSucesso(paginacao);
                }
                else
                {
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100");
                }
            }
            catch (WebServiceException ex)
            {
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoDadosInvalidos(ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as cargas pendentes integração");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public Retorno<bool> ConfirmarPedidosPendentesIntegracao(List<int> protocolosPedido)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCodigos(protocolosPedido);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    pedido.Initialize();
                    pedido.AguardandoIntegracao = false;
                    repPedido.Atualizar(pedido, Auditado);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido> ConsultarSituacaoPedido(int protocoloIntegracaoPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConsultarSituacaoPedido(protocoloIntegracaoPedido));
            });
        }

        public Retorno<bool> AtualizarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarCargaPadrao(cargaIntegracao, protocolo, unitOfWork.StringConexao));
            });
        }

        public Retorno<bool> AtualizarPedidoProduto(Dominio.ObjetosDeValor.WebService.Pedido.AtualizacaoPedidoProduto atualizacaoPedidoProduto)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarPedidoProduto(atualizacaoPedidoProduto));
            });
        }

        public Retorno<bool> AlterarPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlteracaoPedido alteracaoPedido)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Pedido.AlteracaoPedido servicoAlteracaoPedido = new Servicos.Embarcador.Pedido.AlteracaoPedido(unitOfWork);

                servicoAlteracaoPedido.Adicionar(alteracaoPedido, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar o pedido");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> RemoverPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Servicos.Log.TratarErro("RemoverPedido - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if ((cargasPedidos == null) || (cargasPedidos.Count == 0))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível encontrar nenhum pedido com os protocolos informados");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();


                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (cargaPedido.Pedido.Container != null)
                            throw new WebServiceException("O pedido selecionado já possui container vinculado.");

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, removerPedido: false);

                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repositorioCargaPedido.BuscarPedidosPorCarga(carga.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCarga in pedidosCarga)
                        {
                            pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking - 1;

                            if (pedidoCarga.QuantidadeContainerBooking < 0)
                                pedidoCarga.QuantidadeContainerBooking = 0;

                            repositorioPedido.Atualizar(pedidoCarga);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Excluiu pedido vinculado por Integração.", unitOfWork);
                    }
                    else
                    {
                        bool permitirRemoverTodos = !configuracaoGeralCarga.NaoPermitirRemoverUltimoPedidoCarga;

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, null, permitirRemoverTodos, false, configuracaoWebService.NaoRecalcularFreteAoAdicionarRemoverPedido);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Removeu o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} via integração", unitOfWork);

                        if (permitirRemoverTodos && !repositorioCargaPedido.PossuiCargaPedidoPorCarga(carga.Codigo))//Se era o último carga pedido, solicita o cancelamento da carga
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                            {
                                Carga = carga,
                                MotivoCancelamento = "Cancelamento por remoção do último pedido via integração",
                                TipoServicoMultisoftware = TipoServicoMultisoftware,
                                Usuario = Auditado.Usuario
                            };

                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoEmbarcador, unitOfWork);
                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware);

                            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                                throw new WebServiceException(cargaCancelamento.MensagemRejeicaoCancelamento);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, "Adicionou o cancelamento da Carga ao remover o seu último pedido via integração", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Adicionou o cancelamento da Carga ao remover o seu último pedido via integração", unitOfWork);
                        }
                        else
                        {
                            if (!(cargaPedido.Carga.TipoOperacao?.NaoIntegrarOpentech ?? false) && !(cargaPedido.Carga.Veiculo?.NaoIntegrarOpentech ?? false))
                                cargaPedido.Carga.AguardarIntegracaoEtapaTransportador = Servicos.WebService.NFe.NotaFiscal.AdicionarIntegracaoSM(cargaPedido.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, configuracaoEmbarcador.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, cargaPedido.Carga.CargaEmitidaParcialmente, unitOfWork);

                            carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                            if (carga.SituacaoCarga == SituacaoCarga.AgNFe && carga.ExigeNotaFiscalParaCalcularFrete)
                                carga.ProcessandoDocumentosFiscais = true;

                            repositorioCarga.Atualizar(carga);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover o pedido da carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscarTiposOperacoesPendentesIntegracao(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarTiposOperacoesPendentesIntegracao(quantidade ?? 0));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoTiposOperacoes(List<int> listaProtocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoTiposOperacoes(listaProtocolos));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarPedidoPorProtocolo(int protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarPedidoService(protocolo));
            });
        }

        public Retorno<RetornoPacote> EnviarPacote(Dominio.ObjetosDeValor.WebService.Rest.Pedidos.Pacote pacote)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarPacoteAsync(pacote).GetAwaiter().GetResult());
            });
        }

        public Retorno<bool> AjustarDatasDoPedido(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AjustarDatasDoPedido(atualizarDatasPedido));
            });
        }

        public Retorno<bool> AtualizarDataUltimaGeracao(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarDataUltimaGeracao(atualizarDatasPedido));
            });
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>> ConsultarPedidosPorNotaFiscal(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (!Utilidades.Validate.ValidarChaveNFe(notaFiscal?.Chave))
                    throw new WebServiceException("Chave inválida ou não informada!");

                Servicos.WebService.Carga.Pedido serPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repNota.BuscarPedidosPorChaveNFe(notaFiscal.Chave);
                List<Dominio.ObjetosDeValor.WebService.Carga.Pedido> pedidosRetornar = serPedido.ConverterPedidos(pedidos);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoSucesso(pedidosRetornar);
            }
            catch (WebServiceException ex)
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoDadosInvalidos(ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os pedidos!");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>> ObterAgendamentos(Dominio.ObjetosDeValor.WebService.Pedido.ObterAgendamentos obterAgendamentos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ObterAgendamentos(obterAgendamentos));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> AlterarSituacaoComercialPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlterarSituacaoComercialPedido alterarSituacaoComercialPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AlterarSituacaoComercialPedido(alterarSituacaoComercialPedido));
            });
        }

        public Retorno<bool> AtualizarPedidoObservacaoCte(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarPedidoObservacaoCte atualizarPedidoObservacaoCte)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarPedidoObservacaoCte(atualizarPedidoObservacaoCte));
            });
        }

        public Retorno<bool> AtualizarValorFrete(Dominio.ObjetosDeValor.WebService.Carga.Protocolo protocolo, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFrete, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFreteFilialEmissora)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pedido.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AtualizarValorFrete(protocolo, valorFrete, valorFreteFilialEmissora));
            });
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos;
        }

        #endregion
    }
}
