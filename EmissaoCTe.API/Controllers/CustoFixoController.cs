using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class CustoFixoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("custosfixosdeveiculos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];
                string placaVeiculo = Request.Params["PlacaVeiculo"];
                string status = Request.Params["Status"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                
                Repositorio.CustoFixo repCustoFixo = new Repositorio.CustoFixo(unitOfWork);
                var listaCustoFixo = repCustoFixo.Consultar(this.EmpresaUsuario.Codigo, descricao, placaVeiculo, nomeMotorista, status, inicioRegistros, 50);
                int countCustoFixo = repCustoFixo.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, placaVeiculo, nomeMotorista, status);

                var retorno = (from obj in listaCustoFixo
                              select new
                              {
                                  obj.Codigo,
                                  obj.Status,
                                  obj.Descricao,
                                  Placa = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                  Motorista = obj.Funcionario != null ? obj.Funcionario.Nome : string.Empty,
                                  DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                                  DataFinal = obj.DataFinal.ToString("dd/MM/yyyy"),
                                  obj.DescricaoStatus
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|20", "Veículo|14", "Motorista|15", "Data Inicial|13", "Data Final|13", "Status|15" }, countCustoFixo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os custos fixos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.CustoFixo repCustoFixo = new Repositorio.CustoFixo(unitOfWork);
                Dominio.Entidades.CustoFixo custoFixo = repCustoFixo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (custoFixo != null)
                {
                    var retorno = new
                    {
                        custoFixo.Codigo,
                        DataFinal = custoFixo.DataFinal.ToString("dd/MM/yyyy"),
                        DataInicial = custoFixo.DataInicial.ToString("dd/MM/yyyy"),
                        custoFixo.Descricao,
                        custoFixo.Rateio,
                        custoFixo.Status,
                        CodigoTipoCustoFixo = custoFixo.TipoDeCustoFixo.Codigo,
                        DescricaoTipoCustoFixo = custoFixo.TipoDeCustoFixo.Descricao,
                        ValorTotal = custoFixo.ValorTotal.ToString("n2"),
                        CodigoVeiculo = custoFixo.Veiculo != null ? custoFixo.Veiculo.Codigo : 0,
                        PlacaVeiculo = custoFixo.Veiculo != null ? custoFixo.Veiculo.Placa : string.Empty,
                        CodigoMotorista = custoFixo.Funcionario != null ? custoFixo.Funcionario.Codigo : 0,
                        Motorista = custoFixo.Funcionario != null ? custoFixo.Motorista : string.Empty
                    };

                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Custo fixo não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar o custo fixo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoVeiculo, codigoTipoCustoFixo, rateio = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoTipoCustoFixo"], out codigoTipoCustoFixo);
                int.TryParse(Request.Params["Rateio"], out rateio);
                int.TryParse(Request.Params["CodigoMotorista"], out int codigoMotorista);

                decimal valorTotal = 0;
                decimal.TryParse(Request.Params["ValorTotal"], out valorTotal);

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];

                if (dataInicial == DateTime.MinValue)
                    return Json<bool>(false, false, "Data inicial inválida.");

                if (dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Data final inválida.");

                if (rateio <= 0)
                    return Json<bool>(false, false, "Rateio inválido.");

                if (valorTotal <= 0)
                    return Json<bool>(false, false, "Valor total inválido.");

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.TipoDeCustoFixo repTipoCustoFixo = new Repositorio.TipoDeCustoFixo(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.CustoFixo repCustoFixo = new Repositorio.CustoFixo(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.CustoFixo custoFixo = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    custoFixo = repCustoFixo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    custoFixo = new Dominio.Entidades.CustoFixo();
                    custoFixo.Status = "A";
                }

                custoFixo.Descricao = descricao;
                custoFixo.Empresa = this.EmpresaUsuario;
                custoFixo.DataFinal = dataFinal;
                custoFixo.DataInicial = dataInicial;
                custoFixo.Rateio = rateio;
                custoFixo.TipoDeCustoFixo = repTipoCustoFixo.BuscarPorCodigo(codigoTipoCustoFixo, this.EmpresaUsuario.Codigo);
                custoFixo.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo) : null;
                custoFixo.ValorTotal = valorTotal;
                custoFixo.Funcionario = codigoMotorista > 0 ? repMotorista.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista) : null;

                if (custoFixo.TipoDeCustoFixo == null)
                    return Json<bool>(false, false, "Tipo de custo fixo inválido.");

                if (custoFixo.Veiculo == null && custoFixo.Funcionario == null)
                    return Json<bool>(false, false, "Deve ser informado um Veículo ou Motorista.");

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    custoFixo.Status = status;

                if (codigo > 0)
                    repCustoFixo.Atualizar(custoFixo);
                else
                    repCustoFixo.Inserir(custoFixo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o custo fixo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
