using Dominio.Excecoes.Embarcador;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Carga
{
    public class PreCTe : ServicoBase
    {
        #region Atributos Private
        readonly private string _pattern = @"(?<=: )\b\S+\b|(?<=:)\S+";
        #endregion

        #region Construtores
        
        public PreCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        #endregion

        #region Metodos Publicos

        public Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe BuscarCargaPreCTe(dynamic objCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repositorioDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.Frete serFrete = new Frete(unitOfWork, tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = repositorioDocumentoDestinado.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe preCTe = new Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe();
            if (objCTe.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item;


                if (cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item1
                    || cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item2
                    || cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item3)
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[] docsAnt = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[])info.docAnt;
                    if (docsAnt != null && docsAnt.Count() > 0)
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt infDoc = docsAnt.FirstOrDefault();
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt[] idDocs = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt[])infDoc.idDocAnt;
                        if (idDocs != null && idDocs.Count() > 0)
                        {
                            MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDoc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt)idDocs.FirstOrDefault();
                            object idDocAntTipo = idDoc.Items.FirstOrDefault();

                            if (idDocAntTipo.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                            {
                                MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle ele = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)idDocAntTipo;
                                preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveCTeSub(carga?.Codigo ?? 0, ele.chave);
                            }
                        }
                    }
                }
                else
                {
                    object infDoc = info.infDoc.Items.FirstOrDefault();
                    if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveNFE(carga?.Codigo ?? 0, nfe.chave);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaENumeroOutroDoc(carga?.Codigo ?? 0, outro.nDoc);
                    }
                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaENumeroOutroDoc(carga?.Codigo ?? 0, nf.nDoc);
                    }
                }
            }
            else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item;
                    if (info != null)
                    {

                        if (cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item1
                        || cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item2
                        || cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item3
                        )
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[] docsAnt = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[])info.docAnt;
                            if (docsAnt != null && docsAnt.Count() > 0)
                            {
                                for (int i = 0; i < docsAnt.Length; i++)
                                {
                                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt infDoc = docsAnt[i];
                                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt[] idDocs = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt[])infDoc.idDocAnt;
                                    if (idDocs != null && idDocs.Count() > 0)
                                    {
                                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDoc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt)idDocs.FirstOrDefault();
                                        object idDocAntTipo = idDoc.Items.FirstOrDefault();

                                        if (idDocAntTipo != null)
                                        {
                                            if (idDocAntTipo.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                                            {
                                                MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle ele = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)idDocAntTipo;
                                                if (ele != null)
                                                    preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveCTeSub(carga?.Codigo ?? 0, ele.chCTe);

                                                if (preCTe.CargaCTe == null)
                                                {
                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorChave(ele.chCTe);
                                                    if (pedidoCTeParaSubContratacao != null)
                                                    {
                                                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cteProc.CTe.infCte.emit.CNPJ);
                                                        if (empresa != null)
                                                        {
                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(pedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo);
                                                            pedidoCTeParaSubContratacao.CargaPedido.Carga.Empresa = empresa;
                                                            serFrete.DefinirEtapaEFreteCargas(pedidoCTeParaSubContratacao.CargaPedido.Carga, cargaPedidos, unitOfWork);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (preCTe.CargaCTe != null)
                                            break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (info.infDoc != null)
                            {
                                for (int i = 0; i < info.infDoc.Items.Length; i++)
                                {
                                    object infDoc = info.infDoc.Items[i];
                                    if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                                    {
                                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveNFE(carga?.Codigo ?? 0, nfe.chave);
                                    }
                                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                                    {
                                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaENumeroOutroDoc(carga?.Codigo ?? 0, outro.nDoc);
                                    }
                                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                                    {
                                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaENumeroOutroDoc(carga?.Codigo ?? 0, nf.nDoc);
                                    }

                                    if (preCTe.CargaCTe != null)
                                        break;
                                }
                            }
                        }
                    }
                }
                else if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp compl = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Item;

                    if (configuracaoDocumentoDestinado?.VincularCteNaOcorrenciaApartirDaObservacao ?? false)
                    {
                        MatchCollection matches = Regex.Matches(compl.xObs, _pattern);
                        int numeroOcorrencia = 0;

                        foreach (Match match in matches)
                        {
                            int.TryParse(match.Value, out numeroOcorrencia);
                            break;
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorChaveCTesAnterior(compl.chCTe);
                        preCTe.CargaCTeComplementoInfo = cargaCTeComplementoInfos.Where(x => x.CargaOcorrencia != null && x.CargaOcorrencia.NumeroOcorrencia == numeroOcorrencia).FirstOrDefault();

                        return preCTe;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorChaveCTeAnterior(compl.chCTe);

                    if (cargaCTeComplementoInfo?.CargaOcorrencia != null)
                    {
                        preCTe.CargaCTeComplementoInfo = cargaCTeComplementoInfo;
                    }
                }
            }
            else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                if (cteProc.CTe.infCte.Items.FirstOrDefault()?.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Items.First();
                    if (info != null)
                    {

                        if (cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item1
                        || cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item2
                        || cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item3
                        )
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[] docsAnt = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[])info.docAnt;
                            if (docsAnt != null && docsAnt.Count() > 0)
                            {
                                for (int i = 0; i < docsAnt.Length; i++)
                                {
                                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt infDoc = docsAnt[i];
                                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt[] idDocs = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt[])infDoc.idDocAnt;
                                    if (idDocs != null && idDocs.Count() > 0)
                                    {
                                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDoc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt)idDocs.FirstOrDefault();
                                        object idDocAntTipo = idDoc.Items.FirstOrDefault();

                                        if (idDocAntTipo != null)
                                        {
                                            if (idDocAntTipo.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                                            {
                                                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle ele = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)idDocAntTipo;
                                                if (ele != null)
                                                    preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveCTeSub(carga?.Codigo ?? 0, ele.chCTe, cteProc.CTe.infCte.emit.Item);

                                                if (preCTe.CargaCTe == null)
                                                {
                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorChave(ele.chCTe);
                                                    if (pedidoCTeParaSubContratacao != null)
                                                    {
                                                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cteProc.CTe.infCte.emit.Item);
                                                        if (empresa != null)
                                                        {
                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(pedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo);
                                                            pedidoCTeParaSubContratacao.CargaPedido.Carga.Empresa = empresa;
                                                            serFrete.DefinirEtapaEFreteCargas(pedidoCTeParaSubContratacao.CargaPedido.Carga, cargaPedidos, unitOfWork);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (preCTe.CargaCTe != null)
                                            break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (info.infDoc != null)
                            {
                                for (int i = 0; i < info.infDoc.Items.Length; i++)
                                {
                                    object infDoc = info.infDoc.Items[i];
                                    if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                                    {
                                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveNFE(carga?.Codigo ?? 0, nfe.chave, cteProc.CTe.infCte.emit.Item);
                                    }
                                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                                    {
                                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaENumeroOutroDoc(carga?.Codigo ?? 0, outro.nDoc, cteProc.CTe.infCte.emit.Item);
                                    }
                                    else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                                    {
                                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                                        preCTe.CargaCTe = repCargaCTe.BuscarPreCTePorCargaENumeroOutroDoc(carga?.Codigo ?? 0, nf.nDoc, cteProc.CTe.infCte.emit.Item);
                                    }

                                    if (preCTe.CargaCTe != null)
                                        break;
                                }
                            }
                            else if (info.infServVinc != null)
                            {
                                if (cteProc.CTe.infCte.ide.tpServ == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item4)
                                {
                                    foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCTeMultimodal ch in info.infServVinc)
                                    {
                                        if (ch.chCTeMultimodal != null)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.CargaCTe preCTeCargaCTe = repCargaCTe.BuscarPreCTePorCargaEChaveCTeSub(carga?.Codigo ?? 0, ch.chCTeMultimodal, cteProc.CTe.infCte.emit.Item);
                                            if (preCTeCargaCTe != null)
                                            {
                                                preCTe.CargaCTe = preCTeCargaCTe;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (cteProc.CTe.infCte.Items.FirstOrDefault()?.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp compl = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Items.First();

                    //não tem mais essa xObs no 4.00...
                    //if (configuracaoDocumentoDestinado?.VincularCteNaOcorrenciaApartirDaObservacao ?? false)
                    //{
                    //    MatchCollection matches = Regex.Matches(compl.xObs, _pattern);
                    //    int numeroOcorrencia = 0;

                    //    foreach (Match match in matches)
                    //    {
                    //        int.TryParse(match.Value, out numeroOcorrencia);
                    //        break;
                    //    }

                    //    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorChaveCTesAnterior(compl.chCTe);
                    //    preCTe.CargaCTeComplementoInfo = cargaCTeComplementoInfos.Where(x => x.CargaOcorrencia != null && x.CargaOcorrencia.NumeroOcorrencia == numeroOcorrencia).FirstOrDefault();

                    //    return preCTe;
                    //}

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorChaveCTeAnterior(compl.chCTe);

                    if (cargaCTeComplementoInfo?.CargaOcorrencia != null)
                    {
                        preCTe.CargaCTeComplementoInfo = cargaCTeComplementoInfo;
                    }
                }
            }
            return preCTe;
        }

        public string ValidarPreCte(Stream stCTe, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            System.Text.StringBuilder stBuild = new StringBuilder();
            Servicos.Embarcador.CTe.CTe serCTe = new Embarcador.CTe.CTe(unitOfWork);

            object cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(stCTe);

            if (cteLido.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
            {

                try
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                    validarParticipantes(ref stBuild, cteProc, preCTe, configuracao);

                    validarComponentes(ref stBuild, cteProc, preCTe, unitOfWork, configuracao);

                    validarImpostos(ref stBuild, cteProc, preCTe, configuracao);

                    validarInfo(ref stBuild, cteProc, preCTe, configuracao, unitOfWork);

                    if (string.IsNullOrEmpty(stBuild.ToString()))
                    {
                        cteIntegracao = serCTe.ConverterProcCTeParaCTe(cteProc);
                        stCTe.Position = 0;
                        StreamReader reader = new StreamReader(stCTe);
                        cteIntegracao.Xml = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    stBuild.Append("Falha ao processa o XML do cte");
                }
            }
            else
            {
                stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
            }

            return stBuild.ToString();
        }

        public static bool EnviarCTeParaComplementoInfo(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int numeroOcorrencia, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = "";
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Servicos.Embarcador.Carga.PreCTe svcPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorNumeroOcorrenciaEValor(numeroOcorrencia, cte.ValorAReceber);

            if (cargaCTeComplementoInfo != null)
            {
                if (cargaCTeComplementoInfo.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada)
                {
                    if (cargaCTeComplementoInfo.CTe == null)
                    {
                        cargaCTeComplementoInfo.CTe = cte;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                        {
                            cargaCTe.CTe = cte;
                            svcPreCTe.VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                            svcPreCTe.VincularConfiguracaoContabil(cte, cargaCTe.PreCTe, unitOfWork);

                            repCargaCTe.Atualizar(cargaCTe);
                        }
                        repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);
                    }
                    else
                    {
                        erro = "Um CT-e já foi importado para esse pré CT-e, por favor verifique";
                        return false;
                    }
                }
                else
                {
                    erro = "Você não pode mandar os arquivo em uma ocorrência já cancelada, por favor, mande na viagem correta!";
                    return false;
                }

            }
            else
            {
                erro = "A ocorrência não foi localizada para esse pré CT-e";
                return false;
            }

            return true;
        }

        public static bool VerificarEnviouTodosPreDocumentos(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Servicos.Embarcador.Hubs.Ocorrencia serHubOcorrencia = new Servicos.Embarcador.Hubs.Ocorrencia();
            Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado serIntegracaoEnvioProgramado = new Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);

            bool processouTodos = false;
            int naoEnviados = repCargaOcorrencia.ContarCargaPreCTePorNaoEnviados(cargaOcorrencia.Codigo);

            if (naoEnviados == 0)
            {
                cargaOcorrencia.AgImportacaoCTe = false;
                repCargaOcorrencia.Atualizar(cargaOcorrencia);
                processouTodos = true;

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMars = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars);

                if (tipoIntegracaoMars != null)
                    serIntegracaoEnvioProgramado.AdicionarIntegracaoProgramadaOcorrencia(cargaOcorrencia, tipoIntegracaoMars);

                serHubOcorrencia.InformarOcorrenciaAtualizada(cargaOcorrencia.Codigo);
            }

            return processouTodos;
        }

        public string EnviarXMLCTeDoPreCTe(Stream stCTe, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            System.Text.StringBuilder stBuild = new StringBuilder();

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (cargaCTeComplementoInfo != null)
            {
                if (cargaCTeComplementoInfo.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada)
                {
                    if (cargaCTeComplementoInfo.CTe == null)
                    {

                        object cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(stCTe);

                        if (cteLido != null && cteLido.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                        {

                            try
                            {
                                MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                //Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(preCTe.Tomador.CPF_CNPJ));

                                validarParticipantes(ref stBuild, cteProc, preCTe, configuracao);

                                //if (tomador == null || !tomador.NaoValidarValoresCTeImportadoQuandoTomador)
                                //{
                                validarComponentes(ref stBuild, cteProc, preCTe, unitOfWork, configuracao);
                                validarImpostos(ref stBuild, cteProc, preCTe, configuracao);
                                //}

                                validarInfo(ref stBuild, cteProc, preCTe, configuracao, unitOfWork);

                                if (string.IsNullOrEmpty(stBuild.ToString()))
                                {

                                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                    if (cteProc.protCTe == null)
                                    {
                                        stBuild.Append("O xml do CT-e enviado não possui a tag protCTe que contem os campos de autorização do mesmo, ou seja, ele não é válido, por favor verifique.");
                                        return null;
                                    }

                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteExiste = repCTe.BuscarPorChave(cteProc.protCTe.infProt.chCTe);
                                    object retorno = null;
                                    if (cteExiste == null)
                                        retorno = svcCTe.GerarCTeAnterior(stCTe, preCTe.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false);
                                    else
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfoExite = repCargaCTeComplementoInfo.BuscarPorCTe(cteExiste.Codigo);
                                        if (cargaCTeComplementoInfoExite == null || cargaCTeComplementoInfoExite.CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada)
                                            retorno = cteExiste;
                                        else
                                        {
                                            stBuild.Append("O ct-e " + cteExiste.Numero.ToString() + " já foi vinculado para a ocorrência " + cargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia);
                                            return null;
                                        }
                                    }


                                    if (retorno != null)
                                    {
                                        if (retorno.GetType() == typeof(string))
                                        {
                                            stBuild.Append((string)retorno);
                                        }
                                        else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteExiste != null)
                                        {
                                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                                            if (cteExiste == null)
                                                cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                            else
                                                cte = cteExiste;

                                            cargaCTeComplementoInfo.CTe = cte;

                                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);

                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                                            {
                                                cargaCTe.CTe = cte;
                                                VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                                                VincularConfiguracaoContabil(cte, cargaCTe.PreCTe, unitOfWork);

                                                repCargaCTe.Atualizar(cargaCTe);
                                            }
                                            repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);
                                        }
                                        else
                                        {
                                            stBuild.Append("Conhecimento de transporte inválido.");
                                        }
                                    }
                                    else
                                    {
                                        stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                stBuild.Append("Falha ao processar o XML do cte");
                            }
                        }
                        else if (cteLido != null && cteLido.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            try
                            {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Todos;

                                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork).BuscarPrimeiroRegistro();

                                validarParticipantes(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, configuracao);
                                validarComponentes(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, unitOfWork, configuracao, configuracaoCargaEmissaoDocumento);

                                if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                                    validarImpostos(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, configuracao);

                                validarInfo(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, configuracao, unitOfWork);

                                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                if (cteProc.protCTe == null)
                                {
                                    stBuild.Append("O xml do CT-e enviado não possui a tag protCTe que contem os campos de autorização do mesmo, ou seja, ele não é válido, por favor verifique.");
                                    return null;
                                }

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteExiste = repCTe.BuscarPorChave(cteProc.protCTe.infProt.chCTe);
                                object retorno = null;
                                if (cteExiste == null)
                                    retorno = svcCTe.GerarCTeAnterior(stCTe, preCTe.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false);
                                else
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeExite = repCargaCTe.BuscarPorCTe(cteExiste.Codigo);
                                    if (cargaCTeExite == null || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                                        retorno = cteExiste;
                                    else
                                    {
                                        stBuild.Append("O ct-e " + cteExiste.Numero.ToString() + " já foi vinculado para a ocorrência " + cargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia);
                                        return null;
                                    }
                                }
                                if (retorno != null)
                                {
                                    if (retorno.GetType() == typeof(string))
                                    {
                                        stBuild.Append((string)retorno);
                                    }
                                    else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteExiste != null)
                                    {
                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                                        if (cteExiste == null)
                                            cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                        else
                                            cte = cteExiste;

                                        if (configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                                        {
                                            cte.ValorFrete = BuscarFreteLiquidoComponente(cteProc);
                                            repCTe.Atualizar(cte);
                                        }

                                        if (string.IsNullOrEmpty(stBuild.ToString()))
                                        {
                                            cargaCTeComplementoInfo.CTe = cte;

                                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);

                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                                            {
                                                cargaCTe.CTe = cte;
                                                VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                                                VincularConfiguracaoContabil(cte, cargaCTe.PreCTe, unitOfWork);
                                                repCargaCTe.Atualizar(cargaCTe);
                                            }
                                            repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                                            if (cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ObrigatorioAprovarCtesImportados ?? false)
                                                new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria, tipoServicoMultisoftware, cargaCTeComplementoInfo);
                                        }
                                        else
                                        {
                                            new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, motivoInconsistencia, tipoServicoMultisoftware, cargaCTeComplementoInfo);
                                        }
                                    }
                                    else
                                    {
                                        stBuild.Append("Conhecimento de transporte inválido.");
                                    }
                                }
                                else
                                {
                                    stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                                }
                            }
                            catch (ServicoException ex)
                            {
                                stBuild.Append(ex.Message);
                                return null;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                stBuild.Append("Falha ao processar o XML do cte");
                            }
                        }
                        else if (cteLido != null && cteLido.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            try
                            {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Todos;

                                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork).BuscarPrimeiroRegistro();

                                validarParticipantes(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, configuracao);
                                validarComponentes(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, unitOfWork, configuracao, configuracaoCargaEmissaoDocumento);

                                if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                                    validarImpostos(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, configuracao);

                                validarInfo(ref stBuild, cteProc, ref motivoInconsistencia, preCTe, configuracao, unitOfWork);

                                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                if (cteProc.protCTe == null)
                                {
                                    stBuild.Append("O xml do CT-e enviado não possui a tag protCTe que contem os campos de autorização do mesmo, ou seja, ele não é válido, por favor verifique.");
                                    return null;
                                }

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteExiste = repCTe.BuscarPorChave(cteProc.protCTe.infProt.chCTe);
                                object retorno = null;
                                if (cteExiste == null)
                                    retorno = svcCTe.GerarCTeAnterior(stCTe, preCTe.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false);
                                else
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeExite = repCargaCTe.BuscarPorCTe(cteExiste.Codigo);
                                    if (cargaCTeExite == null || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                                        retorno = cteExiste;
                                    else
                                    {
                                        stBuild.Append("O ct-e " + cteExiste.Numero.ToString() + " já foi vinculado para a ocorrência " + cargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia);
                                        return null;
                                    }
                                }
                                if (retorno != null)
                                {
                                    if (retorno.GetType() == typeof(string))
                                    {
                                        stBuild.Append((string)retorno);
                                    }
                                    else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteExiste != null)
                                    {
                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                                        if (cteExiste == null)
                                            cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                        else
                                            cte = cteExiste;

                                        if (configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                                        {
                                            cte.ValorFrete = BuscarFreteLiquidoComponente(cteProc);
                                            repCTe.Atualizar(cte);
                                        }

                                        if (string.IsNullOrEmpty(stBuild.ToString()))
                                        {
                                            cargaCTeComplementoInfo.CTe = cte;

                                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);

                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                                            {
                                                cargaCTe.CTe = cte;
                                                VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                                                VincularConfiguracaoContabil(cte, cargaCTe.PreCTe, unitOfWork);
                                                repCargaCTe.Atualizar(cargaCTe);
                                            }
                                            repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                                            if (cargaCTeComplementoInfo.CargaOcorrencia?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ObrigatorioAprovarCtesImportados ?? false)
                                                new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria, tipoServicoMultisoftware, cargaCTeComplementoInfo);
                                        }
                                        else
                                        {
                                            new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, motivoInconsistencia, tipoServicoMultisoftware, cargaCTeComplementoInfo);
                                        }
                                    }
                                    else
                                    {
                                        stBuild.Append("Conhecimento de transporte inválido.");
                                    }
                                }
                                else
                                {
                                    stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                                }
                            }
                            catch (ServicoException ex)
                            {
                                stBuild.Append(ex.Message);
                                return null;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                stBuild.Append("Falha ao processar o XML do cte");
                            }
                        }
                        else
                        {
                            stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                        }
                    }
                    else
                    {
                        //stBuild.Append("Um CT-e já foi importado para esse pré CT-e, por favor verifique");
                        stBuild.Append("");
                    }
                }
                else
                {
                    stBuild.Append("Você não pode mandar os arquivo em uma ocorrência já cancelada, por favor, mande na viagem correta!");
                }

            }
            else
            {
                stBuild.Append("A ocorrência não foi localizada para esse pré CT-e");
            }

            return stBuild.ToString();
        }

        public static void VerificarSeLiberaCargaSemIntegrarCTes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.AgImportacaoCTe && (carga.TipoOperacao?.NaoAguardarImportacaoDoCTeParaAvancar ?? false))
            {
                carga.AgImportacaoCTe = false;
                carga.LiberadaSemTodosPreCTes = true;
            }
        }

        public bool VerificarEnviouTodosDocumentos(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool ajustarValorFreteAprovacaoManual = false)
        {
            bool processouTodos = false;
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Servicos.Embarcador.Carga.RateioFrete serRateio = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoCTe serIntegracaoCte = new Servicos.Embarcador.Integracao.IntegracaoCTe(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado serIntegracaoEnvioProgramado = new Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            int naoEnviados = repCargaCTe.ContarCargaPreCTePorNaoEnviados(carga.Codigo);
            if (naoEnviados == 0 || ajustarValorFreteAprovacaoManual)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from obj in cargaCTes select obj.CTe).Distinct().ToList();
                List<int> codigosCargaPedido = new List<int>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosNotas = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscalsRateados = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

                if (configuracaoGeralCarga?.AjustarValorFreteAposAprovacaoPreCTe ?? false)
                {
                    if (ajustarValorFreteAprovacaoManual)
                    {
                        ctes = ctes.Where(cte => cte != null && cte.Status == "A").ToList();
                        cargaCTes = cargaCTes.Where(cargaCTe => cargaCTe.CTe != null && cargaCTe.CTe.Status == "A").ToList();
                    }

                    decimal valorReceber = ctes.Sum(obj => obj.ValorAReceber);
                    decimal valorICMS = ctes.Sum(obj => obj.ValorICMS);
                    if (valorReceber != carga.ValorTotalAReceberComICMSeISS || carga.ValorICMS != valorICMS)
                    {
                        repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, false);
                        serRateio.ZerarValoresDaCarga(carga, false, unitOfWork);

                        repCargaPedidoComponenteFrete.DeletarPorCarga(carga.Codigo, false);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTesCarga = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCarga(carga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargaPedidoXMLNotaFiscalCTesCarga
                                                                                                  where obj.CargaCTe.Codigo == cargaCTe.Codigo
                                                                                                  && obj.PedidoXMLNotaFiscal.CargaPedido.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                                                                                                  && obj.PedidoXMLNotaFiscal.CargaPedido.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual
                                                                                                  select obj.PedidoXMLNotaFiscal.CargaPedido
                                                                                                  ).Distinct().ToList();

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscalsCTe = (from obj in cargaPedidoXMLNotaFiscalCTesCarga
                                                                                                                      where
                                                                                                                      obj.CargaCTe.Codigo == cargaCTe.Codigo
                                                                                                                        && (obj.PedidoXMLNotaFiscal.CargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                                                                                                                           || obj.PedidoXMLNotaFiscal.CargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                                                                                                                      select obj.PedidoXMLNotaFiscal).Distinct().ToList();


                            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = cargaPedidos.LastOrDefault();
                            decimal totalValorFrete = 0;
                            decimal totalValorFreteAPagar = 0;
                            decimal totalValorICMS = 0;
                            decimal totalBaseCalculo = 0;

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                            {
                                cargaPedido.ValorFrete = cargaCTe.CTe.ValorFrete / cargaPedidos.Count();
                                cargaPedido.ValorFreteAPagar = cargaCTe.CTe.ValorAReceber / cargaPedidos.Count();
                                cargaPedido.ValorICMS = cargaCTe.CTe.ValorICMS / cargaPedidos.Count();
                                cargaPedido.BaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS / cargaPedidos.Count();
                                cargaPedido.CST = cargaCTe.CTe.CST;
                                cargaPedido.PercentualAliquota = cargaCTe.CTe.AliquotaICMS;
                                cargaPedido.CFOP = cargaCTe.CTe.CFOP;
                                cargaPedido.ImpostoInformadoPeloEmbarcador = true;

                                totalValorFrete += cargaPedido.ValorFrete;
                                totalValorFreteAPagar += cargaPedido.ValorFreteAPagar;
                                totalValorICMS += cargaPedido.ValorICMS;
                                totalBaseCalculo += cargaPedido.BaseCalculoICMS;

                                if (cargaPedido == ultimaCargaPedido)
                                {
                                    cargaPedido.ValorFrete += cargaCTe.CTe.ValorFrete - totalValorFrete;
                                    cargaPedido.ValorFreteAPagar += cargaCTe.CTe.ValorAReceber - totalValorFreteAPagar;
                                    cargaPedido.ValorICMS += cargaCTe.CTe.ValorICMS - totalValorICMS;
                                    cargaPedido.BaseCalculoICMS += cargaCTe.CTe.BaseCalculoICMS - totalBaseCalculo;
                                }

                                if (cargaPedido.ValorFrete == cargaPedido.BaseCalculoICMS && cargaPedido.CST != "60")
                                    cargaPedido.IncluirICMSBaseCalculo = false;

                                repCargaPedido.Atualizar(cargaPedido);
                                codigosCargaPedido.Add(cargaPedido.Codigo);
                            }

                            decimal totalValorFreteNF = 0;
                            decimal totalValorICMSNF = 0;
                            decimal totalBaseCalculoNF = 0;
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXMLNotaFiscal = pedidoXMLNotaFiscalsCTe.LastOrDefault();
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscalsCTe)
                            {
                                pedidoXMLNotaFiscal.ValorFrete = cargaCTe.CTe.ValorFrete / pedidoXMLNotaFiscalsCTe.Count();
                                pedidoXMLNotaFiscal.ValorICMS = cargaCTe.CTe.ValorICMS / pedidoXMLNotaFiscalsCTe.Count();
                                pedidoXMLNotaFiscal.BaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS / pedidoXMLNotaFiscalsCTe.Count();
                                pedidoXMLNotaFiscal.CST = cargaCTe.CTe.CST;
                                pedidoXMLNotaFiscal.PercentualAliquota = cargaCTe.CTe.AliquotaICMS;
                                pedidoXMLNotaFiscal.CFOP = cargaCTe.CTe.CFOP;

                                if (pedidoXMLNotaFiscal.CST != "60" && (pedidoXMLNotaFiscal.ValorFrete + pedidoXMLNotaFiscal.ValorICMS) < pedidoXMLNotaFiscal.BaseCalculoICMS)
                                    pedidoXMLNotaFiscal.ValorTotalComponentes = pedidoXMLNotaFiscal.BaseCalculoICMS - (pedidoXMLNotaFiscal.ValorFrete + pedidoXMLNotaFiscal.ValorICMS);

                                totalValorFreteNF += pedidoXMLNotaFiscal.ValorFrete;
                                totalValorICMSNF += pedidoXMLNotaFiscal.ValorICMS;
                                totalBaseCalculoNF += pedidoXMLNotaFiscal.BaseCalculoICMS;

                                if (pedidoXMLNotaFiscal == ultimoPedidoXMLNotaFiscal)
                                {
                                    pedidoXMLNotaFiscal.ValorFrete += cargaCTe.CTe.ValorFrete - totalValorFreteNF;
                                    pedidoXMLNotaFiscal.ValorICMS += cargaCTe.CTe.ValorICMS - totalValorICMSNF;
                                    pedidoXMLNotaFiscal.BaseCalculoICMS += cargaCTe.CTe.BaseCalculoICMS - totalBaseCalculoNF;
                                }

                                if ((pedidoXMLNotaFiscal.ValorFrete + pedidoXMLNotaFiscal.ValorTotalComponentes) == pedidoXMLNotaFiscal.BaseCalculoICMS && pedidoXMLNotaFiscal.CST != "60")
                                    pedidoXMLNotaFiscal.IncluirICMSBaseCalculo = false;

                                repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                                if (!cargaPedidosNotas.Contains(pedidoXMLNotaFiscal.CargaPedido))
                                    cargaPedidosNotas.Add(pedidoXMLNotaFiscal.CargaPedido);

                                pedidoXMLNotaFiscalsRateados.Add(pedidoXMLNotaFiscal);
                            }

                            carga.ValorFrete += cargaCTe.CTe.ValorFrete;
                            carga.ValorICMS += cargaCTe.CTe.ValorICMS;
                            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                        }

                        if (codigosCargaPedido.Count > 0)
                        {
                            RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRateio = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                            foreach (int codigoCargaPedido in codigosCargaPedido)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoUtilizar = (from obj in cargaPedidos where obj.Codigo == codigoCargaPedido select obj).FirstOrDefault();
                                if (cargaPedidoUtilizar != null)
                                    cargaPedidosRateio.Add(cargaPedidoUtilizar);
                            }
                            serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidosRateio, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>(), false, tipoServicoMultisoftware, unitOfWork, configuracao);
                        }

                        if (cargaPedidosNotas.Count > 0)
                        {

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosNotas)
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = (from obj in pedidoXMLNotaFiscalsRateados where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

                                cargaPedido.ValorFrete = pedidoXMLNotaFiscals.Sum(obj => obj.ValorFrete + obj.ValorTotalComponentes);
                                cargaPedido.IncluirICMSBaseCalculo = pedidoXMLNotaFiscals.FirstOrDefault().IncluirICMSBaseCalculo;
                                cargaPedido.ValorFreteAPagar = cargaPedido.ValorFrete;
                                cargaPedido.ValorICMS = pedidoXMLNotaFiscals.Sum(obj => obj.ValorICMS);
                                if (cargaPedido.IncluirICMSBaseCalculo)
                                    cargaPedido.ValorFreteAPagar += cargaPedido.ValorICMS;

                                cargaPedido.BaseCalculoICMS = pedidoXMLNotaFiscals.Sum(obj => obj.BaseCalculoICMS);
                                cargaPedido.CST = pedidoXMLNotaFiscals.FirstOrDefault().CST;
                                cargaPedido.PercentualAliquota = pedidoXMLNotaFiscals.FirstOrDefault().PercentualAliquota;
                                cargaPedido.CFOP = pedidoXMLNotaFiscals.FirstOrDefault().CFOP;
                                cargaPedido.ImpostoInformadoPeloEmbarcador = true;

                                repCargaPedido.Atualizar(cargaPedido);
                            }
                        }
                    }

                    if (carga.LiberadaSemTodosPreCTes)
                    {
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;
                        carga.FinalizandoProcessoEmissao = true;
                        carga.GerouControleFaturamento = false;
                    }
                }

                if (naoEnviados == 0)
                {
                    carga.AgImportacaoCTe = false;
                    carga.LiberadaSemTodosPreCTes = false;
                    repCarga.Atualizar(carga);
                    processouTodos = true;

                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMars = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars);

                    if (tipoIntegracaoMars != null)
                        serIntegracaoEnvioProgramado.AdicionarCTesLoteParaIntegracao(serIntegracaoCte.BuscarCtesPorCarga(carga.Codigo, unitOfWork), tipoIntegracaoMars, carga);
                }

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
            }
            return processouTodos;
        }

        public void SetarDocumentoOriginario(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoDeTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repCTeDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);

            List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosDeTransporteAnteriorCTe = repDocumentoDeTransporteAnteriorCTe.BuscarPorCTe(cte.Codigo);
            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoDeTransporteAnterior in documentosDeTransporteAnteriorCTe)
            {
                int numero = 0;
                int.TryParse(documentoDeTransporteAnterior.Numero, out numero);
                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                {
                    CTe = cte,
                    Chave = documentoDeTransporteAnterior.Chave,
                    Numero = numero,
                    Serie = documentoDeTransporteAnterior.Serie,
                    DataEmissao = documentoDeTransporteAnterior.DataEmissao
                };
                repCTeDocumentoOriginario.Inserir(documentoOriginario);
            }
        }

        public string EnviarXMLCTeDoPreCTe(Stream stCTe, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool substituirCTe = false)
        {
            System.Text.StringBuilder stBuild = new StringBuilder();
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (cargaCTe != null)
            {
                if (cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                {
                    if (cargaCTe.CTe == null || substituirCTe)
                    {
                        object cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(stCTe);

                        if (cteLido != null && cteLido.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            try
                            {
                                MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                                ValidarCTeV200(cteProc, preCTe, stBuild, configuracao, unitOfWork);

                                if (string.IsNullOrEmpty(stBuild.ToString()))
                                {

                                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                    if (cteProc.protCTe == null)
                                    {
                                        stBuild.Append("O xml do CT-e enviado não possui a tag protCTe que contem os campos de autorização do mesmo, ou seja, ele não é válido, por favor verifique.");
                                        return stBuild.ToString();
                                    }

                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteExiste = repCTe.BuscarPorChave(cteProc.protCTe.infProt.chCTe);
                                    object retorno = null;
                                    if (cteExiste == null)
                                        retorno = svcCTe.GerarCTeAnterior(stCTe, preCTe.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false, null, true);
                                    else
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeExite = repCargaCTe.BuscarPorCTe(cteExiste.Codigo);
                                        if (cargaCTeExite == null || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                                            retorno = cteExiste;
                                        else
                                        {
                                            stBuild.Append("O ct-e " + cteExiste.Numero.ToString() + " já foi vinculado a carga " + cargaCTe.Carga.CodigoCargaEmbarcador);
                                            return stBuild.ToString();
                                        }
                                    }


                                    if (retorno != null)
                                    {
                                        if (retorno.GetType() == typeof(string))
                                        {
                                            stBuild.Append((string)retorno);
                                        }
                                        else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteExiste != null)
                                        {
                                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                                            if (cteExiste == null)
                                                cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                            else
                                                cte = cteExiste;

                                            cargaCTe.CTe = cte;
                                            VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                                            VincularConfiguracaoContabil(cte, preCTe, unitOfWork);
                                            repCargaCTe.Atualizar(cargaCTe);
                                        }
                                        else
                                        {
                                            stBuild.Append("Conhecimento de transporte inválido.");
                                        }
                                    }
                                    else
                                    {
                                        stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                stBuild.Append("Falha ao processar o XML do cte");
                            }
                        }
                        else if (cteLido != null && cteLido.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            try
                            {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Todos;
                                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork).BuscarPrimeiroRegistro();

                                ValidarCTeV300(cteProc, preCTe, stBuild, ref motivoInconsistencia, configuracao, configuracaoCargaEmissaoDocumento, unitOfWork);

                                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                if (cteProc.protCTe == null)
                                {
                                    stBuild.Append("O xml do CT-e enviado não possui a tag protCTe que contem os campos de autorização do mesmo, ou seja, ele não é válido, por favor verifique.");
                                    return stBuild.ToString();
                                }

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteExiste = repCTe.BuscarPorChave(cteProc.protCTe.infProt.chCTe);
                                object retorno = null;
                                if (cteExiste == null)
                                    retorno = svcCTe.GerarCTeAnterior(stCTe, preCTe.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false, null, true);
                                else
                                {

                                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde = cteProc.CTe.infCte.ide;

                                    Dominio.Enumeradores.TipoTomador tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Remetente;

                                    if (infCTeIde.Item != null)
                                    {
                                        Type tipoTomador = infCTeIde.Item.GetType();
                                        if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                                        {
                                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tptomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                                            if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Remetente;
                                            else if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Expedidor;
                                            else if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Recebedor;
                                            else if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Destinatario;
                                        }
                                        else if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                                            tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Outros;
                                    }

                                    if (cteExiste.TipoTomador != tipoTomadorImportado)
                                    {
                                        stBuild.Append("Favor não alterar deliberadamente o tomador do CT-e, isso é fiscalmente ilegal.");
                                        return stBuild.ToString();
                                    }


                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeExite = repCargaCTe.BuscarPorCTe(cteExiste.Codigo);
                                    if (cargaCTeExite == null || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                                        retorno = cteExiste;
                                    else
                                    {
                                        stBuild.Append("O ct-e " + cteExiste.Numero.ToString() + " já foi vinculado a carga " + (cargaCTeExite?.Carga.CodigoCargaEmbarcador ?? ""));
                                        return stBuild.ToString();
                                    }
                                }

                                if (retorno != null)
                                {
                                    if (retorno.GetType() == typeof(string))
                                    {
                                        stBuild.Append((string)retorno);
                                    }
                                    else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteExiste != null)
                                    {
                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                                        if (cteExiste == null)
                                            cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                        else
                                            cte = cteExiste;

                                        if (configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                                        {
                                            cte.ValorFrete = BuscarFreteLiquidoComponente(cteProc);
                                            repCTe.Atualizar(cte);
                                        }

                                        if (string.IsNullOrEmpty(stBuild.ToString()))
                                        {
                                            cargaCTe.CTe = cte;
                                            VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                                            VincularConfiguracaoContabil(cte, preCTe, unitOfWork);
                                            repCargaCTe.Atualizar(cargaCTe);

                                            if (cargaCTe.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ObrigatorioAprovarCtesImportados ?? false)
                                                new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria, tipoServicoMultisoftware, cargaCTe);
                                        }
                                        else
                                        {
                                            new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, motivoInconsistencia, tipoServicoMultisoftware, cargaCTe);
                                        }
                                    }
                                    else
                                    {
                                        stBuild.Append("Conhecimento de transporte inválido.");
                                    }
                                }
                                else
                                {
                                    stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                                }
                            }
                            catch (ServicoException ex)
                            {
                                stBuild.Append(ex.Message);
                                return null;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                stBuild.Append("Falha ao processar o XML do cte");
                            }
                        }
                        else if (cteLido != null && cteLido.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                        {
                            try
                            {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Todos;
                                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork).BuscarPrimeiroRegistro();

                                ValidarCTeV400(cteProc, preCTe, stBuild, ref motivoInconsistencia, configuracao, configuracaoCargaEmissaoDocumento, unitOfWork);

                                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                if (cteProc.protCTe == null)
                                {
                                    stBuild.Append("O xml do CT-e enviado não possui a tag protCTe que contem os campos de autorização do mesmo, ou seja, ele não é válido, por favor verifique.");
                                    return stBuild.ToString();
                                }

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteExiste = repCTe.BuscarPorChave(cteProc.protCTe.infProt.chCTe);
                                object retorno = null;
                                if (cteExiste == null)
                                    retorno = svcCTe.GerarCTeAnterior(stCTe, preCTe.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false, null, true);
                                else
                                {

                                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde = cteProc.CTe.infCte.ide;

                                    Dominio.Enumeradores.TipoTomador tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Remetente;

                                    if (infCTeIde.Item != null)
                                    {
                                        Type tipoTomador = infCTeIde.Item.GetType();
                                        if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                                        {
                                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tptomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                                            if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Remetente;
                                            else if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Expedidor;
                                            else if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Recebedor;
                                            else if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                                                tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Destinatario;
                                        }
                                        else if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                                            tipoTomadorImportado = Dominio.Enumeradores.TipoTomador.Outros;
                                    }

                                    if (cteExiste.TipoTomador != tipoTomadorImportado)
                                    {
                                        stBuild.Append("Favor não alterar deliberadamente o tomador do CT-e, isso é fiscalmente ilegal.");
                                        return stBuild.ToString();
                                    }


                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeExite = repCargaCTe.BuscarPorCTe(cteExiste.Codigo);
                                    if (cargaCTeExite == null || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaCTeExite.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                                        retorno = cteExiste;
                                    else
                                    {
                                        stBuild.Append("O ct-e " + cteExiste.Numero.ToString() + " já foi vinculado a carga " + (cargaCTeExite?.Carga.CodigoCargaEmbarcador ?? ""));
                                        return stBuild.ToString();
                                    }
                                }

                                if (retorno != null)
                                {
                                    if (retorno.GetType() == typeof(string))
                                    {
                                        stBuild.Append((string)retorno);
                                    }
                                    else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteExiste != null)
                                    {
                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                                        if (cteExiste == null)
                                            cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                        else
                                            cte = cteExiste;

                                        if (configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                                        {
                                            cte.ValorFrete = BuscarFreteLiquidoComponente(cteProc);
                                            repCTe.Atualizar(cte);
                                        }

                                        if (string.IsNullOrEmpty(stBuild.ToString()))
                                        {
                                            cargaCTe.CTe = cte;
                                            VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
                                            VincularConfiguracaoContabil(cte, preCTe, unitOfWork);
                                            repCargaCTe.Atualizar(cargaCTe);

                                            if (cargaCTe.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ObrigatorioAprovarCtesImportados ?? false)
                                                new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria, tipoServicoMultisoftware, cargaCTe);
                                        }
                                        else
                                        {
                                            new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork).CriarInconsitencia(cte, motivoInconsistencia, tipoServicoMultisoftware, cargaCTe);
                                        }
                                    }
                                    else
                                    {
                                        stBuild.Append("Conhecimento de transporte inválido.");
                                    }
                                }
                                else
                                {
                                    stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                                }
                            }
                            catch (ServicoException ex)
                            {
                                stBuild.Append(ex.Message);
                                return null;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                stBuild.Append("Falha ao processar o XML do cte");
                            }
                        }
                        else
                        {
                            stBuild.Append("O arquivo enviado não é um CT-e válido, por favor verifique");
                        }
                    }
                    else
                    {
                        stBuild.Append("Um CT-e já foi importado para esse pré CT-e, por favor verifique");
                    }
                }
                else
                {
                    stBuild.Append("Você não pode mandar os arquivo em uma carga já cancelada, por favor, mande na viagem correta!");
                }

            }
            else
            {
                stBuild.Append("A carga não foi localizada para esse pré CT-e");
            }

            return stBuild.ToString();
        }

        public void DesvincularConfiguracaoContabil(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (preCTe == null)
                return;

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repositorioCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);

            cte.CentroResultado = null;
            cte.CentroResultadoEscrituracao = null;
            cte.CentroResultadoICMS = null;
            cte.CentroResultadoPIS = null;
            cte.CentroResultadoCOFINS = null;
            cte.ValorMaximoCentroContabilizacao = 0m;
            cte.CentroResultadoDestinatario = null;
            cte.ItemServico = string.Empty;

            repositorioCTe.Atualizar(cte);
            repositorioCTeContaContabilContabilizacao.DeletarPorCTe(cte.Codigo);
        }

        public void VincularConfiguracaoContabil(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (preCTe == null)
                return;

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repositorioCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.PreCTeContaContabilContabilizacao repositorioPreCTeContaContabilContabilizacao = new Repositorio.PreCTeContaContabilContabilizacao(unitOfWork);
            List<Dominio.Entidades.PreCTeContaContabilContabilizacao> preCTeContaContabilContabilizacaos = repositorioPreCTeContaContabilContabilizacao.BuscarPorPreCTe(preCTe.Codigo);

            cte.CentroResultado = preCTe.CentroResultado;
            cte.CentroResultadoEscrituracao = preCTe.CentroResultadoEscrituracao;
            cte.CentroResultadoICMS = preCTe.CentroResultadoICMS;
            cte.CentroResultadoPIS = preCTe.CentroResultadoPIS;
            cte.CentroResultadoCOFINS = preCTe.CentroResultadoCOFINS;
            cte.ValorMaximoCentroContabilizacao = preCTe.ValorMaximoCentroContabilizacao;
            cte.CentroResultadoDestinatario = preCTe.CentroResultadoDestinatario;
            cte.ItemServico = preCTe.ItemServico;

            repositorioCTe.Atualizar(cte);

            foreach (Dominio.Entidades.PreCTeContaContabilContabilizacao preCTeContaContabilContabilizacao in preCTeContaContabilContabilizacaos)
            {
                Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao = new Dominio.Entidades.CTeContaContabilContabilizacao()
                {
                    Cte = cte,
                    PlanoConta = preCTeContaContabilContabilizacao.PlanoConta,
                    TipoContabilizacao = preCTeContaContabilContabilizacao.TipoContabilizacao,
                    TipoContaContabil = preCTeContaContabilContabilizacao.TipoContaContabil
                };

                repositorioCTeContaContabilContabilizacao.Inserir(cteContaContabilContabilizacao);
            }
        }

        public void VincularPedidoXMLNotaAoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoDeTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentosCTE.BuscarPorCTe(cte.Codigo);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            cte.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                if (documento.NumeroModelo == "55")
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChaveECarga(documento.ChaveNFE, carga.Codigo);
                    if (xmlNotaFiscal != null)
                        cte.XMLNotaFiscais.Add(xmlNotaFiscal);
                }
                else
                {
                    int numero = 0;
                    int.TryParse(documento.Numero, out numero);
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorNumeroECarga(numero, carga.Codigo);
                    if (xmlNotaFiscal != null)
                        cte.XMLNotaFiscais.Add(xmlNotaFiscal);
                }
            }

            if (cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
            {
                List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentoDeTransporteAnteriorCTes = repDocumentoDeTransporteAnteriorCTe.BuscarPorCTe(cte.Codigo);
                foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe docAnterior in documentoDeTransporteAnteriorCTes)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoPedidoNotaFiscals = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorChaveCTeTerceiro(docAnterior.Chave, carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoPedidoNotaFiscals)
                    {
                        if (!cte.XMLNotaFiscais.Any(obj => obj.Codigo == pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo))
                            cte.XMLNotaFiscais.Add(pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal);
                    }
                }
            }

            if (cte.XMLNotaFiscais.Count == 0)
                cte.XMLNotaFiscais = repCargaPedidoXMLNotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(cargaCTe.Codigo);

            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;//todo: rever isso, pois por padrão sempre considera que o CT-e importado tem o valor do icms incluso no frete, isso afeta possiveis ctes de complemento.
            repCTe.Atualizar(cte);
        }

        public void preencherTransportador(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            stBuilder.Append("<b>DADOS DO TRANSPORTADOR</b>").AppendLine();
            stBuilder.Append("Transportador:" + preCTe.Empresa.RazaoSocial).AppendLine();
            stBuilder.Append("Endereço:" + preCTe.Empresa.Endereco + " " + preCTe.Empresa.Numero.ToString() + ". Bairro:" + preCTe.Empresa.Bairro).AppendLine();
            stBuilder.Append("Cidade:" + preCTe.Empresa.Localidade.DescricaoCidadeEstado).AppendLine();
            stBuilder.Append("IE:" + preCTe.Empresa.InscricaoEstadual).AppendLine();
            stBuilder.Append("CNPJ:" + preCTe.Empresa.CNPJ_Formatado).AppendLine().AppendLine();
        }

        public void preencherRemetente(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            stBuilder.Append("<b>DADOS DO REMETENTE</b>").AppendLine();
            stBuilder.Append("Rementente:" + preCTe.Remetente.Nome).AppendLine();
            stBuilder.Append("Endereço:" + preCTe.Remetente.Endereco + " " + preCTe.Remetente.Numero.ToString() + ". Bairro:" + preCTe.Remetente.Bairro).AppendLine();
            stBuilder.Append("Cidade:" + preCTe.Remetente.Localidade.DescricaoCidadeEstado).AppendLine();
            if (preCTe.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
            {
                stBuilder.Append("RG:" + preCTe.Remetente.IE_RG).AppendLine();
                stBuilder.Append("CNPJ:" + preCTe.Remetente.CPF_CNPJ_Formatado).AppendLine().AppendLine();
            }
            else
            {
                stBuilder.Append("IE:" + preCTe.Remetente.IE_RG).AppendLine();
                stBuilder.Append("CNPJ:" + preCTe.Remetente.CPF_CNPJ_Formatado).AppendLine().AppendLine();
            }
        }

        public void preencherDestinatario(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            stBuilder.Append("<B>DADOS DO DESTINATÁRIO</b>").AppendLine();
            stBuilder.Append("Rementente:" + preCTe.Destinatario.Nome).AppendLine();
            stBuilder.Append("Endereço:" + preCTe.Destinatario.Endereco + " " + preCTe.Destinatario.Numero.ToString() + ". Bairro:" + preCTe.Destinatario.Bairro).AppendLine();
            stBuilder.Append("Cidade:" + preCTe.Destinatario.Localidade.DescricaoCidadeEstado).AppendLine();
            if (preCTe.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
            {
                stBuilder.Append("RG:" + preCTe.Destinatario.IE_RG).AppendLine();
                stBuilder.Append("CNPJ:" + preCTe.Destinatario.CPF_CNPJ_Formatado).AppendLine().AppendLine();
            }
            else
            {
                stBuilder.Append("IE:" + preCTe.Destinatario.IE_RG).AppendLine();
                stBuilder.Append("CNPJ:" + preCTe.Destinatario.CPF_CNPJ_Formatado).AppendLine().AppendLine();
            }
        }

        public void preencherTomador(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            stBuilder.Append("<B>DADOS DO TOMADOR</b>").AppendLine();
            stBuilder.Append("Rementente:" + preCTe.Tomador.Nome).AppendLine();
            stBuilder.Append("Endereço:" + preCTe.Tomador.Endereco + " " + preCTe.Tomador.Numero.ToString() + ". Bairro:" + preCTe.Tomador.Bairro).AppendLine();
            stBuilder.Append("Cidade:" + preCTe.Tomador.Localidade.DescricaoCidadeEstado).AppendLine();
            if (preCTe.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
            {
                stBuilder.Append("RG:" + preCTe.Tomador.IE_RG).AppendLine();
                stBuilder.Append("CNPJ:" + preCTe.Tomador.CPF_CNPJ_Formatado).AppendLine().AppendLine();
            }
            else
            {
                stBuilder.Append("IE:" + preCTe.Tomador.IE_RG).AppendLine();
                stBuilder.Append("CNPJ:" + preCTe.Tomador.CPF_CNPJ_Formatado).AppendLine().AppendLine();
            }
        }

        public void preencherRecebedor(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            if (preCTe.Recebedor != null)
            {
                stBuilder.Append("<B>DADOS DO RECEBEDOR</b>").AppendLine();
                stBuilder.Append("Rementente:" + preCTe.Recebedor.Nome).AppendLine();
                stBuilder.Append("Endereço:" + preCTe.Recebedor.Endereco + " " + preCTe.Recebedor.Numero.ToString() + ". Bairro:" + preCTe.Recebedor.Bairro).AppendLine();
                stBuilder.Append("Cidade:" + preCTe.Recebedor.Localidade.DescricaoCidadeEstado).AppendLine();
                if (preCTe.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                {
                    stBuilder.Append("RG:" + preCTe.Recebedor.IE_RG).AppendLine();
                    stBuilder.Append("CNPJ:" + preCTe.Recebedor.CPF_CNPJ_Formatado).AppendLine().AppendLine();
                }
                else
                {
                    stBuilder.Append("IE:" + preCTe.Recebedor.IE_RG).AppendLine();
                    stBuilder.Append("CNPJ:" + preCTe.Recebedor.CPF_CNPJ_Formatado).AppendLine().AppendLine();
                }
            }

        }

        public void preencherExpedidor(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            if (preCTe.Expedidor != null)
            {
                stBuilder.Append("<B>DADOS DO EXPEDIDOR</b>").AppendLine();
                stBuilder.Append("Rementente:" + preCTe.Expedidor.Nome).AppendLine();
                stBuilder.Append("Endereço:" + preCTe.Expedidor.Endereco + " " + preCTe.Expedidor.Numero.ToString() + ". Bairro:" + preCTe.Expedidor.Bairro).AppendLine();
                stBuilder.Append("Cidade:" + preCTe.Expedidor.Localidade.DescricaoCidadeEstado).AppendLine();
                if (preCTe.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                {
                    stBuilder.Append("RG:" + preCTe.Expedidor.IE_RG).AppendLine();
                    stBuilder.Append("CNPJ:" + preCTe.Expedidor.CPF_CNPJ_Formatado).AppendLine().AppendLine();
                }
                else
                {
                    stBuilder.Append("IE:" + preCTe.Expedidor.IE_RG).AppendLine();
                    stBuilder.Append("CNPJ:" + preCTe.Expedidor.CPF_CNPJ_Formatado).AppendLine().AppendLine();
                }
            }
        }

        public void LinkarCTeComPreCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCCT = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            cargaCTe.Initialize();
            cargaCTe.CTe = cte;
            VincularPedidoXMLNotaAoCTe(cte, cargaCTe.Carga, cargaCTe, unitOfWork);
            VincularConfiguracaoContabil(cte, preCTe, unitOfWork);
            repCCT.Atualizar(cargaCTe);

            if (cargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                SetarDocumentoOriginario(cargaCTe.CTe, unitOfWork);

            VerificarEnviouTodosDocumentos(unitOfWork, cargaCTe.Carga, tipoServicoMultisoftware, configuracaoEmbarcador);
            new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(cargaCTe?.Carga?.Codigo ?? 0, cargaCTe ?? null);
        }

        //public void EnviarPreCtesPorEmais(List<Dominio.Entidades.PreConhecimentoDeTransporteEletronico> preCTEs, string stringConexao)
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
        //    Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
        //    Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailAtivo();
        //    if (email != null)
        //    {
        //        Servicos.Email serEmail = new Email(StringConexao);
        //        Servicos.PreCTe serPreCte = new Servicos.PreCTe(StringConexao);
        //        List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();

        //        foreach (Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte in preCTEs)
        //        {
        //            string xml = serPreCte.BuscarXMLPreCte(preCte, unitOfWork);
        //            MemoryStream stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml));
        //            System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(stream, string.Concat("pre_cte_" + preCte.Codigo, ".xml"));
        //            anexos.Add(anexo);

        //        }

        //        System.Text.StringBuilder stBuilder = new StringBuilder();

        //        Dominio.Entidades.Empresa empresa = preCTEs.FirstOrDefault().Empresa;

        //        stBuilder.Append("Olá " + empresa.RazaoSocial + ", segue em anexo o(s) xml(s) para emissão do(s) CT-e(s).").AppendLine().AppendLine();

        //        foreach (Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe in preCTEs)
        //        {
        //            stBuilder.Append("<b> <span> PRÉ CT-E NÚMERO: " + preCTe.Codigo + "</span></b>").AppendLine().AppendLine();
        //            preencherTransportador(ref stBuilder, preCTe);
        //            preencherRemetente(ref stBuilder, preCTe);
        //            preencherDestinatario(ref stBuilder, preCTe);
        //            preencherTomador(ref stBuilder, preCTe);
        //            preencherRecebedor(ref stBuilder, preCTe);
        //            preencherExpedidor(ref stBuilder, preCTe);
        //            preencherSeguroCarga(ref stBuilder, preCTe, unitOfWork);
        //            preencherDadosMercadoria(ref stBuilder, preCTe, unitOfWork);
        //            preencherDadosDoFrete(ref stBuilder, preCTe, unitOfWork);
        //            preencherDadosVeiculo(ref stBuilder, preCTe, unitOfWork);
        //            preencherDadosNotasFiscais(ref stBuilder, preCTe, unitOfWork);
        //            preencherDadosDocumentosAnteriores(ref stBuilder, preCTe, unitOfWork);
        //            stBuilder.Append("<hr/>").AppendLine();
        //        }

        //        if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
        //        {
        //            stBuilder.Append(email.MensagemRodape.Replace("#qLinha#", "<br/>"));
        //        }

        //        if (!string.IsNullOrWhiteSpace(empresa.Email))
        //            serEmail.EnviarEmail(email.Email, email.Email, email.Senha, empresa.Email, "", "", "Pré CT-e(s) para Emissão do(s) CT-e(s)", stBuilder.ToString(), email.Smtp, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp);
        //    }
        //    unitOfWork.Dispose();

        //}

        public void ValidarCTeV300(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, StringBuilder mensagemErro, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            validarParticipantes(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, configuracao);
            validarComponentes(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, unitOfWork, configuracao, configuracaoCargaEmissaoDocumento);

            if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                validarImpostos(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, configuracao);

            validarInfo(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, configuracao, unitOfWork);
        }

        public void ValidarCTeV400(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, StringBuilder mensagemErro, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            validarParticipantes(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, configuracao);
            validarComponentes(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, unitOfWork, configuracao, configuracaoCargaEmissaoDocumento);

            if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
                validarImpostos(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, configuracao);

            validarInfo(ref mensagemErro, cteProc, ref motivoInconsistencia, preCTe, configuracao, unitOfWork);
        }

        #endregion

        #region Metodos Privados

        private void ValidarCTeV200(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, StringBuilder mensagemErro, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(preCTe.Tomador.CPF_CNPJ));

            validarParticipantes(ref mensagemErro, cteProc, preCTe, configuracao);

            if (tomador == null || !tomador.NaoValidarValoresCTeImportadoQuandoTomador)
            {
                validarComponentes(ref mensagemErro, cteProc, preCTe, unitOfWork, configuracao);
                validarImpostos(ref mensagemErro, cteProc, preCTe, configuracao);
            }

            validarInfo(ref mensagemErro, cteProc, preCTe, configuracao, unitOfWork);
        }

        private void validarInfo(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
                return;

            if ((cteProc.CTe.infCte.ide.tpAmb == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TAmb.Item1 && preCTe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao) || (cteProc.CTe.infCte.ide.tpAmb == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TAmb.Item2 && preCTe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao))
            {
                stBuild.Append("O tipo de ambiente do CT-e " + cteProc.CTe.infCte.ide.tpAmb + " está diferente do informado no pré CT-e " + (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TAmb)preCTe.Empresa.TipoAmbiente).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoAmbiente;
            }


            if (cteProc.CTe.infCte.ide.tpCTe != (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TFinCTe)preCTe.TipoCTE)
            {
                stBuild.Append("O tipo de CT-e " + cteProc.CTe.infCte.ide.tpCTe + " está diferente do informado no pré CT-e " + (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TFinCTe)preCTe.TipoCTE).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item;

                //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v300.ModalRodoviario.rodo));
                //byte[] data = Encoding.UTF8.GetBytes(info.infModal.Any.OuterXml);
                //System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
                //MultiSoftware.CTe.v300.ModalRodoviario.rodo modalRodoviario = (MultiSoftware.CTe.v300.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                //memStream.Dispose();
                //memStream.Close();
                validarDocumentosAnteriores(ref stBuild, ref motivoInconsistencia, info, preCTe, unitOfWork);

                Repositorio.InformacaoCargaPreCTE repInformacaoCargaPreCte = new Repositorio.InformacaoCargaPreCTE(unitOfWork);
                List<Dominio.Entidades.InformacaoCargaPreCTE> informacoesCargaPreCTe = repInformacaoCargaPreCte.BuscarPorPreCTe(preCTe.Codigo);

                Repositorio.DocumentosPreCTE repDocumentosPreCte = new Repositorio.DocumentosPreCTE(unitOfWork);
                List<Dominio.Entidades.DocumentosPreCTE> documentosPreCte = repDocumentosPreCte.BuscarPorPreCte(preCTe.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();

                if (documentosPreCte.Count > 0 && documentosPreCte.Any(obj => !string.IsNullOrWhiteSpace(obj.ChaveNFE)) && info.infDoc != null)
                {
                    foreach (object infDoc in info.infDoc.Items)
                    {
                        if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.ChaveNFE == nfe.chave))
                            {
                                if (cargaCTe.Carga?.TipoOperacao?.PermitirImportarCTeComChaveNFeDiferenteNoPreCTe ?? false)
                                    throw new ServicoException("Chave de NFe do CTe não corresponde a chave do Pré-Cte da Carga");

                                stBuild.Append("A chave da nota " + nfe.chave + " informada no CT-e não corresponde a chave do do Pré - Cte da Carga ").AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                            }

                        }
                        else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.Numero == outro.nDoc))
                            {
                                stBuild.Append("O documento informado no CT-e " + outro.nDoc + " informada no CT-e não existe no pré CT-e ").AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                            }

                        }
                        else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.Numero == nf.nDoc))
                            {
                                stBuild.Append("A nota fiscal informada no CT-e " + nf.nDoc + " informada no CT-e não existe no pré CT-e ").AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                            }
                        }
                    }

                    if (documentosPreCte.Count != info.infDoc.Items.Length)
                    {
                        stBuild.Append("O numero de notas fiscais informados no ct-e (" + documentosPreCte.Count.ToString() + ") não corresponde ao numero de notas esperado no pré CT-e (" + info.infDoc.Items.Length.ToString() + ")").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                    }
                }
            }
            else
            {

                if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Item;
                    if (infCTeComple.chCTe != preCTe.ChaveCTESubComp)
                    {
                        stBuild.Append("A chave de complemento informado no CT-e " + infCTeComple.chCTe + " está diferente da informada no pré CT-e").AppendLine();
                    }
                }
            }
        }

        private void validarInfo(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
                return;

            if ((cteProc.CTe.infCte.ide.tpAmb == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TAmb.Item1 && preCTe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao) || (cteProc.CTe.infCte.ide.tpAmb == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TAmb.Item2 && preCTe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao))
            {
                stBuild.Append("O tipo de ambiente do CT-e " + cteProc.CTe.infCte.ide.tpAmb.ToString("d") + " está diferente do informado no pré CT-e " + ((MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TAmb)preCTe.Empresa.TipoAmbiente).ToString("d")).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoAmbiente;
            }

            if (cteProc.CTe.infCte.ide.tpCTe != (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TFinCTe)preCTe.TipoCTE)
            {
                stBuild.Append("O tipo de CT-e " + cteProc.CTe.infCte.ide.tpCTe.ToString("d") + " está diferente do informado no pré CT-e " + ((MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TFinCTe)preCTe.TipoCTE).ToString("d")).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (cteProc.CTe.infCte.Items.FirstOrDefault()?.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Items.FirstOrDefault();

                validarDocumentosAnteriores(ref stBuild, ref motivoInconsistencia, info, preCTe, unitOfWork);

                Repositorio.DocumentosPreCTE repDocumentosPreCte = new Repositorio.DocumentosPreCTE(unitOfWork);
                List<Dominio.Entidades.DocumentosPreCTE> documentosPreCte = repDocumentosPreCte.BuscarPorPreCte(preCTe.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();


                if (documentosPreCte.Count > 0 && documentosPreCte.Any(obj => !string.IsNullOrWhiteSpace(obj.ChaveNFE)) && info.infDoc != null)
                {
                    foreach (object infDoc in info.infDoc.Items)
                    {
                        if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.ChaveNFE == nfe.chave))
                            {
                                if (cargaCTe.Carga?.TipoOperacao?.PermitirImportarCTeComChaveNFeDiferenteNoPreCTe ?? false)
                                    throw new ServicoException("Chave de NFe do CTe não corresponde a chave do Pré-Cte da Carga");

                                stBuild.Append("A chave da nota " + nfe.chave + " informada no CT-e não existe no pré CT-e ").AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                            }

                        }
                        else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.Numero == outro.nDoc))
                            {
                                stBuild.Append("O documento informado no CT-e " + outro.nDoc + " informada no CT-e não existe no pré CT-e ").AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                            }

                        }
                        else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.Numero == nf.nDoc))
                            {
                                stBuild.Append("A nota fiscal informada no CT-e " + nf.nDoc + " informada no CT-e não existe no pré CT-e ").AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                            }
                        }
                    }

                    if (documentosPreCte.Count != info.infDoc.Items.Length)
                    {
                        stBuild.Append("O numero de notas fiscais informados no ct-e (" + documentosPreCte.Count.ToString() + ") não corresponde ao numero de notas esperado no pré CT-e (" + info.infDoc.Items.Length.ToString() + ")").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.NotasFiscais;
                    }
                }
            }
            else
            {

                if (cteProc.CTe.infCte.Items.FirstOrDefault()?.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Items.First();
                    if (infCTeComple.chCTe != preCTe.ChaveCTESubComp)
                    {
                        stBuild.Append("A chave de complemento informado no CT-e " + infCTeComple.chCTe + " está diferente da informada no pré CT-e").AppendLine();
                    }
                }
            }
        }

        private void validarInfo(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
                return;

            //if (cteProc.CTe.infCte.ide.CFOP != preCTe.CFOP.CodigoCFOP.ToString())
            //    stBuild.Append("A CFOP do CT-e " + cteProc.CTe.infCte.ide.CFOP + " está diferente do informado no pré CT-e " + preCTe.CFOP.CodigoCFOP.ToString()).AppendLine();

            //if (cteProc.CTe.infCte.ide.forPag != (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeForPag)preCTe.TipoPagamento)
            //    stBuild.Append("A forma de pagamento do CT-e " + cteProc.CTe.infCte.ide.forPag + " está diferente do informado no pré CT-e " + (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeForPag)preCTe.TipoPagamento).AppendLine();

            if (preCTe.Empresa != null && (cteProc.CTe.infCte.ide.tpAmb != (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TAmb)preCTe.Empresa.TipoAmbiente))
                stBuild.Append("O tipo de ambiente do CT-e " + cteProc.CTe.infCte.ide.tpAmb + " está diferente do informado no pré CT-e " + (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TAmb)preCTe.Empresa.TipoAmbiente).AppendLine();

            if (cteProc.CTe.infCte.ide.tpCTe != (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TFinCTe)preCTe.TipoCTE)
                stBuild.Append("O tipo de CT-e " + cteProc.CTe.infCte.ide.tpCTe + " está diferente do informado no pré CT-e " + (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TFinCTe)preCTe.TipoCTE).AppendLine();

            //if (cteProc.CTe.infCte.ide.cMunEnv != preCTe.LocalidadeEmissao.CodigoIBGE.ToString())
            //    stBuild.Append("A localidade de envio do CT-e " + cteProc.CTe.infCte.ide.cMunEnv + " está diferente do informado no pré CT-e " + preCTe.LocalidadeEmissao.CodigoIBGE.ToString()).AppendLine();

            //if (cteProc.CTe.infCte.ide.cMunIni != preCTe.LocalidadeInicioPrestacao.CodigoIBGE.ToString())
            //    stBuild.Append("A localidade do inicio da prestação do CT-e " + cteProc.CTe.infCte.ide.cMunIni + " está diferente do informado no pré CT-e " + preCTe.LocalidadeInicioPrestacao.CodigoIBGE.ToString()).AppendLine();

            //if (cteProc.CTe.infCte.ide.cMunFim != preCTe.LocalidadeTerminoPrestacao.CodigoIBGE.ToString())
            //    stBuild.Append("O Municipio de envio do CT-e " + cteProc.CTe.infCte.ide.cMunFim + " está diferente do informado no pré CT-e " + preCTe.LocalidadeTerminoPrestacao.CodigoIBGE.ToString()).AppendLine();

            //if (cteProc.CTe.infCte.ide.retira == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item0 && preCTe.Retira == Dominio.Enumeradores.OpcaoSimNao.Nao)
            //    stBuild.Append("A opção retira do CT-e " + cteProc.CTe.infCte.ide.retira + " está diferente do informado no pré CT-e " + MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item1).AppendLine();

            //if (cteProc.CTe.infCte.ide.retira == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item1 && preCTe.Retira == Dominio.Enumeradores.OpcaoSimNao.Sim)
            //    stBuild.Append("A opção retira do CT-e " + cteProc.CTe.infCte.ide.retira + " está diferente do informado no pré CT-e " + MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item0).AppendLine();



            if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item;

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v200.ModalRodoviario.rodo));
                byte[] data = Encoding.UTF8.GetBytes(info.infModal.Any.OuterXml);
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
                MultiSoftware.CTe.v200.ModalRodoviario.rodo modalRodoviario = (MultiSoftware.CTe.v200.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                memStream.Dispose();
                memStream.Close();
                //validarRodo(ref stBuild, modalRodoviario, preCTe, unitOfWork);
                validarDocumentosAnteriores(ref stBuild, info, preCTe, unitOfWork);

                Repositorio.InformacaoCargaPreCTE repInformacaoCargaPreCte = new Repositorio.InformacaoCargaPreCTE(unitOfWork);
                List<Dominio.Entidades.InformacaoCargaPreCTE> informacoesCargaPreCTe = repInformacaoCargaPreCte.BuscarPorPreCTe(preCTe.Codigo);

                //if (decimal.Parse(info.infCarga.vCarga, cultureInfo) != preCTe.ValorTotalMercadoria)
                //    stBuild.Append("O valor da carga " + info.infCarga.vCarga + " informado no CT-e está diferente do informado no pré CT-e " + preCTe.ValorTotalMercadoria.ToString("f2", cultureInfo)).AppendLine();

                //decimal pesoTotalCTE = 0;
                //decimal pesoTotalPreCTE = Math.Round((from obj in informacoesCargaPreCTe select obj.Quantidade).Sum(), 4, MidpointRounding.AwayFromZero);
                //foreach (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCargaInfQ infQ in info.infCarga.infQ)
                //{
                //    pesoTotalCTE += decimal.Parse(infQ.qCarga, cultureInfo);
                //}
                //if (pesoTotalCTE != pesoTotalPreCTE)
                //    stBuild.Append("A quantidade da carga" + pesoTotalCTE.ToString("f4", cultureInfo) + " informado no CT-e está diferente da informada no pré CT-e " + pesoTotalPreCTE.ToString("f4", cultureInfo)).AppendLine();


                Repositorio.DocumentosPreCTE repDocumentosPreCte = new Repositorio.DocumentosPreCTE(unitOfWork);
                List<Dominio.Entidades.DocumentosPreCTE> documentosPreCte = repDocumentosPreCte.BuscarPorPreCte(preCTe.Codigo);


                if (documentosPreCte.Count > 0 && documentosPreCte.Any(obj => !string.IsNullOrWhiteSpace(obj.ChaveNFE)))
                {
                    foreach (object infDoc in info.infDoc.Items)
                    {
                        if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                        {
                            MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.ChaveNFE == nfe.chave))
                                stBuild.Append("A chave da nota " + nfe.chave + " informada no CT-e não existe no pré CT-e ").AppendLine();
                        }
                        else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                        {
                            MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outro = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.Numero == outro.nDoc))
                                stBuild.Append("O documento informado no CT-e " + outro.nDoc + " informada no CT-e não existe no pré CT-e ").AppendLine();
                        }
                        else if (infDoc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                        {
                            MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)infDoc;
                            if (!documentosPreCte.Exists(obj => obj.Numero == nf.nDoc))
                                stBuild.Append("A nota fiscal informada no CT-e " + nf.nDoc + " informada no CT-e não existe no pré CT-e ").AppendLine();
                        }
                    }
                }



                //Repositorio.SeguroPreCTE repSeguroPreCTE = new Repositorio.SeguroPreCTE(unitOfWork);
                //List<Dominio.Entidades.SeguroPreCTE> segurosPreCTE = repSeguroPreCTE.BuscarPorPreCte(preCTe.Codigo);

                //foreach (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormSeg seg in info.seg)
                //{
                //    if (!segurosPreCTE.Exists(obj => (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormSegRespSeg)obj.Tipo == seg.respSeg && obj.Valor == decimal.Parse(!string.IsNullOrEmpty(seg.vCarga) ? seg.vCarga : "0", cultureInfo)))
                //        stBuild.Append("O seguro informado no CT-e do tipo " + seg.respSeg + " no valor " + seg.vCarga + " não existe no pré CT-e ").AppendLine();
                //}

                //foreach (Dominio.Entidades.SeguroPreCTE seguro in segurosPreCTE)
                //{
                //    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormSeg seg = (from obj in info.seg where obj.respSeg == (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormSegRespSeg)seguro.Tipo && decimal.Parse(!string.IsNullOrEmpty(obj.vCarga) ? obj.vCarga : "0", cultureInfo) == seguro.Valor select obj).FirstOrDefault();
                //    if (seg == null)
                //        stBuild.Append("O seguro informado no pré CT-e do tipo " + (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormSegRespSeg)seguro.Tipo + " no valor " + seguro.Valor.ToString("f2", cultureInfo) + " não existe no CT-e ").AppendLine();
                //}
            }
            else
            {

                if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)cteProc.CTe.infCte.Item;
                    if (infCTeComple.chave != preCTe.ChaveCTESubComp)
                    {
                        stBuild.Append("A chave de complemento informado no CT-e " + infCTeComple.chave + " está diferente da informada no pré CT-e").AppendLine();
                    }
                }
            }

        }

        private void validarRodo(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v200.ModalRodoviario.rodo rodo, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VeiculoPreCTE repVeiculoPreCTE = new Repositorio.VeiculoPreCTE(unitOfWork);
            List<Dominio.Entidades.VeiculoPreCTE> veiculosPreCTe = repVeiculoPreCTE.BuscarPorPreCte(preCTe.Codigo);

            Repositorio.MotoristaPreCTE repMotoristaPreCte = new Repositorio.MotoristaPreCTE(unitOfWork);
            List<Dominio.Entidades.MotoristaPreCTE> motoristasPreCTe = repMotoristaPreCte.BuscarPorPreCte(preCTe.Codigo);

            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosTransportaAnterior = repDocumentoDeTransporteAnteriorPreCTE.BuscarPorPreCte(preCTe.Codigo);

            //if (rodo.RNTRC != preCTe.RNTRC)
            //    stBuild.Append("A RNTRC " + rodo.RNTRC + " do CT-e está diferente do informado no pré CT-e ").AppendLine();

            //if (rodo.lota != (MultiSoftware.CTe.v200.ModalRodoviario.rodoLota)preCTe.Lotacao)
            //    stBuild.Append("A Lotação " + rodo.lota + " do CT-e está diferente do informado no pré CT-e " + (MultiSoftware.CTe.v200.ModalRodoviario.rodoLota)preCTe.Lotacao).AppendLine();

            if (rodo.veic != null)
            {
                foreach (MultiSoftware.CTe.v200.ModalRodoviario.rodoVeic vei in rodo.veic)
                {
                    if (!veiculosPreCTe.Exists(obj => obj.Placa == vei.placa && obj.RENAVAM == vei.RENAVAM))
                        stBuild.Append("O Veículo placa " + vei.placa + " RENAVAM" + vei.RENAVAM + " do CT-e não existe no pré CT-e ").AppendLine();
                }
            }

            foreach (Dominio.Entidades.VeiculoPreCTE veiculoPre in veiculosPreCTe)
            {
                MultiSoftware.CTe.v200.ModalRodoviario.rodoVeic vei = null;
                if (rodo.veic != null)
                    vei = (from obj in rodo.veic where obj.placa == veiculoPre.Placa && obj.RENAVAM == veiculoPre.RENAVAM select obj).FirstOrDefault();

                if (vei == null)
                    stBuild.Append("O Veículo placa " + veiculoPre.Placa + " RENAVAM" + veiculoPre.RENAVAM + " informado no pré CT-e não existe no CT-e ").AppendLine();
            }

            if (rodo.moto != null)
            {
                foreach (MultiSoftware.CTe.v200.ModalRodoviario.rodoMoto mot in rodo.moto)
                {
                    if (!motoristasPreCTe.Exists(obj => obj.CPFMotorista == mot.CPF))
                        stBuild.Append("O Motorista " + mot.xNome + "(" + mot.CPF + ") do CT-e não existe no pré CT-e ").AppendLine();
                }
            }

            foreach (Dominio.Entidades.MotoristaPreCTE motoristaPre in motoristasPreCTe)
            {
                MultiSoftware.CTe.v200.ModalRodoviario.rodoMoto mot = null;
                if (rodo.moto != null)
                    mot = (from obj in rodo.moto where obj.CPF == motoristaPre.CPFMotorista select obj).FirstOrDefault();

                if (mot == null)
                    stBuild.Append("O Motorista " + motoristaPre.NomeMotorista + "(" + motoristaPre.CPFMotorista + ") informado no pré CT-e não existe no CT-e ").AppendLine();
            }
        }

        private void validarDocumentosAnteriores(ref System.Text.StringBuilder stBuild, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosTransportaAnterior = repDocumentoDeTransporteAnteriorPreCTE.BuscarPorPreCte(preCTe.Codigo);

            if (info.docAnt != null && documentosTransportaAnterior.Count > 0)
            {
                List<Dominio.Entidades.Cliente> emissoresDocumentosAnteriores = (from obj in documentosTransportaAnterior select obj.Emissor).Distinct().ToList();

                foreach (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt in info.docAnt)
                {
                    if (emissoresDocumentosAnteriores.Exists(obj => obj.CPF_CNPJ_SemFormato == docAnt.Item))
                    {
                        foreach (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDocAnt in docAnt.idDocAnt)
                        {
                            foreach (object idDocAntTipo in idDocAnt.Items)
                            {
                                if (idDocAntTipo.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                                {
                                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle ele = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)idDocAntTipo;
                                    if (!documentosTransportaAnterior.Exists(obj => obj.Emissor.CPF_CNPJ_SemFormato == docAnt.Item && obj.Chave == ele.chCTe))
                                    {
                                        stBuild.Append("A chave " + ele.chCTe + "  do documento anterior no CT-e não existe no pré CT-e").AppendLine();
                                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                                    }

                                }
                                else
                                {
                                    stBuild.Append("O tipo do documento anterior precisa ser do tipo eletrônico").AppendLine();
                                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                                }
                            }
                        }
                    }
                    else
                    {
                        stBuild.Append("O emissor do documento anterior " + docAnt.xNome + " informado no CT-e não existe no pré CT-e").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                    }
                }

                foreach (Dominio.Entidades.Cliente emissor in emissoresDocumentosAnteriores)
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt = (from obj in info.docAnt where obj.Item == emissor.CPF_CNPJ_SemFormato select obj).FirstOrDefault();
                    if (docAnt == null)
                    {
                        stBuild.Append("O emissor do documento anterior informado no pre CT-e " + emissor.Nome + " não existe no CT-e").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                    }
                }
            }

            if (info.docAnt != null && documentosTransportaAnterior.Count == 0)
            {
                stBuild.Append("O CT-e possui documentos anteriores não informados no pré CT-e").AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
            }

            if (info.docAnt == null && documentosTransportaAnterior.Count > 0)
            {
                stBuild.Append("O CT-e não possui documentos anteriores informados no pré CT-e").AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
            }
        }

        private void validarDocumentosAnteriores(ref System.Text.StringBuilder stBuild, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosTransportaAnterior = repDocumentoDeTransporteAnteriorPreCTE.BuscarPorPreCte(preCTe.Codigo);

            if (info.docAnt != null && documentosTransportaAnterior.Count > 0)
            {
                List<Dominio.Entidades.Cliente> emissoresDocumentosAnteriores = (from obj in documentosTransportaAnterior select obj.Emissor).Distinct().ToList();

                foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt in info.docAnt)
                {
                    if (emissoresDocumentosAnteriores.Exists(obj => obj.CPF_CNPJ_SemFormato == docAnt.Item))
                    {
                        foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDocAnt in docAnt.idDocAnt)
                        {
                            foreach (object idDocAntTipo in idDocAnt.Items)
                            {
                                if (idDocAntTipo.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                                {
                                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle ele = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)idDocAntTipo;
                                    if (!documentosTransportaAnterior.Exists(obj => obj.Emissor.CPF_CNPJ_SemFormato == docAnt.Item && obj.Chave == ele.chCTe))
                                    {
                                        stBuild.Append("A chave " + ele.chCTe + "  do documento anterior no CT-e não existe no pré CT-e").AppendLine();
                                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                                    }

                                }
                                else
                                {
                                    stBuild.Append("O tipo do documento anterior precisa ser do tipo eletrônico").AppendLine();
                                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                                }
                            }
                        }
                    }
                    else
                    {
                        stBuild.Append("O emissor do documento anterior " + docAnt.xNome + " informado no CT-e não existe no pré CT-e").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                    }
                }

                foreach (Dominio.Entidades.Cliente emissor in emissoresDocumentosAnteriores)
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt = (from obj in info.docAnt where obj.Item == emissor.CPF_CNPJ_SemFormato select obj).FirstOrDefault();
                    if (docAnt == null)
                    {
                        stBuild.Append("O emissor do documento anterior informado no pre CT-e " + emissor.Nome + " não existe no CT-e").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
                    }
                }
            }

            if (info.docAnt != null && documentosTransportaAnterior.Count == 0)
            {
                stBuild.Append("O CT-e possui documentos anteriores não informados no pré CT-e").AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
            }

            if (info.docAnt == null && info.infServVinc == null && documentosTransportaAnterior.Count > 0)
            {
                stBuild.Append("O CT-e não possui documentos anteriores informados no pré CT-e").AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CTeAnterior;
            }
        }

        private void validarDocumentosAnteriores(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosTransportaAnterior = repDocumentoDeTransporteAnteriorPreCTE.BuscarPorPreCte(preCTe.Codigo);

            if (info.docAnt != null && documentosTransportaAnterior.Count > 0)
            {
                List<Dominio.Entidades.Cliente> emissoresDocumentosAnteriores = (from obj in documentosTransportaAnterior select obj.Emissor).Distinct().ToList();

                foreach (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt in info.docAnt)
                {
                    if (emissoresDocumentosAnteriores.Exists(obj => obj.CPF_CNPJ_SemFormato == docAnt.Item))
                    {
                        foreach (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDocAnt in docAnt.idDocAnt)
                        {
                            foreach (object idDocAntTipo in idDocAnt.Items)
                            {
                                if (idDocAntTipo.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                                {
                                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle ele = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)idDocAntTipo;
                                    if (!documentosTransportaAnterior.Exists(obj => obj.Emissor.CPF_CNPJ_SemFormato == docAnt.Item && obj.Chave == ele.chave))
                                        stBuild.Append("A chave " + ele.chave + "  do documento anterior no CT-e não existe no pré CT-e").AppendLine();
                                }
                                else
                                {
                                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntPap pap = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntPap)idDocAntTipo;
                                    if (!documentosTransportaAnterior.Exists(obj => obj.Emissor.CPF_CNPJ_SemFormato == docAnt.Item && obj.Numero == pap.nDoc && (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TDocAssoc)int.Parse(obj.Tipo) == pap.tpDoc))
                                        stBuild.Append("O número " + pap.nDoc + " do tipo " + pap.tpDoc + "  do documento anterior no CT-e não existe no pré CT-e").AppendLine();
                                }
                            }
                        }
                    }
                    else
                    {
                        stBuild.Append("O emissor do documento anterior " + docAnt.xNome + " informado no CT-e não existe no pré CT-e").AppendLine();
                    }
                }

                foreach (Dominio.Entidades.Cliente emissor in emissoresDocumentosAnteriores)
                {

                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt = (from obj in info.docAnt where obj.Item == emissor.CPF_CNPJ_SemFormato select obj).FirstOrDefault();
                    if (docAnt == null)
                        stBuild.Append("O emissor do documento anterior informado no pre CT-e " + emissor.Nome + " não existe no CT-e").AppendLine();
                }
            }

            if (info.docAnt != null && documentosTransportaAnterior.Count == 0)
                stBuild.Append("O CT-e possui documentos anteriores não informados no pré CT-e").AppendLine();
            if (info.docAnt == null && documentosTransportaAnterior.Count > 0)
                stBuild.Append("O CT-e não possui documentos anteriores informados no pré CT-e").AppendLine();
        }

        private bool ValidarValorComMargem(decimal valorPreCTe, decimal valorCte, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Empresa empresa)
        {
            bool valido = true;
            valorPreCTe = Math.Round(valorPreCTe, 2, MidpointRounding.AwayFromZero);
            valorCte = Math.Round(valorCte, 2, MidpointRounding.AwayFromZero);

            if (configuracaoTMS.AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado && (valorCte < valorPreCTe || valorPreCTe == 0m))
                return true;

            if (valorPreCTe != valorCte)
            {
                decimal margem = (decimal)0.01;
                decimal percentualTolerancia = empresa.PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado;

                if (percentualTolerancia > 0)
                {
                    decimal resultado = Math.Round((valorCte * 100) / valorPreCTe, 2, MidpointRounding.AwayFromZero);
                    decimal diferencaPercentual = 0;
                    if (resultado > 100)
                        diferencaPercentual = resultado - 100;
                    else
                        diferencaPercentual = 100 - resultado;

                    if (diferencaPercentual <= percentualTolerancia)
                        return true;
                    else
                        return false;
                }
                else
                {
                    decimal margemMais = valorCte + margem;
                    decimal margemMenos = valorCte - margem;
                    if (valorPreCTe <= margemMais && valorPreCTe >= margemMenos)
                        valido = true;
                    else
                        valido = false;
                }
            }
            return valido;
        }

        private bool ValidarComponentesFretePreCTe(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteVPrestComp[] componentes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado repositorioComponenteCTeImportado = new Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado(unitOfWork);
            Repositorio.ComponentePrestacaoPreCTE repositorioComponentePrestacaoPreCTe = new Repositorio.ComponentePrestacaoPreCTE(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> componentesCTesImportados = repositorioComponenteCTeImportado.BuscarPorTransportadorComFetchComponenteFrete(preCTe.Empresa.Codigo);

            if (componentesCTesImportados.Count <= 0)
                return true;

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPrestaoPreCTe = repositorioComponentePrestacaoPreCTe.BuscarPorPreCTe(preCTe.Codigo);

            foreach (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteVPrestComp componente in componentes)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado componenteCTesImportados = (from obj in componentesCTesImportados where obj.Descricao == componente.xNome select obj).FirstOrDefault();

                if (componenteCTesImportados == null)
                    return false;

                Dominio.Entidades.ComponentePrestacaoPreCTE componentePrestaoPreCTe = componentesPrestaoPreCTe.Find(obj => obj.ComponenteFrete?.Codigo == componenteCTesImportados.ComponenteFrete.Codigo);

                if (componentePrestaoPreCTe == null)
                    return false;

                if (!ValidarValorComMargem(componentePrestaoPreCTe.Valor, decimal.Parse(componente.vComp, cultureInfo), configuracaoEmbarcador, preCTe.Empresa))
                    return false;
            }

            return true;
        }

        private bool ValidarComponentesFretePreCTe(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp[] componentes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado repositorioComponenteCTeImportado = new Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado(unitOfWork);
            Repositorio.ComponentePrestacaoPreCTE repositorioComponentePrestacaoPreCTe = new Repositorio.ComponentePrestacaoPreCTE(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> componentesCTesImportados = repositorioComponenteCTeImportado.BuscarPorTransportadorComFetchComponenteFrete(preCTe.Empresa.Codigo);

            if (componentesCTesImportados.Count <= 0)
                return true;

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPrestaoPreCTe = repositorioComponentePrestacaoPreCTe.BuscarPorPreCTe(preCTe.Codigo);

            foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp componente in componentes)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado componenteCTesImportados = (from obj in componentesCTesImportados where obj.Descricao == componente.xNome select obj).FirstOrDefault();

                if (componenteCTesImportados == null)
                    return false;

                Dominio.Entidades.ComponentePrestacaoPreCTE componentePrestaoPreCTe = componentesPrestaoPreCTe.Find(obj => obj.ComponenteFrete?.Codigo == componenteCTesImportados.ComponenteFrete.Codigo);

                if (componentePrestaoPreCTe == null)
                    return false;

                if (!ValidarValorComMargem(componentePrestaoPreCTe.Valor, decimal.Parse(componente.vComp, cultureInfo), configuracaoEmbarcador, preCTe.Empresa))
                    return false;
            }

            return true;
        }

        private void validarImpostos(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteImp infImp = cteProc.CTe.infCte.imp;
            if (infImp != null)
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                Type tipoICMS = infImp.ICMS.Item.GetType();
                if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS00))
                {
                    if (preCTe.CST != "00")
                        stBuild.Append("A CST 00 informada no CT-e está diferente do informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    else
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS00 icms = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();

                        decimal valorICMSCTe = decimal.Parse(icms.vICMS, cultureInfo);
                        if (!ValidarValorComMargem(preCTe.ValorICMS, valorICMSCTe, configuracao, preCTe.Empresa))
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS20))
                {
                    if (preCTe.CST != "20")
                        stBuild.Append("A CST 20 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    else
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS20 icms = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMS, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (decimal.Parse(icms.pRedBC, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBC + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();

                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS45))
                {
                    if (preCTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCTe.CST != "40" && preCTe.CST != "41" && preCTe.CST != "51")
                        stBuild.Append("A CST 40 informada no CT-e está diferente do informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS60))
                {
                    if (preCTe.CST != "60")
                        stBuild.Append("A CST 60 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    else
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS60 icms = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMSSTRet, cultureInfo) != preCTe.AliquotaICMS)
                            stBuild.Append("A aliquota do ICMS ST no CT-e " + icms.pICMSSTRet + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBCSTRet, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBCSTRet + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMSSTRet, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("O valor do ICMS ST no CT-e " + icms.vICMSSTRet + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!string.IsNullOrWhiteSpace(icms.vCred))
                        {
                            if (decimal.Parse(icms.vCred, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                                stBuild.Append("O valor de crédito presumido no CT-e " + icms.vCred + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                        }
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS90))
                {
                    if (preCTe.CST != "91")
                        stBuild.Append("A CST 90 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    else
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS90 icms = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMS, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();


                        if (!string.IsNullOrWhiteSpace(icms.pRedBC))
                        {
                            if (decimal.Parse(icms.pRedBC, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                                stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBC + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                        }


                        if (!string.IsNullOrWhiteSpace(icms.vCred))
                        {
                            if (decimal.Parse(icms.vCred, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                                stBuild.Append("O valor de crédito presumido no CT-e " + icms.vCred + " está diferente do informado no pré CT-e " + preCTe.ValorPresumido.ToString("f2", cultureInfo)).AppendLine();
                        }
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {
                    if (preCTe.CST != "90")
                        stBuild.Append("A CST Outra UF informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    else
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSOutraUF icms = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMSOutraUF, cultureInfo) != preCTe.AliquotaICMS)
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMSOutraUF + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBCOutraUF, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBCOutraUF + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMSOutraUF, cultureInfo), configuracao, preCTe.Empresa))
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMSOutraUF + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();

                        if (!string.IsNullOrWhiteSpace(icms.pRedBCOutraUF))
                        {
                            if (decimal.Parse(icms.pRedBCOutraUF, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                                stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBCOutraUF + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                        }

                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    if (preCTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCTe.CST != "40" && preCTe.CST != "41" && preCTe.CST != "51")
                        stBuild.Append("O CT-e informado está em regime de simples nacional enquanto o esperado é possuir a CST " + preCTe.CST).AppendLine();
                }
            }
            else
            {
                stBuild.Append("A tag  infCte.imp não foi localizada no xml do CT-e").AppendLine();
            }
        }

        private void validarImpostos(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteImp infImp = cteProc.CTe.infCte.imp;
            if (infImp != null)
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                Type tipoICMS = infImp.ICMS.Item.GetType();
                if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS00))
                {
                    if (preCTe.CST != "00")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 00 informada no CT-e está diferente do informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS00 icms = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }

                        decimal valorICMSCTe = decimal.Parse(icms.vICMS, cultureInfo);
                        if (!ValidarValorComMargem(preCTe.ValorICMS, valorICMSCTe, configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }
                    }

                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS20))
                {
                    if (preCTe.CST != "20")
                    {
                        stBuild.Append("A CST 20 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                    }
                    else
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS20 icms = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMS, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }


                        if (decimal.Parse(icms.pRedBC, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                        {
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBC + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                        }
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS45))
                {
                    if (preCTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCTe.CST != "40" && preCTe.CST != "41" && preCTe.CST != "51")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 45 informada no CT-e está diferente do informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }


                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS60))
                {

                    if (preCTe.CST != "60")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 60 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS60 icms = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMSSTRet, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS ST no CT-e " + icms.pICMSSTRet + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBCSTRet, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBCSTRet + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMSSTRet, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS ST no CT-e " + icms.vICMSSTRet + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }

                        if (!string.IsNullOrWhiteSpace(icms.vCred))
                        {
                            if (decimal.Parse(icms.vCred, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                                stBuild.Append("O valor de crédito presumido no CT-e " + icms.vCred + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            }

                        }
                    }


                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS90))
                {

                    if (preCTe.CST != "91")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 90 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS90 icms = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }


                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMS, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }

                        if (!string.IsNullOrWhiteSpace(icms.pRedBC))
                        {
                            if (decimal.Parse(icms.pRedBC, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBC + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(icms.vCred))
                        {
                            if (decimal.Parse(icms.vCred, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                stBuild.Append("O valor de crédito presumido no CT-e " + icms.vCred + " está diferente do informado no pré CT-e " + preCTe.ValorPresumido.ToString("f2", cultureInfo)).AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            }
                        }
                    }

                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {

                    if (preCTe.CST != "90")
                    {
                        stBuild.Append("A CST Outra UF informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                    }
                    else
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSOutraUF icms = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMSOutraUF, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMSOutraUF + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBCOutraUF, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBCOutraUF + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }


                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMSOutraUF, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMSOutraUF + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }


                        if (!string.IsNullOrWhiteSpace(icms.pRedBCOutraUF))
                        {
                            if (decimal.Parse(icms.pRedBCOutraUF, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBCOutraUF + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            }

                        }
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    if (preCTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCTe.CST != "40" && preCTe.CST != "41" && preCTe.CST != "51")
                    {
                        stBuild.Append("A CST " + preCTe.CST + " do pré CT-e é diferente da informada no CT-e" + preCTe.CST).AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                    }
                }
            }
            else
            {
                stBuild.Append("A tag  infCte.imp não foi localizada no xml do CT-e").AppendLine();
            }
        }

        private void validarImpostos(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteImp infImp = cteProc.CTe.infCte.imp;
            if (infImp != null)
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                Type tipoICMS = infImp.ICMS.Item.GetType();
                if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00))
                {
                    if (preCTe.CST != "00")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 00 informada no CT-e está diferente do informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00 icms = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }

                        decimal valorICMSCTe = decimal.Parse(icms.vICMS, cultureInfo);
                        if (!ValidarValorComMargem(preCTe.ValorICMS, valorICMSCTe, configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }
                    }

                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20))
                {
                    if (preCTe.CST != "20")
                    {
                        stBuild.Append("A CST 20 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                    }
                    else
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20 icms = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMS, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }


                        if (decimal.Parse(icms.pRedBC, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                        {
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBC + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                        }
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45))
                {
                    if (preCTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCTe.CST != "40" && preCTe.CST != "41" && preCTe.CST != "51")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 45 informada no CT-e está diferente do informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }


                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60))
                {

                    if (preCTe.CST != "60")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 60 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60 icms = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMSSTRet, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS ST no CT-e " + icms.pICMSSTRet + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBCSTRet, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBCSTRet + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }

                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMSSTRet, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS ST no CT-e " + icms.vICMSSTRet + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }

                        if (!string.IsNullOrWhiteSpace(icms.vCred))
                        {
                            if (decimal.Parse(icms.vCred, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                                stBuild.Append("O valor de crédito presumido no CT-e " + icms.vCred + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            }

                        }
                    }


                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90))
                {

                    if (preCTe.CST != "91")
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                        stBuild.Append("A CST 90 informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90 icms = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMS, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMS + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBC, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBC + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }


                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMS, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMS + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }

                        if (!string.IsNullOrWhiteSpace(icms.pRedBC))
                        {
                            if (decimal.Parse(icms.pRedBC, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBC + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(icms.vCred))
                        {
                            if (decimal.Parse(icms.vCred, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                stBuild.Append("O valor de crédito presumido no CT-e " + icms.vCred + " está diferente do informado no pré CT-e " + preCTe.ValorPresumido.ToString("f2", cultureInfo)).AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            }
                        }
                    }

                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {

                    if (preCTe.CST != "90")
                    {
                        stBuild.Append("A CST Outra UF informada no CT-e está diferente da informado no pré CT-e. CST: " + preCTe.CST).AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                    }
                    else
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF icms = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;

                        if (decimal.Parse(icms.pICMSOutraUF, cultureInfo) != preCTe.AliquotaICMS)
                        {
                            stBuild.Append("A aliquota do ICMS no CT-e " + icms.pICMSOutraUF + " está diferente da informado no pré CT-e " + preCTe.AliquotaICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.AliquotaICMS;
                        }

                        if (!ValidarValorComMargem(preCTe.BaseCalculoICMS, decimal.Parse(icms.vBCOutraUF, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("A base de calculo do ICMS no CT-e " + icms.vBCOutraUF + " está diferente da informado no pré CT-e " + preCTe.BaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                        }


                        if (!ValidarValorComMargem(preCTe.ValorICMS, decimal.Parse(icms.vICMSOutraUF, cultureInfo), configuracao, preCTe.Empresa))
                        {
                            stBuild.Append("O valor do ICMS no CT-e " + icms.vICMSOutraUF + " está diferente do informado no pré CT-e " + preCTe.ValorICMS.ToString("f2", cultureInfo)).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ICMS;
                        }


                        if (!string.IsNullOrWhiteSpace(icms.pRedBCOutraUF))
                        {
                            if (decimal.Parse(icms.pRedBCOutraUF, cultureInfo) != preCTe.PercentualReducaoBaseCalculoICMS)
                            {
                                stBuild.Append("O percentual de redução da base de calculo do ICMS no CT-e " + icms.pRedBCOutraUF + " está diferente do informado no pré CT-e " + preCTe.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)).AppendLine();
                                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.BaseCalculo;
                            }

                        }
                    }
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    if (preCTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCTe.CST != "40" && preCTe.CST != "41" && preCTe.CST != "51")
                    {
                        stBuild.Append("A CST " + preCTe.CST + " do pré CT-e é diferente da informada no CT-e" + preCTe.CST).AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CST;
                    }
                }
            }
            else
            {
                stBuild.Append("A tag  infCte.imp não foi localizada no xml do CT-e").AppendLine();
            }
        }

        private decimal BuscarFreteLiquidoComponente(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            decimal valorFreteLiquido = 0;
            foreach (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteVPrestComp comp in cteProc.CTe.infCte.vPrest.Comp)
            {
                if (comp.xNome.ToUpper().Trim() == "FRETE VALOR" || comp.xNome.ToUpper().Trim() == "VALOR FRETE" || Utilidades.String.RemoveDiacritics(comp.xNome).ToUpper().Trim() == "FRETE LIQUIDO")
                {
                    valorFreteLiquido = decimal.Parse(comp.vComp, cultureInfo);
                    break;
                }
            }
            if (valorFreteLiquido == 0)
                valorFreteLiquido = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo);

            return valorFreteLiquido;
        }

        private decimal BuscarFreteLiquidoComponente(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            decimal valorFreteLiquido = 0;
            foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp comp in cteProc.CTe.infCte.vPrest.Comp)
            {
                if (comp.xNome.ToUpper().Trim() == "FRETE VALOR" || comp.xNome.ToUpper().Trim() == "VALOR FRETE" || Utilidades.String.RemoveDiacritics(comp.xNome).ToUpper().Trim() == "FRETE LIQUIDO")
                {
                    valorFreteLiquido = decimal.Parse(comp.vComp, cultureInfo);
                    break;
                }
            }
            if (valorFreteLiquido == 0)
                valorFreteLiquido = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo);

            return valorFreteLiquido;
        }

        private void validarComponentes(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

            if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
            {
                if (!ValidarValorComMargem(preCTe.ValorPrestacaoServico, decimal.Parse(cteProc.CTe.infCte.vPrest.vTPrest, cultureInfo), configuracao, preCTe.Empresa))
                {
                    stBuild.Append("O valor da prestação do serviço do CT-e " + cteProc.CTe.infCte.vPrest.vTPrest + " está diferente do informado no pré CT-e " + preCTe.ValorPrestacaoServico.ToString("f2", cultureInfo)).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ValorFretePrestacao;
                }

                if (!ValidarValorComMargem(preCTe.ValorAReceber, decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo), configuracao, preCTe.Empresa))
                {
                    stBuild.Append("O valor a receber do CT-e " + cteProc.CTe.infCte.vPrest.vRec + " está diferente do informado no pré CT-e " + preCTe.ValorAReceber.ToString("f2", cultureInfo)).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ValorTotalReceber;
                }

                if (!configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
                {
                    if (preCTe.CFOP.CodigoCFOP != cteProc.CTe.infCte.ide.CFOP.ToInt())
                    {
                        stBuild.Append($"O CFOP do CT-e {cteProc.CTe.infCte.ide.CFOP} está diferente do informado no pré CT-e {preCTe.CFOP.CodigoCFOP}").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CFOP;
                    }
                }

                if (configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe && (preCTe.Empresa?.EmissaoDocumentosForaDoSistema ?? false) && !ValidarComponentesFretePreCTe(preCTe, cteProc.CTe.infCte.vPrest.Comp, configuracao, unitOfWork))
                {
                    stBuild.Append($"Os componentes de frete entre o Pré-CTe e o CT-e estão divergentes.").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ComponentesFreteDivergentes;
                }
            }
            else
            {
                if (!ValidarValorComMargem(preCTe.ValorAReceber, decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo), configuracao, preCTe.Empresa))
                {
                    stBuild.Append("O valor a receber do CT-e " + cteProc.CTe.infCte.vPrest.vRec + " está diferente do informado no pré CT-e " + preCTe.ValorAReceber.ToString("f2", cultureInfo)).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ValorTotalReceber;
                }
            }
        }

        private void validarComponentes(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

            if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
            {
                if (!ValidarValorComMargem(preCTe.ValorPrestacaoServico, decimal.Parse(cteProc.CTe.infCte.vPrest.vTPrest, cultureInfo), configuracao, preCTe.Empresa))
                {
                    stBuild.Append("O valor da prestação do serviço do CT-e " + cteProc.CTe.infCte.vPrest.vTPrest + " está diferente do informado no pré CT-e " + preCTe.ValorPrestacaoServico.ToString("f2", cultureInfo)).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ValorFretePrestacao;
                }

                if (!ValidarValorComMargem(preCTe.ValorAReceber, decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo), configuracao, preCTe.Empresa))
                {
                    stBuild.Append("O valor a receber do CT-e " + cteProc.CTe.infCte.vPrest.vRec + " está diferente do informado no pré CT-e " + preCTe.ValorAReceber.ToString("f2", cultureInfo)).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ValorTotalReceber;
                }

                if (!configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
                {
                    if (preCTe.CFOP.CodigoCFOP != cteProc.CTe.infCte.ide.CFOP.ToInt())
                    {
                        stBuild.Append($"O CFOP do CT-e {cteProc.CTe.infCte.ide.CFOP} está diferente do informado no pré CT-e {preCTe.CFOP.CodigoCFOP}").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CFOP;
                    }
                }

                if (configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe && (preCTe.Empresa?.EmissaoDocumentosForaDoSistema ?? false) && !ValidarComponentesFretePreCTe(preCTe, cteProc.CTe.infCte.vPrest.Comp, configuracao, unitOfWork))
                {
                    stBuild.Append($"Os componentes de frete entre o Pré-CTe e o CT-e estão divergentes.").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ComponentesFreteDivergentes;
                }
            }
            else
            {
                if (!ValidarValorComMargem(preCTe.ValorAReceber, decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo), configuracao, preCTe.Empresa))
                {
                    stBuild.Append("O valor a receber do CT-e " + cteProc.CTe.infCte.vPrest.vRec + " está diferente do informado no pré CT-e " + preCTe.ValorAReceber.ToString("f2", cultureInfo)).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.ValorTotalReceber;
                }
            }
        }

        private void validarComponentes(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

            if (!configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe)
            {
                if (!ValidarValorComMargem(preCTe.ValorPrestacaoServico, decimal.Parse(cteProc.CTe.infCte.vPrest.vTPrest, cultureInfo), configuracao, preCTe.Empresa))
                    stBuild.Append("O valor da prestação do serviço do CT-e " + cteProc.CTe.infCte.vPrest.vTPrest + " está diferente do informado no pré CT-e " + preCTe.ValorPrestacaoServico.ToString("f2", cultureInfo)).AppendLine();
            }

            if (!ValidarValorComMargem(preCTe.ValorAReceber, decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultureInfo), configuracao, preCTe.Empresa))
                stBuild.Append("O valor a receber do CT-e " + cteProc.CTe.infCte.vPrest.vRec + " está diferente do informado no pré CT-e " + preCTe.ValorAReceber.ToString("f2", cultureInfo)).AppendLine();

            //Repositorio.ComponentePrestacaoPreCTE repComponentesPrestacaoPreCTe = new Repositorio.ComponentePrestacaoPreCTE(unitOfWork);
            //List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPreCTe = repComponentesPrestacaoPreCTe.BuscarPorPreCTe(preCTe.Codigo);


            //decimal valorTotalComponentesCte = 0;
            //decimal valorTotalComponentesPreCte = (from obj in componentesPreCTe select obj.Valor).Sum();
            //foreach (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteVPrestComp comp in cteProc.CTe.infCte.vPrest.Comp)
            //{
            //    valorTotalComponentesCte += decimal.Parse(comp.vComp, cultureInfo);
            //}

            //if (valorTotalComponentesCte != valorTotalComponentesPreCte)
            //    stBuild.Append("O valor total dos componentes do CT-e " + valorTotalComponentesCte.ToString("f2", cultureInfo) + " está diferente do informado no pré CT-e " + valorTotalComponentesPreCte.ToString("f2", cultureInfo)).AppendLine();

        }

        private void validarParticipantes(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (!configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
            {
                string cnpjEmissor = (preCTe.Empresa != null) ? preCTe.Empresa.CNPJ_SemFormato : preCTe.TransportadorTerceiro.CPF_CNPJ_SemFormato;
                string cnpjCTe = cteProc.CTe.infCte.emit.CNPJ;
                string cnpjPreCTe = cnpjEmissor;

                if (configuracao.ValidarPorRaizDoTransportadorNaImportacaoCTe)
                {
                    cnpjCTe = Utilidades.String.OnlyNumbers(cnpjCTe).Substring(0, 8);
                    cnpjPreCTe = Utilidades.String.OnlyNumbers(cnpjPreCTe).Substring(0, 8);
                }

                if (cnpjCTe != cnpjPreCTe)
                {
                    stBuild.Append("O emitente do CT-e " + cteProc.CTe.infCte.emit.xNome + "(" + cteProc.CTe.infCte.emit.CNPJ + ") é diferente do esperado " + (preCTe.Empresa != null ? preCTe.Empresa.RazaoSocial + " (" + preCTe.Empresa.CNPJ_Formatado + ")" : preCTe.TransportadorTerceiro.Nome + "(" + cnpjEmissor + ")")).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Emissor;
                }

                if (cteProc.CTe.infCte.rem != null && preCTe.Remetente != null && cteProc.CTe.infCte.rem.Item != preCTe.Remetente.CPF_CNPJ_SemFormato)
                {
                    stBuild.Append("O remetente do CT-e " + cteProc.CTe.infCte.rem.xNome + "(" + cteProc.CTe.infCte.rem.Item + ") é diferente do esperado " + preCTe.Remetente.Nome + "(" + preCTe.Remetente.CPF_CNPJ_SemFormato + ")").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Remetente;
                }

                if (preCTe.Destinatario != null && cteProc.CTe.infCte.dest != null && cteProc.CTe.infCte.dest.Item != preCTe.Destinatario.CPF_CNPJ_SemFormato)
                {
                    stBuild.Append("O destinatário do CT-e " + cteProc.CTe.infCte.dest.xNome + "(" + cteProc.CTe.infCte.dest.Item + ") é diferente do esperado " + preCTe.Destinatario.Nome + "(" + preCTe.Destinatario.CPF_CNPJ_SemFormato + ")").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Destinatario;
                }
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.Normal && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item0)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item1)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item2)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item3)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (cteProc.CTe.infCte.exped != null || preCTe.Expedidor != null && preCTe.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
            {
                if (cteProc.CTe.infCte.exped == null && preCTe.Expedidor != null)
                {
                    stBuild.Append("O expedidor " + preCTe.Expedidor.Nome + " não foi informado no CT-e").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Expedidor;
                }

                if (cteProc.CTe.infCte.exped != null && preCTe.Expedidor == null)
                {
                    if (cteProc.CTe.infCte.exped.Item != cteProc.CTe.infCte.rem.Item)
                    {
                        stBuild.Append("O expedidor " + cteProc.CTe.infCte.exped.xNome + " não é esperado").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Expedidor;
                    }
                }

                if ((cteProc.CTe.infCte.exped != null && preCTe.Expedidor != null) && cteProc.CTe.infCte.exped.Item != preCTe.Expedidor.CPF_CNPJ_SemFormato)
                {
                    stBuild.Append("O expedidor do CT-e " + cteProc.CTe.infCte.exped.xNome + "(" + cteProc.CTe.infCte.exped.Item + ") é diferente do esperado " + preCTe.Expedidor.Nome + "(" + preCTe.Expedidor.CPF_CNPJ_SemFormato + ")").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Expedidor;
                }
            }

            //if (cteProc.CTe.infCte.receb != null || preCTe.Recebedor != null)
            //{
            //    if (cteProc.CTe.infCte.receb == null && preCTe.Recebedor != null)
            //        stBuild.Append("O recebedor " + preCTe.Recebedor.Nome + " não foi informado no CT-e").AppendLine();
            //    if (cteProc.CTe.infCte.receb != null && preCTe.Recebedor == null)
            //        stBuild.Append("O recebedor " + cteProc.CTe.infCte.receb.xNome + " não existe no pré CT-e").AppendLine();

            //    if ((cteProc.CTe.infCte.receb != null && preCTe.Recebedor != null) && cteProc.CTe.infCte.receb.Item != preCTe.Recebedor.CPF_CNPJ_SemFormato)
            //        stBuild.Append("O recebedor do CT-e " + cteProc.CTe.infCte.receb.xNome + "(" + cteProc.CTe.infCte.receb.Item + ") é diferente do informado no pré CT-e " + preCTe.Recebedor.Nome + "(" + preCTe.Recebedor.CPF_CNPJ_SemFormato + ")").AppendLine();
            //}

            if (!configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
            {
                if (cteProc.CTe.infCte.ide.Item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    if (preCTe.TipoTomador != Dominio.Enumeradores.TipoTomador.Outros)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + ((MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item).toma + " está diferente do esperado Item4").AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 toma4 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;
                        if (toma4.Item != preCTe.Tomador.CPF_CNPJ_SemFormato)
                        {
                            stBuild.Append("O tomador do CTe " + toma4.xNome + " está diferente do informado no pré CTe " + preCTe.Tomador.Nome).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        }
                    }
                }
                else
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 toma3 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;
                    if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && toma3.toma != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item0").AppendLine();
                    }
                    else if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && toma3.toma != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item1").AppendLine();
                    }
                    else if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && toma3.toma != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item2").AppendLine();
                    }
                    else if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && toma3.toma != MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item3").AppendLine();
                    }
                }
            }
        }

        private void validarParticipantes(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, ref Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento motivoInconsistencia, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (!configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
            {
                string cnpjEmissor = (preCTe.Empresa != null) ? preCTe.Empresa.CNPJ_SemFormato : preCTe.TransportadorTerceiro.CPF_CNPJ_SemFormato;
                string cnpjCTe = cteProc.CTe.infCte.emit.Item;
                string cnpjPreCTe = cnpjEmissor;

                if (configuracao.ValidarPorRaizDoTransportadorNaImportacaoCTe)
                {
                    cnpjCTe = Utilidades.String.OnlyNumbers(cnpjCTe).Substring(0, 8);
                    cnpjPreCTe = Utilidades.String.OnlyNumbers(cnpjPreCTe).Substring(0, 8);
                }

                if (cnpjCTe != cnpjPreCTe)
                {
                    stBuild.Append("O emitente do CT-e " + cteProc.CTe.infCte.emit.xNome + "(" + cteProc.CTe.infCte.emit.Item + ") é diferente do esperado " + (preCTe.Empresa != null ? preCTe.Empresa.RazaoSocial + " (" + preCTe.Empresa.CNPJ_Formatado + ")" : preCTe.TransportadorTerceiro.Nome + "(" + cnpjEmissor + ")")).AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Emissor;
                }

                if (cteProc.CTe.infCte.rem != null && preCTe.Remetente != null && cteProc.CTe.infCte.rem.Item != preCTe.Remetente.CPF_CNPJ_SemFormato)
                {
                    stBuild.Append("O remetente do CT-e " + cteProc.CTe.infCte.rem.xNome + "(" + cteProc.CTe.infCte.rem.Item + ") é diferente do esperado " + preCTe.Remetente.Nome + "(" + preCTe.Remetente.CPF_CNPJ_SemFormato + ")").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Remetente;
                }

                if (preCTe.Destinatario != null && cteProc.CTe.infCte.dest != null && cteProc.CTe.infCte.dest.Item != preCTe.Destinatario.CPF_CNPJ_SemFormato)
                {
                    stBuild.Append("O destinatário do CT-e " + cteProc.CTe.infCte.dest.xNome + "(" + cteProc.CTe.infCte.dest.Item + ") é diferente do esperado " + preCTe.Destinatario.Nome + "(" + preCTe.Destinatario.CPF_CNPJ_SemFormato + ")").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Destinatario;
                }
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.Normal && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item0)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item1)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item2)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (preCTe.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario && cteProc.CTe.infCte.ide.tpServ != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ.Item3)
            {
                stBuild.Append("O tipo de serviço informado " + cteProc.CTe.infCte.ide.tpServ.ToString() + " é deferente do esperado " + preCTe.DescricaoTipoServico).AppendLine();
                motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.TipoCTe;
            }

            if (cteProc.CTe.infCte.exped != null || preCTe.Expedidor != null && preCTe.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
            {
                if (cteProc.CTe.infCte.exped == null && preCTe.Expedidor != null)
                {
                    stBuild.Append("O expedidor " + preCTe.Expedidor.Nome + " não foi informado no CT-e").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Expedidor;
                }

                if (cteProc.CTe.infCte.exped != null && preCTe.Expedidor == null)
                {
                    if (cteProc.CTe.infCte.exped.Item != cteProc.CTe.infCte.rem.Item)
                    {
                        stBuild.Append("O expedidor " + cteProc.CTe.infCte.exped.xNome + " não é esperado").AppendLine();
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Expedidor;
                    }
                }

                if ((cteProc.CTe.infCte.exped != null && preCTe.Expedidor != null) && cteProc.CTe.infCte.exped.Item != preCTe.Expedidor.CPF_CNPJ_SemFormato)
                {
                    stBuild.Append("O expedidor do CT-e " + cteProc.CTe.infCte.exped.xNome + "(" + cteProc.CTe.infCte.exped.Item + ") é diferente do esperado " + preCTe.Expedidor.Nome + "(" + preCTe.Expedidor.CPF_CNPJ_SemFormato + ")").AppendLine();
                    motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Expedidor;
                }
            }

            //if (cteProc.CTe.infCte.receb != null || preCTe.Recebedor != null)
            //{
            //    if (cteProc.CTe.infCte.receb == null && preCTe.Recebedor != null)
            //        stBuild.Append("O recebedor " + preCTe.Recebedor.Nome + " não foi informado no CT-e").AppendLine();
            //    if (cteProc.CTe.infCte.receb != null && preCTe.Recebedor == null)
            //        stBuild.Append("O recebedor " + cteProc.CTe.infCte.receb.xNome + " não existe no pré CT-e").AppendLine();

            //    if ((cteProc.CTe.infCte.receb != null && preCTe.Recebedor != null) && cteProc.CTe.infCte.receb.Item != preCTe.Recebedor.CPF_CNPJ_SemFormato)
            //        stBuild.Append("O recebedor do CT-e " + cteProc.CTe.infCte.receb.xNome + "(" + cteProc.CTe.infCte.receb.Item + ") é diferente do informado no pré CT-e " + preCTe.Recebedor.Nome + "(" + preCTe.Recebedor.CPF_CNPJ_SemFormato + ")").AppendLine();
            //}

            if (!configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
            {
                if (cteProc.CTe.infCte.ide.Item.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    if (preCTe.TipoTomador != Dominio.Enumeradores.TipoTomador.Outros)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + ((MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item).toma + " está diferente do esperado Item4").AppendLine();
                    }
                    else
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 toma4 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;
                        if (toma4.Item != preCTe.Tomador.CPF_CNPJ_SemFormato)
                        {
                            stBuild.Append("O tomador do CTe " + toma4.xNome + " está diferente do informado no pré CTe " + preCTe.Tomador.Nome).AppendLine();
                            motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        }
                    }
                }
                else
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 toma3 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;
                    if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && toma3.toma != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item0").AppendLine();
                    }
                    else if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && toma3.toma != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item1").AppendLine();
                    }
                    else if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && toma3.toma != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item2").AppendLine();
                    }
                    else if (preCTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && toma3.toma != MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                    {
                        motivoInconsistencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.Tomador;
                        stBuild.Append("O tipo do tomador no CTe " + toma3.toma + " está diferente do esperado Item3").AppendLine();
                    }
                }
            }
        }

        private void validarParticipantes(ref System.Text.StringBuilder stBuild, MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (configuracao.NaoValidarDadosParticipantesNaImportacaoCTe)
                return;

            string cnpjEmissor = (preCTe.Empresa != null) ? preCTe.Empresa.CNPJ_SemFormato : preCTe.TransportadorTerceiro.CPF_CNPJ_SemFormato;

            if (cteProc.CTe.infCte.emit.CNPJ != cnpjEmissor)
            {
                stBuild.Append("O emitente do CT-e " + cteProc.CTe.infCte.emit.xNome + "(" + cteProc.CTe.infCte.emit.CNPJ + ") é diferente do esperado " + (preCTe.Empresa != null ? preCTe.Empresa.RazaoSocial + " (" + preCTe.Empresa.CNPJ_Formatado + ")" : preCTe.TransportadorTerceiro.Nome + "(" + cnpjEmissor + ")")).AppendLine();
            }

            if (cteProc.CTe.infCte.rem.Item != preCTe.Remetente.CPF_CNPJ_SemFormato)
                stBuild.Append("O remetente do CT-e " + cteProc.CTe.infCte.rem.xNome + "(" + cteProc.CTe.infCte.rem.Item + ") é diferente do esperado " + preCTe.Remetente.Nome + "(" + preCTe.Remetente.CPF_CNPJ_SemFormato + ")").AppendLine();

            if (preCTe.Destinatario != null && cteProc.CTe.infCte.dest.Item != preCTe.Destinatario.CPF_CNPJ_SemFormato)
                stBuild.Append("O destinatário do CT-e " + cteProc.CTe.infCte.dest.xNome + "(" + cteProc.CTe.infCte.dest.Item + ") é diferente do esperado " + preCTe.Destinatario.Nome + "(" + preCTe.Destinatario.CPF_CNPJ_SemFormato + ")").AppendLine();

            if (cteProc.CTe.infCte.exped != null || preCTe.Expedidor != null)
            {
                if (cteProc.CTe.infCte.exped == null && preCTe.Expedidor != null)
                    stBuild.Append("O expedidor " + preCTe.Expedidor.Nome + " não foi informado no CT-e").AppendLine();
                if (cteProc.CTe.infCte.exped != null && preCTe.Expedidor == null)
                {
                    if (cteProc.CTe.infCte.exped.Item != cteProc.CTe.infCte.rem.Item)
                        stBuild.Append("O expedidor " + cteProc.CTe.infCte.exped.xNome + " não é esperado").AppendLine();
                }

                if ((cteProc.CTe.infCte.exped != null && preCTe.Expedidor != null) && cteProc.CTe.infCte.exped.Item != preCTe.Expedidor.CPF_CNPJ_SemFormato)
                    stBuild.Append("O expedidor do CT-e " + cteProc.CTe.infCte.exped.xNome + "(" + cteProc.CTe.infCte.exped.Item + ") é diferente do esperado " + preCTe.Expedidor.Nome + "(" + preCTe.Expedidor.CPF_CNPJ_SemFormato + ")").AppendLine();
            }

            //if (cteProc.CTe.infCte.receb != null || preCTe.Recebedor != null)
            //{
            //    if (cteProc.CTe.infCte.receb == null && preCTe.Recebedor != null)
            //        stBuild.Append("O recebedor " + preCTe.Recebedor.Nome + " não foi informado no CT-e").AppendLine();
            //    if (cteProc.CTe.infCte.receb != null && preCTe.Recebedor == null)
            //        stBuild.Append("O recebedor " + cteProc.CTe.infCte.receb.xNome + " não existe no pré CT-e").AppendLine();

            //    if ((cteProc.CTe.infCte.receb != null && preCTe.Recebedor != null) && cteProc.CTe.infCte.receb.Item != preCTe.Recebedor.CPF_CNPJ_SemFormato)
            //        stBuild.Append("O recebedor do CT-e " + cteProc.CTe.infCte.receb.xNome + "(" + cteProc.CTe.infCte.receb.Item + ") é diferente do informado no pré CT-e " + preCTe.Recebedor.Nome + "(" + preCTe.Recebedor.CPF_CNPJ_SemFormato + ")").AppendLine();
            //}

            if (cteProc.CTe.infCte.ide.Item.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4) && preCTe.TipoTomador != Dominio.Enumeradores.TipoTomador.Outros)
            {
                stBuild.Append("O tipo do tomador no CTe " + ((MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item).toma + " está diferente do esperado " + preCTe.TipoTomador).AppendLine();
            }
            else
            {
                if (cteProc.CTe.infCte.ide.Item.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 toma4 = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;
                    if (toma4.toma != (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4Toma)preCTe.TipoTomador)
                        stBuild.Append("O tipo do tomador no CTe " + ((MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item).toma + " está diferente do esperado " + preCTe.TipoTomador).AppendLine();

                    if (toma4.Item != preCTe.Tomador.CPF_CNPJ_SemFormato)
                        stBuild.Append("O tomador do CTe " + toma4.xNome + " está diferente do informado no pré CTe " + preCTe.Tomador.Nome).AppendLine();
                }
            }
        }

        private void preencherDadosDocumentosAnteriores(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosAnteriores = repDocumentoDeTransporteAnteriorPreCTE.BuscarPorPreCte(preCTe.Codigo);

            if (documentosAnteriores.Count > 0)
            {
                stBuilder.Append("<b>DOCUMENTOS ANTERIORES</b>").AppendLine();
                foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE documento in documentosAnteriores)
                {
                    stBuilder.Append("Chave:" + documento.Chave).AppendLine();
                }
                stBuilder.AppendLine();
            }
        }

        private void preencherDadosNotasFiscais(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosPreCTE repDocumentosPreCTE = new Repositorio.DocumentosPreCTE(unitOfWork);
            List<Dominio.Entidades.DocumentosPreCTE> documentosPreCTE = repDocumentosPreCTE.BuscarPorPreCte(preCTe.Codigo);

            stBuilder.Append("<b>DOCUMENTOS (NF-E)</b>").AppendLine();
            foreach (Dominio.Entidades.DocumentosPreCTE documento in documentosPreCTE)
            {
                stBuilder.Append("Chave:" + documento.ChaveNFE).AppendLine();
            }
            stBuilder.AppendLine();
        }

        private void preencherDadosVeiculo(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.VeiculoPreCTE repVeiculoPreCTE = new Repositorio.VeiculoPreCTE(unitOfWork);
            List<Dominio.Entidades.VeiculoPreCTE> veiculosPreCTE = repVeiculoPreCTE.BuscarPorPreCte(preCTe.Codigo);

            if (veiculosPreCTE.Count > 0)
            {
                stBuilder.Append("<b>VEÍCULOS</b>").AppendLine();
                foreach (Dominio.Entidades.VeiculoPreCTE veiculo in veiculosPreCTE)
                {

                    stBuilder.Append("Placa:" + veiculo.Placa).AppendLine();
                    stBuilder.Append("RENAVAM:" + veiculo.RENAVAM).AppendLine();
                    stBuilder.Append("UF:" + veiculo.Estado.Sigla).AppendLine().AppendLine();
                }
            }

            Repositorio.MotoristaPreCTE repMotoristaPreCTE = new Repositorio.MotoristaPreCTE(unitOfWork);
            List<Dominio.Entidades.MotoristaPreCTE> motoristasPreCTE = repMotoristaPreCTE.BuscarPorPreCte(preCTe.Codigo);

            if (veiculosPreCTE.Count > 0)
            {
                stBuilder.Append("<b>MOTORISTAS</b>").AppendLine();
                foreach (Dominio.Entidades.MotoristaPreCTE motorista in motoristasPreCTE)
                {
                    stBuilder.Append("Nome:" + motorista.NomeMotorista).AppendLine();
                    stBuilder.Append("CPF:" + motorista.CPFMotorista).AppendLine().AppendLine();
                }
            }

        }

        private void preencherDadosDoFrete(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.InformacaoCargaPreCTE repInformacaoCargaPreCte = new Repositorio.InformacaoCargaPreCTE(unitOfWork);
            List<Dominio.Entidades.InformacaoCargaPreCTE> informacoes = repInformacaoCargaPreCte.BuscarPorPreCTe(preCTe.Codigo);

            stBuilder.Append("<b>DADOS DO FRETE</b>").AppendLine();
            stBuilder.Append("Valor da Prestação:" + preCTe.ValorPrestacaoServico.ToString("n2")).AppendLine();
            stBuilder.Append("Total a Receber:" + preCTe.ValorAReceber.ToString("n2")).AppendLine();
            stBuilder.Append("<b>IMPOSTOS</b>").AppendLine();
            stBuilder.Append("CST:" + preCTe.CST).AppendLine();
            stBuilder.Append("Alíquota:" + preCTe.AliquotaICMS.ToString("n2")).AppendLine();
            stBuilder.Append("Base de Calculo:" + preCTe.BaseCalculoICMS.ToString("n2")).AppendLine();
            stBuilder.Append("Valor ICMS:" + preCTe.ValorICMS.ToString("n2")).AppendLine();
            if (preCTe.PercentualReducaoBaseCalculoICMS > 0)
                stBuilder.Append("Percentual Redução:" + preCTe.PercentualReducaoBaseCalculoICMS.ToString("n2")).AppendLine();

            if (!string.IsNullOrEmpty(preCTe.ObservacoesGerais))
                stBuilder.Append("Observações:" + preCTe.ObservacoesGerais).AppendLine();

            stBuilder.AppendLine();

        }

        private void preencherDadosMercadoria(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.InformacaoCargaPreCTE repInformacaoCargaPreCte = new Repositorio.InformacaoCargaPreCTE(unitOfWork);
            List<Dominio.Entidades.InformacaoCargaPreCTE> informacoes = repInformacaoCargaPreCte.BuscarPorPreCTe(preCTe.Codigo);

            stBuilder.Append("<b>MERCADORIA TRANSPORTADA</b>").AppendLine();
            stBuilder.Append("Valor:" + preCTe.ValorTotalMercadoria.ToString("n2")).AppendLine();
            stBuilder.Append("Peso (kg):" + (from obj in informacoes select obj.Quantidade).Sum().ToString("n2")).AppendLine().AppendLine();
        }

        private void preencherSeguroCarga(ref System.Text.StringBuilder stBuilder, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.SeguroPreCTE repSeguroCTe = new Repositorio.SeguroPreCTE(unitOfWork);
            List<Dominio.Entidades.SeguroPreCTE> segurosPreCTe = repSeguroCTe.BuscarPorPreCte(preCTe.Codigo);

            foreach (Dominio.Entidades.SeguroPreCTE serguroPreCte in segurosPreCTe)
            {
                if (!string.IsNullOrEmpty(serguroPreCte.NomeSeguradora))
                {
                    stBuilder.Append("<b>DADOS DO SEGURO DA CARGA</b>").AppendLine();
                    stBuilder.Append("Seguradora:" + serguroPreCte.NomeSeguradora).AppendLine();
                    stBuilder.Append("Apólice:" + serguroPreCte.NumeroApolice).AppendLine();
                    stBuilder.Append("Responsável:" + serguroPreCte.DescricaoTipo).AppendLine().AppendLine();
                }

            }

        }

        #endregion
    }
}
