using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ContratoFinanciamento", "Financeiros/ContratoFinanciamentoParcelaValor")]
    public class ContratoFinanciamentoController : BaseController
    {
        #region Construtores

        public ContratoFinanciamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Numero"), out int numero);

                string numeroDocumento = Request.Params("NumeroDocumento");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Todos;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                double.TryParse(Request.Params("Fornecedor"), out double fornecedor);

                string placaVeiculo = Request.Params("Veiculo");
                int.TryParse(Request.Params("DocumentoEntrada"), out int numeroDocumentoEntrada);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Número Documento", "NumeroDocumento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Capital", "ValorTotal", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor Acréscimo", "ValorAcrescimo", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, false);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento> contratosFinanciamento = repContratoFinanciamento.Consultar(codigoEmpresa, numero, numeroDocumento, fornecedor, situacao, placaVeiculo, numeroDocumentoEntrada, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContratoFinanciamento.ContarConsulta(codigoEmpresa, numero, numeroDocumento, fornecedor, situacao, placaVeiculo, numeroDocumentoEntrada));

                var lista = (from p in contratosFinanciamento
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 p.NumeroDocumento,
                                 Fornecedor = p.Fornecedor != null ? p.Fornecedor.Nome : string.Empty,
                                 DataEmissao = p.DataEmissao.ToString("dd/MM/yyyy"),
                                 ValorTotal = p.ValorTotal.ToString("n2"),
                                 ValorAcrescimo = p.ValorAcrescimo.ToString("n2"),
                                 Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamentoHelper.ObterDescricao(p.Situacao)
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaParcelas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoContratoFinanciamento);

                Models.Grid.EditableCell editableValor = null;
                Models.Grid.EditableCell editableValorStringNumeroDocumento = null;
                Models.Grid.EditableCell editableValorStringObservacao = null;
                Models.Grid.EditableCell editableValorDate = null;
                Models.Grid.EditableCell editableValorInt = null;
                editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 15);
                editableValorStringNumeroDocumento = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 200);
                editableValorStringObservacao = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 5000);
                editableValorDate = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aData);
                editableValorInt = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Sequência", "Sequencia", 10, Models.Grid.Align.right, false, false, false, false, true, editableValorInt);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, false, false, false, false, true, editableValorStringObservacao);
                grid.AdicionarCabecalho("Cod. Barras", "CodigoBarras", 15, Models.Grid.Align.left, false, false, false, false, true, editableValorStringObservacao);
                grid.AdicionarCabecalho("Número Documento", "NumeroDocumento", 20, Models.Grid.Align.left, false, false, false, false, true, editableValorStringNumeroDocumento);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 10, Models.Grid.Align.center, false, false, false, false, true, editableValorDate);
                grid.AdicionarCabecalho("Nº Titulo", "NumeroTitulo", 10, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Situação Titulo", "SituacaoTitulo", 10, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false, false, false, false, true, editableValor);
                grid.AdicionarCabecalho("Valor Acréscimo", "ValorAcrescimo", 10, Models.Grid.Align.right, false, false, false, false, true, editableValor);

                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela> contratosFinanciamentoParcela = repContratoFinanciamentoParcela.Consultar(codigoContratoFinanciamento, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, 50);
                grid.setarQuantidadeTotal(repContratoFinanciamentoParcela.ContarConsulta(codigoContratoFinanciamento));

                var lista = (from obj in contratosFinanciamentoParcela select retornarContratoFinanciamentoParcelaGrid(obj)).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as parcelas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = null;

                int codigo = Request.GetIntParam("Codigo");

                if (codigo > 0)
                    contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo, true);
                else
                    contratoFinanciamento = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento();

                PreencherContratoFinanciamento(contratoFinanciamento, unitOfWork);
                SalvarParcelamento(contratoFinanciamento, unitOfWork);
                repContratoFinanciamento.Inserir(contratoFinanciamento, Auditado);

                InserirValores(contratoFinanciamento, unitOfWork);
                CriarParcelas(contratoFinanciamento, unitOfWork);

                SalvarVeiculos(contratoFinanciamento, unitOfWork);
                SalvarDocumentosEntrada(contratoFinanciamento, unitOfWork);
                SalvarModalidades(contratoFinanciamento, unitOfWork);

                if (contratoFinanciamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                {
                    Servicos.Embarcador.Financeiro.ContratoFinanciamento svcContratoFinanciamento = new Servicos.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);

                    string erro = string.Empty;
                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    if (!svcContratoFinanciamento.AtualizarTitulos(contratoFinanciamento, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    contratoFinanciamento.Codigo,
                    TipoDocumento = contratoFinanciamento.Observacao,
                    NumeroDocumento = contratoFinanciamento.NumeroDocumento,
                    Valor = contratoFinanciamento.ValorTotal.ToString("n2"),
                    Pessoa = new
                    {
                        Codigo = contratoFinanciamento.Fornecedor?.Codigo ?? 0,
                        Descricao = contratoFinanciamento.Fornecedor?.Descricao ?? string.Empty,
                    }
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo, true);

                PreencherContratoFinanciamento(contratoFinanciamento, unitOfWork);
                SalvarParcelamento(contratoFinanciamento, unitOfWork);
                repContratoFinanciamento.Atualizar(contratoFinanciamento, Auditado);

                SalvarVeiculos(contratoFinanciamento, unitOfWork);
                SalvarDocumentosEntrada(contratoFinanciamento, unitOfWork);
                SalvarModalidades(contratoFinanciamento, unitOfWork);
                ExcluirAnexos(unitOfWork);

                if (contratoFinanciamento.ValorTotal > 0 && (contratoFinanciamento.Parcelas == null || contratoFinanciamento.Parcelas.Count == 0))
                    CriarParcelas(contratoFinanciamento, unitOfWork);
                else if (contratoFinanciamento.ValorTotal > 0)
                {
                    decimal valorTotalParcelas = contratoFinanciamento.Parcelas.Sum(o => o.Valor);
                    if (Math.Abs(Math.Round(contratoFinanciamento.ValorTotal) - Math.Round(valorTotalParcelas)) != 0)
                        return new JsonpResult(false, true, "Valor verifique o valor total das parcelas com o total do contrato.");
                }

                if (contratoFinanciamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                {
                    Servicos.Embarcador.Financeiro.ContratoFinanciamento svcContratoFinanciamento = new Servicos.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);

                    string erro = string.Empty;
                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    if (!svcContratoFinanciamento.AtualizarTitulos(contratoFinanciamento, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                unitOfWork.CommitChanges();
                object retorno = new
                {
                    contratoFinanciamento.Codigo,
                    TipoDocumento = contratoFinanciamento.Observacao,
                    NumeroDocumento = contratoFinanciamento.NumeroDocumento,
                    Valor = contratoFinanciamento.ValorTotal.ToString("n2"),
                    Pessoa = new
                    {
                        Codigo = contratoFinanciamento.Fornecedor?.Codigo ?? 0,
                        Descricao = contratoFinanciamento.Fornecedor?.Descricao ?? string.Empty,
                    }
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo, true);

                if (contratoFinanciamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                    return new JsonpResult(false, true, "Não é possível alterar a observação na situação atual do Contrato de Financiamento.");

                if (contratoFinanciamento == null)
                    return new JsonpResult(false, true, "Contrato de Financiamento não encontrado.");

                unitOfWork.Start();

                contratoFinanciamento.Observacao = observacao;
                repContratoFinanciamento.Atualizar(contratoFinanciamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult("Observação alterada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação do contrato de financiamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo);

                var dynContratoFinanciamento = new
                {
                    contratoFinanciamento.Codigo,
                    contratoFinanciamento.Numero,
                    contratoFinanciamento.NumeroDocumento,
                    contratoFinanciamento.Situacao,
                    contratoFinanciamento.Observacao,
                    contratoFinanciamento.FormaTitulo,
                    DataEmissao = contratoFinanciamento.DataEmissao.ToString("dd/MM/yyyy"),
                    ValorTotal = contratoFinanciamento.ValorTotal.ToString("n2"),
                    TipoMovimento = contratoFinanciamento.TipoMovimento != null ? new { contratoFinanciamento.TipoMovimento.Codigo, contratoFinanciamento.TipoMovimento.Descricao } : null,
                    ValorAcrescimo = contratoFinanciamento.ValorAcrescimo.ToString("n2"),
                    TipoMovimentoAcrescimo = contratoFinanciamento.TipoMovimentoAcrescimo != null ? new { contratoFinanciamento.TipoMovimentoAcrescimo.Codigo, contratoFinanciamento.TipoMovimentoAcrescimo.Descricao } : null,
                    Fornecedor = contratoFinanciamento.Fornecedor != null ? new { contratoFinanciamento.Fornecedor.Codigo, contratoFinanciamento.Fornecedor.Descricao } : null,
                    Empresa = contratoFinanciamento.Empresa != null ? new { contratoFinanciamento.Empresa.Codigo, contratoFinanciamento.Empresa.Descricao } : null,
                    Parcelamento = new
                    {
                        VencimentoPrimeiraParcela = contratoFinanciamento.VencimentoPrimeiraParcela.HasValue ? contratoFinanciamento.VencimentoPrimeiraParcela.Value.ToString("dd/MM/yyyy") : "",
                        contratoFinanciamento.Provisao,
                        contratoFinanciamento.Repetir,
                        contratoFinanciamento.Dividir,
                        contratoFinanciamento.Periodicidade,
                        contratoFinanciamento.NumeroOcorrencia,
                        contratoFinanciamento.DiaOcorrencia
                    },
                    Valores = (from obj in contratoFinanciamento.Valores
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.Tipo,
                                   Valor = obj.Valor.ToString("n2"),
                                   CodigoTipoMovimento = obj.TipoMovimento != null ? obj.TipoMovimento.Codigo : 0,
                                   TipoMovimento = obj.TipoMovimento != null ? obj.TipoMovimento.Descricao : string.Empty,
                                   DescricaoTipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaHelper.ObterDescricao(obj.Tipo)
                               }).ToList(),
                    Veiculos = (from obj in contratoFinanciamento.Veiculos
                                select new
                                {
                                    VEICULO = new
                                    {
                                        obj.Veiculo.Codigo,
                                        obj.Veiculo.Placa,
                                        ModeloVeicularCarga = obj.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                        TipoVeiculo = obj.Veiculo.DescricaoTipoVeiculo,
                                        obj.Veiculo.DescricaoTipo
                                    }
                                }).ToList(),
                    DocumentosEntrada = (from obj in contratoFinanciamento.DocumentosEntrada
                                         select new
                                         {
                                             DOCUMENTOENTRADA = new
                                             {
                                                 obj.DocumentoEntradaTMS.Codigo,
                                                 obj.DocumentoEntradaTMS.Numero,
                                                 obj.DocumentoEntradaTMS.Serie,
                                                 DataEmissao = obj.DocumentoEntradaTMS.DataEmissao.ToString("dd/MM/yyyy"),
                                                 DataEntrada = obj.DocumentoEntradaTMS.DataEntrada.ToString("dd/MM/yyyy"),
                                                 obj.DocumentoEntradaTMS.Chave
                                             }
                                         }).ToList(),
                    Modalidades = (from obj in contratoFinanciamento.Modalidades
                                   select new
                                   {
                                       MODALIDADE = new
                                       {
                                           obj.Codigo,
                                           obj.Descricao,
                                           obj.DescricaoAtivo
                                       }
                                   }).ToList(),
                    ListaAnexos = contratoFinanciamento.Anexos != null ? (from obj in contratoFinanciamento.Anexos
                                                                          select new
                                                                          {
                                                                              obj.Codigo,
                                                                              DescricaoAnexo = obj.Descricao,
                                                                              Arquivo = obj.NomeArquivo
                                                                          }).ToList() : null
                };

                return new JsonpResult(dynContratoFinanciamento);
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

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoAnexo repContratoFinanciamentoAnexo = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoAnexo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo);

                if (contratoFinanciamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.GetStringParam("Descricao").Split(',');

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "ContratoFinanciamento");
                
                for (var i = 0; i < arquivos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoAnexo contratoFinanciamentoAnexo = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoAnexo();

                    Servicos.DTO.CustomFile file = arquivos[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();

                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    contratoFinanciamentoAnexo.CaminhoArquivo = caminho;
                    contratoFinanciamentoAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    contratoFinanciamentoAnexo.Descricao = descricoes[i];
                    contratoFinanciamentoAnexo.ContratoFinanciamento = contratoFinanciamento;

                    repContratoFinanciamentoAnexo.Inserir(contratoFinanciamentoAnexo, Auditado);
                }

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    contratoFinanciamento.Codigo,
                    TipoDocumento = contratoFinanciamento.Observacao,
                    NumeroDocumento = contratoFinanciamento.NumeroDocumento,
                    Valor = contratoFinanciamento.ValorTotal.ToString("n2")
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = Request.GetIntParam("CodigoAnexo");

                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoAnexo repContratoFinanciamentoAnexo = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoAnexo contratoFinanciamentoAnexo = repContratoFinanciamentoAnexo.BuscarPorCodigo(codigoAnexo);

                if (contratoFinanciamentoAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(contratoFinanciamentoAnexo.CaminhoArquivo))
                    return new JsonpResult(false, true, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(contratoFinanciamentoAnexo.CaminhoArquivo), "application/octet-stream", contratoFinanciamentoAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosContratoFinanciamentoParcela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("Sequencia"), out int sequencia);
                decimal.TryParse(Request.Params("Valor"), out decimal valor);
                decimal.TryParse(Request.Params("ValorAcrescimo"), out decimal valorAcrescimo);

                string observacao = Request.Params("Observacao");
                string numeroDocumento = Request.Params("NumeroDocumento");
                string codigoBarras = Request.Params("CodigoBarras");

                DateTime.TryParse(Request.Params("DataVencimento"), out DateTime dataVencimento);
                bool alterouApenasVencimento = false;

                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela contratoFinanciamentoParcela = repContratoFinanciamentoParcela.BuscarPorCodigo(codigo);

                if (contratoFinanciamentoParcela.ContratoFinanciamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Cancelado
                   && contratoFinanciamentoParcela.ContratoFinanciamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                {
                    if (dataVencimento > DateTime.MinValue && contratoFinanciamentoParcela.DataVencimento != dataVencimento)
                    {
                        contratoFinanciamentoParcela.DataVencimento = dataVencimento;
                        alterouApenasVencimento = true;
                    }
                    if (sequencia > 0)
                        contratoFinanciamentoParcela.Sequencia = sequencia;

                    contratoFinanciamentoParcela.Valor = valor;

                    if (valorAcrescimo > 0)
                        contratoFinanciamentoParcela.ValorAcrescimo = valorAcrescimo;
                    if (!string.IsNullOrWhiteSpace(numeroDocumento) && contratoFinanciamentoParcela.NumeroDocumento != numeroDocumento)
                    {
                        contratoFinanciamentoParcela.NumeroDocumento = numeroDocumento;
                        alterouApenasVencimento = true;
                    }
                    if (!string.IsNullOrWhiteSpace(codigoBarras) && contratoFinanciamentoParcela.CodigoBarras != codigoBarras)
                    {
                        contratoFinanciamentoParcela.CodigoBarras = codigoBarras;
                        alterouApenasVencimento = true;
                    }
                    if (!string.IsNullOrWhiteSpace(observacao) && observacao != contratoFinanciamentoParcela.Observacao)
                        alterouApenasVencimento = true;
                    contratoFinanciamentoParcela.Observacao = observacao;

                    repContratoFinanciamentoParcela.Atualizar(contratoFinanciamentoParcela);

                    if (!alterouApenasVencimento)
                        AtualizarParcelas(contratoFinanciamentoParcela.ContratoFinanciamento, sequencia, unitOfWork);

                    var dynParcelaContratoFinanciamento = retornarContratoFinanciamentoParcelaGrid(contratoFinanciamentoParcela);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFinanciamentoParcela.ContratoFinanciamento, null, "Alterou Dados da Parcela.", unitOfWork);

                    var retorno = new
                    {
                        dynParcelaContratoFinanciamento
                    };

                    unitOfWork.CommitChanges();
                    return new JsonpResult(retorno);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento situacao = contratoFinanciamentoParcela.ContratoFinanciamento.Situacao;
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A atual situação do Contrato de Financiamento (" + Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamentoHelper.ObterDescricao(situacao) + ") não permite que ela seja alterada ");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados da parcela");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Estornar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unidadeDeTrabalho);
                Servicos.Embarcador.Financeiro.ContratoFinanciamento svcContratoFinanciamento = new Servicos.Embarcador.Financeiro.ContratoFinanciamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo);

                if (contratoFinanciamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                    return new JsonpResult(false, true, "Não é possível estornar na situação atual do contrato de financiamento.");

                unidadeDeTrabalho.Start();

                contratoFinanciamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Aberto;
                repContratoFinanciamento.Atualizar(contratoFinanciamento);

                string erro = string.Empty;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if (!svcContratoFinanciamento.AtualizarTitulos(contratoFinanciamento, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, Auditado))
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorContratoFinanciamento(codigo);

                if (rateioDespesaVeiculo != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rateioDespesaVeiculo, null, "Excluido e revertido o rateio de despesa do veículo a partir do contrato financeiro", unidadeDeTrabalho);

                    rateioDespesaVeiculo.Veiculos = null;
                    rateioDespesaVeiculo.SegmentosVeiculos = null;
                    rateioDespesaVeiculo.CentroResultados = null;

                    repRateioDespesaVeiculoLancamento.DeletarPorRateioDespesaVeiculo(rateioDespesaVeiculo.Codigo);
                    repRateioDespesaVeiculo.Deletar(rateioDespesaVeiculo);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFinanciamento, null, "Estornou o Contrato.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao estornar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> LimparParcelas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo);

                if (contratoFinanciamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Aberto)
                    return new JsonpResult(false, true, "Não é possível remover as parcelas na situação atual do contrato.");

                unidadeDeTrabalho.Start();

                repContratoFinanciamentoParcela.DeletarPorContratoFinanceiro(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFinanciamento, null, "Limpou as Parcelas.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                object retorno = new
                {
                    contratoFinanciamento.Codigo,
                    TipoDocumento = contratoFinanciamento.Observacao,
                    NumeroDocumento = contratoFinanciamento.NumeroDocumento,
                    Valor = contratoFinanciamento.ValorTotal.ToString("n2")
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao estornar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarSomenteVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento = repContratoFinanciamento.BuscarPorCodigo(codigo);

                if (contratoFinanciamento == null)
                    throw new ControllerException("Contrato Financiamento não encontrado");

                SalvarVeiculos(contratoFinanciamento, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar atualizar Veiculos");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        #endregion

        #region Métodos Privados

        private void PreencherContratoFinanciamento(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
            int.TryParse(Request.Params("TipoMovimento"), out int codigoTipoMovimento);
            int.TryParse(Request.Params("TipoMovimentoAcrescimo"), out int codigoTipoMovimentoAcrescimo);

            double.TryParse(Request.Params("Fornecedor"), out double fornecedor);

            string numeroDocumento = Request.Params("NumeroDocumento");
            string observacao = Request.Params("Observacao");

            decimal.TryParse(Request.Params("ValorTotal"), out decimal valorTotal);
            decimal.TryParse(Request.Params("ValorAcrescimo"), out decimal valorAcrescimo);

            DateTime.TryParse(Request.Params("DataEmissao"), out DateTime dataEmissao);

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento situacao);
            Enum.TryParse(Request.Params("FormaTitulo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            if (contratoFinanciamento.Codigo == 0)
            {
                contratoFinanciamento.Numero = repContratoFinanciamento.ProximoNumeroContrato(codigoEmpresa);
                contratoFinanciamento.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            }
            contratoFinanciamento.NumeroDocumento = numeroDocumento;
            contratoFinanciamento.Observacao = observacao;
            contratoFinanciamento.DataEmissao = dataEmissao;
            contratoFinanciamento.ValorTotal = valorTotal;
            contratoFinanciamento.ValorAcrescimo = valorAcrescimo;
            contratoFinanciamento.Situacao = situacao;
            contratoFinanciamento.FormaTitulo = formaTitulo;

            contratoFinanciamento.Fornecedor = repCliente.BuscarPorCPFCNPJ(fornecedor);
            contratoFinanciamento.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
            contratoFinanciamento.TipoMovimentoAcrescimo = codigoTipoMovimentoAcrescimo > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAcrescimo) : null;
        }

        private void SalvarParcelamento(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic parcelamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parcelamento"));

            int.TryParse((string)parcelamento.NumeroOcorrencia, out int numeroOcorrencia);
            int.TryParse((string)parcelamento.DiaOcorrencia, out int diaOcorrencia);

            bool.TryParse((string)parcelamento.Repetir, out bool repetir);
            bool.TryParse((string)parcelamento.Dividir, out bool dividir);
            bool.TryParse((string)parcelamento.Provisao, out bool provisao);

            Enum.TryParse((string)parcelamento.Periodicidade, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade);

            DateTime? vencimentoPrimeiraParcela = ((string)parcelamento.VencimentoPrimeiraParcela).ToNullableDateTime();

            contratoFinanciamento.Repetir = repetir;
            contratoFinanciamento.Dividir = dividir;
            contratoFinanciamento.Provisao = provisao;

            contratoFinanciamento.NumeroOcorrencia = numeroOcorrencia;
            contratoFinanciamento.DiaOcorrencia = diaOcorrencia;
            contratoFinanciamento.Periodicidade = periodicidade;
            contratoFinanciamento.VencimentoPrimeiraParcela = vencimentoPrimeiraParcela.HasValue && vencimentoPrimeiraParcela.Value > DateTime.MinValue ? vencimentoPrimeiraParcela : null;
        }

        private void InserirValores(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoValor repContratoFinanciamentoValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoValor(unidadeTrabalho);

            dynamic valores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Valores"));

            foreach (var valor in valores)
            {
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor contratoFinanciamentoValor = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor();

                int.TryParse((string)valor.CodigoTipoMovimento, out int codigoTipoMovimento);
                Enum.TryParse((string)valor.Tipo, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipo);

                contratoFinanciamentoValor.Descricao = (string)valor.Descricao;
                contratoFinanciamentoValor.Valor = Utilidades.Decimal.Converter((string)valor.Valor);
                contratoFinanciamentoValor.Tipo = tipo;
                contratoFinanciamentoValor.ContratoFinanciamento = contratoFinanciamento;

                if (codigoTipoMovimento > 0)
                    contratoFinanciamentoValor.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                else
                    contratoFinanciamentoValor.TipoMovimento = null;

                repContratoFinanciamentoValor.Inserir(contratoFinanciamentoValor, Auditado);
            }
        }

        private void CriarParcelas(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoValor repContratoFinanciamentoValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoValor(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor> valores = repContratoFinanciamentoValor.BuscarPorContratoFinanciamento(contratoFinanciamento.Codigo);
            int numeroOcorrencia = contratoFinanciamento.NumeroOcorrencia > 0 ? contratoFinanciamento.NumeroOcorrencia : 1;
            int diaOcorrencia = contratoFinanciamento.DiaOcorrencia;
            decimal valorParcela = contratoFinanciamento.ValorTotal;
            decimal valorAcrescimo = contratoFinanciamento.ValorAcrescimo;
            DateTime dataVencimentoParcela = contratoFinanciamento.VencimentoPrimeiraParcela.HasValue ? contratoFinanciamento.VencimentoPrimeiraParcela.Value : contratoFinanciamento.DataEmissao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade = contratoFinanciamento.Periodicidade;
            decimal valorDiferenca = 0;
            decimal valorDiferencaAcrescimo = 0;

            if (contratoFinanciamento.Dividir)
            {
                valorParcela = Math.Round(valorParcela / numeroOcorrencia, 2);
                valorDiferenca = Math.Round(contratoFinanciamento.ValorTotal - (valorParcela * numeroOcorrencia), 2);

                if (valorAcrescimo > 0)
                {
                    valorAcrescimo = Math.Round(valorAcrescimo / numeroOcorrencia, 2);
                    valorDiferencaAcrescimo = Math.Round(contratoFinanciamento.ValorAcrescimo - (valorAcrescimo * numeroOcorrencia), 2);
                }
            }

            for (int i = 0; i < numeroOcorrencia; i++)
            {
                if (i == 0 && contratoFinanciamento.VencimentoPrimeiraParcela.HasValue)
                    dataVencimentoParcela = dataVencimentoParcela;
                else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                    dataVencimentoParcela = dataVencimentoParcela.AddDays(7);
                else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal)
                    dataVencimentoParcela = dataVencimentoParcela.AddMonths(1);
                else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Bimestral)
                    dataVencimentoParcela = dataVencimentoParcela.AddMonths(2);
                else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Trimestral)
                    dataVencimentoParcela = dataVencimentoParcela.AddMonths(3);
                else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semestral)
                    dataVencimentoParcela = dataVencimentoParcela.AddMonths(6);
                else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Anual)
                    dataVencimentoParcela = dataVencimentoParcela.AddYears(1);

                if (diaOcorrencia > 0 && periodicidade != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                {
                    DateTime? novaData = null;
                    try
                    {
                        novaData = new DateTime(dataVencimentoParcela.Year, dataVencimentoParcela.Month, diaOcorrencia);
                    }
                    catch (Exception)
                    {
                        novaData = dataVencimentoParcela;
                    }
                    if (novaData == null)
                        novaData = dataVencimentoParcela;

                    dataVencimentoParcela = novaData.Value;
                }

                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela contratoFinanciamentoParcela = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela();
                contratoFinanciamentoParcela.ContratoFinanciamento = contratoFinanciamento;
                contratoFinanciamentoParcela.Sequencia = i + 1;
                contratoFinanciamentoParcela.NumeroDocumento = contratoFinanciamento.NumeroDocumento;
                contratoFinanciamentoParcela.DataVencimento = dataVencimentoParcela;
                if (i == 0)
                {
                    contratoFinanciamentoParcela.Valor = Math.Round(valorParcela + valorDiferenca, 2);
                    contratoFinanciamentoParcela.ValorAcrescimo = Math.Round(valorAcrescimo + valorDiferencaAcrescimo, 2);
                }
                else
                {
                    contratoFinanciamentoParcela.Valor = valorParcela;
                    contratoFinanciamentoParcela.ValorAcrescimo = valorAcrescimo;
                }
                contratoFinanciamentoParcela.Observacao = contratoFinanciamento.Observacao;

                repContratoFinanciamentoParcela.Inserir(contratoFinanciamentoParcela);
            }
        }

        private void AtualizarParcelas(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, int sequencia, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoValor repContratoFinanciamentoValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoValor(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoValor> valores = repContratoFinanciamentoValor.BuscarPorContratoFinanciamento(contratoFinanciamento.Codigo);
            int numeroOcorrencia = contratoFinanciamento.NumeroOcorrencia > 0 ? contratoFinanciamento.NumeroOcorrencia : 1;
            decimal valorParcelaOriginal = contratoFinanciamento.ValorTotal;
            decimal acrescimoParcelaOriginal = contratoFinanciamento.ValorAcrescimo;
            decimal valorDiferenca = 0;
            decimal acrescimoDiferenca = 0;

            numeroOcorrencia = numeroOcorrencia - sequencia;
            if (numeroOcorrencia <= 0)
                return;

            decimal valorParcelasAnteriores = repContratoFinanciamentoParcela.BuscarValorParcelasAnterior(contratoFinanciamento.Codigo, sequencia);
            if (valorParcelasAnteriores <= 0)
                return;

            valorParcelaOriginal = valorParcelaOriginal - valorParcelasAnteriores;
            if (valorParcelaOriginal <= 0)
                return;

            decimal acrescimoParcelasAnteriores = repContratoFinanciamentoParcela.BuscarAcrescimoParcelasAnterior(contratoFinanciamento.Codigo, sequencia);
            acrescimoParcelaOriginal = acrescimoParcelaOriginal - acrescimoParcelasAnteriores;

            decimal valorParcela = 0;
            decimal acrescimoParcela = 0;
            if (contratoFinanciamento.Dividir)
            {
                valorParcela = Math.Round(valorParcelaOriginal / numeroOcorrencia, 2);
                valorDiferenca = Math.Round(valorParcelaOriginal - (valorParcela * numeroOcorrencia), 2);

                if (acrescimoParcelaOriginal >= 0)
                {
                    acrescimoParcela = Math.Round(acrescimoParcelaOriginal / numeroOcorrencia, 2);
                    acrescimoDiferenca = Math.Round(acrescimoParcelaOriginal - (acrescimoParcela * numeroOcorrencia), 2);
                }
            }
            if (valorParcela <= 0)
                return;

            for (int i = 0; i < numeroOcorrencia; i++)
            {
                int sequenciaParcela = i + sequencia + 1;
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela contratoFinanciamentoParcela = repContratoFinanciamentoParcela.BuscarPorParcela(contratoFinanciamento.Codigo, sequenciaParcela);
                if (contratoFinanciamentoParcela != null)
                {
                    if (i == 0)
                    {
                        contratoFinanciamentoParcela.Valor = Math.Round(valorParcela + valorDiferenca, 2);
                        if (acrescimoParcela >= 0)
                            contratoFinanciamentoParcela.ValorAcrescimo = Math.Round(acrescimoParcela + acrescimoDiferenca, 2);
                    }
                    else
                    {
                        contratoFinanciamentoParcela.Valor = valorParcela;
                        if (acrescimoParcela >= 0)
                            contratoFinanciamentoParcela.ValorAcrescimo = acrescimoParcela;
                    }

                    repContratoFinanciamentoParcela.Atualizar(contratoFinanciamentoParcela);
                }

            }
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoVeiculo repContratoFinanciamentoVeiculo = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoVeiculo(unidadeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);

            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));
            if (contratoFinanciamento.Veiculos != null && contratoFinanciamento.Veiculos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var veiculo in veiculos)
                    if (veiculo.VEICULO.Codigo != null)
                        codigos.Add((int)veiculo.VEICULO.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo> contratoFinanciamentoVeiculoDeletar = (from obj in contratoFinanciamento.Veiculos where !codigos.Contains(obj.Veiculo.Codigo) select obj).ToList();

                for (var i = 0; i < contratoFinanciamentoVeiculoDeletar.Count; i++)
                    repContratoFinanciamentoVeiculo.Deletar(contratoFinanciamentoVeiculoDeletar[i]);
            }
            else
                contratoFinanciamento.Veiculos = new List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo>();

            foreach (var veiculo in veiculos)
            {
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo contratoFinanciamentoVeiculo = veiculo.VEICULO.Codigo != null ? repContratoFinanciamentoVeiculo.BuscarPorVeiculoEContrato((int)veiculo.VEICULO.Codigo, contratoFinanciamento.Codigo) : null;
                if (contratoFinanciamentoVeiculo == null)
                {
                    contratoFinanciamentoVeiculo = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo();

                    int.TryParse((string)veiculo.VEICULO.Codigo, out int codigoVeiculo);
                    contratoFinanciamentoVeiculo.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    contratoFinanciamentoVeiculo.ContratoFinanciamento = contratoFinanciamento;

                    repContratoFinanciamentoVeiculo.Inserir(contratoFinanciamentoVeiculo);
                }
            }
        }

        private void SalvarDocumentosEntrada(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada repContratoFinanciamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);

            dynamic documentosEntrada = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DocumentosEntrada"));
            if (contratoFinanciamento.DocumentosEntrada != null && contratoFinanciamento.DocumentosEntrada.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var documentoEntrada in documentosEntrada)
                    if (documentoEntrada.DOCUMENTOENTRADA.Codigo != null)
                        codigos.Add((int)documentoEntrada.DOCUMENTOENTRADA.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada> contratoFinanciamentoDocumentoEntradaDeletar = (from obj in contratoFinanciamento.DocumentosEntrada where !codigos.Contains(obj.DocumentoEntradaTMS.Codigo) select obj).ToList();

                for (var i = 0; i < contratoFinanciamentoDocumentoEntradaDeletar.Count; i++)
                    repContratoFinanciamentoDocumentoEntrada.Deletar(contratoFinanciamentoDocumentoEntradaDeletar[i]);
            }
            else
                contratoFinanciamento.DocumentosEntrada = new List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada>();

            foreach (var documentoEntrada in documentosEntrada)
            {
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada contratoFinanciamentoDocumentoEntrada = documentoEntrada.DOCUMENTOENTRADA.Codigo != null ? repContratoFinanciamentoDocumentoEntrada.BuscarPorDocumentoEntradaEContrato((int)documentoEntrada.DOCUMENTOENTRADA.Codigo, contratoFinanciamento.Codigo) : null;
                if (contratoFinanciamentoDocumentoEntrada == null)
                {
                    contratoFinanciamentoDocumentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada();

                    int.TryParse((string)documentoEntrada.DOCUMENTOENTRADA.Codigo, out int codigoDocumentoEntrada);
                    contratoFinanciamentoDocumentoEntrada.DocumentoEntradaTMS = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);
                    contratoFinanciamentoDocumentoEntrada.ContratoFinanciamento = contratoFinanciamento;

                    repContratoFinanciamentoDocumentoEntrada.Inserir(contratoFinanciamentoDocumentoEntrada);
                }
            }
        }

        private void SalvarModalidades(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento repContratoFinanciamentoModalidades = new Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento(unidadeTrabalho);

            dynamic modalidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Modalidades"));

            if (contratoFinanciamento.Modalidades != null && contratoFinanciamento.Modalidades.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic modalidade in modalidades)
                    if (modalidade.MODALIDADE.Codigo != null)
                        codigos.Add((int)modalidade.MODALIDADE.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento> contratoFinanciamentoModalidadeDeletar = contratoFinanciamento.Modalidades.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < contratoFinanciamentoModalidadeDeletar.Count; i++)
                    contratoFinanciamento.Modalidades.Remove(contratoFinanciamentoModalidadeDeletar[i]);
            }

            if (contratoFinanciamento.Modalidades == null || contratoFinanciamento.Modalidades.Count == 0 || contratoFinanciamento.Modalidades == null)
                contratoFinanciamento.Modalidades = new List<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento>();

            foreach (var modalidade in modalidades)
            {
                if (contratoFinanciamento.Modalidades.Any(o => o.Codigo == (int)modalidade.MODALIDADE.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento contratoFinanciamentoModalidade = repContratoFinanciamentoModalidades.BuscarPorCodigo((int)modalidade.MODALIDADE.Codigo, false);
                contratoFinanciamento.Modalidades.Add(contratoFinanciamentoModalidade);

            }
        }


        private void ExcluirAnexos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoAnexo repContratoFinanciamentoAnexo = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoAnexo contratoFinanciamentoAnexo = repContratoFinanciamentoAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(contratoFinanciamentoAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(contratoFinanciamentoAnexo.CaminhoArquivo);

                    repContratoFinanciamentoAnexo.Deletar(contratoFinanciamentoAnexo, Auditado);
                }
            }
        }

        private dynamic retornarContratoFinanciamentoParcelaGrid(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela contratosFinanciamentoParcela)
        {
            var retorno = new
            {
                contratosFinanciamentoParcela.Codigo,
                contratosFinanciamentoParcela.Sequencia,
                contratosFinanciamentoParcela.NumeroDocumento,
                contratosFinanciamentoParcela.CodigoBarras,
                DataVencimento = contratosFinanciamentoParcela.DataVencimento.ToString("dd/MM/yyyy"),
                Valor = contratosFinanciamentoParcela.Valor.ToString("n2"),
                contratosFinanciamentoParcela.Observacao,
                ValorAcrescimo = contratosFinanciamentoParcela.ValorAcrescimo.ToString("n2"),
                SituacaoTitulo = contratosFinanciamentoParcela?.Titulo?.DescricaoSituacao ?? string.Empty,
                NumeroTitulo = contratosFinanciamentoParcela?.Titulo?.Codigo ?? 0
            };
            return retorno;
        }

        #endregion
    }
}
