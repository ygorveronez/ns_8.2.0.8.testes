using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.WebService.Carga
{
    public class Pedido
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;

        #endregion Atributos

        #region Construtores

        public Pedido(Repositorio.UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.WebService.Carga.Pedido> ConverterPedidos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Pedido> objetoPedidos = new List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>();
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new Servicos.WebService.Pessoas.Pessoa();
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                Dominio.ObjetosDeValor.WebService.Carga.Pedido pedidoIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.Pedido()
                {
                    Destinatario = svcPessoa.ConverterObjetoPessoa(pedido.Destinatario),
                    Expedidor = svcPessoa.ConverterObjetoPessoa(pedido.Expedidor),
                    Recebedor = svcPessoa.ConverterObjetoPessoa(pedido.Recebedor),
                    Remetente = svcPessoa.ConverterObjetoPessoa(pedido.Remetente),
                    Tomador = svcPessoa.ConverterObjetoPessoa(pedido.Tomador),
                    NumeroPedido = pedido.NumeroPedidoEmbarcador,
                    Protocolo = pedido.Codigo,
                    TipoPagamento = pedido.TipoPagamento,
                    TipoTomador = pedido.TipoTomador,
                    PedidoCancelado = (pedido.SituacaoPedido == SituacaoPedido.Cancelado || pedido.SituacaoPedido == SituacaoPedido.EmCotacao),
                    NotasFiscais = new List<Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal>(),
                    CanalEntrega = pedido.CanalEntrega != null ? new Dominio.ObjetosDeValor.WebService.Carga.CanalEntrega { Descricao = pedido.CanalEntrega.Descricao, CodigoIntegracao = pedido.CanalEntrega.CodigoIntegracao } : null
                };

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                {
                    notasFiscais.AddRange(repNotaFiscal.BuscarPorCarga(cargaPedido.Carga.Codigo));
                }

                if (pedido.NotasFiscais.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal in pedido.NotasFiscais) //todo: melhorar performance, seria interessante retornar todas a notas e localizar via link assim estamos fazendo uma conexão com o banco a cada pedido para buscar suas notas.
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal notaFiscalIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal()
                        {
                            Chave = xMLNotaFiscal.Chave,
                            DataEmissao = xMLNotaFiscal.DataEmissao,
                            Destinatario = svcPessoa.ConverterObjetoPessoa(pedido.Destinatario, pedido.EnderecoDestino),
                            Emitente = svcPessoa.ConverterObjetoPessoa(pedido.Remetente, pedido.EnderecoOrigem),
                            Numero = xMLNotaFiscal.Numero,
                            Serie = xMLNotaFiscal.Serie,
                            TipoOperacaoNotaFiscal = HelperTipoOperacaoNotaFiscal.getTipoOperacaoNotaFiscal((int)xMLNotaFiscal.TipoOperacaoNotaFiscal),
                            Peso = xMLNotaFiscal.Peso,
                            Valor = xMLNotaFiscal.Valor,
                            Volumes = xMLNotaFiscal.Volumes,
                            Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>()
                        };

                        pedidoIntegracao.NotasFiscais.Add(notaFiscalIntegracao);
                    }
                }
                else if (notasFiscais.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal notaFiscalIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal()
                        {
                            Chave = notaFiscal.Chave,
                            DataEmissao = notaFiscal.DataEmissao,
                            Destinatario = svcPessoa.ConverterObjetoPessoa(pedido.Destinatario, pedido.EnderecoDestino),
                            Emitente = svcPessoa.ConverterObjetoPessoa(pedido.Remetente, pedido.EnderecoOrigem),
                            Numero = notaFiscal.Numero,
                            Serie = notaFiscal.Serie,
                            TipoOperacaoNotaFiscal = HelperTipoOperacaoNotaFiscal.getTipoOperacaoNotaFiscal((int)notaFiscal.TipoOperacaoNotaFiscal),
                            Peso = notaFiscal.Peso,
                            Valor = notaFiscal.Valor,
                            ValorProdutos = notaFiscal.Valor,
                            Volumes = notaFiscal.Volumes,
                            Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>()
                        };

                        pedidoIntegracao.NotasFiscais.Add(notaFiscalIntegracao);
                    }
                }
                objetoPedidos.Add(pedidoIntegracao);
            }

            return objetoPedidos;
        }

        public void AtualizarParticipantesPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, ref Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
        {
            InformarParticipantesPedido(ref pedido, ref cargaIntegracao, ref mensagemErro, tipoServicoMultisoftware, unitOfWorkAdmin, false, Auditado);
            InformarEnderecoPedido(ref pedido, cargaIntegracao, ref mensagemErro, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido AtualizarPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool encerrarMDFeAutomaticamente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = null)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(_unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(_unitOfWork, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            if (protocolo.protocoloIntegracaoCarga > 0)
                carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

            RemoverDadosEssencaisDoPedido(ref cargaIntegracao);

            if (carga != null && (configuracaoDadosTransporte?.ExigirQueApolicePropriaTransportadorEstejaValida ?? false))
                servicoJanelaCarregamentoTransportadorValidacoes.ValidarApoliceSeguro(carga.Codigo, carga.Empresa, null, configuracaoDadosTransporte);

            if (carga != null && string.IsNullOrEmpty(cargaIntegracao.NumeroControlePedido))
            {

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware) && carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn)
                {
                    stMensagem.Append("A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que ela seja modificada; ");
                }

                if (carga.ProcessandoDocumentosFiscais && SituacaoCarga.AgNFe == carga.SituacaoCarga)
                {
                    stMensagem.Append("A atual situação da carga (Processando Documentos Fiscais) não permite que ela seja modificada; ");
                }
            }

            if (protocolo.protocoloIntegracaoPedido > 0)
            {
                //pedido = repPedido.BuscarPorCodigo(protocolo.protocoloIntegracaoPedido);
                pedido = repPedido.BuscarPorProtocolo(protocolo.protocoloIntegracaoPedido);

                if (pedido != null)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    {
                        if (pedido.NumeroPedidoEmbarcador != cargaIntegracao.NumeroPedidoEmbarcador)
                        {
                            if (string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPedidoEmbarcador))
                            {
                                stMensagem.Append("Número do pedido embarcador não informado; ");
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExiste = repPedido.BuscarPorNumeroEmbarcador(cargaIntegracao.NumeroPedidoEmbarcador, filial?.Codigo ?? 0, "", cargaIntegracao.CargaDePreCarga, ignorarPedidosInseridosManualmente: false);
                                if (pedidoExiste != null)
                                {
                                    stMensagem.Append("O número do pedido informado para alteração já foi enviado em outro pedido para a Multisoftware;");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!cargaIntegracao.NaoAtualizarDadosDoPedido)
                            pedido.NumeroPedidoEmbarcador = cargaIntegracao.NumeroPedidoEmbarcador;

                    }
                }
            }
            else if (protocolo.protocoloIntegracaoCarga > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(protocolo.protocoloIntegracaoCarga);
                if (cargaPedidos.Count == 1)
                    pedido = cargaPedidos.FirstOrDefault().Pedido;
            }

            if (pedido != null)
            {
                pedido.Initialize();

                if (stMensagem.Length == 0 && pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto || (cargaIntegracao.ViagemJaFoiFinalizada && pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado))
                {
                    pedido.PedidoIntegradoEmbarcador = true;

                    if (!cargaIntegracao.NaoAtualizarDadosDoPedido)
                    {
                        InformarParticipantesPedido(ref pedido, ref cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, unitOfWorkAdmin);
                        InformarEnderecoPedido(ref pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware);
                    }
                    pedido.NumeroControle = cargaIntegracao.NumeroControlePedido;
                    InformarDadosPedidoEmbarcador(ref pedido, tipoOperacao, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, true, _unitOfWork.StringConexao, Auditado, encerrarMDFeAutomaticamente, configuracao, configuracaoWebService, null, "");

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !cargaIntegracao.NaoAtualizarDadosDoPedido && !string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPedidoEmbarcador))
                        pedido.NumeroPedidoEmbarcador = cargaIntegracao.NumeroPedidoEmbarcador;

                    InformarRotaFretePedido(pedido, configuracaoWebService);
                    PreencherDadosTransporteMaritimo(pedido, cargaIntegracao, stMensagem, tipoServicoMultisoftware, unitOfWorkAdmin);
                    InformarFilialPedido(ref pedido, filial, cargaIntegracao, tipoServicoMultisoftware, ref stMensagem);
                    InformarValorFretePedido(ref pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware);
                    VincularAdicionaisPedido(ref pedido, cargaIntegracao, configuracaoWebService, Auditado);
                    ValidarCalculoFreteStagePedido(pedido, carga, configuracao, tipoServicoMultisoftware);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoTransbordo.BuscarPorPedido(pedido.Codigo);
                    if (transbordos != null && transbordos.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo transbordo in transbordos)
                        {
                            repPedidoTransbordo.Deletar(transbordo);
                        }
                    }

                    if (stMensagem.Length == 0)
                    {
                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) //produto só é obrigatório no Multiembarcador
                        {
                            bool naoTemProdutos = cargaIntegracao.Produtos == null || cargaIntegracao.Produtos.Count <= 0;
                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();

                            if (naoTemProdutos && configuracaoCargaIntegracao?.AceitarPedidosComPendenciasDeProdutos != true)
                                stMensagem.Append("É obrigatório informar os produtos para o pedido;");
                            else
                                AtualizarProdutoPedidos(cargaIntegracao, pedido, configuracao, configuracaoCargaIntegracao, ref stMensagem, Auditado);
                        }

                        if (stMensagem.Length <= 0)
                        {
                            pedido.UltimaAtualizacao = DateTime.Now;
                            repPedido.Atualizar(pedido);
                        }
                    }
                }
                else if (stMensagem.Length == 0)
                {
                    stMensagem.Append("A atual situação do pedido (" + pedido.DescricaoSituacaoPedido + ") não permite que ele seja modificado; ");
                }
            }
            else
            {
                stMensagem.Append("Nâo foi localizado um pedido para o protocolo informado (" + protocolo.protocoloIntegracaoPedido + "). ");
            }
            return pedido;
        }

        public void AtualizarTipoOperacao(ref Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao tipoOperacaoIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (tipoOperacaoIntegracao == null || tipoOperacao == null)
                return;

            if (!tipoOperacaoIntegracao.Atualizar)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            if (auditado != null)
                tipoOperacao.Initialize();

            tipoOperacao.Ativo = true;
            tipoOperacao.CodigoIntegracao = tipoOperacaoIntegracao.CodigoIntegracao;
            tipoOperacao.Descricao = tipoOperacaoIntegracao.Descricao;
            tipoOperacao.BloquearEmissaoDeEntidadeSemCadastro = tipoOperacaoIntegracao.BloquearEmissaoDeEntidadeSemCadastro;
            tipoOperacao.BloquearEmissaoDosDestinatario = tipoOperacaoIntegracao.BloquearEmissaoDosDestinatario;
            tipoOperacao.TipoCobrancaMultimodal = tipoOperacaoIntegracao.TipoCobrancaMultimodal;
            tipoOperacao.ModalPropostaMultimodal = tipoOperacaoIntegracao.ModalPropostaMultimodal;
            tipoOperacao.TipoServicoMultimodal = tipoOperacaoIntegracao.TipoServicoMultimodal;
            tipoOperacao.TipoPropostaMultimodal = tipoOperacaoIntegracao.TipoPropostaMultimodal;
            tipoOperacao.PermiteGerarPedidoSemDestinatario = true;
            tipoOperacao.UsarConfiguracaoEmissao = true;

            if (auditado != null)
                repTipoOperacao.Atualizar(tipoOperacao, auditado);
            else
                repTipoOperacao.Atualizar(tipoOperacao);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido CriarPedido(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref int protocoloPedidoExistente, ref int protocoloCargaExistente, bool buscarCargaPorTransportador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = null, string adminStringConexao = "", bool ignorarPedidosInseridosManualmente = false, bool integracaoViaWS = false, bool notificarAcompanhamento = true, bool incrementarSequencial = true, bool forcarGerarNovoPedido = false)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = !string.IsNullOrWhiteSpace(adminStringConexao) ? new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao) : null;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repositorioPassagemPercursoEstado = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new(_unitOfWork);

            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);
            Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes servicoJanelaCarregamentoTransportadorValidacoes = new Servicos.Embarcador.Logistica.JanelaCarregamentoTransportadorValidacoes(_unitOfWork, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            int codigoFilial = filial?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = null;

            if (!forcarGerarNovoPedido)
            {
                if ((configuracaoWebService?.BloquearInclusaoCargaComMesmoNumeroPedidoEmbarcador ?? false) && repositorioPedido.VerificarExistenciaPedido(cargaIntegracao.NumeroPedidoEmbarcador))
                    if (integracaoViaWS && configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                    {
                        protocoloPedidoExistente = repositorioPedido.BuscarProtocoloPorNumeroPedido(cargaIntegracao.NumeroPedidoEmbarcador);
                        stMensagem.Append("Esse pedido já existe na Base MultiSoftware, não sendo possível adicioná-lo novamente.");
                        if (unitOfWorkAdmin != null)
                            unitOfWorkAdmin.Dispose();
                        return null;
                    }
                    else
                    {
                        if (unitOfWorkAdmin != null)
                            unitOfWorkAdmin.Dispose();
                        throw new ServicoException("Esse pedido já existe na Base MultiSoftware, não sendo possível adicioná-lo novamente.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);
                    }

                if (cargaIntegracao.ProtocoloCotacao > 0)
                    cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(cargaIntegracao.ProtocoloCotacao);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    string cnpjEmpresa = string.Empty;
                    if (cargaIntegracao.TransportadoraEmitente != null && !string.IsNullOrWhiteSpace(cargaIntegracao.TransportadoraEmitente.CNPJ) && buscarCargaPorTransportador)
                        cnpjEmpresa = cargaIntegracao.TransportadoraEmitente.CNPJ;

                    if (string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPedidoEmbarcador))
                        stMensagem.Append("Número do pedido embarcador não informado; ");
                    else
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExiste;

                        if (!(configuracaoTMS?.TrocarPreCargaPorCarga ?? true) || (configuracaoTMS?.UtilizarProtocoloDaPreCargaNaCarga ?? false))
                            pedidoExiste = repositorioPedido.BuscarPorNumeroEmbarcador(cargaIntegracao.NumeroPedidoEmbarcador, codigoFilial, string.Empty, null, ignorarPedidosInseridosManualmente && !configuracaoGeralCarga.RetornarPedidosInseridosManualmenteAoGerarCarga);
                        else if (tipoOperacao != null && tipoOperacao.UtilizarExpedidorComoTransportador && !tipoOperacao.OperacaoDeRedespacho)// quando a operação é por expedição pode usar o mesmo codigo de carga e pedido desde que o transportador (expedidor) seja diferente.
                        {
                            string cnpjExpedidor = "";

                            if (cargaIntegracao.TransportadoraEmitente != null)
                                cnpjExpedidor = cargaIntegracao.TransportadoraEmitente.CNPJ;
                            else if (cargaIntegracao.Expedidor != null)
                                cnpjExpedidor = cargaIntegracao.Expedidor.CPFCNPJSemFormato;

                            pedidoExiste = repositorioPedido.buscarPorNumeroEmbarcadorExpedidor(cargaIntegracao.NumeroPedidoEmbarcador, codigoFilial, cnpjExpedidor, cargaIntegracao.CargaDePreCarga);
                        }
                        else
                            pedidoExiste = repositorioPedido.BuscarPorNumeroEmbarcador(cargaIntegracao.NumeroPedidoEmbarcador, codigoFilial, cnpjEmpresa, cargaIntegracao.CargaDePreCarga, ignorarPedidosInseridosManualmente && !configuracaoGeralCarga.RetornarPedidosInseridosManualmenteAoGerarCarga);


                        if ((pedidoExiste != null && pedidoExiste.Protocolo > 0) && PermitirUtilizarPedidoExistente(pedidoExiste, cargaIntegracao, configuracaoTMS))
                        {
                            protocoloPedidoExistente = pedidoExiste.Protocolo;

                            if (configuracaoTMS?.AtualizarPedidoPorIntegracao ?? false)
                            {
                                Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
                                {
                                    protocoloIntegracaoPedido = protocoloPedidoExistente,
                                };

                                AtualizarPedido(protocolo, filial, tipoOperacao, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, auditado, configuracaoTMS.EncerrarMDFeAutomaticamente, configuracaoTMS, configuracaoWebService, unitOfWorkAdmin);
                            }

                            pedidoExiste.NumeroControle = cargaIntegracao.NumeroControlePedido;
                            pedidoExiste.AguardandoIntegracao = true;

                            if (unitOfWorkAdmin != null)
                                unitOfWorkAdmin.Dispose();

                            return pedidoExiste;
                        }
                    }
                }
            }

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
            pedido.AguardandoIntegracao = true;
            pedido.PedidoRestricaoData = cargaIntegracao.PedidoComRestricaoData;
            pedido.CodigoRastreamento = Guid.NewGuid().ToString().Replace("-", "");

            pedido.PossuiCargaPerigosa = cargaIntegracao.ContemCargaPerigosa;
            pedido.ImprimirObservacaoCTe = cargaIntegracao.ImprimirObservacaoCTe;

            if (forcarGerarNovoPedido)
                pedido.ControleNumeracao = new Random().Next(100000, 1000000 + 1);

            if (cargaIntegracao.PesoBruto > 0)
                pedido.PesoTotal = cargaIntegracao.PesoBruto;

            if (stMensagem.Length <= 0)
            {
                InformarParticipantesPedido(ref pedido, ref cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, unitOfWorkAdmin, integracaoViaWS, auditado);

                if (stMensagem.Length <= 0)
                {
                    InformarDadosPedidoEmbarcador(ref pedido, tipoOperacao, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, false, _unitOfWork.StringConexao, auditado, configuracaoTMS?.EncerrarMDFeAutomaticamente ?? false, configuracaoTMS, configuracaoWebService, clienteAcesso, adminStringConexao, incrementarSequencial);

                    if (!string.IsNullOrWhiteSpace(configuracaoTMS?.ObservacaoCTePadraoEmbarcador))
                    {
                        if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                            pedido.ObservacaoCTe = configuracaoTMS.ObservacaoCTePadraoEmbarcador;
                        else
                            pedido.ObservacaoCTe = string.Concat(pedido.ObservacaoCTe, " / ", configuracaoTMS.ObservacaoCTePadraoEmbarcador);
                    }

                    if (tipoOperacao?.UsarConfiguracaoEmissao ?? false)
                    {
                        if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                            pedido.ObservacaoCTe = tipoOperacao.ObservacaoCTe;
                        else
                            pedido.ObservacaoCTe = string.Concat(pedido.ObservacaoCTe, " / ", tipoOperacao.ObservacaoCTe);
                    }

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (!configuracaoTMS?.PermitirEnviarNumeroPedidoEmbarcadorViaIntegracao ?? false)
                        {
                            if (!cargaIntegracao.NaoAtualizarDadosDoPedido)
                                pedido.NumeroPedidoEmbarcador = string.Empty;
                        }
                    }

                    if (cargaIntegracao.ViagemJaFoiFinalizada)
                        pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado;
                    else
                        pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;

                    pedido.PedidoIntegradoEmbarcador = true;

                    if (stMensagem.Length == 0)
                    {
                        InformarFilialPedido(ref pedido, filial, cargaIntegracao, tipoServicoMultisoftware, ref stMensagem);

                        if (stMensagem.Length == 0)
                            InformarEnderecoPedido(ref pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware);

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) //produto só é obrigatório no Multiembarcador
                        {
                            bool naoTemProdutos = (cargaIntegracao.Produtos == null || cargaIntegracao.Produtos.Count <= 0) && cargaIntegracao.ProdutoPredominante == null;
                            if (naoTemProdutos && configuracaoCargaIntegracao?.AceitarPedidosComPendenciasDeProdutos != true)
                            {
                                stMensagem.Append("É obrigatório informar os produtos para o pedido;");
                            }
                        }

                        if (cargaIntegracao.ProdutoPredominante != null)
                        {
                            Embarcador.ProdutoEmbarcador.ProdutoEmbarcador servicoProdutoEmbarcador = new Embarcador.ProdutoEmbarcador.ProdutoEmbarcador(_unitOfWork);

                            if (!string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.CodigoProduto) && !(configuracaoWebService?.AtivarValidacaoDosProdutosNoAdicionarCarga ?? true))
                            {
                                if (!string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.CodigoDocumentacao))
                                    pedido.ProdutoPrincipal = repProdutoEmbarcador.buscarPorCodigoDocumentacao(cargaIntegracao.ProdutoPredominante.CodigoDocumentacao);

                                if (pedido.ProdutoPrincipal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.CodigoProduto))
                                    pedido.ProdutoPrincipal = repProdutoEmbarcador.buscarPorCodigoEmbarcador(cargaIntegracao.ProdutoPredominante.CodigoProduto);

                                if (pedido.ProdutoPrincipal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.CodigoProduto) && !string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.DescricaoProduto))
                                    pedido.ProdutoPrincipal = servicoProdutoEmbarcador.CriarProdutoEmbarcador(cargaIntegracao.ProdutoPredominante);
                            }
                            else if (pedido.ProdutoPrincipal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.CodigoProduto))
                            {
                                // #53886 Restringe que não seja adicionado pedido sem o produto
                                // #55176 Validações

                                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador existeProdutoCadastrado = repProdutoEmbarcador.buscarPorCodigoEmbarcador(cargaIntegracao.ProdutoPredominante.CodigoProduto);

                                if (existeProdutoCadastrado == null && string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.DescricaoProduto))
                                {
                                    if (unitOfWorkAdmin != null)
                                        unitOfWorkAdmin.Dispose();
                                    throw new ServicoException("Produto não cadastrado, descrição não informada, cadastro não foi realizado.");
                                }

                                if (existeProdutoCadastrado == null && string.IsNullOrEmpty(cargaIntegracao.ProdutoPredominante.CodigoGrupoProduto))
                                {
                                    if (unitOfWorkAdmin != null)
                                        unitOfWorkAdmin.Dispose();
                                    throw new ServicoException("Produto não cadastrado, grupo de produto não informado, cadastro não foi realizado.");
                                }

                                if (existeProdutoCadastrado == null)
                                    pedido.ProdutoPrincipal = servicoProdutoEmbarcador.CriarProdutoEmbarcadorComRestricaoGrupoProduto(cargaIntegracao.ProdutoPredominante);

                                if (cargaIntegracao.ProdutoPredominante.PesoUnitario > 0)
                                    existeProdutoCadastrado.PesoUnitario = cargaIntegracao.ProdutoPredominante.PesoUnitario;

                                if (cargaIntegracao.ProdutoPredominante.Quantidade > 0)
                                {
                                    int.TryParse($"{cargaIntegracao.ProdutoPredominante.Quantidade}", out int quantidade);
                                    bool unidadeIgualTonelada = existeProdutoCadastrado?.Unidade?.UnidadeMedida == Dominio.Enumeradores.UnidadeMedida.TON;

                                    if (unidadeIgualTonelada)
                                        existeProdutoCadastrado.QuantidadeCaixaPorPallet = quantidade;
                                    else
                                        existeProdutoCadastrado.QuantidadeCaixa = quantidade;
                                }
                                repProdutoEmbarcador.Atualizar(existeProdutoCadastrado);

                                pedido.ProdutoPrincipal = existeProdutoCadastrado;
                            }
                            if (!string.IsNullOrWhiteSpace(cargaIntegracao.ProdutoPredominante.DescricaoProduto))
                                pedido.ProdutoPredominante = cargaIntegracao.ProdutoPredominante.DescricaoProduto;
                            else
                                pedido.ProdutoPredominante = pedido.ProdutoPrincipal?.Descricao ?? "";
                        }

                        InformarRotaFretePedido(pedido, configuracaoWebService);

                        if (Servicos.Embarcador.Pedido.Pedido.TomadorPossuiPendenciaFinanceira(pedido, tipoServicoMultisoftware, out string mensagemErro))
                            stMensagem.Append(mensagemErro);

                        Dominio.Entidades.Empresa empresa = null;

                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.TransportadoraEmitente?.CNPJ))
                            empresa = repositorioEmpresa.BuscarPorCNPJ(cargaIntegracao.TransportadoraEmitente.CNPJ);

                        if (configuracaoDadosTransporte?.ExigirQueApolicePropriaTransportadorEstejaValida ?? false)
                            servicoJanelaCarregamentoTransportadorValidacoes.ValidarApoliceSeguro(cargaIntegracao.ProtocoloCarga, empresa, null, configuracaoDadosTransporte);

                        if (stMensagem.Length == 0)
                        {
                            pedido.UltimaAtualizacao = DateTime.Now;
                            pedido.SituacaoAcompanhamentoPedido = SituacaoAcompanhamentoPedido.AgColeta;

                            if (pedido.Destinatario?.ExigeQueEntregasSejamAgendadas ?? false)
                                pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.AguardandoAgendamento;

                            if (!string.IsNullOrWhiteSpace(pedido.NumeroPedidoDevolucao))
                                pedido.PedidoDevolucao = repositorioPedido.BuscarPedidoDeDevolucao(pedido.NumeroPedidoDevolucao, pedido.Filial?.Codigo ?? 0);

                            if (auditado != null)
                            {
                                repositorioPedido.Inserir(pedido);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, "Adicionado. Protocolo " + pedido.Codigo.ToString(), _unitOfWork);
                            }
                            else
                                repositorioPedido.Inserir(pedido, auditado);

                            if (cargaIntegracao.PercursosMDFe != null && cargaIntegracao.PercursosMDFe.Count > 0)
                            {
                                List<string> passagensPercursoEstado = cargaIntegracao.PercursosMDFe.Select(o => o?.Sigla ?? string.Empty).ToList();
                                for (int posicao = 0; posicao < passagensPercursoEstado.Count; posicao++)
                                {
                                    string siglaPassagemPercursoEstado = passagensPercursoEstado[posicao];

                                    Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens percursoEstado = repositorioPassagemPercursoEstado.BuscarPorPedidoESigla(pedido.Codigo, siglaPassagemPercursoEstado);
                                    if (percursoEstado == null)
                                        percursoEstado = new Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens();

                                    percursoEstado.Posicao = posicao + 1;
                                    percursoEstado.Pedido = pedido;
                                    percursoEstado.EstadoDePassagem = new Dominio.Entidades.Estado()
                                    {
                                        Sigla = siglaPassagemPercursoEstado
                                    };

                                    if (percursoEstado.Codigo > 0)
                                        repositorioPassagemPercursoEstado.Atualizar(percursoEstado);
                                    else
                                        repositorioPassagemPercursoEstado.Inserir(percursoEstado);
                                }
                            }

                            if (pedido.PedidoDeDevolucao)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoOrigem = repositorioPedido.BuscarPedidoPorNumeroDevolucao(pedido.NumeroPedidoEmbarcador, pedido.Filial?.Codigo ?? 0);
                                if (pedidoOrigem != null && pedidoOrigem.PedidoDevolucao == null)
                                {
                                    pedidoOrigem.PedidoDevolucao = pedido;
                                    repositorioPedido.Atualizar(pedidoOrigem);
                                }
                            }

                            pedido.Protocolo = pedido.Codigo;
                            pedido.NumeroControle = cargaIntegracao.NumeroControlePedido;
                            pedido.PedidoSVM = cargaIntegracao.CargaSVMProprio;

                            if (!string.IsNullOrEmpty(cargaIntegracao.PrevisaoSaidaDestino))
                                pedido.DataPrevisaoSaidaDestinatario = cargaIntegracao.PrevisaoSaidaDestino.ToNullableDateTime();

                            PreencherDadosTransporteMaritimo(pedido, cargaIntegracao, stMensagem, tipoServicoMultisoftware, unitOfWorkAdmin);

                            pedido.CotacaoPedido = cotacaoPedido;
                            if (cotacaoPedido != null)
                            {
                                if (cotacaoPedido.SolicitacaoCotacao != null)
                                {
                                    List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacaoPedidos = repCotacaoPedido.BuscarPorSolicitacao(cotacaoPedido.SolicitacaoCotacao.Codigo);
                                    foreach (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao in cotacaoPedidos)
                                    {
                                        cotacao.Pedido = pedido;
                                        repCotacaoPedido.Atualizar(cotacao);
                                    }
                                }
                                else if (cotacaoPedido.Pedido != null) // else temporário até que se migre as cotações de pedido para solicitação, após algum tempo pode ser removido.
                                {
                                    List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacaoPedidos = repCotacaoPedido.BuscarPorPedido(cotacaoPedido.Pedido.Codigo);
                                    foreach (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao in cotacaoPedidos)
                                    {
                                        cotacao.Pedido = pedido;
                                        repCotacaoPedido.Atualizar(cotacao);
                                    }
                                }

                                pedido.ValorFreteCotado = cotacaoPedido.ValorFrete;
                                pedido.ValorFreteAReceber = cotacaoPedido.ValorFrete;
                                pedido.PrevisaoEntrega = cotacaoPedido.Previsao;
                                if (cotacaoPedido.Empresa != null)
                                    pedido.Empresa = cotacaoPedido.Empresa;
                            }

                            pedido.TipoTomadorCabotagem = cargaIntegracao.TipoTomadorCabotagem;
                            pedido.TipoModalPropostaCabotagem = cargaIntegracao.TipoModalPropostaCabotagem;
                            pedido.TipoPropostaCabotagem = cargaIntegracao.TipoPropostaCabotagem;

                            repositorioConfiguracaoPedido.Atualizar(configuracaoPedido);

                            repositorioPedido.Atualizar(pedido);

                            VincularNotasFiscais(ref pedido, cargaIntegracao, ref stMensagem);
                            VincularAverbacoesPedidos(ref pedido, cargaIntegracao, ref stMensagem);
                            InformarValorFretePedido(ref pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware);
                            VincularAdicionaisPedido(ref pedido, cargaIntegracao, configuracaoWebService, auditado);

                            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoGerado, pedido, configuracaoTMS, null, null, notificarAcompanhamento);

                            if (pedido.TipoOperacao != null && repositorioTipoOperacaoIntegracao.ExisteTipoOperacaoIntegracao(TipoIntegracao.Routeasy, pedido.TipoOperacao.Codigo))
                                new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(_unitOfWork).AdicionarParaIntegracaoAutomaticamente(pedido.Codigo, TipoRoteirizadorIntegracao.EnviarPedido);

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repositorioPedido.ContemPedidoMesmoNumero(pedido.Numero))
                            {
                                pedido.Numero = repositorioPedido.BuscarProximoNumero();
                                repositorioPedido.Atualizar(pedido);
                            }

                            if (cargaIntegracao.Container != null && !cargaIntegracao.NaoAtualizarDadosDoPedido)
                            {
                                pedido.Container = SalvarContainer(cargaIntegracao.Container, ref stMensagem, auditado);
                                repositorioPedido.Atualizar(pedido);
                            }
                        }
                    }
                }
            }

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoRoutEasy = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.Routeasy);

            if (tipoIntegracaoRoutEasy != null && repositorioPedido.VerificarExistenciaPedidoAsync(pedido.NumeroPedidoEmbarcador, SituacaoPedido.Cancelado).GetAwaiter().GetResult())
            {
                new Servicos.Embarcador.Pedido.Pedido(_unitOfWork).GerarIntegracaoAsync(pedido, tipoIntegracaoRoutEasy).GetAwaiter().GetResult();
            }

            if (unitOfWorkAdmin != null)
                unitOfWorkAdmin.Dispose();

            return pedido;
        }

        public void PreecherEnderecoPedidoPorCliente(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.Entidades.Cliente cliente)
        {
            pedidoEndereco.Bairro = cliente.Bairro;
            pedidoEndereco.CEP = cliente.CEP;
            pedidoEndereco.Localidade = cliente.Localidade;
            pedidoEndereco.Complemento = cliente.Complemento;
            pedidoEndereco.Endereco = cliente.Endereco;
            pedidoEndereco.Numero = string.IsNullOrWhiteSpace(cliente.Numero) ? "S/N" : cliente.Numero;
            pedidoEndereco.IE_RG = string.IsNullOrWhiteSpace(cliente.IE_RG) ? "ISENTO" : cliente.IE_RG;
            pedidoEndereco.Telefone = cliente.Telefone1;

        }

        public void PreecherEnderecoPedidoPorClienteOutroEndereco(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco)
        {
            pedidoEndereco.ClienteOutroEndereco = clienteOutroEndereco;
            pedidoEndereco.Bairro = clienteOutroEndereco.Bairro;
            pedidoEndereco.CEP = clienteOutroEndereco.CEP;
            pedidoEndereco.Localidade = clienteOutroEndereco.Localidade;
            pedidoEndereco.Complemento = clienteOutroEndereco.Complemento;
            pedidoEndereco.Endereco = clienteOutroEndereco.Endereco;
            pedidoEndereco.Numero = string.IsNullOrWhiteSpace(clienteOutroEndereco.Numero) ? "S/N" : clienteOutroEndereco.Numero;
            pedidoEndereco.IE_RG = string.IsNullOrWhiteSpace(clienteOutroEndereco.IE_RG) ? "ISENTO" : clienteOutroEndereco.IE_RG;
            pedidoEndereco.Telefone = clienteOutroEndereco.Telefone;
        }

        public void RemoverDadosEssencaisDoPedido(ref Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracao = repConfiguracaoWebService.BuscarPrimeiroRegistro();
            if ((configuracao?.IgnorarCamposEssenciais ?? false))
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador primeiroProduto = repProdutoEmbarcador.BuscarPrimeiroAtivo();
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga primeiroTipoDeCarga = repTipoDeCarga.BuscarPrimeira();

                cargaIntegracao.CargaRefrigeradaPrecisaEnergia = false;
                cargaIntegracao.CentroCusto = null;
                cargaIntegracao.ContainerADefinir = false;
                cargaIntegracao.ContemCargaPerigosa = false;
                cargaIntegracao.ContemCargaRefrigerada = false;
                cargaIntegracao.DataCriacaoCarga = string.Empty;
                cargaIntegracao.DescricaoCarrierNavioViagem = string.Empty;
                cargaIntegracao.DescricaoTipoPropostaFeeder = string.Empty;
                cargaIntegracao.Destino = null;
                cargaIntegracao.EmpresaResponsavel = null;
                cargaIntegracao.FecharCargaAutomaticamente = true;
                cargaIntegracao.FormaAverbacaoCTE = null;
                cargaIntegracao.NaoAtualizarDadosDoPedido = false;
                cargaIntegracao.NecessitaAverbacao = false;
                cargaIntegracao.NumeroLacre1 = string.Empty;
                cargaIntegracao.NumeroLacre2 = string.Empty;
                cargaIntegracao.NumeroLacre3 = string.Empty;
                cargaIntegracao.ObservacaoProposta = string.Empty;
                cargaIntegracao.Origem = null;
                cargaIntegracao.PedidoDeSVMTerceiro = false;
                cargaIntegracao.PortoDestino = null;
                cargaIntegracao.ProdutoPredominante = primeiroProduto != null ? (new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = primeiroProduto.Descricao, CodigoProduto = primeiroProduto.CodigoProdutoEmbarcador }) : null;
                cargaIntegracao.PropostaComercial = null;
                cargaIntegracao.QuantidadeConhecimentosTaxaDocumentacao = 0;
                cargaIntegracao.QuantidadeContainerBooking = 0;
                cargaIntegracao.QuantidadeVolumes = 0;
                cargaIntegracao.RealizarCobrancaTaxaDocumentacao = false;
                cargaIntegracao.Temperatura = string.Empty;
                cargaIntegracao.TemperaturaObservacao = string.Empty;
                cargaIntegracao.TerminalPortoDestino = null;
                cargaIntegracao.TipoCalculoCargaFracionada = null;
                cargaIntegracao.TipoCargaEmbarcador = primeiroTipoDeCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = primeiroTipoDeCarga.CodigoTipoCargaEmbarcador, Descricao = primeiroTipoDeCarga.Descricao } : null;
                //cargaIntegracao.TipoContainerReserva = null;
                cargaIntegracao.TipoDocumentoAverbacao = null;
                //cargaIntegracao.TipoOperacao = null;
                cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                cargaIntegracao.TipoPropostaFeeder = null;
                cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.NaoInformado;
                cargaIntegracao.Transbordo = null;
                //cargaIntegracao.TransportadoraEmitente = null;
                cargaIntegracao.ValidarNumeroContainer = false;
                cargaIntegracao.ValorCusteioSVM = 0m;
                cargaIntegracao.ValorFrete = null;
                cargaIntegracao.ValorTaxaDocumento = 0m;
                cargaIntegracao.Viagem = null;
            }
        }

        public Dominio.Entidades.Embarcador.Pedidos.Container SalvarContainer(Dominio.ObjetosDeValor.Embarcador.Carga.Container containerIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (containerIntegracao == null)
                return null;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork);

            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            string numeroContainer = "";
            Dominio.Entidades.Embarcador.Pedidos.Container container = null;

            if (!string.IsNullOrWhiteSpace(containerIntegracao.Numero))
            {
                if (!string.IsNullOrWhiteSpace(containerIntegracao.Numero))
                    numeroContainer = containerIntegracao.Numero.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                container = repContainer.BuscarTodosPorNumero(numeroContainer);
            }
            if (container != null)
            {
                if (containerIntegracao.Atualizar)
                {
                    if (!string.IsNullOrWhiteSpace(containerIntegracao.Numero))
                        numeroContainer = containerIntegracao.Numero.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                    if (string.IsNullOrWhiteSpace(numeroContainer) || numeroContainer.Length != 11)
                    {
                        stMensagem.Append("O número do container não possui 11 dígitos.");
                        return null;
                    }
                    container.Initialize();
                    container.Descricao = !string.IsNullOrWhiteSpace(containerIntegracao.Descricao) ? containerIntegracao.Descricao : !string.IsNullOrWhiteSpace(numeroContainer) ? numeroContainer : "";
                    container.Numero = Utilidades.String.SanitizeString(numeroContainer);
                    container.Status = containerIntegracao.InativarCadastro ? false : true;
                    container.DataUltimaAtualizacao = DateTime.Now;
                    container.Integrado = false;
                    container.Tara = containerIntegracao.Tara;
                    container.CodigoIntegracao = containerIntegracao.CodigoIntegracao.ToString("D");
                    container.TipoPropriedade = containerIntegracao.TipoPropriedade;
                    container.MetrosCubicos = containerIntegracao.DencidadeProduto;
                    container.PesoLiquido = containerIntegracao.PesoLiquido;
                    if (containerIntegracao.TipoContainer != null)
                        container.ContainerTipo = SalvarContainerTipo(containerIntegracao.TipoContainer, ref stMensagem, auditado);
                    repContainer.Atualizar(container, auditado);

                    if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
                    {
                        if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                        {
                            stMensagem.Append("Número do container está inválido de acordo com o seu dígito verificado.");
                            return null;
                        }
                    }
                }

                return container;
            }

            if (containerIntegracao.CodigoIntegracao > 0 && !string.IsNullOrWhiteSpace(containerIntegracao.CodigoIntegracao.ToString("D")))
                container = repContainer.BuscarTodosPorCodigoIntegracao(containerIntegracao.CodigoIntegracao.ToString("D"));
            if (container != null)
            {
                if (containerIntegracao.Atualizar)
                {
                    if (!string.IsNullOrWhiteSpace(containerIntegracao.Numero))
                        numeroContainer = containerIntegracao.Numero.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                    if (string.IsNullOrWhiteSpace(numeroContainer) || numeroContainer.Length != 11)
                    {
                        stMensagem.Append("O número do container não possui 11 dígitos.");
                        return null;
                    }
                    container.Initialize();
                    container.Descricao = Utilidades.String.SanitizeString(!string.IsNullOrWhiteSpace(containerIntegracao.Descricao) ? containerIntegracao.Descricao : !string.IsNullOrWhiteSpace(numeroContainer) ? numeroContainer : "");
                    container.Numero = Utilidades.String.SanitizeString(numeroContainer);
                    container.Status = containerIntegracao.InativarCadastro ? false : true;
                    container.DataUltimaAtualizacao = DateTime.Now;
                    container.Integrado = false;
                    container.Tara = containerIntegracao.Tara;
                    container.CodigoIntegracao = containerIntegracao.CodigoIntegracao.ToString("D");
                    container.TipoPropriedade = containerIntegracao.TipoPropriedade;
                    container.MetrosCubicos = containerIntegracao.DencidadeProduto;
                    container.PesoLiquido = containerIntegracao.PesoLiquido;
                    if (containerIntegracao.TipoContainer != null)
                        container.ContainerTipo = SalvarContainerTipo(containerIntegracao.TipoContainer, ref stMensagem, auditado);
                    repContainer.Atualizar(container, auditado);

                    if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
                    {
                        if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                        {
                            stMensagem.Append("Número do container está inválido de acordo com o seu dígito verificado.");
                            return null;
                        }
                    }
                }

                return container;
            }

            if (container == null && !string.IsNullOrWhiteSpace(containerIntegracao.Numero))
            {
                container = repContainer.BuscarPorNumero(containerIntegracao.Numero);
                if (container != null && containerIntegracao.Atualizar)
                {
                    if (!string.IsNullOrWhiteSpace(containerIntegracao.Numero))
                        numeroContainer = containerIntegracao.Numero.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                    if (string.IsNullOrWhiteSpace(numeroContainer) || numeroContainer.Length != 11)
                    {
                        stMensagem.Append("O número do container não possui 11 dígitos.");
                        return null;
                    }
                    container.Initialize();
                    container.Descricao = !string.IsNullOrWhiteSpace(containerIntegracao.Descricao) ? containerIntegracao.Descricao : !string.IsNullOrWhiteSpace(numeroContainer) ? numeroContainer : "";
                    container.Numero = Utilidades.String.SanitizeString(numeroContainer);
                    container.CodigoIntegracao = containerIntegracao.CodigoIntegracao.ToString("D");
                    container.Status = containerIntegracao.InativarCadastro ? false : true;
                    container.DataUltimaAtualizacao = DateTime.Now;
                    container.Tara = containerIntegracao.Tara;
                    container.Integrado = false;
                    container.TipoPropriedade = containerIntegracao.TipoPropriedade;
                    container.MetrosCubicos = containerIntegracao.DencidadeProduto;
                    container.PesoLiquido = containerIntegracao.PesoLiquido;
                    if (containerIntegracao.TipoContainer != null)
                        container.ContainerTipo = SalvarContainerTipo(containerIntegracao.TipoContainer, ref stMensagem, auditado);
                    repContainer.Atualizar(container, auditado);

                    if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
                    {
                        if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                        {
                            stMensagem.Append("Número do container está inválido de acordo com o seu dígito verificado.");
                            return null;
                        }
                    }
                }
                if (container != null)
                    return container;
            }
            if (!string.IsNullOrWhiteSpace(containerIntegracao.Numero))
                numeroContainer = containerIntegracao.Numero.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
            if (string.IsNullOrWhiteSpace(numeroContainer) || numeroContainer.Length != 11)
            {
                stMensagem.Append("O número do container não possui 11 dígitos.");
                return null;
            }
            container = new Dominio.Entidades.Embarcador.Pedidos.Container()
            {
                CodigoIntegracao = containerIntegracao.CodigoIntegracao > 0 ? containerIntegracao.CodigoIntegracao.ToString("D") : "",
                ContainerTipo = SalvarContainerTipo(containerIntegracao.TipoContainer, ref stMensagem, auditado),
                Descricao = !string.IsNullOrWhiteSpace(containerIntegracao.Descricao) ? containerIntegracao.Descricao : !string.IsNullOrWhiteSpace(numeroContainer) ? numeroContainer : "",
                Numero = Utilidades.String.SanitizeString(numeroContainer),
                PesoLiquido = containerIntegracao.PesoLiquido,
                Status = containerIntegracao.InativarCadastro ? false : true,
                Tara = containerIntegracao.Tara,
                TipoPropriedade = containerIntegracao.TipoPropriedade,
                DataUltimaAtualizacao = DateTime.Now,
                Integrado = false
            };
            repContainer.Inserir(container, auditado);

            if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
            {
                if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                {
                    stMensagem.Append("Número do container está inválido de acordo com o seu dígito verificado.");
                    return null;
                }
            }

            return container;
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas SalvarGrupoPessoa(Dominio.ObjetosDeValor.Embarcador.Carga.GrupoPessoa grupoPessoaIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (grupoPessoaIntegracao == null)
                return null;

            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ repGrupoPessoasRaizCNPJ = new Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = null;

            if (string.IsNullOrWhiteSpace(grupoPessoaIntegracao.CodigoIntegracao))
            {
                stMensagem.Append("Favor informe o código de integração do Grupo de Pessoa; ");
                return null;
            }

            Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = repRateioFormula.BuscarPorTipo(grupoPessoaIntegracao.ParametroRateioFormulaExclusivo);

            if (!string.IsNullOrWhiteSpace(grupoPessoaIntegracao.CodigoIntegracao))
                grupoPessoas = repGrupoPessoas.BuscarPorCodigoIntegracao(grupoPessoaIntegracao.CodigoIntegracao);
            if (grupoPessoas != null)
            {
                grupoPessoas.Initialize();

                grupoPessoas.Ativo = grupoPessoaIntegracao.InativarCadastro ? false : true;

                if (grupoPessoaIntegracao.TipoEmissaoCTeDocumentosExclusivo == TipoEmissaoCTeDocumentos.NaoInformado && !grupoPessoas.NaoAlterarDocumentoIntegracao)
                    grupoPessoas.TipoEmissaoCTeDocumentosExclusivo = grupoPessoaIntegracao.TipoEmissaoCTeDocumentosExclusivo;
                else if (grupoPessoaIntegracao.TipoEmissaoCTeDocumentosExclusivo != TipoEmissaoCTeDocumentos.NaoInformado)
                    grupoPessoas.TipoEmissaoCTeDocumentosExclusivo = grupoPessoaIntegracao.TipoEmissaoCTeDocumentosExclusivo;

                if (grupoPessoaIntegracao.ParametroRateioFormulaExclusivo == ParametroRateioFormula.todos && !grupoPessoas.NaoAlterarDocumentoIntegracao)
                    grupoPessoas.RateioFormulaExclusivo = rateioFormula;
                else if (grupoPessoaIntegracao.ParametroRateioFormulaExclusivo != ParametroRateioFormula.todos)
                    grupoPessoas.RateioFormulaExclusivo = rateioFormula;

                grupoPessoas.ExigirNumeroControleCliente = grupoPessoaIntegracao.ExigirNumeroControleCliente;
                grupoPessoas.ExigirNumeroNumeroReferenciaCliente = grupoPessoaIntegracao.ExigirNumeroNumeroReferenciaCliente;
                grupoPessoas.TipoGrupoPessoas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoPessoas.Clientes;
                grupoPessoas.VincularNotaFiscalEmailNaCarga = true;
                if (string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoBooking))
                    grupoPessoas.ExpressaoBooking = @"\d[a-zA-Z]{3}(AK|AM|([a-zA-Z]{2}))\d{0,5}[a-zA-Z]{0,4}\w";
                if (string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoContainer))
                    grupoPessoas.ExpressaoContainer = @"(?<=\s)*([a-z]{3})u[- /\.]*((\d{7}|\d{6}[- /\.]*\d)|(\d{3}[- /\.]*\d{3}[- /\.]*\d))(?=\s)*";

                repGrupoPessoas.Atualizar(grupoPessoas, auditado);

                SalvarPessoasGrupoPessoa(grupoPessoaIntegracao, grupoPessoas.Codigo, auditado);

                return grupoPessoas;
            }

            if (string.IsNullOrWhiteSpace(grupoPessoaIntegracao.Descricao))
                stMensagem.Append("Favor informe a descrição o do Grupo de Pessoa; ");

            grupoPessoas = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas()
            {
                CodigoIntegracao = grupoPessoaIntegracao.CodigoIntegracao,
                Descricao = grupoPessoaIntegracao.Descricao,
                TipoEmissaoCTeDocumentosExclusivo = grupoPessoaIntegracao.TipoEmissaoCTeDocumentosExclusivo,
                RateioFormulaExclusivo = rateioFormula,
                Ativo = grupoPessoaIntegracao.InativarCadastro ? false : true
            };

            grupoPessoas.ExigirNumeroControleCliente = grupoPessoaIntegracao.ExigirNumeroControleCliente;
            grupoPessoas.ExigirNumeroNumeroReferenciaCliente = grupoPessoaIntegracao.ExigirNumeroNumeroReferenciaCliente;
            grupoPessoas.TipoGrupoPessoas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoPessoas.Clientes;
            grupoPessoas.VincularNotaFiscalEmailNaCarga = true;
            if (string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoBooking))
                grupoPessoas.ExpressaoBooking = @"\d[a-zA-Z]{3}(AK|AM|([a-zA-Z]{2}))\d{0,5}[a-zA-Z]{0,4}\w";
            if (string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoContainer))
                grupoPessoas.ExpressaoContainer = @"(?<=\s)*([a-z]{3})u[- /\.]*((\d{7}|\d{6}[- /\.]*\d)|(\d{3}[- /\.]*\d{3}[- /\.]*\d))(?=\s)*";

            repGrupoPessoas.Inserir(grupoPessoas, auditado);

            SalvarPessoasGrupoPessoa(grupoPessoaIntegracao, grupoPessoas.Codigo, auditado);

            return grupoPessoas;
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio SalvarNavio(Dominio.ObjetosDeValor.Embarcador.Carga.Navio navioIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool metodoPorWS)
        {
            if (navioIntegracao == null)
                return null;

            var codigoNavio = "";
            if (!string.IsNullOrWhiteSpace(navioIntegracao?.CodigoNavio ?? ""))
            {
                codigoNavio = navioIntegracao?.CodigoNavio.Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(codigoNavio) && codigoNavio.Length != 3)
                {
                    throw new ServicoException($"O campo 'Código do Navio' precisa ter 3 dígitos ({codigoNavio}).");
                }
            }

            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Navio navio = null;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork).BuscarIntegracao();

            if (integracaoIntercab?.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio == false)
            {
                if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoIntegracao))
                    navio = repNavio.BuscarTodosPorCodigoIntegracao(navioIntegracao.CodigoIntegracao);
                if (navio == null && !string.IsNullOrWhiteSpace(navioIntegracao.CodigoIRIN) && !metodoPorWS)
                    navio = repNavio.BuscarTodosPorCodigoIRIN(navioIntegracao.CodigoIRIN);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoIRIN) && !metodoPorWS)
                    navio = repNavio.BuscarTodosPorCodigoIRIN(navioIntegracao.CodigoIRIN);
                if (navio == null && !string.IsNullOrWhiteSpace(navioIntegracao.Descricao))
                    navio = repNavio.BuscarTodosPorDescricao(navioIntegracao.Descricao);
                if (navio == null && !string.IsNullOrWhiteSpace(navioIntegracao.CodigoIntegracao))
                    navio = repNavio.BuscarTodosPorCodigoIntegracao(navioIntegracao.CodigoIntegracao);
            }

            if (navio != null && !metodoPorWS)
            {
                if (navioIntegracao.Atualizar)
                {
                    navio.Initialize();
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoEmbarcacao))
                        navio.CodigoEmbarcacao = navioIntegracao.CodigoEmbarcacao;
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.Descricao))
                        navio.Descricao = navioIntegracao.Descricao;
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoIRIN))
                        navio.Irin = navioIntegracao.CodigoIRIN;
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoDocumento))
                        navio.CodigoDocumento = navioIntegracao.CodigoDocumento;
                    navio.CodigoNavio = codigoNavio;
                    navio.Status = navioIntegracao.InativarCadastro ? false : true;
                    navio.TipoEmbarcacao = navioIntegracao.TipoEmbarcacao;
                    navio.CodigoIMO = navioIntegracao.CodigoIMO;
                    navio.Integrado = false;
                    repNavio.Atualizar(navio, auditado);
                }

                return navio;
            }

            if (integracaoIntercab?.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio == false)
            {
                if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoIntegracao))
                    navio = repNavio.BuscarTodosPorCodigoIntegracao(navioIntegracao.CodigoIntegracao);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(navioIntegracao.Descricao))
                    navio = repNavio.BuscarTodosPorDescricao(navioIntegracao.Descricao);
                if (navio == null && !string.IsNullOrWhiteSpace(navioIntegracao.CodigoIntegracao))
                    navio = repNavio.BuscarTodosPorCodigoIntegracao(navioIntegracao.CodigoIntegracao);
            }

            if (navio != null)
            {
                if (navioIntegracao.Atualizar)
                {
                    navio.Initialize();
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoEmbarcacao))
                        navio.CodigoEmbarcacao = navioIntegracao.CodigoEmbarcacao;
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.Descricao))
                        navio.Descricao = navioIntegracao.Descricao;
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoIRIN))
                        navio.Irin = navioIntegracao.CodigoIRIN;
                    if (!string.IsNullOrWhiteSpace(navioIntegracao.CodigoDocumento))
                        navio.CodigoDocumento = navioIntegracao.CodigoDocumento;
                    navio.CodigoNavio = codigoNavio;
                    navio.Status = navioIntegracao.InativarCadastro ? false : true;
                    navio.TipoEmbarcacao = navioIntegracao.TipoEmbarcacao;
                    navio.CodigoIMO = navioIntegracao.CodigoIMO;
                    navio.Integrado = false;

                    //if (navio.CodigoNavio.Length < 3)
                    //    return WebServiceException()
                    //        .WithMessage("O campo 'Código do Navio' deve ser de 3 dígitos");

                    repNavio.Atualizar(navio, auditado);
                }

                return navio;
            }

            navio = new Dominio.Entidades.Embarcador.Pedidos.Navio()
            {
                CodigoEmbarcacao = navioIntegracao.CodigoEmbarcacao,
                CodigoIntegracao = navioIntegracao.CodigoIntegracao,
                Descricao = navioIntegracao.Descricao,
                Irin = navioIntegracao.CodigoIRIN,
                CodigoDocumento = navioIntegracao.CodigoDocumento,
                Status = !navioIntegracao.InativarCadastro,
                TipoEmbarcacao = navioIntegracao.TipoEmbarcacao,
                CodigoIMO = navioIntegracao.CodigoIMO,
                CodigoNavio = navioIntegracao?.CodigoNavio,
                Integrado = false
            };
            repNavio.Inserir(navio, auditado);

            return navio;
        }

        public Dominio.Entidades.Embarcador.Pedidos.Porto SalvarPorto(Dominio.ObjetosDeValor.Embarcador.Carga.Porto portoIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (portoIntegracao == null)
                return null;

            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Porto porto = null;

            if (!string.IsNullOrWhiteSpace(portoIntegracao.Descricao))
                porto = repPorto.BuscarPorDescricao(portoIntegracao.Descricao);
            if (porto == null && !string.IsNullOrWhiteSpace(portoIntegracao.CodigoIntegracao))
                porto = repPorto.BuscarPorCodigoIntegracao(portoIntegracao.CodigoIntegracao);
            if (porto == null && !string.IsNullOrWhiteSpace(portoIntegracao.CodigoDocumento))
                porto = repPorto.BuscarPorCodigoDocumento(portoIntegracao.CodigoDocumento);

            if (porto != null)
            {
                if (portoIntegracao.Atualizar)
                {
                    porto.Initialize();

                    porto.CodigoIntegracao = portoIntegracao.CodigoIntegracao;
                    porto.CodigoIATA = portoIntegracao.CodigoIATA;
                    porto.CodigoDocumento = portoIntegracao.CodigoDocumento;
                    porto.Descricao = portoIntegracao.Descricao;
                    porto.Localidade = buscarLocalidadeEndereco(portoIntegracao.Localidade, ref stMensagem);
                    porto.Integrado = false;
                    porto.Ativo = portoIntegracao.InativarCadastro ? false : true;
                    porto.CodigoMercante = portoIntegracao.CodigoMercante;
                    porto.QuantidadeHorasFaturamentoAutomatico = portoIntegracao.QuantidadeHorasFaturamentoAutomatico;
                    porto.AtivarDespachanteComoConsignatario = portoIntegracao.AtivarDespachanteComoConsignatario;
                    porto.DiasAntesDoPodParaEnvioDaDocumentacao = portoIntegracao.DiasAntesDoPodParaEnvioDaDocumentacao;

                    repPorto.Atualizar(porto, auditado);
                }
                return porto;
            }

            porto = new Dominio.Entidades.Embarcador.Pedidos.Porto()
            {
                CodigoIntegracao = portoIntegracao.CodigoIntegracao,
                CodigoIATA = portoIntegracao.CodigoIATA,
                CodigoDocumento = portoIntegracao.CodigoDocumento,
                Descricao = portoIntegracao.Descricao,
                Localidade = buscarLocalidadeEndereco(portoIntegracao.Localidade, ref stMensagem),
                Integrado = false,
                Ativo = portoIntegracao.InativarCadastro ? false : true,
                CodigoMercante = portoIntegracao.CodigoMercante,
                QuantidadeHorasFaturamentoAutomatico = portoIntegracao.QuantidadeHorasFaturamentoAutomatico,
                AtivarDespachanteComoConsignatario = portoIntegracao.AtivarDespachanteComoConsignatario,
                DiasAntesDoPodParaEnvioDaDocumentacao = portoIntegracao.DiasAntesDoPodParaEnvioDaDocumentacao
            };
            repPorto.Inserir(porto, auditado);

            return porto;
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao SalvarTerminalPorto(Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto terminalIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, bool consultarPelaDescricao = false)
        {
            if (terminalIntegracao == null)
                return null;

            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);

            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = null;

            if (consultarPelaDescricao)
                if (!string.IsNullOrWhiteSpace(terminalIntegracao.Descricao))
                    terminal = repTipoTerminalImportacao.BuscarPorDescricao(terminalIntegracao.Descricao);

            if (terminal == null && !string.IsNullOrWhiteSpace(terminalIntegracao.CodigoIntegracao))
                terminal = repTipoTerminalImportacao.BuscarPorCodigoIntegracao(terminalIntegracao.CodigoIntegracao);
            if (terminal == null && !string.IsNullOrWhiteSpace(terminalIntegracao.Descricao))
                terminal = repTipoTerminalImportacao.BuscarPorDescricao(terminalIntegracao.Descricao);
            if (terminal == null && !string.IsNullOrWhiteSpace(terminalIntegracao.CodigoDocumento))
                terminal = repTipoTerminalImportacao.BuscarTodosPorCodigoDocumento(terminalIntegracao.CodigoDocumento);
            if (terminal == null && !string.IsNullOrWhiteSpace(terminalIntegracao.CodigoIntegracao))
                terminal = repTipoTerminalImportacao.BuscarTodosPorCodigoIntegracao(terminalIntegracao.CodigoIntegracao);

            if (terminal == null && tipoServicoMultisoftware.HasValue && tipoServicoMultisoftware.Value == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                throw new ServicoException("Terminal portuário não cadastrado no sistema.");

            if (terminal != null)
            {
                if (terminalIntegracao.Atualizar)
                {
                    terminal.Ativo = terminalIntegracao.InativarCadastro ? false : true;
                    terminal.Descricao = terminalIntegracao.Descricao;
                    terminal.CodigoTerminal = terminalIntegracao.CodigoTerminal;
                    terminal.Porto = SalvarPorto(terminalIntegracao.Porto, ref stMensagem, auditado);
                    if (!string.IsNullOrWhiteSpace(terminalIntegracao.CodigoIntegracao))
                        terminal.CodigoIntegracao = terminalIntegracao.CodigoIntegracao;
                    terminal.CodigoDocumento = terminalIntegracao.CodigoDocumento;
                    terminal.Integrado = false;
                    terminal.Terminal = null;
                    terminal.CodigoMercante = terminalIntegracao.CodigoMercante;
                    terminal.QuantidadeDiasEnvioDocumentacao = terminalIntegracao.QuantidadeDiasEnvioDocumentacao;
                    terminal.CodigoObservacaoContribuinte = terminalIntegracao.CodigoObservacaoContribuinte;

                    if (terminalIntegracao.Terminal != null)
                    {
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoTerminalAtualizar = servicoCliente.ConverterObjetoValorPessoa(terminalIntegracao.Terminal, "Terminal", _unitOfWork, 0, true);
                        if (retornoTerminalAtualizar.Status == false)
                            terminal.Terminal = null;
                        else
                            terminal.Terminal = retornoTerminalAtualizar.cliente;
                    }

                    repTipoTerminalImportacao.Atualizar(terminal, auditado);
                }

                return terminal;
            }

            terminal = new Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao()
            {
                Ativo = true,
                Descricao = terminalIntegracao.Descricao,
                CodigoTerminal = terminalIntegracao.CodigoTerminal,
                Porto = SalvarPorto(terminalIntegracao.Porto, ref stMensagem, auditado),
                Terminal = null,
                CodigoIntegracao = terminalIntegracao.CodigoIntegracao,
                CodigoDocumento = terminalIntegracao.CodigoDocumento,
                Integrado = false,
                CodigoMercante = terminalIntegracao.CodigoMercante,
                QuantidadeDiasEnvioDocumentacao = terminalIntegracao.QuantidadeDiasEnvioDocumentacao,
                CodigoObservacaoContribuinte = terminalIntegracao.CodigoObservacaoContribuinte
            };

            if (terminalIntegracao.Terminal != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoTerminal = servicoCliente.ConverterObjetoValorPessoa(terminalIntegracao.Terminal, "Terminal", _unitOfWork, 0, false);
                if (retornoTerminal.Status == false)
                {
                    stMensagem.Append(retornoTerminal.Mensagem);
                }
                else
                {
                    terminal.Terminal = retornoTerminal.cliente;
                }
            }

            repTipoTerminalImportacao.Inserir(terminal, auditado);

            return terminal;
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao SalvarTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao tipoOperacaoIntegracao, ref StringBuilder stMensagem)
        {
            if (tipoOperacaoIntegracao == null)
                return null;

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;

            tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(tipoOperacaoIntegracao.CodigoIntegracao);
            if (tipoOperacao != null)
                return tipoOperacao;

            if (!tipoOperacaoIntegracao.Atualizar)
                return null;

            tipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao()
            {
                Ativo = true,
                CodigoIntegracao = tipoOperacaoIntegracao.CodigoIntegracao,
                Descricao = tipoOperacaoIntegracao.Descricao,
                BloquearEmissaoDeEntidadeSemCadastro = tipoOperacaoIntegracao.BloquearEmissaoDeEntidadeSemCadastro,
                BloquearEmissaoDosDestinatario = tipoOperacaoIntegracao.BloquearEmissaoDosDestinatario,
                TipoCobrancaMultimodal = tipoOperacaoIntegracao.TipoCobrancaMultimodal,
                ModalPropostaMultimodal = tipoOperacaoIntegracao.ModalPropostaMultimodal,
                TipoServicoMultimodal = tipoOperacaoIntegracao.TipoServicoMultimodal,
                TipoPropostaMultimodal = tipoOperacaoIntegracao.TipoPropostaMultimodal,
                PermiteGerarPedidoSemDestinatario = true,
                UsarConfiguracaoEmissao = true
            };
            if (tipoOperacaoIntegracao.CNPJsDestinatariosNaoAutorizados != null && tipoOperacaoIntegracao.CNPJsDestinatariosNaoAutorizados.Count > 0)
            {
                tipoOperacao.ClientesBloquearEmissaoDosDestinatario = new List<Dominio.Entidades.Cliente>();
                foreach (string cnpj in tipoOperacaoIntegracao.CNPJsDestinatariosNaoAutorizados)
                {
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpj));
                    if (cliente != null)
                        tipoOperacao.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
            }

            repTipoOperacao.Inserir(tipoOperacao);

            return tipoOperacao;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio SalvarViagem(Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagemIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool encerrarMDFeAutomaticamente)
        {
            //aqui não tem nada
            if (viagemIntegracao == null)
                return null;

            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoViagemNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = null;
            Dominio.Entidades.Embarcador.Pedidos.Navio navio = null;
            string descricaoViagem = "";

            if (!string.IsNullOrWhiteSpace(viagemIntegracao.Descricao))
            {
                viagem = repPedidoViagemNavio.BuscarPorDescricao(viagemIntegracao.Descricao);
                if (viagem == null && viagemIntegracao.Navio != null)
                {
                    descricaoViagem = viagemIntegracao.Navio.Descricao + "/" + viagemIntegracao.NumeroViagem.ToString("D") + Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(viagemIntegracao.Direcao);
                    viagem = repPedidoViagemNavio.BuscarPorDescricao(viagemIntegracao.Descricao);
                }
            }

            if (viagem != null)
            {
                if (viagemIntegracao.Atualizar)
                {
                    if (!string.IsNullOrWhiteSpace(viagemIntegracao.Navio.Descricao))
                        navio = repNavio.BuscarTodosPorDescricao(viagemIntegracao.Navio.Descricao);
                    if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoIntegracao))
                        navio = repNavio.BuscarPorCodigoIntegracao(viagemIntegracao.Navio.CodigoIntegracao);
                    if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoIRIN))
                        navio = repNavio.BuscarTodosPorCodigoIRIN(viagemIntegracao.Navio.CodigoIRIN);

                    if (navio != null)
                    {
                        descricaoViagem = navio.Descricao + "/" + viagemIntegracao.NumeroViagem.ToString("D") + Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(viagemIntegracao.Direcao);

                        viagem.Initialize();
                        viagem.Status = viagemIntegracao.InativarCadastro ? false : true;
                        viagem.Integrado = false;
                        viagem.CodigoIntegracao = viagemIntegracao.CodigoIntegracao.ToString("D");
                        viagem.Descricao = descricaoViagem;
                        viagem.DirecaoViagemMultimodal = viagemIntegracao.Direcao;
                        viagem.Navio = navio;
                        viagem.NumeroViagem = viagemIntegracao.NumeroViagem;
                        repPedidoViagemNavio.Atualizar(viagem, Auditado);
                    }
                }

                if (viagemIntegracao.Schedules != null && viagemIntegracao.Schedules.Count > 0)
                {
                    List<int> codigosRemover = new List<int>();
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Schedule schedule in viagemIntegracao.Schedules)
                    {
                        List<int> codigos = new List<int>();
                        SalvarSchedule(schedule, viagem, ref stMensagem, _unitOfWork.StringConexao, Auditado, out codigos, encerrarMDFeAutomaticamente);
                        codigosRemover.AddRange(codigos);
                    }

                    if (codigosRemover != null && codigosRemover.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> schedulesExcluir = repPedidoViagemNavioSchedule.BuscarSchedulesExcluir(codigosRemover, viagem.Codigo);
                        if (schedulesExcluir != null && schedulesExcluir.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule scheduleExcluir in schedulesExcluir)
                            {
                                scheduleExcluir.Initialize();
                                repPedidoViagemNavioSchedule.Deletar(scheduleExcluir, Auditado);
                            }
                        }
                    }
                }

                return viagem;
            }

            if (!string.IsNullOrWhiteSpace(viagemIntegracao.Descricao))
                viagem = repPedidoViagemNavio.BuscarPorDescricao(viagemIntegracao.Descricao);
            if (viagem == null && viagemIntegracao.CodigoIntegracao > 0 && !string.IsNullOrWhiteSpace(viagemIntegracao.CodigoIntegracao.ToString("D")))
                viagem = repPedidoViagemNavio.BuscarPorCodigoIntegracao(viagemIntegracao.CodigoIntegracao.ToString("D"));

            if (viagem != null)
            {
                if (viagemIntegracao.Atualizar)
                {
                    if (!string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoIntegracao))
                        navio = repNavio.BuscarPorCodigoIntegracao(viagemIntegracao.Navio.CodigoIntegracao);
                    if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoIRIN))
                        navio = repNavio.BuscarTodosPorCodigoIRIN(viagemIntegracao.Navio.CodigoIRIN);
                    if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.Descricao))
                        navio = repNavio.BuscarTodosPorDescricao(viagemIntegracao.Navio.Descricao);

                    if (navio != null)
                    {
                        descricaoViagem = navio.Descricao + "/" + viagemIntegracao.NumeroViagem.ToString("D") + Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(viagemIntegracao.Direcao);

                        viagem.Initialize();
                        viagem.Status = viagemIntegracao.InativarCadastro ? false : true;
                        viagem.Integrado = false;
                        viagem.CodigoIntegracao = viagemIntegracao.CodigoIntegracao.ToString("D");
                        viagem.Descricao = descricaoViagem;
                        viagem.DirecaoViagemMultimodal = viagemIntegracao.Direcao;
                        viagem.Navio = navio;
                        viagem.NumeroViagem = viagemIntegracao.NumeroViagem;
                        repPedidoViagemNavio.Atualizar(viagem, Auditado);
                    }
                }

                if (viagemIntegracao.Schedules != null && viagemIntegracao.Schedules.Count > 0)
                {
                    List<int> codigosRemover = new List<int>();
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Schedule schedule in viagemIntegracao.Schedules)
                    {
                        List<int> codigos = new List<int>();
                        SalvarSchedule(schedule, viagem, ref stMensagem, _unitOfWork.StringConexao, Auditado, out codigos, encerrarMDFeAutomaticamente);
                        codigosRemover.AddRange(codigos);
                    }

                    if (codigosRemover != null && codigosRemover.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> schedulesExcluir = repPedidoViagemNavioSchedule.BuscarSchedulesExcluir(codigosRemover, viagem.Codigo);
                        if (schedulesExcluir != null && schedulesExcluir.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule scheduleExcluir in schedulesExcluir)
                            {
                                scheduleExcluir.Initialize();
                                repPedidoViagemNavioSchedule.Deletar(scheduleExcluir, Auditado);
                            }
                        }
                    }
                }

                return viagem;
            }


            if (viagemIntegracao.Navio != null)
            {
                if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.Descricao))
                    navio = repNavio.BuscarTodosPorDescricao(viagemIntegracao.Navio.Descricao);
                if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoIntegracao))
                    navio = repNavio.BuscarPorCodigoIntegracao(viagemIntegracao.Navio.CodigoIntegracao);
                if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoIRIN))
                    navio = repNavio.BuscarTodosPorCodigoIRIN(viagemIntegracao.Navio.CodigoIRIN);
                if (navio == null && !string.IsNullOrWhiteSpace(viagemIntegracao.Navio.CodigoDocumento))
                    navio = repNavio.BuscarPorCodigoDocumento(viagemIntegracao.Navio.CodigoDocumento);

                if (navio == null)
                {
                    navio = new Dominio.Entidades.Embarcador.Pedidos.Navio()
                    {
                        CodigoEmbarcacao = viagemIntegracao.Navio.CodigoEmbarcacao,
                        CodigoIntegracao = viagemIntegracao.Navio.CodigoIntegracao,
                        Descricao = viagemIntegracao.Navio.Descricao,
                        Irin = viagemIntegracao.Navio.CodigoIRIN,
                        CodigoDocumento = viagemIntegracao.Navio.CodigoDocumento,
                        Status = viagemIntegracao.Navio.InativarCadastro ? false : true,
                        TipoEmbarcacao = viagemIntegracao.Navio.TipoEmbarcacao
                    };
                    repNavio.Inserir(navio, Auditado);
                }
            }
            if (navio == null)
            {
                stMensagem.Append("Dados do navio não informado para inserir a viagem; ");
                return null;
            }

            descricaoViagem = navio.Descricao + "/" + viagemIntegracao.NumeroViagem.ToString("D") + Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(viagemIntegracao.Direcao);
            viagem = repPedidoViagemNavio.BuscarPorDescricao(descricaoViagem);
            if (viagem != null)
            {
                return viagem;
                //stMensagem.Append("Já existe uma viagem cadastrada com esta descrição '" + viagem.Descricao + "' porem com outro código de integração: " + viagem.CodigoIntegracao + "; ");
                //return null;
            }

            if ((viagemIntegracao.Navio?.NavioIntegracaoBooking ?? false) && repPedidoViagemNavio.ExistePorCodigoIntegracao(viagemIntegracao.Navio?.CodigoIntegracao))
            {
                stMensagem.Append("Já existe uma viagem cadastrada com o Código de Integração '" + viagem.CodigoIntegracao + "'; ");
                return null;
            }

            viagem = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio()
            {
                CodigoIntegracao = viagemIntegracao.CodigoIntegracao.ToString("D"),
                Descricao = descricaoViagem,
                DirecaoViagemMultimodal = viagemIntegracao.Direcao,
                Navio = navio,
                NumeroViagem = viagemIntegracao.NumeroViagem,
                Status = viagemIntegracao.InativarCadastro ? false : true,
                Integrado = false
            };

            repPedidoViagemNavio.Inserir(viagem, Auditado);

            if (viagemIntegracao.Schedules != null && viagemIntegracao.Schedules.Count > 0)
            {
                List<int> codigosRemover = new List<int>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Schedule schedule in viagemIntegracao.Schedules)
                {
                    List<int> codigos = new List<int>();
                    SalvarSchedule(schedule, viagem, ref stMensagem, _unitOfWork.StringConexao, Auditado, out codigos, encerrarMDFeAutomaticamente);
                    codigosRemover.AddRange(codigos);
                }

                if (codigosRemover != null && codigosRemover.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> schedulesExcluir = repPedidoViagemNavioSchedule.BuscarSchedulesExcluir(codigosRemover, viagem.Codigo);
                    if (schedulesExcluir != null && schedulesExcluir.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule scheduleExcluir in schedulesExcluir)
                        {
                            repPedidoViagemNavioSchedule.Deletar(scheduleExcluir);
                        }
                    }
                }
            }

            return viagem;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        private bool PermitirUtilizarPedidoExistente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExiste, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            if ((pedidoExiste.PreCarga == null) || !(configuracaoTMS?.ValidarDestinatarioPedidoDiferentePreCarga ?? false))
                return true;

            bool destinatarioNaoAlterado = (
                (cargaIntegracao.Destinatario == null) ||
                (pedidoExiste.Destinatario == null) ||
                (pedidoExiste.Destinatario.CPF_CNPJ == cargaIntegracao.Destinatario.CPFCNPJ.ObterSomenteNumeros().ToDouble())
            );

            bool destinoNaoAlterado = (
                (cargaIntegracao.Destino?.Cidade == null) ||
                (pedidoExiste.Destino == null) ||
                (pedidoExiste.Destino.CodigoIBGE == cargaIntegracao.Destino.Cidade.IBGE)
            );

            if (!destinatarioNaoAlterado)
                pedidoExiste.PreCarga.ProblemaVincularCarga = $"Alteração de destinatário ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")}) | Pré Carga (CPF/CNPJ): {pedidoExiste.Destinatario.CPF_CNPJ_Formatado} | Carga (CPF/CNPJ): {cargaIntegracao.Destinatario.CPFCNPJ.ObterSomenteNumeros().ObterCpfOuCnpjFormatado()}.";
            else if (!destinoNaoAlterado)
                pedidoExiste.PreCarga.ProblemaVincularCarga = $"Alteração de destino ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")}) | Pré Carga (IBGE): {pedidoExiste.Destino.CodigoIBGE.ToString("n0")} | Carga (IBGE): {cargaIntegracao.Destino.Cidade.IBGE.ToString("n0")}.";
            else
                pedidoExiste.PreCarga.ProblemaVincularCarga = string.Empty;

            new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork).Atualizar(pedidoExiste.PreCarga);

            return (destinatarioNaoAlterado && destinoNaoAlterado);
        }

        private void VincularNotasFiscais(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem)
        {
            if (cargaIntegracao.NotasFiscais != null && cargaIntegracao.NotasFiscais.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.PedidoNotaParcial repPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(_unitOfWork);
                if (string.IsNullOrWhiteSpace(cargaIntegracao.NotasFiscais.FirstOrDefault().Chave))//apenas armazena notas parciais, ou seja, quando não tem a chave, ver se precisar armazenar mais dados.
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in cargaIntegracao.NotasFiscais)
                    {

                        Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcial = new Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial();
                        pedidoNotaParcial.Pedido = pedido;
                        pedidoNotaParcial.Numero = notaFiscal.Numero;
                        pedidoNotaParcial.NumeroPedido = notaFiscal.NumeroDT;
                        pedidoNotaParcial.DataCriacao = DateTime.Now;
                        repPedidoNotaParcial.Inserir(pedidoNotaParcial);
                    }
                }
            }
        }

        private void VincularAdicionaisPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            VincularPeriodoEntregaPeduido(ref pedido, cargaIntegracao);
            VincularProcessamentoEspecialPedido(ref pedido, cargaIntegracao);
            VincularDetalheEntregaPedido(ref pedido, cargaIntegracao);
            VincularHorarioEntregaPedido(ref pedido, cargaIntegracao);
            VincularZonaTransportePedido(ref pedido, cargaIntegracao);
            VincularDiasRestricaoPedido(ref pedido, cargaIntegracao);
            VincularDemaisInformacoesAdicionalPedido(ref pedido, cargaIntegracao, configuracaoWebService, Auditado);
        }

        private void VincularDemaisInformacoesAdicionalPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (
                string.IsNullOrEmpty(cargaIntegracao.NumeroPedidoICT) && string.IsNullOrEmpty(cargaIntegracao.CondicaoExpedicao) && string.IsNullOrEmpty(cargaIntegracao.GrupoFreteMaterial) &&
                string.IsNullOrEmpty(cargaIntegracao.RestricaoEntrega) && string.IsNullOrEmpty(cargaIntegracao.DataCriacaoRemessa) && string.IsNullOrEmpty(cargaIntegracao.DataCriacaoVenda) &&
                string.IsNullOrEmpty(cargaIntegracao.IndicadorPOF) && string.IsNullOrEmpty(cargaIntegracao.CodigoPedidoCliente) && string.IsNullOrEmpty(cargaIntegracao.NumeroOSMae) &&
                string.IsNullOrEmpty(cargaIntegracao.TipoSeguro) && string.IsNullOrEmpty(cargaIntegracao.NumeroAutorizacaoColetaEntrega) && string.IsNullOrEmpty(cargaIntegracao.TipoServico) &&
                !cargaIntegracao.ExecaoCab.HasValue && !cargaIntegracao.EssePedidopossuiPedidoBonificacao && !cargaIntegracao.EssePedidopossuiPedidoVenda && string.IsNullOrEmpty(cargaIntegracao.NumeroPedidoVinculado) &&
                !cargaIntegracao.ConversaoCargaPaletizada.HasValue && cargaIntegracao.SituacaoEstoque == null
            )
                return;

            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoChaveCTe repositorioPedidoChaveCTe = new Repositorio.Embarcador.Pedidos.PedidoChaveCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.SituacaoEstoquePedido repositorioSituacaoEstoquePedido = new Repositorio.Embarcador.Pedidos.SituacaoEstoquePedido(_unitOfWork);

            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);

            if (pedidoAdicional == null)
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };
            else
                pedidoAdicional.Initialize();

            pedidoAdicional.CondicaoExpedicao = cargaIntegracao.CondicaoExpedicao;
            pedidoAdicional.GrupoFreteMaterial = cargaIntegracao.GrupoFreteMaterial;
            pedidoAdicional.IndicadorPOF = cargaIntegracao.IndicadorPOF;
            pedidoAdicional.NumeroPedidoICT = cargaIntegracao.NumeroPedidoICT;
            pedidoAdicional.RestricaoEntrega = cargaIntegracao.RestricaoEntrega;
            pedidoAdicional.ProdutoVolumoso = cargaIntegracao.ProdutoVolumoso;
            pedidoAdicional.IndicativoColetaEntrega = cargaIntegracao.IndicativoColetaEntrega;
            pedidoAdicional.TipoServico = cargaIntegracao.TipoServico;
            pedidoAdicional.NumeroAutorizacaoColetaEntrega = cargaIntegracao.NumeroAutorizacaoColetaEntrega;
            pedidoAdicional.TipoSeguro = cargaIntegracao.TipoSeguro;
            pedidoAdicional.NumeroOSMae = cargaIntegracao.NumeroOSMae;
            pedidoAdicional.ExecaoCab = cargaIntegracao.ExecaoCab;
            pedidoAdicional.PedidoPaletizado = cargaIntegracao.ConversaoCargaPaletizada;

            if (cargaIntegracao.SituacaoEstoque != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido situacaoEstoquePedido = repositorioSituacaoEstoquePedido.BuscarPorCodigoIntegracao(cargaIntegracao.SituacaoEstoque.CodigoIntegracao);

                if (situacaoEstoquePedido == null)
                    throw new ServicoException("Situação de Estoque Pedido não encontrada.");

                pedidoAdicional.SituacaoEstoquePedido = situacaoEstoquePedido;
            }

            if (configuracaoWebService?.AtualizarNumeroPedidoVinculado ?? false)
            {
                pedidoAdicional.EssePedidopossuiPedidoBonificacao = cargaIntegracao?.EssePedidopossuiPedidoBonificacao ?? false;
                pedidoAdicional.EssePedidopossuiPedidoVenda = cargaIntegracao?.EssePedidopossuiPedidoVenda ?? false;
                pedidoAdicional.NumeroPedidoVinculado = cargaIntegracao?.NumeroPedidoVinculado ?? string.Empty;

                if (pedidoAdicional.EssePedidopossuiPedidoVenda && pedidoAdicional.EssePedidopossuiPedidoBonificacao)
                    throw new ServicoException("Não é Possível Selecionar Esse Pedido Possui Pedido de Bonificação E Esse Pedido Possui Pedido de Venda juntos. ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.CodigoPedidoCliente))
                pedido.CodigoPedidoCliente = cargaIntegracao.CodigoPedidoCliente;

            if (cargaIntegracao.ClientePropostaComercial != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoClientePropostaComercial = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.ClientePropostaComercial, "ClientePropostaComercial", _unitOfWork, 0, false);

                if (retornoClientePropostaComercial.Status)
                    pedidoAdicional.ClientePropostaComercial = retornoClientePropostaComercial.cliente;
            }
            else
                pedidoAdicional.ClientePropostaComercial = null;

            if (cargaIntegracao.ChavesCTes?.Count > 0)
            {
                if (pedido.Codigo > 0)
                    repositorioPedidoChaveCTe.DeletarPorPedido(pedido.Codigo);

                foreach (string chave in cargaIntegracao.ChavesCTes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe pedidoChave = new Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe()
                    {
                        ChaveCTe = chave,
                        Pedido = pedido
                    };

                    repositorioPedidoChaveCTe.Inserir(pedidoChave);
                }
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataCriacaoRemessa))
            {
                DateTime data;
                DateTime.TryParseExact(cargaIntegracao.DataCriacaoRemessa, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data);
                pedidoAdicional.DataCriacaoRemessa = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataCriacaoVenda))
            {
                DateTime data;
                DateTime.TryParseExact(cargaIntegracao.DataCriacaoVenda, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data);
                pedidoAdicional.DataCriacaoVenda = data;
            }

            if (pedidoAdicional.Codigo == 0)
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
            else
            {
                repositorioPedidoAdicional.Atualizar(pedidoAdicional, Auditado);
                pedido.SetExternalChanges(pedidoAdicional.GetCurrentChanges());
            }
        }

        private void VincularDetalheEntregaPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (string.IsNullOrEmpty(cargaIntegracao?.DetalheEntrega?.CodigoIntegracao ?? null))
                return;

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(cargaIntegracao.DetalheEntrega.CodigoIntegracao, TipoTipoDetalhe.DetalheEntrega);
            if (tipoDetalhe == null)
            {
                tipoDetalhe = new Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe();
                tipoDetalhe.CodigoIntegracao = cargaIntegracao.DetalheEntrega.CodigoIntegracao;
                tipoDetalhe.Descricao = cargaIntegracao.DetalheEntrega.Descricao;
                tipoDetalhe.Tipo = TipoTipoDetalhe.DetalheEntrega;
                repositorioTipoDetalhe.Inserir(tipoDetalhe);
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);
            if (pedidoAdicional == null)
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };

            pedidoAdicional.DetalheEntrega = tipoDetalhe;

            if (pedidoAdicional.Codigo == 0)
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
            else
                repositorioPedidoAdicional.Atualizar(pedidoAdicional);
        }

        private void VincularDiasRestricaoPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao?.DiasRestricaoEntrega == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoRestricaoDiaEntrega repositorioPedidoRestricaoDiaEntrega = new Repositorio.Embarcador.Pedidos.PedidoRestricaoDiaEntrega(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega> pedidoRestricaoDiasEntrega = repositorioPedidoRestricaoDiaEntrega.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia in cargaIntegracao.DiasRestricaoEntrega)
            {
                if (!pedidoRestricaoDiasEntrega.Any(x => x.Dia == dia))
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega pedidoRestricaoDiaEntrega = new Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega() { Pedido = pedido, Dia = dia };
                    repositorioPedidoRestricaoDiaEntrega.Inserir(pedidoRestricaoDiaEntrega);
                    pedidoRestricaoDiasEntrega.Add(pedidoRestricaoDiaEntrega);
                }
            }
        }

        private void VincularHorarioEntregaPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (string.IsNullOrEmpty(cargaIntegracao?.HorarioEntrega?.CodigoIntegracao ?? null))
                return;

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(cargaIntegracao.HorarioEntrega.CodigoIntegracao, TipoTipoDetalhe.HorarioEntrega);
            if (tipoDetalhe == null)
            {
                tipoDetalhe = new Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe();
                tipoDetalhe.CodigoIntegracao = cargaIntegracao.HorarioEntrega.CodigoIntegracao;
                tipoDetalhe.Descricao = cargaIntegracao.HorarioEntrega.Descricao;
                tipoDetalhe.Tipo = TipoTipoDetalhe.HorarioEntrega;
                repositorioTipoDetalhe.Inserir(tipoDetalhe);
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);
            if (pedidoAdicional == null)
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };

            pedidoAdicional.HorarioEntrega = tipoDetalhe;

            if (pedidoAdicional.Codigo == 0)
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
            else
                repositorioPedidoAdicional.Atualizar(pedidoAdicional);
        }

        private void VincularPeriodoEntregaPeduido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (string.IsNullOrEmpty(cargaIntegracao?.PeriodoEntrega?.CodigoIntegracao ?? null))
                return;

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(cargaIntegracao.PeriodoEntrega.CodigoIntegracao, TipoTipoDetalhe.PeriodoEntrega);
            if (tipoDetalhe == null)
            {
                tipoDetalhe = new Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe();
                tipoDetalhe.CodigoIntegracao = cargaIntegracao.PeriodoEntrega.CodigoIntegracao;
                tipoDetalhe.Descricao = cargaIntegracao.PeriodoEntrega.Descricao;
                tipoDetalhe.Tipo = TipoTipoDetalhe.PeriodoEntrega;
                repositorioTipoDetalhe.Inserir(tipoDetalhe);
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);
            if (pedidoAdicional == null)
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };

            pedidoAdicional.PeriodoEntrega = tipoDetalhe;

            if (pedidoAdicional.Codigo == 0)
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
            else
                repositorioPedidoAdicional.Atualizar(pedidoAdicional);
        }

        private void VincularProcessamentoEspecialPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (string.IsNullOrEmpty(cargaIntegracao?.ProcessamentoEspecial?.CodigoIntegracao ?? null))
                return;

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(cargaIntegracao.ProcessamentoEspecial.CodigoIntegracao, TipoTipoDetalhe.ProcessamentoEspecial);
            if (tipoDetalhe == null)
            {
                tipoDetalhe = new Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe();
                tipoDetalhe.CodigoIntegracao = cargaIntegracao.ProcessamentoEspecial.CodigoIntegracao;
                tipoDetalhe.Descricao = cargaIntegracao.ProcessamentoEspecial.Descricao;
                tipoDetalhe.Tipo = TipoTipoDetalhe.ProcessamentoEspecial;
                repositorioTipoDetalhe.Inserir(tipoDetalhe);
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);
            if (pedidoAdicional == null)
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };

            pedidoAdicional.ProcessamentoEspecial = tipoDetalhe;

            if (pedidoAdicional.Codigo == 0)
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
            else
                repositorioPedidoAdicional.Atualizar(pedidoAdicional);
        }

        private void VincularZonaTransportePedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (string.IsNullOrEmpty(cargaIntegracao?.ZonaTransporte?.CodigoIntegracao ?? null))
                return;

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(cargaIntegracao.ZonaTransporte.CodigoIntegracao, TipoTipoDetalhe.ZonaTransporte);
            if (tipoDetalhe == null)
            {
                tipoDetalhe = new Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe();
                tipoDetalhe.CodigoIntegracao = cargaIntegracao.ZonaTransporte.CodigoIntegracao;
                tipoDetalhe.Descricao = cargaIntegracao.ZonaTransporte.Descricao;
                tipoDetalhe.Tipo = TipoTipoDetalhe.ZonaTransporte;
                repositorioTipoDetalhe.Inserir(tipoDetalhe);
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);
            if (pedidoAdicional == null)
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };

            pedidoAdicional.ZonaTransporte = tipoDetalhe;

            if (pedidoAdicional.Codigo == 0)
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
            else
                repositorioPedidoAdicional.Atualizar(pedidoAdicional);
        }

        private void VincularAverbacoesPedidos(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem)
        {
            if (cargaIntegracao.Averbacao != null)
            {
                if (string.IsNullOrWhiteSpace(cargaIntegracao.Averbacao.CNPJResponsavel) || !Utilidades.Validate.ValidarCNPJ(cargaIntegracao.Averbacao.CNPJResponsavel))
                    stMensagem.Append("CNPJ do Responsável pela Averbação inválido (" + cargaIntegracao.Averbacao.CNPJResponsavel + ")");
                if (string.IsNullOrWhiteSpace(cargaIntegracao.Averbacao.CNPJSeguradora) || !Utilidades.Validate.ValidarCNPJ(cargaIntegracao.Averbacao.CNPJSeguradora))
                    stMensagem.Append("CNPJ da Seguradora inválido (" + cargaIntegracao.Averbacao.CNPJSeguradora + ")");
                if (string.IsNullOrWhiteSpace(cargaIntegracao.Averbacao.NomeSeguradora) || cargaIntegracao.Averbacao.NomeSeguradora.Length > 30)
                    stMensagem.Append("Nome da Seguradora inválido (" + cargaIntegracao.Averbacao.NomeSeguradora + ")");
                if (string.IsNullOrWhiteSpace(cargaIntegracao.Averbacao.NumeroApolice) || cargaIntegracao.Averbacao.NumeroApolice.Length > 20)
                    stMensagem.Append("Número da apólice de seguro inválido (" + cargaIntegracao.Averbacao.NumeroApolice + ")");
                if (string.IsNullOrWhiteSpace(cargaIntegracao.Averbacao.NumeroAverbacao) || cargaIntegracao.Averbacao.NumeroAverbacao.Length > 40)
                    stMensagem.Append("Número da averbação de seguro inválido (" + cargaIntegracao.Averbacao.NumeroAverbacao + ")");

                Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao pedidoAverbacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao();
                pedidoAverbacao.Pedido = pedido;
                pedidoAverbacao.CNPJResponsavel = Utilidades.String.OnlyNumbers(cargaIntegracao.Averbacao.CNPJResponsavel);
                pedidoAverbacao.CNPJSeguradora = Utilidades.String.OnlyNumbers(cargaIntegracao.Averbacao.CNPJSeguradora);
                pedidoAverbacao.NomeSeguradora = cargaIntegracao.Averbacao.NomeSeguradora;
                pedidoAverbacao.NumeroApolice = cargaIntegracao.Averbacao.NumeroApolice;
                pedidoAverbacao.NumeroAverbacao = cargaIntegracao.Averbacao.NumeroAverbacao;
                repPedidoAverbacao.Inserir(pedidoAverbacao);
            }
        }

        private void InformarValorFretePedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(_unitOfWork);
            Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(_unitOfWork);

            decimal valorCotacao = 0;
            if (cargaIntegracao != null && cargaIntegracao.Remetente != null)
            {
                valorCotacao = cargaIntegracao.ValorTaxaFeeder;
                //double cnpjRemetente = 0;
                //double.TryParse(cargaIntegracao.Remetente.CPFCNPJ, out cnpjRemetente);
                //if (cnpjRemetente > 0)
                //    valorCotacao = serCotacao.BuscarValorCotacaoCliente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, unitOfWork, cnpjRemetente, cargaIntegracao.ValorTaxaFeeder);
            }

            if (cargaIntegracao.ValorFrete != null)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaIntegracao.ValorFrete.FreteProprio > 0)
                {
                    pedido.ValorFreteNegociado = valorCotacao > 0 ? (cargaIntegracao.ValorFrete.FreteProprio * valorCotacao) : cargaIntegracao.ValorFrete.FreteProprio;
                    repPedido.Atualizar(pedido);
                }

                if (cargaIntegracao.ValorFrete.ComponentesAdicionais != null)
                {
                    Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
                    repPedidoComponenteFrete.DeletarPorPedido(pedido.Codigo);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in cargaIntegracao.ValorFrete.ComponentesAdicionais)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);

                        if (componenteFrete == null && !string.IsNullOrWhiteSpace(componenteAdicional.Componente.Descricao))
                            componenteFrete = repComponenteFrete.BuscarPorDescricao(componenteAdicional.Componente.Descricao);

                        if (componenteFrete != null)
                        {
                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                pedidoComponenteFrete.ComponenteFrete = componenteFrete;
                                pedidoComponenteFrete.Pedido = pedido;
                                pedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                                pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                pedidoComponenteFrete.DescontarValorTotalAReceber = componenteAdicional.DescontarValorTotalAReceber;
                                pedidoComponenteFrete.TipoComponenteFrete = pedidoComponenteFrete.ComponenteFrete.TipoComponenteFrete;

                                if (pedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                                {
                                    pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                                    pedidoComponenteFrete.Percentual = componenteAdicional.ValorComponente;
                                    pedidoComponenteFrete.ValorComponente = 0;
                                }
                                else
                                {
                                    pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                                    pedidoComponenteFrete.ValorComponente = valorCotacao > 0 ? (componenteAdicional.ValorComponente * valorCotacao) : componenteAdicional.ValorComponente;
                                }
                                repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                            }
                        }
                        else
                        {
                            stMensagem.Append("O código informado para o componente de frete (" + componenteAdicional.Componente.CodigoIntegracao + ") não existe na base da Multisoftware.");
                        }
                    }
                }

            }
        }

        private void PreencherDatasPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem)
        {
            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataPrevisaoEntrega))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataPrevisaoEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisao))
                    stMensagem.Append("A data de previsão não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.PrevisaoEntrega = dataPrevisao;
                pedido.DataInicioJanelaDescarga = dataPrevisao;
                pedido.DataFinalViagemExecutada = dataPrevisao;
                pedido.DataFinalViagemFaturada = dataPrevisao;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataValidade))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataValidade, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataValidade))
                    stMensagem.Append("A data de validade não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataValidade = dataValidade;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.PrevisaoEntregaTransportador))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.PrevisaoEntregaTransportador, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime previsaoEntregaTransportador))
                    stMensagem.Append("A data de previsão não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.PrevisaoEntregaTransportador = previsaoEntregaTransportador;
            }


            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataInicioCarregamento))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataInicioCarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data inicial de previsão de carregamento não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataInicialColeta = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataColeta))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataColeta, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataColeta))
                    stMensagem.Append("A data de coleta não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataFinalColeta = dataColeta;
                if (!pedido.DataInicialColeta.HasValue)
                    pedido.DataInicialColeta = pedido.DataFinalColeta;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataFinalCarregamento))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataFinalCarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data final de previsão de carregamento não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataInicialViagemExecutada = data;
                pedido.DataInicialViagemFaturada = data;
            }

            if (pedido.DataInicialColeta != null)
                pedido.DataCarregamentoPedido = pedido.DataInicialColeta;
            else
                pedido.DataCarregamentoPedido = null;

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.ETA))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.ETA, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data ETA não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataETA = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.ETS))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.ETS, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data ETS não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataETS = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataPrevisao))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataPrevisao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data de previsão de carregamento não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataPrevisaoSaida = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataInclusaoPCP))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataInclusaoPCP, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data de inclusão do PCP não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataInclusaoPCP = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataInclusaoBooking))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataInclusaoBooking, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data de inclusão do booking não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataInclusaoBooking = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataAgendamento))
            {
                DateTime? dataAgendamento = cargaIntegracao.DataAgendamento.ToNullableDateTime();

                if (!dataAgendamento.HasValue)
                    stMensagem.Append("A data de agendamento não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataAgendamento = dataAgendamento;
                pedido.OrigemCriacaoDataAgendamentoPedido = OrigemCriacao.WebService;

            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataTerminoCarregamento))
            {
                DateTime? dataTerminoCarregamento = cargaIntegracao.DataTerminoCarregamento.ToNullableDateTime();

                if (!dataTerminoCarregamento.HasValue)
                    stMensagem.Append("A data de término do carregamento não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                if (!(pedido.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false))
                    pedido.DataTerminoCarregamento = dataTerminoCarregamento;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataEstufagem))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataEstufagem, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data ETS não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataEstufagem = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataPrevisaoInicioViagem))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataPrevisaoInicioViagem, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data prevista para o início da viagem não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataPrevisaoInicioViagem = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataPrevisaoChegadaDestinatario))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataPrevisaoChegadaDestinatario, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data prevista para chegada no destinatário não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                pedido.DataPrevisaoChegadaDestinatario = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataUltimaLiberacao))
            {
                if (!DateTime.TryParseExact(cargaIntegracao.DataUltimaLiberacao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A data da última liberação não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                pedido.DataUltimaLiberacao = data;
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto> ObterGruposProdutoIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto> gruposProdutos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto>();

            if (cargaIntegracao.Produtos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in cargaIntegracao.Produtos)
                {
                    bool naoTemGrupoProduto = string.IsNullOrWhiteSpace(produtocargaIntegracao.CodigoGrupoProduto) || string.IsNullOrWhiteSpace(produtocargaIntegracao.DescricaoGrupoProduto);
                    if (naoTemGrupoProduto && configuracaoCargaIntegracao?.AceitarPedidosComPendenciasDeProdutos != true)
                    {
                        throw new ServicoException("Grupo de Produto não informado; ");
                    }

                    if (naoTemGrupoProduto && configuracaoCargaIntegracao?.AceitarPedidosComPendenciasDeProdutos == true)
                        continue;

                    if (!gruposProdutos.Any(obj => obj.Codigo == produtocargaIntegracao.CodigoGrupoProduto))
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto grupoProduto = new Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto();
                        grupoProduto.Codigo = produtocargaIntegracao.CodigoGrupoProduto;
                        grupoProduto.Descricao = produtocargaIntegracao.DescricaoGrupoProduto;
                        gruposProdutos.Add(grupoProduto);
                    }
                }
            }

            return gruposProdutos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao> ObterLinhaSeparacaoIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao> linhaSeparacaos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao>();

            if (cargaIntegracao.Produtos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in cargaIntegracao.Produtos)
                {
                    if (produtocargaIntegracao.LinhaSeparacao == null || string.IsNullOrWhiteSpace(produtocargaIntegracao.LinhaSeparacao.CodigoIntegracao) || string.IsNullOrWhiteSpace(produtocargaIntegracao.LinhaSeparacao.Descricao))
                        continue;

                    if (!linhaSeparacaos.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.LinhaSeparacao.CodigoIntegracao))
                        linhaSeparacaos.Add(produtocargaIntegracao.LinhaSeparacao);
                }
            }

            return linhaSeparacaos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto> ObterEnderecosProdutosIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto> enderecoProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto>();

            if (cargaIntegracao.Produtos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in cargaIntegracao.Produtos)
                {
                    if (produtocargaIntegracao.EnderecoProduto == null || string.IsNullOrWhiteSpace(produtocargaIntegracao.EnderecoProduto?.CodigoIntegracao ?? "") || string.IsNullOrWhiteSpace(produtocargaIntegracao.EnderecoProduto?.Descricao ?? ""))
                        continue;

                    if (!enderecoProdutos.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.EnderecoProduto.CodigoIntegracao))
                        enderecoProdutos.Add(produtocargaIntegracao.EnderecoProduto);
                }
            }

            return enderecoProdutos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto> ObterMarcaProdutoIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto> marcas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto>();

            if (cargaIntegracao.Produtos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in cargaIntegracao.Produtos)
                {
                    if (produtocargaIntegracao.MarcaProduto == null || string.IsNullOrWhiteSpace(produtocargaIntegracao.MarcaProduto.CodigoIntegracao) || string.IsNullOrWhiteSpace(produtocargaIntegracao.MarcaProduto.Descricao))
                        continue;

                    if (!marcas.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.MarcaProduto.CodigoIntegracao))
                        marcas.Add(produtocargaIntegracao.MarcaProduto);
                }
            }

            return marcas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem> ObterTiposEmbalagemIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem> tipoEmbalagens = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem>();

            if (cargaIntegracao.Produtos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in cargaIntegracao.Produtos)
                {
                    if (produtocargaIntegracao.TipoEmbalagem == null || string.IsNullOrWhiteSpace(produtocargaIntegracao.TipoEmbalagem.CodigoIntegracao) || string.IsNullOrWhiteSpace(produtocargaIntegracao.TipoEmbalagem.Descricao))
                        continue;

                    if (!tipoEmbalagens.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.TipoEmbalagem.CodigoIntegracao))
                        tipoEmbalagens.Add(produtocargaIntegracao.TipoEmbalagem);
                }
            }

            return tipoEmbalagens;
        }

        private void AtualizarProdutoPedidos(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (cargaIntegracao.Produtos == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
            ProdutoEmbarcador servicoProdutoEmbarcador = new ProdutoEmbarcador(_unitOfWork, configuracaoGeralCarga);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProdutoLote repositorioPedidoProdutoLote = new Repositorio.Embarcador.Pedidos.PedidoProdutoLote(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto> gruposProdutosIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao> linhaSeparacaoIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao>();
            List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto> enderecosProdutosIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem> tipoEmbalagensIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto> marcas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto>();

            try
            {
                gruposProdutosIntegracao = ObterGruposProdutoIntegracao(cargaIntegracao, configuracaoCargaIntegracao);
                linhaSeparacaoIntegracao = ObterLinhaSeparacaoIntegracao(cargaIntegracao);
                enderecosProdutosIntegracao = ObterEnderecosProdutosIntegracao(cargaIntegracao);
                marcas = ObterMarcaProdutoIntegracao(cargaIntegracao);
                tipoEmbalagensIntegracao = ObterTiposEmbalagemIntegracao(cargaIntegracao);
            }
            catch (ServicoException excecao)
            {
                stMensagem.Append(excecao.Message);
                return;
            }

            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto = servicoProdutoEmbarcador.IntegrarGruposProduto(gruposProdutosIntegracao);
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacao = servicoProdutoEmbarcador.IntegrarLinhasSeparacao(linhaSeparacaoIntegracao, Auditado);
            List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> enderecosProdutos = servicoProdutoEmbarcador.IntegrarEnderecosProdutos(enderecosProdutosIntegracao, Auditado);
            List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tiposEmbalagem = servicoProdutoEmbarcador.IntegrarTiposEmbalagem(tipoEmbalagensIntegracao);
            List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcaProdutos = servicoProdutoEmbarcador.IntegrarMarcasProduto(marcas, Auditado);
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            bool utilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false;

            if (!configuracao.SempreInserirNovoProdutoPorIntegracao)
                produtos = repProdutoEmbarcador.buscarPorCodigosEmbarcador((from obj in cargaIntegracao.Produtos select obj.CodigoProduto).Distinct().ToList());

            pedido.ValorTotalNotasFiscais = cargaIntegracao.ValorTotalPedido;

            if (cargaIntegracao.CubagemTotal == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                pedido.CubagemTotal = 0;

            if (utilizarPesoProdutoParaCalcularPesoCarga)
            {
                pedido.PesoTotal = 0;
                pedido.PesoLiquidoTotal = 0;
            }
            else if (cargaIntegracao.PesoBruto == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                pedido.PesoTotal = 0;

            List<int> codigosPedidoProduto = new List<int>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in cargaIntegracao.Produtos)
            {
                if (produtocargaIntegracao == null)
                {
                    stMensagem.Append("Produto informado é Nulo; ");
                    return;
                }

                if (string.IsNullOrWhiteSpace(produtocargaIntegracao.CodigoProduto))
                {
                    stMensagem.Append("Produto não informado; ");
                    return;
                }

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = (from obj in gruposProduto where obj.CodigoGrupoProdutoEmbarcador == produtocargaIntegracao.CodigoGrupoProduto select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = null;
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = null;

                if (produtocargaIntegracao.LinhaSeparacao != null) linhaSeparacao = (from obj in linhasSeparacao where obj.CodigoIntegracao == produtocargaIntegracao.LinhaSeparacao.CodigoIntegracao select obj).FirstOrDefault();
                if (produtocargaIntegracao.TipoEmbalagem != null) tipoEmbalagem = (from obj in tiposEmbalagem where obj.CodigoIntegracao == produtocargaIntegracao.TipoEmbalagem.CodigoIntegracao select obj).FirstOrDefault();
                if (produtocargaIntegracao.MarcaProduto != null) marcaProduto = (from obj in marcaProdutos where obj.CodigoIntegracao == produtocargaIntegracao.MarcaProduto.CodigoIntegracao select obj).FirstOrDefault();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = servicoProdutoEmbarcador.IntegrarProduto(
                    produtos,
                    configuracao,
                    produtocargaIntegracao.CodigoProduto,
                    produtocargaIntegracao.DescricaoProduto,
                    produtocargaIntegracao.PesoUnitario,
                    grupoProduto,
                    produtocargaIntegracao.MetroCubito,
                    Auditado,
                    produtocargaIntegracao.CodigoDocumentacao,
                    produtocargaIntegracao.Atualizar,
                    produtocargaIntegracao.CodigoNCM,
                    produtocargaIntegracao.QuantidadePorCaixa,
                    produtocargaIntegracao.QuantidadeCaixaPorPallet,
                    produtocargaIntegracao.Altura,
                    produtocargaIntegracao.Largura,
                    produtocargaIntegracao.Comprimento,
                    linhaSeparacao,
                    tipoEmbalagem,
                    marcaProduto,
                    produtocargaIntegracao.UnidadeMedida,
                    produtocargaIntegracao.Observacao,
                    "",
                    produtocargaIntegracao?.CodigoEAN ?? string.Empty
                );

                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repPedidoProduto.BuscarPorPedidoProduto(pedido.Codigo, produto.Codigo);

                if (pedidoProduto == null)
                {
                    pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto
                    {
                        Pedido = pedido,
                        Produto = produto
                    };
                }

                pedidoProduto.Observacao = pedidoProduto.Produto.Observacao;
                pedidoProduto.ObservacaoCarga = produtocargaIntegracao.ObservacaoCarga;
                pedidoProduto.ValorProduto = produtocargaIntegracao.ValorUnitario;
                pedidoProduto.QuantidadeEmbalagem = produtocargaIntegracao.QuantidadeEmbalagem;
                pedidoProduto.PesoTotalEmbalagem = produtocargaIntegracao.PesoTotalEmbalagem;
                pedidoProduto.Quantidade = produtocargaIntegracao.Quantidade;
                pedidoProduto.QuantidadePlanejada = produtocargaIntegracao.QuantidadePlanejada;
                pedidoProduto.PesoUnitario = (produtocargaIntegracao.Atualizar || utilizarPesoProdutoParaCalcularPesoCarga) ? produto.PesoUnitario : produtocargaIntegracao.PesoUnitario;
                pedidoProduto.PalletFechado = produtocargaIntegracao.PalletFechado;
                pedidoProduto.MetroCubico = configuracao.MetroCubicoPorUnidadePedidoProdutoIntegracao ? (produto.MetroCubito * produtocargaIntegracao.Quantidade) : produto.MetroCubito;
                pedidoProduto.SetorLogistica = produtocargaIntegracao.SetorLogistica;
                pedidoProduto.ClasseLogistica = produtocargaIntegracao.ClasseLogistica;
                pedidoProduto.QuantidadePalet = produtocargaIntegracao.QuantidadePallet;
                pedidoProduto.CanalDistribuicao = produtocargaIntegracao.CanalDistribuicao;
                pedidoProduto.SiglaModalidade = produtocargaIntegracao.SiglaModalidade;
                pedidoProduto.AlturaCM = produto.AlturaCM;
                pedidoProduto.LarguraCM = produto.LarguraCM;
                pedidoProduto.ComprimentoCM = produto.ComprimentoCM;
                pedidoProduto.QuantidadeCaixaPorPallet = produto.QuantidadeCaixaPorPallet;
                pedidoProduto.QuantidadeCaixa = produto.QuantidadeCaixa;
                pedidoProduto.LinhaSeparacao = produto.LinhaSeparacao;
                pedidoProduto.TipoEmbalagem = produto.TipoEmbalagem;
                pedidoProduto.Canal = produtocargaIntegracao.Canal;
                pedidoProduto.CodigoOrganizacao = produtocargaIntegracao.CodigoOrganizacao;
                pedidoProduto.Setor = produtocargaIntegracao.Setor;
                pedidoProduto.PesoUnitarioProduto = produtocargaIntegracao.PesoUnitario;
                pedidoProduto.CodigoProduto = produtocargaIntegracao.CodigoProduto;

                if (pedidoProduto.QuantidadePalet == 0 && pedidoProduto.QuantidadeCaixaPorPallet > 0 && pedidoProduto.QuantidadeCaixa > 0)
                {
                    decimal caixas = Math.Ceiling(pedidoProduto.Quantidade / pedidoProduto.QuantidadeCaixa);
                    pedidoProduto.QuantidadePalet = Math.Round((caixas / pedidoProduto.QuantidadeCaixaPorPallet), 4, MidpointRounding.ToEven);
                }

                pedido.ValorTotalNotasFiscais += (pedidoProduto.ValorProduto * pedidoProduto.Quantidade);
                pedido.MaiorAlturaProdutoEmCentimetros = Math.Max(pedido.MaiorAlturaProdutoEmCentimetros, pedidoProduto.AlturaCM);
                pedido.MaiorLarguraProdutoEmCentimetros = Math.Max(pedido.MaiorLarguraProdutoEmCentimetros, pedidoProduto.LarguraCM);
                pedido.MaiorComprimentoProdutoEmCentimetros = Math.Max(pedido.MaiorComprimentoProdutoEmCentimetros, pedidoProduto.ComprimentoCM);
                pedido.MaiorVolumeProdutoEmCentimetros = Math.Max(pedido.MaiorVolumeProdutoEmCentimetros, (pedidoProduto.AlturaCM + pedidoProduto.LarguraCM + pedidoProduto.ComprimentoCM));

                if (cargaIntegracao.CubagemTotal == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                    pedido.CubagemTotal += pedidoProduto.MetroCubico;

                if (utilizarPesoProdutoParaCalcularPesoCarga)
                {
                    pedido.PesoTotal += pedidoProduto.PesoProduto;
                    pedido.PesoLiquidoTotal += pedidoProduto.PesoLiquidoProduto;
                }
                else if (cargaIntegracao.PesoBruto == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                    pedido.PesoTotal += pedidoProduto.PesoTotal;

                if (cargaIntegracao.NumeroPaletesFracionado == 0)
                    pedido.NumeroPaletesFracionado += pedidoProduto.QuantidadePalet;

                //TODO: Ver ASSAI, os cabeças mandam a quantidade de caixas, sendo que precisamos da quantidade unitária, 
                // então vamos multiplicar o peso unitário pela quantidade por Caixa para manter o peso unitário da caixa
                // para que o processo de Roteriização evite de quebrar uma caixa em 2 carregamentos...
                // e o Peso da embalagem, estamos recebendo o peso total da caixa.. vamos igrnoprar
                if (configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao == 1000000)
                {
                    pedidoProduto.PesoUnitario = (pedidoProduto.Produto.PesoUnitario * pedidoProduto.Produto.QuantidadeCaixa);
                    pedidoProduto.PesoTotalEmbalagem = 0;
                }

                if (pedidoProduto.Codigo == 0)
                    repPedidoProduto.Inserir(pedidoProduto);
                else
                    repPedidoProduto.Atualizar(pedidoProduto);

                //if (cargaIntegracao.Produtos?.Count > 0) 
                //{
                //    foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoCargaIntegracao in cargaIntegracao.Produtos) WHAAT?
                //    {
                if (produtocargaIntegracao.ProdutoLotes != null)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Lote produtoLote in produtocargaIntegracao.ProdutoLotes)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote pedidoProdutoLote = new Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote();

                        pedidoProdutoLote.PedidoProduto = pedidoProduto;
                        pedidoProdutoLote.NumeroLote = produtoLote.NumeroLote;
                        pedidoProdutoLote.DataFabricacao = produtoLote.DataFabricacao;
                        pedidoProdutoLote.DataValidade = produtoLote.DataValidade;
                        pedidoProdutoLote.Quantidade = produtoLote.QuantidadeLote;

                        repositorioPedidoProdutoLote.Inserir(pedidoProdutoLote);
                    }
                }
                //    }
                //}

                codigosPedidoProduto.Add(pedidoProduto.Codigo);
            }

            RemoverPedidoProdutosNaoIntegrados(pedido, codigosPedidoProduto);

            Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork);
            servicoMontagemCarga.AtualizarSituacaoExigeIscaPorPedido(pedido);
        }

        private void RemoverPedidoProdutosNaoIntegrados(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<int> codigosPedidoProduto)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProdutoLote repositorioPedidoProdutoLote = new Repositorio.Embarcador.Pedidos.PedidoProdutoLote(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidoDeletar = repPedidoProduto.BuscarProdutosNaoIntegrados(pedido.Codigo, codigosPedidoProduto);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote> pedidoProdutosLotesDeletar = repositorioPedidoProdutoLote.BuscarPedidoProdutosLotesPorPedidoProduto(codigosPedidoProduto);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPedido in produtosPedidoDeletar)
            {
                repCarregamentoPedidoProduto.DeletarCarregamentoPedidoProdutoPorCodigoPedidoProdutoViaQuery(produtoPedido.Codigo);
                repPedidoProduto.Deletar(produtoPedido);

                if (pedidoProdutosLotesDeletar.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote> entidadePedidoProdutosLotesDeletar = pedidoProdutosLotesDeletar.Where(obj => obj.PedidoProduto.Codigo == produtoPedido.Codigo).ToList();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote deletarPedidoProdutoLote in entidadePedidoProdutosLotesDeletar)
                        repositorioPedidoProdutoLote.Deletar(deletarPedidoProdutoLote);
                }
            }
        }

        private void InformarDadosPedidoEmbarcador(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool atualizarPedido, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool encerrarMDFeAutomaticamente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = null, string adminStringConexao = "", bool incrementarSequencial = false)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repositorioSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

            RemoverDadosEssencaisDoPedido(ref cargaIntegracao);

            if (string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPedidoEmbarcador))
                cargaIntegracao.NumeroPedidoEmbarcador = "";

            if (cargaIntegracao.NumeroPedidoEmbarcador.Length <= 50)
            {
                if (!atualizarPedido)
                    pedido.Numero = repositorioPedido.BuscarProximoNumero();

                if (!cargaIntegracao.NaoAtualizarDadosDoPedido)
                {
                    pedido.NumeroPedidoEmbarcador = cargaIntegracao.NumeroPedidoEmbarcador;
                    pedido.NumeroPedidoEmbarcadorSemRegra = cargaIntegracao.NumeroPedidoEmbarcadorSemRegra;
                    pedido.RegraMontarNumeroPedidoEmbarcadorWebService = cargaIntegracao.RegraMontarNumeroPedidoEmbarcadorWebService;
                }

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga))
                    pedido.CodigoCargaEmbarcador = cargaIntegracao.NumeroCarga;

                if (!cargaIntegracao.CargaDePreCarga && pedido.PedidoDePreCarga)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                    if (!repositorioCargaPedido.ExisteCargaDePreCarga(pedido.Codigo))
                        pedido.PedidoDePreCarga = false;
                }
                else
                    pedido.PedidoDePreCarga = cargaIntegracao.CargaDePreCarga;

                if (!pedido.AdicionadaManualmente && incrementarSequencial && pedido.NumeroSequenciaPedido == 0 && int.TryParse(pedido.NumeroPedidoEmbarcador, out int numeroSequencial))
                {
                    pedido.NumeroSequenciaPedido = numeroSequencial;
                }
                pedido.NumeroPaletes = cargaIntegracao.NumeroPaletes;
                pedido.NumeroPaletesFracionado = cargaIntegracao.NumeroPaletesFracionado;
                pedido.PalletSaldoRestante = (pedido.NumeroPaletes + pedido.NumeroPaletesFracionado);
                pedido.NumeroPaletesPagos = cargaIntegracao.NumeroPaletesPagos;
                pedido.NumeroSemiPaletes = cargaIntegracao.NumeroSemiPaletes;
                pedido.NumeroSemiPaletesPagos = cargaIntegracao.NumeroSemiPaletesPagos;
                pedido.NumeroCombis = cargaIntegracao.NumeroCombis;
                pedido.NumeroCombisPagas = cargaIntegracao.NumeroCombisPagas;
                pedido.QtVolumes = cargaIntegracao.QuantidadeVolumes;
                pedido.SaldoVolumesRestante = cargaIntegracao.QuantidadeVolumes;
                pedido.PesoTotalPaletes = cargaIntegracao.PesoTotalPaletes;
                pedido.PesoTotal = cargaIntegracao.PesoBruto;
                pedido.PesoLiquidoTotal = cargaIntegracao.PesoLiquido;
                pedido.Adicional1 = cargaIntegracao.Adicional1;
                pedido.Adicional2 = cargaIntegracao.Adicional2;
                pedido.Adicional3 = cargaIntegracao.Adicional3;
                pedido.Adicional4 = cargaIntegracao.Adicional4;
                pedido.Adicional5 = cargaIntegracao.Adicional5;
                pedido.Adicional6 = cargaIntegracao.Adicional6;
                pedido.Adicional7 = cargaIntegracao.Adicional7;
                pedido.QuebraMultiplosCarregamentos = cargaIntegracao.PermiteQuebraPedidoMultiplosCarregamentos;
                pedido.UsuarioCriacaoRemessa = cargaIntegracao.UsuarioCriacaoRemessa;
                pedido.NumeroOrdem = cargaIntegracao.NumeroOrdem;
                pedido.ValorTotalNotasFiscais = cargaIntegracao.ValorTotalPedido;
                pedido.PedidoBloqueado = cargaIntegracao.PedidoBloqueado;
                pedido.DataDeCriacaoPedidoERP = cargaIntegracao.DataCriacaoPedidoERP.ToNullableDateTime();
                pedido.TipoPaleteCliente = cargaIntegracao.TipoPaleteCliente;
                pedido.CategoriaOS = cargaIntegracao.CategoriaOS;
                pedido.NecessariaAverbacao = cargaIntegracao.NecessariaAverbacao == "Sim" ? true : false;
                pedido.DocumentoProvedor = cargaIntegracao.DocumentoProvedor;
                pedido.ValorTotalProvedor = cargaIntegracao.ValorTotalProvedor;
                pedido.LiberarPagamento = cargaIntegracao.LiberarPagamento == "Sim" ? true : false;
                pedido.TipoOS = cargaIntegracao.TipoOS;
                pedido.TipoOSConvertido = cargaIntegracao.TipoOSConvertido;
                pedido.DirecionamentoOS = cargaIntegracao.DirecionamentoOS;
                pedido.TipoServicoXML = cargaIntegracao.TipoServicoXML;
                pedido.IndicLiberacaoOk = cargaIntegracao.IndicLiberacaoOk;

                if (pedido.CubagemTotal == 0 && cargaIntegracao.Produtos?.Count > 0)
                {
                    // Se esta configuração está marcada.. já salva no pedido produto o metro cúbico * qtde... senão só salva o m3 do produto embarcador..(copia)
                    if (!configuracao?.MetroCubicoPorUnidadePedidoProdutoIntegracao ?? false)
                        pedido.CubagemTotal = (from obj in cargaIntegracao.Produtos select obj.MetroCubito * obj.Quantidade).Sum();
                    else
                        pedido.CubagemTotal = (from obj in cargaIntegracao.Produtos select obj.MetroCubito).Sum();
                }

                if (pedido.QtVolumes == 0)
                {
                    if (pedido.CotacaoPedido != null && ((cargaIntegracao.NotasFiscais?.Any(obj => obj.VolumesTotal > 0)) ?? false))
                        pedido.QtVolumes = (int)cargaIntegracao.NotasFiscais.Sum(obj => obj.VolumesTotal);
                    else if (cargaIntegracao.Produtos?.Count > 0)
                    {
                        if (configuracao?.NaoConsiderarProdutosSemPesoParaSumarizarVolumes ?? false)
                            pedido.QtVolumes = (int)(from obj in cargaIntegracao.Produtos where obj.PesoUnitario > 0 select obj.Quantidade).Sum();
                        else
                            pedido.QtVolumes = (int)(from obj in cargaIntegracao.Produtos select obj.Quantidade).Sum();

                        pedido.SaldoVolumesRestante = (int)(from obj in cargaIntegracao.Produtos select obj.Quantidade).Sum();
                    }

                }

                pedido.PesoSaldoRestante = cargaIntegracao.PesoBruto;
                pedido.QtdEntregas = 1;
                pedido.ValorTotalPaletes = cargaIntegracao.ValorTotalPaletes;
                pedido.CubagemTotal = cargaIntegracao.CubagemTotal;

                pedido.TipoPagamento = cargaIntegracao.TipoPagamento;

                pedido.TipoTomador = cargaIntegracao.TipoTomador;
                pedido.UsarTipoTomadorPedido = cargaIntegracao.UtilizarTipoTomadorInformado;

                if (pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros || pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                {
                    pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    pedido.UsarTipoTomadorPedido = true;
                    if (pedido.Tomador == null)
                    {
                        pedido.UsarTipoTomadorPedido = false;
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    }
                }

                pedido.Observacao = cargaIntegracao.Observacao;

                if (!string.IsNullOrWhiteSpace(configuracao?.ObservacaoCTePadraoEmbarcador))
                    pedido.ObservacaoCTe = string.Concat(configuracao.ObservacaoCTePadraoEmbarcador, " / ", cargaIntegracao.ObservacaoCTe);
                else
                    pedido.ObservacaoCTe = cargaIntegracao.ObservacaoCTe;

                pedido.ObservacaoInterna = cargaIntegracao.ObservacaoInterna;
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal;
                pedido.Temperatura = cargaIntegracao.Temperatura;
                pedido.Vendedor = cargaIntegracao.Vendedor;
                pedido.Ordem = cargaIntegracao.Ordem;
                pedido.OrdemColetaProgramada = cargaIntegracao.OrdemColetaProgramada;
                pedido.PortoSaida = cargaIntegracao.PortoSaida;
                pedido.PortoChegada = cargaIntegracao.PortoChegada;
                pedido.Companhia = cargaIntegracao.Companhia;
                pedido.NumeroNavio = cargaIntegracao.Navio;
                pedido.Reserva = cargaIntegracao.Reserva;
                pedido.Resumo = cargaIntegracao.Resumo;
                pedido.TipoEmbarque = cargaIntegracao.TipoEmbarque;
                pedido.ValorFreteCobradoCliente = cargaIntegracao.ValorFreteCobradoCliente;
                pedido.ValorCustoFrete = cargaIntegracao.ValorCustoFrete;
                pedido.DeliveryTerm = cargaIntegracao.DeliveryTerm;
                pedido.IdAutorizacao = cargaIntegracao.IdAutorizacao;
                pedido.PossuiGenset = cargaIntegracao.PossuiGenset;
                pedido.TipoPedido = cargaIntegracao.TipoPedido.HasValue ? cargaIntegracao.TipoPedido.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega;
                pedido.PedidoTrocaNota = cargaIntegracao.PedidoTrocaNota;
                pedido.NumeroPedidoTrocaNota = cargaIntegracao.NumeroPedidoTrocaNota;
                pedido.CIOT = cargaIntegracao.NumeroCIOT;
                pedido.NaoGlobalizarPedido = cargaIntegracao.NaoGlobalizarPedido;
                pedido.ValorFreteInformativo = cargaIntegracao.ValorFreteInformativo;
                pedido.AntecipacaoICMS = cargaIntegracao.AntecipacaoICMS;
                pedido.DiasItinerario = cargaIntegracao.DiasItinerario;
                pedido.DiasUteisPrazoTransportador = cargaIntegracao.DiasUteisPrazoTransportador;
                pedido.CodigoPedidoCliente = cargaIntegracao.NumeroPedidoCliente;
                pedido.IDLoteTrizy = cargaIntegracao.IDLoteTrizy;
                pedido.IDPropostaTrizy = cargaIntegracao.IDPropostaTrizy;
                pedido.PedidoDeDevolucao = cargaIntegracao.PedidoDeDevolucao;
                pedido.NumeroPedidoDevolucao = cargaIntegracao.NumeroPedidoDevolucao;
                pedido.KMAsfaltoAteDestino = cargaIntegracao?.KMAsfaltoAteDestino ?? 0;
                pedido.KMChaoAteDestino = cargaIntegracao?.KMChaoAteDestino ?? 0;

                if (configuracao?.NaoSomarDistanciaPedidosIntegracao ?? false)
                    pedido.Distancia = string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga) ? cargaIntegracao.Distancia : 0;
                else
                    pedido.Distancia = cargaIntegracao.Distancia;

                Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
                if (cargaIntegracao.ClienteAdicional != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoClienteAdicional = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.ClienteAdicional, "ClienteAdicional", _unitOfWork, 0, false);
                    if (!retornoClienteAdicional.Status)
                        stMensagem.Append(retornoClienteAdicional.Mensagem);
                    else
                        pedido.ClienteAdicional = retornoClienteAdicional.cliente;
                }
                else
                    pedido.ClienteAdicional = null;

                if (cargaIntegracao.ClienteDonoContainer != null)
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoClienteDonoContainer = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.ClienteDonoContainer, "ClienteDonoContainer", _unitOfWork, 0, false);
                    if (!retornoClienteDonoContainer.Status)
                        stMensagem.Append(retornoClienteDonoContainer.Mensagem);
                    else
                        pedido.ClienteDonoContainer = retornoClienteDonoContainer.cliente;
                }
                else
                    pedido.ClienteDonoContainer = null;

                if (cargaIntegracao.Despachante != null)
                {
                    pedido.CodigoDespachante = cargaIntegracao.Despachante.Codigo;
                    pedido.DescricaoDespachante = cargaIntegracao.Despachante.Descricao;
                }
                else
                {
                    pedido.CodigoDespachante = null;
                    pedido.DescricaoDespachante = null;
                }

                if (cargaIntegracao.PortoViagemOrigem != null)
                {
                    pedido.CodigoPortoOrigem = cargaIntegracao.PortoViagemOrigem.Codigo;
                    pedido.DescricaoPortoOrigem = cargaIntegracao.PortoViagemOrigem.Descricao;
                    pedido.PaisPortoOrigem = cargaIntegracao.PortoViagemOrigem.Pais;
                    pedido.SiglaPaisPortoOrigem = cargaIntegracao.PortoViagemOrigem.SiglaPais;
                }
                else
                {
                    pedido.CodigoPortoOrigem = string.Empty;
                    pedido.DescricaoPortoOrigem = string.Empty;
                    pedido.PaisPortoOrigem = string.Empty;
                    pedido.SiglaPaisPortoOrigem = string.Empty;
                }

                if (cargaIntegracao.PortoViagemDestino != null)
                {
                    pedido.CodigoPortoDestino = cargaIntegracao.PortoViagemDestino.Codigo;
                    pedido.DescricaoPortoDestino = cargaIntegracao.PortoViagemDestino.Descricao;
                    pedido.PaisPortoDestino = cargaIntegracao.PortoViagemDestino.Pais;
                    pedido.SiglaPaisPortoDestino = cargaIntegracao.PortoViagemDestino.SiglaPais;
                }
                else
                {
                    pedido.CodigoPortoDestino = null;
                    pedido.DescricaoPortoDestino = null;
                    pedido.PaisPortoDestino = null;
                    pedido.SiglaPaisPortoDestino = null;
                }


                if (!string.IsNullOrEmpty(cargaIntegracao.NumeroContratoFreteCliente))
                {
                    Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(_unitOfWork);
                    pedido.ContratoFreteCliente = repContratoFreteCliente.BuscarPorNumeroContrato(cargaIntegracao.NumeroContratoFreteCliente);
                    if (pedido.ContratoFreteCliente == null)
                        stMensagem.Append("Não foi localizado um Contrato Frete Cliente com este número: " + cargaIntegracao.NumeroContratoFreteCliente + ". ");
                    else if (pedido.ContratoFreteCliente.Fechado)
                        stMensagem.Append("O contrato de frete de número: " + cargaIntegracao.NumeroContratoFreteCliente + " já foi fechado. ");
                }

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.RegiaoDestino?.CodigoIntegracao))
                {
                    Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigoIntegracao(cargaIntegracao.RegiaoDestino.CodigoIntegracao);
                    if (regiao != null)
                        pedido.RegiaoDestino = regiao;
                    else
                        stMensagem.Append("Não foi localizada uma região para o código informado " + cargaIntegracao.RegiaoDestino.CodigoIntegracao + ". ");
                }

                if (cargaIntegracao.InLand != null)
                {
                    pedido.CodigoInLand = cargaIntegracao.InLand.Codigo;
                    pedido.DescricaoInLand = cargaIntegracao.InLand.Descricao;
                }
                else
                {
                    pedido.CodigoInLand = null;
                    pedido.DescricaoInLand = null;
                }

                if (cargaIntegracao.Especie != null)
                {
                    pedido.CodigoEspecie = cargaIntegracao.Especie.Codigo;
                    pedido.DescricaoEspecie = cargaIntegracao.Especie.Descricao;
                }
                else
                {
                    pedido.CodigoEspecie = null;
                    pedido.DescricaoEspecie = null;
                }

                if (cargaIntegracao.NavioViagem != null)
                {
                    pedido.CodigoNavioViagem = cargaIntegracao.NavioViagem.Codigo;
                    pedido.NomeNavioViagem = cargaIntegracao.NavioViagem.Nome;

                    DateTime dataDeadLine = DateTime.MinValue;
                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.NavioViagem.DataDeadLine))
                    {
                        DateTime.TryParseExact(cargaIntegracao.NavioViagem.DataDeadLine, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataDeadLine);

                        if (cargaIntegracao.NavioViagem.DataDeadLine.Length == 10)
                            DateTime.TryParseExact(cargaIntegracao.NavioViagem.DataDeadLine, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataDeadLine);

                        if (cargaIntegracao.NavioViagem.DataDeadLine.Length == 8)
                            DateTime.TryParseExact(cargaIntegracao.NavioViagem.DataDeadLine, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataDeadLine);
                    }
                    if (dataDeadLine != DateTime.MinValue)
                        pedido.DataDeadLineNavioViagem = dataDeadLine;
                    else
                        pedido.DataDeadLineNavioViagem = null;

                    DateTime dataDeadLCarga = DateTime.MinValue;
                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.NavioViagem.DataDeadLCarga))
                    {
                        DateTime.TryParseExact(cargaIntegracao.NavioViagem.DataDeadLCarga, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataDeadLCarga);

                        if (cargaIntegracao.NavioViagem.DataDeadLCarga.Length == 10)
                            DateTime.TryParseExact(cargaIntegracao.NavioViagem.DataDeadLCarga, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataDeadLCarga);

                        if (cargaIntegracao.NavioViagem.DataDeadLCarga.Length == 8)
                            DateTime.TryParseExact(cargaIntegracao.NavioViagem.DataDeadLCarga, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataDeadLCarga);
                    }
                    if (dataDeadLCarga != DateTime.MinValue)
                        pedido.DataDeadLCargaNavioViagem = dataDeadLCarga;
                    else
                        pedido.DataDeadLCargaNavioViagem = null;
                }
                else
                {
                    pedido.CodigoNavioViagem = null;
                    pedido.NomeNavioViagem = null;
                    pedido.DataDeadLineNavioViagem = null;
                    pedido.DataDeadLCargaNavioViagem = null;
                }

                pedido.PagamentoMaritimo = cargaIntegracao.PagamentoMaritimo;
                pedido.TipoProbe = cargaIntegracao.TipoProbe;
                pedido.CargaPaletizada = cargaIntegracao.CargaPaletizada;
                pedido.FreeDeten = cargaIntegracao.FreeDeten;
                pedido.NumeroEXP = cargaIntegracao.NumeroEXP;
                pedido.RefEXPTransferencia = cargaIntegracao.RefEXPTransferencia;
                pedido.StatusEXP = cargaIntegracao.StatusEXP;
                pedido.NumeroPedidoProvisorio = cargaIntegracao.NumeroPedidoProvisorio;
                pedido.StatusPedidoEmbarcador = cargaIntegracao.StatusPedidoEmbarcador;
                pedido.AcondicionamentoCarga = cargaIntegracao.AcondicionamentoCarga;
                pedido.Onda = cargaIntegracao.Onda;
                pedido.ClusterRota = cargaIntegracao.ClusterRota;
                pedido.RotaEmbarcador = cargaIntegracao.RotaEmbarcador;
                pedido.NumeroEntregasFinais = cargaIntegracao.NumeroEntregasFinais;
                pedido.NumeroRastreioCorreios = cargaIntegracao.NumeroRastreioCorreios;

                pedido.CodigoAgrupamentoCarregamento = cargaIntegracao.CodigoAgrupamentoCarregamento;
                pedido.CustoFrete = cargaIntegracao.CustoFrete;

                if (pedido.NecessariaAverbacao)
                    pedido.NecessitaAverbacaoAutomatica = true;

                if (cargaIntegracao.SituacaoComercial != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido = repositorioSituacaoComercialPedido.BuscarPorCodigoIntegracao(cargaIntegracao.SituacaoComercial.CodigoIntegracao);

                    if (situacaoComercialPedido == null)
                        stMensagem.Append("Situação Comercial não encontrada, cadastro não foi realizado.");

                    // #76448, caso o pedido esteja em uma sessão ou em um "Carregamento" não pode Bloquear o pedido...
                    if (!(pedido.SituacaoComercialPedido?.BloqueiaPedido ?? true) && (situacaoComercialPedido?.BloqueiaPedido ?? false))
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessoesRoteirizadorPedido = repositorioSessaoRoteirizadorPedido.BuscarPorPedido(pedido.Codigo);
                        if ((sessoesRoteirizadorPedido?.Count ?? 0) > 0)
                            stMensagem.Append($"Pedido encontra-se na sessão {sessoesRoteirizadorPedido.FirstOrDefault().SessaoRoteirizador.Codigo} de roteirização e não pode ser Bloqueado.");

                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedido = repositorioCarregamentoPedido.BuscarPorPedido(pedido.Codigo);
                        if (carregamentosPedido.Any(x => x.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado))
                            stMensagem.Append($"Pedido encontra-se no carregamento {(from o in carregamentosPedido where o.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado select o.Carregamento).FirstOrDefault().NumeroCarregamento} e não pode ser Bloqueado.");
                    }

                    pedido.SituacaoComercialPedido = situacaoComercialPedido;
                }

                PreencherDatasPedido(ref pedido, cargaIntegracao, ref stMensagem);
                PreencherDadosPedido(ref pedido, tipoOperacao, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, Auditado, clienteAcesso, adminStringConexao, configuracao);
                PreencherDadosAquaviario(ref pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, stringConexao, Auditado, encerrarMDFeAutomaticamente, configuracaoWebService, configuracao);

                new Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracao).AtualizarObservacaoTransportadorPorPedido(pedido);//VER
            }
            else
                stMensagem.Append("O número do pedido não deve possuir mais que 50 caracteres;");
        }

        private void PreencherDadosAquaviario(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool encerrarMDFeAutomaticamente, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            if ((configuracaoWebService?.IgnorarCamposEssenciais ?? false))
                pedido.CodigoBooking = cargaIntegracao.CodigoBooking.ToString("D");
            if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroBooking))
                pedido.NumeroBooking = cargaIntegracao.NumeroBooking;
            if (cargaIntegracao.Embarcador != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoRemetente = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Embarcador, "Remetente", _unitOfWork, 0, false);
                if (retornoRemetente.Status == false)
                {
                    stMensagem.Append(retornoRemetente.Mensagem);
                }
                else
                {
                    pedido.Remetente = retornoRemetente.cliente;
                    PreecherEnderecoPedidoPorCliente(pedido.EnderecoOrigem, pedido.Remetente);
                }
            }

            if (cargaIntegracao.ViagemLongoCurso != null)
                pedido.PedidoViagemNavioLongoCurso = SalvarViagem(cargaIntegracao.ViagemLongoCurso, ref stMensagem, Auditado, encerrarMDFeAutomaticamente);
            if (cargaIntegracao.Viagem != null)
            {
                pedido.PedidoViagemNavio = SalvarViagem(cargaIntegracao.Viagem, ref stMensagem, Auditado, encerrarMDFeAutomaticamente);
                pedido.Navio = pedido.PedidoViagemNavio?.Navio ?? null;
                pedido.DirecaoViagemMultimodal = pedido.PedidoViagemNavio?.DirecaoViagemMultimodal ?? DirecaoViagemMultimodal.Sul;
            }
            //if (cargaIntegracao.ViagemLongoCurso != null)
            //    pedido.PedidoViagemNavio = SalvarViagem(cargaIntegracao.ViagemLongoCurso, ref stMensagem, unitOfWork);
            if (cargaIntegracao.PortoOrigem != null)
                pedido.Porto = SalvarPorto(cargaIntegracao.PortoOrigem, ref stMensagem, Auditado);
            if (cargaIntegracao.PortoDestino != null)
                pedido.PortoDestino = SalvarPorto(cargaIntegracao.PortoDestino, ref stMensagem, Auditado);
            if (cargaIntegracao.TerminalPortoDestino != null)
                pedido.TerminalDestino = SalvarTerminalPorto(cargaIntegracao.TerminalPortoDestino, ref stMensagem, Auditado, tipoServicoMultisoftware, integracaoIntercab?.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio ?? false);
            if (cargaIntegracao.TerminalPortoOrigem != null)
                pedido.TerminalOrigem = SalvarTerminalPorto(cargaIntegracao.TerminalPortoOrigem, ref stMensagem, Auditado, tipoServicoMultisoftware, integracaoIntercab?.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio ?? false);
            if (!cargaIntegracao.NaoAtualizarDadosDoPedido && cargaIntegracao.TipoContainerReserva != null)
                pedido.ContainerTipoReserva = SalvarContainerTipo(cargaIntegracao.TipoContainerReserva, ref stMensagem, Auditado);

            if (pedido.Porto != null && pedido.Porto.Empresa != null && configuracaoTMS.UtilizaEmissaoMultimodal)
                pedido.Empresa = pedido.Porto.Empresa;

            if (cargaIntegracao.EmpresaResponsavel != null)
                pedido.PedidoEmpresaResponsavel = SalvarEmpresaResponsavel(cargaIntegracao.EmpresaResponsavel, ref stMensagem);
            if (cargaIntegracao.CentroCusto != null)
                pedido.PedidoCentroCusto = SalvarCentroCusto(cargaIntegracao.CentroCusto, ref stMensagem);

            if (cargaIntegracao.CNPJsDestinatariosNaoAutorizados != null && cargaIntegracao.CNPJsDestinatariosNaoAutorizados.Count > 0)
            {
                pedido.DestinatariosBloqueados = new List<string>();
                foreach (string cnpj in cargaIntegracao.CNPJsDestinatariosNaoAutorizados)
                    pedido.DestinatariosBloqueados.Add(cnpj);
            }

            pedido.PossuiCargaPerigosa = cargaIntegracao.ContemCargaPerigosa;
            pedido.ContemCargaRefrigerada = cargaIntegracao.ContemCargaRefrigerada;
            //pedido.Temperatura = cargaIntegracao.TemperaturaObservacao;
            pedido.ValidarDigitoVerificadorContainer = cargaIntegracao.ValidarNumeroContainer;
            if (cargaIntegracao.PropostaComercial != null)
            {
                pedido.CodigoProposta = cargaIntegracao.PropostaComercial.CodigoIntegracao.ToString("D");
                pedido.NumeroProposta = cargaIntegracao.PropostaComercial.Descricao;
            }
            pedido.CodigoOS = cargaIntegracao.CodigoOrdemServico.ToString("D");
            pedido.NumeroOS = cargaIntegracao.NumeroOrdemServico;
            pedido.Embarque = cargaIntegracao.Embarque;
            pedido.MasterBL = cargaIntegracao.MasterBL;
            pedido.NumeroDI = cargaIntegracao.NumeroDIEmbarque;
            pedido.TipoServicoCarga = cargaIntegracao.TipoServicoCarga;

            if (cargaIntegracao.ProvedorOS != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoProvedorOS = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.ProvedorOS, "ProvedorOS", _unitOfWork, 0, true);
                if (retornoProvedorOS.Status == false)
                {
                    stMensagem.Append(retornoProvedorOS.Mensagem);
                }
                else
                {
                    pedido.ProvedorOS = retornoProvedorOS.cliente;
                    PreecherEnderecoPedidoPorCliente(pedido.EnderecoOrigem, pedido.Remetente);
                }
            }
            if (cargaIntegracao.Container != null && !cargaIntegracao.NaoAtualizarDadosDoPedido)
                pedido.Container = SalvarContainer(cargaIntegracao.Container, ref stMensagem, Auditado);

            if (cargaIntegracao.TaraContainer > 0 && !cargaIntegracao.NaoAtualizarDadosDoPedido)
                pedido.TaraContainer = Utilidades.String.OnlyNumbers(cargaIntegracao.TaraContainer.ToString("n0"));
            else if (pedido.Container != null && pedido.Container.Tara > 0 && !cargaIntegracao.NaoAtualizarDadosDoPedido)
                pedido.TaraContainer = Utilidades.String.OnlyNumbers(pedido.Container.Tara.ToString("n0"));
            if (!cargaIntegracao.NaoAtualizarDadosDoPedido)
            {
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroLacre1))
                    pedido.LacreContainerUm = cargaIntegracao.NumeroLacre1;
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroLacre2))
                    pedido.LacreContainerDois = cargaIntegracao.NumeroLacre2;
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroLacre3))
                    pedido.LacreContainerTres = cargaIntegracao.NumeroLacre3;
            }
            pedido.CodigoBooking = cargaIntegracao.CodigoBooking.ToString("D");
            pedido.NumeroBL = cargaIntegracao.NumeroBL;
            pedido.NecessitaAverbacaoAutomatica = cargaIntegracao.NecessitaAverbacao;
            pedido.NecessitaEnergiaContainerRefrigerado = cargaIntegracao.CargaRefrigeradaPrecisaEnergia;
            pedido.QuantidadeTipoContainerReserva = cargaIntegracao.QuantidadeTipoContainerReserva;
            if (cargaIntegracao.TipoDocumentoAverbacao.HasValue)
                pedido.TipoDocumentoAverbacao = cargaIntegracao.TipoDocumentoAverbacao.Value;
            if (cargaIntegracao.TipoPropostaFeeder.HasValue)
                pedido.TipoPropostaFeeder = cargaIntegracao.TipoPropostaFeeder.Value;
            pedido.DescricaoTipoPropostaFeeder = cargaIntegracao.DescricaoTipoPropostaFeeder;
            pedido.IMOClasse = cargaIntegracao.IMOClasse;
            pedido.IMOSequencia = cargaIntegracao.IMOSequencia;
            pedido.IMOUnidade = cargaIntegracao.IMOUnidade;
            pedido.DescricaoCarrierNavioViagem = cargaIntegracao.DescricaoCarrierNavioViagem;
            pedido.RealizarCobrancaTaxaDocumentacao = cargaIntegracao.RealizarCobrancaTaxaDocumentacao;
            pedido.QuantidadeConhecimentosTaxaDocumentacao = cargaIntegracao.QuantidadeConhecimentosTaxaDocumentacao;
            pedido.ValorTaxaDocumento = cargaIntegracao.ValorTaxaDocumento;
            pedido.ContainerADefinir = cargaIntegracao.ContainerADefinir;
            pedido.ValorCusteioSVM = cargaIntegracao.ValorCusteioSVM;
            pedido.QuantidadeContainerBooking = cargaIntegracao.QuantidadeContainerBooking;
            pedido.PedidoDeSVMTerceiro = cargaIntegracao.PedidoDeSVMTerceiro;
            pedido.ValorTaxaFeeder = cargaIntegracao.ValorTaxaFeeder;
            pedido.Ajudante = cargaIntegracao.NecessarioAjudante;
            pedido.TipoCalculoCargaFracionada = cargaIntegracao.TipoCalculoCargaFracionada;

            if (cargaIntegracao.FormaAverbacaoCTE.HasValue)
                pedido.FormaAverbacaoCTE = cargaIntegracao.FormaAverbacaoCTE.Value;
            if (!string.IsNullOrWhiteSpace(cargaIntegracao.TemperaturaObservacao))
                pedido.ObservacaoCTe += (" " + cargaIntegracao.TemperaturaObservacao);
            if (!string.IsNullOrWhiteSpace(cargaIntegracao.ObservacaoProposta))
                pedido.ObservacaoCTe += (" " + cargaIntegracao.ObservacaoProposta);

            pedido.ValorAdValorem = cargaIntegracao.PercentualADValorem;
            if (pedido.ValorAdValorem > 0)
                SalvarComponenteMultimodal(pedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM, pedido.ValorAdValorem);

            if (!string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                pedido.ObservacaoCTe = pedido.ObservacaoCTe.Trim();

            if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalProprio || pedido.TipoOperacao.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao || pedido.TipoOperacao.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.RedespachoIntermediario))
                pedido.PedidoSubContratado = true;
            else if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro))
                pedido.PedidoDeSVMTerceiro = true;

            if (pedido.PedidoDeSVMTerceiro)
                pedido.PedidoSVM = false;


            if (pedido.TipoOperacao != null && pedido.TerminalOrigem != null && pedido.TerminalOrigem.Terminal != null
                && (pedido.TipoOperacao.ImportarTerminalOrigemComoExpedidor || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorta || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto))
                pedido.Expedidor = pedido.TerminalOrigem.Terminal;

            if (pedido.TipoOperacao != null && pedido.TerminalDestino != null && pedido.TerminalDestino.Terminal != null
                && (pedido.TipoOperacao.ImportarTerminalDestinoComoRecebedor || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorto || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto))
                pedido.Recebedor = pedido.TerminalDestino.Terminal;

            if (integracaoIntercab?.DefinirModalPeloTipoCarga ?? false)
            {
                if (cargaIntegracao.Destino != null && cargaIntegracao.Destino.Cidade != null)
                    pedido.DestinoBooking = buscarLocalidadeEndereco(cargaIntegracao.Destino, ref stMensagem);
                if (cargaIntegracao.Origem != null && cargaIntegracao.Origem.Cidade != null)
                    pedido.OrigemBooking = buscarLocalidadeEndereco(cargaIntegracao.Origem, ref stMensagem);
            }
            else
            {
                if (pedido.TipoOperacao != null && (pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorta || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorta) && cargaIntegracao.Destino != null && cargaIntegracao.Destino.Cidade != null)
                    pedido.DestinoBooking = buscarLocalidadeEndereco(cargaIntegracao.Destino, ref stMensagem);
                if (pedido.TipoOperacao != null && (pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorto || pedido.TipoOperacao.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorta) && cargaIntegracao.Origem != null && cargaIntegracao.Origem.Cidade != null)
                    pedido.OrigemBooking = buscarLocalidadeEndereco(cargaIntegracao.Origem, ref stMensagem);
            }

            if ((pedido.TipoOperacao?.ConfiguracaoCarga?.AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga ?? false) && !string.IsNullOrWhiteSpace(pedido.NumeroBL))
            {
                //if (pedido.NotasFiscais == null)
                //    pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                if (cargaIntegracao.NotasFiscais == null)
                    cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

                int numeroNota = pedido.NumeroBL.ToInt();

                //Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                //{
                //    Chave = pedido.NumeroBL,
                //    NCM = "",
                //    nfAtiva = true,
                //    Numero = numeroNota > 0 ? numeroNota : 1,
                //    Peso = pedido.PesoTotal > 0 ? pedido.PesoTotal : (decimal)1,
                //    Valor = pedido.ValorTotalNotasFiscais > 0 ? pedido.ValorTotalNotasFiscais : (decimal)1,
                //    DataEmissao = DateTime.Now,
                //    Destinatario = pedido.Destinatario,
                //    Emitente = pedido.Remetente,
                //    BaseCalculoICMS = (decimal)0,
                //    ValorICMS = (decimal)0,
                //    BaseCalculoST = (decimal)0,
                //    ValorST = (decimal)0,
                //    ValorTotalProdutos = (decimal)0,
                //    ValorSeguro = (decimal)0,
                //    ValorDesconto = (decimal)0,
                //    ValorImpostoImportacao = (decimal)0,
                //    ValorPIS = (decimal)0,
                //    PesoLiquido = pedido.PesoTotal > 0 ? pedido.PesoTotal : (decimal)1,
                //    Volumes = 0,
                //    ValorCOFINS = (decimal)0,
                //    ValorOutros = (decimal)0,
                //    ValorIPI = (decimal)0,
                //    TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                //    CNPJTranposrtador = pedido.Empresa?.CNPJ_SemFormato ?? "",
                //    ValorFrete = (decimal)0,
                //    TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros,
                //    Descricao = pedido.NumeroBL,
                //    ModalidadeFrete = ModalidadePagamentoFrete.Pago,
                //    RetornoNotaIntegrada = false,
                //    Empresa = pedido.Empresa,
                //    CanceladaPeloEmitente = false,
                //    NumeroPedido = 0,
                //    QuantidadePallets = (decimal)0,
                //    Altura = (decimal)0,
                //    Largura = (decimal)0,
                //    Comprimento = (decimal)0,
                //    MetrosCubicos = (decimal)0,
                //    PesoBaseParaCalculo = pedido.PesoTotal > 0 ? pedido.PesoTotal : (decimal)1,
                //    PesoCubado = (decimal)0,
                //    PesoPaletizado = (decimal)0,
                //    ValorFreteEmbarcador = (decimal)0,
                //    SemCarga = false,
                //    FatorCubagem = (decimal)0,
                //    PesoPorPallet = (decimal)0,
                //    KMRota = (decimal)0,
                //    XML = "",
                //    PlacaVeiculoNotaFiscal = "",
                //    DataRecebimento = DateTime.Now
                //};
                //repXMLNotaFiscal.Inserir(xmlNotaFiscal);
                //if (xmlNotaFiscal != null)
                //{
                //    if (!pedido.NotasFiscais.Any(o => o.Codigo == xmlNotaFiscal.Codigo))
                //        pedido.NotasFiscais.Add(xmlNotaFiscal);
                //}

                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal()
                {
                    AliquotaICMS = 0,
                    BaseCalculoICMS = 0,
                    BaseCalculoST = 0,
                    Canhoto = null,
                    CFOPPredominante = "",
                    Chave = "",
                    ChaveCTe = "",
                    ClassificacaoNFe = ClassificacaoNFe.Revenda,
                    CodigoIntegracaoCliente = "",
                    CodigoProduto = "",
                    Contabilizacao = null,
                    Containeres = null,
                    Cubagem = 0,
                    DataEmissao = DateTime.Now.ToString("dd/MM/yyyy"),
                    DataEmissaoDT = DateTime.Now.ToString("dd/MM/yyyy"),
                    DataHoraCriacaoEmbrcador = DateTime.Now.ToString("dd/MM/yyyy"),
                    DataPrevisao = DateTime.Now.ToString("dd/MM/yyyy"),
                    DescricaoMercadoria = "",
                    Destinatario = cargaIntegracao.Destinatario,
                    DocumentoRecebidoViaNOTFIS = false,
                    Emitente = cargaIntegracao.Remetente,
                    Expedidor = cargaIntegracao.Expedidor,
                    GrauRisco = "",
                    IBGEInicioPrestacao = "",
                    InformacoesComplementares = "",
                    KMRota = 0,
                    MetroCubico = 0,
                    ModalidadeFrete = ModalidadePagamentoFrete.Outros,
                    Modelo = "99",
                    ModeloVeicular = "",
                    NaturezaOP = "",
                    NCMPredominante = "",
                    Numero = numeroNota,
                    NumeroCanhoto = "",
                    NumeroCarregamento = "",
                    NumeroControleCliente = "",
                    NumeroDocumentoEmbarcador = "",
                    NumeroDT = "",
                    NumeroPedido = "",
                    NumeroReferenciaEDI = "",
                    NumeroRomaneio = "",
                    NumeroSolicitacao = "",
                    NumeroTransporte = "",
                    Observacao = "",
                    ObsPlaca = "",
                    ObsTransporte = "",
                    PesoAferido = pedido.PesoTotal > 0 ? pedido.PesoTotal : (decimal)1,
                    PesoBruto = pedido.PesoTotal > 0 ? pedido.PesoTotal : (decimal)1,
                    PesoLiquido = pedido.PesoTotal > 0 ? pedido.PesoTotal : (decimal)1,
                    PINSuframa = "",
                    Produtos = null,
                    Protocolo = 0,
                    QuantidadePallets = 0,
                    Recebedor = cargaIntegracao.Recebedor,
                    Rota = "",
                    Serie = "1",
                    SituacaoNFeSefaz = SituacaoNFeSefaz.Autorizada,
                    SubRota = "",
                    TipoCarga = null,
                    TipoDeCarga = null,
                    TipoDocumento = "99",
                    TipoOperacao = "",
                    TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                    Tomador = cargaIntegracao.Tomador,
                    Transportador = null,
                    Valor = pedido.ValorTotalNotasFiscais > 0 ? pedido.ValorTotalNotasFiscais : (decimal)1,
                    ValorCOFINS = 0,
                    ValorComponenteAdicionalEntrega = 0,
                    ValorComponenteAdValorem = 0,
                    ValorComponenteDescarga = 0,
                    ValorComponenteFreteCrossDocking = 0,
                    ValorComponentePedagio = 0,
                    ValorDesconto = 0,
                    ValorFrete = 0,
                    ValorFreteLiquido = 0,
                    ValorICMS = 0,
                    ValorImpostoImportacao = 0,
                    ValorIPI = 0,
                    ValorOutros = 0,
                    ValorPIS = 0,
                    ValorSeguro = 0,
                    ValorST = 0,
                    ValorTotalProdutos = pedido.ValorTotalNotasFiscais > 0 ? pedido.ValorTotalNotasFiscais : (decimal)1,
                    Veiculo = null,
                    Volumes = null,
                    VolumesTotal = 0,
                    MasterBL = pedido.NumeroBL,
                    NumeroOutroDocumento = pedido.NumeroBL
                };
                cargaIntegracao.NotasFiscais.Add(notaFiscal);
            }
        }

        private void PreencherDadosPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao repPedidoCarregamentoSituacao = new Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new Empresa.Motorista(_unitOfWork);
            Servicos.WebService.Empresa.Funcionario serFuncionario = new Empresa.Funcionario(_unitOfWork);
            Servicos.WebService.Frota.Veiculo serVeiculo = new Frota.Veiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Pedidos.PedidoCarregamentoSituacao pedidoCarregamentoSituacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCarregamentoSituacao
            {
                SituacaoAtualPedidoRetirada = SituacaoAtualPedidoRetirada.LiberacaoFinanceira,
                DataCriacaoPedido = DateTime.Now,
                DataLiberacaoComercial = DateTime.Now,
                DataLiberacaoFinanceira = DateTime.Now
            };

            repPedidoCarregamentoSituacao.Inserir(pedidoCarregamentoSituacao);

            pedido.PedidoCarregamentoSituacao = pedidoCarregamentoSituacao;

            if (cargaIntegracao.TipoCargaEmbarcador != null)
                pedido.TipoDeCarga = SalvarTipoDeCarga(cargaIntegracao.TipoCargaEmbarcador, ref stMensagem);//repTipoCarga.BuscarPorCodigoEmbarcador(cargaIntegracao.TipoCargaEmbarcador.CodigoIntegracao);            

            if (cargaIntegracao.ModeloVeicular != null)
            {
                pedido.ModeloVeicularCarga = repModeloVeicular.buscarPorCodigoIntegracao(cargaIntegracao.ModeloVeicular.CodigoIntegracao);
                if (pedido.ModeloVeicularCarga != null && pedido.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                    pedido.ModeloVeicularCarga = null;
            }

            pedido.SenhaAgendamentoCliente = cargaIntegracao.SenhaAgendamentoEntrega;
            pedido.EntregaAgendada = cargaIntegracao.EntregaAgendada;

            Dominio.Entidades.Empresa empresa = null;
            if (cargaIntegracao.TransportadoraEmitente != null)
            {
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.TransportadoraEmitente.CodigoDocumento))
                    empresa = repEmpresa.BuscarPorDocumentacao(cargaIntegracao.TransportadoraEmitente.CodigoDocumento);

                if (empresa == null)
                {
                    empresa = repEmpresa.BuscarPorCNPJ(cargaIntegracao.TransportadoraEmitente.CNPJ);
                    if (empresa == null)
                        stMensagem.Append("Não foi encontrado um transportador para o CNPJ " + cargaIntegracao.TransportadoraEmitente.CNPJ + " na base da Multisoftware");
                }
                pedido.Empresa = empresa;
            }

            informarCanalEntrega(pedido, cargaIntegracao, ref stMensagem);
            InformarCanalVenda(pedido, cargaIntegracao, ref stMensagem);

            pedido.EscritorioVenda = cargaIntegracao.EscritorioVenda;
            pedido.TipoMercadoria = cargaIntegracao.TipoMercadoria;
            pedido.EquipeVendas = cargaIntegracao.EquipeVendas;

            ReplicarInformacoesClienteComplementar(pedido, auditado);

            if (cargaIntegracao.Deposito != null)
            {
                Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(_unitOfWork);
                pedido.Deposito = repDeposito.BuscarPorCodigoIntegracao(cargaIntegracao.Deposito.CodigoIntegracao);
                if (pedido.Deposito == null)
                    stMensagem.Append("Não foi encontrado um depósito com o código " + cargaIntegracao.Deposito.CodigoIntegracao + " na base da Multisoftware");
            }

            if (cargaIntegracao.FreteRota != null)
            {
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

                pedido.RotaFrete = repRotaFrete.BuscarPorCodigoIntegracao(cargaIntegracao.FreteRota.Codigo);
                if (pedido.RotaFrete == null)
                    stMensagem.Append("Não foi encontrado uma rota com o código " + cargaIntegracao.FreteRota.Codigo + " na base da Multisoftware");
                else
                {
                    if (pedido.RotaFrete.Distribuidor != null)
                    {
                        pedido.Expedidor = pedido.RotaFrete.Distribuidor;
                        pedido.Recebedor = pedido.RotaFrete.Distribuidor;
                    }
                }
            }

            Dominio.Entidades.Veiculo veiculo = null;
            if (cargaIntegracao.Veiculo != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Placa) && !string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Placa.Replace("-", "")))
            {
                pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    veiculo = repVeiculo.BuscarPorPlaca(cargaIntegracao.Veiculo.Placa.Replace("-", ""));
                else
                {
                    if (empresa != null)
                        veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(empresa.Codigo, cargaIntegracao.Veiculo.Placa.Replace("-", ""), false);
                    else
                        stMensagem.Append("É necessário informar o transportador quando informar a placa. ");
                }

                if (veiculo == null)
                {
                    string mensagemVeiculo = "";

                    veiculo = serVeiculo.SalvarVeiculo(cargaIntegracao.Veiculo, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : empresa, false, ref mensagemVeiculo, _unitOfWork, tipoServicoMultisoftware, auditado);

                    if (mensagemVeiculo.Length > 0)
                        stMensagem.Append(mensagemVeiculo);
                }

                if (veiculo != null)
                {
                    if (cargaIntegracao.Veiculo != null && cargaIntegracao.Veiculo.TipoTagValePedagio.HasValue)
                    {
                        veiculo.ModoCompraValePedagioTarget = cargaIntegracao.Veiculo?.TipoTagValePedagio;
                        repVeiculo.Atualizar(veiculo);
                    }
                    if (configuracao?.PermitirSelecionarReboquePedido ?? false)
                        pedido.VeiculoTracao = veiculo;
                    else
                        pedido.Veiculos.Add(veiculo);

                    if (cargaIntegracao.Veiculo.Reboques?.Count > 0)
                    {
                        List<Dominio.Entidades.Veiculo> listaReboques = new List<Dominio.Entidades.Veiculo>();
                        foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in cargaIntegracao.Veiculo.Reboques)
                        {
                            Dominio.Entidades.Veiculo reboque = null;

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                reboque = repVeiculo.BuscarPorPlaca(reboqueIntegracao.Placa.Replace("-", ""));
                            else
                            {
                                if (empresa != null)
                                    reboque = repVeiculo.BuscarPorPlacaVarrendoFiliais(empresa.Codigo, reboqueIntegracao.Placa.Replace("-", ""));
                                else
                                    stMensagem.Append("É necessário informar o transportador quando informar a placa. ");
                            }

                            if (reboque == null)
                            {
                                string mensagemReboque = "";

                                reboque = serVeiculo.SalvarVeiculo(reboqueIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : empresa, false, ref mensagemReboque, _unitOfWork, tipoServicoMultisoftware);

                                if (mensagemReboque.Length > 0)
                                    stMensagem.Append(mensagemReboque);
                            }

                            if (reboque != null)
                            {
                                pedido.Veiculos.Add(reboque);
                                listaReboques.Add(reboque);
                            }
                        }

                        if (configuracaoGeralCarga.AtualizarVinculoVeiculoMotoristaIntegracao)
                            AtualizarVinculoReboquesVeiculo(veiculo, listaReboques, auditado);
                    }
                    else if (veiculo.VeiculosVinculados != null)
                    {
                        foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                            pedido.Veiculos.Add(reboque);
                    }
                }
            }

            if (cargaIntegracao.Motoristas != null)
            {
                if (pedido.Motoristas != null)
                    pedido.Motoristas.Clear();

                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao in cargaIntegracao.Motoristas)
                {
                    Dominio.Entidades.Usuario motorista = null;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        motorista = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF));
                    else
                    {
                        if (empresa != null)
                            motorista = repUsuario.BuscarMotoristaPorCPF(empresa.Codigo, Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF));
                        else
                            stMensagem.Append("É necessário informar o transportador quando informar o motorista. ");
                    }

                    if (motorista == null)
                    {
                        string mensagem = "";
                        motorista = serMotorista.SalvarMotorista(motoristaIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : empresa, ref mensagem, _unitOfWork, tipoServicoMultisoftware, auditado, clienteAcesso, adminStringConexao);
                        stMensagem.Append(mensagem);
                    }

                    if (motorista?.Bloqueado ?? false)
                        stMensagem.Append("Motorista bloqueado");

                    if (motorista != null && !motorista.Bloqueado)
                        pedido.Motoristas.Add(motorista);
                }

                if (configuracaoGeralCarga.AtualizarVinculoVeiculoMotoristaIntegracao)
                    AtualizarVinculoMotoristasVeiculo(veiculo, pedido.Motoristas.ToList(), auditado);
            }

            if (cargaIntegracao.FuncionarioVendedor != null)
            {
                Dominio.Entidades.Usuario funcionarioVendedor = serFuncionario.ConverterObjetoFuncionario(cargaIntegracao.FuncionarioVendedor, empresa, out string mensagem, tipoServicoMultisoftware, auditado, _unitOfWork);
                if (funcionarioVendedor == null)
                    stMensagem.Append(mensagem);
                else
                    pedido.FuncionarioVendedor = funcionarioVendedor;
            }

            if (cargaIntegracao.FuncionarioSupervisor != null)
            {
                Dominio.Entidades.Usuario funcionarioSupervisor = serFuncionario.ConverterObjetoFuncionario(cargaIntegracao.FuncionarioSupervisor, empresa, out string mensagem, tipoServicoMultisoftware, auditado, _unitOfWork);
                if (funcionarioSupervisor == null)
                    stMensagem.Append(mensagem);
                else
                    pedido.FuncionarioSupervisor = funcionarioSupervisor;
            }

            if (cargaIntegracao.FuncionarioGerente != null)
            {
                Dominio.Entidades.Usuario funcionarioGerente = serFuncionario.ConverterObjetoFuncionario(cargaIntegracao.FuncionarioGerente, empresa, out string mensagem, tipoServicoMultisoftware, auditado, _unitOfWork);
                if (funcionarioGerente == null)
                    stMensagem.Append(mensagem);
                else
                    pedido.FuncionarioGerente = funcionarioGerente;
            }

            if (tipoOperacao != null)
            {
                pedido.TipoOperacao = tipoOperacao;

                if (pedido.TipoDeCarga == null && tipoOperacao.TipoDeCargaPadraoOperacao != null)
                    pedido.TipoDeCarga = tipoOperacao.TipoDeCargaPadraoOperacao;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    pedido.CentroResultado = repCentroResultado.BuscarPorTipoOperacao(tipoOperacao);

                if (pedido.TipoOperacao != null)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    {
                        if (pedido.TipoOperacao.UtilizarExpedidorComoTransportador)
                        {
                            if (pedido.Expedidor == null)
                            {
                                if (empresa != null)
                                {
                                    Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
                                    Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(_unitOfWork);
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(serPessoa.ConverterObjetoEmpresa(empresa), "Expedidor", _unitOfWork, 0, false);
                                    if (retorno.Status == true)
                                        pedido.Expedidor = retorno.cliente;
                                }
                            }
                        }
                    }
                    else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {// caso na integração venha um tipo de operação que não pertence ao grupo do remetente ou destinatário da carga o sistema ignora e deixa sem, futuramente criar uma configuração para em algumas integrações rejeite (conforme solicitação do cliente)
                        bool verificarPorGrupo = false;
                        bool verificarPorPessoa = false;

                        if (pedido.TipoOperacao.GrupoPessoas != null)
                            verificarPorGrupo = true;

                        if (pedido.TipoOperacao.Pessoa != null)
                            verificarPorPessoa = true;

                        if (verificarPorPessoa)
                        {
                            if ((pedido.Remetente == null || pedido.Remetente.CPF_CNPJ != pedido.TipoOperacao.Pessoa.CPF_CNPJ) && (pedido.Destinatario == null || pedido.Destinatario.CPF_CNPJ != pedido.TipoOperacao.Pessoa.CPF_CNPJ))
                                pedido.TipoOperacao = null;
                        }
                        else if (verificarPorGrupo)
                        {
                            if ((pedido.Remetente.GrupoPessoas == null || pedido.Remetente.GrupoPessoas.Codigo != pedido.TipoOperacao.GrupoPessoas.Codigo) && (pedido.Destinatario.GrupoPessoas == null || pedido.Destinatario.GrupoPessoas.Codigo != pedido.TipoOperacao.GrupoPessoas.Codigo))
                                pedido.TipoOperacao = null;
                        }
                    }
                }
                else
                {
                    stMensagem.Append("Não existe o tipo de informação cadastrado na base Multisoftware para o codigo de informado (" + cargaIntegracao.TipoOperacao.CodigoIntegracao + ")");
                }
            }

            if (cargaIntegracao.ViaTransporte != null)
            {
                Repositorio.Embarcador.Cargas.ViaTransporte repositorioViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(_unitOfWork);
                pedido.ViaTransporte = repositorioViaTransporte.BuscarPorCodigoIntegracao(cargaIntegracao.ViaTransporte.Codigo);

                if (pedido.ViaTransporte == null)
                    stMensagem.Append("Não foi localizada uma via de transporte cadastrada com o código " + cargaIntegracao.ViaTransporte.Codigo.ToString() + " na base Multisoftware");
                else if (pedido.ViaTransporte.TipoOperacaoPadrao != null)
                    pedido.TipoOperacao = pedido.ViaTransporte.TipoOperacaoPadrao;
            }
            else
                pedido.ViaTransporte = null;
        }

        private void PreencherDadosTransporteMaritimo(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, StringBuilder mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            if (cargaIntegracao.DadosTransporteMaritimo == null)
                return;

            Repositorio.Cliente RepCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.ViaTransporte repviatransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo = repositorioPedidoDadosTransporteMaritimo.BuscarPorPedido(pedido.Codigo);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);

            if (pedidoDadosTransporteMaritimo == null)
                pedidoDadosTransporteMaritimo = new Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo()
                {
                    Pedido = pedido
                };

            pedidoDadosTransporteMaritimo.NumeroEXP = cargaIntegracao.NumeroEXP;
            pedidoDadosTransporteMaritimo.CodigoIdentificacaoCarga = cargaIntegracao.DadosTransporteMaritimo.CodigoIdentificacaoCarga;
            pedidoDadosTransporteMaritimo.CodigoNCM = cargaIntegracao.DadosTransporteMaritimo.CodigoNCM;
            pedidoDadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo = cargaIntegracao.DadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo;
            //pedidoDadosTransporteMaritimo.CodigoPortoDestinoTransbordo = cargaIntegracao.DadosTransporteMaritimo.CodigoPortoDestinoTransbordo;
            pedidoDadosTransporteMaritimo.CodigoRota = cargaIntegracao.DadosTransporteMaritimo.CodigoRota;
            pedidoDadosTransporteMaritimo.DescricaoIdentificacaoCarga = cargaIntegracao.DadosTransporteMaritimo.DescricaoIdentificacaoCarga;
            pedidoDadosTransporteMaritimo.DescricaoPortoCarregamentoTransbordo = cargaIntegracao.DadosTransporteMaritimo.DescricaoPortoCarregamentoTransbordo;
            //pedidoDadosTransporteMaritimo.DescricaoPortoDestinoTransbordo = cargaIntegracao.DadosTransporteMaritimo.DescricaoPortoDestinoTransbordo;
            pedidoDadosTransporteMaritimo.Incoterm = cargaIntegracao.DadosTransporteMaritimo.Incoterm;
            pedidoDadosTransporteMaritimo.MensagemTransbordo = cargaIntegracao.DadosTransporteMaritimo.MensagemTransbordo;
            pedidoDadosTransporteMaritimo.MetragemCarga = cargaIntegracao.DadosTransporteMaritimo.MetragemCarga;
            //pedidoDadosTransporteMaritimo.NomeNavioTransbordo = cargaIntegracao.DadosTransporteMaritimo.NomeNavioTransbordo;
            pedidoDadosTransporteMaritimo.NumeroBL = cargaIntegracao.DadosTransporteMaritimo.NumeroBL;
            pedidoDadosTransporteMaritimo.NumeroViagem = cargaIntegracao.DadosTransporteMaritimo.NumeroViagem;
            pedidoDadosTransporteMaritimo.NumeroViagemTransbordo = cargaIntegracao.DadosTransporteMaritimo.NumeroViagemTransbordo;
            pedidoDadosTransporteMaritimo.TipoEnvio = cargaIntegracao.DadosTransporteMaritimo.TipoEnvio?.ToNullableEnum<TipoEnvioTransporteMaritimo>() ?? TipoEnvioTransporteMaritimo.TON;
            pedidoDadosTransporteMaritimo.Transbordo = cargaIntegracao.DadosTransporteMaritimo.Transbordo;
            pedidoDadosTransporteMaritimo.Status = StatusControleMaritimo.Ativo;
            pedidoDadosTransporteMaritimo.Filial = pedido.Filial;

            pedidoDadosTransporteMaritimo.Halal = cargaIntegracao.DadosTransporteMaritimo.Halal == "sim" ? true : false;
            pedidoDadosTransporteMaritimo.CodigoContratoFOB = cargaIntegracao.DadosTransporteMaritimo.CodigoContratoFOB;
            pedidoDadosTransporteMaritimo.CargaPaletizada = cargaIntegracao.CargaPaletizada;
            pedidoDadosTransporteMaritimo.Remetente = pedido.Remetente;
            pedidoDadosTransporteMaritimo.Temperatura = pedido.Temperatura;
            pedidoDadosTransporteMaritimo.CodigoCargaEmbarcador = pedido.CodigoCargaEmbarcador;
            pedidoDadosTransporteMaritimo.Observacao = cargaIntegracao.Observacao;
            pedidoDadosTransporteMaritimo.TipoDeCarga = pedido.TipoDeCarga;


            if (cargaIntegracao.ViaTransporte != null)
                pedidoDadosTransporteMaritimo.ViaTransporte = repviatransporte.BuscarPorCodigoIntegracao(cargaIntegracao.ViaTransporte.Codigo);

            if (cargaIntegracao.PortoViagemOrigem != null)
                pedidoDadosTransporteMaritimo.PortoOrigem = RepCliente.BuscarPorCodigoIntegracao(cargaIntegracao.PortoViagemOrigem.Codigo);

            if (cargaIntegracao.PortoViagemDestino != null)
                pedidoDadosTransporteMaritimo.PortoDestino = RepCliente.BuscarPorCodigoIntegracao(cargaIntegracao.PortoViagemDestino.Codigo);

            if (cargaIntegracao.Despachante != null)
                pedidoDadosTransporteMaritimo.Despachante = RepCliente.BuscarPorCodigoIntegracao(cargaIntegracao.Despachante.Codigo);

            if (cargaIntegracao.PagamentoMaritimo.HasValue)
            {
                if (cargaIntegracao.PagamentoMaritimo == PagamentoMaritimo.Collect)
                    pedidoDadosTransporteMaritimo.FretePrepaid = FretePrepaid.Collect;
                else if (cargaIntegracao.PagamentoMaritimo == PagamentoMaritimo.Prepaid)
                    pedidoDadosTransporteMaritimo.FretePrepaid = FretePrepaid.Prepaid;
                else if (cargaIntegracao.PagamentoMaritimo == PagamentoMaritimo.PrepaidAbroad)
                    pedidoDadosTransporteMaritimo.FretePrepaid = FretePrepaid.PrepaidAbroad;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataInicioCarregamento))
            {
                DateTime.TryParseExact(cargaIntegracao.DataInicioCarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data);

                if (data != DateTime.MinValue)
                    pedidoDadosTransporteMaritimo.DataPrevisaoEstufagem = data;
            }

            if (cargaIntegracao.Especie != null)
            {
                pedidoDadosTransporteMaritimo.CodigoEspecie = cargaIntegracao.Especie.Codigo;
                pedidoDadosTransporteMaritimo.DescricaoEspecie = cargaIntegracao.Especie.Descricao;
            }

            if (cargaIntegracao.StatusEXP != null)
                pedidoDadosTransporteMaritimo.StatusEXP = cargaIntegracao.StatusEXP;

            if (pedido.Destinatario != null)
                pedidoDadosTransporteMaritimo.Importador = pedido.Destinatario;
            else if (cargaIntegracao.Destinatario != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Destinatario, "Destinatario", _unitOfWork, 0, cargaIntegracao.Destinatario?.AtualizarEnderecoPessoa ?? false, false, null, tipoServicoMultisoftware, false, true, unitOfWorkAdmin);
                if (retornoConversao.Status)
                    pedidoDadosTransporteMaritimo.Importador = retornoConversao.cliente;
            }


            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.SegundaDataDeadLineCarga))
            {
                pedidoDadosTransporteMaritimo.SegundaDataDeadLineCarga = cargaIntegracao.DadosTransporteMaritimo.SegundaDataDeadLineCarga?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.SegundaDataDeadLineCarga.HasValue)
                    mensagemErro.Append("A data Segunda Data Dead Line Carga dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.SegundaDataDeadLineDraf))
            {
                pedidoDadosTransporteMaritimo.SegundaDataDeadLineDraf = cargaIntegracao.DadosTransporteMaritimo.SegundaDataDeadLineDraf?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.SegundaDataDeadLineDraf.HasValue)
                    mensagemErro.Append("A data Segunda Data Dead Line Draf dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataBooking))
            {
                pedidoDadosTransporteMaritimo.DataBooking = cargaIntegracao.DadosTransporteMaritimo.DataBooking?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataBooking.HasValue)
                    mensagemErro.Append("A data do booking dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataDepositoContainer))
            {
                pedidoDadosTransporteMaritimo.DataDepositoContainer = cargaIntegracao.DadosTransporteMaritimo.DataDepositoContainer?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataDepositoContainer.HasValue)
                    mensagemErro.Append("A data do depósito do container dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataETADestino))
            {
                pedidoDadosTransporteMaritimo.DataETADestino = cargaIntegracao.DadosTransporteMaritimo.DataETADestino?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataETADestino.HasValue)
                    mensagemErro.Append("A data ETA do destino dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataETADestinoFinal))
            {
                pedidoDadosTransporteMaritimo.DataETADestinoFinal = cargaIntegracao.DadosTransporteMaritimo.DataETADestinoFinal?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataETADestinoFinal.HasValue)
                    mensagemErro.Append("A data ETA do destino final dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataETASegundaOrigem))
            {
                pedidoDadosTransporteMaritimo.DataETASegundaOrigem = cargaIntegracao.DadosTransporteMaritimo.DataETASegundaOrigem?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataETASegundaOrigem.HasValue)
                    mensagemErro.Append("A data ETA da segunda origem dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataETASegundoDestino))
            {
                pedidoDadosTransporteMaritimo.DataETASegundoDestino = cargaIntegracao.DadosTransporteMaritimo.DataETASegundoDestino?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataETASegundoDestino.HasValue)
                    mensagemErro.Append("A data ETA do segundo destino dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataETATransbordo))
            {
                pedidoDadosTransporteMaritimo.DataETATransbordo = cargaIntegracao.DadosTransporteMaritimo.DataETATransbordo?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataETATransbordo.HasValue)
                    mensagemErro.Append("A data ETA do transbordo dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataETSTransbordo))
            {
                pedidoDadosTransporteMaritimo.DataETSTransbordo = cargaIntegracao.DadosTransporteMaritimo.DataETSTransbordo?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataETSTransbordo.HasValue)
                    mensagemErro.Append("A data ETS do transbordo dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DadosTransporteMaritimo.DataRetiradaContainerDestino))
            {
                pedidoDadosTransporteMaritimo.DataRetiradaContainerDestino = cargaIntegracao.DadosTransporteMaritimo.DataRetiradaContainerDestino?.ToNullableDateTime();

                if (!pedidoDadosTransporteMaritimo.DataRetiradaContainerDestino.HasValue)
                    mensagemErro.Append("A data de retirada do container no destino dos dados de transporte marítimo não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
            }

            if (cargaIntegracao.InLand != null)
            {
                pedidoDadosTransporteMaritimo.TipoInLand = cargaIntegracao.InLand.Codigo.ToNullableEnum<TipoInland>() ?? TipoInland.NaoDefinido;
                //pedidoDadosTransporteMaritimo.DescricaoInLand = cargaIntegracao.InLand.Descricao;
            }

            if (pedido.ClienteDonoContainer != null)
                pedidoDadosTransporteMaritimo.Armador = pedido.ClienteDonoContainer;

            if (pedidoDadosTransporteMaritimo.Codigo > 0)
                repositorioPedidoDadosTransporteMaritimo.Atualizar(pedidoDadosTransporteMaritimo);
            else
                repositorioPedidoDadosTransporteMaritimo.Inserir(pedidoDadosTransporteMaritimo);
        }

        private void InformarFilialPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder stMensagem)
        {
            Servicos.WebService.Filial.Filial serWSFilial = new Filial.Filial(_unitOfWork);
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (filial == null)
                    stMensagem.Append("É obrigatório informar a filial do pedido;");
            }

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            if (filial != null)
                pedido.Filial = filial;

            if (cargaIntegracao.FilialVenda != null)
            {
                pedido.FilialVenda = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.FilialVenda.CodigoIntegracao);

                if (pedido.FilialVenda == null)
                {
                    string mensagem = "Não existe uma filial de venda cadastrada para o código de integração " + cargaIntegracao.Filial.CodigoIntegracao;

                    stMensagem.Append(mensagem);
                }
            }
        }

        private Dominio.Entidades.Localidade retornarLocalidadeExterior(Dominio.ObjetosDeValor.Localidade localidade, ref StringBuilder stMensagem)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(_unitOfWork);
            Dominio.Entidades.Localidade localidadeExt = repLocalidade.buscarPorCodigoEmbarcador(localidade.CodigoIntegracao);
            if (localidadeExt == null)
            {
                localidadeExt = new Dominio.Entidades.Localidade();
                localidadeExt.CEP = "";
                localidadeExt.Codigo = repLocalidade.BuscarPorMaiorCodigo();
                localidadeExt.Codigo++;
                localidadeExt.Descricao = localidade.Descricao;
                localidadeExt.Estado = new Dominio.Entidades.Estado() { Sigla = "EX" };
                localidadeExt.CodigoLocalidadeEmbarcador = localidade.CodigoIntegracao;
                if (localidade.Pais != null)
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(_unitOfWork);
                    localidadeExt.Pais = repPais.BuscarPorCodigo(localidade.Pais.CodigoPais);
                    if (localidadeExt.Pais == null)
                    {
                        stMensagem.Append(string.Concat("Código do país (" + localidade.Pais.CodigoPais.ToString() + ") não foi localizado; "));
                    }
                }

                repLocalidade.Inserir(localidadeExt);
            }
            if (localidade.Regiao != null && !string.IsNullOrWhiteSpace(localidade.Regiao.CodigoIntegracao))
            {
                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigoIntegracao(localidade.Regiao.CodigoIntegracao);
                if (regiao == null)
                {
                    regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao();
                    regiao.Codigo++;
                    regiao.Descricao = localidade.Regiao.Descricao;
                    regiao.CodigoIntegracao = localidade.Regiao.CodigoIntegracao;
                    if (localidade.Regiao.IBGELocalidadePolo > 0 && localidade.Regiao.IBGELocalidadePolo != 9999999)
                        regiao.LocalidadePolo = repLocalidade.BuscarPorCodigoIBGE(localidade.Regiao.IBGELocalidadePolo);
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(localidade.Regiao.CodigoIntegracaoLocalidadePolo))
                            regiao.LocalidadePolo = repLocalidade.buscarPorCodigoEmbarcador(localidade.Regiao.CodigoIntegracao);
                    }

                    repRegiao.Inserir(regiao);
                }

                localidadeExt.Regiao = regiao;
                repLocalidade.Atualizar(localidadeExt);
            }

            return localidadeExt;

        }

        private void InformarEnderecoPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string passouOrigem = "false";
            string passouDestino = "false";
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
                pedido.UsarOutroEnderecoDestino = cargaIntegracao.UsarOutroEnderecoDestino;
                pedido.UsarOutroEnderecoOrigem = cargaIntegracao.UsarOutroEnderecoOrigem;

                if (pedido.UsarOutroEnderecoDestino && cargaIntegracao.Destino != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = null;
                    if (pedido.EnderecoDestino == null)
                        enderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(pedido.EnderecoDestino.Localidade.CodigoDocumento))
                            enderecoDestino = repPedidoEndereco.BuscarPorCodigoDocumento(pedido.EnderecoDestino.Localidade.CodigoDocumento);
                        if (enderecoDestino == null)
                        {
                            enderecoDestino = repPedidoEndereco.BuscarPorCodigo(pedido.EnderecoDestino.Codigo);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.Destino.CodigoIntegracao))
                    {
                        setarOutroEnderecoCliente(enderecoDestino, pedido.Destinatario, cargaIntegracao.Destino, ref stMensagem);
                    }
                    else
                    {
                        preecherEnderecoPedidoPorEndereco(enderecoDestino, cargaIntegracao.Destino, cargaIntegracao.Destinatario?.AtualizarEnderecoPessoa ?? false, pedido.Destinatario, ref stMensagem);
                    }

                    if (enderecoDestino.Localidade == null)
                    {
                        stMensagem.Append("A cidade informada no endereço de destino é inválida");
                        return;
                    }

                    if (enderecoDestino.Codigo > 0)
                        repPedidoEndereco.Atualizar(enderecoDestino);
                    else
                        repPedidoEndereco.Inserir(enderecoDestino);

                    pedido.EnderecoDestino = enderecoDestino;
                }

                if (pedido.UsarOutroEnderecoOrigem && cargaIntegracao.Origem != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = null;
                    if (pedido.EnderecoOrigem == null)
                        enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(pedido.EnderecoOrigem.Localidade.CodigoDocumento))
                            enderecoOrigem = repPedidoEndereco.BuscarPorCodigoDocumento(pedido.EnderecoOrigem.Localidade.CodigoDocumento);
                        if (enderecoOrigem == null)
                            enderecoOrigem = repPedidoEndereco.BuscarPorCodigo(pedido.EnderecoOrigem.Codigo);
                    }


                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.Origem.CodigoIntegracao))
                    {
                        setarOutroEnderecoCliente(enderecoOrigem, pedido.Remetente, cargaIntegracao.Origem, ref stMensagem);
                    }
                    else
                    {
                        preecherEnderecoPedidoPorEndereco(enderecoOrigem, cargaIntegracao.Origem, cargaIntegracao.Remetente?.AtualizarEnderecoPessoa ?? false, pedido.Remetente, ref stMensagem);
                    }

                    if (enderecoOrigem.Codigo > 0)
                        repPedidoEndereco.Atualizar(enderecoOrigem);
                    else
                        repPedidoEndereco.Inserir(enderecoOrigem);

                    pedido.EnderecoOrigem = enderecoOrigem;
                }
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedido.Recebedor != null)
                        pedido.Destino = pedido.Recebedor.Localidade;
                    else if (pedido.Destinatario != null)
                        pedido.Destino = pedido.Destinatario.Localidade;
                    else
                        pedido.Destino = pedido.EnderecoDestino?.Localidade ?? null;

                    if (pedido.Expedidor != null)
                        pedido.Origem = pedido.Expedidor.Localidade;
                    else if (pedido.Remetente != null)
                        pedido.Origem = pedido.Remetente.Localidade;
                    else
                        pedido.Origem = pedido.EnderecoOrigem?.Localidade ?? null;
                }
                else
                {
                    pedido.Origem = pedido.EnderecoOrigem?.Localidade ?? null;
                    pedido.Destino = pedido.EnderecoDestino?.Localidade ?? null;
                }

                if (pedido.Origem.CodigoIBGE == 9999999 || pedido.Destino?.CodigoIBGE == 9999999)
                {
                    if (cargaIntegracao.Fronteira != null)
                    {
                        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

                        Dominio.Entidades.Cliente pedidoFronteira = repositorioCliente.BuscarPorCodigoIntegracao(cargaIntegracao.Fronteira.CodigoIntegracao);
                        if (pedidoFronteira == null && cargaIntegracao.Fronteira != null)
                            stMensagem.Append("O codigo de fronteira informado (" + cargaIntegracao.Fronteira.CodigoIntegracao + ") não está cadastrado no Multi Embarcador; ");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Endereco Origem: passou origem =" + passouOrigem);
                Servicos.Log.TratarErro("Endereco Destino: passou destino =" + passouDestino);
                throw;
            }
        }

        private void setarOutroEnderecoCliente(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, ref StringBuilder stMensagem)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigoEmbarcador(endereco.CodigoIntegracao, cliente.CPF_CNPJ);
            bool inserir = false;
            if (clienteOutroEndereco == null)
            {
                clienteOutroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();
                inserir = true;
            }
            clienteOutroEndereco.Cliente = cliente;
            clienteOutroEndereco.Bairro = endereco.Bairro;
            clienteOutroEndereco.CEP = endereco.CEP;
            clienteOutroEndereco.Complemento = !string.IsNullOrWhiteSpace(endereco.Complemento) ? endereco.Complemento : "";
            clienteOutroEndereco.IE_RG = !string.IsNullOrWhiteSpace(endereco.InscricaoEstadual) ? endereco.InscricaoEstadual : "";
            clienteOutroEndereco.Endereco = endereco.Logradouro;
            clienteOutroEndereco.CodigoEmbarcador = endereco.CodigoIntegracao;
            clienteOutroEndereco.TipoLogradouro = ObterTipoLogradouro(endereco.Logradouro);
            clienteOutroEndereco.Localidade = buscarLocalidadeEndereco(endereco, ref stMensagem);
            if (clienteOutroEndereco.Localidade == null)
                clienteOutroEndereco.Localidade = cliente.Localidade;

            //Se o outro endereco do cliente não possui coordenada ou se a é uma inclusão...
            if (inserir || string.IsNullOrWhiteSpace(clienteOutroEndereco.Latitude))
            {
                clienteOutroEndereco.Latitude = !string.IsNullOrWhiteSpace(endereco.Latitude) ? endereco.Latitude : "";
                clienteOutroEndereco.Longitude = !string.IsNullOrWhiteSpace(endereco.Longitude) ? endereco.Longitude : "";

                if (clienteOutroEndereco.GeoLocalizacaoStatus == GeoLocalizacaoStatus.NaoGerado)
                    if (Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(clienteOutroEndereco?.Latitude) && Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(clienteOutroEndereco?.Longitude))
                        clienteOutroEndereco.GeoLocalizacaoStatus = GeoLocalizacaoStatus.Gerado;
            }

            clienteOutroEndereco.Numero = !string.IsNullOrWhiteSpace(endereco.Numero) ? endereco.Numero : "S/N";
            clienteOutroEndereco.Telefone = !string.IsNullOrWhiteSpace(endereco.Telefone) ? endereco.Telefone : "";

            if (inserir)
                repClienteOutroEndereco.Inserir(clienteOutroEndereco);
            else
                repClienteOutroEndereco.Atualizar(clienteOutroEndereco);

            pedidoEndereco.Bairro = clienteOutroEndereco.Bairro;
            pedidoEndereco.CEP = clienteOutroEndereco.CEP;
            pedidoEndereco.IE_RG = clienteOutroEndereco.IE_RG;
            pedidoEndereco.ClienteOutroEndereco = clienteOutroEndereco;
            pedidoEndereco.Complemento = clienteOutroEndereco.Complemento;
            pedidoEndereco.Endereco = clienteOutroEndereco.Endereco;
            pedidoEndereco.Localidade = clienteOutroEndereco.Localidade;
            pedidoEndereco.Numero = clienteOutroEndereco.Numero;
            pedidoEndereco.Telefone = clienteOutroEndereco.Telefone;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro ObterTipoLogradouro(string logradouro)
        {
            if (string.IsNullOrWhiteSpace(logradouro))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros;

            string texto = logradouro.Trim().ToUpperInvariant();

            if (texto.StartsWith("RUA ") || texto.StartsWith("R ") || texto.StartsWith("R. "))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua;
            if (texto.StartsWith("AV ") || texto.StartsWith("AVENIDA ") || texto.StartsWith("AV. "))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Avenida;
            if (texto.StartsWith("ROD ") || texto.StartsWith("RODOVIA ") || texto.StartsWith("ROD. "))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rodovia;
            if (texto.StartsWith("EST ") || texto.StartsWith("ESTRADA ") || texto.StartsWith("EST. "))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Estrada;
            if (texto.StartsWith("PCA ") || texto.StartsWith("PRAÇA ") || texto.StartsWith("PRACA ") || texto.StartsWith("PCA. "))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Praca;
            if (texto.StartsWith("TV ") || texto.StartsWith("TRAVESSA ") || texto.StartsWith("TV. "))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Travessa;

            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros;
        }

        private void informarOutroEnderecoCliente(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, bool atualizarDados, ref StringBuilder stMensagem)
        {
            if (cliente == null)
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = repClienteOutroEndereco.BuscarPorCEPNumeroLocalidade(pedidoEndereco.CEP, pedidoEndereco.Numero, pedidoEndereco.Localidade?.Codigo ?? 0, cliente.CPF_CNPJ);

            if (clienteOutroEndereco == null)
            {
                clienteOutroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();
                clienteOutroEndereco.Cliente = cliente;
                clienteOutroEndereco.Bairro = pedidoEndereco.Bairro;
                clienteOutroEndereco.CEP = pedidoEndereco.CEP;
                clienteOutroEndereco.Complemento = !string.IsNullOrWhiteSpace(pedidoEndereco.Complemento) ? pedidoEndereco.Complemento : "";
                clienteOutroEndereco.IE_RG = !string.IsNullOrWhiteSpace(pedidoEndereco.IE_RG) ? pedidoEndereco.IE_RG : "";
                clienteOutroEndereco.Endereco = pedidoEndereco.Endereco;
                clienteOutroEndereco.TipoLogradouro = ObterTipoLogradouro(endereco.Logradouro);
                clienteOutroEndereco.Localidade = pedidoEndereco.Localidade;

                clienteOutroEndereco.Latitude = !string.IsNullOrWhiteSpace(endereco.Latitude) ? endereco.Latitude : "";
                clienteOutroEndereco.Longitude = !string.IsNullOrWhiteSpace(endereco.Longitude) ? endereco.Longitude : "";

                clienteOutroEndereco.Numero = !string.IsNullOrWhiteSpace(pedidoEndereco.Numero) ? pedidoEndereco.Numero : "S/N";
                clienteOutroEndereco.Telefone = !string.IsNullOrWhiteSpace(pedidoEndereco.Telefone) ? pedidoEndereco.Telefone : "";

                if (clienteOutroEndereco.GeoLocalizacaoStatus == GeoLocalizacaoStatus.NaoGerado)
                    if (Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(clienteOutroEndereco?.Latitude) && Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(clienteOutroEndereco?.Longitude))
                        clienteOutroEndereco.GeoLocalizacaoStatus = GeoLocalizacaoStatus.Gerado;

                repClienteOutroEndereco.Inserir(clienteOutroEndereco);
            }
            else if (atualizarDados)
            {
                clienteOutroEndereco.Bairro = pedidoEndereco.Bairro;
                clienteOutroEndereco.CEP = pedidoEndereco.CEP;
                clienteOutroEndereco.Complemento = !string.IsNullOrWhiteSpace(pedidoEndereco.Complemento) ? pedidoEndereco.Complemento : "";
                clienteOutroEndereco.IE_RG = !string.IsNullOrWhiteSpace(pedidoEndereco.IE_RG) ? pedidoEndereco.IE_RG : "";
                clienteOutroEndereco.Endereco = pedidoEndereco.Endereco;
                clienteOutroEndereco.TipoLogradouro = ObterTipoLogradouro(endereco.Logradouro);
                clienteOutroEndereco.Localidade = pedidoEndereco.Localidade;
                clienteOutroEndereco.Latitude = !string.IsNullOrWhiteSpace(endereco.Latitude) ? endereco.Latitude : "";
                clienteOutroEndereco.Longitude = !string.IsNullOrWhiteSpace(endereco.Longitude) ? endereco.Longitude : "";
                clienteOutroEndereco.Numero = !string.IsNullOrWhiteSpace(pedidoEndereco.Numero) ? pedidoEndereco.Numero : "S/N";
                clienteOutroEndereco.Telefone = !string.IsNullOrWhiteSpace(pedidoEndereco.Telefone) ? pedidoEndereco.Telefone : "";

                if (clienteOutroEndereco.GeoLocalizacaoStatus == GeoLocalizacaoStatus.NaoGerado)
                    if (Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(clienteOutroEndereco?.Latitude) && Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(clienteOutroEndereco?.Longitude))
                        clienteOutroEndereco.GeoLocalizacaoStatus = GeoLocalizacaoStatus.Gerado;

                repClienteOutroEndereco.Atualizar(clienteOutroEndereco);
            }
            pedidoEndereco.ClienteOutroEndereco = clienteOutroEndereco;
        }

        private void preecherEnderecoPedidoPorEndereco(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, bool atualizarDados, Dominio.Entidades.Cliente cliente, ref StringBuilder stMensagem)
        {
            pedidoEndereco.Bairro = endereco.Bairro;
            pedidoEndereco.CEP = endereco.CEP;
            pedidoEndereco.Localidade = buscarLocalidadeEndereco(endereco, ref stMensagem);
            if (pedidoEndereco.Localidade == null)
                pedidoEndereco.Localidade = pedidoEndereco.Localidade;

            pedidoEndereco.Complemento = endereco.Complemento;
            pedidoEndereco.Endereco = endereco.Logradouro;
            pedidoEndereco.Numero = string.IsNullOrWhiteSpace(endereco.Numero) ? "S/N" : endereco.Numero;
            pedidoEndereco.IE_RG = !string.IsNullOrWhiteSpace(endereco.InscricaoEstadual) ? endereco.InscricaoEstadual : cliente != null && cliente.Localidade.Estado.Sigla == pedidoEndereco.Localidade?.Estado.Sigla && !string.IsNullOrWhiteSpace(cliente.IE_RG) ? cliente.IE_RG : "ISENTO";
            pedidoEndereco.Telefone = endereco.Telefone;
            informarOutroEnderecoCliente(pedidoEndereco, cliente, endereco, atualizarDados, ref stMensagem);
        }

        private void preecherEnderecoPedidoPorCliente(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.Entidades.Cliente cliente, string codigoEndereco)
        {
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(_unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco = repClienteOutroEndereco.BuscarPorCodigoIntegracao(codigoEndereco);
            if (outroEndereco != null)
            {
                pedidoEndereco.Bairro = outroEndereco.Bairro;
                pedidoEndereco.CEP = outroEndereco.CEP;
                pedidoEndereco.Localidade = outroEndereco.Localidade;
                pedidoEndereco.Complemento = outroEndereco.Complemento;
                pedidoEndereco.Endereco = outroEndereco.Endereco;
                pedidoEndereco.Numero = string.IsNullOrWhiteSpace(outroEndereco.Numero) ? "S/N" : outroEndereco.Numero;
                pedidoEndereco.IE_RG = string.IsNullOrWhiteSpace(outroEndereco.IE_RG) ? "ISENTO" : outroEndereco.IE_RG;
                pedidoEndereco.Telefone = outroEndereco.Telefone;
            }
            else
                PreecherEnderecoPedidoPorCliente(pedidoEndereco, cliente);
        }

        private Dominio.Entidades.Localidade buscarLocalidadeEndereco(Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, ref StringBuilder stMensagem)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = null; //new Dominio.Entidades.Localidade();

            if (endereco == null || endereco.Cidade == null)
                return null;

            if (endereco.Cidade.IBGE != 9999999)
            {
                if (endereco.Cidade.IBGE != 0)
                    localidade = repLocalidade.BuscarPorCodigoIBGE(endereco.Cidade.IBGE);

                if (localidade == null && endereco != null && endereco.Cidade != null && !string.IsNullOrWhiteSpace(endereco.Cidade.SiglaUF) && !string.IsNullOrWhiteSpace(endereco.Cidade.Descricao))
                    localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(endereco.Cidade.Descricao).ToUpper(), endereco.Cidade.SiglaUF.ToUpper());

                if (localidade == null && endereco != null)
                    localidade = repLocalidade.BuscarPorCEP(endereco.CEP);
            }
            else
            {
                localidade = retornarLocalidadeExterior(endereco.Cidade, ref stMensagem);
            }
            return localidade;
        }

        private void InformarParticipantesPedido(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, ref Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, bool integracaoViaWS = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoEndereco repEnderecoPedido = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            bool inserirOrigem = false;
            bool inserirDestino = false;

            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
            if (cargaIntegracao.Destinatario == null)
            {
                pedido.Destinatario = null;
                pedido.EnderecoDestino = null;
                inserirDestino = false;
            }
            else
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Destinatario, "Destinatario", _unitOfWork, 0, cargaIntegracao.Destinatario?.AtualizarEnderecoPessoa ?? false, false, Auditado, tipoServicoMultisoftware, cargaIntegracao.Destinatario?.AdicionarComoOutroEndereco ?? false, integracaoViaWS, unitOfWorkAdmin);
                if (retornoConversao.Status == false)
                {
                    stMensagem.Append(retornoConversao.Mensagem);
                }
                else
                {
                    pedido.Destinatario = retornoConversao.cliente;
                    if ((pedido?.Destinatario?.GerarPedidoBloqueado ?? false) || configuracaoPedido.BloquearPedidoAoIntegrar)
                        pedido.PedidoBloqueado = true;

                    if (pedido.EnderecoDestino == null)
                    {
                        pedido.EnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        inserirDestino = true;
                    }

                    if (retornoConversao.UsarOutroEndereco && retornoConversao.clienteOutroEndereco != null)
                    {
                        cargaIntegracao.UsarOutroEnderecoDestino = true;
                        PreecherEnderecoPedidoPorClienteOutroEndereco(pedido.EnderecoDestino, pedido.Destinatario, retornoConversao.clienteOutroEndereco);
                    }
                    else
                        PreecherEnderecoPedidoPorCliente(pedido.EnderecoDestino, pedido.Destinatario);
                }
            }

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoRemetente = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Remetente, "Remetente", _unitOfWork, 0, cargaIntegracao.Remetente?.AtualizarEnderecoPessoa ?? false, false, Auditado, tipoServicoMultisoftware, cargaIntegracao.Remetente?.AdicionarComoOutroEndereco ?? false, integracaoViaWS, unitOfWorkAdmin);
            if (retornoRemetente.Status == false)
            {
                stMensagem.Append(retornoRemetente.Mensagem);
            }
            else
            {
                pedido.Remetente = retornoRemetente.cliente;

                if (pedido.EnderecoOrigem == null)
                {
                    pedido.EnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    inserirOrigem = true;
                }

                if (retornoRemetente.UsarOutroEndereco && retornoRemetente.clienteOutroEndereco != null)
                {
                    cargaIntegracao.UsarOutroEnderecoOrigem = true;
                    PreecherEnderecoPedidoPorClienteOutroEndereco(pedido.EnderecoOrigem, pedido.Remetente, retornoRemetente.clienteOutroEndereco);
                }
                else
                    PreecherEnderecoPedidoPorCliente(pedido.EnderecoOrigem, pedido.Remetente);

            }

            if (cargaIntegracao.Tomador != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoTomador = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Tomador, "Tomador", _unitOfWork, 0, false, false, Auditado, tipoServicoMultisoftware, cargaIntegracao.Tomador?.AdicionarComoOutroEndereco ?? false, integracaoViaWS, unitOfWorkAdmin);

                if (retornoTomador.Status == false && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                && cargaIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros
                && (cargaIntegracao.TipoTomador == null || string.IsNullOrEmpty(cargaIntegracao.Tomador.CodigoIntegracao)))
                {
                    stMensagem.Append("CPF/CNPJ do Tomador não foi informado");
                }
                else if (retornoTomador.Status == false && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    stMensagem.Append(retornoTomador.Mensagem);
                else if (retornoTomador.Status == true)
                    pedido.Tomador = retornoTomador.cliente;
            }
            else
                pedido.Tomador = null;

            if (cargaIntegracao.Recebedor != null && (!string.IsNullOrEmpty(cargaIntegracao.Recebedor.CodigoIntegracao) || !string.IsNullOrEmpty(cargaIntegracao.Recebedor.CPFCNPJ)))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Recebedor, "Recebedor", _unitOfWork, 0, false, false, Auditado, tipoServicoMultisoftware, cargaIntegracao.Recebedor?.AdicionarComoOutroEndereco ?? false, integracaoViaWS, unitOfWorkAdmin);

                if (retorno.Status == false)
                    stMensagem.Append(retorno.Mensagem);
                else
                    pedido.Recebedor = retorno.cliente;
            }
            else
                pedido.Recebedor = null;

            if (cargaIntegracao.Expedidor != null && (!string.IsNullOrEmpty(cargaIntegracao.Expedidor.CodigoIntegracao) || !string.IsNullOrEmpty(cargaIntegracao.Expedidor.CPFCNPJ)))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Expedidor, "Expedidor", _unitOfWork, 0, false, false, Auditado, tipoServicoMultisoftware, cargaIntegracao.Expedidor?.AdicionarComoOutroEndereco ?? false, integracaoViaWS, unitOfWorkAdmin);
                if (retorno.Status == false)
                    stMensagem.Append(retorno.Mensagem);
                else
                    pedido.Expedidor = retorno.cliente;
            }
            else
                pedido.Expedidor = null;

            if (cargaIntegracao.PontoPartida != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.PontoPartida, "Ponto de Partida", _unitOfWork, 0, false, false, Auditado, tipoServicoMultisoftware, cargaIntegracao.PontoPartida?.AdicionarComoOutroEndereco ?? false, integracaoViaWS, unitOfWorkAdmin);

                if (retorno.Status == false)
                    stMensagem.Append(retorno.Mensagem);
                else
                    pedido.PontoPartida = retorno.cliente;
            }
            else
                pedido.PontoPartida = null;


            if (pedido.Recebedor != null)
                pedido.RegiaoDestino = pedido.Recebedor.Regiao;
            else
                pedido.RegiaoDestino = pedido.Destinatario?.Regiao;


            if (stMensagem.Length <= 0)
            {
                if (inserirOrigem)
                    repEnderecoPedido.Inserir(pedido.EnderecoOrigem);

                if (inserirDestino)
                    repEnderecoPedido.Inserir(pedido.EnderecoDestino);

            }
        }

        private bool SalvarSchedule(Dominio.ObjetosDeValor.Embarcador.Carga.Schedule scheduleIntegracao, Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem, ref StringBuilder stMensagem, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, out List<int> codigos, bool encerrarMDFeAutomaticamente)
        {
            codigos = new List<int>();
            if (scheduleIntegracao == null)
                return true;

            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoViagemNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = null;

            Dominio.Entidades.Embarcador.Pedidos.Porto porto = SalvarPorto(scheduleIntegracao.PortoAtracacao, ref stMensagem, Auditado);
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = SalvarTerminalPorto(scheduleIntegracao.TerminalAtracacao, ref stMensagem, Auditado);
            if (porto != null && terminal != null)
            {
                schedule = repPedidoViagemNavioSchedule.BuscarPorViagemPortoTerminal(viagem.Codigo, porto.Codigo, terminal.Codigo);
                if (schedule == null)
                    schedule = new Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule();
                else
                    schedule.Initialize();

                DateTime.TryParseExact(scheduleIntegracao.DataDeadLine, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataDeadLine);
                DateTime.TryParseExact(scheduleIntegracao.DataPrevisaoChegadaNavio, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoChegadaNavio);
                DateTime.TryParseExact(scheduleIntegracao.DataPrevisaoSaidaNavio, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoSaidaNavio);

                DateTime? nullDataDeadLine = null;
                DateTime? nullDataPrevisaoChegadaNavio = null;
                DateTime? nullDataPrevisaoSaidaNavio = null;

                if (dataDeadLine > DateTime.MinValue)
                    nullDataDeadLine = dataDeadLine;
                if (dataPrevisaoChegadaNavio > DateTime.MinValue)
                    nullDataPrevisaoChegadaNavio = dataPrevisaoChegadaNavio;
                if (dataPrevisaoSaidaNavio > DateTime.MinValue)
                    nullDataPrevisaoSaidaNavio = dataPrevisaoSaidaNavio;

                bool etsAnterior = schedule.ETSConfirmado;

                schedule.DataDeadLine = nullDataDeadLine;
                schedule.DataPrevisaoChegadaNavio = nullDataPrevisaoChegadaNavio;
                schedule.DataPrevisaoSaidaNavio = nullDataPrevisaoSaidaNavio;
                schedule.ETAConfirmado = scheduleIntegracao.ETAConfirmado;
                schedule.ETSConfirmado = scheduleIntegracao.ETSConfirmado;
                schedule.PedidoViagemNavio = viagem;
                schedule.PortoAtracacao = porto;
                schedule.TerminalAtracacao = terminal;
                schedule.Status = scheduleIntegracao.InativarCadastro ? false : true;

                if (schedule.Codigo == 0)
                    repPedidoViagemNavioSchedule.Inserir(schedule, Auditado);
                else if (scheduleIntegracao.Atualizar)
                    repPedidoViagemNavioSchedule.Atualizar(schedule, Auditado);

                if (!etsAnterior && schedule.ETSConfirmado)
                {
                    if (encerrarMDFeAutomaticamente && schedule.PortoAtracacao != null && schedule.PortoAtracacao.Localidade != null && schedule.PedidoViagemNavio != null)
                        Servicos.Embarcador.Carga.MDFe.EncerrarMDFePeloETSConfirmado(out string erro, null, "", schedule.PedidoViagemNavio.Codigo, schedule.PortoAtracacao.Codigo, _unitOfWork, stringConexao, Auditado);
                }
                codigos.Add(schedule.Codigo);
            }

            return true;
        }

        private Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel SalvarEmpresaResponsavel(Dominio.ObjetosDeValor.Embarcador.Carga.EmpresaResponsavel empresaResponsavel, ref StringBuilder stMensagem)
        {
            if (empresaResponsavel == null)
                return null;

            Repositorio.Embarcador.Pedidos.PedidoEmpresaResponsavel repPedidoEmpresaResponsavel = new Repositorio.Embarcador.Pedidos.PedidoEmpresaResponsavel(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel pedidoEmpresaResponsavel = null;

            pedidoEmpresaResponsavel = repPedidoEmpresaResponsavel.BuscarPorCodigoIntegracao(empresaResponsavel.CodigoIntegracao);
            if (pedidoEmpresaResponsavel != null)
                return pedidoEmpresaResponsavel;

            pedidoEmpresaResponsavel = new Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel()
            {
                CodigoIntegracao = empresaResponsavel.CodigoIntegracao,
                Descricao = empresaResponsavel.Descricao
            };
            repPedidoEmpresaResponsavel.Inserir(pedidoEmpresaResponsavel);

            return pedidoEmpresaResponsavel;
        }

        private Dominio.Entidades.Embarcador.Pedidos.PedidoCentroCusto SalvarCentroCusto(Dominio.ObjetosDeValor.Embarcador.Carga.CentroCusto centroCusto, ref StringBuilder stMensagem)
        {
            if (centroCusto == null)
                return null;

            Repositorio.Embarcador.Pedidos.PedidoCentroCusto repPedidoCentroCusto = new Repositorio.Embarcador.Pedidos.PedidoCentroCusto(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoCentroCusto pedidoCentroCusto = null;

            pedidoCentroCusto = repPedidoCentroCusto.BuscarPorCodigoIntegracao(centroCusto.CodigoIntegracao);
            if (pedidoCentroCusto != null)
                return pedidoCentroCusto;

            pedidoCentroCusto = new Dominio.Entidades.Embarcador.Pedidos.PedidoCentroCusto()
            {
                CodigoIntegracao = centroCusto.CodigoIntegracao,
                Descricao = centroCusto.Descricao
            };
            repPedidoCentroCusto.Inserir(pedidoCentroCusto);

            return pedidoCentroCusto;
        }

        public Dominio.Entidades.Embarcador.Pedidos.ContainerTipo SalvarContainerTipo(Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer tipoContainerIntegracao, ref StringBuilder stMensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (tipoContainerIntegracao == null)
                return null;

            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = null;

            if (!string.IsNullOrWhiteSpace(tipoContainerIntegracao.Descricao))
            {
                containerTipo = repContainerTipo.BuscarPorDescricao(tipoContainerIntegracao.Descricao);
                if (containerTipo != null)
                    return containerTipo;
            }

            containerTipo = repContainerTipo.BuscarTodosPorCodigoIntegracao(tipoContainerIntegracao.CodigoIntegracao.ToString("D"));
            if (containerTipo != null)
            {
                if (tipoContainerIntegracao.Atualizar)
                {
                    containerTipo.Initialize();
                    containerTipo.CodigoDocumento = tipoContainerIntegracao.CodigoDocumento;
                    containerTipo.CodigoIntegracao = tipoContainerIntegracao.CodigoIntegracao.ToString("d");
                    containerTipo.Descricao = tipoContainerIntegracao.Descricao;
                    containerTipo.Status = tipoContainerIntegracao.InativarCadastro ? false : true;
                    containerTipo.Valor = tipoContainerIntegracao.Valor;
                    containerTipo.Integrado = false;
                    containerTipo.MetrosCubicos = tipoContainerIntegracao.MetrosCubicos;
                    containerTipo.Tara = tipoContainerIntegracao.Tara;
                    containerTipo.PesoLiquido = tipoContainerIntegracao.PesoLiquido;
                    containerTipo.TEU = tipoContainerIntegracao.TEU;
                    containerTipo.FFE = tipoContainerIntegracao.FFE;
                    containerTipo.PesoMaximo = tipoContainerIntegracao.PesoMaximo;
                    repContainerTipo.Atualizar(containerTipo, auditado);
                }
                return containerTipo;
            }

            containerTipo = new Dominio.Entidades.Embarcador.Pedidos.ContainerTipo()
            {
                CodigoDocumento = tipoContainerIntegracao.CodigoDocumento,
                CodigoIntegracao = tipoContainerIntegracao.CodigoIntegracao.ToString("d"),
                Descricao = tipoContainerIntegracao.Descricao,
                Status = tipoContainerIntegracao.InativarCadastro ? false : true,
                Valor = tipoContainerIntegracao.Valor,
                Tipos = null,
                Integrado = false,
                MetrosCubicos = tipoContainerIntegracao.MetrosCubicos,
                Tara = tipoContainerIntegracao.Tara,
                PesoLiquido = tipoContainerIntegracao.PesoLiquido,
                TEU = tipoContainerIntegracao.TEU,
                FFE = tipoContainerIntegracao.FFE,
                PesoMaximo = tipoContainerIntegracao.PesoMaximo
            };
            repContainerTipo.Inserir(containerTipo, auditado);

            return containerTipo;
        }

        private void SalvarPessoasGrupoPessoa(Dominio.ObjetosDeValor.Embarcador.Carga.GrupoPessoa grupoPessoaIntegracao, int codigoGrupoPessoas, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ repGrupoPessoasRaizCNPJ = new Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
            bool contemRaizes = (grupoPessoas.RaizesCNPJ != null && grupoPessoas.RaizesCNPJ.Count > 0);
            bool contemCNPJs = (grupoPessoaIntegracao.CNPJsDoGrupo != null && grupoPessoaIntegracao.CNPJsDoGrupo.Count > 0);

            List<string> listaRaizCNPJ = new List<string>();
            List<double> listaCNPJ = new List<double>();

            if (grupoPessoaIntegracao.RaizCNPJ != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.RaizCNPJ raizCNPJ in grupoPessoaIntegracao.RaizCNPJ)
                {
                    if (!string.IsNullOrWhiteSpace(raizCNPJ.Raiz))
                    {
                        listaRaizCNPJ.Add(raizCNPJ.Raiz);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasExiste = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(raizCNPJ.Raiz));
                        if (grupoPessoasExiste == null || string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(raizCNPJ.Raiz)) || (grupoPessoasExiste.Codigo == grupoPessoas.Codigo))
                        {
                            if (contemRaizes && grupoPessoas.RaizesCNPJ.Any(o => o.RaizCNPJ == raizCNPJ.Raiz))
                                continue;

                            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ raiz = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ();
                            raiz.RaizCNPJ = Utilidades.String.OnlyNumbers(raizCNPJ.Raiz);
                            raiz.AdicionarPessoasMesmaRaiz = true;
                            raiz.GrupoPessoas = grupoPessoas;
                            repGrupoPessoasRaizCNPJ.Inserir(raiz, auditado);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, grupoPessoas, null, "Raíz CNPJ " + raiz.Descricao + " adicionada ao grupo.", _unitOfWork);

                            if (!contemCNPJs)
                                AdicionarPessoasDaRaiz(grupoPessoas, _unitOfWork, raiz.RaizCNPJ, auditado);
                        }
                    }
                }
            }
            if (listaRaizCNPJ != null && listaRaizCNPJ.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ> raizGrupoPessoa = repGrupoPessoasRaizCNPJ.BuscarPorGrupoPessoas(grupoPessoas.Codigo, listaRaizCNPJ);
                if (raizGrupoPessoa != null && raizGrupoPessoa.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ raizDeletar in raizGrupoPessoa)
                        repGrupoPessoasRaizCNPJ.Deletar(raizDeletar);
                }
            }
            if (contemCNPJs)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CNPJsDoGrupo cnpj in grupoPessoaIntegracao.CNPJsDoGrupo)
                {
                    if (!string.IsNullOrWhiteSpace(cnpj.CNPJ))
                    {
                        string cnpjNumero = cnpj.CNPJ.ObterSomenteNumeros();
                        double.TryParse(cnpjNumero, out double cnpjCPF);
                        if (cnpjCPF > 0)
                        {
                            listaCNPJ.Add(cnpjCPF);
                            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjCPF);

                            if (cliente != null)
                            {
                                cliente.GrupoPessoas = grupoPessoas;
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Atualizar(cliente);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou ao Grupo de Pessoa " + grupoPessoas.Descricao + ".", _unitOfWork);
                            }
                        }
                    }
                }

                if (listaCNPJ != null && listaCNPJ.Count > 0)
                {
                    List<Dominio.Entidades.Cliente> clientesRemovidos = repCliente.BuscarPorGrupoPessoa(grupoPessoas.Codigo, listaCNPJ);
                    if (clientesRemovidos != null && clientesRemovidos.Count > 0)
                    {
                        foreach (Dominio.Entidades.Cliente cli in clientesRemovidos)
                        {
                            cli.GrupoPessoas = null;
                            cli.DataUltimaAtualizacao = DateTime.Now;
                            cli.Integrado = false;
                            repCliente.Atualizar(cli);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cli, null, "Removeu do Grupo de Pessoa " + grupoPessoas.Descricao + ".", _unitOfWork);
                        }
                    }
                }
            }

        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga SalvarTipoDeCarga(Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador tipoCargaIntegracao, ref StringBuilder stMensagem)
        {
            if (tipoCargaIntegracao == null)
                return null;

            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;

            if (!string.IsNullOrWhiteSpace(tipoCargaIntegracao.Descricao))
                tipoCarga = repTipoDeCarga.BuscarPorDescricao(tipoCargaIntegracao.Descricao);
            if (tipoCarga != null)
                return tipoCarga;

            tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(tipoCargaIntegracao.CodigoIntegracao);
            if (tipoCarga != null)
                return tipoCarga;

            if (!string.IsNullOrEmpty(tipoCargaIntegracao.Descricao))
            {
                tipoCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga()
                {
                    Ativo = true,
                    ClasseONU = tipoCargaIntegracao.ClasseONU,
                    CodigoPsnONU = tipoCargaIntegracao.CodigoPSNONU,
                    CodigoTipoCargaEmbarcador = tipoCargaIntegracao.CodigoIntegracao,
                    Descricao = tipoCargaIntegracao.Descricao,
                    ObservacaoONU = tipoCargaIntegracao.ObservacaoONU,
                    SequenciaONU = tipoCargaIntegracao.SequenciaONU
                };
                repTipoDeCarga.Inserir(tipoCarga);
            }
            else
            {
                stMensagem.Append("Não foi localizado um tipo de carga para o código de integração " + tipoCargaIntegracao.CodigoIntegracao + " na base multisoftware.");
            }

            return tipoCarga;
        }

        private void SalvarComponenteMultimodal(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete, decimal valorComponente)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
            if (tipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(tipoComponenteFrete);
            else if (tipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS)
                componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador("BAF");

            if (componenteFrete != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = repPedidoComponenteFrete.BuscarPorCompomente(pedido.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete, false);
                bool inserir = false;
                if (pedidoComponenteFrete == null)
                {
                    pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();
                    inserir = true;
                }
                pedidoComponenteFrete.ComponenteFrete = componenteFrete;

                if (componenteFrete.ImprimirOutraDescricaoCTe)
                    pedidoComponenteFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

                pedidoComponenteFrete.ComponenteFilialEmissora = false;
                pedidoComponenteFrete.Pedido = pedido;
                pedidoComponenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;

                if (tipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                {
                    pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                    pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                    pedidoComponenteFrete.IncluirBaseCalculoICMS = true;
                    pedidoComponenteFrete.Percentual = valorComponente;
                    if (pedido.ValorTotalNotasFiscais > 0)
                        pedidoComponenteFrete.ValorComponente = ((valorComponente / 100) * pedido.ValorTotalNotasFiscais);
                    else
                        pedidoComponenteFrete.ValorComponente = (decimal)0;
                }
                else
                {
                    pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                    pedidoComponenteFrete.IncluirBaseCalculoICMS = false;
                    pedidoComponenteFrete.Percentual = 0;
                    pedidoComponenteFrete.ValorComponente = valorComponente;
                }

                if (inserir)
                    repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                else
                    repPedidoComponenteFrete.Atualizar(pedidoComponenteFrete);
            }
        }

        private void AdicionarPessoasDaRaiz(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeDeTrabalho, string raizCNPJ, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            List<Dominio.Entidades.Cliente> listaClientes = repCliente.BuscarPorRaizCNPJ(raizCNPJ, grupoPessoas.Codigo);

            if (grupoPessoas.Clientes == null)
                grupoPessoas.Clientes = new List<Dominio.Entidades.Cliente>();

            for (int i = 0; i < listaClientes.Count(); i++)
            {
                if (!grupoPessoas.Clientes.Contains(listaClientes[i]))
                {
                    Dominio.Entidades.Cliente cliente = listaClientes[i];
                    if (cliente.GrupoPessoas == null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                        cliente.DataUltimaAtualizacao = DateTime.Now;
                        cliente.Integrado = false;
                        repCliente.Atualizar(cliente);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou ao Grupo de Pessoa " + grupoPessoas.Descricao + ".", unidadeDeTrabalho);
                    }
                }
            }
        }

        private void InformarRotaFretePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService)
        {
            if (configuracaoWebService.SelecionarRotaFreteAoAdicionarPedido && pedido.Origem != null && pedido.Destino != null && pedido.Destinatario != null)
            {
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

                pedido.RotaFrete = repositorioRotaFrete.BuscarPorOrigemDestinoDestinatario(pedido.Origem.Codigo, pedido.Destino.Codigo, pedido.Destinatario.CPF_CNPJ);
            }
        }

        private void informarCanalEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem)
        {
            if (cargaIntegracao.CanalEntrega == null)
                return;

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(_unitOfWork);
            pedido.CanalEntrega = repositorioCanalEntrega.BuscarPorCodigoIntegracao(cargaIntegracao.CanalEntrega.CodigoIntegracao);

            if (pedido.CanalEntrega == null)
            {
                stMensagem.Append("Não foi encontrado um canal de entrega com o código " + cargaIntegracao.CanalEntrega.CodigoIntegracao + " na base da Multisoftware");
                return;
            }

            if (pedido.CanalEntrega.Filial?.CodigoFilialEmbarcador != cargaIntegracao.CanalEntrega.Filial?.CodigoIntegracao)
            {
                if (cargaIntegracao.CanalEntrega.Filial?.CodigoIntegracao != null)
                {
                    pedido.CanalEntrega.Filial = repositorioFilial.buscarPorCodigoEmbarcador(cargaIntegracao.CanalEntrega.Filial.CodigoIntegracao);

                    if (pedido.CanalEntrega.Filial == null)
                    {
                        stMensagem.Append("Não foi encontrado uma filial para o canal de entrega com o código de integração: " + cargaIntegracao.CanalEntrega.CodigoIntegracao + " na base da Multisoftware");
                        return;
                    }
                }
                else
                    pedido.CanalEntrega.Filial = null;

                repositorioCanalEntrega.Atualizar(pedido.CanalEntrega);
            }
        }

        private void InformarCanalVenda(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem)
        {
            if (cargaIntegracao.CanalVenda == null)
                return;

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(_unitOfWork);
            pedido.CanalVenda = repositorioCanalVenda.BuscarPorCodigoIntegracao(cargaIntegracao.CanalVenda.CodigoIntegracao);

            if (pedido.CanalVenda == null)
            {
                stMensagem.Append("Não foi encontrado um canal de venda com o código " + cargaIntegracao.CanalVenda.CodigoIntegracao + " na base da Multisoftware");
                return;
            }

            if (pedido.CanalVenda.Filial?.CodigoFilialEmbarcador != cargaIntegracao.CanalVenda.Filial?.CodigoIntegracao)
            {
                if (cargaIntegracao.CanalVenda.Filial?.CodigoIntegracao != null)
                {
                    pedido.CanalVenda.Filial = repositorioFilial.buscarPorCodigoEmbarcador(cargaIntegracao.CanalVenda.Filial.CodigoIntegracao);

                    if (pedido.CanalVenda.Filial == null)
                    {
                        stMensagem.Append("Não foi encontrado uma filial para o canal de venda com o código de integração: " + cargaIntegracao.CanalVenda.CodigoIntegracao + " na base da Multisoftware");
                        return;
                    }
                }
                else
                    pedido.CanalVenda.Filial = null;

                repositorioCanalVenda.Atualizar(pedido.CanalVenda);
            }
        }

        private void AtualizarVinculoReboquesVeiculo(Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> listaReboques, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (veiculo.TipoVeiculo != "0" || listaReboques.Count == 0)
                return;

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            List<Dominio.Entidades.Veiculo> listaVeiculosVinculados = veiculo.VeiculosVinculados?.ToList();
            if (listaReboques.All(o => listaVeiculosVinculados.Contains(o)) && listaReboques.Count == listaVeiculosVinculados.Count)
                return;

            veiculo.Initialize();

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            List<Dominio.Entidades.Veiculo> reboquesRemover = (from obj in listaVeiculosVinculados where !listaReboques.Contains(obj) select obj).ToList();

            foreach (Dominio.Entidades.Veiculo reboqueRemover in reboquesRemover)
            {
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "VeiculosVinculados",
                    De = reboqueRemover.Descricao,
                    Para = ""
                });
                veiculo.VeiculosVinculados.Remove(reboqueRemover);
            }

            foreach (Dominio.Entidades.Veiculo reboque in listaReboques)
            {
                if (listaVeiculosVinculados.Contains(reboque))
                    continue;

                veiculo.VeiculosVinculados.Add(reboque);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "VeiculosVinculados",
                    De = "",
                    Para = reboque.Descricao
                });
            }

            veiculo.SetExternalChanges(alteracoes);
            repVeiculo.Atualizar(veiculo, auditado);
        }

        private void AtualizarVinculoMotoristasVeiculo(Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Usuario> listaMotoristas, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (veiculo.TipoVeiculo != "0" || listaMotoristas.Count == 0)
                return;

            Dominio.Entidades.Usuario motorista = listaMotoristas.FirstOrDefault();

            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = repositorioVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);

            if (veiculoMotorista?.Motorista?.Codigo == motorista.Codigo)
                return;

            if (veiculoMotorista == null)
            {
                veiculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista()
                {
                    Veiculo = veiculo,
                    Principal = true
                };
            }
            else
                veiculoMotorista.Initialize();

            veiculoMotorista.Motorista = motorista;
            veiculoMotorista.CPF = motorista.CPF;
            veiculoMotorista.Nome = motorista.Nome;

            if (veiculoMotorista.Codigo > 0)
            {
                repositorioVeiculoMotorista.Atualizar(veiculoMotorista);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = veiculoMotorista.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, alteracoes, $"Alterou o motorista principal para {motorista.Nome} via web service", _unitOfWork);
            }
            else
            {
                repositorioVeiculoMotorista.Inserir(veiculoMotorista);
                Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Adicionou o motorista principal {motorista.Nome} via web service", _unitOfWork);
            }

            Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoesTrocaMotorista(veiculo, _unitOfWork);
        }

        private void ValidarCalculoFreteStagePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoStage repStagePedido = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado servCalculoFreteStageAgrupamento = new Embarcador.Pedido.CalculoFreteStagePedidoAgrupado(_unitOfWork, tipoServicoMultisoftware, configuracaoTMS);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> pedidosStage = repStagePedido.BuscarPorPedido(pedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            if (pedidosStage == null || pedidosStage.Count <= 0)
                return;

            servCalculoFreteStageAgrupamento.ProcessarFreteEGerarStagesAgrupadas(carga, pedidosStage.DistinctBy(s => s.Stage.Codigo).ToList(), cargaPedidos, false, true);
        }

        private void ReplicarInformacoesClienteComplementar(
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido,
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (pedido.Destinatario == null) return;

            if (string.IsNullOrWhiteSpace(pedido.EscritorioVenda) && string.IsNullOrWhiteSpace(pedido.EquipeVendas))
                return;

            Repositorio.Embarcador.Pessoas.ClienteComplementar repClienteComplementar = new(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repClienteComplementar.BuscarPorCliente(pedido.Destinatario.CPF_CNPJ) ?? new();
            clienteComplementar.Cliente = pedido.Destinatario;

            if (pedido.EscritorioVenda == clienteComplementar.EscritorioVendas && pedido.EquipeVendas == clienteComplementar.EquipeVendas)
                return;

            if (!string.IsNullOrWhiteSpace(pedido.EscritorioVenda))
                clienteComplementar.EscritorioVendas = pedido.EscritorioVenda;

            if (!string.IsNullOrWhiteSpace(pedido.EquipeVendas))
                clienteComplementar.EquipeVendas = pedido.EquipeVendas;

            if (clienteComplementar.Codigo > 0)
                repClienteComplementar.Atualizar(clienteComplementar);
            else
                repClienteComplementar.Inserir(clienteComplementar);

            Auditoria.Auditoria.Auditar(auditado, clienteComplementar.Cliente, null, "Salvou dados complementares pelo pedido.", _unitOfWork);
        }

        #endregion
    }
}