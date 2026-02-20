using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.Reporting.WebForms;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Servicos
{
    public class CCe : ServicoBase
    {
        public CCe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public bool Emitir(Dominio.Entidades.CartaDeCorrecaoEletronica cce, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            ServicoCTe.uCteServiceTSSoapClient svcCCe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            ServicoCTe.CCe cceIntegrar = this.ObterCCeParaEmissao(cce, empresa, unidadeDeTrabalho);

            try
            {
                ServicoCTe.ResultadoInteger retorno = svcCCe.ImportaCCe(cceIntegrar);

                if (retorno.Valor <= 0)
                {
                    if (retorno.Info.Mensagem == "CT-e não encontrado." && cce.CTe != null)
                    {
                        Servicos.CTe svcCTe = new CTe(unidadeDeTrabalho);
                        if (svcCTe.IntegrareCTeOracle(cce.CTe.Empresa, cce.CTe.Codigo, unidadeDeTrabalho))
                        {
                            cceIntegrar = this.ObterCCeParaEmissao(cce, empresa, unidadeDeTrabalho);
                            ServicoCTe.ResultadoInteger retorno2 = svcCCe.ImportaCCe(cceIntegrar);
                            if (retorno2.Valor <= 0)
                            {
                                cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno2.Info.Mensagem);
                                cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                                repCCe.Atualizar(cce);

                                Servicos.Log.TratarErro(retorno2.Info.MensagemOriginal);

                                return false;
                            }
                            else
                            {
                                cce.CodigoIntegrador = retorno2.Valor;
                                cce.MensagemRetornoSefaz = "CT-e em processamento.";
                                cce.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador;
                                cce.Status = Dominio.Enumeradores.StatusCCe.Enviado;
                                repCCe.Atualizar(cce);

                                return true;
                            }
                        }
                        else
                        {
                            cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                            cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                            repCCe.Atualizar(cce);

                            Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                            return false;
                        }
                    }
                    else
                    {
                        cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                        cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                        repCCe.Atualizar(cce);

                        Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                        return false;
                    }
                }
                else
                {
                    cce.CodigoIntegrador = retorno.Valor;
                    cce.MensagemRetornoSefaz = "CT-e em processamento.";
                    cce.Status = Dominio.Enumeradores.StatusCCe.Enviado;
                    repCCe.Atualizar(cce);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                cce.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                repCCe.Atualizar(cce);

                return false;
            }
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica Consultar(int codigoCCe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho = unidadeDeTrabalho != null ? unidadeDeTrabalho : new Repositorio.UnitOfWork(unidadeDeTrabalho.StringConexao);

            try
            {
                ServicoCTe.uCteServiceTSSoapClient svcCCe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe);

                if (cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                {
                    ServicoCTe.RetornoEvento retorno = svcCCe.ConsultaEvento(cce.CodigoIntegrador);

                    unidadeDeTrabalho.Start();

                    Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                    cce.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.CTe);

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("R"))
                        {
                            cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                            cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            cce.Status = Dominio.Enumeradores.StatusCCe.Enviado;
                            cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else
                        {
                            cce.Status = Dominio.Enumeradores.StatusCCe.Autorizado;
                            cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                            cce.Protocolo = retorno.Protocolo;
                            cce.DataRetornoSefaz = retorno.DataRetornoSefaz != DateTime.MinValue ? retorno.DataRetornoSefaz : DateTime.Now;
                        }
                    }
                    else
                    {
                        cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                        cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                    }

                    repCCe.Atualizar(cce);
                }

                unidadeDeTrabalho.CommitChanges();

                return cce;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public bool ReceberEventoCCe(out string mensagemErro, out Exception exception, Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unidadeDeTrabalho, ref Dominio.Entidades.CartaDeCorrecaoEletronica cce)
        {
            mensagemErro = string.Empty;
            exception = null;

            try
            {
                Servicos.CCe svcCCe = new Servicos.CCe(unidadeDeTrabalho);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                if (cce == null)
                    cce = cceOracle.CodigoCTeInterno > 0 ? repCCe.BuscarPorProtocoloAutorizacao(cceOracle.CodigoCTeInterno) : null;

                if (cce == null)
                {
                    mensagemErro = "CC-e " + cceOracle.CodigoCTeInterno + " não localizado na base SqlServer";
                    return false;
                }

                if (cce.Status == Dominio.Enumeradores.StatusCCe.Pendente || cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                {
                    unidadeDeTrabalho.Start();

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoSGT = repConfiguracaoSGT.BuscarConfiguracaoPadrao();

                    bool armazenarEmArquivo = configuracaoSGT?.ArmazenarXMLCTeEmArquivo ?? false;

                    int.TryParse(string.IsNullOrWhiteSpace(cceOracle.CodStatusProtocolo) ? cceOracle.CodStatusEnvio : cceOracle.CodStatusProtocolo, out int statusCCe);

                    DateTime.TryParseExact(cceOracle.DataProtocolo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataProtocolo);

                    if (cceOracle.StatusIntegrador == "R")
                    {
                        cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                        cce.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(statusCCe, Dominio.Enumeradores.TipoErroSefaz.CTe);
                        cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters($"{cceOracle.CodStatusProtocolo} - {cceOracle.DescricaoProtocolo}");

                        repCCe.Atualizar(cce);
                    }
                    else if (cceOracle.StatusIntegrador == "P" || cceOracle.StatusIntegrador == "M")
                    {
                        cce.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(statusCCe, Dominio.Enumeradores.TipoErroSefaz.CTe);
                        cce.Status = Dominio.Enumeradores.StatusCCe.Autorizado;
                        cce.Protocolo = cceOracle.NumeroProtocolo;
                        cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters($"{cceOracle.CodStatusProtocolo} - {cceOracle.DescricaoProtocolo}");
                        cce.DataRetornoSefaz = dataProtocolo != DateTime.MinValue ? dataProtocolo : DateTime.Now;

                        repCCe.Atualizar(cce);

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(cce.CTe?.Codigo ?? 0);
                        if (cte != null)
                        {
                            cte.PossuiCartaCorrecao = true;
                            repCTe.Atualizar(cte);

                            Servicos.Auditoria.Auditoria.Auditar(auditado, cte, "Informou que possui carta de correção.", unidadeDeTrabalho);
                        }

                        svcCCe.ObterESalvarXMLAutorizacaoOracle(cce, armazenarEmArquivo, cceOracle, unidadeDeTrabalho);
                    }

                    Servicos.Embarcador.Integracao.IntegracaoCTe servicoIntegracaoCTe = new Servicos.Embarcador.Integracao.IntegracaoCTe();
                    servicoIntegracaoCTe.GerarIntegracoesCartaCorrecaoCte(cce, unidadeDeTrabalho);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, cce, "Integrou CC-e autorizada.", unidadeDeTrabalho);

                    unidadeDeTrabalho.CommitChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                mensagemErro = "Ocorreu uma falha ao integrar a CC-e";
                return false;
            }
        }

        public Dominio.ObjetosDeValor.Relatorios.Relatorio ObterRelatorio(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            if (cce != null)
            {

                byte[] arquivoCCeByte = null;

                var resultReportApi = ReportRequest.WithType(ReportType.CCe)
                                              .WithExecutionType(ExecutionType.Async)
                                              .AddExtraData("codigoCCe", cce.Codigo)
                                              .CallReport();

                if (resultReportApi == null)
                    return null;
                else
                {
                    arquivoCCeByte = resultReportApi.GetContentFile();
                    string nomeArquivo = $"CCe_{cce.NumeroSequencialEvento}-CTe_{cce.NumeroCTe}_{cce.SerieCTe}.{"pdf"}";

                    return new Dominio.ObjetosDeValor.Relatorios.Relatorio(arquivoCCeByte, "application/pdf", nomeArquivo);
                }
            }

            return null;
        }

        public byte[] ObterXML(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            if (cce != null)
            {
                ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

                ServicoCTe.RetornoEvento retorno = svcCTe.ConsultaEvento(cce.CodigoIntegrador);

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(retorno.XML);
                    return data;
                }
            }

            return null;
        }

        public string ObterDiretorioArmazenamentoXMLCCeArquivo(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Dominio.Enumeradores.StatusCCe status, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRetornoXMLIntegrador;

            if (cce.CTe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Producao");
            else
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Homologacao");

            if (status == Dominio.Enumeradores.StatusCCe.Autorizado)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Autorizacao");
            else
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Outros");

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cce.CTe.Empresa.CNPJ_SemFormato);

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cce.DataEmissao.Value.Month + "-" + cce.DataEmissao.Value.Year);

            return caminho;
        }

        public string ObterCaminhoArmazenamentoXMLCCeArquivo(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Dominio.Enumeradores.StatusCCe status, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterDiretorioArmazenamentoXMLCCeArquivo(cce, status, unitOfWork);

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{cce.CTe.Chave}_{cce.NumeroSequencialEvento}-procCCe.xml");

            return caminho;
        }

        public string CriarERetornarCaminhoXMLCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Dominio.Enumeradores.StatusCCe status, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterCaminhoArmazenamentoXMLCCeArquivo(cce, status, unitOfWork);
        }

        public void ObterESalvarXMLAutorizacaoOracle(Dominio.Entidades.CartaDeCorrecaoEletronica cce, bool armazenarEmArquivo, Dominio.ObjetosDeValor.WebService.CTe.CTeOracle retorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (cce != null && retorno != null && !string.IsNullOrWhiteSpace(retorno.XML))
            {
                string arquivo = CriarERetornarCaminhoXMLCCe(cce, Dominio.Enumeradores.StatusCCe.Autorizado, unidadeDeTrabalho);
                Utilidades.IO.FileStorageService.Storage.WriteAllText(arquivo, retorno.XML);
            }
        }

        public byte[] ObterXMLAutorizacao(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            string arquivo = CriarERetornarCaminhoXMLCCe(cce, Dominio.Enumeradores.StatusCCe.Autorizado, unitOfWork);

            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
        }

        public string BuscarValorAnteriorXML(string tagPai, string tagFilho, int sequencia, Stream stream)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(stream);

                if (string.IsNullOrWhiteSpace(tagPai))
                {
                    var items = document.GetElementsByTagName(tagFilho);
                    int count = 1;
                    foreach (var item in items)
                    {
                        if (sequencia == 0)
                            return ((System.Xml.XmlElement)(item)).InnerText.ToString();
                        else if (sequencia == count)
                            return ((System.Xml.XmlElement)(item)).InnerText.ToString();
                        count += 1;
                    }
                }
                else
                {
                    var itemsPai = document.GetElementsByTagName(tagPai);
                    XmlDocument doc2 = new XmlDocument();
                    doc2.AppendChild(doc2.CreateElement(tagPai));
                    foreach (XmlElement person in itemsPai)
                    {
                        doc2.DocumentElement.AppendChild(doc2.ImportNode(person, true));
                    }

                    var items = doc2.GetElementsByTagName(tagFilho);
                    int count = 1;
                    foreach (var item in items)
                    {
                        if (sequencia == 0)
                            return ((System.Xml.XmlElement)(item)).InnerText.ToString();
                        else if (sequencia == count)
                            return ((System.Xml.XmlElement)(item)).InnerText.ToString();
                        count += 1;
                    }
                }
            }
            catch
            {
                return "";
            }
            return "";
        }

        #endregion

        #region Métodos Privados

        private ServicoCTe.CCe ObterCCeParaEmissao(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoCTe.CCe cceIntegrar = new ServicoCTe.CCe();

            cceIntegrar.Ambiente = (int)cce.CTe.TipoAmbiente;
            cceIntegrar.CodigoCTeInterno = cce.CTe.CodigoCTeIntegrador;
            cceIntegrar.DataEmissao = cce.DataEmissao.HasValue ? cce.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            cceIntegrar.Empresa = this.ObterEmpresaEmitente(empresa);
            cceIntegrar.NumeroSequencialEvento = cce.NumeroSequencialEvento;

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(cce.DataEmissao.HasValue ? cce.DataEmissao.Value : DateTime.Today);
            cceIntegrar.FusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);

            List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cce.Codigo);

            cceIntegrar.Itens = (from obj in itensCCe
                                 select new ServicoCTe.ItemCCe()
                                 {
                                     GrupoCampo = obj.CampoAlterado.GrupoCampo,
                                     NomeCampo = obj.CampoAlterado.NomeCampo,
                                     NumeroItemAlterado = obj.NumeroItemAlterado,
                                     ValorAlterado = obj.ValorAlterado
                                 }).ToList();

            return cceIntegrar;
        }

        private ServicoCTe.Empresa ObterEmpresaEmitente(Dominio.Entidades.Empresa empresa)
        {
            ServicoCTe.Empresa empresaEmitente = new ServicoCTe.Empresa();
            empresaEmitente.Bairro = Utilidades.String.Left(empresa.Bairro, 60);
            empresaEmitente.Cep = Utilidades.String.OnlyNumbers(empresa.CEP);
            empresaEmitente.Cidade = Utilidades.String.Left(empresa.Localidade.Descricao, 60);
            empresaEmitente.CNPJ = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            empresaEmitente.CodigoCidadeIBGE = empresa.Localidade.CodigoIBGE;
            empresaEmitente.Complemento = Utilidades.String.Left(empresa.Complemento, 60);
            empresaEmitente.EmailContador = empresa.EmailContador;
            empresaEmitente.EmailEmitente = empresa.Email;
            empresaEmitente.EnviaEmailContador = empresa.StatusEmail;
            empresaEmitente.EnviaEmailEmitente = empresa.StatusEmailContador;
            empresaEmitente.IE = string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) ? "ISENTO" : empresa.InscricaoEstadual;
            empresaEmitente.Logradouro = Utilidades.String.Left(empresa.Endereco, 255);
            empresaEmitente.NomeContador = Utilidades.String.Left(empresa.NomeContador, 60);
            empresaEmitente.NomeFantasia = Utilidades.String.Left(empresa.NomeFantasia, 60);
            empresaEmitente.NomeRazao = Utilidades.String.Left(empresa.RazaoSocial, 60);
            empresaEmitente.Numero = Utilidades.String.Left(empresa.Numero, 60);
            empresaEmitente.Status = empresa.Status;
            empresaEmitente.Telefone = Utilidades.String.OnlyNumbers(empresa.Telefone);
            empresaEmitente.TelefoneContador = Utilidades.String.OnlyNumbers(empresa.TelefoneContador);
            empresaEmitente.UF = empresa.Localidade.Estado.Sigla;
            return empresaEmitente;
        }

        private string AjustarFuso(int hora, int minutos)
        {
            string minutosString = minutos.ToString();

            if (minutosString.Length == 1)
                minutosString = string.Concat("0", minutosString);

            switch (hora)
            {
                case -12:
                    return "-12:" + minutosString;
                case -11:
                    return "-11:" + minutosString;
                case -10:
                    return "-10:" + minutosString;
                case -9:
                    return "-09:" + minutosString;
                case -8:
                    return "-08:" + minutosString;
                case -7:
                    return "-07:" + minutosString;
                case -6:
                    return "-06:" + minutosString;
                case -5:
                    return "-05:" + minutosString;
                case -4:
                    return "-04:" + minutosString;
                case -3:
                    return "-03:" + minutosString;
                case -2:
                    return "-02:" + minutosString;
                case -1:
                    return "-01:" + minutosString;
                case 0:
                    return "00:" + minutosString;
                case 1:
                    return "+01:" + minutosString;
                case 2:
                    return "+02:" + minutosString;
                case 3:
                    return "+03:" + minutosString;
                case 4:
                    return "+04:" + minutosString;
                case 5:
                    return "+05:" + minutosString;
                case 6:
                    return "+06:" + minutosString;
                case 7:
                    return "+07:" + minutosString;
                case 8:
                    return "+08:" + minutosString;
                case 9:
                    return "+09:" + minutosString;
                case 10:
                    return "+10:" + minutosString;
                case 11:
                    return "+11:" + minutosString;
                case 12:
                    return "+12:" + minutosString;
                case 13:
                    return "+13:" + minutosString;
                default:
                    return "-03:00";
            }

        }

        #endregion
    }
}
