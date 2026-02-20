using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class GTA
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.EDI.GTA.SUINO ConverterCargaEDIIntegracaoParaSUINO(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.GTA.SUINO dadosSuino = new Dominio.ObjetosDeValor.EDI.GTA.SUINO();
            dadosSuino.Paradas = new List<Dominio.ObjetosDeValor.EDI.GTA.Parada>();
            dadosSuino.HorarioInicioViagem = carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm:ss");
            dadosSuino.HorarioTerminoViagem = carga.DataFimViagem?.ToString("dd/MM/yyyy HH:mm:ss");
            dadosSuino.Numero = carga.CodigoCargaEmbarcador;
            dadosSuino.Protocolo = carga.Codigo;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);

            // Cada CargaEntrega é uma parada
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
            {
                Dominio.ObjetosDeValor.EDI.GTA.Parada parada = new Dominio.ObjetosDeValor.EDI.GTA.Parada();
                parada.Numero = cargaEntrega.Codigo.ToString();
                parada.SequenciaPlanejada = cargaEntrega.Ordem;
                parada.SequenciaRealizada = cargaEntrega.OrdemRealizada;
                parada.Situacao = cargaEntrega.DescricaoSituacao;
                parada.Observacao = cargaEntrega.Observacao;

                if (cargaEntrega.Coleta)
                {
                    parada.Tipo = "Coleta";
                    parada.HorarioChegadaProdutor = cargaEntrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm:ss");
                    parada.HorarioSaidaProdutor = cargaEntrega.DataConfirmacao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    parada.HorarioInicioColeta = cargaEntrega.DataInicio?.ToString("dd/MM/yyyy HH:mm:ss");
                    parada.HorarioTerminoColeta = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm:ss");
                    parada.GTA = ObterDadosGTA(carga, cargaEntrega, unidadeTrabalho);
                    parada.Pesquisa = ObterPesquisa(carga, cargaEntrega, unidadeTrabalho);
                    parada.PesquisaDesembarque = ObterPesquisaDesembarque(carga, cargaEntrega, unidadeTrabalho);
                    parada.Pedidos = ObterPedidos(cargaEntrega, cargaEntregaPedidos, cargaPedidoProdutos, unidadeTrabalho);
                }
                else
                {
                    parada.Tipo = "Entrega";
                    parada.HorarioChegadaUnidade = cargaEntrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm:ss");
                }

                dadosSuino.Paradas.Add(parada);
            }

            return dadosSuino;
        }

        public Dominio.ObjetosDeValor.EDI.GTA.AVES ConverterCargaEDIIntegracaoParaAVES(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            var repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.GTA.AVES dadosSuino = new Dominio.ObjetosDeValor.EDI.GTA.AVES();
            dadosSuino.Paradas = new List<Dominio.ObjetosDeValor.EDI.GTA.ParadaAves>();
            dadosSuino.HorarioInicioViagem = carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm:ss");
            dadosSuino.HorarioTerminoViagem = carga.DataFimViagem?.ToString("dd/MM/yyyy HH:mm:ss");
            dadosSuino.Numero = carga.CodigoCargaEmbarcador;
            dadosSuino.Protocolo = carga.Codigo;

            var listaCargaEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);

            // Cada CargaEntrega é uma parada
            foreach (var cargaEntrega in listaCargaEntrega)
            {
                var parada = new Dominio.ObjetosDeValor.EDI.GTA.ParadaAves();
                parada.Numero = cargaEntrega.Codigo.ToString();
                parada.SequenciaPlanejada = cargaEntrega.Ordem;
                parada.SequenciaRealizada = cargaEntrega.OrdemRealizada;
                parada.Situacao = cargaEntrega.DescricaoSituacao;
                parada.Observacao = cargaEntrega.Observacao;

                if (cargaEntrega.Coleta)
                {
                    parada.Tipo = "Coleta";
                    parada.HorarioChegadaProdutor = cargaEntrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    parada.HorarioSaidaProdutor = cargaEntrega.DataConfirmacao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    parada.HorarioInicioColeta = cargaEntrega.DataInicio?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    parada.HorarioTerminoColeta = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    parada.GTA = ObterDadosGTA(carga, cargaEntrega, unidadeTrabalho);
                    parada.Pesquisa = ObterPesquisa(carga, cargaEntrega, unidadeTrabalho);
                    ObterQuantidadesAves(carga, out int quantidadeAvesPorCaixa, out int quantidadeAvesPorCaixaRealizada, out int quantidadeAvesCarregadas, out int quantidadePlanejada, out int caixasVazias, out int caixasVaziasPlanejada);
                    parada.QuantidadeAvesPorCaixa = quantidadeAvesPorCaixa;
                    parada.QuantidadeAvesPorCaixaRealizada = quantidadeAvesPorCaixaRealizada;
                    parada.QuantidadeAvesCarregadas = quantidadeAvesCarregadas;
                    parada.QuantidadeAvesPlanejada = quantidadePlanejada;
                    parada.QuantidadeCaixasVazias = caixasVazias;
                    parada.QuantidadeCaixasVaziasPlanejada = caixasVaziasPlanejada;

                    parada.ChaveNfProdutor = ""; //TODO: esse valor vai vir do app, então tem que atualizar ele quando a #32820 for feita
                }
                else
                {
                    parada.Tipo = "Entrega";
                    parada.HorarioChegadaUnidade = cargaEntrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm:ss");
                }

                dadosSuino.Paradas.Add(parada);
            }

            return dadosSuino;
        }

        #endregion

        #region Métodos Privados

        private void ObterQuantidadesAves(Dominio.Entidades.Embarcador.Cargas.Carga carga, out int quantidadeAvesPorCaixa, out int quantidadeAvesPorCaixaRealizada, out int quantidadeAvesCarregadas, out int quantidadePlanejada, out int caixasVazias, out int caixasVaziasPlanejada)
        {
            quantidadeAvesPorCaixa = 0;
            quantidadeAvesPorCaixaRealizada = 0;
            quantidadeAvesCarregadas = 0;
            quantidadePlanejada = 0;
            caixasVazias = 0;
            caixasVaziasPlanejada = 0;

            var pedido = carga.Pedidos.FirstOrDefault();

            if (pedido != null)
            {
                quantidadeAvesPorCaixa = pedido.Produtos.FirstOrDefault()?.QuantidadeCaixa ?? 0;
                quantidadeAvesPorCaixaRealizada = pedido.Produtos.FirstOrDefault()?.QuantidadePorCaixaRealizada ?? 0;
                quantidadeAvesCarregadas = (int)(pedido.Produtos.FirstOrDefault()?.Quantidade ?? 0);
                quantidadePlanejada = (int)(pedido.Produtos.FirstOrDefault()?.QuantidadePlanejada ?? 0);
                caixasVazias = (int)(pedido.Produtos.FirstOrDefault()?.QuantidadeCaixasVaziasRealizada ?? 0);
                caixasVaziasPlanejada = (int)(pedido.Produtos.FirstOrDefault()?.QuantidadeCaixasVazias ?? 0);
            }
        }

        private Dominio.ObjetosDeValor.EDI.GTA.DadosGTA ObterDadosGTA(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal guiaTransporteAnimal = repCargaEntregaGuiaTransporteAnimal.ConsultarCargasEntrega(cargaEntrega.Codigo, null).FirstOrDefault();
            if (guiaTransporteAnimal != null)
            {
                return new Dominio.ObjetosDeValor.EDI.GTA.DadosGTA
                {
                    Numero = guiaTransporteAnimal.NumeroNotaFiscal,
                    CodigoBarras = guiaTransporteAnimal.CodigoBarras,
                    UF = guiaTransporteAnimal.Estado.Sigla,
                    Serie = guiaTransporteAnimal.Serie,
                    QuantidadeSuinos = guiaTransporteAnimal.Quantidade,
                };
            }

            return null;
        }

        private Dominio.ObjetosDeValor.EDI.GTA.Pesquisa ObterPesquisaDesembarque(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unidadeTrabalho);
            //Dados da pesquisa respondida no app
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntas = repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo, TipoCheckList.Desembarque);

            if (perguntas.Count > 0)
            {
                Dominio.ObjetosDeValor.EDI.GTA.Pesquisa pesquisa = new Dominio.ObjetosDeValor.EDI.GTA.Pesquisa();
                Carga.ControleEntrega.CargaEntregaCheckList servicoCargaEntregaCheckList = new Carga.ControleEntrega.CargaEntregaCheckList(unidadeTrabalho);

                pesquisa = new Dominio.ObjetosDeValor.EDI.GTA.Pesquisa();
                pesquisa.Perguntas = new List<Dominio.ObjetosDeValor.EDI.GTA.Pergunta>();

                foreach (var pergunta in perguntas)
                {
                    pesquisa.Perguntas.Add(new Dominio.ObjetosDeValor.EDI.GTA.Pergunta
                    {
                        CodigoIntegracao = pergunta.CodigoIntegracao,
                        TipoDescricao = pergunta.Tipo.ObterDescricao(),
                        TipoCodigo = pergunta.Tipo.ObterCodigo(),
                        Descricao = pergunta.Descricao,
                        RespostaDescricao = servicoCargaEntregaCheckList.ObterRespostaDescricaoPergunta(pergunta),
                        RespostaCodigosIntegracao = servicoCargaEntregaCheckList.ObterRespostasCodigosPergunta(pergunta),
                    });
                }

                return pesquisa;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.EDI.GTA.Pesquisa ObterPesquisa(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unidadeTrabalho);
            //Dados da pesquisa respondida no app
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntas = repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo);

            if (perguntas.Count > 0)
            {
                Dominio.ObjetosDeValor.EDI.GTA.Pesquisa pesquisa = new Dominio.ObjetosDeValor.EDI.GTA.Pesquisa();
                Carga.ControleEntrega.CargaEntregaCheckList servicoCargaEntregaCheckList = new Carga.ControleEntrega.CargaEntregaCheckList(unidadeTrabalho);

                pesquisa = new Dominio.ObjetosDeValor.EDI.GTA.Pesquisa();
                pesquisa.Perguntas = new List<Dominio.ObjetosDeValor.EDI.GTA.Pergunta>();

                foreach (var pergunta in perguntas)
                {
                    pesquisa.Perguntas.Add(new Dominio.ObjetosDeValor.EDI.GTA.Pergunta
                    {
                        CodigoIntegracao = pergunta.CodigoIntegracao,
                        TipoDescricao = pergunta.Tipo.ObterDescricao(),
                        TipoCodigo = pergunta.Tipo.ObterCodigo(),
                        Descricao = pergunta.Descricao,
                        RespostaDescricao = servicoCargaEntregaCheckList.ObterRespostaDescricaoPergunta(pergunta),
                        RespostaCodigosIntegracao = servicoCargaEntregaCheckList.ObterRespostasCodigosPergunta(pergunta),
                    });
                }

                return pesquisa;
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.EDI.GTA.Pedido> ObterPedidos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, Repositorio.UnitOfWork unidadeTrabalho)
        {
            /*
             * A estrutura de divisão de capacidade está bem bagunçada. Leia o código e comentários com atenção.
             */

            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repositorioCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.EDI.GTA.Pedido> pedidos = new List<Dominio.ObjetosDeValor.EDI.GTA.Pedido>();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaPedidosParada = (from obj in cargaEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> listaDivisaoCapacidade = cargaEntrega.Carga.ModeloVeicularCargaVeiculo?.DivisoesCapacidade?.ToList() ?? new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> listaDivisaoCapacidadeEntrega = repositorioCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(cargaEntrega.Carga.Codigo);

            // Para cada pedido da parada atual
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaPedidoParada in cargaPedidosParada)
            {
                Dominio.ObjetosDeValor.EDI.GTA.Pedido dadosPedido = new Dominio.ObjetosDeValor.EDI.GTA.Pedido();
                dadosPedido.Numero = cargaPedidoParada.CargaPedido.Pedido.Numero;
                dadosPedido.SuinosPorBox = new List<Dominio.ObjetosDeValor.EDI.GTA.SuinoPorBox>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtosPedido = (from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == cargaPedidoParada.CargaPedido.Codigo select obj).ToList();

                // Cada produto do pedido atual
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto produtoPedido in produtosPedido)
                {
                    if (produtoPedido.ImunoPlanejado.HasValue)
                        dadosPedido.ImunoPlanejado += produtoPedido.ImunoPlanejado.Value;

                    if (produtoPedido.ImunoRealizado.HasValue)
                        dadosPedido.ImunoRealizado += produtoPedido.ImunoRealizado.Value;

                    // Para cada divisão do caminhão do produto atual (é pra ser apenas suínos, geralmente 12 boxes)
                    foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade divisaoCapacidade in listaDivisaoCapacidade)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade divisaoCapacidadeProduto = (
                            from o in listaDivisaoCapacidadeEntrega
                            where o.CargaPedidoProduto.Codigo == produtoPedido.Codigo && o.ModeloVeicularCargaDivisaoCapacidade.Codigo == divisaoCapacidade.Codigo
                            select o
                        ).FirstOrDefault();

                        if (divisaoCapacidadeProduto == null)
                            continue;

                        // Adiciona os dados dos suínos que estão em cada Box do caminhão
                        dadosPedido.SuinosPorBox.Add(new Dominio.ObjetosDeValor.EDI.GTA.SuinoPorBox
                        {
                            Descricao = divisaoCapacidadeProduto.ModeloVeicularCargaDivisaoCapacidade?.Descricao ?? "",
                            Quantidade = (int)divisaoCapacidadeProduto.Quantidade,
                            QuantidadePlanejada = (int)divisaoCapacidadeProduto.QuantidadePlanejada,
                            Coluna = divisaoCapacidade.Coluna,
                            Piso = divisaoCapacidade.Piso,
                        });
                    }
                }

                dadosPedido.NumeroPedidoEmbarcador = cargaPedidoParada.CargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty;
                dadosPedido.NumeroProtocoloIntegracaoPedido = cargaPedidoParada.CargaPedido?.Pedido?.Protocolo ?? 0;

                pedidos.Add(dadosPedido);
            }

            return pedidos;
        }

        #endregion
    }
}