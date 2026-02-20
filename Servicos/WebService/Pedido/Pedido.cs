using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.WebService.Pedido
{
    public class Pedido
    {
        #region Propriedades Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Construtores

        public Pedido(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>> BuscarTiposOperacoesPendentesIntegracao(int quantidade)
        {

            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            int quantideRegistrosPendentes = repositorioTipoOperacao.QuantidadeTotalDeRegistrosPendentesDeIntegracao();

            Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao> retornoPaginacaoIntegracao = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>()
            {
                NumeroTotalDeRegistro = quantideRegistrosPendentes,
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>()
            };

            if (quantideRegistrosPendentes == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>.CriarRetornoSucesso(retornoPaginacaoIntegracao);

            int total = quantidade > 0 ? quantidade : 50;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> listaTipoOperacoesPendentesIntegracao = repositorioTipoOperacao.BuscarTiposDeOperacaoesPendentesDeIntegracao(total);

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in listaTipoOperacoesPendentesIntegracao)
                retornoPaginacaoIntegracao.Itens.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao()
                {
                    TipoServicoMultimodal = tipoOperacao.TipoServicoMultimodal,
                    TipoPropostaMultimodal = tipoOperacao.TipoPropostaMultimodal,
                    TipoCobrancaMultimodal = tipoOperacao.TipoCobrancaMultimodal,
                    ModalPropostaMultimodal = tipoOperacao.ModalPropostaMultimodal,
                    Descricao = tipoOperacao?.Descricao ?? string.Empty,
                    Protocolo = tipoOperacao.Codigo,
                    CNPJsDestinatariosNaoAutorizados = ObterListaCnpjClientes(tipoOperacao.ClientesBloquearEmissaoDosDestinatario.ToList()),
                    CNPJsDaOperacao = ObterListaCnpjClientes(tipoOperacao?.GrupoPessoas?.Clientes.ToList()),
                    BloquearEmissaoDosDestinatario = tipoOperacao?.BloquearEmissaoDosDestinatario ?? false,
                    BloquearEmissaoDeEntidadeSemCadastro = tipoOperacao?.BloquearEmissaoDeEntidadeSemCadastro ?? false
                });

            return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao>>.CriarRetornoSucesso(retornoPaginacaoIntegracao);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoTiposOperacoes(List<int> listaProtocolos)
        {
            if (listaProtocolos.Count == 0)
                return Retorno<bool>.CriarRetornoExcecao("Precisam informar protocolos que serão confirmados.");

            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga repositorioTipoOperacaoCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga(_unitOfWork);

            StringBuilder protocolosNaoProcessados = new StringBuilder();

            foreach (int protocolo in listaProtocolos)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao existeTipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(protocolo);
                if (existeTipoOperacao == null)
                {
                    protocolosNaoProcessados.Append(protocolo + ", ");
                    continue;
                }

                if (existeTipoOperacao.ConfiguracaoCarga == null)
                    existeTipoOperacao.ConfiguracaoCarga = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga();

                if (existeTipoOperacao.ConfiguracaoCarga.IntegradoERP)
                    continue;

                existeTipoOperacao.ConfiguracaoCarga.IntegradoERP = true;

                if (existeTipoOperacao.ConfiguracaoCarga.Codigo > 0)
                    repositorioTipoOperacaoCarga.Atualizar(existeTipoOperacao.ConfiguracaoCarga);
                else
                    repositorioTipoOperacaoCarga.Inserir(existeTipoOperacao.ConfiguracaoCarga);

            }
            if (!string.IsNullOrEmpty(protocolosNaoProcessados.ToString()))
                return Retorno<bool>.CriarRetornoSucesso(true, $"O(s) protocolo(s) {protocolosNaoProcessados} não foram confirmados porque não foi achados registros existentes.");

            return Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolos confirmados com sucesso");

        }

        public Retorno<bool> AtualizarPedidoProduto(Dominio.ObjetosDeValor.WebService.Pedido.AtualizacaoPedidoProduto atualizacaoPedidoProduto)
        {
            Servicos.Log.TratarErro($"AtualizarPedidoProduto - {(atualizacaoPedidoProduto != null ? Newtonsoft.Json.JsonConvert.SerializeObject(atualizacaoPedidoProduto) : string.Empty)}", "Request");

            if (atualizacaoPedidoProduto == null || atualizacaoPedidoProduto.ProtocoloIntegracaoPedido == 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo do pedido.");

            if (atualizacaoPedidoProduto.Produtos == null || atualizacaoPedidoProduto.Produtos.Count == 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar os produtos do pedido.");

            StringBuilder mensagemErro = new StringBuilder();

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Servicos.WebService.Carga.ProdutosPedido servicoProdutosPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(atualizacaoPedidoProduto.ProtocoloIntegracaoPedido);
            if (pedido == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizado um pedido para o protocolo informado ({atualizacaoPedidoProduto.ProtocoloIntegracaoPedido}).");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCargaPedido.BuscarCargaPorProtocoloPedido(atualizacaoPedidoProduto.ProtocoloIntegracaoPedido);
            if (carga != null && !servicoCarga.VerificarSeCargaEstaNaLogistica(carga, _tipoServicoMultisoftware))
                return Retorno<bool>.CriarRetornoDadosInvalidos($"A situação da carga {carga.CodigoCargaEmbarcador} não permite atualizar os produtos.");

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            _unitOfWork.Start();

            servicoProdutosPedidoWS.AtualizarProdutosPedido(pedido, carga, atualizacaoPedidoProduto.Produtos, null, 0, 0, 0, 0, ref mensagemErro, configuracaoTMS, _unitOfWork, _auditado, atualizarPesoPedidoComPesoProduto: true);

            if (mensagemErro.Length > 0)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro.ToString());
            }

            Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, $"Atualizou produtos do pedido por WebService", _unitOfWork);

            if (carga != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(carga.Codigo, pedido.Codigo);

                servicoCargaWS.AdicionarProdutosCarga(cargaPedido, atualizacaoPedidoProduto.Produtos, ref mensagemErro, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                if (mensagemErro.Length > 0)
                {
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro.ToString());
                }

                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Atualizou produtos do pedido {pedido.NumeroPedidoEmbarcador} por WebService", _unitOfWork);
            }

            _unitOfWork.CommitChanges();

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<bool> AjustarDatasDoPedido(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido)
        {
            DateTime? dataAgendamento = atualizarDatasPedido.DataAgendamento.ToNullableDateTime();
            DateTime? dataEntrega = atualizarDatasPedido.DataEntrega.ToNullableDateTime();
            string senhaAgendamento = atualizarDatasPedido.SenhaAgendamento;
            string senhaAgendamentoCliente = atualizarDatasPedido.SenhaAgendamentoCliente;
            string observacoesPedido = atualizarDatasPedido.ObservacoesPedido;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorProtocolo(atualizarDatasPedido.ProtocoloPedido);

            if (pedido == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizado um pedido para o protocolo informado ({atualizarDatasPedido.ProtocoloPedido}).");

            if (pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto)
                return Retorno<bool>.CriarRetornoDadosInvalidos("A situacao do pedido não permite alteração.");

            if (dataAgendamento.HasValue && dataAgendamento.Value.Date < DateTime.Now.Date)
                return Retorno<bool>.CriarRetornoDadosInvalidos("A data de agendamento não pode ser menor que a data atual");

            if (!dataAgendamento.HasValue && !dataEntrega.HasValue)
                return Retorno<bool>.CriarRetornoSucesso(false, "Datas não informadas para alteração");

            _unitOfWork.Start();

            if (dataAgendamento.HasValue)
                pedido.DataAgendamento = dataAgendamento;

            if (dataEntrega.HasValue)
                pedido.DataEntrega = dataEntrega;

            if (!string.IsNullOrEmpty(senhaAgendamento))
                pedido.SenhaAgendamento = senhaAgendamento;

            if (!string.IsNullOrEmpty(senhaAgendamentoCliente))
                pedido.SenhaAgendamentoCliente = senhaAgendamentoCliente;

            if (!string.IsNullOrEmpty(observacoesPedido))
                pedido.Observacao = observacoesPedido;

            repPedido.Atualizar(pedido);

            _unitOfWork.CommitChanges();

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<bool> AtualizarDataUltimaGeracao(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarDatasPedido atualizarDatasPedido)
        {
            DateTime? dataUltimaLiberacao = atualizarDatasPedido.DataUltimaLiberacao.ToNullableDateTime();

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorProtocolo(atualizarDatasPedido.ProtocoloPedido);

            if (pedido == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizado um pedido para o protocolo informado ({atualizarDatasPedido.ProtocoloPedido}).");

            _unitOfWork.Start();

            if (dataUltimaLiberacao.HasValue)
                pedido.DataUltimaLiberacao = dataUltimaLiberacao;

            repPedido.Atualizar(pedido);

            _unitOfWork.CommitChanges();

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>> ObterAgendamentos(Dominio.ObjetosDeValor.WebService.Pedido.ObterAgendamentos obterAgendamentos)
        {
            if (obterAgendamentos.Filial == null || string.IsNullOrEmpty(obterAgendamentos.Filial.CodigoIntegracao))
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>>.CriarRetornoDadosInvalidos($"Obrigatório Informar o Código de Integração da Filial.");

            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

            int quantidadeMaxima = 500;
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasDescarregamento = repJanelaDescarregamento.BuscarPorDataEFilial(obterAgendamentos);

            List<int> codigosCarga = janelasDescarregamento.Select(x => x.Carga.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaAgendamentoColetas = repAgendamentoColetaPedido.BuscarPorCargas(codigosCarga, quantidadeMaxima);

            Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos> retornoPaginacaoAgendamento = new Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>()
            {
                NumeroTotalDeRegistro = listaAgendamentoColetas.Count(),
                Itens = new List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>()
            };

            if (retornoPaginacaoAgendamento.NumeroTotalDeRegistro == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>>.CriarRetornoSucesso(retornoPaginacaoAgendamento);

            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamento in listaAgendamentoColetas)
                retornoPaginacaoAgendamento.Itens.Add(ConverterRetornoAgendamento(agendamento, janelasDescarregamento));

            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Pedido.RetornoObterAgendamentos>>.CriarRetornoSucesso(retornoPaginacaoAgendamento);
        }

        public Retorno<bool> LiberarEmissaoSemNFe(int protocoloIntegracaoCarga)
        {
            _unitOfWork.Start();

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

            if (carga == null)
                throw new WebServiceException("A carga informada não existe no Multi Embarcador");

            if (carga.SituacaoCarga != SituacaoCarga.AgNFe)
                throw new WebServiceException("A carga informada não esta pendente de NF-e no MultiEmbarcador");

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware, _auditado);

            _unitOfWork.CommitChanges();

            return Retorno<bool>.CriarRetornoSucesso(true, "Sucesso");
        }

        public Retorno<bool> AlterarSituacaoComercialPedido(Dominio.ObjetosDeValor.WebService.Pedido.AlterarSituacaoComercialPedido alterarSituacaoComercialPedido)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);
                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repositorioSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.SituacaoEstoquePedido repositorioSituacaoEstoquePedido = new Repositorio.Embarcador.Pedidos.SituacaoEstoquePedido(_unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(alterarSituacaoComercialPedido.ProtocoloPedido);
                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(alterarSituacaoComercialPedido.ProtocoloPedido);

                if (pedido == null && pedidoAdicional == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi localizado um pedido para o protocolo informado ({alterarSituacaoComercialPedido.ProtocoloPedido}).");

                Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido = repositorioSituacaoComercialPedido.BuscarPorCodigoIntegracao(alterarSituacaoComercialPedido?.SituacaoComercial?.CodigoIntegracao ?? string.Empty);
                Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido situacaoEstoquePedido = repositorioSituacaoEstoquePedido.BuscarPorCodigoIntegracao(alterarSituacaoComercialPedido?.SituacaoEstoque?.CodigoIntegracao ?? string.Empty);

                if (situacaoComercialPedido == null && situacaoEstoquePedido == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Situação Comercial/Estoque não encontrada.");

                if (situacaoEstoquePedido != null && pedidoAdicional == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Situação Estoque ou pedido não encontrado.");

                if ((!(pedido.SituacaoComercialPedido?.BloqueiaPedido ?? true) && (situacaoComercialPedido?.BloqueiaPedido ?? false)) ||
                    (!(pedidoAdicional?.SituacaoEstoquePedido?.BloqueiaPedido ?? true) && (situacaoEstoquePedido?.BloqueiaPedido ?? false)))
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(_unitOfWork);
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessoesRoteirizadorPedido = repositorioSessaoRoteirizadorPedido.BuscarPorPedido(pedido.Codigo);
                    if ((sessoesRoteirizadorPedido?.Count ?? 0) > 0)
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Pedido {alterarSituacaoComercialPedido.ProtocoloPedido} encontra-se na sessão {sessoesRoteirizadorPedido.FirstOrDefault().SessaoRoteirizador.Codigo} de roteirização e não pode ser Bloqueado.");

                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedido = repositorioCarregamentoPedido.BuscarPorPedido(pedido.Codigo);
                    if (carregamentosPedido.Any(x => x.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado))
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Pedido {alterarSituacaoComercialPedido.ProtocoloPedido} encontra-se no carregamento {(from o in carregamentosPedido where o.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado select o.Carregamento).FirstOrDefault().NumeroCarregamento} e não pode ser Bloqueado.");
                }

                if (pedido?.SituacaoComercialPedido != null && pedido.SituacaoComercialPedido.Codigo == situacaoComercialPedido.Codigo)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"O pedido {alterarSituacaoComercialPedido.ProtocoloPedido} já encontra-se com a Situação Comercial {situacaoComercialPedido.Descricao}.");

                if (pedidoAdicional?.SituacaoEstoquePedido != null && pedidoAdicional.SituacaoEstoquePedido?.Codigo == situacaoEstoquePedido?.Codigo)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"O pedido {alterarSituacaoComercialPedido.ProtocoloPedido} já encontra-se com a Situação Comercial {situacaoComercialPedido.Descricao}.");

                _unitOfWork.Start();

                if (situacaoComercialPedido != null)
                    pedido.SituacaoComercialPedido = situacaoComercialPedido;

                if (situacaoEstoquePedido != null)
                    pedidoAdicional.SituacaoEstoquePedido = situacaoEstoquePedido;

                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, $"Atualizou Situação Comercial do Pedido por WebService", _unitOfWork);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, "Situação Comercial atualizada");
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar a Situação Comercial do Pedido.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }


        public Retorno<bool> AtualizarPedidoObservacaoCte(Dominio.ObjetosDeValor.WebService.Pedido.AtualizarPedidoObservacaoCte atualizarPedidoObservacaoCte)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(atualizarPedidoObservacaoCte.protocoloIntegracaoCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(atualizarPedidoObservacaoCte.protocoloIntegracaoCarga, atualizarPedidoObservacaoCte.protocoloIntegracaoPedido);

                if (carga == null && cargaPedido == null)
                    throw new WebServiceException("A carga informada não existe no Multi Embarcador");

                if (cargaPedido != null)
                    carga = cargaPedido.Carga;

                if (carga.SituacaoCarga != SituacaoCarga.AgNFe && carga.SituacaoCarga != SituacaoCarga.Cancelada)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A atual situação da carga ({carga.SituacaoCarga.ObterDescricao()}) não permite que ele seja modificada.");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCargaEProtocoloPedido(carga.Codigo, atualizarPedidoObservacaoCte.protocoloIntegracaoPedido);

                if (pedido == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Pedido não encontrado");

                pedido.Initialize();

                _unitOfWork.Start();

                pedido.ObservacaoCTe = atualizarPedidoObservacaoCte.ObservacaoCte;

                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, $"Atualizou Observação Cte do Pedido por WebService", _unitOfWork);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, "Observação Cte atualizada");
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar a Observação Cte do Pedido.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AtualizarValorFrete(Dominio.ObjetosDeValor.WebService.Carga.Protocolo protocolo, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFrete, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor valorFreteFilialEmissora)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFreteAuxiliar repCargaComponentesFreteAuxiliar = new Repositorio.Embarcador.Cargas.CargaComponentesFreteAuxiliar(_unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro servicoFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(_unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(_unitOfWork);
                Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(_unitOfWork, _tipoServicoMultisoftware);
                Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(_unitOfWork, configuracaoEmbarcador);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                cargaPedido = repCargaPedido.BuscarPrimeiroPorProtocoloCarga(protocolo.protocoloIntegracaoCarga);

                if (cargaPedido == null && protocolo.protocoloIntegracaoPedido > 0)
                    cargaPedido = repCargaPedido.BuscarPrimeiroPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);

                if (cargaPedido == null)
                    throw new WebServiceException("Carga não foi encontrada através do(s) protocolo(s) informado.");

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                carga.TipoFreteEscolhido = TipoFreteEscolhido.Operador;
                carga.ValorFrete = valorFrete.FreteProprio;

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentesCarga = repCargaComponentesFrete.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteCarga in componentesCarga)
                    repCargaComponentesFrete.Deletar(componenteCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar> componentesCargaAuxiliar = repCargaComponentesFreteAuxiliar.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar componenteCargaAuxiliar in componentesCargaAuxiliar)
                    repCargaComponentesFreteAuxiliar.Deletar(componenteCargaAuxiliar);

                if (valorFrete.ComponentesAdicionais != null && valorFrete.ComponentesAdicionais.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in valorFrete.ComponentesAdicionais)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
                        if (componenteAdicional.Componente.TipoComponenteFrete != TipoComponenteFrete.OUTROS && componenteAdicional.Componente.TipoComponenteFrete != TipoComponenteFrete.TODOS)
                            componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(componenteAdicional.Componente.TipoComponenteFrete);
                        else
                            componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);

                        if (componenteFrete != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete = repCargaComponentesFrete.BuscarPrimeiroPorCargaPorCompomente(carga.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);
                            bool inserir = false;
                            if (cargaComponentesFrete == null)
                            {
                                cargaComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();
                                inserir = true;
                            }
                            cargaComponentesFrete.ComponenteFrete = componenteFrete;
                            cargaComponentesFrete.IncluirBaseCalculoICMS = true;
                            cargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                            if (componenteFrete.ImprimirOutraDescricaoCTe)
                                cargaComponentesFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

                            cargaComponentesFrete.ComponenteFilialEmissora = false;
                            cargaComponentesFrete.Carga = carga;
                            cargaComponentesFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;
                            cargaComponentesFrete.ValorComponente += componenteAdicional.ValorComponente;

                            if (inserir)
                                repCargaComponentesFrete.Inserir(cargaComponentesFrete);
                            else
                                repCargaComponentesFrete.Atualizar(cargaComponentesFrete);

                            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar cargaComponentesFreteAuxiliar = repCargaComponentesFreteAuxiliar.BuscarPrimeiroPorCargaPorCompomente(carga.Codigo, componenteFrete);
                            bool inserirAuxiliar = false;
                            if (cargaComponentesFreteAuxiliar == null)
                            {
                                cargaComponentesFreteAuxiliar = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar();
                                inserirAuxiliar = true;
                            }
                            cargaComponentesFreteAuxiliar.ComponenteFrete = componenteFrete;
                            cargaComponentesFreteAuxiliar.Carga = carga;
                            cargaComponentesFreteAuxiliar.ValorComponente += componenteAdicional.ValorComponente;

                            if (inserirAuxiliar)
                                repCargaComponentesFreteAuxiliar.Inserir(cargaComponentesFreteAuxiliar);
                            else
                                repCargaComponentesFreteAuxiliar.Atualizar(cargaComponentesFreteAuxiliar);
                        }
                        else
                            throw new WebServiceException("Não existe um componente de frete cadastrado do tipo ." + componenteAdicional.Componente.CodigoIntegracao);
                    }
                }

                if (valorFrete.ICMS != null)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(TipoComponenteFrete.ICMS);
                    if (componenteFrete != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar cargaComponentesFreteAuxiliar = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar();
                        cargaComponentesFreteAuxiliar.ComponenteFrete = componenteFrete;
                        cargaComponentesFreteAuxiliar.Carga = carga;
                        cargaComponentesFreteAuxiliar.ValorComponente = valorFrete.ICMS.ValorICMS;
                        repCargaComponentesFreteAuxiliar.Inserir(cargaComponentesFreteAuxiliar);
                    }
                }

                if (carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, false, false, true, _unitOfWork, _tipoServicoMultisoftware, configuracaoEmbarcador, configuracaoPedido);

                    if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador && (retornoFrete.situacao == SituacaoRetornoDadosFrete.ProblemaCalcularFrete))
                        servicoCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Frete.NaoFoiPossivelCalcularFreteCarga, carga.CodigoCargaEmbarcador), _unitOfWork);
                }
                else if (carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
                    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref carga, false, _tipoServicoMultisoftware, _unitOfWork, configuracaoEmbarcador);

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", $" Valor Informado = {carga.ValorFrete.ToString("n2")}", carga.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFrete));
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, _unitOfWork, null);
                servicoFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, TipoFreteEscolhido.Operador, _unitOfWork, false, _tipoServicoMultisoftware, _unitOfWork.StringConexao);

                carga.ValorFreteOperador = carga.ValorFrete;

                repositorioCarga.Atualizar(carga);

                servicoCargaJanelaCarregamento.AtualizarSituacao(carga, _tipoServicoMultisoftware);
                servicoCargaAprovacaoFrete.CriarAprovacao(carga, TipoRegraAutorizacaoCarga.InformadoManualmente, _tipoServicoMultisoftware);
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Atualizou valores de frete.", _unitOfWork);

                _unitOfWork.CommitChanges();

                if (carga.CargaAgrupamento != null)
                {
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento = carga.CargaAgrupamento;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAgrupados = repositorioCargaPedido.BuscarPorCarga(cargaAgrupamento.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = repositorioCarga.BuscarCargasOriginais(cargaAgrupamento.Codigo);

                    cargaAgrupamento.MotivoPendencia = "";
                    cargaAgrupamento.PossuiPendencia = false;
                    cargaAgrupamento.TipoFreteEscolhido = TipoFreteEscolhido.Operador;
                    cargaAgrupamento.ValorFrete = cargasAgrupadas.Sum(o => o.ValorFreteOperador);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentesCargaAgrupamento = repCargaComponentesFrete.BuscarPorCarga(cargaAgrupamento.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteCarga in componentesCargaAgrupamento)
                        repCargaComponentesFrete.Deletar(componenteCarga);

                    decimal valorICMSTotal = 0;
                    decimal valorTotalComponentes = 0;
                    foreach (var cargaAgrupada in cargasAgrupadas)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar> componentesCargasAgrupadas = repCargaComponentesFreteAuxiliar.BuscarPorCarga(cargaAgrupada.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar componeteCargaAgrupada in componentesCargasAgrupadas)
                        {
                            valorTotalComponentes += componeteCargaAgrupada.ValorComponente;

                            if (componeteCargaAgrupada.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                                valorICMSTotal += componeteCargaAgrupada.ValorComponente;
                            else
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete = repCargaComponentesFrete.BuscarPrimeiroPorCargaPorCompomente(cargaAgrupamento.Codigo, componeteCargaAgrupada.ComponenteFrete.TipoComponenteFrete, componeteCargaAgrupada.ComponenteFrete);

                                bool inserir = false;
                                if (cargaComponentesFrete == null)
                                {
                                    cargaComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();
                                    inserir = true;
                                }

                                cargaComponentesFrete.ComponenteFrete = componeteCargaAgrupada.ComponenteFrete;
                                cargaComponentesFrete.IncluirBaseCalculoICMS = true;
                                cargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                if (componeteCargaAgrupada.ComponenteFrete.ImprimirOutraDescricaoCTe)
                                    cargaComponentesFrete.OutraDescricaoCTe = componeteCargaAgrupada.ComponenteFrete.DescricaoCTe;
                                cargaComponentesFrete.ComponenteFilialEmissora = false;
                                cargaComponentesFrete.Carga = cargaAgrupamento;
                                cargaComponentesFrete.TipoComponenteFrete = componeteCargaAgrupada.ComponenteFrete.TipoComponenteFrete;
                                cargaComponentesFrete.ValorComponente += componeteCargaAgrupada.ValorComponente;

                                if (inserir)
                                    repCargaComponentesFrete.Inserir(cargaComponentesFrete);
                                else
                                    repCargaComponentesFrete.Atualizar(cargaComponentesFrete);
                            }
                        }
                    }

                    servicoRateioFrete.RatearValorDoFrenteEntrePedidos(cargaAgrupamento, cargaPedidosAgrupados, configuracaoEmbarcador, false, _unitOfWork, _tipoServicoMultisoftware);

                    if (cargaAgrupamento.EmpresaFilialEmissora != null || cargaAgrupamento.CalcularFreteCliente)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(cargaAgrupamento, false, false, true, _unitOfWork, _tipoServicoMultisoftware, configuracaoEmbarcador, configuracaoPedido);

                        if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador && (retornoFrete.situacao == SituacaoRetornoDadosFrete.ProblemaCalcularFrete))
                            servicoCarga.NotificarAlteracaoAoOperador(cargaAgrupamento, $"Não foi possível calcular o frete da carga n° {cargaAgrupamento.CodigoCargaEmbarcador}", _unitOfWork);
                    }
                    else if (cargaAgrupamento.TipoOperacao != null && cargaAgrupamento.TipoOperacao.EmiteCTeFilialEmissora && cargaAgrupamento.Filial != null && cargaAgrupamento.Filial.EmpresaEmissora != null)
                        Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref cargaAgrupamento, false, _tipoServicoMultisoftware, _unitOfWork, configuracaoEmbarcador);

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFreteCargaAgrupada = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", $" Valor Informado = {cargaAgrupamento.ValorFrete.ToString("n2")}", cargaAgrupamento.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, cargaAgrupamento.ValorFrete));
                    Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(cargaAgrupamento, null, null, null, false, composicaoFreteCargaAgrupada, _unitOfWork, null);
                    servicoFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(cargaAgrupamento, TipoFreteEscolhido.Operador, _unitOfWork, false, _tipoServicoMultisoftware, _unitOfWork.StringConexao);

                    if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().AtualizarValorFrete_AtualizarICMS.Value)
                    {
                        cargaAgrupamento.ValorFreteAPagar = cargaAgrupamento.ValorFrete + valorTotalComponentes;
                        cargaAgrupamento.ValorICMS = valorICMSTotal;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componenteICMSCarga = repCargaComponentesFrete.BuscarPorCargaPorCompomente(cargaAgrupamento.Codigo, TipoComponenteFrete.ICMS, null);
                        if (componenteICMSCarga != null && componenteICMSCarga.Count > 0)
                        {
                            componenteICMSCarga.FirstOrDefault().ValorComponente = valorICMSTotal;
                            repCargaComponentesFrete.Atualizar(componenteICMSCarga.FirstOrDefault());
                        }
                    }

                    cargaAgrupamento.ValorFreteOperador = cargaAgrupamento.ValorFrete;
                    repositorioCarga.Atualizar(cargaAgrupamento);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaAgrupamento, "Atualizou valores de frete nas cargas origens.", _unitOfWork);
                    _unitOfWork.CommitChanges();
                }

                if (valorFreteFilialEmissora != null)
                {
                    StringBuilder stMensagem = new StringBuilder();

                    _unitOfWork.Start();

                    if (valorFreteFilialEmissora.FreteProprio == 0 && valorFreteFilialEmissora.ValorTotalAReceber > 0)
                    {
                        bool incluirBase = false;
                        decimal baseCalculo = valorFreteFilialEmissora.ValorTotalAReceber;
                        decimal percentualIncluir = 100;

                        if (valorFreteFilialEmissora.ICMS != null)
                        {
                            if (valorFreteFilialEmissora.ICMS.CST == "60")
                            {
                                valorFreteFilialEmissora.FreteProprio = valorFreteFilialEmissora.ValorTotalAReceber;
                            }
                            else
                                valorFreteFilialEmissora.FreteProprio = valorFreteFilialEmissora.ValorTotalAReceber - valorFreteFilialEmissora.ICMS.ValorICMS;
                        }
                        else
                        {
                            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                                valorFreteFilialEmissora.FreteProprio = 0;
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.EmpresaFilialEmissora, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cargaPedido.BaseCalculoICMS, null, _unitOfWork, _tipoServicoMultisoftware, configuracaoEmbarcador);
                                if (regraICMS.CST == "60")
                                    valorFreteFilialEmissora.FreteProprio = valorFreteFilialEmissora.ValorTotalAReceber;
                                else
                                    valorFreteFilialEmissora.FreteProprio = valorFreteFilialEmissora.ValorTotalAReceber - regraICMS.ValorICMS;
                            }
                        }
                    }

                    cargaPedido.ValorFreteFilialEmissora = valorFreteFilialEmissora.FreteProprio;
                    cargaPedido.ValorFreteAPagarFilialEmissora = valorFreteFilialEmissora.ValorTotalAReceber;

                    if (valorFreteFilialEmissora.ICMS != null)
                    {
                        cargaPedido.PercentualAliquotaFilialEmissora = valorFreteFilialEmissora.ICMS.Aliquota;
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = valorFreteFilialEmissora.ICMS.IncluirICMSBC;
                        cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                        cargaPedido.ValorICMSFilialEmissora = valorFreteFilialEmissora.ICMS.ValorICMS;
                        cargaPedido.BaseCalculoICMSFilialEmissora = valorFreteFilialEmissora.ICMS.ValorBaseCalculoICMS;
                        cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = valorFreteFilialEmissora.ICMS.ObservacaoCTe;

                        bool incluirBC = cargaPedido.IncluirICMSBaseCalculo;
                        decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;

                        Dominio.Entidades.Empresa empresa = cargaPedido.Carga.EmpresaFilialEmissora;
                        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBC, ref percentualIncluirBaseCalculo, cargaPedido.BaseCalculoICMS, null, _unitOfWork, _tipoServicoMultisoftware, configuracaoEmbarcador);

                        carga.ValorICMSFilialEmissora += cargaPedido.ValorICMS;
                        cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                        if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                            cargaPedido.ObservacaoRegraICMSCTe += regraICMS.ObservacaoCTe;

                        if (!string.IsNullOrWhiteSpace(valorFreteFilialEmissora.ICMS.CST))
                        {
                            cargaPedido.CST = valorFreteFilialEmissora.ICMS.CST;
                            cargaPedido.PercentualIncluirBaseCalculo = valorFreteFilialEmissora.ICMS.PercentualInclusaoBC;
                            cargaPedido.PercentualReducaoBC = valorFreteFilialEmissora.ICMS.PercentualReducaoBC;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                cargaPedido.CST = regraICMS.CST;
                                cargaPedido.PercentualIncluirBaseCalculo = regraICMS.PercentualInclusaoBC;
                                cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                            }
                            else
                                stMensagem.Append("É obrigatório informar a CST");
                        }

                        if (cargaPedido.ValorFreteAPagarFilialEmissora == 0 && valorFreteFilialEmissora.ValorPrestacaoServico > 0)
                        {
                            if (regraICMS.DescontarICMSDoValorAReceber && cargaPedido.ValorICMSFilialEmissora > 0)
                                cargaPedido.ValorFreteAPagarFilialEmissora = valorFreteFilialEmissora.ValorPrestacaoServico - cargaPedido.ValorICMSFilialEmissora;
                            else
                                cargaPedido.ValorFreteAPagarFilialEmissora = valorFreteFilialEmissora.ValorPrestacaoServico;
                        }

                        if (regraICMS.SimplesNacional || valorFreteFilialEmissora.ICMS.SimplesNacional)
                            cargaPedido.CST = "";
                    }
                    else
                    {
                        cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = 100;
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = true;
                    }

                    decimal valorFreteLiquido = 0;
                    CriarComponentesCargaPedido(valorFreteFilialEmissora.ComponentesAdicionais, cargaPedido, cargaPedido.Pedido, true, 0, ref valorFreteLiquido, ref stMensagem, _tipoServicoMultisoftware, _unitOfWork);

                    if ((valorFreteFilialEmissora.ICMS == null && cargaPedido.PossuiCTe) || (valorFreteFilialEmissora.ISS == null && cargaPedido.PossuiNFS))
                    {
                        carga.ValorFreteFilialEmissora += cargaPedido.ValorFreteFilialEmissora;
                        if (cargaPedido.ValorFreteAPagarFilialEmissora > 0)
                            carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                        else
                            carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                    }
                    else
                    {
                        carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                        carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                    }

                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    repCarga.Atualizar(carga);

                    _unitOfWork.CommitChanges();
                }

                return Retorno<bool>.CriarRetornoSucesso(true, "Valores de frete atualizados com sucesso!");
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar o valor do frete.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void CriarComponentesCargaPedido(List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentesAdicionais, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool filialEmissora, decimal valorCotacao, ref decimal valorFreteLiquido, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (componentesAdicionais != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                repPedidoComponenteFrete.DeletarPorPedido(pedido.Codigo);
                repCargaPedidoComponentesFrete.DeletarPorCargaPedido(cargaPedido.Codigo, false);
                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in componentesAdicionais)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                    Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);
                    if (componenteFrete != null)
                    {
                        cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                        cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                        cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                        cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                        cargaPedidoComponenteFrete.DescontarValorTotalAReceber = componenteAdicional.DescontarValorTotalAReceber;
                        cargaPedidoComponenteFrete.TipoComponenteFrete = cargaPedidoComponenteFrete.ComponenteFrete.TipoComponenteFrete;
                        cargaPedidoComponenteFrete.ComponenteFilialEmissora = filialEmissora;

                        if (cargaPedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                        {
                            cargaPedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                            cargaPedidoComponenteFrete.Percentual = componenteAdicional.ValorComponente;
                            cargaPedidoComponenteFrete.ValorComponente = 0;
                        }
                        else
                        {
                            cargaPedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                            cargaPedidoComponenteFrete.ValorComponente = valorCotacao > 0 ? (componenteAdicional.ValorComponente * valorCotacao) : componenteAdicional.ValorComponente;
                        }

                        repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);

                        bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(cargaPedido.Carga?.TabelaFrete, cargaPedidoComponenteFrete.ComponenteFrete);
                        bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? cargaPedido.Carga?.TabelaFrete?.DescontarComponenteFreteLiquido : cargaPedidoComponenteFrete.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false;

                        if (cargaPedidoComponenteFrete.ComponenteFrete.SomarComponenteFreteLiquido)
                            valorFreteLiquido += valorCotacao > 0 ? (componenteAdicional.ValorComponente * valorCotacao) : componenteAdicional.ValorComponente;

                        if (descontarComponenteFreteLiquido)
                            valorFreteLiquido += valorCotacao > 0 ? ((componenteAdicional.ValorComponente * -1) * valorCotacao) : (componenteAdicional.ValorComponente * -1);

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

        private List<string> ObterListaCnpjClientes(List<Dominio.Entidades.Cliente> listaClientes)
        {
            if (listaClientes == null || listaClientes.Count == 0)
                return new List<string>();

            return (from obj in listaClientes select obj.CPF_CNPJ_Formatado).ToList();
        }

        private RetornoObterAgendamentos ConverterRetornoAgendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasDescarregamento)
        {
            Servicos.WebService.Filial.Filial serWSFilial = new Filial.Filial(_unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serWSPessoas = new Pessoas.Pessoa(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento janelaDescarregamento = janelasDescarregamento.FirstOrDefault(x => x.Carga.Codigo == agendamento.AgendamentoColeta.Carga.Codigo);

            decimal valorMedioCaixa = Math.Round(agendamento.Pedido.ValorTotalNotasFiscais / (agendamento.Pedido?.QtVolumes ?? 1), 2);
            string pattern = "dd/MM/yyyy HH:mm";

            return new RetornoObterAgendamentos()
            {
                DataDescarregamento = janelaDescarregamento.InicioDescarregamento.ToDateString(),
                HoraDescarregamento = janelaDescarregamento.InicioDescarregamento.ToTimeString(true),
                DataTentativa = agendamento.AgendamentoColeta.DataAgendamento?.ToString(pattern) ?? "",
                Senha = agendamento.AgendamentoColeta.Senha,
                Fornecedor = serWSPessoas.ConverterObjetoPessoa(agendamento.AgendamentoColeta.Remetente),
                NumeroPedidoEmbarcador = agendamento.Pedido.NumeroPedidoEmbarcador,
                Modalidade = agendamento.Pedido.TipoDeCarga?.Descricao,
                Filial = serWSFilial.ConverterObjetoFilial(agendamento.AgendamentoColeta.Carga.Filial),
                QuantidadeCaixas = agendamento.VolumesEnviar,
                QuantidadeItens = agendamento.SKU,
                ModeloVeicular = agendamento.AgendamentoColeta.ModeloVeicular?.Descricao,
                SituacaoAgendamento = janelaDescarregamento.Situacao,
                AgendaExtra = agendamento.AgendamentoColeta.Carga.AgendaExtra,
                ResponsavelConfirmacao = agendamento.AgendamentoColeta.ResponsavelConfirmacao?.Nome,
                NumeroCarga = agendamento.AgendamentoColeta.Carga.CodigoCargaEmbarcador,
                DataConfirmacao = janelaDescarregamento.DataConfirmacao?.ToString(pattern) ?? "",
                DataSolicitacao = agendamento.AgendamentoColeta.Carga.DataCriacaoCarga.ToString(pattern),
                ValorTotalPedido = Math.Round(agendamento.Pedido.ValorTotalNotasFiscais, 2),
                QuantidadeCaixasPedido = agendamento.Pedido.QtVolumes,
                ValorMedioCaixa = valorMedioCaixa,
                ValorAgendado = agendamento.VolumesEnviar * valorMedioCaixa,
                OperadorAgendamento = agendamento.AgendamentoColeta.Solicitante?.Nome ?? agendamento.AgendamentoColeta.Carga.Operador?.Nome,
                GrupoProduto = agendamento.Pedido.ProdutoPrincipal?.GrupoProduto?.Descricao,
                TipoCarga = agendamento.Pedido.TipoDeCarga?.Descricao,
                Observacao = agendamento.Pedido.Observacao
            };
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido> ConsultarSituacaoPedido(int protocoloIntegracaoPedido)
        {
            Servicos.Log.TratarErro("ConsultarSituacaoPedido - protocoloIntegracaoPedido: " + protocoloIntegracaoPedido.ToString());
            string erro = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocoloIntegracaoPedido);

                if (pedido == null)
                    throw new WebServiceException("O pedido protocolo " + protocoloIntegracaoPedido + " não localizado.");

                Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido situacaoPedido = new Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido
                {
                    Codigo = (int)pedido.SituacaoPedido,
                    Descricao = pedido.SituacaoPedido.ObterDescricao()
                };

                return Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido>.CriarRetornoSucesso(situacaoPedido);
            }
            catch (BaseException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido>.CriarRetornoDadosInvalidos(excecao.Message, null);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.WebService.Pedido.SituacaoPedido>.CriarRetornoExcecao(!string.IsNullOrWhiteSpace(erro) ? erro : "Ocorreu uma falha ao consultar situação do pedido", null);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
