using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Faturas/Fatura", "SAC/AtendimentoCliente")]
    public class FaturaDocumentoController : BaseController
    {
        #region Construtores

        public FaturaDocumentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> PesquisaDocumentosParaFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfigFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configFinanceiro = repConfigFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(Request.GetIntParam("Fatura"));

                if (fatura == null)
                    return new JsonpResult(false, false, "Fatura não encontrada.");

                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosPesquisa = ObterFiltrosDocumentosParaFatura(fatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Documento", "Documento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.left, true);

                if (configFinanceiro.AtivarColunaNumeroContainerConsultaDocumentosFatura)
                    grid.AdicionarCabecalho("Container", "Container", 20, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Tipo Doc. CT-e", "TipoDocumentoCTe", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 20, Models.Grid.Align.left, true);

                if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                    grid.AdicionarCabecalho("Moeda", "Moeda", 12, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Faturar", "ValorAFaturar", 12, Models.Grid.Align.left, true);

                if (configFinanceiro.AtivarColunaCSTConsultaDocumentosFatura)
                    grid.AdicionarCabecalho("CST", "CST", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int countDocumentosFaturamento = repDocumentoFaturamento.ContarConsultaDocumentosFaturamentoParaFatura(filtrosPesquisa, configFinanceiro);
                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamentoParaFatura> documentosFaturamento = countDocumentosFaturamento > 0 ? repDocumentoFaturamento.ConsultarDocumentosFaturamentoParaFatura(filtrosPesquisa, parametrosConsulta, configFinanceiro) : new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamentoParaFatura>();

                grid.setarQuantidadeTotal(countDocumentosFaturamento);

                var retorno = documentosFaturamento.Select(o => new
                {
                    o.Codigo,
                    Documento = o.TipoDocumento == TipoDocumentoFaturamento.CTe ? o.Numero + "-" + o.Serie : o.Numero,
                    o.NumeroCarga,
                    DataEmissao = o.DataEmissao.ToString("dd/MM/yyyy"),
                    TipoDocumentoCTe = o.TipoCTE.ObterDescricao(),
                    Tipo = o.DescricaoTipoDocumento,
                    o.Origem,
                    o.Destino,
                    o.Motorista,
                    o.NotaFiscal,
                    Moeda = o.Moeda.ObterDescricao(),
                    Valor = o.ValorDocumento.ToString("n2"),
                    ValorAFaturar = o.ValorAFaturar.ToString("n2"),
                    o.CST,
                    o.Container

                }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar os documentos para a fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);

                int numero = Request.GetIntParam("Numero");
                int serie = Request.GetIntParam("Serie");
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoEmpresa = Request.GetIntParam("Empresa");

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoDocumento", false);
                grid.AdicionarCabecalho("CodigoCTe", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Modelo", false);
                grid.AdicionarCabecalho("Moeda", false);
                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CT-e Originário", "CteOriginario", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Carga", "Carga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Documento", "CodigoDocumento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número NFe", "NumeroNFe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data CT-e", "DataCTe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Doc. CT-e", "TipoDocumentoCTe", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                if (configuracaoGeral.HabilitarFuncionalidadesProjetoGollum)
                    grid.AdicionarCabecalho("Tipo OS Convertido", "TipoOSConvertido", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Documento", "ValorDocumento", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Cobrar", "ValorACobrar", 8, Models.Grid.Align.left, true, false, false, false, true, new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 12));
                grid.AdicionarCabecalho("Desconto", "ValorDesconto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Total a Cobrar", "TotalACobrar", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status Canhoto", "StatusCanhoto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Escrituração", "Escrituracao", 8, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Tipo")
                    propOrdenar = "Documento.TipoDocumento";
                else if (propOrdenar == "Origem")
                    propOrdenar = "Documento.Origem.Descricao";
                else if (propOrdenar == "Destino")
                    propOrdenar = "Documento.Destino.Descricao";
                else if (propOrdenar == "ValorDocumento")
                    propOrdenar = "Documento.ValorDocumento";
                else if (propOrdenar == "TotalACobrar")
                    propOrdenar = "ValorTotalACobrar";
                else if (propOrdenar == "Carga")
                    propOrdenar = "Documento.CargaPagamento.CodigoCargaEmbarcador";

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repositorioFatura.BuscarPorCodigo(codigoFatura);
                long codigoTomador = fatura?.Tomador?.Codigo ?? 0;

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> faturaDocumentos = repFaturaDocumento.Consultar(codigoFatura, numero, serie, codigoEmpresa, codigoCarga, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, codigoTomador);
                int countDocumentosFatura = repFaturaDocumento.ContarConsulta(codigoFatura, numero, serie, codigoEmpresa, codigoCarga, codigoTomador);

                grid.setarQuantidadeTotal(countDocumentosFatura);

                var retorno = (from o in faturaDocumentos select ObterDetalhesGridFaturaDocumento(o)).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar os documentos da fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDocumentosFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);

                List<int> codigosDocumentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Documentos"));

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "A situação da fatura não permite a inclusão de documentos.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                unidadeTrabalho.Start();

                foreach (int codigoDocumento in codigosDocumentos)
                {
                    if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out string erro, ref fatura, codigoDocumento, 0m, unidadeTrabalho, Auditado))
                    {
                        unidadeTrabalho.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar os documentos na fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDocumentosFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);

                List<int> codigosDocumentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Documentos"));

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, false, "Fatura não encontrada.");

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "A situação da fatura não permite a inclusão de documentos.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                unidadeTrabalho.Start();

                foreach (int codigoDocumento in codigosDocumentos)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigoDocumento);

                    if (faturaDocumento == null)
                        continue;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Removeu o documento " + faturaDocumento.Documento.Numero.ToString() + " da fatura.", unidadeTrabalho);

                    List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentoAcrescimosDescontos = repFaturaDocumentoAcrescimoDesconto.BuscarPorFaturaDocumento(faturaDocumento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto acrescimoDesconto in faturaDocumentoAcrescimosDescontos)
                        repFaturaDocumentoAcrescimoDesconto.Deletar(acrescimoDesconto);

                    faturaDocumento.Documento.ValorEmFatura -= faturaDocumento.ValorACobrar;
                    faturaDocumento.Documento.ValorAFaturar += faturaDocumento.ValorACobrar;
                    faturaDocumento.Documento.Fatura = faturaDocumento.Fatura;

                    repDocumentoFaturamento.Atualizar(faturaDocumento.Documento);
                    repFaturaDocumento.Deletar(faturaDocumento);
                }

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar os documentos na fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DarBaixaDocumento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Documento"), out int codigoDocumento);

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigoDocumento);

                unidadeTrabalho.Start();

                if (faturaDocumento.Documento != null && faturaDocumento.Documento.CTe != null && faturaDocumento.Documento.CTe.Titulo != null)
                {
                    faturaDocumento.Documento.CTe.Titulo.StatusTitulo = StatusTitulo.Quitada;
                    faturaDocumento.Documento.CTe.Titulo.DataLiquidacao = DateTime.Now;
                    faturaDocumento.Documento.CTe.Titulo.ValorPago = faturaDocumento.Documento.CTe.Titulo.ValorTituloOriginal;
                    repTitulo.Atualizar(faturaDocumento.Documento.CTe.Titulo);

                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = faturaDocumento.Documento.CTe.Titulo;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Efetuou Baixa no titulo do documento através da fatura.", unidadeTrabalho);

                }
                else if (faturaDocumento.Documento.CTe.Titulo == null)
                {
                    //caso nao existir o titulo, vamos criar ja quitado
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico Cte = faturaDocumento.Documento.CTe;

                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                    titulo.Acrescimo = 0;
                    titulo.DataEmissao = Cte.DataEmissao;
                    titulo.Desconto = 0;
                    titulo.Historico = "GERADO PELA BAIXA DA FATURA DO CTE DE NÚMERO " + Cte.Numero + " E SÉRIE " + Cte.Serie.Numero;
                    titulo.Pessoa = faturaDocumento.Documento.Tomador;
                    titulo.GrupoPessoas = faturaDocumento.Documento.Tomador.GrupoPessoas;
                    titulo.Sequencia = faturaDocumento.Fatura.Numero;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.DataLiquidacao = DateTime.Now;
                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                    titulo.ValorOriginal = faturaDocumento.ValorTotalACobrar;
                    titulo.ValorPago = faturaDocumento.ValorTotalACobrar;
                    titulo.ValorPendente = faturaDocumento.ValorTotalACobrar;
                    titulo.ValorTituloOriginal = faturaDocumento.ValorTotalACobrar;
                    titulo.TipoDocumentoTituloOriginal = "CT-e";
                    titulo.NumeroDocumentoTituloOriginal = Cte.Numero.ToString();
                    titulo.ConhecimentoDeTransporteEletronico = Cte;
                    titulo.Observacao = "CT-e Nº: " + Cte.Numero;
                    titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                    titulo.TipoAmbiente = Cte.TipoAmbiente;

                    titulo.Usuario = this.Usuario;
                    titulo.DataLancamento = DateTime.Now;

                    titulo.Empresa = Cte.Empresa;

                    repTitulo.Inserir(titulo);

                    Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                    Cte.Titulo = titulo;
                    repCte.Atualizar(Cte);

                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeTrabalho).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Criacao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Criou e Efetuou Baixa no titulo do documento através da fatura.", unidadeTrabalho);

                }

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao efetuar baixa no titulo do documento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DarBaixaDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic dynDocumentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensSelecionados"));

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
                unidadeTrabalho.Start();

                foreach (var documento in dynDocumentos)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo((int)documento.Codigo);


                    if (faturaDocumento.Documento != null && faturaDocumento.Documento.CTe != null && faturaDocumento.Documento.CTe.Titulo != null)
                    {
                        faturaDocumento.Documento.CTe.Titulo.StatusTitulo = StatusTitulo.Quitada;
                        faturaDocumento.Documento.CTe.Titulo.DataLiquidacao = DateTime.Now;
                        faturaDocumento.Documento.CTe.Titulo.ValorPago = faturaDocumento.Documento.CTe.Titulo.ValorTituloOriginal;
                        repTitulo.Atualizar(faturaDocumento.Documento.CTe.Titulo);

                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = faturaDocumento.Documento.CTe.Titulo;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Efetuou Baixa no titulo do documento através da fatura.", unidadeTrabalho);

                    }
                    else if (faturaDocumento.Documento.CTe.Titulo == null)
                    {
                        //caso nao existir o titulo, vamos criar ja quitado
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico Cte = faturaDocumento.Documento.CTe;

                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                        titulo.Acrescimo = 0;
                        titulo.DataEmissao = Cte.DataEmissao;
                        titulo.Desconto = 0;
                        titulo.Historico = "GERADO PELA BAIXA DA FATURA DO CTE DE NÚMERO " + Cte.Numero + " E SÉRIE " + Cte.Serie.Numero;
                        titulo.Pessoa = faturaDocumento.Documento.Tomador;
                        titulo.GrupoPessoas = faturaDocumento.Documento.Tomador.GrupoPessoas;
                        titulo.Sequencia = faturaDocumento.Fatura.Numero;
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.DataLiquidacao = DateTime.Now;
                        titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                        titulo.ValorOriginal = faturaDocumento.ValorTotalACobrar;
                        titulo.ValorPago = faturaDocumento.ValorTotalACobrar;
                        titulo.ValorPendente = faturaDocumento.ValorTotalACobrar;
                        titulo.ValorTituloOriginal = faturaDocumento.ValorTotalACobrar;
                        titulo.TipoDocumentoTituloOriginal = "CT-e";
                        titulo.NumeroDocumentoTituloOriginal = Cte.Numero.ToString();
                        titulo.ConhecimentoDeTransporteEletronico = Cte;
                        titulo.Observacao = "CT-e Nº: " + Cte.Numero;
                        titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                        titulo.TipoAmbiente = Cte.TipoAmbiente;

                        titulo.Usuario = this.Usuario;
                        titulo.DataLancamento = DateTime.Now;

                        titulo.Empresa = Cte.Empresa;

                        repTitulo.Inserir(titulo);

                        Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                        Cte.Titulo = titulo;
                        repCte.Atualizar(Cte);

                        new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeTrabalho).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Criacao);


                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Criou e Efetuou Baixa no titulo do documento através da fatura.", unidadeTrabalho);

                    }
                }


                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao efetuar baixa no titulo do documento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDocumentoFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);
                int.TryParse(Request.Params("Documento"), out int codigoDocumento);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, false, "Fatura não encontrada.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                bool gerarFaturaPorCte = (fatura.ClienteTomadorFatura?.GrupoPessoas?.GerarFaturaPorCte ?? false) && configuracaoEmbarcador.TipoImpressaoFatura != TipoImpressaoFatura.Multimodal;

                if (!IsSituacaoFaturaPermiteRemoverDocumentos(fatura, unidadeTrabalho))
                    return new JsonpResult(false, true, "A situação da fatura não permite a remoção de documentos.");

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigoDocumento);

                if (faturaDocumento != null)
                {
                    List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentoAcrescimosDescontos = repFaturaDocumentoAcrescimoDesconto.BuscarPorFaturaDocumento(faturaDocumento.Codigo);

                    unidadeTrabalho.Start();

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Removeu o documento " + faturaDocumento.Documento.Numero.ToString() + " da fatura.", unidadeTrabalho);

                    foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto acrescimoDesconto in faturaDocumentoAcrescimosDescontos)
                        repFaturaDocumentoAcrescimoDesconto.Deletar(acrescimoDesconto);

                    faturaDocumento.Documento.ValorEmFatura -= faturaDocumento.ValorACobrar;
                    faturaDocumento.Documento.ValorAFaturar += faturaDocumento.ValorACobrar;
                    faturaDocumento.Documento.Fatura = faturaDocumento.Fatura;

                    repDocumentoFaturamento.Atualizar(faturaDocumento.Documento);
                    repFaturaDocumento.Deletar(faturaDocumento);

                    Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                    unidadeTrabalho.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao remover os documentos na fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosDocumento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoDocumento);
                decimal.TryParse(Request.Params("ValorACobrar"), out decimal valorACobrar);

                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigoDocumento);

                if (faturaDocumento.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "Não é possível alterar os dados do documento na situação atual da fatura.");

                if (faturaDocumento.Documento.Moeda.HasValue && faturaDocumento.Documento.Moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    return new JsonpResult(false, true, "Não é possível alterar o valor a cobrar de um documento com valor emitido em " + faturaDocumento.Documento.Moeda.Value.ObterDescricao() + ".");

                unidadeTrabalho.Start();

                faturaDocumento.Documento.ValorEmFatura -= faturaDocumento.ValorACobrar;
                faturaDocumento.Documento.ValorAFaturar += faturaDocumento.ValorACobrar;
                faturaDocumento.Documento.Fatura = faturaDocumento.Fatura;

                decimal valorAFaturar = faturaDocumento.Documento.ValorDocumento - faturaDocumento.Documento.ValorEmFatura;

                if (valorACobrar > valorAFaturar)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "O valor a cobrar não pode ser maior que o valor total a faturar (" + valorAFaturar.ToString("n2") + ").");
                }

                if (valorACobrar <= 0)
                    valorACobrar = valorAFaturar;

                faturaDocumento.Documento.ValorEmFatura += valorACobrar;
                faturaDocumento.Documento.ValorAFaturar -= valorACobrar;
                faturaDocumento.ValorACobrar = valorACobrar;
                faturaDocumento.ValorTotalACobrar = valorACobrar + faturaDocumento.ValorAcrescimo - faturaDocumento.ValorDesconto;

                repFaturaDocumento.Atualizar(faturaDocumento);
                repDocumentoFaturamento.Atualizar(faturaDocumento.Documento);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaDocumento.Fatura;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Alterou Dados dos Documentos.", unidadeTrabalho);

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(ObterDetalhesGridFaturaDocumento(faturaDocumento));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar os documentos na fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarDocumentos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFatura = Request.GetIntParam("Fatura");

                Servicos.Embarcador.Fatura.Fatura svcFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repsitorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = await repsitorioFatura.BuscarPorCodigoAsync(codigoFatura, true);

                await unitOfWork.StartAsync(cancellationToken);

                if (!svcFatura.ConfirmarDocumentos(out string msgErro, fatura, ConfiguracaoEmbarcador, this.Usuario, Auditado, null))
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return new JsonpResult(false, true, msgErro);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(svcFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar os documentos da fatura.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [Obsolete("Esta função está obsoleta, foi transferida para um serviço")]
        public async Task<IActionResult> obsoletaConfirmarDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);

                Servicos.Embarcador.Fatura.Fatura svcFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeTrabalho);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacaoAlcadaFatura = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura, true);

                if (fatura.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas)
                    return new JsonpResult(false, true, "A fatura ainda está em processo de lançamento de documentos, não sendo possível confirmar os documentos.");

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "Os documentos não podem ser confirmados na situação atual da fatura.");

                if (repFaturaDocumento.ContarPorFatura(fatura.Codigo) <= 0)
                    return new JsonpResult(false, true, "Não existem documentos vinculados à fatura, não sendo possível avançar.");

                unidadeTrabalho.Start();

                fatura.Etapa = EtapaFatura.Fechamento;
                fatura.ImprimeObservacaoFatura = false;

                if (fatura.Numero == 0)
                {
                    if (ConfiguracaoEmbarcador.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                        anoAtual = (anoAtual % 100);
                        if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                            fatura.Numero = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.Numero = repFatura.UltimoNumeracao() + 1;
                    fatura.ControleNumeracao = null;
                }

                if (fatura.Empresa == null)
                    fatura.Empresa = repFaturaDocumento.ObterPrimeiraEmpresaEmissora(fatura.Codigo);

                if (ConfiguracaoEmbarcador.UtilizarDadosBancariosDaEmpresa && fatura.Empresa != null && fatura.Empresa.Banco != null)
                {
                    fatura.Banco = fatura.Empresa.Banco;
                    fatura.Agencia = fatura.Empresa.Agencia;
                    fatura.DigitoAgencia = fatura.Empresa.DigitoAgencia;
                    fatura.NumeroConta = fatura.Empresa.NumeroConta;
                    fatura.TipoContaBanco = fatura.Empresa.TipoContaBanco;
                }

                if (fatura.Cliente != null && !fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                {
                    if (fatura.Cliente.GrupoPessoas != null)
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Cliente.GrupoPessoas.Banco;
                            fatura.Agencia = fatura.Cliente.GrupoPessoas.Agencia;
                            fatura.DigitoAgencia = fatura.Cliente.GrupoPessoas.DigitoAgencia;
                            fatura.NumeroConta = fatura.Cliente.GrupoPessoas.NumeroConta;
                            fatura.TipoContaBanco = fatura.Cliente.GrupoPessoas.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Cliente.GrupoPessoas.ClienteTomadorFatura;
                        fatura.ObservacaoFatura = fatura.Cliente.GrupoPessoas.ObservacaoFatura;
                    }
                    else
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Cliente.Banco;
                            fatura.Agencia = fatura.Cliente.Agencia;
                            fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                            fatura.NumeroConta = fatura.Cliente.NumeroConta;
                            fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                        fatura.ObservacaoFatura = fatura.Cliente.ObservacaoFatura;
                    }
                }
                else if (fatura.Cliente != null)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.Cliente.Banco;
                        fatura.Agencia = fatura.Cliente.Agencia;
                        fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                        fatura.NumeroConta = fatura.Cliente.NumeroConta;
                        fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.Cliente.ObservacaoFatura;
                }
                else if (fatura.GrupoPessoas != null)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.GrupoPessoas.Banco;
                        fatura.Agencia = fatura.GrupoPessoas.Agencia;
                        fatura.DigitoAgencia = fatura.GrupoPessoas.DigitoAgencia;
                        fatura.NumeroConta = fatura.GrupoPessoas.NumeroConta;
                        fatura.TipoContaBanco = fatura.GrupoPessoas.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.GrupoPessoas.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.GrupoPessoas.ObservacaoFatura;
                }

                if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                    fatura.ImprimeObservacaoFatura = true;

                if (fatura.ClienteTomadorFatura == null)
                    fatura.ClienteTomadorFatura = repFaturaDocumento.ObterPrimeiroTomador(fatura.Codigo);

                if (configuracaoFinanceiro.PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados == true)
                {
                    if ((fatura.Documentos.Select(o => o.Documento?.CTe?.CodigoEscrituracao)).Any(b => string.IsNullOrEmpty(b)))
                        return new JsonpResult(false, true, "Não foi possível confirmar os documentos. Existem documentos sem escrituração.");
                }

                repFatura.Atualizar(fatura, Auditado);

                svcFatura.InserirLog(fatura, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouCargas, this.Usuario);

                svcFatura.LancarParcelaFatura(fatura, unidadeTrabalho, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal);

                //if (possuiRegrasAprovacao)
                //svcFatura.EtapaAprovacao(fatura, TipoServicoMultisoftware);

                repFatura.Atualizar(fatura);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(svcFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar os documentos da fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }




        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao obter as configurações para importação.");
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic parametros = JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));

                int.TryParse((string)parametros.Fatura, out int codigoFatura);

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, "Fatura não encontrada.");

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, "Não é possível importar documentos para a fatura na situação atual.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();
                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;

                for (int i = 0; i < totalLinhas; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroCTe = linha.Colunas?.Where(o => o.NomeCampo == "NumeroCTe").FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerieCTe = linha.Colunas?.Where(o => o.NomeCampo == "SerieCTe").FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJEmitente = linha.Colunas?.Where(o => o.NomeCampo == "CNPJEmitente").FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorACobrar = linha.Colunas?.Where(o => o.NomeCampo == "ValorACobrar").FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorAValidar = linha.Colunas?.Where(o => o.NomeCampo == "ValorAValidar").FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModelo = linha.Colunas?.Where(o => o.NomeCampo == "Modelo").FirstOrDefault();

                    int.TryParse(Utilidades.String.OnlyNumbers(colNumeroCTe?.Valor), out int numeroCTe);
                    int.TryParse(Utilidades.String.OnlyNumbers(colSerieCTe?.Valor), out int serieCTe);
                    decimal valorAValidar = Math.Round(Utilidades.Decimal.Converter(colValorAValidar?.Valor), 2, MidpointRounding.AwayFromZero);
                    decimal valorACobrar = Math.Round(Utilidades.Decimal.Converter(colValorACobrar?.Valor), 2, MidpointRounding.AwayFromZero);
                    string cnpjEmpresa = Utilidades.String.OnlyNumbers(colCNPJEmitente?.Valor);
                    string modelo = ((string)colModelo?.Valor ?? string.Empty).Trim();

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarPorEmpresaECTe(cnpjEmpresa, numeroCTe, serieCTe, valorAValidar, modelo);

                    if (documentosFaturamento.Count == 0)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Nenhum documento encontrado com este número, série, valor ou modelo.", i));
                        continue;
                    }

                    if (documentosFaturamento.Count > 1)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Mais de um documento encontrado para este número, série, valor ou modelo.", i));
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = documentosFaturamento[0];

                    if ((fatura.GrupoPessoas != null && documentoFaturamento.GrupoPessoas != null && fatura.GrupoPessoas.Codigo != documentoFaturamento.GrupoPessoas.Codigo) || (fatura.Cliente != null && documentoFaturamento.Tomador != null && fatura.Cliente.CPF_CNPJ != documentoFaturamento.Tomador.CPF_CNPJ))
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O grupo de pessoas/tomador da fatura não condiz com o grupo de pessoas/tomador do documento.", i));
                        continue;
                    }

                    if (documentoFaturamento.ValorAFaturar < valorACobrar)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O valor à cobrar é maior que o valor disponível para faturar (" + documentoFaturamento.ValorAFaturar.ToString("n2") + ").", i));
                        continue;
                    }

                    if (documentoFaturamento.ValorAFaturar <= 0m)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não há valor disponível para faturar.", i));
                        continue;
                    }

                    //unitOfWork.Start();

                    if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out erro, ref fatura, documentoFaturamento.Codigo, valorACobrar, unitOfWork, null, true))
                    {
                        //unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(erro, i));
                        continue;
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Adicionou o documento " + documentoFaturamento.Numero.ToString() + " por importação.", unitOfWork);

                    contador++;

                    retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                    //unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }

                fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unitOfWork);

                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            try
            {
                int codigoFatura = Request.GetIntParam("Fatura");

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                var configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                List<int> codigosCTes = repFaturaDocumento.BuscarCodigosCTesPorFatura(codigoFatura);
                List<int> listaCodigosNFSes = new List<int>();

                if (codigosCTes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CT-es para esta fatura.");

                int maximoArquivos = configuracao.MaxDownloadsPorVez;

                if (maximoArquivos == 0)
                    maximoArquivos = 2000;

                if (maximoArquivos > 0 && maximoArquivos < codigosCTes.Count)
                    return new JsonpResult(false, $"Não é permitido o download de mais de {configuracao.MaxDownloadsPorVez} arquivos");

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeDACTE(codigosCTes, listaCodigosNFSes, unitOfWork);

                return Arquivo(arquivo, "application/octet-stream", $"{fatura.Numero}_DACTE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de DACTE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFatura = Request.GetIntParam("Fatura");

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                List<int> codigosCTes = repFaturaDocumento.BuscarCodigosCTesPorFatura(codigoFatura);

                if (codigosCTes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CT-es para esta fatura.");

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteXML(codigosCTes, unitOfWork);

                return Arquivo(arquivo, "application/octet-stream", $"{fatura.Numero}_XML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarAcrescimoDescontoDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");

                int.TryParse(Request.Params("Codigo"), out int codigoFatura);
                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);
                decimal.TryParse(Request.Params("Valor"), out decimal valorDescontoAcrescimo);
                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                DateTime? dataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                decimal valorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                decimal valorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, false, "Fatura não encontrada.");

                if (fatura.Situacao != SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "A situação da fatura não permite a inclusão de documentos.");

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (!this.Usuario.UsuarioAdministrador)
                {
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto) && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar desconto na fatura.");
                    else if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo) && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para adicionar acréscimo na fatura.");
                }

                unidadeTrabalho.Start();

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> listafaturaDocumento = repFaturaDocumento.BuscarPorFatura(codigoFatura).OrderBy(o => o.ValorTotalACobrar).ToList();

                decimal valorTotalDocumentos = listafaturaDocumento.Sum(o => o.ValorTotalACobrar);
                decimal valorTotalDocumentosMoeda = listafaturaDocumento.Where(o => o.Documento.ValorTotalMoeda.HasValue).Sum(o => o.Documento.ValorTotalMoeda.Value);
                decimal valorTotalRateado = 0m;
                decimal valorTotalRateadoMoeda = 0m;

                int totalRegistros = listafaturaDocumento.Count();

                for (int i = 0; i < totalRegistros; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = listafaturaDocumento[i];

                    decimal valorProporcional = 0m;
                    decimal valorPropMoeda = 0m;

                    if ((i + 1) == totalRegistros)
                    {
                        valorProporcional = valorDescontoAcrescimo - valorTotalRateado;
                        valorPropMoeda = valorOriginalMoedaEstrangeira - valorTotalRateadoMoeda;
                    }
                    else
                    {
                        valorProporcional = Math.Round((faturaDocumento.ValorTotalACobrar / valorTotalDocumentos) * valorDescontoAcrescimo, 2, MidpointRounding.ToEven);
                        valorPropMoeda = valorTotalDocumentosMoeda <= 0m ? 0m : Math.Round(((faturaDocumento.Documento.ValorTotalMoeda.HasValue ? faturaDocumento.Documento.ValorTotalMoeda.Value : 0) / valorTotalDocumentosMoeda) * valorOriginalMoedaEstrangeira, 2, MidpointRounding.ToEven);
                    }

                    if (valorProporcional <= 0m)
                        continue;

                    valorTotalRateado += valorProporcional;
                    valorTotalRateadoMoeda += valorPropMoeda;

                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto()
                    {
                        FaturaDocumento = faturaDocumento,
                        Observacao = observacao,
                        Justificativa = justificativa,
                        Valor = valorProporcional,
                        MoedaCotacaoBancoCentral = moeda,
                        DataBaseCRT = dataBaseCRT,
                        ValorMoedaCotacao = valorMoedaCotacao,
                        ValorOriginalMoedaEstrangeira = valorPropMoeda,
                        Usuario = Usuario
                    };

                    if (!faturaDocumentoAcrescimoDesconto.Justificativa.GerarMovimentoAutomatico && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        unidadeTrabalho.Rollback();
                        return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");
                    }

                    faturaDocumentoAcrescimoDesconto.TipoJustificativa = faturaDocumentoAcrescimoDesconto.Justificativa.TipoJustificativa;
                    faturaDocumentoAcrescimoDesconto.TipoMovimentoUso = faturaDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa;
                    faturaDocumentoAcrescimoDesconto.TipoMovimentoReversao = faturaDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa;

                    repFaturaDocumentoAcrescimoDesconto.Inserir(faturaDocumentoAcrescimoDesconto, Auditado);

                    if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    {
                        faturaDocumento.ValorAcrescimo += faturaDocumentoAcrescimoDesconto.Valor;
                        faturaDocumento.ValorAcrescimoMoeda += faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                    }
                    else
                    {
                        faturaDocumento.ValorDesconto += faturaDocumentoAcrescimoDesconto.Valor;
                        faturaDocumento.ValorDescontoMoeda += faturaDocumentoAcrescimoDesconto.ValorOriginalMoedaEstrangeira;
                    }

                    faturaDocumento.ValorTotalACobrar = faturaDocumento.ValorACobrar + faturaDocumento.ValorAcrescimo - faturaDocumento.ValorDesconto;

                    if (faturaDocumento.ValorTotalACobrar < 0m)
                    {
                        unidadeTrabalho.Rollback();
                        return new JsonpResult(false, true, "Não é possível adicionar este valor pois o total a cobrar ficará negativo.");
                    }

                    repFaturaDocumento.Atualizar(faturaDocumento);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, faturaDocumento, null, "Adicionou um " + faturaDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + ".", unidadeTrabalho);
                }

                Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsSituacaoFaturaPermiteRemoverDocumentos(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                return true;

            if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado)
                return false;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            return (fatura.ClienteTomadorFatura?.GrupoPessoas?.GerarFaturaPorCte ?? false) && configuracaoEmbarcador.TipoImpressaoFatura != TipoImpressaoFatura.Multimodal;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilha()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do CT-e", Propriedade = "NumeroCTe", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Série do CT-e", Propriedade = "SerieCTe", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CNPJ da Empresa/Filial", Propriedade = "CNPJEmitente", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Valor à Cobrar", Propriedade = "ValorACobrar", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Valor à Validar", Propriedade = "ValorAValidar", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Modelo de Documento", Propriedade = "Modelo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private dynamic ObterDetalhesGridFaturaDocumento(Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento)
        {
            return new
            {
                faturaDocumento.Codigo,
                faturaDocumento.Documento.TipoDocumento,
                Moeda = faturaDocumento.Documento.Moeda.HasValue ? faturaDocumento.Documento.Moeda.Value : MoedaCotacaoBancoCentral.Real,
                CodigoDocumento = faturaDocumento.Documento.Codigo,
                DataCTe = faturaDocumento.Documento?.CTe?.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                CodigoCTe = faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CTe.Codigo : 0 : 0,
                CodigoCarga = faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga ? faturaDocumento.Documento?.Carga?.Codigo : 0,
                Modelo = faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CTe?.ModeloDocumentoFiscal?.Numero : string.Empty : string.Empty,
                Documento = faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CTe?.Numero + "-" + faturaDocumento.Documento?.CTe?.Serie?.Numero : faturaDocumento.Documento?.Carga?.CodigoCargaEmbarcador : faturaDocumento.Documento?.Carga?.CodigoCargaEmbarcador,
                Tipo = faturaDocumento.Documento.DescricaoTipoDocumento,
                Origem = faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CTe?.LocalidadeInicioPrestacao?.DescricaoCidadeEstado : faturaDocumento.Documento.Carga?.DadosSumarizados.Origens : faturaDocumento.Documento.Carga?.DadosSumarizados.Origens,
                Destino = faturaDocumento.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe ? faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CTe?.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado : faturaDocumento.Documento.Carga?.DadosSumarizados.Destinos : faturaDocumento.Documento.Carga?.DadosSumarizados.Destinos,
                TipoOSConvertido = faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CargaPagamento?.TipoOSConvertido?.ObterDescricao() ?? string.Empty : string.Empty,
                ValorDocumento = faturaDocumento.Documento.ValorDocumento.ToString("n2"),
                ValorACobrar = faturaDocumento.ValorACobrar.ToString("n2"),
                ValorAcrescimo = faturaDocumento.ValorAcrescimo.ToString("n2"),
                ValorDesconto = faturaDocumento.ValorDesconto.ToString("n2"),
                TotalACobrar = faturaDocumento.ValorTotalACobrar.ToString("n2"),
                TipoDocumentoCTe = faturaDocumento.Documento?.CTe != null ? faturaDocumento.Documento?.CTe?.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "Complementar" : faturaDocumento.Documento.CTe?.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal ? "Normal" : string.Empty : string.Empty,
                Carga = faturaDocumento.Documento?.CargaPagamento?.CodigoCargaEmbarcador ?? faturaDocumento.Documento?.Carga?.CodigoCargaEmbarcador ?? "",
                StatusCanhoto = faturaDocumento.Documento?.CanhotosDigitalizados ?? false ? "Digitalizado" : "Pendente de Digitalização",
                NumeroNFe = faturaDocumento.Documento?.CTe != null ? string.Join(", ", faturaDocumento.Documento?.CTe?.XMLNotaFiscais?.Select(x => x.Numero)) : string.Empty,
                Escrituracao = faturaDocumento.Documento?.CTe != null ? string.IsNullOrWhiteSpace(faturaDocumento.Documento?.CTe?.CodigoEscrituracao) ? "Não" : "Sim" : "Sim",
                CteOriginario = faturaDocumento.Documento?.CTe.DocumentosOriginarios != null ? string.Join(", ", faturaDocumento.Documento?.CTe.DocumentosOriginarios?.Select(x => x.Numero)) : "0"
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura ObterFiltrosDocumentosParaFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                CodigoFatura = fatura.Codigo,
                CodigoCarga = Request.GetIntParam("Carga"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                TiposPropostasMultimodal = Usuario?.PerfilAcesso?.TiposPropostasMultimodal?.ToList(),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                CodigoGrupoPessoas = fatura.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa ? fatura.GrupoPessoas?.Codigo ?? 0 : 0,
                CPFCNPJTomador = fatura.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa ? fatura.Cliente?.CPF_CNPJ ?? 0D : 0D,
                AliquotaICMS = fatura.AliquotaICMS,
                CodigoContainer = fatura.Container?.Codigo ?? 0,
                CodigoCTe = fatura.CTe?.Codigo ?? 0,
                CodigoMDFe = fatura.MDFe?.Codigo ?? 0,
                CodigoVeiculo = fatura.Veiculo?.Codigo ?? 0,
                Destino = fatura.Destino?.Codigo ?? 0,
                NumeroBooking = fatura.NumeroBooking,
                NumeroControleCliente = fatura.NumeroControleCliente,
                NumeroReferenciaEDI = fatura.NumeroReferenciaEDI,
                PedidoViagemNavio = fatura.PedidoViagemNavio?.Codigo ?? 0,
                Origem = fatura.Origem?.Codigo ?? 0,
                TerminalDestino = fatura.TerminalDestino?.Codigo ?? 0,
                TerminalOrigem = fatura.TerminalOrigem?.Codigo ?? 0,
                TipoCarga = fatura.TipoCarga?.Codigo ?? 0,
                TipoOperacao = fatura.TipoOperacao?.Codigo ?? 0,
                TipoPropostaMultimodal = fatura.TipoPropostaMultimodal?.ToList(),
                Empresa = fatura.Transportador?.Codigo ?? 0,
                TipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoDocumento"),
                NumeroDocumentoInicial = Request.GetIntParam("NumeroDocumentoInicial"),
                NumeroDocumentoFinal = Request.GetIntParam("NumeroDocumentoFinal"),
                Serie = Request.GetIntParam("Serie"),
                PaisOrigem = fatura.PaisOrigem?.Codigo ?? 0,
                Notas = Request.GetIntParam("Notas"),
                HabilitarOpcaoGerarFaturasApenasCanhotosAprovados = fatura?.GerarDocumentosApenasCanhotosAprovados ?? false,
                Filial = fatura.Filial?.Codigo ?? 0,
                TomadorFatura = fatura.Tomador?.Codigo ?? 0,
                NumeroContainer = Request.GetStringParam("Container"),
                CodigoCentroResultado = fatura.CentroResultado?.Codigo ?? 0,
                TiposOSConvertidos = fatura.TiposOSConvertidos.ToList(),
                GerarDocumentosApenasCanhotosAprovados = fatura.GerarDocumentosApenasCanhotosAprovados,
            };

            if (!(filtros.DataInicial >= fatura.DataInicial && filtros.DataInicial <= fatura.DataFinal))
                filtros.DataInicial = fatura.DataInicial;

            if (!(filtros.DataFinal >= fatura.DataInicial && filtros.DataFinal <= fatura.DataFinal))
                filtros.DataFinal = fatura.DataFinal;

            return filtros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                propriedadeOrdenar = "Numero";
            else if (propriedadeOrdenar == "Valor")
                propriedadeOrdenar = "ValorDocumento";
            else if (propriedadeOrdenar == "TipoDocumentoCTe")
                propriedadeOrdenar = "TipoCTE";
            else if (propriedadeOrdenar == "Tipo")
                propriedadeOrdenar = "AbreviacaoModeloDocumentoFiscal";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
