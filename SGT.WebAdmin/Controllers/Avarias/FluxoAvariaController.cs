using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/FluxoAvaria")]
    public class FluxoAvariaController : BaseController
    {
        #region Construtores

        public FluxoAvariaController(Conexao conexao) : base(conexao) { }

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
                var repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                var repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                var solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (solicitacaoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var anexos = repSolicitacaoAvariaAnexos.BuscarPorSolicitacao(solicitacaoAvaria.Codigo);

                // Verifica se usuario pode cancelar a Avaria
                var podeCancelar = PermiteCancelar(solicitacaoAvaria, unitOfWork);
                var isDesabilitarBotaoTermo = solicitacaoAvaria.MotivoAvaria?.DesabilitarBotaoTermo != null ? solicitacaoAvaria.MotivoAvaria?.DesabilitarBotaoTermo : false;

                // Formata retorno
                var retorno = new
                {
                    solicitacaoAvaria.Codigo,
                    NumeroAvaria = solicitacaoAvaria.NumeroAvaria,
                    DataAvaria = solicitacaoAvaria.DataAvaria.ToString("dd/MM/yyyy"),
                    Carga = solicitacaoAvaria.Carga != null ? new { Codigo = solicitacaoAvaria.Carga.Codigo, Descricao = solicitacaoAvaria.Carga.CodigoCargaEmbarcador } : null,
                    TipoOperacao = solicitacaoAvaria.Carga.TipoOperacao != null ? new { Codigo = solicitacaoAvaria.Carga.TipoOperacao.Codigo, Descricao = solicitacaoAvaria.Carga.TipoOperacao.Descricao } : null,
                    solicitacaoAvaria.Motorista,
                    solicitacaoAvaria.RGMotorista,
                    solicitacaoAvaria.CPFMotorista,
                    solicitacaoAvaria.Justificativa,
                    solicitacaoAvaria.Situacao,
                    solicitacaoAvaria.SituacaoFluxo,
                    solicitacaoAvaria.CentroResultado,
                    Lote = solicitacaoAvaria?.Lote?.Codigo ?? 0,
                    PodeCancelar = podeCancelar,
                    MotivoAvaria = solicitacaoAvaria.MotivoAvaria != null
                        ? new
                        {
                            Codigo = solicitacaoAvaria.MotivoAvaria.Codigo,
                            Descricao = solicitacaoAvaria.MotivoAvaria.Descricao,
                            DesabilitarBotaoTermo = isDesabilitarBotaoTermo
                        }
                        : null,
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
                    Viagem = solicitacaoAvaria.Carga?.CodigoCargaEmbarcador ?? "-",
                    MotivoAvaria = solicitacaoAvaria.MotivoAvaria?.Descricao ?? "-",
                    Solicitante = solicitacaoAvaria.Solicitante?.Nome ?? "-",
                    Situacao = solicitacaoAvaria.DescricaoSituacaoFluxo,
                    DataAvaria = solicitacaoAvaria.DataAvaria.ToString("dd/MM/yyyy"),
                    Transportador = solicitacaoAvaria.Transportador?.RazaoSocial ?? "-",
                    Filial = solicitacaoAvaria.Carga.Filial?.Descricao ?? "-",
                    Percurso = (solicitacaoAvaria.Carga.DadosSumarizados?.Origens + solicitacaoAvaria.Carga.DadosSumarizados?.Destinos) ?? "-",
                    Veiculo = solicitacaoAvaria.Carga.Veiculo?.Placa,
                    Motorista = solicitacaoAvaria.Motorista,
                    ValorAvaria = solicitacaoAvaria.ValorAvaria.ToString("n2"),
                    ValorDesconto = solicitacaoAvaria.ValorDesconto.ToString("n2"),
                    TipoOperacao = solicitacaoAvaria.Carga.TipoOperacao?.Descricao ?? "-",
                    Lote = solicitacaoAvaria?.Lote?.Numero ?? 0,
                    CentroResultado = solicitacaoAvaria.CentroResultado?.Descricao ?? "-"
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                var repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                var repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
                var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Busca informacoes
                var solicitacaoAvaria = new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria();

                // Preenche entidade com dados
                PreencheEntidade(ref solicitacaoAvaria, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(solicitacaoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Valida se esta cadastrado
                if (solicitacaoAvaria?.MotivoAvaria?.Codigo != 0)
                {
                    var motivoAvaria = repMotivoAvaria.BuscarPorCodigo(solicitacaoAvaria.MotivoAvaria.Codigo);

                    if (motivoAvaria.NaoPermitirAberturaAvariasMesmoMotivoECarga)
                    {
                        Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria duplicidade = repSolicitacaoAvaria.BuscarPorCargaEMotivo(solicitacaoAvaria.Carga.Codigo, solicitacaoAvaria.MotivoAvaria.Codigo);
                        if (duplicidade != null)
                            return new JsonpResult(false, true, "Processo Abortado! Já existe avaria para essa viagem e motivo.");
                    }
                }

                // Atualiza tipo operação da carga
                repCarga.Atualizar(solicitacaoAvaria.Carga);

                // Cria o controle de tempo
                Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao();
                tempoEtapa.SolicitacaoAvaria = solicitacaoAvaria;
                tempoEtapa.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Criacao;
                tempoEtapa.Entrada = DateTime.Now;

                // Situacao
                //solicitacaoAvaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.EmCriacao;
                solicitacaoAvaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos;

                // Persiste dados
                repSolicitacaoAvaria.Inserir(solicitacaoAvaria, Auditado);
                repTempoEtapaSolicitacao.Inserir(tempoEtapa, Auditado);

                // Audita atualizacao da carga
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoAvaria.Carga, null, "Tipo de Operação atualizado pela abertura da solicitação de avaria " + solicitacaoAvaria.Descricao + ".", unitOfWork);

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
                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Avaria");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo, true);

                // Buscar Parametro Motivo Avaria
                bool PermitirInformarQuantidadeMaiorMercadoriaAvariada = false;
                if (solicitacaoAvaria?.MotivoAvaria?.Codigo != 0)
                {
                    var motivoAvaria = repMotivoAvaria.BuscarPorCodigo(solicitacaoAvaria.MotivoAvaria.Codigo);
                    if (motivoAvaria != null)
                        PermitirInformarQuantidadeMaiorMercadoriaAvariada = motivoAvaria.PermitirInformarQuantidadeMaiorMercadoriaAvariada;
                }

                // Validações ---------------------------------------
                if (solicitacaoAvaria == null)
                    throw new ServicoException("Não foi possível encontrar o registro da avaria.");

                if (solicitacaoAvaria.ProdutosAvariados.Count() == 0)
                    throw new ServicoException("Nenhum produto cadastrado.");

                if (solicitacaoAvaria.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos)
                    throw new ServicoException("A situação da solicitação não permite alterações.");

                List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtos = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork).BuscarPorSolicitacao(codigo, "", "Codigo", "asc", 0, 0);

                foreach (Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto in produtos)
                {
                    if (produto.CaixasAvariadas > produto.ProdutoNotaFiscal.Quantidade && !PermitirInformarQuantidadeMaiorMercadoriaAvariada)
                        throw new ServicoException("Produto com quantidade informada maior que a quantidade da nota.");
                    if (produto.UnidadesAvariadas > produto.ProdutoNotaFiscal.Quantidade * ((produto?.ProdutoNotaFiscal.Produto.FatorConversao ?? 0) > 0 ? produto.ProdutoNotaFiscal.Produto.FatorConversao : 1))
                        throw new ServicoException("Produto com unidades informada maior que a quantidade da nota com fator de conversão.");
                    if (produto.ValorAvaria > produto.ProdutoNotaFiscal.ValorTotal)
                        throw new ServicoException("Produto com valor informada maior que o valor da nota.");
                }
                //---------------------------------------------------

                // Alterações ---------------------------------------
                // Altera a situacao
                //solicitacaoAvaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgAprovacao;
                solicitacaoAvaria.DataSolicitacao = DateTime.Now;

                // Valida entidade
                string erro;
                if (!ValidaEntidade(solicitacaoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Fecha o tempo da etapa e abre da nova etapa
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

                solicitacaoAvaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao;



                foreach (Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto in produtos)
                {
                    if (produto.GeraEstoque)
                    {
                        if (produto.LocalArmazenamento == null)
                            throw new ServicoException("O produto " + produto.ProdutoEmbarcador.Descricao + " está marcado para gerar estoque mas não possui local de armazenamento informado.");

                        Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote produtoLote = repProdutoEmbarcadorLote.BuscarPorProduto(produto.ProdutoEmbarcador.Codigo);
                        if (produtoLote != null)
                        {
                            produtoLote.QuantidadeAtual += produto.UnidadesAvariadas;
                            repProdutoEmbarcadorLote.Atualizar(produtoLote, Auditado);
                        }
                        else
                        {
                            produtoLote = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote();
                            produtoLote.ProdutoEmbarcador = produto.ProdutoEmbarcador;
                            produtoLote.QuantidadeAtual = produto.UnidadesAvariadas;
                            repProdutoEmbarcadorLote.Inserir(produtoLote, Auditado);
                        }
                    }
                }

                // Verifica regras de aprovacao
                VerificarRegrasAprovacao(ref solicitacaoAvaria, unitOfWork);

                // Persiste dados
                repSolicitacaoAvaria.Atualizar(solicitacaoAvaria, Auditado);
                repTempoEtapaSolicitacao.Inserir(tempoEtapa, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoAvaria, null, "Finalizou a solicitação de avaria.", unitOfWork);
                unitOfWork.CommitChanges();
                //---------------------------------------------------

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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
                int codigo = Request.GetIntParam("Codigo");
                List<int> notas = Request.GetListParam<int>("Notas");
                List<int> notasCadastradas = new List<int>();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork).BuscarPorCodigo(codigo);

                if (solicitacao == null)
                    return new JsonpResult(false, true, "Falha ao consultar avaria para inserção dos produtos");

                // Dominio
                List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtosAvariados = repProdutosAvariados.BuscarProdutosSolicitacao(codigo);

                var notasFaltantes = notas.Where(x => !produtosAvariados.Select(y => y.ProdutoNotaFiscal.XMLNotaFiscal.Codigo).Contains(x)).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosNotas = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(notasFaltantes);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produto in produtosNotas)
                {
                    Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = new Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados();
                    produtoAvariado.SolicitacaoAvaria = solicitacao;
                    produtoAvariado.ProdutoNotaFiscal = produto;
                    produtoAvariado.ProdutoEmbarcador = produto.Produto;
                    produtoAvariado.ValorInformadoOperador = produto.ValorTotal;
                    produtoAvariado.ValorAvaria = produto.ValorTotal;
                    produtoAvariado.CaixasAvariadas = (int)produto.Quantidade;
                    produtoAvariado.UnidadesAvariadas = (int)(produto.Quantidade * ((produto.Produto?.FatorConversao ?? 0) > 0 ? produto.Produto?.FatorConversao : 1));
                    produtoAvariado.ProdutoAvariado = produtoAvariado.UnidadesAvariadas > 0;
                    produtoAvariado.NotaFiscal = produto.XMLNotaFiscal.Numero.ToString();


                    repProdutosAvariados.Inserir(produtoAvariado, Auditado);
                }

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

        public async Task<IActionResult> AtualizarCamposProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                int avaria = Request.GetIntParam("Avaria");
                int quantidade = Request.GetIntParam("Quantidade");
                int unidades = Request.GetIntParam("Unidades");
                decimal valor = Request.GetDecimalParam("Valor");
                bool geraEstqoue = Request.GetBoolParam("GeraEstoque");

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = repSolicitacaoAvaria.BuscarPorCodigo(avaria, true);
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorSolicitacaoEProduto(avaria, codigo);

                if (produtoAvariado == null)
                    throw new ServicoException("Não foi possível encontrar o produto para atulização dos valores.");

                bool PermitirInformarQuantidadeMaiorMercadoriaAvariada = false;
                if (solicitacaoAvaria?.MotivoAvaria?.Codigo != 0)
                {
                    var motivoAvaria = repMotivoAvaria.BuscarPorCodigo(solicitacaoAvaria.MotivoAvaria.Codigo);
                    if (motivoAvaria != null)
                        PermitirInformarQuantidadeMaiorMercadoriaAvariada = motivoAvaria.PermitirInformarQuantidadeMaiorMercadoriaAvariada;
                }

                if (quantidade > produtoAvariado.ProdutoNotaFiscal.Quantidade && !PermitirInformarQuantidadeMaiorMercadoriaAvariada)
                    throw new ServicoException("Não é permitido informar quantidade maior do que a presente na nota fiscal no produto.");

                if (unidades > produtoAvariado.ProdutoNotaFiscal.Quantidade * ((produtoAvariado?.ProdutoNotaFiscal.Produto.FatorConversao ?? 0) > 0 ? produtoAvariado.ProdutoNotaFiscal.Produto.FatorConversao : 1))
                    throw new ServicoException("Não é permitido informar unidades maior do que a quantidade presente na nota fiscal multiplicada pelo fator de conversão no produto.");

                if (valor > produtoAvariado.ProdutoNotaFiscal.ValorTotal)
                    throw new ServicoException("Não é permitido informar valor maior do que a presente na nota fiscal no produto.");

                produtoAvariado.CaixasAvariadas = quantidade;
                produtoAvariado.UnidadesAvariadas = unidades;
                produtoAvariado.ValorAvaria = valor;
                produtoAvariado.ValorInformadoOperador = produtoAvariado.ValorAvaria;
                produtoAvariado.GeraEstoque = geraEstqoue;
                repProdutosAvariados.Atualizar(produtoAvariado, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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
                int codigoProduto = Request.GetIntParam("Codigo");
                int avaria = Request.GetIntParam("Avaria");

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorSolicitacaoEProduto(avaria, codigoProduto);

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

        public async Task<IActionResult> ExcluirNota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                int nota = Request.GetIntParam("Nota");

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtosNota = repProdutosAvariados.BuscarPorSolicitacaoENota(codigo, nota);

                // Valida
                if (produtosNota != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produto in produtosNota)
                    {
                        repProdutosAvariados.Deletar(produto, Auditado);
                    }
                }

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

        public async Task<IActionResult> ConfirmarEtapaTermo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                avaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgLote;

                repSolicitacaoAvaria.Atualizar(avaria, Auditado);

                // Retorna o arquivo
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o termo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProdutosAvariadosGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridProdutos(unitOfWork);

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

        public async Task<IActionResult> BuscarNotasProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaNotasProdutos(ref totalRegistros, "Codigo", null, 0, 0, unitOfWork);

                return new JsonpResult(lista);
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

        public async Task<IActionResult> AtualizarLocalArmazenamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                int codigoAvaria = Request.GetIntParam("CodigoAvaria");
                int LocalArmazenamento = Request.GetIntParam("LocalArmazenamento");

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorSolicitacaoEProduto(codigoAvaria, codigo);
                Dominio.Entidades.Embarcador.WMS.Deposito localArmazenamento = new Repositorio.Embarcador.WMS.Deposito(unitOfWork).BuscarPorCodigo(LocalArmazenamento);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork).BuscarPorCodigo(codigoAvaria);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro da avaria.");
                if (produtoAvariado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro do produto avariado.");
                if (localArmazenamento == null)
                    return new JsonpResult(false, true, "Erro ao buscar local de armazenamento.");
                if (avaria.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos)
                    return new JsonpResult(false, true, "A situação da avaria não permite esta ação.");
                produtoAvariado.Initialize();

                produtoAvariado.LocalArmazenamento = localArmazenamento;
                repProdutosAvariados.Atualizar(produtoAvariado, Auditado);

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

        public async Task<IActionResult> AtualizarProdutoEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                int codigoAvaria = Request.GetIntParam("CodigoAvaria");
                int ProdutoEmbarcador = Request.GetIntParam("ProdutoEmbarcador");

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                // Dominio
                Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados produtoAvariado = repProdutosAvariados.BuscarPorSolicitacaoEProduto(codigoAvaria, codigo);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork).BuscarPorCodigo(ProdutoEmbarcador);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork).BuscarPorCodigo(codigoAvaria);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro da avaria.");
                if (produtoAvariado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro do produto avariado.");
                if (produtoEmbarcador == null)
                    return new JsonpResult(false, true, "Erro ao buscar local de armazenamento.");
                if (avaria.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos)
                    return new JsonpResult(false, true, "A situação da avaria não permite esta ação.");
                produtoAvariado.Initialize();

                produtoAvariado.ProdutoEmbarcador = produtoEmbarcador;
                repProdutosAvariados.Atualizar(produtoAvariado, Auditado);

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
                solicitacao.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Cancelado;
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

                if (solicitacao.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Dados && solicitacao.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos)
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

                if (solicitacao.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Dados && solicitacao.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos)
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

        public async Task<IActionResult> ValidarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                int qtdAnexo = Request.GetIntParam("qtdAnexo");
                int codigoMotivoAvaria = Request.GetIntParam("codigoMotivoAvaria");

                if (codigoMotivoAvaria != 0)
                {
                    var motivoAvaria = repMotivoAvaria.BuscarPorCodigo(codigoMotivoAvaria);

                    if ((motivoAvaria?.ObrigarAnexo ?? false) && qtdAnexo == 0)
                        return new JsonpResult(false, "Processo Abortado! Obrigatório adicionar anexos.");
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar os anexos.");
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

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos repSolicitacaoAvariaAnexos = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = Request.GetIntParam("Codigo");
                int codigoAvaria = Request.GetIntParam("Avaria");
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos anexos = repSolicitacaoAvariaAnexos.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork).BuscarPorCodigo(codigoAvaria);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, "Erro ao buscar avaria.");

                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar o arquivo anexado.");

                if (avaria.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Dados && avaria.SituacaoFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Produtos)
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
                int codigo = Request.GetIntParam("Avaria");

                // Busca Ocorrencia
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Registro não encontrada.");

                // Verifica qual regras consultar
                bool atualizaAvaria = false;
                if (avaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.SemRegraAprovacao)
                {
                    // Busca se ha regras e cria
                    if (VerificarRegrasAutorizacaoAvaria(avaria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao, TipoServicoMultisoftware, unitOfWork))
                    {
                        atualizaAvaria = true;
                        avaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgAprovacao;
                    }
                }
                else if (avaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.SemRegraLote)
                {
                    // Caso não tenha nenhum resposnavel, atualiza situacao
                    if (Servicos.Embarcador.Avarias.ResponsavelAvaria.CriaResponsavelSolicitacao(avaria, unitOfWork))
                        avaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgLote;
                    else
                        avaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.SemRegraLote;

                    atualizaAvaria = true;
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, avaria, null, "Reconsultou as regras de aprovação da avaria.", unitOfWork);

                // Retorno de informacoes
                var retorno = new
                {
                    Situacao = avaria.SituacaoFluxo
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

        public async Task<IActionResult> GerarLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Busca as avarias
                int codigo = Request.GetIntParam("Codigo");

                // Agrupa avarias por transportador
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork).BuscarPorCodigo(codigo);

                if (avaria == null)
                    throw new ServicoException("Não foi possível encontrar o registro da avaria.");

                GerarRegistroLote(avaria, unitOfWork);

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
        }

        public async Task<IActionResult> ConfirmarEtapaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (avaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a avaria.");
                if (avaria.Lote == null)
                    return new JsonpResult(false, true, "Solicitação de avaria sem Lote gerado.");

                if (avaria.Lote.MotivoAvaria.Responsavel != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.Transportador)
                    return new JsonpResult(false, true, "Esse lote não pode ter integrações.");

                GerarRegistrosIntegracao(avaria.Lote, unitOfWork);

                // Seta todas avarias como finalizadas
                FinalizarEtapaLote(avaria, unitOfWork);

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarProdutosAvariados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

                List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> produtosAvariados = repProdutosAvariados.BuscarProdutosAvariadosDeLote(codigo);

                dynamic retorno = new
                {
                    ProdutosAvariados = (from obj in produtosAvariados
                                         select new
                                         {
                                             obj.Codigo,
                                             obj.SolicitacaoAvaria.NumeroAvaria,
                                             NumeroLote = obj.SolicitacaoAvaria.Lote.Numero,
                                             ProdutoEmbarcador = obj.ProdutoEmbarcador.Descricao,
                                             obj.NotaFiscal,
                                             obj.UnidadesAvariadas,
                                             CodigoProdutoEmbarcador = obj.ProdutoEmbarcador.Codigo
                                         }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os produtos avariados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaProdutoDestinacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoLote = Request.GetIntParam("CodigoLote");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Produto", "Produto", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 20, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarProdutoDestinacao);
                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino> loteAvariasDestino = repLoteAvariaDestino.Consultar(codigoLote, parametrosConsulta);
                grid.setarQuantidadeTotal(repLoteAvariaDestino.ContarConsulta(codigoLote));

                var lista = (from p in loteAvariasDestino
                             select new
                             {
                                 p.Codigo,
                                 Produto = p.ProdutoEmbarcador.Descricao,
                                 p.Quantidade,
                                 Destino = p.Destino.ObterDescricao()
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

        public async Task<IActionResult> AdicionarProdutoDestinacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = new Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino();

                PreencherLoteAvariaDestino(loteAvariaDestino, unitOfWork);

                repLoteAvariaDestino.Inserir(loteAvariaDestino);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarProdutoDestinacao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = repLoteAvariaDestino.BuscarPorCodigo(codigo, true);

                if (loteAvariaDestino == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherLoteAvariaDestino(loteAvariaDestino, unitOfWork);

                repLoteAvariaDestino.Atualizar(loteAvariaDestino);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutoDestinacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = repLoteAvariaDestino.BuscarPorCodigo(codigo, false);

                if (loteAvariaDestino == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynLoteAvariaDestino = new
                {
                    loteAvariaDestino.Codigo,
                    loteAvariaDestino.Quantidade,
                    loteAvariaDestino.Destino,
                    Valor = loteAvariaDestino.Valor.ToString("n2"),
                    DataVencimento = loteAvariaDestino.DataVencimento?.ToDateString(),
                    Produto = new { loteAvariaDestino.ProdutoEmbarcador.Codigo, loteAvariaDestino.ProdutoEmbarcador.Descricao },
                    Motorista = new { Codigo = loteAvariaDestino.Motorista?.Codigo ?? 0, Descricao = loteAvariaDestino.Motorista?.Descricao ?? string.Empty },
                    Cliente = new { Codigo = loteAvariaDestino.Cliente?.Codigo ?? 0, Descricao = loteAvariaDestino.Cliente?.Nome ?? string.Empty },
                    Carga = new { Codigo = loteAvariaDestino.Carga?.Codigo ?? 0, Descricao = loteAvariaDestino.Carga?.Descricao ?? string.Empty },
                    TipoMovimento = new { Codigo = loteAvariaDestino.TipoMovimento?.Codigo ?? 0, Descricao = loteAvariaDestino.TipoMovimento?.Descricao ?? string.Empty },
                    loteAvariaDestino.NumeroFatura
                };

                return new JsonpResult(dynLoteAvariaDestino);
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

        public async Task<IActionResult> ExcluirProdutoDestinacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);

                Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino = repLoteAvariaDestino.BuscarPorCodigo(codigo, true);

                if (loteAvariaDestino == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repLoteAvariaDestino.Deletar(loteAvariaDestino);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarDestinoAvaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.SolicitacaoAvaria repAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repAvaria.BuscarPorCodigo(codigo);

                if (avaria.Lote == null)
                    return new JsonpResult(false, true, "Lote não encontrado.");

                if (avaria.Lote.Situacao != SituacaoLote.Finalizada)
                    return new JsonpResult(false, true, "Já foi finalizado o destino da avaria.");

                if (avaria.SituacaoFluxo != SituacaoFluxoAvaria.Destinacao)
                    return new JsonpResult(false, true, "Situação da avaria não permite essa ação.");

                unitOfWork.Start();

                EfetivarDestinoProdutoAvaria(avaria.Lote.Codigo, unitOfWork);

                avaria.Lote.Situacao = SituacaoLote.FinalizadaComDestino;
                repLote.Atualizar(avaria.Lote, Auditado);

                avaria.SituacaoFluxo = SituacaoFluxoAvaria.Finalizado;
                repAvaria.Atualizar(avaria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o destino da avaria.");
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
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            var repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);
            var repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
            var repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);


            // Converte valores
            var codigoCarga = 0;
            int.TryParse(Request.Params("Carga"), out codigoCarga);
            var carga = repCarga.BuscarPorCodigo(codigoCarga);

            int codigoMotivoAvaria = 0;
            int.TryParse(Request.Params("MotivoAvaria"), out codigoMotivoAvaria);

            var motivo = repMotivoAvaria.BuscarPorCodigo(codigoMotivoAvaria);

            var justificativa = Request.Params("Justificativa");
            if (string.IsNullOrWhiteSpace(justificativa))
                justificativa = string.Empty;

            var motorista = Request.Params("Motorista");
            if (string.IsNullOrWhiteSpace(motorista))
                motorista = string.Empty;

            var RGmotorista = Request.Params("RGMotorista");
            if (string.IsNullOrWhiteSpace(RGmotorista))
                RGmotorista = string.Empty;

            var cpfMotorista = Request.Params("CPFMotorista");
            if (string.IsNullOrWhiteSpace(cpfMotorista))
                cpfMotorista = string.Empty;

            var CentroResultado = new Dominio.Entidades.Embarcador.Financeiro.CentroResultado();
            int centroResultadoValido;
            if (int.TryParse(Request.Params("CentroResultado"), out centroResultadoValido))
                CentroResultado = repCentroResultado.BuscarPorCodigo(centroResultadoValido);


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

            if (solicitacaoAvaria.Motorista != solicitacaoAvaria.MotoristaOriginal)
                solicitacaoAvaria.MotoristaModificado = true;

            if (solicitacaoAvaria.RGMotorista != solicitacaoAvaria.RGMotoristaOriginal)
                solicitacaoAvaria.RGMotoristaModificado = true;

            solicitacaoAvaria.DataSolicitacao = DateTime.Now;
            solicitacaoAvaria.Solicitante = this.Usuario;
            solicitacaoAvaria.NumeroAvaria = repSolicitacaoAvaria.BuscarProximoCodigo();
            solicitacaoAvaria.CentroResultado = CentroResultado;
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
                msgErro = "Carga é obrigatória.";
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
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            // Dados do filtro
            int numeroAvaria = 0;
            int.TryParse(Request.Params("NumeroAvaria"), out numeroAvaria);
            int transportadora = 0;
            int.TryParse(Request.Params("Transportadora"), out transportadora);
            int motivoAvaria = 0;
            int.TryParse(Request.Params("MotivoAvaria"), out motivoAvaria);
            int numeroNota = 0;
            //int.TryParse(Request.Params("NumeroNota"), out numeroNota);

            DateTime dataInicio;
            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
            DateTime dataFim;
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

            string carga = Request.Params("Carga");

            Dominio.Entidades.Embarcador.Cargas.Carga cargaBusca = repCarga.BuscarPorCodigo(Int32.Parse(carga));

            if (cargaBusca != null)
            {
                carga = cargaBusca.CodigoCargaEmbarcador;
            }

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
                            Carga = obj.Carga.CodigoCargaEmbarcador,
                            DataAvaria = obj.DataAvaria.ToString("dd/MM/yyyy"),
                            MotivoAvaria = obj.MotivoAvaria.Descricao,
                            Solicitante = obj.Solicitante.Nome,
                            DescricaoSituacao = obj.DescricaoSituacaoFluxo,
                            CentroResultado = obj.CentroResultado?.Descricao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaProdutos(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            // Dados do filtro
            int codigo = Request.GetIntParam("Codigo");

            string descricao = Request.Params("DescricaoProduto");

            // Consulta produtos da solicitacao
            List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> listaGrid = repProdutosAvariados.BuscarPorSolicitacao(codigo, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProdutosAvariados.ContarBuscaPorSolicitacao(codigo, descricao);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj?.ProdutoNotaFiscal.Produto.Descricao,
                            Quantidade = obj?.CaixasAvariadas ?? 0,
                            Unidades = obj?.UnidadesAvariadas ?? 0,
                            Valor = obj?.ValorAvaria.ToString("F") ?? "0",
                            NFe = obj?.NotaFiscal ?? "",
                            LocalArmazenamento = obj?.LocalArmazenamento?.Descricao ?? string.Empty,
                            ProdutoEmbarcador = obj?.ProdutoEmbarcador?.Descricao ?? string.Empty,
                            GeraEstoque = (obj?.GeraEstoque ?? false),
                            CodigoNota = obj?.ProdutoNotaFiscal?.XMLNotaFiscal?.Codigo ?? 0
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaNotasProdutos(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
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

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj?.ProdutoNotaFiscal?.XMLNotaFiscal?.Codigo,
                            Descricao = obj?.NotaFiscal ?? string.Empty,
                            NFe = obj?.NotaFiscal ?? string.Empty,
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Carga") propOrdenar = "Carga.CodigoCargaEmbarcador";
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
            grid.AdicionarCabecalho("Número", "NumeroAvaria", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data", "DataAvaria", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Motivo", "MotivoAvaria", 15, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Solicitante", false);
            else
                grid.AdicionarCabecalho("Solicitante", "Solicitante", 15, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private Models.Grid.Grid GridProdutos(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            Models.Grid.EditableCell campoInt = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt, 9);
            Models.Grid.EditableCell campoDecimal = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 25);
            Models.Grid.EditableCell campoBool = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aBool);

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 5, Models.Grid.Align.right, true, false, false, false, true, campoInt);
            grid.AdicionarCabecalho("Unidades", "Unidades", 7, Models.Grid.Align.right, true, false, false, false, true, campoInt);
            grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true, false, false, false, true, campoDecimal);
            grid.AdicionarCabecalho("NF-e", "NFe", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Local Armazenamento", "LocalArmazenamento", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Produto Embarcador", "ProdutoEmbarcador", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Gera Estoque", "GeraEstoque", 7, Models.Grid.Align.center, true, false, false, false, true, campoBool);
            grid.AdicionarCabecalho("CodigoNota", false);

            return grid;
        }

        private void VerificarRegrasAprovacao(ref Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada = Servicos.Embarcador.Avarias.AutorizacaoSolicitacaoAvaria.VerificarRegrasAutorizacaoAvaria(solicitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao, unitOfWork);

            if (listaFiltrada.Any())
            {
                Servicos.Embarcador.Avarias.AutorizacaoSolicitacaoAvaria.CriarRegrasAutorizacao(listaFiltrada, solicitacao, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
            }
            else
            {
                solicitacao.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.SemRegraAprovacao;
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
                solicitacaoAvaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Dados ||
                solicitacaoAvaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.SemRegraAprovacao ||
                solicitacaoAvaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Cancelado
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
                Situacao = solicitacaoAvaria.DescricaoSituacaoFluxo,
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

            if (solicitacaoAvaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.AgLote && (repResponsavelAvaria.BuscaPorUsuarioAvaria(solicitacaoAvaria.Codigo, this.Usuario.Codigo) != null))
                return true;
            if (solicitacaoAvaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.SemRegraLote)
                return true;

            return false;
        }

        private void GerarRegistroLote(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria, Repositorio.UnitOfWork unitOfWork)
        {
            // Inicia transacao
            unitOfWork.Start();

            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork);
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            // Busca informacoes
            Dominio.Entidades.Embarcador.Avarias.Lote lote = new Dominio.Entidades.Embarcador.Avarias.Lote();

            // Dados Criacao 
            lote.Numero = repLote.BuscarProximoNumero();
            lote.DataGeracao = DateTime.Now;
            lote.Transportador = avaria.Transportador;
            lote.MotivoAvaria = avaria.MotivoAvaria;
            lote.Criador = this.Usuario;
            lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCriacao;
            lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote;

            // Insere lote no banco
            repLote.Inserir(lote, Auditado);

            avaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.LoteGerado;
            avaria.Lote = lote;
            repSolicitacaoAvaria.Atualizar(avaria);

            // Inclui controle de tempo por etapa
            Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote();
            tempoEtapa.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote;
            tempoEtapa.Entrada = DateTime.Now;
            tempoEtapa.Saida = null;
            tempoEtapa.Lote = lote;
            repTempoEtapaLote.Inserir(tempoEtapa);

            unitOfWork.CommitChanges();
        }

        private void GerarRegistrosIntegracao(Dominio.Entidades.Embarcador.Avarias.Lote lote, Repositorio.UnitOfWork unitOfWork)
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

        private void FinalizarEtapaLote(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            if (avaria.SituacaoFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Destinacao)
                return;

            avaria.SituacaoFluxo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoAvaria.Destinacao;
            repSolicitacaoAvaria.Atualizar(avaria, Auditado);

            if (avaria.TituloBaixaAgrupadoDocumento != null)
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = avaria.TituloBaixaAgrupadoDocumento;
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento;

                if (tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe)
                    documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                else
                    documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloBaixaAgrupadoDocumento.TituloDocumento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);

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

        private void PreencherLoteAvariaDestino(Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            int codigoLote = Request.GetIntParam("CodigoLote");
            int codigoProduto = Request.GetIntParam("Produto");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoCarga = Request.GetIntParam("Carga");
            int codigoTipoMovimento = Request.GetIntParam("TipoMovimento");
            double codigoCliente = Request.GetDoubleParam("Cliente");
            int numeroFatura = Request.GetIntParam("NumeroFatura");

            if (loteAvariaDestino.Codigo == 0)
            {
                if (!repProdutosAvariados.ProdutoAvariadoExisteNoLote(codigoLote, codigoProduto))
                    throw new ControllerException("Produto que está tentando adicionar não existe no lote");

                loteAvariaDestino.Lote = repLote.BuscarPorCodigo(codigoLote);
            }

            int quantidadeLote = repProdutosAvariados.QuantidadeProdutoAvariadoNoLote(codigoLote, codigoProduto);
            int quantidadeJaInformada = repLoteAvariaDestino.QuantidadeProdutoInformado(codigoLote, codigoProduto, loteAvariaDestino.Codigo);

            if (Request.GetIntParam("Quantidade") + quantidadeJaInformada > quantidadeLote)
                throw new ControllerException("Quantidade informada no item excede o limite do lote. Limite disponível para informar: " + (quantidadeLote - quantidadeJaInformada));

            loteAvariaDestino.Quantidade = Request.GetIntParam("Quantidade");
            loteAvariaDestino.Destino = Request.GetEnumParam<DestinoProdutoAvaria>("Destino");
            loteAvariaDestino.Valor = Request.GetDecimalParam("Valor");
            loteAvariaDestino.DataVencimento = Request.GetNullableDateTimeParam("DataVencimento");

            loteAvariaDestino.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);
            loteAvariaDestino.Motorista = codigoMotorista > 0 ? repMotorista.BuscarPorCodigo(codigoMotorista) : null;
            loteAvariaDestino.Carga = codigoCarga > 0 ? repCarga.BuscarPorCodigo(codigoCarga) : null;
            loteAvariaDestino.TipoMovimento = codigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;
            loteAvariaDestino.Cliente = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;
            loteAvariaDestino.NumeroFatura = numeroFatura;
        }

        private string ObterPropriedadeOrdenarProdutoDestinacao(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Produto")
                return "ProdutoEmbarcador.Descricao";

            return propriedadeOrdenar;
        }

        private void EfetivarDestinoProdutoAvaria(int codigoLote, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
            Repositorio.Embarcador.Avarias.ProdutosAvariados repProdutosAvariados = new Repositorio.Embarcador.Avarias.ProdutosAvariados(unitOfWork);

            List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino> loteAvariasDestino = repLoteAvariaDestino.BuscarPorLote(codigoLote);
            if (loteAvariasDestino.Count == 0)
                throw new ControllerException("Nenhum produto avariado foi lançado para destino");

            if (repLoteAvariaDestino.ExisteProdutoNaoInformado(codigoLote))
                throw new ControllerException("Há produtos avariados que não foram informado o destino");

            foreach (Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino in loteAvariasDestino)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = loteAvariaDestino.ProdutoEmbarcador;

                int quantidadeDasAvarias = repProdutosAvariados.QuantidadeProdutoAvariadoNoLote(codigoLote, produtoEmbarcador.Codigo);
                int quantidadeDosLotes = loteAvariasDestino.Where(o => o.ProdutoEmbarcador.Codigo == produtoEmbarcador.Codigo).Sum(o => o.Quantidade);

                if (quantidadeDasAvarias != loteAvariaDestino.Quantidade && quantidadeDosLotes != quantidadeDasAvarias)
                    throw new ControllerException($"Quantidade informada no produto {produtoEmbarcador.Descricao} é diferente da quantidade das avarias");

                if (loteAvariaDestino.Destino != DestinoProdutoAvaria.Descartada && loteAvariaDestino.Destino != DestinoProdutoAvaria.DevolvidaCliente && loteAvariaDestino.Destino != DestinoProdutoAvaria.DescontoFatura)
                    GerarTituloFinanceiro(loteAvariaDestino, unitOfWork);

                MovimentarEstoque(produtoEmbarcador, loteAvariaDestino.Quantidade, unitOfWork);
            }
        }

        private void GerarTituloFinanceiro(Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino loteAvariaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.LoteAvariaDestino repLoteAvariaDestino = new Repositorio.Embarcador.Avarias.LoteAvariaDestino(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            Dominio.Entidades.Cliente pessoa = loteAvariaDestino.Cliente;
            if (loteAvariaDestino.Destino == DestinoProdutoAvaria.DescontadaMotorista)
            {
                pessoa = repCliente.BuscarPorCPFCNPJ(loteAvariaDestino.Motorista.CPF.ToDouble());
                if (pessoa == null)
                {
                    if (loteAvariaDestino.Motorista.Localidade == null)
                        throw new ControllerException("Motorista está com o endereço incompleto no cadastro! Favor preencher antes de prosseguir.");

                    pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(loteAvariaDestino.Motorista, unitOfWork);
                    repCliente.Inserir(pessoa, Auditado);
                }
            }

            if (pessoa == null)
                throw new ControllerException("Cliente não está cadastrado");

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = Usuario;
            titulo.TipoTitulo = TipoTitulo.Receber;
            titulo.DataEmissao = DateTime.Now.Date;
            titulo.DataVencimento = loteAvariaDestino.DataVencimento;
            titulo.DataProgramacaoPagamento = titulo.DataVencimento;
            titulo.GrupoPessoas = pessoa.GrupoPessoas;
            titulo.Pessoa = pessoa;
            titulo.Sequencia = 1;
            titulo.ValorOriginal = loteAvariaDestino.Valor;
            titulo.ValorPendente = loteAvariaDestino.Valor;
            titulo.StatusTitulo = StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.Observacao = $"Referente ao destino da avaria do produto {loteAvariaDestino.ProdutoEmbarcador.Descricao} do lote nº {loteAvariaDestino.Lote.Numero}";
            titulo.Empresa = null;
            titulo.ValorTituloOriginal = titulo.ValorOriginal;
            titulo.TipoDocumentoTituloOriginal = "Lote Avaria";
            titulo.NumeroDocumentoTituloOriginal = loteAvariaDestino.Lote.Numero.ToString();
            titulo.FormaTitulo = FormaTitulo.Outros;
            titulo.TipoMovimento = loteAvariaDestino.TipoMovimento;

            if (titulo.DataVencimento.Value.Date < titulo.DataEmissao)
                throw new ControllerException("A data de vencimento não podem ser menor que a data de emissão.");

            repTitulo.Inserir(titulo);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Adicionado pelo Lote de Avaria.", unitOfWork);

            if (!svcProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Observacao, unitOfWork, TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, loteAvariaDestino.Motorista?.Codigo ?? 0, null, null, titulo.Codigo, TipoMovimentoEntidade.Saida, titulo.Pessoa, null, titulo.DataEmissao))
                throw new ControllerException(erro);

            loteAvariaDestino.Titulo = titulo;
            repLoteAvariaDestino.Atualizar(loteAvariaDestino);
        }

        private void MovimentarEstoque(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, int quantidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote produtoEmbarcadorLote = repProdutoEmbarcadorLote.BuscarPorProduto(produtoEmbarcador.Codigo);

            if (produtoEmbarcadorLote == null)
                throw new ControllerException($"O produto {produtoEmbarcador.Descricao} não possui lote no cadastro");

            produtoEmbarcadorLote.QuantidadeAtual -= quantidade;

            repProdutoEmbarcadorLote.Atualizar(produtoEmbarcadorLote);
        }
        #endregion
    }
}
