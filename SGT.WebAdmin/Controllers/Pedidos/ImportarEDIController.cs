using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/ImportarEDI")]
    public class ImportarEDIController : BaseController
    {
		#region Construtores

		public ImportarEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacaoDescricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoaDescricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Layout EDI", "LayoutEDIDescricao", 10, Models.Grid.Align.left, true);


                Repositorio.Embarcador.Pedidos.ImportarEDI repImportarEDI = new Repositorio.Embarcador.Pedidos.ImportarEDI(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI> importarEDIs = repImportarEDI.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repImportarEDI.ContarConsulta(filtrosPesquisa));

                var lista = (from p in importarEDIs
                             select new
                             {
                                 p.Codigo,
                                 TipoOperacaoDescricao = p.TipoOperacao?.Descricao,
                                 GrupoPessoaDescricao = p.GrupoPessoas?.Descricao,
                                 LayoutEDIDescricao = p.LayoutEDI?.Descricao,
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Processar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.ImportarEDI repImportarEDI = new Repositorio.Embarcador.Pedidos.ImportarEDI(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI = new Dominio.Entidades.Embarcador.Pedidos.ImportarEDI();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                PreencherImportarEDI(importarEDI, unitOfWork);
                ProcessarArquivo(importarEDI, unitOfWork);

                repImportarEDI.Inserir(importarEDI, Auditado);


                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (BaseException execption)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, false, execption.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao importar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ImportarEDI repImportarEDI = new Repositorio.Embarcador.Pedidos.ImportarEDI(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI = repImportarEDI.BuscarPorCodigo(codigo, false);

                var dynImportarEDI = new
                {
                    importarEDI.Codigo,
                    importarEDI.TipoOperacao,
                    importarEDI.GrupoPessoas,
                    importarEDI.LayoutEDI,
                    importarEDI.Empresa
                };

                return new JsonpResult(dynImportarEDI);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ImportarEDI repImportarEDI = new Repositorio.Embarcador.Pedidos.ImportarEDI(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI = repImportarEDI.BuscarPorCodigo(codigo, true);

                if (importarEDI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repImportarEDI.Deletar(importarEDI, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarViaWs()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Pedidos.ImportarEDI repImportarEDI = new Repositorio.Embarcador.Pedidos.ImportarEDI(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI = new Dominio.Entidades.Embarcador.Pedidos.ImportarEDI();

                int codigoEmpresa = Request.GetIntParam("Empresa");
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin integracaoMichelin = new Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin(unitOfWork);
                Dominio.ObjetosDeValor.EDI.Pedido.Pedido pedidos = integracaoMichelin.ObterPedidos(empresa?.CNPJ_SemFormato ?? "");

                if (pedidos == null)
                    return new JsonpResult(false, "Não há pedidos para serem importados.");

                //unitOfWork.Start();

                PreencherImportarEDI(importarEDI, unitOfWork);

                SalvarPedidos(importarEDI, pedidos, unitOfWork, true);

                repImportarEDI.Inserir(importarEDI, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                //unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherImportarEDI(Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");
            int codigoLayoutEDI = Request.GetIntParam("LayoutEDI");
            int codigoEmpresa = Request.GetIntParam("Empresa");

            importarEDI.Usuario = this.Usuario;
            importarEDI.LayoutEDI = repLayoutEDI.BuscarPorCodigo(codigoLayoutEDI);
            importarEDI.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            importarEDI.GrupoPessoas = repGrupoPessoa.BuscarPorCodigo(codigoGrupoPessoa);
            importarEDI.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
            importarEDI.DataHora = DateTime.Now;
        }

        private void ProcessarArquivo(Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.DTO.CustomFile file = HttpContext.GetFile();

            importarEDI.NomeArquivo = file.FileName;
            Servicos.LeituraEDI svcLeituraEDI = new Servicos.LeituraEDI(this.Usuario.Empresa, importarEDI.LayoutEDI, file.InputStream, unitOfWork);
            Dominio.ObjetosDeValor.EDI.Pedido.Pedido pedidos = svcLeituraEDI.LerPedido();
            SalvarPedidos(importarEDI, pedidos, unitOfWork, false);

        }

        private void SalvarPedidos(Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI, Dominio.ObjetosDeValor.EDI.Pedido.Pedido pedidos, Repositorio.UnitOfWork unitOfWork, bool viaWS)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoMichelin repPedidoMichelin = new Repositorio.Embarcador.Pedidos.PedidoMichelin(unitOfWork);

            double cnpjRemetente = pedidos.CabecalhoDocumento.CNPJEmissora.ToDouble();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionados = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = this.ConfiguracaoEmbarcador;
            List<int> codigosPedidosExistentes = new List<int>();
            List<int> codigosPedidosParaCancelar = new List<int>();
            List<string> nomesArquivosRecebidos = new List<string>();
            List<string> nomesArquivosJaNaBase = new List<string>();

            if (pedidos.Pedidos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = ObterPedidoBase(importarEDI, unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionarLista = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                foreach (var pedido in pedidos.Pedidos)
                {
                    if (!string.IsNullOrWhiteSpace(pedido.NomeArquivo) && nomesArquivosJaNaBase.Contains(pedido.NomeArquivo))
                        continue;

                    if (!string.IsNullOrWhiteSpace(pedido.NomeArquivo) && !nomesArquivosRecebidos.Contains(pedido.NomeArquivo))
                    {
                        if (!repPedidoMichelin.ContemPorNomeArquivo(pedido.NomeArquivo))
                        {
                            nomesArquivosRecebidos.Add(pedido.NomeArquivo);
                            Dominio.Entidades.Embarcador.Pedidos.PedidoMichelin pedidoMichelin = new Dominio.Entidades.Embarcador.Pedidos.PedidoMichelin()
                            {
                                DataCriacao = pedido.DataCriacao,
                                NomeArquivo = pedido.NomeArquivo
                            };
                            repPedidoMichelin.Inserir(pedidoMichelin);
                        }
                        else
                        {
                            nomesArquivosJaNaBase.Add(pedido.NomeArquivo);
                            continue;
                        }
                    }

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExistente = repPedido.BuscarPorNumeroPedidoEmbarcadorECodigoPedidoCliente(pedido.NumeroCarga, pedido.NumeroPedido);
                    if (pedidoExistente != null && (pedidoBase?.TipoOperacao?.ConfiguracaoIntegracao?.AtivarRegraCancelamentoDosPedidosMichelin ?? false))
                    {
                        if (pedido.DataCriacao.HasValue)
                        {
                            TimeSpan diff = DateTime.Now - pedido.DataCriacao.Value.AddHours(-4);
                            double hours = diff.TotalHours;
                            if (hours > (pedidoBase?.TipoOperacao?.ConfiguracaoIntegracao?.HorasParaCalculoCancelamento ?? 3))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoExistente, null, "Este pedido foi recebido via WS, porém será cancelado devido a regra da sua hora de criação", unitOfWork);
                                codigosPedidosParaCancelar.Add(pedidoExistente.Codigo);
                                continue;
                            }
                        }
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoExistente, null, "Pedido recebido novamente via " + (viaWS ? " WS." : " EDI."), unitOfWork);
                        codigosPedidosExistentes.Add(pedidoExistente.Codigo);
                        continue;
                    }
                    else if (pedidoExistente != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoExistente, null, "Pedido recebido novamente via " + (viaWS ? " WS." : " EDI."), unitOfWork);
                        codigosPedidosExistentes.Add(pedidoExistente.Codigo);
                        continue;
                    }

                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCodigoIntegracao(pedido.CodigoCliente);

                    if (destinatario == null)
                        throw new ControllerException($"Não foi encontrado o código destinatário {pedido.CodigoCliente} no pedido {pedido.NumeroCarga}");

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionar = pedidoBase.Clonar();

                    if (!string.IsNullOrWhiteSpace(pedido.CNPJRemetente))
                    {
                        double cnpjRemetentePedido = pedido.CNPJRemetente.ToDouble();
                        if (cnpjRemetentePedido > 0)
                            remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetentePedido);
                    }

                    pedidoAdicionar.NumeroPedidoEmbarcador = pedido.NumeroCarga;
                    pedidoAdicionar.CodigoPedidoCliente = pedido.NumeroPedido;
                    pedidoAdicionar.Destinatario = destinatario;
                    pedidoAdicionar.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
                    pedidoAdicionar.Remetente = remetente;
                    pedidoAdicionar.Origem = remetente.Localidade;
                    pedidoAdicionar.PesoLiquidoTotal = pedido.Peso;
                    pedidoAdicionar.Destino = repLocalidade.BuscarPorDescricaoEUF(pedido.Cidade, pedido.UF);
                    pedidoAdicionar.DataCarregamentoPedido = pedido.DataFaturamento;
                    pedidoAdicionar.DataInicialColeta = pedido.DataFaturamento;
                    pedidoAdicionar.TipoDeCarga = !string.IsNullOrWhiteSpace(pedido.Item) ? repTipoDeCarga.BuscarPorCodigoEmbarcador(pedido.Item) : null;

                    pedidoAdicionar.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
                    pedidoAdicionar.Numero = pedidoAdicionar.NumeroSequenciaPedido;
                    pedidoAdicionar.PesoTotal = pedido.Peso;
                    pedidoAdicionar.PesoSaldoRestante = pedido.Peso;
                    pedidoAdicionar.QtVolumes = (int)Math.Truncate(pedido.Quantidade);

                    pedidoAdicionar.FileIdMichelin = pedido.FileId;
                    pedidoAdicionar.MessageIdentifierCodeMichelin = pedido.MessageIdentifierCode;

                    PreencherCodigoCargaEmbarcador(unitOfWork, pedidoAdicionar);
                    repPedido.Inserir(pedidoAdicionar);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProduto.buscarPorCodigoEmbarcador(pedido.CodigoItem.Trim());

                    if (produto == null)
                    {
                        produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                        produto.Descricao = pedido.DescricaoItem;
                        produto.Integrado = false;
                        produto.CodigoProdutoEmbarcador = pedido.CodigoItem.Trim();
                        repProduto.Inserir(produto);
                    }

                    //pedidoAdicionar.CubagemTotal = produto.MetroCubito;
                    pedidoAdicionar.ProdutoPredominante = produto.Descricao;

                    pedidoProduto.Pedido = pedidoAdicionar;
                    pedidoProduto.Produto = produto;
                    pedidoProduto.Quantidade = pedido.Quantidade;
                    pedidoProduto.PesoUnitario = pedido.Peso > 0 && pedido.Quantidade > 0 ? pedido.Peso / pedido.Quantidade : 0m;
                    pedidoProduto.MetroCubico = configuracao.MetroCubicoPorUnidadePedidoProdutoIntegracao ? (produto.MetroCubito * pedidoProduto.Quantidade) : produto.MetroCubito;

                    repPedidoProduto.Inserir(pedidoProduto);

                    pedidoAdicionar.CubagemTotal += pedidoProduto.MetroCubico;

                    pedidoAdicionar.Protocolo = pedidoAdicionar.Codigo;

                    if (VerificarRegrasPedido(pedidoAdicionar, TipoServicoMultisoftware, unitOfWork))
                    {
                        pedidoAdicionar.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente;
                        pedidoAdicionar.EtapaPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.AgAutorizacao;
                    }
                    else
                    {
                        pedidoAdicionar.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                        pedidoAdicionar.EtapaPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Finalizada;
                    }

                    repPedido.Atualizar(pedidoAdicionar);
                    codigosPedidosExistentes.Add(pedidoAdicionar.Codigo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoAdicionar, null, "Pedido inserido via " + (viaWS ? " WS." : " EDI."), unitOfWork);

                    pedidosAdicionarLista.Add(pedidoAdicionar);
                }

                if (pedidosAdicionarLista.Count <= 0 && (codigosPedidosParaCancelar == null || codigosPedidosParaCancelar.Count == 0))
                    throw new ControllerException($"Os pedidos deste EDI/WS já foram importados!");

                if (pedidosAdicionarLista.Count > 0 && IsGerarCargaAutomaticamente(pedidoBase))
                {
                    Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    string mensagemErroCriarCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidosAdicionarLista, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador, false, true);

                    if (!string.IsNullOrWhiteSpace(mensagemErroCriarCarga))
                        throw new ControllerException(mensagemErroCriarCarga);

                    importarEDI.Pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionarLista)
                    {
                        importarEDI.Pedidos.Add(pedido);
                        repositorioPedido.Atualizar(pedido);
                    }

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                        PreencherDadosRotaFreteClienteDeslocamento(unitOfWork, pedido);
                }

                if (viaWS)
                {
                    List<int> pedidosQueNaoRetornaram = repPedido.BuscarPedidosParaCancelamento(codigosPedidosExistentes);
                    if (pedidosQueNaoRetornaram != null && pedidosQueNaoRetornaram.Count > 0)
                    {
                        foreach (var codigoPedidoCancelar in pedidosQueNaoRetornaram)
                        {
                            if (!repCargaPedido.ContemPedidoEmCarga(codigoPedidoCancelar))
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCancelar = repPedido.BuscarPorCodigo(codigoPedidoCancelar, true);
                                pedidoCancelar.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                                repPedido.Atualizar(pedidoCancelar, Auditado);

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoCancelar, null, "Pedido cancelado via integração da WS Michelin pelo fator de dias em aberto.", unitOfWork);
                            }
                        }
                    }
                    if (codigosPedidosParaCancelar != null && codigosPedidosParaCancelar.Count > 0)
                    {
                        foreach (var codigoPedidoCancelar in codigosPedidosParaCancelar)
                        {
                            if (!repCargaPedido.ContemPedidoEmCarga(codigoPedidoCancelar))
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCancelar = repPedido.BuscarPorCodigo(codigoPedidoCancelar, true);
                                pedidoCancelar.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                                repPedido.Atualizar(pedidoCancelar, Auditado);

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoCancelar, null, "Pedido cancelado via integração da WS Michelin pelo fator de calculo de horas.", unitOfWork);
                            }
                        }
                    }
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI()
            {
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
            };

            return filtrosPesquisa;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoBase(Dominio.Entidades.Embarcador.Pedidos.ImportarEDI importarEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

            pedido.TipoOperacao = importarEDI.TipoOperacao;

            pedido.PedidoIntegradoEmbarcador = !ConfiguracaoEmbarcador.UtilizarIntegracaoPedido;
            pedido.GerarAutomaticamenteCargaDoPedido = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ? Request.GetBoolParam("GerarAutomaticamenteCargaDoPedido") : true;
            pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.DataCadastro = DateTime.Now;
            pedido.Usuario = this.Usuario;
            pedido.Autor = this.Usuario;

            if (pedido.QtdEntregas == 0)
                pedido.QtdEntregas = 1;

            pedido.TipoOperacao = importarEDI.TipoOperacao;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (pedido.TipoOperacao != null)
                {
                    if (!pedido.TipoOperacao.GeraCargaAutomaticamente)
                    {
                        pedido.GerarAutomaticamenteCargaDoPedido = false;
                        pedido.PedidoTotalmenteCarregado = false;
                        //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                        Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. ImportarEDIController.ObterPedidoBase", "SaldoPedido");
                    }

                    pedido.ColetaEmProdutorRural = pedido.TipoOperacao.ColetaEmProdutorRural;
                }
            }

            pedido.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.LiberacaoFinanceira;

            return pedido;
        }

        private bool IsGerarCargaAutomaticamente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return (pedido.PedidoIntegradoEmbarcador && !pedido.ColetaEmProdutorRural && !pedido.Cotacao);
        }

        private bool VerificarRegrasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaFiltrada = Servicos.Embarcador.Pedido.Pedido.VerificarRegrasPedido(pedido, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.Pedido.Pedido.CriarRegrasAutorizacao(listaFiltrada, pedido, this.Usuario, tipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                return true;
            }

            return false;
        }

        private void PreencherDadosRotaFreteClienteDeslocamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Dominio.Entidades.RotaFrete rotaFreteClienteDeslocamento = ObterRotaFreteClienteDeslocamento(unitOfWork, pedido);

            if (rotaFreteClienteDeslocamento != null)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repositorioCargaPedido.BuscarPorPedido(pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
                {
                    cargaPedido.Carga.RotaClienteDeslocamento = rotaFreteClienteDeslocamento;
                    cargaPedido.Carga.DeslocamentoQuilometros = rotaFreteClienteDeslocamento.Quilometros;

                    repositorioCarga.Atualizar(cargaPedido.Carga);
                }
            }
        }

        private Dominio.Entidades.RotaFrete ObterRotaFreteClienteDeslocamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.TipoOperacao?.UtilizarDeslocamentoPedido ?? false)
            {
                if (pedido.Remetente == null)
                    throw new ControllerException("Cliente remetente não informado.");

                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Dominio.Entidades.RotaFrete rotaFreteClienteDeslocamento = repositorioRotaFrete.BuscarPorRemetenteDestinatario(pedido.ClienteDeslocamento.CPF_CNPJ, pedido.Remetente?.CPF_CNPJ ?? 0d);

                if (rotaFreteClienteDeslocamento == null)
                {
                    List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
                    remetentes.Add(pedido.ClienteDeslocamento);

                    List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
                    destinatarios.Add(pedido.Remetente);

                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
                    destinatariosOrdenados.AddRange(from obj in destinatarios
                                                    select new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                                    {
                                                        Cliente = obj,
                                                        Ordem = 0
                                                    });

                    rotaFreteClienteDeslocamento = Servicos.Embarcador.Carga.RotaFrete.GerarRota(remetentes, destinatariosOrdenados, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota, null, Auditado);
                    if (rotaFreteClienteDeslocamento == null)
                        throw new ControllerException("Rota de frete entre o cliente de deslocamento e o remetente não encontrada.");
                }

                return rotaFreteClienteDeslocamento;
            }

            return null;
        }

        private void PreencherCodigoCargaEmbarcador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (IsGerarCargaAutomaticamente(pedido) && pedido.GerarAutomaticamenteCargaDoPedido)
            {
                if (!ConfiguracaoEmbarcador.NumeroCargaSequencialUnico)
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                else
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();

                pedido.AdicionadaManualmente = true;
            }
        }

        #endregion
    }
}
