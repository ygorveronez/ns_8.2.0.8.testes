using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ReportApi.ReportService
{
    public sealed class CCe
    {
        /*
        public bool Emitir(Dominio.Entidades.CartaDeCorrecaoEletronica cce, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.ServicoCTe.uCteServiceTSSoapClient svcCCe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).
                ObterClient<Servicos.ServicoCTe.uCteServiceTSSoapClient, Servicos.ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            Servicos.ServicoCTe.CCe cceIntegrar = this.ObterCCeParaEmissao(cce, empresa, unidadeDeTrabalho);

            try
            {
                Servicos.ServicoCTe.ResultadoInteger retorno = svcCCe.ImportaCCe(cceIntegrar);

                if (retorno.Valor <= 0)
                {
                    if (retorno.Info.Mensagem == "CT-e não encontrado." && cce.CTe != null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho.StringConexao);
                        if (svcCTe.IntegrareCTeOracle(cce.CTe.Empresa, cce.CTe.Codigo, unidadeDeTrabalho))
                        {
                            cceIntegrar = this.ObterCCeParaEmissao(cce, empresa, unidadeDeTrabalho);
                            Servicos.ServicoCTe.ResultadoInteger retorno2 = svcCCe.ImportaCCe(cceIntegrar);
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
                Servicos.ServicoCTe.uCteServiceTSSoapClient svcCCe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).
                    ObterClient<Servicos.ServicoCTe.uCteServiceTSSoapClient, Servicos.ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe);

                if (cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                {
                    Servicos.ServicoCTe.RetornoEvento retorno = svcCCe.ConsultaEvento(cce.CodigoIntegrador);

                    unidadeDeTrabalho.Start();

                    Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho.StringConexao);
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
        */

        public byte[] ObterRelatorio(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            if (cce != null)
            {
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                Stream stream = null;
                byte[] data = null;
                if (cce.CTe != null)
                {
                    data = svcCTe.ObterXMLAutorizacao(cce.CTe, unitOfWork);
                    if (data != null && data.Length > 0)
                    {
                        try
                        {
                            stream = new MemoryStream(data);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"Erro ao utilizar o XML de autorização do CTE: {ex}");
                            throw new Exception("Erro ao utilizar o XML de autorização do CTE");
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Erro : Não foi possível obter o XML de autorização do CTE!");
                        throw new Exception("Erro: Não foi possível obter o XML de autorização do CTE!");
                    }

                }

                List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cce.Codigo);
                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                if (itensCCe != null && itensCCe.Count > 0 && stream != null)
                {
                    foreach (var item in itensCCe)
                    {
                        stream = new MemoryStream(data);
                        string valorAnterior = "";
                        if (!item.Descricao.ToLower().Contains("navio") && !item.Descricao.ToLower().Contains("booking"))
                            valorAnterior = BuscarValorAnteriorXML(item.GrupoCampo, item.NomeCampo, item.NumeroItemAlterado, stream);
                        if (string.IsNullOrWhiteSpace(valorAnterior) && (item.Descricao.ToLower().Contains("lacre") || item.Descricao.ToLower().Contains("container")))
                        {
                            if (cce.CTe.Containers != null && cce.CTe.Containers.Count > 0)
                            {
                                valorAnterior = "";
                                foreach (var container in cce.CTe.Containers)
                                {
                                    if (item.Descricao.ToLower().Contains("lacre"))
                                    {
                                        var lacres = new List<string>();

                                        if (container != null && !string.IsNullOrWhiteSpace(container.Lacre1))
                                            lacres.Add(container.Lacre1);
                                        if (container != null && !string.IsNullOrWhiteSpace(container.Lacre2))
                                            lacres.Add(container.Lacre2);
                                        if (container != null && !string.IsNullOrWhiteSpace(container.Lacre3))
                                            lacres.Add(container.Lacre3);

                                        if (lacres.Count == 1)
                                            valorAnterior += lacres[0];

                                        else if (lacres.Count > 1)
                                        {

                                            if (!string.IsNullOrWhiteSpace(container.Lacre1))
                                                valorAnterior += " Lacre 1: " + container.Lacre1;
                                            if (!string.IsNullOrWhiteSpace(container.Lacre2))
                                                valorAnterior += " Lacre 2: " + container.Lacre2;
                                            if (!string.IsNullOrWhiteSpace(container.Lacre3))
                                                valorAnterior += " Lacre 3: " + container.Lacre3;
                                        }
                                    }

                                    else if (item.Descricao.ToLower().Contains("container"))
                                    {
                                        if (container != null && !string.IsNullOrWhiteSpace(container.Numero))
                                            valorAnterior += " Container: " + container.Numero;
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(valorAnterior))
                        {
                            item.ValorAlterado = "Onde se lê: " + valorAnterior + " > Leia-se: " + item.ValorAlterado;
                        }
                    }
                }
                ReportDataSource dataSourceCCe = new ReportDataSource();
                dataSourceCCe.Name = "CCe";
                dataSourceCCe.Value = new Dominio.Entidades.CartaDeCorrecaoEletronica[] { cce };
                dataSources.Add(dataSourceCCe);

                ReportDataSource dataSourceItensCCe = new ReportDataSource();
                dataSourceItensCCe.Name = "ItensCCe";
                dataSourceItensCCe.Value = itensCCe;
                dataSources.Add(dataSourceItensCCe);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = GerarWeb(Utilidades.IO.FileStorageService.Storage.Combine("Areas", "Relatorios", "ReportViwer", "RelatorioCCe.rdlc"), "PDF", null, dataSources);

                string nomeArquivo = $"CCe_{cce.NumeroSequencialEvento}-CTe_{cce.NumeroCTe}_{cce.SerieCTe}.{arquivo.FileNameExtension}";
                return arquivo.Arquivo;
            }

            return null;
        }

        public byte[] ObterXML(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            if (cce != null)
            {
                Servicos.ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork)
                    .ObterClient<Servicos.ServicoCTe.uCteServiceTSSoapClient, Servicos.ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

                Servicos.ServicoCTe.RetornoEvento retorno = svcCTe.ConsultaEvento(cce.CodigoIntegrador);

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

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cce.CTe.Empresa.CNPJ_SemFormato, cce.DataEmissao.Value.Month + "-" + cce.DataEmissao.Value.Year);

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
            string caminho = ObterDiretorioArmazenamentoXMLCCeArquivo(cce, status, unitOfWork);

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

        private Servicos.ServicoCTe.CCe ObterCCeParaEmissao(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.ServicoCTe.CCe cceIntegrar = new Servicos.ServicoCTe.CCe();

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
                                 select new Servicos.ServicoCTe.ItemCCe()
                                 {
                                     GrupoCampo = obj.CampoAlterado.GrupoCampo,
                                     NomeCampo = obj.CampoAlterado.NomeCampo,
                                     NumeroItemAlterado = obj.NumeroItemAlterado,
                                     ValorAlterado = obj.ValorAlterado
                                 }).ToList();

            return cceIntegrar;
        }

        private Servicos.ServicoCTe.Empresa ObterEmpresaEmitente(Dominio.Entidades.Empresa empresa)
        {
            Servicos.ServicoCTe.Empresa empresaEmitente = new Servicos.ServicoCTe.Empresa();
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

        private Dominio.ObjetosDeValor.Relatorios.Relatorio GerarWeb(string path, string format, List<Microsoft.Reporting.WebForms.ReportParameter> parameters, List<Microsoft.Reporting.WebForms.ReportDataSource> dataSources, Microsoft.Reporting.WebForms.SubreportProcessingEventHandler subreportHandler = null)
        {
            Microsoft.Reporting.WebForms.LocalReport reportViewer = new Microsoft.Reporting.WebForms.LocalReport();

            reportViewer.ReportPath = Utilidades.IO.FileStorageService.Storage.Combine(ReportApi.Extensions.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), path);

            reportViewer.EnableExternalImages = true;

            if (parameters != null)
                reportViewer.SetParameters(parameters);

            foreach (Microsoft.Reporting.WebForms.ReportDataSource dataSource in dataSources)
                reportViewer.DataSources.Add(dataSource);

            if (subreportHandler != null)
                reportViewer.SubreportProcessing += subreportHandler;

            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = reportViewer.Render(format, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            try
            {
                reportViewer.Dispose();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao fazer dispose do ReportViewer CCe: {ex.ToString()}", "CatchNoAction");
            }

            reportViewer = null;
            path = null;
            parameters = null;
            dataSources = null;
            subreportHandler = null;

            GC.Collect();

            return new Dominio.ObjetosDeValor.Relatorios.Relatorio(bytes, mimeType, filenameExtension);
        }
    }
}
