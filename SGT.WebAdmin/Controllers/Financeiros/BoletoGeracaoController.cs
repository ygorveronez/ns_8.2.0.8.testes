using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao;
using AdminMultisoftware.Dominio.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BoletoGeracao")]
    public class BoletoGeracaoController : BaseController
    {
        #region Construtores

        public BoletoGeracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTitulosParaGerarBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Título", "Codigo", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("BoletoStatusTitulo", false);
                grid.AdicionarCabecalho("CodigoRemessa", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status Boleto", "DescricaoStatusBoleto", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Nosso Número", "NossoNumero", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número Remessa", "NumeroRemessa", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Configuração Boleto", "BoletoConfiguracao", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CaminhoBoleto", false);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarTituloGeracaoBoleto(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsultarTituloGeracaoBoleto(filtrosPesquisa));

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 CodigoRemessa = p.BoletoRemessa?.Codigo ?? 0,
                                 p.BoletoStatusTitulo,
                                 Pessoa = p.Pessoa?.Nome ?? "",
                                 DescricaoStatusBoleto = p.BoletoStatusTitulo.ObterDescricao(),
                                 DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                 Valor = p.ValorOriginal.ToString("n2"),
                                 p.NossoNumero,
                                 NumeroRemessa = p.BoletoRemessa?.NumeroSequencial.ToString("n0") ?? string.Empty,
                                 BoletoConfiguracao = p.BoletoConfiguracao?.Descricao ?? "",
                                 p.CaminhoBoleto
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTitulosParaGerarBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarTituloGeracaoBoleto(filtrosPesquisa, "", "", 0, 0);

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 CodigoRemessa = p.BoletoRemessa?.Codigo ?? 0,
                                 p.BoletoStatusTitulo,
                                 Pessoa = p.Pessoa.Nome,
                                 DescricaoStatusBoleto = p.BoletoStatusTitulo.ObterDescricao(),
                                 DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                 Valor = p.ValorOriginal.ToString("n2"),
                                 p.NossoNumero,
                                 NumeroRemessa = p.BoletoRemessa?.NumeroSequencial.ToString("n0") ?? string.Empty,
                                 p.CaminhoBoleto
                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);

                int tipoAtualizacao = int.Parse(Request.Params("TipoAtualizacao"));
                int codigoConfiguracaoBoleto = 0;
                int.TryParse(Request.Params("CodigoConfiguracaoBoleto"), out codigoConfiguracaoBoleto);
                //tipoAtualizacao = 1 = Atualizar
                //tipoAtualizacao = 2 = Gerar Boletos
                //tipoAtualizacao = 3 = Gerar Remessa                

                List<int> listaCodigos = new List<int>();
                listaCodigos = RetornaCodigosTitulos(unitOfWork);
                if (listaCodigos.Count() == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");

                if (tipoAtualizacao == 2)
                {
                    if (codigoConfiguracaoBoleto == 0)
                        return new JsonpResult(false, "Favor informe o banco para a geração dos boletos.");

                    for (int i = 0; i < listaCodigos.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);
                        if (string.IsNullOrWhiteSpace(titulo.NossoNumero))
                        {
                            titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                            titulo.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoConfiguracaoBoleto);
                            titulo.UltimoRetornoGeracaoBoleto = "";
                            titulo.CaminhoBoleto = "";
                            repTitulo.Atualizar(titulo);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Gerou Boleto.", unitOfWork);
                          
                            Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                            servTitulo.IntegrarEmitido(titulo, unitOfWork);
                        }
                    }
                }
                else if (tipoAtualizacao == 3)
                {
                    bool gerarRemessa = false;
                    int codigoConfiguracaoBoletoRemessa = 0;
                    for (int i = 0; i < listaCodigos.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);

                        if (!string.IsNullOrWhiteSpace(titulo.NossoNumero) && titulo.BoletoConfiguracao != null && titulo.BoletoRemessa == null && titulo.BoletoStatusTitulo == BoletoStatusTitulo.Gerado)
                        {
                            codigoConfiguracaoBoletoRemessa = titulo.BoletoConfiguracao.Codigo;
                            gerarRemessa = true;
                            break;
                        }
                    }

                    if (gerarRemessa && codigoConfiguracaoBoletoRemessa > 0)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa remessa = new Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa();
                        remessa.DataGeracao = DateTime.Now;
                        remessa.Empresa = this.Usuario.Empresa;
                        remessa.NumeroSequencial = repBoletoRemessa.BuscarProximaNumereracao(codigoConfiguracaoBoletoRemessa);
                        remessa.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoConfiguracaoBoletoRemessa);

                        repBoletoRemessa.Inserir(remessa, Auditado);

                        for (int i = 0; i < listaCodigos.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);

                            if (!string.IsNullOrWhiteSpace(titulo.NossoNumero) && titulo.BoletoRemessa == null && titulo.BoletoConfiguracao.Codigo == codigoConfiguracaoBoletoRemessa)
                            {
                                titulo.BoletoStatusTitulo = BoletoStatusTitulo.AguardandoRemessa;
                                titulo.BoletoRemessa = remessa;

                                repTitulo.Atualizar(titulo);

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Gerou Remessa.", unitOfWork);
                            }
                        }
                    }
                }


                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorListaCodigo(listaCodigos);

                var dynRetorno = new
                {
                    ListaTitulos = (from p in listaTitulos
                                    select new
                                    {
                                        p.Codigo,
                                        CodigoRemessa = p.BoletoRemessa?.Codigo ?? 0,
                                        p.BoletoStatusTitulo,
                                        Pessoa = p.Pessoa.Nome,
                                        DescricaoStatusBoleto = p.BoletoStatusTitulo.ObterDescricao(),
                                        DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                        DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                        Valor = p.ValorOriginal.ToString("n2"),
                                        p.NossoNumero,
                                        NumeroRemessa = p.BoletoRemessa?.NumeroSequencial.ToString("n0") ?? string.Empty,
                                        p.CaminhoBoleto
                                    }).ToList()
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> EnviarEmailBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Dominio.Entidades.Empresa empresa = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario?.Empresa ?? null;

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Servicos.Embarcador.Financeiro.BaixaTituloPagar svcBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa?.Codigo ?? 0);

                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");

                string mensagemErro = "Erro ao enviar e-mail";
                string mensagemEmailOriginal = Request.GetStringParam("MensagemEmail");

                List<int> listaCodigos = new List<int>();
                listaCodigos = RetornaCodigosTitulosRemessa(unitOfWork);
                if (listaCodigos.Count() == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");

                string urlBase = _conexao.ObterHost;

                for (int i = 0; i < listaCodigos.Count; i++)
                {
                    string assunto = string.Empty;
                    string mensagemEmail = mensagemEmailOriginal;
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);

                    Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(titulo.BoletoConfiguracao.Codigo);
                    if (!string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                    {
                        if (!string.IsNullOrEmpty(boletoConfiguracao.AssuntoEmail))
                        {
                            assunto = boletoConfiguracao.AssuntoEmail;
                            assunto = assunto.Replace("#Empresa", (boletoConfiguracao.Empresa?.NomeFantasia ?? ""));
                            assunto = assunto.Replace("#Cliente", titulo.Pessoa?.Nome);
                            assunto = assunto.Replace("#ValorTitulo", titulo.ValorOriginal.ToString("n2"));
                            assunto = assunto.Replace("#DataVencimento", titulo.DataVencimento.Value.ToString("dd/MM/yyyy"));
                            assunto = assunto.Replace("#qLinha#", "<br/>");
                        }
                        else
                            assunto = "Boleto " + (empresa?.NomeFantasia ?? "");

                        if (!string.IsNullOrEmpty(boletoConfiguracao.MensagemEmail))
                        {
                            string numeroCTe = titulo.NumeroDocumentos;
                            if (string.IsNullOrWhiteSpace(numeroCTe))
                                numeroCTe = titulo.NumeroConhecimentos;
                            if (string.IsNullOrWhiteSpace(numeroCTe) && titulo.ConhecimentoDeTransporteEletronico != null)
                                numeroCTe = titulo.ConhecimentoDeTransporteEletronico.Numero.ToString("n0");

                            int numeroFatura = titulo.FaturaCargaDocumento?.Fatura?.Numero ?? titulo.FaturaDocumento?.Fatura?.Numero ?? titulo.FaturaParcela?.Fatura?.Numero ?? 0;

                            mensagemEmail += boletoConfiguracao.MensagemEmail;
                            mensagemEmail = mensagemEmail.Replace("#TagRazaoSocialAdmin", (boletoConfiguracao.Empresa?.NomeFantasia ?? ""));
                            mensagemEmail = mensagemEmail.Replace("#TagCNPJAdmin", (boletoConfiguracao.Empresa?.CNPJ_Formatado ?? ""));
                            mensagemEmail = mensagemEmail.Replace("#TagRazaoSocialCliente", titulo.Pessoa?.Nome ?? "");
                            mensagemEmail = mensagemEmail.Replace("#TagCNPJCliente", titulo.Pessoa?.CPF_CNPJ_Formatado ?? "");

                            mensagemEmail = mensagemEmail.Replace("#Empresa", (boletoConfiguracao.Empresa?.NomeFantasia ?? ""));
                            mensagemEmail = mensagemEmail.Replace("#Cliente", titulo.Pessoa?.Nome);
                            mensagemEmail = mensagemEmail.Replace("#ValorTitulo", titulo.ValorOriginal.ToString("n2"));
                            mensagemEmail = mensagemEmail.Replace("#DataVencimento", titulo.DataVencimento.Value.ToString("dd/MM/yyyy"));
                            mensagemEmail = mensagemEmail.Replace("#NumeroCTe", numeroCTe);
                            mensagemEmail = mensagemEmail.Replace("#NumeroFatura", numeroFatura.ToString());
                            mensagemEmail = mensagemEmail.Replace("#qLinha#", "<br/>");
                        }
                        else if (string.IsNullOrWhiteSpace(mensagemEmail))
                        {
                            mensagemEmail = "Olá,<br/><br/>Seguem em anexo o boleto da empresa " + (empresa?.NomeFantasia ?? "") + ".<br/><br/>";
                            mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                        }

                        string portalClienteCodigo = Servicos.Embarcador.Financeiro.Titulo.ObterPortalClienteCodigo(titulo, repTitulo);
                        string portalClienteUrl = Servicos.Embarcador.Financeiro.Titulo.ObterURLPortalClienteCodigo(urlBase, portalClienteCodigo);
						mensagemEmail += $"<br/><br/>Link de acesso aos dados da compra: <a href=\"{portalClienteUrl}\" title=\"Dados da compra\">{portalClienteUrl}</a>";

						List<string> emails = new List<string>();
                        if (!ConfiguracaoEmbarcador.EnviarBoletoApenasParaEmailSecundario)
                        {
                            if (!string.IsNullOrWhiteSpace(titulo.Pessoa.Email))
                                emails.AddRange(titulo.Pessoa.Email.Split(';').ToList());
                        }

                        for (int a = 0; a < titulo.Pessoa.Emails.Count; a++)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = titulo.Pessoa.Emails[a];
                            if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A"
                                && (outroEmail.TipoEmail != TipoEmail.Administrativo || ConfiguracaoEmbarcador.EnviarBoletoApenasParaEmailSecundario))
                                emails.Add(outroEmail.Email);
                        }

                        if (empresa != null)
                        {
                            if (!string.IsNullOrWhiteSpace(empresa.Email) && empresa.StatusEmail == "A")
                                emails.AddRange(empresa.Email.Split(';').ToList());

                            if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) && empresa.StatusEmailAdministrativo == "A")
                                emails.AddRange(empresa.EmailAdministrativo.Split(';').ToList());
                        }

                        emails = emails.Distinct().ToList();
                        if (emails.Count > 0)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Enviou Boleto para E-mail.", unitOfWork);

                            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                            attachments.Add(new System.Net.Mail.Attachment(titulo.CaminhoBoleto));

                            //Adiciona também as DACTEs
                            if (titulo.TituloBaixaNegociacao != null)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPai = svcBaixaTituloPagar.RetornaTituloPai(titulo);
                                if (tituloPai != null)
                                {
                                    titulo = tituloPai;
                                    while (tituloPai != null)
                                    {
                                        tituloPai = svcBaixaTituloPagar.RetornaTituloPai(tituloPai);
                                        if (tituloPai != null)
                                            titulo = tituloPai;
                                    }
                                }
                            }
                            if (titulo.ConhecimentoDeTransporteEletronico != null)
                            {
                                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, titulo.ConhecimentoDeTransporteEletronico.Empresa.CNPJ, titulo.ConhecimentoDeTransporteEletronico.Chave) + ".pdf";
                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                    attachments.Add(new System.Net.Mail.Attachment(caminhoPDF));
                                else
                                {
                                    string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;
                                    string nomeArquivoFisico = "";

                                    if (titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || (titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
                                        nomeArquivoFisico = titulo.ConhecimentoDeTransporteEletronico.Numero.ToString() + "_" + titulo.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString();

                                    if (ConfiguracaoEmbarcador.GerarPDFCTeCancelado && titulo.ConhecimentoDeTransporteEletronico.Status == "C" && titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                        nomeArquivoFisico = nomeArquivoFisico + "_Canc";

                                    if (titulo.ConhecimentoDeTransporteEletronico.Status == "F")
                                        nomeArquivoFisico = nomeArquivoFisico + "_FSDA";

                                    caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, titulo.ConhecimentoDeTransporteEletronico.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";
                                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                        attachments.Add(new System.Net.Mail.Attachment(caminhoPDF));
                                    else if (titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                                    {
                                        byte[] pdf = null;
                                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                                        if (titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                                            pdf = svcNFSe.ObterDANFSECTe(titulo.ConhecimentoDeTransporteEletronico.Codigo, null, true);
                                        else
                                            pdf = svcNFSe.ObterDANFSECTe(titulo.ConhecimentoDeTransporteEletronico.Codigo);
                                        if (pdf != null)
                                        {
                                            Stream stream = new MemoryStream(pdf);
                                            attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivoFisico + ".pdf"));
                                        }
                                    }
                                }
                                string nomeArquivo = "";
                                byte[] data = null;
                                if (titulo.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Numero == "39")
                                {
                                    nomeArquivo = titulo.ConhecimentoDeTransporteEletronico.Numero.ToString() + "_" + titulo.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString() + ".xml";
                                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                                    data = svcNFSe.ObterXMLAutorizacaoCTe(titulo.ConhecimentoDeTransporteEletronico.Codigo, unitOfWork);
                                    if (data != null)
                                    {
                                        Stream stream = new MemoryStream(data);
                                        attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                                    }
                                }
                                else
                                {
                                    nomeArquivo = string.Concat(titulo.ConhecimentoDeTransporteEletronico.Chave, ".xml");
                                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                                    data = svcCTe.ObterXMLAutorizacao(titulo.ConhecimentoDeTransporteEletronico, unitOfWork);
                                    if (data != null)
                                    {
                                        Stream stream = new MemoryStream(data);
                                        attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                                    }
                                }
                            }
                            else if (titulo.Documentos != null && titulo.Documentos.Count > 0)
                            {
                                string nomeArquivo = "";
                                byte[] data = null;
                                foreach (var documento in titulo.Documentos)
                                {
                                    if (documento.CTe != null)
                                    {
                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documento.CTe.Codigo);
                                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";
                                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                            attachments.Add(new System.Net.Mail.Attachment(caminhoPDF));
                                        else
                                        {
                                            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;
                                            string nomeArquivoFisico = "";

                                            if (documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || (documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
                                                nomeArquivoFisico = documento.CTe.Numero.ToString() + "_" + documento.CTe.Serie.Numero.ToString();

                                            if (ConfiguracaoEmbarcador.GerarPDFCTeCancelado && documento.CTe.Status == "C" && documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                                nomeArquivoFisico = nomeArquivoFisico + "_Canc";

                                            if (documento.CTe.Status == "F")
                                                nomeArquivoFisico = nomeArquivoFisico + "_FSDA";

                                            caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, documento.CTe.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";
                                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                                attachments.Add(new System.Net.Mail.Attachment(caminhoPDF));
                                            else if (documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                                            {
                                                byte[] pdf = null;
                                                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                                                if (documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                                                    pdf = svcNFSe.ObterDANFSECTe(documento.CTe.Codigo, null, true);
                                                else
                                                    pdf = svcNFSe.ObterDANFSECTe(documento.CTe.Codigo);
                                                if (pdf != null)
                                                {
                                                    Stream stream = new MemoryStream(pdf);
                                                    attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivoFisico + ".pdf"));
                                                }
                                            }
                                        }

                                        nomeArquivo = "";
                                        data = null;
                                        if (cte.ModeloDocumentoFiscal.Numero == "39")
                                        {
                                            nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                                            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                                            data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, unitOfWork);
                                            if (data != null)
                                            {
                                                Stream stream = new MemoryStream(data);
                                                attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                                            }
                                        }
                                        else
                                        {
                                            nomeArquivo = string.Concat(cte.Chave, ".xml");
                                            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                                            data = svcCTe.ObterXMLAutorizacao(cte, unitOfWork);
                                            if (data != null)
                                            {
                                                Stream stream = new MemoryStream(data);
                                                attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                                            }
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                                mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                            bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                                attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, (empresa?.Codigo ?? 0));
                            if (!sucesso)
                                return new JsonpResult(false, "Problemas ao enviar o boleto por e-mail: " + mensagemErro);
                        }
                    }
                }

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha na montagem do e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTitulosParaEnvioEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> listaCodigos = new List<int>();
                listaCodigos = RetornaCodigosTitulosRemessa(unitOfWork);
                if (listaCodigos.Count() == 0)
                    listaCodigos.Add(-1);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Título", "Codigo", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("BoletoStatusTitulo", false);
                grid.AdicionarCabecalho("CodigoRemessa", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status Boleto", "DescricaoStatusBoleto", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Nosso Número", "NossoNumero", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número Remessa", "NumeroRemessa", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CaminhoBoleto", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarTituloGeracaoBoleto(listaCodigos, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsultarTituloGeracaoBoleto(listaCodigos));

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 CodigoRemessa = p.BoletoRemessa?.Codigo ?? 0,
                                 p.BoletoStatusTitulo,
                                 Pessoa = p.Pessoa.Nome,
                                 DescricaoStatusBoleto = p.BoletoStatusTitulo.ObterDescricao(),
                                 DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                 Valor = p.ValorOriginal.ToString("n2"),
                                 p.NossoNumero,
                                 NumeroRemessa = p.BoletoRemessa?.NumeroSequencial.ToString("n0") ?? string.Empty,
                                 p.CaminhoBoleto
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);

                List<int> listaRemessa = new List<int>();
                listaRemessa = RetornaCodigosRemessa(unitOfWork);
                if (listaRemessa.Count() == 0)
                    return new JsonpResult(false, "Nenhuma remessa encontrada.");

                if (listaRemessa.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum arquivo de remessa encontrada nos títulos.");
                else if (listaRemessa.Count() == 1)
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa = repBoletoRemessa.BuscarPorCodigo(listaRemessa[0]);
                    if (boletoRemessa != null && !string.IsNullOrWhiteSpace(boletoRemessa.Observacao))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(boletoRemessa.Observacao))
                            {
                                boletoRemessa.DownloadRealizado = true;
                                repBoletoRemessa.Atualizar(boletoRemessa);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoRemessa, null, "Realizou o download da Remessa pela Geração de Boleto.", unitOfWork);
                                unitOfWork.CommitChanges();

                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(boletoRemessa.Observacao), "application/x-pkcs12", System.IO.Path.GetFileName(boletoRemessa.Observacao));
                            }
                            else
                                return new JsonpResult(false, "O arquivo da remessa " + boletoRemessa.Observacao + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do txt do remessa.");
                        }
                    }
                    else
                        return new JsonpResult(false, true, "Este boleto não possui o txt disponível para download.");
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa> remessas = repBoletoRemessa.BuscarPorNotas(listaRemessa);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa boletoRemessa in remessas)
                    {
                        boletoRemessa.DownloadRealizado = true;
                        repBoletoRemessa.Atualizar(boletoRemessa);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, boletoRemessa, null, "Realizou o download da Remessa por Lote pela Geração de Boleto.", unitOfWork);
                        unitOfWork.CommitChanges();
                    }

                    Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                    return Arquivo(svcNFe.ObterLoteDeRemessa(listaRemessa, unitOfWork), "application/zip", "LoteRemessas.zip");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadFrancesinha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<int> listaCodigos = new List<int>();
                listaCodigos = RetornaCodigosTitulos(unitOfWork);
                if (listaCodigos.Count() == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");
                string nomeEmpresa = Cliente.NomeFantasia;
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R124_Francesinha, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R124_Francesinha, TipoServicoMultisoftware, "Relatorio de Francesinha", "Financeiros", "Francesinha.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha> dadosFrancesinha = repTitulo.RelatorioFrancesinha(listaCodigos);
                if (dadosFrancesinha.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioFrancesinha(nomeEmpresa, stringConexao, relatorioControleGeracao, dadosFrancesinha));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de titúlos para gerar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private List<int> RetornaCodigosRemessa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTitulos")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTitulos"));
                if (listaTitulo != null)
                {
                    foreach (var titulo in listaTitulo)
                    {
                        listaCodigos.Add(int.Parse((string)titulo.Codigo));
                    }
                }
            }
            return listaCodigos;
        }

        private List<int> RetornaCodigosTitulos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTitulos")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTitulos"));
                if (listaTitulo != null)
                {
                    foreach (var titulo in listaTitulo)
                    {
                        listaCodigos.Add(int.Parse((string)titulo.Titulo.Codigo));
                    }
                }
            }
            return listaCodigos;
        }

        private List<int> RetornaCodigosTitulosRemessa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTitulos")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTitulos"));
                if (listaTitulo != null)
                {
                    foreach (var titulo in listaTitulo)
                    {
                        listaCodigos.Add(int.Parse((string)titulo.Codigo));
                    }
                }
            }
            return listaCodigos;
        }

        private void GerarRelatorioFrancesinha(string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha> dadosFrancesinha)
        {
            ReportRequest.WithType(ReportType.Francesinha)
                .WithExecutionType(ExecutionType.Async)
                .AddExtraData("NomeEmpresa", nomeEmpresa)
                .AddExtraData("DadosFrancesinhaDs", dadosFrancesinha.ToJson())
                .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                .CallReport();
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao()
            {
                SomentePendentes = Request.GetBoolParam("SomentePendentes"),
                SomenteSemRemessa = Request.GetBoolParam("SomenteSemRemessa"),
                CodigoFatura = Request.GetIntParam("Fatura"),
                CodigoOperadorFatura = Request.GetIntParam("OperadorFatura"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                CodigoConhecimento = Request.GetIntParam("Conhecimento"),
                CodigoRemessa = Request.GetIntParam("Remessa"),
                FormaTitulo = Request.GetEnumParam<FormaTitulo>("FormaTitulo"),
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.Enumeradores.TipoAmbiente.Nenhum : this.Usuario.Empresa.TipoAmbiente,
                CodigosEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? new List<int> { Usuario.Empresa.Codigo } : Request.GetListParam<int>("EmpresaFilial"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                NumeroControleCliente = Request.GetStringParam("NumeroControleCliente"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                TipoProposta = Request.GetEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                CodigoConfiguracaoBoleto = Request.GetIntParam("ConfiguracaoBoleto")
            };
        }

        

        #endregion
    }
}
