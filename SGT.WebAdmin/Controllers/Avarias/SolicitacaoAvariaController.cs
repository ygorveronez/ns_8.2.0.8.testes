using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/SolicitacaoAvaria")]
    public class SolicitacaoAvariaController : BaseController
    {
        #region Construtores

        public SolicitacaoAvariaController(Conexao conexao) : base(conexao) { }

        #endregion

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
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (solicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos> anexos = repSolicitacaoAvariaAnexos.BuscarPorSolicitacao(solicitacaoAvaria.Codigo);

                // Verifica se usuario pode cancelar a Avaria
                bool podeCancelar = PermiteCancelar(solicitacaoAvaria, unitOfWork);

                // Formata retorno
                var retorno = new
                {
                    solicitacaoAvaria.Codigo,
                    NumeroAvaria = solicitacaoAvaria.NumeroAvaria,
                    DataAvaria = solicitacaoAvaria.DataAvaria.ToString("dd/MM/yyyy"),
                    DataSolicitacao = solicitacaoAvaria.DataSolicitacao.ToDateTimeString(),
                    Carga = solicitacaoAvaria.Carga != null ? new { Codigo = solicitacaoAvaria.Carga.Codigo, Descricao = solicitacaoAvaria.Carga.CodigoCargaEmbarcador } : null,
                    MotivoAvaria = solicitacaoAvaria.MotivoAvaria != null ? new { Codigo = solicitacaoAvaria.MotivoAvaria.Codigo, Descricao = solicitacaoAvaria.MotivoAvaria.Descricao } : null,
                    TipoOperacao = solicitacaoAvaria.Carga.TipoOperacao != null ? new { Codigo = solicitacaoAvaria.Carga.TipoOperacao.Codigo, Descricao = solicitacaoAvaria.Carga.TipoOperacao.Descricao } : null,
                    solicitacaoAvaria.Motorista,
                    solicitacaoAvaria.RGMotorista,
                    solicitacaoAvaria.CPFMotorista,
                    solicitacaoAvaria.Justificativa,
                    solicitacaoAvaria.Situacao,
                    PodeCancelar = podeCancelar,
                    Anexos = from obj in anexos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Descricao,
                                 obj.NomeArquivo,
                             },
                    Detalhes = DetalhasSolicitacao(solicitacaoAvaria, unitOfWork)
                };

                // Retorna informacoes
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

        [AllowAuthenticate]
        public async Task<IActionResult> ResumoSolicitacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (solicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    NumeroAvaria = solicitacaoAvaria.NumeroAvaria,
                    Viagem = solicitacaoAvaria.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    MotivoAvaria = solicitacaoAvaria.MotivoAvaria?.Descricao ?? string.Empty,
                    Solicitante = solicitacaoAvaria.Solicitante?.Nome ?? string.Empty,
                    Situacao = solicitacaoAvaria.DescricaoSituacao,
                    DataAvaria = solicitacaoAvaria.DataAvaria.ToString("dd/MM/yyyy"),
                    DataSolicitacao = solicitacaoAvaria.DataSolicitacao.ToDateTimeString(),
                    Transportador = solicitacaoAvaria.Transportador?.RazaoSocial ?? string.Empty,
                    Filial = solicitacaoAvaria.Carga.Filial?.Descricao ?? string.Empty,
                    Percurso = (solicitacaoAvaria.Carga.DadosSumarizados?.Origens + solicitacaoAvaria.Carga.DadosSumarizados?.Destinos) ?? string.Empty,
                    Veiculo = solicitacaoAvaria.Carga.Veiculo?.Placa,
                    Motorista = solicitacaoAvaria.Motorista,
                    ValorAvaria = solicitacaoAvaria.ValorAvaria.ToString("n2"),
                    ValorDesconto = solicitacaoAvaria.ValorDesconto.ToString("n2"),
                    TipoOperacao = solicitacaoAvaria.Carga.TipoOperacao?.Descricao ?? string.Empty,
                };

                // Retorna informacoes
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

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao solicitacaoAutorizacao = repSolicitacaoAvariaAutorizacao.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    solicitacaoAutorizacao.Codigo,
                    Regra = TituloRegra(solicitacaoAutorizacao),
                    Situacao = solicitacaoAutorizacao.DescricaoSituacao,
                    Usuario = solicitacaoAutorizacao.Usuario.Nome,

                    Data = solicitacaoAutorizacao.Data.HasValue ? solicitacaoAutorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Justificativa = solicitacaoAutorizacao.MotivoAvaria?.Descricao ?? string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(solicitacaoAutorizacao.Motivo) ? solicitacaoAutorizacao.Motivo : string.Empty,
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

        public async Task<IActionResult> ValorAvaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (solicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    ValorAvaria = solicitacaoAvaria.ValorAvaria.ToString("n2")
                };

                // Retorna informacoes
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria();

                PreencheEntidade(ref solicitacaoAvaria, unitOfWork);

                string erro;
                if (!ValidaEntidade(solicitacaoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                repCarga.Atualizar(solicitacaoAvaria.Carga);

                Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao();
                tempoEtapa.SolicitacaoAvaria = solicitacaoAvaria;
                tempoEtapa.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Criacao;
                tempoEtapa.Entrada = DateTime.Now;

                solicitacaoAvaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao;

                repSolicitacaoAvaria.Inserir(solicitacaoAvaria, Auditado);
                repTempoEtapaSolicitacao.Inserir(tempoEtapa, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoAvaria.Carga, null, "Tipo de Operação atualizado pela abertura da solicitação de avaria " + solicitacaoAvaria.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    solicitacaoAvaria.Codigo
                });
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

        public async Task<IActionResult> FinalizarSolicitacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string motorista = Request.Params("Motorista");
                if (string.IsNullOrWhiteSpace(motorista)) motorista = string.Empty;

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo, true);

                // Valida
                if (solicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacaoAvaria.ProdutosAvariados.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum produto cadastrado.");

                if (solicitacaoAvaria.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao)
                    return new JsonpResult(false, true, "A situação da solicitação (" + solicitacaoAvaria.DescricaoSituacao + ") não permite alterações.");

                // Altera a situacao
                solicitacaoAvaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao;
                solicitacaoAvaria.Motorista = motorista;

                // Valida entidade
                string erro;
                if (!ValidaEntidade(solicitacaoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Verifica regras de aprovacao
                VerificarRegrasAprovacao(ref solicitacaoAvaria, unitOfWork);

                // Fecha o tempo da e tapa e abre da nova etapa
                Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao fechaTempoEtapa = repTempoEtapaSolicitacao.BuscarUltimaEtapa(solicitacaoAvaria.Codigo);
                if (fechaTempoEtapa != null)
                {
                    fechaTempoEtapa.Initialize();
                    fechaTempoEtapa.Saida = DateTime.Now;
                    repTempoEtapaSolicitacao.Atualizar(fechaTempoEtapa, Auditado);
                }

                Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao();
                tempoEtapa.SolicitacaoAvaria = solicitacaoAvaria;
                tempoEtapa.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Autorizacao;
                tempoEtapa.Entrada = DateTime.Now;

                // Persiste dados
                repSolicitacaoAvaria.Atualizar(solicitacaoAvaria, Auditado);
                repTempoEtapaSolicitacao.Inserir(tempoEtapa, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoAvaria, null, "Finalizou a solicitação de avaria.", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    Codigo = solicitacaoAvaria.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = new Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados();

                // Preenche entidade com dados
                PreencheEntidadeProduto(ref produtoAvariado, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidadeProduto(produtoAvariado, out erro))
                    return new JsonpResult(false, true, erro);

                // Valida
                if (produtoAvariado.SolicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Erro ao adicionar produto (Solicitação não encontrada).");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao = produtoAvariado.SolicitacaoAvaria.Situacao;
                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Aberta && situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao && situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Todas)
                    return new JsonpResult(false, true, "A situação da solicitação não permite essa operação.");

                // Valida se esta cadastrado
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados duplicidade = repProdutosAvariados.BuscarPorSolicitacaoEProdutoEmbarcador(produtoAvariado.SolicitacaoAvaria.Codigo, produtoAvariado.ProdutoEmbarcador.Codigo);
                if (duplicidade != null)
                    return new JsonpResult(false, true, "Produto selecionado já consta na grade de produtos.");

                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados duplicidadeNota = repProdutosAvariados.BuscarPorSolicitacaoPorNumeroNotaECarga(produtoAvariado.SolicitacaoAvaria.Carga.Codigo, produtoAvariado.NotaFiscal);
                if (duplicidadeNota != null && duplicidadeNota.SolicitacaoAvaria.Codigo != produtoAvariado.SolicitacaoAvaria.Codigo)
                    return new JsonpResult(false, true, "Essa nota fiscal já foi lançada na avaria " + duplicidadeNota.SolicitacaoAvaria.NumeroAvaria + ".");

                // Atualiza o valor da avaria do produto
                AtualizaAvariaProduto(ref produtoAvariado, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoAvariado.SolicitacaoAvaria, null, "Adicionou o produto " + produtoAvariado.Descricao + ".", unitOfWork);

                repProdutosAvariados.Inserir(produtoAvariado, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
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

        public async Task<IActionResult> AtualizarProduto()
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

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorSolicitacaoEProduto(solicitacao, codigo);

                // Valida
                if (produtoAvariado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");
                produtoAvariado.Initialize();
                // Preenche entidade com dados
                PreencheEntidadeProduto(ref produtoAvariado, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidadeProduto(produtoAvariado, out erro))
                    return new JsonpResult(false, true, erro);

                // Valida
                if (produtoAvariado.SolicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Erro ao adicionar produto (Solicitação não encontrada).");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao = produtoAvariado.SolicitacaoAvaria.Situacao;
                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Aberta && situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao && situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Todas)
                    return new JsonpResult(false, true, "A situação da solicitação não permite essa operação.");

                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados duplicidadeNota = repProdutosAvariados.BuscarPorSolicitacaoPorNumeroNotaECarga(produtoAvariado.SolicitacaoAvaria.Carga.Codigo, produtoAvariado.NotaFiscal);
                if (duplicidadeNota != null && duplicidadeNota.SolicitacaoAvaria.Codigo != produtoAvariado.SolicitacaoAvaria.Codigo)
                    return new JsonpResult(false, true, "Essa nota fiscal já foi lançada na avaria " + duplicidadeNota.SolicitacaoAvaria.NumeroAvaria + ".");

                // Atualiza o valor da avaria do produto
                AtualizaAvariaProduto(ref produtoAvariado, unitOfWork);
                repProdutosAvariados.Atualizar(produtoAvariado, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoAvariado.SolicitacaoAvaria, null, "Atualizou o produto " + produtoAvariado.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirProduto()
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

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorSolicitacaoEProduto(solicitacao, codigo);

                // Valida
                if (produtoAvariado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, produtoAvariado.SolicitacaoAvaria, null, "Removeu o produto " + produtoAvariado.Descricao + ".", unitOfWork);
                repProdutosAvariados.Deletar(produtoAvariado, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorCodigo(codigo);

                // Valida
                if (produtoAvariado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                var retorno = new
                {
                    produtoAvariado.Codigo,
                    Produto = new { produtoAvariado.ProdutoEmbarcador.Codigo, produtoAvariado.ProdutoEmbarcador.Descricao },
                    produtoAvariado.CaixasAvariadas,
                    ValorInformadoOperador = produtoAvariado.ValorInformadoOperador.ToString("n2"),
                    produtoAvariado.UnidadesAvariadas,
                    produtoAvariado.NotaFiscal
                };

                // Retorna sucesso
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> TermoSolicitacaoAvaria()
        {
            try
            {
                var pdf = ReportRequest.WithType(ReportType.TermoAvaria)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoTermoAvaria", Request.Params("Codigo"))
                    .CallReport().GetContentFile();

                // Retorna o arquivo
                return Arquivo(pdf, "application/pdf", "Termo de Avaria.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o termo.");
            }
        }

        public async Task<IActionResult> ProdutosAvariados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridProdutos();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaProdutos(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaProdutos(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = repSolicitacaoAvaria.BuscarPorCodigo(codigo, true);

                // Valida
                if (solicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!PermiteCancelar(solicitacao, unitOfWork))
                    return new JsonpResult(false, true, "Não é possível cancelar a avaria.");

                // Fecha Etapa
                Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao fechaTempoEtapa = repTempoEtapaSolicitacao.BuscarUltimaEtapa(solicitacao.Codigo);
                if (fechaTempoEtapa != null)
                {
                    fechaTempoEtapa.Saida = DateTime.Now;
                    repTempoEtapaSolicitacao.Atualizar(fechaTempoEtapa);
                }

                // Seta situacao
                solicitacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Cancelada;
                repSolicitacaoAvaria.Atualizar(solicitacao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, null, "Cancelou a solicitação de avaria.", unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (solicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao)
                    return new JsonpResult(false, true, "A situação da avaria não permite essa operação.");

                // Etapa
                List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao> etapas = repTempoEtapaSolicitacao.BuscarPorSolicitacao(codigo);

                // Anexos
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos> anexos = repSolicitacaoAvariaAnexos.BuscarPorSolicitacao(codigo);

                // Deleta
                solicitacao.ProdutosAvariados.Clear();
                for (var i = 0; i < anexos.Count(); i++) repSolicitacaoAvariaAnexos.Deletar(anexos[i]);
                for (var i = 0; i < etapas.Count(); i++) repTempoEtapaSolicitacao.Deletar(etapas[i]);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, null, "Removeu a solicitação de avaria.", unitOfWork);
                repSolicitacaoAvaria.Deletar(solicitacao, Auditado);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, true);

                // Dados do filtro
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos> anexos = repSolicitacaoAvariaAnexos.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repSolicitacaoAvariaAnexos.ContarConsulta(codigo);
                var lista = from obj in anexos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.NomeArquivo
                            };

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

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Busca Ocorrencia
                int codigo = 0;
                int.TryParse(Request.Params("Solicitacao"), out codigo);

                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (solicitacao == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (solicitacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Aberta && solicitacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao)
                    return new JsonpResult(false, true, "Situação da solicitação não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    // Salva na pasta
                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    // Insere no banco
                    Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos solicitacaoAvariaAnexos = new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos();

                    solicitacaoAvariaAnexos.SolicitacaoAvaria = solicitacao;
                    solicitacaoAvariaAnexos.Descricao = i < descricoes.Length ? descricoes[i] : string.Empty; // Descrição vem numa lista separada
                    solicitacaoAvariaAnexos.GuidArquivo = guidArquivo;
                    solicitacaoAvariaAnexos.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, null, "Adicionou o arquivo " + solicitacaoAvariaAnexos.NomeArquivo + ".", unitOfWork);
                    repSolicitacaoAvariaAnexos.Inserir(solicitacaoAvariaAnexos, Auditado);
                }

                // Commita
                unitOfWork.CommitChanges();

                // Busca todos anexos
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos> anexosSolicitacao = repSolicitacaoAvariaAnexos.BuscarPorSolicitacao(solicitacao.Codigo);

                // Retorna arquivos
                var dynAnexos = from obj in anexosSolicitacao
                                select new
                                {
                                    obj.Codigo,
                                    obj.Descricao,
                                    obj.NomeArquivo
                                };

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos anexo = repSolicitacaoAvariaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirAnexao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos anexos = repSolicitacaoAvariaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.SolicitacaoAvaria.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Aberta && anexos.SolicitacaoAvaria.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao)
                    return new JsonpResult(false, "Situação da Solicitação não permite excluir arquivos.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos.SolicitacaoAvaria, null, "Removeu o arquivo " + anexos.NomeArquivo + ".", unitOfWork);
                repSolicitacaoAvariaAnexos.Deletar(anexos, Auditado);

                // Commita
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);

                int codOcorrencia = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Justificativa", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Etapa
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao;

                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> listaAutorizacao = repSolicitacaoAvariaAutorizacao.ConsultarAutorizacoesPorSolicitacaoEEtapa(codOcorrencia, etapa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSolicitacaoAvariaAutorizacao.ContarConsultaAutorizacoesPorSolicitacao(codOcorrencia, etapa));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = TituloRegra(obj),
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 Justificativa = obj.MotivoAvaria?.Descricao ?? string.Empty,
                                 DT_RowColor = CorAprovacao(obj.Situacao)
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRegrasEtapas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositoriso
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Converte parametros
                int codigo = 0;
                int.TryParse(Request.Params("Solicitacao"), out codigo);

                // Busca Ocorrencia
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Registro não encontrada.");

                // Verifica qual regras consultar
                bool atualizaAvaria = false;
                if (avaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraAprovacao)
                {
                    // Busca se ha regras e cria
                    if (VerificarRegrasAutorizacaoAvaria(avaria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao, TipoServicoMultisoftware, unitOfWork))
                    {
                        atualizaAvaria = true;
                        avaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao;
                    }
                }
                else if (avaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraLote)
                {
                    // Caso não tenha nenhum resposnavel, atualiza situacao
                    if (Servicos.Embarcador.Avarias.ResponsavelAvaria.CriaResponsavelSolicitacao(avaria, unitOfWork))
                        avaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgLote;
                    else
                        avaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraLote;

                    atualizaAvaria = true;
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, avaria, null, "Reconsultou as regras de aprovação da avaria.", unitOfWork);

                // Retorno de informacoes
                var retorno = new
                {
                    Situacao = avaria.Situacao
                };

                // Atualiza a ocorrencia
                if (atualizaAvaria)
                    repSolicitacaoAvaria.Atualizar(avaria);
                else
                    retorno = null;

                // Finaliza instancia
                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LoteSolicitacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Busca
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                if (avaria == null || avaria.Lote == null)
                    return new JsonpResult(new
                    {
                        PossuiLote = false
                    });

                return new JsonpResult(new
                {
                    PossuiLote = true,
                    SituacaoLote = avaria.Lote.Situacao
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados do lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Lote", "Lote", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Responsaveis", "Responsaveis", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Etapa", "Etapa", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Criador", "Criador", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Total do Lote", "ValorLote", 20, Models.Grid.Align.right, false);

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

                // Busca
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes = new List<Dominio.Entidades.Embarcador.Avarias.Lote>();

                if (avaria != null && avaria.Lote != null)
                    lotes.Add(avaria.Lote);

                int totalRegistros = lotes.Count();
                var lista = from obj in lotes
                            select new
                            {
                                obj.Codigo,
                                Lote = obj.Numero.ToString(),
                                Responsaveis = String.Join(", ", (from r in repResponsavelAvaria.ResponsavelLote(obj.Codigo) select r.Nome).ToArray()),
                                Etapa = obj.DescricaoEtapa,
                                Criador = obj.Criador.Nome,
                                ValorLote = obj.ValorLote.ToString("n2")
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados do lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private bool VerificarRegrasAutorizacaoAvaria(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada = Servicos.Embarcador.Avarias.AutorizacaoSolicitacaoAvaria.VerificarRegrasAutorizacaoAvaria(avaria, etapaAutorizacaoAvaria, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.Avarias.AutorizacaoSolicitacaoAvaria.CriarRegrasAutorizacao(listaFiltrada, avaria, this.Usuario, tipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                return true;
            }

            return false;
        }

        private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao situacao)
        {
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "SolicitacaoAvaria");

            return caminho;
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            // Converte valores
            int codigoCarga = 0;
            int.TryParse(Request.Params("Carga"), out codigoCarga);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            int codigoMotivoAvaria = 0;
            int.TryParse(Request.Params("MotivoAvaria"), out codigoMotivoAvaria);
            Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivo = repMotivoAvaria.BuscarPorCodigo(codigoMotivoAvaria);

            string justificativa = Request.Params("Justificativa");
            if (string.IsNullOrWhiteSpace(justificativa)) justificativa = string.Empty;

            string motorista = Request.Params("Motorista");
            if (string.IsNullOrWhiteSpace(motorista)) motorista = string.Empty;

            string RGmotorista = Request.Params("RGMotorista");
            if (string.IsNullOrWhiteSpace(RGmotorista)) RGmotorista = string.Empty;

            string rgMotorista = Request.Params("RGMotorista");
            if (string.IsNullOrWhiteSpace(rgMotorista)) rgMotorista = string.Empty;

            string cpfMotorista = Request.Params("CPFMotorista");
            if (string.IsNullOrWhiteSpace(cpfMotorista)) cpfMotorista = string.Empty;

            DateTime dataAvaria;
            DateTime.TryParseExact(Request.Params("DataAvaria"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAvaria);

            // Vincula dados
            solicitacaoAvaria.MotivoAvaria = motivo;
            solicitacaoAvaria.Justificativa = justificativa;
            solicitacaoAvaria.DataAvaria = dataAvaria;
            solicitacaoAvaria.Transportador = carga.Empresa;
            solicitacaoAvaria.Carga = carga;

            solicitacaoAvaria.MotoristaOriginal = carga?.Motoristas.FirstOrDefault()?.Nome ?? string.Empty;
            solicitacaoAvaria.RGMotoristaOriginal = carga?.Motoristas.FirstOrDefault()?.RG ?? string.Empty;
            solicitacaoAvaria.Motorista = motorista;
            solicitacaoAvaria.RGMotorista = RGmotorista;
            solicitacaoAvaria.CPFMotorista = cpfMotorista;

            if (solicitacaoAvaria.Motorista != solicitacaoAvaria.MotoristaOriginal) solicitacaoAvaria.MotoristaModificado = true;
            if (solicitacaoAvaria.RGMotorista != solicitacaoAvaria.RGMotoristaOriginal) solicitacaoAvaria.RGMotoristaModificado = true;

            solicitacaoAvaria.DataSolicitacao = DateTime.Now;
            solicitacaoAvaria.Solicitante = this.Usuario;
            solicitacaoAvaria.NumeroAvaria = repSolicitacaoAvaria.BuscarProximoCodigo();
        }

        private void PreencheEntidadeProduto(ref Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = repSolicitacaoAvaria.BuscarPorCodigo(Request.GetIntParam("Solicitacao"));
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.BuscarPorCodigo(Request.GetIntParam("Produto"));

            produtoAvariado.CaixasAvariadas = Request.GetIntParam("CaixasAvariadas");
            produtoAvariado.UnidadesAvariadas = Request.GetIntParam("UnidadesAvariadas");
            produtoAvariado.NotaFiscal = Request.GetStringParam("NotaFiscal");
            produtoAvariado.ProdutoAvariado = produtoAvariado.UnidadesAvariadas > 0 || produtoAvariado.CaixasAvariadas > 0;
            produtoAvariado.ValorInformadoOperador = Request.GetDecimalParam("ValorInformadoOperador");

            if (produtoAvariado.Codigo == 0)
            {
                produtoAvariado.SolicitacaoAvaria = solicitacao;
                produtoAvariado.ProdutoEmbarcador = produto;
            }
        }


        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria, out string msgErro)
        {
            msgErro = "";

            if (solicitacaoAvaria.Carga == null)
            {
                msgErro = "Viagem é obrigatória.";
                return false;
            }

            if (solicitacaoAvaria.DataAvaria == DateTime.MinValue)
            {
                msgErro = "Data da Avaria é obrigatória.";
                return false;
            }

            if (solicitacaoAvaria.MotivoAvaria == null)
            {
                msgErro = "Motivo da Avaria é obrigatório.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(solicitacaoAvaria.Motorista))
            {
                msgErro = "Motorista é obrigatório.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(solicitacaoAvaria.CPFMotorista))
            {
                msgErro = "CPF do Motorista é obrigatório.";
                return false;
            }

            if (solicitacaoAvaria.CPFMotorista.Length != 14)
            {
                msgErro = "CPF do Motorista é inválido.";
                return false;
            }

            return true;
        }

        private bool ValidaEntidadeProduto(Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado, out string msgErro)
        {
            msgErro = "";

            if (produtoAvariado.ProdutoEmbarcador == null)
            {
                msgErro = "Produto é obrigatória.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            // Dados do filtro
            int numeroAvaria = 0;
            int.TryParse(Request.Params("NumeroAvaria"), out numeroAvaria);
            int transportadora = 0;
            int.TryParse(Request.Params("Transportadora"), out transportadora);
            int motivoAvaria = 0;
            int.TryParse(Request.Params("MotivoAvaria"), out motivoAvaria);
            int numeroNota = 0;
            int.TryParse(Request.Params("NumeroNota"), out numeroNota);

            DateTime dataInicio;
            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
            DateTime dataFim;
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

            string carga = Request.Params("Carga");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao;
            Enum.TryParse(Request.Params("SituacaoAvaria"), out situacao);

            // Filtro MultiCTe
            int solicitante = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                solicitante = this.Usuario.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaGrid = repSolicitacaoAvaria.Consultar(solicitante, numeroAvaria, dataInicio, dataFim, transportadora, motivoAvaria, carga, situacao, propOrdenar, dirOrdena, inicio, limite, numeroNota);
            totalRegistros = repSolicitacaoAvaria.ContarConsulta(solicitante, numeroAvaria, dataInicio, dataFim, transportadora, motivoAvaria, carga, situacao, numeroNota);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            NumeroAvaria = obj.NumeroAvaria,
                            DataAvaria = obj.DataAvaria.ToString("dd/MM/yyyy"),
                            DataSolicitacao = obj.DataSolicitacao.ToDateTimeString(),
                            Viagem = obj.Carga.CodigoCargaEmbarcador,
                            MotivoAvaria = obj.MotivoAvaria.Descricao,
                            Solicitante = obj.Solicitante.Nome,
                            DescricaoSituacao = obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaProdutos(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            // Dados do filtro
            int codigo = 0;
            int.TryParse(Request.Params("Codigo"), out codigo);

            string descricao = Request.Params("DescricaoProduto");

            // Consulta produtos da solicitacao
            List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> listaGrid = repProdutosAvariados.BuscarPorSolicitacao(codigo, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProdutosAvariados.ContarBuscaPorSolicitacao(codigo, descricao);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Produto = obj.ProdutoEmbarcador.Descricao,
                            CaixasAvariadas = obj.CaixasAvariadas,
                            UnidadesAvariadas = obj.UnidadesAvariadas,
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Viagem") propOrdenar = "Carga.CodigoCargaEmbarcador";
            else if (propOrdenar == "MotivoAvaria") propOrdenar += ".Descricao";
            else if (propOrdenar == "Solicitante") propOrdenar += ".Nome";
            else if (propOrdenar == "DescricaoSituacao") propOrdenar = "Situacao";
        }
        private void PropOrdenaProdutos(ref string propOrdenar)
        {
            if (propOrdenar == "Produto") propOrdenar = "ProdutoEmbarcador.Descricao";
            else if (propOrdenar == "DescricaoUnidade") propOrdenar = "Unidade";
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "NumeroAvaria", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "DataAvaria", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Solicitação", "DataSolicitacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Viagem", "Viagem", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motivo", "MotivoAvaria", 15, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Solicitante", false);
            else
                grid.AdicionarCabecalho("Solicitante", "Solicitante", 15, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private Models.Grid.Grid GridProdutos()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cx Avariadas", "CaixasAvariadas", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Un Avariadas", "UnidadesAvariadas", 7, Models.Grid.Align.right, true);

            return grid;
        }

        private void VerificarRegrasAprovacao(ref Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada = Servicos.Embarcador.Avarias.AutorizacaoSolicitacaoAvaria.VerificarRegrasAutorizacaoAvaria(solicitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.Avarias.AutorizacaoSolicitacaoAvaria.CriarRegrasAutorizacao(listaFiltrada, solicitacao, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
            }
            else
            {
                solicitacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraAprovacao;
            }
        }

        private void AtualizaAvariaProduto(ref Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.ProdutoAvaria repProdutoAvaria = new Repositorio.Embarcador.Avarias.ProdutoAvaria(unitOfWork);

            Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria produto = repProdutoAvaria.BuscarPorProduto(produtoAvariado.ProdutoEmbarcador.Codigo);

            if (produto == null)
                return;

            if (produtoAvariado.ValorInformadoOperador > 0)
            {
                produtoAvariado.ValorAvaria = produtoAvariado.ValorInformadoOperador;
                return;
            }

            int qtdUnidades = produtoAvariado.UnidadesAvariadas;

            if (produtoAvariado.CaixasAvariadas > 0)
                qtdUnidades += produtoAvariado.CaixasAvariadas * produto.QuantidadeCaixa;

            decimal valorAvaria = qtdUnidades * produto.CustoPrimario;

            produtoAvariado.ValorAvaria = valorAvaria;
            produtoAvariado.CustoPrimario = produto.CustoPrimario;
        }

        private dynamic DetalhasSolicitacao(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria, Repositorio.UnitOfWork unitOfWork)
        {
            // Verifica situacao
            if (
                solicitacaoAvaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao ||
                solicitacaoAvaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraAprovacao ||
                solicitacaoAvaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Cancelada
                )
                return null;

            // Respositorios
            Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repSolicitacaoAvariaAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            return new
            {
                Solicitante = solicitacaoAvaria.Solicitante.Nome,
                DataSolicitacao = solicitacaoAvaria.DataSolicitacao.ToString("dd/MM/yyyy"),
                NumeroAprovadores = repSolicitacaoAvariaAutorizacao.ContarAutorizacaoPorSolicitacao(solicitacaoAvaria.Codigo),
                Aprovacoes = repSolicitacaoAvariaAutorizacao.ContarAprovacoesPorSolicitacao(solicitacaoAvaria.Codigo),
                Reprovacoes = repSolicitacaoAvariaAutorizacao.ContarReprovacoesPorSolicitacao(solicitacaoAvaria.Codigo),
                Situacao = solicitacaoAvaria.DescricaoSituacao,
                ResponsavelLote = String.Join(", ", (from o in repResponsavelAvaria.BuscaPorSolicitacao(solicitacaoAvaria.Codigo) select o.Nome))
            };
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao regra)
        {
            if (regra.OrigemRegraAvaria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Delegada)
                return "(Delegado)";
            else
                return regra.RegrasAutorizacaoAvaria?.Descricao;
        }

        private bool PermiteCancelar(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            if (solicitacaoAvaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgLote && (repResponsavelAvaria.BuscaPorUsuarioAvaria(solicitacaoAvaria.Codigo, this.Usuario.Codigo) != null))
                return true;
            if (solicitacaoAvaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraLote || solicitacaoAvaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.SemRegraAprovacao)
                return true;

            return false;
        }
        #endregion
    }
}
