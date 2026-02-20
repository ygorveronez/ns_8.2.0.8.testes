using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ChamadoTMS", "Chamados/ControleChamadoTMS")]
    public class ChamadoTMSAnaliseController : BaseController
    {
		#region Construtores

		public ChamadoTMSAnaliseController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnalise(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise analise;
                if (codigo > 0)
                    analise = repChamadoAnalise.BuscarPorCodigo(codigo, true);
                else
                    analise = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise();

                PreencheAnalise(analise, unitOfWork);

                if (codigo > 0)
                    repChamadoAnalise.Atualizar(analise, Auditado);
                else
                    repChamadoAnalise.Inserir(analise, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Chamados.ChamadoTMSAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnalise(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise analise = repChamadoAnalise.BuscarPorCodigo(codigo, false);

                if (analise == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    analise.Codigo,
                    analise.Observacao,
                    DataAnalise = analise.DataCriacao.ToString("dd/MM/yyyy HH:mm")
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.ChamadoTMSAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnalise(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise analise = repChamadoAnalise.BuscarPorCodigo(codigo, true);

                if (analise == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (analise.Chamado.Situacao != SituacaoChamadoTMS.EmAnalise)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                repChamadoAnalise.Deletar(analise, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, analise.Chamado, null, "Excluiu a análise " + analise.Observacao + " - da Data: " + analise.DataCriacao.ToString("dd/MM/yyyy HH:mm"), unitOfWork);

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

        public async Task<IActionResult> CancelarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                chamado.Situacao = SituacaoChamadoTMS.Cancelado;
                repChamado.Atualizar(chamado, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "Chamado não possui responsável.");

                if (chamado.Responsavel.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, true, "Usuário sem permissão para esse chamado.");

                chamado.Situacao = SituacaoChamadoTMS.Finalizado;
                chamado.DataFinalizacao = DateTime.Now;

                unitOfWork.Start();
                repChamado.Atualizar(chamado, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Delegar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Usuario"), out int usuario);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Usuario usuarioDelegado = repUsuario.BuscarPorCodigo(usuario);
                chamado.Responsavel = usuarioDelegado;

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "É obrigatório informar o usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuarioDelegado.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, "Delegou o chamado para o usuário " + chamado.Responsavel.Nome, unitOfWork);

                repChamado.Atualizar(chamado);
                return new JsonpResult(chamado.Situacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvaDocumentoAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado.Situacao != SituacaoChamadoTMS.EmAnalise)
                    return new JsonpResult(false, true, "Situação do chamado não permite salvar Documento da Análise.");

                chamado.DataDocumentoRecebido = Request.GetDateTimeParam("DataDocumentoRecebido");
                chamado.ValorRecibo = Request.GetDecimalParam("ValorRecibo");
                chamado.NumeroDocumento = Request.GetStringParam("NumeroDocumento");
                chamado.ObservacaoDocumento = Request.GetStringParam("ObservacaoDocumento");

                NivelToleranciaValor nivelToleranciaValorCliente = chamado.Carga.GrupoPessoaPrincipal?.NivelToleranciaValorCliente ?? NivelToleranciaValor.NaoAceitaDivergencia;

                if ((chamado.ValorTotalDescarga < chamado.ValorRecibo && nivelToleranciaValorCliente == NivelToleranciaValor.AceitaValorMenor)
                 || (chamado.ValorTotalDescarga != chamado.ValorRecibo && nivelToleranciaValorCliente == NivelToleranciaValor.NaoAceitaDivergencia))
                    chamado.Situacao = SituacaoChamadoTMS.AguardandoAutorizacao;
                else
                    chamado.Situacao = SituacaoChamadoTMS.LiberadaOcorrencia;

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(chamado.Codigo);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar documento da análise.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvaAutorizacaoAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado.Situacao != SituacaoChamadoTMS.AguardandoAutorizacao)
                    return new JsonpResult(false, true, "Situação do chamado não permite salvar Autorização da Análise.");

                chamado.FormaAutorizacaoPagamento = Request.GetEnumParam<FormaAutorizacaoPagamentoChamado>("FormaAutorizacaoPagamento");
                chamado.NovoValorAutorizado = Request.GetDecimalParam("NovoValorAutorizado");
                chamado.JustificativaAutorizacao = Request.GetStringParam("JustificativaAutorizacao");

                if (chamado.FormaAutorizacaoPagamento == FormaAutorizacaoPagamentoChamado.PagamentoNaoAutorizado)
                    chamado.Situacao = SituacaoChamadoTMS.PagamentoNaoAutorizado;
                else
                    chamado.Situacao = SituacaoChamadoTMS.LiberadaOcorrencia;

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(chamado.Codigo);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar autorização da análise.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheAnalise(Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise analise, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);

            int codigoChamado = Request.GetIntParam("CodigoChamado");
            Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigoChamado, false);

            if (chamado.Situacao != SituacaoChamadoTMS.EmAnalise)
                throw new ControllerException("A situação não permite essa operação.");

            if (analise.Codigo == 0)
            {
                analise.Autor = this.Usuario;
                analise.DataCriacao = DateTime.Now;
                analise.Chamado = chamado;
            }

            analise.Observacao = Request.GetStringParam("Observacao");
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Autor", "Autor", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "DataCriacao", 15, Models.Grid.Align.center, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoTMSAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnalise(unitOfWork);

            int codigoChamado = Request.GetIntParam("CodigoChamado");

            List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise> listaGrid = repChamadoAnalise.Consultar(codigoChamado, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repChamadoAnalise.ContarConsulta(codigoChamado);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Autor = obj.Autor.Nome,
                            DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy"),
                            obj.Observacao
                        };

            return lista.ToList();
        }

        #endregion
    }
}
