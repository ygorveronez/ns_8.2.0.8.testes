using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MonitoramentoLogistico
{
    [CustomAuthorize("Cargas/MonitoramentoLogisticoIntegracao")]
    public class MonitoramentoLogisticoIntegracaoController : BaseController
    {
		#region Construtores

		public MonitoramentoLogisticoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao = repositorioIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {integracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
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
        public async Task<IActionResult> PesquisaHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                var arquivosTransacaoRetornar = (
                    from arquivoTransacao in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosTransacaoRetornar);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaMonitoramentoLogisticoIntegracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: true);

                if (cargaMonitoramentoLogisticoIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMonitoramentoLogisticoIntegracao, null, "Reenviou integração.", unitOfWork);

                cargaMonitoramentoLogisticoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaMonitoramentoLogisticoIntegracao.DataIntegracao = DateTime.Now;
                cargaMonitoramentoLogisticoIntegracao.ProblemaIntegracao = "";
                cargaMonitoramentoLogisticoIntegracao.NumeroTentativas++;

                repositorioIntegracao.Atualizar(cargaMonitoramentoLogisticoIntegracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao()
            {
                CodigoFilial = Request.GetListParam<int>("Filial"),
                CodigoVeiculo = Request.GetIntParam("CodigoVeiculo"),
                CodigoMotorista = Request.GetIntParam("CodigoMotorista"),
                DataInicialAgendamento = Request.GetNullableDateTimeParam("DataInicialAgendamento"),
                DataLimiteAgendamento = Request.GetNullableDateTimeParam("DataLimiteAgendamento"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao"),
                Protocolo = Request.GetStringParam("Protocolo"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                Transportadores = Request.GetListParam<int>("Transportador")                
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Filial", "Filial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data do Agendamento", "InicioCarregamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao repositorio = new Repositorio.Embarcador.Cargas.MonitoramentoLogistico.MonitoramentoLogisticoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> listaCargaMonitoramentoLogisticoIntegracao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

                var retorno = from obj in listaCargaMonitoramentoLogisticoIntegracao
                              select new
                              {
                                  Codigo = obj.Codigo,
                                  CodigoCargaEmbarcador = obj.Carga.CodigoCargaEmbarcador,
                                  Protocolo = obj.Protocolo,
                                  Filial = obj.Carga.Filial?.Descricao ?? "",
                                  Transportador = obj.Carga.Empresa?.NomeCNPJ ?? "",
                                  Motorista = obj.Carga.Motoristas.FirstOrDefault()?.Nome ?? "",
                                  Veiculo = obj.Carga.Veiculo?.Placa ?? "",
                                  DataIntegracao = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                                  InicioCarregamento = obj.Carga.DataAgendamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                                  NumeroTentativas = obj.NumeroTentativas,
                                  SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                  ProblemaIntegracao = obj.ProblemaIntegracao
                              };

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Filial")
                return "Carga.Filial.Descricao";

            if (propriedadeOrdenar == "InicioCarregamento")
                return "Carga.JanelaCarregamento.InicioCarregamento";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
