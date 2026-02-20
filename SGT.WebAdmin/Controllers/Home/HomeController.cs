using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGT.WebAdmin.Models;
using System.Diagnostics;

namespace SGT.WebAdmin.Controllers.Home
{
    [CustomAuthorize("Home")]
    public class HomeController : BaseController
    {
		#region Construtores

		public HomeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Index()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
                Dominio.Entidades.Usuario usuarioLogado = this.Usuario;
                ViewBag.NomeUsuario = usuarioLogado.Nome;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    ViewBag.Saudacao = $"{Localization.Resources.Gerais.Home.SejaBemVindo}, {usuarioLogado.Nome ?? string.Empty}!";
                else
                    ViewBag.Saudacao = $"{Localization.Resources.Gerais.Home.SejaBemVindo}, {usuarioLogado.Nome?.Split(' ')[0] ?? string.Empty}!";

                ViewBag.LayoutAmarelo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaLayoutPersonalizado;
                ViewBag.DataAtual = DateTime.Now.ToString("dddd, d 'de' MMMM");


                bool portalCliente = IsLayoutClienteAtivo(unitOfWork);
                bool portalCabotagem = IsLayoutCabotagem(unitOfWork);

                string caminhoBaseViews = "~/Views/Home/";
                string caminhosViewMasterLayout = portalCabotagem ? $"{caminhoBaseViews}/IndexCabotagem.cshtml" : portalCliente ? $"{caminhoBaseViews}/IndexCliente.cshtml" : $"{caminhoBaseViews}/Index.cshtml";

                return View(caminhosViewMasterLayout);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAvisoFornecedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = ObterModalidadeFornecedorDoUsuario(unitOfWork);

                if (modalidadeFornecedorPessoas == null)
                    return new JsonpResult(new { });

                return new JsonpResult(new
                {
                    TextoAviso = modalidadeFornecedorPessoas.TextoAvisoHTML,
                    Anexos = (from a in modalidadeFornecedorPessoas.Anexos
                              select new
                              {
                                  a.Codigo,
                                  a.Descricao
                              }).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os valores do faturamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexoAviso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.MensagemAvisoAnexo repMensagemAvisoAnexo = new Repositorio.MensagemAvisoAnexo(unitOfWork);

                Dominio.Entidades.MensagemAvisoAnexo anexo = repMensagemAvisoAnexo.BuscarPorCodigoETipoServico(codigo, TipoServicoMultisoftware);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string caminho = ObterCaminhoArquivos(unitOfWork);
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extensao}");
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.EntidadeAnexo, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexoFornecedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.PessoaFornecedorAnexo repPessoaFornecedorAnexo = new Repositorio.Embarcador.Pessoas.PessoaFornecedorAnexo(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = ObterModalidadeFornecedorDoUsuario(unitOfWork);
                if (modalidadeFornecedorPessoas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo anexo = repPessoaFornecedorAnexo.BuscarPorModalidadeECodigo(modalidadeFornecedorPessoas.Codigo, codigo);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string caminho = ObterCaminhoArquivosFornecedor(unitOfWork);
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extensao}");
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.EntidadeAnexo, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarGraficosGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresa = this.Usuario.Empresa.Codigo;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.GraficoFaturamento> valores = repMovimentoFinanceiro.BuscarGraficoFaturamento(codigoEmpresa, tipoAmbiente);

                var lista = (from obj in valores
                             select new
                             {
                                 obj.Codigo,
                                 obj.Cor,
                                 obj.DescricaoTipo,
                                 obj.Icone,
                                 obj.Tipo,
                                 obj.Imagem,
                                 obj.Quantidade,
                                 Valor = obj.Valor.ToString("n2")
                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os valores do faturamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterValoresFaturamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);

                DateTime dataAtual = DateTime.Now.Date;
                DateTime data1 = dataAtual.AddMonths(-1);
                DateTime data2 = dataAtual.AddMonths(-2);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                decimal valorFaturamentoData5 = repTitulo.ValorFaturamentoMes(codigoEmpresa, data2, tipoAmbiente);
                decimal valorReceitasData5 = repTitulo.ValorReceitasMes(codigoEmpresa, data2, tipoAmbiente);
                decimal valorRecebidoData5 = repTitulo.ValorRecebidoMes(codigoEmpresa, data2, tipoAmbiente);
                decimal valorLiquidadoData5 = repTitulo.ValorLiquidadoMes(codigoEmpresa, data2, tipoAmbiente);
                decimal valorDespesaData5 = repTitulo.ValorDespesasMes(codigoEmpresa, data2, tipoAmbiente);
                decimal valorPagoData5 = repTitulo.ValorPagoMes(codigoEmpresa, data2, tipoAmbiente);

                decimal valorFaturamentoData4 = repTitulo.ValorFaturamentoMes(codigoEmpresa, data1, tipoAmbiente);
                decimal valorReceitasData4 = repTitulo.ValorReceitasMes(codigoEmpresa, data1, tipoAmbiente);
                decimal valorRecebidoData4 = repTitulo.ValorRecebidoMes(codigoEmpresa, data1, tipoAmbiente);
                decimal valorLiquidadoData4 = repTitulo.ValorLiquidadoMes(codigoEmpresa, data1, tipoAmbiente);
                decimal valorDespesaData4 = repTitulo.ValorDespesasMes(codigoEmpresa, data1, tipoAmbiente);
                decimal valorPagoData4 = repTitulo.ValorPagoMes(codigoEmpresa, data1, tipoAmbiente);

                decimal valorFaturamentoDataAtual = repTitulo.ValorFaturamentoMes(codigoEmpresa, dataAtual, tipoAmbiente);
                decimal valorReceitasDataAtual = repTitulo.ValorReceitasMes(codigoEmpresa, dataAtual, tipoAmbiente);
                decimal valorRecebidoDataAtual = repTitulo.ValorRecebidoMes(codigoEmpresa, dataAtual, tipoAmbiente);
                decimal valorLiquidadoDataAtual = repTitulo.ValorLiquidadoMes(codigoEmpresa, dataAtual, tipoAmbiente);
                decimal valorDespesaDataAtual = repTitulo.ValorDespesasMes(codigoEmpresa, dataAtual, tipoAmbiente);
                decimal valorPagoDataAtual = repTitulo.ValorPagoMes(codigoEmpresa, dataAtual, tipoAmbiente);

                var colorFaturamento = "#2B71B5";
                var colorReceita = "#7AD684";
                var colorRecebido = "#4ABA58";
                var colorLiquidado = "#00695C";
                var colorDespesa = "#DE7775";
                var colorPago = "#C9312D";

                List<dynamic> retorno = new List<dynamic>();

                retorno.Add(new { Descricao = "Faturamento " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data2.Month).ToUpper().Substring(0, 3) + " - " + data2.Year.ToString(), Valor = valorFaturamentoData5, DescricaoValor = "Faturamento: " + valorFaturamentoData5.ToString("n2"), ForaPrazo = false, Cor = colorFaturamento });
                retorno.Add(new { Descricao = "Receita " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data2.Month).ToUpper().Substring(0, 3) + " - " + data2.Year.ToString(), Valor = valorReceitasData5, DescricaoValor = "Receita: " + valorReceitasData5.ToString("n2"), ForaPrazo = false, Cor = colorReceita });
                retorno.Add(new { Descricao = "Recebido " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data2.Month).ToUpper().Substring(0, 3) + " - " + data2.Year.ToString(), Valor = valorRecebidoData5, DescricaoValor = "Recebido: " + valorRecebidoData5.ToString("n2"), ForaPrazo = false, Cor = colorRecebido });
                retorno.Add(new { Descricao = "Liquidado " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data2.Month).ToUpper().Substring(0, 3) + " - " + data2.Year.ToString(), Valor = valorLiquidadoData5, DescricaoValor = "Liquidado: " + valorLiquidadoData5.ToString("n2"), ForaPrazo = false, Cor = colorLiquidado });
                retorno.Add(new { Descricao = "Despesa " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data2.Month).ToUpper().Substring(0, 3) + " - " + data2.Year.ToString(), Valor = valorDespesaData5, DescricaoValor = "Despesas: " + valorDespesaData5.ToString("n2"), ForaPrazo = false, Cor = colorDespesa });
                retorno.Add(new { Descricao = "Pago " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data2.Month).ToUpper().Substring(0, 3) + " - " + data2.Year.ToString(), Valor = valorPagoData5, DescricaoValor = "Pago: " + valorPagoData5.ToString("n2"), ForaPrazo = false, Cor = colorPago });

                retorno.Add(new { Descricao = "Faturamento " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data1.Month).ToUpper().Substring(0, 3) + " - " + data1.Year.ToString(), Valor = valorFaturamentoData4, DescricaoValor = "Faturamento: " + valorFaturamentoData4.ToString("n2"), ForaPrazo = false, Cor = colorFaturamento });
                retorno.Add(new { Descricao = "Receita " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data1.Month).ToUpper().Substring(0, 3) + " - " + data1.Year.ToString(), Valor = valorReceitasData4, DescricaoValor = "Receita: " + valorReceitasData4.ToString("n2"), ForaPrazo = false, Cor = colorReceita });
                retorno.Add(new { Descricao = "Recebido " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data1.Month).ToUpper().Substring(0, 3) + " - " + data1.Year.ToString(), Valor = valorRecebidoData4, DescricaoValor = "Recebido: " + valorRecebidoData4.ToString("n2"), ForaPrazo = false, Cor = colorRecebido });
                retorno.Add(new { Descricao = "Liquidado " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data1.Month).ToUpper().Substring(0, 3) + " - " + data1.Year.ToString(), Valor = valorLiquidadoData4, DescricaoValor = "Liquidado: " + valorLiquidadoData4.ToString("n2"), ForaPrazo = false, Cor = colorLiquidado });
                retorno.Add(new { Descricao = "Despesa " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data1.Month).ToUpper().Substring(0, 3) + " - " + data1.Year.ToString(), Valor = valorDespesaData4, DescricaoValor = "Despesas: " + valorDespesaData4.ToString("n2"), ForaPrazo = false, Cor = colorDespesa });
                retorno.Add(new { Descricao = "Pago " + DateTimeFormatInfo.CurrentInfo.GetMonthName(data1.Month).ToUpper().Substring(0, 3) + " - " + data1.Year.ToString(), Valor = valorPagoData4, DescricaoValor = "Pago: " + valorPagoData4.ToString("n2"), ForaPrazo = false, Cor = colorPago });

                retorno.Add(new { Descricao = "Faturamento " + DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.Month).ToUpper().Substring(0, 3) + " - " + dataAtual.Year.ToString(), Valor = valorFaturamentoDataAtual, DescricaoValor = "Faturamento: " + valorFaturamentoDataAtual.ToString("n2"), ForaPrazo = false, Cor = colorFaturamento });
                retorno.Add(new { Descricao = "Receita " + DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.Month).ToUpper().Substring(0, 3) + " - " + dataAtual.Year.ToString(), Valor = valorReceitasDataAtual, DescricaoValor = "Receita: " + valorReceitasDataAtual.ToString("n2"), ForaPrazo = false, Cor = colorReceita });
                retorno.Add(new { Descricao = "Recebido " + DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.Month).ToUpper().Substring(0, 3) + " - " + dataAtual.Year.ToString(), Valor = valorRecebidoDataAtual, DescricaoValor = "Recebido: " + valorRecebidoDataAtual.ToString("n2"), ForaPrazo = false, Cor = colorRecebido });
                retorno.Add(new { Descricao = "Liquidado " + DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.Month).ToUpper().Substring(0, 3) + " - " + dataAtual.Year.ToString(), Valor = valorLiquidadoDataAtual, DescricaoValor = "Liquidado: " + valorLiquidadoDataAtual.ToString("n2"), ForaPrazo = false, Cor = colorLiquidado });
                retorno.Add(new { Descricao = "Despesa " + DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.Month).ToUpper().Substring(0, 3) + " - " + dataAtual.Year.ToString(), Valor = valorDespesaDataAtual, DescricaoValor = "Despesas: " + valorDespesaDataAtual.ToString("n2"), ForaPrazo = false, Cor = colorDespesa });
                retorno.Add(new { Descricao = "Pago " + DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.Month).ToUpper().Substring(0, 3) + " - " + dataAtual.Year.ToString(), Valor = valorPagoDataAtual, DescricaoValor = "Pago: " + valorPagoDataAtual.ToString("n2"), ForaPrazo = false, Cor = colorPago });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter os valores do gráfico.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ContasEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição / Pessoa", "Descricao", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoTipo")
                    propOrdenar = "TipoTitulo";
                else if (propOrdenar == "Valor")
                    propOrdenar = "ValorPendente";

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultaTitulosPendentes(this.Usuario.Empresa.Codigo, this.Usuario.Empresa.TipoAmbiente, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContaConsultaTitulosPendentes(this.Usuario.Empresa.Codigo, this.Usuario.Empresa.TipoAmbiente));

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 p.DescricaoTipo,
                                 DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                 Descricao = (p.Pessoa.CPF_CNPJ_SemFormato != this.Usuario.Empresa.CNPJ_SemFormato) || string.IsNullOrWhiteSpace(p.Observacao) ? p.Pessoa.Nome : p.Observacao,
                                 Valor = p.ValorPendente.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as atividades pendentes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VerificarCertificadosVencidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                List<Dominio.Entidades.Empresa> listaEmpresas = new List<Dominio.Entidades.Empresa>();

                if (Usuario != null && Empresa != null)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        if (Empresa.Codigo == 137)
                            listaEmpresas = repEmpresa.BuscarTodas("A");
                        else
                            listaEmpresas = repEmpresa.BuscarPorEmpresaPaiEStatus(Empresa.Codigo, "A");
                    }
                    else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        listaEmpresas = repEmpresa.BuscarPorCodigos(new List<int>() { Empresa.Codigo });
                    else
                        return new JsonpResult(false, true, "Tipo do serviço não habilitado para o controle de certificado.");
                }

                if (listaEmpresas?.Count > 0)
                {
                    var lista = from p in listaEmpresas
                                where p.DataFinalCertificado != null && p.DataFinalCertificado > DateTime.MinValue
                                && p.DataFinalCertificado <= DateTime.Now.AddDays(30)
                                select new
                                {
                                    CNPJ = p.CNPJ_Formatado,
                                    Nome = p.RazaoSocial,
                                    Fone = p.Telefone,
                                    DataVencimento = p.DataFinalCertificado.Value.ToString("dd/MM/yyyy")
                                };

                    return new JsonpResult(lista);
                }
                else
                    return new JsonpResult(false, true, "Não possui nenhum certificado a vencer.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os certificados a vencer.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VerificaPermissaoUsuario()
        {
            try
            {
                var dynRetorno = new
                {
                    VisualizarGraficosIniciais = Usuario?.VisualizarGraficosIniciais ?? false
                };
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as permissões do usuário;");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregaTermoDeUsoSistema()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (Empresa?.EmpresaPai == null)
                    return new JsonpResult(false, true, "Não possui termo de uso");

                Dominio.Entidades.Empresa empresa = Empresa;
                Dominio.Entidades.Empresa empresaPai = Empresa.EmpresaPai;

                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);
                Dominio.Entidades.EmpresaContrato contrato = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    contrato = repEmpresaContrato.BuscarPorEmpresaTMS(empresaPai.Codigo);
                else
                    contrato = repEmpresaContrato.BuscarPorTransportador(empresa.Codigo);

                if (contrato == null || string.IsNullOrWhiteSpace(contrato.Contrato))
                    return new JsonpResult(false, true, "Não possui termo de uso");

                string sContrato = contrato.Contrato;
                sContrato = sContrato.Replace("#TagRazaoSocialCliente", empresa.RazaoSocial);
                sContrato = sContrato.Replace("#TagCNPJCliente", empresa.CNPJ_Formatado);
                sContrato = sContrato.Replace("#TagEnderecoCliente", empresa.Endereco + " Nº " + empresa.Numero + " CEP: " + empresa.CEP);
                sContrato = sContrato.Replace("#TagComplementoCliente", empresa.Complemento);
                sContrato = sContrato.Replace("#TagBairroCliente", empresa.Bairro);
                sContrato = sContrato.Replace("#TagCidadeUFCliente", empresa.LocalidadeUF);

                sContrato = sContrato.Replace("#TagRazaoSocialAdmin", empresaPai.RazaoSocial);
                sContrato = sContrato.Replace("#TagCNPJAdmin", empresaPai.CNPJ_Formatado);
                sContrato = sContrato.Replace("#TagEnderecoAdmin", empresaPai.Endereco + " Nº " + empresaPai.Numero + " CEP: " + empresaPai.CEP);
                sContrato = sContrato.Replace("#TagComplementoAdmin", empresaPai.Complemento);
                sContrato = sContrato.Replace("#TagBairroAdmin", empresaPai.Bairro);
                sContrato = sContrato.Replace("#TagCidadeUFAdmin", empresaPai.LocalidadeUF);

                sContrato = sContrato.Replace("#TagResponsavelAdmin", empresaPai.Responsavel);
                sContrato = sContrato.Replace("#TagDataAtual", DateTime.Now.ToString("dd/MM/yyyy"));
                sContrato = sContrato.Replace("#TagDataCadastro", empresa.DataCadastro.Value.ToString("dd/MM/yyyy"));
                sContrato = sContrato.Replace("\n", "<br/>");

                bool necessarioNovoAceite = false;
                DateTime? dataAceiteTermosUso = empresa.DataAceiteTermosUso;
                if ((contrato.RecorrenciaEmDias > 0 && empresa.AceitouTermosUso && (!dataAceiteTermosUso.HasValue || (dataAceiteTermosUso.HasValue && dataAceiteTermosUso.Value.Date.AddDays(contrato.RecorrenciaEmDias) < DateTime.Now.Date)))
                    || (dataAceiteTermosUso.HasValue && contrato.DataUltimaAlteracao.HasValue && dataAceiteTermosUso.Value < contrato.DataUltimaAlteracao.Value))
                    necessarioNovoAceite = true;

                var dynRetorno = new
                {
                    TermoUso = sContrato,
                    empresa.AceitouTermosUso,
                    NecessarioNovoAceite = necessarioNovoAceite,
                    DataVigenciaContrato = contrato.DataUltimaAlteracao.HasValue ? $"a partir de {contrato.DataUltimaAlteracao.Value.ToString("dd/MM/yyyy")}" : string.Empty,
                    CodigoAnexo = contrato.Anexos?.Count > 0 ? contrato.Anexos[0].Codigo : 0,
                    Codigo = contrato.Codigo
                };
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o termo de uso do sistema.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregaDadosCertificadoDigital()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);

                var dynRetorno = new
                {
                    empresa.Codigo,
                    DataInicialCertificado = empresa.DataInicialCertificado != null ? empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : "",
                    DataFinalCertificado = empresa.DataFinalCertificado != null ? empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : "",
                    empresa.SerieCertificado,
                    empresa.SenhaCertificado,
                    PossuiCertificado = !string.IsNullOrWhiteSpace(empresa.NomeCertificado) ? Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) : false
                };
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o termo de uso do sistema.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AceitarTermoDeUsoSistema()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                if (Empresa?.EmpresaPai == null)
                    return new JsonpResult(false, true, "Não possui termo de uso");

                bool necessarioNovoAceite = Request.GetBoolParam("NecessarioNovoAceite");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);

                Dominio.Entidades.Empresa empresa = Empresa;

                if (empresa.AceitouTermosUso && !necessarioNovoAceite)
                    return new JsonpResult(true);

                string logAceite = "Termos de uso aceito por #usuario# (#login#) dia #data# às #hora#.";
                DateTime dataAceite = DateTime.Now;

                logAceite = logAceite
                    .Replace("#usuario#", this.Usuario.Nome)
                    .Replace("#login#", this.Usuario.Login)
                    .Replace("#data#", dataAceite.ToString("dd/MM/yyyy"))
                    .Replace("#hora#", dataAceite.ToString("HH:mm"));

                empresa.AceitouTermosUso = true;
                empresa.LogAceiteTermosUso = !string.IsNullOrWhiteSpace(empresa.LogAceiteTermosUso) ? empresa.LogAceiteTermosUso + " - " + logAceite : logAceite;
                empresa.DataAceiteTermosUso = dataAceite;
                empresa.EmpresaContrato = repEmpresaContrato.BuscarPorCodigo(Request.GetIntParam("Codigo"));

                repEmpresa.Atualizar(empresa);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao aceitar o termo de uso do sistema.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = Request.GetIntParam("CodigoAnexo");

                Repositorio.EmpresaContratoAnexo repEmpresaContratoAnexo = new Repositorio.EmpresaContratoAnexo(unitOfWork);
                Dominio.Entidades.EmpresaContratoAnexo empresaContratoAnexo = repEmpresaContratoAnexo.BuscarPorCodigo(codigoAnexo, false);

                if (empresaContratoAnexo == null)
                    return new JsonpResult(false, true, "Anexo do Contrato não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(empresaContratoAnexo.CaminhoArquivo))
                    return new JsonpResult(false, true, "Anexo do Contrato não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(empresaContratoAnexo.CaminhoArquivo), "application/pdf", empresaContratoAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo do Contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion

        #region Métodos Privados

        private string ObterCaminhoArquivosFornecedor(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas).Name });
        }

        private string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.MensagemAviso).Name });
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas ObterModalidadeFornecedorDoUsuario(Repositorio.UnitOfWork unitOfWork)
        {
            if (this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                return null;

            if (this.Usuario.ClienteFornecedor == null)
                return null;

            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, this.Usuario.ClienteFornecedor.CPF_CNPJ);

            if (modalidadePessoas == null)
                return null;

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            return modalidadeFornecedorPessoas;
        }

        #endregion
    }
}
