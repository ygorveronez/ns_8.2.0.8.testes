using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ConfiguracaoContabil
{
    [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoNaturezaOperacao")]
    public class ConfiguracaoNaturezaOperacaoController : BaseController
    {
		#region Construtores

		public ConfiguracaoNaturezaOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                double remetente, destinatario, tomador;
                int grupoRemetente, grupoDestinatario, grupoTomador, tipoOperacao, empresa;
                double.TryParse(Request.Params("Remetente"), out remetente);
                double.TryParse(Request.Params("Destinatario"), out destinatario);
                double.TryParse(Request.Params("Tomador"), out tomador);
                int.TryParse(Request.Params("GrupoRemetente"), out grupoRemetente);
                int.TryParse(Request.Params("GrupoDestinatario"), out grupoDestinatario);
                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("GrupoTomador"), out grupoTomador);
                int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);
                int categoriaDestinatario = Request.GetIntParam("CategoriaDestinatario");
                int categoriaRemetente = Request.GetIntParam("CategoriaRemetente");
                int categoriaTomador = Request.GetIntParam("CategoriaTomador");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Natureza", "Natureza", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("UF Origem", "UFOrigem", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("UF Destino", "UFDestino", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Doc", "ModeloDocumentoFiscal", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emitente Fora Estado", "EstadoEmissorDiferenteUFOrigem", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Fora Estado", "EstadoOrigemDiferenteUFDestino", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Dentro Estado", "EstadoOrigemIgualUFDestino", 8, Models.Grid.Align.center, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao> listaConfiguracaoNaturezaOperacao = repConfiguracaoNaturezaOperacao.Consultar(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, tipoOperacao, ativo, categoriaDestinatario, categoriaRemetente, categoriaTomador, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConfiguracaoNaturezaOperacao.ContarConsulta(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, tipoOperacao, ativo, categoriaDestinatario, categoriaRemetente, categoriaTomador));

                var retorno = (from obj in listaConfiguracaoNaturezaOperacao
                               select new
                               {
                                   obj.Codigo,
                                   TipoOperacao = obj.TipoOperacao?.Descricao ?? "",
                                   Empresa = obj.Empresa?.Descricao ?? "",
                                   UFOrigem = (obj.UFOrigem?.Sigla ?? "") + (obj.EstadoOrigemDiferente ? " (Diferente) " : ""),
                                   UFDestino = (obj.UFDestino?.Sigla ?? "") + (obj.EstadoDestinoDiferente ? " (Diferente) " : ""),
                                   EstadoEmissorDiferenteUFOrigem = obj.EstadoEmissorDiferenteUFOrigem ? "Sim" : "Não",
                                   EstadoOrigemDiferenteUFDestino = obj.EstadoOrigemDiferenteUFDestino ? "Sim" : "Não",
                                   EstadoOrigemIgualUFDestino = obj.EstadoOrigemIgualUFDestino ? "Sim" : "Não",
                                   ModeloDocumentoFiscal = obj.ModeloDocumentoFiscal?.Abreviacao ?? "",
                                   Natureza = obj.NaturezaDaOperacaoCompra?.BuscarDescricao ?? "",
                                   Tomador = obj.Tomador != null ? obj.Tomador.Descricao : (obj.GrupoTomador != null ? obj.GrupoTomador.Descricao : ""),
                                   obj.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao();
                string erro = preecherConfiguracaoNaturezaOperacao(ref configuracaoNaturezaOperacao, unitOfWork);

                if (!string.IsNullOrWhiteSpace(erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                repConfiguracaoNaturezaOperacao.Inserir(configuracaoNaturezaOperacao, Auditado);
                SalvarConfiguracoesDeContabilizacao(configuracaoNaturezaOperacao, unitOfWork);
                SalvarConfiguracoesDeEscrituracao(configuracaoNaturezaOperacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = repConfiguracaoNaturezaOperacao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                string erro = preecherConfiguracaoNaturezaOperacao(ref configuracaoNaturezaOperacao, unitOfWork);

                if (!string.IsNullOrWhiteSpace(erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repConfiguracaoNaturezaOperacao.Atualizar(configuracaoNaturezaOperacao, Auditado);
                SalvarConfiguracoesDeContabilizacao(configuracaoNaturezaOperacao, unitOfWork, historico);
                SalvarConfiguracoesDeEscrituracao(configuracaoNaturezaOperacao, unitOfWork, historico);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = repConfiguracaoNaturezaOperacao.BuscarPorCodigo(codigo);

                var entidade = new
                {
                    configuracaoNaturezaOperacao.Ativo,
                    configuracaoNaturezaOperacao.Codigo,
                    Destinatario = new { Codigo = configuracaoNaturezaOperacao.Destinatario != null ? configuracaoNaturezaOperacao.Destinatario.CPF_CNPJ : 0, Descricao = configuracaoNaturezaOperacao.Destinatario != null ? configuracaoNaturezaOperacao.Destinatario.Descricao : "" },
                    Empresa = new { Codigo = configuracaoNaturezaOperacao.Empresa != null ? configuracaoNaturezaOperacao.Empresa.Codigo : 0, Descricao = configuracaoNaturezaOperacao.Empresa != null ? configuracaoNaturezaOperacao.Empresa.Descricao : "" },
                    Remetente = new { Codigo = configuracaoNaturezaOperacao.Remetente != null ? configuracaoNaturezaOperacao.Remetente.CPF_CNPJ : 0, Descricao = configuracaoNaturezaOperacao.Remetente != null ? configuracaoNaturezaOperacao.Remetente.Descricao : "" },
                    Tomador = new { Codigo = configuracaoNaturezaOperacao.Tomador != null ? configuracaoNaturezaOperacao.Tomador.CPF_CNPJ : 0, Descricao = configuracaoNaturezaOperacao.Tomador != null ? configuracaoNaturezaOperacao.Tomador.Descricao : "" },
                    CategoriaDestinatario = new { Codigo = configuracaoNaturezaOperacao.CategoriaDestinatario?.Codigo ?? 0, Descricao = configuracaoNaturezaOperacao.CategoriaDestinatario?.Descricao ?? "" },
                    CategoriaRemetente = new { Codigo = configuracaoNaturezaOperacao.CategoriaRemetente?.Codigo ?? 0, Descricao = configuracaoNaturezaOperacao.CategoriaRemetente?.Descricao ?? "" },
                    CategoriaTomador = new { Codigo = configuracaoNaturezaOperacao.CategoriaTomador?.Codigo ?? 0, Descricao = configuracaoNaturezaOperacao.CategoriaTomador?.Descricao ?? "" },
                    GrupoDestinatario = new { Codigo = configuracaoNaturezaOperacao.GrupoDestinatario != null ? configuracaoNaturezaOperacao.GrupoDestinatario.Codigo : 0, Descricao = configuracaoNaturezaOperacao.GrupoDestinatario != null ? configuracaoNaturezaOperacao.GrupoDestinatario.Descricao : "" },
                    GrupoRemetente = new { Codigo = configuracaoNaturezaOperacao.GrupoRemetente != null ? configuracaoNaturezaOperacao.GrupoRemetente.Codigo : 0, Descricao = configuracaoNaturezaOperacao.GrupoRemetente != null ? configuracaoNaturezaOperacao.GrupoRemetente.Descricao : "" },
                    GrupoTomador = new { Codigo = configuracaoNaturezaOperacao.GrupoTomador != null ? configuracaoNaturezaOperacao.GrupoTomador.Codigo : 0, Descricao = configuracaoNaturezaOperacao.GrupoTomador != null ? configuracaoNaturezaOperacao.GrupoTomador.Descricao : "" },
                    TipoOperacao = new { Codigo = configuracaoNaturezaOperacao.TipoOperacao != null ? configuracaoNaturezaOperacao.TipoOperacao.Codigo : 0, Descricao = configuracaoNaturezaOperacao.TipoOperacao != null ? configuracaoNaturezaOperacao.TipoOperacao.Descricao : "" },
                    AtividadeDestinatario = new { Codigo = configuracaoNaturezaOperacao.AtividadeDestinatario != null ? configuracaoNaturezaOperacao.AtividadeDestinatario.Codigo : 0, Descricao = configuracaoNaturezaOperacao.AtividadeDestinatario != null ? configuracaoNaturezaOperacao.AtividadeDestinatario.Descricao : "" },
                    UFDestino = configuracaoNaturezaOperacao.UFDestino != null ? configuracaoNaturezaOperacao.UFDestino.Sigla : "",
                    UFOrigem = configuracaoNaturezaOperacao.UFOrigem != null ? configuracaoNaturezaOperacao.UFOrigem.Sigla : "",
                    ModeloDocumentoFiscal = configuracaoNaturezaOperacao.ModeloDocumentoFiscal != null ? new { configuracaoNaturezaOperacao.ModeloDocumentoFiscal.Codigo, configuracaoNaturezaOperacao.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                    configuracaoNaturezaOperacao.EstadoDestinoDiferente,
                    configuracaoNaturezaOperacao.EstadoOrigemDiferente,
                    configuracaoNaturezaOperacao.EstadoEmissorDiferenteUFOrigem,
                    configuracaoNaturezaOperacao.EstadoOrigemIgualUFDestino,
                    configuracaoNaturezaOperacao.EstadoOrigemDiferenteUFDestino,
                    AtividadeRemetente = new { Codigo = configuracaoNaturezaOperacao.AtividadeRemetente != null ? configuracaoNaturezaOperacao.AtividadeRemetente.Codigo : 0, Descricao = configuracaoNaturezaOperacao.AtividadeRemetente != null ? configuracaoNaturezaOperacao.AtividadeRemetente.Descricao : "" },
                    AtividadeTomador = new { Codigo = configuracaoNaturezaOperacao.AtividadeTomador != null ? configuracaoNaturezaOperacao.AtividadeTomador.Codigo : 0, Descricao = configuracaoNaturezaOperacao.AtividadeTomador != null ? configuracaoNaturezaOperacao.AtividadeTomador.Descricao : "" },
                    GrupoProduto = new { Codigo = configuracaoNaturezaOperacao.GrupoProduto != null ? configuracaoNaturezaOperacao.GrupoProduto.Codigo : 0, Descricao = configuracaoNaturezaOperacao.GrupoProduto != null ? configuracaoNaturezaOperacao.GrupoProduto.Descricao : "" },
                    RotaFrete = new { Codigo = configuracaoNaturezaOperacao.RotaFrete != null ? configuracaoNaturezaOperacao.RotaFrete.Codigo : 0, Descricao = configuracaoNaturezaOperacao.RotaFrete != null ? configuracaoNaturezaOperacao.RotaFrete.Descricao : "" },
                    ConfiguracaoNaturezaOperacaoContabilizacaos = (from obj in configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoContabilizacoes
                                                                   select new
                                                                   {
                                                                       Codigo = obj.Codigo,
                                                                       NaturezaOperacao = new { obj.NaturezaDaOperacao.Codigo, Descricao = obj.NaturezaDaOperacao.BuscarDescricao },
                                                                       obj.TipoContaContabil
                                                                   }).ToList(),
                    ConfiguracaoNaturezaOperacaoEscrituracaos = (from obj in configuracaoNaturezaOperacao.ConfiguracaoNaturezaOperacaoEscrituracoes
                                                                 select new
                                                                 {
                                                                     Codigo = obj.Codigo,
                                                                     NaturezaOperacao = new { obj.NaturezaDaOperacao.Codigo, Descricao = obj.NaturezaDaOperacao.BuscarDescricao },
                                                                     obj.TipoContaContabil
                                                                 }).ToList(),
                    NaturezaDaOperacaoVenda = new { configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.Codigo, Descricao = configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda.BuscarDescricao },
                    NaturezaDaOperacaoCompra = new { configuracaoNaturezaOperacao.NaturezaDaOperacaoCompra.Codigo, Descricao = configuracaoNaturezaOperacao.NaturezaDaOperacaoCompra.BuscarDescricao }
                };

                return new JsonpResult(entidade);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao = repConfiguracaoNaturezaOperacao.BuscarPorCodigo(codigo);
                repConfiguracaoNaturezaOperacao.Deletar(configuracaoNaturezaOperacao, Auditado);
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
        
        #endregion

        #region MétodosPrivados

        private string preecherConfiguracaoNaturezaOperacao(ref Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao repConfiguracaoNaturezaOperacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao(unitOfWork);

            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorioCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            string retorno = "";
            double remetente, destinatario, tomador;
            double.TryParse(Request.Params("Remetente"), out remetente);
            double.TryParse(Request.Params("Destinatario"), out destinatario);
            double.TryParse(Request.Params("Tomador"), out tomador);

            int grupoRemetente, grupoDestinatario, grupoTomador, rotaFrete, empresa, tipoOperacao, grupoProduto, NaturezaDaOperacaoVenda, NaturezaDaOperacaoCompra, modeloDocumentoFiscal, atividadeRemente, atividadeDestinatario, atividadeTomador;
            int.TryParse(Request.Params("GrupoRemetente"), out grupoRemetente);
            int.TryParse(Request.Params("GrupoDestinatario"), out grupoDestinatario);
            int.TryParse(Request.Params("GrupoTomador"), out grupoTomador);
            int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);
            int.TryParse(Request.Params("GrupoProduto"), out grupoProduto);
            int.TryParse(Request.Params("Empresa"), out empresa);
            int.TryParse(Request.Params("RotaFrete"), out rotaFrete);
            int.TryParse(Request.Params("ModeloDocumentoFiscal"), out modeloDocumentoFiscal);
            int.TryParse(Request.Params("NaturezaDaOperacaoVenda"), out NaturezaDaOperacaoVenda);
            int.TryParse(Request.Params("NaturezaDaOperacaoCompra"), out NaturezaDaOperacaoCompra);

            int.TryParse(Request.Params("AtividadeRemetente"), out atividadeRemente);
            int.TryParse(Request.Params("AtividadeDestinatario"), out atividadeDestinatario);
            int.TryParse(Request.Params("AtividadeTomador"), out atividadeTomador);
            int categoriaDestinatario = Request.GetIntParam("CategoriaDestinatario");
            int categoriaRemetente = Request.GetIntParam("CategoriaRemetente");
            int categoriaTomador = Request.GetIntParam("CategoriaTomador");

            string ufOrigem = Request.Params("UFOrigem");
            string ufDestino = Request.Params("UFDestino");

            bool ativo, estadoOrigemDiferente, estadoDestinoDiferente, ufOrigemDiferenteDeUFDestino, ufOrigemIgualUFDestino, estadoEmissorDiferenteUFOrigem;
            bool.TryParse(Request.Params("Ativo"), out ativo);
            bool.TryParse(Request.Params("EstadoOrigemDiferente"), out estadoOrigemDiferente);
            bool.TryParse(Request.Params("EstadoDestinoDiferente"), out estadoDestinoDiferente);
            bool.TryParse(Request.Params("EstadoOrigemDiferenteUFDestino"), out ufOrigemDiferenteDeUFDestino);
            bool.TryParse(Request.Params("EstadoEmissorDiferenteUFOrigem"), out estadoEmissorDiferenteUFOrigem);
            bool.TryParse(Request.Params("EstadoOrigemIgualUFDestino"), out ufOrigemIgualUFDestino);
            configuracaoNaturezaOperacao.Ativo = ativo;

            configuracaoNaturezaOperacao.NaturezaDaOperacaoCompra = repNaturezaDaOperacao.BuscarPorId(NaturezaDaOperacaoCompra);
            configuracaoNaturezaOperacao.NaturezaDaOperacaoVenda = repNaturezaDaOperacao.BuscarPorId(NaturezaDaOperacaoVenda);
            configuracaoNaturezaOperacao.Remetente = remetente > 0 ? repCliente.BuscarPorCPFCNPJ(remetente) : null;
            configuracaoNaturezaOperacao.Tomador = tomador > 0 ? repCliente.BuscarPorCPFCNPJ(tomador) : null;
            configuracaoNaturezaOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
            configuracaoNaturezaOperacao.Destinatario = destinatario > 0 ? repCliente.BuscarPorCPFCNPJ(destinatario) : null;

            configuracaoNaturezaOperacao.AtividadeTomador = atividadeTomador > 0 ? repAtividade.BuscarPorCodigo(atividadeTomador) : null;
            configuracaoNaturezaOperacao.AtividadeRemetente = atividadeRemente > 0 ? repAtividade.BuscarPorCodigo(atividadeRemente) : null;
            configuracaoNaturezaOperacao.AtividadeDestinatario = atividadeDestinatario > 0 ? repAtividade.BuscarPorCodigo(atividadeDestinatario) : null;
            configuracaoNaturezaOperacao.ModeloDocumentoFiscal = modeloDocumentoFiscal > 0 ? repModeloDocumentoFiscal.BuscarPorId(modeloDocumentoFiscal) : null;

            configuracaoNaturezaOperacao.CategoriaDestinatario = categoriaDestinatario > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaDestinatario) : null;
            configuracaoNaturezaOperacao.CategoriaRemetente = categoriaRemetente > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaRemetente) : null;
            configuracaoNaturezaOperacao.CategoriaTomador = categoriaTomador > 0 ? repositorioCategoriaPessoa.BuscarPorCodigo(categoriaTomador) : null;
            configuracaoNaturezaOperacao.GrupoDestinatario = repGrupoPessoas.BuscarPorCodigo(grupoDestinatario);
            configuracaoNaturezaOperacao.GrupoRemetente = repGrupoPessoas.BuscarPorCodigo(grupoRemetente);
            configuracaoNaturezaOperacao.GrupoTomador = repGrupoPessoas.BuscarPorCodigo(grupoTomador);
            configuracaoNaturezaOperacao.GrupoProduto = repGrupoProduto.BuscarPorCodigo(grupoProduto);
            configuracaoNaturezaOperacao.UFOrigem = !string.IsNullOrWhiteSpace(ufOrigem) ? repEstado.BuscarPorSigla(ufOrigem) : null;
            configuracaoNaturezaOperacao.UFDestino = !string.IsNullOrWhiteSpace(ufDestino) ? repEstado.BuscarPorSigla(ufDestino) : null;
            configuracaoNaturezaOperacao.Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null;

            configuracaoNaturezaOperacao.EstadoOrigemDiferente = estadoOrigemDiferente;
            configuracaoNaturezaOperacao.EstadoDestinoDiferente = estadoDestinoDiferente;
            configuracaoNaturezaOperacao.EstadoOrigemDiferenteUFDestino = ufOrigemDiferenteDeUFDestino;
            configuracaoNaturezaOperacao.EstadoEmissorDiferenteUFOrigem = estadoEmissorDiferenteUFOrigem;
            configuracaoNaturezaOperacao.EstadoOrigemIgualUFDestino = ufOrigemIgualUFDestino;
            configuracaoNaturezaOperacao.RotaFrete = repRotaFrete.BuscarPorCodigo(rotaFrete);

            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacaoExistente = repConfiguracaoNaturezaOperacao.BuscarPorParametros(remetente, destinatario, tomador, empresa, grupoRemetente, grupoDestinatario, grupoTomador, atividadeRemente, atividadeDestinatario, atividadeTomador, modeloDocumentoFiscal, tipoOperacao, grupoProduto, ufOrigem, ufDestino, estadoOrigemDiferente, estadoDestinoDiferente, ufOrigemDiferenteDeUFDestino, ufOrigemIgualUFDestino, estadoEmissorDiferenteUFOrigem, rotaFrete, categoriaRemetente, categoriaDestinatario, categoriaTomador);

            if (configuracaoNaturezaOperacaoExistente != null && configuracaoNaturezaOperacaoExistente.Codigo != configuracaoNaturezaOperacao.Codigo)
                retorno = "Já existe uma regra cadastrada para essa configuração de Centros de Resultado";

            return retorno;
        }

        private void SalvarConfiguracoesDeContabilizacao(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao repConfiguracaoNaturezaOperacaoContabilizacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao(unidadeDeTrabalho);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unidadeDeTrabalho);

            dynamic dynConfiguracaoNaturezaOperacaoContabilizacaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoNaturezaOperacaoContabilizacaos"));

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao> contabilizacoes = repConfiguracaoNaturezaOperacaoContabilizacao.BuscarPorConfiguracaoNaturezaOperacao(configuracaoNaturezaOperacao.Codigo);
            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao contabilizacao in contabilizacoes)
                repConfiguracaoNaturezaOperacaoContabilizacao.Deletar(contabilizacao, historico != null ? Auditado : null, historico);

            if (dynConfiguracaoNaturezaOperacaoContabilizacaos.Count > 0)
            {
                foreach (var dynConfiguracaoNaturezaOperacaoContabilizacao in dynConfiguracaoNaturezaOperacaoContabilizacaos)
                {
                    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao contabilizacao = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao();
                    contabilizacao.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId((int)dynConfiguracaoNaturezaOperacaoContabilizacao.NaturezaOperacao.Codigo);
                    contabilizacao.TipoContaContabil = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil)dynConfiguracaoNaturezaOperacaoContabilizacao.TipoContaContabil;
                    contabilizacao.ConfiguracaoNaturezaOperacao = configuracaoNaturezaOperacao;
                    repConfiguracaoNaturezaOperacaoContabilizacao.Inserir(contabilizacao, historico != null ? Auditado : null, historico);
                }
            }
        }

        private void SalvarConfiguracoesDeEscrituracao(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao configuracaoNaturezaOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao repConfiguracaoNaturezaOperacaoEscrituracao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao(unidadeDeTrabalho);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unidadeDeTrabalho);

            dynamic dynConfiguracaoNaturezaOperacaoEscrituracaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoNaturezaOperacaoEscrituracaos"));

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao> contabilizacoes = repConfiguracaoNaturezaOperacaoEscrituracao.BuscarPorConfiguracaoNaturezaOperacao(configuracaoNaturezaOperacao.Codigo);
            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao escrituracao in contabilizacoes)
                repConfiguracaoNaturezaOperacaoEscrituracao.Deletar(escrituracao, Auditado, historico);

            if (dynConfiguracaoNaturezaOperacaoEscrituracaos.Count > 0)
            {
                foreach (var dynConfiguracaoNaturezaOperacaoEscrituracao in dynConfiguracaoNaturezaOperacaoEscrituracaos)
                {
                    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao escrituracao = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao();
                    escrituracao.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId((int)dynConfiguracaoNaturezaOperacaoEscrituracao.NaturezaOperacao.Codigo);
                    escrituracao.TipoContaContabil = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil)dynConfiguracaoNaturezaOperacaoEscrituracao.TipoContaContabil;
                    escrituracao.ConfiguracaoNaturezaOperacao = configuracaoNaturezaOperacao;
                    repConfiguracaoNaturezaOperacaoEscrituracao.Inserir(escrituracao, Auditado, historico);
                }
            }
        }

        #endregion
    }
}
