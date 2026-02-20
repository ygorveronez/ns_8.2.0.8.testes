using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositorio;
using Repositorio.Embarcador.Documentos;
using SGTAdmin.Controllers;
using System.Data;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "BuscarSeriePorCodigo", "BuscarPorCodigo", "byteArrayToImage", "VerificarCNPJCadastrado", "RequisicaoConsultaCNPJ", "ConsultaCNPJCentralizada", "InformarCaptchaConsultaCNPJ", "ConsultaCNPJReceitaWS", "ExportarIntelipostTipoOcorrencia", "VerificarSeExisteEscalationList", "ObterConfiguracao" }, "Transportadores/Transportador", "Transportadores/GestaoTransportador")]
    public class TransportadorController : BaseController
    {
        #region Construtores

        public TransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarDadosAcessoaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));
                Dominio.Entidades.Usuario usuario;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    usuario = repUsuario.BuscarPorLogin(empresa.CNPJ_SemFormato);
                else
                    usuario = repUsuario.BuscarPorCPF(empresa.Codigo, "11111111111", "U");

                if (usuario != null)
                    EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Transportadores.Transportador.ReenviouOsDadosDeAcesso, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoEnviarOsDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

                bool sistemaEstrangeiro = ConfiguracaoEmbarcador.Pais != TipoPais.Brasil;

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("AptaEmissao", false);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("CertificadoVencido", false);
                grid.AdicionarCabecalho("EmpresaPropria", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("HabilitarCIOT", false);
                grid.AdicionarCabecalho("ObrigatoriedadeCIOTEmissaoMDFe", false);
                grid.AdicionarCabecalho("EmissaoDocumentosForaDoSistema", false);
                grid.AdicionarCabecalho("RaizCnpj", false);
                grid.AdicionarCabecalho("PossuiInformacoesIMO", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Transportador.RazaoSocial, "RazaoSocial", 32, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Transportador.CodigoIntegracao, "CodigoIntegracao", 16, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho(sistemaEstrangeiro ? Localization.Resources.Consultas.Transportador.Identificacao : Localization.Resources.Consultas.Transportador.CPFCNPJ, "CNPJ", 16, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Transportador.Telefone, "Telefone", 11, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Transportador.Localidade, "Localidade", 18, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("RestringirLocaisCarregamentoAutorizadosMotoristas", false);
                grid.AdicionarCabecalho("OrdenarCargasMobileCrescente", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("IdentificadorTransportadorElectrolux", false);

                if (ConfiguracaoEmbarcador.PermitirAutomatizarPagamentoTransportador)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Transportador.IntegracaoAutomaticaCTEGOLD, "LiberacaoParaPagamentoAutomatico", 11, Models.Grid.Align.center, false, false, true);

                if (filtrosPesquisa.StatusTodos)
                    filtrosPesquisa.SituacaoPesquisa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;

                if (filtrosPesquisa.SituacaoPesquisa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 8, Models.Grid.Align.center, false);

                if (filtrosPesquisa.CodigoFilial > 0 && filtrosPesquisa.TelaMontagemCargaMapa && configuracaoMontagemCarga.UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa)
                {
                    filtrosPesquisa.SomenteTransportadorHabilitadoTransportarParaFilial = true;
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int totalRegistros = repEmpresa.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Empresa> listaEmpresa = totalRegistros > 0 ? repEmpresa.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Empresa>();

                try
                {
                    dynamic listaEmpresaRetornar = (
                        from empresa in listaEmpresa
                        select new
                        {
                            empresa.Codigo,
                            empresa.DescricaoStatus,
                            empresa.Telefone,
                            empresa.EmpresaPropria,
                            empresa.EmissaoDocumentosForaDoSistema,
                            empresa.RazaoSocial,
                            empresa.CodigoIntegracao,
                            empresa.Descricao,
                            HabilitarCIOT = empresa.Configuracao?.TipoIntegradoraCIOT.HasValue ?? false,
                            ObrigatoriedadeCIOTEmissaoMDFe = empresa.Configuracao?.ObrigatoriedadeCIOTEmissaoMDFe ?? false,
                            CNPJ = sistemaEstrangeiro ? empresa.CNPJ_Identificacao_Exterior.ObterCnpjFormatado() : empresa.CNPJ_Formatado,
                            empresa.RaizCnpj,
                            CodigoLocalidade = empresa.Localidade.Codigo,
                            Localidade = empresa.Localidade.DescricaoCidadeEstado,
                            AptaEmissao = ((empresa.EmpresaPai != null && empresa.TipoAmbiente == empresa.EmpresaPai.TipoAmbiente && !string.IsNullOrWhiteSpace(empresa.NomeCertificado)) || empresa.EmissaoDocumentosForaDoSistema ? true : false),
                            CertificadoVencido = (empresa.ModeloDocumentoFiscalCargaPropria != null && empresa.ModeloDocumentoFiscalCargaPropria.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && empresa.ModeloDocumentoFiscalCargaPropria.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe) ? true : (empresa.DataFinalCertificado.HasValue ? empresa.DataFinalCertificado.Value <= DateTime.Now ? false : true : true),
                            LiberacaoParaPagamentoAutomatico = empresa.DescricaoLiberacaoParaPagamentoAutomatico,
                            empresa.RestringirLocaisCarregamentoAutorizadosMotoristas,
                            empresa.OrdenarCargasMobileCrescente,
                            empresa.Tipo,
                            IdentificadorTransportadorElectrolux = empresa.Configuracao?.IdentificadorTransportadorElectrolux ?? "",
                            DT_RowColor = obterCorRow(empresa),
                            DT_FontColor = obterFonteColor(empresa),
                            PossuiInformacoesIMO = !string.IsNullOrEmpty(empresa.IMO) && (empresa.DataValidadeIMO.HasValue && empresa.DataValidadeIMO.Value > DateTime.Now)
                        }
                    ).ToList();

                    grid.AdicionaRows(listaEmpresaRetornar);
                    grid.setarQuantidadeTotal(totalRegistros);
                    return new JsonpResult(grid);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    if (listaEmpresa != null)
                    {
                        Servicos.Log.TratarErro(Localization.Resources.Transportadores.Transportador.DadosEmpresaExcecao + JsonConvert.SerializeObject(listaEmpresa));
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repositorioConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT> operadorasCIOTExistentes = repConfiguracaoCIOT.BuscarOperadorasDisponiveis();

                List<TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

                return new JsonpResult(new
                {
                    TemIntegracaoIntelipost = tiposIntegracao.Any(o => o == TipoIntegracao.InteliPost),
                    TemIntegracaoKrona = tiposIntegracao.Any(o => o == TipoIntegracao.Krona),
                    TemIntegracaoOpenTech = tiposIntegracao.Any(o => o == TipoIntegracao.OpenTech),
                    TemIntegracaoSintegra = tiposIntegracao.Any(o => o == TipoIntegracao.Sintegra),
                    TemIntegracaoRepomRest = repositorioConfiguracaoValePedagio.PossuiIntegracaoRepomRest(),
                    TemIntegracaoRepom = operadorasCIOTExistentes.Contains(OperadoraCIOT.Repom) || operadorasCIOTExistentes.Contains(OperadoraCIOT.RepomFrete),
                    TemIntegracaoElectrolux = tiposIntegracao.Any(o => o == TipoIntegracao.Electrolux),
                    TemIntegracaoMigrate = tiposIntegracao.Any(o => o == TipoIntegracao.Migrate),
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterAsConfiguracoesDeIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarSeries()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTransportador, numero;

                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);
                int.TryParse(Request.Params("Numero"), out numero);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Numero, "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "DescricaoTipo", 40, Models.Grid.Align.left, false);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.ProximoNumeroDocumento, "ProximoNumeroDocumento", 25, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("ProximoNumeroDocumento", false);

                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);

                List<Dominio.Entidades.EmpresaSerie> series = repSerie.Consultar(codigoTransportador, numero, "", grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);

                int countSeries = repSerie.ContarConsulta(codigoTransportador, numero, "");

                grid.setarQuantidadeTotal(countSeries);

                grid.AdicionaRows((from obj in series
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Tipo,
                                       Descricao = obj.Numero + " (" + obj.DescricaoTipo + ")",
                                       obj.Numero,
                                       obj.DescricaoTipo,
                                       obj.ProximoNumeroDocumento
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarSeriesPorTipo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransportador, numero, codigoFilial;

                if (!int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador))
                    int.TryParse(Request.Params("Transportador"), out codigoTransportador);

                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("Numero"), out numero);

                Dominio.Enumeradores.TipoSerie tipoSerie = (Dominio.Enumeradores.TipoSerie)int.Parse(Request.Params("TipoSerie"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Numero, "Numero", 60, Models.Grid.Align.left, false);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.ProximoNumeroDocumento, "ProximoNumeroDocumento", 20, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("ProximoNumeroDocumento", false);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                if (codigoFilial > 0)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(filial.CNPJ);
                    if (empresa != null)
                        codigoTransportador = empresa.Codigo;
                }


                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);

                List<Dominio.Entidades.EmpresaSerie> series = repSerie.ConsultarPorTipo(codigoTransportador, numero, "", tipoSerie, grid.inicio, grid.limite);

                int countSeries = repSerie.ContarConsultaPorTipo(codigoTransportador, numero, "", tipoSerie);

                grid.setarQuantidadeTotal(countSeries);

                var resultado = (from obj in series
                                 select new
                                 {
                                     obj.Codigo,
                                     obj.Tipo,
                                     Descricao = obj.Numero.ToString(),
                                     obj.Numero,
                                     obj.DescricaoTipo,
                                     obj.ProximoNumeroDocumento
                                 }).ToList();

                grid.AdicionaRows(resultado);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSeriePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);

                Dominio.Entidades.EmpresaSerie serie = repSerie.BuscarPorCodigo(codigo);

                if (serie == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.SerieNaoEncontrada);

                var retorno = new
                {
                    serie.Codigo,
                    serie.Numero,
                    serie.Tipo,
                    serie.ProximoNumeroDocumento
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterOsDetalhesDaSerie);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = new List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada>();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoPossivelInserirUmaNovaEmpresaNesteModulo);

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = new Dominio.Entidades.Empresa();

                empresa.Tipo = !string.IsNullOrWhiteSpace(Request.Params("TipoEmpresa")) ? Request.Params("TipoEmpresa") : "J";

                if (empresa.Tipo.Equals("F"))
                    empresa.CNPJ = Utilidades.String.OnlyNumbers(Request.Params("CPF"));
                else if (empresa.Tipo.Equals("E"))
                    empresa.CNPJ = string.Empty;
                else
                    empresa.CNPJ = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));

                PreencherTransportador(empresa, unitOfWork, permissoesPersonalizadas);

                empresa.CaminhoLogoSistema = Request.Params("CaminhoLogoSistema");
                empresa.CodigoIntegracao = Request.Params("CodigoIntegracao");
                empresa.Email = Request.Params("Email").ToLower();
                empresa.EmailAdministrativo = Request.Params("EmailAdministrativo").ToLower();
                empresa.EmailContador = Request.Params("EmailContador").ToLower();
                empresa.AtivarCondicao = Request.GetBoolParam("AtivarCondicao");


                if (ConfiguracaoEmbarcador.ExigirCodigoIntegracaoTransportador && string.IsNullOrWhiteSpace(empresa.CodigoIntegracao) && !configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas)
                    throw new ControllerException(Localization.Resources.Transportadores.Transportador.ObrigatorioInformarUmCodigoDeIntegracaoNoCadastroDaEmpresa);


                bool enviarEmail, enviarEmailAdministrativo, enviarEmailContador, liberarEmissaoSemAverbacao, optanteSimplesNacionalComExcessoReceitaBruta;
                bool.TryParse(Request.Params("EnviarEmail"), out enviarEmail);
                bool.TryParse(Request.Params("EnviarEmailAdministrativo"), out enviarEmailAdministrativo);
                bool.TryParse(Request.Params("EnviarEmailContador"), out enviarEmailContador);
                bool.TryParse(Request.Params("OptanteSimplesNacionalComExcessoReceitaBruta"), out optanteSimplesNacionalComExcessoReceitaBruta);
                empresa.OptanteSimplesNacionalComExcessoReceitaBruta = optanteSimplesNacionalComExcessoReceitaBruta;
                empresa.LiberacaoParaPagamentoAutomatico = Request.GetBoolParam("LiberacaoParaPagamentoAutomatico");


                bool.TryParse(Request.Params("LiberarEmissaoSemAverbacao"), out liberarEmissaoSemAverbacao);
                empresa.LiberarEmissaoSemAverbacao = liberarEmissaoSemAverbacao;

                bool transportadorAdministrador;
                bool.TryParse(Request.Params("TransportadorAdministrador"), out transportadorAdministrador);
                empresa.TransportadorAdministrador = transportadorAdministrador;

                bool habilitaSincronismoDocumentosDestinados = false;
                bool.TryParse(Request.Params("HabilitaSincronismoDocumentosDestinados"), out habilitaSincronismoDocumentosDestinados);
                empresa.HabilitaSincronismoDocumentosDestinados = habilitaSincronismoDocumentosDestinados;

                int proximoNumeroNFe = 1;
                int.TryParse(Request.Params("ProximoNumeroNFe"), out proximoNumeroNFe);
                empresa.UltimoNumeroNFe = proximoNumeroNFe;

                int proximoNumeroNFCe = 1;
                int.TryParse(Request.Params("ProximoNumeroNFCe"), out proximoNumeroNFCe);
                empresa.UltimoNumeroNFCe = proximoNumeroNFCe;

                string token = Request.Params("IdTokenNFCe");
                string cscToken = Request.Params("IdCSCNFCe");
                if (!string.IsNullOrWhiteSpace(token) && Utilidades.String.OnlyNumbers(token).Length != 6)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TokenDeNFCeEstaInvalidoMesmoDevePossuirSeisNumeros);
                else if (string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(cscToken))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TokenDeNFCeNaoConfigurado);
                else if (!string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(cscToken))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.CSCDoTokenDeNFCeNaoConfigurado);
                empresa.IdTokenNFCe = token;
                empresa.IdCSCNFCe = cscToken;
                empresa.StatusFinanceiro = Request.Params("StatusFinanceiro");
                string serieRps = Request.Params("SerieRPS");

                decimal aliquotaICMSSimples = 0;
                decimal.TryParse(Request.Params("AliquotaICMSSimples"), out aliquotaICMSSimples);
                empresa.AliquotaICMSSimples = aliquotaICMSSimples;
                empresa.VersaoNFe = (VersaoNFe)int.Parse(Request.Params("VersaoNFe"));

                empresa.StatusEmail = enviarEmail ? "A" : "I";
                empresa.StatusEmailAdministrativo = enviarEmailAdministrativo ? "A" : "I";
                empresa.StatusEmailContador = enviarEmailContador ? "A" : "I";
                empresa.StatusEmissao = "S";
                empresa.DataCadastro = DateTime.Now;
                empresa.UsuarioCadastro = Auditado.Usuario;

                bool usuarioUsaMobile;
                bool.TryParse(Request.Params("EmpresaMobile"), out usuarioUsaMobile);
                empresa.EmpresaMobile = usuarioUsaMobile;

                empresa.OrdenarCargasMobileCrescente = Request.GetBoolParam("OrdenarCargasMobileCrescente");

                bool integrarCorreios;
                bool.TryParse(Request.Params("IntegrarCorreios"), out integrarCorreios);
                empresa.IntegrarCorreios = integrarCorreios;

                empresa.GerarPedidoAoReceberCarga = Request.GetBoolParam("GerarPedidoAoReceberCarga");
                empresa.DataProximaConsultaSintegra = Request.GetNullableDateTimeParam("DataProximaConsultaSintegra");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    empresa.EmpresaPai = this.Usuario.Empresa;
                else
                {
                    empresa.EmpresaPai = repEmpresa.BuscarEmpresaPai();

                    if (empresa.EmpresaPai != null && empresa.TipoAmbiente != empresa.EmpresaPai.TipoAmbiente)
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TipoDeAmbienteInformadoDiferenteDoAmbienteGeral);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    empresa.URLSistema = "http://www.commerce.inf.br";
                    empresa.VisualizarSomenteClientesAssociados = true;
                }

                if (string.IsNullOrWhiteSpace(empresa.CEP) && !configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.FavorInformeCEPDaEmpresa);

                Dominio.Entidades.Empresa empresaExiste = empresa.Tipo == "E" ? repEmpresa.BuscarPorCodigoIntegracao(empresa.CodigoIntegracao) : repEmpresa.BuscarPorCNPJ(empresa.CNPJ);
                if (empresaExiste == null)
                {
                    int codigoMobile = 0;
                    if (empresa.EmpresaMobile)
                        ConfigurarEmpresaMobile(ref empresa, ref codigoMobile);

                    SalvarCodigosComercialDistribuidor(empresa, repEmpresa);

                    if (empresa.Tipo == "E")
                    {
                        empresa.DataInicialCertificado = DateTime.Now;
                        empresa.DataFinalCertificado = DateTime.Now.AddYears(100);
                        empresa.NomeCertificado = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas, Utilidades.String.OnlyNumbers(empresa.CodigoIntegracao), "Certificado", "Certificado.pfx");
                        empresa.SenhaCertificado = "1234";
                    }

                    repEmpresa.Inserir(empresa, Auditado);

                    SalvarSeries(empresa, unitOfWork);
                    SalvarConfiguracoes(empresa, unitOfWork);
                    SalvarDadoBancario(empresa, unitOfWork);
                    SalvarTermoQuitacao(empresa, unitOfWork);
                    SalvarComponentesCTeImportado(empresa, unitOfWork);
                    SalvarLayoutsEDI(empresa, unitOfWork);
                    SalvarEstadosFeeder(empresa, unitOfWork);
                    SalvarEmpresaFilial(empresa, unitOfWork);
                    SalvarEmpresaFilialEmbarcador(empresa, unitOfWork);
                    SalvarIntelipostDadosIntegracao(empresa, unitOfWork);
                    SalvarIntelipostTipoOcorrencia(empresa, unitOfWork);
                    SalvarPermissoes(empresa, unitOfWork);
                    SalvarPermissoesTransportador(empresa, unitOfWork);
                    SalvarIntegracaoKrona(empresa, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(serieRps))
                        SalvarSerieRPS(serieRps, empresa, unitOfWork);

                    AdicionarSeriePadraoConfiguracao(empresa, unitOfWork);
                    SalvarTransportadorFilial(empresa, unitOfWork);
                    SalvarRotasFreteValePedagio(empresa, unitOfWork);
                    SalvarInscricoesST(empresa, unitOfWork);

                    if (empresa.Configuracao.TipoIntegradoraCIOT.HasValue)
                        SalvarImpostoContratoFrete(empresa, unitOfWork);

                    AdicionarOuAtualizarCondicoesPagamento(empresa, unitOfWork);
                    SalvarUsuarioPadrao(empresa, codigoMobile, configuracaoTransportador, unitOfWork);

                    SalvarTransportadorNFSe(empresa);
                    SalvarAutomacaoEmissaoNFSManual(empresa, unitOfWork);
                    SalvarCodigosIntegracao(empresa, unitOfWork);
                    SalvarOperadoresTransportador(empresa, unitOfWork);
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        SalvarDadosCliente(empresa, unitOfWork);

                    repEmpresa.Atualizar(empresa);
                    unitOfWork.CommitChanges();

                    Servicos.AtualizacaoEmpresa svcAtualizaEmpresa = new Servicos.AtualizacaoEmpresa(unitOfWork);
                    svcAtualizaEmpresa.Atualizar(empresa, unitOfWork);

                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTeType == TipoEmissorDocumento.NSTech || Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFeType == TipoEmissorDocumento.NSTech)
                    {
                        string mensagemErro = string.Empty;
                        Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                        if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresa, unitOfWork))
                        {
                            Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro}");
                            return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoSalvarCertificado);
                        }

                        if (!integracaoNStech.IncluirAtualizarLogo(out mensagemErro, empresa, unitOfWork))
                            Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o logo para o emissor NStech: {mensagemErro}");
                    }

                    //Enviar Certificado Digital Key Vault
                    svcAtualizaEmpresa.EnviarCertificadoKeyVault(empresa, ClienteAcesso, unitOfWork);

                    Servicos.Embarcador.Login.Login svcLogin = new Servicos.Embarcador.Login.Login(unitOfWork);
                    svcLogin.BloquearDesbloquearEmpresaCommerce(empresa, "N", TipoServicoMultisoftware, unitOfWork);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        this.SalvarScriptPadrao(this.Usuario.Empresa.Codigo, empresa, unitOfWork);

                    await AdicionarIntegracaoTransportador(empresa, unitOfWork);

                    return new JsonpResult(true);
                }
                else
                {
                    if (empresa.Tipo == "E")
                        throw new ControllerException(Localization.Resources.Transportadores.Transportador.JaExisteUmaEmpresaDoExteriorCadastradaComCodigo + empresa.CodigoIntegracao);
                    else
                        throw new ControllerException(Localization.Resources.Transportadores.Transportador.JaExisteUmaEmpresaCadastradaComCNPJ + empresa.CNPJ);
                }
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<bool> AdicionarIntegracaoTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUB = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB);

            if (tipoIntegracao == null)
                return false;

            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracao = await repositorioCargaIntegracaoHUB.ConsultarIntegracaoTransportador(empresa.Codigo);

                if (integracao == null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas empresaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas
                    {
                        Empresa = empresa,
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                        DataIntegracao = DateTime.Now,
                        TipoEnvioHUBOfertas = TipoEnvioHUBOfertas.EnvioTransportador,
                        NumeroTentativas = 0,
                        TipoIntegracao = tipoIntegracao,
                        ProblemaIntegracao = ""
                    };
                    await repositorioCargaIntegracaoHUB.InserirAsync(empresaIntegracao);
                }
                else
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.DataIntegracao = DateTime.Now;
                    await repositorioCargaIntegracaoHUB.AtualizarAsync(integracao);
                }
                return true;
            }
            catch (Exception excecao)
            {
                Servicos.Log.GravarInfo($"Erro integração Transportador: {excecao.Message}", "HUBOfertas");
                return false;
            }

        }


        public async Task<IActionResult> AtualizarTransportadorGestao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Repositorio.Localidade RepLocalidade = new Repositorio.Localidade(unitOfWork);

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Codigo"));
                Dominio.Entidades.Empresa empresaExiste = empresa.Tipo == "E" ? repEmpresa.BuscarPorCodigoIntegracao(empresa.CodigoIntegracao) : repEmpresa.BuscarPorCNPJ(empresa.CNPJ);
                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorCodigoEmpresa(empresa.Codigo);
                Dominio.Entidades.Localidade localidade = RepLocalidade.BuscarPorCodigo(Request.GetIntParam("Localidade"));

                if (empresa == null)
                    throw new ControllerException("Empresa não existe");

                empresa.DataAtualizacao = DateTime.Now;
                empresa.UsuarioAtualizacao = Auditado.Usuario;

                // Datos del Transportador 
                empresa.RazaoSocial = Request.GetStringParam("RazaoSocial");
                empresa.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                empresa.NomeFantasia = Request.GetStringParam("NomeFantasia");
                empresa.Tipo = Request.GetStringParam("TipoEmpresa");
                empresa.CNPJ = Request.GetStringParam("CNPJ");
                empresa.RegistroANTT = Request.GetStringParam("RegistroANTT");
                empresa.InscricaoEstadual = Request.GetStringParam("InscricaoEstadual");
                empresa.InscricaoMunicipal = Request.GetStringParam("InscricaoMunicipal");
                empresa.CNAE = Request.GetStringParam("CNAE");
                empresa.CEP = Request.GetStringParam("CEP");
                empresa.Endereco = Request.GetStringParam("Endereco");
                empresa.Numero = Request.GetStringParam("Numero");
                empresa.Bairro = Request.GetStringParam("Bairro");
                empresa.Complemento = Request.GetStringParam("Complemento");
                empresa.Telefone = Request.GetStringParam("Telefone");
                empresa.Status = Request.GetStringParam("status");
                empresa.Localidade = localidade;
                empresa.Email = Request.Params("Email").ToLower();
                empresa.EmailAdministrativo = Request.Params("EmailAdministrativo").ToLower();
                empresa.EmailContador = Request.Params("EmailContador").ToLower();

                bool enviarEmail, enviarEmailAdministrativo, enviarEmailContador;

                bool.TryParse(Request.Params("EnviarEmail"), out enviarEmail);
                bool.TryParse(Request.Params("EnviarEmailAdministrativo"), out enviarEmailAdministrativo);
                bool.TryParse(Request.Params("EnviarEmailContador"), out enviarEmailContador);

                empresa.StatusEmail = enviarEmail ? "A" : "I";
                empresa.StatusEmailAdministrativo = enviarEmailAdministrativo ? "A" : "I";
                empresa.StatusEmailContador = enviarEmailContador ? "A" : "I";

                dynamic certificado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Certificado"));

                // Datos Certificado Transportador
                empresa.DataInicialCertificado = ((string)certificado["DataInicial"]).ToNullableDateTime();
                empresa.DataFinalCertificado = ((string)certificado["DataFinal"]).ToNullableDateTime();
                empresa.SerieCertificado = certificado["Serie"];
                empresa.SenhaCertificado = certificado["Senha"];

                dynamic CTEMDF = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CTEMDF"));

                Repositorio.EmpresaSerie repositorioSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Dominio.Entidades.EmpresaSerie serieIntraStadual = repositorioSerie.BuscarPorCodigo((int)CTEMDF["SerieIntraestadual"]);
                Dominio.Entidades.EmpresaSerie serieInterStadual = repositorioSerie.BuscarPorCodigo((int)CTEMDF["SerieInterestadual"]);
                Dominio.Entidades.EmpresaSerie SerieMDFe = repositorioSerie.BuscarPorCodigo((int)CTEMDF["SerieMDFe"]);

                empresa.Configuracao.SerieIntraestadual = serieIntraStadual;
                empresa.Configuracao.SerieInterestadual = serieInterStadual;
                empresa.Configuracao.SerieMDFe = SerieMDFe;
                empresa.TipoEmissaoIntramunicipal = (TipoEmissaoIntramunicipal)CTEMDF["documentoFretes"];

                int tempoDelay = 0;
                int.TryParse((string)CTEMDF["tempoEmissao"], out tempoDelay);
                empresa.TempoDelayHorasParaIniciarEmissao = tempoDelay;

                int QuantidadeMaxima = 0;
                int.TryParse((string)CTEMDF["quantidadeEmailRPS"], out QuantidadeMaxima);
                empresa.QuantidadeMaximaEmailRPS = QuantidadeMaxima;

                empresa.AliquotaICMSSimples = Utilidades.Decimal.Converter((string)CTEMDF["aliquotaICMS"]);
                empresa.Configuracao.AliquotaPIS = Utilidades.Decimal.Converter((string)CTEMDF["aliquotaPIS"]);
                empresa.Configuracao.AliquotaCOFINS = Utilidades.Decimal.Converter((string)CTEMDF["aliquotaCOFINS"]);

                empresa.Configuracao.FraseSecretaNFSe = (string)CTEMDF["FraseNFS"];
                empresa.Configuracao.SenhaNFSe = (string)CTEMDF["senhaPrefeitura"];
                empresa.ObservacaoCTe = (string)CTEMDF["observacaoCTe"];

                // Salvando Series
                SalvarSeries(empresa, unitOfWork);

                // Seguros

                empresa.UsarTipoOperacaoApolice = Request.GetBoolParam("UsarTipoOperacaoApolice");

                // Dados Configuração NFS-e
                dynamic NFSeConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("configuracaoNFE"));

                if (transportadorConfiguracaoNFSe == null)
                    transportadorConfiguracaoNFSe = new Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe();

                decimal AliquotaISS = 0;
                decimal.TryParse((string)NFSeConfig["AliquotaISS"], out AliquotaISS);
                transportadorConfiguracaoNFSe.AliquotaISS = AliquotaISS;

                decimal RetencaoISS = 0;
                decimal.TryParse((string)NFSeConfig["RetencaoISS"], out RetencaoISS);
                transportadorConfiguracaoNFSe.RetencaoISS = RetencaoISS;

                bool PermiteAnular = false;
                bool.TryParse((string)NFSeConfig["PermiteAnular"], out PermiteAnular);
                transportadorConfiguracaoNFSe.PermiteAnular = PermiteAnular;


                int PrazoCancelamento = 0;
                int.TryParse((string)NFSeConfig["PrazoCancelamento"], out PrazoCancelamento);
                transportadorConfiguracaoNFSe.PrazoCancelamento = PrazoCancelamento;

                transportadorConfiguracaoNFSe.FraseSecreta = (string)NFSeConfig["FraseSecreta"];
                transportadorConfiguracaoNFSe.LoginSitePrefeitura = (string)NFSeConfig["LoginSitePrefeitura"];
                transportadorConfiguracaoNFSe.DiscriminacaoNFSe = (string)NFSeConfig["DiscriminacaoNFSe"];
                transportadorConfiguracaoNFSe.SenhaSitePrefeitura = (string)NFSeConfig["SenhaSitePrefeitura"];
                transportadorConfiguracaoNFSe.SerieRPS = (string)NFSeConfig["SerieRPS"];
                transportadorConfiguracaoNFSe.URLPrefeitura = (string)NFSeConfig["URLPrefeitura"];

                Repositorio.Estado RepEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.NaturezaNFSe RepNatureza = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.ServicoNFSe RepServico = new Repositorio.ServicoNFSe(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas RepGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao RepTipoOp = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Cliente RepCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Estado UFTomador = (int)NFSeConfig["UFTomador"] > 0 ? RepEstado.BuscarPorCodigo((int)NFSeConfig["UFTomador"], false) : null;
                Dominio.Entidades.Localidade LocalidadePrestacao = (int)NFSeConfig["LocalidadeTomador"] > 0 ? RepLocalidade.BuscarPorCodigo((int)NFSeConfig["LocalidadeTomador"]) : null;
                Dominio.Entidades.NaturezaNFSe NaturezaNFSe = (int)NFSeConfig["NaturezaNFSe"] > 0 ? RepNatureza.BuscarPorCodigo((int)NFSeConfig["NaturezaNFSe"]) : null;
                Dominio.Entidades.ServicoNFSe ServicoNFSe = (int)NFSeConfig["ServicoNFSe"] > 0 ? RepServico.BuscarPorCodigo((int)NFSeConfig["ServicoNFSe"]) : null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoTomador = (int)NFSeConfig["GrupoTomador"] > 0 ? RepGrupoPessoa.BuscarPorCodigo((int)NFSeConfig["GrupoTomador"]) : null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao = (int)NFSeConfig["TipoOperacao"] > 0 ? RepTipoOp.BuscarPorCodigo((int)NFSeConfig["TipoOperacao"]) : null;
                Dominio.Entidades.Cliente ClienteTomador = (int)NFSeConfig["ClienteTomador"] > 0 ? RepCliente.BuscarPorCodigo((int)NFSeConfig["ClienteTomador"], false) : null;

                transportadorConfiguracaoNFSe.UFTomador = UFTomador;
                transportadorConfiguracaoNFSe.LocalidadePrestacao = LocalidadePrestacao;
                transportadorConfiguracaoNFSe.NBS = NFSeConfig["NBS"];
                transportadorConfiguracaoNFSe.NaturezaNFSe = NaturezaNFSe;
                transportadorConfiguracaoNFSe.ServicoNFSe = ServicoNFSe;
                transportadorConfiguracaoNFSe.GrupoTomador = GrupoTomador;
                transportadorConfiguracaoNFSe.TipoOperacao = TipoOperacao;
                transportadorConfiguracaoNFSe.ClienteTomador = ClienteTomador;

                int.TryParse((string)NFSeConfig["Codigo"], out int codigoTransportadorConfiguracaoNFSe);

                if (codigoTransportadorConfiguracaoNFSe > 0)
                    repTransportadorConfiguracaoNFSe.Atualizar(transportadorConfiguracaoNFSe);
                else
                {
                    transportadorConfiguracaoNFSe.Empresa = empresa;
                    repTransportadorConfiguracaoNFSe.Inserir(transportadorConfiguracaoNFSe);
                }

                repEmpresa.Atualizar(empresa);

                if (empresaExiste != null && empresaExiste.Codigo == empresa.Codigo)
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                    throw new ControllerException($"Empresa com CNPJ:{empresa.CNPJ} não existe.");

            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                PreencherTransportador(empresa, unitOfWork, permissoesPersonalizadas);

                empresa.DataAtualizacao = DateTime.Now;
                empresa.UsuarioAtualizacao = Auditado.Usuario;

                if (empresa.Tipo == "E")
                {
                    Dominio.Entidades.Empresa empresaAnterior = repEmpresa.BuscarPorCodigoIntegracao(Request.Params("CodigoIntegracao"));
                    if (empresaAnterior != null && empresaAnterior.Codigo != empresa.Codigo)
                    {
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        {
                            if (empresa.Codigo != 7874 && empresa.Codigo != 137)
                                return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.JaExisteOutraTransportadoraDoExteriorCadastradaComMesmoCodigoDeIntegracao + empresa.RazaoSocial);
                        }
                        else
                            return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.JaExisteOutraTransportadoraDoExteriorCadastradaComMesmoCodigoDeIntegracao + empresa.RazaoSocial);
                    }
                }
                else
                    empresa.CodigoIntegracao = Request.Params("CodigoIntegracao");

                if (ConfiguracaoEmbarcador.ExigirCodigoIntegracaoTransportador && string.IsNullOrWhiteSpace(empresa.CodigoIntegracao) && !configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas)
                    throw new ControllerException(Localization.Resources.Transportadores.Transportador.ObrigatorioInformarUmCodigoDeIntegracaoNoCadastroDaEmpresa);

                empresa.CaminhoLogoSistema = Request.Params("CaminhoLogoSistema");
                empresa.Email = Request.Params("Email").ToLower();
                empresa.EmailAdministrativo = Request.Params("EmailAdministrativo").ToLower();
                empresa.EmailContador = Request.Params("EmailContador").ToLower();
                empresa.EmailEnvioCanhoto = Request.Params("EmailEnvioCanhoto").ToLower();
                empresa.EmailEnvioCTeRejeitado = Request.Params("EmailEnvioCTeRejeitado").ToLower();
                empresa.AtivarCondicao = Request.GetBoolParam("AtivarCondicao");

                bool enviarEmail, enviarEmailAdministrativo, enviarEmailContador, liberarEmissaoSemAverbacao, optanteSimplesNacionalComExcessoReceitaBruta;
                bool.TryParse(Request.Params("EnviarEmail"), out enviarEmail);
                bool.TryParse(Request.Params("EnviarEmailAdministrativo"), out enviarEmailAdministrativo);
                bool.TryParse(Request.Params("EnviarEmailContador"), out enviarEmailContador);
                bool.TryParse(Request.Params("OptanteSimplesNacionalComExcessoReceitaBruta"), out optanteSimplesNacionalComExcessoReceitaBruta);
                empresa.OptanteSimplesNacionalComExcessoReceitaBruta = optanteSimplesNacionalComExcessoReceitaBruta;
                empresa.LiberacaoParaPagamentoAutomatico = Request.GetBoolParam("LiberacaoParaPagamentoAutomatico");

                bool.TryParse(Request.Params("LiberarEmissaoSemAverbacao"), out liberarEmissaoSemAverbacao);
                empresa.LiberarEmissaoSemAverbacao = liberarEmissaoSemAverbacao;

                int proximoNumeroNFe = 1;
                int.TryParse(Request.Params("ProximoNumeroNFe"), out proximoNumeroNFe);
                empresa.UltimoNumeroNFe = proximoNumeroNFe;

                int proximoNumeroNFCe = 1;
                int.TryParse(Request.Params("ProximoNumeroNFCe"), out proximoNumeroNFCe);
                empresa.UltimoNumeroNFCe = proximoNumeroNFCe;

                bool transportadorAdministrador;
                bool.TryParse(Request.Params("TransportadorAdministrador"), out transportadorAdministrador);
                empresa.TransportadorAdministrador = transportadorAdministrador;

                bool habilitaSincronismoDocumentosDestinados = false;
                bool.TryParse(Request.Params("HabilitaSincronismoDocumentosDestinados"), out habilitaSincronismoDocumentosDestinados);
                empresa.HabilitaSincronismoDocumentosDestinados = habilitaSincronismoDocumentosDestinados;

                bool usuarioUsaMobile;
                bool.TryParse(Request.Params("EmpresaMobile"), out usuarioUsaMobile);
                empresa.EmpresaMobile = usuarioUsaMobile;

                empresa.OrdenarCargasMobileCrescente = Request.GetBoolParam("OrdenarCargasMobileCrescente");

                bool integrarCorreios;
                bool.TryParse(Request.Params("IntegrarCorreios"), out integrarCorreios);
                empresa.IntegrarCorreios = integrarCorreios;

                empresa.GerarPedidoAoReceberCarga = Request.GetBoolParam("GerarPedidoAoReceberCarga");
                empresa.DataProximaConsultaSintegra = Request.GetNullableDateTimeParam("DataProximaConsultaSintegra");

                string token = Request.Params("IdTokenNFCe");
                string cscToken = Request.Params("IdCSCNFCe");
                if (!string.IsNullOrWhiteSpace(token) && Utilidades.String.OnlyNumbers(token).Length != 6)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TokenDeNFCeEstaInvalidoMesmoDevePossuirSeisNumeros);
                else if (string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(cscToken))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TokenDeNFCeNaoConfigurado);
                else if (!string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(cscToken))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.CSCDoTokenDeNFCeNaoConfigurado);
                empresa.IdTokenNFCe = token;
                empresa.IdCSCNFCe = cscToken;

                string statusFinanceiroAnterior = empresa.StatusFinanceiro;
                empresa.StatusFinanceiro = Request.Params("StatusFinanceiro");

                if (statusFinanceiroAnterior == "N" && empresa.StatusFinanceiro == "B" && !this.Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Transportador_PermiteAlterarStatusFinanceiro))
                    throw new ControllerException("Você não tem permissão para alterar Status Financeiro");

                string serieRps = Request.Params("SerieRPS");

                decimal aliquotaICMSSimples = 0;
                decimal.TryParse(Request.Params("AliquotaICMSSimples"), out aliquotaICMSSimples);
                empresa.AliquotaICMSSimples = aliquotaICMSSimples;
                empresa.VersaoNFe = (VersaoNFe)int.Parse(Request.Params("VersaoNFe"));

                empresa.StatusEmail = enviarEmail ? "A" : "I";
                empresa.StatusEmailAdministrativo = enviarEmailAdministrativo ? "A" : "I";
                empresa.StatusEmailContador = enviarEmailContador ? "A" : "I";
                empresa.StatusEmissao = "S";

                empresa.Integrado = false;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    if (empresa.EmpresaPai != null && empresa.TipoAmbiente != empresa.EmpresaPai.TipoAmbiente)
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TipoDeAmbienteInformadoDiferenteDoAmbienteGeral);

                if (string.IsNullOrWhiteSpace(empresa.CEP) && !configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.FavorInformeCEPDaEmpresa);

                Dominio.Entidades.Empresa empresaExiste = empresa.Tipo == "E" ? repEmpresa.BuscarPorCodigoIntegracao(empresa.CodigoIntegracao) : repEmpresa.BuscarPorCNPJ(empresa.CNPJ);

                int codigoMobile = 0;

                if (empresa.EmpresaMobile)
                    ConfigurarEmpresaMobile(ref empresa, ref codigoMobile);

                SalvarTransportadorNFSe(empresa);
                SalvarCodigosComercialDistribuidor(empresa, repEmpresa);

                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> formulariosCadastrados = repTransportadorFormulario.BuscarPorEmpresa(empresa.Codigo);
                foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario formularioCadastrado in formulariosCadastrados)
                    repTransportadorFormulario.Deletar(formularioCadastrado);

                SalvarConfiguracaoIntegracao(empresa, unitOfWork);
                SalvarSeries(empresa, unitOfWork);
                SalvarConfiguracoesPorTipoOperacao(empresa, unitOfWork);
                SalvarConfiguracoes(empresa, unitOfWork);
                SalvarDadoBancario(empresa, unitOfWork);
                SalvarTermoQuitacao(empresa, unitOfWork);
                SalvarComponentesCTeImportado(empresa, unitOfWork);
                SalvarLayoutsEDI(empresa, unitOfWork);
                SalvarEstadosFeeder(empresa, unitOfWork);
                SalvarEmpresaFilial(empresa, unitOfWork);
                SalvarEmpresaFilialEmbarcador(empresa, unitOfWork);
                SalvarIntelipostDadosIntegracao(empresa, unitOfWork);
                SalvarIntelipostTipoOcorrencia(empresa, unitOfWork);
                SalvarPermissoes(empresa, unitOfWork);
                SalvarPermissoesTransportador(empresa, unitOfWork);
                SalvarUsuarioPadrao(empresa, codigoMobile, configuracaoTransportador, unitOfWork);
                SalvarIntegracaoKrona(empresa, unitOfWork);

                if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                    SalvarSerieRPS(serieRps, empresa, unitOfWork);

                SalvarTransportadorFilial(empresa, unitOfWork);
                SalvarRotasFreteValePedagio(empresa, unitOfWork);
                SalvarInscricoesST(empresa, unitOfWork);
                SalvarAutomacaoEmissaoNFSManual(empresa, unitOfWork);
                SalvarCodigosIntegracao(empresa, unitOfWork);
                SalvarOperadoresTransportador(empresa, unitOfWork);

                if (empresa.Configuracao.TipoIntegradoraCIOT.HasValue)
                    SalvarImpostoContratoFrete(empresa, unitOfWork);

                AdicionarOuAtualizarCondicoesPagamento(empresa, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    SalvarDadosCliente(empresa, unitOfWork, (configuracaoTransportador?.NaoAtualizarNomeFantasiaClienteAlterarDadosTransportador ?? false));

                await AdicionarIntegracaoTransportador(empresa, unitOfWork);

                if (empresaExiste == null || empresaExiste.Codigo == empresa.Codigo)
                {
                    repEmpresa.Atualizar(empresa, Auditado);
                    unitOfWork.CommitChanges();

                    Servicos.AtualizacaoEmpresa svcAtualizaEmpresa = new Servicos.AtualizacaoEmpresa(unitOfWork);
                    svcAtualizaEmpresa.Atualizar(empresa, unitOfWork);

                    Servicos.Embarcador.Login.Login svcLogin = new Servicos.Embarcador.Login.Login(unitOfWork);
                    svcLogin.BloquearDesbloquearEmpresaCommerce(empresa, statusFinanceiroAnterior, TipoServicoMultisoftware, unitOfWork);

                    if (!string.IsNullOrEmpty(empresa.NomeCertificado) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                    {
                        if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTeType == TipoEmissorDocumento.NSTech || Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFeType == TipoEmissorDocumento.NSTech)
                        {
                            string mensagemErro = string.Empty;
                            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                            if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresa, unitOfWork))
                            {
                                Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro}");
                                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoSalvarCertificado);
                            }

                            if (!integracaoNStech.IncluirAtualizarLogo(out mensagemErro, empresa, unitOfWork))
                                Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o logo para o emissor NStech: {mensagemErro}");
                        }

                        //Enviar Certificado Digital Key Vault
                        svcAtualizaEmpresa.EnviarCertificadoKeyVault(empresa, ClienteAcesso, unitOfWork);
                    }

                    return new JsonpResult(true);
                }
                else
                    throw new ControllerException(Localization.Resources.Transportadores.Transportador.JaExisteUmaEmpresaCadastradaComCNPJ + empresa.CNPJ);



            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAtualizar);
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
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);
                dynamic dynEmpresa = BuscarDynEmpresa(empresa, unitOfWork);

                return new JsonpResult(dynEmpresa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);
                repEmpresa.Deletar(empresa, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoEmOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExcluir);
                }

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarLogo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);

                if (empresa == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.EmpresaNaoFoiEncontrada);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoLogo");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NenhumaLogoSelecionadaParaAdicionar);

                Servicos.DTO.CustomFile file = arquivos[0];

                long byteCount = file.Length;
                if ((byteCount / 1024) > 60)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TamanhoMaximoPermitidoParaLogoDeSessentaKB);

                string extensaoArquivo = Path.GetExtension(file.FileName).ToLowerInvariant();
                string caminho;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    if (extensaoArquivo != ".png")
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.ExtensaoDoArquivoInvalidaSelecioneUmArquivoComExtensaoPng);

                    caminho = Servicos.FS.GetPath("C:\\Arquivos SGT\\Logos\\");

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, empresa.CNPJ + extensaoArquivo);
                }
                else
                {
                    if (extensaoArquivo != ".bmp")
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.ExtensaoDoArquivoInvalidaSelecioneUmArquivoComExtensaoBmp);

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas, Utilidades.String.OnlyNumbers(empresa.CNPJ));

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Logo", "DACTE");

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, file.FileName);
                }

                using (BinaryReader binaryReader = new BinaryReader(file.InputStream))
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                    file.SaveAs(caminho);

                    empresa.CaminhoLogoDacte = caminho;

                    repEmpresa.Atualizar(empresa);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.AlterouLogoDaEmpresa, unitOfWork);
                }

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTeType == TipoEmissorDocumento.NSTech || Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFeType == TipoEmissorDocumento.NSTech)
                {
                    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                    if (!integracaoNStech.IncluirAtualizarLogo(out string mensagemErro, empresa, unitOfWork))
                    {
                        Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o logo para o emissor NStech: {mensagemErro}");
                        return new JsonpResult(false, true, $"Ocorreu um erro ao enviar o logo para o emissor NStech: {mensagemErro}");
                    }
                }

                return new JsonpResult(new { LogoEmpresa = ObterLogoBase64(empresa) });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionarLogo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirLogo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);

                if (empresa == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.EmpresaNaoFoiEncontrada);

                if (!string.IsNullOrWhiteSpace(empresa.CaminhoLogoDacte) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.CaminhoLogoDacte))
                    Utilidades.IO.FileStorageService.Storage.Delete(empresa.CaminhoLogoDacte);
                else
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.EmpresaNaoFoiEncontrada);

                empresa.CaminhoLogoDacte = string.Empty;
                repEmpresa.Atualizar(empresa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.RemoveuLogoDaEmpresa, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoRemoverLogo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Empresa empresa;

                int codigoTransportador;
                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

                string senha = Request.Params("SenhaCertificado");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                if (empresa == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.TransportadorNaoFoiEncontrado);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.SelecioneUmCertificadoParaEnvio);

                Servicos.DTO.CustomFile file = files[0];

                if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pfx")
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ExtensaoDoArquivoInvalidaSelecioneUmArquivoComExtensaoPfx);

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas, Utilidades.String.OnlyNumbers(empresa.CNPJ), "Certificado");

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, empresa.CNPJ + ".pfx");

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }

                file.SaveAs(caminho);

                byte[] certificadoDigitalByteArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);

                /// Flag MachineKeySet não tem suporte no linux, em testes ocorreu erro.
                /// Flag DefaultKeySet ocorreu erro em alguns ambientes Windows, acredito que o motivo seja por que tenta usar o perfil do usuário atual e esteja faltando alguma permissão.
                X509Certificate2 certificado = null;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    certificado = new X509Certificate2(certificadoDigitalByteArray, senha, X509KeyStorageFlags.DefaultKeySet
                                            | X509KeyStorageFlags.PersistKeySet
                                            | X509KeyStorageFlags.Exportable);
                else
                    certificado = new X509Certificate2(certificadoDigitalByteArray, senha, X509KeyStorageFlags.MachineKeySet
                                            | X509KeyStorageFlags.PersistKeySet
                                            | X509KeyStorageFlags.Exportable);

                string cnpjCpfCertificado = certificado.ObterCnpj();
                if (!string.IsNullOrWhiteSpace(cnpjCpfCertificado) && cnpjCpfCertificado != empresa.CNPJ.Substring(0, 8))
                    return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.CNPJDoCertificado + " (" + cnpjCpfCertificado + ") " + Localization.Resources.Transportadores.Transportador.NaoPertenceAoCNPJDaEmpresa + "(" + empresa.CNPJ.Substring(0, 8) + ").");

                if (string.IsNullOrWhiteSpace(cnpjCpfCertificado))
                {
                    cnpjCpfCertificado = certificado.ObterCpf();

                    if (string.IsNullOrEmpty(cnpjCpfCertificado))
                    {
                        Servicos.Log.GravarInfo("Subject: " + certificado.Subject);
                        Servicos.Log.GravarInfo("Issuer: " + certificado.Issuer);
                        foreach (var ext in certificado.Extensions)
                        {
                            Servicos.Log.GravarInfo($"{ext.Oid.FriendlyName} ({ext.Oid.Value}): {BitConverter.ToString(ext.RawData)}");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(cnpjCpfCertificado))
                        return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.NaofoiPossivelFdentificarEmpresaDoCertificadoDigitalFavorVerificarCertificadoSelecionado + " \n\n" + Localization.Resources.Transportadores.Transportador.DuvidasEntrarEmContatoComSuporte);
                    else if (cnpjCpfCertificado != empresa.CNPJ)
                        return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.CNPJDoCertificado + "(" + cnpjCpfCertificado + ") " + Localization.Resources.Transportadores.Transportador.NaoPertenceAoCPFDoEmissor + " (" + empresa.CNPJ + ").");
                }

                empresa.SenhaCertificado = senha;
                empresa.SerieCertificado = certificado.SerialNumber;
                empresa.DataInicialCertificado = certificado.NotBefore;
                empresa.DataFinalCertificado = certificado.NotAfter;
                empresa.Initialize();
                empresa.NomeCertificado = caminho;

                repEmpresa.Atualizar(empresa);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empresa.GetChanges(), Localization.Resources.Transportadores.Transportador.AtualizouCertificadoDaEmpresa, unitOfWork);

                return new JsonpResult(new
                {
                    empresa.SerieCertificado,
                    DataInicialCertificado = empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy"),
                    DataFinalCertificado = empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy"),
                    empresa.SenhaCertificado
                });
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.SenhaInvalidaNaofoiPossivelExtrairAsInformacoesDoCertificado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoSalvarCertificado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransportador;
                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                if (empresa == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.TransportadorNaoEncontrado);

                if (!string.IsNullOrWhiteSpace(empresa.NomeCertificado) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    Utilidades.IO.FileStorageService.Storage.Delete(empresa.NomeCertificado);
                }

                empresa.DataInicialCertificado = null;
                empresa.DataFinalCertificado = null;
                empresa.SerieCertificado = string.Empty;
                empresa.SenhaCertificado = string.Empty;
                empresa.Initialize();
                empresa.NomeCertificado = string.Empty;

                repEmpresa.Atualizar(empresa);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empresa.GetChanges(), Localization.Resources.Transportadores.Transportador.RemoveuCertificadoDaEmpresa, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoRemoverCertificado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTransportador = 0;
                int.TryParse(Request.Params("CodigoTransportador"), out codigoTransportador);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                if (empresa == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.TransportadorNaoEncontrado);

                if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.CertificadoNaoEncontrado);

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                byte[] arquivoRetorno = null;
                if (!string.IsNullOrEmpty(empresa.NomeCertificadoKeyVault) && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarCertificadoKeyVault.Value)
                {
                    try
                    {
                        Servicos.SecretManagement.ISecretManager secretManager = new Servicos.SecretManagement.AzureKeyVaultSecretManager();
                        var certificado = secretManager.GetCertificate(empresa.NomeCertificadoKeyVault);
                        if (certificado != null)
                            arquivoRetorno = certificado.Export(X509ContentType.Pfx, empresa.SenhaCertificado);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }
                }

                if (arquivoRetorno == null)
                    arquivoRetorno = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(empresa.NomeCertificado);

                return Arquivo(arquivoRetorno, "application/x-pkcs12", empresa.CNPJ + ".pfx");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterCertificadoDigitalDoTransportador);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoEmpresa);

                if (codigoEmpresa == 0)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.FavorSelecioneUmaEmpresaCadastrada);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".xml")
                    {
                        Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                        string retorno = z.SalvarNFeAnterior(codigoEmpresa, file.InputStream, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                        if (retorno == "")
                        {
                            unitOfWork.CommitChanges();
                            return new JsonpResult(true, Localization.Resources.Transportadores.Transportador.ImportacaoDaNFeFoiRealizadaComSucesso);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, retorno);
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(true, Localization.Resources.Transportadores.Transportador.LayoutDoArquivoEstaForaDePadrao);
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ArquivoNaoEncontradoPorFavorVerifique);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoImportarDaNFeErro + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public async Task<IActionResult> VerificarCNPJCadastrado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);
                dynamic dynEmpresa = null;
                if (empresa != null)
                {
                    dynEmpresa = BuscarDynEmpresa(empresa, unitOfWork);
                }
                return new JsonpResult(dynEmpresa, true, "");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultarOsDadosDaEmpresa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RequisicaoConsultaCNPJ()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);
                dynamic dynEmpresa = null;
                if (empresa != null)
                {
                    dynEmpresa = BuscarDynEmpresa(empresa, unitOfWork);
                }
                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader(Localization.Resources.Transportadores.Transportador.Token, "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaCNPJ.RetornoOfRequisicaoFazendaPessoaJuridicaDggAjPvf requisicaoCNPJ = consultaCNPJ.SolicitarRequisicaoFazendaPessoaJuridica();
                if (requisicaoCNPJ.Status)
                {


                    string base64String = Convert.ToBase64String(requisicaoCNPJ.Objeto.Captcha, 0, requisicaoCNPJ.Objeto.Captcha.Length);
                    string htmlstr = "data:image/png;base64," + base64String;

                    var retorno = new
                    {
                        Empresa = dynEmpresa,
                        chaptcha = htmlstr,
                        Cookies = requisicaoCNPJ.Objeto.Cookies
                    };
                    return new JsonpResult(retorno);
                }
                else
                {
                    var retorno = new
                    {
                        Empresa = dynEmpresa,
                        chaptcha = false
                    };
                    return new JsonpResult(retorno, true, requisicaoCNPJ.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultarOsDadosDaEmpresa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultaCNPJCentralizada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                string cnpj = Request.Params("CNPJ").Replace(" ", "");

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader(Localization.Resources.Transportadores.Transportador.Token, "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = consultaCNPJ.ConsultarCadastroCentralizado(cnpj);
                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(3);

                        Dominio.Entidades.Localidade localidade = null; ;
                        if (retorno.Objeto.Pessoa.Endereco.Cidade.IBGE > 0)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(retorno.Objeto.Pessoa.Endereco.Cidade.IBGE);

                        if (localidade == null)
                        {
                            List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao);
                            localidade = listaLocalidades.Count() == 1 ? listaLocalidades.FirstOrDefault() : null;
                        }


                        string nomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) && !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "")) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;
                        string razaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;

                        if (nomeFantasia.Length > 79)
                            nomeFantasia = nomeFantasia.Substring(0, 79);
                        if (razaoSocial.Length > 79)
                            razaoSocial = razaoSocial.Substring(0, 79);

                        object retornoDados = new
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)),
                            Localidade = localidade != null ? new { localidade.Codigo, Descricao = localidade.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Numero,
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Trim() : string.Empty,
                            NomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : retorno.Objeto.Pessoa.NomeFantasia,
                            RazaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial,
                            EnderecoDigitado = true,
                            CNAE = retorno.Objeto.Pessoa.CNAE,
                            InscricaoEstadual = retorno.Objeto.Pessoa.RGIE
                        };
                        return new JsonpResult(retornoDados);
                    }
                    else
                    {
                        return new JsonpResult(false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return new JsonpResult(false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultarOsDadosDoCiente);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarCaptchaConsultaCNPJ()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica();
                requisicaoSefaz.Cookies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.WebService.CookieDinamico>>((string)Request.Params("Cookies"));
                string CNPJ = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));
                string captcha = Request.Params("Captcha");

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader(Localization.Resources.Transportadores.Transportador.Token, "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = consultaCNPJ.ConsultarPessoaJuridicaFazenda(requisicaoSefaz, CNPJ, captcha);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        string ie = "";
                        ConsultaCNPJ.RetornoOfstring retornoSintegra = consultaCNPJ.ConsultarInscricaoSintegra(CNPJ);
                        if (retornoSintegra.Status)
                            ie = retornoSintegra.Objeto;

                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao);
                        string cep = String.Format(@"{0:00\.000-000}", int.Parse(Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)));
                        object retornoDados = new
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = cep,
                            Localidade = listaLocalidades.Count() == 1 ? new { Codigo = listaLocalidades[0].Codigo, Descricao = listaLocalidades[0].DescricaoCidadeEstado } : null,
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Numero,
                            Telefone = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Trim() : string.Empty,
                            NomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : retorno.Objeto.Pessoa.NomeFantasia,
                            RazaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial,
                            EnderecoDigitado = true,
                            InscricaoEstadual = ie
                        };



                        return new JsonpResult(retornoDados);
                    }
                    else
                    {
                        return new JsonpResult(false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return new JsonpResult(false, retorno.Mensagem);
                }
                //return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoCarregarOsDadosDaReceita);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultaCNPJReceitaWS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {                
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ").Replace(" ", ""));

                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                string json_data = string.Empty;
                using (WebClient w = new WebClient())
                {
                    w.Encoding = System.Text.Encoding.UTF8;
                    json_data = w.DownloadString("https://www.receitaws.com.br/v1/cnpj/" + cnpj);
                }

                if (string.IsNullOrWhiteSpace(json_data))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ServicoIndisponivelNoMomento);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = new ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf();
                dynamic retornoReceitaWS = JsonConvert.DeserializeObject<dynamic>(json_data);

                ConverterObjetoRetornoWS(ref retorno, retornoReceitaWS);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(3);

                        Dominio.Entidades.Localidade localidade = null; ;
                        if (retorno.Objeto.Pessoa.Endereco.Cidade.IBGE > 0)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(retorno.Objeto.Pessoa.Endereco.Cidade.IBGE);

                        if (localidade == null)
                        {
                            List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao);
                            localidade = listaLocalidades.Count() == 1 ? listaLocalidades.FirstOrDefault() : null;
                        }

                        string nomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) && !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "")) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;
                        string razaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;

                        if (nomeFantasia.Length > 79)
                            nomeFantasia = nomeFantasia.Substring(0, 79);
                        if (razaoSocial.Length > 79)
                            razaoSocial = razaoSocial.Substring(0, 79);

                        object retornoDados = new
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)),
                            Localidade = localidade != null ? new { localidade.Codigo, Descricao = localidade.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Numero,
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Trim() : string.Empty,
                            NomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : retorno.Objeto.Pessoa.NomeFantasia,
                            RazaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial,
                            EnderecoDigitado = true,
                            CNAE = retorno.Objeto.Pessoa.CNAE,
                            InscricaoEstadual = retorno.Objeto.Pessoa.RGIE
                        };
                        return new JsonpResult(retornoDados);
                    }
                    else
                    {
                        return new JsonpResult(false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return new JsonpResult(false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultarOsDadosDoCNPJ);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaSeguros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.TipoOperacao, "TipoOperacao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Apolice, "Apolice", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Desconto, "Desconto", 20, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Apolice") propOrdenar = "ApoliceSeguro.NumeroApolice";
                else if (propOrdenar == "TipoOperacao") propOrdenar = "TipoOperacao.Descricao";

                // Dados do filtro
                int transportador = 0;
                int.TryParse(Request.Params("Transportador"), out transportador);

                // Consulta
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> listaGrid = repTransportadorAverbacao.Consultar(transportador, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTransportadorAverbacao.ContarConsulta(transportador);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                TipoOperacao = obj.TipoOperacao?.Descricao ?? string.Empty,
                                Apolice = obj.ApoliceSeguro?.DescricaoComSeguradora ?? string.Empty,
                                Desconto = obj.Desconto?.ToString("n4") ?? "-",
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista.ToList());
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarSeguro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork);

                // Converte parametros
                int codigoTransportador = 0;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);

                int codigoTipoOperacao = 0;
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);

                int codigoApoliceSeguro = 0;
                int.TryParse(Request.Params("Apolice"), out codigoApoliceSeguro);

                decimal? desconto = null;
                decimal descontoAux = 0m;
                if (decimal.TryParse(Request.Params("Desconto"), out descontoAux))
                    desconto = descontoAux;

                // Busca informacoes
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(codigoTransportador);
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigoApoliceSeguro);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                // Valida 
                if (transportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelEncontrarTransportador);

                if (apoliceSeguro == null && tipoOperacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.PrecisoSelecionarUmaApoliceDeSeguroOuUmTipoDeOperacao);

                if (!repTransportadorAverbacao.ValidaDuplicidade(codigoTransportador, codigoTipoOperacao, codigoApoliceSeguro))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.JaExisteUmaApoliceDeSeguroParaEsseTipoDeOperacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, transportador, null, Localization.Resources.Transportadores.Transportador.AdicionouApoliceDeSeguro + apoliceSeguro?.Descricao ?? (tipoOperacao?.Descricao ?? "") + ".", unitOfWork);

                // Vincula dados
                Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao tipoOperacaoSeguro = new Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao();
                tipoOperacaoSeguro.Empresa = transportador;
                tipoOperacaoSeguro.ApoliceSeguro = apoliceSeguro;
                tipoOperacaoSeguro.TipoOperacao = tipoOperacao;
                tipoOperacaoSeguro.Desconto = desconto;

                // Persiste dados
                repTransportadorAverbacao.Inserir(tipoOperacaoSeguro, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirSeguroPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao tipoOperacaoSeguro = repTransportadorAverbacao.BuscarPorCodigo(codigo);

                // Valida
                if (tipoOperacaoSeguro == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (tipoOperacaoSeguro.Empresa != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacaoSeguro.Empresa, null, Localization.Resources.Transportadores.Transportador.RemoveuApoliceDeSeguro + (tipoOperacaoSeguro.ApoliceSeguro?.Descricao ?? "") + ".", unitOfWork);

                // Persiste dados
                repTransportadorAverbacao.Deletar(tipoOperacaoSeguro, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoRemoverDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarIntelipostTipoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.TipoOperacao, "TipoOcorrencia", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CodigoIntegracao, "CodigoIntegracao", 12, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.MicroStatus, "MicroStatus", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.MacroStatus, "MacroStatus", 10, Models.Grid.Align.left);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);

                if (empresa == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NenhumRegistroSelecionado);

                List<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia> listaEmpresaIntelipostTipoOcorrencia = empresa.EmpresaIntelipostTipoOcorrencia.ToList();
                int count = listaEmpresaIntelipostTipoOcorrencia.Count();
                if (count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NenhumRegistroSalvoParaExportacao);

                var retorno = (from obj in listaEmpresaIntelipostTipoOcorrencia
                               select new
                               {
                                   obj.CodigoIntegracao,
                                   obj.MacroStatus,
                                   obj.MicroStatus,
                                   TipoOcorrencia = obj.TipoOcorrencia?.Descricao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(count);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", Localization.Resources.Transportadores.Transportador.TiposOcorrênciasTransportador + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExportarOsDadosSalvos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSeExisteEscalationList()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                var retorno = new
                {
                    EscalationList = repositorioMotivoChamado.VerificarSeExisteEscalationList(),
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

        #endregion

        #region Métodos Privados

        private void PreencherTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unitOfWork);

            int codigoPais = Request.GetIntParam("Pais");
            string ie = Utilidades.String.OnlyNumbers(Request.GetStringParam("InscricaoEstadual"));

            int diasRotatividadePallets;
            int.TryParse(Request.Params("DiasRotatividadePallets"), out diasRotatividadePallets);

            int tempoDelayHorasParaIniciarEmissao;
            int.TryParse(Request.Params("TempoDelayHorasParaIniciarEmissao"), out tempoDelayHorasParaIniciarEmissao);

            bool usarTipoOperacaoApolice;
            bool.TryParse(Request.Params("UsarTipoOperacaoApolice"), out usarTipoOperacaoApolice);

            TimeSpan? horaCorteCarregamento = Request.GetNullableTimeParam("HoraCorteCarregamento");

            empresa.CodigoEstabelecimento = Request.GetStringParam("CodigoEstabelecimento");
            empresa.CodigoDocumento = Request.GetStringParam("CodigoDocumento");
            empresa.CodigoEmpresa = Request.GetStringParam("CodigoEmpresa");
            empresa.CodigoCentroCusto = Request.GetStringParam("CodigoCentroCusto");
            empresa.GerarLoteEscrituracao = Request.GetBoolParam("GerarLoteEscrituracao");
            empresa.GerarLoteEscrituracaoCancelamento = Request.GetBoolParam("GerarLoteEscrituracaoCancelamento");
            empresa.ProvisionarDocumentos = Request.GetBoolParam("ProvisionarDocumentos");
            empresa.EmpresaPropria = Request.GetBoolParam("EmpresaPropria");
            empresa.ValidarMotoristaTeleriscoAoConfirmarTransportador = Request.GetBoolParam("ValidarMotoristaTeleriscoAoConfirmarTransportador");
            empresa.PontuacaoFixa = Request.GetIntParam("PontuacaoFixa");
            empresa.RecusarIntegracaoPODUnilever = Request.GetBoolParam("RecusarIntegracaoPODUnilever");
            empresa.DataValidadeIMO = Request.GetNullableDateTimeParam("DataValidadeIMO");
            empresa.IMO = Request.GetStringParam("IMO");

            int modeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscalCargaPropria");
            if (modeloDocumentoFiscal > 0)
                empresa.ModeloDocumentoFiscalCargaPropria = repModeloDocumentoFiscal.BuscarPorId(modeloDocumentoFiscal);
            else
                empresa.ModeloDocumentoFiscalCargaPropria = null;

            empresa.DiasRotatividadePallets = diasRotatividadePallets;
            empresa.TempoDelayHorasParaIniciarEmissao = tempoDelayHorasParaIniciarEmissao;
            empresa.HoraCorteCarregamento = horaCorteCarregamento;
            empresa.RazaoSocial = Request.Params("RazaoSocial");
            empresa.NomeFantasia = Request.Params("NomeFantasia");
            empresa.InscricaoEstadual = string.IsNullOrWhiteSpace(ie) ? "ISENTO" : ie;
            empresa.InscricaoMunicipal = Request.Params("InscricaoMunicipal");
            empresa.COTM = Request.Params("COTM");
            empresa.CNAE = Request.Params("CNAE");
            empresa.Suframa = Request.Params("Suframa");
            empresa.Setor = Request.Params("Setor");
            empresa.CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
            empresa.Endereco = Request.Params("Endereco");
            empresa.Complemento = Request.Params("Complemento");
            empresa.Numero = Request.Params("Numero");
            empresa.Bairro = Request.Params("Bairro");
            empresa.Telefone = Request.Params("Telefone");
            empresa.Localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Localidade")));
            empresa.Contato = Request.Params("Contato");
            empresa.TelefoneContato = Request.Params("TelefoneContato");
            empresa.NomeContador = Request.Params("NomeContador");
            empresa.CRCContador = Request.Params("CRCContador");
            empresa.TelefoneContador = Request.Params("TelefoneContador");
            empresa.TipoInclusaoPedagioBaseCalculoICMS = (TipoInclusaoPedagioBaseCalculoICMS)int.Parse(Request.Params("TipoInclusaoPedagioBaseCalculoICMS"));
            empresa.OptanteSimplesNacional = bool.Parse(Request.Params("OptanteSimplesNacional"));
            empresa.RegimeEspecial = (Dominio.Enumeradores.RegimeEspecialEmpresa)int.Parse(Request.Params("RegimeEspecial"));
            empresa.ExigeEtiquetagem = Request.Params("ExigeEtiquetagem").ToBool();
            empresa.EmissaoDocumentosForaDoSistema = bool.Parse(Request.Params("EmissaoDocumentosForaDoSistema"));
            empresa.EmissaoMDFeForaDoSistema = Request.GetBoolParam("EmissaoMDFeForaDoSistema");
            empresa.EmissaoCRTForaDoSistema = Request.GetNullableBoolParam("EmissaoCRTForaDoSistema");
            empresa.PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado = Request.GetDecimalParam("PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado");
            empresa.UsarTipoOperacaoApolice = usarTipoOperacaoApolice;
            empresa.EmiteMDFe20IntraEstadual = bool.Parse(Request.Params("EmiteMDFe20IntraEstadual"));
            empresa.IntegrarComGerenciadoraDeRisco = bool.Parse(Request.Params("IntegrarComGerenciadoraDeRisco"));
            empresa.PermiteEmitirSubcontratacao = bool.Parse(Request.Params("PermiteEmitirSubcontratacao"));
            empresa.UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora = bool.Parse(Request.Params("UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora"));

            empresa.RegistroANTT = Request.Params("RegistroANTT");
            empresa.FusoHorario = Request.Params("FusoHorario");
            empresa.TipoAmbiente = (Dominio.Enumeradores.TipoAmbiente)int.Parse(Request.Params("TipoAmbiente"));
            empresa.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro = Request.GetBoolParam("GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro");

            if (Request.GetStringParam("Status") == "A")
                empresa.Status = Request.GetStringParam("Status");
            else
            {
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Transportador_PermiteInativarTransportador))
                    empresa.Status = Request.GetStringParam("Status");
                else
                    throw new ControllerException("Você não possui Permissão para Inativar um Transportador");
            }

            empresa.RegimenTributario = Request.GetEnumParam<RegimenTributacao>("RegimenTributario");
            empresa.RegimeTributarioCTe = Request.GetEnumParam<RegimeTributarioCTe>("RegimeTributarioCTe");

            DateTime data;
            if (DateTime.TryParseExact(Request.Params("DataInicioAtividade"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                empresa.DataInicioAtividade = data;
            else
                empresa.DataInicioAtividade = null;

            empresa.Pais = codigoPais > 0 ? repositorioPais.BuscarPorCodigo(codigoPais) : null;

            //Vale Pedágio
            empresa.CompraValePedagio = Request.GetBoolParam("CompraValePedagio");

            List<TipoIntegracao> tiposIntegracaoValePedagio = Request.GetListEnumParam<TipoIntegracao>("TipoIntegracaoValePedagio");

            if (empresa.TiposIntegracaoValePedagio == null)
                empresa.TiposIntegracaoValePedagio = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            else
                empresa.TiposIntegracaoValePedagio.Clear();

            foreach (TipoIntegracao tipoIntegracaoValePedagio in tiposIntegracaoValePedagio)
                empresa.TiposIntegracaoValePedagio.Add(repTipoIntegracao.BuscarPorTipo(tipoIntegracaoValePedagio));

            empresa.Contribuinte = Request.GetBoolParam("Contribuinte");
            empresa.DataValidadeContribuinte = Request.GetNullableDateTimeParam("DataValidadeContribuinte");
            empresa.ValidarTransportadorContribuinte = Request.GetBoolParam("ValidarTransportadorContribuinte");
            empresa.EquiparadoTAC = Request.GetBoolParam("EquiparadoTAC");

            empresa.NaoGerarSMNaBrk = Request.GetBoolParam("NaoGerarSMNaBrk");
            empresa.IgnorarDocumentosDuplicadosNaEmissaoCTe = Request.GetBoolParam("IgnorarDocumentosDuplicadosNaEmissaoCTe");
            empresa.NaoPermitirReenviarIntegracaoDasCargasAppTrizy = Request.GetBoolParam("NaoPermitirReenviarIntegracaoDasCargasAppTrizy");
            empresa.NaoPermitirInformarInicioEFimPreTrip = Request.GetBoolParam("NaoPermitirInformarInicioEFimPreTrip");
            empresa.NaoGerarIntegracaoSuperAppTrizy = Request.GetBoolParam("NaoGerarIntegracaoSuperAppTrizy");
            empresa.MostrarOcorrenciasFiliaisMatriz = Request.GetBoolParam("MostrarOcorrenciasFiliaisMatriz");
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador()
            {
                RazaoSocial = Request.Params("Descricao"),
                NomeFantasia = Request.Params("NomeFantasia"),
                CodigoEmpresa = empresaPai.Codigo,
                CNPJ = Utilidades.String.OnlyNumbers(Request.Params("CNPJ")),
                SomenteProducao = Request.GetBoolParam("SomenteProducao"),
                SemEmpresaPai = true,
                TipoAmbiente = empresaPai.TipoAmbiente,
                SistemaEmissor = Request.GetEnumParam("SistemaEmissor", SistemaEmissor.Todos),
                SituacaoPesquisa = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                CodigoFilial = Request.GetIntParam("Filial"),
                UFFilialTransportador = Request.GetIntParam("LocalidadeFilialTransportador") > 0 ? repLocalidade.BuscarPorCodigo(Request.GetIntParam("LocalidadeFilialTransportador"))?.Estado?.Sigla ?? string.Empty : null,
                StatusTodos = Request.GetBoolParam("StatusTodos"),
                RaizCnpj = Request.GetStringParam("RaizCnpj"),
                SomenteEmpresaNaoTransportadoraPadraoContratacao = Request.GetBoolParam("SomenteEmpresaNaoTransportadoraPadraoContratacao"),
                SomenteTransportadoresPermitidosCadastroAgendamentoColeta = Request.GetBoolParam("SomenteTransportadoresPermitidosCadastroAgendamentoColeta"),
                SomenteTransportadoresManuais = Request.GetBoolParam("SomenteTransportadoresManuais"),
                CodigoEmpresaMatriz = Request.GetIntParam("EmpresaMatriz"),
                TelaMontagemCargaMapa = Request.GetBoolParam("TelaMontagemCargaMapa"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Localidade = Request.GetIntParam("Localidade") > 0 ? repLocalidade.BuscarPorCodigo(Request.GetIntParam("Localidade"))?.Descricao : string.Empty,
                Estado = Request.GetStringParam("Estado"),
                CodigosTransportadores = Request.GetListParam<int>("CodigosTransportadores"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoGrupoTransportador = Request.GetIntParam("GrupoTransportador")
            };

            if (Request.GetBoolParam("ConsultarSomenteAssociadoAoUsuario") && (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0 && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false))
                filtro.CodigosEmpresa = this.Usuario.Empresas.Select(c => c.Codigo).ToList();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                filtro.CodigoEmpresa = Usuario.Empresa.Codigo;
                filtro.TipoAmbiente = Usuario.Empresa.TipoAmbiente;
                filtro.SemEmpresaPai = false;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                filtro.CodigoEmpresa = Usuario.Empresa.Codigo * -1;
                filtro.TipoAmbiente = Usuario.Empresa.TipoAmbiente;
                filtro.SemEmpresaPai = false;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                filtro.Codigos = modalidadeFornecedor?.Transportadores?.Select(o => o.Codigo).ToList() ?? null;

                if (filtro.Codigos?.Count > 0)
                    filtro.UFFilialTransportador = string.Empty;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtro.CNPJ = Empresa.Matriz.FirstOrDefault().CNPJ_SemFormato;
                else
                    filtro.CNPJ = Empresa.CNPJ_SemFormato;

                filtro.BuscarFiliais = true;
            }

            if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica", valorPadrao: true))
                filtro.ListaCodigoTransportadorPermitidos = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);

            if (filtro.SomenteEmpresaNaoTransportadoraPadraoContratacao)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork).BuscarConfiguracaoPadrao();
                filtro.CodigoEmpresaTransportadoraPadraoContratacao = configuracaoTransportador?.TransportadorPadraoContratacao?.Codigo ?? 0;
            }

            int codigoPedidoBase = Request.GetIntParam("CodigoPedidoBase");
            if (codigoPedidoBase > 0 && filtro.SomenteEmpresaNaoTransportadoraPadraoContratacao)
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedidoBase);
                if ((pedido?.Empresa?.Codigo ?? 0) == filtro.CodigoEmpresaTransportadoraPadraoContratacao)
                    filtro.SomenteSubEmpresasTransportadoraPadraoContratacao = true;
            }

            return filtro;
        }

        private void AdicionarOuAtualizarCondicoesPagamento(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic condicoesPagamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CondicoesPagamento"));

            ExcluirCondicoesPagamentoRemovidas(empresa, condicoesPagamento, unitOfWork);
            InserirCondicoesPagamentoAdicionadas(empresa, condicoesPagamento, unitOfWork);
        }

        private void ExcluirCondicoesPagamentoRemovidas(Dominio.Entidades.Empresa empresa, dynamic condicoesPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (empresa.CondicoesPagamento?.Count > 0)
            {
                Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador repositorioCondicaoPagamentoTransportador = new Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic condicaoPagamento in condicoesPagamento)
                {
                    int? codigo = ((string)condicaoPagamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Transportadores.CondicaoPagamentoTransportador> condicoesPagamentoRemover = (from condicao in empresa.CondicoesPagamento where !listaCodigosAtualizados.Contains(condicao.Codigo) select condicao).ToList();

                foreach (Dominio.Entidades.Embarcador.Transportadores.CondicaoPagamentoTransportador condicaoPagamento in condicoesPagamentoRemover)
                {
                    repositorioCondicaoPagamentoTransportador.Deletar(condicaoPagamento);
                }

                if (condicoesPagamentoRemover.Count > 0)
                {
                    string descricaoAcao = condicoesPagamentoRemover.Count == 1 ? Localization.Resources.Transportadores.Transportador.CondicaoDePagamentoRemovida : Localization.Resources.Transportadores.Transportador.MultiplasCondicoesDePagamentoRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirCondicoesPagamentoAdicionadas(Dominio.Entidades.Empresa empresa, dynamic condicoesPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador repositorioCondicaoPagamentoTransportador = new Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            int totalCondicoesPagamentoAdicionadasOuAtualizadas = 0;

            foreach (dynamic condicaoPagamento in condicoesPagamento)
            {
                int? codigo = ((string)condicaoPagamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Transportadores.CondicaoPagamentoTransportador condicaoPagamentoTransportador;

                if (codigo.HasValue)
                    condicaoPagamentoTransportador = repositorioCondicaoPagamentoTransportador.BuscarPorCodigo(codigo.Value, true) ?? throw new ControllerException(Localization.Resources.Transportadores.Transportador.CondicaoDePagamentoNaoEncontrada);
                else
                    condicaoPagamentoTransportador = new Dominio.Entidades.Embarcador.Transportadores.CondicaoPagamentoTransportador();

                condicaoPagamentoTransportador.Empresa = empresa;
                condicaoPagamentoTransportador.DiaEmissaoLimite = ((string)condicaoPagamento.DiaEmissaoLimite).ToNullableInt();
                condicaoPagamentoTransportador.DiaMes = ((string)condicaoPagamento.DiaMes).ToNullableInt();
                condicaoPagamentoTransportador.DiaSemana = ((string)condicaoPagamento.DiaSemana).ToNullableEnum<DiaSemana>();
                condicaoPagamentoTransportador.DiasDePrazoPagamento = ((string)condicaoPagamento.DiasDePrazoPagamento).ToNullableInt();
                condicaoPagamentoTransportador.TipoPrazoPagamento = ((string)condicaoPagamento.TipoPrazoPagamento).ToNullableEnum<TipoPrazoPagamento>();
                condicaoPagamentoTransportador.VencimentoForaMes = (bool)condicaoPagamento.VencimentoForaMes;
                condicaoPagamentoTransportador.ConsiderarDiaUtilVencimento = (bool)condicaoPagamento.ConsiderarDiaUtilVencimento;

                int codigoTipoCarga = ((string)condicaoPagamento.CodigoTipoCarga).ToInt();
                condicaoPagamentoTransportador.TipoDeCarga = codigoTipoCarga > 0 ? repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoCarga) ?? throw new ControllerException(Localization.Resources.Transportadores.Transportador.TipoDeCargaNaoEncontrado) : null;

                int codigoTipoOperacao = ((string)condicaoPagamento.CodigoTipoOperacao).ToInt();
                condicaoPagamentoTransportador.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) ?? throw new ControllerException(Localization.Resources.Transportadores.Transportador.TipoDeOperacaoNaoEncontrado) : null;

                if (codigo.HasValue)
                {
                    repositorioCondicaoPagamentoTransportador.Atualizar(condicaoPagamentoTransportador);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = condicaoPagamentoTransportador.GetChanges();
                    if (alteracoes.Count > 0)
                        totalCondicoesPagamentoAdicionadasOuAtualizadas++;
                }
                else
                {
                    repositorioCondicaoPagamentoTransportador.Inserir(condicaoPagamentoTransportador);
                    totalCondicoesPagamentoAdicionadasOuAtualizadas++;
                }
            }

            if (empresa.IsInitialized() && (totalCondicoesPagamentoAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = totalCondicoesPagamentoAdicionadasOuAtualizadas == 1 ? Localization.Resources.Transportadores.Transportador.CondicaoDePagamentoAdicionadaOuAtualizada : Localization.Resources.Transportadores.Transportador.MultiplasCondicoesDePagamentoAdicionadasOuAtualizadas;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private dynamic BuscarDynEmpresa(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unitOfWork);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa repConfiguracaoIntegracaoEmpresa = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa(unitOfWork);
            Repositorio.ImpostoContratoFrete repImposto = new Repositorio.ImpostoContratoFrete(unitOfWork);
            Repositorio.IRImpostoContratoFrete repImpostoIR = new Repositorio.IRImpostoContratoFrete(unitOfWork);
            Repositorio.INSSImpostoContratoFrete repImpostoINSS = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
            Repositorio.EmpresaIntegracaoKrona repositorioEmpresaIntegracaoKrona = new Repositorio.EmpresaIntegracaoKrona(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio repTransportadorRotaFreteValePedagio = new Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorInscricaoST repTransportadorInscricaoST = new Repositorio.Embarcador.Transportadores.TransportadorInscricaoST(unitOfWork);

            Dominio.Entidades.ImpostoContratoFrete imposto = repImposto.BuscarPorEmpresa(empresa.Codigo);
            Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa configuracaoIntegracaoEmpresa = repConfiguracaoIntegracaoEmpresa.BuscarPorEmpresa(empresa.Codigo);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(empresa.Codigo);
            Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados integracaoFTP = repIntegracaoFTPDocumentosDestinados.BuscarPorEmpresa(empresa.Codigo);
            Dominio.Entidades.EmpresaIntegracaoKrona empresaIntegracaoKrona = repositorioEmpresaIntegracaoKrona.BuscarPorEmpresa(empresa.Codigo);
            List<Dominio.Entidades.EmpresaSerie> series = repSerie.BuscarTodosPorEmpresa(empresa.Codigo);
            List<Dominio.Entidades.PermissaoEmpresa> permissoes = repPermissaoEmpresa.BuscarPorEmpresa(empresa.Codigo);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> formulariosLiberados = repTransportadorFormulario.BuscarPorEmpresa(empresa.Codigo);
            List<Dominio.Entidades.IRImpostoContratoFrete> faixasIR = imposto != null ? repImpostoIR.BuscarPorImposto(imposto.Codigo) : new List<Dominio.Entidades.IRImpostoContratoFrete>();
            List<Dominio.Entidades.INSSImpostoContratoFrete> faixasINSS = imposto != null ? repImpostoINSS.BuscarPorImposto(imposto.Codigo) : new List<Dominio.Entidades.INSSImpostoContratoFrete>();
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio> rotasFrete = repTransportadorRotaFreteValePedagio.BuscarPorEmpresa(empresa.Codigo);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST> inscricoesST = repTransportadorInscricaoST.BuscarPorEmpresa(empresa.Codigo);

            bool sistemaEstrangeiro = ConfiguracaoEmbarcador.Pais != TipoPais.Brasil;

            var dynEmpresa = new
            {
                empresa.Codigo,
                empresa.CodigoIntegracao,
                empresa.CodigoCentroCusto,
                empresa.CodigoEmpresa,
                empresa.CodigoEstabelecimento,
                empresa.CodigoDocumento,
                empresa.DiasRotatividadePallets,
                empresa.TempoDelayHorasParaIniciarEmissao,
                HoraCorteCarregamento = empresa.HoraCorteCarregamento.HasValue ? empresa.HoraCorteCarregamento.Value.ToString(@"hh\:mm") : "",
                empresa.GerarLoteEscrituracao,
                empresa.RecusarIntegracaoPODUnilever,
                empresa.GerarLoteEscrituracaoCancelamento,
                empresa.ProvisionarDocumentos,
                empresa.EmpresaPropria,
                empresa.BloquearTransportador,
                empresa.EmpresaRetiradaProduto,
                empresa.NotificarDestinatarioAgendamentoColeta,
                empresa.MotivoBloqueio,
                ModeloDocumentoFiscalCargaPropria = new { Codigo = empresa.ModeloDocumentoFiscalCargaPropria?.Codigo ?? 0, Descricao = empresa.ModeloDocumentoFiscalCargaPropria?.Descricao ?? "" },
                empresa.RazaoSocial,
                empresa.NomeFantasia,
                TipoEmpresa = !string.IsNullOrWhiteSpace(empresa.Tipo) ? empresa.Tipo : "J",
                CNPJ = !string.IsNullOrWhiteSpace(empresa.Tipo) && empresa.Tipo.Equals("F") ? string.Empty : (sistemaEstrangeiro ? empresa.CNPJ_Identificacao_Exterior : empresa.CNPJ_Formatado),
                CPF = !string.IsNullOrWhiteSpace(empresa.Tipo) && empresa.Tipo.Equals("F") ? empresa.CNPJ_Formatado : string.Empty,
                empresa.InscricaoEstadual,
                empresa.InscricaoMunicipal,
                empresa.CNAE,
                empresa.Suframa,
                empresa.Setor,
                empresa.LiberarEmissaoSemAverbacao,
                OpenTech = configuracaoIntegracaoEmpresa?.CodigoIntegracao ?? string.Empty,
                CodigoClienteOpenTech = configuracaoIntegracaoEmpresa?.CodigoClienteOpenTech ?? 0,
                CodigoPASOpenTech = configuracaoIntegracaoEmpresa?.CodigoPASOpenTech ?? 0,
                Repom = new
                {
                    CodigoFilialRepom = empresa.Configuracao?.CodigoFilialRepom,
                },
                Electrolux = new
                {
                    IdentificadorTransportador = empresa.Configuracao?.IdentificadorTransportadorElectrolux,
                },
                Migrate = new
                {
                    TokenMigrate = empresa.Configuracao?.TokenMigrate ?? "",
                    PossuiIntegracaoMigrate = empresa.Configuracao?.PossuiIntegracaoMigrate ?? false,
                    IntegracaoMigrateRegimeTributario = empresa.Configuracao?.IntegracaoMigrateRegimeTributario != null ? new { Codigo = empresa.Configuracao?.IntegracaoMigrateRegimeTributario?.Codigo ?? 0, Descricao = empresa.Configuracao?.IntegracaoMigrateRegimeTributario?.Descricao ?? "" } : new { Codigo = 0, Descricao = "" },
                    EnviarObservacaoNaDiscriminacaoServicoMigrate = empresa.Configuracao?.EnviarObservacaoNaDiscriminacaoServicoMigrate ?? false,

                },
                CEP = !string.IsNullOrWhiteSpace(empresa.CEP) ? string.Format(@"{0:00\.000-000}", int.Parse(Utilidades.String.OnlyNumbers(empresa.CEP))) : string.Empty,
                empresa.Endereco,
                empresa.Complemento,
                empresa.Numero,
                empresa.Bairro,
                empresa.Telefone,
                empresa.LiberacaoParaPagamentoAutomatico,
                Localidade = new { empresa.Localidade.Codigo, empresa.Localidade.Descricao },
                empresa.Contato,
                empresa.TelefoneContato,
                empresa.NomeContador,
                empresa.CRCContador,
                empresa.TransportadorFerroviario,
                empresa.PermitirUtilizarCadastroAgendamentoColeta,
                empresa.COTM,
                empresa.IMO,
                DataValidadeIMO = empresa.DataValidadeIMO != null ? empresa.DataValidadeIMO.Value.ToString("dd/MM/yyyy") : "",
                Contador = new { Codigo = empresa.Contador != null ? empresa.Contador.CPF_CNPJ : 0, Descricao = empresa.Contador != null ? empresa.Contador.Nome : "" },
                empresa.TelefoneContador,
                DataInicialCertificado = empresa.DataInicialCertificado != null ? empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : "",
                DataFinalCertificado = empresa.DataFinalCertificado != null ? empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : "",
                empresa.SerieCertificado,
                empresa.SenhaCertificado,
                PossuiCertificado = !string.IsNullOrWhiteSpace(empresa.NomeCertificado) ? Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) : false,
                empresa.OptanteSimplesNacional,
                empresa.ExigeEtiquetagem,
                empresa.OptanteSimplesNacionalComExcessoReceitaBruta,
                empresa.EmissaoDocumentosForaDoSistema,
                empresa.EmissaoMDFeForaDoSistema,
                EmissaoCRTForaDoSistema = empresa.EmissaoCRTForaDoSistema ?? false,
                PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado = empresa.PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado.ToString("n2"),
                empresa.EmiteMDFe20IntraEstadual,
                empresa.IntegrarComGerenciadoraDeRisco,
                empresa.PermiteEmitirSubcontratacao,
                empresa.UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora,
                empresa.EmpresaMobile,
                empresa.OrdenarCargasMobileCrescente,
                empresa.CompraValePedagio,
                empresa.IntegrarCorreios,
                EnviarEmail = empresa.StatusEmail == "A" ? true : false,
                EnviarEmailAdministrativo = empresa.StatusEmailAdministrativo == "A" ? true : false,
                EnviarEmailContador = empresa.StatusEmailContador == "A" ? true : false,
                empresa.RegistroANTT,
                empresa.RegimenTributario,
                empresa.FusoHorario,
                empresa.RegimeEspecial,
                empresa.RegimeTributarioCTe,
                empresa.TipoAmbiente,
                DataInicioAtividade = empresa.DataInicioAtividade.HasValue ? empresa.DataInicioAtividade.Value.ToString("dd/MM/yyyy") : "",
                empresa.Status,
                empresa.TipoInclusaoPedagioBaseCalculoICMS,
                empresa.Email,
                empresa.EmailAdministrativo,
                empresa.EmailContador,
                empresa.EmailEnvioCanhoto,
                empresa.EmailEnvioCTeRejeitado,
                empresa.CaminhoLogoSistema,
                ProximoNumeroNFe = empresa.UltimoNumeroNFe,
                ProximoNumeroNFCe = empresa.UltimoNumeroNFCe,
                empresa.IdTokenNFCe,
                empresa.IdCSCNFCe,
                empresa.StatusFinanceiro,
                empresa.AliquotaICMSSimples,
                empresa.VersaoNFe,
                empresa.UsarTipoOperacaoApolice,
                empresa.PontuacaoFixa,
                SerieRPS = transportadorConfiguracaoNFSe != null ? transportadorConfiguracaoNFSe.SerieRPS : string.Empty,
                DataUltimaConsultaSintegra = empresa.DataUltimaConsultaSintegra?.ToDateString() ?? string.Empty,
                DataProximaConsultaSintegra = empresa.DataProximaConsultaSintegra?.ToDateString() ?? string.Empty,
                CondicaoPagamento = new
                {
                    empresa.AtivarCondicao,
                    CondicoesPagamento = (
                        from condicao in empresa.CondicoesPagamento
                        select new
                        {
                            condicao.Codigo,
                            CodigoTipoCarga = condicao.TipoDeCarga?.Codigo ?? 0,
                            CodigoTipoOperacao = condicao.TipoOperacao?.Codigo ?? 0,
                            DescricaoTipoCarga = condicao.TipoDeCarga?.Descricao ?? "",
                            DescricaoTipoOperacao = condicao.TipoOperacao?.Descricao ?? "",
                            condicao.DiaEmissaoLimite,
                            DiaEmissaoLimiteDescricao = condicao.DiaEmissaoLimite > 0 ? condicao.DiaEmissaoLimite.Value.ToString() : Localization.Resources.Transportadores.Transportador.SemConfiguracao,
                            condicao.DiaMes,
                            DiaMesDescricao = condicao.DiaMes > 0 ? condicao.DiaMes.Value.ToString() : Localization.Resources.Transportadores.Transportador.SemConfiguracao,
                            condicao.DiasDePrazoPagamento,
                            DiasDePrazoPagamentoDescricao = condicao.DiasDePrazoPagamento > 0 ? condicao.DiasDePrazoPagamento.Value.ToString() : Localization.Resources.Transportadores.Transportador.SemConfiguracao,
                            condicao.DiaSemana,
                            DiaSemanaDescricao = condicao.DiaSemana.HasValue ? condicao.DiaSemana.Value.ObterDescricao() : Localization.Resources.Transportadores.Transportador.SemConfiguracao,
                            condicao.TipoPrazoPagamento,
                            TipoPrazoPagamentoDescricao = condicao.TipoPrazoPagamento.HasValue ? condicao.TipoPrazoPagamento.Value.ObterDescricao() : Localization.Resources.Transportadores.Transportador.SemConfiguracao,
                            condicao.VencimentoForaMes,
                            condicao.ConsiderarDiaUtilVencimento,
                            VencimentoForaMesDescricao = condicao.VencimentoForaMes ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao
                        }
                    ).ToList()
                },
                ImpostoRendaCIOT = (
                    from o in faixasIR
                    select new
                    {
                        o.Codigo,
                        BaseCalculo = "0,00", //Deixa isso aqui... é pra não quebrar o mapeamento do list
                        De = new { val = o.ValorInicial.ToString("n2"), tipo = "decimal" },
                        Ate = new { val = o.ValorFinal.ToString("n2"), tipo = "decimal" },
                        Aplicar = new { val = o.PercentualAplicar.ToString("n2"), tipo = "decimal" },
                        Deduzir = new { val = o.ValorDeduzir.ToString("n2"), tipo = "decimal" },
                    }
                ).ToList(),
                ImpostoINSSCIOT = (
                    from o in faixasINSS
                    select new
                    {
                        o.Codigo,
                        BaseCalculo = "0,00", //Deixa isso aqui... é pra não quebrar o mapeamento do list
                        TetoRetencao = "0,00", //Deixa isso aqui... é pra não quebrar o mapeamento do list
                        De = new { val = o.ValorInicial.ToString("n2"), tipo = "decimal" },
                        Ate = new { val = o.ValorFinal.ToString("n2"), tipo = "decimal" },
                        Aplicar = new { val = o.PercentualAplicar.ToString("n2"), tipo = "decimal" },
                    }
                ).ToList(),
                ImpostoCIOTBaseCalculoIR = imposto?.PercentualBCIR.ToString("n2") ?? string.Empty,
                ImpostoCIOTBaseCalculoINSS = imposto?.PercentualBCINSS.ToString("n2") ?? string.Empty,
                ImpostoCIOTTetoRetencaoINSS = imposto?.ValorTetoRetencaoINSS.ToString("n2") ?? string.Empty,
                ImpostoCIOTAliquotaSEST = imposto?.AliquotaSEST.ToString("n2") ?? string.Empty,
                ImpostoCIOTAliquotaSENAT = imposto?.AliquotaSENAT.ToString("n2") ?? string.Empty,
                Configuracao = empresa.Configuracao != null ? new
                {
                    PagamentoMotoristaTipo = empresa.PagamentoMotoristaTipo != null ? new { empresa.PagamentoMotoristaTipo.Codigo, empresa.PagamentoMotoristaTipo.Descricao } : null,
                    TransportadoraPadraoContratacao = empresa.UtilizaTransportadoraPadraoContratacao,
                    empresa.Configuracao.PrincipalFilialEmissoraTMS,
                    empresa.Configuracao.EmpresaPadraoLancamentoGuarita,
                    EmitirEstadoOrigemForMesmoDaFilial = empresa.Configuracao.EstadosDeEmissao.Contains(empresa.Localidade.Estado),
                    SerieInterestadual = empresa.Configuracao.SerieInterestadual != null ? new { Codigo = empresa.Configuracao.SerieInterestadual.Codigo, Descricao = empresa.Configuracao.SerieInterestadual.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    SerieIntraestadual = empresa.Configuracao.SerieIntraestadual != null ? new { Codigo = empresa.Configuracao.SerieIntraestadual.Codigo, Descricao = empresa.Configuracao.SerieIntraestadual.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    SerieMDFe = empresa.Configuracao.SerieMDFe != null ? new { Codigo = empresa.Configuracao.SerieMDFe.Codigo, Descricao = empresa.Configuracao.SerieMDFe.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    CSTPISCOFINS = empresa.Configuracao.CSTPISCOFINS,
                    AliquotaPIS = (empresa.Configuracao.AliquotaPIS ?? 0m).ToString("n2"),
                    AliquotaCOFINS = (empresa.Configuracao.AliquotaCOFINS ?? 0m).ToString("n2"),
                    empresa.CalculaIBPTNFe,
                    empresa.TipoEmissaoIntramunicipal,
                    empresa.SempreEmitirNFS,
                    empresa.CasasQuantidadeProdutoNFe,
                    empresa.CasasValorProdutoNFe,
                    empresa.TipoImpressaoPedidoVenda,
                    empresa.GerarParcelaAutomaticamente,
                    empresa.EmitirVendaPrazoNFCe,
                    empresa.TipoLancamentoFinanceiroSemOrcamento,
                    TipoPagamentoRecebimento = empresa.TipoPagamentoRecebimento != null ? new { empresa.TipoPagamentoRecebimento.Codigo, empresa.TipoPagamentoRecebimento.Descricao } : null,
                    NaturezaDaOperacaoNFCe = empresa.NaturezaDaOperacaoNFCe != null ? new { empresa.NaturezaDaOperacaoNFCe.Codigo, empresa.NaturezaDaOperacaoNFCe.Descricao } : null,
                    empresa.UtilizaIntegracaoDocumentosDestinado,
                    EnderecoFTP = integracaoFTP != null ? integracaoFTP.EnderecoFTP : string.Empty,
                    Usuario = integracaoFTP != null ? integracaoFTP.Usuario : string.Empty,
                    Senha = integracaoFTP != null ? integracaoFTP.Senha : string.Empty,
                    Porta = integracaoFTP != null ? integracaoFTP.Porta : string.Empty,
                    DiretorioInput = integracaoFTP != null ? integracaoFTP.DiretorioInput : string.Empty,
                    DiretorioOutput = integracaoFTP != null ? integracaoFTP.DiretorioOutput : string.Empty,
                    DiretorioXML = integracaoFTP != null ? integracaoFTP.DiretorioXML : string.Empty,
                    Passivo = integracaoFTP != null ? integracaoFTP.Passivo : false,
                    UtilizarSFTP = integracaoFTP != null ? integracaoFTP.UtilizarSFTP : false,
                    SSL = integracaoFTP != null ? integracaoFTP.SSL : false,
                    HabilitarCIOT = empresa.Configuracao.TipoIntegradoraCIOT.HasValue,
                    ObrigatoriedadeCIOTEmissaoMDFe = empresa.Configuracao.ObrigatoriedadeCIOTEmissaoMDFe ?? false,
                    empresa.Configuracao.EncerrarCIOTPorViagem,
                    TipoIntegracaoCIOT = empresa.Configuracao.TipoIntegradoraCIOT,
                    CodigoIntegracaoEfrete = empresa.Configuracao.CodigoIntegradorEFrete,
                    empresa.ObservacaoSimplesNacional,
                    empresa.ObservacaoCTe,
                    TipoIntegracao = empresa.TipoIntegracaoCarga != null ? empresa.TipoIntegracaoCarga.Tipo : TipoIntegracao.NaoInformada,
                    TipoMovimento = empresa.TipoMovimento != null ? new { empresa.TipoMovimento.Codigo, empresa.TipoMovimento.Descricao } : null,
                    empresa.HabilitaLancamentoProdutoLote,
                    FormaRateioSVM = empresa.FormaRateioSVM.HasValue ? empresa.FormaRateioSVM.Value : FormaRateioSVM.Nenhum,
                    SVMMesmoQueMultimodal = empresa.SVMMesmoQueMultimodal.HasValue ? empresa.SVMMesmoQueMultimodal.Value : false,
                    SVMTerminaisPortuarioOrigemDestino = empresa.SVMTerminaisPortuarioOrigemDestino.HasValue ? empresa.SVMTerminaisPortuarioOrigemDestino.Value : false,
                    SVMBUSPortoOrigemDestino = empresa.SVMBUSPortoOrigemDestino.HasValue ? empresa.SVMBUSPortoOrigemDestino.Value : false,
                    empresa.QuantidadeMaximaEmailRPS,
                    empresa.AliquotaICMSNegociado,
                    CodigoServicoCorreios = empresa.CodigoServicoCorreios ?? string.Empty,
                    empresa.SubtraiDescontoBaseICMS,
                    empresa.ArmazenarDanfeParaSMS,
                    empresa.AtivarEnvioDanfeSMS,
                    empresa.GerarCreditoC197SPEDFiscal,
                    empresa.TokenSMS,
                    empresa.HabilitarTabelaValorOrdemServicoVenda,
                    empresa.PermiteAlterarEmpresaOrdemServicoVenda,
                    empresa.HabilitarNumeroInternoOrdemServicoVenda,
                    empresa.Configuracao.FraseSecretaNFSe,
                    empresa.Configuracao.SenhaNFSe,
                    PerfilSPEDFiscal = empresa.Configuracao.Perfil,
                    empresa.CNPJContabilidade,
                    empresa.CPFContabilidade,
                    empresa.UtilizaDataVencimentoNaEmissao,
                    empresa.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual,
                    empresa.HabilitarEtiquetaProdutosNFe,
                    empresa.PermitirImportarApenasPedidoVendaFinalizado,
                    empresa.CadastrarProdutoAutomaticamenteDocumentoEntrada,
                    empresa.DeixarPadraoFinalizadoDocumentoEntrada,
                    empresa.ControlarEstoqueNegativo,
                    empresa.NumeroCertificadoIdoneidade,
                    empresa.VisualizarSomenteClientesAssociados,
                    empresa.RestringirLocaisCarregamentoAutorizadosMotoristas,
                    empresa.OrdenarCargasMobileCrescente,
                    FormaDeducaoValePedagio = empresa.FormaDeducaoValePedagio != null ? empresa.FormaDeducaoValePedagio.Value : FormaDeducaoValePedagio.NaoAplicado,
                    VersaoCTe = !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoCTe) ? empresa.Configuracao.VersaoCTe : "4.00",
                    NaoComprarValePedagioCargaTransbordo = empresa.Configuracao?.NaoComprarValePedagioCargaTransbordo ?? false,
                    EnviarNovoImposto = empresa.Configuracao?.EnviarNovoImposto ?? false,
                    ReduzirPISCOFINSBaseCalculoIBSCBS = empresa.Configuracao?.ReduzirPISCOFINSBaseCalculoIBSCBS ?? false
                } : new
                {
                    PagamentoMotoristaTipo = empresa.PagamentoMotoristaTipo != null ? new { empresa.PagamentoMotoristaTipo.Codigo, empresa.PagamentoMotoristaTipo.Descricao } : null,
                    TransportadoraPadraoContratacao = empresa.UtilizaTransportadoraPadraoContratacao,
                    PrincipalFilialEmissoraTMS = false,
                    EmpresaPadraoLancamentoGuarita = false,
                    EmitirEstadoOrigemForMesmoDaFilial = false,
                    SerieInterestadual = new { Codigo = 0, Descricao = "" },
                    SerieIntraestadual = new { Codigo = 0, Descricao = "" },
                    SerieMDFe = new { Codigo = 0, Descricao = "" },
                    CSTPISCOFINS = "",
                    AliquotaPIS = "0,00",
                    AliquotaCOFINS = "0,00",
                    CalculaIBPTNFe = true,
                    empresa.TipoEmissaoIntramunicipal,
                    empresa.SempreEmitirNFS,
                    CasasQuantidadeProdutoNFe = 4,
                    CasasValorProdutoNFe = 5,
                    TipoImpressaoPedidoVenda = TipoImpressaoPedidoVenda.Pedido,
                    GerarParcelaAutomaticamente = false,
                    EmitirVendaPrazoNFCe = false,
                    TipoLancamentoFinanceiroSemOrcamento = TipoLancamentoFinanceiroSemOrcamento.Liberar,
                    TipoPagamentoRecebimento = new { Codigo = 0, Descricao = "" },
                    NaturezaDaOperacaoNFCe = new { Codigo = 0, Descricao = "" },
                    UtilizaIntegracaoDocumentosDestinado = false,
                    EnderecoFTP = string.Empty,
                    Usuario = string.Empty,
                    Senha = string.Empty,
                    Porta = string.Empty,
                    DiretorioInput = string.Empty,
                    DiretorioOutput = string.Empty,
                    DiretorioXML = string.Empty,
                    Passivo = false,
                    UtilizarSFTP = false,
                    SSL = false,
                    HabilitarCIOT = false,
                    ObrigatoriedadeCIOTEmissaoMDFe = false,
                    EncerrarCIOTPorViagem = false,
                    TipoIntegracaoCIOT = (Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT?)null,
                    CodigoIntegracaoEfrete = string.Empty,
                    ObservacaoSimplesNacional = string.Empty,
                    ObservacaoCTe = string.Empty,
                    TipoIntegracao = TipoIntegracao.NaoInformada,
                    TipoMovimento = new { Codigo = 0, Descricao = "" },
                    HabilitaLancamentoProdutoLote = false,
                    FormaRateioSVM = empresa.FormaRateioSVM.HasValue ? empresa.FormaRateioSVM.Value : FormaRateioSVM.Nenhum,
                    SVMMesmoQueMultimodal = empresa.SVMMesmoQueMultimodal.HasValue ? empresa.SVMMesmoQueMultimodal.Value : false,
                    SVMTerminaisPortuarioOrigemDestino = empresa.SVMTerminaisPortuarioOrigemDestino.HasValue ? empresa.SVMTerminaisPortuarioOrigemDestino.Value : false,
                    SVMBUSPortoOrigemDestino = empresa.SVMBUSPortoOrigemDestino.HasValue ? empresa.SVMBUSPortoOrigemDestino.Value : false,
                    QuantidadeMaximaEmailRPS = 0,
                    AliquotaICMSNegociado = (decimal)0,
                    CodigoServicoCorreios = empresa.CodigoServicoCorreios ?? string.Empty,
                    SubtraiDescontoBaseICMS = false,
                    ArmazenarDanfeParaSMS = false,
                    AtivarEnvioDanfeSMS = false,
                    GerarCreditoC197SPEDFiscal = false,
                    TokenSMS = string.Empty,
                    HabilitarTabelaValorOrdemServicoVenda = false,
                    PermiteAlterarEmpresaOrdemServicoVenda = false,
                    HabilitarNumeroInternoOrdemServicoVenda = false,
                    FraseSecretaNFSe = string.Empty,
                    SenhaNFSe = string.Empty,
                    PerfilSPEDFiscal = Dominio.Enumeradores.PerfilEmpresa.A,
                    CNPJContabilidade = string.Empty,
                    CPFContabilidade = string.Empty,
                    UtilizaDataVencimentoNaEmissao = false,
                    BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual = false,
                    HabilitarEtiquetaProdutosNFe = false,
                    PermitirImportarApenasPedidoVendaFinalizado = false,
                    CadastrarProdutoAutomaticamenteDocumentoEntrada = false,
                    DeixarPadraoFinalizadoDocumentoEntrada = false,
                    ControlarEstoqueNegativo = false,
                    NumeroCertificadoIdoneidade = string.Empty,
                    VisualizarSomenteClientesAssociados = false,
                    RestringirLocaisCarregamentoAutorizadosMotoristas = false,
                    OrdenarCargasMobileCrescente = false,
                    FormaDeducaoValePedagio = FormaDeducaoValePedagio.NaoAplicado,
                    VersaoCTe = "4.00",
                    NaoComprarValePedagioCargaTransbordo = false,
                    EnviarNovoImposto = false,
                    ReduzirPISCOFINSBaseCalculoIBSCBS = false,
                },
                Series = (
                    from obj in series
                    orderby obj.Numero, obj.Tipo
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        obj.Status,
                        obj.Tipo,
                        obj.ProximoNumeroDocumento,
                        obj.NaoGerarCargaAutomaticamente
                    }
                ).ToList(),
                ConfiguracaoTipoOperacaos = (
                    from obj in empresa.ConfiguracoesTipoOperacao
                    select new
                    {
                        obj.Codigo,
                        TipoOperacao = new { obj.TipoOperacao.Codigo, obj.TipoOperacao.Descricao },
                        SerieIntraestadual = obj.SerieIntraestadual != null ? new { obj.SerieIntraestadual.Codigo, Descricao = obj.SerieIntraestadual.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                        SerieInterestadual = obj.SerieInterestadual != null ? new { obj.SerieInterestadual.Codigo, Descricao = obj.SerieInterestadual.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                        SerieMDFe = obj.SerieMDFe != null ? new { obj.SerieMDFe.Codigo, Descricao = obj.SerieMDFe.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                        obj.TipoEmissaoIntramunicipal,
                        SerieDentroEstado = obj.SerieIntraestadual != null ? obj.SerieIntraestadual.Numero.ToString() : "",
                        SerieForaEstado = obj.SerieInterestadual != null ? obj.SerieInterestadual.Numero.ToString() : ""
                    }
                ).ToList(),
                ConfiguracaoLayoutEDI = (
                    from obj in empresa.TransportadorLayoutsEDI
                    orderby obj.LayoutEDI.Descricao
                    select new
                    {
                        obj.Codigo,
                        CodigoLayoutEDI = obj.LayoutEDI.Codigo,
                        DescricaoLayoutEDI = obj.LayoutEDI.Descricao,
                        TipoIntegracao = obj.TipoIntegracao.Tipo,
                        DescricaoTipoIntegracao = obj.TipoIntegracao.Descricao,
                        obj.Diretorio,
                        obj.Emails,
                        obj.EnderecoFTP,
                        obj.Passivo,
                        obj.Porta,
                        obj.Senha,
                        obj.Usuario,
                        obj.UtilizarSFTP,
                        obj.SSL,
                        obj.CriarComNomeTemporaraio
                    }
                ).ToList(),
                Filiais = (
                    from obj in empresa.Filiais
                    orderby obj.Codigo
                    select new
                    {
                        Empresa = new
                        {
                            obj.Codigo,
                            Descricao = obj.RazaoSocial,
                        },
                        Localidade = obj.Localidade.DescricaoCidadeEstado
                    }
                ).ToList(),
                EstadosFeeder = (
                    from obj in empresa.EstadosFeeder
                    orderby obj.Codigo
                    select new
                    {
                        Estado = new
                        {
                            obj.Codigo,
                            obj.Descricao,
                        }
                    }
                ).ToList(),
                IntelipostDadosIntegracao = (
                    from obj in empresa.EmpresaIntelipostIntegracao
                    orderby obj.Codigo
                    select new
                    {
                        obj.Token,
                        CanalEntrega = new
                        {
                            Codigo = obj.CanalEntrega?.Codigo ?? 0,
                            Descricao = obj.CanalEntrega?.Descricao ?? "",
                        },

                    }
                ).ToList(),
                IntelipostTipoOcorrencia = (
                    from obj in empresa.EmpresaIntelipostTipoOcorrencia
                    orderby obj.Codigo
                    select new
                    {
                        obj.CodigoIntegracao,
                        obj.MacroStatus,
                        obj.MicroStatus,
                        TipoOcorrencia = new
                        {
                            Codigo = obj.TipoOcorrencia?.Codigo ?? 0,
                            Descricao = obj.TipoOcorrencia?.Descricao ?? "",
                        },

                    }
                ).ToList(),
                FiliaisEmbarcador = (
                    from obj in empresa.FiliaisEmbarcadorHabilitado
                    orderby obj.Codigo
                    select new
                    {
                        Filial = new
                        {
                            obj.Codigo,
                            obj.Descricao,
                        },
                        Localidade = obj.Localidade.DescricaoCidadeEstado
                    }
                ).ToList(),
                Permissoes = (
                    from obj in permissoes
                    where obj.PermissaoDeAcesso == "A"
                    select new
                    {
                        obj.Pagina.Codigo,
                        obj.Pagina.Descricao
                    }
                ).ToList(),
                empresa.TransportadorAdministrador,
                empresa.HabilitaSincronismoDocumentosDestinados,
                PerfilAcessoTransportador = empresa.PerfilAcessoTransportador != null ? new { empresa.PerfilAcessoTransportador.Codigo, empresa.PerfilAcessoTransportador.Descricao } : null,
                FormulariosTransportador = (
                    from obj in formulariosLiberados
                    select new
                    {
                        obj.CodigoFormulario,
                        obj.SomenteLeitura,
                        PermissoesPersonalizadas = (
                            from pp in obj.TransportadorFormularioPermissaoesPersonalizadas
                            select new
                            {
                                pp.CodigoPermissao
                            }
                        ).ToList(),
                    }
                ).ToList(),
                ModulosTransportador = (
                    from codigoModulo in empresa.ModulosLiberados
                    select new
                    {
                        CodigoModulo = codigoModulo
                    }
                ).ToList(),
                TransportadorFiliais = (
                    from obj in empresa.TransportadorFiliais
                    orderby obj.Codigo
                    select new
                    {
                        obj.Codigo,
                        CNPJ = obj.CNPJ_Formatado
                    }
                ).ToList(),
                CodigosComercialDistribuidor = (
                    from codigosComercialDistribuidorTransportador in empresa.CodigosComercialDistribuidor
                    select new
                    {
                        Codigo = codigosComercialDistribuidorTransportador,
                        CodigoComercialDistribuidor = codigosComercialDistribuidorTransportador
                    }
                ).ToList(),
                DadoBancario = new
                {
                    Banco = empresa.Banco != null ? new { Codigo = empresa.Banco.Codigo, Descricao = empresa.Banco.Descricao } : null,
                    empresa.Agencia,
                    Digito = empresa.DigitoAgencia,
                    empresa.NumeroConta,
                    empresa.CnpjIpef,
                    TipoConta = empresa.TipoContaBanco != null ? (int)empresa.TipoContaBanco : (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente,
                    empresa.AntecipacaoPagamento,
                    EmpresaFavorecida = new { Codigo = empresa.EmpresaFavorecida?.Codigo ?? 0, Descricao = empresa.EmpresaFavorecida?.Descricao ?? string.Empty },
                    empresa.TipoChavePIX,
                    ChavePIXCPFCNPJ = empresa.TipoChavePIX == Dominio.ObjetosDeValor.Enumerador.TipoChavePix.CPFCNPJ ? empresa.ChavePIX : "",
                    ChavePIXEmail = empresa.TipoChavePIX == Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Email ? empresa.ChavePIX : "",
                    ChavePIXCelular = empresa.TipoChavePIX == Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Celular ? empresa.ChavePIX : "",
                    ChavePIXAleatoria = empresa.TipoChavePIX == Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Aleatoria ? empresa.ChavePIX : "",
                },
                ComponentesCTesImportados = ObterComponentesCTesImportados(empresa, unitOfWork),
                CodigosIntegracao = ObterCodigosIntegracao(empresa, unitOfWork),
                TipoIntegracaoValePedagio = empresa.TiposIntegracaoValePedagio.Select(o => o.Tipo).ToList(),
                IntegracaoKrona = new
                {
                    Senha = empresaIntegracaoKrona?.Senha ?? "",
                    Usuario = empresaIntegracaoKrona?.Usuario ?? ""
                },
                empresa.ValidarMotoristaTeleriscoAoConfirmarTransportador,
                empresa.GerarPedidoAoReceberCarga,
                LogoEmpresa = ObterLogoBase64(empresa),
                RotasFreteValePedagio = (
                        from rotaFrete in rotasFrete
                        select new
                        {
                            rotaFrete.Codigo,
                            RotaFrete = new { rotaFrete.RotaFrete.Codigo, rotaFrete.RotaFrete.Descricao },
                            rotaFrete.TipoRotaFrete
                        }
                    ).ToList(),
                InscricoesST = (
                        from inscricao in inscricoesST
                        select new
                        {
                            inscricao.Codigo,
                            Estado = new { Codigo = inscricao.Estado?.Sigla ?? string.Empty, Descricao = inscricao.Estado?.Nome ?? string.Empty },
                            InscricaoEstadual = inscricao.InscricaoST
                        }
                    ).ToList(),
                Anexos = (
                        from anexo in empresa.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                TransportadorNFSe = new
                {
                    empresa.EmissaoNFSeForaDoSistema,
                    empresa.NaoIncrementarNumeroLoteRPSAutomaticamente,
                    empresa.NFSeNacional,
                },
                AutomacaoEmissaoNFSManual = new
                {
                    Periodicidade = empresa.PeriodicidadeEmissaoNFSManual,
                    DiaSemana = empresa.DiaSemanaEmissaoNFSManual,
                    DiaMes = empresa.DiaMesEmissaoNFSManual
                },
                Operadores = (
                    from obj in empresa.Operadores
                    orderby obj.Descricao
                    select new
                    {
                        Tipo = new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    }
                     ).ToList(),
                Pais = new { Codigo = empresa.Pais?.Codigo ?? 0, Descricao = empresa.Pais?.Descricao ?? string.Empty },
                empresa.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro,

                LeituraFTP = new
                {
                    EnderecoFTP = integracaoFTP != null ? integracaoFTP.EnderecoFTP : string.Empty,
                    Usuario = integracaoFTP != null ? integracaoFTP.Usuario : string.Empty,
                    Senha = integracaoFTP != null ? integracaoFTP.Senha : string.Empty,
                    Porta = integracaoFTP != null ? integracaoFTP.Porta : string.Empty,
                    DiretorioInput = integracaoFTP != null ? integracaoFTP.DiretorioInput : string.Empty,
                    DiretorioOutput = integracaoFTP != null ? integracaoFTP.DiretorioOutput : string.Empty,
                    DiretorioXML = integracaoFTP != null ? integracaoFTP.DiretorioXML : string.Empty,
                    Passivo = integracaoFTP != null ? integracaoFTP.Passivo : false,
                    UtilizarSFTP = integracaoFTP != null ? integracaoFTP.UtilizarSFTP : false,
                    SSL = integracaoFTP != null ? integracaoFTP.SSL : false,
                },
                TermoQuitacao = new
                {
                    empresa.GerarAvisoPeriodico,
                    empresa.ACadaAvisoPeriodico,
                    empresa.PeriodoAvisoPeriodico,
                    empresa.TempoAguardarParaGerarTermo,
                    DataFimTermoQuitacaoInicial = empresa.DataFimTermoQuitacaoInicial?.ToString("dd/MM/yyyy hh:mm:ss") ?? "",
                    empresa.TipoGeracaoTermo,
                    empresa.TipoTermo,
                    empresa.ACadaTipoTermo,
                    empresa.PeriodoTipoTermo,
                },
                empresa.Contribuinte,
                DataValidadeContribuinte = empresa.DataValidadeContribuinte?.ToString("dd/MM/yyyy hh:mm:ss") ?? "",
                empresa.ValidarTransportadorContribuinte,
                empresa.EquiparadoTAC,
                empresa.NaoGerarSMNaBrk,
                empresa.IgnorarDocumentosDuplicadosNaEmissaoCTe,
                empresa.NaoPermitirReenviarIntegracaoDasCargasAppTrizy,
                empresa.NaoPermitirInformarInicioEFimPreTrip,
                empresa.NaoGerarIntegracaoSuperAppTrizy,
                empresa.MostrarOcorrenciasFiliaisMatriz,
            };

            return dynEmpresa;
        }

        private string ConfigurarEmpresaMobile(ref Dominio.Entidades.Empresa empresa, ref int codigoMobile)
        {
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = ClienteAcesso;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCFP(empresa.CNPJ);
                bool inserir = false;
                if (usuarioMobile == null)
                {
                    inserir = true;
                    usuarioMobile = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile();
                    usuarioMobile.CPF = empresa.CNPJ;
                    usuarioMobile.Celular = "";
                    usuarioMobile.Nome = empresa.RazaoSocial;
                    usuarioMobile.Sessao = "";
                }
                usuarioMobile.DataSessao = DateTime.Now;
                usuarioMobile.Senha = "";
                usuarioMobile.Ativo = true;

                if (inserir)
                    repUsuarioMobile.Inserir(usuarioMobile);
                else
                    repUsuarioMobile.Atualizar(usuarioMobile);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = null;
                if (usuarioMobile.Clientes != null)
                    usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == clienteAcesso.Cliente.Codigo select obj).FirstOrDefault();

                inserir = false;
                if (usuarioMobileCliente == null)
                {
                    usuarioMobileCliente = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente();
                    usuarioMobileCliente.Cliente = clienteAcesso.Cliente;
                    usuarioMobileCliente.UsuarioMobile = usuarioMobile;
                    usuarioMobileCliente.BaseHomologacao = clienteAcesso.URLHomologacao;
                    inserir = true;
                }
                else
                {
                    if (usuarioMobileCliente.BaseHomologacao == false && ClienteAcesso.URLHomologacao)
                        return Localization.Resources.Transportadores.Transportador.EsseMotoristaJaEstaAptoUtilizarAplicativoEmProducaoNaoSendoPossivelConfigurarMesmoEmHomologacao;

                    usuarioMobileCliente.BaseHomologacao = ClienteAcesso.URLHomologacao;
                }

                if (inserir)
                    repUsuarioMobileCliente.Inserir(usuarioMobileCliente);
                else
                    repUsuarioMobileCliente.Atualizar(usuarioMobileCliente);

                codigoMobile = usuarioMobile.Codigo;
                return "";
            }
            catch (Exception ex)
            {
                unitOfWorkAdmin.Dispose();
                throw;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        private void ConverterObjetoRetornoWS(ref ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno, dynamic retornoReceitaWS)
        {
            if (!string.IsNullOrWhiteSpace((string)retornoReceitaWS.status) && !string.IsNullOrWhiteSpace((string)retornoReceitaWS.message))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = 0;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyy");
                retorno.Mensagem = (string)retornoReceitaWS.message;
                retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
                retorno.Objeto.ConsultaValida = false;
                retorno.Objeto.MensagemReceita = (string)retornoReceitaWS.message;
            }
            else
            {
                retorno.Status = true;
                retorno.CodigoMensagem = 1;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyy");
                retorno.Mensagem = Localization.Resources.Gerais.Geral.Sucesso;
                retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
                retorno.Objeto.ConsultaValida = true;
                retorno.Objeto.MensagemReceita = Localization.Resources.Gerais.Geral.Sucesso;
                retorno.Objeto.Pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                retorno.Objeto.Pessoa.AtualizarEnderecoPessoa = true;
                retorno.Objeto.Pessoa.ClienteExterior = false;

                retorno.Objeto.Pessoa.CNAE = "";
                if (retornoReceitaWS.atividade_principal.Count > 0)
                    retorno.Objeto.Pessoa.CNAE = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.atividade_principal[0].code);

                retorno.Objeto.Pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                retorno.Objeto.Pessoa.Endereco.Bairro = ((string)retornoReceitaWS.bairro).ToUpper();
                retorno.Objeto.Pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.cep);
                retorno.Objeto.Pessoa.Endereco.CEPSemFormato = (string)retornoReceitaWS.cep;

                retorno.Objeto.Pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                retorno.Objeto.Pessoa.Endereco.Cidade.Descricao = ((string)retornoReceitaWS.municipio).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF = ((string)retornoReceitaWS.uf).ToUpper();

                retorno.Objeto.Pessoa.Endereco.Complemento = ((string)retornoReceitaWS.complemento).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Logradouro = ((string)retornoReceitaWS.logradouro).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Numero = ((string)retornoReceitaWS.numero).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Telefone = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.telefone);

                retorno.Objeto.Pessoa.NomeFantasia = ((string)retornoReceitaWS.fantasia).ToUpper();
                retorno.Objeto.Pessoa.RazaoSocial = ((string)retornoReceitaWS.nome).ToUpper();
                retorno.Objeto.Pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                retorno.Objeto.Pessoa.RGIE = "";
            }

        }

        private void SalvarCodigosComercialDistribuidor(Dominio.Entidades.Empresa empresa, Repositorio.Empresa repositorioEmpresa)
        {
            empresa.CodigosComercialDistribuidor?.Clear();

            dynamic codigosComercialDistribuidor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CodigosComercialDistribuidor"));

            if (codigosComercialDistribuidor.Count > 0)
            {
                empresa.CodigosComercialDistribuidor = new List<string>();

                foreach (dynamic codigoComercialDistribuidorTransportador in codigosComercialDistribuidor)
                {
                    Dominio.Entidades.Empresa empresaCodigoComercialDistribuidorDuplicado = repositorioEmpresa.BuscarPorCodigoComercialDistribuidorDuplicado(empresa.Codigo, (string)codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor);

                    if (empresaCodigoComercialDistribuidorDuplicado != null)
                        throw new ControllerException(Localization.Resources.Transportadores.Transportador.JaExisteUmTransportadorComCodigoComercialDeDistribuidor + "(string)codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor}" + Localization.Resources.Transportadores.Transportador.Cadastrado);

                    empresa.CodigosComercialDistribuidor.Add((string)codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor);
                }
            }
        }

        private void SalvarScriptPadrao(int codigoEmpresaPai, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unidadeTrabalho);
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unidadeTrabalho);
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens repGrupoImpostoItens = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens(unidadeTrabalho);
                bool transacaoAtiva = unidadeTrabalho.IsActiveTransaction();

                try
                {
                    if (!transacaoAtiva)
                        unidadeTrabalho.Start();

                    List<Dominio.Entidades.CFOP> listaCFOP = repCFOP.BuscarCFOPEmpresa(codigoEmpresaPai);

                    for (int i = 0; i < listaCFOP.Count; i++)
                    {
                        Dominio.Entidades.CFOP cfop = new Dominio.Entidades.CFOP();
                        cfop = listaCFOP[i].Clonar();
                        cfop.Empresa = empresa;
                        cfop.NaturezaDaOperacao = null;
                        cfop.NaturezasOperacoes = null;
                        repCFOP.Inserir(cfop);
                    }

                    List<Dominio.Entidades.NaturezaDaOperacao> listaNaturezaDaOperacao = repNaturezaDaOperacao.BuscarNaturezaEmpresa(codigoEmpresaPai);
                    for (int i = 0; i < listaNaturezaDaOperacao.Count; i++)
                    {
                        Dominio.Entidades.NaturezaDaOperacao naturezaClonar = listaNaturezaDaOperacao[i];
                        Dominio.Entidades.NaturezaDaOperacao natureza = new Dominio.Entidades.NaturezaDaOperacao();
                        natureza = naturezaClonar.Clonar();
                        natureza.Empresa = empresa;
                        natureza.CFOPs = null;
                        natureza.NaturezaNFSe = null;
                        natureza.Localidade = null;
                        natureza.TipoMovimento = null;
                        natureza.TipoMovimentoReversaoEntrada = null;
                        natureza.TipoMovimentoUsoEntrada = null;
                        natureza.CFOP = naturezaClonar.CFOP != null ? repCFOP.BuscarPorCFOPEmpresa(naturezaClonar.CFOP.CodigoCFOP, empresa.Codigo) : null;
                        repNaturezaDaOperacao.Inserir(natureza);
                    }

                    List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> listaPlanoConta = repPlanoConta.BuscarPlanoEmpresa(codigoEmpresaPai);
                    for (int i = 0; i < listaPlanoConta.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.PlanoConta plano = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta();
                        plano = listaPlanoConta[i].Clonar();
                        plano.Empresa = empresa;
                        repPlanoConta.Inserir(plano);
                    }

                    List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> listaTipoMovimento = repTipoMovimento.BuscarTipoMovimentoEmpresa(codigoEmpresaPai);
                    for (int i = 0; i < listaTipoMovimento.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimento();
                        tipoMovimento = listaTipoMovimento[i].Clonar();
                        tipoMovimento.Empresa = empresa;
                        tipoMovimento.CentrosResultados = null;
                        tipoMovimento.ContasExportacao = null;
                        tipoMovimento.PlanoDeContaCredito = repPlanoConta.BuscarPorPlanoEmpresa(listaTipoMovimento[i].PlanoDeContaCredito.Plano, empresa.Codigo);
                        tipoMovimento.PlanoDeContaDebito = repPlanoConta.BuscarPorPlanoEmpresa(listaTipoMovimento[i].PlanoDeContaDebito.Plano, empresa.Codigo);

                        if (!transacaoAtiva && tipoMovimento.PlanoDeContaCredito != null && tipoMovimento.PlanoDeContaDebito != null)
                            repTipoMovimento.Inserir(tipoMovimento);
                    }

                    Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);
                    Dominio.Entidades.Empresa empresaFilho = repEmpresa.BuscarPorCodigo(empresa.Codigo);
                    empresaFilho.CaminhoLogoSistema = empresaPai.CaminhoLogoSistema;
                    repEmpresa.Atualizar(empresaFilho);

                    if (!transacaoAtiva)
                        unidadeTrabalho.CommitChanges();
                }
                catch
                {
                    if (!transacaoAtiva)
                        unidadeTrabalho.Rollback();

                    throw;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception(Localization.Resources.Transportadores.Transportador.RegistroPossuiDependenciasNaoPodeSerExcluido, ex);
                    }
                }
                throw;
            }
        }

        private void SalvarUsuarioPadrao(Dominio.Entidades.Empresa empresa, int codigoMobile, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.PaginaUsuario repPermissao = new Repositorio.PaginaUsuario(unidadeTrabalho);
            Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unidadeTrabalho);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLogin(empresa.CNPJ);
            if (usuarioAux == null)
            {
                Repositorio.Setor repSetor = new Repositorio.Setor(unidadeTrabalho);
                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();
                usuario.Setor = repSetor.BuscarPorCodigo(1);
                usuario.CPF = empresa.CNPJ;
                usuario.Email = "";
                usuario.Empresa = empresa;
                usuario.Endereco = empresa.Endereco;
                usuario.Localidade = empresa.Localidade;
                usuario.Login = empresa.CNPJ;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    usuario.Senha = empresa.CNPJ.Substring(0, 5);
                else
                    usuario.Senha = "multi@2015";
                usuario.Status = "A";
                usuario.Tipo = "U";
                usuario.Telefone = empresa.Telefone;
                usuario.Nome = empresa.RazaoSocial;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    usuario.UsuarioAdministrador = true;

                if (empresa.EmpresaPai != null)
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                else if (empresa.EmpresaAdministradora != null)
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Admin;

                repUsuario.Inserir(usuario);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    if (empresa.StatusEmail == "A" && empresa.Email != "")
                        this.EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, unidadeTrabalho);
                }
                else
                {
                    List<Dominio.Entidades.PermissaoEmpresa> listaPermissaoEmpresa = repPermissaoEmpresa.BuscarPorEmpresa(usuario.Empresa.Codigo);

                    foreach (Dominio.Entidades.PermissaoEmpresa permissaoEmpresa in listaPermissaoEmpresa)
                    {
                        Dominio.Entidades.PaginaUsuario permissao = new Dominio.Entidades.PaginaUsuario();
                        permissao.Usuario = usuario;
                        permissao.Pagina = permissaoEmpresa.Pagina;
                        permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                        permissao.PermissaoDeAlteracao = permissaoEmpresa.PermissaoDeAlteracao;
                        permissao.PermissaoDeDelecao = permissaoEmpresa.PermissaoDeDelecao;
                        permissao.PermissaoDeInclusao = permissaoEmpresa.PermissaoDeInclusao;
                        repPermissao.Inserir(permissao);
                    }
                }
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && empresa.Tipo != "E")
                SalvarUsuarioEmpresa(empresa, codigoMobile, configuracaoTransportador, unidadeTrabalho);
        }

        private void SalvarUsuarioEmpresa(Dominio.Entidades.Empresa empresa, int codigoMobile, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Setor repSetor = new Repositorio.Setor(unidadeTrabalho);

            if (configuracaoTransportador.NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador)
                return;

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(empresa.Codigo, "11111111111", "U");

            if (usuario == null)
            {
                usuario = new Dominio.Entidades.Usuario();
                usuario.Setor = repSetor.BuscarPorCodigo(1);
                usuario.CPF = "11111111111";
                usuario.Empresa = empresa;
                usuario.Endereco = empresa.Endereco;
                usuario.Localidade = empresa.Localidade;
                if (ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
                {
                    usuario.Login = empresa.CNPJ.Substring(0, 5) + "-" + empresa.Localidade.Estado.Sigla;
                    usuario.Senha = empresa.CNPJ.Substring(0, 5);
                }
                else
                {
                    usuario.Login = empresa.CodigoIntegracao + "-" + empresa.Localidade.Estado.Sigla;
                    usuario.Senha = empresa.CodigoIntegracao;
                }

                usuario.Status = "A";
                usuario.Tipo = "U";
                usuario.Telefone = empresa.Telefone;
                usuario.Nome = empresa.RazaoSocial;
                usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                usuario.Email = empresa.Email;
                usuario.AlterarSenhaAcesso = true;
                usuario.UsuarioAdministrador = true;
                usuario.CodigoMobile = codigoMobile;

                repUsuario.Inserir(usuario);

                //this.SalvarPermissoesAdminNovo(usuario, unidadeTrabalho);

                if (empresa.StatusEmail == "A" && empresa.Email != "")
                    this.EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, unidadeTrabalho);
            }
            else
            {
                usuario.CodigoMobile = codigoMobile;
                usuario.UsuarioAdministrador = true;
                repUsuario.Atualizar(usuario);
            }
        }

        private void SalvarPermissoesAdminNovo(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unidadeTrabalho);
            Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada repFuncionarioFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada(unidadeTrabalho);
            Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unidadeTrabalho);

            // Adiciona os modulos
            usuario.ModulosLiberados = new List<int>();
            foreach (int codigoModulo in usuario.Empresa.ModulosLiberados)
                usuario.ModulosLiberados.Add(codigoModulo);

            // Adiciona os formularios
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> formularios = repTransportadorFormulario.BuscarPorEmpresa(usuario.Empresa.Codigo);

            foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario formulario in formularios)
            {
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario();

                funcionarioFormulario.Usuario = usuario;
                funcionarioFormulario.SomenteLeitura = formulario.SomenteLeitura;
                funcionarioFormulario.CodigoFormulario = formulario.CodigoFormulario;

                repFuncionarioFormulario.Inserir(funcionarioFormulario);

                // Atualiza Permissões personalizadas
                if (formulario.TransportadorFormularioPermissaoesPersonalizadas != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada permissao in formulario.TransportadorFormularioPermissaoesPersonalizadas)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada funcionarioFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada();
                        funcionarioFormularioPermissaoPersonalizada.CodigoPermissao = permissao.CodigoPermissao;
                        funcionarioFormularioPermissaoPersonalizada.FuncionarioFormulario = funcionarioFormulario;
                        repFuncionarioFormularioPermissaoPersonalizada.Inserir(funcionarioFormularioPermissaoPersonalizada);
                    }
                }
            }
        }

        private void SalvarSeriePadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);
            if (empresa.EmpresaPai.Configuracao != null)
            {
                Dominio.Entidades.EmpresaSerie empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.NFe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);

                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.NFSe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);

                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);

                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);
            }
            else
            {
                Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                serie.Empresa = empresa;
                serie.Numero = 1;
                serie.Status = "A";
                serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                repEmpresaSerie.Inserir(serie);
            }
        }

        private void AdicionarSeriePadraoConfiguracao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);

            if (ConfiguracaoEmbarcador.NumeroSerieNotaDebitoPadrao > 0)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorAbreviacao("ND");
                if (modelo != null)
                {
                    Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                    serie.Empresa = empresa;
                    serie.Numero = ConfiguracaoEmbarcador.NumeroSerieNotaDebitoPadrao;
                    serie.Status = "A";
                    serie.Tipo = Dominio.Enumeradores.TipoSerie.OutrosDocumentos;
                    repEmpresaSerie.Inserir(serie);

                    modelo.Series.Add(serie);
                    repModeloDocumentoFiscal.Atualizar(modelo);
                }
            }

            if (ConfiguracaoEmbarcador.NumeroSerieNotaCreditoPadrao > 0)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorAbreviacao("NC");
                if (modelo != null)
                {
                    Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                    serie.Empresa = empresa;
                    serie.Numero = ConfiguracaoEmbarcador.NumeroSerieNotaCreditoPadrao;
                    serie.Status = "A";
                    serie.Tipo = Dominio.Enumeradores.TipoSerie.OutrosDocumentos;
                    repEmpresaSerie.Inserir(serie);

                    modelo.Series.Add(serie);
                    repModeloDocumentoFiscal.Atualizar(modelo);
                }
            }
        }

        private void EnviarEmailDeNotificacaoDeUsuarioCadastrado(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                sb.Append("<p>" + Localization.Resources.Transportadores.Transportador.SeusDadosParaAcessoAoSistemaMultiNFeSao + "<br /><br />");
                sb.Append(Localization.Resources.Transportadores.Transportador.Usuario + ": ").Append(usuario.Login).Append("<br />");
                sb.Append(Localization.Resources.Transportadores.Transportador.Senha + ": ").Append(usuario.Senha).Append("</p><br />");

                if (usuario.Empresa.EmpresaPai != null && !string.IsNullOrWhiteSpace(usuario.Empresa.EmpresaPai.URLSistema))
                    sb.Append(Localization.Resources.Transportadores.Transportador.ParaUtilizarSistemaMultiNFeAcesse).Append(usuario.Empresa.EmpresaPai.URLSistema);
                else
                    sb.Append(Localization.Resources.Transportadores.Transportador.ParaUtilizarSistemaMultiNFeAcesseHttpWwwCommerceInfBrUtilizeOpcaoDeLogin);

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append(Localization.Resources.Transportadores.Transportador.CommerceSistemasHttpWwwCommerceInfBr + "< br />");
                ss.Append(Localization.Resources.Transportadores.Transportador.FoneFax4930259500 + "<br />");
                ss.Append(Localization.Resources.Transportadores.Transportador.EmailSuporteCommerceInfBr);

                svcEmail.EnviarEmail("nfe@commerce.inf.br", "nfe@commerce.inf.br", "cesaoexp18", usuario.Empresa.Email.Split(',')[0], "", "", Localization.Resources.Transportadores.Transportador.MultiNFeDadosParaAcessoApSistema, sb.ToString(), "smtp.commerce.inf.br", null, ss.ToString(), false, "suporte@commerce.inf.br", 0, unitOfWork, usuario.Empresa.Codigo);
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                try
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipoProducao(Cliente.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);

                    if (clienteURLAcesso != null)
                    {
                        sb.Append("<p>" + Localization.Resources.Transportadores.Transportador.SeusDadosParaAcessoAoSistemaMultiCTeSao + "<br /><br />");
                        sb.Append(Localization.Resources.Transportadores.Transportador.Usuario + ": ").Append(usuario.Login).Append("<br />");

                        Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                        bool exigirTrocaSenhaPrimeiroAcesso = politicaSenha?.ExigirTrocaSenhaPrimeiroAcesso ?? false;

                        if (exigirTrocaSenhaPrimeiroAcesso && usuario.AlterarSenhaAcesso)
                        {
                            if (!string.IsNullOrWhiteSpace(usuario.Senha))
                            {
                                sb.Append(Localization.Resources.Transportadores.Transportador.Senha + ": ")
                                  .Append(usuario.Senha)
                                  .Append("</p><br />");
                            }
                        }
                        else
                        {
                            if (!usuario.SenhaCriptografada)
                            {
                                sb.Append("Senha: ")
                                  .Append(usuario.Senha)
                                  .Append("</p><br />");
                            }
                        }

                        sb.Append(Localization.Resources.Transportadores.Transportador.ParaUtilizarSistemaMultiCTeAcesse).Append("https://" + clienteURLAcesso.URLAcesso);

                        System.Text.StringBuilder ss = new System.Text.StringBuilder();
                        ss.Append("MultiSoftware - http://www.multisoftware.com.br/ <br />");
                        ss.Append("Embarcador - " + Cliente.RazaoSocial + "<br />");
                        ss.Append(Localization.Resources.Transportadores.Transportador.FoneFax + "(49)3025-9500 <br />");
                        ss.Append(Localization.Resources.Transportadores.Transportador.Celular + "(49)9999-8880(TIM) <br />");
                        ss.Append(Localization.Resources.Transportadores.Transportador.Email + "cte@multisoftware.com.br");

                        svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, usuario.Empresa.Email.Split(',')[0], "", "", Localization.Resources.Transportadores.Transportador.MultiNFeDadosParaAcessoApSistema, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    unitOfWorkAdmin.Dispose();
                }
            }
        }

        private void SalvarDadoBancario(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Banco repBanco = new Repositorio.Banco(unidadeDeTrabalho);

            dynamic dynDadosBancarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DadoBancario"));

            if (dynDadosBancarios != null)
            {
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Banco) && (string)dynDadosBancarios.Banco != "0")
                    empresa.Banco = new Dominio.Entidades.Banco() { Codigo = int.Parse((string)dynDadosBancarios.Banco) };
                else
                    empresa.Banco = null;

                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Agencia))
                    empresa.Agencia = (string)dynDadosBancarios.Agencia;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Digito))
                    empresa.DigitoAgencia = (string)dynDadosBancarios.Digito;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.NumeroConta))
                    empresa.NumeroConta = (string)dynDadosBancarios.NumeroConta;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.CnpjIpef))
                    empresa.CnpjIpef = (string)dynDadosBancarios.CnpjIpef;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.TipoConta) && (string)dynDadosBancarios.TipoConta != "0")
                    empresa.TipoContaBanco = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco)int.Parse((string)dynDadosBancarios.TipoConta);
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.TipoChavePIX) && (string)dynDadosBancarios.TipoChavePIX != "0")
                    empresa.TipoChavePIX = (Dominio.ObjetosDeValor.Enumerador.TipoChavePix)int.Parse((string)dynDadosBancarios.TipoChavePIX);
                empresa.ChavePIX = new[]
                {
                    (string)dynDadosBancarios.ChavePIXCPFCNPJ,
                    (string)dynDadosBancarios.ChavePIXEmail,
                    (string)dynDadosBancarios.ChavePIXCelular,
                    (string)dynDadosBancarios.ChavePIXAleatoria
                }
                .FirstOrDefault(chave => !string.IsNullOrWhiteSpace(chave) && chave != "0");
                int codigoEmpresaFavorecida = ((string)dynDadosBancarios.EmpresaFavorecida).ToInt();

                if (codigoEmpresaFavorecida > 0)
                    empresa.EmpresaFavorecida = repEmpresa.BuscarPorCodigo(codigoEmpresaFavorecida);
                else
                    empresa.EmpresaFavorecida = null;

                bool.TryParse((string)dynDadosBancarios.AntecipacaoPagamento, out bool antecipacaoPagamento);
                empresa.AntecipacaoPagamento = antecipacaoPagamento;
            }
        }

        private void SalvarTermoQuitacao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            dynamic termoQuitacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TermoQuitacao"));

            if (termoQuitacao != null)
            {
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.GerarAvisoPeriodico))
                    empresa.GerarAvisoPeriodico = (bool)termoQuitacao.GerarAvisoPeriodico;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.ACadaAvisoPeriodico))
                    empresa.ACadaAvisoPeriodico = (int)termoQuitacao.ACadaAvisoPeriodico;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.ACadaTipoTermo))
                    empresa.ACadaTipoTermo = (int)termoQuitacao.ACadaTipoTermo;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.PeriodoAvisoPeriodico))
                    empresa.PeriodoAvisoPeriodico = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno)termoQuitacao.PeriodoAvisoPeriodico;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.PeriodoTipoTermo))
                    empresa.PeriodoTipoTermo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno)termoQuitacao.PeriodoTipoTermo;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.TipoGeracaoTermo))
                    empresa.TipoGeracaoTermo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTermo)termoQuitacao.TipoGeracaoTermo;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.TipoTermo))
                    empresa.TipoTermo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTermo)termoQuitacao.TipoTermo;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.TempoAguardarParaGerarTermo))
                    empresa.TempoAguardarParaGerarTermo = (int)termoQuitacao.TempoAguardarParaGerarTermo;
                if (!string.IsNullOrWhiteSpace((string)termoQuitacao.DataFimTermoQuitacaoInicial))
                {
                    DateTime.TryParse((string)termoQuitacao.DataFimTermoQuitacaoInicial, out DateTime data);
                    empresa.DataFimTermoQuitacaoInicial = data;
                }
            }
        }

        private void SalvarComponentesCTeImportado(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado repositorio = new Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> listaComponenteCTeImportadoExcluir = repositorio.BuscarPorTransportador(empresa.Codigo);

            foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado componenteCTeImportado in listaComponenteCTeImportadoExcluir)
                repositorio.Deletar(componenteCTeImportado);

            dynamic dynComponentesCTesImportados = JsonConvert.DeserializeObject<dynamic>(Request.Params("ComponentesCTesImportados"));

            List<int> codigosComponenteFrete = ((IEnumerable<dynamic>)dynComponentesCTesImportados).Select(obj => ((string)obj.CodigoComponenteFrete).ToInt()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentesFrete = codigosComponenteFrete.Count > 0 ? repositorioComponenteFrete.BuscarPorCodigos(codigosComponenteFrete) : new List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            foreach (dynamic dynComponenteCTeImportado in dynComponentesCTesImportados)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado componenteCTeImportado = new Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado()
                {
                    Empresa = empresa,
                    Descricao = ((string)dynComponenteCTeImportado.Descricao).ToString(),
                    ComponenteFrete = (from obj in componentesFrete where obj.Codigo == ((string)dynComponenteCTeImportado.CodigoComponenteFrete).ToInt() select obj).FirstOrDefault()
                };

                repositorio.Inserir(componenteCTeImportado);
            }
        }

        private void SalvarLeituraFTP(Dominio.Entidades.Empresa empresa, IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados, dynamic configuracoes)
        {
            Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados integracaoFTP = repIntegracaoFTPDocumentosDestinados.BuscarPorEmpresa(empresa.Codigo);

            if (integracaoFTP == null)
            {
                ValidarLeituraFTP(configuracoes);
                integracaoFTP = new Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados();
            }

            integracaoFTP.Empresa = empresa;
            integracaoFTP.EnderecoFTP = (string)configuracoes.EnderecoFTP;
            integracaoFTP.Usuario = (string)configuracoes.Usuario;
            integracaoFTP.Senha = (string)configuracoes.Senha;
            integracaoFTP.Porta = (string)configuracoes.Porta;
            integracaoFTP.DiretorioInput = (string)configuracoes.DiretorioInput;
            integracaoFTP.DiretorioOutput = (string)configuracoes.DiretorioOutput;
            integracaoFTP.DiretorioXML = (string)configuracoes.DiretorioXML;

            if (!string.IsNullOrWhiteSpace((string)configuracoes.Passivo))
                integracaoFTP.Passivo = (bool)configuracoes.Passivo;
            else
                integracaoFTP.Passivo = false;

            if (!string.IsNullOrWhiteSpace((string)configuracoes.UtilizarSFTP))
                integracaoFTP.UtilizarSFTP = (bool)configuracoes.UtilizarSFTP;
            else
                integracaoFTP.UtilizarSFTP = true;
            if (!string.IsNullOrWhiteSpace((string)configuracoes.SSL))
                integracaoFTP.SSL = (bool)configuracoes.SSL;
            else
                integracaoFTP.SSL = false;
            if (integracaoFTP.Codigo > 0)
                repIntegracaoFTPDocumentosDestinados.Atualizar(integracaoFTP);
            else
                repIntegracaoFTPDocumentosDestinados.Inserir(integracaoFTP);
        }

        private void ValidarLeituraFTP(dynamic configuracoes)
        {
            List<string> camposFaltantes = new List<string>();

            if (string.IsNullOrWhiteSpace((string)configuracoes.EnderecoFTP))
                camposFaltantes.Add("Endereço FTP");

            if (string.IsNullOrWhiteSpace((string)configuracoes.Porta))
                camposFaltantes.Add("Porta");

            if (string.IsNullOrWhiteSpace((string)configuracoes.Usuario))
                camposFaltantes.Add("Usuário");

            if (string.IsNullOrWhiteSpace((string)configuracoes.Senha))
                camposFaltantes.Add("Senha");

            if (camposFaltantes.Count != 4)
                throw new ControllerException($"Os seguintes campos estão faltando: {string.Join(", ", camposFaltantes)} em Configurações > Leitura FTP. Por favor, preencha-os.");
        }

        private void SalvarConfiguracoes(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.EstadosDeEmissaoSerie repEstadosDeEmissaoSerie = new Repositorio.EstadosDeEmissaoSerie(unidadeDeTrabalho);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.IntegracaoMigrateRegimeTributario repIntegracaoMigrateRegimeTributario = new Repositorio.Embarcador.Integracao.IntegracaoMigrateRegimeTributario(unidadeDeTrabalho);

            dynamic configuracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Configuracao"));

            bool novaConfiguracao = false;

            if (empresa.Configuracao == null)
            {
                novaConfiguracao = true;
                empresa.Configuracao = new Dominio.Entidades.ConfiguracaoEmpresa();
            }
            else
                empresa.Configuracao.Initialize();

            empresa.Configuracao.CSTPISCOFINS = (string)configuracoes.CSTPISCOFINS;
            empresa.Configuracao.AliquotaPIS = Utilidades.Decimal.Converter((string)configuracoes.AliquotaPIS);
            empresa.Configuracao.AliquotaCOFINS = Utilidades.Decimal.Converter((string)configuracoes.AliquotaCOFINS);

            List<Dominio.Entidades.EmpresaSerie> empresaSeries = repSerie.BuscarTodosPorEmpresa(empresa.Codigo);

            if (empresa.Configuracao.SerieInterestadual == null)
                empresa.Configuracao.SerieInterestadual = (from obj in empresaSeries where obj.Tipo == Dominio.Enumeradores.TipoSerie.CTe && obj.Status == "A" select obj).FirstOrDefault();

            if (empresa.Configuracao.SerieIntraestadual == null)
                empresa.Configuracao.SerieIntraestadual = (from obj in empresaSeries where obj.Tipo == Dominio.Enumeradores.TipoSerie.CTe && obj.Status == "A" select obj).FirstOrDefault();

            if (empresa.Configuracao.SerieMDFe == null)
                empresa.Configuracao.SerieMDFe = (from obj in empresaSeries where obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe && obj.Status == "A" select obj).FirstOrDefault();

            if (empresa.Configuracao.SerieNFSe == null)
                empresa.Configuracao.SerieNFSe = (from obj in empresaSeries where obj.Tipo == Dominio.Enumeradores.TipoSerie.NFSe && obj.Status == "A" select obj).FirstOrDefault();

            if (novaConfiguracao && empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null)
            {
                empresa.Configuracao.CodigoSeguroATM = empresa.EmpresaPai.Configuracao.CodigoSeguroATM;
                empresa.Configuracao.SenhaSeguroATM = empresa.EmpresaPai.Configuracao.SenhaSeguroATM;
                empresa.Configuracao.UsuarioSeguroATM = empresa.EmpresaPai.Configuracao.UsuarioSeguroATM;
                empresa.Configuracao.AverbaAutomaticoATM = empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM;

                empresa.Configuracao.TipoImpressao = empresa.EmpresaPai.Configuracao.TipoImpressao;
                empresa.Configuracao.DiasParaEmissaoDeCTeComplementar = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeComplementar;
                empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeSubstituicao;
                empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeAnulacao;
                empresa.Configuracao.DiasParaEntrega = empresa.EmpresaPai.Configuracao.DiasParaEntrega;

                if (string.IsNullOrWhiteSpace(empresa.Configuracao.ProdutoPredominante))
                    empresa.Configuracao.ProdutoPredominante = empresa.EmpresaPai.Configuracao.ProdutoPredominante;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if ((bool)configuracoes.PrincipalFilialEmissoraTMS)
                {
                    Dominio.Entidades.Empresa empresaPrincipalEmissoraTMS = repEmpresa.BuscarPrincipalEmissoraTMS();

                    if (empresaPrincipalEmissoraTMS != null)
                    {
                        empresaPrincipalEmissoraTMS.Configuracao.PrincipalFilialEmissoraTMS = false;
                        repEmpresa.Atualizar(empresaPrincipalEmissoraTMS);
                    }

                    empresa.Configuracao.PrincipalFilialEmissoraTMS = true;
                }
                else
                    empresa.Configuracao.PrincipalFilialEmissoraTMS = false;

                bool empresaPadraoLancamentoGuarita = ((string)configuracoes.EmpresaPadraoLancamentoGuarita).ToBool();
                if (empresaPadraoLancamentoGuarita)
                {
                    Dominio.Entidades.Empresa empresaPadrao = repEmpresa.BuscarEmpresaPadraoLancamentoGuarita();
                    if (empresaPadrao != null)
                    {
                        empresaPadrao.Configuracao.EmpresaPadraoLancamentoGuarita = false;
                        repEmpresa.Atualizar(empresaPadrao);
                    }
                }
                empresa.Configuracao.EmpresaPadraoLancamentoGuarita = empresaPadraoLancamentoGuarita;

                bool existe = false;
                List<Dominio.Entidades.EstadosDeEmissaoSerie> estadosDeEmissaoSerie = repEstadosDeEmissaoSerie.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                foreach (Dominio.Entidades.EstadosDeEmissaoSerie estadoEmissaoSerie in estadosDeEmissaoSerie)
                {
                    if (!(bool)configuracoes.EmitirEstadoOrigemForMesmoDaFilial || estadoEmissaoSerie.Estado.Sigla != empresa.Localidade.Estado.Sigla)
                        repEstadosDeEmissaoSerie.Deletar(estadoEmissaoSerie);
                    else
                        existe = true;
                }

                if ((bool)configuracoes.EmitirEstadoOrigemForMesmoDaFilial && !existe)
                {
                    Dominio.Entidades.EstadosDeEmissaoSerie estadoEmissaoSerie = new Dominio.Entidades.EstadosDeEmissaoSerie();
                    estadoEmissaoSerie.ConfiguracaoEmpresa = empresa.Configuracao;
                    estadoEmissaoSerie.Estado = empresa.Localidade.Estado;
                    repEstadosDeEmissaoSerie.Inserir(estadoEmissaoSerie);
                }
            }

            empresa.TipoEmissaoIntramunicipal = (TipoEmissaoIntramunicipal)configuracoes.TipoEmissaoIntramunicipal;

            empresa.SempreEmitirNFS = (bool)configuracoes.SempreEmitirNFS;

            if ((int)configuracoes.SerieIntraestadual > 0)
                empresa.Configuracao.SerieIntraestadual = repSerie.BuscarPorCodigo((int)configuracoes.SerieIntraestadual);

            if ((int)configuracoes.SerieInterestadual > 0)
                empresa.Configuracao.SerieInterestadual = repSerie.BuscarPorCodigo((int)configuracoes.SerieInterestadual);

            if ((int)configuracoes.SerieMDFe > 0)
                empresa.Configuracao.SerieMDFe = repSerie.BuscarPorCodigo((int)configuracoes.SerieMDFe);

            if (!string.IsNullOrWhiteSpace((string)configuracoes.CasasQuantidadeProdutoNFe))
                empresa.CasasQuantidadeProdutoNFe = (int)configuracoes.CasasQuantidadeProdutoNFe;
            else
                empresa.CasasQuantidadeProdutoNFe = 4;

            if (!string.IsNullOrWhiteSpace((string)configuracoes.CalculaIBPTNFe))
                empresa.CalculaIBPTNFe = (bool)configuracoes.CalculaIBPTNFe;
            else
                empresa.CalculaIBPTNFe = true;

            if (!string.IsNullOrWhiteSpace((string)configuracoes.CasasValorProdutoNFe))
                empresa.CasasValorProdutoNFe = (int)configuracoes.CasasValorProdutoNFe;
            else
                empresa.CasasValorProdutoNFe = 5;

            empresa.Configuracao.FraseSecretaNFSe = (string)configuracoes.FraseSecretaNFSe;
            empresa.Configuracao.SenhaNFSe = (string)configuracoes.SenhaNFSe;
            empresa.Configuracao.Perfil = ((string)configuracoes.PerfilSPEDFiscal).ToEnum<Dominio.Enumeradores.PerfilEmpresa>();
            empresa.Configuracao.NaoComprarValePedagioCargaTransbordo = ((string)configuracoes.NaoComprarValePedagioCargaTransbordo).ToBool();
            empresa.Configuracao.EnviarNovoImposto = ((string)configuracoes.EnviarNovoImposto).ToBool();
            empresa.Configuracao.ReduzirPISCOFINSBaseCalculoIBSCBS = ((string)configuracoes.ReduzirPISCOFINSBaseCalculoIBSCBS).ToBool();

            empresa.Configuracao.VersaoCTe = ((string)configuracoes.VersaoCTe);

            if (!string.IsNullOrWhiteSpace((string)configuracoes.TipoImpressaoPedidoVenda))
            {
                TipoImpressaoPedidoVenda tipoImpressaoPedidoVenda = TipoImpressaoPedidoVenda.Pedido;
                Enum.TryParse((string)configuracoes.TipoImpressaoPedidoVenda, out tipoImpressaoPedidoVenda);
                empresa.TipoImpressaoPedidoVenda = tipoImpressaoPedidoVenda;
            }
            else
                empresa.TipoImpressaoPedidoVenda = TipoImpressaoPedidoVenda.Pedido;

            if (!string.IsNullOrWhiteSpace((string)configuracoes.TipoLancamentoFinanceiroSemOrcamento))
            {
                TipoLancamentoFinanceiroSemOrcamento tipoLancamentoFinanceiroSemOrcamento = TipoLancamentoFinanceiroSemOrcamento.Liberar;
                Enum.TryParse((string)configuracoes.TipoLancamentoFinanceiroSemOrcamento, out tipoLancamentoFinanceiroSemOrcamento);
                empresa.TipoLancamentoFinanceiroSemOrcamento = tipoLancamentoFinanceiroSemOrcamento;
            }
            else
                empresa.TipoLancamentoFinanceiroSemOrcamento = TipoLancamentoFinanceiroSemOrcamento.Liberar;

            empresa.GerarParcelaAutomaticamente = bool.Parse((string)configuracoes.GerarParcelaAutomaticamente);
            empresa.EmitirVendaPrazoNFCe = bool.Parse((string)configuracoes.EmitirVendaPrazoNFCe);
            empresa.HabilitaLancamentoProdutoLote = bool.Parse((string)configuracoes.HabilitaLancamentoProdutoLote);
            empresa.SubtraiDescontoBaseICMS = bool.Parse((string)configuracoes.SubtraiDescontoBaseICMS);
            empresa.ArmazenarDanfeParaSMS = bool.Parse((string)configuracoes.ArmazenarDanfeParaSMS);
            empresa.AtivarEnvioDanfeSMS = bool.Parse((string)configuracoes.AtivarEnvioDanfeSMS);
            empresa.GerarCreditoC197SPEDFiscal = bool.Parse((string)configuracoes.GerarCreditoC197SPEDFiscal);
            empresa.HabilitarTabelaValorOrdemServicoVenda = bool.Parse((string)configuracoes.HabilitarTabelaValorOrdemServicoVenda);
            empresa.PermiteAlterarEmpresaOrdemServicoVenda = ((string)configuracoes.PermiteAlterarEmpresaOrdemServicoVenda).ToBool();
            empresa.HabilitarNumeroInternoOrdemServicoVenda = ((string)configuracoes.HabilitarNumeroInternoOrdemServicoVenda).ToBool();
            empresa.UtilizaDataVencimentoNaEmissao = bool.Parse((string)configuracoes.UtilizaDataVencimentoNaEmissao);
            empresa.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual = bool.Parse((string)configuracoes.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual);
            empresa.HabilitarEtiquetaProdutosNFe = bool.Parse((string)configuracoes.HabilitarEtiquetaProdutosNFe);
            empresa.PermitirImportarApenasPedidoVendaFinalizado = ((string)configuracoes.PermitirImportarApenasPedidoVendaFinalizado).ToBool();
            empresa.CadastrarProdutoAutomaticamenteDocumentoEntrada = ((string)configuracoes.CadastrarProdutoAutomaticamenteDocumentoEntrada).ToBool();
            empresa.DeixarPadraoFinalizadoDocumentoEntrada = ((string)configuracoes.DeixarPadraoFinalizadoDocumentoEntrada).ToBool();
            empresa.ControlarEstoqueNegativo = ((string)configuracoes.ControlarEstoqueNegativo).ToBool();
            empresa.VisualizarSomenteClientesAssociados = ((string)configuracoes.VisualizarSomenteClientesAssociados).ToBool();

            int.TryParse((string)configuracoes.TipoPagamentoRecebimento, out int tipoPagamentoRecebimento);
            int.TryParse((string)configuracoes.NaturezaDaOperacaoNFCe, out int naturezaDaOperacaoNFCe);
            int.TryParse((string)configuracoes.TipoMovimento, out int tipoMovimento);
            int codigoPagamentoMotoristaTipo = ((string)configuracoes.PagamentoMotoristaTipo).ToInt();

            empresa.NaturezaDaOperacaoNFCe = naturezaDaOperacaoNFCe > 0 ? repNaturezaDaOperacao.BuscarPorId(naturezaDaOperacaoNFCe) : null;
            empresa.TipoPagamentoRecebimento = tipoPagamentoRecebimento > 0 ? repTipoPagamentoRecebimento.BuscarPorCodigo(tipoPagamentoRecebimento) : null;
            empresa.TipoMovimento = tipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(tipoMovimento) : null;
            empresa.PagamentoMotoristaTipo = codigoPagamentoMotoristaTipo > 0 ? repPagamentoMotoristaTipo.BuscarPorCodigo(codigoPagamentoMotoristaTipo) : null;
            empresa.UtilizaTransportadoraPadraoContratacao = ((string)configuracoes.TransportadoraPadraoContratacao).ToBool();

            if (!string.IsNullOrWhiteSpace((string)configuracoes.UtilizaIntegracaoDocumentosDestinado))
                empresa.UtilizaIntegracaoDocumentosDestinado = (bool)configuracoes.UtilizaIntegracaoDocumentosDestinado;
            else
                empresa.UtilizaIntegracaoDocumentosDestinado = false;

            empresa.ObservacaoSimplesNacional = (string)configuracoes.ObservacaoSimplesNacional;
            empresa.ObservacaoCTe = (string)configuracoes.ObservacaoCTe;
            empresa.TokenSMS = (string)configuracoes.TokenSMS;
            empresa.NumeroCertificadoIdoneidade = (string)configuracoes.NumeroCertificadoIdoneidade;
            empresa.QuantidadeMaximaEmailRPS = ((string)configuracoes.QuantidadeMaximaEmailRPS).ToInt();

            empresa.AliquotaICMSNegociado = Request.GetDecimalParam("AliquotaICMSNegociado");

            empresa.CodigoServicoCorreios = (string)configuracoes.CodigoServicoCorreios;

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");
            bool permiteBloquearTransportador = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Transportador_PermiteBloquearTransportador) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            if (permiteBloquearTransportador)
            {
                empresa.BloquearTransportador = Request.GetBoolParam("BloquearTransportador");
                empresa.MotivoBloqueio = empresa.BloquearTransportador ? Request.GetStringParam("MotivoBloqueio") : "";
            }

            empresa.TransportadorFerroviario = Request.GetBoolParam("TransportadorFerroviario");
            empresa.PermitirUtilizarCadastroAgendamentoColeta = Request.GetBoolParam("PermitirUtilizarCadastroAgendamentoColeta");
            empresa.NotificarDestinatarioAgendamentoColeta = Request.GetBoolParam("NotificarDestinatarioAgendamentoColeta");

            empresa.CNPJContabilidade = ((string)configuracoes.CNPJContabilidade).ObterSomenteNumeros();
            empresa.CPFContabilidade = ((string)configuracoes.CPFContabilidade).ObterSomenteNumeros();
            if (!string.IsNullOrWhiteSpace(empresa.CNPJContabilidade) && !Utilidades.Validate.ValidarCNPJ(empresa.CNPJContabilidade))
                throw new ControllerException(Localization.Resources.Transportadores.Transportador.CNPJContabilidadeInformadoInvalido);
            if (!string.IsNullOrWhiteSpace(empresa.CPFContabilidade) && !Utilidades.Validate.ValidarCPF(empresa.CPFContabilidade))
                throw new ControllerException(Localization.Resources.Transportadores.Transportador.CPFContabilidadeInformadoInvalido);

            TipoIntegracao? tipoIntegracaoCarga = ((string)configuracoes.TipoIntegracao).ToNullableEnum<TipoIntegracao>();
            empresa.TipoIntegracaoCarga = tipoIntegracaoCarga.HasValue ? repTipoIntegracao.BuscarPorTipo(tipoIntegracaoCarga.Value) : null;

            empresa.NaturezaDaOperacaoNFCe = naturezaDaOperacaoNFCe > 0 ? repNaturezaDaOperacao.BuscarPorId(naturezaDaOperacaoNFCe) : null;

            empresa.FormaDeducaoValePedagio = ((string)configuracoes.FormaDeducaoValePedagio).ToNullableEnum<FormaDeducaoValePedagio>();

            empresa.RestringirLocaisCarregamentoAutorizadosMotoristas = ((string)configuracoes.RestringirLocaisCarregamentoAutorizadosMotoristas).ToBool();

            if (empresa.UtilizaIntegracaoDocumentosDestinado)
            {
                SalvarLeituraFTP(empresa, repIntegracaoFTPDocumentosDestinados, configuracoes);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                dynamic leituraFTPParams = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LeituraFTP"));
                SalvarLeituraFTP(empresa, repIntegracaoFTPDocumentosDestinados, leituraFTPParams);
            }

            /**
             * CIOT
             */
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if ((bool)configuracoes.HabilitarCIOT)
                {
                    empresa.Configuracao.TipoIntegradoraCIOT = (Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT)configuracoes.TipoIntegracaoCIOT;
                    empresa.Configuracao.EncerrarCIOTPorViagem = (bool)configuracoes.EncerrarCIOTPorViagem;
                    empresa.Configuracao.CodigoIntegradorEFrete = (string)configuracoes.CodigoIntegracaoEfrete;
                }
                else
                {
                    empresa.Configuracao.TipoIntegradoraCIOT = null;
                }

                empresa.FormaRateioSVM = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM)configuracoes.FormaRateioSVM;
                empresa.SVMMesmoQueMultimodal = (bool)configuracoes.SVMMesmoQueMultimodal;
                empresa.SVMTerminaisPortuarioOrigemDestino = (bool)configuracoes.SVMTerminaisPortuarioOrigemDestino;
                empresa.SVMBUSPortoOrigemDestino = (bool)configuracoes.SVMBUSPortoOrigemDestino;
            }

            empresa.Configuracao.ObrigatoriedadeCIOTEmissaoMDFe = (bool)configuracoes.ObrigatoriedadeCIOTEmissaoMDFe;

            #region Configurações Repom

            dynamic repom = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Repom"));

            empresa.Configuracao.CodigoFilialRepom = (string)repom.CodigoFilialRepom;

            #endregion

            #region Configurações Electrolux

            dynamic electrolux = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Electrolux"));

            empresa.Configuracao.IdentificadorTransportadorElectrolux = (string)electrolux.IdentificadorTransportador;

            #endregion

            #region Configurações Migrate

            dynamic migrate = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Migrate"));

            int codigoIntegracaoMigrateRegimeTributario = (int)migrate.IntegracaoMigrateRegimeTributario;

            empresa.Configuracao.PossuiIntegracaoMigrate = (bool)migrate.PossuiIntegracaoMigrate;
            empresa.Configuracao.TokenMigrate = (string)migrate.TokenMigrate;
            empresa.Configuracao.IntegracaoMigrateRegimeTributario = codigoIntegracaoMigrateRegimeTributario > 0 ? repIntegracaoMigrateRegimeTributario.BuscarPorCodigo(codigoIntegracaoMigrateRegimeTributario, false) : null;
            empresa.Configuracao.EnviarObservacaoNaDiscriminacaoServicoMigrate = (bool)migrate.EnviarObservacaoNaDiscriminacaoServicoMigrate;


            #endregion

            empresa.SetExternalChanges(empresa.Configuracao.GetCurrentChanges());
        }

        private void SalvarConfiguracoesPorTipoOperacao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Embarcador.Transportadores.ConfiguracaoTipoOperacao repConfiguracaoTipoOperacao = new Repositorio.Embarcador.Transportadores.ConfiguracaoTipoOperacao(unidadeDeTrabalho);

            dynamic configuracaoTipoOperacaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoTipoOperacaos"));

            List<int> codigosTipoOperacao = repConfiguracaoTipoOperacao.BuscarCodigosTipoOperacaoPorEmpresa(empresa.Codigo);

            if (configuracaoTipoOperacaos.Count > 0)
            {
                foreach (dynamic dynConfiguracaoTipoOperacao in configuracaoTipoOperacaos)
                {
                    Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao configuracaoTipoOperacao = repConfiguracaoTipoOperacao.BuscarPorCodigo((int)dynConfiguracaoTipoOperacao.Codigo, false);

                    bool novo = false;
                    if (configuracaoTipoOperacao == null)
                    {
                        configuracaoTipoOperacao = new Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao();
                        novo = true;
                    }
                    else
                    {
                        configuracaoTipoOperacao.Initialize();
                        codigosTipoOperacao.Remove((int)dynConfiguracaoTipoOperacao.Codigo);
                    }

                    configuracaoTipoOperacao.TipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = (int)dynConfiguracaoTipoOperacao.TipoOperacao.Codigo };
                    configuracaoTipoOperacao.TipoEmissaoIntramunicipal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal)dynConfiguracaoTipoOperacao.TipoEmissaoIntramunicipal;
                    configuracaoTipoOperacao.Empresa = empresa;

                    if ((int)dynConfiguracaoTipoOperacao.SerieIntraestadual.Codigo > 0)
                        configuracaoTipoOperacao.SerieIntraestadual = repSerie.BuscarPorCodigo((int)dynConfiguracaoTipoOperacao.SerieIntraestadual.Codigo);

                    if ((int)dynConfiguracaoTipoOperacao.SerieInterestadual.Codigo > 0)
                        configuracaoTipoOperacao.SerieInterestadual = repSerie.BuscarPorCodigo((int)dynConfiguracaoTipoOperacao.SerieInterestadual.Codigo);

                    if ((int)dynConfiguracaoTipoOperacao.SerieMDFe.Codigo > 0)
                        configuracaoTipoOperacao.SerieMDFe = repSerie.BuscarPorCodigo((int)dynConfiguracaoTipoOperacao.SerieMDFe.Codigo);

                    if (novo)
                    {
                        repConfiguracaoTipoOperacao.Inserir(configuracaoTipoOperacao);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, $"Adicionou a configuração de tipo operação.", unidadeDeTrabalho);
                    }
                    else
                    {
                        repConfiguracaoTipoOperacao.Atualizar(configuracaoTipoOperacao);
                        if (configuracaoTipoOperacao.GetChanges().Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, configuracaoTipoOperacao.GetChanges(), $"Atualizou a configuração de tipo operação.", unidadeDeTrabalho);
                    }
                }
            }

            foreach (int configuracao in codigosTipoOperacao)
            {
                Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao configuracaoTipoOperacao = repConfiguracaoTipoOperacao.BuscarPorCodigo(configuracao, false);
                repConfiguracaoTipoOperacao.Deletar(configuracaoTipoOperacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, $"Deletou a configuração de tipo operação.", unidadeDeTrabalho);
            }
        }

        private void SalvarConfiguracaoIntegracao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa repConfiguracaoIntegracaoEmpresa = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa(unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa configuracaoIntegracaoEmpresa = repConfiguracaoIntegracaoEmpresa.BuscarPorEmpresa(empresa.Codigo);

            if (configuracaoIntegracaoEmpresa == null)
            {
                configuracaoIntegracaoEmpresa = new Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa()
                {
                    Empresa = empresa,
                    TipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
                };
            }

            configuracaoIntegracaoEmpresa.CodigoIntegracao = Request.Params("OpenTech") ?? string.Empty;
            configuracaoIntegracaoEmpresa.CodigoClienteOpenTech = Request.GetIntParam("CodigoClienteOpenTech");
            configuracaoIntegracaoEmpresa.CodigoPASOpenTech = Request.GetIntParam("CodigoPASOpenTech");

            if (configuracaoIntegracaoEmpresa.Codigo > 0)
                repConfiguracaoIntegracaoEmpresa.Atualizar(configuracaoIntegracaoEmpresa);
            else
                repConfiguracaoIntegracaoEmpresa.Inserir(configuracaoIntegracaoEmpresa);
        }

        private void SalvarSeries(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

            dynamic series = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Series"));

            if (series.Count > 0)
            {
                foreach (dynamic serie in series)
                {
                    Dominio.Entidades.EmpresaSerie empSerie = repSerie.BuscarPorSerie(empresa.Codigo, (int)serie.Numero, (Dominio.Enumeradores.TipoSerie)serie.Tipo);

                    if (empSerie == null)
                    {
                        empSerie = new Dominio.Entidades.EmpresaSerie();
                        empSerie.Empresa = empresa;
                        empSerie.Numero = (int)serie.Numero;
                        empSerie.Tipo = (Dominio.Enumeradores.TipoSerie)serie.Tipo;
                    }

                    empSerie.Initialize();
                    empSerie.ProximoNumeroDocumento = (int)serie.ProximoNumeroDocumento;
                    empSerie.Status = (string)serie.Status;
                    empSerie.NaoGerarCargaAutomaticamente = (bool)serie.NaoGerarCargaAutomaticamente;

                    if (empSerie.Codigo > 0)
                    {
                        repSerie.Atualizar(empSerie);
                        if (empSerie.GetChanges().Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empSerie.GetChanges(), $"Atualizou a série {empSerie.Numero}.", unidadeDeTrabalho);
                    }
                    else
                    {
                        repSerie.Inserir(empSerie);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empSerie.GetChanges(), $"Adicionou a série {empSerie.Numero}.", unidadeDeTrabalho);
                    }
                }
            }
            else
                SalvarSeriePadrao(empresa, unidadeDeTrabalho);
        }

        private void SalvarEmpresaFilialEmbarcador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);

            dynamic dynFilials = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FiliaisEmbarcador"));

            if (empresa.FiliaisEmbarcadorHabilitado == null)
                empresa.FiliaisEmbarcadorHabilitado = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            empresa.FiliaisEmbarcadorHabilitado.Clear();

            foreach (dynamic dynFilial in dynFilials)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo((int)dynFilial.Filial.Codigo);
                empresa.FiliaisEmbarcadorHabilitado.Add(filial);
            }
            repEmpresa.Atualizar(empresa);
        }

        private void SalvarEmpresaFilial(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            dynamic empresasFilials = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            if (empresa.Filiais == null)
                empresa.Filiais = new List<Dominio.Entidades.Empresa>();

            empresa.Filiais.Clear();

            foreach (dynamic dynEmpresaFilial in empresasFilials)
            {
                Dominio.Entidades.Empresa empresaFilial = repEmpresa.BuscarPorCodigo((int)dynEmpresaFilial.Empresa.Codigo);

                Dominio.Entidades.Empresa matriz = null;
                if (empresaFilial.Matriz.Count > 0)
                {
                    matriz = empresaFilial.Matriz.FirstOrDefault();
                }

                if ((matriz != null && matriz.Codigo != empresa.Codigo) || empresaFilial.Filiais.Count > 0 || empresa.Matriz.Count > 0)
                    throw new ControllerException("Não é possível adicionar uma filial que pertence à outra Matriz");
                empresa.Filiais.Add(empresaFilial);
            }
        }

        private void SalvarEstadosFeeder(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

            dynamic estadosFeeder = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosFeeder"));

            if (empresa.EstadosFeeder == null)
            {
                empresa.EstadosFeeder = new List<Dominio.Entidades.Estado>();

                foreach (dynamic dynEstado in estadosFeeder)
                {
                    Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)dynEstado.Estado.Codigo);
                    empresa.EstadosFeeder.Add(estado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Adicionou estados feeder", unidadeDeTrabalho);
            }
            else
            {
                List<int> codigosDeletar = empresa.EstadosFeeder.Select(o => o.Codigo).ToList();

                foreach (dynamic dynEstado in estadosFeeder)
                {
                    if (empresa.EstadosFeeder.Any(o => o.Codigo == (int)dynEstado.Estado.Codigo))
                        codigosDeletar.Remove((int)dynEstado.Estado.Codigo);
                    else
                    {
                        Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)dynEstado.Estado.Codigo);
                        empresa.EstadosFeeder.Add(estado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, $"Adicionou estado feeder {estado.Descricao}", unidadeDeTrabalho);
                    }
                }

                foreach (int codigo in codigosDeletar)
                {
                    Dominio.Entidades.Estado estado = empresa.EstadosFeeder.First(o => o.Codigo == codigo);
                    empresa.EstadosFeeder.Remove(estado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, $"Removeu estado feeder {estado.Descricao}", unidadeDeTrabalho);
                }
            }
        }

        private void SalvarIntelipostDadosIntegracao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Global.EmpresaIntelipostIntegracao repEmpresaIntelipostIntegracao = new Repositorio.Global.EmpresaIntelipostIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unidadeDeTrabalho);

            dynamic dadosIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("IntelipostDadosIntegracao"));

            repEmpresaIntelipostIntegracao.DeletarPorEmpresa(empresa.Codigo);

            foreach (dynamic dyndadosIntegracao in dadosIntegracao)
            {

                Dominio.Entidades.EmpresaIntelipostIntegracao integracao = new Dominio.Entidades.EmpresaIntelipostIntegracao
                {
                    Empresa = empresa,
                    CanalEntrega = repCanalEntega.BuscarPorCodigo((int)dyndadosIntegracao.CanalEntrega.Codigo),
                    Token = dyndadosIntegracao.Token
                };

                repEmpresaIntelipostIntegracao.Inserir(integracao);

            }
        }

        private void SalvarIntelipostTipoOcorrencia(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Global.EmpresaIntelipostTipoOcorrencia repEmpresaIntelipostTipoOcorrencia = new Repositorio.Global.EmpresaIntelipostTipoOcorrencia(unidadeDeTrabalho);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);

            dynamic dadosTipoOcorrencia = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("IntelipostTipoOcorrencia"));

            repEmpresaIntelipostTipoOcorrencia.DeletarPorEmpresa(empresa.Codigo);

            foreach (dynamic dyndadosTipoOcorrencia in dadosTipoOcorrencia)
            {

                Dominio.Entidades.EmpresaIntelipostTipoOcorrencia tipoOcorrencia = new Dominio.Entidades.EmpresaIntelipostTipoOcorrencia
                {
                    Empresa = empresa,

                    MicroStatus = dyndadosTipoOcorrencia.MicroStatus,
                    MacroStatus = dyndadosTipoOcorrencia.MacroStatus,
                    CodigoIntegracao = dyndadosTipoOcorrencia.CodigoIntegracao,
                    TipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo((int)dyndadosTipoOcorrencia.TipoOcorrencia.Codigo)
                };

                repEmpresaIntelipostTipoOcorrencia.Inserir(tipoOcorrencia);

            }

        }

        private void SalvarLayoutsEDI(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Transportadores.TransportadorLayoutEDI repTransportadorLayoutEDI = new Repositorio.Embarcador.Transportadores.TransportadorLayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

            dynamic layoutsEDI = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoLayoutEDI"));
            List<int> codigosExistentes = new List<int>();
            int codigo = 0;

            for (int i = 0; i < layoutsEDI.Count; i++)
                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    codigosExistentes.Add(codigo);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsExistentes = repTransportadorLayoutEDI.BuscarPorEmpresa(empresa.Codigo);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsDeletar = (from obj in layoutsExistentes where !codigosExistentes.Contains(obj.Codigo) select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutDeletar in layoutsDeletar)
            {
                layoutsExistentes.Remove(layoutDeletar);
                repTransportadorLayoutEDI.Deletar(layoutDeletar);
            }

            for (int i = 0; i < layoutsEDI.Count; i++)
            {
                dynamic layoutEDI = layoutsEDI[i];

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI empresaLayoutEDI = null;

                if (int.TryParse((string)layoutEDI.Codigo, out codigo))
                    empresaLayoutEDI = (from obj in layoutsExistentes where obj.Codigo == codigo select obj).FirstOrDefault();

                if (empresaLayoutEDI == null)
                    empresaLayoutEDI = new Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI();

                empresaLayoutEDI.Empresa = empresa;
                empresaLayoutEDI.LayoutEDI = repLayoutEDI.Buscar((int)layoutEDI.CodigoLayoutEDI);
                empresaLayoutEDI.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao)(int)layoutsEDI[i].TipoIntegracao);

                empresaLayoutEDI.EnderecoFTP = null;
                empresaLayoutEDI.Diretorio = null;
                empresaLayoutEDI.Passivo = false;
                empresaLayoutEDI.Porta = null;
                empresaLayoutEDI.Senha = null;
                empresaLayoutEDI.Usuario = null;
                empresaLayoutEDI.Emails = null;
                empresaLayoutEDI.UtilizarSFTP = false;
                empresaLayoutEDI.CriarComNomeTemporaraio = false;
                empresaLayoutEDI.SSL = false;

                if (empresaLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                {
                    empresaLayoutEDI.EnderecoFTP = (string)layoutEDI.EnderecoFTP;
                    empresaLayoutEDI.Diretorio = (string)layoutEDI.Diretorio;
                    empresaLayoutEDI.Passivo = (bool)layoutEDI.Passivo;
                    empresaLayoutEDI.Porta = (string)layoutEDI.Porta;
                    empresaLayoutEDI.Senha = (string)layoutEDI.Senha;
                    empresaLayoutEDI.Usuario = (string)layoutEDI.Usuario;
                    empresaLayoutEDI.UtilizarSFTP = (bool)layoutEDI.UtilizarSFTP;
                    empresaLayoutEDI.CriarComNomeTemporaraio = (bool)layoutEDI.CriarComNomeTemporaraio;
                    empresaLayoutEDI.SSL = (bool)layoutEDI.SSL;
                }
                else if (empresaLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                {
                    empresaLayoutEDI.Emails = (string)layoutEDI.Emails;
                }

                if (empresaLayoutEDI.Codigo > 0)
                {
                    empresaLayoutEDI.Initialize();
                    repTransportadorLayoutEDI.Atualizar(empresaLayoutEDI, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesLayout = empresaLayoutEDI.GetChanges();
                    if (alteracoesLayout.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, alteracoesLayout, Localization.Resources.Transportadores.Transportador.AlterouLayout + empresaLayoutEDI.Descricao + ".", unidadeDeTrabalho);
                }
                else
                {
                    repTransportadorLayoutEDI.Inserir(empresaLayoutEDI, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.AlterouLayout + empresaLayoutEDI.Descricao + ".", unidadeDeTrabalho);
                }
            }
        }

        private void SalvarPermissoes(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unidadeDeTrabalho);
            Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);
            Repositorio.PaginaUsuario repPermissaoUsuario = new Repositorio.PaginaUsuario(unidadeDeTrabalho);

            dynamic perm = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Permissoes"));

            List<int> codigosPaginas = new List<int>();

            for (int i = 0; i < perm.Count; i++)
                codigosPaginas.Add((int)perm[i].Codigo);

            List<Dominio.Entidades.PermissaoEmpresa> permissoes = repPermissaoEmpresa.BuscarPorEmpresa(empresa.Codigo);

            if (permissoes.Count() > 0)
            {
                List<Dominio.Entidades.PermissaoEmpresa> permissoesDeletar = (from obj in permissoes where !codigosPaginas.Contains(obj.Pagina.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.PermissaoEmpresa permissaoDeletar in permissoesDeletar)
                {
                    repPermissaoEmpresa.Deletar(permissaoDeletar);

                    List<Dominio.Entidades.PaginaUsuario> paginasUsuarios = repPermissaoUsuario.BuscarPorEmpresaEPagina(empresa.Codigo, permissaoDeletar.Pagina.Codigo);

                    foreach (Dominio.Entidades.PaginaUsuario pagina in paginasUsuarios)
                        repPermissaoUsuario.Deletar(pagina);
                }
            }

            foreach (int codigoPagina in codigosPaginas)
            {
                Dominio.Entidades.PermissaoEmpresa permissao = (from obj in permissoes where obj.Pagina.Codigo == codigoPagina select obj).FirstOrDefault();

                if (permissao == null)
                    permissao = new Dominio.Entidades.PermissaoEmpresa();

                permissao.Empresa = empresa;
                permissao.Pagina = repPagina.BuscarPorCodigo(codigoPagina);
                permissao.PermissaoDeAcesso = "A";
                permissao.PermissaoDeAlteracao = "A";
                permissao.PermissaoDeDelecao = "A";
                permissao.PermissaoDeInclusao = "A";

                if (permissao.Codigo > 0)
                    repPermissaoEmpresa.Atualizar(permissao);
                else
                    repPermissaoEmpresa.Inserir(permissao);
            }
        }

        private void SalvarPermissoesTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador repPerfilAcessoTransportador = new Repositorio.Embarcador.Transportadores.PerfilAcessoTransportador(unitOfWork);


            int codigoPerfil;
            int.TryParse(Request.Params("PerfilAcessoTransportador"), out codigoPerfil);
            Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador perfil = repPerfilAcessoTransportador.BuscarPorCodigo(codigoPerfil);


            empresa.PerfilAcessoTransportador = perfil;

            // Módulos de acesso
            if (empresa.PerfilAcessoTransportador != null && perfil.ModulosLiberados != null && perfil.ModulosLiberados?.Count > 0)
            {
                empresa.ModulosLiberados = new List<int>();

                foreach (int modulo in perfil.ModulosLiberados)
                    empresa.ModulosLiberados.Add(modulo);
            }
            else
            {
                empresa.ModulosLiberados = new List<int>();

                dynamic jModulosUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosTransportador"));

                foreach (dynamic jModulo in jModulosUsuario)
                    empresa.ModulosLiberados.Add((int)jModulo.CodigoModulo);
            }

            // Formulários de acesso
            // Trecho comentado até que situação seja resolvida conforme solicitada pelo Rodrigo na Tarefa #66976
            //SalvarFormulariosPerfilAcessoTransportadorParaTransportador(empresa, unitOfWork);

            repEmpresa.Atualizar(empresa);
        }

        private void SalvarSerieRPS(string serieRps, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(empresa.Codigo);

            if (transportadorConfiguracaoNFSe == null && !string.IsNullOrWhiteSpace(serieRps))
            {
                transportadorConfiguracaoNFSe = new Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe();

                transportadorConfiguracaoNFSe.Empresa = empresa;
                transportadorConfiguracaoNFSe.SerieRPS = serieRps;
                repTransportadorConfiguracaoNFSe.Inserir(transportadorConfiguracaoNFSe);
            }
            else if (transportadorConfiguracaoNFSe != null)
            {
                transportadorConfiguracaoNFSe.SerieRPS = serieRps;
                repTransportadorConfiguracaoNFSe.Atualizar(transportadorConfiguracaoNFSe);
            }
        }

        private void SalvarTransportadorFilial(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.TransportadorFilial repTransportadorFilial = new Repositorio.Embarcador.Transportadores.TransportadorFilial(unidadeDeTrabalho);

            if (empresa.Codigo > 0)
                repTransportadorFilial.DeletarPorEmpresa(empresa.Codigo);

            dynamic empresasFilials = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TransportadorFiliais"));

            foreach (dynamic dynEmpresaFilial in empresasFilials)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial empresaFilial = new Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial();
                empresaFilial.CNPJ = Utilidades.String.OnlyNumbers((string)dynEmpresaFilial.CNPJ);
                empresaFilial.Empresa = empresa;
                repTransportadorFilial.Inserir(empresaFilial);
            }
        }

        private void SalvarIntegracaoKrona(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool existeIntegracaoKrona = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Krona);

            if (!existeIntegracaoKrona)
                return;

            Repositorio.EmpresaIntegracaoKrona repositorioEmpresaIntegracaoKrona = new Repositorio.EmpresaIntegracaoKrona(unitOfWork);
            Dominio.Entidades.EmpresaIntegracaoKrona empresaIntegracaoKrona = (empresa.Codigo > 0) ? repositorioEmpresaIntegracaoKrona.BuscarPorEmpresa(empresa.Codigo) : null;
            dynamic integracaoKrona = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("IntegracaoKrona"));

            if (empresaIntegracaoKrona == null)
            {
                empresaIntegracaoKrona = new Dominio.Entidades.EmpresaIntegracaoKrona()
                {
                    Empresa = empresa
                };
            }

            empresaIntegracaoKrona.Senha = ((string)integracaoKrona.Senha).Trim();
            empresaIntegracaoKrona.Usuario = ((string)integracaoKrona.Usuario).Trim();

            if (empresaIntegracaoKrona.Codigo > 0)
                repositorioEmpresaIntegracaoKrona.Atualizar(empresaIntegracaoKrona);
            else
                repositorioEmpresaIntegracaoKrona.Inserir(empresaIntegracaoKrona);
        }

        private void SalvarRotasFreteValePedagio(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio repTransportadorRotaFreteValePedagio = new Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            dynamic dynRotasFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasFreteValePedagio"));

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio> rotasFrete = repTransportadorRotaFreteValePedagio.BuscarPorEmpresa(empresa.Codigo);

            if (rotasFrete.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic rotaFrete in dynRotasFrete)
                {
                    int codigo = ((string)rotaFrete.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio> deletar = (from obj in rotasFrete where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio rotaFrete in deletar)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.ExcluiuRotaRrete + "{rotaFrete.RotaFrete.Descricao}" + Localization.Resources.Transportadores.Transportador.DoTipo + "{rotaFrete.TipoRotaFrete.ObterDescricao()}" + Localization.Resources.Transportadores.Transportador.DoValePedagio, unitOfWork);
                    repTransportadorRotaFreteValePedagio.Deletar(rotaFrete);
                }
            }

            foreach (dynamic rotaFrete in dynRotasFrete)
            {
                int codigo = ((string)rotaFrete.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio rotaFreteValePedagio = codigo > 0 ? repTransportadorRotaFreteValePedagio.BuscarPorCodigo(codigo, false) : null;

                if (rotaFreteValePedagio == null)
                {
                    rotaFreteValePedagio = new Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio();

                    rotaFreteValePedagio.Empresa = empresa;
                    rotaFreteValePedagio.RotaFrete = repRotaFrete.BuscarPorCodigo(((string)rotaFrete.RotaFrete.Codigo).ToInt());
                    rotaFreteValePedagio.TipoRotaFrete = ((string)rotaFrete.TipoRotaFrete).ToEnum<TipoRotaFrete>();

                    repTransportadorRotaFreteValePedagio.Inserir(rotaFreteValePedagio);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.AdicionouRotaFrete + "{rotaFreteValePedagio.RotaFrete.Descricao}" + Localization.Resources.Transportadores.Transportador.DoTipo + "{rotaFreteValePedagio.TipoRotaFrete.ObterDescricao()}" + Localization.Resources.Transportadores.Transportador.DoValePedagio, unitOfWork);
                }
            }
        }

        private void SalvarInscricoesST(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorInscricaoST repTransportadorInscricaoST = new Repositorio.Embarcador.Transportadores.TransportadorInscricaoST(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            dynamic dynInscricoesST = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("InscricoesST"));

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST> inscricoesST = repTransportadorInscricaoST.BuscarPorEmpresa(empresa.Codigo);

            if (inscricoesST.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic inscricaoST in dynInscricoesST)
                {
                    int codigo = ((string)inscricaoST.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST> deletar = (from obj in inscricoesST where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST inscricaoST in deletar)
                {
                    string autoriaEstado = inscricaoST.Estado != null ? Localization.Resources.Transportadores.Transportador.DoEstado + inscricaoST.Estado?.Nome : string.Empty;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.ExcluiuInscricaoST + inscricaoST.InscricaoST + autoriaEstado, unitOfWork);
                    repTransportadorInscricaoST.Deletar(inscricaoST);
                }
            }

            foreach (dynamic inscricaoST in dynInscricoesST)
            {
                int codigo = ((string)inscricaoST.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST transportadorInscricaoST = codigo > 0 ? repTransportadorInscricaoST.BuscarPorCodigo(codigo, false) : null;

                if (transportadorInscricaoST == null)
                {
                    transportadorInscricaoST = new Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST();

                    string estado = (string)inscricaoST.Estado.Codigo;

                    transportadorInscricaoST.Empresa = empresa;
                    transportadorInscricaoST.Estado = !string.IsNullOrWhiteSpace(estado) && !estado.Equals("0") ? repEstado.BuscarPorSigla(estado) : null;
                    transportadorInscricaoST.InscricaoST = (string)inscricaoST.InscricaoEstadual;

                    repTransportadorInscricaoST.Inserir(transportadorInscricaoST);

                    string autoriaEstado = transportadorInscricaoST.Estado != null ? Localization.Resources.Transportadores.Transportador.DoEstado + transportadorInscricaoST.Estado?.Nome : string.Empty;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.AdicionouInscricaoST + transportadorInscricaoST.InscricaoST + autoriaEstado, unitOfWork);
                }
            }
        }

        private void SalvarDadosCliente(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool naoAtualizarNomeFantasiaCliente = false)
        {
            if (empresa.Tipo == "E") //Exterior
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
            bool inserir = false;
            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
                cliente.CPF_CNPJ = double.Parse(empresa.CNPJ);
                cliente.Atividade = repAtividade.BuscarPrimeiraAtividade();
                cliente.Ativo = true;
                cliente.Tipo = "J";
            }

            cliente.Email = empresa.Email;
            cliente.EmailStatus = !string.IsNullOrWhiteSpace(empresa.Email) ? "A" : "I";
            cliente.EmailContador = empresa.EmailContador;
            cliente.EmailContadorStatus = !string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) ? "A" : "I";
            cliente.IE_RG = Utilidades.String.OnlyNumbers(empresa.InscricaoEstadual);
            cliente.Nome = empresa.RazaoSocial;
            if (!naoAtualizarNomeFantasiaCliente)
                cliente.NomeFantasia = empresa.NomeFantasia;
            cliente.Bairro = empresa.Bairro;
            cliente.CEP = empresa.CEP;
            cliente.Complemento = Utilidades.String.Left(empresa.Complemento, 60);
            cliente.Endereco = empresa.Endereco;
            cliente.Localidade = empresa.Localidade;
            cliente.Numero = empresa.Numero;
            cliente.Telefone1 = Utilidades.String.OnlyNumbers(empresa.Telefone);
            cliente.Telefone2 = Utilidades.String.OnlyNumbers(empresa.TelefoneContato);
            cliente.Pais = cliente.Localidade.Pais;

            cliente.NumeroConta = empresa.NumeroConta;
            cliente.Agencia = empresa.Agencia;
            cliente.DigitoAgencia = empresa.DigitoAgencia;
            cliente.Banco = empresa.Banco;
            cliente.TipoContaBanco = empresa.TipoContaBanco;

            if (inserir)
                repCliente.Inserir(cliente);
            else
                repCliente.Atualizar(cliente);
        }

        private decimal ConverteDecimalImpostoCIOT(dynamic valor)
        {
            string strValor = (string)valor;
            strValor = strValor.Replace(".", ",");

            decimal.TryParse(strValor, out decimal valorDecimal);
            return valorDecimal;
            //if (string.IsNullOrEmpty((string)valor))
            //    return 0;

            //return (decimal)valor;


            //string strValor = (string)valor;
            //strValor = strValor.Replace(".", "");

            //return strValor.ToDecimal();
        }

        private void SalvarImpostoContratoFrete(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IRImpostoContratoFrete repIR = new Repositorio.IRImpostoContratoFrete(unitOfWork);
            Repositorio.INSSImpostoContratoFrete repINSS = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
            Repositorio.ImpostoContratoFrete repImposto = new Repositorio.ImpostoContratoFrete(unitOfWork);


            // Agrupador dos impstos
            Dominio.Entidades.ImpostoContratoFrete imposto = repImposto.BuscarPorEmpresa(empresa.Codigo);
            if (imposto == null)
            {
                imposto = new Dominio.Entidades.ImpostoContratoFrete
                {
                    Empresa = empresa
                };
                repImposto.Inserir(imposto);
            }
            else
            {
                imposto.Initialize();
            }

            Dominio.Entidades.Auditoria.HistoricoObjeto historico = repImposto.Atualizar(imposto, Auditado);


            // Imposto de Renda
            imposto.PercentualBCIR = Request.GetDecimalParam("ImpostoCIOTBaseCalculoIR");
            dynamic faixasIR = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ImpostoRendaCIOT"));
            List<int> codigosIRCadastrados = new List<int>();

            foreach (dynamic objIR in faixasIR)
            {
                int.TryParse((string)objIR.Codigo, out int codigo);
                Dominio.Entidades.IRImpostoContratoFrete ir = repIR.BuscarPorCodigo(codigo);

                if (ir == null)
                {
                    ir = new Dominio.Entidades.IRImpostoContratoFrete
                    {
                        Imposto = imposto
                    };
                }
                else
                {
                    ir.Initialize();
                }

                ir.PercentualAplicar = ConverteDecimalImpostoCIOT(objIR.Aplicar);
                ir.ValorDeduzir = ConverteDecimalImpostoCIOT(objIR.Deduzir);
                ir.ValorInicial = ConverteDecimalImpostoCIOT(objIR.De);
                ir.ValorFinal = ConverteDecimalImpostoCIOT(objIR.Ate);

                if (ir.Codigo > 0)
                    repIR.Atualizar(ir, Auditado, historico);
                else
                    repIR.Inserir(ir, Auditado, historico);

                codigosIRCadastrados.Add(ir.Codigo);
            }

            foreach (Dominio.Entidades.IRImpostoContratoFrete ir in repIR.BuscarCodigosDiferentes(imposto.Codigo, codigosIRCadastrados))
                repIR.Deletar(ir, Auditado, historico);


            // INSS
            imposto.PercentualBCINSS = Request.GetDecimalParam("ImpostoCIOTBaseCalculoINSS");
            imposto.ValorTetoRetencaoINSS = Request.GetDecimalParam("ImpostoCIOTTetoRetencaoINSS");
            dynamic faixasINSS = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ImpostoINSSCIOT"));
            List<int> codigosINSSCadastrados = new List<int>();

            foreach (dynamic objINSS in faixasINSS)
            {
                int.TryParse((string)objINSS.Codigo, out int codigo);
                Dominio.Entidades.INSSImpostoContratoFrete inss = repINSS.BuscarPorCodigo(codigo);

                if (inss == null)
                {
                    inss = new Dominio.Entidades.INSSImpostoContratoFrete
                    {
                        Imposto = imposto
                    };
                }
                else
                {
                    inss.Initialize();
                }

                inss.PercentualAplicar = ConverteDecimalImpostoCIOT(objINSS.Aplicar);
                inss.ValorInicial = ConverteDecimalImpostoCIOT(objINSS.De);
                inss.ValorFinal = ConverteDecimalImpostoCIOT(objINSS.Ate);

                if (inss.Codigo > 0)
                    repINSS.Atualizar(inss, Auditado, historico);
                else
                    repINSS.Inserir(inss, Auditado, historico);

                codigosINSSCadastrados.Add(inss.Codigo);
            }

            foreach (Dominio.Entidades.INSSImpostoContratoFrete inss in repINSS.BuscarCodigosDiferentes(imposto.Codigo, codigosINSSCadastrados))
                repINSS.Deletar(inss, Auditado, historico);


            // SEST/SENAT
            imposto.AliquotaSEST = Request.GetDecimalParam("ImpostoCIOTAliquotaSEST");
            imposto.AliquotaSENAT = Request.GetDecimalParam("ImpostoCIOTAliquotaSENAT");

            if (imposto.IsInitialized())
                repImposto.Atualizar(imposto, Auditado, historico);
        }

        private void SalvarAutomacaoEmissaoNFSManual(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic dynAutomacaoEmissaoNFSManual = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AutomacaoEmissaoNFSManual"));

            if (dynAutomacaoEmissaoNFSManual != null)
            {
                if (!string.IsNullOrWhiteSpace((string)dynAutomacaoEmissaoNFSManual.Periodicidade))
                    empresa.PeriodicidadeEmissaoNFSManual = (Periodicidade)dynAutomacaoEmissaoNFSManual.Periodicidade;

                if (empresa.PeriodicidadeEmissaoNFSManual > 0 && empresa.PeriodicidadeEmissaoNFSManual == Periodicidade.Mensal)
                {
                    empresa.DiaMesEmissaoNFSManual = string.IsNullOrWhiteSpace((string)dynAutomacaoEmissaoNFSManual.DiaMes) ? 1 : (int)dynAutomacaoEmissaoNFSManual.DiaMes;
                    empresa.DiaSemanaEmissaoNFSManual = DiaSemana.Segunda;
                }
                else if (empresa.PeriodicidadeEmissaoNFSManual > 0 && empresa.PeriodicidadeEmissaoNFSManual == Periodicidade.Semanal && !string.IsNullOrWhiteSpace((string)dynAutomacaoEmissaoNFSManual.DiaSemana))
                {
                    empresa.DiaSemanaEmissaoNFSManual = (DiaSemana)dynAutomacaoEmissaoNFSManual.DiaSemana;
                    empresa.DiaMesEmissaoNFSManual = 1;
                }
                else if (empresa.PeriodicidadeEmissaoNFSManual > 0 && empresa.PeriodicidadeEmissaoNFSManual == Periodicidade.Diario)
                {
                    empresa.DiaSemanaEmissaoNFSManual = DiaSemana.Segunda;
                    empresa.DiaMesEmissaoNFSManual = 1;
                }
            }
        }

        private void SalvarTransportadorNFSe(Dominio.Entidades.Empresa empresa)
        {
            dynamic dynTransportadorNFSe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TransportadorNFSe"));
            if (dynTransportadorNFSe == null)
                return;

            empresa.EmissaoNFSeForaDoSistema = ((string)dynTransportadorNFSe.EmissaoNFSeForaDoSistema).ToBool();
            empresa.NaoIncrementarNumeroLoteRPSAutomaticamente = ((string)dynTransportadorNFSe.NaoIncrementarNumeroLoteRPSAutomaticamente).ToBool();
            empresa.NFSeNacional = ((string)dynTransportadorNFSe.NFSeNacional).ToBool();
        }

        private void SalvarCodigosIntegracao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorCodigoIntegracao repositorioTransportadorCodigoIntegracao = new Repositorio.Embarcador.Transportadores.TransportadorCodigoIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> listaCodigosIntegracao = repositorioTransportadorCodigoIntegracao.BuscarPorTransportador(empresa.Codigo);
            dynamic dynCodigosIntegracao = JsonConvert.DeserializeObject<dynamic>(Request.Params("CodigosIntegracao"));

            foreach (dynamic dynCodigoIntegracao in dynCodigosIntegracao)
            {
                int codigo = ((string)dynCodigoIntegracao.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao codigoIntegracao = codigo > 0 ? listaCodigosIntegracao.First(o => o.Codigo == codigo) : null;

                if (codigoIntegracao != null && listaCodigosIntegracao.Contains(codigoIntegracao))
                    listaCodigosIntegracao.Remove(codigoIntegracao);
                else
                {
                    Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao transportadorCodigoIntegracao = new Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao()
                    {
                        CodigoIntegracao = ((string)dynCodigoIntegracao.CodigoIntegracao).ToString(),
                        Empresa = empresa
                    };

                    repositorioTransportadorCodigoIntegracao.Inserir(transportadorCodigoIntegracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, $"Adicionou  o código de integração {(string)dynCodigoIntegracao.CodigoIntegracao}.", unitOfWork);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao codigoIntegracaoARemover in listaCodigosIntegracao)
            {
                repositorioTransportadorCodigoIntegracao.Deletar(codigoIntegracaoARemover);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, $"Removeu o código de integração {codigoIntegracaoARemover.CodigoIntegracao}.", unitOfWork);
            }
        }

        private string obterFonteColor(Dominio.Entidades.Empresa empresa)
        {
            string DT_FontColor = "";
            if (empresa.EmissaoDocumentosForaDoSistema)
                DT_FontColor = "#FFF";

            return DT_FontColor;
        }

        private dynamic ObterComponentesCTesImportados(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado repositorio = new Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado(unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> listaComponentesCTesImportados = repositorio.BuscarPorTransportadorComFetchComponenteFrete(empresa.Codigo);

            dynamic dynComponentes = (from obj in listaComponentesCTesImportados
                                      select new
                                      {
                                          obj.Codigo,
                                          obj.Descricao,
                                          CodigoComponenteFrete = obj.ComponenteFrete.Codigo,
                                          DescricaoComponenteFrete = obj.ComponenteFrete.Descricao
                                      }).ToList();

            return dynComponentes;
        }

        private dynamic ObterCodigosIntegracao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorCodigoIntegracao repositorioTransportadorCodigoIntegracao = new Repositorio.Embarcador.Transportadores.TransportadorCodigoIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> codigosIntegracao = repositorioTransportadorCodigoIntegracao.BuscarPorTransportador(empresa.Codigo);

            return (from obj in codigosIntegracao
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoIntegracao
                    }).ToList();
        }

        private string obterCorRow(Dominio.Entidades.Empresa empresa)
        {
            string DT_RowColor = "";
            if (!empresa.EmissaoDocumentosForaDoSistema)
                DT_RowColor = empresa.DataFinalCertificado.HasValue ? (empresa.EmpresaPai != null && empresa.TipoAmbiente == empresa.EmpresaPai.TipoAmbiente && !String.IsNullOrWhiteSpace(empresa.NomeCertificado) && empresa.DataFinalCertificado.Value >= DateTime.Now) ? "#dff0d8" : "" : "";
            else
                DT_RowColor = "#20B2AA";

            return DT_RowColor;
        }

        private string ObterLogoBase64(Dominio.Entidades.Empresa empresa)
        {
            string caminho = empresa.CaminhoLogoDacte;
            if (string.IsNullOrWhiteSpace(caminho) || !Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Descricao")
                propriedadeOrdenar = "RazaoSocial";

            return propriedadeOrdenar;
        }

        private void SalvarOperadoresTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            dynamic operadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Operadores"));

            if (empresa.Operadores == null)
                empresa.Operadores = new List<Dominio.Entidades.Usuario>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic operador in operadores)
                    codigos.Add((int)operador.Codigo);

                List<Dominio.Entidades.Usuario> tiposDeletar = empresa.Operadores.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Usuario operadorDeletar in tiposDeletar)
                    empresa.Operadores.Remove(operadorDeletar);
            }

            foreach (dynamic operador in operadores)
            {
                if (empresa.Operadores.Any(o => o.Codigo == (int)operador.Codigo))
                    continue;

                Dominio.Entidades.Usuario operadorObj = repositorioUsuario.BuscarPorCodigo((int)operador.Codigo);
                empresa.Operadores.Add(operadorObj);
            }
        }

        private void SalvarFormulariosPerfilAcessoTransportadorParaTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);
            Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario repositorioPerfilTransportadorFormulario = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormulario(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada repTransportadorFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada(unitOfWork);
            Repositorio.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada repPerfilTransportadorFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario> listaPerfilTransportadorFormulario = repositorioPerfilTransportadorFormulario.BuscarPorPerfil(empresa?.PerfilAcessoTransportador?.Codigo ?? 0);

            if (listaPerfilTransportadorFormulario?.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> transportadorFormularioValidacao = repTransportadorFormulario.BuscarPorEmpresa(empresa?.Codigo ?? 0);

                foreach (Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario perfilTransportadorFormulario in listaPerfilTransportadorFormulario)
                {
                    if (transportadorFormularioValidacao.Any(o => o.Empresa.Codigo == empresa.Codigo) &&
                        transportadorFormularioValidacao.Any(o => o.SomenteLeitura == perfilTransportadorFormulario.SomenteLeitura) &&
                        transportadorFormularioValidacao.Any(o => o.CodigoFormulario == perfilTransportadorFormulario.CodigoFormulario))
                        continue;
                    else
                    {
                        Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario transportadorFormulario = new Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario();
                        transportadorFormulario.Empresa = empresa;
                        transportadorFormulario.SomenteLeitura = perfilTransportadorFormulario.SomenteLeitura;
                        transportadorFormulario.CodigoFormulario = perfilTransportadorFormulario.CodigoFormulario;
                        repTransportadorFormulario.Inserir(transportadorFormulario);
                    }
                    // Permissões personalizada dos formulários
                    foreach (Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada formularioPermissaoPersonalizada in perfilTransportadorFormulario.FormularioPermissaoPersonalizada)
                    {
                        Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada perfilTransportadorFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada();
                        perfilTransportadorFormularioPermissaoPersonalizada.CodigoPermissao = formularioPermissaoPersonalizada.CodigoPermissao;
                        perfilTransportadorFormularioPermissaoPersonalizada.PerfilTransportadorFormulario = perfilTransportadorFormulario;
                        repPerfilTransportadorFormularioPermissaoPersonalizada.Inserir(perfilTransportadorFormularioPermissaoPersonalizada);
                    }
                }
            }
            else
            {
                dynamic jFormulariosUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FormulariosTransportador"));

                foreach (dynamic jFormulario in jFormulariosUsuario)
                {
                    Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario transportadorFormulario = new Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario();
                    transportadorFormulario.Empresa = empresa;
                    transportadorFormulario.SomenteLeitura = (bool)jFormulario.SomenteLeitura;
                    transportadorFormulario.CodigoFormulario = (int)jFormulario.CodigoFormulario;
                    repTransportadorFormulario.Inserir(transportadorFormulario);

                    // Permissões personalizada dos formulários
                    foreach (dynamic dynPermissao in jFormulario.PermissoesPersonalizadas)
                    {
                        Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada transportadorFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada();
                        transportadorFormularioPermissaoPersonalizada.CodigoPermissao = (int)dynPermissao.CodigoPermissaoPersonalizada;
                        transportadorFormularioPermissaoPersonalizada.TransportadorFormulario = transportadorFormulario;
                        repTransportadorFormularioPermissaoPersonalizada.Inserir(transportadorFormularioPermissaoPersonalizada);
                    }
                }
            }
        }
        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }


        #endregion

        #region Importação

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoISS()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ", Propriedade = "CNPJ", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Municipio", Propriedade = "LocalidadePrestacao", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "UF", Propriedade = "UF", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Serie", Propriedade = "SerieNFSe", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Serie do RPS", Propriedade = "SerieRPS", Tamanho = 10, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Servico NFSe", Propriedade = "ServicoNFSe", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Natureza", Propriedade = "NaturezaNFSe", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Aliquota do ISS", Propriedade = "AliquotaISS", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Retencao do ISS", Propriedade = "RetencaoISS", Tamanho = 6, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Tipo de Operacao", Propriedade = "TipoOperacao", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Incluir ISS Base Calculo", Propriedade = "IncluirISSBaseCalculo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        public async Task<IActionResult> ConfiguracaoImportacaoISS()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoISS();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarISS()
        {
            using var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var configuracoes = ObterConfiguracaoImportacaoISS();
                var dados = Request.Params("Dados");
                var linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                var listaTransportadorConfiguracaoNFSes = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
                var retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };
                var empresaSerie = new Dominio.Entidades.EmpresaSerie();
                var servicoNFSe = new Dominio.Entidades.ServicoNFSe();
                var repEmpresa = new Repositorio.Empresa(unitOfWork);
                var repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                var repLocalidade = new Repositorio.Localidade(unitOfWork);
                var repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                var repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                var repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                var repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                var repEstado = new Repositorio.Estado(unitOfWork);

                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        var transportadorConfiguracaoNFSe = new Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe();
                        var linha = linhas[i];
                        var erros = new StringBuilder();

                        string GetValor(string campo) =>
                            linha.Colunas.FirstOrDefault(c => c.NomeCampo == campo)?.Valor;

                        var cnpj = GetValor("CNPJ");
                        if (!string.IsNullOrWhiteSpace(cnpj))
                        {
                            var retornoCnpj = repEmpresa.BuscarEmpresaPorCNPJ(cnpj);
                            if (retornoCnpj == null)
                            {
                                erros.Append("Empresa não encontrada para o CNPJ informado. ");
                            }
                            else
                            {
                                transportadorConfiguracaoNFSe.Empresa = retornoCnpj;
                            }
                        }
                        else
                        {
                            erros.Append("Coluna 'CNPJ' é obrigatória. ");
                        }

                        var uf = GetValor("UF");
                        if (!string.IsNullOrWhiteSpace(uf))
                        {
                            var retornoUf = repEstado.BuscarPorSigla(uf);
                            if (retornoUf == null)
                            {
                                erros.Append("Empresa não encontrada para o CNPJ informado. ");
                            }
                            else
                            {
                                transportadorConfiguracaoNFSe.UFTomador = retornoUf;
                            }
                        }
                        else
                        {
                            erros.Append("Coluna 'UF' é obrigatória. ");
                        }

                        var localidade = GetValor("LocalidadePrestacao");
                        if (!string.IsNullOrWhiteSpace(localidade) && !string.IsNullOrWhiteSpace(uf))
                        {
                            var localidadePrestacao = repLocalidade.BuscarPorCidadeUF(localidade, uf);
                            if (localidadePrestacao == null)
                            {
                                erros.Append("Localidade de prestação não encontrada para a cidade e UF informadas. ");
                            }
                            else
                            {
                                transportadorConfiguracaoNFSe.LocalidadePrestacao = localidadePrestacao;
                                var servico = GetValor("ServicoNFSe");
                                if (!string.IsNullOrWhiteSpace(servico) && !string.IsNullOrWhiteSpace(localidade))
                                {
                                    var retornoServico = repServicoNFSe.BuscarPorNumeroELocalidadeECodigoTributacao(servico, localidadePrestacao.Codigo, "");
                                    if (retornoServico == null)
                                    {
                                        erros.Append("Servico não encontrada para a empresa e ServicoNFSe informadas. ");
                                    }
                                    else
                                    {
                                        transportadorConfiguracaoNFSe.ServicoNFSe = retornoServico;
                                    }
                                }
                                else
                                {
                                    erros.Append("Coluna 'Servico' é obrigatória. ");
                                }

                                var naturezaNFSe = GetValor("NaturezaNFSe");
                                if (!string.IsNullOrWhiteSpace(naturezaNFSe))
                                {
                                    var natureza = repNaturezaNFSe.BuscarPorDescricaoELocalidade(naturezaNFSe, localidadePrestacao.Codigo);
                                    if (natureza == null)
                                    {
                                        erros.Append("Natureza não encontrada para a NaturezaNFSe informadas. ");
                                    }
                                    else
                                    {
                                        transportadorConfiguracaoNFSe.NaturezaNFSe = natureza;
                                    }
                                }
                                else
                                {
                                    erros.Append("Coluna 'Natureza' é obrigatória. ");
                                }
                            }
                        }
                        else
                        {
                            erros.Append("Coluna 'Localidade' é obrigatória. ");
                        }

                        var serieRPS = GetValor("SerieRPS");
                        if (!string.IsNullOrWhiteSpace(serieRPS))
                            transportadorConfiguracaoNFSe.SerieRPS = serieRPS;

                        var tipoOperacao = GetValor("TipoOperacao");
                        if (!string.IsNullOrWhiteSpace(tipoOperacao))
                        {
                            var tipoRetorno = repTipoOperacao.BuscarPorDescricao(tipoOperacao);
                            if (tipoRetorno == null)
                            {
                                erros.Append("Coluna 'Tipo Operacao' esta Nula. ");
                            }
                            else
                            {
                                transportadorConfiguracaoNFSe.TipoOperacao = tipoRetorno;
                            }
                        }

                        var serie = GetValor("SerieNFSe");
                        if (!string.IsNullOrWhiteSpace(serie))
                        {
                            var retornoSerie = repEmpresaSerie.BuscarPorEmpresaNumeroTipo(transportadorConfiguracaoNFSe.Empresa.Codigo, int.Parse(serie), Dominio.Enumeradores.TipoSerie.NFSe);
                            if (retornoSerie == null)
                            {
                                erros.Append("Serie não encontrada para a empresa e SerieNFSe informadas. ");
                            }
                            else
                            {
                                transportadorConfiguracaoNFSe.SerieNFSe = retornoSerie;
                            }
                        }
                        else { erros.Append("Coluna 'SerieNFSe' é obrigatória."); }

                        var existeConfiguracao = repTransportadorConfiguracaoNFSe.BuscarParaAtualizarNaImportacao(
                                      transportadorConfiguracaoNFSe.Empresa.Codigo,
                                      transportadorConfiguracaoNFSe.LocalidadePrestacao != null ? transportadorConfiguracaoNFSe.LocalidadePrestacao.Codigo : 0,
                                       transportadorConfiguracaoNFSe.UFTomador.Sigla,
                                       transportadorConfiguracaoNFSe.ServicoNFSe != null ? transportadorConfiguracaoNFSe.ServicoNFSe.Codigo : 0,
                                       transportadorConfiguracaoNFSe.SerieRPS,
                                       transportadorConfiguracaoNFSe.NaturezaNFSe != null ? transportadorConfiguracaoNFSe.NaturezaNFSe.Codigo : 0,
                                       transportadorConfiguracaoNFSe.TipoOperacao != null ? transportadorConfiguracaoNFSe.TipoOperacao.Codigo : 0,
                                       transportadorConfiguracaoNFSe.IncluirISSBaseCalculo,
                                       transportadorConfiguracaoNFSe.SerieNFSe != null ? transportadorConfiguracaoNFSe.SerieNFSe.Codigo : 0
                                      );

                        var incluirISSBaseCalculo = GetValor("IncluirISSBaseCalculo");
                        if (!string.IsNullOrEmpty(incluirISSBaseCalculo))
                        {
                            if (existeConfiguracao is not null)
                            {
                                existeConfiguracao.IncluirISSBaseCalculo = incluirISSBaseCalculo.ToUpper() == "SIM";
                            }
                            else
                                transportadorConfiguracaoNFSe.IncluirISSBaseCalculo = incluirISSBaseCalculo.ToUpper() == "SIM";
                        }

                        var aliquotaISS = GetValor("AliquotaISS");
                        if (string.IsNullOrWhiteSpace(aliquotaISS) || !decimal.TryParse(aliquotaISS, out var aliquotaDecimal))
                            erros.Append("Coluna 'AliquotaISS' é obrigatória e deve ser numérica. ");
                        else
                        {
                            if (existeConfiguracao is not null)
                            {
                                existeConfiguracao.AliquotaISS = aliquotaDecimal;
                            }
                            else
                                transportadorConfiguracaoNFSe.AliquotaISS = aliquotaDecimal;
                        }

                        var retencaoISS = GetValor("RetencaoISS");
                        if (string.IsNullOrWhiteSpace(retencaoISS) || !decimal.TryParse(retencaoISS, out var retencaoDecimal))
                            erros.Append("Coluna 'RetencaoISS' é obrigatória e deve ser numérica. ");
                        else
                        {
                            if (existeConfiguracao is not null)
                            {
                                existeConfiguracao.RetencaoISS = retencaoDecimal;
                            }
                            else
                                transportadorConfiguracaoNFSe.RetencaoISS = retencaoDecimal;
                        }

                        if (erros.Length > 0)
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(erros.ToString(), i));
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, transportadorConfiguracaoNFSe, null, string.Concat(Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoImportarArquivo, " - ", erros.ToString()), unitOfWork);
                        }
                        else
                        {
                            contador++;
                            retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                            {
                                indice = i,
                                processou = true,
                                mensagemFalha = ""
                            });

                            //se nao existe insere um novo
                            if (existeConfiguracao != null)
                            {
                                repTransportadorConfiguracaoNFSe.Atualizar(existeConfiguracao);
                            }
                            else
                            {
                                repTransportadorConfiguracaoNFSe.Inserir(transportadorConfiguracaoNFSe);
                            }
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, transportadorConfiguracaoNFSe, null, string.Concat(Localization.Resources.Transportadores.Transportador.ImportacaoRealizadaComSucesso), unitOfWork);
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, unitOfWork, null, ex.Message, unitOfWork);
                unitOfWork.CommitChanges();
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
        }
        #endregion
    }
}