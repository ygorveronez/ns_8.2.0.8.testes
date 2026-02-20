using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public sealed class RetiradaProduto
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultiSoftware;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly bool _cadastrarDadosCarregamento;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private string _urlAcessoCliente;

        #endregion Atributos

        #region Construtores

        public RetiradaProduto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultiSoftware = clienteMultiSoftware;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _auditado = auditado;
            _cadastrarDadosCarregamento = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        public RetiradaProduto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string urlAcessoCliente)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultiSoftware = clienteMultiSoftware;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _auditado = auditado;
            _cadastrarDadosCarregamento = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            _urlAcessoCliente = urlAcessoCliente;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento PreencherCarregamentoIntegracao(Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Dominio.Entidades.Empresa transportador = ObterTransportadorCarregamentoIntegracao(carregamentoIntegracao);
            string nomeTransportador = !transportador.EmpresaRetiradaProduto ? transportador?.RazaoSocial : carregamentoIntegracao?.Transportador?.RazaoSocial ?? string.Empty;

            Dominio.Entidades.Usuario motorista = ObterMotoristaCarregamentoIntegracao(carregamentoIntegracao, transportador);
            Dominio.Entidades.Veiculo veiculo = ObterVeiculoCarregamentoIntegracao(carregamentoIntegracao, transportador, motorista);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
            if (!string.IsNullOrWhiteSpace(carregamentoIntegracao.TipoOperacao?.CodigoIntegracao))
                tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(carregamentoIntegracao.TipoOperacao?.CodigoIntegracao);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
            if (!string.IsNullOrWhiteSpace(carregamentoIntegracao.Filial?.CodigoIntegracao))
                filial = repFilial.buscarPorCodigoEmbarcador(carregamentoIntegracao.Filial?.CodigoIntegracao);

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(carregamentoIntegracao.TipoCarga?.CodigoIntegracao);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao(carregamentoIntegracao.ModeloVeicular?.CodigoIntegracao);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento()
            {
                DataCriacao = DateTime.Now,
                EncaixarHorario = configuracaoJanelaCarregamento.EncaixarHorarioRetiradaProduto,
                Empresa = transportador,
                DataCarregamentoCarga = carregamentoIntegracao.DataHoraEnxcaixeConvertido,
                Motoristas = motorista != null ? new List<Dominio.Entidades.Usuario>() { motorista } : new List<Dominio.Entidades.Usuario>(),
                Veiculo = veiculo,
                TipoOperacao = tipoOperacao,
                Filial = filial,
                TipoDeCarga = tipoCarga,
                ModeloVeicularCarga = modeloVeicular,
                PesoCarregamento = 0m,
                NomeTransportadora = nomeTransportador,
                PlacaVeiculo = veiculo?.Placa ?? "",
                AutoSequenciaNumero = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork).ObterProximoCodigoCarregamento(),
                CarregamentoIntegradoERP = true
            };

            carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();

            repCarregamento.Inserir(carregamento);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = InserirPedidosIntegracao(carregamento, carregamentoIntegracao);

            repCarregamento.Atualizar(carregamento);

            GerarRoteirizacaoObrigatoriaCarregamento(carregamento, carregamentoPedidos);

            return carregamento;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> GerarCarga(int codigoCarregamento)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork, _configuracaoEmbarcador);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>() {
                carregamento.Filial
            };

            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
            {
                MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Sim,
                GeradoViaWs = true,
            };

            servicoMontagemCarga.GerarCarga(carregamento, filiais, carregamentoPedidos, _tipoServicoMultisoftware, _clienteMultiSoftware, _auditado, propriedades, _urlAcessoCliente);

            return repCarga.BuscarCargasPorCarregamento(carregamento.Codigo);
        }

        public void GerarRoteirizacaoObrigatoriaCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos)
        {
            //#44400
            if ((_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && _configuracaoEmbarcador.RoteirizacaoObrigatoriaMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.APIGoogle);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

                bool roteirizacaoOpcionalTipoOperacao = false;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carregamento.TipoOperacao;
                if (tipoOperacao == null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = (from o in carregamentoPedidos
                                                                                              where o.Pedido?.TipoOperacao != null
                                                                                              select o.Pedido?.TipoOperacao
                                                                                             ).Distinct().ToList();

                    roteirizacaoOpcionalTipoOperacao = tiposOperacoes.Any(x => x.NaoExigeRoteirizacaoMontagemCarga);
                }
                else
                    roteirizacaoOpcionalTipoOperacao = tipoOperacao.NaoExigeRoteirizacaoMontagemCarga;

                if (carregamentoRoteirizacao == null && tipoIntegracao != null && !roteirizacaoOpcionalTipoOperacao)
                {
                    // Aki devemos efetuar a roteirização dos pedidos...
                    Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork).Buscar();
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork).BuscarPorFilial(carregamento?.Filial?.Codigo ?? 0);

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from ped in carregamentoPedidos
                                                                                 select ped.Pedido).ToList();

                    Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Carga.MontagemCarga.MontagemCarga(_unitOfWork, _configuracaoEmbarcador, _unitOfWork.StringConexao);
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPonto = servicoMontagemCarga.ObterTipoUltimoPontoRoteirizacao(pedidos);
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = servicoMontagemCarga.RoteirizarPedidos(pedidos, carregamento.ModeloVeicularCarga, configuracaoIntegracao.ServidorRouteOSM, tipoUltimoPonto, centroCarregamento, carregamento, true, false);

                    servicoMontagemCarga.GerarRotaCarregamento(carregamento, respostaRoteirizacao, tipoUltimoPonto, carregamento.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private Dominio.Entidades.Usuario ObterMotoristaCarregamentoIntegracao(Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Dominio.Entidades.Usuario motorista = null;

            if (string.IsNullOrWhiteSpace(carregamentoIntegracao.Motorista?.CodigoIntegracao) && !_cadastrarDadosCarregamento)
                return null;

            if (!string.IsNullOrWhiteSpace(carregamentoIntegracao.Motorista?.CodigoIntegracao))
                motorista = repUsuario.BuscarPorCodigoIntegracao(carregamentoIntegracao.Motorista.CodigoIntegracao);

            if (motorista != null)
                return motorista;

            motorista = repUsuario.BuscarPorCPF(carregamentoIntegracao.Motorista?.CPF);

            if (motorista != null)
                return motorista;

            if (!_cadastrarDadosCarregamento)
                throw new ServicoException("Não foi possível encontrar o motorista");

            motorista = Servicos.Usuario.PreencherMotoristaGenerico(carregamentoIntegracao.Motorista.Nome, empresa, _unitOfWork);
            motorista.CPF = carregamentoIntegracao?.Motorista?.CPF ?? "11111111111";
            motorista.Status = "A";
            repUsuario.Atualizar(motorista);

            return motorista;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoCarregamentoIntegracao(Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Usuario motorista)
        {
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            string mensagemCadastroVeiculo = string.Empty;
            if (string.IsNullOrWhiteSpace(carregamentoIntegracao.Veiculo?.Placa))
                return null;

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(carregamentoIntegracao.Veiculo?.Placa);
            if (veiculo == null && _cadastrarDadosCarregamento)
                veiculo = Servicos.Veiculo.PreencherVeiculoGenerico(carregamentoIntegracao.Veiculo?.Placa, empresa, _unitOfWork);

            if (veiculo == null)
                veiculo = serWSVeiculo.SalvarVeiculo(carregamentoIntegracao.Veiculo, null, false, ref mensagemCadastroVeiculo, _unitOfWork, _tipoServicoMultisoftware, _auditado, true, motorista?.CPF);
            else if (veiculo != null && carregamentoIntegracao.Veiculo.Reboques != null && carregamentoIntegracao.Veiculo.Reboques.Count > 0)
            {
                if (veiculo.VeiculosVinculados != null)
                    veiculo.VeiculosVinculados.Clear();

                if (carregamentoIntegracao.Veiculo.Reboques != null)
                {
                    string mensagem = string.Empty;
                    veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                    foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in carregamentoIntegracao.Veiculo.Reboques)
                    {
                        Dominio.Entidades.Veiculo reboque = serWSVeiculo.SalvarVeiculo(reboqueIntegracao, null, false, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado);
                        if (reboque != null)
                            veiculo.VeiculosVinculados.Add(reboque);
                    }

                    if (!string.IsNullOrWhiteSpace(mensagem))
                        throw new ServicoException("Não foi possível cadastrar o reboque do veículo. " + mensagem);

                    repVeiculo.Atualizar(veiculo);
                }
            }

            if (veiculo == null && !string.IsNullOrWhiteSpace(mensagemCadastroVeiculo))
                throw new ServicoException("Não foi possível encontrar o veículo. " + mensagemCadastroVeiculo);

            return veiculo;
        }

        private Dominio.Entidades.Empresa ObterTransportadorCarregamentoIntegracao(Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Empresa transportador = repEmpresa.BuscarEmpresaPorCNPJ(carregamentoIntegracao.Transportador?.CNPJ);
            if (transportador == null)
            {
                if (!_cadastrarDadosCarregamento)
                    throw new ServicoException($"Não foi encontrado a transportadora com CNPJ {carregamentoIntegracao.Transportador?.CNPJ}");

                transportador = repEmpresa.BuscarEmpresaPadraoRetirada();
            }

            return transportador;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> InserirPedidosIntegracao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            if (carregamentoIntegracao.Pedidos == null || carregamentoIntegracao.Pedidos.Count == 0)
                throw new ServicoException($"Nenhum pedido informado");

            foreach (Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Pedido pedidoIntegracao in carregamentoIntegracao.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                if (pedidoIntegracao.CodigoIntegracao > 0)
                    pedido = repositorioPedido.BuscarPorCodigo(pedidoIntegracao.CodigoIntegracao);
                //if (pedido == null && pedidoIntegracao.IdLote > 0 && pedidoIntegracao.IdProposta > 0)
                //    pedido = repositorioPedido.BuscarPorIntegracaoTrizy(pedidoIntegracao.IdProposta, pedidoIntegracao.IdLote);
                if (pedido == null && pedidoIntegracao.IdLote > 0)
                    pedido = repositorioPedido.BuscarPorIdLoteTrizy(pedidoIntegracao.IdLote);
                //if (pedido == null && pedidoIntegracao.IdProposta > 0)
                //    pedido = repositorioPedido.BuscarPorIdPropostaTrizy(pedidoIntegracao.IdProposta);

                if (pedido == null)
                    throw new ServicoException($"Não foi possível encontrar o pedido {pedidoIntegracao.CodigoIntegracao}");

                if (pedidoIntegracao.Produtos == null || pedidoIntegracao.Produtos.Count == 0)
                    throw new ServicoException($"Nenhum produto informado");

                if (pedidoIntegracao.IdProposta > 0)
                {
                    carregamento.IDPropostaTrizy = pedidoIntegracao.IdProposta;
                    //pedido.IDPropostaTrizy = pedidoIntegracao.IdProposta;
                    repositorioPedido.Atualizar(pedido);
                }

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = repositorioCarregamentoPedido.BuscarPorCarregamentoEPedido(pedido.Codigo, carregamento.Codigo);

                if (carregamentoPedido == null)
                {
                    carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido
                    {
                        Pedido = pedido,
                        Carregamento = carregamento,
                        Peso = 0m
                    };
                    repositorioCarregamentoPedido.Inserir(carregamentoPedido);
                }
                else if ((pedidoIntegracao?.Produtos?.Count ?? 0) > 0)
                    carregamentoPedido.Peso = 0;

                // Vamos buscar o saldo do pedido, para validar a quantidade...
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, _unitOfWork);

                foreach (Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Produto produtoIntegracao in pedidoIntegracao.Produtos)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoIntegracao.CodigoIntegracao);

                    if (produto == null)
                        throw new ServicoException($"Não foi possível encontrar o produto {produtoIntegracao.CodigoIntegracao}");

                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repositorioPedidoProduto.BuscarPorPedidoProduto(pedido.Codigo, produto.Codigo);

                    if (pedidoProduto == null)
                        throw new ServicoException($"O produto {produtoIntegracao.CodigoIntegracao} não pertence ao pedido {pedidoIntegracao.CodigoIntegracao}");

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = repositorioCarregamentoPedidoProduto.BuscaPorCarregamentoPedidoProduto(carregamentoPedido.Codigo, pedidoProduto.Codigo);

                    if (carregamentoPedidoProduto == null)
                    {
                        carregamentoPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto
                        {
                            CarregamentoPedido = carregamentoPedido,
                            PedidoProduto = pedidoProduto
                        };
                        repositorioCarregamentoPedidoProduto.Inserir(carregamentoPedidoProduto);
                    }

                    carregamentoPedidoProduto.Peso = produtoIntegracao.Peso;
                    carregamentoPedidoProduto.Quantidade = produtoIntegracao.Quantidade;

                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProduto = (from obj in saldoProdutos where obj.CodigoProduto == pedidoProduto.Produto.Codigo select obj).ToList();
                    if (carregamentoPedidoProduto.Quantidade > saldoProduto.Sum(s => s.SaldoQtde))
                        throw new ServicoException(string.Format("4 - A quantidade {0} do pedido produto {1} não pode ser maior que o saldo {2} de quantidade do pedido produto.", carregamentoPedidoProduto.Quantidade, pedidoProduto.Produto.CodigoProdutoEmbarcador, saldoProduto.Sum(s => s.SaldoQtde)));

                    if (carregamentoPedidoProduto.Peso == 0 && carregamentoPedidoProduto.PedidoProduto.Quantidade > 0)
                        carregamentoPedidoProduto.Peso = (carregamentoPedidoProduto.PedidoProduto.PesoTotal / carregamentoPedidoProduto.PedidoProduto.Quantidade) * carregamentoPedidoProduto.Quantidade;

                    if (carregamentoPedidoProduto.QuantidadePallet == 0 && carregamentoPedidoProduto.PedidoProduto.Quantidade > 0)
                        carregamentoPedidoProduto.QuantidadePallet = (carregamentoPedidoProduto.PedidoProduto.QuantidadePalet / carregamentoPedidoProduto.PedidoProduto.Quantidade) * carregamentoPedidoProduto.Quantidade;

                    if (carregamentoPedidoProduto.MetroCubico == 0 && carregamentoPedidoProduto.PedidoProduto.Quantidade > 0)
                        carregamentoPedidoProduto.MetroCubico = (carregamentoPedidoProduto.PedidoProduto.MetroCubico / carregamentoPedidoProduto.PedidoProduto.Quantidade) * carregamentoPedidoProduto.Quantidade;

                    repositorioCarregamentoPedidoProduto.Atualizar(carregamentoPedidoProduto);

                    //if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    //{
                    carregamentoPedido.Peso += carregamentoPedidoProduto.Peso;
                    carregamentoPedido.Pallet += carregamentoPedidoProduto.QuantidadePallet;
                    carregamento.PesoCarregamento += carregamentoPedidoProduto.Peso;
                    //}
                }

                repositorioCarregamentoPedido.Atualizar(carregamentoPedido);

                if (carregamento.Filial == null)
                    carregamento.Filial = pedido.Filial;

                if (carregamento.TipoOperacao == null)
                    carregamento.TipoOperacao = pedido.TipoOperacao;

                carregamentoPedidos.Add(carregamentoPedido);
            }

            return carregamentoPedidos;
        }

        #endregion
    }
}
