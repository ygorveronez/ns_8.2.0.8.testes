using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "PagamentosMotoristas/AutorizacaoPagamentoMotorista")]
    public class AutorizacaoPagamentoMotoristaController : BaseController
    {
		#region Construtores

		public AutorizacaoPagamentoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaPagamentos = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaPagamentos, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaPagamentos, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistro);

                // Retorna Dados
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaPagamentos = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaPagamentos, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaPagamentos, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorio
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Usuario"), out int usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("MotivoRejeicao", false);

                // Buscas regras do usuario para essa ocorrencia
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> regras = repPagamentoMotoristaAutorizacao.BuscarPorPagamentoUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = from ocorrenciaAutorizacao in regras
                            select new
                            {
                                ocorrenciaAutorizacao.Codigo,
                                Regra = TituloRegra(ocorrenciaAutorizacao),
                                MotivoRejeicao = ocorrenciaAutorizacao.Motivo,
                                Observacao = ocorrenciaAutorizacao.PagamentoMotoristaTMS.Observacao,
                                Situacao = ocorrenciaAutorizacao.DescricaoSituacao,
                                Usuario = ocorrenciaAutorizacao.Usuario.Nome,
                                Etapa = ocorrenciaAutorizacao.DescricaoEtapaAutorizacaoOcorrencia,
                                // Verifica se o usuario ja motificou essa autorizacao
                                PodeAprovar = repPagamentoMotoristaAutorizacao.VerificarSePodeAprovar(codigo, ocorrenciaAutorizacao.Codigo, this.Usuario.Codigo),
                                // Busca a cor de acordo com a situacao da autorizacao
                                DT_RowColor = this.CoresRegras(ocorrenciaAutorizacao)
                            };

                // Retorna Grid
                grid.setarQuantidadeTotal(regras.Count());
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
            // Busca a ocorrencia
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo repPagamentoMotoristaTMSAnexo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo> anexos = repPagamentoMotoristaTMSAnexo.BuscarPorPagamentoMotorista(pagamentoMotorista.Codigo);

                if (pagamentoMotorista == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynOcorrencia = new
                {
                    pagamentoMotorista.Codigo,
                    ValorPagamentoMotorista = pagamentoMotorista.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                    NumeroPagamentoMotorista = pagamentoMotorista.Numero.ToString("n0"),
                    DataPagamentoMotorista = pagamentoMotorista.DataPagamento.ToString("dd/MM/yyyy"),

                    Situacao = pagamentoMotorista.DescricaoSituacao,
                    CodigoCarga = pagamentoMotorista.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    TipoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.Descricao ?? string.Empty,

                    Motorista = pagamentoMotorista.Motorista?.Nome ?? string.Empty,
                    Solicitante = pagamentoMotorista.Usuario?.Nome ?? string.Empty,

                    pagamentoMotorista.Observacao,
                    MotivoCancelamento = string.Empty,

                    EnumSituacao = pagamentoMotorista.SituacaoPagamentoMotorista,

                    PermiteSelecionarTomador = false,

                    Tomador = 0,

                    Anexos = (
                        from anexo in anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                };

                return new JsonpResult(dynOcorrencia);
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

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            /* Busca todas as regras da ocorrencia
             * Aprova todas as regras
             * Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoOcorrencia);

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> pagamentosAutorizacoes = repPagamentoMotoristaAutorizacao.BuscarPendentesPorPagamentoEUsuario(codigoOcorrencia, this.Usuario.Codigo);

                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < pagamentosAutorizacoes.Count(); i++)
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentosAutorizacoes[i], this.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                string msgRetorno = "";
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(repPagamentoMotorista.BuscarPorCodigo(codigoOcorrencia), unitOfWork, ref msgRetorno, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, this.Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = pagamentosAutorizacoes.Count(),
                    MensagemRetorno = msgRetorno
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os pagamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPorCodigo(codigo);

                if (pagamentoMotoristaAutorizacao == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar os dados.");

                if (pagamentoMotoristaAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, true, "A situação da aprovação não permite alterações da mesma.");

                unitOfWork.Start();

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, this.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                string msgRetorno = "";
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, this.Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { MensagemRetorno = msgRetorno });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
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
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                string motivo = Request.GetStringParam("Motivo");

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPorCodigo(codigo);

                if (pagamentoMotoristaAutorizacao == null || pagamentoMotoristaAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar os dados.");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, "Motivo é obrigatório.");

                if (pagamentoMotoristaAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, true, "A situação da aprovação não permite alterações da mesma.");

                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                pagamentoMotoristaAutorizacao.Data = DateTime.Now;
                pagamentoMotoristaAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada;
                pagamentoMotoristaAutorizacao.Motivo = motivo;

                repPagamentoMotoristaAutorizacao.Atualizar(pagamentoMotoristaAutorizacao);

                string msgRetorno = "";
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.NotificarAlteracao(false, pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, this.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, this.Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { MensagemRetorno = msgRetorno });
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

        public async Task<IActionResult> ReprovarMultiplasPagamentoMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> pagamentos = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

                int codigoJustificativa = Request.GetIntParam("Justificativa");

                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, "Motivo é obrigatório.");

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    pagamentos = ObterPagamentosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> pagamentosAutorizacoes = BuscarRegrasPorpagamentos(pagamentos, this.Usuario.Codigo, unitOfWork);

                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosOcorrenciasVerificados = new List<int>();

                // Aprova todas as regras
                string mensagemRetorno = string.Empty;
                for (int i = 0; i < pagamentosAutorizacoes.Count(); i++)
                {
                    unitOfWork.Start();
                    try
                    {
                        int codigo = pagamentosAutorizacoes[i].PagamentoMotoristaTMS.Codigo;

                        // Metodo de rejeitar avaria
                        pagamentosAutorizacoes[i].Data = DateTime.Now;
                        pagamentosAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada;
                        pagamentosAutorizacoes[i].Motivo = motivo;

                        repPagamentoMotoristaAutorizacao.Atualizar(pagamentosAutorizacoes[i]);

                        string msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(repPagamentoMotorista.BuscarPorCodigo(codigo), unitOfWork, ref msgRetorno, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, this.Usuario);

                        if (!string.IsNullOrWhiteSpace(msgRetorno))
                            mensagemRetorno += msgRetorno;

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                    }
                }

                return new JsonpResult(new
                {
                    RegrasModificadas = pagamentosAutorizacoes.Count(),
                    MensagemRetorno = mensagemRetorno
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar os pagamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasPagamentoMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> pagamentos = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    pagamentos = ObterPagamentosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> pagamentoMotoristaAutorizacao = BuscarRegrasPorpagamentos(pagamentos, this.Usuario.Codigo, unitOfWork);

                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosPagamentosVerificados = new List<int>();

                // Aprova todas as regras
                string mensagemRetorno = string.Empty;
                for (int i = 0; i < pagamentoMotoristaAutorizacao.Count(); i++)
                {
                    unitOfWork.Start();
                    try
                    {
                        int codigo = pagamentoMotoristaAutorizacao[i].PagamentoMotoristaTMS.Codigo;

                        // Metodo de aprovar a ocorrencia
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao[i], this.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                        string msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(repPagamentoMotorista.BuscarPorCodigo(codigo), unitOfWork, ref msgRetorno, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, this.Usuario);

                        if (!string.IsNullOrWhiteSpace(msgRetorno))
                            mensagemRetorno += msgRetorno;

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                    }
                }

                return new JsonpResult(new
                {
                    RegrasModificadas = pagamentoMotoristaAutorizacao.Count(),
                    MensagemRetorno = mensagemRetorno
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os pagamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "DataPagamento", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº Pagamento", "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo Pagamento", "PagamentoMotoristaTipo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº Carga", "CodigoCargaEmbarcador", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "CodigoCargaEmbarcador")
                propOrdena = "Carga.CodigoCargaEmbarcador";
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaPagamentos, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("NumeroPagamentoMotorista"), out int numeroPagamentoMotorista);
            int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int codigoCentroResultado = Request.GetIntParam("CentroResultado");

            string numeroCarga = Request.Params("NumeroCarga") ?? string.Empty;

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao);

            List<int> codigosTipoPagamento = JsonConvert.DeserializeObject<List<int>>(Request.Params("TipoPagamento"));

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            listaPagamentos = repPagamentoMotoristaAutorizacao.Consultar(usuario, dataInicial, dataFinal, situacao, numeroPagamentoMotorista, numeroCarga, codigosTipoPagamento, codigoMotorista, codigoCentroResultado, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repPagamentoMotoristaAutorizacao.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numeroPagamentoMotorista, numeroCarga, codigosTipoPagamento, codigoMotorista, codigoCentroResultado);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaPagamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

            var lista = from pagamento in listaPagamentos
                        select new
                        {
                            pagamento.Codigo,
                            DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                            Numero = pagamento.Numero.ToString("n0"),
                            Motorista = pagamento.Motorista?.Nome ?? string.Empty,
                            PagamentoMotoristaTipo = pagamento.PagamentoMotoristaTipo?.Descricao ?? string.Empty,
                            CodigoCargaEmbarcador = pagamento.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                            Responsavel = FormataResponsaveis(repPagamentoMotoristaAutorizacao.ResponsavelOcorrencia(pagamento.Codigo)),
                            Valor = pagamento.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                            Situacao = pagamento.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private string FormataResponsaveis(List<Dominio.Entidades.Usuario> responsaveis)
        {
            var aprovadores = (from o in responsaveis where !string.IsNullOrWhiteSpace(o.Nome) select o.Nome).ToList();

            return String.Join(", ", aprovadores);
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao regra)
        {
            return regra.RegrasPagamentoMotorista?.Descricao;
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }


        private List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> ObterPagamentosSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaPagamentoMotoristas = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaPagamentoMotoristas, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaPagamentoMotoristasNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PagamentoMotoristasNaoSelecionadas"));
                foreach (var dybPagamentoNaoSelecionada in listaPagamentoMotoristasNaoSelecionadas)
                    listaPagamentoMotoristas.Remove(new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS() { Codigo = (int)dybPagamentoNaoSelecionada.Codigo });
            }
            else
            {
                dynamic listaPagamentoMotoristasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PagamentoMotoristasSelecionadas"));
                foreach (var dynPagamentoSelecionada in listaPagamentoMotoristasSelecionadas)
                    listaPagamentoMotoristas.Add(repPagamentoMotorista.BuscarPorCodigo((int)dynPagamentoSelecionada.Codigo));
            }

            return listaPagamentoMotoristas;
        }

        private List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> BuscarRegrasPorpagamentos(List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> pagamentos, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> pagamentoMotoristaAutorizacao = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>();

            // Itera todas as ocorrencias
            foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento in pagamentos)
            {
                // Busca as autorizacoes da ocorrencias                
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> regras = repPagamentoMotoristaAutorizacao.BuscarPendentesPorPagamentoEUsuario(pagamento.Codigo, usuario);

                // Adiciona na lista
                pagamentoMotoristaAutorizacao.AddRange(regras);
            }

            // Retornas a lista com todas as autorizacao das ocorrencias
            return pagamentoMotoristaAutorizacao;
        }

        #endregion
    }
}
