using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFreteComissaoProduto")]
    public class TabelaFreteComissaoProdutoController : BaseController
    {
		#region Construtores

		public TabelaFreteComissaoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoTabelaFrete, codigoContratoFreteTransportador, codigoProduto, codigoPessoa, codigoGrupoPessoas, codigoTransportador;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProduto);
                int.TryParse(Request.Params("Pessoa"), out codigoPessoa);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Contrato de Frete", "ContratoFreteTransportador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Produto", "ProdutoEmbarcador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("% Comissão", "PercentualValorProduto", 10, Models.Grid.Align.center, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Transportador")
                    propOrdenar = "ContratoFreteTransportador.Transportador.RazaoSocial";
                else if (propOrdenar == "ProdutoEmbarcador")
                    propOrdenar = "ProdutoEmbarcador.Descricao";
                else if (propOrdenar == "ContratoFreteTransportador")
                    propOrdenar = "ContratoFreteTransportador.Descricao";
                else if (propOrdenar == "GrupoPessoas")
                    propOrdenar = "GrupoPessoas.Descricao";
                else if (propOrdenar == "Pessoa")
                    propOrdenar = "Pessoa.Nome";

                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto> listaTabelaFreteComissaoProduto = repTabelaFreteComissaoProduto.Consultar(codigoTabelaFrete, ativo, codigoTransportador, codigoProduto, codigoContratoFreteTransportador, codigoPessoa, codigoGrupoPessoas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaFreteComissaoProduto.ContarConsulta(codigoTabelaFrete, ativo, codigoTransportador, codigoProduto, codigoContratoFreteTransportador, codigoPessoa, codigoGrupoPessoas));

                var lista = (from obj in listaTabelaFreteComissaoProduto
                            select new
                            {
                                obj.Codigo,
                                ContratoFreteTransportador = obj.ContratoFreteTransportador?.Descricao ?? string.Empty,
                                Transportador = obj.ContratoFreteTransportador?.Transportador?.RazaoSocial ?? string.Empty,
                                ProdutoEmbarcador = obj.ProdutoEmbarcador.Descricao,
                                Pessoa = obj.Pessoa?.Nome ?? string.Empty,
                                GrupoPessoas = obj.GrupoPessoas?.Descricao ?? string.Empty,
                                PercentualValorProduto = obj.PercentualValorProduto.ToString("n2") + "%",
                                DescricaoAtivo = obj.DescricaoAtivo
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
                unidadeTrabalho.Dispose();
            }
        }


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                int codigoTabelaFrete, codigoGrupoPessoas, codigoProduto, codigoContratoFreteTransportador;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProduto);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);

                decimal percentualValorProduto;
                decimal.TryParse(Request.Params("PercentualValorProduto"), out percentualValorProduto);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Frete.LogTabelaComissaoProduto repLogTabelaComissaoProduto = new Repositorio.Embarcador.Frete.LogTabelaComissaoProduto(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto tabelaFreteComissaoProduto = new Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto();

                tabelaFreteComissaoProduto.ContratoFreteTransportador = repContratoFrete.BuscarPorCodigo(codigoContratoFreteTransportador);
                tabelaFreteComissaoProduto.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                tabelaFreteComissaoProduto.Pessoa = cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                tabelaFreteComissaoProduto.TabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
                tabelaFreteComissaoProduto.ProdutoEmbarcador = repProduto.BuscarPorCodigo(codigoProduto);
                tabelaFreteComissaoProduto.PercentualValorProduto = percentualValorProduto;
                tabelaFreteComissaoProduto.Ativo = ativo;
                
                if (repTabelaFreteComissaoProduto.VerificarSeExiste(0, codigoTabelaFrete, codigoContratoFreteTransportador, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe um frete cadastrado para esse contrato de frete, pessoa/grupo de pessoas e produto");
                }

                repTabelaFreteComissaoProduto.Inserir(tabelaFreteComissaoProduto);

                Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto logTabelaComissaoProduto = new Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto();
                logTabelaComissaoProduto.SetarLog(tabelaFreteComissaoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.insert, this.Usuario);
                repLogTabelaComissaoProduto.Inserir(logTabelaComissaoProduto);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                int codigo, codigoTabelaFrete, codigoGrupoPessoas, codigoProduto, codigoContratoFreteTransportador;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProduto);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);

                decimal percentualValorProduto;
                decimal.TryParse(Request.Params("PercentualValorProduto"), out percentualValorProduto);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Frete.LogTabelaComissaoProduto repLogTabelaComissaoProduto = new Repositorio.Embarcador.Frete.LogTabelaComissaoProduto(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto tabelaFreteComissaoProduto = repTabelaFreteComissaoProduto.BuscarPorCodigo(codigo);

                tabelaFreteComissaoProduto.ContratoFreteTransportador = repContratoFrete.BuscarPorCodigo(codigoContratoFreteTransportador);
                tabelaFreteComissaoProduto.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                tabelaFreteComissaoProduto.Pessoa = cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                tabelaFreteComissaoProduto.TabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
                tabelaFreteComissaoProduto.ProdutoEmbarcador = repProduto.BuscarPorCodigo(codigoProduto);
                tabelaFreteComissaoProduto.PercentualValorProduto = percentualValorProduto;
                tabelaFreteComissaoProduto.Ativo = ativo;

                if (repTabelaFreteComissaoProduto.VerificarSeExiste(codigo, codigoTabelaFrete, codigoContratoFreteTransportador, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe um frete cadastrado para esse contrato de frete, pessoa/grupo de pessoas e produto");
                }

                repTabelaFreteComissaoProduto.Atualizar(tabelaFreteComissaoProduto);

                Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto logTabelaComissaoProduto = new Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto();
                logTabelaComissaoProduto.SetarLog(tabelaFreteComissaoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.update, this.Usuario);

                repLogTabelaComissaoProduto.Inserir(logTabelaComissaoProduto);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto tabelaFreteComissaoProduto = repTabelaFreteComissaoProduto.BuscarPorCodigo(codigo);

                var entidade = new
                {
                    tabelaFreteComissaoProduto.Codigo,
                    tabelaFreteComissaoProduto.Ativo,
                    ContratoFreteTransportador = new
                    {
                        Codigo = tabelaFreteComissaoProduto.ContratoFreteTransportador?.Codigo ?? 0,
                        Descricao = tabelaFreteComissaoProduto.ContratoFreteTransportador?.Descricao ?? string.Empty
                    },
                    Pessoa = new
                    {
                        Codigo = tabelaFreteComissaoProduto.Pessoa?.CPF_CNPJ_SemFormato ?? "0",
                        Descricao = tabelaFreteComissaoProduto.Pessoa?.Nome ?? string.Empty
                    },
                    GrupoPessoas = new
                    {
                        Codigo = tabelaFreteComissaoProduto.GrupoPessoas?.Codigo ?? 0,
                        Descricao = tabelaFreteComissaoProduto.GrupoPessoas?.Descricao ?? string.Empty
                    },
                    ProdutoEmbarcador = new
                    {
                        tabelaFreteComissaoProduto.ProdutoEmbarcador.Codigo,
                        Descricao = tabelaFreteComissaoProduto.ProdutoEmbarcador.Descricao
                    },
                    tabelaFreteComissaoProduto.PercentualValorProduto
                };

                return new JsonpResult(entidade);
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.LogTabelaComissaoProduto repLogTabelaComissaoProduto = new Repositorio.Embarcador.Frete.LogTabelaComissaoProduto(unitOfWork);
                
                Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto tabelaFreteComissaoProduto = repTabelaFreteComissaoProduto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto logTabelaComissaoProduto = new Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto();
                logTabelaComissaoProduto.SetarLog(tabelaFreteComissaoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.delete, this.Usuario);

                repTabelaFreteComissaoProduto.Deletar(tabelaFreteComissaoProduto);
                repLogTabelaComissaoProduto.Inserir(logTabelaComissaoProduto);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        #endregion

    }
}
