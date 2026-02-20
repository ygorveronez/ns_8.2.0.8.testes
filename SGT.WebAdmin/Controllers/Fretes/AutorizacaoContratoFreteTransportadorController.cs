using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Fretes/AutorizacaoContratoFreteTransportador", "Fretes/ContratoFreteTransportador")]
    public class AutorizacaoContratoFreteTransportadorController : BaseController
    {
		#region Construtores

		public AutorizacaoContratoFreteTransportadorController(Conexao conexao) : base(conexao) { }

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

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> lista = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
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
                Models.Grid.Grid grid = GridPesquisa(unitOfWork, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> lista = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

                int totalRegistro = 0;

                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var listaDinamica = RetornoDynamicExportacao(lista, unitOfWork);

                grid.AdicionaRows(listaDinamica);

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

        public async Task<IActionResult> Delegar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Usuario"), out int codigoUsuario);
                int.TryParse(Request.Params("Contrato"), out int codigoContrato);

                // Entidades
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigoContrato);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Erro ao buscar dados.");

                if (contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao)
                    return new JsonpResult(false, true, "A situação do contatrato não permite essa operação.");

                if (usuario == null)
                    return new JsonpResult(false, true, "Erro ao buscar usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuario.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                unitOfWork.Start();

                EfetuarResponsabilidade(contrato, usuario, unitOfWork, out string erro);

                if (!string.IsNullOrWhiteSpace(erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar a ocorrência.");
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
                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratos = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                if (contratos == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = new
                {
                    contratos.Codigo,
                    contratos.Numero,
                    contratos.Descricao,
                    DataInicial = contratos.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = contratos.DataFinal.ToString("dd/MM/yyyy"),
                    ValorMensal = contratos.ValorMensal.ToString("N2"),
                    TipoFechamento = contratos.PeriodoAcordo.ObterDescricao(),
                    contratos.QuantidadeMensalCargas,
                    TipoCargas = String.Join(", ", contratos.TipoCargas.Select(t => t.TipoDeCarga.Descricao).ToList()),
                    CanaisEntrega = String.Join(", ", contratos.CanaisEntrega.Select(o => o.Descricao).ToList()),
                    Acordos = (
                        from obj in contratos.Acordos
                        select new
                        {
                            Codigo = obj.Codigo,
                            ModeloVeicular = new { obj.ModeloVeicular.Codigo, obj.ModeloVeicular.Descricao },
                            ValorAcordado = obj.ValorAcordado.ToString("n2"),
                        }
                    ).ToList(),
                };

                return new JsonpResult(dynDados);
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

        public async Task<IActionResult> BuscarGridAcordos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratos = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                Models.Grid.Grid grid = GridAcordos();

                string tipoPeriodo = contratos.PeriodoAcordo.ObterDescricao();

                if (tipoPeriodo.Equals("Decendial"))
                    tipoPeriodo = "Dezena";
                if (tipoPeriodo.Equals("Quinzenal"))
                    tipoPeriodo = "Quinzena";

                if (contratos == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = (
                        from obj in contratos.Acordos
                        select new
                        {
                            Codigo = obj.Codigo,
                            FranquiaPorKm = obj.FranquiaPorKm ? "Sim" : "Não",
                            ModeloVeicular = obj.ModeloVeicular.Descricao,
                            Periodo = (!tipoPeriodo.Equals("Mensal") ? (obj.Periodo + 1).ToString() + " " : "") + tipoPeriodo,
                            Quantidade = obj.Quantidade,
                            Total = obj.Total.ToString("n2"),
                            ValorAcordado = obj.ValorAcordado.ToString("n2"),
                        }
                    ).ToList();

                grid.AdicionaRows(dynDados);
                grid.setarQuantidadeTotal(dynDados.Count());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Usuario"), out int usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);

                // Instancia repositorio 
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> regras = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorContratoEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from contratoAutorizacao in regras
                             select new
                             {
                                 contratoAutorizacao.Codigo,
                                 Regra = TituloRegra(contratoAutorizacao),
                                 Situacao = contratoAutorizacao.DescricaoSituacao,
                                 Usuario = contratoAutorizacao.Usuario.Nome,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaContratoFreteTransportador.VerificarSePodeAprovar(codigo, contratoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(contratoAutorizacao)
                             }).ToList();

                // Retorna Grid
                grid.setarQuantidadeTotal(lista.Count());
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

        public async Task<IActionResult> AprovarMultiplosContratos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

                try
                {
                    contratos = ObterObterContratosSelecionadasSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }
                List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes = new List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario>();
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> contratosAutorizacoes = BuscarRegrasPorContratos(contratos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosContratosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < contratosAutorizacoes.Count(); i++)
                {
                    int codigo = contratosAutorizacoes[i].ContratoFreteTransportador.Codigo;

                    if (!codigosContratosVerificados.Contains(codigo))
                        codigosContratosVerificados.Add(codigo);

                    EfetuarAprovacao(contratosAutorizacoes[i], unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratosAutorizacoes[i].ContratoFreteTransportador, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosContratosVerificados)
                {
                    this.VerificarSituacaoContrato(repContratoFreteTransportador.BuscarPorCodigo(cod), out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoesContrato, unitOfWork);
                    notificacoes.AddRange(notificacoesContrato);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();

                NotificarUsuarios(notificacoes, unitOfWork);

                return new JsonpResult(new
                {
                    RegrasModificadas = contratosAutorizacoes.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplosContratos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

                try
                {
                    contratos = ObterObterContratosSelecionadasSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> contratosAutorizacao = BuscarRegrasPorContratos(contratos, this.Usuario.Codigo, unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes = new List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario>();

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosContratosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < contratosAutorizacao.Count(); i++)
                {
                    int codigo = contratosAutorizacao[i].ContratoFreteTransportador.Codigo;

                    if (!codigosContratosVerificados.Contains(codigo))
                        codigosContratosVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    contratosAutorizacao[i].Data = DateTime.Now;
                    contratosAutorizacao[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    contratosAutorizacao[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratosAutorizacao[i], null, "Reprovou a regra. Motivo: " + contratosAutorizacao[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaContratoFreteTransportador.Atualizar(contratosAutorizacao[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratosAutorizacao[i].ContratoFreteTransportador, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosContratosVerificados)
                {
                    this.VerificarSituacaoContrato(repContratoFreteTransportador.BuscarPorCodigo(cod), out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoesContrato, unitOfWork);
                    notificacoes.AddRange(notificacoesContrato);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();

                NotificarUsuarios(notificacoes, unitOfWork);

                return new JsonpResult(new
                {
                    RegrasModificadas = contratosAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> contratoAutorizacao = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorContratoUsuarioSituacao(codigo, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < contratoAutorizacao.Count(); i++)
                    EfetuarAprovacao(contratoAutorizacao[i], unitOfWork);

                this.VerificarSituacaoContrato(contrato, out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes, unitOfWork);
                NotificarUsuarios(notificacoes, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = contratoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
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
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador alcada = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (alcada == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (alcada.Bloqueada)
                    return new JsonpResult(false, "Não é possível aprovar essa alçada antes que as alçadas de os níveis anteriores sejam aprovadas.");

                // Valida a situacao
                if (alcada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(alcada, true, out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes, unitOfWork);
                NotificarUsuarios(notificacoes, unitOfWork);

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
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador contratoAutorizacao = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (contratoAutorizacao == null || contratoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (contratoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                contratoAutorizacao.Data = DateTime.Now;
                contratoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                contratoAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaContratoFreteTransportador.Atualizar(contratoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, contratoAutorizacao.ContratoFreteTransportador, unitOfWork);
                this.VerificarSituacaoContrato(contratoAutorizacao.ContratoFreteTransportador, out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes, unitOfWork);
                NotificarUsuarios(notificacoes, unitOfWork);

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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 7, Models.Grid.Align.center, true);
            //grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);

            if (exportacao)
            {
                grid.AdicionarCabecalho("Descrição", "Descricao");
                grid.AdicionarCabecalho("Data Inicial", "DataInicial");
                grid.AdicionarCabecalho("Data Final", "DataFinal");
                grid.AdicionarCabecalho("Valor Mensal", "ValorMensal");
                grid.AdicionarCabecalho("Tipo de Fechamento", "TipoFechamento");
                grid.AdicionarCabecalho("Quantidade Mensal de Cargas", "QuantidadeMensalCargas");
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular");
                grid.AdicionarCabecalho("Valor Acordado", "ValorAcordado");
                grid.AdicionarCabecalho("Tipos de Carga", "TipoCargas");
                grid.AdicionarCabecalho("Canais de Entrega", "CanaisEntrega");
                grid.AdicionarCabecalho("Quan. Veículo", "Quantidade");
                grid.AdicionarCabecalho("Total", "Total");
            }

            return grid;
        }

        private Models.Grid.Grid GridAcordos()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Periodo").Nome("Período").Tamanho(5);
            grid.Prop("ModeloVeicular").Nome("Modelo do Veículo").Tamanho(20);
            grid.Prop("ValorAcordado").Nome("Valor Acordado").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Quantidade").Nome("Quan. Veículo").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Total").Nome("Total").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("FranquiaPorKm").Nome("Franquia KM").Tamanho(5).Align(Models.Grid.Align.center);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador alcada, bool verificarSeEstaAprovado, out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);
            notificacoes = new List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario>();

            // So modifica a autorizacao quando ela for pendente
            if (alcada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && alcada.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                alcada.Data = DateTime.Now;
                alcada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaContratoFreteTransportador.Atualizar(alcada);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, alcada, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoContrato(alcada.ContratoFreteTransportador, out notificacoes, unitOfWork);

                this.NotificarAlteracao(true, alcada.ContratoFreteTransportador, unitOfWork);
            }
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador alcada, Repositorio.UnitOfWork unitOfWork)
        {
            EfetuarAprovacao(alcada, false, out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificacoes, unitOfWork);
        }

        private List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> BuscarRegrasPorContratos(List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> contratoAutorizacao = new List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador>();

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato in contratos)
            {
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> regras = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorContratoUsuarioSituacao(contrato.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);
                contratoAutorizacao.AddRange(regras);
            }

            return contratoAutorizacao;
        }

        private List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> ObterObterContratosSelecionadasSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> listaContrato = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaContrato, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaContratosNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ContratosNaoSelecionados"));
                foreach (var dybContratosNaoSelecionada in listaContratosNaoSelecionados)
                    listaContrato.Remove(new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador() { Codigo = (int)dybContratosNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaContratosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ContratosSelecionados"));
                foreach (var dynContratosSelecionada in listaContratosSelecionados)
                    listaContrato.Add(repContratoFreteTransportador.BuscarPorCodigo((int)dynContratosSelecionada.Codigo));
            }

            // Retorna lista
            return listaContrato;
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
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
                //string titulo = "Contrato Transportador";
                string mensagem = (aprovada ? "Aprovou" : "Rejeitou") + " o Contrato de Frete " + contrato.Numero;
                //serNotificacao.GerarNotificacaoEmail(contrato.Usuario, this.Usuario, contrato.Codigo, string.Empty, titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, mensagem, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void NotificarUsuarios(List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificar, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

            foreach (Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario notificacao in notificar)
            {
                serNotificacao.GerarNotificacao(notificacao.Usuario, notificacao.UsuarioGerouNotificacao, notificacao.CodigoObjeto, notificacao.URLPagina, notificacao.Nota, notificacao.Icone, notificacao.TipoNotificacao, TipoServicoMultisoftware, unitOfWork);
            }
        }

        private void VerificarSituacaoContrato(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, out List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario> notificar, Repositorio.UnitOfWork unitOfWork)
        {
            notificar = new List<Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario>();

            try
            {
                if (contrato.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao)
                {
                    Servicos.Embarcador.Frete.ContratoTransporteFrete servicosContratoTransporteFrete = new Servicos.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

                    Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repositorioAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> regras = repositorioAprovacaoAlcadaContratoFreteTransportador.BuscarRegrasDesbloqueadas(contrato.Codigo);
                    bool rejeitada = false;
                    bool aprovada = true;

                    foreach (Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regra in regras)
                    {
                        int pendentes = repositorioAprovacaoAlcadaContratoFreteTransportador.ContarPendentes(contrato.Codigo, regra.Codigo);
                        int aprovacoes = repositorioAprovacaoAlcadaContratoFreteTransportador.ContarAprovacoesSolicitacao(contrato.Codigo, regra.Codigo);
                        int rejeitadas = repositorioAprovacaoAlcadaContratoFreteTransportador.ContarRejeitadas(contrato.Codigo, regra.Codigo);
                        int necessariosParaAprovar = repositorioAprovacaoAlcadaContratoFreteTransportador.BuscarNumeroAprovacoesNecessariasPorRegra(contrato.Codigo, regra.Codigo);

                        if (rejeitadas > 0)
                            rejeitada = true;
                        if (aprovacoes < necessariosParaAprovar)
                            aprovada = false;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado;

                    if (rejeitada)
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Rejeitado;
                    else if (aprovada)
                        aprovada = Servicos.Embarcador.Frete.ContratoFreteTransportador.LiberarProximasHierarquiasDeAprovacao(contrato, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);

                    if (aprovada || rejeitada)
                    {
                        contrato.Situacao = situacao;

                        Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                        repositorioContratoFreteTransportador.Atualizar(contrato);

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone = rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                        string mensagem = string.Format(Localization.Resources.Fretes.AutorizacaoContratoFreteTransportador.ContratoFreteFoi, contrato.Numero, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitado : Localization.Resources.Gerais.Geral.Aprovado));

                        servicosContratoTransporteFrete.GerarRegistroIntegracaoContratoFreteCustoFixo(contrato);

                        notificar.Add(new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoUsuario
                        {
                            Usuario = contrato.Usuario,
                            UsuarioGerouNotificacao = Usuario,
                            CodigoObjeto = contrato.Codigo,
                            URLPagina = "",
                            Nota = mensagem,
                            Icone = icone,
                            TipoNotificacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                        });
                    }

                    Servicos.Embarcador.Frete.ContratoFreteTransportador.ContratoAprovado(contrato, Auditado, unitOfWork);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

            int transportador = Request.GetIntParam("Transportador");
            int usuario = Request.GetIntParam("Usuario");
            int numero = Request.GetIntParam("Numero");
            DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
            DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
            SituacaoContratoFreteTransportador? situacao = Request.GetNullableEnumParam<SituacaoContratoFreteTransportador>("Situacao");
            TipoAprovadorRegra tipoAprovadorRegra = TipoAprovadorRegra.Usuario;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                usuario = Usuario.Codigo;
                tipoAprovadorRegra = TipoAprovadorRegra.Transportador;
            }

            lista = repAprovacaoAlcadaContratoFreteTransportador.Consultar(usuario, dataInicial, dataFinal, situacao, numero, transportador, tipoAprovadorRegra, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaContratoFreteTransportador.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numero, transportador, tipoAprovadorRegra);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> lista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

            var listaProcessada = from item in lista
                                  select new
                                  {
                                      item.Codigo,
                                      Data = item.DataAlteracao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                      Numero = item.Numero,
                                      Transportador = item.Transportador?.Descricao ?? string.Empty,
                                      Situacao = item.DescricaoSituacao
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornoDynamicExportacao(List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> lista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> acordos = lista.SelectMany(x => x.Acordos).ToList();

            var listaProcessada = from item in lista
                                  join acordo in acordos on item.Codigo equals acordo.ContratoFrete.Codigo
                                  select new
                                  {
                                      item.Codigo,
                                      //Data = item.DataAlteracao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                      Numero = item.Numero,
                                      Transportador = item.Transportador?.Descricao ?? string.Empty,
                                      Situacao = item.DescricaoSituacao,
                                      Descricao = item.Descricao,
                                      DataInicial = item.DataInicial.ToString("dd/MM/yyyy"),
                                      DataFinal = item.DataFinal.ToString("dd/MM/yyyy"),
                                      ValorMensal = item.ValorMensal.ToString("n2"),
                                      TipoFechamento = item.PeriodoAcordo.ObterDescricao(),
                                      QuantidadeMensalCargas = item.QuantidadeMensalCargas,
                                      ModeloVeicular = acordo.ModeloVeicular.Descricao,
                                      ValorAcordado = acordo.ValorAcordado.ToString("n2"),
                                      TipoCargas = string.Join(" ,", item.TipoCargas.Select(x => x.TipoDeCarga.Descricao).ToList()),
                                      CanaisEntrega = string.Join(" ,", item.CanaisEntrega.Select(x => x.Descricao).ToList()),
                                      Quantidade = acordo.Quantidade,
                                      Total = acordo.Total.ToString("n2"),
                                  };

            return listaProcessada.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador regra)
        {
            return regra.RegraContratoFreteTransportador?.Descricao;
        }

        private void EfetuarResponsabilidade(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Usuario responsavel, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> verificacao = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorContratoEUsuario(contrato.Codigo, responsavel.Codigo);

            if (verificacao.Count() == 0)
            {
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador alcada = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador
                {
                    ContratoFreteTransportador = contrato,
                    Usuario = responsavel,
                    Delegada = true,
                    DataCriacao = contrato.DataAlteracao,
                };

                repAprovacaoAlcadaContratoFreteTransportador.Inserir(alcada);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, "Adicionou " + responsavel.Descricao + " como responsável de aprovação.", unitOfWork);
            }
            else
            {
                erro = "O usuário selecionado já é responsável pela aprovação.";
            }
        }

        #endregion
    }
}
