using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EntregaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("entregas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string placaVeiculo = Request.Params["PlacaVeiculo"];
                string nomeMotorista = Request.Params["NomeMotorista"];

                DateTime dataEntrega;
                DateTime.TryParseExact(Request.Params["DataEntrega"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrega);

                Repositorio.Entrega repEntrega = new Repositorio.Entrega(unidadeDeTrabalho);

                dynamic retorno = repEntrega.Consultar(this.EmpresaUsuario.Codigo, dataEntrega, placaVeiculo, nomeMotorista, inicioRegistros, 50);

                int countEntregas = repEntrega.ContarConsuta(this.EmpresaUsuario.Codigo, dataEntrega, placaVeiculo, nomeMotorista);

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Data|15", "Veículo|15", "Motorista|50" }, countEntregas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as entregas.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros, codigoDestino, numeroInicial, numeroFinal;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CodigoDestino"], out codigoDestino);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]);
                string numeroDocumento = Request.Params["NumeroDocumento"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                dynamic retorno = repCTe.ConsutarParaEntregas(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, cpfCnpjRemetente, cpfCnpjDestinatario, codigoDestino, numeroDocumento, inicioRegistros, 50);

                int countCTes = repCTe.ContarConsutaParaEntregas(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, cpfCnpjRemetente, cpfCnpjDestinatario, codigoDestino, numeroDocumento);

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Inicio Prestação|23", "Término Prestação|23", "Destinatário|24", "Valor Frete|10" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodosCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDestino, numeroInicial, numeroFinal;
                int.TryParse(Request.Params["CodigoDestino"], out codigoDestino);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]);
                string numeroDocumento = Request.Params["NumeroDocumento"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                dynamic retorno = repCTe.ConsutarParaEntregas(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, cpfCnpjRemetente, cpfCnpjDestinatario, codigoDestino, numeroDocumento, 0, 2000);

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoMotorista, codigoVeiculo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                DateTime dataEntrega;
                DateTime.TryParseExact(Request.Params["DataEntrega"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrega);

                string observacao = Request.Params["Observacao"];

                List<int> codigosCTes = JsonConvert.DeserializeObject<List<int>>(Request.Params["CTes"]);

                Repositorio.Entrega repEntrega = new Repositorio.Entrega(unidadeDeTrabalho);

                Dominio.Entidades.Entrega entrega = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    entrega = repEntrega.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    entrega = new Dominio.Entidades.Entrega();
                    entrega.Empresa = this.EmpresaUsuario;
                }

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                entrega.Data = dataEntrega;
                entrega.Motorista = repMotorista.BuscarPorCodigo(codigoMotorista);
                entrega.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                entrega.Observacao = observacao;

                entrega.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                foreach (int codigoCTe in codigosCTes)
                {
                    entrega.CTes.Add(repCTe.BuscarPorCodigo(codigoCTe));
                }

                if (entrega.Motorista == null)
                    return Json<bool>(false, false, "Motorista não encontrado.");

                if (entrega.Veiculo == null)
                    return Json<bool>(false, false, "Veículo não encontrado.");

                if (entrega.Data == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da entrega inválida.");

                if (entrega.CTes.Count <= 0)
                    return Json<bool>(false, false, "O número total de CT-es na entrega é inválido.");

                unidadeDeTrabalho.Start();

                if (entrega.Codigo > 0)
                {
                    repEntrega.Atualizar(entrega);
                }
                else
                {
                    entrega.Numero = repEntrega.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                    repEntrega.Inserir(entrega);
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a entrega.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["CodigoEntrega"], out codigo);

                Repositorio.Entrega repEntrega = new Repositorio.Entrega(unitOfWork);

                Dominio.Entidades.Entrega entrega = repEntrega.BuscarPorCodigo(codigo);

                if (entrega == null)
                    return Json<bool>(false, false, "Entrega não encontrada. Atualize a página e tente novamente.");

                var retorno = new
                {
                    entrega.Codigo,
                    entrega.Numero,
                    Data = entrega.Data.ToString("dd/MM/yyyy"),
                    CodigoMotorista = entrega.Motorista.Codigo,
                    NomeMotorista = entrega.Motorista.CPF + " - " + entrega.Motorista.Nome,
                    CodigoVeiculo = entrega.Veiculo.Codigo,
                    PlacaVeiculo = entrega.Veiculo.Placa,
                    entrega.Observacao,
                    CTes = (from obj in entrega.CTes
                            select new
                            {
                                obj.Codigo,
                                Numero = obj.Numero + " - " + obj.Serie.Numero,
                                InicioPrestacao = obj.LocalidadeInicioPrestacao.Estado.Sigla + " / " + obj.LocalidadeInicioPrestacao.Descricao,
                                TerminoPrestacao = obj.LocalidadeTerminoPrestacao.Estado.Sigla + " / " + obj.LocalidadeTerminoPrestacao.Descricao,
                                Destinatario = obj.Destinatario.CPF_CNPJ + " - " + obj.Destinatario.Nome,
                                ValorFrete = obj.ValorFrete.ToString("n2")
                            }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
