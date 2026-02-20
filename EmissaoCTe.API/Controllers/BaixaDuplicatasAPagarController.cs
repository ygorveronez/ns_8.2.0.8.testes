using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EmissaoCTe.API.Controllers
{
    public class BaixaDuplicatasAPagarController : ApiController
    {
        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDocumentoEntrada = 0, inicioRegistros = 0;
                int.TryParse(Request.Params["CodigoDocumentoEntrada"], out codigoDocumentoEntrada);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.StatusDuplicata status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["Status"], out status);

                double cpfCnpjCliente = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CpfCnpjCliente"]), out cpfCnpjCliente);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unitOfWork);

                List<Dominio.Entidades.ParcelaDocumentoEntrada> parcelas = repParcelaDocumentoEntrada.Consultar(this.EmpresaUsuario.Codigo, codigoDocumentoEntrada, dataInicial, dataFinal, status, cpfCnpjCliente, inicioRegistros, 50);

                int countParcelas = repParcelaDocumentoEntrada.ContarConsulta(this.EmpresaUsuario.Codigo, codigoDocumentoEntrada, dataInicial, dataFinal, status, cpfCnpjCliente);

                var retorno = from obj in parcelas select new { obj.Codigo, Numero = obj.Numero, DocumentoEntrada = obj.DocumentoEntrada.Numero.ToString(), Valor = obj.Valor.ToString("n2"), DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"), Cliente = obj.DocumentoEntrada.Fornecedor != null ? string.Concat(obj.DocumentoEntrada.Fornecedor.CPF_CNPJ_Formatado, " - ", obj.DocumentoEntrada.Fornecedor.Nome) : string.Empty };

                return Json(retorno, true, null, new string[] { "Codigo", "Duplicata|15", "Doc. Entrada|15", "Valor|15", "Data de Vencimento|15", "Fornecedor|30" }, countParcelas);
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
                int codigoDocumentoEntrada = 0, inicioRegistros = 0;
                int.TryParse(Request.Params["CodigoDocumentoEntrada"], out codigoDocumentoEntrada);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.StatusDuplicata status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["Status"], out status);

                double cpfCnpjCliente = 0f;
                double.TryParse(Request.Params["CpfCnpjCliente"], out cpfCnpjCliente);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unitOfWork);

                int countParcelas = repParcelaDocumentoEntrada.ContarConsulta(this.EmpresaUsuario.Codigo, codigoDocumentoEntrada, dataInicial, dataFinal, status, cpfCnpjCliente);

                if (countParcelas > 1000)
                    return Json<bool>(false, false, "Selecione a quantidade máxima de 1000 (mil) duplicatas (" + countParcelas.ToString() + " selecionadas).");

                List<Dominio.Entidades.ParcelaDocumentoEntrada> parcelas = repParcelaDocumentoEntrada.Consultar(this.EmpresaUsuario.Codigo, codigoDocumentoEntrada, dataInicial, dataFinal, status, cpfCnpjCliente, inicioRegistros, 50);

                var retorno = from obj in parcelas select new { obj.Codigo, obj.Numero, DocumentoEntrada = obj.DocumentoEntrada.Numero, Valor = obj.Valor.ToString("n2"), DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"), Cliente = obj.DocumentoEntrada.Fornecedor != null ? string.Concat(obj.DocumentoEntrada.Fornecedor.CPF_CNPJ_Formatado, " - ", obj.DocumentoEntrada.Fornecedor.Nome) : string.Empty };

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

                    Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unidadeDeTrabalho);
                    List<Dominio.Entidades.ParcelaDocumentoEntrada> parcelas = repParcelaDocumentoEntrada.BuscarPorListaDeCodigos(this.EmpresaUsuario.Codigo, listaCodigoDuplicatas);

                    foreach (Dominio.Entidades.ParcelaDocumentoEntrada parcela in parcelas)
                    {
                        if (parcela.Status == Dominio.Enumeradores.StatusDuplicata.Pendente)
                        {
                            parcela.Status = Dominio.Enumeradores.StatusDuplicata.Paga;
                            parcela.DataPagamento = DateTime.Now;

                            if (!string.IsNullOrWhiteSpace(observacao))
                                parcela.Observacao += string.Concat("\n", this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": ", observacao);
                            else
                                parcela.Observacao += string.Concat("\n", this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": Baixa da duplicata realizada.");

                            repParcelaDocumentoEntrada.Atualizar(parcela);

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

                Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unidadeDeTrabalho);

                Dominio.Entidades.ParcelaDocumentoEntrada parcela = repParcelaDocumentoEntrada.BuscarPorCodigo(codigoDuplicata);

                if (parcela == null)
                    return Json<bool>(false, false, "Duplicata não encontrada.");

                if (parcela.Status != Dominio.Enumeradores.StatusDuplicata.Paga)
                    return Json<bool>(false, false, "Status da parcela inválido para a reversão.");

                unidadeDeTrabalho.Start();

                parcela.Status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                parcela.DataPagamento = null;
                parcela.Observacao += string.Concat("\n", this.Usuario.CPF, "-", this.Usuario.Nome, " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": Estorno da duplicata realizado.");

                repParcelaDocumentoEntrada.Atualizar(parcela);

                this.SalvarMovimentoDoFinanceiro(parcela, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao reverter a duplicata.");
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarMovimentoDoFinanceiro(Dominio.Entidades.ParcelaDocumentoEntrada parcela, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);

            Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorDuplicataDocumentoEntrada(this.EmpresaUsuario.Codigo, parcela.Codigo);

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
