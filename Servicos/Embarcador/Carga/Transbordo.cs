using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class Transbordo
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido _configuracaoPedido;

        #endregion

        #region Construtores

        public Transbordo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaTransbordo(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(_unitOfWork);

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(_unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga = transbordo.Carga;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaTransbordo = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                AgConfirmacaoUtilizacaoCredito = cargaAntiga.AgConfirmacaoUtilizacaoCredito,
                AgImportacaoCTe = cargaAntiga.AgImportacaoCTe,
                AgImportacaoMDFe = cargaAntiga.AgImportacaoMDFe,
                AguardandoEmissaoDocumentoAnterior = cargaAntiga.AguardandoEmissaoDocumentoAnterior,
                AutorizouTodosCTes = cargaAntiga.AutorizouTodosCTes,
                CargaCancelamento = cargaAntiga.CargaCancelamento,
                CargaFechada = cargaAntiga.CargaFechada,
                CargaIntegradaEmbarcador = cargaAntiga.CargaIntegradaEmbarcador,
                CargaTransbordo = cargaAntiga.CargaTransbordo,
                CodigoCargaEmbarcador = cargaAntiga.CodigoCargaEmbarcador,
                ControlaTempoParaEmissao = cargaAntiga.ControlaTempoParaEmissao,
                Terceiro = cargaAntiga.Terceiro,
                DataCarregamentoCarga = cargaAntiga.DataCarregamentoCarga,
                DataCriacaoCarga = DateTime.Now,
                DataEnvioUltimaNFe = cargaAntiga.DataEnvioUltimaNFe,
                DataRecebimentoUltimaNFe = cargaAntiga.DataRecebimentoUltimaNFe,
                DataFechamentoCarga = cargaAntiga.DataFechamentoCarga,
                DataFinalizacaoEmissao = cargaAntiga.DataFinalizacaoEmissao,
                DataFinalPrevisaoCarregamento = cargaAntiga.DataFinalPrevisaoCarregamento,
                DataInicialPrevisaoCarregamento = cargaAntiga.DataInicialPrevisaoCarregamento,
                DataPrevisaoTerminoCarga = cargaAntiga.DataPrevisaoTerminoCarga,
                DataInicioViagemPrevista = transbordo?.DataTransbordo ?? DateTime.Now, //#68279
                //EmitindoCTes = cargaAntiga.EmitindoCTes,
                Empresa = transbordo.Empresa,
                ExigeNotaFiscalParaCalcularFrete = cargaAntiga.ExigeNotaFiscalParaCalcularFrete,
                Filial = cargaAntiga.Filial,
                FreteDeTerceiro = cargaAntiga.FreteDeTerceiro,
                //GerandoIntegracoes = cargaAntiga.GerandoIntegracoes,
                GrupoPessoaPrincipal = cargaAntiga.GrupoPessoaPrincipal,
                ModeloVeicularCarga = cargaAntiga.ModeloVeicularCarga,
                MotivoPendencia = cargaAntiga.MotivoPendencia,
                MotivoPendenciaFrete = cargaAntiga.MotivoPendenciaFrete,
                NaoExigeVeiculoParaEmissao = cargaAntiga.NaoExigeVeiculoParaEmissao,
                Operador = cargaAntiga.Operador,
                PossuiPendencia = false,
                PrioridadeEnvioIntegracao = cargaAntiga.PrioridadeEnvioIntegracao,
                problemaCTE = cargaAntiga.problemaCTE,
                problemaAverbacaoCTe = cargaAntiga.problemaAverbacaoCTe,
                problemaMDFe = cargaAntiga.problemaMDFe,
                problemaNFS = cargaAntiga.problemaNFS,
                Rota = cargaAntiga.Rota,
                SegmentoGrupoPessoas = cargaAntiga.SegmentoGrupoPessoas,
                SegmentoModeloVeicularCarga = cargaAntiga.SegmentoModeloVeicularCarga,
                FaixaTemperatura = cargaAntiga.FaixaTemperatura,
                TabelaFrete = cargaAntiga.TabelaFrete,
                TabelaFreteFilialEmissora = cargaAntiga.TabelaFreteFilialEmissora,
                //TipoContratacaoCarga = cargaAntiga.TipoContratacaoCarga,
                TipoDeCarga = transbordo.TipoDeCarga ?? cargaAntiga.TipoDeCarga,
                TipoFreteEscolhido = cargaAntiga.TipoFreteEscolhido,
                TipoOperacao = transbordo.TipoOperacao ?? cargaAntiga.TipoOperacao,
                ValorFrete = cargaAntiga.ValorFrete,
                ValorFreteAPagar = cargaAntiga.ValorFreteAPagar,
                ValorFreteEmbarcador = cargaAntiga.ValorFreteEmbarcador,
                ValorFreteLeilao = cargaAntiga.ValorFreteLeilao,
                ValorFreteLiquido = cargaAntiga.ValorFreteLiquido,
                ValorFreteOperador = cargaAntiga.ValorFreteOperador,
                ValorFreteTabelaFrete = cargaAntiga.ValorFreteTabelaFrete,
                ValorICMS = cargaAntiga.ValorICMS,
                ValorISS = cargaAntiga.ValorISS,
                ValorRetencaoISS = cargaAntiga.ValorRetencaoISS,
                ValorIBSEstadual = cargaAntiga.ValorIBSEstadual,
                ValorIBSMunicipal = cargaAntiga.ValorIBSMunicipal,
                ValorCBS = cargaAntiga.ValorCBS,
                VeiculoIntegradoEmbarcador = cargaAntiga.VeiculoIntegradoEmbarcador,
                Veiculo = transbordo.Veiculo,
                EmiteMDFeFilialEmissora = cargaAntiga.EmiteMDFeFilialEmissora,
                LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = cargaAntiga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora,
                EmpresaFilialEmissora = cargaAntiga.EmpresaFilialEmissora,
                ControleNumeracao = repCarga.BuscarUltimoControleNumeracao(cargaAntiga.CodigoCargaEmbarcador) + 1,
                SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                Motoristas = transbordo.Motoristas.ToList(),
                UtilizarCTesAnterioresComoCTeFilialEmissora = cargaAntiga.UtilizarCTesAnterioresComoCTeFilialEmissora,
                Parqueada = cargaAntiga.Parqueada,

            };

            if (transbordo?.ClonaDataInicioViagemEntrega ?? false)
                cargaTransbordo.DataInicioViagem = cargaAntiga.DataInicioViagem;

            cargaTransbordo.VeiculosVinculados = transbordo.VeiculosVinculados.ToList();
            cargaTransbordo.CargaTransbordo = true;
            cargaTransbordo.DadosSumarizados = null;
            repCarga.Inserir(cargaTransbordo, _auditado);

            transbordo.CargaGerada = cargaTransbordo;
            repTransbordo.Atualizar(transbordo);

            new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(cargaTransbordo.Rota);

            // Replicar as fronteiras da carga antiga pra nova
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteirasCargaAntiga = serCargaFronteira.ObterFronteirasPorCarga(cargaAntiga);
            repCargaFronteira.CopiarFronteirasParaCarga(fronteirasCargaAntiga, cargaTransbordo);

            cargaTransbordo.Protocolo = cargaTransbordo.Codigo;
            if (transbordo.Entregas.Count > 0)
                cargaTransbordo.OrdemRoteirizacaoDefinida = true;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoXMLNotaFiscalCTePorCarga(cargaAntiga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaCargaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(cargaAntiga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCarga(cargaAntiga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoPedidoNotaFiscalsCarga = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCarga(cargaAntiga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargasEntregasPedidos = repositorioCargaEntregaPedido.BuscarPorCargaEntregas(transbordo.Entregas.Select(x => x.Codigo).ToList());
            List<CargaPedidoClonagem> cargaPedidosJaClonados = new List<CargaPedidoClonagem>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasTransbordosJaGeradas = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTransbordo = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscalsClonadas = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in transbordo.Entregas)
            {
                List<CargaPedidoClonagem> cargaPedidosClonados = new List<CargaPedidoClonagem>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargasEntregasPedidos where obj.CargaEntrega.Codigo == entrega.Codigo select obj.CargaPedido).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    CargaPedidoClonagem cargaPedidoClonagem = (from obj in cargaPedidosJaClonados where obj.Clonado.Codigo == cargaPedido.Codigo select obj).FirstOrDefault();

                    if (cargaPedidoClonagem == null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoClonado = new Dominio.Entidades.Embarcador.Cargas.CargaPedido();
                        cargaPedidoClonado = cargaPedido.Clonar(false);
                        Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoClonado);
                        cargaPedidoClonado.Carga = cargaTransbordo;
                        cargaPedidoClonado.CargaOrigem = cargaTransbordo;
                        cargaPedidoClonado.Pedido = cargaPedido.Pedido;
                        cargaPedidoClonado.Origem = transbordo.localidadeTransbordo;

                        if (cargaPedidoClonado.ReentregaSolicitada)
                        {
                            cargaPedidoClonado.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                            cargaPedidoClonado.CTesEmitidos = true;
                        }

                        repCargaPedido.Inserir(cargaPedidoClonado);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> listaStagesPedido = repPedidoStage.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                        foreach (var stagePedido in listaStagesPedido)
                        {
                            if (stagePedido.Stage != null)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Stage stageClonado = new Dominio.Entidades.Embarcador.Pedidos.Stage();
                                stageClonado = stagePedido.Stage.Clonar();
                                stageClonado.CargaDT = cargaTransbordo;
                                stageClonado.StageAgrupamento = null;

                                repStage.Inserir(stageClonado);

                                Dominio.Entidades.Embarcador.Pedidos.PedidoStage stagePedidoNovo = new Dominio.Entidades.Embarcador.Pedidos.PedidoStage();
                                stagePedidoNovo.Stage = stageClonado;
                                stagePedidoNovo.Pedido = cargaPedido.Pedido;
                                repPedidoStage.Inserir(stagePedidoNovo);
                            }
                        }

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedido.Produtos)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProdutoTransbordo = cargaPedidoProduto.Clonar();
                            cargaPedidoProdutoTransbordo.CargaPedido = cargaPedidoClonado;
                            repCargaPedidoProduto.Inserir(cargaPedidoProdutoTransbordo);
                            //SetarPropriedadesObjetoDuplicar(cargaPedidoProdutoTransbordo);
                        }
                        cargaPedidoClonagem = new CargaPedidoClonagem(cargaPedido, cargaPedidoClonado);
                        cargaPedidosJaClonados.Add(cargaPedidoClonagem);
                        cargaPedidosTransbordo.Add(cargaPedidoClonado);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = (from obj in pedidoXMLNotaCargaFiscals where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscals)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalTransbordo = pedidoXMLNotaFiscal.Clonar();
                            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoXMLNotaFiscalTransbordo);
                            pedidoXMLNotaFiscalTransbordo.CargaPedido = cargaPedidoClonado;
                            repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscalTransbordo);
                            pedidoXMLNotaFiscalsClonadas.Add(pedidoXMLNotaFiscalTransbordo);
                        }
                    }

                    cargaPedidosClonados.Add(cargaPedidoClonagem);
                }


                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = (from obj in cargaPedidoXMLNotaFiscalCTes where cargaPedidos.Contains(obj.PedidoXMLNotaFiscal.CargaPedido) select obj.CargaCTe).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeParaTransbordo = repCargaCte.BuscarPorCodigo(cargaCte.Codigo);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteTransbordo = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
                    cargaCteTransbordo.CTe = cargaCTeParaTransbordo.CTe;
                    cargaCteTransbordo.DataVinculoCarga = cargaCTeParaTransbordo.DataVinculoCarga;
                    cargaCteTransbordo.PreCTe = cargaCTeParaTransbordo.PreCTe;
                    cargaCteTransbordo.CargaCTeFilialEmissora = null;
                    cargaCteTransbordo.SistemaEmissor = cargaCTeParaTransbordo.SistemaEmissor;
                    cargaCteTransbordo.Carga = cargaTransbordo;
                    cargaCteTransbordo.CargaOrigem = cargaTransbordo;

                    if (cargasTransbordosJaGeradas.Any(o => o.CTe.Codigo == (cargaCTeParaTransbordo?.CTe?.Codigo ?? 0) && o.Carga.Codigo == (cargaCteTransbordo?.Carga?.Codigo ?? 0)))
                        continue;
                    else
                        repCargaCte.Inserir(cargaCteTransbordo);

                    cargasTransbordosJaGeradas.Add(cargaCteTransbordo);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = null;
                    if (cargaCTeParaTransbordo.CargaCTeFilialEmissora != null && !cargaCTes.Any(o => o.Codigo == (cargaCTeParaTransbordo?.CargaCTeFilialEmissora?.Codigo ?? 0)))
                    {
                        cargaCTeFilialEmissora = repCargaCte.BuscarPorCodigo(cargaCTeParaTransbordo.CargaCTeFilialEmissora.Codigo);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteTransbordoFillialEmissora = new Dominio.Entidades.Embarcador.Cargas.CargaCTe(); //cargaCTeFilialEmissora.Clonar();
                        cargaCteTransbordoFillialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCteTransbordo;
                        cargaCteTransbordoFillialEmissora.Carga = cargaTransbordo;
                        cargaCteTransbordoFillialEmissora.CargaOrigem = cargaTransbordo;
                        cargaCteTransbordoFillialEmissora.CTe = cargaCTeFilialEmissora.CTe;
                        cargaCteTransbordoFillialEmissora.DataVinculoCarga = cargaCTeFilialEmissora.DataVinculoCarga;
                        cargaCteTransbordoFillialEmissora.PreCTe = cargaCTeFilialEmissora.PreCTe;
                        cargaCteTransbordoFillialEmissora.SistemaEmissor = cargaCTeFilialEmissora.SistemaEmissor;
                        repCargaCte.Inserir(cargaCteTransbordoFillialEmissora);

                        cargaCteTransbordo.CargaCTeFilialEmissora = cargaCteTransbordoFillialEmissora;
                        repCargaCte.Atualizar(cargaCteTransbordo);
                    }

                    foreach (CargaPedidoClonagem cargaPedidoClonado in cargaPedidosClonados)
                    {
                        ReplicarNotas(cargaPedidoClonado, pedidoXMLNotaCargaFiscals, cargaCteTransbordo, _unitOfWork, pedidoXMLNotaFiscalsClonadas);
                        if (pedidoCTeParaSubContratacao.Count > 0)
                        {
                            ReplicarCTeAnterior(cargaPedidoClonado, pedidoXMLNotaFiscalsClonadas, pedidoCTeParaSubContratacaoPedidoNotaFiscalsCarga, pedidoCTeParaSubContratacao, false, _unitOfWork);
                            if (cargaCTeFilialEmissora != null)
                                ReplicarCTeAnterior(cargaPedidoClonado, pedidoXMLNotaFiscalsClonadas, pedidoCTeParaSubContratacaoPedidoNotaFiscalsCarga, pedidoCTeParaSubContratacao, true, _unitOfWork);
                        }
                    }
                }
            }

            ClonarCanhotos(cargaAntiga, cargaTransbordo, _unitOfWork);
            svcCarga.FecharCarga(cargaTransbordo, _unitOfWork, _tipoServicoMultisoftware, clienteMultisoftware, true);
            servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(cargaTransbordo, cargaPedidosTransbordo, _unitOfWork, _tipoServicoMultisoftware, ObterConfiguracaoPedido());
            CancelarValePedagioCargaAntiga(cargaAntiga);

            cargaTransbordo.CargaFechada = true;
            repCarga.Atualizar(cargaTransbordo);

            return cargaTransbordo;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void ClonarCanhotos(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Dominio.Entidades.Embarcador.Cargas.Carga cargaTransbordo, Repositorio.UnitOfWork _unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> listaCanhotos = repCanhoto.BuscarPorCarga(cargaAntiga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in listaCanhotos)
            {
                //Não pode duplicar o canhoto, tem que migrar para carga nova
                //Dominio.Entidades.Embarcador.Canhotos.Canhoto copiaCanhoto = canhoto.Clonar();
                //copiaCanhoto.Codigo = 0;
                //copiaCanhoto.Carga = cargaTransbordo;
                //repCanhoto.Inserir(copiaCanhoto, Auditado);
                canhoto.Carga = cargaTransbordo;
                repCanhoto.Atualizar(canhoto, _auditado);
            }
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido ObterConfiguracaoPedido()
        {
            if (_configuracaoPedido == null)
                _configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoPedido;
        }

        private void ReplicarNotas(CargaPedidoClonagem cargaPedidoClonado, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaCargaFiscals, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork _unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscalsClonadas)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = (from obj in pedidoXMLNotaFiscalsClonadas where obj.CargaPedido.Codigo == cargaPedidoClonado.Clone.Codigo select obj).ToList();
            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscalsClonadas = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscals)
            {
                //Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalTransbordo = pedidoXMLNotaFiscal.Clonar();
                //Utilidades.Object.DefinirListasGenericasComoNulas(pedidoXMLNotaFiscalTransbordo);
                //pedidoXMLNotaFiscalTransbordo.CargaPedido = cargaPedidoClonado.Clone;
                //repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscalTransbordo);
                //pedidoXMLNotaFiscalsClonadas.Add(pedidoXMLNotaFiscalTransbordo);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalTransbordo = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                cargaPedidoXMLNotaFiscalTransbordo.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                cargaPedidoXMLNotaFiscalTransbordo.CargaCTe = cargaCTe;
                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalTransbordo);
            }

            //return pedidoXMLNotaFiscalsClonadas;
        }

        private void ReplicarCTeAnterior(CargaPedidoClonagem cargaPedidoClonado, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasClonadas, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoPedidoNotaFiscalsCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTeParaSubContratacaosCarga, bool filialEmissora, Repositorio.UnitOfWork _unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(_unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTeParaSubContratacaos = (from obj in pedidoCTeParaSubContratacaosCarga where obj.CteSubContratacaoFilialEmissora == filialEmissora && obj.CargaPedido.Codigo == cargaPedidoClonado.Clonado.Codigo select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTeParaSubContratacaos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoTransbordo = pedidoCTeParaSubContratacao.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(pedidoCTeParaSubContratacaoTransbordo);
                pedidoCTeParaSubContratacaoTransbordo.CargaPedido = cargaPedidoClonado.Clone;
                repPedidoCTeParaSubContratacao.Inserir(pedidoCTeParaSubContratacaoTransbordo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoPedidoNota = (from obj in pedidoCTeParaSubContratacaoPedidoNotaFiscalsCarga where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoPedidoNota)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscalTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal();
                    pedidoCTeParaSubContratacaoPedidoNotaFiscalTransbordo.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacaoTransbordo;
                    pedidoCTeParaSubContratacaoPedidoNotaFiscalTransbordo.PedidoXMLNotaFiscal = (from obj in notasClonadas where obj.XMLNotaFiscal.Codigo == pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo select obj).FirstOrDefault();
                    repPedidoCTeParaSubContratacaoPedidoNotaFiscal.Inserir(pedidoCTeParaSubContratacaoPedidoNotaFiscalTransbordo);
                }
            }
        }

        private void CancelarValePedagioCargaAntiga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga)
        {
            if (!new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).ExisteCancelarValePedagioQuandoGerarCargaTransbordo())
                return;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracoesValePedagio = repCargaIntegracaoValePedagio.BuscarPorCarga(cargaAntiga.Codigo);

            if (cargaIntegracoesValePedagio == null || cargaIntegracoesValePedagio.Count == 0)
                return;

            foreach (var cargaIntegracao in cargaIntegracoesValePedagio)
            {
                if ((cargaIntegracao.SituacaoValePedagio == SituacaoValePedagio.Confirmada) ||
                    (cargaIntegracao.SituacaoValePedagio == SituacaoValePedagio.Comprada && cargaIntegracao.TipoIntegracao.Tipo.IntegraCancelamentoValePedagio()))
                {
                    cargaIntegracao.SituacaoValePedagio = SituacaoValePedagio.EmCancelamento;
                    repCargaIntegracaoValePedagio.Atualizar(cargaIntegracao);
                }
            }

        }

        #endregion Métodos Privados
    }

    public class CargaPedidoClonagem
    {

        public CargaPedidoClonagem(Dominio.Entidades.Embarcador.Cargas.CargaPedido clonado, Dominio.Entidades.Embarcador.Cargas.CargaPedido clone)
        {
            this.Clonado = clonado;
            this.Clone = clone;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido Clonado { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido Clone { get; set; }
    }
}
