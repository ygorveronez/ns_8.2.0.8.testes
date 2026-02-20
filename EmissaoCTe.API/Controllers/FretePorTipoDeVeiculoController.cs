using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FretePorTipoDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretesportipodeveiculo.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string tipoVeiculo = Request.Params["TipoVeiculo"];
                string clienteOrigem = Request.Params["ClienteOrigem"];
                string clienteDestino = Request.Params["ClienteDestino"];
                string status = Request.Params["Status"];

                double.TryParse(Request.Params["CNPJOrigem"], out double cnpjOrigem);
                double.TryParse(Request.Params["CNPJDestino"], out double cnpjDestino);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                Repositorio.FretePorTipoDeVeiculo repFreteTipoVeiculo = new Repositorio.FretePorTipoDeVeiculo(unitOfWork);

                List<Dominio.Entidades.FretePorTipoDeVeiculo> listaFreteTipoVeiculo = repFreteTipoVeiculo.Consultar(this.EmpresaUsuario.Codigo, status, tipoVeiculo, clienteOrigem, cnpjOrigem, clienteDestino, cnpjDestino, inicioRegistros, 50);
                int countFreteTipoVeiculo = repFreteTipoVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, status, tipoVeiculo, clienteOrigem, cnpjOrigem, clienteDestino, cnpjDestino);

                var retorno = (from obj in listaFreteTipoVeiculo
                               select new
                               {
                                   obj.Codigo,
                                   TipoVeiculo = obj.TipoVeiculo.Descricao,
                                   ClienteOrigem = obj.ClienteOrigem != null ? obj.ClienteOrigem.CPF_CNPJ_Formatado + " - " + obj.ClienteOrigem.Nome : obj.CidadeOrigem.Estado.Sigla + " / " + obj.CidadeOrigem.Descricao,
                                   ClienteDestino = obj.ClienteDestino != null && obj.ClienteDestino.CPF_CNPJ > 0 ? obj.ClienteDestino.CPF_CNPJ_Formatado + " - " + obj.ClienteDestino.Nome : obj.CidadeDestino != null && obj.CidadeDestino.Codigo > 0 ? obj.CidadeDestino.Estado.Sigla + " - " + obj.CidadeDestino.Descricao : string.Empty,
                                   ValorFrete = obj.ValorFrete.ToString("n2"),
                                   ValorPedagio = obj.ValorPedagio.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Tipo Veículo|20", "Origem|20", "Destino|20", "Valor Frete|13", "Valor Pedágio|13" }, countFreteTipoVeiculo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao obter os fretes.");
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

                Repositorio.FretePorTipoDeVeiculo repFrete = new Repositorio.FretePorTipoDeVeiculo(unitOfWork);

                Dominio.Entidades.FretePorTipoDeVeiculo frete = repFrete.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (frete == null)
                    return Json<bool>(false, false, "Frete não encontrado.");

                var retorno = new
                {
                    CPFCNPJClienteDestino = frete.ClienteDestino != null && frete.ClienteDestino.CPF_CNPJ > 0 ? frete.ClienteDestino.CPF_CNPJ_Formatado : string.Empty,
                    NomeClienteDestino = frete.ClienteDestino != null && frete.ClienteDestino.CPF_CNPJ > 0 ? frete.ClienteDestino.Nome : string.Empty,
                    CPFCNPJClienteOrigem = frete.ClienteOrigem != null ? frete.ClienteOrigem.CPF_CNPJ_Formatado : null,
                    NomeClienteOrigem = frete.ClienteOrigem != null ? frete.ClienteOrigem.Nome : null,
                    CodigoLocalidadeOrigem = frete.CidadeOrigem != null ? frete.CidadeOrigem.Codigo : 0,
                    DescricaoLocalidadeOrigem = frete.CidadeOrigem != null ? frete.CidadeOrigem.Descricao : string.Empty,
                    UFOrigem = frete.CidadeOrigem != null ? frete.CidadeOrigem.Estado.Sigla : string.Empty,
                    CodigoLocalidadeDestino = frete.CidadeDestino != null && frete.CidadeDestino.Codigo > 0 ? frete.CidadeDestino.Codigo : 0,
                    DescricaoLocalidadeDestino = frete.CidadeDestino != null && frete.CidadeDestino.Codigo > 0 ? frete.CidadeDestino.Descricao : string.Empty,
                    UFDestino = frete.CidadeDestino != null && frete.CidadeDestino.Codigo > 0 ? frete.CidadeDestino.Estado.Sigla : string.Empty,
                    frete.Codigo,
                    DataFinal = frete.DataFinal.HasValue ? frete.DataFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataInicial = frete.DataInicial.HasValue ? frete.DataInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                    frete.Status,
                    CodigoTipoVeiculo = frete.TipoVeiculo.Codigo,
                    DescricaoTipoVeiculo = frete.TipoVeiculo.Descricao,
                    ValorFrete = frete.ValorFrete.ToString("n2"),
                    ValorPedagio = frete.ValorPedagio.ToString("n2"),
                    frete.TipoPagamento,
                    frete.AliquotaICMS,
                    ValorDescarga = frete.ValorDescarga.ToString("n2"),
                    PercentualAdValorem = frete.PercentualAdValorem != null ? frete.PercentualAdValorem.ToString("n2") : "0,00",
                    ValorAdValorem = frete.ValorAdValorem != null ? frete.ValorAdValorem.ToString("n2") : "0,00",
                    frete.AdicionarAdValoremBcICMS,
                    PercentualGris = frete.PercentualGris.ToString("n4"),
                    IncluirICMS = frete.IncluiICMS,
                    AdicionarGrisBcICMS = frete.AdicionarGrisBcICMS,
                    AdicionarPedagioBcICMS = frete.AdicionarPedagioBcICMS,
                    AdicionarDescargaBcICMS = frete.AdicionarDescargaBcICMS
                };

                return Json(retorno, true);

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
                int codigoTipoVeiculo, codigo, codigoLocalidadeOrigem, codigoLocalidadeDestino;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoTipoVeiculo"], out codigoTipoVeiculo);
                int.TryParse(Request.Params["CodigoLocalidadeOrigem"], out codigoLocalidadeOrigem);
                int.TryParse(Request.Params["CodigoLocalidadeDestino"], out codigoLocalidadeDestino);

                double cpfCnpjClienteOrigem, cpfCnpjClienteDestino;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOrigem"]), out cpfCnpjClienteOrigem);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteDestino"]), out cpfCnpjClienteDestino);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                decimal valorFrete, valorPedagio, aliquotaICMS, valorDescarga, percentualGris, percentualAdValorem, valorAdValorem;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);
                decimal.TryParse(Request.Params["AliquotaICMS"], out aliquotaICMS);
                decimal.TryParse(Request.Params["ValorDescarga"], out valorDescarga);
                decimal.TryParse(Request.Params["PercentualGris"], out percentualGris);
                decimal.TryParse(Request.Params["PercentualAdValorem"], out percentualAdValorem);
                decimal.TryParse(Request.Params["ValorAdValorem"], out valorAdValorem);

                Dominio.Enumeradores.TipoPagamentoFrete tipoPagamento;
                Enum.TryParse<Dominio.Enumeradores.TipoPagamentoFrete>(Request.Params["TipoPagamento"], out tipoPagamento);

                Dominio.Enumeradores.IncluiICMSFrete incluirICMS;
                Enum.TryParse<Dominio.Enumeradores.IncluiICMSFrete>(Request.Params["IncluirICMS"], out incluirICMS);

                Dominio.Enumeradores.OpcaoSimNao adicionarGrisBcICMS;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["AdicionarGrisBcICMS"], out adicionarGrisBcICMS);

                Dominio.Enumeradores.OpcaoSimNao adicionarPedagioBcICMS;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["AdicionarPedagioBcICMS"], out adicionarPedagioBcICMS);

                Dominio.Enumeradores.OpcaoSimNao adicionarDescargaBcICMS;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["AdicionarDescargaBcICMS"], out adicionarDescargaBcICMS);

                Dominio.Enumeradores.OpcaoSimNao adicionarAdValoremBcICMS;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["AdicionarAdValoremBcICMS"], out adicionarAdValoremBcICMS);

                string status = Request.Params["Status"];

                bool.TryParse(Request.Params["TodasCidadesDoEstado"], out bool todasCidadesDoEstado);
                if (cpfCnpjClienteOrigem > 0 || cpfCnpjClienteDestino > 0)
                    todasCidadesDoEstado = false;

                Repositorio.FretePorTipoDeVeiculo repFrete = new Repositorio.FretePorTipoDeVeiculo(unitOfWork);
                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.FretePorTipoDeVeiculo freteAux = repFrete.BuscarPorOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, cpfCnpjClienteOrigem, cpfCnpjClienteDestino, codigoLocalidadeOrigem, codigoLocalidadeDestino, codigoTipoVeiculo, null, false, tipoPagamento);

                if (freteAux != null && freteAux.Codigo != codigo)
                    return Json<bool>(false, false, "Já existe um frete com a mesma combinação de Origem, Desino e Tipo de Veículo!");

                Dominio.Entidades.FretePorTipoDeVeiculo frete;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");

                    frete = repFrete.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada!");

                    frete = new Dominio.Entidades.FretePorTipoDeVeiculo();
                    frete.Empresa = this.EmpresaUsuario;
                }

                frete.ClienteDestino = cpfCnpjClienteDestino > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteDestino) : null;
                frete.ClienteOrigem = cpfCnpjClienteOrigem > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteOrigem) : null;
                //frete.CidadeDestino = repLocalidade.BuscarPorCodigo(codigoLocalidadeDestino);
                if (codigoLocalidadeDestino > 0 && !todasCidadesDoEstado)
                    frete.CidadeDestino = repLocalidade.BuscarPorCodigo(codigoLocalidadeDestino);
                else
                    frete.CidadeDestino = null;
                frete.CidadeOrigem = repLocalidade.BuscarPorCodigo(codigoLocalidadeOrigem);
                frete.TipoVeiculo = repTipoVeiculo.BuscarPorCodigo(codigoTipoVeiculo, this.EmpresaUsuario.Codigo);
                frete.IncluiICMS = incluirICMS;
                frete.AdicionarGrisBcICMS = adicionarGrisBcICMS;
                frete.AdicionarPedagioBcICMS = adicionarPedagioBcICMS;
                frete.AdicionarDescargaBcICMS = adicionarDescargaBcICMS;
                frete.AdicionarAdValoremBcICMS = adicionarAdValoremBcICMS;

                if (dataFinal != DateTime.MinValue)
                    frete.DataFinal = dataFinal;
                else
                    frete.DataFinal = null;

                if (dataInicial != DateTime.MinValue)
                    frete.DataInicial = dataInicial;
                else
                    frete.DataInicial = null;

                frete.Status = status;
                frete.ValorFrete = valorFrete;
                frete.ValorPedagio = valorPedagio;
                frete.TipoPagamento = tipoPagamento;
                frete.AliquotaICMS = aliquotaICMS;
                frete.ValorDescarga = valorDescarga;
                frete.PercentualGris = percentualGris;
                frete.PercentualAdValorem = percentualAdValorem;
                frete.ValorAdValorem = valorAdValorem;

                if (!todasCidadesDoEstado)
                {
                    if (codigo > 0)
                        repFrete.Atualizar(frete);
                    else
                        repFrete.Inserir(frete);
                }
                else
                {
                    SalvarTabelaPorCidade(frete, codigoLocalidadeDestino, unitOfWork);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha genérica ao salvar o frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void SalvarTabelaPorCidade(Dominio.Entidades.FretePorTipoDeVeiculo frete, int codigoCidadeDestino, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.FretePorTipoDeVeiculo repFrete = new Repositorio.FretePorTipoDeVeiculo(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoCidadeDestino);

            if (localidade != null)
            {
                List<Dominio.Entidades.Localidade> listaCidades = repLocalidade.BuscarPorUF(localidade.Estado.Sigla, 0);
                foreach (Dominio.Entidades.Localidade cidade in listaCidades)
                {
                    Dominio.Entidades.FretePorTipoDeVeiculo fretePorCidade = repFrete.BuscarPorLocalidadeOrigemDestinoETipoVeiculo(frete.Empresa.Codigo, frete.CidadeOrigem.Codigo, cidade.Codigo, frete.TipoVeiculo.Codigo, frete.Status, false, frete.TipoPagamento);
                    if (fretePorCidade == null)
                        fretePorCidade = new Dominio.Entidades.FretePorTipoDeVeiculo();

                    fretePorCidade.ClienteOrigem = frete.ClienteOrigem;
                    fretePorCidade.ClienteDestino = frete.ClienteDestino;
                    fretePorCidade.CidadeOrigem = frete.CidadeOrigem;
                    fretePorCidade.CidadeDestino = cidade;
                    fretePorCidade.DataInicial = frete.DataInicial;
                    fretePorCidade.DataFinal = frete.DataFinal;
                    fretePorCidade.Empresa = frete.Empresa;
                    fretePorCidade.PercentualAdValorem = frete.PercentualAdValorem;
                    fretePorCidade.PercentualGris = frete.PercentualGris;
                    fretePorCidade.TipoVeiculo = frete.TipoVeiculo;
                    fretePorCidade.TipoPagamento = frete.TipoPagamento;
                    fretePorCidade.IncluiICMS = frete.IncluiICMS;
                    fretePorCidade.Status = frete.Status;
                    fretePorCidade.ValorAdValorem = frete.ValorAdValorem;
                    fretePorCidade.ValorDescarga = frete.ValorDescarga;
                    fretePorCidade.ValorFrete = frete.ValorFrete;
                    fretePorCidade.ValorPedagio = frete.ValorPedagio;
                    fretePorCidade.AdicionarAdValoremBcICMS = frete.AdicionarAdValoremBcICMS;
                    fretePorCidade.AdicionarDescargaBcICMS = frete.AdicionarDescargaBcICMS;
                    fretePorCidade.AdicionarGrisBcICMS = frete.AdicionarGrisBcICMS;
                    fretePorCidade.AdicionarPedagioBcICMS = frete.AdicionarPedagioBcICMS;
                    fretePorCidade.AliquotaICMS = frete.AliquotaICMS;

                    if (fretePorCidade.Codigo > 0)
                        repFrete.Atualizar(fretePorCidade);
                    else
                        repFrete.Inserir(fretePorCidade);
                }
            }
            else
                throw new Exception("Não foi possível selecionar Estado para replicar tabela de frete Valor.");

        }

        #endregion
    }
}
