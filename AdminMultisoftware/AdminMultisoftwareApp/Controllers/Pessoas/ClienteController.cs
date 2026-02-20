using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Cliente")]
    public class ClienteController : BaseController
    {
        #region Métodos Globais

        public ActionResult Pesquisa()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string nome = Request.Params["RazaoSocial"];
                string cnpj = ObterSomenteNumeros(Request.Params["CNPJ"]);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "RazaoSocial", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CNPJ", "CNPJ", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descricao", false);

                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWork);
                List<AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente> listaCliente = repCliente.Consultar(nome, cnpj, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCliente.ContarConsulta(nome, cnpj));
                var lista = (from p in listaCliente
                             select new
                             {
                                 p.Codigo,
                                 p.RazaoSocial,
                                 CNPJ = p.CNPJFormatado,
                                 p.Descricao
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
                unitOfWork.Start();

                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente();

                PreencherCliente(cliente, unitOfWork);
                cliente.DataCadastro = DateTime.Now;

                repCliente.Inserir(cliente, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(int.Parse(Request.Params["Codigo"]));

                cliente.Initialize();
                PreencherCliente(cliente, unitOfWork);

                repCliente.Atualizar(cliente, Auditado);
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

                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigo);

                var resCliente = new
                {
                    cliente.Codigo,
                    cliente.RazaoSocial,
                    CNPJ = cliente.CNPJFormatado,
                    IE = cliente.InscricaoEstadual,
                    cliente.NomeFantasia,
                    Telefone = cliente.TelefoneFormatado,
                    cliente.Email,
                    cliente.Ativo,
                    cliente.TipoOperadora,
                    cliente.Logradouro,
                    cliente.Bairro,
                    CEP = cliente.CEPFormatado,
                    cliente.Complemento,
                    cliente.Numero,
                    MobileURL = cliente.UrlMobile,
                    MobileURLHomologacao = cliente.UrlMobileHomologacao,
                    cliente.Site,
                    cliente.Logo,
                    cliente.LogoLight,
                    cliente.HeightLogo,
                    BloquearLoginVersaoAntiga = cliente.BloquearLoginVersaoAntigaAPP,
                    ConfiguracaoCliente = new
                    {
                        ProducaoBase = cliente.ClienteConfiguracao.DBBase,
                        ProducaoInstanciaBase = new { cliente.ClienteConfiguracao.InstanciaBase?.Codigo, Descricao = cliente.ClienteConfiguracao.InstanciaBase?.Descricao },
                        ProducaoTipoServico = cliente.ClienteConfiguracao.TipoServicoMultisoftware,
                        HomologacaoBase = cliente.ClienteConfiguracaoHomologacao?.DBBase ?? string.Empty,
                        HomologacaoInstanciaBase = cliente.ClienteConfiguracaoHomologacao != null ? new { cliente.ClienteConfiguracaoHomologacao.InstanciaBase?.Codigo, Descricao = cliente.ClienteConfiguracaoHomologacao.InstanciaBase?.Descricao } : null,
                        HomologacaoTipoServico = cliente.ClienteConfiguracaoHomologacao?.TipoServicoMultisoftware ?? TipoServicoMultisoftware.MultiEmbarcador
                    },
                };
                return new JsonpResult(resCliente);
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
                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWork);
                AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracao repClienteConfiguracao = new AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracao(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigo);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao clienteConfiguracaoProducao = repClienteConfiguracao.BuscarPorCodigo(cliente.ClienteConfiguracao.Codigo);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao clienteConfiguracaoHomologacao = cliente.ClienteConfiguracaoHomologacao != null ? repClienteConfiguracao.BuscarPorCodigo(cliente.ClienteConfiguracaoHomologacao.Codigo) : null;

                repCliente.Deletar(cliente, Auditado);
                repClienteConfiguracao.Deletar(clienteConfiguracaoProducao);
                if (clienteConfiguracaoHomologacao != null)
                    repClienteConfiguracao.Deletar(clienteConfiguracaoHomologacao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #region Metódos Privados 

        private void PreencherCliente(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {

            TipoOperadora tipoOperadora;
            Enum.TryParse(Request.Params["TipoOperadora"], out tipoOperadora);

            int heightLogo;
            int.TryParse(Request.Params["HeightLogo"], out heightLogo);

            string telefone = ObterSomenteNumeros(Request.Params["Telefone"]);

            cliente.RazaoSocial = Request.Params["RazaoSocial"];
            cliente.TipoOperadora = tipoOperadora;
            cliente.CNPJ = ObterSomenteNumeros(Request.Params["CNPJ"]);
            cliente.InscricaoEstadual = Request.Params["IE"];
            cliente.NomeFantasia = Request.Params["NomeFantasia"];
            cliente.DDDTelefone = telefone.Substring(0, 2);
            cliente.Telefone = telefone.Substring(2, telefone.Length - 2);
            cliente.Email = Request.Params["Email"];
            cliente.Ativo = bool.Parse(Request.Params["Ativo"]);
            cliente.Logradouro = Request.Params["Logradouro"];
            cliente.Bairro = Request.Params["Bairro"];
            cliente.CEP = ObterSomenteNumeros(Request.Params["CEP"]);
            cliente.Complemento = Request.Params["Complemento"];
            cliente.Numero = Request.Params["Numero"];
            cliente.Localidade = new AdminMultisoftware.Dominio.Entidades.Localidades.Localidade() { Codigo = 1 };
            cliente.ClienteConfiguracao = SalvarClienteConfiguracaoProducao(cliente.ClienteConfiguracao, unitOfWork);
            cliente.ClienteConfiguracaoHomologacao = SalvarClienteConfiguracaoHomologacao(cliente.ClienteConfiguracaoHomologacao, unitOfWork);
            cliente.UrlMobile = Request.Params["MobileURL"];
            cliente.UrlMobileHomologacao = Request.Params["MobileURLHomologacao"];
            cliente.Site = Request.Params["Site"];
            cliente.Logo = Request.Params["Logo"];
            cliente.LogoLight = Request.Params["LogoLight"];
            cliente.HeightLogo = heightLogo;
            cliente.BloquearLoginVersaoAntigaAPP = bool.Parse(Request.Params["BloquearLoginVersaoAntiga"]);
            cliente.URLAutenticadaViaCodigoDeIntegracaoDoUsuarioParaPortalMultiClifor = bool.Parse(Request.Params["URLAutenticadaViaCodigoDeIntegracaoDoUsuarioParaPortalMultiClifor"]);
        }

        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao SalvarClienteConfiguracaoProducao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao clienteConfiguracao, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracao repClienteConfiguracao = new AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracao(unitOfWork);
            AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);
            dynamic dynClienteConfiguracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params["ClienteConfiguracao"]);

            if (clienteConfiguracao == null)
            {
                clienteConfiguracao = new AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao();
            }
            int codigoInstanciaBaseProd;
            int.TryParse((string)dynClienteConfiguracao.ProducaoInstanciaBase, out codigoInstanciaBaseProd);

            TipoServicoMultisoftware prodTipoServico;
            Enum.TryParse((string)dynClienteConfiguracao.ProducaoTipoServico, out prodTipoServico);

            clienteConfiguracao.DBBase = dynClienteConfiguracao.ProducaoBase;
            clienteConfiguracao.InstanciaBase = codigoInstanciaBaseProd > 0 ? repInstanciaBase.BuscarPorCodigo(codigoInstanciaBaseProd) : null;
            clienteConfiguracao.TipoServicoMultisoftware = prodTipoServico;

            if (clienteConfiguracao.Codigo > 0)
                repClienteConfiguracao.Atualizar(clienteConfiguracao);
            else
                repClienteConfiguracao.Inserir(clienteConfiguracao);

            return clienteConfiguracao;
        }

        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao SalvarClienteConfiguracaoHomologacao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao clienteConfiguracao, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracao repClienteConfiguracao = new AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracao(unitOfWork);
            AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase repInstanciaBase = new AdminMultisoftware.Repositorio.Configuracoes.InstanciaBase(unitOfWork);
            dynamic dynClienteConfiguracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params["ClienteConfiguracao"]);

            if (clienteConfiguracao == null)
            {
                clienteConfiguracao = new AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao();
            }

            int codigoInstanciaBaseHomo;
            int.TryParse((string)dynClienteConfiguracao.HomologacaoInstanciaBase, out codigoInstanciaBaseHomo);

            TipoServicoMultisoftware homoTipoServico;
            Enum.TryParse((string)dynClienteConfiguracao.ProducaoTipoServico, out homoTipoServico);

            clienteConfiguracao.DBBase = dynClienteConfiguracao.HomologacaoBase;
            clienteConfiguracao.InstanciaBase = codigoInstanciaBaseHomo > 0 ? repInstanciaBase.BuscarPorCodigo(codigoInstanciaBaseHomo) : null;
            clienteConfiguracao.TipoServicoMultisoftware = homoTipoServico;

            if (clienteConfiguracao.Codigo > 0)
                repClienteConfiguracao.Atualizar(clienteConfiguracao);
            else
                repClienteConfiguracao.Inserir(clienteConfiguracao);

            return clienteConfiguracao;
        }

        private string ObterSomenteNumeros(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            return System.Text.RegularExpressions.Regex.Replace(valor, "[^0-9]", "");
        }

        #endregion
    }
}