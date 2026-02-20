using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AbastecimentoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("abastecimentos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string posto = Request.Params["Posto"];
                string placaVeiculo = Request.Params["PlacaVeiculo"];

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                IList<Dominio.Entidades.Abastecimento> listaAbastecimento = repAbastecimento.Consultar(this.EmpresaUsuario.Codigo, posto, placaVeiculo, data, inicioRegistros, 50);

                int countAbastecimentos = repAbastecimento.ContarConsulta(this.EmpresaUsuario.Codigo, posto, placaVeiculo, data);

                var retorno = (from obj in listaAbastecimento
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data != null ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                   Posto = obj.Posto != null && obj.Posto.CPF_CNPJ > 0 ? obj.Posto.Nome : obj.NomePosto,
                                   Litros = obj.Litros.ToString("n2"),
                                   ValorUnitario = obj.ValorUnitario.ToString("n4")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "Veículo|15", "Posto|30", "Litros|15", "Valor Un.|15" }, countAbastecimentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os abastecimentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoAcertoViagem = 0;
                int.TryParse(Request.Params["CodigoAcertoViagem"], out codigoAcertoViagem);

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.BuscarPorAcertoDeViagem(codigoAcertoViagem);

                var retorno = (from obj in listaAbastecimentos
                              select new Dominio.ObjetosDeValor.AbastecimentoAcertoDeViagem
                              {
                                  Codigo = obj.Codigo,
                                  CodigoPosto = obj.Posto != null && obj.Posto.CPF_CNPJ > 0 ? obj.Posto.CPF_CNPJ_Formatado : string.Empty,
                                  Data = obj.Data != null ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  DescricaoPosto = obj.Posto != null && obj.Posto.CPF_CNPJ > 0 ? string.Concat(obj.Posto.CPF_CNPJ_Formatado, " - ", obj.Posto.Nome) : obj.NomePosto,
                                  Excluir = false,
                                  KMFinal = obj.Kilometragem,
                                  KMInicial = obj.KilometragemAnterior,
                                  Litros = obj.Litros.ToString("n2"),
                                  Media = obj.Media.ToString("n2"),
                                  ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                  Pago = obj.Pago
                              }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os abastecimentos do acerto de viagem.");
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

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo);

                if (abastecimento != null)
                {
                    var retorno = new
                    {
                        abastecimento.Codigo,
                        Data = abastecimento.Data != null ? abastecimento.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                        abastecimento.Kilometragem,
                        abastecimento.KilometragemAnterior,
                        Litros = abastecimento.Litros.ToString("n2"),
                        Media = abastecimento.Media.ToString("n2"),
                        DescricaoMotorista = abastecimento.Motorista != null ? string.Concat(abastecimento.Motorista.CPF, " - ", abastecimento.Motorista.Nome) : string.Empty,
                        CodigoMotorista = abastecimento.Motorista != null ? abastecimento.Motorista.Codigo : 0,
                        DescricaoPosto = abastecimento.Posto != null && abastecimento.Posto.CPF_CNPJ > 0 ? string.Concat(abastecimento.Posto.CPF_CNPJ_Formatado, " - ", abastecimento.Posto.Nome) : abastecimento.NomePosto,
                        CodigoPosto = abastecimento.Posto != null && abastecimento.Posto.CPF_CNPJ > 0 ? abastecimento.Posto.CPF_CNPJ_Formatado : string.Empty,
                        abastecimento.Situacao,
                        abastecimento.Status,
                        abastecimento.ValorUnitario,
                        DescricaoVeiculo = abastecimento.Veiculo != null ? abastecimento.Veiculo.Placa : string.Empty,
                        CodigoVeiculo = abastecimento.Veiculo != null ? abastecimento.Veiculo.Codigo : 0,
                        abastecimento.Pago
                    };

                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Abastecimento não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do abastecimento.");
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
                int codigo, codigoVeiculo, codigoMotorista = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                double codigoPosto = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoPosto"]), out codigoPosto);

                decimal kmInicial, kmFinal, litros, valorUnitario, media = 0m;
                decimal.TryParse(Request.Params["KMInicial"], out kmInicial);
                decimal.TryParse(Request.Params["KMFinal"], out kmFinal);
                decimal.TryParse(Request.Params["Litros"], out litros);
                decimal.TryParse(Request.Params["ValorUnitario"], out valorUnitario);
                decimal.TryParse(Request.Params["Media"], out media);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                string situacao = Request.Params["Situacao"];
                string status = Request.Params["Status"];
                string descricaoPosto = Request.Params["DescricaoPosto"];

                bool pago = false;
                bool.TryParse(Request.Params["Pago"], out pago);

                if (codigoVeiculo <= 0)
                    return Json<bool>(false, false, "Veículo obrigatório.");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");
                    abastecimento = repAbastecimento.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                    abastecimento = new Dominio.Entidades.Abastecimento();
                }

                if (data != DateTime.MinValue)
                    abastecimento.Data = data;
                else
                    abastecimento.Data = null;

                abastecimento.Kilometragem = kmFinal;
                abastecimento.KilometragemAnterior = kmInicial;
                abastecimento.Litros = litros;
                abastecimento.Media = media;
                abastecimento.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                abastecimento.Posto = codigoPosto > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPosto) : null;
                abastecimento.NomePosto = descricaoPosto;
                abastecimento.Situacao = situacao;
                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.Status = status;
                abastecimento.ValorUnitario = valorUnitario;
                abastecimento.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                abastecimento.Pago = pago;

                if (abastecimento.Veiculo != null && abastecimento.Veiculo.KilometragemAtual < kmFinal && abastecimento.Situacao == "F")
                {
                    abastecimento.Veiculo.KilometragemAtual = int.Parse(kmFinal.ToString());
                    repVeiculo.Atualizar(abastecimento.Veiculo, Auditado, null, "Altualizada a Quilometragem do Veículo via Abastecimento");
                }

                if (codigo > 0)
                    repAbastecimento.Atualizar(abastecimento);
                else
                    repAbastecimento.Inserir(abastecimento);

                Servicos.Abastecimento svcAbastecimento = new Servicos.Abastecimento(unitOfWork);
                svcAbastecimento.GerarMovimentoDoFinanceiro(abastecimento.Codigo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o abastecimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
