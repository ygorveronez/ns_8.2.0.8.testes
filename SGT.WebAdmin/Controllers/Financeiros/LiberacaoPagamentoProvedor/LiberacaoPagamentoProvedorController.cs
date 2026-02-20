using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.ImpostoMotorista;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositorio.Embarcador.Financeiro;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Financeiros.LiberacaoPagamentoProvedor
{
    [CustomAuthorize("Financeiros/LiberacaoPagamentoProvedor")]
    public class LiberacaoPagamentoProvedorController : BaseController
    {
        #region Construtores

        public LiberacaoPagamentoProvedorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisaDetalhesCarga()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDetalhesCarga());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamentoProvedor = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentosProvedorAnexo repositorioDocumentosProvedorAnexo = new Repositorio.Embarcador.Financeiro.DocumentosProvedorAnexo(unitOfWork);
                Repositorio.Embarcador.Financeiro.RegraPagamentoProvedor repositorioRegra = new Repositorio.Embarcador.Financeiro.RegraPagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamentoProvedor.BuscarPorCodigo(codigo);

                if (pagamentoProvedor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentosProvedorAnexo> documentosProvedorAnexos = repositorioDocumentosProvedorAnexo.BuscarPorPagamnetoProvedor(pagamentoProvedor.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioPagamentoProvedorCarga.BuscarCargasProvedor(pagamentoProvedor.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargas(cargas.Select(o => o.Codigo).ToList());
                List<Dominio.Entidades.Localidade> localidades = pagamentoProvedor.PagamentoProvedorLocalidadePrestacao.Select(o => o.LocalidadePrestacao).ToList();
                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentosProvedorCarga = repositorioPagamentoProvedorCarga.BuscarPorCodigosPagamentoProvedor(pagamentoProvedor.Codigo);

                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacaoAlcadaPagamentoProvedor = repositorioRegra.BuscarPorCodigoRegra(pagamentoProvedor.Codigo);

                var retorno = new
                {
                    Codigo = pagamentosProvedorCarga.Select(o => o.Codigo).ToList(),
                    CodigoPagamentoProvedor = pagamentoProvedor.Codigo,
                    CodigoAprovacaoAlcadaRegra = aprovacaoAlcadaPagamentoProvedor != null ? aprovacaoAlcadaPagamentoProvedor.Codigo : 0,
                    pagamentoProvedor.TipoDocumentoProvedor,
                    pagamentoProvedor.EtapaLiberacaoPagamentoProvedor,
                    ValorCTes = pagamentoProvedor.ValorCTes.ToString("n2"),
                    pagamentoProvedor.NumeroCTes,
                    pagamentoProvedor.MultiplosCTe,
                    DataEmissaoCTes = pagamentoProvedor.DataEmissaoCTes.HasValue ? pagamentoProvedor.DataEmissaoCTes.Value.ToString("dd/MM/yyy") : string.Empty,
                    LocalidadePrestacao = (from obj in localidades
                                           select new
                                           {
                                               obj.Codigo,
                                               obj.Descricao
                                           }).ToList(),
                    ListaDocumentosProvedor = ObterListaDocumentosProvedor(pagamentoProvedor.TipoDocumentoProvedor, pagamentoProvedor, cargaPedidos, localidades, unitOfWork),
                    ListaAprovacao = (pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Aprovacao || pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Liberacao) ? ObterRetornoAprovacao(pagamentoProvedor, cargaPedidos, unitOfWork) : null,
                    Status = pagamentoProvedor.EtapaLiberacaoPagamentoProvedor,
                    Situacao = pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor,
                    Anexos = (
                        from obj in documentosProvedorAnexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList()
                };


                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPagamentoProvedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo, true);

                pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Cancelada;

                repositorioPagamento.Atualizar(pagamentoProvedor, Auditado);

                return new JsonpResult(true, true, "Cancelado com sucesso");
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar o pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ConfirmarDocumentosEmpresaFilial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                TipoDocumentoProvedor tipoDocumentoProvedor = Request.GetEnumParam<TipoDocumentoProvedor>("TipoDocumentoProvedor");

                List<int> codigosCargaPedido = JsonConvert.DeserializeObject<List<int>>(Request.Params("CodigosCargaPedido") ?? "[]");
                List<int> codigosLocaisPrestacao = Request.GetListParam<int>("LocalidadePrestacao");

                if (tipoDocumentoProvedor == TipoDocumentoProvedor.NFSe && codigosLocaisPrestacao.Count == 0)
                    throw new ControllerException("O Tipo de Documento NFSe exige que seja preenchido uma Localidade Prestação.");

                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao repositorioPagamentoPagamentoProvedorLocalidadePrestacao = new Repositorio.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCodigos(codigosCargaPedido);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargasPedido.Select(o => o.Carga).ToList();
                List<Dominio.Entidades.Localidade> localidades = repositorioLocalidade.BuscarPorCodigos(codigosLocaisPrestacao);

                if (pagamentoProvedor != null && pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.DocumentoProvedor)
                    throw new ControllerException("O fluxo de liberação de pagamento ao provedor já foi iniciado com essa carga.");

                if (cargas.Count == 0)
                    throw new Exception("É necessário selecionar uma carga para confirmar.");

                if ((tipoDocumentoProvedor == TipoDocumentoProvedor.CTe || tipoDocumentoProvedor == TipoDocumentoProvedor.CTeComplementar) && codigosCargaPedido.Count > 1)
                    throw new ControllerException("Não é possivel selecionar mais de uma carga quando o Tipo de Documento for CTe ou CTe Complementar.");

                ValidacoesTomadorTipoDocumentoNFSe(tipoDocumentoProvedor, cargasPedido);

                unitOfWork.Start();

                if (pagamentoProvedor == null)
                    pagamentoProvedor = new Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor();
                else
                    pagamentoProvedor.Initialize();

                pagamentoProvedor.TipoDocumentoProvedor = tipoDocumentoProvedor;
                pagamentoProvedor.EtapaLiberacaoPagamentoProvedor = EtapaLiberacaoPagamentoProvedor.DocumentoProvedor;
                pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Aberto;

                pagamentoProvedor.PagamentoProvedorCarga = new List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
                pagamentoProvedor.PagamentoProvedorLocalidadePrestacao = new List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao>();

                if (pagamentoProvedor.Codigo > 0)
                    repositorioPagamento.Atualizar(pagamentoProvedor, Auditado);
                else
                    repositorioPagamento.Inserir(pagamentoProvedor, Auditado);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga = new Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga();
                    pagamentoProvedorCarga.Carga = carga;
                    pagamentoProvedorCarga.PagamentoProvedor = pagamentoProvedor;

                    repositorioPagamentoCarga.Inserir(pagamentoProvedorCarga);

                    List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentosRejeitados = repositorioPagamentoCarga.BuscarPagamentosRejeitadosPorCarga(carga.Codigo);

                    if (pagamentosRejeitados != null && pagamentosRejeitados.Count > 0)
                    {
                        foreach (var pagamentoRejeitado in pagamentosRejeitados)
                        {
                            pagamentoRejeitado.PagamentoProvedor.NovoFluxoIniciado = true;
                            repositorioPagamento.Atualizar(pagamentoRejeitado.PagamentoProvedor);
                        }
                    }
                }

                foreach (Dominio.Entidades.Localidade localidade in localidades)
                {
                    Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao pagamentoPagamentoProvedorLocalidadePrestacao = new Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao();
                    pagamentoPagamentoProvedorLocalidadePrestacao.LocalidadePrestacao = localidade;
                    pagamentoPagamentoProvedorLocalidadePrestacao.PagamentoProvedor = pagamentoProvedor;

                    repositorioPagamentoPagamentoProvedorLocalidadePrestacao.Inserir(pagamentoPagamentoProvedorLocalidadePrestacao);
                }

                unitOfWork.CommitChanges();

                dynamic retorno = ObterListaDocumentosProvedor(tipoDocumentoProvedor, pagamentoProvedor, cargasPedido, localidades, unitOfWork);

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarDocumentosProvedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoProvedor = Request.GetIntParam("CodigoPagamentoProvedor");
                int numeroNFS = Request.GetIntParam("NumeroNFS");
                decimal valorTotalProvedor = Request.GetDecimalParam("ValorTotalProvedor");
                DateTime dataEmissao = Request.GetDateTimeParam("DataEmissao");
                bool multiplosCTe = Request.GetBoolParam("MultiplosCTe");
                decimal valorCTes = Request.GetDecimalParam("ValorCTes");
                string numeroCTes = Request.GetStringParam("NumeroCTes");
                DateTime dataEmissaoCTes = Request.GetDateTimeParam("DataEmissaoCTes");

                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga = repositorioPagamentoCarga.BuscarPorCodigoPagamentoProvedor(codigoPagamentoProvedor);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigoPagamentoProvedor);

                if (pagamentoProvedor == null)
                    throw new ControllerException("Não foi localizado o registro.");

                Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

                unitOfWork.Start();

                pagamentoProvedor.EtapaLiberacaoPagamentoProvedor = EtapaLiberacaoPagamentoProvedor.Aprovacao;
                pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Aberto;

                if (multiplosCTe)
                {
                    pagamentoProvedor.MultiplosCTe = multiplosCTe;

                    if (dataEmissaoCTes == DateTime.MinValue)
                        throw new ControllerException("Preencha data de emissão CT-e's");

                    pagamentoProvedor.DataEmissaoCTes = dataEmissaoCTes;
                    pagamentoProvedor.DataEmissaoNFSe = dataEmissaoCTes;

                    if (valorCTes == 0)
                        throw new ControllerException("Preencha valor CT-e's");

                    pagamentoProvedor.ValorCTes = valorCTes;
                    pagamentoProvedor.ValorNFSe = valorCTes;
                    pagamentoProvedor.ValorAReceberCTe = valorCTes;
                    pagamentoProvedor.ValorProvedor = valorCTes;
                    pagamentoProvedor.AliquotaCTeProvedor = valorCTes;
                    pagamentoProvedor.ValorICMSCTe = valorCTes;

                    if (string.IsNullOrWhiteSpace(numeroCTes))
                        throw new ControllerException("Preencha número CT-e's");

                    pagamentoProvedor.NumeroCTes = numeroCTes;
                }
                else if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe)
                {
                    if (numeroNFS == 0 || dataEmissao == DateTime.MinValue || valorTotalProvedor == 0)
                        throw new ControllerException("Quando o Tipo de Documento for NFSe é preciso preencher os campos.");

                    pagamentoProvedor.ValorProvedor = valorTotalProvedor;
                    pagamentoProvedor.ValorNFSe = valorTotalProvedor;
                    pagamentoProvedor.DataEmissaoNFSe = dataEmissao;
                    pagamentoProvedor.NumeroNFSe = numeroNFS;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = servicoAprovacaoPagamentoProvedor.ObterArquivoIntegracao(pagamentoProvedorCarga.PagamentoProvedor);

                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoAprovacaoPagamentoProvedor.LerCTeDeArquivo(arquivoIntegracao);

                    if (cte == null)
                        throw new ControllerException("CT-e não encontrado.");

                    pagamentoProvedor.ValorNFSe = cte.ValorFrete?.ValorTotalAReceber ?? 0;
                    pagamentoProvedor.DataEmissaoNFSe = cte.DataEmissao;
                    pagamentoProvedor.NumeroNFSe = cte.Numero;

                    SalvarXMLCTe(pagamentoProvedor, unitOfWork);
                }

                repositorioPagamento.Atualizar(pagamentoProvedor, Auditado);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    Status = pagamentoProvedor.EtapaLiberacaoPagamentoProvedor,
                    Situacao = pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor,
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PreencherDocumentosAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoProvedor = Request.GetIntParam("CodigoPagamentoProvedor");

                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigoPagamentoProvedor);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioPagamentoProvedorCarga.BuscarCargasProvedor(pagamentoProvedor.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargas(cargas.Select(o => o.Codigo).ToList());

                if (pagamentoProvedor == null)
                    throw new ControllerException("Não foi localizado o registro.");

                dynamic retorno = ObterRetornoAprovacao(pagamentoProvedor, cargaPedidos, unitOfWork);

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CriarAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool houveDivergenciaEntreCampos = Request.GetBoolParam("HouveDivergenciaEntreCampos");
                int codigoPagamentoProvedor = Request.GetIntParam("CodigoPagamentoProvedor");
                decimal valorTotalProvedorCargas = 0;

                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigoPagamentoProvedor);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga = repositorioPagamentoProvedorCarga.BuscarPorCodigoPagamentoProvedor(codigoPagamentoProvedor);

                Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

                if (pagamentoProvedor == null)
                    throw new ControllerException("Não foi localizado o registro.");

                if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentoProvedorCargas = repositorioPagamentoProvedorCarga.BuscarCodigoPagamentoProvedorCargas(pagamentoProvedor.Codigo);

                    valorTotalProvedorCargas = ObterValorTotalProvedor(pagamentoProvedorCargas);
                }

                servicoAprovacaoPagamentoProvedor.CriarAprovacao(pagamentoProvedor, pagamentoProvedorCarga, houveDivergenciaEntreCampos, valorTotalProvedorCargas, repositorioPagamento, TipoServicoMultisoftware);

                var retorno = new
                {
                    Status = pagamentoProvedor.EtapaLiberacaoPagamentoProvedor,
                    Situacao = pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor,
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarPDF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.ArquivoIntegracao repositorioArquivoIntegracao = new Repositorio.Embarcador.Integracao.ArquivoIntegracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        if (!extensao.Equals(".pdf"))
                            throw new ControllerException("A extensão do arquivo é inválida.");

                        string caminho = ObterCaminhoArquivos(unitOfWork);

                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, file.FileName);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                        file.InputStream.Position = 0;
                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, ConverToBytes(file));

                        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
                        {
                            NomeArquivo = caminho
                        };

                        unitOfWork.Start();

                        repositorioArquivoIntegracao.Inserir(arquivoIntegracao);

                        pagamentoProvedor.ArquivoPDF = arquivoIntegracao;

                        repositorioPagamento.Atualizar(pagamentoProvedor);

                        unitOfWork.CommitChanges();
                    }
                }

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.ArquivoIntegracao repositorioArquivoIntegracao = new Repositorio.Embarcador.Integracao.ArquivoIntegracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    throw new ControllerException("Nenhum arquivo enviado.");

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];

                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                    if (!extensao.Equals(".xml"))
                        throw new ControllerException("A extensão do arquivo é invalida.");

                    unitOfWork.Start();

                    if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTe || pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTeComplementar)
                        ValidacaoXMLCTe(file);

                    string caminho = ObterCaminhoArquivos(unitOfWork);

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, file.FileName);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                    file.InputStream.Position = 0;
                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, ConverToBytes(file));

                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
                    {
                        NomeArquivo = caminho
                    };

                    repositorioArquivoIntegracao.Inserir(arquivoIntegracao);

                    if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTe || pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTeComplementar)
                        pagamentoProvedor.ArquivoXMLCTe = arquivoIntegracao;
                    else if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe)
                        pagamentoProvedor.ArquivoXMLNFSe = arquivoIntegracao;

                    repositorioPagamento.Atualizar(pagamentoProvedor);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);

            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o XML do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoPagamentoProvedor");

                Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = servicoAprovacaoPagamentoProvedor.ObterArquivoIntegracao(pagamentoProvedor);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoIntegracao.NomeArquivo))
                    return new JsonpResult(true, "Arquivo não encontrado para Download");

                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoIntegracao.NomeArquivo);

                return Arquivo(arquivo, "text/xml", Path.GetFileName(arquivoIntegracao.NomeArquivo));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDACTE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoPagamentoProvedor");

                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo);

                if (pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.Nenhum)
                    return new JsonpResult(true, "Não foi possível realizar o download.");

                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = pagamentoProvedor.ArquivoPDF;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoIntegracao.NomeArquivo))
                    return new JsonpResult(true, "Arquivo não encontrado para Download");

                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoIntegracao.NomeArquivo);

                return Arquivo(arquivo, "application/pdf", Path.GetFileName(arquivoIntegracao.NomeArquivo));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoPagamentoProvedor");

                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.BuscarPorCodigo(codigo);

                if (pagamentoProvedor == null)
                    throw new ControllerException("Não foi localizado o registro.");

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoAprovacaoPagamentoProvedor.LerCTeDeArquivo(pagamentoProvedor.ArquivoXMLCTe);

                if (cte == null)
                    throw new ControllerException("Não foi possível exibir os detalhes.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeXML = repositorioCargaCTe.BuscarCTePorNumeroCTe(cte.Numero);

                return new JsonpResult(new { Codigo = cargaCTeXML?.CTe?.Codigo ?? 0 });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Provedor", "Provedor", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Carga", "Carga", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "SituacaoProvedor", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Etapa", "Etapa", 25, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoProvedor filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamentoProvedor = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int totalRegistros = repositorioPagamentoProvedorCarga.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> pagamentosProvedor =
                    (totalRegistros > 0) ? repositorioPagamentoProvedorCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>();

                //var listarRejeitados = pagamentosProvedor
                //                       .Where(obj => obj.SituacaoLiberacaoPagamentoProvedor == SituacaoLiberacaoPagamentoProvedor.Rejeitada)
                //                       .Select(obj => obj.Codigo)
                //                       .ToList();

                var listaPagamentoProvedor = (
                   from obj in pagamentosProvedor
                   select new
                   {
                       Codigo = obj.Codigo,
                       Provedor = string.Join(", ", repositorioCargaPedido.BuscarProvedorPedidoPorPagamentoProvedor(obj.Codigo)),
                       Carga = string.Join(", ", repositorioPagamentoProvedorCarga.BuscarNumerosCargasPorPagamentoProvedor(obj.Codigo)),
                       SituacaoProvedor = obj.SituacaoLiberacaoPagamentoProvedor.ObterDescricao(),
                       Etapa = obj.EtapaLiberacaoPagamentoProvedor.ObterDescricao()
                   }
               ).ToList();

                //foreach (var item in listaPagamentoProvedor)
                //{
                //    if (filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor.Count == 1 &&
                //        filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor.Contains(SituacaoLiberacaoPagamentoProvedor.Rejeitada))
                //    {
                //        listaPagamentoProvedor = listaPagamentoProvedor.ToList();
                //    }
                //    else
                //    {
                //        listaPagamentoProvedor = listaPagamentoProvedor
                //           .Where(item => !listarRejeitados.Contains(item.Codigo))
                //           .ToList();
                //    }
                //}

                grid.AdicionaRows(listaPagamentoProvedor);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaDetalhesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Numero da Carga", "Carga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Local de Prestação", "LocalPrestacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Provedor", "Provedor", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Total Provedor", "ValorTotalProvedor", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga filtrosPesquisa = ObterFiltrosPesquisaDetalhesCarga();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedor = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);

                int totalRegistros = repositorioPagamentoProvedor.ContarConsultaCargasPedidos(filtrosPesquisa, parametrosConsulta);

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.LiberacaoPagamentoProvedorCargaPedido> cargasPedidos = totalRegistros > 0 ? repositorioPagamentoProvedor.BuscarCargasPedidosPagamentoProvedor(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.LiberacaoPagamentoProvedorCargaPedido>();

                var listaPagamentoProvedor = (
                                    from obj in cargasPedidos
                                    select new
                                    {
                                        obj.Codigo,
                                        Carga = obj.Carga,
                                        Tomador = obj.EmpresaTomador,
                                        Provedor = obj.Provedor,
                                        ValorTotalProvedor = obj.ValorTotalPrestacao,
                                        LocalPrestacao = obj.Localidade
                                    }
                                ).ToList();

                grid.AdicionaRows(listaPagamentoProvedor);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoProvedor ObterFiltrosPesquisa()
        {
            double provedor;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                provedor = this.Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;
            else
                provedor = Request.GetDoubleParam("Provedor");

            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoProvedor()
            {
                CodigoProvedor = provedor,
                CodigoCarga = Request.GetIntParam("Carga"),
                SituacaoLiberacaoPagamentoProvedor = Request.GetListEnumParam<SituacaoLiberacaoPagamentoProvedor>("Situacao"),
                EtapaLiberacaoPagamentoProvedor = Request.GetListEnumParam<EtapaLiberacaoPagamentoProvedor>("Etapa"),
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga ObterFiltrosPesquisaDetalhesCarga()
        {
            double provedor = Request.GetDoubleParam("TransportadorProvedor");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                provedor = this.Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;

            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga()
            {
                Codigo = Request.GetListParam<int>("Codigo"),
                Provedor = provedor,
                EmpresaTomador = Request.GetIntParam("Tomador"),
                Carga = Request.GetIntParam("NumeroCarga"),
                Localidade = Request.GetListParam<int>("LocalidadePrestacao"),
                DocumentoProvedor = Request.GetEnumParam<TipoDocumentoProvedor>("TipoDocumentoProvedor"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CodigoProvedor = provedor
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "SituacaoLiberacaoPagamentoProvedor")
                return "SituacaoLiberacaoPagamentoProvedor";

            return propriedadeOrdenar;
        }

        private string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao", "LiberacaoPagamentoProvedor" })}";
        }

        private static byte[] ConverToBytes(Servicos.DTO.CustomFile file)
        {
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes((int)file.Length);
            }

            return fileData;
        }

        private void ValidacoesTomadorTipoDocumentoNFSe(TipoDocumentoProvedor tipoDocumentoProvedor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido)
        {
            var tomadorReferencia = cargasPedido.FirstOrDefault().Carga.Empresa.Codigo;

            bool todosIguais = cargasPedido.All(cp => cp.Carga.Empresa.Codigo == tomadorReferencia);

            if ((tipoDocumentoProvedor == TipoDocumentoProvedor.NFSe) && !todosIguais)
                throw new ControllerException("Não é possível selecionar cargas com tomadores diferentes.");
        }

        private void ValidacaoXMLCTe(Servicos.DTO.CustomFile file)
        {
            var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);

            if (cteLido == null)
                throw new ControllerException("Não foi possivel ler o arquivo.");
        }

        private void SalvarXMLCTe(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = null;
            arquivoIntegracao = servicoAprovacaoPagamentoProvedor.ObterArquivoIntegracao(pagamentoProvedor);

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoAprovacaoPagamentoProvedor.LerCTeDeArquivo(arquivoIntegracao);

            pagamentoProvedor.ValorAReceberCTe = cte?.ValorFrete?.ValorTotalAReceber ?? 0;
            pagamentoProvedor.AliquotaCTe = cte?.ValorFrete?.ICMS?.Aliquota ?? 0;
            pagamentoProvedor.ValorICMSCTe = cte?.ValorFrete?.ICMS?.ValorICMS ?? 0;
            pagamentoProvedor.Tomador = cte?.Tomador != null && cte.Tomador.CPFCNPJ.ToDouble() > 0 ? repCliente.BuscarPorCPFCNPJ(cte.Tomador.CPFCNPJ.ToDouble()) : repCliente.BuscarPorNomeEndereco(cte.Tomador?.RazaoSocial ?? "", cte.Tomador?.Endereco?.Logradouro ?? "");
            pagamentoProvedor.Remetente = cte?.Remetente != null && cte.Remetente.CPFCNPJ.ToDouble() > 0 ? repCliente.BuscarPorCPFCNPJ(cte.Remetente.CPFCNPJ.ToDouble()) : repCliente.BuscarPorNomeEndereco(cte.Remetente?.RazaoSocial ?? "", cte.Remetente?.Endereco?.Logradouro ?? "");
            pagamentoProvedor.Destinatario = cte?.Destinatario != null && cte.Destinatario.CPFCNPJ.ToDouble() > 0 ? repCliente.BuscarPorCPFCNPJ(cte.Destinatario.CPFCNPJ.ToDouble()) : repCliente.BuscarPorNomeEndereco(cte.Destinatario?.RazaoSocial ?? "", cte.Destinatario?.Endereco?.Logradouro ?? "");
            pagamentoProvedor.Expedidor = cte?.Expedidor != null && cte.Expedidor.CPFCNPJ.ToDouble() > 0 ? repCliente.BuscarPorCPFCNPJ(cte.Expedidor.CPFCNPJ.ToDouble()) : repCliente.BuscarPorNomeEndereco(cte.Expedidor?.RazaoSocial ?? "", cte.Expedidor?.Endereco?.Logradouro ?? "");
            pagamentoProvedor.Recebedor = cte?.Recebedor != null && cte.Recebedor.CPFCNPJ.ToDouble() > 0 ? repCliente.BuscarPorCPFCNPJ(cte.Recebedor.CPFCNPJ.ToDouble()) : repCliente.BuscarPorNomeEndereco(cte.Recebedor?.RazaoSocial ?? "", cte.Recebedor?.Endereco?.Logradouro ?? "");
            pagamentoProvedor.EmissorCTe = cte?.Emitente != null ? repCliente.BuscarPorCPFCNPJ(cte.Emitente.CNPJ.ToDouble()) : null;
        }

        private decimal ObterValorTotalProvedor(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentoProvedorCargas)
        {
            return pagamentoProvedorCargas.Sum(o => o.Carga.ValorTotalProvedor);
        }

        private dynamic ObterListaDocumentosProvedor(TipoDocumentoProvedor tipoDocumentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Localidade> localidades, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
            Repositorio.Aliquota repositorioAliquota = new Repositorio.Aliquota(unitOfWork);
            Repositorio.Embarcador.Financeiro.RegraPagamentoProvedor repositorioRegra = new Repositorio.Embarcador.Financeiro.RegraPagamentoProvedor(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentosProvedorCarga = repositorioPagamentoProvedorCarga.BuscarPorCodigosPagamentoProvedor(pagamentoProvedor.Codigo);
            Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga = repositorioPagamentoProvedorCarga.BuscarPorCodigoPagamentoProvedor(pagamentoProvedor.Codigo);
            Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacaoAlcadaPagamentoProvedor = repositorioRegra.BuscarPorCodigoRegra(pagamentoProvedor.Codigo);

            decimal valorTotalProvedor = ObterValorTotalProvedor(pagamentosProvedorCarga);
            Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            decimal valorAliquotaICMS = 0;
            decimal percentualIncluir = 0;
            bool incluirBase = true;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido?.Remetente, cargaPedido.Pedido?.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Pedido?.Remetente?.Localidade ?? cargaPedido.Origem, cargaPedido.Pedido?.Destinatario?.Localidade ?? cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cargaPedido.BaseCalculoICMS, null, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador, tabelaAliquotas, null);

            if (regraICMS == null || regraICMS?.CodigoRegra == 0)
                valorAliquotaICMS = repositorioAliquota.ObterPercetualAliquota(pagamentoProvedorCarga.Carga.Empresa.Localidade.Estado.Sigla, pagamentoProvedorCarga.Carga.DadosSumarizados.UFOrigens, pagamentoProvedorCarga.Carga.DadosSumarizados.UFDestinos);
            else
                valorAliquotaICMS = regraICMS.Aliquota;

            return new
            {
                Codigo = pagamentosProvedorCarga.Select(o => o.Codigo).ToList(),
                TipoDocumentoProvedor = tipoDocumentoProvedor.ObterDescricao(),
                CodigoPagamentoProvedor = pagamentoProvedor.Codigo,
                CodigoAprovacaoAlcadaRegra = aprovacaoAlcadaPagamentoProvedor != null ? aprovacaoAlcadaPagamentoProvedor.Codigo : 0,
                Status = pagamentoProvedor.EtapaLiberacaoPagamentoProvedor,
                ValorTotalProvedor = pagamentoProvedor?.ValorProvedor > 0 ? pagamentoProvedor.ValorProvedor : 0,
                DataEmissao = pagamentoProvedor.DataEmissaoNFSe.HasValue ? pagamentoProvedor.DataEmissaoNFSe.Value.ToString("dd/MM/yyy") : string.Empty,
                NumeroNFS = pagamentoProvedor?.NumeroNFSe > 0 ? pagamentoProvedor.NumeroNFSe : 0,
                Situacao = pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor,
                Tomador = cargaPedido?.Carga?.Empresa?.NomeCNPJ ?? cargaPedido?.Pedido?.Empresa?.NomeCNPJ ?? string.Empty,
                Carga = (from obj in cargaPedidos
                         select new
                         {
                             Codigo = obj.Codigo,
                             Transportador = obj.Pedido.ProvedorOS?.NomeCNPJ ?? string.Empty,
                             Remetente = obj.Pedido.Remetente?.NomeCNPJ ?? string.Empty,
                             Destinatario = obj.Pedido.Destinatario?.NomeCNPJ ?? string.Empty,
                             Expedidor = obj.Pedido.Expedidor?.NomeCNPJ ?? string.Empty,
                             Recebedor = obj.Pedido.Recebedor?.NomeCNPJ ?? string.Empty,
                             ICMS = pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTe || pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.CTeComplementar ? (valorTotalProvedor * (valorAliquotaICMS / 100)).ToString("n2") : string.Empty,
                             ValorTotalProvedor = obj.Carga.ValorTotalProvedor,
                             NumeroCarga = obj.Carga.CodigoCargaEmbarcador
                         }).ToList(),
                LocalidadePrestacao = (from obj in localidades
                                       select new
                                       {
                                           Codigo = obj.Codigo,
                                           Descricao = obj.Descricao
                                       }).ToList(),
            };
        }

        private dynamic ObterRetornoAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

            Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Aliquota repositorioAliquota = new Repositorio.Aliquota(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentoProvedorCargas = repositorioPagamentoProvedorCarga.BuscarCodigoPagamentoProvedorCargas(pagamentoProvedor.Codigo);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga = repositorioPagamentoProvedorCarga.BuscarPorCodigoPagamentoProvedor(pagamentoProvedor.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorUnicaCarga(pagamentoProvedorCarga.Carga.Codigo);
            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = null;
            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();

            var tomadorCarga = cargaPedidos.FirstOrDefault()?.Pedido?.Tomador;

            arquivoIntegracao = servicoAprovacaoPagamentoProvedor.ObterArquivoIntegracao(pagamentoProvedor);

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoAprovacaoPagamentoProvedor.LerCTeDeArquivo(arquivoIntegracao);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeXML = repositorioCargaCTe.BuscarCTePorNumeroCTe(cte?.Numero ?? 0);

            List<string> notasFiscaisCTes = cte?.NotasFiscais?.Select(o => o.Numero).ToList() ?? new List<string>();
            List<int> notasFiscaisCargaCTes = cargaCTe?.CTe?.XMLNotaFiscais?.Select(o => o.Numero).ToList() ?? new List<int>();

            decimal valorTotalProvedor = ObterValorTotalProvedor(pagamentoProvedorCargas);
            Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            string descricaoRegraICMS = "Alíquota padrão ICMS";
            decimal valorAliquotaICMS = 0;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

            decimal percentualIncluir = 0;
            bool incluirBase = true;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido?.Remetente, cargaPedido.Pedido?.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Pedido?.Remetente?.Localidade ?? cargaPedido.Origem, cargaPedido.Pedido?.Destinatario?.Localidade ?? cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cargaPedido.BaseCalculoICMS, null, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador, tabelaAliquotas, null);

            valorAliquotaICMS = regraICMS != null && regraICMS.CodigoRegra > 0 ? regraICMS.Aliquota : repositorioAliquota.ObterPercetualAliquota(pagamentoProvedorCarga.Carga.Empresa.Localidade.Estado.Sigla, pagamentoProvedorCarga.Carga.DadosSumarizados.UFOrigens, pagamentoProvedorCarga.Carga.DadosSumarizados.UFDestinos);
            pagamentoProvedor.RegraICMSCTe = string.IsNullOrWhiteSpace(regraICMS?.Descricao) ? descricaoRegraICMS : regraICMS.Descricao;

            return new
            {
                DetalhesDocumento = new
                {
                    ValorTotalProvedor = valorTotalProvedor.ToString("n2"),
                    AliquotaICMS = valorAliquotaICMS.ToString("n2"),
                    ValorICMS = (valorTotalProvedor * (valorAliquotaICMS / 100)).ToString("n2"),
                    Tomador = cargaPedido.Carga.Empresa?.NomeCNPJ ?? string.Empty,
                    Remetente = cargaPedido.Pedido.Remetente?.NomeCNPJ ?? string.Empty,
                    Destinatario = cargaPedido.Pedido.Destinatario?.NomeCNPJ ?? string.Empty,
                    Expedidor = cargaPedido.Pedido.Expedidor?.NomeCNPJ ?? string.Empty,
                    Recebedor = cargaPedido.Pedido.Recebedor?.NomeCNPJ ?? string.Empty,
                    Emissor = cargaPedido.Pedido.ProvedorOS?.NomeCNPJ ?? string.Empty,
                    RegraICMS = pagamentoProvedor.RegraICMSCTe,
                },
                DetalhesDocumentoRecebido = new
                {
                    ValorAReceber = pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe ? pagamentoProvedor.ValorProvedor.ToString("n2") : pagamentoProvedor.MultiplosCTe ? pagamentoProvedor.ValorCTes.ToString("n2") : pagamentoProvedor.ValorAReceberCTe.ToString("n2") ?? string.Empty,
                    AliquotaICMS = (pagamentoProvedor?.AliquotaCTe ?? 0).ToString("n2"),
                    ValorICMS = pagamentoProvedor?.ValorICMSCTe.ToString("n2") ?? string.Empty,
                    Tomador = pagamentoProvedor?.Tomador?.NomeCNPJ ?? string.Empty,
                    Remetente = pagamentoProvedor?.Remetente?.NomeCNPJ ?? string.Empty,
                    Destinatario = pagamentoProvedor?.Destinatario?.NomeCNPJ ?? string.Empty,
                    Recebedor = pagamentoProvedor?.Recebedor?.NomeCNPJ ?? string.Empty,
                    Expedidor = pagamentoProvedor?.Expedidor?.NomeCNPJ ?? string.Empty,
                    Emissor = pagamentoProvedor?.EmissorCTe?.NomeCNPJ ?? string.Empty,
                    DataEmissaoNFSe = pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe || pagamentoProvedor.MultiplosCTe ? pagamentoProvedor.DataEmissaoNFSe.ToDateTimeString() : string.Empty,
                    NumeroNFSe = pagamentoProvedor.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe ? pagamentoProvedor.NumeroNFSe : 0,
                },
                Autorizacao = new
                {
                    Codigo = pagamentoProvedor.Codigo,
                }
            };
        }

        #endregion
    }
}
