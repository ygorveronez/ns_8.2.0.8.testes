using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ContaBancaria")]
    public class ContaBancariaController : BaseController
    {
        #region Construtores

        public ContaBancariaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.ContaBancaria repContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);

                string agencia = Request.GetStringParam("Agencia");
                string numeroConta = Request.GetStringParam("NumeroConta");
                double portador = Request.GetDoubleParam("ClientePortadorConta");
                int banco = Request.GetIntParam("Banco");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco>("TipoConta");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Portador", "Portador", 70, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Banco", "Banco", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Agência", "Agencia", 70, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número da Conta", "NumeroConta", 70, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo de Conta", "TipoConta", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descricao", "Descricao", 20, Models.Grid.Align.center, true,false);


                List<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria> listaContaBancaria = repContaBancaria.Consultar(portador,banco,agencia,numeroConta,tipoConta, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContaBancaria.ContarConsulta(portador, banco, agencia, numeroConta, tipoConta));
                var lista = (from contaBancaria in listaContaBancaria
                             select new
                             {
                                 contaBancaria.Codigo,
                                 Portador = contaBancaria.ClientePortadorConta?.CPF_CNPJ_Formatado ?? "",
                                 Banco = contaBancaria.Banco?.Descricao ?? "",
                                 Agencia = contaBancaria.Agencia ?? "",
                                 NumeroConta = contaBancaria.NumeroConta ?? "",
                                 TipoConta = contaBancaria.TipoContaBanco.ObterDescricao() ?? "",
                                 Descricao = (contaBancaria.Banco?.Descricao ?? "") + " / " + (contaBancaria.NumeroConta ?? "")
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Cliente repClinte = new Repositorio.Cliente(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Embarcador.Financeiro.ContaBancaria repContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaBancaria contaBancaria = repContaBancaria.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                string agencia = Request.GetStringParam("Agencia");
                string numeroConta = Request.GetStringParam("NumeroConta");
                int portador = Request.GetIntParam("ClientePortadorConta");
                int banco = Request.GetIntParam("Banco");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco>("TipoConta");
                Dominio.ObjetosDeValor.Enumerador.TipoChavePix tipoChavePIX = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.TipoChavePix>("TipoChavePix");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracaoDadosBancarios");
                string digito = Request.GetStringParam("Digito");
                string chavepix = Request.GetStringParam("ChavePix");

                contaBancaria.ClientePortadorConta = repClinte.BuscarPorCPFCNPJ(portador);
                contaBancaria.Agencia = agencia;
                contaBancaria.TipoContaBanco = tipoConta;
                contaBancaria.TipoChavePix = tipoChavePIX;
                contaBancaria.DigitoAgencia = digito;
                contaBancaria.CodigoIntegracao = codigoIntegracao;
                contaBancaria.ChavePix = chavepix;
                contaBancaria.Banco = repBanco.BuscarPorCodigo(banco);
                contaBancaria.NumeroConta = numeroConta;
                repContaBancaria.Atualizar(contaBancaria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                unitOfWork.Start();
                Repositorio.Cliente repClinte = new Repositorio.Cliente(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Embarcador.Financeiro.ContaBancaria repContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaBancaria contaBancaria = new Dominio.Entidades.Embarcador.Financeiro.ContaBancaria();

                string agencia = Request.GetStringParam("Agencia");
                string numeroConta = Request.GetStringParam("NumeroConta");
                int portador = Request.GetIntParam("ClientePortadorConta");
                int banco = Request.GetIntParam("Banco");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco>("TipoConta");
                Dominio.ObjetosDeValor.Enumerador.TipoChavePix tipoChavePIX = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.TipoChavePix>("TipoChavePix");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracaoDadosBancarios");
                string digito = Request.GetStringParam("Digito");
                string chavepix = Request.GetStringParam("ChavePix");

                contaBancaria.ClientePortadorConta = repClinte.BuscarPorCPFCNPJ(portador);
                contaBancaria.Agencia = agencia;
                contaBancaria.TipoContaBanco = tipoConta;
                contaBancaria.TipoChavePix = tipoChavePIX;
                contaBancaria.DigitoAgencia = digito;
                contaBancaria.CodigoIntegracao = codigoIntegracao;
                contaBancaria.ChavePix = chavepix;
                contaBancaria.Banco = repBanco.BuscarPorCodigo(banco);
                contaBancaria.NumeroConta = numeroConta;
                repContaBancaria.Inserir(contaBancaria, Auditado);

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
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.ContaBancaria repContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaBancaria ContaBancaria = repContaBancaria.BuscarPorCodigo(codigo,false);
                var dynFormaTitulo = new
                {
                    ContaBancaria.Codigo,
                    ClientePortadorConta = new {
                        Codigo = ContaBancaria.ClientePortadorConta?.CPF_CNPJ_SemFormato ?? "",
                        Descricao = ContaBancaria.ClientePortadorConta?.CPF_CNPJ_Formatado ?? ""
                    },
                    Banco =  new
                    {
                        Codigo = ContaBancaria.Banco?.Codigo ?? 0,
                        Descricao = ContaBancaria.Banco?.Descricao ?? string.Empty
                    },
                    ContaBancaria.Agencia,
                    ContaBancaria.NumeroConta,
                    Digito = ContaBancaria.DigitoAgencia,
                    ContaBancaria.TipoContaBanco,
                    ContaBancaria.TipoChavePix,
                    ContaBancaria.ChavePix,
                    CodigoIntegracaoDadosBancarios = ContaBancaria.CodigoIntegracao
                };
                return new JsonpResult(dynFormaTitulo);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.ContaBancaria repContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaBancaria ContaBancaria = repContaBancaria.BuscarPorCodigo(codigo, false);
                repContaBancaria.Deletar(ContaBancaria, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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
