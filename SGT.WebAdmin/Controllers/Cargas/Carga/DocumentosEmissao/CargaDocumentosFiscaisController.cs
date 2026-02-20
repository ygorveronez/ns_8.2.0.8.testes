using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "Logistica/AgendamentoColeta")]
    public class CargaDocumentosFiscaisController : BaseController
    {
        #region Construtores

        public CargaDocumentosFiscaisController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Propriedades Globais

        public static bool ThreadExecutada = false;
        public static bool Sucesso = false;
        public static Dominio.Entidades.LayoutEDI LayoutEDI = null;
        public static Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis NOTFIS = null;
        public static Stream ArquivoEDI = null;

        #endregion

        #region Metodos Globais


        public async Task<IActionResult> VerificarDivergenciaProdutosEsperadoVSRecebidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                int codigoCarga = int.Parse(Request.Params("Carga"));

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.ProdutoDivergente> produtosDivergentes = servicoCarga.VerificarDivergenciaProdutosEsperadoVSRecebidos(codigoCarga);

                return new JsonpResult(produtosDivergentes);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar produtos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarEnvioDosDocumentosFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoEntrega = Request.GetIntParam("Entrega");
                bool naoValidarApolice = Request.GetBoolParam("NaoValidarApolice");
                bool liberouProdutosDivergentes = Request.GetBoolParam("LiberouProdutosDivergentes");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null && codigoEntrega > 0)
                    carga = repositorioCargaEntrega.BuscarPorCodigo(codigoEntrega).Carga;

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                unitOfWork.Start();

                carga.AvancouCargaEtapaDocumentoLote = false;

                var resultado = servicoCarga.ConfirmarEnvioDosDocumentos(carga, naoValidarApolice, liberouProdutosDivergentes, TipoServicoMultisoftware, permissoesPersonalizadas, Auditado, WebServiceConsultaCTe, Usuario, unitOfWork);

                repositorioCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(resultado);
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
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o envio dos documentos fiscais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarCargaDocOSMaeOS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int buscarDocOSMae = Request.GetIntParam("DocOSMae");
                int codigoCarga = Request.GetIntParam("Carga");

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Servicos.Embarcador.Carga.CargaDadosAverbacao servicoCargaDadosAverbacao = new Servicos.Embarcador.Carga.CargaDadosAverbacao(unitOfWork);
                Servicos.Embarcador.CTe.CTe servicoCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                Servicos.Embarcador.Carga.CTeSubContratacao servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);


                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possivel salvar a carga.");

                Dominio.Entidades.Embarcador.Cargas.Carga cargaOS = repositorioCarga.BuscarPorCodigo(buscarDocOSMae);

                if (cargaOS == null)
                    return new JsonpResult(false, true, "Não foi possivel salvar a carga.");

                if (!servicoCargaPedido.VerificarSeCargasPossuemMesmaQuantidadeDePedidos(new List<int>() { carga.Codigo, cargaOS.Codigo }, unitOfWork))
                    return new JsonpResult(false, true, "Não foi possivel salvar a carga, pois as cargas devem ter a mesma quantidade de pedidos.");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

                bool enviarCTeApenasParaTomador = configuracaoGeral.EnviarCTeApenasParaTomador;

                List<string> chavesCte = repositorioCargaCte.BuscarNumerosCtePorCarga(cargaOS.Codigo);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repositorioCargaCte.BuscarCTePorCarga(cargaOS.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao> dadosAverbacaos = servicoCargaDadosAverbacao.BuscarDadosAverbacao(chavesCte, enviarCTeApenasParaTomador);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorUnicaCarga(carga.Codigo);

                unitOfWork.Start();

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe objetoCte = servicoCte.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork);
                    servicoCTeSubContratacao.VincularCTeTerceiroACargaPedido(objetoCte, cargaPedido, unitOfWork, TipoServicoMultisoftware);
                }

                foreach (var dadoAverbacao in dadosAverbacaos)
                    servicoCargaDadosAverbacao.SalvarDadosAverbacaoCarga(dadoAverbacao, carga, unitOfWork);

                carga.CargaDocOS = cargaOS;
                repositorioCarga.Atualizar(carga);

                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o envio dos documentos fiscais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarArquivoParaEmissao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                bool alteradoTipoDeCarga = false;
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaValidacao = repositorioCarga.BuscarPorCargaPedido(codCargaPedido);
                if (cargaValidacao?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, true, "A carga não pode ser editada.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                List<TipoServicoMultisoftware> tipoServicosPermitidos = new List<TipoServicoMultisoftware>()
                {
                   TipoServicoMultisoftware.MultiEmbarcador,
                   TipoServicoMultisoftware.Fornecedor
                };

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && !Usuario.UsuarioAdministrador && !tipoServicosPermitidos.Contains(TipoServicoMultisoftware))
                {
                    if (this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || (!(cargaValidacao?.TipoOperacao?.PermitirTransportadorEnviarNotasFiscais ?? false)))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                unitOfWork.Start();

                string rotas = Request.Params("Rotas");
                ClassificacaoNFe? classificacaoNFe = Request.GetNullableEnumParam<ClassificacaoNFe>("ClassificacaoNFe");

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAnexo repositorioAnexo = new Repositorio.Embarcador.Pedidos.PedidoAnexo(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();

                if (cargaPedido != null)
                {
                    bool etapaNfeForcada = false;

                    if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValidarRelevanciaNotasPrechekin ?? false) && (cargaPedido.StageRelevanteCusto == null))
                        return new JsonpResult(false, "Não é permitido checkin das notas para essa etapa pois a mesma é irrelevante");

                    if (cargaPedido.Carga.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ObrigarInformarCTePortalFornecedor ?? false)
                    {
                        Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                        Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork, TipoServicoMultisoftware);
                        Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaPedido.Carga.Codigo);
                        etapaNfeForcada = servicoAgendamentoColeta.IsForcarEtapaNFe(agendamentoColeta);
                    }

                    if ((cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe) || cargaPedido.Carga.CargaEmitidaParcialmente || etapaNfeForcada)
                    {
                        List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                        if (files.Count > 0)
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                Servicos.DTO.CustomFile file = files[i];
                                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                                string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                                RetornoArquivo retornoArquivo = new RetornoArquivo();
                                retornoArquivo.nome = file.FileName;
                                retornoArquivo.processada = true;
                                retornoArquivo.mensagem = "";

                                if ((ConfiguracaoEmbarcador.ExibirClassificacaoNFe || (cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false)) && (!classificacaoNFe.HasValue || classificacaoNFe == ClassificacaoNFe.SemClassificacao))
                                {
                                    retornoArquivo.processada = false;
                                    retornoArquivo.mensagem = "É necessário informar a Classificação NFe antes de enviar o arquivo.";
                                    retornoArquivos.Add(retornoArquivo);
                                    continue;
                                }

                                if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.BloquearInclusaoArquivosXMLDeNFeCarga ?? false) && extensao.Equals(".xml"))
                                {
                                    retornoArquivo.processada = false;
                                    retornoArquivo.mensagem = "Tipo de operação não permite inclusão de arquivos XML de NFe na carga";
                                    retornoArquivos.Add(retornoArquivo);
                                    continue;
                                }

                                //TODO: refazer aqui, encontrar uma melhor forma de intenfificar qual é o arquivo que esta sendo enviado
                                bool arquivoEDI = false;
                                if (extensao.Equals(".txt") || extensao.Equals(".pal") || extensao.Equals(".edi") || extensao.Equals(".nnn") || extensao.Equals(".sao"))
                                {
                                    Dominio.Entidades.LayoutEDI layoutEDI = null;
                                    if (cargaPedido.Carga.TipoOperacao != null)
                                        layoutEDI = (from obj in cargaPedido.Carga.TipoOperacao.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();

                                    if (layoutEDI == null && cargaPedido.Pedido.Remetente == null && cargaPedido.Pedido.GrupoPessoas != null)
                                        layoutEDI = (from obj in cargaPedido.Pedido.GrupoPessoas.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();

                                    if (layoutEDI == null && cargaPedido.Pedido.Remetente != null)
                                        layoutEDI = (from obj in cargaPedido.Pedido.Remetente.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();

                                    if (layoutEDI == null && cargaPedido.Pedido.Remetente != null)
                                    {
                                        if (cargaPedido.Pedido.Remetente.GrupoPessoas != null)
                                            layoutEDI = (from obj in cargaPedido.Pedido.Remetente.GrupoPessoas.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();
                                    }

                                    if (layoutEDI != null)
                                    {
                                        AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                                        try
                                        {
                                            string retorono = ProcessarEDI(cargaPedido, unitOfWork, adminUnitOfWork, file.InputStream, layoutEDI, rotas, TipoServicoMultisoftware, out alteradoTipoDeCarga);
                                            if (!string.IsNullOrWhiteSpace(retorono))
                                            {
                                                retornoArquivo.processada = false;
                                                retornoArquivo.mensagem = retorono;
                                                serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, true, retorono, unitOfWork);
                                            }
                                            else
                                                serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, false, "", unitOfWork);
                                            arquivoEDI = true;
                                        }
                                        catch
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            adminUnitOfWork.Dispose();
                                        }

                                    }
                                }
                                else if (extensao == ".csv" || extensao == ".xls" || extensao == ".xlsx")
                                {
                                    AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                                    Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacao = null;

                                    try
                                    {
                                        if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                                            arquivoImportacao = cargaPedido.Carga.TipoOperacao.ArquivoImportacaoNotaFiscal;
                                        else if (cargaPedido.Pedido.Remetente != null && (cargaPedido.Pedido.Remetente.NaoUsarConfiguracaoEmissaoGrupo || cargaPedido.Pedido.Remetente.GrupoPessoas == null))
                                            arquivoImportacao = cargaPedido.Pedido.Remetente.ArquivoImportacaoNotaFiscal;
                                        else
                                        {
                                            if (cargaPedido.Pedido.Remetente != null)
                                                arquivoImportacao = cargaPedido.Pedido.Remetente.GrupoPessoas.ArquivoImportacaoNotaFiscal;
                                        }

                                        if (arquivoImportacao != null)
                                        {
                                            string retorno = ProcessarPlanilha(cargaPedido, file.InputStream, arquivoImportacao, unitOfWork, adminUnitOfWork, extensao);
                                            if (!string.IsNullOrWhiteSpace(retorno))
                                            {
                                                retornoArquivo.processada = false;
                                                retornoArquivo.mensagem = retorno;
                                            }
                                            arquivoEDI = true;
                                        }
                                    }
                                    catch
                                    {
                                        throw;
                                    }
                                    finally
                                    {
                                        adminUnitOfWork.Dispose();
                                    }
                                }

                                if (!arquivoEDI)
                                {
                                    if (extensao.Equals(".pdf"))
                                    {
                                        AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                                        try
                                        {
                                            string caminho = ObterCaminhoArquivos(unitOfWork);
                                            string extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();
                                            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                                            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}");

                                            file.SaveAs(caminhoArquivo);

                                            byte[] fileData = null;
                                            using (var binaryReader = new BinaryReader(file.InputStream))
                                            {
                                                fileData = binaryReader.ReadBytes((int)file.Length);
                                            }

                                            string retorno = ProcessarPDF(cargaPedido, fileData, unitOfWork, adminUnitOfWork, extensao);
                                            if (!string.IsNullOrWhiteSpace(retorno))
                                            {
                                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                                                    Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);

                                                retornoArquivo.processada = false;
                                                retornoArquivo.mensagem = retorno;
                                            }
                                            else
                                            {
                                                Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo pedidoAnexo = new Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo()
                                                {
                                                    Descricao = "NFe absorvida",
                                                    EntidadeAnexo = cargaPedido.Pedido,
                                                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(file.FileName))),
                                                    GuidArquivo = guidArquivo
                                                };
                                                await repositorioAnexo.InserirAsync(pedidoAnexo, Auditado);
                                            }
                                            arquivoEDI = false;
                                        }
                                        catch
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            adminUnitOfWork.Dispose();
                                        }
                                    }
                                    else if (extensao.Equals(".xml") || extensao.Equals(".txt") || extensao.Equals(".pal") || extensao.Equals(".edi") || extensao.Equals(".nnn") || extensao.Equals(".sao"))
                                    {
                                        try
                                        {
                                            var objetoNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(file.InputStream);
                                            if (objetoNFe != null)
                                            {
                                                if (cargaPedido.CTeEmitidoNoEmbarcador)
                                                {
                                                    retornoArquivo.processada = false;
                                                    retornoArquivo.mensagem = "Não é possível importar uma NF-e em uma carga onde os CT-es são emitidos pelo embarcador.";
                                                }
                                                else
                                                {
                                                    bool msgAlertaObservacao = false;
                                                    string retorno = ProcessarXMLNFe(file.InputStream, cargaPedido, unitOfWork, out msgAlertaObservacao, 0, classificacaoNFe);
                                                    if (!string.IsNullOrEmpty(retorno) && !msgAlertaObservacao)
                                                    {
                                                        retornoArquivo.processada = false;
                                                        retornoArquivo.mensagem = retorno;
                                                        serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, true, retorno, unitOfWork);
                                                    }
                                                    else if (msgAlertaObservacao)
                                                    {
                                                        retornoArquivo.processada = true;
                                                        retornoArquivo.mensagem = retorno;
                                                        serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, false, "", unitOfWork);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                                                if (objCTe != null)
                                                {
                                                    string retorno = ProcessarXMLCTe(objCTe, file.InputStream, cargaPedido, unitOfWork);
                                                    if (!string.IsNullOrEmpty(retorno))
                                                    {
                                                        retornoArquivo.processada = false;
                                                        retornoArquivo.mensagem = retorno;
                                                        serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, true, retorno, unitOfWork);
                                                    }
                                                    else
                                                        serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, false, "", unitOfWork);
                                                }
                                                else
                                                {
                                                    var objMDFe = MultiSoftware.MDFe.Servicos.Leitura.Ler(file.InputStream);

                                                    if (objMDFe != null)
                                                    {
                                                        string mensagemRetorno = await ProcessarXMLMDFeAsync(objMDFe, file.InputStream, cargaPedido, unitOfWork, cancellationToken);

                                                        if (!string.IsNullOrEmpty(mensagemRetorno))
                                                        {
                                                            retornoArquivo.processada = false;
                                                            retornoArquivo.mensagem = mensagemRetorno;
                                                        }
                                                    }
                                                    else if (repCargaIntegracao.ExistePorCargaETipo(cargaPedido.Carga.Codigo, TipoIntegracao.Intercement))
                                                    {
                                                        var objIntercement = Servicos.Embarcador.Integracao.Intercement.IntegracaoIntercement.Ler(file.InputStream);
                                                        if (objIntercement != null)
                                                        {
                                                            string retorno = Servicos.Embarcador.Integracao.Intercement.IntegracaoIntercement.ProcessarXMLIntercement(objIntercement, cargaPedido, unitOfWork, Auditado, ConfiguracaoEmbarcador, _conexao.StringConexao, TipoServicoMultisoftware);
                                                            if (!string.IsNullOrEmpty(retorno))
                                                            {
                                                                retornoArquivo.processada = false;
                                                                retornoArquivo.mensagem = retorno;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            retornoArquivo.processada = false;
                                                            retornoArquivo.mensagem = "O xml informado não é um espelho da Intercement, por favor verifique.";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        retornoArquivo.processada = false;
                                                        retornoArquivo.mensagem = "O xml informado não é uma NF-e, um CT-e ou um MDF-e, por favor verifique.";
                                                    }
                                                }
                                            }
                                        }
                                        catch (ServicoException ex)
                                        {
                                            unitOfWork.Rollback();
                                            retornoArquivo.processada = false;
                                            retornoArquivo.mensagem = ex.Message;
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro(ex);
                                            unitOfWork.Rollback();
                                            retornoArquivo.processada = false;
                                            retornoArquivo.mensagem = "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido.";
                                        }
                                        finally
                                        {
                                            file.InputStream.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        retornoArquivo.processada = false;
                                        retornoArquivo.mensagem = "A extensão do arquivo é inválida.";
                                    }
                                }

                                retornoArquivos.Add(retornoArquivo);

                                if (retornoArquivo.processada)
                                {
                                    if (cargaPedido.PedidoSemNFe)
                                    {
                                        cargaPedido.PedidoSemNFe = false;
                                        repCargaPedido.Atualizar(cargaPedido);
                                    }
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, $"Enviou o arquivo {nomeArquivo} para compor os documentos.", unitOfWork);
                                }
                            }

                            unitOfWork.CommitChanges();
                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                            if (cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null)
                                tipoOperacao = repTipoOperacao.BuscarPorCodigo(cargaPedido.Carga.TipoOperacao.Codigo);

                            if (alteradoTipoDeCarga || tipoContratacao != cargaPedido.TipoContratacaoCarga || (tipoOperacao != null && tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes))
                            {
                                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                                serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);

                                var dadosRetorno = new
                                {
                                    DetalhesCarga = serCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork),
                                    Arquivos = retornoArquivos
                                };
                                return new JsonpResult(dadosRetorno);
                            }
                            else
                            {
                                var dadosRetorno = new
                                {
                                    Arquivos = retornoArquivos
                                };
                                return new JsonpResult(dadosRetorno);
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite o envio de notas fiscais");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Pedido não encontrado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarEnvioDocumentosFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool bloquear = false;
                bool possuiPendencia = false;
                bool limiteDeTaraAtingido = false;
                bool limiteDePesoAtingido = false;
                bool limiteDeValorAtingido = false;
                bool possuiPedidoDevolucaoPacoteSemPacote = false;

                string mensagem = string.Empty;

                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                decimal limitePesoDocumentoCarga = configuracaoGeralCarga?.LimitePesoDocumentoCarga ?? 0m;
                decimal limiteValorDocumentoCarga = configuracaoGeralCarga?.LimiteValorDocumentoCarga ?? 0m;
                decimal limiteTaraPedidosCarga = configuracaoGeralCarga?.LimiteTaraPedidosCarga ?? 0m;

                if (ConfiguracaoEmbarcador.AvisarMDFeEmitidoEmbarcadorSemSeguroValido)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);

                    if (repCargaPedido.ExisteCTeEmitidoNoEmbarcador(codigoCarga) && repCargaPedidoDocumentoMDFe.ExisteMDFeSemAverbacaoValidaPorCarga(codigoCarga))
                    {
                        possuiPendencia = true;
                        mensagem = "Existem MDF-es sem averbação válida para esta carga.";
                    }
                }

                if (limitePesoDocumentoCarga > 0)
                {
                    decimal pesoDocumentosCarga = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(codigoCarga);
                    if (pesoDocumentosCarga > limitePesoDocumentoCarga && pesoDocumentosCarga > 0)
                    {
                        mensagem = "O total dos pesos das notas (" + pesoDocumentosCarga.ToString("n3") + ") excede o limite configurado (" + limitePesoDocumentoCarga.ToString("n3") + ").";
                        possuiPendencia = true;
                        limiteDePesoAtingido = true;
                    }
                }

                if (limiteValorDocumentoCarga > 0)
                {
                    decimal valorDocumentosCarga = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(codigoCarga);
                    if (valorDocumentosCarga > limiteValorDocumentoCarga && valorDocumentosCarga > 0)
                    {
                        mensagem = "O total dos valores das notas (" + valorDocumentosCarga.ToString("n2") + ") excede o limite configurado (" + limiteValorDocumentoCarga.ToString("n2") + ").";
                        possuiPendencia = true;
                        limiteDeValorAtingido = true;
                    }
                }

                if ((ConfiguracaoEmbarcador.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada || limiteTaraPedidosCarga > 0) && string.IsNullOrWhiteSpace(mensagem))
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);
                    foreach (var cargaPedido in cargaPedidos)
                    {
                        if (limiteTaraPedidosCarga > 0)
                        {
                            decimal taraPedido = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.TaraContainer) ? cargaPedido.Pedido.TaraContainer.Replace(".", "").ToDecimal() : 0m;
                            if (taraPedido > 0 && taraPedido > limiteTaraPedidosCarga)
                            {
                                mensagem = "A tara do pedido Nº " + cargaPedido.Pedido.Numero.ToString("D") + " - CT.: " + (cargaPedido.Pedido.Container?.Numero ?? "") + " (" + cargaPedido.Pedido.TaraContainer.Replace(".", "").ToDecimal().ToString("n2") + ") excede o limite configurado (" + limiteTaraPedidosCarga.ToString("n2") + ").";
                                limiteDeTaraAtingido = true;
                                possuiPendencia = true;
                            }
                        }

                        if (ConfiguracaoEmbarcador.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada && !(cargaPedido.Carga.TipoOperacao?.NaoValidarNotaFiscalExistente ?? false))
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo, true);
                            foreach (var notaFiscal in notasFiscais)
                            {
                                if (notaFiscal.XMLNotaFiscal != null)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;
                                    if (notaFiscal.XMLNotaFiscal.Modelo == "55" && !cargaPedido.CTeEmitidoNoEmbarcador)
                                        pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtiva(notaFiscal.XMLNotaFiscal.Chave, cargaPedido.Codigo);
                                    if (pedidoXMLNotaFiscal != null)
                                    {
                                        if (pedidoXMLNotaFiscal.CargaPedido != null && pedidoXMLNotaFiscal.CargaPedido.Carga != null && pedidoXMLNotaFiscal.CargaPedido.Carga.Codigo != cargaPedido.Carga.Codigo)
                                        {
                                            possuiPendencia = true;
                                            mensagem += $"Já existe uma NF-e com esta chave ({notaFiscal.XMLNotaFiscal.Chave}) e número ({notaFiscal.XMLNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}.</br>";
                                            notaFiscal.NotaFiscalEmOutraCarga = true;
                                        }
                                        else
                                            notaFiscal.NotaFiscalEmOutraCarga = false;
                                    }
                                    else
                                        notaFiscal.NotaFiscalEmOutraCarga = false;

                                    repPedidoXMLNotaFiscal.Atualizar(notaFiscal);
                                }
                            }
                        }
                    }
                }

                bool existePacote = repPacote.ExistePacote();
                if (existePacote)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosSemPacote = repositorioCargaPedido.BuscarPedidosDevolucaoPacotesSemPacote(codigoCarga);

                    if (pedidosSemPacote != null && pedidosSemPacote.Count > 0)
                    {
                        possuiPendencia = true;
                        possuiPedidoDevolucaoPacoteSemPacote = true;
                        mensagem = $"Existem {pedidosSemPacote.Count} pedidos de Devolução de Pacotes, sem pacotes.";
                    }
                }

                return new JsonpResult(new
                {
                    Bloquear = bloquear,
                    PossuiPendencia = possuiPendencia,
                    Mensagem = mensagem,
                    LimiteDeTaraAtingido = limiteDeTaraAtingido,
                    LimiteDePesoAtingido = limiteDePesoAtingido,
                    LimiteDeValorAtingido = limiteDeValorAtingido,
                    PossuiPedidoDevolucaoPacoteSemPacote = possuiPedidoDevolucaoPacoteSemPacote
                });
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o envio dos documentos fiscais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarAuditoriaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                bool limiteDeTaraAtingido = Request.GetBoolParam("LimiteDeTaraAtingido");
                bool limiteDePesoAtingido = Request.GetBoolParam("LimiteDePesoAtingido");
                bool limiteDeValorAtingido = Request.GetBoolParam("LimiteDeValorAtingido");
                bool pedidosDevolucaoPacoteSemPacote = Request.GetBoolParam("PedidosDevolucaoPacotesSemPacote");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (limiteDeTaraAtingido)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Operador autorizou o limite da tara atingido.", unitOfWork);
                if (limiteDePesoAtingido)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Operador autorizou o limite de peso atingido.", unitOfWork);
                if (limiteDeValorAtingido)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Operador autorizou o limite de valor atingido.", unitOfWork);
                if (pedidosDevolucaoPacoteSemPacote)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Operador  liberou a carga, com pedidos de devolução de pacotes, sem pacotes.", unitOfWork);

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a auditoria da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDocumentosDestinados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe ||
                    cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A situação da carga não permite que sejam adicionadas notas fiscais à mesma.");

                if (cargaPedido.CTeEmitidoNoEmbarcador)
                    return new JsonpResult(false, true, "Não é possível adicionar notas fiscais em uma carga onde os CT-es são emitidos pelo embarcador.");

                if (cargaPedido.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é possível adicionar notas fiscais em uma carga de transbordo.");

                if (cargaPedido.Carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, true, "A carga não pode ser editada.");

                List<long> codigos = Request.GetListParam<long>("Documentos");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.BuscarPorCodigos(0, codigos);

                foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado in documentosDestinados)
                {
                    string caminhoXML = Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterCaminhoDocumentoDestinado(documentoDestinado, unitOfWork);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                        return new JsonpResult(false, true, $"O XML da nota fiscal {documentoDestinado.Numero} [{documentoDestinado.Chave}] não foi encontrado, não sendo possível adicioná-lo na carga. Faça o download da mesma e realize o envio manualmente.");

                    using (System.IO.Stream streamXML = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoXML))
                    {
                        var objetoNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(streamXML);

                        if (objetoNFe == null)
                            return new JsonpResult(false, true, $"Não foi possível ler o XML da nota fiscal {documentoDestinado.Numero} [{documentoDestinado.Chave}]. Faça o download da mesma e realize o envio manualmente.");

                        unitOfWork.Start();

                        string retorno = ProcessarXMLNFe(streamXML, cargaPedido, unitOfWork, out bool msgAlertaObservacao);

                        if (!string.IsNullOrEmpty(retorno))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, retorno);
                        }

                        unitOfWork.CommitChanges();
                    }
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o envio dos documentos fiscais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterToleranciaParaAvancoEtapaCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                bool podeAvancarAutomaticamente = true;
                List<Dominio.ObjetosDeValor.Embarcador.Carga.QuantidadePacotesCargaPorCargaPedido> quantidadePacotesCargaPorCargaPedidos = await repositorioCargaPedidoPacote.BuscarQuantidadePacoteCargaPorCargaPedidoAsync(carga.Codigo, cancellationToken);
                foreach (var item in quantidadePacotesCargaPorCargaPedidos)
                    if (item.PERCENTUAL <= carga.TipoOperacao?.ConfiguracaoCarga?.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes)
                        podeAvancarAutomaticamente = false;

                if (carga.ProcessandoImportacaoPacote ?? false)
                    podeAvancarAutomaticamente = false;

                var retorno = new
                {
                    PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes = carga?.TipoOperacao?.ConfiguracaoCarga?.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes ?? 0,
                    Pacotes = quantidadePacotesCargaPorCargaPedidos.Where(x => x.CPE_CODIGO == codigoCargaPedido).Sum(x => x.QTD_PACOTES),
                    CTesAnteriores = quantidadePacotesCargaPorCargaPedidos.Where(x => x.CPE_CODIGO == codigoCargaPedido).Sum(x => x.QTD_CTES_ANTERIORES),
                    VerificarCorVerdeOuVermelho = podeAvancarAutomaticamente,
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> VerificarIncosistenciaRegraPlanejamentoFrota()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            var serRegraPlanejamentoFrota = new Servicos.Embarcador.Logistica.RegraPlanejamentoFrota(unitOfWork);
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            int codigoCarga = int.Parse(Request.Params("Carga"));
            string erroMensagem = "";
            bool sucesso = true;
            bool usuarioComPermissao = false;

            var carga = repCarga.BuscarPorCodigo(codigoCarga);

            var permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

            var retornoRegraPlanejamento = serRegraPlanejamentoFrota.BuscarRegraPlanejamentoFrota(unitOfWork, DateTime.Now, codigoCarga);

            if (retornoRegraPlanejamento != null)
            {
                if (!serRegraPlanejamentoFrota.ValidarRegraPlanjamentoFrota(retornoRegraPlanejamento, unitOfWork, DateTime.Now, codigoCarga, out erroMensagem))
                {
                    if (Usuario == null || permissoesPersonalizadas == null || Auditado == null)
                        sucesso = false;
                    else
                    {
                        if (Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(PermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeicaoPlanejamentoFrota))
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Operador liberou carga com rejeição ao Planejamento de Frota: " + erroMensagem.Left(200), unitOfWork);
                            sucesso = false;
                            usuarioComPermissao = true;
                        }
                        else
                            sucesso = false;
                    }
                }
            }

            var retorno = new
            {
                Sucesso = sucesso,
                Mensagem = erroMensagem,
                UsuarioComPermissao = usuarioComPermissao
            };

            return new JsonpResult(retorno);

        }

        #endregion

        #region Metodos Privados

        private string ProcessarXMLNFe(System.IO.Stream xml, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, out bool msgAlertaObservacao, decimal valorFrete = 0m, ClassificacaoNFe? classificacaoNFe = null, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null)
        {
            msgAlertaObservacao = false;
            bool notaFiscalEmOutraCarga = false;
            string retorno = "";
            string xmlNotaProduto = string.Empty;
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeral.BuscarPrimeiroRegistro();

            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

            if (xmlNotaFiscal == null)
            {

                xml.Position = 0;
                System.IO.StreamReader stReaderXML = new StreamReader(xml);
                xmlNotaProduto = stReaderXML.ReadToEnd();
                if (!serNFe.BuscarDadosNotaFiscal(out string erro, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, false, TipoServicoMultisoftware, false, configuracao?.UtilizarValorFreteNota ?? false, cargaPedido, configuracaoGeralCarga, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                    return erro;
            }

            if (valorFrete > 0m)
                xmlNotaFiscal.ValorFrete = valorFrete;

            Servicos.WebService.Carga.Pedido serWSPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponente = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoParcialxml = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = null;
            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao && cargaPedido.Carga.TipoOperacao.TipoIntegracao != null)
                tipoIntegracaoCarga = cargaPedido.Carga.TipoOperacao.TipoIntegracao;
            else if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.TipoIntegracao != null)
                tipoIntegracaoCarga = cargaPedido.Carga.GrupoPessoaPrincipal.TipoIntegracao;
            else if (cargaPedido.ObterTomador()?.NaoUsarConfiguracaoEmissaoGrupo ?? false)
                tipoIntegracaoCarga = cargaPedido.ObterTomador()?.TipoIntegracao;

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarDadosPedidoParaNotasExterior && xmlNotaFiscal.Destinatario != null && xmlNotaFiscal.Destinatario.Tipo == "E")
            {
                switch (cargaPedido.Pedido.TipoPagamento)
                {
                    case Dominio.Enumeradores.TipoPagamento.Pago:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.A_Pagar:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.Outros:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                        break;
                    default:
                        break;
                }

                if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                {
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Remetente;
                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        serWSPedido.PreecherEnderecoPedidoPorCliente(enderecoOrigem, cargaPedido.Pedido.Remetente);
                        repPedidoEndereco.Inserir(enderecoOrigem);
                        cargaPedido.Pedido.EnderecoOrigem = enderecoOrigem;
                    }
                }
                else if (cargaPedido.Pedido.Destinatario != null)
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;
            }

            xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;

            retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);

            if (string.IsNullOrEmpty(retorno) || msgAlertaObservacao)
            {
                xmlNotaFiscal.SemCarga = false;
                xmlNotaFiscal.ClassificacaoNFe = classificacaoNFe;
                cargaPedido.IndicadorRemessaVenda = (classificacaoNFe == ClassificacaoNFe.Remessa || classificacaoNFe == ClassificacaoNFe.Venda);

                if (xmlNotaFiscal.Codigo == 0)
                {
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;

                    repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                }
                else
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                if (!string.IsNullOrEmpty(xmlNotaProduto))
                    serNFe.SalvarProdutosNota(xmlNotaProduto, xmlNotaFiscal, Auditado, TipoServicoMultisoftware, unitOfWork);

                if (tipoIntegracaoCarga != null && tipoIntegracaoCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement)
                {
                    if (!serCargaNotaFiscal.InformarComponentesOperacaoIntercement(out string msgErro, cargaPedido, xmlNotaFiscal))
                        return msgErro;
                    else
                        repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repPedidoXMLNotaFiscal.ContemNotaFiscalEmOutraCarga(cargaPedido.Carga.Codigo, xmlNotaFiscal.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement))
                {
                    repXMLNotaFiscalComponente.DeletarPorXMLNotaFiscal(xmlNotaFiscal.Codigo);
                    xmlNotaFiscal.ValorFrete = 0;
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                {
                    XDocument documentoXml = XDocument.Parse(xmlNotaFiscal.XML);
                    string observacaoNfe = (from XElement elemento in documentoXml.Descendants() where elemento.Name.LocalName == "infCpl" select elemento)?.FirstOrDefault()?.Value ?? "";

                    new Servicos.Embarcador.Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, observacaoNfe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, unitOfWork, xmlNotaFiscal, Auditado);
                }

                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                    repCarga.Atualizar(cargaPedido.Carga);

                repPedido.Atualizar(cargaPedido.Pedido);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial existeNotaParcialPorChaveNota = repCargaPedidoParcialxml.BuscarPorChaveNota(xmlNotaFiscal.Chave);
                if (existeNotaParcialPorChaveNota != null && existeNotaParcialPorChaveNota.XMLNotaFiscal == null)
                {
                    existeNotaParcialPorChaveNota.XMLNotaFiscal = xmlNotaFiscal;
                    existeNotaParcialPorChaveNota.CargaPedido = cargaPedido;
                    repCargaPedidoParcialxml.Atualizar(existeNotaParcialPorChaveNota);
                }

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, TipoNotaFiscal.Venda, ConfiguracaoEmbarcador, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, Auditado);

                new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado).AdicionarMovimentacaoPalletAutomatico(xmlNotaFiscal, cargaPedido);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, null, "Adicionado via upload de arquivo", unitOfWork);

                if (!ConfiguracaoEmbarcador.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                {
                    if (pedidoXMLNotaFiscal != null && pedidoXMLNotaFiscal.CargaPedido.Codigo != cargaPedido.Codigo)
                        retorno = $"A NF-e {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} já foi adicionada ao pedido {pedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador}.";
                }
            }

            return retorno;
        }

        private bool isVincularMultiModal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (cargaPedido != null && cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.TipoServicoMultimodal != null && cargaPedido.Carga.TipoOperacao.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                return true;
            return false;
        }

        private string ProcessarXMLCTe(dynamic objCTe, Stream xml, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            string expressaoRegularBooking = cargaPedido?.Carga?.TipoOperacao?.ExpressaoBooking ?? "";
            string expressaoRegularContainer = cargaPedido?.Carga?.TipoOperacao?.ExpressaoContainer ?? "";

            Type tipoCTe = objCTe.GetType();

            if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);


                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    return "Não é possível importar um CT-e de complemento.";

                if (string.IsNullOrWhiteSpace(cte.Xml))
                {
                    xml.Position = 0;
                    StreamReader reader = new StreamReader(xml);
                    cte.Xml = reader.ReadToEnd();
                }

                Dominio.Entidades.Empresa empresa = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);

                if (!cargaPedido.CTeEmitidoNoEmbarcador && (empresa == null || isVincularMultiModal(cargaPedido) || (cargaPedido?.Carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.ImportarCTeSempreComoSubcontratacao ?? false)))
                {
                    Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                    try
                    {
                        if (!unitOfWork.IsActiveTransaction())
                            unitOfWork.Start();
                        serCargaCteParaSubContratacao.VincularCTeTerceiroACargaPedido(cte, cargaPedido, unitOfWork, TipoServicoMultisoftware);
                        unitOfWork.CommitChanges();
                    }
                    catch (ServicoException exception)
                    {
                        unitOfWork.Rollback();
                        return exception.Message;
                    }
                }
                else
                {
                    if (empresa != null)
                    {
                        object cteRetorno = null;

                        unitOfWork.Start();

                        if (tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                            cteRetorno = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem);
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc))
                            cteRetorno = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc)objCTe, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem);
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                            cteRetorno = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem);
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                            cteRetorno = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)objCTe, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, false, false, TipoServicoMultisoftware);

                        if (cteRetorno.GetType() == typeof(string))
                        {
                            unitOfWork.Rollback();
                            retorno = (string)cteRetorno;
                        }
                        else if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;

                            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido != null && cargaPedido.Carga != null)
                                svcCTe.SalvarInformacoesMultiModal(cteConvertido, cargaPedido, cteConvertido.ValorAReceber, unitOfWork);

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                            {
                                CargaPedido = cargaPedido,
                                CTe = cteConvertido
                            };

                            cteConvertido.CTeSemCarga = false;

                            repCTe.Atualizar(cteConvertido);
                            repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasCTe = cteConvertido.TomadorPagador?.GrupoPessoas;

                            if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroPedidoEmbarcador) && !string.IsNullOrWhiteSpace(cte.NumeroPedido) && (grupoPessoasCTe?.LerNumeroPedidoObservacaoCTe ?? false) && (grupoPessoasCTe?.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe ?? false))
                            {
                                cargaPedido.Pedido.NumeroPedidoEmbarcador = cte.NumeroPedido;

                                repPedido.Atualizar(cargaPedido.Pedido);
                            }

                            if (cargaPedido.Carga.TipoOperacao?.CTeEmitidoNoEmbarcador ?? false)
                            {
                                cargaPedido.CTeEmitidoNoEmbarcador = true;
                                repCargaPedido.Atualizar(cargaPedido);
                            }


                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, $"Vinculou o CT-e {cteConvertido.Descricao} à carga.", unitOfWork);

                            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                            serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                        }
                        else
                        {
                            retorno = "Conhecimento de transporte inválido.";
                        }
                    }
                    else
                    {
                        retorno = "O CT-e informado não foi emitido por uma transportadora cadastrada.";
                    }
                }

                return retorno;
            }
            else if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteInutilizado.TProcInutCTe) && cargaPedido.CTeEmitidoNoEmbarcador)
            {
                object cteRetorno = svcCTe.GerarCTeAnteriorInutilizado(xml, cargaPedido, (MultiSoftware.CTe.v300.ConhecimentoDeTransporteInutilizado.TProcInutCTe)objCTe, unitOfWork);

                if (cteRetorno.GetType() == typeof(string))
                {
                    unitOfWork.Rollback();
                    retorno = (string)cteRetorno;
                }
                else if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;

                    cteConvertido.CTeSemCarga = false;

                    repCTe.Atualizar(cteConvertido);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                    {
                        CargaPedido = cargaPedido,
                        CTe = cteConvertido
                    };

                    repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, $"Vinculou o CT-e {cteConvertido.Descricao} à carga.", unitOfWork);
                }
                else
                {
                    retorno = "Conhecimento de transporte inválido.";
                }
            }
            else
            {
                retorno = "A versão do CT-e não é compativel, por favor, verique com a multisoftware";
            }
            return retorno;
        }

        private async Task<string> ProcessarXMLMDFeAsync(dynamic objMDFe, System.IO.Stream xml, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!cargaPedido.CTeEmitidoNoEmbarcador)
                return "A importação do MDF-e não é válida para um tomador que não está configurado para emitir o MDF-e.";

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork, cancellationToken);

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

            Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe = Servicos.Embarcador.MDFe.MDFe.ConverterProcMDFeParaMDFePorObjeto(objMDFe);

            Dominio.Entidades.Empresa empresa = null;

            if (mdfe != null)
            {
                empresa = await repositorioEmpresa.BuscarPorCNPJAsync(mdfe.Emitente.CNPJ);

                if (empresa == null)
                    return "O MDF-e informado não foi emitido por uma transportadora cadastrada.";
            }

            object mdfeRetorno = null;

            if (objMDFe.GetType() == typeof(MultiSoftware.MDFe.v100a.mdfeProc))
                mdfeRetorno = await servicoMDFe.GerarMDFeAnteriorAsync(empresa, (MultiSoftware.MDFe.v100a.mdfeProc)objMDFe, xml, false);
            else if (objMDFe.GetType() == typeof(MultiSoftware.MDFe.v300.mdfeProc))
                mdfeRetorno = await servicoMDFe.GerarMDFeAnteriorAsync(empresa, (MultiSoftware.MDFe.v300.mdfeProc)objMDFe, xml, false, cargaPedido);
            else if (objMDFe.GetType() == typeof(MultiSoftware.MDFe.v300.TProcEvento))
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork, cancellationToken);

                MultiSoftware.MDFe.v300.TProcEvento procEvento = (MultiSoftware.MDFe.v300.TProcEvento)objMDFe;

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeIntegrado = await repositorioMDFe.BuscarPorChaveAsync(procEvento.retEventoMDFe.infEvento.chMDFe);

                if (mdfeIntegrado != null)
                {
                    if (procEvento.retEventoMDFe.infEvento.cStat == "135")
                    {
                        if (procEvento.retEventoMDFe.infEvento.tpEvento == "110111") //cancelamento
                        {
                            string mensagem = await servicoMDFe.CancelarMDFeImportadoAsync(mdfeIntegrado, procEvento, xml, null, Auditado);

                            if (!string.IsNullOrWhiteSpace(mensagem))
                                mdfeRetorno = mensagem;
                            else
                                mdfeRetorno = "Cancelamento do MDF-e importado, porém, não é possível vincular um cancelamento à carga.";
                        }
                        else if (procEvento.retEventoMDFe.infEvento.tpEvento == "110112") //encerramento
                        {
                            string mensagem = await servicoMDFe.EncerrarMDFeImportadoAsync(mdfeIntegrado, procEvento, xml, null, Auditado);

                            if (!string.IsNullOrWhiteSpace(mensagem))
                                mdfeRetorno = mensagem;
                            else
                                mdfeRetorno = "Encerramento do MDF-e importado, porém, não é possível vincular um encerramento à carga.";
                        }
                    }
                }
            }

            if (mdfeRetorno is string)
                return (string)mdfeRetorno;


            else if (mdfeRetorno.GetType() == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais) || mdfeRetorno.GetType().BaseType == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais))
            {
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeConvertido = (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais)mdfeRetorno;

                await unitOfWork.StartAsync();

                string erro = string.Empty;

                if (!Servicos.Embarcador.MDFe.MDFeImportado.VincularMDFeACargaPedido(out erro, cargaPedido.Codigo, mdfeConvertido.Codigo, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                {
                    await unitOfWork.RollbackAsync();
                    return erro;
                }

                await unitOfWork.CommitChangesAsync();

                return string.Empty;
            }
            else
                return "MDF-e inválido.";
        }

        private string ProcessarEDI(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, System.IO.Stream stream, Dominio.Entidades.LayoutEDI layoutEDI, string rotas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out bool alteradoTipoDeCarga)
        {
            alteradoTipoDeCarga = false;

            if ((layoutEDI.ValidarRota || layoutEDI.ValidarNumeroReferenciaEDI) && string.IsNullOrWhiteSpace(rotas))
                return "É necessário informar a identificação das notas para processar este arquivo EDI.";

            stream.Position = 0;

            if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS)
            {
                MemoryStream streamEDI = new MemoryStream();
                stream.CopyTo(streamEDI);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(this.Empresa, layoutEDI, streamEDI, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = serLeituraEDI.GerarNotasFiscais();

                    if (notasFiscais.Count == 0)
                        return "Nenhuma Nota Fiscal importada.";

                    return ProcessarNotasFiscaisArquivo(cargaPedido, layoutEDI.ValidarRota, layoutEDI.ValidarNumeroReferenciaEDI, rotas, notasFiscais, unitOfWork, adminUnitOfWork, layoutEDI, out alteradoTipoDeCarga);
                }
                else
                {
                    System.Text.Encoding encoding = null;

                    if (!string.IsNullOrWhiteSpace(layoutEDI.Encoding))
                        encoding = Utilidades.Encoding.ObterEncoding(layoutEDI.Encoding);

                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(this.Empresa, layoutEDI, streamEDI, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, encoding, null);
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = serLeituraEDI.GerarNotasFiscais();

                    return ProcessarNotasFiscaisArquivo(cargaPedido, layoutEDI.ValidarRota, layoutEDI.ValidarNumeroReferenciaEDI, rotas, notasFiscais, unitOfWork, adminUnitOfWork, layoutEDI, out alteradoTipoDeCarga);
                }

            }
            else
            {
                ThreadExecutada = false;
                Sucesso = false;
                LayoutEDI = layoutEDI;
                ArquivoEDI = stream;

                int executionCount = 0;

                Thread thread = new Thread(GerarNOTFIS, 80000000);

                thread.Start();

                while (!ThreadExecutada)
                {
                    executionCount++;

                    if (executionCount == 20)
                    {
                        thread.Abort();
                        return "Ocorreu uma falha ao ler o NOTFIS. Tempo de execução muito longo.";
                    }

                    System.Threading.Thread.Sleep(500);

                    if (ThreadExecutada)
                    {
                        if (Sucesso)
                            return ProcessarNotasFiscaisNOTFIS(NOTFIS, cargaPedido, layoutEDI.ValidarRota, layoutEDI.ValidarNumeroReferenciaEDI, rotas, unitOfWork, adminUnitOfWork, layoutEDI);
                        else
                            return "Ocorreu uma falha ao ler o NOTFIS.";
                    }
                }

                return "Não foi possível ler o NOTFIS, tente novamente.";
            }
        }

        private static void GerarNOTFIS()
        {
            try
            {
                Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(null, LayoutEDI, ArquivoEDI, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));

                NOTFIS = serLeituraEDI.GerarNotasFis();

                Sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            ThreadExecutada = true;
        }

        private string ProcessarPlanilha(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, System.IO.Stream stream, Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacaoNotaFiscal, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, string extensao)
        {
            stream.Position = 0;
            MemoryStream streamArquivo = new MemoryStream();
            stream.CopyTo(streamArquivo);

            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.LerArquivo(extensao, streamArquivo, arquivoImportacaoNotaFiscal, unitOfWork);

            return ProcessarNotasFiscaisArquivo(cargaPedido, false, false, string.Empty, notasFiscais, unitOfWork, adminUnitOfWork, null, out bool alteradoTipoDeCarga);
        }

        private string ProcessarPDF(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, byte[] fileData, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, string extensao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.LerArquivoPDF(fileData, out string mensagemRetorno, unitOfWork);

            if (notasFiscais != null && notasFiscais.Count > 0 && string.IsNullOrWhiteSpace(mensagemRetorno))
                return ProcessarNotasFiscaisArquivo(cargaPedido, false, false, string.Empty, notasFiscais, unitOfWork, adminUnitOfWork, null, out bool alteradoTipoDeCarga);
            else
                return "Nenhuma nota fiscal encontrada no PDF. (" + mensagemRetorno + ")";
        }

        private string ProcessarNotasFiscaisArquivo(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool validarRotas, bool validarNumeroReferenciaEDI, string rotas, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.LayoutEDI layoutEDI, out bool alteradoTipoDeCarga)
        {
            alteradoTipoDeCarga = false;
            bool notaAceita = false;
            string mensagem = "";
            string[] idenNotas = rotas.Replace(" ", "").Split('/');

            AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotas repCargaPedidoRotas = new Repositorio.Embarcador.Cargas.CargaPedidoRotas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(_conexao.StringConexao);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa();

            bool separarNotasEDIPorPedido = configuracaoTMS.GerarPedidoImportacaoNotfisEtapaNFe;

            //Se não tiver notas ou não tem numero do pedido não sepera
            if (separarNotasEDIPorPedido && (notasFiscais.Count == 0 || string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().NumeroPedido)))
                separarNotasEDIPorPedido = false;

            List<string> identificaoesUsadas = new List<string>();

            if (layoutEDI?.AgruparNotasFiscaisDosCTesParaSubcontratacao ?? false)
            {
                ProcessarCTesSubcontratacaoNOTFIS(ref mensagem, ref notaAceita, idenNotas, notasFiscais, cargaPedido, unitOfWork, adminUnitOfWork, layoutEDI);
            }
            else
            {
                int notas = 0;

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscais)
                {
                    if ((!validarRotas || (validarRotas && idenNotas.Contains(notaFiscal.Rota.Replace(" ", "")))) &&
                        (!validarNumeroReferenciaEDI || (validarNumeroReferenciaEDI && idenNotas.Contains(notaFiscal.NumeroReferenciaEDI.Replace(" ", "")))))
                    {
                        if (separarNotasEDIPorPedido)
                        {
                            //Se pedido não tem nota ou o numero do pedido é o mesmo que da nota
                            if (notas == 0 && repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo) == 0)
                            {
                                cargaPedido.Pedido.NumeroPedidoEmbarcador = notaFiscal.NumeroPedido;
                                repPedido.Atualizar(cargaPedido.Pedido);
                            }
                            else
                            {
                                //Criar novo carga pedido
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNovo = new Dominio.Entidades.Embarcador.Cargas.CargaPedido();
                                Servicos.Embarcador.Carga.CargaPedido.Duplicar(out mensagem, out cargaPedidoNovo, cargaPedido, notaFiscal.NumeroPedido, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);
                                cargaPedido = cargaPedidoNovo;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(notaFiscal.Rota) && !identificaoesUsadas.Contains(notaFiscal.Rota.Replace(" ", "")))
                            identificaoesUsadas.Add(notaFiscal.Rota.Replace(" ", ""));

                        if (string.IsNullOrWhiteSpace(notaFiscal.Chave) && (layoutEDI?.BuscarNotaSemChaveDosDocumentosDestinados ?? false))
                        {
                            int.TryParse(notaFiscal.Serie, out int serie);

                            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa = repDocumentoDestinadoEmpresa.BuscarPorNumeroSerieEEmitente(notaFiscal.Numero, serie, notaFiscal.Emitente.CPFCNPJ);

                            if (documentoDestinadoEmpresa != null)
                            {
                                string caminhoXML = Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterCaminhoDocumentoDestinado(documentoDestinadoEmpresa, unitOfWork);

                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                                {
                                    System.IO.Stream streamNF = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoXML);

                                    string mensagemRetorno = ProcessarXMLNFe(streamNF, cargaPedido, unitOfWork, out bool msgAlertaObservacao, notaFiscal.ValorFrete);

                                    if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                                        mensagem = mensagemRetorno;
                                    else
                                        notaAceita = true;

                                    continue;
                                }
                            }
                        }

                        notaFiscal.Modelo = "55";
                        notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                        notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                        notaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);

                        if (layoutEDI != null && layoutEDI.UtilizarEmitenteDaChave)
                        {
                            notaFiscal.Emitente.CPFCNPJ = Utilidades.Chave.ObterCNPJEmitente(notaFiscal.Chave);

                            double cnpjEmitente = 0D;

                            double.TryParse(notaFiscal.Emitente.CPFCNPJ, out cnpjEmitente);

                            Dominio.Entidades.Cliente emitente = cnpjEmitente > 0D ? repCliente.BuscarPorCPFCNPJ(cnpjEmitente) : null;

                            if (emitente == null)
                                return "O emitente da nota fiscal " + notaFiscal.Chave + " não foi encontrado.";

                            notaFiscal.Emitente.AtualizarEnderecoPessoa = false;
                            notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                        }
                        else
                        {
                            if (notaFiscal.Emitente != null)
                            {
                                string[] splitEnderecorEmitente = notaFiscal.Emitente.Endereco.Logradouro.Split(',');
                                notaFiscal.Emitente.Endereco.Logradouro = splitEnderecorEmitente[0].Trim();

                                if (string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Logradouro) || notaFiscal.Emitente.Endereco.Logradouro.Length <= 2)
                                    notaFiscal.Emitente.Endereco.Logradouro = "NAO INFORMADO";

                                if (splitEnderecorEmitente.Length > 1)
                                {
                                    string[] splitNumero = splitEnderecorEmitente[1].Split('-');
                                    notaFiscal.Emitente.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");

                                    if (notaFiscal.Emitente.Endereco.Numero == "0")
                                        notaFiscal.Emitente.Endereco.Numero = "S/N";
                                    if (splitNumero.Count() > 1)
                                        notaFiscal.Emitente.Endereco.Complemento = splitNumero[1].Trim();
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Numero))
                                        notaFiscal.Emitente.Endereco.Numero = "S/N";
                                }

                                if (notaFiscal.Emitente.Endereco.Bairro == null || notaFiscal.Emitente.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Logradouro))
                                {
                                    if (!string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.CEP))
                                    {
                                        if (!adminUnitOfWork.IsOpenSession())
                                        {
                                            adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                                            repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                                        }

                                        AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(notaFiscal.Emitente.Endereco.CEP)).ToString());

                                        if (endereco != null)
                                            notaFiscal.Emitente.Endereco.Bairro = endereco.Bairro?.Descricao;

                                        if (string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Logradouro))
                                            notaFiscal.Emitente.Endereco.Logradouro = endereco.Logradouro;
                                    }
                                }

                                notaFiscal.Emitente.CPFCNPJ = Utilidades.String.OnlyNumbers(notaFiscal.Emitente.CPFCNPJ);
                                notaFiscal.Emitente.AtualizarEnderecoPessoa = false;

                                if (notaFiscal.Emitente.CPFCNPJ.Length >= 14)
                                {
                                    notaFiscal.Emitente.CPFCNPJ = notaFiscal.Emitente.CPFCNPJ.Substring(notaFiscal.Emitente.CPFCNPJ.Length - 14);
                                    if (Utilidades.Validate.ValidarCNPJ(notaFiscal.Emitente.CPFCNPJ))
                                        notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                                    else
                                    {
                                        notaFiscal.Emitente.CPFCNPJ = notaFiscal.Emitente.CPFCNPJ.Substring(notaFiscal.Emitente.CPFCNPJ.Length - 11);
                                        notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                                    }
                                }
                                else
                                    notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;

                                notaFiscal.Emitente.Endereco.Telefone = Utilidades.String.OnlyNumbers(notaFiscal.Emitente.Endereco.Telefone);
                            }
                            else if (cargaPedido.Pedido.Remetente != null) //Pega emitente do pedido
                            {
                                notaFiscal.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                                notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);
                            }
                            else
                                return "Emitente não localizado.";
                        }

                        if (notaFiscal.Destinatario != null)
                        {
                            string[] splitEnderecorDestinatario = notaFiscal.Destinatario.Endereco?.Logradouro?.Split(',') ?? "".Split(',');
                            if (notaFiscal.Destinatario.Endereco == null)
                                notaFiscal.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                            notaFiscal.Destinatario.Endereco.Logradouro = splitEnderecorDestinatario[0].Trim();

                            if (notaFiscal.Destinatario.Endereco == null || string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Logradouro) || notaFiscal.Destinatario.Endereco.Logradouro.Length <= 2)
                                notaFiscal.Destinatario.Endereco.Logradouro = "NAO INFORMADO";

                            if (splitEnderecorDestinatario.Length > 1)
                            {
                                string[] splitNumero = splitEnderecorDestinatario[1].Split('-');
                                notaFiscal.Destinatario.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");
                                if (notaFiscal.Destinatario.Endereco.Numero == "0")
                                    notaFiscal.Destinatario.Endereco.Numero = "S/N";
                                if (splitNumero.Length > 1)
                                {
                                    notaFiscal.Destinatario.Endereco.Complemento = splitNumero[1].Trim();
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Numero))
                                    notaFiscal.Destinatario.Endereco.Numero = "S/N";
                            }

                            if (notaFiscal.Destinatario.Endereco.Bairro == null || notaFiscal.Destinatario.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Logradouro))
                            {
                                if (!string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.CEP))
                                {
                                    if (!adminUnitOfWork.IsOpenSession())
                                    {
                                        adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                                        repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                                    }

                                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.Endereco.CEP)).ToString());
                                    if (endereco != null)
                                        notaFiscal.Destinatario.Endereco.Bairro = endereco.Bairro?.Descricao;

                                    if (string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Logradouro))
                                        notaFiscal.Destinatario.Endereco.Logradouro = endereco.Logradouro;
                                }
                            }

                            notaFiscal.Destinatario.CPFCNPJ = Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.CPFCNPJ);
                            notaFiscal.Destinatario.AtualizarEnderecoPessoa = true;

                            if (notaFiscal.Destinatario.CPFCNPJ.Length >= 14)
                            {
                                notaFiscal.Destinatario.CPFCNPJ = notaFiscal.Destinatario.CPFCNPJ.Substring(notaFiscal.Destinatario.CPFCNPJ.Length - 14);
                                if (Utilidades.Validate.ValidarCNPJ(notaFiscal.Destinatario.CPFCNPJ))
                                    notaFiscal.Destinatario.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                                else if (notaFiscal.Destinatario.CPFCNPJ == "00000000000000" || string.IsNullOrWhiteSpace(notaFiscal.Destinatario.CPFCNPJ))
                                {
                                    notaFiscal.Destinatario.RGIE = "ISENTO";
                                    notaFiscal.Destinatario.ClienteExterior = true;
                                }
                                else
                                {
                                    notaFiscal.Destinatario.CPFCNPJ = notaFiscal.Destinatario.CPFCNPJ.Substring(notaFiscal.Destinatario.CPFCNPJ.Length - 11);
                                    notaFiscal.Destinatario.RGIE = "ISENTO";
                                    notaFiscal.Destinatario.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                                }
                            }
                            else
                                notaFiscal.Destinatario.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;


                            notaFiscal.Destinatario.Endereco.Telefone = Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.Endereco.Telefone);
                        }
                        else if (cargaPedido.Pedido.Destinatario != null) //Pega destinatário do pedido
                        {
                            notaFiscal.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                            notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario);
                        }
                        else
                            return "Destinatário não localizado.";

                        if (notaFiscal.Tomador != null)
                        {
                            if (layoutEDI != null && layoutEDI.UtilizarTomadorExistente)
                            {
                                double cpfCnpjTomador = 0D;

                                double.TryParse(Utilidades.String.OnlyNumbers(notaFiscal.Tomador.CPFCNPJ), out cpfCnpjTomador);

                                Dominio.Entidades.Cliente tomador = cpfCnpjTomador > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;

                                if (tomador == null)
                                    return "O tomador (" + notaFiscal.Tomador.CPFCNPJ + ") da nota fiscal (" + notaFiscal.Chave + ") não está cadastrado.";

                                notaFiscal.Tomador.AtualizarEnderecoPessoa = false;
                                notaFiscal.Tomador.TipoPessoa = tomador.Tipo == "F" ? Dominio.Enumeradores.TipoPessoa.Fisica : Dominio.Enumeradores.TipoPessoa.Juridica;
                            }
                            else
                            {
                                notaFiscal.Tomador = servicoCliente.SetarDadosPessoa(notaFiscal.Tomador, adminUnitOfWork, unitOfWork, false);
                            }
                        }

                        string retorno = serCargaNotaFiscal.InformarDadosNotaCarga(notaFiscal, cargaPedido, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Auditado, out alteradoTipoDeCarga);

                        if (!string.IsNullOrWhiteSpace(retorno))
                            mensagem = retorno;
                        else
                            notaAceita = true;

                        notas++;
                    }
                }
            }

            if (notaAceita)
                mensagem = "";

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                foreach (string identi in identificaoesUsadas)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas cargaPedidoIdentificacaoNotas = repCargaPedidoRotas.BuscarPorCodigoIdentificacao(cargaPedido.Codigo, identi);
                    if (cargaPedidoIdentificacaoNotas == null)
                    {
                        cargaPedidoIdentificacaoNotas = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas();
                        cargaPedidoIdentificacaoNotas.IdenticacaoRota = identi;
                        cargaPedidoIdentificacaoNotas.CargaPedido = cargaPedido;
                        repCargaPedidoRotas.Inserir(cargaPedidoIdentificacaoNotas);
                    }
                }
            }

            return mensagem;
        }

        private string ProcessarNotasFiscaisNOTFIS(Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool validarRotas, bool validarNumeroReferenciaEDI, string rotas, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.LayoutEDI layoutEDI)
        {
            bool notaAceita = false;
            string mensagem = "";
            string[] idenNotas = rotas.Replace(" ", "").Split('/');

            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotas repCargaPedidoRotas = new Repositorio.Embarcador.Cargas.CargaPedidoRotas(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Cliente svcCliente = new Servicos.Cliente(_conexao.StringConexao);

            List<string> identificaoesUsadas = new List<string>();

            bool cteSubContratacao = TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && notfis.CabecalhoDocumento.Embarcadores.Any(e => e.Destinatarios.Any(d => d.CTes != null && d.CTes.Any(n => !string.IsNullOrEmpty(n.Chave))));

            if ((notfis.CabecalhoDocumento != null && notfis.CabecalhoDocumento.Embarcador != null && notfis.CabecalhoDocumento.Embarcadores.Any(e => e.Destinatarios.Any(d => d.NotasFiscais.Any(n => n.CTe != null && !string.IsNullOrWhiteSpace(n.CTe.Chave))))) || (!string.IsNullOrWhiteSpace(notfis.ChaveCTeAnterior)) || cteSubContratacao)
            {
                ProcessarCTesSubcontratacaoNOTFIS(ref mensagem, ref notaAceita, idenNotas, notfis, cargaPedido, validarRotas, rotas, unitOfWork, adminUnitOfWork, layoutEDI);
            }
            else
            {
                foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador remetente in notfis.CabecalhoDocumento.Embarcadores)
                {
                    foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario in remetente.Destinatarios)
                    {
                        if (destinatario.NotasFiscais != null)
                        {
                            foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal documento in destinatario.NotasFiscais)
                            {
                                if (!adminUnitOfWork.IsOpenSession())
                                    adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                                if (validarRotas)
                                {
                                    if (!idenNotas.Contains(documento.NFe.Rota))
                                        continue;

                                    string rota = documento.NFe.Rota?.Replace(" ", "");

                                    if (!string.IsNullOrWhiteSpace(rota) && !identificaoesUsadas.Contains(rota))
                                        identificaoesUsadas.Add(rota);
                                }

                                if (validarNumeroReferenciaEDI)
                                {
                                    if (!idenNotas.Contains(documento.NFe.NumeroReferenciaEDI))
                                        continue;

                                    string numeroReferencia = documento.NFe.NumeroReferenciaEDI?.Replace(" ", "");

                                    if (!string.IsNullOrWhiteSpace(numeroReferencia) && !identificaoesUsadas.Contains(numeroReferencia))
                                        identificaoesUsadas.Add(numeroReferencia);
                                }

                                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = documento.NFe;

                                if (string.IsNullOrWhiteSpace(notaFiscal.Chave) && layoutEDI.BuscarNotaSemChaveDosDocumentosDestinados)
                                {
                                    int.TryParse(notaFiscal.Serie, out int serie);

                                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa = repDocumentoDestinadoEmpresa.BuscarPorNumeroSerieEEmitente(notaFiscal.Numero, serie, notaFiscal.Emitente.CPFCNPJ);

                                    if (documentoDestinadoEmpresa != null)
                                    {
                                        string caminhoXML = Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterCaminhoDocumentoDestinado(documentoDestinadoEmpresa, unitOfWork);

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                                        {
                                            System.IO.Stream streamNF = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoXML);

                                            string mensagemRetorno = ProcessarXMLNFe(streamNF, cargaPedido, unitOfWork, out bool msgAlertaObservacao, notaFiscal.ValorFrete);

                                            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                                                mensagem = mensagemRetorno;
                                            else
                                                notaAceita = true;

                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        double.TryParse(notaFiscal?.Emitente?.CPFCNPJ, out double cpfCnpjEmitente);
                                        double.TryParse(notaFiscal?.Destinatario?.CPFCNPJ, out double cpfCnpjDestinatario);

                                        if (cpfCnpjEmitente > 0D && cpfCnpjDestinatario > 0D && notaFiscal.Numero > 0 && serie > 0)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorNumeroSerieEParticipantes(notaFiscal.Numero, serie, cpfCnpjEmitente, cpfCnpjDestinatario);

                                            if (xmlNotaFiscal != null)
                                            {
                                                string mensagemRetorno = ProcessarXMLNFe(null, cargaPedido, unitOfWork, out bool msgAlertaObservacao, notaFiscal.ValorFrete, null, xmlNotaFiscal);

                                                if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                                                    mensagem = mensagemRetorno;
                                                else
                                                    notaAceita = true;

                                                continue;
                                            }
                                        }
                                    }
                                }

                                notaFiscal.Emitente = svcCliente.SetarDadosPessoa(remetente.Pessoa, adminUnitOfWork, unitOfWork);
                                notaFiscal.Destinatario = svcCliente.SetarDadosPessoa(destinatario.Pessoa, adminUnitOfWork, unitOfWork);
                                notaFiscal.Modelo = "55";
                                notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                                notaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);
                                notaFiscal.DocumentoRecebidoViaNOTFIS = true;

                                if (string.IsNullOrWhiteSpace(notaFiscal.Chave) && documento.ComplementoNotaFiscal != null && !string.IsNullOrWhiteSpace(documento.ComplementoNotaFiscal.ChaveNFe))
                                    notaFiscal.Chave = documento.ComplementoNotaFiscal.ChaveNFe;

                                if (documento.Recebedor != null && documento.Recebedor.Pessoa != null && documento.NFe != null && documento.NFe.Recebedor == null)
                                    documento.NFe.Recebedor = documento.Recebedor.Pessoa;

                                string retorno = serCargaNotaFiscal.InformarDadosNotaCarga(notaFiscal, cargaPedido, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Auditado, out bool alteradoTipoDeCarga);

                                if (!string.IsNullOrWhiteSpace(retorno))
                                    mensagem = retorno;
                                else
                                    notaAceita = true;
                            }
                        }

                        if (destinatario.CTes != null)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte in destinatario.CTes)
                            {
                                if (!adminUnitOfWork.IsOpenSession())
                                    adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                                cte.Remetente = SetarDadosPessoa(remetente.Pessoa, adminUnitOfWork, unitOfWork);
                                cte.Destinatario = SetarDadosPessoa(destinatario.Pessoa, adminUnitOfWork, unitOfWork);

                                if (cte.Emitente == null)
                                    cte.Emitente = cte.NFEs?.FirstOrDefault()?.Transportador;

                                SetarDadosEmitente(cte.Emitente, adminUnitOfWork);

                                cte.LocalidadeInicioPrestacao = remetente.Pessoa.Endereco.Cidade;
                                cte.LocalidadeFimPrestacao = destinatario.Pessoa.Endereco.Cidade;

                                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

                                cte.NumeroRomaneio = cte.NFEs.FirstOrDefault()?.NumeroRomaneio;
                                cte.NumeroPedido = cte.NFEs.FirstOrDefault()?.NumeroPedido;

                                cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>()
                            {
                                new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                                {
                                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                                    Medida = "Quilograma",
                                    Quantidade = cte.NFEs.Sum(o => o.Peso)
                                }
                            };
                                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                                string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                                if (!string.IsNullOrWhiteSpace(retorno))
                                    mensagem = retorno;
                            }
                        }
                    }
                }
            }

            if (notaAceita)
                mensagem = "";

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                foreach (string identi in identificaoesUsadas)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas cargaPedidoIdentificacaoNotas = repCargaPedidoRotas.BuscarPorCodigoIdentificacao(cargaPedido.Codigo, identi);
                    if (cargaPedidoIdentificacaoNotas == null)
                    {
                        cargaPedidoIdentificacaoNotas = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas();
                        cargaPedidoIdentificacaoNotas.IdenticacaoRota = identi;
                        cargaPedidoIdentificacaoNotas.CargaPedido = cargaPedido;
                        repCargaPedidoRotas.Inserir(cargaPedidoIdentificacaoNotas);
                    }
                }
            }

            return mensagem;
        }

        private void ProcessarCTesSubcontratacaoNOTFIS(ref string mensagem, ref bool notaAceita, string[] idenNotas, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.LayoutEDI layoutEDI)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

            List<string> chavesCTes = notasFiscais.Select(o => o.ChaveCTe).Distinct().ToList();

            foreach (string chaveCTe in chavesCTes)
            {
                if (!Utilidades.Validate.ValidarChave(chaveCTe))
                {
                    mensagem = $"Chave do CT-e [{chaveCTe}] inválida.";
                    notaAceita = false;

                    return;
                }

                List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscaisAgrupadas = notasFiscais.Where(o => o.ChaveCTe == chaveCTe).ToList();

                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscalPrimeira = notasFiscais.FirstOrDefault();

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe()
                {
                    Numero = chaveCTe.Substring(25, 9).ToInt(),
                    Serie = chaveCTe.Substring(22, 3).ToInt().ToString(),
                    Chave = chaveCTe,
                    Remetente = SetarDadosPessoa(notaFiscalPrimeira.Emitente, adminUnitOfWork, unitOfWork),
                    Destinatario = SetarDadosPessoa(notaFiscalPrimeira.Destinatario, adminUnitOfWork, unitOfWork),
                    Tomador = SetarDadosPessoa(notaFiscalPrimeira.Tomador, adminUnitOfWork, unitOfWork),
                    Emitente = notaFiscalPrimeira.Transportador
                };

                SetarDadosEmitente(cte.Emitente, adminUnitOfWork);

                cte.LocalidadeInicioPrestacao = cte.Remetente.Endereco.Cidade;
                cte.LocalidadeFimPrestacao = cte.Destinatario.Endereco.Cidade;

                if (layoutEDI != null && layoutEDI.UtilizarTomadorComoExpedidor)
                    cte.Expedidor = cte.Tomador;

                if (cte.Recebedor != null)
                    cte.LocalidadeFimPrestacao = cte.Recebedor.Endereco.Cidade;

                if (cte.Expedidor != null)
                    cte.LocalidadeInicioPrestacao = cte.Expedidor.Endereco.Cidade;

                cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
                {
                    ValorPrestacaoServico = notasFiscaisAgrupadas.Sum(o => o.ValorFrete),
                    ValorTotalAReceber = notasFiscaisAgrupadas.Sum(o => o.ValorFrete)
                };

                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                cte.NumeroRomaneio = notaFiscalPrimeira.NumeroRomaneio;
                cte.NumeroPedido = notaFiscalPrimeira.NumeroPedido;

                cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscaisAgrupadas)
                {
                    DateTime dataEmissao;
                    if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "ddMMyyyy", null, DateTimeStyles.None, out dataEmissao))
                        dataEmissao = DateTime.Now;

                    int numero = notaFiscal.Numero;
                    if (numero <= 0 && !string.IsNullOrWhiteSpace(notaFiscal.Chave) && notaFiscal.Chave.Length >= 44)
                        int.TryParse(notaFiscal.Chave.Substring(25, 9), out numero);

                    cte.NFEs.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                    {
                        Chave = notaFiscal.Chave.Length > 44 ? notaFiscal.Chave.Substring(0, 44) : notaFiscal.Chave,
                        DataEmissao = dataEmissao,
                        Numero = numero,
                        Peso = notaFiscal.PesoBruto,
                        Valor = notaFiscal.Valor,
                        NumeroPedido = notaFiscal.NumeroPedido,
                        NumeroRomaneio = notaFiscal.NumeroRomaneio,
                        NumeroReferenciaEDI = notaFiscal.NumeroReferenciaEDI,
                        PINSuframa = notaFiscal.PINSuframa,
                        NCMPredominante = notaFiscal.NCMPredominante,
                        NumeroControleCliente = notaFiscal.NumeroControleCliente
                    });
                }

                cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga()
                {
                    ValorTotalCarga = notasFiscaisAgrupadas.Sum(o => o.Valor)
                };

                cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>()
                                {
                                    new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                                    {
                                         Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                                         Medida = "Quilograma",
                                         Quantidade = notasFiscaisAgrupadas.Sum(o => o.PesoBruto)
                                    }
                                };
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                if (!string.IsNullOrWhiteSpace(retorno))
                    mensagem = retorno;
            }
        }

        private void ProcessarCTesSubcontratacaoNOTFIS(ref string mensagem, ref bool notaAceita, string[] idenNotas, Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool validarRotas, string rotas, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.LayoutEDI layoutEDI)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

            if (notfis.CabecalhoDocumento != null && notfis.CabecalhoDocumento.Embarcadores != null && notfis.CabecalhoDocumento.Embarcadores.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador remetente in notfis.CabecalhoDocumento.Embarcadores)
                {
                    foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario in remetente.Destinatarios)
                    {
                        List<string> chavesCTes = null;
                        if (destinatario.CTes != null)
                            chavesCTes = destinatario.CTes.Select(o => o.Chave).Distinct().ToList();
                        else
                            chavesCTes = destinatario.NotasFiscais.Select(o => o.CTe.Chave).Distinct().ToList();

                        foreach (string chaveCTe in chavesCTes)
                        {
                            List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> notasFiscaisCTe = null;
                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = null;
                            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa tomador = null;
                            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa expedidor = null;
                            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa recebedor = null;

                            if (destinatario.CTes != null)
                            {
                                notasFiscaisCTe = destinatario.NotasFiscais;
                                cte = destinatario.CTes.Where(o => o.Chave == chaveCTe).FirstOrDefault();
                                tomador = destinatario.ResponsavelFrete.Pessoa;
                                recebedor = destinatario.Recebedor?.Pessoa != null ? destinatario.Recebedor?.Pessoa : notasFiscaisCTe[0].Recebedor?.Pessoa ?? null;
                                expedidor = notasFiscaisCTe[0].Expedidor?.Pessoa ?? null;
                            }
                            else
                            {
                                notasFiscaisCTe = destinatario.NotasFiscais.Where(o => o.CTe.Chave == chaveCTe).ToList();
                                cte = notasFiscaisCTe[0].CTe;
                                tomador = notasFiscaisCTe[0].ResponsavelFrete.Pessoa;
                                recebedor = notasFiscaisCTe[0].Recebedor?.Pessoa;
                                expedidor = notasFiscaisCTe[0].Expedidor?.Pessoa;
                            }

                            cte.Remetente = SetarDadosPessoa(remetente.Pessoa, adminUnitOfWork, unitOfWork);
                            cte.Destinatario = SetarDadosPessoa(destinatario.Pessoa, adminUnitOfWork, unitOfWork);
                            cte.Tomador = SetarDadosPessoa(tomador, adminUnitOfWork, unitOfWork);
                            cte.Emitente = Servicos.Embarcador.Pessoa.Pessoa.Converter(cte.Tomador);

                            if (recebedor != null)
                                cte.Recebedor = SetarDadosPessoa(recebedor, adminUnitOfWork, unitOfWork);

                            if (expedidor != null)
                                cte.Expedidor = SetarDadosPessoa(expedidor, adminUnitOfWork, unitOfWork);

                            cte.LocalidadeInicioPrestacao = remetente.Pessoa.Endereco.Cidade;
                            cte.LocalidadeFimPrestacao = destinatario.Pessoa.Endereco.Cidade;

                            if (layoutEDI != null && layoutEDI.UtilizarTomadorComoExpedidor)
                                cte.Expedidor = cte.Tomador;

                            if (cte.Recebedor != null)
                                cte.LocalidadeFimPrestacao = cte.Recebedor.Endereco.Cidade;

                            if (cte.Expedidor != null)
                                cte.LocalidadeInicioPrestacao = cte.Expedidor.Endereco.Cidade;

                            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
                            {
                                ValorPrestacaoServico = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete) > 0 ? notasFiscaisCTe.Sum(o => o.NFe.ValorFrete) : cte.ValorFrete?.ValorTotalAReceber ?? 0m,
                                ValorTotalAReceber = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete) > 0 ? notasFiscaisCTe.Sum(o => o.NFe.ValorFrete) : cte.ValorFrete?.ValorTotalAReceber ?? 0m
                            };

                            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                            cte.NumeroRomaneio = notasFiscaisCTe[0].NumeroRomaneio;
                            cte.NumeroPedido = notasFiscaisCTe[0].NumeroPedido;

                            if (cte.NFEs == null || cte.NFEs.Count <= 0)
                            {
                                cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                                foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal in notasFiscaisCTe)
                                {
                                    DateTime dataEmissao;
                                    if (!DateTime.TryParseExact(notaFiscal.NFe.DataEmissao, "ddMMyyyy", null, DateTimeStyles.None, out dataEmissao))
                                        dataEmissao = DateTime.Now;

                                    int numero = notaFiscal.NFe.Numero;
                                    if (numero <= 0 && !string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave) && notaFiscal.NFe.Chave.Length >= 44)
                                        int.TryParse(notaFiscal.NFe.Chave.Substring(25, 9), out numero);

                                    cte.NFEs.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                                    {
                                        Chave = notaFiscal.NFe.Chave.Length > 44 ? notaFiscal.NFe.Chave.Substring(0, 44) : notaFiscal.NFe.Chave,
                                        DataEmissao = dataEmissao,
                                        Numero = numero,
                                        Peso = notaFiscal.NFe.PesoBruto,
                                        Valor = notaFiscal.NFe.Valor,
                                        NumeroPedido = notaFiscal.NumeroPedido,
                                        NumeroRomaneio = notaFiscal.NumeroRomaneio,
                                        NumeroReferenciaEDI = notaFiscal.NFe.NumeroReferenciaEDI,
                                        PINSuframa = notaFiscal.NFe.PINSuframa,
                                        NCMPredominante = notaFiscal.NFe.NCMPredominante,
                                        NumeroControleCliente = notaFiscal.NFe.NumeroControleCliente
                                    });
                                }
                            }

                            cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga()
                            {
                                ValorTotalCarga = cte.NFEs.Sum(o => o.Valor)
                            };

                            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>()
                                {
                                    new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                                    {
                                         Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                                         Medida = "Quilograma",
                                         Quantidade = cte.NFEs.Sum(o => o.Peso)
                                    }
                                };
                            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                            string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                            if (!string.IsNullOrWhiteSpace(retorno))
                                mensagem = retorno;
                        }
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(notfis.ChaveCTeAnterior))
            {
                List<string> chavesCTes = new List<string>();
                chavesCTes.Add(notfis.ChaveCTeAnterior);

                foreach (string chaveCTe in chavesCTes)
                {
                    List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> notasFiscaisCTe = notfis.NotasFiscais.ToList();

                    Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal primeiraNotaFiscal = notasFiscaisCTe[0];
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();

                    cte.Chave = notfis.ChaveCTeAnterior;
                    cte.Serie = notfis.SerieCTEAnterior;
                    int.TryParse(notfis.NumeroCTeAnterior, out int numeroCTe);
                    cte.Numero = numeroCTe;
                    cte.Remetente = SetarDadosPessoa(notfis.Participantes.Remetente.Pessoa, adminUnitOfWork, unitOfWork);
                    cte.Destinatario = SetarDadosPessoa(notfis.Participantes.Destinatario.Pessoa, adminUnitOfWork, unitOfWork);
                    cte.Tomador = SetarDadosPessoa(notasFiscaisCTe[0].ResponsavelFrete.Pessoa, adminUnitOfWork, unitOfWork);
                    cte.Emitente = Servicos.Embarcador.Pessoa.Pessoa.Converter(cte.Tomador);

                    if (primeiraNotaFiscal.Recebedor != null)
                        cte.Recebedor = SetarDadosPessoa(primeiraNotaFiscal.Recebedor.Pessoa, adminUnitOfWork, unitOfWork);

                    cte.LocalidadeInicioPrestacao = notfis.Participantes.Remetente.Pessoa.Endereco.Cidade;
                    cte.LocalidadeFimPrestacao = notfis.Participantes.Destinatario.Pessoa.Endereco.Cidade;

                    if (layoutEDI != null && layoutEDI.UtilizarTomadorComoExpedidor)
                        cte.Expedidor = cte.Tomador;

                    if (cte.Recebedor != null)
                        cte.LocalidadeFimPrestacao = cte.Recebedor.Endereco.Cidade;

                    if (cte.Expedidor != null)
                        cte.LocalidadeInicioPrestacao = cte.Expedidor.Endereco.Cidade;

                    cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
                    {
                        ValorPrestacaoServico = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete),
                        ValorTotalAReceber = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete)
                    };

                    cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                    cte.NumeroRomaneio = notasFiscaisCTe[0].NumeroRomaneio;
                    cte.NumeroPedido = notasFiscaisCTe[0].NumeroPedido;

                    if (cte.NFEs == null || cte.NFEs.Count <= 0)
                    {
                        cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                        foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal in notasFiscaisCTe)
                        {
                            DateTime dataEmissao;
                            if (!DateTime.TryParseExact(notaFiscal.NFe.DataEmissao, "ddMMyyyy", null, DateTimeStyles.None, out dataEmissao))
                                dataEmissao = DateTime.Now;

                            int numero = notaFiscal.NFe.Numero;
                            if (numero <= 0 && !string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave) && notaFiscal.NFe.Chave.Length >= 44)
                                int.TryParse(notaFiscal.NFe.Chave.Substring(25, 9), out numero);

                            cte.NFEs.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                            {
                                Chave = notaFiscal.NFe.Chave.Length > 44 ? notaFiscal.NFe.Chave.Substring(0, 44) : notaFiscal.NFe.Chave,
                                DataEmissao = dataEmissao,
                                Numero = numero,
                                Peso = notaFiscal.NFe.PesoBruto,
                                Valor = notaFiscal.NFe.Valor,
                                NumeroPedido = notaFiscal.NumeroPedido,
                                NumeroRomaneio = notaFiscal.NumeroRomaneio,
                                NumeroReferenciaEDI = notaFiscal.NFe.NumeroReferenciaEDI,
                                PINSuframa = notaFiscal.NFe.PINSuframa,
                                NCMPredominante = notaFiscal.NFe.NCMPredominante,
                                NumeroControleCliente = notaFiscal.NFe.NumeroControleCliente
                            });
                        }
                    }

                    cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga()
                    {
                        ValorTotalCarga = cte.NFEs.Sum(o => o.Valor)
                    };

                    cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>()
                                {
                                    new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                                    {
                                         Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                                         Medida = "Quilograma",
                                         Quantidade = cte.NFEs.Sum(o => o.Peso)
                                    }
                                };
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                    string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                    if (!string.IsNullOrWhiteSpace(retorno))
                        mensagem = retorno;
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa SetarDadosPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Repositorio.UnitOfWork unidadeTrabalho)
        {
            string[] splitEnderecorEmitente = pessoa.Endereco.Logradouro.Split(',');
            pessoa.Endereco.Logradouro = splitEnderecorEmitente[0].Trim();

            if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro) || pessoa.Endereco.Logradouro.Length <= 2)
                pessoa.Endereco.Logradouro = "NAO INFORMADO";

            if (splitEnderecorEmitente.Length > 1)
            {
                string[] splitNumero = splitEnderecorEmitente[1].Split('-');
                pessoa.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");

                if (pessoa.Endereco.Numero == "0")
                    pessoa.Endereco.Numero = "S/N";
                if (splitNumero.Count() > 1)
                    pessoa.Endereco.Complemento = splitNumero[1].Trim();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pessoa.Endereco.Numero))
                    pessoa.Endereco.Numero = "S/N";
            }

            AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = null;

            if (pessoa.Endereco.Bairro == null || pessoa.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
            {
                if (!string.IsNullOrWhiteSpace(pessoa.Endereco.CEP))
                {
                    if (!adminUnitOfWork.IsOpenSession())
                        adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                    endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP)).ToString());
                    if (endereco != null)
                        pessoa.Endereco.Bairro = endereco.Bairro?.Descricao;

                    if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
                        pessoa.Endereco.Logradouro = endereco.Logradouro;
                }
            }

            if (pessoa.Endereco.Cidade.IBGE <= 0)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(pessoa.Endereco.Cidade.Descricao, pessoa.Endereco.Cidade.SiglaUF);

                if (localidade != null)
                {
                    pessoa.Endereco.Cidade.IBGE = localidade.CodigoIBGE;
                }
                else
                {
                    if (endereco == null)
                    {
                        if (!adminUnitOfWork.IsOpenSession())
                            adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                        AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                        endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP)).ToString());
                    }

                    if (endereco != null)
                    {
                        int.TryParse(endereco.Localidade.CodigoIBGE, out int codigoIBGE);
                        pessoa.Endereco.Cidade.IBGE = codigoIBGE;
                    }
                }
            }

            pessoa.CPFCNPJ = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ);
            pessoa.AtualizarEnderecoPessoa = false;

            if (pessoa.CPFCNPJ.Length >= 14)
            {
                pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 14);
                if (Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                    pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                else
                {
                    pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11);

                    if (string.IsNullOrWhiteSpace(pessoa.RGIE))
                        pessoa.RGIE = "ISENTO";

                    pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                }
            }
            else
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;

            return pessoa;
        }

        private void SetarDadosEmitente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa emitente, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork)
        {
            string[] splitEnderecorEmitente = emitente.Endereco.Logradouro.Split(',');
            emitente.Endereco.Logradouro = splitEnderecorEmitente[0].Trim();

            if (string.IsNullOrWhiteSpace(emitente.Endereco.Logradouro) || emitente.Endereco.Logradouro.Length <= 2)
                emitente.Endereco.Logradouro = "NAO INFORMADO";

            if (splitEnderecorEmitente.Length > 1)
            {
                string[] splitNumero = splitEnderecorEmitente[1].Split('-');
                emitente.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");

                if (emitente.Endereco.Numero == "0")
                    emitente.Endereco.Numero = "S/N";
                if (splitNumero.Count() > 1)
                    emitente.Endereco.Complemento = splitNumero[1].Trim();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(emitente.Endereco.Numero))
                    emitente.Endereco.Numero = "S/N";
            }

            if (emitente.Endereco.Bairro == null || emitente.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(emitente.Endereco.Logradouro))
            {
                if (!string.IsNullOrWhiteSpace(emitente.Endereco.CEP))
                {
                    if (!adminUnitOfWork.IsOpenSession())
                        adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(emitente.Endereco.CEP)).ToString());
                    if (endereco != null)
                        emitente.Endereco.Bairro = endereco.Bairro?.Descricao;
                    else
                        emitente.Endereco.Bairro = "NAO INFORMADO";

                    if (string.IsNullOrWhiteSpace(emitente.Endereco.Logradouro))
                        emitente.Endereco.Logradouro = endereco.Logradouro;
                }
            }

            emitente.CNPJ = Utilidades.String.OnlyNumbers(emitente.CNPJ);
        }

        protected string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" });
        }
    }

    #endregion

    public class RetornoArquivo
    {
        public string nome { get; set; }
        public bool processada { get; set; }
        public string mensagem { get; set; }
    }
}
