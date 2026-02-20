using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/DocumentacaoAFRMM")]
    public class DocumentacaoAFRMMController : BaseController
    {
		#region Construtores

		public DocumentacaoAFRMMController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Publicos
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentosParaDocumentacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string numeroBooking = Request.GetStringParam("NumeroBooking");
                string numeroControle = Request.GetStringParam("NumeroControle");
                string numeroManifesto = Request.GetStringParam("NumeroManifesto");
                string numeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
                string numeroCEMercante = Request.GetStringParam("NumeroCEMercante");

                DateTime descargaPODInicial = Request.GetDateTimeParam("DescargaPODInicial");
                DateTime descargaPODFinal = Request.GetDateTimeParam("DescargaPODFinal");
                DateTime envioDocInicial = Request.GetDateTimeParam("EnvioDocInicial");
                DateTime envioDocFinal = Request.GetDateTimeParam("EnvioDocFinal");

                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoPortoDestino = Request.GetIntParam("PortoDestino");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao situacaoEnvio = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº CTe", "Numero", 7, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Serviço", "DescricaoTipoServico", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modal", "DescricaoTipoModal", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N/V/D", "Viagem", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Descarga", "DataDescarga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Envio Doc", "DataUltimoEnvioDocumentacaoAFRMM", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação Envio Doc", "DescricaoSituacaoEnvioDocumentacaAFRMM", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação Envio e-mail", "DescricaoSituacaoEnvioDocumentacaAFRMMEmail", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Manifesto", "NumeroManifesto", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Manifesto Transbordo", "NumeroManifestoTransbordo", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº CE", "NumeroCEMercante", 7, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCarga.ConsultarConhecimentosDocumentacaoAFRMM(numeroBooking, numeroControle, numeroManifesto, numeroManifestoTransbordo, numeroCEMercante, descargaPODInicial, descargaPODFinal, envioDocInicial, envioDocFinal, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoServico, situacaoEnvio, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCarga.ContarConsultarConhecimentosDocumentacaoAFRMM(numeroBooking, numeroControle, numeroManifesto, numeroManifestoTransbordo, numeroCEMercante, descargaPODInicial, descargaPODFinal, envioDocInicial, envioDocFinal, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoServico, situacaoEnvio));

                var lista = (from p in listaCTe
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroBooking,
                                 p.Numero,
                                 p.NumeroControle,
                                 DescricaoTipoServico = p.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || p.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario ? "Redespacho Intermediário" : p.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ? "Subcontratação" : p.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && p.NumeroControle.StartsWith("SVM") ? "Vinculado Multimodal Próprio" : p.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal ? "Vinculado Multimodal Terceiro" : "Normal",
                                 p.DescricaoTipoModal,
                                 Viagem = p.Viagem?.Descricao ?? "",
                                 PortoOrigem = p.PortoOrigem?.Descricao ?? "",
                                 PortoDestino = p.PortoDestino?.Descricao ?? "",
                                 DataDescarga = p.DataDescarga.HasValue ? p.DataDescarga.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 p.DescricaoStatus,
                                 DataUltimoEnvioDocumentacaoAFRMM = p.DataUltimoEnvioDocumentacaoAFRMM.HasValue ? p.DataUltimoEnvioDocumentacaoAFRMM.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 p.DescricaoSituacaoEnvioDocumentacaAFRMM,
                                 p.DescricaoSituacaoEnvioDocumentacaAFRMMEmail,
                                 p.NumeroManifesto,
                                 p.NumeroManifestoTransbordo,
                                 p.NumeroCEMercante
                             }).ToList();

                grid.AdicionaRows(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaConhecimentosParaDocumentacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string numeroBooking = Request.GetStringParam("NumeroBooking");
                string numeroControle = Request.GetStringParam("NumeroControle");
                string numeroManifesto = Request.GetStringParam("NumeroManifesto");
                string numeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
                string numeroCEMercante = Request.GetStringParam("NumeroCEMercante");

                DateTime descargaPODInicial = Request.GetDateTimeParam("DescargaPODInicial");
                DateTime descargaPODFinal = Request.GetDateTimeParam("DescargaPODFinal");
                DateTime envioDocInicial = Request.GetDateTimeParam("EnvioDocInicial");
                DateTime envioDocFinal = Request.GetDateTimeParam("EnvioDocFinal");

                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoPortoDestino = Request.GetIntParam("PortoDestino");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao situacaoEnvio = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº CTe", "Numero", 7, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Serviço", "DescricaoTipoServico", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modal", "DescricaoTipoModal", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N/V/D", "Viagem", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Descarga", "DataDescarga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Envio Doc", "DataUltimoEnvioDocumentacaoAFRMM", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação Envio Doc", "DescricaoSituacaoEnvioDocumentacaAFRMM", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação Envio e-mail", "DescricaoSituacaoEnvioDocumentacaAFRMMEmail", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Manifesto", "NumeroManifesto", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Manifesto Transbordo", "NumeroManifestoTransbordo", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº CE", "NumeroCEMercante", 7, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCarga.ConsultarConhecimentosDocumentacaoAFRMM(numeroBooking, numeroControle, numeroManifesto, numeroManifestoTransbordo, numeroCEMercante, descargaPODInicial, descargaPODFinal, envioDocInicial, envioDocFinal, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoServico, situacaoEnvio, propOrdenar, grid.dirOrdena, grid.inicio, 0);
                grid.setarQuantidadeTotal(repCarga.ContarConsultarConhecimentosDocumentacaoAFRMM(numeroBooking, numeroControle, numeroManifesto, numeroManifestoTransbordo, numeroCEMercante, descargaPODInicial, descargaPODFinal, envioDocInicial, envioDocFinal, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoServico, situacaoEnvio));

                var lista = (from p in listaCTe
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroBooking,
                                 p.Numero,
                                 p.NumeroControle,
                                 DescricaoTipoServico = p.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || p.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario ? "Redespacho Intermediário" : p.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ? "Subcontratação" : p.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && p.NumeroControle.StartsWith("SVM") ? "Vinculado Multimodal Próprio" : p.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal ? "Vinculado Multimodal Terceiro" : "Normal",
                                 p.DescricaoTipoModal,
                                 Viagem = p.Viagem?.Descricao ?? "",
                                 PortoOrigem = p.PortoOrigem?.Descricao ?? "",
                                 PortoDestino = p.PortoDestino?.Descricao ?? "",
                                 DataDescarga = p.DataDescarga.HasValue ? p.DataDescarga.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 p.DescricaoStatus,
                                 DataUltimoEnvioDocumentacaoAFRMM = p.DataUltimoEnvioDocumentacaoAFRMM.HasValue ? p.DataUltimoEnvioDocumentacaoAFRMM.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 p.DescricaoSituacaoEnvioDocumentacaAFRMM,
                                 p.DescricaoSituacaoEnvioDocumentacaAFRMMEmail,
                                 p.NumeroManifesto,
                                 p.NumeroManifestoTransbordo,
                                 p.NumeroCEMercante
                             }).ToList();

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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
        

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string emailEnvio = Request.GetStringParam("EmailEnvio");
                if (string.IsNullOrWhiteSpace(emailEnvio))
                    return new JsonpResult(false, "Favor informe o e-mail para envio manual.");

                List<int> codigosCTes = new List<int>();
                codigosCTes = RetornaCodigosConhecimentos(unitOfWork);

                if (codigosCTes == null || codigosCTes.Count == 0)
                    return new JsonpResult(false, "Nenhum documento selecionado, favor verifique os filtros realizados.");

                if (codigosCTes.Count > 8000)
                    return new JsonpResult(false, "O limite de envios é 8000.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM repEnvioDocumentacaoAFRMM = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(codigosCTes);

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoAFRMM = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");

                envioDocumentacaoAFRMM.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando;
                envioDocumentacaoAFRMM.DataGeracao = DateTime.Now;
                envioDocumentacaoAFRMM.EmailInformadoManualmente = Request.GetStringParam("EmailEnvio");
                envioDocumentacaoAFRMM.NumeroBooking = Request.GetStringParam("NumeroBooking");
                envioDocumentacaoAFRMM.NumeroControle = Request.GetStringParam("NumeroControle");
                envioDocumentacaoAFRMM.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(Request.GetIntParam("PedidoViagemDirecao"));
                envioDocumentacaoAFRMM.SituacaoEnvioDocumentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.NaoEnviado;// Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);
                envioDocumentacaoAFRMM.PortoDestino = repPorto.BuscarPorCodigo(Request.GetIntParam("PortoDestino"));
                envioDocumentacaoAFRMM.PortoOrigem = repPorto.BuscarPorCodigo(Request.GetIntParam("PortoOrigem"));
                envioDocumentacaoAFRMM.Usuario = this.Usuario;
                envioDocumentacaoAFRMM.TipoDocumentacaoAFRMM = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentacaoAFRMM.Email;
                envioDocumentacaoAFRMM.Retorno = "";
                envioDocumentacaoAFRMM.NotificadoOperador = false;
                envioDocumentacaoAFRMM.QuantidadeTentativaEnvio = 0;
                envioDocumentacaoAFRMM.DescargaPODFinal = Request.GetNullableDateTimeParam("DescargaPODFinal");
                envioDocumentacaoAFRMM.DescargaPODInicial = Request.GetNullableDateTimeParam("DescargaPODInicial");
                envioDocumentacaoAFRMM.EnvioDocInicial = Request.GetNullableDateTimeParam("EnvioDocInicial");
                envioDocumentacaoAFRMM.EnvioDocFinal = Request.GetNullableDateTimeParam("EnvioDocFinal");
                envioDocumentacaoAFRMM.NumeroManifesto = Request.GetStringParam("NumeroManifesto");
                envioDocumentacaoAFRMM.NumeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
                envioDocumentacaoAFRMM.NumeroCEMercante = Request.GetStringParam("NumeroCEMercante");

                envioDocumentacaoAFRMM.TiposServicos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>();
                if (tipoServico != null && tipoServico.Count > 0)
                {
                    foreach (var tipo in tipoServico)
                    {
                        envioDocumentacaoAFRMM.TiposServicos.Add(tipo);
                    }
                }

                envioDocumentacaoAFRMM.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (ctes != null && ctes.Count > 0)
                {
                    foreach (var cte in ctes)
                    {
                        envioDocumentacaoAFRMM.CTes.Add(cte);
                    }
                }

                repEnvioDocumentacaoAFRMM.Inserir(envioDocumentacaoAFRMM);

                return new JsonpResult(true, "Sucesso");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o envio da documentação por e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarDocumentacaoAFRMM()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCTes = new List<int>();
                codigosCTes = RetornaCodigosConhecimentos(unitOfWork);

                if (codigosCTes == null || codigosCTes.Count == 0)
                    return new JsonpResult(false, "Nenhum documento selecionado, favor verifique os filtros realizados.");

                if (codigosCTes.Count > 8000)
                    return new JsonpResult(false, "O limite de envios é 8000.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM repEnvioDocumentacaoAFRMM = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoAFRMM(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(codigosCTes);

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM envioDocumentacaoAFRMM = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");

                envioDocumentacaoAFRMM.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando;
                envioDocumentacaoAFRMM.DataGeracao = DateTime.Now;
                envioDocumentacaoAFRMM.EmailInformadoManualmente = Request.GetStringParam("EmailEnvio");
                envioDocumentacaoAFRMM.NumeroBooking = Request.GetStringParam("NumeroBooking");
                envioDocumentacaoAFRMM.NumeroControle = Request.GetStringParam("NumeroControle");
                envioDocumentacaoAFRMM.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(Request.GetIntParam("PedidoViagemDirecao"));
                envioDocumentacaoAFRMM.SituacaoEnvioDocumentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.NaoEnviado;//Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);
                envioDocumentacaoAFRMM.PortoDestino = repPorto.BuscarPorCodigo(Request.GetIntParam("PortoDestino"));
                envioDocumentacaoAFRMM.PortoOrigem = repPorto.BuscarPorCodigo(Request.GetIntParam("PortoOrigem"));
                envioDocumentacaoAFRMM.Usuario = this.Usuario;
                envioDocumentacaoAFRMM.TipoDocumentacaoAFRMM = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentacaoAFRMM.FTP;
                envioDocumentacaoAFRMM.Retorno = "";
                envioDocumentacaoAFRMM.NotificadoOperador = false;
                envioDocumentacaoAFRMM.QuantidadeTentativaEnvio = 0;
                envioDocumentacaoAFRMM.DescargaPODFinal = Request.GetNullableDateTimeParam("DescargaPODFinal");
                envioDocumentacaoAFRMM.DescargaPODInicial = Request.GetNullableDateTimeParam("DescargaPODInicial");
                envioDocumentacaoAFRMM.EnvioDocInicial = Request.GetNullableDateTimeParam("EnvioDocInicial");
                envioDocumentacaoAFRMM.EnvioDocFinal = Request.GetNullableDateTimeParam("EnvioDocFinal");
                envioDocumentacaoAFRMM.NumeroManifesto = Request.GetStringParam("NumeroManifesto");
                envioDocumentacaoAFRMM.NumeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
                envioDocumentacaoAFRMM.NumeroCEMercante = Request.GetStringParam("NumeroCEMercante");

                envioDocumentacaoAFRMM.TiposServicos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>();
                if (tipoServico != null && tipoServico.Count > 0)
                {
                    foreach (var tipo in tipoServico)
                    {
                        envioDocumentacaoAFRMM.TiposServicos.Add(tipo);
                    }
                }

                envioDocumentacaoAFRMM.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (ctes != null && ctes.Count > 0)
                {
                    foreach (var cte in ctes)
                    {
                        envioDocumentacaoAFRMM.CTes.Add(cte);
                    }
                }

                repEnvioDocumentacaoAFRMM.Inserir(envioDocumentacaoAFRMM);

                return new JsonpResult(true, "Sucesso");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o envio da documentação por e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        #endregion

        #region Métodos Privados

        private List<int> RetornaCodigosConhecimentos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaConhecimentos")))
            {
                dynamic listaCTes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConhecimentos"));
                if (listaCTes != null)
                {
                    foreach (var cte in listaCTes)
                    {
                        listaCodigos.Add(int.Parse((string)cte.Codigo));
                    }
                }
                else
                    listaCodigos = RetornaTodosCodigosConhecimentos(unidadeDeTrabalho);
            }
            else
            {
                listaCodigos = RetornaTodosCodigosConhecimentos(unidadeDeTrabalho);
            }
            return listaCodigos;
        }

        private List<int> RetornaTodosCodigosConhecimentos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            string numeroBooking = Request.GetStringParam("NumeroBooking");
            string numeroControle = Request.GetStringParam("NumeroControle");
            string numeroManifesto = Request.GetStringParam("NumeroManifesto");
            string numeroManifestoTransbordo = Request.GetStringParam("NumeroManifestoTransbordo");
            string numeroCEMercante = Request.GetStringParam("NumeroCEMercante");

            DateTime descargaPODInicial = Request.GetDateTimeParam("DescargaPODInicial");
            DateTime descargaPODFinal = Request.GetDateTimeParam("DescargaPODFinal");
            DateTime envioDocInicial = Request.GetDateTimeParam("EnvioDocInicial");
            DateTime envioDocFinal = Request.GetDateTimeParam("EnvioDocFinal");

            int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
            int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
            int codigoPortoDestino = Request.GetIntParam("PortoDestino");

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao situacaoEnvio = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            List<int> codigosCTes = new List<int>();

            codigosCTes = repCarga.ConsultarCodigosConhecimentosDocumentacaoAFRMM(numeroBooking, numeroControle, numeroManifesto, numeroManifestoTransbordo, numeroCEMercante, descargaPODInicial, descargaPODFinal, envioDocInicial, envioDocFinal, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, tipoServico, situacaoEnvio);

            return codigosCTes.Distinct().ToList();
        }

        #endregion
    }
}
