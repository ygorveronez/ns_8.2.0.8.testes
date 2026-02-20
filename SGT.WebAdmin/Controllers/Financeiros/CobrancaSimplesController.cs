using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    public class CobrancaSimplesController : BaseController
    {
		#region Construtores

		public CobrancaSimplesController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Financeiros/CobrancaSimples")]

        public async Task<IActionResult> SalvarTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = 0, codigoConfiguracaoBanco = 0, codigoTipoMovimento = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("ConfiguracaoBanco"), out codigoConfiguracaoBanco);
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);

                double pessoa;
                double.TryParse(Request.Params("Pessoa"), out pessoa);

                decimal valorTitulo;
                decimal.TryParse(Request.Params("ValorTitulo"), out valorTitulo);

                DateTime dataVencimento;
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);

                string observacao = Request.Params("Observacao");

                FormaTitulo formaTitulo;
                Enum.TryParse(Request.Params("FormaTitulo"), out formaTitulo);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo;
                if (codigo > 0)
                {
                    titulo = repTitulo.BuscarPorCodigo(codigo);
                }
                else
                {
                    titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                    titulo.Acrescimo = 0;
                    titulo.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoConfiguracaoBanco);
                    if (titulo.BoletoConfiguracao != null)
                        titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                    titulo.DataAutorizacao = null;
                    titulo.DataCancelamento = null;
                    titulo.DataEmissao = DateTime.Now;
                    titulo.DataLiquidacao = null;
                    titulo.DataBaseLiquidacao = null;
                    titulo.DataVencimento = dataVencimento;
                    titulo.DataProgramacaoPagamento = dataVencimento;
                    titulo.Desconto = 0;
                    titulo.Empresa = this.Usuario.Empresa;
                    titulo.Historico = "TÍTULO GERADO DA COBRANÇA SIMPLES";
                    titulo.Observacao = observacao;
                    titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                    titulo.Sequencia = 1;
                    titulo.StatusTitulo = StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoMovimento = null;
                    titulo.TipoTitulo = TipoTitulo.Receber;
                    titulo.ValorOriginal = valorTitulo;
                    titulo.ValorPago = 0;
                    titulo.NotaFiscal = null;
                    titulo.CTeParcela = null;
                    titulo.ValorPendente = valorTitulo;
                    titulo.ValorTituloOriginal = valorTitulo;
                    titulo.FormaTitulo = formaTitulo;
                    titulo.Usuario = this.Usuario;
                    titulo.DataLancamento = DateTime.Now;
                    if (codigoTipoMovimento > 0)
                        titulo.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                 
                    if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                    {
                        Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                        servTitulo.IntegrarEmitido(titulo, unitOfWork);
                    }

                    repTitulo.Inserir(titulo, Auditado);

                    if (titulo.TipoMovimento != null)
                        servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "TÍTULO " + titulo.Sequencia.ToString() + " - Gerado de Cobrança Simples ", unitOfWork, TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, null, null, titulo.Codigo);
                }

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    Codigo = titulo.Codigo
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha na geração do título.");
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
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                List<int> listaCodigos = new List<int>();
                listaCodigos.Add(codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorListaCodigo(listaCodigos);

                var dynRetorno = new
                {
                    ListaTitulos = (from p in listaTitulos
                                    select new
                                    {
                                        p.Codigo,
                                        p.BoletoStatusTitulo,
                                        Pessoa = p.Pessoa.Nome,
                                        DescricaoStatusBoleto = p.BoletoStatusTitulo.ObterDescricao(),
                                        DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                        DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                        Valor = p.ValorOriginal.ToString("n2"),
                                        p.NossoNumero,
                                        p.CaminhoBoleto
                                    }).ToList()
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar título.");
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
                unitOfWork.Start();

                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa.Codigo);

                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                string mensagemDigitada = Request.Params("MensagemEmail");

                string assunto = "Boleto " + empresa.NomeFantasia;
                string mensagemEmail = "Olá,<br/><br/>Segue em anexo o boleto da empresa " + empresa.NomeFantasia + ".<br/><br/>";
                if (!string.IsNullOrWhiteSpace(mensagemDigitada))
                    mensagemEmail += mensagemDigitada + "<br/><br/>";
                mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

				if (codigo == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigo);

				string urlBase = _conexao.ObterHost;
				string portalClienteCodigo = Servicos.Embarcador.Financeiro.Titulo.ObterPortalClienteCodigo(titulo, repTitulo);
				string portalClienteUrl = Servicos.Embarcador.Financeiro.Titulo.ObterURLPortalClienteCodigo(urlBase, portalClienteCodigo);
				mensagemEmail += "<br/><br/>Link de acesso aos dados da compra: " + portalClienteUrl;

				if (!string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                {
                    List<string> emails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(titulo.Pessoa.Email))
                        emails.AddRange(titulo.Pessoa.Email.Split(';').ToList());

                    for (int a = 0; a < titulo.Pessoa.Emails.Count; a++)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = titulo.Pessoa.Emails[a];
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                            emails.Add(outroEmail.Email);
                    }

                    if (!string.IsNullOrWhiteSpace(empresa.Email) && empresa.StatusEmail == "A")
                        emails.AddRange(empresa.Email.Split(';').ToList());

                    if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) && empresa.StatusEmailAdministrativo == "A")
                        emails.AddRange(empresa.EmailAdministrativo.Split(';').ToList());

                    emails = emails.Distinct().ToList();
                    if (emails.Count > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Enviou Boleto por E-mail.", unitOfWork);
                        byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto);
                        bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdf), System.IO.Path.GetFileName(titulo.CaminhoBoleto), "application/pdf") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, empresa.Codigo);
                        if (!sucesso)
                            return new JsonpResult(false, "Problemas ao enviar o boleto por e-mail: " + mensagemErro);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha com o envio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarConfiguracaoBanco()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoleto = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                List<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao> configBoleto = repBoleto.BuscarPorEmpresa(codigoEmpresa);

                object retorno = null;
                if (configBoleto.Count == 1)
                {
                    retorno = new
                    {
                        Codigo = configBoleto[0].Codigo,
                        Nome = configBoleto[0].DescricaoBanco
                    };
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a configuração do banco.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
