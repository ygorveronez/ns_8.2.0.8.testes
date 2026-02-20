using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ClienteURLAcesso")]
    public class ClienteURLAcessoController : BaseController
    {
        #region Métodos Globais     
        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCliente = int.Parse(Request.Params["Cliente"]);
                string urlAcesso = Request.Params["URLAcesso"];
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico;
                Enum.TryParse(Request.Params["TipoServico"], out tipoServico);
                AdminMultisoftware.Dominio.Enumeradores.SituacaoAtivoPesquisa situacao;
                Enum.TryParse(Request.Params["Ativo"], out situacao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "RazaoSocial", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("URL De Acesso ", "URLAcesso", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Serviço", "DescricaoTipoServicoMultisoftware", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "AtivoFormatado", 30, Models.Grid.Align.left, true);

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso> listaClienteURLAcesso = repClienteURLAcesso.Consultar(codigoCliente, urlAcesso, tipoServico, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repClienteURLAcesso.ContarConsulta(codigoCliente, urlAcesso, tipoServico, situacao));
                var lista = (from p in listaClienteURLAcesso
                             select new
                             {
                                 p.Codigo,
                                 p.Cliente.RazaoSocial,
                                 p.URLAcesso,
                                 p.TipoServicoMultisoftware,
                                 p.DescricaoTipoServicoMultisoftware,
                                 p.AtivoFormatado,
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

        public ActionResult Adicionar()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCliente;
                int.TryParse(Request.Params["Cliente"], out codigoCliente);
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico;
                Enum.TryParse(Request.Params["TipoServico"], out tipoServico);

                unitOfWork.Start();
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = new AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso();

                clienteURLAcesso.Cliente = codigoCliente > 0 ? new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = codigoCliente } : null;
                clienteURLAcesso.TipoServicoMultisoftware = tipoServico;
                clienteURLAcesso.URLAcesso = Request.Params["URLAcesso"];
                clienteURLAcesso.Ativo = bool.Parse(Request.Params["Ativo"]);
                clienteURLAcesso.UriSistemaEmissaoCTe = Request.Params["URLSistemaEmissaoCte"];
                clienteURLAcesso.WebServiceConsultaCTe = Request.Params["URLWebServiceConsultaCte"];
                clienteURLAcesso.WebServiceOracle = Request.Params["URLWebServiceOracle"];
                clienteURLAcesso.Layout = Request.Params["Layout"];
                clienteURLAcesso.Logo = Request.Params["Logo"];
                clienteURLAcesso.CorFundoUsuario = Request.Params["CorFuncionario"];
                clienteURLAcesso.Favicon = Request.Params["Icone"];
                clienteURLAcesso.LayoutLogin = Request.Params["LayoutLogin"];
                clienteURLAcesso.LogoLogin = Request.Params["LogoLogin"];
                clienteURLAcesso.CorFundoUsuarioLogin = Request.Params["CorFuncionarioLogin"];
                clienteURLAcesso.FaviconLogin = Request.Params["IconeLogin"];
                clienteURLAcesso.PossuiFila = bool.Parse(Request.Params["PossuiFila"]);
                clienteURLAcesso.URLHomologacao = bool.Parse(Request.Params["URLHomologacao"]);

                repClienteURLAcesso.Inserir(clienteURLAcesso, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult Atualizar()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));

                int codigoCliente;
                int.TryParse(Request.Params["Cliente"], out codigoCliente);
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico;
                Enum.TryParse(Request.Params["TipoServico"], out tipoServico);

                clienteURLAcesso.Initialize();
                clienteURLAcesso.Cliente = codigoCliente > 0 ? new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = codigoCliente } : null;
                clienteURLAcesso.TipoServicoMultisoftware = tipoServico;
                clienteURLAcesso.URLAcesso = Request.Params["URLAcesso"];
                clienteURLAcesso.Ativo = bool.Parse(Request.Params["Ativo"]);
                clienteURLAcesso.UriSistemaEmissaoCTe = Request.Params["URLSistemaEmissaoCte"];
                clienteURLAcesso.WebServiceConsultaCTe = Request.Params["URLWebServiceConsultaCte"];
                clienteURLAcesso.WebServiceOracle = Request.Params["URLWebServiceOracle"];
                clienteURLAcesso.Layout = Request.Params["Layout"];
                clienteURLAcesso.Logo = Request.Params["Logo"];
                clienteURLAcesso.CorFundoUsuario = Request.Params["CorFuncionario"];
                clienteURLAcesso.Favicon = Request.Params["Icone"];
                clienteURLAcesso.LayoutLogin = Request.Params["LayoutLogin"];
                clienteURLAcesso.LogoLogin = Request.Params["LogoLogin"];
                clienteURLAcesso.CorFundoUsuarioLogin = Request.Params["CorFuncionarioLogin"];
                clienteURLAcesso.FaviconLogin = Request.Params["IconeLogin"];
                clienteURLAcesso.PossuiFila = bool.Parse(Request.Params["PossuiFila"]);
                clienteURLAcesso.URLHomologacao = bool.Parse(Request.Params["URLHomologacao"]);

                repClienteURLAcesso.Atualizar(clienteURLAcesso, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult BuscarPorCodigo()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params["Codigo"]);

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorCodigo(codigo);
                var dynClienteURLAcesso = new
                {
                    clienteURLAcesso.Codigo,
                    Cliente = new { clienteURLAcesso.Cliente.Codigo, Descricao = clienteURLAcesso.Cliente.RazaoSocial },
                    TipoServico = clienteURLAcesso.TipoServicoMultisoftware,
                    clienteURLAcesso.URLAcesso,
                    clienteURLAcesso.Ativo,
                    URLSistemaEmissaoCte = clienteURLAcesso.UriSistemaEmissaoCTe,
                    URLWebServiceConsultaCte = clienteURLAcesso.WebServiceConsultaCTe,
                    URLWebServiceOracle = clienteURLAcesso.WebServiceOracle,
                    clienteURLAcesso.Layout,
                    clienteURLAcesso.Logo,
                    CorFuncionario = clienteURLAcesso.CorFundoUsuario,
                    Icone = clienteURLAcesso.Favicon,
                    clienteURLAcesso.LayoutLogin,
                    clienteURLAcesso.LogoLogin,
                    CorFuncionarioLogin = clienteURLAcesso.CorFundoUsuarioLogin,
                    IconeLogin = clienteURLAcesso.FaviconLogin,
                    clienteURLAcesso.PossuiFila,
                    clienteURLAcesso.URLHomologacao
                };
                return new JsonpResult(dynClienteURLAcesso);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public ActionResult ExcluirPorCodigo()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params["Codigo"]);
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorCodigo(codigo);
                repClienteURLAcesso.Deletar(clienteURLAcesso, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vinculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}