using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/RegraParcelamentoOcorrencia")]
    public class RegraParcelamentoOcorrenciaController : BaseController
    {
		#region Construtores

		public RegraParcelamentoOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia();

                PreencherRegraParcelamentoOcorrencia(regraParcelamentoOcorrencia, unitOfWork);
                repositorio.Inserir(regraParcelamentoOcorrencia, Auditado);
                AdicionarOuAtualizarParcelamentos(regraParcelamentoOcorrencia, unitOfWork);

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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (regraParcelamentoOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegraParcelamentoOcorrencia(regraParcelamentoOcorrencia, unitOfWork);
                AdicionarOuAtualizarParcelamentos(regraParcelamentoOcorrencia, unitOfWork);
                repositorio.Atualizar(regraParcelamentoOcorrencia, Auditado);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (regraParcelamentoOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento repositorioParcelamento = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentos = repositorioParcelamento.BuscarPorRegraParcelamentoOcorrencia(regraParcelamentoOcorrencia.Codigo);

                return new JsonpResult(new
                {
                    Regra = new
                    {
                        regraParcelamentoOcorrencia.Codigo,
                        regraParcelamentoOcorrencia.Descricao,
                        regraParcelamentoOcorrencia.PeriodoFaturamento,
                        regraParcelamentoOcorrencia.QuantidadePeriodos,
                        Status = regraParcelamentoOcorrencia.Ativo
                    },
                    Parcelamentos = (
                        from o in parcelamentos
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            NumeroParcelas = o.NumeroParcelas.ToString("n0"),
                            PercentualInicial = o.PercentualInicial.ToString("n2"),
                            PercentualFinal = o.PercentualFinal.ToString("n2"),
                            PercentualJurosParcela = o.PercentualJurosParcela.ToString("n2")
                        }
                    )
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
                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento repositorioParcelamento = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (regraParcelamentoOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioParcelamento.DeletarPorRegraParcelamentoOcorrencia(regraParcelamentoOcorrencia.Codigo);
                repositorio.Deletar(regraParcelamentoOcorrencia, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencherRegraParcelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            regraParcelamentoOcorrencia.Ativo = Request.GetBoolParam("Status");
            regraParcelamentoOcorrencia.Descricao = Request.GetStringParam("Descricao");
            regraParcelamentoOcorrencia.PeriodoFaturamento = Request.GetEnumParam<PeriodoFechamento>("PeriodoFaturamento");
            regraParcelamentoOcorrencia.QuantidadePeriodos = Request.GetIntParam("QuantidadePeriodos");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                string descricao = Request.GetStringParam("Descricao");
                SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Período de Faturamento", "PeriodoFaturamento", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Quantidade de Períodos", "QuantidadePeriodos", 20, Models.Grid.Align.center, true);

                if (status == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, status);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia> listaRegraParcelamentoOcorrencia = (totalRegistros > 0) ? repositorio.Consultar(descricao, status, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia>();

                var listaRegraParcelamentoOcorrenciaRetornar = (
                    from regraParcelamentoOcorrencia in listaRegraParcelamentoOcorrencia
                    select new
                    {
                        regraParcelamentoOcorrencia.Codigo,
                        regraParcelamentoOcorrencia.Descricao,
                        regraParcelamentoOcorrencia.DescricaoAtivo,
                        PeriodoFaturamento = regraParcelamentoOcorrencia.PeriodoFaturamento.ObterDescricao(),
                        regraParcelamentoOcorrencia.QuantidadePeriodos
                    }
                ).ToList();

                grid.AdicionaRows(listaRegraParcelamentoOcorrenciaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Privados - Parcelamento

        private void AdicionarOuAtualizarParcelamentos(Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic parcelamentosAdicionarOuAtualizar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaParcelamento"));
            Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento repositorioParcelamento = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentos = repositorioParcelamento.BuscarPorRegraParcelamentoOcorrencia(regraParcelamentoOcorrencia.Codigo);

            ExcluirParcelamentosRemovidos(regraParcelamentoOcorrencia, parcelamentos, parcelamentosAdicionarOuAtualizar, unitOfWork);
            InserirParcelamentosAdicionados(regraParcelamentoOcorrencia, parcelamentos, parcelamentosAdicionarOuAtualizar, unitOfWork);
        }

        private void ExcluirParcelamentosRemovidos(Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia, List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentos, dynamic parcelamentosAdicionarOuAtualizar, Repositorio.UnitOfWork unitOfWork)
        {
            if (parcelamentos.Count == 0)
                return;

            Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento repositorioParcelamento = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento(unitOfWork);
            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var parcelamentoAdicionarOuAtualizar in parcelamentosAdicionarOuAtualizar)
            {
                int? codigo = ((string)parcelamentoAdicionarOuAtualizar.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> listaParcelamentoRemover = (from o in parcelamentos where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento parcelamentoRemover in listaParcelamentoRemover)
                repositorioParcelamento.Deletar(parcelamentoRemover);
        }

        private void InserirParcelamentosAdicionados(Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia, List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentos, dynamic parcelamentosAdicionarOuAtualizar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento repositorioParcelamento = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentosCadastradosOuAtualizados = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento>();

            foreach (var parcelamentoAdicionarOuAtualizar in parcelamentosAdicionarOuAtualizar)
            {
                int? codigo = ((string)parcelamentoAdicionarOuAtualizar.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento parcelamentoSalvar;

                if (codigo.HasValue)
                    parcelamentoSalvar = repositorioParcelamento.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException("Parcelamento não encontrada");
                else
                    parcelamentoSalvar = new Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento();

                parcelamentoSalvar.RegraParcelamentoOcorrencia = regraParcelamentoOcorrencia;
                parcelamentoSalvar.NumeroParcelas = ((string)parcelamentoAdicionarOuAtualizar.NumeroParcelas).ToInt();
                parcelamentoSalvar.PercentualInicial = ((string)parcelamentoAdicionarOuAtualizar.PercentualInicial).ToDecimal();
                parcelamentoSalvar.PercentualFinal = ((string)parcelamentoAdicionarOuAtualizar.PercentualFinal).ToDecimal();
                parcelamentoSalvar.PercentualJurosParcela = ((string)parcelamentoAdicionarOuAtualizar.PercentualJurosParcela).ToDecimal();

                ValidarParcelamentoDuplicado(parcelamentosCadastradosOuAtualizados, parcelamentoSalvar);

                if (codigo.HasValue)
                    repositorioParcelamento.Atualizar(parcelamentoSalvar);
                else
                    repositorioParcelamento.Inserir(parcelamentoSalvar);

                parcelamentosCadastradosOuAtualizados.Add(parcelamentoSalvar);
            }
        }

        private void ValidarParcelamentoDuplicado(List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentosCadastradosOuAtualizados, Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento parcelamentoSalvar)
        {
            Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento parcelamentoDuplicado = (
                from parcelamento in parcelamentosCadastradosOuAtualizados
                where (
                    (parcelamentoSalvar.PercentualInicial >= parcelamento.PercentualInicial && parcelamentoSalvar.PercentualInicial <= parcelamento.PercentualFinal) ||
                    (parcelamentoSalvar.PercentualFinal >= parcelamento.PercentualInicial && parcelamentoSalvar.PercentualFinal <= parcelamento.PercentualFinal) ||
                    (parcelamento.PercentualInicial >= parcelamentoSalvar.PercentualInicial && parcelamento.PercentualInicial <= parcelamentoSalvar.PercentualFinal) ||
                    (parcelamento.PercentualFinal >= parcelamentoSalvar.PercentualInicial && parcelamento.PercentualFinal <= parcelamentoSalvar.PercentualFinal)
                )
                select parcelamento
            ).FirstOrDefault();

            if (parcelamentoDuplicado != null)
                throw new ControllerException($"Já existe um cadastro que contém a faixa de percentual {parcelamentoSalvar.Descricao.ToLower()}");
        }

        #endregion
    }
}
