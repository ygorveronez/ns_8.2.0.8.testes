using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.CRT;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Documentos
{
    [CustomAuthorize(new string[] { "DownloadPreDacte", "DownloadPreXML" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaPreCTeController : BaseController
    {
        #region Construtores

        public CargaPreCTeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaPreCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);

                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool ctesSubContratacaoFilialEmissora = false;
                bool ctesSemSubContratacaoFilialEmissora = false;

                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out ctesSemSubContratacaoFilialEmissora);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPreCTE", false);
                grid.AdicionarCabecalho("CteEnviado", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("SerieCteEnviado", false);
                grid.AdicionarCabecalho("CT-e Enviado", "NumeroCteEnviado", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modelo", "Modelo", 8, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Status Aprovação", "StatusAprovacao", 8, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "CTEReferenciado")
                    propOrdenacao = "CargaCTe.CTe.Numero";
                else
                    propOrdenacao = "PreCTe." + propOrdenacao;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarPreCTes(codigoCarga, 0, ctesSubContratacaoFilialEmissora, ctesSemSubContratacaoFilialEmissora, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento> gestaoDocumentosCTe = cargaCTes.Count > 0 ? repositorioGestaoDocumento.BuscarPorCargasCTe(cargaCTes.Select(obj => obj.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>();

                grid.setarQuantidadeTotal(repCargaCTe.ContarConsultaPreCTes(codigoCarga, 0, ctesSubContratacaoFilialEmissora, ctesSemSubContratacaoFilialEmissora));
                var lista = (from obj in cargaCTes select RetornarPreCTe(obj, (from gestaoDocumento in gestaoDocumentosCTe where gestaoDocumento.CargaCTe.Codigo == obj.Codigo select gestaoDocumento).FirstOrDefault())).ToList();
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                if (carga == null)
                    return new JsonpResult(false, false, "Carga não encontrada");

                Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI integracao = repControleIntegracao.BuscarPorCodigo(codigo);
                if (integracao == null)
                    return new JsonpResult(false, false, "EDI não encontrado, atualize a página e tente novamente.");

                Dominio.Entidades.LayoutEDI layoutEDI = null;
                if (integracao.LayoutEDI != null)
                {
                    //Integracao carga EDI Mattel, possui layout EDI e deve ser criado arquivo Notfis
                    layoutEDI = integracao.LayoutEDI;
                    Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = Servicos.Embarcador.Carga.CargaIntegracaoEDI.ConverterCargaEmNotFis(carga, integracao.LayoutEDI, unitOfWork);

                    Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, null);
                    MemoryStream msNotFis = serGeracaoEDI.GerarArquivoRecursivo(notfis);
                    StreamReader readerProcessados = new StreamReader(msNotFis);
                    byte[] retorno = msNotFis.ToArray();
                    msNotFis.Dispose();
                    return Arquivo(retorno, "text/txt", integracao.NomeArquivo);

                }
                else
                {

                    layoutEDI = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS).FirstOrDefault();
#if DEBUG
                    layoutEDI = repLayoutEDI.BuscarPorCodigo(13026);
#endif
                    string caminho = "";

                    if (integracao.ArquivoImportacaoPedido)
                        caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "ImportacaoArquivos", typeof(Dominio.ObjetosDeValor.ImportacaoArquivo.Importado).Name });
                    else
                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados");

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, integracao.GuidArquivo);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        return new JsonpResult(false, false, "EDI não encontrado, atualize a página e tente novamente.");

                    byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                    if (data == null)
                        return new JsonpResult(false, false, "EDI não encontrado, atualize a página e tente novamente.");

                    if (integracao.ArquivoImportacaoPedido)
                    {
                        string extensao = System.IO.Path.GetExtension(integracao.NomeArquivo).ToLower();
                        return Arquivo(data, "application/octet-stream", integracao.NomeArquivo);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, System.Text.Encoding.GetEncoding("iso-8859-1"));
                        Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = leituraEDI.GerarNotasFis();

                        Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, null);

                        //Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis novoNotifis = FiltrarEDIPelaCarga(carga, notfis);

                        MemoryStream msNotFis = serGeracaoEDI.GerarArquivoRecursivo(notfis);
                        StreamReader readerProcessados = new StreamReader(msNotFis);
                        byte[] retorno = msNotFis.ToArray();
                        ms.Dispose();
                        msNotFis.Dispose();
                        return Arquivo(retorno, "text/txt", integracao.NomeArquivo);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        [AllowAuthenticate]
        public async Task<IActionResult> DownloadPreXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPreCTe;
                int.TryParse(Request.Params("CodigoPreCTe"), out codigoPreCTe);

                if (codigoPreCTe > 0)
                {
                    Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = repPreCTe.BuscarPorCodigo(codigoPreCTe);

                    if (preCte != null)
                    {
                        Servicos.PreCTe svcPreCTe = new Servicos.PreCTe(unitOfWork);

                        byte[] data = System.Text.Encoding.Unicode.GetBytes(svcPreCTe.BuscarXMLPreCte(preCte));

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat("pre_cte_" + preCte.Codigo, ".xml"));
                        }
                    }
                }

                return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPreCTe = Request.GetIntParam("CodigoPreCTe");
                int codigoCargaCTe = Request.GetIntParam("Codigo");
                bool substituicaoCTe = Request.GetBoolParam("SubstituicaoCTe");

                Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = repPreCTe.BuscarPorCodigo(codigoPreCTe);

                if (preCte.ModeloDocumentoFiscal != null && preCte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
                    return new JsonpResult(false, true, "Modelo do documento não é um CT-e.");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");

                Servicos.DTO.CustomFile file = files[0];
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                if (!extensao.Equals(".xml"))
                    return new JsonpResult(false, true, "A extensão do arquivo é inválida.");

                unitOfWork.Start();

                Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(file.InputStream, preCte, cargaCTe, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, substituicaoCTe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.Carga, null, substituicaoCTe ? "Reenviou XML do Pre CT-e" : "Enviou XML do Pre CT-e", unitOfWork);

                file.InputStream.Dispose();

                if (retorno.Length != 0)
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(false, true, retorno);
                }

                if (serCargaPreCTe.VerificarEnviouTodosDocumentos(unitOfWork, cargaCTe.Carga, TipoServicoMultisoftware, ConfiguracaoEmbarcador))
                {
                    if (cargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                        serCargaPreCTe.SetarDocumentoOriginario(cargaCTe.CTe, unitOfWork);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = cargaCTe.CTe != null ? repositorioGestaoDocumento.BuscarPorCTe(cargaCTe.CTe.Codigo) : null;

                    new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(cargaCTe.Carga.Codigo);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(RetornarPreCTe(cargaCTe, gestaoDocumento));
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarCTesParaPreCTe()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCarga = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codCarga);
                Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

                        RetornoArquivo retornoArquivo = new RetornoArquivo();
                        retornoArquivo.nome = file.FileName;
                        retornoArquivo.processada = true;
                        retornoArquivo.mensagem = "";
                        if (extensao.Equals(".xml"))
                        {
                            try
                            {
                                await unitOfWork.StartAsync();
                                var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                                if (objCTe != null)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe preCTe = serCargaPreCTe.BuscarCargaPreCTe(objCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, carga, unitOfWork);
                                    if (preCTe.CargaCTe == null)
                                    {
                                        retornoArquivo.processada = false;
                                        retornoArquivo.mensagem = "Não foi localizado nenhuma nota compativel com os documentos informados no CT-e para essa carga.";
                                        if (objCTe.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                                        {
                                            file.InputStream.Position = 0;
                                            MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                                            servicoGestaoDocumento.CriarCTe(cteProc, file.InputStream);
                                            new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(preCTe?.CargaCTe?.Carga?.Codigo ?? 0);
                                            await unitOfWork.CommitChangesAsync();
                                        }
                                        else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                                        {
                                            file.InputStream.Position = 0;
                                            MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                                            servicoGestaoDocumento.CriarCTe(cteProc, file.InputStream);
                                            new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(preCTe?.CargaCTe?.Carga?.Codigo ?? 0, preCTe?.CargaCTe ?? null);
                                            await unitOfWork.CommitChangesAsync();
                                        }
                                        else
                                        {
                                            await unitOfWork.RollbackAsync();
                                        }
                                    }
                                    else
                                    {

                                        file.InputStream.Position = 0;
                                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(file.InputStream, preCTe.CargaCTe.PreCTe, preCTe.CargaCTe, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);

                                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, preCTe.CargaCTe.Carga, null, "Enviou XML do Pre CT-e", unitOfWork);
                                        await file.InputStream.DisposeAsync();
                                        if (retorno.Length == 0)
                                        {
                                            if (preCTe.CargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                                serCargaPreCTe.SetarDocumentoOriginario(preCTe.CargaCTe.CTe, unitOfWork);

                                            serCargaPreCTe.VerificarEnviouTodosDocumentos(unitOfWork, preCTe.CargaCTe.Carga, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
                                            new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(preCTe?.CargaCTe?.Carga?.Codigo ?? 0, preCTe?.CargaCTe ?? null);
                                            await unitOfWork.CommitChangesAsync();
                                        }
                                        else
                                        {
                                            retornoArquivo.processada = false;
                                            retornoArquivo.mensagem = retorno;
                                            await unitOfWork.CommitChangesAsync();
                                        }
                                    }
                                }
                                else
                                {
                                    retornoArquivo.processada = false;
                                    retornoArquivo.mensagem = "O xml informado não é uma NF-e ou um CT-e, por favor verifique.";
                                    await unitOfWork.RollbackAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                await unitOfWork.RollbackAsync();
                                retornoArquivo.processada = false;
                                retornoArquivo.mensagem = "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido.";
                            }
                            finally
                            {
                                await file.InputStream.DisposeAsync();
                            }
                        }
                        else
                        {
                            if (extensao.Equals(".json"))
                            {
                                IntegracaoCRT integracaoCRT = null;

                                using (StreamReader reader = new StreamReader(file.InputStream))
                                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    integracaoCRT = serializer.Deserialize<Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT>(jsonReader);
                                }

                                Dominio.ObjetosDeValor.WebService.Retorno<bool> retorno = await new Servicos.WebService.CRT.CRT(unitOfWork).InformarCRTPorCargaAsync(integracaoCRT);
                                retornoArquivo.processada = retorno.Status;
                                retornoArquivo.mensagem = retorno.Mensagem;
                            }
                            else
                            {
                                retornoArquivo.processada = false;
                                retornoArquivo.mensagem = "A extensão do arquivo é inválida.";
                            }
                        }
                        retornoArquivos.Add(retornoArquivo);
                    }

                    var dadosRetorno = new
                    {
                        Arquivos = retornoArquivos
                    };

                    bool houveramErros = retornoArquivos.Exists(ra => !ra.processada);

                    return new JsonpResult(
                        dadosRetorno, 
                        !houveramErros,
                        houveramErros ? $"Um ou mais arquivos falharam: {retornoArquivos.Find(ra => !ra.processada).mensagem}" : null
                    );
                }
                else
                {
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar os documentos");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> LiberarSemTodosPreCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if ((carga.ProblemaIntegracaoValePedagio || carga.IntegrandoValePedagio) && !carga.LiberadoComProblemaValePedagio)
                    return new JsonpResult(false, true, "Não é possível liberar a carga pois o vale pedágio está em processo de integração");

                carga.AgImportacaoCTe = false;
                carga.LiberadaSemTodosPreCTes = true;

                repositorioCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Liberou carga sem receber todos Pré CT-e(s)", unitOfWork);

                new Servicos.Embarcador.Hubs.Carga().InformarCargaAtualizada(carga.Codigo, TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar a carga sem receber todos os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadTodosPreCte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Servicos.PreCTe svcPreCTe = new Servicos.PreCTe(unitOfWork);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                if (codigoCarga == 0)
                    return new JsonpResult(false, false, "Não foi possivel encontrar a carga para baixar os pre-ctes");

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repositorioCargaCte.BuscarCargaCtePorCarga(codigoCarga);

                if (cargaCtes.Count == 0 || cargaCtes == null)
                    return new JsonpResult(false, false, "Registros de Pre-Ctes Não encontrados");

                Dictionary<string, byte[]> preCtes = new Dictionary<string, byte[]>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCtes)
                {
                    byte[] data = System.Text.Encoding.Unicode.GetBytes(svcPreCTe.BuscarXMLPreCte(cargaCte.PreCTe));
                    preCtes.Add($"Pre-Cte-{cargaCte.PreCTe.Codigo}.xml", data);
                }

                MemoryStream arquivoComprimido = Utilidades.File.GerarArquivoCompactado(preCtes);

                return Arquivo(arquivoComprimido, "application/zip", "DocumentosPreCte.zip");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadPreviaCusto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                if (codigoCarga == 0)
                    return new JsonpResult(false, false, "Não foi possivel encontrar a carga para baixar os pre-ctes");

                byte[] previaCusto = new Servicos.Embarcador.Carga.Carga(unitOfWork).ObterPreviaCustoCarga(codigoCarga, unitOfWork);

                if (previaCusto == null)
                    return new JsonpResult(false, false, "Não foi possivel gerar previa custos da carga");

                return Arquivo(previaCusto, "text/csv", "PreviaCustos.csv");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarNFSeParaPreCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCargaCTe = Request.GetIntParam("CodigoCargaCTe");
                int codigoPreCTe = Request.GetIntParam("CodigoPreCTe");
                int numero = Request.GetIntParam("Numero");
                int serie = Request.GetIntParam("Serie");
                DateTime dataEmissao = Request.GetDateTimeParam("DataEmissao");
                decimal aliquotaISS = Request.GetDecimalParam("AliquotaISS");
                decimal baseCalculo = Request.GetDecimalParam("BaseCalculo");
                decimal valorISS = Request.GetDecimalParam("ValorISS");
                decimal percentualRetencao = Request.GetDecimalParam("PercentualRetencao");
                decimal valorRetencao = Request.GetDecimalParam("ValorRetencao");
                decimal valorPIS = Request.GetDecimalParam("ValorPIS");
                decimal valorCOFINS = Request.GetDecimalParam("ValorCOFINS");
                decimal valorIR = Request.GetDecimalParam("ValorIR");
                decimal valorCSLL = Request.GetDecimalParam("ValorCSLL");
                decimal valorReceber = Request.GetDecimalParam("ValorReceber");
                string numeroRPS = Request.GetStringParam("NumeroRPS");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                Servicos.PreCTe serPreCTe = new Servicos.PreCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = repPreCTe.BuscarPorCodigo(codigoPreCTe);

                if (preCte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] tipos = Request.TryGetArrayParam<string>("Tipo");

                if (arquivos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (arquivos.Count == 1 && !tipos.Contains("DANFSE"))
                    return new JsonpResult(false, true, "É necessário selecionar o DANFSe para envio.");

                string xml = string.Empty;
                byte[] pdfDanfse = null;
                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    string tipo = i < tipos.Length ? tipos[i] : string.Empty;

                    if (tipo.Equals("XML"))
                    {
                        if (Path.GetExtension(file.FileName).ToLower() != ".xml")
                            return new JsonpResult(false, true, "Arquivo de XML inválido.");

                        // Converte o xml em string
                        List<string> xmlData = new List<string>();
                        StreamReader reader = new StreamReader(file.InputStream);
                        while (!reader.EndOfStream)
                            xmlData.Add(reader.ReadLine());
                        xml = string.Join("", xmlData);
                    }
                    else if (tipo.Equals("DANFSE"))
                    {
                        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                            return new JsonpResult(false, true, "Arquivo de PDF inválido.");

                        // Converte arquivo em bytes
                        var reader = new BinaryReader(file.InputStream);
                        pdfDanfse = reader.ReadBytes((int)file.Length);
                    }
                }

                serPreCTe.GerarNFSPorPreCTe(preCte, cargaCTe, numero, serie, xml, pdfDanfse, dataEmissao, aliquotaISS, baseCalculo, valorISS, percentualRetencao, valorRetencao, valorPIS, valorCOFINS, valorIR, valorCSLL, preCte.ValorFrete, preCte.ValorPrestacaoServico, valorReceber, numeroRPS, observacao, ConfiguracaoEmbarcador, TipoServicoMultisoftware);
                serCargaPreCTe.VerificarEnviouTodosDocumentos(unitOfWork, cargaCTe.Carga, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
                new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(cargaCTe.Carga.Codigo, cargaCTe);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ArgumentNullException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a NFS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis FiltrarEDIPelaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis EDIBase)
        {
            Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = new Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis();

            List<string> codigosCarga = new List<string>();

            if (carga.CodigosAgrupados != null && carga.CodigosAgrupados.Count > 0)
                codigosCarga.AddRange(carga.CodigosAgrupados);
            else
                codigosCarga.Add(carga.CodigoCargaEmbarcador);

            notfis.Emitente = EDIBase.Emitente;
            notfis.Data = EDIBase.Data;
            notfis.Destinatario = EDIBase.Destinatario;
            notfis.Emitente = EDIBase.Emitente;
            notfis.Filler = EDIBase.Filler;
            notfis.Intercambio = EDIBase.Intercambio;
            notfis.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento();
            notfis.CabecalhoDocumento.Filler = EDIBase.CabecalhoDocumento.Filler;
            notfis.CabecalhoDocumento.Embarcadores = new List<Dominio.ObjetosDeValor.EDI.Notfis.Embarcador>();
            notfis.CabecalhoDocumento.IdDocumento = EDIBase.CabecalhoDocumento.IdDocumento;
            notfis.CabecalhoDocumento.IdCTe = EDIBase.CabecalhoDocumento.IdCTe;
            notfis.CabecalhoDocumento.Totais = new Dominio.ObjetosDeValor.EDI.Notfis.Totais();
            notfis.CabecalhoDocumento.Totais.Filler = EDIBase.CabecalhoDocumento.Totais != null ? EDIBase.CabecalhoDocumento.Totais.Filler : "";
            foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador in EDIBase.CabecalhoDocumento.Embarcadores)
            {
                Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcadorNovo = embarcador.Clonar();
                embarcadorNovo.Destinatarios = new List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario>();
                foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario in embarcador.Destinatarios)
                {
                    Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatarioNovo = destinatario.Clonar();
                    destinatarioNovo.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal>();
                    foreach (var nota in destinatario.NotasFiscais)
                    {
                        if (codigosCarga.Contains(nota.CDRemessa))
                        {
                            bool podeAdd = true;
                            if (carga.TipoOperacao != null && carga.TipoOperacao.UtilizarExpedidorComoTransportador)//todo: regra fixa para Danone
                            {
                                if (nota.NumeroOrdem != "1")
                                    podeAdd = false;
                            }
                            else
                            {
                                if (nota.NumeroOrdem != "-")
                                    podeAdd = false;
                            }

                            if (podeAdd)
                            {
                                destinatarioNovo.NotasFiscais.Add(nota);
                                notfis.CabecalhoDocumento.Totais.CubagemTotalNotas += nota.PesoCubagem;
                                notfis.CabecalhoDocumento.Totais.PesoTotal += nota.NFe.PesoBruto;
                                notfis.CabecalhoDocumento.Totais.QuantidadeTotal += nota.NFe.VolumesTotal;
                                notfis.CabecalhoDocumento.Totais.ValorTotal += nota.NFe.Valor;
                                notfis.CabecalhoDocumento.Totais.ValorTotalFrete += nota.ValorCobrado;
                                notfis.CabecalhoDocumento.Totais.ValorTotalSeguro += nota.NFe.ValorSeguro;
                            }
                        }
                    }
                    if (destinatarioNovo.NotasFiscais.Count > 0)
                        embarcadorNovo.Destinatarios.Add(destinatarioNovo);
                }
                if (embarcadorNovo.Destinatarios.Count > 0)
                    notfis.CabecalhoDocumento.Embarcadores.Add(embarcadorNovo);
            }

            return notfis;
        }

        private dynamic RetornarPreCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            var retorno = new
            {
                cargaCTe.Codigo,
                CodigoPreCTE = cargaCTe.PreCTe.Codigo,
                CodigoCTE = cargaCTe.CTe != null ? cargaCTe.CTe.Codigo : 0,
                NumeroCteEnviado = cargaCTe.CTe != null ? cargaCTe.CTe.Numero.ToString() : "",
                SerieCteEnviado = cargaCTe.CTe?.Serie?.Numero.ToString() ?? string.Empty,
                Modelo = cargaCTe.PreCTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao() ?? Dominio.Enumeradores.TipoDocumento.CTe.ObterDescricao(),
                TipoDocumentoEmissao = cargaCTe.PreCTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.CTe,
                CteEnviado = cargaCTe.CTe == null ? false : true,
                Remetente = cargaCTe.PreCTe.Remetente != null ? cargaCTe.PreCTe.Remetente.Nome + "(" + cargaCTe.PreCTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                Destinatario = cargaCTe.PreCTe.Destinatario != null ? cargaCTe.PreCTe.Destinatario.Nome + "(" + cargaCTe.PreCTe.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                Destino = cargaCTe.PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                ValorFrete = cargaCTe.PreCTe.ValorAReceber.ToString("n2"),
                StatusAprovacao = gestaoDocumento != null ? gestaoDocumento.SituacaoGestaoDocumento.ObterDescricao() : string.Empty,
                DT_RowColor = cargaCTe.CTe != null ? "#dff0d8" : "#fcf8e3",
            };
            return retorno;
        }

        #endregion
    }

    public class RetornoArquivo
    {
        public string nome { get; set; }
        public bool processada { get; set; }
        public string mensagem { get; set; }
    }
}
