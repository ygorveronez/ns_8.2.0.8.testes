using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ImpressaoLoteCarga
{
    [CustomAuthorize("Cargas/CancelamentoCargaLote", "Cargas/ImpressaoLoteCarga")]
    public class ImpressaoLoteCargaController : BaseController
    {
        #region Construtores

        public ImpressaoLoteCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentosParaImpressao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                double provedorOS = Request.GetDoubleParam("ProvedorOS");

                string numeroBooking = Request.GetStringParam("NumeroBooking");
                string numeroOS = Request.GetStringParam("NumeroOS");
                string numeroControle = Request.GetStringParam("NumeroControle");

                int numeroFiscal = Request.GetIntParam("NumeroFiscal");

                int codigoContainer = Request.GetIntParam("Container");
                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");

                Dominio.Enumeradores.OpcaoSimNaoPesquisa foiAnulado = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiAnulado");
                Dominio.Enumeradores.OpcaoSimNaoPesquisa foiSubstituido = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiSubstituido");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao modalEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao>("ModalEnvio");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao situacaoEnvio = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

                if (codigoTerminalOrigem > 0 && codigoPedidoViagemDirecao > 0)
                {
                    if (tipoPropostaMultimodal == null || tipoPropostaMultimodal.Count == 0)
                        return new JsonpResult(false, "Favor informe ao menos um Tipo de Proposta.");
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº CTe", "Numero", 7, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Container", "ListaContainer", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Provedor O.S.", "ClienteProvedorOS", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº O.S.", "NumeroOS", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modal", "DescricaoTipoModal", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Último Email Enviado", "UltimoEmailEnvioDocumentacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Último Envio", "DataUltimoEnvioDocumentacao", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCarga.ConsultarConhecimentosParaImpressaoLote(modalEnvioDocumentacao, foiAnulado, foiSubstituido, tipoServico, situacaoEnvio, provedorOS, numeroBooking, numeroOS, numeroControle, numeroFiscal, codigoContainer, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, codigoGrupoPessoa, tiposPropostasMultimodal, tipoPropostaMultimodal, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCarga.ContarConsultarConhecimentosParaImpressaoLote(modalEnvioDocumentacao, foiAnulado, foiSubstituido, tipoServico, situacaoEnvio, provedorOS, numeroBooking, numeroOS, numeroControle, numeroFiscal, codigoContainer, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, codigoGrupoPessoa, tiposPropostasMultimodal, tipoPropostaMultimodal));

                var lista = (from p in listaCTe
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 p.ListaContainer,
                                 p.NumeroBooking,
                                 ClienteProvedorOS = p.ClienteProvedorOS?.Descricao ?? "",
                                 p.NumeroOS,
                                 DataEmissao = p.DataEmissao.HasValue ? p.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                 p.DescricaoStatus,
                                 p.DescricaoTipoModal,
                                 p.UltimoEmailEnvioDocumentacao,
                                 DataUltimoEnvioDocumentacao = p.DataUltimoEnvioDocumentacao.HasValue ? p.DataUltimoEnvioDocumentacao.Value.ToString("dd/MM/yyyy HH:mm") : ""
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
        public async Task<IActionResult> GerarImpressaoLoteCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCTes = new List<int>();
                codigosCTes = RetornaCodigosConhecimentos(unitOfWork);

                bool informarEmailEnvio = Request.GetBoolParam("InformarEmailEnvio");
                string emailEnvio = Request.GetStringParam("EmailEnvio");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao formaEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao>("FormaEnvioDocumentacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao modalEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao>("ModalEnvio");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(codigosCTes);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDestinoPorto = repCTe.BuscarPorCodigoEDestinoPorto(codigosCTes);

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote();

                double provedorOS = Request.GetDoubleParam("ProvedorOS");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

                envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando;
                envioDocumentacaoLote.Container = repContainer.BuscarPorCodigo(Request.GetIntParam("Container"));
                envioDocumentacaoLote.DataGeracao = DateTime.Now;
                envioDocumentacaoLote.EmailInformadoManualmente = Request.GetStringParam("EmailEnvio");
                envioDocumentacaoLote.FoiAnulado = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiAnulado");
                envioDocumentacaoLote.FoiSubstituido = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiSubstituido");
                envioDocumentacaoLote.FormaEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao>("FormaEnvioDocumentacao");
                envioDocumentacaoLote.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(Request.GetIntParam("GrupoPessoa"));
                envioDocumentacaoLote.ModalEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao>("ModalEnvio");
                envioDocumentacaoLote.NumeroBooking = Request.GetStringParam("NumeroBooking");
                envioDocumentacaoLote.NumeroFiscal = Request.GetIntParam("NumeroFiscal");
                envioDocumentacaoLote.NumeroOS = Request.GetStringParam("NumeroOS");
                envioDocumentacaoLote.NumeroControle = Request.GetStringParam("NumeroControle");
                envioDocumentacaoLote.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(Request.GetIntParam("PedidoViagemDirecao"));
                envioDocumentacaoLote.ProvedorOS = provedorOS > 0 ? repCliente.BuscarPorCPFCNPJ(provedorOS) : null;
                envioDocumentacaoLote.SituacaoEnvioDocumentacao = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);
                envioDocumentacaoLote.TerminalDestino = repTerminal.BuscarPorCodigo(Request.GetIntParam("TerminalDestino"));
                envioDocumentacaoLote.TerminalOrigem = repTerminal.BuscarPorCodigo(Request.GetIntParam("TerminalOrigem"));
                envioDocumentacaoLote.Usuario = this.Usuario;
                envioDocumentacaoLote.TipoImpressaoLote = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoLote.Impressao;
                envioDocumentacaoLote.EnvioAutomatico = false;
                envioDocumentacaoLote.NotificadoOperador = false;
                envioDocumentacaoLote.QuantidadeTentativaEnvio = 0;

                envioDocumentacaoLote.TiposProposta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                if (tipoPropostaMultimodal != null && tipoPropostaMultimodal.Count > 0)
                {
                    foreach (var tipoProposta in tipoPropostaMultimodal)
                    {
                        envioDocumentacaoLote.TiposProposta.Add(tipoProposta);
                    }
                }
                if (tiposPropostasMultimodal != null && tiposPropostasMultimodal.Count > 0)
                {
                    foreach (var tipoProposta in tiposPropostasMultimodal)
                    {
                        envioDocumentacaoLote.TiposProposta.Add(tipoProposta);
                    }
                }
                envioDocumentacaoLote.TiposServicos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>();
                if (tipoServico != null && tipoServico.Count > 0)
                {
                    foreach (var tipo in tipoServico)
                    {
                        envioDocumentacaoLote.TiposServicos.Add(tipo);
                    }
                }

                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (ctes != null && ctes.Count > 0)
                {
                    foreach (var cte in ctes)
                    {
                        envioDocumentacaoLote.CTes.Add(cte);
                    }
                }

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a impressão em lote.");
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
                List<int> codigosCTes = new List<int>();
                codigosCTes = RetornaCodigosConhecimentos(unitOfWork);

                bool informarEmailEnvio = Request.GetBoolParam("InformarEmailEnvio");
                string emailEnvio = Request.GetStringParam("EmailEnvio");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao formaEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao>("FormaEnvioDocumentacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao modalEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao>("ModalEnvio");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(codigosCTes);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDestinoPorto = repCTe.BuscarPorCodigoEDestinoPorto(codigosCTes);

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote();

                double provedorOS = Request.GetDoubleParam("ProvedorOS");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

                envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando;
                envioDocumentacaoLote.Container = repContainer.BuscarPorCodigo(Request.GetIntParam("Container"));
                envioDocumentacaoLote.DataGeracao = DateTime.Now;
                envioDocumentacaoLote.EmailInformadoManualmente = Request.GetStringParam("EmailEnvio");
                envioDocumentacaoLote.FoiAnulado = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiAnulado");
                envioDocumentacaoLote.FoiSubstituido = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiSubstituido");
                envioDocumentacaoLote.FormaEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao>("FormaEnvioDocumentacao");
                envioDocumentacaoLote.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(Request.GetIntParam("GrupoPessoa"));
                envioDocumentacaoLote.ModalEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao>("ModalEnvio");
                envioDocumentacaoLote.NumeroBooking = Request.GetStringParam("NumeroBooking");
                envioDocumentacaoLote.NumeroFiscal = Request.GetIntParam("NumeroFiscal");
                envioDocumentacaoLote.NumeroOS = Request.GetStringParam("NumeroOS");
                envioDocumentacaoLote.NumeroControle = Request.GetStringParam("NumeroControle");
                envioDocumentacaoLote.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(Request.GetIntParam("PedidoViagemDirecao"));
                envioDocumentacaoLote.ProvedorOS = provedorOS > 0 ? repCliente.BuscarPorCPFCNPJ(provedorOS) : null;
                envioDocumentacaoLote.SituacaoEnvioDocumentacao = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);
                envioDocumentacaoLote.TerminalDestino = repTerminal.BuscarPorCodigo(Request.GetIntParam("TerminalDestino"));
                envioDocumentacaoLote.TerminalOrigem = repTerminal.BuscarPorCodigo(Request.GetIntParam("TerminalOrigem"));
                envioDocumentacaoLote.Usuario = this.Usuario;
                envioDocumentacaoLote.TipoImpressaoLote = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoLote.Email;

                envioDocumentacaoLote.TiposProposta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                if (tipoPropostaMultimodal != null && tipoPropostaMultimodal.Count > 0)
                {
                    foreach (var tipoProposta in tipoPropostaMultimodal)
                    {
                        envioDocumentacaoLote.TiposProposta.Add(tipoProposta);
                    }
                }
                if (tiposPropostasMultimodal != null && tiposPropostasMultimodal.Count > 0)
                {
                    foreach (var tipoProposta in tiposPropostasMultimodal)
                    {
                        envioDocumentacaoLote.TiposProposta.Add(tipoProposta);
                    }
                }
                envioDocumentacaoLote.TiposServicos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>();
                if (tipoServico != null && tipoServico.Count > 0)
                {
                    foreach (var tipo in tipoServico)
                    {
                        envioDocumentacaoLote.TiposServicos.Add(tipo);
                    }
                }

                envioDocumentacaoLote.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (ctes != null && ctes.Count > 0)
                {
                    foreach (var cte in ctes)
                    {
                        envioDocumentacaoLote.CTes.Add(cte);
                    }
                }

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                return new JsonpResult(true, "Sucesso");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a impressão em lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> AnexoEmail(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string guid = Request.GetStringParam("guid").Split(',').FirstOrDefault();
                string anexo = Request.GetStringParam("Anexo").Split(',').FirstOrDefault();

                string caminhoRelatoriosEmbarcador = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
                string caminhoArquivosAnexos = Utilidades.Directory.CriarCaminhoArquivos([Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido"]);
                if (string.IsNullOrWhiteSpace(caminhoRelatoriosEmbarcador))
                    return new ContentResult() { Content = "Caminho não configurado!" };

                string nomeArquivo = guid;
                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, nomeArquivo + "." + anexo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                {
                    string nomeArquivoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivosAnexos, $"{nomeArquivo}{anexo}");
                    if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivoAnexo))
                        caminhoArquivo = nomeArquivoAnexo;
                    else
                    {
                        caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, nomeArquivo + anexo);
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                            return new ContentResult() { Content = "Arquivo ainda não disponível para download, tente novamente mais tarde!" };
                    }
                }

                byte[] arquivo = await Utilidades.IO.FileStorageService.Storage.ReadAllBytesAsync(caminhoArquivo, cancellationToken);
                if (arquivo == null)
                    return new ContentResult() { Content = "Arquivo dda documentação inválido!" };

                return Arquivo(arquivo, "application/" + anexo, nomeArquivo + "." + anexo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new ContentResult() { Content = "Ocorreu uma falha ao baixar a documentação solicitada." };
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

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
            double provedorOS = Request.GetDoubleParam("ProvedorOS");

            string numeroBooking = Request.GetStringParam("NumeroBooking");
            string numeroOS = Request.GetStringParam("NumeroOS");
            string numeroControle = Request.GetStringParam("NumeroControle");

            int numeroFiscal = Request.GetIntParam("NumeroFiscal");

            int codigoContainer = Request.GetIntParam("Container");
            int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
            int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
            int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");
            int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");

            Dominio.Enumeradores.OpcaoSimNaoPesquisa foiAnulado = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiAnulado");
            Dominio.Enumeradores.OpcaoSimNaoPesquisa foiSubstituido = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FoiSubstituido");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao modalEnvioDocumentacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao>("ModalEnvio");
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal>("TipoServico");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao situacaoEnvio = Request.GetEnumParam("SituacaoEnvioDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Todos);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            List<int> codigosCTesTransbordo = new List<int>();
            List<int> codigosCTes = new List<int>();
            codigosCTesTransbordo = repCarga.ConsultarCodigosTransbordoConhecimentosParaImpressaoLote(modalEnvioDocumentacao, foiAnulado, foiSubstituido, tipoServico, situacaoEnvio, provedorOS, numeroBooking, numeroOS, numeroControle, numeroFiscal, codigoContainer, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, codigoGrupoPessoa, tiposPropostasMultimodal, tipoPropostaMultimodal);
            codigosCTes = repCarga.ConsultarCodigosConhecimentosParaImpressaoLote(modalEnvioDocumentacao, foiAnulado, foiSubstituido, tipoServico, situacaoEnvio, provedorOS, numeroBooking, numeroOS, numeroControle, numeroFiscal, codigoContainer, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, codigoGrupoPessoa, tiposPropostasMultimodal, tipoPropostaMultimodal);

            codigosCTes.AddRange(codigosCTesTransbordo);

            return codigosCTes.Distinct().ToList();

        }
    }
}
