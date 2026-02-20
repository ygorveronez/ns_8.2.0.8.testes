using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FreteFracionadoValorController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretefracionadovalor.aspx") select obj).FirstOrDefault();
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
                Repositorio.FreteFracionadoValor repFrete = new Repositorio.FreteFracionadoValor(unitOfWork);

                List<Dominio.Entidades.FreteFracionadoValor> listaFrete = repFrete.Consultar(this.EmpresaUsuario.Codigo, status, cliente, cpfCnpjCliente, cidade, inicioRegistros, 50);
                int countFrete = repFrete.ContarConsulta(this.EmpresaUsuario.Codigo, status, cliente, cidade, cpfCnpjCliente);

                var retorno = (from obj in listaFrete
                               select new
                               {
                                   obj.Codigo,
                                   Cliente = obj.ClienteOrigem != null ? obj.ClienteOrigem.Nome : string.Empty,
                                   Destino = obj.LocalidadeDestino != null ? string.Concat(obj.LocalidadeDestino.Estado.Sigla, " / ", obj.LocalidadeDestino.Descricao) : string.Empty,
                                   ValorDe = obj.ValorDe.ToString("n4"),
                                   ValorAte = obj.ValorAte.ToString("n4"),
                                   ValorFrete = obj.ValorFrete.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Cliente Origem|25", "Cidade Destino|20", "Valor De|15", "Valor Até|15", "Valor Frete|15" }, countFrete);
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
                Repositorio.FreteFracionadoValor repFrete = new Repositorio.FreteFracionadoValor(unitOfWork);
                Dominio.Entidades.FreteFracionadoValor frete = repFrete.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
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
                        ValorDe = frete.ValorDe.ToString("n4"),
                        ValorAte = frete.ValorAte.ToString("n4"),
                        ValorFrete = frete.ValorFrete.ToString("n2"),
                        ValorPedagio = frete.ValorPedagio.ToString("n2"),
                        ValorTAS = frete.ValorTAS.ToString("n2"),
                        PercentualGris = frete.PercentualGris.ToString("n2"),
                        PercentualAdValorem = frete.PercentualAdValorem.ToString("n2"),
                        ValorExcedente = frete.ValorExcedente.ToString("n2"),
                        IncluirICMS = frete.IncluiICMS,
                        ValorMinimoGris = frete.ValorMinimoGris.ToString("n2"),
                        ValorMinimoAdValorem = frete.ValorMinimoAdValorem.ToString("n2"),
                        frete.TipoCliente,
                        frete.TipoValor
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

                decimal valorDe, valorAte, valorFrete, valorPedagio, valorExcedente, percentualGris, percentualAdValorem, valorTAS = 0;
                decimal.TryParse(Request.Params["ValorDe"], out valorDe);
                decimal.TryParse(Request.Params["ValorAte"], out valorAte);
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);
                decimal.TryParse(Request.Params["ValorTAS"], out valorTAS);
                decimal.TryParse(Request.Params["ValorExcedente"], out valorExcedente);
                decimal.TryParse(Request.Params["PercentualGris"], out percentualGris);
                decimal.TryParse(Request.Params["PercentualAdValorem"], out percentualAdValorem);

                decimal.TryParse(Request.Params["ValorMinimoGris"], out decimal valorMinimoGris);
                decimal.TryParse(Request.Params["ValorMinimoAdValorem"], out decimal valorMinimoAdValorem);

                double codigoClienteOrigem = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOrigem"]), out codigoClienteOrigem);

                string status = Request.Params["Status"];
                string tipoValor = Request.Params["TipoValor"];

                bool.TryParse(Request.Params["TodasCidadesDoEstado"], out bool todasCidadesDoEstado);

                Dominio.Enumeradores.IncluiICMSFrete incluirICMS;
                Enum.TryParse<Dominio.Enumeradores.IncluiICMSFrete>(Request.Params["IncluirICMS"], out incluirICMS);

                Dominio.Enumeradores.TipoTomador tipoCliente;
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["TipoCliente"], out tipoCliente);

                Repositorio.FreteFracionadoValor repFrete = new Repositorio.FreteFracionadoValor(unitOfWork);
                Dominio.Entidades.FreteFracionadoValor frete = null;

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
                    frete = new Dominio.Entidades.FreteFracionadoValor();
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                frete.ClienteOrigem = codigoClienteOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(codigoClienteOrigem) : null;

                frete.LocalidadeDestino = codigoCidadeDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoCidadeDestino) : null;
                if (frete.LocalidadeDestino == null)
                    return Json<bool>(false, false, "Localidade Destino é obrigatório.");

                frete.Empresa = this.EmpresaUsuario;
                frete.ValorDe = valorDe;
                frete.ValorAte = valorAte;
                frete.ValorFrete = valorFrete;
                frete.ValorExcedente = tipoValor == "V" ? valorExcedente : 0;
                frete.PercentualGris = percentualGris;
                frete.PercentualAdValorem = percentualAdValorem;
                frete.ValorPedagio = valorPedagio;
                frete.ValorTAS = valorTAS;
                frete.IncluiICMS = incluirICMS;
                frete.TipoCliente = tipoCliente;
                frete.ValorMinimoGris = valorMinimoGris;
                frete.ValorMinimoAdValorem = valorMinimoAdValorem;
                frete.TipoValor = tipoValor;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    frete.Status = status;

                if (!todasCidadesDoEstado)
                {
                    if (codigo > 0)
                        repFrete.Atualizar(frete);
                    else
                        repFrete.Inserir(frete);
                }
                else
                    SalvarTabelaPorCidade(frete, codigoCidadeDestino, unitOfWork);

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


        private void SalvarTabelaPorCidade(Dominio.Entidades.FreteFracionadoValor frete, int codigoCidadeDestino, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.FreteFracionadoValor repFrete = new Repositorio.FreteFracionadoValor(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoCidadeDestino);

            if (localidade != null)
            {
                List<Dominio.Entidades.Localidade> listaCidades = repLocalidade.BuscarPorUF(localidade.Estado.Sigla, 0);
                foreach (Dominio.Entidades.Localidade cidade in listaCidades)
                {
                    Dominio.Entidades.FreteFracionadoValor fretePorCidade = repFrete.BuscarPorClienteELocalidadeDestino(frete.Empresa.Codigo, frete.ClienteOrigem?.CPF_CNPJ ?? 0, cidade.Codigo, frete.TipoCliente, frete.Status);
                    if (fretePorCidade == null)
                        fretePorCidade = new Dominio.Entidades.FreteFracionadoValor();

                    fretePorCidade.Empresa = frete.Empresa;
                    fretePorCidade.ClienteOrigem = frete.ClienteOrigem;
                    fretePorCidade.TipoCliente = frete.TipoCliente;
                    fretePorCidade.LocalidadeDestino = cidade;

                    fretePorCidade.ValorDe = frete.ValorDe;
                    fretePorCidade.ValorAte = frete.ValorAte;
                    fretePorCidade.ValorFrete = frete.ValorFrete;
                    fretePorCidade.ValorExcedente = frete.ValorExcedente;
                    fretePorCidade.PercentualGris = frete.PercentualGris;
                    fretePorCidade.PercentualAdValorem = frete.PercentualAdValorem;
                    fretePorCidade.ValorPedagio = frete.ValorPedagio;
                    fretePorCidade.ValorTAS = frete.ValorTAS;
                    fretePorCidade.IncluiICMS = frete.IncluiICMS;
                    fretePorCidade.ValorMinimoGris = frete.ValorMinimoGris;
                    fretePorCidade.ValorMinimoAdValorem = frete.ValorMinimoAdValorem;
                    fretePorCidade.TipoValor = frete.TipoValor;
                    fretePorCidade.Status = frete.Status;

                    if (fretePorCidade.Codigo > 0)
                        repFrete.Atualizar(fretePorCidade);
                    else
                        repFrete.Inserir(fretePorCidade);
                }
            }
            else
                throw new Exception("Não foi possível selecionar Estado para replicar tabela de frete.");

        }

        #endregion
    }
}
