using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Logistica.MotivoParadaCentro
{
    [CustomAuthorize("Logistica/MotivoParadaCentro")]
    public class MotivoParadaCentroController : BaseController
    {
		#region Construtores

		public MotivoParadaCentroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.MotivoParadaCentro repMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivo = new Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro();
                PreencherEntidade(motivo, unitOfWork);

                unitOfWork.Start();

                repMotivoParadaCentro.Inserir(motivo, Auditado);

                if (motivo.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                    SalvarTipoOperacao(motivo, null, unitOfWork);

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
                Repositorio.Embarcador.Logistica.MotivoParadaCentro repMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivo = repMotivoParadaCentro.BuscarPorCodigo(codigo, true);

                if (motivo == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherEntidade(motivo, unitOfWork);

                unitOfWork.Start();

                var historico = repMotivoParadaCentro.Atualizar(motivo, Auditado);
                if (motivo.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                    SalvarTipoOperacao(motivo, historico, unitOfWork);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.MotivoParadaCentro repMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivo = repMotivoParadaCentro.BuscarPorCodigo(codigo, true);

                if (motivo.DataInicio < DateTime.Now)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoEPossivelExcluirRegistroDataRetroativa);

                repMotivoParadaCentro.Deletar(motivo, Auditado);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
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
                Repositorio.Embarcador.Logistica.MotivoParadaCentro repMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivo = repMotivoParadaCentro.BuscarPorCodigo(codigo, false);

                if (motivo == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    motivo.Ativo,
                    CentroCarregamento = new
                    {
                        motivo.CentroCarregamento.Codigo,
                        motivo.CentroCarregamento.Descricao,
                        motivo.CentroCarregamento.LimiteCarregamentos
                    },
                    Data = motivo.DataInicio.ToDateString(),
                    PeriodoInicio = motivo.DataInicio.ToTimeString(),
                    PeriodoFim = motivo.DataFim.ToTimeString(),
                    PermiteEditar = motivo.DataInicio > DateTime.Now,
                    motivo.QuantidadeParada,

                    ContainerTipoOperacao = (
                        from tipoOperacao in motivo.TiposOperacao
                        select new
                        {
                            tipoOperacao.Codigo,
                            QuantidadeTipoOperacao = tipoOperacao.Quantidade,
                            TipoOperacao = new
                            {
                                tipoOperacao.TipoOperacao.Codigo,
                                tipoOperacao.TipoOperacao.Descricao,
                            }
                        }
                    ).ToList(),
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

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

            DateTime data = Request.GetDateTimeParam("Data");
            TimeSpan periodoInicio = Request.GetTimeParam("PeriodoInicio");
            TimeSpan periodoFim = Request.GetTimeParam("PeriodoFim");

            motivo.Descricao = Request.GetStringParam("Descricao");
            motivo.Ativo = Request.GetBoolParam("Ativo");
            motivo.QuantidadeParada = Request.GetIntParam("QuantidadeParada");
            motivo.CentroCarregamento = repCentroCarregamento.BuscarPorCodigo(Request.GetIntParam("CentroCarregamento")) ?? throw new ControllerException("Centro de Carregamento é obrigatório");
            motivo.DataInicio = data.Add(periodoInicio);
            motivo.DataFim = data.Add(periodoFim);

            if (motivo.DataInicio >= motivo.DataFim)
                throw new ControllerException(Localization.Resources.Logistica.MotivoParadaCentro.OPeriodoInicialNaoPodeSerMaiorQueOPeriodoFinal);

            if (motivo.DataInicio < DateTime.Now)
                throw new ControllerException(Localization.Resources.Logistica.MotivoParadaCentro.NaoEPossívelCadastrarUmaDataRetroativa);
        }

        private (int CentroCarregamento, string Descricao, SituacaoAtivoPesquisa Situacao) ObterFiltrosPesquisa()
        {
            int centroCarregamento = Request.GetIntParam("CentroCarregamento");
            string descricao = Request.GetStringParam("Descricao");
            SituacaoAtivoPesquisa situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo");

            return ValueTuple.Create(centroCarregamento, descricao, situacao);
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
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.MotivoParadaCentro.Centro, "CentroCarregamento", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Periodo, "Periodo", 30, Models.Grid.Align.left, true).Ord(false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Ativo", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                var filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Logistica.MotivoParadaCentro repMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(unitOfWork);
                int totalRegistros = repMotivoParadaCentro.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> listaMotivoParadaCentro = (totalRegistros > 0) ? repMotivoParadaCentro.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>();

                var retorno = listaMotivoParadaCentro.Select(motivo => new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    CentroCarregamento = motivo.CentroCarregamento.Descricao,
                    Periodo = $"({motivo.DataInicio.ToDateString()}) {motivo.DataInicio.ToTimeString()} - {motivo.DataFim.ToTimeString()}",
                    Ativo = motivo.DescricaoAtivo,
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            if (propriedadeOrdenar == "CentroCarregamento")
                return "CentroCarregamento.Descricao";

            return propriedadeOrdenar;
        }

        private void SalvarTipoOperacao(Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivo, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic dynTiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ContainerTipoOperacao"));

            Repositorio.Embarcador.Logistica.MotivoParadaCentroTipoOperacao repMotivoParadaCentroTipoOperacao = new Repositorio.Embarcador.Logistica.MotivoParadaCentroTipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            if (motivo.TiposOperacao == null)
                motivo.TiposOperacao = new List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao>();

            List<int> codigosTipoOperacaoAdicionadoOuAtualizado = new List<int>();
            foreach (var dynTipoOperacao in dynTiposOperacao)
            {
                int? codigo = ((string)dynTipoOperacao.TipoOperacao?.Codigo).ToNullableInt();

                if (!codigo.HasValue)
                    throw new ControllerException(Localization.Resources.Logistica.MotivoParadaCentro.NãoFoiPossivelEncontrarOTipoDeOperação);

                Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao tipoOperacao = repMotivoParadaCentroTipoOperacao.BuscarPorTipoOperacao(codigo.Value);
                if (tipoOperacao == null) tipoOperacao = new Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao();
                tipoOperacao.Initialize();

                tipoOperacao.MotivoParadaCentro = motivo;
                tipoOperacao.Quantidade = ((string)dynTipoOperacao.QuantidadeTipoOperacao).ToInt();
                tipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigo.Value);

                if (tipoOperacao.Codigo > 0)
                    repMotivoParadaCentroTipoOperacao.Atualizar(tipoOperacao, historico != null ? Auditado : null, historico);
                else
                    repMotivoParadaCentroTipoOperacao.Inserir(tipoOperacao, historico != null ? Auditado : null, historico);

                codigosTipoOperacaoAdicionadoOuAtualizado.Add(tipoOperacao.Codigo);
            }

            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao> registrosParaDeletar = (from o in motivo.TiposOperacao
                                                                                                                where !codigosTipoOperacaoAdicionadoOuAtualizado.Contains(o.Codigo)
                                                                                                                select o).ToList();

            foreach (var registro in registrosParaDeletar)
                repMotivoParadaCentroTipoOperacao.Deletar(registro, historico != null ? Auditado : null, historico);
        }

        #endregion
    }
}
