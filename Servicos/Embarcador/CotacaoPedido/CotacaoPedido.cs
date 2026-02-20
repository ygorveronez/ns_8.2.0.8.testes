using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CotacaoPedido
{
    public class CotacaoPedido : ServicoBase
    {
        #region Construtores

        public CotacaoPedido(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public static List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> VerificarRegrasCotacaoPedido(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido repRegrasCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaRegras = new List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido>();
            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaFiltrada = new List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido>();

            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaRegraTipoCarga = null;
            if (cotacaoPedido.TipoDeCarga != null)
            {
                listaRegraTipoCarga = repRegrasCotacaoPedido.AlcadasPorTipoCarga(cotacaoPedido.TipoDeCarga.Codigo, cotacaoPedido.Previsao.HasValue ? cotacaoPedido.Previsao.Value.Date : DateTime.Now.Date);
                listaRegras.AddRange(listaRegraTipoCarga);
            }

            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaRegraTipoOperacao = null;
            if (cotacaoPedido.TipoOperacao != null)
            {
                listaRegraTipoOperacao = repRegrasCotacaoPedido.AlcadasPorTipoOperacao(cotacaoPedido.TipoOperacao.Codigo, cotacaoPedido.Previsao.HasValue ? cotacaoPedido.Previsao.Value.Date : DateTime.Now.Date);
                listaRegras.AddRange(listaRegraTipoOperacao);
            }

            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaRegraValor = repRegrasCotacaoPedido.AlcadasPorValor(cotacaoPedido.ValorTotalCotacao, cotacaoPedido.Previsao.HasValue ? cotacaoPedido.Previsao.Value.Date : DateTime.Now.Date);
            listaRegras.AddRange(listaRegraValor);

            if (listaRegras.Distinct().Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorTipoCarga && cotacaoPedido.TipoDeCarga != null)
                    {
                        bool valido = false;
                        if (regra.RegrasCotacaoPedidoTipoCarga.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeCarga.Codigo == cotacaoPedido.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoTipoCarga.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeCarga.Codigo == cotacaoPedido.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoTipoCarga.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeCarga.Codigo != cotacaoPedido.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoTipoCarga.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeCarga.Codigo != cotacaoPedido.TipoDeCarga.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTipoOperacao && cotacaoPedido.TipoOperacao != null)
                    {
                        bool valido = false;
                        if (regra.RegrasCotacaoPedidoTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoOperacao.Codigo == cotacaoPedido.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoOperacao.Codigo == cotacaoPedido.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoOperacao.Codigo != cotacaoPedido.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoOperacao.Codigo != cotacaoPedido.TipoOperacao.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValorFrete)
                    {
                        bool valido = false;
                        if (regra.RegrasCotacaoPedidoValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Valor == cotacaoPedido.ValorTotalCotacao))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Valor == cotacaoPedido.ValorTotalCotacao))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Valor != cotacaoPedido.ValorTotalCotacao))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Valor != cotacaoPedido.ValorTotalCotacao))
                            valido = true;
                        if (regra.RegrasCotacaoPedidoValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && cotacaoPedido.ValorTotalCotacao >= o.Valor))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && cotacaoPedido.ValorTotalCotacao >= o.Valor))
                            valido = true;
                        if (regra.RegrasCotacaoPedidoValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && cotacaoPedido.ValorTotalCotacao <= o.Valor))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && cotacaoPedido.ValorTotalCotacao <= o.Valor))
                            valido = true;
                        if (regra.RegrasCotacaoPedidoValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && cotacaoPedido.ValorTotalCotacao > o.Valor))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && cotacaoPedido.ValorTotalCotacao > o.Valor))
                            valido = true;
                        if (regra.RegrasCotacaoPedidoValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && cotacaoPedido.ValorTotalCotacao < o.Valor))
                            valido = true;
                        else if (regra.RegrasCotacaoPedidoValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && cotacaoPedido.ValorTotalCotacao < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                }
            }

            return listaFiltrada;
        }

        public static void CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaFiltrada, Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regra in listaFiltrada)
            {
                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao autorizacao = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao();
                    autorizacao.CotacaoPedido = cotacaoPedido;
                    autorizacao.Usuario = aprovador;
                    autorizacao.RegrasCotacaoPedido = regra;
                    autorizacao.EtapaAutorizacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia;
                    autorizacao.Data = DateTime.Now;
                    repCotacaoPedidoAutorizacao.Inserir(autorizacao);

                    string nota = string.Empty;
                    nota = string.Format(Localization.Resources.Pedidos.Pedido.UsuarioSolicitouLiberacaoCotacaoPedido, usuario.Nome, cotacaoPedido.Numero.ToString("n0"), cotacaoPedido.ValorTotalCotacao.ToString("n2"));
                    serNotificacao.GerarNotificacao(aprovador, usuario, (int)cotacaoPedido.Codigo, "Cotacoes/CotacaoPedido", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                }
            }

        }

        public static void VerificarSituacaoCotacaoPedido(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
            if (cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
            {
                // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao repCotacaoPedidoAutorizacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoAutorizacao(unitOfWork);
                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido repConfiguracaoCotacaoPedidoTipoOperacao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

                // Busca todas regras da ocorrencia (Distintas)
                List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> regras = repCotacaoPedidoAutorizacao.BuscarRegrasPedido(cotacaoPedido.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cotacaoPedido.TipoOperacao != null ? repTipoOperacao.BuscarPorCodigo(cotacaoPedido.TipoOperacao.Codigo) : null;
                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido configuracaoCotacaoPedidoTipoOperacao = (tipoOperacao != null && tipoOperacao.ConfiguracaoCotacaoPedido != null) ? repConfiguracaoCotacaoPedidoTipoOperacao.BuscarPorCodigo(tipoOperacao.ConfiguracaoCotacaoPedido.Codigo, false) : null;

                // Flag de rejeicao
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regra in regras)
                {
                    // Quantidade de usuarios que marcaram como aprovado ou rejeitado
                    int pendentes = repCotacaoPedidoAutorizacao.ContarPendentes(cotacaoPedido.Codigo, regra.Codigo); // P

                    // Quantidade de aprovacoes
                    int aprovacoes = repCotacaoPedidoAutorizacao.ContarAprovacoesOcorrencia(cotacaoPedido.Codigo, regra.Codigo); // A

                    int rejeitadas = repCotacaoPedidoAutorizacao.ContarRejeitadas(cotacaoPedido.Codigo, regra.Codigo); // R

                    // Numero de aprovacoes minimas
                    int necessariosParaAprovar = regra.NumeroAprovadores; // N

                    if (rejeitadas > 0)
                        rejeitada = true; // Se uma regra foi reprovada, a carga ocorrencia é reprovada
                    else if (aprovacoes < necessariosParaAprovar) // A >= N -> Aprovacoes > NumeroMinimo
                        aprovada = false; // Se nao esta rejeitada e nem reprovada, esta pendente (nao faz nada)
                }

                // Define situacao da ocorrencia
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao;

                // Rejeicao na autorizacao
                if (rejeitada && (cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente))
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado;

                // Se houve alteracao de status, atualiza etapa da ocorencia
                if (rejeitada || aprovada)
                {
                    // Verifica se a situacao e ag aprovacao para testar a regra de etapa ag emissao
                    if ((cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || cotacaoPedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente) && !rejeitada)
                    {
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado;
                    }

                    // Seta a nova situacao
                    cotacaoPedido.SituacaoPedido = situacao;

                    if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado)
                    {
                        bool liberarCriacaoPedido = tipoOperacao == null || configuracaoCotacaoPedidoTipoOperacao == null || (configuracaoCotacaoPedidoTipoOperacao != null && !configuracaoCotacaoPedidoTipoOperacao.HabilitaInformarDadosDosPedidosNaCotacao);

                        //if (cotacaoPedido.TipoClienteCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteNovo || cotacaoPedido.TipoClienteCotacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteProspect)
                        //    throw new Exception("Não é possível aprovar uma cotação sem antes cadastrar o cliente.");
                        //else
                        //{

                        if (liberarCriacaoPedido)
                        {
                            string retorno = Servicos.Embarcador.CotacaoPedido.CotacaoPedido.CriarPedido(cotacaoPedido, unitOfWork, usuario, tipoServicoMultisoftware, stringConexao, auditado);
                            if (!string.IsNullOrWhiteSpace(retorno))
                                throw new Exception(retorno);
                        }
                        //}
                    }

                    // Define icone
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                    if (rejeitada)
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                    else
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                    // Emite notificação
                    string mensagem = string.Format(Localization.Resources.Pedidos.Pedido.CotacaoPedidoValorFoi, cotacaoPedido.Numero.ToString("n0"), cotacaoPedido.ValorTotalCotacao.ToString("n2"), (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));
                    serNotificacao.GerarNotificacao(cotacaoPedido.Usuario, usuario, (int)cotacaoPedido.Codigo, "Cotacoes/CotacaoPedido", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                }
            }
        }

        public static string CriarPedido(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            var configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork).BuscarConfiguracaoPadrao();

            string retorno = string.Empty;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoImportacao repPedidoImportacao = new Repositorio.Embarcador.Pedidos.PedidoImportacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao repConfiguracaoCotacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao configuracaoCotacao = repConfiguracaoCotacao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = null;

            if (cotacaoPedido.EnderecoDestino != null)
            {
                enderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco()
                {
                    Bairro = cotacaoPedido.EnderecoDestino.Bairro,
                    CEP = cotacaoPedido.EnderecoDestino.CEP,
                    ClienteOutroEndereco = cotacaoPedido.EnderecoDestino.ClienteOutroEndereco,
                    Complemento = cotacaoPedido.EnderecoDestino.Complemento,
                    Endereco = cotacaoPedido.EnderecoDestino.Endereco,
                    IE_RG = cotacaoPedido.EnderecoDestino.IE_RG,
                    Localidade = cotacaoPedido.EnderecoDestino.Localidade,
                    Numero = cotacaoPedido.EnderecoDestino.Numero,
                    Telefone = cotacaoPedido.EnderecoDestino.Telefone
                };
                repPedidoEndereco.Inserir(enderecoDestino);
            }
            if (cotacaoPedido.EnderecoOrigem != null)
            {
                enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco()
                {
                    Bairro = cotacaoPedido.EnderecoOrigem.Bairro,
                    CEP = cotacaoPedido.EnderecoOrigem.CEP,
                    ClienteOutroEndereco = cotacaoPedido.EnderecoOrigem.ClienteOutroEndereco,
                    Complemento = cotacaoPedido.EnderecoOrigem.Complemento,
                    Endereco = cotacaoPedido.EnderecoOrigem.Endereco,
                    IE_RG = cotacaoPedido.EnderecoOrigem.IE_RG,
                    Localidade = cotacaoPedido.EnderecoOrigem.Localidade,
                    Numero = cotacaoPedido.EnderecoOrigem.Numero,
                    Telefone = cotacaoPedido.EnderecoOrigem.Telefone
                };
                repPedidoEndereco.Inserir(enderecoOrigem);
            }

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido()
            {
                AdicionadaManualmente = true,
                Ajudante = cotacaoPedido.Ajudante,
                ArmadorImportacao = cotacaoPedido.ArmadorImportacao,
                BairroEntregaImportacao = cotacaoPedido.BairroEntregaImportacao,
                BaseCalculoICMS = 0,
                Blocos = null,
                CargasPedido = null,
                CanalEntrega = null,
                CodigoCargaEmbarcador = string.Empty,
                CodigoPedidoCliente = configuracaoPedido.NaoLevarNumeroCotacaoParaPedidoGerado == false ? cotacaoPedido.Numero.ToString() : null,
                ColetaEmProdutorRural = false,
                CEPEntregaImportacao = cotacaoPedido.CEPEntregaImportacao,
                ClienteDeslocamento = null,
                Cotacao = false,
                CotacaoPedido = cotacaoPedido,
                CubagemTotal = cotacaoPedido.CubagemTotal,
                DataFinalViagemExecutada = null,
                DataFinalViagemFaturada = null,
                DataInicialViagemExecutada = null,
                DataInicialViagemFaturada = null,
                DataPrevisaoChegadaDestinatario = null,
                DataPrevisaoSaidaDestinatario = null,
                DataCarregamentoPedido = cotacaoPedido.Previsao,
                DataCriacao = DateTime.Now.Date,
                DataFinalColeta = cotacaoPedido.DataFinalColeta,
                DataInicialColeta = cotacaoPedido.DataInicialColeta,
                DataPrevisaoSaida = cotacaoPedido.DataInicialColeta,
                DataVencimentoArmazenamentoImportacao = cotacaoPedido.DataVencimentoArmazenamentoImportacao,
                Destinatario = cotacaoPedido.Destinatario,
                Destino = cotacaoPedido.Destinatario != null && cotacaoPedido.Destinatario.Localidade != null ? cotacaoPedido.Destinatario.Localidade : cotacaoPedido.Destino,
                DespachoTransitoAduaneiro = false,
                Empresa = cotacaoPedido.Empresa,
                EmpresaSerie = null,
                EnderecoDestino = enderecoDestino,
                EnderecoEntregaImportacao = cotacaoPedido.EnderecoEntregaImportacao,
                EnderecoOrigem = enderecoOrigem,
                EscoltaArmada = cotacaoPedido.EscoltaArmada,
                EscoltaMunicipal = false,
                EtapaPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Pedido,
                Expedidor = cotacaoPedido.Expedidor,
                Filial = null,
                GerarAutomaticamenteCargaDoPedido = true,
                GerarUmCTEPorNFe = false,
                GerenciamentoRisco = cotacaoPedido.GerenciamentoRisco,
                GrupoPessoas = cotacaoPedido.GrupoPessoas,
                ImpostoNegociado = false,
                ImprimirObservacaoCTe = false,
                IncluirBaseCalculoICMS = false,
                LacreContainerDois = string.Empty,
                LacreContainerTres = string.Empty,
                LacreContainerUm = string.Empty,
                LocalidadeEntregaImportacao = cotacaoPedido.LocalidadeEntregaImportacao,
                ModeloVeicularCarga = cotacaoPedido.ModeloVeicularCarga,
                MotivoCancelamento = string.Empty,
                Motoristas = null,
                NecessarioReentrega = false,
                NotasFiscais = null,
                Numero = repPedido.BuscarProximoNumero(),
                NumeroBL = cotacaoPedido.NumeroBL,
                NumeroContainer = cotacaoPedido.NumeroContainer,
                NumeroNavio = cotacaoPedido.NumeroNavio,
                NumeroNotaCliente = cotacaoPedido.QuantidadeNotas,
                NumeroPaletes = cotacaoPedido.NumeroPaletes,
                NumeroPaletesFracionado = 0,
                NumeroPedidoEmbarcador = configuracaoPedido.NaoLevarNumeroCotacaoParaPedidoGerado == false ? cotacaoPedido.Numero.ToString() : null,
                Observacao = cotacaoPedido.Observacao,
                ObservacaoCTe = string.Empty,
                ObservacaoCTeTerceiro = string.Empty,
                ObservacaoInterna = (configuracaoCotacao?.GravarNumeroCotacaoObservacaoInternaAoCriarPedido ?? false ) ? string.Concat("Cotação número: ", cotacaoPedido.Numero.ToString()," - ", cotacaoPedido.ObservacaoInterna) : cotacaoPedido.ObservacaoInterna,
                Ordem = string.Empty,
                Origem = cotacaoPedido.Origem,
                PedidoAutorizacao = null,
                PedidoImportacao = null,
                PedidoIntegradoEmbarcador = false,
                PedidoNotasParciais = null,
                PedidoRedespachoTotalmenteCarregado = false,
                PedidoRefaturado = false,
                PedidoRefaturamento = false,
                PedidosRecolhimentoTroca = null,
                PedidoSubContratado = false,
                PedidoTotalmenteCarregado = false,
                PedidoTransbordo = false,
                PercentualAliquota = 0,
                PercentualInclusaoBC = 0,
                PesoCubado = cotacaoPedido.PesoCubado,
                PesoSaldoRestante = cotacaoPedido.PesoTotal,
                PesoTotal = cotacaoPedido.PesoTotal,
                PesoTotalPaletes = 0,
                PontoReferenciaImportacao = cotacaoPedido.PontoReferenciaImportacao,
                Porto = null,//cotacaoPedido.Porto,
                PreCarga = null,
                PrevisaoEntrega = cotacaoPedido.Previsao,
                ProdutoPredominante = cotacaoPedido.Produto?.Descricao ?? string.Empty,
                Produtos = null,
                QtdAjudantes = cotacaoPedido.QtdAjudantes,
                QtdEntregas = cotacaoPedido.QtdEntregas,
                QtVolumes = cotacaoPedido.QtVolumes,
                SaldoVolumesRestante = cotacaoPedido.QtVolumes,
                Rastreado = cotacaoPedido.Rastreado,
                Recebedor = cotacaoPedido.Recebedor,
                Remetente = cotacaoPedido.ClienteAtivo != null ? cotacaoPedido.ClienteAtivo : cotacaoPedido.ClienteInativo != null ? cotacaoPedido.ClienteInativo : null,
                Requisitante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta.Remetente,
                ResponsavelRedespacho = null,
                RestricoesDescarga = null,
                RotaFrete = null,
                Seguro = false,
                SenhaAgendamento = string.Empty,
                SenhaAgendamentoCliente = string.Empty,
                SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto,
                SituacaoPlanejamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedido.Pendente,
                SubContratante = null,
                TaraContainer = string.Empty,
                Temperatura = cotacaoPedido.Temperatura,
                TipoCarga = null,
                TipoColeta = null,
                TipoDeCarga = cotacaoPedido.TipoDeCarga,
                TipoOperacao = cotacaoPedido.TipoOperacao,
                TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal,
                TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar,
                TipoPessoa = cotacaoPedido.GrupoPessoas != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa,
                TipoTerminalImportacao = cotacaoPedido.TipoTerminalImportacao,
                TipoTomador = cotacaoPedido.UsarTipoTomadorCotacaoPedido ? cotacaoPedido.TipoTomador : Dominio.Enumeradores.TipoTomador.Remetente,
                Tomador = cotacaoPedido.UsarTipoTomadorCotacaoPedido && cotacaoPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? cotacaoPedido.Tomador : null,
                UsarTipoTomadorPedido = cotacaoPedido.UsarTipoTomadorCotacaoPedido,
                UltimaAtualizacao = DateTime.Now,
                UsarOutroEnderecoDestino = cotacaoPedido.UsarOutroEnderecoDestino,
                UsarOutroEnderecoOrigem = cotacaoPedido.UsarOutroEnderecoOrigem,
                UsarTipoPagamentoNF = false,
                Usuario = usuario,
                Autor = usuario,
                ValorFreteAReceber = cotacaoPedido.ValorTotalCotacao,
                ValorFreteCotado = cotacaoPedido.ValorTotalCotacao,
                ValorFreteNegociado = cotacaoPedido.ValorFrete,
                ValorICMS = 0,
                ValorTotalCarga = 0,
                ValorTotalNotasFiscais = 0,
                ValorTotalPaletes = 0,
                Veiculos = null,
                SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta,
                ValorFreteTransportadorTerceiro = cotacaoPedido.ValorFreteTerceiro,

            };

            if (Servicos.Embarcador.Pedido.Pedido.TomadorPossuiPendenciaFinanceira(pedido, tipoServicoMultisoftware, out string mensagemErro))
                return mensagemErro;

            repPedido.Inserir(pedido);

            pedido.Protocolo = pedido.Codigo;

            SalvarComponentesFrete(cotacaoPedido, pedido, unitOfWork);

            repPedido.Atualizar(pedido);

            if (cotacaoPedido.Produto != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtos = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                {
                    AlturaCM = 0,
                    ComprimentoCM = 0,
                    LarguraCM = 0,
                    MetroCubico = 0,
                    Observacao = string.Empty,
                    Pedido = pedido,
                    PedidoProdutoONUs = null,
                    PesoTotalEmbalagem = 0,
                    PesoUnitario = 0,
                    Produto = cotacaoPedido.Produto,
                    Quantidade = 0,
                    QuantidadeEmbalagem = 0,
                    QuantidadePalet = 0,
                    ValorProduto = 0
                };
                repPedidoProduto.Inserir(produtos);
            }
            if (cotacaoPedido.CotacaoPedidoImportacao != null && cotacaoPedido.CotacaoPedidoImportacao.Count > 0)
            {
                foreach (var imp in cotacaoPedido.CotacaoPedidoImportacao)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao importacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao()
                    {
                        CodigoImportacao = imp.CodigoImportacao,
                        CodigoReferencia = imp.CodigoReferencia,
                        NumeroDI = imp.NumeroDI,
                        Pedido = pedido,
                        Peso = imp.Peso,
                        ValorCarga = imp.ValorCarga,
                        Volume = imp.Volume
                    };
                    repPedidoImportacao.Inserir(importacao);
                }

            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Gerou pedido a partir da cotação de número " + cotacaoPedido.Numero.ToString(), unitOfWork);

            return retorno;
        }

        public static string CriarPedido(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, decimal pesoLiquido, int numeroPaletes, decimal cubagem, int unidades, DateTime dataColeta)
        {
            var configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork).BuscarConfiguracaoPadrao();

            string retorno = string.Empty;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoImportacao repPedidoImportacao = new Repositorio.Embarcador.Pedidos.PedidoImportacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao repConfiguracaoCotacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao configuracaoCotacao = repConfiguracaoCotacao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = null;

            if (cotacaoPedido.EnderecoDestino != null)
            {
                enderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco()
                {
                    Bairro = cotacaoPedido.EnderecoDestino.Bairro,
                    CEP = cotacaoPedido.EnderecoDestino.CEP,
                    ClienteOutroEndereco = cotacaoPedido.EnderecoDestino.ClienteOutroEndereco,
                    Complemento = cotacaoPedido.EnderecoDestino.Complemento,
                    Endereco = cotacaoPedido.EnderecoDestino.Endereco,
                    IE_RG = cotacaoPedido.EnderecoDestino.IE_RG,
                    Localidade = cotacaoPedido.EnderecoDestino.Localidade,
                    Numero = cotacaoPedido.EnderecoDestino.Numero,
                    Telefone = cotacaoPedido.EnderecoDestino.Telefone
                };
                repPedidoEndereco.Inserir(enderecoDestino);
            }
            if (cotacaoPedido.EnderecoOrigem != null)
            {
                enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco()
                {
                    Bairro = cotacaoPedido.EnderecoOrigem.Bairro,
                    CEP = cotacaoPedido.EnderecoOrigem.CEP,
                    ClienteOutroEndereco = cotacaoPedido.EnderecoOrigem.ClienteOutroEndereco,
                    Complemento = cotacaoPedido.EnderecoOrigem.Complemento,
                    Endereco = cotacaoPedido.EnderecoOrigem.Endereco,
                    IE_RG = cotacaoPedido.EnderecoOrigem.IE_RG,
                    Localidade = cotacaoPedido.EnderecoOrigem.Localidade,
                    Numero = cotacaoPedido.EnderecoOrigem.Numero,
                    Telefone = cotacaoPedido.EnderecoOrigem.Telefone
                };
                repPedidoEndereco.Inserir(enderecoOrigem);
            }

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido()
            {
                AdicionadaManualmente = true,
                Ajudante = cotacaoPedido.Ajudante,
                ArmadorImportacao = cotacaoPedido.ArmadorImportacao,
                BairroEntregaImportacao = cotacaoPedido.BairroEntregaImportacao,
                BaseCalculoICMS = 0,
                Blocos = null,
                CargasPedido = null,
                CanalEntrega = null,
                CodigoCargaEmbarcador = string.Empty,
                CodigoPedidoCliente = configuracaoPedido.NaoLevarNumeroCotacaoParaPedidoGerado == false ? cotacaoPedido.Numero.ToString() : null,
                ColetaEmProdutorRural = false,
                CEPEntregaImportacao = cotacaoPedido.CEPEntregaImportacao,
                ClienteDeslocamento = null,
                Cotacao = false,
                CotacaoPedido = cotacaoPedido,
                CubagemTotal = cotacaoPedido.CubagemTotal,
                DataFinalViagemExecutada = null,
                DataFinalViagemFaturada = null,
                DataInicialViagemExecutada = null,
                DataInicialViagemFaturada = null,
                DataPrevisaoChegadaDestinatario = null,
                DataPrevisaoSaidaDestinatario = null,
                DataCarregamentoPedido = dataColeta,
                DataCriacao = DateTime.Now.Date,
                DataFinalColeta = cotacaoPedido.DataFinalColeta,
                DataInicialColeta = cotacaoPedido.DataInicialColeta,
                DataPrevisaoSaida = cotacaoPedido.DataInicialColeta,
                DataVencimentoArmazenamentoImportacao = cotacaoPedido.DataVencimentoArmazenamentoImportacao,
                Destinatario = cotacaoPedido.Destinatario,
                Destino = cotacaoPedido.Destinatario != null && cotacaoPedido.Destinatario.Localidade != null ? cotacaoPedido.Destinatario.Localidade : cotacaoPedido.Destino,
                DespachoTransitoAduaneiro = false,
                Empresa = cotacaoPedido.Empresa,
                EmpresaSerie = null,
                EnderecoDestino = enderecoDestino,
                EnderecoEntregaImportacao = cotacaoPedido.EnderecoEntregaImportacao,
                EnderecoOrigem = enderecoOrigem,
                EscoltaArmada = cotacaoPedido.EscoltaArmada,
                EscoltaMunicipal = false,
                EtapaPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Pedido,
                Expedidor = cotacaoPedido.Expedidor,
                Filial = null,
                GerarAutomaticamenteCargaDoPedido = true,
                GerarUmCTEPorNFe = false,
                GerenciamentoRisco = cotacaoPedido.GerenciamentoRisco,
                GrupoPessoas = cotacaoPedido.GrupoPessoas,
                ImpostoNegociado = false,
                ImprimirObservacaoCTe = false,
                IncluirBaseCalculoICMS = false,
                LacreContainerDois = string.Empty,
                LacreContainerTres = string.Empty,
                LacreContainerUm = string.Empty,
                LocalidadeEntregaImportacao = cotacaoPedido.LocalidadeEntregaImportacao,
                ModeloVeicularCarga = cotacaoPedido.ModeloVeicularCarga,
                MotivoCancelamento = string.Empty,
                Motoristas = null,
                NecessarioReentrega = false,
                NotasFiscais = null,
                Numero = repPedido.BuscarProximoNumero(),
                NumeroBL = cotacaoPedido.NumeroBL,
                NumeroContainer = cotacaoPedido.NumeroContainer,
                NumeroNavio = cotacaoPedido.NumeroNavio,
                NumeroNotaCliente = cotacaoPedido.QuantidadeNotas,
                NumeroPaletes = numeroPaletes,
                NumeroPaletesFracionado = 0,
                NumeroPedidoEmbarcador = configuracaoPedido.NaoLevarNumeroCotacaoParaPedidoGerado == false ? cotacaoPedido.Numero.ToString() : null,
                Observacao = cotacaoPedido.Observacao,
                ObservacaoCTe = string.Empty,
                ObservacaoCTeTerceiro = string.Empty,
                ObservacaoInterna = (configuracaoCotacao?.GravarNumeroCotacaoObservacaoInternaAoCriarPedido ?? false) ? string.Concat("Cotação número: ", cotacaoPedido.Numero.ToString(), " - ", cotacaoPedido.ObservacaoInterna) : cotacaoPedido.ObservacaoInterna,
                Ordem = string.Empty,
                Origem = cotacaoPedido.Origem,
                PedidoAutorizacao = null,
                PedidoImportacao = null,
                PedidoIntegradoEmbarcador = false,
                PedidoNotasParciais = null,
                PedidoRedespachoTotalmenteCarregado = false,
                PedidoRefaturado = false,
                PedidoRefaturamento = false,
                PedidosRecolhimentoTroca = null,
                PedidoSubContratado = false,
                PedidoTotalmenteCarregado = false,
                PedidoTransbordo = false,
                PercentualAliquota = 0,
                PercentualInclusaoBC = 0,
                PesoCubado = cubagem,
                PesoSaldoRestante = cotacaoPedido.PesoTotal,
                PesoTotal = pesoLiquido,
                PesoTotalPaletes = 0,
                PontoReferenciaImportacao = cotacaoPedido.PontoReferenciaImportacao,
                Porto = null,//cotacaoPedido.Porto,
                PreCarga = null,
                PrevisaoEntrega = cotacaoPedido.Previsao,
                ProdutoPredominante = cotacaoPedido.Produto?.Descricao ?? string.Empty,
                Produtos = null,
                QtdAjudantes = cotacaoPedido.QtdAjudantes,
                QtdEntregas = unidades,
                QtVolumes = cotacaoPedido.QtVolumes,
                SaldoVolumesRestante = cotacaoPedido.QtVolumes,
                Rastreado = cotacaoPedido.Rastreado,
                Recebedor = cotacaoPedido.Recebedor,
                Remetente = cotacaoPedido.ClienteAtivo != null ? cotacaoPedido.ClienteAtivo : cotacaoPedido.ClienteInativo != null ? cotacaoPedido.ClienteInativo : null,
                Requisitante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta.Remetente,
                ResponsavelRedespacho = null,
                RestricoesDescarga = null,
                RotaFrete = null,
                Seguro = false,
                SenhaAgendamento = string.Empty,
                SenhaAgendamentoCliente = string.Empty,
                SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto,
                SituacaoPlanejamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedido.Pendente,
                SubContratante = null,
                TaraContainer = string.Empty,
                Temperatura = cotacaoPedido.Temperatura,
                TipoCarga = null,
                TipoColeta = null,
                TipoDeCarga = cotacaoPedido.TipoDeCarga,
                TipoOperacao = cotacaoPedido.TipoOperacao,
                TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal,
                TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar,
                TipoPessoa = cotacaoPedido.GrupoPessoas != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa,
                TipoTerminalImportacao = cotacaoPedido.TipoTerminalImportacao,
                TipoTomador = cotacaoPedido.UsarTipoTomadorCotacaoPedido ? cotacaoPedido.TipoTomador : Dominio.Enumeradores.TipoTomador.Remetente,
                Tomador = cotacaoPedido.UsarTipoTomadorCotacaoPedido && cotacaoPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? cotacaoPedido.Tomador : null,
                UsarTipoTomadorPedido = cotacaoPedido.UsarTipoTomadorCotacaoPedido,
                UltimaAtualizacao = DateTime.Now,
                UsarOutroEnderecoDestino = cotacaoPedido.UsarOutroEnderecoDestino,
                UsarOutroEnderecoOrigem = cotacaoPedido.UsarOutroEnderecoOrigem,
                UsarTipoPagamentoNF = false,
                Usuario = usuario,
                Autor = usuario,
                ValorFreteAReceber = cotacaoPedido.ValorTotalCotacao,
                ValorFreteCotado = cotacaoPedido.ValorTotalCotacao,
                ValorFreteNegociado = cotacaoPedido.ValorFrete,
                ValorICMS = 0,
                ValorTotalCarga = 0,
                ValorTotalNotasFiscais = 0,
                ValorTotalPaletes = 0,
                Veiculos = null,
                SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta,
                ValorFreteTransportadorTerceiro = cotacaoPedido.ValorFreteTerceiro,
            };

            if (Servicos.Embarcador.Pedido.Pedido.TomadorPossuiPendenciaFinanceira(pedido, tipoServicoMultisoftware, out string mensagemErro))
                return mensagemErro;

            repPedido.Inserir(pedido);

            pedido.Protocolo = pedido.Codigo;

            SalvarComponentesFrete(cotacaoPedido, pedido, unitOfWork);

            repPedido.Atualizar(pedido);

            if (cotacaoPedido.Produto != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtos = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                {
                    AlturaCM = 0,
                    ComprimentoCM = 0,
                    LarguraCM = 0,
                    MetroCubico = 0,
                    Observacao = string.Empty,
                    Pedido = pedido,
                    PedidoProdutoONUs = null,
                    PesoTotalEmbalagem = 0,
                    PesoUnitario = 0,
                    Produto = cotacaoPedido.Produto,
                    Quantidade = 0,
                    QuantidadeEmbalagem = 0,
                    QuantidadePalet = 0,
                    ValorProduto = 0
                };
                repPedidoProduto.Inserir(produtos);
            }
            if (cotacaoPedido.CotacaoPedidoImportacao != null && cotacaoPedido.CotacaoPedidoImportacao.Count > 0)
            {
                foreach (var imp in cotacaoPedido.CotacaoPedidoImportacao)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao importacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao()
                    {
                        CodigoImportacao = imp.CodigoImportacao,
                        CodigoReferencia = imp.CodigoReferencia,
                        NumeroDI = imp.NumeroDI,
                        Pedido = pedido,
                        Peso = imp.Peso,
                        ValorCarga = imp.ValorCarga,
                        Volume = imp.Volume
                    };
                    repPedidoImportacao.Inserir(importacao);
                }

            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Gerou pedido a partir da cotação de número " + cotacaoPedido.Numero.ToString(), unitOfWork);

            return retorno;
        }

        private static void SalvarComponentesFrete(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente repCotacaoPedidoComponente = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> componentesCotacaoPedido = repCotacaoPedidoComponente.BuscarPorCotacao(cotacaoPedido.Codigo);

            foreach (var componenteFrete in componentesCotacaoPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componenteFretePedido = null;

                int codigo = 0;
                
                componenteFretePedido = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

                componenteFretePedido.Pedido = pedido;
                componenteFretePedido.ComponenteFilialEmissora = false;
                componenteFretePedido.ComponenteFrete = repComponenteFrete.BuscarPorCodigo((int)componenteFrete.ComponenteFrete.Codigo);
                componenteFretePedido.TipoComponenteFrete = componenteFretePedido.ComponenteFrete.TipoComponenteFrete;

                if (componenteFretePedido.ComponenteFrete.ImprimirOutraDescricaoCTe)
                    componenteFretePedido.OutraDescricaoCTe = componenteFretePedido.ComponenteFrete.DescricaoCTe;

                if (componenteFretePedido.ComponenteFrete.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
                {
                    componenteFretePedido.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                    componenteFretePedido.IncluirIntegralmenteContratoFreteTerceiro = false;
                    componenteFretePedido.Percentual = componenteFrete.Percentual;
                    componenteFretePedido.ValorComponente = (componenteFretePedido.Percentual * pedido.ValorTotalNotasFiscais);
                }
                else
                {
                    componenteFretePedido.IncluirIntegralmenteContratoFreteTerceiro = false;
                    componenteFretePedido.Percentual = 0;
                    componenteFretePedido.ValorComponente = componenteFrete.Valor;
                }


                repPedidoComponenteFrete.Inserir(componenteFretePedido);
            }
        }

        #endregion
    }
}
