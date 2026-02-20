using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/AutorizacaoNFS")]
    public class AutorizacaoNFSController : BaseController
    {
        #region Construtores

        public AutorizacaoNFSController(Conexao conexao) : base(conexao) { }

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

                // Lista para preencher a grid
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaGrid, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaGrid, unitOfWork);

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

                // Lista para preencher o csv
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consulta
                ExecutaPesquisa(ref listaGrid, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaGrid, unitOfWork);

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
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                if (nfsManual == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
                var dynNFS = new
                {
                    nfsManual.Codigo,
                    nfsManual.DadosNFS.Numero,
                    Serie = nfsManual.DadosNFS.Serie.Numero,
                    NumeroCarga = nfsManual.Documentos.Select(x => x.Carga.CodigoCargaEmbarcador).FirstOrDefault(),
                    AliquotaISS = nfsManual.DadosNFS.AliquotaISS.ToString("n2"),
                    ValorISS = nfsManual.DadosNFS.ValorISS.ToString("n2"),
                    ValorBaseCalculo = nfsManual.DadosNFS.ValorBaseCalculo.ToString("n2"),
                    ValorRetido = nfsManual.DadosNFS.ValorRetido.ToString("n2"),
                    PercentualRetencao = nfsManual.DadosNFS.PercentualRetencao.ToString("n2"),
                    DataCriacao = nfsManual.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                    Situacao = nfsManual.DescricaoSituacao,
                    Transportador = nfsManual.Transportador?.RazaoSocial ?? string.Empty,
                    Filial = nfsManual.Filial?.Descricao ?? string.Empty,
                    Tomador = $"{nfsManual.Tomador?.Nome ?? string.Empty} {nfsManual.Tomador?.NomeFantasia ?? string.Empty}",
                    ValorFrete = nfsManual.DadosNFS.ValorFrete.ToString("n2"),
                    ValorFreteBruto = (nfsManual.DadosNFS.ValorDescontos > 0m) ? nfsManual.Documentos.Sum(obj => obj.ValorFrete).ToString("n2") : "",
                    Descontos = (nfsManual.DadosNFS.ValorDescontos > 0m) ? nfsManual.DadosNFS.ValorDescontos.ToString("n2") : "",
                    PossuiAnexo = !string.IsNullOrWhiteSpace(nfsManual.DadosNFS.AnexoNFS)
                };

                return new JsonpResult(dynNFS);
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
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

                // Buscas regras do usuario para essa ocorrencia
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> regras = repLancamentoNFSAutorizacao.BuscarPorNFSUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from lancamentoAutorizacao in regras
                             select new
                             {
                                 lancamentoAutorizacao.Codigo,
                                 Regra = lancamentoAutorizacao.RegrasAutorizacaoNFSManual.Descricao,
                                 Situacao = lancamentoAutorizacao.DescricaoSituacao,
                                 Usuario = lancamentoAutorizacao.Usuario.Nome,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repLancamentoNFSAutorizacao.VerificarSePodeAprovar(codigo, lancamentoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(lancamentoAutorizacao)
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
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigoNFS);

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> lancamentosAutorizacoes = repLancamentoNFSAutorizacao.BuscarPorNFSUsuarioSituacao(codigoNFS, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao autorizacao in lancamentosAutorizacoes)
                {
                    if (Servicos.Embarcador.NFSe.NFSManual.IsPermitirAprovacaoOuReprovacaoNFS(autorizacao, this.Usuario.Codigo))
                        Servicos.Embarcador.NFSe.NFSManual.EfetuarAprovacao(autorizacao, false, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);
                }

                // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(repLancamentoNFSManual.BuscarPorCodigo(codigoNFS), unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = lancamentosAutorizacoes.Count()
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

        public async Task<IActionResult> AprovarMultiplosLancamentos()
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

                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentos = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    lancamentos = ObterLancamentosSelecionados(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> lancamentosAutorizacoes = BuscarRegrasPorLancamentos(lancamentos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosLancamentosVerificados = new List<int>();

                // Aprova todas as regras
                foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao autorizacao in lancamentosAutorizacoes)
                {
                    if (Servicos.Embarcador.NFSe.NFSManual.IsPermitirAprovacaoOuReprovacaoNFS(autorizacao, this.Usuario.Codigo))
                    {
                        int codigo = autorizacao.LancamentoNFSManual.Codigo;

                        if (!codigosLancamentosVerificados.Contains(codigo))
                            codigosLancamentosVerificados.Add(codigo);

                        Servicos.Embarcador.NFSe.NFSManual.EfetuarAprovacao(autorizacao, false, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);
                    }
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosLancamentosVerificados)
                    // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                    Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(repLancamentoNFSManual.BuscarPorCodigo(cod), unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = lancamentosAutorizacoes.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os lançamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplosLancamentos()
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
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
                Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicao = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual servicoNFSManual = new Servicos.Embarcador.NFSe.NFSManual(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);
                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS justificativa = repMotivoRejeicao.BuscarPorCodigo(codigoJustificativa);

                // Valida justificativa (obrigatoria)
                if (justificativa == null)
                    return new JsonpResult(false, "Erro ao buscar justificativa.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentos = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    lancamentos = ObterLancamentosSelecionados(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> lancamentosAutorizacoes = BuscarRegrasPorLancamentos(lancamentos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                // Guarda os valores das ocorrencias para fazer a checagem geral
                List<int> codigosLancamentossVerificados = new List<int>();

                // Aprova todas as regras
                foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao lancamentosAutorizacao in lancamentosAutorizacoes)
                {
                    if (Servicos.Embarcador.NFSe.NFSManual.IsPermitirAprovacaoOuReprovacaoNFS(lancamentosAutorizacao, this.Usuario.Codigo))
                    {
                        int codigo = lancamentosAutorizacao.LancamentoNFSManual.Codigo;

                        if (!codigosLancamentossVerificados.Contains(codigo))
                            codigosLancamentossVerificados.Add(codigo);

                        // Metodo de rejeitar avaria
                        lancamentosAutorizacao.Data = DateTime.Now;
                        lancamentosAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada;
                        lancamentosAutorizacao.Motivo = motivo;
                        lancamentosAutorizacao.MotivoRejeicao = justificativa;
                        // Atualiza banco
                        repLancamentoNFSAutorizacao.Atualizar(lancamentosAutorizacao);
                    }
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosLancamentossVerificados)
                    // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                    Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(repLancamentoNFSManual.BuscarPorCodigo(cod), unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);

                await Task.Factory.StartNew(() => servicoNFSManual.EnviarEmailTransportadorReprovacaoNFS(codigosLancamentossVerificados));

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = lancamentosAutorizacoes.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar os lançamentos.");
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
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
                Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicao = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual servicoNFSManual = new Servicos.Embarcador.NFSe.NFSManual(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);
                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao lancamentosAutorizacao = repLancamentoNFSAutorizacao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS justificativa = repMotivoRejeicao.BuscarPorCodigo(codigoJustificativa);

                // Valida se é o usuario da regra
                if (lancamentosAutorizacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida justificativa (obrigatoria)
                if (justificativa == null)
                    return new JsonpResult(false, "Erro ao buscar justificativa.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (!Servicos.Embarcador.NFSe.NFSManual.IsPermitirAprovacaoOuReprovacaoNFS(lancamentosAutorizacao, this.Usuario.Codigo))
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                lancamentosAutorizacao.Data = DateTime.Now;
                lancamentosAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada;
                lancamentosAutorizacao.Motivo = motivo;
                lancamentosAutorizacao.MotivoRejeicao = justificativa;

                // Atualiza banco
                repLancamentoNFSAutorizacao.Atualizar(lancamentosAutorizacao);
                unitOfWork.Flush();

                // Verifica status gerais
                Servicos.Embarcador.NFSe.NFSManual.NotificarAlteracao(false, lancamentosAutorizacao.LancamentoNFSManual, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);
                Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(lancamentosAutorizacao.LancamentoNFSManual, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);
                await Task.Factory.StartNew(() => servicoNFSManual.EnviarEmailTransportadorReprovacaoNFS(new List<int>() { lancamentosAutorizacao.LancamentoNFSManual.Codigo }));

                unitOfWork.Clear();

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

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao lancamento = repLancamentoNFSAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (lancamento == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (!Servicos.Embarcador.NFSe.NFSManual.IsPermitirAprovacaoOuReprovacaoNFS(lancamento, this.Usuario.Codigo))
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                if (lancamento.LancamentoNFSManual.AlcadaComRequisito) //Alçada possui pré-requisito
                {
                    if (lancamento.RegrasAutorizacaoNFSManual.Requisito != null && !lancamento.LancamentoNFSManual.AlcadaComRequisitoAprovadas)
                        return new JsonpResult(false, "Pré-requisito não aprovado.");
                }

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                Servicos.Embarcador.NFSe.NFSManual.EfetuarAprovacao(lancamento, true, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);

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

        /* BuscarRegrasPorOcorrencias
         * Obtem todas regras do usuario relacionadas as ocorrencias
         */
        private List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> BuscarRegrasPorLancamentos(List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentos, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> lancamentoAutorizacao = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao>();

            // Itera todas as ocorrencias
            foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento in lancamentos)
            {
                // Busca as autorizacoes da ocorrencias
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> regras = repLancamentoNFSAutorizacao.BuscarPorNFSUsuarioSituacao(lancamento.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente);

                if (regras.Count > 0)
                    lancamentoAutorizacao.AddRange(regras);
            }

            // Retornas a lista com todas as autorizacao das ocorrencias
            return lancamentoAutorizacao;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Numero")
                propOrdena = "DadosNFS.Numero";
            else if (propOrdena == "Serie")
                propOrdena = "DadosNFS.Serie.Numero";
            else if (propOrdena == "Filial")
                propOrdena = "Filial.Descricao";
            else if (propOrdena == "Tomador")
                propOrdena = "Tomador.Nome";
            else if (propOrdena == "Transportador")
                propOrdena = "Transportador.RazaoSocial";
        }

        /* ExecutaPesquisa
         * Converte os valores vindo por POST 
         * E faz consulta de ocorrencias pendentes de aprovações
         */
        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("Transportador"), out int transportador);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            double.TryParse(Request.Params("Tomador"), out double tomador);

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacao);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);
            string codigoCargaEmbarcador = Request.Params("NumeroCarga");


            listaGrid = repLancamentoNFSAutorizacao.Consultar(numero, filial, transportador, tomador, usuario, situacao, dataInicial, dataFinal, codigoCargaEmbarcador, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repLancamentoNFSAutorizacao.ContarConsulta(numero, filial, transportador, tomador, usuario, situacao, dataInicial, dataFinal, codigoCargaEmbarcador);
        }

        /* ObterOcorrenciasSelecionadas
         * Duas maneiras de ocorrer a aprovacao em massa
         * - Selecionar todos (remove excecoes)
         * - Busca apenas selecionados
         */
        private List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> ObterLancamentosSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaGrid, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaLinhasNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionadas"));
                foreach (var dynLinhaNaoSelecionada in listaLinhasNaoSelecionadas)
                    listaGrid.Remove(new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual() { Codigo = (int)dynLinhaNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaLinhasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionadas"));
                foreach (var dynLinhaSelecionada in listaLinhasSelecionadas)
                    listaGrid.Add(repLancamentoNFSManual.BuscarPorCodigo((int)dynLinhaSelecionada.Codigo));
            }

            // Retorna lista
            return listaGrid;
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid, Repositorio.UnitOfWork unitOfWork)
        {
            var lista = from item in listaGrid
                        select new
                        {
                            item.Codigo,
                            Numero = item.DadosNFS?.Numero.ToString() ?? string.Empty,
                            Serie = item.DadosNFS?.Serie.Numero.ToString() ?? string.Empty,
                            DataCriacao = item.DataCriacao.ToString("dd/MM/yyyy"),
                            Filial = item.Filial?.Descricao ?? string.Empty,
                            Tomador = item.Tomador.NomeCNPJ,
                            Transportador = item.Transportador.RazaoSocial,
                            Situacao = item.DescricaoSituacao,
                            NumeroCarga = item.Documentos.FirstOrDefault()?.Carga.CodigoCargaEmbarcador ?? string.Empty
                        };

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Numero", "Numero", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data", "DataCriacao", 5, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.center, true);
            else
                grid.AdicionarCabecalho("Transportador", "Transportador", 40, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

            return grid;
        }

        /* CoresRegras
         * Retorna a cor da linha de acordo com a situacoa
         */
        private string CoresRegras(Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            else
                return "";
        }

        #endregion
    }
}
