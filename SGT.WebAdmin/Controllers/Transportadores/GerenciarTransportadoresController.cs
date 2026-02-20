using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Net;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "Consultar", "BuscarDadosParaAcesso" }, "Transportadores/GerenciarTransportadores")]
    public class GerenciarTransportadoresController : BaseController
    {
        #region Construtores

        public GerenciarTransportadoresController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.Params("Nome");
                string nomeFantasia = Request.Params("NomeFantasia");
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));
                string placaVeiculo = Request.Params("Placa");
                string status = "A";// Request.Params("Status");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("CodigoCriptografado", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Codigo, "Codigo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.RazaoSocial, "RazaoSocial", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.NomeFantasia, "NomeFantasia", 15, Models.Grid.Align.left, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CNPJCPF, "CNPJ", 15, Models.Grid.Align.left);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.CNPJ, "CNPJ", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Localidade, "Localidade", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Telefone, "Telefone", 10, Models.Grid.Align.left);

                List<int> codigosEmpresa = new List<int>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    codigosEmpresa = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unidadeDeTrabalho);

                IList<Dominio.Entidades.Empresa> listaEmpresas = null;
                int countEmpresas = 0;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                    (this.Usuario.Empresa.CNPJ == "16482040000157") || (this.Usuario.Empresa.CNPJ == "13969629000196" && Request.Headers["Referer"].ToString().Contains("Transportadores/GerenciarTransportadores")))
                {
                    listaEmpresas = repEmpresa.Consultar(nome, cnpj, placaVeiculo, status, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena,
                        false, Dominio.Enumeradores.TipoAmbiente.Producao, nomeFantasia, codigosEmpresa);
                    countEmpresas = repEmpresa.ContarConsulta(nome, cnpj, placaVeiculo, status, false, Dominio.Enumeradores.TipoAmbiente.Producao, nomeFantasia, codigosEmpresa);
                }
                else
                {
                    listaEmpresas = repEmpresa.Consultar(this.Usuario.Empresa.Codigo, nome, cnpj, placaVeiculo, status, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena,
                        false, Dominio.Enumeradores.TipoAmbiente.Producao, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, 0, nomeFantasia);
                    countEmpresas = repEmpresa.ContarConsulta(this.Usuario.Empresa.Codigo, nome, cnpj, placaVeiculo, status,
                        false, Dominio.Enumeradores.TipoAmbiente.Producao, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, 0, nomeFantasia);
                }

                grid.setarQuantidadeTotal(countEmpresas);

                grid.AdicionaRows((from obj in listaEmpresas
                                   select new
                                   {
                                       CodigoCriptografado = WebUtility.UrlEncode(Servicos.Criptografia.Criptografar(obj.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3")),
                                       obj.Codigo,
                                       obj.RazaoSocial,
                                       obj.NomeFantasia,
                                       Localidade = obj.Localidade.DescricaoCidadeEstado,
                                       CNPJ = obj.CNPJ_Formatado,
                                       obj.Telefone
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterOsDadosDasEmpresas);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> BuscarDadosParaAcesso()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
#if !DEBUG
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    if (this.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Embarcador)
                        return new JsonpResult(false, "Acesso negado.");
#endif

                int codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                if (codigoEmpresa <= 0)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.EmpresaNaoEncontrada);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresa, this.Empresa.Codigo);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPrimeiroPorEmpresa(codigoEmpresa, Dominio.Enumeradores.TipoAcesso.Emissao);

                if (usuario == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.NenhumUsuarioEncontradoParaEstaEmpresa);

                Dominio.ObjetosDeValor.ChaveAcesso chave = new Dominio.ObjetosDeValor.ChaveAcesso();
                chave.Login = System.Web.HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(usuario.Login, string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"))));
                chave.Senha = System.Web.HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(usuario.Senha, string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"))));
                chave.Usuario = System.Web.HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(this.Usuario.Codigo.ToString(), string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"))));

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin ||
                    TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    Dominio.Entidades.Empresa empresaAcessar = usuario.Empresa;

                    Servicos.Embarcador.Login.Login svcLogin = new Servicos.Embarcador.Login.Login(unidadeDeTrabalho);
                    svcLogin.ValidarAcessoEmpresaCommerce(empresaAcessar, unidadeDeTrabalho, TipoServicoMultisoftware);

                    if (empresaAcessar.StatusFinanceiro == "B")
                        return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.EmpresaComPendenciasContateSetorDoSuporteParaMaioresInformacoes);
                    else
                    {
                        chave.UriAcesso = string.Concat(ClienteAcesso.UriSistemaEmissaoCTe, "EncodedLogin");
#if DEBUG
                        chave.UriAcesso = string.Concat("http://localhost:1736/", "EncodedLogin");//Testar a conexão debugando aqui interno
#endif
                    }
                }
                else
                {
                    chave.UriAcesso = string.Concat(ClienteAcesso.UriSistemaEmissaoCTe, "/EncodedLogin.aspx");
#if DEBUG
                    chave.UriAcesso = string.Concat("http://localhost:2324", "/EncodedLogin.aspx");
#endif
                }

                return new JsonpResult(chave);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterOsDadosParaAcessoAoSistema);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
