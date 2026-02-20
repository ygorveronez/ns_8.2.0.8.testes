using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaLocaisPrestacao : ServicoBase
    {

        public CargaLocaisPrestacao(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga VerificarEAjustarLocaisPrestacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosbase, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool cargaDeRetorno = false)
        {
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaPedidosbase.Where(obj => !obj.PedidoPallet).OrderBy(coleta => coleta.OrdemColeta).ThenBy(entrega => entrega.OrdemEntrega).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisAtual = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            bool existePercursoCarga = repCargaPercurso.ExistePorCarga(carga.Codigo),
                 podeAlterar = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!serCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes, tipoServicoMultisoftware, true))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = CriarLocalPrestacao(cargaPedido, cargaLocaisPrestacao, unitOfWork, out bool criou);

                    if (criou || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        podeAlterar = true;

                    if (cargaLocalPrestacao != null)
                    {
                        if (!cargaLocaisAtual.Contains(cargaLocalPrestacao))
                            cargaLocaisAtual.Add(cargaLocalPrestacao);
                    }
                }
                else
                {
                    podeAlterar = true;

                    RecriarLocaisPrestacaoPorNotasFiscais(carga, cargaPedido, unitOfWork, ref cargaLocaisAtual, cargaDeRetorno);
                }
            }

            if (RemoverLocaisPrestacaoNaoExistemMais(carga, cargaLocaisAtual, unitOfWork))
                podeAlterar = true;

            if (AdicionarLocaisPrestacaoConformePedido(cargaPedidos, cargaLocaisAtual, tipoServicoMultisoftware, unitOfWork))
                podeAlterar = false;

            if (podeAlterar)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga retornoRotas = serCargaRota.CriarRotaPorLocaisDePrestacao(carga, cargaPedidos, unitOfWork);

                if (retornoRotas.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida)
                    serCargaPedido.SetarDestinatarioFinalCarga(carga, cargaPedidos, unitOfWork, configuracaoPedido);

                return retornoRotas;
            }

            return null;
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga> VerificarEAjustarLocaisPrestacaoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosbase, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool cargaDeRetorno = false)
        {
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaPedidosbase.Where(obj => !obj.PedidoPallet).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = await repCargaLocaisPrestacao.BuscarPorCargaAsync(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisAtual = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            bool podeAlterar = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!serCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes, tipoServicoMultisoftware, true))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = CriarLocalPrestacao(cargaPedido, cargaLocaisPrestacao, unitOfWork, out bool criou);

                    if (criou || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        podeAlterar = true;

                    if (cargaLocalPrestacao != null && !cargaLocaisAtual.Contains(cargaLocalPrestacao))
                        cargaLocaisAtual.Add(cargaLocalPrestacao);
                }
                else
                {
                    podeAlterar = true;

                    RecriarLocaisPrestacaoPorNotasFiscais(carga, cargaPedido, unitOfWork, ref cargaLocaisAtual, cargaDeRetorno);
                }
            }

            if (RemoverLocaisPrestacaoNaoExistemMais(carga, cargaLocaisAtual, unitOfWork))
                podeAlterar = true;

            if (AdicionarLocaisPrestacaoConformePedido(cargaPedidos, cargaLocaisAtual, tipoServicoMultisoftware, unitOfWork))
                podeAlterar = false;

            if (podeAlterar)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga retornoRotas = serCargaRota.CriarRotaPorLocaisDePrestacao(carga, cargaPedidos, unitOfWork);

                if (retornoRotas.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida)
                    serCargaPedido.SetarDestinatarioFinalCarga(carga, cargaPedidos, unitOfWork, configuracaoPedido);

                return retornoRotas;
            }

            return null;
        }

        public bool AdicionarLocaisPrestacaoConformePedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisAtual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (cargaPedidos.Count != 1 || cargaLocaisAtual.Count != 1)
                    return false;
            }

            if (cargaPedidos.Count == 0)
                return false;

            Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repPedidoLocaisPrestacaoPassagens = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[0];
            //Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = cargaLocaisAtual.LastOrDefault();

            bool existe = false;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens> pedidoLocaisPrestacaoPassagens = repPedidoLocaisPrestacaoPassagens.BuscarPorPedido(cargaPedido.Pedido.Codigo);

            if (pedidoLocaisPrestacaoPassagens.Count > 0)
                existe = true;

            foreach (var cargaLocalPrestacao in cargaLocaisAtual)
            {
                if (repCargaLocaisPrestacaoPassagens.ExistePorLocalPrestacao(cargaLocalPrestacao.Codigo))
                    return false;

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens pedidoLocalPrestacaoPassagem in pedidoLocaisPrestacaoPassagens)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cargaPedidos.Count > 0
                        && pedidoLocalPrestacaoPassagem.EstadoDePassagem.Sigla == cargaLocalPrestacao.LocalidadeTerminoPrestacao.Estado.Sigla)
                        break;

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens()
                    {
                        CargaLocaisPrestacao = cargaLocalPrestacao,
                        EstadoDePassagem = pedidoLocalPrestacaoPassagem.EstadoDePassagem,
                        Posicao = pedidoLocalPrestacaoPassagem.Posicao
                    };

                    repCargaLocaisPrestacaoPassagens.Inserir(cargaLocalPrestacaoPassagem);
                }

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    break;
            }

            return existe;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga VerificarEAjustarLocaisPrestacaoPorCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
            RecriarLocaisPrestacaoPorCTes(carga, cargaCTes, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga retornoRotas = serCargaRota.CriarRotaPorLocaisDePrestacao(carga, cargaPedidos, unitOfWork);
            serCargaPedido.SetarDestinatarioFinalCarga(carga, cargaPedidos, unitOfWork, configuracaoPedido);
            return retornoRotas;
        }

        public void RecriarLocaisPrestacaoPorCTes(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> listaCargaLocaisPrestacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao localExistente = (from obj in listaCargaLocaisPrestacao where obj.LocalidadeInicioPrestacao == cargaCTe.CTe.LocalidadeInicioPrestacao && obj.LocalidadeTerminoPrestacao == cargaCTe.CTe.LocalidadeTerminoPrestacao select obj).FirstOrDefault();
                if (localExistente == null)
                {
                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novoLocal = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                    novoLocal.Carga = carga;
                    novoLocal.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                    novoLocal.LocalidadeInicioPrestacao = cargaCTe.CTe.LocalidadeInicioPrestacao;
                    novoLocal.LocalidadeTerminoPrestacao = cargaCTe.CTe.LocalidadeTerminoPrestacao;
                    listaCargaLocaisPrestacao.Add(novoLocal);
                }

            }
            PreecherLocaisDePrestacaoPorDocumentoFiscal(carga, listaCargaLocaisPrestacao, unitOfWork);
        }

        public void RecriarLocaisPrestacaoPorNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosbase, Repositorio.UnitOfWork unitOfWork, bool cargaDeRetorno = false)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> listaCargaLocaisPrestacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaPedidosbase.Where(obj => !obj.PedidoPallet).ToList();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisNotasSaidas = (from obj in pedidoXMLNotasFiscais where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida select obj).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisNotasEntrada = (from obj in pedidoXMLNotasFiscais where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlSaidas in pedidoXMLNotasFiscaisNotasSaidas)
                {
                    Dominio.Entidades.Localidade localidadeOrigem;
                    if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null)
                        localidadeOrigem = cargaPedido.Pedido.EnderecoOrigem.Localidade;
                    else
                    {
                        if (cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor &&
                        cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor &&
                        !cargaPedido.Pedido.PedidoTransbordo)
                            localidadeOrigem = cargaDeRetorno ? xmlSaidas.XMLNotaFiscal.Destinatario.Localidade : xmlSaidas.XMLNotaFiscal.Emitente.Localidade;
                        else
                            localidadeOrigem = cargaPedido.Origem;
                    }

                    Dominio.Entidades.Localidade localidadeDestino;
                    if (cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null)
                        localidadeDestino = cargaPedido.Pedido.EnderecoDestino.Localidade;
                    else
                        localidadeDestino = cargaDeRetorno ? xmlSaidas.XMLNotaFiscal.Emitente.Localidade : xmlSaidas.XMLNotaFiscal.Destinatario.Localidade;

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao localExistente = (from obj in listaCargaLocaisPrestacao where obj.LocalidadeInicioPrestacao == localidadeOrigem && obj.LocalidadeTerminoPrestacao == localidadeDestino select obj).FirstOrDefault();
                    if (localExistente == null)
                    {
                        var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novoLocal = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                        novoLocal.Carga = carga;
                        novoLocal.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                        novoLocal.LocalidadeInicioPrestacao = localidadeOrigem;
                        novoLocal.LocalidadeTerminoPrestacao = localidadeDestino;
                        listaCargaLocaisPrestacao.Add(novoLocal);
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlEntrada in pedidoXMLNotasFiscaisNotasEntrada)//quando é importação de mercadoria é uma nota de entrada por isso é feita essa validação
                {
                    Dominio.Entidades.Localidade localidadeOrigem;
                    if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null)
                        localidadeOrigem = cargaPedido.Pedido.EnderecoOrigem.Localidade;
                    else
                    {
                        if (cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor &&
                            cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor &&
                            !cargaPedido.Pedido.PedidoTransbordo)
                            localidadeOrigem = cargaDeRetorno ? xmlEntrada.XMLNotaFiscal.Emitente.Localidade : xmlEntrada.XMLNotaFiscal.Destinatario.Localidade;
                        else
                            localidadeOrigem = cargaPedido.Origem;
                    }

                    Dominio.Entidades.Localidade localidadeDestino;
                    if (cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null)
                        localidadeDestino = cargaPedido.Pedido.EnderecoDestino.Localidade;
                    else
                        localidadeDestino = cargaDeRetorno ? xmlEntrada.XMLNotaFiscal.Destinatario.Localidade : xmlEntrada.XMLNotaFiscal.Emitente.Localidade;

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao localExistente = (from obj in listaCargaLocaisPrestacao where obj.LocalidadeTerminoPrestacao == localidadeDestino && obj.LocalidadeInicioPrestacao == localidadeOrigem select obj).FirstOrDefault();
                    if (localExistente == null)
                    {
                        var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novoLocal = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                        novoLocal.Carga = cargaPedido.Carga;
                        novoLocal.LocalidadeTerminoPrestacao = xmlEntrada.XMLNotaFiscal.Emitente.Localidade;
                        novoLocal.LocalidadeInicioPrestacao = localidadeOrigem;
                        novoLocal.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                        listaCargaLocaisPrestacao.Add(novoLocal);
                    }
                }
            }
            PreecherLocaisDePrestacaoPorDocumentoFiscal(carga, listaCargaLocaisPrestacao, unitOfWork);
        }

        public void RecriarLocaisPrestacaoPorNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, ref List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> listaCargaLocaisPrestacao, bool cargaDeRetorno = false)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);

            if (cargaPedido.PedidoPallet)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisNotasSaidas = pedidoXMLNotasFiscais.Where(obj => obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisNotasEntrada = pedidoXMLNotasFiscais.Where(obj => obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlSaidas in pedidoXMLNotasFiscaisNotasSaidas)
            {
                Dominio.Entidades.Localidade localidadeOrigem;
                if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null)
                    localidadeOrigem = cargaPedido.Pedido.EnderecoOrigem.Localidade;
                else
                {
                    if (cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor &&
                        cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor &&
                        !cargaPedido.Pedido.PedidoTransbordo)
                        localidadeOrigem = cargaDeRetorno ? xmlSaidas.XMLNotaFiscal.Destinatario.Localidade : xmlSaidas.XMLNotaFiscal.Emitente.Localidade;
                    else
                        localidadeOrigem = cargaPedido.Origem;
                }

                Dominio.Entidades.Localidade localidadeDestino;
                if (cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null)
                    localidadeDestino = cargaPedido.Pedido.EnderecoDestino.Localidade;
                else
                    localidadeDestino = cargaDeRetorno ? xmlSaidas.XMLNotaFiscal.Emitente.Localidade : xmlSaidas.XMLNotaFiscal.Destinatario.Localidade;

                if (!listaCargaLocaisPrestacao.Any(obj => obj.LocalidadeInicioPrestacao?.Codigo == localidadeOrigem?.Codigo && obj.LocalidadeTerminoPrestacao?.Codigo == localidadeDestino?.Codigo))
                {

                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novoLocal = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                    novoLocal.Carga = carga;
                    novoLocal.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                    novoLocal.LocalidadeInicioPrestacao = localidadeOrigem;
                    novoLocal.LocalidadeTerminoPrestacao = localidadeDestino;

                    repCargaLocaisPrestacao.Inserir(novoLocal);
                    listaCargaLocaisPrestacao.Add(novoLocal);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlEntrada in pedidoXMLNotasFiscaisNotasEntrada)//quando é importação de mercadoria é uma nota de entrada por isso é feita essa validação
            {
                Dominio.Entidades.Localidade localidadeOrigem;
                if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null)
                    localidadeOrigem = cargaPedido.Pedido.EnderecoOrigem.Localidade;
                else
                {
                    if (cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor &&
                        cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor &&
                        !cargaPedido.Pedido.PedidoTransbordo)
                        localidadeOrigem = cargaDeRetorno ? xmlEntrada.XMLNotaFiscal.Emitente.Localidade : xmlEntrada.XMLNotaFiscal.Destinatario.Localidade;
                    else
                        localidadeOrigem = cargaPedido.Origem;
                }

                Dominio.Entidades.Localidade localidadeDestino;
                if (cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null)
                    localidadeDestino = cargaPedido.Pedido.EnderecoDestino.Localidade;
                else
                    localidadeDestino = cargaDeRetorno ? xmlEntrada.XMLNotaFiscal.Destinatario.Localidade : xmlEntrada.XMLNotaFiscal.Emitente.Localidade;

                if (!listaCargaLocaisPrestacao.Any(obj => obj.LocalidadeInicioPrestacao?.Codigo == localidadeOrigem?.Codigo && obj.LocalidadeTerminoPrestacao?.Codigo == localidadeDestino?.Codigo))
                {
                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novoLocal = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                    novoLocal.Carga = carga;
                    novoLocal.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                    novoLocal.LocalidadeInicioPrestacao = localidadeOrigem;
                    novoLocal.LocalidadeTerminoPrestacao = localidadeDestino;

                    repCargaLocaisPrestacao.Inserir(novoLocal);
                    listaCargaLocaisPrestacao.Add(novoLocal);
                }
            }
        }

        public void ValidarLocaisPrestacaoUtilizandoNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, ref List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> listaCargaLocaisPrestacao, bool cargaDeRetorno = false)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);

            if (cargaPedido.PedidoPallet)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisNotasSaidas = pedidoXMLNotasFiscais.Where(obj => obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisNotasEntrada = pedidoXMLNotasFiscais.Where(obj => obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlSaidas in pedidoXMLNotasFiscaisNotasSaidas)
            {
                Dominio.Entidades.Localidade localidadeOrigem;
                if (cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null)
                    localidadeOrigem = cargaPedido.Pedido.EnderecoOrigem.Localidade;
                else
                {
                    if (cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor &&
                        cargaPedido.TipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor &&
                        !cargaPedido.Pedido.PedidoTransbordo)
                        localidadeOrigem = cargaDeRetorno ? xmlSaidas.XMLNotaFiscal.Destinatario.Localidade : xmlSaidas.XMLNotaFiscal.Emitente.Localidade;
                    else
                        localidadeOrigem = cargaPedido.Origem;
                }

                Dominio.Entidades.Localidade localidadeDestino;
                if (cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null)
                    localidadeDestino = cargaPedido.Pedido.EnderecoDestino.Localidade;
                else
                    localidadeDestino = cargaDeRetorno ? xmlSaidas.XMLNotaFiscal.Emitente.Localidade : xmlSaidas.XMLNotaFiscal.Destinatario.Localidade;

                if (!listaCargaLocaisPrestacao.Any(obj => obj.LocalidadeInicioPrestacao?.Codigo == localidadeOrigem?.Codigo && obj.LocalidadeTerminoPrestacao?.Codigo == localidadeDestino?.Codigo))
                {
                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novoLocal = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                    novoLocal.Carga = carga;
                    novoLocal.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                    novoLocal.LocalidadeInicioPrestacao = localidadeOrigem;
                    novoLocal.LocalidadeTerminoPrestacao = localidadeDestino;
                    repCargaLocaisPrestacao.Inserir(novoLocal);
                    listaCargaLocaisPrestacao.Add(novoLocal);
                }
            }
        }
        private void PreecherLocaisDePrestacaoPorDocumentoFiscal(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> listaCargaLocaisPrestacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoExistente = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacaoExiste in cargaLocaisPrestacaoExistente)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> passagens = repCargaLocaisPrestacaoPassagens.BuscarPorLocalPrestacao(cargaLocalPrestacaoExiste.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens passagem in passagens)
                    repCargaLocaisPrestacaoPassagens.Deletar(passagem);

                Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacaoManter = (from obj in listaCargaLocaisPrestacao where obj.LocalidadeInicioPrestacao.Codigo == cargaLocalPrestacaoExiste.LocalidadeInicioPrestacao.Codigo && obj.LocalidadeTerminoPrestacao.Codigo == cargaLocalPrestacaoExiste.LocalidadeTerminoPrestacao.Codigo select obj).FirstOrDefault();
                if (cargaLocalPrestacaoManter == null)
                    repCargaLocaisPrestacao.Deletar(cargaLocalPrestacaoExiste);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao novaLocalPrestacao in listaCargaLocaisPrestacao)
            {

                Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaExistente = (from obj in cargaLocaisPrestacaoExistente where obj.LocalidadeInicioPrestacao.Codigo == novaLocalPrestacao.LocalidadeInicioPrestacao.Codigo && obj.LocalidadeTerminoPrestacao.Codigo == novaLocalPrestacao.LocalidadeTerminoPrestacao.Codigo select obj).FirstOrDefault();
                if (cargaExistente == null)
                {
                    repCargaLocaisPrestacao.Inserir(novaLocalPrestacao);
                }

            }

        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> RetornaPassagensProntasParaMDFe(Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, Dominio.Entidades.Localidade fronteira, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens)
        {
            if (estadoInicioPrestacao.Sigla == "EX")
            {
                if (fronteira != null && fronteira.Estado.Sigla != estadoTerminoPrestacao.Sigla)
                {
                    passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Posicao = 0, Sigla = fronteira.Estado.Sigla });
                    passagens = passagens.OrderBy(obj => obj.Posicao).ToList();
                }
            }

            if (estadoTerminoPrestacao.Sigla == "EX" && (fronteira?.Estado?.Sigla != "EX"))
            {
                if (fronteira != null && fronteira.Estado.Sigla != estadoInicioPrestacao.Sigla)
                {
                    int posicao = 1;

                    if (passagens.Count > 0)
                        posicao = passagens.Last().Posicao + 1;

                    passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Posicao = posicao, Sigla = fronteira.Estado.Sigla });
                }
            }

            return passagens;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens> RetornarPassagensParaLocaisPercursoEntreOsEstados(List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoEstado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens> listaPassagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacaoEstado in cargaLocaisPrestacaoEstado)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPassagens = repCargaLocaisPrestacaoPassagens.BuscarPorLocalPrestacao(cargaLocalPrestacaoEstado.Codigo);
                if (listaPassagens.Count > 0)
                {
                    bool encontrouMesmoPercurso = false;
                    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens mdfePassagem in listaPassagens)
                    {
                        var percursoMesmo = true;
                        if (cargaLocaisPassagens.Count != mdfePassagem.Passagem.Count)
                            percursoMesmo = false;
                        else
                        {
                            for (int i = 0; i < cargaLocaisPassagens.Count; i++)
                            {
                                if (mdfePassagem.Passagem[i].Sigla != cargaLocaisPassagens[i].EstadoDePassagem.Sigla)
                                {
                                    percursoMesmo = false;
                                    break;
                                }
                            }
                        }

                        if (cargaLocaisPassagens.Count == 0 && listaPassagens.Any(lp => lp.Passagem.Any(passagem => passagem.Sigla == cargaLocalPrestacaoEstado.LocalidadeInicioPrestacao.Estado.Sigla)))
                            percursoMesmo = true;

                        if (percursoMesmo)
                        {
                            encontrouMesmoPercurso = true;
                            break;
                        }
                    }

                    if (!encontrouMesmoPercurso)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens mdfePassagem = new Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens();
                        mdfePassagem.Passagem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
                        mdfePassagem.Origem = cargaLocalPrestacaoEstado.LocalidadeInicioPrestacao;
                        mdfePassagem.Destino = cargaLocalPrestacaoEstado.LocalidadeTerminoPrestacao;
                        mdfePassagem.CargaLocaisPrestacao = cargaLocalPrestacaoEstado;
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPassagem in cargaLocaisPassagens)
                            mdfePassagem.Passagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = cargaLocalPassagem.EstadoDePassagem.Sigla, Posicao = cargaLocalPassagem.Posicao });

                        listaPassagens.Add(mdfePassagem);
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens mdfePassagem = new Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens();
                    mdfePassagem.Passagem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
                    mdfePassagem.CargaLocaisPrestacao = cargaLocalPrestacaoEstado;
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPassagem in cargaLocaisPassagens)
                        mdfePassagem.Passagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = cargaLocalPassagem.EstadoDePassagem.Sigla, Posicao = cargaLocalPassagem.Posicao });
                    listaPassagens.Add(mdfePassagem);
                }
            }

            listaPassagens.Reverse();
            return listaPassagens;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao CriarLocalPrestacao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao, Repositorio.UnitOfWork unitOfWork, out bool criouNovo)
        {
            criouNovo = false;
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

            Dominio.Entidades.Localidade inicioPrestacao = serRota.BuscarLocalidadeOrigem(cargaPedido);
            Dominio.Entidades.Localidade terminoPrestacao = serRota.BuscarLocalidadeDestino(cargaPedido);

            if (terminoPrestacao != null && inicioPrestacao != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = (from obj in cargaLocaisPrestacao where obj.LocalidadeInicioPrestacao.Codigo == inicioPrestacao.Codigo && obj.LocalidadeTerminoPrestacao.Codigo == terminoPrestacao.Codigo select obj).FirstOrDefault(); //repCargaLocaisPrestacao.BuscarPorOrigemDestinoECarga(cargaPedido.Carga.Codigo, inicioPrestacao.Codigo, terminoPrestacao.Codigo);
                if (cargaLocalPrestacao == null)
                {
                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                    cargaLocalPrestacao = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao();
                    cargaLocalPrestacao.Carga = cargaPedido.Carga;
                    cargaLocalPrestacao.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(cargaPedido.Carga)?.Fronteira.Localidade; ;
                    cargaLocalPrestacao.LocalidadeInicioPrestacao = inicioPrestacao;
                    cargaLocalPrestacao.LocalidadeTerminoPrestacao = terminoPrestacao;
                    repCargaLocaisPrestacao.Inserir(cargaLocalPrestacao);
                    cargaLocaisPrestacao.Add(cargaLocalPrestacao);
                    criouNovo = true;
                }
                return cargaLocalPrestacao;
            }
            else
            {
                return null;
            }

        }

        public bool RemoverLocaisPrestacaoNaoExistemMais(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> locaisAtualizados, Repositorio.UnitOfWork unitOfWork)
        {
            bool removeu = false;
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagem = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisExistentes = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.BuscarPorCarga(carga.Codigo);


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocaisExistente in cargaLocaisExistentes)
            {
                if (!locaisAtualizados.Contains(cargaLocaisExistente))
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPrestacaoPassagens = repCargaLocaisPrestacaoPassagem.BuscarPorLocalPrestacao(cargaLocaisExistente.Codigo);

                    // Na unilever, existe um cenário de cancelar apenas os documentos da carga e retorná-la para etapa 2.
                    // Por vezes, ainda permenece algum mdfe com locais prestação, e ao passar nesse método, o sistema falece por conta do vínculo... Por isso precisa excluí-los também.
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfesDeletar = cargaMDFes.Where(mdfe => (mdfe.CargaLocaisPrestacao?.Codigo ?? 0) == cargaLocaisExistente.Codigo).ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe mdfe in mdfesDeletar)
                        repCargaMDFe.Deletar(mdfe);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem in cargaLocaisPrestacaoPassagens)
                        repCargaLocaisPrestacaoPassagem.Deletar(cargaLocalPrestacaoPassagem);

                    repCargaLocaisPrestacao.Deletar(cargaLocaisExistente);


                    removeu = true;
                }
            }
            return removeu;

        }

        public void LimparPassagensLocaisPrestacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao in cargaLocaisPrestacao)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPrestacaoPassagens = repCargaLocaisPrestacaoPassagens.BuscarPorLocalPrestacao(cargaLocalPrestacao.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem in cargaLocaisPrestacaoPassagens)
                    repCargaLocaisPrestacaoPassagens.Deletar(cargaLocalPrestacaoPassagem);

            }

        }

        public bool ValidarPassagensMDFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Logistica.MapRequestApi(unitOfWork);
            Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);

            var todosPercursoPreechido = true; //Todo: a regra que garante que o percurso informado é valido, atualmente está apenas no javascript, futuramente adicionar no server Side.
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao in cargaLocaisPrestacao)
            {
                if (repCargaLocaisPrestacaoPassagens.ContarPorLocalPrestacao(cargaLocalPrestacao.Codigo) == 0)
                {
                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

                    Dominio.Entidades.Localidade localidadeInicioMDFe = buscarLocalidadeMDFe(cargaLocalPrestacao.LocalidadeInicioPrestacao, cargaLocalPrestacao.LocalidadeFronteira);
                    var fronteiraCarga = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira;
                    Dominio.Entidades.Localidade localidadeFimMDFe = buscarLocalidadeMDFe(cargaLocalPrestacao.LocalidadeTerminoPrestacao, cargaLocalPrestacao.LocalidadeFronteira, fronteiraCarga);

                    if (localidadeInicioMDFe == null || localidadeFimMDFe == null)
                    {
                        todosPercursoPreechido = false;
                        break;
                    }

                    if (localidadeInicioMDFe.Estado.Sigla != localidadeFimMDFe.Estado.Sigla)
                    {
                        List<string> divisas = serMapRequestAPI.BuscarDivisas(localidadeInicioMDFe.Estado.Sigla);
                        if (!divisas.Contains(localidadeFimMDFe.Estado.Sigla))
                        {
                            todosPercursoPreechido = false;
                            break;
                        }
                    }
                }
            }


            if (!todosPercursoPreechido)
            {
                bool percursoPadraoValido = true;
                List<Dominio.Entidades.Localidade> rotaLocalidades = new List<Dominio.Entidades.Localidade>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursos = repCargaPercurso.ConsultarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in cargaPercursos)
                    rotaLocalidades.Add(cargaPercurso.Origem);

                if (cargaPercursos.Count > 0)
                    rotaLocalidades.Add(cargaPercursos[cargaPercursos.Count - 1].Destino);

                var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
                var fronteiraPrincipal = serCargaFronteira.ObterFronteiraPrincipal(carga);
                var localidadeFronteira = fronteiraPrincipal?.Fronteira.Localidade;
                List<Dominio.Entidades.Localidade> localidadesMDFe = BuscarRotasParaMDFe(rotaLocalidades, localidadeFronteira);
                List<string> ufsDestinos = (from obj in cargaLocaisPrestacao select obj.LocalidadeTerminoPrestacao.Estado.Sigla).Distinct().ToList();

                Dominio.Entidades.PercursoEstado percursoEstado = repPercursoEstado.BuscarPorOrigemEDestino(carga.Empresa != null ? carga.Empresa.Codigo : 0, localidadesMDFe.First().Estado.Sigla, ufsDestinos);

                if (percursoEstado == null)
                    percursoEstado = repPercursoEstado.BuscarPorOrigemEDestino(carga.Empresa != null ? carga.Empresa.EmpresaPai.Codigo : 0, localidadesMDFe.First().Estado.Sigla, ufsDestinos);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao in cargaLocaisPrestacao)
                {
                    if (percursoEstado != null)
                    {
                        percursoPadraoValido = adicionarPercursoPadrao(cargaLocalPrestacao, percursoEstado, fronteiraPrincipal?.Fronteira, unitOfWork);
                        if (!percursoPadraoValido)
                            break;
                    }
                    else
                    {
                        Dominio.Entidades.Localidade localidadeInicioMDFe = buscarLocalidadeMDFe(cargaLocalPrestacao.LocalidadeInicioPrestacao, cargaLocalPrestacao.LocalidadeFronteira);
                        Dominio.Entidades.Localidade localidadeFimMDFe = buscarLocalidadeMDFe(cargaLocalPrestacao.LocalidadeTerminoPrestacao, cargaLocalPrestacao.LocalidadeFronteira, fronteiraPrincipal?.Fronteira);
                        if (localidadeInicioMDFe != null && localidadeFimMDFe != null)
                        {
                            if (localidadeInicioMDFe.Estado.Sigla != localidadeFimMDFe.Estado.Sigla)
                            {
                                List<string> divisas = serMapRequestAPI.BuscarDivisas(localidadeInicioMDFe.Estado.Sigla);
                                if (!divisas.Contains(localidadeFimMDFe.Estado.Sigla))
                                {
                                    percursoPadraoValido = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                return percursoPadraoValido;
            }
            else
            {
                return true;
            }
        }

        private bool adicionarPercursoPadrao(Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao, Dominio.Entidades.PercursoEstado percursoEstado, Dominio.Entidades.Cliente fronteiraCarga, Repositorio.UnitOfWork unitOfWork)
        {
            bool valido = true;

            Dominio.Entidades.Localidade localidadeInicioMDFe = buscarLocalidadeMDFe(cargaLocalPrestacao.LocalidadeInicioPrestacao, cargaLocalPrestacao.LocalidadeFronteira);
            Dominio.Entidades.Localidade localidadeFimMDFe = buscarLocalidadeMDFe(cargaLocalPrestacao.LocalidadeTerminoPrestacao, cargaLocalPrestacao.LocalidadeFronteira, fronteiraCarga);

            Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
            if (percursoEstado != null)
            {
                List<Dominio.Entidades.PassagemPercursoEstado> passagensPercursoEstado = repPassagemPercursoEstado.Buscar(percursoEstado.Codigo);
                foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagensPercursoEstado)
                    passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = passagem.EstadoDePassagem.Sigla, Posicao = passagem.Ordem });
            }
            passagens = passagens.OrderBy(obj => obj.Posicao).ToList();


            if (percursoEstado.EstadoOrigem.Sigla == localidadeInicioMDFe.Estado.Sigla && percursoEstado.EstadoDestino.Sigla == localidadeFimMDFe.Estado.Sigla)
                adicionarCargaLocaisPrestacaoPassagens(cargaLocalPrestacao, passagens, unitOfWork);
            else
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> tempPassagem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
                if (percursoEstado.EstadoOrigem.Sigla == localidadeInicioMDFe.Estado.Sigla)
                {
                    bool encontrouDestino = false;
                    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passagem in passagens)
                    {
                        if (passagem.Sigla == localidadeFimMDFe.Estado.Sigla || (localidadeFimMDFe.Estado.Sigla == localidadeInicioMDFe.Estado.Sigla))
                        {
                            encontrouDestino = true;
                            break;
                        }
                        else
                            tempPassagem.Add(passagem);
                    }
                    if (encontrouDestino)
                        adicionarCargaLocaisPrestacaoPassagens(cargaLocalPrestacao, tempPassagem, unitOfWork);
                    else
                        valido = false;
                }
                else
                {
                    if (percursoEstado.EstadoDestino.Sigla == localidadeFimMDFe.Estado.Sigla)
                    {
                        bool encontrouOrigem = false;
                        foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passagem in passagens)
                        {
                            if (encontrouOrigem)
                                tempPassagem.Add(passagem);
                            if (passagem.Sigla == localidadeInicioMDFe.Estado.Sigla)
                                encontrouOrigem = true;
                        }
                        if (encontrouOrigem)
                            adicionarCargaLocaisPrestacaoPassagens(cargaLocalPrestacao, tempPassagem, unitOfWork);
                        else
                            valido = false;
                    }
                    else
                    {
                        bool encontrouOrigem = false;
                        bool encontrouDestino = false;
                        foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passagem in passagens)
                        {
                            if (passagem.Sigla == localidadeFimMDFe.Estado.Sigla)
                            {
                                encontrouDestino = true;
                                break;
                            }

                            if (encontrouOrigem)
                            {
                                tempPassagem.Add(passagem);
                            }
                            if (passagem.Sigla == localidadeInicioMDFe.Estado.Sigla)
                                encontrouOrigem = true;
                        }
                        if (encontrouOrigem && encontrouDestino)
                            adicionarCargaLocaisPrestacaoPassagens(cargaLocalPrestacao, tempPassagem, unitOfWork);
                        else
                            valido = false;
                    }
                }

            }
            return valido;
        }

        private void adicionarCargaLocaisPrestacaoPassagens(Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);
            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passagem in passagens)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocaisPrestacaoPassagens = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens();
                cargaLocaisPrestacaoPassagens.CargaLocaisPrestacao = cargaLocalPrestacao;
                cargaLocaisPrestacaoPassagens.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = passagem.Sigla };
                cargaLocaisPrestacaoPassagens.Posicao = passagem.Posicao;
                repCargaLocaisPrestacaoPassagens.Inserir(cargaLocaisPrestacaoPassagens);
            }
        }

        public List<Dominio.Entidades.Localidade> BuscarRotasParaMDFe(List<Dominio.Entidades.Localidade> localidades, Dominio.Entidades.Localidade fronteira)
        {
            List<Dominio.Entidades.Localidade> localidadesMDFe = new List<Dominio.Entidades.Localidade>();
            for (int i = 0; i < localidades.Count; i++)
            {
                Dominio.Entidades.Localidade localidadeMDFe = localidades[i];
                if (localidades[i].CodigoIBGE == 9999999 || localidades[i].Estado?.Sigla == "EX")
                    localidadeMDFe = fronteira != null ? fronteira : localidadeMDFe;

                if (localidadesMDFe.Count > 0)
                {
                    if (localidades[i].Codigo != localidades[i - 1].Codigo)
                    {
                        localidadesMDFe.Add(localidadeMDFe);
                    }
                }
                else
                    localidadesMDFe.Add(localidadeMDFe);
            }

            return localidadesMDFe;
        }

        private Dominio.Entidades.Localidade buscarLocalidadeMDFe(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Localidade fronteira, Dominio.Entidades.Cliente fronteiraCarga = null)
        {
            if (localidade.CodigoIBGE != 9999999 && localidade.Estado?.Sigla != "EX")
                return localidade;
            else
                return fronteira != null ? fronteira : fronteiraCarga?.Localidade;
        }

        public bool AdicaoPercursoCargaSemPassagem(Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao, Dominio.Entidades.PercursoEstado percursoEstado, Dominio.Entidades.Cliente fronteiraCarga, Repositorio.UnitOfWork unitOfWork)
        {
            return adicionarPercursoPadrao(cargaLocalPrestacao, percursoEstado, fronteiraCarga, unitOfWork);
        }

    }
}
