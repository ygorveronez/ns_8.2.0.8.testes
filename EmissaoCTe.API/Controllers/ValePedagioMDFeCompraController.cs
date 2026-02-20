using Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ValePedagioMDFeCompraController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);

                List<Dominio.Entidades.ValePedagioMDFeCompra> listaComprasValePedagio = repValePedagioMDFeCompra.BuscarPorMDFe(codigoMDFe);

                var retorno = (from obj in listaComprasValePedagio
                               orderby obj.Codigo descending
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Integradora = obj.DescricaoIntegradora,
                                   Tipo = obj.DescricaoTipo,
                                   Status = obj.DescricaoStatus,
                                   Mensagem = !string.IsNullOrWhiteSpace(obj.Mensagem) ? obj.Mensagem : string.Empty,
                                   Comprovante = !string.IsNullOrWhiteSpace(obj.NumeroComprovante) ? obj.NumeroComprovante : string.Empty,
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Integradora|15", "Tipo|15", "Status|15", "Mensagem|20", "Comprovante|25" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de compra de vale pedágio do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarLogIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.ValePedagioMDFeCompraXML repValePedagioMDFeCompraXML = new Repositorio.ValePedagioMDFeCompraXML(unitOfWork);

                List<Dominio.Entidades.ValePedagioMDFeCompraXML> listaComprasValePedagioXML = repValePedagioMDFeCompraXML.BuscarPorValePedagioMDFeCompra(codigo);

                var retorno = (from obj in listaComprasValePedagioXML
                               orderby obj.Codigo descending
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Tipo = obj.DescricaoTipo,
                                   Data = obj.DataHora.ToString("dd/MM/yyyy HH:mm:ss")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Tipo|50", "Data Hora|20" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de log compra de vale pedágio do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);
                Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);

                Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra = repValePedagioMDFeCompra.BuscarPorCodigo(codigo);

                if (valePedagioMDFeCompra == null)
                    return Json<bool>(false, false, "Integração de compra do vale pedágio não encontrada. Atualize a página e tente novamente.");

                if (valePedagioMDFeCompra.Status != Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento &&
                    valePedagioMDFeCompra.Status != Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra)
                    return Json<bool>(false, false, "Somente é permitido reenviar uma integração com status de Rejeição.");

                if (this.EmpresaUsuario.Configuracao == null && this.EmpresaUsuario.EmpresaPai?.Configuracao == null)
                    return Json<bool>(false, false, "O transportador não possui configurações para integração.");

                if (!(this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.ValePedagioIntegraAutomatico == 1 && this.EmpresaUsuario.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma) &&
                    !(this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null && this.EmpresaUsuario.EmpresaPai.Configuracao.ValePedagioIntegraAutomatico == 1 && this.EmpresaUsuario.EmpresaPai.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma))
                    return Json<bool>(false, false, "O transportador não está configurado para integraçaõ de compra de vale pedágio.");

                Servicos.Target svcTarget = new Servicos.Target(unidadeDeTrabalho);
                Servicos.SemParar semParar = new Servicos.SemParar(unidadeDeTrabalho);


                bool sucesso = false;

                switch (valePedagioMDFeCompra.Integradora)
                {
                    case Dominio.Enumeradores.IntegradoraValePedagio.Target:
                        if (valePedagioMDFeCompra.Tipo == Dominio.Enumeradores.TipoIntegracaoValePedagio.Cancelamento)
                            sucesso = svcTarget.CancelarCompraValePedagioMDFe(ref valePedagioMDFeCompra, unidadeDeTrabalho);
                        else
                            sucesso = svcTarget.ComprarValePedagioMDFe(ref valePedagioMDFeCompra, unidadeDeTrabalho);
                        break;

                    case Dominio.Enumeradores.IntegradoraValePedagio.SemParar:
                        if (valePedagioMDFeCompra.Tipo == Dominio.Enumeradores.TipoIntegracaoValePedagio.Cancelamento)
                            sucesso = semParar.CancelarCompraValePedagioMDFe(ref valePedagioMDFeCompra, unidadeDeTrabalho);
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = semParar.Autenticar(valePedagioMDFeCompra, unidadeDeTrabalho);

                            if (!credencial.Autenticado)
                            {
                                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                                valePedagioMDFeCompra.Mensagem = credencial.Retorno;

                                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                                break;
                            }

                            sucesso = !string.IsNullOrWhiteSpace(valePedagioMDFeCompra.NumeroComprovante);

                            if (!sucesso)
                                sucesso = semParar.ComprarValePedagioMDFe(ref valePedagioMDFeCompra, credencial, unidadeDeTrabalho);

                            sucesso = !string.IsNullOrWhiteSpace(valePedagioMDFeCompra.NumeroComprovante) && valePedagioMDFeCompra.Valor <= 0;

                            if (sucesso)
                                sucesso = semParar.ObterReciboCompraValePedagio(ref valePedagioMDFeCompra, credencial, unidadeDeTrabalho);

                            if (sucesso)
                                sucesso = semParar.ConsultarIdVpo(valePedagioMDFeCompra, credencial, unidadeDeTrabalho);
                        }
                        break;
                }

                if (!sucesso)
                {
                    if (valePedagioMDFeCompra.Tipo == Dominio.Enumeradores.TipoIntegracaoValePedagio.Cancelamento)
                        return Json<bool>(false, false, "Integração de cancelamento solicitada porém sem retorno de sucesso.");

                    if (valePedagioMDFeCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                    {
                        valePedagioMDFeCompra.MDFe.MensagemRetornoSefaz = valePedagioMDFeCompra.Mensagem;
                        repMDFe.Atualizar(valePedagioMDFeCompra.MDFe);
                    }

                    return Json<bool>(false, false, "Integração solicitada porém sem retorno de sucesso.");
                }

                if (valePedagioMDFeCompra.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso)
                {
                    if (valePedagioMDFeCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                    {
                        var listaValePedagioMDFe = repValePedagioMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);
                        foreach (var valePedagio in listaValePedagioMDFe)
                        {
                            if (string.IsNullOrWhiteSpace(valePedagio.NumeroComprovante) || valePedagio.NumeroComprovante == "0")
                                repValePedagioMDFe.Deletar(valePedagio);
                        }
                    }

                    var valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe
                    {
                        MDFe = valePedagioMDFeCompra.MDFe,
                        NumeroComprovante = !string.IsNullOrEmpty(valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT) ? valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT : valePedagioMDFeCompra.NumeroComprovante,
                        ValorValePedagio = valePedagioMDFeCompra.Valor,
                        CNPJFornecedor = valePedagioMDFeCompra.CNPJFornecedor,
                        CNPJResponsavel = valePedagioMDFeCompra.CNPJResponsavel,
                        QuantidadeEixos = valePedagioMDFeCompra.QuantidadeEixos,
                        TipoCompra = valePedagioMDFeCompra.TipoCompra
                    };

                    repValePedagioMDFe.Inserir(valePedagioMDFe);
                }

                if (valePedagioMDFeCompra.MDFe.Status == Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                {
                    var listaComprasPedagioMDFe = repValePedagioMDFeCompra.BuscarPorMDFeTipo(valePedagioMDFeCompra.MDFe.Codigo, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao);
                    int quantidadePendentes = listaComprasPedagioMDFe.Count(obj => obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente);
                    int quantidadeRejeitados = listaComprasPedagioMDFe.Count(obj => obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra);

                    if (quantidadePendentes == 0 && quantidadeRejeitados == 0)
                    {
                        Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);
                        if (servicoMDFe.Emitir(valePedagioMDFeCompra.MDFe, unidadeDeTrabalho))
                            servicoMDFe.AdicionarMDFeNaFilaDeConsulta(valePedagioMDFeCompra.MDFe, unidadeDeTrabalho);
                    }
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar a integração de compra de vale pedágio.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLLog()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["Tipo"], out int tipo);

                if (codigo > 0)
                {
                    Repositorio.ValePedagioMDFeCompraXML repValePedagioMDFeCompraXML = new Repositorio.ValePedagioMDFeCompraXML(unidadeDeTrabalho);

                    Dominio.Entidades.ValePedagioMDFeCompraXML xml = repValePedagioMDFeCompraXML.BuscarPorCodigo(codigo);

                    if (xml != null)
                    {
                        byte[] data = null;
                        if (tipo == 0)
                            data = System.Text.Encoding.UTF8.GetBytes(xml.Requisicao);
                        else
                            data = System.Text.Encoding.UTF8.GetBytes(xml.Resposta);

                        if (data != null)
                        {
                            if (tipo == 0)
                                return Arquivo(data, "text/xml", string.Concat("Requisicao" + xml.Codigo, ".xml"));
                            else
                                return Arquivo(data, "text/xml", string.Concat("Resposta" + xml.Codigo, ".xml"));
                        }
                        else
                            return Json<bool>(false, false, "Ocorreu uma falha ao carregar XML, atualize a página e tente novamente.");
                    }
                    else
                        return Json<bool>(false, false, "Nenhum XML salvo.");
                }
                else
                    return Json<bool>(false, false, "Integração não encontrada, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);

                if (codigo == 0)
                    return Json<bool>(false, false, "Integração não encontrada, atualize a página e tente novamente.");

                Repositorio.ValePedagioMDFeCompra repositorioValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);

                Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra = repositorioValePedagioMDFeCompra.BuscarPorCodigo(codigo);

                if (valePedagioMDFeCompra == null)
                    return Json<bool>(false, false, "Nenhuma integração localizada.");

                if (valePedagioMDFeCompra.Status != Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso && valePedagioMDFeCompra.Tipo != Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao)
                    return Json<bool>(false, false, "Documento disponível apenas para integrações de Autorização efetuadas com sucesso.");

                else if (valePedagioMDFeCompra.Integradora == Dominio.Enumeradores.IntegradoraValePedagio.Target)
                {
                    Servicos.Target servicoTarget = new Servicos.Target(unitOfWork);

                    string retorno = string.Empty;
                    byte[] data = null;

                    bool documentoGerado = servicoTarget.ObterDocumento(valePedagioMDFeCompra, ref data, ref retorno, unitOfWork);

                    if (!documentoGerado)
                        return Json<bool>(false, false, retorno);

                    return Arquivo(data, "text/xml", string.Concat("Comprovante_" + valePedagioMDFeCompra.NumeroComprovante, ".pdf"));
                }
                else if (valePedagioMDFeCompra.Integradora == Dominio.Enumeradores.IntegradoraValePedagio.SemParar)
                {
                    Servicos.SemParar semParar = new Servicos.SemParar(unitOfWork);

                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo> dsInformacoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo>();
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoReciboPracas> pracas = new List<ImpressaoReciboPracas>();

                    Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = semParar.Autenticar(valePedagioMDFeCompra, unitOfWork);

                    if (!credencial.Autenticado)
                        return Json<bool>(false, false, credencial.Retorno);
                    Servicos.SemPararValePedagio.Recibo recibo = semParar.ObterReciboViagem(valePedagioMDFeCompra.NumeroComprovante, credencial, out string _, out string _, unitOfWork);

                    if (recibo.status != 2 && recibo.status != 0)
                        return Json<bool>(false, false, semParar.ObterMensagemRetorno(recibo.status));

                    ObterInformacoesEPracas(recibo, ref dsInformacoes, ref pracas);

                    Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                    string logo = recibo.logo.Split('.')[0];
                    string caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao", "LogoSemParar"), logo + ".png");

                    var parameters = new List<Microsoft.Reporting.WebForms.ReportParameter>()
                    {
                        new ReportParameter("CaminhoLogoValePedagio")
                    };

                    Dominio.ObjetosDeValor.Relatorios.Relatorio relatorio = svcRelatorio.GerarWeb("Relatorios/SemParar.rdlc", "PDF", parameters, ObterDataSurceRecibo(dsInformacoes), ObterSubDataSetRecibo(pracas));

                    return Arquivo(relatorio.Arquivo, relatorio.MimeType, "SemParar." + relatorio.FileNameExtension);
                }

                return Json<bool>(false, false, "Nenhuma integração localizada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter Documento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public List<ReportDataSource> ObterDataSurceRecibo(List<ImpressaoRecibo> dsInformacoes)
        {
            return new List<ReportDataSource>{
                new ReportDataSource("DataSetPrincipal", dsInformacoes)
            };
        }

        public SubreportProcessingEventHandler ObterSubDataSetRecibo(List<ImpressaoReciboPracas> pracas)
        {
            return new SubreportProcessingEventHandler((sender, e) =>
            {
                e.DataSources.Add(new ReportDataSource("DataSetPracas", pracas));
            });
        }

        private void ObterInformacoesEPracas(Servicos.SemPararValePedagio.Recibo recibo, ref List<ImpressaoRecibo> dsInformacoes, ref List<ImpressaoReciboPracas> pracas)
        {
            dsInformacoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoRecibo()
                    {
                        NumeroVale = recibo.viagem.ToString(),
                        Tipo = recibo.tipo,
                        Emissor = "Via Fácil",
                        Embarcador = recibo.nomeEmissor,
                        CNPJEmbarcador = recibo.cnpjEmissor,
                        Transportador = recibo.nomeTransp,
                        CNPJTransportador = recibo.cnpjTransp,
                        DataConfirmacao = recibo.dataCompra?.ToDateString() ?? " - ",
                        DataExpiracao = recibo.dataExp?.ToDateString() ?? " - ",
                        DataViagem = recibo.dataViagem?.ToDateString() ?? " - ",
                        VeiculoCategoria = recibo.catVeiculo,
                        Rota = recibo.nomeRota,
                        Total = (from p in recibo.pracas where p.tarifa.HasValue select p.tarifa.Value).Sum(),
                        Observacao1 = "",
                        Observacao2 = "",
                        Observacao3 = "",
                        Observacao4 = "",
                        Observacao5 = "",
                        Observacao6 = "",
                    }
                };

            pracas = (from p in recibo.pracas
                      select new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.ImpressaoReciboPracas
                      {
                          Praca = p.nomePraca,
                          Rodovia = p.nomeRodovia,
                          Concessionaria = p.nomeConcessionaria,
                          Placa = p.placa,
                          NumeroTAG = p.tag,
                          Valor = p.tarifa ?? 0
                      }).ToList();

        }
    }

}