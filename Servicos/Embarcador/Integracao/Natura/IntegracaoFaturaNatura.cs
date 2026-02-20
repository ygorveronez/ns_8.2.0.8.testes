using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Natura
{
    public class IntegracaoFaturaNatura
    {
        public void ConsultarPreFaturas(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, long numeroPreFatura, DateTime dataInicial, DateTime dataFinal, bool atualizarPreFatura, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracaoNatura = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFaturaNatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.ItemPreFaturaNatura repItemPreFaturaNatura = new Repositorio.Embarcador.Integracao.ItemPreFaturaNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
                throw new Exception("Os dados para a integração com a Natura não estão configurados.");

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OBClient svcPreFatura = IntegracaoDTNatura.ObterClientNatura<Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OBClient, Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OB>(configuracaoIntegracao.UsuarioNatura, configuracaoIntegracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaPreFatura, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspector);

            var dados = new Servicos.ServicoNatura.ProcessaPreFatura.DT_EnviaParamPreFaturaDados()
            {
                codTranspMatriz = empresa.Configuracao.CodigoFilialNatura // empresa.Configuracao.CodigoMatrizNatura
            };

            if (numeroPreFatura > 0)
                dados.numeroPreFatura = numeroPreFatura.ToString();
            else
            {
                if (dataInicial != DateTime.MinValue)
                    dados.dataDe = dataInicial.ToString("yyyy-MM-dd");

                if (dataFinal != DateTime.MinValue)
                    dados.dataAte = dataFinal.ToString("yyyy-MM-dd");
            }

            if (svcPreFatura.Endpoint.Address.ToString().Contains("qxx.transportes.natura.com.br"))
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            ServicoNatura.ProcessaPreFatura.DT_RecebePreFaturaDados[] retorno = null;

            try
            {
                retorno = svcPreFatura.SI_ProcessaPreFaturaSync_OB(new Servicos.ServicoNatura.ProcessaPreFatura.DT_EnviaParamPreFatura() { dados = dados });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura integracaoNatura = new Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura();

            integracaoNatura.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
            integracaoNatura.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
            integracaoNatura.DataConsulta = DateTime.Now;

            if (dataFinal != DateTime.MinValue)
                integracaoNatura.ParametroDataFinal = dataFinal;

            if (dataInicial != DateTime.MinValue)
                integracaoNatura.ParametroDataInicial = dataInicial;

            if (numeroPreFatura > 0)
                integracaoNatura.ParametroNumero = numeroPreFatura;

            integracaoNatura.Usuario = usuario;
            integracaoNatura.Protocolo = null;
            integracaoNatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoNatura.Sucesso;
            integracaoNatura.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.Fatura;
            integracaoNatura.Retorno = "Consulta de faturas realizada com sucesso.";

            repIntegracaoNatura.Inserir(integracaoNatura);

            if (retorno == null || retorno[0].numeroPreFatura == null)
                return;

            List<long> numerosPreFatura = (from obj in retorno select long.Parse(obj.numeroPreFatura)).Distinct().ToList();

            foreach (long numPreFatura in numerosPreFatura)
            {
                List<ServicoNatura.ProcessaPreFatura.DT_RecebePreFaturaDados> preFaturas = (from obj in retorno where obj.numeroPreFatura == numPreFatura.ToString() select obj).ToList();

                Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFatura = repPreFaturaNatura.BuscarPorPreFatura(numPreFatura);

                bool novaPreFatura = false;

                if (preFatura == null)
                {
                    novaPreFatura = true;
                    preFatura = new Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura();

                    preFatura.Empresa = empresa;
                    preFatura.DataPreFatura = DateTime.ParseExact(preFaturas.First().dataPreFatura, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None);
                    preFatura.NumeroPreFatura = numPreFatura;
                    preFatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura.Atualizando;
                    preFatura.MensagemSituacao = "";

                    repPreFaturaNatura.Inserir(preFatura);
                }
                else
                {
                    if (preFatura.Situacao == SituacaoPreFaturaNatura.Pendente || preFatura.Situacao == SituacaoPreFaturaNatura.Falha)
                    {
                        preFatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura.Atualizando;

                        repPreFaturaNatura.Atualizar(preFatura);
                    }
                }

                decimal valorFrete = 0m;

                if (novaPreFatura || atualizarPreFatura)
                {
                    foreach (ServicoNatura.ProcessaPreFatura.DT_RecebePreFaturaDados preFat in preFaturas)
                    {
                        foreach (var item in preFat.itens)
                        {
                            long.TryParse(item.transporte.docTransporte, out long numeroDT);
                            int.TryParse(item.numeroDocFiscal, out int numeroDocFiscal);
                            int.TryParse(item.serieDocFiscal, out int serieDocFiscal);

                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = null;

                            if (item.modeloDocFiscal == 7)
                                cargaCTe = repCargaCTe.BuscarPorNumeroSerieETipoDocumento(numeroDocFiscal, serieDocFiscal, Dominio.Enumeradores.TipoDocumento.NFSe, new string[] { "A" }, item.docFiscalRef.Select(o => o.chaveNFeCTe).ToArray());
                            else
                                cargaCTe = repCargaCTe.BuscarPorChaveCTeSituacao(item.chaveCTe, new string[] { "A" });

                            Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDTNatura.BuscarPorNumero(numeroDT);

                            if (cargaCTe == null)
                                continue;

                            if (dtNatura == null)
                                continue;

                            Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura itemPreFatura = repItemPreFaturaNatura.BuscarPorCargaCTe(preFatura.Codigo, cargaCTe.Codigo);

                            if (itemPreFatura != null)
                                continue;

                            unidadeDeTrabalho.Start();

                            itemPreFatura = new Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura();

                            itemPreFatura.AliquotaCOFINS = item.impostos.aliquotaCOFINS;
                            itemPreFatura.AliquotaICMS = item.impostos.aliquotaICMS;
                            itemPreFatura.AliquotaICMSST = item.impostos.aliquotaICMSST;
                            itemPreFatura.AliquotaISS = item.impostos.aliquotaISS;
                            itemPreFatura.AliquotaPIS = item.impostos.aliquotaPIS;
                            itemPreFatura.ValorDoDesconto = string.IsNullOrWhiteSpace(item.transporte.difValorFrete) ? 0 : decimal.Parse(item.transporte.difValorFrete, cultura);
                            itemPreFatura.BaseCalculoCOFINS = string.IsNullOrWhiteSpace(item.impostos.baseCOFINS) ? 0 : decimal.Parse(item.impostos.baseCOFINS, cultura);
                            itemPreFatura.BaseCalculoICMS = string.IsNullOrWhiteSpace(item.impostos.baseICMS) ? 0 : decimal.Parse(item.impostos.baseICMS, cultura);
                            itemPreFatura.BaseCalculoICMSST = string.IsNullOrWhiteSpace(item.impostos.baseICMSST) ? 0 : decimal.Parse(item.impostos.baseICMSST, cultura);
                            itemPreFatura.BaseCalculoISS = string.IsNullOrWhiteSpace(item.impostos.baseISS) ? 0 : decimal.Parse(item.impostos.baseISS, cultura);
                            itemPreFatura.BaseCalculoPIS = string.IsNullOrWhiteSpace(item.impostos.basePIS) ? 0 : decimal.Parse(item.impostos.basePIS, cultura);
                            itemPreFatura.IVAICMSST = string.IsNullOrWhiteSpace(item.impostos.ivaICMSST) ? 0 : decimal.Parse(item.impostos.ivaICMSST, cultura);
                            itemPreFatura.ValorCOFINS = string.IsNullOrWhiteSpace(item.impostos.valorCOFINS) ? 0 : decimal.Parse(item.impostos.valorCOFINS, cultura);
                            itemPreFatura.ValorICMS = string.IsNullOrWhiteSpace(item.impostos.valorICMS) ? 0 : decimal.Parse(item.impostos.valorICMS, cultura);
                            itemPreFatura.ValorICMSST = string.IsNullOrWhiteSpace(item.impostos.valorICMSST) ? 0 : decimal.Parse(item.impostos.valorICMSST, cultura);
                            itemPreFatura.ValorISS = string.IsNullOrWhiteSpace(item.impostos.valorISS) ? 0 : decimal.Parse(item.impostos.valorISS, cultura);
                            itemPreFatura.ValorPIS = string.IsNullOrWhiteSpace(item.impostos.valorPIS) ? 0 : decimal.Parse(item.impostos.valorPIS, cultura);

                            itemPreFatura.CargaCTe = cargaCTe;
                            itemPreFatura.PreFatura = preFatura;
                            itemPreFatura.DocumentoTransporte = dtNatura;

                            repItemPreFaturaNatura.Inserir(itemPreFatura);

                            valorFrete += decimal.Parse(item.transporte.valorFrete, cultura);

                            unidadeDeTrabalho.CommitChanges();

                            unidadeDeTrabalho.FlushAndClear();
                        }
                    }

                    preFatura = repPreFaturaNatura.BuscarPorCodigo(preFatura.Codigo);
                }

                if (preFatura.Integracoes == null)
                    preFatura.Integracoes = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

                preFatura.ValorFrete += valorFrete;
                preFatura.Integracoes.Add(integracaoNatura);

                if (preFatura.Situacao == SituacaoPreFaturaNatura.Atualizando)
                    preFatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura.Pendente;

                repPreFaturaNatura.Atualizar(preFatura);
            }
        }

        public bool GerarFatura(out string erro, Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFatura, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (preFatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura.Atualizando)
            {
                erro = $"Não é possível gerar uma fatura na situação atual da pré fatura ({preFatura.Situacao.ObterDescricao()}).";
                return false;
            }

            if (preFatura.Fatura != null && preFatura.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado)
            {
                erro = "Esta pré fatura já está vinculada a uma fatura.";
                return false;
            }

            if (preFatura.Itens.Count <= 0)
            {
                erro = "Não há documentos vinculados a esta pré fatura.";
                return false;
            }

            Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFaturaNatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura, true);

            if (grupoPessoas == null)
            {
                erro = "Não foi encontrado um grupo de pessoas configurado com integração à Natura.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarPorPreFaturaNatura(preFatura.Codigo);

            if(documentosFaturamento.Any(o => o.Situacao == SituacaoDocumentoFaturamento.Anulado || o.Situacao == SituacaoDocumentoFaturamento.Cancelado))
            {
                erro = "Não é possível gerar uma fatura com documentos anulados ou cancelados.";
                return false;
            }

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura()
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento,
                //DataInicial = (from obj in preFatura.Itens orderby obj.CargaCTe.CTe.DataEmissao select obj.CargaCTe.CTe.DataEmissao.Value).FirstOrDefault(),
                //DataFinal = (from obj in preFatura.Itens orderby obj.CargaCTe.CTe.DataEmissao select obj.CargaCTe.CTe.DataEmissao.Value).LastOrDefault(),
                DataInicial = documentosFaturamento.Min(o => o.DataEmissao),
                DataFinal = documentosFaturamento.Max(o => o.DataEmissao),
                Empresa = preFatura.Empresa,
                GrupoPessoas = grupoPessoas,
                Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Documentos,
                NumeroPreFatura = preFatura.NumeroPreFatura,
                DataFatura = preFatura.DataPreFatura,
                TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa,
                Usuario = usuario,
                NovoModelo = true,
                ControleNumeracao = repFatura.BuscarProximoControleNumeracao()
            };

            repFatura.Inserir(fatura);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumento
                {
                    Fatura = fatura,
                    Documento = documentoFaturamento,
                    ValorACobrar = documentoFaturamento.ValorAFaturar,
                    ValorTotalACobrar = documentoFaturamento.ValorAFaturar
                };

                repFaturaDocumento.Inserir(faturaDocumento);

                documentoFaturamento.ValorEmFatura += faturaDocumento.ValorACobrar;
                documentoFaturamento.ValorAFaturar -= faturaDocumento.ValorACobrar;
                documentoFaturamento.Fatura = faturaDocumento.Fatura;

                repDocumentoFaturamento.Atualizar(documentoFaturamento);
            }

            //List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            //List<int> codigosCargaCTe = new List<int>();

            //foreach (Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura item in preFatura.Itens)
            //{
            //    if (item.CargaCTe == null)
            //        continue;

            //    //if (!cargas.Any(o => o.Codigo == item.CargaCTe.Carga.Codigo))
            //    //    cargas.Add(item.CargaCTe.Carga);

            //    //if (!codigosCargaCTe.Any(o => o == item.CargaCTe.Codigo))
            //    //    codigosCargaCTe.Add(item.CargaCTe.Codigo);

            //    //if (repFaturaCargaDocumento.ContemDocumentoEmOutraFatura(fatura.Codigo, item.CargaCTe.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento))
            //    //{
            //    //    erro = "O CT-e " + item.CargaCTe.CTe.Numero + "-" + item.CargaCTe.CTe.Serie.Numero + " está vinculado a outra fatura, não sendo possível gerar a fatura.";
            //    //    return false;
            //    //}

            //    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(item.CargaCTe.CTe.Codigo);

            //    if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out erro, ref fatura, documentoFaturamento.Codigo, documentoFaturamento.ValorAFaturar, unidadeTrabalho))
            //        return false;

            //    //Modo antigo
            //    //Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento documento = new Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento()
            //    //{
            //    //    Carga = item.CargaCTe.Carga,
            //    //    ConhecimentoDeTransporteEletronico = item.CargaCTe.CTe,
            //    //    Fatura = fatura,
            //    //    StatusDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal,
            //    //    TipoDocumentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento
            //    //};
            //    //repFaturaDocumento.Inserir(documento);

            //    //item.CargaCTe.CTe.Fatura = fatura;

            //    //repCTe.Atualizar(item.CargaCTe.CTe);
            //}

            Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, unidadeTrabalho);

            repFatura.Atualizar(fatura);

            //foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            //{
            //    Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga situacao;

            //    if (carga.CargaCTes.All(o => codigosCargaCTe.Contains(o.Codigo)))
            //        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.Faturada;
            //    else
            //        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.FaturadaParcial;

            //    Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = new Dominio.Entidades.Embarcador.Fatura.FaturaCarga()
            //    {
            //        Carga = carga,
            //        Fatura = fatura,
            //        StatusFaturaCarga = situacao
            //    };

            //    repFaturaCarga.Inserir(faturaCarga);
            //}

            preFatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura.Gerada;
            preFatura.Fatura = fatura;

            repPreFaturaNatura.Atualizar(preFatura);

            erro = string.Empty;
            return true;
        }

        public static void EnviarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoFatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(unidadeTrabalho);
            Repositorio.Embarcador.Integracao.PreFaturaNatura repPreFaturaNatura = new Repositorio.Embarcador.Integracao.PreFaturaNatura(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoMatrizNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNatura) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNatura))
            {
                integracaoFatura.MensagemRetorno = "A configuração de integração para a Natura é inválida.";
                integracaoFatura.Tentativas += 1;
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repFaturaIntegracao.Atualizar(integracaoFatura);

                return;
            }

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
            string mensagemRetorno = string.Empty;
            ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaResponseDados[] retorno = null;
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo arquivoIntegracao = null;

            Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura preFaturaNatura = repPreFaturaNatura.BuscarPorFatura(integracaoFatura.Fatura.Codigo);

            List<ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens> itens = new List<ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens>();

            try
            {
                if (preFaturaNatura != null)
                {
                    if (integracaoFatura.Fatura.NovoModelo)
                    {
                        List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentosFatura = repFaturaDocumento.BuscarPorFatura(integracaoFatura.Fatura.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento in documentosFatura)
                        {
                            Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura itemPreFatura = (from obj in preFaturaNatura.Itens where obj.CargaCTe.CTe.Codigo == faturaDocumento.Documento.CTe.Codigo select obj).FirstOrDefault();
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = null;

                            if (itemPreFatura == null)
                                throw new ServicoException($"CT-e de nro {faturaDocumento.Documento.CTe.Numero} não localizado na pré-fatura de nro {preFaturaNatura.NumeroPreFatura}.");

                            if (itemPreFatura.CargaCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                                cteOriginal = repCTe.BuscarPorChave(itemPreFatura.CargaCTe.CTe.ChaveCTESubComp);

                            bool nfse = itemPreFatura.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || itemPreFatura.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS;

                            itens.Add(new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens()
                            {
                                chaveCTe = nfse ? "" : itemPreFatura.CargaCTe.CTe.Chave,
                                codTranspEmit = itemPreFatura.CargaCTe.CTe.Empresa.Configuracao.CodigoFilialNatura,
                                dataDocFiscal = itemPreFatura.CargaCTe.CTe.DataEmissao.Value,
                                docFiscalRef = itemPreFatura.CargaCTe.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento ?
                                               (from documento in itemPreFatura.CargaCTe.CTe.Documentos
                                                select new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef()
                                                {
                                                    chaveNFeCTe = documento.ChaveNFE,
                                                    dataEmissao = documento.DataEmissao,
                                                    modelo = documento.ModeloDocumentoFiscal.Numero,
                                                    numero = documento.Numero,
                                                    serie = documento.SerieOuSerieDaChave
                                                }).ToArray() :
                                                (from documento in cteOriginal.Documentos
                                                 select new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef()
                                                 {
                                                     chaveNFeCTe = documento.ChaveNFE,
                                                     dataEmissao = documento.DataEmissao,
                                                     modelo = documento.ModeloDocumentoFiscal.Numero,
                                                     numero = documento.Numero,
                                                     serie = documento.SerieOuSerieDaChave
                                                 }).ToArray(),
                                impostos = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensImpostos()
                                {
                                    aliquotaCOFINS = itemPreFatura.AliquotaCOFINS.ToString("0.00", cultura),
                                    aliquotaICMS = itemPreFatura.AliquotaICMS.ToString("0.00", cultura),
                                    aliquotaICMSST = itemPreFatura.AliquotaICMSST.ToString("0.00", cultura),
                                    aliquotaISS = itemPreFatura.AliquotaISS.ToString("0.00", cultura),
                                    aliquotaPIS = itemPreFatura.AliquotaPIS.ToString("0.00", cultura),
                                    baseCOFINS = itemPreFatura.BaseCalculoCOFINS.ToString("0.00", cultura),
                                    baseICMS = itemPreFatura.BaseCalculoICMS.ToString("0.00", cultura),
                                    baseICMSST = itemPreFatura.BaseCalculoICMSST.ToString("0.00", cultura),
                                    baseISS = itemPreFatura.BaseCalculoISS.ToString("0.00", cultura),
                                    basePIS = itemPreFatura.BaseCalculoPIS.ToString("0.00", cultura),
                                    ivaICMSST = itemPreFatura.IVAICMSST.ToString("0.00", cultura),
                                    valorCOFINS = itemPreFatura.ValorCOFINS.ToString("0.00", cultura),
                                    valorICMS = itemPreFatura.ValorICMS.ToString("0.00", cultura),
                                    valorICMSST = itemPreFatura.ValorICMSST.ToString("0.00", cultura),
                                    valorISS = itemPreFatura.ValorISS.ToString("0.00", cultura),
                                    valorPIS = itemPreFatura.ValorPIS.ToString("0.00", cultura)
                                },
                                modeloDocFiscal = nfse ? "07" : "57",
                                numeroDocFiscal = itemPreFatura.CargaCTe.CTe.Numero.ToString(),
                                serieDocFiscal = string.Format("{0:000}", itemPreFatura.CargaCTe.CTe.Serie.Numero),
                                transporte = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensTransporte()
                                {
                                    difValorFrete = itemPreFatura.ValorDoDesconto.ToString("0.00", cultura),
                                    docTransporte = itemPreFatura.DocumentoTransporte.Numero.ToString(),
                                    valorFrete = itemPreFatura.CargaCTe.CTe.ValorAReceber.ToString("0.00", cultura)
                                }
                            });
                        }
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> documentosFatura = repFaturaCargaDocumento.BuscarPorFatura(integracaoFatura.Fatura.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal);

                        foreach (Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento faturaDocumento in documentosFatura)
                        {
                            Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura itemPreFatura = (from obj in preFaturaNatura.Itens where obj.CargaCTe.CTe.Codigo == faturaDocumento.ConhecimentoDeTransporteEletronico.Codigo select obj).FirstOrDefault();
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = null;

                            if (itemPreFatura == null)
                                throw new ServicoException($"CT-e de nro {faturaDocumento.ConhecimentoDeTransporteEletronico.Numero} não localizado na pré-fatura de nro {preFaturaNatura.NumeroPreFatura}.");

                            if (itemPreFatura.CargaCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                            {
                                cteOriginal = repCTe.BuscarPorChave(itemPreFatura.CargaCTe.CTe.ChaveCTESubComp);
                            }

                            itens.Add(new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens()
                            {
                                chaveCTe = itemPreFatura.CargaCTe.CTe.Chave,
                                codTranspEmit = itemPreFatura.CargaCTe.CTe.Empresa.Configuracao.CodigoFilialNatura,
                                dataDocFiscal = itemPreFatura.CargaCTe.CTe.DataEmissao.Value,
                                docFiscalRef = itemPreFatura.CargaCTe.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento ?
                                               (from documento in itemPreFatura.CargaCTe.CTe.Documentos
                                                select new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef()
                                                {
                                                    chaveNFeCTe = documento.ChaveNFE,
                                                    dataEmissao = documento.DataEmissao,
                                                    modelo = documento.ModeloDocumentoFiscal.Numero,
                                                    numero = documento.Numero,
                                                    serie = documento.SerieOuSerieDaChave
                                                }).ToArray() :
                                                //new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef[] { new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef() {
                                                //         chaveNFeCTe = cteOriginal.Documentos.First().ChaveNFE,
                                                //         dataEmissao = cteOriginal.Documentos.First().DataEmissao,
                                                //         modelo = "55",
                                                //         numero =cteOriginal.Documentos.First().Numero,
                                                //         serie = cteOriginal.Documentos.First().SerieOuSerieDaChave
                                                //    }
                                                //},
                                                (from documento in cteOriginal.Documentos
                                                 select new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef()
                                                 {
                                                     chaveNFeCTe = documento.ChaveNFE,
                                                     dataEmissao = documento.DataEmissao,
                                                     modelo = documento.ModeloDocumentoFiscal.Numero,
                                                     numero = documento.Numero,
                                                     serie = documento.SerieOuSerieDaChave
                                                 }).ToArray(),
                                impostos = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensImpostos()
                                {
                                    aliquotaCOFINS = itemPreFatura.AliquotaCOFINS.ToString("0.00", cultura),
                                    aliquotaICMS = itemPreFatura.AliquotaICMS.ToString("0.00", cultura),
                                    aliquotaICMSST = itemPreFatura.AliquotaICMSST.ToString("0.00", cultura),
                                    aliquotaISS = itemPreFatura.AliquotaISS.ToString("0.00", cultura),
                                    aliquotaPIS = itemPreFatura.AliquotaPIS.ToString("0.00", cultura),
                                    baseCOFINS = itemPreFatura.BaseCalculoCOFINS.ToString("0.00", cultura),
                                    baseICMS = itemPreFatura.BaseCalculoICMS.ToString("0.00", cultura),
                                    baseICMSST = itemPreFatura.BaseCalculoICMSST.ToString("0.00", cultura),
                                    baseISS = itemPreFatura.BaseCalculoISS.ToString("0.00", cultura),
                                    basePIS = itemPreFatura.BaseCalculoPIS.ToString("0.00", cultura),
                                    ivaICMSST = itemPreFatura.IVAICMSST.ToString("0.00", cultura),
                                    valorCOFINS = itemPreFatura.ValorCOFINS.ToString("0.00", cultura),
                                    valorICMS = itemPreFatura.ValorICMS.ToString("0.00", cultura),
                                    valorICMSST = itemPreFatura.ValorICMSST.ToString("0.00", cultura),
                                    valorISS = itemPreFatura.ValorISS.ToString("0.00", cultura),
                                    valorPIS = itemPreFatura.ValorPIS.ToString("0.00", cultura)
                                },
                                modeloDocFiscal = "57",
                                numeroDocFiscal = itemPreFatura.CargaCTe.CTe.Numero.ToString(),
                                serieDocFiscal = string.Format("{0:000}", itemPreFatura.CargaCTe.CTe.Serie.Numero),
                                transporte = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensTransporte()
                                {
                                    difValorFrete = itemPreFatura.ValorDoDesconto.ToString("0.00", cultura),
                                    docTransporte = itemPreFatura.DocumentoTransporte.Numero.ToString(),
                                    valorFrete = itemPreFatura.CargaCTe.CTe.ValorAReceber.ToString("0.00", cultura)
                                }
                            });
                        }
                    }

                    ServicoNaturaNovo.ProcessaFatura.SI_ProcessaFatura_SyncClient svcFatura = IntegracaoDTNatura.ObterClientNatura<ServicoNaturaNovo.ProcessaFatura.SI_ProcessaFatura_SyncClient, ServicoNaturaNovo.ProcessaFatura.SI_ProcessaFatura_Sync>(configuracaoIntegracao.UsuarioNatura, configuracaoIntegracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_Novo_SI_ProcessaFatura, unidadeTrabalho, out inspector);

                    var dados = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDados()
                    {
                        codTranspMatriz = configuracaoIntegracao.CodigoMatrizNatura,
                        dataFatura = integracaoFatura.Fatura.DataFatura,
                        dataPreFatura = preFaturaNatura.DataPreFatura,
                        dataPreFaturaSpecified = true,
                        dataVencFatura = integracaoFatura.Fatura.Parcelas.Select(o => o.DataVencimento).First(),
                        numeroFatura = integracaoFatura.Fatura.Numero.ToString(),
                        numeroPreFatura = preFaturaNatura.NumeroPreFatura.ToString(),
                        itens = itens.ToArray()
                    };

                    try
                    {
                        if (svcFatura.Endpoint.Address.ToString().Contains("qxx.transportes.natura.com.br"))
                            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        retorno = svcFatura.SI_ProcessaFatura_Sync(new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFatura()
                        {
                            dados = dados
                        });

                        if (retorno != null)
                            mensagemRetorno = string.Join(" / ", from obj in retorno select obj.number + " - " + obj.message);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        mensagemRetorno = ex.Message;
                    }

                    arquivoIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo();

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeTrabalho);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeTrabalho);
                    arquivoIntegracao.Data = DateTime.Now;
                    arquivoIntegracao.Mensagem = mensagemRetorno;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                    repFaturaIntegracaoArquivo.Inserir(arquivoIntegracao);
                }
                else
                {
                    mensagemRetorno = "Pré fatura não encontrada para esta fatura, não sendo possível integrar com a Natura.";
                }
            }
            catch (ServicoException excecao)
            {
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagemRetorno = "Ocorreu uma falha ao integrar a fatura.";
            }

            integracaoFatura.MensagemRetorno = mensagemRetorno;
            integracaoFatura.Tentativas += 1;
            integracaoFatura.DataEnvio = DateTime.Now;

            if (arquivoIntegracao != null)
                integracaoFatura.ArquivosIntegracao.Add(arquivoIntegracao);

            if (retorno != null && retorno.Any(o => o.number == "100"))
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                integracaoFatura.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            repFaturaIntegracao.Atualizar(integracaoFatura);
        }
    }
}
