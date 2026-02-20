using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class HistoricoDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("historicosdeveiculos.aspx") select obj).FirstOrDefault();
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
                string placa = Request.Params["Placa"];
                string servico = Request.Params["Servico"];
                string status = Request.Params["Status"];

                Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(unitOfWork);
                var listaHistoricoVeiculos = repHistoricoVeiculo.Consultar(this.EmpresaUsuario.Codigo, placa, servico, status, inicioRegistros, 50);
                int countHistoricoVeiculos = repHistoricoVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, placa, servico, status);

                var retorno = from obj in listaHistoricoVeiculos select new { obj.Codigo, obj.Status, Data = obj.Data != null ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty, obj.Veiculo.Placa, obj.Servico.Descricao, Quantidade = obj.Quantidade.ToString("n2"), Valor = obj.Valor.ToString("n2"), obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Data|15", "Veículo|15", "Serviço|26", "Quantidade|12", "Valor|12", "Status|10" }, countHistoricoVeiculos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os históricos de veículos.");
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
                Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(unitOfWork);
                Dominio.Entidades.HistoricoVeiculo historico = repHistoricoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                if (historico != null)
                {
                    var retorno = new
                    {
                        historico.Codigo,
                        Data = historico.Data != null ? historico.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataGarantia = historico.DataGarantia != null ? historico.DataGarantia.Value.ToString("dd/MM/yyyy") : string.Empty,
                        CodigoFornecedor = historico.Fornecedor != null ? historico.Fornecedor.CPF_CNPJ_Formatado : string.Empty,
                        DescricaoFornecedor = historico.Fornecedor != null ? string.Concat(historico.Fornecedor.CPF_CNPJ_Formatado, " - ", historico.Fornecedor.Nome) : historico.NomeFornecedor,
                        historico.Observacao,
                        historico.KMVeiculo,
                        historico.Quantidade,
                        CodigoServico = historico.Servico != null ? historico.Servico.Codigo : 0,
                        DescricaoServico = historico.Servico != null ? historico.Servico.Descricao : string.Empty,
                        CodigoVeiculo = historico.Veiculo != null ? historico.Veiculo.Codigo : 0,
                        DescricaoVeiculo = historico.Veiculo != null ? historico.Veiculo.Placa : string.Empty,
                        historico.Valor,
                        historico.Status
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Histórico não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do histórico de veículos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo, kmVeiculo, codigoServico, codigoVeiculo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoServico"], out codigoServico);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["KilometragemVeiculo"], out kmVeiculo);
                decimal valor, quantidade = 0;
                decimal.TryParse(Request.Params["Valor"], out valor);
                decimal.TryParse(Request.Params["Quantidade"], out quantidade);
                DateTime data, dataGarantia;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);
                DateTime.TryParseExact(Request.Params["DataGarantia"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataGarantia);
                string status = Request.Params["Status"];
                string observacao = Request.Params["Observacao"];

                Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(unidadeDeTrabalho);
                Dominio.Entidades.HistoricoVeiculo historicoVeiculo;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    historicoVeiculo = new Dominio.Entidades.HistoricoVeiculo();
                    historicoVeiculo.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    historicoVeiculo = repHistoricoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                Servicos.HistoricoDeVeiculo svcHistoricoVeiculo = new Servicos.HistoricoDeVeiculo(unidadeDeTrabalho);
                Repositorio.ServicoVeiculo repServicoVeiculo = new Repositorio.ServicoVeiculo(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                if (data != DateTime.MinValue)
                    historicoVeiculo.Data = data;
                else
                    historicoVeiculo.Data = null;

                if (dataGarantia != DateTime.MinValue)
                    historicoVeiculo.DataGarantia = dataGarantia;
                else
                    historicoVeiculo.DataGarantia = null;

                if (!string.IsNullOrWhiteSpace(Request.Params["CodigoFornecedor"]))
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                    double cpfCnpjFornecedor = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoFornecedor"]), out cpfCnpjFornecedor);
                    historicoVeiculo.Fornecedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor);
                    historicoVeiculo.NomeFornecedor = string.Empty;
                }
                else
                {
                    historicoVeiculo.NomeFornecedor = Request.Params["DescricaoFornecedor"];
                    historicoVeiculo.Fornecedor = null;
                }

                historicoVeiculo.KMVeiculo = kmVeiculo;
                historicoVeiculo.Observacao = observacao;
                historicoVeiculo.Quantidade = quantidade;
                historicoVeiculo.Servico = repServicoVeiculo.BuscarPorCodigo(codigoServico, this.EmpresaUsuario.Codigo);
                historicoVeiculo.Valor = valor;
                historicoVeiculo.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    historicoVeiculo.Status = status;

                unidadeDeTrabalho.Start();

                if (codigo > 0)
                    repHistoricoVeiculo.Atualizar(historicoVeiculo);
                else
                    repHistoricoVeiculo.Inserir(historicoVeiculo);

                svcHistoricoVeiculo.GerarMovimentoDoFinanceiro(historicoVeiculo.Codigo, historicoVeiculo.Veiculo.Empresa.Codigo);

                unidadeDeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o histórico de veículo.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
