using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoAvonController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("integracaoavon.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ReenviarDocumentosComFalhaNaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int minutaAvon;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoManifesto"]), out minutaAvon);

                Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unitOfWork);
                Repositorio.DocumentoManifestoAvon repDocumentosAvon = new Repositorio.DocumentoManifestoAvon(unitOfWork);


                Dominio.Entidades.ManifestoAvon manifestoAvon = repManifestoAvon.BuscarPorCodigo(minutaAvon);

                if (manifestoAvon.Status == Dominio.Enumeradores.StatusManifestoAvon.FalhaNoRetorno)
                {
                    List<Dominio.Entidades.DocumentoManifestoAvon> documentosComFalha = repDocumentosAvon.BuscarPorManifesto(minutaAvon, Dominio.Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno);
                    foreach (Dominio.Entidades.DocumentoManifestoAvon docManifesto in documentosComFalha)
                    {
                        docManifesto.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido;
                        docManifesto.NumeroTentativas = 0;
                        docManifesto.GuidEnvio = string.Empty;
                        repDocumentosAvon.Atualizar(docManifesto);
                    }

                    manifestoAvon.Status = Dominio.Enumeradores.StatusManifestoAvon.Emitido;
                    repManifestoAvon.Atualizar(manifestoAvon);

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "O Manifesto não possui falhas de integração, sendo assim, não é possível solicitar nenhum reenvio");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es emitidos para o documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarTodosDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int minutaAvon;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoManifesto"]), out minutaAvon);

                unitOfWork.Start();

                Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unitOfWork);
                Repositorio.DocumentoManifestoAvon repDocumentosAvon = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                Dominio.Entidades.ManifestoAvon manifestoAvon = repManifestoAvon.BuscarPorCodigo(minutaAvon);

                List<Dominio.Entidades.DocumentoManifestoAvon> documentos = repDocumentosAvon.BuscarPorManifesto(minutaAvon);
                foreach (Dominio.Entidades.DocumentoManifestoAvon docManifesto in documentos)
                {
                    docManifesto.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido;
                    docManifesto.NumeroTentativas = 0;
                    docManifesto.GuidEnvio = string.Empty;
                    repDocumentosAvon.Atualizar(docManifesto);
                }

                manifestoAvon.Status = Dominio.Enumeradores.StatusManifestoAvon.Emitido;
                repManifestoAvon.Atualizar(manifestoAvon);

                unitOfWork.CommitChanges();

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unitOfWork.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es emitidos para o documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarDocumentosRetornoAvon()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int minutaAvon, inicioRegistros;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["MinutaAvon"]), out minutaAvon);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.StatusDocumentoManifestoAvon statusDocumentoManifestoAvon = (Dominio.Enumeradores.StatusDocumentoManifestoAvon)int.Parse(Request.Params["StatusDocumentoManifestoAvon"]);

                Repositorio.DocumentoManifestoAvon repDocumentosAvon = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                List<Dominio.Entidades.DocumentoManifestoAvon> documentosManifestoAvon = repDocumentosAvon.ConsultarRetornos(minutaAvon, statusDocumentoManifestoAvon, inicioRegistros, 50);
                int countDocumentos = repDocumentosAvon.ContarConsultaRetornos(minutaAvon, statusDocumentoManifestoAvon);

                var retorno = (from documento in documentosManifestoAvon
                               select new
                               {
                                   CodigoCriptografado = Servicos.Criptografia.Criptografar(documento.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                                   documento.Codigo,
                                   Status = documento.CTe.Status,
                                   CodigoCTe = documento.CTe.Codigo,
                                   Numero = documento.CTe.Numero,
                                   DescricaoTipoCTE = documento.CTe.DescricaoTipoCTE,
                                   documento.DescricaoStatus,
                                   InformacaoAvon = documento.MesagemAvon,
                                   Valor = string.Format("{0:n2}", documento.CTe.ValorAReceber)
                               }).ToList();
                return Json(retorno, true, null, new string[] { "CodigoCriptografado", "Codigo", "Status", "CodigoCTe", "CT-e.|10", "Tipo do CT-e|15", "Situação Avon.|20", "Informação Avon.|40", "Valor|10" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es emitidos para o documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string numeroManifesto = Utilidades.String.OnlyNumbers(Request.Params["NumeroManifesto"]);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto integradoraAux;
                Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto? integradora = null;
                if (Enum.TryParse<Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto>(Request.Params["Integradora"], out integradoraAux))
                    integradora = integradoraAux;

                int inicioRegistros, numeroCTe;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);

                List<Dominio.Entidades.ManifestoAvon> manifestos = repManifesto.Consultar(this.EmpresaUsuario.Codigo, numeroManifesto, dataInicial, dataFinal, numeroCTe, inicioRegistros, 50, null, false, integradora);

                int countManifestos = repManifesto.ContarConsulta(this.EmpresaUsuario.Codigo, numeroManifesto, dataInicial, dataFinal, numeroCTe, null, false, integradora);

                var retorno = (from obj in manifestos
                               select new
                               {
                                   obj.Codigo,
                                   CodStatus = obj.Status,
                                   obj.Numero,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   ValorFrete = obj.ValorFrete.ToString("n2"),
                                   ValorPedagio = obj.ValorPedagio.ToString("n2"),
                                   //ValorAReceber = obj.ValorAReceber.ToString("n2"),
                                   Veiculo = obj.Veiculo.Placa,
                                   Status = obj.DescricaoStatus,
                                   Emissor = obj.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Manual ? "Informada Manualmente" : "Integração Avon"
                               }).ToList();

                return Json(retorno, true, "", new string[] { "Código", "CodStatus", "Número|10", "Data Emissão|18", "Valor Frete|12", "Valor Pedágio|12", "Veículo|10", "Status|15", "Tipo Minuta|15" }, countManifestos); //"Valor a Receber|15",
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os manifestos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarSumarizado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string numeroManifesto = Utilidades.String.OnlyNumbers(Request.Params["NumeroManifesto"]);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int inicioRegistros, numeroCTe;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);

                Dominio.Enumeradores.StatusManifestoAvon? status = null;
                Dominio.Enumeradores.StatusManifestoAvon statusAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusManifestoAvon>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);

                List<Dominio.Entidades.ManifestoAvon> manifestos = repManifesto.Consultar(this.EmpresaUsuario.Codigo, numeroManifesto, dataInicial, dataFinal, numeroCTe, inicioRegistros, 50, status, true);

                int countManifestos = repManifesto.ContarConsulta(this.EmpresaUsuario.Codigo, numeroManifesto, dataInicial, dataFinal, numeroCTe, status, true);

                var retorno = (from obj in manifestos
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   ValorFrete = obj.ValorFrete.ToString("n2"),
                                   ValorPedagio = obj.ValorPedagio.ToString("n2"),
                                   ValorAReceber = obj.ValorAReceber.ToString("n2")
                               }).ToList();

                return Json(retorno, true, "", new string[] { "Código", "Número|20", "Data Emissão|20", "Valor Frete|15", "Valor Pedágio|15", "Valor a Receber|20" }, countManifestos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os manifestos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoManifesto;
                int.TryParse(Request.Params["CodigoManifesto"], out codigoManifesto);

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);

                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(codigoManifesto, this.EmpresaUsuario.Codigo);

                if (manifesto == null)
                    return Json<bool>(false, false, "Manifesto não encontrado.");

                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                int countDocumentos = repDocumento.ContarPorManifesto(manifesto.Codigo);
                decimal valorCTes = repDocumento.ObterValorDosCTesPorManifesto(manifesto.Codigo);

                return Json(new
                {
                    Codigo = manifesto.Codigo,
                    QuantidadeCTe = countDocumentos,
                    ValorCTe = valorCTes,
                    Numero = manifesto.Numero
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do manifesto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult CriarMinutalManual()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão de acesso negada!");

                string numeroManifesto = Utilidades.String.OnlyNumbers(Request.Params["NumeroManifesto"]);

                int codigoVeiculo, codigoMotorista;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto tipoIntegradora = Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Manual;


                if (string.IsNullOrWhiteSpace(numeroManifesto))
                    return Json<bool>(false, false, "Número do manifesto inválido.");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.FretePorTipoDeVeiculo repFrete = new Repositorio.FretePorTipoDeVeiculo(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (veiculo == null)
                    return Json<bool>(false, false, "Veículo inválido.");

                if (motorista == null)
                    return Json<bool>(false, false, "Motorista inválido.");

                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorNumero(numeroManifesto, tipoIntegradora, this.EmpresaUsuario.Codigo);

                if (manifesto != null)
                    return Json<bool>(false, false, "Já existe um manifesto emitido com esta numeração.");


                manifesto = new Dominio.Entidades.ManifestoAvon();

                manifesto.DataEmissao = DateTime.Now;
                manifesto.Empresa = this.EmpresaUsuario;
                manifesto.Numero = numeroManifesto;
                manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.Finalizado;
                manifesto.Motorista = motorista;
                manifesto.Veiculo = veiculo;
                manifesto.TipoIntegradora = tipoIntegradora;

                repManifesto.Inserir(manifesto);


                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao emitir os CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult EmitirCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão de acesso negada!");

                string numeroManifesto = Utilidades.String.OnlyNumbers(Request.Params["NumeroManifesto"]);

                int codigoVeiculo, codigoMotorista, codigoTabelaFrete;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoTabelaFrete"], out codigoTabelaFrete);

                decimal valorFrete, valorPedagio, valorAReceber;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);
                decimal.TryParse(Request.Params["ValorAReceber"], out valorAReceber);

                Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto tipoIntegradora;
                Enum.TryParse(Request.Params["TipoIntegradora"], out tipoIntegradora);

                string groupId = Request.Params["GroupId"];


                if (string.IsNullOrWhiteSpace(numeroManifesto))
                    return Json<bool>(false, false, "Número do manifesto inválido.");

                if (valorAReceber <= 0)
                    return Json<bool>(false, false, "Valor a receber inválido.");

                if (valorFrete <= 0)
                    return Json<bool>(false, false, "Valor do frete inválido.");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.FretePorTipoDeVeiculo repFrete = new Repositorio.FretePorTipoDeVeiculo(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.FretePorTipoDeVeiculo frete = repFrete.BuscarPorCodigo(codigoTabelaFrete, this.EmpresaUsuario.Codigo);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (veiculo == null)
                    return Json<bool>(false, false, "Veículo inválido.");

                if (frete == null)
                    return Json<bool>(false, false, "Tabela de frete inválida.");

                if (motorista == null)
                    return Json<bool>(false, false, "Motorista inválido.");

                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorNumero(numeroManifesto, tipoIntegradora, this.EmpresaUsuario.Codigo);

                if (manifesto != null)
                    return Json<bool>(false, false, "Já existe um manifesto emitido com esta numeração.");

                List<Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica> nfes = null;

                Servicos.Avon svcAvon = new Servicos.Avon(unidadeDeTrabalho);
                Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

                if (tipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Avon)
                    nfes = svcAvon.ConsultarNFesParaEmissao(this.EmpresaUsuario.Codigo, numeroManifesto, groupId);
                //else if (tipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Natura)
                //    nfes = svcNatura.ConsultarNFesParaEmissao(this.EmpresaUsuario.Codigo, numeroManifesto, unidadeDeTrabalho);

                if (nfes == null || nfes.Count() <= 0)
                    return Json<bool>(false, false, "Não foram encontradas notas fiscais para o manifesto informado.");

                decimal valorDoFretePorCTe = Math.Floor((valorFrete / nfes.Count()) * 100) / 100;
                decimal valorDoFreteDoUltimoCTe = valorFrete - (valorDoFretePorCTe * (nfes.Count() - 1));
                decimal valorDoPedagioPorCTe = Math.Floor((valorPedagio / nfes.Count()) * 100) / 100;
                decimal valorDoPedagioDoUltimoCTe = valorPedagio - (valorDoPedagioPorCTe * (nfes.Count() - 1));

                unidadeDeTrabalho.Start();

                manifesto = new Dominio.Entidades.ManifestoAvon();

                manifesto.DataEmissao = DateTime.Now;
                manifesto.Empresa = this.EmpresaUsuario;
                manifesto.Numero = numeroManifesto;
                manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.Enviado;
                manifesto.ValorFrete = valorFrete;
                manifesto.ValorAReceber = valorAReceber;
                manifesto.ValorPedagio = valorPedagio;
                manifesto.TabelaFrete = frete;
                manifesto.Motorista = motorista;
                manifesto.Veiculo = veiculo;
                manifesto.TipoIntegradora = tipoIntegradora;

                repManifesto.Inserir(manifesto);

                Repositorio.DocumentoManifestoAvon repDocumentoManifesto = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

                for (int i = 0; i < nfes.Count(); i++)
                {
                    Dominio.Entidades.DocumentoManifestoAvon documentoManifesto = new Dominio.Entidades.DocumentoManifestoAvon();

                    documentoManifesto.Documento = nfes[i].Documento;
                    documentoManifesto.Manifesto = manifesto;
                    documentoManifesto.Numero = nfes[i].Numero;
                    documentoManifesto.Serie = nfes[i].Serie;
                    documentoManifesto.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado;
                    documentoManifesto.ValorFrete = valorDoFretePorCTe;
                    documentoManifesto.ValorPedagio = valorDoPedagioPorCTe;

                    if (nfes.Count() == (i + 1))
                    {
                        documentoManifesto.ValorFrete = valorDoFreteDoUltimoCTe;
                        documentoManifesto.ValorPedagio = valorDoPedagioDoUltimoCTe;
                    }

                    repDocumentoManifesto.Inserir(documentoManifesto);
                }

                if ((tipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Avon && svcAvon.FinalizarManifesto(this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.Configuracao.TokenIntegracaoAvon, numeroManifesto)) || tipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Natura)
                {
                    unidadeDeTrabalho.CommitChanges();

                    FilaEmissaoManifestoAvon.GetInstance().QueueItem(this.EmpresaUsuario.Codigo, manifesto.Codigo, Conexao.StringConexao);
                }
                else
                {
                    unidadeDeTrabalho.Rollback();

                    return Json<bool>(false, false, "Não foi possível finalizar o manifesto. Tente novamente.");
                }

                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha genérica ao emitir os CT-es.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarStatusSEFAZCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoManifesto;
                int.TryParse(Request.Params["CodigoManifesto"], out codigoManifesto);

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(codigoManifesto);

                if (manifesto == null)
                    return Json<bool>(false, false, "O manifesto não foi encontrado!");

                if (manifesto.Status != Dominio.Enumeradores.StatusManifestoAvon.Emitido)
                    return Json<bool>(false, false, "O status do manifesto não permite esta operação!");

                Repositorio.DocumentoManifestoAvon repDocumentoManifesto = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repDocumentoManifesto.BuscarCTesPorStatus(manifesto.Codigo, "Y");

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    cte.Status = "E";

                    repCTe.Atualizar(cte);

                    if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar o status dos CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDocumentosDoManifestoParaEnvio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoManifesto;
                int.TryParse(Request.Params["CodigoManifesto"], out codigoManifesto);

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);

                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(codigoManifesto);

                if (manifesto == null)
                    return Json<bool>(false, false, "O manifesto não foi encontrado!");

                if (manifesto.Status == Dominio.Enumeradores.StatusManifestoAvon.Enviado)
                    return Json<bool>(false, false, "O manifesto ainda não foi finalizado!");

                Repositorio.DocumentoManifestoAvon repDocumentoManifesto = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                List<Dominio.Entidades.DocumentoManifestoAvon> documentos = repDocumentoManifesto.BuscarPorManifesto(manifesto.Codigo);

                if (documentos == null || documentos.Count <= 0 || !(from obj in documentos where obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido || obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado select obj).Any())
                    return Json<bool>(false, false, "Não há documentos pendentes para o envio de retorno!");

                if ((from obj in documentos where obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado || obj.CTe == null || (obj.CTe.Status != "A" && obj.CTe.Status != "C") select obj).Any())
                    return Json<bool>(false, false, "Não é possível obter os documentos do manifesto, pois nem todos estão emitidos ou autorizados!");

                var retorno = (from obj in documentos where obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido select new { obj.Codigo, NumeroNFe = obj.Numero, SerieNFe = obj.Serie, NumeroCTe = obj.CTe.Numero, SerieCTe = obj.CTe.Serie.Numero }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os documentos do manifesto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarRetornoDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            List<int> codigosDocumentos = new List<int>();

            try
            {
                codigosDocumentos = JsonConvert.DeserializeObject<List<int>>(Request.Params["CodigosDocumentos"]);

                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                List<Dominio.Entidades.DocumentoManifestoAvon> documentos = repDocumento.BuscarPorCodigo(codigosDocumentos.ToArray());

                if (documentos == null || documentos.Count <= 0)
                    return Json<bool>(false, false, "Documento não encontrado.");

                if (documentos[0].Manifesto.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Avon)
                    return EnviarRetornoAvon(ref documentos, codigosDocumentos, unitOfWork);
                //else if (documentos[0].Manifesto.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Natura)
                //    return EnviarRetornoNatura(ref documentos, codigosDocumentos);
                else
                    return Json(new { Mensagem = "Retorno não implementado para a integradora.", Documentos = codigosDocumentos.ToArray() }, false);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json(new { Mensagem = "Ocorreu uma falha ao processar a requisição!", Documentos = codigosDocumentos.ToArray() }, false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarMDFeDoManifesto()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoManifesto;
                int.TryParse(Request.Params["CodigoManifesto"], out codigoManifesto);

                string numeroLacre = Request.Params["NumeroLacre"];

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
                Repositorio.DocumentoManifestoAvon repDocumentoManifesto = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(codigoManifesto, this.EmpresaUsuario.Codigo);

                if (manifesto == null)
                    return Json<bool>(false, false, "Manifesto não encontrado.");

                if (manifesto.MDFe != null && manifesto.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && manifesto.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao)
                    return Json<bool>(false, false, "Já existe um MDF-e emitido para este manifesto (" + manifesto.MDFe.Numero + " - " + manifesto.MDFe.Serie.Numero + "). Cancele o MDF-e para realizar uma nova emissão.");

                List<Dominio.Entidades.DocumentoManifestoAvon> documentos = repDocumentoManifesto.BuscarPorManifesto(manifesto.Codigo);

                if ((from obj in documentos where obj.CTe.Status != "A" || obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado select obj).Any())
                    return Json<bool>(false, false, "Não é possível emitir o MDF-e pois há CT-es que ainda não foram autorizados ou emitidos.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);
                string ufInicio = (from obj in documentos where obj.CTe.Status.Equals("A") select obj.CTe.LocalidadeInicioPrestacao.Estado.Sigla).FirstOrDefault();
                string ufFim = (from obj in documentos where obj.CTe.Status.Equals("A") select obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla).FirstOrDefault();

                unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = svcMDFe.GerarMDFePorCTes(this.EmpresaUsuario, (from obj in documentos where obj.CTe.Status.Equals("A") && obj.CTe.LocalidadeInicioPrestacao.Estado.Sigla.Equals(ufInicio) && obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla.Equals(ufFim) select obj.CTe).Take(4000).ToList(), unidadeDeTrabalho, null, null, null, new List<Dominio.ObjetosDeValor.MDFe.Lacre>() { new Dominio.ObjetosDeValor.MDFe.Lacre() { Numero = numeroLacre } });

                unidadeDeTrabalho.CommitChanges();

                if (svcMDFe.Emitir(mdfe, unidadeDeTrabalho))
                {
                    manifesto.MDFe = mdfe;

                    repManifesto.Atualizar(manifesto);

                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                }
                else
                {
                    return Json<bool>(false, false, "Ocorreu uma falha e o MDF-e não pode ser emitido. Tente novamente.");
                }

                return Json<bool>(true, true, "O MDF-e foi emitido com sucesso!");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o MDF-e do manifesto.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RemoverCTeMinuta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoManifestoAvon documento = repDocumento.BuscarPorCTe(codigoCTe);

                if (documento == null)
                    return Json<bool>(false, false, "CT-e não encontrado.");

                unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoAvon manifestoAvon = repManifestoAvon.BuscarPorCodigo(documento.Manifesto.Codigo);

                if (manifestoAvon.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Manual)
                {
                    manifestoAvon.ValorFrete += -documento.CTe.ValorFrete;
                    manifestoAvon.ValorAReceber += -documento.CTe.ValorAReceber;
                    repManifestoAvon.Atualizar(manifestoAvon);
                }

                repDocumento.Deletar(documento);

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao remover o CT-e da minuta.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        //private ActionResult EnviarRetornoNatura(ref List<Dominio.Entidades.DocumentoManifestoAvon> documentos, List<int> codigosDocumentos)
        //{
        //    Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

        //    Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

        //    svcNatura.EnviarRetornoCTe(this.EmpresaUsuario.Codigo, codigosDocumentos);

        //    Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);

        //    foreach (Dominio.Entidades.DocumentoManifestoAvon documento in documentos)
        //    {
        //        documento.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado;

        //        repDocumento.Atualizar(documento);
        //    }

        //    if (repDocumento.ContarPorManifestoEStatusDiff(documentos[0].Manifesto.Codigo, Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado) <= 0)
        //    {
        //        Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);

        //        Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(documentos[0].Manifesto.Codigo);

        //        manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.Finalizado;

        //        repManifesto.Atualizar(manifesto);
        //    }

        //    return Json(new { Mensagem = "200 - CT-e enviado.", Documentos = (from obj in documentos select obj.Codigo).ToArray() }, true);
        //}

        private ActionResult EnviarRetornoAvon(ref List<Dominio.Entidades.DocumentoManifestoAvon> documentos, List<int> codigosDocumentos, Repositorio.UnitOfWork unitOfWork)
        {
            if (documentos == null || documentos.Count <= 0)
                return Json<bool>(false, false, "Documento não encontrado.");

            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            List<Dominio.Entidades.XMLCTe> xmls = repXMLCTe.BuscarPorCTe((from obj in documentos select obj.CTe.Codigo).ToArray(), Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

            Servicos.Avon svcAvon = new Servicos.Avon(unitOfWork);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = svcAvon.EnviarRetornoCTe(this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.Configuracao.TokenIntegracaoAvon, xmls);

            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200" && retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Document != null && retorno.CrossTalk_Body.Document[0].Response != null && retorno.CrossTalk_Body.Document[0].Response.InnerCode == "100")
            {
                Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unitOfWork);

                foreach (Dominio.Entidades.DocumentoManifestoAvon documento in documentos)
                {
                    documento.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado;

                    repDocumento.Atualizar(documento);
                }

                if (repDocumento.ContarPorManifestoEStatusDiff(documentos[0].Manifesto.Codigo, Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado) <= 0)
                {
                    Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);

                    Dominio.Entidades.ManifestoAvon manifesto = repManifesto.BuscarPorCodigo(documentos[0].Manifesto.Codigo);

                    manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.Finalizado;

                    repManifesto.Atualizar(manifesto);
                }

                return Json(new { Mensagem = retorno.CrossTalk_Body.Document[0].Response.InnerCode + " - " + retorno.CrossTalk_Body.Document[0].Response.Description, Documentos = (from obj in documentos select obj.Codigo).ToArray() }, true);
            }
            else if (retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Document != null && retorno.CrossTalk_Body.Document[0].Response != null && retorno.CrossTalk_Body.Document[0].Response.ErrorMessages != null)
            {
                string msgRetorno = string.Empty;

                foreach (string error in retorno.CrossTalk_Body.Document[0].Response.ErrorMessages)
                    msgRetorno += error;

                return Json(new { Mensagem = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage, Documentos = (from obj in documentos select obj.Codigo).ToArray() }, false);
            }
            else
            {
                return Json(new { Mensagem = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage, Documentos = (from obj in documentos select obj.Codigo).ToArray() }, false);
            }
        }

        #endregion
    }
}
