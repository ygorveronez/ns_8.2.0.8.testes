using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/Lotes")]
    public class LotesDestinoAvariaController : BaseController
    {
		#region Construtores

		public LotesDestinoAvariaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarProdutosAvariados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtosAvariados = repProdutosAvariados.BuscarProdutosAvariadosDeLote(codigo);

                dynamic retorno = new
                {
                    ProdutosAvariados = (from obj in produtosAvariados
                                         select new
                                         {
                                             obj.Codigo,
                                             obj.SolicitacaoAvaria.NumeroAvaria,
                                             ProdutoEmbarcador = obj.ProdutoEmbarcador.Descricao,
                                             obj.NotaFiscal,
                                             obj.UnidadesAvariadas
                                         }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os produtos avariados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoLote = Request.GetIntParam("CodigoLote");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Produto", "Produto", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 20, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino> loteAvariasDestino = repLoteAvariaDestino.Consultar(codigoLote, parametrosConsulta);
                grid.setarQuantidadeTotal(repLoteAvariaDestino.ContarConsulta(codigoLote));

                var lista = (from p in loteAvariasDestino
                             select new
                             {
                                 p.Codigo,
                                 Produto = p.ProdutoEmbarcador.Descricao,
                                 p.Quantidade,
                                 Destino = p.Destino.ObterDescricao()
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = new Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino();

                PreencherLoteAvariaDestino(loteAvariaDestino, unitOfWork);

                repLoteAvariaDestino.Inserir(loteAvariaDestino);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = repLoteAvariaDestino.BuscarPorCodigo(codigo, true);

                if (loteAvariaDestino == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherLoteAvariaDestino(loteAvariaDestino, unitOfWork);

                repLoteAvariaDestino.Atualizar(loteAvariaDestino);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = repLoteAvariaDestino.BuscarPorCodigo(codigo, false);

                if (loteAvariaDestino == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynLoteAvariaDestino = new
                {
                    loteAvariaDestino.Codigo,
                    loteAvariaDestino.Quantidade,
                    loteAvariaDestino.Destino,
                    Valor = loteAvariaDestino.Valor.ToString("n2"),
                    DataVencimento = loteAvariaDestino.DataVencimento?.ToDateString(),
                    Produto = new { loteAvariaDestino.ProdutoEmbarcador.Codigo, loteAvariaDestino.ProdutoEmbarcador.Descricao },
                    Motorista = new { Codigo = loteAvariaDestino.Motorista?.Codigo ?? 0, Descricao = loteAvariaDestino.Motorista?.Descricao ?? string.Empty },
                    Cliente = new { Codigo = loteAvariaDestino.Cliente?.Codigo ?? 0, Descricao = loteAvariaDestino.Cliente?.Nome ?? string.Empty },
                    Carga = new { Codigo = loteAvariaDestino.Carga?.Codigo ?? 0, Descricao = loteAvariaDestino.Carga?.Descricao ?? string.Empty },
                    TipoMovimento = new { Codigo = loteAvariaDestino.TipoMovimento?.Codigo ?? 0, Descricao = loteAvariaDestino.TipoMovimento?.Descricao ?? string.Empty }
                };

                return new JsonpResult(dynLoteAvariaDestino);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);

                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = repLoteAvariaDestino.BuscarPorCodigo(codigo, true);

                if (loteAvariaDestino == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repLoteAvariaDestino.Deletar(loteAvariaDestino);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarDestinoAvaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                if (lote == null)
                    return new JsonpResult(false, true, "Lote não encontrado.");

                if (lote.Situacao != SituacaoLote.Finalizada)
                    return new JsonpResult(false, true, "Já foi finalizado o destino da avaria.");

                unitOfWork.Start();

                EfetivarDestinoProdutoAvaria(codigo, unitOfWork);

                lote.Situacao = SituacaoLote.FinalizadaComDestino;
                repLote.Atualizar(lote);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Finalizou destino da avaria", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o destino da avaria.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherLoteAvariaDestino(Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            int codigoLote = Request.GetIntParam("CodigoLote");
            int codigoProduto = Request.GetIntParam("Produto");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoCarga = Request.GetIntParam("Carga");
            int codigoTipoMovimento = Request.GetIntParam("TipoMovimento");
            double codigoCliente = Request.GetDoubleParam("Cliente");

            if (loteAvariaDestino.Codigo == 0)
            {
                if (!repProdutosAvariados.ProdutoAvariadoExisteNoLote(codigoLote, codigoProduto))
                    throw new ControllerException("Produto que está tentando adicionar não existe no lote");

                loteAvariaDestino.Lote = repLote.BuscarPorCodigo(codigoLote);
            }

            loteAvariaDestino.Quantidade = Request.GetIntParam("Quantidade");
            loteAvariaDestino.Destino = Request.GetEnumParam<DestinoProdutoAvaria>("Destino");
            loteAvariaDestino.Valor = Request.GetDecimalParam("Valor");
            loteAvariaDestino.DataVencimento = Request.GetNullableDateTimeParam("DataVencimento");

            loteAvariaDestino.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);
            loteAvariaDestino.Motorista = codigoMotorista > 0 ? repMotorista.BuscarPorCodigo(codigoMotorista) : null;
            loteAvariaDestino.Carga = codigoCarga > 0 ? repCarga.BuscarPorCodigo(codigoCarga) : null;
            loteAvariaDestino.TipoMovimento = codigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;
            loteAvariaDestino.Cliente = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Produto")
                return "ProdutoEmbarcador.Descricao";

            return propriedadeOrdenar;
        }

        private void EfetivarDestinoProdutoAvaria(int codigoLote, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

            List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino> loteAvariasDestino = repLoteAvariaDestino.BuscarPorLote(codigoLote);
            if (loteAvariasDestino.Count == 0)
                throw new ControllerException("Nenhum produto avariado foi lançado para destino");

            if (repLoteAvariaDestino.ExisteProdutoNaoInformado(codigoLote))
                throw new ControllerException("Há produtos avariados que não foram informado o destino");

            foreach (Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino in loteAvariasDestino)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = loteAvariaDestino.ProdutoEmbarcador;

                int quantidadeDasAvarias = repProdutosAvariados.QuantidadeProdutoAvariadoNoLote(codigoLote, produtoEmbarcador.Codigo);
                int quantidadeDosLotes = loteAvariasDestino.Where(o => o.ProdutoEmbarcador.Codigo == produtoEmbarcador.Codigo).Sum(o => o.Quantidade);

                if (quantidadeDasAvarias != loteAvariaDestino.Quantidade && quantidadeDosLotes != quantidadeDasAvarias)
                    throw new ControllerException($"Quantidade informada no produto { produtoEmbarcador.Descricao } é diferente da quantidade das avarias");

                if (loteAvariaDestino.Destino == DestinoProdutoAvaria.Descartada || loteAvariaDestino.Destino == DestinoProdutoAvaria.DevolvidaCliente)
                {
                    MovimentarEstoque(produtoEmbarcador, loteAvariaDestino.Quantidade, unitOfWork);
                }
                else
                {
                    GerarTituloFinanceiro(loteAvariaDestino, unitOfWork);
                    MovimentarEstoque(produtoEmbarcador, loteAvariaDestino.Quantidade, unitOfWork);
                }
            }
        }

        private void GerarTituloFinanceiro(Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            Dominio.Entidades.Cliente pessoa = loteAvariaDestino.Cliente;
            if (loteAvariaDestino.Destino == DestinoProdutoAvaria.DescontadaMotorista)
            {
                pessoa = repCliente.BuscarPorCPFCNPJ(loteAvariaDestino.Motorista.CPF.ToDouble());
                if (pessoa == null)
                {
                    if (loteAvariaDestino.Motorista.Localidade == null)
                        throw new ControllerException("Motorista está com o endereço incompleto no cadastro! Favor preencher antes de prosseguir.");

                    pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(loteAvariaDestino.Motorista, unitOfWork);
                    repCliente.Inserir(pessoa, Auditado);
                }
            }

            if (pessoa == null)
                throw new ControllerException("Cliente não está cadastrado");

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = Usuario;
            titulo.TipoTitulo = TipoTitulo.Receber;
            titulo.DataEmissao = DateTime.Now.Date;
            titulo.DataVencimento = loteAvariaDestino.DataVencimento;
            titulo.DataProgramacaoPagamento = titulo.DataVencimento;
            titulo.GrupoPessoas = pessoa.GrupoPessoas;
            titulo.Pessoa = pessoa;
            titulo.Sequencia = 1;
            titulo.ValorOriginal = loteAvariaDestino.Valor;
            titulo.ValorPendente = loteAvariaDestino.Valor;
            titulo.StatusTitulo = StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.Observacao = $"Referente ao destino da avaria do produto { loteAvariaDestino.ProdutoEmbarcador.Descricao } do lote nº { loteAvariaDestino.Lote.Numero }";
            titulo.Empresa = null;
            titulo.ValorTituloOriginal = titulo.ValorOriginal;
            titulo.TipoDocumentoTituloOriginal = "Lote Avaria";
            titulo.NumeroDocumentoTituloOriginal = loteAvariaDestino.Lote.Numero.ToString();
            titulo.FormaTitulo = FormaTitulo.Outros;
            titulo.TipoMovimento = loteAvariaDestino.TipoMovimento;

            if (titulo.DataVencimento.Value.Date < titulo.DataEmissao)
                throw new ControllerException("A data de vencimento não podem ser menor que a data de emissão.");

            repTitulo.Inserir(titulo);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Adicionado pelo Lote de Avaria.", unitOfWork);

            if (!svcProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Observacao, unitOfWork, TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, loteAvariaDestino.Motorista?.Codigo ?? 0, null, null, titulo.Codigo, TipoMovimentoEntidade.Saida, titulo.Pessoa, null, titulo.DataEmissao))
                throw new ControllerException(erro);

            loteAvariaDestino.Titulo = titulo;
            repLoteAvariaDestino.Atualizar(loteAvariaDestino);
        }

        private void MovimentarEstoque(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, int quantidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote produtoEmbarcadorLote = repProdutoEmbarcadorLote.BuscarPorProduto(produtoEmbarcador.Codigo);

            if (produtoEmbarcadorLote == null)
                throw new ControllerException($"O produto { produtoEmbarcador.Descricao } não possui lote no cadastro");

            produtoEmbarcadorLote.QuantidadeAtual -= quantidade;

            repProdutoEmbarcadorLote.Atualizar(produtoEmbarcadorLote);
        }

        #endregion
    }
}
