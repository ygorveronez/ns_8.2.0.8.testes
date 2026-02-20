using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/AutorizacaoAvaria")]
    public class AutorizacaoAvariaController : BaseController
    {
        #region Construtores

        public AutorizacaoAvariaController(Conexao conexao) : base(conexao) { }

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
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaSolicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaSolicitacoes, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaSolicitacoes, unitOfWork);

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
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaSolicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaSolicitacoes, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaSolicitacoes, unitOfWork);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            // Busca a ocorrencia
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                if (solicitacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                // Cliente pedido
                Dominio.Entidades.Cliente cliente = RetornarClientePedido(solicitacao);

                var dynOcorrencia = new
                {
                    solicitacao.Codigo,
                    solicitacao.NumeroAvaria,
                    EnumSituacao = solicitacao.Situacao,
                    DataAvaria = solicitacao.DataAvaria.ToString("dd/MM/yyyy"),
                    CodigoCarga = solicitacao.Carga.CodigoCargaEmbarcador,
                    Situacao = solicitacao.DescricaoSituacao,
                    Cliente = cliente.Nome + " (" + cliente.CPF_CNPJ_Formatado + ")",
                    Transportador = solicitacao.Transportador.RazaoSocial,
                    Veiculos = solicitacao.Carga.Veiculo != null ? solicitacao.Carga.Veiculo.Placa : string.Empty,

                    ValorAvaria = solicitacao.ValorAvaria.ToString("n2"),
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

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Converte parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int usuario = 0;
                int.TryParse(Request.Params("Usuario"), out usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Etapa", "Etapa", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);

                // Instancia repositorio
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);

                // Buscas regras do usuario para essa ocorrencia
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> regras = repSolicitacaoAvariaAutorizacao.BuscarPorSolicitacaoUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from solicitacaoAutorizacao in regras
                             select new
                             {
                                 solicitacaoAutorizacao.Codigo,
                                 Regra = TituloRegra(solicitacaoAutorizacao),
                                 Situacao = solicitacaoAutorizacao.DescricaoSituacao,
                                 Usuario = solicitacaoAutorizacao.Usuario.Nome,
                                 Etapa = solicitacaoAutorizacao.DescricaoEtapaAutorizacao,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repSolicitacaoAvariaAutorizacao.VerificarSePodeAprovar(codigo, solicitacaoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(solicitacaoAutorizacao)
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

        public async Task<IActionResult> AprovarMultiplasSolicitacoes()
        {
            /* Busca todas as ocorrencias selecionadas
             * Busca todas as regras das ocorrencias selecionadas
             * Aprova todas as regras
             * Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> solicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    solicitacoes = ObterSolicitacoesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> solicitacoesAutorizacoes = BuscarRegrasPorSolicitacoes(solicitacoes, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosSolicitacoesVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < solicitacoesAutorizacoes.Count(); i++)
                {
                    unitOfWork.FlushAndClear();
                    int codigo = solicitacoesAutorizacoes[i].SolicitacaoAvaria.Codigo;

                    if (!codigosSolicitacoesVerificados.Contains(codigo))
                        codigosSolicitacoesVerificados.Add(codigo);

                    // Metodo de aprovar a ocorrencia
                    EfetuarAprovacao(solicitacoesAutorizacoes[i], false, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacoesAutorizacoes[i].SolicitacaoAvaria, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosSolicitacoesVerificados)
                {
                    unitOfWork.FlushAndClear();
                    // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                    this.VerificarSituacaoAvaria(repSolicitacaoAvaria.BuscarPorCodigo(cod), unitOfWork);
                }
                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = solicitacoesAutorizacoes.Count()
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

        public async Task<IActionResult> ReprovarMultiplasSolicitacoes()
        {
            /* Busca todas as ocorrencias selecionadas
             * Busca todas as regras das ocorrencias selecionadas
             * Aprova todas as regras
             * Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");
                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Codigo da regra
                int codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria justificativa = repMotivoAvaria.BuscarPorCodigo(codigoJustificativa);

                // Valida justificativa (obrigatoria)
                if (justificativa == null)
                    return new JsonpResult(false, "Erro ao buscar justificativa.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> solicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    solicitacoes = ObterSolicitacoesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> solicitacoesAutorizacoes = BuscarRegrasPorSolicitacoes(solicitacoes, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosSolicitacoesVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < solicitacoesAutorizacoes.Count(); i++)
                {
                    int codigo = solicitacoesAutorizacoes[i].SolicitacaoAvaria.Codigo;

                    if (!codigosSolicitacoesVerificados.Contains(codigo))
                        codigosSolicitacoesVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    solicitacoesAutorizacoes[i].Data = DateTime.Now;
                    solicitacoesAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada;
                    solicitacoesAutorizacoes[i].Motivo = motivo;
                    solicitacoesAutorizacoes[i].MotivoAvaria = justificativa;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacoesAutorizacoes[i], null, "Reprovou a regra. Motivo: " + solicitacoesAutorizacoes[i].MotivoAvaria, unitOfWork);

                    // Atualiza banco
                    repSolicitacaoAvariaAutorizacao.Atualizar(solicitacoesAutorizacoes[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacoesAutorizacoes[i].SolicitacaoAvaria, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosSolicitacoesVerificados)
                    // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                    this.VerificarSituacaoAvaria(repSolicitacaoAvaria.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = solicitacoesAutorizacoes.Count()
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

        public async Task<IActionResult> DelegarMultiplasSolicitacoes()
        {
            /* Busca todas as solicitacoes selecionadas
             * Vincula o usuário selecionado a todas elas seguindo critério de situacao
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


                // Codigo da regra
                int codigoUsuario = 0;
                int.TryParse(Request.Params("UsuarioDelegado"), out codigoUsuario);

                // Entidade responsavel
                Dominio.Entidades.Usuario responsavel = repUsuario.BuscarPorCodigo(codigoUsuario);

                // Valida justificativa (obrigatoria)
                if (responsavel == null)
                    return new JsonpResult(false, true, "Erro ao buscar usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == responsavel.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> solicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

                try
                {
                    // Busca todas as avarias selecionadas
                    solicitacoes = ObterSolicitacoesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < solicitacoes.Count(); i++)
                {
                    // Só adiciona responsabilida à avarias que estão em situação Ag Lote
                    if (PermiteDelegar(solicitacoes[i].Situacao))
                        EfetuarResponsabilidade(solicitacoes[i], responsavel, unitOfWork);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DelegarSolicitacao()
        {
            /* Busca a avaria e o usuario
             * Cria vínculo
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


                // Codigo da regra
                int codigoUsuario = 0;
                int.TryParse(Request.Params("UsuarioDelegado"), out codigoUsuario);

                int codigoSolicitacao = 0;
                int.TryParse(Request.Params("Solicitacao"), out codigoSolicitacao);

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = repSolicitacaoAvaria.BuscarPorCodigo(codigoSolicitacao);
                Dominio.Entidades.Usuario responsavel = repUsuario.BuscarPorCodigo(codigoUsuario);

                // Valida
                if (solicitacao == null)
                    return new JsonpResult(false, true, "Erro ao buscar solicitação.");

                if (!PermiteDelegar(solicitacao.Situacao))
                    return new JsonpResult(false, true, "A situação da solicitação não permite essa operação.");

                if (responsavel == null)
                    return new JsonpResult(false, true, "Erro ao buscar usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == responsavel.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                // Inicia transacao
                unitOfWork.Start();

                string erro;

                // Adiciona responsável
                EfetuarResponsabilidade(solicitacao, responsavel, unitOfWork, out erro);

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar as solicitações.");
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

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Instancia
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Converte parametros
                int codigoSolicitacao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoSolicitacao);

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> solicitacoesAutorizacoes = repSolicitacaoAvariaAutorizacao.BuscarPorSolicitacaoUsuarioSituacao(codigoSolicitacao, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < solicitacoesAutorizacoes.Count(); i++)
                    EfetuarAprovacao(solicitacoesAutorizacoes[i], false, unitOfWork);

                // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                this.VerificarSituacaoAvaria(repSolicitacaoAvaria.BuscarPorCodigo(codigoSolicitacao), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = solicitacoesAutorizacoes.Count()
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

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao solicitacao = repSolicitacaoAvariaAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (solicitacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (solicitacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(solicitacao, true, unitOfWork);

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
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Codigo da regra
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao solicitacaoAutorizacao = repSolicitacaoAvariaAutorizacao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria justificativa = repMotivoAvaria.BuscarPorCodigo(codigoJustificativa);

                // Valida se é o usuario da regra
                if (solicitacaoAutorizacao == null || solicitacaoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida justificativa (obrigatoria)
                if (justificativa == null)
                    return new JsonpResult(false, "Erro ao buscar justificativa.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (solicitacaoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                solicitacaoAutorizacao.Data = DateTime.Now;
                solicitacaoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada;
                solicitacaoAutorizacao.Motivo = motivo;
                solicitacaoAutorizacao.MotivoAvaria = justificativa;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repSolicitacaoAvariaAutorizacao.Atualizar(solicitacaoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, solicitacaoAutorizacao.SolicitacaoAvaria, unitOfWork);
                this.VerificarSituacaoAvaria(solicitacaoAutorizacao.SolicitacaoAvaria, unitOfWork);

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

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "DataAvaria", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Solicitação", "NumeroAvaria", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Viagem", "NumeroViagem", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 20, Models.Grid.Align.left, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            else
                grid.AdicionarCabecalho("Motivo", "Motivo", 20, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tempo na Etapa", "TempoEtapa", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Valor")
                propOrdena = "ValorAvaria";
            else if (propOrdena == "Situacao")
                propOrdena = "SituacaoAvaria";
            else if (propOrdena == "NumeroViagem")
                propOrdena = "Carga.CodigoCargaEmbarcador";
            else if (propOrdena == "Avaria")
                propOrdena = "NumeroAvaria";
            else if (propOrdena == "Transportador")
                propOrdena = "Transportador.RazaoSocial";
        }

        /// <summary>
        /// Vncula responsável à solicitacao de acordo com a situação da avaria
        /// Avarias aguardado aprovação recebem mais um aprovador com tipo Delegado
        /// Avarias aprovadas recebem mais um responsável
        /// </summary>
        private void EfetuarResponsabilidade(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Dominio.Entidades.Usuario responsavel, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Metodo para avarias aprovadas
            if (solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgLote || solicitacao.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgLote)
            {
                Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

                // Verifica se registro já não consta
                Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria verificacao = repResponsavelAvaria.BuscaPorUsuarioAvaria(solicitacao.Codigo, responsavel.Codigo);

                if (verificacao == null)
                {
                    // Cria entidade
                    Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria responsavelAvaria = new Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria();
                    responsavelAvaria.SolicitacaoAvaria = solicitacao;
                    responsavelAvaria.Usuario = responsavel;

                    repResponsavelAvaria.Inserir(responsavelAvaria);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, null, "Delegou o responsável " + responsavel.Nome, unitOfWork);
                }
                else
                {
                    erro = "O usuário selecionado já é responsável pela avaria.";
                }
            }



            // Metodo para avarias aguardando aprovacao
            if (solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao || solicitacao.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao)
            {
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);

                // Verifica se registro já não consta
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> verificacao = repSolicitacaoAvariaAutorizacao.BuscarPorSolicitacaoUsuario(solicitacao.Codigo, responsavel.Codigo);

                if (verificacao.Count() == 0)
                {
                    // Cria entidade
                    Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao regraAvaria = new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao();
                    regraAvaria.SolicitacaoAvaria = solicitacao;
                    regraAvaria.Usuario = responsavel;
                    regraAvaria.OrigemRegraAvaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Delegada;
                    regraAvaria.EtapaAutorizacaoAvaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao;

                    repSolicitacaoAvariaAutorizacao.Inserir(regraAvaria);
                }
                else
                {
                    erro = "O usuário selecionado já é responsável pela avaria.";
                }
            }

        }

        private void EfetuarResponsabilidade(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Dominio.Entidades.Usuario responsavel, Repositorio.UnitOfWork unitOfWork)
        {
            string erro = string.Empty;
            EfetuarResponsabilidade(solicitacao, responsavel, unitOfWork, out erro);
        }

        /* EfetuarAprovacao
         * Aprova a autorizacao da carga
         */
        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao solicitacao, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente && solicitacao.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                solicitacao.Data = DateTime.Now;
                solicitacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada;

                // Atualiza os dados
                repSolicitacaoAvariaAutorizacao.Atualizar(solicitacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoAvaria(solicitacao.SolicitacaoAvaria, unitOfWork);

                // Notifica usuario que criou a ocorrencia
                this.NotificarAlteracao(true, solicitacao.SolicitacaoAvaria, unitOfWork);
            }
        }

        /* BuscarRegrasPorOcorrencias
         * Obtem todas regras do usuario relacionadas as ocorrencias
         */
        private List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> BuscarRegrasPorSolicitacoes(List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> solicitacoes, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> solicitacaoAutorizacao = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();

            // Itera todas as ocorrencias
            foreach (Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao in solicitacoes)
            {
                // Busca as autorizacoes da ocorrencias
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> regras = repSolicitacaoAvariaAutorizacao.BuscarPorSolicitacaoUsuarioSituacao(solicitacao.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente);

                // Adiciona na lista
                solicitacaoAutorizacao.AddRange(regras);
            }

            // Retornas a lista com todas as autorizacao das ocorrencias
            return solicitacaoAutorizacao;
        }

        /* ObterOcorrenciasSelecionadas
         * Duas maneiras de ocorrer a aprovacao em massa
         * - Selecionar todos (remove excecoes)
         * - Busca apenas selecionados
         */
        private List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> ObterSolicitacoesSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaSolicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaSolicitacoes, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaAvariasNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AvariasNaoSelecionadas"));
                foreach (var dybAvariasNaoSelecionada in listaAvariasNaoSelecionadas)
                    listaSolicitacoes.Remove(new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria() { Codigo = (int)dybAvariasNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaAvariasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AvariasSelecionadas"));
                foreach (var dynOAvariasSelecionada in listaAvariasSelecionadas)
                    listaSolicitacoes.Add(repSolicitacaoAvaria.BuscarPorCodigo((int)dynOAvariasSelecionada.Codigo));
            }

            // Retorna lista
            return listaSolicitacoes;
        }

        /* NotificarAlteracao
         * Envia notificacao para o autor da ocorrencia
         */
        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Repositorio.UnitOfWork unitOfWork)
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
                string titulo = Localization.Resources.Avarias.AutorizacaoAvaria.Avaria;
                string mensagem = string.Format(Localization.Resources.Avarias.AutorizacaoAvaria.UsuarioSolicitacaoValorCarga, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), solicitacao.NumeroAvaria, solicitacao.ValorAvaria.ToString("n2"), solicitacao.Carga.CodigoCargaEmbarcador);
                serNotificacao.GerarNotificacaoEmail(solicitacao.Solicitante, Usuario, solicitacao.Codigo, "Avarias/SolicitacaoAvaria", titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        /* VerificarSituacaoOcorrencia
         * Verificar a situacao da ocorrencia
         * Itera todas as regras da ocorrencia, se uma estiver rejeitada, rejeita a ocorrencia
         * Caso o numero minimo de aprovadores foi alcancado, regra esta aprovada
         * Caso o numero de pendentes for menor que o numero minimo, a carga automaticamente é rejeitada
         */
        private void VerificarSituacaoAvaria(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
                if (solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao || solicitacao.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao)
                {
                    // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                    Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
                    Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                    Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
                    Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                    // Busca todas regras da ocorrencia (Distintas)
                    //List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> regras = repSolicitacaoAvariaAutorizacao.BuscarRegrasAvaria(solicitacao.Codigo);
                    List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> alcadas = repSolicitacaoAvariaAutorizacao.BuscarPorSolicitacaoUsuario(solicitacao.Codigo, 0);
                    List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> regras = (from r in alcadas where r.RegrasAutorizacaoAvaria != null select r.RegrasAutorizacaoAvaria).Distinct().ToList();

                    // Flag de rejeicao
                    bool rejeitada = false;
                    bool aprovada = true;

                    foreach (Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regra in regras)
                    {
                        // Quantidade de usuarios que marcaram como aprovado ou rejeitado
                        int pendentes = repSolicitacaoAvariaAutorizacao.ContarPendentes(solicitacao.Codigo, regra.Codigo); // P

                        // Quantidade de aprovacoes
                        int aprovacoes = repSolicitacaoAvariaAutorizacao.ContarAprovacoesSolicitacao(solicitacao.Codigo, regra.Codigo); // A

                        int rejeitadas = repSolicitacaoAvariaAutorizacao.ContarRejeitadas(solicitacao.Codigo, regra.Codigo); // A

                        // Numero de aprovacoes minimas
                        int necessariosParaAprovar = (from a in alcadas where a.RegrasAutorizacaoAvaria.Codigo == regra.Codigo select a.NumeroAprovadores).FirstOrDefault();

                        // Situacao
                        //if (pendentes < (necessariosParaAprovar - aprovacoes)) // P < N -> Pendentes < NumeroMinimo
                        if (rejeitadas > 0)
                            rejeitada = true; // Se uma regra foi reprovada, a carga ocorrencia é reprovada

                        if (aprovacoes < necessariosParaAprovar) // A >= N -> Aprovacoes > NumeroMinimo
                            aprovada = false; // Se nao esta rejeitada e nem reprovada, esta pendente (nao faz nada)
                    }

                    // Define situacao da ocorrencia
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria situacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao;

                    if (rejeitada)
                    {
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.RejeitadaAutorizacao;
                        situacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.RejeitadaAutorizacao;
                    }

                    // Aprovada
                    else if (aprovada)
                    {
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgLote;
                        situacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Termo;
                    }

                    // Se houve alteracao de status, atualiza etapa da ocorencia
                    if (aprovada || rejeitada)
                    {
                        // Verifica se a situacao e ag aprovacao para testar a regra de etapa ag emissao
                        if ((solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao || solicitacao.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao) && aprovada)
                        {
                            // Fecha etapa
                            Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao fechaTempoEtapa = repTempoEtapaSolicitacao.BuscarUltimaEtapa(solicitacao.Codigo);
                            if (fechaTempoEtapa != null)
                            {
                                fechaTempoEtapa.Saida = DateTime.Now;
                                repTempoEtapaSolicitacao.Atualizar(fechaTempoEtapa);
                            }

                            // Caso não tenha nenhum resposnavel, atualiza situacao
                            if (!Servicos.Embarcador.Avarias.ResponsavelAvaria.CriaResponsavelSolicitacao(solicitacao, unitOfWork))
                                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraLote;
                        }

                        // Seta a nova situacao
                        solicitacao.Situacao = situacao;
                        solicitacao.SituacaoFluxo = situacaoFluxo;

                        // Atualzia entidade
                        repSolicitacaoAvaria.Atualizar(solicitacao);

                        // Define icone
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                        if (rejeitada)
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                        else
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                        // Emite notificação
                        string mensagem = string.Format(Localization.Resources.Avarias.AutorizacaoAvaria.SolicitacaoValorCargaFoi, solicitacao.NumeroAvaria.ToString(), solicitacao.ValorAvaria.ToString("n2"), solicitacao.Carga.CodigoCargaEmbarcador, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));
                        serNotificacao.GerarNotificacao(solicitacao.Solicitante, this.Usuario, solicitacao.Codigo, "Avarias/AutorizacaoAvaria", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        /* RetornarClientePedido
         * Busca informacao do primeiro pedido da carga
         */
        private Dominio.Entidades.Cliente RetornarClientePedido(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao)
        {

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = solicitacao.Carga.CargaOrigemPedidos.FirstOrDefault();
            return cargaPedido.ObterTomador();
        }

        /* CoresRegras
         * Retorna a cor da linha de acordo com a situacoa
         */
        private string CoresRegras(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente && regra.OrigemRegraAvaria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Delegada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }

        /* ExecutaPesquisa
         * Converte os valores vindo por POST 
         * E faz consulta de ocorrencias pendentes de aprovações
         */
        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaSolicitacoes, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria.FiltroPesquisaAutorizacaoAvaria filtroPesquisaAutorizacaoAvaria = ObterFiltrosPesquisa();

            int empresa = 0;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                empresa = this.Usuario.Empresa.Codigo;

            listaSolicitacoes = repSolicitacaoAvariaAutorizacao.Consultar(empresa, filtroPesquisaAutorizacaoAvaria, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repSolicitacaoAvariaAutorizacao.ContarConsulta(empresa, filtroPesquisaAutorizacaoAvaria);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaSolicitacoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

            List<int> codigosAvaria = (from obj in listaSolicitacoes select obj.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> responsaveis = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();
            List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao> tempoEtapaSolicitacaos = new List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao>();
            List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtosAvariados = new List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            if (codigosAvaria.Count <= 1000)
            {
                responsaveis = repSolicitacaoAvariaAutorizacao.ResponsaveisSolicitacaoAvaria(codigosAvaria);
                tempoEtapaSolicitacaos = repTempoEtapaSolicitacao.TemposNaEtapa(codigosAvaria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Autorizacao);
                produtosAvariados = repProdutosAvariados.BuscarPorAvarias(codigosAvaria);
            }

            var lista = from solicitacao in listaSolicitacoes
                        select new
                        {
                            solicitacao.Codigo,
                            DataAvaria = solicitacao.DataAvaria.ToString("dd/MM/yyyy"),
                            NumeroAvaria = solicitacao.NumeroAvaria,
                            NumeroViagem = solicitacao.Carga.CodigoCargaEmbarcador,
                            Motivo = solicitacao.MotivoAvaria.Descricao,
                            Responsavel = (solicitacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao || solicitacao.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao) ? FormataResponsaveis((from obj in responsaveis where obj.SolicitacaoAvaria.Codigo == solicitacao.Codigo select obj.Usuario).ToList()) : solicitacao.Responsaveis,
                            Transportador = solicitacao.Transportador?.Descricao ?? string.Empty,
                            Filial = solicitacao.Carga.Filial != null ? solicitacao.Carga.Filial.Descricao : string.Empty,
                            Valor = (from obj in produtosAvariados where obj.SolicitacaoAvaria.Codigo == solicitacao.Codigo select obj.ValorAvaria).Sum().ToString("n2"),
                            TempoEtapa = formatarTempos((from obj in tempoEtapaSolicitacaos where obj.SolicitacaoAvaria.Codigo == solicitacao.Codigo select obj).ToList()),
                            Situacao = solicitacao.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private string formatarTempos(List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao> tempoEtapaSolicitacaos)
        {
            double horas = 0;
            foreach (Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao tempo in tempoEtapaSolicitacaos)
                horas = horas + tempo.Saida.Value.Subtract(tempo.Entrada).TotalHours;

            return horas.ToString("n1").Replace(',', '.') + "h";
        }

        private string FormataResponsaveis(List<Dominio.Entidades.Usuario> responsaveis)
        {
            var aprovadores = (from o in responsaveis where !string.IsNullOrWhiteSpace(o.Nome) select o.Nome).ToList();

            return String.Join(", ", aprovadores);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            return repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
        }

        private bool PermiteDelegar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacaoa)
        {
            bool pode = false;

            if (situacaoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgLote)
                pode = true;
            else if (situacaoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao)
                pode = true;

            return pode;
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao regra)
        {
            if (regra.OrigemRegraAvaria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Delegada)
                return "(Delegado)";
            else
                return regra.RegrasAutorizacaoAvaria?.Descricao;
        }

        private Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria.FiltroPesquisaAutorizacaoAvaria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.AutorizacaoAvaria.FiltroPesquisaAutorizacaoAvaria()
            {
                NumeroAvaria = Request.GetIntParam("NumeroAvaria"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                EtapaAutorizacaoAvaria = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria>("EtapaAutorizacao"),
                SituacaoAvaria = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria>("Situacao")
            };
        }

        #endregion
    }
}
