using Newtonsoft.Json;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.CTe.CartaCorrecao
{
    [CustomAuthorize("CTe/CartaCorrecao", "Cargas/Carga")]
    public class CartaCorrecaoController : BaseController
    {
		#region Construtores

		public CartaCorrecaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCarga = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = new Dominio.Entidades.CartaDeCorrecaoEletronica();

                if (!PreencherEntidade(cce, unitOfWork, out string mensagemErro, true))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }
                if (cce.CTe != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorCTe(cce.CTe.Codigo);
                    if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                        return new JsonpResult(false, true, "A carga se encontra bloqueada.");
                }

                unitOfWork.CommitChanges();

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.EmitirCCe(cce, cce.CTe.Empresa.Codigo, unitOfWork))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cce.CTe, "Gerou carta de correção pelo TMS.", unitOfWork);
                    return new JsonpResult(new { Sucesso = false });
                }

                return new JsonpResult(new { Sucesso = true });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCarga = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigo, true);

                if (cce == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cce.CTe != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorCTe(cce.CTe.Codigo);
                    if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                        return new JsonpResult(false, true, "A carga se encontra bloqueada.");
                }

                unitOfWork.Start();

                if (!PreencherEntidade(cce, unitOfWork, out string mensagemErro, false))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                unitOfWork.CommitChanges();

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.EmitirCCe(cce, cce.CTe.Empresa.Codigo, unitOfWork))
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cce.CTe, "Gerou carta de correção pelo TMS.", unitOfWork);
                    return new JsonpResult(new { Sucesso = false });
                }

                return new JsonpResult(new { Sucesso = true });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigo, false);

                if (cce == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.ItemCCe> itens = repItemCCe.BuscarPorCCe(cce.Codigo);

                return new JsonpResult(new
                {
                    cce.Codigo,
                    CTe = new
                    {
                        cce.CTe.Codigo,
                        cce.CTe.Descricao
                    },
                    cce.Status,
                    DataEmissao = cce.DataEmissao?.ToString("dd/MM/yyyy HH:mm"),
                    cce.MensagemRetornoSefaz,
                    cce.NumeroSequencialEvento,
                    cce.Protocolo,
                    ListaItens = (from obj in itens
                                  select new
                                  {
                                      obj.Codigo,
                                      CampoAlterado = new
                                      {
                                          obj.CampoAlterado.Codigo,
                                          obj.CampoAlterado.Descricao
                                      },
                                      DescricaoCampoAlterado = obj.CampoAlterado.Descricao,
                                      obj.NumeroItemAlterado,
                                      obj.ValorAlterado,
                                      obj.CampoAlterado.IndicadorRepeticao,
                                      obj.CampoAlterado.TipoCampo,
                                      obj.CampoAlterado.QuantidadeCaracteres,
                                      obj.CampoAlterado.QuantidadeDecimais,
                                      obj.CampoAlterado.QuantidadeInteiros

                                  }).ToList()
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                if (cte == null)
                    return new JsonpResult(false, true, "Não foi possível obter os detalhes do CT-e.");

                return new JsonpResult(new
                {
                    CTe = cte.Codigo,
                    NumeroCTe = cte.Descricao,
                    Remetente = cte.Remetente?.Descricao,
                    Destinatario = cte.Destinatario?.Descricao,
                    Tomador = cte.TomadorPagador?.Descricao,
                    Origem = cte.LocalidadeInicioPrestacao?.DescricaoCidadeEstado,
                    Destino = cte.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado,
                    ValorReceber = cte.ValorAReceber.ToString("n2")
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadPDF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCCe = Request.GetIntParam("Codigo");
                byte[] pdf = ReportRequest
                    .WithType(ReportType.CCe)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoCCe", codigoCCe)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(false, false, "Carta de correção não disponível.");

                Repositorio.CartaDeCorrecaoEletronica repositorioCartaCorrecao = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repositorioCartaCorrecao.BuscarPorCodigo(codigoCCe);
                string nomeArquivo = $"CCe_{cce.NumeroSequencialEvento}-CTe_{cce.NumeroCTe}_{cce.SerieCTe}.{"pdf"}";

                return Arquivo(pdf, "application/pdf", nomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "CartaCorrecaoCTe");
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCCe = Request.GetIntParam("Codigo");

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe);

                if (cce != null)
                {
                    Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);

                    byte[] data = svcCCe.ObterXMLAutorizacao(cce, unitOfWork);

                    if (data != null)
                        return Arquivo(data, "text/xml", $"{cce.CTe.Chave}_{cce.NumeroSequencialEvento}-procCCe.xml");
                }

                return new JsonpResult(false, true, "XML não encontrado.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> SincronizarDocumentoEmProcessamento()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

                int codigoCCe;
                int.TryParse(Request.Params("Codigo"), out codigoCCe);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe, false);

                if (cce != null)
                {
                    string retorno = "";
                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    cce = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cce.SistemaEmissor).ConsultarCCe(codigoCCe, unitOfWork);
                    if (cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                        retorno = "Não foi possível efetuar a sincronização do documento.";

                    if (string.IsNullOrWhiteSpace(retorno))
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cce, null, "Documento sincronizado.", unitOfWork);
                        return new JsonpResult(true);
                    }
                    else
                        return new JsonpResult(false, true, retorno);
                }
                else
                {
                    return new JsonpResult(false, "O Documento informado não foi localizado");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o Documento.");
            }
        }

        #endregion

        #region Métodos Privados

        private bool PreencherEntidade(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, bool inserindo)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

            if (cce.Codigo > 0 && cce.Status != Dominio.Enumeradores.StatusCCe.EmDigitacao && cce.Status != Dominio.Enumeradores.StatusCCe.Rejeicao)
            {
                mensagemErro = "A situação da carta de correção não permite que a mesma seja alterada.";
                return false;
            }

            int codigoCTe = Request.GetIntParam("CTe");

            DateTime dataEmissao = Request.GetDateTimeParam("DataEmissao");

            if (cce.Codigo == 0)
            {
                cce.CTe = repCTe.BuscarPorCodigo(codigoCTe);
                if (cce.CTe == null)
                {
                    mensagemErro = "CT-e não encontrado.";
                    return false;
                }
            }

            if (cce.CTe.Status != "A")
            {
                mensagemErro = "É necessário que o CT-e esteja autorizado para gerar uma carta de correção.";
                return false;
            }

            cce.DataEmissao = dataEmissao;
            cce.Status = Dominio.Enumeradores.StatusCCe.Pendente;

            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;

            if (cce.Codigo > 0)
            {
                historicoObjeto = repCCe.Atualizar(cce);
            }
            else
            {
                cce.NumeroSequencialEvento = repCCe.BuscarUltimoNumeroSequencial(cce.CTe.Codigo) + 1;
                repCCe.Inserir(cce);
            }

            return SalvarItensCCe(cce, unitOfWork, historicoObjeto, out mensagemErro, true, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);
        }

        private bool SalvarItensCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, out string mensagemErro, bool inserindo, bool operacaoMultimodal)
        {
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);
            Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

            dynamic itensCCe = JsonConvert.DeserializeObject<dynamic>(Request.Params("Itens"));

            if (itensCCe == null || itensCCe.Count <= 0)
            {
                mensagemErro = "É necessário informar ao menos um campo para emitir a carta de correção.";
                return false;
            }

            List<Dominio.Entidades.ItemCCe> itensRegistrados = repItemCCe.BuscarPorCCe(cce.Codigo);

            if (itensRegistrados.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var item in itensCCe)
                    codigos.Add((int)item.Codigo);

                List<Dominio.Entidades.ItemCCe> itensDeletar = (from obj in itensRegistrados where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < itensDeletar.Count; i++)
                    repItemCCe.Deletar(itensDeletar[i], Auditado, historicoObjeto);
            }

            if (inserindo && operacaoMultimodal)
            {
                //Buscar alteraões anteriores
                Dominio.Entidades.CartaDeCorrecaoEletronica ultimaCarga = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cce.CTe.Codigo);
                if (ultimaCarga != null)
                {
                    List<Dominio.Entidades.ItemCCe> itensAnteriores = repItemCCe.BuscarPorCCe(ultimaCarga.Codigo);
                    if (itensAnteriores != null && itensAnteriores.Count > 0)
                    {
                        foreach (var item in itensAnteriores)
                        {
                            Dominio.Entidades.ItemCCe itemCCe = new Dominio.Entidades.ItemCCe();

                            itemCCe.CCe = cce;
                            itemCCe.CampoAlterado = item.CampoAlterado;
                            itemCCe.NumeroItemAlterado = item.NumeroItemAlterado;
                            itemCCe.ValorAlterado = item.ValorAlterado;

                            repItemCCe.Inserir(itemCCe);
                        }
                    }
                }
            }

            foreach (var item in itensCCe)
            {
                Dominio.Entidades.ItemCCe itemCCe = null;

                int codigo = 0;

                if (item.Codigo != null && int.TryParse((string)item.Codigo, out codigo))
                    itemCCe = repItemCCe.BuscarPorCodigo(codigo, true);

                if (itemCCe == null)
                    itemCCe = new Dominio.Entidades.ItemCCe();

                itemCCe.CCe = cce;
                int codigoCampoAlterado = 0;
                try
                {
                    int.TryParse((string)item.CampoAlterado, out codigoCampoAlterado);
                }
                catch
                {
                    if (codigoCampoAlterado == 0)
                        int.TryParse((string)item.CampoAlterado.Codigo, out codigoCampoAlterado);
                }
                itemCCe.CampoAlterado = repCampoCCe.BuscarPorCodigo(codigoCampoAlterado);
                itemCCe.NumeroItemAlterado = (int)item.NumeroItemAlterado;
                itemCCe.ValorAlterado = item.ValorAlterado;

                if (itemCCe.Codigo > 0)
                    repItemCCe.Atualizar(itemCCe, Auditado, historicoObjeto);
                else
                    repItemCCe.Inserir(itemCCe, Auditado, historicoObjeto);
            }

            mensagemErro = string.Empty;
            return true;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CTe");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Nº Evento", "NumeroSequencialEvento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoStatus", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "MensagemRetornoSEFAZ", 49, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("HabilitarSincronizarDocumento", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

                int totalRegistros = repCCe.ContarConsulta(codigoCTe);
                List<Dominio.Entidades.CartaDeCorrecaoEletronica> cces = totalRegistros > 0 ? repCCe.Consultar(codigoCTe, parametrosConsulta) : new List<Dominio.Entidades.CartaDeCorrecaoEletronica>();


                var retorno = (from obj in cces
                               select new
                               {
                                   obj.Codigo,
                                   obj.Status,
                                   obj.NumeroSequencialEvento,
                                   DataEmissao = obj.DataEmissao?.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoStatus,
                                   MensagemRetornoSEFAZ = obj.MensagemStatus != null ? string.Concat(obj.MensagemStatus.CodigoDoErro, " - ", obj.MensagemStatus.MensagemDoErro) : obj.MensagemRetornoSefaz,
                                   HabilitarSincronizarDocumento = obj.Status == Dominio.Enumeradores.StatusCCe.Enviado && obj.DataEmissao != null && (System.DateTime.Now.AddMinutes(-30) > obj.DataEmissao) && (obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech || (obj.SistemaEmissor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoStatus")
                return "Status";

            return propriedadeOrdenar;
        }


        private Dominio.ObjetosDeValor.Embarcador.CTe.CTe ProcessarXMLCTe(dynamic objCTe, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);;
                if (objCTe.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                    return serCte.ConverterProcCTeParaCTe(cteProc);
                }
                else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                    return serCte.ConverterProcCTeParaCTe(cteProc);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        #endregion
    }
}
