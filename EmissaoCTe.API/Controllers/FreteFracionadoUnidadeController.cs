using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FreteFracionadoUnidadeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretefracionadounidade.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string cliente = Request.Params["NomeCliente"];
                string cidade = Request.Params["NomeCidade"];
                string status = Request.Params["Status"];
                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.FreteFracionadoUnidade repFrete = new Repositorio.FreteFracionadoUnidade(unitOfWork);

                List<Dominio.Entidades.FreteFracionadoUnidade> listaFrete = repFrete.Consultar(this.EmpresaUsuario.Codigo, status, cliente, cpfCnpjCliente, cidade, inicioRegistros, 50);
                int countFrete = repFrete.ContarConsulta(this.EmpresaUsuario.Codigo, status, cliente, cidade, cpfCnpjCliente);

                var retorno = (from obj in listaFrete
                               select new
                               {
                                   obj.Codigo,
                                   Cliente = obj.ClienteOrigem != null ? obj.ClienteOrigem.Nome : string.Empty,
                                   Destino = obj.LocalidadeDestino != null ? string.Concat(obj.LocalidadeDestino.Estado.Sigla, " / ", obj.LocalidadeDestino.Descricao) : string.Empty,
                                   PesoDe = obj.PesoDe.ToString("n4"),
                                   PesoAte = obj.PesoAte.ToString("n4"),
                                   ValorFrete = obj.ValorFrete.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Cliente Origem|25", "Cidade Destino|20", "Peso De|15", "Peso Até|15", "Valor Frete|15" }, countFrete);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os fretes.");
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
                Repositorio.FreteFracionadoUnidade repFrete = new Repositorio.FreteFracionadoUnidade(unitOfWork);
                Dominio.Entidades.FreteFracionadoUnidade frete = repFrete.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                if (frete != null)
                {
                    var retorno = new
                    {
                        frete.Codigo,
                        CPFCNPJClienteOrigem = frete.ClienteOrigem != null ? frete.ClienteOrigem.CPF_CNPJ_SemFormato : string.Empty,
                        NomeClienteOrigem = frete.ClienteOrigem != null ? string.Concat(frete.ClienteOrigem.CPF_CNPJ_Formatado, " - ", frete.ClienteOrigem.Nome) : string.Empty,
                        UFDestino = frete.LocalidadeDestino != null ? frete.LocalidadeDestino.Estado.Sigla : string.Empty,
                        CodigoLocalidadeDestino = frete.LocalidadeDestino != null ? frete.LocalidadeDestino.Codigo : 0,
                        frete.Status,
                        CodigoUnidadeMedida = frete.UnidadeDeMedida.Codigo,
                        DescricaoUnidadeMedida = string.Concat(frete.UnidadeDeMedida.CodigoDaUnidade, " - ", frete.UnidadeDeMedida.Descricao),
                        PesoDe = frete.PesoDe.ToString("n4"),
                        PesoAte = frete.PesoAte.ToString("n4"),
                        ValorFrete = frete.ValorFrete.ToString("n2"),
                        ValorPedagio = frete.ValorPedagio.ToString("n2"),
                        ValorTAS = frete.ValorTAS.ToString("n2"),
                        PercentualGris = frete.PercentualGris.ToString("n2"),
                        PercentualAdValorem = frete.PercentualAdValorem.ToString("n2"),
                        ValorExcedente = frete.ValorExcedente.ToString("n2"),
                        IncluirICMS = frete.IncluiICMS,
                        ValorMinimoGris = frete.ValorMinimoGris.ToString("n2"),
                        ValorMinimoAdValorem = frete.ValorMinimoAdValorem.ToString("n2"),
                        ValorPorUnidadeMedida = frete.ValorPorUnidadeMedida.ToString("n4"),
                        frete.TipoCliente
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Frete não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do frete.");
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
                int codigo, codigoCidadeDestino, codigoUnidadeMedida = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoCidadeDestino"], out codigoCidadeDestino);
                int.TryParse(Request.Params["CodigoUnidadeMedida"], out codigoUnidadeMedida);

                decimal pesoDe, pesoAte, valorFrete, valorPedagio, valorExcedente, percentualGris, percentualAdValorem, valorTAS, valorPorUnidadeMedida = 0;
                decimal.TryParse(Request.Params["PesoDe"], out pesoDe);
                decimal.TryParse(Request.Params["PesoAte"], out pesoAte);
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);
                decimal.TryParse(Request.Params["ValorTAS"], out valorTAS);
                decimal.TryParse(Request.Params["ValorExcedente"], out valorExcedente);
                decimal.TryParse(Request.Params["PercentualGris"], out percentualGris);
                decimal.TryParse(Request.Params["PercentualAdValorem"], out percentualAdValorem);
                decimal.TryParse(Request.Params["ValorPorUnidadeMedida"], out valorPorUnidadeMedida);                

                decimal.TryParse(Request.Params["ValorMinimoGris"], out decimal valorMinimoGris);
                decimal.TryParse(Request.Params["ValorMinimoAdValorem"], out decimal valorMinimoAdValorem);

                double codigoClienteOrigem = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOrigem"]), out codigoClienteOrigem);

                string status = Request.Params["Status"];

                Dominio.Enumeradores.IncluiICMSFrete incluirICMS;
                Enum.TryParse<Dominio.Enumeradores.IncluiICMSFrete>(Request.Params["IncluirICMS"], out incluirICMS);

                Dominio.Enumeradores.TipoTomador tipoCliente;
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["TipoCliente"], out tipoCliente);

                Repositorio.FreteFracionadoUnidade repFrete = new Repositorio.FreteFracionadoUnidade(unitOfWork);
                Dominio.Entidades.FreteFracionadoUnidade frete = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");
                    frete = repFrete.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                    frete = new Dominio.Entidades.FreteFracionadoUnidade();
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                frete.ClienteOrigem = codigoClienteOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(codigoClienteOrigem) : null;

                frete.LocalidadeDestino = codigoCidadeDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoCidadeDestino) : null;
                if (frete.LocalidadeDestino == null)
                    return Json<bool>(false, false, "Localidade Destino é obrigatório.");

                frete.UnidadeDeMedida = repUnidadeMedida.BuscarPorCodigo(codigoUnidadeMedida);
                if (frete.UnidadeDeMedida == null)
                    return Json<bool>(false, false, "Unidade de medida é obrigatório.");

                //Validar se já existe uma tabela com intervamos de pesos iguais
                //Dominio.Entidades.Frete freteExistente = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoCidadeDestino, false, tipoPagamento);
                //if (freteExistente != null && freteExistente.Codigo != frete.Codigo)
                //    return Json<bool>(false, false, "Já existe um frete com a mesma origem e a mesma localidade.");

                frete.Empresa = this.EmpresaUsuario;
                frete.PesoDe = pesoDe;
                frete.PesoAte = pesoAte;
                frete.ValorFrete = valorFrete;
                frete.ValorExcedente = valorExcedente;
                frete.PercentualGris = percentualGris;
                frete.PercentualAdValorem = percentualAdValorem;
                frete.ValorPedagio = valorPedagio;
                frete.ValorTAS = valorTAS;
                frete.IncluiICMS = incluirICMS;
                frete.TipoCliente = tipoCliente;
                frete.ValorMinimoGris = valorMinimoGris;
                frete.ValorMinimoAdValorem = valorMinimoAdValorem;
                frete.ValorPorUnidadeMedida = valorPorUnidadeMedida;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    frete.Status = status;

                if (codigo > 0)
                    repFrete.Atualizar(frete);
                else
                    repFrete.Inserir(frete);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
       
        #endregion
    }
}
