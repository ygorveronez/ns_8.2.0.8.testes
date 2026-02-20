using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ExcecaoCapacidadeDescarregamento")]
    public class ExcecaoCapacidadeDescarregamentoController : BaseController
    {
		#region Construtores

		public ExcecaoCapacidadeDescarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao = new Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento();

                PreencherExcecaoCapacidadeDescarregamento(excecao, unitOfWork);

                repositorio.Inserir(excecao, Auditado);

                AdicionarOuAtualizarQuantidadePorTipoDeCargaDescarregamento(excecao, unitOfWork);
                AtualizarPeriodosDescarregamento(excecao, null, unitOfWork);
                AtualizarPrevisoesDescarregamento(excecao, null, unitOfWork);
                ValidarCapacidadeDescarregamento(excecao, unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
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
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (excecao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                PreencherExcecaoCapacidadeDescarregamento(excecao, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorio.Atualizar(excecao, Auditado);

                AdicionarOuAtualizarQuantidadePorTipoDeCargaDescarregamento(excecao, unitOfWork);
                AtualizarPeriodosDescarregamento(excecao, historico, unitOfWork);
                AtualizarPrevisoesDescarregamento(excecao, historico, unitOfWork);
                ValidarCapacidadeDescarregamento(excecao, unitOfWork);

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
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (excecao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadeTipoCargaTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda repositorioPeriodoCanalVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioPeriodoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorioPeriodoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repositorioPeriodoTipoDeCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repositorioPeriodoRemetente = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> quantidadesPorTipoCargaTipoCarga = repositorioQuantidadeTipoCargaTipoCarga.BuscarPorExcecao(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> quantidadesPorTipoCarga = quantidadesPorTipoCargaTipoCarga.Select(obj => obj.QuantidadePorTipoDeCargaDescarregamento).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> periodoCanaisVenda = repositorioPeriodoCanalVenda.BuscarPorExcecao(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> periodoGruposProdutos = repositorioPeriodoGrupoProduto.BuscarPorExcecao(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> periodoGruposPessoas = repositorioPeriodoGrupoPessoa.BuscarPorExcecao(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> periodoTiposDeCarga = repositorioPeriodoTipoDeCarga.BuscarPorExcecao(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> periodoRemetentes = repositorioPeriodoRemetente.BuscarPorExcecao(codigo);


                return new JsonpResult(new
                {
                    excecao.Codigo,
                    CentroDescarregamento = new
                    {
                        excecao.CentroDescarregamento.Codigo,
                        excecao.CentroDescarregamento.Descricao,
                        excecao.CentroDescarregamento.TipoCapacidadeDescarregamentoPorPeso,
                    },
                    CapacidadeDescarregamento = excecao.CapacidadeDescarregamento > 0 ? excecao.CapacidadeDescarregamento.ToString("n0") : "",
                    DataInicial = excecao.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = excecao.DataFinal.ToString("dd/MM/yyyy"),
                    excecao.Descricao,
                    PeriodosDescarregamento = (
                        from periodo in excecao.PeriodosDescarregamento
                        select new
                        {
                            periodo.Codigo,
                            DiaSemana = 0,
                            HoraInicio = periodo.HoraInicio.ToString(@"hh\:mm"),
                            HoraTermino = periodo.HoraTermino.ToString(@"hh\:mm"),
                            periodo.ToleranciaExcessoTempo,
                            periodo.CapacidadeDescarregamentoSimultaneo,
                            periodo.CapacidadeDescarregamentoSimultaneoAdicional,
                            CapacidadeDescarregamento = (periodo.CapacidadeDescarregamento > 0) ? periodo.CapacidadeDescarregamento.ToString("n0") : "",
                            SkuDe = periodo.SkuDe?.ToString() ?? string.Empty,
                            SkuAte = periodo.SkuAte?.ToString() ?? string.Empty,
                            CanaisVenda = (from periodoCanalVenda in periodoCanaisVenda where periodoCanalVenda.PeriodoDescarregamento.Codigo == periodo.Codigo select new { periodoCanalVenda.CanalVenda.Codigo, periodoCanalVenda.CanalVenda.Descricao }).ToList(),
                            GruposProdutos = (from periodoGrupoProduto in periodoGruposProdutos where periodoGrupoProduto.PeriodoDescarregamento.Codigo == periodo.Codigo select new { periodoGrupoProduto.GrupoProduto.Codigo, periodoGrupoProduto.GrupoProduto.Descricao }).ToList(),
                            Remetentes = (from periodoRemetente in periodoRemetentes where periodoRemetente.PeriodoDescarregamento.Codigo == periodo.Codigo select new { periodoRemetente.Remetente.Codigo, periodoRemetente.Remetente.Descricao }).ToList(),
                            TiposCarga = (from periodoTipoCarga in periodoTiposDeCarga where periodoTipoCarga.PeriodoDescarregamento.Codigo == periodo.Codigo select new { periodoTipoCarga.TipoDeCarga.Codigo, periodoTipoCarga.TipoDeCarga.Descricao }).ToList(),
                            GruposPessoas = (from periodoGrupoPessoa in periodoGruposPessoas where periodoGrupoPessoa.PeriodoDescarregamento.Codigo == periodo.Codigo select new { periodoGrupoPessoa.GrupoPessoas.Codigo, periodoGrupoPessoa.GrupoPessoas.Descricao }).ToList()

                        }).ToList(),
                    PrevisoesDescarregamento = (
                        from previsao in excecao.PrevisoesDescarregamento
                        select new
                        {
                            previsao.Codigo,
                            Descricao = excecao.Descricao ?? string.Empty,
                            DiaSemana = 0,
                            previsao.QuantidadeCargas,
                            previsao.QuantidadeCargasExcedentes,
                            Rota = new { previsao.Rota.Codigo, previsao.Rota.Descricao },
                            ModelosVeiculos = (
                                from modelo in previsao.ModelosVeiculos
                                select new
                                {
                                    modelo.Codigo,
                                    modelo.Descricao
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    QuantidadesPorTipoDeCargaDescarregamento = (
                        from obj in quantidadesPorTipoCarga
                        select new
                        {
                            obj.Codigo,
                            obj.Volumes,
                            Tolerancia = obj?.Tolerancia ?? 0,
                            ToleranciaCancelamentoAgendaConfirmada = obj?.ToleranciaCancelamentoAgendaConfirmada ?? 0,
                            ToleranciaCancelamentoAgendaNaoConfirmada = obj?.ToleranciaCancelamentoAgendaNaoConfirmada ?? 0,
                            DescricaoTipoCarga = string.Join(", ", (from elemento in quantidadesPorTipoCargaTipoCarga where elemento.QuantidadePorTipoDeCargaDescarregamento.Codigo == obj.Codigo select elemento.TipoCarga.Descricao).ToList()),
                            TiposCarga = (
                                from elemento in quantidadesPorTipoCargaTipoCarga
                                where elemento.QuantidadePorTipoDeCargaDescarregamento.Codigo == obj.Codigo
                                select new
                                {
                                    elemento.TipoCarga.Codigo,
                                    elemento.TipoCarga.Descricao
                                }
                            ).ToList()
                        }
                    ).ToList()
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
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PrevisaoDescarregamento repPrevisaoDescarregamento = new Repositorio.Embarcador.Logistica.PrevisaoDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadePorTipoCargaTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento repositorioQuantidadeTipoCargaDescarregamento = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (excecao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> previsaoDescarregamentos = repPrevisaoDescarregamento.BuscarPorExcecao(excecao.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> tiposDeCarga = repositorioQuantidadePorTipoCargaTipoCarga.BuscarPorExcecao(excecao.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> quantidadesTipoCarga = repositorioQuantidadeTipoCargaDescarregamento.BuscarPorExcecao(excecao.Codigo);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento previsaoDescarregamento in previsaoDescarregamentos)
                {
                    previsaoDescarregamento.ModelosVeiculos.Clear();
                    repPrevisaoDescarregamento.Deletar(previsaoDescarregamento);
                }

                foreach (Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga tipoCarga in tiposDeCarga)
                    repositorioQuantidadePorTipoCargaTipoCarga.Deletar(tipoCarga);

                unitOfWork.Flush();

                foreach (Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento quantidadeTipoCarga in quantidadesTipoCarga)
                    repositorioQuantidadeTipoCargaDescarregamento.Deletar(quantidadeTipoCarga);

                unitOfWork.Flush();

                repositorio.Deletar(excecao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
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
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
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

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorioExcecaoCapacidadeDescarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento repositorioQuantidadePorTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioPeriodoDescarregamentoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadeTipoCargaTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unitOfWork);
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };
                int totalRegistrosImportados = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();

                        string retorno = "";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoCapacidadeDescarregamento = null;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCentroDescarregamento = (from obj in linha.Colunas where obj.NomeCampo == "CentroDescarregamento" select obj).FirstOrDefault();
                        string codigoIntegracaoCentroDescarregamento = "";
                        Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = null;

                        if (colCentroDescarregamento?.Valor != null)
                        {
                            codigoIntegracaoCentroDescarregamento = colCentroDescarregamento.Valor.Trim();

                            centroDescarregamento = repositorioCentroDescarregamento.BuscarPorDescricao(codigoIntegracaoCentroDescarregamento);

                            if (centroDescarregamento == null)
                                throw new ControllerException("Centro de Descarregamento não encontrado por essa Descrição.");
                        }
                        else
                            throw new ControllerException("Centro de Descarregamento é obrigatório.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataInicio = (from obj in linha.Colunas where obj.NomeCampo == "DataInicial" select obj).FirstOrDefault();
                        string dataInicio = "";
                        DateTime dataInicial = DateTime.MinValue;

                        if (colDataInicio?.Valor != null)
                        {
                            dataInicio = colDataInicio.Valor;
                            double.TryParse(dataInicio, out double dataFormatoExcel);
                            if (dataFormatoExcel > 0)
                                dataInicial = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                            else if (!string.IsNullOrWhiteSpace(dataInicio))
                            {
                                DateTime dataInicialTest;
                                DateTime.TryParse(dataInicio, out dataInicialTest);
                                if (dataInicialTest > DateTime.MinValue)
                                    dataInicial = dataInicialTest;
                            }
                        }
                        else
                            throw new ControllerException("Data Inicial é obrigatório.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataFinal = (from obj in linha.Colunas where obj.NomeCampo == "DataFinal" select obj).FirstOrDefault();
                        string dataFim = "";
                        DateTime dataFinal = DateTime.MinValue;

                        if (colDataFinal?.Valor != null)
                        {
                            dataFim = colDataFinal.Valor;
                            double.TryParse(dataFim, out double dataFormatoExcel);
                            if (dataFormatoExcel > 0)
                                dataFinal = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                            else if (!string.IsNullOrWhiteSpace(dataFim))
                            {
                                DateTime dataInicialTest;
                                DateTime.TryParse(dataFim, out dataInicialTest);
                                if (dataInicialTest > DateTime.MinValue)
                                    dataFinal = dataInicialTest;
                            }
                        }
                        else
                            throw new ControllerException("Data Final é obrigatório.");

                        excecaoCapacidadeDescarregamento = repositorioExcecaoCapacidadeDescarregamento.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo, dataInicial, dataFinal);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                        string descricaoCentroDescarregamento = "";
                        if (colDescricao.Valor != null)
                            descricaoCentroDescarregamento = colDescricao.Valor;
                        else
                            throw new ControllerException("Descrição é obrigatório.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescarregamentoSimulaneo = (from obj in linha.Colunas where obj.NomeCampo == "DescarregamentoSimultaneo" select obj).FirstOrDefault();
                        int descarregamentoSimultaneo = 0;
                        if (colDescarregamentoSimulaneo?.Valor != null)
                            int.TryParse(colDescarregamentoSimulaneo?.Valor ?? "0", out descarregamentoSimultaneo);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadeTipoCarga" select obj).FirstOrDefault();
                        int quantidadeTipoCarga = 0;
                        if (colQuantidadeTipoCarga?.Valor != null)
                            int.TryParse(colQuantidadeTipoCarga?.Valor ?? "0", out quantidadeTipoCarga);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHorarioInicio = (from obj in linha.Colunas where obj.NomeCampo == "HorarioInicio" select obj).FirstOrDefault();
                        string horaInicio = "";
                        TimeSpan horarioInicial = TimeSpan.MinValue;

                        if (colHorarioInicio?.Valor != null)
                        {
                            horaInicio = colHorarioInicio.Valor;
                            horarioInicial = ObterHoraImportacao(horaInicio);

                            if (horarioInicial == null)
                                throw new Exception("Horário de início inválido.");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHorarioTermino = (from obj in linha.Colunas where obj.NomeCampo == "HorarioTermino" select obj).FirstOrDefault();
                        string horaTermino = "";
                        TimeSpan horarioTermino = TimeSpan.MinValue;

                        if (colHorarioTermino?.Valor != null)
                        {
                            horaTermino = colHorarioTermino.Valor;
                            horarioTermino = ObterHoraImportacao(horaTermino);

                            if (horarioTermino == null)
                                throw new Exception("Horário de término inválido.");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoDeCarga = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTipoDeCarga" select obj).FirstOrDefault();
                        List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> listaTipoDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
                        string codigoIntegracaoTipoDeCarga = "";

                        if (colTipoDeCarga?.Valor != null)
                        {
                            codigoIntegracaoTipoDeCarga = colTipoDeCarga.Valor;
                            string[] listaTiposCarga = codigoIntegracaoTipoDeCarga.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < listaTiposCarga.Length; j++)
                            {
                                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(listaTiposCarga[j].Trim());

                                if (tipoDeCarga == null)
                                    throw new ControllerException("Tipo de Carga não encontrada por esse Código de Integração.");

                                listaTipoDeCarga.Add(tipoDeCarga);
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoProduto = (from obj in linha.Colunas where obj.NomeCampo == "GrupoProduto" select obj).FirstOrDefault();
                        List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> listaGrupoProduto = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
                        string codigoIntegracaoGrupoProduto = "";

                        if (colGrupoProduto?.Valor != null)
                        {
                            codigoIntegracaoGrupoProduto = colGrupoProduto.Valor;
                            string[] listaGruposProduto = codigoIntegracaoGrupoProduto.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < listaGruposProduto.Length; j++)
                            {
                                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repositorioGrupoProduto.BuscarPorCodigoEmbarcador(listaGruposProduto[j].Trim());

                                if (grupoProduto == null)
                                    throw new ControllerException("Grupo de Produto não encontrada por esse Código de Integração.");

                                listaGrupoProduto.Add(grupoProduto);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(retorno, i));
                        }
                        else
                        {
                            totalRegistrosImportados++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i, contar: false);
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            if (excecaoCapacidadeDescarregamento == null)
                                excecaoCapacidadeDescarregamento = new Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento();
                            else
                                excecaoCapacidadeDescarregamento.Initialize();

                            excecaoCapacidadeDescarregamento.CentroDescarregamento = centroDescarregamento;
                            excecaoCapacidadeDescarregamento.Descricao = descricaoCentroDescarregamento;
                            excecaoCapacidadeDescarregamento.DataInicial = dataInicial;
                            excecaoCapacidadeDescarregamento.DataFinal = dataFinal;
                            excecaoCapacidadeDescarregamento.Usuario = this.Usuario;

                            if (excecaoCapacidadeDescarregamento.Codigo > 0)
                                repositorioExcecaoCapacidadeDescarregamento.Atualizar(excecaoCapacidadeDescarregamento, Auditado, null, "Atualizado via importação de planilha.");
                            else
                                repositorioExcecaoCapacidadeDescarregamento.Inserir(excecaoCapacidadeDescarregamento, Auditado, null, "Adicionado via importação de planilha.");

                            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorExcecao(excecaoCapacidadeDescarregamento.Codigo);

                            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = periodosDescarregamento.Where(o => o.HoraInicio == horarioInicial && o.HoraTermino == horarioTermino).FirstOrDefault();
                            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> periodosDescarregamentoGrupoProduto = repositorioPeriodoDescarregamentoGrupoProduto.BuscarPorExcecao(excecaoCapacidadeDescarregamento.Codigo);
                            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> quantidadesPorTipoCargaTipoCarga = repositorioQuantidadeTipoCargaTipoCarga.BuscarPorExcecao(excecaoCapacidadeDescarregamento.Codigo);

                            if (periodoDescarregamento == null)
                            {
                                periodoDescarregamento = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento();
                                periodoDescarregamento.ExcecaoCapacidadeDescarregamento = excecaoCapacidadeDescarregamento;
                            }

                            periodoDescarregamento.HoraInicio = horarioInicial;
                            periodoDescarregamento.HoraTermino = horarioTermino;
                            periodoDescarregamento.CapacidadeDescarregamentoSimultaneo = descarregamentoSimultaneo;

                            if (periodoDescarregamento.Codigo > 0)
                                repositorioPeriodoDescarregamento.Atualizar(periodoDescarregamento);
                            else
                                repositorioPeriodoDescarregamento.Inserir(periodoDescarregamento);

                            foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto in listaGrupoProduto)
                            {
                                if (periodosDescarregamentoGrupoProduto.Any(o => o.GrupoProduto.Codigo == grupoProduto.Codigo))
                                    continue;

                                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto periodoDescarregamentoGrupoProduto = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto()
                                {
                                    GrupoProduto = grupoProduto,
                                    PeriodoDescarregamento = periodoDescarregamento
                                };

                                repositorioPeriodoDescarregamentoGrupoProduto.Inserir(periodoDescarregamentoGrupoProduto);
                            };

                            if (!quantidadesPorTipoCargaTipoCarga.Any(o => listaTipoDeCarga.Select(p => p.Codigo).ToList().Contains(o.TipoCarga.Codigo) && o.QuantidadePorTipoDeCargaDescarregamento.Volumes == quantidadeTipoCarga))
                            {
                                Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento quantidadePorTipoDeCargaDescarregamento = new Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento()
                                {
                                    Volumes = quantidadeTipoCarga,
                                    ExcecaoCapacidadeDescarregamento = excecaoCapacidadeDescarregamento,
                                    CentroDescarregamento = centroDescarregamento
                                };

                                repositorioQuantidadePorTipoCarga.Inserir(quantidadePorTipoDeCargaDescarregamento);

                                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in listaTipoDeCarga)
                                {
                                    Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga quantidadePorTipoDeCargaTipoDeCarga = new Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga()
                                    {
                                        TipoCarga = tipoDeCarga,
                                        QuantidadePorTipoDeCargaDescarregamento = quantidadePorTipoDeCargaDescarregamento,
                                    };

                                    repositorioQuantidadeTipoCargaTipoCarga.Inserir(quantidadePorTipoDeCargaTipoDeCarga);
                                }
                            }

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (BaseException excecao)
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(excecao.Message, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = totalRegistrosImportados;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarOuAtualizarPeriodosDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, dynamic periodosDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);

            if (excecao.PeriodosDescarregamento == null)
                excecao.PeriodosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();

            foreach (var periodoDescarregamento in periodosDescarregamento)
            {
                int? codigo = ((string)periodoDescarregamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo;

                if (codigo.HasValue)
                    periodo = repositorioPeriodoDescarregamento.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não foi possível encontrar o período de descarregamento.");
                else
                    periodo = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento();

                periodo.CentroDescarregamento = excecao.CentroDescarregamento;
                periodo.ExcecaoCapacidadeDescarregamento = excecao;
                periodo.CapacidadeDescarregamento = ((string)periodoDescarregamento.CapacidadeDescarregamento).ToInt();
                periodo.CapacidadeDescarregamentoSimultaneo = ((string)periodoDescarregamento.CapacidadeDescarregamentoSimultaneo).ToInt();
                periodo.CapacidadeDescarregamentoSimultaneoAdicional = ((string)periodoDescarregamento.CapacidadeDescarregamentoSimultaneoAdicional).ToInt();
                periodo.Dia = ((string)periodoDescarregamento.DiaSemana).ToEnum<DiaSemana>();
                periodo.HoraInicio = ((string)periodoDescarregamento.HoraInicio).ToTime();
                periodo.HoraTermino = ((string)periodoDescarregamento.HoraTermino).ToTime();
                periodo.ToleranciaExcessoTempo = ((string)periodoDescarregamento.ToleranciaExcessoTempo).ToInt();
                periodo.SkuDe = null;
                periodo.SkuAte = null;
                if (int.TryParse((string)periodoDescarregamento.SkuDe, out int skuDe))
                    periodo.SkuDe = skuDe;
                if (int.TryParse((string)periodoDescarregamento.SkuAte, out int skuAte))
                    periodo.SkuAte = skuAte;

                if (periodo.Codigo > 0)
                    repositorioPeriodoDescarregamento.Atualizar(periodo, historico != null ? Auditado : null, historico);
                else
                    repositorioPeriodoDescarregamento.Inserir(periodo, historico != null ? Auditado : null, historico);

                SalvarPeriodoDescarregamentoCanaisVenda(periodo, periodoDescarregamento.CanaisVenda, unitOfWork);
                SalvarPeriodoDescarregamentoGruposProdutos(periodo, periodoDescarregamento.GruposProdutos, unitOfWork);
                SalvarPeriodoDescarregamentoGrupoPessoas(periodo, periodoDescarregamento.GruposPessoas, unitOfWork);
                SalvarPeriodoDescarregamentoTiposCarga(periodo, periodoDescarregamento.TiposCarga, unitOfWork);
                SalvarPeriodoDescarregamentoRemetentes(periodo, periodoDescarregamento.Remetentes, unitOfWork);
            }
        }

        private void AdicionarOuAtualizarQuantidadePorTipoDeCargaDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento repositorioQuantidadePorTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadePorTipoCargaTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            dynamic quantidadesPorTipoCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("QuantidadesPorTipoDeCargaDescarregamento"));

            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> listaQuantidadesTipoDeCargaTipoDeCargaDeletar = repositorioQuantidadePorTipoCargaTipoCarga.BuscarPorExcecao(excecao.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> listaQuantidadeTipoDeCargaDeletar = (from o in listaQuantidadesTipoDeCargaTipoDeCargaDeletar select o.QuantidadePorTipoDeCargaDescarregamento).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga quantidadeTipoCargaTipoCargaDeletar in listaQuantidadesTipoDeCargaTipoDeCargaDeletar)
                repositorioQuantidadePorTipoCargaTipoCarga.Deletar(quantidadeTipoCargaTipoCargaDeletar);

            unitOfWork.Flush();

            foreach (Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento quantidadeTipoCargaDeletar in listaQuantidadeTipoDeCargaDeletar)
                repositorioQuantidadePorTipoCarga.Deletar(quantidadeTipoCargaDeletar);

            foreach (var quantidadePorTipoCarga in quantidadesPorTipoCarga)
            {
                int? codigo = ((string)quantidadePorTipoCarga.Codigo).ToNullableInt();

                Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento quantidadePorTipoDeCargaDescarregamento = new Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento();

                quantidadePorTipoDeCargaDescarregamento.CentroDescarregamento = excecao.CentroDescarregamento;
                quantidadePorTipoDeCargaDescarregamento.ExcecaoCapacidadeDescarregamento = excecao;
                quantidadePorTipoDeCargaDescarregamento.Tolerancia = ((string)quantidadePorTipoCarga.Tolerancia).ToInt();
                quantidadePorTipoDeCargaDescarregamento.Volumes = ((string)quantidadePorTipoCarga.Volumes).ToInt();
                quantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaConfirmada = ((string)quantidadePorTipoCarga.ToleranciaCancelamentoAgendaConfirmada).ToInt();
                quantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaNaoConfirmada = ((string)quantidadePorTipoCarga.ToleranciaCancelamentoAgendaNaoConfirmada).ToInt();

                repositorioQuantidadePorTipoCarga.Inserir(quantidadePorTipoDeCargaDescarregamento, Auditado);

                List<int> codigosTiposCarga = new List<int>();

                foreach (var tipoCarga in quantidadePorTipoCarga.TiposCarga)
                    codigosTiposCarga.Add(((string)tipoCarga.Codigo).ToInt());

                codigosTiposCarga = codigosTiposCarga.Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = codigosTiposCarga.Count > 0 ? repositorioTipoDeCarga.BuscarPorCodigos(codigosTiposCarga) : new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

                Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga quantidadePorTipoCargaTipoCarga = null;


                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in tiposDeCarga)
                {
                    quantidadePorTipoCargaTipoCarga = new Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga()
                    {
                        QuantidadePorTipoDeCargaDescarregamento = quantidadePorTipoDeCargaDescarregamento,
                        TipoCarga = tipoDeCarga
                    };

                    repositorioQuantidadePorTipoCargaTipoCarga.Inserir(quantidadePorTipoCargaTipoCarga);
                }
            }
        }

        private void AdicionarOuAtualizarPrevisoesDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, dynamic previsoesDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PrevisaoDescarregamento repositorioPrevisaoDescarregamento = new Repositorio.Embarcador.Logistica.PrevisaoDescarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            if (excecao.PrevisoesDescarregamento == null)
                excecao.PrevisoesDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();

            foreach (var previsaoDescarregamento in previsoesDescarregamento)
            {
                int? codigo = ((string)previsaoDescarregamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento previsao;

                if (codigo.HasValue)
                    previsao = repositorioPrevisaoDescarregamento.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não foi possível encontrar a previsão de descarregamento.");
                else
                    previsao = new Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento();

                previsao.CentroDescarregamento = excecao.CentroDescarregamento;
                previsao.ExcecaoCapacidadeDescarregamento = excecao;
                previsao.QuantidadeCargas = ((string)previsaoDescarregamento.QuantidadeCargas).ToInt();
                previsao.QuantidadeCargasExcedentes = ((string)previsaoDescarregamento.QuantidadeCargasExcedentes).ToInt();
                previsao.Dia = ((string)previsaoDescarregamento.DiaSemana).ToEnum<DiaSemana>();
                previsao.Descricao = (string)previsaoDescarregamento.Descricao;
                previsao.Rota = repositorioRotaFrete.BuscarPorCodigo(((string)previsaoDescarregamento.Rota.Codigo).ToInt()) ?? throw new ControllerException("Não foi possível encontrar a rota");
                previsao.ModelosVeiculos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                var modelosVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>
                    (previsaoDescarregamento.ModelosVeiculos.ToString());

                foreach (var modeloVeiculo in modelosVeiculos)
                    previsao.ModelosVeiculos.Add(repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloVeiculo.Codigo).ToInt()) ?? throw new ControllerException("Não foi possível encontrar o modelo veicular de carga."));

                if (previsao.Codigo > 0)
                    repositorioPrevisaoDescarregamento.Atualizar(previsao, historico != null ? Auditado : null, historico);
                else
                    repositorioPrevisaoDescarregamento.Inserir(previsao, historico != null ? Auditado : null, historico);
            }
        }

        private void AtualizarPeriodosDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic periodosDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PeriodosDescarregamento"));

            ExcluirPeriodosDescarregamentoRemovidos(excecao, periodosDescarregamento, historico, unitOfWork);
            AdicionarOuAtualizarPeriodosDescarregamento(excecao, periodosDescarregamento, historico, unitOfWork);
        }

        private void AtualizarPrevisoesDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic previsoesDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PrevisoesDescarregamento"));

            ExcluirPrevisoesDescarregamentoRemovidos(excecao, previsoesDescarregamento, historico, unitOfWork);
            AdicionarOuAtualizarPrevisoesDescarregamento(excecao, previsoesDescarregamento, historico, unitOfWork);
        }

        private void ExcluirPeriodosDescarregamentoRemovidos(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, dynamic periodosDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);

            if (excecao.PeriodosDescarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var periodoDescarregamento in periodosDescarregamento)
                {
                    int? codigo = ((string)periodoDescarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamentoDeletar = (from o in excecao.PeriodosDescarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento in periodosDescarregamentoDeletar)
                    repositorioPeriodoDescarregamento.Deletar(periodoDescarregamento, historico != null ? Auditado : null, historico);
            }
        }

        private void ExcluirPrevisoesDescarregamentoRemovidos(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, dynamic periodosDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PrevisaoDescarregamento repositorioPrevisaoDescarregamento = new Repositorio.Embarcador.Logistica.PrevisaoDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repositorioReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

            if (excecao.PrevisoesDescarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var previsaoDescarregamento in periodosDescarregamento)
                {
                    int? codigo = ((string)previsaoDescarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> previsoesDescarregamentoDeletar = (from o in excecao.PrevisoesDescarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento previsaoDescarregamento in previsoesDescarregamentoDeletar)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> reservasCargaGrupoPessoa = repositorioReservaCargaGrupoPessoa.BuscarPorPrevisao(previsaoDescarregamento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reservaCargaGrupoPessoa in reservasCargaGrupoPessoa)
                        repositorioReservaCargaGrupoPessoa.Deletar(reservaCargaGrupoPessoa);

                    repositorioPrevisaoDescarregamento.Deletar(previsaoDescarregamento, historico != null ? Auditado : null, historico);
                }
            }
        }

        private void PreencherExcecaoCapacidadeDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento) ?? throw new ControllerException("O centro de descarregamento deve ser informado.");

            excecao.CentroDescarregamento = centroDescarregamento;
            excecao.DataInicial = Request.GetDateTimeParam("DataInicial");
            excecao.DataFinal = Request.GetDateTimeParam("DataFinal");
            excecao.Descricao = Request.GetStringParam("Descricao");
            excecao.CapacidadeDescarregamento = Request.GetIntParam("CapacidadeDescarregamento");

            if (excecao.Codigo == 0)
                excecao.Usuario = Usuario;
        }

        private void SalvarPeriodoDescarregamentoCanaisVenda(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoCanaisVenda, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda repositorioPeriodoCanalVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> canaisVendaCadastrados = repositorioPeriodoCanalVenda.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda canalVendaExcluir in canaisVendaCadastrados)
                repositorioPeriodoCanalVenda.Deletar(canalVendaExcluir);

            dynamic canaisVenda = periodoCanaisVenda ?? new List<dynamic>();

            if (canaisVenda.Count == 0)
                return;

            foreach (var canalVenda in canaisVenda)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda periodoCanalVendaSalvar = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda()
                {
                    CanalVenda = new Dominio.Entidades.Embarcador.Pedidos.CanalVenda() { Codigo = ((string)canalVenda.Codigo).ToInt() },
                    PeriodoDescarregamento = periodo
                };

                repositorioPeriodoCanalVenda.Inserir(periodoCanalVendaSalvar);
            }
        }

        private void SalvarPeriodoDescarregamentoRemetentes(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoRemetentes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repPeriodoDescarregamentoRemetente = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> remetentesExcluir = repPeriodoDescarregamentoRemetente.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);
            foreach (var remetenteExluir in remetentesExcluir)
            {
                repPeriodoDescarregamentoRemetente.Deletar(remetenteExluir);
            };

            dynamic remetentes = periodoRemetentes ?? new List<dynamic>();

            if (remetentes.Count == 0)
                return;


            foreach (var remetente in remetentes)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente periodoRemetente = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente()
                {
                    Remetente = new Dominio.Entidades.Cliente() { CPF_CNPJ = ((string)remetente.Codigo).ToDouble() },
                    PeriodoDescarregamento = periodo
                };


                repPeriodoDescarregamentoRemetente.Inserir(periodoRemetente);
            }
        }

        private void SalvarPeriodoDescarregamentoGrupoPessoas(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoGruposPessoas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorioPeriodoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> gruposPessoaExcluir = repositorioPeriodoGrupoPessoa.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);
            foreach (var grupoPessoaExcluir in gruposPessoaExcluir)
                repositorioPeriodoGrupoPessoa.Deletar(grupoPessoaExcluir);

            dynamic gruposPessoa = periodoGruposPessoas ?? new List<dynamic>();

            if (gruposPessoa.Count == 0)
                return;


            foreach (var grupoPessoa in gruposPessoa)
            {

                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa periodoGrupoPessoa = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa()
                {
                    GrupoPessoas = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = ((string)grupoPessoa.Codigo).ToInt() },
                    PeriodoDescarregamento = periodo
                };

                repositorioPeriodoGrupoPessoa.Inserir(periodoGrupoPessoa);
            }
        }

        private void SalvarPeriodoDescarregamentoTiposCarga(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoTiposCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repPeriodoDescarregamentoTipoCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> tiposCargaExluir = repPeriodoDescarregamentoTipoCarga.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);
            foreach (var tipoCargaExcluir in tiposCargaExluir)
            {
                repPeriodoDescarregamentoTipoCarga.Deletar(tipoCargaExcluir);
            };

            dynamic tiposCarga = periodoTiposCarga ?? new List<dynamic>();

            if (tiposCarga.Count == 0)
                return;


            foreach (var tipoCarga in tiposCarga)
            {
                    Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga periodoTipoCarga = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga()
                    {
                        TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = ((string)tipoCarga.Codigo).ToInt() },
                        PeriodoDescarregamento = periodo
                    };

                    repPeriodoDescarregamentoTipoCarga.Inserir(periodoTipoCarga);
            }
        }

        private void SalvarPeriodoDescarregamentoGruposProdutos(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoGruposProdutos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioPeriodoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> GruposProdutosCadastrados = repositorioPeriodoGrupoProduto.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto grupoProdutoExcluir in GruposProdutosCadastrados)
                repositorioPeriodoGrupoProduto.Deletar(grupoProdutoExcluir);

            dynamic GruposProdutos = periodoGruposProdutos ?? new List<dynamic>();

            if (GruposProdutos.Count == 0)
                return;

            foreach (var grupoProduto in GruposProdutos)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto periodoGrupoProdutoSalvar = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto()
                {
                    GrupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto() { Codigo = ((string)grupoProduto.Codigo).ToInt() },
                    PeriodoDescarregamento = periodo
                };

                repositorioPeriodoGrupoProduto.Inserir(periodoGrupoProdutoSalvar);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeDescarregamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeDescarregamento()
            {
                CodigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite")
            };
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
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.DataInicial, "DataInicial", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.DataFinal, "DataFinal", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CentroDescarregamento, "CentroDescarregamento", 30, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeDescarregamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento> listaExcecaoCapacidadeDescarregamento = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento>();

                var listaExcecaoCapacidadeDescarregamentoRetornar = (
                    from excecao in listaExcecaoCapacidadeDescarregamento
                    select new
                    {
                        excecao.Codigo,
                        DataInicial = excecao.DataInicial.ToString("dd/MM/yyyy"),
                        DataFinal = excecao.DataFinal.ToString("dd/MM/yyyy"),
                        excecao.Descricao,
                        CentroDescarregamento = excecao.CentroDescarregamento.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaExcecaoCapacidadeDescarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroDescarregamento")
                return "CentroDescarregamento.Descricao";

            return propriedadeOrdenar;
        }

        private void ValidarCapacidadeDescarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!excecao.CentroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosCarregamento = repositorioPeriodoDescarregamento.BuscarPorExcecao(excecao.Codigo);

            if (periodosCarregamento.Count == 0)
                return;

            DateTime dataBase = excecao.DataInicial.Date;

            while (dataBase <= excecao.DataFinal.Date)
            {
                if (excecao.CentroDescarregamento.TipoCapacidadeDescarregamentoPorPeso == TipoCapacidadeDescarregamentoPorPeso.DiaSemana)
                {
                    decimal pesoTotalDescarregamentoDia = repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoDia(codigoCargaDesconsiderar: 0, excecao.CentroDescarregamento.Codigo, dataBase, excecao.CentroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido);
                    int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamento(excecao.CentroDescarregamento.Codigo, dataBase);
                    decimal capacidadeDescarregamentoUtilizada = pesoTotalDescarregamentoDia - capacidadeDescarregamentoAdicional;

                    if (excecao.CapacidadeDescarregamento < capacidadeDescarregamentoUtilizada)
                        throw new ControllerException($"{Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CapacidadeDescarregamentoDia} {dataBase.ToDateString()} {Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.DeveSerMaiorOuIgual} {capacidadeDescarregamentoUtilizada.ToString("n0")}");
                }
                else if (excecao.CentroDescarregamento.TipoCapacidadeDescarregamentoPorPeso == TipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento)
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento in periodosCarregamento)
                    {
                        List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda?.Select(canalVenda => canalVenda.CanalVenda?.Codigo ?? 0).ToList();
                        decimal pesoTotalDescarregamentoPeriodo = repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoPeriodo(codigoCargaDesconsiderar: 0, excecao.CentroDescarregamento.Codigo, dataBase, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, excecao.CentroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido, codigosCanaisVenda);
                        int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamentoPorPeriodo(excecao.CentroDescarregamento.Codigo, dataBase, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, codigosCanaisVenda);
                        decimal capacidadeDescarregamentoUtilizada = pesoTotalDescarregamentoPeriodo - capacidadeDescarregamentoAdicional;

                        if (periodoDescarregamento.CapacidadeDescarregamento < capacidadeDescarregamentoUtilizada)
                            throw new ControllerException($"{Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.CapacidadeDescarregamentoDia} {dataBase.ToDateString()} {periodoDescarregamento.DescricaoPeriodo.ToLower()} {Localization.Resources.Logistica.ExcecaoCapacidadeDescarregamento.DeveSerMaiorOuIgual} {capacidadeDescarregamentoUtilizada.ToString("n0")}");
                    }
                }

                dataBase = dataBase.AddDays(1);
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Centro de Descarregamento", Propriedade = "CentroDescarregamento", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Data Inicial", Propriedade = "DataInicial", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Data Final", Propriedade = "DataFinal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Horário de Início", Propriedade = "HorarioInicio", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Horário de Término", Propriedade = "HorarioTermino", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Descarregamento", Propriedade = "DescarregamentoSimultaneo", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Tipo de Carga", Propriedade = "CodigoIntegracaoTipoDeCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Quantidade por Tipo de Carga", Propriedade = "QuantidadeTipoCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Grupo de produto", Propriedade = "GrupoProduto", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private static TimeSpan ObterHoraImportacao(string hora)
        {
            try
            {
                DateTime.TryParse(hora, out DateTime data);

                return new TimeSpan(data.Hour, data.Minute, 0);
            }
            catch
            {
                return TimeSpan.MinValue;
            }
        }

        #endregion
    }
}

