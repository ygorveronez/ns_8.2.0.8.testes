using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers.Cargas.ControleEntrega;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.TorreControle
{

    [CustomAuthorize("TorreControle/FinalizacaoColetaEntregaEmLote")]
    public class FinalizacaoColetaEntregaEmLoteController : BaseControleEntregaController
    {
        #region Construtores

        public FinalizacaoColetaEntregaEmLoteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
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

        public async Task<IActionResult> PesquisaProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisaProcessamento());
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

        public async Task<IActionResult> FinalizarColetaEntregaEmMassa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();


                Servicos.Embarcador.Carga.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote servicoProcessamentoFinalizacaoColetaEntregaEmLote = new Servicos.Embarcador.Carga.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote();
                Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote repositorioProcessamentoFinalizacao = new Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote> processamentosFinalizacaoColetaEntregaEmLote = repositorioProcessamentoFinalizacao.BuscarTodos();

                List<int> listaCargaEntrega = ObterCodigosCargasSelecionadas(unitOfWork);

                if (listaCargaEntrega.Count == 0)
                    return new JsonpResult(false, Localization.Resources.TorreControle.FinalizacaoEmLote.NenhumaCargaEncontrada);


                foreach (int codigo in listaCargaEntrega)
                {
                    if (processamentosFinalizacaoColetaEntregaEmLote.Exists(x => x.CodigoCarga == codigo))
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote novoProcessamentoFinalizacaoColetaEntregaEmLote = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote();

                    novoProcessamentoFinalizacaoColetaEntregaEmLote.CodigoCarga = codigo;
                    novoProcessamentoFinalizacaoColetaEntregaEmLote.DataProcessamento = DateTime.Now;
                    novoProcessamentoFinalizacaoColetaEntregaEmLote.Situacao = SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.PendenteFinalizacao;
                    novoProcessamentoFinalizacaoColetaEntregaEmLote.Descricao = "Enviada para finalização";

                    repositorioProcessamentoFinalizacao.Inserir(novoProcessamentoFinalizacaoColetaEntregaEmLote);

                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> ObterDadosFiltroCarrosselFinalizacaoColetaEmLote(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterDadosFiltroCarrossel();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Carga, "CodigoCargaEmbarcador", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Remetente, "Remetente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.SituacaoCarga, "DescricaoSituacaoCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Origens, "Origem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Destinatario, "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Destino, "Destino", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.TiposOperacao, "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.DataInicioViagem, "DataInicioViagem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.DataEmissao, "DataEmissao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.DataCriacaoCarga, "DataCriacaoCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.DataPrevisaoDescarga, "DataPrevisaoDescarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Transportador, "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Placa, "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.SituacaoMonitoramento, "StatusMonitoramento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Etapa, "Etapa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Rastreador, "Rastreador", 10, Models.Grid.Align.left, false);

                Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "FinalizacaoColetaEntregaEmLote/Pesquisa", "grid-finalizacao-coleta-entrega-lote");
                grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal = configuracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote repositorioProcessamentoFinalizacao = new Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repCarga.ContarConsultarCargaEntregaNaoFinalizada(filtrosPesquisa, this.Usuario);
                IList<Dominio.Entidades.Embarcador.Cargas.Carga> listaRegistros = totalRegistros > 0 ? repCarga.ConsultarCargasEntregaNaoFinalizadaAsync(filtrosPesquisa, parametrosConsulta, this.Usuario).Result : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = repositorioMonitoramento.BuscarPorCargas(listaRegistros.Select(c => c.Codigo).ToList());

                var lista = (from p in listaRegistros
                             let monitoramento = monitoramentos?.FirstOrDefault(m => m?.Carga?.Codigo == p?.Codigo)
                             where p != null
                             select new
                             {
                                 p.Codigo,
                                 CodigoCargaEmbarcador = p.CodigoCargaEmbarcador,
                                 Remetente = p.DadosSumarizados?.Remetentes ?? string.Empty,
                                 DescricaoSituacaoCarga = p.SituacaoCarga.ObterDescricao() ?? string.Empty,
                                 Origem = p.DadosSumarizados?.Origens ?? string.Empty,
                                 Destinatario = p.DadosSumarizados?.Destinatarios ?? string.Empty,
                                 Destino = p.DadosSumarizados?.Destinos ?? string.Empty,
                                 TipoOperacao = p.TipoOperacao?.Descricao ?? string.Empty,
                                 DataInicioViagem = p.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 DataEmissao = p.DataInicioEmissaoDocumentos?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 DataCriacaoCarga = p.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm"),
                                 DataPrevisaoDescarga = p?.Entregas?.FirstOrDefault()?.DataPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 Transportador = p.Empresa?.NomeCNPJ ?? string.Empty,
                                 Placa = p.DadosSumarizados?.Veiculos ?? string.Empty,
                                 StatusMonitoramento = monitoramento?.Status.ObterDescricao() ?? string.Empty,
                                 Etapa = monitoramento?.StatusViagem?.Descricao ?? string.Empty,
                                 Rastreador = monitoramento?.UltimaPosicao?.DataVeiculo == null ? 1 : (monitoramento?.UltimaPosicao?.DataVeiculo != DateTime.MinValue && (DateTime.Now - monitoramento?.UltimaPosicao?.DataVeiculo)?.TotalMinutes <= filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal ? 3 : 4)
                             })
                             .ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return grid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Carga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.DataProcessamento, "DataProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Descricao, "Descricao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.Situacao, "Situacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.TorreControle.FinalizacaoEmLote.NumeroTentativas, "NumeroTentativas", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote repProcessamentoFinalizacao = new Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repProcessamentoFinalizacao.ContarConsultaAsync(filtrosPesquisa, parametrosConsulta).Result;

                IList<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote> listaRegistros = totalRegistros > 0 ? repProcessamentoFinalizacao.ConsultarAsync(filtrosPesquisa, parametrosConsulta).Result : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>();


                List<Dominio.ObjetosDeValor.Embarcador.Carga.CodigoEmbarcador> carga = repCarga.BuscarCodigoEmbarcadorPorCodigosCarga(listaRegistros.Select(c => c.CodigoCarga).ToList());

                var lista = (from p in listaRegistros
                             orderby p.Situacao
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 DataProcessamento = p.DataProcessamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                                 Situacao = p.Situacao.ObterDescricao(),
                                 CodigoCargaEmbarcador = (
                                 carga.Where(c => c.CodigoCarga == p.CodigoCarga).Select(o => o.CodigoCargaEmbarcador).FirstOrDefault()),
                                 NumeroTentativas = p.Tentativas
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return grid;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        private List<int> ObterCodigosCargasSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

            List<int> listaCodigosCarga = new List<int>();

            foreach (var item in listaItensSelecionados)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo((int)item.Codigo);
                listaCodigosCarga.Add(carga.Codigo);
            }

            return listaCodigosCarga.ToList();
        }

        private void IniciarViagemDaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int codigoCarga = carga.Codigo;
            try
            {
                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(codigoCarga, DateTime.Now, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado, unitOfWork))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Início de viagem informado manualmente", unitOfWork);

                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listMonitoramento = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(carga?.Veiculo.Placa);

                    if (listMonitoramento.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Monitoramento encerrado na carga {carga.CodigoCargaEmbarcador}, visto que foi iniciada viagem na carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw new Exception(Localization.Resources.TorreControle.FinalizacaoEmLote.FalhaInformarInicioViagem);
            }
        }

        private IActionResult ObterDadosFiltroCarrossel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote repProcessamentoFinalizacao = new Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa = ObterFiltrosPesquisa();

                IList<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote> retornoConsulta = repProcessamentoFinalizacao.ConsultarFiltroCarrossel(filtrosPesquisa, parametrosConsulta);

                var retorno = (from obj in retornoConsulta
                               select new
                               {
                                   obj.CodigoCarga,
                                   obj.Situacao,
                                   obj.Descricao,
                                   obj.DataProcessamento,
                                   obj.Tentativas
                               }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                throw new Exception(Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento()
            {
                CodigosCarga = Request.GetListParam<int>("NumeroCarga"),
                Situacao = Request.GetNullableEnumParam<SituacaoProcessamentoFinalizacaoColetaEntregaEmLote>("SituacaoProcessamento"),
                DataInicialProcessamento = Request.GetNullableDateTimeParam("DataInicioProcessamento"),
                DataFinalProcessamento = Request.GetNullableDateTimeParam("DataFinalProcessamento")
            };
            return filtrosPesquisa;
        }

    }

    #endregion
}