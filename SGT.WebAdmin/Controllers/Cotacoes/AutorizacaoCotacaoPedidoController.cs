using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cotacoes
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Cotacoes/AutorizacaoCotacaoPedido")]
    public class AutorizacaoCotacaoPedidoController : BaseController
    {
		#region Construtores

		public AutorizacaoCotacaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº Cotação", "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo Carga", "TipoDeCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

            return grid;
        }

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

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> listaCotacaoPedidos = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaCotacaoPedidos, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaCotacaoPedidos, unitOfWork);

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

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> listaCotacaoPedidos = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaCotacaoPedidos, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaCotacaoPedidos, unitOfWork);

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
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

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
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> regras = repCotacaoPedidoAutorizacao.BuscarPorPedidoUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = from ocorrenciaAutorizacao in regras
                            select new
                            {
                                ocorrenciaAutorizacao.Codigo,
                                Regra = TituloRegra(ocorrenciaAutorizacao),
                                MotivoRejeicao = ocorrenciaAutorizacao.Motivo,
                                Observacao = ocorrenciaAutorizacao.CotacaoPedido.Observacao,
                                Situacao = ocorrenciaAutorizacao.DescricaoSituacao,
                                Usuario = ocorrenciaAutorizacao.Usuario.Nome,
                                Etapa = ocorrenciaAutorizacao.DescricaoEtapaAutorizacaoOcorrencia,
                                // Verifica se o usuario ja motificou essa autorizacao
                                PodeAprovar = repCotacaoPedidoAutorizacao.VerificarSePodeAprovar(codigo, ocorrenciaAutorizacao.Codigo, this.Usuario.Codigo),
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
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repCotacaoPedido.BuscarPorCodigo(codigo);

                if (cotacaoPedido == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynOcorrencia = new
                {
                    cotacaoPedido.Codigo,
                    ValorTotalCotacao = cotacaoPedido.ValorTotalCotacao.ToString("n2"),
                    NumeroPedido = cotacaoPedido.Numero.ToString("n0"),
                    DataPedido = cotacaoPedido.Data.HasValue ? cotacaoPedido.Data.Value.ToString("dd/MM/yyyy") : string.Empty,

                    Situacao = cotacaoPedido.DescricaoSituacaoPedido,
                    TipoCarga = cotacaoPedido.TipoDeCarga?.Descricao ?? string.Empty,

                    TipoOperacao = cotacaoPedido.TipoOperacao?.Descricao ?? string.Empty,
                    Destinatario = cotacaoPedido.Destinatario?.Nome ?? string.Empty,

                    Solicitante = cotacaoPedido.Usuario?.Nome ?? string.Empty,

                    cotacaoPedido.Observacao,
                    MotivoCancelamento = string.Empty,

                    EnumSituacao = cotacaoPedido.SituacaoPedido,

                    PermiteSelecionarTomador = false,

                    Tomador = 0,
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
                // Instancia
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

                // Converte parametros
                long codigoPedido = 0;
                long.TryParse(Request.Params("Codigo"), out codigoPedido);

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> cotacaoPedidoAutorizacao = repCotacaoPedidoAutorizacao.BuscarPendentesPorPedidoEUsuario(codigoPedido, this.Usuario.Codigo);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < cotacaoPedidoAutorizacao.Count(); i++)
                {
                    EfetuarAprovacao(cotacaoPedidoAutorizacao[i], unitOfWork);
                    if ((cotacaoPedidoAutorizacao[i].Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada) && (cotacaoPedidoAutorizacao[i].CotacaoPedido.TipoClienteCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteNovo || cotacaoPedidoAutorizacao[i].CotacaoPedido.TipoClienteCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteProspect))
                        return new JsonpResult(false, "Não é possível aprovar uma cotação sem antes cadastrar o cliente.");
                }

                // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(repCotacaoPedido.BuscarPorCodigo(codigoPedido), unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = cotacaoPedidoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as cotações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao cotacaoPedidoAutorizacao = repCotacaoPedidoAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (cotacaoPedidoAutorizacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (cotacaoPedidoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(cotacaoPedidoAutorizacao, unitOfWork);

                // Faz verificacao se a carga esta aprovada
                Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(cotacaoPedidoAutorizacao.CotacaoPedido, unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

                // Codigo da regra
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao cotacaoPedidoAutorizacao = repCotacaoPedidoAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (cotacaoPedidoAutorizacao == null || cotacaoPedidoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (cotacaoPedidoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                cotacaoPedidoAutorizacao.Data = DateTime.Now;
                cotacaoPedidoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada;
                cotacaoPedidoAutorizacao.Motivo = motivo;

                // Atualiza banco
                repCotacaoPedidoAutorizacao.Atualizar(cotacaoPedidoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, cotacaoPedidoAutorizacao.CotacaoPedido, unitOfWork);
                Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(cotacaoPedidoAutorizacao.CotacaoPedido, unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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

        public async Task<IActionResult> ReprovarMultiplasCotacaoPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacaoPedidos = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

                // Codigo da regra
                int codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    cotacaoPedidos = ObterCotacaoPedidosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> cotacaoPedidoAutorizacao = BuscarRegrasPorCotacaoPedidos(cotacaoPedidos, this.Usuario.Codigo, unitOfWork);


                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosOcorrenciasVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < cotacaoPedidoAutorizacao.Count(); i++)
                {

                    // Inicia transacao
                    unitOfWork.Start();
                    try
                    {
                        long codigo = cotacaoPedidoAutorizacao[i].CotacaoPedido.Codigo;

                        // Metodo de rejeitar avaria
                        cotacaoPedidoAutorizacao[i].Data = DateTime.Now;
                        cotacaoPedidoAutorizacao[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada;
                        cotacaoPedidoAutorizacao[i].Motivo = motivo;

                        // Atualiza banco
                        repCotacaoPedidoAutorizacao.Atualizar(cotacaoPedidoAutorizacao[i]);
                        Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(repCotacaoPedido.BuscarPorCodigo(codigo), unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);

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
                    RegrasModificadas = cotacaoPedidoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as cotações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasCotacaoPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacaoPedidos = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    cotacaoPedidos = ObterCotacaoPedidosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> cotacaoPedidoAutorizacao = BuscarRegrasPorCotacaoPedidos(cotacaoPedidos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao


                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosPedidosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < cotacaoPedidoAutorizacao.Count(); i++)
                {
                    unitOfWork.Start();
                    try
                    {
                        long codigo = cotacaoPedidoAutorizacao[i].CotacaoPedido.Codigo;

                        // Metodo de aprovar a ocorrencia
                        EfetuarAprovacao(cotacaoPedidoAutorizacao[i], unitOfWork);

                        if ((cotacaoPedidoAutorizacao[i].Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada) && (cotacaoPedidoAutorizacao[i].CotacaoPedido.TipoClienteCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteNovo || cotacaoPedidoAutorizacao[i].CotacaoPedido.TipoClienteCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteProspect))
                            return new JsonpResult(false, "Não é possível aprovar uma cotação sem antes cadastrar o cliente.");

                        Servicos.Embarcador.CotacaoPedido.CotacaoPedido.VerificarSituacaoCotacaoPedido(repCotacaoPedido.BuscarPorCodigo(codigo), unitOfWork, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);
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
                    RegrasModificadas = cotacaoPedidoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as cotações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> listaCotacaoPedidos, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("NumeroPedido"), out int numeroPedido);
            int.TryParse(Request.Params("TipoCarga"), out int codigoTipoCarga);
            int.TryParse(Request.Params("GrupoPessoa"), out int codigoGrupoPessoa);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("TipoOperacao"), out int codigoTipoOperacao);

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            listaCotacaoPedidos = repCotacaoPedidoAutorizacao.Consultar(usuario, dataInicial, dataFinal, situacao, numeroPedido, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repCotacaoPedidoAutorizacao.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numeroPedido, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> listaCotacaoPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

            var lista = from cotacaoPedido in listaCotacaoPedidos
                        select new
                        {
                            cotacaoPedido.Codigo,
                            Data = cotacaoPedido.Data.HasValue ? cotacaoPedido.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                            Numero = cotacaoPedido.Numero.ToString("n0"),
                            TipoDeCarga = cotacaoPedido.TipoDeCarga?.Descricao ?? string.Empty,
                            TipoOperacao = cotacaoPedido.TipoOperacao?.Descricao ?? string.Empty,
                            Destinatario = cotacaoPedido.GrupoPessoas != null ? cotacaoPedido.GrupoPessoas.Descricao : cotacaoPedido.Destinatario != null ? cotacaoPedido.Destinatario.Nome : string.Empty,
                            Valor = cotacaoPedido.ValorTotalCotacao.ToString("n2"),
                            Situacao = cotacaoPedido.DescricaoSituacaoPedido
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao regra)
        {
            return regra.RegrasCotacaoPedido?.Descricao;
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao regra)
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

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao cotacaoPedidoAutorizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (cotacaoPedidoAutorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente && cotacaoPedidoAutorizacao.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                cotacaoPedidoAutorizacao.Data = DateTime.Now;
                cotacaoPedidoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada;

                // Atualiza os dados
                repCotacaoPedidoAutorizacao.Atualizar(cotacaoPedidoAutorizacao);

                // Notifica usuario que criou a ocorrencia
                this.NotificarAlteracao(true, cotacaoPedidoAutorizacao.CotacaoPedido, unitOfWork);
            }
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                string titulo = Localization.Resources.Cotacoes.AutorizacaoCotacaoPedido.CotacaoPedido;
                string mensagem = string.Format(Localization.Resources.Cotacoes.AutorizacaoCotacaoPedido.UsuarioCotacaoPedidoValor, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), cotacaoPedido.Numero.ToString("n0"), cotacaoPedido.ValorTotalCotacao.ToString("n2"));
                serNotificacao.GerarNotificacaoEmail(cotacaoPedido.Usuario, this.Usuario, (int)cotacaoPedido.Codigo, "CotacaoPedido/CotacaoPedido", titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        

        private List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> ObterCotacaoPedidosSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> listaCotacaoPedidos = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaCotacaoPedidos, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaCotacaoPedidosNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CotacaoPedidosNaoSelecionadas"));
                foreach (var dybPedidoNaoSelecionada in listaCotacaoPedidosNaoSelecionadas)
                    listaCotacaoPedidos.Remove(new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido() { Codigo = (int)dybPedidoNaoSelecionada.Codigo });
            }
            else
            {
                dynamic listaCotacaoPedidosSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CotacaoPedidosSelecionadas"));
                foreach (var dynPedidoSelecionada in listaCotacaoPedidosSelecionadas)
                    listaCotacaoPedidos.Add(repCotacaoPedido.BuscarPorCodigo((int)dynPedidoSelecionada.Codigo));
            }

            return listaCotacaoPedidos;
        }

        private List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> BuscarRegrasPorCotacaoPedidos(List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacaoPedidos, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> cotacaoPedidoAutorizacao = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao>();

            // Itera todas as ocorrencias
            foreach (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido in cotacaoPedidos)
            {
                // Busca as autorizacoes da ocorrencias                
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao> regras = repCotacaoPedidoAutorizacao.BuscarPendentesPorPedidoEUsuario(cotacaoPedido.Codigo, usuario);

                // Adiciona na lista
                cotacaoPedidoAutorizacao.AddRange(regras);
            }

            // Retornas a lista com todas as autorizacao das ocorrencias
            return cotacaoPedidoAutorizacao;
        }

        #endregion

    }
}
