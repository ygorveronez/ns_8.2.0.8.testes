using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EmissaoCTe.API.Controllers
{
    public class CobrancaCTeController : ApiController
    {
        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0, inicioRegistros = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.StatusDuplicata status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["Status"], out status);

                double cpfCnpjCliente = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CpfCnpjCliente"]), out cpfCnpjCliente);

                DateTime dataInicial = DateTime.MinValue, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.ParcelaCobrancaCTe repParcelaCobrancaCTe = new Repositorio.ParcelaCobrancaCTe(unitOfWork);

                List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCobrancaCTe.Consultar(this.EmpresaUsuario.Codigo, codigoCTe, dataInicial, dataFinal, status, cpfCnpjCliente, inicioRegistros, 50);

                int countParcelas = repParcelaCobrancaCTe.ContarConsulta(this.EmpresaUsuario.Codigo, codigoCTe, dataInicial, dataFinal, status, cpfCnpjCliente);

                var retorno = from obj in parcelas select new { obj.Codigo, Numero = string.Concat(obj.Cobranca.Numero, " - ", obj.Numero), CTe = string.Concat(obj.Cobranca.CTe.Numero, " - ", obj.Cobranca.CTe.Serie.Numero), Valor = obj.Valor.ToString("n2"), DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"), Cliente = obj.Cobranca.Cliente != null ? string.Concat(obj.Cobranca.Cliente.CPF_CNPJ_Formatado, " - ", obj.Cobranca.Cliente.Nome) : string.Empty };

                return Json(retorno, true, null, new string[] { "Codigo", "Duplicata|15", "CT-e|15", "Valor|15", "Data de Vencimento|15", "Cliente|30" }, countParcelas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das duplicatas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Dominio.Enumeradores.StatusDuplicata status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["Status"], out status);

                double cpfCnpjCliente = 0f;
                double.TryParse(Request.Params["CpfCnpjCliente"], out cpfCnpjCliente);

                DateTime dataInicial = DateTime.MinValue, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.ParcelaCobrancaCTe repParcelaCobrancaCTe = new Repositorio.ParcelaCobrancaCTe(unitOfWork);

                int countParcelas = repParcelaCobrancaCTe.ContarConsulta(this.EmpresaUsuario.Codigo, codigoCTe, dataInicial, dataFinal, status, cpfCnpjCliente);

                if (countParcelas > 1000)
                    return Json<bool>(false, false, "Selecione a quantidade máxima de 1000 (mil) duplicatas (" + countParcelas.ToString() + " selecionadas).");

                List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCobrancaCTe.Consultar(this.EmpresaUsuario.Codigo, codigoCTe, dataInicial, dataFinal, status, cpfCnpjCliente, 0, 1000);

                var retorno = from obj in parcelas select new { obj.Codigo, Numero = string.Concat(obj.Cobranca.Numero, " - ", obj.Numero), CTe = string.Concat(obj.Cobranca.CTe.Numero, " - ", obj.Cobranca.CTe.Serie.Numero), Valor = obj.Valor.ToString("n2"), DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"), Cliente = obj.Cobranca.Cliente != null ? string.Concat(obj.Cobranca.Cliente.CPF_CNPJ_Formatado, " - ", obj.Cobranca.Cliente.Nome) : string.Empty };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das duplicatas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarProximoNumeroDeFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.CobrancaCTe repCobrancaCTe = new Repositorio.CobrancaCTe(unitOfWork);
                int numero = repCobrancaCTe.BuscarUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                return Json(numero, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar o próximo número para as faturas do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.ParcelaCobrancaCTe repParcelaCobrancaCTe = new Repositorio.ParcelaCobrancaCTe(unitOfWork);
                    List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCobrancaCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);
                    var retorno = from obj in parcelas
                                  select new Dominio.ObjetosDeValor.Cobranca
                                  {
                                      Codigo = obj.Codigo,
                                      DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                      Excluir = false,
                                      Numero = obj.Cobranca.Numero,
                                      Parcela = obj.Numero,
                                      Valor = obj.Valor
                                  };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Código do CT-e inválido.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de cobrança do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BaixarDuplicatas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                List<int> listaCodigoDuplicatas = JsonConvert.DeserializeObject<List<int>>(Request.Params["Duplicatas"]);
                string observacao = Request.Params["Observacao"];

                if (listaCodigoDuplicatas.Count() > 0)
                {
                    unidadeDeTrabalho.Start();

                    Repositorio.ParcelaCobrancaCTe repParcelaCobranca = new Repositorio.ParcelaCobrancaCTe(unidadeDeTrabalho);
                    List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCobranca.BuscarPorListaDeCodigo(this.EmpresaUsuario.Codigo, listaCodigoDuplicatas);

                    foreach (Dominio.Entidades.ParcelaCobrancaCTe parcela in parcelas)
                    {
                        if (parcela.Status == Dominio.Enumeradores.StatusDuplicata.Pendente)
                        {
                            parcela.Status = Dominio.Enumeradores.StatusDuplicata.Paga;
                            parcela.DataPagamento = DateTime.Now;

                            if (!string.IsNullOrWhiteSpace(observacao))
                                parcela.Observacao += string.Concat("\n", this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": ", observacao);
                            else
                                parcela.Observacao += string.Concat("\n", this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": Baixa da duplicata realizada.");

                            repParcelaCobranca.Atualizar(parcela);

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
        public ActionResult ReverterDuplicata()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDuplicata = 0;
                int.TryParse(Request.Params["Codigo"], out codigoDuplicata);

                Repositorio.ParcelaCobrancaCTe repParcelaCobranca = new Repositorio.ParcelaCobrancaCTe(unidadeDeTrabalho);
                Dominio.Entidades.ParcelaCobrancaCTe parcela = repParcelaCobranca.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoDuplicata);

                if (parcela != null)
                {
                    if (parcela.Status == Dominio.Enumeradores.StatusDuplicata.Paga)
                    {
                        unidadeDeTrabalho.Start();

                        parcela.Status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                        parcela.DataPagamento = null;
                        parcela.Observacao += string.Concat("\n", this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": Estorno da duplicata realizado.");
                        
                        repParcelaCobranca.Atualizar(parcela);

                        this.SalvarMovimentoDoFinanceiro(parcela, unidadeDeTrabalho);

                        unidadeDeTrabalho.CommitChanges();

                        return Json<bool>(true, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, "Status da parcela inválido para a reversão.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Duplicata não encontrada.");
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao reverter a duplicata.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarMovimentoDoFinanceiro(Dominio.Entidades.ParcelaCobrancaCTe parcela, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);

            Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorParcelaCobrancaCTe(this.EmpresaUsuario.Codigo, parcela.Codigo);

            if (movimento != null)
            {
                if (parcela.Status == Dominio.Enumeradores.StatusDuplicata.Paga)
                {
                    movimento.DataPagamento = parcela.DataPagamento;
                    movimento.DataBaixa = parcela.DataPagamento;
                }
                else if (parcela.Status == Dominio.Enumeradores.StatusDuplicata.Pendente)
                {
                    movimento.DataPagamento = null;
                    movimento.DataBaixa = null;
                }

                repMovimento.Atualizar(movimento);
            }
        }

        #endregion

    }
}
