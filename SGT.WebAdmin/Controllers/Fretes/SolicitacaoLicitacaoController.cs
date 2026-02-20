using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/SolicitacaoLicitacao")]
    public class SolicitacaoLicitacaoController : BaseController
    {
		#region Construtores

		public SolicitacaoLicitacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaSolicitacaoLicitacao filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("DataPrazoResposta", false);
                grid.AdicionarCabecalho("DescricaoCotacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Origem", "Origem", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);

                if (filtrosPesquisa.Situacao == SituacaoSolicitacaoLicitacao.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao> dados = repSolicitacaoLicitacao.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSolicitacaoLicitacao.ContarConsulta(filtrosPesquisa));

                var lista = (from p in dados
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.Numero,
                                 p.DescricaoCotacao,
                                 DataPrazoResposta = p.DataPrazoResposta.ToString("dd/MM/yyyy"),
                                 Origem = p.ClienteOrigem?.Descricao ?? p.LocalidadeOrigem?.DescricaoCidadeEstado,
                                 Destino = p.ClienteDestino?.Descricao ?? p.LocalidadeDestino?.DescricaoCidadeEstado,
                                 Data = p.Data.ToString("dd/MM/yyyy HH:mm"),
                                 DescricaoSituacao = p.Situacao.ObterDescricao()
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

                Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao = new Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao();

                PreencherSolicitacaoLicitacao(solicitacaoLicitacao, unitOfWork);

                repSolicitacaoLicitacao.Inserir(solicitacaoLicitacao, Auditado);

                AdicionarProdutos(solicitacaoLicitacao, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
                Repositorio.Embarcador.Frete.SolicitacaoLicitacaoProduto repositorioSolicitacaoLicitacaoProduto = new Repositorio.Embarcador.Frete.SolicitacaoLicitacaoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao = repSolicitacaoLicitacao.BuscarPorCodigo(codigo, false);

                List<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto> produtos = repositorioSolicitacaoLicitacaoProduto.BuscarPorSolicitacaoLicitacao(solicitacaoLicitacao.Codigo);

                var dynSolicitacaoLicitacao = new
                {
                    solicitacaoLicitacao.Codigo,
                    solicitacaoLicitacao.Situacao,
                    solicitacaoLicitacao.Numero,
                    Comprimento = solicitacaoLicitacao.Comprimento > 0 ? solicitacaoLicitacao.Comprimento.ToString("n2") : "",
                    Altura = solicitacaoLicitacao.Altura > 0 ? solicitacaoLicitacao.Altura.ToString("n2") : "",
                    Largura = solicitacaoLicitacao.Largura > 0 ? solicitacaoLicitacao.Largura.ToString("n2") : "",
                    TipoDeCarga = solicitacaoLicitacao.TipoDeCarga != null ? new { solicitacaoLicitacao.TipoDeCarga.Codigo, solicitacaoLicitacao.TipoDeCarga.Descricao } : null,
                    solicitacaoLicitacao.Observacao,
                    Usuario = solicitacaoLicitacao.Usuario.Nome,
                    EnderecoOrigem = solicitacaoLicitacao.ClienteOrigem != null ? $"{solicitacaoLicitacao.ClienteOrigem?.Endereco} N° {solicitacaoLicitacao.ClienteOrigem?.Numero} ({solicitacaoLicitacao.ClienteOrigem?.Localidade?.Descricao})" : "",
                    EnderecoDestino = solicitacaoLicitacao.ClienteDestino != null ? $"{solicitacaoLicitacao.ClienteDestino?.Endereco} N° {solicitacaoLicitacao.ClienteDestino?.Numero} ({solicitacaoLicitacao.ClienteDestino?.Localidade?.Descricao})" : "",
                    UsuarioLogadoCriouACotacao = solicitacaoLicitacao.Usuario.Codigo == Usuario.Codigo,
                    Quantidade = solicitacaoLicitacao.Quantidade.ToString("n2"),
                    DataInicioEmbarque = solicitacaoLicitacao.DataInicioEmbarque.ToString("dd/MM/yyyy"),
                    DataFimEmbarque = solicitacaoLicitacao.DataFimEmbarque.ToString("dd/MM/yyyy"),
                    DataPrazoResposta = solicitacaoLicitacao.DataPrazoResposta.ToString("dd/MM/yyyy"),
                    TipoOrigem = solicitacaoLicitacao.LocalidadeOrigem != null ? PessoaLocalidade.Localidade : PessoaLocalidade.Pessoa,
                    ClienteOrigem = solicitacaoLicitacao.ClienteOrigem != null ? new { solicitacaoLicitacao.ClienteOrigem.Codigo, solicitacaoLicitacao.ClienteOrigem.Descricao } : null,
                    LocalidadeOrigem = solicitacaoLicitacao.LocalidadeOrigem != null ? new { solicitacaoLicitacao.LocalidadeOrigem.Codigo, Descricao = solicitacaoLicitacao.LocalidadeOrigem.DescricaoCidadeEstado } : null,
                    TipoDestino = solicitacaoLicitacao.LocalidadeDestino != null ? PessoaLocalidade.Localidade : PessoaLocalidade.Pessoa,
                    ClienteDestino = solicitacaoLicitacao.ClienteDestino != null ? new { solicitacaoLicitacao.ClienteDestino.Codigo, solicitacaoLicitacao.ClienteDestino.Descricao } : null,
                    LocalidadeDestino = solicitacaoLicitacao.LocalidadeDestino != null ? new { solicitacaoLicitacao.LocalidadeDestino.Codigo, Descricao = solicitacaoLicitacao.LocalidadeDestino.DescricaoCidadeEstado } : null,
                    UnidadeMedida = solicitacaoLicitacao.UnidadeMedida != null ? new { solicitacaoLicitacao.UnidadeMedida.Codigo, solicitacaoLicitacao.UnidadeMedida.Descricao } : null,
                    solicitacaoLicitacao.DescricaoCotacao,
                    Produtos = (from produto in produtos
                                select new
                                {
                                    produto.ProdutoEmbarcador.Codigo,
                                    produto.ProdutoEmbarcador.Descricao
                                }).ToList(),
                    Cotacao = new
                    {
                        solicitacaoLicitacao.Codigo,
                        //Resumo
                        Origem = solicitacaoLicitacao.ClienteOrigem?.Descricao ?? solicitacaoLicitacao.LocalidadeOrigem?.DescricaoCidadeEstado,
                        Destino = solicitacaoLicitacao.ClienteDestino?.Descricao ?? solicitacaoLicitacao.LocalidadeDestino?.DescricaoCidadeEstado,
                        Produto = produtos.Count > 0 ? string.Join(", ", produtos.Select(x => x.ProdutoEmbarcador.Descricao)) : "",
                        Acondicionamento = solicitacaoLicitacao.TipoDeCarga?.Descricao ?? "",
                        Quantidade = solicitacaoLicitacao.Quantidade.ToString("n2"),
                        PeriodoEmbarque = "De " + solicitacaoLicitacao.DataInicioEmbarque.ToString("dd/MM/yyyy") + " até " + solicitacaoLicitacao.DataFimEmbarque.ToString("dd/MM/yyyy"),
                        solicitacaoLicitacao.Observacao,
                        UsuarioCotacao = solicitacaoLicitacao.UsuarioCotacao?.Nome,


                        //Campos
                        Empresa = solicitacaoLicitacao.Empresa != null ? new { solicitacaoLicitacao.Empresa.Codigo, solicitacaoLicitacao.Empresa.Descricao } : null,
                        ModeloVeicularCarga = solicitacaoLicitacao.ModeloVeicularCarga != null ? new { solicitacaoLicitacao.ModeloVeicularCarga.Codigo, solicitacaoLicitacao.ModeloVeicularCarga.Descricao } : null,
                        ValorTrecho = solicitacaoLicitacao.ValorTrecho > 0 ? solicitacaoLicitacao.ValorTrecho.ToString("n2") : "",
                        ValorTonelada = solicitacaoLicitacao.ValorTonelada > 0 ? solicitacaoLicitacao.ValorTonelada.ToString("n2") : "",
                        ValorPedagio = solicitacaoLicitacao.ValorPedagio > 0 ? solicitacaoLicitacao.ValorPedagio.ToString("n2") : "",
                        ValorTotalNotaFiscal = solicitacaoLicitacao.ValorTotalNotaFiscal > 0 ? solicitacaoLicitacao.ValorTotalNotaFiscal.ToString("n2") : "",
                        solicitacaoLicitacao.TipoCotacao
                    }
                };

                return new JsonpResult(dynSolicitacaoLicitacao);
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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao = repSolicitacaoLicitacao.BuscarPorCodigo(codigo, false);

                if (solicitacaoLicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacaoLicitacao.Situacao != SituacaoSolicitacaoLicitacao.AgCotacao)
                    return new JsonpResult(false, true, "Não é possível finalizar nessa situação.");

                if (solicitacaoLicitacao.Usuario.Codigo == Usuario.Codigo)
                    return new JsonpResult(false, true, "Você criou a solicitação, não é permitido finalizar a mesma.");

                int codigoEmpresa = Request.GetIntParam("Empresa");
                int codigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga");

                solicitacaoLicitacao.Situacao = SituacaoSolicitacaoLicitacao.Finalizada;
                solicitacaoLicitacao.UsuarioCotacao = Usuario;
                solicitacaoLicitacao.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                solicitacaoLicitacao.ModeloVeicularCarga = codigoModeloVeicularCarga > 0 ? repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga) : null;
                solicitacaoLicitacao.ValorTrecho = Request.GetDecimalParam("ValorTrecho");
                solicitacaoLicitacao.ValorTonelada = Request.GetDecimalParam("ValorTonelada");
                solicitacaoLicitacao.ValorPedagio = Request.GetDecimalParam("ValorPedagio");
                solicitacaoLicitacao.ValorTotalNotaFiscal = Request.GetDecimalParam("ValorTotalNotaFiscal");
                solicitacaoLicitacao.TipoCotacao = Request.GetEnumParam<TipoCotacaoSolicitacaoLicitacao>("TipoCotacao");

                repSolicitacaoLicitacao.Atualizar(solicitacaoLicitacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoLicitacao, null, "Finalizou a solicitação de licitação", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Rejeitar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao = repSolicitacaoLicitacao.BuscarPorCodigo(codigo, false);

                if (solicitacaoLicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacaoLicitacao.Situacao != SituacaoSolicitacaoLicitacao.AgCotacao)
                    return new JsonpResult(false, true, "Não é possível rejeitar nessa situação.");

                solicitacaoLicitacao.Situacao = SituacaoSolicitacaoLicitacao.Rejeitada;
                solicitacaoLicitacao.UsuarioCotacao = Usuario;

                repSolicitacaoLicitacao.Atualizar(solicitacaoLicitacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoLicitacao, null, "Rejeitou a solicitação de licitação", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar.");
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao = repSolicitacaoLicitacao.BuscarPorCodigo(codigo, false);

                if (solicitacaoLicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacaoLicitacao.Situacao != SituacaoSolicitacaoLicitacao.AgCotacao)
                    return new JsonpResult(false, true, "Não é possível cancelar nessa situação.");

                solicitacaoLicitacao.Situacao = SituacaoSolicitacaoLicitacao.Cancelada;

                repSolicitacaoLicitacao.Atualizar(solicitacaoLicitacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoLicitacao, null, "Cancelou a solicitação de licitação", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherSolicitacaoLicitacao(Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.UnidadeDeMedida repUnidadeDeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            dynamic dados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("SolicitacaoLicitacao"));

            solicitacaoLicitacao.Numero = repSolicitacaoLicitacao.BuscarProximoNumero();
            solicitacaoLicitacao.Data = DateTime.Now;
            solicitacaoLicitacao.Usuario = Usuario;
            solicitacaoLicitacao.Situacao = SituacaoSolicitacaoLicitacao.AgCotacao;

            int localidadeOrigem = ((string)dados.LocalidadeOrigem).ToInt();
            int localidadeDestino = ((string)dados.LocalidadeDestino).ToInt(); 
            int unidadeMedida = ((string)dados.UnidadeMedida).ToInt(); 
            double clienteOrigem = ((string)dados.ClienteOrigem).ToDouble();
            double clienteDestino = ((string)dados.ClienteDestino).ToDouble();

            solicitacaoLicitacao.ClienteOrigem = clienteOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(clienteOrigem) : null;
            solicitacaoLicitacao.ClienteDestino = clienteDestino > 0 ? repCliente.BuscarPorCPFCNPJ(clienteDestino) : null;
            solicitacaoLicitacao.LocalidadeOrigem = localidadeOrigem > 0 ? repLocalidade.BuscarPorCodigo(localidadeOrigem) : null;
            solicitacaoLicitacao.LocalidadeDestino = localidadeDestino > 0 ? repLocalidade.BuscarPorCodigo(localidadeDestino) : null;
            solicitacaoLicitacao.UnidadeMedida = repUnidadeDeMedida.BuscarPorCodigo(unidadeMedida);

            int codigoTipoDeCarga = ((string)dados.TipoDeCarga).ToInt();

            solicitacaoLicitacao.TipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);
            solicitacaoLicitacao.Observacao = ((string)dados.Observacao).ToString();
            solicitacaoLicitacao.Quantidade = ((string)dados.Quantidade).ToDecimal();
            solicitacaoLicitacao.DataInicioEmbarque = ((string)dados.DataInicioEmbarque).ToDateTime();
            solicitacaoLicitacao.DataFimEmbarque = ((string)dados.DataFimEmbarque).ToDateTime();
            solicitacaoLicitacao.DataPrazoResposta = ((string)dados.DataPrazoResposta).ToDateTime();
            solicitacaoLicitacao.Comprimento = ((string)dados.Comprimento).ToDecimal();
            solicitacaoLicitacao.Altura = ((string)dados.Altura).ToDecimal();
            solicitacaoLicitacao.Largura = ((string)dados.Largura).ToDecimal();
            solicitacaoLicitacao.DescricaoCotacao = ((string)dados.DescricaoCotacao).ToString();
        }

        private void AdicionarProdutos(Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao solicitacaoLicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.SolicitacaoLicitacaoProduto repositorioSolicitacaoLicitacaoProduto = new Repositorio.Embarcador.Frete.SolicitacaoLicitacaoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            dynamic dynProduto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

            List<int> codigosProdutos = new List<int>();

            foreach(dynamic produto in dynProduto)
                codigosProdutos.Add(((string)produto.Codigo).ToInt());

            if (codigosProdutos.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = repositorioProdutoEmbarcador.BuscarPorCodigo(codigosProdutos.ToArray());

            foreach(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto in produtos)
            {
                Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto solicitacaoLicitacaoProduto = new Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto()
                {
                    ProdutoEmbarcador = produto,
                    SolicitacaoLicitacao = solicitacaoLicitacao
                };

                repositorioSolicitacaoLicitacaoProduto.Inserir(solicitacaoLicitacaoProduto);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaSolicitacaoLicitacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaSolicitacaoLicitacao()
            {
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                Situacao = Request.GetEnumParam<SituacaoSolicitacaoLicitacao>("Situacao"),
                CodigoFuncionarioSolicitante = Request.GetIntParam("FuncionarioSolicitante"),
                CodigoFuncionarioCotacao = Request.GetIntParam("FuncionarioCotacao"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                Numero = Request.GetIntParam("Numero")
            };
        }

        #endregion
    }
}
