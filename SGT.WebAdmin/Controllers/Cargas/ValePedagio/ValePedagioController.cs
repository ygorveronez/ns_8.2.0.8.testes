using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ValePedagio
{
    [CustomAuthorize("Cargas/ValePedagio")]
    public class ValePedagioController : BaseController
    {
        #region Construtores

        public ValePedagioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio filtroPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                int total = repCargaIntegracaoValePedagio.ContarConsulta(filtroPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracaoValePedagios = new List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();
                Models.Grid.Grid grid = ObterGridPesquisa();
                if (total > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                    cargaIntegracaoValePedagios = repCargaIntegracaoValePedagio.Consulta(filtroPesquisa, parametrosConsulta);
                }
                PreencherGrid(grid, total, cargaIntegracaoValePedagios);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar vales pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaIntegracaoValePedagio.BuscarPorCodigo(codigo);
                if (cargaIntegracaoValePedagio != null)
                {
                    if (cargaIntegracaoValePedagio.PedagioIntegradoEmbarcador)
                        return new JsonpResult(true, false, "Pedágio integrado do embarcador não pode ser cancelado.");

                    if ((cargaIntegracaoValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada || cargaIntegracaoValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada) && cargaIntegracaoValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    {
                        // Apenas altera para "Em cancelamento" para ser processado pela thread IntegracaoCarga::GerarIntegracoesCancelamentoValePedagio
                        cargaIntegracaoValePedagio.Initialize();
                        cargaIntegracaoValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.EmCancelamento;
                        cargaIntegracaoValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        repCargaIntegracaoValePedagio.Atualizar(cargaIntegracaoValePedagio);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaIntegracaoValePedagio, cargaIntegracaoValePedagio.GetChanges(), "Solicitado o cancelamento", unitOfWork);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        return new JsonpResult(false, "Vale pedágio não está comprado e integrado para solicitar o cancelanento.");
                    }
                }
                else
                {
                    return new JsonpResult(false, "Vale pedágio não encontrado.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao agendar o cancelamento.");
            }
        }

        public async Task<IActionResult> RecompraSelecionadosValePedagio()
        {
            string stringConexao = _conexao.StringConexao;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);

                List<int> codigosValePedagio = new List<int>();

                dynamic registrosGridSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GridValePedagio"));

                if (registrosGridSelecionados != null && registrosGridSelecionados.Count > 0)
                {
                    foreach (var registro in registrosGridSelecionados)
                        if (registro.Codigo != null)
                            codigosValePedagio.Add((int)registro.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaCargaValePedagio = repCargaValePedagio.BuscarPorCodigos(codigosValePedagio);

                if (listaCargaValePedagio?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar os registros.");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio in listaCargaValePedagio)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, TipoServicoMultisoftware);

                    if (cargaValePedagio.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                        throw new ControllerException("Não é possível fazer a Recompra de Vale Pedágio em cargas que estão canceladas.");

                    // Atualiza situação da carga
                    if (cargaValePedagio.Carga.PossuiPendencia || cargaValePedagio.Carga.AgImportacaoCTe)
                    {
                        cargaValePedagio.Carga.PossuiPendencia = false;
                        cargaValePedagio.Carga.ProblemaIntegracaoValePedagio = false;
                        cargaValePedagio.Carga.IntegrandoValePedagio = true;
                        cargaValePedagio.Carga.MotivoPendencia = "";

                        repCarga.Atualizar(cargaValePedagio.Carga);

                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                        else
                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaValePedagio, null, "Reenviou integração rejeitada.", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaValePedagio.Carga, null, "Reenviou integração rejeitada.", unitOfWork);
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        bool gerarPedagioValorZeradoForaDoMesVigente = integracaoSemParar != null && integracaoSemParar.ComprarSomenteNoMesVigente && !cargaValePedagio.Carga.DataCriacaoCarga.IsDateSameMonth(DateTime.Now) &&
                             !(cargaValePedagio.Carga.DataCriacaoCarga.IsLastDayOfMonth() && cargaValePedagio.Carga.DataCriacaoCarga.Hour >= 20 && DateTime.Now.IsFirstDayOfMonth());

                        if (gerarPedagioValorZeradoForaDoMesVigente)
                            return new JsonpResult(false, true, "Configurado para não comprar em mês diferente da criação da carga. Caso deseje reenviar a solicitação fora do mês vigente, é necessário retirar a configuração existente.");
                    }
                    svcHubCarga.InformarCargaAtualizada(cargaValePedagio.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reenviar a integração do vale pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoValePedagio", false);
            grid.AdicionarCabecalho("SituacaoIntegracao", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data da carga", "DataCriacaoCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Placa", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo VP", "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Número VP", "NumeroValePedagio", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação do VP", "DescricaoSituacaoValePedagio", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor VP", "ValorValePedagio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data da integração", "DataIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação integração", "DescricaoSituacaoIntegracao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Retorno integração", "ProblemaIntegracao", 10, Models.Grid.Align.left, true);
            return grid;
        }

        private void PreencherGrid(Models.Grid.Grid grid, int total, List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracaoValePedagios)
        {
            var rows = (
                from row in cargaIntegracaoValePedagios
                select new
                {
                    row.Codigo,
                    Descricao = row.NumeroValePedagio,
                    row.Carga.CodigoCargaEmbarcador,
                    DataCriacaoCarga = row.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm"),
                    Placa = row.Carga.Veiculo?.Placa ?? string.Empty,
                    TipoIntegracao = row.TipoIntegracao.Descricao,
                    NumeroValePedagio = row.NumeroValePedagio ?? string.Empty,
                    row.SituacaoValePedagio,
                    row.DescricaoSituacaoValePedagio,
                    ValorValePedagio = row.ValorValePedagio.ToString("n2"),
                    DataIntegracao = row.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                    row.SituacaoIntegracao,
                    row.DescricaoSituacaoIntegracao,
                    row.ProblemaIntegracao
                }).ToList();
            grid.AdicionaRows(rows);
            grid.setarQuantidadeTotal(total);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                DataCargaInicial = Request.GetNullableDateTimeParam("DataCargaInicial"),
                DataCargaFinal = Request.GetNullableDateTimeParam("DataCargaFinal"),
                CodigoTipoIntegracao = Request.GetIntParam("TipoIntegracao"),
                NumeroValePedagio = Request.GetStringParam("NumeroValePedagio"),
                DataIntegracaoInicial = Request.GetNullableDateTimeParam("DataIntegracaoInicial"),
                DataIntegracaoFinal = Request.GetNullableDateTimeParam("DataIntegracaoFinal"),
                SituacaoValePedagio = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio>("SituacaoValePedagio"),
                SituacaoIntegracao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("SituacaoIntegracao"),
                NumeroParcialCarga = Request.GetStringParam("NumeroParcialCarga"),
                Filiais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)
            };
            return filtrosPesquisa;
        }

        #endregion
    }
}
