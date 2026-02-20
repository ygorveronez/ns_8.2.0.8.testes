using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize(new string[] { "ProdutosSeparacao", "Imprimir" }, "WMS/SeparacaoMercadorias")]
    public class SeparacaoMercadoriasController : BaseController
    {
		#region Construtores

		public SeparacaoMercadoriasController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.Separacao repSeparacaoMercadorias = new Repositorio.Embarcador.WMS.Separacao(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.Prop("Codigo");
                grid.Prop("Cargas").Nome("Cargas").Tamanho(30);
                grid.Prop("Data").Nome("Data").Align(Models.Grid.Align.center).Tamanho(30);
                grid.Prop("Situacao").Nome("Situação").Tamanho(15);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                int separador = this.Usuario.Codigo;

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao? situacao = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao situacaoAux))
                    situacao = situacaoAux;

                // Consulta
                List<Dominio.Entidades.Embarcador.WMS.Separacao> listaGrid = repSeparacaoMercadorias.Consultar(separador, situacao, dataInicial, dataFinal, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repSeparacaoMercadorias.ContarConsulta(separador, situacao, dataInicial, dataFinal);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                obj.Cargas,
                                Data = obj.Selecao.Data.ToString("dd/MM/yyyy"),
                                Situacao = obj.Selecao.DescricaoSituacaoSelecaoSeparacao
                            };

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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

        public async Task<IActionResult> ProdutosSeparacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                //grid.Prop("Pedido").Nome("Pedido").Tamanho(15);
                grid.Prop("Produto").Nome("Produto").Tamanho(15);
                grid.Prop("CodigoBarras").Nome("Cód. Barras").Tamanho(20);
                grid.Prop("DescricaoDepositoPosicao").Nome("Local Armazenamento").Tamanho(15);
                grid.Prop("QuantidadeSeparar").Nome("Qtd a Separar").Tamanho(10).Align(Models.Grid.Align.right);
                grid.Prop("QuantidadeSeparada").Nome("Qtd Separada").Tamanho(10).Align(Models.Grid.Align.right);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Produto")
                    propOrdenar = "ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao";
                else if (propOrdenar == "CodigoBarras")
                    propOrdenar = "ProdutoEmbarcadorLote.CodigoBarras";
                else if (propOrdenar == "DescricaoDepositoPosicao")
                    propOrdenar = "ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao";
                else if (propOrdenar == "QuantidadeSeparar")
                    propOrdenar = "Quantidade";

                // Dados do filtro
                int.TryParse(Request.Params("Codigo"), out int separacao);

                // Consulta
                List<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador> listaGrid = repSeparacaoProdutoEmbarcador.Consultar(separacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repSeparacaoProdutoEmbarcador.ContarConsulta(separacao);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                //Pedido = obj.Separacao.Selecao.Cargas.fir,
                                Produto = obj.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao,
                                CodigoBarras = obj.ProdutoEmbarcadorLote.CodigoBarras,
                                DescricaoDepositoPosicao = obj.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao,
                                QuantidadeSeparar = obj.Quantidade.ToString("n3"),
                                QuantidadeSeparada = obj.QuantidadeSeparada.ToString("n3"),
                            };

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.Separacao repSeparacaoMercadorias = new Repositorio.Embarcador.WMS.Separacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.Separacao separacao = repSeparacaoMercadorias.BuscarPorCodigo(codigo);

                // Valida
                if (separacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    separacao.Codigo,
                    Situacao = separacao.SituacaoSelecaoSeparacao,
                    Carga = this.CargasSeparacao(separacao),
                    Produtos = this.ProdutosSeparacao(separacao),
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutoEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Separacao"), out int separacao);
                string produto = Request.Params("Produto") ?? string.Empty;

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produtoEmbarcador = repSeparacaoProdutoEmbarcador.BuscarPorSeparacaoECodigoBarra(separacao, produto);

                // Valida
                if (produtoEmbarcador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    produtoEmbarcador.ProdutoEmbarcadorLote.ProdutoEmbarcador.Codigo,
                    produtoEmbarcador.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPosicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.DepositoPosicao reDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Separacao"), out int separacao);
                int.TryParse(Request.Params("Codigo"), out int codigo);
                string posicao = Request.Params("Posicao") ?? string.Empty;

                if (codigo > 0)
                {
                    Dominio.Entidades.Embarcador.WMS.DepositoPosicao depositoPosicao = reDepositoPosicao.BuscarPorCodigo(codigo);
                    if (depositoPosicao == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar o registro de deposito.");

                    // Formata retorno
                    var retorno = new
                    {
                        depositoPosicao.Codigo,
                        depositoPosicao.Abreviacao
                    };

                    // Retorna informacoes
                    return new JsonpResult(retorno);
                }
                else
                {

                    // Busca informacoes
                    Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produtoEmbarcador = repSeparacaoProdutoEmbarcador.BuscarPorSeparacaoEPosicao(separacao, posicao);

                    // Valida
                    if (produtoEmbarcador == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                    // Formata retorno
                    var retorno = new
                    {
                        produtoEmbarcador.ProdutoEmbarcadorLote.DepositoPosicao.Codigo,
                        produtoEmbarcador.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao
                    };

                    // Retorna informacoes
                    return new JsonpResult(retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ConferirVolume()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int separacao);
                int.TryParse(Request.Params("Posicao"), out int posicao);

                string numeroNS = Request.Params("NumeroNS");
                string volume = Request.Params("Volume");
                string cnpjRemetente = Request.Params("CNPJRemetente");
                string numeroNota = Request.Params("NumeroNota");
                string serieNota = Request.Params("SerieNota");
                string codigoBarrasLocalizar = Request.Params("CodigoBarrasLocalizar");

                //int.TryParse(Request.Params("ProdutoEmbarcador"), out int produto);
                //decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote = repProdutoEmbarcadorLote.BuscarPorCodigoBarrasPosicao(codigoBarrasLocalizar, posicao);
                if (lote == null)
                    return new JsonpResult(false, "Código de barras não localizado.");

                if (repSeparacaoProdutoEmbarcador.BuscarPorSeparacaoProdutoEPosicaoCodigoBarras(separacao, lote.ProdutoEmbarcador.Codigo, posicao, codigoBarrasLocalizar))
                    return new JsonpResult(false, "Código de barras já conferido.");

                unitOfWork.Start();

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produtoSeparacao = repSeparacaoProdutoEmbarcador.BuscarPorSeparacaoProdutoEPosicao(separacao, lote.ProdutoEmbarcador.Codigo, posicao, 1, codigoBarrasLocalizar);

                // Valida
                if (produtoSeparacao == null)
                    return new JsonpResult(true, false, "Não foi possível encontrar o registro.");

                if (produtoSeparacao.Separacao.SituacaoSelecaoSeparacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente)
                    return new JsonpResult(true, false, "A situação da separação não permite essa operação.");

                if (produtoSeparacao.QuantidadeSeparada == produtoSeparacao.Quantidade)
                    return new JsonpResult(true, false, "Esse produto no armazenamento " + produtoSeparacao.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao + " já foi separado.");

                decimal quantidadeSeparada = produtoSeparacao.QuantidadeSeparada;
                produtoSeparacao.QuantidadeSeparada += 1;
                if (!string.IsNullOrWhiteSpace(produtoSeparacao.CodigoBarrasConferido))
                    produtoSeparacao.CodigoBarrasConferido += "|" + codigoBarrasLocalizar;
                else
                    produtoSeparacao.CodigoBarrasConferido = codigoBarrasLocalizar;

                if (produtoSeparacao.QuantidadeSeparada > produtoSeparacao.Quantidade)
                    return new JsonpResult(true, false, "Não é possível separar quantidade superior a " + (produtoSeparacao.Quantidade - quantidadeSeparada).ToString("n3") + ".");

                // Persiste dados
                repSeparacaoProdutoEmbarcador.Atualizar(produtoSeparacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoSeparacao.Separacao, null, "Separou " + (1m).ToString("n3") + " de " + produtoSeparacao.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao + " em " + produtoSeparacao.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoSeparacao.Separacao.Selecao, null, "Separou " + (1m).ToString("n3") + " de " + produtoSeparacao.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao + " em " + produtoSeparacao.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int separacao);
                int.TryParse(Request.Params("Posicao"), out int posicao);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out int produto);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produtoSeparacao = repSeparacaoProdutoEmbarcador.BuscarPorSeparacaoProdutoEPosicao(separacao, produto, posicao, quantidade, "");

                // Valida
                if (produtoSeparacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (produtoSeparacao.Separacao.SituacaoSelecaoSeparacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente)
                    return new JsonpResult(false, true, "A situação da separação não permite essa operação.");

                if (produtoSeparacao.QuantidadeSeparada == produtoSeparacao.Quantidade)
                    return new JsonpResult(false, true, "Esse produto no armazenamento " + produtoSeparacao.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao + " já foi separado.");

                decimal quantidadeSeparada = produtoSeparacao.QuantidadeSeparada;
                produtoSeparacao.QuantidadeSeparada += quantidade;

                if (produtoSeparacao.QuantidadeSeparada > produtoSeparacao.Quantidade)
                    return new JsonpResult(false, true, "Não é possível separar quantidade superior a " + (produtoSeparacao.Quantidade - quantidadeSeparada).ToString("n3") + ".");

                // Persiste dados
                repSeparacaoProdutoEmbarcador.Atualizar(produtoSeparacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoSeparacao.Separacao, null, "Separou " + quantidade.ToString("n3") + " de " + produtoSeparacao.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao + " em " + produtoSeparacao.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoSeparacao.Separacao.Selecao, null, "Separou " + quantidade.ToString("n3") + " de " + produtoSeparacao.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao + " em " + produtoSeparacao.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.Separacao repSeparacaoMercadorias = new Repositorio.Embarcador.WMS.Separacao(unitOfWork);
                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.Separacao separacao = repSeparacaoMercadorias.BuscarPorCodigo(codigo);

                // Valida
                if (separacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (separacao.SituacaoSelecaoSeparacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente)
                    return new JsonpResult(false, true, "A situação da separação não permite essa operação.");

                if (!ValidaFinalizacaoSeparacao(separacao, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                separacao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada;
                separacao.Selecao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada;

                // Persiste dados
                repSeparacaoMercadorias.Atualizar(separacao);
                repSelecao.Atualizar(separacao.Selecao);

                Servicos.Embarcador.WMS.LoteProdutoEmbarcador.BaixarNoEstoqueSeparacao(separacao, unitOfWork, Auditado);
                Servicos.Embarcador.WMS.LoteProdutoEmbarcador.GerarExpedicao(separacao, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, separacao, null, "Finalizou a Separação", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, separacao.Selecao, null, "Finalizou a Separação", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                var pdf = ReportRequest.WithType(ReportType.ImpressaoProdutos)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo",codigo.ToString())
                    .CallReport()
                    .GetContentFile();
            
                return Arquivo(pdf, "application/pdf", "Separação Produtos.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.Separacao repSeparacaoMercadorias = new Repositorio.Embarcador.WMS.Separacao(unitOfWork);
                Repositorio.Embarcador.WMS.Selecao repSelecao = new Repositorio.Embarcador.WMS.Selecao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.Separacao separacao = repSeparacaoMercadorias.BuscarPorCodigo(codigo);

                // Valida
                if (separacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (separacao.SituacaoSelecaoSeparacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente)
                    return new JsonpResult(false, true, "A situação da separação não permite essa operação.");

                separacao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Cancelada;
                separacao.Selecao.SituacaoSelecaoSeparacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Cancelada;

                // Persiste dados
                repSeparacaoMercadorias.Atualizar(separacao);
                repSelecao.Atualizar(separacao.Selecao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, separacao, null, "Cancelou a Separação", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, separacao.Selecao, null, "Cancelou a Separação", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "Data") propOrdenar = "Selecao.Data";
            else if (propOrdenar == "Situacao") propOrdenar = "Selecao.SituacaoSelecaoSeparacao";
        }

        private string CargasSeparacao(Dominio.Entidades.Embarcador.WMS.Separacao separacao)
        {
            List<string> codigosCargas = (from c in separacao.Selecao.Cargas select c.Carga.CodigoCargaEmbarcador).Distinct().ToList();

            if (codigosCargas.Count == 0)
                return " - ";

            return String.Join(", ", codigosCargas);
        }

        private string ProdutosSeparacao(Dominio.Entidades.Embarcador.WMS.Separacao separacao)
        {
            List<string> nomesProdutos = (from p in separacao.Produtos select p.ProdutoEmbarcadorLote.ProdutoEmbarcador.Descricao).Distinct().ToList();

            if (nomesProdutos.Count == 0)
                return " - ";

            return String.Join(", ", nomesProdutos);
        }

        private bool ValidaFinalizacaoSeparacao(Dominio.Entidades.Embarcador.WMS.Separacao separacao, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            erro = "";
            List<string> msgProdutos = new List<string>();
            Dictionary<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador, decimal> produtosNaoSeparados = new Dictionary<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador, decimal>();

            foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produto in separacao.Produtos)
            {
                if (produto.Quantidade > produto.QuantidadeSeparada)
                {
                    produtosNaoSeparados.TryGetValue(produto.ProdutoEmbarcadorLote.ProdutoEmbarcador, out decimal quantidade);
                    produtosNaoSeparados[produto.ProdutoEmbarcadorLote.ProdutoEmbarcador] = quantidade + (produto.Quantidade - produto.QuantidadeSeparada);
                }
            }

            foreach (var produtoNaoSeparado in produtosNaoSeparados)
            {
                string msg = "Falta separar ";
                msg += produtoNaoSeparado.Value.ToString("n3");
                msg += " de ";
                msg += produtoNaoSeparado.Key.Descricao;

                msgProdutos.Add(msg);
            }

            if (msgProdutos.Count > 0)
            {
                erro = string.Join("<br>", msgProdutos);
                return false;
            }

            return true;
        }
        #endregion
    }
}
