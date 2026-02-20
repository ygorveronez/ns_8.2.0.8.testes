using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize(new string[] { "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "WMS/DescarteLoteProdutoEmbarcador", "WMS/DescarteLoteProduto")]
    public class DescarteLoteProdutoEmbarcadorController : BaseController
    {
		#region Construtores

		public DescarteLoteProdutoEmbarcadorController(Conexao conexao) : base(conexao) { }

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
        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> listaAutorizacao = repAprovacaoAlcadaDescarteLoteProduto.ConsultarAutorizacoesPorDescarte(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAprovacaoAlcadaDescarteLoteProduto.ContarConsultaAutorizacoesPorDescarte(codigo));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = TituloRegra(obj),
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 DT_RowColor = CoresRegras(obj)
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

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto autorizacao = repAprovacaoAlcadaDescarteLoteProduto.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = TituloRegra(autorizacao),
                    Situacao = autorizacao.DescricaoSituacao,
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,

                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = autorizacao.Motivo ?? string.Empty,
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte = repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(codigo);

                // Valida
                if (descarte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    descarte.Codigo,
                    descarte.Situacao,
                    DadosDescarte = new
                    {
                        LoteProdutoEmbarcador = descarte.Lote != null ? new { descarte.Lote.Codigo, descarte.Lote.Descricao } : null,
                        Produto = descarte.Produto != null ? new { descarte.Produto.Codigo, descarte.Produto.Descricao } : null,
                        Quantidade = descarte.Quantidade.ToString("n3"),
                        Numero = descarte.Lote?.Numero ?? "",
                        CodigoBarras = descarte.Lote?.CodigoBarras ?? "",
                        DataVencimento = descarte.Lote?.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        QuantidadeLote = descarte.Lote?.QuantidadeLote.ToString("n3") ?? "",
                        QuantidadeAtual = descarte.Lote?.QuantidadeAtual.ToString("n3") ?? "",
                        DepositoPosicao = descarte.Lote?.DepositoPosicao.Abreviacao ?? "",
                        descarte.Motivo
                    },
                    Resumo = new
                    {
                        ResumoSolicitacao = new
                        {
                            Situacao = descarte.DescricaoSituacao,
                            Data = descarte.Data.ToString("dd/MM/yyyy"),
                            Usuario = descarte.Usuario.Nome,
                            ProdutoEmbarcador = descarte.Lote?.ProdutoEmbarcador.Descricao ?? "",
                            Quantidade = descarte.Quantidade.ToString("n3"),
                            DepositoPosicao = descarte.Lote?.DepositoPosicao.Abreviacao ?? "",
                            CodigoBarra = descarte.Lote?.CodigoBarras ?? "",
                            Justificativa = descarte.Motivo,
                        },
                        ResumoRetorno = new
                        {
                            DataRetorno = descarte.DataAprovacao?.ToString("dd/MM/yyyy hh:mm") ?? " - ",
                            Aprovador = descarte.UsuarioAprovador?.Nome ?? " - ",
                        }
                    }
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
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte = new Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador();

                // Preenche entidade com dados
                PreencheEntidade(ref descarte, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(descarte, out string erro))
                    return new JsonpResult(false, true, erro);

                if (!Servicos.Embarcador.ProdutoEmbarcador.Lote.ValidaDescarteLote(descarte, unitOfWork))
                    return new JsonpResult(false, true, "Não é possível adicionar o descarte por não conter estoque suficiente.");

                repDescarteLoteProdutoEmbarcador.Inserir(descarte, Auditado);
                EtapaAprovacao(ref descarte, unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    descarte.Codigo
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

        [AllowAuthenticate]
        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte = repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(codigo);

                // Valida
                if (descarte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (descarte.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                if (!Servicos.Embarcador.ProdutoEmbarcador.Lote.ValidaDescarteLote(descarte, unitOfWork))
                    return new JsonpResult(false, true, "Não é possível reprocessar o descarte por não conter estoque suficiente.");

                // Inicia transacao
                unitOfWork.Start();

                // Busca as regras
                EtapaAprovacao(ref descarte, unitOfWork);

                // Persiste dados
                repDescarteLoteProdutoEmbarcador.Atualizar(descarte);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    descarte.Codigo,
                    PossuiRegra = descarte.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.AgAprovacao,
                    Finalizado = descarte.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Finalizado,
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Lote").Nome("Lote").Tamanho(15);
            grid.Prop("Produto").Nome("Produto").Tamanho(15);
            grid.Prop("Data").Nome("Data Do Descarte").Tamanho(20).Align(Models.Grid.Align.center);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("Situacao").Nome("Situação").Tamanho(20);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);

            // Dados do filtro
            DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicio);
            DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFim);

            int.TryParse(Request.Params("ProdutoEmbarcador"), out int produtoEmbarcador);
            int.TryParse(Request.Params("Produto"), out int produto);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador situacaoAux))
                situacao = situacaoAux;

            // Consulta
            List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> listaGrid = repDescarteLoteProdutoEmbarcador.Consultar(produto, dataInicio, dataFim, produtoEmbarcador, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDescarteLoteProdutoEmbarcador.ContarConsulta(produto, dataInicio, dataFim, produtoEmbarcador, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Lote = obj.Lote?.Descricao ?? "",
                            Produto = obj.Produto?.Descricao ?? "",
                            Data = obj.Data.ToString("dd/MM/yyyy"),
                            Quantidade = obj.Quantidade.ToString("n3"),
                            Situacao = obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("LoteProdutoEmbarcador"), out int lote);
            int.TryParse(Request.Params("Produto"), out int produto);
            string motivo = Request.Params("Motivo") ?? string.Empty;
            decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

            // Vincula dados
            descarte.Motivo = motivo;
            descarte.Quantidade = quantidade;
            descarte.Lote = repProdutoEmbarcadorLote.BuscarPorCodigo(lote);
            descarte.Produto = repProduto.BuscarPorCodigo(produto);

            // Dados Criacao 
            if (descarte.Codigo == 0)
            {
                descarte.Data = DateTime.Now;
                descarte.Usuario = this.Usuario;
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, out string msgErro)
        {
            msgErro = "";

            if (descarte.Lote == null)
            {
                msgErro = "Lote ou o Produto são obrigatórios.";
                return false;
            }

            if (descarte.Quantidade == 0)
            {
                msgErro = "Quantidade é obrigatória.";
                return false;
            }

            if (descarte.Lote != null && descarte.Quantidade > descarte.Lote.QuantidadeAtual)
            {
                msgErro = "Quantidade informada não pode ser maior que a Quantidade Atual.";
                return false;
            }

            if (descarte.Motivo.Length == 0)
            {
                msgErro = "Motivo é obrigatório.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Lote") propOrdenar = "Lote.Descricao";
            if (propOrdenar == "Produto") propOrdenar = "Produto.Descricao";
        }

        private void EtapaAprovacao(ref Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> regras = Servicos.Embarcador.ProdutoEmbarcador.Lote.VerificarRegrasAutorizacaoDescarte(descarte, unitOfWork);

            bool possuiRegra = regras.Count() > 0;
            bool agAprovacao = true;

            if (possuiRegra)
            {
                descarte.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.AgAprovacao;

                agAprovacao = Servicos.Embarcador.ProdutoEmbarcador.Lote.CriarRegrasAutorizacao(regras, descarte, descarte.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);

                if (!agAprovacao)
                    descarte.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Finalizado;

                Servicos.Embarcador.ProdutoEmbarcador.Lote.DescarteLoteAprovado(descarte, unitOfWork, Auditado, TipoServicoMultisoftware);
            }
            else
            {
                descarte.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.SemRegra;
            }
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto regra)
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

        private string TituloRegra(Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto regra)
        {
            return regra.RegraDescarte?.Descricao;
        }

        #endregion
    }
}
