using AdminMultisoftware.Dominio.Entidades.Pessoas;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Localization.Resources.Consultas;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System.Data.Common;
using System.Drawing;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize(new string[] { "byteArrayToImage", "RequisicaoConsultaCNPJ", "InformarCaptchaConsultaCNPJ", "ConsultaFaturamentoFornecedor", "BuscarValorCombustivelTabela", "BuscarPorCodigo", "ConsultaCNPJReceitaWS" }, "Pessoas/Pessoa", "Financeiros/PlanoOrcamentario", "Financeiros/LancamentoConta", "Financeiros/TituloFinanceiro")]
    public class PessoaController : BaseController
    {
        #region Construtores

        public PessoaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public async Task<IActionResult> RequisicaoConsultaCNPJ()
        {
            try
            {                
                string chave = Request.Params("CNPJ").Replace(" ", "");

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader(Localization.Resources.Pessoas.Pessoa.Token, "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaCNPJ.RetornoOfRequisicaoFazendaPessoaJuridicaDggAjPvf requisicaoCNPJ = consultaCNPJ.SolicitarRequisicaoFazendaPessoaJuridica();
                if (requisicaoCNPJ.Status)
                {
                    string base64String = Convert.ToBase64String(requisicaoCNPJ.Objeto.Captcha, 0, requisicaoCNPJ.Objeto.Captcha.Length);
                    string htmlstr = "data:image/png;base64," + base64String;

                    var retorno = new
                    {
                        chaptcha = htmlstr,
                        Cookies = requisicaoCNPJ.Objeto.Cookies
                    };
                    return new JsonpResult(retorno);
                }
                else
                {
                    return new JsonpResult(false, true, requisicaoCNPJ.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoConsultarOsDadosDoCliente);
            }
        }

        public async Task<IActionResult> ConsultarCoordenadasOpenMap()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Integracao.MapRequest.Geocoding serGeocoding = new Servicos.Embarcador.Integracao.MapRequest.Geocoding();
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Localidade")));
                string endereco = Request.Params("Endereco");
                string numero = Request.Params("Numero");
                string cEP = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
                Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas coordenadas = serGeocoding.BuscarCoordenadasEndereco(localidade, endereco, numero);

                return new JsonpResult(coordenadas);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarCoordenadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            // TODO: checar aqui por permissão para fazer a atualização

            try
            {
                // Instancia repositorios
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.UsuarioSemPermissaoParaRealizarEssaAcao);

                // Parametros
                double dCPFCNPJ = 0;
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CPF_CNPJ"));

                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = double.Parse(cpfcnpj);

                string latitude = Request.GetStringParam("Latitude");
                string longitude = Request.GetStringParam("Longitude");

                // Busca informacoes
                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(dCPFCNPJ);

                // Valida
                if (pessoa == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelEncontrarPessoa);


                if (latitude == null || latitude == "" || longitude == null || longitude == "")
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.CoordenadasInvalidas);

                unitOfWork.Start();
                try
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, Localization.Resources.Pessoas.Pessoa.AsCoordenadasForamAlteradasAtravesDaTelaDeControleDeEntregaDe + "{pessoa.Latitude}, {pessoa.Longitude}" + Localization.Resources.Pessoas.Pessoa.Para + "({latitude}, {longitude})", unitOfWork);

                    pessoa.Latitude = latitude;
                    pessoa.Longitude = longitude;
                    pessoa.DataUltimaAtualizacao = DateTime.Now;
                    pessoa.Integrado = false;
                    repPessoa.Atualizar(pessoa);

                    unitOfWork.CommitChanges();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFlhaAoAlterarAsCoordenadasDoCliente);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ConsultaCNPJReceitaWS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ").Replace(" ", ""));

                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                string json_data = string.Empty;
                using (WebClient w = new WebClient())
                {
                    w.Encoding = System.Text.Encoding.UTF8;
                    json_data = w.DownloadString("https://www.receitaws.com.br/v1/cnpj/" + cnpj);
                }

                if (string.IsNullOrWhiteSpace(json_data))
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.ServicoIndisponivelNoMomento);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = new ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf();
                dynamic retornoReceitaWS = JsonConvert.DeserializeObject<dynamic>(json_data);

                ConverterObjetoRetornoWS(ref retorno, retornoReceitaWS);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(3);

                        Dominio.Entidades.Localidade localidade = null; ;
                        if (retorno.Objeto.Pessoa.Endereco.Cidade.IBGE > 0)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(retorno.Objeto.Pessoa.Endereco.Cidade.IBGE);

                        if (localidade == null)
                        {
                            List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao);
                            localidade = listaLocalidades.Count() == 1 ? listaLocalidades.FirstOrDefault() : null;
                        }


                        string nomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) && !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "")) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;
                        string razaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;

                        if (nomeFantasia.Length > 79)
                            nomeFantasia = nomeFantasia.Substring(0, 79);
                        if (razaoSocial.Length > 79)
                            razaoSocial = razaoSocial.Substring(0, 79);

                        object retornoDados = new
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.CEP) ? String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP.PadLeft(8, '0'))) : string.Empty,
                            Localidade = localidade != null ? new { localidade.Codigo, Descricao = localidade.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Numero,
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Trim() : string.Empty,
                            TelefoneSecundario = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone2).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone2).Trim() : string.Empty,
                            Fantasia = nomeFantasia,
                            Nome = razaoSocial,
                            EnderecoDigitado = true,
                            Atividade = atividade != null ? new { Codigo = atividade.Codigo, Descricao = atividade.Descricao } : null
                        };
                        return new JsonpResult(retornoDados);
                    }
                    else
                    {
                        return new JsonpResult(false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return new JsonpResult(false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoConsultarOsDadosDoCNPJ);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultaCNPJCentralizada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                string cnpj = Request.Params("CNPJ").Replace(" ", "");

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader(Localization.Resources.Pessoas.Pessoa.Token, "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = consultaCNPJ.ConsultarCadastroCentralizado(cnpj);
                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(3);

                        Dominio.Entidades.Localidade localidade = null; ;
                        if (retorno.Objeto.Pessoa.Endereco.Cidade.IBGE > 0)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(retorno.Objeto.Pessoa.Endereco.Cidade.IBGE);

                        if (localidade == null)
                        {
                            List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao);
                            localidade = listaLocalidades.Count() == 1 ? listaLocalidades.FirstOrDefault() : null;
                        }


                        string nomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) && !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "")) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;
                        string razaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;

                        if (nomeFantasia.Length > 79)
                            nomeFantasia = nomeFantasia.Substring(0, 79);
                        if (razaoSocial.Length > 79)
                            razaoSocial = razaoSocial.Substring(0, 79);

                        object retornoDados = new
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.CEP) ? String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP.PadLeft(8, '0'))) : string.Empty,
                            Localidade = localidade != null ? new { localidade.Codigo, Descricao = localidade.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Numero,
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Trim() : string.Empty,
                            Fantasia = nomeFantasia,
                            Nome = razaoSocial,
                            EnderecoDigitado = true,
                            IE_RG = retorno.Objeto.Pessoa.RGIE,
                            Atividade = atividade != null ? new { Codigo = atividade.Codigo, Descricao = atividade.Descricao } : null,
                        };
                        return new JsonpResult(retornoDados);
                    }
                    else
                    {
                        return new JsonpResult(false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return new JsonpResult(false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoConsultarOsDadosDoCliente);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarCaptchaConsultaCNPJ()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica();
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                requisicaoSefaz.Cookies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.WebService.CookieDinamico>>(Request.Params("Cookies"));
                string CNPJ = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));//"13969629000196"; //Request.Params("CNPJ").Replace(" ", "").Replace("/", "").Replace("-", "").Replace(".", "");
                string captcha = Request.Params("Captcha");

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader(Localization.Resources.Pessoas.Pessoa.Token, "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = consultaCNPJ.ConsultarPessoaJuridicaFazenda(requisicaoSefaz, CNPJ, captcha);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        string ie = "";
                        ConsultaCNPJ.RetornoOfstring retornoSintegra = consultaCNPJ.ConsultarInscricaoSintegra(CNPJ);
                        if (retornoSintegra.Status)
                            ie = retornoSintegra.Objeto;

                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        Dominio.Entidades.Atividade atividade = repAtividade.BuscarPrimeiraAtividade();
                        List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao);

                        string nomeFantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) && !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "")) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;
                        string razaoSocial = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial;

                        if (nomeFantasia.Length > 79)
                            nomeFantasia = nomeFantasia.Substring(0, 79);
                        if (razaoSocial.Length > 79)
                            razaoSocial = razaoSocial.Substring(0, 79);

                        object retornoDados = new
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)),
                            Localidade = listaLocalidades.Count() == 1 ? new { Codigo = listaLocalidades[0].Codigo, Descricao = listaLocalidades[0].DescricaoCidadeEstado } : null,
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Numero,
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Trim() : string.Empty,
                            Fantasia = nomeFantasia,
                            Nome = razaoSocial,
                            EnderecoDigitado = true,
                            IE_RG = ie,
                            Atividade = atividade != null ? new { Codigo = atividade.Codigo, Descricao = atividade.Descricao } : null,
                        };
                        return new JsonpResult(retornoDados);
                    }
                    else
                    {
                        return new JsonpResult(false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return new JsonpResult(false, retorno.Mensagem);
                }
                //return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoCarregarOsDadosDaReceita);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/Pessoa");

                int tipoPessoa = int.Parse(Request.Params("TipoPessoa"));
                string cpfcnpj = "";
                if (tipoPessoa > 0 && !string.IsNullOrWhiteSpace(Request.Params("CNPJ")))
                    cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));
                else if (tipoPessoa == 0)
                    cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CPF"));
                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = double.Parse(cpfcnpj);

                string tipo = tipoPessoa == 0 ? "F" : tipoPessoa == 1 ? "J" : "E";

                bool tipoCliente = bool.Parse(Request.Params("TipoCliente"));
                bool tipoFornecedor = bool.Parse(Request.Params("TipoFornecedor"));
                bool tipoTransportador = bool.Parse(Request.Params("TipoTransportador"));

                if (!tipoCliente && !tipoFornecedor && !tipoTransportador)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.FavorSelecioneAoMenosUmTipoDePessoa);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (tipoFornecedor && !Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pessoa_PermiteCriarFornecedor))
                        return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.VoceNaoPossuiPermissaoParaCriarUmFornecedor);

                unitOfWork.Start();

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);

                if (tipo == "E")
                    dCPFCNPJ = repPessoa.BuscarPorProximoExterior();

                bool inserir = true;
                Dominio.Entidades.Empresa empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null;

                Dominio.Entidades.Cliente pessoaValidar = repPessoa.BuscarPorCPFCNPJ(dCPFCNPJ);
                if (pessoaValidar != null)
                {
                    if (empresa?.VisualizarSomenteClientesAssociados ?? false)
                    {
                        Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(empresa.Codigo, dCPFCNPJ);
                        if (dadosCliente == null)
                            inserir = false;
                    }

                    if (inserir)
                        throw new ControllerException(Localization.Resources.Pessoas.Pessoa.JaExisteUmaPessoaCadastradaComEsteCNPJCPF + " <br/>" + Localization.Resources.Pessoas.Pessoa.SituacaoAtual + " " + pessoaValidar.DescricaoAtivo + "<br/>" + Localization.Resources.Pessoas.Pessoa.FavorIrNoInicioDaTelaAbrirPesquisaFiltrarEditar);
                }

                Dominio.Entidades.Cliente pessoa = !inserir ? pessoaValidar : new Dominio.Entidades.Cliente();
                if (!inserir)
                    pessoa.Initialize();

                PreencherDadosPessoa(pessoa, permissoesPersonalizadas, unitOfWork, true);
                SalvarOutrosCodigosIntegracao(ref pessoa, unitOfWork);
                PreencherDadosAdicionaisPessoa(pessoa, unitOfWork);

                dynamic configuracaoFatura = SalvarConfiguracaoFatura(pessoa, permissoesPersonalizadas, unitOfWork);

                if (inserir)
                {
                    SalvarConfiguracaoFaturaListasAuditarManual(pessoa, permissoesPersonalizadas, unitOfWork);

                    pessoa.CPF_CNPJ = dCPFCNPJ;

                    ValidarRaizCNPJ(pessoa, unitOfWork);
                    pessoa.DataUltimaAtualizacao = DateTime.Now;
                    pessoa.Integrado = false;
                    repPessoa.Inserir(pessoa, Auditado);
                }
                else
                {
                    pessoa.DataUltimaAtualizacao = DateTime.Now;
                    pessoa.Integrado = false;
                    repPessoa.Atualizar(pessoa, Auditado);
                }

                SalvarLayoutsEDI(pessoa, unitOfWork);
                SalvarListaEmail(pessoa, unitOfWork);
                SalvarListaSubarea(pessoa, unitOfWork);
                SalvarListaEndereco(pessoa, unitOfWork);
                SalvarListaDocumento(pessoa, unitOfWork);
                dynamic configuracao = SalvarConfiguracaoEmissaoCTe(pessoa, permissoesPersonalizadas, unitOfWork);
                SalvarDadosDescarga(pessoa, unitOfWork);
                PreecherDadosCliente(pessoa, unitOfWork, tipoCliente);
                PreecherDadosFornecedor(pessoa, unitOfWork, tipoFornecedor);
                PreecherTransportadorTerceiro(pessoa, unitOfWork, tipoTransportador);
                SalvarUsuarioTerceiro(pessoa, unitOfWork, tipoTransportador);
                SalvarUsuarioTerceiroAdicionais(pessoa, unitOfWork, tipoTransportador);
                SalvarFuncionario(pessoa, unitOfWork);
                SalvarUsuarioAcessoPortal(pessoa, unitOfWork);
                SalvarContatos(pessoa, unitOfWork);
                SalvarOutrasDescricoesPessoaExterior(pessoa, unitOfWork);
                SalvarLicencas(pessoa, unitOfWork);
                SalvarDadosArmador(pessoa, unitOfWork);
                SalvarVendedores(pessoa, unitOfWork);
                SalvarRecebedoresAutorizados(pessoa, unitOfWork);
                SalvarRotas(pessoa, unitOfWork);
                SalvarComponentes(pessoa, unitOfWork);
                SalvarDadosClientePorEmpresa(pessoa, unitOfWork);
                AtualizarRestricoesFilaCarregamento(pessoa, unitOfWork);
                SalvarObservacoesCTe(pessoa, unitOfWork);
                SalvarAreasRedex(pessoa, unitOfWork);
                SalvarSuprimentoDeGas(pessoa, unitOfWork);
                SalvarFrequenciaCarregamento(pessoa, unitOfWork);
                SalvarGrupoPessoas(pessoa, unitOfWork);
                SalvarTipoComprovantes(pessoa, unitOfWork);
                SalvarFilialCliente(pessoa, unitOfWork);
                SalvarContasBancarias(pessoa, unitOfWork);
                AtualizarConfiguracoesEmissao(pessoa, configuracao, null, unitOfWork);
                AtualizarConfiguracoesFatura(pessoa, configuracaoFatura, null, unitOfWork);

                new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork).GerarIntegracaoPessoa(unitOfWork, pessoa);
                new Servicos.Embarcador.GestaoPallet.RegraPalletHistorico(unitOfWork).SalvarNovaRegraPeriodo(pessoa);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarLatLngClientes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                dynamic pessoasPontos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PessoasPontos"));
                if (pessoasPontos != null)
                {
                    foreach (dynamic pessoaPonto in pessoasPontos)
                    {
                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ((double)pessoaPonto.CPFCNPJ);
                        if (cliente != null)
                        {
                            cliente.Latitude = (string)pessoaPonto.Latitude;
                            cliente.Longitude = (string)pessoaPonto.Longitude;
                            cliente.TipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.ponto;
                            cliente.DataUltimaAtualizacao = DateTime.Now;
                            cliente.Integrado = false;
                            repCliente.Atualizar(cliente);
                        }
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoAtualizarLatitudeLongitudeDosClientes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarRaioMaximoLatLngClienteXLocalidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                int codigoLocalidade = Request.GetIntParam("CodigoLocalidade");
                string slatitude = Request.GetStringParam("Latitude");
                string slongitude = Request.GetStringParam("Longitude");
                int raioMaximo = Request.GetIntParam("RaioMaximo");

                double latitude = slatitude.ToDouble();
                double longitude = slongitude.ToDouble();

                if (latitude + longitude == 0)
                    return new JsonpResult(true);

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);

                if (localidade == null)
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelLocalidarLocalidadeDaPessoa);

                else if (localidade.Latitude.HasValue && localidade.Longitude.HasValue)
                {
                    double distancia = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia((double)localidade.Latitude.Value, (double)localidade.Longitude.Value, latitude, longitude) / 1000;
                    if (distancia < raioMaximo)
                        return new JsonpResult(true);
                    else
                        return new JsonpResult(true, string.Format(Localization.Resources.Pessoas.Pessoa.GeolocalizacaoDaPessoaEstaUmRaioDeComRelacxoGeolocalizacaoDaCidade, distancia.ToString("n2")));
                }
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoValidarRaioMaximoEntreClienteLocalidade);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/Pessoa");

                double dCPFCNPJ = 0;
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("Codigo"));

                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = double.Parse(cpfcnpj);

                bool tipoCliente = bool.Parse(Request.Params("TipoCliente"));
                bool tipoFornecedor = bool.Parse(Request.Params("TipoFornecedor"));
                bool tipoTransportador = bool.Parse(Request.Params("TipoTransportador"));

                if (!tipoCliente && !tipoFornecedor && !tipoTransportador)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.FavorSelecioneAoMenosUmTipoDePessoa);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (tipoFornecedor && !Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pessoa_PermiteCriarFornecedor))
                        return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.VoceNaoPossuiPermissaoParaAtualizarUmFornecedor);

                unitOfWork.Start();

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(dCPFCNPJ);
                if (pessoa == null)
                    throw new ControllerException(Localization.Resources.Pessoas.Pessoa.ProblemasAoConsultarPessoaParaAtualizar);

                string oldLatitude = pessoa.Latitude;
                string oldLongitude = pessoa.Longitude;

                pessoa.Initialize();
                PreencherDadosPessoa(pessoa, permissoesPersonalizadas, unitOfWork);

                ValidarRaizCNPJ(pessoa, unitOfWork);

                string codigoAnterior = pessoa.CodigoDocumento;
                PreencherDadosAdicionaisPessoa(pessoa, unitOfWork);
                string novoCodigo = pessoa.CodigoDocumento;

                tipoTransportador = bool.Parse(Request.Params("TipoTransportador"));

                SalvarLayoutsEDI(pessoa, unitOfWork);
                SalvarListaEmail(pessoa, unitOfWork);
                SalvarListaSubarea(pessoa, unitOfWork);
                SalvarListaEndereco(pessoa, unitOfWork);
                SalvarListaDocumento(pessoa, unitOfWork);
                dynamic configuracao = SalvarConfiguracaoEmissaoCTe(pessoa, permissoesPersonalizadas, unitOfWork);
                dynamic configuracaoFatura = SalvarConfiguracaoFatura(pessoa, permissoesPersonalizadas, unitOfWork);
                SalvarDadosDescarga(pessoa, unitOfWork);
                PreecherDadosCliente(pessoa, unitOfWork, tipoCliente);
                PreecherDadosFornecedor(pessoa, unitOfWork, tipoFornecedor);
                PreecherTransportadorTerceiro(pessoa, unitOfWork, tipoTransportador);
                SalvarUsuarioTerceiro(pessoa, unitOfWork, tipoTransportador);
                SalvarUsuarioTerceiroAdicionais(pessoa, unitOfWork, tipoTransportador);
                SalvarFuncionario(pessoa, unitOfWork);
                SalvarUsuarioAcessoPortal(pessoa, unitOfWork);
                SalvarContatos(pessoa, unitOfWork);
                SalvarOutrasDescricoesPessoaExterior(pessoa, unitOfWork);
                SalvarLicencas(pessoa, unitOfWork);
                SalvarDadosArmador(pessoa, unitOfWork);
                SalvarVendedores(pessoa, unitOfWork);
                SalvarRecebedoresAutorizados(pessoa, unitOfWork);
                SalvarRotas(pessoa, unitOfWork);
                SalvarComponentes(pessoa, unitOfWork);
                SalvarDadosClientePorEmpresa(pessoa, unitOfWork);
                AtualizarRestricoesFilaCarregamento(pessoa, unitOfWork);
                SalvarObservacoesCTe(pessoa, unitOfWork);
                SalvarAreasRedex(pessoa, unitOfWork);
                SalvarSuprimentoDeGas(pessoa, unitOfWork);
                SalvarFrequenciaCarregamento(pessoa, unitOfWork);
                SalvarGrupoPessoas(pessoa, unitOfWork);
                SalvarTipoComprovantes(pessoa, unitOfWork);
                SalvarFilialCliente(pessoa, unitOfWork);
                SalvarContasBancarias(pessoa, unitOfWork);

                pessoa.DataUltimaAtualizacao = DateTime.Now;
                pessoa.Integrado = false;

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repPessoa.Atualizar(pessoa, Auditado);

                SalvarConfiguracaoFaturaListasAuditarManual(pessoa, permissoesPersonalizadas, unitOfWork);
                SalvarOutrosCodigosIntegracao(ref pessoa, unitOfWork);
                AtualizarConfiguracoesEmissao(pessoa, configuracao, historico, unitOfWork);
                AtualizarConfiguracoesFatura(pessoa, configuracaoFatura, historico, unitOfWork);

                new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork).GerarIntegracaoPessoa(unitOfWork, pessoa);
                new Servicos.Embarcador.GestaoPallet.RegraPalletHistorico(unitOfWork).SalvarNovaRegraPeriodo(pessoa);

                string mensagem = Localization.Resources.Pessoas.Pessoa.AtualizadoComSucesso;
                unitOfWork.CommitChanges();

                if (ConfiguracaoEmbarcador.AtualizarRotasQuandoAlterarLocalizacaoCliente && (oldLatitude != pessoa.Latitude || oldLongitude != pessoa.Longitude))
                {
                    unitOfWork.Start();
                    int total = new Servicos.Embarcador.Carga.RotaFrete(unitOfWork).AtualizarRotasComCliente(pessoa, TipoServicoMultisoftware);
                    unitOfWork.CommitChanges();
                    if (total > 0) mensagem += Localization.Resources.Pessoas.Pessoa.ForamAtualizadasTotasDevidoAlteracaoDasCoordenadas;
                }

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && codigoAnterior != novoCodigo && !string.IsNullOrWhiteSpace(novoCodigo))
                {
                    Servicos.Log.TratarErro($"Atualização de CAR_CARGA_INTEGRADA_EMBARCADOR para false pelo PessoaController", "AtualizacaoCargaIntegradaEmbarcador");

                    bool atualizarTodos = string.IsNullOrWhiteSpace(codigoAnterior);
                    DbConnection connection = unitOfWork.GetConnection();
                    DbTransaction transaction = connection.BeginTransaction();
                    Servicos.Embarcador.Pessoa.Pessoa.VerificarCargasEmitidasAnteriormente(pessoa.CPF_CNPJ, atualizarTodos, connection, transaction);
                    transaction.Commit();
                }

                return new JsonpResult(true, true, mensagem);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");

                else
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoAtualizar);
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
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("Codigo"));
                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = Double.Parse(cpfcnpj);

                double cpfCnpj1 = Request.GetDoubleParam("Codigo");

                Repositorio.Cliente repGrupoPessoas = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP repClienteIntegracaoFTP = new Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteObservacaoCTe repClienteObservacaoCTe = new Repositorio.Embarcador.Pessoas.ClienteObservacaoCTe(unitOfWork);
                Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao repPessoaExteriorOutraDescricao = new Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteAreaRedex repClienteAreaRedex = new Repositorio.Embarcador.Pessoas.ClienteAreaRedex(unitOfWork);
                Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento repPessoaFaturaVencimento = new Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento(unitOfWork);
                Repositorio.Embarcador.Filiais.SuprimentoDeGas repositorioSuprimentoDeGas = new Repositorio.Embarcador.Filiais.SuprimentoDeGas(unitOfWork);
                Repositorio.Embarcador.Pessoas.PessoaArmador repositorioPessoaArmador = new Repositorio.Embarcador.Pessoas.PessoaArmador(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento repositorioClienteFrequenciaCarregamento = new Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfig = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico repClienteAcrescimoDesconto = new Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                Repositorio.Embarcador.Pessoas.PessoaLicenca repositorioPessoaLicenca = new Repositorio.Embarcador.Pessoas.PessoaLicenca(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteComponente repositorioClienteComponete = new Repositorio.Embarcador.Pessoas.ClienteComponente(unitOfWork);
                Repositorio.Embarcador.Pessoas.PessoaAnexo repositorioPessoaAnexo = new Repositorio.Embarcador.Pessoas.PessoaAnexo(unitOfWork);
                Repositorio.Embarcador.Logistica.SubareaCliente repositorioSubArea = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repositorioClienteOutroEnderecos = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                Repositorio.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor repPortalMultiCliforVendedor = new Repositorio.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repositorioModalidadeTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo repositorioModalidadeDiaFechamentoCIOTPeriodo = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTms = repConfig.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor);
                if (politicaSenha == null)
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                List<Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas> suprimentosDeGas = repositorioSuprimentoDeGas.BuscarPorCliente(dCPFCNPJ);
                Dominio.Entidades.Cliente pessoa = repGrupoPessoas.BuscarPorCPFCNPJ(dCPFCNPJ);

                if (pessoa == null)
                    return new JsonpResult(false, "Pessoa não encontrada");


                object retorno = null;

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe> observacoesCTe = repClienteObservacaoCTe.BuscarPorPessoa(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao> pessoaExteriorOutrasDescricoes = repPessoaExteriorOutraDescricao.BuscarPorPessoa(pessoa.CPF_CNPJ);

                Dominio.Entidades.Usuario usuarioTerceiro = repUsuario.BuscarPorCliente(pessoa.CPF_CNPJ, Dominio.Enumeradores.TipoAcesso.Terceiro);
                Dominio.Entidades.Usuario usuarioFornecedor = repUsuario.BuscarPorClienteFornecedor(pessoa.CPF_CNPJ, Dominio.Enumeradores.TipoAcesso.Fornecedor);

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento> listaVencimentos = repVencimento.BuscarPorPessoa(pessoa.CPF_CNPJ);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, pessoa.CPF_CNPJ);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = null;

                if (modalidadePessoas != null)
                    modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, pessoa.CPF_CNPJ);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = null;
                if (modalidadePessoas != null)
                    modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
                if (modalidadePessoas != null)
                    modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidade(modalidadePessoas.Codigo);

                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(pessoa.CPF_CNPJ);
                Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP clienteIntegracaoFTP = repClienteIntegracaoFTP.BuscarPorClienteAsync(pessoa.CPF_CNPJ, default).Result;

                Dominio.Entidades.DadosCliente dadosCliente = this.Usuario.Empresa != null ? repDadosCliente.Buscar(this.Usuario.Empresa.Codigo, pessoa.CPF_CNPJ) : null;

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> clientesAreaRedex = repClienteAreaRedex.BuscarPorCNPJCPFCliente(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento> pessoaFaturaVencimentos = repPessoaFaturaVencimento.BuscarPorCliente(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador> listaPessoaArmador = repositorioPessoaArmador.BuscarPorPessoa(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> clientesFrequenciaCarregamentos = repositorioClienteFrequenciaCarregamento.BuscarPorCliente(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico> contratoAcrescimoDesconto = repClienteAcrescimoDesconto.BuscarContratoFreteAcrescimoDescontoPorPessoa(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca> licencasPessoa = repositorioPessoaLicenca.BuscarPorPessoa(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> componentes = repositorioClienteComponete.BuscarComponentesPorCliente(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo> anexos = repositorioPessoaAnexo.BuscarPorPessoa(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> subAreasCliente = repositorioSubArea.BuscarPorCliente(pessoa);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clientesOutrosEnderecos = repositorioClienteOutroEnderecos.BuscarPorPessoa(pessoa.CPF_CNPJ);
                List<Dominio.Entidades.Usuario> listaUsuarioTerceiroAdicionais = usuarioTerceiro != null ? repUsuario.BuscarPorClienteTerceiroAdicionais(pessoa.CPF_CNPJ, usuarioTerceiro) : null;
                List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT> tiposPagamentoCIOT = (modalidadeTransportadoraPessoas != null) ? repositorioModalidadeTipoPagamentoCIOT.BuscarPorModalidadeTransportador(modalidadeTransportadoraPessoas.Codigo) : new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>();
                List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo> diasFechamentoCIOTPeriodo = (modalidadeTransportadoraPessoas != null) ? repositorioModalidadeDiaFechamentoCIOTPeriodo.BuscarPorModalidadeTransportador(modalidadeTransportadoraPessoas.Codigo) : new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo>();


                double latCliente = pessoa.Latitude.ToDouble();
                double lngCliente = pessoa.Longitude.ToDouble();
                double latLocalidade = 0;
                if (pessoa != null && pessoa.Localidade != null && pessoa.Localidade.Latitude.HasValue)
                    latLocalidade = (double)(pessoa?.Localidade?.Latitude.Value ?? 0);
                double lngLocalidade = 0;
                if (pessoa != null && pessoa.Localidade != null && pessoa.Localidade.Longitude.HasValue)
                    lngLocalidade = (double)(pessoa?.Localidade?.Longitude.Value ?? 0);

                double raio = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(latLocalidade, lngLocalidade, latCliente, lngCliente) / 1000;

                double latitudeTransbordo = pessoa.LatitudeTransbordo.ToDouble();
                double longitudeTransbordo = pessoa.LongitudeTransbordo.ToDouble();

                int tipoPessoa = 0;
                if (pessoa.Tipo.Equals("J"))
                    tipoPessoa = 1;
                else if (pessoa.Tipo.Equals("F"))
                    tipoPessoa = 0;
                else
                    tipoPessoa = 2;

                string vendedorPortalMulticlifor = string.Empty;

                if (!string.IsNullOrEmpty(usuarioFornecedor?.Login ?? string.Empty))
                    vendedorPortalMulticlifor = repPortalMultiCliforVendedor.BuscarPorUsuarioAcessoPortal(usuarioFornecedor.Login)?.Vendedor ?? string.Empty;

                retorno = new
                {
                    pessoa.NomeNomenclaturaArquivosDownloadCTe,
                    pessoa.Funcionario,
                    pessoa.Motorista,
                    pessoa.NaoAtualizarDados,
                    pessoa.RegimeTributario,
                    CNPJ_CPF = configuracaoTms.Pais == TipoPais.Brasil ? pessoa.CPF_CNPJ_Formatado : pessoa.CPF_CNPJ_SemFormato,
                    CNPJ = configuracaoTms.Pais == TipoPais.Brasil ? pessoa.CPF_CNPJ_Formatado : pessoa.CPF_CNPJ_SemFormato,
                    CPF = configuracaoTms.Pais == TipoPais.Brasil ? pessoa.CPF_CNPJ_Formatado : pessoa.CPF_CNPJ_SemFormato,
                    TipoPessoa = tipoPessoa,
                    pessoa.IE_RG,
                    pessoa.Nome,
                    pessoa.Ativo,
                    Fantasia = pessoa.NomeFantasia,
                    NomeVisoesBI = pessoa.NomeVisoesBI,
                    TelefonePrincipal = pessoa.Telefone1,
                    TelefoneSecundario = pessoa.Telefone2,
                    pessoa.DigitalizaCanhoto,
                    pessoa.NaoEmitirCTeFilialEmissora,
                    pessoa.ObrigarInformarDataEntregaClienteAoBaixarCanhotos,
                    Atividade = pessoa.Atividade != null ? new { pessoa.Atividade.Codigo, pessoa.Atividade.Descricao } : null,
                    Categoria = new
                    {
                        Codigo = pessoa.Categoria?.Codigo ?? 0,
                        Descricao = pessoa.Categoria?.Descricao ?? string.Empty
                    },
                    GrupoPessoas = new
                    {
                        Codigo = pessoa.GrupoPessoas?.Codigo ?? 0,
                        Descricao = pessoa.GrupoPessoas?.Descricao ?? string.Empty,
                        DisponibilizarDocumentosParaLoteEscrituracao = (pessoa.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false),
                        DisponibilizarDocumentosParaLoteEscrituracaoCancelamento = (pessoa.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento ?? false)
                    },
                    Pessoa = new
                    {
                        Codigo = pessoa.CPF_CNPJ_SemFormato,
                        Descricao = pessoa.Nome
                    },
                    pessoa.Endereco,
                    CodigoIntegracao = pessoa.CodigoIntegracao != null ? pessoa.CodigoIntegracao : "",
                    pessoa.Numero,
                    pessoa.Bairro,
                    pessoa.Complemento,
                    CEP = String.Format(@"{0:00\.000\-000}", pessoa.CEP),
                    Localidade = new { Codigo = pessoa.Localidade.Codigo, Descricao = pessoa.Localidade.DescricaoCidadeEstado },
                    pessoa.Email,
                    RG = pessoa.RG_Passaporte,
                    Passaporte = pessoa.RG_Passaporte,
                    NumeroCUITRUT = pessoa.NumeroCUITRUT,
                    TipoCliente = (repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente, pessoa.CPF_CNPJ) != null),
                    TipoFornecedor = modalidadeFornecedorPessoas != null,
                    TipoTransportador = modalidadeTransportadoraPessoas != null,
                    DescontoPadrao = modalidadeTransportadoraPessoas?.PercentualDesconto ?? 0m,
                    PercentualDesconto = modalidadeTransportadoraPessoas?.PercentualCobranca ?? 0m,
                    PercentualAdiantamentoFretesTerceiro = modalidadeTransportadoraPessoas?.PercentualAdiantamentoFretesTerceiro ?? 0m,
                    PercentualAbastecimentoFretesTerceiro = modalidadeTransportadoraPessoas?.PercentualAbastecimentoFretesTerceiro ?? 0m,
                    AliquotaCOFINS = modalidadeTransportadoraPessoas?.AliquotaCOFINS ?? 0m,
                    AliquotaPIS = modalidadeTransportadoraPessoas?.AliquotaPIS ?? 0m,
                    RNTRC = modalidadeTransportadoraPessoas?.RNTRC ?? string.Empty,
                    ObservacaoCTe = modalidadeTransportadoraPessoas?.ObservacaoCTe ?? string.Empty,
                    CodigoIntegracaoTributaria = modalidadeTransportadoraPessoas?.CodigoIntegracaoTributaria ?? string.Empty,
                    pessoa.Observacao,
                    pessoa.EnderecoDigitado,
                    pessoa.TipoLocalizacao,
                    pessoa.TipoLogradouro,
                    pessoa.DescricaoTipoLogradouro,
                    pessoa.TipoEndereco,
                    pessoa.TipoEmail,
                    pessoa.Bloqueado,
                    pessoa.MotivoBloqueio,
                    pessoa.PossuiRestricaoTrafego,
                    pessoa.PontoTransbordo,
                    pessoa.ExcecaoCheckinFilaH,
                    pessoa.AguardandoConferenciaInformacao,
                    EnviarEmail = pessoa.EmailStatus == "A" ? true : false,
                    pessoa.NaoUsarConfiguracaoFaturaGrupo,
                    pessoa.NaoUsarConfiguracaoEmissaoGrupo,
                    RateioFormula = pessoa.RateioFormula != null ? new { pessoa.RateioFormula.Codigo, pessoa.RateioFormula.Descricao } : null,
                    pessoa.TipoEmissaoCTeDocumentos,
                    pessoa.Latitude,
                    pessoa.Longitude,
                    pessoa.GeoLocalizacaoRaioLocalidade,
                    RaioGeoLocalizacaoLocalidade = raio,
                    LatitudeTransbordo = latitudeTransbordo == 0 ? "" : latitudeTransbordo.ToString(),
                    LongitudeTransbordo = longitudeTransbordo == 0 ? "" : longitudeTransbordo.ToString(),
                    pessoa.RaioEmMetros,
                    pessoa.AtualizarPontoApoioMaisProximoAutomaticamente,
                    PontoDeApoio = new
                    {
                        Codigo = pessoa.PontoDeApoio?.Codigo ?? 0,
                        Descricao = pessoa.PontoDeApoio?.Descricao ?? ""
                    },
                    pessoa.Area,
                    pessoa.TipoArea,
                    pessoa.AlvoEstrategico,
                    pessoa.LerVeiculoObservacaoNotaParaAbastecimento,
                    pessoa.LerPlacaObservacaoNotaParaAbastecimentoInicial,
                    pessoa.LerPlacaObservacaoNotaParaAbastecimentoFinal,
                    pessoa.LerKMObservacaoNotaParaAbastecimentoInicial,
                    pessoa.LerKMObservacaoNotaParaAbastecimentoFinal,
                    pessoa.LerHorimetroObservacaoNotaParaAbastecimentoInicial,
                    pessoa.LerHorimetroObservacaoNotaParaAbastecimentoFinal,
                    pessoa.LerChassiObservacaoNotaParaAbastecimentoInicial,
                    pessoa.LerChassiObservacaoNotaParaAbastecimentoFinal,
                    pessoa.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe,
                    pessoa.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa,
                    pessoa.ExigirComprovantesLiberacaoPagamentoContratoFrete,

                    UsuarioPortal = usuarioFornecedor?.Login ?? string.Empty,
                    SenhaUsuarioPortal = politicaSenha?.HabilitarPoliticaSenha ?? false ? "" : usuarioFornecedor?.Senha ?? string.Empty,

                    AcessoPortal = new
                    {
                        AtivarAcessoPortal = pessoa.AtivarAcessoFornecedor,
                        CompartilharAcessoEntreGrupoPessoas = pessoa.CompartilharAcessoEntreGrupoPessoas,
                        VisualizarApenasParaPedidoDesteTomador = pessoa.VisualizarApenasParaPedidoDesteTomador,
                        VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas = pessoa.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas,
                        DesabilitarCancelamentoAgendamentoColeta = pessoa.DesabilitarCancelamentoAgendamentoColeta,
                        Usuario = usuarioFornecedor?.Login ?? string.Empty,
                        Senha = politicaSenha?.HabilitarPoliticaSenha ?? false ? "" : usuarioFornecedor?.Senha ?? string.Empty,
                        ConfirmaSenha = politicaSenha?.HabilitarPoliticaSenha ?? false ? "" : usuarioFornecedor?.Senha ?? string.Empty,
                        CodigoUsuario = usuarioFornecedor?.Codigo,
                        Vendedor = vendedorPortalMulticlifor,
                        HabilitarFornecedorParaLancamentoOrdemServico = pessoa?.HabilitarFornecedorParaLancamentoOrdemServico ?? false,
                    },
                    Fornecedor = modalidadeFornecedorPessoas != null ? new
                    {
                        CodigoFornecedor = modalidadeFornecedorPessoas.Codigo,
                        pessoa.GerarDuplicataNotaEntrada,
                        pessoa.SempreConsiderarValorOrcadoFechamentoOrdemServico,
                        pessoa.DiaPadraoDuplicataNotaEntrada,
                        pessoa.ParcelasDuplicataNotaEntrada,
                        pessoa.IntervaloDiasDuplicataNotaEntrada,
                        pessoa.IgnorarDuplicataRecebidaXMLNotaEntrada,
                        pessoa.CodigoIntegracaoDuplicataNotaEntrada,
                        modalidadeFornecedorPessoas.PostoConveniado,
                        AvisoFornecedor = modalidadeFornecedorPessoas.TextoAviso,
                        MultiplosVencimentos = modalidadeFornecedorPessoas.PermitirMultiplosVencimentos,
                        modalidadeFornecedorPessoas.PagoPorFatura,
                        modalidadeFornecedorPessoas.Oficina,
                        modalidadeFornecedorPessoas.PermiteDownloadDocumentos,
                        modalidadeFornecedorPessoas.EnviarEmailFornecedorDadosTransporte,
                        modalidadeFornecedorPessoas.NaoEObrigatorioInformarNfeNaColeta,
                        modalidadeFornecedorPessoas.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento,
                        modalidadeFornecedorPessoas.GerarAgendamentoSomentePedidosExistentes,
                        TipoOperacoes = modalidadeFornecedorPessoas.TipoOperacoes.Select(o => new
                        {
                            o.Codigo,
                            o.Descricao
                        }).ToList(),
                        Transportadores = modalidadeFornecedorPessoas.Transportadores.Select(o => new
                        {
                            o.Codigo,
                            o.Descricao
                        }).ToList(),
                        TipoCargas = modalidadeFornecedorPessoas.TipoCargas.Select(o => new
                        {
                            o.Codigo,
                            o.Descricao
                        }).ToList(),
                        ModelosVeicular = modalidadeFornecedorPessoas.ModelosVeicular.Select(o => new
                        {
                            o.Codigo,
                            o.Descricao
                        }).ToList(),
                        RestricaoModelosVeicular = modalidadeFornecedorPessoasRestricaoModeloVeicular.Select(o => new
                        {
                            o.Codigo,
                            CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                            DescricaoModeloVeicular = o.ModeloVeicular.Descricao,
                            CodigoTipoOperacao = o.TipoOperacao?.Codigo ?? 0,
                            DescricaoTipoOperacao = o.TipoOperacao?.Descricao ?? ""
                        }).ToList(),
                        Destinatarios = modalidadeFornecedorPessoas.Destinatarios.Select(o => new
                        {
                            o.Codigo,
                            o.Descricao
                        }).ToList(),
                        TipoOficina = modalidadeFornecedorPessoas.TipoOficina ?? TipoOficina.Interna,
                        EmpresaOficina = new { Codigo = modalidadeFornecedorPessoas.EmpresaOficina?.Codigo ?? 0, Descricao = modalidadeFornecedorPessoas.EmpresaOficina?.Descricao ?? string.Empty },
                        pessoa.CodigoDocumentoFornecedor,
                        Anexos = (from anexo in anexos
                                  select new
                                  {
                                      anexo.Codigo,
                                      anexo.Descricao,
                                      anexo.NomeArquivo,
                                  }).ToList(),
                        TabelaMultiplosVencimentos = (
                            from vencimento in listaVencimentos
                            select new
                            {
                                DataEmissao = vencimento.DataEmissao.ToString(),
                                Codigo = vencimento.Codigo.ToString(),
                                Vencimento = vencimento.Vencimento.ToString(),
                                DiaEmissaoFinal = vencimento.DiaEmissaoFinal.ToString(),
                                DiaEmissaoInicial = vencimento.DiaEmissaoInicial.ToString()
                            }
                        ).ToList(),
                        TabelaValores = modalidadeFornecedorPessoas.TabelasValores != null ? (from p in modalidadeFornecedorPessoas.TabelasValores
                                                                                              select new
                                                                                              {
                                                                                                  p.Codigo,
                                                                                                  Produto = p.Produto.Descricao,
                                                                                                  CodigoProduto = p.Produto.Codigo,
                                                                                                  UnidadeDeMedida = p.DescricaoUnidadeDeMedida,
                                                                                                  CodigoIntegracao = p.CodigoIntegracao,
                                                                                                  CodigoUnidadeDeMedida = p.UnidadeDeMedida,
                                                                                                  ValorFixo = p.ValorFixo.ToString("n4"),
                                                                                                  ValorAte = p.ValorAte.HasValue ? p.ValorAte.Value.ToString("n4") : string.Empty,
                                                                                                  PercentualDesconto = p.PercentualDesconto.ToString("n4"),
                                                                                                  DataInicial = p.DataInicial.HasValue ? p.DataInicial.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                  DataFinal = p.DataFinal.HasValue ? p.DataFinal.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                  MoedaCotacaoBancoCentral = p.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real,
                                                                                                  ValorMoedaCotacao = p.ValorMoedaCotacao.ToString("n10"),
                                                                                                  ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2")
                                                                                              }).ToList() : null,
                        TipoMovimentoTituloPagar = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? null :
                            dadosCliente != null ? dadosCliente.TipoMovimentoTituloPagar != null ? new { dadosCliente.TipoMovimentoTituloPagar.Codigo, dadosCliente.TipoMovimentoTituloPagar.Descricao } : null : null,
                        pessoa.FormaTituloFornecedor
                    } : null,
                    ConfiguracaoLayoutEDI = (from obj in pessoa.LayoutsEDI
                                             orderby obj.LayoutEDI.Descricao
                                             select new
                                             {
                                                 obj.Codigo,
                                                 CodigoLayoutEDI = obj.LayoutEDI.Codigo,
                                                 DescricaoLayoutEDI = obj.LayoutEDI.Descricao,
                                                 TipoIntegracao = obj.TipoIntegracao.Tipo,
                                                 DescricaoTipoIntegracao = obj.TipoIntegracao.Descricao,
                                                 obj.Diretorio,
                                                 obj.EmailsAlertaLeituraEDI,
                                                 obj.Emails,
                                                 obj.EnderecoFTP,
                                                 obj.Passivo,
                                                 obj.UtilizarSFTP,
                                                 obj.SSL,
                                                 obj.UtilizarLeituraArquivos,
                                                 obj.AdicionarEDIFilaProcessamento,
                                                 obj.CriarComNomeTemporaraio,
                                                 obj.Porta,
                                                 obj.Senha,
                                                 obj.Usuario
                                             }).ToList(),
                    Rota = (from obj in pessoa.Rotas
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                            }).ToList(),
                    ConfiguracaoEmissaoCTe = new
                    {
                        TipoIntegracaoMercadoLivre = pessoa.TipoIntegracaoMercadoLivre ?? TipoIntegracaoMercadoLivre.HandlingUnit,
                        pessoa.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente,
                        pessoa.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente,
                        pessoa.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida,
                        TempoAcrescimoDecrescimoDataPrevisaoSaida = $"{(int)pessoa.TempoAcrescimoDecrescimoDataPrevisaoSaida.TotalHours:d3}:{pessoa.TempoAcrescimoDecrescimoDataPrevisaoSaida:mm}",
                        pessoa.DisponibilizarDocumentosParaNFsManual,
                        pessoa.ValorFreteLiquidoDeveSerValorAReceber,
                        pessoa.ValorFreteLiquidoDeveSerValorAReceberSemICMS,
                        pessoa.GerarCIOTParaTodasAsCargas,
                        ValorMaximoEmissaoPendentePagamento = (pessoa.ValorMaximoEmissaoPendentePagamento ?? 0m).ToString("n2"),
                        ValorLimiteFaturamento = (pessoa.ValorLimiteFaturamento ?? 0m).ToString("n2"),
                        DiasEmAbertoAposVencimento = (pessoa.DiasEmAbertoAposVencimento ?? 0).ToString("n0"),
                        TipoEnvioEmail = pessoa.TipoEnvioEmail ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe.Normal,
                        pessoa.ObservacaoEmissaoCarga,
                        pessoa.DescricaoItemPesoCTeSubcontratacao,
                        pessoa.CaracteristicaTransporteCTe,
                        pessoa.ImportarRedespachoIntermediario,
                        EmitenteImportacaoRedespachoIntermediario = new
                        {
                            Codigo = pessoa.EmitenteImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                            Descricao = pessoa.EmitenteImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                        },
                        ExpedidorImportacaoRedespachoIntermediario = new
                        {
                            Codigo = pessoa.ExpedidorImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                            Descricao = pessoa.ExpedidorImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                        },
                        RecebedorImportacaoRedespachoIntermediario = new
                        {
                            Codigo = pessoa.RecebedorImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                            Descricao = pessoa.RecebedorImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                        },
                        pessoa.GerarMDFeTransbordoSemConsiderarOrigem,
                        pessoa.BloquearDiferencaValorFreteEmbarcador,
                        PercentualBloquearDiferencaValorFreteEmbarcador = pessoa.PercentualBloquearDiferencaValorFreteEmbarcador.ToString("n2"),
                        pessoa.EmitirComplementoDiferencaFreteEmbarcador,
                        TipoOcorrenciaComplementoDiferencaFreteEmbarcador = new
                        {
                            Descricao = pessoa.TipoOcorrenciaComplementoDiferencaFreteEmbarcador?.Descricao ?? string.Empty,
                            Codigo = pessoa.TipoOcorrenciaComplementoDiferencaFreteEmbarcador?.Codigo ?? 0
                        },
                        pessoa.GerarOcorrenciaSemTabelaFrete,
                        TipoOcorrenciaSemTabelaFrete = new
                        {
                            Descricao = pessoa.TipoOcorrenciaSemTabelaFrete?.Descricao ?? string.Empty,
                            Codigo = pessoa.TipoOcorrenciaSemTabelaFrete?.Codigo ?? 0
                        },
                        TipoOcorrenciaCTeEmitidoEmbarcador = new
                        {
                            Descricao = pessoa.TipoOcorrenciaCTeEmitidoEmbarcador?.Descricao ?? string.Empty,
                            Codigo = pessoa.TipoOcorrenciaCTeEmitidoEmbarcador?.Codigo ?? 0
                        },
                        pessoa.NaoEmitirMDFe,
                        pessoa.ProvisionarDocumentos,
                        pessoa.DisponibilizarDocumentosParaLoteEscrituracao,
                        pessoa.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento,
                        pessoa.DisponibilizarDocumentosParaPagamento,
                        pessoa.QuitarDocumentoAutomaticamenteAoGerarLote,
                        pessoa.EscriturarSomenteDocumentosEmitidosParaNFe,
                        pessoa.NaoValidarNotaFiscalExistente,
                        pessoa.NaoValidarNotasFiscaisComDiferentesPortos,
                        pessoa.AgruparMovimentoFinanceiroPorPedido,
                        pessoa.ValePedagioObrigatorio,
                        Diretorio = clienteIntegracaoFTP?.Diretorio ?? string.Empty,
                        EnderecoFTP = clienteIntegracaoFTP?.EnderecoFTP ?? string.Empty,
                        Passivo = clienteIntegracaoFTP?.Passivo ?? false,
                        UtilizarSFTP = clienteIntegracaoFTP?.UtilizarSFTP ?? false,
                        SSL = clienteIntegracaoFTP?.SSL ?? false,
                        Porta = clienteIntegracaoFTP?.Porta ?? string.Empty,
                        Senha = clienteIntegracaoFTP?.Senha ?? string.Empty,
                        Usuario = clienteIntegracaoFTP?.Usuario ?? string.Empty,
                        NomenclaturaArquivo = clienteIntegracaoFTP?.NomenclaturaArquivo ?? string.Empty,
                        ModeloDocumentoFiscal = pessoa.ModeloDocumentoFiscal != null ? new { pessoa.ModeloDocumentoFiscal.Codigo, pessoa.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                        EmpresaEmissora = pessoa.EmpresaEmissora != null ? new { pessoa.EmpresaEmissora.Codigo, Descricao = pessoa.EmpresaEmissora.RazaoSocial + " (" + pessoa.EmpresaEmissora.Localidade.DescricaoCidadeEstado + ")" } : new { Codigo = 0, Descricao = "" },
                        EmitirEmpresaFixa = pessoa.EmpresaEmissora != null ? true : false,
                        CobrarOutroDocumento = pessoa.ModeloDocumentoFiscal != null ? true : false,
                        TipoRateioDocumentos = pessoa.TipoEmissaoCTeDocumentos,
                        pessoa.CTeEmitidoNoEmbarcador,
                        pessoa.ExigirNumeroPedido,
                        pessoa.RegexValidacaoNumeroPedidoEmbarcador,
                        TipoEmissaoCTeParticipantes = pessoa.TipoEmissaoCTeParticipantes,
                        TipoEmissaoIntramunicipal = pessoa.TipoEmissaoIntramunicipal,
                        pessoa.DescricaoComponenteFreteEmbarcador,
                        pessoa.UtilizarOutroModeloDocumentoEmissaoMunicipal,
                        pessoa.GerarOcorrenciaComplementoSubcontratacao,
                        pessoa.ObrigatorioInformarMDFeEmitidoPeloEmbarcador,
                        TipoOcorrenciaComplementoSubcontratacao = new
                        {
                            Codigo = pessoa.TipoOcorrenciaComplementoSubcontratacao?.Codigo ?? 0,
                            Descricao = pessoa.TipoOcorrenciaComplementoSubcontratacao?.Descricao
                        },
                        ModeloDocumentoFiscalEmissaoMunicipal = new
                        {
                            Codigo = pessoa.ModeloDocumentoFiscalEmissaoMunicipal?.Codigo ?? 0,
                            Descricao = pessoa.ModeloDocumentoFiscalEmissaoMunicipal?.Descricao ?? string.Empty
                        },
                        ArquivoImportacaoNotasFiscais = new
                        {
                            Codigo = pessoa.ArquivoImportacaoNotaFiscal?.Codigo ?? 0,
                            Descricao = pessoa.ArquivoImportacaoNotaFiscal?.Descricao ?? string.Empty
                        },
                        FormulaRateioFrete = new
                        {
                            Codigo = pessoa.RateioFormula?.Codigo ?? 0,
                            Descricao = pessoa.RateioFormula?.Descricao ?? string.Empty
                        },
                        TipoIntegracao = pessoa.TipoIntegracao?.Tipo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                        ApolicesSeguro = (from obj in pessoa.ApolicesSeguro
                                          select new
                                          {
                                              obj.Codigo,
                                              Seguradora = obj.Seguradora.Nome,
                                              obj.NumeroApolice,
                                              obj.NumeroAverbacao,
                                              Responsavel = obj.DescricaoResponsavel,
                                              Vigencia = obj.InicioVigencia.ToString("dd/MM/yyyy") + " até " + obj.FimVigencia.ToString("dd/MM/yyyy")
                                          }).ToList(),
                        ComponentesFrete = (from obj in pessoa.ClienteConfiguracoesComponentes
                                            select new
                                            {
                                                Codigo = obj.ComponenteFrete.Codigo,
                                                ComponenteFrete = new { obj.ComponenteFrete.Codigo, obj.ComponenteFrete.Descricao },
                                                CobrarOutroDocumento = obj.ModeloDocumentoFiscal != null ? true : false,
                                                ModeloDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? new { obj.ModeloDocumentoFiscal.Codigo, obj.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                                                ImprimirOutraDescricaoCTe = !string.IsNullOrEmpty(obj.OutraDescricaoCTe) ? true : false,
                                                DescricaoCTe = obj.OutraDescricaoCTe,
                                                UsarOutraFormulaRateio = obj.RateioFormula != null ? true : false,
                                                FormulaRateioFrete = obj.RateioFormula != null ? new { obj.RateioFormula.Codigo, obj.RateioFormula.Descricao } : new { Codigo = 0, Descricao = "" },
                                                obj.IncluirICMS,
                                                obj.IncluirIntegralmenteContratoFreteTerceiro
                                            }).ToList(),
                        ClientesBloqueados = (from obj in pessoa.ClientesBloquearEmissaoDosDestinatario
                                              select new
                                              {
                                                  obj.Codigo,
                                                  CPF_CNPJ = obj.CPF_CNPJ,
                                                  obj.Nome
                                              }).ToList(),
                        pessoa.TipoPropostaMultimodal,
                        pessoa.TipoServicoMultimodal,
                        pessoa.ModalPropostaMultimodal,
                        pessoa.TipoCobrancaMultimodal,
                        pessoa.BloquearEmissaoDeEntidadeSemCadastro,
                        pessoa.BloquearEmissaoDosDestinatario,
                        Observacao = pessoa.ObservacaoCTe,
                        ObservacaoTerceiro = pessoa.ObservacaoCTeTerceiro,
                        pessoa.NaoPermitirVincularCTeComplementarEmCarga,
                        TempoCarregamento = $"{(int)pessoa.TempoCarregamento.TotalHours:d3}:{pessoa.TempoCarregamento:mm}",
                        TempoDescarregamento = $"{(int)pessoa.TempoDescarregamento.TotalHours:d3}:{pessoa.TempoDescarregamento:mm}",
                        UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = pessoa.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false
                    },
                    ListaEmail = (from obj in pessoa.Emails
                                  orderby obj.Email
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      Email = obj.Email,
                                      EnviarEmail = obj.EmailStatus == "A" ? true : false,
                                      TipoEmail = obj.TipoEmail,
                                      DescricaoTipo = obj.DescricaoTipoEmail
                                  }).ToList(),
                    SuprimentoGas = new
                    {
                        HabilitarSolicitacao = pessoa.HabilitarSolicitacaoSuprimentoDeGas,
                        ListaSuprimentosGas = RetornaDynSuprimentoDeGas(suprimentosDeGas)
                    },
                    ListaEndereco = (from obj in clientesOutrosEnderecos
                                     orderby obj.Localidade.Descricao
                                     select new
                                     {
                                         Codigo = obj.Codigo,
                                         Bairro = !string.IsNullOrWhiteSpace(obj.Bairro) ? obj.Bairro : "",
                                         CEP = !string.IsNullOrWhiteSpace(obj.CEP) ? obj.CEP : "",
                                         Complemento = !string.IsNullOrWhiteSpace(obj.Complemento) ? obj.Complemento : "",
                                         CodigoDocumento = !string.IsNullOrWhiteSpace(obj.CodigoDocumento) ? obj.CodigoDocumento : "",
                                         Endereco = !string.IsNullOrWhiteSpace(obj.Endereco) ? obj.Endereco : "",
                                         obj.EnderecoDigitado,
                                         Latitude = obj?.Latitude ?? "",
                                         Longitude = obj?.Longitude ?? "",
                                         Numero = !string.IsNullOrWhiteSpace(obj.Numero) ? obj.Numero : "",
                                         Localidade = obj.Localidade.DescricaoCidadeEstado,
                                         CodigoLocalidade = obj.Localidade.Codigo,
                                         DescricaoLocalidade = obj.Localidade.Descricao,
                                         obj.TipoEndereco,
                                         obj.TipoLogradouro,
                                         RaioEmMetrosSecundario = obj?.RaioEmMetros ?? 0,
                                         AreaSecundario = !string.IsNullOrWhiteSpace(obj.Area) ? obj.Area : "",
                                         TipoAreaEnderecoSecundario = obj.TipoArea,
                                         IE = !string.IsNullOrWhiteSpace(obj.IE_RG) ? obj.IE_RG : "",
                                         CodigoIntegracao = !string.IsNullOrWhiteSpace(obj.CodigoEmbarcador) ? obj.CodigoEmbarcador : "",
                                         Telefone = !string.IsNullOrWhiteSpace(obj.Telefone) ? obj.Telefone.ObterTelefoneFormatado() : ""
                                     }).ToList(),
                    ListaDocumento = (from obj in pessoa.Documentos
                                      orderby obj.DataVencimento
                                      select new
                                      {
                                          Codigo = obj.Codigo,
                                          obj.Descricao,
                                          DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                          DataVencimento = obj.DataVencimento.HasValue ? obj.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty
                                      }).ToList(),
                    UsuarioTerceiro = usuarioTerceiro != null ? new
                    {
                        Usuario = usuarioTerceiro.Login,
                        Senha = usuarioTerceiro.Senha,
                        ConfirmaSenha = usuarioTerceiro.Senha
                    } : null,
                    TransportadorTerceiro = modalidadeTransportadoraPessoas != null ? new
                    {
                        DiasVencimentoAdiantamentoContratoFrete = modalidadeTransportadoraPessoas.DiasVencimentoAdiantamentoContratoFrete?.ToString() ?? string.Empty,
                        DiasVencimentoSaldoContratoFrete = modalidadeTransportadoraPessoas.DiasVencimentoSaldoContratoFrete?.ToString() ?? string.Empty,
                        modalidadeTransportadoraPessoas.TextoAdicionalContratoFrete,
                        modalidadeTransportadoraPessoas.ReterImpostosContratoFrete,
                        modalidadeTransportadoraPessoas.NaoSomarValorPedagioContratoFrete,
                        modalidadeTransportadoraPessoas.QuantidadeDependentes,
                        PercentualCobradoPadrao = modalidadeTransportadoraPessoas.PercentualCobranca > 0 ? modalidadeTransportadoraPessoas.PercentualCobranca.ToString("n2") : "",
                        DescontoPadrao = modalidadeTransportadoraPessoas.PercentualDesconto > 0 ? modalidadeTransportadoraPessoas.PercentualDesconto.ToString("n4") : "",
                        modalidadeTransportadoraPessoas.RNTRC,
                        PercentualAdiantamentoFretesTerceiro = modalidadeTransportadoraPessoas.PercentualAdiantamentoFretesTerceiro > 0m ? modalidadeTransportadoraPessoas.PercentualAdiantamentoFretesTerceiro.ToString("n2") : "",
                        PercentualAbastecimentoFretesTerceiro = modalidadeTransportadoraPessoas.PercentualAbastecimentoFretesTerceiro > 0m ? modalidadeTransportadoraPessoas.PercentualAbastecimentoFretesTerceiro.ToString("n2") : "",
                        AliquotaCOFINS = modalidadeTransportadoraPessoas.AliquotaCOFINS > 0m ? modalidadeTransportadoraPessoas.AliquotaCOFINS.ToString("n4") : "",
                        modalidadeTransportadoraPessoas.CodigoIntegracaoTributaria,
                        AliquotaPIS = modalidadeTransportadoraPessoas.AliquotaPIS > 0m ? modalidadeTransportadoraPessoas.AliquotaPIS.ToString("n4") : "",
                        modalidadeTransportadoraPessoas.ObservacaoCTe,
                        modalidadeTransportadoraPessoas.GerarCIOT,
                        PagamentoMotoristaTipo = new { Codigo = modalidadeTransportadoraPessoas.PagamentoMotoristaTipo?.Codigo ?? 0, Descricao = modalidadeTransportadoraPessoas.PagamentoMotoristaTipo?.Descricao ?? "" },
                        modalidadeTransportadoraPessoas.GerarPagamentoTerceiro,
                        TipoTransportadorTerceiro = modalidadeTransportadoraPessoas?.TipoTransportador ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado,
                        modalidadeTransportadoraPessoas.ObservacaoContratoFrete,
                        modalidadeTransportadoraPessoas.ExigeCanhotoFechamentoContratoFrete,
                        modalidadeTransportadoraPessoas.HabilitarDataFixaVencimento,
                        modalidadeTransportadoraPessoas.CodigoINSS,
                        TipoFavorecidoCIOT = modalidadeTransportadoraPessoas.TipoFavorecidoCIOT?.ToString("D") ?? string.Empty,
                        TipoPagamentoCIOT = modalidadeTransportadoraPessoas.TipoPagamentoCIOT?.ToString("D") ?? string.Empty,
                        modalidadeTransportadoraPessoas.NumeroCartao,
                        DataEmissaoRNTRC = modalidadeTransportadoraPessoas.DataEmissaoRNTRC?.ToString("dd/MM/yyyy"),
                        DataVencimentoRNTRC = modalidadeTransportadoraPessoas.DataVencimentoRNTRC?.ToString("dd/MM/yyyy"),
                        TipoGeracaoCIOT = modalidadeTransportadoraPessoas.TipoGeracaoCIOT?.ToString("D") ?? string.Empty,
                        TipoQuitacaoCIOT = modalidadeTransportadoraPessoas.TipoQuitacaoCIOT?.ToString("D") ?? string.Empty,
                        TipoAdiantamentoCIOT = modalidadeTransportadoraPessoas.TipoAdiantamentoCIOT?.ToString("D") ?? string.Empty,
                        TipoPagamentoContratoFreteTerceiro = modalidadeTransportadoraPessoas.TipoPagamentoContratoFreteTerceiro?.ToString("D") ?? string.Empty,
                        CodigoProvedor = modalidadeTransportadoraPessoas.CodigoProvedor,
                        ConfiguracaoCIOT = new { Codigo = modalidadeTransportadoraPessoas.ConfiguracaoCIOT?.Codigo ?? 0, Descricao = modalidadeTransportadoraPessoas.ConfiguracaoCIOT?.Descricao ?? "" },
                        TipoTerceiro = new { Codigo = modalidadeTransportadoraPessoas.TipoTerceiro?.Codigo ?? 0, Descricao = modalidadeTransportadoraPessoas.TipoTerceiro?.Descricao ?? "" },
                        TiposPagamentoCIOTOperadora = (from obj in tiposPagamentoCIOT
                                                       select new
                                                       {
                                                           obj.Codigo,
                                                           obj.TipoPagamentoCIOT,
                                                           OperadoraCIOT = obj.Operadora,
                                                           DescricaoTipoPagamentoCIOT = obj.TipoPagamentoCIOT.ObterDescricao(),
                                                           DescricaoOperadoraCIOT = obj.Operadora.ObterDescricao(),
                                                       }).ToList(),
                        DiasFechamentoCIOTPeriodo = (from obj in diasFechamentoCIOTPeriodo
                                                     select new
                                                     {
                                                         obj.Codigo,
                                                         obj.DiaFechamentoCIOT,
                                                     }).ToList(),
                    } : null,
                    DadosBancarios = new
                    {
                        ClientePortadorConta = pessoa.ClientePortadorConta != null ? new { Codigo = pessoa.ClientePortadorConta.CPF_CNPJ, Descricao = $"{pessoa.ClientePortadorConta.Nome} - {pessoa.ClientePortadorConta.CPF_CNPJ_Formatado}" } : null,
                        Banco = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ?
                            pessoa.Banco != null ? new { Codigo = pessoa.Banco.Codigo, Descricao = pessoa.Banco.Descricao } : null :
                            dadosCliente != null ? dadosCliente.Banco != null ? new { Codigo = dadosCliente.Banco.Codigo, Descricao = dadosCliente.Banco.Descricao } : null : null,
                        Agencia = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? pessoa.Agencia :
                            dadosCliente != null ? dadosCliente.Agencia : string.Empty,
                        Digito = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? pessoa.DigitoAgencia :
                            dadosCliente != null ? dadosCliente.DigitoAgencia : string.Empty,
                        NumeroConta = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? pessoa.NumeroConta :
                            dadosCliente != null ? dadosCliente.NumeroConta : string.Empty,
                        TipoConta = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ?
                            pessoa.TipoContaBanco != null ? (int)pessoa.TipoContaBanco : (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente :
                            dadosCliente != null ? dadosCliente.TipoConta != null ? (int)dadosCliente.TipoConta : (int)Dominio.ObjetosDeValor.Enumerador.TipoConta.Corrente :
                            (int)Dominio.ObjetosDeValor.Enumerador.TipoConta.Corrente,
                        pessoa.CnpjIpef,
                        pessoa.TipoChavePix,
                        pessoa.ChavePix,
                        pessoa.CodigoIntegracaoDadosBancarios,
                        pessoa.UtilizarCadastroContaBancaria,
                        ContasBancarias = (from obj in pessoa.ContasBancarias
                                           select new
                                           {
                                               Codigo = obj.Codigo,
                                               Descricao = (obj.Banco?.Descricao ?? "") + " / " + (obj.NumeroConta ?? "")
                                           }).ToList(),
                    },
                    OutrosCodigosIntegracao = (
                        from obj in pessoa.OutrosCodigosIntegracao
                        select new
                        {
                            Codigo = obj,
                            CodigoIntegracao = obj
                        }
                        ).ToList(),
                    FrequenciasCarregamento = clientesFrequenciaCarregamentos.GroupBy(
                        obj => obj.Empresa).Select(obj => new
                        {
                            Codigo = obj.Key.Codigo,
                            Transportador = new { Codigo = obj.Key.Codigo, Descricao = obj.Key.RazaoSocial },
                            DiaSemana = "[" + string.Join(",", obj.Select(a => a.DiaSemana).Select(o => (int)o).ToList()) + "]"
                        }
                        ).ToList(),

                    AcrescimoDescontoAutomatico = (from obj in contratoAcrescimoDesconto
                                                   select new
                                                   {
                                                       obj.Codigo,
                                                       obj.Descricao,
                                                       Justificativa = new { Descricao = obj.Justificativa.Descricao, Codigo = obj.Justificativa.Codigo },
                                                       obj.Valor,
                                                       TipoValor = obj.TipoValor.ObterDescricao(),
                                                       TipoCalculo = obj.TipoCalculo?.ObterDescricao(),
                                                       obj.Observacoes,
                                                   }).ToList(),
                    Descarga = clienteDescarga != null ? new
                    {
                        ValorPorPallet = clienteDescarga.ValorPorPallet.ToString("n2"),
                        ValorPorVolume = clienteDescarga.ValorPorVolume.ToString("n3"),
                        RestricoesDescarga = (from obj in clienteDescarga.RestricoesDescarga
                                              select new
                                              {
                                                  obj.Codigo,
                                                  obj.Descricao,
                                              }).ToList(),
                        Distribuidor = new { Codigo = clienteDescarga.Distribuidor?.Codigo ?? 0, Descricao = clienteDescarga.Distribuidor?.Descricao ?? "" },
                        VeiculoDistribuidor = new { Codigo = clienteDescarga.VeiculoDistribuidor?.Codigo ?? 0, Descricao = clienteDescarga.VeiculoDistribuidor?.Descricao ?? "" },
                        clienteDescarga.HoraLimiteDescarga,
                        clienteDescarga.NaoRecebeCargaCompartilhada,
                        clienteDescarga.NaoExigePreenchimentoDeChecklistEntrega,
                        FilialResponsavelRedespacho = new { Codigo = clienteDescarga.FilialResponsavelRedespacho?.Codigo ?? 0, Descricao = clienteDescarga.FilialResponsavelRedespacho?.Descricao ?? "" },
                        TipoDeCarga = new { Codigo = clienteDescarga.TipoDeCarga?.Codigo ?? 0, Descricao = clienteDescarga.TipoDeCarga?.Descricao ?? "" },
                        clienteDescarga.AgendamentoExigeNotaFiscal,
                        clienteDescarga.ExigeAgendamento,
                        clienteDescarga.FormaAgendamento,
                        clienteDescarga.TempoAgendamento,
                        clienteDescarga.LinkParaAgendamento,
                        clienteDescarga.AgendamentoDescargaObrigatorio,
                        clienteDescarga.PossuiCanhotoDeDuasOuMaisPaginas,
                        clienteDescarga.QuantidadeDePaginasDoCanhoto,
                        clienteDescarga.ExigeSenhaNoAgendamento,
                    } : null,

                    ConfiguracaoFatura = new
                    {
                        PermiteFinalSemana = pessoa.PermiteFinalDeSemana,
                        TipoPrazoFaturamento = pessoa.TipoPrazoFaturamento,
                        FormaGeracaoTituloFatura = pessoa.FormaGeracaoTituloFatura,
                        DiasDePrazoFatura = pessoa.DiasDePrazoFatura,
                        ExigeCanhotoFisico = pessoa.ExigeCanhotoFisico,
                        ArmazenaCanhotoFisicoCTe = pessoa.ArmazenaCanhotoFisicoCTe ?? false,
                        SomenteOcorrenciasFinalizadoras = pessoa.SomenteOcorrenciasFinalizadoras,
                        FaturarSomenteOcorrenciasFinalizadoras = pessoa.FaturarSomenteOcorrenciasFinalizadoras,
                        NaoGerarFaturaAteReceberCanhotos = pessoa.NaoGerarFaturaAteReceberCanhotos,
                        Banco = new
                        {
                            Codigo = pessoa.Banco?.Codigo ?? 0,
                            Descricao = pessoa.Banco?.Descricao ?? string.Empty
                        },
                        Agencia = pessoa.Agencia,
                        Digito = pessoa.DigitoAgencia,
                        NumeroConta = pessoa.NumeroConta,
                        TipoConta = pessoa.TipoContaBanco,
                        TomadorFatura = new
                        {
                            Codigo = pessoa.ClienteTomadorFatura?.CPF_CNPJ ?? 0d,
                            Descricao = pessoa.ClienteTomadorFatura?.Nome ?? string.Empty
                        },
                        ObservacaoFatura = pessoa.ObservacaoFatura,
                        FormaPagamento = new
                        {
                            Codigo = pessoa.FormaPagamento?.Codigo ?? 0,
                            Descricao = pessoa.FormaPagamento?.Descricao ?? string.Empty
                        },
                        GerarTituloPorDocumentoFiscal = pessoa.GerarTituloPorDocumentoFiscal,
                        BoletoConfiguracao = new
                        {
                            Codigo = pessoa.BoletoConfiguracao?.Codigo ?? 0,
                            Descricao = pessoa.BoletoConfiguracao?.Descricao ?? string.Empty
                        },
                        EnviarBoletoPorEmailAutomaticamente = pessoa.EnviarBoletoPorEmailAutomaticamente,
                        EnviarDocumentacaoFaturamentoCTe = pessoa.EnviarDocumentacaoFaturamentoCTe,
                        pessoa.GerarTituloAutomaticamente,
                        pessoa.GerarFaturaAutomaticaCte,
                        pessoa.GerarFaturamentoAVista,
                        pessoa.AssuntoEmailFatura,
                        pessoa.CorpoEmailFatura,
                        pessoa.GerarBoletoAutomaticamente,
                        pessoa.EnviarArquivosDescompactados,
                        pessoa.NaoEnviarEmailFaturaAutomaticamente,
                        pessoa.TipoEnvioFatura,
                        pessoa.TipoAgrupamentoFatura,
                        pessoa.FormaTitulo,
                        DiasSemanaFatura = pessoa.DiasSemanaFatura.Select(o => o).ToList(),
                        DiasMesFatura = pessoa.DiasMesFatura.Select(o => o).ToList(),
                        pessoa.EmailEnvioDocumentacao,
                        pessoa.TipoAgrupamentoEnvioDocumentacao,
                        pessoa.AssuntoDocumentacao,
                        pessoa.CorpoEmailDocumentacao,
                        pessoa.EmailFatura,
                        pessoa.HabilitarPeriodoVencimentoEspecifico,

                        InformarEmailEnvioDocumentacao = !string.IsNullOrWhiteSpace(pessoa.EmailEnvioDocumentacao),
                        pessoa.FormaEnvioDocumentacao,
                        pessoa.EmailEnvioDocumentacaoPorta,
                        pessoa.TipoAgrupamentoEnvioDocumentacaoPorta,
                        pessoa.AssuntoDocumentacaoPorta,
                        pessoa.CorpoEmailDocumentacaoPorta,
                        pessoa.GerarFaturamentoMultiplaParcela,
                        pessoa.QuantidadeParcelasFaturamento,
                        pessoa.AvisoVencimetoHabilitarConfiguracaoPersonalizada,
                        pessoa.AvisoVencimetoQunatidadeDias,
                        pessoa.AvisoVencimetoEnviarDiariamente,
                        pessoa.CobrancaHabilitarConfiguracaoPersonalizada,
                        pessoa.CobrancaQunatidadeDias,
                        pessoa.AvisoVencimetoNaoEnviarEmail,
                        pessoa.CobrancaNaoEnviarEmail,
                        InformarEmailEnvioDocumentacaoPorta = !string.IsNullOrWhiteSpace(pessoa.EmailEnvioDocumentacaoPorta),
                        pessoa.FormaEnvioDocumentacaoPorta,

                        FaturaVencimentos = (from vencimento in pessoaFaturaVencimentos
                                             select new
                                             {
                                                 vencimento.Codigo,
                                                 vencimento.DiaInicial,
                                                 vencimento.DiaFinal,
                                                 vencimento.DiaVencimento
                                             }).ToList(),

                        GerarTituloAutomaticamenteComAdiantamentoSaldo = pessoa.ConfiguracaoFatura?.GerarTituloAutomaticamenteComAdiantamentoSaldo ?? false,
                        PercentualAdiantamentoTituloAutomatico = pessoa.ConfiguracaoFatura?.PercentualAdiantamentoTituloAutomatico.ToString("n2") ?? string.Empty,
                        PrazoAdiantamentoEmDiasTituloAutomatico = pessoa.ConfiguracaoFatura?.PrazoAdiantamentoEmDiasTituloAutomatico.ToString("n0") ?? string.Empty,
                        PercentualSaldoTituloAutomatico = pessoa.ConfiguracaoFatura?.PercentualSaldoTituloAutomatico.ToString("n2") ?? string.Empty,
                        PrazoSaldoEmDiasTituloAutomatico = pessoa.ConfiguracaoFatura?.PrazoSaldoEmDiasTituloAutomatico.ToString("n0") ?? string.Empty,
                        EfetuarImpressaoDaTaxaDeMoedaEstrangeira = pessoa.ConfiguracaoFatura?.EfetuarImpressaoDaTaxaDeMoedaEstrangeira ?? false,
                    },
                    pessoa.IndicadorIE,
                    pessoa.InscricaoSuframa,
                    pessoa.InscricaoMunicipal,
                    Pais = new
                    {
                        Codigo = pessoa.Pais?.Codigo ?? 0,
                        Descricao = pessoa.Pais?.Nome ?? string.Empty
                    },
                    DataFixaVencimento = (from obj in pessoa.PessoaDataFixaVencimento
                                          select new
                                          {
                                              obj.Codigo,
                                              obj.DiaInicialEmissao,
                                              obj.DiaFinalEmissao,
                                              obj.DiaVencimento
                                          }).ToList(),
                    Contatos = (from obj in pessoa.Contatos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Contato,
                                    obj.Email,
                                    Situacao = obj.Ativo,
                                    obj.Telefone,
                                    TipoContato = obj.TiposContato.Select(o => o.Codigo).ToList(),
                                    obj.DescricaoTipoContato,
                                    obj.DescricaoSituacao,
                                    obj.CPF,
                                    obj.Cargo
                                }).ToList(),
                    UsuariosAdicionais = (listaUsuarioTerceiroAdicionais?.Any() ?? false) ? (from usuarioTerceiroAdicional in listaUsuarioTerceiroAdicionais
                                                                                             select new
                                                                                             {
                                                                                                 Codigo = usuarioTerceiroAdicional.Codigo,
                                                                                                 NomeColaborador = usuarioTerceiroAdicional.Nome,
                                                                                                 UsuarioAcesso = usuarioTerceiroAdicional.Login,
                                                                                                 CNPJ_CPF = usuarioTerceiroAdicional.CPF,
                                                                                                 Email = usuarioTerceiroAdicional.Email,
                                                                                                 Senha = usuarioTerceiroAdicional.Senha,
                                                                                                 Ativo = usuarioTerceiroAdicional.Status == "A" ? "Ativo" : "Inativo",
                                                                                             }).ToList() : null,
                    OutrasDescricoesPessoaExterior = pessoaExteriorOutrasDescricoes.Select(o => new
                    {
                        o.Codigo,
                        o.RazaoSocial,
                        o.Endereco
                    }).ToList(),
                    ListaLicencas = (from obj in licencasPessoa
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao,
                                         obj.Numero,
                                         DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                         DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                         FormaAlerta = "[" + string.Join(",", obj.FormasAlerta.Select(o => (int)o).ToList()) + "]",
                                         obj.Status,
                                         DescricaoLicenca = obj.Licenca != null ? obj.Licenca.Descricao : string.Empty,
                                         CodigoLicenca = obj.Licenca != null ? obj.Licenca.Codigo : 0
                                     }).ToList(),
                    ListaVendedores = ObterListaVendedores(pessoa),
                    ListaRecebedoresAutorizados = ObterListaRecebedoresAutorizados(pessoa, unitOfWork),
                    ListaComponentes = (from obj in componentes
                                        select new
                                        {
                                            obj.Codigo,
                                            CodigoFilial = obj.Filial?.Codigo ?? 0,
                                            Filial = obj.Filial?.Descricao ?? string.Empty,
                                            CodigoTransportadora = obj.Empresa?.Codigo ?? 0,
                                            Transportadora = obj.Empresa?.RazaoSocial ?? string.Empty,
                                            CodigoComponente = obj.ComponenteFrete.Codigo,
                                            Componente = obj.ComponenteFrete.Descricao,
                                            ValorComponente = obj.Valor.ToString("n2")
                                        }).ToList(),
                    ListaDadosArmador = (from pessoaArmador in listaPessoaArmador
                                         select new
                                         {
                                             pessoaArmador.Codigo,
                                             ValorDiariaAposFreetime = pessoaArmador.ValorDariaAposFreetime.HasValue ? pessoaArmador.ValorDariaAposFreetime.Value.ToString("n2") : "0,00",
                                             pessoaArmador.DiasFreetime,
                                             DataVigenciaInicial = pessoaArmador.DataVigenciaInicial?.ToString("d") ?? string.Empty,
                                             DataVigenciaFinal = pessoaArmador.DataVigenciaFinal?.ToString("d") ?? string.Empty,
                                             VigenciaDescricao = string.Concat(pessoaArmador.DataVigenciaInicial.HasValue ? $"{Localization.Resources.Gerais.Geral.De} {pessoaArmador.DataVigenciaInicial?.ToString("d")} " : string.Empty, pessoaArmador.DataVigenciaFinal.HasValue ? $"{Localization.Resources.Gerais.Geral.Ate} {pessoaArmador.DataVigenciaFinal?.ToString("d")}" : string.Empty).Trim(),
                                             DescricaoTipoContainer = pessoaArmador.ContainerTipo?.Descricao ?? "",
                                             CodigoTipoContainer = pessoaArmador.ContainerTipo?.Codigo ?? 0
                                         }).ToList(),
                    RestricoesFilaCarregamento = ObterRestricoesFilaCarregamento(pessoa),
                    EmailInterno = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? dadosCliente != null ? dadosCliente.Email : string.Empty : string.Empty,
                    EnviarEmailInterno = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? dadosCliente != null ? dadosCliente.EmailStatus == "A" ? true : false : false : false,
                    DadosAdicionais = new
                    {
                        pessoa.PISPASEP,
                        DataNascimento = pessoa.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        pessoa.NomeSocio,
                        pessoa.TipoClienteIntegracaoLBC,
                        pessoa.CPFSocio,
                        pessoa.Profissao,
                        pessoa.TituloEleitoral,
                        pessoa.ZonaEleitoral,
                        pessoa.SecaoEleitoral,
                        pessoa.NumeroCEI,
                        pessoa.GerarPedidoColeta,
                        pessoa.GerarPedidoBloqueado,
                        pessoa.ExigirNumeroControleCliente,
                        pessoa.ReplicarNumeroReferenciaTodasNotasCarga,
                        pessoa.DigitalizacaoCanhotoInteiro,
                        pessoa.ReplicarNumeroControleCliente,
                        pessoa.ExigirNumeroNumeroReferenciaCliente,
                        pessoa.EnviarAutomaticamenteDocumentacaoCarga,
                        OrgaoEmissaoRG = pessoa.OrgaoEmissorRG,
                        ClientePai = pessoa.ClientePai != null ? new { pessoa.ClientePai.Codigo, pessoa.ClientePai.Descricao } : null,
                        RecebedorColeta = new { pessoa.RecebedorColeta?.Codigo, pessoa.RecebedorColeta?.Descricao },
                        Transportador = pessoa.Empresa != null ? new { pessoa.Empresa.Codigo, pessoa.Empresa.Descricao } : null,
                        pessoa.CodigoPortuario,
                        TipoEmissaoCTeDocumentosExclusivo = pessoa.TipoEmissaoCTeDocumentosExclusivo.HasValue ? pessoa.TipoEmissaoCTeDocumentosExclusivo.Value : TipoEmissaoCTeDocumentos.NaoInformado,
                        RateioFormulaExclusivo = pessoa.RateioFormulaExclusivo != null ? new { pessoa.RateioFormulaExclusivo.Codigo, pessoa.RateioFormulaExclusivo.Descricao } : null,
                        pessoa.Celular,
                        pessoa.ObservacaoInterna,
                        pessoa.CodigoCompanhia,
                        pessoa.ContaFornecedorEBS,
                        pessoa.CodigoDocumento,
                        pessoa.SenhaLiberacaoMobile,
                        pessoa.SenhaConfirmacaoColetaEntrega,
                        pessoa.ValorMinimoCarga,
                        pessoa.FronteiraAlfandega,
                        TempoMedioPermanenciaFronteira = $"{pessoa.TempoMedioPermanenciaFronteira / 60:D3}:{pessoa.TempoMedioPermanenciaFronteira % 60:D2}",
                        pessoa.CodigoAduaneiro,
                        pessoa.CodigoURFAduaneiro,
                        pessoa.CodigoRAAduaneiro,
                        pessoa.CodigoAduanaDestino,
                        pessoa.ExigeQueEntregasSejamAgendadas,
                        pessoa.AreaRedex,
                        pessoa.Armador,
                        pessoa.PermiteAgendarComViagemIniciada,
                        //pessoa.DiasFreetime,
                        //ValorDiariaAposFreetime = pessoa.ValorDariaAposFreetime,
                        PedidoTipoPagamento = pessoa.PedidoTipoPagamento != null ? new { pessoa.PedidoTipoPagamento.Codigo, pessoa.PedidoTipoPagamento.Descricao } : null,
                        TipoOperacaoPadrao = pessoa.TipoOperacaoPadrao != null ? new { pessoa.TipoOperacaoPadrao.Codigo, pessoa.TipoOperacaoPadrao.Descricao } : null,
                        ArmazemResponsavel = pessoa.ArmazemResponsavel != null ? new { pessoa.ArmazemResponsavel.Codigo, pessoa.ArmazemResponsavel.Descricao } : null,
                        CondicaoPagamentoPadrao = pessoa.CondicaoPagamentoPadrao != null ? new { pessoa.CondicaoPagamentoPadrao.Codigo, pessoa.CondicaoPagamentoPadrao.Descricao } : null,
                        TipoIntegracaoValePedagio = pessoa.TipoIntegradoraValePedagio != null ? new { pessoa.TipoIntegradoraValePedagio.Codigo, pessoa.TipoIntegradoraValePedagio.Descricao } : null,
                        pessoa.SituacaoFinanceira,
                        EstadoCivil = pessoa.EstadoCivil.HasValue ? pessoa.EstadoCivil.Value : EstadoCivil.Outros,
                        pessoa.CotacaoEspecial,
                        Sexo = pessoa.Sexo.HasValue ? pessoa.Sexo.Value : Dominio.ObjetosDeValor.Enumerador.Sexo.NaoInformado,
                        pessoa.TipoFornecedor,
                        pessoa.CodigoCategoriaTrabalhador,
                        pessoa.Funcao,
                        pessoa.PagamentoEmBanco,
                        pessoa.FormaPagamentoeSocial,
                        pessoa.TipoAutonomo,
                        pessoa.CodigoReceita,
                        pessoa.TipoPagamentoBancario,
                        pessoa.NaoDescontaIRRF,
                        pessoa.CodigoAlternativo,
                        BancoDOC = pessoa.BancoDOC != null ? new { pessoa.BancoDOC.Codigo, pessoa.BancoDOC.Descricao } : null,
                        EstadoRG = pessoa.EstadoRG != null ? new { pessoa.EstadoRG.Codigo, pessoa.EstadoRG.Descricao } : null,
                        pessoa.CodigoSap,
                        pessoa.EnviarDocumentacaoCTeAverbacaoSegundaInstancia,
                        pessoa.Referencia,
                        pessoa.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente,
                        pessoa.ExigeEtiquetagem,
                        pessoa.EhPontoDeApoio,
                        MesoRegiao = pessoa.MesoRegiao != null ? new { pessoa.MesoRegiao.Codigo, pessoa.MesoRegiao.Descricao } : null,
                        Regiao = pessoa.Regiao != null ? new { pessoa.Regiao.Codigo, pessoa.Regiao.Descricao } : null,
                        pessoa.InstituicaoGovernamental,
                        pessoa.NaoAplicarChecklistMultiMobile,
                        pessoa.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento,
                        pessoa.NaoComprarValePedagio,
                        pessoa.ValorMinimoEntrega,
                        pessoa.FazParteGrupoEconomico,
                        LocalidadeNascimento = pessoa.LocalidadeNascimento != null ? new { pessoa.LocalidadeNascimento.Codigo, Descricao = pessoa.LocalidadeNascimento.DescricaoCidadeEstado } : null,
                        CanalEntrega = pessoa.CanalEntrega != null ? new { pessoa.CanalEntrega.Codigo, Descricao = pessoa.CanalEntrega.Descricao } : null,
                        pessoa.RegraPallet,
                        pessoa.RKST,
                        pessoa.MDGCode,
                        pessoa.CMDID,
                    },
                    ObservacoesCTes = observacoesCTe.Select(o => new
                    {
                        o.Codigo,
                        o.Identificador,
                        o.Tipo,
                        o.Texto,
                        DescricaoTipo = o.Tipo.ObterDescricao()
                    }).ToList(),
                    ListaSubarea = (from obj in subAreasCliente
                                    orderby obj.Descricao
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        Descricao = obj.Descricao,
                                        Area = obj.Area,
                                        TipoSubarea = obj.TipoSubarea.Codigo,
                                        TipoSubareaDescricao = obj.TipoSubarea.Descricao,
                                        Ativo = obj.Ativo,
                                        AtivoDescricao = ((obj.Ativo) ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo),
                                        FluxoDePatio = obj.TipoSubarea.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea,
                                        ListaSubareaClienteAcoesFluxoDePatio = obj.AcoesFluxoPatio.ToList().Select(o => new
                                        {
                                            CodigoAcao = o.Codigo,
                                            CodigoAcaoMonitoramentoFluxoDePatio = o.AcaoMonitoramento,
                                            AcaoMonitoramentoFluxoDePatio = MonitoramentoEventoDataHelper.ObterDescricao(o.AcaoMonitoramento),
                                            CodigoEtapaFluxoDePatio = o.EtapaFluxoPatio,
                                            EtapaFluxoDePatio = EtapaFluxoGestaoPatioHelper.ObterDescricao(o.EtapaFluxoPatio),
                                            CodigoAcaoFluxoDePatio = o.AcaoFluxoPatio,
                                            AcaoFluxoDePatio = AcaoFluxoGestaoPatioHelper.ObterDescricao(o.AcaoFluxoPatio)
                                        }),
                                        obj.CodigoTag
                                    }).ToList(),
                    Anexos = (from anexo in anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo,
                              }).ToList(),
                    AreasRedex = (
                            from clienteAreaRedex in clientesAreaRedex
                            select new
                            {
                                ClienteRedex = (new
                                {
                                    Descricao = clienteAreaRedex.AreaRedex.Descricao,
                                    Codigo = clienteAreaRedex.AreaRedex.CPF_CNPJ,
                                })
                            }
                        ).ToList(),
                    AreaRedex = new
                    {
                        TipoOperacaoRedespacho = new { Codigo = pessoa.TipoOperacaoPadraoRedespachoAreaRedex?.Codigo ?? 0, Descricao = pessoa.TipoOperacaoPadraoRedespachoAreaRedex?.Descricao ?? "" }
                    },
                    GrupoPessoasAcessoPortal = (from obj in pessoa.GruposPessoas
                                                select new
                                                {
                                                    Codigo = obj.Codigo,
                                                    Descricao = obj.Descricao,
                                                }).ToList(),
                    Comprovantes = (from obj in pessoa.TiposComprovante
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        Descricao = obj.Descricao
                                    }).ToList(),
                    FilialCliente = ObterFilialCliente(pessoa),

                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarPorCNPJCPF);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultaFaturamentoFornecedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("Codigo"));
                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = Double.Parse(cpfcnpj);

                Repositorio.Cliente repPessoas = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                Dominio.Entidades.Cliente pessoa = repPessoas.BuscarPorCPFCNPJ(dCPFCNPJ);
                object retorno = null;

                if (pessoa != null)
                {
                    retorno = new
                    {
                        Tipo = pessoa != null ? pessoa.Modalidades != null && pessoa.Modalidades.Any(p => p.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor && p.ModalidadesFornecedores.Any(o => o.PagoPorFatura == false)) ? "Motorista" : "Faturamento" : "Faturamento"
                        //Tipo = pessoa != null ? pessoa.Modalidades != null && pessoa.Modalidades[0].Codigo > 0 ? repModalidadeFornecedor.BuscarPorModalidade(pessoa.Modalidades[0].Codigo) != null ? repModalidadeFornecedor.BuscarPorModalidade(pessoa.Modalidades[0].Codigo).PagoPorFatura ? "Faturamento" : "Motorista" : "Motorista" : "Motorista" : "Motorista"
                    };
                }
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarTipoDoFaturamentoDoFornecedor);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarValorCombustivelTabela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CodigoFornecedor"));
                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = double.Parse(cpfcnpj);
                int codigoCombustivel = 0;
                int.TryParse(Request.Params("CodigoCombustivel"), out codigoCombustivel);

                Repositorio.Cliente repPessoas = new Repositorio.Cliente(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
                Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repTabelaValor = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);

                Dominio.Entidades.Cliente pessoa = repPessoas.BuscarPorCPFCNPJ(dCPFCNPJ);
                object retorno = null;
                decimal valorUnitario = 0;
                if (pessoa != null)
                {
                    Dominio.Entidades.Produto combustivel = repProduto.BuscarPorCodigo(0, codigoCombustivel);
                    if (combustivel != null)
                    {
                        valorUnitario = repTabelaValor.BuscarValorCombustivel(codigoCombustivel, dCPFCNPJ);
                        retorno = new
                        {
                            ValorUnitario = valorUnitario.ToString("n4")
                        };
                    }
                }
                if (retorno == null || valorUnitario <= 0m)
                {
                    string filial = Utilidades.String.OnlyNumbers(Request.Params("CodigoFornecedor"));
                    filial = filial.PadLeft(14, '0');

                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque = repProdutoEstoque.BuscarPorProdutoECNPJFilial(codigoCombustivel, filial);
                    Dominio.Entidades.Produto prod = repProduto.BuscarPorCodigo(codigoCombustivel);

                    retorno = new
                    {
                        ValorUnitario = produtoEstoque != null && produtoEstoque.UltimoCusto > 0 ? produtoEstoque.UltimoCusto.ToString("n4") : (prod?.UltimoCusto.ToString("n4") ?? "0,000")
                    };
                }
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarValorDeTabelaDoCombustivel);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoa = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao repConfiguracaoPessoaEmissao = new Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao(unitOfWork);
                Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura repConfiguracaoPessoaFatura = new Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico repClienteAcrescimoDesconto = new Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormulario repositorioFormularioFuncionario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorisata = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Codigo")), out double dCPFCNPJ);

                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(dCPFCNPJ);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico> clienteAcrescimoDesconto = repClienteAcrescimoDesconto.BuscarPorPessoa(dCPFCNPJ);

                if (pessoa == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                if (repositorioCargaMotorisata.ExistePorMotorista(pessoa.CPF_CNPJ))
                    return new JsonpResult(false, "Este Usuário já está vinculado a uma carga");


                for (int i = 0; i < pessoa.Modalidades.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = pessoa.Modalidades[i];

                    if (modalidade.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente)
                        repModalidadeClientePessoas.DeletarPorModalidade(modalidade.Codigo);
                    else if (modalidade.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)
                        repModalidadeFornecedorPessoas.DeletarPorModalidade(modalidade.Codigo);
                    else if (modalidade.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro)
                    {
                        Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCliente(pessoa.CPF_CNPJ, Dominio.Enumeradores.TipoAcesso.Terceiro);

                        if (usuario != null)
                        {
                            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Pessoa.RemoveuPorExclusaoDePessoa, unitOfWork);
                            repOperadorLogistica.DeletarOperadorLogisticoVinculados(usuario.Codigo);
                            repositorioFormularioFuncionario.DeletarFormulariosVinculados(usuario.Codigo);
                            repUsuario.Deletar(usuario, Auditado);

                        }

                        repModalidadeTransportadoraPessoas.DeletarPorModalidade(modalidade.Codigo);
                    }
                }

                for (int i = 0; i < pessoa.Vencimentos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = pessoa.Vencimentos[i];
                    repVencimento.DeletarPorEntidade(vencimento);
                }

                if (clienteAcrescimoDesconto.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico obj in clienteAcrescimoDesconto)
                        if (obj.Codigo > 0)
                            codigos.Add((int)obj.Codigo);

                    List<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico> listaClienteAcrescimoDescontoDeletar = clienteAcrescimoDesconto.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

                    for (int i = 0; i < listaClienteAcrescimoDescontoDeletar.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico clienteAcrescimoDescontoDeletar = listaClienteAcrescimoDescontoDeletar[i];

                        repClienteAcrescimoDesconto.Deletar(clienteAcrescimoDescontoDeletar);
                    }
                }


                if (pessoa.ConfiguracaoEmissao != null)
                    repConfiguracaoPessoaEmissao.Deletar(pessoa.ConfiguracaoEmissao);
                if (pessoa.ConfiguracaoFatura != null)
                    repConfiguracaoPessoaFatura.Deletar(pessoa.ConfiguracaoFatura);


                repPessoa.Deletar(pessoa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailUsuarioTerceiro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string cpfcnpj = "";
                cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));

                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = Double.Parse(cpfcnpj);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCliente(dCPFCNPJ, Dominio.Enumeradores.TipoAcesso.Terceiro);

                if (usuario == null)
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.NaoFoiLocalizadoUsuarioParaEsteClienteNecessarioSalvarCadastroAntesDeEnviarEmail);

                if (string.IsNullOrEmpty(usuario.Email))
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.ClienteSemEmailCadastrado);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.SemEmailConfiguradoParaEnvio);

                string mensagemErro = this.EnviarEmailUsuarioTerceiro(usuario, unitOfWork);
                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, mensagemErro);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Pessoa.EnviouEmailUsuarioTerceiro, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoEnviarEmailUsuarioTerceiro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailUsuarioTerceiroAdicional()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string cpfcnpj = "";

                cpfcnpj = Request.Params("CNPJ");

                cpfcnpj = new string(cpfcnpj.Where(char.IsDigit).ToArray());

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorClienteTerceiroAdicionais(cpfcnpj, Dominio.Enumeradores.TipoAcesso.Terceiro);

                if (usuario == null)
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.NaoFoiLocalizadoUsuarioParaEsteClienteNecessarioSalvarCadastroAntesDeEnviarEmail);

                if (string.IsNullOrEmpty(usuario.Email))
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.ClienteSemEmailCadastrado);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.SemEmailConfiguradoParaEnvio);

                string mensagemErro = this.EnviarEmailUsuarioTerceiro(usuario, unitOfWork);
                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, mensagemErro);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Pessoa.EnviouEmailUsuarioTerceiro, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoEnviarEmailUsuarioTerceiro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> HistoricoNotaEntrada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada()
                {
                    CpfCnpjFornecedor = Request.GetDoubleParam("Codigo")
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.NumeroDocumento, "Numero", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Emissao, "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.SituacaoNF, "DescricaoSituacao", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.StatusFinanceiro, "StatusFinanceiro", 25, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaProduto = repDocumentoEntradaTMS.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repDocumentoEntradaTMS.ContarConsulta(filtrosPesquisa);

                var lista = (from o in listaProduto
                             orderby o.DataEmissao
                             select new
                             {
                                 o.Codigo,
                                 o.Numero,
                                 DataEmissao = o.DataEmissao.ToString("dd/MM/yyyy"),
                                 DescricaoSituacao = o.Situacao.ObterDescricao(),
                                 o.StatusFinanceiro
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DocumentosFornecedor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ClienteDocumentacao repClienteDocumentacao = new Repositorio.Embarcador.Pessoas.ClienteDocumentacao(unitOfWork);

                double.TryParse(Request.Params("Codigo"), out double codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Emissao, "DataEmissao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Vencimento, "DataVencimento", 15, Models.Grid.Align.center, false);

                string propOrdena = "DataVencimento";
                string dirOrdena = "descending";

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao> listaDocumento = repClienteDocumentacao.Consultar(codigo, "", propOrdena, dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repClienteDocumentacao.ContarConsulta(codigo, "");

                var lista = (from o in listaDocumento
                                 //orderby o.DataVencimento descending
                             select new
                             {
                                 o.Codigo,
                                 o.Descricao,
                                 DataEmissao = o.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DataVencimento = o.DataVencimento.Value.ToString("dd/MM/yyyy")
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosPessoaPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                double.TryParse(Request.Params("Codigo"), out double codigo);

                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);
                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(this.Usuario.Empresa.Codigo, codigo);

                if (dadosCliente == null)
                    return new JsonpResult(true);

                var dynTipoMovimento = new
                {
                    TipoMovimento = dadosCliente.TipoMovimentoTituloPagar != null ? new { dadosCliente.TipoMovimentoTituloPagar.Codigo, dadosCliente.TipoMovimentoTituloPagar.Descricao } : null
                };
                return new JsonpResult(dynTipoMovimento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarDadosDaPessoa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultaTipoSubareaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.TipoSubareaCliente repTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente> tipoSubareaClientes = repTipoSubareaCliente.BuscarTodosAtivos();
                if (tipoSubareaClientes.Count == 0)
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.NenhumTipoDeSubareaEncontradoNecessarioCadastrarTipoAntesDeCriarAsSubareasNoCliente);
                return new JsonpResult(tipoSubareaClientes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarOsTiposDeSubarea);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Bloquear()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double codigo = Request.GetDoubleParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 20)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.MotivoDoBloqueioDeveTerNoMinimoVinteCaracteres);

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(codigo);

                if (pessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                pessoa.Initialize();

                pessoa.Bloqueado = true;
                pessoa.MotivoBloqueio = motivo;
                pessoa.DataBloqueio = DateTime.Now;
                pessoa.DataUltimaAtualizacao = DateTime.Now;
                pessoa.Integrado = false;
                repPessoa.Atualizar(pessoa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, pessoa.GetChanges(), Usuario.Nome + " bloqueou a pessoa.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBloquearPessoa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Desbloquear()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double codigo = Request.GetDoubleParam("Codigo");

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(codigo);

                if (pessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                pessoa.Initialize();

                pessoa.Bloqueado = false;
                pessoa.MotivoBloqueio = null;
                pessoa.DataBloqueio = null;
                pessoa.DataUltimaAtualizacao = DateTime.Now;
                pessoa.Integrado = false;
                repPessoa.Atualizar(pessoa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, pessoa.GetChanges(), Usuario.Nome + " desbloqueou a pessoa.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoDesbloquearPessoa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoAreaRedex()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repClienteRegex = new Repositorio.Cliente(unitOfWork);
                double codigo = Request.GetDoubleParam("Codigo");

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(codigo);

                return new JsonpResult(new
                {
                    PossuiAreaRedex = repClienteRegex.PossuiClientesRedex(),
                    CodigoOperacaoRedespachoAreaRedex = pessoa?.TipoOperacaoPadraoRedespachoAreaRedex?.Codigo ?? 0,
                    OperacaoRedespachoAreaRedex = pessoa?.TipoOperacaoPadraoRedespachoAreaRedex?.Descricao ?? "",
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoBuscarAsConfiguracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GeocodificarEndereco()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

                Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosEndereco dadosEndereco = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosEndereco>(Request.Params("DadosEndereco"));

                Servicos.Embarcador.Logistica.Nominatim.RootObject geolocalizacao = Servicos.Embarcador.Carga.CargaRotaFrete.Geocodificar(dadosEndereco, configuracaoIntegracao);

                if (geolocalizacao == null)
                    return new JsonpResult(false);//, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoNotFound);
                else
                    return new JsonpResult(geolocalizacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoErroNominatim);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfig = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTms = repConfig.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("CPF_CNPJ_SemFormato", false);
            grid.AdicionarCabecalho("Latitude", false);
            grid.AdicionarCabecalho("Longitude", false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                grid.AdicionarCabecalho("CodigoIntegracao", false);
            else
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "CodigoIntegracao", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.RazaoSocial, "Nome", 38, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.NomeFantasia, "NomeFantasia", 38, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CNPJCPF, "CPF_CNPJ", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.IE, "IE_RG", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Endereco, "Endereco", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Localidade, "Localidade", 10, Models.Grid.Align.left, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Telefone, "Telefone1", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.CEP, "CEP", 8, Models.Grid.Align.left, false);
            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            int countCliente = repositorioCliente.ContarConsulta(filtrosPesquisa);

            if (exportacao && countCliente > 10000)
                return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.ExportacaoNaoPermitidaParaMaisDeDezMilRegistros);

            string propriedadeOrdenacao = "Nome";

            List<Dominio.Entidades.Cliente> listaClientes = repositorioCliente.Consultar(filtrosPesquisa, propriedadeOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(countCliente);

            dynamic lista = (from p in listaClientes
                             select new
                             {
                                 Codigo = p.Codigo,
                                 p.Nome,
                                 p.Descricao,
                                 p.Latitude,
                                 p.Longitude,
                                 p.CPF_CNPJ_SemFormato,
                                 p.CodigoIntegracao,
                                 p.NomeFantasia,
                                 CPF_CNPJ = (p.Tipo?.Equals("E") ?? false) ? p.CPF_CNPJ_Formatado : p.CPF_CNPJ_Formatado,
                                 p.IE_RG,
                                 p.CEP,
                                 p.Endereco,
                                 p.DescricaoAtivo,
                                 Localidade = p.Localidade.DescricaoCidadeEstado,
                                 Telefone1 = p.Telefone1.ObterTelefoneFormatado(),
                             }).ToList();

            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoGerarArquivo);
            }
            else
                return new JsonpResult(grid);
        }

        private Dominio.ObjetosDeValor.FiltroPesquisaCliente ObterFiltrosPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            string nome = Request.GetStringParam("Nome");
            string nomeFantasia = Request.GetStringParam("NomeFantasia");
            string cpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CNPJ_CPF"));
            int tipoPessoa = Request.GetIntParam("TipoPessoa", valorPadrao: 99);
            string tipo = "";

            if (tipoPessoa == 0)
                tipo = "F";
            else if (tipoPessoa == 1)
                tipo = "J";
            else if (tipoPessoa == 2)
                tipo = "E";

            int codigoLocalidade = Request.GetIntParam("LocalidadePrincipal");
            Dominio.Entidades.Localidade localidade = codigoLocalidade > 0 ? new Dominio.Entidades.Localidade() { Codigo = codigoLocalidade } : null;

            if (string.IsNullOrWhiteSpace(cpfCnpj))
            {
                string cpfCnpjInformadoCampoNome = Utilidades.String.OnlyNumbers(nome);

                if (Utilidades.Validate.ValidarCNPJ(cpfCnpjInformadoCampoNome) || Utilidades.Validate.ValidarCPF(cpfCnpjInformadoCampoNome))
                {
                    cpfCnpj = cpfCnpjInformadoCampoNome;
                    nome = string.Empty;
                }
            }

            List<TipoModalidade> modalidades = new List<TipoModalidade>();
            TipoModalidade? modalidade = Request.GetNullableEnumParam<TipoModalidade>("TipoModalidadePessoa");
            if (modalidade.HasValue)
                modalidades.Add(modalidade.Value);

            Dominio.Entidades.Empresa empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null;
            Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa = new Dominio.ObjetosDeValor.FiltroPesquisaCliente()
            {
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                GeoLocalizacaoTipo = Request.GetEnumParam("GeoLocalizacaoTipo", GeoLocalizacaoTipo.Todos),
                FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos = configuracao.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos,
                CpfCnpj = cpfCnpj.ToDouble(),
                Estado = Request.GetStringParam("Estado"),
                CodigoPais = Request.GetIntParam("Pais"),
                Localidade = localidade,
                Nome = nome,
                NomeFantasia = nomeFantasia,
                SomenteSemValorDescarga = Request.GetBoolParam("SomenteSemValorDescarga"),
                SomenteSemGeolocalizacao = Request.GetBoolParam("SomenteSemGeolocalizacao"),
                Telefone = Utilidades.String.OnlyNumbers(Request.GetStringParam("TelefonePrincipal")),
                Tipo = tipo,
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoCategoria = Request.GetIntParam("Categoria"),
                Modalidades = modalidades,
                CodigoEmpresa = empresa != null && empresa.VisualizarSomenteClientesAssociados ? empresa.Codigo : 0,
                PossuiExcecaoCheckinFilaH = Request.GetBoolParam("PossuiExcecaoCheckinFilaH"),
            };

            return filtrosPesquisa;
        }

        private void ConverterObjetoRetornoWS(ref ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno, dynamic retornoReceitaWS)
        {
            if (!string.IsNullOrWhiteSpace((string)retornoReceitaWS.status) && !string.IsNullOrWhiteSpace((string)retornoReceitaWS.message))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = 0;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyy");
                retorno.Mensagem = (string)retornoReceitaWS.message;
                retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
                retorno.Objeto.ConsultaValida = false;
                retorno.Objeto.MensagemReceita = (string)retornoReceitaWS.message;
            }
            else
            {
                retorno.Status = true;
                retorno.CodigoMensagem = 1;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyy");
                retorno.Mensagem = Localization.Resources.Gerais.Geral.Sucesso;
                retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
                retorno.Objeto.ConsultaValida = true;
                retorno.Objeto.MensagemReceita = Localization.Resources.Gerais.Geral.Sucesso;
                retorno.Objeto.Pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                retorno.Objeto.Pessoa.AtualizarEnderecoPessoa = true;
                retorno.Objeto.Pessoa.ClienteExterior = false;

                retorno.Objeto.Pessoa.CNAE = "";
                if (retornoReceitaWS.atividade_principal.Count > 0)
                    retorno.Objeto.Pessoa.CNAE = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.atividade_principal[0].code);

                retorno.Objeto.Pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                retorno.Objeto.Pessoa.Endereco.Bairro = ((string)retornoReceitaWS.bairro).ToUpper();
                retorno.Objeto.Pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.cep);
                retorno.Objeto.Pessoa.Endereco.CEPSemFormato = (string)retornoReceitaWS.cep;

                retorno.Objeto.Pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                retorno.Objeto.Pessoa.Endereco.Cidade.Descricao = ((string)retornoReceitaWS.municipio).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF = ((string)retornoReceitaWS.uf).ToUpper();

                retorno.Objeto.Pessoa.Endereco.Complemento = ((string)retornoReceitaWS.complemento).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Logradouro = ((string)retornoReceitaWS.logradouro).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Numero = ((string)retornoReceitaWS.numero).ToUpper();

                string[] telefoneRetorno = ((string)retornoReceitaWS.telefone).Split('/');
                string telefone = "";
                string telefone2 = "";
                if (telefoneRetorno.Count() > 0)
                    telefone = Utilidades.String.OnlyNumbers(telefoneRetorno[0].ToString());
                if (telefoneRetorno.Count() > 1)
                    telefone2 = Utilidades.String.OnlyNumbers(telefoneRetorno[1].ToString());
                retorno.Objeto.Pessoa.Endereco.Telefone = telefone;
                retorno.Objeto.Pessoa.Endereco.Telefone2 = telefone2;

                retorno.Objeto.Pessoa.NomeFantasia = ((string)retornoReceitaWS.fantasia).ToUpper();
                retorno.Objeto.Pessoa.RazaoSocial = ((string)retornoReceitaWS.nome).ToUpper();
                retorno.Objeto.Pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                retorno.Objeto.Pessoa.RGIE = "";
            }
        }

        private string ValidaExtencaoNumeroTelefone(string telefone, bool permiteTelefoneInternacional)
        {
            // Valida a que a extenção do numero não seja maior a 11 digitos, caso não possua a configuração de telefone internacional.
            int extensaoTelefone = permiteTelefoneInternacional ? 12 : 11;

            if (string.IsNullOrEmpty(telefone))
                // se o string é um valor null retorna uma string vazia
                return string.Empty;

            if (telefone.Length < 10 || telefone.Length > extensaoTelefone)
                // se o string tem uma extenção menor a 10 ou maior a 11 digitos retorna uma string vazia
                return string.Empty;

            return telefone;
        }

        private void PreencherDadosPessoa(Dominio.Entidades.Cliente pessoa, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unitOfWork, bool adicionar = false)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repositorioConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repositorioConfiguracaoPessoa.BuscarPrimeiroRegistro();
            int grupoPessoa = Request.GetIntParam("GrupoPessoas");
            int categoria = Request.GetIntParam("Categoria");
            int tipoPessoa = int.Parse(Request.Params("TipoPessoa"));
            string cpfcnpj = "";
            if (tipoPessoa > 0 && !string.IsNullOrWhiteSpace(Request.Params("CNPJ")))
                cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));
            else if (tipoPessoa == 0)
                cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CPF"));
            string iE_RG = Request.Params("IE_RG");
            string nome = Request.Params("Nome");
            string fantasia = Request.Params("Fantasia");
            string nomeVisoesBI = Request.Params("NomeVisoesBI");
            string telefonePrincipal = ValidaExtencaoNumeroTelefone(Utilidades.String.OnlyNumbers(Request.Params("TelefonePrincipal")), configuracaoPessoa.PermitirCadastroDeTelefoneInternacional);
            string telefoneSecundario = ValidaExtencaoNumeroTelefone(Utilidades.String.OnlyNumbers(Request.Params("TelefoneSecundario")), configuracaoPessoa.PermitirCadastroDeTelefoneInternacional);
            string endereco = Request.Params("Endereco");
            string numero = Request.Params("Numero");
            string bairro = Request.Params("Bairro");
            string complemento = Request.Params("Complemento");
            string cEP = Utilidades.String.OnlyNumbers(Request.Params("CEP")).PadLeft(8, '0');
            string rG = Request.Params("RG");
            if (string.IsNullOrWhiteSpace(rG))
                rG = Request.Params("Passaporte");
            string email = Request.Params("Email");
            string observacao = Request.Params("Observacao");
            string latitude = Request.Params("Latitude");
            string longitude = Request.Params("Longitude");
            string latitudeTransbordo = Request.Params("LatitudeTransbordo");
            string longitudeTransbordo = Request.Params("LongitudeTransbordo");
            string inscricaoSuframa = Request.Params("InscricaoSuframa");
            string codigoIntegracao = Request.Params("CodigoIntegracao");

            bool enviarEmail;
            bool.TryParse(Request.Params("EnviarEmail"), out enviarEmail);
            bool ativo = true;
            bool.TryParse(Request.Params("Ativo"), out ativo);

            pessoa.EmailStatus = enviarEmail ? "A" : "I";

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE indicadorIE;
            Enum.TryParse(Request.Params("IndicadorIE"), out indicadorIE);

            pessoa.Banco = null;
            pessoa.Agencia = null;
            pessoa.DigitoAgencia = null;
            pessoa.NumeroConta = null;
            pessoa.TipoContaBanco = null;
            pessoa.ClientePortadorConta = null;
            pessoa.CnpjIpef = null;
            pessoa.TipoChavePix = null;
            pessoa.CodigoIntegracaoDadosBancarios = null;
            pessoa.ChavePix = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                pessoa.Ativo = true;
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                pessoa.Ativo = ativo;
            else
            {
                if (pessoa.Codigo == 0)
                    pessoa.Ativo = true;
            }

            dynamic dynDadosBancarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DadosBancarios"));
            if (dynDadosBancarios != null)
            {
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.ClientePortadorConta) && (string)dynDadosBancarios.ClientePortadorConta != "0")
                    pessoa.ClientePortadorConta = new Dominio.Entidades.Cliente() { CPF_CNPJ = double.Parse((string)dynDadosBancarios.ClientePortadorConta) };
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Banco) && (string)dynDadosBancarios.Banco != "0")
                    pessoa.Banco = new Dominio.Entidades.Banco() { Codigo = int.Parse((string)dynDadosBancarios.Banco) };
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Agencia))
                    pessoa.Agencia = (string)dynDadosBancarios.Agencia;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Digito))
                    pessoa.DigitoAgencia = (string)dynDadosBancarios.Digito;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.NumeroConta))
                {
                    pessoa.NumeroConta = (string)dynDadosBancarios.NumeroConta;
                    pessoa.NumeroConta = pessoa.NumeroConta.Replace(" ", "");
                }
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.TipoConta) && (string)dynDadosBancarios.TipoConta != "0")
                    pessoa.TipoContaBanco = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco)int.Parse((string)dynDadosBancarios.TipoConta);
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.CnpjIpef))
                    pessoa.CnpjIpef = (string)dynDadosBancarios.CnpjIpef;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.TipoChavePix))
                    pessoa.TipoChavePix = (Dominio.ObjetosDeValor.Enumerador.TipoChavePix)int.Parse((string)dynDadosBancarios.TipoChavePix);
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.ChavePix))
                    pessoa.ChavePix = (string)dynDadosBancarios.ChavePix;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.CodigoIntegracaoDadosBancarios))
                {
                    pessoa.CodigoIntegracaoDadosBancarios = (string)dynDadosBancarios.CodigoIntegracaoDadosBancarios;
                    pessoa.CodigoIntegracaoDadosBancarios = pessoa.CodigoIntegracaoDadosBancarios.Replace(" ", "");
                }

                if (pessoa.ClientePortadorConta != null && pessoa.ClientePortadorConta.CPF_CNPJ == pessoa.CPF_CNPJ)
                    throw new ControllerException(Localization.Resources.Pessoas.Pessoa.NaoPermitidoInformarPortadorDaContaMesmoDaPessoa);

                if (dynDadosBancarios.UtilizarCadastroContaBancaria != null && pessoa.UtilizarCadastroContaBancaria != bool.Parse((string)dynDadosBancarios.UtilizarCadastroContaBancaria))
                    pessoa.UtilizarCadastroContaBancaria = bool.Parse((string)dynDadosBancarios.UtilizarCadastroContaBancaria);

            }

            int codigoAtividade = int.Parse(Request.Params("Atividade"));

            if (pessoa.Atividade != null)
            {
                if (pessoa.Atividade.Codigo > 0 && pessoa.Atividade.Codigo != codigoAtividade)
                    pessoa.LogAtividade = Localization.Resources.Pessoas.Pessoa.AlteraDe + pessoa.Atividade.Codigo + Localization.Resources.Pessoas.Pessoa.Para + codigoAtividade + Localization.Resources.Pessoas.Pessoa.Por + this.Usuario.Nome + Localization.Resources.Pessoas.Pessoa.As + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }

            pessoa.Atividade = new Dominio.Entidades.Atividade() { Codigo = codigoAtividade };
            pessoa.Bairro = bairro;
            pessoa.CEP = cEP;
            pessoa.InscricaoSuframa = inscricaoSuframa;
            pessoa.InscricaoMunicipal = Request.GetStringParam("InscricaoMunicipal");



            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            pessoa.Pais = repPais.BuscarPorCodigo(Request.GetIntParam("Pais"));
            if (indicadorIE > 0)
                pessoa.IndicadorIE = indicadorIE;
            pessoa.EnderecoDigitado = bool.Parse(Request.Params("EnderecoDigitado"));
            pessoa.Latitude = latitude != "0" ? latitude : null;
            pessoa.Longitude = longitude != "0" ? longitude : null;
            pessoa.LatitudeTransbordo = latitudeTransbordo;
            pessoa.LongitudeTransbordo = longitudeTransbordo;
            pessoa.Complemento = complemento.Length <= 60 ? complemento : complemento.Substring(0, 60);
            if (adicionar)
                pessoa.DataCadastro = DateTime.Now;
            pessoa.Endereco = endereco;
            pessoa.IE_RG = iE_RG;
            pessoa.CodigoIntegracao = codigoIntegracao;
            pessoa.Email = email;
            pessoa.RG_Passaporte = rG;
            pessoa.NumeroCUITRUT = Request.GetStringParam("NumeroCUITRUT");
            pessoa.Localidade = new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Localidade")) };
            pessoa.Nome = nome;
            pessoa.NomeFantasia = fantasia.Length > 80 ? fantasia.Substring(0, 80) : fantasia;
            pessoa.NomeVisoesBI = nomeVisoesBI;
            pessoa.Numero = numero;
            pessoa.Telefone1 = telefonePrincipal;
            pessoa.Telefone2 = telefoneSecundario;
            pessoa.TipoLocalizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao)int.Parse(Request.Params("TipoLocalizacao"));
            pessoa.Observacao = observacao;
            pessoa.NomeNomenclaturaArquivosDownloadCTe = Request.GetStringParam("NomeNomenclaturaArquivosDownloadCTe");
            pessoa.IntegradoERP = false;

            if (pessoa.Latitude == null || pessoa.Longitude == null)
                pessoa.GeoLocalizacaoStatus = GeoLocalizacaoStatus.NaoGerado;
            else
                pessoa.GeoLocalizacaoStatus = GeoLocalizacaoStatus.Gerado;

            pessoa.GeoLocalizacaoRaioLocalidade = GeoLocalizacaoRaioLocalidade.NaoValidado;

            if (tipoPessoa == 0)
            {
                pessoa.Tipo = "F";
                pessoa.NomeFantasia = nome.Length > 80 ? nome.Substring(0, 80) : fantasia;
            }
            else if (tipoPessoa == 1)
                pessoa.Tipo = "J";
            else
                pessoa.Tipo = "E";

            pessoa.GrupoPessoas = grupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(grupoPessoa) : null;

            if (pessoa.Tipo == "J" && pessoa.GrupoPessoas == null)
            {
                if (ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
                {
                    cpfcnpj = Utilidades.String.OnlyNumbers(cpfcnpj).Remove(8, 6);
                }
                else if (ConfiguracaoEmbarcador.Pais == TipoPais.Exterior)
                {
                    cpfcnpj = Utilidades.String.OnlyNumbers(cpfcnpj);
                }

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(cpfcnpj);
                if (grupoPessoas != null)
                    pessoa.GrupoPessoas = grupoPessoas;
            }

            pessoa.Categoria = categoria > 0 ? repCategoriaPessoa.BuscarPorCodigo(categoria) : null;
            if (pessoa.Categoria == null && ConfiguracaoEmbarcador.ExigirCategoriaCadastroPessoa)
                throw new ControllerException(Localization.Resources.Pessoas.Pessoa.CategoriaObrigatoria);

            if (string.IsNullOrWhiteSpace(pessoa.Telefone1))
                throw new ControllerException(Localization.Resources.Pessoas.Pessoa.TelefonePrincipalObrigatório);

            dynamic dynAcessoPortal = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("AcessoPortal"));
            pessoa.AtivarAcessoFornecedor = ((string)dynAcessoPortal.AtivarAcessoPortal).ToBool();
            pessoa.HabilitarFornecedorParaLancamentoOrdemServico = ((string)dynAcessoPortal.HabilitarFornecedorParaLancamentoOrdemServico).ToBool();
            pessoa.CompartilharAcessoEntreGrupoPessoas = ((string)dynAcessoPortal.CompartilharAcessoEntreGrupoPessoas).ToBool();
            pessoa.VisualizarApenasParaPedidoDesteTomador = ((string)dynAcessoPortal.VisualizarApenasParaPedidoDesteTomador).ToBool();
            pessoa.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas = ((string)dynAcessoPortal.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas).ToBool();
            pessoa.DesabilitarCancelamentoAgendamentoColeta = ((string)dynAcessoPortal.DesabilitarCancelamentoAgendamentoColeta).ToBool();

            RegimeTributario? regimeTributario = Request.GetNullableEnumParam<RegimeTributario>("RegimeTributario");
            TipoLogradouro tipoLogradouro = (TipoLogradouro)int.Parse(Request.Params("TipoLogradouro"));
            TipoEndereco tipoEndereco = (TipoEndereco)int.Parse(Request.Params("TipoEndereco"));
            TipoEmail tipoEmail = (TipoEmail)int.Parse(Request.Params("TipoEmail"));
            bool naoAtualizarDados = bool.Parse(Request.Params("NaoAtualizarDados"));
            bool naoUsarConfiguracaoEmissaoGrupo = bool.Parse(Request.Params("NaoUsarConfiguracaoEmissaoGrupo"));
            bool naoUsarConfiguracaoFaturaGrupo = bool.Parse(Request.Params("NaoUsarConfiguracaoFaturaGrupo"));
            bool possuiRestricaoTrafego = bool.Parse(Request.Params("PossuiRestricaoTrafego"));
            bool verificarUnidadeNegocioPorDestinatario = bool.Parse(Request.Params("VerificarUnidadeNegocioPorDestinatario"));

            pessoa.NaoAtualizarDados = naoAtualizarDados;
            pessoa.Funcionario = Request.GetBoolParam("Funcionario");
            pessoa.Motorista = Request.GetBoolParam("Motorista");
            pessoa.RegimeTributario = regimeTributario;
            pessoa.TipoLogradouro = tipoLogradouro;
            pessoa.TipoEmail = tipoEmail;
            pessoa.TipoEndereco = tipoEndereco;
            pessoa.NaoUsarConfiguracaoEmissaoGrupo = naoUsarConfiguracaoEmissaoGrupo;
            pessoa.NaoUsarConfiguracaoFaturaGrupo = naoUsarConfiguracaoFaturaGrupo;
            pessoa.Integrado = false;
            pessoa.PossuiRestricaoTrafego = possuiRestricaoTrafego;
            pessoa.VerificarUnidadeNegocioPorDestinatario = verificarUnidadeNegocioPorDestinatario;
            pessoa.ExcecaoCheckinFilaH = Request.GetBoolParam("ExcecaoCheckinFilaH");
            pessoa.PontoTransbordo = Request.GetBoolParam("PontoTransbordo");
            pessoa.DigitalizaCanhoto = Request.GetBoolParam("DigitalizaCanhoto");
            pessoa.NaoEmitirCTeFilialEmissora = Request.GetBoolParam("NaoEmitirCTeFilialEmissora");
            pessoa.AguardandoConferenciaInformacao = Request.GetBoolParam("AguardandoConferenciaInformacao");
            pessoa.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa = Request.GetBoolParam("NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa");
            pessoa.ExigirComprovantesLiberacaoPagamentoContratoFrete = Request.GetBoolParam("ExigirComprovantesLiberacaoPagamentoContratoFrete");
            pessoa.DataUltimaAtualizacao = DateTime.Now;
            pessoa.RaioEmMetros = Request.GetIntParam("RaioEmMetros");
            pessoa.AtualizarPontoApoioMaisProximoAutomaticamente = Request.GetBoolParam("AtualizarPontoApoioMaisProximoAutomaticamente");
            int pontoApoioID = Request.GetIntParam("PontoDeApoio");
            pessoa.PontoDeApoio = pontoApoioID > 0 ? new Dominio.Entidades.Embarcador.Logistica.Locais() { Codigo = pontoApoioID } : null;
            pessoa.Area = Request.GetStringParam("Area");
            pessoa.TipoArea = Request.GetEnumParam<TipoArea>("TipoArea");
            pessoa.AlvoEstrategico = Request.GetBoolParam("AlvoEstrategico");
            pessoa.TipoClienteIntegracaoLBC = Request.GetEnumParam<TipoClienteIntegracaoLBC>("TipoClienteIntegracaoLBC");
            pessoa.ObrigarInformarDataEntregaClienteAoBaixarCanhotos = Request.GetBoolParam("ObrigarInformarDataEntregaClienteAoBaixarCanhotos");

            pessoa.LerVeiculoObservacaoNotaParaAbastecimento = Request.GetBoolParam("LerVeiculoObservacaoNotaParaAbastecimento");
            pessoa.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe = Request.GetBoolParam("ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe");
            pessoa.LerPlacaObservacaoNotaParaAbastecimentoInicial = Request.Params("LerPlacaObservacaoNotaParaAbastecimentoInicial");
            pessoa.LerPlacaObservacaoNotaParaAbastecimentoFinal = Request.Params("LerPlacaObservacaoNotaParaAbastecimentoFinal");
            pessoa.LerKMObservacaoNotaParaAbastecimentoInicial = Request.Params("LerKMObservacaoNotaParaAbastecimentoInicial");
            pessoa.LerKMObservacaoNotaParaAbastecimentoFinal = Request.Params("LerKMObservacaoNotaParaAbastecimentoFinal");
            pessoa.LerHorimetroObservacaoNotaParaAbastecimentoInicial = Request.Params("LerHorimetroObservacaoNotaParaAbastecimentoInicial");
            pessoa.LerHorimetroObservacaoNotaParaAbastecimentoFinal = Request.Params("LerHorimetroObservacaoNotaParaAbastecimentoFinal");
            pessoa.LerChassiObservacaoNotaParaAbastecimentoInicial = Request.GetStringParam("LerChassiObservacaoNotaParaAbastecimentoInicial");
            pessoa.LerChassiObservacaoNotaParaAbastecimentoFinal = Request.GetStringParam("LerChassiObservacaoNotaParaAbastecimentoFinal");

            //dynamic dynDadosArmador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DadosArmador"));

            //pessoa.DiasFreetime = ((string)dynDadosArmador.DiasFreetime).ToInt();
            //pessoa.ValorDariaAposFreetime = ((string)dynDadosArmador.ValorDiariaAposFreetime).ToDecimal(); // Request.GetDecimalParam("ValorDiariaAposFreetime");

            //SalvarOutrosCodigosIntegracao(ref pessoa, unitOfWork);

            pessoa.HabilitarSolicitacaoSuprimentoDeGas = Request.GetBoolParam("HabilitarSolicitacaoGas");
        }

        private void SalvarOutrosCodigosIntegracao(ref Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic outrosCodigosIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OutrosCodigosIntegracao"));

            if (pessoa.OutrosCodigosIntegracao == null)
                pessoa.OutrosCodigosIntegracao = new List<string>();

            pessoa.OutrosCodigosIntegracao.Clear();

            foreach (dynamic outroCodigo in outrosCodigosIntegracao)
            {
                if (!string.IsNullOrEmpty((string)outroCodigo.CodigoIntegracao) && repCliente.ValidarCodigoExiste((string)outroCodigo.CodigoIntegracao, pessoa.Codigo))
                    throw new ControllerException(string.Format(Localization.Resources.Pessoas.Pessoa.JaExistePessoaComCodigoIntegracaoCadastrado, (string)outroCodigo.CodigoIntegracao));

                pessoa.OutrosCodigosIntegracao.Add((string)outroCodigo.CodigoIntegracao);
            }
        }

        private void PreecherDadosCliente(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiCliente)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
            if (possuiCliente)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente, unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modaliadeClientePessoas = repModalidadeClientePessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                if (modaliadeClientePessoas == null)
                    modaliadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();

                modaliadeClientePessoas.ModalidadePessoas = modalidadePessoas;
                if (modaliadeClientePessoas.Codigo == 0)
                    repModalidadeClientePessoas.Inserir(modaliadeClientePessoas);
                else
                    repModalidadeClientePessoas.Atualizar(modaliadeClientePessoas);
            }
            else
            {
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente, pessoa.CPF_CNPJ);
                if (modalidadePessoas != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modaliadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();
                    modaliadeClientePessoas = repModalidadeClientePessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                    if (modaliadeClientePessoas != null)
                    {
                        repModalidadeClientePessoas.Deletar(modaliadeClientePessoas);
                    }
                    repModalidadePessoas.Deletar(modalidadePessoas);
                }
            }
        }

        private void PreecherDadosFornecedor(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiFornecedor)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            if (!possuiFornecedor)
            {
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoasRemocao = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, pessoa.CPF_CNPJ);

                if (modalidadePessoasRemocao == null)
                    return;

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoasRemocao = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoasRemocao.Codigo);
                modalidadeFornecedorPessoasRemocao = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoasRemocao.Codigo);

                if (modalidadeFornecedorPessoasRemocao != null)
                {
                    repModalidadeFornecedorPessoas.Deletar(modalidadeFornecedorPessoasRemocao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuModalidade + modalidadeFornecedorPessoasRemocao.Descricao + ".", unitOfWork);
                    return;
                }
            }

            dynamic dynFornecedor = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Fornecedor"));

            if (dynFornecedor.Fornecedor == null)
                return;

            pessoa.SempreConsiderarValorOrcadoFechamentoOrdemServico = ((string)dynFornecedor.Fornecedor.SempreConsiderarValorOrcadoFechamentoOrdemServico).ToBool();
            pessoa.GerarDuplicataNotaEntrada = ((string)dynFornecedor.Fornecedor.GerarDuplicataNotaEntrada).ToBool();
            pessoa.DiaPadraoDuplicataNotaEntrada = ((string)dynFornecedor.Fornecedor.DiaPadraoDuplicataNotaEntrada).ToInt();
            pessoa.IntervaloDiasDuplicataNotaEntrada = (string)dynFornecedor.Fornecedor.IntervaloDiasDuplicataNotaEntrada;
            pessoa.ParcelasDuplicataNotaEntrada = ((string)dynFornecedor.Fornecedor.ParcelasDuplicataNotaEntrada).ToInt();
            pessoa.IgnorarDuplicataRecebidaXMLNotaEntrada = ((string)dynFornecedor.Fornecedor.IgnorarDuplicataRecebidaXMLNotaEntrada).ToBool();
            pessoa.CodigoDocumentoFornecedor = (string)dynFornecedor.Fornecedor.CodigoDocumentoFornecedor;
            pessoa.FormaTituloFornecedor = ((string)dynFornecedor.Fornecedor.FormaTituloFornecedor).ToInt() > 0 ? ((string)dynFornecedor.Fornecedor.FormaTituloFornecedor).ToNullableEnum<FormaTitulo>() : null;
            pessoa.DataUltimaAtualizacao = DateTime.Now;
            pessoa.Integrado = false;
            pessoa.CodigoIntegracaoDuplicataNotaEntrada = (string)dynFornecedor.Fornecedor.CodigoIntegracaoDuplicataNotaEntrada;
            repCliente.Atualizar(pessoa);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(pessoa, TipoModalidade.Fornecedor, unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            if (modalidadeFornecedorPessoas == null)
                modalidadeFornecedorPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas();
            else
                modalidadeFornecedorPessoas.Initialize();

            int codigoEmpresaOficina = ((string)dynFornecedor.Fornecedor.EmpresaOficina).ToInt();

            modalidadeFornecedorPessoas.ModalidadePessoas = modalidadePessoas;
            modalidadeFornecedorPessoas.PostoConveniado = ((string)dynFornecedor.Fornecedor.PostoConveniado).ToBool();
            modalidadeFornecedorPessoas.TextoAviso = (string)dynFornecedor.Fornecedor.AvisoFornecedor;
            modalidadeFornecedorPessoas.PermitirMultiplosVencimentos = ((string)dynFornecedor.Fornecedor.MultiplosVencimentos).ToBool();
            modalidadeFornecedorPessoas.PagoPorFatura = ((string)dynFornecedor.Fornecedor.PagoPorFatura).ToBool();
            modalidadeFornecedorPessoas.Oficina = (bool)dynFornecedor.Fornecedor.Oficina;
            modalidadeFornecedorPessoas.TipoOficina = modalidadeFornecedorPessoas.Oficina ? (TipoOficina)dynFornecedor.Fornecedor.TipoOficina : (TipoOficina?)null;
            modalidadeFornecedorPessoas.EmpresaOficina = codigoEmpresaOficina > 0 && modalidadeFornecedorPessoas.Oficina ? repEmpresa.BuscarPorCodigo(codigoEmpresaOficina) : null;
            modalidadeFornecedorPessoas.PermiteDownloadDocumentos = ((string)dynFornecedor.Fornecedor.PermiteDownloadDocumentos).ToBool();
            modalidadeFornecedorPessoas.EnviarEmailFornecedorDadosTransporte = ((string)dynFornecedor.Fornecedor.EnviarEmailFornecedorDadosTransporte).ToBool();
            modalidadeFornecedorPessoas.NaoEObrigatorioInformarNfeNaColeta = ((string)dynFornecedor.Fornecedor.NaoEObrigatorioInformarNfeNaColeta).ToBool();
            modalidadeFornecedorPessoas.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento = ((string)dynFornecedor.Fornecedor.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento).ToBool();
            modalidadeFornecedorPessoas.GerarAgendamentoSomentePedidosExistentes = ((string)dynFornecedor.Fornecedor.GerarAgendamentoSomentePedidosExistentes).ToBool();

            SalarConfiguracaoPortalFornecedor(dynFornecedor, modalidadeFornecedorPessoas, unitOfWork);

            if (modalidadeFornecedorPessoas.Codigo == 0)
            {
                repModalidadeFornecedorPessoas.Inserir(modalidadeFornecedorPessoas, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouModalidadeFornecedor + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
            }
            else
            {
                repModalidadeFornecedorPessoas.Atualizar(modalidadeFornecedorPessoas, Auditado);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = modalidadeFornecedorPessoas.GetChanges();
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouModalidadeFornecedor + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
            }

            SalvarTabelaValoresFornecedor(dynFornecedor, modalidadeFornecedorPessoas, unitOfWork);

            SalvarModalidadeFornecedorPessoasRestricaoModeloVeicular(dynFornecedor, modalidadeFornecedorPessoas, unitOfWork);
        }

        private void SalarConfiguracaoPortalFornecedor(dynamic dynFornecedor, Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (modalidadeFornecedorPessoas.TipoOperacoes != null) modalidadeFornecedorPessoas.TipoOperacoes.Clear();
            else modalidadeFornecedorPessoas.TipoOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            if (modalidadeFornecedorPessoas.Transportadores != null) modalidadeFornecedorPessoas.Transportadores.Clear();
            else modalidadeFornecedorPessoas.Transportadores = new List<Dominio.Entidades.Empresa>();

            if (modalidadeFornecedorPessoas.TipoCargas != null) modalidadeFornecedorPessoas.TipoCargas.Clear();
            else modalidadeFornecedorPessoas.TipoCargas = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            if (modalidadeFornecedorPessoas.ModelosVeicular != null) modalidadeFornecedorPessoas.ModelosVeicular.Clear();
            else modalidadeFornecedorPessoas.ModelosVeicular = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            if (modalidadeFornecedorPessoas.Destinatarios != null) modalidadeFornecedorPessoas.Destinatarios.Clear();
            else modalidadeFornecedorPessoas.Destinatarios = new List<Dominio.Entidades.Cliente>();

            foreach (dynamic dynTipoOperacao in dynFornecedor.Fornecedor.TipoOperacoes)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperaocao = repTipoOperacao.BuscarPorCodigo(((string)dynTipoOperacao.Codigo).ToInt());
                if (tipoOperaocao != null)
                    modalidadeFornecedorPessoas.TipoOperacoes.Add(tipoOperaocao);
            }

            foreach (dynamic dynTransportador in dynFornecedor.Fornecedor.Transportadores)
            {
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(((string)dynTransportador.Codigo).ToInt());
                if (transportador != null)
                    modalidadeFornecedorPessoas.Transportadores.Add(transportador);
            }

            foreach (dynamic dynTipoDeCarga in dynFornecedor.Fornecedor.TipoCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoDeCarga.BuscarPorCodigo(((string)dynTipoDeCarga.Codigo).ToInt());
                if (tipoCarga != null)
                    modalidadeFornecedorPessoas.TipoCargas.Add(tipoCarga);
            }

            foreach (dynamic dynModeloVeicularCarga in dynFornecedor.Fornecedor.ModelosVeicular)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(((string)dynModeloVeicularCarga.Codigo).ToInt());
                if (modeloVeicularCarga != null)
                    modalidadeFornecedorPessoas.ModelosVeicular.Add(modeloVeicularCarga);
            }

            foreach (dynamic dynDestinatario in dynFornecedor.Fornecedor.Destinatarios)
            {
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(((string)dynDestinatario.Codigo).ToDouble());
                if (destinatario != null)
                    modalidadeFornecedorPessoas.Destinatarios.Add(destinatario);
            }
        }

        private void SalvarModalidadeFornecedorPessoasRestricaoModeloVeicular(dynamic dynFornecedor, Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repositorioRestricao = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(unitOfWork);

            List<int> codigosAtualizar = new List<int>();
            foreach (dynamic dynModeloVeicularCarga in dynFornecedor.Fornecedor.RestricaoModelosVeicular)
            {
                int codigo = ((string)dynModeloVeicularCarga.Codigo).ToInt();
                if (codigo > 0)
                    codigosAtualizar.Add(codigo);
            }

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> existentes = repositorioRestricao.BuscarPorModalidadeFornecedorPessoa(modalidadeFornecedorPessoas.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> excluir = (from obj in existentes
                                                                                                                     where !codigosAtualizar.Any(x => x == obj.Codigo)
                                                                                                                     select obj).ToList();

            for (int i = 0; i < excluir.Count; i++)
                repositorioRestricao.Deletar(excluir[i]);

            existentes = (from obj in existentes
                          where codigosAtualizar.Any(x => x == obj.Codigo)
                          select obj).ToList();

            foreach (dynamic dynModeloVeicularCarga in dynFornecedor.Fornecedor.RestricaoModelosVeicular)
            {
                int codigo = ((string)dynModeloVeicularCarga.Codigo).ToInt();

                int codigoModelo = ((string)dynModeloVeicularCarga.CodigoModeloVeicular).ToInt();
                //Se o código do modelo for informado...
                if (codigoModelo > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular registro = (from obj in existentes
                                                                                                                        where obj.Codigo == codigo
                                                                                                                        select obj).FirstOrDefault();

                    if (registro == null)
                        registro = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular()
                        {
                            ModalidadeFornecedorPessoa = modalidadeFornecedorPessoas
                        };

                    registro.ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo(codigoModelo);
                    if (registro.ModeloVeicular == null)
                        continue;

                    int codigoTipoOperacao = ((string)dynModeloVeicularCarga.CodigoTipoOperacao).ToInt();
                    if (codigoTipoOperacao > 0)
                        registro.TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                    if (registro.Codigo > 0)
                        repositorioRestricao.Atualizar(registro);
                    else
                        repositorioRestricao.Inserir(registro);
                }
            }
        }

        private void SalvarTabelaValoresFornecedor(dynamic dynFornecedor, Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            Dominio.Entidades.Cliente pessoa = modalidadeFornecedorPessoas.ModalidadePessoas.Cliente;

            if (modalidadeFornecedorPessoas != null && modalidadeFornecedorPessoas.TabelasValores != null)
            {
                for (int i = 0; i < modalidadeFornecedorPessoas.TabelasValores.Count(); i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuModalidadeFornecedor + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
                    repPostoCombustivelTabelaValores.DeletarRegistroImportacaoTabelaValor(modalidadeFornecedorPessoas.TabelasValores[i].Codigo);
                    repPostoCombustivelTabelaValores.Deletar(modalidadeFornecedorPessoas.TabelasValores[i]);
                }
            }

            foreach (dynamic dynTabelaValor in dynFornecedor.TabelaValores)
            {
                Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores postoCombustivelTabelaValores = new Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores();
                postoCombustivelTabelaValores.ModalidadeFornecedorPessoas = modalidadeFornecedorPessoas;
                if (!string.IsNullOrWhiteSpace((string)dynTabelaValor.PercentualDesconto.val))
                    postoCombustivelTabelaValores.PercentualDesconto = decimal.Parse((string)dynTabelaValor.PercentualDesconto.val);
                else
                    postoCombustivelTabelaValores.PercentualDesconto = 0;

                postoCombustivelTabelaValores.CodigoIntegracao = (string)dynTabelaValor.CodigoIntegracao.val;
                postoCombustivelTabelaValores.Produto = repProduto.BuscarPorCodigo(0, (int)dynTabelaValor.CodigoProduto.val);
                postoCombustivelTabelaValores.UnidadeDeMedida = (UnidadeDeMedida)(int)dynTabelaValor.CodigoUnidadeDeMedida.val;

                if (!string.IsNullOrWhiteSpace((string)dynTabelaValor.ValorFixo.val))
                    postoCombustivelTabelaValores.ValorFixo = decimal.Parse((string)dynTabelaValor.ValorFixo.val);
                else
                    postoCombustivelTabelaValores.ValorFixo = 0;

                if (!string.IsNullOrWhiteSpace((string)dynTabelaValor.ValorAte.val))
                    postoCombustivelTabelaValores.ValorAte = decimal.Parse((string)dynTabelaValor.ValorAte.val);
                else
                    postoCombustivelTabelaValores.ValorAte = 0;

                if (!string.IsNullOrWhiteSpace((string)dynTabelaValor.DataInicial.val))
                    postoCombustivelTabelaValores.DataInicial = DateTime.Parse((string)dynTabelaValor.DataInicial.val);
                else
                    postoCombustivelTabelaValores.DataInicial = null;

                if (!string.IsNullOrWhiteSpace((string)dynTabelaValor.DataFinal.val))
                    postoCombustivelTabelaValores.DataFinal = DateTime.Parse((string)dynTabelaValor.DataFinal.val);
                else
                    postoCombustivelTabelaValores.DataFinal = null;

                postoCombustivelTabelaValores.MoedaCotacaoBancoCentral = ((string)dynTabelaValor.MoedaCotacaoBancoCentral.val).ToNullableEnum<MoedaCotacaoBancoCentral>();
                postoCombustivelTabelaValores.ValorMoedaCotacao = ((string)dynTabelaValor.ValorMoedaCotacao.val).ToDecimal();
                postoCombustivelTabelaValores.ValorOriginalMoedaEstrangeira = ((string)dynTabelaValor.ValorOriginalMoedaEstrangeira.val).ToDecimal();
                postoCombustivelTabelaValores.Integrado = false;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouPostoCombustivel + postoCombustivelTabelaValores.Descricao + ".", unitOfWork);
                repPostoCombustivelTabelaValores.Inserir(postoCombustivelTabelaValores, Auditado);
            }

            if (modalidadeFornecedorPessoas.PermitirMultiplosVencimentos)
            {
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento> listaVencimentos = repVencimento.BuscarPorPessoa(pessoa.CPF_CNPJ);
                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento in listaVencimentos)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.DeletouVencimento + vencimento.DataEmissao + ".", unitOfWork);
                    repVencimento.Deletar(vencimento);
                }

                foreach (dynamic dynTabelaMultiplosVencimentos in dynFornecedor.TabelaMultiplosVencimentos)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = new Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento();
                    vencimento.Cliente = pessoa;
                    vencimento.DiaEmissaoInicial = int.Parse((string)dynTabelaMultiplosVencimentos.DiaEmissaoInicial.val);
                    vencimento.DiaEmissaoFinal = int.Parse((string)dynTabelaMultiplosVencimentos.DiaEmissaoFinal.val);
                    vencimento.Vencimento = dynTabelaMultiplosVencimentos.Vencimento.val;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouVencimento + vencimento.Vencimento + ".", unitOfWork);
                    repVencimento.Inserir(vencimento);
                }
            }
        }

        private void PreecherTransportadorTerceiro(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiTransporteTerceiro)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);

            if (possuiTransporteTerceiro)

            {
                dynamic dynTransportadorTerceiro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TransportadorTerceiro"));

                decimal descontoPadrao = 0;
                decimal percentualCobradoPadrao = 0;
                string rNTRC = "";
                string observacaoTransportadorTerceiro = "";
                string textoAdicionalContratoFrete = "";
                decimal aliquotaPIS = 0m;
                decimal aliquotaCOFINS = 0m;
                decimal percentualAdiantamentoFretesTerceiro = 0m;
                decimal percentualAbastecimentoFretesTerceiro = 0m;
                bool gerarCIOT = false;
                int codigoPagamentoMotoristaTipo = 0;
                bool gerarPagamentoTerceiro = false;
                bool exigeCanhotoFechamentoContratoFrete = true;
                bool habilitarDataFixaVencimento = false;
                string observacaoContratoFrete = string.Empty;
                string codigoIntegracaoTributaria = string.Empty;
                bool? reterImpostosContratoFrete = null;
                bool? naoSomarValorPedagioContratoFrete = null;
                int? diasVencimentoAdiantamentoContratoFrete = null;
                int? diasVencimentoSaldoContratoFrete = null;
                int? quantidadeDependentes = null;
                DateTime? dataEmissaoRNTRC = null;
                DateTime? dataVencimentoRNTRC = null;
                string codigoINSS = null;
                string numeroCartao = null;
                int codigoProvedor = 0;
                int codigoTipoTerceiro = 0;
                int codigoConfiguracaoCIOT = 0;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT? tipoFavorecidoCIOT = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT? tipoGeracaoCIOT = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT? tipoQuitacaoCIOT = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT? tipoAdiantamentoCIOT = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete? tipoPagamentoContratoFreteTerceiro = null;

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
                Repositorio.Embarcador.Pessoas.TipoTerceiro repTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo tipoTransportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;

                if (dynTransportadorTerceiro != null)
                {
                    if (Enum.TryParse((string)dynTransportadorTerceiro.TipoFavorecidoCIOT, false, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT tipoFavorecidoCIOTAux))
                        tipoFavorecidoCIOT = tipoFavorecidoCIOTAux;

                    if (Enum.TryParse((string)dynTransportadorTerceiro.TipoPagamentoCIOT, false, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT tipoPagamentoCIOTAux))
                        tipoPagamentoCIOT = tipoPagamentoCIOTAux;

                    if (Enum.TryParse((string)dynTransportadorTerceiro.TipoGeracaoCIOT, false, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT tipoGeracaoCIOTAux))
                        tipoGeracaoCIOT = tipoGeracaoCIOTAux;

                    if (Enum.TryParse((string)dynTransportadorTerceiro.TipoQuitacaoCIOT, false, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT tipoQuitacaoCIOTAux))
                        tipoQuitacaoCIOT = tipoQuitacaoCIOTAux;

                    if (Enum.TryParse((string)dynTransportadorTerceiro.TipoAdiantamentoCIOT, false, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT tipoAdiantamentoCIOTAux))
                        tipoAdiantamentoCIOT = tipoAdiantamentoCIOTAux;

                    if (Enum.TryParse((string)dynTransportadorTerceiro.TipoPagamentoContratoFreteTerceiro, false, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete tipoPagamentoContratoFreteTerceiroAux))
                        tipoPagamentoContratoFreteTerceiro = tipoPagamentoContratoFreteTerceiroAux;

                    string sDescontoPadrao = (string)dynTransportadorTerceiro.DescontoPadrao;

                    numeroCartao = (string)dynTransportadorTerceiro.NumeroCartao;
                    codigoINSS = (string)dynTransportadorTerceiro.CodigoINSS;
                    rNTRC = (string)dynTransportadorTerceiro.RNTRC;
                    observacaoTransportadorTerceiro = (string)dynTransportadorTerceiro.ObservacaoCTe;
                    observacaoContratoFrete = (string)dynTransportadorTerceiro.ObservacaoContratoFrete;
                    textoAdicionalContratoFrete = (string)dynTransportadorTerceiro.TextoAdicionalContratoFrete;
                    codigoIntegracaoTributaria = (string)dynTransportadorTerceiro.CodigoIntegracaoTributaria;

                    if (rNTRC.Length < 8)
                        throw new ControllerException(Localization.Resources.Pessoas.Pessoa.RNTRCInvalidoNecessario8Digitos);

                    if (DateTime.TryParseExact((string)dynTransportadorTerceiro.DataEmissaoRNTRC, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoRNTRCAux))
                        dataEmissaoRNTRC = dataEmissaoRNTRCAux;

                    if (DateTime.TryParseExact((string)dynTransportadorTerceiro.DataVencimentoRNTRC, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimentoRNTRCAux))
                        dataVencimentoRNTRC = dataVencimentoRNTRCAux;

                    if (int.TryParse((string)dynTransportadorTerceiro.DiasVencimentoAdiantamentoContratoFrete, out int diasVencimentoAdiantamentoContratoFreteAux) && diasVencimentoAdiantamentoContratoFreteAux > 0)
                        diasVencimentoAdiantamentoContratoFrete = diasVencimentoAdiantamentoContratoFreteAux;

                    if (int.TryParse((string)dynTransportadorTerceiro.DiasVencimentoSaldoContratoFrete, out int diasVencimentoSaldoContratoFreteAux) && diasVencimentoSaldoContratoFreteAux > 0)
                        diasVencimentoSaldoContratoFrete = diasVencimentoSaldoContratoFreteAux;

                    if (bool.TryParse((string)dynTransportadorTerceiro.ReterImpostosContratoFrete, out bool reterImpostosContratoFreteAux))
                        reterImpostosContratoFrete = reterImpostosContratoFreteAux;
                    else
                        reterImpostosContratoFrete = null;

                    if (bool.TryParse((string)dynTransportadorTerceiro.NaoSomarValorPedagioContratoFrete, out bool naoSomarValorPedagioContratoFreteAux))
                        naoSomarValorPedagioContratoFrete = naoSomarValorPedagioContratoFreteAux;
                    else
                        naoSomarValorPedagioContratoFrete = null;

                    if (int.TryParse((string)dynTransportadorTerceiro.QuantidadeDependentes, out int quantidadeDependentesAux) && quantidadeDependentesAux > 0)
                        quantidadeDependentes = quantidadeDependentesAux;

                    if (!string.IsNullOrEmpty(sDescontoPadrao))
                        descontoPadrao = decimal.Parse(dynTransportadorTerceiro.DescontoPadrao.ToString());

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.PercentualCobradoPadrao))
                        percentualCobradoPadrao = decimal.Parse(dynTransportadorTerceiro.PercentualCobradoPadrao.ToString());

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.PercentualAdiantamentoFretesTerceiro))
                        percentualAdiantamentoFretesTerceiro = decimal.Parse(dynTransportadorTerceiro.PercentualAdiantamentoFretesTerceiro.ToString());

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.AliquotaCOFINS))
                        aliquotaCOFINS = decimal.Parse(dynTransportadorTerceiro.AliquotaCOFINS.ToString());

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.AliquotaPIS))
                        aliquotaPIS = decimal.Parse(dynTransportadorTerceiro.AliquotaPIS.ToString());

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.GerarCIOT))
                        gerarCIOT = (bool)dynTransportadorTerceiro.GerarCIOT;

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.GerarPagamentoTerceiro))
                        gerarPagamentoTerceiro = (bool)dynTransportadorTerceiro.GerarPagamentoTerceiro;

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.PagamentoMotoristaTipo))
                        codigoPagamentoMotoristaTipo = ((string)dynTransportadorTerceiro.PagamentoMotoristaTipo).ToInt();

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.ExigeCanhotoFechamentoContratoFrete))
                        exigeCanhotoFechamentoContratoFrete = (bool)dynTransportadorTerceiro.ExigeCanhotoFechamentoContratoFrete;

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.HabilitarDataFixaVencimento))
                        habilitarDataFixaVencimento = (bool)dynTransportadorTerceiro.HabilitarDataFixaVencimento;

                    if (int.TryParse((string)dynTransportadorTerceiro.CodigoProvedor, out int codigoProvedorAux))
                        codigoProvedor = codigoProvedorAux;

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.TipoTerceiro))
                        codigoTipoTerceiro = ((string)dynTransportadorTerceiro.TipoTerceiro).ToInt();

                    if (!string.IsNullOrWhiteSpace((string)dynTransportadorTerceiro.ConfiguracaoCIOT))
                        codigoConfiguracaoCIOT = ((string)dynTransportadorTerceiro.ConfiguracaoCIOT).ToInt();

                    tipoTransportador = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo)dynTransportadorTerceiro.TipoTransportadorTerceiro;
                    percentualAbastecimentoFretesTerceiro = Utilidades.Decimal.Converter((string)dynTransportadorTerceiro.PercentualAbastecimentoFretesTerceiro);
                }

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                bool inserir = false;
                if (modalidadeTransportadoraPessoas == null)
                {
                    modalidadeTransportadoraPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();
                    inserir = true;
                }
                else
                    modalidadeTransportadoraPessoas.Initialize();

                modalidadeTransportadoraPessoas.TipoQuitacaoCIOT = tipoQuitacaoCIOT;
                modalidadeTransportadoraPessoas.TipoAdiantamentoCIOT = tipoAdiantamentoCIOT;
                modalidadeTransportadoraPessoas.TipoGeracaoCIOT = tipoGeracaoCIOT;
                modalidadeTransportadoraPessoas.NumeroCartao = numeroCartao;
                modalidadeTransportadoraPessoas.TipoPagamentoCIOT = tipoPagamentoCIOT;
                modalidadeTransportadoraPessoas.TipoFavorecidoCIOT = tipoFavorecidoCIOT;
                modalidadeTransportadoraPessoas.DataEmissaoRNTRC = dataEmissaoRNTRC;
                modalidadeTransportadoraPessoas.DataVencimentoRNTRC = dataVencimentoRNTRC;
                modalidadeTransportadoraPessoas.CodigoINSS = codigoINSS;
                modalidadeTransportadoraPessoas.DiasVencimentoSaldoContratoFrete = diasVencimentoSaldoContratoFrete;
                modalidadeTransportadoraPessoas.DiasVencimentoAdiantamentoContratoFrete = diasVencimentoAdiantamentoContratoFrete;
                modalidadeTransportadoraPessoas.ReterImpostosContratoFrete = reterImpostosContratoFrete;
                modalidadeTransportadoraPessoas.TextoAdicionalContratoFrete = textoAdicionalContratoFrete;
                modalidadeTransportadoraPessoas.ModalidadePessoas = modalidadePessoas;
                modalidadeTransportadoraPessoas.PercentualDesconto = descontoPadrao;
                modalidadeTransportadoraPessoas.PercentualCobranca = percentualCobradoPadrao;
                modalidadeTransportadoraPessoas.PercentualAdiantamentoFretesTerceiro = percentualAdiantamentoFretesTerceiro;
                modalidadeTransportadoraPessoas.AliquotaPIS = aliquotaPIS;
                modalidadeTransportadoraPessoas.AliquotaCOFINS = aliquotaCOFINS;
                modalidadeTransportadoraPessoas.RNTRC = rNTRC;
                modalidadeTransportadoraPessoas.ObservacaoCTe = observacaoTransportadorTerceiro;
                modalidadeTransportadoraPessoas.GerarCIOT = gerarCIOT;
                modalidadeTransportadoraPessoas.PagamentoMotoristaTipo = codigoPagamentoMotoristaTipo > 0 ? repPagamentoMotoristaTipo.BuscarPorCodigo(codigoPagamentoMotoristaTipo) : null;
                modalidadeTransportadoraPessoas.GerarPagamentoTerceiro = gerarPagamentoTerceiro;

                modalidadeTransportadoraPessoas.TipoTransportador = tipoTransportador;
                modalidadeTransportadoraPessoas.ObservacaoContratoFrete = observacaoContratoFrete;
                modalidadeTransportadoraPessoas.ExigeCanhotoFechamentoContratoFrete = exigeCanhotoFechamentoContratoFrete;
                modalidadeTransportadoraPessoas.HabilitarDataFixaVencimento = habilitarDataFixaVencimento;
                modalidadeTransportadoraPessoas.PercentualAbastecimentoFretesTerceiro = percentualAbastecimentoFretesTerceiro;
                modalidadeTransportadoraPessoas.CodigoIntegracaoTributaria = codigoIntegracaoTributaria;
                modalidadeTransportadoraPessoas.TipoPagamentoContratoFreteTerceiro = tipoPagamentoContratoFreteTerceiro;
                modalidadeTransportadoraPessoas.NaoSomarValorPedagioContratoFrete = naoSomarValorPedagioContratoFrete;
                modalidadeTransportadoraPessoas.QuantidadeDependentes = quantidadeDependentes;
                modalidadeTransportadoraPessoas.CodigoProvedor = codigoProvedor;
                modalidadeTransportadoraPessoas.TipoTerceiro = codigoTipoTerceiro > 0 ? repTipoTerceiro.BuscarPorCodigo(codigoTipoTerceiro) : null;
                modalidadeTransportadoraPessoas.ConfiguracaoCIOT = codigoConfiguracaoCIOT > 0 ? repConfiguracaoCIOT.BuscarPorCodigo(codigoConfiguracaoCIOT) : null;
                SalvarAcrescimoDescontoAutomatico(pessoa, unitOfWork);
                SalvarDataFixaVencimento(pessoa, unitOfWork);

                if (inserir)
                {
                    repModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadoraPessoas);

                    if (!gerarCIOT)
                    {
                        Repositorio.Embarcador.Configuracoes.Integracao repConfiguracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao = repConfiguracao.Buscar();

                        if (configuracao != null)
                        {
                            Servicos.Embarcador.CIOT.EFrete serEFrete = new Servicos.Embarcador.CIOT.EFrete();
                            bool gerar = false;//serEFrete.IntegrarClienteProprietario(pessoa, configuracao, unitOfWork, out mensagem);

                            if (gerar)
                            {
                                modalidadeTransportadoraPessoas.GerarCIOT = true;
                                repModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportadoraPessoas);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.NaConsultaDoFreteProprietarioDeveEmitirCIOT, unitOfWork);
                            }
                        }
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouModalidadeTransportador + modalidadeTransportadoraPessoas.Descricao + ".", unitOfWork);
                }
                else
                {
                    repModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportadoraPessoas);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = modalidadeTransportadoraPessoas.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouModalidadeTransportador + modalidadeTransportadoraPessoas.Descricao + ".", unitOfWork);
                }

                SalvarTiposPagamentoCIOT(modalidadeTransportadoraPessoas, unitOfWork, dynTransportadorTerceiro);
                SalvarDiasFechamentoCIOTPeriodo(modalidadeTransportadoraPessoas, unitOfWork, dynTransportadorTerceiro);
            }
            else
            {
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, pessoa.CPF_CNPJ);
                if (modalidadePessoas != null)
                {
                    repModalidadePessoas.Deletar(modalidadePessoas);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuModalidadeTransportador + ".", unitOfWork);
                }
            }
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas RetornarModalidadePessoa(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(tipoModalidade, pessoa.CPF_CNPJ);

            if (modalidadePessoas == null)
            {
                modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                modalidadePessoas.Cliente = pessoa;
                modalidadePessoas.TipoModalidade = tipoModalidade;
                repModalidadePessoas.Inserir(modalidadePessoas);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouModalidade + modalidadePessoas.DescricaoTipoModalidade + Localization.Resources.Pessoas.Pessoa.APessoa, unitOfWork);
            }
            return modalidadePessoas;
        }

        private void SalvarDadosDescarga(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (string.IsNullOrWhiteSpace(Request.Params("Descarga")))
                return;

            dynamic descarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Descarga"));

            if (descarga != null)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unidadeDeTrabalho);
                    Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unidadeDeTrabalho);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                    Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);
                    Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);


                    List<int> codigosRestricoes = Request.GetListParam<int>("Restricoes");

                    Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(cliente.CPF_CNPJ);

                    decimal.TryParse(descarga.ValorPorPallet.ToString(), out decimal valorPorPallet);
                    decimal.TryParse(descarga.ValorPorVolume.ToString(), out decimal valorPorVolume);

                    int codigoFilial = ((string)descarga.FilialResponsavelRedespacho).ToString().ToInt();
                    int codigoTipoDeCarga = ((string)descarga.TipoDeCarga).ToString().ToInt();

                    bool inserir = false;
                    if (clienteDescarga == null)
                    {
                        clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga();
                        inserir = true;
                    }
                    else
                        clienteDescarga.Initialize();

                    clienteDescarga.HoraLimiteDescarga = descarga.HoraLimiteDescarga.ToString();
                    clienteDescarga.Cliente = cliente;
                    clienteDescarga.Distribuidor = repEmpresa.BuscarPorCodigo((int)descarga.Distribuidor);
                    clienteDescarga.VeiculoDistribuidor = repVeiculo.BuscarPorCodigo((int)descarga.VeiculoDistribuidor);
                    clienteDescarga.FilialResponsavelRedespacho = repositorioFilial.BuscarPorCodigo(codigoFilial);
                    clienteDescarga.TipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);

                    clienteDescarga.ValorPorPallet = valorPorPallet;
                    clienteDescarga.ValorPorVolume = valorPorVolume;
                    clienteDescarga.NaoRecebeCargaCompartilhada = descarga.NaoRecebeCargaCompartilhada;
                    clienteDescarga.NaoExigePreenchimentoDeChecklistEntrega = descarga.NaoExigePreenchimentoDeChecklistEntrega;

                    clienteDescarga.AgendamentoExigeNotaFiscal = descarga.AgendamentoExigeNotaFiscal;
                    clienteDescarga.ExigeAgendamento = descarga.ExigeAgendamento;
                    clienteDescarga.TempoAgendamento = descarga.TempoAgendamento;
                    clienteDescarga.FormaAgendamento = descarga.FormaAgendamento;
                    clienteDescarga.LinkParaAgendamento = descarga.LinkParaAgendamento;
                    clienteDescarga.AgendamentoDescargaObrigatorio = descarga.AgendamentoDescargaObrigatorio;
                    clienteDescarga.PossuiCanhotoDeDuasOuMaisPaginas = descarga.PossuiCanhotoDeDuasOuMaisPaginas;
                    clienteDescarga.QuantidadeDePaginasDoCanhoto = descarga.QuantidadeDePaginasDoCanhoto != null ? ((int)descarga.QuantidadeDePaginasDoCanhoto) : 0;
                    clienteDescarga.ExigeSenhaNoAgendamento = descarga.ExigeSenhaNoAgendamento;

                    if (clienteDescarga.Distribuidor != null)
                    {
                        double.TryParse(clienteDescarga.Distribuidor.CNPJ_SemFormato, out double cnpjDistri);

                        Dominio.Entidades.Cliente distribuidor = repCliente.BuscarPorCPFCNPJ(cnpjDistri);
                        if (distribuidor == null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = Servicos.Embarcador.Pessoa.Pessoa.Converter(clienteDescarga.Distribuidor);
                            Servicos.Cliente serCliente = new Servicos.Cliente(unidadeDeTrabalho.StringConexao);
                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(pessoa, "Distribuidor", unidadeDeTrabalho, 0, false);
                            if (!retorno.Status)
                                throw new ControllerException(retorno.Mensagem);
                        }
                    }

                    if (clienteDescarga.RestricoesDescarga == null)
                        clienteDescarga.RestricoesDescarga = new List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega>();
                    clienteDescarga.RestricoesDescarga.Clear();

                    List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricoesDescarga = repRestricaoEntrega.BuscarPorCodigos(codigosRestricoes);

                    foreach (Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricao in restricoesDescarga)
                    {
                        clienteDescarga.RestricoesDescarga.Add(restricao);
                    }

                    if (inserir)
                    {
                        repClienteDescarga.Inserir(clienteDescarga);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouDadoDeDescarga + clienteDescarga.Descricao + ".", unidadeDeTrabalho);
                    }
                    else
                    {
                        repClienteDescarga.Atualizar(clienteDescarga);
                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = clienteDescarga.GetChanges();
                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouDadoDeDescarga + clienteDescarga.Descricao + ".", unidadeDeTrabalho);
                    }
                }
            }
        }

        private void SalvarImportacaoDadosDescarga(Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteDescarga clienteDescargaImportacao, List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna> colunasDescargaImportacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(cliente.CPF_CNPJ);

            bool inserir = false;
            if (clienteDescarga == null)
            {
                clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga();
                inserir = true;
            }
            else
                clienteDescarga.Initialize();

            clienteDescarga.Cliente = cliente;

            foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna in colunasDescargaImportacao)
            {
                switch (coluna.NomeCampo)
                {
                    case "AgendamentoDescargaObrigatorio":
                        clienteDescarga.AgendamentoDescargaObrigatorio = clienteDescargaImportacao.AgendamentoDescargaObrigatorio;
                        break;

                    case "ExigeAgendamento":
                        clienteDescarga.ExigeAgendamento = clienteDescargaImportacao.ExigeAgendamento;
                        break;

                    case "AgendamentoExigeNotaFiscal":
                        clienteDescarga.AgendamentoExigeNotaFiscal = clienteDescargaImportacao.AgendamentoExigeNotaFiscal;
                        break;

                    case "FormaAgendamento":
                        clienteDescarga.FormaAgendamento = clienteDescargaImportacao.FormaAgendamento;
                        break;

                    case "TempoAgendamento":
                        clienteDescarga.TempoAgendamento = clienteDescargaImportacao.TempoAgendamento;
                        break;

                    case "LinkParaAgendamento":
                        clienteDescarga.LinkParaAgendamento = clienteDescargaImportacao.LinkParaAgendamento;
                        break;
                }
            }

            if (inserir)
            {
                repClienteDescarga.Inserir(clienteDescarga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouDadoDeDescarga + clienteDescarga.Descricao + " via planilha.", unitOfWork);
            }
            else
            {
                repClienteDescarga.Atualizar(clienteDescarga);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = clienteDescarga.GetChanges();
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouDadoDeDescarga + clienteDescarga.Descricao + " via planilha.", unitOfWork);
            }
        }

        private dynamic SalvarConfiguracaoEmissaoCTe(Dominio.Entidades.Cliente cliente, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoEmissao))
                return null;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP repClienteIntegracaoFTP = new Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP(unidadeDeTrabalho);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP clienteIntegracaoFTP = repClienteIntegracaoFTP.BuscarPorClienteAsync(cliente.CPF_CNPJ, default).Result;

            dynamic configuracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoEmissaoCTe"));

            int codigoTipoOcorrenciaComplementoSubcontratacao = (int)configuracao.TipoOcorrenciaComplementoSubcontratacao;

            cliente.DisponibilizarDocumentosParaNFsManual = (bool)configuracao.DisponibilizarDocumentosParaNFsManual;
            cliente.TipoOcorrenciaComplementoSubcontratacao = codigoTipoOcorrenciaComplementoSubcontratacao > 0 ? repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrenciaComplementoSubcontratacao) : null;
            cliente.GerarOcorrenciaComplementoSubcontratacao = (bool)configuracao.GerarOcorrenciaComplementoSubcontratacao;
            cliente.ValorFreteLiquidoDeveSerValorAReceber = (bool)configuracao.ValorFreteLiquidoDeveSerValorAReceber;
            cliente.ValorFreteLiquidoDeveSerValorAReceberSemICMS = (bool)configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
            cliente.ValorMaximoEmissaoPendentePagamento = Utilidades.Decimal.Converter((string)configuracao.ValorMaximoEmissaoPendentePagamento);
            cliente.ValorLimiteFaturamento = Utilidades.Decimal.Converter((string)configuracao.ValorLimiteFaturamento);
            int.TryParse((string)configuracao.DiasEmAbertoAposVencimento, out int diasEmAbertoAposVencimento);
            cliente.DiasEmAbertoAposVencimento = diasEmAbertoAposVencimento;
            cliente.TipoEnvioEmail = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe)configuracao.TipoEnvioEmail;
            cliente.ObservacaoEmissaoCarga = (string)configuracao.ObservacaoEmissaoCarga;
            cliente.GerarMDFeTransbordoSemConsiderarOrigem = (bool)configuracao.GerarMDFeTransbordoSemConsiderarOrigem;
            cliente.NaoValidarNotaFiscalExistente = (bool)configuracao.NaoValidarNotaFiscalExistente;
            cliente.NaoValidarNotasFiscaisComDiferentesPortos = (bool)configuracao.NaoValidarNotasFiscaisComDiferentesPortos;
            cliente.AgruparMovimentoFinanceiroPorPedido = (bool)configuracao.AgruparMovimentoFinanceiroPorPedido;
            cliente.ValePedagioObrigatorio = !string.IsNullOrWhiteSpace((string)configuracao.ValePedagioObrigatorio) ? (bool)configuracao.ValePedagioObrigatorio : false;
            cliente.NaoEmitirMDFe = (bool)configuracao.NaoEmitirMDFe;
            cliente.ProvisionarDocumentos = (bool)configuracao.ProvisionarDocumentos;
            cliente.DisponibilizarDocumentosParaLoteEscrituracao = (bool)configuracao.DisponibilizarDocumentosParaLoteEscrituracao;
            cliente.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento = (bool)configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento;
            cliente.DisponibilizarDocumentosParaPagamento = (bool)configuracao.DisponibilizarDocumentosParaPagamento;
            cliente.QuitarDocumentoAutomaticamenteAoGerarLote = ((string)configuracao.QuitarDocumentoAutomaticamenteAoGerarLote).ToBool();
            cliente.EscriturarSomenteDocumentosEmitidosParaNFe = (bool)configuracao.EscriturarSomenteDocumentosEmitidosParaNFe;
            cliente.CTeEmitidoNoEmbarcador = (bool)configuracao.CTeEmitidoNoEmbarcador;
            cliente.ExigirNumeroPedido = (bool)configuracao.ExigirNumeroPedido;
            cliente.RegexValidacaoNumeroPedidoEmbarcador = (string)configuracao.RegexValidacaoNumeroPedidoEmbarcador;
            cliente.TipoEmissaoCTeDocumentos = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos)configuracao.TipoRateioDocumentos;
            cliente.TipoEmissaoCTeParticipantes = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes)configuracao.TipoEmissaoCTeParticipantes;
            cliente.TipoEmissaoIntramunicipal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal)configuracao.TipoEmissaoIntramunicipal;
            cliente.RateioFormula = repFormulaRateio.BuscarPorCodigo((int)configuracao.FormulaRateioFrete);
            cliente.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao)(int)configuracao.TipoIntegracao);
            cliente.ArquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.BuscarPorCodigo((int)configuracao.ArquivoImportacaoNotasFiscais);
            cliente.DescricaoComponenteFreteEmbarcador = (string)configuracao.DescricaoComponenteFreteEmbarcador;
            cliente.BloquearDiferencaValorFreteEmbarcador = (bool)configuracao.BloquearDiferencaValorFreteEmbarcador;
            cliente.EmitirComplementoDiferencaFreteEmbarcador = (bool)configuracao.EmitirComplementoDiferencaFreteEmbarcador;
            cliente.GerarOcorrenciaSemTabelaFrete = (bool)configuracao.GerarOcorrenciaSemTabelaFrete;
            cliente.TipoOcorrenciaSemTabelaFrete = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaSemTabelaFrete);
            cliente.PercentualBloquearDiferencaValorFreteEmbarcador = (decimal)configuracao.PercentualBloquearDiferencaValorFreteEmbarcador;
            cliente.TipoOcorrenciaComplementoDiferencaFreteEmbarcador = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador);
            cliente.TipoOcorrenciaCTeEmitidoEmbarcador = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaCTeEmitidoEmbarcador);
            cliente.ObrigatorioInformarMDFeEmitidoPeloEmbarcador = cliente.CTeEmitidoNoEmbarcador ? (bool)configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador : false;

            double.TryParse((string)configuracao.EmitenteImportacaoRedespachoIntermediario, out double cpfCnpjEmitenteImportacaoRedespachoIntermediario);
            double.TryParse((string)configuracao.ExpedidorImportacaoRedespachoIntermediario, out double cpfCnpjExpedidorImportacaoRedespachoIntermediario);
            double.TryParse((string)configuracao.RecebedorImportacaoRedespachoIntermediario, out double cpfCnpjRecebedorImportacaoRedespachoIntermediario);

            cliente.ImportarRedespachoIntermediario = (bool)configuracao.ImportarRedespachoIntermediario;
            cliente.EmitenteImportacaoRedespachoIntermediario = cpfCnpjEmitenteImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEmitenteImportacaoRedespachoIntermediario) : null;
            cliente.ExpedidorImportacaoRedespachoIntermediario = cpfCnpjExpedidorImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidorImportacaoRedespachoIntermediario) : null;
            cliente.RecebedorImportacaoRedespachoIntermediario = cpfCnpjRecebedorImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedorImportacaoRedespachoIntermediario) : null;

            cliente.DescricaoItemPesoCTeSubcontratacao = (string)configuracao.DescricaoItemPesoCTeSubcontratacao;
            cliente.CaracteristicaTransporteCTe = (string)configuracao.CaracteristicaTransporteCTe;
            cliente.ObservacaoCTe = (string)configuracao.Observacao;
            cliente.ObservacaoCTeTerceiro = (string)configuracao.ObservacaoTerceiro;
            cliente.GerarCIOTParaTodasAsCargas = (bool)configuracao.GerarCIOTParaTodasAsCargas;
            cliente.NaoPermitirVincularCTeComplementarEmCarga = (bool)configuracao.NaoPermitirVincularCTeComplementarEmCarga;
            cliente.TempoCarregamento = RetornarTimeSpan((string)configuracao.TempoCarregamento);
            cliente.TempoDescarregamento = RetornarTimeSpan((string)configuracao.TempoDescarregamento);

            if (cliente.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual)
            {
                cliente.UtilizarOutroModeloDocumentoEmissaoMunicipal = (bool)configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal;

                if (cliente.UtilizarOutroModeloDocumentoEmissaoMunicipal)
                    cliente.ModeloDocumentoFiscalEmissaoMunicipal = repModeloDocumentoFiscal.BuscarPorId((int)configuracao.ModeloDocumentoFiscalEmissaoMunicipal);
                else
                    cliente.ModeloDocumentoFiscalEmissaoMunicipal = null;
            }
            else
            {
                cliente.UtilizarOutroModeloDocumentoEmissaoMunicipal = false;
                cliente.ModeloDocumentoFiscalEmissaoMunicipal = null;
            }

            if (cliente.ApolicesSeguro == null)
                cliente.ApolicesSeguro = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            if (cliente.ClientesBloquearEmissaoDosDestinatario == null)
                cliente.ClientesBloquearEmissaoDosDestinatario = new List<Dominio.Entidades.Cliente>();

            if ((int)configuracao.ModeloDocumentoFiscal > 0)
                cliente.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId((int)configuracao.ModeloDocumentoFiscal);
            else
                cliente.ModeloDocumentoFiscal = null;

            if ((int)configuracao.EmpresaEmissora > 0)
                cliente.EmpresaEmissora = repEmpresa.BuscarPorCodigo((int)configuracao.EmpresaEmissora);
            else
                cliente.EmpresaEmissora = null;

            List<int> codigosApolicesSeguro = new List<int>();

            for (int i = 0; i < configuracao.ApolicesSeguro.Count; i++)
                codigosApolicesSeguro.Add((int)configuracao.ApolicesSeguro[i]);

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesRemover = cliente.ApolicesSeguro.Where(o => !codigosApolicesSeguro.Contains(o.Codigo)).ToList();

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceRemover in apolicesRemover)
            {
                cliente.ApolicesSeguro.Remove(apoliceRemover);

                //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Removeu a apólice de seguro " + apoliceRemover.Descricao + ".", unidadeDeTrabalho);
            }

            foreach (int codigoApoliceSeguro in codigosApolicesSeguro)
            {
                if (!cliente.ApolicesSeguro.Any(o => o.Codigo == codigoApoliceSeguro))
                {
                    Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigoApoliceSeguro);

                    cliente.ApolicesSeguro.Add(apoliceSeguro);

                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Adicionou a apólice de seguro " + apoliceSeguro.Descricao + ".", unidadeDeTrabalho);
                }
            }

            cliente.TipoPropostaMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal)configuracao.TipoPropostaMultimodal;
            cliente.TipoServicoMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal)configuracao.TipoServicoMultimodal;
            cliente.ModalPropostaMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal)configuracao.ModalPropostaMultimodal;
            cliente.TipoCobrancaMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal)configuracao.TipoCobrancaMultimodal;

            bool.TryParse((string)configuracao.BloquearEmissaoDeEntidadeSemCadastro, out bool bloquearEmissaoDeEntidadeSemCadastro);
            bool.TryParse((string)configuracao.BloquearEmissaoDosDestinatario, out bool bloquearEmissaoDosDestinatario);
            cliente.BloquearEmissaoDeEntidadeSemCadastro = bloquearEmissaoDeEntidadeSemCadastro;
            cliente.BloquearEmissaoDosDestinatario = bloquearEmissaoDosDestinatario;

            cliente.TipoIntegracaoMercadoLivre = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre)configuracao.TipoIntegracaoMercadoLivre;
            cliente.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente = (bool)configuracao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente;
            cliente.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente = (bool)configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente;
            cliente.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida)configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida;
            cliente.TempoAcrescimoDecrescimoDataPrevisaoSaida = RetornarTimeSpan((string)configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida);

            List<double> codigosClientesBloqueio = new List<double>();

            for (int i = 0; i < configuracao.ClientesBloqueados.Count; i++)
                codigosClientesBloqueio.Add((double)configuracao.ClientesBloqueados[i]);

            List<Dominio.Entidades.Cliente> clientesRemover = cliente.ClientesBloquearEmissaoDosDestinatario.Where(o => !codigosClientesBloqueio.Contains(o.CPF_CNPJ)).ToList();

            foreach (Dominio.Entidades.Cliente clienteRemover in clientesRemover)
            {
                cliente.ClientesBloquearEmissaoDosDestinatario.Remove(clienteRemover);
            }

            foreach (double codigoClientesBloquei in codigosClientesBloqueio)
            {
                if (!cliente.ClientesBloquearEmissaoDosDestinatario.Any(o => o.Codigo == codigoClientesBloquei))
                {
                    Dominio.Entidades.Cliente clienteBloqueio = repCliente.BuscarPorCPFCNPJ(codigoClientesBloquei);

                    cliente.ClientesBloquearEmissaoDosDestinatario.Add(clienteBloqueio);
                }
            }

            if (cliente.TipoIntegracao != null && cliente.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
            {
                if (clienteIntegracaoFTP == null)
                    clienteIntegracaoFTP = new Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP();
                else
                    clienteIntegracaoFTP.Initialize();

                clienteIntegracaoFTP.Cliente = cliente;
                clienteIntegracaoFTP.Diretorio = (string)configuracao.Diretorio;
                clienteIntegracaoFTP.EnderecoFTP = (string)configuracao.EnderecoFTP;
                clienteIntegracaoFTP.Passivo = (bool)configuracao.Passivo;
                clienteIntegracaoFTP.UtilizarSFTP = (bool)configuracao.UtilizarSFTP;
                clienteIntegracaoFTP.SSL = (bool)configuracao.SSL;
                clienteIntegracaoFTP.Porta = (string)configuracao.Porta;
                clienteIntegracaoFTP.Senha = (string)configuracao.Senha;
                clienteIntegracaoFTP.Usuario = (string)configuracao.Usuario;
                clienteIntegracaoFTP.NomenclaturaArquivo = (string)configuracao.NomenclaturaArquivo;

                if (clienteIntegracaoFTP.Codigo > 0)
                {
                    repClienteIntegracaoFTP.Atualizar(clienteIntegracaoFTP, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = clienteIntegracaoFTP.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouConfiguracaoFTP + clienteIntegracaoFTP.Descricao + ".", unidadeDeTrabalho);
                }
                else
                {
                    repClienteIntegracaoFTP.Inserir(clienteIntegracaoFTP, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouUmaConfiguracaoFTP + clienteIntegracaoFTP.Descricao + ".", unidadeDeTrabalho);
                }
            }
            else if (clienteIntegracaoFTP != null)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.DeletouConfiguracaoFTP + clienteIntegracaoFTP.Descricao + ".", unidadeDeTrabalho);
                repClienteIntegracaoFTP.Deletar(clienteIntegracaoFTP, Auditado);
            }

            SalvarConfiguracaoComponentesFrete(cliente, configuracao.ComponentesFrete, unidadeDeTrabalho);
            cliente.Integrado = false;
            cliente.DataUltimaAtualizacao = DateTime.Now;
            repCliente.Atualizar(cliente);

            return configuracao;
        }

        private void SalvarConfiguracaoComponentesFrete(Dominio.Entidades.Cliente pessoa, dynamic configuracaoComponentes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracaoComponentes != null)
            {
                Repositorio.Embarcador.Pessoas.ClienteConfiguracaoComponentes repClienteConfiguracaoComponentes = new Repositorio.Embarcador.Pessoas.ClienteConfiguracaoComponentes(unidadeDeTrabalho);
                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes> pessoasConfiguracaesComponentesFreteExcluir = repClienteConfiguracaoComponentes.BuscarPorCliente(pessoa.CPF_CNPJ);
                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes pessoasConfiguracaoComponentesFreteExcluir in pessoasConfiguracaesComponentesFreteExcluir)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.ConfiguracaoDoComponente + pessoasConfiguracaoComponentesFreteExcluir.Descricao + Localization.Resources.Pessoas.Pessoa.RemovidoDaPessoa, unidadeDeTrabalho);
                    repClienteConfiguracaoComponentes.Deletar(pessoasConfiguracaoComponentesFreteExcluir);
                }

                for (int i = 0; i < configuracaoComponentes.Count; i++)
                {
                    dynamic dynConfiguracaoComponentes = configuracaoComponentes[i];

                    Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes pessoaConfiguracaoComponentesFrete = new Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes
                    {
                        ComponenteFrete = repComponenteFrete.BuscarPorCodigo((int)dynConfiguracaoComponentes.ComponenteFrete.Codigo),
                        Cliente = pessoa,
                        ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId((int)dynConfiguracaoComponentes.ModeloDocumentoFiscal.Codigo),
                        RateioFormula = repRateioFormula.BuscarPorCodigo((int)dynConfiguracaoComponentes.FormulaRateioFrete.Codigo),
                        OutraDescricaoCTe = (string)dynConfiguracaoComponentes.DescricaoCTe,
                        IncluirICMS = (bool)dynConfiguracaoComponentes.IncluirICMS,
                        IncluirIntegralmenteContratoFreteTerceiro = (bool)dynConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro,
                    };

                    repClienteConfiguracaoComponentes.Inserir(pessoaConfiguracaoComponentesFrete, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.ConfiguracaoDoComponente + pessoaConfiguracaoComponentesFrete.Descricao + Localization.Resources.Pessoas.Pessoa.AdicionadoPessoa, unidadeDeTrabalho);
                }
            }
        }

        private void AtualizarConfiguracoesEmissao(Dominio.Entidades.Cliente pessoa, dynamic configuracao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracao == null)
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao repConfiguracaoPessoaEmissao = new Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao configuracaoPessoaEmissao = pessoa.ConfiguracaoEmissao ?? new Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao();

            if (configuracaoPessoaEmissao.Codigo > 0)
                configuracaoPessoaEmissao.Initialize();

            configuracaoPessoaEmissao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = ((string)configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao).ToBool();

            if (configuracaoPessoaEmissao.Codigo == 0)
                repConfiguracaoPessoaEmissao.Inserir(configuracaoPessoaEmissao);
            else
                repConfiguracaoPessoaEmissao.Atualizar(configuracaoPessoaEmissao, Auditado, historico);

            pessoa.ConfiguracaoEmissao = configuracaoPessoaEmissao;

            repCliente.Atualizar(pessoa);
        }

        private void SalvarLayoutsEDI(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                return;

            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ClienteLayoutEDI repClienteLayoutEDI = new Repositorio.Embarcador.Pessoas.ClienteLayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

            dynamic layoutsEDI = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoLayoutEDI"));
            List<int> codigosExistentes = new List<int>();
            int codigo = 0;

            for (int i = 0; i < layoutsEDI.Count; i++)
                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    codigosExistentes.Add(codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsExistentes = repClienteLayoutEDI.BuscarPorPessoa(cliente.CPF_CNPJ);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsDeletar = (from obj in layoutsExistentes where !codigosExistentes.Contains(obj.Codigo) select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI layoutDeletar in layoutsDeletar)
            {
                layoutsExistentes.Remove(layoutDeletar);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.RemoveuLayout + layoutDeletar.Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeDeTrabalho);
                repClienteLayoutEDI.Deletar(layoutDeletar);
            }

            for (int i = 0; i < layoutsEDI.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI clienteLayoutEDI = null;

                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    clienteLayoutEDI = (from obj in layoutsExistentes where obj.Codigo == codigo select obj).FirstOrDefault();

                if (clienteLayoutEDI == null)
                    clienteLayoutEDI = new Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI();
                else
                    clienteLayoutEDI.Initialize();

                clienteLayoutEDI.Cliente = cliente;
                clienteLayoutEDI.LayoutEDI = repLayoutEDI.Buscar((int)layoutsEDI[i].CodigoLayoutEDI);
                clienteLayoutEDI.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao)(int)layoutsEDI[i].TipoIntegracao);

                clienteLayoutEDI.EnderecoFTP = null;
                clienteLayoutEDI.Diretorio = null;
                clienteLayoutEDI.Passivo = false;
                clienteLayoutEDI.UtilizarSFTP = false;
                clienteLayoutEDI.SSL = false;
                clienteLayoutEDI.UtilizarLeituraArquivos = false;
                clienteLayoutEDI.AdicionarEDIFilaProcessamento = false;
                clienteLayoutEDI.CriarComNomeTemporaraio = false;
                clienteLayoutEDI.Porta = null;
                clienteLayoutEDI.Senha = null;
                clienteLayoutEDI.Usuario = null;
                clienteLayoutEDI.Emails = null;
                clienteLayoutEDI.EmailsAlertaLeituraEDI = null;

                if (clienteLayoutEDI.TipoIntegracao != null && clienteLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                {
                    clienteLayoutEDI.EnderecoFTP = (string)layoutsEDI[i].EnderecoFTP;
                    clienteLayoutEDI.Diretorio = (string)layoutsEDI[i].Diretorio;
                    clienteLayoutEDI.Passivo = (bool)layoutsEDI[i].Passivo;
                    clienteLayoutEDI.UtilizarSFTP = (bool)layoutsEDI[i].UtilizarSFTP;
                    clienteLayoutEDI.SSL = (bool)layoutsEDI[i].SSL;
                    clienteLayoutEDI.UtilizarLeituraArquivos = (bool)layoutsEDI[i].UtilizarLeituraArquivos;
                    clienteLayoutEDI.AdicionarEDIFilaProcessamento = (bool)layoutsEDI[i].AdicionarEDIFilaProcessamento;
                    clienteLayoutEDI.CriarComNomeTemporaraio = (bool)layoutsEDI[i].CriarComNomeTemporaraio;
                    clienteLayoutEDI.Porta = (string)layoutsEDI[i].Porta;
                    clienteLayoutEDI.Senha = (string)layoutsEDI[i].Senha;
                    clienteLayoutEDI.Usuario = (string)layoutsEDI[i].Usuario;
                    clienteLayoutEDI.EmailsAlertaLeituraEDI = (string)layoutsEDI[i].EmailsAlertaLeituraEDI;
                }
                else if (clienteLayoutEDI.TipoIntegracao != null && clienteLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                {
                    clienteLayoutEDI.Emails = (string)layoutsEDI[i].Emails;
                }

                if (clienteLayoutEDI.Codigo > 0)
                {
                    repClienteLayoutEDI.Atualizar(clienteLayoutEDI, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = clienteLayoutEDI.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouLayout + clienteLayoutEDI.Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeDeTrabalho);
                }
                else
                {
                    repClienteLayoutEDI.Inserir(clienteLayoutEDI, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouLayout + clienteLayoutEDI.Descricao + Localization.Resources.Pessoas.Pessoa.APessoa, unidadeDeTrabalho);
                }
            }
        }

        private void SalvarListaEmail(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ClienteOutroEmail repClienteOutroEmail = new Repositorio.Embarcador.Pessoas.ClienteOutroEmail(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> emailsCliente = repClienteOutroEmail.BuscarPorCNPJCPFCliente(cliente.CPF_CNPJ);
            for (int i = 0; i < emailsCliente.Count(); i++)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.RemoveuEmail + emailsCliente[i].Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeDeTrabalho);
                repClienteOutroEmail.Deletar(emailsCliente[i]);
            }

            dynamic listaEmail = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaEmail"));
            if (listaEmail != null)
            {
                foreach (dynamic emailPessoa in listaEmail)
                {
                    bool enviarEmail = true;
                    bool.TryParse((string)emailPessoa.EnviarEmail, out enviarEmail);

                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail email = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail
                    {
                        Email = (string)emailPessoa.Email,
                        EmailStatus = enviarEmail ? "A" : "I",
                        TipoEmail = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail)int.Parse((string)emailPessoa.TipoEmail),
                        Cliente = cliente
                    };
                    repClienteOutroEmail.Inserir(email, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouEmail + email.Descricao + Localization.Resources.Pessoas.Pessoa.APessoa, unidadeDeTrabalho);
                }
            }
        }

        private void SalvarListaEndereco(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            List<int> enderecosMantidos = new List<int>();

            List<JsonEndereco> listaEndereco = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonEndereco>>(Request.Params("ListaEndereco"));
            if (listaEndereco != null)
            {
                foreach (JsonEndereco enderecoPessoa in listaEndereco)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = repClienteOutroEndereco.BuscarPorCodigoEPessoa(enderecoPessoa.Codigo ?? 0, cliente.CPF_CNPJ);
                    if (endereco == null) endereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();
                    else endereco.Initialize();

                    endereco.Cliente = cliente;
                    endereco.Bairro = enderecoPessoa.Bairro;
                    endereco.CEP = Utilidades.String.OnlyNumbers(enderecoPessoa.CEP);
                    endereco.Complemento = enderecoPessoa.Complemento;
                    endereco.CodigoDocumento = enderecoPessoa.CodigoDocumento;
                    endereco.Endereco = enderecoPessoa.Endereco;
                    endereco.EnderecoDigitado = enderecoPessoa.EnderecoDigitado;
                    endereco.Latitude = enderecoPessoa.Latitude;
                    endereco.Longitude = enderecoPessoa.Longitude;
                    endereco.Numero = enderecoPessoa.Numero;
                    endereco.IE_RG = enderecoPessoa.IE;
                    endereco.Localidade = repLocalidade.BuscarPorCodigo(enderecoPessoa.CodigoLocalidade);
                    endereco.TipoEndereco = enderecoPessoa.TipoEndereco;
                    endereco.TipoLogradouro = enderecoPessoa.TipoLogradouro;
                    endereco.Area = enderecoPessoa.AreaSecundario;
                    endereco.TipoArea = enderecoPessoa.TipoAreaEnderecoSecundario;
                    endereco.RaioEmMetros = enderecoPessoa.RaioEmMetrosSecundario;
                    endereco.CodigoEmbarcador = enderecoPessoa.CodigoIntegracao;
                    endereco.Telefone = enderecoPessoa.Telefone.ObterSomenteNumeros();

                    if (endereco.GeoLocalizacaoStatus == GeoLocalizacaoStatus.NaoGerado)
                        if (Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(endereco?.Latitude) && Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(endereco?.Longitude))
                            endereco.GeoLocalizacaoStatus = GeoLocalizacaoStatus.Gerado;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, endereco.Codigo > 0 ? endereco.GetChanges() : null, (endereco.Codigo > 0 ? Localization.Resources.Pessoas.Pessoa.Atualizou : Localization.Resources.Pessoas.Pessoa.Adicionou) + Localization.Resources.Pessoas.Pessoa.OEndereco + endereco.Descricao + Localization.Resources.Pessoas.Pessoa.APessoa, unidadeDeTrabalho);

                    if (endereco.Codigo > 0)
                        repClienteOutroEndereco.Atualizar(endereco, Auditado);
                    else
                        repClienteOutroEndereco.Inserir(endereco, Auditado);

                    enderecosMantidos.Add(endereco.Codigo);
                }


                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> enderecosCliente = repClienteOutroEndereco.BuscarPorEnderecosNaoMantidosPorPessoa(enderecosMantidos, cliente.CPF_CNPJ);
                for (int i = 0; i < enderecosCliente.Count(); i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.RemoveuEndereco + enderecosCliente[i].Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeDeTrabalho);
                    repClienteOutroEndereco.Deletar(enderecosCliente[i]);
                }
            }
        }

        private void SalvarListaDocumento(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ClienteDocumentacao repClienteDocumentacao = new Repositorio.Embarcador.Pessoas.ClienteDocumentacao(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            List<int> enderecosMantidos = new List<int>();

            List<JsonDocumento> listaDocumento = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonDocumento>>(Request.Params("ListaDocumento"));
            if (listaDocumento != null)
            {
                foreach (JsonDocumento documento in listaDocumento)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao doc = repClienteDocumentacao.BuscarPorCodigoEPessoa(documento.Codigo ?? 0, cliente.CPF_CNPJ);
                    if (doc == null) doc = new Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao();
                    else doc.Initialize();

                    doc.Cliente = cliente;
                    doc.Descricao = documento.Descricao;
                    DateTime dataEmissao;
                    DateTime.TryParse(documento.DataEmissao, out dataEmissao);
                    doc.DataEmissao = dataEmissao;
                    DateTime dataVencimento;
                    DateTime.TryParse(documento.DataVencimento, out dataVencimento);
                    doc.DataVencimento = dataVencimento;

                    if (doc.Codigo > 0)
                        repClienteDocumentacao.Atualizar(doc, Auditado);
                    else
                        repClienteDocumentacao.Inserir(doc, Auditado);

                    enderecosMantidos.Add(doc.Codigo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, (doc.Codigo > 0 ? Localization.Resources.Pessoas.Pessoa.Atualizou : Localization.Resources.Pessoas.Pessoa.Adicionou) + Localization.Resources.Pessoas.Pessoa.ODocumento + doc.Descricao + Localization.Resources.Pessoas.Pessoa.APessoa, unidadeDeTrabalho);
                }


                List<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao> documentosCliente = repClienteDocumentacao.BuscarPorDocumentoNaoMantidosPorPessoa(enderecosMantidos, cliente.CPF_CNPJ);
                for (int i = 0; i < documentosCliente.Count(); i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.RemoveuEndereco + documentosCliente[i].Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeDeTrabalho);
                    repClienteDocumentacao.Deletar(documentosCliente[i]);
                }
            }
        }

        private dynamic SalvarConfiguracaoFatura(Dominio.Entidades.Cliente pessoa, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                return null;

            if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoFatura))
                return null;

            if (string.IsNullOrWhiteSpace(Request.Params("ConfiguracaoFatura")))
                return null;

            dynamic configuracaoFatura = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoFatura"));
            if (configuracaoFatura == null)
                return null;

            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            TipoPrazoFaturamento tipoPrazoFaturamento;
            FormaGeracaoTituloFatura formaGeracaoTituloFatura;
            TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvioDocumentacao;
            TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvioDocumentacaoPorta;
            FormaEnvioDocumentacao formaEnvioDocumentacao;
            FormaEnvioDocumentacao formaEnvioDocumentacaoPorta;

            pessoa.TipoPrazoFaturamento = null;
            if (configuracaoFatura.TipoPrazoFaturamento != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoPrazoFaturamento, out tipoPrazoFaturamento);
                pessoa.TipoPrazoFaturamento = tipoPrazoFaturamento;
            }

            pessoa.FormaGeracaoTituloFatura = null;
            if (configuracaoFatura.FormaGeracaoTituloFatura != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaGeracaoTituloFatura, out formaGeracaoTituloFatura);
                pessoa.FormaGeracaoTituloFatura = formaGeracaoTituloFatura;
            }

            pessoa.DiasDePrazoFatura = 0;
            if (configuracaoFatura.DiasDePrazoFatura != null)
            {
                int diasPrazo = 0;
                int.TryParse((string)configuracaoFatura.DiasDePrazoFatura, out diasPrazo);
                pessoa.DiasDePrazoFatura = diasPrazo;
            }

            pessoa.PermiteFinalDeSemana = false;
            if (configuracaoFatura.PermiteFinalSemana != null)
                pessoa.PermiteFinalDeSemana = (bool)configuracaoFatura.PermiteFinalSemana;

            pessoa.ExigeCanhotoFisico = false;
            if (configuracaoFatura.ExigeCanhotoFisico != null)
                pessoa.ExigeCanhotoFisico = (bool)configuracaoFatura.ExigeCanhotoFisico;

            pessoa.ArmazenaCanhotoFisicoCTe = false;
            if (configuracaoFatura.ArmazenaCanhotoFisicoCTe != null)
                pessoa.ArmazenaCanhotoFisicoCTe = (bool)configuracaoFatura.ArmazenaCanhotoFisicoCTe;

            pessoa.SomenteOcorrenciasFinalizadoras = false;
            if (configuracaoFatura.SomenteOcorrenciasFinalizadoras != null)
                pessoa.SomenteOcorrenciasFinalizadoras = (bool)configuracaoFatura.SomenteOcorrenciasFinalizadoras;

            pessoa.FaturarSomenteOcorrenciasFinalizadoras = false;
            if (configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras != null)
                pessoa.FaturarSomenteOcorrenciasFinalizadoras = (bool)configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras;

            pessoa.NaoGerarFaturaAteReceberCanhotos = false;
            if (configuracaoFatura.NaoGerarFaturaAteReceberCanhotos != null)
                pessoa.NaoGerarFaturaAteReceberCanhotos = (bool)configuracaoFatura.NaoGerarFaturaAteReceberCanhotos;

            pessoa.GerarFaturamentoMultiplaParcela = false;
            if (configuracaoFatura.GerarFaturamentoMultiplaParcela != null)
                pessoa.GerarFaturamentoMultiplaParcela = (bool)configuracaoFatura.GerarFaturamentoMultiplaParcela;

            pessoa.AvisoVencimetoHabilitarConfiguracaoPersonalizada = false;
            if (configuracaoFatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada != null)
                pessoa.AvisoVencimetoHabilitarConfiguracaoPersonalizada = (bool)configuracaoFatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada;

            pessoa.AvisoVencimetoNaoEnviarEmail = false;
            if (configuracaoFatura.AvisoVencimetoNaoEnviarEmail != null)
                pessoa.AvisoVencimetoNaoEnviarEmail = (bool)configuracaoFatura.AvisoVencimetoNaoEnviarEmail;

            pessoa.CobrancaNaoEnviarEmail = false;
            if (configuracaoFatura.CobrancaNaoEnviarEmail != null)
                pessoa.CobrancaNaoEnviarEmail = (bool)configuracaoFatura.CobrancaNaoEnviarEmail;

            pessoa.AvisoVencimetoQunatidadeDias = 0;
            if (configuracaoFatura.AvisoVencimetoQunatidadeDias != null && !string.IsNullOrWhiteSpace((string)configuracaoFatura.AvisoVencimetoQunatidadeDias))
                pessoa.AvisoVencimetoQunatidadeDias = (int)configuracaoFatura.AvisoVencimetoQunatidadeDias;

            pessoa.AvisoVencimetoEnviarDiariamente = false;
            if (configuracaoFatura.AvisoVencimetoEnviarDiariamente != null)
                pessoa.AvisoVencimetoEnviarDiariamente = (bool)configuracaoFatura.AvisoVencimetoEnviarDiariamente;

            pessoa.CobrancaHabilitarConfiguracaoPersonalizada = false;
            if (configuracaoFatura.CobrancaHabilitarConfiguracaoPersonalizada != null)
                pessoa.CobrancaHabilitarConfiguracaoPersonalizada = (bool)configuracaoFatura.CobrancaHabilitarConfiguracaoPersonalizada;

            pessoa.CobrancaQunatidadeDias = 0;
            if (configuracaoFatura.CobrancaQunatidadeDias != null && !string.IsNullOrWhiteSpace((string)configuracaoFatura.CobrancaQunatidadeDias))
                pessoa.CobrancaQunatidadeDias = (int)configuracaoFatura.CobrancaQunatidadeDias;

            if (pessoa.ExigeCanhotoFisico.Value == false)
                pessoa.NaoGerarFaturaAteReceberCanhotos = false;

            if (configuracaoFatura.TomadorFatura != null && (double)configuracaoFatura.TomadorFatura > 0)
                pessoa.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ((double)configuracaoFatura.TomadorFatura);
            else
                pessoa.ClienteTomadorFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.ObservacaoFatura))
                pessoa.ObservacaoFatura = (string)configuracaoFatura.ObservacaoFatura;
            else
                pessoa.ObservacaoFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailFatura))
                pessoa.EmailFatura = (string)configuracaoFatura.EmailFatura;
            else
                pessoa.EmailFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoEmailFatura))
                pessoa.AssuntoEmailFatura = (string)configuracaoFatura.AssuntoEmailFatura;
            else
                pessoa.AssuntoEmailFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailFatura))
                pessoa.CorpoEmailFatura = (string)configuracaoFatura.CorpoEmailFatura;
            else
                pessoa.CorpoEmailFatura = null;

            bool informarEmailEnvioDocumentacao = (bool)configuracaoFatura.InformarEmailEnvioDocumentacao;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailEnvioDocumentacao) && informarEmailEnvioDocumentacao)
                pessoa.EmailEnvioDocumentacao = (string)configuracaoFatura.EmailEnvioDocumentacao;
            else
                pessoa.EmailEnvioDocumentacao = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoDocumentacao))
                pessoa.AssuntoDocumentacao = (string)configuracaoFatura.AssuntoDocumentacao;
            else
                pessoa.AssuntoDocumentacao = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailDocumentacao))
                pessoa.CorpoEmailDocumentacao = (string)configuracaoFatura.CorpoEmailDocumentacao;
            else
                pessoa.CorpoEmailDocumentacao = null;

            pessoa.TipoAgrupamentoEnvioDocumentacao = null;
            if (configuracaoFatura.TipoAgrupamentoEnvioDocumentacao != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoEnvioDocumentacao, out tipoAgrupamentoEnvioDocumentacao);
                pessoa.TipoAgrupamentoEnvioDocumentacao = tipoAgrupamentoEnvioDocumentacao;
            }

            pessoa.FormaEnvioDocumentacao = null;
            if (configuracaoFatura.FormaEnvioDocumentacao != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaEnvioDocumentacao, out formaEnvioDocumentacao);
                pessoa.FormaEnvioDocumentacao = formaEnvioDocumentacao;
            }

            bool informarEmailEnvioDocumentacaoPorta = (bool)configuracaoFatura.InformarEmailEnvioDocumentacaoPorta;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailEnvioDocumentacaoPorta) && informarEmailEnvioDocumentacaoPorta)
                pessoa.EmailEnvioDocumentacaoPorta = (string)configuracaoFatura.EmailEnvioDocumentacaoPorta;
            else
                pessoa.EmailEnvioDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoDocumentacaoPorta))
                pessoa.AssuntoDocumentacaoPorta = (string)configuracaoFatura.AssuntoDocumentacaoPorta;
            else
                pessoa.AssuntoDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailDocumentacaoPorta))
                pessoa.CorpoEmailDocumentacaoPorta = (string)configuracaoFatura.CorpoEmailDocumentacaoPorta;
            else
                pessoa.CorpoEmailDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.QuantidadeParcelasFaturamento))
                pessoa.QuantidadeParcelasFaturamento = (string)configuracaoFatura.QuantidadeParcelasFaturamento;
            else
                pessoa.QuantidadeParcelasFaturamento = null;

            pessoa.TipoAgrupamentoEnvioDocumentacaoPorta = null;
            if (configuracaoFatura.TipoAgrupamentoEnvioDocumentacaoPorta != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoEnvioDocumentacaoPorta, out tipoAgrupamentoEnvioDocumentacaoPorta);
                pessoa.TipoAgrupamentoEnvioDocumentacaoPorta = tipoAgrupamentoEnvioDocumentacaoPorta;
            }

            pessoa.FormaEnvioDocumentacaoPorta = null;
            if (configuracaoFatura.FormaEnvioDocumentacaoPorta != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaEnvioDocumentacaoPorta, out formaEnvioDocumentacaoPorta);
                pessoa.FormaEnvioDocumentacaoPorta = formaEnvioDocumentacaoPorta;
            }

            if (configuracaoFatura.BoletoConfiguracao != null && !string.IsNullOrWhiteSpace((string)configuracaoFatura.BoletoConfiguracao))
            {
                int.TryParse((string)configuracaoFatura.BoletoConfiguracao, out int codigoBoletoConfiguracao);
                if (codigoBoletoConfiguracao > 0)
                    pessoa.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    pessoa.BoletoConfiguracao = null;
            }
            else
                pessoa.BoletoConfiguracao = null;
            pessoa.EnviarBoletoPorEmailAutomaticamente = (bool)configuracaoFatura.EnviarBoletoPorEmailAutomaticamente;
            pessoa.EnviarDocumentacaoFaturamentoCTe = (bool)configuracaoFatura.EnviarDocumentacaoFaturamentoCTe;
            pessoa.FormaPagamento = repTipoPagamentoRecebimento.BuscarPorCodigo((int)configuracaoFatura.FormaPagamento);
            pessoa.GerarTituloPorDocumentoFiscal = (bool)configuracaoFatura.GerarTituloPorDocumentoFiscal;
            pessoa.GerarTituloAutomaticamente = (bool)configuracaoFatura.GerarTituloAutomaticamente;
            pessoa.GerarFaturaAutomaticaCte = (bool)configuracaoFatura.GerarFaturaAutomaticaCte;
            pessoa.GerarFaturamentoAVista = (bool)configuracaoFatura.GerarFaturamentoAVista;

            pessoa.GerarBoletoAutomaticamente = (bool)configuracaoFatura.GerarBoletoAutomaticamente;
            pessoa.EnviarArquivosDescompactados = (bool)configuracaoFatura.EnviarArquivosDescompactados;
            pessoa.NaoEnviarEmailFaturaAutomaticamente = (bool)configuracaoFatura.NaoEnviarEmailFaturaAutomaticamente;
            pessoa.HabilitarPeriodoVencimentoEspecifico = ((string)configuracaoFatura.HabilitarPeriodoVencimentoEspecifico).ToBool();

            if (configuracaoFatura.TipoEnvioFatura != null && (int)configuracaoFatura.TipoEnvioFatura > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoEnvioFatura, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura tipoEnvioFatura);
                pessoa.TipoEnvioFatura = tipoEnvioFatura;
            }
            else
                pessoa.TipoEnvioFatura = null;

            if (configuracaoFatura.TipoAgrupamentoFatura != null && (int)configuracaoFatura.TipoAgrupamentoFatura > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoFatura, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura tipoAgrupamentoFatura);
                pessoa.TipoAgrupamentoFatura = tipoAgrupamentoFatura;
            }
            else
                pessoa.TipoAgrupamentoFatura = null;


            if (configuracaoFatura.FormaTitulo != null && (int)configuracaoFatura.FormaTitulo > 0)
            {
                Enum.TryParse((string)configuracaoFatura.FormaTitulo, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo);
                pessoa.FormaTitulo = formaTitulo;
            }
            else
                pessoa.FormaTitulo = null;

            SalvarConfiguracaoFaturaVencimentos(pessoa, configuracaoFatura.FaturaVencimentos, unidadeDeTrabalho);

            return configuracaoFatura;
        }

        private void SalvarConfiguracaoFaturaVencimentos(Dominio.Entidades.Cliente pessoa, dynamic dynConfiguracaoFaturaVencimentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dynConfiguracaoFaturaVencimentos == null)
                return;

            Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento repPessoaFaturaVencimento = new Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento> pessoaFaturaVencimentos = repPessoaFaturaVencimento.BuscarPorCliente(pessoa.CPF_CNPJ);

            if (pessoaFaturaVencimentos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic configuracaoFaturaVencimentos in dynConfiguracaoFaturaVencimentos)
                {
                    int codigo = ((string)configuracaoFaturaVencimentos.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento> deletar = (from obj in pessoaFaturaVencimentos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < deletar.Count; i++)
                    repPessoaFaturaVencimento.Deletar(deletar[i]);
            }

            foreach (dynamic configuracaoFaturaVencimentos in dynConfiguracaoFaturaVencimentos)
            {
                int codigo = ((string)configuracaoFaturaVencimentos.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento pessoaFaturaVencimento = codigo > 0 ? repPessoaFaturaVencimento.BuscarPorCodigo(codigo, false) : null;

                if (pessoaFaturaVencimento == null)
                {
                    pessoaFaturaVencimento = new Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento();
                    pessoaFaturaVencimento.Cliente = pessoa;
                }

                pessoaFaturaVencimento.DiaInicial = ((string)configuracaoFaturaVencimentos.DiaInicial).ToInt();
                pessoaFaturaVencimento.DiaFinal = ((string)configuracaoFaturaVencimentos.DiaFinal).ToInt();
                pessoaFaturaVencimento.DiaVencimento = ((string)configuracaoFaturaVencimentos.DiaVencimento).ToInt();

                if (pessoaFaturaVencimento.Codigo > 0)
                    repPessoaFaturaVencimento.Atualizar(pessoaFaturaVencimento);
                else
                    repPessoaFaturaVencimento.Inserir(pessoaFaturaVencimento);
            }
        }

        private void SalvarConfiguracaoFaturaListasAuditarManual(Dominio.Entidades.Cliente pessoa, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                return;

            if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pessoa_PermiteAlterarConfiguracaoFatura))
                return;

            if (!string.IsNullOrWhiteSpace(Request.Params("ConfiguracaoFatura")) && Request.Params("ConfiguracaoFatura").Length > 4)
            {
                dynamic configuracaoFatura = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoFatura"));
                if (configuracaoFatura != null)
                {
                    string diasSemanaAnterior = "", diasMesAnterior = "";

                    if (pessoa.DiasSemanaFatura == null)
                        pessoa.DiasSemanaFatura = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();
                    else
                    {
                        diasSemanaAnterior = string.Join(", ", pessoa.DiasSemanaFatura);
                        pessoa.DiasSemanaFatura.Clear();
                    }

                    if (configuracaoFatura.DiasSemanaFatura != null)
                    {
                        foreach (dynamic dia in configuracaoFatura.DiasSemanaFatura)
                        {
                            Enum.TryParse((string)dia, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemanaFatura);
                            pessoa.DiasSemanaFatura.Add(diaSemanaFatura);
                        }
                    }

                    if (pessoa.DiasMesFatura == null)
                        pessoa.DiasMesFatura = new List<int>();
                    else
                    {
                        diasMesAnterior = string.Join(", ", pessoa.DiasMesFatura);
                        pessoa.DiasMesFatura.Clear();
                    }

                    if (configuracaoFatura.DiasMesFatura != null)
                    {
                        foreach (dynamic diaMesFatura in configuracaoFatura.DiasMesFatura)
                            pessoa.DiasMesFatura.Add((int)diaMesFatura);
                    }

                    if (pessoa.Codigo > 0)
                    {
                        string diasSemanaAtuais = string.Join(", ", pessoa.DiasSemanaFatura);
                        if (!diasSemanaAnterior.Equals(diasSemanaAtuais))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AlterouDiasDaSemanaDe + "'" + diasSemanaAnterior + "'" + Localization.Resources.Pessoas.Pessoa.Para + "'" + diasSemanaAtuais + "'.", unidadeDeTrabalho);

                        string diasMesAtuais = string.Join(", ", pessoa.DiasMesFatura);
                        if (!diasMesAnterior.Equals(diasMesAtuais))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AlterouDiasDoMesDe + "'" + diasMesAnterior + "'" + Localization.Resources.Pessoas.Pessoa.Para + "'" + diasMesAtuais + "'.", unidadeDeTrabalho);
                    }

                    if (pessoa.PermiteFinalDeSemana == false)
                    {
                        foreach (DiaSemana diaSemana in pessoa.DiasSemanaFatura)
                        {
                            if (diaSemana == DiaSemana.Sabado || diaSemana == DiaSemana.Domingo)
                                throw new ControllerException(Localization.Resources.Pessoas.Pessoa.FoiSelecionadoUmDiaDoFinalDeSemanaParaConfiguracaoDaFatura);
                        }
                    }
                }
            }
        }

        private void AtualizarConfiguracoesFatura(Dominio.Entidades.Cliente pessoa, dynamic configuracao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracao == null)
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura repConfiguracaoPessoaFatura = new Repositorio.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura configuracaoPessoaFatura = pessoa.ConfiguracaoFatura ?? new Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura();

            if (configuracaoPessoaFatura.Codigo > 0)
                configuracaoPessoaFatura.Initialize();

            configuracaoPessoaFatura.GerarTituloAutomaticamenteComAdiantamentoSaldo = ((string)configuracao.GerarTituloAutomaticamenteComAdiantamentoSaldo).ToBool();
            configuracaoPessoaFatura.PercentualAdiantamentoTituloAutomatico = ((string)configuracao.PercentualAdiantamentoTituloAutomatico).ToDecimal();
            configuracaoPessoaFatura.PrazoAdiantamentoEmDiasTituloAutomatico = ((string)configuracao.PrazoAdiantamentoEmDiasTituloAutomatico).ToInt();
            configuracaoPessoaFatura.PercentualSaldoTituloAutomatico = ((string)configuracao.PercentualSaldoTituloAutomatico).ToDecimal();
            configuracaoPessoaFatura.PrazoSaldoEmDiasTituloAutomatico = ((string)configuracao.PrazoSaldoEmDiasTituloAutomatico).ToInt();
            configuracaoPessoaFatura.EfetuarImpressaoDaTaxaDeMoedaEstrangeira = ((string)configuracao.EfetuarImpressaoDaTaxaDeMoedaEstrangeira).ToNullableBool();

            decimal somaPercentual = configuracaoPessoaFatura.PercentualAdiantamentoTituloAutomatico + configuracaoPessoaFatura.PercentualSaldoTituloAutomatico;
            if (configuracaoPessoaFatura.GerarTituloAutomaticamenteComAdiantamentoSaldo && somaPercentual != 100)
                throw new ControllerException("A soma dos percentuais de Adiantamento e Saldo devem somar 100% para geração de título automático!");

            if (configuracaoPessoaFatura.Codigo == 0)
                repConfiguracaoPessoaFatura.Inserir(configuracaoPessoaFatura);
            else
                repConfiguracaoPessoaFatura.Atualizar(configuracaoPessoaFatura, Auditado, historico);

            pessoa.ConfiguracaoFatura = configuracaoPessoaFatura;

            repCliente.Atualizar(pessoa);
        }

        private void ValidarRaizCNPJ(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            if (pessoa.GrupoPessoas != null)
            {
                List<string> listaRaizes = new List<string>();
                Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ repositorioRaizesCNPJ = new Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ> raizesCNPJ = repositorioRaizesCNPJ.BuscarPorGrupoPessoas(pessoa.GrupoPessoas.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ raizCNPJ in raizesCNPJ)
                {
                    listaRaizes.Add(raizCNPJ.RaizCNPJ);
                }

                string raizCNPJPessoa = (pessoa.CPF_CNPJ_SemFormato).Substring(0, 8);

                if (!listaRaizes.Contains(raizCNPJPessoa) && ConfiguracaoEmbarcador.ValidarRaizCNPJGrupoPessoa)
                {
                    throw new ControllerException(Localization.Resources.Pessoas.Pessoa.RaizDoCNPJDaPessoaNaoEstaNaListaDeRaizesDeCNPJDoGrupoDePessoas);
                }
            }
        }

        private void SalvarContasBancarias(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            if (!cliente.UtilizarCadastroContaBancaria)
                return;

            Repositorio.Embarcador.Financeiro.ContaBancaria repositorioContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);
            Repositorio.Cliente repUsuario = new Repositorio.Cliente(unitOfWork);

            dynamic dynDadosBancarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DadosBancarios"));
            dynamic contasBancarias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynDadosBancarios.ContasBancarias);

            if (cliente.ContasBancarias != null && cliente.ContasBancarias.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic contaBancaria in contasBancarias)
                    codigos.Add((int)contaBancaria.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria> tiposDeletar = cliente.ContasBancarias.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Financeiro.ContaBancaria contasBancariasDeletar in tiposDeletar)
                    cliente.ContasBancarias.Remove(contasBancariasDeletar);
            }
            else
                cliente.ContasBancarias = new List<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria>();

            if (contasBancarias != null && contasBancarias.Count > 0)
            {
                foreach (dynamic contaBancaria in contasBancarias)
                {
                    int.TryParse((string)contaBancaria.Codigo, out int codigoContaBancaria);
                    Dominio.Entidades.Embarcador.Financeiro.ContaBancaria existeContasBancarias = repositorioContaBancaria.BuscarPorCodigo(codigoContaBancaria, false);

                    if (existeContasBancarias == null)
                        continue;

                    bool existeContaBancaria = cliente.ContasBancarias.Any(o => o.Codigo == existeContasBancarias.Codigo);

                    if (!existeContaBancaria)
                        cliente.ContasBancarias.Add(existeContasBancarias);
                }
            }

            repUsuario.Atualizar(cliente);
        }
        private void SalvarUsuarioAcessoPortal(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
            Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

            bool tipoServicoPermiteCriarUsuarioDeAcesso = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            if (!pessoa.AtivarAcessoFornecedor || !tipoServicoPermiteCriarUsuarioDeAcesso)
            {
                Dominio.Entidades.Usuario usuarioInativar = repUsuario.BuscarPorClienteFornecedor(pessoa.CPF_CNPJ, TipoAcesso.Fornecedor);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    usuarioInativar = repUsuario.BuscarPorCPF(pessoa.CPF_CNPJ_SemFormato);

                if (usuarioInativar != null && usuarioInativar.Tipo != "M" && usuarioInativar.TipoAcesso == TipoAcesso.Fornecedor)
                {
                    usuarioInativar.Status = "I";
                    repUsuario.Atualizar(usuarioInativar);
                }

                return;
            }

            dynamic dynAcessoPortal = JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AcessoPortal"));
            string login = dynAcessoPortal?.Usuario;
            string vendedorPortalMulticlifor = dynAcessoPortal?.Vendedor;
            string senha = dynAcessoPortal?.Senha;
            string confirmaSenha = dynAcessoPortal?.ConfirmaSenha;
            int codigoUsuario = (dynAcessoPortal?.CodigoUsuario != null ? dynAcessoPortal.CodigoUsuario : 0);
            bool inserir = false;

            SalvarCampoVendedorPortalMultiClifor(login, vendedorPortalMulticlifor, unitOfWork);

            if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(login))
                return;

            Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor);

            //Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorClienteFornecedor(pessoa.CPF_CNPJ, TipoAcesso.Fornecedor);
            //if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            Dominio.Entidades.Usuario usuario = null;
            if (pessoa.Tipo == "E" && codigoUsuario > 0)
                usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
            else
                usuario = repUsuario.BuscarPorCPF(pessoa.CPF_CNPJ_SemFormato);

            if (usuario == null)
            {
                inserir = true;
                usuario = new Dominio.Entidades.Usuario
                {
                    CPF = pessoa.CPF_CNPJ_SemFormato,
                    ClienteFornecedor = pessoa,
                    Tipo = "U",
                    DataNascimento = DateTime.Today,
                    DataAdmissao = DateTime.Today,
                    Salario = 0,
                };
            }
            else
                usuario.Initialize();

            usuario.UsuarioAdministrador = true;
            usuario.Cliente = pessoa;
            usuario.ClienteFornecedor = pessoa;
            usuario.Nome = pessoa.Nome;
            usuario.Telefone = pessoa.Telefone1;
            usuario.Localidade = pessoa.Localidade;
            usuario.Endereco = pessoa.Endereco;
            usuario.Complemento = pessoa.Complemento;
            usuario.Email = pessoa.Email.Split(';').FirstOrDefault();
            usuario.Login = login;
            usuario.TipoPessoa = pessoa.Tipo;
            usuario.Status = "A";
            usuario.TipoAcesso = TipoAcesso.Fornecedor;
            usuario.Empresa = Empresa;
            usuario.Setor = usuario.Setor ?? new Dominio.Entidades.Setor() { Codigo = 1 };

            if (!string.IsNullOrWhiteSpace(senha) && senha != confirmaSenha)
                throw new ControllerException(Localization.Resources.Pessoas.Pessoa.SenhaConfirmacaoDaSenhaInformadosParaFornecedorPrecisamSerIguais);

            if (politicaSenha?.HabilitarPoliticaSenha ?? false)
            {
                if (!string.IsNullOrWhiteSpace(senha) && usuario.Senha != senha)
                {
                    usuario.Senha = senha;
                    usuario.AlterarSenhaAcesso = politicaSenha.ExigirTrocaSenhaPrimeiroAcesso;

                    if (!politicaSenha.ExigirTrocaSenhaPrimeiroAcesso)
                    {
                        string retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);
                        if (!string.IsNullOrWhiteSpace(retornoPoliticaSenha))
                        {
                            unitOfWork.Rollback();
                            throw new ControllerException(retornoPoliticaSenha);
                        }

                        usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                    }
                }
            }
            else
            {
                usuario.Senha = senha;
                usuario.SenhaCriptografada = false;
            }

            if (inserir)
                repUsuario.Inserir(usuario, Auditado);
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                repUsuario.Atualizar(usuario, Auditado);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                this.SalvarPermissoesPorServico(usuario, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor);

        }

        private void SalvarUsuarioTerceiro(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiTransporteTerceiro)
        {
            if (ConfiguracaoEmbarcador.NaoUtilizarUsuarioTransportadorTerceiro)
                return;

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (possuiTransporteTerceiro)
            {
                dynamic dynUsuarioTerceiro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("UsuarioTerceiro"));
                string login = dynUsuarioTerceiro.Usuario;
                string senha = dynUsuarioTerceiro.Senha;
                //string confirmaSenha = dynUsuarioTerceiro.ConfirmaSenha;
                bool inserir = false;
                Random numAleatorio = new Random();
                int valorUK = numAleatorio.Next(1, 1000000);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCliente(pessoa.CPF_CNPJ, Dominio.Enumeradores.TipoAcesso.Terceiro);

                if (usuario == null && repUsuario.ValidarControleDupliciadade(valorUK, pessoa.CPF_CNPJ))
                {
                    inserir = true;
                    usuario = new Dominio.Entidades.Usuario();
                    usuario.CPF = pessoa.CPF_CNPJ_SemFormato;
                    usuario.ClienteTerceiro = pessoa;
                    usuario.Tipo = "U";
                    usuario.DataNascimento = DateTime.Today;
                    usuario.DataAdmissao = DateTime.Today;
                    usuario.Salario = 0;
                    usuario.ControleDuplicidadeUK = valorUK;
                }
                else
                    usuario.Initialize();

                usuario.Nome = pessoa.Nome;
                usuario.Telefone = pessoa.Telefone1;
                usuario.Localidade = pessoa.Localidade;
                usuario.Endereco = pessoa.Endereco;
                usuario.Complemento = pessoa.Complemento;
                usuario.Email = pessoa.Email;

                if (!string.IsNullOrEmpty(senha))
                    usuario.Senha = senha;

                usuario.Login = login;
                usuario.UsuarioAdministrador = true;
                usuario.Status = "A";
                usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Terceiro;
                usuario.Empresa = this.Empresa;

                if (usuario.Setor == null)
                    usuario.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };

                if (inserir)
                    repUsuario.Inserir(usuario, Auditado);
                else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    repUsuario.Atualizar(usuario, Auditado);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    this.SalvarPermissoesPorServico(usuario, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros);

                if (inserir)
                {
                    Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = new Dominio.Entidades.Embarcador.Operacional.OperadorLogistica();
                    operadorLogistica.Ativo = true;
                    operadorLogistica.SupervisorLogistica = true;
                    operadorLogistica.PermiteAdicionarComplementosDeFrete = true;
                    operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = false;
                    operadorLogistica.Usuario = usuario;
                    repOperadorLogistica.Inserir(operadorLogistica);
                }
                //this.VincularHierarquiaUsuario(usuario, unitOfWork);
            }
        }

        private void SalvarFuncionario(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (pessoa.Funcionario)
            {
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(pessoa.CPF_CNPJ_SemFormato);

                if (usuario == null)
                {
                    usuario = new Dominio.Entidades.Usuario();
                    usuario.CPF = pessoa.CPF_CNPJ_SemFormato;
                    usuario.ClienteTerceiro = pessoa;
                    usuario.Tipo = pessoa.Motorista ? "M" : "U";
                    usuario.DataNascimento = pessoa.DataNascimento;
                    usuario.DataAdmissao = DateTime.Today;
                    usuario.Salario = 0;
                    usuario.Nome = pessoa.Nome;
                    usuario.Telefone = pessoa.Telefone1;
                    usuario.Localidade = pessoa.Localidade;
                    usuario.Endereco = pessoa.Endereco;
                    usuario.Complemento = pessoa.Complemento;
                    usuario.Email = pessoa.Email;
                    usuario.UsuarioAdministrador = false;
                    usuario.Status = "A";
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                    usuario.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
                    usuario.Empresa = this.Empresa;
                    usuario.RG = pessoa.RG_Passaporte;
                    usuario.Senha = usuario.CPF;

                    if (pessoa.Motorista)
                    {
                        usuario.Bloqueado = false;
                        usuario.NaoBloquearAcessoSimultaneo = false;
                        usuario.PendenteIntegracaoEmbarcador = true;
                        usuario.TipoMotorista = TipoMotorista.Proprio;
                        usuario.NaoGeraComissaoAcerto = false;
                        usuario.AtivarFichaMotorista = false;
                        usuario.Categoria = "";
                        usuario.OrgaoEmissorRG = Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.SSP;
                        //usuario.SituacaoColaborador = SituacaoColaborador.Trabalhando;
                        usuario.EstadoCivil = EstadoCivil.Outros;
                        usuario.CorRaca = CorRaca.SemInformacao;
                        usuario.Escolaridade = Escolaridade.SemInstrucaoFormal;
                    }

                    repUsuario.Inserir(usuario, Auditado);
                }
            }
        }

        private void SalvarSuprimentoDeGas(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas repositorioFilialSuprimentoDeGas = new Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas(unitOfWork);
            Repositorio.Embarcador.Filiais.SuprimentoDeGas repositorioSuprimentoDeGas = new Repositorio.Embarcador.Filiais.SuprimentoDeGas(unitOfWork);

            dynamic dynSuprimentosDeGas = JsonConvert.DeserializeObject<dynamic>(Request.Params("SuprimentoGas"));

            List<int> codigosSuprimentosExistentes = repositorioSuprimentoDeGas.BuscarCodigosPorCliente(pessoa.CPF_CNPJ);
            List<int> codigosAdicionadosAtualizados = new List<int>();

            foreach (dynamic dynSuprimentoDeGas in dynSuprimentosDeGas)
            {
                Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas suprimentoDeGas = new Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas();

                int codigo = ((string)dynSuprimentoDeGas.Codigo).ToInt();

                if (codigo > 0)
                    suprimentoDeGas = repositorioSuprimentoDeGas.BuscarPorCodigo(codigo, false);

                if (suprimentoDeGas == null)
                    suprimentoDeGas = new Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas();

                decimal capacidade = ((string)dynSuprimentoDeGas.Capacidade).ToDecimal();
                decimal lastro = ((string)dynSuprimentoDeGas.Lastro).ToDecimal();
                decimal estoqueMinimo = ((string)dynSuprimentoDeGas.EstoqueMinimo).ToDecimal();
                decimal estoqueMaximo = ((string)dynSuprimentoDeGas.EstoqueMaximo).ToDecimal();

                TimeSpan? horarioLimiteSolicitacao = ((string)dynSuprimentoDeGas.HoraLimiteSolicitacao).ToNullableTime();
                TimeSpan? horarioLimiteGerente = ((string)dynSuprimentoDeGas.HoraLimiteGerente).ToNullableTime();
                TimeSpan? horaBloqueioSolicitacao = ((string)dynSuprimentoDeGas.HoraBloqueioSolicitacao).ToNullableTime();

                double supridorPadrao = ((string)dynSuprimentoDeGas.SupridorPadrao).ToDouble();
                int produtoPadrao = ((string)dynSuprimentoDeGas.ProdutoPadrao).ToInt();
                int modeloVeicularPadrao = ((string)dynSuprimentoDeGas.ModeloVeicularPadrao).ToInt();
                int tipoCargaPadrao = ((string)dynSuprimentoDeGas.TipoCargaPadrao).ToInt();
                int tipoOperacaoPadrao = ((string)dynSuprimentoDeGas.TipoOperacaoPadrao).ToInt();

                string notificarPorEmailLimite = ((string)dynSuprimentoDeGas.NotificarPorEmailLimite).ToString();
                string notificarPorEmailGerente = ((string)dynSuprimentoDeGas.NotificarPorEmailGerente).ToString();
                string notificarPorEmailBloqueio = ((string)dynSuprimentoDeGas.NotificarPorEmailBloqueio).ToString();

                if (supridorPadrao > 0)
                {
                    Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                    suprimentoDeGas.SupridorPadrao = repositorioCliente.BuscarPorCPFCNPJ(supridorPadrao);
                }

                if (produtoPadrao > 0)
                {
                    Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                    suprimentoDeGas.ProdutoPadrao = repositorioProduto.BuscarPorCodigo(produtoPadrao);
                }

                if (modeloVeicularPadrao > 0)
                {
                    Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                    suprimentoDeGas.ModeloVeicularPadrao = repositorioModeloVeicularCarga.BuscarPorCodigo(modeloVeicularPadrao);
                }

                if (tipoCargaPadrao > 0)
                {
                    Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                    suprimentoDeGas.TipoCargaPadrao = repositorioTipoCarga.BuscarPorCodigo(tipoCargaPadrao);
                }

                if (tipoOperacaoPadrao > 0)
                {
                    Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                    suprimentoDeGas.TipoOperacaoPadrao = repositorioTipoOperacao.BuscarPorCodigo(tipoOperacaoPadrao);
                }

                suprimentoDeGas.Capacidade = capacidade;
                suprimentoDeGas.Lastro = lastro;
                suprimentoDeGas.EstoqueMinimo = estoqueMinimo;
                suprimentoDeGas.EstoqueMaximo = estoqueMaximo;

                suprimentoDeGas.HoraBloqueioSolicitacao = horaBloqueioSolicitacao;
                suprimentoDeGas.HoraLimiteGerente = horarioLimiteGerente;
                suprimentoDeGas.HoraLimiteSolicitacao = horarioLimiteSolicitacao;

                suprimentoDeGas.NotificarPorEmailBloqueio = notificarPorEmailBloqueio;
                suprimentoDeGas.NotificarPorEmailGerente = notificarPorEmailGerente;
                suprimentoDeGas.NotificarPorEmailLimite = notificarPorEmailLimite;

                if (suprimentoDeGas.Codigo > 0)
                    repositorioSuprimentoDeGas.Atualizar(suprimentoDeGas);
                else
                {
                    repositorioSuprimentoDeGas.Inserir(suprimentoDeGas);
                    Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas filialSuprimentoGas = new Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas()
                    {
                        Cliente = pessoa,
                        SuprimentoDeGas = suprimentoDeGas
                    };

                    repositorioFilialSuprimentoDeGas.Inserir(filialSuprimentoGas);
                }

                codigosAdicionadosAtualizados.Add(suprimentoDeGas.Codigo);
            }

            List<int> codigosSuprimentosDeletar = codigosSuprimentosExistentes.Where(obj => !codigosAdicionadosAtualizados.Any(o => obj == o)).ToList();

            if (pessoa.Codigo > 0)
                repositorioFilialSuprimentoDeGas.DeletarRegistrosExcluidos(codigosSuprimentosDeletar);
        }

        //private void VincularHierarquiaUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        //{
        //    Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquia = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unidadeTrabalho);
        //    List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquia = repHierarquia.BuscarPorRecebedor(usuario.Codigo);
        //    if (hierarquia == null || hierarquia.Count == 0)
        //    {
        //        Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaNova = new Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito();
        //        hierarquiaNova.Creditor = this.Usuario;
        //        hierarquiaNova.Solicitante = usuario;
        //        repHierarquia.Inserir(hierarquiaNova);
        //    }
        //}

        private void SalvarPermissoesPorServico(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unidadeTrabalho);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);

            List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = repFormulario.BuscarPorTipoServico(tipoServico);

            foreach (AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario in formularios)
            {
                if (formulario.TiposServicosMultisoftware.Contains(tipoServico) &&
                    repFuncionarioFormulario.ContarPorUsuarioEFormulario(usuario.Codigo, formulario.CodigoFormulario) <= 0)
                {
                    Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario
                    {
                        Usuario = usuario,
                        SomenteLeitura = false,
                        CodigoFormulario = formulario.CodigoFormulario
                    };

                    repFuncionarioFormulario.Inserir(funcionarioFormulario);
                }
            }

            unitOfWorkAdmin.Dispose();
        }

        private string EnviarEmailUsuarioTerceiro(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            string assunto = "MultiTMS - " + Localization.Resources.Pessoas.Pessoa.UsuarioTerceiroParaAcesso + usuario.Nome;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>" + Localization.Resources.Pessoas.Pessoa.PrezadoCliente).Append(usuario.Nome).Append("</p><br />");
            sb.Append("<p>" + Localization.Resources.Pessoas.Pessoa.DisponibilizadoUsuarioParaAcessoAoAmbienteDeTransportadoresDo).Append(usuario.Empresa.RazaoSocial).Append("</p>");
            sb.Append("<p>" + Localization.Resources.Pessoas.Pessoa.SeusDadosParaAcessoAoSistemaMultiCTeSao).Append("</p>");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso clienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);

                var _ClienteURLAcesso = clienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso retClienteURLAcesso = null;

                if (_ClienteURLAcesso.URLHomologacao)
                    retClienteURLAcesso = clienteURLAcesso.BuscarPorClienteETipo(Cliente.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro);
                else
                    retClienteURLAcesso = clienteURLAcesso.BuscarPorClienteETipoProducao(Cliente.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro);

                if (retClienteURLAcesso != null)
                    sb.Append($"<p>Link: <a href='http://{retClienteURLAcesso.URLAcesso}/'>http://{retClienteURLAcesso.URLAcesso}</a>").Append("<br />");

                unitOfWorkAdmin.Dispose();
            }
            else
            {
                sb.Append("<p>Link: http://gpaterceiros.multicte.com.br/").Append("<br />");
            }

            sb.Append(Localization.Resources.Gerais.Geral.Usuario + ": ").Append(usuario.Login).Append("<br />");
            sb.Append(Localization.Resources.Gerais.Geral.Senha + ": ").Append(usuario.Senha).Append("</p><br />");

            sb.Append("<p>" + Localization.Resources.Pessoas.Pessoa.EmailEnviadoAutomaticamenteFavorNaoResponder + "</p>");

            string mensagemErro = "";
            Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, usuario.Email, null, null, assunto, sb.ToString(), email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);
            return mensagemErro;
        }

        private void SalvarContatos(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(unidadeTrabalho);
            Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> contatosPessoa = repPessoaContato.BuscarPorPessoa(pessoa.CPF_CNPJ);

            dynamic contatos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaContatos"));

            if (contatosPessoa.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic contato in contatos)
                    if (int.TryParse((string)contato.Codigo, out int codigoContato))
                        codigos.Add(codigoContato);

                List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> contatosDeletar = (from obj in contatosPessoa where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < contatosDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuContato + contatosDeletar[i].Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeTrabalho);
                    repPessoaContato.Deletar(contatosDeletar[i]);
                }
            }

            foreach (dynamic contato in contatos)
            {
                Dominio.Entidades.Embarcador.Contatos.PessoaContato contatoGrupoPessoas = null;

                if (int.TryParse((string)contato.Codigo, out int codigoContato))
                    contatoGrupoPessoas = repPessoaContato.BuscarPorCodigo(codigoContato, false);

                if (contatoGrupoPessoas == null)
                    contatoGrupoPessoas = new Dominio.Entidades.Embarcador.Contatos.PessoaContato();
                else
                    contatoGrupoPessoas.Initialize();

                contatoGrupoPessoas.Pessoa = pessoa;
                contatoGrupoPessoas.Ativo = (bool)contato.Situacao;
                contatoGrupoPessoas.Contato = (string)contato.Contato;
                contatoGrupoPessoas.Email = (string)contato.Email;
                contatoGrupoPessoas.Telefone = (string)contato.Telefone;
                contatoGrupoPessoas.CPF = Utilidades.String.OnlyNumbers((string)contato.CPF);
                contatoGrupoPessoas.Cargo = (string)contato.Cargo;

                if (contatoGrupoPessoas.TiposContato == null)
                    contatoGrupoPessoas.TiposContato = new List<Dominio.Entidades.Embarcador.Contatos.TipoContato>();
                else
                    contatoGrupoPessoas.TiposContato.Clear();

                if (contato.TipoContato != null)
                {
                    foreach (dynamic codigoTipoContato in contato.TipoContato)
                        contatoGrupoPessoas.TiposContato.Add(repTipoContato.BuscarPorCodigo((int)codigoTipoContato));
                }

                if (contatoGrupoPessoas.Codigo > 0)
                {
                    repPessoaContato.Atualizar(contatoGrupoPessoas);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = contatoGrupoPessoas.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouContato + contatoGrupoPessoas.Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeTrabalho);
                }
                else
                {
                    repPessoaContato.Inserir(contatoGrupoPessoas);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouUmContato + contatoGrupoPessoas.Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeTrabalho);
                }
            }
        }

        private void SalvarOutrasDescricoesPessoaExterior(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao repPessoaExteriorOutraDescricao = new Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao> outrasDescricoesPessoa = repPessoaExteriorOutraDescricao.BuscarPorPessoa(pessoa.CPF_CNPJ);

            dynamic outrasDescricoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaOutrasDescricoesPessoaExterior"));

            if (outrasDescricoesPessoa.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic outraDescricao in outrasDescricoes)
                    if (int.TryParse((string)outraDescricao.Codigo, out int codigoOutraDescricao))
                        codigos.Add(codigoOutraDescricao);

                List<Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao> outrasDescricoesDeletar = (from obj in outrasDescricoesPessoa where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < outrasDescricoesDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuOutraDescricaoDoExterior + outrasDescricoesPessoa[i].Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeTrabalho);

                    repPessoaExteriorOutraDescricao.Deletar(outrasDescricoesPessoa[i]);
                }
            }

            foreach (dynamic outraDescricao in outrasDescricoes)
            {
                Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao pessoaExteriorOutraDescricao = null;

                if (int.TryParse((string)outraDescricao.Codigo, out int codigoOutraDescricao))
                    pessoaExteriorOutraDescricao = repPessoaExteriorOutraDescricao.BuscarPorCodigo(codigoOutraDescricao, false);

                if (pessoaExteriorOutraDescricao == null)
                    pessoaExteriorOutraDescricao = new Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao();
                else
                    pessoaExteriorOutraDescricao.Initialize();

                pessoaExteriorOutraDescricao.Pessoa = pessoa;
                pessoaExteriorOutraDescricao.Endereco = (string)outraDescricao.Endereco;
                pessoaExteriorOutraDescricao.RazaoSocial = (string)outraDescricao.RazaoSocial;

                if (pessoaExteriorOutraDescricao.Codigo > 0)
                {
                    repPessoaExteriorOutraDescricao.Atualizar(pessoaExteriorOutraDescricao);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = pessoaExteriorOutraDescricao.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouOutraDescricaoDoExterior + pessoaExteriorOutraDescricao.Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unidadeTrabalho);
                }
                else
                {
                    repPessoaExteriorOutraDescricao.Inserir(pessoaExteriorOutraDescricao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouOutraDescricaoDoExterior + pessoaExteriorOutraDescricao.Descricao + Localization.Resources.Pessoas.Pessoa.ParaPessoa, unidadeTrabalho);
                }
            }
        }

        private List<double> RetornaCodigosCNPJ(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<double> listaCNPJ = new List<double>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaCNPJConsultar")))
            {
                dynamic listaCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCNPJConsultar"));
                if (listaCliente != null)
                {
                    foreach (dynamic cnpj in listaCliente)
                    {
                        listaCNPJ.Add(double.Parse((string)cnpj.CodigoFornecedor));
                    }
                }
            }
            return listaCNPJ;
        }

        private void SalvarLicencas(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaLicenca repLicencaPessoa = new Repositorio.Embarcador.Pessoas.PessoaLicenca(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

            dynamic dynLicencas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaLicencas"));

            if (pessoa.Licencas?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic licenca in dynLicencas)
                    if (licenca.Codigo != null)
                        codigos.Add((int)licenca.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca> licencasDeletar = (from obj in pessoa.Licencas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < licencasDeletar.Count; i++)
                    repLicencaPessoa.Deletar(licencasDeletar[i]);
            }
            else
                pessoa.Licencas = new List<Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca>();

            foreach (dynamic dynLicenca in dynLicencas)
            {
                int codigoLicencaPessoa = ((string)dynLicenca.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca licenca = codigoLicencaPessoa > 0 ? repLicencaPessoa.BuscarPorCodigo(codigoLicencaPessoa, true) : null;
                if (licenca == null)
                    licenca = new Dominio.Entidades.Embarcador.Pessoas.PessoaLicenca();

                int.TryParse((string)dynLicenca.CodigoLicenca, out int codigoLicenca);
                DateTime.TryParse((string)dynLicenca.DataEmissao, out DateTime dataEmissao);
                DateTime.TryParse((string)dynLicenca.DataVencimento, out DateTime dataVencimento);
                Enum.TryParse((string)dynLicenca.Status, out StatusLicenca status);

                licenca.Licenca = codigoLicenca > 0 ? repLicenca.BuscarPorCodigo(codigoLicenca) : null;
                licenca.DataEmissao = dataEmissao;
                licenca.DataVencimento = dataVencimento;
                licenca.Descricao = (string)dynLicenca.Descricao;
                licenca.Numero = (string)dynLicenca.Numero;
                licenca.Pessoa = pessoa;
                licenca.Status = status;

                dynamic dynFormasAlerta = JsonConvert.DeserializeObject<dynamic>((string)dynLicenca.FormaAlerta);
                licenca.FormasAlerta = new List<ControleAlertaForma>();
                if (dynFormasAlerta?.Count > 0)
                {
                    foreach (dynamic codigoFormaAlerta in dynFormasAlerta)
                        licenca.FormasAlerta.Add(((string)codigoFormaAlerta).ToEnum<ControleAlertaForma>());
                }

                if (licenca.Codigo > 0)
                    repLicencaPessoa.Atualizar(licenca, Auditado);
                else
                    repLicencaPessoa.Inserir(licenca, Auditado);
            }
        }

        private void SalvarDadosArmador(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaArmador repPessoaArmador = new Repositorio.Embarcador.Pessoas.PessoaArmador(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador> ListaPessoaArmador = repPessoaArmador.BuscarPorPessoa(pessoa.CPF_CNPJ);

            dynamic dynDadosArmador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaDadosArmador"));

            if (ListaPessoaArmador?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic armador in dynDadosArmador)
                    if (armador.Codigo != null)
                        codigos.Add((int)armador.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.PessoaArmador> armadorDeletar = (from obj in ListaPessoaArmador where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < armadorDeletar.Count; i++)
                    repPessoaArmador.Deletar(armadorDeletar[i]);
            }

            foreach (dynamic dynArmador in dynDadosArmador)
            {
                int codigoArmadorPessoa = ((string)dynArmador.Codigo).ToInt();
                int codigoTipoContainer = ((string)dynArmador.CodigoTipoContainer).ToInt();

                Dominio.Entidades.Embarcador.Pessoas.PessoaArmador pessoaArmador = codigoArmadorPessoa > 0 ? repPessoaArmador.BuscarPorCodigo(codigoArmadorPessoa, false) : null;
                if (pessoaArmador == null)
                    pessoaArmador = new Dominio.Entidades.Embarcador.Pessoas.PessoaArmador();

                DateTime.TryParse((string)dynArmador.DataVigenciaInicial, out DateTime dataInicioVigencia);
                DateTime.TryParse((string)dynArmador.DataVigenciaFinal, out DateTime dataFimVigencia);

                DateTime? nullTime = null;
                pessoaArmador.ContainerTipo = codigoTipoContainer > 0 ? repContainerTipo.BuscarPorCodigo(codigoTipoContainer) : null;
                pessoaArmador.DiasFreetime = (int)dynArmador.DiasFreetime;
                pessoaArmador.ValorDariaAposFreetime = ((string)dynArmador.ValorDiariaAposFreetime).ToDecimal();
                pessoaArmador.DataVigenciaInicial = dataInicioVigencia != DateTime.MinValue ? dataInicioVigencia.Date : nullTime;
                pessoaArmador.DataVigenciaFinal = dataFimVigencia != DateTime.MinValue ? dataFimVigencia : nullTime;
                pessoaArmador.Pessoa = pessoa;

                if (pessoaArmador.Codigo > 0)
                    repPessoaArmador.Atualizar(pessoaArmador);
                else
                    repPessoaArmador.Inserir(pessoaArmador);
            }
        }

        private void SalvarVendedores(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaFuncionario repPessoaFuncionario = new Repositorio.Embarcador.Pessoas.PessoaFuncionario(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            repPessoaFuncionario.DeletarPorPessoaEmpresa(pessoa.CPF_CNPJ, codigoEmpresa);
            dynamic dynVendedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaVendedores"));
            if (dynVendedores.Count > 0)
            {
                foreach (dynamic dynVendedor in dynVendedores)
                {
                    Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario vendedor = new Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario();

                    int codigoFuncionario = int.Parse((string)dynVendedor.CodigoFuncionario);
                    int codigoTipoDeCarga = int.Parse((string)dynVendedor.CodigoTipoDeCarga);
                    decimal percentualComissao;
                    decimal.TryParse((string)dynVendedor.PercentualComissao, out percentualComissao);

                    DateTime.TryParse((string)dynVendedor.DataInicioVigencia, out DateTime dataInicioVigencia);
                    DateTime.TryParse((string)dynVendedor.DataFimVigencia, out DateTime dataFimVigencia);

                    vendedor.PercentualComissao = percentualComissao;
                    if (dataInicioVigencia > DateTime.MinValue)
                        vendedor.DataInicioVigencia = dataInicioVigencia;
                    if (dataFimVigencia > DateTime.MinValue)
                        vendedor.DataFimVigencia = dataFimVigencia;
                    if (codigoFuncionario > 0)
                        vendedor.Funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionario);
                    if (codigoTipoDeCarga > 0)
                        vendedor.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);
                    vendedor.Pessoa = pessoa;

                    repPessoaFuncionario.Inserir(vendedor);
                }
            }
        }

        private void SalvarRecebedoresAutorizados(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaRecebedorAutorizado repPessoaRecebedorAutorizado = new Repositorio.Embarcador.Pessoas.PessoaRecebedorAutorizado(unitOfWork);

            repPessoaRecebedorAutorizado.DeletarPorPessoaEmpresa(pessoa.CPF_CNPJ);
            dynamic dynRecebedoresAutorizados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaRecebedoresAutorizados"));
            if (dynRecebedoresAutorizados.Count > 0)
            {
                foreach (dynamic dynRecebedorAutorizado in dynRecebedoresAutorizados)
                {
                    Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado recebedorAutorizado = new Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado();

                    string nome = (string)dynRecebedorAutorizado.Nome;
                    string cpf = (string)dynRecebedorAutorizado.CPF;
                    string foto = (string)dynRecebedorAutorizado.Foto;
                    foto = foto.Split(',')[1];

                    // Salva a foto em disco
                    byte[] buffer = System.Convert.FromBase64String(foto);
                    MemoryStream ms = new MemoryStream(buffer);
                    string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Cliente", "FotoRecebedor" });
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ArmazenarArquivoFisico(ms, caminho, out string guidFoto);

                    recebedorAutorizado.Pessoa = pessoa;
                    recebedorAutorizado.Nome = nome;
                    recebedorAutorizado.CPF = cpf;
                    recebedorAutorizado.GuidFoto = guidFoto;

                    repPessoaRecebedorAutorizado.Inserir(recebedorAutorizado);
                }
            }
        }

        private void SalvarRotas(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            List<int> codigosRotas = Request.GetListParam<int>("Rota");
            List<Dominio.Entidades.RotaFrete> rotas = repRotaFrete.BuscarPorCodigos(codigosRotas);
            List<Dominio.Entidades.RotaFrete> rotasExclusao = repRotaFrete.BuscarPorDistribuidorParaExclusao(pessoa.CPF_CNPJ, codigosRotas);

            foreach (Dominio.Entidades.RotaFrete rota in rotas)
            {
                rota.Initialize();
                rota.Distribuidor = pessoa;
                repRotaFrete.Atualizar(rota, Auditado);
            }

            foreach (Dominio.Entidades.RotaFrete rota in rotasExclusao)
            {
                rota.Initialize();
                rota.Distribuidor = null;
                repRotaFrete.Atualizar(rota, Auditado);
            }
        }

        private void SalvarComponentes(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteComponente repClienteComponente = new Repositorio.Embarcador.Pessoas.ClienteComponente(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            repClienteComponente.DeletarPorCliente(pessoa.CPF_CNPJ);
            dynamic dynLista = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaComponentes"));
            if (dynLista.Count > 0)
            {
                foreach (dynamic item in dynLista)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteComponente componente = new Dominio.Entidades.Embarcador.Pessoas.ClienteComponente();

                    int.TryParse((string)item.CodigoComponente, out int codigoComponente);
                    int.TryParse((string)item.CodigoFilial, out int codigoFilial);
                    int.TryParse((string)item.CodigoTransportadora, out int codigoTransportadora);

                    decimal.TryParse((string)item.ValorComponente, out decimal valorComponente);

                    componente.Cliente = pessoa;
                    componente.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
                    componente.Empresa = codigoTransportadora > 0 ? repEmpresa.BuscarPorCodigo(codigoTransportadora) : null;
                    componente.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(codigoComponente);
                    componente.Valor = valorComponente;

                    repClienteComponente.Inserir(componente);
                }
            }
        }

        private void SalvarDadosClientePorEmpresa(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                return;

            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);
            Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(this.Usuario.Empresa.Codigo, pessoa.CPF_CNPJ);

            bool inserir = false;
            if (dadosCliente == null)
            {
                inserir = true;
                dadosCliente = new Dominio.Entidades.DadosCliente();
            }
            else
                dadosCliente.Initialize();

            dynamic dynDadosBancarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DadosBancarios"));
            if (dynDadosBancarios != null)
            {
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.ClientePortadorConta) && (string)dynDadosBancarios.ClientePortadorConta != "0")
                    pessoa.ClientePortadorConta = new Dominio.Entidades.Cliente() { CPF_CNPJ = double.Parse((string)dynDadosBancarios.ClientePortadorConta) };
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Banco) && (string)dynDadosBancarios.Banco != "0")
                    dadosCliente.Banco = new Dominio.Entidades.Banco() { Codigo = int.Parse((string)dynDadosBancarios.Banco) };
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Agencia))
                    dadosCliente.Agencia = (string)dynDadosBancarios.Agencia;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.Digito))
                    dadosCliente.DigitoAgencia = (string)dynDadosBancarios.Digito;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.NumeroConta))
                    dadosCliente.NumeroConta = (string)dynDadosBancarios.NumeroConta;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.TipoConta) && (string)dynDadosBancarios.TipoConta != "0")
                    dadosCliente.TipoConta = (Dominio.ObjetosDeValor.Enumerador.TipoConta)int.Parse((string)dynDadosBancarios.TipoConta);
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.CnpjIpef))
                    pessoa.CnpjIpef = (string)dynDadosBancarios.CnpjIpef;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.TipoChavePix))
                    pessoa.TipoChavePix = (Dominio.ObjetosDeValor.Enumerador.TipoChavePix)int.Parse((string)dynDadosBancarios.TipoChavePix);
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.ChavePix))
                    pessoa.ChavePix = (string)dynDadosBancarios.ChavePix;
                if (!string.IsNullOrWhiteSpace((string)dynDadosBancarios.CodigoIntegracaoDadosBancarios))
                    pessoa.CodigoIntegracaoDadosBancarios = (string)dynDadosBancarios.CodigoIntegracaoDadosBancarios;

                string email = Request.Params("EmailInterno");
                bool enviarEmail;
                bool.TryParse(Request.Params("EnviarEmailInterno"), out enviarEmail);

                dadosCliente.Email = email;
                dadosCliente.EmailStatus = enviarEmail ? "A" : "I";
            }

            dynamic dynFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Fornecedor"));
            if (dynFornecedor.Fornecedor != null)
            {
                int tipoMovimentoTituloPagar = 0;

                string sTipoMovimentoTituloPagar = (string)dynFornecedor.Fornecedor.TipoMovimentoTituloPagar;
                if (!string.IsNullOrEmpty(sTipoMovimentoTituloPagar))
                    tipoMovimentoTituloPagar = int.Parse(sTipoMovimentoTituloPagar);

                if (tipoMovimentoTituloPagar > 0)
                    dadosCliente.TipoMovimentoTituloPagar = repTipoMovimento.BuscarPorCodigo(tipoMovimentoTituloPagar);
                else
                    dadosCliente.TipoMovimentoTituloPagar = null;
            }

            if (inserir)
            {
                dadosCliente.Cliente = pessoa;
                dadosCliente.Empresa = this.Usuario.Empresa;
                repDadosCliente.Inserir(dadosCliente, Auditado);
            }
            else
                repDadosCliente.Atualizar(dadosCliente, Auditado);
        }

        private void PreencherDadosAdicionaisPessoa(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
            Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);

            dynamic dynDadosAdicionais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DadosAdicionais"));
            if (dynDadosAdicionais != null)
            {
                int.TryParse((string)dynDadosAdicionais.PedidoTipoPagamento, out int codigoPedidoTipoPagamento);
                double.TryParse((string)dynDadosAdicionais.ClientePai, out double clientePai);
                int.TryParse((string)dynDadosAdicionais.TipoOperacaoPadrao, out int codigoTipoOperacao);
                int.TryParse((string)dynDadosAdicionais.BancoDOC, out int codigoBancoDOC);
                int.TryParse((string)dynDadosAdicionais.MesoRegiao, out int codigoMesoRegiao);
                int.TryParse((string)dynDadosAdicionais.Regiao, out int codigoRegiao);
                int.TryParse((string)dynDadosAdicionais.TipoIntegracaoValePedagio, out int codigoTipoIntegracaValePedagio);
                string codigoEstadoRG = (string)dynDadosAdicionais.EstadoRG;
                double.TryParse((string)dynDadosAdicionais.ArmazemResponsavel, out double codigoArmazemResponsavel);
                int.TryParse((string)dynDadosAdicionais.CondicaoPagamentoPadrao, out int codigoCondicaoPagamentoPadrao);

                DateTime.TryParse((string)dynDadosAdicionais.DataNascimento, out DateTime dataNascimento);

                Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG orgaoEmissorRG;
                Enum.TryParse((string)dynDadosAdicionais.OrgaoEmissaoRG, out orgaoEmissorRG);

                pessoa.PISPASEP = (string)dynDadosAdicionais.PISPASEP;
                if (dataNascimento > DateTime.MinValue)
                    pessoa.DataNascimento = dataNascimento;
                else
                    pessoa.DataNascimento = null;
                if (orgaoEmissorRG > 0)
                    pessoa.OrgaoEmissorRG = orgaoEmissorRG;
                else
                    pessoa.OrgaoEmissorRG = null;

                pessoa.NomeSocio = (string)dynDadosAdicionais.NomeSocio;
                pessoa.LocalidadeNascimento = dynDadosAdicionais.LocalidadeNascimento != null ? repLocalidade.BuscarPorCodigo((int)dynDadosAdicionais.LocalidadeNascimento) : null;
                pessoa.CanalEntrega = dynDadosAdicionais.CanalEntrega != null ? repositorioCanalEntrega.BuscarPorCodigo((int)dynDadosAdicionais.CanalEntrega) : null;
                pessoa.CPFSocio = Utilidades.String.OnlyNumbers((string)dynDadosAdicionais.CPFSocio);
                if (!string.IsNullOrWhiteSpace(pessoa.CPFSocio) && !Utilidades.Validate.ValidarCPF(pessoa.CPFSocio))
                    throw new ControllerException("CPF do Sócio é inválido");
                pessoa.Profissao = (string)dynDadosAdicionais.Profissao;
                pessoa.TituloEleitoral = (string)dynDadosAdicionais.TituloEleitoral;
                pessoa.ZonaEleitoral = (string)dynDadosAdicionais.ZonaEleitoral;
                pessoa.SecaoEleitoral = (string)dynDadosAdicionais.SecaoEleitoral;
                pessoa.NumeroCEI = (string)dynDadosAdicionais.NumeroCEI;
                pessoa.GerarPedidoColeta = (bool)dynDadosAdicionais.GerarPedidoColeta;
                pessoa.GerarPedidoBloqueado = (bool)dynDadosAdicionais.GerarPedidoBloqueado;
                pessoa.InstituicaoGovernamental = (bool)dynDadosAdicionais.InstituicaoGovernamental;
                pessoa.EnviarDocumentacaoCTeAverbacaoSegundaInstancia = (bool)dynDadosAdicionais.EnviarDocumentacaoCTeAverbacaoSegundaInstancia;
                pessoa.ExigirNumeroControleCliente = (bool)dynDadosAdicionais.ExigirNumeroControleCliente;
                pessoa.ExigirNumeroNumeroReferenciaCliente = (bool)dynDadosAdicionais.ExigirNumeroNumeroReferenciaCliente;
                pessoa.EnviarAutomaticamenteDocumentacaoCarga = (bool)dynDadosAdicionais.EnviarAutomaticamenteDocumentacaoCarga;
                pessoa.ReplicarNumeroReferenciaTodasNotasCarga = (bool)dynDadosAdicionais.ReplicarNumeroReferenciaTodasNotasCarga;
                pessoa.DigitalizacaoCanhotoInteiro = (bool)dynDadosAdicionais.DigitalizacaoCanhotoInteiro;
                pessoa.ReplicarNumeroControleCliente = (bool)dynDadosAdicionais.ReplicarNumeroControleCliente;
                pessoa.CodigoPortuario = (string)dynDadosAdicionais.CodigoPortuario;
                pessoa.Empresa = repEmpresa.BuscarPorCodigo((int)dynDadosAdicionais.Transportador);
                pessoa.RateioFormulaExclusivo = repFormulaRateio.BuscarPorCodigo((int)dynDadosAdicionais.RateioFormulaExclusivo);
                pessoa.TipoEmissaoCTeDocumentosExclusivo = (TipoEmissaoCTeDocumentos)dynDadosAdicionais.TipoEmissaoCTeDocumentosExclusivo;
                pessoa.ObservacaoInterna = (string)dynDadosAdicionais.ObservacaoInterna;
                pessoa.TipoIntegradoraValePedagio = codigoTipoIntegracaValePedagio > 0 ? repTipoIntegracao.BuscarPorCodigo(codigoTipoIntegracaValePedagio) : null;
                pessoa.RKST = (string)dynDadosAdicionais.RKST?.Value?.Replace(" ", "");
                pessoa.MDGCode = (string)dynDadosAdicionais.MDGCode?.Value?.Replace(" ", "");
                pessoa.CMDID = (string)dynDadosAdicionais.CMDID?.Value?.Replace(" ", "");

                if (dynDadosAdicionais?.RecebedorColeta != null)
                {
                    int codigoRecebedorColeta = !string.IsNullOrEmpty((string)dynDadosAdicionais.RecebedorColeta) ? dynDadosAdicionais.RecebedorColeta : 0;
                    if (codigoRecebedorColeta > 0)
                    {
                        Dominio.Entidades.Cliente recebedorColeta = repCliente.BuscarPorCPFCNPJ(codigoRecebedorColeta);
                        pessoa.RecebedorColeta = recebedorColeta;
                    }
                }

                pessoa.ClientePai = clientePai > 0 ? repCliente.BuscarPorCPFCNPJ(clientePai) : null;
                pessoa.Celular = Utilidades.String.OnlyNumbers((string)dynDadosAdicionais.Celular);
                pessoa.CodigoCompanhia = (string)dynDadosAdicionais.CodigoCompanhia;
                pessoa.ContaFornecedorEBS = (string)dynDadosAdicionais.ContaFornecedorEBS;
                pessoa.CodigoDocumento = (string)dynDadosAdicionais.CodigoDocumento;
                pessoa.SenhaLiberacaoMobile = (string)dynDadosAdicionais.SenhaLiberacaoMobile;
                pessoa.SenhaConfirmacaoColetaEntrega = (string)dynDadosAdicionais.SenhaConfirmacaoColetaEntrega;
                pessoa.ValorMinimoCarga = ((string)dynDadosAdicionais.ValorMinimoCarga).ToDecimal();
                pessoa.ValorMinimoEntrega = ((string)dynDadosAdicionais.ValorMinimoEntrega).ToDecimal();

                pessoa.FronteiraAlfandega = ((string)dynDadosAdicionais.FronteiraAlfandega).ToBool();

                string tempoMedioPermanencia = ((string)dynDadosAdicionais.TempoMedioPermanenciaFronteira);
                if (!string.IsNullOrEmpty(tempoMedioPermanencia))
                {
                    string[] horaEMinuto = tempoMedioPermanencia.Split(':');
                    if (horaEMinuto.Count() == 2)
                    {
                        pessoa.TempoMedioPermanenciaFronteira = int.Parse(horaEMinuto[0]) * 60 + int.Parse(horaEMinuto[1]);
                    }
                }
                else
                {
                    pessoa.TempoMedioPermanenciaFronteira = 0;
                }
                pessoa.CodigoAduanaDestino = (string)dynDadosAdicionais.CodigoAduanaDestino;
                pessoa.ExigeQueEntregasSejamAgendadas = ((string)dynDadosAdicionais.ExigeQueEntregasSejamAgendadas).ToBool();
                pessoa.AreaRedex = ((string)dynDadosAdicionais.AreaRedex).ToBool();
                pessoa.Armador = ((string)dynDadosAdicionais.Armador).ToBool();
                pessoa.PermiteAgendarComViagemIniciada = ((string)dynDadosAdicionais.PermiteAgendarComViagemIniciada).ToBool();
                pessoa.PedidoTipoPagamento = codigoPedidoTipoPagamento > 0 ? repPedidoTipoPagamento.BuscarPorCodigo(codigoPedidoTipoPagamento) : null;
                pessoa.ArmazemResponsavel = codigoArmazemResponsavel > 0 ? repCliente.BuscarPorCPFCNPJ(codigoArmazemResponsavel) : null;
                pessoa.CondicaoPagamentoPadrao = codigoCondicaoPagamentoPadrao > 0 ? repCondicaoPagamento.BuscarPorCodigo(codigoCondicaoPagamentoPadrao) : null;
                pessoa.TipoOperacaoPadrao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
                pessoa.Sexo = ((string)dynDadosAdicionais.Sexo).ToNullableEnum<Dominio.ObjetosDeValor.Enumerador.Sexo>();
                pessoa.EstadoCivil = ((string)dynDadosAdicionais.EstadoCivil).ToNullableEnum<EstadoCivil>();
                pessoa.CotacaoEspecial = ((string)dynDadosAdicionais.CotacaoEspecial).ToDecimal();

                pessoa.CodigoAduaneiro = (string)dynDadosAdicionais.CodigoAduaneiro;
                pessoa.CodigoURFAduaneiro = (string)dynDadosAdicionais.CodigoURFAduaneiro;
                pessoa.CodigoRAAduaneiro = (string)dynDadosAdicionais.CodigoRAAduaneiro;

                pessoa.TipoFornecedor = (string)dynDadosAdicionais.TipoFornecedor;
                pessoa.CodigoCategoriaTrabalhador = (string)dynDadosAdicionais.CodigoCategoriaTrabalhador;
                pessoa.Funcao = (string)dynDadosAdicionais.Funcao;
                pessoa.PagamentoEmBanco = (string)dynDadosAdicionais.PagamentoEmBanco;
                pessoa.FormaPagamentoeSocial = (string)dynDadosAdicionais.FormaPagamentoeSocial;
                pessoa.TipoAutonomo = (string)dynDadosAdicionais.TipoAutonomo;
                pessoa.CodigoReceita = (string)dynDadosAdicionais.CodigoReceita;
                pessoa.TipoPagamentoBancario = (string)dynDadosAdicionais.TipoPagamentoBancario;
                pessoa.NaoDescontaIRRF = (string)dynDadosAdicionais.NaoDescontaIRRF;
                pessoa.CodigoAlternativo = (string)dynDadosAdicionais.CodigoAlternativo;
                pessoa.BancoDOC = codigoBancoDOC > 0 ? repBanco.BuscarPorCodigo(codigoBancoDOC) : null;
                pessoa.EstadoRG = !string.IsNullOrWhiteSpace(codigoEstadoRG) ? repEstado.BuscarPorSigla(codigoEstadoRG) : null;
                pessoa.CodigoSap = (string)dynDadosAdicionais.CodigoSap;
                pessoa.Referencia = (string)dynDadosAdicionais.Referencia;
                pessoa.ExigeEtiquetagem = ((string)dynDadosAdicionais.ExigeEtiquetagem).ToBool();
                pessoa.EhPontoDeApoio = ((string)dynDadosAdicionais.EhPontoDeApoio).ToBool();
                pessoa.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = ((string)dynDadosAdicionais.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento).ToBool();
                pessoa.NaoComprarValePedagio = ((string)dynDadosAdicionais.NaoComprarValePedagio).ToBool();
                pessoa.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente = ((string)dynDadosAdicionais.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente).ToBool();
                pessoa.MesoRegiao = codigoMesoRegiao > 0 ? repMesoRegiao.BuscarPorCodigo(codigoMesoRegiao, false) : null;
                pessoa.Regiao = codigoRegiao > 0 ? repRegiao.BuscarPorCodigo(codigoRegiao, false) : null;
                pessoa.TipoClienteIntegracaoLBC = (TipoClienteIntegracaoLBC)dynDadosAdicionais.TipoClienteIntegracaoLBC;
                pessoa.NaoAplicarChecklistMultiMobile = ((string)dynDadosAdicionais.NaoAplicarChecklistMultiMobile).ToBool();
                pessoa.FazParteGrupoEconomico = (bool)dynDadosAdicionais.FazParteGrupoEconomico;
                pessoa.RegraPallet = ((string)dynDadosAdicionais.RegraPallet).ToEnum<RegraPallet>();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    SituacaoFinanceira? situacaoFinanceiraAnterior = pessoa.SituacaoFinanceira;
                    pessoa.SituacaoFinanceira = ((string)dynDadosAdicionais.SituacaoFinanceira).ToNullableEnum<SituacaoFinanceira>();
                    if (pessoa.SituacaoFinanceira.HasValue && pessoa.SituacaoFinanceira != situacaoFinanceiraAnterior)
                        pessoa.DataAlteracaoSituacaoFinanceira = DateTime.Now;
                }
            }
        }

        private void SalvarObservacoesCTe(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteObservacaoCTe repClienteObservacaoCTe = new Repositorio.Embarcador.Pessoas.ClienteObservacaoCTe(unitOfWork);

            dynamic observacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ObservacoesCTes"));

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe> observacoesCTes = repClienteObservacaoCTe.BuscarPorPessoa(cliente.CPF_CNPJ);

            if (observacoesCTes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic observacao in observacoes)
                    if (observacao.Codigo != null)
                        codigos.Add((int)observacao.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe> observacoesDeletar = observacoesCTes.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

                for (int i = 0; i < observacoesDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe observacaoDeletar = observacoesDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.RemoveuObservacaoParaCTe + " " + observacaoDeletar.Descricao + ".", unitOfWork);

                    repClienteObservacaoCTe.Deletar(observacaoDeletar);
                }
            }

            foreach (dynamic observacao in observacoes)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe obs = null;

                int codigo = 0;
                if (observacao.Codigo != null && int.TryParse((string)observacao.Codigo, out codigo))
                    obs = repClienteObservacaoCTe.BuscarPorCodigo(codigo, true);

                if (obs == null)
                    obs = new Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe();

                obs.Cliente = cliente;
                obs.Identificador = (string)observacao.Identificador;
                obs.Texto = (string)observacao.Texto;
                obs.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe)observacao.Tipo;

                if (obs.Codigo > 0)
                {
                    repClienteObservacaoCTe.Atualizar(obs);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = obs.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, alteracoes, Localization.Resources.Pessoas.Pessoa.AlterouObservacaoParaCTe + $" {obs.Descricao}" + ".", unitOfWork);
                }
                else
                {
                    repClienteObservacaoCTe.Inserir(obs);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouObservacaoParaCTe + $" {obs.Descricao}" + ".", unitOfWork);
                }
            }
        }

        private void SalvarAreasRedex(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteAreaRedex repClienteRedex = new Repositorio.Embarcador.Pessoas.ClienteAreaRedex(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> clientesAreaRedex = repClienteRedex.BuscarPorCNPJCPFCliente(cliente.CPF_CNPJ);
            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex clienteAreaRedex in clientesAreaRedex)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.RemoveuAreaRedex + clienteAreaRedex.AreaRedex.Descricao + Localization.Resources.Pessoas.Pessoa.DaPessoa, unitOfWork);
                repClienteRedex.Deletar(clienteAreaRedex);
            }

            dynamic areasRedex = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AreasRedex"));
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacaoRedespacho");
            if (codigoTipoOperacao > 0)
                cliente.TipoOperacaoPadraoRedespachoAreaRedex = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            else
                cliente.TipoOperacaoPadraoRedespachoAreaRedex = null;

            if (areasRedex != null)
            {
                Repositorio.Cliente repClienteAreaRedex = new Repositorio.Cliente(unitOfWork);

                foreach (dynamic areaRedex in areasRedex)
                {
                    double cpfCnpj;
                    double.TryParse((string)areaRedex.ClienteRedex.Codigo, out cpfCnpj);

                    Dominio.Entidades.Cliente clienteAreaRedex = repClienteAreaRedex.BuscarPorCPFCNPJ(cpfCnpj);

                    Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex clienteRedex = new Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex
                    {
                        Cliente = cliente,
                        AreaRedex = clienteAreaRedex
                    };
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, Localization.Resources.Pessoas.Pessoa.AdicionouAreaRedex + clienteRedex.AreaRedex.Descricao + Localization.Resources.Pessoas.Pessoa.APessoa, unitOfWork);
                    repClienteRedex.Inserir(clienteRedex);
                }
            }
        }

        private void SalvarFrequenciaCarregamento(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento repClienteFrequenciaCarregamento = new Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic dynFrequenciasCarregamento = JsonConvert.DeserializeObject<dynamic>(Request.Params("FrequenciasCarregamento"));

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> frequenciasCarregamento = repClienteFrequenciaCarregamento.BuscarPorCliente(cliente.CPF_CNPJ);

            if (frequenciasCarregamento.Count > 0)
            {
                List<int> codigosTransportador = new List<int>();
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> frequenciasCarregamentoDeletar = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento>();

                foreach (dynamic frequenciaCarregamento in dynFrequenciasCarregamento)
                {
                    int codigoTransportador = ((string)frequenciaCarregamento.Transportador.Codigo).ToInt();
                    if (codigoTransportador > 0)
                        codigosTransportador.Add(codigoTransportador);

                    List<DiaSemana> diasSemana = new List<DiaSemana>();

                    dynamic dynDiaSemana = JsonConvert.DeserializeObject<dynamic>((string)frequenciaCarregamento.DiaSemana);

                    foreach (dynamic frequenciaCarregamentoDiaSemana in dynDiaSemana)
                        diasSemana.Add(((string)frequenciaCarregamentoDiaSemana).ToEnum<DiaSemana>());

                    frequenciasCarregamentoDeletar.AddRange(frequenciasCarregamento.Where(obj => obj.Empresa.Codigo == codigoTransportador && !diasSemana.Contains(obj.DiaSemana)).ToList());
                }

                frequenciasCarregamentoDeletar.AddRange(frequenciasCarregamento.Where(obj => !codigosTransportador.Contains(obj.Empresa.Codigo)).ToList());

                for (int i = 0; i < frequenciasCarregamentoDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento frequenciaCarregamentoDeletar = frequenciasCarregamentoDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, string.Format(Localization.Resources.Pessoas.Pessoa.RemoveuDiaDaSemanaDoTransportadorDaFrequenciaDeCarregamento, frequenciaCarregamentoDeletar.DiaSemana.ObterDescricao(), frequenciaCarregamentoDeletar.Empresa.RazaoSocial), unitOfWork);

                    repClienteFrequenciaCarregamento.Deletar(frequenciaCarregamentoDeletar);
                }
            }

            foreach (dynamic frequenciaCarregamento in dynFrequenciasCarregamento)
            {
                int codigoTransportador = ((string)frequenciaCarregamento.Transportador.Codigo).ToInt();

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                dynamic dynDiaSemana = JsonConvert.DeserializeObject<dynamic>((string)frequenciaCarregamento.DiaSemana);

                foreach (dynamic frequenciaCarregamentoDiaSemana in dynDiaSemana)
                {
                    DiaSemana diaSemana = ((string)frequenciaCarregamentoDiaSemana).ToEnum<DiaSemana>();

                    if (!frequenciasCarregamento.Any(obj => obj.Empresa.Codigo == codigoTransportador && obj.DiaSemana == diaSemana))
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento clienteFrequenciaCarregamento = new Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento();

                        clienteFrequenciaCarregamento.Cliente = cliente;
                        clienteFrequenciaCarregamento.Empresa = empresa;
                        clienteFrequenciaCarregamento.DiaSemana = diaSemana;

                        repClienteFrequenciaCarregamento.Inserir(clienteFrequenciaCarregamento);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, string.Format(Localization.Resources.Pessoas.Pessoa.AdicionouNoClienteFrequenciaDeCarregamentoNoDiaDaSemanaNoTransportador, diaSemana.ObterDescricao(), empresa.RazaoSocial), unitOfWork);
                    }
                }
            }
        }

        private void SalvarDataFixaVencimento(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaDataFixaVencimento repPessoaDataFixaVencimento = new Repositorio.Embarcador.Pessoas.PessoaDataFixaVencimento(unitOfWork);
            List<dynamic> dyndataFixaVencimento = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("DataFixaVencimento"));

            List<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento> dataFixaVencimentoExistentes = repPessoaDataFixaVencimento.BuscarPorPessoa(pessoa.CPF_CNPJ);

            List<int> codigos = dyndataFixaVencimento.Where(q => q.Codigo != null).Select(s => (int)s.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento> dataFixaVencimentoDeletar = dataFixaVencimentoExistentes.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

            foreach (Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento dataFixaVencimento in dataFixaVencimentoDeletar)
            {
                repPessoaDataFixaVencimento.Deletar(dataFixaVencimento);
            }
            foreach (dynamic obj in dyndataFixaVencimento)
            {
                Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento pessoaDataFixaVencimento = null;

                int codigo = 0;
                if (obj.Codigo != null && int.TryParse((string)obj.Codigo, out codigo))
                    pessoaDataFixaVencimento = repPessoaDataFixaVencimento.BuscarPorCodigo(codigo);

                int diaInicialEmissao;
                int.TryParse((string)obj.DiaInicialEmissao, out diaInicialEmissao);
                int diaFinalEmissao;
                int.TryParse((string)obj.DiaFinalEmissao, out diaFinalEmissao);
                int diaVencimento;
                int.TryParse((string)obj.DiaVencimento, out diaVencimento);

                if (pessoaDataFixaVencimento == null)
                    pessoaDataFixaVencimento = new Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento();

                pessoaDataFixaVencimento.Pessoa = pessoa;
                pessoaDataFixaVencimento.DiaInicialEmissao = diaInicialEmissao;
                pessoaDataFixaVencimento.DiaFinalEmissao = diaFinalEmissao;
                pessoaDataFixaVencimento.DiaVencimento = diaVencimento;

                if (pessoaDataFixaVencimento.Codigo > 0)
                    repPessoaDataFixaVencimento.Atualizar(pessoaDataFixaVencimento);
                else
                    repPessoaDataFixaVencimento.Inserir(pessoaDataFixaVencimento);

            }

        }

        private void SalvarAcrescimoDescontoAutomatico(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico repClienteAcrescimoDesconto = new Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repContratoAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);


            dynamic dynAcrescimoDescontoAutomatico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AcrescimoDescontoAutomatico"));

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico> acrescimoDescontoAutomatico = repClienteAcrescimoDesconto.BuscarPorPessoa(cliente.CPF_CNPJ);


            List<int> codigos = new List<int>();

            foreach (dynamic obj in dynAcrescimoDescontoAutomatico)
                if (obj.Codigo != null)
                    codigos.Add((int)obj.Regra.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico> listaClienteAcrescimoDescontoDeletar = acrescimoDescontoAutomatico.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

            for (int i = 0; i < listaClienteAcrescimoDescontoDeletar.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico clienteAcrescimoDescontoDeletar = listaClienteAcrescimoDescontoDeletar[i];

                repClienteAcrescimoDesconto.Deletar(clienteAcrescimoDescontoDeletar);
            }



            foreach (dynamic obj in dynAcrescimoDescontoAutomatico)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico clienteAcrescimoDesconto = null;

                int codigo = 0;
                if (obj.Codigo != null && int.TryParse((string)dynAcrescimoDescontoAutomatico.Regra.Codigo, out codigo))
                    clienteAcrescimoDesconto = repClienteAcrescimoDesconto.BuscarPorCodigo(codigo, true);

                if (clienteAcrescimoDesconto == null)
                    clienteAcrescimoDesconto = new Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico();

                clienteAcrescimoDesconto.Cliente = cliente;
                clienteAcrescimoDesconto.AcrescimoDescontoAutomatico = repContratoAcrescimoDesconto.BuscarPorCodigo((int)obj.Regra.Codigo);

                if (clienteAcrescimoDesconto.Codigo > 0)
                    repClienteAcrescimoDesconto.Atualizar(clienteAcrescimoDesconto);
                else
                    repClienteAcrescimoDesconto.Inserir(clienteAcrescimoDesconto);
            }
        }

        private void SalvarTiposPagamentoCIOT(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas, Repositorio.UnitOfWork unitOfWork, dynamic dynTransportadorTerceiro)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repTipoPagamento = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repContratoAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);

            if (dynTransportadorTerceiro == null)
                return;

            dynamic dynTiposPagamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynTransportadorTerceiro.TiposPagamentoCIOTOperadora);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT> tiposPagamento = repTipoPagamento.BuscarPorModalidadeTransportador(modalidadeTransportadoraPessoas.Codigo);
            List<int> codigos = new List<int>();

            foreach (dynamic obj in dynTiposPagamento)
                if (obj.Codigo != null)
                    codigos.Add((int)obj.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT> listaTiposPagamentoDeletar = tiposPagamento.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

            for (int i = 0; i < listaTiposPagamentoDeletar.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT tipoPagamentoDeletar = listaTiposPagamentoDeletar[i];

                repTipoPagamento.Deletar(tipoPagamentoDeletar);
            }

            foreach (dynamic obj in dynTiposPagamento)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT tipoPagamento = null;

                int codigo = 0;
                if (obj.Codigo != null && int.TryParse((string)obj.Codigo ?? "", out codigo))
                    tipoPagamento = repTipoPagamento.BuscarPorCodigo(codigo, true);

                if (tipoPagamento == null)
                    tipoPagamento = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT();

                tipoPagamento.ModalidadeTransportadoraPessoas = modalidadeTransportadoraPessoas;
                tipoPagamento.TipoPagamentoCIOT = ((string)obj.TipoPagamentoCIOT).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT>();

                tipoPagamento.Operadora = ((string)obj.OperadoraCIOT).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT>();

                if (tipoPagamento.Codigo > 0)
                    repTipoPagamento.Atualizar(tipoPagamento);
                else
                    repTipoPagamento.Inserir(tipoPagamento);
            }
        }

        private void SalvarDiasFechamentoCIOTPeriodo(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas, Repositorio.UnitOfWork unitOfWork, dynamic dynTransportadorTerceiro)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo repDiaFechamento = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo(unitOfWork);

            if (dynTransportadorTerceiro == null)
                return;

            dynamic dynDiasFechamentoCIOT = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)dynTransportadorTerceiro.DiasFechamentoCIOTPeriodo);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo> diasFechamentoCIOT = repDiaFechamento.BuscarPorModalidadeTransportador(modalidadeTransportadoraPessoas.Codigo);
            List<int> codigos = new List<int>();

            foreach (dynamic obj in dynDiasFechamentoCIOT)
                if (obj.Codigo != null)
                    codigos.Add((int)obj.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo> listaDiasFechamentoCIOTDeletar = diasFechamentoCIOT.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

            for (int i = 0; i < listaDiasFechamentoCIOTDeletar.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo diaFechamentoCIOTDeletar = listaDiasFechamentoCIOTDeletar[i];
                repDiaFechamento.Deletar(diaFechamentoCIOTDeletar);
            }

            if (modalidadeTransportadoraPessoas.TipoGeracaoCIOT == TipoGeracaoCIOT.PorPeriodo)
            {
                int totalRegistros = 0;
                foreach (dynamic obj in dynDiasFechamentoCIOT)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo diaFechamentoCIOT = null;

                    totalRegistros++;
                    int codigo = 0;
                    if (obj.Codigo != null && int.TryParse((string)obj.Codigo ?? "", out codigo))
                        diaFechamentoCIOT = repDiaFechamento.BuscarPorCodigo(codigo, true);

                    if (diaFechamentoCIOT == null)
                        diaFechamentoCIOT = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo();

                    diaFechamentoCIOT.ModalidadeTransportadoraPessoas = modalidadeTransportadoraPessoas;
                    if (int.TryParse((string)obj.DiaFechamentoCIOT, out int diaFechamentoCIOTAux) && diaFechamentoCIOTAux > 0)
                        diaFechamentoCIOT.DiaFechamentoCIOT = diaFechamentoCIOTAux;

                    if (diaFechamentoCIOT.Codigo > 0)
                        repDiaFechamento.Atualizar(diaFechamentoCIOT);
                    else
                        repDiaFechamento.Inserir(diaFechamentoCIOT);
                }

                if (totalRegistros == 0)
                    throw new ControllerException(Localization.Resources.Pessoas.Pessoa.ParaAberturaCIOTAutomaticaENecessarioInformarDiasFechamento);
            }
        }

        private TimeSpan RetornarTimeSpan(string strTempo)
        {
            if (strTempo != string.Empty)
            {
                string[] HrMin = strTempo.Split(':');
                double hr = HrMin[0].ToDouble();
                double min = HrMin[1].ToDouble();
                TimeSpan tempo = TimeSpan.FromHours(hr) + TimeSpan.FromMinutes(min);

                return tempo;
            }
            else
                return TimeSpan.Zero;
        }

        private dynamic RetornaDynSuprimentoDeGas(List<Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas> suprimentosDeGas)
        {
            return (from o in suprimentosDeGas
                    select new
                    {
                        o.Codigo,
                        o.Capacidade,
                        o.Lastro,
                        o.EstoqueMinimo,
                        o.EstoqueMaximo,
                        ModeloVeicularPadrao = o.ModeloVeicularPadrao?.Codigo ?? 0,
                        ModeloVeicularPadraoDescricao = o.ModeloVeicularPadrao?.Descricao,
                        SupridorPadrao = o.SupridorPadrao?.CPF_CNPJ ?? 0,
                        SupridorPadraoDescricao = o.SupridorPadrao?.Descricao,
                        TipoOperacaoPadrao = o.TipoOperacaoPadrao?.Codigo ?? 0,
                        TipoOperacaoPadraoDescricao = o.TipoOperacaoPadrao?.Descricao,
                        ProdutoPadrao = o.ProdutoPadrao?.Codigo ?? 0,
                        ProdutoPadraoDescricao = o.ProdutoPadrao?.Descricao,
                        TipoCargaPadrao = o.TipoCargaPadrao?.Codigo ?? 0,
                        TipoCargaPadraoDescricao = o.TipoCargaPadrao?.Descricao,
                        o.NotificarPorEmailBloqueio,
                        o.NotificarPorEmailGerente,
                        o.NotificarPorEmailLimite,
                        HoraBloqueioSolicitacao = o.HoraBloqueioSolicitacao?.ToString(@"hh\:mm"),
                        HoraLimiteGerente = o.HoraLimiteGerente?.ToString(@"hh\:mm"),
                        HoraLimiteSolicitacao = o.HoraLimiteSolicitacao?.ToString(@"hh\:mm")
                    }).ToList();
        }

        private void SalvarUsuarioTerceiroAdicionais(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiTransporteTerceiro)
        {
            if (ConfiguracaoEmbarcador.NaoUtilizarUsuarioTransportadorTerceiro)
                return;

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (possuiTransporteTerceiro)
            {
                dynamic listaUsuariosAdicionais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaUsuariosAdicionais"));

                string login = string.Empty;
                bool inserir;
                Random numAleatorio = new Random();
                int valorUK;

                foreach (dynamic UsuarioAdicional in listaUsuariosAdicionais)
                {
                    inserir = false;
                    login = UsuarioAdicional.UsuarioAcesso;
                    valorUK = numAleatorio.Next(1, 1000000);

                    string cnpj_cpf = UsuarioAdicional.CNPJ_CPF;

                    cnpj_cpf = new string(cnpj_cpf.Where(char.IsDigit).ToArray());

                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorClienteTerceiroAdicionais(cnpj_cpf, Dominio.Enumeradores.TipoAcesso.Terceiro);

                    if (usuario == null && repUsuario.ValidarControleDupliciadadeUsuarioTerceiroAdicional(valorUK, cnpj_cpf))
                    {
                        inserir = true;
                        usuario = new Dominio.Entidades.Usuario();
                        usuario.CPF = cnpj_cpf;
                        usuario.ClienteTerceiro = pessoa;
                        usuario.Tipo = "U";
                        usuario.DataNascimento = DateTime.Today;
                        usuario.DataAdmissao = DateTime.Today;
                        usuario.Salario = 0;
                        usuario.ControleDuplicidadeUK = valorUK;
                    }
                    else
                        usuario.Initialize();

                    usuario.Nome = UsuarioAdicional.NomeColaborador;
                    usuario.Email = UsuarioAdicional.Email;

                    if (!string.IsNullOrEmpty((string)UsuarioAdicional.Senha))
                        usuario.Senha = UsuarioAdicional.Senha;

                    usuario.Login = login;
                    usuario.UsuarioAdministrador = true;
                    usuario.Status = UsuarioAdicional.Ativo == "Ativo" ? "A" : "I";
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Terceiro;
                    usuario.Empresa = this.Empresa;

                    if (usuario.Setor == null)
                        usuario.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };

                    if (inserir)
                        repUsuario.Inserir(usuario, Auditado);
                    else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        repUsuario.Atualizar(usuario, Auditado);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        this.SalvarPermissoesPorServico(usuario, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros);

                    if (inserir)
                    {
                        Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                        Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = new Dominio.Entidades.Embarcador.Operacional.OperadorLogistica();
                        operadorLogistica.Ativo = true;
                        operadorLogistica.SupervisorLogistica = true;
                        operadorLogistica.PermiteAdicionarComplementosDeFrete = true;
                        operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = false;
                        operadorLogistica.Usuario = usuario;
                        repOperadorLogistica.Inserir(operadorLogistica);
                    }
                }
            }
        }

        #endregion

        #region Métodos Privados Restrições da Fila de Carregamento

        private void AtualizarRestricoesFilaCarregamento(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic restricoesFilaCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RestricoesFilaCarregamento"));

            ExcluirRestricoesFilaCarregamentoRemovidas(pessoa, restricoesFilaCarregamento, unitOfWork);
            SalvarRestricoesFilaCarregamentoAdicionadasOuAtualizadas(pessoa, restricoesFilaCarregamento, unitOfWork);
        }

        private void ExcluirRestricoesFilaCarregamentoRemovidas(Dominio.Entidades.Cliente pessoa, dynamic restricoesFilaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (pessoa.RestricoesFilaCarregamento != null)
            {
                Repositorio.ClienteRestricaoFilaCarregamento repositorioRestricaoFilaCarregamento = new Repositorio.ClienteRestricaoFilaCarregamento(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic restricao in restricoesFilaCarregamento)
                {
                    int? codigo = ((string)restricao.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.ClienteRestricaoFilaCarregamento> listaRestricaoFilaCarregamento = (from restricao in pessoa.RestricoesFilaCarregamento where !listaCodigosAtualizados.Contains(restricao.Codigo) select restricao).ToList();

                foreach (Dominio.Entidades.ClienteRestricaoFilaCarregamento restricao in listaRestricaoFilaCarregamento)
                {
                    repositorioRestricaoFilaCarregamento.Deletar(restricao);
                }

                if (listaRestricaoFilaCarregamento.Count > 0)
                {
                    string descricaoAcao = listaRestricaoFilaCarregamento.Count == 1 ? Localization.Resources.Pessoas.Pessoa.RestricaoDaFilaDeCarregamentoRemovida : Localization.Resources.Pessoas.Pessoa.MultiplasRestricoesFaFilaDeCarregamentoRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, descricaoAcao, unitOfWork);
                }
            }
        }

        private dynamic ObterRestricoesFilaCarregamento(Dominio.Entidades.Cliente pessoa)
        {
            return (
                from restricao in pessoa.RestricoesFilaCarregamento
                select new
                {
                    restricao.Codigo,
                    CodigoTipoCarga = restricao.TipoCarga.Codigo,
                    restricao.Tipo,
                    TipoDescricao = restricao.Tipo.ObterDescricao(),
                    TipoCargaDescricao = restricao.TipoCarga.Descricao
                }
            ).ToList();
        }

        private dynamic ObterListaVendedores(Dominio.Entidades.Cliente pessoa)
        {
            return (from obj in pessoa.Vendedores
                    where TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ?
                     obj.Funcionario.Empresa.Codigo == this.Usuario.Empresa.Codigo : true
                    select new
                    {
                        obj.Codigo,
                        CodigoFuncionario = obj.Funcionario?.Codigo ?? 0,
                        Funcionario = obj.Funcionario?.Nome ?? string.Empty,
                        CodigoTipoDeCarga = obj.TipoDeCarga?.Codigo ?? 0,
                        TipoDeCarga = obj.TipoDeCarga?.Descricao ?? string.Empty,
                        PercentualComissao = obj.PercentualComissao.ToString("n5"),
                        DataInicioVigencia = obj?.DataInicioVigencia?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFimVigencia = obj?.DataFimVigencia?.ToString("dd/MM/yyyy") ?? string.Empty,
                    }).ToList();
        }

        private dynamic ObterListaRecebedoresAutorizados(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaRecebedorAutorizado repRecebedorAutorizado = new Repositorio.Embarcador.Pessoas.PessoaRecebedorAutorizado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado> recebedores = repRecebedorAutorizado.BuscarPorPessoa(pessoa.CPF_CNPJ);

            return (from obj in recebedores
                    select new
                    {
                        obj.Codigo,
                        obj.Nome,
                        obj.CPF,
                        Foto = ObterBase64Imagem(Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Cliente", "FotoRecebedor" }), obj.GuidFoto + ".jpg")),
                    }).ToList();
        }

        private string ObterBase64Imagem(string caminho)
        {
            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return "data:image/png;base64," + base64ImageRepresentation;
            }
            else
            {
                return "";
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoCarga(int? codigoTipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!codigoTipoCarga.HasValue)
                return null;

            Repositorio.Embarcador.Cargas.TipoDeCarga repositorio = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoCarga.Value) ?? throw new ControllerException(Localization.Resources.Pessoas.Pessoa.TipoDeCargaNaoEncontrado);
        }

        private void PreencherDadosRestricaoFilaCarregamento(Dominio.Entidades.ClienteRestricaoFilaCarregamento clienteRestricaoFilaCarregamento, dynamic restricaoFilaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            clienteRestricaoFilaCarregamento.Tipo = ((string)restricaoFilaCarregamento.Tipo).ToEnum<Dominio.Enumeradores.TipoTomador>();
            clienteRestricaoFilaCarregamento.TipoCarga = ObterTipoCarga(((string)restricaoFilaCarregamento.CodigoTipoCarga).ToNullableInt(), unitOfWork);
        }

        private void SalvarRestricoesFilaCarregamentoAdicionadasOuAtualizadas(Dominio.Entidades.Cliente pessoa, dynamic restricoesFilaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ClienteRestricaoFilaCarregamento repositorioRestricaoFilaCarregamento = new Repositorio.ClienteRestricaoFilaCarregamento(unitOfWork);
            List<Dominio.Entidades.ClienteRestricaoFilaCarregamento> restricoesFilaCarregamentoCadastradasOuAtualizadas = new List<Dominio.Entidades.ClienteRestricaoFilaCarregamento>();
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (dynamic restricao in restricoesFilaCarregamento)
            {
                Dominio.Entidades.ClienteRestricaoFilaCarregamento clienteRestricaoFilaCarregamento;
                int? codigo = ((string)restricao.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    clienteRestricaoFilaCarregamento = repositorioRestricaoFilaCarregamento.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Pessoas.Pessoa.RestricaoDaFilaDeCarregamentoNaoEncontrada);
                else
                    clienteRestricaoFilaCarregamento = new Dominio.Entidades.ClienteRestricaoFilaCarregamento() { Cliente = pessoa };

                PreencherDadosRestricaoFilaCarregamento(clienteRestricaoFilaCarregamento, restricao, unitOfWork);

                restricoesFilaCarregamentoCadastradasOuAtualizadas.Add(clienteRestricaoFilaCarregamento);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += clienteRestricaoFilaCarregamento.GetChanges().Count > 0 ? 1 : 0;
                    repositorioRestricaoFilaCarregamento.Atualizar(clienteRestricaoFilaCarregamento);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioRestricaoFilaCarregamento.Inserir(clienteRestricaoFilaCarregamento);
                }
            }

            if (pessoa.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Pessoas.Pessoa.RestricaoDaFilaDeCarregamentoAtualizada : Localization.Resources.Pessoas.Pessoa.MultiplasRestricoesDaFilaDeCarregamentoAtualizadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, descricaoAcao, unitOfWork);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Pessoas.Pessoa.RestricaoDaFilaDeCarregamentoAdicionada : Localization.Resources.Pessoas.Pessoa.MultiplasRestricoesDaFilaDeCarregamentoAdicionada;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, descricaoAcao, unitOfWork);
                }
            }
        }

        #endregion

        #region Importação

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoPessoaParcial()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoDeIntegracao, Propriedade = "CodigoIntegracao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Pessoas.Pessoa.CNPJ, Propriedade = "CNPJ", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pessoas.Pessoa.InscricaoMunicipal, Propriedade = "InscricaoMunicipal", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pessoas.Pessoa.IE, Propriedade = "IE", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Pessoas.Pessoa.NomeFantasia, Propriedade = "Fantasia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Pessoas.Pessoa.Telefone, Propriedade = "Telefone", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Pessoas.Pessoa.Email, Propriedade = "Email", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });


            return configuracoes;
        }

        public async Task<IActionResult> ConfiguracaoImportacaoPessoaParcial()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoPessoaParcial();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarPessoaParcial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            if (!this.Usuario.UsuarioMultisoftware)
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.SomenteUsuariosMulsoftwarePodemFazerEssaImportacao);

            Repositorio.Embarcador.Pessoas.ClienteParcial repClienteParcial = new Repositorio.Embarcador.Pessoas.ClienteParcial(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoPessoaParcial();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);


                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Dominio.Entidades.Embarcador.Pessoas.ClienteParcial clienteParcial = new Dominio.Entidades.Embarcador.Pessoas.ClienteParcial();
                        clienteParcial.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmail = (from obj in linha.Colunas where obj.NomeCampo == "Email" select obj).FirstOrDefault();
                        clienteParcial.Email = "";
                        if (colEmail != null)
                            clienteParcial.Email = colEmail.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracao" select obj).FirstOrDefault();
                        clienteParcial.CodigoIntegracao = "";
                        if (colCodigoIntegracao != null)
                            clienteParcial.CodigoIntegracao = colCodigoIntegracao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone = (from obj in linha.Colunas where obj.NomeCampo == "Telefone" select obj).FirstOrDefault();
                        clienteParcial.Telefone = "";
                        if (colTelefone != null)
                            clienteParcial.Telefone = colTelefone.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFantasia = (from obj in linha.Colunas where obj.NomeCampo == "Fantasia" select obj).FirstOrDefault();
                        clienteParcial.NomeFantasia = "";
                        if (colFantasia != null)
                            clienteParcial.NomeFantasia = colFantasia.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IE" select obj).FirstOrDefault();
                        clienteParcial.InscricaoEstadual = "";
                        if (colIE != null)
                            clienteParcial.InscricaoEstadual = colIE.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colInscricaoMuniciapal = (from obj in linha.Colunas where obj.NomeCampo == "InscricaoMunicipal" select obj).FirstOrDefault();
                        clienteParcial.InscricaoMunicipal = "";
                        if (colInscricaoMuniciapal != null)
                            clienteParcial.InscricaoMunicipal = colInscricaoMuniciapal.Valor;



                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJ = (from obj in linha.Colunas where obj.NomeCampo == "CNPJ" select obj).FirstOrDefault();
                        clienteParcial.CNPJ = "";
                        if (colCPFCNPJ != null)
                            clienteParcial.CNPJ = colCPFCNPJ.Valor;
                        else
                            retorno = "CNPJ é obrigatório";

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            repClienteParcial.Inserir(clienteParcial);

                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPessoa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();
            int tamanho = 200;
            bool categoriaObrigatoria = ConfiguracaoEmbarcador?.ExigirCategoriaCadastroPessoa ?? false;

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoDeIntegracao, Propriedade = "CodigoIntegracao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Pessoas.Pessoa.CNPJCPF, Propriedade = "CNPJCPF", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Pessoas.Pessoa.IBGE, Propriedade = "IBGE", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pessoas.Pessoa.IERG, Propriedade = "IE", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Pessoas.Pessoa.RazaoSocial, Propriedade = "Razao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Pessoas.Pessoa.NomeFantasia, Propriedade = "Fantasia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Pessoas.Pessoa.Logradouro, Propriedade = "Logradouro", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Pessoas.Pessoa.Numero, Propriedade = "Numero", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Pessoas.Pessoa.Complemento, Propriedade = "Complemento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Pessoas.Pessoa.Bairro, Propriedade = "Bairro", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Pessoas.Pessoa.CEP, Propriedade = "CEP", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Pessoas.Pessoa.Telefone, Propriedade = "Telefone", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Pessoas.Pessoa.Email, Propriedade = "Email", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Pessoas.Pessoa.Localidade, Propriedade = "Localidade", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Pessoas.Pessoa.Estado, Propriedade = "Estado", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Pessoas.Pessoa.Latitude, Propriedade = "Latitude", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Pessoas.Pessoa.Longitude, Propriedade = "Longitude", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoGrupoDaPessoa, Propriedade = "GrupoPessoa", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Pessoas.Pessoa.TipoPessoaJuridicaZeroFisicaUm, Propriedade = "TipoPessoa", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = Localization.Resources.Pessoas.Pessoa.Fornecedor, Propriedade = "Fornecedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = Localization.Resources.Pessoas.Pessoa.NaoAtualizarEndereco, Propriedade = "NaoAtualizarEndereco", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = Localization.Resources.Pessoas.Pessoa.PessoaCategoria, Propriedade = "Categoria", Tamanho = 200, Obrigatorio = categoriaObrigatoria, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = Localization.Resources.Pessoas.Pessoa.CompartilharAcessoEntreClientesDoMesmoGrupo, Propriedade = "CompartilharAcessoEntreClientesDoMesmoGrupoDePessoa", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = Localization.Resources.Pessoas.Pessoa.Observacao, Propriedade = "Observacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = Localization.Resources.Pessoas.Pessoa.ContaContabil, Propriedade = "ContaContabil", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = Localization.Resources.Pessoas.Pessoa.TipoClienteNaoZeroSimUm, Propriedade = "TipoCliente", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = Localization.Resources.Pessoas.Pessoa.TipoFornecedorNaoZeroSimUm, Propriedade = "TipoFornecedor", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = Localization.Resources.Pessoas.Pessoa.TipoTransportadorTerceiroNaoZeroSimUm, Propriedade = "TipoTransportador", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = Localization.Resources.Pessoas.Pessoa.RNTRC, Propriedade = "RNTRC", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = Localization.Resources.Pessoas.Pessoa.DataEmissaoRNTRC, Propriedade = "DataEmissaoRNTRC", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = Localization.Resources.Pessoas.Pessoa.DataVencimentoRNTRC, Propriedade = "DataVencimentoRNTRC", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 32, Descricao = Localization.Resources.Pessoas.Pessoa.TipoDeTransportadorAgregadoZeroIndependenteUmOutrosDois, Propriedade = "TipoTransportadorTerceiro", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 33, Descricao = Localization.Resources.Pessoas.Pessoa.ReterImpostosNaoZeroSimUm, Propriedade = "ReterImpostosContratoFrete", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 34, Descricao = Localization.Resources.Pessoas.Pessoa.DiasVencimentAdiantamento, Propriedade = "DiasVencimentoAdiantamentoContratoFrete", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 35, Descricao = Localization.Resources.Pessoas.Pessoa.DiasVencimentoSaldo, Propriedade = "DiasVencimentoSaldoContratoFrete", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 36, Descricao = Localization.Resources.Pessoas.Pessoa.PorcentagemDeAdiantamento, Propriedade = "PercentualAdiantamentoFretesTerceiro", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 37, Descricao = Localization.Resources.Pessoas.Pessoa.ExigeEtiquetagem, Propriedade = "ExigeEtiquetagem", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 38, Descricao = Localization.Resources.Pessoas.Pessoa.ValorMinimoCarga, Propriedade = "ValorMinimoCarga", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 39, Descricao = Localization.Resources.Pessoas.Pessoa.TipoDeCarga, Propriedade = "TipoDeCarga", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 40, Descricao = Localization.Resources.Pessoas.Pessoa.GerarPedidoBloqueado, Propriedade = "GerarPedidoBloqueado", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 41, Descricao = Localization.Resources.Pessoas.Pessoa.AtividadeServicoIndustrial, Propriedade = "Atividade", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });

            if (repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FilaH))
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 42, Descricao = Localization.Resources.Pessoas.Pessoa.ExcecaoCheckinFilaH, Propriedade = "ExcecaoCheckinFilaH", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 43, Descricao = Localization.Resources.Pessoas.Pessoa.PorcentagemDeAbastecimentoNoValorDoFreteQuandoSubcontrataComoTerceiroEsseTransportador, Propriedade = "PercentualAbastecimentoFretesTerceiro", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 44, Descricao = Localization.Resources.Pessoas.Pessoa.TipoTerceiro, Propriedade = "TipoTerceiro", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 45, Descricao = Localization.Resources.Pessoas.Pessoa.RaioMetros, Propriedade = "RaioEmMetros", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });


            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 46, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoIndicadorIE, Propriedade = "CodigoIndicadorIE", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 47, Descricao = Localization.Resources.Pessoas.Pessoa.EmailStatus, Propriedade = "EmailStatus", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 48, Descricao = Localization.Resources.Pessoas.Pessoa.PisPasep, Propriedade = "PISPASEP", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 49, Descricao = Localization.Resources.Pessoas.Pessoa.DataNascimento, Propriedade = "DataNascimento", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 50, Descricao = Localization.Resources.Pessoas.Pessoa.EmailSecundarios, Propriedade = "EmailSecundario", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 51, Descricao = Localization.Resources.Pessoas.Pessoa.TipoEmailSecundario, Propriedade = "TipoEmailSecundario", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 52, Descricao = Localization.Resources.Pessoas.Pessoa.AtivarAcessoAoPortal, Propriedade = "AtivarAcessoPortal", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 53, Descricao = Localization.Resources.Pessoas.Pessoa.VisualizarApenasParaPedidoDesteTomador, Propriedade = "VisualizarApenasParaPedidoDesteTomador", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 54, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoBanco, Propriedade = "CodigoBanco", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 55, Descricao = Localization.Resources.Pessoas.Pessoa.Agencia, Propriedade = "Agencia", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 56, Descricao = Localization.Resources.Pessoas.Pessoa.Digito, Propriedade = "Digito", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 57, Descricao = Localization.Resources.Pessoas.Pessoa.NumeroConta, Propriedade = "NumeroConta", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 58, Descricao = Localization.Resources.Pessoas.Pessoa.TipoContaBanco, Propriedade = "TipoContaBanco", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 59, Descricao = Localization.Resources.Pessoas.Pessoa.TerceiroGerarCIOT, Propriedade = "TerceiroGerarCIOT", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 60, Descricao = Localization.Resources.Pessoas.Pessoa.TerceiroTipoCIOT, Propriedade = "TerceiroTipoCIOT", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 61, Descricao = Localization.Resources.Pessoas.Pessoa.TerceiroTipoFavorecidoCIOT, Propriedade = "TerceiroTipoFavorecidoCIOT", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 62, Descricao = Localization.Resources.Pessoas.Pessoa.TerceiroTipoPagamentoCIOT, Propriedade = "TerceiroTipoPagamentoCIOT", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 63, Descricao = Localization.Resources.Pessoas.Pessoa.TerceiroTipoQuitacaoCIOT, Propriedade = "TerceiroTipoQuitacaoCIOT", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 64, Descricao = Localization.Resources.Pessoas.Pessoa.TerceiroTipoAdiantamentoCIOT, Propriedade = "TerceiroTipoAdiantamentoCIOT", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 65, Descricao = Localization.Resources.Pessoas.Pessoa.Contato, Propriedade = "Contato", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 66, Descricao = Localization.Resources.Pessoas.Pessoa.ContatoTipo, Propriedade = "ContatoTipo", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 67, Descricao = Localization.Resources.Pessoas.Pessoa.ContatoAtivo, Propriedade = "ContatoAtivo", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 68, Descricao = Localization.Resources.Pessoas.Pessoa.ContatoEmail, Propriedade = "ContatoEmail", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 69, Descricao = Localization.Resources.Pessoas.Pessoa.ContatoTelefone, Propriedade = "ContatoTelefone", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 70, Descricao = Localization.Resources.Pessoas.Pessoa.ContatoCPF, Propriedade = "ContatoCPF", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 71, Descricao = Localization.Resources.Pessoas.Pessoa.ContatoCargo, Propriedade = "ContatoCargo", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 72, Descricao = Localization.Resources.Pessoas.Pessoa.NomeSocio, Propriedade = "NomeSocio", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 73, Descricao = Localization.Resources.Pessoas.Pessoa.CPFSocio, Propriedade = "CPFSocio", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 74, Descricao = Localization.Resources.Pessoas.Pessoa.AgendamentoExigeNotaFiscal, Propriedade = "AgendamentoExigeNotaFiscal", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 75, Descricao = Localization.Resources.Pessoas.Pessoa.ExigeAgendamento, Propriedade = "ExigeAgendamento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 76, Descricao = Localization.Resources.Pessoas.Pessoa.FormaAgendamento, Propriedade = "FormaAgendamento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 77, Descricao = Localization.Resources.Pessoas.Pessoa.TempoAgendamento, Propriedade = "TempoAgendamento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 78, Descricao = Localization.Resources.Pessoas.Pessoa.LinkParaAgendamento, Propriedade = "LinkParaAgendamento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 79, Descricao = Localization.Resources.Pessoas.Pessoa.AgendamentoDescargaObrigatorio, Propriedade = "AgendamentoDescargaObrigatorio", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 80, Descricao = Localization.Resources.Pessoas.Pessoa.ExigeQueSuasEntregasSejamAgendadas, Propriedade = "ExigeQueSuasEntregasSejamAgendadas", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoHierarquia()
        {
            bool categoriaObrigatoria = ConfiguracaoEmbarcador?.ExigirCategoriaCadastroPessoa ?? false;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoDeIntegracao, Propriedade = "CodigoIntegracao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Pessoas.Pessoa.CNPJCPF, Propriedade = "CPF", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pessoas.Pessoa.RG, Propriedade = "RG", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Pessoas.Pessoa.Nome, Propriedade = "Nome", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Pessoas.Pessoa.Endereco, Propriedade = "Endereco", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Pessoas.Pessoa.Numero, Propriedade = "Numero", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Pessoas.Pessoa.Complemento, Propriedade = "Complemento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Pessoas.Pessoa.Bairro, Propriedade = "Bairro", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Pessoas.Pessoa.Telefone, Propriedade = "Telefone", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Pessoas.Pessoa.Email, Propriedade = "Email", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoFuncionario, Propriedade = "CodigoFuncionario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoSuperior, Propriedade = "CodigoSuperior", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoFuncao, Propriedade = "CodigoFuncao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Pessoas.Pessoa.CNPJHierarquia, Propriedade = "CNPJ", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = Localization.Resources.Pessoas.Pessoa.TipoDeCarga, Propriedade = "TipoCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Pessoas.Pessoa.CodigoSAP, Propriedade = "CodigoSap", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoFrequenciaCarregamento()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Pessoas.Pessoa.CNPJTransportador, Propriedade = "CNPJTransportador", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Pessoas.Pessoa.CPFCNPJCliente, Propriedade = "CPFCNPJCliente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Pessoas.Pessoa.Domingo, Propriedade = "Domingo", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pessoas.Pessoa.SegundaFeira, Propriedade = "Segunda", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Pessoas.Pessoa.TercaFeira, Propriedade = "Terca", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Pessoas.Pessoa.QuartaFeira, Propriedade = "Quarta", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Pessoas.Pessoa.QuintaFeira, Propriedade = "Quinta", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Pessoas.Pessoa.SextaFeira, Propriedade = "Sexta", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Pessoas.Pessoa.Sabado, Propriedade = "Sabado", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Pessoas.Pessoa.ExcluirRegistrosNaoExistentesNaPlanilha, Propriedade = "ExcluirRegistros", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPessoa(unitOfWork);

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfiguracaoImportacaoVendedores()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoHierarquia();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfiguracaoImportacaoFrequenciasCarregamento()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoFrequenciaCarregamento();

            return new JsonpResult(configuracoes.ToList());
        }


        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();
            Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = serPoliticaSenha.BuscarPoliticaSenha(unitOfWork, TipoServicoMultisoftware);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPessoa(unitOfWork);
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                        Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmail = (from obj in linha.Colunas where obj.NomeCampo == "Email" select obj).FirstOrDefault();
                        clienteEmbarcador.Emails = "";
                        if (colEmail != null)
                            clienteEmbarcador.Emails = colEmail.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = (from obj in linha.Colunas where obj.NomeCampo == "Bairro" select obj).FirstOrDefault();
                        clienteEmbarcador.Bairro = "";
                        if (colBairro != null)
                            clienteEmbarcador.Bairro = colBairro.Valor;
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.BairroObrigatorio;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracao" select obj).FirstOrDefault();
                        clienteEmbarcador.CodigoCliente = "";
                        if (colCodigoIntegracao != null)
                            clienteEmbarcador.CodigoCliente = colCodigoIntegracao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEP = (from obj in linha.Colunas where obj.NomeCampo == "CEP" select obj).FirstOrDefault();
                        clienteEmbarcador.CEP = "";
                        if (colCEP != null)
                            clienteEmbarcador.CEP = colCEP.Valor;
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.CEPObrigatorio;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidade = (from obj in linha.Colunas where obj.NomeCampo == "Localidade" select obj).FirstOrDefault();
                        string localidade = "";
                        if (colLocalidade != null)
                            localidade = colLocalidade.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEstado = (from obj in linha.Colunas where obj.NomeCampo == "Estado" select obj).FirstOrDefault();
                        string estado = "";
                        if (colEstado != null)
                            estado = colEstado.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLatitude = (from obj in linha.Colunas where obj.NomeCampo == "Latitude" select obj).FirstOrDefault();
                        clienteEmbarcador.Latitude = "";
                        if (colLatitude != null)
                            clienteEmbarcador.Latitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(colLatitude.Valor.Replace(",", "."));

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLongitude = (from obj in linha.Colunas where obj.NomeCampo == "Longitude" select obj).FirstOrDefault();
                        clienteEmbarcador.Longitude = "";
                        if (colLongitude != null)
                            clienteEmbarcador.Longitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(colLongitude.Valor.Replace(",", "."));

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna logradouroCEP = (from obj in linha.Colunas where obj.NomeCampo == "Logradouro" select obj).FirstOrDefault();
                        clienteEmbarcador.Endereco = "";
                        if (logradouroCEP != null)
                            clienteEmbarcador.Endereco = logradouroCEP.Valor;
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.LogradouroObrigatorio;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplemento = (from obj in linha.Colunas where obj.NomeCampo == "Complemento" select obj).FirstOrDefault();
                        clienteEmbarcador.Complemento = "";
                        if (colComplemento != null)
                            clienteEmbarcador.Complemento = colComplemento.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGerarPedidoBloqueado = (from obj in linha.Colunas where obj.NomeCampo == "GerarPedidoBloqueado" select obj).FirstOrDefault();
                        clienteEmbarcador.GerarPedidoBloqueado = false;
                        if (colGerarPedidoBloqueado != null)
                            clienteEmbarcador.GerarPedidoBloqueado = ((string)colGerarPedidoBloqueado.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCategoria = (from obj in linha.Colunas where obj.NomeCampo == "Código da Categoria" select obj).FirstOrDefault();
                        clienteEmbarcador.CodigoCategoria = "";
                        if (colCategoria != null)
                        {
                            clienteEmbarcador.CodigoCategoria = colCategoria.Valor;
                            if (string.IsNullOrWhiteSpace(clienteEmbarcador.CodigoCategoria))
                                retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarCategoria;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = (from obj in linha.Colunas where obj.NomeCampo == "Numero" select obj).FirstOrDefault();
                        clienteEmbarcador.Numero = "S/N";
                        if (colNumero != null)
                            clienteEmbarcador.Numero = colNumero.Valor;

                        if (string.IsNullOrWhiteSpace(clienteEmbarcador.Numero))
                            clienteEmbarcador.Numero = "S/N";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone = (from obj in linha.Colunas where obj.NomeCampo == "Telefone" select obj).FirstOrDefault();
                        clienteEmbarcador.Telefone1 = "";
                        if (colTelefone != null)
                            clienteEmbarcador.Telefone1 = colTelefone.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIBGE = (from obj in linha.Colunas where obj.NomeCampo == "IBGE" select obj).FirstOrDefault();
                        int codIBGE = 0;
                        if (colIBGE != null)
                            int.TryParse((string)colIBGE.Valor, out codIBGE);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCompartilharAcessoEntrePessoasDoMesmoGrupo = (from obj in linha.Colunas where obj.NomeCampo == "CompartilharAcessoEntreClientesDoMesmoGrupoDePessoa" select obj).FirstOrDefault();
                        bool? compartilharAcesso = null;
                        if (colCompartilharAcessoEntrePessoasDoMesmoGrupo != null)
                        {
                            string compartilharAcessoEmString = ((string)colCompartilharAcessoEntrePessoasDoMesmoGrupo.Valor).ToUpper();
                            if (compartilharAcessoEmString == "SIM")
                                compartilharAcesso = true;
                            else if (compartilharAcessoEmString == "NAO" || compartilharAcessoEmString == "NÃO")
                                compartilharAcesso = false;
                        }
                        clienteEmbarcador.CompartilharAcessoEntreGrupoPessoas = compartilharAcesso;

                        if (codIBGE == 0 && !string.IsNullOrWhiteSpace(clienteEmbarcador.CEP))
                        {
                            using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
                            {
                                AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                                AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = repEndereco.BuscarCEP(Utilidades.String.OnlyNumbers(clienteEmbarcador.CEP));

                                if (enderecoCEP != null)
                                {
                                    codIBGE = int.Parse(enderecoCEP.Localidade.CodigoIBGE);

                                    if (string.IsNullOrWhiteSpace(clienteEmbarcador.Bairro))
                                        clienteEmbarcador.Bairro = enderecoCEP.Bairro?.Descricao;

                                }
                            }
                        }

                        decimal latitudeLocalidade = 0;
                        decimal longitudeLocalidade = 0;
                        if (codIBGE == 0)
                        {
                            Dominio.Entidades.Localidade localidadeporCidadade = null;

                            if (estado.Length == 3)
                                localidadeporCidadade = repLocalidade.BuscarPorDescricaoAbreviacaoUF(Utilidades.String.RemoveDiacritics(localidade), estado);
                            else
                                localidadeporCidadade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(localidade), estado);

                            if (localidadeporCidadade != null)
                            {
                                codIBGE = localidadeporCidadade.CodigoIBGE;
                                if (localidadeporCidadade.Latitude.HasValue && localidadeporCidadade.Longitude.HasValue)
                                {
                                    latitudeLocalidade = localidadeporCidadade.Latitude.Value;
                                    longitudeLocalidade = localidadeporCidadade.Longitude.Value;
                                }
                            }
                        }

                        if (codIBGE == 0)
                            retorno = Localization.Resources.Pessoas.Pessoa.LocalidadeDoClienteObrigatoria;

                        clienteEmbarcador.CodigoIBGECidade = codIBGE;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFantasia = (from obj in linha.Colunas where obj.NomeCampo == "Fantasia" select obj).FirstOrDefault();
                        clienteEmbarcador.NomeFantasia = "";
                        if (colFantasia != null)
                            clienteEmbarcador.NomeFantasia = colFantasia.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IE" select obj).FirstOrDefault();
                        clienteEmbarcador.RGIE = "";
                        if (colIE != null && colIE.Valor != "#N/A")
                            clienteEmbarcador.RGIE = colIE.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazao = (from obj in linha.Colunas where obj.NomeCampo == "Razao" select obj).FirstOrDefault();
                        clienteEmbarcador.RazaoSocial = "";
                        if (colRazao != null)
                            clienteEmbarcador.RazaoSocial = colRazao.Valor;
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.RazaoSocialObrigatoria;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoPessoa = (from obj in linha.Colunas where obj.NomeCampo == "TipoPessoa" select obj).FirstOrDefault();
                        string tipoPessoa = "0";
                        if (colTipoPessoa != null)
                        {
                            tipoPessoa = colTipoPessoa.Valor;
                            if (tipoPessoa == "2" || tipoPessoa.ToUpper() == "E")
                                clienteEmbarcador.Exportacao = true;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJ = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPF" select obj).FirstOrDefault();
                        clienteEmbarcador.CPFCNPJ = "";
                        if (colCPFCNPJ != null)
                        {
                            clienteEmbarcador.CPFCNPJ = colCPFCNPJ.Valor;
                            if (tipoPessoa == "1" || tipoPessoa.ToUpper() == "F")
                                clienteEmbarcador.CPFCNPJ = Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).ToLong().ToString("d11");
                            else if (tipoPessoa == "2" || tipoPessoa.ToUpper() == "E")
                                clienteEmbarcador.CPFCNPJ = repCliente.BuscarPorProximoExterior().ToString();
                            else
                                clienteEmbarcador.CPFCNPJ = Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).ToLong().ToString("d14");
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.CPFCNPJObrigatorio;

                        clienteEmbarcador.Pais = ConfiguracaoEmbarcador.Pais;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoPessoa = (from obj in linha.Colunas where obj.NomeCampo == "GrupoPessoa" select obj).FirstOrDefault();
                        clienteEmbarcador.CodigGrupoPessoa = "";
                        if (colGrupoPessoa != null)
                            clienteEmbarcador.CodigGrupoPessoa = colGrupoPessoa.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFornecedor = (from obj in linha.Colunas where obj.NomeCampo == "Fornecedor" select obj).FirstOrDefault();
                        string fornecedor = "";
                        if (colFornecedor != null)
                            fornecedor = ((string)colFornecedor.Valor).ToUpper().Trim();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoDeCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoDeCarga" select obj).FirstOrDefault();
                        if (colTipoDeCarga != null)
                            clienteEmbarcador.CodigoTipoDeCarga = (string)colTipoDeCarga.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                        clienteEmbarcador.Observacao = "";
                        if (colObservacao != null)
                            clienteEmbarcador.Observacao = colObservacao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAtualizarEndereco = (from obj in linha.Colunas where obj.NomeCampo == "NaoAtualizarEndereco" select obj).FirstOrDefault();
                        string strAtualizarEndereco = "";
                        if (colAtualizarEndereco != null)
                            strAtualizarEndereco = ((string)colAtualizarEndereco.Valor).ToUpper().Trim();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContaContabil = (from obj in linha.Colunas where obj.NomeCampo == "ContaContabil" select obj).FirstOrDefault();
                        clienteEmbarcador.ContaContabil = "";
                        if (colContaContabil != null)
                            clienteEmbarcador.ContaContabil = colContaContabil.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCliente = (from obj in linha.Colunas where obj.NomeCampo == "TipoCliente" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoCliente = false;
                        if (colTipoCliente != null)
                            clienteEmbarcador.TipoCliente = ((string)colTipoCliente.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoFornecedor = (from obj in linha.Colunas where obj.NomeCampo == "TipoFornecedor" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoFornecedor = false;
                        if (colTipoFornecedor != null)
                            clienteEmbarcador.TipoFornecedor = ((string)colTipoFornecedor.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoTransportador = (from obj in linha.Colunas where obj.NomeCampo == "TipoTransportador" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoTransportador = false;
                        if (colTipoTransportador != null)
                            clienteEmbarcador.TipoTransportador = ((string)colTipoTransportador.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRNTRC = (from obj in linha.Colunas where obj.NomeCampo == "RNTRC" select obj).FirstOrDefault();
                        clienteEmbarcador.RNTRC = "";
                        if (colRNTRC != null)
                            clienteEmbarcador.RNTRC = colRNTRC.Valor;
                        if (string.IsNullOrWhiteSpace(clienteEmbarcador.RNTRC) && clienteEmbarcador.TipoTransportador)
                            retorno = Localization.Resources.Pessoas.Pessoa.RNTRCObrigatorioParaTransportadorTerceiro;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEmissaoRNTRC = (from obj in linha.Colunas where obj.NomeCampo == "DataEmissaoRNTRC" select obj).FirstOrDefault();
                        clienteEmbarcador.DataEmissaoRNTRC = null;
                        if (colDataEmissaoRNTRC != null)
                            clienteEmbarcador.DataEmissaoRNTRC = ((string)colDataEmissaoRNTRC.Valor).ToNullableDateTime();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataVencimentoRNTRC = (from obj in linha.Colunas where obj.NomeCampo == "DataVencimentoRNTRC" select obj).FirstOrDefault();
                        clienteEmbarcador.DataVencimentoRNTRC = null;
                        if (colDataVencimentoRNTRC != null)
                            clienteEmbarcador.DataVencimentoRNTRC = ((string)colDataVencimentoRNTRC.Valor).ToNullableDateTime();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoTransportadorTerceiro = (from obj in linha.Colunas where obj.NomeCampo == "TipoTransportadorTerceiro" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoTransportadorTerceiro = null;
                        if (colTipoTransportadorTerceiro != null)
                            clienteEmbarcador.TipoTransportadorTerceiro = ((string)colTipoTransportadorTerceiro.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colReterImpostosContratoFrete = (from obj in linha.Colunas where obj.NomeCampo == "ReterImpostosContratoFrete" select obj).FirstOrDefault();
                        clienteEmbarcador.ReterImpostosContratoFrete = false;
                        if (colReterImpostosContratoFrete != null)
                            clienteEmbarcador.ReterImpostosContratoFrete = ((string)colReterImpostosContratoFrete.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDiasVencimentoAdiantamentoContratoFrete = (from obj in linha.Colunas where obj.NomeCampo == "DiasVencimentoAdiantamentoContratoFrete" select obj).FirstOrDefault();
                        clienteEmbarcador.DiasVencimentoAdiantamentoContratoFrete = null;
                        if (colDiasVencimentoAdiantamentoContratoFrete != null)
                            clienteEmbarcador.DiasVencimentoAdiantamentoContratoFrete = ((string)colDiasVencimentoAdiantamentoContratoFrete.Valor).ToNullableInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDiasVencimentoSaldoContratoFrete = (from obj in linha.Colunas where obj.NomeCampo == "DiasVencimentoSaldoContratoFrete" select obj).FirstOrDefault();
                        clienteEmbarcador.DiasVencimentoSaldoContratoFrete = null;
                        if (colDiasVencimentoSaldoContratoFrete != null)
                            clienteEmbarcador.DiasVencimentoSaldoContratoFrete = ((string)colDiasVencimentoSaldoContratoFrete.Valor).ToNullableInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPercentualAdiantamentoFretesTerceiro = (from obj in linha.Colunas where obj.NomeCampo == "PercentualAdiantamentoFretesTerceiro" select obj).FirstOrDefault();
                        clienteEmbarcador.PercentualAdiantamentoFretesTerceiro = 0m;
                        if (colPercentualAdiantamentoFretesTerceiro != null)
                            clienteEmbarcador.PercentualAdiantamentoFretesTerceiro = ((string)colPercentualAdiantamentoFretesTerceiro.Valor).ToDecimal();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExigeEtiquetagem = (from obj in linha.Colunas where obj.NomeCampo == "ExigeEtiquetagem" select obj).FirstOrDefault();
                        clienteEmbarcador.ExigeEtiquetagem = false;
                        if (colExigeEtiquetagem != null)
                            clienteEmbarcador.ExigeEtiquetagem = ((string)colExigeEtiquetagem.Valor).ToString().ObterSomenteNumerosELetras() == "B";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorMinimoCarga = (from obj in linha.Colunas where obj.NomeCampo == "ValorMinimoCarga" select obj).FirstOrDefault();
                        clienteEmbarcador.ValorMinimoCarga = 0m;
                        if (colValorMinimoCarga != null)
                            clienteEmbarcador.ValorMinimoCarga = ((string)colValorMinimoCarga.Valor).ToDecimal();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAtividade = (from obj in linha.Colunas where obj.NomeCampo == "Atividade" select obj).FirstOrDefault();
                        clienteEmbarcador.CodigoAtividade = 3;
                        if (colAtividade != null)
                            clienteEmbarcador.CodigoAtividade = ((string)colAtividade.Valor).ToInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExcecaoCheckinFilaH = (from obj in linha.Colunas where obj.NomeCampo == "ExcecaoCheckinFilaH" select obj).FirstOrDefault();
                        clienteEmbarcador.ExcecaoCheckinFilaH = false;
                        if (colExcecaoCheckinFilaH != null)
                        {
                            string tmp = (string)colExcecaoCheckinFilaH.Valor.Trim();
                            clienteEmbarcador.ExcecaoCheckinFilaH = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPercentualAbastecimentoFretesTerceiro = (from obj in linha.Colunas where obj.NomeCampo == "PercentualAbastecimentoFretesTerceiro" select obj).FirstOrDefault();
                        clienteEmbarcador.PercentualAbastecimentoFretesTerceiro = 0m;
                        if (colPercentualAbastecimentoFretesTerceiro != null)
                            clienteEmbarcador.PercentualAbastecimentoFretesTerceiro = ((string)colPercentualAbastecimentoFretesTerceiro.Valor).ToDecimal();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoTerceiro = (from obj in linha.Colunas where obj.NomeCampo == "TipoTerceiro" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoTerceiro = null;
                        if (colTipoTerceiro != null)
                            clienteEmbarcador.TipoTerceiro = colTipoTerceiro.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRaioEmMetros = (from obj in linha.Colunas where obj.NomeCampo == "RaioEmMetros" select obj).FirstOrDefault();
                        clienteEmbarcador.RaioEmMetros = null;
                        if (colRaioEmMetros != null)
                            clienteEmbarcador.RaioEmMetros = ((string)colRaioEmMetros.Valor).ToNullableInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIndicadorIE = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIndicadorIE" select obj).FirstOrDefault();
                        clienteEmbarcador.CodigoIndicadorIE = 0;
                        if (colIndicadorIE != null)
                            clienteEmbarcador.CodigoIndicadorIE = ((string)colIndicadorIE.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE>();
                        if (!string.IsNullOrWhiteSpace(clienteEmbarcador.RGIE) && clienteEmbarcador.CodigoIndicadorIE == 0)
                            clienteEmbarcador.CodigoIndicadorIE = IndicadorIE.ContribuinteICMS;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone2 = (from obj in linha.Colunas where obj.NomeCampo == "Telefone2" select obj).FirstOrDefault();
                        clienteEmbarcador.Telefone2 = "";
                        if (colTelefone2 != null)
                            clienteEmbarcador.Telefone2 = colTelefone2.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmailStatus = (from obj in linha.Colunas where obj.NomeCampo == "EmailStatus" select obj).FirstOrDefault();
                        clienteEmbarcador.EmailStatus = "";
                        if (colEmailStatus != null)
                            clienteEmbarcador.EmailStatus = ((string)colEmailStatus.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPISPASEP = (from obj in linha.Colunas where obj.NomeCampo == "PISPASEP" select obj).FirstOrDefault();
                        clienteEmbarcador.PISPASEP = "";
                        if (colPISPASEP != null)
                            clienteEmbarcador.PISPASEP = ((string)colPISPASEP.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataNascimento = (from obj in linha.Colunas where obj.NomeCampo == "DataNascimento" select obj).FirstOrDefault();
                        clienteEmbarcador.DataNascimento = null;
                        if (colDataNascimento != null)
                            clienteEmbarcador.DataNascimento = ((string)colDataNascimento.Valor).ToNullableDateTime();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmailSecundario = (from obj in linha.Colunas where obj.NomeCampo == "EmailSecundario" select obj).FirstOrDefault();
                        clienteEmbarcador.EmailSecundario = "";
                        if (colEmailSecundario != null)
                            clienteEmbarcador.EmailSecundario = ((string)colEmailSecundario.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoEmailSecundario = (from obj in linha.Colunas where obj.NomeCampo == "TipoEmailSecundario" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoEmailSecundario = null;
                        if (colTipoEmailSecundario != null)
                            clienteEmbarcador.TipoEmailSecundario = ((string)colTipoEmailSecundario.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAtivarAcessoPortal = (from obj in linha.Colunas where obj.NomeCampo == "AtivarAcessoPortal" select obj).FirstOrDefault();
                        clienteEmbarcador.AtivarAcessoPortal = false;
                        if (colAtivarAcessoPortal != null)
                            clienteEmbarcador.AtivarAcessoPortal = ((string)colAtivarAcessoPortal.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVisualizarApenasParaPedidoDesteTomador = (from obj in linha.Colunas where obj.NomeCampo == "VisualizarApenasParaPedidoDesteTomador" select obj).FirstOrDefault();
                        clienteEmbarcador.VisualizarApenasParaPedidoDesteTomador = false;
                        if (colVisualizarApenasParaPedidoDesteTomador != null)
                            clienteEmbarcador.VisualizarApenasParaPedidoDesteTomador = ((string)colVisualizarApenasParaPedidoDesteTomador.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBanco = (from obj in linha.Colunas where obj.NomeCampo == "CodigoBanco" select obj).FirstOrDefault();
                        clienteEmbarcador.CodigoBanco = 0;
                        if (colBanco != null)
                            clienteEmbarcador.CodigoBanco = ((string)colBanco.Valor).ToInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAgencia = (from obj in linha.Colunas where obj.NomeCampo == "Agencia" select obj).FirstOrDefault();
                        clienteEmbarcador.Agencia = "";
                        if (colAgencia != null)
                            clienteEmbarcador.Agencia = ((string)colAgencia.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroConta = (from obj in linha.Colunas where obj.NomeCampo == "NumeroConta" select obj).FirstOrDefault();
                        clienteEmbarcador.NumeroConta = "";
                        if (colNumeroConta != null)
                            clienteEmbarcador.NumeroConta = ((string)colNumeroConta.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDigito = (from obj in linha.Colunas where obj.NomeCampo == "Digito" select obj).FirstOrDefault();
                        clienteEmbarcador.Digito = "";
                        if (colDigito != null)
                            clienteEmbarcador.Digito = ((string)colDigito.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoConta = (from obj in linha.Colunas where obj.NomeCampo == "TipoConta" select obj).FirstOrDefault();
                        clienteEmbarcador.TipoContaBanco = null;
                        if (colTipoConta != null)
                            clienteEmbarcador.TipoContaBanco = ((string)colTipoConta.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerceiroGerarCIOT = (from obj in linha.Colunas where obj.NomeCampo == "TerceiroGerarCIOT" select obj).FirstOrDefault();
                        clienteEmbarcador.TerceiroGerarCIOT = false;
                        if (colTerceiroGerarCIOT != null)
                            clienteEmbarcador.TerceiroGerarCIOT = ((string)colTerceiroGerarCIOT.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerceiroTipoFavorecidoCIOT = (from obj in linha.Colunas where obj.NomeCampo == "TerceiroTipoFavorecidoCIOT" select obj).FirstOrDefault();
                        clienteEmbarcador.TerceiroTipoFavorecidoCIOT = null;
                        if (colTerceiroTipoFavorecidoCIOT != null)
                            clienteEmbarcador.TerceiroTipoFavorecidoCIOT = ((string)colTerceiroTipoFavorecidoCIOT.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerceiroTipoPagamentoCIOT = (from obj in linha.Colunas where obj.NomeCampo == "TerceiroTipoPagamentoCIOT" select obj).FirstOrDefault();
                        clienteEmbarcador.TerceiroTipoPagamentoCIOT = null;
                        if (colTerceiroTipoPagamentoCIOT != null)
                            clienteEmbarcador.TerceiroTipoPagamentoCIOT = ((string)colTerceiroTipoPagamentoCIOT.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerceiroTipoQuitacaoCIOT = (from obj in linha.Colunas where obj.NomeCampo == "TerceiroTipoQuitacaoCIOT" select obj).FirstOrDefault();
                        clienteEmbarcador.TerceiroTipoQuitacaoCIOT = null;
                        if (colTerceiroTipoQuitacaoCIOT != null)
                            clienteEmbarcador.TerceiroTipoQuitacaoCIOT = ((string)colTerceiroTipoQuitacaoCIOT.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerceiroTipoAdiantamentoCIOT = (from obj in linha.Colunas where obj.NomeCampo == "TerceiroTipoAdiantamentoCIOT" select obj).FirstOrDefault();
                        clienteEmbarcador.TerceiroTipoAdiantamentoCIOT = null;
                        if (colTerceiroTipoAdiantamentoCIOT != null)
                            clienteEmbarcador.TerceiroTipoAdiantamentoCIOT = ((string)colTerceiroTipoAdiantamentoCIOT.Valor).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT>();


                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContato = (from obj in linha.Colunas where obj.NomeCampo == "Contato" select obj).FirstOrDefault();
                        clienteEmbarcador.Contato = "";
                        if (colContato != null)
                            clienteEmbarcador.Contato = ((string)colContato.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContatoCPF = (from obj in linha.Colunas where obj.NomeCampo == "ContatoCPF" select obj).FirstOrDefault();
                        clienteEmbarcador.ContatoCPF = "";
                        if (colContatoCPF != null)
                            clienteEmbarcador.ContatoCPF = ((string)colContatoCPF.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNomeSocio = (from obj in linha.Colunas where obj.NomeCampo == "NomeSocio" select obj).FirstOrDefault();
                        clienteEmbarcador.NomeSocio = "";
                        if (colNomeSocio != null)
                            clienteEmbarcador.NomeSocio = ((string)colNomeSocio.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFSocio = (from obj in linha.Colunas where obj.NomeCampo == "CPFSocio" select obj).FirstOrDefault();
                        clienteEmbarcador.CPFSocio = "";
                        if (colCPFSocio != null)
                            clienteEmbarcador.CPFSocio = ((string)colCPFSocio.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContatoTipo = (from obj in linha.Colunas where obj.NomeCampo == "ContatoTipo" select obj).FirstOrDefault();
                        clienteEmbarcador.ContatoTipo = 0;
                        if (colContatoTipo != null)
                            clienteEmbarcador.ContatoTipo = ((string)colContatoTipo.Valor).ToInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContatoAtivo = (from obj in linha.Colunas where obj.NomeCampo == "ContatoAtivo" select obj).FirstOrDefault();
                        clienteEmbarcador.ContatoAtivo = true;
                        if (colContatoAtivo != null)
                            clienteEmbarcador.ContatoAtivo = ((string)colContatoAtivo.Valor).ToBool();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContatoEmail = (from obj in linha.Colunas where obj.NomeCampo == "ContatoEmail" select obj).FirstOrDefault();
                        clienteEmbarcador.ContatoEmail = "";
                        if (colContatoEmail != null)
                            clienteEmbarcador.ContatoEmail = ((string)colContatoEmail.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContatoTelefone = (from obj in linha.Colunas where obj.NomeCampo == "ContatoTelefone" select obj).FirstOrDefault();
                        clienteEmbarcador.ContatoTelefone = "";
                        if (colContatoTelefone != null)
                            clienteEmbarcador.ContatoTelefone = ((string)colContatoTelefone.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContatoCargo = (from obj in linha.Colunas where obj.NomeCampo == "ContatoCargo" select obj).FirstOrDefault();
                        clienteEmbarcador.ContatoCargo = "";
                        if (colContatoCargo != null)
                            clienteEmbarcador.ContatoCargo = ((string)colContatoCargo.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExigeQueSuasEntregasSejamAgendadas = (from obj in linha.Colunas where obj.NomeCampo == "ExigeQueSuasEntregasSejamAgendadas" select obj).FirstOrDefault();
                        clienteEmbarcador.ExigeQueSuasEntregasSejamAgendadas = false;
                        if (colExigeQueSuasEntregasSejamAgendadas != null)
                        {
                            clienteEmbarcador.ExigeQueSuasEntregasSejamAgendadas = ((string)colExigeQueSuasEntregasSejamAgendadas.Valor).ToBool();
                        }

                        Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteDescarga clienteDescarga = new Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteDescarga();
                        List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna> colunasDescargaImportacao = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna>();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAgendamentoExigeNotaFiscal = (from obj in linha.Colunas where obj.NomeCampo == "AgendamentoExigeNotaFiscal" select obj).FirstOrDefault();
                        if (colAgendamentoExigeNotaFiscal != null)
                        {
                            clienteDescarga.AgendamentoExigeNotaFiscal = ((string)colAgendamentoExigeNotaFiscal.Valor).ToBool();
                            colunasDescargaImportacao.Add(colAgendamentoExigeNotaFiscal);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExigeAgendamento = (from obj in linha.Colunas where obj.NomeCampo == "ExigeAgendamento" select obj).FirstOrDefault();
                        if (colExigeAgendamento != null)
                        {
                            clienteDescarga.ExigeAgendamento = ((string)colExigeAgendamento.Valor).ToBool();
                            colunasDescargaImportacao.Add(colExigeAgendamento);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAgendamentoDescargaObrigatorio = (from obj in linha.Colunas where obj.NomeCampo == "AgendamentoDescargaObrigatorio" select obj).FirstOrDefault();
                        if (colAgendamentoDescargaObrigatorio != null)
                        {
                            clienteDescarga.AgendamentoDescargaObrigatorio = ((string)colAgendamentoDescargaObrigatorio.Valor).ToBool();
                            colunasDescargaImportacao.Add(colAgendamentoDescargaObrigatorio);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFormaAgendamento = (from obj in linha.Colunas where obj.NomeCampo == "FormaAgendamento" select obj).FirstOrDefault();
                        if (colFormaAgendamento != null)
                        {
                            clienteDescarga.FormaAgendamento = ((string)colFormaAgendamento.Valor);
                            colunasDescargaImportacao.Add(colFormaAgendamento);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTempoAgendamento = (from obj in linha.Colunas where obj.NomeCampo == "TempoAgendamento" select obj).FirstOrDefault();
                        if (colTempoAgendamento != null)
                        {
                            clienteDescarga.TempoAgendamento = ((string)colTempoAgendamento.Valor);
                            colunasDescargaImportacao.Add(colTempoAgendamento);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLinkParaAgendamento = (from obj in linha.Colunas where obj.NomeCampo == "LinkParaAgendamento" select obj).FirstOrDefault();
                        if (colLinkParaAgendamento != null)
                        {
                            clienteDescarga.LinkParaAgendamento = ((string)colLinkParaAgendamento.Valor);
                            colunasDescargaImportacao.Add(colLinkParaAgendamento);
                        }

                        //Salvar o cliente
                        clienteEmbarcador.NaoAtualizarDadosCadastrais = strAtualizarEndereco == "SIM" ? true : false;
                        clienteEmbarcador.CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && Empresa.VisualizarSomenteClientesAssociados ? Empresa.Codigo : 0;
                        if (string.IsNullOrWhiteSpace(retorno))
                        {
                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoCliente = serCliente.converterClienteEmbarcador(clienteEmbarcador, Localization.Resources.Pessoas.Pessoa.Cliente, unitOfWork);
                            Dominio.Entidades.Cliente pessoa = retornoCliente.cliente;

                            if (!retornoCliente.Status)
                                retorno = retornoCliente.Mensagem;
                            else if (!string.IsNullOrWhiteSpace(fornecedor) && string.IsNullOrEmpty(retorno))
                                SalvarImportacaoFornecedor(pessoa, fornecedor, unitOfWork);
                            else if (colunasDescargaImportacao.Count > 0 && string.IsNullOrEmpty(retorno))
                                SalvarImportacaoDadosDescarga(pessoa, clienteDescarga, colunasDescargaImportacao, unitOfWork);

                            if (string.IsNullOrWhiteSpace(retorno) && pessoa != null)
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.ImportadoViaPlanilha, unitOfWork);
                        }

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            contador++;

                            double excedeuRaio = 0;
                            if (ConfiguracaoEmbarcador.RaioMaximoGeoLocalidadeGeoCliente > 0)
                            {
                                if (latitudeLocalidade != 0 && longitudeLocalidade != 0 && !string.IsNullOrEmpty(clienteEmbarcador.Latitude) && !string.IsNullOrEmpty(clienteEmbarcador.Longitude))
                                {
                                    double distancia = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia((double)latitudeLocalidade, (double)longitudeLocalidade, clienteEmbarcador.Latitude.ToDouble(), clienteEmbarcador.Longitude.ToDouble()) / 1000;
                                    if (distancia > ConfiguracaoEmbarcador.RaioMaximoGeoLocalidadeGeoCliente)
                                        excedeuRaio = distancia;
                                }
                            }
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = (excedeuRaio == 0 ? "" : Localization.Resources.Pessoas.Pessoa.VerificquePosicaoGeograficaDaPessoaPoisUltrapassouRaioMaximoComRelacaoSuaCidade + "( " + excedeuRaio.ToString("n2") + ").") };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarVendedores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Usuario repVendedor = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Pedidos.RegraTomador repRegraTomador = new Repositorio.Embarcador.Pedidos.RegraTomador(unitOfWork);
            Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorioHistoricoImportacao = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoHierarquia();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;
                string mensagemAuditoria = "";

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                        Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPF = (from obj in linha.Colunas where obj.NomeCampo == "CPF" select obj).FirstOrDefault();
                        Dominio.Entidades.Usuario vendedor = new Dominio.Entidades.Usuario();

                        if (colCPF != null)
                        {
                            string cpfCNPJ = Utilidades.String.OnlyNumbers((string)colCPF.Valor);

                            if (string.IsNullOrWhiteSpace(cpfCNPJ))
                                throw new Exception(Localization.Resources.Pessoas.Pessoa.CPFCNPJNaoInformado);

                            vendedor = repVendedor.BuscarPorCPF(cpfCNPJ);

                            if (vendedor == null)
                                vendedor = new Dominio.Entidades.Usuario();


                            vendedor.CPF = cpfCNPJ;
                        }

                        if (colCPF == null)
                            throw new Exception(Localization.Resources.Pessoas.Pessoa.CPFCNPJNaoInformado);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNome = (from obj in linha.Colunas where obj.NomeCampo == "Nome" select obj).FirstOrDefault();
                        if (colNome != null)
                            vendedor.Nome = colNome.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoFuncionario" select obj).FirstOrDefault();

                        if (colCodigoIntegracao != null)
                            vendedor.CodigoIntegracao = colCodigoIntegracao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRG = (from obj in linha.Colunas where obj.NomeCampo == "RG" select obj).FirstOrDefault();
                        if (colRG != null)
                            vendedor.RG = colRG.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEndereco = (from obj in linha.Colunas where obj.NomeCampo == "Endereco" select obj).FirstOrDefault();
                        if (colEndereco != null)
                            vendedor.Endereco = colEndereco.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = (from obj in linha.Colunas where obj.NomeCampo == "Numero" select obj).FirstOrDefault();
                        if (colNumero != null)
                            vendedor.NumeroEndereco = colNumero.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = (from obj in linha.Colunas where obj.NomeCampo == "Bairro" select obj).FirstOrDefault();
                        if (colBairro != null)
                            vendedor.Bairro = colBairro.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplemento = (from obj in linha.Colunas where obj.NomeCampo == "Complemento" select obj).FirstOrDefault();
                        if (colComplemento != null)
                            vendedor.Complemento = colComplemento.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone = (from obj in linha.Colunas where obj.NomeCampo == "Telefone" select obj).FirstOrDefault();
                        if (colTelefone != null)
                            vendedor.Telefone = colTelefone.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmail = (from obj in linha.Colunas where obj.NomeCampo == "Email" select obj).FirstOrDefault();
                        if (colEmail != null)
                            vendedor.Email = colEmail.Valor;

                        vendedor.TipoAcesso = TipoAcesso.Embarcador;

                        if (vendedor.Codigo > 0)
                        {
                            mensagemAuditoria = Localization.Resources.Pessoas.Pessoa.VendedorAtualizadoViaImportacaoManual;
                            repVendedor.Atualizar(vendedor);
                        }
                        else
                        {
                            mensagemAuditoria = Localization.Resources.Pessoas.Pessoa.VendedorAdicionadoViaImportacaoManual;
                            vendedor.Status = "A";
                            repVendedor.Inserir(vendedor);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, vendedor, mensagemAuditoria, unitOfWork);

                        unitOfWork.CommitChanges();

                        contador++;
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico importacaoHierarquiaHistorico = new Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico()
                {
                    Data = DateTime.Now,
                    TipoArquivo = "RE_",
                    NomeArquivo = "",
                    Descricao = Localization.Resources.Pessoas.Pessoa.ImportadoManualmente,
                    QuantidadeRegistrosTotal = linhas.Count,
                    QuantidadeRegistrosImportados = contador,
                    Situacao = SituacaoImportacaoHierarquia.Sucesso
                };

                repositorioHistoricoImportacao.Inserir(importacaoHierarquiaHistorico);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarSupervisores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorioHistoricoImportacao = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoHierarquia();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                        Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

                        string codigoFuncionario = "";
                        TipoComercial? codigoFuncao = null;
                        string aux = "";
                        string codigoSuperior = "";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoFuncionario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoFuncionario" select obj).FirstOrDefault();
                        if (colCodigoFuncionario != null)
                            codigoFuncionario = colCodigoFuncionario.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoFuncao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoFuncao" select obj).FirstOrDefault();
                        if (colCodigoFuncao != null)
                        {
                            aux = colCodigoFuncao.Valor;
                            codigoFuncao = aux.ToEnum<TipoComercial>();
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoSuperior = (from obj in linha.Colunas where obj.NomeCampo == "CodigoSuperior" select obj).FirstOrDefault();
                        if (colCodigoSuperior != null)
                            codigoSuperior = colCodigoSuperior.Valor;

                        if (!string.IsNullOrWhiteSpace(codigoFuncionario))
                        {
                            Dominio.Entidades.Usuario funcionario = repUsuario.BuscarPorCodigoIntegracao(codigoFuncionario);
                            if (funcionario != null)
                            {
                                funcionario.TipoComercial = codigoFuncao;
                                if (!string.IsNullOrWhiteSpace(codigoSuperior))
                                {
                                    Dominio.Entidades.Usuario superior = repUsuario.BuscarPorCodigoIntegracao(codigoSuperior);
                                    if (superior != null)
                                    {
                                        if (superior.TipoComercial == TipoComercial.SupervisorDanone)
                                            funcionario.Supervisor = superior;
                                        else if (superior.TipoComercial == TipoComercial.GerenteArea || superior.TipoComercial == TipoComercial.GerenteNacional || superior.TipoComercial == TipoComercial.GerenteRede)
                                            funcionario.Gerente = superior;
                                    }
                                }

                                repUsuario.Atualizar(funcionario);

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, funcionario, Localization.Resources.Pessoas.Pessoa.FuncionarioAtualizadoViaImportacaoManual, unitOfWork);
                            }
                        }

                        unitOfWork.CommitChanges();

                        contador++;
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico importacaoHierarquiaHistorico = new Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico()
                {
                    Data = DateTime.Now,
                    TipoArquivo = "DEREGISTRO",
                    NomeArquivo = "",
                    Descricao = "Importado Manualmente",
                    QuantidadeRegistrosTotal = linhas.Count,
                    QuantidadeRegistrosImportados = contador,
                    Situacao = SituacaoImportacaoHierarquia.Sucesso
                };

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarHierarquia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaFuncionario repPessoaFuncionario = new Repositorio.Embarcador.Pessoas.PessoaFuncionario(unitOfWork);
            Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorioHistoricoImportacao = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoHierarquia();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;
                string mensagemAuditoria = "";

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        bool sucesso = true;
                        Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                        Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

                        double cnpj = 0;
                        string cnpjAux = "";
                        string tipoCarga = "";
                        string codigoFuncionario = "";
                        string codigoSap = "";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJ = (from obj in linha.Colunas where obj.NomeCampo == "CNPJ" select obj).FirstOrDefault();
                        if (colCNPJ != null)
                        {
                            cnpjAux = colCNPJ.Valor;
                            cnpj = cnpjAux.ToDouble();
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                        if (colTipoCarga != null)
                            tipoCarga = colTipoCarga.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoFuncionario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoFuncionario" select obj).FirstOrDefault();
                        if (colCodigoFuncionario != null)
                            codigoFuncionario = colCodigoFuncionario.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoSap = (from obj in linha.Colunas where obj.NomeCampo == "CodigoSap" select obj).FirstOrDefault();
                        if (colCodigoSap != null)
                            codigoSap = colCodigoSap.Valor;

                        if (cnpj > 0)
                        {
                            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();
                            Dominio.Entidades.Usuario funcionario = new Dominio.Entidades.Usuario();

                            cliente = repCliente.BuscarPorCPFCNPJ(cnpj);
                            if (cliente != null)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario vendedor = repPessoaFuncionario.BuscarPorPessoaECodigoIntegracao(codigoFuncionario, cliente.CPF_CNPJ);
                                cliente.CodigoSap = codigoSap;

                                if (vendedor == null)
                                    vendedor = new Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario();

                                if (!string.IsNullOrWhiteSpace(codigoFuncionario))
                                {
                                    vendedor.Funcionario = repUsuario.BuscarPorCodigoIntegracao(codigoFuncionario);

                                    if (vendedor.Funcionario == null)
                                    {
                                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.FuncionarioNaoEncontradoNaBaseDeDados, i));
                                        sucesso = false;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(tipoCarga))
                                    vendedor.TipoDeCarga = repTipoDeCarga.BuscarPorCodigoErp(tipoCarga);

                                vendedor.Pessoa = cliente;
                                if (vendedor.Codigo > 0)
                                {
                                    repPessoaFuncionario.Atualizar(vendedor);
                                    mensagemAuditoria = Localization.Resources.Pessoas.Pessoa.VendedorAtualizadoViaImportacaoManual;
                                }
                                else
                                {
                                    repPessoaFuncionario.Inserir(vendedor);
                                    mensagemAuditoria = Localization.Resources.Pessoas.Pessoa.VendedorInseridoViaImportacaoManual;
                                }

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, vendedor, mensagemAuditoria, unitOfWork);
                            }
                        }
                        if (sucesso)
                            retornoImportacao.Retornolinhas.Add(RetornarSucessoLinha(i));

                        unitOfWork.CommitChanges();
                        contador++;
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico importacaoHierarquiaHistorico = new Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico()
                {
                    Data = DateTime.Now,
                    TipoArquivo = "DECLIUNREG",
                    NomeArquivo = "",
                    Descricao = Localization.Resources.Pessoas.Pessoa.ImportadoManualmente,
                    QuantidadeRegistrosTotal = linhas.Count,
                    QuantidadeRegistrosImportados = contador,
                    Situacao = SituacaoImportacaoHierarquia.Sucesso
                };

                repositorioHistoricoImportacao.Inserir(importacaoHierarquiaHistorico);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarFrequenciaCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento repClienteFrequenciaCarregamento = new Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoFrequenciaCarregamento();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<(double cnpjCPFCliente, int codigoTransportador)> listaImportados = new List<(double cnpjCPFCliente, int codigoTransportador)>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        bool sucesso = false;
                        Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                        Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

                        double cnpjCPFCliente = 0;
                        string cnpjAux = "";
                        string cnpjTransportador = "";
                        string retorno = "";
                        bool excluirRegistros = false;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExcluirRegistros = (from obj in linha.Colunas where obj.NomeCampo == "ExcluirRegistros" select obj).FirstOrDefault();
                        if (colExcluirRegistros != null)
                            if (colExcluirRegistros.Valor == "1")
                                excluirRegistros = true;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJCPFCliente = (from obj in linha.Colunas where obj.NomeCampo == "CPFCNPJCliente" select obj).FirstOrDefault();
                        if (colCNPJCPFCliente != null)
                        {
                            cnpjAux = colCNPJCPFCliente.Valor;
                            cnpjCPFCliente = Utilidades.String.OnlyNumbers(cnpjAux).ToDouble();
                        }
                        if (cnpjCPFCliente == 0)
                            retorno = Localization.Resources.Pessoas.Pessoa.ClienteObrigatorio;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJTransportador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJTransportador" select obj).FirstOrDefault();
                        if (colCNPJTransportador != null)
                            cnpjTransportador = Utilidades.String.OnlyNumbers(colCNPJTransportador.Valor);
                        if (string.IsNullOrWhiteSpace(cnpjTransportador))
                            retorno = Localization.Resources.Pessoas.Pessoa.TransportadorObrigatorio;

                        List<DiaSemana> diasSemana = new List<DiaSemana>();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSeg = (from obj in linha.Colunas where obj.NomeCampo == "Segunda" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTer = (from obj in linha.Colunas where obj.NomeCampo == "Terca" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaQua = (from obj in linha.Colunas where obj.NomeCampo == "Quarta" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaQui = (from obj in linha.Colunas where obj.NomeCampo == "Quinta" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSex = (from obj in linha.Colunas where obj.NomeCampo == "Sexta" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSab = (from obj in linha.Colunas where obj.NomeCampo == "Sabado" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDom = (from obj in linha.Colunas where obj.NomeCampo == "Domingo" select obj).FirstOrDefault();

                        if (colunaSeg != null && !string.IsNullOrWhiteSpace(colunaSeg.Valor))
                        {
                            if (colunaSeg.Valor == "1")
                                diasSemana.Add(DiaSemana.Segunda);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNaSegundaFeira;

                        if (colunaTer != null && !string.IsNullOrWhiteSpace(colunaTer.Valor))
                        {
                            if (colunaTer.Valor == "1")
                                diasSemana.Add(DiaSemana.Terca);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNaTercaFeira;

                        if (colunaQua != null && !string.IsNullOrWhiteSpace(colunaQua.Valor))
                        {
                            if (colunaQua.Valor == "1")
                                diasSemana.Add(DiaSemana.Quarta);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNaQuartaFeira;

                        if (colunaQui != null && !string.IsNullOrWhiteSpace(colunaQui.Valor))
                        {
                            if (colunaQui.Valor == "1")
                                diasSemana.Add(DiaSemana.Quinta);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNaQuintaFeira;

                        if (colunaSex != null && !string.IsNullOrWhiteSpace(colunaSex.Valor))
                        {
                            if (colunaSex.Valor == "1")
                                diasSemana.Add(DiaSemana.Sexta);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNaSextaFeira;

                        if (colunaSab != null && !string.IsNullOrWhiteSpace(colunaSab.Valor))
                        {
                            if (colunaSab.Valor == "1")
                                diasSemana.Add(DiaSemana.Sabado);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNoSabado;

                        if (colunaDom != null && !string.IsNullOrWhiteSpace(colunaDom.Valor))
                        {
                            if (colunaDom.Valor == "1")
                                diasSemana.Add(DiaSemana.Domingo);
                        }
                        else
                            retorno = Localization.Resources.Pessoas.Pessoa.ObrigatorioInformarSeHaveraOuNaoFrequenciaNoDomingo;

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjCPFCliente);
                            if (cliente == null)
                                throw new ControllerException(Localization.Resources.Pessoas.Pessoa.ClienteNaoPossuiCadastroNoSistema);
                            else
                            {
                                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                                if (cliente == null)
                                    throw new ControllerException(Localization.Resources.Pessoas.Pessoa.TransportadorNaoPossuiCadastroNoSistema);
                                else
                                {
                                    if (excluirRegistros)
                                    {
                                        listaImportados.Add(ValueTuple.Create(cliente.CPF_CNPJ, transportador.Codigo));

                                        List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> frequenciasCarregamento = repClienteFrequenciaCarregamento.BuscarPorClienteETransportador(cliente.CPF_CNPJ, transportador.Codigo);

                                        if (frequenciasCarregamento.Count > 0)
                                        {
                                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> frequenciasCarregamentoDeletarTransportadores = frequenciasCarregamento.Where(obj => !diasSemana.Contains(obj.DiaSemana)).ToList();

                                            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento frequenciaCarregamentoDeletartTransportador in frequenciasCarregamentoDeletarTransportadores)
                                            {
                                                repClienteFrequenciaCarregamento.Deletar(frequenciaCarregamentoDeletartTransportador);
                                            }
                                        }
                                    }

                                    foreach (DiaSemana diaSemana in diasSemana)
                                    {
                                        List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> clientesFrequenciaCarregamento = repClienteFrequenciaCarregamento.BuscarPorCliente(cnpjCPFCliente);
                                        if (!clientesFrequenciaCarregamento.Any(obj => obj.Empresa.Codigo == transportador.Codigo && obj.DiaSemana == diaSemana))
                                        {
                                            Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento clienteFrequenciaCarregamento = new Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento();

                                            clienteFrequenciaCarregamento.Cliente = cliente;
                                            clienteFrequenciaCarregamento.Empresa = transportador;
                                            clienteFrequenciaCarregamento.DiaSemana = diaSemana;

                                            repClienteFrequenciaCarregamento.Inserir(clienteFrequenciaCarregamento);
                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, string.Format(Localization.Resources.Pessoas.Pessoa.AdicionouNoClienteFrequenciaDeCarregamentoNoDiaDaSemanaNoTransportador, diaSemana.ObterDescricao(), transportador.RazaoSocial), unitOfWork);
                                        }
                                        sucesso = true;
                                    }
                                }
                            }
                        }

                        if (sucesso)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarSucessoLinha(i));
                            contador++;
                            unitOfWork.CommitChanges();
                        }
                        else
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Já existe uma frequência cadastrada para esse transportador neste dia.", i));

                    }
                    catch (ControllerException ex2)
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex2.Message, i));
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> frequenciasCarregamentos = repClienteFrequenciaCarregamento.BuscarPorCliente(listaImportados.Select(o => o.cnpjCPFCliente).Distinct().ToList());

                List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> frequenciasCarregamentoDeletar = frequenciasCarregamentos.Where(obj => !listaImportados.Select(o => o.codigoTransportador).Contains(obj.Empresa.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento frequenciaCarregamentoDeletar in frequenciasCarregamentoDeletar)
                    repClienteFrequenciaCarregamento.Deletar(frequenciaCarregamentoDeletar);

                unitOfWork.CommitChanges();

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true };
            return retorno;
        }

        private void SalvarImportacaoFornecedor(Dominio.Entidades.Cliente pessoa, string fornecedor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

            try
            {
                bool deletar = string.Compare(fornecedor, "SIM") == 0 ? false : true;

                if (deletar)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoasRemocao = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, pessoa.CPF_CNPJ);

                    if (modalidadePessoasRemocao == null)
                        return;

                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoasRemocao = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoasRemocao.Codigo);
                    modalidadeFornecedorPessoasRemocao = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoasRemocao.Codigo);

                    if (modalidadeFornecedorPessoasRemocao != null)
                    {
                        repModalidadeFornecedorPessoas.Deletar(modalidadeFornecedorPessoasRemocao);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuModalidade + modalidadeFornecedorPessoasRemocao.Descricao + ".", unitOfWork);
                    }

                    return;
                }

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(pessoa.CPF_CNPJ_SemFormato);
                bool inserir = true;

                if (usuario != null)
                {
                    inserir = false;
                    usuario.Initialize();
                }
                else
                {
                    usuario = new Dominio.Entidades.Usuario
                    {
                        CPF = pessoa.CPF_CNPJ_SemFormato,
                        ClienteFornecedor = pessoa
                    };

                    usuario.UsuarioAdministrador = true;
                    usuario.Cliente = pessoa;
                    usuario.Nome = pessoa.Nome;
                    usuario.Telefone = pessoa.Telefone1;
                    usuario.Localidade = pessoa.Localidade;
                    usuario.Endereco = pessoa.Endereco;
                    usuario.Complemento = pessoa.Complemento;
                    usuario.Email = pessoa.Email;
                    usuario.Login = pessoa.CPF_CNPJ_SemFormato;
                    usuario.Senha = pessoa.CPF_CNPJ_SemFormato.Substring(0, 5);
                    usuario.Status = "A";
                    usuario.TipoAcesso = TipoAcesso.Fornecedor;
                    usuario.Empresa = this.Empresa;
                }

                if (usuario.Setor == null)
                    usuario.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };

                if (inserir)
                    repUsuario.Inserir(usuario);
                else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    repUsuario.Atualizar(usuario);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, pessoa.CPF_CNPJ);

                if (modalidadePessoas == null)
                {
                    modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                    modalidadePessoas.Cliente = pessoa;
                    modalidadePessoas.TipoModalidade = TipoModalidade.Fornecedor;
                    repModalidadePessoas.Inserir(modalidadePessoas);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouModalidade + modalidadePessoas.DescricaoTipoModalidade + Localization.Resources.Pessoas.Pessoa.RemoveuModalidade, unitOfWork);
                }

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorCliente(pessoa.CPF_CNPJ);

                if (modalidadeFornecedorPessoas == null)
                {
                    modalidadeFornecedorPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas();
                    modalidadeFornecedorPessoas.ModalidadePessoas = modalidadePessoas;

                    repModalidadeFornecedorPessoas.Inserir(modalidadeFornecedorPessoas);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pessoa, null, Localization.Resources.Pessoas.Pessoa.AdicionouModalidadeFornecedor + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private string CriarParticipante(ref Dominio.Entidades.Cliente participante, string cnpjCPF, Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string tipoParticipante, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";


            Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
            Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmail = (from obj in linha.Colunas where obj.NomeCampo == "Email" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Emails = "";
            if (colEmail != null)
                clienteEmbarcador.Emails = colEmail.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = (from obj in linha.Colunas where obj.NomeCampo == "Bairro" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Bairro = "";
            if (colBairro != null)
                clienteEmbarcador.Bairro = colBairro.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEP = (from obj in linha.Colunas where obj.NomeCampo == "CEP" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.CEP = "";
            if (colCEP != null)
                clienteEmbarcador.CEP = colCEP.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna logradouroCEP = (from obj in linha.Colunas where obj.NomeCampo == "Logradouro" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Endereco = "";
            if (logradouroCEP != null)
                clienteEmbarcador.Endereco = logradouroCEP.Valor;
            else
                return "";

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplemento = (from obj in linha.Colunas where obj.NomeCampo == "Complemento" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Complemento = "";
            if (colComplemento != null)
                clienteEmbarcador.Complemento = colComplemento.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = (from obj in linha.Colunas where obj.NomeCampo == "Numero" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Numero = "S/N";
            if (colNumero != null)
                clienteEmbarcador.Numero = colNumero.Valor;

            if (string.IsNullOrWhiteSpace(clienteEmbarcador.Numero))
                clienteEmbarcador.Numero = "S/N";

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone = (from obj in linha.Colunas where obj.NomeCampo == "Telefone" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Telefone1 = "";
            clienteEmbarcador.Telefone2 = "";
            if (colTelefone != null)
                clienteEmbarcador.Telefone1 = colTelefone.Valor;


            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIBGE = (from obj in linha.Colunas where obj.NomeCampo == "IBGE" + tipoParticipante select obj).FirstOrDefault();
            int codIBGE = 0;
            if (colIBGE != null)
                int.TryParse((string)colIBGE.Valor, out codIBGE);
            else
                return "";

            clienteEmbarcador.CodigoIBGECidade = codIBGE;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFantasia = (from obj in linha.Colunas where obj.NomeCampo == "Fantasia" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.NomeFantasia = "";
            if (colFantasia != null)
                clienteEmbarcador.NomeFantasia = colFantasia.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IE" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.RGIE = "";
            if (colIE != null)
                clienteEmbarcador.RGIE = colIE.Valor;
            else
                return "";

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazao = (from obj in linha.Colunas where obj.NomeCampo == "Razao" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.RazaoSocial = "";
            if (colRazao != null)
                clienteEmbarcador.RazaoSocial = colRazao.Valor;
            else
                return "";

            clienteEmbarcador.CPFCNPJ = cnpjCPF;
            clienteEmbarcador.CodigoAtividade = 3;

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoCliente = serCliente.converterClienteEmbarcador(clienteEmbarcador, tipoParticipante, unitOfWork);
            if (retornoCliente.Status)
                participante = retornoCliente.cliente;
            else
                retorno = retornoCliente.Mensagem;


            return retorno;
        }

        private void SalvarListaSubarea(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.SubareaCliente repoSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
            Repositorio.Embarcador.Logistica.TipoSubareaCliente repoTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);
            Repositorio.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio repSubareaClienteAcoesFluxoDePatio = new Repositorio.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> subareasExistentes = repoSubareaCliente.BuscarPorCliente(cliente);

            string stringListaSubarea = Request.Params("ListaSubarea");
            dynamic subareasRecebidas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(stringListaSubarea);
            List<int> subareasAtualizadas = new List<int>();
            if (subareasRecebidas != null)
            {
                foreach (dynamic subareaRecebida in subareasRecebidas)
                {
                    int codigoTipoSubarea = Int32.Parse((string)subareaRecebida.TipoSubarea);
                    Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente tipoSubarea = repoTipoSubareaCliente.BuscarPorCodigo(codigoTipoSubarea, false);
                    if (tipoSubarea != null)
                    {
                        Dominio.Entidades.Embarcador.Logistica.SubareaCliente subareaAcoes = new Dominio.Entidades.Embarcador.Logistica.SubareaCliente();
                        int codigo = 0;
                        try
                        {
                            codigo = Int32.Parse((string)subareaRecebida.Codigo);
                        }
                        catch (FormatException ex) 
                        {
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter código da subárea do cliente para inteiro: {ex.ToString()}", "CatchNoAction");
                        }

                        if (codigo > 0)
                        {
                            // Subárea existente, edição
                            Dominio.Entidades.Embarcador.Logistica.SubareaCliente subareaExistente = repoSubareaCliente.BuscarPorCodigo(codigo, false);
                            if (subareaExistente != null)
                            {
                                subareaExistente.Descricao = subareaRecebida.Descricao;
                                subareaExistente.Area = subareaRecebida.Area;
                                subareaExistente.TipoSubarea = tipoSubarea;
                                subareaExistente.Ativo = subareaRecebida.Ativo;
                                subareaExistente.CodigoTag = subareaRecebida.CodigoTag;
                                repoSubareaCliente.Atualizar(subareaExistente);
                                subareasAtualizadas.Add(codigo);
                                subareaAcoes = subareaExistente;
                            }
                            else
                            {
                                codigo = 0;
                            }
                        }

                        if (codigo == 0)
                        {
                            Dominio.Entidades.Embarcador.Logistica.SubareaCliente novaSubarea = new Dominio.Entidades.Embarcador.Logistica.SubareaCliente();
                            novaSubarea.Cliente = cliente;
                            novaSubarea.Descricao = subareaRecebida.Descricao;
                            novaSubarea.Area = subareaRecebida.Area;
                            novaSubarea.TipoSubarea = tipoSubarea;
                            novaSubarea.Ativo = subareaRecebida.Ativo;
                            novaSubarea.CodigoTag = subareaRecebida.CodigoTag;
                            repoSubareaCliente.Inserir(novaSubarea);
                            subareaAcoes = novaSubarea;
                        }

                        if (subareaRecebida.ListaSubareaClienteAcoesFluxoDePatio.Count > 0)
                        {
                            excluirSubareaClienteAcoesFluxoDePatio(codigo, repSubareaClienteAcoesFluxoDePatio);

                            if ((tipoSubarea.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea ?? false))
                                gravarSubareaClienteAcoesFluxoDePatio(subareaRecebida, subareaAcoes, repSubareaClienteAcoesFluxoDePatio);
                        }
                    }
                }
            }

            // Remove as subareas removidas
            foreach (Dominio.Entidades.Embarcador.Logistica.SubareaCliente subareaExistente in subareasExistentes)
            {
                if (!subareasAtualizadas.Contains(subareaExistente.Codigo))
                {
                    subareaExistente.Cliente = null;
                    repoSubareaCliente.Atualizar(subareaExistente);
                }
            }

        }
        private void excluirSubareaClienteAcoesFluxoDePatio(int codigoSubAreaCliente, Repositorio.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio repSubareaClienteAcoesFluxoDePatio)
        {
            List<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio> listaAcoesSubAreaCliente = repSubareaClienteAcoesFluxoDePatio.BuscarPorCodigoSubareaCliente(codigoSubAreaCliente);
            foreach (Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio acao in listaAcoesSubAreaCliente)
                repSubareaClienteAcoesFluxoDePatio.Deletar(acao);
        }

        private void gravarSubareaClienteAcoesFluxoDePatio(dynamic subareaRecebida, Dominio.Entidades.Embarcador.Logistica.SubareaCliente subareaCliente, Repositorio.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio repSubareaClienteAcoesFluxoDePatio)
        {
            foreach (dynamic acaoFluxoDePatio in subareaRecebida.ListaSubareaClienteAcoesFluxoDePatio)
            {
                Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio acao = new Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio()
                {
                    SubareaCliente = subareaCliente,
                    AcaoMonitoramento = (MonitoramentoEventoData)acaoFluxoDePatio.CodigoAcaoMonitoramentoFluxoDePatio,
                    EtapaFluxoPatio = (EtapaFluxoGestaoPatio)acaoFluxoDePatio.CodigoEtapaFluxoDePatio,
                    AcaoFluxoPatio = (AcaoFluxoGestaoPatio)acaoFluxoDePatio.CodigoAcaoFluxoDePatio
                };
                repSubareaClienteAcoesFluxoDePatio.Inserir(acao);
            }
        }

        private void SalvarGrupoPessoas(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Cliente repUsuario = new Repositorio.Cliente(unitOfWork);

            dynamic gruposPessoas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GrupoPessoasAcessoPortal"));

            if (cliente.GruposPessoas != null && cliente.GruposPessoas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic grupoPessoa in gruposPessoas)
                    codigos.Add((int)grupoPessoa.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> tiposDeletar = cliente.GruposPessoas.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaDeletar in tiposDeletar)
                    cliente.GruposPessoas.Remove(grupoPessoaDeletar);
            }
            else
                cliente.GruposPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            foreach (dynamic grupoPessoa in gruposPessoas)
            {
                int.TryParse((string)grupoPessoa.Codigo, out int codigoGrupoPessoa);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas existeGrupoPessoas = repositorioGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);

                if (existeGrupoPessoas == null)
                    continue;

                bool existeGrupoPessoa = cliente.GruposPessoas.Any(o => o.Codigo == existeGrupoPessoas.Codigo);

                if (!existeGrupoPessoa)
                    cliente.GruposPessoas.Add(existeGrupoPessoas);
            }

            repUsuario.Atualizar(cliente);
        }

        private void SalvarTipoComprovantes(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unidadeDeTrabalho);
            dynamic tipoComprovantes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Comprovantes"));

            if (pessoa.TiposComprovante == null)
                pessoa.TiposComprovante = new List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoComprovante in tipoComprovantes)
                    codigos.Add((int)tipoComprovante.Tipo.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> tipoComprovantesDeletar = pessoa.TiposComprovante.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovanteDeletar in tipoComprovantesDeletar)
                    pessoa.TiposComprovante.Remove(tipoComprovanteDeletar);
            }

            foreach (dynamic tipoComprovante in tipoComprovantes)
            {
                if (pessoa.TiposComprovante.Any(o => o.Codigo == (int)tipoComprovante.Tipo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovanteObj = repTipoComprovante.BuscarPorCodigo((int)tipoComprovante.Tipo.Codigo);
                pessoa.TiposComprovante.Add(tipoComprovanteObj);
            }
        }

        private void SalvarFilialCliente(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            dynamic filialClientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FilialCliente"));

            if (pessoa.FilialCliente == null)
                pessoa.FilialCliente = new List<Dominio.Entidades.Cliente>();
            else
            {
                List<double> codigos = new List<double>();

                foreach (dynamic filial in filialClientes)
                    codigos.Add((double)filial.Codigo);

                List<Dominio.Entidades.Cliente> filiaisClienteRemover = pessoa.FilialCliente.Where(o => !codigos.Contains(o.CPF_CNPJ)).ToList();

                foreach (Dominio.Entidades.Cliente filialClienteRemover in filiaisClienteRemover)
                    pessoa.FilialCliente.Remove(filialClienteRemover);
            }

            foreach (dynamic filialCliente in filialClientes)
            {
                if (pessoa.FilialCliente.Any(o => o.Codigo == (double)filialCliente.Codigo))
                    continue;

                Dominio.Entidades.Cliente existeCliente = repositorioCliente.BuscarPorCPFCNPJ((double)filialCliente.Codigo);

                if (existeCliente == null)
                    continue;

                pessoa.FilialCliente.Add(existeCliente);
            }
            pessoa.PossuiFilialCliente = pessoa.FilialCliente.Count > 0;

            repositorioCliente.Atualizar(pessoa);
        }

        private dynamic ObterFilialCliente(Dominio.Entidades.Cliente pessoa)
        {
            return (from obj in pessoa.FilialCliente
                    select new
                    {
                        Codigo = obj.CPF_CNPJ,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private void SalvarCampoVendedorPortalMultiClifor(string login, string vendedor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor repositorioPortalMultiCliforVendedor = new Repositorio.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor portalMultiCliforVendedor = repositorioPortalMultiCliforVendedor.BuscarPorUsuarioAcessoPortal(login);


            if (portalMultiCliforVendedor == null)
            {
                if (string.IsNullOrEmpty(vendedor) || string.IsNullOrEmpty(login))
                    return;

                portalMultiCliforVendedor = new Dominio.Entidades.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor();
                portalMultiCliforVendedor.UsuarioAcessoPortal = repUsuario.BuscarPorLogin(login).Login;
                portalMultiCliforVendedor.Vendedor = vendedor;

                repositorioPortalMultiCliforVendedor.Inserir(portalMultiCliforVendedor);
            }
            else
            {
                portalMultiCliforVendedor.Vendedor = vendedor;

                repositorioPortalMultiCliforVendedor.Atualizar(portalMultiCliforVendedor);
            }

        }

        #endregion

        #region JsonEndereco

        private class JsonEndereco
        {
            public int? Codigo { get; set; }
            public int CodigoLocalidade { get; set; }
            public string CEP { get; set; }
            public string Bairro { get; set; }
            public string Complemento { get; set; }

            public string CodigoDocumento { get; set; }
            public string Endereco { get; set; }
            public bool EnderecoDigitado { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Numero { get; set; }
            public string IE { get; set; }
            public TipoEndereco TipoEndereco { get; set; }
            public TipoLogradouro TipoLogradouro { get; set; }
            public string AreaSecundario { get; set; }
            public TipoArea TipoAreaEnderecoSecundario { get; set; }
            public int? RaioEmMetrosSecundario { get; set; }
            public string CodigoIntegracao { get; set; }
            public string Telefone { get; set; }
        }

        private class JsonDocumento
        {
            public int? Codigo { get; set; }
            public string Descricao { get; set; }
            public string DataEmissao { get; set; }
            public string DataVencimento { get; set; }
        }

        #endregion
    }
}