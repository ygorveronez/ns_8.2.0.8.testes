using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zen.Barcode;
using Dominio.ObjetosDeValor.Relatorios;
using Utilidades.Extensions;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize(new string[] { "FiltroBusca" }, "WMS/Deposito", "WMS/Posicao", "WMS/Bloco", "WMS/Rua")]
    public class DepositoController : BaseController
    {
		#region Construtores

		public DepositoController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> FiltroBusca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                RetornoFiltro retorno = FiltroBuscaEndereco(unitOfWork);

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
                Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.Deposito deposito = new Dominio.Entidades.Embarcador.WMS.Deposito();

                // Preenche entidade com dados
                PreencheEntidade(ref deposito, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(deposito, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Inserir(deposito, Auditado);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.Deposito deposito = repDeposito.BuscarPorCodigo(codigo, true);

                // Valida
                if (deposito == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref deposito, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(deposito, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Atualizar(deposito, Auditado);

                unitOfWork.CommitChanges();

                // Replica Cascata
                Servicos.Embarcador.WMS.Deposito.AtulizarAbreviacaoDeposito(deposito, unitOfWork);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.Deposito deposito = repDeposito.BuscarPorCodigo(codigo);

                // Valida
                if (deposito == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Valida entidade
                if (!ValidaExclusao(deposito, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Deletar(deposito, Auditado);
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


        public async Task<IActionResult> ImprimirEtiqueta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R245_EtiquetaDeposito, TipoServicoMultisoftware);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R245_EtiquetaDeposito, TipoServicoMultisoftware, "Etiqueta Depósito", "Deposito", "EtiquetaDeposito.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                int codigo = 0;
                string tipoEtiqueta = "";
                int.TryParse(Request.Params("Codigo"), out codigo);
                tipoEtiqueta = Request.Params("TipoEtiqueta");

                string nomeEtiqueta = "EtiquetaDeposito.rpt";
                List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> dadosEtiqueta = new List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito>();
                switch (tipoEtiqueta)
                {
                    case nameof(Dominio.Relatorios.Embarcador.Enumeradores.EtiquetaDeposito.ETIQUETA_DEPOSITO):
                        dadosEtiqueta = RetornarDadosEtiqueta(codigo, unitOfWork);
                        break;
                    case nameof(Dominio.Relatorios.Embarcador.Enumeradores.EtiquetaDeposito.ETIQUETA_POSICAO):
                        dadosEtiqueta = RetornarDadosEtiquetaPosicao(codigo, unitOfWork);
                        break;
                }

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                return Etiquetas(dadosEtiqueta, relatorioControleGeracao, nomeEtiqueta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private JsonpResult Etiquetas(List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> dadosEtiqueta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, string nomeEtiqueta)
        {
            string stringConexao = _conexao.StringConexao;
            Task.Factory.StartNew(() => GerarEtiquetasDeposito(stringConexao, relatorioControleGeracao, dadosEtiqueta, nomeEtiqueta));
            return new JsonpResult(true);
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> RetornarDadosEtiquetaPosicao(int codigoPosicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
            Dominio.Entidades.Embarcador.WMS.DepositoPosicao depositoPosicao = repDepositoPosicao.BuscarPorCodigo(codigoPosicao);
            Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito etiqueta = new Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito();

            List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> listaRetorno = new List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito>();

            string codigoBarrasFormatado = depositoPosicao.Codigo.ToString().PadLeft(14, '0');
            byte[] codigoBarras = Utilidades.Barcode.Gerar(codigoBarrasFormatado, ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);

            etiqueta.Descricao = depositoPosicao.Abreviacao;
            etiqueta.CodigoBarras = codigoBarras;

            listaRetorno.Add(etiqueta);

            return listaRetorno;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> RetornarDadosEtiqueta(int codigoDeposito, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);
            Dominio.Entidades.Embarcador.WMS.Deposito depositoRegistro = repDeposito.BuscarPorCodigo(codigoDeposito);
            Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito etiqueta = new Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito();

            List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> listaRetorno = new List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito>();

            string codigoBarrasFormatado = depositoRegistro.Codigo.ToString().PadLeft(14, '0');
            byte[] codigoBarras = Utilidades.Barcode.Gerar(codigoBarrasFormatado, ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);

            etiqueta.Descricao = depositoRegistro.Descricao;
            etiqueta.CodigoBarras = codigoBarras;

            listaRetorno.Add(etiqueta);

            return listaRetorno;
        }

        private void GerarEtiquetasDeposito(string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaDeposito> dadosEtiqueta, string nomeEtiqueta)
        {
            string nomeCliente = Cliente.NomeFantasia;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            var result = ReportRequest.WithType(ReportType.EtiquetaDeposito)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("DadosEtiqueta", dadosEtiqueta.ToJson()) // TODO: Verificar serializacao pq esta sendo passado a classe inteira
                .AddExtraData("NomeCliente", nomeCliente)
                .AddExtraData("NomeEtiqueta", nomeEtiqueta)
                .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                .CallReport();

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork).RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.WMS.Deposito deposito, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            string codIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;

            deposito.Ativo = ativo;
            deposito.Descricao = descricao;
            deposito.CodigoIntegracao = codIntegracao;
            deposito.NumeroUnidadeImpressao = Request.GetIntParam("NumeroUnidadeImpressao");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.Deposito deposito, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (deposito.Descricao.Length == 0)
            {
                msgErro = "Descrição é Obrigatória.";
                return false;
            }

            return true;
        }

        private bool ValidaExclusao(Dominio.Entidades.Embarcador.WMS.Deposito deposito, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            Repositorio.Embarcador.WMS.DepositoRua repDepositoRua = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);

            // Busca informacoes
            int ruas = repDepositoRua.ContarRuasPorDeposito(deposito.Codigo);

            if (ruas > 0)
            {
                msgErro = "Não é possível excluir um depósito quando já existe ruas vinculadas.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);

            string descricao = Request.Params("DescricaoBusca") ?? string.Empty;

            List<Dominio.Entidades.Embarcador.WMS.Deposito> listaGrid = repDeposito.Consultar(descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDeposito.ContarConsulta(descricao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.CodigoIntegracao,
                            Status = obj.Ativo,
                            obj.DescricaoAtivo,
                            obj.NumeroUnidadeImpressao
                        };

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Status", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("NumeroUnidadeImpressao", false);

            return grid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoAtivo")
                propOrdenar = "Ativo";
        }

        private RetornoFiltro FiltroBuscaEndereco(Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);
            Repositorio.Embarcador.WMS.DepositoRua repDepositoRua = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);
            Repositorio.Embarcador.WMS.DepositoBloco repDepositoBloco = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

            // Dados do filtro
            string abreviacao = Request.Params("Abreviacao") ?? string.Empty;

            int.TryParse(Request.Params("Deposito"), out int codigoDeposito);
            int.TryParse(Request.Params("Rua"), out int codigoRua);
            int.TryParse(Request.Params("Bloco"), out int codigoBloco);
            int.TryParse(Request.Params("Posicao"), out int codigoPosicao);

            // Consulta
            if (codigoPosicao > 0 || !string.IsNullOrWhiteSpace(abreviacao))
            {
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao depositoPosicao = repDepositoPosicao.BuscarPorCodigoEAbreviacao(codigoPosicao, abreviacao);
                if (depositoPosicao != null)
                {
                    return new RetornoFiltro
                    {
                        Deposito = new { depositoPosicao.Bloco.Rua.Deposito.Codigo, depositoPosicao.Bloco.Rua.Deposito.Descricao },
                        Rua = new { depositoPosicao.Bloco.Rua.Codigo, depositoPosicao.Bloco.Rua.Descricao },
                        Bloco = new { depositoPosicao.Bloco.Codigo, depositoPosicao.Bloco.Descricao },
                        Posicao = new { depositoPosicao.Codigo, Descricao = depositoPosicao.Abreviacao },
                    };
                }
            }

            if (codigoBloco > 0)
            {
                Dominio.Entidades.Embarcador.WMS.DepositoBloco depositoBloco = repDepositoBloco.BuscarPorCodigo(codigoBloco);
                if (depositoBloco != null)
                {
                    return new RetornoFiltro
                    {
                        Deposito = new { depositoBloco.Rua.Deposito.Codigo, depositoBloco.Rua.Deposito.Descricao },
                        Rua = new { depositoBloco.Rua.Codigo, depositoBloco.Rua.Descricao },
                        Bloco = new { depositoBloco.Codigo, depositoBloco.Descricao },
                        Posicao = null
                    };
                }
            }

            if (codigoRua > 0)
            {
                Dominio.Entidades.Embarcador.WMS.DepositoRua depositoRua = repDepositoRua.BuscarPorCodigo(codigoRua);
                if (depositoRua != null)
                {
                    return new RetornoFiltro
                    {
                        Deposito = new { depositoRua.Deposito.Codigo, depositoRua.Deposito.Descricao },
                        Rua = new { depositoRua.Codigo, depositoRua.Descricao },
                        Bloco = null,
                        Posicao = null
                    };
                }
            }

            if (codigoDeposito > 0)
            {
                Dominio.Entidades.Embarcador.WMS.Deposito depositoDeposito = repDeposito.BuscarPorCodigo(codigoDeposito);
                if (depositoDeposito != null)
                {
                    return new RetornoFiltro
                    {
                        Deposito = new { depositoDeposito.Codigo, depositoDeposito.Descricao },
                        Rua = null,
                        Bloco = null,
                        Posicao = null
                    };
                }
            }

            return new RetornoFiltro
            {
                Deposito = null,
                Rua = null,
                Bloco = null,
                Posicao = null
            };
        }

        #endregion

        public class RetornoFiltro
        {
            public dynamic Deposito { get; set; }
            public dynamic Rua { get; set; }
            public dynamic Bloco { get; set; }
            public dynamic Posicao { get; set; }
        }
    }
}
