using Dominio.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Models.Grid;
using Repositorio;
using System.Text;
using com.alianca.intercab.emp.doc.booking;
using com.maersk.vessel.smds.operations.MSK;
using Alianca.PushService.Domain.Models.Avro;
using com.maersk.customer.smds.commercial.msk;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/AuditoriaEMP")]
    public class AuditoriaEMPController : BaseController
    {
		#region Construtores

		public AuditoriaEMPController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as Auditorias EMP.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaRecebido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisaRecebido(unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as Auditorias EMP.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisaRecebido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisaRecebido(unitOfWork);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRecebimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoIntegracaoEMPRecebimento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento repositorioIntegracaoEMPRecebimento = new Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento integracaoEMPRecebimento = repositorioIntegracaoEMPRecebimento.BuscarPorCodigo(codigoIntegracaoEMPRecebimento);

                List<TipoIntegracaoEMP> tiposPermitidos = new List<TipoIntegracaoEMP> { TipoIntegracaoEMP.Vessel, TipoIntegracaoEMP.Booking, TipoIntegracaoEMP.Container, TipoIntegracaoEMP.Customer, TipoIntegracaoEMP.Schedule };

                if (integracaoEMPRecebimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!tiposPermitidos.Contains(integracaoEMPRecebimento.TipoIntegracao))
                    return new JsonpResult(false, true, "Esse tipo de registro não pode ser reprocessado.");

                if (string.IsNullOrWhiteSpace(integracaoEMPRecebimento.ArquivoRecebimento))
                    return new JsonpResult(false, true, "Registro não possui JSON para reprocessamento.");

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Texto = "Integração EMP",
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                };

                if (integracaoEMPRecebimento.TipoIntegracao == TipoIntegracaoEMP.Booking)
                {
                    IntercabDocBooking booking = Newtonsoft.Json.JsonConvert.DeserializeObject<IntercabDocBooking>(integracaoEMPRecebimento.ArquivoRecebimento);

                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).RecebimentoBooking("Manual", booking, out string msgRetorno, auditado, TipoServicoMultisoftware);
                }
                else if (integracaoEMPRecebimento.TipoIntegracao == TipoIntegracaoEMP.Vessel)
                {
                    vesselMessage vessel = new vesselMessage();
                    vessel.vessel = Newtonsoft.Json.JsonConvert.DeserializeObject<com.maersk.vessel.smds.operations.MSK.vessel>(integracaoEMPRecebimento.ArquivoRecebimento);

                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).RecebimentoNavio("Manual", vessel, out string msgRetorno, auditado);
                }
                else if (integracaoEMPRecebimento.TipoIntegracao == TipoIntegracaoEMP.Container)
                {
                    ContainerViagem containerViagem = Newtonsoft.Json.JsonConvert.DeserializeObject<ContainerViagem>(integracaoEMPRecebimento.ArquivoRecebimento);

                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).RecebimentoContainer("Manual", containerViagem, out string msgRetorno, auditado, TipoServicoMultisoftware);
                }
                else if (integracaoEMPRecebimento.TipoIntegracao == TipoIntegracaoEMP.Customer)
                {
                    CustomerMessage customer = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomerMessage>(integracaoEMPRecebimento.ArquivoRecebimento);

                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).RecebimentoPessoa("Manual", customer, out string msgRetorno, auditado);
                }
                else if (integracaoEMPRecebimento.TipoIntegracao == TipoIntegracaoEMP.Schedule)
                {
                    com.schedule.dto.ScheduleEvent schedule = Newtonsoft.Json.JsonConvert.DeserializeObject<com.schedule.dto.ScheduleEvent>(integracaoEMPRecebimento.ArquivoRecebimento);

                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).RecebimentoSchedule("Manual", schedule, out string msgRetorno, auditado);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirJustificativaRecebimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoIntegracaoEMPRecebimento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento repositorioIntegracaoEMPRecebimento = new Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento integracaoEMPRecebimento = repositorioIntegracaoEMPRecebimento.BuscarPorCodigo(codigoIntegracaoEMPRecebimento);

                if (integracaoEMPRecebimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                SituacaoIntegracaoEMP situacaoIntegracaoEMPRecebido = Request.GetEnumParam<SituacaoIntegracaoEMP>("SituacaoIntegracaoEMPRecebido");
                string justificativaRecebido = Request.GetStringParam("JustificativaRecebido");

                integracaoEMPRecebimento.SituacaoIntegracao = situacaoIntegracaoEMPRecebido;
                integracaoEMPRecebimento.Justificativa = justificativaRecebido;

                unitOfWork.Start();

                repositorioIntegracaoEMPRecebimento.Atualizar(integracaoEMPRecebimento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirJustificativaEnvio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoIntegracaoEMPEnvio = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEMPLog repositorioIntegracaoEMPEnvio = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPEnvio = repositorioIntegracaoEMPEnvio.BuscarPorCodigo(codigoIntegracaoEMPEnvio);

                if (integracaoEMPEnvio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                SituacaoIntegracaoEMP situacaoIntegracaoEMPEnvio = Request.GetEnumParam<SituacaoIntegracaoEMP>("SituacaoIntegracaoEMPEnvio");
                string justificativaEnvio = Request.GetStringParam("JustificativaEnvio");

                integracaoEMPEnvio.SituacaoIntegracao = situacaoIntegracaoEMPEnvio;
                integracaoEMPEnvio.Justificativa = justificativaEnvio;

                unitOfWork.Start();

                repositorioIntegracaoEMPEnvio.Atualizar(integracaoEMPEnvio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoEnvio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMP = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMP = repIntegracaoEMP.BuscarPorCodigo(codigo);

                byte[] arquivo = null;
                string caminho = "";

                if (!string.IsNullOrWhiteSpace(integracaoEMP.ArquivoEnvio))
                {
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(integracaoEMP.ArquivoEnvio) + ".json";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                    else
                        return new JsonpResult(false, false, "Arquivo não encontrado.");
                }
                else
                    return new JsonpResult(false, false, "Arquivo não encontrado.");

                return Arquivo(arquivo, "application/json", "ArquivoEnvio.json");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMP = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMP = repIntegracaoEMP.BuscarPorCodigo(codigo);

                byte[] arquivo = null;
                string caminho = "";

                if (!string.IsNullOrWhiteSpace(integracaoEMP.ArquivoRetorno))
                {
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(integracaoEMP.ArquivoRetorno) + ".json";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                    else
                        return new JsonpResult(false, false, "Arquivo não encontrado.");
                }
                else
                    return new JsonpResult(false, false, "Arquivo não encontrado.");

                return Arquivo(arquivo, "application/json", "ArquivoRetorno.json");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoRecebimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento repIntegracaoEMP = new Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento integracaoEMP = repIntegracaoEMP.BuscarPorCodigo(codigo);

                byte[] arquivo = null;
                //string caminho = "";

                if (!string.IsNullOrWhiteSpace(integracaoEMP.ArquivoRecebimento))
                {
                    arquivo = Encoding.ASCII.GetBytes(integracaoEMP.ArquivoRecebimento);

                    //caminho = Utilidades.IO.FileStorageService.Storage.Combine(integracaoEMP.ArquivoRecebimento) + ".json";
                    //if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    //    arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                    //else
                    //    return new JsonpResult(false, false, "Arquivo não encontrado.");
                }
                else
                    return new JsonpResult(false, false, "Arquivo não encontrado.");

                return Arquivo(arquivo, "application/json", "ArquivoRecebimento.json");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Topic = Request.GetStringParam("Topic"),
                Schedule = Request.GetStringParam("Schedule"),
                Booking = Request.GetStringParam("Booking"),
                Customer = Request.GetStringParam("Customer"),
                Justificativa = Request.GetEnumParam<SimNao>("Justificativa"),
                Status = Request.GetNullableEnumParam<SituacaoIntegracaoEMP>("Status"),
                TipoIntegracao = Request.GetNullableEnumParam<TipoIntegracaoEMP>("TipoIntegracao"),
                Fatura = Request.GetStringParam("Fatura"),
                Boleto = Request.GetStringParam("Boleto"),
            };

            return filtrosPesquisa;
        }

        private string CorPorSituacaoIntegracaoEMP(Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog)
        {
            if (integracaoEMPLog.SituacaoIntegracao == SituacaoIntegracaoEMP.Integrado)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (integracaoEMPLog.SituacaoIntegracao == SituacaoIntegracaoEMP.ErroResolvido)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (integracaoEMPLog.SituacaoIntegracao == SituacaoIntegracaoEMP.NotPersist)
                return ClasseCorFundo.Danger(IntensidadeCor._100);

            return "";
        }

        private string CorPorSituacaoIntegracaoEMPRecebimento(Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento integracaoEMPLogRecebimento)
        {
            if (integracaoEMPLogRecebimento.SituacaoIntegracao == SituacaoIntegracaoEMP.Integrado)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (integracaoEMPLogRecebimento.SituacaoIntegracao == SituacaoIntegracaoEMP.ErroResolvido)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (integracaoEMPLogRecebimento.SituacaoIntegracao == SituacaoIntegracaoEMP.NotPersist)
                return ClasseCorFundo.Danger(IntensidadeCor._100);

            return "";
        }

        private Grid ObterGridPesquisa(UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data Integração", "DataEnvio", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Topic", "Topic", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Booking", "Booking", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Fatura", "Fatura", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Boleto", "Boleto", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo de Integração", "TipoIntegracao", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Mensagem Retorno", "MensagemRetorno", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Status", "StatusIntegracaoEMPDescricao", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação da Integração", "SituacaoIntegracao", 9, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", 8, Models.Grid.Align.center, false);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMP = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog> integracaoEMP = repIntegracaoEMP.Consultar(filtrosPesquisa, parametrosConsulta);
            int countIntegracaoEMP = repIntegracaoEMP.ContarConsulta(filtrosPesquisa);

            grid.setarQuantidadeTotal(countIntegracaoEMP);

            var lista = (from obj in integracaoEMP
                         select new
                         {
                             Codigo = obj.Codigo,
                             obj.Topic,
                             Booking = obj.NumeroBooking,
                             DataEnvio = obj.DataEnvio.ToString() ?? string.Empty,
                             MensagemRetorno = obj.MensageRetorno ?? string.Empty,
                             StatusIntegracaoEMPDescricao = obj.StatusIntegracaoEMP.ObterDescricao(),
                             TipoIntegracao = obj.TipoIntegracao.ObterDescricao(),
                             Justificativa = obj.Justificativa ?? string.Empty,
                             SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                             DT_RowClass = this.CorPorSituacaoIntegracaoEMP(obj),
                             Fatura = obj.Fatura ?? string.Empty,
                             Boleto = obj.Boleto ?? string.Empty,
                         }).ToList();

            grid.AdicionaRows(lista);
            return grid;
        }

        private Grid ObterGridPesquisaRecebido(UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAuditoriaEMP filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data Integração", "DataRecebimento", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Topic", "Topic", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Schedule", "ScheduleViagemNavio", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Booking", "NumeroBooking", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Fatura", "Fatura", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Boleto", "Boleto", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Customer", "CustomerCode", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo de Integração", "TipoIntegracao", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Mensagem Retorno", "MensagemRetorno", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação da Integração", "SituacaoIntegracao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", 8, Models.Grid.Align.center, false);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento repositorioIntegracaoEMP = new Repositorio.Embarcador.Integracao.IntegracaoEMPLogRecebimento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLogRecebimento> integracaoEMPRecebimento = repositorioIntegracaoEMP.Consultar(filtrosPesquisa, parametrosConsulta);
            int countIntegracaoEMP = repositorioIntegracaoEMP.ContarConsulta(filtrosPesquisa);

            grid.setarQuantidadeTotal(countIntegracaoEMP);

            var lista = (from obj in integracaoEMPRecebimento
                         select new
                         {
                             obj.Codigo,
                             obj.Topic,
                             ScheduleViagemNavio = obj.ScheduleViagemNavio ?? string.Empty,
                             obj.NumeroBooking,
                             obj.Fatura,
                             obj.CustomerCode,
                             DataRecebimento = obj.DataRecebimento.ToString() ?? string.Empty,
                             MensagemRetorno = obj.MensageRetorno ?? string.Empty,
                             TipoIntegracao = obj.TipoIntegracao.ObterDescricao(),
                             Justificativa = obj.Justificativa ?? string.Empty,
                             SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                             DT_RowClass = this.CorPorSituacaoIntegracaoEMPRecebimento(obj),
                         }).ToList();

            grid.AdicionaRows(lista);
            return grid;
        }

        #endregion Métodos Privados
    }
}
