using System.Dynamic;
using System.Linq.Dynamic.Core;
using Dominio.Entidades.Embarcador.Bidding;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Bidding;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingAvaliacao")]
    public class BiddingAvaliacaoController : BaseController
    {
        #region Construtores

        public BiddingAvaliacaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingOferta repOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingOfertaRota repOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Bidding.BiddingChecklist repositorioBiddingCheckList = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);

                BiddingConvite entidadeBiddingConvite = await repBiddingConvite.BuscarPorCodigoAsync(codigo, false);

                if (entidadeBiddingConvite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta = await repOferta.BuscarOfertaAsync(entidadeBiddingConvite);
                List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaDados> listaTransportadorRota = await repTransportadorRota.BuscarTransportadorRotaPorBiddingSomenteOfertadasAsync(entidadeBiddingConvite);
                IList<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> listaRotas = repOfertaRota.BuscarRotasProcessadas(biddingOferta.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaOfertaDados> listaOfertas = await repositorioTransportadorOferta.BuscarPorCodigosTransportadorRotaOfertaAsync(listaTransportadorRota.Select(obj => obj.Codigo).ToList());
                IList<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas = repTransportadorOferta.BuscarValoresPorCodigosTransportadorRota(listaTransportadorRota.Select(obj => obj.Codigo).ToList());

                DateTime? dataPrazoChecklist = await repositorioBiddingCheckList.BuscarDataPrazoChecklistAsync(entidadeBiddingConvite);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = await repositorioConfiguracaoBidding.BuscarPrimeiroRegistroAsync();

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                StatusBiddingConvite statusBidding = await servicoBidding.AutomatizacaoEtapasEmbarcadorAsync(configuracaoBidding, entidadeBiddingConvite, repositorioConviteConvidado, this.Usuario.Empresa, TipoServicoMultisoftware);

                TipoLanceBidding tipoLance = listaOfertas.Select(o => o.TipoLance).FirstOrDefault();

                return new JsonpResult(new
                {
                    Prazos = new
                    {
                        PrazoConvite = entidadeBiddingConvite.DataPrazoAceiteConvite.HasValue ? entidadeBiddingConvite.DataPrazoAceiteConvite.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        PrazoCheckList = dataPrazoChecklist?.ToString("dd/MM/yyyy HH:mm"),
                        PrazoOferta = biddingOferta.DataPrazoOferta.HasValue ? biddingOferta.DataPrazoOferta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                    },
                    Dados = new
                    {
                        entidadeBiddingConvite.Codigo,
                        Situacao = statusBidding,
                        entidadeBiddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador,
                        TipoBidding = entidadeBiddingConvite.TipoBidding.CodigoIntegracao,
                        entidadeBiddingConvite.TipoBidding.ExibirRankOfertas
                    },
                    Resumo = new
                    {
                        entidadeBiddingConvite.Codigo,
                        DataLimite = entidadeBiddingConvite.DataLimite.ToString("dd/MM/yyyy"),
                        Situacao = statusBidding.ObterDescricao(),
                        CodigoBiddingOferta = biddingOferta.Codigo,
                    },
                    Convites = ObterConvite(entidadeBiddingConvite, unitOfWork),
                    Checklist = ObterChecklist(entidadeBiddingConvite, unitOfWork),
                    TipoPreenchimentoChecklist = ObterTipoPreenchimentoChecklist(entidadeBiddingConvite, unitOfWork),
                    Ofertas = (
                    from o in listaTransportadorRota
                    select new
                    {
                        o.Codigo,
                        Transportador = o.Transportador,
                        RotaCodigo = o.RotaCodigo,
                        RotaDescricao = o.RotaDescricao,
                        o.Ranking,
                        Situacao = o.Status.ObterDescricao(),
                        SituacaoEnum = o.Status,
                        Rodada = $"{o.Rodada}ª Rodada",
                        Target = o.Target.ToString("n2"),
                        CustoEstimado = (from obj in listaOfertas where obj.CodigoTransportadorRota == o.Codigo select obj.CustoEstimado).DefaultIfEmpty().Min().ToString("n2"),
                        DataRetorno = o.DataRetorno?.ToString("dd/MM/yyyy")
                    }),
                    TipoOferta = tipoLance,
                    TipoLance = biddingOferta.TipoLance,
                    TransportadoresDasRotas = (from obj in listaTransportadorRota
                                               select new
                                               {
                                                   obj.Codigo,
                                                   Transportador = obj.Transportador,
                                                   TransportadorCodigo = obj.TransportadorCodigo,
                                                   Valor = listaTotalOfertas.ToList().Find(x => x.codigoTransportador == obj.TransportadorCodigo && x.codigoRota == obj.RotaCodigo).custoEstimado.ToString("n2"),
                                                   RotaCodigo = obj.RotaCodigo,
                                               }).ToList(),
                    OfertasComponente = (from obj in listaTransportadorRota
                                         let oferta = listaOfertas.Find(o => o.CodigoTransportadorRota == obj.Codigo)
                                         select new Dominio.ObjetosDeValor.Embarcador.Bidding.OfertaComponenteDados
                                         {
                                             CodigoRota = obj.RotaCodigo,
                                             CodigoTransportador = obj.TransportadorCodigo,
                                             RotaDescricao = obj.RotaDescricao,
                                             Origem = obj.Origem,
                                             Destino = obj.Destino,
                                             ValorFrete = oferta?.FreteTonelada.ToString("n2") ?? string.Empty,
                                             AdicionalPorEntrega = oferta?.AdicionalPorEntrega.ToString("n2") ?? string.Empty,
                                             Ajudante = oferta?.Ajudante.ToString("n2") ?? string.Empty,
                                             Pedagio = oferta?.PedagioParaEixo.ToString("n2") ?? string.Empty,
                                             Transportador = obj.Transportador,
                                             VeiculosVerdes = oferta?.VeiculosVerdes ?? 0,
                                             Total = (
                                             (oferta?.Ajudante ?? 0)
                                             + (oferta?.AdicionalPorEntrega ?? 0)
                                             + (oferta?.FreteTonelada ?? 0) +
                                             (oferta?.PedagioParaEixo ?? 0))
                                         }).ToList(),
                    Filtros = servicoBidding.ProcessarListaRotas(listaRotas)
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> RecarregarGridConviteChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite = repositorioBiddingConvite.BuscarPorCodigo(codigo, false);

                if (biddingConvite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Convites = ObterConvite(biddingConvite, unitOfWork),
                    Checklist = ObterChecklist(biddingConvite, unitOfWork)
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarRespostas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta repRespostas = new Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta(unitOfWork);
                List<BiddingAceitamentoQuestaoResposta> listaRespostas = repRespostas.BuscarPorBiddingChecklistTransportador(codigo);

                Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo repAnexos = new Repositorio.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo(unitOfWork);
                List<BiddingAceitamentoQuestionarioAnexo> listaAnexos = repAnexos.BuscarPorRespostas(listaRespostas);

                return new JsonpResult(new
                {
                    Respostas = (
                   from o in listaRespostas
                   select new
                   {
                       o.Codigo,
                       Pergunta = $"{o.Pergunta.Descricao} ({o.Pergunta.Requisito.ObterDescricao()})",
                       Resposta = o.Resposta == true ? "Sim" : "Não",
                       Observacao = string.IsNullOrWhiteSpace(o.Observacao) ? "Sem observação." : o.Observacao,
                       Anexos = (
                       from anexo in listaAnexos
                       where anexo.EntidadeAnexo.Codigo == o.Codigo
                       select new
                       {
                           anexo.Codigo,
                           anexo.Descricao,
                           anexo.NomeArquivo
                       }).ToList()
                   })
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarOfertas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota = Request.GetIntParam("CodigoRota");
                int codigoTransportador = Request.GetIntParam("CodigoTransportador");
                decimal valorCalculoTotalLiquido = 0.9075M;

                decimal freteComICMSPeso = 0;
                decimal pedagioComICMSPeso = 0;
                decimal totalBrutoPesoTon = 0;
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
                decimal adicionalFracionadoComICMS = 0;
                decimal adicionalAjudanteComICMS = 0;
                decimal adicionalFracionadoAjudanteComICMS = 0;

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                BiddingTransportadorRota biddingTransportadorRota = repTransportadorRota.BuscarPorRotaETransportador(codigoRota, codigoTransportador);

                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                List<BiddingTransportadorRotaOferta> listaOfertas = repTransportadorOferta.BuscarPorCodigoTransportadorRota(biddingTransportadorRota.Codigo);

                Repositorio.Embarcador.Bidding.BiddingOfertaRota repOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                List<BiddingOfertaRota> listaRotas = repOfertaRota.BuscarRotasPorBidding(biddingTransportadorRota.Rota.BiddingOferta.BiddingConvite);

                foreach (BiddingTransportadorRotaOferta oferta in listaOfertas)
                {
                    if (oferta.TipoOferta == TipoLanceBidding.LancePorPeso)
                    {
                        freteComICMSPeso = (oferta.FreteTonelada / (1 - (oferta.ICMSPorcentagem / 100)));
                        pedagioComICMSPeso = ((oferta.PedagioParaEixo / (1 - (oferta.ICMSPorcentagem / 100))) * oferta.ModeloVeicular.NumeroEixos) ?? 0;
                        totalBrutoPeso = pedagioComICMSPeso > 0 ? freteComICMSPeso + (pedagioComICMSPeso / (oferta.ModeloVeicular.CapacidadePesoTransporte / 1000)) : freteComICMSPeso;
                        totalLiquidoPeso = Math.Round((totalBrutoPeso * (1 - (oferta.ICMSPorcentagem / 100)) * valorCalculoTotalLiquido), 2);
                    }

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorCapacidade)
                    {
                        freteComICMSCapacidadeTon = (oferta.FreteTonelada / (1 - (oferta.ICMSPorcentagem / 100)));
                        freteComICMSCapacidade = freteComICMSCapacidadeTon * (oferta.ModeloVeicular.CapacidadePesoTransporte / 1000);
                        pedagioComICMSCapacidade = ((oferta.PedagioParaEixo / (1 - (oferta.ICMSPorcentagem / 100))) * oferta.ModeloVeicular.NumeroEixos) ?? 0;
                        totalBrutoCapacidade = pedagioComICMSCapacidade > 0 ? (freteComICMSCapacidade + pedagioComICMSCapacidade) : freteComICMSCapacidade;
                        totalLiquidoCapacidade = Math.Round((totalBrutoCapacidade * (1 - (oferta.ICMSPorcentagem / 100)) * valorCalculoTotalLiquido), 2);
                    }

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorFreteViagem)
                    {
                        totalBrutoViagem = (oferta.FreteTonelada / (1 - (oferta.ICMSPorcentagem / 100))) + (oferta.PedagioParaEixo / (1 - (oferta.ICMSPorcentagem / 100)));
                        totalLiquidoViagem = Math.Round((totalBrutoViagem * (1 - (oferta.ICMSPorcentagem / 100)) * valorCalculoTotalLiquido), 2);
                    }

                    if (oferta.TipoOferta == TipoLanceBidding.LancePorViagemEntregaAjudante)
                    {
                        viagemComPedagio = (oferta.FreteTonelada + oferta.PedagioParaEixo) / (1 - (oferta.ICMSPorcentagem / 100));
                        adicionalAjudanteComICMS = (oferta.FreteTonelada + oferta.PedagioParaEixo + oferta.Ajudante) / (1 - (oferta.ICMSPorcentagem / 100));
                        adicionalFracionadoComICMS = (oferta.FreteTonelada + oferta.PedagioParaEixo + (oferta.AdicionalPorEntrega * (listaRotas.Find(x => x.Codigo == oferta.TransportadorRota.Rota.Codigo).MediaEntregasFracionada - 1))) / (1 - (oferta.ICMSPorcentagem / 100));
                        adicionalFracionadoAjudanteComICMS = (oferta.FreteTonelada + oferta.PedagioParaEixo + oferta.Ajudante + (oferta.AdicionalPorEntrega * (listaRotas.Find(x => x.Codigo == oferta.TransportadorRota.Rota.Codigo).MediaEntregasFracionada - 1))) / (1 - (oferta.ICMSPorcentagem / 100));
                    }
                }

                return new JsonpResult(new
                {
                    Rodada = listaOfertas.Count > 0 ? $"{listaOfertas[0].Rodada.ToString()}ª" : "",
                    Codigo = biddingTransportadorRota.Codigo,
                    SituacaoEnum = biddingTransportadorRota.Status,
                    Tabs = (
                    from o in listaOfertas
                    select new
                    {
                        Identificador = $"#tabDetalhe{(int)o.TipoOferta}"
                    }),
                    Ofertas = (
                        from o in listaOfertas
                        select new
                        {
                            o.Codigo,
                            CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                            Descricao = $"{o.ModeloVeicular.Descricao} ({o.TipoOferta.ObterDescricao()})"
                        }
                    ),
                    TransportadoresDasRotas = (from obj in listaOfertas
                                               select new
                                               {
                                                   Codigo = obj.Codigo,
                                                   Transportador = obj.TransportadorRota.Transportador.NomeFantasia,
                                                   TransportadorCodigo = obj.TransportadorRota.Transportador.Codigo,
                                                   Rota = obj.TransportadorRota.Rota.Descricao,
                                                   RotaCodigo = obj.TransportadorRota.Rota.Codigo,
                                               }).ToList(),

                    Equipamento = (
                    from o in listaOfertas
                    where o.TipoOferta == TipoLanceBidding.LancePorEquipamento
                    select new
                    {
                        o.Codigo,
                        CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                        ModeloVeicular = o.ModeloVeicular.Descricao,
                        ValorMes = o.ValorFixoEquipamento
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
                        o.Porcentagem
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
                        ValorKm = o.ValorKmRodado
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
                        o.Quilometragem
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
                        Adicional = o.ValorEntrega
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
                        TotalLiquido = totalBrutoPeso != 0 ? totalLiquidoPeso.ToString("n2") : 0.ToString("n2")
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
                        TotalLiquido = totalBrutoCapacidade != 0 ? totalLiquidoCapacidade.ToString("n2") : 0.ToString("n2")
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
                        TotalLiquido = (totalBrutoViagem != 0) ? totalLiquidoViagem.ToString("n2") : 0.ToString("n2")
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
                        AdicionalFracionadoComICMS = adicionalFracionadoComICMS.ToString("n2") ?? 0.ToString("n2"),
                        AdicionalFracionadoAjudanteComICMS = adicionalFracionadoAjudanteComICMS.ToString("n2") ?? 0.ToString("n2"),
                    }),

                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> SalvarTitularidadeTransportador()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("CodigoOferta");
                List<int> codigos = Request.GetListParam<int>("CodigosOfertas");

                TipoTransportadorBidding titularidade = Request.GetEnumParam<TipoTransportadorBidding>("Titularidade");

                if (codigo > 0)
                    codigos.Add(codigo);

                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorio = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);

                List<BiddingTransportadorRotaOferta> ofertas = repositorio.BuscarPorCodigos(codigos.Distinct().ToList());

                foreach (BiddingTransportadorRotaOferta oferta in ofertas)
                {
                    oferta.TipoTransportador = titularidade;
                    repositorio.Atualizar(oferta);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a titularidade da oferta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDuvidas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingDuvida repDuvidas = new Repositorio.Embarcador.Bidding.BiddingDuvida(unitOfWork);
                List<BiddingDuvida> listaDuvidas = repDuvidas.BuscarPorConvite(codigo);

                return new JsonpResult(new
                {
                    Duvidas = (
                    from o in listaDuvidas
                    select new
                    {
                        o.Codigo,
                        Data = o.Data.ToString("dd/MM/yyyy"),
                        o.Pergunta,
                        o.Resposta,
                        Transportador = o.Empresa?.NomeCNPJ ?? string.Empty,
                    })
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> AprovarChecklist()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");
                Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador repChecklistTransportador = new Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                BiddingChecklistBiddingTransportador entidadeChecklistTransportador = repChecklistTransportador.BuscarPorCodigo(codigo, false);
                BiddingConviteConvidado convidado = repConviteConvidado.BuscarConvidado(entidadeChecklistTransportador.BiddingConvite, entidadeChecklistTransportador.Transportador);

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                convidado.StatusBidding = StatusBiddingConvite.Ofertas;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, entidadeChecklistTransportador, "Aceitou a checklist", unitOfWork);

                entidadeChecklistTransportador.Observacao = observacao;
                entidadeChecklistTransportador.Situacao = StatusBiddingConviteTransportadorRespostas.Aprovado;

                repChecklistTransportador.Atualizar(entidadeChecklistTransportador);
                repConviteConvidado.Atualizar(convidado);
                servicoBidding.EnviarRotas(convidado.BiddingConvite, this.Usuario, unitOfWork);

                unitOfWork.CommitChanges();

                servicoBidding.NotificarConvidado($"Sua checklist referente ao bidding \"{convidado.BiddingConvite.Descricao}\" foi aprovada. <br />Observação: {observacao} <br /> Aguarde o início da próxima etapa.", "Aviso Checklist Bidding", convidado);

                List<BiddingChecklistBiddingTransportador> listaChecklist = repChecklistTransportador.BuscarPorBidding(entidadeChecklistTransportador.BiddingConvite);

                return new JsonpResult(
                    new
                    {
                        Checklist = (
                            from o in listaChecklist
                            where o.Situacao != StatusBiddingConviteTransportadorRespostas.Reprovado
                            select new
                            {
                                o.Codigo,
                                Transportador = o.Transportador.NomeFantasia,
                                Aceitacao = o.Aceitamento,
                                AceitacaoDesejavel = o.AceitamentoDesejavel,
                                Situacao = o.Situacao.ObterDescricao(),
                                DataRetorno = o.DataRetorno.ToString("dd/MM/yyyy")
                            })
                    }
                );
            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarChecklist()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador repChecklistTransportador = new Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportadorRotaOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador entidadeChecklistTransportador = repChecklistTransportador.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado convidado = repConviteConvidado.BuscarConvidado(entidadeChecklistTransportador.BiddingConvite, entidadeChecklistTransportador.Transportador);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> listaTransportadorRota = repositorioTransportadorRota.BuscarPorBiddingConviteETransportador(entidadeChecklistTransportador.BiddingConvite, entidadeChecklistTransportador.Transportador.Codigo);

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota transportadorRota in listaTransportadorRota)
                {
                    transportadorRota.TransportadorRejeitado = true;
                    repositorioTransportadorRota.Atualizar(transportadorRota);
                }

                convidado.Status = StatusBiddingConviteConvidado.Rejeitado;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, entidadeChecklistTransportador, "Aceitou a checklist", unitOfWork);

                entidadeChecklistTransportador.Observacao = observacao;
                entidadeChecklistTransportador.Situacao = StatusBiddingConviteTransportadorRespostas.Reprovado;

                repChecklistTransportador.Atualizar(entidadeChecklistTransportador);
                repConviteConvidado.Atualizar(convidado);

                unitOfWork.CommitChanges();

                servicoBidding.NotificarConvidado($"Sua checklist referente ao bidding \"{convidado.BiddingConvite.Descricao}\" foi reprovada. <br />Observação: {observacao}", "Aviso Checklist Bidding", convidado);

                List<BiddingChecklistBiddingTransportador> listaChecklist = repChecklistTransportador.BuscarPorBidding(entidadeChecklistTransportador.BiddingConvite);

                return new JsonpResult(
                    new
                    {
                        Checklist = (
                            from o in listaChecklist
                            where o.Situacao != StatusBiddingConviteTransportadorRespostas.Reprovado
                            select new
                            {
                                o.Codigo,
                                Transportador = o.Transportador.NomeFantasia,
                                Aceitacao = o.Aceitamento,
                                AceitacaoDesejavel = o.AceitamentoDesejavel,
                                Situacao = o.Situacao.ObterDescricao(),
                                DataRetorno = o.DataRetorno.ToString("dd/MM/yyyy")
                            })
                    }
                );

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProporNovaRodada()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                decimal target = Request.GetDecimalParam("Target");

                if (target <= 0)
                    return new JsonpResult(false, true, "É necessário preencher o campo de Valor Alvo para propor uma nova rodada.");

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorRotaOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);

                Repositorio.Embarcador.Bidding.BiddingOfertaRota repOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                Servicos.Embarcador.Bidding.Bidding serBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                BiddingTransportadorRota transportadorRotaAntiga = repTransportadorRota.BuscarPorCodigo(codigo, false);

                if (transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite.Status != StatusBiddingConvite.Ofertas)
                    return new JsonpResult(false, true, "É necessário que o bidding esteja na etapa de ofertas para propor uma nova rodada.");

                BiddingTransportadorRota transportadorRotaNova = new BiddingTransportadorRota();

                transportadorRotaNova.Rodada = transportadorRotaAntiga.Rodada + 1;
                transportadorRotaNova.Rota = transportadorRotaAntiga.Rota;
                transportadorRotaNova.Status = StatusBiddingRota.Aguardando;
                transportadorRotaNova.Transportador = transportadorRotaAntiga.Transportador;
                transportadorRotaNova.Target = target;

                transportadorRotaAntiga.Status = StatusBiddingRota.NovaRodada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, this.Usuario, null, $"Propôs uma nova rodada {transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite.Descricao}.", unitOfWork);

                repTransportadorRota.Inserir(transportadorRotaNova);
                repTransportadorRota.Atualizar(transportadorRotaAntiga);

                serBidding.AtualizarRanking(transportadorRotaAntiga.Rota);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                BiddingConviteConvidado convidado = new BiddingConviteConvidado
                {
                    Convidado = transportadorRotaNova.Transportador
                };

                servicoBidding.NotificarConvidado($"Uma nova rodada ({transportadorRotaAntiga.Rodada}) foi proposta para a sua oferta da rota \"{transportadorRotaAntiga.Rota.Descricao}\", de Número: {transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite.Codigo}.", "Aviso Bidding - Oferta", convidado);

                List<BiddingOfertaRota> listaRotas = repOfertaRota.BuscarRotasPorBidding(transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite);
                List<BiddingTransportadorRota> listaTransportadorRota = repTransportadorRota.BuscarPorBiddingSomenteOfertadas(transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite);
                List<BiddingTransportadorRotaOferta> listaOfertas = repTransportadorRotaOferta.BuscarPorCodigosTransportadorRota(listaTransportadorRota.Select(obj => obj.Codigo).ToList());

                return new JsonpResult(new
                {
                    ListaRotas = (
                    from o in listaRotas
                    select new
                    {
                        o.Codigo,
                        o.Descricao
                    }),
                    ListaModelosVeiculares = ObterModelosVeiculares(listaOfertas)

                });

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProporMultiplasRodadas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOfertaRota repositorioOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                Repositorio.Embarcador.Bidding.Baseline repositorioBaseline = new Repositorio.Embarcador.Bidding.Baseline(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportadorRotaOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);

                Servicos.Embarcador.Bidding.Bidding serBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                dynamic multiplasRotasTransportadores = JsonConvert.DeserializeObject<dynamic>(Request.Params("MultiplasRotasTransportadores"));

                List<(int CodigoRota, decimal Target)> listaMultiplasNovasRodadas = new List<(int CodigoRota, decimal Target)>();

                foreach (dynamic item in multiplasRotasTransportadores)
                {
                    listaMultiplasNovasRodadas.Add(ValueTuple.Create(((string)item.Codigo).ToInt(), ((string)item.Target).ToDecimal()));
                }

                if (listaMultiplasNovasRodadas.Any(o => o.Target == 0))
                    return new JsonpResult(false, true, "É necessário preencher o campo de Valor Alvo para propor uma nova rodada.");

                int codigoRota = listaMultiplasNovasRodadas.Select(o => o.CodigoRota).FirstOrDefault();
                List<int> codigosRotas = listaMultiplasNovasRodadas.Select(o => o.CodigoRota).ToList();

                List<BiddingTransportadorRota> transportadorRotasAntigas = await repositorioTransportadorRota.BuscarPorRotasAsync(codigosRotas);

                BiddingConvite biddingConvite = transportadorRotasAntigas.FirstOrDefault()?.Rota.BiddingOferta.BiddingConvite ?? null;

                List<Dominio.Entidades.Embarcador.Bidding.Baseline> baselines = repositorioBaseline.BuscarPorBiddingConviteERotas(biddingConvite?.Codigo ?? 0, codigoRota);

                List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> biddingTransportadoresRotaOferta = repositorioTransportadorRotaOferta.BuscarPorCodigosTransportadorRota(transportadorRotasAntigas.Select(o => o.Codigo).ToList());

                List<int> rotasComOfertas = transportadorRotasAntigas.Select(o => o.Rota.Codigo).Distinct().ToList();
                List<int> rotasSemOfertas = codigosRotas.Except(rotasComOfertas).ToList();

                if (rotasSemOfertas.Any())
                {
                    List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> descricaoRotasSemOfertas = repositorioOfertaRota.BuscarRotasPorCodigo(rotasSemOfertas);

                    string rotasSemOfertasStr = string.Join(", ", descricaoRotasSemOfertas.Select(r => r.Descricao));
                    return new JsonpResult(false, true, $"As seguintes rotas não têm ofertas para propor uma nova rodada: {rotasSemOfertasStr}.");
                }

                if (!transportadorRotasAntigas.Any())
                    return new JsonpResult(false, true, "Rota não tem ofertas para propor uma nova rodada.");

                if (biddingConvite != null && biddingConvite.Status != StatusBiddingConvite.Ofertas)
                    return new JsonpResult(false, true, "É necessário que o bidding esteja na etapa de ofertas para propor uma nova rodada.");

                if (transportadorRotasAntigas.Exists(o => o.Status != StatusBiddingRota.EmAnalise))
                    return new JsonpResult(false, true, "Existem rotas selecionadas com situação diferente de Aguardando Avaliação.");

                foreach (BiddingTransportadorRota transportadorRotaAntiga in transportadorRotasAntigas)
                {
                    BiddingTransportadorRota transportadorRotaNova = new BiddingTransportadorRota();

                    transportadorRotaNova.ValorAnterior = biddingTransportadoresRotaOferta.Find(o => o.TransportadorRota.Rota.Codigo == transportadorRotaAntiga.Rota.Codigo && o.TransportadorRota.Transportador.Codigo == transportadorRotaAntiga.Transportador.Codigo).CustoEstimado;
                    transportadorRotaNova.Rodada = transportadorRotaAntiga.Rodada + 1;
                    transportadorRotaNova.Rota = transportadorRotaAntiga.Rota;
                    transportadorRotaNova.Status = StatusBiddingRota.Aguardando;
                    transportadorRotaNova.Transportador = transportadorRotaAntiga.Transportador;
                    transportadorRotaNova.Target = listaMultiplasNovasRodadas.Find(x => x.CodigoRota == transportadorRotaAntiga.Rota.Codigo).Target;

                    transportadorRotaAntiga.Status = StatusBiddingRota.NovaRodada;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, this.Usuario, null, $"Propôs uma nova rodada {transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite.Descricao}.", unitOfWork);

                    repositorioTransportadorRota.Inserir(transportadorRotaNova);
                    repositorioTransportadorRota.Atualizar(transportadorRotaAntiga);

                    serBidding.AtualizarRanking(transportadorRotaAntiga.Rota);

                    BiddingConviteConvidado convidado = new BiddingConviteConvidado
                    {
                        Convidado = transportadorRotaNova.Transportador
                    };

                    serBidding.NotificarConvidado($"Uma nova rodada ({transportadorRotaAntiga.Rodada}) foi proposta para a sua oferta da rota \"{transportadorRotaAntiga.Rota.Descricao}\", de Número: {transportadorRotaAntiga.Rota.BiddingOferta.BiddingConvite.Codigo}.", "Aviso Bidding - Oferta", convidado);

                    if (baselines.Count > 0 && biddingTransportadoresRotaOferta.Count > 0 && transportadorRotaNova.Rodada == 2)
                    {
                        foreach (Dominio.Entidades.Embarcador.Bidding.Baseline baseline in baselines)
                        {
                            if (baseline.Valor > 0)
                                continue;

                            decimal somaValores = 0;

                            foreach (BiddingTransportadorRotaOferta biddingTransportadorRotaOferta in biddingTransportadoresRotaOferta)
                            {
                                somaValores += biddingTransportadorRotaOferta.CustoEstimado;
                            }

                            decimal mediaValorBaseline = somaValores / transportadorRotasAntigas.Count;

                            baseline.Valor = mediaValorBaseline;

                            repositorioBaseline.Atualizar(baseline);
                        }
                    }
                }

                unitOfWork.CommitChanges();

                List<BiddingOfertaRota> listaRotas = repositorioOfertaRota.BuscarRotasPorBidding(biddingConvite);
                List<BiddingTransportadorRota> listaTransportadorRota = repositorioTransportadorRota.BuscarPorBiddingSomenteOfertadas(biddingConvite);
                List<BiddingTransportadorRotaOferta> listaOfertas = repositorioTransportadorRotaOferta.BuscarPorCodigosTransportadorRota(listaTransportadorRota.Select(obj => obj.Codigo).ToList());

                return new JsonpResult(new
                {
                    ListaRotas = (
                    from o in listaRotas
                    select new
                    {
                        o.Codigo,
                        o.Descricao
                    }),
                    ListaModelosVeiculares = ObterModelosVeiculares(listaOfertas)
                });

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao propor novas rodadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AceitarOferta()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorRotaOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);

                dynamic ofertas = JsonConvert.DeserializeObject<dynamic>(Request.Params("Ofertas"));
                int codigo = Request.GetIntParam("Codigo");

                BiddingTransportadorRota transportadorRota = repTransportadorRota.BuscarPorCodigo(codigo, false);
                transportadorRota.Status = StatusBiddingRota.Aprovada;

                foreach (dynamic oferta in ofertas)
                {
                    BiddingTransportadorRotaOferta biddingTransportadorRotaOferta = repTransportadorRotaOferta.BuscarPorCodigo((int)oferta.Codigo, false);
                    biddingTransportadorRotaOferta.Aceito = true;

                    repTransportadorRotaOferta.Atualizar(biddingTransportadorRotaOferta);
                }

                repTransportadorRota.Atualizar(transportadorRota);

                List<BiddingTransportadorRota> listaTransportadorRota = repTransportadorRota.BuscarPorBiddingSomenteOfertadas(transportadorRota.Rota.BiddingOferta.BiddingConvite);
                List<BiddingTransportadorRotaOferta> listaOfertas = repTransportadorRotaOferta.BuscarPorCodigosTransportadorRota(listaTransportadorRota.Select(obj => obj.Codigo).ToList());

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                BiddingConviteConvidado convidado = new BiddingConviteConvidado
                {
                    Convidado = transportadorRota.Transportador
                };

                servicoBidding.NotificarConvidado($"Sua oferta para a rota \"{transportadorRota.Rota.Descricao}\" foi aceita.", "Aviso Bidding - Oferta", convidado);

                return new JsonpResult(new
                {
                    Ofertas = (
                    from o in listaTransportadorRota
                    select new
                    {
                        o.Codigo,
                        Transportador = o.Transportador.NomeFantasia,
                        RotaCodigo = o.Rota.Codigo,
                        RotaDescricao = o.Rota.Descricao,
                        o.Ranking,
                        Situacao = o.Status.ObterDescricao(),
                        SituacaoEnum = o.Status,
                        Rodada = $"{o.Rodada}ª Rodada",
                        Target = o.Target.ToString("n2"),
                        DataRetorno = o.DataRetorno?.ToString("dd/MM/yyyy"),
                        CustoEstimado = (from obj in listaOfertas where obj.TransportadorRota.Codigo == o.Codigo select obj.CustoEstimado).Min().ToString("n2"),
                    }
                    ),
                    ListaModelosVeiculares = ObterModelosVeiculares(listaOfertas)
                });

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ResponderDuvida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                string resposta = Request.GetStringParam("Resposta");

                Repositorio.Embarcador.Bidding.BiddingDuvida repDuvida = new Repositorio.Embarcador.Bidding.BiddingDuvida(unitOfWork);
                BiddingDuvida duvida = repDuvida.BuscarPorCodigo(codigo, false);

                if (duvida == null)
                    throw new Exception("Dúvida nula");

                duvida.Resposta = resposta;
                repDuvida.Atualizar(duvida);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao responder a dúvida.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        public async Task<IActionResult> CompararCargasJaFeitas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int mes = Request.GetIntParam("Mes");
                int ano = Request.GetIntParam("Ano");

                if (ano <= 0)
                    return new JsonpResult(false, "Ano inválido.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);

                BiddingTransportadorRotaOferta entidadeOferta = repTransportadorOferta.BuscarPorCodigo(codigo, false);
                BiddingOfertaRota rota = entidadeOferta.TransportadorRota.Rota;

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new FiltroPesquisaCarga();
                DateTime data = new DateTime(ano, mes, 1);
                filtrosPesquisa.DataInicial = data;
                filtrosPesquisa.DataFinal = data.AddMonths(1).AddDays(-1);

                //todo: validar, ahco que o correto não é buscar por carga, mas buscar por tabela de frete, ou seja, veriifcar se existe uma ou mais tabelas copativeis com a configuração se sim retornar as tabelas e pedir para selcionar na comparação, após isso buscar as cargas no periodo que usaram da tabela para ter esse comparativo.

                if (rota.ClientesOrigem.Count > 0)
                    filtrosPesquisa.CpfCnpjRemetente = rota.ClientesOrigem.FirstOrDefault().CPF_CNPJ;
                else if (rota.Origens.Count > 0)
                    filtrosPesquisa.CodigoOrigem = rota.Origens.FirstOrDefault().Codigo;
                else if (rota.EstadosOrigem.Count > 0)
                    filtrosPesquisa.SiglaEstadoOrigem = rota.EstadosOrigem.FirstOrDefault().Sigla;

                if (rota.ClientesDestino.Count > 0)
                    filtrosPesquisa.CpfCnpjDestinatario = rota.ClientesDestino.FirstOrDefault().CPF_CNPJ;
                else if (rota.Destinos.Count > 0)
                    filtrosPesquisa.CodigoDestino = rota.Destinos.FirstOrDefault().Codigo;
                else if (rota.EstadosDestino.Count > 0)
                    filtrosPesquisa.SiglaEstadoDestino = rota.EstadosDestino.FirstOrDefault().Sigla;

                filtrosPesquisa.Situacoes = new List<SituacaoCarga>();
                filtrosPesquisa.Situacoes.Add(SituacaoCarga.EmTransporte);
                filtrosPesquisa.Situacoes.Add(SituacaoCarga.Encerrada);
                filtrosPesquisa.Situacoes.Add(SituacaoCarga.AgImpressaoDocumentos);
                filtrosPesquisa.Situacoes.Add(SituacaoCarga.LiberadoPagamento);

                filtrosPesquisa.CodigosTipoCarga = (from obj in rota.TiposCarga select obj.Codigo).ToList();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                parametrosConsulta.PropriedadeOrdenar = "Codigo";
                parametrosConsulta.DirecaoOrdenar = "desc";
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.Consultar(filtrosPesquisa, parametrosConsulta);

                decimal valorTotal = cargas.Sum(obj => obj.ValorFreteAPagar - obj.ValorICMS);
                decimal distanciaTotal = cargas.Sum(obj => obj.DadosSumarizados.Distancia);
                decimal pesoTotal = cargas.Sum(obj => obj.DadosSumarizados.PesoTotal);
                decimal valorTotalMercadoria = cargas.Sum(obj => obj.DadosSumarizados.ValorTotalProdutos);
                int quantidadeEntregas = cargas.Sum(obj => obj.DadosSumarizados.NumeroEntregas);
                int volume = cargas.Sum(obj => obj.DadosSumarizados.VolumesTotal);
                int quantidadeViagens = cargas.Count();
                if (quantidadeViagens == 0)
                    quantidadeViagens = 1;

                decimal valorPorViagem = valorTotal / quantidadeViagens;
                string Transportadores = string.Join(",", (from obj in cargas select obj.Empresa?.Descricao ?? "").Distinct().ToList());

                decimal capacidadeVeiculo = entidadeOferta.ModeloVeicular.CapacidadePesoTransporte;
                if (entidadeOferta.ModeloVeicular.CapacidadePesoTransporte == 0)
                    capacidadeVeiculo = 1;

                int quantidadeCargasNecessarias = (int)(pesoTotal / capacidadeVeiculo);

                decimal valorProjetado = 0;

                switch (entidadeOferta.TipoOferta)
                {
                    case TipoLanceBidding.LanceFrotaFixaFranquia:
                        valorProjetado = entidadeOferta.ValorFranquia;
                        break;
                    case TipoLanceBidding.LancePorEquipamento:
                        valorProjetado = (quantidadeViagens * entidadeOferta.ValorFixoEquipamento);
                        break;
                    case TipoLanceBidding.LanceFrotaFixaKmRodado:
                        valorProjetado = entidadeOferta.ValorFixoMensal + (entidadeOferta.ValorKmRodado * distanciaTotal);
                        break;
                    case TipoLanceBidding.LancePorcentagemNota:
                        valorProjetado = (entidadeOferta.ValorFixo / 100) * valorTotalMercadoria;
                        break;
                    case TipoLanceBidding.LanceViagemAdicional:
                        valorProjetado = (quantidadeViagens * entidadeOferta.ValorFixoEquipamento) + (entidadeOferta.ValorEntrega * (quantidadeEntregas * quantidadeCargasNecessarias));
                        break;
                }

                if (quantidadeCargasNecessarias == 0)
                    quantidadeCargasNecessarias = 1;

                decimal valorProjetadoPorCarga = valorProjetado / quantidadeCargasNecessarias;
                decimal diferencaPercentual = 0;
                if (valorTotal > 0)
                    diferencaPercentual = valorProjetado * 100 / valorTotal;

                return new JsonpResult(new
                {
                    Transportador = Transportadores,
                    NumeroViagens = quantidadeViagens,
                    PesoTotal = pesoTotal.ToString("n2"),
                    Volume = volume,
                    ValorTotal = valorTotal.ToString("n2"),
                    ValorMedioPorViagem = valorPorViagem.ToString("n2"),
                    QuantidadeCargaNecessaria = quantidadeCargasNecessarias,
                    ValorTotalProjetado = valorProjetado.ToString("n2"),
                    ValorPorViagemProjetado = valorProjetadoPorCarga.ToString("n2"),
                    Variacao = diferencaPercentual.ToString("n2")
                });

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a comparação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarOfertas(CancellationToken cancellationToken)
        {
            try
            {
                Models.Grid.Grid grid = await ObterGridOfertaAvaliacaoExcel(cancellationToken);
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "Ofertas do Bidding." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os Dados!");
            }
        }

        public async Task<IActionResult> ExportarChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 12, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data Retorno", "DataRetorno", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("% Aceitação Ind.", "Aceitacao", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("% Aceitação Des.", "AceitacaoDesejavel", 10, Models.Grid.Align.left);

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                BiddingConvite entidadeBiddingConvite = repBiddingConvite.BuscarPorCodigo(codigo, false);

                Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador repChecklistTransportador = new Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador(unitOfWork);
                List<BiddingChecklistBiddingTransportador> listaChecklist = repChecklistTransportador.BuscarPorBidding(entidadeBiddingConvite);

                int count = listaChecklist.Count();
                if (count == 0)
                    return new JsonpResult(false, true, "Nenhum registro encontrado!");

                var retorno = (
                    from o in listaChecklist
                    where o.Situacao != StatusBiddingConviteTransportadorRespostas.Reprovado
                    select new
                    {
                        o.Codigo,
                        Transportador = o.Transportador.NomeFantasia,
                        Aceitacao = o.Aceitamento,
                        AceitacaoDesejavel = o.AceitamentoDesejavel,
                        Situacao = o.Situacao.ObterDescricao(),
                        DataRetorno = o.DataRetorno.ToString("dd/MM/yyyy")
                    }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(count);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "Checklist do Bidding." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os Dados!");
            }
        }

        public async Task<IActionResult> FecharBidding()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                BiddingConviteConvidado convidado = repConviteConvidado.BuscarPorConvite(codigo);

                convidado.StatusBidding = StatusBiddingConvite.Fechamento;
                repConviteConvidado.Atualizar(convidado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao fechar o bidding.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisarAvaliacaoOfertasTransportadores(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridOfertaAvaliacao(false, cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisarAvaliacaoOfertasComponente(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(ObterGridOfertasComponente(false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisarRankingCherryPickingOfertas(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridRankingOfertas(TipoRankingBidding.GridRankingCherryPickingOferas, cancellationToken, true));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisarRankingOfertas(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridRankingOfertas(TipoRankingBidding.GridRankingOfertas, cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> FinalizarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingChecklist repBiddingChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOferta repBiddingOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);

                BiddingConvite biddingConvite = repBiddingConvite.BuscarPorCodigo(codigo, false);
                List<BiddingConviteConvidado> convidados = repConviteConvidado.BuscarConvidadosConfirmados(biddingConvite);
                BiddingChecklist checklist = repBiddingChecklist.BuscarChecklist(biddingConvite);
                StatusBiddingConvite situacaoOriginal = biddingConvite.Status;

                biddingConvite.Status = biddingConvite.Status.ObterProximo();

                if (biddingConvite.Status == StatusBiddingConvite.Checklist && checklist.TipoPreenchimentoChecklist == TipoPreenchimentoChecklist.PreenchimentoDesabilitado)
                    biddingConvite.Status = biddingConvite.Status.ObterProximo();

                foreach (BiddingConviteConvidado convidado in convidados)
                {
                    while (convidado.StatusBidding != biddingConvite.Status)
                    {
                        convidado.StatusBidding = convidado.StatusBidding.ObterProximo();

                        if (convidado.StatusBidding == StatusBiddingConvite.Checklist &&
                            checklist.TipoPreenchimentoChecklist == TipoPreenchimentoChecklist.PreenchimentoDesabilitado)
                        {
                            convidado.StatusBidding = convidado.StatusBidding.ObterProximo();
                        }
                    }

                    repConviteConvidado.Atualizar(convidado);
                }

                repBiddingConvite.Atualizar(biddingConvite);

                if (situacaoOriginal == StatusBiddingConvite.Ofertas)
                {
                    List<int> codigosVencedores = new List<int>();

                    dynamic listaVencedores = JsonConvert.DeserializeObject<dynamic>(Request.Params("VencedoresDefinidos"));

                    AprovarTransportadoresVencedores(listaVencedores, codigosVencedores, biddingConvite.Codigo, unitOfWork);

                    List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> listaTransportadoresRejeitados = repTransportadorRota.BuscarPorBiddingRejeitados(repBiddingOferta.BuscarOferta(biddingConvite), codigosVencedores);

                    RejeitarTransportadoresRestantes(listaTransportadoresRejeitados, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException e)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, e.Message);
            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa do bidding.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> NotificarInteressados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConvite = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Bidding.BiddingConvite repBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite repAprovacaoBiddingConvite = new Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite(unitOfWork);
                Servicos.Embarcador.Bidding.Bidding serBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                BiddingConvite biddingConvite = repBiddingConvite.BuscarPorCodigo(codigoConvite, false);

                List<string> emailsUsuariosInteressados = repAprovacaoBiddingConvite.BuscarEmailsAprovadoresPorBiddingConvite(codigoConvite);

                if (emailsUsuariosInteressados == null || !emailsUsuariosInteressados.Any())
                    return new JsonpResult(false, true, "Não possuem e-mails configurados entre os Usuários interessados");

                if (biddingConvite.Status != StatusBiddingConvite.Fechamento)
                    serBidding.NotificarInteressadosOferta(biddingConvite, emailsUsuariosInteressados, ClienteAcesso.URLAcesso);
                else
                    serBidding.NotificarInteressadosFechamento(biddingConvite, emailsUsuariosInteressados);

                return new JsonpResult(true);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao notificar os interessados do Bidding.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FiltrarOpcoesFiltrosPorRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> listaRotas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados>>(Request.GetStringParam("Filtros"));
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas = ObterFiltroPesquisaBiddingOfertas();
                Servicos.Embarcador.Bidding.Bidding servicoBidding = new Servicos.Embarcador.Bidding.Bidding(unitOfWork);

                listaRotas = FiltrarRotasOpcoesFiltro(listaRotas, filtroPesquisaBiddingOfertas);

                return new JsonpResult(new
                {
                    Filtros = servicoBidding.ProcessarListaRotas(listaRotas)
                });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as opções de filtros.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterDadosParaTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");
                int codigoBidding = Request.GetIntParam("CodigoBidding");

                string codigosRotas = null;
                string transportadores = null;
                string tipoOferta = null;
                string modelosVeicularesTracao = null;
                string modelosVeicularesReboque = null;
                string tiposCarga = null;
                string filiaisParticipantes = null;
                string faixaEntrega = null;
                string faixaPeso = null;
                string faixaAjudante = null;

                List<int> codigosTransportadores = new List<int>();
                List<int> codigosModelosVeiculares = new List<int>();
                List<int> codigosTiposCarga = new List<int>();
                List<int> codigosFiliaisParticipantes = new List<int>();
                List<int> valoresFaixaEntrega = new List<int>();
                List<decimal> valoresFaixaPeso = new List<decimal>();
                List<int> valoresFaixaAjudante = new List<int>();

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOfertaRota repositorioBiddingOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                BiddingConvite biddingSelecionado = repositorioBiddingConvite.BuscarPorCodigo(codigoBidding, false);

                if (codigos.IsNullOrEmpty())
                    return new JsonpResult(false, true, "Nenhuma rota selecionada para criar uma tabela de frete");

                foreach (int codigo in codigos)
                {
                    codigosRotas += codigo.ToString() + "-";
                    BiddingTransportadorRotaOferta rotaSelecionada = repositorioTransportadorOferta.BuscarPorCodigo(codigo, false);
                    BiddingOfertaRota biddingOfertaRota = repositorioBiddingOfertaRota.BuscarPorCodigo(rotaSelecionada.TransportadorRota.Rota.Codigo, false);

                    if (rotaSelecionada.NaoOfertar != null && (bool)rotaSelecionada.NaoOfertar)
                        return new JsonpResult(false, true, "Uma ou mais rotas selecionadas não foram ofertadas");

                    tipoOferta = ((int)rotaSelecionada.TipoOferta).ToString();

                    if (rotaSelecionada.TransportadorRota.Transportador != null)
                    {
                        if (!codigosTransportadores.Any(id => id == rotaSelecionada.TransportadorRota.Transportador.Codigo))
                        {
                            transportadores += rotaSelecionada.TransportadorRota.Transportador.Codigo + "-" + rotaSelecionada.TransportadorRota.Transportador.RazaoSocial + "_";
                            codigosTransportadores.Add(rotaSelecionada.TransportadorRota.Transportador.Codigo);
                        }
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular in biddingOfertaRota.ModelosVeiculares)
                    {
                        if (codigosModelosVeiculares.Any(id => id == modeloVeicular.Codigo))
                            continue;

                        codigosModelosVeiculares.Add(modeloVeicular.Codigo);

                        if (modeloVeicular.Tipo == TipoModeloVeicularCarga.Reboque)
                            modelosVeicularesReboque += modeloVeicular.Codigo + "-" + modeloVeicular.Descricao + "_";
                        else
                            modelosVeicularesTracao += modeloVeicular.Codigo + "-" + modeloVeicular.Descricao + "_";
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in biddingOfertaRota.TiposCarga)
                    {
                        if (codigosTiposCarga.Any(id => id == tipoCarga.Codigo))
                            continue;
                        codigosTiposCarga.Add(tipoCarga.Codigo);
                        tiposCarga += tipoCarga.Codigo + "-" + tipoCarga.Descricao + "_";
                    }

                    foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in biddingOfertaRota.FiliaisParticipante)
                    {
                        if (codigosFiliaisParticipantes.Any(id => id == filial.Codigo))
                            continue;
                        codigosFiliaisParticipantes.Add(filial.Codigo);
                        filiaisParticipantes += filial.Codigo + "-" + filial.Descricao + "_";
                    }

                    if (valoresFaixaEntrega.Any(valor => valor == rotaSelecionada.TransportadorRota.Rota.NumeroEntrega))
                        continue;
                    valoresFaixaEntrega.Add(rotaSelecionada.TransportadorRota.Rota.NumeroEntrega);
                    faixaEntrega += rotaSelecionada.TransportadorRota.Rota.NumeroEntrega + "-" + rotaSelecionada.TransportadorRota.Rota.QuantidadeAjudantePorVeiculo + "_";

                    if (valoresFaixaPeso.Any(valor => valor == rotaSelecionada.TransportadorRota.Rota.Peso))
                        continue;
                    valoresFaixaPeso.Add(rotaSelecionada.TransportadorRota.Rota.Peso);
                    faixaPeso += rotaSelecionada.TransportadorRota.Rota.Peso.ToString() + "-" + rotaSelecionada.TransportadorRota.Rota.QuantidadeAjudantePorVeiculo + "_";

                    if (valoresFaixaAjudante.Any(valor => valor == rotaSelecionada.TransportadorRota.Rota.FrequenciaMensalComAjudante))
                        continue;
                    valoresFaixaAjudante.Add(rotaSelecionada.TransportadorRota.Rota.FrequenciaMensalComAjudante);
                    faixaAjudante += rotaSelecionada.TransportadorRota.Rota.FrequenciaMensalComAjudante + "-" + rotaSelecionada.TransportadorRota.Rota.QuantidadeAjudantePorVeiculo + "_";
                }

                var retorno = new
                {
                    Rotas = codigosRotas,
                    Descricao = "Bidding " + biddingSelecionado.Descricao,
                    TipoBidding = biddingSelecionado.TipoBidding.Descricao,
                    Transportador = transportadores,
                    //InicioVigencia = biddingSelecionado?.DataInicioVigencia.ToString() ?? null,
                    //FimVigencia = biddingSelecionado?.DataFimVigencia.ToString() ?? null,
                    InicioVigencia = biddingSelecionado?.DataInicio.ToString() ?? null,
                    FimVigencia = biddingSelecionado?.DataLimite.ToString() ?? null,
                    Formato = tipoOferta,
                    ModeloVeicularTracao = modelosVeicularesTracao,
                    ModeloVeicularReboque = modelosVeicularesReboque,
                    TipoCarga = tiposCarga,
                    FiliaisParticipantes = filiaisParticipantes,
                    FaixaEntrega = faixaEntrega,
                    FaixaPeso = faixaPeso,
                    FaixaAjudante = faixaAjudante
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao buscar os dados para criar tabela de frete");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosParaTabelaFreteCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoBidding = Request.GetIntParam("CodigoBidding");

                string origens = null;
                string destinos = null;
                string clienteOrigens = null;
                string clienteDestinos = null;
                string estadoOrigens = null;
                string estadoDestinos = null;
                string regiaoOrigens = null;
                string regiaoDestinos = null;
                string rotasOrigens = null;
                string rotasDestinos = null;
                string cepOrigens = null;
                string cepDestinos = null;
                string paisOrigens = null;
                string paisDestinos = null;
                string ofertasSemAjudante = null;
                string ofertasComAjudante = null;

                List<int> codigosOrigens = new List<int>();
                List<int> codigosDestinos = new List<int>();
                List<long> codigosClienteOrigens = new List<long>();
                List<long> codigosClienteDestinos = new List<long>();
                List<int> codigosEstadoOrigens = new List<int>();
                List<int> codigosEstadoDestinos = new List<int>();
                List<int> codigosRegiaoOrigens = new List<int>();
                List<int> codigosRegiaoDestinos = new List<int>();
                List<int> codigosRotasOrigens = new List<int>();
                List<int> codigosRotasDestinos = new List<int>();
                List<int> codigosCepOrigens = new List<int>();
                List<int> codigosCepDestinos = new List<int>();
                List<int> codigosPaisOrigens = new List<int>();
                List<int> codigosPaisDestinos = new List<int>();

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOfertaRota repositorioBiddingOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

                BiddingConvite biddingSelecionado = repositorioBiddingConvite.BuscarPorCodigo(codigoBidding, false);
                BiddingTransportadorRotaOferta rotaSelecionada = repositorioTransportadorOferta.BuscarPorCodigo(codigo, false);
                BiddingOfertaRota biddingOfertaRota = repositorioBiddingOfertaRota.BuscarPorCodigo(rotaSelecionada.TransportadorRota.Rota.Codigo, false);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFretePai = repositorioTabelaFrete.BuscarPorCodigo(rotaSelecionada.CodigoTabelaFretePai, false);

                if (tabelaFretePai == null)
                    return new JsonpResult(false, true, "Erro ao buscar a tabela de frete pai");

                foreach (Dominio.Entidades.Localidade origem in biddingOfertaRota.Origens)
                {
                    if (codigosOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosOrigens.Add(origem.Codigo);
                    origens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (Dominio.Entidades.Localidade destino in biddingOfertaRota.Destinos)
                {
                    if (codigosDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosDestinos.Add(destino.Codigo);
                    destinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

                foreach (Dominio.Entidades.Cliente origem in biddingOfertaRota.ClientesOrigem)
                {
                    if (codigosClienteOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosClienteOrigens.Add(origem.Codigo);
                    clienteOrigens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (Dominio.Entidades.Cliente destino in biddingOfertaRota.ClientesDestino)
                {
                    if (codigosClienteDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosClienteDestinos.Add(destino.Codigo);
                    clienteDestinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

                foreach (Dominio.Entidades.Estado origem in biddingOfertaRota.EstadosOrigem)
                {
                    if (codigosEstadoOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosEstadoOrigens.Add(origem.Codigo);
                    estadoOrigens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (Dominio.Entidades.Estado destino in biddingOfertaRota.EstadosDestino)
                {
                    if (codigosEstadoDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosEstadoDestinos.Add(destino.Codigo);
                    estadoDestinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao origem in biddingOfertaRota.RegioesOrigem)
                {
                    if (codigosRegiaoOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosRegiaoOrigens.Add(origem.Codigo);
                    regiaoOrigens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao destino in biddingOfertaRota.RegioesDestino)
                {
                    if (codigosRegiaoDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosRegiaoDestinos.Add(destino.Codigo);
                    regiaoDestinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

                foreach (Dominio.Entidades.RotaFrete origem in biddingOfertaRota.RotasOrigem)
                {
                    if (codigosRotasOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosRotasOrigens.Add(origem.Codigo);
                    rotasOrigens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (Dominio.Entidades.RotaFrete destino in biddingOfertaRota.RotasDestino)
                {
                    if (codigosRotasDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosRotasDestinos.Add(destino.Codigo);
                    rotasDestinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

                foreach (BiddingOfertaRotaCEPOrigem origem in biddingOfertaRota.CEPsOrigem)
                {
                    if (codigosCepOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosCepOrigens.Add(origem.Codigo);
                    cepOrigens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (BiddingOfertaRotaCEPDestino destino in biddingOfertaRota.CEPsDestino)
                {
                    if (codigosCepDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosCepDestinos.Add(destino.Codigo);
                    cepDestinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

                foreach (Dominio.Entidades.Pais origem in biddingOfertaRota.PaisesOrigem)
                {
                    if (codigosPaisOrigens.Any(id => id == origem.Codigo))
                        continue;
                    codigosPaisOrigens.Add(origem.Codigo);
                    paisOrigens += origem.Codigo + "-" + origem.Descricao + "_";
                }

                foreach (Dominio.Entidades.Pais destino in biddingOfertaRota.PaisesDestino)
                {
                    if (codigosPaisDestinos.Any(id => id == destino.Codigo))
                        continue;
                    codigosPaisDestinos.Add(destino.Codigo);
                    paisDestinos += destino.Codigo + "-" + destino.Descricao + "_";
                }

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
                decimal adicionalFracionadoComICMS = 0;
                decimal adicionalAjudanteComICMS = 0;
                decimal adicionalFracionadoAjudanteComICMS = 0;

                if (rotaSelecionada.TipoOferta == TipoLanceBidding.LancePorPeso)
                {
                    freteComICMSPeso = (rotaSelecionada.FreteTonelada / (1 - (rotaSelecionada.ICMSPorcentagem / 100)));
                    pedagioComICMSPeso = ((rotaSelecionada.PedagioParaEixo / (1 - (rotaSelecionada.ICMSPorcentagem / 100))) * rotaSelecionada.ModeloVeicular.NumeroEixos) ?? 0;
                    totalBrutoPeso = pedagioComICMSPeso > 0 ? freteComICMSPeso + (pedagioComICMSPeso / (rotaSelecionada.ModeloVeicular.CapacidadePesoTransporte / 1000)) : freteComICMSPeso;
                    totalLiquidoPeso = Math.Round((totalBrutoPeso * (1 - (rotaSelecionada.ICMSPorcentagem / 100)) * valorCalculoTotalLiquido), 2);
                }

                if (rotaSelecionada.TipoOferta == TipoLanceBidding.LancePorCapacidade)
                {
                    freteComICMSCapacidadeTon = (rotaSelecionada.FreteTonelada / (1 - (rotaSelecionada.ICMSPorcentagem / 100)));
                    freteComICMSCapacidade = freteComICMSCapacidadeTon * (rotaSelecionada.ModeloVeicular.CapacidadePesoTransporte / 1000);
                    pedagioComICMSCapacidade = ((rotaSelecionada.PedagioParaEixo / (1 - (rotaSelecionada.ICMSPorcentagem / 100))) * rotaSelecionada.ModeloVeicular.NumeroEixos) ?? 0;
                    totalBrutoCapacidade = pedagioComICMSCapacidade > 0 ? (freteComICMSCapacidade + pedagioComICMSCapacidade) : freteComICMSCapacidade;
                    totalLiquidoCapacidade = Math.Round((totalBrutoCapacidade * (1 - (rotaSelecionada.ICMSPorcentagem / 100)) * valorCalculoTotalLiquido), 2);
                }

                if (rotaSelecionada.TipoOferta == TipoLanceBidding.LancePorFreteViagem)
                {
                    totalBrutoViagem = (rotaSelecionada.FreteTonelada / (1 - (rotaSelecionada.ICMSPorcentagem / 100))) + (rotaSelecionada.PedagioParaEixo / (1 - (rotaSelecionada.ICMSPorcentagem / 100)));
                    totalLiquidoViagem = Math.Round((totalBrutoViagem * (1 - (rotaSelecionada.ICMSPorcentagem / 100)) * valorCalculoTotalLiquido), 2);
                }

                if (rotaSelecionada.TipoOferta == TipoLanceBidding.LancePorViagemEntregaAjudante)
                {
                    viagemComPedagio = (rotaSelecionada.FreteTonelada + rotaSelecionada.PedagioParaEixo) / (1 - (rotaSelecionada.ICMSPorcentagem / 100));
                    adicionalAjudanteComICMS = (rotaSelecionada.FreteTonelada + rotaSelecionada.PedagioParaEixo + rotaSelecionada.Ajudante) / (1 - (rotaSelecionada.ICMSPorcentagem / 100));
                    adicionalFracionadoComICMS = (rotaSelecionada.FreteTonelada + rotaSelecionada.PedagioParaEixo + (rotaSelecionada.AdicionalPorEntrega * (biddingOfertaRota.MediaEntregasFracionada - 1))) / (1 - (rotaSelecionada.ICMSPorcentagem / 100));
                    adicionalFracionadoAjudanteComICMS = (rotaSelecionada.FreteTonelada + rotaSelecionada.PedagioParaEixo + rotaSelecionada.Ajudante + (rotaSelecionada.AdicionalPorEntrega * (biddingOfertaRota.MediaEntregasFracionada - 1))) / (1 - (rotaSelecionada.ICMSPorcentagem / 100));
                }

                decimal valorSemAjudanteAnterior = 0;
                decimal valorComAjudanteAnterior = 0;
                for (int i = 0; i < 15; i++)
                {
                    valorSemAjudanteAnterior = (valorSemAjudanteAnterior + (rotaSelecionada.FreteTonelada + rotaSelecionada.PedagioParaEixo) / (1 - (rotaSelecionada.ICMSPorcentagem / 100)));
                    ofertasSemAjudante += valorSemAjudanteAnterior.ToString() + "-";
                    valorComAjudanteAnterior = (valorComAjudanteAnterior + (rotaSelecionada.FreteTonelada + rotaSelecionada.PedagioParaEixo + rotaSelecionada.Ajudante) / (1 - (rotaSelecionada.ICMSPorcentagem / 100)));
                    ofertasComAjudante += valorComAjudanteAnterior.ToString() + "-";
                }

                var retorno = new
                {
                    TabelaFrete = tabelaFretePai.Codigo + "-" + tabelaFretePai.Descricao,
                    TipoBidding = biddingSelecionado.TipoBidding.Descricao,
                    TipoOferta = ((int)rotaSelecionada.TipoOferta).ToString(),
                    tabelaFretePai.ParametroBase,
                    Origem = origens,
                    Destino = destinos,
                    ClienteOrigem = clienteOrigens,
                    ClienteDestino = clienteDestinos,
                    EstadoOrigem = estadoOrigens,
                    EstadoDestino = estadoDestinos,
                    RegiaoOrigem = regiaoOrigens,
                    RegiaoDestino = regiaoDestinos,
                    RotasOrigem = rotasOrigens,
                    RotasDestino = rotasDestinos,
                    CepOrigem = cepOrigens,
                    CepDestino = cepDestinos,
                    PaisOrigem = paisOrigens,
                    PaisDestino = paisDestinos,
                    OfertasSemAjudante = ofertasSemAjudante,
                    OfertasComAjudantes = ofertasComAjudante,
                    Oferta = rotaSelecionada.ModeloVeicular.Descricao + "-" +
                             rotaSelecionada.ValorFixo.ToString("n2") + "-" +
                             rotaSelecionada.ValorFixoEquipamento.ToString("n2") + "-" +
                             rotaSelecionada.Porcentagem.ToString("n2") + "-" +
                             rotaSelecionada.ValorFixoMensal.ToString("n2") + "-" +
                             rotaSelecionada.ValorKmRodado.ToString("n2") + "-" +
                             rotaSelecionada.ValorFranquia.ToString("n2") + "-" +
                             rotaSelecionada.ValorViagem.ToString("n2") + "-" +
                             rotaSelecionada.ValorEntrega.ToString("n2") + "-" +
                             totalLiquidoPeso.ToString("n2") + "-" +
                             totalLiquidoCapacidade.ToString("n2") + "-" +
                             totalLiquidoViagem.ToString("n2") + "-" +
                             viagemComPedagio.ToString("n2") + "-" +
                             adicionalAjudanteComICMS.ToString("n2") + ""
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao buscar os dados para criar tabela de frete cliente");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterConvite(BiddingConvite biddingConvite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioConviteConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(unitOfWork);
            List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> listaConvites = repositorioConviteConvidado.BuscarConvidados(biddingConvite);

            return (
                    from o in listaConvites
                    select new
                    {
                        o.Codigo,
                        Transportador = o.Convidado.NomeFantasia + $" ({o.Convidado.CNPJ_Formatado})",
                        Situacao = o.Status.ObterDescricao(),
                        DataRetorno = o.DataRetorno?.ToString("dd/MM/yyyy")
                    });

        }

        private dynamic ObterChecklist(BiddingConvite biddingConvite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador repositorioChecklistTransportador = new Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador> listaChecklist = repositorioChecklistTransportador.BuscarPorBidding(biddingConvite);

            return (
                from o in listaChecklist
                where o.Situacao != StatusBiddingConviteTransportadorRespostas.Reprovado
                select new
                {
                    o.Codigo,
                    Transportador = o.Transportador.NomeFantasia,
                    Aceitacao = o.Aceitamento,
                    AceitacaoDesejavel = o.AceitamentoDesejavel,
                    Situacao = o.Situacao.ObterDescricao(),
                    DataRetorno = o.DataRetorno.ToString("dd/MM/yyyy")
                });
        }

        private TipoPreenchimentoChecklist ObterTipoPreenchimentoChecklist(BiddingConvite biddingConvite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingChecklist repositorioChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(unitOfWork);
            Dominio.Entidades.Embarcador.Bidding.BiddingChecklist checklist = repositorioChecklist.BuscarChecklist(biddingConvite);
            return checklist.TipoPreenchimentoChecklist ?? TipoPreenchimentoChecklist.PreenchimentoObrigatorio;
        }

        private Models.Grid.Grid ObterGridPesquisaResultados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Vencedor", "Vencedor", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Rota", "Rota", 20, Models.Grid.Align.left, true);
				    grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, true);
				    grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Modelo Veícular", "ModeloVeicular", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Oferta", "Oferta", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Valor Oferta", "CustoEstimado", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo da oferta", "TipoOferta", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Rodada", "Rodada", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Titularidade", "Titularidade", 5, Models.Grid.Align.left, false);

            int codigo = Request.GetIntParam("Codigo");
            int codigoRota = Request.GetIntParam("Rotas");

            Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
            List<BiddingTransportadorRotaOferta> listaVencedores = repTransportadorOferta.BuscarVencedores(codigo, codigoRota);

            int totalRegistros = listaVencedores.Count();

            var retorno = (from o in listaVencedores
                            select new
                            {
                                o.Codigo,
                                Vencedor = o.TransportadorRota.Transportador.NomeFantasia,
                                Rota = o.TransportadorRota.Rota.Descricao,
								Origem = o.TransportadorRota.Rota.DescricaoOrigem,
                                Destino = o.TransportadorRota.Rota.DescricaoDestino,
								RotaCodigo = o.TransportadorRota.Rota.Codigo,
                                ModeloVeicular = o.ModeloVeicular.Descricao,
                                Oferta = o.DescricaoOferta,
								o.CustoEstimado,
                                TipoOferta = o.DescricaoTipoOferta,
                                o.TransportadorRota.Rodada,
                                Titularidade = o.TipoTransportador.HasValue ? o.TipoTransportador.Value.ObterDescricao() : "",
                                DT_RowColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde,
                            }).ToList();

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private async Task<Models.Grid.Grid> ObterGridOfertaAvaliacaoExcel(CancellationToken cancellationToken)
        {

            Models.Grid.Grid gridOfertas = await ObterGridOfertaAvaliacao(true, cancellationToken);
            Models.Grid.Grid gridComponente = ObterGridOfertasComponente(true);
            Models.Grid.Grid gridRanking = await ObterGridRankingOfertas(TipoRankingBidding.GridRankingOfertas, cancellationToken, false, true, gridOfertas);
            Models.Grid.Grid gridRankingCherryPicking = await ObterGridRankingOfertas(TipoRankingBidding.GridRankingCherryPickingOferas, cancellationToken, true, true, gridOfertas);

            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> gridOfertasRegistros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>>(gridOfertas.ObterDataRows().ToJson());
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> gridComponenteRegistros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>>(gridComponente.ObterDataRows().ToJson());
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta> gridRankingRegistros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta>>(gridRanking.ObterDataRows().ToJson());
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta> gridRankingCherryPickingRegistros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta>>(gridRankingCherryPicking.ObterDataRows().ToJson());

            gridComponente.header.ForEach(x =>
            {
                x.name += "Componente";
                x.data += "Componente";
            });

            gridRanking.header.ForEach(x =>
            {
                x.name += "Ranking";
                x.data += "Ranking";
            });

            gridRankingCherryPicking.header.ForEach(x =>
            {
                x.name += "CherryPicking";
                x.data += "CherryPicking";
            });

            Models.Grid.Grid grid;

            if (gridOfertasRegistros != null && gridOfertasRegistros.FirstOrDefault().tipoLanceBidding == TipoLanceBidding.LancePorViagemEntregaAjudante)
            {
                grid = new Models.Grid.Grid
                {
                    header = gridOfertas.header.Where(x => x.visible)
                .Concat(gridComponente.header.Where(x => x.visible))
                .Concat(gridRanking.header.Where(x => x.visible))
                .Concat(gridRankingCherryPicking.header.Where(x => x.visible))
                .ToList()
                };
            }
            else
            {
                grid = new Models.Grid.Grid
                {
                    header = gridOfertas.header.Where(x => x.visible)
                .Concat(gridComponente.header.Where(x => x.visible))
                .ToList()
                };
            }

            List<dynamic> dados = new List<dynamic>(gridOfertasRegistros.Count);

            foreach (Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta item in gridOfertasRegistros)
            {
                Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta? registroComponenteDinamico = gridComponenteRegistros.Find(x => x.Codigo == item.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta? registroRankingDinamico = gridRankingRegistros.Find(x => x.Codigo == item.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta? registroRankingChecerryPickingRegistroDinamico = gridRankingCherryPickingRegistros.Find(x => x.Codigo == item.Codigo);

                dynamic registro = new ExpandoObject();

                registro.Codigo = item.Codigo;
                registro.RotaDescricao = item.RotaDescricao;
                registro.Origem = item.Origem;
                registro.Destino = item.Destino;
                registro.QuantidadeViagensAno = item.QuantidadeViagensAno;
                registro.VolumeTonAno = item.VolumeTonAno;
                registro.ColunaDinamicaBaseLine1 = item.ColunaDinamicaBaseLine1;
                registro.QuantidadeEntregas = item.QuantidadeEntregas;
                registro.QuantidadeAjudantes = item.QuantidadeAjudantes;

                List<int> codigosTrasportadores = gridOfertas.header.Select(x => x.dynamicCode).Where(x => x > 0).ToList();

                foreach (int codigoTrasportador in codigosTrasportadores)
                {
                    ((IDictionary<string, object>)registro).Add($"ColunaTransportador{codigoTrasportador}", item.ColunasTrasportador.Find(x => x.CodigoTrasportador == codigoTrasportador)?.Valor);
                }

                registro.Target = item.Target;
                registro.Rodada = item.Rodada;
                registro.Transportador = item.Transportador;
                registro.MenorValor = item.MenorValor;
                registro.ImpactoReais = item.ImpactoReais != null ? item.ImpactoReais : string.Empty;
                registro.ImpactoPercentual = item.ImpactoPercentual ?? string.Empty;

                registro.RotaDescricaoComponente = item.RotaDescricao ?? string.Empty;
                registro.OrigemComponente = item.Origem ?? string.Empty;
                registro.DestinoComponente = item.Destino ?? string.Empty;
                registro.ValorFreteComponente = registroComponenteDinamico?.ValorFrete ?? string.Empty;
                registro.TransportadorValorFreteComponente = registroComponenteDinamico?.TransportadorValorFrete ?? string.Empty;
                registro.AdicionalPorEntregaComponente = registroComponenteDinamico?.AdicionalPorEntrega ?? string.Empty;
                registro.TransportadorAdicionalPorEntregaComponente = registroComponenteDinamico?.TransportadorAdicionalPorEntrega ?? string.Empty;
                registro.AjudanteComponente = registroComponenteDinamico?.Ajudante ?? string.Empty;
                registro.TransportadorAjudanteComponente = registroComponenteDinamico?.TransportadorAjudante ?? string.Empty;
                registro.TransportadorPedagioComponente = registroComponenteDinamico?.TransportadorPedagio ?? string.Empty;
                registro.PedagioComponente = registroComponenteDinamico?.Pedagio ?? string.Empty;
                registro.TotalComponente = registroComponenteDinamico?.Total ?? string.Empty;
                registro.AliquotaICMSComponente = registroComponenteDinamico?.AliquotaICMS;
                registro.VeiculosVerdesComponente = registroComponenteDinamico?.VeiculosVerdes;

                registro.RotaDescricaoRanking = item.RotaDescricao ?? string.Empty;
                registro.OrigemRanking = item.Origem ?? string.Empty;
                registro.DestinoRanking = item.Destino ?? string.Empty;
                registro.PrimeiroTransportadorValorRanking = registroRankingDinamico?.PrimeiroTransportadorValor ?? string.Empty;
                registro.PrimeiroTransportadorDescricaoRanking = registroRankingDinamico?.PrimeiroTransportadorDescricao ?? string.Empty;
                registro.PrimeiroBaselineRanking = registroRankingDinamico?.PrimeiroBaseline ?? string.Empty;
                registro.SegundoTransportadorValorRanking = registroRankingDinamico?.SegundoTransportadorValor ?? string.Empty;
                registro.SegundoTransportadorDescricaoRanking = registroRankingDinamico?.SegundoTransportadorDescricao ?? string.Empty;
                registro.SegundoBaselineRanking = registroRankingDinamico?.SegundoBaseline ?? string.Empty;
                registro.TerceiroTransportadorValorRanking = registroRankingDinamico?.TerceiroTransportadorDescricao ?? string.Empty;
                registro.TerceiroTransportadorDescricaoRanking = registroRankingDinamico?.TerceiroTransportadorDescricao ?? string.Empty;
                registro.TerceiroBaselineRanking = registroRankingDinamico?.TerceiroBaseline ?? string.Empty;
                registro.ValorSimuladoRanking = registroRankingDinamico?.ValorSimulado ?? string.Empty;
                registro.TransportadorDescricaoSimuladoRanking = registroRankingDinamico?.TransportadorDescricaoSimulado ?? string.Empty;
                registro.BaselineSimuladoRanking = registroRankingDinamico?.BaselineSimulado ?? string.Empty;

                registro.RotaDescricaoCherryPicking = item.RotaDescricao ?? string.Empty;
                registro.OrigemCherryPicking = item.Origem ?? string.Empty;
                registro.DestinoCherryPicking = item.Destino ?? string.Empty;
                registro.PrimeiroTransportadorValorCherryPicking = registroRankingChecerryPickingRegistroDinamico?.PrimeiroTransportadorValor ?? string.Empty;
                registro.PrimeiroTransportadorDescricaoCherryPicking = registroRankingChecerryPickingRegistroDinamico?.PrimeiroTransportadorDescricao ?? string.Empty;
                registro.PrimeiroBaselineCherryPicking = registroRankingChecerryPickingRegistroDinamico?.PrimeiroBaseline ?? string.Empty;
                registro.SegundoTransportadorValorCherryPicking = registroRankingChecerryPickingRegistroDinamico?.SegundoTransportadorValor ?? string.Empty;
                registro.SegundoTransportadorDescricaoCherryPicking = registroRankingChecerryPickingRegistroDinamico?.SegundoTransportadorDescricao ?? string.Empty;
                registro.SegundoBaselineCherryPicking = registroRankingChecerryPickingRegistroDinamico?.SegundoBaseline ?? string.Empty;
                registro.TerceiroTransportadorValorCherryPicking = registroRankingChecerryPickingRegistroDinamico?.TerceiroTransportadorDescricao ?? string.Empty;
                registro.TerceiroTransportadorDescricaoCherryPicking = registroRankingChecerryPickingRegistroDinamico?.TerceiroTransportadorDescricao ?? string.Empty;
                registro.TerceiroBaselineCherryPicking = registroRankingChecerryPickingRegistroDinamico?.TerceiroBaseline ?? string.Empty;
                registro.ValorSimuladoCherryPicking = registroRankingChecerryPickingRegistroDinamico?.ValorSimulado ?? string.Empty;
                registro.TransportadorDescricaoSimuladoCherryPicking = registroRankingChecerryPickingRegistroDinamico?.TransportadorDescricaoSimulado ?? string.Empty;
                registro.BaselineSimuladoCherryPicking = registroRankingChecerryPickingRegistroDinamico?.BaselineSimulado ?? string.Empty;


                dados.Add(registro);
            }

            grid.AdicionaRows(dados);
            grid.setarQuantidadeTotal(dados.Count);

            return grid;
        }

        private async Task<Models.Grid.Grid> ObterGridOfertaAvaliacao(bool exportacao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Bidding.Baseline repositorioBaseline = new Repositorio.Embarcador.Bidding.Baseline(unitOfWork, cancellationToken);

            string menorValorTransportador = string.Empty;
            string segundoMenorValorTransportador = string.Empty;
            string terceiroMenorValorTransportador = string.Empty;
            string baselineValor = string.Empty;
            string impactoPercentualFormatado = string.Empty;
            decimal impactoReais = 0;
            decimal segundoMenorValor = 0;
            decimal terceiroMenorValor = 0;
            string primeiroTransportador = null;
            string segundoTransportador = null;
            string terceiroTransportador = null;
            int maximoColunaBaseline = 10;
            string regiaoOrigem = string.Empty;
            string regiaoDestino = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas = ObterFiltroPesquisaBiddingOfertas();


            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

            Models.Grid.Grid grid = PreencherGridParametrosConsulta(exportacao, ref parametroConsulta);
            // REVER ISSO URGENTEMENTE
            int totalRegistros = await repTransportadorRota.ContarPorBiddingRotaOfertasAsync(filtroPesquisaBiddingOfertas);

            List<BiddingOfertaRota> rotas = totalRegistros > 0 ? await repTransportadorRota.BuscarPorBiddingRotaOfertasAsync(filtroPesquisaBiddingOfertas, parametroConsulta, false, true) : new List<BiddingOfertaRota>();
            List<Dominio.Entidades.Embarcador.Bidding.Baseline> baselines = totalRegistros > 0 ? await repositorioBaseline.BuscarPorBiddingConviteERotaAsync(filtroPesquisaBiddingOfertas) : new List<Dominio.Entidades.Embarcador.Bidding.Baseline>();
            List<BiddingTransportadorRota> listaTransportadorRota = totalRegistros > 0 ? await repTransportadorRota.BuscarPorRotasAsync(rotas.Select(obj => obj.Codigo).ToList()) : new List<BiddingTransportadorRota>();

            // TODO: ToList, .Find não funciona com IList
            List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas = repTransportadorOferta.BuscarValoresPorCodigosTransportadorRota(listaTransportadorRota.Select(obj => obj.Codigo).ToList()).ToList();

            List<(int codigoTransportador, decimal custoEstimadoTotal)> totalPorTransportador = listaTotalOfertas.GroupBy(o => o.codigoTransportador).Select(o => ValueTuple.Create(o.Key, o.Sum(x => x.custoEstimado))).OrderBy(o => o.Item2).ToList();

            List<Dominio.Entidades.Empresa> transportadores = listaTransportadorRota.Select(o => o.Transportador).Distinct().OrderBy(o =>
            {
                return totalPorTransportador.FindIndex(x => x.codigoTransportador == o.Codigo);
            }).ToList();

            List<int> codigosTransportadores = transportadores.Select(x => x.Codigo).ToList();
            List<(int codigoTransportador, string descricaoTrasportador)> codigosDescricaoTrasnportador = transportadores.Select(x => ValueTuple.Create(x.Codigo, x.Descricao)).ToList();

            TipoLanceBidding? lance = rotas?.FirstOrDefault()?.BiddingOferta?.TipoLance;
            bool lancePorViagemEntregaAjudante = rotas?.FirstOrDefault()?.BiddingOferta?.TipoLance == TipoLanceBidding.LancePorViagemEntregaAjudante;
            bool lancePorPesoCapacidadeViagem = lance == TipoLanceBidding.LancePorPeso || lance == TipoLanceBidding.LancePorCapacidade || lance == TipoLanceBidding.LancePorFreteViagem;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Spend", false);
            grid.AdicionarCabecalho("SpendPercentual", false);
            grid.AdicionarCabecalho("RegiaoOrigem", false);
            grid.AdicionarCabecalho("RegiaoDestino", false);
            grid.AdicionarCabecalho("Baseline", false);
            grid.AdicionarCabecalho("BaselinesString", false);
            grid.AdicionarCabecalho("CelulasPersonalizadasString", false);
            grid.AdicionarCabecalho("ColunasTrasportadorString", false);
            grid.AdicionarCabecalho("AliquotaICMS", false);
            grid.AdicionarCabecalho("VolumeTonAnoTransportador", false);
            grid.AdicionarCabecalho("QuantidadeViagensAnoTransportador", false);

            grid.AdicionarCabecalho("Rota", "RotaDescricao", 0.5m, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 0.5m, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 0.5m, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quantidade Viagem Ano", "QuantidadeViagensAno", 0.5m, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Volume (Ton) Ano", "VolumeTonAno", 0.5m, Models.Grid.Align.left, true);

            if (!lancePorPesoCapacidadeViagem)
            {
                grid.AdicionarCabecalho("Quantidade de Entregas", "QuantidadeEntregas", 0.5m, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade de Ajudantes por Veículo", "QuantidadeAjudantes", 0.5m, Models.Grid.Align.left, true);
            }

            Dictionary<int, int> colunas = AdicionarColunasDinamicasBaseline(grid, baselines, ref maximoColunaBaseline);


            AdicionarColunasTransportador(grid, transportadores);

            grid.AdicionarCabecalho("Valor Alvo", "Target", 1, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Rodada", "Rodada", 1, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Transportador", "Transportador", 1, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Menor Valor", "MenorValor", 1, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Impacto (R$)", "ImpactoReais", 1, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Impacto (%)", "ImpactoPercentual", 1, Models.Grid.Align.center, true);

            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>();

            int ultimaRodada = listaTransportadorRota.Max(x => x.Rodada);

            foreach (var rota in rotas)
            {
                decimal menorValor = 0;
                menorValorTransportador = menorValor.ToString("n2");

                List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaRotaOfertas = listaTotalOfertas
                   .Where(x => x.codigoRota == rota.Codigo)
                   .OrderBy(x => x.custoEstimado)
                   .ToList();

                List<(decimal custoEstimado, List<string> nomesTransportadores)> transportadoresPorCusto = new List<(decimal custoEstimado, List<string> nomesTransportadores)>();
                bool naoOfertar = listaRotaOfertas.Exists(transportadorPorCusto => transportadorPorCusto.naoOfertar);

                foreach ((int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar) oferta in listaRotaOfertas)
                {
                    if (!oferta.naoOfertar)
                    {
                        Dominio.Entidades.Empresa transportador = transportadores.Find(t => t.Codigo == oferta.codigoTransportador);
                        if (transportador != null)
                        {
                            (decimal custoEstimado, List<string> nomesTransportadores) tuplaExistente = transportadoresPorCusto.Find(t => t.custoEstimado == oferta.custoEstimado);
                            if (tuplaExistente.Equals(default))
                            {
                                transportadoresPorCusto.Add((oferta.custoEstimado, new List<string> { transportador.NomeFantasia }));
                            }
                            else
                            {
                                tuplaExistente.nomesTransportadores.Add(transportador.NomeFantasia);
                            }
                        }
                    }
                }

                List<(decimal custoEstimado, List<string> nomesTransportadores)> valoresUnicos = transportadoresPorCusto
                    .OrderBy(t => t.custoEstimado)
                    .Take(3)
                    .ToList();

                if (valoresUnicos.Count > 0)
                {
                    (decimal custoEstimado, List<string> nomesTransportadores) primeiroValor = valoresUnicos[0];
                    menorValor = primeiroValor.custoEstimado;
                    menorValorTransportador = menorValor.ToString("n2");
                    primeiroTransportador = string.Join(", ", primeiroValor.nomesTransportadores);
                }

                if (valoresUnicos.Count > 1)
                {
                    (decimal custoEstimado, List<string> nomesTransportadores) segundoValor = valoresUnicos[1];
                    segundoMenorValor = segundoValor.custoEstimado;
                    segundoMenorValorTransportador = segundoMenorValor.ToString("n2");
                    segundoTransportador = string.Join(", ", segundoValor.nomesTransportadores);
                }

                if (valoresUnicos.Count > 2)
                {
                    (decimal custoEstimado, List<string> nomesTransportadores) terceiroValor = valoresUnicos[2];
                    terceiroMenorValor = terceiroValor.custoEstimado;
                    terceiroMenorValorTransportador = terceiroMenorValor.ToString("n2");
                    terceiroTransportador = string.Join(", ", terceiroValor.nomesTransportadores);
                }


                Dominio.Entidades.Embarcador.Bidding.Baseline baseline = baselines.Find(x => x.BiddingOfertaRota?.Codigo == rota.Codigo);
                baselineValor = baseline?.Valor.ToString("n2") ?? "";


                if (baseline != null)
                {
                    impactoReais = lance switch
                    {
                        TipoLanceBidding.LancePorViagemEntregaAjudante => menorValor - baseline.Valor,
                        TipoLanceBidding.LancePorFreteViagem => (menorValor * rota.QuantidadeViagensPorAno) - (baseline.Valor * rota.QuantidadeViagensPorAno),
                        _ => (menorValor * rota.VolumeTonAno) - (baseline.Valor * rota.VolumeTonAno)
                    };

                    decimal impactoPercentual = menorValor / baseline.Valor - 1;
                    impactoPercentualFormatado = impactoPercentual.ToString("P2");
                }

                decimal valorTotalBaselineRota = baselines
                   .Where(baseline => baseline.BiddingOfertaRota.Codigo == rota.Codigo)
                   .Select(baseline => baseline.Valor)
                   .Sum();

                decimal spend = valorTotalBaselineRota * rota.QuantidadeViagensPorAno;

                regiaoOrigem = string.Join(", ", rota.Origens.Select(x => x.Regiao?.Descricao)) ?? string.Join(", ", rota.ClientesOrigem.Select(x => x.Localidade?.Regiao?.Descricao)) ?? string.Join(", ", rota.EstadosOrigem.Select(x => x.RegiaoBrasil?.Descricao)) ?? string.Empty;
                regiaoDestino = string.Join(", ", rota.Destinos.Select(x => x.Regiao?.Descricao)) ?? string.Join(", ", rota.ClientesDestino.Select(x => x.Localidade?.Regiao?.Descricao)) ?? string.Join(", ", rota.EstadosDestino.Select(x => x.RegiaoBrasil?.Descricao)) ?? string.Empty;

                BiddingTransportadorRota biddingTransportadorRota = listaTransportadorRota.Find(x => x.Rota.Codigo == rota.Codigo);
                int rodada = biddingTransportadorRota?.Rodada ?? 1;

                ConsultaBiddingOferta consultaBiddingOferta = new()
                {
                    Codigo = rota.Codigo,
                    RotaCodigo = rota.Codigo,
                    RotaDescricao = rota.Descricao,
                    Origem = rota.DescricaoOrigem,
                    Destino = rota.DescricaoDestino,
                    QuantidadeViagensAno = rota.QuantidadeViagensPorAno.ToString("n2"),
                    VolumeTonAno = rota.VolumeTonAno,
                    QuantidadeEntregas = rota.NumeroEntrega.ToString("N2"),
                    QuantidadeAjudantes = rota.QuantidadeAjudantePorVeiculo.ToString("N2"),
                    AliquotaICMS = rota.AlicotaPadraoICMS?.ToString() ?? string.Empty,
                    Spend = spend.ToString(),
                    RegiaoOrigem = regiaoOrigem,
                    RegiaoDestino = regiaoDestino,
                    Target = (biddingTransportadorRota?.Target ?? 0).ToString("n2"),
                    Rodada = $"{rodada}ª Rodada",
                    Baseline = baselineValor,
                    MenorValor = menorValorTransportador,
                    Transportador = primeiroTransportador != null && !naoOfertar ? primeiroTransportador : "",
                    SegundoMenorValor = segundoMenorValorTransportador,
                    SegundoTransportador = segundoTransportador != null ? segundoTransportador : "",
                    TerceiroMenorValor = terceiroMenorValorTransportador,
                    TerceiroTransportador = terceiroTransportador != null ? terceiroTransportador : "",
                    ImpactoReais = naoOfertar ? "" : impactoReais.ToString(),
                    ImpactoPercentual = naoOfertar ? "" : impactoPercentualFormatado.Replace(" ", ""),
                    tipoLanceBidding = lancePorViagemEntregaAjudante ? TipoLanceBidding.LancePorViagemEntregaAjudante : TipoLanceBidding.LancePorFreteViagem,
                    VolumeTonAnoTransportador = biddingTransportadorRota?.Rota?.VolumeTonAno ?? 0,
                    QuantidadeViagensAnoTransportador = biddingTransportadorRota?.Rota?.QuantidadeViagensPorAno.ToString()
                };
                consultaBiddingOferta.SetRodada(rodada);

                retorno.Add(consultaBiddingOferta);
            }

            decimal spendTotal = retorno.Select(o => o.Spend.ToDecimal()).Sum();

            if (totalRegistros > 0)
            {
                PreencherColunasTransportador(retorno, listaTotalOfertas, codigosDescricaoTrasnportador, rotas.Count, ultimaRodada);
                PreencherCelulasPeronalizadas(retorno, listaTotalOfertas, codigosTransportadores, rotas.Count);

                List<Dominio.ObjetosDeValor.Embarcador.Bidding.ColunaTrasportador> totalPorTransportadorValor = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ColunaTrasportador>();

                foreach (int codigoTransportador in codigosTransportadores)
                {
                    decimal item = totalPorTransportador.FirstOrDefault(o => o.codigoTransportador == codigoTransportador).custoEstimadoTotal;

                    totalPorTransportadorValor.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.ColunaTrasportador()
                    {
                        CodigoTrasportador = codigoTransportador,
                        Valor = item.ToString("n2")
                    });
                }

                decimal menorValor = retorno.Sum(x => x.MenorValor.ToDecimal());
                decimal baseline = retorno.Sum(x => x.Baseline.ToDecimal());
                decimal impactoPercentual = 0;

                if (baseline != 0)
                {
                    impactoPercentual = menorValor / baseline - 1;
                }

                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta()
                {
                    Codigo = -1,
                    RotaDescricao = "TOTAL",
                    ColunasTrasportador = totalPorTransportadorValor,
                    MenorValor = menorValor.ToString("n2"),
                    Baseline = baseline.ToString("n2"),
                    ImpactoReais = retorno.Sum(x => x.ImpactoReais.ToDecimal()).ToString(),
                    ImpactoPercentual = impactoPercentual.ToString("P2"),
                    Target = retorno.Sum(x => x.Target.ToDecimal()).ToString("n2"),
                    QuantidadeViagensAno = rotas.Sum(x => x.QuantidadeViagensPorAno).ToString("n2"),
                    VolumeTonAno = rotas.Sum(x => x.VolumeTonAno),
                    ColunaDinamicaBaseLine1 = baselines.Sum(x => x.Valor).ToString("n2"),
                });

                PreencherColunasDinamicasBaseline(retorno, baselines, maximoColunaBaseline, colunas);
                PreencherTransportadorTotalSpend(retorno, listaTotalOfertas, codigosTransportadores, rotas);


                var ultimoretorno = retorno.Last();
                ultimoretorno.Spend = spendTotal.ToString();
                ultimoretorno.ColunaDinamicaBaseLine1 = CalculoTotalSpendFixa(retorno, rotas).ToString("n2");

            }

            if (spendTotal > 0)
                retorno.ForEach(o => o.SpendPercentual = (o.Spend.ToDecimal() / spendTotal) * 100);

            if (string.IsNullOrWhiteSpace(regiaoOrigem))
                grid.OcultarCabecalho("RegiaoOrigem");

            if (string.IsNullOrWhiteSpace(regiaoDestino))
                grid.OcultarCabecalho("RegiaoDestino");

            bool lancePorPesoCapacidade = lance == TipoLanceBidding.LancePorPeso || lance == TipoLanceBidding.LancePorCapacidade;

            if (lancePorPesoCapacidade)
            {
                grid.OcultarCabecalho("Spend");
                grid.OcultarCabecalho("QuantidadeViagensAno");
                grid.OcultarCabecalho("SpendPercentual");
            }

            if (!lancePorPesoCapacidade)
                grid.OcultarCabecalho("VolumeTonAno");

            if (string.IsNullOrWhiteSpace(menorValorTransportador))
                grid.OcultarCabecalho("MenorValor");

            if (primeiroTransportador == null)
                grid.OcultarCabecalho("Transportador");

            if (impactoReais == 0)
                grid.OcultarCabecalho("ImpactoReais");

            if (string.IsNullOrWhiteSpace(impactoPercentualFormatado))
                grid.OcultarCabecalho("ImpactoPercentual");

            if (segundoTransportador == null)
                grid.OcultarCabecalho("SegundoTransportador");

            if (segundoMenorValor == 0)
                grid.OcultarCabecalho("SegundoMenorValor");

            if (terceiroTransportador == null)
                grid.OcultarCabecalho("TerceiroTransportador");

            if (terceiroMenorValor == 0)
                grid.OcultarCabecalho("TerceiroMenorValor");

            retorno = OrdenarColunas(retorno, parametroConsulta);

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(totalRegistros + 2);

            return grid;
        }

        private List<ConsultaBiddingOferta> OrdenarColunas(List<ConsultaBiddingOferta> retorno, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            if (retorno == null || retorno.Count == 0 || string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                return retorno;

            bool ascendente = parametroConsulta.DirecaoOrdenar == "asc";
            string propriedadeOrdenar = parametroConsulta.PropriedadeOrdenar;

            if (!propriedadeOrdenar.Contains("ColunaTransportador") && !propriedadeOrdenar.Contains("MenorValor"))
            {
                System.Reflection.PropertyInfo propriedade = typeof(ConsultaBiddingOferta).GetProperty(propriedadeOrdenar);
                if (propriedade == null) return retorno;

                return ascendente
                    ? retorno.OrderBy(x => propriedade.GetValue(x)).ToList()
                    : retorno.OrderByDescending(x => propriedade.GetValue(x)).ToList();
            }

            if (propriedadeOrdenar.Contains("MenorValor"))
            {
                return retorno
                    .Select(x => new
                    {
                        Item = x,
                        Valor = decimal.TryParse(x.MenorValor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out decimal d) ? d : 0
                    })
                    .OrderBy(x => ascendente ? x.Valor : -x.Valor)
                    .ThenBy(x => x.Item.Codigo)
                    .Select(x => x.Item)
                    .ToList();
            }

            string codigoTransportador = propriedadeOrdenar.ObterSomenteNumeros();

            return retorno
                .Select(x =>
                {
                    ColunaTrasportador coluna = x.ColunasTrasportador.FirstOrDefault(y => y.CodigoTrasportador.ToString() == codigoTransportador);
                    return new
                    {
                        Item = x,
                        Valor = decimal.TryParse(coluna?.Valor, out decimal d) ? d : 0
                    };
                })
                .OrderBy(x => ascendente ? x.Valor : -x.Valor)
                .ThenBy(x => x.Item.Codigo)
                .Select(x => x.Item)
                .ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas ObterFiltroPesquisaBiddingOfertas()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoRota = Request.GetIntParam("CodigoRota"),
                CodigoModeloVeicular = Request.GetIntParam("CodigoModeloVeicular"),
                CodigoFilialParticipante = Request.GetIntParam("CodigoFilialParticipante"),
                CodigoDestino = Request.GetIntParam("CodigoDestino"),
                CodigoOrigem = Request.GetIntParam("CodigoOrigem"),
                CodigoMesorregiaoDestino = Request.GetIntParam("CodigoMesorregiaoDestino"),
                CodigoMesorregiaoOrigem = Request.GetIntParam("CodigoMesorregiaoOrigem"),
                QuantidadeEntregas = Request.GetIntParam("QuantidadeEntregas"),
                QuantidadeAjudantes = Request.GetIntParam("QuantidadeAjudantes"),
                QuantidadeViagensAno = Request.GetIntParam("QuantidadeViagensAno"),
                CodigoRegiaoDestino = Request.GetIntParam("CodigoRegiaoDestino"),
                CodigoRegiaoOrigem = Request.GetIntParam("CodigoRegiaoOrigem"),
                CodigoRotaDestino = Request.GetIntParam("RotaDestino"),
                CodigoRotaOrigem = Request.GetIntParam("RotaOrigem"),
                CodigoClienteDestino = Request.GetDoubleParam("ClienteDestino"),
                CodigoClienteOrigem = Request.GetDoubleParam("ClienteOrigem"),
                CodigoEstadoDestino = Request.GetIntParam("EstadoDestino"),
                CodigoEstadoOrigem = Request.GetIntParam("EstadoOrigem"),
                CodigoPaisDestino = Request.GetIntParam("PaisDestino"),
                CodigoPaisOrigem = Request.GetIntParam("PaisOrigem"),
                CEPOrigem = Request.GetStringParam("CEPOrigem").ObterSomenteNumeros().ToInt(),
                CEPDestino = Request.GetStringParam("CEPDestino").ObterSomenteNumeros().ToInt()
            };
        }

        private Models.Grid.Grid ObterGridOfertasComponente(bool exportacao)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

            Models.Grid.Grid grid = PreencherGridParametrosConsulta(exportacao, ref parametroConsulta);

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Rota", "RotaDescricao", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Aliquota ICMS", "AliquotaICMS", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Transportador", "TransportadorValorFrete", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Adicional por Entrega", "AdicionalPorEntrega", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Transportador", "TransportadorAdicionalPorEntrega", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Ajudante", "Ajudante", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Transportador", "TransportadorAjudante", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Pedagio/Eixo", "Pedagio", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Transportador", "TransportadorPedagio", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Total", "Total", 10, Models.Grid.Align.left);


            List<ConsultaBiddingOferta> gridOferta = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>>(Request.GetStringParam("GridOfertas"));
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.OfertaComponenteDados> dadosComponentes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.OfertaComponenteDados>>(Request.GetStringParam("OfertasComponente"));

            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>();

            foreach (ConsultaBiddingOferta rota in gridOferta)
            {
                // remover linhas TOTAL e TOTAL SPEND
                if (rota.Codigo == -1 || rota.Codigo == -2)
                {
                    continue;
                }

                (decimal Valor, List<string> Transportadores) menorValorFrete = (Valor: 0, Transportadores: new List<string>());
                (decimal Valor, List<string> Transportadores) menorValorAdicional = (Valor: 0, Transportadores: new List<string>());
                (decimal Valor, List<string> Transportadores) menorValorAjudante = (Valor: 0, Transportadores: new List<string>());
                (decimal Valor, List<string> Transportadores) menorValorPedagio = (Valor: 0, Transportadores: new List<string>());
                List<int> listaVeiculosVerdes = [];

                List<OfertaComponenteDados> ofertasTransportador = dadosComponentes.Where(o => o.CodigoRota == rota.Codigo).ToList();

                foreach (var transportador in ofertasTransportador)
                {
                    AtualizarMenorValor(ref menorValorFrete, transportador.ValorFrete.ToDecimal(), transportador.Transportador);
                    AtualizarMenorValor(ref menorValorAdicional, transportador.AdicionalPorEntrega.ToDecimal(), transportador.Transportador);
                    AtualizarMenorValor(ref menorValorAjudante, transportador.Ajudante.ToDecimal(), transportador.Transportador);
                    AtualizarMenorValor(ref menorValorPedagio, transportador.Pedagio.ToDecimal(), transportador.Transportador);
                    listaVeiculosVerdes.Add(transportador.VeiculosVerdes);
                }

                decimal total = menorValorFrete.Valor + menorValorAdicional.Valor +
                            menorValorAjudante.Valor + menorValorPedagio.Valor;

                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta
                {
                    Codigo = rota.Codigo,
                    RotaCodigo = rota.Codigo,
                    RotaDescricao = rota.RotaDescricao,
                    Origem = rota.Origem,
                    Destino = rota.Destino,
                    ValorFrete = menorValorFrete.Valor.ToString("n2"),
                    Transportador = rota.Transportador,
                    AliquotaICMS = rota.AliquotaICMS,
                    TransportadorValorFrete = string.Join(", ", menorValorFrete.Transportadores.Distinct()),
                    AdicionalPorEntrega = menorValorAdicional.Valor.ToString("n2"),
                    TransportadorAdicionalPorEntrega = string.Join(", ", menorValorAdicional.Transportadores.Distinct()),
                    Ajudante = menorValorAjudante.Valor.ToString("n2"),
                    TransportadorAjudante = string.Join(", ", menorValorAjudante.Transportadores.Distinct()),
                    TransportadorPedagio = string.Join(", ", menorValorPedagio.Transportadores.Distinct()),
                    Pedagio = menorValorPedagio.Valor.ToString("n2"),
                    Total = total.ToString("n2"),
                });
            }

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(retorno.Count);

            return grid;

        }

        private void AtualizarMenorValor(ref (decimal Valor, List<string> Transportadores) menorValor, decimal valorAtual, string nomeTransportador)
        {

            if (valorAtual < menorValor.Valor || menorValor.Valor == 0)
            {
                menorValor = (valorAtual, new List<string> { nomeTransportador });
            }
            else if (valorAtual == menorValor.Valor && !menorValor.Transportadores.Contains(nomeTransportador))
            {
                menorValor.Transportadores.Add(nomeTransportador);
            }
        }

        private async Task<Models.Grid.Grid> ObterGridRankingOfertas(TipoRankingBidding gridFront, CancellationToken cancellationToken, bool calculaCherry = false, bool exportacao = false, Models.Grid.Grid gridOfertas = null)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>(),
                scrollHorizontal = true
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Rota", "RotaDescricao", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("1º Cherry Picking", "PrimeiroTransportadorValor", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("1º Transportador", "PrimeiroTransportadorDescricao", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("1º - % Baseline", "PrimeiroBaseline", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("2º Cherry Picking", "SegundoTransportadorValor", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("2º Transportador", "SegundoTransportadorDescricao", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("2º - % Baseline", "SegundoBaseline", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("3º Cherry Picking", "TerceiroTransportadorValor", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("3º Transportador", "TerceiroTransportadorDescricao", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("3º - % Baseline", "TerceiroBaseline", 10, Models.Grid.Align.left);

            if (calculaCherry)
                grid.AdicionarCabecalho("Spend cenário trabalhado", "ValorSimulado", 10, Models.Grid.Align.left);
            else
                grid.AdicionarCabecalho("Cherry Picking Simulado", "ValorSimulado", 10, Models.Grid.Align.left);

            grid.AdicionarCabecalho("Transportador Simulado", "TransportadorDescricaoSimulado", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("% Baseline Simulado", "BaselineSimulado", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("CodigoTransportadorSimulado", false);
            grid.AdicionarCabecalho("TransportadoresCalculados", false);

            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta> gridAtual = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta>>(Request.GetStringParam(gridFront.ObterDescricao()));
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> gridOferta = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>();

            if (gridOfertas != null)
                gridOferta = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>>(gridOfertas.ObterDataRows().ToJson());
            else if (!Request.GetStringParam("GridOfertas").Equals("{}"))
                gridOferta = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>>(Request.GetStringParam("GridOfertas"));

            bool carregarGrid = Request.GetBoolParam("CarregarGrid");

            if (gridAtual?.Count > 0 && !carregarGrid)
                return PreencheGridRankingCherryPicking(grid, gridAtual, carregarGrid, gridFront);

            grid = await ObterGridRanking(grid, cancellationToken, calculaCherry, exportacao, gridOferta, gridFront);
            return PreencheGridRankingCherryPicking(grid, gridAtual, carregarGrid, gridFront);
        }

        private Models.Grid.Grid PreencheGridRankingCherryPicking(Models.Grid.Grid grid, List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta> gridAtual, bool carregarGrid, TipoRankingBidding tipoRankingBidding)
        {
            if (gridAtual == null)
                return grid;

            decimal valorTotalSimulado = 0;
            decimal baselineTotalSimulado = 0;

            if (carregarGrid)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta> gridRegistros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta>>(grid.ObterDataRows().ToJson());
                gridAtual = gridAtual.Where(x => gridRegistros.Select(x => x.Codigo).Contains(x.Codigo)).ToList();
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta item in gridAtual)
            {
                if (item.CodigoTransportadorSimulado > 0 && item.PrimeiroTransportadorValor.ToDecimal() > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding transportador = item.TransportadoresCalculadosObjeto.Find(x => x.CodigoTransportador == item.CodigoTransportadorSimulado);

                    if (transportador != null)
                    {
                        string valorSimulado = transportador.Valor.ToString("N2");
                        item.ValorSimulado = valorSimulado;
                        item.TransportadorDescricaoSimulado = transportador.Transportador;
                        item.BaselineSimulado = transportador.Baseline;

                        valorTotalSimulado += transportador.Valor;
                        baselineTotalSimulado += transportador.BaselineCalculado;
                    }
                }
                else
                {
                    item.ValorSimulado = string.Empty;
                    item.TransportadorDescricaoSimulado = string.Empty;
                    item.BaselineSimulado = string.Empty;
                }
            }

            string baselineCalculado = baselineTotalSimulado == 0 ? "0%" : ((valorTotalSimulado / baselineTotalSimulado - 1) * 100).ToString("n2") + "%";
            Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta ultimoRegistro = gridAtual.LastOrDefault();

            if (ultimoRegistro != null && ultimoRegistro.PrimeiroTransportadorValor.ToDecimal() > 0)
            {
                gridAtual.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta()
                {
                    RotaDescricao = "Total",
                    ValorSimulado = valorTotalSimulado.ToString("N2"),
                    BaselineSimulado = baselineCalculado
                });
            }
            else if (ultimoRegistro != null)
            {
                ultimoRegistro.ValorSimulado = valorTotalSimulado.ToString("N2");
                ultimoRegistro.BaselineSimulado = baselineCalculado;
            }

            // remover linhas TOTAL e TOTAL SPEND da grid 3
            if (tipoRankingBidding == TipoRankingBidding.GridRankingOfertas)
            {
                gridAtual.RemoveAll(x => x.Codigo == -1 || x.Codigo == -2);
            }

            // remover linha TOTAL SPEND da grid 4
            if (tipoRankingBidding == TipoRankingBidding.GridRankingCherryPickingOferas)
            {
                gridAtual.RemoveAll(x => x.Codigo == -2);
            }

            grid.AdicionaRows(gridAtual);
            grid.setarQuantidadeTotal(gridAtual.Count);

            return grid;
        }


        private async Task<Models.Grid.Grid> ObterGridRanking(Models.Grid.Grid grid, CancellationToken cancellationToken, bool calculaCherry, bool exportacao, List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> gridOferta, TipoRankingBidding tipoRankingBidding)
        {
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioBiddingTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(new Repositorio.UnitOfWork(_conexao.StringConexao));

            Models.Grid.Grid gridDinamica;
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> gridDinamicaRegistros;

            if (gridOferta?.Count > 0)
                gridDinamicaRegistros = gridOferta;
            else
            {
                gridDinamica = await ObterGridOfertaAvaliacao(exportacao, cancellationToken);
                gridDinamicaRegistros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta>>(gridDinamica.ObterDataRows().ToJson())
                    .Where(x => x.ColunasTrasportador != null && x.ColunasTrasportador.Exists(x => !string.IsNullOrWhiteSpace(x.Descricao))).ToList();
            }

            var consultaRankingOferta = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta>(gridDinamicaRegistros.Count);

            var codigosRotaTransportador = gridDinamicaRegistros
                .Where(consulta => consulta.ColunasTrasportador != null)
                .Select(consulta => new
                {
                    consulta.Codigo,
                    CodigoTransportador = consulta.ColunasTrasportador.FirstOrDefault()?.CodigoTrasportador
                })
                .Where(x => x.CodigoTransportador != null)
                .Distinct()
                .ToList();

            var transportadoresRotas = await repositorioBiddingTransportadorRota.BuscarPorRotasETransportadoresAsync(
                codigosRotaTransportador.Select(x => x.Codigo).Distinct().ToList(),
                codigosRotaTransportador.Select(x => x.CodigoTransportador).Distinct().ToList()
            );

            var mapaTransportadoresRotas = transportadoresRotas.ToDictionary(
                x => (x.CodigoRota, x.CodigoTransportador),
                x => x
            );

            foreach (var consultaBiddingOferta in gridDinamicaRegistros)
            {
                if (consultaBiddingOferta.ColunasTrasportador == null) continue;

                int? primeiroTransportador = consultaBiddingOferta.ColunasTrasportador.FirstOrDefault()?.CodigoTrasportador;
                if (primeiroTransportador == null) continue;

                mapaTransportadoresRotas.TryGetValue((consultaBiddingOferta.Codigo, primeiroTransportador), out var rotaInfo);

                var consulta = new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta
                {
                    Codigo = consultaBiddingOferta.Codigo,
                    RotaDescricao = rotaInfo?.RotaDescricao ?? consultaBiddingOferta.RotaDescricao,
                    Origem = rotaInfo?.Origem ?? consultaBiddingOferta.Origem,
                    Destino = rotaInfo?.Destino ?? consultaBiddingOferta.Destino
                };

                ProcessarRankingTransportadores(consulta, consultaBiddingOferta, calculaCherry);

                consultaRankingOferta.Add(consulta);
            }

            // remover linhas TOTAL e TOTAL SPEND da grid 3
            if (tipoRankingBidding == TipoRankingBidding.GridRankingOfertas)
            {
                consultaRankingOferta.RemoveAll(x => x.Codigo == -1 || x.Codigo == -2);
            }

            // remover linha TOTAL SPEND da grid 4
            if (tipoRankingBidding == TipoRankingBidding.GridRankingCherryPickingOferas)
            {
                consultaRankingOferta.RemoveAll(x => x.Codigo == -2);
            }

            grid.AdicionaRows(consultaRankingOferta);
            grid.setarQuantidadeTotal(consultaRankingOferta.Count);

            return grid;
        }

        private void ProcessarRankingTransportadores(Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaRankingOferta consulta, Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta consultaBiddingOferta, bool calculaCherry = false)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding> rotaCalculada = CalculaRankingCherryPicking(consultaBiddingOferta, calculaCherry);
            consulta.TransportadoresCalculados = JsonConvert.SerializeObject(rotaCalculada);

            rotaCalculada = rotaCalculada.Where(x => x.Valor > 0).ToList();

            List<(decimal Valor, string Transportadores)> transportadoresPorValor = rotaCalculada
                .GroupBy(x => x.Valor)
                .Select(g => (Valor: g.Key, Transportadores: string.Join(", ", g.Select(x => x.Transportador))))
                .OrderBy(t => t.Valor)
                .Take(3)
                .ToList();

            for (int i = 0; i < transportadoresPorValor.Count; i++)
            {
                (decimal Valor, string Transportadores) transportador = transportadoresPorValor[i];
                string valorFormatado = transportador.Valor.ToString("N2");
                string descricaoTransportadores = transportador.Transportadores;

                string baseline = rotaCalculada.First(x => x.Valor == transportador.Valor).Baseline;

                switch (i)
                {
                    case 0:
                        consulta.PrimeiroTransportadorValor = valorFormatado;
                        consulta.PrimeiroTransportadorDescricao = descricaoTransportadores;
                        consulta.PrimeiroBaseline = baseline;
                        break;
                    case 1:
                        consulta.SegundoTransportadorValor = valorFormatado;
                        consulta.SegundoTransportadorDescricao = descricaoTransportadores;
                        consulta.SegundoBaseline = baseline;
                        break;
                    case 2:
                        consulta.TerceiroTransportadorValor = valorFormatado;
                        consulta.TerceiroTransportadorDescricao = descricaoTransportadores;
                        consulta.TerceiroBaseline = baseline;
                        break;
                }
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding> CalculaRankingCherryPicking(Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta consultaBiddingOferta, bool calculaCherry = false)
        {
            decimal baselineDecimal = string.IsNullOrEmpty(consultaBiddingOferta.Baseline) ? 0 : consultaBiddingOferta.Baseline.ToDecimal();

            if (calculaCherry)
            {
                decimal fatorMultiplicador = (consultaBiddingOferta.tipoLanceBidding == TipoLanceBidding.LancePorPeso || consultaBiddingOferta.tipoLanceBidding == TipoLanceBidding.LancePorCapacidade)
                                      ? consultaBiddingOferta.VolumeTonAnoTransportador
                                      : consultaBiddingOferta.QuantidadeViagensAnoTransportador.ToInt();

                return consultaBiddingOferta.ColunasTrasportador
                    .Select(transportador =>
                    {
                        decimal valorMultiplicado = transportador.Valor.ToDecimal() * fatorMultiplicador;
                        decimal BaselineCalculado = fatorMultiplicador * baselineDecimal;
                        string baseline = baselineDecimal == 0 || fatorMultiplicador == 0 ? "0%" : ((valorMultiplicado / BaselineCalculado - 1) * 100).ToString("n2") + "%";

                        return new Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding()
                        {
                            CodigoTransportador = transportador.CodigoTrasportador,
                            Transportador = transportador.Descricao,
                            Valor = valorMultiplicado,
                            Baseline = baseline,
                            BaselineCalculado = BaselineCalculado
                        };
                    }).ToList();

            }

            return consultaBiddingOferta.ColunasTrasportador
                .Select(transportador =>
                {
                    decimal valorDecimal = transportador.Valor.ToDecimal();
                    string baseline = baselineDecimal == 0 ? "0%" : ((transportador.Valor.ToDecimal() / baselineDecimal - 1) * 100).ToString("n2") + "%";

                    return new Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding()
                    {
                        CodigoTransportador = transportador.CodigoTrasportador,
                        Transportador = transportador.Descricao,
                        Valor = valorDecimal,
                        Baseline = baseline,
                        BaselineCalculado = baselineDecimal
                    };

                }).ToList();
        }

        private Dictionary<int, int> AdicionarColunasDinamicasBaseline(Models.Grid.Grid grid, List<Dominio.Entidades.Embarcador.Bidding.Baseline> baselines, ref int maximoColunaBaseline)
        {
            int UltimaColunaDinamica = 1;

            Dictionary<int, int> colunaTipoBaseline = new Dictionary<int, int>();

            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> tiposBaseline = baselines.Where(o => o.TipoBaseline != null).Select(o => o.TipoBaseline).Distinct().ToList();

            maximoColunaBaseline = Math.Min(tiposBaseline.Count, maximoColunaBaseline);
            for (int i = 0; i < maximoColunaBaseline; i++)
            {
                grid.AdicionarCabecalho(tiposBaseline[i].Descricao, "ColunaDinamicaBaseLine" + UltimaColunaDinamica.ToString(), 0.5m, Models.Grid.Align.center, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tiposBaseline[i].Codigo);

                colunaTipoBaseline.Add(tiposBaseline[i].Codigo, UltimaColunaDinamica);

                UltimaColunaDinamica++;
            }

            return colunaTipoBaseline;
        }

        private void AdicionarColunasTransportador(Models.Grid.Grid grid, List<Dominio.Entidades.Empresa> transportadores)
        {
            Models.Grid.EditableCell editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal);

            foreach (Dominio.Entidades.Empresa trasportador in transportadores)
            {
                grid.AdicionarCabecalho(trasportador.NomeFantasia, $"ColunaTransportador{trasportador.Codigo}", 1, Models.Grid.Align.center, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, trasportador.Codigo).Editable(editableValor);
            }
        }

        private void PreencherColunasDinamicasBaseline(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno, List<Dominio.Entidades.Embarcador.Bidding.Baseline> baselines, int maximoColunaBaseline, Dictionary<int, int> coluna)
        {
            if (maximoColunaBaseline == 0)
                return;

            List<int> rotas = retorno.Select(r => r.RotaCodigo).Distinct().ToList();

            foreach (int rota in rotas)
            {
                foreach (Dominio.Entidades.Embarcador.Bidding.Baseline baseline in baselines.Where(o => o.BiddingOfertaRota.Codigo == rota))
                {
                    string custoEstimado = baseline.Valor.ToString("n2");

                    int numeroColuna = coluna.FirstOrDefault(x => x.Key == baseline.TipoBaseline.Codigo).Value;
                    System.Reflection.PropertyInfo propertyInfoValor = retorno.First(r => r.RotaCodigo == rota).GetType().GetProperty($"ColunaDinamicaBaseLine{numeroColuna}");
                    Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta transportadorRota = retorno.First(r => r.RotaCodigo == rota);
                    propertyInfoValor.SetValue(transportadorRota, custoEstimado, null);

                    if (transportadorRota.Baselines == null)
                        transportadorRota.Baselines = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.Baseline>();

                    transportadorRota.Baselines.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.Baseline() { Valor = decimal.Parse(custoEstimado) });
                }
            }
        }

        private void PreencherCelulasPeronalizadas(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno, List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas, List<int> codigosTransportadores, int quantidadeRegistros)
        {
            if (quantidadeRegistros <= 0)
                return;

            for (int indiceTransportadorRota = 0; indiceTransportadorRota < quantidadeRegistros; indiceTransportadorRota++)
            {
                Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta transportadorRota = retorno[indiceTransportadorRota];

                if (transportadorRota.CelulasPersonalizadas == null)
                    transportadorRota.CelulasPersonalizadas = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.CelulaPersonalizada>();

                for (int indiceTransportador = 0; indiceTransportador < codigosTransportadores.Count; indiceTransportador++)
                {
                    int codigoTransportador = codigosTransportadores[indiceTransportador];

                    string custoEstimadoBaseline = CalculoPercentualBaseline(transportadorRota, listaTotalOfertas, codigoTransportador);
                    string custoEstimadoAlvo = CalculoPercentualValorAlvo(transportadorRota, listaTotalOfertas, codigoTransportador);

                    transportadorRota.CelulasPersonalizadas.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.CelulaPersonalizada
                    {
                        CodigoTrasportador = codigoTransportador,
                        ValorBaseline = custoEstimadoBaseline,
                        ValorAlvo = custoEstimadoAlvo
                    });
                }
            }
        }

        private void PreencherColunasTransportador(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno, List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas, List<(int codigoTransportador, string descricaoTrasportador)> codigosDescricaoTrasnportador, int quantidadeRegistros, int ultimaRodada)
        {
            for (int indiceTransportadorRota = 0; indiceTransportadorRota < quantidadeRegistros; indiceTransportadorRota++)
            {
                foreach ((int codigoTransportador, string descricaoTrasportador) codigoDescricaoTrasnportador in codigosDescricaoTrasnportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta transportadorRota = retorno[indiceTransportadorRota];

                    if (transportadorRota.ColunasTrasportador == null)
                        transportadorRota.ColunasTrasportador = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ColunaTrasportador>();

                    (int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar) totalOfertaTransportador = listaTotalOfertas.Find(o => o.codigoTransportador == codigoDescricaoTrasnportador.codigoTransportador && o.codigoRota == transportadorRota.Codigo);

                    decimal custoEstimado = totalOfertaTransportador.custoEstimado;
                    bool naoOfertou = totalOfertaTransportador.naoOfertar;

                    ColunaTrasportador colunaTrasportador = new()
                    {
                        CodigoTrasportador = codigoDescricaoTrasnportador.codigoTransportador,
                        Valor = naoOfertou ? "Não Ofertado" : custoEstimado.ToString("n2"),
                        Descricao = codigoDescricaoTrasnportador.descricaoTrasportador,
                        NaoOfertou = naoOfertou
                    };
                    if (transportadorRota.IsUltimaRodadaMaiorQueUm(ultimaRodada))
                    {
                        colunaTrasportador.Valor = string.Empty;
                    }

                    transportadorRota.ColunasTrasportador.Add(colunaTrasportador);
                }
            }
        }

        private void PreencherTransportadorTotalSpend(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno, List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas, List<int> codigosTransportadores, List<BiddingOfertaRota> rotas)
        {
            Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta consultaBiddingOferta = CalculaValoresTotalSpend(retorno, rotas);

            for (int indiceTransportadorRota = 0; indiceTransportadorRota < codigosTransportadores.Count; indiceTransportadorRota++)
            {

                for (int indiceTransportador = 0; indiceTransportador < retorno.Where(x => x.Codigo != 0).Count(); indiceTransportador++)
                {

                    int codigoTransportador = codigosTransportadores[indiceTransportadorRota];
                    decimal calculoQtdViagemPesoPorValorTotal = 0;
                    decimal resultadoQtdViagemPesoPorValorTotal = 0;
                    decimal custoEstimado = 0;

                    foreach (BiddingOfertaRota rota in rotas)
                    {
                        custoEstimado = listaTotalOfertas.Find(o => o.codigoTransportador == codigoTransportador && o.codigoRota == rota.Codigo).custoEstimado;

                        if (rota.QuantidadeViagensPorAno != 0)
                            calculoQtdViagemPesoPorValorTotal = custoEstimado * rota.QuantidadeViagensPorAno;
                        else if (rota.VolumeTonAno != 0)
                            calculoQtdViagemPesoPorValorTotal = custoEstimado * rota.VolumeTonAno;

                        resultadoQtdViagemPesoPorValorTotal += calculoQtdViagemPesoPorValorTotal;
                    }
                    consultaBiddingOferta.ColunasTrasportador.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.ColunaTrasportador() { CodigoTrasportador = codigoTransportador, Valor = resultadoQtdViagemPesoPorValorTotal.ToString("n2") });
                }
            }
            retorno.Add(consultaBiddingOferta);
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta CalculaValoresTotalSpend(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno, List<BiddingOfertaRota> rotas)
        {

            TipoLanceBidding tipoLance = rotas.Select(x => x.BiddingOferta.TipoLance).FirstOrDefault();
            List<(int codigoOfertaRota, decimal valorLance)> valoresLance;

            switch (tipoLance)
            {
                case TipoLanceBidding.LancePorCapacidade:
                case TipoLanceBidding.LancePorPeso:
                    valoresLance = rotas.Select(x => (x.Codigo, (decimal)x.VolumeTonAno)).ToList();
                    break;
                default:
                    valoresLance = rotas.Select(x => (x.Codigo, (decimal)x.QuantidadeViagensPorAno)).ToList();
                    break;
            }

            List<(int codigoOfertaRota, decimal valorParametroCalculo)> valoresMenorValor = retorno.Select(x => (x.Codigo, x.MenorValor.ToDecimal())).ToList();
            List<(int codigoOfertaRota, decimal valorParametroCalculo)> valoresAlvo = retorno.Select(x => (x.Codigo, x.Target.ToDecimal())).ToList();
            List<(int codigoOfertaRota, decimal valorParametroCalculo)> valoresBaseline = retorno.Select(x => (x.Codigo, x.Baseline.ToDecimal())).ToList();

            Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta consultaBiddingOferta = new Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta()
            {
                ColunasTrasportador = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.ColunaTrasportador>(),
                Codigo = -2,
                RotaDescricao = "TOTAL SPEND",
                MenorValor = CalculaParametros(valoresLance, valoresMenorValor).ToString("N2"),
                Target = CalculaParametros(valoresLance, valoresAlvo).ToString("N2")
            };

            decimal totalSpend = CalculaParametros(valoresLance, valoresBaseline);
            if (totalSpend > 0)
            {
                consultaBiddingOferta.ImpactoReais = (consultaBiddingOferta.MenorValor.ToDecimal() - totalSpend).ToString();
                consultaBiddingOferta.ImpactoPercentual = ((consultaBiddingOferta.MenorValor.ToDecimal() / totalSpend) - 1).ToString("P2");
            }

            return consultaBiddingOferta;
        }

        private decimal CalculaParametros(List<(int codigoOfertaRota, decimal valorLance)> valoresLance, List<(int codigoOfertaRota, decimal valorParametroCalculo)> valoresParametrosCalculo)
        {
            decimal soma = 0;

            foreach ((int codigoOfertaRota, decimal valorLance) item in valoresLance)
            {
                if (item.codigoOfertaRota <= 0)
                    continue;

                soma += item.valorLance * valoresParametrosCalculo.FirstOrDefault(x => x.codigoOfertaRota == item.codigoOfertaRota).valorParametroCalculo;
            }

            return soma;
        }

        private decimal CalculoTotalSpendFixa(List<Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta> retorno, List<BiddingOfertaRota> rotas)
        {
            decimal soma = 0;

            TipoLanceBidding tipoLance = rotas.Select(x => x.BiddingOferta.TipoLance).FirstOrDefault();

            foreach (Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta item in retorno)
            {
                if (item.Codigo <= 0)
                    continue;

                if (tipoLance == TipoLanceBidding.LancePorCapacidade || tipoLance == TipoLanceBidding.LancePorPeso)
                {
                    soma += (item.Baselines?.FirstOrDefault()?.Valor ?? 0) * (rotas.FirstOrDefault(x => x.Codigo == item.Codigo)?.VolumeTonAno ?? 0);
                    continue;
                }

                soma += (item.Baselines?.FirstOrDefault()?.Valor ?? 0) * (rotas.FirstOrDefault(x => x.Codigo == item.Codigo)?.QuantidadeViagensPorAno ?? 0);
            }

            return soma;
        }

        private string CalculoPercentualValorAlvo(Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta transportadorRota, List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas, int codigoTransportador)
        {
            string resultadoFormatadoPercentualDiferencaValorAlvo = string.Empty;

            (int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar) totalTransportador = listaTotalOfertas.FirstOrDefault(o => o.codigoTransportador == codigoTransportador && o.codigoRota == transportadorRota.Codigo);

            if (transportadorRota == null)
                return string.Empty;

            decimal valorAlvo = transportadorRota.Target.ToDecimal();
            decimal valorTransportador = totalTransportador.custoEstimado;

            if (valorAlvo > 0)
            {
                decimal percentualDiferencaValorAlvo = ((valorTransportador / valorAlvo) - 1) * 100;

                resultadoFormatadoPercentualDiferencaValorAlvo = percentualDiferencaValorAlvo >= 0 ? $"Valor Alvo = +{percentualDiferencaValorAlvo.ToString("n2")}% " : $"Valor Alvo = {percentualDiferencaValorAlvo.ToString("n2")}%";
            }
            else
                resultadoFormatadoPercentualDiferencaValorAlvo = "Valor Alvo: 0.00";

            return resultadoFormatadoPercentualDiferencaValorAlvo;
        }

        private string CalculoPercentualBaseline(Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOferta transportadorRota, List<(int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar)> listaTotalOfertas, int codigoTransportador)
        {
            string resultadoFormatadoPercentualDiferencaBaseline = string.Empty;

            (int codigoTransportador, int codigoRota, decimal custoEstimado, bool naoOfertar) totalTransportador = listaTotalOfertas.FirstOrDefault(o => o.codigoTransportador == codigoTransportador && o.codigoRota == transportadorRota.Codigo);

            if (transportadorRota == null)
                return string.Empty;

            decimal valorBaseline = transportadorRota.Baseline.ToDecimal();
            decimal valorTransportador = totalTransportador.custoEstimado;

            if (valorBaseline > 0)
            {
                decimal percentualDiferencaBaseline = ((valorTransportador / valorBaseline) - 1) * 100;

                resultadoFormatadoPercentualDiferencaBaseline = percentualDiferencaBaseline >= 0 ? $"Baseline = +{percentualDiferencaBaseline.ToString("n2")}%" : $"Baseline = {percentualDiferencaBaseline.ToString("n2")}%";
            }
            else
                resultadoFormatadoPercentualDiferencaBaseline = "Baseline: 0.00";

            return resultadoFormatadoPercentualDiferencaBaseline;
        }

        private void AprovarTransportadoresVencedores(dynamic listaVencedores, List<int> codigosVencedores, int codigoBiddingConvite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repositorioTransportador = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(unitOfWork);
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);

            foreach (dynamic listaVencedor in listaVencedores)
            {
                BiddingTransportadorRotaOferta biddingTransportadorRotaOferta = repositorioTransportador.BuscarVencedoresRota(codigoBiddingConvite, (int)listaVencedor.TransportadorCodigo, (int)listaVencedor.RotaCodigo);

                if (biddingTransportadorRotaOferta == null)
                    throw new ControllerException("Selecionou o transportador sem informar Valor.");

                biddingTransportadorRotaOferta.Aceito = true;

                BiddingTransportadorRota transportadorRota = biddingTransportadorRotaOferta.TransportadorRota;
                transportadorRota.Status = StatusBiddingRota.Aprovada;

                repositorioTransportador.Atualizar(biddingTransportadorRotaOferta);
                repositorioTransportadorRota.Atualizar(transportadorRota);

                codigosVencedores.Add(transportadorRota.Codigo);
            }
        }

        private void RejeitarTransportadoresRestantes(List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> listaTransportadoresRejeitados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota transportadorRejeitado in listaTransportadoresRejeitados)
            {
                transportadorRejeitado.Status = StatusBiddingRota.Rejeitada;

                repositorioTransportadorRota.Atualizar(transportadorRejeitado);
            }
        }

        private dynamic ObterModelosVeiculares(List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> listaOfertas)
        {
            return (from o in listaOfertas
                    select new
                    {
                        o.ModeloVeicular.Codigo,
                        o.ModeloVeicular.Descricao
                    }).GroupBy(x => x.Codigo).Select(g => g.First()).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> FiltrarRotasOpcoesFiltro(List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> listaRotas, Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas)
        {

            if (filtroPesquisaBiddingOfertas.CodigoRota > 0)
                listaRotas = listaRotas.Where(obj => obj.Codigo == filtroPesquisaBiddingOfertas.CodigoRota).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoModeloVeicular > 0)
                listaRotas = listaRotas.Where(o => o.ModelosVeicularesCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoModeloVeicular).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoFilialParticipante > 0)
                listaRotas = listaRotas.Where(o => o.FiliaisCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoFilialParticipante).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoOrigem > 0)
                listaRotas = listaRotas.Where(o => o.OrigensCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoOrigem).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoDestino > 0)
                listaRotas = listaRotas.Where(o => o.DestinosCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoDestino).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoRegiaoDestino > 0)
            {
                listaRotas = listaRotas
                    .Where(o =>
                        o.DestinosRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino ||
                        o.EstadoDestinoRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino ||
                        o.RegioesDestinoCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino ||
                        o.ClienteDestinoRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino ||
                        o.RotaDestinoRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino
                    )
                    .ToList();
            }

            if (filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem > 0)
            {
                listaRotas = listaRotas
                    .Where(o =>
                        o.OrigensRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem ||
                        o.EstadoOrigemRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem ||
                        o.RegioesOrigemCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem ||
                        o.ClienteOrigemRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem ||
                        o.RotaOrigemRegiaoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem
                    )
                    .ToList();
            }

            if (filtroPesquisaBiddingOfertas.CodigoMesorregiaoDestino > 0)
                listaRotas = listaRotas.Where(o => o.RegioesDestinoCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoMesorregiaoDestino).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoMesorregiaoOrigem > 0)
                listaRotas = listaRotas.Where(o => o.RegioesOrigemCodigos.ToInt() == filtroPesquisaBiddingOfertas.CodigoMesorregiaoOrigem).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoRotaDestino > 0)
                listaRotas = listaRotas.Where(o => o.RotaDestinoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRotaDestino).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoRotaOrigem > 0)
                listaRotas = listaRotas.Where(o => o.RotaOrigemCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoRotaOrigem).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoClienteDestino > 0)
                listaRotas = listaRotas.Where(o => o.ClienteDestinoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoClienteDestino).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoClienteOrigem > 0)
                listaRotas = listaRotas.Where(o => o.ClienteDestinoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoClienteOrigem).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoEstadoDestino > 0)
                listaRotas = listaRotas.Where(o => o.EstadoDestinoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoEstadoDestino).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoEstadoOrigem > 0)
                listaRotas = listaRotas.Where(o => o.EstadoOrigemCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoEstadoOrigem).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoPaisDestino > 0)
                listaRotas = listaRotas.Where(o => o.PaisDestinoCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoPaisDestino).ToList();

            if (filtroPesquisaBiddingOfertas.CodigoPaisOrigem > 0)
                listaRotas = listaRotas.Where(o => o.PaisOrigemCodigo.ToInt() == filtroPesquisaBiddingOfertas.CodigoPaisOrigem).ToList();

            //if (filtroPesquisaBiddingOfertas.CEPDestino > 0)
            //    listaRotas = listaRotas.Where(o => o.CEPsDestino.Any(x => x.CEPInicial <= filtroPesquisaBiddingOfertas.CEPDestino && x.CEPFinal >= filtroPesquisaBiddingOfertas.CEPDestino).ToList();

            //if (filtroPesquisaBiddingOfertas.CEPOrigem > 0)
            //    listaRotas = listaRotas.Where(o => o.CEPsOrigem.Any(x => x.CEPInicial <= filtroPesquisaBiddingOfertas.CEPOrigem && x.CEPFinal >= filtroPesquisaBiddingOfertas.CEPOrigem).ToList();

            if (filtroPesquisaBiddingOfertas.QuantidadeEntregas > 0)
                listaRotas = listaRotas.Where(o => o.QuantidadeEntregas == filtroPesquisaBiddingOfertas.QuantidadeEntregas).ToList();

            if (filtroPesquisaBiddingOfertas.QuantidadeAjudantes > 0)
                listaRotas = listaRotas.Where(o => o.QuantidadeAjudantes == filtroPesquisaBiddingOfertas.QuantidadeAjudantes).ToList();

            if (filtroPesquisaBiddingOfertas.QuantidadeViagensAno > 0)
                listaRotas = listaRotas.Where(o => o.QuantidadeViagensAno == filtroPesquisaBiddingOfertas.QuantidadeViagensAno).ToList();

            return listaRotas;

        }

        private Models.Grid.Grid PreencherGridParametrosConsulta(bool exportacao, ref Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            Models.Grid.Grid grid;

            if (!exportacao)
            {
                grid = new Models.Grid.Grid(Request)
                {
                    scrollHorizontal = true
                };
            }
            else
                grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

            grid.AdicionarCabecalho("Codigo", false);

            if (!exportacao)
                parametroConsulta = grid.ObterParametrosConsulta();

            grid.header = new List<Models.Grid.Head>();
            return grid;
        }

        #endregion
    }
}
