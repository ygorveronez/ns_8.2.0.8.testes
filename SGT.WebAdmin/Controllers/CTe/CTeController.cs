using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.CTe
{
    //[CustomAuthorize("CTe/ConsultaCTe", "Cargas/Carga", "CTe/CTeCancelamento", "CTe/ConhecimentoEletronico", "CTe/CampoCartaCorrecao", "CTe/CartaCorrecao", "CTe/AutorizacaoCTeLote", "CTe/AutorizacaoAverbacaoLote")]
    public class CTeController : BaseController
    {
        #region Construtores

        public CTeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Construtores

        [CustomAuthorize("CTe/ObservacaoContribuinte")]
        public async Task<IActionResult> ObservacaoContribuinte()
        {
            return View();
        }

        [CustomAuthorize("CTe/ConsultaCTe")]
        public async Task<IActionResult> ConsultaCTe()
        {
            return View();
        }

        [CustomAuthorize("CTe/DownloadLoteCTe")]
        public async Task<IActionResult> DownloadLoteCTe()
        {
            return View();
        }

        [CustomAuthorize("CTe/CTeCancelamento")]
        public async Task<IActionResult> CTeCancelamento()
        {
            return View();
        }

        [CustomAuthorize("CTe/ConhecimentoEletronico")]
        public async Task<IActionResult> ConhecimentoEletronico()
        {
            return View();
        }

        [CustomAuthorize("CTe/CampoCartaCorrecao")]
        public async Task<IActionResult> CampoCartaCorrecao()
        {
            return View();
        }

        [CustomAuthorize("CTe/CartaCorrecao")]
        public async Task<IActionResult> CartaCorrecao()
        {
            return View();
        }

        [CustomAuthorize("CTe/AutorizacaoCTeLote")]
        public async Task<IActionResult> AutorizacaoCTeLote()
        {
            return View();
        }

        [CustomAuthorize("CTe/AutorizacaoAverbacaoLote")]
        public async Task<IActionResult> AutorizacaoAverbacaoLote()
        {
            return View();
        }

        [CustomAuthorize("CTe/CTeTerceiro")]
        public async Task<IActionResult> CTeTerceiro()
        {
            return View();
        }

        [CustomAuthorize("CTe/RegraAutorizacaoIntegracaoCTe")]
        public async Task<IActionResult> RegraAutorizacaoIntegracaoCTe()
        {
            return View();
        }

        [CustomAuthorize("CTe/AutorizacaoIntegracaoCTe")]
        public async Task<IActionResult> AutorizacaoIntegracaoCTe()
        {
            return View();
        }

        [CustomAuthorize("CTe/ImportacaoCTeEmitidoForaEmbarcador")]
        public async Task<IActionResult> ImportacaoCTeEmitidoForaEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("CTe/CancelamentoCTeSemCarga")]
        public async Task<IActionResult> CancelamentoCTeSemCarga()
        {
            return View();
        }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarXMLCancelamentoCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCte = 0;

                int.TryParse(Request.Params("CodigoCTe"), out codigoCte);

                unitOfWork.Start();
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork); ;
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCte);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);
                    if (extensao.Equals(".xml"))
                    {
                        var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                        if (cteLido.GetType() == typeof(MultiSoftware.CTe.v200.Eventos.TProcEvento))
                        {
                            MultiSoftware.CTe.v200.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v200.Eventos.TProcEvento)cteLido;
                            if (procEvento.retEventoCTe.infEvento.cStat == "135")
                            {
                                if (cte.Chave == procEvento.retEventoCTe.infEvento.chCTe)
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);
                                    if (conhecimento != null)
                                    {
                                        string retorno = svcCTe.CancelarConhencimentoImportado(conhecimento, procEvento, file.InputStream, unitOfWork);
                                        if (string.IsNullOrWhiteSpace(retorno))
                                        {
                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, conhecimento, null, "Enviou XML de Cancelamento", unitOfWork);
                                            unitOfWork.CommitChanges();
                                            return new JsonpResult(true);
                                        }
                                        else
                                        {
                                            unitOfWork.Rollback();
                                            return new JsonpResult(false, true, (string)retorno);
                                        }
                                    }
                                    else
                                    {
                                        unitOfWork.Rollback();
                                        return new JsonpResult(false, true, "O xml de cancelamento enviado não pertence a nenhum CT-e existe na base.");
                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, "O xml de cancelamento não pertence ao CT-e informado.");
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "O arquivo enviado não é um XML de cancelamento de CT-e.");
                            }
                        }
                        else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.Eventos.TProcEvento))
                        {
                            MultiSoftware.CTe.v300.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v300.Eventos.TProcEvento)cteLido;
                            if (procEvento.retEventoCTe.infEvento.cStat == "135")
                            {
                                if (cte.Chave == procEvento.retEventoCTe.infEvento.chCTe)
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);
                                    if (conhecimento != null)
                                    {
                                        string retorno = svcCTe.CancelarConhencimentoImportado(conhecimento, procEvento, file.InputStream, unitOfWork);
                                        if (string.IsNullOrWhiteSpace(retorno))
                                        {
                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, conhecimento, null, "Enviou XML de Cancelamento", unitOfWork);
                                            unitOfWork.CommitChanges();
                                            return new JsonpResult(true);
                                        }
                                        else
                                        {
                                            unitOfWork.Rollback();
                                            return new JsonpResult(false, true, (string)retorno);
                                        }
                                    }
                                    else
                                    {
                                        unitOfWork.Rollback();
                                        return new JsonpResult(false, true, "O xml de cancelamento enviado não pertence a nenhum CT-e existe na base.");
                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, "O xml de cancelamento não pertence ao CT-e informado.");
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "O arquivo enviado não é um XML de cancelamento de CT-e.");
                            }
                        }
                        else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.Eventos.TProcEvento))
                        {
                            MultiSoftware.CTe.v400.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v400.Eventos.TProcEvento)cteLido;
                            if (procEvento.retEventoCTe.infEvento.cStat == "135")
                            {
                                if (cte.Chave == procEvento.retEventoCTe.infEvento.chCTe)
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);
                                    if (conhecimento != null)
                                    {
                                        string retorno = svcCTe.CancelarConhencimentoImportado(conhecimento, procEvento, file.InputStream, unitOfWork);
                                        if (string.IsNullOrWhiteSpace(retorno))
                                        {
                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, conhecimento, null, "Enviou XML de Cancelamento", unitOfWork);
                                            unitOfWork.CommitChanges();
                                            return new JsonpResult(true);
                                        }
                                        else
                                        {
                                            unitOfWork.Rollback();
                                            return new JsonpResult(false, true, retorno);
                                        }
                                    }
                                    else
                                    {
                                        unitOfWork.Rollback();
                                        return new JsonpResult(false, true, "O xml de cancelamento enviado não pertence a nenhum CT-e existe na base.");
                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, "O xml de cancelamento não pertence ao CT-e informado.");
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "O arquivo enviado não é um XML de cancelamento de CT-e.");
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O arquivo enviado não é um XML de cancelamento de CT-e.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A extensão do arquivo é inválida.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Nenhum arquivo enviado.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao envio o XML do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarXMLCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork); ;
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);
                    if (extensao.Equals(".xml"))
                    {
                        var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);

                        Type tipoCTe = cteLido.GetType();

                        if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);
                            if (empresa != null)
                            {
                                object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, file.InputStream, unitOfWork, string.Empty, string.Empty);
                                if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteRetorno.GetType().BaseType == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                                    unitOfWork.CommitChanges();
                                    cte.Codigo = cteConvertido.Codigo;
                                    return new JsonpResult(cte);
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, (string)cteRetorno);
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "A empresa pela qual o CT-e foi emitido não está cadastrada.");
                            }
                        }
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);
                            if (empresa != null)
                            {
                                object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, file.InputStream, unitOfWork, string.Empty, string.Empty, false, false, false);
                                if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteRetorno.GetType().BaseType == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                                    unitOfWork.CommitChanges();
                                    cte.Codigo = cteConvertido.Codigo;
                                    return new JsonpResult(cte);
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, (string)cteRetorno);
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "A empresa pela qual o CT-e foi emitido não está cadastrada.");
                            }
                        }
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);
                            if (empresa != null)
                            {
                                object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, file.InputStream, unitOfWork, string.Empty, string.Empty, false, false, false);
                                if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteRetorno.GetType().BaseType == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                                    unitOfWork.CommitChanges();
                                    cte.Codigo = cteConvertido.Codigo;
                                    return new JsonpResult(cte);
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, (string)cteRetorno);
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "A empresa pela qual o CT-e foi emitido não está cadastrada.");
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O arquivo enviado não é um XML de CT-e.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A extensão do arquivo é inválida.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Nenhum arquivo enviado.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao envio o XML do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params("Codigo"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, "CT-e não encontrado.");

                Repositorio.EntregaCTe repEntregaCTe = new Repositorio.EntregaCTe(unidadeDeTrabalho);
                List<Dominio.Entidades.EntregaCTe> entregasCte = null;
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado)
                    entregasCte = repEntregaCTe.BuscarPorCTe(cte.Codigo);

                var retorno = new
                {
                    CTe = ObterDetalhesCTe(cte),
                    Remetente = Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Remetente, cte),
                    Destinatario = Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Destinatario, cte),
                    Expedidor = Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Expedidor, cte),
                    Recebedor = Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Recebedor, cte),
                    Tomador = Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Outros, cte),
                    Documentos = ObterDocumentosCTe(cte),
                    EntregasSimplificado = ObterEntregasSimplificado(cte, entregasCte),
                    EntregasSimplificadoDocumentos = ObterEntregasSimplificadoDocumentos(cte, entregasCte, unidadeDeTrabalho),
                    EntregasSimplificadoDocumentosTransporteAnterior = ObterEntregasSimplificadoDocumentosTransporteAnterior(cte, entregasCte, unidadeDeTrabalho),
                    EntregasSimplificadoComponentesPrestacaoServico = ObterEntregasSimplificadoComponentesPrestacaoServico(cte, entregasCte, unidadeDeTrabalho),
                    Rodoviario = ObterModalRodoviarioCTe(cte),
                    Veiculos = ObterVeiculosCTe(cte),
                    Motoristas = ObterMotoristasCTe(cte),
                    InformacaoCarga = ObterInformacaoCargaCTe(cte),
                    QuantidadesCarga = ObterQuantidadesCargaCTe(cte, unidadeDeTrabalho),
                    Seguros = ObterSegurosCTe(cte),
                    DocumentosTransporteAnteriorPapel = ObterDocumentosTransporteAnteriorPapelCTe(cte),
                    DocumentosTransporteAnteriorEletronico = ObterDocumentosTransporteAnteriorEletronicoCTe(cte),
                    Observacoes = ObterObservacoesCTe(cte),
                    ObservacoesFisco = ObterObservacoesFiscoCTe(cte),
                    ObservacoesContribuinte = ObterObservacoesContribuinteCTe(cte),
                    TotalServico = ObterTotaisServicoCTe(cte),
                    Componentes = ObterComponentesPrestacaoCTe(cte, unidadeDeTrabalho),

                    ProdutosPerigosos = ObterProdutosPerigososCTe(cte),
                    ModalAquaviario = ObterModalAquaviarioCTe(cte),
                    Balsas = ObterBalsasCTe(cte),
                    Containers = ObterContainersCTe(cte),
                    ContainerDocumentos = ObterContainerDocumentosCTe(cte, unidadeDeTrabalho),
                    ModalDutoviario = ObterModalDutoviarioCTe(cte),
                    ModalMultimodal = ObterModalMultimodalCTe(cte),
                    ModalAereo = ObterModalAereoCTe(cte),
                    Manuseios = ObterInformacaoManuseiosCTe(cte),
                    ModalFerroviario = ObterModalFerroviarioCTe(cte),
                    Ferrovias = ObterFerroviasCTe(cte),
                    InformacaoModal = ObterInformacaoModalCTe(cte),
                    CTeSubstituicao = ObterCTeSubstituicao(cte, unidadeDeTrabalho),
                    CTeAnulacao = ObterCTeAnulacao(cte),
                    CTeComplementar = ObterCTeComplementar(cte)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados do CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterInformacoesTerceiro()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double terceiro = 0;
                double.TryParse(Request.Params("Codigo"), out terceiro);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeTrabalho);


                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(terceiro);

                if (cliente == null)
                    return new JsonpResult(false, true, "Transportador não encontrado.");

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = cliente.Modalidades.Where(obj => obj.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault();

                if (modalidade == null)
                    return new JsonpResult(false, true, "O transportador informado não é um terceiro.");

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidade.Codigo);

                return new JsonpResult(new
                {
                    Codigo = cliente.CPF_CNPJ,
                    Localidade = new
                    {
                        cliente.Localidade.Codigo,
                        Descricao = cliente.Localidade.DescricaoCidadeEstado,
                        Estado = cliente.Localidade.Estado.Sigla
                    },
                    RNTRC = modalidadeTransportador.RNTRC,
                    RazaoSocial = cliente.Nome,
                    cliente.NomeFantasia,
                    CNPJ = cliente.CPF_CNPJ_Formatado,
                    SerieInterestadual = new
                    {
                        Codigo = 0,
                        Numero = ""
                    },
                    SerieIntraestadual = new
                    {
                        Codigo = 0,
                        Numero = ""
                    },
                    Serie = new
                    {
                        Codigo = 0,
                        Numero = ""
                    },
                    DiasParaEntrega = 0,
                    ProdutoPredominante = string.Empty
                });

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do transportador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterInformacoesEmpresa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    return new JsonpResult(false, true, "Transportador não encontrado.");

                Dominio.Entidades.EmpresaSerie serie = repSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.CTe);

                return new JsonpResult(new
                {
                    empresa.Codigo,
                    Localidade = new
                    {
                        empresa.Localidade.Codigo,
                        Descricao = empresa.Localidade.DescricaoCidadeEstado,
                        Estado = empresa.Localidade.Estado.Sigla
                    },
                    RNTRC = empresa.RegistroANTT,
                    empresa.RazaoSocial,
                    empresa.NomeFantasia,
                    CNPJ = empresa.CNPJ_Formatado,
                    SerieInterestadual = new
                    {
                        Codigo = empresa.Configuracao?.SerieInterestadual?.Codigo ?? 0,
                        Numero = empresa.Configuracao?.SerieInterestadual?.Numero.ToString()
                    },
                    SerieIntraestadual = new
                    {
                        Codigo = empresa.Configuracao?.SerieIntraestadual?.Codigo ?? 0,
                        Numero = empresa.Configuracao?.SerieIntraestadual?.Numero.ToString() ?? ""
                    },
                    Serie = new
                    {
                        Codigo = serie?.Codigo ?? 0,
                        Numero = serie?.Numero.ToString() ?? ""
                    },
                    DiasParaEntrega = empresa.Configuracao?.DiasParaEntrega ?? 0,
                    ProdutoPredominante = empresa.Configuracao?.ProdutoPredominante ?? string.Empty,
                    NumeroCOTM = empresa.COTM
                });

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do transportador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterInformacoesImpostos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                string ufInicioPrestacao = Request.Params("EstadoInicioPrestacao");
                string ufTerminoPrestacao = Request.Params("EstadoTerminoPrestacao");

                int codigoAtividadeRemetente, codigoAtividadeDestinatario, codigoAtividadeTomador, codigoLocalidadeInicioPrestacao, codigoLocalidadeTerminoPrestacao;
                int.TryParse(Request.Params("AtividadeRemetente"), out codigoAtividadeRemetente);
                int.TryParse(Request.Params("AtividadeDestinatario"), out codigoAtividadeDestinatario);
                int.TryParse(Request.Params("AtividadeTomador"), out codigoAtividadeTomador);
                int.TryParse(Request.Params("LocalidadeInicioPrestacao"), out codigoLocalidadeInicioPrestacao);
                int.TryParse(Request.Params("LocalidadeTerminoPrestacao"), out codigoLocalidadeTerminoPrestacao);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Dominio.Entidades.Localidade localidadeInicioPrestacao = repLocalidade.BuscarPorCodigo(codigoLocalidadeInicioPrestacao);
                Dominio.Entidades.Localidade localidadeTerminoPrestacao = repLocalidade.BuscarPorCodigo(codigoLocalidadeTerminoPrestacao);

                if (empresa == null)
                    return new JsonpResult(false, true, "Transportador não encontrado para obter os dados de impostos.");

                if (localidadeInicioPrestacao == null)
                    return new JsonpResult(false, true, "Localidade de início da prestação não encontrada para obter os dados de impostos.");

                if (localidadeTerminoPrestacao == null)
                    return new JsonpResult(false, true, "Localidade de término da prestação não encontrada para obter os dados de impostos.");

                Dominio.Entidades.Aliquota aliquota = Servicos.CTe.ObterAliquota(empresa.Localidade.Estado.Sigla, localidadeInicioPrestacao.Estado.Sigla, localidadeTerminoPrestacao.Estado.Sigla, codigoAtividadeRemetente, codigoAtividadeDestinatario, codigoAtividadeTomador, unidadeTrabalho);

                if (aliquota != null)
                {
                    return new JsonpResult(new
                    {
                        Aliquota = aliquota.Percentual.ToString("n2"),
                        CFOP = new
                        {
                            Codigo = aliquota.CFOP != null ? aliquota.CFOP.Codigo : 0,
                            Numero = aliquota.CFOP != null ? aliquota.CFOP.CodigoCFOP.ToString() : string.Empty
                        },
                        CST = Servicos.CTe.ObterEnumeradorICMS(aliquota.CST)
                    });
                }
                else
                {
                    return new JsonpResult(null);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do transportador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterInformacoesModalRodoviario()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosVeiculos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Veiculos"));
                List<int> codigosMotoristas = JsonConvert.DeserializeObject<List<int>>(Request.Params("Motoristas"));

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeTrabalho);

                List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarPorCodigo(codigosVeiculos.ToArray());
                List<Dominio.Entidades.Usuario> motoristas = repMotorista.BuscarMotoristaPorCodigo(codigosMotoristas.ToArray());

                var retorno = new
                {
                    Motoristas = (from obj in motoristas
                                  select new
                                  {
                                      obj.Codigo,
                                      CPF = obj.CPF_Formatado,
                                      Nome = obj.Nome
                                  }).ToList(),
                    Veiculos = (from obj in veiculos
                                select new
                                {
                                    Codigo = Guid.NewGuid().ToString(),
                                    CodigoVeiculo = obj.Codigo,
                                    Placa = obj.Placa,
                                    Estado = obj.Estado.Sigla,
                                    RENAVAM = obj.Renavam,
                                    Propriedade = obj.DescricaoTipo,
                                    Rodado = obj.DescricaoTipoRodado,
                                    Carroceria = obj.DescricaoTipoCarroceria
                                }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações dos veículos e motoristas.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
                int.TryParse(Request.Params("Serie"), out int serie);
                int.TryParse(Request.Params("Empresa"), out int empresa);
                int.TryParse(Request.Params("ModeloDocumento"), out int codigoModeloDocumento);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

                decimal.TryParse(Request.Params("ValorInicial"), out decimal valorInicial);
                decimal.TryParse(Request.Params("ValorFinal"), out decimal valorFinal);

                string chave = Request.Params("Chave");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento? tipoLiquidacao = null;
                if (Enum.TryParse(Request.Params("SituacaoLiquidacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacaoRelatorioDocumentoFaturamento tipoLiquidacaoAux))
                    tipoLiquidacao = tipoLiquidacaoAux;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo", "ModeloDocumentoFiscal", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "TomadorPagador", 23, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo Pessoas", "GrupoPessoas", 23, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorAReceber", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Chave", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Serie")
                    propOrdena = "Serie.Numero";
                else if (propOrdena == "ModeloDocumentoFiscal")
                    propOrdena = "ModeloDocumentoFiscal.Abreviacao";
                else if (propOrdena == "TomadorPagador")
                    propOrdena = "TomadorPagador.Nome";
                else if (propOrdena == "GrupoPessoas")
                    propOrdena = "TomadorPagador.GrupoPessoas.Descricao";


                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCTe.Consultar(numeroInicial, numeroFinal, serie, chave, empresa, codigoModeloDocumento, codigoGrupoPessoas, valorInicial, valorFinal, tipoLiquidacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCTe.ContarConsulta(numeroInicial, numeroFinal, serie, chave, empresa, codigoModeloDocumento, codigoGrupoPessoas, valorInicial, valorFinal, tipoLiquidacao));

                var lista = (from p in listaCTe
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.Numero,
                                 Numero = p.Numero.ToString(),
                                 Serie = p.Serie?.Numero.ToString() ?? string.Empty,
                                 DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 ModeloDocumentoFiscal = p.ModeloDocumentoFiscal?.Abreviacao,
                                 TomadorPagador = p.TomadorPagador != null ? p.TomadorPagador.Nome + " (" + p.TomadorPagador.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 GrupoPessoas = p.TomadorPagador?.GrupoPessoas?.Descricao,
                                 ValorAReceber = p.ValorAReceber,
                                 Chave = p.Chave
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
        public async Task<IActionResult> ObterInformacoesSeguro()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Tomador")), out cpfCnpjTomador);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

                if (tomador == null)
                    return new JsonpResult(false, true, "Tomador não encontrado.");

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = null;

                if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                    apoliceSeguro = tomador.ApolicesSeguro.Where(o => o.InicioVigencia <= DateTime.Now.Date && o.FimVigencia >= DateTime.Now.AddDays(1).Date).FirstOrDefault();
                else
                    apoliceSeguro = tomador.GrupoPessoas.ApolicesSeguro.Where(o => o.InicioVigencia <= DateTime.Now.Date && o.FimVigencia >= DateTime.Now.Date).FirstOrDefault();

                if (apoliceSeguro != null)
                {
                    return new JsonpResult(new
                    {
                        apoliceSeguro.NumeroApolice,
                        apoliceSeguro.NumeroAverbacao,
                        apoliceSeguro.Responsavel,
                        Seguradora = new
                        {
                            Codigo = apoliceSeguro.Seguradora?.Codigo,
                            Descricao = apoliceSeguro.Seguradora?.Nome.Left(29),
                            CNPJ = apoliceSeguro.Seguradora?.ClienteSeguradora?.CPF_CNPJ_Formatado ?? string.Empty
                        }
                    });
                }
                else
                {
                    return new JsonpResult(null);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados de seguro.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosGeraisControleCTe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repositorioPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadoBaseCalculo = repositorioPedagioEstadoBaseCalculo.BuscarTodos();

                var retorno = new
                {
                    RegrasPedagioPorEstado = (from obj in pedagioEstadoBaseCalculo select new { Estado = obj.Estado.Sigla, IncluirBaseCalculoICMS = obj.IncluiBaseICMS }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados gerais para controle do CT-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterContainerCargaPedido()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CodigoCargaPedido")), out codigoCargaPedido);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Carga pedido não encontrado.");

                if (cargaPedido != null && cargaPedido.Pedido != null)
                {
                    return new JsonpResult(new
                    {
                        Lacre1 = cargaPedido.Pedido.LacreContainerUm,
                        Lacre2 = cargaPedido.Pedido.LacreContainerDois,
                        Lacre3 = cargaPedido.Pedido.LacreContainerTres,
                        CodigoContainer = cargaPedido.Pedido.Container?.Codigo ?? 0,
                        NumeroContainer = cargaPedido.Pedido.Container?.Numero ?? ""
                    });
                }
                else
                {
                    return new JsonpResult(null);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados de seguro.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private object ObterDetalhesCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                Codigo = cte.Codigo,
                Numero = cte.Numero,
                Status = cte.Status,
                Empresa = new
                {
                    Codigo = cte.Empresa.Codigo,
                    Descricao = cte.Empresa.RazaoSocial + " (" + cte.Empresa.Localidade.DescricaoCidadeEstado + ")"
                },
                Serie = new
                {
                    Codigo = cte.Serie.Codigo,
                    Descricao = cte.Serie.Numero
                },
                TipoPagamento = cte.TipoPagamento,
                TipoServico = cte.TipoServico,
                Tipo = cte.TipoCTE,
                TipoImpressao = cte.TipoImpressao,
                TipoTomador = cte.TipoTomador,
                DataEmissao = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                CFOP = new
                {
                    Codigo = cte.CFOP.Codigo,
                    Descricao = cte.CFOP.CodigoCFOP
                },
                Chave = cte.Chave,
                ProtocoloAutorizacao = cte.Protocolo,
                ProtocoloCancelamentoInutilizacao = cte.ProtocoloCancelamentoInutilizacao,
                JustificativaCancelamento = cte.ObservacaoCancelamento,
                RecebedorRetira = cte.Retira == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                RecebedorRetiraDetalhes = cte.DetalhesRetira,
                LocalidadeEmissao = new
                {
                    Codigo = cte.LocalidadeEmissao?.Codigo ?? cte.Empresa.Localidade.Codigo,
                    Descricao = cte.LocalidadeEmissao?.DescricaoCidadeEstado ?? cte.Empresa.Localidade.DescricaoCidadeEstado
                },
                LocalidadeInicioPrestacao = new
                {
                    Codigo = cte.LocalidadeInicioPrestacao?.Codigo ?? cte.Empresa.Localidade.Codigo,
                    Descricao = cte.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? cte.Empresa.Localidade.DescricaoCidadeEstado
                },
                LocalidadeTerminoPrestacao = new
                {
                    Codigo = cte.LocalidadeTerminoPrestacao?.Codigo ?? cte.Empresa.Localidade.Codigo,
                    Descricao = cte.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? cte.Empresa.Localidade.DescricaoCidadeEstado
                },
                IndicadorTomador = cte.IndicadorIETomador,
                Globalizado = cte.IndicadorGlobalizado,
                cte.TipoModal
            };

            return retorno;
        }

        private object ObterDocumentosCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Documentos
                           select new
                           {
                               obj.Codigo,
                               BaseCalculoICMS = obj.BaseCalculoICMS.ToString("n2"),
                               BaseCalculoICMSST = obj.BaseCalculoICMSST.ToString("n2"),
                               Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                               CFOP = !string.IsNullOrWhiteSpace(obj.CFOP) && obj.CFOP != "0" ? obj.CFOP : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == obj.ChaveNFE)?.FirstOrDefault()?.CFOP : "",
                               Chave = obj.ChaveNFE,
                               DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                               ValorNotaFiscal = obj.Valor.ToString("n2"),
                               ValorICMS = obj.ValorICMS.ToString("n2"),
                               ValorICMSST = obj.ValorICMSST.ToString("n2"),
                               ValorProdutos = obj.ValorProdutos.ToString("n2"),
                               Peso = obj.Peso.ToString("n2"),
                               PIN = obj.PINSuframa,
                               obj.Serie,
                               obj.Numero,
                               Modelo = obj.NumeroModelo,
                               obj.DescricaoCTe,
                               TipoDocumento = obj.NumeroModelo == "55" ? Dominio.Enumeradores.TipoDocumentoCTe.NFe :
                                               obj.NumeroModelo == "00" || obj.NumeroModelo == "99" ? Dominio.Enumeradores.TipoDocumentoCTe.Outros :
                                               Dominio.Enumeradores.TipoDocumentoCTe.NF,
                               NumeroReferenciaEDI = !string.IsNullOrWhiteSpace(obj.NumeroReferenciaEDI) ? obj.NumeroReferenciaEDI : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == obj.ChaveNFE)?.FirstOrDefault()?.NumeroReferenciaEDI : "",
                               PINSuframa = !string.IsNullOrWhiteSpace(obj.PINSuframa) ? obj.PINSuframa : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == obj.ChaveNFE)?.FirstOrDefault()?.PINSUFRAMA : "",
                               NCMPredominante = !string.IsNullOrWhiteSpace(obj.NCMPredominante) ? obj.NCMPredominante : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == obj.ChaveNFE)?.FirstOrDefault()?.NCM : "",
                               NumeroControleCliente = !string.IsNullOrWhiteSpace(obj.NumeroControleCliente) ? obj.NumeroControleCliente : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == obj.ChaveNFE)?.FirstOrDefault()?.NumeroControleCliente : "",
                           }).ToList();

            return retorno;
        }

        private object ObterEntregasSimplificado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.EntregaCTe> entregasCte)
        {
            List<object> retorno = new List<object>();

            if (entregasCte != null)
            {
                foreach (Dominio.Entidades.EntregaCTe entrega in entregasCte)
                {
                    retorno.Add(new
                    {
                        Codigo = entrega.Codigo,
                        CodigoLocalidadeOrigem = entrega.Origem.Codigo,
                        CodigoLocalidadeDestino = entrega.Destino.Codigo,
                        Origem = entrega.Origem.DescricaoCidadeEstado,
                        Destino = entrega.Destino.DescricaoCidadeEstado,
                        ValorFrete = entrega.ValorFrete,
                        ValorPrestacaoServico = entrega.ValorPrestacaoServico,
                        ValorAReceber = entrega.ValorAReceber
                    });
                }
            }

            return retorno;
        }

        private object ObterEntregasSimplificadoDocumentos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.EntregaCTe> entregasCte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<object> retorno = new List<object>();

            if (entregasCte == null || entregasCte.Count == 0)
                return retorno;

            Repositorio.EntregaCTeDocumento repositorioCTeDocumento = new Repositorio.EntregaCTeDocumento(unidadeDeTrabalho);

            List<Dominio.Entidades.EntregaCTeDocumento> listaDocumentosEntregaCTe = repositorioCTeDocumento.BuscarPorCTe(cte.Codigo);

            if (listaDocumentosEntregaCTe.Count == 0)
                return retorno;

            foreach (Dominio.Entidades.EntregaCTe entrega in entregasCte)
            {
                List<Dominio.Entidades.EntregaCTeDocumento> documentosEntregaCTe = listaDocumentosEntregaCTe.FindAll(documentoEntregaCTe => documentoEntregaCTe.EntregaCTe.Codigo == entrega.Codigo);

                foreach (Dominio.Entidades.EntregaCTeDocumento documento in documentosEntregaCTe)
                {
                    retorno.Add(new
                    {
                        CodigoEntrega = entrega.Codigo,
                        CodigoLocalidadeOrigem = entrega.Origem.Codigo,
                        CodigoLocalidadeDestino = entrega.Destino.Codigo,
                        documento.DocumentosCTE.Codigo,
                        BaseCalculoICMS = documento.DocumentosCTE.BaseCalculoICMS.ToString("n2"),
                        BaseCalculoICMSST = documento.DocumentosCTE.BaseCalculoICMSST.ToString("n2"),
                        Descricao = !string.IsNullOrWhiteSpace(documento.DocumentosCTE.Descricao) ? documento.DocumentosCTE.Descricao : string.Empty,
                        CFOP = !string.IsNullOrWhiteSpace(documento.DocumentosCTE.CFOP) && documento.DocumentosCTE.CFOP != "0" ? documento.DocumentosCTE.CFOP : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == documento.DocumentosCTE.ChaveNFE)?.FirstOrDefault()?.CFOP : "",
                        Chave = documento.DocumentosCTE.ChaveNFE,
                        DataEmissao = documento.DocumentosCTE.DataEmissao.ToString("dd/MM/yyyy"),
                        ValorNotaFiscal = documento.DocumentosCTE.Valor.ToString("n2"),
                        ValorICMS = documento.DocumentosCTE.ValorICMS.ToString("n2"),
                        ValorICMSST = documento.DocumentosCTE.ValorICMSST.ToString("n2"),
                        ValorProdutos = documento.DocumentosCTE.ValorProdutos.ToString("n2"),
                        Peso = documento.DocumentosCTE.Peso.ToString("n2"),
                        PIN = documento.DocumentosCTE.PINSuframa,
                        documento.DocumentosCTE.Serie,
                        documento.DocumentosCTE.Numero,
                        Modelo = documento.DocumentosCTE.NumeroModelo,
                        documento.DocumentosCTE.DescricaoCTe,
                        TipoDocumento = documento.DocumentosCTE.NumeroModelo == "55" ? Dominio.Enumeradores.TipoDocumentoCTe.NFe :
                                               documento.DocumentosCTE.NumeroModelo == "00" || documento.DocumentosCTE.NumeroModelo == "99" ? Dominio.Enumeradores.TipoDocumentoCTe.Outros :
                                               Dominio.Enumeradores.TipoDocumentoCTe.NF,
                        NumeroReferenciaEDI = !string.IsNullOrWhiteSpace(documento.DocumentosCTE.NumeroReferenciaEDI) ? documento.DocumentosCTE.NumeroReferenciaEDI : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == documento.DocumentosCTE.ChaveNFE)?.FirstOrDefault()?.NumeroReferenciaEDI : "",
                        PINSuframa = !string.IsNullOrWhiteSpace(documento.DocumentosCTE.PINSuframa) ? documento.DocumentosCTE.PINSuframa : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == documento.DocumentosCTE.ChaveNFE)?.FirstOrDefault()?.PINSUFRAMA : "",
                        NCMPredominante = !string.IsNullOrWhiteSpace(documento.DocumentosCTE.NCMPredominante) ? documento.DocumentosCTE.NCMPredominante : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == documento.DocumentosCTE.ChaveNFE)?.FirstOrDefault()?.NCM : "",
                        NumeroControleCliente = !string.IsNullOrWhiteSpace(documento.DocumentosCTE.NumeroControleCliente) ? documento.DocumentosCTE.NumeroControleCliente : cte.XMLNotaFiscais != null ? cte.XMLNotaFiscais.Where(x => x.Chave == documento.DocumentosCTE.ChaveNFE)?.FirstOrDefault()?.NumeroControleCliente : ""
                    });
                }
            }

            return retorno;
        }

        private object ObterEntregasSimplificadoDocumentosTransporteAnterior(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.EntregaCTe> entregasCte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<object> retorno = new List<object>();

            if (entregasCte == null || entregasCte.Count == 0)
                return retorno;

            Repositorio.EntregaCTeDocumentoTransporteAnterior repositorioCTeDocumentoTransporteAnterior = new Repositorio.EntregaCTeDocumentoTransporteAnterior(unidadeDeTrabalho);

            List<Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior> listaDocumentosTransporteAnteriorEntregaCTe = repositorioCTeDocumentoTransporteAnterior.BuscarPorCTe(cte.Codigo);

            if (listaDocumentosTransporteAnteriorEntregaCTe.Count == 0)
                return retorno;

            foreach (Dominio.Entidades.EntregaCTe entrega in entregasCte)
            {
                List<Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior> documentosTransporteAnteriorEntregaCTe = listaDocumentosTransporteAnteriorEntregaCTe.FindAll(o => o.EntregaCTe.Codigo == entrega.Codigo);

                foreach (Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior documento in documentosTransporteAnteriorEntregaCTe)
                {
                    retorno.Add(new
                    {
                        CodigoEntrega = entrega.Codigo,
                        CodigoLocalidadeOrigem = entrega.Origem.Codigo,
                        CodigoLocalidadeDestino = entrega.Destino.Codigo,
                        documento.DocumentoTransporteAnterior.Codigo,
                        Chave = documento.DocumentoTransporteAnterior.Chave,
                        CodigoEmitente = documento.DocumentoTransporteAnterior.Emissor.CPF_CNPJ_SemFormato,
                        Emitente = documento.DocumentoTransporteAnterior.Emissor.Nome
                    });
                }
            }

            return retorno;
        }

        private object ObterEntregasSimplificadoComponentesPrestacaoServico(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.EntregaCTe> entregasCte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<object> retorno = new List<object>();

            if (entregasCte == null || entregasCte.Count == 0)
                return retorno;

            Repositorio.EntregaCTeComponentePrestacao repositorioEntregaComponentePrestacao = new Repositorio.EntregaCTeComponentePrestacao(unidadeDeTrabalho);

            List<Dominio.Entidades.EntregaCTeComponentePrestacao> listaComponentesPrestacao = repositorioEntregaComponentePrestacao.BuscarPorCTe(cte.Codigo);

            if (listaComponentesPrestacao.Count == 0)
                return retorno;

            foreach (Dominio.Entidades.EntregaCTe entrega in entregasCte)
            {
                List<Dominio.Entidades.EntregaCTeComponentePrestacao> componentesPrestacao = listaComponentesPrestacao.FindAll(componentePrestacao => componentePrestacao.EntregaCTe.Codigo == entrega.Codigo);

                foreach (Dominio.Entidades.EntregaCTeComponentePrestacao componente in componentesPrestacao)
                {
                    retorno.Add(new
                    {
                        componente.Codigo,
                        CodigoLocalidadeOrigem = entrega.Origem.Codigo,
                        CodigoLocalidadeDestino = entrega.Destino.Codigo,
                        CodigoComponente = 0,
                        DescricaoComponente = componente.Nome,
                        IncluirBaseCalculoICMS = componente.IncluiNaBaseDeCalculoDoICMS,
                        IncluirTotalReceber = componente.IncluiNoTotalAReceber,
                        DescontarTotalReceber = false,
                        Valor = componente.Valor.ToString("n2")
                    });
                }
            }

            return retorno;
        }

        private object ObterModalRodoviarioCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                cte.RNTRC,
                PrevisaoEntrega = cte.DataPrevistaEntrega.HasValue ? cte.DataPrevistaEntrega.Value.ToString("dd/MM/yyyy") : string.Empty,
                CIOT = cte.CIOT,
                IndicadorLotacao = cte.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false
            };

            return retorno;
        }

        private object ObterModalAereoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                DataPrevisaoEntrega = cte.DataPrevistaEntrega.HasValue ? cte.DataPrevistaEntrega.Value.ToString("dd/MM/yyyy") : string.Empty,
                cte.NumeroMinuta,
                cte.NumeroOCA,
                cte.Dimensao,
                cte.CodigoTarifa,
                cte.ClasseTarifa,
                ValorTarifa = cte.ValorTarifa?.ToString("n2") ?? 0.ToString("n2"),
            };

            return retorno;
        }

        private object ObterModalAquaviarioCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                ValorPrestacaoAfrmm = cte.ValorPrestacaoAFRMM?.ToString("n2") ?? 0.ToString("n2"),
                ValorAdicionalAfrmm = cte.ValorAdicionalAFRMM?.ToString("n2") ?? 0.ToString("n2"),
                cte.NumeroViagem,
                cte.Direcao,
                Navio = cte.Navio != null ? new { cte.Navio.Codigo, cte.Navio.Descricao } : null
            };

            return retorno;
        }

        private object ObterModalFerroviarioCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                cte.NumeroFluxoFerroviario,
                cte.TipoTrafego,
                ValorFrete = cte.ValorFreteTrafego?.ToString("n2") ?? 0.ToString("n2"),
                cte.ChaveCTeFerroviaOrigem,
                ResponsavelFaturamento = cte.FerroviaResponsavelFaturamento,
                cte.FerroviaEmitente
            };

            return retorno;
        }

        private object ObterModalDutoviarioCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                ValorTarifa = cte.ValorTarifa?.ToString("n6") ?? 0.ToString("n6"),
                DataInicioPrestacaoServico = cte.DataInicioPrestacaoServico.HasValue ? cte.DataInicioPrestacaoServico.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataFimPrestacaoServico = cte.DataFimPrestacaoServico.HasValue ? cte.DataFimPrestacaoServico.Value.ToString("dd/MM/yyyy") : string.Empty
            };

            return retorno;
        }

        private object ObterModalMultimodalCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                cte.NumeroCOTM,
                cte.IndicadorNegociavel
            };

            return retorno;
        }

        private object ObterInformacaoModalCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                PortoOrigem = cte.PortoOrigem != null ? new { cte.PortoOrigem.Codigo, cte.PortoOrigem.Descricao } : null,
                PortoPassagemUm = cte.PortoPassagemUm != null ? new { cte.PortoPassagemUm.Codigo, cte.PortoPassagemUm.Descricao } : null,
                PortoPassagemDois = cte.PortoPassagemDois != null ? new { cte.PortoPassagemDois.Codigo, cte.PortoPassagemDois.Descricao } : null,
                PortoPassagemTres = cte.PortoPassagemTres != null ? new { cte.PortoPassagemTres.Codigo, cte.PortoPassagemTres.Descricao } : null,
                PortoPassagemQuatro = cte.PortoPassagemQuatro != null ? new { cte.PortoPassagemQuatro.Codigo, cte.PortoPassagemQuatro.Descricao } : null,
                PortoPassagemCinco = cte.PortoPassagemCinco != null ? new { cte.PortoPassagemCinco.Codigo, cte.PortoPassagemCinco.Descricao } : null,
                PortoDestino = cte.PortoDestino != null ? new { cte.PortoDestino.Codigo, cte.PortoDestino.Descricao } : null,
                TerminalOrigem = cte.TerminalOrigem != null ? new { cte.TerminalOrigem.Codigo, cte.TerminalOrigem.Descricao } : null,
                TerminalDestino = cte.TerminalDestino != null ? new { cte.TerminalDestino.Codigo, cte.TerminalDestino.Descricao } : null,
                Viagem = cte.Viagem != null ? new { cte.Viagem.Codigo, cte.Viagem.Descricao } : null,
                cte.NumeroControle,
                cte.NumeroBooking,
                cte.DescricaoCarrier,
                cte.TipoPropostaFeeder,
                cte.OcorreuSinistroAvaria
            };

            return retorno;
        }

        private object ObterMotoristasCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Motoristas
                           select new
                           {
                               obj.Codigo,
                               CPF = obj.CPFMotorista,
                               Nome = obj.NomeMotoristaCTe
                           }).ToList();

            return retorno;
        }

        private object ObterVeiculosCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Veiculos
                           select new
                           {
                               Codigo = obj.Codigo,
                               CodigoVeiculo = obj.Veiculo.Codigo,
                               Placa = obj.Veiculo.Placa,
                               Estado = obj.Veiculo.Estado.Sigla,
                               RENAVAM = obj.Veiculo.Renavam,
                               Propriedade = obj.Veiculo.DescricaoTipo,
                               Rodado = obj.Veiculo.DescricaoTipoRodado,
                               Carroceria = obj.Veiculo.DescricaoTipoCarroceria
                           }).ToList();

            return retorno;
        }

        private object ObterInformacaoCargaCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                ValorTotalCarga = cte.ValorTotalMercadoria,
                ProdutoPredominante = cte.ProdutoPredominanteCTe,
                ValorCargaAverbacao = cte.ValorCarbaAverbacao,
                Container = cte.Container,
                DataEntregaContainer = cte.DataPrevistaContainer.HasValue ? cte.DataPrevistaContainer.Value.ToString("dd/MM/yyyy") : string.Empty,
                NumeroLacreContainer = cte.LacreContainer,
                OutrasCaracteristicas = cte.OutrasCaracteristicasDaCargaCTe,
                CaracteristicaAdicionalTransporte = cte.CaracteristicaTransporteCTe,
                CaracteristicaAdicionalServico = cte.CaracteristicaServicoCTe
            };

            return retorno;
        }

        private object ObterQuantidadesCargaCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unidadeDeTrabalho);

            var retorno = (from obj in cte.QuantidadesCarga
                           select new
                           {
                               obj.Codigo,
                               CodigoUnidadeMedida = repUnidadeMedida.BuscarPorCodigoUnidade(obj.UnidadeMedida)?.Codigo ?? 0,
                               UnidadeMedida = obj.DescricaoUnidadeMedida,
                               TipoMedida = obj.Tipo,
                               Quantidade = obj.Quantidade.ToString("n4")
                           }).ToList();

            return retorno;
        }

        private object ObterSegurosCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Seguros
                           select new
                           {
                               Codigo = obj.Codigo,
                               CodigoResponsavel = obj.Tipo,
                               Responsavel = obj.DescricaoTipo,
                               Seguradora = obj.NomeSeguradora,
                               CNPJSeguradora = obj.CNPJSeguradora,
                               NumeroApolice = obj.NumeroApolice,
                               NumeroAverbacao = obj.NumeroAverbacao,
                               Valor = obj.Valor.ToString("n2")
                           }).ToList();

            return retorno;
        }

        private object ObterDocumentosTransporteAnteriorPapelCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.DocumentosTransporteAnterior
                           where string.IsNullOrEmpty(obj.Chave)
                           select new
                           {
                               Codigo = obj.Codigo,
                               Numero = obj.Numero,
                               Serie = obj.Serie,
                               DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                               Tipo = obj.Tipo,
                               CodigoEmitente = obj.Emissor.CPF_CNPJ_SemFormato,
                               Emitente = obj.Emissor.Nome
                           }).ToList();

            return retorno;
        }

        private object ObterDocumentosTransporteAnteriorEletronicoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.DocumentosTransporteAnterior
                           where !string.IsNullOrEmpty(obj.Chave)
                           select new
                           {
                               Codigo = obj.Codigo,
                               Chave = obj.Chave,
                               CodigoEmitente = obj.Emissor.CPF_CNPJ_SemFormato,
                               Emitente = obj.Emissor.Nome
                           }).ToList();

            return retorno;
        }

        private object ObterObservacoesCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            return new
            {
                ObservacaoGeral = cte.ObservacoesGerais
            };
        }

        private object ObterObservacoesFiscoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.ObservacoesFisco
                           select new
                           {
                               obj.Codigo,
                               obj.Descricao,
                               obj.Identificador
                           }).ToList();

            return retorno;
        }

        private object ObterObservacoesContribuinteCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.ObservacoesContribuinte
                           select new
                           {
                               obj.Codigo,
                               obj.Descricao,
                               obj.Identificador
                           }).ToList();

            return retorno;
        }

        private object ObterTotaisServicoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = new
            {
                ValorFrete = cte.ValorFrete,
                ValorPrestacaoServico = cte.ValorPrestacaoServico,
                ValorReceber = cte.ValorAReceber,
                IncluirICMSFrete = cte.IncluirICMSNoFrete,
                PercentualICMSIncluir = cte.PercentualICMSIncluirNoFrete,
                ICMS = cte.CSTICMS,
                BaseCalculoICMS = cte.BaseCalculoICMS,
                AliquotaICMS = cte.AliquotaICMS.ToString("n2"),
                ValorICMS = cte.ValorICMS,
                PercentualReducaoBaseCalculoICMS = cte.PercentualReducaoBaseCalculoICMS,
                ValorCredito = cte.ValorPresumido,
                ExibirNaDACTE = cte.ExibeICMSNaDACTE,
                BaseCalculoIBSUF = cte.BaseCalculoIBSCBS,
                AliquotaIBSUF = cte.AliquotaIBSEstadual,
                ValorBrutoTributoIBSUF = cte.ValorIBSEstadual,
                PercentualReducaoIBSUF = cte.PercentualReducaoIBSEstadual,
                AliquotaEfetivaIBSUF = cte.AliquotaIBSEstadualEfetiva,
                ValorTributoIBSUF = cte.ValorIBSEstadual,
                BaseCalculoIBSMunicipal = cte.BaseCalculoIBSCBS,
                AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal,
                ValorBrutoTributoIBSMunicipal = cte.ValorIBSMunicipal,
                PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal,
                AliquotaEfetivaIBSMunicipal = cte.AliquotaIBSMunicipalEfetiva,
                ValorTributoIBSMunicipal = cte.ValorIBSMunicipal,
                BaseCalculoCBS = cte.BaseCalculoIBSCBS,
                AliquotaCBS = cte.AliquotaCBS,
                ValorBrutoTributoCBS = cte.ValorCBS,
                PercentualReducaoCBS = cte.PercentualReducaoCBS,
                AliquotaEfetivaCBS = cte.AliquotaCBSEfetiva,
                ValorTributoCBS = cte.ValorCBS,
                CodigoClassificacaoTributaria = cte.OutrasAliquotas?.CodigoClassificacaoTributaria ?? cte.ClassificacaoTributariaIBSCBS ?? string.Empty,
            };

            return retorno;
        }

        private object ObterComponentesPrestacaoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

            if (cargaCTe != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTesComponentesFrete = repCargaCTeComponentesFrete.BuscarPorSemComposicaoFreteLiquidoCargaCTe(cargaCTe.Codigo);
                var retorno = (from obj in cargaCTesComponentesFrete
                               select new
                               {
                                   obj.Codigo,
                                   CodigoComponente = obj.ComponenteFrete != null ? obj.ComponenteFrete.Codigo : 0,
                                   DescricaoComponente = obj.ComponenteFrete != null ? obj.ComponenteFrete.Descricao : obj.DescricaoComponente,
                                   IncluirBaseCalculoICMS = obj.IncluirBaseCalculoICMS,
                                   IncluirTotalReceber = true,
                                   DescontarTotalReceber = obj.DescontarValorTotalAReceber,
                                   Valor = obj.ValorComponente.ToString("n2")
                               }).ToList();

                return retorno;
            }
            else
            {
                var retorno = (from obj in cte.ComponentesPrestacao
                               select new
                               {
                                   obj.Codigo,
                                   CodigoComponente = 0,
                                   DescricaoComponente = obj.Nome,
                                   IncluirBaseCalculoICMS = obj.IncluiNaBaseDeCalculoDoICMS,
                                   IncluirTotalReceber = obj.IncluiNoTotalAReceber,
                                   DescontarTotalReceber = false,
                                   Valor = obj.Valor.ToString("n2")
                               }).ToList();

                return retorno;
            }
        }

        private object ObterProdutosPerigososCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.ProdutosPerigosos
                           select new
                           {
                               obj.Codigo,
                               obj.ClasseRisco,
                               obj.NumeroONU,
                               Nome = obj.NomeApropriado,
                               obj.Quantidade,
                               QuantidadeTipoVolume = obj.Volumes,
                               GrupoEmbalagem = obj.Grupo,
                               obj.PontoFulgor,
                               QuantidadeTotal = obj.QuantidadeTotal?.ToString("n4") ?? 0.ToString("n4"),
                               obj.UnidadeDeMedida
                           }).ToList();

            return retorno;
        }

        private object ObterBalsasCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Balsas
                           select new
                           {
                               obj.Codigo,
                               Balsa = obj.Descricao
                           }).ToList();

            return retorno;
        }

        private object ObterContainersCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Containers
                           select new
                           {
                               obj.Codigo,
                               Container = obj.Container?.Codigo ?? 0,
                               DescricaoContainer = obj.Container?.Descricao ?? string.Empty,
                               obj.Lacre1,
                               obj.Lacre2,
                               obj.Lacre3
                           }).ToList();

            return retorno;
        }

        private object ObterContainerDocumentosCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeContainerDocumento repContainerDocumentoCTe = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
            List<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> documentos = repContainerDocumentoCTe.BuscarPorCTe(cte.Codigo);

            var retorno = (from obj in documentos
                           select new
                           {
                               obj.Codigo,
                               CodigoContainer = obj.ContainerCTE.Codigo,
                               CodigoTipoDocumento = obj.TipoDocumento.ToString("D"),
                               TipoDocumento = Dominio.Enumeradores.TipoDocumentoCTeHelper.ObterDescricao(obj.TipoDocumento),
                               obj.Serie,
                               obj.Numero,
                               obj.Chave,
                               UnidadeMedidaRateada = obj.UnidadeMedidaRateada.ToString("n2")
                           }).ToList();

            return retorno;
        }

        private object ObterInformacaoManuseiosCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.InformacoesManuseio
                           select new
                           {
                               obj.Codigo,
                               CodigoInformacaoManuseio = obj.InformacaoManuseio.ToString("D").ToInt(),
                               DescricaoInformacaoManuseio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.InformacaoManuseioHelper.ObterDescricao(obj.InformacaoManuseio)
                           }).ToList();

            return retorno;
        }

        private object ObterFerroviasCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Ferrovias
                           select new
                           {
                               obj.Codigo,
                               Ferrovia = obj.Ferrovia.CPF_CNPJ.ToString(),
                               DescricaoFerrovia = obj.Ferrovia.Descricao
                           }).ToList();

            return retorno;
        }

        private object ObterCTeSubstituicao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosAnulacaoCTE repDocAnulacaoCTe = new Repositorio.DocumentosAnulacaoCTE(unitOfWork);
            Dominio.Entidades.DocumentosAnulacaoCTE documentoAnulacao = repDocAnulacaoCTe.BuscarPorCTe(cte.Codigo);

            if (documentoAnulacao != null)
            {
                var retorno = new
                {
                    ChaveCTeSubstituido = cte.ChaveCTESubComp,
                    documentoAnulacao.ContribuinteICMS,
                    documentoAnulacao.Tipo,
                    documentoAnulacao.Chave,
                    documentoAnulacao.Numero,
                    documentoAnulacao.Serie,
                    documentoAnulacao.Subserie,
                    DataEmissao = documentoAnulacao.DataEmissao.HasValue ? documentoAnulacao.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Valor = documentoAnulacao.Valor.ToString("n2"),
                    ModeloDocumentoFiscal = documentoAnulacao.ModeloDocumentoFiscal != null ? new { documentoAnulacao.ModeloDocumentoFiscal.Codigo, documentoAnulacao.ModeloDocumentoFiscal.Descricao } : null,
                    Emitente = documentoAnulacao.Emitente != null ? new { documentoAnulacao.Emitente.Codigo, documentoAnulacao.Emitente.Descricao } : null
                };

                return retorno;
            }
            else
                return null;
        }

        private object ObterCTeAnulacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
            {
                return new
                {
                    ChaveCTeAnulado = cte.ChaveCTESubComp,
                    DataAnulacao = cte.DataAnulacao.HasValue ? cte.DataAnulacao.Value.ToString("dd/MM/yyyy") : string.Empty
                };
            }
            else
                return null;
        }

        private object ObterCTeComplementar(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
            {
                return new
                {
                    ChaveCTeComplementado = cte.ChaveCTESubComp
                };
            }
            else
                return null;
        }

        #endregion
    }
}
