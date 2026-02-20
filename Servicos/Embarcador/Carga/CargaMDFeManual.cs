using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CargaMDFeManual
    {

        public void VincularMDfesACarga(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao repCargaMDFeAquaviarioManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao(unidadeTrabalho);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
                Servicos.Embarcador.Carga.MDFe servicoMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();                

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoSemIntegracao = null;
                Dominio.Entidades.LayoutEDI layoutEDIFiscal = null;

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargaMDFeManual.CTes.Select(o => o.Carga).Distinct().ToList();
                cargas.AddRange(cargaMDFeManual.Cargas.Select(o => o).Distinct().ToList());
                cargas = cargas.Distinct().ToList();

                Servicos.Embarcador.Carga.Impressao serImpressao = new Impressao(unidadeTrabalho);
                Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unidadeTrabalho);

                unidadeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe in cargaMDFeManual.MDFeManualMDFes)
                {
                    bool possuiEDIFiscal = false;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursos = cargaMDFeManual.Percursos.ToList();
                    List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
                    if (cargaMDFeManual.UsarListaDestinos())
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> destinosMDF = repCargaMDFeManualDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
                        destinos = destinosMDF.Select(o => o.Localidade).ToList();
                    }
                    else
                    {
                        destinos.Add(cargaMDFeManual.Destino);
                    }

                    bool exigeEDIFiscalMT = cargaMDFeManual.Empresa?.Configuracao?.ExigeEDIFiscalMT ?? false;

                    if (exigeEDIFiscalMT && percursos.Any(o => o.Estado.Sigla == "MT") || (cargaMDFeManual.Origem.Estado.Sigla != "MT" && destinos.Any(obj => obj.Estado.Sigla == "MT")))
                    {
                        layoutEDIFiscal = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.FISCAL, true);

                        if (layoutEDIFiscal != null)
                            possuiEDIFiscal = true;
                    }

                    bool possuiEDIUVTRN = destinos.Any(o => o.Estado.Sigla == "RN");
                    Dominio.Entidades.LayoutEDI layoutEDIUVTRN = possuiEDIUVTRN ? repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.UVT_RN, true) : null;

                    possuiEDIUVTRN = layoutEDIUVTRN == null;

                    if (possuiEDIUVTRN || possuiEDIFiscal)
                    {
                        tipoIntegracaoSemIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao);

                        if (tipoIntegracaoSemIntegracao == null)
                            possuiEDIFiscal = possuiEDIUVTRN = false;
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFeECarga(cargaMDFeManualMDFe.MDFe.Codigo, carga.Codigo);

                        if (cargaMDFe != null)
                            continue;

                        cargaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe()
                        {
                            Carga = carga,
                            MDFe = cargaMDFeManualMDFe.MDFe,
                            SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe
                        };

                        repCargaMDFe.Inserir(cargaMDFe);

                        servicoMDFe.ReplicarMDFeParaCargaDT(cargaMDFe, unidadeTrabalho);

                        if (possuiEDIFiscal && !repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracaoSemIntegracao.Codigo, layoutEDIFiscal.Codigo, 0D))
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao()
                            {
                                Carga = carga,
                                DataIntegracao = DateTime.Now,
                                NumeroTentativas = 0,
                                ProblemaIntegracao = string.Empty,
                                TipoIntegracao = tipoIntegracaoSemIntegracao,
                                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                                LayoutEDI = layoutEDIFiscal
                            };

                            repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);
                        }

                        if (possuiEDIUVTRN && layoutEDIUVTRN != null && !repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracaoSemIntegracao.Codigo, layoutEDIUVTRN.Codigo, 0))
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao()
                            {
                                Carga = carga,
                                DataIntegracao = DateTime.Now,
                                NumeroTentativas = 0,
                                ProblemaIntegracao = string.Empty,
                                TipoIntegracao = tipoIntegracaoSemIntegracao,
                                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                                LayoutEDI = layoutEDIUVTRN
                            };

                            repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);
                        }

                        carga.NaoGerarMDFe = false;

                        if (configuracaoTMS.AtualizarCargaComVeiculoMDFeManual)
                        {
                            carga.Veiculo = cargaMDFeManual.Veiculo;

                            servicoCargaMotorista.AtualizarMotoristas(carga, cargaMDFeManual.Motoristas?.ToList());

                            if (configuracaoMonitoramento?.AtualizarMonitoramentoAoGerarMDFeManual ?? false)
                                Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoriaPorCarga(carga, configuracaoTMS, null, "Geração de MDF-e Manual", unidadeTrabalho); //se existe monitoramento para a carga e esta iniciado vai substituir. senao existir cria um novo
                        }

                        repCarga.Atualizar(carga);
                    }

                    cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.AgIntegracao;
                    cargaMDFeManual.GerandoIntegracoes = true;
                    repCargaMDFeManual.Atualizar(cargaMDFeManual);

                    if (cargaMDFeManual.MDFeRecebidoDeIntegracao != true && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab);
                        if (tipoIntegracao != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao cargaMDFeAquaviarioManual = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao()
                            {
                                CargaMDFeManual = cargaMDFeManual,
                                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                                TipoIntegracao = tipoIntegracao,
                                DataIntegracao = DateTime.Now,
                                NumeroTentativas = 0,
                                ProblemaIntegracao = ""
                            };
                            repCargaMDFeAquaviarioManualIntegracao.Inserir(cargaMDFeAquaviarioManual);
                        }
                    }

                    serCargaDadosSumarizados.AtualizarDadosMDFeAquaviario(cargaMDFeManual.Codigo, unidadeTrabalho);

                }

                unidadeTrabalho.CommitChanges();

                if (cargas != null && cargas.Count > 0)
                {
                    if (configuracaoTMS.EnviarMDFeAutomaticamenteParaImpressao)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaImpressao = repCarga.BuscarPorCodigo(cargas.FirstOrDefault().Codigo);
                        if (cargaImpressao != null)
                            serImpressao.EnviarMDFeParaImpressao(cargaImpressao);
                    }

                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogMDFe);
                    if (tipoIntegracao != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegragao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(cargas.FirstOrDefault().Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog);
                        if (cargaIntegragao != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao()
                            {
                                Carga = cargas.FirstOrDefault(), //Gera registro apenas para a primeira carga pois o retorno para a Magalu é para o MDFe Manual
                                DataIntegracao = DateTime.Now,
                                NumeroTentativas = 0,
                                ProblemaIntegracao = string.Empty,
                                TipoIntegracao = tipoIntegracao,
                                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                            };
                            
                            repCargaCargaIntegracao.Inserir(cargaCargaIntegracao);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                throw;
            }
        }
    }
}
