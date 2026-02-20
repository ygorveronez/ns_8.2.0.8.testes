using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Cargas.CTeAgrupado
{
    [CustomAuthorize("Cargas/CargaCTeAgrupado")]
    public class CargaCTeAgrupadoController : BaseController
    {
        #region Construtores

        public CargaCTeAgrupadoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                if (!PreencherEntidade(out Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, out string mensagemErro, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { Codigo = cargaCTeAgrupado.Codigo });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(codigo, false);

                if (cargaCTeAgrupado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    cargaCTeAgrupado.Codigo,
                    cargaCTeAgrupado.Numero,
                    cargaCTeAgrupado.NumeroCargas,
                    cargaCTeAgrupado.Situacao,
                    cargaCTeAgrupado.ObservacaoCTe,
                    cargaCTeAgrupado.MotivoCancelamento,
                    cargaCTeAgrupado.Mensagem,
                    cargaCTeAgrupado.GerarCTePorCarga
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCarga()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaCarga());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaCarga()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaCarga();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> AlterarMoedaCTeAgrupado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CargaCTeAgrupado");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarMoeda))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                if (!ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                    return new JsonpResult("Não é possível utilizar moeda estrangeira neste ambiente.");

                int codigoCargaCTeAgrupado = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");
                decimal valorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda");

                if (valorCotacaoMoeda <= 0m)
                    return new JsonpResult(false, true, "O valor da cotação da moeda deve ser maior que zero.");

                if (!moeda.HasValue)
                    return new JsonpResult(false, true, "Moeda inválida.");

                if (!new Servicos.Embarcador.Carga.Moeda(unitOfWork, Auditado).AlterarMoedaCTeAgrupado(out string erro, codigoCargaCTeAgrupado, moeda.Value, valorCotacaoMoeda))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a moeda da carga.");
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
                int codigoCargaCTeAgrupado = Request.GetIntParam("Codigo");

                string motivoCancelamento = Request.GetStringParam("Motivo");

                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCTeAgrupadoCTes = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCTeAgrupado.BuscarPorCodigo(codigoCargaCTeAgrupado, true);

                if (cargaCTeAgrupado == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                if (cargaCTeAgrupado.Situacao != SituacaoCargaCTeAgrupado.Finalizado &&
                    cargaCTeAgrupado.Situacao != SituacaoCargaCTeAgrupado.Rejeitado &&
                    cargaCTeAgrupado.Situacao != SituacaoCargaCTeAgrupado.AgIntegracao &&
                    cargaCTeAgrupado.Situacao != SituacaoCargaCTeAgrupado.FalhaIntegracao)
                    return new JsonpResult(false, true, $"A situação ({cargaCTeAgrupado.Situacao.ObterDescricao()}) não permite que o registro seja cancelado.");

                if (string.IsNullOrWhiteSpace(motivoCancelamento) || motivoCancelamento.Length < 20)
                    return new JsonpResult(false, true, "O motivo do cancelamento deve ter no mínimo 20 caracteres.");

                List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> cargaCTeAgrupadoCTes = repCTeAgrupadoCTes.BuscarPorCargaCTeAgrupado(cargaCTeAgrupado.Codigo);

                if (cargaCTeAgrupadoCTes.Any(o => o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Autorizada &&
                                                  o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Rejeitada &&
                                                  o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Cancelada &&
                                                  o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Inutilizada))
                    return new JsonpResult(false, true, "A situação dos CT-es não permite que os mesmos sejam cancelados.");

                string erro = null;

                if (cargaCTeAgrupadoCTes.Any(o => o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Autorizada && !Servicos.Embarcador.CTe.CTe.VerificarSeCTeEstaAptoParaCancelamento(out erro, o.CTe, unitOfWork)))
                    return new JsonpResult(false, true, erro);

                cargaCTeAgrupado.Initialize();

                unitOfWork.Start();

                cargaCTeAgrupado.Situacao = SituacaoCargaCTeAgrupado.EmCancelamento;
                cargaCTeAgrupado.MotivoCancelamento = motivoCancelamento;
                cargaCTeAgrupado.EnviouDocumentosParaCancelamento = false;

                repCTeAgrupado.Atualizar(cargaCTeAgrupado);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = cargaCTeAgrupado.GetChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeAgrupado, alteracoes, "Solicitou o cancelamento.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a moeda da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reprocessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCTeAgrupado = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCTeAgrupadoCTes = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(codigoCargaCTeAgrupado, true);

                if (cargaCTeAgrupado == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                if (cargaCTeAgrupado.Situacao != SituacaoCargaCTeAgrupado.Rejeitado)
                    return new JsonpResult(false, true, $"A situação ({cargaCTeAgrupado.Situacao.ObterDescricao()}) não permite o reprocessamento.");

                if (repCTeAgrupadoCTes.ExisteCTeDiferenteDeAutorizado(cargaCTeAgrupado.Codigo))
                    return new JsonpResult(false, true, "A situação dos CT-es não permite que o reprocessamento.");

                unitOfWork.Start();

                cargaCTeAgrupado.Situacao = SituacaoCargaCTeAgrupado.EmEmissao;

                repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = cargaCTeAgrupado.GetChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeAgrupado, alteracoes, "Solicitou o reprocessamento.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a moeda da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool PreencherEntidade(out Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);

            cargaCTeAgrupado = new Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado();

            if (cargaCTeAgrupado.Codigo <= 0)
            {
                cargaCTeAgrupado.Data = DateTime.Now;
                cargaCTeAgrupado.ObservacaoCTe = Request.GetStringParam("ObservacaoCTe");
                cargaCTeAgrupado.GerarCTePorCarga = Request.GetBoolParam("GerarCTePorCarga");
                cargaCTeAgrupado.Numero = repCargaCTeAgrupado.BuscarProximoNumero();
            }

            cargaCTeAgrupado.Situacao = SituacaoCargaCTeAgrupado.EmEmissao;

            repCargaCTeAgrupado.Inserir(cargaCTeAgrupado, Auditado);

            if (!PreencherCargas(out mensagemErro, cargaCTeAgrupado, unitOfWork))
                return false;

            repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);

            return true;
        }

        private bool PreencherCargas(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga repCargaCTeAgrupadoCarga = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado.FiltroSelecaoCargas filtros = ObterFiltroSelecaoCargas();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado.CargaSelecionada> cargasSelecionadas = repCarga.ObterCargasSelecionadasParaCTeAgrupado(filtros);

            if (cargasSelecionadas.Count > 20)
            {
                mensagemErro = "São permitidas apenas 20 cargas por CT-e.";
                return false;
            }

            cargaCTeAgrupado.NumeroCargas = string.Join(", ", cargasSelecionadas.Select(o => o.NumeroCarga));

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado.CargaSelecionada cargaSelecionada in cargasSelecionadas)
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga cargaCTeAgrupadoCarga = new Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga()
                {
                    Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = cargaSelecionada.Codigo },
                    CargaCTeAgrupado = cargaCTeAgrupado
                };

                repCargaCTeAgrupadoCarga.Inserir(cargaCTeAgrupadoCarga);
            }

            mensagemErro = string.Empty;
            return true;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado.FiltroSelecaoCargas ObterFiltroSelecaoCargas()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado.FiltroSelecaoCargas()
            {
                CodigoCTeAgrupado = Request.GetIntParam("Codigo"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoMotorista = Request.GetListParam<int>("Motorista"),
                CodigoTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigoTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigoVeiculo = Request.GetListParam<int>("Veiculo"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                SemCTeAgrupado = Request.GetNullableBoolParam("SemCTeAgrupado"),
                CargasSelecionadas = Request.GetListParam<int>("ListaCargas"),
                SelecionarTodos = Request.GetBoolParam("SelecionarTodos"),
                Situacao = new List<SituacaoCarga>() { SituacaoCarga.EmTransporte, SituacaoCarga.Encerrada }
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCTe = Request.GetIntParam("CTe");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cargas", "NumeroCargas", 45, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CT-e Gerado", "CTe", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.left, false);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> listaCargaCTeAgrupado = repCargaCTeAgrupado.Consultar(codigoCarga, codigoCTe, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCargaCTeAgrupado.ContarConsulta(codigoCarga, codigoCTe, situacao);

                dynamic retorno = listaCargaCTeAgrupado.Select(o => new
                {
                    o.Codigo,
                    o.Numero,
                    Data = o.Data.ToString("dd/MM/yyyy HH:mm"),
                    o.NumeroCargas,
                    Situacao = o.Situacao.ObterDescricao(),
                    CTe = string.Join(", ", repCargaCTeAgrupadoCTe.BuscarNumeroCTesPorCTeAgrupado(o.Codigo)),
                    o.Mensagem
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CTeAgrupado.FiltroSelecaoCargas filtros = ObterFiltroSelecaoCargas();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "DataCriacaoCarga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 10, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                dynamic listaCarga = repCarga.ConsultarParaCTeAgrupado(filtros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCarga.ContarConsultaParaCTeAgrupado(filtros);

                grid.AdicionaRows(listaCarga);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
