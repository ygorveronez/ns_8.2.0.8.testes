using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EmissaoCTe.API.Controllers
{
    public class BaixaDeParcelasDuplicatasController : ApiController
    {
        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                var inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int numeroCTe = 0;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroCTe"]), out numeroCTe);

                string numeroDocumento = Request.Params["NumeroDocumento"];

                Dominio.Enumeradores.TipoDuplicata tipoDuplicata = Dominio.Enumeradores.TipoDuplicata.AReceber;
                Enum.TryParse<Dominio.Enumeradores.TipoDuplicata>(Request.Params["Tipo"], out tipoDuplicata);

                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CpfCnpjCliente"]), out cpfCnpjCliente);

                DateTime dataInicial = DateTime.MinValue, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unitOfWork);

                List<Dominio.Entidades.DuplicataParcelas> parcelas = repDuplicataParcelas.ConsultarParcelasPendentes(this.EmpresaUsuario.Codigo, tipoDuplicata, dataInicial, dataFinal, cpfCnpjCliente, numeroDocumento, numeroCTe, inicioRegistros, 50);

                int countParcelas = repDuplicataParcelas.ContarParcelasPendentes(this.EmpresaUsuario.Codigo, tipoDuplicata, dataInicial, dataFinal, cpfCnpjCliente, numeroDocumento, numeroCTe);

                var retorno = from obj in parcelas
                              select new
                              {
                                  obj.Codigo,
                                  Numero = string.Concat(obj.Duplicata.Numero, "/", obj.Parcela),
                                  Documento = obj.Duplicata.Documento,
                                  Valor = obj.Valor.ToString("n2"),
                                  DataVencimento = obj.DataVcto.ToString("dd/MM/yyyy"),
                                  Cliente = obj.Duplicata.Pessoa != null ? string.Concat(obj.Duplicata.Pessoa.CPF_CNPJ_Formatado, " - ", obj.Duplicata.Pessoa.Nome) : string.Empty
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Duplicata/Parcela|15", "Documento|15", "Valor|15", "Data de Vencimento|15", "Pessoa|30" }, countParcelas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter parcelas de duplicatas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BaixarParcelas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                List<int> listaCodigoParcelas = JsonConvert.DeserializeObject<List<int>>(Request.Params["Parcelas"]);

                string observacao = Request.Params["Observacao"];

                var codigoPlanoDeConta = 0;
                int.TryParse(Request.Params["PlanoDeConta"], out codigoPlanoDeConta);

                if (listaCodigoParcelas.Count() > 0)
                {
                    unidadeDeTrabalho.Start();

                    Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);
                    List<Dominio.Entidades.DuplicataParcelas> parcelas = repDuplicataParcelas.BuscarPorListaDeCodigos(this.EmpresaUsuario.Codigo, listaCodigoParcelas);

                    Repositorio.PlanoDeConta repPlanoDeConta = new Repositorio.PlanoDeConta(unidadeDeTrabalho);
                    Dominio.Entidades.PlanoDeConta planoDeConta = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoDeConta);

                    foreach (Dominio.Entidades.DuplicataParcelas parcela in parcelas)
                    {
                        if (parcela.Status == Dominio.Enumeradores.StatusDuplicata.Pendente)
                        {
                            parcela.Status = Dominio.Enumeradores.StatusDuplicata.Paga;
                            parcela.DataPgto = DateTime.Now;
                            parcela.ValorPgto = parcela.Valor;
                            if (planoDeConta != null)
                                parcela.PlanoDeConta = planoDeConta;

                            if (!string.IsNullOrWhiteSpace(observacao))
                                parcela.ObservacaoBaixa += string.Concat(this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": ", observacao);
                            else
                                parcela.ObservacaoBaixa += string.Concat(this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": Baixa da duplicata realizada.");

                            parcela.Funcionario = this.Usuario;

                            repDuplicataParcelas.Atualizar(parcela);

                            if (planoDeConta != null)
                                this.SalvarMovimentoDoFinanceiro(parcela, unidadeDeTrabalho);
                        }
                    }

                    unidadeDeTrabalho.CommitChanges();

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Quantidade de duplicatas inválida.");
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao efetuar a baixa das duplicatas.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ObterInformacoesParcela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoParcela"], "CT3##MULT1@#$S0FTW4R3"), out codigo);

                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unitOfWork);

                Dominio.Entidades.DuplicataParcelas parcela = repDuplicataParcelas.BuscarPorCodigo(codigo);

                if (parcela != null)
                {
                    var retorno = new
                    {
                        Codigo = parcela.Codigo,
                        Numero = string.Concat(parcela.Duplicata.Numero, "/", parcela.Parcela),
                        Valor = parcela.Valor.ToString("n2"),
                        DataVencimento = parcela.DataVcto.ToString("dd/MM/yyyy"),
                        Cliente = parcela.Duplicata.Pessoa != null ? string.Concat(parcela.Duplicata.Pessoa.CPF_CNPJ_Formatado," - ", parcela.Duplicata.Pessoa.Nome) : string.Empty
                    };

                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Parcela não encontrada.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter dados da parcela");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarMovimentoDoFinanceiro(Dominio.Entidades.DuplicataParcelas parcela, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (parcela != null)
            {
                if (parcela.PlanoDeConta != null)
                {
                    Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                    Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorDuplicataParcela(parcela.Duplicata.Empresa.Codigo, parcela.Codigo);

                    if (movimento == null)
                        movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                    movimento.Data = (DateTime)parcela.DataPgto;
                    movimento.Documento = parcela.Duplicata.Documento;
                    movimento.Empresa = parcela.Duplicata.Empresa;
                    movimento.PlanoDeConta = parcela.PlanoDeConta;
                    movimento.Valor = parcela.ValorPgto;
                    movimento.Pessoa = parcela.Duplicata.Pessoa;
                    movimento.Veiculo = parcela.Duplicata.Veiculo1;
                    movimento.Observacao = string.Concat("Ref. à baixa da parcela " + parcela.Parcela + " da duplicata " + parcela.Duplicata.Numero + ".");
                    movimento.Tipo = parcela.Duplicata.Tipo == Dominio.Enumeradores.TipoDuplicata.AReceber ? Dominio.Enumeradores.TipoMovimento.Entrada : Dominio.Enumeradores.TipoMovimento.Saida;
                    if (parcela.Duplicata.Motorista != null)
                        movimento.Motorista = parcela.Duplicata.Motorista;

                    if (movimento.DuplicataParcelas != null && movimento.DuplicataParcelas.Count() > 0)
                        movimento.DuplicataParcelas.Clear();
                    else if (movimento.DuplicataParcelas == null)
                        movimento.DuplicataParcelas = new List<Dominio.Entidades.DuplicataParcelas>();

                    movimento.DuplicataParcelas.Add(parcela);

                    if (movimento.Codigo > 0)
                        repMovimento.Atualizar(movimento);
                    else
                        repMovimento.Inserir(movimento);

                    if (parcela.Duplicata.PlanoDeConta != null)
                    {
                        Repositorio.MovimentoDoFinanceiro repMovimentoParcela = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                        Dominio.Entidades.MovimentoDoFinanceiro movimentoParcela = new Dominio.Entidades.MovimentoDoFinanceiro();

                        movimentoParcela.Data = (DateTime)parcela.DataPgto;
                        movimentoParcela.Documento = parcela.Duplicata.Documento;
                        movimentoParcela.Empresa = parcela.Duplicata.Empresa;
                        movimentoParcela.PlanoDeConta = parcela.Duplicata.PlanoDeConta;
                        movimentoParcela.Valor = (parcela.ValorPgto * -1);
                        movimentoParcela.Pessoa = parcela.Duplicata.Pessoa;
                        movimentoParcela.Veiculo = parcela.Duplicata.Veiculo1;
                        movimentoParcela.Observacao = string.Concat("Ref. à baixa da parcela " + parcela.Parcela + " da duplicata " + parcela.Duplicata.Numero + ".");
                        movimentoParcela.Tipo = Dominio.Enumeradores.TipoMovimento.Saida;
                        if (parcela.Duplicata.Motorista != null)
                            movimentoParcela.Motorista = parcela.Duplicata.Motorista;

                        if (movimentoParcela.Codigo > 0)
                            repMovimentoParcela.Atualizar(movimentoParcela);
                        else
                            repMovimentoParcela.Inserir(movimentoParcela);
                    }
                }
            }
        }

        #endregion
    }
}