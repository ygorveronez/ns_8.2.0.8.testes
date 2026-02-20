using Dominio.Interfaces.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class Rota : ServicoBase
    {        
        public Rota(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga CriarRotaPorLocaisDePrestacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            DeletarPercursoDestinosCarga(carga, unitOfWork);

            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacoes = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new CargaLocaisPrestacao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota> DynRotas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota>();
            List<Dominio.Entidades.Embarcador.Logistica.Rota> rotasNaoEncontradas = new List<Dominio.Entidades.Embarcador.Logistica.Rota>();

            List<Dominio.Entidades.Localidade> localidadesInicioPrestacao = (from obj in cargaLocaisPrestacoes select obj.LocalidadeInicioPrestacao).Distinct().ToList();
            List<Dominio.Entidades.Localidade> localidadesFimPrestacao = (from obj in cargaLocaisPrestacoes select obj.LocalidadeTerminoPrestacao).Distinct().ToList();

            Dominio.Entidades.Localidade ultimaOrigem = null;
            if (localidadesInicioPrestacao.Count > 1)
            {
                //tem q ser ordenado por ordemColeta (Romanovski, Rodrigo Junho de 2021) 
                Dominio.Entidades.Localidade origemPedidoInicioCarga = (from obj in cargaPedidos orderby obj.OrdemColeta select obj.Origem).FirstOrDefault();
                if (origemPedidoInicioCarga == null)//todo: rever, pois quando não souber qual é a origem sempre pegar a do primeiro pedido não sei se é a melhor opção.
                    origemPedidoInicioCarga = cargaPedidos.FirstOrDefault().Origem;

                foreach (Dominio.Entidades.Localidade inicioPrestacao in localidadesInicioPrestacao)
                {
                    if (inicioPrestacao.Codigo != origemPedidoInicioCarga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Rota rotaOrigem = AdicionarRotas(origemPedidoInicioCarga, inicioPrestacao, null, ref rotasNaoEncontradas, unitOfWork);
                        if (rotaOrigem != null)
                            addDynRota(DynRotas, rotaOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.carregamento);
                    }
                }
                DynRotas = DynRotas.OrderBy(p => p.DistanciaKM).ToList();
                ultimaOrigem = (from obj in DynRotas select obj.Destino).LastOrDefault();
            }
            else if (localidadesInicioPrestacao.Count == 1)
                ultimaOrigem = localidadesInicioPrestacao[0];

            foreach (Dominio.Entidades.Localidade localidadeFimPrestacao in localidadesFimPrestacao)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (from obj in cargaPedidos where obj.Destino != null && obj.Destino.Codigo == localidadeFimPrestacao.Codigo select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Logistica.Rota rota = AdicionarRotas(ultimaOrigem, localidadeFimPrestacao, cargaPedido, ref rotasNaoEncontradas, unitOfWork);
                if (rota != null)
                    addDynRota(DynRotas, rota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento);
            }

            return AdicionarRotas(carga, DynRotas, rotasNaoEncontradas, unitOfWork);
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga CriarRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {

            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new CargaLocaisPrestacao(unitOfWork);


            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota> DynRotas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota>();
            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = new Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga();
            List<Dominio.Entidades.Embarcador.Logistica.Rota> rotasNaoEncontradas = new List<Dominio.Entidades.Embarcador.Logistica.Rota>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosbase = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaPedidosbase.Where(obj => !obj.PedidoPallet).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisAtual = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return dynRota;

            if (cargaPedidos.Count <= 0)
                return dynRota;

            //todo: mesmo que a emissão seja por nota, é necessário verificar se as notas foram enviadas senão criar o primeiro percurso com base no pedido.
            bool percursoPorNota = serCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedidos.FirstOrDefault().TipoRateio, cargaPedidos.FirstOrDefault()?.TipoEmissaoCTeParticipantes ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal, tipoServicoMultisoftware);
            if (percursoPorNota)
            {
                bool enviouTodas = true;
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cp in cargaPedidos)
                {
                    if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cp.Codigo) <= 0)
                    {
                        enviouTodas = false;
                        break;
                    }
                }
                if (enviouTodas)
                    percursoPorNota = true;
                else
                    percursoPorNota = false;
            }

            if (!percursoPorNota)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoCarga = repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Localidade origemPedido = BuscarLocalidadeOrigem(cargaPedido);
                    if (!origens.Contains(origemPedido))
                        origens.Add(origemPedido);

                    bool criouNovo = false;
                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = serCargaLocaisPrestacao.CriarLocalPrestacao(cargaPedido, cargaLocaisPrestacaoCarga, unitOfWork, out criouNovo);
                    if (cargaLocalPrestacao != null)
                    {
                        if (!cargaLocaisAtual.Contains(cargaLocalPrestacao))
                            cargaLocaisAtual.Add(cargaLocalPrestacao);
                    }
                }
                serCargaLocaisPrestacao.RemoverLocaisPrestacaoNaoExistemMais(carga, cargaLocaisAtual, unitOfWork);


                Dominio.Entidades.Localidade ultimaOrigem;
                if (origens.Count > 1)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoInicio = (from obj in cargaPedidos where obj.InicioDaCarga select obj).FirstOrDefault();

                    if (cargaPedidoInicio == null)
                        cargaPedidoInicio = cargaPedidos.FirstOrDefault();

                    Dominio.Entidades.Localidade origemPedidoInicioCarga = BuscarLocalidadeOrigem(cargaPedidoInicio);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        Dominio.Entidades.Localidade origemPedido = BuscarLocalidadeOrigem(cargaPedido);
                        if (origemPedido.Codigo != origemPedidoInicioCarga.Codigo)
                        {
                            Dominio.Entidades.Embarcador.Logistica.Rota rotaOrigem = AdicionarRotas(origemPedidoInicioCarga, origemPedido, cargaPedido, ref rotasNaoEncontradas, unitOfWork);
                            if (rotaOrigem != null)
                                addDynRota(DynRotas, rotaOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.carregamento);
                        }
                    }
                    DynRotas = DynRotas.OrderBy(p => p.DistanciaKM).ToList();
                    ultimaOrigem = (from obj in DynRotas select obj.Destino).LastOrDefault();
                }
                else
                {
                    ultimaOrigem = origens[0];
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Localidade destinoPedido = BuscarLocalidadeDestino(cargaPedido);

                    if (destinoPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Rota rota = AdicionarRotas(ultimaOrigem, destinoPedido, cargaPedido, ref rotasNaoEncontradas, unitOfWork);

                        if (rota != null)
                        {
                            addDynRota(DynRotas, rota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento);
                        }
                    }
                }
                return AdicionarRotas(carga, DynRotas, rotasNaoEncontradas, unitOfWork);
            }
            else
            {
                serCargaLocaisPrestacao.RecriarLocaisPrestacaoPorNotasFiscais(carga, cargaPedidos, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga retornoRotas = CriarRotaPorLocaisDePrestacao(carga, cargaPedidos, unitOfWork);
                if (retornoRotas.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida)
                    serCargaPedido.SetarDestinatarioFinalCarga(carga, cargaPedidos, unitOfWork, configuracaoPedido);

                return retornoRotas;
            }
        }


        private Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga AdicionarRotas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota> DynRotas, List<Dominio.Entidades.Embarcador.Logistica.Rota> rotasNaoEncontradas, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = new Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga();

            if (rotasNaoEncontradas.Count <= 0)
            {
                //todo: a rota padrão é sugerida com base em um calculo manual entre entre as distancias, iniciando sempre pelos locais de carregamento
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota> DynRotasOrdenada = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota>();
                DynRotasOrdenada.AddRange((from obj in DynRotas where obj.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.carregamento select obj).OrderBy(obj => obj.DistanciaKM).ToList());
                DynRotasOrdenada.AddRange((from obj in DynRotas where obj.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento select obj).OrderBy(obj => obj.DistanciaKM).ToList());
                CriarPercursoDestinosCarga(carga, DynRotasOrdenada, unitOfWork);
                dynRota.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida;
            }
            else
            {
                dynRota.rotasNaoEncontradas = rotasNaoEncontradas;
                dynRota.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.NaoEncontrada;
            }
            return dynRota;
        }



        private void addDynRota(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota> dynRotas, Dominio.Entidades.Embarcador.Logistica.Rota rota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota tipoRota)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.Rota dynRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.Rota();
            dynRota.Codigo = rota.Codigo;
            dynRota.Origem = rota.Origem;
            dynRota.Destino = rota.Destino;
            dynRota.Remetente = rota.Remetente;
            dynRota.Destinatario = rota.Destinatario;
            dynRota.DistanciaKM = rota.DistanciaKM;
            dynRota.TipoRota = tipoRota;

            if (!dynRotas.Contains(dynRota))
                dynRotas.Add(dynRota);

        }


        public void DeletarPercursoDestinosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            serCargaLocaisPrestacao.LimparPassagensLocaisPrestacao(carga, unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursosAtual = repCargaPercurso.ConsultarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in cargaPercursosAtual)
            {
                repCargaPercurso.Deletar(cargaPercurso);
            }
        }

        public async Task DeletarPercursoDestinosCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);

            serCargaLocaisPrestacao.LimparPassagensLocaisPrestacao(carga, unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursosAtual = repCargaPercurso.ConsultarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in cargaPercursosAtual)
            {
                await repCargaPercurso.DeletarAsync(cargaPercurso);
            }
        }

        public void CriarPercursoDestinosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota> rotas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Logistica.MapRequestApi(unitOfWork);

            int posicao = 1;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Rota rotaOrigemPercurso = null;
            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Rota rota in rotas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso = new Dominio.Entidades.Embarcador.Cargas.CargaPercurso();
                cargaPercurso.Posicao = posicao;
                cargaPercurso.Carga = carga;
                cargaPercurso.Destino = rota.Destino;
                cargaPercurso.TipoRota = rota.TipoRota;

                if (rotaOrigemPercurso == null)
                {
                    cargaPercurso.Origem = rota.Origem;
                    cargaPercurso.DistanciaKM = rota.DistanciaKM;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Logistica.Rota rotaDestino = null;
                    if (rota.Remetente != null && rota.Destinatario != null)
                        rotaDestino = repRota.BuscarRotaPorRemetenteDestino(rota.Remetente.CPF_CNPJ, rota.Destinatario.CPF_CNPJ);

                    if (rotaDestino == null)
                        rotaDestino = repRota.BuscarRotaPorOrigemDestino(rotaOrigemPercurso.Destino.Codigo, rota.Destino.Codigo);

                    if (rotaDestino == null)
                    {
                        rotaDestino = serMapRequestAPI.CriarRota(rotaOrigemPercurso.Destino, rota.Destino, unitOfWork);
                        if (rotaDestino == null)
                        {
                            //Aqui não foi possivel verificar a rota na API da Open Map então é feita um calculo manual para chegar a distancia aproximada.
                            rotaDestino = new Dominio.Entidades.Embarcador.Logistica.Rota();
                            rotaDestino.DistanciaKM = rota.DistanciaKM - rotaOrigemPercurso.DistanciaKM;
                        }
                    }
                    else
                    {
                        //if (rotaDestino.PossuiPedagio)
                        //{
                        //    Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                        //    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);
                        //    if (integracao != null && !string.IsNullOrWhiteSpace(rotaDestino.DescricaoRotaSemParar))
                        //    {
                        //        Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                        //        Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(unitOfWork);
                        //        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorTipoIntegracao(carga.Codigo, integracao.Codigo);
                        //        if (cargaValePedagio == null)
                        //        {
                        //            cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                        //            cargaValePedagio.Carga = carga;
                        //            cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Pendete;
                        //            cargaValePedagio.TipoIntegracao = integracao;
                        //            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        //            repCargaValePedagio.Inserir(cargaValePedagio);
                        //        }
                        //        List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotasValePedagio = repCargaValePedagioRota.BuscarPorCargaValePedagio(cargaValePedagio.Codigo);
                        //        if (!rotasValePedagio.Any(obj => obj.DescricaoRota == rotaDestino.DescricaoRotaSemParar))
                        //        {
                        //            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota cargaValePedagioRota = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota();
                        //            cargaValePedagioRota.CargaValePedagio = cargaValePedagio;
                        //            cargaValePedagioRota.DescricaoRota = rotaDestino.DescricaoRotaSemParar;
                        //            repCargaValePedagioRota.Inserir(cargaValePedagioRota);
                        //        }
                        //    }
                        //}
                    }

                    cargaPercurso.Origem = rotaOrigemPercurso.Destino;
                    cargaPercurso.DistanciaKM = rotaDestino.DistanciaKM;
                }
                repCargaPercurso.Inserir(cargaPercurso);
                rotaOrigemPercurso = rota;
                posicao++;
            }
        }


        public Dominio.Entidades.Embarcador.Logistica.Rota AdicionarRotas(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargapedido, ref List<Dominio.Entidades.Embarcador.Logistica.Rota> rotasNaoEncontradas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequestApi = new Logistica.MapRequestApi(unitOfWork);
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.Rota rota = null;
            if (cargapedido != null && (cargapedido.Expedidor != null || cargapedido.Pedido.Remetente != null) && (cargapedido.Recebedor != null || cargapedido.Pedido.Destinatario != null))
            {
                rota = repRota.BuscarRotaPorRemetenteDestino(cargapedido.Expedidor != null ? cargapedido.Expedidor.CPF_CNPJ : cargapedido.Pedido.Remetente.CPF_CNPJ, cargapedido.Recebedor != null ? cargapedido.Recebedor.CPF_CNPJ : cargapedido.Pedido.Destinatario.CPF_CNPJ);
            }

            if (rota == null)
                rota = repRota.BuscarRotaPorOrigemDestino(origem.Codigo, destino.Codigo);

            if (rota == null)
            {
                if (origem.Codigo == destino.Codigo)
                {   //caso a cidade de origem e destino forem iguais é incluida uma rota de com 1 km, futuro pode-se alterar caso necessário informações de distancias dentro da mesma cidade.
                    rota = new Dominio.Entidades.Embarcador.Logistica.Rota();
                    rota.Origem = origem;
                    rota.Destino = destino;
                    rota.DistanciaKM = 1;
                    rota.Ativo = true;
                    rota.PossuiPedagio = false;
                    repRota.Inserir(rota);
                }
                else
                {
                    rota = serMapRequestApi.CriarRota(origem, destino, unitOfWork);
                    if (rota == null)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Rota rotaNaoEncontrada = new Dominio.Entidades.Embarcador.Logistica.Rota();
                        rotaNaoEncontrada.Origem = repLocalidade.BuscarPorCodigo(origem.Codigo);
                        rotaNaoEncontrada.Destino = repLocalidade.BuscarPorCodigo(destino.Codigo);
                        if (!rotasNaoEncontradas.Contains(rotaNaoEncontrada))
                        {
                            rotasNaoEncontradas.Add(rotaNaoEncontrada);
                        }
                    }
                }
            }

            if (origem.Codigo != destino.Codigo && rota?.DistanciaKM == 1)
            {
                Servicos.Embarcador.Carga.CargaRotaFrete servicoCargaRotaFrete = new Servicos.Embarcador.Carga.CargaRotaFrete(unitOfWork);
                int km = servicoCargaRotaFrete.CalcularDistanciaPorOrigemEDestino(origem, destino);

                if (km > 1)
                {
                    rota.DistanciaKM = km;
                    repRota.Atualizar(rota);
                }
            }

            return rota;
        }


        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornarDadosRotaNaoEncontrada(Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
            retorno.valorFrete = 0;
            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.RotaNaoEncontrada;
            retorno.rotasNaoEncontradas = from p in dynRota.rotasNaoEncontradas
                                          select new
                                          {
                                              p.Codigo,
                                              Destino = new { p.Destino.Codigo, Descricao = p.Destino.DescricaoCidadeEstado },
                                              Origem = new { p.Origem.Codigo, Descricao = p.Origem.DescricaoCidadeEstado }
                                          };
            return retorno;
        }

        public Dominio.Entidades.Localidade BuscarLocalidadeDestino(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Dominio.Entidades.Localidade destino = cargaPedido.Destino;
            return destino;
        }

        public Dominio.Entidades.Localidade BuscarLocalidadeOrigem(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Dominio.Entidades.Localidade origem = cargaPedido.Origem;
            return origem;
        }
    }
}
