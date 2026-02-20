using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MovimentoDoFinanceiroController : ApiController
    {

        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("movimentosdofinanceiro.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                decimal valorInicial, valorFinal = 0;
                decimal.TryParse(Request.Params["ValorInicial"], out valorInicial);
                decimal.TryParse(Request.Params["ValorFinal"], out valorFinal);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params["DataInicial"], out dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out dataFinal);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string documento = Request.Params["Documento"];

                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unitOfWork);

                List<Dominio.Entidades.MovimentoDoFinanceiro> listaMovimentos = repMovimento.Consultar(this.EmpresaUsuario.Codigo, valorInicial, valorFinal, dataInicial, dataFinal, documento, inicioRegistros, 50);
                int countMovimentos = repMovimento.ContarConsulta(this.EmpresaUsuario.Codigo, valorInicial, valorFinal, dataInicial, dataFinal, documento);

                var retorno = from obj in listaMovimentos select new { obj.Codigo, Data = obj.Data.ToString("dd/MM/yyyy"), PlacaVeiculo = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty, Plano = string.Concat(obj.PlanoDeConta.Conta, " - ", obj.PlanoDeConta.Descricao), Valor = obj.Valor.ToString("n2") };

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "Veículo|15", "Plano de Conta|45", "Valor|15" }, countMovimentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os movimentos.");
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
                Repositorio.MovimentoDoFinanceiro repMovimentoFinanceiro = new Repositorio.MovimentoDoFinanceiro(unitOfWork);
                Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimentoFinanceiro.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                if (movimento != null)
                {
                    var retorno = new
                    {
                        movimento.Codigo,
                        Data = movimento.Data.ToString("dd/MM/yyyy"),
                        DataPagamento = movimento.DataPagamento.HasValue ? movimento.DataPagamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataBaixa = movimento.DataBaixa.HasValue ? movimento.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                        movimento.Documento,
                        movimento.Observacao,
                        Valor = movimento.Valor.ToString("n2"),
                        CodigoPlanoDeConta = movimento.PlanoDeConta.Codigo,
                        DescricaoPlanoDeConta = string.Concat(movimento.PlanoDeConta.Conta, " - ", movimento.PlanoDeConta.Descricao),
                        CodigoVeiculo = movimento.Veiculo != null ? movimento.Veiculo.Codigo : 0,
                        PlacaVeiculo = movimento.Veiculo != null ? movimento.Veiculo.Placa : string.Empty,
                        CPFCNPJPessoa = movimento.Pessoa != null ? movimento.Pessoa.CPF_CNPJ_Formatado : string.Empty,
                        NomePessoa = movimento.Pessoa != null ? movimento.Pessoa.CPF_CNPJ_Formatado + " - " + movimento.Pessoa.Nome : string.Empty,
                        Tipo = movimento.Tipo != null ? movimento.Tipo : Dominio.Enumeradores.TipoMovimento.Entrada,
                        CodigoMotorista = movimento.Motorista != null ? movimento.Motorista.Codigo : 0,
                        Motorista = movimento.Motorista != null ? movimento.Motorista.CPF + " - " + movimento.Motorista.Nome : string.Empty
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Movimento não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do movimento.");
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
                DateTime data, dataPagamento, dataBaixa;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);
                DateTime.TryParseExact(Request.Params["DataPagamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagamento);
                DateTime.TryParseExact(Request.Params["DataBaixa"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataBaixa);

                int codigo, codigoPlanoDeConta, codigoVeiculo, codigoMotorista = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoPlanoDeConta"], out codigoPlanoDeConta);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                decimal valor = 0m;
                decimal.TryParse(Request.Params["Valor"], out valor);

                double cpfCnpjPessoa = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoPessoa"]), out cpfCnpjPessoa);

                string observacao = Request.Params["Observacao"];
                string documento = Request.Params["Documento"];

                if (data == DateTime.MinValue)
                    return Json<bool>(false, false, "Data inválida.");
                if (valor <= 0)
                    return Json<bool>(false, false, "Valor inválido.");

                Dominio.Enumeradores.TipoMovimento tipoAux;
                Dominio.Enumeradores.TipoMovimento? tipo = null;
                if (Enum.TryParse<Dominio.Enumeradores.TipoMovimento>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unitOfWork);
                Dominio.Entidades.MovimentoDoFinanceiro movimento;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");
                    movimento = repMovimento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                    movimento = new Dominio.Entidades.MovimentoDoFinanceiro();
                }

                //Só deixa atualizar o movimento se não houver nenhum item vinculado a ele, senão, atualiza somente a data de pagamento e de baixa
                if (codigo <= 0 || (movimento.HistoricosVeiculos.Count() <= 0 && movimento.ParcelasCTe.Count() <= 0 && movimento.Despesas.Count() <= 0 && movimento.ParcelaDocumentoEntrada.Count() <= 0 && movimento.CIOTs.Count() <= 0 && movimento.PagamentosMotoristasCTe.Count() <= 0 && movimento.PagamentosMotoristasMDFe.Count() <= 0))
                {
                    Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unitOfWork);
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                    movimento.Pessoa = cpfCnpjPessoa > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                    movimento.Data = data;
                    movimento.Documento = documento;
                    movimento.Empresa = this.EmpresaUsuario;
                    movimento.Observacao = observacao;
                    movimento.Valor = valor;
                    movimento.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo) : null;
                    movimento.PlanoDeConta = repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoDeConta);
                    movimento.Motorista = codigoMotorista > 0 ? repMotorista.BuscarPorCodigo(codigoMotorista) : null;

                    if (movimento.PlanoDeConta == null)
                        return Json<bool>(false, false, "Plano de conta obrigatório.");
                }

                if (dataPagamento != DateTime.MinValue)
                    movimento.DataPagamento = dataPagamento;
                else
                    movimento.DataPagamento = null;

                if (dataBaixa != DateTime.MinValue)
                    movimento.DataBaixa = dataBaixa;
                else
                    movimento.DataBaixa = null;
           
                movimento.Tipo = tipo;

                if (codigo > 0)
                    repMovimento.Atualizar(movimento);
                else
                    repMovimento.Inserir(movimento);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o movimento.");
            }
        }

        #endregion

    }
}
