using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using NFe.Servicos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace Servicos.Embarcador.Integracao.Emillenium
{
    public class IntegracaoEmillenium
    {
        #region Atributos Globais

        string stringConexao;
        Repositorio.UnitOfWork unitOfWork;
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;

        #endregion

        #region Construtores

        public IntegracaoEmillenium(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            this.unitOfWork = unitOfWork;
            this.tipoServicoMultisoftware = tipoServicoMultisoftware;
            this.stringConexao = stringConexao;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Salva a lista de pedidos da Emillenium no banco na entidade PedidoAguardandoIntegracao. Depois, cada um deles deve ser integrado separadamente.
        /// </summary>
        public void SalvarListaPedidos()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            if (string.IsNullOrEmpty(configuracaoIntegracao.URLEmillenium) || string.IsNullOrEmpty(configuracaoIntegracao.UsuarioEmillenium) || string.IsNullOrEmpty(configuracaoIntegracao.SenhaEmillenium))
                return;

            string request = "";
            string response = "";
            DateTime dataPesquisa = configuracaoIntegracao.DataUltimaIntegracaoEmillenium ?? DateTime.Now.AddDays(-1);

            var listaPedidos = BuscarListaPedidos(configuracaoIntegracao, dataPesquisa, DateTime.Now, out request, out response);

            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

            // Salva os pedidos no banco para integração posterior
            foreach (var pedido in listaPedidos)
            {
                unitOfWork.Start();

                if (pedido.CodPedidov == null) continue;

                // Necessário procurar no banco para ver se já foi cadastrado, porque a API não dá uma maneira confiável de fazer isso
                var pedidoJaCadastrado = repPedidoAguardandoIntegracao.BuscarPorIdTipo(pedido.CodPedidov, TipoIntegracao.Emillenium, Dominio.Enumeradores.TipoIntegracaoEmillenium.Pedidos);
                if (pedidoJaCadastrado != null)
                {
                    continue;
                }

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracao.IdIntegracao = pedido.CodPedidov;
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.AgIntegracao;
                pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracao.DataCriacaoPedido = pedido.DataEmissao;
                pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.Emillenium;
                pedidoAguardandoIntegracao.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.Pedidos;
                pedidoAguardandoIntegracao.Informacao = "";
                pedidoAguardandoIntegracao.DataPesquisa = dataPesquisa;
                pedidoAguardandoIntegracao.DataEmbarquePedido = pedido.DataConclusaoEmbarque;
                pedidoAguardandoIntegracao.UltimaDataEmbarqueLista = (from o in listaPedidos select o.DataConclusaoEmbarque).Max();
                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                unitOfWork.CommitChanges();
            }

            unitOfWork.Start();

            if (listaPedidos.Count > 0)
            {
                // Salva a última data de criação dos pedidos nas configurações para ser usada posteriormente
                DateTime ultimaDataEmbarque = (from o in listaPedidos select o.DataConclusaoEmbarque).Max();
                configuracaoIntegracao.DataUltimaIntegracaoEmillenium = ultimaDataEmbarque.ToUniversalTime().AddSeconds(-1); // Adiciona um segundo para não pegar a mesma de novo
                repIntegracao.Atualizar(configuracaoIntegracao);
            }

            unitOfWork.CommitChanges();

        }

        /// <summary>
        /// Integra os pedidos, um por um.
        /// </summary>
        public void IntegrarPedidos()
        {
            // Para cada pedido a ser integrado, pega os dados completos dele.
            // Ver se já tem o pedido no banco (que veio da VTEX)

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            var listaPedidoAguardandoIntegracaoEmillenium = repPedidoAguardandoIntegracao.BuscarPorSituacoesETipoIntegracao(new List<SituacaoPedidoAguardandoIntegracao>() { SituacaoPedidoAguardandoIntegracao.AgIntegracao, SituacaoPedidoAguardandoIntegracao.ErroGenerico }, TipoIntegracao.Emillenium, 0, 3);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            //listaPedidoAguardandoIntegracaoEmillenium.Clear();
            //listaPedidoAguardandoIntegracaoEmillenium.Add(8173708);

            for (int i = 0; i < listaPedidoAguardandoIntegracaoEmillenium.Count; i++)
            {
                var pedidoAguardandoIntegracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(listaPedidoAguardandoIntegracaoEmillenium[i]);

                IntegrarPedidoAguardandoIntegracao(pedidoAguardandoIntegracao, configuracaoIntegracao);

                unitOfWork.FlushAndClear();
            }
        }

        public void GerarCargasPedidos()
        {
            // Para cada pedido a ser integrado, pega os dados completos dele.
            // Ver se já tem o pedido no banco (que veio da VTEX)

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

            var listaPedidoAguardandoIntegracaoEmillenium = repPedidoAguardandoIntegracao.BuscarPorSituacoesETipoIntegracao(new List<SituacaoPedidoAguardandoIntegracao>() { SituacaoPedidoAguardandoIntegracao.AgGerarCarga }, TipoIntegracao.Emillenium, 1, 2);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            if (listaPedidoAguardandoIntegracaoEmillenium != null && listaPedidoAguardandoIntegracaoEmillenium.Count > 0)
            {
                var pedidoAguardandoIntegracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(listaPedidoAguardandoIntegracaoEmillenium[0]);
                IntegrarPedidoAguardandoGerarCarga(pedidoAguardandoIntegracao.NumeroCarga, configuracaoIntegracao);
            }

            unitOfWork.FlushAndClear();
        }

        /// <summary>
        /// Integra as notas dos pedidos
        /// </summary>
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoConsultaNotas BuscarNotasPedidos(int transIdManualmente = 0)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            string request = "";
            string response = "";
            bool integrado = false;
            int ultimaTransId = 0;
            int NumeroNotasImportadas = 0;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoConsultaNotas retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoConsultaNotas();
            List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> ListpedidoAguardandoIntegracaoNota = new List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();

            Log.TratarErro("INICIO Busca Notas..", "Notas_Emillenium");

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            // devemos consultar 3 vezes na base da e-millenium; cancelada, cartacorrecao e normal
            for (int i = 0; i < 3; i++)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> dadosnotasPedido;
                if (transIdManualmente > 0)
                    dadosnotasPedido = BuscarNotasPedidoNaApiManualmente(configuracaoIntegracao, transIdManualmente, out request, out response);
                else
                    dadosnotasPedido = BuscarNotasPedidoNaApi(configuracaoIntegracao, configuracaoIntegracao.TransIdAtualEmillenium, out request, out response, i);

                if (dadosnotasPedido == null)
                    continue;

                Log.TratarErro("Request: " + request, "Notas_Emillenium");
                Log.TratarErro("Response: " + response, "Notas_Emillenium");

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido dadosNotaPedido in dadosnotasPedido)
                {
                    unitOfWork.Start();

                    if (dadosNotaPedido != null)
                    {
                        if (pedidoAguardandoIntegracaoNota.IdIntegracao != dadosNotaPedido.cod_pedidov || pedidoAguardandoIntegracaoNota.Codigo == 0)
                        {
                            integrado = false;
                            pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                            pedidoAguardandoIntegracaoNota.IdIntegracao = dadosNotaPedido.cod_pedidov;
                            pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                            pedidoAguardandoIntegracaoNota.CreatedAt = DateTime.Now;
                            pedidoAguardandoIntegracaoNota.DataCriacaoPedido = dadosNotaPedido.data_hora_emissao.HasValue ? dadosNotaPedido.data_hora_emissao.Value : DateTime.Now;
                            pedidoAguardandoIntegracaoNota.TipoIntegracao = TipoIntegracao.Emillenium;
                            pedidoAguardandoIntegracaoNota.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais;

                            repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracaoNota);
                        }

                        try
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade = null;
                            string numeroPedido = dadosNotaPedido.n_pedido_cliente;
                            bool notaDevolucao = !string.IsNullOrEmpty(dadosNotaPedido.tipo_operacao) ? dadosNotaPedido.tipo_operacao == "E" : false;

                            if (!string.IsNullOrWhiteSpace(numeroPedido))
                                pedidoEntidade = repPedido.BuscarPorNumeroOrdem(numeroPedido);
                            else
                                throw new ServicoException($"Campo n_pedido_cliente vazio");

                            if (numeroPedido.Contains("SLR-"))
                                numeroPedido = numeroPedido.Replace("SLR-", "").Trim();

                            if (pedidoEntidade == null)
                                pedidoEntidade = repPedido.BuscarPorNumeroPedidoEmbarcador(numeroPedido);

                            if (pedidoEntidade == null)
                                throw new ServicoException($"O pedido com número " + numeroPedido + " (TransID " + dadosNotaPedido.trans_id + " ) não está cadastrado no sistema");

                            if (pedidoEntidade != null)
                            {

                                if (string.IsNullOrWhiteSpace(pedidoEntidade.CodigoPedidoCliente) && !string.IsNullOrWhiteSpace(dadosNotaPedido.cod_pedidov))
                                    pedidoEntidade.CodigoPedidoCliente = dadosNotaPedido.cod_pedidov;

                                if (dadosNotaPedido.cancelado)
                                {
                                    //remover a nota do pedido.
                                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                                    if (xMLNotaFiscal == null)
                                        throw new ServicoException("Nota cancelada não encontrada");
                                    else
                                        pedidoEntidade.NotasFiscais.Remove(xMLNotaFiscal);

                                    repPedido.Atualizar(pedidoEntidade);
                                    pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Cancelado. Nota Fiscal " + dadosNotaPedido.chave_nf + " removida.";

                                    integrado = true;
                                }
                                else
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoDevolucao = null;

                                    if (notaDevolucao)
                                        pedidoDevolucao = CriarPedidoDevolucaoEmilleniumPorNota(pedidoEntidade, dadosNotaPedido, pedidoAguardandoIntegracaoNota, configuracao);

                                    string xmlNota = dadosNotaPedido.xml;

                                    if (!string.IsNullOrWhiteSpace(xmlNota) && dadosNotaPedido.status != "100")
                                        throw new ServicoException($"Nota fiscal " + dadosNotaPedido.chave_nf + " não importada, status " + dadosNotaPedido.status);

                                    if (!string.IsNullOrWhiteSpace(xmlNota))
                                    {
                                        Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                                        string path = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                                        if (xmlNotaFiscalExiste == null)
                                        {
                                            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(dadosNotaPedido.chave_nf, ".xml"));
                                            try
                                            {
                                                XmlDocument xdoc = new XmlDocument();
                                                xdoc.LoadXml(xmlNota);

                                                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                                                {
                                                    xdoc.Save(stream);
                                                    Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, stream);
                                                }
                                            }
                                            catch (ServicoException ex)
                                            {
                                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] ServicoException ignorada durante processamento Emillenium - segue processamento: {ex.ToString()}", "CatchNoAction");
                                            }

                                            System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                                            Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                                            dynamic nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork, false, false);

                                            if (serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, null, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                                            {
                                                repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                                                if (pedidoDevolucao != null)
                                                {
                                                    pedidoDevolucao.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                                                    pedidoDevolucao.NotasFiscais.Add(xmlNotaFiscal);
                                                    repPedido.Atualizar(pedidoDevolucao);
                                                }
                                                else
                                                {

                                                    pedidoEntidade.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                                                    pedidoEntidade.NotasFiscais.Add(xmlNotaFiscal);

                                                    repPedido.Atualizar(pedidoEntidade);
                                                }

                                                int codigoPedido = pedidoDevolucao != null ? pedidoDevolucao.Codigo : pedidoEntidade.Codigo;

                                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPorPedido(codigoPedido);
                                                if (cargaPedido != null)
                                                {
                                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);
                                                    if (pedidosExistente.Any(obj => obj.CargaPedido?.Carga?.Codigo == cargaPedido.Carga.Codigo))
                                                        throw new ServicoException($" A nota fiscal " + xmlNotaFiscal.Chave + " já foi enviada para outro pedido nesta mesma carga");

                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                                                    pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                                                    pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                                                    pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                                                    repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                                                    string retornoFinalizacaa = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, xmlNotaFiscal.Peso, xmlNotaFiscal.Volumes, null, null, null, null, null, configuracao, tipoServicoMultisoftware, null, null, unitOfWork); ;
                                                    if (!string.IsNullOrWhiteSpace(retornoFinalizacaa))
                                                        throw new ServicoException(retornoFinalizacaa);
                                                }

                                                NumeroNotasImportadas += 1;
                                                integrado = true;

                                                if (pedidoDevolucao == null)
                                                    pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Nota Fiscal " + dadosNotaPedido.chave_nf + " adicionada.";
                                            }
                                            else
                                            {
                                                throw new ServicoException("Problemas ao adicionar nota fiscal " + xmlNotaFiscal.Chave + " : " + erro);
                                            }
                                        }
                                        else
                                            pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Nota Fiscal " + dadosNotaPedido.chave_nf + " ja adicionada.";
                                    }

                                    if (dadosNotaPedido.carta_correcao != null && dadosNotaPedido.carta_correcao.Count > 0 && !string.IsNullOrWhiteSpace(dadosNotaPedido.cnpj_transportadora))
                                    {
                                        //verificar se ouve troca de transportador..
                                        var transportador = repEmpresa.BuscarPorCNPJ(dadosNotaPedido.cnpj_transportadora.ObterSomenteNumeros());
                                        if (transportador != null && pedidoEntidade.Empresa != transportador)
                                        {
                                            pedidoEntidade.Empresa = transportador;
                                            pedidoAguardandoIntegracaoNota.Informacao = " Pedido " + dadosNotaPedido.n_pedido_cliente + " possuí carta correção; troca de transportador " + dadosNotaPedido.cnpj_transportadora;
                                            integrado = true;
                                        }

                                        repPedido.Atualizar(pedidoEntidade);
                                    }

                                }
                            }
                        }
                        catch (ServicoException e)
                        {
                            pedidoAguardandoIntegracaoNota.Informacao = "TransID " + dadosNotaPedido?.trans_id + " - " + e.Message;
                            pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                            repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                            integrado = true;
                        }
                        catch (Exception e)
                        {
                            pedidoAguardandoIntegracaoNota.Informacao = "Erro genérico";
                            pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                            repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                            integrado = true;

                            Log.TratarErro($"Erro generico Consulta notas Emillenium pedido: {dadosNotaPedido.n_pedido_cliente} , {e.Message}");
                            Servicos.Log.TratarErro(e);
                        }

                    }

                    unitOfWork.Flush();

                    if (!integrado)
                    {
                        // nao teve nenhuma alteracao, xml da nota ja adicionado sem cancelamento ou nao teve alteracao de carta correcao.
                        if (transIdManualmente == 0)
                        {
                            repPedidoAguardandoIntegracao.deletarPorCodigoIntegracao(pedidoAguardandoIntegracaoNota.Codigo);
                            //reinicia a variavel pois estava tentando dar update em pedidos deletados quando eram devolução
                            pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                        }
                    }
                    else
                    {
                        ListpedidoAguardandoIntegracaoNota.Add(pedidoAguardandoIntegracaoNota);

                        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                        AdicionarArquivoTransacao(pedidoAguardandoIntegracaoNota, arquivoRequisicao, arquivoResposta, unitOfWork);
                        repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                    }

                    unitOfWork.CommitChanges();
                }

                if (i == 0)
                    ultimaTransId = (from o in dadosnotasPedido select o.trans_id).Max();

                if (transIdManualmente > 0)
                    break;
            }

            retorno.ListapedidoAguardandoIntegracaoNotaretorno = ListpedidoAguardandoIntegracaoNota;
            retorno.ultimaTransId = ultimaTransId;

            Log.TratarErro($"FIM Busca Notas. ULTIMO TRANS_ID: {ultimaTransId}", "Notas_Emillenium");
            Log.TratarErro($"NOTAS IMPORTADAS: {NumeroNotasImportadas}", "Notas_Emillenium");

            if (transIdManualmente == 0 && ultimaTransId > 0)
            {
                unitOfWork.Start();

                configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();
                configuracaoIntegracao.TransIdAtualEmillenium = ultimaTransId;
                repIntegracao.Atualizar(configuracaoIntegracao);

                unitOfWork.CommitChanges();
            }

            return retorno;

        }

        public HttpRequisicaoResposta ConfirmarEntrega(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal)
        {
            var httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                sucesso = false,
                extensaoRequisicao = "json",
                extensaoResposta = "json"
            };

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEmilleniumConfirmarEntrega) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioEmillenium) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaEmillenium))
            {
                httpRequisicaoResposta.mensagem = "A integração não está configurada.";
                return httpRequisicaoResposta;
            }

            string uri = $"{configuracaoIntegracao.URLEmilleniumConfirmarEntrega}?CHAVE_NFE={notaFiscal.Chave}";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEmillenium));
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Add("Dkt_api", $"{configuracaoIntegracao?.SenhaFrontDoor ?? string.Empty}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{configuracaoIntegracao.UsuarioEmillenium}:{configuracaoIntegracao.SenhaEmillenium}")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpResponseMessage respostaServer = client.GetAsync(uri).Result;

            httpRequisicaoResposta.httpStatusCode = respostaServer.StatusCode;
            httpRequisicaoResposta.conteudoResposta = respostaServer.Content.ReadAsStringAsync().Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                httpRequisicaoResposta.mensagem = respostaServer.Content.ReadAsStringAsync().Result;
                httpRequisicaoResposta.sucesso = true;
            }
            else
                httpRequisicaoResposta.mensagem = "Retorno integração E-Millenium: " + respostaServer.StatusCode.ToString();

            return httpRequisicaoResposta;
        }

        public void NotificarFilaIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.EmailsNotificacaoEmillenium) && configuracaoIntegracao.QuantidadeNotificacaoEmillenium > 0)
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

                DateTime dataReferencia = DateTime.Now.AddMinutes(-25);
                var quantidadePendentes = repPedidoAguardandoIntegracao.ContarPorTipoESituacao(TipoIntegracao.Emillenium, SituacaoPedidoAguardandoIntegracao.AgIntegracao, dataReferencia);

                if (quantidadePendentes > configuracaoIntegracao.QuantidadeNotificacaoEmillenium)
                {
                    Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

                    string assunto = "Integração Emillenium pendentes.";
                    string ambiente = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                    if (!string.IsNullOrWhiteSpace(ambiente))
                        assunto = assunto + " - " + ambiente + ".";

                    StringBuilder stBuilder = new StringBuilder();
                    stBuilder
                        .Append("Atenção, existem " + quantidadePendentes + " integrações da Emillenium pendentes.")
                        .AppendLine() // Quebra de linha
                        .AppendLine();

                    stBuilder.Append("<br /> <br />");
                    stBuilder.Append("Favor não responder! E-mail enviado automaticamente para: " + configuracaoIntegracao.EmailsNotificacaoEmillenium).Append("<br /> <br />");

                    List<string> listaEmails = new List<string>();
                    listaEmails = configuracaoIntegracao.EmailsNotificacaoEmillenium.Split(';').ToList();

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

                    for (var j = 0; j < listaEmails.Distinct().Count(); j++)
                        svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, listaEmails[j], "", "", assunto, stBuilder.ToString(), string.Empty, null, ss.ToString(), true, listaEmails[j], 0, unitOfWork);
                }
            }
        }

        /// <summary>
        /// Integra as notas dos pedidos
        /// </summary>
        public void BuscaNotasMassiva()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium repIntegracaoEmile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium configuracaoIntegracao = repIntegracaoEmile.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configintegracao = repIntegracao.BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            string request = "";
            string response = "";
            bool integrado = false;
            int ultimoTransIdBuscaMassiva = 0;
            int numeroNotasImportadas = 0;

            List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> ListpedidoAguardandoIntegracaoNota = new List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();

            Log.TratarErro("INICIO Emillenium MASSIVO..", "Notas_Emillenium_massivo");

            while (ultimoTransIdBuscaMassiva < configuracaoIntegracao.TransIdFimBuscaMassiva && configuracaoIntegracao.TransIdInicioBuscaMassiva > 0)
            {
                // devemos consultar 3 vezes na base da e-millenium; cancelada, cartacorrecao e normal
                for (int i = 0; i < 3; i++)
                {

                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> dadosnotasPedido;
                    dadosnotasPedido = BuscarNotasPedidoNaApi(configintegracao, configuracaoIntegracao.TransIdInicioBuscaMassiva, out request, out response, i, 150);

                    if (dadosnotasPedido == null)
                        continue;

                    Log.TratarErro("Request: " + request, "Notas_Emillenium_massivo");
                    Log.TratarErro("Response: " + response, "Notas_Emillenium_massivo");

                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido dadosNotaPedido in dadosnotasPedido)
                    {
                        if (dadosNotaPedido != null)
                        {

                            if (pedidoAguardandoIntegracaoNota.IdIntegracao != dadosNotaPedido.cod_pedidov || pedidoAguardandoIntegracaoNota.Codigo == 0)
                            {
                                integrado = false;
                                pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                                pedidoAguardandoIntegracaoNota.IdIntegracao = dadosNotaPedido.cod_pedidov;
                                pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                                pedidoAguardandoIntegracaoNota.CreatedAt = DateTime.Now;
                                pedidoAguardandoIntegracaoNota.DataCriacaoPedido = dadosNotaPedido.data_hora_emissao.HasValue ? dadosNotaPedido.data_hora_emissao.Value : DateTime.Now;
                                pedidoAguardandoIntegracaoNota.TipoIntegracao = TipoIntegracao.Emillenium;
                                pedidoAguardandoIntegracaoNota.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais;
                                pedidoAguardandoIntegracaoNota.Informacao = "";
                                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracaoNota);
                            }

                            try
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade = null;

                                if (!string.IsNullOrWhiteSpace(dadosNotaPedido.n_pedido_cliente))
                                    pedidoEntidade = repPedido.BuscarPorNumeroOrdem(dadosNotaPedido.n_pedido_cliente);
                                else
                                    throw new ServicoException($"Campo n_pedido_cliente vazio");

                                if (pedidoEntidade == null)
                                    pedidoEntidade = repPedido.BuscarPorNumeroPedidoEmbarcador(dadosNotaPedido.n_pedido_cliente);

                                if (pedidoEntidade == null)
                                    throw new ServicoException($"O pedido com número " + dadosNotaPedido.n_pedido_cliente + " (TransID " + dadosNotaPedido.trans_id + " ) não está cadastrado no sistema");

                                if (pedidoEntidade != null)
                                {
                                    unitOfWork.Start();

                                    if (dadosNotaPedido.cancelado)
                                    {
                                        //remover a nota do pedido.
                                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                                        if (xMLNotaFiscal == null)
                                            throw new ServicoException("Nota cancelada não encontrada");
                                        else
                                            pedidoEntidade.NotasFiscais.Remove(xMLNotaFiscal);

                                        repPedido.Atualizar(pedidoEntidade);
                                        pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Cancelado. Nota Fiscal " + dadosNotaPedido.chave_nf + " removida.";

                                        integrado = true;
                                    }
                                    else
                                    {
                                        string xmlNota = dadosNotaPedido.xml;

                                        if (!string.IsNullOrWhiteSpace(xmlNota) && dadosNotaPedido.status != "100")
                                            throw new ServicoException($"Nota fiscal " + dadosNotaPedido.chave_nf + " não importada, status " + dadosNotaPedido.status);

                                        if (!string.IsNullOrWhiteSpace(xmlNota))
                                        {
                                            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                                            string path = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                                            if (xmlNotaFiscalExiste == null)
                                            {
                                                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(dadosNotaPedido.chave_nf, ".xml"));
                                                XmlDocument xdoc = new XmlDocument();
                                                xdoc.LoadXml(xmlNota);

                                                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                                                {
                                                    xdoc.Save(stream);
                                                    Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, stream);
                                                }

                                                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                                                Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                                                dynamic nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork, false, false);

                                                if (serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, null, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                                                {
                                                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                                                    pedidoEntidade.NotasFiscais.Add(xmlNotaFiscal);
                                                    repPedido.Atualizar(pedidoEntidade);

                                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPorPedido(pedidoEntidade.Codigo);
                                                    if (cargaPedido != null)
                                                    {
                                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);
                                                        if (pedidosExistente.Any(obj => obj.CargaPedido.Carga.Codigo == cargaPedido.Carga.Codigo))
                                                            throw new ServicoException($" A nota fiscal " + xmlNotaFiscal.Chave + " já foi enviada para outro pedido nesta mesma carga");

                                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                                                        pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                                                        pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                                                        pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                                                        repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                                                        string retornoFinalizacaa = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, xmlNotaFiscal.Peso, xmlNotaFiscal.Volumes, null, null, null, null, null, configuracao, tipoServicoMultisoftware, null, null, unitOfWork); ;
                                                        if (!string.IsNullOrWhiteSpace(retornoFinalizacaa))
                                                            throw new ServicoException(retornoFinalizacaa);
                                                    }

                                                    integrado = true;
                                                    numeroNotasImportadas += 1;
                                                    pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Nota Fiscal " + dadosNotaPedido.chave_nf + " adicionada.";
                                                }
                                                else
                                                {
                                                    throw new ServicoException("Problemas ao adicionar nota fiscal " + xmlNotaFiscal.Chave + " : " + erro);
                                                }
                                            }
                                            else
                                                pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Nota Fiscal " + dadosNotaPedido.chave_nf + " ja adicionada.";
                                        }

                                        if (dadosNotaPedido.carta_correcao != null && dadosNotaPedido.carta_correcao.Count > 0)
                                        {
                                            //verificar se ouve troca de transportador..
                                            var transportador = repEmpresa.BuscarPorCNPJ(dadosNotaPedido.cnpj_transportadora.ObterSomenteNumeros());
                                            if (pedidoEntidade.Empresa != transportador)
                                            {
                                                pedidoEntidade.Empresa = transportador;
                                                pedidoAguardandoIntegracaoNota.Informacao = " Pedido " + dadosNotaPedido.n_pedido_cliente + " possuí carta correção; troca de transportador " + dadosNotaPedido.cnpj_transportadora;
                                                integrado = true;
                                            }

                                            repPedido.Atualizar(pedidoEntidade);
                                        }
                                    }

                                    unitOfWork.CommitChanges();
                                }

                            }
                            catch (ServicoException e)
                            {
                                pedidoAguardandoIntegracaoNota.Informacao = "TransID " + dadosNotaPedido?.trans_id + " - " + e.Message;
                                pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                                integrado = true;
                            }
                            catch (Exception e)
                            {
                                pedidoAguardandoIntegracaoNota.Informacao = "Erro genérico";
                                pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                                integrado = true;
                            }
                        }

                        ListpedidoAguardandoIntegracaoNota.Add(pedidoAguardandoIntegracaoNota);

                        if (!integrado)
                        {
                            // nao teve nenhuma alteracao, xml da nota ja adicionado sem cancelamento ou nao teve alteracao de carta correcao.
                            repPedidoAguardandoIntegracao.deletarPorCodigoIntegracao(pedidoAguardandoIntegracaoNota.Codigo);
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                            AdicionarArquivoTransacao(pedidoAguardandoIntegracaoNota, arquivoRequisicao, arquivoResposta, unitOfWork);
                            repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                        }
                    }

                    if (i == 0)
                        ultimoTransIdBuscaMassiva = (from o in dadosnotasPedido select o.trans_id).Max();

                }

                Log.TratarErro($"FIM Emillenium MASSIVO. ULTIMO TRANS_ID: {ultimoTransIdBuscaMassiva}", "Notas_Emillenium_massivo");
                Log.TratarErro($"NOTAS IMPORTADAS: {numeroNotasImportadas}", "Notas_Emillenium_massivo");

                configuracaoIntegracao.TransIdInicioBuscaMassiva = ultimoTransIdBuscaMassiva;
            }

            if (ultimoTransIdBuscaMassiva >= configuracaoIntegracao.TransIdFimBuscaMassiva)
            {
                //deve parar de buscar.
                unitOfWork.Start();

                configuracaoIntegracao.TransIdInicioBuscaMassiva = 0;
                configuracaoIntegracao.DataFinalizacaoBuscaMassiva = DateTime.Now;
                Log.TratarErro($"FIM ULTIMO TRANS_ID: {ultimoTransIdBuscaMassiva}", "Notas_Emillenium_massivo");

                repIntegracaoEmile.Atualizar(configuracaoIntegracao);

                unitOfWork.CommitChanges();
            }

        }

        /// <summary>
        /// Integra as notas dos pedidos
        /// </summary>
        public void BuscarNotasPedidosAgNFe()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            string request = "";
            string response = "";

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAguardandoNotas = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAguardandoNotas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            //cargasAguardandoNotas.Add(repCarga.BuscarPorCodigo(1017903, false));
            cargasAguardandoNotas = repCarga.BuscarCargaAgNotaPorDataCriacao(DateTime.Now.AddDays(-1));

            Log.TratarErro($"INICIO buscar cargas sem Nota: {cargasAguardandoNotas.Count}", "Notas_Emillenium_Pedidos_Aguardando");

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasAguardandoNotas)
            {
                pedidosAguardandoNotas = repPedido.BuscarPedidosPorCaraSemNotasFiscais(carga.Codigo);

                Log.TratarErro($"Carga: {carga.CodigoCargaEmbarcador}", "Notas_Emillenium_Pedidos_Aguardando");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAguardandoNotas)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> dadosnotasPedido;

                    if (pedido.NotasFiscais != null && pedido.NotasFiscais.Count > 0)
                        continue;

                    string mensagem = $"Pedido: {(!string.IsNullOrEmpty(pedido.CodigoPedidoCliente) ? pedido.CodigoPedidoCliente : pedido.NumeroPedidoEmbarcador)}";
                    Log.TratarErro(mensagem, "Notas_Emillenium_Pedidos_Aguardando");

                    if (!string.IsNullOrEmpty(pedido.CodigoPedidoCliente))
                        dadosnotasPedido = BuscarNotasPorPedidoNaApi(configuracaoIntegracao, pedido.CodigoPedidoCliente, out request, out response);
                    else
                        dadosnotasPedido = BuscarNotasPorPedidoNaApi(configuracaoIntegracao, pedido.NumeroPedidoEmbarcador, out request, out response);

                    if (dadosnotasPedido == null)
                    {
                        Log.TratarErro("Sem retorno Emillenium.", "Notas_Emillenium_Pedidos_Aguardando");
                        continue;
                    }

                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido dadosNotaPedido in dadosnotasPedido)
                        ProcessarDadosNotaPedido(dadosNotaPedido, request, response);


                }
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento BuscarNotasPorPedidosPendentes(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento() { status = true, mensagem = "Sucesso no processaento" };

            string request = "";
            string response = "";

            if (pedidos == null || pedidos.Count == 0)
            {
                retorno.status = false;
                retorno.mensagem = "Não foram encontrados pedidos pendentes";
                return retorno;
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAguardandoNotas = pedidos;

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.BuscarPrimeiroRegistro();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento> listaProcessados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAguardandoNotas)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> dadosnotasPedido;

                if (pedido.NotasFiscais != null && pedido.NotasFiscais.Count > 0)
                {
                    listaProcessados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento() { status = true, mensagem = $"Pedido: {pedido.NumeroPedidoEmbarcador} - já contém notas fiscais" });
                    continue;
                }

                string mensagem = $"Pedido: {(!string.IsNullOrEmpty(pedido.CodigoPedidoCliente) ? pedido.CodigoPedidoCliente : pedido.NumeroPedidoEmbarcador)}";
                Log.TratarErro(mensagem, "Notas_Emillenium_Pedidos_Manual");

                if (!string.IsNullOrEmpty(pedido.CodigoPedidoCliente))
                    dadosnotasPedido = BuscarNotasPorPedidoNaApi(configuracaoIntegracao, pedido.CodigoPedidoCliente, out request, out response);
                else
                    dadosnotasPedido = BuscarNotasPorPedidoNaApi(configuracaoIntegracao, pedido.NumeroPedidoEmbarcador, out request, out response);

                if (dadosnotasPedido == null)
                {
                    Log.TratarErro("Sem retorno Emillenium.", "Notas_Emillenium_Pedidos_Manual");
                    listaProcessados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento() { status = false, mensagem = $"Pedido: {pedido.NumeroPedidoEmbarcador} - Sem retorno Emillenium" });
                    continue;
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido dadosNotaPedido in dadosnotasPedido)
                    ProcessarDadosNotaPedido(dadosNotaPedido, request, response);
                listaProcessados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoProcessamento() { status = true, mensagem = $"Pedido: {pedido.NumeroPedidoEmbarcador} - Processado com sucesso" });

            }
            retorno = (listaProcessados.Count > 0 && listaProcessados.Any(obj => obj.status == false)) ? listaProcessados.Where(obj => obj.status == false).FirstOrDefault() : retorno;
            return retorno;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void ProcessarDadosNotaPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido dadosNotaPedido, string request, string response)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            if (dadosNotaPedido != null)
            {
                pedidoAguardandoIntegracaoNota = repPedidoAguardandoIntegracao.BuscarPorIdTipo(dadosNotaPedido.cod_pedidov, TipoIntegracao.Emillenium, Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais);

                if (pedidoAguardandoIntegracaoNota == null || pedidoAguardandoIntegracaoNota.Codigo <= 0)
                {
                    pedidoAguardandoIntegracaoNota = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                    pedidoAguardandoIntegracaoNota.IdIntegracao = dadosNotaPedido.cod_pedidov;
                    pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                    pedidoAguardandoIntegracaoNota.CreatedAt = DateTime.Now;
                    pedidoAguardandoIntegracaoNota.DataCriacaoPedido = dadosNotaPedido.data_hora_emissao.HasValue ? dadosNotaPedido.data_hora_emissao.Value : DateTime.Now;
                    pedidoAguardandoIntegracaoNota.TipoIntegracao = TipoIntegracao.Emillenium;
                    pedidoAguardandoIntegracaoNota.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais;
                    pedidoAguardandoIntegracaoNota.Informacao = "";
                    repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracaoNota);
                }

                try
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade = null;
                    string numeroPedido = dadosNotaPedido.n_pedido_cliente;

                    if (!string.IsNullOrWhiteSpace(numeroPedido))
                        pedidoEntidade = repPedido.BuscarPorNumeroOrdem(numeroPedido);
                    else
                        throw new ServicoException($"Campo n_pedido_cliente vazio");

                    if (numeroPedido.Contains("SLR-"))
                        numeroPedido = numeroPedido.Replace("SLR-", "").Trim();

                    if (pedidoEntidade == null)
                        pedidoEntidade = repPedido.BuscarPorNumeroPedidoEmbarcador(numeroPedido);

                    if (pedidoEntidade == null)
                        throw new ServicoException($"O pedido com número " + numeroPedido + " (TransID " + dadosNotaPedido.trans_id + " ) não está cadastrado no sistema");

                    if (pedidoEntidade != null)
                    {
                        unitOfWork.Start();

                        if (string.IsNullOrWhiteSpace(pedidoEntidade.CodigoPedidoCliente) && !string.IsNullOrWhiteSpace(dadosNotaPedido.cod_pedidov))
                            pedidoEntidade.CodigoPedidoCliente = dadosNotaPedido.cod_pedidov;

                        if (dadosNotaPedido.cancelado)
                        {
                            //remover a nota do pedido.
                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                            if (xMLNotaFiscal == null)
                                throw new ServicoException("Nota cancelada não encontrada");
                            else
                                pedidoEntidade.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                            repPedido.Atualizar(pedidoEntidade);
                            pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Cancelado. Nota Fiscal " + dadosNotaPedido.chave_nf + " removida.";

                        }
                        else
                        {
                            string xmlNota = dadosNotaPedido.xml;

                            if (!string.IsNullOrWhiteSpace(xmlNota) && dadosNotaPedido.status != "100")
                                throw new ServicoException($"Nota fiscal " + dadosNotaPedido.chave_nf + " não importada, status " + dadosNotaPedido.status);

                            if (!string.IsNullOrWhiteSpace(xmlNota))
                            {
                                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                                string path = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                                if (xmlNotaFiscalExiste != null)
                                {
                                    pedidoEntidade.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                                    pedidoEntidade.NotasFiscais.Add(xmlNotaFiscalExiste);
                                    repPedido.Atualizar(pedidoEntidade);

                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPorPedido(pedidoEntidade.Codigo);
                                    if (cargaPedido != null)
                                    {
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscalExiste.Codigo);
                                        if (pedidosExistente.Any(obj => obj.CargaPedido.Carga.Codigo == cargaPedido.Carga.Codigo))
                                            throw new ServicoException($" A nota fiscal " + xmlNotaFiscalExiste.Chave + " já foi enviada para outro pedido nesta mesma carga");

                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                                        pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                                        pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscalExiste;
                                        pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                                        repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                                        Log.TratarErro($"XML: {dadosNotaPedido.chave_nf} vinculado ao pedido {pedidoEntidade.NumeroPedidoEmbarcador}", "Notas_Emillenium_Pedidos_Aguardando");

                                        string retornoFinalizacaa = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, xmlNotaFiscalExiste.Peso, xmlNotaFiscalExiste.Volumes, null, null, null, null, null, configuracao, tipoServicoMultisoftware, null, null, unitOfWork); ;
                                        if (!string.IsNullOrWhiteSpace(retornoFinalizacaa))
                                            throw new ServicoException(retornoFinalizacaa);
                                    }

                                    pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Nota Fiscal " + dadosNotaPedido.chave_nf + " adicionada.";
                                }
                                else
                                {

                                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(dadosNotaPedido.chave_nf, ".xml"));
                                    XmlDocument xdoc = new XmlDocument();
                                    xdoc.LoadXml(xmlNota);

                                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                                    {
                                        xdoc.Save(stream);
                                        Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, stream);
                                    }

                                    System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                                    Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                                    dynamic nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork, false, false);

                                    if (serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, null, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                                    {
                                        repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                                        pedidoEntidade.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                                        pedidoEntidade.NotasFiscais.Add(xmlNotaFiscal);

                                        repPedido.Atualizar(pedidoEntidade);

                                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPorPedido(pedidoEntidade.Codigo);
                                        if (cargaPedido != null)
                                        {
                                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);
                                            if (pedidosExistente.Any(obj => obj.CargaPedido.Carga.Codigo == cargaPedido.Carga.Codigo))
                                                throw new ServicoException($" A nota fiscal " + xmlNotaFiscal.Chave + " já foi enviada para outro pedido nesta mesma carga");

                                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                                            pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                                            pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                                            pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                                            repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                                            Log.TratarErro($"XML: {dadosNotaPedido.chave_nf} vinculado ao pedido {pedidoEntidade.NumeroPedidoEmbarcador}", "Notas_Emillenium_Pedidos_Aguardando");

                                            string retornoFinalizacaa = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, xmlNotaFiscal.Peso, xmlNotaFiscal.Volumes, null, null, null, null, null, configuracao, tipoServicoMultisoftware, null, null, unitOfWork); ;
                                            if (!string.IsNullOrWhiteSpace(retornoFinalizacaa))
                                                throw new ServicoException(retornoFinalizacaa);
                                        }

                                        pedidoAguardandoIntegracaoNota.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " Nota Fiscal " + dadosNotaPedido.chave_nf + " adicionada.";
                                    }
                                    else
                                        throw new ServicoException("Problemas ao adicionar nota fiscal " + xmlNotaFiscal.Chave + " : " + erro);
                                }
                            }

                            if (dadosNotaPedido.carta_correcao != null && dadosNotaPedido.carta_correcao.Count > 0)
                            {
                                //verificar se ouve troca de transportador..
                                var transportador = repEmpresa.BuscarPorCNPJ(dadosNotaPedido.cnpj_transportadora.ObterSomenteNumeros());
                                if (transportador != null && pedidoEntidade.Empresa != transportador)
                                {
                                    pedidoEntidade.Empresa = transportador;
                                    pedidoAguardandoIntegracaoNota.Informacao = " Pedido " + dadosNotaPedido.n_pedido_cliente + " possuí carta correção; troca de transportador " + dadosNotaPedido.cnpj_transportadora;
                                }

                                repPedido.Atualizar(pedidoEntidade);
                            }
                        }

                        unitOfWork.CommitChanges();
                    }

                }
                catch (ServicoException e)
                {
                    pedidoAguardandoIntegracaoNota.Informacao = "TransID " + dadosNotaPedido?.trans_id + " - " + e.Message;
                    pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                    repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);
                }
                catch (Exception e)
                {
                    pedidoAguardandoIntegracaoNota.Informacao = "Erro genérico";
                    pedidoAguardandoIntegracaoNota.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                    repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);

                    Log.TratarErro($"Erro generico Consulta notas Emillenium pedido: {dadosNotaPedido.n_pedido_cliente} , {e.Message}");
                    Servicos.Log.TratarErro(e);
                }
            }

            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);
            AdicionarArquivoTransacao(pedidoAguardandoIntegracaoNota, arquivoRequisicao, arquivoResposta, unitOfWork);
            repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoNota);

        }

        /*
         * Retorna a lista resumida de pedidos entre duas datas
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido> BuscarListaPedidos(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao, DateTime dataCriacaoInicial, DateTime dataCriacaoFinal, out string request, out string response)
        {
            response = "";
            string dataCriacaoInicialFormatada = dataCriacaoInicial.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
            //string dataCriacaoFinalFormatada = dataCriacaoFinal.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido> embarques = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido>() { };

            //string Uri = $"{configuracao.URLEmillenium}/listapedidos3?$dateformat=iso&$format=json&data_emissao_inicial={dataCriacaoInicialFormatada}&data_emissao_final={dataCriacaoFinalFormatada}";
            string Uri = $"{configuracao.URLEmillenium}/listapedidos3_TMS?$dateformat=iso&$format=json&Data_Conclusao_Embarque={dataCriacaoInicialFormatada}";
            request = Uri;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEmillenium));
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Add("Dkt_api", $"{configuracao?.SenhaFrontDoor ?? string.Empty}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{configuracao.UsuarioEmillenium}:{configuracao.SenhaEmillenium}")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                response = body;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.RetornoListaPedidos retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.RetornoListaPedidos>(body);
                embarques.AddRange(retorno.Value);
            }
            else
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracao.IdIntegracao = "";
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracao.DataCriacaoPedido = DateTime.Now;
                pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.Emillenium;
                pedidoAguardandoIntegracao.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.Pedidos;
                pedidoAguardandoIntegracao.Informacao = "Erro ao Buscar Lista pedidos Emillenium - URL: " + Uri + "Erro: " + respostaServer.ReasonPhrase;

                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                Log.TratarErro("Retorno " + Uri + " - " + respostaServer.ReasonPhrase);
            }

            return embarques;
        }

        /*
        * Retorna detalhes do pedido
        */
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido BuscarPedidoNaApi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao, string codigoPedido, out string request, out string response)
        {
            response = "";

            string Uri = $"{configuracao.URLEmillenium}/listapedidos3_TMS?$dateformat=iso&$format=json&cod_pedidov={codigoPedido}";
            request = Uri;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEmillenium));
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Add("Dkt_api", $"{configuracao?.SenhaFrontDoor ?? string.Empty}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{configuracao.UsuarioEmillenium}:{configuracao.SenhaEmillenium}")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                response = body;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.RetornoListaPedidos retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.RetornoListaPedidos>(body);
                if (retorno.Value.Count > 0)
                {
                    return retorno.Value[0];
                }
            }
            else
            {
                Log.TratarErro("Retorno " + Uri + " - " + respostaServer.ReasonPhrase);
            }

            return null;
        }

        /*
        * Retorna detalhes do pedido com notas, carta correcao e cancelamentos
        */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> BuscarNotasPedidoNaApi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao, int transIdAtual, out string request, out string response, int chamado, int top = 0)
        {
            string Uri = "";
            if (chamado == 0)
            {
                if (top > 0)
                    Uri = $"{configuracao.URLEmillenium}/listafaturamentos?$dateformat=iso&$format=json&$top={top}&trans_id={transIdAtual}&gera_xml=t&aprovado=true&cancelada=true";
                else
                    Uri = $"{configuracao.URLEmillenium}/listafaturamentos?$dateformat=iso&$format=json&trans_id={transIdAtual}&gera_xml=t&aprovado=true&cancelada=true";
            }
            else if (chamado == 1)
                Uri = $"{configuracao.URLEmillenium}/listafaturamentos?$dateformat=iso&$format=json&trans_id={transIdAtual}&carta_correcao=t&gera_xml=t";
            else if (chamado == 2)
                Uri = $"{configuracao.URLEmillenium}/listafaturamentos?$dateformat=iso&$format=json&trans_id={transIdAtual}&cancelada=t&gera_xml=t";

            response = "";
            request = Uri;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEmillenium));
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Add("Dkt_api", $"{configuracao?.SenhaFrontDoor ?? string.Empty}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{configuracao.UsuarioEmillenium}:{configuracao.SenhaEmillenium}")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                response = body;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoNotasPedido retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoNotasPedido>(body);
                if (retorno.Value.Count > 0)
                {
                    return retorno.Value;
                }
            }
            else
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracao.IdIntegracao = "";
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracao.DataCriacaoPedido = DateTime.Now;
                pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.Emillenium;
                pedidoAguardandoIntegracao.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais;
                pedidoAguardandoIntegracao.Informacao = "Erro ao Buscar Notas Emillenium - URL: " + Uri + "Erro: " + respostaServer.ReasonPhrase;

                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                Log.TratarErro("Retorno " + Uri + " - " + respostaServer.ReasonPhrase);
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> BuscarNotasPorPedidoNaApi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao, string codigoPedido, out string request, out string response)
        {
            string Uri = $"{configuracao.URLEmillenium}/listafaturamentos?aprovado=true&cancelada=true&$format=json&$dateformat=iso&cod_pedidov={codigoPedido}&gera_xml=t";

            response = "";
            request = Uri;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEmillenium));
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Add("Dkt_api", $"{configuracao?.SenhaFrontDoor ?? string.Empty}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{configuracao.UsuarioEmillenium}:{configuracao.SenhaEmillenium}")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                response = body;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoNotasPedido retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoNotasPedido>(body);
                if (retorno.Value.Count > 0)
                {
                    return retorno.Value;
                }
            }
            else
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracao.IdIntegracao = "";
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracao.DataCriacaoPedido = DateTime.Now;
                pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.Emillenium;
                pedidoAguardandoIntegracao.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais;
                pedidoAguardandoIntegracao.Informacao = "Erro ao Buscar Notas Emillenium - URL: " + Uri + "Erro: " + respostaServer.ReasonPhrase;

                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                Log.TratarErro("Retorno " + Uri + " - " + respostaServer.ReasonPhrase);
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido> BuscarNotasPedidoNaApiManualmente(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao, int transId, out string request, out string response)
        {
            string Uri = $"{configuracao.URLEmillenium}/listafaturamentos?$dateformat=iso&$format=json&$top=50&trans_id={transId}&gera_xml=t&aprovado=true&cancelada=true";

            response = "";
            request = Uri;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEmillenium));
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.Add("Dkt_api", $"{configuracao?.SenhaFrontDoor ?? string.Empty}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{configuracao.UsuarioEmillenium}:{configuracao.SenhaEmillenium}")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                response = body;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoNotasPedido retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.RetornoNotasPedido>(body);
                if (retorno.Value.Count > 0)
                {
                    return retorno.Value;
                }
            }
            else
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracao.IdIntegracao = "";
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracao.DataCriacaoPedido = DateTime.Now;
                pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.Emillenium;
                pedidoAguardandoIntegracao.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.NotasFiscais;
                pedidoAguardandoIntegracao.Informacao = "Erro ao Buscar Notas Emillenium - URL: " + Uri + "Erro: " + respostaServer.ReasonPhrase;

                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                Log.TratarErro("Retorno " + Uri + " - " + respostaServer.ReasonPhrase);
            }

            return null;
        }

        private bool IntegrarPedidoAguardandoIntegracao(Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            try
            {
                string request = "";
                string response = "";

                Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido dadosCompletos = BuscarPedidoNaApi(configuracaoIntegracao, pedidoAguardandoIntegracao.IdIntegracao, out request, out response);

                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                if (dadosCompletos == null)
                    throw new ServicoException($"Dados do pedido não retornado da Emillenium");

                unitOfWork.Start();

                ValidarDadosPedido(dadosCompletos, pedidoAguardandoIntegracao, out var pedidoEntidade);

                bool deveCriarCarga = false;

                // Se um cliente ativo existir com o código de integração igual a transportadora_original_rnl_id, ele é o recebedor do pedido. Se não, é a transportadora
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Cliente clienteTransportador = !string.IsNullOrWhiteSpace(dadosCompletos.TransportadoraOriginalRnlId) ? repCliente.BuscarPorCodigoIntegracao(dadosCompletos.TransportadoraOriginalRnlId) : null;
                if (clienteTransportador != null)
                {
                    pedidoEntidade.Recebedor = clienteTransportador;
                    pedidoAguardandoIntegracao.Informacao = "";
                }
                else
                {
                    string idCanalEntrega = pedidoEntidade.CanalEntrega?.CodigoIntegracao ?? "";
                    string[] canaisNaoCriamCarga = { "RNL_Normal" };
                    deveCriarCarga = !canaisNaoCriamCarga.Contains(idCanalEntrega);

                    //Separado em nova Thread para gerar a carga com varios pedidos
                    //if(deveCriarCarga)
                    //{
                    //    var criadorCargaIntegracaoEmillenium = new CriadorCargaIntegracaoEmillenium(unitOfWork, tipoServicoMultisoftware);
                    //    criadorCargaIntegracaoEmillenium.CriarOuAdicionarNaCarga(dadosCompletos, pedidoEntidade, out var carga);
                    //    pedidoAguardandoIntegracao.Informacao = $"Pedido {pedidoEntidade.NumeroPedidoEmbarcador} adicionado na carga {carga.CodigoCargaEmbarcador}";
                    //} else 
                    //{
                    //    pedidoAguardandoIntegracao.Informacao = $"Pedido {pedidoEntidade.NumeroPedidoEmbarcador}. Lote {dadosCompletos.CodEmbarque}. Não foi criada/adicionada em nenhuma carga.";
                    //}
                }

                pedidoEntidade.CodigoPedidoCliente = pedidoAguardandoIntegracao.IdIntegracao;
                pedidoEntidade.NumeroLote = dadosCompletos.CodEmbarque;
                pedidoEntidade.NumeroRastreioCorreios = dadosCompletos.NumeroObjeto.Left(150);
                repPedido.Atualizar(pedidoEntidade);

                pedidoAguardandoIntegracao.NumeroCarga = dadosCompletos.CodEmbarque;
                pedidoAguardandoIntegracao.ObjetoPedido = JsonConvert.SerializeObject(dadosCompletos);
                pedidoAguardandoIntegracao.NumeroTentativas += 1;

                if (deveCriarCarga)
                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.AgGerarCarga;
                else
                {
                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                    pedidoAguardandoIntegracao.Informacao = $"Integracao Emillenium - Pedido {pedidoEntidade.NumeroPedidoEmbarcador} não deve gerar carga.";
                }

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                unitOfWork.CommitChanges();
            }
            catch (BaseException e)
            {
                if (unitOfWork.IsActiveTransaction())
                    unitOfWork.Rollback();
                pedidoAguardandoIntegracao.Informacao = e.Message;
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
            }
            catch (Exception e)
            {
                if (unitOfWork.IsActiveTransaction())
                    unitOfWork.Rollback();
                Log.TratarErro(e);
                pedidoAguardandoIntegracao.Informacao = "Erro genérico";
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ErroGenerico;
                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
            }

            return true;
        }

        private bool IntegrarPedidoAguardandoGerarCarga(string numeroCarga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> pedidosAguardandoIntegracao = repPedidoAguardandoIntegracao.BuscarPendentesCarga(numeroCarga);

            if (pedidosAguardandoIntegracao == null || pedidosAguardandoIntegracao.Count == 0)
                return true;

            try
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                foreach (var pedidoAguardandoIntegracao in pedidosAguardandoIntegracao)
                {
                    string request = "";
                    string response = "";

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido dadosCompletos = !string.IsNullOrWhiteSpace(pedidoAguardandoIntegracao.ObjetoPedido) ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido>(pedidoAguardandoIntegracao.ObjetoPedido) : BuscarPedidoNaApi(configuracaoIntegracao, pedidoAguardandoIntegracao.IdIntegracao, out request, out response);

                    if (dadosCompletos == null)
                        throw new ServicoException($"Dados do pedido " + pedidoAguardandoIntegracao.IdIntegracao + " não encontrado na Emillenium");

                    //Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade = null;
                    ValidarDadosPedido(dadosCompletos, pedidoAguardandoIntegracao, out var pedido);

                    listaPedidos.Add(pedido);
                }

                unitOfWork.Start();

                if (listaPedidos.Count > 0)
                {
                    var criadorCargaIntegracaoEmillenium = new CriadorCargaIntegracaoEmillenium(unitOfWork, tipoServicoMultisoftware);
                    criadorCargaIntegracaoEmillenium.CriarOuAdicionarNaCarga(numeroCarga, listaPedidos, out var carga);
                }

                foreach (var pedidoAguardandoIntegracao in pedidosAguardandoIntegracao)
                {
                    pedidoAguardandoIntegracao.NumeroTentativas += 1;
                    pedidoAguardandoIntegracao.Informacao = $"Pedido adicionado na carga {numeroCarga}";
                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;

                    repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
                }

                unitOfWork.CommitChanges();

            }
            catch (BaseException e)
            {
                Log.TratarErro(e);

                if (unitOfWork.IsActiveTransaction())
                    unitOfWork.Rollback();

                if (pedidosAguardandoIntegracao != null && pedidosAguardandoIntegracao.Count > 0)
                {
                    foreach (var pedidoAguardandoIntegracao in pedidosAguardandoIntegracao)
                    {
                        pedidoAguardandoIntegracao.Informacao = e.Message;
                        pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaGerarCarga;
                        repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
                    }
                }
            }
            catch (Exception e)
            {
                Log.TratarErro(e);

                if (unitOfWork.IsActiveTransaction())
                    unitOfWork.Rollback();

                if (pedidosAguardandoIntegracao != null && pedidosAguardandoIntegracao.Count > 0)
                {
                    foreach (var pedidoAguardandoIntegracao in pedidosAguardandoIntegracao)
                    {
                        pedidoAguardandoIntegracao.Informacao = "Erro genérico";
                        pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaGerarCarga;
                        repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
                    }
                }
            }

            return true;
        }

        private void ValidarDadosPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Pedido dadosCompletos, Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao, out Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade)
        {
            // O pedido já deve estar cadastrado, se não, falha
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (string.IsNullOrEmpty(dadosCompletos?.CodEmbarque))
            {
                if (string.IsNullOrEmpty(dadosCompletos.NPedidoCliente))
                {
                    throw new ServicoException($"A requisição não tem um código de lote (sem número de pedido)");
                }
                else
                {
                    throw new ServicoException($"A requisição não tem um código de lote (pedido {dadosCompletos.NPedidoCliente})");
                }
            }

            pedidoEntidade = repPedido.BuscarPorNumeroEmbarcador(dadosCompletos.NPedidoCliente);
            if (pedidoEntidade == null)
            {
                pedidoAguardandoIntegracao.NumeroCarga = dadosCompletos.CodEmbarque;
                throw new ServicoException($"O pedido com número {dadosCompletos.NPedidoCliente} não está cadastrado no sistema");
            }

            /// #45329 validacao nao deve existir pois ja esta se buscando pedidos pela dataembarque na chamada anterior;
            //if (dadosCompletos.StatusWorkflowDesc != "AG. CONFIRMAÇÃO DE ENTREGA"
            //    && dadosCompletos.StatusWorkflowDesc != "AG. CONFIRMAÇÃO DE ENTREGA")
            //{
            //    if (string.IsNullOrEmpty(dadosCompletos.NPedidoCliente))
            //    {
            //        throw new ServicoException($"O embarque ainda não foi finalizado na E-Millenium (sem número de pedido)");
            //    }
            //    else
            //    {
            //        throw new ServicoException($"O embarque ainda não foi finalizado na E-Millenium (pedido {dadosCompletos.NPedidoCliente})");
            //    }
            //}
        }

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return _configuracaoIntegracao;

            _configuracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoIntegracao;
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao, Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao, Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = arquivoRequisicao,
                ArquivoResposta = arquivoResposta,
                Data = DateTime.Now,
                Mensagem = "Dados Obtidos para Procesamento",
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            try
            {
                if (pedidoAguardandoIntegracao.ArquivosTransacao == null)
                    pedidoAguardandoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            }
            catch (Exception)
            {
                //forcar a criacao dos arquivos
                pedidoAguardandoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            }

            pedidoAguardandoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido CriarPedidoDevolucaoEmilleniumPorNota(Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade, Dominio.ObjetosDeValor.Embarcador.Integracao.Emillenium.Notas.Pedido dadosNotaPedido, Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracaoNota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

            Log.TratarErro($"NOVA NOTA DEVOLUÇÃO EMILLENIUM Pedido: {pedidoEntidade.NumeroPedidoEmbarcador}", "Notas_Emillenium_DEVOLUCAO");
            //CASO É UMA NOTA DE DEVOLUCAO DEVEMOS CRIAR UM NOVO PEDIDO (IGUAL AO PEDIDO ORIGINAL) e tambem uma carga para esse pedido, ja com a nota.

            // Necessário procurar no banco para ver se já foi cadastrado, porque a API não dá uma maneira confiável de fazer isso
            string NumeroPedidoDevolucao = "D-" + pedidoEntidade.NumeroPedidoEmbarcador;

            Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracaoDevolucao = repPedidoAguardandoIntegracao.BuscarPorIdTipo(NumeroPedidoDevolucao, TipoIntegracao.Emillenium, Dominio.Enumeradores.TipoIntegracaoEmillenium.PedidosDevolucao);
            if (pedidoAguardandoIntegracaoDevolucao == null)
            {
                pedidoAguardandoIntegracaoDevolucao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracaoDevolucao.IdIntegracao = NumeroPedidoDevolucao;
                pedidoAguardandoIntegracaoDevolucao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.AgIntegracao;
                pedidoAguardandoIntegracaoDevolucao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracaoDevolucao.DataCriacaoPedido = DateTime.Now;
                pedidoAguardandoIntegracaoDevolucao.TipoIntegracao = TipoIntegracao.Emillenium;
                pedidoAguardandoIntegracaoDevolucao.TipoIntegracaoEmillenium = Dominio.Enumeradores.TipoIntegracaoEmillenium.PedidosDevolucao;
                pedidoAguardandoIntegracaoDevolucao.Informacao = "Pedido " + dadosNotaPedido.n_pedido_cliente + " devolução. Nota Fiscal " + dadosNotaPedido.chave_nf + " adicionada a devolução.";
                pedidoAguardandoIntegracaoDevolucao.DataPesquisa = DateTime.Now;
                pedidoAguardandoIntegracaoDevolucao.DataEmbarquePedido = pedidoAguardandoIntegracaoNota.DataEmbarquePedido;
                pedidoAguardandoIntegracaoDevolucao.UltimaDataEmbarqueLista = pedidoAguardandoIntegracaoNota.UltimaDataEmbarqueLista;
                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracaoDevolucao);
            }

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoDevolucaoEntidade = null;

            if (!string.IsNullOrWhiteSpace(NumeroPedidoDevolucao))
                pedidoDevolucaoEntidade = repPedido.BuscarPorNumeroOrdem(NumeroPedidoDevolucao);
            else
                throw new ServicoException($"Campo n_pedido_cliente vazio");

            if (pedidoDevolucaoEntidade == null)
                pedidoDevolucaoEntidade = repPedido.BuscarPorNumeroPedidoEmbarcador(NumeroPedidoDevolucao);

            if (pedidoDevolucaoEntidade == null)
                pedidoDevolucaoEntidade = repPedido.BuscarPorNumeroEmbarcador(NumeroPedidoDevolucao);

            if (pedidoDevolucaoEntidade == null)
            {
                pedidoDevolucaoEntidade = pedidoEntidade.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(pedidoDevolucaoEntidade);

                pedidoDevolucaoEntidade.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                pedidoDevolucaoEntidade.Veiculos = pedidoEntidade.Veiculos != null ? pedidoEntidade.Veiculos.ToList() : new List<Dominio.Entidades.Veiculo>();
                pedidoDevolucaoEntidade.Motoristas = pedidoEntidade.Motoristas != null ? pedidoEntidade.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>();
                pedidoDevolucaoEntidade.NumeroPedidoEmbarcador = NumeroPedidoDevolucao;
                pedidoDevolucaoEntidade.ControleNumeracao = 0;
                pedidoDevolucaoEntidade.DataTerminoCarregamento = null;
                pedidoDevolucaoEntidade.DataAgendamento = null;
                pedidoDevolucaoEntidade.PrevisaoEntrega = null;

                Dominio.Entidades.Cliente remetente = pedidoEntidade.Destinatario;
                Dominio.Entidades.Cliente destinatario = pedidoEntidade.Remetente;

                Dominio.Entidades.Cliente expedidor = pedidoEntidade.Recebedor;
                Dominio.Entidades.Cliente recebedor = pedidoEntidade.Expedidor;

                Dominio.Entidades.Localidade origem = pedidoEntidade.Destino;
                Dominio.Entidades.Localidade destino = pedidoEntidade.Origem;

                pedidoDevolucaoEntidade.Remetente = remetente;
                pedidoDevolucaoEntidade.Destinatario = destinatario;
                pedidoDevolucaoEntidade.Expedidor = expedidor;
                pedidoDevolucaoEntidade.Recebedor = recebedor;
                pedidoDevolucaoEntidade.Origem = origem;
                pedidoDevolucaoEntidade.Destino = destino;
                pedidoDevolucaoEntidade.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalDevolucaoExiste = repXMLNotaFiscal.BuscarPorChave(dadosNotaPedido.chave_nf);

                pedidoDevolucaoEntidade.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                if (xmlNotaFiscalDevolucaoExiste != null)
                    pedidoDevolucaoEntidade.NotasFiscais.Add(xmlNotaFiscalDevolucaoExiste);

                repPedido.Inserir(pedidoDevolucaoEntidade);

                Log.TratarErro($"NOVO PEDIDO DEVOLUCAO: {pedidoDevolucaoEntidade.NumeroPedidoEmbarcador}", "Notas_Emillenium_DEVOLUCAO");
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            pedidos.Add(pedidoDevolucaoEntidade);

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            string mensagemErroCriarCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, tipoServicoMultisoftware, null, configuracao, true, true);

            if (!string.IsNullOrWhiteSpace(mensagemErroCriarCarga))
            {
                Servicos.Log.TratarErro("falha" + mensagemErroCriarCarga, "Notas_Emillenium_DEVOLUCAO");
                throw new ServicoException(mensagemErroCriarCarga);
            }

            repositorioPedido.Atualizar(pedidoDevolucaoEntidade);

            pedidoAguardandoIntegracaoDevolucao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
            pedidoAguardandoIntegracaoDevolucao.NumeroCarga = carga.CodigoCargaEmbarcador;
            repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracaoDevolucao);

            pedidoAguardandoIntegracaoNota.Informacao = $"Nota de devolucao recebida, Novo pedido criado: {NumeroPedidoDevolucao} Nota Fiscal {dadosNotaPedido.chave_nf} adicionada a devolução. Carga criada: {carga.CodigoCargaEmbarcador}";

            return pedidoDevolucaoEntidade;
        }

        #endregion Métodos Privados
    }
}

