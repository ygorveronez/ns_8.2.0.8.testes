using Dominio.Entidades.Embarcador.Bidding;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Bidding;
using Dominio.ObjetosDeValor.Embarcador.Bidding.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System.Drawing;
using System.Linq.Dynamic;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingAceitacao")]
    public class BiddingAceitamentoController : BaseController
    {
        #region Construtores

        public BiddingAceitamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingConvite grupoBiddingConvite = await repBiddingConvite.BuscarPorCodigoAsync(codigo, false);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                if (grupoBiddingConvite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado convidado = repConvidado.BuscarConvidado(grupoBiddingConvite, this.Usuario.Empresa);

                if (convidado == null)
                    return new JsonpResult(false, "Convidado não encontrado");

                Repositorio.Embarcador.Bidding.BiddingChecklist repChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingChecklist listaChecklist = repChecklist.BuscarChecklist(grupoBiddingConvite);

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklist entidadeChecklist = await repChecklist.BuscarPorCodigoAsync(listaChecklist.Codigo, false);

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario repQuestionarios = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario> listaQuestionarios = repQuestionarios.BuscarQuestionarios(entidadeChecklist);

                Repositorio.Embarcador.Bidding.BiddingOferta repOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(unitOfWork);
                BiddingOferta entidadeOferta = await repOferta.BuscarOfertaAsync(grupoBiddingConvite);

                Repositorio.Embarcador.Bidding.BiddingOfertaRota repOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                List<BiddingOfertaRota> listaRotas = repOfertaRota.BuscarRotas(entidadeOferta.Codigo);

                Repositorio.Embarcador.Bidding.BiddingDuvida repDuvida = new Repositorio.Embarcador.Bidding.BiddingDuvida(unitOfWork);
                List<BiddingDuvida> listaDuvidas = repDuvida.BuscarPorConvite(grupoBiddingConvite.Codigo);

                Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta repQuestionarioResposta = new Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta(unitOfWork);
                List<BiddingAceitamentoQuestaoResposta> listaRespostas = repQuestionarioResposta.BuscarPorTransportadorBidding(this.Usuario.Empresa, grupoBiddingConvite);

                Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo repAceitamentoQuestionarioAnexo = new Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo(unitOfWork);
                List<BiddingAceitamentoQuestionarioAnexo> listaAnexosPorResposta = repAceitamentoQuestionarioAnexo.BuscarPorRespostas(listaRespostas);

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                List<BiddingTransportadorRota> listaTransportadorRotas = repTransportadorRota.BuscarPorOferta(entidadeOferta, this.Usuario.Empresa.Codigo).OrderByDescending(o => o.Codigo).ToList();

                Repositorio.Embarcador.Bidding.TipoBiddingAnexo repositorioTipoBiddingAnexo = new Repositorio.Embarcador.Bidding.TipoBiddingAnexo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo> tipoBiddingAnexos = repositorioTipoBiddingAnexo.BuscarPorTipoBidding(grupoBiddingConvite.TipoBidding?.Codigo ?? 0);

                IList<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> listaRotasDados = repOfertaRota.BuscarRotasProcessadas(entidadeOferta.Codigo);
                dynamic rotasProcessadas = servicoBidding.ProcessarListaRotas(listaRotasDados);

                return new JsonpResult(new
                {
                    grupoBiddingConvite.Codigo,
                    NaoPossuiPedagioFluxoOferta = grupoBiddingConvite.TipoBidding.NaoPossuiPedagioFluxoOferta,
                    NaoIncluirImpostoValorTotalOferta = grupoBiddingConvite.TipoBidding.NaoIncluirImpostoValorTotalOferta,
                    Status = servicoBidding.AutomatizacaoEtapasTransportador(convidado, grupoBiddingConvite, configuracaoBidding, TipoServicoMultisoftware),
                    DadosBiddingConvite = new
                    {
                        grupoBiddingConvite.Codigo,
                        grupoBiddingConvite.Situacao,
                        grupoBiddingConvite.Descricao,
                        DataInicio = grupoBiddingConvite.DataInicio.ToString(),
                        DataLimite = grupoBiddingConvite.DataLimite.ToString(),
                        grupoBiddingConvite.DescritivoConvite,
                        grupoBiddingConvite.DescritivoTransportador,
                        PrazoAceiteConvite = grupoBiddingConvite.DataPrazoAceiteConvite.HasValue ? grupoBiddingConvite.DataPrazoAceiteConvite.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        TempoRestante = grupoBiddingConvite.DataPrazoAceiteConvite.HasValue ? grupoBiddingConvite.DataPrazoAceiteConvite.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        StatusConviteConvidado = convidado.Status.GetHashCode(),
                        RemoverEtapaAceite = grupoBiddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador,
                        TipoFrete = grupoBiddingConvite.TipoFrete?.ObterDescricao() ?? "",
                        DataInicioVigencia = !(grupoBiddingConvite.DataInicioVigencia.IsNullOrMinValue()) ? grupoBiddingConvite.DataInicioVigencia?.ToString("dd/MM/yyyy") : string.Empty,
                    },
                    Duvidas = (
                        from duvida in listaDuvidas
                        select new
                        {
                            duvida.Codigo,
                            duvida.Pergunta,
                            duvida.Resposta,
                            Data = duvida.Data.ToString("dd/MM/yyyy")
                        }
                    ).ToList(),
                    OfertasRotas = (
                        from rota in listaTransportadorRotas
                        select new
                        {
                            rota.Codigo,
                            CodigoRota = rota.Rota.Codigo,
                            Rota = rota.Rota.Descricao,
                            ValorTransportado = rota.Rota.ValorCargaMes,
                            VolumeTransportado = rota.Rota.Volume,
                            PesoTransportado = rota.Rota.Peso,
                            FrequenciaMensal = rota.Rota.Frequencia,
                            NumeroEntregas = rota.Rota.NumeroEntrega,
                            rota.Rota.QuilometragemMedia,
                            rota.Rota.AdicionalAPartirDaEntregaNumero,
                            TipoCarga = (
                                from tipoCarga in rota.Rota.TiposCarga
                                select new
                                {
                                    tipoCarga.Descricao
                                }
                            ).ToList(),
                            ModelosVeiculares = (
                                from modeloVeicular in rota.Rota.ModelosVeiculares
                                select new
                                {
                                    modeloVeicular.Descricao,
                                    modeloVeicular.Codigo,
                                    modeloVeicular.NumeroEixos,
                                    CapacidadePesoTransporte = modeloVeicular.CapacidadePesoTransporte.ToString("n2"),
                                    NomeTabEquipamento = "#Equipamento" + modeloVeicular.Codigo,
                                    NomeTabFrotaFixaKmRodado = "#FrotaFixaKmRodado" + modeloVeicular.Codigo,
                                    NomeTabPorcentagemSobreNota = "#PorcentagemNota" + modeloVeicular.Codigo,
                                    NomeTabViagemAdicional = "#ViagemAdicional" + modeloVeicular.Codigo,
                                    NomeTabFrotaFixaFranquia = "#FrotaFixaFranquia" + modeloVeicular.Codigo,
                                    NomeTabFretePorPeso = "#FretePorPeso" + modeloVeicular.Codigo,
                                    NomeTabFretePorCapacidade = "#FretePorCapacidade" + modeloVeicular.Codigo,
                                    NomeTabFretePorViagem = "#FretePorViagem" + modeloVeicular.Codigo,
                                }
                            ).ToList(),
                            Rodada = rota.Rodada + "ª Rodada",
                            Target = "R$ " + rota.Target.ToString("n2"),
                            Situacao = rota.Status.ObterDescricao(),
                            SituacaoCodigo = rota.Status,
                            DT_RowColor = rota.Status.ObterCorLinha(),
                            DT_FontColor = rota.Status.ObterCorFonte(),
                            Tomador = rota.Rota.Tomador?.Nome ?? string.Empty,
                            GrupoModeloVeicular = rota.Rota.GrupoModeloVeicular?.Descricao,
                            CarroceriaVeiculo = rota.Rota.CarroceriaVeiculo?.ObterDescricao(),
                            AlicotaPadraoICMS = rota.Rota.AlicotaPadraoICMS?.ToString() ?? string.Empty,
                            FrequenciaMensalComAjudante = rota.Rota.FrequenciaMensalComAjudante,
                            QuantidadeAjudantesPorVeiculo = rota.Rota.QuantidadeAjudantePorVeiculo,
                            MediaEntregasFracionadas = rota.Rota.MediaEntregasFracionada,
                            MaximaEntregasFracionadas = rota.Rota.MaximaEntregasFacionada,
                            Observacao = rota.Rota.Observacao,
                            Inconterm = rota.Rota.Inconterm?.ObterDescricao(),
                            QuantidadeViagensPorAno = rota.Rota.QuantidadeViagensPorAno,
                            VolumeTonAno = rota.Rota.VolumeTonAno,
                            VolumeTonViagem = rota.Rota.VolumeTonViagem,
                            TempoColeta = rota.Rota.TempoColeta,
                            TempoDescarga = rota.Rota.TempoDescarga,
                            Compressor = rota.Rota.Compressor?.ObterDescricao(),
                            Origem = rota.Rota.DescricaoOrigem,
                            Destino = rota.Rota.DescricaoDestino,
                            ModeloVeicular = string.Join(", ", rota.Rota.ModelosVeiculares.Select(o => o.Descricao)),
                        }
                    ).ToList(),
                    Convidado = new
                    {
                        convidado.Convidado.Codigo,
                        convidado.Convidado.Descricao,
                        convidado.StatusBidding
                    },
                    Anexos = (
                        from o in grupoBiddingConvite.Anexos
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    AnexosTipoBidding = (
                        from o in tipoBiddingAnexos
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    Checklist = new
                    {
                        entidadeChecklist.Codigo,
                        Prazo = entidadeChecklist.DataPrazo?.ToString("dd/MM/yyyy HH:mm"),
                        TipoPreenchimentoChecklist = entidadeChecklist.TipoPreenchimentoChecklist ?? TipoPreenchimentoChecklist.PreenchimentoObrigatorio
                    },
                    Questionarios = (
                        from questionario in listaQuestionarios
                        select new
                        {
                            questionario.Codigo,
                            questionario.Descricao,
                            ChecklistAnexo = from ChecklistAnexo in questionario.Anexos
                                             where ChecklistAnexo.EntidadeAnexo.Checklist == questionario.Checklist
                                             select new
                                             {
                                                 ChecklistAnexo.Codigo,
                                                 ChecklistAnexo.Descricao,
                                                 ChecklistAnexo.NomeArquivo
                                             },
                            Respostas = (
                            from resposta in listaRespostas
                            select new
                            {
                                resposta.Codigo,
                                resposta.Observacao,
                                resposta.Resposta,
                                AnexosResposta = from anexo in listaAnexosPorResposta
                                                 where anexo.EntidadeAnexo.Codigo == resposta.Codigo
                                                 select new
                                                 {
                                                     anexo.Codigo,
                                                     anexo.Descricao,
                                                     anexo.NomeArquivo,
                                                     CodigoPergunta = resposta.Pergunta.Codigo
                                                 }
                            }
                            ).ToList(),
                        }
                    ).ToList(),
                    Oferta = new
                    {
                        entidadeOferta.Codigo,
                        entidadeOferta.TipoLance,
                        DataPrazoOferta = entidadeOferta.DataPrazoOferta.HasValue ? entidadeOferta.DataPrazoOferta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                    },
                    Rotas = (
                        from rota in listaRotas
                        select new
                        {
                            rota.Codigo,
                            rota.Descricao,
                            Rota = rota.Descricao,
                            rota.FlagOrigem,
                            rota.FlagDestino,
                            rota.NumeroEntrega,
                            ClienteDestino = rotasProcessadas.ListaClienteDestino,
                            ClienteOrigem = rotasProcessadas.ListaClienteOrigem,
                            CidadeDestino = rotasProcessadas.ListaDestino,
                            CidadeOrigem = rotasProcessadas.ListaOrigem,
                            RegiaoDestino = rotasProcessadas.ListaRegiaoDestino,
                            RegiaoOrigem = rotasProcessadas.ListaRegiaoOrigem,
                            PaisOrigem = rotasProcessadas.ListaPaisOrigem,
                            PaisDestino = rotasProcessadas.ListaPaisDestino,
                            EstadoDestino = rotasProcessadas.ListaEstadoDestino,
                            EstadoOrigem = rotasProcessadas.ListaEstadoOrigem,
                            rota.QuilometragemMedia,
                            Frequencia = rota.Frequencia > 0 ? rota.Frequencia.ToString() : "",
                            rota.Peso,
                            AdicionalAPartirDaEntregaNumero = rota.AdicionalAPartirDaEntregaNumero > 0 ? rota.AdicionalAPartirDaEntregaNumero.ToString() : "",
                            rota.Volume,
                            rota.ValorCargaMes,
                            TipoCarga = (from tipocarga in rota.TiposCarga
                                         select new
                                         {
                                             tipocarga.Codigo,
                                             tipocarga.Descricao
                                         }).ToList(),
                            ModeloVeicular = (from modeloveicular in rota.ModelosVeiculares
                                              select new
                                              {
                                                  modeloveicular.Codigo,
                                                  modeloveicular.Descricao
                                              }).ToList(),
                            rota.Observacao,
                            CEPOrigem = (from cepOrigem in rota.CEPsOrigem
                                         select new
                                         {
                                             cepOrigem.Codigo,
                                             CEPInicial = cepOrigem.CEPInicial.ToString(),
                                             CEPFinal = cepOrigem.CEPFinal.ToString()
                                         }).ToList(),
                            CEPDestino = (from cepDestino in rota.CEPsDestino
                                          select new
                                          {
                                              cepDestino.Codigo,
                                              CEPInicial = cepDestino.CEPInicial.ToString(),
                                              CEPFinal = cepDestino.CEPFinal.ToString()
                                          }).ToList(),
                            RotaOrigem = rotasProcessadas.ListaRotaOrigem,
                            RotaDestino = rotasProcessadas.ListaRotaDestino,
                            Tomador = rota.Tomador?.Nome ?? "",
                            GrupoModeloVeicular = rota.GrupoModeloVeicular?.Descricao ?? "",
                            CarroceriaVeiculo = rota.CarroceriaVeiculo?.ObterDescricao() ?? "",
                            AlicotaPadraoICMS = rota.AlicotaPadraoICMS?.ToString() ?? string.Empty,
                            FrequenciaMensalComAjudante = rota.FrequenciaMensalComAjudante > 0 ? rota.FrequenciaMensalComAjudante.ToString() : "",
                            QuantidadeAjudantesPorVeiculo = rota.QuantidadeAjudantePorVeiculo > 0 ? rota.QuantidadeAjudantePorVeiculo.ToString() : "",
                            MediaEntregasFracionadas = rota.MediaEntregasFracionada > 0 ? rota.MediaEntregasFracionada.ToString() : "",
                            MaximaEntregasFracionadas = rota.MaximaEntregasFacionada > 0 ? rota.MaximaEntregasFacionada.ToString() : "",
                            Inconterm = rota.Inconterm?.ObterDescricao() ?? "",
                            QuantidadeViagensPorAno = rota.QuantidadeViagensPorAno > 0 ? rota.QuantidadeViagensPorAno.ToString() : "",
                            VolumeTonAno = rota.VolumeTonAno > 0 ? rota.VolumeTonAno.ToString() : "",
                            VolumeTonViagem = rota.VolumeTonViagem > 0 ? rota.VolumeTonViagem.ToString() : "",
                            TempoColeta = rota.TempoColeta.ToString() ?? "",
                            TempoDescarga = rota.TempoDescarga.ToString() ?? "",
                            Compressor = rota.Compressor?.ObterDescricao() ?? ""
                        }
                    ).ToList(),
                    OpcoesRota = rotasProcessadas.ListaRotas,
                    InformarVeiculosVerdes = entidadeOferta.PermitirInformarVeiculosVerdes,
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarConvite()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                BiddingConvite biddingConvite;

                int codigo = Request.GetIntParam("Codigo");
                biddingConvite = repBiddingConvite.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Empresa convidado = this.Usuario.Empresa;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, convidado, null, $"Rejeitou o bidding " + biddingConvite.Descricao + ".", unitOfWork);

                BiddingConviteConvidado convite = repConvidado.BuscarConvidado(biddingConvite, convidado);
                convite.DataRetorno = DateTime.Now;
                convite.Status = StatusBiddingConviteConvidado.Rejeitado;
                repConvidado.Atualizar(convite);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar o convite.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AceitarConvite()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingChecklist repBiddingChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                BiddingConvite biddingConvite;
                Dominio.Entidades.Empresa convidado = this.Usuario.Empresa;
                biddingConvite = repBiddingConvite.BuscarPorCodigo(codigo, false);

                BiddingConviteConvidado convidadoConvite = repConvidado.BuscarConvidado(biddingConvite, convidado);
                convidadoConvite.DataRetorno = DateTime.Now;
                convidadoConvite.StatusBidding = StatusBiddingConvite.Checklist;
                convidadoConvite.Status = StatusBiddingConviteConvidado.Aceito;

                if (biddingConvite.TempoRestante != 0 && biddingConvite.TempoRestante <= 0)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, convidado, null, $"Rejeitou o bidding " + biddingConvite.Descricao + ".", unitOfWork);
                    ModificarStatusConvidado(convidadoConvite, StatusBiddingConviteConvidado.Rejeitado, unitOfWork);
                    unitOfWork.CommitChanges();
                    throw new ControllerException("O tempo para esse convite está esgotado. O convite foi rejeitado automaticamente.");
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, convidado, null, $"Aceitou o bidding " + biddingConvite.Descricao + ".", unitOfWork);

                ModificarStatusConvidado(convidadoConvite, StatusBiddingConviteConvidado.Aceito, unitOfWork);

                BiddingChecklist biddingChecklist = repBiddingChecklist.BuscarChecklist(biddingConvite);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding)
                {
                    Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConviteConvidado = convidadoConvite.BiddingConvite;

                    if (biddingChecklist?.TipoPreenchimentoChecklist == TipoPreenchimentoChecklist.PreenchimentoDesabilitado)
                    {
                        convidadoConvite.StatusBidding = StatusBiddingConvite.Ofertas;
                        biddingConviteConvidado.Status = StatusBiddingConvite.Ofertas;
                    }
                    else
                    {
                        convidadoConvite.StatusBidding = StatusBiddingConvite.Checklist;
                        biddingConviteConvidado.Status = StatusBiddingConvite.Checklist;
                    }

                    repConvidado.Atualizar(convidadoConvite);
                    repBiddingConvite.Atualizar(biddingConviteConvidado);
                }

                if (biddingChecklist?.TipoPreenchimentoChecklist == TipoPreenchimentoChecklist.PreenchimentoDesabilitado)
                    servicoBidding.EnviarRotas(biddingConvite, this.Usuario, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aceitar o convite.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarDuvida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingDuvida repBiddingDuvida = new Repositorio.Embarcador.Bidding.BiddingDuvida(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite;
                Dominio.Entidades.Embarcador.Bidding.BiddingDuvida biddingDuvida = new Dominio.Entidades.Embarcador.Bidding.BiddingDuvida();

                int codigo = Request.GetIntParam("Codigo");
                string pergunta = Request.GetStringParam("Pergunta");
                biddingConvite = repBiddingConvite.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Empresa convidado = this.Usuario.Empresa;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, convidado, null, $"Enviou uma duvida sobre o bidding " + biddingConvite.Descricao + ".", unitOfWork);

                biddingDuvida.BiddingConvite = biddingConvite;
                biddingDuvida.Empresa = convidado;
                biddingDuvida.Data = DateTime.Now;
                biddingDuvida.Pergunta = pergunta;

                if (string.IsNullOrWhiteSpace(pergunta))
                    throw new Exception("Pergunta vazia!");

                repBiddingDuvida.Inserir(biddingDuvida);

                unitOfWork.CommitChanges();

                List<BiddingDuvida> listaDuvidas = BuscarDuvidas(unitOfWork, biddingConvite);

                return new JsonpResult(new
                {
                    Duvidas = (
                        from duvida in listaDuvidas
                        select new
                        {
                            Pergunta = duvida.Pergunta.ToString(),
                            Data = duvida.Data.ToString("dd/MM/yyyy"),
                            Resposta = duvida.Resposta
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar o convite.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarRespostas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario repChecklistQuestionario = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta repResposta = new Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConvite repBidding = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo repRespostaAnexo = new Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador repChecklistBiddingTransportador = new Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                dynamic dados = JsonConvert.DeserializeObject<dynamic>(Request.Params("Dados"));

                int codigoBidding = Request.GetIntParam("CodigoBidding");
                BiddingConvite convite = repBidding.BuscarPorCodigo(codigoBidding, false);
                BiddingConviteConvidado conviteConvidado = repConviteConvidado.BuscarConvidado(convite, this.Usuario.Empresa);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                List<BiddingAceitamentoQuestaoResposta> listaRespostasExistentes = repResposta.BuscarPorTransportadorBidding(this.Usuario.Empresa, convite);
                BiddingChecklistBiddingTransportador checklistBiddingTransportador = repChecklistBiddingTransportador.BuscarPorBiddingTransportador(this.Usuario.Empresa, convite);

                if (checklistBiddingTransportador == null)
                    checklistBiddingTransportador = new BiddingChecklistBiddingTransportador();

                checklistBiddingTransportador.BiddingConvite = convite;
                checklistBiddingTransportador.Transportador = this.Usuario.Empresa;
                checklistBiddingTransportador.DataRetorno = DateTime.Now;
                checklistBiddingTransportador.Situacao = StatusBiddingConviteTransportadorRespostas.Aguardando;
                checklistBiddingTransportador.Aceitamento = "Aguardando";
                checklistBiddingTransportador.AceitamentoDesejavel = "Aguardando";

                if (checklistBiddingTransportador.Codigo > 0)
                    repChecklistBiddingTransportador.Atualizar(checklistBiddingTransportador);
                else
                    repChecklistBiddingTransportador.Inserir(checklistBiddingTransportador);

                foreach (BiddingAceitamentoQuestaoResposta respostaExistente in listaRespostasExistentes)
                {
                    if (respostaExistente.Pergunta.Checklist.DataLimite < DateTime.Now)
                    {
                        ModificarStatusConvidado(conviteConvidado, StatusBiddingConviteConvidado.Rejeitado, unitOfWork);
                        unitOfWork.CommitChanges();
                        throw new ControllerException("Prazo esgotado, o convite foi automaticamente rejeitado.");
                    }
                    repRespostaAnexo.DeletarTodosPorCodigo(respostaExistente.Codigo);
                    repResposta.Deletar(respostaExistente);
                }

                List<BiddingAceitamentoQuestaoResposta> codigosResposta = new List<BiddingAceitamentoQuestaoResposta>();

                float contadorFalsas = 0, contadorTotal = 0, contadorFalsasDesejavel = 0, contadorTotalDesejavel = 0;

                foreach (dynamic dado in dados)
                {
                    BiddingAceitamentoQuestaoResposta resposta = new BiddingAceitamentoQuestaoResposta();
                    int codigoPergunta = dado.CodigoPergunta;
                    BiddingChecklistQuestionario pergunta = repChecklistQuestionario.BuscarPorCodigo(codigoPergunta, false);
                    if (pergunta.Checklist.DataLimite < DateTime.Now)
                    {
                        ModificarStatusConvidado(conviteConvidado, StatusBiddingConviteConvidado.Rejeitado, unitOfWork);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(false, "Prazo esgotado, o convite foi automaticamente rejeitado.");
                    }
                    resposta.Pergunta = pergunta;
                    resposta.Observacao = dado.Observacao;
                    resposta.Resposta = dado.Resposta;

                    if (pergunta.Requisito == TipoRequisitoBiddingChecklist.Indispensavel)
                        contadorTotal += 1;
                    if (pergunta.Requisito == TipoRequisitoBiddingChecklist.Desejavel)
                        contadorTotalDesejavel += 1;

                    if (dado.Resposta != true && pergunta.Requisito == TipoRequisitoBiddingChecklist.Indispensavel)
                        contadorFalsas += 1;
                    if (dado.Resposta != true && pergunta.Requisito == TipoRequisitoBiddingChecklist.Desejavel)
                        contadorFalsasDesejavel += 1;

                    resposta.ChecklistBiddingTransportador = checklistBiddingTransportador;
                    repResposta.Inserir(resposta);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pergunta, null, $"Respondeu a pergunta " + pergunta.Descricao + ".", unitOfWork);
                    codigosResposta.Add(resposta);
                }

                float porcentagemAceitacao = contadorTotal == 0 ? 100 : (contadorTotal - contadorFalsas) / contadorTotal * 100;
                float porcentagemAceitacaoDesejavel = contadorTotalDesejavel == 0 ? 100 : (contadorTotalDesejavel - contadorFalsasDesejavel) / contadorTotalDesejavel * 100;
                checklistBiddingTransportador.Aceitamento = $"{porcentagemAceitacao.ToString("n0")}%";
                checklistBiddingTransportador.AceitamentoDesejavel = $"{porcentagemAceitacaoDesejavel.ToString("n0")}%";

                if (porcentagemAceitacao == 100)
                {
                    checklistBiddingTransportador.Situacao = StatusBiddingConviteTransportadorRespostas.Aprovado;
                    conviteConvidado.StatusBidding = StatusBiddingConvite.Ofertas;

                    BiddingConviteConvidado convidado = new BiddingConviteConvidado
                    {
                        Convidado = checklistBiddingTransportador.Transportador
                    };

                    servicoBidding.NotificarConvidado($"Você foi automaticamente aprovado na etapa de Checklist. Aguarde o início da próxima etapa.", "Aviso Bidding - Checklist", convidado);
                }

                repConviteConvidado.Atualizar(conviteConvidado);

                if (checklistBiddingTransportador.Codigo > 0)
                    repChecklistBiddingTransportador.Atualizar(checklistBiddingTransportador);
                else
                    repChecklistBiddingTransportador.Inserir(checklistBiddingTransportador);

                ValidacoesParaEnviarRotas(configuracaoBidding, porcentagemAceitacao, convite, servicoBidding, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding)
                {
                    conviteConvidado.StatusBidding = StatusBiddingConvite.Ofertas;

                    Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConviteConvidado = conviteConvidado.BiddingConvite;

                    biddingConviteConvidado.Status = StatusBiddingConvite.Ofertas;

                    repConviteConvidado.Atualizar(conviteConvidado);
                    repBidding.Atualizar(biddingConviteConvidado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    CodigosRespostas = (
                        from resp in codigosResposta
                        select new
                        {
                            resp.Codigo,
                            CodigoPergunta = resp.Pergunta.Codigo
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar as respostas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarOferta(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoBiddingConvite = Request.GetIntParam("CodigoBiddingConvite");

                decimal valorCalculoTotalLiquido = 0.9075M;

                decimal freteComICMSPeso = 0;
                decimal pedagioComICMSPeso = 0;
                decimal totalBrutoPeso = 0;
                decimal totalLiquidoPeso = 0;

                decimal freteComICMSCapacidadeTon = 0;
                decimal freteComICMSCapacidade = 0;
                decimal pedagioComICMSCapacidade = 0;
                decimal totalBrutoCapacidade = 0;
                decimal totalLiquidoCapacidade = 0;

                decimal totalBrutoViagem = 0;
                decimal totalLiquidoViagem = 0;

                decimal viagemComPedagio = 0;
                decimal adicionalAjudanteComICMS = 0;

                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork, cancellationToken);

                BiddingConvite grupoBiddingConvite = await repositorioBiddingConvite.BuscarPorCodigoAsync(codigoBiddingConvite, false);
                List<BiddingTransportadorRotaOferta> listaOfertas = await repositorioTransportadorOferta.BuscarPorRotaAsync(codigo, this.Usuario.Empresa.Codigo);

                bool naoIncluirICMS = grupoBiddingConvite.TipoBidding.NaoIncluirImpostoValorTotalOferta;

                foreach (BiddingTransportadorRotaOferta oferta in listaOfertas)
                {
                    Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                    decimal porcentagemICMS = servicoBidding.ObterPorcentagemICMSCalculada(oferta.ICMSPorcentagem, naoIncluirICMS);

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorPeso)
                    {
                        freteComICMSPeso = (oferta.FreteTonelada / porcentagemICMS);
                        pedagioComICMSPeso = ((oferta.PedagioParaEixo / porcentagemICMS) * oferta.ModeloVeicular.NumeroEixos) ?? 0;
                        totalBrutoPeso = pedagioComICMSPeso > 0 ? freteComICMSPeso + (pedagioComICMSPeso / (oferta.ModeloVeicular.CapacidadePesoTransporte / 1000)) : freteComICMSPeso;
                        totalLiquidoPeso = Math.Round((totalBrutoPeso * porcentagemICMS * valorCalculoTotalLiquido), 2);
                    }

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorCapacidade)
                    {
                        freteComICMSCapacidadeTon = (oferta.FreteTonelada / porcentagemICMS);
                        freteComICMSCapacidade = freteComICMSCapacidadeTon * (oferta.ModeloVeicular.CapacidadePesoTransporte / 1000);
                        pedagioComICMSCapacidade = ((oferta.PedagioParaEixo / porcentagemICMS) * oferta.ModeloVeicular.NumeroEixos) ?? 0;
                        totalBrutoCapacidade = pedagioComICMSCapacidade > 0 ? (freteComICMSCapacidade + pedagioComICMSCapacidade) : freteComICMSCapacidade;
                        totalLiquidoCapacidade = Math.Round((totalBrutoCapacidade * porcentagemICMS * valorCalculoTotalLiquido), 2);
                    }

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorFreteViagem)
                    {
                        totalBrutoViagem = (oferta.FreteTonelada / porcentagemICMS) + (oferta.PedagioParaEixo / porcentagemICMS);
                        totalLiquidoViagem = Math.Round((totalBrutoViagem * porcentagemICMS * valorCalculoTotalLiquido), 2);
                    }

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorViagemEntregaAjudante)
                    {
                        viagemComPedagio = (oferta.FreteTonelada + oferta.PedagioParaEixo) / porcentagemICMS;
                        adicionalAjudanteComICMS = (oferta.FreteTonelada + oferta.PedagioParaEixo + oferta.Ajudante) / porcentagemICMS;
                    }
                }

                return new JsonpResult(new
                {
                    NaoIncluirImpostoValorTotalOferta = grupoBiddingConvite != null ? grupoBiddingConvite.TipoBidding.NaoIncluirImpostoValorTotalOferta : false,
                    Rodada = listaOfertas.Count > 0 ? $"{listaOfertas[0].Rodada}ª" : "",
                    Codigo = codigo,
                    Tabs = (
                    from o in listaOfertas
                    select new
                    {
                        Identificador = $"#tabDetalhe{(int)o.TipoOferta}"
                    }),
                    Equipamento = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorEquipamento
                    select new
                    {
                        o.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        ValorMes = o.ValorFixoEquipamento,
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    PorcentagemSobreNota = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorcentagemNota
                    select new
                    {
                        o.Codigo,
                        o.CustoEstimado,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        o.Porcentagem,
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    FrotaFixaKm = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LanceFrotaFixaKmRodado
                    select new
                    {
                        o.Codigo,
                        o.CustoEstimado,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        ValorFixo = o.ValorFixoMensal,
                        ValorKm = o.ValorKmRodado,
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    FrotaFixaFranquia = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LanceFrotaFixaFranquia
                    select new
                    {
                        o.Codigo,
                        o.CustoEstimado,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        o.ValorFixo,
                        o.ValorFranquia,
                        o.Quilometragem,
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    ViagemAdicional = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LanceViagemAdicional
                    select new
                    {
                        o.Codigo,
                        o.CustoEstimado,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        o.ValorViagem,
                        Adicional = o.ValorEntrega,
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    FretePorPeso = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorPeso
                    select new
                    {
                        o.Codigo,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        NumeroEixos = o.ModeloVeicular.NumeroEixos,
                        Capacidade = o.ModeloVeicular.CapacidadePesoTransporte.ToString("n2"),
                        ICMS = o.ICMSPorcentagem,
                        FreteTonelada = o.FreteTonelada.ToString("n2"),
                        PedagioEixo = o.PedagioParaEixo.ToString("n2"),
                        FreteComICMS = o.FreteTonelada != 0 ? freteComICMSPeso.ToString("n2") : 0.ToString("n2"),
                        PedagioComICMS = o.PedagioParaEixo != 0 ? pedagioComICMSPeso.ToString("n2") : 0.ToString("n2"),
                        TotalBruto = freteComICMSPeso != 0 ? totalBrutoPeso.ToString("n2") : 0.ToString("n2"),
                        TotalLiquido = totalBrutoPeso != 0 ? totalLiquidoPeso.ToString("n2") : 0.ToString("n2"),
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    FretePorCapacidade = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorCapacidade
                    select new
                    {
                        o.Codigo,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        NumeroEixos = o.ModeloVeicular.NumeroEixos,
                        Capacidade = o.ModeloVeicular.CapacidadePesoTransporte.ToString("n2"),
                        ICMS = o.ICMSPorcentagem,
                        FreteTonelada = o.FreteTonelada.ToString("n2"),
                        PedagioEixo = o.PedagioParaEixo.ToString("n2"),
                        FreteComICMS = o.FreteTonelada != 0 ? freteComICMSCapacidade.ToString("n2") : 0.ToString("n2"),
                        PedagioComICMS = o.PedagioParaEixo != 0 ? pedagioComICMSCapacidade.ToString("n2") : 0.ToString("n2"),
                        TotalBruto = totalBrutoCapacidade != 0 ? totalBrutoCapacidade.ToString("n2") : 0.ToString("n2"),
                        TotalLiquido = totalBrutoCapacidade != 0 ? totalLiquidoCapacidade.ToString("n2") : 0.ToString("n2"),
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    FretePorViagem = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorFreteViagem
                    select new
                    {
                        o.Codigo,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        NumeroEixos = o.ModeloVeicular.NumeroEixos,
                        Capacidade = o.ModeloVeicular.CapacidadePesoTransporte.ToString("n2"),
                        ICMS = o.ICMSPorcentagem,
                        FreteTonelada = o.FreteTonelada.ToString("n2"),
                        PedagioEixo = o.PedagioParaEixo.ToString("n2"),
                        TotalBruto = (o.FreteTonelada != 0 && o.ICMSPorcentagem != 0 && o.PedagioParaEixo != 0) ? totalBrutoViagem.ToString("n2") : o.FreteTonelada.ToString("n2"),
                        TotalLiquido = (totalBrutoViagem != 0) ? totalLiquidoViagem.ToString("n2") : 0.ToString("n2"),
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    }),
                    ViagemEntregaAjudante = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorViagemEntregaAjudante
                    select new
                    {
                        o.Codigo,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        NumeroEixos = o.ModeloVeicular.NumeroEixos,
                        Capacidade = o.ModeloVeicular.CapacidadePesoTransporte,
                        ICMS = o.ICMSPorcentagem,
                        FreteTonelada = o.FreteTonelada.ToString("n2"),
                        PedagioEixo = o.PedagioParaEixo.ToString("n2"),
                        Ajudante = o.Ajudante.ToString("n2"),
                        AdicionalPorEntrega = o.AdicionalPorEntrega.ToString("n2"),
                        ViagemComPedagio = viagemComPedagio.ToString("n2") ?? 0.ToString("n2"),
                        AdicionalAjudanteComICMS = adicionalAjudanteComICMS.ToString("n2") ?? 0.ToString("n2"),
                        VeiculosVerdes = o.VeiculosVerdes,
                        InformarVeiculosVerdes = o.InformarVeiculosVerdes
                    })
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> EnviarOfertas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            try
            {
                Servicos.Embarcador.Bidding.Bidding serBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork, Auditado, Usuario);
                List<OfertaImportacao> ofertas = JsonConvert.DeserializeObject<List<OfertaImportacao>>(Request.Params("Ofertas"));
                int codigoOfertaRota = Request.GetIntParam("CodigoOfertaRota");
                int veiculosVerdes = Request.GetIntParam("VeiculosVerdes");
                bool informarVeiculosVerdes = Request.GetBoolParam("InformarVeiculosVerdes");

                var primeiraOferta = ofertas.FirstOrDefault();
                if (primeiraOferta != null)
                {
                    primeiraOferta.BiddingTransportadorRota = codigoOfertaRota;
                    primeiraOferta.ModeloVeicular = await repositorioModeloVeiculo.BuscarPorCodigoAsync(primeiraOferta.Oferta.ModeloVeicular, false);
                }

                serBidding.EnviarOfertas(ofertas, codigoOfertaRota, veiculosVerdes, informarVeiculosVerdes);



                BiddingTransportadorRota transportadorRota = await repTransportadorRota.BuscarPorCodigoAsync(codigoOfertaRota, false);
                List<BiddingTransportadorRota> listaTransportadorRotas = repTransportadorRota.BuscarPorOferta(transportadorRota.Rota.BiddingOferta, this.Usuario.Empresa.Codigo);

                return new JsonpResult(new
                {
                    OfertasRotas = (
                        from rota in listaTransportadorRotas
                        select new
                        {
                            rota.Codigo,
                            CodigoRota = rota.Rota.Codigo,
                            Rota = rota.Rota.Descricao,
                            ValorTransportado = rota.Rota.ValorCargaMes,
                            VolumeTransportado = rota.Rota.Volume,
                            PesoTransportado = rota.Rota.Peso,
                            FrequenciaMensal = rota.Rota.Frequencia,
                            NumeroEntregas = rota.Rota.NumeroEntrega,
                            rota.Rota.QuilometragemMedia,
                            rota.Rota.AdicionalAPartirDaEntregaNumero,
                            TipoCarga = (
                                from tipoCarga in rota.Rota.TiposCarga
                                select new
                                {
                                    tipoCarga.Descricao
                                }
                            ).ToList(),
                            ModelosVeiculares = (
                                from modeloVeicular in rota.Rota.ModelosVeiculares
                                select new
                                {
                                    modeloVeicular.Descricao,
                                    modeloVeicular.Codigo,
                                    NomeTabEquipamento = "#Equipamento" + modeloVeicular.Codigo,
                                    NomeTabFrotaFixaKmRodado = "#FrotaFixaKmRodado" + modeloVeicular.Codigo,
                                    NomeTabPorcentagemSobreNota = "#PorcentagemNota" + modeloVeicular.Codigo,
                                    NomeTabViagemAdicional = "#ViagemAdicional" + modeloVeicular.Codigo,
                                    NomeTabFrotaFixaFranquia = "#FrotaFixaFranquia" + modeloVeicular.Codigo,
                                    NomeTabFretePorPeso = "#FretePorPeso" + modeloVeicular.Codigo,
                                    NomeTabFretePorCapacidade = "#FretePorCapacidade" + modeloVeicular.Codigo,
                                    NomeTabFretePorViagem = "#FretePorViagem" + modeloVeicular.Codigo,
                                    NomeTabViagemEntregaAjudante = "#ViagemEntregaAjudante" + modeloVeicular.Codigo
                                }
                            ).ToList(),
                            Rodada = rota.Rodada + "ª Rodada",
                            Target = "R$ " + rota.Target.ToString("n2"),
                            SituacaoCodigo = rota.Status,
                            Origem = rota.Rota.DescricaoOrigem,
                            Destino = rota.Rota.DescricaoDestino,
                            ModeloVeicular = string.Join(", ", rota.Rota.ModelosVeiculares.Select(o => o.Descricao)),
                            Situacao = rota.Status.ObterDescricao(),
                            DT_RowColor = rota.Status.ObterCorLinha(),
                            DT_FontColor = rota.Status.ObterCorFonte()
                        }
                    ).ToList(),
                });
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }

        public async Task<IActionResult> PesquisarResultados()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaResultados());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ExportarPesquisaResultados()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaResultados();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> PesquisarAvaliacaoOfertasTransportadores()
        {
            try
            {
                return new JsonpResult(ObterGridOfertaAvaliacaoTransportadores());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ExportarPesquisaOfertaAvaliacaoTransportadores()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridOfertaAvaliacaoTransportadores(true);

                List<Dominio.ObjetosDeValor.Grid.HeaderExportacao> headersExportacao = grid.ObterHeadersExportacao(grid.header);

                headersExportacao.RemoveAll(x => !x.visible);

                Dictionary<string, Color> coresFixas = new Dictionary<string, Color>
                {
                    { "ValorAnterior", Color.SkyBlue },
                    { "Target", Color.SkyBlue },
                    { "ComponenteOfertado1Transportador1", Color.Yellow },
                    { "ComponenteOfertado2Transportador1", Color.Yellow },
                    { "ComponenteOfertado3Transportador1", Color.Yellow },
                    { "ComponenteOfertado4Transportador1", Color.Yellow },
                    { "ComponenteOfertado5Transportador1", Color.Yellow }
                };

                List<dynamic> dados = (List<dynamic>)grid.data;

                byte[] arquivoBinario = new Servicos.Embarcador.Exportacao.ExcelExport().GerarExcelGenerico(dados, headersExportacao, coresFixas, "Exportacao");

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OfertaAvaliacaoTransportadores.xlsx");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> ObterTemplateOferta()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridTemplateOferta();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", "TemplateOfertaTransportador." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
        }

        public async Task<IActionResult> ObterResumosBidding()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                DateTime dataLimite = Request.GetDateTimeParam("DataLimite");

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);

                Dominio.Entidades.Empresa empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa : null;

                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisaResumoAgConvite = new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding()
                {
                    Empresa = empresa,
                    Situacao = new List<StatusBiddingConvite>() { StatusBiddingConvite.Aguardando },
                    DataInicio = dataInicial,
                    DataLimite = dataLimite
                };
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisaResumoAgChecklist = new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding()
                {
                    Empresa = empresa,
                    Situacao = new List<StatusBiddingConvite>() { StatusBiddingConvite.Checklist },
                    DataInicio = dataInicial,
                    DataLimite = dataLimite
                };
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisaResumoAgOfertas = new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding()
                {
                    Empresa = empresa,
                    Situacao = new List<StatusBiddingConvite>() { StatusBiddingConvite.Ofertas },
                    DataInicio = dataInicial,
                    DataLimite = dataLimite
                };
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisaResumoFinalizados = new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding()
                {
                    Empresa = empresa,
                    Situacao = new List<StatusBiddingConvite>() { StatusBiddingConvite.Fechamento },
                    DataInicio = dataInicial,
                    DataLimite = dataLimite
                };

                int totalAgConvite = repositorioBiddingConvite.ContarConsulta(filtrosPesquisaResumoAgConvite);
                int totalAgChecklist = repositorioBiddingConvite.ContarConsulta(filtrosPesquisaResumoAgChecklist);
                int totalAgOfertas = repositorioBiddingConvite.ContarConsulta(filtrosPesquisaResumoAgOfertas);
                int totalFinalizados = repositorioBiddingConvite.ContarConsulta(filtrosPesquisaResumoFinalizados);

                var retorno = new
                {
                    ResumoTodos = totalAgConvite + totalAgChecklist + totalAgOfertas + totalFinalizados,
                    ResumoAgConvite = totalAgConvite,
                    ResumoAgChecklist = totalAgChecklist,
                    ResumoAgOfertas = totalAgOfertas,
                    ResumoFinalizados = totalFinalizados
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao buscar os resumos de bidding");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportarOfertas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Servicos.Embarcador.Bidding.Bidding svcBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork, Auditado, Usuario);

            return new JsonpResult(svcBidding.ObterConfiguracaoImportacaoOferta());
        }

        public async Task<IActionResult> ImportarOfertas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Bidding.Bidding svcBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork, Auditado, Usuario);
                string dados = Request.GetStringParam("Dados");

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = await svcBidding.ImportarOfertasAsync(dados);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private void ValidacoesParaEnviarRotas(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding, float porcentagemAceitacao, Dominio.Entidades.Embarcador.Bidding.BiddingConvite convite, Servicos.Embarcador.Bidding.Bidding servicoBidding, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoBidding.PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento)
            {
                if (porcentagemAceitacao >= configuracaoBidding.InformePorcentagemAceitacaoInd)
                {
                    servicoBidding.EnviarRotas(convite, this.Usuario, unitOfWork);
                }
            }
            else
            {
                if (porcentagemAceitacao >= 100)
                {
                    servicoBidding.EnviarRotas(convite, this.Usuario, unitOfWork);
                }
            }
        }

        private Models.Grid.Grid ObterGridPesquisaResultados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Rota", "Rota", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Oferta", "Oferta", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Rodada", "Rodada", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Ofertado", "ValorOfertado", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Titularidade", "Titularidade", 10, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                int codigoRota = Request.GetIntParam("Rota");
                int situacao = Request.GetIntParam("Situacao");

                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repOfertas = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                List<BiddingTransportadorRotaOferta> listaOfertas = repOfertas.BuscarPorTransportadorOferta(this.Usuario.Empresa.Codigo, codigo, codigoRota, situacao);

                int totalRegistros = listaOfertas.Count();


                //grid.ObterParametrosConsulta();

                var retorno = (from o in listaOfertas
                               select new
                               {
                                   o.Codigo,
                                   Rota = o.TransportadorRota.Rota.Descricao,
                                   Origem = o.TransportadorRota.Rota.DescricaoOrigem,
                                   Destino = o.TransportadorRota.Rota.DescricaoDestino,
                                   ModeloVeicular = o.ModeloVeicular.Descricao,
                                   Oferta = o.Descricao,
                                   o.TransportadorRota.Rodada,
                                   Situacao = o.Aceito ? "Aprovada" : "Não Aprovada",
                                   Titularidade = o.TipoTransportador.HasValue ? o.TipoTransportador.Value.ObterDescricao() : "",
                                   ValorOfertado = o.CustoEstimado,
                                   DT_RowColor = o.Aceito ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho,
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridOfertaAvaliacaoTransportadores(bool adicionarCamposExportarExcel = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int maximoColunaTransportador = 20;

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                Servicos.Embarcador.Bidding.Bidding svcBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork, Auditado, Usuario);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>(),
                    scrollHorizontal = true
                };

                int totalRegistros = repTransportadorRota.ContarPorBiddingRotas(codigo);
                List<BiddingTransportadorRota> listaTransportadorRota = totalRegistros > 0 ? repTransportadorRota.BuscarPorBiddingRotas(codigo, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Empresa.Codigo : 0) : new List<BiddingTransportadorRota>();

                List<Dominio.ObjetosDeValor.Embarcador.Bidding.ListaBiddingOfertaAceitamento> listaTotalOfertas = repTransportadorOferta.BuscarValoresComponentesPorCodigosTransportadorRota(listaTransportadorRota.Select(obj => obj.Codigo).ToList());
                List<(int codigoTransportador, decimal custoEstimadoTotal)> totalPorTransportador = listaTotalOfertas.GroupBy(o => o.CodigoTransportador).Select(o => ValueTuple.Create(o.Key, o.Sum(x => x.CustoEstimado))).OrderBy(o => o.Item2).ToList();

                List<Dominio.Entidades.Empresa> transportadores = listaTransportadorRota.Select(o => o.Transportador).Distinct().OrderBy(o =>
                {
                    return totalPorTransportador.FindIndex(x => x.codigoTransportador == o.Codigo);
                }).ToList();

                List<BiddingOfertaRota> rotas = listaTransportadorRota.Select(o => o.Rota).Distinct().ToList();
                totalRegistros = rotas.Count;

                List<TipoLanceBidding> tipoLanceBiddingsCostumizacaoColunas = new List<TipoLanceBidding> { TipoLanceBidding.LancePorCapacidade,
                    TipoLanceBidding.LancePorPeso,
                    TipoLanceBidding.LancePorFreteViagem
                };

                TipoLanceBidding tipoLanceBidding = totalRegistros > 0 ? rotas.First().BiddingOferta.TipoLance : new TipoLanceBidding();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCodigo", false);
                grid.AdicionarCabecalho("NaoOfertar", false);
                grid.AdicionarCabecalho("Protocolo Importação", "ProtocoloImportacao", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Situação", "Situacao", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Rota", "Rota", 3, Models.Grid.Align.center);
                if (!adicionarCamposExportarExcel)
                {
                    grid.AdicionarCabecalho("Origem", "Origem", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Destino", "Destino", 3, Models.Grid.Align.center);
                }
                grid.AdicionarCabecalho(nameof(ConsultaBiddingOfertaAceitamento.CodigoModeloVeicular), false);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Rodada", "Rodada", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Valor Alvo", "Target", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Valor Anterior", "ValorAnterior", 3, Models.Grid.Align.center);

                if (adicionarCamposExportarExcel)
                {
                    grid.AdicionarCabecalho("Origem", "Origem", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Origem UF", "OrigemUF", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Região Origem", "OrigemRegiaoBrasil", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Origem Mesorregião", "OrigemMesorregiao", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Destino", "Destino", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Destino UF", "DestinoUF", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Região Destino", "DestinoRegiaoBrasil", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Destino Mesorregião", "DestinoMesorregiao", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("KM Média da Rota", "Quilometragem", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Grupo Modelo Veicular", "GrupoModeloVeicular", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Modelo Carroceria Veículo", "Carroceria", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Tomador", "Tomador", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Tipos Carga", "MaterialTransportado", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Filiais Participantes", "UnidadeNegocio", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Quantidade de Cargas Ano", "QuantidadeViagensAno", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Volume (Ton) Ano", "Volume", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Incoterm", "Incoterm", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Quantidade de Entregas", "QtdEntrega", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Quantidade de Ajudantes por Veículo", "QtdAjudante", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Tempo de Coleta", "TempoColeta", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Tempo de Descarga", "TempoDescarga", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Compressor", "Compressor", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("Valor Médio NF-e", "MediaValorNF", 3, Models.Grid.Align.center);
                    grid.AdicionarCabecalho("CNPJ Transportador", "FilialParticipante", 3, Models.Grid.Align.center);
                }

                AdicionarColunasDinamicasTransportador(grid, transportadores, listaTotalOfertas, ref maximoColunaTransportador, out List<int> codigosTransportadores, adicionarCamposExportarExcel, tipoLanceBidding);

                listaTransportadorRota = listaTransportadorRota.OrderByDescending(o => o.Codigo).ToList();

                List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento>();

                foreach (BiddingOfertaRota rota in rotas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento itemRetorno = new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento()
                    {
                        Codigo = rota.Codigo,
                        Rota = rota.Descricao,
                        Origem = rota.DescricaoOrigem,
                        Destino = rota.DescricaoDestino,
                        CodigoModeloVeicular = rota.ModelosVeiculares.FirstOrDefault()?.Codigo,
                        ModeloVeicular = string.Join(", ", rota.ModelosVeiculares.Select(p => p.Descricao)),
                        OrigemUF = string.Join(", ", rota.Origens.Select(p => p.Estado?.Descricao)),
                        OrigemRegiaoBrasil = string.Join(",", svcBidding.ObterRegiaoBrasilRota(rota, Dominio.Enumeradores.OpcaoSimNao.Sim).Select(p => p.Descricao)),
                        OrigemMesorregiao = string.Join(", ", rota.Origens.Select(p => p.Regiao?.Descricao)),
                        DestinoUF = string.Join(", ", rota.Destinos.Select(p => p.Estado?.Descricao)),
                        DestinoRegiaoBrasil = string.Join(",", svcBidding.ObterRegiaoBrasilRota(rota, Dominio.Enumeradores.OpcaoSimNao.Nao).Select(p => p.Descricao)),
                        DestinoMesorregiao = string.Join(", ", rota.Destinos.Select(p => p.Regiao?.Descricao)),
                        DT_RowColor = null
                    };

                    BiddingTransportadorRota transportadorRota = listaTransportadorRota.Find(x => x.Rota.Codigo == rota.Codigo);

                    if (transportadorRota != null)
                    {
                        itemRetorno.Target = transportadorRota.Target.ToString("n2");
                        itemRetorno.Rodada = $"{transportadorRota.Rodada}ª Rodada";
                        itemRetorno.Situacao = transportadorRota.Status.ObterDescricao();
                        itemRetorno.SituacaoCodigo = transportadorRota.Status;
                        itemRetorno.ValorAnterior = transportadorRota.ValorAnterior.ToString("n2");
                        itemRetorno.QtdAjudante = transportadorRota.Rota.QuantidadeAjudantePorVeiculo.ToString();
                        itemRetorno.QtdEntrega = transportadorRota.Rota.NumeroEntrega.ToString();
                        itemRetorno.ProtocoloImportacao = transportadorRota.Rota.ProtocoloImportacao.ToString();
                        itemRetorno.Quilometragem = transportadorRota.Rota.QuilometragemMedia.ToString();
                        itemRetorno.GrupoModeloVeicular = transportadorRota.Rota.GrupoModeloVeicular?.Descricao;
                        itemRetorno.Carroceria = transportadorRota.Rota.ModeloCarroceria?.Descricao;
                        itemRetorno.Tomador = transportadorRota.Rota.Tomador != null ? $"{transportadorRota.Rota.Tomador?.Nome} - {transportadorRota.Rota.Tomador?.CPF_CNPJ_Formatado}" : "";
                        itemRetorno.MaterialTransportado = string.Join(", ", transportadorRota.Rota.TiposCarga.Select(tipoCarga => tipoCarga.Descricao));
                        itemRetorno.UnidadeNegocio = string.Join(", ", transportadorRota.Rota.FiliaisParticipante.Select(filial => $"{filial.Descricao} - {filial.CNPJ_Formatado}"));
                        itemRetorno.QuantidadeViagensAno = transportadorRota.Rota.QuantidadeViagensPorAno.ToString();
                        itemRetorno.Volume = transportadorRota.Rota.VolumeTonAno.ToString();
                        itemRetorno.Incoterm = transportadorRota.Rota.Inconterm?.ObterDescricao();
                        itemRetorno.TempoColeta = transportadorRota.Rota.TempoColeta?.ToString();
                        itemRetorno.TempoDescarga = transportadorRota.Rota.TempoDescarga?.ToString();
                        itemRetorno.Compressor = transportadorRota.Rota.Compressor?.ObterDescricao();
                        itemRetorno.MediaValorNF = transportadorRota.Rota.ValorMedioNFe.ToString("n2");
                        itemRetorno.FilialParticipante = transportadorRota.Transportador.CNPJ_Formatado;
                        itemRetorno.NaoOfertar = false;
                        itemRetorno.DT_RowColor = transportadorRota.Status.ObterCorLinha();
                        itemRetorno.AlicotaPadraoICMS = transportadorRota.Rota.AlicotaPadraoICMS;

                        if (listaTotalOfertas.Find(o => o.CodigoTransportador == transportadorRota.Transportador.Codigo && o.CodigoRota == transportadorRota.Codigo) != null)
                            itemRetorno.NaoOfertar = listaTotalOfertas.Find(o => o.CodigoTransportador == transportadorRota.Transportador.Codigo && o.CodigoRota == transportadorRota.Rota.Codigo).NaoOfertar;

                    }
                    retorno.Add(itemRetorno);
                }

                if (totalRegistros > 0)
                {
                    retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento()
                    {
                        Codigo = 0,
                        RotaDescricao = "TOTAL",
                        ProtocoloImportacao = "",
                    });
                    PreencherColunasDinamicasTransportador(retorno, listaTotalOfertas, totalPorTransportador, codigosTransportadores, maximoColunaTransportador, totalRegistros, tipoLanceBidding);
                }

                if (adicionarCamposExportarExcel)
                {
                    if (tipoLanceBiddingsCostumizacaoColunas.Contains(tipoLanceBidding))
                        grid.OcultarCabecalho("OrigemMesorregiao");

                    if (tipoLanceBiddingsCostumizacaoColunas.Contains(tipoLanceBidding))
                        grid.OcultarCabecalho("DestinoMesorregiao");

                    if (!tipoLanceBiddingsCostumizacaoColunas.Contains(tipoLanceBidding))
                        grid.OcultarCabecalho("TempoColeta");

                    if (!tipoLanceBiddingsCostumizacaoColunas.Contains(tipoLanceBidding))
                        grid.OcultarCabecalho("TempoDescarga");

                    List<TipoLanceBidding> tiposParaRemoverMesorregiao =
                    [
                        TipoLanceBidding.LancePorCapacidade,
                        TipoLanceBidding.LancePorPeso,
                        TipoLanceBidding.LancePorFreteViagem,
                        TipoLanceBidding.LancePorViagemEntregaAjudante, 
                        TipoLanceBidding.LanceViagemAdicional,
                    ];

                    if (tiposParaRemoverMesorregiao.Contains(tipoLanceBidding))
                    {
                        grid.OcultarCabecalho("OrigemMesorregiao");
                        grid.OcultarCabecalho("DestinoMesorregiao");
                    }

                    if (tipoLanceBidding == TipoLanceBidding.LancePorViagemEntregaAjudante)
                    {
                        grid.OcultarCabecalho("TempoColeta");
                        grid.OcultarCabecalho("TempoDescarga");
                    }
                }

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros + 1);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridTemplateOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                int totalRegistros = repTransportadorRota.ContarPorBiddingRotas(codigo);

                List<BiddingTransportadorRota> listaTransportadorRota = totalRegistros > 0 ? repTransportadorRota.BuscarPorBiddingRotas(codigo, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Empresa.Codigo : 0) : new List<BiddingTransportadorRota>();

                List<BiddingOfertaRota> rotas = listaTransportadorRota.Select(o => o.Rota).Distinct().ToList();
                totalRegistros = rotas.Count;

                TipoLanceBidding tipoLanceBidding = totalRegistros > 0 ? rotas.First().BiddingOferta.TipoLance : new TipoLanceBidding();

                grid.AdicionarCabecalho("Protocolo Importação", "ProtocoloImportacao", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 3, Models.Grid.Align.center);
                grid.AdicionarCabecalho("ICMS(%)", "AliquotaICMS", 3, Models.Grid.Align.center);

                if (new List<TipoLanceBidding> { TipoLanceBidding.LanceFrotaFixaFranquia, TipoLanceBidding.LancePorEquipamento, TipoLanceBidding.LancePorFreteViagem, TipoLanceBidding.LancePorViagemEntregaAjudante }.Contains(tipoLanceBidding))
                    grid.AdicionarCabecalho("Valor Fixo", "ValorFixo", 3, Models.Grid.Align.center);

                if (new List<TipoLanceBidding> { TipoLanceBidding.LancePorCapacidade, TipoLanceBidding.LancePorPeso }.Contains(tipoLanceBidding))
                    grid.AdicionarCabecalho("Valor por Tonelada", "FreteTonelada", 3, Models.Grid.Align.center);

                if (new List<TipoLanceBidding> { TipoLanceBidding.LancePorViagemEntregaAjudante, TipoLanceBidding.LanceViagemAdicional }.Contains(tipoLanceBidding))
                    grid.AdicionarCabecalho("Adicional Por Entrega", "AdicionalEntrega", 3, Models.Grid.Align.center);

                if (tipoLanceBidding == TipoLanceBidding.LanceFrotaFixaKmRodado)
                    grid.AdicionarCabecalho("Valor Fixo Mensal", "ValorFixoMensal", 3, Models.Grid.Align.center);

                if (tipoLanceBidding == TipoLanceBidding.LancePorcentagemNota)
                    grid.AdicionarCabecalho("Porcentagem sobre Nota", "PorcentagemNota", 3, Models.Grid.Align.center);

                if (tipoLanceBidding == TipoLanceBidding.LanceViagemAdicional)
                    grid.AdicionarCabecalho("Valor por Viagem", "ValorViagem", 3, Models.Grid.Align.center);

                if (tipoLanceBidding == TipoLanceBidding.LanceFrotaFixaFranquia)
                {
                    grid.AdicionarCabecalho("Valor por Franquia", "ValorFranquia", 3, Models.Grid.Align.center);
                }

                grid.AdicionarCabecalho("Quilometragem", "Quilometragem", 3, Models.Grid.Align.center);

                if (tipoLanceBidding == TipoLanceBidding.LanceFrotaFixaKmRodado)
                    grid.AdicionarCabecalho("Valor KM Rodado", "ValorKmRodado", 3, Models.Grid.Align.center);

                if (new List<TipoLanceBidding> { TipoLanceBidding.LancePorCapacidade, TipoLanceBidding.LancePorPeso, TipoLanceBidding.LancePorViagemEntregaAjudante, TipoLanceBidding.LancePorFreteViagem }.Contains(tipoLanceBidding))
                    grid.AdicionarCabecalho("Pedágio/Eixo ", "ValorPedagio", 3, Models.Grid.Align.center);

                if (tipoLanceBidding == TipoLanceBidding.LancePorViagemEntregaAjudante)
                    grid.AdicionarCabecalho("Ajudante (R$)", "Ajudante", 3, Models.Grid.Align.center);

                List<Dominio.ObjetosDeValor.Embarcador.Bidding.TemplateOfertaTransportador> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.TemplateOfertaTransportador>();

                foreach (BiddingOfertaRota rota in rotas)
                {

                    Dominio.ObjetosDeValor.Embarcador.Bidding.TemplateOfertaTransportador itemRetorno = new Dominio.ObjetosDeValor.Embarcador.Bidding.TemplateOfertaTransportador()
                    {
                        ProtocoloImportacao = rota.ProtocoloImportacao.ToString(),
                        AliquotaICMS = rota.AlicotaPadraoICMS?.ToString() ?? string.Empty,
                        Quilometragem = rota.QuilometragemMedia.ToString(),
                    };

                    retorno.Add(itemRetorno);
                }

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void AdicionarColunasDinamicasTransportador(Models.Grid.Grid grid, List<Dominio.Entidades.Empresa> transportadores, List<Dominio.ObjetosDeValor.Embarcador.Bidding.ListaBiddingOfertaAceitamento> listaTotalOfertas, ref int maximoColunaTransportador, out List<int> codigosTransportadores, bool adicionarCamposExportarExcel, TipoLanceBidding tipoLanceBidding)
        {
            Models.Grid.EditableCell editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal);
            int UltimaColunaDinamica = 1;
            codigosTransportadores = new List<int>();

            maximoColunaTransportador = Math.Min(transportadores.Count, maximoColunaTransportador);
            for (int i = 0; i < maximoColunaTransportador; i++)
            {
                grid.AdicionarCabecalho(transportadores[i].NomeFantasia, "ColunaDinamicaTransportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                if (adicionarCamposExportarExcel)
                {
                    ListaBiddingOfertaAceitamento ofertaTransportadorAtual = listaTotalOfertas.Find(o => o.CodigoTransportador == transportadores[i].Codigo);
                    TipoLanceBidding tipoLanceAtual = tipoLanceBidding;

                    if (ofertaTransportadorAtual != null)
                        tipoLanceAtual = ofertaTransportadorAtual.TipoOferta;

                    switch (tipoLanceAtual)
                    {
                        case TipoLanceBidding.LanceFrotaFixaFranquia:
                            grid.AdicionarCabecalho("Valor Fixo", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Valor Franquia", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Km", "ComponenteOfertado3Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LancePorEquipamento:
                            grid.AdicionarCabecalho("Valor Fixo Equipamento", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LanceFrotaFixaKmRodado:
                            grid.AdicionarCabecalho("Valor Fixo Mensal", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Valor Km Rodado", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Km", "ComponenteOfertado3Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LancePorcentagemNota:
                            grid.AdicionarCabecalho("Porcentagem", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LanceViagemAdicional:
                            grid.AdicionarCabecalho("Valor Viagem", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Valor Entrega", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LancePorPeso:
                            grid.AdicionarCabecalho("ICMS", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Frete TON", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Pedágio/Eixo", "ComponenteOfertado3Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LancePorCapacidade:
                            grid.AdicionarCabecalho("ICMS", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Frete TON", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Pedágio/Eixo", "ComponenteOfertado3Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LancePorFreteViagem:
                            grid.AdicionarCabecalho("ICMS", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Frete/Viagem", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Pedágio/Eixo", "ComponenteOfertado3Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                        case TipoLanceBidding.LancePorViagemEntregaAjudante:
                            grid.AdicionarCabecalho("ICMS", "ComponenteOfertado2Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Frete/Viagem", "ComponenteOfertado1Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Pedágio/Eixo", "ComponenteOfertado3Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Adicional Entrega", "ComponenteOfertado4Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            grid.AdicionarCabecalho("Ajudante", "ComponenteOfertado5Transportador" + UltimaColunaDinamica.ToString(), 5, Models.Grid.Align.center, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, transportadores[i].Codigo).Editable(editableValor);
                            break;
                    }
                }

                codigosTransportadores.Add(transportadores[i].Codigo);
                UltimaColunaDinamica++;
            }
        }

        private void PreencherColunasDinamicasTransportador(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento> retorno, List<Dominio.ObjetosDeValor.Embarcador.Bidding.ListaBiddingOfertaAceitamento> listaTotalOfertas, List<(int codigoTransportador, decimal custoEstimadoTotal)> totalPorTransportador, List<int> codigosTransportadores, int maximoColunaTransportador, int quantidadeRegistros, TipoLanceBidding tipoLanceBidding)
        {
            if (maximoColunaTransportador > 0 && codigosTransportadores.Count > 0)
            {
                for (int indiceTransportadorRota = 0; indiceTransportadorRota < quantidadeRegistros; indiceTransportadorRota++)
                {
                    int numeroProximoComponente = 0;

                    for (int indiceTransportador = 0; indiceTransportador < maximoColunaTransportador; indiceTransportador++)
                    {
                        System.Reflection.PropertyInfo propertyInfoValor = retorno[indiceTransportadorRota].GetType().GetProperty($"ColunaDinamicaTransportador{++numeroProximoComponente}");
                        System.Reflection.PropertyInfo propertyInfoComponente1 = retorno[indiceTransportadorRota].GetType().GetProperty($"ComponenteOfertado1Transportador{numeroProximoComponente}");
                        System.Reflection.PropertyInfo propertyInfoComponente2 = retorno[indiceTransportadorRota].GetType().GetProperty($"ComponenteOfertado2Transportador{numeroProximoComponente}");
                        System.Reflection.PropertyInfo propertyInfoComponente3 = retorno[indiceTransportadorRota].GetType().GetProperty($"ComponenteOfertado3Transportador{numeroProximoComponente}");
                        System.Reflection.PropertyInfo propertyInfoComponente4 = retorno[indiceTransportadorRota].GetType().GetProperty($"ComponenteOfertado4Transportador{numeroProximoComponente}");
                        System.Reflection.PropertyInfo propertyInfoComponente5 = retorno[indiceTransportadorRota].GetType().GetProperty($"ComponenteOfertado5Transportador{numeroProximoComponente}");
                        Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento transportadorRota = retorno[indiceTransportadorRota];
                        Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOfertaAceitamento ultimaLinha = retorno.LastOrDefault();

                        int codigoTransportador = codigosTransportadores[indiceTransportador];
                        propertyInfoValor.SetValue(transportadorRota, "0,00", null);

                        ListaBiddingOfertaAceitamento ofertaTransportadorAtual = listaTotalOfertas.Find(o => o.CodigoTransportador == codigoTransportador && o.CodigoRota == transportadorRota.Codigo);
                        if (ofertaTransportadorAtual == null)
                        {
                            TipoLanceBidding[] tiposLancePossuemIcms =
                            [
                                TipoLanceBidding.LancePorPeso,
                                TipoLanceBidding.LancePorCapacidade,
                                TipoLanceBidding.LancePorFreteViagem,
                                TipoLanceBidding.LancePorViagemEntregaAjudante,
                            ];

                            if (tiposLancePossuemIcms.Contains(tipoLanceBidding))
                            {
                                propertyInfoComponente2.SetValue(transportadorRota, transportadorRota.AlicotaPadraoICMS?.ToString("n2"), null);
                            }

                            continue;
                        }

                        decimal custoEstimado = ofertaTransportadorAtual.CustoEstimado;
                        if (ofertaTransportadorAtual.NaoOfertar)
                        {
                            propertyInfoValor.SetValue(transportadorRota, "Não Ofertado", null);
                            continue;
                        }
                        else
                            propertyInfoValor.SetValue(transportadorRota, custoEstimado.ToString("n2"), null);
                        propertyInfoValor.SetValue(ultimaLinha, totalPorTransportador.Find(o => o.codigoTransportador == codigoTransportador).custoEstimadoTotal.ToString("n2"), null);

                        switch (ofertaTransportadorAtual.TipoOferta)
                        {
                            case TipoLanceBidding.LanceFrotaFixaFranquia:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.ValorFixo.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ValorFranquia.ToString("n2"), null);
                                propertyInfoComponente3.SetValue(transportadorRota, ofertaTransportadorAtual.Quilometragem.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LancePorEquipamento:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.ValorFixoEquipamento.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LanceFrotaFixaKmRodado:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.ValorFixoMensal.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ValorKmRodado.ToString("n2"), null);
                                propertyInfoComponente3.SetValue(transportadorRota, ofertaTransportadorAtual.Quilometragem.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LancePorcentagemNota:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.Porcentagem.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LanceViagemAdicional:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.ValorViagem.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ValorEntrega.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LancePorPeso:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.FreteTonelada.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ICMSPorcentagem.ToString("n2"), null);
                                propertyInfoComponente3.SetValue(transportadorRota, ofertaTransportadorAtual.PedagioParaEixo.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LancePorCapacidade:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.FreteTonelada.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ICMSPorcentagem.ToString("n2"), null);
                                propertyInfoComponente3.SetValue(transportadorRota, ofertaTransportadorAtual.PedagioParaEixo.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LancePorFreteViagem:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.FreteTonelada.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ICMSPorcentagem.ToString("n2"), null);
                                propertyInfoComponente3.SetValue(transportadorRota, ofertaTransportadorAtual.PedagioParaEixo.ToString("n2"), null);
                                break;
                            case TipoLanceBidding.LancePorViagemEntregaAjudante:
                                propertyInfoComponente1.SetValue(transportadorRota, ofertaTransportadorAtual.FreteTonelada.ToString("n2"), null);
                                propertyInfoComponente2.SetValue(transportadorRota, ofertaTransportadorAtual.ICMSPorcentagem.ToString("n2"), null);
                                propertyInfoComponente3.SetValue(transportadorRota, ofertaTransportadorAtual.PedagioParaEixo.ToString("n2"), null);
                                propertyInfoComponente4.SetValue(transportadorRota, ofertaTransportadorAtual.AdicionalPorEntrega.ToString("n2"), null);
                                propertyInfoComponente5.SetValue(transportadorRota, ofertaTransportadorAtual.Ajudante.ToString("n2"), null);
                                break;
                        }
                    }
                }
            }
        }

        private List<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida> BuscarDuvidas(Repositorio.UnitOfWork unitOfWork, BiddingConvite biddingConvite)
        {
            Repositorio.Embarcador.Bidding.BiddingDuvida repBiddingDuvida = new Repositorio.Embarcador.Bidding.BiddingDuvida(unitOfWork);

            List<BiddingDuvida> listaDuvidas = repBiddingDuvida.BuscarPorConvidadoConvite(this.Usuario.Empresa, biddingConvite);

            return listaDuvidas;
        }

        private void ModificarStatusConvidado(BiddingConviteConvidado convidado, StatusBiddingConviteConvidado status, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);

            convidado.Status = status;
            repConvidado.Atualizar(convidado);
        }
        #endregion
    }
}
