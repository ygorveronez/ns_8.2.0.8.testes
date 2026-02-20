using AdminMultisoftware.Repositorio;
using Servicos.Embarcador.Integracao;
using SGT.BackgroundWorkers.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 300000)]

    public class EnvioEmailBoleto : LongRunningProcessBase<EnvioEmailBoleto>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificaBoletosPendenteEnvio(unitOfWork);
        }

        private void VerificaBoletosPendenteEnvio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.BoletoPendenteEnvioEmail> boletosPendentesEnvio = repTitulo.BuscarBoletosPendenteEnvio();

            if (boletosPendentesEnvio != null && boletosPendentesEnvio.Count > 0)
            {
                foreach (var boletoPendente in boletosPendentesEnvio)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(boletoPendente.Codigo);

                    if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                        continue;

                    bool enviouEmails = EnviarEmailBoletoPendente(titulo, unitOfWork);
                    if (enviouEmails)
                    {
                        unitOfWork.Start();

                        titulo.BoletoEnviadoPorEmail = true;

                        repTitulo.Atualizar(titulo);

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, null, "Enviou documentação por e-mail automaticamente.", unitOfWork);

                        unitOfWork.CommitChanges();
                    }

                    unitOfWork.FlushAndClear();
                }
            }
        }

        private bool EnviarEmailBoletoPendente(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

            List<Dominio.Entidades.LayoutEDI> edis = null;

            if (email == null || titulo == null)
                return false;

            List<string> emails = new List<string>();

            string assuntoEmail = "";
            string corpoEmail = "";

            if (titulo.ContratoFrete?.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                assuntoEmail = titulo.ContratoFrete?.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.AssuntoEmailFatura ?? "";
                corpoEmail = titulo.ContratoFrete?.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.CorpoEmailFatura ?? "";
                //edis = titulo.Pessoa?.LayoutsEDI?.Where(c => c.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)?.Select(c => c.LayoutEDI)?.ToList() ?? null;
            }
            else if (titulo.Pessoa.NaoUsarConfiguracaoFaturaGrupo)
            {
                assuntoEmail = titulo.Pessoa.AssuntoEmailFatura;
                corpoEmail = titulo.Pessoa.CorpoEmailFatura;
                edis = titulo.Pessoa?.LayoutsEDI?.Where(c => c.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)?.Select(c => c.LayoutEDI)?.ToList() ?? null;
            }
            else if (titulo.Pessoa.GrupoPessoas != null)
            {
                assuntoEmail = titulo.Pessoa.GrupoPessoas.AssuntoEmailFatura;
                corpoEmail = titulo.Pessoa.GrupoPessoas.CorpoEmailFatura;
                edis = titulo.Pessoa?.GrupoPessoas?.LayoutsEDI?.Where(c => c.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)?.Select(c => c.LayoutEDI)?.ToList() ?? null;
            }

            if (titulo.Pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(titulo.Pessoa.GrupoPessoas.EmailFatura))
                emails.AddRange(titulo.Pessoa.GrupoPessoas.EmailFatura.Split(';').ToList());

            if (titulo.Pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(titulo.Pessoa.GrupoPessoas.Email))
                emails.AddRange(titulo.Pessoa.GrupoPessoas.Email.Split(';').ToList());

            if (titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(titulo.Pessoa.GrupoPessoas.EmailFatura))
                emails.AddRange(titulo.Pessoa.GrupoPessoas.EmailFatura.Split(';').ToList());

            if (titulo.Pessoa != null && !string.IsNullOrWhiteSpace(titulo.Pessoa.Email))
                emails.AddRange(titulo.Pessoa.Email.Split(';').ToList());

            if (string.IsNullOrWhiteSpace(assuntoEmail))
                assuntoEmail = "Envio de Documentação de Cobrança " + (titulo.Empresa?.Descricao ?? "");
            if (string.IsNullOrWhiteSpace(corpoEmail))
                corpoEmail = "Segue em anexo o documentação de cobrança";

            if (!string.IsNullOrWhiteSpace(assuntoEmail))
            {
                assuntoEmail = assuntoEmail.Replace("#NumeroFatura", Utilidades.String.OnlyNumbers(titulo.FaturaParcela?.Fatura?.Numero.ToString() ?? ""));
                assuntoEmail = assuntoEmail.Replace("#ObservacaoFatura", titulo.FaturaParcela?.Fatura?.Observacao ?? "");
                assuntoEmail = assuntoEmail.Replace("#DataFatura", titulo.FaturaParcela?.Fatura?.DataFatura.ToString("dd/MM/yyyy") ?? "");
                assuntoEmail = assuntoEmail.Replace("#CNPJTomador", titulo.Pessoa?.CPF_CNPJ_Formatado);
                assuntoEmail = assuntoEmail.Replace("#NomeTomador", titulo.Pessoa?.Nome);
                assuntoEmail = assuntoEmail.Replace("#CNPJEmpresa", titulo.Empresa?.CNPJ_Formatado);
                assuntoEmail = assuntoEmail.Replace("#NomeEmpresa", titulo.Empresa?.RazaoSocial);
                assuntoEmail = assuntoEmail.Replace("#NumeroCTe", string.Join(", ", titulo.Documentos.Where(f => f.CTe != null).Select(f => f.CTe.Numero).Distinct().ToList()));
                assuntoEmail = assuntoEmail.Replace("#DatasVencimentosBoletos", titulo.FaturaParcela?.DataVencimento.ToString("dd/MM/yyyy") ?? "");
            }
            if (!string.IsNullOrWhiteSpace(corpoEmail))
            {
                corpoEmail = corpoEmail.Replace("#QuebraLinha", "<br/>");
                corpoEmail = corpoEmail.Replace("#NumeroFatura", Utilidades.String.OnlyNumbers(titulo.FaturaParcela?.Fatura?.Numero.ToString() ?? ""));
                corpoEmail = corpoEmail.Replace("#ObservacaoFatura", titulo.FaturaParcela?.Fatura?.Observacao ?? "");
                corpoEmail = corpoEmail.Replace("#DataFatura", titulo.FaturaParcela?.Fatura?.DataFatura.ToString("dd/MM/yyyy") ?? "");
                corpoEmail = corpoEmail.Replace("#CNPJTomador", titulo.Pessoa?.CPF_CNPJ_Formatado);
                corpoEmail = corpoEmail.Replace("#NomeTomador", titulo.Pessoa?.Nome);
                corpoEmail = corpoEmail.Replace("#CNPJEmpresa", titulo.Empresa?.CNPJ_Formatado);
                corpoEmail = corpoEmail.Replace("#NomeEmpresa", titulo.Empresa?.RazaoSocial);
                corpoEmail = corpoEmail.Replace("#DatasVencimentosBoletos", titulo.FaturaParcela?.Fatura.Total.ToString("dd/MM/yyyy") ?? "");
                corpoEmail = corpoEmail.Replace("#ValorFatura", $"R$ {titulo.FaturaParcela?.Valor:N2}" ?? "");

                string tabelaCorpo = "";
                if (corpoEmail.Contains("#Tabela"))
                {
                    tabelaCorpo = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                    tabelaCorpo += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                    tabelaCorpo += "<table style='width:100%; align='center'; border='1';>";
                    tabelaCorpo += "<tr>";
                    tabelaCorpo += "<th>Documento</th>";
                    tabelaCorpo += "<th>Número</th>";

                    tabelaCorpo += "<th>Fatura</th>";
                    tabelaCorpo += "</tr>";

                    foreach (var documento in titulo.Documentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documento.CTe?.Codigo ?? 0);
                        if (cte != null)
                        {
                            tabelaCorpo += "<tr>";
                            tabelaCorpo += "<td>" + cte.ModeloDocumentoFiscal?.Abreviacao + "</td>";
                            tabelaCorpo += "<td>" + cte.Numero + "</td>";
                            tabelaCorpo += "<td>" + Utilidades.String.OnlyNumbers(titulo.FaturaParcela?.Fatura?.Numero.ToString() ?? "") + "</td>";
                            tabelaCorpo += "</tr>";
                        }
                    }
                    tabelaCorpo += "</table>";
                }
                corpoEmail = corpoEmail.Replace("#Tabela", tabelaCorpo);
            }



            if (!string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                attachments.Add(new System.Net.Mail.Attachment(titulo.CaminhoBoleto));

            if (titulo.EnviarDocumentacaoFaturamentoCTe && titulo.Documentos != null && titulo.Documentos.Count > 0)
            {
                foreach (var documento in titulo.Documentos)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documento.CTe?.Codigo ?? 0);
                    if (cte == null)
                        continue;
                    string nomeArquivo = "";
                    byte[] data = null;
                    if (cte.ModeloDocumentoFiscal.Numero == "39")
                    {
                        nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                        Servicos.NFSe svcNFSe = new Servicos.NFSe();
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
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        nomeArquivo = Path.GetFileName(caminhoPDF);
                        data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                        if (data != null)
                        {
                            Stream stream = new MemoryStream(data);
                            attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                        }
                    }
                }

                if (edis != null && edis.Count > 0)
                {
                    try
                    {
                        foreach (var layoutEDI in edis)
                        {
                            using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(titulo, layoutEDI, unitOfWork))
                            {
                                Stream stream = new MemoryStream(arquivoEDI.ToArray());
                                string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(titulo, layoutEDI, unitOfWork, false);
                                nomeArquivo = nomeArquivo.Replace("-", "");
                                attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo, "text/plain"));
                            }
                        }
                    }
                    catch
                    {
                        corpoEmail += "<br/><br/>Problemas ao anexar o EDI configurado, por gentileza solicite o arquivo.";
                    }
                }
            }

            corpoEmail += "<br/><br/>By Multisoftware";

            string mensagemErro = "Erro ao enviar e-mail";
            emails = emails.Distinct().ToList();
            if (emails.Count > 0)
            {
                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails.Count == 1 ? emails[0] : null, emails.ToArray(), null, assuntoEmail, corpoEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                    attachments != null && attachments.Count > 0 ? attachments : null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
                if (!sucesso)
                    return false;
            }
            return true;
        }


    }
}