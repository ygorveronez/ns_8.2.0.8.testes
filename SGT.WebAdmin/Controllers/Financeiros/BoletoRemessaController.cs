using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BoletoRemessa")]
    public class BoletoRemessaController : BaseController
    {
		#region Construtores

		public BoletoRemessaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("BoletoConfiguracao"), out int boletoConfiguracao);
                int.TryParse(Request.Params("NumeroSequencial"), out int numeroSequencial);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                DownloadRealizado statusDownload = DownloadRealizado.Todos;
                Enum.TryParse(Request.Params("DownloadRealizado"), out statusDownload);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Sequencial", "NumeroSequencial", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Geração", "DataGeracao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Configuração Boleto (Banco)", "BoletoConfiguracao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descricao", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa> listaBoletoRemessa = repBoletoRemessa.Consultar(codigoEmpresa, numeroSequencial, boletoConfiguracao, statusDownload, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBoletoRemessa.ContarConsulta(codigoEmpresa, numeroSequencial, boletoConfiguracao, statusDownload));
                var lista = (from p in listaBoletoRemessa
                             select new
                             {
                                 p.Codigo,
                                 NumeroSequencial = p.NumeroSequencial.ToString("n0"),
                                 DataGeracao = p.DataGeracao.ToString("dd/MM/yyyy HH:mm"),
                                 BoletoConfiguracao = p.BoletoConfiguracao?.BoletoBanco.ObterDescricao() ?? string.Empty,
                                 Descricao = p.Descricao + " - " + p.BoletoConfiguracao?.BoletoBanco.ObterDescricao() ?? string.Empty
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa = repBoletoRemessa.BuscarPorCodigo(codigo);
                var dynTipoMovimento = new
                {
                    boletoRemessa.Codigo,
                    boletoRemessa.NumeroSequencial,
                    DataGeracao = boletoRemessa.DataGeracao.ToString("dd/MM/yyyy HH:mm"),
                    BoletoConfiguracao = new { Codigo = boletoRemessa.BoletoConfiguracao != null ? boletoRemessa.BoletoConfiguracao.Codigo : 0, Descricao = boletoRemessa.BoletoConfiguracao != null ? boletoRemessa.BoletoConfiguracao.BoletoBanco.ObterDescricao() : "" },
                    CaminhoArquivo = boletoRemessa.Observacao,
                    boletoRemessa.RemessaDeCancelamento
                };
                return new JsonpResult(dynTipoMovimento);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa = repBoletoRemessa.BuscarPorCodigo(codigo);

                if (boletoRemessa == null)
                    return new JsonpResult(false, "Remessa não encontrada!");

                if (boletoRemessa.RemessaDeCancelamento)
                    return new JsonpResult(false, true, "Não é permitido excluir Remessa de Cancelamento!");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.BuscarPorBoletoRemessa(boletoRemessa.Codigo);
                for (int i = 0; i < listaTitulo.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                    titulo.BoletoStatusTitulo = BoletoStatusTitulo.Gerado;
                    titulo.Historico += " - BOLETO REMOVIDO DA REMESSA POR " + this.Usuario.Nome + " EM " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    if (titulo.BoletoRemessa != null)
                        titulo.Historico += " REMESSA ANTERIOR: " + titulo.BoletoRemessa.NumeroSequencial.ToString();
                    titulo.BoletoRemessa = null;

                    repTitulo.Atualizar(titulo);
                }

                repBoletoRemessa.Deletar(boletoRemessa, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoRemessa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRemessa);

                if (codigoRemessa > 0)
                {
                    Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                    Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa = repBoletoRemessa.BuscarPorCodigo(codigoRemessa);

                    if (boletoRemessa != null && !string.IsNullOrWhiteSpace(boletoRemessa.Observacao))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(boletoRemessa.Observacao))
                            {
                                boletoRemessa.DownloadRealizado = true;
                                repBoletoRemessa.Atualizar(boletoRemessa);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoRemessa, null, "Realizou o download da Remessa.", unitOfWork);
                                unitOfWork.CommitChanges();

                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(boletoRemessa.Observacao), "application/x-pkcs12", System.IO.Path.GetFileName(boletoRemessa.Observacao));
                            }
                            else
                                return new JsonpResult(false, "O arquivo da remessa " + boletoRemessa.Observacao + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do txt do remessa.");
                        }
                    }
                    else
                        return new JsonpResult(false, true, "Esta remessa não possui o txt disponível para download.");
                }
                return new JsonpResult(false, true, "Remessa não encontrada");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LimparDadosBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa = repBoletoRemessa.BuscarPorCodigo(codigo);

                if (boletoRemessa == null)
                    return new JsonpResult(false, "Remessa não encontrada!");

                if (boletoRemessa.RemessaDeCancelamento)
                    return new JsonpResult(false, true, "Não é possível limpar dados de Remessa de Cancelamento!");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.BuscarPorBoletoRemessa(boletoRemessa.Codigo);

                Servicos.Embarcador.Financeiro.BoletoRemessa svcBoletoRemessa = new Servicos.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                if (!svcBoletoRemessa.GerarRemessaDeCancelamento(listaTitulo, unitOfWork, Auditado, this.Usuario.Empresa))
                    return new JsonpResult(false, true, "Todos os boletos da remessa já foram limpados!");

                Servicos.Embarcador.Financeiro.Titulo.LimparDadosBoleto(listaTitulo, this.Usuario, unitOfWork, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao limpar dados de boletos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
