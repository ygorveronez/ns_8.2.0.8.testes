using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Net;

namespace SGT.WebAdmin.Controllers.MDFe
{
    [CustomAuthorize("MDFe/ConsultaMDFe")]
    public class ConsultaMDFeController : BaseController
    {
        #region Construtores

        public ConsultaMDFeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Origem", "EstadoCarregamento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "EstadoDescarregamento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoStatus", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSEFAZ", 20, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "EstadoOrigem")
                    propOrdenacao += ".Nome";
                else if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";
                else if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repositorioMDFe.Consultar(filtrosPesquisa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int countMDFes = repositorioMDFe.ContarConsulta(filtrosPesquisa);

                grid.setarQuantidadeTotal(countMDFes);

                var lista = (from obj in mdfes
                             select new
                             {
                                 obj.Codigo,
                                 obj.Status,
                                 obj.Numero,
                                 Serie = obj.Serie.Numero,
                                 DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                 Veiculo = obj.Veiculos.FirstOrDefault()?.Placa ?? string.Empty,
                                 Transportador = obj.Empresa.Descricao,
                                 EstadoCarregamento = obj.EstadoCarregamento.Sigla + " - " + obj.EstadoCarregamento.Nome,
                                 EstadoDescarregamento = obj.EstadoDescarregamento.Sigla + " - " + obj.EstadoDescarregamento.Nome,
                                 obj.DescricaoStatus,
                                 RetornoSEFAZ = (obj.MensagemStatus == null ? (obj.MensagemRetornoSefaz != null ? WebUtility.HtmlEncode(obj.MensagemRetornoSefaz) : string.Empty) : obj.MensagemStatus.MensagemDoErro),
                                 DT_RowColor = (obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? "#dff0d8" : (obj.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || obj.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia) ? "rgba(193, 101, 101, 1)" : (obj.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || obj.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) ? "#777" : ""),
                                 DT_FontColor = (obj.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || obj.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || obj.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) ? "#FFFFFF" : "",
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os MDF-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para o download do DAMDFE.");

                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeTrabalho);

                byte[] arquivo = null;

                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
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
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para realizar o download do XML de autorização.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLAutorizacao(mdfe, unidadeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML de autorização.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLEncerramento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = Request.GetIntParam("MDFe");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar encerrado para realizar o download do XML de encerramento.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLEncerramento(mdfe, unidadeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML de encerramento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                int countMDFes = repMDFe.ContarConsulta(filtrosPesquisa);

                if (countMDFes > 20000)
                    return new JsonpResult(false, true, "Quantidade de MDF-es para geração de lote inválida (" + countMDFes + "). É permitido o download de um lote com o máximo de 20000 MDF-es.");

                List<int> listaCodigosMDFe = repMDFe.ConsultarCodigos(filtrosPesquisa);

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                return Arquivo(svcMDFe.ObterLoteDeXML(listaCodigosMDFe, filtrosPesquisa.CodigoEmpresa, unidadeTrabalho), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                int countMDFes = repMDFe.ContarConsulta(filtrosPesquisa);

                if (countMDFes > 1000)
                    return new JsonpResult(false, true, "Quantidade de MDF-es para geração de lote inválida (" + countMDFes + "). É permitido o download de um lote com o máximo de 1000 MDF-es.");

                List<string> listaChavesMDFe = repMDFe.ConsultarChaves(filtrosPesquisa);

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                return Arquivo(svcMDFe.ObterLoteDeDAMDFE(listaChavesMDFe, filtrosPesquisa.CodigoEmpresa, unidadeTrabalho), "application/zip", "LoteDAMDFE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirLoteMDFesContingenciados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                if ((filtrosPesquisa.DataEmissaoFinal - filtrosPesquisa.DataEmissaoInicial) > TimeSpan.FromDays(1))
                    return new JsonpResult(false, true, "O período para emissão em Lote não pode ser maior que um dia.");

                filtrosPesquisa.Status = StatusMDFe.EmitidoContingencia;
                //Setado fixo para caso o usuário altere o Status antes de ter feito uma nova pesquisa

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                int countMDFes = repMDFe.ContarConsulta(filtrosPesquisa);

                if (countMDFes == 0)
                    return new JsonpResult(false, true, "Nenhum MDFe encontrado para os filtros atuais");


                if (countMDFes > 100)
                    return new JsonpResult(false, true, "Quantidade de MDF-es para emissão em lote inválida (" + countMDFes + "). É permitido a emissão de 100 MDF-es por Lote.");

                List<int> listaCodigosMDFe = repMDFe.ConsultarCodigos(filtrosPesquisa);

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                return new JsonpResult(true, svcMDFe.EmitirLoteMDFeContigenciado(listaCodigosMDFe, Usuario, Auditado, ClienteAcesso.WebServiceConsultaCTe, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar a emissão em lote.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosParaEncerramentoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramentoMDFe = Servicos.Embarcador.Carga.MDFe.ObterDadosEncerramentoMDFe(codigo, unitOfWork);

                var encerramentoMDF = new
                {
                    dadosEncerramentoMDFe.Codigo,
                    Estado = dadosEncerramentoMDFe.Estado.Nome.Trim() + " - " + dadosEncerramentoMDFe.Estado.Sigla,
                    DataEncerramento = dadosEncerramentoMDFe.DataEncerramento.ToString("dd/MM/yyyy"),
                    HoraEncerramento = dadosEncerramentoMDFe.DataEncerramento.ToString("HH:mm"),
                    Localidades = dadosEncerramentoMDFe.Localidades.Select(obj => new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList()
                };

                return new JsonpResult(encerramentoMDF);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados para encerramento do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EncerrarMDFe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                int codigo = Request.GetIntParam("Codigo");
                int codigoLocalidade = Request.GetIntParam("Localidade");

                DateTime dataEncerramento;
                DateTime.TryParseExact(Request.Params("DataEncerramento") + " " + Request.Params("HoraEncerramento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);

                VerificarEEncerrarMDFe(unidadeTrabalho, codigo, codigoLocalidade, dataEncerramento, mdfe);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unidadeTrabalho.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unidadeTrabalho.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> EncerrarManualMDFe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

                string chave = Request.GetStringParam("Chave");
                string protocolo = Request.GetStringParam("Protocolo");
                int codigoLocalidade = Request.GetIntParam("LocalidadeManualMdfe");
                int codigoEmpresa = Request.GetIntParam("EmpresaManualMdfe");

                DateTime dataEncerramento;
                DateTime.TryParseExact(Request.Params("DataEncerramento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Dominio.Entidades.Localidade localidade = await repLocalidade.BuscarPorCodigoAsync(codigoLocalidade);
                Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(codigoEmpresa);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = await new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork).BuscarPorChaveAsync(chave);
                if (mdfe != null)
                    VerificarEEncerrarMDFe(unitOfWork, mdfe.Codigo, codigoLocalidade, dataEncerramento, mdfe);
                else if (!Servicos.Embarcador.Carga.MDFe.EncerrarMDFeEmissorExterno(out string erroMDFeExterno, chave, localidade, protocolo, empresa, dataEncerramento, Usuario, unitOfWork))
                    throw new ServicoException(erroMDFeExterno);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
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

                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void VerificarEEncerrarMDFe(Repositorio.UnitOfWork unitOfWork, int codigo, int codigoLocalidade, DateTime dataEncerramento, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            if (mdfe.Importado && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                throw new ControllerException("Não é possível executar esta operação para MDF-e recebido da integração.");

            Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Solicitou Encerramento do MDF-e", unitOfWork);

            if (!Servicos.Embarcador.Carga.MDFe.EncerrarMDFe(out string erro, codigo, codigoLocalidade, dataEncerramento, WebServiceConsultaCTe, this.Usuario, unitOfWork, _conexao.StringConexao, Auditado))
                throw new ServicoException(erro);
        }

        private Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroConsultaMDFe()
            {
                ChaveCTe = Request.GetStringParam("ChaveCTe"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                EstadoDestino = Request.GetStringParam("EstadoDestino"),
                Status = Request.GetNullableEnumParam<Dominio.Enumeradores.StatusMDFe>("Status"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                CPFCNPJRemetente = Request.GetDoubleParam("Remetente"),
                Placa = Request.GetStringParam("Placa"),
                Serie = Request.GetIntParam("Serie"),
                CodigoCarga = Request.GetIntParam("CargaCodigo"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoEmpresa = Empresa.Codigo;

            return filtrosPesquisa;
        }

        #endregion
    }
}
