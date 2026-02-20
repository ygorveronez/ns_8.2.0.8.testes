using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FretePorValorController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretesporvalor.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string cliente = Request.Params["NomeCliente"];
                string status = Request.Params["Status"];

                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.FretePorValor repFrete = new Repositorio.FretePorValor(unidadeDeTrabalho);

                List<Dominio.Entidades.FretePorValor> listaFrete = repFrete.Consultar(this.EmpresaUsuario.Codigo, status, cliente, cpfCnpjCliente, inicioRegistros, 50);
                int countFrete = repFrete.ContarConsulta(this.EmpresaUsuario.Codigo, status, cliente, cpfCnpjCliente);

                var retorno = (from obj in listaFrete
                               select new
                               {
                                   obj.Codigo,
                                   Nome = obj.ClienteOrigem != null ? obj.ClienteOrigem.Nome : string.Empty,
                                   Destino = obj.LocalidadeDestino != null ? string.Concat(obj.LocalidadeDestino.Estado.Sigla, " / ", obj.LocalidadeDestino.Descricao) : "Todas",
                                   DataInicio = obj.DataInicio.HasValue ? obj.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   DataFim = obj.DataFim.HasValue ? obj.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   ValorMinimoFrete = obj.ValorMinimoFrete.ToString("n2"),
                                   PercentualSobreNF = obj.PercentualSobreNF.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Cliente Origem|24", "Cidade Destino|20", "Data Início|11", "Data Fim|11", "Valor Mínimo|14", "% Sobre NF|10" }, countFrete);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os fretes.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.FretePorValor repFrete = new Repositorio.FretePorValor(unidadeDeTrabalho);
                Dominio.Entidades.FretePorValor frete = repFrete.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                if (frete != null)
                {
                    var retorno = new
                    {
                        frete.Codigo,
                        CPFCNPJClienteOrigem = frete.ClienteOrigem != null ? frete.ClienteOrigem.CPF_CNPJ : 0f,
                        NomeClienteOrigem = frete.ClienteOrigem != null ? string.Concat(frete.ClienteOrigem.CPF_CNPJ_Formatado, " - ", frete.ClienteOrigem.Nome) : string.Empty,
                        DataFim = frete.DataFim.HasValue ? frete.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataInicio = frete.DataInicio.HasValue ? frete.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                        UFDestino = frete.LocalidadeDestino != null ? frete.LocalidadeDestino.Estado.Sigla : string.Empty,
                        CodigoLocalidadeDestino = frete.LocalidadeDestino != null ? frete.LocalidadeDestino.Codigo : 0,
                        frete.Status,
                        ValorMinimoFrete = frete.ValorMinimoFrete.ToString("n2"),
                        PercentualSobreNF = frete.PercentualSobreNF.ToString("n2"),
                        Tipo = frete.Tipo,
                        frete.TipoPagamento,
                        ValorPedagio = frete.ValorPedagio.ToString("n2"),
                        frete.TipoRateio,
                        IncluirICMS = frete.IncluiICMS,
                        frete.IncluirPedagioBC,
                        frete.CodigoIntegracao,
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo, codigoCidadeDestino = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoCidadeDestino"], out codigoCidadeDestino);

                decimal valorMinimoFrete, percentualSobreNF, valorPedagio = 0;
                decimal.TryParse(Request.Params["ValorMinimoFrete"], out valorMinimoFrete);
                decimal.TryParse(Request.Params["PercentualSobreNF"], out percentualSobreNF);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);

                double codigoClienteOrigem = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOrigem"]), out codigoClienteOrigem);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.Enumeradores.TipoFreteValor tipo;
                Enum.TryParse<Dominio.Enumeradores.TipoFreteValor>(Request.Params["Tipo"], out tipo);

                Dominio.Enumeradores.TipoPagamentoFrete tipoPagamento;
                Enum.TryParse<Dominio.Enumeradores.TipoPagamentoFrete>(Request.Params["TipoPagamento"], out tipoPagamento);

                Dominio.Enumeradores.TipoRateioTabelaFreteValor tipoRateio;
                Enum.TryParse<Dominio.Enumeradores.TipoRateioTabelaFreteValor>(Request.Params["TipoRateio"], out tipoRateio);

                Dominio.Enumeradores.IncluiICMSFrete incluirICMS;
                Enum.TryParse<Dominio.Enumeradores.IncluiICMSFrete>(Request.Params["IncluirICMS"], out incluirICMS);

                Dominio.Enumeradores.OpcaoSimNao incluirPedagioBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirPedagioBC"], out incluirPedagioBC);

                Dominio.Enumeradores.TipoTomador tipoCliente;
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["TipoCliente"], out tipoCliente);

                string status = Request.Params["Status"];
                string codigoIntegracao = Request.Params["CodigoIntegracao"];

                bool.TryParse(Request.Params["TodasCidadesDoEstado"], out bool todasCidadesDoEstado);

                Repositorio.FretePorValor repFrete = new Repositorio.FretePorValor(unidadeDeTrabalho);
                Dominio.Entidades.FretePorValor frete;

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

                    frete = new Dominio.Entidades.FretePorValor();
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                if (dataInicial != DateTime.MinValue)
                    frete.DataInicio = dataInicial;
                else
                    frete.DataInicio = null;

                if (dataFinal != DateTime.MinValue)
                    frete.DataFim = dataFinal;
                else
                    frete.DataFim = null;

                if (codigoClienteOrigem > 0)
                    frete.ClienteOrigem = repCliente.BuscarPorCPFCNPJ(codigoClienteOrigem);
                else
                    frete.ClienteOrigem = null;

                if (codigoCidadeDestino > 0 && !todasCidadesDoEstado)
                    frete.LocalidadeDestino = repLocalidade.BuscarPorCodigo(codigoCidadeDestino);
                else
                    frete.LocalidadeDestino = null;

                //if (frete.LocalidadeDestino == null)
                //    return Json<bool>(false, false, "Localidade de destino é obrigatório.");

                Dominio.Entidades.FretePorValor freteExistente = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoCidadeDestino, false, tipoPagamento);

                if (freteExistente != null && freteExistente.Codigo != frete.Codigo && string.IsNullOrWhiteSpace(codigoIntegracao))
                    return Json<bool>(false, false, "Já existe um frete com a mesma origem e a mesma localidade.");

                frete.Empresa = this.EmpresaUsuario;
                frete.PercentualSobreNF = percentualSobreNF;
                frete.ValorMinimoFrete = valorMinimoFrete;
                frete.ValorPedagio = valorPedagio;
                frete.Tipo = tipo;
                frete.TipoPagamento = tipoPagamento;
                frete.TipoRateio = tipoRateio;
                frete.IncluiICMS = incluirICMS;
                frete.IncluirPedagioBC = incluirPedagioBC;
                frete.CodigoIntegracao = codigoIntegracao;
                frete.Status = "A";
                frete.TipoCliente = tipoCliente;

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
                {
                    SalvarTabelaPorCidade(frete, codigoCidadeDestino, unidadeDeTrabalho);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o frete.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        private void SalvarTabelaPorCidade(Dominio.Entidades.FretePorValor frete, int codigoCidadeDestino, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.FretePorValor repFrete = new Repositorio.FretePorValor(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoCidadeDestino);

            if (localidade != null)
            {
                List<Dominio.Entidades.Localidade> listaCidades = repLocalidade.BuscarPorUF(localidade.Estado.Sigla, 0);
                foreach (Dominio.Entidades.Localidade cidade in listaCidades)
                {
                    Dominio.Entidades.FretePorValor fretePorCidade = repFrete.BuscarPorOrigemEDestino(frete.Empresa.Codigo, frete.ClienteOrigem != null ? frete.ClienteOrigem.CPF_CNPJ : 0, cidade.Codigo, false, frete.TipoPagamento);
                    if (fretePorCidade == null)
                        fretePorCidade = new Dominio.Entidades.FretePorValor();

                    fretePorCidade.ClienteOrigem = frete.ClienteOrigem;
                    fretePorCidade.LocalidadeDestino = cidade;
                    fretePorCidade.DataInicio = frete.DataInicio;
                    fretePorCidade.DataFim = frete.DataFim;
                    fretePorCidade.Empresa = frete.Empresa;
                    fretePorCidade.PercentualSobreNF = frete.PercentualSobreNF;
                    fretePorCidade.ValorMinimoFrete = frete.ValorMinimoFrete;
                    fretePorCidade.ValorPedagio = frete.ValorPedagio;
                    fretePorCidade.Tipo = frete.Tipo;
                    fretePorCidade.TipoPagamento = frete.TipoPagamento;
                    fretePorCidade.TipoRateio = frete.TipoRateio;
                    fretePorCidade.IncluiICMS = frete.IncluiICMS;
                    fretePorCidade.IncluirPedagioBC = frete.IncluirPedagioBC;
                    fretePorCidade.CodigoIntegracao = frete.CodigoIntegracao;
                    fretePorCidade.Status = frete.Status;

                    if (fretePorCidade.Codigo > 0)
                        repFrete.Atualizar(fretePorCidade);
                    else
                        repFrete.Inserir(fretePorCidade);
                }
            }
            else
                throw new Exception("Não foi possível selecionar Estado para replicar tabela de frete Valor.");

        }
    }
}
