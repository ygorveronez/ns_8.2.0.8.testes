using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PlanoEmissaoCTeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("planos.aspx") select obj).FirstOrDefault();
        }

        protected List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas> _RelatorioCobranca;

        protected List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2> _RelatorioCobranca2;

        protected enum FiltroRelatorio
        {
            ComEmissao = 1,
            SemEmissao = 2,
            Todos = 0
        }

        #endregion

        #region Métodos Publicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["incioRegistros"], out inicioRegistros);

                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];

                Repositorio.PlanoEmissaoCTe repPlano = new Repositorio.PlanoEmissaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.PlanoEmissaoCTe> planos = repPlano.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countPlanos = repPlano.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in planos select new { obj.Codigo, obj.Status, obj.Descricao, obj.DescricaoFaixas, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Descricao|55", "DescricaoFaixas|20", "Status|15" }, countPlanos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os planos.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ObterFaixasDoPlano()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoPlano = 0;
                int.TryParse(Request.Params["CodigoPlano"], out codigoPlano);

                Repositorio.FaixaEmissaoCTe repFaixaEmissao = new Repositorio.FaixaEmissaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.FaixaEmissaoCTe> faixas = repFaixaEmissao.BuscarPorPlano(codigoPlano);

                var resultado = from obj in faixas
                                select new Dominio.ObjetosDeValor.FaixaEmissaoCTe()
                                {
                                    Codigo = obj.Codigo,
                                    Excluir = false,
                                    Quantidade = obj.Quantidade,
                                    Valor = obj.Valor
                                };

                return Json(resultado, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as faixas de emissão do plano.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ObterValoresPorDocumentos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoPlano = 0;
                int.TryParse(Request.Params["CodigoPlano"], out codigoPlano);

                Repositorio.ValoresPorDocumentos repValoresPorDocumentos = new Repositorio.ValoresPorDocumentos(unidadeDeTrabalho);

                List<Dominio.Entidades.ValoresPorDocumentos> valoresPorDocumentos = repValoresPorDocumentos.BuscarPorPlano(codigoPlano);

                var resultado = from obj in valoresPorDocumentos
                                select new Dominio.ObjetosDeValor.ValoresPorDocumentos()
                                {
                                    Codigo = obj.Codigo,
                                    Excluir = false,
                                    Descricao = obj.Descricao,
                                    Series = obj.Series,
                                    SerieDiferente = obj.SerieDiferente ? "Sim" : "Não",
                                    Valor = obj.Valor
                                };

                return Json(resultado, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os valores por documentos do plano.");
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
                if (this.EmpresaUsuario.EmpresaPai != null || this.EmpresaUsuario.EmpresaAdministradora == null)
                    return Json<bool>(false, false, "Empresa inválida.");

                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                string descricao = Request.Params["Descricao"];
                string descricaoFaixas = Request.Params["DescricaoFaixas"];
                string status = Request.Params["Status"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.PlanoEmissaoCTe repPlano = new Repositorio.PlanoEmissaoCTe(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.PlanoEmissaoCTe plano = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    plano = repPlano.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    plano = new Dominio.Entidades.PlanoEmissaoCTe();
                }

                plano.Descricao = descricao;
                plano.Status = status;
                plano.Empresa = this.EmpresaUsuario;
                plano.DescricaoFaixas = descricaoFaixas;

                if (plano.Codigo > 0)
                    repPlano.Atualizar(plano);
                else
                    repPlano.Inserir(plano);

                List<Dominio.ObjetosDeValor.FaixaEmissaoCTe> faixasDeEmissao = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.FaixaEmissaoCTe>>(Request.Params["FaixasDeEmissao"]);

                if (faixasDeEmissao != null)
                {
                    Repositorio.FaixaEmissaoCTe repFaixa = new Repositorio.FaixaEmissaoCTe(unidadeDeTrabalho);

                    foreach (Dominio.ObjetosDeValor.FaixaEmissaoCTe faixaEmissao in faixasDeEmissao)
                    {
                        Dominio.Entidades.FaixaEmissaoCTe faixa = faixaEmissao.Codigo > 0 ? repFaixa.BuscarPorCodigo(faixaEmissao.Codigo) : new Dominio.Entidades.FaixaEmissaoCTe();
                        if (!faixaEmissao.Excluir)
                        {
                            faixa.Plano = plano;
                            faixa.Quantidade = faixaEmissao.Quantidade;
                            faixa.Valor = faixaEmissao.Valor;

                            if (faixa.Codigo > 0)
                                repFaixa.Atualizar(faixa);
                            else
                                repFaixa.Inserir(faixa);
                        }
                        else if (faixaEmissao.Codigo > 0)
                        {
                            repFaixa.Deletar(faixa);
                        }
                    }
                }

                List<Dominio.ObjetosDeValor.ValoresPorDocumentos> valoresPorDocumentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ValoresPorDocumentos>>(Request.Params["ValoresPorDocumentos"]);

                Repositorio.ValoresPorDocumentos repValoresPorDocumentos = new Repositorio.ValoresPorDocumentos(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.ValoresPorDocumentos valorPorDocumento in valoresPorDocumentos)
                {
                    Dominio.Entidades.ValoresPorDocumentos valor = valorPorDocumento.Codigo > 0 ? repValoresPorDocumentos.BuscarPorCodigo(valorPorDocumento.Codigo) : new Dominio.Entidades.ValoresPorDocumentos();
                    if (!valorPorDocumento.Excluir)
                    {
                        valor.Plano = plano;
                        valor.Descricao = valorPorDocumento.Descricao;
                        valor.Series = valorPorDocumento.Series;
                        valor.SerieDiferente = valorPorDocumento.SerieDiferente == "Sim";
                        valor.Valor = valorPorDocumento.Valor;

                        if (valor.Codigo > 0)
                            repValoresPorDocumentos.Atualizar(valor);
                        else
                            repValoresPorDocumentos.Inserir(valor);
                    }
                    else if (valorPorDocumento.Codigo > 0)
                    {
                        repValoresPorDocumentos.Deletar(valor);
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o plano de emissão.");
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult RelatorioCobrancas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                /* Fluxo:
                 * - Iterar as empresas (no caso do relatorio for apenas de uma, cria um array de empresas com 1 item apenas)
                 * - Definir os objetos de valores
                 * - Busca empresas pai
                 * - Percorrer cada empresa
                 *   - Busca empresa unica ou todassr
                 *     - Buscar CTes no periodo
                 *     - Buscar MDFes no periodo
                 *     - Sumariza total de documentos
                 *     - Verifica enquadramento do preço
                 *     - Soma no objeto do relatorio
                 * - Cria o relatorio
                 */

                int codigoEmpresa, codigoEmpresaPai = 0;
                int.TryParse(Request.Params["Empresa"], out codigoEmpresa);
                codigoEmpresaPai = this.EmpresaUsuario.CNPJ == "13969629000196" ? 0 : this.EmpresaUsuario.Codigo;

                DateTime dataInicial, dataFinal, originalDataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string tipoArquivo = Request.Params["Arquivo"];
                string layout = Request.Params["Layout"];

                FiltroRelatorio filtroAux;
                FiltroRelatorio? filtro = null;
                if (Enum.TryParse<FiltroRelatorio>(Request.Params["Filtro"], out filtroAux))
                    filtro = filtroAux;

                // Remove os dias das datas
                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, 1);
                dataFinal = new DateTime(dataFinal.Year, dataFinal.Month, 1);

                if (dataInicial == DateTime.MinValue || dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Data inicial e final são obrigatórios.");

                if (DateTime.Compare(dataInicial, dataFinal) > 0)
                    return Json<bool>(false, false, "Data inicial deve ser menor ou igual à data final.");

                if (filtro == null)
                    return Json<bool>(false, false, "O filtro selecionado é inválido.");

                if (string.IsNullOrWhiteSpace(tipoArquivo) || !(tipoArquivo.Equals("PDF") || tipoArquivo.Equals("Excel")))
                    return Json<bool>(false, false, "O tipo do arquivo é inválido.");

                string empresaSelecionada = "";
                string descricaoFiltro = "";

                // Descricao do filtro
                if (filtro == FiltroRelatorio.ComEmissao)
                    descricaoFiltro = "Com Emissão";
                else if (filtro == FiltroRelatorio.SemEmissao)
                    descricaoFiltro = "Sem Emissão";
                else
                    descricaoFiltro = "Todas";

                // Mantem a data final salva para mostrar no relatorio
                originalDataFinal = dataFinal;

                // Seta o mes final como ultimo dia
                dataFinal = dataFinal.AddDays(DateTime.DaysInMonth(dataFinal.Year, dataFinal.Month)).AddDays(-1);

                if (layout == "1")
                {
                    this._RelatorioCobranca = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas>();

                    // Objeto global do relatorio
                    List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas> relatorioCobrancas = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas>();

                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                    List<Dominio.Entidades.Empresa> empresas = new List<Dominio.Entidades.Empresa>();

                    if (codigoEmpresa > 0)
                    {
                        // Adiciona a empresa filha na lista de empresas
                        Dominio.Entidades.Empresa empresaFilha = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                        if (empresaFilha != null)
                        {
                            empresas.Add(empresaFilha);
                            // Seta nome para usar no relatorio
                            empresaSelecionada = empresaFilha.RazaoSocial;
                        }
                    }
                    else
                        empresas = repEmpresa.BuscarTodasEmpresaPai("A", codigoEmpresaPai);

                    // Gera a lista de relatorios
                    this.RelatorioEmpresaFilha(empresas, dataInicial, dataFinal, filtro, unidadeDeTrabalho);


                    // Monta o arquivo PDF
                    List<ReportParameter> parametros = new List<ReportParameter>();
                    parametros.Add(new ReportParameter("Empresa", !string.IsNullOrWhiteSpace(empresaSelecionada) ? empresaSelecionada : "Todas"));
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("MM/yyyy"), " até ", originalDataFinal.ToString("MM/yyyy"))));
                    parametros.Add(new ReportParameter("FiltroEmissao", descricaoFiltro));

                    List<ReportDataSource> dataSources = new List<ReportDataSource>();
                    dataSources.Add(new ReportDataSource("Empresas", this._RelatorioCobranca));

                    Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCobrancas.rdlc", tipoArquivo, parametros, dataSources);

                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, string.Concat("RelatorioCobrancas", dataInicial.ToString("MM-yyyy"), "_", originalDataFinal.ToString("MM-yyyy"), ".", arquivo.FileNameExtension.ToLower()));
                }
                else
                {
                    this._RelatorioCobranca2 = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2>();

                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                    Repositorio.PlanoEmissaoCTe repPlano = new Repositorio.PlanoEmissaoCTe(unidadeDeTrabalho);
                    Repositorio.FaixaEmissaoCTe repFaixaEmisao = new Repositorio.FaixaEmissaoCTe(unidadeDeTrabalho);
                    Repositorio.ValoresPorDocumentos repValoresPorDocumentos = new Repositorio.ValoresPorDocumentos(unidadeDeTrabalho);
                    Repositorio.DespesaAdicionalEmpresa repDespesaAdicional = new Repositorio.DespesaAdicionalEmpresa(unidadeDeTrabalho);
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                    List<string> colunasDocumentos = repValoresPorDocumentos.BuscarDescricoes(codigoEmpresaPai);
                    List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarComPlano(codigoEmpresaPai, codigoEmpresa, this.EmpresaUsuario.TipoAmbiente);
                    List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2> relatorioCobrancas2 = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2>();

                    for (var i = 0; i < listaEmpresas.Count(); i++)
                    {
                        Dominio.Entidades.PlanoEmissaoCTe plano = repPlano.BuscarPorEmpresa(listaEmpresas[i].Codigo);
                        if (plano != null && listaEmpresas[i].EmpresaCobradora == null) //
                        {
                            List<Dominio.Entidades.Empresa> listaEmpresasAssociadas = repEmpresa.BuscarPorEmpresasDaEmpresaCobradora(listaEmpresas[i].CNPJ);

                            Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2 cobranca = new Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2();
                            cobranca.CodigoEmpresaPai = listaEmpresas[i].EmpresaPai.Codigo;
                            cobranca.EmpresaPai = listaEmpresas[i].EmpresaPai.Descricao;

                            cobranca.Email = !string.IsNullOrWhiteSpace(listaEmpresas[i].EmailAdministrativo) ? "E-MAIL" : string.Empty; //Solicitado pela Sintravir quando tiver e-mail cadastrado no campo Administrativo exibir a palava e-mail
                            cobranca.Empresa = listaEmpresas[i].RazaoSocial;

                            int quantidadeTotalCTes = 0;

                            for (var j = 0; j < colunasDocumentos.Distinct().Count(); j++)
                            {
                                Dominio.Entidades.ValoresPorDocumentos valorPorDocumento = (from o in plano.ValoresPorDocumentos where o.Descricao == colunasDocumentos[j] select o).FirstOrDefault();
                                if (valorPorDocumento != null)
                                {
                                    List<int> series = null;
                                    List<int> seriesDiferente = null;

                                    if (valorPorDocumento.SerieDiferente && !string.IsNullOrWhiteSpace(valorPorDocumento.Series))
                                        seriesDiferente = Array.ConvertAll(valorPorDocumento.Series.Split(','), int.Parse).ToList();
                                    else if (!string.IsNullOrWhiteSpace(valorPorDocumento.Series))
                                        series = Array.ConvertAll(valorPorDocumento.Series.Split(','), int.Parse).ToList();

                                    int quantidadeCTe = repCTe.ContarCTesParaCobrancaMensal(codigoEmpresaPai, listaEmpresas[i].Codigo, dataInicial, dataFinal, listaEmpresas[i].TipoAmbiente, series != null ? series.ToArray() : null, seriesDiferente != null ? seriesDiferente.ToArray() : null);
                                    int quantidadeMDFe = repMDFe.ContarMDFesParaCobrancaMensal(codigoEmpresaPai, listaEmpresas[i].Codigo, dataInicial, dataFinal, listaEmpresas[i].TipoAmbiente, series != null ? series.ToArray() : null, seriesDiferente != null ? seriesDiferente.ToArray() : null);

                                    //busca quantidade CTes empresas associadas
                                    foreach (Dominio.Entidades.Empresa empresaAssociada in listaEmpresasAssociadas)
                                    {
                                        quantidadeCTe += repCTe.ContarCTesParaCobrancaMensal(codigoEmpresaPai, empresaAssociada.Codigo, dataInicial, dataFinal, empresaAssociada.TipoAmbiente, series != null ? series.ToArray() : null, seriesDiferente != null ? seriesDiferente.ToArray() : null);
                                       quantidadeMDFe += repMDFe.ContarMDFesParaCobrancaMensal(codigoEmpresaPai, empresaAssociada.Codigo, dataInicial, dataFinal, empresaAssociada.TipoAmbiente, series != null ? series.ToArray() : null, seriesDiferente != null ? seriesDiferente.ToArray() : null);
                                    }

                                    decimal valorCobradoCTe = valorPorDocumento.Valor * quantidadeCTe;
                                    quantidadeTotalCTes = quantidadeTotalCTes + quantidadeCTe;

                                    if (j == 0)
                                    {
                                        cobranca.DescricaoCTe1 = "Qtd. " + colunasDocumentos[j];
                                        cobranca.QtdCTe1 = quantidadeCTe;
                                        cobranca.DescricaoValorCTe1 = "Total " + colunasDocumentos[j];
                                        cobranca.ValorCTe1 = valorCobradoCTe;
                                    }
                                    else if (j == 1)
                                    {
                                        cobranca.DescricaoCTe2 = "Qtd. " + colunasDocumentos[j];
                                        cobranca.QtdCTe2 = quantidadeCTe;
                                        cobranca.DescricaoValorCTe2 = "Total " + colunasDocumentos[j];
                                        cobranca.ValorCTe2 = valorCobradoCTe;
                                    }
                                    else if (j == 2)
                                    {
                                        cobranca.DescricaoCTe3 = "Qtd. " + colunasDocumentos[j];
                                        cobranca.QtdCTe3 = quantidadeCTe;
                                        cobranca.DescricaoValorCTe3 = "Total " + colunasDocumentos[j];
                                        cobranca.ValorCTe3 = valorCobradoCTe;
                                    }
                                    else if (j == 3)
                                    {
                                        cobranca.DescricaoCTe4 = "Qtd. " + colunasDocumentos[j];
                                        cobranca.QtdCTe3 = quantidadeCTe;
                                        cobranca.DescricaoValorCTe4 = "Total " + colunasDocumentos[j];
                                        cobranca.ValorCTe4 = valorCobradoCTe;
                                    }
                                    else if (j == 4)
                                    {
                                        cobranca.DescricaoCTe5 = "Qtd. " + colunasDocumentos[j];
                                        cobranca.QtdCTe4 = quantidadeCTe;
                                        cobranca.DescricaoValorCTe5 = "Total " + colunasDocumentos[j];
                                        cobranca.ValorCTe5 = valorCobradoCTe;
                                    }
                                    else if (j == 5)
                                    {
                                        cobranca.DescricaoCTe6 = "Qtd. " + colunasDocumentos[j];
                                        cobranca.QtdCTe5 = quantidadeCTe;
                                        cobranca.DescricaoValorCTe = "Total " + colunasDocumentos[j];
                                        cobranca.ValorCTe6 = valorCobradoCTe;
                                    }

                                    decimal valorCobradoMDFe = valorPorDocumento.Valor * quantidadeMDFe;

                                    cobranca.DescricaoMDFe = "Qtd. MDFe";
                                    cobranca.QtdMDFe += quantidadeMDFe;
                                    cobranca.DescricaoValorMDFe = "Total MDFe";
                                    cobranca.ValorMDFe += valorCobradoMDFe;
                                }
                            }

                            List<Dominio.Entidades.FaixaEmissaoCTe> faixas = null;
                            if (plano != null)
                                faixas = repFaixaEmisao.BuscarPorPlanoOrdenado(plano.Codigo);

                            cobranca.DescricaoFaixas = plano.DescricaoFaixas;
                            cobranca.ValorFaixas = BuscaValorPorEmpresa(faixas, quantidadeTotalCTes); // + cobranca.QtdMDFe : Sintravir pediu para não considera o valor do MDFe 

                            cobranca.Acrescimos = repDespesaAdicional.BuscarTotalPorEmpresa(listaEmpresas[i].Codigo, dataInicial, dataFinal, "A", "A");
                            cobranca.Descontos = repDespesaAdicional.BuscarTotalPorEmpresa(listaEmpresas[i].Codigo, dataInicial, dataFinal, "A", "D");

                            cobranca.Total = cobranca.ValorFaixas + cobranca.ValorCTe1 + cobranca.ValorCTe2 + cobranca.ValorCTe3 + cobranca.ValorCTe4 + cobranca.ValorCTe5 + cobranca.ValorCTe6 + cobranca.ValorMDFe + cobranca.Acrescimos - cobranca.Descontos;
                            if (filtro == FiltroRelatorio.ComEmissao)
                            {
                                if (cobranca.QtdMDFe + quantidadeTotalCTes > 0)
                                    relatorioCobrancas2.Add(cobranca);
                            }
                            else if (filtro == FiltroRelatorio.SemEmissao)
                            {
                                if (cobranca.QtdMDFe + quantidadeTotalCTes == 0)
                                    relatorioCobrancas2.Add(cobranca);
                            }
                            else
                                relatorioCobrancas2.Add(cobranca);
                        }
                    }

                    //Gerar excel
                    //List<List<string>> estruturaCSV = new List<List<string>>();                    
                    //List<string> cabecalho = RetornarCabecalho(colunasDocumentos.Distinct().Count(), relatorioCobrancas2); //Cabeçalho

                    List<ReportParameter> parametros = new List<ReportParameter>();
                    parametros.Add(new ReportParameter("Empresa", !string.IsNullOrWhiteSpace(empresaSelecionada) ? empresaSelecionada : "Todas"));
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("MM/yyyy"), " até ", originalDataFinal.ToString("MM/yyyy"))));
                    parametros.Add(new ReportParameter("FiltroEmissao", descricaoFiltro));

                    List<ReportDataSource> dataSources = new List<ReportDataSource>();
                    dataSources.Add(new ReportDataSource("Empresas", relatorioCobrancas2));

                    Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCobrancas2.rdlc", tipoArquivo, parametros, dataSources);

                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, string.Concat("RelatorioCobrancas", dataInicial.ToString("MM-yyyy"), "_", originalDataFinal.ToString("MM-yyyy"), ".", arquivo.FileNameExtension.ToLower()));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        //private List<string> RetornarCabecalho(int colunasCTe, List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2> cobrancas)
        //{
        //    List<string> estruturaCSV = null;

        //    if (colunasCTe == 1)
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    else if (colunasCTe == 2)
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoCTe2,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe2,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    else if (colunasCTe == 3)
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoCTe2,
        //            cobrancas.FirstOrDefault().DescricaoCTe3,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe2,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe3,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    else if (colunasCTe == 4)
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoCTe2,
        //            cobrancas.FirstOrDefault().DescricaoCTe3,
        //            cobrancas.FirstOrDefault().DescricaoCTe4,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe2,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe3,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe4,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    else if (colunasCTe == 5)
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoCTe2,
        //            cobrancas.FirstOrDefault().DescricaoCTe3,
        //            cobrancas.FirstOrDefault().DescricaoCTe4,
        //            cobrancas.FirstOrDefault().DescricaoCTe5,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe2,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe3,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe4,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe5,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    else if (colunasCTe == 6)
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoCTe2,
        //            cobrancas.FirstOrDefault().DescricaoCTe3,
        //            cobrancas.FirstOrDefault().DescricaoCTe4,
        //            cobrancas.FirstOrDefault().DescricaoCTe5,
        //            cobrancas.FirstOrDefault().DescricaoCTe6,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe2,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe3,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe4,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe5,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe6,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    else
        //    {
        //        estruturaCSV = new List<string>()
        //        {
        //            "EMAIL",
        //            "EMPRESA",
        //            cobrancas.FirstOrDefault().DescricaoCTe1,
        //            cobrancas.FirstOrDefault().DescricaoCTe2,
        //            cobrancas.FirstOrDefault().DescricaoCTe3,
        //            cobrancas.FirstOrDefault().DescricaoMDFe,
        //            cobrancas.FirstOrDefault().DescricaoFaixas,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe1,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe2,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoCTe3,
        //            "TOTAL " + cobrancas.FirstOrDefault().DescricaoMDFe,
        //            "ACRÉSCIMOS",
        //            "DESCONTOS",
        //            "TOTAL"
        //        };
        //    }
        //    return estruturaCSV;
        //}

        //private List<List<string>> RetornarDados(int colunasCTe, List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2> cobrancas)
        //{
        //    List<List<string>> estruturaCSV = new List<List<string>>();

        //    for (var i = 0; i < cobrancas.Count; i++)
        //    {
        //        if (colunasCTe == 1)
        //        {
        //            estruturaCSV.Add(new List<string>() {
        //            cobrancas[i].Email,
        //            cobrancas[i].Empresa,
        //            cobrancas[i].QtdCTe1.ToString(),
        //            cobrancas[i].QtdMDFe.ToString(),
        //            cobrancas[i].ValorFaixas.ToString("n2"),
        //            cobrancas[i].ValorCTe1.ToString("n2"),
        //            cobrancas[i].ValorMDFe.ToString("n2"),
        //            cobrancas[i].Acrescimos.ToString("n2"),
        //            cobrancas[i].Descontos.ToString("n2"),
        //            cobrancas[i].Total.ToString("n2"),
        //            });
        //        } else 
        //    }

        //    return estruturaCSV;
        //}


        private void RelatorioEmpresaFilha(List<Dominio.Entidades.Empresa> empresas, DateTime dataInicial, DateTime dataFinal, FiltroRelatorio? filtro, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas> relatorioEmpresaFilha = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas>();
            Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas relatorioEmpresa = new Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas();

            for (var i = 0; i < empresas.Count(); i++)
            {
                // Pai
                relatorioEmpresa.NomeEmpresaPai = empresas[i].EmpresaPai?.RazaoSocial;
                relatorioEmpresa.CNPJEmpresaPai = empresas[i].EmpresaPai?.CNPJ_Formatado;

                // Seta informacoes
                relatorioEmpresa.NomeEmpresa = empresas[i].RazaoSocial;
                relatorioEmpresa.CNPJEmpresa = empresas[i].CNPJ_Formatado;

                // Relatorio por empresa
                this.PeriodosRelatorioDaEmpresa(empresas[i], dataInicial, dataFinal, filtro, relatorioEmpresa, unitOfWork);
            }
        }

        private void PeriodosRelatorioDaEmpresa(Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, FiltroRelatorio? filtro, Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas relatorioEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.FaixaEmissaoCTe repFaixaEmisao = new Repositorio.FaixaEmissaoCTe(unitOfWork);

            int numeroDeMeses = (((dataFinal.Year - dataInicial.Year) * 12) + dataFinal.Month - dataInicial.Month);
            DateTime periodo = dataInicial;
            List<Dominio.Entidades.FaixaEmissaoCTe> faixas = null;

            if (empresa.PlanoEmissaoCTe?.Codigo > 0)
                faixas = repFaixaEmisao.BuscarPorPlanoOrdenado(empresa.PlanoEmissaoCTe.Codigo);

            for (var i = 0; i < numeroDeMeses; i++)
            {
                Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas relatorioPeriodo = new Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas()
                {
                    NomeEmpresaPai = relatorioEmpresa.NomeEmpresaPai,
                    CNPJEmpresaPai = relatorioEmpresa.CNPJEmpresaPai,
                    NomeEmpresa = relatorioEmpresa.NomeEmpresa,
                    CNPJEmpresa = relatorioEmpresa.CNPJEmpresa
                };

                int ctesEmitidos = repCTe.RelatorioCobrancaQuantidadeCTes(empresa.Codigo, periodo, periodo.AddMonths(1));
                int mdfesEmitidos = repMDFe.RelatorioCobrancaQuantidadeMDFes(empresa.Codigo, periodo, periodo.AddMonths(1));

                relatorioPeriodo.Periodo = this.MontaPeriodo(periodo);
                relatorioPeriodo.CTesEmitidos = ctesEmitidos;
                relatorioPeriodo.MDFesEmitidos = mdfesEmitidos;
                relatorioPeriodo.TotalDeDocumentos = ctesEmitidos + mdfesEmitidos;
                relatorioPeriodo.ValorCobranca = this.BuscaValorPorEmpresa(faixas, relatorioPeriodo.TotalDeDocumentos);

                if (this.FiltraRelatorio(relatorioPeriodo, filtro))
                    this._RelatorioCobranca.Add(relatorioPeriodo);

                periodo = periodo.AddMonths(1);
            }
        }

        private bool FiltraRelatorio(Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas relatorioPeriodo, FiltroRelatorio? filtro)
        {
            if (filtro == FiltroRelatorio.ComEmissao && relatorioPeriodo.TotalDeDocumentos > 0)
                return true;
            else if (filtro == FiltroRelatorio.SemEmissao && relatorioPeriodo.TotalDeDocumentos == 0)
                return true;
            else if (filtro == FiltroRelatorio.Todos)
                return true;

            return false;
        }

        private decimal BuscaValorPorEmpresa(List<Dominio.Entidades.FaixaEmissaoCTe> faixas, int numeroDeDocumentos)
        {
            if (faixas != null && faixas.Count > 0 && numeroDeDocumentos > 0)
            {
                for (var i = 0; i < faixas.Count(); i++)
                {
                    if (numeroDeDocumentos >= faixas[i].Quantidade)
                    {
                        if (i == 0 || numeroDeDocumentos == faixas[i].Quantidade)
                            return faixas[i].Valor;
                        else
                            return faixas[i - 1].Valor;
                    }
                }
                return faixas.LastOrDefault().Valor;
            }

            return 0;
        }

        private string MontaPeriodo(DateTime periodo)
        {
            string mes = periodo.ToString("MMMM", CultureInfo.CreateSpecificCulture("pt-BR"));
            string ano = periodo.Year.ToString();

            return mes.Substring(0, 3) + "/" + ano.Substring(2, 2);
        }
        #endregion

    }
}
