using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "ConsultarPreCTesOcorrencia", "Download" }, "Ocorrencias/Ocorrencia")]
    public class OcorrenciaPreCTeController : BaseController
    {
		#region Construtores

		public OcorrenciaPreCTeController(Conexao conexao) : base(conexao) { }

		#endregion


        public async Task<IActionResult> ConsultarPreCTesOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                int codOcorrencia = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codOcorrencia);

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao)// se está com pendencia na emissão o sistema tente ver se autorizou os rejeitados para o fluxo andar.
                {
                    Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                    serOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, WebServiceConsultaCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, false, Auditado, WebServiceOracle, clienteMultisoftware: Cliente);
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPreCTE", false);
                grid.AdicionarCabecalho("CteEnviado", false);
                grid.AdicionarCabecalho("Núm. Enviado", "NumeroCteEnviado", 10, Models.Grid.Align.center, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissor", "Emissor", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao = "PreCTe." + propOrdenacao + ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado";

                if (propOrdenacao == "Destino")
                    propOrdenacao = "PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado";

                if (propOrdenacao == "Tomador")
                    propOrdenacao = "PreCTe.Tomador.Nome";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "ValorFrete")
                    propOrdenacao = "PreCTe.ValorAReceber";

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> complementaresInfo = repCargaCTeComplementoInfo.BuscarPreCTesPorOcorrencia(ocorrencia.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                var lista = (from obj in complementaresInfo select RetornarPreCTe(obj)).ToList();

                grid.setarQuantidadeTotal(repCargaCTeComplementoInfo.ContarPorPreCTEsOcorrencia(ocorrencia.Codigo));
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic RetornarPreCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complementoInfo)
        {
            Dominio.Entidades.Empresa emitente = ObtemEmitenteOcorrencia(complementoInfo.CargaOcorrencia);
            var retorno = new
            {
                complementoInfo.Codigo,
                CodigoPreCTE = complementoInfo.PreCTe?.Codigo ?? 0,
                NumeroCteEnviado = complementoInfo.CTe != null ? complementoInfo.CTe.Numero.ToString() : "",
                CteEnviado = complementoInfo.CTe == null ? false : true,
                Remetente = complementoInfo.PreCTe.Remetente.Nome + "(" + complementoInfo.PreCTe.Remetente.CPF_CNPJ_Formatado + ")",
                Destinatario = complementoInfo.PreCTe.Destinatario != null ? complementoInfo.PreCTe.Destinatario.Nome + "(" + complementoInfo.PreCTe.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                Emissor = emitente != null ? emitente.Descricao + "(" + emitente.CNPJ_Formatado + ")" : string.Empty,
                Tomador = complementoInfo.PreCTe.Tomador.Nome + "(" + complementoInfo.PreCTe.Tomador.CPF_CNPJ_Formatado + ")",
                Destino = complementoInfo.PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                ValorFrete = complementoInfo.PreCTe.ValorAReceber.ToString("n2"),
                DT_RowColor = complementoInfo.CTe != null ? "#dff0d8" : "#fcf8e3",
            };
            return retorno;
        }

        private Dominio.Entidades.Empresa ObtemEmitenteOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
                return ocorrencia.Emitente;
            else if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
                return ocorrencia.Carga.Empresa;
            else if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato)
                return ocorrencia.ContratoFrete?.Transportador;
            else
                return null;
        }

        public async Task<IActionResult> EnviarCTesParaPreCTe()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codOcorrencia = int.Parse(Request.Params("Ocorrencia"));

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codOcorrencia);

                //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        RetornoArquivo retornoArquivo = new RetornoArquivo();
                        retornoArquivo.nome = file.FileName;
                        retornoArquivo.processada = true;
                        retornoArquivo.mensagem = "";
                        if (extensao.Equals(".xml"))
                        {
                            try
                            {
                                unitOfWork.Start();
                                var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                                if (objCTe != null)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = BuscarCargaCTeComplementoInfo(objCTe, cargaOcorrencia, unitOfWork);
                                    if (cargaCTeComplementoInfo == null)
                                    {
                                        retornoArquivo.processada = false;
                                        retornoArquivo.mensagem = "Não foi localizada nenhuma nota compativel com os documentos informados no CT-e para essa ocorrência.";
                                        unitOfWork.Rollback();
                                    }
                                    else
                                    {
                                        Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                                        file.InputStream.Position = 0;
                                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(file.InputStream, cargaCTeComplementoInfo.PreCTe, cargaCTeComplementoInfo, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);
                                        file.InputStream.Dispose();
                                        if (retorno.Length == 0)
                                        {
                                            Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(cargaOcorrencia, unitOfWork);
                                            unitOfWork.CommitChanges();
                                        }
                                        else
                                        {
                                            retornoArquivo.processada = false;
                                            retornoArquivo.mensagem = retorno;
                                            unitOfWork.CommitChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    retornoArquivo.processada = false;
                                    retornoArquivo.mensagem = "O xml informado não é uma NF-e ou um CT-e, por favor verifique.";
                                    unitOfWork.Rollback();
                                }
                            }
                            catch (Exception)
                            {
                                unitOfWork.Rollback();
                                retornoArquivo.processada = false;
                                retornoArquivo.mensagem = "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido.";
                            }
                            finally
                            {
                                file.InputStream.Dispose();
                            }
                        }
                        else
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = "A extensão do arquivo é inválida.";
                        }
                        retornoArquivos.Add(retornoArquivo);
                    }

                    var dadosRetorno = new
                    {
                        Arquivos = retornoArquivos
                    };
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, "Enviou CT-es para Pre CT-e.", unitOfWork);
                    return new JsonpResult(dadosRetorno);
                }
                else
                {
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPreCTe = int.Parse(Request.Params("CodigoPreCTe"));
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorCodigo(codigo);
                
                Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = cargaCTeComplementoInfo.PreCTe;

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);
                    if (extensao.Equals(".xml"))
                    {
                        Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(file.InputStream, preCte, cargaCTeComplementoInfo, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);
                        file.InputStream.Dispose();
                        if (retorno.Length == 0)
                        {
                            if (Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(cargaCTeComplementoInfo.CargaOcorrencia, unitOfWork))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeComplementoInfo.CargaOcorrencia, null, "Enviou CT-e para Pre CT-e.", unitOfWork);
                                return new JsonpResult(true);
                            }
                            else
                            {
                                return new JsonpResult(RetornarPreCTe(cargaCTeComplementoInfo));
                            }
                        }
                        else
                        {
                            return new JsonpResult(false, true, retorno);
                        }
                    }
                    else
                    {
                        return new JsonpResult(false, true, "A extensão do arquivo é inválida.");
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        private Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarCargaCTeComplementoInfo(dynamic objCTe, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = null;

            if (objCTe.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)objCTe;

                Type tipoInfoCTe = cteProc.CTe.infCte.Item.GetType();
                if (tipoInfoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Item;
                    cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorChaveAnterior(ocorrencia.Codigo, infCTeComple.chave);
                }
                else
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item;

                    var infDoc = info.infDoc.Items.FirstOrDefault();
                    if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaEChaveNFE(ocorrencia.Codigo, nfe.chave);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaENumeroOutroDoc(ocorrencia.Codigo, outro.nDoc);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaENumeroOutroDoc(ocorrencia.Codigo, nf.nDoc);
                    }
                }

            }
            else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe;

                Type tipoInfoCTe = cteProc.CTe.infCte.Item.GetType();
                if (tipoInfoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Item;
                    cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorChaveAnterior(ocorrencia.Codigo, infCTeComple.chCTe);
                }
                else
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item;

                    var infDoc = info.infDoc.Items.FirstOrDefault();
                    if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaEChaveNFE(ocorrencia.Codigo, nfe.chave);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaENumeroOutroDoc(ocorrencia.Codigo, outro.nDoc);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaENumeroOutroDoc(ocorrencia.Codigo, nf.nDoc);
                    }
                }
            }
            else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe;

                Type tipoInfoCTe = cteProc.CTe.infCte.Items.FirstOrDefault()?.GetType();
                if (tipoInfoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Items.First();
                    cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorChaveAnterior(ocorrencia.Codigo, infCTeComple.chCTe);
                }
                else
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Items.FirstOrDefault();

                    var infDoc = info.infDoc.Items.FirstOrDefault();
                    if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaEChaveNFE(ocorrencia.Codigo, nfe.chave);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaENumeroOutroDoc(ocorrencia.Codigo, outro.nDoc);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                        cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPreCTePorOcorrenciaENumeroOutroDoc(ocorrencia.Codigo, nf.nDoc);
                    }
                }
            }
            return cargaCTeComplementoInfo;
        }
        
    }

    public class RetornoArquivo
    {
        public string nome { get; set; }
        public bool processada { get; set; }
        public string mensagem { get; set; }
    }
}
