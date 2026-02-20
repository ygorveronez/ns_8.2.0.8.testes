using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFreteComissaoGrupoProduto")]
    public class TabelaFreteComissaoGrupoProdutoController : BaseController
    {
		#region Construtores

		public TabelaFreteComissaoGrupoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoTabelaFrete, codigoContratoFreteTransportador, codigoGrupoProduto, codigoPessoa, codigoGrupoPessoas, codigoTransportador;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);
                int.TryParse(Request.Params("GrupoProduto"), out codigoGrupoProduto);
                int.TryParse(Request.Params("Pessoa"), out codigoPessoa);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Contrato de Frete", "ContratoFreteTransportador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Produto", "GrupoProduto", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("% Comissão", "PercentualValorProduto", 10, Models.Grid.Align.center, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Transportador")
                    propOrdenar = "ContratoFreteTransportador.Transportador.RazaoSocial";
                else if (propOrdenar == "GrupoProduto")
                    propOrdenar = "GrupoProduto.Descricao";
                else if (propOrdenar == "ContratoFreteTransportador")
                    propOrdenar = "ContratoFreteTransportador.Descricao";
                else if (propOrdenar == "GrupoPessoas")
                    propOrdenar = "GrupoPessoas.Descricao";
                else if (propOrdenar == "Pessoa")
                    propOrdenar = "Pessoa.Nome";

                Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto> listaTabelaFreteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.Consultar(codigoTabelaFrete, ativo, codigoTransportador, codigoGrupoProduto, codigoContratoFreteTransportador, codigoPessoa, codigoGrupoPessoas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaFreteComissaoGrupoProduto.ContarConsulta(codigoTabelaFrete, ativo, codigoTransportador, codigoGrupoProduto, codigoContratoFreteTransportador, codigoPessoa, codigoGrupoPessoas));

                var lista = (from obj in listaTabelaFreteComissaoGrupoProduto
                             select new
                             {
                                 obj.Codigo,
                                 ContratoFreteTransportador = obj.ContratoFreteTransportador?.Descricao ?? string.Empty,
                                 Transportador = obj.ContratoFreteTransportador?.Transportador?.RazaoSocial ?? string.Empty,
                                 GrupoProduto = obj.GrupoProduto.Descricao,
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

                int codigoTabelaFrete, codigoGrupoPessoas, codigoContratoFreteTransportador;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);

                decimal percentualValorProduto;
                decimal.TryParse(Request.Params("PercentualValorProduto"), out percentualValorProduto);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                dynamic gruposProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GruposProdutos"));

                Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Frete.LogTabelaComissaoGrupoProduto repLogTabelaComissaoGrupoProduto = new Repositorio.Embarcador.Frete.LogTabelaComissaoGrupoProduto(unitOfWork);

                unitOfWork.Start();

                foreach (dynamic grupoProduto in gruposProdutos)
                {
                    int codigoGrupoProduto = (int)grupoProduto.Codigo;

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto tabelaFreteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.Buscar(codigoTabelaFrete, codigoContratoFreteTransportador, codigoGrupoProduto, codigoGrupoPessoas, cpfCnpjPessoa);

                    if (tabelaFreteComissaoGrupoProduto == null)
                        tabelaFreteComissaoGrupoProduto = new Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto();

                    tabelaFreteComissaoGrupoProduto.ContratoFreteTransportador = repContratoFrete.BuscarPorCodigo(codigoContratoFreteTransportador);
                    tabelaFreteComissaoGrupoProduto.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                    tabelaFreteComissaoGrupoProduto.Pessoa = cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                    tabelaFreteComissaoGrupoProduto.TabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
                    tabelaFreteComissaoGrupoProduto.GrupoProduto = repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto);
                    tabelaFreteComissaoGrupoProduto.PercentualValorProduto = percentualValorProduto;
                    tabelaFreteComissaoGrupoProduto.Ativo = ativo;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog tipoLog;

                    if (tabelaFreteComissaoGrupoProduto.Codigo > 0)
                    {
                        repTabelaFreteComissaoGrupoProduto.Atualizar(tabelaFreteComissaoGrupoProduto);
                        tipoLog = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.update;
                    }
                    else
                    {
                        repTabelaFreteComissaoGrupoProduto.Inserir(tabelaFreteComissaoGrupoProduto);
                        tipoLog = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.insert;
                    }

                    Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto logTabelaComissaoGrupoProduto = new Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto();
                    logTabelaComissaoGrupoProduto.SetarLog(tabelaFreteComissaoGrupoProduto, tipoLog, this.Usuario);
                    repLogTabelaComissaoGrupoProduto.Inserir(logTabelaComissaoGrupoProduto);
                }

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

        //public async Task<IActionResult> Atualizar()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {

        //        bool ativo;
        //        bool.TryParse(Request.Params("Ativo"), out ativo);

        //        int codigo, codigoTabelaFrete, codigoGrupoPessoas, codigoGrupoProduto, codigoContratoFreteTransportador;
        //        int.TryParse(Request.Params("Codigo"), out codigo);
        //        int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
        //        int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
        //        int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);

        //        decimal percentualValorProduto;
        //        decimal.TryParse(Request.Params("PercentualValorProduto"), out percentualValorProduto);

        //        double cpfCnpjPessoa;
        //        double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

        //        Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unitOfWork);
        //        Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
        //        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
        //        Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
        //        Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
        //        Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
        //        Repositorio.Embarcador.Frete.LogTabelaComissaoGrupoProduto repLogTabelaComissaoGrupoProduto = new Repositorio.Embarcador.Frete.LogTabelaComissaoGrupoProduto(unitOfWork);

        //        unitOfWork.Start();

        //        Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto tabelaFreteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorCodigo(codigo);

        //        tabelaFreteComissaoGrupoProduto.ContratoFreteTransportador = repContratoFrete.BuscarPorCodigo(codigoContratoFreteTransportador);
        //        tabelaFreteComissaoGrupoProduto.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
        //        tabelaFreteComissaoGrupoProduto.Pessoa = cpfCnpjPessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
        //        tabelaFreteComissaoGrupoProduto.TabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
        //        tabelaFreteComissaoGrupoProduto.GrupoProduto = repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto);
        //        tabelaFreteComissaoGrupoProduto.PercentualValorProduto = percentualValorProduto;
        //        tabelaFreteComissaoGrupoProduto.Ativo = ativo;

        //        if (repTabelaFreteComissaoGrupoProduto.VerificarSeExiste(codigo, codigoTabelaFrete, codigoContratoFreteTransportador, codigoGrupoProduto, codigoGrupoPessoas, cpfCnpjPessoa))
        //        {
        //            unitOfWork.Rollback();
        //            return new JsonpResult(false, true, "Já existe um frete cadastrado para esse contrato de frete, pessoa/grupo de pessoas e grupo de produto.");
        //        }

        //        repTabelaFreteComissaoGrupoProduto.Atualizar(tabelaFreteComissaoGrupoProduto);

        //        Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto logTabelaComissaoGrupoProduto = new Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto();
        //        logTabelaComissaoGrupoProduto.SetarLog(tabelaFreteComissaoGrupoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.update, this.Usuario);

        //        repLogTabelaComissaoGrupoProduto.Inserir(logTabelaComissaoGrupoProduto);

        //        unitOfWork.CommitChanges();

        //        return new JsonpResult(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        unitOfWork.Rollback();
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        //public async Task<IActionResult> BuscarPorCodigo()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        int codigo;
        //        int.TryParse(Request.Params("Codigo"), out codigo);

        //        Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unitOfWork);

        //        Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto tabelaFreteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorCodigo(codigo);

        //        var entidade = new
        //        {
        //            tabelaFreteComissaoGrupoProduto.Codigo,
        //            tabelaFreteComissaoGrupoProduto.Ativo,
        //            ContratoFreteTransportador = new
        //            {
        //                Codigo = tabelaFreteComissaoGrupoProduto.ContratoFreteTransportador?.Codigo ?? 0,
        //                Descricao = tabelaFreteComissaoGrupoProduto.ContratoFreteTransportador?.Descricao ?? string.Empty
        //            },
        //            Pessoa = new
        //            {
        //                Codigo = tabelaFreteComissaoGrupoProduto.Pessoa?.CPF_CNPJ_SemFormato ?? "0",
        //                Descricao = tabelaFreteComissaoGrupoProduto.Pessoa?.Nome ?? string.Empty
        //            },
        //            GrupoPessoas = new
        //            {
        //                Codigo = tabelaFreteComissaoGrupoProduto.GrupoPessoas?.Codigo ?? 0,
        //                Descricao = tabelaFreteComissaoGrupoProduto.GrupoPessoas?.Descricao ?? string.Empty
        //            },
        //            GrupoProduto = new
        //            {
        //                tabelaFreteComissaoGrupoProduto.GrupoProduto.Codigo,
        //                Descricao = tabelaFreteComissaoGrupoProduto.GrupoProduto.Descricao
        //            },
        //            tabelaFreteComissaoGrupoProduto.PercentualValorProduto
        //        };

        //        return new JsonpResult(entidade);
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }

        //}

        //public async Task<IActionResult> ExcluirPorCodigo()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        int codigo;
        //        int.TryParse(Request.Params("Codigo"), out codigo);

        //        unitOfWork.Start();

        //        Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unitOfWork);
        //        Repositorio.Embarcador.Frete.LogTabelaComissaoGrupoProduto repLogTabelaComissaoGrupoProduto = new Repositorio.Embarcador.Frete.LogTabelaComissaoGrupoProduto(unitOfWork);

        //        Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto tabelaFreteComissaoGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorCodigo(codigo);
        //        Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto logTabelaComissaoGrupoProduto = new Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto();
        //        logTabelaComissaoGrupoProduto.SetarLog(tabelaFreteComissaoGrupoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog.delete, this.Usuario);

        //        repTabelaFreteComissaoGrupoProduto.Deletar(tabelaFreteComissaoGrupoProduto);
        //        repLogTabelaComissaoGrupoProduto.Inserir(logTabelaComissaoGrupoProduto);

        //        unitOfWork.CommitChanges();

        //        return new JsonpResult(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        unitOfWork.Rollback();
        //        if (ExcessaoPorPossuirDependeciasNoBanco(ex))
        //            return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
        //        else
        //        {
        //            Servicos.Log.TratarErro(ex);
        //            return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
        //        }
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        #endregion
    }
}
