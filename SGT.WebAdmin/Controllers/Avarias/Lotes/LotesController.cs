using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/Lotes")]
    public class LotesController : BaseController
    {
        #region Construtores

        public LotesController(Conexao conexao) : base(conexao) { }

        #endregion

        private string CorProdutoRemovido = "#dfd0ef";

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAvarias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaAvarias();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaAvarias(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaAvarias(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaProduto();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaProduto(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaProduto(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                if (lote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    lote.Codigo,
                    NumeroLote = lote.Numero.ToString(),
                    Transportador = lote.Transportador?.RazaoSocial ?? string.Empty,
                    Responsavel = string.Join(", ", (from r in repResponsavelAvaria.ResponsavelLote(lote.Codigo) select r.Nome).ToArray()),
                    Criador = lote.Criador.Nome,
                    NumeroAvarias = lote.Avarias.Count(),
                    Situacao = lote.Situacao,
                    SituacaoLote = lote.DescricaoSituacao,
                    CodigoMotivoAvaria = lote.MotivoAvaria?.Codigo ?? 0,
                    MotivoAvaria = lote.MotivoAvaria?.Descricao ?? string.Empty,
                    // Lotes pendentes
                    DataGeracao = lote.DataGeracao.ToString("dd/MM/yyyy"),
                    ValorLote = lote.ValorLote.ToString("n2"),
                    ValorAvarias = lote.ValorAvarias.ToString("n2"),
                    ValorDescontos = lote.ValorDescontos.ToString("n2"),
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

        public async Task<IActionResult> DescontarAvaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int.TryParse(Request.Params("Solicitacao"), out int solicitacao);
                int.TryParse(Request.Params("Motivo"), out int motivoDescontoAvaria);
                decimal.TryParse(Request.Params("Desconto"), out decimal desconto);

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoDescontoAvaria repMotivoDescontoAvaria = new Repositorio.Embarcador.Avarias.MotivoDescontoAvaria(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(solicitacao, true);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (avaria.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.LoteGerado)
                    return new JsonpResult(false, true, "A situação da Avaria não permite essa alteração.");

                // Valida
                Dominio.Entidades.Embarcador.Avarias.MotivoDescontoAvaria motivo = repMotivoDescontoAvaria.BuscarPorCodigo(motivoDescontoAvaria);
                if (motivo == null && desconto > 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro de motivo.");

                // Seta como removido
                avaria.ValorDesconto = desconto;
                avaria.MotivoDesconto = desconto > 0 ? motivo : null;

                repSolicitacaoAvaria.Atualizar(avaria, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, avaria.Lote, null, "Aplicou desconto na avaria.", unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar desconto da avaria.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int solicitacao = 0;
                int.TryParse(Request.Params("Solicitacao"), out solicitacao);

                bool removido = false;
                string erro;

                if (solicitacao == 0)
                    removido = RemoverProdutosLote(codigo, unitOfWork, out erro);
                else
                    removido = RemoverProdutoAvaria(codigo, solicitacao, unitOfWork, out erro);

                if (!removido)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover produto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReadionarProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int solicitacao = 0;
                int.TryParse(Request.Params("Solicitacao"), out solicitacao);

                bool readicionado = false;
                string erro;

                if (solicitacao == 0)
                    readicionado = ReadionarProdutosLote(codigo, unitOfWork, out erro);
                else
                    readicionado = ReadionarProdutoAvaria(codigo, solicitacao, unitOfWork, out erro);

                if (!readicionado)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar produto.");
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

                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                if (lote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCriacao && lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCorrecao)
                    return new JsonpResult(false, true, "A situação do lote não permite essa ação.");

                // Busca controle de tempo da etapa aberta e atualiza
                Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote finalizarEtapa = repTempoEtapaLote.BuscarUltimaEtapa(lote.Codigo);
                finalizarEtapa.Saida = DateTime.Now;
                repTempoEtapaLote.Atualizar(finalizarEtapa);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || lote.MotivoAvaria.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.CarregamentoDescarregamento)
                {
                    // Finaliza lote
                    lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Finalizada;
                    lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Finalizado;

                    // Seta todas avarias como finalizadas
                    FinalizaAvariasLote(lote, unitOfWork);

                    GerarOcorrencia(lote, unitOfWork);
                }
                else
                {
                    // Exige o aceite do transportador
                    lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoTransportador;
                    lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.AutorizacaoLote;

                    // Cria o novo controle de tempo
                    Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoLote = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote();
                    tempoLote.Entrada = DateTime.Now;
                    tempoLote.Saida = null;
                    tempoLote.Lote = lote;
                    tempoLote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.AutorizacaoLote;

                    repTempoEtapaLote.Inserir(tempoLote);
                }

                repLote.Atualizar(lote);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Corrigir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoFechar = repTempoEtapaLote.BuscarUltimaEtapa(codigo);

                // Valida
                if (lote == null || tempoFechar == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.ReprovacaoTransportador)
                    return new JsonpResult(false, true, "A situação do lote não permite essa ação.");

                // Seta a nova situação e etapa
                lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCorrecao;
                lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote;

                // Atualiza etapa tempo
                tempoFechar.Saida = DateTime.Now;

                // Cria o tempo da nova etapa
                Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote();
                tempoEtapa.Entrada = DateTime.Now;
                tempoEtapa.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote;
                tempoEtapa.Lote = lote;
                tempoEtapa.Saida = null;

                // Persiste dados
                repTempoEtapaLote.Atualizar(tempoFechar);
                repLote.Atualizar(lote);
                repTempoEtapaLote.Inserir(tempoEtapa);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Iniciou correção do lote", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IntegrarLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                // Valida
                if (lote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoIntegracao)
                    return new JsonpResult(false, true, "A situação do lote não permite essa operação.");

                if (lote.MotivoAvaria.Responsavel != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.Transportador)
                    return new JsonpResult(false, true, "Esse lote não pode ter integrações.");

                EfetuarIntegracaoLote(lote, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Integrou o lote " + lote.Descricao, unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao integrar o lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarMultiplosLotes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes = ObterLotesSelecionadas(unitOfWork);

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote> listaUltimasEtapasLotes = repTempoEtapaLote.BuscarUltimasEtapasLotes(lotes.Select(x => x.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Avarias.Lote lote in lotes)
                {
                    if (lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCriacao && lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCorrecao)
                        continue;

                    Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote finalizarEtapa = listaUltimasEtapasLotes.Find(x => x.Lote.Codigo == lote.Codigo);
                    finalizarEtapa.Saida = DateTime.Now;
                    await repTempoEtapaLote.AtualizarAsync(finalizarEtapa);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || lote.MotivoAvaria.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.CarregamentoDescarregamento)
                    {
                        lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Finalizada;
                        lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Finalizado;

                        FinalizaAvariasLote(lote, unitOfWork);
                        GerarOcorrencia(lote, unitOfWork);
                    }
                    else
                    {
                        lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoTransportador;
                        lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.AutorizacaoLote;

                        Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoLote = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote();
                        tempoLote.Entrada = DateTime.Now;
                        tempoLote.Saida = null;
                        tempoLote.Lote = lote;
                        tempoLote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.AutorizacaoLote;

                        await repTempoEtapaLote.InserirAsync(tempoLote);
                    }

                    await repLote.AtualizarAsync(lote);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao finalizar um ou mais lotes.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> IntegrarMultiplosLotes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes = ObterLotesSelecionadas(unitOfWork);

                await unitOfWork.StartAsync();

                for (int i = 0; i < lotes.Count; i++)
                    if (lotes[i].Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoIntegracao && lotes[i].MotivoAvaria.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.Transportador)
                    {
                        EfetuarIntegracaoLote(lotes[i], unitOfWork);
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, lotes[i], null, "Integrou o lote " + lotes[i].Descricao, unitOfWork);
                    }

                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao aprovar os lotes.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                // Valida
                if (lote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoIntegracao)
                    return new JsonpResult(false, true, "A situação do lote não permite essa ação.");

                // Atualiza a situação do lote
                lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoTransportador;

                // Persiste dados
                repLote.Atualizar(lote);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Voltou etapa do lote", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao integrar o lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTipoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                int codigoMotivoAvaria = Request.GetIntParam("MotivoAvaria");

                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivo = repMotivoAvaria.BuscarPorCodigo(codigoMotivoAvaria);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = motivo?.TipoOcorrencia;

                var retorno = new
                {
                    Codigo = tipoOcorrencia?.Codigo ?? 0,
                    Descricao = tipoOcorrencia?.Descricao ?? "",
                    OrigemOcorrencia = tipoOcorrencia?.OrigemOcorrencia ?? OrigemOcorrencia.PorCarga,
                    ComponenteCodigo = tipoOcorrencia?.ComponenteFrete?.Codigo ?? 0,
                    ComponenteDescricao = tipoOcorrencia?.ComponenteFrete?.Descricao ?? "",
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o tipo de ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool ReadionarProdutoAvaria(int codigo, int solicitacao, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Instancia repositorios
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

            // Busca informacoes
            Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto = repProdutosAvariados.BuscarPorSolicitacaoEProduto(solicitacao, codigo);

            // Valida
            if (produto == null)
            {
                erro = "Não foi possível encontrar o registro.";
                return false;
            }
            if (produto.RemovidoLote == false)
            {
                erro = "O produto já está no lote.";
                return false;
            }
            produto.Initialize();

            // Seta como nao removido
            produto.RemovidoLote = false;
            produto.RemovidoObservacao = string.Empty;
            produto.MotivoRemocaoLote = null;
            Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.SolicitacaoAvaria, null, "Readicionou o Produto" + produto.ProdutoEmbarcador.Descricao, unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.SolicitacaoAvaria.Lote, null, "Readicionou o Produto" + produto.ProdutoEmbarcador.Descricao, unitOfWork);

            // Persiste dados
            repProdutosAvariados.Atualizar(produto, Auditado);

            return true;
        }
        private bool ReadionarProdutosLote(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Instancia repositorios
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

            // Busca de todas as avarias esse produto
            Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoDaLista = repProdutosAvariados.BuscarPorCodigo(codigo);

            // Valida
            if (produtoDaLista == null)
            {
                erro = "Não foi possível encontrar o registro.";
                return false;
            }

            // Busca os produtos
            List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtos = repProdutosAvariados.BuscarPorProdutoDeLote(produtoDaLista.SolicitacaoAvaria.Lote.Codigo, produtoDaLista.ProdutoEmbarcador.Codigo);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoDaLista.SolicitacaoAvaria.Lote, null, "Readicionou o Produto" + produtoDaLista.ProdutoEmbarcador.Descricao, unitOfWork);

            // Seta como removido
            for (var i = 0; i < produtos.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto = produtos[i];
                produto.Initialize();

                produto.RemovidoLote = false;
                produto.RemovidoObservacao = string.Empty;
                produto.MotivoRemocaoLote = null;

                repProdutosAvariados.Atualizar(produto, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.SolicitacaoAvaria, null, "Readicionou o Produto" + produto.ProdutoEmbarcador.Descricao, unitOfWork);
            }

            return true;
        }

        private bool RemoverProdutoAvaria(int codigo, int solicitacao, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Instancia repositorios
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

            // Busca informacoes
            Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto = repProdutosAvariados.BuscarPorSolicitacaoEProduto(solicitacao, codigo);

            // Valida
            if (produto == null)
            {
                erro = "Não foi possível encontrar o registro.";
                return false;
            }
            if (produto.RemovidoLote == true)
            {
                erro = "O produto já está removido do lote.";
                return false;
            }
            produto.Initialize();

            // Busca os parametros de remocao
            int motivoRemovidoLote = 0;
            int.TryParse(Request.Params("Motivo"), out motivoRemovidoLote);
            Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivo = repMotivoRemocaoLote.BuscarPorCodigo(motivoRemovidoLote);

            string removidoObservacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(removidoObservacao)) removidoObservacao = string.Empty;

            // Seta como removido
            produto.RemovidoLote = true;
            produto.RemovidoObservacao = removidoObservacao;
            produto.MotivoRemocaoLote = motivo;

            Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.SolicitacaoAvaria, null, "Removeu o Produto" + produto.ProdutoEmbarcador.Descricao, unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.SolicitacaoAvaria.Lote, null, "Removeu o Produto" + produto.ProdutoEmbarcador.Descricao, unitOfWork);

            repProdutosAvariados.Atualizar(produto, Auditado);

            return true;
        }
        private bool RemoverProdutosLote(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Instancia repositorios
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

            // Busca de todas as avarias esse produto
            Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoDaLista = repProdutosAvariados.BuscarPorCodigo(codigo);

            // Valida
            if (produtoDaLista == null)
            {
                erro = "Não foi possível encontrar o registro.";
                return false;
            }

            // Busca os parametros de remocao
            int motivoRemovidoLote = 0;
            int.TryParse(Request.Params("Motivo"), out motivoRemovidoLote);
            Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivo = repMotivoRemocaoLote.BuscarPorCodigo(motivoRemovidoLote);

            string removidoObservacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(removidoObservacao)) removidoObservacao = string.Empty;

            List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtos = repProdutosAvariados.BuscarPorProdutoDeLote(produtoDaLista.SolicitacaoAvaria.Lote.Codigo, produtoDaLista.ProdutoEmbarcador.Codigo);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoDaLista.SolicitacaoAvaria.Lote, null, "Removeu o Produto" + produtoDaLista.ProdutoEmbarcador.Descricao, unitOfWork);

            // Seta como removido
            for (var i = 0; i < produtos.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto = produtos[i];
                produto.Initialize();

                produto.RemovidoLote = true;
                produto.RemovidoObservacao = removidoObservacao;
                produto.MotivoRemocaoLote = motivo;

                repProdutosAvariados.Atualizar(produto, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.SolicitacaoAvaria, null, "Removeu o Produto" + produto.ProdutoEmbarcador.Descricao, unitOfWork);
            }

            return true;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("EnumSituacaoLote", false);
            grid.AdicionarCabecalho("Número Lote", "Numero", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Criador", "Criador", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data da Geração", "DataGeracao", 10, Models.Grid.Align.left, true);

            return grid;
        }
        private Models.Grid.Grid GridPesquisaAvarias()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("MotivoCodigo", false);
            grid.AdicionarCabecalho("MotivoDescricao", false);
            grid.AdicionarCabecalho("Desconto", false);
            grid.AdicionarCabecalho("Número", "NumeroAvaria", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Avaria", "DataSolicitacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Valor", "ValorAvaria", 10, Models.Grid.Align.right, false);

            return grid;
        }
        private Models.Grid.Grid GridPesquisaProduto()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Para mudar o cabecalho
            int solicitacao = 0;
            int.TryParse(Request.Params("Solicitacao"), out solicitacao);

            bool ordena = solicitacao > 0;

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("RemovidoLote", false);
            grid.AdicionarCabecalho("Código", "CodigoProduto", 10, Models.Grid.Align.left, ordena);
            grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, ordena);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 15, Models.Grid.Align.left, ordena);
            grid.AdicionarCabecalho("Caixas Avariadas", "CaixasAvariadas", 10, Models.Grid.Align.right, ordena);
            grid.AdicionarCabecalho("Unidades Avariadas", "UnidadesAvariadas", 10, Models.Grid.Align.right, ordena);
            grid.AdicionarCabecalho("Valor Total", "ValorAvaria", 10, Models.Grid.Align.right, ordena);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            // Dados do filtro
            int transportadora = 0;
            int.TryParse(Request.Params("Transportadora"), out transportadora);

            int filial = 0;
            int.TryParse(Request.Params("Filial"), out filial);

            int motivo = 0;
            int.TryParse(Request.Params("Motivo"), out motivo);

            int lote = 0;
            int.TryParse(Request.Params("NumeroLote"), out lote);

            DateTime dataInicio;
            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
            DateTime dataFim;
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao;
            Enum.TryParse(Request.Params("Situacao"), out situacao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Todas;

            int solicitante = 0;

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.Lote> listaGrid = repLote.Consultar(solicitante, transportadora, filial, motivo, lote, dataInicio, dataFim, situacao, etapa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLote.ContarConsulta(solicitante, transportadora, filial, motivo, lote, dataInicio, dataFim, situacao, etapa);

            List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> responsaveis = repResponsavelAvaria.ResponsavelLotes((from obj in listaGrid select obj.Codigo).ToList());

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            EnumSituacaoLote = obj.Situacao,
                            Numero = obj.Numero.ToString(),
                            Transportador = obj.Transportador?.Descricao ?? string.Empty,
                            Responsavel = String.Join(", ", (from r in responsaveis where r.SolicitacaoAvaria.Lote != null && r.SolicitacaoAvaria.Lote.Codigo == obj.Codigo select r.Usuario.Nome).ToArray()),
                            Criador = obj.Criador.Nome,
                            Situacao = obj.DescricaoSituacao,
                            DataGeracao = obj.DataGeracao.ToString("dd/MM/yyyy")
                        };

            return lista.ToList();
        }
        private dynamic ExecutaPesquisaAvarias(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            // Dados do filtro
            int lote = 0;
            int.TryParse(Request.Params("Lote"), out lote);

            int numeroAvaria = 0;
            int.TryParse(Request.Params("NumeroAvaria"), out numeroAvaria);

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaGrid = repSolicitacaoAvaria.ConsultarPorLote(lote, numeroAvaria, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repSolicitacaoAvaria.ContarConsultaPorLote(lote, numeroAvaria);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            MotivoCodigo = obj.MotivoDesconto?.Codigo ?? 0,
                            MotivoDescricao = obj.MotivoDesconto?.Descricao ?? string.Empty,
                            Desconto = obj.ValorDesconto.ToString("n2"),
                            NumeroAvaria = obj.NumeroAvaria.ToString(),
                            DataSolicitacao = obj.DataSolicitacao.ToString("dd/MM/yyyy"),
                            Filial = obj.Carga.Filial?.Descricao ?? string.Empty,
                            ValorAvaria = obj.ValorAvaria.ToString("n2")
                        };

            return lista.ToList();
        }
        private dynamic ExecutaPesquisaProduto(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

            // Dados do filtro
            int lote = 0;
            int.TryParse(Request.Params("Lote"), out lote);

            int solicitacao = 0;
            int.TryParse(Request.Params("Solicitacao"), out solicitacao);

            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = "";

            string codigoEmbarcador = Request.Params("CodigoEmbarcador");
            if (string.IsNullOrWhiteSpace(codigoEmbarcador)) codigoEmbarcador = "";

            if (solicitacao > 0)
            {
                var listaGrid = repLote.ConsultarProdutosAvaria(lote, solicitacao, descricao, codigoEmbarcador, null, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = repLote.ContarConsultaProdutosAvaria(lote, solicitacao, descricao, codigoEmbarcador, null);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                RemovidoLote = obj.RemovidoLote,
                                CodigoProduto = obj.ProdutoEmbarcador.Codigo,
                                Produto = obj.ProdutoEmbarcador.Descricao,
                                GrupoProduto = obj.ProdutoEmbarcador.GrupoProduto?.Descricao ?? string.Empty,
                                CaixasAvariadas = obj.CaixasAvariadas,
                                UnidadesAvariadas = obj.UnidadesAvariadas,
                                ValorAvaria = $"R$ {obj.ValorAvaria.ToString("n2")}",
                                DT_RowColor = obj.RemovidoLote ? CorProdutoRemovido : ""
                            };

                return lista.ToList();
            }
            else
            {
                var listaGrid = repLote.ConsultarProdutosLote(lote, descricao, codigoEmbarcador, inicio, limite);
                totalRegistros = repLote.ContarConsultaProdutosLote(lote, descricao, codigoEmbarcador);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                RemovidoLote = false,
                                obj.CodigoProduto,
                                Produto = obj.ProdutoEmbarcador,
                                GrupoProduto = obj.GrupoProduto,
                                CaixasAvariadas = obj.CaixasAvariadas,
                                UnidadesAvariadas = obj.UnidadesAvariadas,
                                ValorAvaria = $"R$ {obj.ValorAvaria.ToString("n2")}",
                            };

                return lista.ToList();
            }
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Criador") propOrdenar = "Criador.Nome";
        }
        private void PropOrdenaProduto(ref string propOrdenar)
        {
            if (propOrdenar == "Produto") propOrdenar = "ProdutoEmbarcador.Descricao";
            else if (propOrdenar == "GrupoProduto") propOrdenar = "ProdutoEmbarcador.GrupoProduto.Descricao";
        }
        private void PropOrdenaAvarias(ref string propOrdenar)
        {
            if (propOrdenar == "Filial") propOrdenar = "Carga.Filial.Descricao";
        }

        private List<Dominio.Entidades.Embarcador.Avarias.Lote> ObterLotesSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            List<Dominio.Entidades.Embarcador.Avarias.Lote> listaLotes = new List<Dominio.Entidades.Embarcador.Avarias.Lote>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    listaLotes = ExecutaPesquisa(ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaLotesNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("LotesNaoSelecionados"));
                foreach (var dybLotesNaoSelecionada in listaLotesNaoSelecionadas)
                    listaLotes.Remove(new Dominio.Entidades.Embarcador.Avarias.Lote() { Codigo = (int)dybLotesNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaLotesSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("LotesSelecionados"));
                foreach (var dynLoteSelecionada in listaLotesSelecionadas)
                    listaLotes.Add(repLote.BuscarPorCodigo((int)dynLoteSelecionada.Codigo));
            }

            // Retorna lista
            return listaLotes;
        }

        private List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> BuscarResposaveisPorLote(List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            // Pega os id
            List<int> idsLotes = (from o in lotes select o.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> responsaveis = repResponsavelAvaria.AprovacoesLote(idsLotes, usuario);

            return responsaveis;
        }

        private void EfetuarIntegracaoLote(Dominio.Entidades.Embarcador.Avarias.Lote lote, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork);
            Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            // Busca Tempo Lote e fecha o mesmo
            Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoLote = repTempoEtapaLote.BuscarUltimaEtapa(lote.Codigo);
            if (tempoLote != null)
            {
                tempoLote.Saida = DateTime.Now;
                repTempoEtapaLote.Atualizar(tempoLote);
            }

            // Atualiza status do lote
            lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgIntegracao;

            // Cria entidade para integracao
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = Servicos.Embarcador.Avarias.LoteGrupoPessoasLayoutEDI.LayoutEDILote(lote);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> EDIs = (from o in layouts where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.EAI select o).ToList();

            for (var i = 0; i < EDIs.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDIIntegracao = new Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao
                {
                    Lote = lote,
                    LayoutEDI = EDIs[i].LayoutEDI,
                    TipoIntegracao = EDIs[i].TipoIntegracao,
                    ProblemaIntegracao = "",
                    NumeroTentativas = 0,
                    DataIntegracao = DateTime.Now,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                };

                repLoteEDIIntegracao.Inserir(loteEDIIntegracao);
            }

            // Seta todas avarias como finalizadas
            FinalizaAvariasLote(lote, unitOfWork);

            // Quando existe integrações pendentes, o lote ficam EM INTEGRAÇÃO
            // Pois o controle da situação do passa a ser responsabilidade da thread de integrações
            // Caso contrário, o lote é finalizado aqui mesmo
            if (EDIs.Count() == 0)
                lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Finalizada;
            else
                lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmIntegracao;

            lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Integrado;
            repLote.Atualizar(lote);
        }

        private void FinalizaAvariasLote(Dominio.Entidades.Embarcador.Avarias.Lote lote, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> avarias = repSolicitacaoAvaria.BuscarPorLote(lote.Codigo);

            foreach (Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria in avarias)
            {
                if (avaria.Situacao == SituacaoAvaria.Finalizada)
                    continue;

                avaria.Situacao = SituacaoAvaria.Finalizada;
                repSolicitacaoAvaria.Atualizar(avaria);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, avaria, null, "Finalizou o lote " + lote.Descricao, unitOfWork);

                if (avaria.TituloBaixaAgrupadoDocumento != null)
                {
                    Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                    Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = avaria.TituloBaixaAgrupadoDocumento;
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento;

                    if (tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento == TipoDocumentoTitulo.CTe)
                        documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Codigo, TipoLiquidacao.Fatura);
                    else
                        documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloBaixaAgrupadoDocumento.TituloDocumento.Carga.Codigo, TipoLiquidacao.Fatura);

                    decimal valorAvaria = avaria.ValorAvaria;

                    tituloBaixaAgrupadoDocumento.ValorAvaria += valorAvaria;
                    repTituloBaixaAgrupadoDocumento.Atualizar(tituloBaixaAgrupadoDocumento);

                    if (documentoFaturamento != null)
                    {
                        documentoFaturamento.ValorAvaria += valorAvaria;
                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    }
                }
            }
        }

        private void GerarOcorrencia(Dominio.Entidades.Embarcador.Avarias.Lote lote, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;
            if (lote.MotivoAvaria == null || !lote.MotivoAvaria.GerarOcorrenciaAutomaticamente)
                return;
            if (lote.MotivoAvaria.GerarOcorrenciaAutomaticamente && lote.MotivoAvaria.TipoOcorrencia == null)
                throw new ControllerException("Não foi informado o Tipo de Ocorrência no cadastro do motivo da avaria para gerar automaticamente a ocorrência!");

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia servicoOcorrenciaCalculoFrete = new Servicos.Embarcador.Carga.Ocorrencia();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = lote.Avarias.Select(o => o.Carga).FirstOrDefault();
            Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria = lote.MotivoAvaria;

            if (carga == null)
                throw new ControllerException("Nenhuma carga foi encontrada das avarias!");

            if (lote.ValorLote == 0 && lote.MotivoAvaria.ObrigarInformarValorParaLiberarOcorrencia)
                throw new ControllerException("Valor das avarias do lote estão zeradas!");

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            bool retornarPreCtes = false;
            if (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe)
                retornarPreCtes = true;
            cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, false, true, 0, 0, retornarPreCtes);

            if (cargaCTEs == null || cargaCTEs.Count == 0)
                throw new ControllerException("Não encontrado CTes autorizados para geração da ocorrência.");

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(motivoAvaria.TipoOcorrencia.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia = null;

            if (motivoAvaria.CalcularOcorrenciaPorTabelaFrete)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroBooleano = tipoOcorrencia.ParametrosOcorrencia.Where(o => o.TipoParametro == TipoParametroOcorrencia.Booleano).FirstOrDefault();
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroInteiro = tipoOcorrencia.ParametrosOcorrencia.Where(o => o.TipoParametro == TipoParametroOcorrencia.Inteiro).FirstOrDefault();
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroPeriodo = tipoOcorrencia.ParametrosOcorrencia.Where(o => o.TipoParametro == TipoParametroOcorrencia.Periodo).FirstOrDefault();

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
                {
                    CodigoCarga = carga.Codigo,
                    CodigoParametroBooleano = parametroBooleano?.Codigo ?? 0,
                    CodigoParametroInteiro = parametroInteiro?.Codigo ?? 0,
                    CodigoParametroPeriodo = parametroPeriodo?.Codigo ?? 0,
                    CodigoTipoOcorrencia = tipoOcorrencia.Codigo,
                    DataFim = DateTime.Now,
                    DataInicio = DateTime.Now,
                    Minutos = 0,
                    HorasSemFranquia = tipoOcorrencia?.HorasSemFranquia ?? 0,
                    KmInformado = 0,
                    PermiteInformarValor = false,
                    ValorOcorrencia = 0,
                    ListaCargaCTe = cargaCTEs
                };

                calculoFreteOcorrencia = servicoOcorrenciaCalculoFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
            }

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
            cargaOcorrencia.LoteAvaria = lote;
            cargaOcorrencia.DataOcorrencia = DateTime.Now;
            cargaOcorrencia.DataAlteracao = DateTime.Now;
            cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            if (cargaOcorrencia.Observacao is null)
                cargaOcorrencia.Observacao = "";

            cargaOcorrencia.ObservacaoCTe = "";
            cargaOcorrencia.ObservacaoCTes = "";
            cargaOcorrencia.Carga = carga;
            cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;
            cargaOcorrencia.OrigemOcorrencia = cargaOcorrencia.TipoOcorrencia.OrigemOcorrencia;
            cargaOcorrencia.ComponenteFrete = cargaOcorrencia.TipoOcorrencia?.ComponenteFrete;
            cargaOcorrencia.IncluirICMSFrete = cargaCTEs.Count > 0 ? (cargaCTEs.FirstOrDefault().CTe != null ? (cargaCTEs.FirstOrDefault().CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false) : false) : false;

            if (motivoAvaria.CalcularOcorrenciaPorTabelaFrete)
                cargaOcorrencia.ValorOcorrencia = calculoFreteOcorrencia?.ValorOcorrencia ?? 0;
            else
            {
                cargaOcorrencia.ValorOcorrencia = lote.ValorLote;
                cargaOcorrencia.ValorOcorrenciaOriginal = lote.ValorLote;
            }

            repCargaOcorrencia.Inserir(cargaOcorrencia);

            if (cargaOcorrencia.TipoOcorrencia.BloqueiaOcorrenciaDuplicada)
            {
                if (cargaOcorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    // Validacao de ocorrencia por periodo
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorPeriodo(cargaOcorrencia, out string erro, unitOfWork, this.Usuario))
                        throw new ControllerException(erro);
                }
                else
                {
                    // Validacao de ocorrencia por CTe
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, cargaOcorrencia, out string erro, unitOfWork, TipoServicoMultisoftware))
                        throw new ControllerException(erro);
                }
            }

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, cargaCTEs, unitOfWork);
            if (calculoFreteOcorrencia != null)
                GerarParametrosOcorrencia(cargaOcorrencia, lote, tipoOcorrencia, calculoFreteOcorrencia, false, unitOfWork);

            string mensagemRetorno = string.Empty;
            if (!srvOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, Usuario, configuracaoEmbarcador, Cliente, "", false, false, Auditado))
                throw new ControllerException(mensagemRetorno);

            repCargaOcorrencia.Atualizar(cargaOcorrencia);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, "Adicionou ocorrência pelo lote de avaria " + lote.Descricao, unitOfWork);
        }

        private void GerarParametrosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Avarias.Lote lote, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia, bool ocorrenciaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);


            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroBooleano = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                              where obj.TipoParametro == TipoParametroOcorrencia.Booleano
                                                                                              select obj).FirstOrDefault();

            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroInteiro = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                             where obj.TipoParametro == TipoParametroOcorrencia.Inteiro
                                                                                             select obj).FirstOrDefault();

            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroPeriodo = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                             where obj.TipoParametro == TipoParametroOcorrencia.Periodo
                                                                                             select obj).FirstOrDefault();

            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroTexto = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                           where obj.TipoParametro == TipoParametroOcorrencia.Texto
                                                                                           select obj).FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> parametrosData = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                                 where obj.TipoParametro == TipoParametroOcorrencia.Data
                                                                                                 select obj).ToList();

            int codigoParametroPeriodo = parametroPeriodo?.Codigo ?? 0;
            int codigoParametroInteiro = parametroInteiro?.Codigo ?? 0;
            int codigoParametroBooleano = parametroBooleano?.Codigo ?? 0;
            int codigoParametroData1 = parametrosData != null && parametrosData.Count == 1 ? parametrosData[0].Codigo : 0;
            int codigoParametroData2 = parametrosData != null && parametrosData.Count == 2 ? parametrosData[1].Codigo : 0;
            int codigoParametroTexto = parametroTexto?.Codigo ?? 0;

            string textoParametroTexto = lote.Descricao;
            int textoParametroInteiro = 0;

            decimal horasOcorrencia = !ocorrenciaDestino ? calculoFreteOcorrencia.HorasOcorrencia : calculoFreteOcorrencia.HorasOcorrenciaDestino;

            DateTime dataInicio = DateTime.MinValue;
            DateTime dataFim = DateTime.MinValue;
            DateTime parametroData1 = lote.DataGeracao;
            DateTime parametroData2 = DateTime.MinValue;

            if (codigoParametroPeriodo > 0 && dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametros = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroPeriodo),
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    TotalHoras = horasOcorrencia
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametros);
            }

            if (codigoParametroBooleano > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosBooleano = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroBooleano),
                    Booleano = false
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosBooleano);
            }

            if (codigoParametroInteiro > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosInteiro = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroInteiro),
                    Texto = textoParametroInteiro.ToString()
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosInteiro);
            }

            if (codigoParametroData1 > 0 && (parametroData1 > DateTime.MinValue))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosData1 = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroData1),
                    Data = parametroData1
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosData1);
            }

            if (codigoParametroData2 > 0 && parametroData2 > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosData2 = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroData2),
                    Data = parametroData2
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosData2);
            }

            if (codigoParametroTexto > 0 && !string.IsNullOrWhiteSpace(textoParametroTexto))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosTexto = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroTexto),
                    Texto = textoParametroTexto
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosTexto);
            }
        }

        #endregion
    }
}
