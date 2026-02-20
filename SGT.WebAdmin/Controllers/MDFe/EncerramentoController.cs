using Dominio.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.MDFe
{
    [CustomAuthorize("MDFe/Encerramento")]
    public class EncerramentoController : BaseController
    {
        #region Construtores

        public EncerramentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.Enumeradores.StatusMDFe? situacao = null;
                Dominio.Enumeradores.StatusMDFe situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoVeiculo, codigoTransportador, numeroInicial, numeroFinal;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);

                string ufCarregamento = Request.Params("EstadoCarregamento");
                string ufDescarregamento = Request.Params("EstadoDescarregamento");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoTransportador = Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Estado de Carregamento", "EstadoCarregamento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Estado de Descarregamento", "EstadoDescarregamento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSefaz", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Serie")
                    propOrdena = "Serie.Numero";
                else if (propOrdena == "EstadoCarregamento")
                    propOrdena = "EstadoCarregamento.Sigla";
                else if (propOrdena == "EstadoDescarregamento")
                    propOrdena = "EstadoDescarregamento.Sigla";

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;

                int rowCount = repMDFe.ContarConsulta(codigoTransportador, numeroInicial, numeroFinal, dataInicial, dataFinal, situacao, ufCarregamento, ufDescarregamento, veiculo?.Placa);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

                if (rowCount > 0)
                    mdfes = repMDFe.Consultar(codigoTransportador, numeroInicial, numeroFinal, dataInicial, dataFinal, situacao, ufCarregamento, ufDescarregamento, veiculo?.Placa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(rowCount);

                var retorno = (from obj in mdfes
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.Numero.ToString(),
                                   Serie = obj.Serie.Numero.ToString(),
                                   DataEmissao = obj.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                   Veiculo = obj.Veiculos.FirstOrDefault()?.Placa ?? string.Empty,
                                   EstadoCarregamento = obj.EstadoCarregamento.Sigla + " - " + obj.EstadoCarregamento.Nome,
                                   EstadoDescarregamento = obj.EstadoDescarregamento.Sigla + " - " + obj.EstadoDescarregamento.Nome,
                                   Situacao = obj.DescricaoStatus,
                                   RetornoSefaz = obj.MensagemStatus?.MensagemDoErro ?? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz),
                                   obj.Status,
                                   DT_RowColor = ObterRowColor(obj),
                                   DT_FontColor = ObterFontColor(obj)
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Encerrar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, DateTime.Now, unidadeTrabalho))
                    {
                        mdfe.Log = "Encerrado manualmente pelo usuário " + this.Usuario.Nome + " às " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        repMDFe.Atualizar(mdfe);

                        svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, DateTime.Now, mdfe.Empresa, mdfe.Empresa.Localidade, mdfe.Log, unidadeTrabalho);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Encerramento manual solicitado", unidadeTrabalho);
                    }

                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não é possível encerrar um MDF-e na situação " + mdfe.DescricaoStatus + ".");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

                int.TryParse(Request.Params("MDFe"), out int codigoMDFe);
                string justificativa = Request.Params("Justificativa") ?? string.Empty;
                DateTime? dataCancelamento = null;
                if (DateTime.TryParse(Request.Params("DataHora"), out DateTime dataCancelamentoAux))
                    dataCancelamento = dataCancelamentoAux;

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (dataCancelamento == null)
                    return new JsonpResult(false, true, "Data Hora é obrigatório.");

                if (justificativa.Length < 20)
                    return new JsonpResult(false, true, "Justificativa de ter no mínimo 20 caracteres.");

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return new JsonpResult(false, true, "Não é possível encerrar um MDF-e na situação " + mdfe.DescricaoStatus + ".");

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).CancelarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, justificativa, unitOfWork, dataCancelamento))
                {
                    mdfe.Log = "Cancelado manualmente pelo usuário " + this.Usuario.Nome + " às " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    repMDFe.Atualizar(mdfe);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Cancelado manualmente", unitOfWork);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            try
            {
                Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado/cancelado/encerrado para o download do DAMDFE.");

                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeDeTrabalho);

                byte[] arquivo = null;

                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (arquivo == null)
                    arquivo = svcDAMDFE.Gerar(mdfe.Codigo);

                if (arquivo != null)
                    return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Chave, ".pdf"));
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o DAMDFE, atualize a página e tente novamente.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do DAMDFE.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Dominio.Enumeradores.TipoXMLMDFe tipo;
                Enum.TryParse(Request.Params("Tipo"), out tipo);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (tipo == Dominio.Enumeradores.TipoXMLMDFe.Autorizacao &&
                    (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado))
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado/encerrado/cancelado para o download do XML.");

                if (tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    return new JsonpResult(false, false, "O MDF-e deve estar cancelado para o download do XML.");

                if (tipo == Dominio.Enumeradores.TipoXMLMDFe.Encerramento && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar encerrado para o download do XML.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXML(mdfe, tipo, unitOfWork);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
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
        public async Task<IActionResult> DownloadEDIFiscal()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe, lacre = 0;
                int.TryParse(Request.Params("CodigoMDFe"), out codigoMDFe);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Lacre")), out lacre);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);


                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado/encerrado para a geração do EDI Fiscal.");

                if (mdfe.Lacres.Count <= 0 && lacre == 0)
                    return new JsonpResult(false, false, "O MDF-e deve possuir ao menos um lacre para a geração do EDI Fiscal.");

                if (mdfe.Lacres.Count <= 0 && lacre >= 0)
                {
                    Repositorio.LacreMDFe repLacreMDFe = new Repositorio.LacreMDFe(unidadeDeTrabalho);
                    Dominio.Entidades.LacreMDFe lacreMDFe = new Dominio.Entidades.LacreMDFe();
                    lacreMDFe.MDFe = mdfe;
                    lacreMDFe.Numero = lacre.ToString();
                    repLacreMDFe.Inserir(lacreMDFe);

                    mdfe.Lacres.Add(lacreMDFe);
                }

                Dominio.Entidades.LayoutEDI layout = mdfe.Empresa.LayoutsEDI.Where(o => o.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL).FirstOrDefault();

                if (layout == null)
                    return new JsonpResult(false, false, "Layout do EDI Fiscal não encontrado.");

                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unidadeDeTrabalho, new Dominio.ObjetosDeValor.MDFe.EDIMDFe(mdfe), layout);

                MemoryStream arquivo = svcEDI.GerarArquivoMDFe();

                return Arquivo(arquivo, "text/plain", string.Concat(mdfe.Chave, " - EDI Fiscal.txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar EDI Fiscal.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarMotoristaMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeTrabalho);

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                int codigo = Request.GetIntParam("Codigo");
                int codigoMotorista = Request.GetIntParam("Motorista");

                DateTime dataEvento = Request.GetDateTimeParam("DataEvento");

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCodigo(codigoMotorista);

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return new JsonpResult(false, true, $"A situação do MDF-e ({mdfe.Status.ObterDescricao()}) não permite a inclusão de motorista.");

                if (motorista == null)
                    return new JsonpResult(false, true, "Motorista não encontrado.");

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).IncluirMotorista(mdfe.Codigo, mdfe.Empresa.Codigo, motorista.CPF, motorista.Nome, unidadeTrabalho, dataEvento))
                    return new JsonpResult(false, true, "Não foi possível realizar o evento de inclusão do motorista ao MDF-e.");

                Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe()
                {
                    CPF = Utilidades.String.OnlyNumbers(motorista.CPF),
                    Nome = Utilidades.String.Left(motorista.Nome, 60),
                    MDFe = mdfe,
                    Tipo = TipoMotoristaMDFe.SolicitadoEventoInclusao
                };

                repMotoristaMDFe.Inserir(motoristaMDFe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, $"Solicitou a inclusão do motorista {motorista.CPF_Formatado} - {motorista.Nome} ao MDF-e.", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o motorista no MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterRowColor(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            switch (mdfe.Status)
            {
                case Dominio.Enumeradores.StatusMDFe.Autorizado:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;
                case Dominio.Enumeradores.StatusMDFe.Cancelado:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza;
                case Dominio.Enumeradores.StatusMDFe.Encerrado:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul;
                case Dominio.Enumeradores.StatusMDFe.Rejeicao:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
                case Dominio.Enumeradores.StatusMDFe.EmCancelamento:
                case Dominio.Enumeradores.StatusMDFe.EmEncerramento:
                case Dominio.Enumeradores.StatusMDFe.Enviado:
                case Dominio.Enumeradores.StatusMDFe.Pendente:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;
                default:
                    return string.Empty;
            }
        }

        private string ObterFontColor(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco;

            return string.Empty;
        }

        #endregion
    }
}
