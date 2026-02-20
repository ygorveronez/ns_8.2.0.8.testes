using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Bidding;
using Dominio.Entidades.Embarcador.Localidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Bidding;
using Dominio.ObjetosDeValor.Embarcador.Bidding.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Bidding
{
    public sealed class Bidding
    {
        #region Variáveis
        readonly Repositorio.UnitOfWork _unitOfWork;
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly Dominio.Entidades.Usuario _usuario;
        readonly CancellationToken _cancellationToken;

        #endregion

        #region Construtor

        public Bidding(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Bidding(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;
        }
        public Bidding(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Públicos

        public void AtualizarRanking(Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota rota, List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> biddingTransportadorRotaOfertas = null)
        {
            Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(_unitOfWork);
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repBiddingTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> biddingTransportadoresRotaOferta;

            if (biddingTransportadorRotaOfertas?.Count > 0)
                biddingTransportadoresRotaOferta = biddingTransportadorRotaOfertas.Where(obj => obj.TransportadorRota.Rota.Codigo == rota.Codigo && obj.TransportadorRota.Status != StatusBiddingRota.NovaRodada && obj.TransportadorRota.Status != StatusBiddingRota.Rejeitada).ToList();
            else
                biddingTransportadoresRotaOferta = repOferta.BuscarPorRota(rota.Codigo).Where(
                        obj => obj.TransportadorRota.Status != StatusBiddingRota.NovaRodada
                        && obj.TransportadorRota.Status != StatusBiddingRota.Rejeitada).ToList();


            List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> biddingTransportadoresRota = (from obj in biddingTransportadoresRotaOferta
                                                                                                              select obj.TransportadorRota).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota biddingTransportadorRota in biddingTransportadoresRota)
            {
                biddingTransportadorRota.Ranking = 0;
            }

            List<IGrouping<int, BiddingTransportadorRotaOferta>> groupTransportadorRotaOferta = biddingTransportadoresRotaOferta.OrderBy(o => o.CustoEstimado).GroupBy(o => o.TransportadorRota.Codigo).ToList();

            Dictionary<int, decimal> listaCustoEstimado = new Dictionary<int, decimal>();

            foreach (IGrouping<int, BiddingTransportadorRotaOferta> biddingTransportadorRotaOferta in groupTransportadorRotaOferta)
            {
                listaCustoEstimado.Add(biddingTransportadorRotaOferta.Select(o => o.Codigo).FirstOrDefault(), biddingTransportadorRotaOferta.Min(o => o.CustoEstimado));
            }

            List<int> codigosOrdenados = listaCustoEstimado.OrderBy(o => o.Value).Select(o => o.Key).ToList();

            foreach (Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta biddingTransportadorRotaOferta in biddingTransportadoresRotaOferta)
            {
                for (int i = 0; i < codigosOrdenados.Count; i++)
                {
                    int codigoOferta = codigosOrdenados[i];
                    if (codigoOferta == biddingTransportadorRotaOferta.Codigo)
                    {
                        biddingTransportadorRotaOferta.TransportadorRota.Ranking = i + 1;
                        repBiddingTransportadorRota.Atualizar(biddingTransportadorRotaOferta.TransportadorRota);
                        break;
                    }
                }
            }

        }

        public decimal ObterPorcentagemICMSCalculada(decimal porcentagemICMS, bool naoConsiderarICMS)
        {
            if (naoConsiderarICMS)
                return 1;

            return (1 - (porcentagemICMS / 100));
        }

        public void GerarEstimativaDeCustoOferta(ref BiddingTransportadorRotaOferta entidadeOferta, bool naoIncluirImpostoICMS)
        {
            decimal valorCalculoTotalLiquido = naoIncluirImpostoICMS ? 1 : 0.9075M;
            decimal porcentagemICMS = ObterPorcentagemICMSCalculada(entidadeOferta.ICMSPorcentagem, naoIncluirImpostoICMS);

            switch (entidadeOferta.TipoOferta)
            {
                case TipoLanceBidding.LanceFrotaFixaFranquia:
                    entidadeOferta.CustoEstimado = entidadeOferta.ValorFixo + entidadeOferta.ValorFranquia * (entidadeOferta.TransportadorRota.Rota.QuilometragemMedia - entidadeOferta.Quilometragem);
                    break;
                case TipoLanceBidding.LancePorEquipamento:
                    entidadeOferta.CustoEstimado = entidadeOferta.ValorFixoEquipamento;
                    break;
                case TipoLanceBidding.LanceFrotaFixaKmRodado:
                    entidadeOferta.CustoEstimado = entidadeOferta.ValorFixoMensal + (entidadeOferta.ValorKmRodado * entidadeOferta.TransportadorRota.Rota.QuilometragemMedia);
                    break;
                case TipoLanceBidding.LancePorcentagemNota:
                    entidadeOferta.CustoEstimado = (entidadeOferta.Porcentagem / 100) * entidadeOferta.TransportadorRota.Rota.ValorCargaMes;
                    break;
                case TipoLanceBidding.LanceViagemAdicional:
                    entidadeOferta.CustoEstimado = (entidadeOferta.ValorViagem * entidadeOferta.TransportadorRota.Rota.Frequencia) + (entidadeOferta.ValorEntrega * (entidadeOferta.TransportadorRota.Rota.NumeroEntrega - entidadeOferta.TransportadorRota.Rota.AdicionalAPartirDaEntregaNumero));
                    break;
                case TipoLanceBidding.LancePorPeso:
                    decimal freteComICMSPeso = (entidadeOferta.FreteTonelada / porcentagemICMS);
                    decimal pedagioComICMSPeso = ((entidadeOferta.PedagioParaEixo / porcentagemICMS) * entidadeOferta.ModeloVeicular.NumeroEixos) ?? 0;
                    decimal totalBrutoPeso = freteComICMSPeso + (pedagioComICMSPeso / (entidadeOferta.ModeloVeicular.CapacidadePesoTransporte / 1000));
                    entidadeOferta.CustoEstimado = Math.Round((totalBrutoPeso * porcentagemICMS * valorCalculoTotalLiquido), 2);
                    break;
                case TipoLanceBidding.LancePorCapacidade:
                    decimal freteComICMSCapacidadeTon = (entidadeOferta.FreteTonelada / porcentagemICMS);
                    decimal freteComICMSCapacidade = freteComICMSCapacidadeTon * (entidadeOferta.ModeloVeicular.CapacidadePesoTransporte / 1000);
                    decimal pedagioComICMSCapacidade = ((entidadeOferta.PedagioParaEixo / porcentagemICMS) * entidadeOferta.ModeloVeicular.NumeroEixos) ?? 0;
                    decimal totalBrutoCapacidade = (freteComICMSCapacidade + pedagioComICMSCapacidade);
                    entidadeOferta.CustoEstimado = Math.Round((totalBrutoCapacidade * porcentagemICMS * valorCalculoTotalLiquido), 2);
                    break;
                case TipoLanceBidding.LancePorFreteViagem:
                    decimal totalBrutoViagem = ((entidadeOferta.FreteTonelada / porcentagemICMS) + (decimal)(entidadeOferta.PedagioParaEixo * entidadeOferta.ModeloVeicular.NumeroEixos) / porcentagemICMS);
                    entidadeOferta.CustoEstimado = Math.Round((totalBrutoViagem * valorCalculoTotalLiquido), 2);
                    break;
                case TipoLanceBidding.LancePorViagemEntregaAjudante:
                    entidadeOferta.CustoEstimado = Math.Round((decimal)(((entidadeOferta.TransportadorRota.Rota.NumeroEntrega * entidadeOferta.AdicionalPorEntrega) + (entidadeOferta.ModeloVeicular.NumeroEixos * entidadeOferta.PedagioParaEixo) + (entidadeOferta.TransportadorRota.Rota.QuantidadeAjudantePorVeiculo * entidadeOferta.Ajudante) + entidadeOferta.FreteTonelada) / porcentagemICMS), 2);
                    break;
            }
        }

        public void EnviarEmailConvidados()
        {
            try
            {
                _unitOfWork.Start();

                Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork);

                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioBiddingConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(_unitOfWork);

                List<BiddingConviteConvidado> listaConvidados = repositorioBiddingConvidado.BuscarConvidadosNaoAvisados();

                foreach (BiddingConviteConvidado convidado in listaConvidados)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                    {
                        AssuntoEmail = "Alerta de Convite para Bidding",
                        CabecalhoMensagem = "Alerta de Convite para Bidding",
                        Empresa = convidado.Convidado,
                        Mensagem = $"Você foi convidado para o Bidding \"{convidado.BiddingConvite.Descricao}\" que ficará disponível no dia {convidado.BiddingConvite.DataInicio.ToString("dd/MM/yyyy")} a partir das  {convidado.BiddingConvite.DataInicio.ToString("HH:mm")}."
                    };

                    servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);

                    convidado.EmailAvisoConvidadoEnviado = true;
                    repositorioBiddingConvidado.Atualizar(convidado);
                }
                ;

                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(e);
            }
        }

        public void NotificarConvidado(string mensagem, string titulo, BiddingConviteConvidado convidado)
        {
            try
            {
                _unitOfWork.Start();

                Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                {
                    AssuntoEmail = titulo,
                    CabecalhoMensagem = titulo,
                    Empresa = convidado.Convidado,
                    Mensagem = mensagem
                };

                servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);

                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(e);
            }
        }

        public void AvancarEtapaUm()
        {
            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBidding = new Repositorio.Embarcador.Bidding.BiddingConvite(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingChecklist repositorioBiddingChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioBiddingConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(_unitOfWork);

                List<BiddingConvite> listaBidding = repositorioBidding.BuscarPorEtapa(StatusBiddingConvite.Aguardando);

                foreach (BiddingConvite bidding in listaBidding)
                {
                    if (bidding.DataPrazoAceiteConvite.Value < DateTime.Now)
                    {
                        bidding.Status = StatusBiddingConvite.Checklist;
                        repositorioBidding.Atualizar(bidding);

                        BiddingChecklist checklist = repositorioBiddingChecklist.BuscarChecklist(bidding);
                        List<BiddingConviteConvidado> listaBiddingConvidado = repositorioBiddingConvidado.BuscarConvidadosPorBiddingEtapa(bidding, new List<StatusBiddingConvite> { StatusBiddingConvite.Aguardando, StatusBiddingConvite.Checklist });

                        if (checklist.TipoPreenchimentoChecklist == TipoPreenchimentoChecklist.PreenchimentoDesabilitado)
                        {
                            bidding.Status = StatusBiddingConvite.Ofertas;
                            repositorioBidding.Atualizar(bidding);

                            Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador repositorioChecklistTransportador = new Repositorio.Embarcador.Bidding.BiddingChecklistBiddingTransportador(_unitOfWork);
                            List<BiddingChecklistBiddingTransportador> transportadores = repositorioChecklistTransportador.BuscarPorBidding(bidding);

                            foreach (BiddingChecklistBiddingTransportador transportador in transportadores)
                            {
                                transportador.Situacao = StatusBiddingConviteTransportadorRespostas.Aprovado;
                                repositorioChecklistTransportador.Atualizar(transportador);

                                BiddingConviteConvidado convidado = repositorioBiddingConvidado.BuscarConvidado(transportador.BiddingConvite, transportador.Transportador);
                                if (convidado.StatusBidding != StatusBiddingConvite.Aguardando)
                                    convidado.StatusBidding = StatusBiddingConvite.Ofertas;

                                repositorioBiddingConvidado.Atualizar(convidado);
                            }

                            foreach (BiddingConviteConvidado convidado in listaBiddingConvidado)
                            {
                                if (convidado.StatusBidding == StatusBiddingConvite.Aguardando)
                                {
                                    convidado.Status = StatusBiddingConviteConvidado.Rejeitado;
                                    repositorioBiddingConvidado.Atualizar(convidado);
                                }
                                else
                                    NotificarConvidado($"O Bidding \"{bidding.Descricao}\" avançou para a etapa de Ofertas.", "Bidding - Avanço de Etapa", convidado);
                            }
                        }
                        else
                        {
                            foreach (BiddingConviteConvidado convidado in listaBiddingConvidado)
                            {
                                if (convidado.StatusBidding == StatusBiddingConvite.Aguardando)
                                {
                                    convidado.Status = StatusBiddingConviteConvidado.Rejeitado;
                                    repositorioBiddingConvidado.Atualizar(convidado);
                                }
                                else
                                    NotificarConvidado($"O Bidding \"{bidding.Descricao}\" avançou para a etapa de Checklist.<br />A etapa de Checklist acaba em {checklist.DataPrazo?.ToString("dd/MM/yyyy HH:mm")}.", "Bidding - Avanço de Etapa", convidado);
                            }
                        }
                    }
                }

                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(e);
            }
        }

        public void AvancarEtapaDois()
        {
            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBidding = new Repositorio.Embarcador.Bidding.BiddingConvite(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioBiddingConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingChecklist repositorioBiddingChecklist = new Repositorio.Embarcador.Bidding.BiddingChecklist(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOferta repositorioBiddingOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(_unitOfWork);

                List<BiddingConvite> listaBidding = repositorioBidding.BuscarPorEtapa(StatusBiddingConvite.Checklist);

                foreach (BiddingConvite bidding in listaBidding)
                {
                    BiddingChecklist biddingChecklist = repositorioBiddingChecklist.BuscarChecklist(bidding);

                    if (biddingChecklist.DataLimite < DateTime.Now)
                    {
                        bidding.Status = StatusBiddingConvite.Ofertas;
                        repositorioBidding.Atualizar(bidding);

                        BiddingOferta oferta = repositorioBiddingOferta.BuscarOferta(bidding);

                        List<BiddingConviteConvidado> listaBiddingConvidado = repositorioBiddingConvidado.BuscarConvidadosPorBiddingEtapa(bidding, new List<StatusBiddingConvite> { StatusBiddingConvite.Checklist, StatusBiddingConvite.Ofertas });

                        foreach (BiddingConviteConvidado convidado in listaBiddingConvidado)
                        {
                            if (convidado.StatusBidding == StatusBiddingConvite.Checklist && biddingChecklist.TipoPreenchimentoChecklist == TipoPreenchimentoChecklist.PreenchimentoObrigatorio)
                            {
                                convidado.Status = StatusBiddingConviteConvidado.Rejeitado;
                                repositorioBiddingConvidado.Atualizar(convidado);
                            }
                            else
                                NotificarConvidado($"O Bidding \"{bidding.Descricao}\" avançou para a etapa de Ofertas.<br />A etapa de Ofertas acaba em {oferta.DataPrazoOferta?.ToString("dd/MM/yyyy HH:mm")}.", "Bidding - Avanço de Etapa", convidado);
                        }
                    }
                }

                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(e);
            }
        }

        public void AvancarEtapaTres()
        {
            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBidding = new Repositorio.Embarcador.Bidding.BiddingConvite(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioBiddingConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOferta repositorioBiddingOferta = new Repositorio.Embarcador.Bidding.BiddingOferta(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(_unitOfWork);

                List<BiddingConvite> listaBidding = repositorioBidding.BuscarPorEtapa(StatusBiddingConvite.Ofertas);

                foreach (BiddingConvite bidding in listaBidding)
                {
                    BiddingOferta biddingOferta = repositorioBiddingOferta.BuscarOferta(bidding);

                    if (biddingOferta.DataLimite < DateTime.Now)
                    {
                        bidding.Status = StatusBiddingConvite.Fechamento;
                        repositorioBidding.Atualizar(bidding);

                        List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> listaTransportadorRota = repositorioTransportadorRota.BuscarPorBiddingStatus(bidding, new List<StatusBiddingRota> { StatusBiddingRota.Aguardando, StatusBiddingRota.NovaRodada });
                        List<BiddingConviteConvidado> listaBiddingConvidado = repositorioBiddingConvidado.BuscarConvidadosPorBiddingEtapa(bidding, new List<StatusBiddingConvite> { StatusBiddingConvite.Ofertas });

                        foreach (Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota transportadorRota in listaTransportadorRota)
                        {
                            transportadorRota.Status = StatusBiddingRota.Rejeitada;
                            repositorioTransportadorRota.Atualizar(transportadorRota);
                        }

                        foreach (BiddingConviteConvidado convidado in listaBiddingConvidado)
                        {
                            convidado.StatusBidding = StatusBiddingConvite.Fechamento;
                            repositorioBiddingConvidado.Atualizar(convidado);
                            NotificarConvidado($"O Bidding \"{bidding.Descricao}\" foi finalizado.", "Bidding - Fechamento", convidado);
                        }
                    }
                }

                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(e);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoRotas()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            #region Origem

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Cidades Origem", Propriedade = "CidadesOrigem", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Clientes Origem", Propriedade = "ClientesOrigem", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Estados Origem", Propriedade = "EstadosOrigem", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Regiões Origem", Propriedade = "RegioesOrigem", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Rotas Origem", Propriedade = "RotasOrigem", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Faixas de CEP Origem", Propriedade = "FaixasCEPOrigem", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Paises Origem", Propriedade = "PaisesOrigem", Tamanho = 100, CampoEntidade = true });

            #endregion Origem

            #region Destino

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Cidades Destino", Propriedade = "CidadesDestino", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Clientes Destino", Propriedade = "ClientesDestino", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Estados Destino", Propriedade = "EstadosDestino", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Regiões Destino", Propriedade = "RegioesDestino", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Rotas Destino", Propriedade = "RotasDestino", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Faixas de CEP Destino", Propriedade = "FaixasCEPDestino", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Paises Destino", Propriedade = "PaisesDestino", Tamanho = 100, CampoEntidade = true });

            #endregion Destino

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "*Descrição", Propriedade = "Descricao", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Frequência Mensal", Propriedade = "FrequenciaMensal", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Volume (caixas expedidas por mês)", Propriedade = "Volume", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Número entregas Mensal", Propriedade = "NumeroEntregasMensal", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "Valor Carga Mês", Propriedade = "ValorCargaMes", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "KM Média da Rota", Propriedade = "KMMediaRota", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = "Peso (ton. por mês)", Propriedade = "Peso", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = "Adicional a partir da entrega N°", Propriedade = "AdicionalPartirEntregaN", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = "*Tipos Carga", Propriedade = "TiposCarga", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = "*Modelos Veiculares", Propriedade = "ModelosVeiculares", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = "Tomador", Propriedade = "Tomador", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = "Grupo Modelo Veicular", Propriedade = "GrupoModeloVeicular", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = "Carroceria Veículo", Propriedade = "CarroceriaVeiculo", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = "Frequência Mensal com Ajudante", Propriedade = "FrequenciaMensalComAjudante", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = "*Quantidade Ajudante Por Veículo", Propriedade = "QuantidadeAjudantesVeiculo", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = "Média Entregas Fracionadas", Propriedade = "MediaEntregasFracionadas", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 32, Descricao = "Máxima Entregas Fracionadas", Propriedade = "MaximaEntregasFracionadas", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 33, Descricao = "Inconterm", Propriedade = "Inconterm", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 35, Descricao = "*Quantidade Viagens por Ano", Propriedade = "QuantidadeViagensAno", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 36, Descricao = "Volume (Ton) Ano", Propriedade = "VolumeToneladaAno", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 37, Descricao = "Volume (Ton) Carga", Propriedade = "VolumeToneladaViagem", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 38, Descricao = "Tempo de Coleta", Propriedade = "TempoColeta", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 39, Descricao = "Tempo de Descarga", Propriedade = "TempoDescarga", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 40, Descricao = "Compressor", Propriedade = "Compressor", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 41, Descricao = "Código de Integração Baseline", Propriedade = "Baseline", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 42, Descricao = "Valor Baseline", Propriedade = "ValorBaseline", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 43, Descricao = "Filiais Participantes", Propriedade = "FiliaisParticipantes", Tamanho = 100, CampoEntidade = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 44, Descricao = "Valor Médio NF-e", Propriedade = "ValorMedioNFe", Tamanho = 100 });

            return configuracoes;
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao> ImportarRotasConvite(HttpRequestBase request, string dados, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhasImportacao = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(request.Params["Dados"]);

            int total = linhasImportacao.Count;

            if (total == 0)
                throw new ServicoException("Nenhuma linha encontrada na planilha");

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> colunas = ConfiguracaoImportacaoRotas();
            Dictionary<string, List<string>> linhas = Servicos.Embarcador.Importacao.Importacao.ObterValoresLinha(colunas, linhasImportacao);

            BiddingImportacao servicoBiddingConviteImportar = new BiddingImportacao(unitOfWork, _cancellationToken);

            DadosImportacaoBidding dadosImportacaoBidding = await servicoBiddingConviteImportar.ObterDadosImportacao(linhas);

            List<dynamic> rotasGrid = new List<dynamic>();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork).BuscarPrimeiroRegistroAsync();

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(request, rotasGrid, (dados) =>
            {
                BiddingImportacao servicoImportar = new BiddingImportacao(unitOfWork, dados, configuracaoIntegracao);

                return servicoImportar.ObterImportacaoRota(dadosImportacaoBidding);
            });

            if (retorno == null)
                throw new ServicoException(Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

            retorno.Retorno = rotasGrid;

            return retorno;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoOferta()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracao = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Protocolo Importação", Propriedade = "ProtocoloImportacao", Tamanho = 100, Obrigatorio = true, CampoEntidade = true });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Modelo Veicular", Propriedade = "ModeloVeicular", Tamanho = 100, Obrigatorio = true, CampoEntidade = true });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Valor Fixo - Sem ICMS", Propriedade = "ValorFixo", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Valor Por Franquia", Propriedade = "ValorFranquia", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "ICMS(%)", Propriedade = "AliquotaICMS", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Pedágio - Sem ICMS", Propriedade = "ValorPedagio", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Valor Fixo Mensal", Propriedade = "ValorFixoMensal", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Ajudante (R$)", Propriedade = "Ajudante", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Adicional Por Entrega", Propriedade = "AdicionalPorEntrega", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Valor por Viagem", Propriedade = "ValorViagem", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Porcentagem sobre Nota", Propriedade = "PorcentagemNota", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Valor KM Rodado", Propriedade = "ValorKmRodado", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Valor por Tonelada", Propriedade = "FreteTonelada", Tamanho = 100 });
            configuracao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Quilometragem", Propriedade = "Quilometragem", Tamanho = 100 });


            return configuracao;
        }


        public async Task<Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao> ImportarOfertasAsync(string dados)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioBiddingTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(_unitOfWork, _cancellationToken);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> colunas = ObterConfiguracaoImportacaoOferta();
            Dictionary<string, List<string>> linhasValores = Servicos.Embarcador.Importacao.Importacao.ObterValoresLinha(colunas, linhas);

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = new();

            if (linhasValores.TryGetValue("ModeloVeicular", out List<string> listaModelos) && listaModelos?.Count > 0)
            {
                List<string> modelos = listaModelos.Select(v => v.Trim()).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> encontrados = new();

                encontrados.AddRange(await repositorioModeloVeicularCarga.BuscarPorCodigosIntegracaoAsync(modelos));

                List<string> naoEncontrados = modelos
                    .Where(v => !encontrados.Any(m => m.CodigoIntegracao == v))
                    .ToList();

                if (naoEncontrados.Count > 0)
                    encontrados.AddRange(await repositorioModeloVeicularCarga.BuscarPorDescricoesAsync(naoEncontrados));

                modelosVeiculares = encontrados.Distinct().ToList();
            }

            List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BiddingTransportadoresRota = new();

            if (linhasValores.TryGetValue("ProtocoloImportacao", out List<string> protocolosIntegracao) && listaModelos?.Count > 0)
            {
                BiddingTransportadoresRota = await repositorioBiddingTransportadorRota.BuscarPorProtocolosImportacaoAsync(protocolosIntegracao.Select(x => x.ToInt()).ToList(), _usuario.Empresa);
            }

            List<OfertaImportacao> ofertas = new List<OfertaImportacao>();

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarOfertaLinha(linhas[i], modelosVeiculares, ofertas, BiddingTransportadoresRota.Distinct().ToList());
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    if (retornoLinha.contar)
                        contador++;

                }
                catch (Dominio.Excecoes.Embarcador.ServicoException ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex.Message, i));
                    continue;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            if (ofertas.Count > 0)
                EnviarOfertas(ofertas, 0, 0, false);

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public void NotificarInteressadosOferta(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, List<string> emailsUsuariosInteressados, string urlAcesso)
        {
            List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

            mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
            {
                Destinatarios = emailsUsuariosInteressados,
                Assunto = $"Bidding {biddingConvite.Descricao}",
                Corpo = $"Olá, <br /> O Bidding número {biddingConvite.Descricao} teve uma nova rodada lançada. <br /> <br /> O Link abaixo permite o acesso ao bidding para avaliação: <br /> Bidding: https://{urlAcesso}/#Bidding/BiddingAvaliacao?tokenAcesso={Servicos.Criptografia.Criptografar(biddingConvite.Codigo.ToString(), "BIDDING-AVALIACAO")}"
            });

            Servicos.Email.EnviarMensagensAsync(mensagens, _unitOfWork);
        }

        public void NotificarInteressadosFechamento(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, List<string> emailsUsuariosInteressados)
        {
            Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repTransportadorOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(_unitOfWork);
            List<BiddingTransportadorRotaOferta> listaVencedores = repTransportadorOferta.BuscarVencedores(biddingConvite.Codigo, 0);

            List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();

            mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
            {
                Destinatarios = emailsUsuariosInteressados,
                Assunto = $"Bidding {biddingConvite.Descricao}",
                Corpo = PreencherCorpoEmailFechamento(listaVencedores, biddingConvite.Descricao)
            });

            Servicos.Email.EnviarMensagensAsync(mensagens, _unitOfWork);

        }

        public void EnviarOfertas(List<OfertaImportacao> ofertas, int codigoOfertaRota, int veiculosVerdes, bool informarVeiculosVerdes)
        {
            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorOferta repOferta = new Repositorio.Embarcador.Bidding.BiddingTransportadorOferta(_unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConviteConvidado repConvidado = new Repositorio.Embarcador.Bidding.BiddingConviteConvidado(_unitOfWork);

                List<BiddingTransportadorRota> transportadoresRota = repTransportadorRota.BuscarPorCodigos(ofertas.Select(x => x.BiddingTransportadorRota).ToList(), false);

                List<BiddingConviteConvidado> convidados = repConvidado.BuscarConvidados(transportadoresRota.FirstOrDefault().Rota.BiddingOferta.BiddingConvite);

                if (convidados.FirstOrDefault().BiddingConvite.Status != StatusBiddingConvite.Ofertas)
                    throw new ServicoException("O bidding selecionado não está na etapa de ofertas.");

                if (DateTime.Now > transportadoresRota.FirstOrDefault().Rota.BiddingOferta.BiddingConvite.DataLimite)
                    throw new ServicoException("Não é possível enviar ofertas, pois a data limite já foi excedida.");

                if (transportadoresRota.FirstOrDefault().Rota.BiddingOferta.DataLimite < DateTime.Now)
                {
                    foreach (var item in convidados)
                    {
                        item.Status = StatusBiddingConviteConvidado.Rejeitado;
                        repConvidado.Atualizar(item);
                        _unitOfWork.Flush();
                    }

                    throw new ServicoException("Prazo esgotado, o convite foi automaticamente rejeitado.");
                }

                bool naoIncluirImpostoICMS = convidados.FirstOrDefault().BiddingConvite.TipoBidding.NaoIncluirImpostoValorTotalOferta;

                List<BiddingTransportadorRotaOferta> rotaofertas = repOferta.BuscarPorCodigosBiddingTransportadorRota(transportadoresRota.Select(x => x.Rota.Codigo).ToList());

                List<BiddingTransportadorRotaOferta> entidadesOfertaInserir = new List<BiddingTransportadorRotaOferta>();

                foreach (var item in ofertas)
                {

                    BiddingTransportadorRota transportadorRota = transportadoresRota.FirstOrDefault(x => x.Codigo == item.BiddingTransportadorRota);

                    transportadorRota.Status = StatusBiddingRota.EmAnalise;
                    transportadorRota.DataRetorno = DateTime.Now;
                    repTransportadorRota.Atualizar(transportadorRota);

                    BiddingTransportadorRotaOferta entidadeOferta = new BiddingTransportadorRotaOferta();

                    if (item.Oferta.Codigo > 0)
                        entidadeOferta = rotaofertas.FirstOrDefault(x => x.Codigo == item.Oferta.Codigo);

                    if (item.NaoOfertar)
                    {
                        entidadeOferta.NaoOfertar = true;
                        entidadeOferta.Rodada = transportadorRota.Rodada;
                        entidadeOferta.TransportadorRota = transportadorRota;
                        entidadeOferta.CustoEstimado = 0;
                        entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                    }
                    else
                    {
                        TipoLanceBidding tipoOferta = item.Tipo;
                        switch (tipoOferta)
                        {
                            case TipoLanceBidding.LanceFrotaFixaFranquia:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LanceFrotaFixaFranquia;
                                entidadeOferta.Quilometragem = item.Oferta.Quilometragem.ToDecimal();
                                entidadeOferta.ValorFixo = item.Oferta.ValorFixo.ToDecimal();
                                entidadeOferta.ValorFranquia = item.Oferta.ValorPorFranquia.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LancePorEquipamento:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LancePorEquipamento;
                                entidadeOferta.ValorFixoEquipamento = item.Oferta.ValorFixo.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LanceFrotaFixaKmRodado:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LanceFrotaFixaKmRodado;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ValorFixoMensal = item.Oferta.ValorFixoMensal.ToDecimal();
                                entidadeOferta.ValorKmRodado = item.Oferta.ValorKmRodado.ToDecimal();
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LancePorcentagemNota:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LancePorcentagemNota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.Porcentagem = item.Oferta.PorcentagemNota.ToDecimal();
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LanceViagemAdicional:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LanceViagemAdicional;
                                entidadeOferta.ValorViagem = item.Oferta.ValorViagem.ToDecimal();
                                entidadeOferta.ValorEntrega = item.Oferta.ValorEntrega.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LancePorPeso:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LancePorPeso;
                                entidadeOferta.ICMSPorcentagem = item.Oferta.ICMS.ToDecimal();
                                entidadeOferta.ReplicarICMSDesteModeloVeicular = item.Oferta.ReplicarICMSDesteModeloVeicular.ToBool();
                                entidadeOferta.FreteTonelada = item.Oferta.FreteTonelada.ToDecimal();
                                entidadeOferta.PedagioParaEixo = item.Oferta.PedagioEixo.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LancePorCapacidade:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LancePorCapacidade;
                                entidadeOferta.ICMSPorcentagem = item.Oferta.ICMS.ToDecimal();
                                entidadeOferta.ReplicarICMSDesteModeloVeicular = item.Oferta.ReplicarICMSDesteModeloVeicular.ToBool();
                                entidadeOferta.FreteTonelada = item.Oferta.FreteTonelada.ToDecimal();
                                entidadeOferta.PedagioParaEixo = item.Oferta.PedagioEixo.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LancePorFreteViagem:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LancePorFreteViagem;
                                entidadeOferta.ICMSPorcentagem = item.Oferta.ICMS.ToDecimal();
                                entidadeOferta.ReplicarICMSDesteModeloVeicular = item.Oferta.ReplicarICMSDesteModeloVeicular.ToBool();
                                entidadeOferta.FreteTonelada = item.Oferta.FreteTonelada.ToDecimal();
                                entidadeOferta.PedagioParaEixo = item.Oferta.PedagioEixo.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            case TipoLanceBidding.LancePorViagemEntregaAjudante:
                                entidadeOferta.Rodada = transportadorRota.Rodada;
                                entidadeOferta.TipoOferta = TipoLanceBidding.LancePorViagemEntregaAjudante;
                                entidadeOferta.ICMSPorcentagem = item.Oferta.ICMS.ToDecimal();
                                entidadeOferta.ReplicarICMSDesteModeloVeicular = item.Oferta.ReplicarICMSDesteModeloVeicular.ToBool();
                                entidadeOferta.FreteTonelada = item.Oferta.FreteTonelada.ToDecimal();
                                entidadeOferta.PedagioParaEixo = item.Oferta.PedagioEixo.ToDecimal();
                                entidadeOferta.Ajudante = item.Oferta.Ajudante.ToDecimal();
                                entidadeOferta.AdicionalPorEntrega = item.Oferta.AdicionalPorEntrega.ToDecimal();
                                entidadeOferta.TransportadorRota = transportadorRota;
                                entidadeOferta.ModeloVeicular = item.ModeloVeicular;
                                entidadeOferta.VeiculosVerdes = veiculosVerdes;
                                entidadeOferta.InformarVeiculosVerdes = informarVeiculosVerdes;
                                break;
                            default:
                                continue;


                        }

                        GerarEstimativaDeCustoOferta(ref entidadeOferta, naoIncluirImpostoICMS);
                        if (item.Oferta.Codigo > 0)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(_auditado, transportadorRota, null, $"Editou a oferta " + transportadorRota.Rota.Descricao + ".", _unitOfWork);
                            repOferta.Atualizar(entidadeOferta);
                        }
                        else
                            entidadesOfertaInserir.Add(entidadeOferta);
                    }

                    AtualizarRanking(transportadorRota.Rota, rotaofertas);
                }

                repOferta.Inserir(entidadesOfertaInserir, "T_BIDDING_TRANSPORTADOR_ROTA_OFERTA", 1000);
                _unitOfWork.CommitChanges();
            }
            catch (BaseException ex)
            {
                _unitOfWork.Rollback();
                throw new ServicoException(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(ex);
                throw;
            }
        }

        public void EnviarRotas(BiddingConvite convite, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.BiddingOfertaRota repRotas = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);
            Repositorio.Embarcador.Bidding.BiddingAceitamentoRota repAceitamentoRota = new Repositorio.Embarcador.Bidding.BiddingAceitamentoRota(unitOfWork);
            List<BiddingOfertaRota> listaRotas = repRotas.BuscarRotasPorBidding(convite);

            foreach (BiddingOfertaRota rota in listaRotas)
            {
                repAceitamentoRota.DeletarPorRotaTransportador(rota.Codigo, usuario.Empresa.Codigo);
                BiddingTransportadorRota transportadorRota = new BiddingTransportadorRota();
                transportadorRota.Rodada = 1;
                transportadorRota.Status = StatusBiddingRota.Aguardando;
                transportadorRota.Rota = rota;
                transportadorRota.Transportador = usuario.Empresa;
                repAceitamentoRota.Inserir(transportadorRota);
            }
        }

        public async Task<StatusBiddingConvite> AutomatizacaoEtapasEmbarcadorAsync(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding, BiddingConvite entidadeBiddingConvite, Repositorio.Embarcador.Bidding.BiddingConviteConvidado repositorioConviteConvidado, Dominio.Entidades.Empresa empresa, TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            if (configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (entidadeBiddingConvite.Status == StatusBiddingConvite.Fechamento)
                        return StatusBiddingConvite.Fechamento;
                    else
                        return StatusBiddingConvite.Ofertas;
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    return await repositorioConviteConvidado.BuscarConvidadoStatusAsync(entidadeBiddingConvite, empresa.Codigo);
                }
            }

            return entidadeBiddingConvite.Status;
        }

        public StatusBiddingConvite AutomatizacaoEtapasTransportador(BiddingConviteConvidado convidado, Dominio.Entidades.Embarcador.Bidding.BiddingConvite grupoBiddingConvite, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding, TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding)
            {
                if (convidado.Status == StatusBiddingConviteConvidado.Aguardando)
                {
                    return StatusBiddingConvite.Aguardando;
                }
                else if (convidado.Status == StatusBiddingConviteConvidado.Aceito)
                {
                    return convidado.StatusBidding;
                }
            }

            return grupoBiddingConvite.Status;
        }

        public BiddingRotaFiltro ProcessarListaRotas(IList<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> listaRotas)
        {
            if (listaRotas == null || !listaRotas.Any())
                return new BiddingRotaFiltro();

            var listaRota = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaFilial = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaMesorregiaoDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaMesorregiaoOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaQuantidadeEntregas = new List<int>();
            var listaQuantidadeAjudantes = new List<int>();
            var listaQuantidadeViagensAno = new List<int>();
            var listaRegiaoDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaRegiaoOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaClienteDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaClienteOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaRotaDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaRotaOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaEstadoDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaEstadoOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaPaisDestino = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaPaisOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();
            var listaModelosVeiculares = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding>();

            bool possuiCEPDestino = false;
            bool possuiCEPOrigem = false;

            foreach (var x in listaRotas)
            {
                listaRota.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.Codigo, Descricao = x.Descricao });

                if (!string.IsNullOrEmpty(x.FiliaisCodigos))
                    listaFilial.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.FiliaisCodigos.ToInt(), Descricao = x.FiliaisDescricoes });

                if (!string.IsNullOrEmpty(x.OrigensCodigos))
                {
                    listaOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.OrigensCodigos.ToInt(), Descricao = x.OrigensDescricoes });
                    listaRegiaoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.OrigensRegiaoCodigo.ToInt(), Descricao = x.OrigensRegiaoDescricao });
                }

                if (!string.IsNullOrEmpty(x.DestinosCodigos))
                {
                    listaDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.DestinosCodigos.ToInt(), Descricao = x.DestinosDescricoes });
                    listaRegiaoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.DestinosRegiaoCodigo.ToInt(), Descricao = x.DestinosRegiaoDescricao });
                }

                if (!string.IsNullOrEmpty(x.MesorregioesDestinoCodigo))
                    listaMesorregiaoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.MesorregioesDestinoCodigo.ToInt(), Descricao = x.MesorregioesDestino });

                if (!string.IsNullOrEmpty(x.MesorregioesOrigemCodigo))
                    listaMesorregiaoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.MesorregioesOrigemCodigo.ToInt(), Descricao = x.MesorregioesOrigem });

                if (x.QuantidadeEntregas > 0)
                    listaQuantidadeEntregas.Add(x.QuantidadeEntregas);

                if (x.QuantidadeAjudantes > 0)
                    listaQuantidadeAjudantes.Add(x.QuantidadeAjudantes);

                if (x.QuantidadeViagensAno > 0)
                    listaQuantidadeViagensAno.Add(x.QuantidadeViagensAno);

                if (!string.IsNullOrEmpty(x.RegioesDestinoCodigos))
                    listaRegiaoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.RegioesDestinoCodigos.ToInt(), Descricao = x.RegioesDestinoDescricoes });

                if (!string.IsNullOrEmpty(x.RegioesOrigemCodigos))
                    listaRegiaoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.RegioesOrigemCodigos.ToInt(), Descricao = x.RegioesOrigemDescricoes });

                if (!string.IsNullOrEmpty(x.ClienteDestinoCodigo))
                {
                    listaClienteDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.ClienteDestinoCodigo.ToInt(), Descricao = x.ClienteDestino });
                    listaRegiaoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.ClienteDestinoRegiaoCodigo.ToInt(), Descricao = x.ClienteDestinoRegiaoDescricao });
                }

                if (!string.IsNullOrEmpty(x.ClienteOrigemCodigo))
                {
                    listaClienteOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.ClienteOrigemCodigo.ToInt(), Descricao = x.ClienteOrigem });
                    listaRegiaoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.ClienteOrigemRegiaoCodigo.ToInt(), Descricao = x.ClienteOrigemRegiaoDescricao });
                }

                if (!string.IsNullOrEmpty(x.RotaDestinoCodigo))
                {
                    listaRotaDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.RotaDestinoCodigo.ToInt(), Descricao = x.RotaDestino });
                    listaRegiaoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.RotaDestinoRegiaoCodigo.ToInt(), Descricao = x.RotaDestinoRegiaoDescricao });
                }

                if (!string.IsNullOrEmpty(x.ClienteOrigemCodigo))
                {
                    listaRotaOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.ClienteOrigemCodigo.ToInt(), Descricao = x.ClienteOrigem });
                    listaRegiaoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.RotaOrigemRegiaoCodigo.ToInt(), Descricao = x.RotaOrigemRegiaoDescricao });
                }


                if (!string.IsNullOrEmpty(x.EstadoDestinoCodigo))
                {
                    listaEstadoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.EstadoDestinoCodigo.ToInt(), Descricao = x.EstadoDestino });
                    listaRegiaoDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.EstadoDestinoRegiaoCodigo.ToInt(), Descricao = x.EstadoDestinoRegiaoDescricao });
                }


                if (!string.IsNullOrEmpty(x.EstadoOrigemCodigo))
                {
                    listaEstadoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.EstadoOrigemCodigo.ToInt(), Descricao = x.EstadoOrigem });
                    listaRegiaoOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.EstadoOrigemRegiaoCodigo.ToInt(), Descricao = x.EstadoOrigemRegiaoDescricao });
                }

                if (!string.IsNullOrEmpty(x.PaisDestinoCodigo))
                    listaPaisDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.PaisDestinoCodigo.ToInt(), Descricao = x.PaisDestino });

                if (!string.IsNullOrEmpty(x.PaisOrigemCodigo))
                    listaPaisOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.PaisOrigemCodigo.ToInt(), Descricao = x.PaisOrigem });

                if (x.PossuiCEPDestino > 0) possuiCEPDestino = true;

                if (x.PossuiCEPOrigem > 0) possuiCEPOrigem = true;

                if (!string.IsNullOrEmpty(x.ModelosVeicularesCodigos))
                    listaModelosVeiculares.Add(new Dominio.ObjetosDeValor.Embarcador.Bidding.LocalidesBidding { Codigo = x.ModelosVeicularesCodigos.ToInt(), Descricao = x.ModelosVeicularesDescricoes });
            }

            return new BiddingRotaFiltro
            {
                ListaRotas = listaRota.DistinctBy(x => x.Descricao).ToList(),
                ListaFilial = listaFilial.DistinctBy(x => x.Descricao).ToList(),
                ListaOrigem = listaOrigem.DistinctBy(x => x.Descricao).ToList(),
                ListaDestino = listaDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaMesorregiaoDestino = listaMesorregiaoDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaMesorregiaoOrigem = listaMesorregiaoOrigem.DistinctBy(x => x.Descricao).ToList(),
                ListaQuantidadeEntregas = System.Linq.Enumerable.Distinct(listaQuantidadeEntregas).ToList(),
                ListaQuantidadeAjudantes = System.Linq.Enumerable.Distinct(listaQuantidadeAjudantes).ToList(),
                ListaQuantidadeViagensAno = System.Linq.Enumerable.Distinct(listaQuantidadeViagensAno).ToList(),
                ListaRegiaoDestino = listaRegiaoDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaRegiaoOrigem = listaRegiaoOrigem.DistinctBy(x => x.Descricao).ToList(),
                ListaClienteDestino = listaClienteDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaClienteOrigem = listaClienteOrigem.DistinctBy(x => x.Descricao).ToList(),
                ListaRotaDestino = listaRotaDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaRotaOrigem = listaRotaOrigem.DistinctBy(x => x.Descricao).ToList(),
                ListaEstadoDestino = listaEstadoDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaEstadoOrigem = listaEstadoOrigem.DistinctBy(x => x.Descricao).ToList(),
                ListaPaisDestino = listaPaisDestino.DistinctBy(x => x.Descricao).ToList(),
                ListaPaisOrigem = listaPaisOrigem.DistinctBy( x => x.Descricao).ToList(),
                PossuiCEPDestino = possuiCEPDestino,
                PossuiCEPOrigem = possuiCEPOrigem,
                ListaModelosVeiculares = listaModelosVeiculares.DistinctBy(x => x.Descricao).ToList(),
                FiltrosString = listaRotas.ToJson()
            };
        }

        public List<RegiaoBrasil> ObterRegiaoBrasilRota(BiddingOfertaRota rota, Dominio.Enumeradores.OpcaoSimNao origem)
        {
            List<RegiaoBrasil> regioesBrasil = new List<RegiaoBrasil>();

            Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOrigemDestino consultaBidding = ObterObjetoConsultaBidding(rota, origem);

            regioesBrasil.AddRange(consultaBidding.Localidades
                .Where(localidade => localidade.Estado != null && localidade.Estado.RegiaoBrasil != null)
                .Select(localidade => localidade.Estado.RegiaoBrasil)
                .ToList());

            regioesBrasil.AddRange(consultaBidding.Estados
                .Where(estado => estado.RegiaoBrasil != null)
                .Select(estado => estado.RegiaoBrasil)
                .ToList());

            regioesBrasil.AddRange(consultaBidding.Clientes
                .Where(cliente => cliente.Localidade.Estado.RegiaoBrasil != null)
                .Select(cliente => cliente.Localidade.Estado.RegiaoBrasil)
                .ToList());

            regioesBrasil.AddRange(consultaBidding.Regioes.SelectMany(regiao => regiao.Localidades)
                .Where(localidade => localidade.Estado.RegiaoBrasil != null)
                .Select(localidade => localidade.Estado.RegiaoBrasil)
                .ToList());

            regioesBrasil.AddRange(consultaBidding.RotasFrete.SelectMany(obj => obj.LocalidadesOrigem)
                .Where(localidade => localidade.Estado.RegiaoBrasil != null)
                .Select(localidade => localidade.Estado.RegiaoBrasil)
                .ToList());

            return regioesBrasil
                .GroupBy(obj => obj.Codigo)
                .Select(group => group.First())
                .ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Bidding.ConsultaBiddingOrigemDestino ObterObjetoConsultaBidding(BiddingOfertaRota rota, Dominio.Enumeradores.OpcaoSimNao origem)
        {
            ConsultaBiddingOrigemDestino consultaBiddingOrigemDestino = new ConsultaBiddingOrigemDestino();

            if (origem == Dominio.Enumeradores.OpcaoSimNao.Sim || origem == Dominio.Enumeradores.OpcaoSimNao.Todos)
            {
                consultaBiddingOrigemDestino.Localidades.AddRange(rota.Origens ?? new List<Localidade>());
                consultaBiddingOrigemDestino.Estados.AddRange(rota.EstadosOrigem ?? new List<Estado>());
                consultaBiddingOrigemDestino.Regioes.AddRange(rota.RegioesOrigem ?? new List<Regiao>());
                consultaBiddingOrigemDestino.Clientes.AddRange(rota.ClientesOrigem ?? new List<Dominio.Entidades.Cliente>());
                consultaBiddingOrigemDestino.RotasFrete.AddRange(rota.RotasOrigem ?? new List<RotaFrete>());
            }

            if (origem == Dominio.Enumeradores.OpcaoSimNao.Nao || origem == Dominio.Enumeradores.OpcaoSimNao.Todos)
            {
                consultaBiddingOrigemDestino.Localidades.AddRange(rota.Destinos ?? new List<Localidade>());
                consultaBiddingOrigemDestino.Estados.AddRange(rota.EstadosDestino ?? new List<Estado>());
                consultaBiddingOrigemDestino.Regioes.AddRange(rota.RegioesDestino ?? new List<Regiao>());
                consultaBiddingOrigemDestino.Clientes.AddRange(rota.ClientesDestino ?? new List<Dominio.Entidades.Cliente>());
                consultaBiddingOrigemDestino.RotasFrete.AddRange(rota.RotasDestino ?? new List<RotaFrete>());
            }

            return consultaBiddingOrigemDestino;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha()
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarOfertaLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares, List<OfertaImportacao> ofertas, List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BiddingTransportadoresRota)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProtocoloImportacao = (from obj in linha.Colunas where obj.NomeCampo == "ProtocoloImportacao" select obj).FirstOrDefault();
            int protocoloImportacao = 0;
            if (colProtocoloImportacao?.Valor != null && !string.IsNullOrWhiteSpace((string)colProtocoloImportacao.Valor))
            {
                string strProtocoloImportacao = (string)colProtocoloImportacao.Valor;
                int.TryParse(strProtocoloImportacao, out protocoloImportacao);
            }
            else
                throw new ServicoException("É necessário informar o Protocolo Importação.");

            Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota biddingTransportadorRota = BiddingTransportadoresRota.FirstOrDefault(x => x.Rota.ProtocoloImportacao == protocoloImportacao);

            if (biddingTransportadorRota == null)
                throw new ServicoException("Protocolo Importação não corresponde a nenhuma rota.");

            if (!(biddingTransportadorRota.Status == StatusBiddingRota.NovaRodada || biddingTransportadorRota.Status == StatusBiddingRota.Aguardando) && biddingTransportadorRota.Rodada != 1)
                throw new ServicoException("Essa rota não está com Status de Nova Rodada.");

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicular = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicular" select obj).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;
            if (colModeloVeicular?.Valor != null && !string.IsNullOrWhiteSpace((string)colModeloVeicular.Valor))
            {

                modeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorCodigoIntegracao((string)colModeloVeicular.Valor, modelosVeiculares);

                if (modeloVeicularCarga == null)
                    modeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorDescricao((string)colModeloVeicular.Valor, modelosVeiculares);
            }
            if (modeloVeicularCarga == null)
                throw new ServicoException("Modelo Veicular não encontrado.");

            if (!biddingTransportadorRota.Rota.ModelosVeiculares.Contains(modeloVeicularCarga))
                throw new ServicoException("Não é possível ofertar para o modelo veícular informado.");

            bool naoOfertar = false;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFixo = (from obj in linha.Colunas where obj.NomeCampo == "ValorFixo" select obj).FirstOrDefault();
            decimal valorFixo = 0;
            if (colValorFixo != null)
            {
                if (string.IsNullOrWhiteSpace((string)colValorFixo.Valor))
                    naoOfertar = true;
                else
                    if (colValorFixo?.Valor != null) decimal.TryParse((string)colValorFixo.Valor, out valorFixo);
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAliquotaICMS = (from obj in linha.Colunas where obj.NomeCampo == "AliquotaICMS" select obj).FirstOrDefault();
            decimal aliquotaICMS = 0;
            if (colAliquotaICMS?.Valor != null && !string.IsNullOrWhiteSpace((string)colAliquotaICMS.Valor))
                decimal.TryParse((string)colAliquotaICMS.Valor, out aliquotaICMS);
            else
                aliquotaICMS = biddingTransportadorRota.Rota.AlicotaPadraoICMS ?? 0;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorPedagio = (from obj in linha.Colunas where obj.NomeCampo == "ValorPedagio" select obj).FirstOrDefault();
            decimal valorPedagio = 0;
            if (colValorPedagio?.Valor != null && !string.IsNullOrWhiteSpace((string)colValorPedagio.Valor))
                decimal.TryParse((string)colValorPedagio.Valor, out valorPedagio);

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFranquia = (from obj in linha.Colunas where obj.NomeCampo == "ValorFranquia" select obj).FirstOrDefault();
            decimal valorFranquia = 0;
            if (colValorFranquia?.Valor != null && !string.IsNullOrWhiteSpace((string)colValorFranquia.Valor))
                decimal.TryParse((string)colValorFranquia.Valor, out valorFranquia);

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAjudante = (from obj in linha.Colunas where obj.NomeCampo == "Ajudante" select obj).FirstOrDefault();
            decimal ajudante = 0;
            if (colAjudante?.Valor != null && !string.IsNullOrWhiteSpace((string)colAjudante.Valor))
                decimal.TryParse((string)colAjudante.Valor, out ajudante);

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFixoMensal = (from obj in linha.Colunas where obj.NomeCampo == "ValorFixoMensal" select obj).FirstOrDefault();
            decimal valorFixoMensal = 0;
            if (colValorFixoMensal != null)
            {
                if (string.IsNullOrWhiteSpace((string)colValorFixoMensal.Valor))
                    naoOfertar = true;
                else
                    if (colValorFixoMensal?.Valor != null) decimal.TryParse((string)colValorFixoMensal.Valor, out valorFixoMensal);
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicionalPorEntrega = (from obj in linha.Colunas where obj.NomeCampo == "AdicionalPorEntrega" select obj).FirstOrDefault();
            decimal adicionalPorEntrega = 0;
            if (colAdicionalPorEntrega?.Valor != null && !string.IsNullOrWhiteSpace((string)colAdicionalPorEntrega.Valor))
                decimal.TryParse((string)colAdicionalPorEntrega.Valor, out adicionalPorEntrega);

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorViagem = (from obj in linha.Colunas where obj.NomeCampo == "ValorViagem" select obj).FirstOrDefault();
            decimal valorViagem = 0;
            if (colValorViagem != null)
            {
                if (string.IsNullOrWhiteSpace((string)colValorViagem.Valor))
                    naoOfertar = true;
                else
                    if (colValorViagem?.Valor != null) decimal.TryParse((string)colValorViagem.Valor, out valorViagem);
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPorcentagemNota = (from obj in linha.Colunas where obj.NomeCampo == "PorcentagemNota" select obj).FirstOrDefault();
            decimal porcentagemNota = 0;
            if (colPorcentagemNota != null)
            {
                if (string.IsNullOrWhiteSpace((string)colPorcentagemNota.Valor))
                    naoOfertar = true;
                else
                    if (colPorcentagemNota?.Valor != null) decimal.TryParse((string)colPorcentagemNota.Valor, out porcentagemNota);
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorKmRodado = (from obj in linha.Colunas where obj.NomeCampo == "ValorKmRodado" select obj).FirstOrDefault();
            decimal valorKmRodado = 0;
            if (colValorKmRodado?.Valor != null && !string.IsNullOrWhiteSpace((string)colValorKmRodado.Valor))
                decimal.TryParse((string)colValorKmRodado.Valor, out valorKmRodado);

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFreteTonelada = (from obj in linha.Colunas where obj.NomeCampo == "FreteTonelada" select obj).FirstOrDefault();
            decimal freteTonelada = 0;
            if (colFreteTonelada != null)
            {
                if (string.IsNullOrWhiteSpace((string)colFreteTonelada.Valor))
                    naoOfertar = true;
                else
                    if (colFreteTonelada?.Valor != null) decimal.TryParse((string)colFreteTonelada.Valor, out freteTonelada);
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuilometragem = (from obj in linha.Colunas where obj.NomeCampo == "Quilometragem" select obj).FirstOrDefault();
            decimal quilometragem = 0;
            if (colQuilometragem?.Valor != null && !string.IsNullOrWhiteSpace((string)colQuilometragem.Valor))
                decimal.TryParse((string)colQuilometragem.Valor, out quilometragem);

            if (biddingTransportadorRota.Rota.BiddingOferta.TipoLance == TipoLanceBidding.LancePorViagemEntregaAjudante || biddingTransportadorRota.Rota.BiddingOferta.TipoLance == TipoLanceBidding.LancePorFreteViagem)
                freteTonelada = valorFixo;
            //Esses dois tipos de lance utilizam essa propriedade para o calculo, porem diferentemente das outras que utilizam, não há multiplicação, funciona exatamente como o frete fixo, porém visualmente na importação vai confundir o usuário


            OfertaImportacao oferta = new OfertaImportacao()
            {
                Tipo = biddingTransportadorRota.Rota.BiddingOferta.TipoLance,
                BiddingTransportadorRota = biddingTransportadorRota.Codigo,
                NaoOfertar = naoOfertar,
                ModeloVeicular = modeloVeicularCarga,
                Oferta = new OfertaImportacaoDados()
                {
                    Codigo = 0,
                    Quilometragem = quilometragem.ToString(),
                    ValorFixo = valorFixo.ToString(),
                    ValorFranquia = valorFranquia.ToString(),
                    ModeloVeicular = modeloVeicularCarga.Codigo,
                    ValorFixoMensal = valorFixoMensal.ToString(),
                    ValorKmRodado = valorKmRodado.ToString(),
                    PorcentagemNota = porcentagemNota.ToString(),
                    ValorViagem = valorViagem.ToString(),
                    ValorEntrega = adicionalPorEntrega.ToString(),
                    ICMS = aliquotaICMS.ToString(),
                    ReplicarICMSDesteModeloVeicular = "0",
                    FreteTonelada = freteTonelada.ToString(),
                    PedagioEixo = valorPedagio.ToString(),
                    Ajudante = ajudante.ToString(),
                    AdicionalPorEntrega = adicionalPorEntrega.ToString()
                }
            };

            ofertas.Add(oferta);

            //AdicionalPorEntrega e ValorEntrega são duas propriedades diferentes, com basicamente a mesma finalidade, porém criar duas colunas na importação com o mesmo nome iria confundir o usuário.
            //ReplicarICMSDesteModeloVeicular precisa ser sempre falso na importação
            //Codigo da Oferta = 0, pois foi implementada uma opção de editar ofertas, fazendo com que quebre quando vindo da importação, por não ter essa informação no dynamic montado.

            ValidarInformacoesPorTipoOferta(oferta);

            return RetornarSucessoLinha();
        }

        private void ValidarInformacoesPorTipoOferta(dynamic oferta)
        {
            if ((bool)oferta.NaoOfertar)
                return;

            TipoLanceBidding tipoOferta = oferta.Tipo;
            switch (tipoOferta)
            {
                case TipoLanceBidding.LanceFrotaFixaFranquia:
                case TipoLanceBidding.LancePorEquipamento:
                case TipoLanceBidding.LancePorFreteViagem:
                case TipoLanceBidding.LancePorViagemEntregaAjudante:
                    if (((string)oferta.Oferta.ValorFixo).ToDecimal() == 0)
                        throw new ServicoException("É obrigatório informar o Valor Fixo");
                    break;
                case TipoLanceBidding.LanceFrotaFixaKmRodado:
                    if (((string)oferta.Oferta.ValorFixoMensal).ToDecimal() == 0)
                        throw new ServicoException("É obrigatório informar o Valor Fixo Mensal");
                    break;
                case TipoLanceBidding.LancePorcentagemNota:
                    if (((string)oferta.Oferta.PorcentagemNota).ToDecimal() == 0)
                        throw new ServicoException("É obrigatório informar a Porcentagem sobre Nota Fiscal");
                    break;
                case TipoLanceBidding.LanceViagemAdicional:
                    if (((string)oferta.Oferta.ValorViagem).ToDecimal() == 0)
                        throw new ServicoException("É obrigatório informar o Valor por Viagem");
                    break;
                case TipoLanceBidding.LancePorPeso:
                case TipoLanceBidding.LancePorCapacidade:
                    if (((string)oferta.Oferta.FreteTonelada).ToDecimal() == 0)
                        throw new ServicoException("Para Lance Por Peso/Capacidade é obrigatório preencher o campo Valor por Tonelada");
                    break;
            }
        }

        private static string PreencherCorpoEmailFechamento(List<BiddingTransportadorRotaOferta> listaVencedores, string descricaoBidding)
        {
            System.Text.StringBuilder corpoEmail = new System.Text.StringBuilder();
            corpoEmail.AppendLine($"</span><span style=\"width: 100%; display: inline-block\">Ola, </span>");
            corpoEmail.AppendLine($"</span><span style=\"width: 100%; display: inline-block\">O Bidding {descricaoBidding} foi finalizado e a titularidade das rotas foi definida. </span>");
            corpoEmail.AppendLine("<table style =\"margin: 30px 0 30px 0; border: 1px solid #b9b5b5; border-collapse: collapse; border-collapse: collapse;\">");
            corpoEmail.AppendLine("<thead style=\"background-color: #d9e1f2; color: black;\">");
            corpoEmail.AppendLine("<tr>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Vencedor </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Rota  </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Modelo Veícular </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Oferta </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Tipo da Oferta </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Rodada </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Titularidade </th>");
            corpoEmail.AppendLine("</tr>");
            corpoEmail.AppendLine("</thead>");
            corpoEmail.AppendLine("<tbody>");
            foreach (BiddingTransportadorRotaOferta vencedor in listaVencedores)
            {
                corpoEmail.AppendLine("<tr>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.TransportadorRota.Transportador.NomeFantasia ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.TransportadorRota.Rota.Descricao ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.ModeloVeicular.Descricao ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.DescricaoOferta ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.DescricaoTipoOferta ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.TransportadorRota.Rodada.ToString() ?? ""}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{vencedor.TipoTransportador?.ObterDescricao() ?? ""}</td>");
                corpoEmail.AppendLine("</tr>");
            }
            corpoEmail.AppendLine("</tbody>");
            corpoEmail.AppendLine("</table>");

            return corpoEmail.ToString();
        }

        #endregion Métodos Privados
    }
}
