using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DocumentoEntradaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("documentoentrada.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int numero, cfop, inicioRegistros;
                int.TryParse(Request.Params["Numero"], out numero);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CFOP"], out cfop);

                DateTime dataEmissao, dataEntrada;
                DateTime.TryParseExact(Request.Params["DataEntrada"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrada);
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                string nomeFornecedor = Request.Params["NomeFornecedor"];

                Dominio.Enumeradores.StatusDocumentoEntrada statusAux;
                Dominio.Enumeradores.StatusDocumentoEntrada? status = null;
                if (Enum.TryParse<Dominio.Enumeradores.StatusDocumentoEntrada>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                Repositorio.DocumentoEntrada repDocumento = new Repositorio.DocumentoEntrada(unitOfWork);

                List<Dominio.Entidades.DocumentoEntrada> documentos = repDocumento.Consultar(this.EmpresaUsuario.Codigo, dataEntrada, dataEmissao, numero, nomeFornecedor, status, cfop, inicioRegistros, 50);
                int countDocumentos = repDocumento.ContarConsulta(this.EmpresaUsuario.Codigo, dataEntrada, dataEmissao, numero, nomeFornecedor, status, cfop);

                var retorno = from obj in documentos
                              select new
                              {
                                  obj.Codigo,
                                  NumeroLancamento = obj.NumeroLancamento.ToString(),
                                  DataEntrada = obj.DataEntrada.ToString("dd/MM/yyyy"),
                                  DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                  Numero = obj.Numero.ToString(),
                                  Fornecedor = obj.Fornecedor.Nome,
                                  ValorTotal = obj.ValorTotal.ToString("n2"),
                                  obj.DescricaoStatus
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Lcto.|10", "Dt. Entrada|12", "Dt. Emissão|12", "Número|12", "Fornecedor|22", "Valor Total|12", "Status|10" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os documentos de entrada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["CodigoDocumento"], out codigo);

                Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unidadeDeTrabalho);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
                
                Dominio.Entidades.DocumentoEntrada documento = repDocumentoEntrada.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (documento == null)
                    return Json<bool>(false, false, "Documento de entrada não encontrado.");

                Repositorio.ItemDocumentoEntrada repItem = new Repositorio.ItemDocumentoEntrada(unidadeDeTrabalho);
                Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unidadeDeTrabalho);
                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);
                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);

                List<Dominio.Entidades.ItemDocumentoEntrada> itens = repItem.BuscarPorDocumentoEntrada(documento.Codigo);
                List<Dominio.Entidades.ParcelaDocumentoEntrada> duplicatas = repParcelaDocumentoEntrada.BuscarPorDocumentoEntrada(documento.Codigo);
                List<Dominio.Entidades.Abastecimento> abastecimentos = repAbastecimento.BuscarPorDocumentoEntrada(documento.Codigo);
                
                Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorDocumentoEntrada(this.EmpresaUsuario.Codigo, codigo);
                bool parcelasPagas = false;
                if (duplicata != null)
                {
                    List<Dominio.Entidades.DuplicataParcelas> duplicatasParcelas = repDuplicataParcelas.BuscarPorDuplicata(duplicata.Codigo);
                    for (var i = 0; i < duplicatasParcelas.Count; i++)
                    {
                        if (duplicatasParcelas[i].ValorPgto > 0)
                        {
                            parcelasPagas = true;
                            break;
                        }
                    }
                }

                var retorno = new
                {
                    BaseCalculoICMS = documento.BaseCalculoICMS.ToString("n2"),
                    BaseCalculoICMSST = documento.BaseCalculoICMSST.ToString("n2"),
                    documento.Codigo,
                    DataEmissao = documento.DataEmissao.ToString("dd/MM/yyyy"),
                    DataEntrada = documento.DataEntrada.ToString("dd/MM/yyyy"),
                    SiglaEspecie = documento.Especie.Sigla,
                    DescricaoEspecie = documento.Especie.Sigla + " - " + documento.Especie.Descricao,
                    CPFCNPJFornecedor = documento.Fornecedor.CPF_CNPJ_Formatado,
                    NomeFornecedor = documento.Fornecedor.Nome,
                    CodigoModelo = documento.Modelo.Codigo,
                    DescricaoModelo = documento.Modelo.Descricao,
                    documento.Numero,
                    documento.NumeroLancamento,
                    documento.Chave,
                    CodigoPlanoConta = documento.PlanoDeConta.Codigo,
                    DescricaoPlanoConta = documento.PlanoDeConta.Conta + " - " + documento.PlanoDeConta.Descricao,
                    documento.Serie,
                    documento.Status,
                    documento.IndicadorPagamento,
                    ValorProdutos = documento.ValorProdutos.ToString("n2"),
                    ValorTotal = documento.ValorTotal.ToString("n2"),
                    ValorTotalCOFINS = documento.ValorTotalCOFINS.ToString("n2"),
                    ValorTotalDesconto = documento.ValorTotalDesconto.ToString("n2"),
                    ValorTotalFrete = documento.ValorTotalFrete.ToString("n2"),
                    ValorTotalICMS = documento.ValorTotalICMS.ToString("n2"),
                    ValorTotalICMSST = documento.ValorTotalICMSST.ToString("n2"),
                    ValorTotalIPI = documento.ValorTotalIPI.ToString("n2"),
                    ValorTotalOutrasDespesas = documento.ValorTotalOutrasDespesas.ToString("n2"),
                    ValorTotalPIS = documento.ValorTotalPIS.ToString("n2"),
                    PlacaVeiculo = documento.Veiculo != null ? documento.Veiculo.Placa : string.Empty,
                    CodigoVeiculo = documento.Veiculo != null ? documento.Veiculo.Codigo : 0,                    
                    Itens = (from obj in itens
                             select new Dominio.ObjetosDeValor.ItemDocumentoEntrada()
                             {
                                 AliquotaICMS = obj.AliquotaICMS,
                                 AliquotaIPI = obj.AliquotaIPI,
                                 BaseCalculoICMS = obj.BaseCalculoICMS,
                                 BaseCalculoICMSST = obj.BaseCalculoICMSST,
                                 BaseCalculoIPI = obj.BaseCalculoIPI,
                                 Codigo = obj.Codigo,
                                 CodigoCFOP = obj.CFOP.Codigo,
                                 CodigoProduto = obj.Produto.Codigo,
                                 CodigoProdutoFornecedor = "",
                                 CodigoUnidadeMedida = obj.UnidadeMedida.Codigo,
                                 CST = obj.CST,
                                 CSTCOFINS = obj.CSTCOFINS,
                                 CSTIPI = obj.CSTIPI,
                                 CSTPIS = obj.CSTPIS,
                                 Desconto = obj.Desconto,
                                 DescricaoCFOP = obj.CFOP.CodigoCFOP.ToString(),
                                 DescricaoProduto = obj.Produto.Descricao,
                                 DescricaoUnidadeMedida = obj.UnidadeMedida.Sigla + " - " + obj.UnidadeMedida.Descricao,
                                 Excluir = false,
                                 Quantidade = obj.Quantidade,
                                 Sequencial = obj.Sequencial,
                                 ValorCOFINS = obj.ValorCOFINS,
                                 ValorFrete = obj.ValorFrete,
                                 ValorICMS = obj.ValorICMS,
                                 ValorICMSST = obj.ValorICMSST,
                                 ValorIPI = obj.ValorIPI,
                                 ValorOutrasDespesas = obj.OutrasDespesas,
                                 ValorPIS = obj.ValorPIS,
                                 ValorTotal = obj.ValorTotal,
                                 ValorUnitario = obj.ValorUnitario
                             }).ToList(),
                    Cobrancas = (from obj in duplicatas
                                 select new Dominio.ObjetosDeValor.ParcelaDocumentoEntrada()
                                 {
                                     Codigo = obj.Codigo,
                                     DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                     Excluir = false,
                                     Numero = obj.Numero,
                                     Valor = obj.Valor
                                 }).ToList(),
                    Abastecimentos = (from obj in abastecimentos
                                      select new Dominio.ObjetosDeValor.AbastecimentoAcertoDeViagem
                                      {
                                          Codigo = obj.Codigo,
                                          CodigoPosto = obj.Posto != null ? obj.Posto.CPF_CNPJ_Formatado : string.Empty,
                                          Data = obj.Data != null ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                          DescricaoPosto = obj.Posto != null ? string.Concat(obj.Posto.CPF_CNPJ_Formatado, " - ", obj.Posto.Nome) : obj.NomePosto,
                                          Excluir = false,
                                          KMFinal = obj.Kilometragem,
                                          KMInicial = obj.KilometragemAnterior,
                                          Litros = obj.Litros.ToString("n2"),
                                          Media = obj.Media.ToString("n2"),
                                          ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                          Pago = obj.Pago
                                      }).ToList(),
                    ParcelasPagas = parcelasPagas
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do documento de entrada.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult ObterDetalhesPorNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                var notaFiscal = MultiSoftware.NFe.Servicos.Leitura.Ler(Request.Files[0].InputStream);

                if (notaFiscal != null)
                {
                    Servicos.DocumentoEntrada svcDocumentoEntrada = new Servicos.DocumentoEntrada(unitOfWork);

                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.DocumentoEntrada documento = svcDocumentoEntrada.ObterDetalhesPorNFe(this.EmpresaUsuario, notaFiscal, unitOfWork);

                    unitOfWork.CommitChanges();

                    return Json(documento, true);
                }
                else
                {
                    return Json<bool>(false, false, "Não foi possível realizar a leitura da NF-e.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler o XML da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoModelo, codigoPlano, numero, codigoVeiculo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Modelo"], out codigoModelo);
                int.TryParse(Request.Params["PlanoConta"], out codigoPlano);
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["Numero"], out numero);

                DateTime dataEmissao, dataEntrada;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                DateTime.TryParseExact(Request.Params["DataEntrada"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrada);

                decimal baseCalculoICMS, valorTotal, valorTotalCOFINS, valorTotalICMS, valorTotalPIS, valorTotalIPI, valorTotalICMSST, baseCalculoICMSST, valorTotalFrete, valorTotalOutrasDespesas, valorTotalDesconto, valorProdutos;
                decimal.TryParse(Request.Params["BaseCalculoICMS"], out baseCalculoICMS);
                decimal.TryParse(Request.Params["ValorTotal"], out valorTotal);
                decimal.TryParse(Request.Params["ValorTotalCOFINS"], out valorTotalCOFINS);
                decimal.TryParse(Request.Params["ValorTotalICMS"], out valorTotalICMS);
                decimal.TryParse(Request.Params["ValorTotalPIS"], out valorTotalPIS);
                decimal.TryParse(Request.Params["ValorTotalIPI"], out valorTotalIPI);
                decimal.TryParse(Request.Params["ValorTotalICMSST"], out valorTotalICMSST);
                decimal.TryParse(Request.Params["BaseCalculoICMSST"], out baseCalculoICMSST);
                decimal.TryParse(Request.Params["ValorTotalFrete"], out valorTotalFrete);
                decimal.TryParse(Request.Params["ValorTotalOutrasDespesas"], out valorTotalOutrasDespesas);
                decimal.TryParse(Request.Params["ValorTotalDesconto"], out valorTotalDesconto);
                decimal.TryParse(Request.Params["ValorProdutos"], out valorProdutos);

                Dominio.Enumeradores.StatusDocumentoEntrada status;
                Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada indicadorPagamento;

                Enum.TryParse<Dominio.Enumeradores.StatusDocumentoEntrada>(Request.Params["Status"], out status);
                Enum.TryParse<Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada>(Request.Params["IndicadorPagamento"], out indicadorPagamento);

                string serie = Request.Params["Serie"];
                string especie = Request.Params["Especie"];
                string chave = Request.Params["Chave"];

                double cnpjFornecedor;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Fornecedor"]), out cnpjFornecedor);

                Dominio.Entidades.DocumentoEntrada documento = null;

                Repositorio.DocumentoEntrada repDocumento = new Repositorio.DocumentoEntrada(unidadeDeTrabalho);
                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);
                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Documento de Entrada negada!");

                    documento = repDocumento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Documento de Entrada negada!");

                    documento = new Dominio.Entidades.DocumentoEntrada();
                }

                // Valida duplicado
                if (this.VerificaDuplicidadeDocumento(unidadeDeTrabalho))
                    return Json<bool>(false, false, "Já existe um documento de entrada com o mesmo número, série e modelo deste fornecedor!");

                /*List<Dominio.Entidades.DocumentoEntrada> listaDocumentosDuplicidade = repDocumento.BuscarPorFornecedorNumeroSerieModelo(this.EmpresaUsuario.Codigo, cnpjFornecedor, numero, serie, codigoModelo);
                if (listaDocumentosDuplicidade != null && listaDocumentosDuplicidade.Count > 0)
                {
                    for (var i = 0; i < listaDocumentosDuplicidade.Count; i++)
                    {
                        if (codigo != listaDocumentosDuplicidade[i].Codigo && listaDocumentosDuplicidade[i].Status != Dominio.Enumeradores.StatusDocumentoEntrada.Cancelado)
                            return Json<bool>(false, false, "Já existe outro documento de entrada com mesmo Modelo, Número e Série deste Fornecedor!");
                    }
                }*/

                if (codigo > 0 && status == Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado)
                {
                    Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorDocumentoEntrada(this.EmpresaUsuario.Codigo, codigo);
                    if (duplicata != null)
                    {
                        List<Dominio.Entidades.DuplicataParcelas> duplicatasParcelas = repDuplicataParcelas.BuscarPorDuplicata(duplicata.Codigo);
                        for (var i = 0; i < duplicatasParcelas.Count; i++)
                        {
                            if (duplicatasParcelas[i].ValorPgto > 0)
                                return Json<bool>(false, false, "Não é possível fazer alterações, duplicata possui parcelas pagas!");
                        }
                    }
                }

                documento.BaseCalculoICMS = baseCalculoICMS;
                documento.BaseCalculoICMSST = baseCalculoICMSST;
                documento.DataEmissao = dataEmissao;
                documento.DataEntrada = dataEntrada;
                documento.Empresa = this.EmpresaUsuario;

                Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                Repositorio.PlanoDeConta repPlano = new Repositorio.PlanoDeConta(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                documento.Especie = repEspecie.BuscarPorSigla(especie);
                documento.Fornecedor = repCliente.BuscarPorCPFCNPJ(cnpjFornecedor);
                documento.Modelo = repModelo.BuscarPorId(codigoModelo);
                documento.PlanoDeConta = repPlano.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlano);
                documento.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                documento.Numero = numero;
                documento.Serie = serie;
                documento.ValorTotal = valorTotal;
                documento.ValorTotalCOFINS = valorTotalCOFINS;
                documento.ValorTotalICMS = valorTotalICMS;
                documento.ValorTotalPIS = valorTotalPIS;
                documento.ValorTotalDesconto = valorTotalDesconto;
                documento.ValorTotalFrete = valorTotalFrete;
                documento.ValorTotalICMSST = valorTotalICMSST;
                documento.ValorTotalIPI = valorTotalIPI;
                documento.ValorTotalOutrasDespesas = valorTotalOutrasDespesas;
                documento.Chave = chave;
                documento.IndicadorPagamento = indicadorPagamento;
                documento.Status = status;
                documento.ValorProdutos = valorProdutos;

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);

                if (codigo > 0)
                {
                    repDocumento.Atualizar(documento);
                }
                else
                {
                    documento.NumeroLancamento = repDocumento.BuscarUltimoNumeroLancamento(this.EmpresaUsuario.Codigo) + 1;

                    repDocumento.Inserir(documento);
                }

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);

                List<Dominio.Entidades.Abastecimento> listaAbastecimento = repAbastecimento.BuscarPorDocumentoEntrada(documento.Codigo);
                if (listaAbastecimento.Count > 0 && status != Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado)
                {
                    unidadeDeTrabalho.Rollback();
                    throw new ControllerException("Esse documento já está atrelado a outro abastecimento.");
                }

                this.SalvarParcelasDocumentoEntrada(documento, unidadeDeTrabalho);

                this.SalvarItens(documento, unidadeDeTrabalho);

                this.SalvarAbastecimentos(documento, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (documento.Status == Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado)
                {

                    Servicos.Duplicatas svcDuplictas = new Servicos.Duplicatas(unidadeDeTrabalho);

                    svcDuplictas.DeletarDuplicataDocumentoEntrada(documento.Empresa.Codigo, documento.Codigo, unidadeDeTrabalho);

                    if (!svcDuplictas.GeraDuplicatasPorDocumentoDeEntrada(documento.Empresa.Codigo, documento.Codigo, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo.Codigo : this.Usuario.Codigo, unidadeDeTrabalho))
                    {
                        documento.Status = Dominio.Enumeradores.StatusDocumentoEntrada.Aberto;
                        repDocumento.Atualizar(documento);

                        return Json<bool>(false, false, "Documento de Entrada foi salvo porém não foi possível gerar Duplicata.");
                    }
                }
                else if (documento.Status == Dominio.Enumeradores.StatusDocumentoEntrada.Cancelado || documento.Status == Dominio.Enumeradores.StatusDocumentoEntrada.Aberto)
                {
                    Servicos.Duplicatas svcDuplictas = new Servicos.Duplicatas(unidadeDeTrabalho);

                    svcDuplictas.DeletarDuplicataDocumentoEntrada(documento.Empresa.Codigo, documento.Codigo, unidadeDeTrabalho);
                }

                return Json<bool>(true, true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return Json<bool>(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o Documento de Entrada.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterAliquotaPisCofinsEmpresa()
        {
            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                {
                    return Json<bool>(false, false, "Empresa sem configuração de PIS e COFINS.");
                }

                var retorno = new
                {
                    AliquotaPIS = this.EmpresaUsuario.Configuracao.AliquotaPIS,
                    AliquotaCOFINS = this.EmpresaUsuario.Configuracao.AliquotaCOFINS
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os Aliquotas do documento de entrada.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificaDuplicidade()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                var retorno = new
                {
                    DocumentoExiste = this.VerificaDuplicidadeDocumento(unidadeDeTrabalho)
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao verificar duplicidade.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool VerificaDuplicidadeDocumento(Repositorio.UnitOfWork unidadeDeTrabalho) {
            int codigo, numero, codigoModelo;
            int.TryParse(Request.Params["Codigo"], out codigo);
            int.TryParse(Request.Params["Numero"], out numero);
            int.TryParse(Request.Params["Modelo"], out codigoModelo);

            double cnpjFornecedor;
            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Fornecedor"]), out cnpjFornecedor);

            string serie = Request.Params["Serie"];

            // Valida entrada
            if (numero == 0 || codigoModelo == 0 || cnpjFornecedor == 0 || string.IsNullOrEmpty(serie))
                return false;

            Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unidadeDeTrabalho);
            Dominio.Entidades.DocumentoEntrada documento = repDocumentoEntrada.BuscarPorParametros(this.EmpresaUsuario.Codigo, codigo, numero, serie, codigoModelo, cnpjFornecedor, null, Dominio.Enumeradores.StatusDocumentoEntrada.Cancelado);

            return (documento != null);
        }

        private void SalvarAbastecimentos(Dominio.Entidades.DocumentoEntrada documento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Abastecimentos"]) && documento.Veiculo != null)
            {
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
                Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira repRetornoAngellira = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira(unidadeDeTrabalho);
                Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento repComissaoAbastecimento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento(unidadeDeTrabalho);

                Servicos.Abastecimento svcAbastecimento = new Servicos.Abastecimento(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.AbastecimentoAcertoDeViagem> abastecimentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.AbastecimentoAcertoDeViagem>>(Request.Params["Abastecimentos"]);

                for (var i = 0; i < abastecimentos.Count; i++)
                {
                    Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigoEDocumentoEntrada(abastecimentos[i].Codigo, documento.Codigo);

                    if (!abastecimentos[i].Excluir)
                    {
                        if (abastecimento == null)
                            abastecimento = new Dominio.Entidades.Abastecimento();
                        else
                            abastecimento.Initialize();

                        abastecimento.DocumentoEntrada = documento;

                        DateTime data;
                        DateTime.TryParseExact(abastecimentos[i].Data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        if (data != DateTime.MinValue)
                            abastecimento.Data = data;
                        else
                            abastecimento.Data = null;

                        abastecimento.Kilometragem = abastecimentos[i].KMFinal;
                        abastecimento.KilometragemAnterior = abastecimentos[i].KMInicial;
                        abastecimento.Litros = decimal.Parse(abastecimentos[i].Litros);
                        abastecimento.Media = decimal.Parse(abastecimentos[i].Media);
                        abastecimento.Situacao = documento.Status == Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado ? "F" : "A";
                        abastecimento.DataAlteracao = DateTime.Now;
                        abastecimento.ValorUnitario = decimal.Parse(abastecimentos[i].ValorUnitario);
                        abastecimento.Veiculo = documento.Veiculo;
                        abastecimento.Pago = abastecimentos[i].Pago;
                        abastecimento.Posto = documento.Fornecedor;
                        abastecimento.NomePosto = string.Empty;
                        abastecimento.Status = "A";                        

                        if (abastecimento.Veiculo != null && abastecimento.Veiculo.KilometragemAtual < abastecimentos[i].KMFinal && abastecimento.Situacao == "F")
                        {
                            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                            abastecimento.Veiculo.KilometragemAtual = int.Parse(abastecimentos[i].KMFinal.ToString());

                            repVeiculo.Atualizar(abastecimento.Veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Abastecimentos em Documento de Entrada");
                        }

                        if (abastecimento.Codigo > 0)
                            repAbastecimento.Atualizar(abastecimento, Auditado);
                        else
                            repAbastecimento.Inserir(abastecimento, Auditado);

                        svcAbastecimento.GerarMovimentoDoFinanceiro(abastecimento.Codigo);
                    }
                    else if (abastecimento != null)
                    {
                        if (!repAcertoAbastecimento.ContemAbastecimentoEmAcerto(abastecimento.Codigo)) //&& !repComissaoAbastecimento.ContemAbastecimentoEmAcerto(abastecimento.Codigo) && !repRetornoAngellira.ContemAbastecimentoEmAcerto(abastecimento.Codigo)
                        {
                            abastecimento.Initialize();

                            svcAbastecimento.DeletarMovimentoDoFinanceiro(abastecimento.Codigo);
                            repAbastecimento.Deletar(abastecimento, Auditado);
                        }
                    }
                }
            }
        }

        private void SalvarItens(Dominio.Entidades.DocumentoEntrada documento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Itens"]))
            {
                List<Dominio.ObjetosDeValor.ItemDocumentoEntrada> itens = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ItemDocumentoEntrada>>(Request.Params["Itens"]);

                if (itens != null)
                {
                    Repositorio.ItemDocumentoEntrada repItem = new Repositorio.ItemDocumentoEntrada(unidadeDeTrabalho);
                    Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
                    Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
                    Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);
                    Repositorio.UnidadeMedidaGeral repUnidadeMedida = new Repositorio.UnidadeMedidaGeral(unidadeDeTrabalho);

                    for (var i = 0; i < itens.Count; i++)
                    {
                        Dominio.Entidades.ItemDocumentoEntrada item = repItem.BuscarPorCodigo(itens[i].Codigo, documento.Codigo);

                        if (!itens[i].Excluir)
                        {
                            if (item == null)
                                item = new Dominio.Entidades.ItemDocumentoEntrada();

                            item.DocumentoEntrada = documento;
                            item.AliquotaICMS = itens[i].AliquotaICMS;
                            item.AliquotaIPI = itens[i].AliquotaIPI;
                            item.BaseCalculoICMS = itens[i].BaseCalculoICMS;
                            item.BaseCalculoICMSST = itens[i].BaseCalculoICMSST;
                            item.BaseCalculoIPI = itens[i].BaseCalculoIPI;
                            item.CFOP = repCFOP.BuscarPorId(itens[i].CodigoCFOP);
                            item.CST = itens[i].CST;
                            item.CSTCOFINS = itens[i].CSTCOFINS;
                            item.CSTIPI = itens[i].CSTIPI;
                            item.CSTPIS = itens[i].CSTPIS;
                            item.Desconto = itens[i].Desconto;
                            item.OutrasDespesas = itens[i].ValorOutrasDespesas;
                            item.Produto = repProduto.BuscarPorCodigo(this.EmpresaUsuario.Codigo, itens[i].CodigoProduto);
                            item.Quantidade = itens[i].Quantidade;
                            item.UnidadeMedida = repUnidadeMedida.BuscarPorCodigo(this.EmpresaUsuario.Codigo, itens[i].CodigoUnidadeMedida);
                            item.ValorCOFINS = itens[i].ValorCOFINS;
                            item.ValorFrete = itens[i].ValorFrete;
                            item.ValorICMS = itens[i].ValorICMS;
                            item.ValorICMSST = itens[i].ValorICMSST;
                            item.ValorIPI = itens[i].ValorIPI;
                            item.ValorPIS = itens[i].ValorPIS;
                            item.ValorTotal = itens[i].ValorTotal;
                            item.ValorUnitario = itens[i].ValorUnitario;
                            item.Sequencial = itens[i].Sequencial;

                            if (!string.IsNullOrWhiteSpace(itens[i].CodigoProdutoFornecedor))
                            {
                                Dominio.Entidades.ProdutoFornecedor produtoFornecedor;

                                produtoFornecedor = repProdutoFornecedor.BuscarPorCodigoProdutoEFornecedor(this.EmpresaUsuario.Codigo, itens[i].CodigoProdutoFornecedor, documento.Fornecedor.CPF_CNPJ);

                                if (produtoFornecedor == null)
                                    produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor();

                                produtoFornecedor.CodigoProduto = itens[i].CodigoProdutoFornecedor;
                                produtoFornecedor.Fornecedor = documento.Fornecedor;
                                produtoFornecedor.Produto = item.Produto;

                                if(produtoFornecedor.Codigo > 0)
                                    repProdutoFornecedor.Atualizar(produtoFornecedor);
                                else
                                    repProdutoFornecedor.Inserir(produtoFornecedor);
                            }

                            if (item.Codigo > 0)
                                repItem.Atualizar(item);
                            else
                                repItem.Inserir(item);

                        }
                        else if (item != null && item.Codigo > 0)
                        {
                            repItem.Deletar(item);
                        }
                    }
                }
            }
        }

        private void SalvarParcelasDocumentoEntrada(Dominio.Entidades.DocumentoEntrada documento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Cobrancas"]))
            {
                List<Dominio.ObjetosDeValor.ParcelaDocumentoEntrada> parcelas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ParcelaDocumentoEntrada>>(Request.Params["Cobrancas"]);

                if (parcelas != null)
                {
                    //Servicos.DuplicataDocumentoEntrada svcDuplicata = new Servicos.DuplicataDocumentoEntrada(Conexao.StringConexao);

                    Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unidadeDeTrabalho);
                    Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unidadeDeTrabalho);

                    for (var i = 0; i < parcelas.Count; i++)
                    {
                        Dominio.Entidades.ParcelaDocumentoEntrada parcela = repParcelaDocumentoEntrada.BuscarPorCodigo(parcelas[i].Codigo, documento.Codigo);

                        if (!parcelas[i].Excluir)
                        {
                            if (parcela == null)
                                parcela = new Dominio.Entidades.ParcelaDocumentoEntrada();

                            parcela.DocumentoEntrada = documento;

                            DateTime dataVencimento;
                            DateTime.TryParseExact(parcelas[i].DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                            parcela.DataVencimento = dataVencimento;
                            parcela.Numero = !string.IsNullOrWhiteSpace(parcelas[i].Numero) ? parcelas[i].Numero : string.Empty;
                            parcela.Valor = parcelas[i].Valor;

                            if (parcela.Codigo > 0)
                                repParcelaDocumentoEntrada.Atualizar(parcela);
                            else
                                repParcelaDocumentoEntrada.Inserir(parcela);

                            //svcDuplicata.GerarMovimentoDoFinanceiro(duplicata.Codigo, documento.Codigo, unidadeDeTrabalho);
                        }
                        else if (parcela != null && parcela.Codigo > 0)
                        {
                            //svcDuplicata.DeletarMovimentoDoFinanceiro(duplicata.Codigo, documento.Codigo, unidadeDeTrabalho);

                            repParcelaDocumentoEntrada.Deletar(parcela);
                        }
                    }
                }
            }
        }

        public void GerarMovimentoDoFinanceiro(Dominio.Entidades.DocumentoEntrada documento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (documento != null)
            {
                if (documento.PlanoDeConta != null)
                {
                    Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                    Dominio.Entidades.MovimentoDoFinanceiro movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                    movimento.Data = documento.DataEmissao;
                    movimento.Documento = documento.Numero.ToString();
                    movimento.Empresa = documento.Empresa;
                    movimento.PlanoDeConta = documento.PlanoDeConta;
                    movimento.Valor = documento.ValorTotal;
                    movimento.Pessoa = documento.Fornecedor;
                    movimento.Veiculo = documento.Veiculo;
                    movimento.Observacao = string.Concat("Ref. ao Doc. Entrada nº " + documento.Numero.ToString() + ".");
                    movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;

                    repMovimento.Inserir(movimento);
                }
            }
        }

        #endregion
    }
}
