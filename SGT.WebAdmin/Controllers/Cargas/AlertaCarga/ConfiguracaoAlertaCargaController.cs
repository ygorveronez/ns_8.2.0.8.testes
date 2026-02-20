using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.AlertaCarga
{
    public class ConfiguracaoAlertaCargaController : BaseController
    {
		#region Construtores

		public ConfiguracaoAlertaCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga configAlertaCarga = new Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga();
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaTempoConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);

                preencherConfigAlertaCarga(configAlertaCarga);

                unitOfWork.Start();

                if (configAlertaCarga.TipoCargaAlerta == TipoAlertaCarga.AtrasoColetaDescarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configTemposAcomapanhamentosEntrega = repAcompanhamentoEntregaTempoConfiguracao.BuscarConfiguracao();
                    if (configTemposAcomapanhamentosEntrega == null)
                        configTemposAcomapanhamentosEntrega = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();

                    preencherConfigTemposAcompanhamentosEntrega(configTemposAcomapanhamentosEntrega);

                    if (configTemposAcomapanhamentosEntrega.Codigo > 0)
                        repAcompanhamentoEntregaTempoConfiguracao.Atualizar(configTemposAcomapanhamentosEntrega);
                    else
                        repAcompanhamentoEntregaTempoConfiguracao.Inserir(configTemposAcomapanhamentosEntrega);
                }

                new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork).Inserir(configAlertaCarga, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repConfigAlertaCarga = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaTempoConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga configAlertaCarga = repConfigAlertaCarga.BuscarPorCodigo(codigo, auditavel: true);

                if (configAlertaCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                preencherConfigAlertaCarga(configAlertaCarga);

                unitOfWork.Start();

                if (configAlertaCarga.TipoCargaAlerta == TipoAlertaCarga.AtrasoColetaDescarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configTemposAcomapanhamentosEntrega = repAcompanhamentoEntregaTempoConfiguracao.BuscarConfiguracao();
                    if (configTemposAcomapanhamentosEntrega == null)
                        configTemposAcomapanhamentosEntrega = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();

                    preencherConfigTemposAcompanhamentosEntrega(configTemposAcomapanhamentosEntrega);

                    if (configTemposAcomapanhamentosEntrega.Codigo > 0)
                        repAcompanhamentoEntregaTempoConfiguracao.Atualizar(configTemposAcomapanhamentosEntrega);
                    else
                        repAcompanhamentoEntregaTempoConfiguracao.Inserir(configTemposAcomapanhamentosEntrega);
                }

                new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork).Atualizar(configAlertaCarga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, configAlertaCarga, "Atualizado", unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repConfigAlertaCarga = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga configAlertaCarga = repConfigAlertaCarga.BuscarPorCodigo(codigo, auditavel: true);

                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configTemposAcomapanhamentosEntrega = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();
                if (configAlertaCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (configAlertaCarga.TipoCargaAlerta == TipoAlertaCarga.AtrasoColetaDescarga)
                {
                    Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaTempoConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                    configTemposAcomapanhamentosEntrega = repAcompanhamentoEntregaTempoConfiguracao.BuscarConfiguracao();
                }

                return new JsonpResult(new
                {
                    Alerta = new
                    {
                        configAlertaCarga.Codigo,
                        configAlertaCarga.Descricao,
                        Status = configAlertaCarga.Ativo,
                        configAlertaCarga.TipoCargaAlerta,
                        configAlertaCarga.Cor,
                        configAlertaCarga.NaoGerarParaPreCarga,
                        configAlertaCarga.GerarAlertaAcompanhamentoCarga,
                        VisualizacaoAlerta = TipoAlertaCargaHelper.isAlertaAcompanhamentoCarga(configAlertaCarga.TipoCargaAlerta),
                        DataBase = configAlertaCarga.DataBaseAlerta,
                        ConsiderarDataDeEntradaNoDestinoComoDataBase = configAlertaCarga.ConsiderarDataDeEntradaNoDestinoComoDataBase,
                    },
                    Gatilho = new
                    {
                        configAlertaCarga.Tempo,
                        configAlertaCarga.TempoEvento,

                        TempoAcompanhamentoEntregaAdiantado = configTemposAcomapanhamentosEntrega?.DestinoEmTempo,
                        TempoAcompanhamentoEntregaNoHorario = configTemposAcomapanhamentosEntrega?.DestinoAtraso1,
                        TempoAcompanhamentoEntregaPoucoAtrasado = configTemposAcomapanhamentosEntrega?.DestinoAtraso2,
                        TempoAcompanhamentoEntregaAtrasado = configTemposAcomapanhamentosEntrega?.DestinoAtraso3,

                        AlertarAdiantado = configTemposAcomapanhamentosEntrega?.GerarAlertaDestinoEmTempo ?? false,
                        AlertarNoHorario = configTemposAcomapanhamentosEntrega?.GerarAlertaDestinoAtraso1 ?? false,
                        AlertarPoucoAtrasado = configTemposAcomapanhamentosEntrega?.GerarAlertaDestinoAtraso2 ?? false,
                        AlertarAtrasado = configTemposAcomapanhamentosEntrega?.GerarAlertaDestinoAtraso3 ?? false,

                        TempoAcompanhamentoColetaAdiantado = configTemposAcomapanhamentosEntrega?.SaidaEmTempo,
                        TempoAcompanhamentoColetaNoHorario = configTemposAcomapanhamentosEntrega?.SaidaAtraso1,
                        TempoAcompanhamentoColetaPoucoAtrasado = configTemposAcomapanhamentosEntrega?.SaidaAtraso2,
                        TempoAcompanhamentoColetaAtrasado = configTemposAcomapanhamentosEntrega?.SaidaAtraso3,

                        AlertarAdiantadoColeta = configTemposAcomapanhamentosEntrega?.GerarAlertaSaidaEmTempo ?? false,
                        AlertarNoHorarioColeta = configTemposAcomapanhamentosEntrega?.GerarAlertaSaidaAtraso1 ?? false,
                        AlertarPoucoAtrasadoColeta = configTemposAcomapanhamentosEntrega?.GerarAlertaSaidaAtraso2 ?? false,
                        AlertarAtrasadoColeta = configTemposAcomapanhamentosEntrega?.GerarAlertaSaidaAtraso3 ?? false,
                    },
                    Tratativa = new
                    {
                        configAlertaCarga.EnvioEmailCliente,
                        configAlertaCarga.EnvioEmailTransportador,
                        TempoLimiteTratativaAutomatica = configAlertaCarga.TempoLimiteTratativaAutomaticaTime
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repConfigAlertaCarga = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga configAlertaCarga = repConfigAlertaCarga.BuscarPorCodigo(codigo, auditavel: true);

                if (configAlertaCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();
                repConfigAlertaCarga.Deletar(configAlertaCarga, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRemover);
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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
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

        #endregion

        #region Metodos Privados

        private void preencherConfigAlertaCarga(Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga configuracaoAlertaCarga)
        {
            configuracaoAlertaCarga.Ativo = Request.GetBoolParam("Status");
            configuracaoAlertaCarga.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException("Descrição do evento é obrigatória.");
            configuracaoAlertaCarga.TipoCargaAlerta = Request.GetNullableEnumParam<TipoAlertaCarga>("TipoCargaAlerta") ?? throw new ControllerException("Tipo do alerta é obrigatório.");
            configuracaoAlertaCarga.Cor = Request.GetStringParam("Cor");
            configuracaoAlertaCarga.NaoGerarParaPreCarga = Request.GetBoolParam("NaoGerarParaPreCarga");
            configuracaoAlertaCarga.GerarAlertaAcompanhamentoCarga = Request.GetBoolParam("GerarAlertaAcompanhamentoCarga");
            configuracaoAlertaCarga.ConsiderarDataDeEntradaNoDestinoComoDataBase = Request.GetBoolParam("ConsiderarDataDeEntradaNoDestinoComoDataBase");

            configuracaoAlertaCarga.DataBaseAlerta = Request.GetNullableEnumParam<DataBaseAlerta>("DataBase");

            dynamic dadosGatilho = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Gatilho"));
            configuracaoAlertaCarga.Tempo = ((string)dadosGatilho.Tempo).ToInt();
            configuracaoAlertaCarga.TempoEvento = ((string)dadosGatilho.TempoEvento).ToInt();

            dynamic tratativa = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Tratativa"));
            configuracaoAlertaCarga.EnvioEmailTransportador = ((string)tratativa.EnvioEmailTransportador).ToBool();
            configuracaoAlertaCarga.EnvioEmailCliente = ((string)tratativa.EnvioEmailCliente).ToBool();
            if (!string.IsNullOrWhiteSpace(tratativa.TempoLimiteTratativaAutomatica.ToString()))
                configuracaoAlertaCarga.TempoLimiteTratativaAutomaticaTime = (TimeSpan)tratativa.TempoLimiteTratativaAutomatica;



        }


        private void preencherConfigTemposAcompanhamentosEntrega(Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoAcompanhamentoEntrega)
        {

            dynamic dadosGatilho = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Gatilho"));

            configuracaoAcompanhamentoEntrega.DestinoEmTempo = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoEntregaAdiantado) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoEntregaAdiantado : new TimeSpan();
            configuracaoAcompanhamentoEntrega.DestinoAtraso1 = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoEntregaNoHorario) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoEntregaNoHorario : new TimeSpan();
            configuracaoAcompanhamentoEntrega.DestinoAtraso2 = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoEntregaPoucoAtrasado) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoEntregaPoucoAtrasado : new TimeSpan();
            configuracaoAcompanhamentoEntrega.DestinoAtraso3 = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoEntregaAtrasado) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoEntregaAtrasado : new TimeSpan();

            configuracaoAcompanhamentoEntrega.GerarAlertaDestinoEmTempo = ((string)dadosGatilho.AlertarAdiantado).ToBool();
            configuracaoAcompanhamentoEntrega.GerarAlertaDestinoAtraso1 = ((string)dadosGatilho.AlertarNoHorario).ToBool();
            configuracaoAcompanhamentoEntrega.GerarAlertaDestinoAtraso2 = ((string)dadosGatilho.AlertarPoucoAtrasado).ToBool();
            configuracaoAcompanhamentoEntrega.GerarAlertaDestinoAtraso3 = ((string)dadosGatilho.AlertarAtrasado).ToBool();

            configuracaoAcompanhamentoEntrega.SaidaEmTempo = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoColetaAdiantado) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoColetaAdiantado : new TimeSpan();
            configuracaoAcompanhamentoEntrega.SaidaAtraso1 = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoColetaNoHorario) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoColetaNoHorario : new TimeSpan();
            configuracaoAcompanhamentoEntrega.SaidaAtraso2 = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoColetaPoucoAtrasado) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoColetaPoucoAtrasado : new TimeSpan();
            configuracaoAcompanhamentoEntrega.SaidaAtraso3 = !string.IsNullOrEmpty((string)dadosGatilho.TempoAcompanhamentoColetaAtrasado) ? (TimeSpan)dadosGatilho.TempoAcompanhamentoColetaAtrasado : new TimeSpan();

            configuracaoAcompanhamentoEntrega.GerarAlertaSaidaEmTempo = ((string)dadosGatilho.AlertarAdiantadoColeta).ToBool();
            configuracaoAcompanhamentoEntrega.GerarAlertaSaidaAtraso1 = ((string)dadosGatilho.AlertarNoHorarioColeta).ToBool();
            configuracaoAcompanhamentoEntrega.GerarAlertaSaidaAtraso2 = ((string)dadosGatilho.AlertarPoucoAtrasadoColeta).ToBool();
            configuracaoAcompanhamentoEntrega.GerarAlertaSaidaAtraso3 = ((string)dadosGatilho.AlertarAtrasadoColeta).ToBool();
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAlertaCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAlertaCarga()
            {
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                TipoAlerta = Request.GetNullableEnumParam<TipoAlertaCarga>("TipoCargaAlerta")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");
                SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "Tipo", 20, Models.Grid.Align.left, false);

                if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAlertaCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repositorio = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> listaAlertas = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga>();

                var listaMonitoramentoEventoRetornar = (
                    from alerta in listaAlertas
                    select new
                    {
                        alerta.Codigo,
                        alerta.Descricao,
                        alerta.DescricaoAtivo,
                        Tipo = alerta.TipoCargaAlerta.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaMonitoramentoEventoRetornar);
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }


        #endregion


    }
}
