using Dominio.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers
{
    public class BaseController : SignController
    {
        #region Construtores

        public BaseController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;
        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao _clienteConfiguracao;
        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteURLAcesso;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral _configuracaoGeral;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega _configuracaoAgendamentoEntrega;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto _configuracaoCanhoto;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;
        private string _corFundoUsuario;
        private Dominio.Entidades.Empresa _empresa;
        private string _favicon;
        private bool? _isHomologacao;
        private string _layout;
        private string _logo;
        private Dominio.Entidades.Embarcador.Operacional.OperadorLogistica _operadorLogistica;
        private Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto _operadorCanhoto;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.Entidades.Usuario _usuario;
        private string _webServiceConsultaCTe;
        private string _webServiceOracle;

        #endregion Atributos

        #region Propriedades

        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _cliente = ClienteAcesso.Cliente;

                return _cliente;
            }
        }

        public AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao ClienteConfiguracao
        {
            get
            {
                if (_clienteConfiguracao == null)
                {
                    if (ClienteAcesso.URLHomologacao && (ClienteAcesso.Cliente.ClienteConfiguracaoHomologacao != null))
                        _clienteConfiguracao = ClienteAcesso.Cliente.ClienteConfiguracaoHomologacao;
                    else
                        _clienteConfiguracao = ClienteAcesso.Cliente.ClienteConfiguracao;
                }

                return _clienteConfiguracao;
            }
        }

        public bool IsHomologacao
        {
            get
            {
                if (!_isHomologacao.HasValue)
                    _isHomologacao = !Servicos.Embarcador.Configuracoes.Ambiente.Producao();

                return _isHomologacao.Value;
            }
        }

        public string WebServiceConsultaCTe
        {
            get
            {
                if (_webServiceConsultaCTe == null)
                    _webServiceConsultaCTe = ClienteAcesso.WebServiceConsultaCTe;

                return _webServiceConsultaCTe;
            }
        }

        public string WebServiceOracle
        {
            get
            {
                if (_webServiceOracle == null)
                    _webServiceOracle = ClienteAcesso.WebServiceOracle;

                return _webServiceOracle;
            }
        }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _tipoServicoMultisoftware = ClienteAcesso.TipoServicoMultisoftware;

                return _tipoServicoMultisoftware;
            }
        }

        public string Layout
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _layout = ClienteAcesso.Layout;

                return _layout;
            }
        }

        public string Logo
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _logo = ClienteAcesso.Logo;

                return _logo;
            }
        }

        public string CorFundoUsuario
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _corFundoUsuario = ClienteAcesso.CorFundoUsuario;

                return _corFundoUsuario;
            }
        }

        public string Favicon
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _favicon = ClienteAcesso.Favicon;

                return _favicon;
            }
        }

        public AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso ClienteAcesso
        {
            get
            {
                if (_clienteURLAcesso == null)
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao))
                    {
                        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repositorioClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);

                        _clienteURLAcesso = repositorioClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);
                        _tipoServicoMultisoftware = _clienteURLAcesso.TipoServicoMultisoftware;
                        _layout = _clienteURLAcesso.Layout;
                        _logo = _clienteURLAcesso.Logo;
                        _corFundoUsuario = _clienteURLAcesso.CorFundoUsuario;
                        _favicon = _clienteURLAcesso.Favicon;
                        _cliente = _clienteURLAcesso.Cliente;
                    }
                }

                return _clienteURLAcesso;
            }
        }

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador
        {
            get
            {
                if (_configuracaoEmbarcador == null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                    _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                }

                return this._configuracaoEmbarcador;
            }
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral ConfiguracaoGeral
        {
            get
            {
                if (_configuracaoGeral == null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                    _configuracaoGeral = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                }

                return this._configuracaoGeral;
            }
        }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto ConfiguracaoCanhoto
        {
            get
            {
                if (_configuracaoCanhoto == null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

                    _configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();
                }

                return this._configuracaoCanhoto;
            }
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega ConfiguracaoAgendamentoEntrega
        {
            get
            {
                if (_configuracaoAgendamentoEntrega == null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega repositorioConfiguracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(unitOfWork);
                    _configuracaoAgendamentoEntrega = repositorioConfiguracaoAgendamentoEntrega.BuscarConfiguracaoPadrao();
                }

                return this._configuracaoAgendamentoEntrega;
            }
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ConfiguracaoGeralCarga
        {
            get
            {
                if (_configuracaoGeralCarga == null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                    _configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                }

                return this._configuracaoGeralCarga;
            }
        }

        public Dominio.Entidades.Usuario Usuario
        {
            get
            {
#if DEBUG
                if (this._usuario == null)
                {
                    int codigoUsuarioDebugPadrao = 17048; //Padrão Multisoftware
                    int codigoUsuario = _conexao.ObterUsuarioDebugPadrao(codigoUsuarioDebugPadrao);
                    int codigoUsuarioContexto = HttpContext.User.GetCodigoUsuario().ToInt();

                    if (codigoUsuarioContexto == 0 || codigoUsuarioContexto != codigoUsuario)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                        Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                        this._usuario = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                        base.SignIn(this._usuario);

                        return this._usuario;
                    }
                }
#endif

                if (this._usuario == null && HttpContext.User.GetCodigoUsuario() != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                    this._usuario = repositorioUsuario.BuscarPorCodigo(int.Parse(HttpContext.User.GetCodigoUsuario()));
                    string internalUser = HttpContext.User.GetInternalUser();

                    if (!string.IsNullOrWhiteSpace(internalUser))
                        _usuario.UsuarioInterno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.UsuarioInterno>(internalUser);
                }

                return this._usuario;
            }
            set
            {
                this._usuario = value;
            }
        }

        public Dominio.Entidades.Empresa Empresa
        {
            get
            {
                if (this.Usuario != null && this._empresa == null)
                    this._empresa = this.Usuario.Empresa;

                return _empresa;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado
        {
            get
            {
                if (this._auditado == null)
                {
                    this._auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    this._auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario;
                    this._auditado.Usuario = Usuario;
                    this._auditado.Empresa = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) ? Empresa : null;
                    this._auditado.Texto = "";
                }

                return _auditado;
            }
        }

        #endregion Propriedades

        #region Métodos Privados

        private void SetarCookieFileDownload()
        {
            Response.Cookies.Append("fileDownload", "true", new CookieOptions { Path = "/" });
        }

        #endregion Métodos Privados

        #region Métodos Protegidos

        protected FileStreamResult Arquivo(System.IO.Stream fileStream, string contentType, string fileDownloadName)
        {
            this.SetarCookieFileDownload();

            return File(fileStream, contentType, fileDownloadName);
        }

        protected FileContentResult Arquivo(byte[] bytes, string contentType, string fileDownloadName)
        {
            this.SetarCookieFileDownload();

            return File(bytes, contentType, fileDownloadName);
        }

        protected bool ExcessaoPorPossuirDependeciasNoBanco(Exception ex)
        {
            Servicos.Log.TratarErro(ex);
            if (ex.Message == "O registro possui dependências e não pode ser excluido.")
            {
                return true;
            }
            else if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
            {
                System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                if (excecao.Number == 547)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool IsCompartilharAcessoEntreGrupoPessoas()
        {
            return Usuario.ClienteFornecedor?.CompartilharAcessoEntreGrupoPessoas ?? false;
        }

        protected bool IsLayoutClienteAtivo(Repositorio.UnitOfWork unitOfWork)
        {
            return TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NovoLayoutPortalFornecedor.Value;
        }

        protected bool IsLayoutCabotagem(Repositorio.UnitOfWork unitOfWork)
        {
            return Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NovoLayoutCabotagem.Value && (ConfiguracaoEmbarcador?.UtilizaEmissaoMultimodal ?? false);
        }

        protected bool IsVisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas()
        {
            return Usuario.ClienteFornecedor?.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas ?? false;
        }

        protected List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> ObterPermissoesPersonalizadas(params string[] paginas)
        {
            Modulos controllerModulos = new Modulos(_conexao);
            List<Controllers.CacheFormulario> formulariosEmCache = controllerModulos.RetornarFormulariosEmCache();
            List<Controllers.CacheFormulario> cacheFormularios = (from obj in formulariosEmCache where paginas.Contains(obj.CaminhoFormulario) select obj).ToList();
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = new List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada>();

            if (cacheFormularios.Count > 0)
            {
                foreach (Controllers.CacheFormulario cacheFormulario in cacheFormularios)
                {
                    bool moduloLiberado = this.Usuario.ModulosLiberados.Contains(cacheFormulario.CacheModulo.CodigoModulo);

                    if (!moduloLiberado)
                        moduloLiberado = controllerModulos.VerificarModulosPaiLiberadoRecursivamente(cacheFormulario.CacheModulo, this.Usuario.ModulosLiberados.ToList());

                    if (moduloLiberado || this.Usuario.UsuarioAdministrador)
                    {
                        foreach (AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada permissao in cacheFormulario.PermissoesPersonalizadas)
                        {
                            if (!permissoesPersonalizadas.Contains(permissao))
                                permissoesPersonalizadas.Add(permissao);
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formularioFuncionario = (from obj in this.Usuario.FormulariosLiberados where obj.CodigoFormulario == cacheFormulario.CodigoFormulario select obj).FirstOrDefault();
                        if (formularioFuncionario != null)
                        {
                            foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada funcionarioFormularioPermissaoPersonalizada in formularioFuncionario.FuncionarioFormularioPermissaoesPersonalizadas)
                            {
                                if (!permissoesPersonalizadas.Contains((AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada)funcionarioFormularioPermissaoPersonalizada.CodigoPermissao))
                                    permissoesPersonalizadas.Add((AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada)funcionarioFormularioPermissaoPersonalizada.CodigoPermissao);
                            }
                        }
                    }
                }
            }

            return permissoesPersonalizadas;
        }

        protected void SalvarFiltroPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa, string dados, Repositorio.UnitOfWork unitOfWork)
        {
            SalvarFiltroPesquisa(0, null, codigoFiltroPesquisa, dados, true, unitOfWork);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoRegistroFiltroPesquisa">Se existir.. e atualizando...</param>
        /// <param name="descricao">Descrição do cadastro de filtro..</param>
        /// <param name="codigoFiltroPesquisa">Código do filtro de pesquisa</param>
        /// <param name="dados">JSON com os dados do filtro de pesquisa...</param>
        /// <param name="apenasUmRegistroPorFiltroPesquisa">Se é apenas um registro por tipo de filtro e usuário....</param>
        /// <param name="unitOfWork"></param>
        protected void SalvarFiltroPesquisa(int codigoRegistroFiltroPesquisa, string descricao, Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa, string dados, bool apenasUmRegistroPorFiltroPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            if (dados == string.Empty)
                return;

            Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
            Repositorio.FiltroPesquisaModelo repositorioFiltroPesquisaModelo = new Repositorio.FiltroPesquisaModelo(unitOfWork);

            Dominio.Entidades.Global.FiltroPesquisa filtroPesquisa = null;

            if (codigoRegistroFiltroPesquisa > 0)
                filtroPesquisa = repositorioFiltroPesquisa.BuscarPorCodigo(codigoRegistroFiltroPesquisa, false);
            else if (apenasUmRegistroPorFiltroPesquisa)
                filtroPesquisa = repositorioFiltroPesquisa.BuscarFiltroPesquisa(codigoFiltroPesquisa, this.Usuario.Codigo);

            if (filtroPesquisa != null)
            {
                filtroPesquisa.Dados = dados;

                if (!string.IsNullOrEmpty(descricao) && filtroPesquisa.Modelo != null)
                {
                    filtroPesquisa.Modelo.Descricao = descricao;
                    repositorioFiltroPesquisaModelo.Atualizar(filtroPesquisa.Modelo);
                }

                repositorioFiltroPesquisa.Atualizar(filtroPesquisa);
            }
            else
            {
                Dominio.Entidades.Global.FiltroPesquisaModelo modeloFiltroPesquisa = new Dominio.Entidades.Global.FiltroPesquisaModelo()
                {
                    Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "Modelo padrão do usuário",
                    ModeloPadrao = !string.IsNullOrWhiteSpace(descricao) ? false : true,
                    ModeloExclusivoUsuario = !string.IsNullOrWhiteSpace(descricao) ? false : true
                };

                repositorioFiltroPesquisaModelo.Inserir(modeloFiltroPesquisa);

                filtroPesquisa = new Dominio.Entidades.Global.FiltroPesquisa
                {
                    Dados = dados,
                    CodigoFiltroPesquisa = codigoFiltroPesquisa,
                    Usuario = this.Usuario,
                    Modelo = modeloFiltroPesquisa
                };

                repositorioFiltroPesquisa.Inserir(filtroPesquisa);
            }
        }

        protected bool UsuarioPossuiPermissao(string pagina)
        {
            if (this.Usuario.UsuarioAdministrador)
                return true;

            Controllers.Modulos controllerModulo = new Controllers.Modulos(_conexao);
            List<Controllers.CacheFormulario> formulariosEmCache = controllerModulo.RetornarFormulariosEmCache();
            Controllers.CacheFormulario formulario = (from o in formulariosEmCache where o != null && o.CaminhoFormulario == pagina select o).FirstOrDefault();

            if (formulario == null)
                return false;

            if (this.Usuario.ModulosLiberados.Contains(formulario.CacheModulo.CodigoModulo))
                return true;

            if (controllerModulo.VerificarModulosPaiLiberadoRecursivamente(formulario.CacheModulo, this.Usuario.ModulosLiberados.ToList()))
                return true;

            Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formularioFuncionario = (from o in this.Usuario.FormulariosLiberados where o.CodigoFormulario == formulario.CodigoFormulario select o).FirstOrDefault();

            return ((formularioFuncionario != null) && !formularioFuncionario.SomenteLeitura);
        }

        protected bool ValidarPeriodoRelatorio(DateTime dataInicial, DateTime dataFinal, out string mensagem)
        {
            if (ConfiguracaoEmbarcador.QuantidadeMaximaDiasRelatorios > 0 && (dataFinal - dataInicial).Days > ConfiguracaoEmbarcador.QuantidadeMaximaDiasRelatorios)
            {
                mensagem = $"O período não pode exceder {ConfiguracaoEmbarcador.QuantidadeMaximaDiasRelatorios} dias!";
                return false;
            }

            mensagem = null;
            return true;
        }

        protected bool UsuarioPossuiPermissaoAlterarConfiguracoesSistema()
        {
            if (this.Usuario.UsuarioMultisoftware || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                return true;

            if ((this.ClienteAcesso?.URLHomologacao ?? false) && this.Usuario.UsuarioAtendimento)
                return true;

            return this.Usuario.UsuarioInterno?.Administrador ?? false;
        }

        #endregion Métodos Protegidos

        #region Métodos Protegidos - Operador Logística

        protected List<double> ObterListaCnpjCpfClientePermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Clientes select cliente.Cliente.CPF_CNPJ).ToList();

            return new List<double>();
        }
        protected async Task<List<double>> ObterListaCnpjCpfClientePermitidosOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellation)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellation);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Clientes select cliente.Cliente.CPF_CNPJ).ToList();

            return new List<double>();
        }

        protected List<string> ObterListaCnpjCpfSemFormatoClientePermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Clientes select cliente.Cliente.CPF_CNPJ_SemFormato).ToList();

            return new List<string>();
        }

        protected List<double> ObterListaCnpjCpfTomadorPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Tomadores select cliente.CPF_CNPJ).ToList();

            return new List<double>();
        }

        protected List<double> ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Recebedores select cliente.CPF_CNPJ).ToList();

            return new List<double>();
        }

        protected async Task<List<double>> ObterListaCnpjCpfRecebedorPermitidosOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellationToken);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Recebedores select cliente.CPF_CNPJ).ToList();

            return new List<double>();
        }

        protected List<double> ObterListaCnpjCpfExpedidorPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from cliente in operadorLogistica.Expedidores select cliente.CPF_CNPJ).ToList();

            return new List<double>();
        }

        protected List<int> ObterListaCodigoCentroCarregamentoPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from centroCarregamento in operadorLogistica.CentrosCarregamento select centroCarregamento.Codigo).ToList();

            return new List<int>();
        }

        protected List<int> ObterListaCodigoCentroDescarregamentoPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from centroDescarregamento in operadorLogistica.CentrosDescarregamento select centroDescarregamento.Codigo).ToList();

            return new List<int>();
        }

        protected List<int> ObterListaCodigoFilialPermitidasOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
            return ObterListaCodigoFilialPermitidasOperadorLogistica(operadorLogistica);
        }

        protected List<int> ObterListaCodigoFilialPermitidasOperadorLogistica(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica)
        {
            if (operadorLogistica?.Ativo ?? false)
            {
                List<int> listaCodigoFiliais = (from filial in operadorLogistica.Filiais select filial.Filial.Codigo).Distinct().ToList();

                if (operadorLogistica.RegraRecebedorSeraSobrepostaNasDemais)
                    listaCodigoFiliais.Add(-1);

                return listaCodigoFiliais;
            }

            return new List<int>();
        }

        protected async Task<List<int>> ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellationToken);

            if (operadorLogistica?.Ativo ?? false)
            {
                List<int> listaCodigoFiliais = (from filial in operadorLogistica.Filiais select filial.Filial.Codigo).Distinct().ToList();

                if (operadorLogistica.RegraRecebedorSeraSobrepostaNasDemais)
                    listaCodigoFiliais.Add(-1);

                return listaCodigoFiliais;
            }

            return new List<int>();
        }

        protected List<int> ObterListaCodigoFilialVendaPermitidasOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from filial in operadorLogistica.FiliaisVenda select filial.Codigo).ToList();

            return ObterListaCodigoFilialVendaPermitidasOperadorLogistica(operadorLogistica);
        }

        protected List<int> ObterListaCodigoFilialVendaPermitidasOperadorLogistica(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica)
        {
            if (operadorLogistica?.Ativo ?? false)
                return (from filial in operadorLogistica.FiliaisVenda select filial.Codigo).ToList();

            return new List<int>();
        }

        protected async Task<List<int>> ObterListaCodigoFilialVendaPermitidasOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellationToken);

            if (operadorLogistica?.Ativo ?? false)
                return (from filial in operadorLogistica.FiliaisVenda select filial.Codigo).ToList();

            return new List<int>();
        }

        protected List<int> ObterListaCodigoTipoCargaPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
            return ObterListaCodigoTipoCargaPermitidosOperadorLogistica(operadorLogistica);
        }

        protected List<int> ObterListaCodigoTipoCargaPermitidosOperadorLogistica(Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica)
        {
            if (operadorLogistica?.Ativo ?? false)
            {
                List<int> listaCodigoTipoCarga = (from tipoCarga in operadorLogistica.TiposCarga select tipoCarga.TipoDeCarga.Codigo).ToList();

                if (listaCodigoTipoCarga.Count > 0)
                    listaCodigoTipoCarga.Add(-1);

                return listaCodigoTipoCarga;
            }

            return new List<int>();
        }

        protected async Task<List<int>> ObterListaCodigoTipoCargaPermitidosOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellationToken);

            if (operadorLogistica?.Ativo ?? false)
            {
                List<int> listaCodigoTipoCarga = (from tipoCarga in operadorLogistica.TiposCarga select tipoCarga.TipoDeCarga.Codigo).ToList();

                if (listaCodigoTipoCarga.Count > 0)
                    listaCodigoTipoCarga.Add(-1);

                return listaCodigoTipoCarga;
            }

            return new List<int>();
        }

        protected List<int> ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if ((operadorLogistica?.Ativo ?? false) && operadorLogistica.PossuiFiltroTipoOperacao)
            {
                List<int> listaCodigoTipoOperacao = (from tipoOperacao in operadorLogistica.TipoOperacoes select tipoOperacao.Codigo).ToList();

                if ((listaCodigoTipoOperacao.Count > 0) && operadorLogistica.VisualizaCargasSemTipoOperacao)
                    listaCodigoTipoOperacao.Add(-1);

                return listaCodigoTipoOperacao;
            }

            return new List<int>();
        }

        protected async Task<List<int>> ObterListaCodigoTipoOperacaoPermitidosOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellationToken);

            if ((operadorLogistica?.Ativo ?? false) && operadorLogistica.PossuiFiltroTipoOperacao)
            {
                List<int> listaCodigoTipoOperacao = (from tipoOperacao in operadorLogistica.TipoOperacoes select tipoOperacao.Codigo).ToList();

                if ((listaCodigoTipoOperacao.Count > 0) && operadorLogistica.VisualizaCargasSemTipoOperacao)
                    listaCodigoTipoOperacao.Add(-1);

                return listaCodigoTipoOperacao;
            }

            return new List<int>();
        }

        protected List<int> ObterListaCodigoGrupoPessoasPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if ((operadorLogistica?.Ativo ?? false) && operadorLogistica.PossuiFiltroGrupoPessoas)
            {
                List<int> listaCodigoGrupoPessoas = (from gruposPessoas in operadorLogistica.GrupoPessoas select gruposPessoas.Codigo).ToList();

                if ((listaCodigoGrupoPessoas.Count > 0) && operadorLogistica.VisualizaCargasSemGrupoPessoas)
                    listaCodigoGrupoPessoas.Add(-1);

                return listaCodigoGrupoPessoas;
            }

            return new List<int>();
        }

        protected List<int> ObterListaCodigoTransportadorPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from empresa in operadorLogistica.Transportadores select empresa.Codigo).ToList();

            return new List<int>();
        }

        protected List<int> ObterListaCodigoVendedoresPermitidosOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

            if (operadorLogistica?.Ativo ?? false)
                return (from vendedor in operadorLogistica.Vendedores select vendedor.Codigo).ToList();

            return new List<int>();
        }

        protected string ObterVendedor(string usuarioAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor repositorioPortalMultiCliforVendedor = new Repositorio.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor(unitOfWork);
            return repositorioPortalMultiCliforVendedor.BuscarPorUsuarioAcessoPortal(usuarioAcesso)?.Vendedor ?? string.Empty;
        }

        protected Dominio.Entidades.Embarcador.Operacional.OperadorLogistica ObterOperadorLogistica(Repositorio.UnitOfWork unitOfWork)
        {
            if (_operadorLogistica == null)
                _operadorLogistica = (Usuario != null) ? new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork).BuscarPorUsuario(Usuario.Codigo) : null;

            return _operadorLogistica;
        }
        protected async Task<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica> ObterOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork)
        {
            if (_operadorLogistica == null)
                _operadorLogistica = (Usuario != null) ? await new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork).BuscarPorUsuarioAsync(Usuario.Codigo) : null;

            return _operadorLogistica;
        }

        protected async Task<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica> ObterOperadorLogisticaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (_operadorLogistica == null)
                _operadorLogistica = (Usuario != null) ? await new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork, cancellationToken).BuscarPorUsuarioAsync(Usuario.Codigo) : null;

            return _operadorLogistica;
        }

        #endregion Métodos Protegidos - Operador Logística

        #region Métodos Protegidos - Operador Canhoto

        protected List<int> ObterListaCodigoFilialPermitidasOperadorCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto = ObterOperadorCanhoto(unitOfWork);

            if (operadorCanhoto?.Ativo ?? false)
                return (from filial in operadorCanhoto.Filiais select filial.Codigo).ToList();

            return new List<int>();
        }

        protected List<int> ObterListaCodigoTipoCargaPermitidosOperadorCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto = ObterOperadorCanhoto(unitOfWork);

            if (operadorCanhoto?.Ativo ?? false)
            {
                List<int> listaCodigoTipoCarga = (from tipoCarga in operadorCanhoto.TiposCarga select tipoCarga.TipoDeCarga.Codigo).ToList();

                if (listaCodigoTipoCarga.Count > 0)
                    listaCodigoTipoCarga.Add(-1);

                return listaCodigoTipoCarga;
            }

            return new List<int>();
        }

        protected List<int> ObterListaCodigoTipoOperacaoPermitidosOperadorCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto operadorCanhoto = ObterOperadorCanhoto(unitOfWork);

            if ((operadorCanhoto?.Ativo ?? false) && operadorCanhoto.PossuiFiltroTipoOperacao)
            {
                List<int> listaCodigoTipoOperacao = (from tipoOperacao in operadorCanhoto.TiposOperacao select tipoOperacao.Codigo).ToList();

                if ((listaCodigoTipoOperacao.Count > 0) && operadorCanhoto.VisualizaCargasSemTipoOperacao)
                    listaCodigoTipoOperacao.Add(-1);

                return listaCodigoTipoOperacao;
            }

            return new List<int>();
        }

        protected Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto ObterOperadorCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            if (_operadorCanhoto == null)
                _operadorCanhoto = (Usuario != null) ? new Repositorio.Embarcador.Operacional.Canhoto.OperadorCanhoto(unitOfWork).BuscarPorUsuario(Usuario.Codigo) : null;

            return _operadorCanhoto;
        }

        #endregion Métodos Protegidos - Operador Canhoto

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFiltroPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("codigoFiltroPesquisa");
                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                Dominio.Entidades.Global.FiltroPesquisa filtroPesquisa = repositorioFiltroPesquisa.BuscarFiltroPesquisa(codigoFiltroPesquisa, this.Usuario.Codigo);

                return new JsonpResult(filtroPesquisa?.Dados ?? string.Empty);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter filtros de pesquisa");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public CookieOptions GerarCookieOptions(int expireDays = 1)
        {
            return new CookieOptions
            {
                Expires = DateTime.Now.AddDays(expireDays),
                HttpOnly = true,
                Secure = true
            };
        }

        #endregion Métodos Públicos
    }
}
