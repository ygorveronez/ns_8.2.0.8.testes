using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.DocumentoTransporte
{
    public class ProcessarDocumentoTransporte
    {
        private static void ProcessarModeloVeicularCarga(Dominio.Entidades.Embarcador.Pedidos.Stage stage, Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular modeloVeicular, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modeloVeicularCadastrados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            if (modeloVeicular == null)
                return;

            if (stage.ModeloVeicularCarga == null)
                stage.ModeloVeicularCarga = modeloVeicularCadastrados.Where(x => x.CodigoIntegracao == modeloVeicular.CodigoIntegracao).FirstOrDefault();

            if (stage.ModeloVeicularCarga != null && stage?.ModeloVeicularCarga?.CodigoIntegracao != modeloVeicular.CodigoIntegracao)
                stage.ModeloVeicularCarga = modeloVeicularCadastrados.Where(x => x.CodigoIntegracao == modeloVeicular.CodigoIntegracao).FirstOrDefault();

            if (stage.ModeloVeicularCarga == null && string.IsNullOrEmpty(modeloVeicular.Descricao))
                throw new ServicoException($"O modelo veicular {modeloVeicular.CodigoIntegracao} informado não está cadastrado no MultiEmbarcado");

            if (stage.ModeloVeicularCarga == null && !string.IsNullOrEmpty(modeloVeicular.Descricao))
            {
                var novoModeloVeicular = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();

                novoModeloVeicular.CodigoIntegracao = modeloVeicular.CodigoIntegracao;
                novoModeloVeicular.Descricao = modeloVeicular.Descricao;
                novoModeloVeicular.Ativo = true;
                novoModeloVeicular.CapacidadePesoTransporte = 30000;
                novoModeloVeicular.ToleranciaPesoExtra = 5000;
                novoModeloVeicular.ToleranciaPesoMenor = 1;
                novoModeloVeicular.VeiculoPaletizado = false;
                novoModeloVeicular.ModeloControlaCubagem = false;
                novoModeloVeicular.ModeloTracaoReboquePadrao = false;
                novoModeloVeicular.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral;
                repModeloVeicularCarga.Inserir(novoModeloVeicular);

                stage.ModeloVeicularCarga = novoModeloVeicular;
            }
        }

        private static void SalvarEnderencoClientesStage(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Cliente clienteStage)
        {

            if (cliente == null)
                return;

            if (!clienteStage.SalvarEndereco)
                return;

            if (cliente.NaoAtualizarDados)
                return;

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (clienteStage.Bairro != null && !clienteStage.Bairro.Equals(cliente?.Bairro))
                cliente.Bairro = !string.IsNullOrWhiteSpace(clienteStage?.Bairro) && clienteStage?.Bairro.Length > 40 ? clienteStage?.Bairro.Substring(0, 40) : clienteStage?.Bairro;

            if (clienteStage.CEP != null && !clienteStage.CEP.Equals(cliente?.CEP))
                cliente.CEP = clienteStage?.CEP ?? string.Empty;

            if (clienteStage.Complemento != null && !clienteStage.Complemento.Equals(cliente?.Complemento))
                cliente.Complemento = clienteStage?.Complemento ?? string.Empty;

            if (clienteStage.Endereco != null && !clienteStage.Endereco.Equals(cliente?.Endereco))
                cliente.Endereco = clienteStage?.Endereco ?? string.Empty;

            if (cliente.Localidade.Codigo != clienteStage?.Localidade && clienteStage?.Localidade > 0)
                cliente.Localidade = repositorioLocalidade.BuscarPorCodigo(clienteStage?.Localidade ?? 0);

            if (clienteStage.Numero != null && !clienteStage.Numero.Equals(cliente?.Numero))
                cliente.Numero = clienteStage?.Numero ?? string.Empty;

            if (clienteStage.Telefone1 != null && !clienteStage.Telefone1.Equals(cliente?.Telefone1))
                cliente.Telefone1 = clienteStage?.Telefone1 ?? string.Empty;


            repositorioCliente.Atualizar(cliente);
        }

        private static Dominio.Entidades.Cliente ObterCliente(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Cliente clienteStage, string numeroEtapa, string tipo)
        {

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(unitOfWork.StringConexao);

            if (!string.IsNullOrWhiteSpace(clienteStage.CPFCNPJ))
            {
                double cnpjcliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(clienteStage.CPFCNPJ), out cnpjcliente);

                if (cliente != null && cliente.CPF_CNPJ == cnpjcliente)
                {
                    SalvarEnderencoClientesStage(unitOfWork, cliente, clienteStage);
                    return cliente;
                }
                else
                {
                    cliente = repCliente.BuscarPorCPFCNPJ(cnpjcliente);
                    if (cliente == null && (clienteStage?.SalvarEndereco ?? false))
                    {
                        string retorno = servicoCliente.CadastrarCliente(ref cliente, clienteStage, unitOfWork);
                        if (!string.IsNullOrWhiteSpace(retorno))
                            throw new ServicoException(retorno);
                    }
                    else if (cliente == null)
                        throw new ServicoException($"{tipo} da Etapa {numeroEtapa} não esta cadastrado no MultiEmbarcador");

                    SalvarEnderencoClientesStage(unitOfWork, cliente, clienteStage);
                    return cliente;
                }
            }
            string codigoIntegracao = clienteStage.CodigoIntegracao;

            if (codigoIntegracao == string.Empty)
                throw new ServicoException($"{tipo} da Etapa {numeroEtapa} não esta cadastrado no MultiEmbarcador");

            if (codigoIntegracao == null)
                return null;


            if (cliente != null && cliente.CodigoIntegracao == codigoIntegracao)
            {
                SalvarEnderencoClientesStage(unitOfWork, cliente, clienteStage);
                return cliente;
            }

            if (cliente != null && cliente.CodigoIntegracao != codigoIntegracao && !string.IsNullOrEmpty(codigoIntegracao))
            {
                Dominio.Entidades.Cliente existeCliente = repCliente.BuscarPorCodigoIntegracao(codigoIntegracao) ?? throw new ServicoException($"{tipo} da Etapa {numeroEtapa} não esta cadastrado no MultiEmbarcador");
                SalvarEnderencoClientesStage(unitOfWork, cliente, clienteStage);
                return existeCliente;
            }

            if (cliente == null)
            {
                cliente = repCliente.BuscarPorCodigoIntegracao(codigoIntegracao);

                if (cliente == null)
                    cliente = repCliente.BuscarPorCodigoIntegracaoComEndereco(codigoIntegracao);

                if (cliente == null && (clienteStage?.SalvarEndereco ?? false))
                {
                    string retorno = servicoCliente.CadastrarCliente(ref cliente, clienteStage, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ServicoException(retorno);
                }
                else if (cliente == null)
                    throw new ServicoException($"{tipo} da Etapa {numeroEtapa} não esta cadastrado no MultiEmbarcador");

                SalvarEnderencoClientesStage(unitOfWork, cliente, clienteStage);
            }

            if (cliente.Tipo == null)
                cliente.Tipo = cliente.CPF_CNPJ.ToString()?.Length == 14 ? "J" : "F";

            return cliente;
        }

        private static Dominio.Entidades.Empresa AtualizarTransportadorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte, Repositorio.UnitOfWork unitOfWork, ref bool removerFreteEmbarcador)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = carga.Empresa;

            if ((documentoTransporte.TransportadoraEmitente == null || string.IsNullOrEmpty(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao)) && empresa != null)
                return empresa;

            if (empresa != null && documentoTransporte.TransportadoraEmitente.CodigoIntegracao != empresa.CodigoIntegracao)
            {
                removerFreteEmbarcador = true;
                empresa = repositorioEmpresa.BuscarPorCodigoIntegracao(documentoTransporte.TransportadoraEmitente.CodigoIntegracao);
            }

            return empresa;
        }

        public static void AtualizarDadosSumarizadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            try
            {
                Servicos.Log.TratarErro($"Iniciando AtualizarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoDadosSumarzados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados = carga.DadosSumarizados ?? new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> listaStage = new List<Dominio.Entidades.Embarcador.Pedidos.Stage>();

                if (dadosSumarizados.Codigo == 0 || dadosSumarizados == null)
                    throw new Exception($"Não existe dadosSumarizados na carga.");

                List<int> cargas = new List<int> { carga.Codigo };

                cargaPedidosBase = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork).BuscarPorCargasFetchBasico(cargas);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargaPedidosBase where !obj.PedidoPallet && obj.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select obj).ToList();

                if (cargaPedidos.Count() == 0)
                    throw new Exception($"Não existe pedidos carga.");


                List<string> expedidores = new List<string>();
                expedidores = (from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas == null select obj.Expedidor.Descricao).Distinct().ToList();
                expedidores.AddRange((from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas != null select obj.Expedidor.GrupoPessoas.Descricao).Distinct());

                Servicos.Log.TratarErro($"Retornado os seguintes expedidores: {string.Join(" / ", expedidores)} AtualizarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

                List<string> recebedores = new List<string>();
                bool carregamentoSegundoTrecho = (carga?.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false);
                if (!carregamentoSegundoTrecho)
                {
                    recebedores = (from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas == null select obj.Recebedor.Descricao).Distinct().ToList();
                    recebedores.AddRange((from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas != null select obj.Recebedor.GrupoPessoas.Descricao).Distinct());

                    Servicos.Log.TratarErro($"Retornado os seguintes recebedores: {string.Join(" / ", recebedores)} AtualizarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");
                }

                if (expedidores.Count() == 0 || recebedores.Count() == 0)
                {
                    listaStage = repStage.BuscarStagesPorCarga(carga.Codigo);

                    if (listaStage.Count() == 0)
                        throw new Exception($"Não existe stages para carga.");

                    foreach (var stage in listaStage)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaPedidoStage = repPedidoStage.BuscarPorStageFetch(stage.Codigo);

                        if (listaPedidoStage.Count() == 0)
                            throw new Exception($"Não existe pedidos stage para carga. Stage: {stage.Codigo}");

                        foreach (var pedidoStage in listaPedidoStage)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAtualizar = cargaPedidos.Where(x => x.Pedido.Codigo == pedidoStage.Pedido.Codigo).ToList();

                            foreach (var pedidoAtualizar in cargaPedidosAtualizar)
                            {
                                pedidoAtualizar.Expedidor = stage.Expedidor;
                                pedidoAtualizar.Recebedor = stage.Recebedor;

                                repCargaPedido.Atualizar(pedidoAtualizar);
                            }

                            servicoDadosSumarzados.AtualizarOrigensEDestinos(dadosSumarizados, carga, cargaPedidosAtualizar, unitOfWork, tipoServicoMultisoftware);
                        }
                    }

                    expedidores = (from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas == null select obj.Expedidor.Descricao).Distinct().ToList();
                    expedidores.AddRange((from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas != null select obj.Expedidor.GrupoPessoas.Descricao).Distinct());

                    Servicos.Log.TratarErro($"Atualizado os seguintes expedidores: {string.Join(" / ", expedidores)} AtualizarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

                    if (!carregamentoSegundoTrecho)
                    {
                        recebedores = (from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas == null select obj.Recebedor.Descricao).Distinct().ToList();
                        recebedores.AddRange((from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas != null select obj.Recebedor.GrupoPessoas.Descricao).Distinct());

                        Servicos.Log.TratarErro($"Atualizado os seguintes recebedores: {string.Join(" / ", recebedores)} AtualizarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");
                    }

                    dadosSumarizados.Distancia = servicoCarga.ObterDistancia(carga, configuracaoTMS, unitOfWork);
                }

                dadosSumarizados.Recebedores = string.Join(" / ", recebedores);
                dadosSumarizados.Expedidores = string.Join(" / ", expedidores);

                if (dadosSumarizados.Codigo > 0)
                    repCargaDadosSumarizados.Atualizar(dadosSumarizados);

                Servicos.Log.TratarErro($"Finalizando AtualizarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");
            }
            catch (Exception ex)
            {
                throw new ServicoException($"Erro ao AtualizarDadosSumarizadosCarga. Carga código: {carga.Codigo}. {ex.Message}");
            }
        }

        public static Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaPorDocumentoTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, string adminStringConexao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalVenda repCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario repCargaPedidoxmlTemporario = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido repStageAgrupamentoRemovido = new Repositorio.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repositorioTipocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete repStageAgrupamentoComposicao = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE cargaPedidoRecusaCTE = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado servCalculoFreteStages = new Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado(unitOfWork, tipoServicoMultisoftware, configuracaoTMS);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever servicoIntegracaoUnilever = new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.RecusaCargaCTe servicoRecusaCte = new Servicos.Embarcador.Carga.RecusaCargaCTe(unitOfWork, tipoServicoMultisoftware, configuracaoTMS, auditado);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

            List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage> stagesCarga = new List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage>();
            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesSalvas = new List<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            List<string> numerosStageDistintos = new List<string>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            if (carga == null)
                throw new ServicoException($"Não encontrada carga com o protocolo {carga.Codigo}.");

            if (documentoTransporte == null)
                throw new ServicoException($"Não foi possível carregar dados da integração.");

            if (carga.Filial == null && documentoTransporte?.Filial?.Codigo > 0)
                carga.Filial = repFilial.buscarPorCodigoEmbarcador(documentoTransporte.Filial.Codigo.ToString());

            if (carga.Filial == null)
                throw new ServicoException($"Não encontrada filial para carga.");

            Servicos.Log.TratarErro($"INICIOU O GERAR GerarCargaPorDocumentoTransporte referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOld = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosSalvos = cargaPedidosOld.ToList();

            foreach (var pedidoDocumentoTransporte in documentoTransporte.Pedido)
                stagesCarga.AddRange(pedidoDocumentoTransporte.Stage);

            decimal distanciaStages = 0;
            bool adicionouNovoPedido = false;
            bool removerTransportes = false;
            bool atualizouInicioViagemPrevisto = false;
            bool removerFreteEmbarcador = false;
            List<int> listaStatusVpEmbarcadorValidos = new List<int>() { 920, 924 };

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaOld = carga.ModeloVeicularCarga;

            Dominio.Entidades.Empresa transportadorNovo = null;
            if (!string.IsNullOrWhiteSpace(documentoTransporte.TransportadoraEmitente?.CNPJ))
                transportadorNovo = repEmpresa.BuscarEmpresaPorCNPJ(documentoTransporte.TransportadoraEmitente?.CNPJ);
            else if (!string.IsNullOrWhiteSpace(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao))
                transportadorNovo = repEmpresa.BuscarPorCodigoIntegracao(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao);

            if (transportadorNovo != null && carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && carga.Empresa != transportadorNovo)
                removerTransportes = true;

            if (stagesCarga.Count() > 0)
            {
                //salva STAGES agrupadas pelo numero, tambem verifica se ja existe por Numero e Carga DT;
                numerosStageDistintos = stagesCarga.Select(obj => obj.NumeroStage).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> StagesExistentes = repStage.BuscarStagesPorNumeroECargaDT(numerosStageDistintos, carga.Codigo);

                List<Dominio.ObjetosDeValor.Veiculo> veiculos = stagesCarga.Where(obj => obj.Veiculo != null).Select(obj => obj.Veiculo).Distinct().ToList();
                List<Dominio.Entidades.Veiculo> VeiculosStagesCadastrados = repVeiculo.BuscarPorCodigo(veiculos.Select(x => x.Codigo).Distinct().ToList());

                List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> transportadoresStages = stagesCarga.Where(obj => obj.TransportadoraEmitente != null).Select(obj => obj.TransportadoraEmitente).Distinct().ToList();
                List<Dominio.Entidades.Empresa> EmpresasCadastradas = repEmpresa.BuscarPorCodigosIntegracao(transportadoresStages.Select(x => x.CodigoIntegracao).Distinct().ToList());

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.CanalVenda> canaisVendaStages = stagesCarga.Where(obj => obj.CanalVenda != null).Select(obj => obj.CanalVenda).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> CanalVendaCadastrados = repCanalVenda.BuscarPorCodigosIntegracao(canaisVendaStages.Select(x => x.CodigoIntegracao).Distinct().ToList());

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.CanalEntrega> canaisEntregaStages = stagesCarga.Where(obj => obj.CanalEntrega != null).Select(obj => obj.CanalEntrega).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> CanalEntregaCadastrados = repCanalEntrega.BuscarPorCodigosIntegracao(canaisEntregaStages.Select(x => x.CodigoIntegracao).Distinct().ToList());

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> modelosVeicularStages = stagesCarga.Where(obj => obj.ModeloVeicular != null).Select(obj => obj.ModeloVeicular).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModeloVeicularCadastrados = repModeloVeicularCarga.BuscarPorCodigosIntegracao(modelosVeicularStages.Select(x => x.CodigoIntegracao).Distinct().ToList());


                foreach (string numero in numerosStageDistintos)
                {
                    Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage firstStage = stagesCarga.Where(obj => obj.NumeroStage == numero).FirstOrDefault();

                    Dominio.Entidades.Embarcador.Pedidos.Stage existeStage = (from obj in StagesExistentes where obj.NumeroStage == firstStage.NumeroStage select obj).FirstOrDefault();

                    if (existeStage == null)
                        existeStage = new Dominio.Entidades.Embarcador.Pedidos.Stage();
                    else
                    {
                        if (existeStage.TipoPercurso != firstStage.TipoPercurso)
                            removerTransportes = true;
                    }

                    existeStage.NumeroStage = firstStage.NumeroStage;
                    existeStage.Distancia = firstStage.Distancia;
                    existeStage.OrdemEntrega = firstStage.OrdemEntrega;
                    existeStage.NumeroVeiculo = firstStage.NumeroVeiculo;
                    existeStage.TipoPercurso = firstStage.TipoPercurso;
                    existeStage.TipoModal = firstStage.TipoModal;
                    existeStage.Agrupamento = firstStage.Agrupamento;
                    existeStage.RelevanciaCusto = firstStage.RelevanciaCusto;
                    existeStage.NaoPossuiValePedagio = firstStage.NaoPossuiValePedagio;
                    existeStage.Processado = false;
                    existeStage.StatusVPEmbarcador = firstStage.StatusVPEmbarcador;
                    existeStage.CargaDT = carga;
                    existeStage.Veiculo = firstStage.Veiculo != null ? VeiculosStagesCadastrados.Where(x => x.Codigo == firstStage.Veiculo.Codigo).FirstOrDefault() : null;
                    existeStage.EmpresaEmitente = firstStage.TransportadoraEmitente != null ? EmpresasCadastradas.Where(x => x.CodigoIntegracao == firstStage.TransportadoraEmitente.CodigoIntegracao).FirstOrDefault() : null; //criar lista
                    existeStage.RelevanteVP = !((existeStage.NaoPossuiValePedagio && existeStage.StatusVPEmbarcador == 0) || (!existeStage.NaoPossuiValePedagio && listaStatusVpEmbarcadorValidos.Contains(existeStage.StatusVPEmbarcador)));

                    if (firstStage.Expedidor != null)
                    {
                        Dominio.Entidades.Cliente expedidorAnterior = existeStage.Expedidor;
                        existeStage.Expedidor = ObterCliente(unitOfWork, existeStage.Expedidor, firstStage.Expedidor, firstStage?.NumeroStage, "Expedidor");

                        if (existeStage.Expedidor?.Codigo != expedidorAnterior?.Codigo && existeStage.Codigo > 0)
                        {
                            removerFreteEmbarcador = true;
                            removerTransportes = true;
                        }
                    }

                    if (firstStage.Recebedor != null)
                    {
                        Dominio.Entidades.Cliente recebedorAnterior = existeStage.Recebedor;
                        existeStage.Recebedor = ObterCliente(unitOfWork, existeStage.Recebedor, firstStage.Recebedor, firstStage?.NumeroStage, "Recebedor");

                        if (existeStage.Recebedor?.Codigo != recebedorAnterior?.Codigo && existeStage.Codigo > 0)
                        {
                            removerFreteEmbarcador = true;
                            removerTransportes = true;
                        }
                    }

                    if (existeStage?.CanalVenda == null)
                    {
                        if (firstStage.CanalVenda != null)
                        {
                            removerFreteEmbarcador = true;
                            existeStage.CanalVenda = CanalVendaCadastrados.Where(x => x.CodigoIntegracao == firstStage.CanalVenda.CodigoIntegracao).FirstOrDefault() ?? throw new ServicoException($"O canal de venda {firstStage.CanalVenda.CodigoIntegracao} da stage {firstStage.NumeroStage} não esta cadastrado no MultiEmbarcado");
                        }
                    }

                    if (existeStage?.CanalVenda != null && firstStage.CanalVenda != null && existeStage.CanalVenda.CodigoIntegracao != firstStage.CanalVenda?.CodigoIntegracao)
                    {
                        removerFreteEmbarcador = true;
                        existeStage.CanalVenda = CanalVendaCadastrados.Where(x => x.CodigoIntegracao == firstStage.CanalVenda.CodigoIntegracao).FirstOrDefault() ?? throw new ServicoException($"O canal de venda {firstStage.CanalVenda.CodigoIntegracao} da stage {firstStage.NumeroStage} não esta cadastrado no Mulembarcador");
                    }

                    if (existeStage?.CanalEntrega == null)
                    {
                        if (firstStage.CanalEntrega != null && !string.IsNullOrEmpty(firstStage.CanalEntrega.CodigoIntegracao))
                        {
                            removerFreteEmbarcador = true;
                            existeStage.CanalEntrega = CanalEntregaCadastrados.Where(x => x.CodigoIntegracao == firstStage.CanalEntrega.CodigoIntegracao).FirstOrDefault();
                        }
                    }

                    if (existeStage?.CanalEntrega != null && firstStage.CanalEntrega != null && existeStage.CanalEntrega.CodigoIntegracao != firstStage.CanalEntrega.CodigoIntegracao)
                    {
                        removerFreteEmbarcador = true;
                        existeStage.CanalEntrega = CanalEntregaCadastrados.Where(x => x.CodigoIntegracao == firstStage.CanalEntrega.CodigoIntegracao).FirstOrDefault();
                    }

                    if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                    {
                        Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage existeStageMesmoNumeroComPercursoDifrente = stagesCarga.Where(obj => obj.NumeroStage == firstStage.NumeroStage && obj.TipoPercurso != firstStage.TipoPercurso).FirstOrDefault();
                        if (existeStageMesmoNumeroComPercursoDifrente != null)
                            throw new ServicoException($"Atenção: Stage {existeStageMesmoNumeroComPercursoDifrente.NumeroStage} está com Tipo Percurso diferente dos demais registros com mesmo número");
                    }

                    if ((carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && carga.TipoOperacao?.TipoOperacaoRedespacho != null)
                    {
                        //na geracao do redespacho, a carga DT s? possui stage 1, para isso a distancia apenas da 1
                        if (existeStage.NumeroStage.ToInt() == 1)
                        {
                            if (existeStage.Recebedor != null && existeStage.TipoPercurso != Vazio.PercursoRegreso)
                                distanciaStages = !stagesSalvas.Any(obj => obj.Recebedor?.Codigo == existeStage.Recebedor.Codigo) ? distanciaStages += existeStage.Distancia : distanciaStages;
                            else
                                distanciaStages += existeStage.Distancia;
                        }
                    }
                    else
                    {
                        //soma distancia apenas se recebedor ainda nao foi adicionado
                        if (existeStage.Recebedor != null && existeStage.TipoPercurso != Vazio.PercursoRegreso)
                            distanciaStages = !stagesSalvas.Any(obj => obj.Recebedor?.Codigo == existeStage.Recebedor.Codigo) ? distanciaStages += existeStage.Distancia : distanciaStages;
                        else
                            distanciaStages += existeStage.Distancia;
                    }

                    ProcessarModeloVeicularCarga(existeStage, firstStage.ModeloVeicular, ModeloVeicularCadastrados, unitOfWork);

                    if (existeStage.Codigo > 0)
                        repStage.Atualizar(existeStage);
                    else
                    {
                        repStage.Inserir(existeStage);
                        StagesExistentes.Add(existeStage);
                    }

                    stagesSalvas.Add(existeStage);
                }

                var stage = stagesSalvas.FirstOrDefault();

                if (stage != null && stage.ModeloVeicularCarga != null)
                    carga.ModeloVeicularCarga = ModeloVeicularCadastrados.Where(x => x.CodigoIntegracao == stage.ModeloVeicularCarga.CodigoIntegracao).FirstOrDefault();

            }

            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            List<(Dominio.Entidades.Embarcador.Pedidos.Stage, int)> listaStagesRemover = new List<(Dominio.Entidades.Embarcador.Pedidos.Stage, int)>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoRecusa = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosProtocolos = repPedido.BuscarPorProtocolos(documentoTransporte.Pedido.Select(x => int.Parse(x.ProtocoloPedido)).Distinct().ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaGeralStagesPedido = repPedidoStage.BuscarPorPedidoSECargaStage(pedidosProtocolos.Select(x => x.Codigo).Distinct().ToList(), carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> listaGeralStagesAgrupamento = repStageAgrupamento.BuscarPorCodigosFetch(listaGeralStagesPedido.Where(x => x.Stage?.StageAgrupamento != null).Select(x => x.Stage.StageAgrupamento.Codigo).Distinct().ToList());
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario> cargaPedidoXmlTemporario = repCargaPedidoxmlTemporario.BuscarPorPedidos(documentoTransporte.Pedido.Select(x => int.Parse(x.ProtocoloPedido)).Distinct().ToList());

            foreach (var pedidoDocumentoTransporte in documentoTransporte.Pedido)
            {
                int.TryParse(pedidoDocumentoTransporte.ProtocoloPedido, out int protocoloPedido);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidosProtocolos.Where(x => x.Protocolo == protocoloPedido).FirstOrDefault();

                if (pedido == null)
                    throw new ServicoException($"Pedido protocolo {pedidoDocumentoTransporte.ProtocoloPedido} não localizado.");

                if (pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado)
                    throw new ServicoException($"Pedido protocolo {pedidoDocumentoTransporte.ProtocoloPedido} está cancelado.");

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedido = listaGeralStagesPedido.Where(x => x.Pedido.Codigo == pedido.Codigo).ToList(); //repPedidoStage.BuscarPorPedidoECargaStage(pedido.Codigo, carga.Codigo);

                if (listaStagesPedido == null || listaStagesPedido.Count <= 0)
                    listaStagesPedido = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

                List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Pedido> objPedidos = documentoTransporte.Pedido.Where(obj => obj.ProtocoloPedido == pedidoDocumentoTransporte.ProtocoloPedido).ToList();
                if (listaStagesPedido.Count > 0)
                {
                    List<string> numeroStages = new List<string>();

                    List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage> stagesPedido = objPedidos.SelectMany(x => x.Stage).ToList();

                    foreach (var objStage in stagesPedido)
                        numeroStages.Add(objStage.NumeroStage);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStageRemover = listaStagesPedido.Where(o => !numeroStages.Contains(o.Stage.NumeroStage)).ToList();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoStage stageRemover in listaStageRemover)
                    {
                        repPedidoStage.ExecuteDeletarPorStagePedido(stageRemover.Stage.Codigo, stageRemover.Pedido.Codigo);
                        bool possuiRecusa = false;

                        if (!stagesSalvas.Where(j => j.Codigo == stageRemover.Stage.Codigo).Any() && !listaStagesPedidos.Where(x => x.Stage.Codigo == stageRemover.Stage.Codigo).Any())
                        {
                            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = listaGeralStagesAgrupamento.Where(x => x.Codigo == (stageRemover.Stage.StageAgrupamento?.Codigo ?? 0)).FirstOrDefault(); //repStageAgrupamento.BuscarPorCodigo((stageRemover.Stage.StageAgrupamento?.Codigo ?? 0), false);

                            if (agrupamento != null && agrupamento.CargaGerada != null)
                            {
                                //ja temos carga gerada para o agrupamento, precisa verificar e encontrar o cargaPedido do pedido e remover o mesmo;
                                //guardar em nova tabela valor frete do agrupamento e as informa??es para um possivel vinculo posterior deste pedido a outra carga;
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRemover = stageRemover.Pedido;
                                if (pedidoRemover != null)
                                {
                                    if (carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn && !cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECarga(pedidoRemover.Protocolo, carga.Codigo))
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRemover = repCargaPedido.BuscarPorCargaEPedido(agrupamento.CargaGerada.Codigo, pedidoRemover.Codigo);

                                        if (cargaPedidoRemover.CTesEmitidos)
                                            throw new ServicoException($"N?o foi possivel remover pedido ${cargaPedidoRemover.Pedido.NumeroPedidoEmbarcador} porque ja possui CT-e Emitido");

                                        if (cargaPedidoRemover != null)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido StageAgrupamentoRemovido = repStageAgrupamentoRemovido.BuscarPorPedido(pedidoRemover.Codigo);
                                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCte.BuscarPorCargaPedido(cargaPedidoRemover.Codigo);

                                            if (StageAgrupamentoRemovido == null)
                                                StageAgrupamentoRemovido = new Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoPedidoRemovido();

                                            StageAgrupamentoRemovido.ValorComponentes = agrupamento.ValorComponentes;
                                            StageAgrupamentoRemovido.ValorFreteTotal = cargaPedidoRemover.ValorFrete;
                                            StageAgrupamentoRemovido.Pedido = pedidoRemover;
                                            StageAgrupamentoRemovido.CTe = cargaCTe?.CTe ?? null;

                                            if (StageAgrupamentoRemovido.Codigo == 0)
                                                repStageAgrupamentoRemovido.Inserir(StageAgrupamentoRemovido);
                                            else
                                                repStageAgrupamentoRemovido.Atualizar(StageAgrupamentoRemovido);

                                            cargaPedidoRemover.StageRelevanteCusto = null;
                                            repCargaPedido.Atualizar(cargaPedidoRemover);

                                            //Removendo antes porque esta gerando Deadlock
                                            Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaPedidoRemover.Carga, cargaPedidoRemover, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, null, true, true);
                                        }
                                    }
                                    else
                                    {
                                        // pedido de recusa de CTE sempre validando pela carga mae.
                                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRecusa = repCargaPedido.BuscarPorCargaEPedido(carga.Codigo, pedidoRemover.Codigo);
                                        if (cargaPedidoRecusa != null && cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECarga(cargaPedidoRecusa.Pedido.Protocolo, cargaPedidoRecusa.Carga.Codigo) && !listaCargaPedidoRecusa.Contains(cargaPedidoRecusa))
                                        {
                                            possuiRecusa = true;
                                            listaCargaPedidoRecusa.Add(cargaPedidoRecusa);
                                        }
                                    }
                                }
                            }

                            //caso esta tentado remover uma stage q faz parte desta carga nao pode remover pois ela deve fazer parte de outro pedido
                            if (!stagesSalvas.Contains(stageRemover.Stage))
                                listaStagesRemover.Add((stageRemover.Stage, stageRemover.Pedido.Codigo));

                            //aqui remover vinculo da stage agrupada caso nao tem stages no agrupamento
                            if (agrupamento != null && repStage.ContarStagesPorAgrupamento(agrupamento.Codigo) == 0 && !possuiRecusa)
                            {
                                if (agrupamento.CargaGerada != null)
                                {
                                    agrupamento.CargaGerada.CargaFechada = false;
                                    agrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.Cancelada;
                                    repCarga.Atualizar(agrupamento.CargaGerada);
                                }

                                repStageAgrupamentoComposicao.ExecuteDeletarPorAgrupamento(agrupamento.Codigo);
                                repStageAgrupamento.Deletar(agrupamento);
                            }

                        }

                        listaStagesPedido.Remove(stageRemover);
                    }
                }

                listaStagesPedido = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

                List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage> stagesDT = new List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage>();

                //Regra: aqui vamos gerar a DT apenas com pedidos da stage 1
                if ((carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && carga.TipoOperacao?.TipoOperacaoRedespacho != null)
                    stagesDT = pedidoDocumentoTransporte.Stage.Where(obj => obj.NumeroStage.ToInt() == 1).ToList();
                else
                    stagesDT = pedidoDocumentoTransporte.Stage;

                bool possuiStage = false;

                foreach (var stages in stagesDT)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Stage stageSalva = stagesSalvas.Where(s => s.NumeroStage == stages.NumeroStage).FirstOrDefault();

                    if (stageSalva == null)
                        continue;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoStage pedidoStage = listaGeralStagesPedido.Where(x => x.Pedido.Codigo == pedido.Codigo && x.Stage.Codigo == stageSalva.Codigo).FirstOrDefault(); //repPedidoStage.BuscarPorStageEPedido(stageSalva.Codigo, pedido.Codigo);//buscar todas as stagesPedido pelos pedidos e comparar aqui

                    possuiStage = true;

                    if (pedidoStage == null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoStage stagePedido = new Dominio.Entidades.Embarcador.Pedidos.PedidoStage();

                        stagePedido.Stage = stageSalva;
                        stagePedido.Pedido = pedido;
                        repPedidoStage.Inserir(stagePedido);

                        listaStagesPedido.Add(stagePedido);
                    }
                    else
                        listaStagesPedido.Add(pedidoStage);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoExiste = cargaPedidosSalvos.Where(obj => obj.Pedido.Protocolo == protocoloPedido).FirstOrDefault(); //repCargaPedido.BuscarPorCargaEProtocoloPedido(carga.Codigo, protocoloPedido); 

                Dominio.Entidades.Embarcador.Pedidos.PedidoStage pedStage = null;
                if (listaStagesPedido.Count > 0)
                    pedStage = listaStagesPedido.FirstOrDefault();

                if (cargaPedidoExiste == null)
                {
                    cargaPedidoExiste = serCargaWS.CriarCargaPedidoPorDocumentoTransporte(carga, pedido, pedStage?.Stage, cargaPedidoXmlTemporario, configuracaoTMS, tipoServicoMultisoftware, auditado, unitOfWork);

                    if (cargaPedidoExiste == null)
                        throw new ServicoException($"Não foi possível vincular pedido protocolo {pedidoDocumentoTransporte.ProtocoloPedido} a carga.");

                    adicionouNovoPedido = true;
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto = repPedidoProduto.BuscarPorPedido(pedido.Codigo);
                    serCargaPedido.AdicionarProdutosCargaPedido(cargaPedidoExiste, listaPedidoProduto, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga, unitOfWork);
                }

                if (pedStage != null && cargaPedidoExiste.OrdemEntrega != pedStage.Stage?.OrdemEntrega)
                {
                    cargaPedidoExiste.OrdemEntrega = listaStagesPedido.FirstOrDefault().Stage.OrdemEntrega;

                    if (cargaPedidoExiste.OrdemEntrega > 0)
                        carga.OrdemRoteirizacaoDefinida = true;
                }

                //devemos atualizar o cargaPedido;
                if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.NaoConsolida)
                {
                    cargaPedidoExiste.Recebedor = pedStage?.Stage?.Recebedor;
                    cargaPedidoExiste.Expedidor = pedStage?.Stage?.Expedidor;

                    if (cargaPedidoExiste.Recebedor == null && pedido.Recebedor != null)
                    {
                        cargaPedidoExiste.Recebedor = pedido.Recebedor;

                        if ((cargaPedidoExiste.Carga.TipoOperacao?.UsarRecebedorComoPontoPartidaCarga ?? false) && cargaPedidoExiste.Recebedor != null)
                        {
                            cargaPedidoExiste.PontoPartida = cargaPedidoExiste.Recebedor;
                            cargaPedidoExiste.PossuiColetaEquipamentoPontoPartida = true;
                        }
                        else
                        {
                            cargaPedidoExiste.PontoPartida = pedido.PontoPartida;
                            cargaPedidoExiste.PossuiColetaEquipamentoPontoPartida = false;
                        }
                    }

                    if (cargaPedidoExiste.Expedidor == null && pedido.Expedidor != null)
                    {
                        cargaPedidoExiste.Expedidor = pedido.Expedidor;

                        if (cargaPedidoExiste.Carga.TipoOperacao != null && cargaPedidoExiste.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedidoExiste.Carga.Empresa == null)
                            cargaPedidoExiste.Carga.Empresa = repEmpresa.BuscarPorCNPJ(cargaPedidoExiste.Expedidor.CPF_CNPJ_SemFormato);
                    }

                    if (cargaPedidoExiste.Recebedor != null && cargaPedidoExiste.Expedidor == null)
                        cargaPedidoExiste.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;

                    if (cargaPedidoExiste.Recebedor == null && cargaPedidoExiste.Expedidor != null)
                        cargaPedidoExiste.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;

                    if (cargaPedidoExiste.Recebedor != null && cargaPedidoExiste.Expedidor != null)
                        cargaPedidoExiste.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;

                    repCargaPedido.Atualizar(cargaPedidoExiste);
                }

                if (possuiStage)
                {
                    pedido.PossuiStage = possuiStage;

                    if (carga?.TipoOperacao?.ConfiguracaoCarga?.ConsiderarKMRecibidoDoEmbarcador ?? false)
                        pedido.Distancia = pedStage?.Stage != null ? pedStage.Stage.Distancia : 0M;

                    repPedido.Atualizar(pedido);
                }

                cargaPedidoExiste.StageRelevanteCusto = Servicos.Embarcador.Pedido.Stage.ObterStageMaisRelevante(listaStagesPedido);
                cargaPedidoExiste.CanalEntrega = pedStage?.Stage?.CanalEntrega;
                cargaPedidoExiste.CanalVenda = pedStage?.Stage?.CanalVenda;

                if (serCargaWS.ValidarNotasPedidoEnviada(cargaPedidoExiste, unitOfWork))
                    cargaPedidoExiste.SituacaoEmissao = SituacaoNF.NFEnviada;

                repCargaPedido.Atualizar(cargaPedidoExiste);

                if (!cargasPedidos.Contains(cargaPedidoExiste))
                    cargasPedidos.Add(cargaPedidoExiste);

                if (!cargaPedidosSalvos.Contains(cargaPedidoExiste))
                    cargaPedidosSalvos.Add(cargaPedidoExiste);

                listaStagesPedidos.AddRange(listaStagesPedido);

                //if (cargaPedidoRecusaCTE.ExisteRecusaPorPedido(protocoloPedido))
                //{
                //    //Recebemos novamente o pedido antes removido por recusa de CTE 
                //    servicoRecusaCte.CriarCargaCtePedidoRecusa(protocoloPedido, cargaPedidoExiste, carga);
                //}
            }

            if (!carga.CargaEmitidaParcialmente)
            {
                carga.CargaGeradaViaDocumentoTransporte = true;
                carga.ExternalID1 = documentoTransporte.ExternalID1;
                carga.ExternalID2 = documentoTransporte.ExternalID2;

                if (documentoTransporte.DocumentoGlobalizado != carga.CteGlobalizado)
                    carga.CteGlobalizado = documentoTransporte.DocumentoGlobalizado;

                unitOfWork.Flush();

                if (!string.IsNullOrEmpty(documentoTransporte.DataPrevisaoInicioViagem))
                {
                    atualizouInicioViagemPrevisto = true;
                    carga.DataInicioViagemPrevista = documentoTransporte?.DataPrevisaoInicioViagem.ToNullableDateTime();
                    if (carga.DadosSumarizados != null)
                        carga.DadosSumarizados.DataPrevisaoInicioViagem = documentoTransporte?.DataPrevisaoInicioViagem.ToNullableDateTime();

                    Servicos.Log.TratarErro($"DATA INICIO VIAGEM CARGA {carga.CodigoCargaEmbarcador} - 1 - DATA: {documentoTransporte?.DataPrevisaoInicioViagem.ToNullableDateTime()} DADOS SUMARIZADOS {carga.DadosSumarizados != null}", "GerarCargaPorDocumentoTransporte");
                }

                //feito somente para ARCELOR
                if (configuracaoTMS.IncluirCargaCanceladaProcessarDT && string.IsNullOrEmpty(documentoTransporte.DataPrevisaoInicioViagem))
                {
                    Servicos.Log.TratarErro($"DATA PREVISTA INICIO VIAGEM NULL", "GerarCargaPorDocumentoTransporte");

                    if (!string.IsNullOrEmpty(documentoTransporte.DataTerminoCarregamento))
                    {
                        atualizouInicioViagemPrevisto = true;
                        carga.DataInicioViagemPrevista = documentoTransporte?.DataTerminoCarregamento.ToNullableDateTime();
                        Servicos.Log.TratarErro($"PEGOU DataTerminoCarregamento CARGA {carga.CodigoCargaEmbarcador} - 2 - DATA: {documentoTransporte?.DataTerminoCarregamento.ToNullableDateTime()}", "GerarCargaPorDocumentoTransporte");
                    }
                    else if (!string.IsNullOrEmpty(documentoTransporte.DataInicioCarregamento))
                    {
                        atualizouInicioViagemPrevisto = true;
                        carga.DataInicioViagemPrevista = documentoTransporte?.DataInicioCarregamento.ToNullableDateTime();
                        Servicos.Log.TratarErro($"PEGOU DataInicioCarregamento CARGA {carga.CodigoCargaEmbarcador} - 3 - DATA: {documentoTransporte?.DataInicioCarregamento.ToNullableDateTime()}", "GerarCargaPorDocumentoTransporte");
                    }
                }

                if (!string.IsNullOrEmpty(documentoTransporte?.TipoDT?.CodigoIntegracao) && documentoTransporte?.TipoDT?.CodigoIntegracao != carga?.TipoDocumentoTransporte?.CodigoIntegracao)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte tipodocumentoTransporte = repositorioTipocumentoTransporte.BuscarPorCodigoIntegracao(documentoTransporte?.TipoDT?.CodigoIntegracao ?? string.Empty);

                    if (tipodocumentoTransporte == null && !string.IsNullOrEmpty(documentoTransporte?.TipoDT?.CodigoIntegracao) && !string.IsNullOrEmpty(documentoTransporte?.TipoDT?.Descricao))
                    {
                        tipodocumentoTransporte = new Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte();
                        tipodocumentoTransporte.CodigoIntegracao = documentoTransporte?.TipoDT?.CodigoIntegracao;
                        tipodocumentoTransporte.Descricao = documentoTransporte?.TipoDT?.Descricao;
                        tipodocumentoTransporte.Status = true;

                        repositorioTipocumentoTransporte.Inserir(tipodocumentoTransporte);
                    }

                    carga.TipoDocumentoTransporte = tipodocumentoTransporte;
                }


                if (!string.IsNullOrEmpty(documentoTransporte.ControleIntegracaoEmbarcador) && carga?.ControleIntegracaoEmbarcador != documentoTransporte.ControleIntegracaoEmbarcador)
                    carga.ControleIntegracaoEmbarcador = documentoTransporte.ControleIntegracaoEmbarcador;

                if (!string.IsNullOrEmpty(documentoTransporte?.TipoOperacao?.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(documentoTransporte.TipoOperacao.CodigoIntegracao);

                    if (carga.TipoOperacao != null && carga.TipoOperacao.CodigoIntegracao != documentoTransporte.TipoOperacao.CodigoIntegracao)
                    {
                        if ((carga?.Empresa?.TransportadorFerroviario ?? false) && tipoOperacao != null && tipoOperacao?.ConfiguracaoTransportador?.TipoOperacaoModalFerroviario != null)
                            tipoOperacao = tipoOperacao.ConfiguracaoTransportador.TipoOperacaoModalFerroviario;
                    }
                    else if (carga.TipoOperacao == null)
                    {
                        if ((carga?.Empresa?.TransportadorFerroviario ?? false) && tipoOperacao != null && tipoOperacao?.ConfiguracaoTransportador?.TipoOperacaoModalFerroviario != null)
                            tipoOperacao = tipoOperacao.ConfiguracaoTransportador.TipoOperacaoModalFerroviario;
                    }

                    carga.TipoOperacao = tipoOperacao;
                }

                if (documentoTransporte.TipoCargaEmbarcador != null)
                {
                    if (carga.TipoDeCarga == null && !string.IsNullOrEmpty(documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao))
                        carga.TipoDeCarga = repositorioTipoCarga.BuscarPorCodigoEmbarcador(documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao);
                    else if (carga.TipoDeCarga != null && !string.IsNullOrEmpty(documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao) && documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao != carga.TipoDeCarga.CodigoTipoCargaEmbarcador)
                        carga.TipoDeCarga = repositorioTipoCarga.BuscarPorCodigoEmbarcador(documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao);
                }

                servicoIntegracaoUnilever.ProcessamentoDadosTransporteCarga(carga, documentoTransporte, tipoServicoMultisoftware, auditado, clienteAcesso, adminStringConexao);
            }

            List<int> codigoCargaPedidos = cargasPedidos.Select(x => x.Codigo).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoRemover = cargaPedidosOld.Where(obj => !codigoCargaPedidos.Contains(obj.Codigo)).ToList();
            int pedidosRemover = listaCargaPedidoRemover?.Count() ?? 0;

            if (pedidosRemover > 0)
            {
                foreach (var cargaPedido in listaCargaPedidoRemover)
                {
                    if (cargaPedido.Carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn && !cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECarga(cargaPedido.Pedido.Protocolo, cargaPedido.Carga.Codigo))
                    {
                        if (cargaPedido.CTesEmitidos)
                            throw new ServicoException($"Não foi possivel remover pedido ${cargaPedido.Pedido.NumeroPedidoEmbarcador} porque ja possui CT-e Emitido");

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedido = repPedidoStage.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoStage stageRemover in listaStagesPedido)
                        {
                            repPedidoStage.ExecuteDeletarPorStagePedido(stageRemover.Stage.Codigo, stageRemover.Pedido.Codigo);

                            if (!stagesSalvas.Where(j => j.Codigo == stageRemover.Stage.Codigo).Any())
                            {
                                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigo((stageRemover.Stage.StageAgrupamento?.Codigo ?? 0), false);

                                //caso esta tentado remover uma stage q faz parte deste cargaPedido, adicionamos na lista que vai validar se pode ser removida pois ela pode fazer parte de outro pedido
                                listaStagesRemover.Add((stageRemover.Stage, stageRemover.Pedido.Codigo));

                                //aqui remover vinculo da stage agrupada caso nao tem stages no agrupamento
                                if (agrupamento != null && repStage.ContarStagesPorAgrupamento(agrupamento.Codigo) == 0)
                                {
                                    repStageAgrupamentoComposicao.ExecuteDeletarPorAgrupamento(agrupamento.Codigo);
                                    repStageAgrupamento.Deletar(agrupamento);
                                }
                            }
                        }
                    }
                    else
                    {
                        // pedido de recusa de CTE.
                        if (cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECarga(cargaPedido.Pedido.Protocolo, cargaPedido.Carga.Codigo) && !listaCargaPedidoRecusa.Contains(cargaPedido))
                            listaCargaPedidoRecusa.Add(cargaPedido);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedido = repPedidoStage.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoStage stageCteRecusado in listaStagesPedido)
                        {
                            repPedidoStage.ExecuteDeletarPorStagePedido(stageCteRecusado.Stage.Codigo, stageCteRecusado.Pedido.Codigo);

                            if (!stagesSalvas.Where(j => j.Codigo == stageCteRecusado.Stage.Codigo).Any())
                            {

                                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigo((stageCteRecusado.Stage.StageAgrupamento?.Codigo ?? 0), false);

                                if (agrupamento != null)
                                    stageCteRecusado.Stage.StageAgrupamento = null;

                                if (stageCteRecusado.Stage.CargaDT != null && stageCteRecusado.Stage.CargaDT.Codigo == carga.Codigo)
                                {
                                    stageCteRecusado.Stage.CargaDT = null;
                                    repDocumentoProvisao.ExcluirPorStageECarga(stageCteRecusado.Stage.Codigo, carga.Codigo);
                                }

                                repStage.Atualizar(stageCteRecusado.Stage);

                                if (agrupamento != null && repStage.ContarStagesPorAgrupamento(agrupamento.Codigo) == 0)
                                {
                                    agrupamento.CargaDT = null;
                                    repStageAgrupamento.Atualizar(agrupamento);

                                    if (agrupamento.CargaGerada != null && !listaCargaPedidoRecusa.Contains(cargaPedido))
                                    {
                                        agrupamento.CargaGerada.CargaFechada = false;
                                        repCarga.Atualizar(agrupamento.CargaGerada);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRemover in listaCargaPedidoRemover)
            {
                if (listaCargaPedidoRecusa.Count() > 0 && listaCargaPedidoRecusa.Contains(cargaPedidoRemover))
                {
                    cargaPedidoRemover.StageRelevanteCusto = null;
                    repCargaPedido.Atualizar(cargaPedidoRemover);

                    //vamos remover o cargaPedido COM RECUSA (essa etapa esta recusada na carga DT e deve ser removida)
                    servicoRecusaCte.RemoverCteRecusaCargaPreChekin(cargaPedidoRemover, configuracaoGeralCarga);
                }
                else
                {
                    if (cargaPedidoRemover.CTesEmitidos)
                        throw new ServicoException($"Não foi possivel remover pedido ${cargaPedidoRemover.Pedido.NumeroPedidoEmbarcador} porque ja possui CT-e Emitido");

                    //Removendo antes porque esta gerando Deadlock
                    cargaPedidoRemover.StageRelevanteCusto = null;
                    repCargaPedido.Atualizar(cargaPedidoRemover);

                    //na arcelor podem ficar cargaPedidos de trechos anteriores de cargas ja "abandonadas" é necessario limpar antes de deletar o cargapedido
                    if (configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                        removerReferenciasPedidoTrecho(cargaPedidoRemover, unitOfWork);

                    Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedidoRemover, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, null, true, true);
                }
            }

            List<int> listaCodigosStageRemovidas = new List<int>();
            foreach (var (stage, codigoPedido) in listaStagesRemover)
            {
                if (!repPedidoStage.ExisteStageEmOutroPedido(stage.Codigo, codigoPedido))
                {
                    if (!listaCodigosStageRemovidas.Contains(stage.Codigo))
                    {
                        listaCodigosStageRemovidas.Add(stage.Codigo);
                        repCargaPedido.removerStageRelevanteCargaPedido(stage.Codigo);
                        repDocumentoProvisao.ExcluirPorStageECarga(stage.Codigo, carga.Codigo);//#74474
                        repCargaCancelamentoCargaIntegracao.removerStageCargaCancelamento(stage.Codigo);
                        repStage.Deletar(stage);
                    }
                }
                else
                {
                    //A stage nao pode ser removida pois pertence a outro pedido; entretanto se essa stage ja pertence a uma carga deve ser removida da carga e removida do agrupamento caso pertencer.
                    Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = stage.StageAgrupamento;

                    if (agrupamento != null)
                        stage.StageAgrupamento = null;

                    if (stage.CargaDT != null && stage.CargaDT.Codigo == carga.Codigo)
                    {
                        stage.CargaDT = null;
                        repDocumentoProvisao.ExcluirPorStageECarga(stage.Codigo, carga.Codigo);//#74474
                    }

                    repStage.Atualizar(stage);

                    repPedidoStage.ExecuteDeletarPorStagePedido(stage.Codigo, codigoPedido);

                    if (agrupamento != null && repStage.ContarStagesPorAgrupamento(agrupamento.Codigo) == 0)
                    {
                        if (agrupamento.CargaGerada != null)
                        {
                            agrupamento.CargaGerada.CargaFechada = false;
                            agrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.Cancelada;
                            repCarga.Atualizar(agrupamento.CargaGerada);
                        }

                        repStageAgrupamentoComposicao.ExecuteDeletarPorAgrupamento(agrupamento.Codigo);
                        repStageAgrupamento.Deletar(agrupamento);
                    }
                }
            }

            if ((carga.SituacaoCarga == SituacaoCarga.AgNFe) && !cargasPedidos.Any(obj => obj.SituacaoEmissao != SituacaoNF.NFEnviada))
            {
                carga.ProcessandoDocumentosFiscais = true;
                carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                carga.DataInicioEmissaoDocumentos = DateTime.Now;
            }

            if (documentoTransporte?.DataInicioCarregamento.ToNullableDateTime() != null && documentoTransporte?.DataInicioCarregamento.ToNullableDateTime() != carga.DataCarregamentoCarga)
                carga.DataCarregamentoCarga = documentoTransporte?.DataInicioCarregamento.ToNullableDateTime();

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoComparar = carga.TipoOperacao;

            carga.TipoOperacao = servicoCarga.ProcessarRegrasTipoOperacao(ref carga, carga.Pedidos.ToList(), unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = repConfiguracaoTrizy.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

            if (tipoOperacaoComparar != carga.TipoOperacao)
                removerTransportes = true;

            //validar se ja possui cargas geradas nos agrupamentos
            if (removerTransportes && carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                servCalculoFreteStages.RemoverStagesAgrupadasEcargasGeradas(carga);

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                foreach (var cargaPedido in cargasPedidos)
                {
                    if (cargaPedido.Recebedor != null)
                    {
                        cargaPedido.Recebedor = null;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }
            }


            if (carga.CargaEmitidaParcialmente)
            {
                Servicos.Log.TratarErro($"Pronto para AlterarDadosSumarizadosCarga (CargaEmitidaParcialmente), referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

                Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargasPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargasPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargasPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoTMS, tipoServicoMultisoftware);
                servicoRateioFrete.RatearValorFreteCargaEmitidaParcialmente(carga, cargasPedidos, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);
                servCalculoFreteStages.ProcessarFreteEGerarStagesAgrupadas(carga, listaStagesPedidos, cargasPedidos, true);
            }
            else
            {
                Servicos.Log.TratarErro($"Pronto para AlterarDadosSumarizadosCarga, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

                if ((adicionouNovoPedido && Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.CargaPossuiControleEntrega(carga, unitOfWork)) || (configuracaoTrizy?.NaoFinalizarPreTrip ?? false))
                {
                    Servicos.Log.TratarErro($"Pronto para AlterarDadosSumarizadosCarga (adicionando novo pedido), referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

                    //reroteirizar a carga para quando inserido novo pedido atualizar no controle entrega e monitoramento;
                    //PONTO DE LENTIDAO
                    Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargasPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                    servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargasPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                    servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargasPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                    Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoTMS, tipoServicoMultisoftware);
                }

                if (configuracaoTMS.IncluirCargaCanceladaProcessarDT && !carga.DataInicioViagem.HasValue && Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.CargaPossuiControleEntrega(carga, unitOfWork) && atualizouInicioViagemPrevisto)
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, configuracaoTMS, unitOfWork, DateTime.MinValue, tipoServicoMultisoftware);

                if (carga.TipoOperacao != null)
                {
                    carga.ExigeNotaFiscalParaCalcularFrete = carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete;
                    carga.NaoExigeVeiculoParaEmissao = carga.TipoOperacao.NaoExigeVeiculoParaEmissao;

                    if (carga.TipoOperacao.ConfiguracaoCarga?.ConsiderarKMRecibidoDoEmbarcador ?? false)
                        carga.Distancia = distanciaStages;
                }
                else
                {
                    carga.ExigeNotaFiscalParaCalcularFrete = configuracaoTMS.ExigirNotaFiscalParaCalcularFreteCarga;
                    carga.NaoExigeVeiculoParaEmissao = false;
                }

                if (adicionouNovoPedido)
                    carga.DataEnvioUltimaNFe = null;

                if ((documentoTransporte.Pedido == null || documentoTransporte.Pedido.Count <= 0) && (pedidosRemover > 0 || repCargaPedido.ContarPorCarga(carga.Codigo) <= 0))
                {
                    carga.CargaFechada = false;
                    carga.DataEnvioUltimaNFe = null;
                }
                else
                {
                    servicoCarga.FecharCarga(carga, unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: true, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: true);
                    if (carga.CargaAgrupamento == null)
                        carga.CargaFechada = true;


                    Servicos.Log.TratarErro($"Fechou Carga GerarCargaPorDocumentoTransporte, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");
                }

                if (carga.TipoOperacao != null && carga.DadosSumarizados != null)
                    carga.DadosSumarizados.TiposDeOperacao = carga.TipoOperacao.Descricao;

                carga.DataAgendamentoCarga = documentoTransporte?.DataAgendamento.ToNullableDateTime();
                carga.DataLoger = documentoTransporte?.DataPreparacaoPosCarregamento.ToNullableDateTime();
                carga.DataRealFaturamento = documentoTransporte?.DataRealFaturamento.ToNullableDateTime();
                carga.StatusLoger = documentoTransporte?.StatusLoger ?? "";
                carga.StagesGeradas = false;
                carga.Empresa = AtualizarTransportadorCarga(carga, documentoTransporte, unitOfWork, ref removerFreteEmbarcador);

                if (carga.Empresa != null && carga.IsChangedByPropertyName("Empresa"))
                    repCanhoto.SetarTransportadorCanhotos(carga.Codigo, carga.Empresa.Codigo);

                if (!string.IsNullOrEmpty(carga.StatusLoger) && carga.StatusLoger == "Datado")
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                servicoCarga.ConfirmarPendenciasTipoOperacao(carga, unitOfWork);

                if (!configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                    servCalculoFreteStages.ProcessarFreteEGerarStagesAgrupadas(carga, listaStagesPedidos, cargasPedidos, true);


                if ((carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && carga.TipoOperacao?.TipoOperacaoRedespacho != null)
                {
                    List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage> stagesDeRedespacho = new List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage>();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedidosRedespacho = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

                    foreach (var pedidoDocumentoTransporte in documentoTransporte.Pedido)
                    {
                        stagesDeRedespacho = pedidoDocumentoTransporte.Stage.Where(obj => obj.NumeroStage.ToInt() > 1).ToList();
                        foreach (var stages in stagesDeRedespacho)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Stage stageSalva = stagesSalvas.Where(s => s.NumeroStage == stages.NumeroStage).FirstOrDefault();

                            if (stageSalva == null)
                                continue;

                            Dominio.Entidades.Embarcador.Pedidos.PedidoStage pedidoStage = repPedidoStage.BuscarPorStageEPedido(stageSalva.Codigo, pedidoDocumentoTransporte.ProtocoloPedido.ToInt());

                            if (pedidoStage == null)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.PedidoStage stagePedido = new Dominio.Entidades.Embarcador.Pedidos.PedidoStage();

                                stagePedido.Stage = stageSalva;
                                stagePedido.Pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido { Codigo = pedidoDocumentoTransporte.ProtocoloPedido.ToInt() };
                                repPedidoStage.Inserir(stagePedido);

                                listaStagesPedidosRedespacho.Add(stagePedido);
                            }
                            else
                                listaStagesPedidosRedespacho.Add(pedidoStage);
                        }
                    }

                    servicoCarga.ValidarCargaRedespachoJaGeradaCarregamentoStage(carga, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);
                    servicoCarga.GerarCargaRedespachoOutrasEtapasCarregamentoStages(carga, listaStagesPedidosRedespacho.Distinct().ToList(), cargasPedidos, documentoTransporte.Pedido, tipoServicoMultisoftware, configuracaoTMS, carga.TipoOperacao.TipoOperacaoRedespacho, unitOfWork);
                }
            }

            if (carga.TipoOperacao != null)
                new Servicos.Embarcador.Carga.CargaPallets(unitOfWork, configuracaoTMS).RatearPaletesModeloVeicularCargaEntreOsPedidos(carga, cargasPedidos);

            bool controlePreCalculoFrete = adicionouNovoPedido || removerTransportes || pedidosRemover > 0;

            if (!carga.CargaEmitidaParcialmente && controlePreCalculoFrete)
                servicoIntegracaoUnilever.ReenviarIntegracoesPreCalculo(carga.Codigo);

            AtualizarDadosCargaPedidosFilhos(listaStagesPedidos, unitOfWork);

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && carga.DadosSumarizados != null)
                carga.DadosSumarizados.CargaTrecho = CargaTrechoSumarizada.Agrupadora;

            carga.DadosSumarizados.GlobalStatus = documentoTransporte?.InformacoesAdicionais?.GlobalStatus ?? string.Empty;

            if (modeloVeicularCargaOld != null && carga.ModeloVeicularCarga != null && modeloVeicularCargaOld.Codigo != carga.ModeloVeicularCarga.Codigo)
            {
                removerFreteEmbarcador = true;
                if (carga.VeiculosVinculados.Count() < carga.ModeloVeicularCarga.NumeroReboques && (carga.SituacaoCarga == SituacaoCarga.AgNFe || carga.SituacaoCarga == SituacaoCarga.CalculoFrete))
                    carga.SituacaoCarga = SituacaoCarga.Nova;
            }

            if (documentoTransporte.EventosDT != null)
            {
                DateTime? dataFimReal = documentoTransporte.EventosDT.Where(x => x.DataFimReal != null).FirstOrDefault()?.DataFimReal;

                if (dataFimReal.HasValue && dataFimReal.Value > DateTime.MinValue && configuracaoTMS.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem)
                {
                    carga.DataPreViagemFim = dataFimReal;
                    if (dataFimReal.HasValue)
                        repCarga.SetarDataPreViagemFimCargaTransbordo(carga.Codigo, dataFimReal.Value);
                    Auditoria.Auditoria.Auditar(auditado, carga, "Fim de Pré Trip - Confirmação de entrada na unidade via SAP ", unitOfWork);
                }
            }

            //#66766 Feito o avança automatico das carga
            if (!carga.CargaEmitidaParcialmente && repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever) && carga.TipoOperacao != null && carga.TipoOperacao.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn && (carga?.ModeloVeicularCarga?.Codigo ?? 0) > 0 && (carga?.TipoDeCarga?.Codigo ?? 0) > 0 && !carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete)
            {
                Servicos.Embarcador.Carga.Carga serviceCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.DadosCarga dadosCarga = ObterDadosCarga(carga, tipoServicoMultisoftware, auditado);
                serviceCarga.AvancarACargaSomenteComTipoDeCargaEModeloVeicular(dadosCarga, unitOfWork);
            }

            if (listaCargaPedidoRecusa.Count() > 0 && carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && !carga.CargaEmitidaParcialmente && carga.SituacaoCarga != SituacaoCarga.AgIntegracao)
            {
                carga.SituacaoCarga = SituacaoCarga.AgIntegracao;
                carga = servicoCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
            }

            if (removerFreteEmbarcador && carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador && carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn && carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.AutorizacaoEmissao)
                carga.TipoFreteEscolhido = TipoFreteEscolhido.Operador;

            repCarga.Atualizar(carga);
            repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);

            Servicos.Log.TratarErro($"Retornando Carga GerarCargaPorDocumentoTransporte, referente a carga código: {carga.Codigo}", "GerarCargaPorDocumentoTransporte_Rastreio");

            return carga;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Carga.DadosCarga ObterDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.DadosCarga
            {
                Carga = carga,
                CodigoTipoCarga = carga.TipoDeCarga.Codigo,
                CodigoModeloVeiculo = carga.ModeloVeicularCarga.Codigo,
                CodigoTipoContainer = carga?.TipoContainer?.Codigo ?? 0,
                Justificativa = "",
                CodigoTransportador = carga?.Empresa?.Codigo ?? 0,
                CodigoVeiculo = carga?.Veiculo?.Codigo ?? 0,
                CodigoReboque = (carga.VeiculosVinculados?.Count > 0) ? carga.VeiculosVinculados.ElementAt(0).Codigo : 0,
                CodigoSegundoReboque = (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(1).Codigo : 0,
                CodigoTerceiroReboque = (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(2).Codigo : 0,
                AvancoAutomatico = true,
                Usuario = carga?.Operador ?? null,
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                Auditado = auditado,
                CodigosMotoristas = new List<int>()
            };
        }

        private static void AtualizarDadosCargaPedidosFilhos(List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = listaStagesPedidos.Select(stagePedido => stagePedido.Stage).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Stage stage in stages)
            {
                if (stage.StageAgrupamento?.CargaGerada == null)
                    continue;

                List<int> codigosPedidos = listaStagesPedidos.Where(stagePedido => stagePedido.Stage.Codigo == stage.Codigo).Select(stagePedido => stagePedido.Pedido.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosFilhos = repositorioCargaPedido.BuscarPorCargaEPedidos(stage.StageAgrupamento.CargaGerada.Codigo, codigosPedidos);

                Dominio.Entidades.Embarcador.Pedidos.PedidoStage pedStage = listaStagesPedidos.Where(x => x.Stage.Codigo == stage.Codigo).FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listPedStage = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();
                listPedStage.Add(pedStage);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada = stage.StageAgrupamento?.CargaGerada;
                cargaGerada.ModeloVeicularCarga = stage.ModeloVeicularCarga;
                repositorioCarga.Atualizar(cargaGerada);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoFilho in cargaPedidosFilhos)
                {
                    cargaPedidoFilho.CanalEntrega = stage.CanalEntrega;
                    cargaPedidoFilho.CanalVenda = stage.CanalVenda;
                    cargaPedidoFilho.StageRelevanteCusto = Servicos.Embarcador.Pedido.Stage.ObterStageMaisRelevante(listPedStage);
                    repositorioCargaPedido.Atualizar(cargaPedidoFilho);
                }
            }
        }

        private static void removerReferenciasPedidoTrecho(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRemover, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoTrechoAnterior = repositorioCargaPedido.BuscarCargaPedidoTrechoAnteriorCargaDiferente(cargaPedidoRemover.Codigo, cargaPedidoRemover.Carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoTrechoAnterior)
            {
                cargaPedido.CargaPedidoTrechoAnterior = null;
                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoProximoTrecho = repositorioCargaPedido.BuscarCargaPedidoProximoTrechoCargaDiferente(cargaPedidoRemover.Codigo, cargaPedidoRemover.Carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoProximoTrecho)
            {
                cargaPedido.CargaPedidoProximoTrecho = null;
                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            if ((cargaPedidoRemover.Carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && cargaPedidoRemover.Carga.TipoOperacao?.TipoOperacaoRedespacho != null)
            {
                //essas cargas devem ser recriadas e os vinculos aqui sao desfeitos, para nao ter problemas no remover

                cargaPedidoRemover.CargaPedidoProximoTrecho = null;
                cargaPedidoRemover.CargaPedidoTrechoAnterior = null;
                repositorioCargaPedido.Atualizar(cargaPedidoRemover);
            }
        }

    }
}
