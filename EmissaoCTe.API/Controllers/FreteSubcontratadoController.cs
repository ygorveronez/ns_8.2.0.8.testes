using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace EmissaoCTe.API.Controllers
{
    public class FreteSubcontratadoController : ApiController
    {
        #region Variáveis Globais
        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretesubcontratado.aspx") select obj).FirstOrDefault();
        }
        #endregion

        #region Métodos Globais
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo, inicioRegistros, numeroCTe, numeroNFe = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["InicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CTe"], out numeroCTe);
                int.TryParse(Request.Params["NFe"], out numeroNFe);

                double cnpjParceiro = 0;
                double.TryParse(Request.Params["Parceiro"], out cnpjParceiro);

                string nomeParceiro = Request.Params["NomeParceiro"];

                var dataEntrada = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataEntrada"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrada);

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);
                List<Dominio.Entidades.FreteSubcontratado> listaFreteSubcontratado = repFreteSubcontratado.Consultar(this.EmpresaUsuario.Codigo, nomeParceiro, cnpjParceiro, numeroCTe, numeroNFe, dataEntrada, inicioRegistros, 50);
                int countRegistros = repFreteSubcontratado.ContarConsulta(this.EmpresaUsuario.Codigo, nomeParceiro, cnpjParceiro, numeroCTe, numeroNFe, dataEntrada);

                var retorno = (from obj in listaFreteSubcontratado
                               select new
                               {
                                   obj.Codigo,
                                   Parceiro = obj.Parceiro.CPF_CNPJ + " " + obj.Parceiro.Nome,
                                   Tipo = obj.DescricaoTipo,
                                   CTe = obj.NumeroCTe,
                                   NFe = obj.NumeroNFe,
                                   DataEntrada = obj.DataEntrada.ToString("dd/MM/yyyy")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Parceiro|30", "Tipo|10", "CTe|15", "NFe|15", "Data Entrada|10" }, countRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar frete subcontrado.");
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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);

                Dominio.Entidades.FreteSubcontratado freteSubcontratado = repFreteSubcontratado.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (freteSubcontratado == null)
                    return Json<bool>(false, false, "Frete Subcontratado não encontrado.");

                decimal valorMinimo = 0;
                Repositorio.ClienteComissao repClienteComissao = new Repositorio.ClienteComissao(unitOfWork);
                valorMinimo = repClienteComissao.BuscaValorMinimo(this.EmpresaUsuario.Codigo, freteSubcontratado.Parceiro.CPF_CNPJ, freteSubcontratado.LocalidadeDestino.Codigo);

                var retorno = new
                {
                    freteSubcontratado.Codigo,
                    Parceiro = freteSubcontratado.Parceiro.CPF_CNPJ + " " + freteSubcontratado.Parceiro.Nome,
                    CNPJCPFParceiro = freteSubcontratado.Parceiro.CPF_CNPJ,
                    Filial = freteSubcontratado.Filial,
                    NumeroCTe = freteSubcontratado.NumeroCTe,
                    NumeroNFe = freteSubcontratado.NumeroNFe,
                    DataEntrada = freteSubcontratado.DataEntrada.ToString("dd/MM/yyyy"),
                    Remetente = freteSubcontratado.Remetente.CPF_CNPJ + " " + freteSubcontratado.Remetente.Nome,
                    CNPJCPFRemetente = freteSubcontratado.Remetente.CPF_CNPJ,
                    Destinatario = freteSubcontratado.Destinatario.CPF_CNPJ + " " + freteSubcontratado.Destinatario.Nome,
                    CNPJCPFDestinatario = freteSubcontratado.Destinatario.CPF_CNPJ,
                    Localidade = freteSubcontratado.LocalidadeDestino.Descricao + " / " + freteSubcontratado.LocalidadeDestino.Estado.Sigla,
                    CodigoLocalidade = freteSubcontratado.LocalidadeDestino.Codigo,
                    Tipo = freteSubcontratado.Tipo,
                    Status = freteSubcontratado.Status,
                    Peso = freteSubcontratado.Peso.ToString("n2"),
                    Quantidade = freteSubcontratado.Quantidade.ToString(),
                    ValorFrete = freteSubcontratado.ValorFreteTotal.ToString("n2"),
                    ValorICMS = freteSubcontratado.ValorICMS.ToString("n2"),
                    ValorFreteLiquido = freteSubcontratado.ValorFreteLiquido.ToString("n2"),
                    ValorTaxaAdicional = freteSubcontratado.ValorTaxaAdicional.ToString("n2"),
                    ValorTDA = freteSubcontratado.ValorTDA.ToString("n2"),
                    ValorTDE = freteSubcontratado.ValorTDE.ToString("n2"),
                    ValorCarroDedicado = freteSubcontratado.ValorCarroDedicado.ToString("n2"),
                    ValorComissao = freteSubcontratado.ValorComissao.ToString("n2"),
                    PercentualComissao = freteSubcontratado.PercentualComissao.ToString("n2"),
                    Motorista = freteSubcontratado.Motorista != null ? freteSubcontratado.Motorista.CPF + " " + freteSubcontratado.Motorista.Nome : string.Empty,
                    CodigoMotorista = freteSubcontratado.Motorista != null ? freteSubcontratado.Motorista.Codigo : 0,
                    RecebedorDocumento = freteSubcontratado.RecebedorDocumento,
                    DataEntrega = freteSubcontratado.DataEntrega != null ? freteSubcontratado.DataEntrega.ToString() : string.Empty,
                    Observacao = freteSubcontratado.Observacao,
                    ValorMinimo = valorMinimo.ToString("n2")
                };
                return Json(retorno, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes dos Fretes Subcontratados.");
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
                int codigo, numeroCTe, numeroNFe, quantidade, codigoLocalidade, codigoMotorista = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["NumeroNFe"], out numeroNFe);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);
                int.TryParse(Request.Params["Quantidade"], out quantidade);
                int.TryParse(Request.Params["Localidade"], out codigoLocalidade);
                int.TryParse(Request.Params["Motorista"], out codigoMotorista);

                double cnpjParceiro, cnpjRemetente, cnpjDestinatario = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Parceiro"]), out cnpjParceiro);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Remetente"]), out cnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]), out cnpjDestinatario);

                decimal peso, valorFreteTotal, valorICMS, valorFreteLiquido, valorTaxaAdicional, valorTDA, valorTDE, valorCarroDedicado, valorComissao, percentualComissao = 0;
                decimal.TryParse(Request.Params["Peso"], out peso);
                decimal.TryParse(Request.Params["ValorFrete"], out valorFreteTotal);
                decimal.TryParse(Request.Params["ValorICMS"], out valorICMS);
                decimal.TryParse(Request.Params["ValorFreteLiquido"], out valorFreteLiquido);
                decimal.TryParse(Request.Params["ValorTaxaAdicional"], out valorTaxaAdicional);
                decimal.TryParse(Request.Params["ValorTDA"], out valorTDA);
                decimal.TryParse(Request.Params["ValorTDE"], out valorTDE);
                decimal.TryParse(Request.Params["ValorCarroDedicado"], out valorCarroDedicado);
                decimal.TryParse(Request.Params["ValorComissao"], out valorComissao);
                decimal.TryParse(Request.Params["PercentualComissao"], out percentualComissao);

                string filial = Request.Params["Filial"];
                string recebedorDocumento = Request.Params["RecebedorDocumento"];
                string observacao = Request.Params["Observacao"];

                DateTime dataEntrada, dataEntregaAux;
                DateTime? dataEntrega = null;
                DateTime.TryParseExact(Request.Params["DataEntrada"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrada);
                DateTime.TryParseExact(Request.Params["DataEntrega"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEntregaAux);
                if (dataEntregaAux > DateTime.MinValue)
                    dataEntrega = dataEntregaAux;

                Dominio.Enumeradores.TipoFreteSubcontratado tipo;
                Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipo);

                Dominio.Enumeradores.StatusFreteSubcontratado status;
                Enum.TryParse<Dominio.Enumeradores.StatusFreteSubcontratado>(Request.Params["Status"], out status);

                Dominio.Entidades.FreteSubcontratado freteSubcontratado = null;

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Frete Subcontratado negada!");

                    freteSubcontratado = repFreteSubcontratado.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Frete Subcontratado negada!");

                    freteSubcontratado = new Dominio.Entidades.FreteSubcontratado();
                }

                freteSubcontratado.Empresa = this.EmpresaUsuario;
                freteSubcontratado.Filial = filial;
                freteSubcontratado.NumeroCTe = numeroCTe;
                freteSubcontratado.NumeroNFe = numeroNFe;
                freteSubcontratado.DataEntrada = dataEntrada;
                freteSubcontratado.Tipo = tipo;
                freteSubcontratado.Status = status;
                freteSubcontratado.Peso = peso;
                freteSubcontratado.Quantidade = quantidade;
                freteSubcontratado.ValorFreteTotal = valorFreteTotal;
                freteSubcontratado.ValorICMS = valorICMS;
                freteSubcontratado.ValorFreteLiquido = valorFreteLiquido;
                freteSubcontratado.ValorTaxaAdicional = valorTaxaAdicional;
                freteSubcontratado.ValorTDA = valorTDA;
                freteSubcontratado.ValorTDE = valorTDE;
                freteSubcontratado.ValorCarroDedicado = valorCarroDedicado;
                freteSubcontratado.ValorComissao = valorComissao;
                freteSubcontratado.PercentualComissao = percentualComissao;
                freteSubcontratado.RecebedorDocumento = recebedorDocumento;
                freteSubcontratado.DataEntrega = dataEntrega;
                freteSubcontratado.Observacao = observacao;

                Repositorio.Cliente repParceiro = new Repositorio.Cliente(unitOfWork);
                Repositorio.Cliente repRemetente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Cliente repDestinatario = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                freteSubcontratado.Parceiro = repParceiro.BuscarPorCPFCNPJ(cnpjParceiro);
                freteSubcontratado.Remetente = repParceiro.BuscarPorCPFCNPJ(cnpjRemetente);
                freteSubcontratado.Destinatario = repDestinatario.BuscarPorCPFCNPJ(cnpjDestinatario);
                freteSubcontratado.LocalidadeDestino = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                freteSubcontratado.Motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                unitOfWork.Start(System.Data.IsolationLevel.Serializable);

                if (codigo > 0)
                {
                    repFreteSubcontratado.Atualizar(freteSubcontratado);
                }
                else
                {
                    repFreteSubcontratado.Inserir(freteSubcontratado);
                }

                unitOfWork.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Frete Subcontratado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificaDuplicidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int numeroCTe, codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);

                double cnpjParceiro = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Parceiro"]), out cnpjParceiro);

                Dominio.Enumeradores.TipoFreteSubcontratado tipo;
                Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipo);

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);
                Dominio.Entidades.FreteSubcontratado freteSubcontratado = repFreteSubcontratado.BuscaPorParceiroCteTipo(this.EmpresaUsuario.Codigo, codigo, cnpjParceiro, numeroCTe, tipo);

                if (freteSubcontratado == null)
                    return Json<bool>(false, false, "Frete Subcontratado não encontrado.");

                var retorno = new
                {
                    freteSubcontratado.Codigo,
                    NumeroCTe = freteSubcontratado.NumeroCTe,
                    DataEntrada = freteSubcontratado.DataEntrada.ToString("dd/MM/yyyy")
                };
                return Json(retorno, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes dos Fretes Subcontratados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }
}

