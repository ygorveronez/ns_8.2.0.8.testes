using Servicos.Embarcador.Documentos;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/Bem")]
    public class BemController : BaseController
    {
        #region Construtores

        public BemController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("FuncionarioAlocado"), out int codigoFuncionarioAlocado);

                string descricao = Request.Params("Descricao");
                string numeroSerie = Request.Params("NumeroSerie");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número de Série", "NumeroSerie", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Fim Garantia", "DataFimGarantia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Patrimônio", "ValorBem", 10, Models.Grid.Align.right, true);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Patrimonio.Bem> bens = repBem.Consultar(codigo, codigoEmpresa, descricao, numeroSerie, codigoFuncionarioAlocado, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBem.ContarConsulta(codigo, codigoEmpresa, descricao, numeroSerie, codigoFuncionarioAlocado));

                var lista = (from p in bens
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.NumeroSerie,
                                 DataFimGarantia = p.DataFimGarantia.ToString("dd/MM/yyyy"),
                                 ValorBem = p.ValorBem.ToString("n4")
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.Bem bem = new Dominio.Entidades.Embarcador.Patrimonio.Bem();

                PreencherBem(bem, unitOfWork);
                SalvarSaldo(bem, unitOfWork);
                repBem.Inserir(bem, Auditado);

                SalvarComponentes(bem, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.Bem bem = repBem.BuscarPorCodigo(codigo, true);

                PreencherBem(bem, unitOfWork);
                SalvarSaldo(bem, unitOfWork);
                repBem.Atualizar(bem, Auditado);

                SalvarComponentes(bem, unitOfWork);
                ExcluirAnexos(unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.Bem bem = repBem.BuscarPorCodigo(codigo);

                var dynBem = new
                {
                    bem.Codigo,
                    bem.Descricao,
                    bem.DescricaoNota,
                    bem.NumeroSerie,
                    bem.Observacao,
                    DataAquisicao = bem.DataAquisicao.ToString("dd/MM/yyyy"),
                    DataFimGarantia = bem.DataFimGarantia.ToString("dd/MM/yyyy"),
                    GrupoProdutoTMS = bem.GrupoProdutoTMS != null ? new { bem.GrupoProdutoTMS.Codigo, bem.GrupoProdutoTMS.Descricao } : null,
                    Produto = bem.Produto != null ? new { bem.Produto.Codigo, bem.Produto.Descricao } : null,
                    CentroResultado = bem.CentroResultado != null ? new { bem.CentroResultado.Codigo, bem.CentroResultado.Descricao } : null,
                    Almoxarifado = bem.Almoxarifado != null ? new { bem.Almoxarifado.Codigo, bem.Almoxarifado.Descricao } : null,
                    Fornecedor = bem.Fornecedor != null ? new { bem.Fornecedor.Codigo, bem.Fornecedor.Descricao } : null,
                    DocumentoEntradaItem = bem.DocumentoEntradaItem != null ? new { bem.DocumentoEntradaItem.Codigo, Descricao = bem.DocumentoEntradaItem.Produto.DescricaoNotaFiscal } : null,
                    Empresa = bem.Empresa != null ? new { bem.Empresa.Codigo, bem.Empresa.Descricao } : null,
                    Saldo = new
                    {
                        bem.VidaUtil,
                        ValorBem = bem.ValorBem.ToString("n4"),
                        PercentualResidual = bem.PercentualResidual.ToString("n2"),
                        ValorResidual = bem.ValorResidual.ToString("n2"),
                        ValorDepreciar = bem.ValorDepreciar.ToString("n2"),
                        DepreciacaoAcumulada = bem.DepreciacaoAcumulada.ToString("n2"),
                        PercentualDepreciacao = bem.PercentualDepreciacao.ToString("n2"),
                        VidaUtilTaxaDepreciacao = bem.VidaUtilTaxaDepreciacao.ToString("n2"),
                        DataImplantacao = bem.DataImplantacao.ToString("dd/MM/yyyy"),
                        DataEntradaTransferencia = bem.DataEntradaTransferencia.HasValue ? bem.DataEntradaTransferencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataBaixa = bem.DataBaixa.HasValue ? bem.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataAlocado = bem.DataAlocado.HasValue ? bem.DataAlocado.Value.ToString("dd/MM/yyyy") : string.Empty,
                        FuncionarioAlocado = bem.FuncionarioAlocado != null ? new { bem.FuncionarioAlocado.Codigo, Descricao = bem.FuncionarioAlocado.Nome } : null
                    },
                    Componentes = (from obj in bem.Componentes
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao,
                                       obj.NumeroSerie,
                                       DataFimGarantia = obj.DataFimGarantia.ToString("dd/MM/yyyy")
                                   }).ToList(),
                    ListaAnexos = bem.Anexos != null ? (from obj in bem.Anexos
                                                        select new
                                                        {
                                                            obj.Codigo,
                                                            DescricaoAnexo = obj.Descricao,
                                                            Arquivo = obj.NomeArquivo
                                                        }).ToList() : null
                };

                return new JsonpResult(dynBem);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.Bem bem = repBem.BuscarPorCodigo(codigo, true);

                if (bem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repBem.Deletar(bem, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinanciamentoBem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);

                int.TryParse(Request.Params("DocumentoEntradaItem"), out int codigoItem);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Doc.", "NumeroDocumentoTituloOriginal", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sequência", "Sequencia", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Saldo", "ValorSaldo", 10, Models.Grid.Align.right, false);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem documentoEntradaItem = repDocumentoEntradaItem.BuscarPorCodigo(codigoItem);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repBem.ConsultarFinanceiroBem(documentoEntradaItem?.DocumentoEntrada?.Codigo ?? 0, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repBem.ContarFinanceiroBem(documentoEntradaItem?.DocumentoEntrada?.Codigo ?? 0);

                var lista = (from o in listaTitulos
                             select new
                             {
                                 o.Codigo,
                                 o.NumeroDocumentoTituloOriginal,
                                 o.Sequencia,
                                 o.DescricaoSituacao,
                                 Pessoa = o.Pessoa?.Descricao ?? string.Empty,
                                 ValorOriginal = o.ValorOriginal.ToString("n2"),
                                 DataEmissao = o.DataEmissao.HasValue ? o.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVencimento = o.DataVencimento.HasValue ? o.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 ValorSaldo = o.Saldo.ToString("n2"),
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
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

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                unitOfWork.Start();

                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Repositorio.Embarcador.Patrimonio.BemAnexo repBemAnexo = new Repositorio.Embarcador.Patrimonio.BemAnexo(unitOfWork);

                int codigoBem = 0;
                int.TryParse(Request.Params("CodigoBem"), out codigoBem);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "Bem");
                
                for (var i = 0; i < files.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Patrimonio.BemAnexo bemAnexo = new Dominio.Entidades.Embarcador.Patrimonio.BemAnexo();

                    string descricao = Request.Params("DescricaoAnexo");
                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    bemAnexo.CaminhoArquivo = caminho;
                    bemAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));
                    bemAnexo.Descricao = descricao;
                    bemAnexo.Bem = repBem.BuscarPorCodigo(codigoBem);

                    repBemAnexo.Inserir(bemAnexo, Auditado);
                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = 0;
                int.TryParse(Request.Params("CodigoAnexo"), out codigoAnexo);

                Repositorio.Embarcador.Patrimonio.BemAnexo repBemAnexo = new Repositorio.Embarcador.Patrimonio.BemAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemAnexo bemAnexo = repBemAnexo.BuscarPorCodigo(codigoAnexo);

                if (bemAnexo == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(bemAnexo.CaminhoArquivo))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(bemAnexo.CaminhoArquivo), "image/jpeg", bemAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo do Chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherBem(Dominio.Entidades.Embarcador.Patrimonio.Bem bem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProdutoTMS = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
            int.TryParse(Request.Params("GrupoProdutoTMS"), out int codigoGrupoProduto);
            int.TryParse(Request.Params("CentroResultado"), out int codigoCentroResultado);
            int.TryParse(Request.Params("Almoxarifado"), out int codigoAlmoxarifado);
            int.TryParse(Request.Params("Produto"), out int codigoProduto);

            double.TryParse(Request.Params("Fornecedor"), out double fornecedor);

            string descricao = Request.Params("Descricao");
            string numeroSerie = Request.Params("NumeroSerie");
            string descricaoNota = Request.Params("DescricaoNota");
            string observacao = Request.Params("Observacao");

            DateTime.TryParse(Request.Params("DataAquisicao"), out DateTime dataAquisicao);
            DateTime.TryParse(Request.Params("DataFimGarantia"), out DateTime dataFimGarantia);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            bem.Descricao = descricao;
            bem.DescricaoNota = descricaoNota;
            bem.NumeroSerie = numeroSerie;
            bem.Observacao = observacao;
            bem.DataAquisicao = dataAquisicao;
            bem.DataFimGarantia = dataFimGarantia;

            if (codigoAlmoxarifado > 0)
                bem.Almoxarifado = repAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado);
            else
                bem.Almoxarifado = null;
            if (codigoCentroResultado > 0)
                bem.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
            else
                bem.CentroResultado = null;
            bem.GrupoProdutoTMS = repGrupoProdutoTMS.BuscarPorCodigo(codigoGrupoProduto);
            bem.Produto = repProduto.BuscarPorCodigo(codigoProduto);
            bem.Fornecedor = repCliente.BuscarPorCPFCNPJ(fornecedor);
            bem.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
        }

        private void SalvarSaldo(Dominio.Entidades.Embarcador.Patrimonio.Bem bem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            dynamic saldo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Saldo"));

            int.TryParse((string)saldo.VidaUtil, out int vidaUtil);
            int.TryParse((string)saldo.FuncionarioAlocado, out int codigoFuncionarioAlocado);

            DateTime.TryParse((string)saldo.DataImplantacao, out DateTime dataImplantacao);
            DateTime.TryParse((string)saldo.DataEntradaTransferencia, out DateTime dataEntradaTransferencia);
            DateTime.TryParse((string)saldo.DataBaixa, out DateTime dataBaixa);
            DateTime.TryParse((string)saldo.DataAlocado, out DateTime dataAlocado);

            bem.VidaUtil = vidaUtil;
            bem.DataImplantacao = dataImplantacao;
            if (dataEntradaTransferencia > DateTime.MinValue)
                bem.DataEntradaTransferencia = dataEntradaTransferencia;
            else
                bem.DataEntradaTransferencia = null;
            if (dataBaixa > DateTime.MinValue)
                bem.DataBaixa = dataBaixa;
            else
                bem.DataBaixa = null;
            if (dataAlocado > DateTime.MinValue)
                bem.DataAlocado = dataAlocado;
            else
                bem.DataAlocado = null;

            bem.ValorBem = Utilidades.Decimal.Converter((string)saldo.ValorBem);
            bem.PercentualResidual = Utilidades.Decimal.Converter((string)saldo.PercentualResidual);
            bem.ValorResidual = Utilidades.Decimal.Converter((string)saldo.ValorResidual);
            bem.ValorDepreciar = Utilidades.Decimal.Converter((string)saldo.ValorDepreciar);
            bem.PercentualDepreciacao = Utilidades.Decimal.Converter((string)saldo.PercentualDepreciacao);
            bem.VidaUtilTaxaDepreciacao = Utilidades.Decimal.Converter((string)saldo.VidaUtilTaxaDepreciacao);

            bem.FuncionarioAlocado = codigoFuncionarioAlocado > 0 ? repFuncionario.BuscarPorCodigo(codigoFuncionarioAlocado) : null;
        }

        private void SalvarComponentes(Dominio.Entidades.Embarcador.Patrimonio.Bem bem, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Patrimonio.BemComponente repBemComponente = new Repositorio.Embarcador.Patrimonio.BemComponente(unidadeTrabalho);

            dynamic componentes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Componentes"));
            if (bem.Componentes != null && bem.Componentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var componente in componentes)
                    if (componente.Codigo != null)
                        codigos.Add((int)componente.Codigo);

                List<Dominio.Entidades.Embarcador.Patrimonio.BemComponente> bemComponenteDeletar = (from obj in bem.Componentes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < bemComponenteDeletar.Count; i++)
                    repBemComponente.Deletar(bemComponenteDeletar[i]);
            }
            else
                bem.Componentes = new List<Dominio.Entidades.Embarcador.Patrimonio.BemComponente>();

            foreach (var componente in componentes)
            {
                Dominio.Entidades.Embarcador.Patrimonio.BemComponente bemComponente = componente.Codigo != null ? repBemComponente.BuscarPorCodigo((int)componente.Codigo, true) : null;
                if (bemComponente == null)
                    bemComponente = new Dominio.Entidades.Embarcador.Patrimonio.BemComponente();

                DateTime.TryParse((string)componente.DataFimGarantia, out DateTime dataFimGarantia);

                bemComponente.Descricao = (string)componente.Descricao;
                bemComponente.NumeroSerie = (string)componente.NumeroSerie;
                bemComponente.DataFimGarantia = dataFimGarantia;
                bemComponente.Bem = bem;

                if (bemComponente.Codigo > 0)
                    repBemComponente.Atualizar(bemComponente, Auditado);
                else
                    repBemComponente.Inserir(bemComponente, Auditado);
            }
        }

        private void ExcluirAnexos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Patrimonio.BemAnexo repBemAnexo = new Repositorio.Embarcador.Patrimonio.BemAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.Patrimonio.BemAnexo bemAnexo = repBemAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(bemAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(bemAnexo.CaminhoArquivo);

                    repBemAnexo.Deletar(bemAnexo, Auditado);
                }
            }
        }

        #endregion
    }
}
