using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/Motorista")]
    public class MotoristaController : BaseController
    {
        #region Construtores

        public MotoristaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                DateTime? dataEmissaoRG = Request.GetNullableDateTimeParam("DataEmissaoRG");
                DateTime? dataPrimeiraHabilitacao = Request.GetNullableDateTimeParam("DataPrimeiraHabilitacao");
                DateTime? dataFimPeriodoExperiencia = Request.GetNullableDateTimeParam("DataFimPeriodoExperiencia");

                int codigoEmpresa, codigoPlanoConta, codigoBanco = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("PlanoAcertoViagem"), out codigoPlanoConta);
                int.TryParse(Request.Params("Banco"), out codigoBanco);
                int.TryParse(Request.Params("PerfilAcessoMobile"), out int codigoPerfilAcessoMobile);

                bool ativo, usuarioUsaMobile, bloqueado, naoBloquearAcessoSimultaneo;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("Bloqueado"), out bloqueado);
                bool.TryParse(Request.Params("UsuarioMobile"), out usuarioUsaMobile);
                bool.TryParse(Request.Params("NaoBloquearAcessoSimultaneo"), out naoBloquearAcessoSimultaneo);

                bool naoGeraComissaoAcerto = false, ativarFichaMotorista = false;
                bool.TryParse(Request.Params("NaoGeraComissaoAcerto"), out naoGeraComissaoAcerto);
                bool.TryParse(Request.Params("AtivarFichaMotorista"), out ativarFichaMotorista);
                double clienteTerceiro;
                double.TryParse(Request.Params("ClienteTerceiro"), out clienteTerceiro);

                TipoMotorista tipoMotorista;
                Enum.TryParse(Request.Params("TipoMotorista"), out tipoMotorista);

                string codigoIntegracaoContabilidade = Request.Params("CodigoIntegracaoContabilidade");
                string categoriaHabilitacao = Request.Params("CategoriaHabilitacao");
                string celular = Request.Params("Celular");

                DateTime? dataValidadeCNH = Request.GetNullableDateTimeParam("DataValidadeCNH");
                DateTime? dataEmissaoCNH = Request.GetNullableDateTimeParam("DataEmissaoCNH");

                TipoEndereco tipoEndereco = (TipoEndereco)int.Parse(Request.Params("TipoEndereco"));
                TipoEmail tipoEmail = (TipoEmail)int.Parse(Request.Params("TipoEmail"));
                Dominio.Entidades.Usuario motorista = new Dominio.Entidades.Usuario();
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioConfiguracaoMotorista.BuscarConfiguracaoPadrao();

                bool motoristaEstrangeiro = Request.GetBoolParam("MotoristaEstrangeiro");
                string cpfMotorista = motoristaEstrangeiro ? Request.GetStringParam("CodigoMotoristaEstrangeiro") : Utilidades.String.OnlyNumbers(Request.Params("CPF"));

                bool duplicarCadastro = Request.GetBoolParam("DuplicarCadastro");

                if (duplicarCadastro && repMotorista.ExisteMotoristaPorCPFEEmpresa(cpfMotorista, codigoEmpresa))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.EsseMotoristaJaPossuiVinculoComEsseTransportador);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    if (Usuario.ClienteTerceiro != null)
                        motorista.Empresa = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                    if (motorista.Empresa == null)
                        return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.TransportadorTerceiroNaoCadastradoComoEmpresa);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    motorista.Empresa = Usuario.Empresa;
                else if (codigoEmpresa > 0)
                    motorista.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (motorista.Empresa != null && motorista.Empresa.Matriz != null && motorista.Empresa.Matriz.Count > 0)
                        motorista.Empresa = motorista.Empresa.Matriz.FirstOrDefault();
                }

                if (ativo)
                    motorista.Status = "A";
                else
                    motorista.Status = "I";

                TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Motorista");
                bool permiteBloquearMotorista = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermiteBloquearMotorista) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

                motorista.Bloqueado = permiteBloquearMotorista ? bloqueado : motorista.Bloqueado;
                motorista.DataHabilitacao = dataEmissaoCNH;
                motorista.NaoBloquearAcessoSimultaneo = naoBloquearAcessoSimultaneo;
                motorista.DataPrimeiraHabilitacao = dataPrimeiraHabilitacao;
                motorista.DataEmissaoRG = dataEmissaoRG;
                motorista.DataFimPeriodoExperiencia = dataFimPeriodoExperiencia;
                motorista.Banco = codigoBanco > 0 ? repBanco.BuscarPorCodigo(codigoBanco) : null;
                motorista.Agencia = Request.Params("Agencia");
                motorista.DigitoAgencia = Request.Params("Digito");
                motorista.NumeroConta = Request.Params("NumeroConta");
                motorista.PendenteIntegracaoEmbarcador = true;
                motorista.TipoContaBanco = tipoConta;
                motorista.ObservacaoConta = Request.Params("ObservacaoConta");

                motorista.PISAdministrativo = Request.Params("PISAdministrativo");
                motorista.Cargo = Request.Params("Cargo");
                motorista.CBO = Request.Params("CBO");
                motorista.NumeroMatricula = Request.Params("NumeroMatricula");
                motorista.NumeroProntuario = Request.Params("NumeroProntuario");

                motorista.Tipo = "M";
                motorista.Nome = Request.Params("Nome");
                motorista.Apelido = Request.Params("Apelido");
                motorista.MotoristaEstrangeiro = motoristaEstrangeiro;
                motorista.CPF = cpfMotorista;
                motorista.RG = Request.Params("RG");
                motorista.EstadoRG = repEstado.BuscarPorSigla(Request.Params("EstadoRG"));
                motorista.UFEmissaoCNH = repEstado.BuscarPorSigla(Request.Params("UFEmissaoCNH"));
                motorista.OrgaoEmissorRG = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG>("EmissorRG");
                motorista.Telefone = Request.Params("Telefone");
                motorista.Celular = celular;
                motorista.NumeroCartao = Request.Params("NumeroCartao");
                motorista.TipoMotorista = tipoMotorista;
                motorista.CodigoIntegracao = Request.Params("CodigoIntegracao");
                motorista.NaoGeraComissaoAcerto = naoGeraComissaoAcerto;
                motorista.AtivarFichaMotorista = ativarFichaMotorista;
                motorista.CodigoIntegracaoContabilidade = codigoIntegracaoContabilidade;
                motorista.PIS = Request.Params("PISPASEP");

                if (configuracaoMotorista.NaoPermitirRealizarCadastroMotoristaBloqueado)
                {
                    if (repMotorista.ExisteMotoristaMesmoCpfBloqueado(motorista.CPF))
                        throw new ControllerException("Não é possivel criar novo cadastro para um motorista bloqueado.");
                }

                int.TryParse(Request.Params("Filial"), out int filial);

                if (filial > 0)
                    motorista.Filial = repFilial.BuscarPorCodigo(filial);

                if (clienteTerceiro > 0)
                    motorista.ClienteTerceiro = repCliente.BuscarPorCPFCNPJ(clienteTerceiro);
                else
                    motorista.ClienteTerceiro = null;

                motorista.DataVencimentoHabilitacao = dataValidadeCNH;

                if (!string.IsNullOrWhiteSpace(Request.Params("DataValidadeLiberacaoSeguradora")))
                {
                    DateTime dataValidadeLiberacaoSeguradora;
                    DateTime.TryParseExact(Request.Params("DataValidadeLiberacaoSeguradora"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidadeLiberacaoSeguradora);
                    motorista.DataValidadeLiberacaoSeguradora = dataValidadeLiberacaoSeguradora;
                }


                motorista.Bairro = Request.Params("Bairro");
                motorista.CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
                motorista.Complemento = Request.Params("Complemento");
                motorista.NumeroEndereco = Request.Params("NumeroEndereco");
                motorista.TipoLogradouro = (TipoLogradouro)int.Parse(Request.Params("TipoLogradouro"));
                motorista.TipoResidencia = (TipoResidencia)int.Parse(Request.Params("TipoResidencia"));
                motorista.EnderecoDigitado = bool.Parse(Request.Params("EnderecoDigitado"));
                motorista.Latitude = Request.Params("Latitude");
                motorista.Longitude = Request.Params("Longitude");
                motorista.TipoEmail = tipoEmail;
                motorista.TipoEndereco = tipoEndereco;
                motorista.NumeroCTPS = Request.Params("NumeroCTPS");
                motorista.SerieCTPS = Request.Params("SerieCTPS");
                if (codigoPlanoConta > 0)
                    motorista.PlanoAcertoViagem = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);

                DateTime.TryParseExact(Request.Params("DataNascimento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataNascimento);
                DateTime.TryParseExact(Request.Params("DataAdmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAdmissao);
                DateTime.TryParseExact(Request.Params("DataDemissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataDemissao);
                DateTime.TryParseExact(Request.Params("DataVencimentoMoop"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimentoMoop);

                if (dataNascimento > DateTime.MinValue)
                    motorista.DataNascimento = dataNascimento;
                else
                    motorista.DataNascimento = null;
                if (dataAdmissao > DateTime.MinValue)
                    motorista.DataAdmissao = dataAdmissao;
                else
                    motorista.DataAdmissao = null;
                if (dataDemissao > DateTime.MinValue)
                    motorista.DataDemissao = dataDemissao;
                else
                    motorista.DataDemissao = null;
                if (dataVencimentoMoop > DateTime.MinValue)
                    motorista.DataVencimentoMoop = dataVencimentoMoop;
                else
                    motorista.DataVencimentoMoop = null;

                motorista.NumeroHabilitacao = Request.Params("NumeroHabilitacao");
                motorista.NumeroRegistroHabilitacao = Request.Params("NumeroRegistroHabilitacao");
                motorista.Categoria = categoriaHabilitacao;

                if (!string.IsNullOrWhiteSpace(Request.Params("Localidade")) && Request.Params("Localidade") != "0")
                    motorista.Localidade = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Localidade")) };
                motorista.Endereco = Request.Params("Endereco");
                motorista.Email = Request.Params("Email");
                motorista.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
                motorista.UsuarioAdministradorMobile = Request.GetBoolParam("UsuarioAdministradorMobile");
                motorista.PerfilAcessoMobile = codigoPerfilAcessoMobile > 0 ? repPerfilAcessoMobile.BuscarPorCodigo(codigoPerfilAcessoMobile) : null;
                motorista.DataValidadeGR = Request.GetNullableDateTimeParam("DataValidadeGR");
                motorista.OrdenarCargasMobileCrescente = Request.GetBoolParam("OrdenarCargasMobileCrescente");

                dynamic dynTransportadoras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Transportadoras"));
                if (dynTransportadoras.Count > 0)
                {
                    motorista.Empresas = new List<Dominio.Entidades.Empresa>();
                    foreach (var dynTransportador in dynTransportadoras)
                    {
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo((int)dynTransportador.Codigo);
                        motorista.Empresas.Add(empresa);
                    }
                }

                if (ativo)
                {
                    if (!ValidarCamposReferenteCIOT(motorista, out string erroValidacaoCIOT))
                        return new JsonpResult(true, false, erroValidacaoCIOT);

                    if (!Utilidades.Validate.ValidarPISPASEP(motorista.PIS))
                        return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.PISPASEPInformadoNaoValido);
                }

                unitOfWork.Start();

                string novaSenhaMobile = ClienteExigeContraSenha() ? Request.GetStringParam("ContraSenhaMobile") : null;

                PreencherDadosAdicionaisMotorista(motorista, unitOfWork);
                SalvarLocaisCarregamentoAutorizados(motorista, unitOfWork);

                Dominio.Entidades.Usuario motoristaExiste = repMotorista.BuscarMotoristaPorCPFVarrendoFiliais(motorista.Empresa?.Codigo ?? 0, motorista.CPF);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    motoristaExiste = repMotorista.BuscarPorCPF(motorista.CPF);

                if (motoristaExiste != null && motoristaExiste.Tipo == "M")
                    throw new ControllerException(Localization.Resources.Transportadores.Motorista.JaExisteUmMotoristaCadastradoComMesmoCPF);
                else if (motoristaExiste != null && motoristaExiste.Tipo != "M")
                {
                    motorista = repMotorista.BuscarPorCodigo(motoristaExiste.Codigo, true);
                    motorista.Tipo = "M";

                    PreencherDadosMotorista(motorista, unitOfWork);

                    if (usuarioUsaMobile)
                    {
                        string retorno = Servicos.Usuario.ConfigurarUsuarioMobile(ref motorista, novaSenhaMobile, ClienteAcesso, unitOfWorkAdmin);
                        if (!string.IsNullOrWhiteSpace(retorno))
                            throw new ControllerException(retorno);
                    }

                    repMotorista.Atualizar(motorista, Auditado);

                    SalvarLicencas(motorista, unitOfWork);
                    SalvarLiberacoesGR(motorista, unitOfWork);
                    SalvarDadosBancarios(motorista, unitOfWork);
                    SalvarContatos(motorista, unitOfWork);
                    SalvarEPIs(motorista, unitOfWork);
                    Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracoes(motorista, unitOfWork);

                    motorista.ModulosLiberadosMobile.Clear();
                    var jModulosUsuarioMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuarioMobile"));
                    foreach (var jModulo in jModulosUsuarioMobile)
                    {
                        motorista.ModulosLiberadosMobile.Add((int)jModulo.CodigoModulo);
                    }
                }
                else
                {
                    if (usuarioUsaMobile)
                    {
                        string retorno = Servicos.Usuario.ConfigurarUsuarioMobile(ref motorista, novaSenhaMobile, ClienteAcesso, unitOfWorkAdmin);
                        if (!string.IsNullOrWhiteSpace(retorno))
                            throw new ControllerException(retorno);
                    }


                    repMotorista.Inserir(motorista);

                    SalvarLicencas(motorista, unitOfWork);
                    SalvarLiberacoesGR(motorista, unitOfWork);
                    SalvarDadosBancarios(motorista, unitOfWork);
                    SalvarContatos(motorista, unitOfWork);
                    SalvarEPIs(motorista, unitOfWork);
                    Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracoes(motorista, unitOfWork);

                    motorista.ModulosLiberadosMobile = new List<int>();
                    var jModulosUsuarioMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuarioMobile"));
                    foreach (var jModulo in jModulosUsuarioMobile)
                    {
                        motorista.ModulosLiberadosMobile.Add((int)jModulo.CodigoModulo);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, Localization.Resources.Transportadores.Motorista.AdicionouMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.MotoristaInvalidoOneTrust)
            {
                unitOfWork.Rollback();
                return new JsonpResult(new
                {
                    ExibirModalLGPD = true
                }, true, excecao.Message);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarTodosMotoristaMobile()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<int> motoristas = repUsuario.BuscarTodosMotoristas();
                foreach (int codigoMotorista in motoristas)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoMotorista);
                    Dominio.Entidades.Usuario motorista = usuario;
                    Servicos.Usuario.ConfigurarUsuarioMobile(ref motorista, null, ClienteAcesso, unitOfWorkAdmin);
                    repUsuario.Atualizar(motorista);

                    unitOfWork.CommitChanges();
                    unitOfWork.FlushAndClear();
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoHabilitarMotoristaParaUtilizarMobile);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFoto()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigo);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotoristaNaoEncontrado);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoFoto");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.NenhumaFotoSelecionadaParaAdicionar);

                Servicos.DTO.CustomFile arquivoFoto = arquivos[0];
                string extensaoArquivo = System.IO.Path.GetExtension(arquivoFoto.FileName).ToLower();
                string caminho = ObterCaminhoArquivoFoto(unitOfWork);

                Utilidades.IO.FileStorageService.Storage.CreateIfNotExists(caminho);

                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                arquivoFoto.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{codigo}{extensaoArquivo}"));

                Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, Localization.Resources.Transportadores.Motorista.AlterouFotoDoMotorista, unitOfWork);

                return new JsonpResult(new
                {
                    FotoMotorista = ObterFotoBase64(codigo, unitOfWork)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoAdicionarFotoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFotoGaleria()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigo);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotoristaNaoEncontrado);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoFoto");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.NenhumaFotoSelecionadaParaAdicionar);

                Servicos.DTO.CustomFile arquivoFoto = arquivos.FirstOrDefault();
                string extensaoArquivo = System.IO.Path.GetExtension(arquivoFoto.FileName).ToLower();
                string caminho = $"{ObterCaminhoArquivoGaleria(unitOfWork)}";

                arquivoFoto.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{codigo}_{Guid.NewGuid()}{extensaoArquivo}"));

                Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, Localization.Resources.Transportadores.Motorista.AdicionouFotoGaleriaMotorista, unitOfWork);

                return new JsonpResult(new
                {
                    GaleriaMotorista = (from o in ObterFotosGaleria(codigo, unitOfWork)
                                        select new
                                        {
                                            o.NomeArquivo,
                                            o.Base64
                                        }).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoAdicionarFotoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Frota.FrotaCarga frotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(unitOfWork);
                Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(unitOfWork);
                Servicos.Embarcador.Frota.Frota servFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);
                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();
                PreencherDadosMotorista(motorista, unitOfWork);
                PreencherDadosAdicionaisMotorista(motorista, unitOfWork);
                SalvarLocaisCarregamentoAutorizados(motorista, unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Motorista");

                bool usuarioUsaMobile, naoBloquearAcessoSimultaneo;
                bool.TryParse(Request.Params("UsuarioMobile"), out usuarioUsaMobile);

                bool.TryParse(Request.Params("NaoBloquearAcessoSimultaneo"), out naoBloquearAcessoSimultaneo);

                if (!Utilidades.Validate.ValidarPISPASEP(motorista.PIS))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.PISPASEPInformadoNaoValido);

                if (!ValidarCamposReferenteCIOT(motorista, out string erroValidacaoCIOT))
                    return new JsonpResult(true, false, erroValidacaoCIOT);

                Dominio.Entidades.Usuario motoristaExiste = repMotorista.BuscarMotoristaPorCPFVarrendoFiliais(motorista.Empresa?.Codigo ?? 0, motorista.CPF);
                if (motoristaExiste != null && motoristaExiste.Codigo != motorista.Codigo && motorista.Status != "I")
                    return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.JaExisteUmMotoristaCadastradoComMesmoCPF);

                unitOfWork.Start();

                if (usuarioUsaMobile)
                {
                    string novaSenhaMobile = ClienteExigeContraSenha() ? Request.GetStringParam("ContraSenhaMobile") : null;

                    string retorno = Servicos.Usuario.ConfigurarUsuarioMobile(ref motorista, novaSenhaMobile, ClienteAcesso, unitOfWorkAdmin);
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ControllerException(retorno);
                }
                else
                {
                    Servicos.Usuario servicoUsuario = new Servicos.Usuario(unitOfWork);

                    string retorno = servicoUsuario.RemoveUsuarioMobile(ref motorista, ClienteAcesso, unitOfWorkAdmin);
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ControllerException(retorno);
                    motorista.CodigoMobile = 0;
                }

                SalvarLicencas(motorista, unitOfWork, true);
                SalvarLiberacoesGR(motorista, unitOfWork);
                SalvarDadosBancarios(motorista, unitOfWork);
                SalvarContatos(motorista, unitOfWork);
                SalvarEPIs(motorista, unitOfWork);

                repMotorista.Atualizar(motorista, Auditado);

                ValidacaoParaBloquearMotoristaOutrosCadastro(unitOfWork, motorista);

                if (motorista.Status == "I")
                {
                    if (!this.Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermitirInativarComSaldo))
                    {
                        if (ConfiguracaoEmbarcador.NaoPermitirInativarFuncionarioComSaldo && (motorista.SaldoAdiantamento != 0 || motorista.SaldoDiaria != 0))
                            throw new ControllerException(Localization.Resources.Transportadores.Motorista.NaoPermitidoInativarMotoristaFuncionarioComSaldo);

                        if (configuracaoMotorista.NaoPermitirInativarMotoristaComSaldoNoExtrato)
                        {
                            decimal saldoMotorista = repMovimentoFinanceiro.BuscarSaldoMotorista(0, motorista.Codigo);
                            if (saldoMotorista != 0m)
                                throw new ControllerException(string.Format(Localization.Resources.Transportadores.Motorista.NaoPossivelInativarMotoristaQuePossuiSaldoEmSeuExtrato, saldoMotorista.ToString("n2")));
                        }
                    }

                    List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarVeiculosPorMotorista(motorista.Codigo);
                    foreach (var veiculo in veiculos)
                    {
                        //veiculo.Initialize();
                        //veiculo.Motorista = null;
                        //repVeiculo.Atualizar(veiculo, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                        Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);
                    }
                    //repVeiculo.RemoverMotoristaVeiculos(motorista.Codigo);

                    if (ConfiguracaoEmbarcador.AoInativarMotoristaTransformarEmFuncionario)
                    {
                        motorista.Tipo = "U";
                        motorista.Status = "A";
                        repMotorista.Atualizar(motorista);
                    }

                    List<Dominio.Entidades.Embarcador.Frota.FrotaCarga> frotasCarga = frotaCarga.BuscarFrotaCargaFuturaPorMotorista(motorista.Codigo, DateTime.Now);
                    foreach (var reg in frotasCarga)
                        servFrota.RemoverFrotaCargaAoInativarMotorista(reg, motorista.Codigo, Auditado);

                    List<Dominio.Entidades.Embarcador.Frota.Frota> frotas = repFrota.BuscarPorMotorista(motorista.Codigo);
                    foreach (var reg in frotas)
                    {
                        if (reg.Motorista.Codigo == motorista.Codigo)
                            reg.Motorista = null;

                        if (reg?.MotoristaAuxiliar?.Codigo == motorista.Codigo)
                            reg.MotoristaAuxiliar = null;

                        repFrota.Atualizar(reg);
                    }
                }

                Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracoes(motorista, unitOfWork);

                motorista.ModulosLiberadosMobile.Clear();
                var jModulosUsuarioMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuarioMobile"));
                foreach (var jModulo in jModulosUsuarioMobile)
                {
                    motorista.ModulosLiberadosMobile.Add((int)jModulo.CodigoModulo);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.MotoristaInvalidoOneTrust)
            {
                unitOfWork.Rollback();
                return new JsonpResult(new
                {
                    ExibirModalLGPD = true
                }, true, excecao.Message);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public async Task<IActionResult> ValidarMotoristaPlanejamentoFrota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.FrotaCarga frotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (motorista != null)
                {
                    if (frotaCarga.ExisteProgramacaoFuturaParaMotorista(motorista.Codigo, DateTime.Now))
                    {
                        return new JsonpResult(new
                        {
                            ExibirConfirmacaoMotoristaPlanejamentoFuturo = true
                        }, true, $"O Motorista {motorista.Descricao} está em um ou mais planejamentos ainda não executados, caso ele seja inativado, o planejamento que está vinculado será desfeito, deseja prosseguir?");

                    }
                }

                return new JsonpResult(new
                {
                    ExibirConfirmacaoMotoristaPlanejamentoFuturo = false
                }, true, "");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarMotoristaSituacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var frotaCargaRepositorio = new Repositorio.Embarcador.Frota.FrotaCarga(unitOfWork);
                var motoristaRepositorio = new Repositorio.Usuario(unitOfWork);
                var motorista = motoristaRepositorio.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repositorioColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repositorioColaboradorLancamento.BuscarPorCodigoMotoristaProximo(motorista.Codigo, DateTime.Now);

                if (colaboradorLancamento != null)
                {
                    string dataIni = colaboradorLancamento?.DataInicial.ToString("dd/MM/yyyy");
                    string dataFim = colaboradorLancamento?.DataFinal.ToString("dd/MM/yyyy");
                    var msgNaoTrabalhando = colaboradorLancamento.ColaboradorSituacao.SituacaoColaborador == SituacaoColaborador.Ferias
                       ? $"O Motorista {motorista.Descricao} se encontra de férias. Período {dataIni} até {dataFim}"
                       : $"O Motorista {motorista.Descricao}  se encontra em {colaboradorLancamento.Descricao}. Período {dataIni} até {dataFim}";

                    return new JsonpResult(new { ExibirConfirmacaoMotoristaSituacao = true }, true, msgNaoTrabalhando);
                }
                else
                {
                    return new JsonpResult(new { ExibirConfirmacaoMotoristaSituacao = false }, true, "");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarVeiculoMotoristaSituacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            try
            {
                var frotaCargaRepositorio = new Repositorio.Embarcador.Frota.FrotaCarga(unitOfWork);
                var motoristaRepositorio = new Repositorio.Usuario(unitOfWork);

                int codigoVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                var motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (motorista != null)
                {
                    Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repositorioColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

                    Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repositorioColaboradorLancamento.BuscarPorCodigoMotoristaProximo(motorista.Codigo, DateTime.Now);

                    if (colaboradorLancamento != null)
                    {
                        string dataIni = colaboradorLancamento?.DataInicial.ToString("dd/MM/yyyy");
                        string dataFim = colaboradorLancamento?.DataFinal.ToString("dd/MM/yyyy");
                        var msgNaoTrabalhando = colaboradorLancamento.ColaboradorSituacao.SituacaoColaborador == SituacaoColaborador.Ferias
                           ? $"O Motorista {motorista.Descricao} se encontra de férias. Período {dataIni} até {dataFim}"
                           : $"O Motorista {motorista.Descricao}  se encontra em {colaboradorLancamento.Descricao}. Período {dataIni} até {dataFim}";

                        return new JsonpResult(new { ExibirConfirmacaoMotoristaSituacao = true }, true, msgNaoTrabalhando);
                    }
                }

                return new JsonpResult(new { ExibirConfirmacaoMotoristaSituacao = false }, true, "");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarFrotaMotoristaSituacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            try
            {
                string numeroFrota = Request.GetNullableStringParam("NumeroFrota");

                List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarPorNumeroDaFrota(numeroFrota);

                if (veiculos?.FirstOrDefault() != null)
                {
                    var motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculos.FirstOrDefault().Codigo);
                    if (motorista != null)
                    {
                        Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repositorioColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

                        Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repositorioColaboradorLancamento.BuscarPorCodigoMotoristaProximo(motorista.Codigo, DateTime.Now);

                        if (colaboradorLancamento != null)
                        {
                            string dataIni = colaboradorLancamento?.DataInicial.ToString("dd/MM/yyyy");
                            string dataFim = colaboradorLancamento?.DataFinal.ToString("dd/MM/yyyy");
                            var msgNaoTrabalhando = colaboradorLancamento.ColaboradorSituacao.SituacaoColaborador == SituacaoColaborador.Ferias
                               ? $"O Motorista {motorista.Descricao} se encontra de férias. Período {dataIni} até {dataFim}"
                               : $"O Motorista {motorista.Descricao}  se encontra em {colaboradorLancamento.Descricao}. Período {dataIni} até {dataFim}";

                            return new JsonpResult(new { ExibirConfirmacaoMotoristaSituacao = true }, true, msgNaoTrabalhando);
                        }
                    }
                }
                return new JsonpResult(new { ExibirConfirmacaoMotoristaSituacao = false }, true, "");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(unitOfWork);

                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> anexos = repFuncionarioAnexo.BuscarPorCodigoUsuario(codigo);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
                Repositorio.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado repMotoristaLocalCarregamentoAutorizado = new Repositorio.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioEPI repositorioMotoristaEPI = new Repositorio.Embarcador.Usuarios.FuncionarioEPI(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioConfiguracaoMotorista.BuscarPrimeiroRegistro();
                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado> motoristaLocaisCarregamentoAutorizados = repMotoristaLocalCarregamentoAutorizado.BuscarPorMotorista(motorista.Codigo);
                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> motoristaEPIs = repositorioMotoristaEPI.BuscarPorUsuario(motorista);

                var entidade = new
                {
                    Ativo = motorista.Status == "A" ? true : false,
                    motorista.Bloqueado,
                    motorista.TipoEndereco,
                    motorista.Codigo,
                    motorista.Status,
                    motorista.Tipo,
                    motorista.Nome,
                    motorista.Apelido,
                    EmpresaOrdenarCargasMobileCrescente = motorista.Empresa?.OrdenarCargasMobileCrescente ?? false,
                    MotoristaEstrangeiro = motorista.MotoristaEstrangeiro && (configuracaoMotorista.PermitirCadastrarMotoristaEstrangeiro || motorista.Empresa?.Tipo == "E"),
                    UsuarioMobile = motorista.CodigoMobile > 0 ? true : false,
                    NaoBloquearAcessoSimultaneo = motorista.NaoBloquearAcessoSimultaneo,
                    CodigoMotoristaEstrangeiro = (motorista.MotoristaEstrangeiro && (configuracaoMotorista.PermitirCadastrarMotoristaEstrangeiro || motorista.Empresa?.Tipo == "E")) ? motorista.CPF : "",
                    CPF = motorista.MotoristaEstrangeiro ? "" : ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? motorista.CPF_Formatado : motorista.CPF,
                    motorista.RG,
                    PISPASEP = motorista.PIS,
                    EstadoRG = new { Codigo = motorista.EstadoRG?.Sigla ?? "", Descricao = motorista.EstadoRG?.Nome ?? "" },
                    UFEmissaoCNH = new { Codigo = motorista.UFEmissaoCNH?.Sigla ?? "", Descricao = motorista.UFEmissaoCNH?.Nome ?? "" },
                    CargoMotorista = new { Codigo = motorista.CargoMotorista?.Codigo, Descricao = motorista.CargoMotorista?.Descricao ?? "" },
                    EmissorRG = motorista.OrgaoEmissorRG,
                    motorista.CodigoIntegracao,
                    DataValidadeLiberacaoSeguradora = motorista.DataValidadeLiberacaoSeguradora.HasValue ? motorista.DataValidadeLiberacaoSeguradora.Value.ToString("dd/MM/yyyy") : "",
                    Telefone = motorista.Telefone_Formatado,
                    DataNascimento = motorista.DataNascimento != null ? motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                    Empresa = motorista.Empresa != null ? new { Codigo = motorista.Empresa.Codigo, Descricao = motorista.Empresa.RazaoSocial, Tipo = motorista.Empresa.Tipo } : null,
                    motorista.NumeroHabilitacao,
                    motorista.NumeroRegistroHabilitacao,
                    RenachCNH = motorista.RenachHabilitacao,
                    CategoriaHabilitacao = motorista.Categoria ?? string.Empty,
                    Localidade = motorista.Localidade != null ? new { Codigo = motorista.Localidade.Codigo, Descricao = motorista.Localidade.DescricaoCidadeEstado } : null,
                    ClienteTerceiro = motorista.ClienteTerceiro != null ? new { Codigo = motorista.ClienteTerceiro.Codigo, Descricao = motorista.ClienteTerceiro.Nome } : null,
                    motorista.Endereco,
                    motorista.Email,
                    DataFechamentoAcerto = motorista.DataFechamentoAcerto != null ? motorista.DataFechamentoAcerto.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataValidadeGR = motorista.DataValidadeGR != null ? motorista.DataValidadeGR.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Transportadoras = (from obj in motorista.Empresas
                                       select new
                                       {
                                           obj.Codigo,
                                           CNPJ = obj.CNPJ_Formatado,
                                           obj.RazaoSocial
                                       }).ToList(),
                    motorista.Bairro,
                    motorista.Complemento,
                    motorista.CEP,
                    motorista.NumeroEndereco,
                    motorista.TipoLogradouro,
                    motorista.TipoResidencia,
                    motorista.EnderecoDigitado,
                    motorista.Latitude,
                    motorista.Longitude,
                    motorista.TipoMotorista,
                    motorista.OrdenarCargasMobileCrescente,
                    PlanoAcertoViagem = motorista.PlanoAcertoViagem != null ? new { Codigo = motorista.PlanoAcertoViagem.Codigo, Descricao = motorista.PlanoAcertoViagem.Descricao } : null,
                    motorista.NaoGeraComissaoAcerto,
                    motorista.AtivarFichaMotorista,
                    Banco = motorista.Banco != null ? new { Codigo = motorista.Banco.Codigo, Descricao = motorista.Banco.Descricao } : null,
                    motorista.Agencia,
                    Digito = motorista.DigitoAgencia,
                    motorista.NumeroConta,
                    TipoConta = motorista.TipoContaBanco,
                    motorista.NumeroCartao,
                    motorista.TipoCartao,
                    ValidarCamposReferenteCIOT = motorista.Empresa?.Configuracao?.TipoIntegradoraCIOT.HasValue ?? false,
                    Filial = new { Codigo = motorista.Filial?.Codigo ?? 0, Descricao = motorista.Filial?.Descricao ?? "" },
                    motorista.CodigoIntegracaoContabilidade,
                    DataValidadeCNH = motorista.DataVencimentoHabilitacao?.ToString("dd/MM/yyyy"),
                    GridMotoristaLicencas = (from obj in motorista.Licencas
                                             select new
                                             {
                                                 obj.Codigo,
                                                 obj.Descricao,
                                                 obj.Numero,
                                                 DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                                 DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                                 FormaAlerta = "[" + string.Join(",", obj.FormasAlerta.Select(o => (int)o).ToList()) + "]",
                                                 obj.Status,
                                                 DescricaoLicenca = obj.Licenca != null ? obj.Licenca.Descricao : string.Empty,
                                                 CodigoLicenca = obj.Licenca != null ? obj.Licenca.Codigo : 0,
                                                 obj.BloquearCriacaoPedidoLicencaVencida,
                                                 obj.BloquearCriacaoPlanejamentoPedidoLicencaVencida,
                                                 obj.ConfirmadaLeituraPendencia
                                             }).ToList(),
                    FotoMotorista = configuracaoMotorista.MotoristaUsarFotoDoApp ? "" : ObterFotoBase64(codigo, unitOfWork),
                    Anexos = (from obj in anexos
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  obj.NomeArquivo,
                                  TipoAnexoMotorista = obj.TipoAnexoMotorista.ObterDescricao(),
                                  NomeTela = "Motorista",
                                  ImprimirNaFicha = obj.ImprimeNaFichaMotorista ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao
                              }).ToList(),
                    DataAdmissao = motorista.DataAdmissao != null ? motorista.DataAdmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataDemissao = motorista.DataDemissao != null ? motorista.DataDemissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimentoMoop = motorista.DataVencimentoMoop != null ? motorista.DataVencimentoMoop.Value.ToString("dd/MM/yyyy") : string.Empty,
                    motorista.NumeroCTPS,
                    motorista.SerieCTPS,
                    motorista.ObservacaoConta,
                    motorista.PISAdministrativo,
                    motorista.Cargo,
                    motorista.CBO,
                    motorista.NumeroMatricula,
                    GridMotoristaDadoBancarios = (from obj in motorista.DadosBancarios
                                                  select new
                                                  {
                                                      obj.Codigo,
                                                      CodigoBanco = obj.Banco != null ? obj.Banco.Codigo : 0,
                                                      DescricaoTipoContaBanco = TipoContaBancoHelper.ObterDescricao(obj.TipoContaBanco),
                                                      Banco = obj.Banco != null ? obj.Banco.Descricao : string.Empty,
                                                      obj.Agencia,
                                                      obj.DigitoAgencia,
                                                      obj.NumeroConta,
                                                      obj.TipoContaBanco,
                                                      obj.ObservacaoConta,
                                                      TipoChavePix = obj.TipoChavePix ?? TipoChavePix.CPFCNPJ,
                                                      ChavePix = obj.ChavePix ?? ""
                                                  }).ToList(),
                    GridMotoristaContatos = (from obj in motorista.Contatos
                                             select new
                                             {
                                                 obj.Codigo,
                                                 obj.Email,
                                                 obj.Nome,
                                                 obj.Telefone,
                                                 obj.TipoParentesco,
                                                 CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? obj.CPF_Formatado : obj.CPF,
                                                 DataNascimento = obj.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty
                                             }).ToList(),
                    DataPrimeiraHabilitacao = motorista.DataPrimeiraHabilitacao?.ToString("dd/MM/yyyy"),
                    DataEmissaoRG = motorista.DataEmissaoRG?.ToString("dd/MM/yyyy"),
                    DataFimPeriodoExperiencia = motorista.DataFimPeriodoExperiencia?.ToString("dd/MM/yyyy"),
                    Celular = motorista.Celular_Formatado,
                    DataEmissaoCNH = motorista.DataHabilitacao?.ToString("dd/MM/yyyy"),
                    motorista.SituacaoColaborador,
                    motorista.SaldoAdiantamento,
                    motorista.SaldoDiaria,
                    motorista.DiasTrabalhado,
                    motorista.DiasFolgaRetirado,
                    UsuarioAdministradorMobile = motorista.UsuarioAdministradorMobile.HasValue ? motorista.UsuarioAdministradorMobile.Value : true,
                    PerfilAcessoMobile = motorista.PerfilAcessoMobile != null ? new { motorista.PerfilAcessoMobile.Codigo, motorista.PerfilAcessoMobile.Descricao } : null,
                    ModulosUsuarioMobile = (from codigoModuloMobile in motorista.ModulosLiberadosMobile
                                            select new
                                            {
                                                CodigoModulo = codigoModuloMobile
                                            }).ToList(),
                    motorista.NumeroProntuario,
                    motorista.Observacao,
                    motorista.EstadoCivil,
                    LocalidadeNascimento = motorista.LocalidadeNascimento != null ? new { Codigo = motorista.LocalidadeNascimento.Codigo, Descricao = motorista.LocalidadeNascimento.DescricaoCidadeEstado } : null,
                    motorista.TituloEleitoral,
                    motorista.ZonaEleitoral,
                    motorista.SecaoEleitoral,
                    motorista.CorRaca,
                    motorista.Sexo,
                    motorista.Escolaridade,
                    motorista.Aposentadoria,
                    motorista.SenhaGR,
                    motorista.FiliacaoMotoristaPai,
                    motorista.FiliacaoMotoristaMae,
                    EstadoCTPS = new { Codigo = motorista.EstadoCTPS?.Sigla ?? "", Descricao = motorista.EstadoCTPS?.Nome ?? "" },
                    DataExpedicaoCTPS = motorista.DataExpedicaoCTPS?.ToString("dd/MM/yyyy"),
                    CentroResultado = motorista.CentroResultado != null ? new { motorista.CentroResultado.Codigo, motorista.CentroResultado.Descricao } : null,
                    LocalidadeMunicipioEstadoCNH = motorista.LocalidadeMunicipioEstadoCNH != null ? new { Codigo = motorista.LocalidadeMunicipioEstadoCNH.Codigo, Descricao = motorista.LocalidadeMunicipioEstadoCNH.DescricaoCidadeEstado } : null,
                    motorista.CodigoSegurancaCNH,
                    Gestor = motorista.Gestor != null ? new { Codigo = motorista.Gestor.Codigo, Descricao = motorista.Gestor.Nome } : null,
                    NumeroCartaoValePedagio = motorista.NumeroCartaoValePedagio ?? "",
                    GridSituacoesColaborador = (from colaborador in motorista.SituacoesColaborador
                                                select new
                                                {
                                                    Codigo = colaborador.Codigo,
                                                    Descricao = colaborador.Descricao ?? string.Empty,
                                                    DataInicial = colaborador.DataInicial.ToString("dd/MM/yyyy") ?? string.Empty,
                                                    DataFinal = colaborador.DataFinal.ToString("dd/MM/yyyy") ?? string.Empty,
                                                    Situacao = SituacaoLancamentoColaboradorHelper.ObterDescricao(colaborador.SituacaoLancamento)
                                                }).ToList(),
                    motorista.PossuiControleDisponibilidade,
                    DataSuspensaoInicio = motorista.DataSuspensaoInicio?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataSuspensaoFim = motorista.DataSuspensaoFim?.ToString("dd/MM/yyyy") ?? string.Empty,
                    motorista.MotivoBloqueio,
                    RestringirLocaisCarregamentoAutorizadosMotoristas = motorista.Empresa?.RestringirLocaisCarregamentoAutorizadosMotoristas ?? false,
                    LocaisCarregamentosAutorizados = (from obj in motoristaLocaisCarregamentoAutorizados
                                                      select new
                                                      {
                                                          Codigo = obj.Cliente.CPF_CNPJ,
                                                          Descricao = obj.Cliente.Nome
                                                      }).ToList(),
                    GridEPIs = (from epi in motoristaEPIs
                                select new
                                {
                                    epi.Codigo,
                                    CodigoEPI = epi.EPI?.Codigo ?? 0,
                                    DescricaoEPI = epi.EPI?.Descricao ?? string.Empty,
                                    DataRepasse = epi.DataRepasse?.ToString("dd/MM/yyyy") ?? string.Empty,
                                    SerieEPI = epi.SerieEPI ?? string.Empty,
                                    Quantidade = epi.Quantidade,
                                }).ToList(),
                    GaleriaMotorista = (from o in ObterFotosGaleria(motorista.Codigo, unitOfWork)
                                        select new
                                        {
                                            o.NomeArquivo,
                                            o.Base64
                                        }).ToList(),
                    GridMotoristaLiberacoesGR = (from obj in motorista.LiberacoesGR
                                                 select new
                                                 {
                                                     obj.Codigo,
                                                     obj.Descricao,
                                                     obj.Numero,
                                                     DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                                     DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                                     DescricaoLicenca = obj.Licenca != null ? obj.Licenca.Descricao : string.Empty,
                                                     CodigoLicenca = obj.Licenca != null ? obj.Licenca.Codigo : 0,
                                                 }).ToList()

                };

                return new JsonpResult(entidade);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCPF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string cpf = Request.GetStringParam("cpf").ObterSomenteNumeros();
                bool buscarPorEmpresaPai = Request.GetBoolParam("BuscarPorEmpresaPai");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = null;

                if (buscarPorEmpresaPai)
                {
                    Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresaPai = repositorioEmpresa.BuscarEmpresaPai();

                    motorista = repositorioMotorista.BuscarMotoristaPorCPFEEmpresa(cpf, empresaPai?.Codigo ?? 0);
                }
                else
                    motorista = repositorioMotorista.BuscarMotoristaPorCPF(cpf);

                if (motorista == null)
                    return new JsonpResult(null);

                return new JsonpResult(new
                {
                    motorista.Codigo,
                    motorista.Status,
                    motorista.Tipo,
                    motorista.Nome,
                    CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? motorista.CPF_Formatado : motorista.CPF,
                    motorista.RG,
                    motorista.Telefone,
                    DataNascimento = motorista.DataNascimento != null ? motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                    Empresa = new { Codigo = motorista.Empresa.Codigo, Descricao = motorista.Empresa.RazaoSocial },
                    motorista.NumeroHabilitacao,
                    motorista.NumeroRegistroHabilitacao,
                    Localidade = motorista.Localidade != null ? new { motorista.Localidade.Codigo, motorista.Localidade.Descricao } : null,
                    motorista.Endereco,
                    motorista.Email,
                    motorista.TipoMotorista
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoBuscarMotoristaPorCPF);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterUrlRegularizacaoOneTrust()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.Buscar();

                return new JsonpResult(new
                {
                    Url = configuracaoIntegracao.UrlRegularizacaoOneTrust
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um erro ao obter a url de regularização.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirFoto()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigo);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotoristaNaoEncontrado);

                string caminho = ObterCaminhoArquivoFoto(unitOfWork);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.FotoNaoEncontrada);
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, Localization.Resources.Transportadores.Motorista.RemoveuFoto, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoRemoverFotoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirFotoGaleria()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nomeArquivo = Request.GetStringParam("NomeArquivo");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigo);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotoristaNaoEncontrado);

                Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, Localization.Resources.Transportadores.Motorista.RemoveuFoto, unitOfWork);

                return new JsonpResult(new
                {
                    GaleriaMotorista = (from o in ObterFotosGaleria(codigo, unitOfWork)
                                        select new
                                        {
                                            o.NomeArquivo,
                                            o.Base64
                                        }).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoRemoverFotoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigo, true);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Usuario servicoUsuario = new Servicos.Usuario(unitOfWork);
                servicoUsuario.RemoveUsuarioMobile(ref motorista, ClienteAcesso, unitOfWorkAdmin);

                Repositorio.Embarcador.Usuarios.FuncionarioEPI repositorioMotoristaEPI = new Repositorio.Embarcador.Usuarios.FuncionarioEPI(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> motoristaEPIs = repositorioMotoristaEPI.BuscarPorUsuario(motorista);

                foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI epi in motoristaEPIs)
                    repositorioMotoristaEPI.Deletar(epi);

                repMotorista.Deletar(motorista, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public async Task<IActionResult> ZerarSaldoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Motorista");

                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermiteZerarSaldo)))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.SeuUsuarioNaoPossuiPermissaoParaZerarSaldoDoMotorista);

                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                decimal saldoDiaria = motorista.SaldoDiaria;
                decimal saldoAdiantamento = motorista.SaldoAdiantamento;

                motorista.SaldoDiaria = 0;
                motorista.SaldoAdiantamento = 0;

                repMotorista.Atualizar(motorista, Auditado);

                Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista repHistoricoSaldoMotorista = new Repositorio.Embarcador.Acerto.HistoricoSaldoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista entidadeHistorico;
                if (saldoDiaria != 0)
                {
                    entidadeHistorico = new Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista();
                    entidadeHistorico.Motorista = motorista;
                    entidadeHistorico.Usuario = this.Usuario;
                    entidadeHistorico.TipoPagamentoMotorista = TipoPagamentoMotorista.Diaria;
                    entidadeHistorico.Data = DateTime.Now;
                    entidadeHistorico.DataLancamento = DateTime.Now;
                    entidadeHistorico.Valor = saldoDiaria * -1;
                    repHistoricoSaldoMotorista.Inserir(entidadeHistorico, Auditado);
                }

                if (saldoAdiantamento != 0)
                {
                    entidadeHistorico = new Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista();
                    entidadeHistorico.Motorista = motorista;
                    entidadeHistorico.Usuario = this.Usuario;
                    entidadeHistorico.TipoPagamentoMotorista = TipoPagamentoMotorista.Adiantamento;
                    entidadeHistorico.Data = DateTime.Now;
                    entidadeHistorico.DataLancamento = DateTime.Now;
                    entidadeHistorico.Valor = saldoAdiantamento * -1;
                    repHistoricoSaldoMotorista.Inserir(entidadeHistorico, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, Localization.Resources.Transportadores.Motorista.ZerouSaldoDoMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoZerarSaldo);
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
#if DEBUG
                //Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                //var configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                //Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                //var carga = repCarga.BuscarPorCodigo(69823);

                //Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                //svcCarga.ConfirmarEnvioDosDocumentos(carga, unitOfWork, unitOfWork.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, configuracaoTMS, Auditado, Cliente);

                //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                //Servicos.SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.SemPararValePedagio.ValePedagioClient();
                //Servicos.SemPararValePedagio.Identificador identifador = new Servicos.SemPararValePedagio.Identificador();
                //identifador = valePedagioClient.autenticarUsuario("2022184763", "ADMINISTRADOR", "Helena@2022");

                //try
                //{
                //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                //    Servicos.SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.SemPararValePedagio.ValePedagioClient, Servicos.SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                //    var identifador = valePedagioClient.autenticarUsuario("33453598000123", "administrador", "grupostp");
                //}
                //catch (Exception ex)
                //{

                //    throw;
                //}
#endif

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                bool exibirTipoMotorista = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? true : Request.GetBoolParam("ExibirTipoMotorista");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 34, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.TipoMotorista, "TipoMotorista", 10, Models.Grid.Align.center, false, exibirTipoMotorista);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.Cpf, "CPF", 13, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.Transportador, "Empresa", 32, Models.Grid.Align.left, true, true, true, false, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? false : true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false, true, true);
                if (ConfiguracaoEmbarcador.VisualizarVeiculosPropriosETerceiros)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.Terceiro, "Terceiro", 10, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Usuario> listaMotorista = new List<Dominio.Entidades.Usuario>();
                int countMotoristas = repMotorista.ContarConsultaEmbarcador(filtrosPesquisa);

                if (countMotoristas > 0)
                    listaMotorista = repMotorista.ConsultarEmbarcador(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = (from p in listaMotorista
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 CodigoEmpresa = p.Empresa?.Codigo ?? 0,
                                 p.Nome,
                                 CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? p.CPF_Formatado : p.CPF,
                                 Empresa = p.Empresa != null ? p.Empresa?.RazaoSocial + " (" + p.Empresa?.Localidade.DescricaoCidadeEstado + ")" : string.Empty,
                                 p.DescricaoStatus,
                                 Terceiro = p.ClienteTerceiro != null ? p.ClienteTerceiro.Descricao ?? "" : string.Empty,
                                 TipoMotorista = p.TipoMotorista.ObterDescricao(),
                                 TipoMotoristaAjudante = ObterTipoMotoristaAjudante(p.Ajudante).ObterDescricao(),
                             }).ToList();

                grid.setarQuantidadeTotal(countMotoristas);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, ex.Message);
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
        public async Task<IActionResult> PesquisaMobile()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 34, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.CPF, "CPF", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.Transportador, "Empresa", 32, Models.Grid.Align.left, true, true, true, false, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? false : true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                    Cpf = Request.GetStringParam("CPF"),
                    Nome = Request.GetStringParam("Nome"),
                    PlacaVeiculo = Request.GetStringParam("Placa"),
                    Transportador = ObterTransportador(unitOfWork),
                    UtilizarProgramacaoCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga
                };

                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> listaMotorista = new List<Dominio.Entidades.Usuario>();
                int totalMotoristas = repositorioMotorista.ContarConsultaMotoristaMobile(filtrosPesquisa);

                if (totalMotoristas > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                    listaMotorista = repositorioMotorista.ConsultarMotoristaMobile(filtrosPesquisa, parametrosConsulta);
                }

                var listaMotoristaRetornar = (
                    from motorista in listaMotorista
                    select new
                    {
                        motorista.Codigo,
                        motorista.Nome,
                        CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? motorista.CPF_Formatado : motorista.CPF,
                        Empresa = motorista.Empresa != null ? $"{motorista.Empresa.RazaoSocial} ({motorista.Empresa.Localidade.DescricaoCidadeEstado})" : string.Empty,
                        motorista.DescricaoStatus
                    }
                ).ToList();

                grid.setarQuantidadeTotal(totalMotoristas);
                grid.AdicionaRows(listaMotoristaRetornar);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMotoristaGestaoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista()
                {
                    SituacaoColaborador = SituacaoColaborador.Todos,
                    Empresa = null,
                    Nome = Request.GetStringParam("Nome"),
                    CpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF")),
                    Tipo = "M",
                    Status = SituacaoAtivoPesquisa.Todos,
                    PlacaVeiculo = "",
                    SomentePendenteDeVinculo = false,
                    PendenteIntegracaoEmbarcador = false,
                    CodigoCargo = 0,
                    ProprietarioTerceiro = 0d,
                    NumeroMatricula = "",
                    CnpjRemetenteLocalCarregamentoAutorizado = 0d,
                    NaoBloqueado = false,
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.CPF, "CPF", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 70, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> listaMotorista = repMotorista.ConsultarEmbarcador(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMotorista.ContarConsultaEmbarcador(filtrosPesquisa));

                var lista = (from p in listaMotorista
                             select new
                             {
                                 Codigo = p.Codigo + "_" + "motorista",
                                 p.Nome,
                                 CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? p.CPF_Formatado : p.CPF,
                                 DT_RowColor = "#FFFFFF"
                             }).ToList();
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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
        public async Task<IActionResult> PesquisaMotoristaMovimentacaoDePlacas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista()
                {
                    SituacaoColaborador = SituacaoColaborador.Todos,
                    Empresa = null,
                    Nome = Request.GetStringParam("Nome"),
                    CpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF")),
                    Tipo = "M",
                    Status = SituacaoAtivoPesquisa.Ativo,
                    PlacaVeiculo = Request.GetStringParam("Placa"),
                    SomentePendenteDeVinculo = Request.GetBoolParam("SomentePendentes"),
                    PendenteIntegracaoEmbarcador = false,
                    CodigoCargo = 0,
                    ProprietarioTerceiro = 0d,
                    NumeroMatricula = "",
                    CnpjRemetenteLocalCarregamentoAutorizado = 0d,
                    NaoBloqueado = false,
                };

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
                    filtrosPesquisa.PlacaVeiculo = filtrosPesquisa.PlacaVeiculo.Replace("_", "");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.CPF, "CPF", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ContemVinculo", false);
                grid.AdicionarCabecalho("PlacaVinculo", false);
                grid.AdicionarCabecalho("TipoVinculo", false);

                //string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                string propOrdenar = "DataRemocaoVinculo";

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> listaMotorista = repMotorista.ConsultarEmbarcador(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMotorista.ContarConsultaEmbarcador(filtrosPesquisa));

                var lista = (from p in listaMotorista
                             select new
                             {
                                 Codigo = p.Codigo + "_" + "motorista",
                                 p.Nome,
                                 CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? p.CPF_Formatado : p.CPF,
                                 DT_RowClass = repMotorista.ContemVinculoEmTracao(p) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(IntensidadeCor._100) : "",
                                 ContemVinculo = repMotorista.ContemVinculoEmTracao(p),
                                 PlacaVinculo = repMotorista.NomeVinculoEmTracao(p),
                                 TipoVinculo = repMotorista.TipoVeiculoVinculado(p)
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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
        public async Task<IActionResult> PesquisaPorCPF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista()
                {
                    Nome = Request.GetStringParam("CPF"),
                    SituacaoColaborador = Request.GetEnumParam<SituacaoColaborador>("SituacaoColaborador"),
                    CpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("Nome")),
                    Tipo = "M",
                    Status = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                    PlacaVeiculo = "",
                    SomentePendenteDeVinculo = false,
                    PendenteIntegracaoEmbarcador = false,
                    CodigoCargo = 0,
                    ProprietarioTerceiro = 0d,
                    NumeroMatricula = "",
                    CnpjRemetenteLocalCarregamentoAutorizado = 0d,
                    NaoBloqueado = false,
                };

                Dominio.Entidades.Empresa empresa = null;
                int codigoEmpresa = Request.GetIntParam("Empresa", 0);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = Usuario.Empresa;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                    if (empresaTerceiro == null)
                        return new JsonpResult(false, string.Format(Localization.Resources.Consultas.Motorista.CnpjNaoPossuiCadastroComoTransportadorVerifiqueComSeuSuporte, Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato));
                    empresa = empresaTerceiro;
                }
                else if (codigoEmpresa > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                filtrosPesquisa.Empresa = empresa;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 34, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.Cpf, "CPF", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;


                List<Dominio.Entidades.Usuario> listaMotorista = repMotorista.ConsultarEmbarcador(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMotorista.ContarConsultaEmbarcador(filtrosPesquisa));

                var lista = (from p in listaMotorista select new { p.Codigo, p.Nome, CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? p.CPF_Formatado : p.CPF, p.DescricaoStatus }).ToList();
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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
        public async Task<IActionResult> PesquisaPorCPFPortalRetira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista()
                {
                    CpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF")),
                    Tipo = "M",
                    Status = SituacaoAtivoPesquisa.Ativo,
                    PlacaVeiculo = "",
                    SomentePendenteDeVinculo = false,
                    PendenteIntegracaoEmbarcador = false,
                    CodigoCargo = 0,
                    ProprietarioTerceiro = 0d,
                    NumeroMatricula = "",
                    CnpjRemetenteLocalCarregamentoAutorizado = 0d,
                    NaoBloqueado = false,
                };

                //Removido o filtro por transportadora  pois foi exigido na tarefa #62381
                //Dominio.Entidades.Empresa empresa = null;
                //int codigoEmpresa = Request.GetIntParam("Empresa", 0);

                //if (codigoEmpresa > 0)
                //    empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);
                //else
                //    empresa = repositorioEmpresa.BuscarEmpresaPadraoRetirada();

                //filtrosPesquisa.Empresa = empresa;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 34, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Motorista.Cpf, "CPF", 13, Models.Grid.Align.left, false);
                //grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;


                List<Dominio.Entidades.Usuario> listaMotorista = repMotorista.ConsultarEmbarcador(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMotorista.ContarConsultaEmbarcador(filtrosPesquisa));

                var lista = (from p in listaMotorista select new { p.Codigo, p.Nome, CPF = ConfiguracaoEmbarcador.Pais != TipoPais.Exterior ? p.CPF_Formatado : p.CPF }).ToList();
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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
        public async Task<IActionResult> PesquisaMotoristaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

                string nome = Request.Params("Nome");
                string cpf = Utilidades.String.OnlyNumbers(Request.Params("CPF"));
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 34, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.CPF, "CPF", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descricao", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Usuario> listaMotoristas = repCargaMotorista.ConsultarMotoristasCarga(codigoCarga, nome, cpf, parametrosConsulta);
                grid.setarQuantidadeTotal(repCargaMotorista.ContarMotoristasCarga(codigoCarga, nome, cpf));

                dynamic lista = (from p in listaMotoristas
                                 select new
                                 {
                                     p.Codigo,
                                     p.Nome,
                                     CPF = p.CPF_Formatado,
                                     p.Descricao
                                 }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoConsultarOsMotoristas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoMotoristas();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();
                unitOfWorkAdmin.Start();

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoMotoristas();
                List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, motoristas, ((dados) =>
                {
                    Servicos.Embarcador.Transportadores.ImportacaoMotorista servicoMotoristaImportar = new Servicos.Embarcador.Transportadores.ImportacaoMotorista(dados, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Empresa, unitOfWorkAdmin);
                    return servicoMotoristaImportar.ObterMotoristaImportar();
                }));

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Transportadores.MotoristaDadoBancario repDadoBancario = new Repositorio.Embarcador.Transportadores.MotoristaDadoBancario(unitOfWork);

                foreach (Dominio.Entidades.Usuario motorista in ValidacaoDuplicidade(motoristas))
                {
                    if ((motorista.Codigo > 0) && permiteAtualizar)
                    {
                        repositorioUsuario.Atualizar(motorista, Auditado);
                        totalRegistrosImportados++;
                    }
                    else if ((motorista.Codigo == 0) && permiteInserir)
                    {
                        Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario dadoBancario = null;
                        if (motorista.DadosBancarios != null && motorista.DadosBancarios.Count == 1)
                        {
                            dadoBancario = motorista.DadosBancarios[0];
                            motorista.DadosBancarios = null;
                        }
                        repositorioUsuario.Inserir(motorista, Auditado);

                        if (dadoBancario != null)
                            repDadoBancario.Inserir(dadoBancario);

                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarLicencaMotoristas(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Transportadores.MotoristaLicenca repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);

                bool validar = Request.GetBoolParam("Validar");
                var dynRetorno = new
                {
                    Msg = "",
                    validar = false
                };

                if (!validar)
                {
                    dynRetorno = new
                    {
                        Msg = "",
                        validar = true
                    };

                    return new JsonpResult(dynRetorno);
                }

                dynamic motoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Motoristas"));

                List<int> codigosMotoristas = new List<int>();

                foreach (var motorista in motoristas)
                    codigosMotoristas.Add((int)motorista.Codigo);

                List<Dominio.Entidades.Usuario> motoristasEntidade = await repositorioMotorista.BuscarPorCodigosAsync(codigosMotoristas);

                if (!(motoristasEntidade != null))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoBuscarMotoristaPorCPF);

                foreach (Dominio.Entidades.Usuario motoristaUsuario in motoristasEntidade)
                {
                    List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> motoristaLicencasLista = motoristaUsuario.Licencas.ToList();

                    if (motoristaLicencasLista.Count == 0)
                        return new JsonpResult(false, true, $"Motorista {motoristaUsuario.Nome} sem Licença!");

                    foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca motoristaLicenca in motoristaLicencasLista)
                    {
                        if (motoristaLicenca.DataVencimento.HasValue && motoristaLicenca.DataVencimento.Value.Date < DateTime.Now.Date)
                        {
                            motoristaLicenca.Status = StatusLicenca.Vencido;

                            await repositorioMotoristaLicenca.AtualizarAsync(motoristaLicenca);

                            return new JsonpResult(false, true, $"Motorista {motoristaLicenca.Motorista.Nome} com licença Vencida!");
                        }

                    }
                }

                dynRetorno = new
                {
                    Msg = "Motoristas listados com Sucesso!",
                    validar = true
                };

                return new JsonpResult(dynRetorno);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoBuscarMotoristaPorCPF);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscaLicencaMotoristasAVencer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            bool.TryParse(Request.Params("ApenasSemOrdemAberta"), out bool apenasSemOrdemAberta);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Motorista", "Nome", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Licença", "LicencaDescricao", 5, Models.Grid.Align.center, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                List<dynamic> licencasAvencer = ExecutaPesquisaLicencaMotoristasVencer(apenasSemOrdemAberta);

                grid.setarQuantidadeTotal(licencasAvencer.Count);
                grid.AdicionaRows(licencasAvencer);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as licenças dos motoristas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> ObterLicencasSelecionadas(Repositorio.UnitOfWork unitOfWork, HttpRequest request)
        {
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> listaMotoristaLicenca = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();

            bool.TryParse(request.Params("SelecionarTodos"), out bool todosSelecionados);
            bool.TryParse(request.Params("ApenasSemOrdemAberta"), out bool apenasSemOrdemAberta);

            if (todosSelecionados)
            {
                try
                {
                    // Executa a pesquisa e obtém as licenças
                    List<dynamic> todasLinhasDinamicas = ExecutaPesquisaLicencaMotoristasVencer(apenasSemOrdemAberta);

                    foreach (var linhaDinamica in todasLinhasDinamicas)
                    {
                        // Busca a licença por código
                        Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca motoristaLicenca = repMotoristaLicenca.BuscarLicencasMotoristaPorCodigo((int)linhaDinamica.Codigo);

                        // Adiciona a licença à lista, se encontrada
                        if (motoristaLicenca != null)
                            listaMotoristaLicenca.Add(motoristaLicenca);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converter dados.");
                }

                dynamic listaLicencasNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)request.Params("LicencasNaoSelecionadas"));
                foreach (var dybLicencaNaoSelecionada in listaLicencasNaoSelecionadas)
                    listaMotoristaLicenca.Remove(new Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca() { Codigo = (int)dybLicencaNaoSelecionada.Codigo });
            }
            else
            {
                dynamic listaLicencasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)request.Params("LicencasSelecionadas"));
                foreach (var dynLicencaSelecionada in listaLicencasSelecionadas)
                    listaMotoristaLicenca.Add(repMotoristaLicenca.BuscarLicencasMotoristaPorCodigo((int)dynLicencaSelecionada.Codigo));
            }

            return listaMotoristaLicenca;
        }

        #endregion

        #region Métodos Privados
        private List<dynamic> ExecutaPesquisaLicencaMotoristasVencer(bool apenasSemOrdemAberta)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<dynamic> todasLinhas = new List<dynamic>();
            try
            {
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Transportadores.MotoristaLicenca repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);

                List<Dominio.Entidades.Usuario> motoristasEntidade = new List<Dominio.Entidades.Usuario>();
                if (apenasSemOrdemAberta)
                {
                    motoristasEntidade = repositorioMotorista.BuscarTodosMotoristasAtivosSemOrdemAberta(0, 9999);
                }
                else
                {
                    motoristasEntidade = repositorioMotorista.BuscarTodosMotoristasAtivos(0, 9999);
                }

                foreach (Dominio.Entidades.Usuario motoristaUsuario in motoristasEntidade)
                {
                    List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> motoristaLicencasLista = motoristaUsuario.Licencas.ToList();
                    if (motoristaLicencasLista.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca motoristaLicenca in motoristaLicencasLista)
                        {
                            if ((motoristaLicenca.DataVencimento.HasValue && motoristaLicenca.DataVencimento.Value.Date < DateTime.Now.Date.AddDays(-30))
                                     && (motoristaLicenca.Licenca?.GerarRequisicao ?? false))
                            {
                                todasLinhas.Add(new
                                {
                                    motoristaLicenca.Codigo,
                                    motoristaLicenca.Motorista?.Nome,
                                    DataEmissao = motoristaLicenca.DataEmissao?.ToString("dd/MM/yyyy") ?? "",
                                    DataVencimento = motoristaLicenca.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                                    Descricao = motoristaLicenca.Descricao ?? "",
                                    LicencaDescricao = motoristaLicenca.Licenca?.Descricao ?? ""
                                });
                            }

                        }
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
            return todasLinhas;
        }

        private bool ClienteExigeContraSenha()
        {
            return (IsHomologacao ? Cliente.ClienteConfiguracaoHomologacao : Cliente.ClienteConfiguracao)?.ExigeContraSenha ?? false;
        }

        private void PreencherDadosMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Motorista");

            DateTime? dataEmissaoRG = Request.GetNullableDateTimeParam("DataEmissaoRG");
            DateTime? dataPrimeiraHabilitacao = Request.GetNullableDateTimeParam("DataPrimeiraHabilitacao");

            int codigoEmpresa, codigoPlanoConta, codigoBanco = 0;
            int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
            int.TryParse(Request.Params("Banco"), out codigoBanco);
            int.TryParse(Request.Params("PlanoAcertoViagem"), out codigoPlanoConta);
            TipoMotorista tipoMotorista;
            Enum.TryParse(Request.Params("TipoMotorista"), out tipoMotorista);
            int.TryParse(Request.Params("PerfilAcessoMobile"), out int codigoPerfilAcessoMobile);

            bool ativo, usuarioUsaMobile, bloqueado, naoBloquearAcessoSimultaneo;
            bool.TryParse(Request.Params("Ativo"), out ativo);
            bool.TryParse(Request.Params("Bloqueado"), out bloqueado);
            bool.TryParse(Request.Params("UsuarioMobile"), out usuarioUsaMobile);
            bool.TryParse(Request.Params("NaoBloquearAcessoSimultaneo"), out naoBloquearAcessoSimultaneo);

            bool naoGeraComissaoAcerto = false, ativarFichaMotorista = false;
            bool.TryParse(Request.Params("NaoGeraComissaoAcerto"), out naoGeraComissaoAcerto);
            bool.TryParse(Request.Params("AtivarFichaMotorista"), out ativarFichaMotorista);

            double clienteTerceiro;
            double.TryParse(Request.Params("ClienteTerceiro"), out clienteTerceiro);

            string codigoIntegracaoContabilidade = Request.Params("CodigoIntegracaoContabilidade");
            string categoriaHabilitacao = Request.Params("CategoriaHabilitacao");
            string celular = Request.Params("Celular");

            DateTime? dataEmissaoCNH = Request.GetNullableDateTimeParam("DataEmissaoCNH");
            DateTime? dataValidadeCNH = Request.GetNullableDateTimeParam("DataValidadeCNH");
            DateTime? dataFimPeriodoExperiencia = Request.GetNullableDateTimeParam("DataFimPeriodoExperiencia");

            TipoEndereco tipoEndereco = (TipoEndereco)int.Parse(Request.Params("TipoEndereco"));
            TipoEmail tipoEmail = (TipoEmail)int.Parse(Request.Params("TipoEmail"));
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);

            motorista.NaoBloquearAcessoSimultaneo = naoBloquearAcessoSimultaneo;
            motorista.DataHabilitacao = dataEmissaoCNH;
            motorista.DataEmissaoRG = dataEmissaoRG;
            motorista.DataFimPeriodoExperiencia = dataFimPeriodoExperiencia;
            motorista.DataPrimeiraHabilitacao = dataPrimeiraHabilitacao;
            motorista.OrdenarCargasMobileCrescente = Request.GetBoolParam("OrdenarCargasMobileCrescente");
            motorista.Status = Request.Params("Status");

            if (motorista.Status == "A" && !ativo && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermitirInativarCadastroMotorista))
                throw new ControllerException("Precisa ter permissão especial para inativar o cadastro do motorista");

            if (motorista.Status == "I" && ativo && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermitirInativarCadastroMotorista))
                throw new ControllerException("Precisa ter permissão especial para inativar o cadastro do motorista");

            int.TryParse(Request.Params("Filial"), out int filial);

            if (filial > 0)
                motorista.Filial = repFilial.BuscarPorCodigo(filial);
            else
                motorista.Filial = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                if (Usuario.ClienteTerceiro != null)
                    motorista.Empresa = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                if (motorista.Empresa == null)
                    throw new ControllerException(Localization.Resources.Transportadores.Motorista.TransportadorTerceiroNaoCadastradoComoEmpresa);
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                motorista.Empresa = Usuario.Empresa;
            else if (codigoEmpresa > 0)
                motorista.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            else
                motorista.Empresa = null;

            if (!String.IsNullOrWhiteSpace(Request.Params("Empresa")) && int.Parse(Request.Params("Empresa")) > 0)
                motorista.Empresa = repEmpresa.BuscarPorCodigo(int.Parse(Request.Params("Empresa")));
            else
                motorista.Empresa = null;

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (motorista.Empresa != null && motorista.Empresa.Matriz != null && motorista.Empresa.Matriz.Count > 0)
                    motorista.Empresa = motorista.Empresa.Matriz.FirstOrDefault();
            }

            if (ativo)
                motorista.Status = "A";
            else
                motorista.Status = "I";

            if (!string.IsNullOrWhiteSpace(Request.Params("DataValidadeLiberacaoSeguradora")))
            {
                DateTime dataValidadeLiberacaoSeguradora;
                DateTime.TryParseExact(Request.Params("DataValidadeLiberacaoSeguradora"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidadeLiberacaoSeguradora);
                motorista.DataValidadeLiberacaoSeguradora = dataValidadeLiberacaoSeguradora;
            }
            else
                motorista.DataValidadeLiberacaoSeguradora = null;

            TipoContaBanco tipoConta;
            Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

            bool permiteBloquearMotorista = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermiteBloquearMotorista) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            motorista.Bloqueado = permiteBloquearMotorista ? bloqueado : motorista.Bloqueado;
            motorista.Banco = codigoBanco > 0 ? repBanco.BuscarPorCodigo(codigoBanco) : null;
            motorista.Agencia = Request.Params("Agencia");
            motorista.DigitoAgencia = Request.Params("Digito");
            motorista.NumeroConta = Request.Params("NumeroConta");
            motorista.PendenteIntegracaoEmbarcador = true;
            motorista.TipoContaBanco = tipoConta;
            motorista.ObservacaoConta = Request.Params("ObservacaoConta");

            motorista.PISAdministrativo = Request.Params("PISAdministrativo");
            motorista.Cargo = Request.Params("Cargo");
            motorista.CBO = Request.Params("CBO");
            motorista.NumeroMatricula = Request.Params("NumeroMatricula");
            motorista.NumeroProntuario = Request.Params("NumeroProntuario");

            motorista.Tipo = "M";
            motorista.Nome = Request.Params("Nome");
            motorista.Apelido = Request.Params("Apelido");
            motorista.MotoristaEstrangeiro = Request.GetBoolParam("MotoristaEstrangeiro");
            motorista.CPF = motorista.MotoristaEstrangeiro ? Request.GetStringParam("CodigoMotoristaEstrangeiro") : Utilidades.String.OnlyNumbers(Request.Params("CPF"));
            motorista.RG = Request.Params("RG");
            motorista.EstadoRG = repEstado.BuscarPorSigla(Request.Params("EstadoRG"));
            motorista.UFEmissaoCNH = repEstado.BuscarPorSigla(Request.Params("UFEmissaoCNH"));
            motorista.OrgaoEmissorRG = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG>("EmissorRG");
            motorista.Telefone = Request.Params("Telefone");
            motorista.Celular = celular;
            motorista.CodigoIntegracao = Request.Params("CodigoIntegracao");
            motorista.NumeroCartao = Request.Params("NumeroCartao");
            motorista.TipoCartao = Request.GetNullableEnumParam<TipoPessoaCartao>("TipoCartao");
            motorista.TipoEmail = tipoEmail;
            motorista.TipoEndereco = tipoEndereco;
            motorista.TipoMotorista = tipoMotorista;
            motorista.NaoGeraComissaoAcerto = naoGeraComissaoAcerto;
            motorista.AtivarFichaMotorista = ativarFichaMotorista;
            motorista.PIS = Request.Params("PISPASEP");
            if (clienteTerceiro > 0)
                motorista.ClienteTerceiro = repCliente.BuscarPorCPFCNPJ(clienteTerceiro);
            else
                motorista.ClienteTerceiro = null;
            motorista.CodigoIntegracaoContabilidade = codigoIntegracaoContabilidade;

            DateTime.TryParseExact(Request.Params("DataNascimento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataNascimento);
            DateTime.TryParseExact(Request.Params("DataAdmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAdmissao);
            DateTime.TryParseExact(Request.Params("DataDemissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataDemissao);
            DateTime.TryParseExact(Request.Params("DataVencimentoMoop"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimentoMoop);

            if (dataNascimento > DateTime.MinValue)
                motorista.DataNascimento = dataNascimento;
            else
                motorista.DataNascimento = null;
            if (dataAdmissao > DateTime.MinValue)
                motorista.DataAdmissao = dataAdmissao;
            else
                motorista.DataAdmissao = null;
            if (dataDemissao > DateTime.MinValue)
                motorista.DataDemissao = dataDemissao;
            else
                motorista.DataDemissao = null;
            if (dataVencimentoMoop > DateTime.MinValue)
                motorista.DataVencimentoMoop = dataVencimentoMoop;
            else
                motorista.DataVencimentoMoop = null;

            motorista.NumeroHabilitacao = Request.Params("NumeroHabilitacao");
            motorista.NumeroRegistroHabilitacao = Request.Params("NumeroRegistroHabilitacao");
            motorista.Categoria = categoriaHabilitacao;
            motorista.DataVencimentoHabilitacao = dataValidadeCNH;

            if (!string.IsNullOrWhiteSpace(Request.Params("Localidade")) && Request.Params("Localidade") != "0")
                motorista.Localidade = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Localidade")) };

            motorista.Endereco = Request.Params("Endereco");
            motorista.Email = Request.Params("Email");

            motorista.Bairro = Request.Params("Bairro");
            motorista.CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
            motorista.Complemento = Request.Params("Complemento");
            motorista.NumeroEndereco = Request.Params("NumeroEndereco");
            motorista.TipoLogradouro = (TipoLogradouro)int.Parse(Request.Params("TipoLogradouro"));
            motorista.TipoResidencia = (TipoResidencia)int.Parse(Request.Params("TipoResidencia"));
            motorista.EnderecoDigitado = bool.Parse(Request.Params("EnderecoDigitado"));
            motorista.Latitude = Request.Params("Latitude");
            motorista.Longitude = Request.Params("Longitude");
            motorista.NumeroCTPS = Request.Params("NumeroCTPS");
            motorista.SerieCTPS = Request.Params("SerieCTPS");

            motorista.PlanoAcertoViagem = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
            motorista.UsuarioAdministradorMobile = Request.GetBoolParam("UsuarioAdministradorMobile");
            motorista.PerfilAcessoMobile = codigoPerfilAcessoMobile > 0 ? repPerfilAcessoMobile.BuscarPorCodigo(codigoPerfilAcessoMobile) : null;
            motorista.DataValidadeGR = Request.GetNullableDateTimeParam("DataValidadeGR");

            SalvarTransportadoras(motorista, unitOfWork);
        }

        private void PreencherDadosAdicionaisMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Pessoas.Cargo repCargo = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int codigoLocalidadeNascimento = Request.GetIntParam("LocalidadeNascimento");
            int codigoLocalidadeMunicipioEstadoCNH = Request.GetIntParam("LocalidadeMunicipioEstadoCNH");
            string estadoCTPS = Request.GetStringParam("EstadoCTPS");
            int codigoCentroResultado = Request.GetIntParam("CentroResultado");
            int codigoCargo = Request.GetIntParam("CargoMotorista");
            int codigoUsuario = Request.GetIntParam("Gestor");

            motorista.Observacao = Request.GetStringParam("Observacao");
            motorista.TituloEleitoral = Request.GetStringParam("TituloEleitoral");
            motorista.ZonaEleitoral = Request.GetStringParam("ZonaEleitoral");
            motorista.SecaoEleitoral = Request.GetStringParam("SecaoEleitoral");
            motorista.DataExpedicaoCTPS = Request.GetNullableDateTimeParam("DataExpedicaoCTPS");
            motorista.RenachHabilitacao = Request.GetStringParam("RenachCNH");
            motorista.PossuiControleDisponibilidade = Request.GetBoolParam("PossuiControleDisponibilidade");
            motorista.DataSuspensaoInicio = Request.GetNullableDateTimeParam("DataSuspensaoInicio");
            motorista.DataSuspensaoFim = Request.GetNullableDateTimeParam("DataSuspensaoFim");
            motorista.MotivoBloqueio = motorista.Bloqueado ? Request.GetNullableStringParam("MotivoBloqueio") : "";

            motorista.EstadoCivil = Request.GetNullableEnumParam<EstadoCivil>("EstadoCivil");
            motorista.CorRaca = Request.GetNullableEnumParam<CorRaca>("CorRaca");
            motorista.Sexo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Enumerador.Sexo>("Sexo");
            motorista.Escolaridade = Request.GetNullableEnumParam<Escolaridade>("Escolaridade");
            motorista.Aposentadoria = Request.GetNullableEnumParam<Aposentadoria>("Aposentadoria");
            motorista.SenhaGR = Request.GetStringParam("SenhaGR");
            motorista.FiliacaoMotoristaMae = Request.GetStringParam("FiliacaoMotoristaMae");
            motorista.FiliacaoMotoristaPai = Request.GetStringParam("FiliacaoMotoristaPai");

            motorista.CargoMotorista = codigoCargo > 0 ? repCargo.BuscarPorCodigo(codigoCargo) : null;
            motorista.LocalidadeNascimento = codigoLocalidadeNascimento > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeNascimento) : null;
            motorista.EstadoCTPS = !string.IsNullOrWhiteSpace(estadoCTPS) ? repEstado.BuscarPorSigla(estadoCTPS) : null;
            motorista.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;

            motorista.CodigoSegurancaCNH = Request.GetStringParam("CodigoSegurancaCNH");
            motorista.LocalidadeMunicipioEstadoCNH = codigoLocalidadeMunicipioEstadoCNH > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeMunicipioEstadoCNH) : null;

            motorista.Gestor = codigoUsuario > 0 ? repUsuario.BuscarPorCodigo(codigoUsuario) : null;
            motorista.NumeroCartaoValePedagio = Request.GetNullableStringParam("NumeroCartaoValePedagio");
            motorista.Ajudante = Request.GetBoolParam("Ajudante");
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoMotoristas()
        {
            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;
            var tipoServicoMultiEmbarcador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Gerais.Geral.Nome, Propriedade = "Nome", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Transportadores.Motorista.CPF, Propriedade = "CPF", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Transportadores.Motorista.CNPJTransportadora, Propriedade = "CnpjTransportadora", Tamanho = tamanho, Obrigatorio = tipoServicoMultiEmbarcador, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Transportadores.Motorista.DataValidadeSeguradora, Propriedade = "DataValidadeLiberacaoSeguradora", Tamanho = tamanho, Obrigatorio = tipoServicoMultiEmbarcador, CampoInformacao = true, Regras = new List<string> { "required" } });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Transportadores.Motorista.NumeroCNH, Propriedade = "CNH", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Transportadores.Motorista.DataValidadeCNH, Propriedade = "ValidadeCNH", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Transportadores.Motorista.Telefone, Propriedade = "Telefone", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Transportadores.Motorista.DataNascimento, Propriedade = "DataNascimento", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Transportadores.Motorista.PISPASEP, Propriedade = "PISPASEP", Tamanho = tamanho, CampoInformacao = true });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Transportadores.Motorista.RG, Propriedade = "RG", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Transportadores.Motorista.DataEmissaoRG, Propriedade = "DataEmissaoRG", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Transportadores.Motorista.EmissorRG, Propriedade = "EmissorRG", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Transportadores.Motorista.EstadoRG, Propriedade = "EstadoRG", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Transportadores.Motorista.CategoriaCNH, Propriedade = "CategoriaHabilitacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Transportadores.Motorista.DataEmissaoCNH, Propriedade = "DataEmissaoCNH", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Transportadores.Motorista.DataPrimeiraCNH, Propriedade = "DataPrimeiraHabilitacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Transportadores.Motorista.Celular, Propriedade = "Celular", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 18, Descricao = Localization.Resources.Transportadores.Motorista.CEP, Propriedade = "CEP", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Transportadores.Motorista.DataAdmissao, Propriedade = "DataAdmissao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 20, Descricao = Localization.Resources.Transportadores.Motorista.NumeroCTPS, Propriedade = "NumeroCTPS", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 21, Descricao = Localization.Resources.Transportadores.Motorista.SerieCTPS, Propriedade = "SerieCTPS", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 22, Descricao = Localization.Resources.Transportadores.Motorista.Cargo, Propriedade = "Cargo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 23, Descricao = Localization.Resources.Transportadores.Motorista.CBO, Propriedade = "CBO", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 24, Descricao = Localization.Resources.Transportadores.Motorista.NumeroMatricula, Propriedade = "NumeroMatricula", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 25, Descricao = Localization.Resources.Transportadores.Motorista.NumeroEndereco, Propriedade = "NumeroEndereco", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 26, Descricao = Localization.Resources.Transportadores.Motorista.Complemento, Propriedade = "Complemento", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 27, Descricao = Localization.Resources.Transportadores.Motorista.NumeroRegistroCNH, Propriedade = "NumeroRegistroHabilitacao", Tamanho = tamanho, CampoInformacao = true });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 28, Descricao = Localization.Resources.Transportadores.Motorista.CodigoIBGE, Propriedade = "Localidade", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 29, Descricao = Localization.Resources.Transportadores.Motorista.Endereco, Propriedade = "Endereco", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 30, Descricao = Localization.Resources.Transportadores.Motorista.Bairro, Propriedade = "Bairro", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 31, Descricao = Localization.Resources.Transportadores.Motorista.NumeroCartao, Propriedade = "NumeroCartao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 32, Descricao = Localization.Resources.Transportadores.Motorista.Banco, Propriedade = "Banco", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 33, Descricao = Localization.Resources.Transportadores.Motorista.Agencia, Propriedade = "Agencia", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 34, Descricao = Localization.Resources.Transportadores.Motorista.NumeroConta, Propriedade = "NumeroConta", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 35, Descricao = Localization.Resources.Transportadores.Motorista.TipoMotorista, Propriedade = "TipoMotorista", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 36, Descricao = Localization.Resources.Transportadores.Motorista.PessoaAgregado, Propriedade = "PessoaAgregado", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 37, Descricao = Localization.Resources.Transportadores.Motorista.Cidade, Propriedade = "Cidade", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 38, Descricao = Localization.Resources.Transportadores.Motorista.UF, Propriedade = "UF", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 39, Descricao = Localization.Resources.Transportadores.Motorista.UFEmissaoCNH, Propriedade = "UFEmissaoCNH", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 40, Descricao = Localization.Resources.Transportadores.Motorista.CodigoIntegracao, Propriedade = "CodigoIntegracao", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        private bool ValidarCamposReferenteCIOT(Dominio.Entidades.Usuario motorista, out string erroValidacaoCIOT)
        {
            erroValidacaoCIOT = "";

            if (motorista.Empresa != null && motorista.Empresa.Configuracao != null && motorista.Empresa.Configuracao.TipoIntegradoraCIOT != null && motorista.Empresa.Configuracao.TipoIntegradoraCIOT.HasValue)
            {
                if (motorista.DataVencimentoHabilitacao == DateTime.MinValue)
                {
                    erroValidacaoCIOT = Localization.Resources.Transportadores.Motorista.DataValidadeCNHObrigatorio;
                    return false;
                }

                if (motorista.DataNascimento == DateTime.MinValue)
                {
                    erroValidacaoCIOT = Localization.Resources.Transportadores.Motorista.DataDeNascimentoObrigatorio;
                    return false;
                }

                if (string.IsNullOrEmpty(motorista.Telefone))
                {
                    erroValidacaoCIOT = Localization.Resources.Transportadores.Motorista.TelefoneObrigatorio;
                    return false;
                }
            }

            return true;
        }

        private string ObterCaminhoArquivoFoto(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" });
        }

        private string ObterCaminhoArquivoGaleria(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Galeria", "Motorista" });
        }

        private List<(string NomeArquivo, string Base64)> ObterFotosGaleria(int codigoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            List<(string NomeArquivo, string Base64)> listaArquivos = new List<(string NomeArquivo, string Base64)>();

            string caminho = ObterCaminhoArquivoGaleria(unitOfWork);

            try
            {
                Utilidades.IO.FileStorageService.Storage.CreateIfNotExists(caminho);
            }
            catch (Exception e)
            {
                Log.TratarErro(e);
            }

            string[] arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigoMotorista}_*.*").ToArray();

            for (int i = 0; i < arquivos.Length; i++)
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivos[i]);
                listaArquivos.Add((arquivos[i], Convert.ToBase64String(imageArray)));
            }

            return listaArquivos;
        }

        private string ObterFotoBase64(int codigoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivoFoto(unitOfWork);

            try
            {
                Utilidades.IO.FileStorageService.Storage.CreateIfNotExists(caminho);
            }
            catch (Exception e)
            {
                Log.TratarErro(e);
            }

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigoMotorista}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                return Usuario.Empresa;
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                int codigoEmpresa = Request.GetIntParam("Empresa");

                if (codigoEmpresa > 0)
                {
                    Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                    return repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);
                }
            }

            return null;
        }

        private void SalvarLicencas(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork, bool atualizando = false)
        {
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repLicencaMotorista = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> licencas = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();

            dynamic dynLicencas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridMotoristaLicencas"));

            if (motorista.Licencas?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic licenca in dynLicencas)
                    if (licenca.Codigo != null)
                        codigos.Add((int)licenca.Codigo);

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> licencasDeletar = (from obj in motorista.Licencas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < licencasDeletar.Count; i++)
                    repLicencaMotorista.Deletar(licencasDeletar[i]);
            }
            else
                motorista.Licencas = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();

            foreach (dynamic dynLicenca in dynLicencas)
            {
                int codigoLicencaMotorista = ((string)dynLicenca.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca licenca = codigoLicencaMotorista > 0 ? repLicencaMotorista.BuscarPorCodigo(codigoLicencaMotorista, false) : null;
                if (licenca == null)
                    licenca = new Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca();

                int.TryParse((string)dynLicenca.CodigoLicenca, out int codigoLicenca);
                DateTime.TryParse((string)dynLicenca.DataEmissao, out DateTime dataEmissao);
                DateTime.TryParse((string)dynLicenca.DataVencimento, out DateTime dataVencimento);
                Enum.TryParse((string)dynLicenca.Status, out StatusLicenca status);

                licenca.Licenca = codigoLicenca > 0 ? repLicenca.BuscarPorCodigo(codigoLicenca) : null;
                licenca.DataEmissao = dataEmissao;
                licenca.DataVencimento = dataVencimento;
                licenca.Descricao = (string)dynLicenca.Descricao;
                licenca.Numero = (string)dynLicenca.Numero;
                licenca.Motorista = motorista;
                licenca.Status = status;
                licenca.BloquearCriacaoPedidoLicencaVencida = ((string)dynLicenca.BloquearCriacaoPedidoLicencaVencida).ToBool();
                licenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida = ((string)dynLicenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida).ToBool();

                dynamic dynFormasAlerta = JsonConvert.DeserializeObject<dynamic>((string)dynLicenca.FormaAlerta);
                licenca.FormasAlerta = new List<ControleAlertaForma>();
                if (dynFormasAlerta?.Count > 0)
                {
                    foreach (dynamic codigoFormaAlerta in dynFormasAlerta)
                        licenca.FormasAlerta.Add(((string)codigoFormaAlerta).ToEnum<ControleAlertaForma>());
                }

                licencas.Add(licenca);

                if (licenca.Codigo > 0)
                    repLicencaMotorista.Atualizar(licenca, Auditado);
                else
                    repLicencaMotorista.Inserir(licenca, Auditado);
            }

            if (atualizando && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                !(licencas.Count > 0 && !licencas.Any(obj => obj.DataVencimento.HasValue && obj.DataVencimento.Value.Date >= DateTime.Now.Date) && licencas.Any(obj => obj.Licenca != null && obj.Licenca.BloquearCheckListComLicencaInvalida)))
            {
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> checklistsParaLiberarFluxo = motorista.Codigo > 0 ? repositorioChecklist.BuscarChecklistsPendentesPorMotorista(motorista.Codigo) : new List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>();
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist in checklistsParaLiberarFluxo)
                {
                    checklist.Situacao = SituacaoCheckList.Finalizado;
                    repositorioChecklist.Atualizar(checklist);

                    if ((checklist.FluxoGestaoPatio != null) && (checklist.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.CheckList))
                        servicoFluxoGestaoPatio.LiberarProximaEtapa(checklist.FluxoGestaoPatio, EtapaFluxoGestaoPatio.CheckList);
                }
            }
        }

        private void SalvarLiberacoesGR(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Repositorio.Embarcador.Transportadores.MotoristaLiberacaoGR repLiberacaoGR = new Repositorio.Embarcador.Transportadores.MotoristaLiberacaoGR(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR> liberacoesGR = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR>();

                dynamic dynLiberacoesGR = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridMotoristaLiberacoesGR"));

                if (motorista.LiberacoesGR?.Count > 0)
                {
                    List<int> codigos = new List<int>();
                    foreach (dynamic liberacaoGR in dynLiberacoesGR)
                        if (liberacaoGR.Codigo != null)
                            codigos.Add((int)liberacaoGR.Codigo);

                    List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR> liberacoesGRDeletar = (from obj in motorista.LiberacoesGR where !codigos.Contains(obj.Codigo) select obj).ToList();

                    for (int i = 0; i < liberacoesGRDeletar.Count; i++)
                        repLiberacaoGR.Deletar(liberacoesGRDeletar[i]);
                }
                else
                    motorista.LiberacoesGR = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR>();

                foreach (dynamic dynLiberacaoGR in dynLiberacoesGR)
                {
                    int codigoLiberacaoGRMotorista = ((string)dynLiberacaoGR.Codigo).ToInt();

                    Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR liberacaoGR = codigoLiberacaoGRMotorista > 0 ? repLiberacaoGR.BuscarPorCodigo(codigoLiberacaoGRMotorista, false) : null;
                    if (liberacaoGR == null)
                        liberacaoGR = new Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR();

                    int.TryParse((string)dynLiberacaoGR.CodigoLicenca, out int codigoLicenca);
                    DateTime.TryParse((string)dynLiberacaoGR.DataEmissao, out DateTime dataEmissao);
                    DateTime.TryParse((string)dynLiberacaoGR.DataVencimento, out DateTime dataVencimento);

                    liberacaoGR.Licenca = codigoLicenca > 0 ? repLicenca.BuscarPorCodigo(codigoLicenca) : null;
                    liberacaoGR.DataEmissao = dataEmissao;
                    liberacaoGR.DataVencimento = dataVencimento;
                    liberacaoGR.Descricao = (string)dynLiberacaoGR.Descricao;
                    liberacaoGR.Numero = (string)dynLiberacaoGR.Numero;
                    liberacaoGR.Motorista = motorista;

                    liberacoesGR.Add(liberacaoGR);

                    if (liberacaoGR.Codigo > 0)
                        repLiberacaoGR.Atualizar(liberacaoGR, Auditado);
                    else
                        repLiberacaoGR.Inserir(liberacaoGR, Auditado);
                }
            }
        }
        private void SalvarDadosBancarios(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Embarcador.Transportadores.MotoristaDadoBancario repMotoristaDadoBancario = new Repositorio.Embarcador.Transportadores.MotoristaDadoBancario(unitOfWork);

            dynamic dynDadosBancarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridMotoristaDadoBancarios"));

            if (motorista.DadosBancarios != null && motorista.DadosBancarios.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoBancario in dynDadosBancarios)
                    if (dadoBancario.Codigo != null)
                        codigos.Add((int)dadoBancario.Codigo);

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario> dadoBancarioMotoristaDeletar = (from obj in motorista.DadosBancarios where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < dadoBancarioMotoristaDeletar.Count; i++)
                    repMotoristaDadoBancario.Deletar(dadoBancarioMotoristaDeletar[i], Auditado);
            }
            else
                motorista.DadosBancarios = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario>();

            foreach (var dadoBancario in dynDadosBancarios)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario motoristaDadoBancario = dadoBancario.Codigo != null ? repMotoristaDadoBancario.BuscarPorCodigo((int)dadoBancario.Codigo, true) : null;
                if (motoristaDadoBancario == null)
                    motoristaDadoBancario = new Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario();

                int.TryParse((string)dadoBancario.CodigoBanco, out int codigoBanco);
                Enum.TryParse((string)dadoBancario.TipoContaBanco, out TipoContaBanco tipoContaBanco);
                Enum.TryParse((string)dadoBancario.TipoChavePix, out TipoChavePix tipoChavePix);

                motoristaDadoBancario.Agencia = (string)dadoBancario.Agencia;
                motoristaDadoBancario.DigitoAgencia = (string)dadoBancario.DigitoAgencia;
                motoristaDadoBancario.NumeroConta = (string)dadoBancario.NumeroConta;
                motoristaDadoBancario.ObservacaoConta = (string)dadoBancario.ObservacaoConta;
                motoristaDadoBancario.Motorista = motorista;
                motoristaDadoBancario.TipoContaBanco = tipoContaBanco;
                motoristaDadoBancario.TipoChavePix = tipoChavePix;
                motoristaDadoBancario.ChavePix = (string)dadoBancario.ChavePix;


                if (codigoBanco > 0)
                    motoristaDadoBancario.Banco = repBanco.BuscarPorCodigo(codigoBanco);
                else
                    motoristaDadoBancario.Banco = null;

                if (motoristaDadoBancario.Codigo > 0)
                    repMotoristaDadoBancario.Atualizar(motoristaDadoBancario, Auditado);
                else
                    repMotoristaDadoBancario.Inserir(motoristaDadoBancario, Auditado);
            }
        }

        private void SalvarContatos(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioContato repFuncionarioContato = new Repositorio.Embarcador.Usuarios.FuncionarioContato(unitOfWork);

            dynamic dynContatos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridMotoristaContatos"));

            if (motorista.Contatos != null && motorista.Contatos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoContato in dynContatos)
                    if (dadoContato.Codigo != null)
                        codigos.Add((int)dadoContato.Codigo);

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato> contatoMotoristaDeletar = (from obj in motorista.Contatos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < contatoMotoristaDeletar.Count; i++)
                    repFuncionarioContato.Deletar(contatoMotoristaDeletar[i], Auditado);
            }
            else
                motorista.Contatos = new List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato>();

            foreach (var contato in dynContatos)
            {
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato motoristaContato = contato.Codigo != null ? repFuncionarioContato.BuscarPorCodigo((int)contato.Codigo, true) : null;
                if (motoristaContato == null)
                    motoristaContato = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato();

                Enum.TryParse((string)contato.TipoParentesco, out TipoParentesco tipoParentesco);

                motoristaContato.Nome = (string)contato.Nome;
                motoristaContato.Email = (string)contato.Email;
                motoristaContato.Telefone = (string)contato.Telefone;
                motoristaContato.Usuario = motorista;
                motoristaContato.TipoParentesco = tipoParentesco;
                motoristaContato.CPF = ((string)contato.CPF).ObterSomenteNumeros();
                motoristaContato.DataNascimento = ((string)contato.DataNascimento).ToNullableDateTime();

                if (motoristaContato.Codigo > 0)
                    repFuncionarioContato.Atualizar(motoristaContato, Auditado);
                else
                    repFuncionarioContato.Inserir(motoristaContato, Auditado);
            }
        }

        private void SalvarTransportadoras(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            if (motorista.Empresas?.Count > 0)
                motorista.Empresas.Clear();
            else
                motorista.Empresas = new List<Dominio.Entidades.Empresa>();

            dynamic dynTransportadoras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Transportadoras"));
            foreach (var dynTransportador in dynTransportadoras)
                motorista.Empresas.Add(repEmpresa.BuscarPorCodigo((int)dynTransportador.Codigo));
        }

        private void SalvarLocaisCarregamentoAutorizados(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado repMotoristaLocalCarregamentoAutorizado = new Repositorio.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic dynMotoristaLocaisCarregamentoAutorizados = JsonConvert.DeserializeObject<dynamic>(Request.Params("LocaisCarregamentosAutorizados"));

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado> motoristaLocaisCarregamentoAutorizados = repMotoristaLocalCarregamentoAutorizado.BuscarPorMotorista(motorista.Codigo);

            if (motoristaLocaisCarregamentoAutorizados.Count > 0)
            {
                List<double> codigos = new List<double>();

                foreach (var localCarregamentoAutorizado in dynMotoristaLocaisCarregamentoAutorizados)
                {
                    double codigoLocalCarregamentoAutorizado = ((string)localCarregamentoAutorizado.Codigo).ToDouble();
                    if (codigoLocalCarregamentoAutorizado > 0)
                        codigos.Add(codigoLocalCarregamentoAutorizado);
                }

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado> listaDeletar = (from obj in motoristaLocaisCarregamentoAutorizados where !codigos.Contains(obj.Cliente.CPF_CNPJ) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado deletar in listaDeletar)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, $"Removeu o local de carregamento {deletar.Cliente.Descricao} autorizado do motorista {deletar.Motorista.Descricao}.", unitOfWork);
                    repMotoristaLocalCarregamentoAutorizado.Deletar(deletar);
                }
            }

            foreach (var localCarregamentoAutorizado in dynMotoristaLocaisCarregamentoAutorizados)
            {
                double codigoLocalCarregamentoAutorizado = ((string)localCarregamentoAutorizado.Codigo).ToDouble();

                Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado motoristaLocalCarregamentoAutorizadoCliente = repMotoristaLocalCarregamentoAutorizado.BuscarPorMotoristaECliente(motorista.Codigo, codigoLocalCarregamentoAutorizado);

                if (motoristaLocalCarregamentoAutorizadoCliente == null)
                {
                    motoristaLocalCarregamentoAutorizadoCliente = new Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado();

                    motoristaLocalCarregamentoAutorizadoCliente.Motorista = motorista;
                    motoristaLocalCarregamentoAutorizadoCliente.Cliente = repCliente.BuscarPorCPFCNPJ(codigoLocalCarregamentoAutorizado);

                    repMotoristaLocalCarregamentoAutorizado.Inserir(motoristaLocalCarregamentoAutorizadoCliente);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, $"Adicionou o local de carregamento {motoristaLocalCarregamentoAutorizadoCliente.Cliente.Descricao} autorizado ao motorista {motoristaLocalCarregamentoAutorizadoCliente.Motorista.Descricao}.", unitOfWork);
                }
            }
        }

        private void SalvarEPIs(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.FuncionarioEPI repositorioMotoristaEPI = new Repositorio.Embarcador.Usuarios.FuncionarioEPI(unitOfWork);
            Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);

            List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> motoristaEPIs = repositorioMotoristaEPI.BuscarPorUsuario(motorista);

            dynamic dynEPIs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridEPIs"));

            if (motoristaEPIs.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoContato in dynEPIs)
                    if (dadoContato.Codigo != null)
                        codigos.Add((int)dadoContato.Codigo);

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> epiMotoristaDeletar = (from obj in motoristaEPIs where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < epiMotoristaDeletar.Count; i++)
                    repositorioMotoristaEPI.Deletar(epiMotoristaDeletar[i], Auditado);
            }
            else
                motoristaEPIs = new List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI>();

            foreach (var epi in dynEPIs)
            {
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI motoristaEPI = epi.Codigo != null ? repositorioMotoristaEPI.BuscarPorCodigo((int)epi.Codigo, true) : null;
                if (motoristaEPI == null)
                    motoristaEPI = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI();

                int.TryParse((string)epi.CodigoEPI, out int codigoEPI);
                int.TryParse((string)epi.Quantidade, out int quantidade);
                DateTime.TryParse((string)epi.DataRepasse, out DateTime dataRepasse);

                motoristaEPI.EPI = codigoEPI > 0 ? repositorioEPI.BuscarPorCodigo(codigoEPI, false) : null;
                motoristaEPI.Usuario = motorista;
                motoristaEPI.DataRepasse = dataRepasse;
                motoristaEPI.SerieEPI = (string)epi.SerieEPI;
                motoristaEPI.Quantidade = quantidade;

                if (motoristaEPI.Codigo > 0)
                    repositorioMotoristaEPI.Atualizar(motoristaEPI, Auditado);
                else
                    repositorioMotoristaEPI.Inserir(motoristaEPI, Auditado);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioConfiguracaoMotorista.BuscarPrimeiroRegistro();

            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista()
            {
                SituacaoColaborador = Request.GetEnumParam("SituacaoColaborador", SituacaoColaborador.Todos),
                Nome = Request.GetStringParam("Nome"),
                CpfCnpj = Request.GetStringParam("CPF"),
                Tipo = "M",
                Status = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Todos),
                PlacaVeiculo = Request.GetStringParam("Placa"),
                CodigoCargo = Request.GetIntParam("CargoMotorista"),
                ProprietarioTerceiro = Request.GetDoubleParam("Proprietario"),
                SomentePendenteDeVinculo = Request.GetBoolParam("SomentePedentes"),
                NumeroMatricula = Request.GetStringParam("NumeroMatricula"),
                CnpjRemetenteLocalCarregamentoAutorizado = Request.GetDoubleParam("RemetenteLocalCarregamentoAutorizado"),
                NaoBloqueado = Request.GetBoolParam("NaoBloqueado"),
                TipoMotorista = Request.GetEnumParam<TipoMotorista>("TipoMotorista"),
                TipoMotoristaAjudante = Request.GetEnumParam<TipoMotoristaAjudante>("TipoMotoristaAjudante"),
                NaoAjudante = Request.GetNullableBoolParam("NaoAjudante"),
                NumeroFrota = Request.GetStringParam("NumeroFrota")
            };

            int codigoEmpresa = Request.GetIntParam("Empresa");
            filtrosPesquisa.CpfCnpj = configuracaoMotorista.PermitirCadastrarMotoristaEstrangeiro ? filtrosPesquisa.CpfCnpj : Utilidades.String.OnlyNumbers(filtrosPesquisa.CpfCnpj);
            filtrosPesquisa.TipoServicoMultisoftware = TipoServicoMultisoftware;

            if (filtrosPesquisa.NaoAjudante.HasValue && filtrosPesquisa.TipoMotoristaAjudante != TipoMotoristaAjudante.Todos)
            {
                filtrosPesquisa.TipoMotoristaAjudante = (filtrosPesquisa.NaoAjudante.Value ? TipoMotoristaAjudante.Motorista : TipoMotoristaAjudante.Ajudante);
            }

            Dominio.Entidades.Empresa empresa = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                if (Usuario.ClienteTerceiro != null)
                    empresa = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                if (empresa == null)
                    throw new ControllerException(Localization.Resources.Transportadores.Motorista.TransportadorTerceiroNaoCadastradoComoEmpresa);
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                empresa = Usuario.Empresa;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
            {
                filtrosPesquisa.ProprietarioTerceiro = Usuario?.ClienteTerceiro.CPF_CNPJ ?? 0;
            }
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && codigoEmpresa > 0)
            {
                empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            }

            filtrosPesquisa.Empresa = empresa;

            if (filtrosPesquisa.CodigosEmpresa == null || filtrosPesquisa.CodigosEmpresa.Count == 0)
                filtrosPesquisa.CodigosEmpresa = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private List<Dominio.Entidades.Usuario> ValidacaoDuplicidade(List<Dominio.Entidades.Usuario> motoristas)
        {
            List<Dominio.Entidades.Usuario> usuarioSemDuplicidade = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Usuario motorita in motoristas)
            {
                bool existeMotoristaMesmaEmpresa = false;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    existeMotoristaMesmaEmpresa = usuarioSemDuplicidade.Any(m => m.CPF == motorita.CPF);
                else
                    existeMotoristaMesmaEmpresa = usuarioSemDuplicidade.Any(m => m.CPF == motorita.CPF && m.Empresa.Codigo == motorita.Empresa.Codigo);
                if (existeMotoristaMesmaEmpresa)
                    continue;

                usuarioSemDuplicidade.Add(motorita);
            }

            return usuarioSemDuplicidade;
        }

        private void ValidacaoParaBloquearMotoristaOutrosCadastro(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracao = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();

            if (!motorista.Bloqueado && !configuracao.NaoPermitirRealizarCadastroMotoristaBloqueado)
                return;

            List<Dominio.Entidades.Usuario> motoristasMesmoCpf = repMotorista.BuscarTodosMotoristasPorCPF(motorista.CPF);

            foreach (Dominio.Entidades.Usuario mesmoMotoristaOutroCadastro in motoristasMesmoCpf)
            {
                mesmoMotoristaOutroCadastro.Bloqueado = motorista.Bloqueado;
                repMotorista.Atualizar(mesmoMotoristaOutroCadastro);
            }

        }

        private TipoMotoristaAjudante ObterTipoMotoristaAjudante(bool? ajudante)
        {
            if (ajudante.HasValue)
            {
                if (ajudante.Value)
                {
                    return TipoMotoristaAjudante.Ajudante;
                }

                return TipoMotoristaAjudante.Motorista;
            }

            return TipoMotoristaAjudante.Todos;
        }

        #endregion
    }
}

