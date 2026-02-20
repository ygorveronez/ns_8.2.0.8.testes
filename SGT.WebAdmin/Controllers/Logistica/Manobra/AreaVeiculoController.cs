using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AreaVeiculo")]
    public class AreaVeiculoController : BaseController
    {
		#region Construtores

		public AreaVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = new Dominio.Entidades.Embarcador.Logistica.AreaVeiculo();

                PreencherAreaVeiculo(areaVeiculo, unitOfWork);

                unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork).Inserir(areaVeiculo, Auditado);

                AdicionarOuAtualizarPosicoes(areaVeiculo, unitOfWork);
                AdicionarOuAtualizarTiposRetornoCarga(areaVeiculo, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (areaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherAreaVeiculo(areaVeiculo, unitOfWork);

                unitOfWork.Start();

                repositorio.Atualizar(areaVeiculo, Auditado);

                AdicionarOuAtualizarPosicoes(areaVeiculo, unitOfWork);
                AdicionarOuAtualizarTiposRetornoCarga(areaVeiculo, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> BaixarQrCode()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAreaVeiculo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = repositorio.BuscarPorCodigo(codigoAreaVeiculo);

                if (areaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] pdf = new Servicos.Embarcador.Logistica.AreaVeiculo().ObterPdfQRCodeAreaVeiculo(areaVeiculo);

                return Arquivo(pdf, "application/pdf", $"QR Code {areaVeiculo.Descricao}.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o QR Code.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarTodosQrCode()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            TipoArquivo tipoArquivo = Request.GetEnumParam("TipoArquivo", TipoArquivo.PDF);

            try
            {
                int codigoAreaVeiculo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = repositorio.BuscarPorCodigo(codigoAreaVeiculo);

                if (areaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = servicoArquivo.Adicionar("QR Code Locais", Usuario, tipoArquivo);

                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => BaixarTodosQrCode(stringConexao, areaVeiculo, controleGeracaoArquivo));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, $"Ocorreu uma falha ao baixar o arquivo {tipoArquivo.ObterDescricao()} dos QR Code dos locais.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = repositorio.BuscarPorCodigo(codigo);

                if (areaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Dados = new
                    {
                        areaVeiculo.Codigo,
                        CentroCarregamento = new { areaVeiculo.CentroCarregamento.Codigo, areaVeiculo.CentroCarregamento.Descricao },
                        areaVeiculo.Descricao,
                        Status = areaVeiculo.Ativo,
                        areaVeiculo.Observacao,
                        areaVeiculo.QRCode,
                        areaVeiculo.Tipo
                    },
                    Posicoes = (
                        from posicao in areaVeiculo.Posicoes
                        select new
                        {
                            posicao.Codigo,
                            posicao.Descricao,
                            posicao.QRCode,
                            posicao.Desenho
                        }
                    ).ToList(),
                    TiposRetornoCarga = (
                        from tipoRetornoCarga in areaVeiculo.TiposRetornoCarga
                        select new
                        {
                            tipoRetornoCarga.Codigo,
                            tipoRetornoCarga.Descricao
                        }
                    ).ToList()
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (areaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(areaVeiculo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.AreaVeiculo repAreaVeiculo = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo> areasVeiculo = repAreaVeiculo.BuscarTodosAtivos();

                var retorno = (from area in areasVeiculo
                               select new
                               {
                                   value = area.Codigo,
                                   text = area.Descricao
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void BaixarTodosQrCode(string stringConexao, Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);

            try
            {
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorioAreaVeiculo = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                areaVeiculo = repositorioAreaVeiculo.BuscarPorCodigo(areaVeiculo.Codigo);

                if (areaVeiculo != null)
                {
                    Servicos.Embarcador.Logistica.AreaVeiculo servicoAreaVeiculo = new Servicos.Embarcador.Logistica.AreaVeiculo();
                    byte[] arquivoTodosQrCode = controleGeracaoArquivo.TipoArquivo == TipoArquivo.PDF ? servicoAreaVeiculo.ObterPdfTodosQRCodeAreaVeiculoPosicao(areaVeiculo) : servicoAreaVeiculo.ObterTodosPdfQRCodeCompactado(areaVeiculo);

                    servicoArquivo.SalvarArquivo(controleGeracaoArquivo, arquivoTodosQrCode);
                    servicoArquivo.Finalizar(controleGeracaoArquivo, nota: $"Geração do arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} dos QR Code dos locais concluído", urlPagina: "Logistica/AreaVeiculo");
                }
                else
                    servicoArquivo.Remover(controleGeracaoArquivo);
            }
            catch (Exception excecao)
            {
                servicoArquivo.FinalizarComFalha(controleGeracaoArquivo, nota: $"Ocorreu uma falha ao gerar o arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} dos QR Code dos locais concluído", urlPagina: "Logistica/AreaVeiculo", excecao: excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void AdicionarOuAtualizarPosicoes(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic posicoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Posicoes"));

            ExcluirPosicoesRemovidas(areaVeiculo, posicoes, unitOfWork);
            InserirPosicoesAdicionadas(areaVeiculo, posicoes, unitOfWork);
        }

        private void AdicionarOuAtualizarTiposRetornoCarga(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tiposRetornoCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposRetornoCarga"));

            ExcluirTiposRetornoCargaRemovidos(areaVeiculo, tiposRetornoCarga, unitOfWork);
            InserirTiposRetornoCargaAdicionados(areaVeiculo, tiposRetornoCarga, unitOfWork);
        }

        private void ExcluirPosicoesRemovidas(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, dynamic posicoes, Repositorio.UnitOfWork unitOfWork)
        {
            if (areaVeiculo.Posicoes?.Count > 0)
            {
                Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioAreaVeiculoPosicao = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var posicao in posicoes)
                {
                    int? codigo = ((string)posicao.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao> listaPosicoesRemover = (from posicao in areaVeiculo.Posicoes where !listaCodigosAtualizados.Contains(posicao.Codigo) select posicao).ToList();

                foreach (var posicao in listaPosicoesRemover)
                {
                    repositorioAreaVeiculoPosicao.Deletar(posicao);
                }

                if (listaPosicoesRemover.Count > 0)
                {
                    string descricaoAcao = listaPosicoesRemover.Count == 1 ? "Posição removida" : "Múltiplas posições removidas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, areaVeiculo, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ExcluirTiposRetornoCargaRemovidos(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, dynamic tiposRetornoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (areaVeiculo.TiposRetornoCarga?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var tipoRetornoCarga in tiposRetornoCarga)
                    listaCodigosAtualizados.Add(((string)tipoRetornoCarga.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> listaTipoRetornoCargaRemover = (from tipoRetornoCarga in areaVeiculo.TiposRetornoCarga where !listaCodigosAtualizados.Contains(tipoRetornoCarga.Codigo) select tipoRetornoCarga).ToList();

                foreach (var tipoRetornoCarga in listaTipoRetornoCargaRemover)
                    areaVeiculo.TiposRetornoCarga.Remove(tipoRetornoCarga);
            }
        }

        private void InserirPosicoesAdicionadas(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, dynamic posicoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioAreaVeiculoPosicao = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);
            int totalPosicoesAdicionadasOuAtualizadas = 0;

            foreach (var posicao in posicoes)
            {
                int? codigo = ((string)posicao.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao areaVeiculoPosicao;

                if (codigo.HasValue)
                    areaVeiculoPosicao = repositorioAreaVeiculoPosicao.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException("Posição não encontrada");
                else
                    areaVeiculoPosicao = new Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao();

                areaVeiculoPosicao.AreaVeiculo = areaVeiculo;
                areaVeiculoPosicao.Descricao = (string)posicao.Descricao;
                areaVeiculoPosicao.QRCode = (string)posicao.QRCode;
                areaVeiculoPosicao.Desenho = (string)posicao.Desenho;

                if (codigo.HasValue)
                    repositorioAreaVeiculoPosicao.Atualizar(areaVeiculoPosicao);
                else
                    repositorioAreaVeiculoPosicao.Inserir(areaVeiculoPosicao);

                totalPosicoesAdicionadasOuAtualizadas++;
            }

            if (areaVeiculo.IsInitialized() && (totalPosicoesAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = totalPosicoesAdicionadasOuAtualizadas == 1 ? "Posição adicionada ou atualizada" : "Múltiplas posições adicionadas ou atualizadas";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, areaVeiculo, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void InserirTiposRetornoCargaAdicionados(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, dynamic tiposRetornoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);

            if (areaVeiculo.TiposRetornoCarga == null)
                areaVeiculo.TiposRetornoCarga = new List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>();

            foreach (var tipoRetornoCarga in tiposRetornoCarga)
            {
                int codigo = ((string)tipoRetornoCarga.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCargaAdicionar = repositorioTipoRetornoCarga.BuscarPorCodigo(codigo) ?? throw new ControllerException("Tipo de retorno de carga não encontrado");

                if (!areaVeiculo.TiposRetornoCarga.Contains(tipoRetornoCargaAdicionar))
                    areaVeiculo.TiposRetornoCarga.Add(tipoRetornoCargaAdicionar);
            }
        }

        private void PreencherAreaVeiculo(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            areaVeiculo.Ativo = Request.GetBoolParam("Status");
            areaVeiculo.CentroCarregamento = ObterCentroCarregamento(unitOfWork);
            areaVeiculo.Descricao = Request.GetStringParam("Descricao");
            areaVeiculo.Observacao = Request.GetNullableStringParam("Observacao");
            areaVeiculo.QRCode = Request.GetStringParam("QRCode");
            areaVeiculo.Tipo = Request.GetEnumParam<TipoAreaVeiculo>("Tipo");
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");

            if (codigoCentroCarregamento == 0)
                throw new ControllerException("Centro de carregamento deve ser informado");

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorio = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ControllerException("Centro de carregamento não encontrado");
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculo()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                Tipo = Request.GetNullableEnumParam<TipoAreaVeiculo>("Tipo")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculo filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 14, Models.Grid.Align.center, false);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo> listaAreaVeiculo = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculo>();

                var listaAreaVeiculoRetornar = (
                    from areaVeiculo in listaAreaVeiculo
                    select new
                    {
                        areaVeiculo.Codigo,
                        CentroCarregamento = areaVeiculo.CentroCarregamento.Descricao,
                        areaVeiculo.Descricao,
                        areaVeiculo.DescricaoAtivo,
                        Tipo = areaVeiculo.Tipo.Obterdescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaAreaVeiculoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroCarregamento")
                return "CentroCarregamento.Descricao";

            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
