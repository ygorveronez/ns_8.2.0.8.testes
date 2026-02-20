using SGTAdmin.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Seguros
{
    [CustomAuthorize("Seguros/FechamentoAverbacao")]
    public class FechamentoAverbacaoController : BaseController
    {
		#region Construtores

		public FechamentoAverbacaoController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> PesquisaAverbacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaAverbacoes();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaAverbacoes(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaAverbacoes(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento = repFechamentoAverbacao.BuscarPorCodigo(codigo);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    fechamento.Codigo,
                    fechamento.Situacao,
                    DadosFechamento = new
                    {
                        Numero = fechamento.Numero,
                        Transportador = fechamento.Transportador == null ? null : new { fechamento.Transportador.Codigo, Descricao = fechamento.Transportador.RazaoSocial },
                        TipoOperacao = fechamento.TipoOperacao == null ? null : new { fechamento.TipoOperacao.Codigo, fechamento.TipoOperacao.Descricao },
                        DataInicio = fechamento.DataInicial.HasValue ? fechamento.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                        DataFim = fechamento.DataFinal.HasValue ? fechamento.DataFinal.Value.ToString("dd/MM/yyyy") : ""
                    },
                    Averbacoes = new
                    {
                        Adicional = fechamento.Adicional.ToString("n2"),
                        IOF = fechamento.IOF.ToString("n2")
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

        public async Task<IActionResult> GerarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento = new Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao();

                // Preenche entidade com dados
                PreencheEntidade(ref fechamento, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(fechamento, out string erro))
                    return new JsonpResult(false, true, erro);

                repFechamentoAverbacao.Inserir(fechamento, Auditado);

                // Vincula averbacoes
                VincularAverbacoesFiltradas(fechamento, unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    fechamento.Codigo
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

        public async Task<IActionResult> AtualizarValores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento = repFechamentoAverbacao.BuscarPorCodigo(codigo, true);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fechamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Aberta)
                    return new JsonpResult(false, true, "A situação do fechamento não permite essa operação");

                // Preenche entidade com dados
                AtualizarValoresAverbacoes(fechamento, unitOfWork);

                // Persiste dados
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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento = repFechamentoAverbacao.BuscarPorCodigo(codigo, true);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fechamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Aberta)
                    return new JsonpResult(false, true, "A situação do fechamento não permite essa operação");

                // Preenche entidade com dados
                fechamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Fechada;
                FinalizarAverbacoes(fechamento, unitOfWork);

                // Persiste dados
                repFechamentoAverbacao.Atualizar(fechamento);
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

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento = repFechamentoAverbacao.BuscarPorCodigo(codigo, true);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fechamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Cancelada)
                    return new JsonpResult(false, true, "A situação do fechamento não permite essa operação");

                CancelaAverbacoes(fechamento, unitOfWork);
                if (fechamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Aberta)
                {
                    fechamento.Averbacoes.Clear();
                    repFechamentoAverbacao.Deletar(fechamento);
                }
                else
                {
                    fechamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Cancelada;
                }

                // Persiste dados
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
        #endregion

        #region Métodos Privados
        private void VincularAverbacoesFiltradas(Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Repos
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Seguros.AverbacoesFechamento repAverbacoesFechamento = new Repositorio.Embarcador.Seguros.AverbacoesFechamento(unitOfWork);

            // Dados
            int transportador = fechamento.Transportador?.Codigo ?? 0;
            int tipoOperacao = fechamento.TipoOperacao?.Codigo ?? 0;
            DateTime? dataInicio = fechamento.DataInicial;
            DateTime? dataFim = fechamento.DataFinal;

            // Busca Averbacoes
            List<Dominio.Entidades.AverbacaoCTe> averbacoesFiltradas = repAverbacaoCTe.BuscarAverbacoesFechamento(transportador, tipoOperacao, dataInicio, dataFim);

            foreach (Dominio.Entidades.AverbacaoCTe averbacao in averbacoesFiltradas)
            {
                averbacao.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.EmFechamento;
                repAverbacaoCTe.Atualizar(averbacao);

                Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento averbacaoFechamento = new Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento()
                {
                    AverbacaoCTe = averbacao,
                    FechamentoAverbacao = fechamento
                };
                repAverbacoesFechamento.Inserir(averbacaoFechamento);
            }
        }

        private void AtualizarValoresAverbacoes(Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Repos
            Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);
            Repositorio.Embarcador.Seguros.AverbacoesFechamento repAverbacoesFechamento = new Repositorio.Embarcador.Seguros.AverbacoesFechamento(unitOfWork);

            // Valores Atualizacao
            decimal.TryParse(Request.Params("IOF"), out decimal iof);
            decimal.TryParse(Request.Params("Adicional"), out decimal adicional);

            fechamento.IOF = iof;
            fechamento.Adicional = adicional;
            repFechamentoAverbacao.Atualizar(fechamento, Auditado);

            int totalAverbacoes = fechamento.Averbacoes.Count();
            decimal adicionalRateado = Math.Floor((fechamento.Adicional / totalAverbacoes) * 100m) / 100m;
            decimal diferencaAdicional = adicional - (adicionalRateado * totalAverbacoes);

            for (var i = 0; i < totalAverbacoes; i++)
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento averbacao = fechamento.Averbacoes[i];
                averbacao.Initialize();
                averbacao.IOF = (averbacao.AverbacaoCTe.CTe?.ValorAReceber ?? averbacao.AverbacaoCTe.CargaDocumentoParaEmissaoNFSManual.ValorFrete) * (fechamento.IOF / 100m);

                // Joga diferença pro último carinha
                if ((totalAverbacoes - 1) == i)
                    averbacao.Adicional = adicionalRateado + diferencaAdicional;
                else
                    averbacao.Adicional = adicionalRateado;
                repAverbacoesFechamento.Atualizar(averbacao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Atualizou valor do Fechamento.", unitOfWork);
            }
        }

        private void FinalizarAverbacoes(Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Repos
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento averbacao in fechamento.Averbacoes)
            {
                averbacao.AverbacaoCTe.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.Finalizada;
                averbacao.AverbacaoCTe.IOF = averbacao.IOF;
                averbacao.AverbacaoCTe.Adicional = averbacao.Adicional;
                repAverbacaoCTe.Atualizar(averbacao.AverbacaoCTe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Finalizou Fechamento.", unitOfWork);
            }
        }

        private void CancelaAverbacoes(Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Repos
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento averbacao in fechamento.Averbacoes)
            {
                averbacao.AverbacaoCTe.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.EmAberto;
                repAverbacaoCTe.Atualizar(averbacao.AverbacaoCTe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Cancelou Fechamento.", unitOfWork);
            }
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Numero", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data Fechamento", "DataFechamento", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);

            return grid;
        }
        private Models.Grid.Grid GridPesquisaAverbacoes()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "NumeroSerie", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Valor do CT-e", "ValorCTe", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("% Desconto", "PercentualDesconto", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Total Desconto", "Desconto", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("IOF", "IOF", 7, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Adicional", "Adicional", 7, Models.Grid.Align.right, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisaAverbacoes(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.AverbacoesFechamento repAverbacoesFechamento = new Repositorio.Embarcador.Seguros.AverbacoesFechamento(unitOfWork);

            int.TryParse(Request.Params("Codigo"), out int fechamento);

            // Consulta
            List<Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento> listaGrid = repAverbacoesFechamento.Consultar(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repAverbacoesFechamento.ContarConsulta(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            NumeroSerie = obj.AverbacaoCTe.CTe != null ? (obj.AverbacaoCTe.CTe.Numero + " - " + obj.AverbacaoCTe.CTe.Serie.Numero) : (obj.AverbacaoCTe.XMLNotaFiscal.Numero + " - " + obj.AverbacaoCTe.XMLNotaFiscal.Serie),
                            DataEmissao = obj.AverbacaoCTe.CTe != null ? (obj.AverbacaoCTe.CTe.DataEmissao.HasValue ? obj.AverbacaoCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty) : obj.AverbacaoCTe.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy"),
                            Transportador = obj.AverbacaoCTe.CTe?.Empresa.RazaoSocial ?? "",
                            ValorMercadoria = obj.AverbacaoCTe.CTe?.ValorTotalMercadoria.ToString("n2") ?? obj.AverbacaoCTe.XMLNotaFiscal.Valor.ToString("n2"),
                            ValorCTe = obj.AverbacaoCTe.CTe?.ValorAReceber.ToString("n2") ?? obj.AverbacaoCTe.CargaDocumentoParaEmissaoNFSManual.ValorFrete.ToString("n2"),
                            PercentualDesconto = obj.AverbacaoCTe.Percentual.HasValue ? obj.AverbacaoCTe.Percentual.Value.ToString("n2") : string.Empty,
                            Desconto = obj.AverbacaoCTe.Desconto.ToString("n2"),
                            IOF = obj.IOF.ToString("n2"),
                            Adicional = obj.Adicional.ToString("n2")
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
            int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
            int.TryParse(Request.Params("Transportador"), out int transportador);

            // Consulta
            List<Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao> listaGrid = repFechamentoAverbacao.Consultar(numeroInicial, numeroFinal, transportador, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repFechamentoAverbacao.ContarConsulta(numeroInicial, numeroFinal, transportador);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.Numero,
                            DataFechamento = obj.DataFechamento.ToString("dd/MM/yyyy"),
                            Situacao = obj.DescricaoSituacao,
                            Transportador = obj.Transportador?.RazaoSocial ?? string.Empty
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Seguros.FechamentoAverbacao repFechamentoAverbacao = new Repositorio.Embarcador.Seguros.FechamentoAverbacao(unitOfWork);

            // Converte valores
            DateTime? dataInicial = null;
            if (DateTime.TryParse(Request.Params("DataInicio"), out DateTime auxDataInicio))
                dataInicial = auxDataInicio;
            DateTime? dataFinal = null;
            if (DateTime.TryParse(Request.Params("DataFim"), out DateTime auxDataFinal))
                dataFinal = auxDataFinal;

            int.TryParse(Request.Params("Transportador"), out int codigoTransportador);
            Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(codigoTransportador);

            int.TryParse(Request.Params("TipoOperacao"), out int codigoTipoOperacao);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

            // Dados Criacao 
            fechamento.Numero = repFechamentoAverbacao.BuscarProximoCodigo();
            fechamento.DataInicial = dataInicial;
            fechamento.DataFinal = dataFinal;
            fechamento.Transportador = transportador;
            fechamento.TipoOperacao = tipoOperacao;
            fechamento.Usuario = this.Usuario;
            fechamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Aberta;
            fechamento.DataFechamento = DateTime.Now;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao fechamento, out string msgErro)
        {
            msgErro = "";

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Transportador") propOrdenar = "Transportador.RazaoSocial";
        }
        private void PropOrdenaAverbacoes(ref string propOrdenar)
        {
            if (propOrdenar == "NumeroSerie") propOrdenar = "AverbacaoCTe.CTe.Numero";
            else if (propOrdenar == "DataEmissao") propOrdenar = "AverbacaoCTe.CTe.DataEmissao";
            else if (propOrdenar == "Transportador") propOrdenar = "AverbacaoCTe.CTe.Empresa.RazaoSocial";
            else if (propOrdenar == "ValorMercadoria") propOrdenar = "AverbacaoCTe.CTe.ValorTotalMercadoria";
            else if (propOrdenar == "ValorCTe") propOrdenar = "AverbacaoCTe.CTe.ValorAReceber";
            else if (propOrdenar == "PercentualDesconto") propOrdenar = "AverbacaoCTe.Percentual";
            else if (propOrdenar == "Desconto") propOrdenar = "AverbacaoCTe.Desconto";
        }
        #endregion
    }
}
