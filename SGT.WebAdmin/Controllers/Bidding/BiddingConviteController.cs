using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGTAdmin.Controllers;
using System.Collections.Concurrent;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingConvite", "Bidding/BiddingAceitacao")]
    public class BiddingConviteController : BaseController
    {
        #region Construtores

        public BiddingConviteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Bidding.BiddingConvite grupoBiddingConvite = new Dominio.Entidades.Embarcador.Bidding.BiddingConvite();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = await repositorioConfiguracaoBidding.BuscarConfiguracaoPadraoAsync();
                bool transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding = configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding;

                Servicos.Embarcador.Bidding.BiddingConviteAprovacao servicoBiddingConvite = new Servicos.Embarcador.Bidding.BiddingConviteAprovacao(unitOfWork);

                DateTime prazoAceite = Request.GetDateTimeParam("PrazoAceiteConvite");
                DateTime dataLimite = Request.GetDateTimeParam("DataLimite");

                if (prazoAceite > dataLimite)
                    return new JsonpResult(false, true, "Prazo para aceite do convite não pode ser maior que a Data Limite.");

                await PreencherEntidadeAsync(grupoBiddingConvite, transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding, false, unitOfWork, cancellationToken);

                await repBiddingConvite.InserirAsync(grupoBiddingConvite, Auditado);

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklist biddingChecklist = await SalvarChecklistAsync(grupoBiddingConvite, unitOfWork, cancellationToken, atualizar: false);
                Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta = await SalvarOfertasAsync(grupoBiddingConvite, unitOfWork, cancellationToken, atualizar: false);

                await SalvarConvidadosAsync(grupoBiddingConvite, transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding, estaAtualizandoBiddingConvite: false, unitOfWork, cancellationToken);

                Dictionary<string, int> questionarioCodigos = await SalvarQuestionariosAsync(biddingChecklist, unitOfWork, grupoBiddingConvite, cancellationToken, atualizar: false);
                Servicos.Log.TratarErro($"Iniciar Salvar Rotas {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", "SalvarRotasAsync");
                await SalvarRotasAsync(biddingOferta, unitOfWork, cancellationToken);
                Servicos.Log.TratarErro($"Finalizou Salvar Rotas {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", "SalvarRotasAsync");

                Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite = await repBiddingConvite.BuscarPorCodigoAsync(grupoBiddingConvite.Codigo, false);

                if (biddingConvite != null && grupoBiddingConvite.TipoBidding != null)
                    servicoBiddingConvite.EtapaAprovacao(biddingConvite, TipoServicoMultisoftware);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, grupoBiddingConvite, null, $"Adicionou bidding convite.", unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(new
                {
                    grupoBiddingConvite.Codigo,
                    questionarioCodigos = (
                        from questionarioCodigo in questionarioCodigos
                        select new
                        {
                            codigo = questionarioCodigo.Key,
                            novoCodigo = questionarioCodigo.Value
                        }
                    ).ToList()
                });

            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisar()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingConvite grupoBiddingConvite = repBiddingConvite.BuscarPorCodigo(codigo, false);

                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> listaConvidados = repConvidado.BuscarConvidados(grupoBiddingConvite);

                Repositorio.Embarcador.Bidding.BiddingChecklist repChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingChecklist listaChecklist = repChecklist.BuscarChecklist(grupoBiddingConvite);

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklist entidadeChecklist = repChecklist.BuscarPorCodigo(listaChecklist.Codigo, false);

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario repQuestionarios = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario> listaQuestionarios = repQuestionarios.BuscarQuestionarios(entidadeChecklist);

                Repositorio.Embarcador.Bidding.BiddingOferta repOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingOferta entidadeOferta = repOferta.BuscarOferta(grupoBiddingConvite);

                Repositorio.Embarcador.Bidding.BiddingOfertaRota repOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> listaRotas = repOfertaRota.BuscarRotas(entidadeOferta.Codigo);

                Repositorio.Embarcador.Bidding.Baseline repositorioBaseline = new Repositorio.Embarcador.Bidding.Baseline(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.Baseline> baselines = repositorioBaseline.BuscarPorBiddingConvite(grupoBiddingConvite.Codigo);

                Repositorio.Embarcador.Bidding.TipoBiddingAnexo repositorioTipoBiddingAnexo = new Repositorio.Embarcador.Bidding.TipoBiddingAnexo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo> tipoBiddingAnexo = repositorioTipoBiddingAnexo.BuscarPorTipoBidding(grupoBiddingConvite.TipoBidding.Codigo);

                Repositorio.Embarcador.Bidding.BiddingDuvida repDuvida = new Repositorio.Embarcador.Bidding.BiddingDuvida(unitOfWork);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida> listaDuvidas;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    listaDuvidas = repDuvida.BuscarPorConvidadoConvite(this.Usuario.Empresa, grupoBiddingConvite);
                else
                    listaDuvidas = repDuvida.BuscarPorConvite(grupoBiddingConvite.Codigo);

                if (grupoBiddingConvite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Codigo = duplicar ? 0 : grupoBiddingConvite.Codigo,
                    DadosBiddingConvite = new
                    {
                        Iniciado = grupoBiddingConvite.DataInicio <= DateTime.Now,
                        grupoBiddingConvite.Codigo,
                        Situacao = grupoBiddingConvite.Situacao,
                        SituacaoAprovacao = grupoBiddingConvite.Status,
                        Descricao = grupoBiddingConvite.Descricao,
                        DataInicio = grupoBiddingConvite.DataInicio.ToString(),
                        DataLimite = grupoBiddingConvite.DataLimite.ToString(),
                        DescritivoConvite = grupoBiddingConvite.DescritivoConvite,
                        DescritivoTransportador = grupoBiddingConvite.DescritivoTransportador,
                        ExigirPreenchimentoChecklistConvitePeloTransportador = grupoBiddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador,
                        PrazoAceiteConvite = grupoBiddingConvite.DataPrazoAceiteConvite.HasValue ? grupoBiddingConvite.DataPrazoAceiteConvite.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        TempoRestante = grupoBiddingConvite.DataPrazoAceiteConvite.HasValue ? grupoBiddingConvite.DataPrazoAceiteConvite.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        DataInicioVigencia = grupoBiddingConvite.DataInicioVigencia.HasValue ? grupoBiddingConvite.DataInicioVigencia.Value.ToString() : null,
                        DataFimVigencia = grupoBiddingConvite.DataFimVigencia.HasValue ? grupoBiddingConvite.DataFimVigencia.Value.ToString() : null,
                        Etapa = grupoBiddingConvite.Status.ObterDescricao(),
                        TipoFrete = grupoBiddingConvite.TipoFrete,
                        TipoBidding = new
                        {
                            Codigo = grupoBiddingConvite.TipoBidding?.Codigo ?? 0,
                            Descricao = grupoBiddingConvite.TipoBidding?.Descricao ?? string.Empty
                        },
                    },
                    Duvidas = (
                        from duvida in listaDuvidas
                        select new
                        {
                            Codigo = duvida.Codigo,
                            Pergunta = duvida.Pergunta,
                            Resposta = duvida.Resposta,
                            Data = duvida.Data.ToString("dd/MM/yyyy")
                        }
                    ).ToList(),
                    Convidados = (
                        from convidado in listaConvidados
                        select new
                        {
                            convidado.Convidado.Codigo,
                            convidado.Convidado.Descricao,
                            convidado.Status
                        }
                    ).ToList(),
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
                        from o in tipoBiddingAnexo
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    Checklist = new
                    {
                        Codigo = entidadeChecklist.Codigo,
                        Prazo = entidadeChecklist.DataPrazo?.ToString("dd/MM/yyyy HH:mm"),
                        TipoPreenchimentoChecklist = entidadeChecklist.TipoPreenchimentoChecklist
                    },
                    Questionarios = (
                        from questionario in listaQuestionarios
                        select new
                        {
                            questionario.Codigo,
                            questionario.Descricao,
                            Requisito = questionario.Requisito.ObterDescricao(),
                            ChecklistAnexo = from ChecklistAnexo in questionario.Anexos
                                             where ChecklistAnexo.EntidadeAnexo.Checklist == questionario.Checklist
                                             select new
                                             {
                                                 ChecklistAnexo.Codigo,
                                                 ChecklistAnexo.Descricao,
                                                 ChecklistAnexo.NomeArquivo
                                             }
                        }
                    ).ToList(),
                    Oferta = new
                    {
                        Codigo = entidadeOferta.Codigo,
                        entidadeOferta.TipoLance,
                        PrazoOferta = entidadeOferta.DataPrazoOferta.HasValue ? entidadeOferta.DataPrazoOferta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        PermitirInformarVeiculosVerdes = entidadeOferta.PermitirInformarVeiculosVerdes,
                    },
                    Rotas = (
                        from rota in listaRotas
                        select new
                        {
                            Codigo = rota.Codigo,
                            Descricao = rota.Descricao,
                            Rota = rota.Descricao,
                            FlagOrigem = rota.FlagOrigem,
                            FlagDestino = rota.FlagDestino,
                            NumeroEntrega = rota.NumeroEntrega,
                            ClienteDestino = (from cliente in rota.ClientesDestino
                                              select new
                                              {
                                                  cliente.Codigo,
                                                  cliente.Descricao
                                              }).ToList(),
                            ClienteOrigem = (from cliente in rota.ClientesOrigem
                                             select new
                                             {
                                                 cliente.Codigo,
                                                 cliente.Descricao
                                             }).ToList(),
                            CidadeDestino = (from destino in rota.Destinos
                                             select new
                                             {
                                                 destino.Codigo,
                                                 destino.Descricao
                                             }).ToList(),
                            CidadeOrigem = (from origem in rota.Origens
                                            select new
                                            {
                                                origem.Codigo,
                                                origem.Descricao
                                            }).ToList(),
                            RegiaoDestino = (from regiao in rota.RegioesDestino
                                             select new
                                             {
                                                 regiao.Codigo,
                                                 regiao.Descricao
                                             }).ToList(),
                            RegiaoOrigem = (from regiao in rota.RegioesOrigem
                                            select new
                                            {
                                                regiao.Codigo,
                                                regiao.Descricao
                                            }).ToList(),
                            PaisOrigem = (from pais in rota.PaisesOrigem
                                          select new
                                          {
                                              pais.Codigo,
                                              pais.Descricao
                                          }).ToList(),
                            PaisDestino = (from pais in rota.PaisesDestino
                                           select new
                                           {
                                               pais.Codigo,
                                               pais.Descricao
                                           }).ToList(),
                            EstadoDestino = (from estado in rota.EstadosDestino
                                             select new
                                             {
                                                 Codigo = estado.Sigla,
                                                 Descricao = estado.Nome
                                             }).ToList(),
                            EstadoOrigem = (from estado in rota.EstadosOrigem
                                            select new
                                            {
                                                Codigo = estado.Sigla,
                                                Descricao = estado.Nome
                                            }).ToList(),
                            rota.QuilometragemMedia,
                            rota.Peso,
                            rota.AdicionalAPartirDaEntregaNumero,
                            rota.Frequencia,
                            rota.Volume,
                            rota.ValorCargaMes,
                            rota.FrequenciaMensalComAjudante,
                            rota.QuantidadeAjudantePorVeiculo,
                            rota.MediaEntregasFracionada,
                            rota.MaximaEntregasFacionada,
                            rota.QuantidadeViagensPorAno,
                            rota.VolumeTonAno,
                            rota.VolumeTonViagem,
                            rota.ValorMedioNFe,
                            TempoColeta = rota.TempoColeta.HasValue ? rota.TempoColeta.Value.ToString(@"hh\:mm") : "",
                            TempoDescarga = rota.TempoDescarga.HasValue ? rota.TempoDescarga.Value.ToString(@"hh\:mm") : "",
                            GrupoModeloVeicular = new { Codigo = rota.GrupoModeloVeicular?.Codigo ?? 0, Descricao = rota.GrupoModeloVeicular?.Descricao ?? string.Empty },
                            Tomador = new { Codigo = rota.Tomador?.Codigo ?? 0, Descricao = rota.Tomador?.Descricao ?? string.Empty },
                            ModeloCarroceria = new { Codigo = rota.ModeloCarroceria?.Codigo ?? 0, Descricao = rota.ModeloCarroceria?.Descricao ?? string.Empty },
                            Inconterm = rota.Inconterm.HasValue ? rota.Inconterm.Value : Inconterm.CIF,
                            Compressor = rota.Compressor.HasValue ? rota.Compressor.Value : SimNaoNA.NaoAplicavel,
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
                            FiliaisParticipante = (from filial in rota.FiliaisParticipante
                                                   select new
                                                   {
                                                       filial.Codigo,
                                                       filial.Descricao
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
                            RotaOrigem = (from rotaorigem in rota.RotasOrigem
                                          select new
                                          {
                                              rotaorigem.Codigo,
                                              rotaorigem.Descricao
                                          }).ToList(),
                            RotaDestino = (from rotadestino in rota.RotasDestino
                                           select new
                                           {
                                               rotadestino.Codigo,
                                               rotadestino.Descricao
                                           }).ToList(),
                            Baseline = (from obj in baselines
                                        where obj.BiddingOfertaRota.Codigo == rota.Codigo
                                        select new
                                        {
                                            Codigo = obj.Codigo,
                                            CodigoTipoBaseline = obj.TipoBaseline?.Codigo ?? 0,
                                            TipoBaseline = obj.TipoBaseline?.Descricao ?? string.Empty,
                                            Valor = obj.Valor.ToString("n2")
                                        }).ToList()
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork, cancellationToken);

                Servicos.Embarcador.Bidding.BiddingConviteAprovacao servicoBiddingConvite = new Servicos.Embarcador.Bidding.BiddingConviteAprovacao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite = await repBiddingConvite.BuscarPorCodigoAsync(codigo, false);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = await repositorioConfiguracaoBidding.BuscarConfiguracaoPadraoAsync();
                bool transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding = configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding;

                if (biddingConvite == null)
                    return new JsonpResult(false, true, "Bidding Convite não foi encontrado.");

                biddingConvite.Initialize();

                DateTime prazoAceite = Request.GetDateTimeParam("PrazoAceiteConvite");
                DateTime dataLimite = Request.GetDateTimeParam("DataLimite");

                if (prazoAceite > dataLimite)
                    return new JsonpResult(false, true, "Prazo para aceite do convite não pode ser maior que a Data Limite.");

                if (configuracaoBidding.PermiteRemoverObrigatoriedadeDatas && (dataLimite <= DateTime.Now))
                    return new JsonpResult(false, true, "Não é possível alterar quando a data atual for maior que a Data Limite.");

                await PreencherEntidadeAsync(biddingConvite, transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding, true, unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklist biddingChecklist = await SalvarChecklistAsync(biddingConvite, unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta = await SalvarOfertasAsync(biddingConvite, unitOfWork, cancellationToken);

                await AtualizarRotasAsync(biddingOferta, unitOfWork, cancellationToken);
                Dictionary<string, int> questionarioCodigos = await SalvarQuestionariosAsync(biddingChecklist, unitOfWork, biddingConvite, cancellationToken);
                await SalvarConvidadosAsync(biddingConvite, transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding, true, unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado biddingConviteConvidado = await repositorioConviteConvidado.BuscarPorConviteAsync(biddingConvite.Codigo);

                if (biddingConvite.TipoBidding != null && (!transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding && biddingConviteConvidado == null))
                    servicoBiddingConvite.EtapaAprovacao(biddingConvite, TipoServicoMultisoftware);

                await repBiddingConvite.AtualizarAsync(biddingConvite, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);


                return new JsonpResult(new
                {
                    questionarioCodigos = (
                        from questionarioCodigo in questionarioCodigos
                        select new
                        {
                            codigo = questionarioCodigo.Key,
                            novoCodigo = questionarioCodigo.Value
                        }
                    ).ToList()
                });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirQuestionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario repBiddingQuestionario = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario biddingQuestionario = repBiddingQuestionario.BuscarPorCodigo(codigo, false);
                repBiddingQuestionario.Deletar(biddingQuestionario);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CalculoKMMediaRotaCidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDestino = Request.GetIntParam("CodigoDestino");
                int codigoOrigem = Request.GetIntParam("CodigoOrigem");

                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                Dominio.Entidades.Localidade destino = repositorioLocalidade.BuscarPorCodigo(codigoDestino);
                Dominio.Entidades.Localidade origem = repositorioLocalidade.BuscarPorCodigo(codigoOrigem);

                double origemLatitude = (double)(origem?.Latitude ?? 0);
                double origemLongitude = (double)(origem?.Longitude ?? 0);
                double destinoLatitude = (double)(destino?.Latitude ?? 0);
                double destinoLongitude = (double)(destino?.Longitude ?? 0);

                if (origemLatitude == 0 || origemLongitude == 0)
                    return new JsonpResult(false, true, "Latitude ou Longitude da Origem não está preenchido, portanto, não é possível realizar o cálculo da Quilometragem Média da Rota.");

                if (destinoLatitude == 0 || destinoLongitude == 0)
                    return new JsonpResult(false, true, "Latitude ou Longitude do Destino não está preenchido, portanto, não é possível realizar o cálculo da Quilometragem Média da Rota.");

                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);

                rota.Clear();

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

                wayPoints.Add(rota.ObterWaypoint(origemLatitude, origemLongitude, origem.DescricaoCidadeEstado, 1, TipoPontoPassagem.Coleta));
                wayPoints.Add(rota.ObterWaypoint(destinoLatitude, destinoLongitude, destino.DescricaoCidadeEstado, 2, TipoPontoPassagem.Entrega));

                rota.Add(wayPoints);

                var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                {
                    AteOrigem = false,
                    Ordenar = false,
                    PontosNaRota = false
                };

                respostaRoteirizacao = rota.Roteirizar(opcoes);

                var retorno = new
                {
                    QuilometragemMedia = respostaRoteirizacao.Distancia.ToString("n2")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao calcular a Quilometragem Média da Rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CalculoKMMediaRotaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double codigoDestino = Request.GetDoubleParam("CodigoDestino");
                double codigoOrigem = Request.GetDoubleParam("CodigoOrigem");

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;
                Dominio.Entidades.Cliente destino = repositorioCliente.BuscarPorCPFCNPJ(codigoDestino);
                Dominio.Entidades.Cliente origem = repositorioCliente.BuscarPorCPFCNPJ(codigoOrigem);

                double origemLatitude, origemLongitude, destinoLatitude, destinoLongitude;
                double.TryParse(origem?.Latitude ?? string.Empty, out origemLatitude);
                double.TryParse(origem?.Longitude ?? string.Empty, out origemLongitude);
                double.TryParse(destino?.Latitude ?? string.Empty, out destinoLatitude);
                double.TryParse(destino?.Longitude ?? string.Empty, out destinoLongitude);

                if (origemLatitude == 0 || origemLongitude == 0)
                    return new JsonpResult(false, true, "Latitude ou Longitude da Origem não está preenchido, portanto, não é possível realizar o cálculo da Quilometragem Média da Rota.");

                if (destinoLatitude == 0 || destinoLongitude == 0)
                    return new JsonpResult(false, true, "Latitude ou Longitude do Destino não está preenchido, portanto, não é possível realizar o cálculo da Quilometragem Média da Rota.");

                Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);

                rota.Clear();

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

                wayPoints.Add(rota.ObterWaypoint(origemLatitude, origemLongitude, origem.EnderecoCompletoCidadeeEstado, 1, TipoPontoPassagem.Coleta));
                wayPoints.Add(rota.ObterWaypoint(destinoLatitude, destinoLongitude, destino.EnderecoCompletoCidadeeEstado, 2, TipoPontoPassagem.Entrega));

                rota.Add(wayPoints);

                var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                {
                    AteOrigem = false,
                    Ordenar = false,
                    PontosNaRota = false
                };

                respostaRoteirizacao = rota.Roteirizar(opcoes);

                var retorno = new
                {
                    QuilometragemMedia = respostaRoteirizacao.Distancia.ToString("n2")
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao calcular a Quilometragem Média da Rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CalculoKMMediaRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDestino = Request.GetIntParam("CodigoDestino");
                int codigoOrigem = Request.GetIntParam("CodigoOrigem");

                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                if (codigoOrigem != codigoDestino)
                    return new JsonpResult(false, true, "A Rota de Origem não corresponde com destino, portanto, não é possível realizar o cálculo da Quilometragem Média da Rota.");

                Dominio.Entidades.RotaFrete origem = repositorioRotaFrete.BuscarPorCodigo(codigoOrigem);

                string quilometros = origem != null ? origem.Quilometros.ToString("n2") : string.Empty;

                var retorno = new
                {
                    QuilometragemMedia = quilometros
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao calcular a Quilometragem Média da Rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Globais - Importações

        public async Task<IActionResult> ConfiguracaoImportacaoRotas()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Bidding.Bidding svcBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = svcBidding.ConfiguracaoImportacaoRotas();

                return new JsonpResult(configuracoes.ToList());
            }
        }

        public async Task<IActionResult> ImportarRotas(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Servicos.Embarcador.Bidding.Bidding svcBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork, cancellationToken);

                string dados = Request.GetStringParam("Dados");

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = await svcBidding.ImportarRotasConvite(Request, dados, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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

        public async Task<IActionResult> VerificarExistenciaRegraBiddingConvite()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding repRegraAutorizacaoBidding = new Repositorio.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding(unitOfWork);

                var retorno = new
                {
                    Regras = repRegraAutorizacaoBidding.BuscarRegistroAtivo(),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar regras de Bidding Convite.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais - Importações

        #endregion Métodos Globais

        #region Métodos Privados

        private async Task PreencherEntidadeAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite grupoBiddingConvite, bool transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding, bool estaAtualizandoBiddingConvite, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Bidding.TipoBidding repTipoDeBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork, cancellationToken);
            DateTime? dataInicialVigencia = Request.GetNullableDateTimeParam("DataInicioVigencia");
            DateTime? dataFimVigencia = Request.GetNullableDateTimeParam("DataFimVigencia");

            grupoBiddingConvite.Situacao = Request.GetBoolParam("Situacao");
            grupoBiddingConvite.Descricao = Request.GetStringParam("Descricao");
            grupoBiddingConvite.DataInicio = Request.GetDateTimeParam("DataInicio");
            grupoBiddingConvite.DataLimite = Request.GetDateTimeParam("DataLimite");
            grupoBiddingConvite.DescritivoConvite = Request.GetStringParam("DescritivoConvite");
            grupoBiddingConvite.DescritivoTransportador = Request.GetStringParam("DescritivoTransportador");
            grupoBiddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador = Request.GetBoolParam("ExigirPreenchimentoChecklistConvitePeloTransportador");
            grupoBiddingConvite.DataPrazoAceiteConvite = Request.GetNullableDateTimeParam("PrazoAceiteConvite");
            grupoBiddingConvite.Status = (transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding && estaAtualizandoBiddingConvite && grupoBiddingConvite.Convidados.Any()) ? grupoBiddingConvite.Convidados.FirstOrDefault().StatusBidding : StatusBiddingConvite.Aguardando;
            grupoBiddingConvite.DataInicioVigencia = dataInicialVigencia ?? null;
            grupoBiddingConvite.DataFimVigencia = dataFimVigencia ?? null;
            grupoBiddingConvite.TipoFrete = Request.GetNullableEnumParam<TipoFrete>("TipoFrete");
            int codigoTipoBidding = Request.GetIntParam("TipoBidding");
            grupoBiddingConvite.TipoBidding = codigoTipoBidding > 0 ? await repTipoDeBidding.BuscarPorCodigoAsync(codigoTipoBidding, false) : null;
            grupoBiddingConvite.Solicitante = Usuario;
        }

        private async Task SalvarConvidadosAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite, bool transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding, bool estaAtualizandoBiddingConvite, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            dynamic convite = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Convite"));
            Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork, cancellationToken);
            await repConvidado.DeletarPorEntidadeAsync(entidadeBiddingConvite);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado biddingConvidado;

            List<Dominio.Entidades.Empresa> convidados = await repEmpresa.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)convite).Select(c => (int)(c.Codigo.ToInt())).ToList()
            );

            List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> biddingConvidados = new List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>();

            foreach (var convidado in convite)
            {
                biddingConvidado = new Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado();
                biddingConvidado.BiddingConvite = entidadeBiddingConvite;
                biddingConvidado.Convidado = convidados.FirstOrDefault(c => c.Codigo == (int)(convidado.Codigo.ToInt()));
                biddingConvidado.Status = (transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding && estaAtualizandoBiddingConvite && convidado.Status != null) ? convidado.Status : StatusBiddingConviteConvidado.Aguardando;
                biddingConvidado.StatusBidding = (transportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding && estaAtualizandoBiddingConvite && entidadeBiddingConvite.Convidados.Any()) ? entidadeBiddingConvite.Convidados.FirstOrDefault().StatusBidding : StatusBiddingConvite.Aguardando;

                biddingConvidados.Add(biddingConvidado);
            }

            await repConvidado.InserirAsync(biddingConvidados, "T_BIDDING_CONVITE_CONVIDADO");
        }

        private async Task<Dominio.Entidades.Embarcador.Bidding.BiddingChecklist> SalvarChecklistAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, UnitOfWork unitOfWork, CancellationToken cancellationToken, bool atualizar = true)
        {
            dynamic checklistPrazo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PrazoChecklist"));

            Repositorio.Embarcador.Bidding.BiddingChecklist repBiddingChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Bidding.BiddingChecklist biddingChecklist;

            if (biddingConvite.Codigo > 0 && atualizar)
            {
                biddingChecklist = await repBiddingChecklist.BuscarChecklistAsync(biddingConvite);
                biddingChecklist.Initialize();
            }
            else
            {
                biddingChecklist = new Dominio.Entidades.Embarcador.Bidding.BiddingChecklist();
            }

            string prazoChecklist = (string)checklistPrazo.PrazoChecklist;

            biddingChecklist.DataPrazo = !string.IsNullOrEmpty(prazoChecklist) ? prazoChecklist.ToDateTime() : null;
            biddingChecklist.BiddingConvite = biddingConvite;
            biddingChecklist.DataLimite = biddingChecklist.DataPrazo;
            biddingChecklist.TipoPreenchimentoChecklist = ((TipoPreenchimentoChecklist)checklistPrazo.PreenchimentoChecklist);

            if (biddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador)
            {
                biddingChecklist.DataPrazo = biddingConvite.DataPrazoAceiteConvite.HasValue ? biddingConvite.DataPrazoAceiteConvite.Value : null;
                biddingChecklist.DataLimite = biddingConvite.DataPrazoAceiteConvite.HasValue ? biddingConvite.DataPrazoAceiteConvite.Value : null;
            }

            if (biddingConvite.Codigo > 0 && atualizar)
            {
                await repBiddingChecklist.AtualizarAsync(biddingChecklist);
                biddingConvite.SetExternalChanges(biddingChecklist.GetCurrentChanges());
            }
            else
                await repBiddingChecklist.InserirAsync(biddingChecklist);

            return biddingChecklist;
        }

        private async Task<Dictionary<string, int>> SalvarQuestionariosAsync(Dominio.Entidades.Embarcador.Bidding.BiddingChecklist entidadeBiddingChecklist, UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, CancellationToken cancellationToken, bool atualizar = true)
        {
            Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario repBiddingChecklistQuestionario = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionario(unitOfWork, cancellationToken);

            dynamic checklist = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Checklist"));

            Dictionary<string, int> questionariosAdicionados = new Dictionary<string, int>();

            foreach (var questionario in checklist)
            {
                int? codigo = ((string)questionario.Codigo).ToNullableInt();

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario biddingChecklistQuestionario = null;

                if (codigo.HasValue && atualizar)
                {
                    biddingChecklistQuestionario = await repBiddingChecklistQuestionario.BuscarPorCodigoAsync(codigo.Value, false);
                    biddingChecklistQuestionario.Initialize();
                }
                else
                    biddingChecklistQuestionario = new Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario();

                biddingChecklistQuestionario.Checklist = entidadeBiddingChecklist;
                biddingChecklistQuestionario.Descricao = ((string)questionario.Pergunta).ToString();
                string requisito = ((string)questionario.Requisito);
                requisito = requisito == "Desejável" ? "Desejavel" : "Indispensavel";
                biddingChecklistQuestionario.Requisito = requisito.ToEnum<TipoRequisitoBiddingChecklist>();

                if (biddingChecklistQuestionario.Codigo == 0)
                {
                    await repBiddingChecklistQuestionario.InserirAsync(biddingChecklistQuestionario);
                    questionariosAdicionados.Add(((string)questionario.Codigo), biddingChecklistQuestionario.Codigo);
                }
                else
                {
                    await repBiddingChecklistQuestionario.AtualizarAsync(biddingChecklistQuestionario);
                    biddingConvite.SetExternalChanges(biddingChecklistQuestionario.GetCurrentChanges());
                }
            }

            return questionariosAdicionados;
        }

        private async Task<Dominio.Entidades.Embarcador.Bidding.BiddingOferta> SalvarOfertasAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, UnitOfWork unitOfWork, CancellationToken cancellationToken, bool atualizar = true)
        {
            Repositorio.Embarcador.Bidding.BiddingOferta repBiddingOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(unitOfWork, cancellationToken);

            dynamic ofertaCampos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OfertaCampos"));

            Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta;

            if (biddingConvite.Codigo > 0 && atualizar)
            {
                biddingOferta = await repBiddingOferta.BuscarOfertaAsync(biddingConvite);
                biddingOferta.Initialize();
            }
            else
            {
                biddingOferta = new Dominio.Entidades.Embarcador.Bidding.BiddingOferta();
            }
            if (!string.IsNullOrWhiteSpace((string)ofertaCampos.PrazoOferta))
                biddingOferta.DataPrazoOferta = ((string)ofertaCampos.PrazoOferta).ToDateTime();

            biddingOferta.TipoLance = ((string)ofertaCampos.TipoLance).ToEnum<TipoLanceBidding>();
            biddingOferta.PermitirInformarVeiculosVerdes = ofertaCampos.PermitirInformarVeiculosVerdes;
            biddingOferta.BiddingConvite = biddingConvite;
            biddingOferta.DataLimite = biddingOferta.DataPrazoOferta;

            if (biddingConvite.Codigo > 0 && atualizar)
            {
                await repBiddingOferta.AtualizarAsync(biddingOferta);
                biddingConvite.SetExternalChanges(biddingOferta.GetCurrentChanges());
            }
            else
                await repBiddingOferta.InserirAsync(biddingOferta);

            return biddingOferta;
        }

        private async Task SalvarRotasAsync(Dominio.Entidades.Embarcador.Bidding.BiddingOferta entidadeBiddingOferta, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var oferta = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.Rota>>(Request.Params("Oferta"));

            Repositorio.Embarcador.Bidding.BiddingOfertaRota repOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.GrupoModeloVeicular repositorioGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Veiculos.ModeloCarroceria repositorioModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork, cancellationToken);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork, cancellationToken);
            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unitOfWork, cancellationToken);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingCEPOrigem repCEPOrigem = new Repositorio.Embarcador.Bidding.BiddingCEPOrigem(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingCEPDestino repCEPDestino = new Repositorio.Embarcador.Bidding.BiddingCEPDestino(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.Baseline repositorioBaseline = new Repositorio.Embarcador.Bidding.Baseline(unitOfWork, cancellationToken);
            Repositorio.Aliquota repAliquotaICMS = new Repositorio.Aliquota(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> listaRotas = new List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>();
            Servicos.Log.TratarErro($"Iniciar consultas {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", "SalvarRotasAsync");

            List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> listaGrupoModeloVeicular = await repositorioGrupoModeloVeicular.BuscarPorCodigosAsync(
                (oferta)
                    .Select(rota => ((string)rota.GrupoModeloVeicular.Codigo).ToInt())
                    .Distinct()
                    .ToList()
            );

            List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> listaModeloCarroceria = await repositorioModeloCarroceria.BuscarPorCodigosAsync(
                (oferta)
                    .Select(rota => ((string)rota.ModeloCarroceria.Codigo).ToInt())
                    .Distinct()
                    .ToList());

            List<Dominio.Entidades.Cliente> listaTomadores = await repositorioCliente.BuscarPorCPFCNPJsAsync(((IEnumerable<dynamic>)oferta)
                    .Select(rota => ((double)rota.Tomador.Codigo))
                    .Distinct()
                    .ToList());

            List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFiliais = await repositorioFilial.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(filiaisParticipante =>
                    ((IEnumerable<dynamic>)filiaisParticipante.FiliaisParticipante ?? Enumerable.Empty<dynamic>())
                        .Select(filial => filial.Codigo != null ? ((string)filial.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> listaTiposCarga = await repTipoDeCarga.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(tiposCarga =>
                    ((IEnumerable<dynamic>)tiposCarga.TipoCarga ?? Enumerable.Empty<dynamic>())
                        .Select(tipoCarga => tipoCarga.Codigo != null ? ((string)tipoCarga.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> listaModelosVeicular = await repModeloVeicular.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(modelosVeicular =>
                    ((IEnumerable<dynamic>)modelosVeicular.ModeloVeicular ?? Enumerable.Empty<dynamic>())
                        .Select(modeloVeicular => modeloVeicular.Codigo != null ? ((string)modeloVeicular.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Localidade> cidadesOrigem = await repLocalidade.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(cidadesOrigem =>
                    ((IEnumerable<dynamic>)cidadesOrigem.CidadeOrigem ?? Enumerable.Empty<dynamic>())
                        .Select(cidadeOrigem => cidadeOrigem.Codigo != null ? ((string)cidadeOrigem.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Localidade> cidadesDestino = await repLocalidade.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(cidadesDestino =>
                    ((IEnumerable<dynamic>)cidadesDestino.CidadeDestino ?? Enumerable.Empty<dynamic>())
                        .Select(cidadeDestino => cidadeDestino.Codigo != null ? ((string)cidadeDestino.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Estado> estadosOrigem = await repEstado.BuscarPorSiglasAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(estadosOrigem =>
                    ((IEnumerable<dynamic>)estadosOrigem.EstadoOrigem ?? Enumerable.Empty<dynamic>())
                        .Select(estadoOrigem => estadoOrigem.Codigo != null ? ((string)estadoOrigem.Codigo).ToString() : "")
                )
                .Where(codigo => !string.IsNullOrEmpty(codigo))
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Estado> estadosDestino = await repEstado.BuscarPorSiglasAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(estadosDestino =>
                    ((IEnumerable<dynamic>)estadosDestino.EstadoDestino ?? Enumerable.Empty<dynamic>())
                        .Select(estadoDestino => estadoDestino.Codigo != null ? ((string)estadoDestino.Codigo).ToString() : "")
                )
                .Where(codigo => !string.IsNullOrEmpty(codigo))
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem = await repRegiao.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(regioesOrigem =>
                    ((IEnumerable<dynamic>)regioesOrigem.RegiaoOrigem ?? Enumerable.Empty<dynamic>())
                        .Select(regiaoOrigem => regiaoOrigem.Codigo != null ? ((string)regiaoOrigem.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = await repRegiao.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(regioesDestino =>
                    ((IEnumerable<dynamic>)regioesDestino.RegiaoDestino ?? Enumerable.Empty<dynamic>())
                        .Select(regiaoDestino => regiaoDestino.Codigo != null ? ((string)regiaoDestino.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.RotaFrete> rotasOrigem = await repRota.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(rotasOrigem =>
                    ((IEnumerable<dynamic>)rotasOrigem.RotaOrigem ?? Enumerable.Empty<dynamic>())
                        .Select(rotaOrigem => rotaOrigem.Codigo != null ? ((string)rotaOrigem.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.RotaFrete> rotasDestino = await repRota.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(rotasDestino =>
                    ((IEnumerable<dynamic>)rotasDestino.RotaDestino ?? Enumerable.Empty<dynamic>())
                        .Select(rotaDestino => rotaDestino.Codigo != null ? ((string)rotaDestino.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Pais> paisesOrigem = await repPais.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(paisesOrigem =>
                    ((IEnumerable<dynamic>)paisesOrigem.PaisOrigem ?? Enumerable.Empty<dynamic>())
                        .Select(paisOrigem => paisOrigem.Codigo != null ? ((string)paisOrigem.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Pais> paisesDestino = await repPais.BuscarPorCodigosAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(paisesDestino =>
                    ((IEnumerable<dynamic>)paisesDestino.PaisDestino ?? Enumerable.Empty<dynamic>())
                        .Select(paisDestino => paisDestino.Codigo != null ? ((string)paisDestino.Codigo).ToInt() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Cliente> clientesOrigem = await repositorioCliente.BuscarPorCPFCNPJsAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(clientesOrigem =>
                    ((IEnumerable<dynamic>)clientesOrigem.ClienteOrigem ?? Enumerable.Empty<dynamic>())
                        .Select(clienteOrigem => clienteOrigem.Codigo != null ? ((string)clienteOrigem.Codigo).ToDouble() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            List<Dominio.Entidades.Cliente> clientesDestino = await repositorioCliente.BuscarPorCPFCNPJsAsync(
                ((IEnumerable<dynamic>)oferta)
                .SelectMany(clientesDestino =>
                    ((IEnumerable<dynamic>)clientesDestino.ClienteDestino ?? Enumerable.Empty<dynamic>())
                        .Select(clienteDestino => clienteDestino.Codigo != null ? ((string)clienteDestino.Codigo).ToDouble() : 0)
                )
                .Where(codigo => codigo > 0)
                .Distinct()
                .ToList()
            );

            var baselineCodigos = ((IEnumerable<dynamic>)oferta)
               .SelectMany(baselines => baselines.Baseline as IEnumerable<dynamic> ?? Enumerable.Empty<dynamic>())
               .Where(baseline => baseline.Codigo != null)
               .Select(baseline => int.TryParse((string)baseline.Codigo, out var result) ? result : 0)
               .Where(codigo => codigo > 0)
               .Distinct()
               .ToList();

            var tipoBaselineCodigos = ((IEnumerable<dynamic>)oferta)
               .SelectMany(tiposBaseline => ((IEnumerable<dynamic>)tiposBaseline.Baseline ?? Enumerable.Empty<dynamic>())
                   .Select(tipoBaseline => tipoBaseline.CodigoTipoBaseline != null ? ((int)tipoBaseline.CodigoTipoBaseline) : 0))
               .Where(codigo => codigo > 0)
               .Distinct()
               .ToList();

            List<Dominio.Entidades.Embarcador.Bidding.Baseline> listaBaselines = await repositorioBaseline.BuscarPorCodigosAsync(baselineCodigos);
            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> listaTiposBaseline = await repositorioTipoBaseline.BuscarPorCodigosAsync(tipoBaselineCodigos);

            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.AliquotaICMS> todasAliquotas = await repAliquotaICMS.BuscarTodasAliquotasParaBidding();
            int protoloIntegracao = await repOfertaRota.BuscarUltimoProtocoloImportacaoAsync();

            List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPOrigem> listaCEPOrigem = new List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPOrigem>();
            List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino> listaCEPDestino = new List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino>();
            List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> listaBiddingOfertaRota = new List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>();
            List<Dominio.Entidades.Embarcador.Bidding.Baseline> listaBaseLine = new List<Dominio.Entidades.Embarcador.Bidding.Baseline>();


            var resultados = new ConcurrentBag<ResultadoProcessamentoRota>();

            Parallel.ForEach(oferta, rota =>
            {
                var resultado = ProcessarRota(
                    rota,
                    Interlocked.Increment(ref protoloIntegracao),
                    entidadeBiddingOferta,
                    listaGrupoModeloVeicular,
                    listaModeloCarroceria,
                    listaTomadores,
                    listaModelosVeicular,
                    listaFiliais,
                    listaTiposCarga,
                    cidadesOrigem,
                    clientesOrigem,
                    estadosOrigem,
                    regioesOrigem,
                    rotasOrigem,
                    paisesOrigem,
                    cidadesDestino,
                    clientesDestino,
                    estadosDestino,
                    regioesDestino,
                    rotasDestino,
                    paisesDestino,
                    listaTiposBaseline,
                    listaBaselines,
                    todasAliquotas);

                resultados.Add(resultado);
            });

            listaBiddingOfertaRota.AddRange(resultados.Select(r => r.BiddingOfertaRota));
            listaCEPOrigem.AddRange(resultados.SelectMany(r => r.CEPsOrigem));
            listaCEPDestino.AddRange(resultados.SelectMany(r => r.CEPsDestino));
            listaBaseLine.AddRange(resultados.SelectMany(r => r.Baselines));

            if (listaBiddingOfertaRota.Count > 0)
                await repOfertaRota.InserirAsync(listaBiddingOfertaRota, "T_BIDDING_OFERTA_ROTA");


            if (listaCEPOrigem.Count > 0)
                await repCEPOrigem.InserirAsync(listaCEPOrigem, "T_BIDDING_OFERTA_ROTA_CEP_ORIGEM");

            if (listaCEPDestino.Count > 0)
                await repCEPDestino.InserirAsync(listaCEPDestino, "T_BIDDING_OFERTA_ROTA_CEP_DESTINO");

            if (listaBaseLine.Count > 0)
                await repositorioBaseline.InserirAsync(listaBaseLine, "T_BASELINE");
        }

        private static void SetDescricaoLocalidade(Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota biddingOfertaRota)
        {

            biddingOfertaRota.DescricaoOrigem = biddingOfertaRota.FlagOrigem switch
            {
                "Cidade" => string.Join(", ", biddingOfertaRota.Origens.Select(p => p.DescricaoCidadeEstado)),
                "Cliente" => string.Join(", ", biddingOfertaRota.ClientesOrigem.Select(p => p.Descricao)),
                "Estado" => string.Join(", ", biddingOfertaRota.EstadosOrigem.Select(p => p.Descricao)),
                "Regiao" => string.Join(", ", biddingOfertaRota.RegioesOrigem.Select(p => p.Descricao)),
                "Rota" => string.Join(", ", biddingOfertaRota.RotasOrigem.Select(p => p.Descricao)),
                "Pais" => string.Join(", ", biddingOfertaRota.PaisesOrigem.Select(p => p.Descricao)),
                "CEP" => string.Join(", ", biddingOfertaRota.CEPsOrigem.Select(p => p.Descricao)),
                _ => string.Empty
            };

            biddingOfertaRota.DescricaoDestino = biddingOfertaRota.FlagDestino switch
            {
                "Cidade" => string.Join(", ", biddingOfertaRota.Destinos.Select(p => p.DescricaoCidadeEstado)),
                "Cliente" => string.Join(", ", biddingOfertaRota.ClientesDestino.Select(p => p.Descricao)),
                "Estado" => string.Join(", ", biddingOfertaRota.EstadosDestino.Select(p => p.Descricao)),
                "Regiao" => string.Join(", ", biddingOfertaRota.RegioesDestino.Select(p => p.Descricao)),
                "Rota" => string.Join(", ", biddingOfertaRota.RotasDestino.Select(p => p.Descricao)),
                "Pais" => string.Join(", ", biddingOfertaRota.PaisesDestino.Select(p => p.Descricao)),
                "CEP" => string.Join(", ", biddingOfertaRota.CEPsDestino.Select(p => p.Descricao)),
                _ => string.Empty
            };
        }

        private static decimal? ObterAlicotaICMSAsync(Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota biddingOfertaRota, List<Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.AliquotaICMS> aliquotas)
        {
            if (aliquotas == null || aliquotas.Count == 0)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.AliquotaICMS aliquota = null;

            string ufOrigem = ObterSiglaUFOrigem(biddingOfertaRota);
            string ufDestino = ObterSiglaUFDestino(biddingOfertaRota);

            if (!string.IsNullOrEmpty(ufOrigem) && !string.IsNullOrEmpty(ufDestino))
                aliquota = aliquotas.FirstOrDefault(o => o.EstadoOrigem.Equals(ufOrigem) && o.EstadoDestino.Equals(ufDestino));

            return aliquota?.Aliquota;
        }

        private static string ObterSiglaUFOrigem(Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota biddingOfertaRota)
        {
            string ufOrigem = string.Empty;

            if (biddingOfertaRota.Origens?.Count > 0)
                ufOrigem = biddingOfertaRota.Origens.Select(x => x.Estado.Sigla).FirstOrDefault();

            else if (biddingOfertaRota.EstadosOrigem?.Count > 0)
                ufOrigem = biddingOfertaRota.EstadosOrigem.Select(x => x.Sigla).FirstOrDefault();

            return ufOrigem;
        }

        private static string ObterSiglaUFDestino(Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota biddingOfertaRota)
        {
            string ufDestino = string.Empty;

            if (biddingOfertaRota.Destinos?.Count > 0)
                ufDestino = biddingOfertaRota.Destinos.Select(x => x.Estado.Sigla).FirstOrDefault();

            else if (biddingOfertaRota.EstadosDestino?.Count > 0)
                ufDestino = biddingOfertaRota.EstadosDestino.Select(x => x.Sigla).FirstOrDefault();

            return ufDestino;
        }

        private async Task AtualizarRotasAsync(Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Bidding.BiddingOfertaRota repBiddingRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingCEPOrigem repCEPOrigem = new Repositorio.Embarcador.Bidding.BiddingCEPOrigem(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingCEPDestino repCEPDestino = new Repositorio.Embarcador.Bidding.BiddingCEPDestino(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repBiddingTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> listaBiddingOfertaRota = await repBiddingRota.BuscarRotasAsync(biddingOferta.Codigo);

            if (await repBiddingTransportadorRota.ExisteRotaVinculoTransportadorAsync(listaBiddingOfertaRota.Select(o => o.Codigo).ToList()))
                return;

            foreach (Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota rota in listaBiddingOfertaRota)
            {
                await repCEPOrigem.DeletarTodosPorCodigoAsync(rota.Codigo);
                await repCEPDestino.DeletarTodosPorCodigoAsync(rota.Codigo);
                await repBiddingRota.DeletarAsync(rota);
            }

            await SalvarRotasAsync(biddingOferta, unitOfWork, cancellationToken);
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding()
            {
                Descricao = Request.Params("Descricao"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa : null,
                CodigoTipoBidding = Request.GetIntParam("TipoBidding"),
                CodigoSolicitante = Request.GetIntParam("Solicitante"),
                CodigosComprador = Request.GetListParam<int>("Comprador"),
                Situacao = Request.GetNullableListParam<StatusBiddingConvite>("Situacao"),
                NumeroBidding = Request.GetIntParam("NumeroBidding"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                FiliaisParticipantes = Request.GetListParam<int>("FiliaisParticipante"),
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número", "Codigo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "TipoBidding", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Solicitante", "Solicitante", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Comprador", "Comprador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Etapa", "Status", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Limite", "DataLimite", 10, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite repositorioAprovacao = new Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> listaBiddingConvite = repBiddingConvite.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                List<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite> listaAutorizacao = repositorioAprovacao.BuscarPorCodigos(listaBiddingConvite.Select(o => o.Codigo).ToList());

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

                int totalRegistros = repBiddingConvite.ContarConsulta(filtrosPesquisa);

                var retorno = (from convite in listaBiddingConvite
                               select new
                               {
                                   convite.Codigo,
                                   convite.Descricao,
                                   Solicitante = convite.Solicitante?.Nome ?? string.Empty,
                                   Comprador = string.Join(", ", listaAutorizacao.Where(o => o.OrigemAprovacao.Codigo == convite.Codigo && o.Situacao == SituacaoAlcadaRegra.Aprovada).Select(o => o.Usuario.Nome).ToList()),
                                   Status = servicoBidding.AutomatizacaoEtapasEmbarcadorAsync(configuracaoBidding, convite, repositorioConviteConvidado, empresa, TipoServicoMultisoftware).Result.ObterDescricao(),
                                   convite.DataInicio,
                                   convite.DataLimite,
                                   TipoBidding = convite.TipoBidding?.Descricao ?? ""
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<Dominio.Entidades.Embarcador.Bidding.Baseline> SalvarBaselineRota(Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota biddingOfertaRota, List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> listaTiposBaseline, List<Dominio.Entidades.Embarcador.Bidding.Baseline> listaBaselines, List<Baseline> dynBaselines)
        {
            List<Dominio.Entidades.Embarcador.Bidding.Baseline> listaBaseline = new List<Dominio.Entidades.Embarcador.Bidding.Baseline>();

            foreach (Baseline biddingBaseline in dynBaselines)
            {
                Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline = listaTiposBaseline.FirstOrDefault(tipoBaseline => biddingBaseline.CodigoTipoBaseline == tipoBaseline.Codigo);

                int? codigo = biddingBaseline.Codigo.ToNullableInt();
                Dominio.Entidades.Embarcador.Bidding.Baseline baseline;

                if (codigo.HasValue)
                    baseline = listaBaselines.FirstOrDefault(baseline => codigo.Value == baseline.Codigo) ?? throw new ControllerException("Não encontrou nenhum registro!");
                else
                    baseline = new Dominio.Entidades.Embarcador.Bidding.Baseline()
                    {
                        BiddingOfertaRota = biddingOfertaRota
                    };

                baseline.Valor = biddingBaseline.Valor.ToDecimal();
                baseline.TipoBaseline = tipoBaseline;
                baseline.BiddingOfertaRota = biddingOfertaRota;

                listaBaseline.Add(baseline);
            }

            return listaBaseline;
        }

        private static TimeSpan ObterHoraImportacao(string hora)
        {
            try
            {
                DateTime.TryParse(hora, out DateTime data);

                return new TimeSpan(data.Hour, data.Minute, 0);
            }
            catch
            {
                return TimeSpan.MinValue;
            }
        }

        public ResultadoProcessamentoRota ProcessarRota(
            Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.Rota rota,
            int protocoloIntegracao,
            Dominio.Entidades.Embarcador.Bidding.BiddingOferta entidadeBiddingOferta,
            List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> listaGrupoModeloVeicular,
            List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> listaModeloCarroceria,
            List<Dominio.Entidades.Cliente> listaTomadores,
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> listaModelosVeicular,
            List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFiliais,
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> listaTiposCarga,
            List<Dominio.Entidades.Localidade> cidadesOrigem,
            List<Dominio.Entidades.Cliente> clientesOrigem,
            List<Dominio.Entidades.Estado> estadosOrigem,
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem,
            List<Dominio.Entidades.RotaFrete> rotasOrigem,
            List<Dominio.Entidades.Pais> paisesOrigem,
            List<Dominio.Entidades.Localidade> cidadesDestino,
            List<Dominio.Entidades.Cliente> clientesDestino,
            List<Dominio.Entidades.Estado> estadosDestino,
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino,
            List<Dominio.Entidades.RotaFrete> rotasDestino,
            List<Dominio.Entidades.Pais> paisesDestino,
            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> listaTiposBaseline,
            List<Dominio.Entidades.Embarcador.Bidding.Baseline> listaBaselines,
            List<AliquotaICMS> todasAliquotas
        )
        {
            var resultado = new Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota.ResultadoProcessamentoRota();
            var biddingOfertaRota = new Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota();

            int codigoGrupoModeloVeicular = rota.GrupoModeloVeicular?.Codigo.ToInt() ?? 0;
            int codigoModeloCarroceria = rota.ModeloCarroceria?.Codigo.ToInt() ?? 0;
            double cpfcnpjTomador = rota.Tomador?.Codigo ?? 0;
            string flagOrigem = rota.FlagOrigem;
            string flagDestino = rota.FlagDestino;
            int codigoDestino = 0;
            int codigoOrigem = 0;

            biddingOfertaRota.BiddingOferta = entidadeBiddingOferta;
            biddingOfertaRota.Descricao = rota.Descricao;
            biddingOfertaRota.QuilometragemMedia = rota.QuilometragemMedia;
            biddingOfertaRota.Frequencia = rota.Frequencia.ToInt();
            biddingOfertaRota.Volume = rota.Volume.ToDecimal();
            biddingOfertaRota.NumeroEntrega = rota.NumeroEntrega;
            biddingOfertaRota.Observacao = rota.Observacao;
            biddingOfertaRota.ValorCargaMes = rota.ValorCargaMes;
            biddingOfertaRota.Peso = rota.Peso;
            biddingOfertaRota.AdicionalAPartirDaEntregaNumero = rota.AdicionalAPartirDaEntregaNumero;
            biddingOfertaRota.FrequenciaMensalComAjudante = rota.FrequenciaMensalComAjudante;
            biddingOfertaRota.QuantidadeAjudantePorVeiculo = rota.QuantidadeAjudantePorVeiculo;
            biddingOfertaRota.MediaEntregasFracionada = rota.MediaEntregasFracionada;
            biddingOfertaRota.MaximaEntregasFacionada = rota.MaximaEntregasFacionada;
            biddingOfertaRota.QuantidadeViagensPorAno = rota.QuantidadeViagensPorAno;
            biddingOfertaRota.VolumeTonAno = rota.VolumeTonAno;
            biddingOfertaRota.VolumeTonViagem = rota.VolumeTonViagem;
            biddingOfertaRota.ValorMedioNFe = rota.ValorMedioNFe;

            biddingOfertaRota.GrupoModeloVeicular = listaGrupoModeloVeicular.FirstOrDefault(g => g.Codigo == codigoGrupoModeloVeicular);
            biddingOfertaRota.Tomador = listaTomadores.FirstOrDefault(t => t.CPF_CNPJ == cpfcnpjTomador);
            biddingOfertaRota.ModeloCarroceria = listaModeloCarroceria.FirstOrDefault(m => m.Codigo == codigoModeloCarroceria);
            biddingOfertaRota.Inconterm = (rota.Inconterm).ToNullableEnum<Inconterm>();
            biddingOfertaRota.Compressor = (rota.Compressor).ToNullableEnum<SimNaoNA>();
            biddingOfertaRota.ProtocoloImportacao = protocoloIntegracao;

            if (!string.IsNullOrWhiteSpace(rota.TempoColeta))
                biddingOfertaRota.TempoColeta = ObterHoraImportacao(rota.TempoColeta);
            if (!string.IsNullOrWhiteSpace(rota.TempoDescarga))
                biddingOfertaRota.TempoDescarga = ObterHoraImportacao(rota.TempoDescarga);

            List<int> codigosModelosVeiculares = ((IEnumerable<dynamic>)rota.ModeloVeicular)
                    .Select(modeloVeicular => ((string)modeloVeicular.Codigo).ToInt())
                    .Distinct()
                    .ToList();

            List<int> codigosFiliais = ((IEnumerable<dynamic>)rota.FiliaisParticipante)
                    .Select(filial => ((string)filial.Codigo).ToInt())
                    .Distinct()
                    .ToList();

            List<int> codigosTiposCarga = ((IEnumerable<dynamic>)rota.TipoCarga)
                    .Select(tipoCarga => ((string)tipoCarga.Codigo).ToInt())
                    .Distinct()
                    .ToList();

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicular = listaModelosVeicular.Where(modeloVeicular => codigosModelosVeiculares.Contains(modeloVeicular.Codigo)).ToList();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisParticipante = listaFiliais.Where(filial => codigosFiliais.Contains(filial.Codigo)).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = listaTiposCarga.Where(tipoCarga => codigosTiposCarga.Contains(tipoCarga.Codigo)).ToList();

            biddingOfertaRota.ModelosVeiculares = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            if (modelosVeicular != null && modelosVeicular.Count > 0)
                biddingOfertaRota.ModelosVeiculares = modelosVeicular;

            biddingOfertaRota.FiliaisParticipante = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            if (filiaisParticipante != null && filiaisParticipante.Count > 0)
                biddingOfertaRota.FiliaisParticipante = filiaisParticipante;

            biddingOfertaRota.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            if (tiposCarga != null && tiposCarga.Count > 0)
                biddingOfertaRota.TiposCarga = tiposCarga;

            biddingOfertaRota.FlagOrigem = flagOrigem;
            biddingOfertaRota.FlagDestino = flagDestino;

            switch (flagOrigem)
            {
                case "Cidade":
                    biddingOfertaRota.Origens = new List<Dominio.Entidades.Localidade>();
                    biddingOfertaRota.FlagOrigem = "Cidade";
                    foreach (var cidade in rota.CidadeOrigem)
                    {
                        codigoOrigem = (cidade.Codigo).ToInt();
                        Dominio.Entidades.Localidade localidadeEntidade = cidadesOrigem.FirstOrDefault(cidadeOrigem => cidadeOrigem.Codigo == codigoOrigem);
                        biddingOfertaRota.Origens.Add(localidadeEntidade);
                    }
                    break;
                case "Cliente":
                    biddingOfertaRota.FlagOrigem = "Cliente";
                    biddingOfertaRota.ClientesOrigem = new List<Dominio.Entidades.Cliente>();
                    foreach (var cliente in rota.ClienteOrigem)
                    {
                        double cpfcnpj = (cliente.Codigo).ToDouble();
                        Dominio.Entidades.Cliente clienteEntidade = clientesOrigem.FirstOrDefault(clienteOrigem => clienteOrigem.CPF_CNPJ == cpfcnpj);
                        biddingOfertaRota.ClientesOrigem.Add(clienteEntidade);
                    }
                    break;
                case "Estado":
                    biddingOfertaRota.FlagOrigem = "Estado";
                    biddingOfertaRota.EstadosOrigem = new List<Dominio.Entidades.Estado>();
                    foreach (var estado in rota.EstadoOrigem)
                    {
                        string siglaOrigem = (estado.Codigo).ToString();

                        Dominio.Entidades.Estado estadoEntidade = estadosOrigem.FirstOrDefault(estadoOrigem => estadoOrigem.Sigla.Equals(siglaOrigem));
                        biddingOfertaRota.EstadosOrigem.Add(estadoEntidade);
                    }
                    break;
                case "Regiao":
                    biddingOfertaRota.FlagOrigem = "Regiao";
                    biddingOfertaRota.RegioesOrigem = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
                    foreach (var regiao in rota.RegiaoOrigem)
                    {
                        codigoOrigem = (regiao.Codigo).ToInt();
                        Dominio.Entidades.Embarcador.Localidades.Regiao regiaoEntidade = regioesOrigem.FirstOrDefault(regiaoOrigem => regiaoOrigem.Codigo == codigoOrigem);
                        biddingOfertaRota.RegioesOrigem.Add(regiaoEntidade);
                    }
                    break;
                case "Rota":
                    biddingOfertaRota.FlagOrigem = "Rota";
                    biddingOfertaRota.RotasOrigem = new List<Dominio.Entidades.RotaFrete>();
                    foreach (var rotaO in rota.RotaOrigem)
                    {
                        codigoOrigem = (rotaO.Codigo).ToInt();

                        Dominio.Entidades.RotaFrete rotaEntidade = rotasOrigem.FirstOrDefault(rotaOrigem => rotaOrigem.Codigo == codigoOrigem);
                        biddingOfertaRota.RotasOrigem.Add(rotaEntidade);
                    }
                    break;
                case "Pais":
                    biddingOfertaRota.FlagOrigem = "Pais";
                    biddingOfertaRota.PaisesOrigem = new List<Dominio.Entidades.Pais>();
                    foreach (var pais in rota.PaisOrigem)
                    {
                        codigoOrigem = (pais.Codigo).ToInt();

                        Dominio.Entidades.Pais paisEntidade = paisesOrigem.FirstOrDefault(paisOrigem => paisOrigem.Codigo == codigoOrigem);
                        biddingOfertaRota.PaisesOrigem.Add(paisEntidade);
                    }
                    break;
                case "CEP":
                    biddingOfertaRota.FlagOrigem = "CEP";
                    foreach (var cep in rota.CEPOrigem)
                    {
                        Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPOrigem entidadeCEPOrigem = new Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPOrigem();
                        string CEPInicial = String.Join("", System.Text.RegularExpressions.Regex.Split((cep.CEPInicial).ToString(), @"[^\d]"));
                        string CEPFinal = String.Join("", System.Text.RegularExpressions.Regex.Split((cep.CEPFinal).ToString(), @"[^\d]"));
                        entidadeCEPOrigem.BiddingOfertaRota = biddingOfertaRota;
                        entidadeCEPOrigem.CEPInicial = CEPInicial.ToInt();
                        entidadeCEPOrigem.CEPFinal = CEPFinal.ToInt();
                        resultado.CEPsOrigem.Add(entidadeCEPOrigem);
                    }
                    break;
            }

            switch (flagDestino)
            {
                case "Cidade":
                    biddingOfertaRota.FlagDestino = "Cidade";
                    biddingOfertaRota.Destinos = new List<Dominio.Entidades.Localidade>();
                    foreach (var cidade in rota.CidadeDestino)
                    {
                        codigoDestino = (cidade.Codigo).ToInt();
                        Dominio.Entidades.Localidade localidadeEntidade = cidadesDestino.FirstOrDefault(cidadesDestino => cidadesDestino.Codigo == codigoDestino);
                        biddingOfertaRota.Destinos.Add(localidadeEntidade);
                    }
                    break;
                case "Cliente":
                    biddingOfertaRota.FlagDestino = "Cliente";
                    biddingOfertaRota.ClientesDestino = new List<Dominio.Entidades.Cliente>();
                    foreach (var cliente in rota.ClienteDestino)
                    {
                        double cpfcnpj = (cliente.Codigo).ToDouble();
                        Dominio.Entidades.Cliente clienteEntidade = clientesDestino.FirstOrDefault(clienteDestino => clienteDestino.CPF_CNPJ == cpfcnpj);
                        biddingOfertaRota.ClientesDestino.Add(clienteEntidade);
                    }
                    break;
                case "Estado":
                    biddingOfertaRota.FlagDestino = "Estado";
                    biddingOfertaRota.EstadosDestino = new List<Dominio.Entidades.Estado>();
                    foreach (var estado in rota.EstadoDestino)
                    {
                        string siglaDestino = (estado.Codigo).ToString();
                        Dominio.Entidades.Estado estadoEntidade = estadosDestino.FirstOrDefault(estadosDestino => estadosDestino.Sigla.Equals(siglaDestino));
                        biddingOfertaRota.EstadosDestino.Add(estadoEntidade);
                    }
                    break;
                case "Regiao":
                    biddingOfertaRota.FlagDestino = "Regiao";
                    biddingOfertaRota.RegioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
                    foreach (var regiao in rota.RegiaoDestino)
                    {
                        codigoDestino = (regiao.Codigo).ToInt();
                        Dominio.Entidades.Embarcador.Localidades.Regiao regiaoEntidade = regioesDestino.FirstOrDefault(regiaoDestino => regiaoDestino.Codigo == codigoDestino);
                        biddingOfertaRota.RegioesDestino.Add(regiaoEntidade);
                    }
                    break;
                case "Rota":
                    biddingOfertaRota.FlagDestino = "Rota";
                    biddingOfertaRota.RotasDestino = new List<Dominio.Entidades.RotaFrete>();
                    foreach (var rotaO in rota.RotaDestino)
                    {
                        codigoDestino = (rotaO.Codigo).ToInt();
                        Dominio.Entidades.RotaFrete rotaEntidade = rotasDestino.FirstOrDefault(rotasDestino => rotasDestino.Codigo == codigoDestino);
                        biddingOfertaRota.RotasDestino.Add(rotaEntidade);
                    }
                    break;
                case "Pais":
                    biddingOfertaRota.FlagDestino = "Pais";
                    biddingOfertaRota.PaisesDestino = new List<Dominio.Entidades.Pais>();
                    foreach (var pais in rota.PaisDestino)
                    {
                        codigoDestino = (pais.Codigo).ToInt();
                        Dominio.Entidades.Pais paisEntidade = paisesDestino.FirstOrDefault(paisesDestino => paisesDestino.Codigo == codigoDestino);
                        biddingOfertaRota.PaisesDestino.Add(paisEntidade);
                    }
                    break;
                case "CEP":
                    biddingOfertaRota.FlagDestino = "CEP";
                    foreach (var cep in rota.CEPDestino)
                    {
                        Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino entidadeCEPDestino = new Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino();
                        string CEPInicial = String.Join("", System.Text.RegularExpressions.Regex.Split((cep.CEPInicial).ToString(), @"[^\d]"));
                        string CEPFinal = String.Join("", System.Text.RegularExpressions.Regex.Split((cep.CEPFinal).ToString(), @"[^\d]"));
                        entidadeCEPDestino.BiddingOfertaRota = biddingOfertaRota;
                        entidadeCEPDestino.CEPInicial = CEPInicial.ToInt();
                        entidadeCEPDestino.CEPFinal = CEPFinal.ToInt();
                        resultado.CEPsDestino.Add(entidadeCEPDestino);
                    }
                    break;
            }

            SetDescricaoLocalidade(biddingOfertaRota);

            biddingOfertaRota.AlicotaPadraoICMS = ObterAlicotaICMSAsync(biddingOfertaRota, todasAliquotas);

            resultado.BiddingOfertaRota = biddingOfertaRota;
            resultado.Baselines.AddRange(SalvarBaselineRota(biddingOfertaRota, listaTiposBaseline, listaBaselines, rota.Baseline));

            return resultado;
        }

        #endregion Métodos Privados
    }
}
