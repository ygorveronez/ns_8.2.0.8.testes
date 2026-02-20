using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "ObterDadosIntegracoes" }, "Ocorrencias/Ocorrencia")]
    public class OcorrenciaIntegracaoController : BaseController
    {
		#region Construtores

		public OcorrenciaIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia = Request.GetIntParam("Ocorrencia");
                bool filialEmissora = Request.GetBoolParam("FilialEmissora");

                Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
                //Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = repOcorrenciaCTeIntegracao.BuscarTipoIntegracaoPorOcorrencia(codigoOcorrencia);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repOcorrenciaEDIIntegracao.BuscarTipoIntegracaoPorOcorrenciaEFilialEmissora(codigoOcorrencia, filialEmissora);

                //List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCarga = repCargaCargaIntegracao.BuscarTipoIntegracaoPorCarga(codigoCarga);

                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                    TiposIntegracoesEDI = tiposIntegracoesEDI
                    //TiposIntegracoesCarga = tiposIntegracoesCarga
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia;
                int.TryParse(Request.Params("Ocorrencia"), out codigoOcorrencia);

                //Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeDeTrabalho);

                // Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (ocorrencia == null)
                    return new JsonpResult(true, false, "Ocorrência não encontrada.");

                if (ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao && ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.FalhaIntegracao)
                    return new JsonpResult(true, false, "A situação da ocorrência não permite a finalização da etapa.");

                if (ocorrencia.GerandoIntegracoes)
                    return new JsonpResult(false, true, "O sistema ainda está gerando as integrações, não sendo possível finalizar a etapa. Aguarde alguns minutos e tente novamente.");

                unidadeDeTrabalho.Start();

                //Dominio.Entidades.Embarcador.Cargas.CargaLog log = new Dominio.Entidades.Embarcador.Cargas.CargaLog();

                //log.Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCarga.FinalizarEtapaIntegracao;
                //log.Carga = carga;
                //log.Data = DateTime.Now;
                //log.Usuario = Usuario;

                //repLog.Inserir(log);

                ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
                repCargaOcorrencia.Atualizar(ocorrencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, null, "Finalizou etapa de integração.", unidadeDeTrabalho);
                unidadeDeTrabalho.CommitChanges();

                //svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        #region Metodos integracao Embarcador

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador filtros = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador> registros = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador>();

                int countRegistros = repOcorrenciaImportacaoEmbarcador.ContarConsulta(filtros);

                if (countRegistros > 0)
                    registros = repOcorrenciaImportacaoEmbarcador.Consultar(filtros, parametroConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       DataOcorrencia = obj.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                                       NumeroOcorrencia = obj.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                                       NumeroOcorrenciaEmbarcador = obj.NumeroOcorrenciaEmbarcador,
                                       Empresa = obj.Empresa?.Descricao ?? string.Empty,
                                       Situacao = obj.Situacao.ObterDescricao(),
                                       obj.Mensagem
                                   }).ToList());

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

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador filtros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaOcorrenciaIntegracaoEmbarcador()
            {
                CodigoEmpresa = Request.GetNullableListParam<int>("Empresa"),
                CodigoGrupoPessoa = Request.GetNullableListParam<int>("GrupoPessoa"),
                DataFinalOcorrencia = Request.GetNullableDateTimeParam("DataFinalOcorrencia"),
                DataInicialOcorrencia = Request.GetNullableDateTimeParam("DataInicialOcorrencia"),
                NumeroOcorrencia = Request.GetNullableStringParam("NumeroOcorrencia"),
                NumeroOcorrenciaEmbarcador = Request.GetNullableStringParam("NumeroOcorrenciaEmbarcador"),
                Situacao = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador>("Situacao"),
            };

            return filtros;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Ocorrência Embarcador", "NumeroOcorrenciaEmbarcador", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº Ocorrência", "NumeroOcorrencia", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Ocorrência", "DataOcorrencia", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mensagem Processamento", "Mensagem", 12, Models.Grid.Align.left, false);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            if (prop == "SituacaoCarga")
                return "Carga.SituacaoCarga";
            return prop;
        }

        #endregion
    }
}
