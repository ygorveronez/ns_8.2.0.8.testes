using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Carga
{
    public class FretePedidoAgrupado
    {
        #region Propriedades Privadas

        private Repositorio.UnitOfWork _unitOfWork;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;

        #endregion

        #region Construtores

        public FretePedidoAgrupado(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracao = configuracao;
        }

        #endregion        

        #region Métodos Públicos

        public bool CalcularFrete(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

            Servicos.Embarcador.Carga.Frete svcFrete = new Embarcador.Carga.Frete(_tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.ComponetesFrete svcCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new RateioFrete(_unitOfWork);
            Servicos.Embarcador.Carga.FreteCTeSubcontratacao svcFreteCTeSubcontratacao = new FreteCTeSubcontratacao(_unitOfWork);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(_unitOfWork);
            Servicos.Embarcador.Carga.CargaAprovacaoFrete svcCargaAprovacaoFrete = new CargaAprovacaoFrete(_unitOfWork, _configuracao);
            Servicos.Embarcador.Carga.ICMS svcCargaICMS = new Servicos.Embarcador.Carga.ICMS(_unitOfWork);
            Servicos.Embarcador.Carga.RateioNotaFiscal svcRateioNotaFiscal = new RateioNotaFiscal(_unitOfWork);
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(_unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();

            bool cargaPossuiAjudante = cargaPedidos.Any(obj => obj.Pedido.Ajudante);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCarga = svcFrete.ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, calculoFreteFilialEmissora, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware, _configuracao);

            List<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> cargaPedidosAgrupados = cargaPedidos.GroupBy(cargaPedido => new { ClienteOutroEndereco = cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo ?? 0, Remetente = cargaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0D, Destinatario = cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0D, cargaPedido.TipoTomador, Destino = cargaPedido.Destino?.Codigo, StageRelevanteCusto = cargaPedido.StageRelevanteCusto?.Codigo }).Select(grupo => grupo.ToList()).ToList();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();


            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = tabelaFrete.ContratoFreteTransportador;
            if ((carga?.TipoOperacao?.PermiteUtilizarEmContratoFrete ?? false) && contratoFreteTransportador == null)
                contratoFreteTransportador = svcFrete.ObterContratoCombativel(tabelaFrete, carga, _unitOfWork);

            if (carga.TipoFreteEscolhido != TipoFreteEscolhido.Embarcador)
            {
                if (contratoFreteTransportador != null &&
                 contratoFreteTransportador.Ativo &&
                 !contratoFreteTransportador.ExigeTabelaFreteComValor &&
                 ((contratoFreteTransportador.FranquiaValorKM > 0) ||
                  (contratoFreteTransportador.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui && contratoFreteTransportador.DeduzirValorPorCarga) ||
                  (contratoFreteTransportador.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista) ||
                  (configuracao.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)))
                {
                    carga.ValorFrete = 0;
                    carga.ValorFreteAPagar = 0;

                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametrosContratoFreteTransportadorValorFreteMinimo = svcFrete.ObterParametrosContratoFreteTransportadorValorFreteMinimo(contratoFreteTransportador, parametrosCarga);

                    string retornoContrato = Servicos.Embarcador.Carga.ContratoFrete.CalcularFretePorContratoFrete(contratoFreteTransportador, parametrosContratoFreteTransportadorValorFreteMinimo, carga, cargaPedidos, configuracao, apenasVerificar, calculoFreteFilialEmissora, _tipoServicoMultisoftware, _unitOfWork);

                    if (!string.IsNullOrEmpty(retornoContrato)) { }
                    ///O que fazer ? 
                }
            }

            if (tabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado)
            {
                cargaPedidosAgrupados = cargaPedidos.Where(obj => obj.Recebedor != null).GroupBy(cargaPedido => new { Recebedor = cargaPedido.Recebedor }).Select(grupo => grupo.ToList()).ToList();
                cargaPedidosAgrupados.AddRange(cargaPedidos.Where(obj => obj.Recebedor == null).GroupBy(cargaPedido => new { ClienteOutroEndereco = cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo ?? 0, Remetente = cargaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0D, Destinatario = cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0D, cargaPedido.TipoTomador, Destino = cargaPedido.Destino?.Codigo }).Select(grupo => grupo.ToList()).ToList());
            }

            //#74140 cargas do tipo documento transporte só calcula frete para stages relevantes, mas para cargas do consolidado (subcargas) pode calcular.
            if (carga.TipoDocumentoTransporte != null && carga.DadosSumarizados?.CargaTrecho != CargaTrechoSumarizada.SubCarga)
                cargaPedidosAgrupados = cargaPedidos.Where(obj => obj.StageRelevanteCusto != null).GroupBy(cargaPedido => new { ClienteOutroEndereco = cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo ?? 0, Remetente = cargaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0D, Destinatario = cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0D, cargaPedido.TipoTomador, Destino = cargaPedido.Destino?.Codigo, StageRelevanteCusto = cargaPedido.StageRelevanteCusto?.Codigo }).Select(grupo => grupo.ToList()).ToList();

            if (parametrosCarga == null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete
                {
                    tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente,
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete,
                    mensagem = mensagemRetorno.Insert(0, "Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis (exemplo, somente pedidos de pallet).").ToString()
                };

                return true;
            }

            parametrosCarga.NumeroPedidos = cargaPedidos.Count;
            parametrosCarga.FormulaRateio = cargaPedidos[0].FormulaRateio;

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && !apenasVerificar)
            {
                svcRateioFrete.ZerarValoresDaCarga(carga, calculoFreteFilialEmissora, _unitOfWork);
                repCargaPedidoComponenteFrete.DeletarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            }

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesPorCarga = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();
            List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidos select obj.Origem.Estado.Sigla).Distinct().ToList());
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = svcCargaICMS.ObterProdutosCargaContidosEmRegras(carga, _unitOfWork);
            List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora(cargaPedidos.Where(o => o.CargaPedidoFilialEmissora).Select(o => o.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), _unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, _unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePedagio = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);

            int distanciaPercurso = repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = null;
            if (carga.Carregamento != null)
                carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carga.Carregamento.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao = repPedidoCTeParaSubContratacao.BuscarTotalSumarizadoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete = repCargaPedidoRotaFrete.BuscarPorCarga(carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido> fretesCargaPedido = new List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido>();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelaFreteClientes = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais = null;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            if (!Servicos.Embarcador.Carga.CTeSimplificado.ValidarCTeSimplificado(cargaPedidos, _unitOfWork, _tipoServicoMultisoftware, _configuracao))
                Servicos.Embarcador.Carga.CTeGlobalizado.ValidarCTeGlobalizadoPorDestinatario(cargaPedidos, _unitOfWork, _tipoServicoMultisoftware, _configuracao);

            bool abriuTransacao = false;

            int distanciaCargaEmMetros = repositorioCargaRotaFretePontosPassagem.BuscarDistanciaPorCarga(carga.Codigo);

            int distanciaPorCarga = (distanciaCargaEmMetros / 1000);

            Dictionary<double, int> cacheDistancias = new Dictionary<double, int>();

            for (int i = 0; i < cargaPedidosAgrupados.Count; i++)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCalculo = cargaPedidosAgrupados[i];

                double cpfCnpj = cargaPedidosCalculo.First().ClienteEntrega?.CPF_CNPJ ?? 0;

                int distanciaKmPedido = ObterDistanciaPorPedidoAgrupadoComCache(cacheDistancias, carga.Codigo, cpfCnpj, repositorioCargaRotaFretePontosPassagem);

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFrete(tabelaFrete, carga, cargaPedidosCalculo, distanciaPercurso, carregamentoRoteirizacao, cargaPedidoQuantidadesCarga, cargaPedidosValoresNotas, cargaPedidosValoresCTesSubcontratacao, cargaPedidoRotasFrete, calculoFreteFilialEmissora, 0m, parametrosCarga, configuracaoGeralCarga, distanciaKmPedido, distanciaPorCarga);
                if (tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos && cargaPossuiAjudante)
                    parametrosCalculo.ParametrosCarga.NecessarioAjudante = true;

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, _tipoServicoMultisoftware);

                if (!svcFreteCliente.PermiteCalcularFrete(tabelasCliente) && cargaPedidosCalculo.Any(o => o.TipoContratacaoCarga != TipoContratacaoCarga.SubContratada))
                {
                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete
                        {
                            tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente,
                            situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete
                        };

                        if (tabelasCliente.Count <= 0)
                            retorno.mensagem = mensagemRetorno.Insert(0, $"Não foi localizada uma configuração de frete para a tabela de frete {tabelaFrete.Descricao} compatível com as configurações do(s) pedido(s) {string.Join(", ", cargaPedidosCalculo.Select(o => _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? o.Pedido.Numero.ToString() : o.Pedido.NumeroPedidoEmbarcador))}.\n").ToString();
                        else if (tabelasCliente.Count > 1)
                            retorno.mensagem = mensagemRetorno.Insert(0, $"Foram encontradas múltiplas configurações de valores de frete (códigos {string.Join(", ", tabelasCliente.Select(o => o.Codigo))}) disponíveis para o(s) pedido(s) {string.Join(", ", cargaPedidosCalculo.Select(o => _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? o.Pedido.Numero.ToString() : o.Pedido.NumeroPedidoEmbarcador))} na tabela de frete " + tabelaFrete.Descricao + ".").ToString(); // SQL-INJECTION-SAFE
                        else if (tabelasCliente[0].Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao)
                            retorno.mensagem = mensagemRetorno.Insert(0, $"A tabela de frete {tabelaFrete.Descricao} ainda não foi aprovada e não pode ser utilizada no(s) pedido(s) {string.Join(", ", cargaPedidosCalculo.Select(o => _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? o.Pedido.Numero.ToString() : o.Pedido.NumeroPedidoEmbarcador))}.").ToString();

                        if (!apenasVerificar)
                        {
                            carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

                            if (svcCarga.VerificarSeCargaEstaNaLogistica(carga, _tipoServicoMultisoftware))
                            {
                                carga.PossuiPendencia = true;
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 6 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                            }

                            carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                            if (!calculoFreteFilialEmissora)
                                carga.TabelaFrete = null;
                            else
                                carga.TabelaFreteFilialEmissora = null;

                            repCarga.Atualizar(carga);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> cargaPedidoTabelasClientes = repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaPedidosCalculo.Select(o => o.Codigo).ToList(), calculoFreteFilialEmissora);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaCliente in cargaPedidoTabelasClientes)
                                repCargaPedidoTabelaFreteCliente.Deletar(cargaPedidoTabelaCliente);

                            if (!calculoFreteFilialEmissora)
                            {
                                abriuTransacao = false;
                                if (!_unitOfWork.IsActiveTransaction())
                                {
                                    _unitOfWork.Start();
                                    abriuTransacao = true;
                                }

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosCalculo)
                                {
                                    if (pedidoNotasFiscais == null)
                                        pedidoNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                                    svcFrete.AjustarCargaPedidoTabelaNaoExiste(cargaPedido, pedidoNotasFiscais, _unitOfWork);
                                }

                                if (abriuTransacao)
                                    _unitOfWork.CommitChanges();

                                Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, _configuracao, _tipoServicoMultisoftware, _unitOfWork);
                            }
                        }

                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                            return true;
                    }
                }
                else if (cargaPedidosCalculo.Any(o => o.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada) && ((tabelasCliente.Count == 0) || (tabelasCliente[0] == null)))
                {
                    retorno = svcFreteCTeSubcontratacao.BuscarTabelaFreteSubcontratado(carga, cargaPedidos, _configuracao, apenasVerificar, _tipoServicoMultisoftware, _unitOfWork);

                    if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
                    {
                        retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaSubContratacao;
                        return true;
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelasCliente[0];

                    Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido = new Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido
                    {
                        cargaPedidos = cargaPedidosCalculo,
                        parametrosCalculo = parametrosCalculo,
                        tabelaFreteCliente = tabelaFreteCliente
                    };

                    if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                        svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, _tipoServicoMultisoftware, _configuracao);
                    else
                        svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, _tipoServicoMultisoftware, _configuracao);

                    freteCargaPedido.dadosCalculoFrete = dados;
                    fretesCargaPedido.Add(freteCargaPedido);
                }
            }

            if (!calculoFreteFilialEmissora)
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    carga.TabelaFrete = tabelaFrete;
            }
            else
                carga.TabelaFreteFilialEmissora = tabelaFrete;

            if (tabelaFrete != null && (tabelaFrete.UtilizaModeloVeicularVeiculo || tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo))
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = carga.VeiculosVinculados?.FirstOrDefault()?.ModeloVeicularCarga;

                if (modeloVeicularCalculo == null && carga.Veiculo != null)
                    modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;

                if (modeloVeicularCalculo != null)
                    carga.ModeloVeicularCarga = modeloVeicularCalculo;
                else
                    modeloVeicularCalculo = carga.ModeloVeicularCarga;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> tabelaCargaPedidoCargas = repCargaPedidoTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosModalidades = repPedidoXMLNotaFiscal.BuscarModalidadesDeFretePadraoCargaPedidoPorCarga(carga.Codigo);

            abriuTransacao = false;
            if (!_unitOfWork.IsActiveTransaction())
            {
                _unitOfWork.Start();
                abriuTransacao = true;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosAgrupados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            decimal maiorValorBaseFreteDosPedidos = 0m;
            decimal pesoTotalParaCalculoFatorCubagem = svcRateio.ObterPesoTotalCubadoFatorCubagem(pedidoNotasFiscais);

            if (tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido> fretesPedidosPagos = fretesCargaPedido.Where(o => o.cargaPedidos.Any(c => c.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)).ToList();
                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = fretesCargaPedido.OrderByDescending(o => o.dadosCalculoFrete.ValorBase).Select(o => o.dadosCalculoFrete).FirstOrDefault();

                maiorValorBaseFreteDosPedidos = dadosCalculoFrete?.ValorBase ?? 0;

                decimal valorTotalFOB = fretesCargaPedido.Where(o => o.cargaPedidos.Any(c => c.TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente)).Sum(o => o.dadosCalculoFrete.ValorFrete + o.dadosCalculoFrete.ValorTotalComponentes);
                decimal diferenca = maiorValorBaseFreteDosPedidos - valorTotalFOB;

                if (fretesPedidosPagos.Count > 0)
                {
                    if (diferenca < 0)
                        diferenca = 0;

                    decimal valorTotalRateado = 0;
                    Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido ultimoFreteCargaPedido = fretesPedidosPagos.LastOrDefault();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido in fretesPedidosPagos)
                    {
                        decimal valorRateioOriginal = 0m;
                        decimal pesoParaCalculoFatorCubagem = 0m;

                        if (freteCargaPedido.cargaPedidos.Select(o => o.FormulaRateio).FirstOrDefault()?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                            pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(freteCargaPedido.cargaPedidos.FirstOrDefault().FormulaRateio?.ParametroRateioFormula, freteCargaPedido.cargaPedidos.FirstOrDefault().TipoUsoFatorCubagemRateioFormula, freteCargaPedido.cargaPedidos.FirstOrDefault().FatorCubagemRateioFormula ?? 0, freteCargaPedido.cargaPedidos.Sum(o => o.Pedido.QtVolumes), freteCargaPedido.cargaPedidos.Sum(o => o.Peso), repPedidoXMLNotaFiscal.BuscarPorCargaPedido(freteCargaPedido.cargaPedido.Codigo)?.Sum(x => x.XMLNotaFiscal.MetrosCubicos) ?? 0);

                        freteCargaPedido.dadosCalculoFrete.ValorFixo = 0;

                        if ((maiorValorBaseFreteDosPedidos - valorTotalFOB) < 0 && tabelaFrete.ValorMinimoDiferencaFreteNegativo > 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoMin = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                            Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicaoMin, "Diferença negativa (" + freteCargaPedido.dadosCalculoFrete.ValorFrete.ToString("n2") + "), aplicado frete mínimo (" + tabelaFrete.ValorMinimoDiferencaFreteNegativo.ToString("n2") + ")", tabelaFrete.ValorMinimoDiferencaFreteNegativo.ToString("n2"), tabelaFrete.ValorMinimoDiferencaFreteNegativo);
                            freteCargaPedido.dadosCalculoFrete.ValorFrete = tabelaFrete.ValorMinimoDiferencaFreteNegativo;
                            freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.Add(composicaoMin);
                            diferenca += freteCargaPedido.dadosCalculoFrete.ValorFrete;
                        }
                        else
                            freteCargaPedido.dadosCalculoFrete.ValorFrete = svcRateio.AplicarFormulaRateio(freteCargaPedido.cargaPedidos.Select(o => o.FormulaRateio).FirstOrDefault(), diferenca, fretesPedidosPagos.Count(), 1, fretesPedidosPagos.Sum(obj => obj.cargaPedidos.Sum(o => o.Peso)), freteCargaPedido.cargaPedidos.Sum(o => o.Peso), freteCargaPedido.cargaPedidos.Sum(o => o.Pedido.ValorTotalNotasFiscais), fretesPedidosPagos.Sum(obj => obj.cargaPedidos.Sum(c => c.Pedido.ValorTotalNotasFiscais)), 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, freteCargaPedido.cargaPedidos.Sum(o => o.PesoLiquido), fretesPedidosPagos.Sum(obj => obj.cargaPedidos.Sum(c => c.PesoLiquido)), freteCargaPedido.cargaPedidos.Sum(o => o.Pedido.QtVolumes), fretesPedidosPagos.Sum(obj => obj.cargaPedidos.Sum(c => c.Pedido.QtVolumes)), null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        valorTotalRateado += freteCargaPedido.dadosCalculoFrete.ValorFrete;

                        if (freteCargaPedido.ID == ultimoFreteCargaPedido.ID)
                        {
                            decimal diferencao = diferenca - valorTotalRateado;

                            freteCargaPedido.dadosCalculoFrete.ValorFrete = freteCargaPedido.dadosCalculoFrete.ValorFrete + diferencao;

                            if (freteCargaPedido.dadosCalculoFrete.ValorFrete < 0)
                            {
                                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                                retorno.mensagem = mensagemRetorno.Insert(0, "O valor do frete residual não pode ser superior ao valor do frete pois neste caso o CT-e será gerado com valor negativo oque fiscalmente não é possível (" + tabelaFrete.Descricao + ")").ToString();
                                return true;
                            }
                        }

                        freteCargaPedido.dadosCalculoFrete.Componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();
                        freteCargaPedido.dadosCalculoFrete.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();

                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "Diferença do maior valor (" + maiorValorBaseFreteDosPedidos.ToString("n2") + ") base entre as tabelas para fretes CIF menos o total de FOB (" + valorTotalFOB.ToString("n2") + ") rateado por " + (freteCargaPedido.cargaPedidos.FirstOrDefault()?.FormulaRateio?.Descricao ?? "Peso"), diferenca + " / Formula de Rateio = " + freteCargaPedido.dadosCalculoFrete.ValorFrete, freteCargaPedido.dadosCalculoFrete.ValorFrete);

                        if (dadosCalculoFrete?.ComposicaoValorBaseFrete != null && dadosCalculoFrete.ComposicaoValorBaseFrete.Count > 0)
                            freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.AddRange(dadosCalculoFrete.ComposicaoValorBaseFrete);

                        freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.Add(composicao);
                    }
                }
                else
                {
                    if (diferenca != 0m)
                    {
                        string decontoAliquota = "";

                        if (carga.Empresa.AliquotaICMSNegociado > 0m)
                        {
                            diferenca = diferenca * ((100 - carga.Empresa.AliquotaICMSNegociado) / 100);
                            decontoAliquota = " (valor com o desconto da alíquota de ICMS de " + carga.Empresa.AliquotaICMSNegociado.ToString("n2") + ")";
                        }

                        decimal valorTotalRateado = 0m;

                        Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido ultimoFreteCargaPedido = fretesCargaPedido.LastOrDefault();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido in fretesCargaPedido)
                        {
                            freteCargaPedido.dadosCalculoFrete.ValorFixo = 0m;

                            decimal valorRateioOriginal = 0m;
                            decimal pesoParaCalculoFatorCubagem = 0m;

                            if (freteCargaPedido.cargaPedidos.Select(o => o.FormulaRateio).FirstOrDefault()?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(freteCargaPedido.cargaPedidos.FirstOrDefault().FormulaRateio?.ParametroRateioFormula, freteCargaPedido.cargaPedidos.FirstOrDefault().TipoUsoFatorCubagemRateioFormula, freteCargaPedido.cargaPedidos.FirstOrDefault().FatorCubagemRateioFormula ?? 0, freteCargaPedido.cargaPedidos.Sum(o => o.Pedido.QtVolumes), freteCargaPedido.cargaPedidos.Sum(o => o.Peso), repPedidoXMLNotaFiscal.BuscarPorCargaPedido(freteCargaPedido.cargaPedido.Codigo)?.Sum(x => x.XMLNotaFiscal.MetrosCubicos) ?? 0);

                            freteCargaPedido.dadosCalculoFrete.ValorFreteResidual = svcRateio.AplicarFormulaRateio(freteCargaPedido.cargaPedidos.Select(o => o.FormulaRateio).FirstOrDefault(), diferenca, fretesCargaPedido.Count(), 1, fretesCargaPedido.Sum(obj => obj.cargaPedidos.Sum(c => c.Peso)), freteCargaPedido.cargaPedidos.Sum(o => o.Peso), freteCargaPedido.cargaPedidos.Sum(o => o.Pedido.ValorTotalNotasFiscais), fretesCargaPedido.Sum(obj => obj.cargaPedidos.Sum(c => c.Pedido.ValorTotalNotasFiscais)), 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, freteCargaPedido.cargaPedidos.Sum(o => o.PesoLiquido), fretesCargaPedido.Sum(obj => obj.cargaPedidos.Sum(c => c.PesoLiquido)), freteCargaPedido.cargaPedidos.Sum(o => o.Pedido.QtVolumes), fretesCargaPedido.Sum(obj => obj.cargaPedidos.Sum(c => c.Pedido.QtVolumes)), null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                            valorTotalRateado += freteCargaPedido.dadosCalculoFrete.ValorFreteResidual;

                            if (freteCargaPedido.ID == ultimoFreteCargaPedido.ID)
                            {
                                decimal diferencaIteracao = diferenca - valorTotalRateado;
                                freteCargaPedido.dadosCalculoFrete.ValorFreteResidual += diferencaIteracao;
                            }

                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();

                            Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "(Lote Residual) Diferença do maior valor (" + maiorValorBaseFreteDosPedidos.ToString("n2") + ") base entre as tabelas para fretes CIF menos o total de FOB (" + valorTotalFOB.ToString("n2") + ") rateado por " + (freteCargaPedido.cargaPedidos.Select(o => o.FormulaRateio?.Descricao).First() ?? "Peso") + decontoAliquota, diferenca + " / Formula de Rateio = " + freteCargaPedido.dadosCalculoFrete.ValorFrete, freteCargaPedido.dadosCalculoFrete.ValorFrete);

                            if (dadosCalculoFrete?.ComposicaoValorBaseFrete != null && dadosCalculoFrete.ComposicaoValorBaseFrete.Count > 0)
                                freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.AddRange(dadosCalculoFrete.ComposicaoValorBaseFrete);

                            freteCargaPedido.dadosCalculoFrete.ComposicaoFrete.Add(composicao);
                        }
                    }
                }
            }

            if (!apenasVerificar)
                carga.MaiorValorBaseFreteDosPedidos = maiorValorBaseFreteDosPedidos;

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.FreteCargaPedido freteCargaPedido in fretesCargaPedido)
            {
                SetarTabelaFreteCargaPedidos(carga, cargasOrigem, freteCargaPedido.cargaPedidos, freteCargaPedido.parametrosCalculo, freteCargaPedido.dadosCalculoFrete, freteCargaPedido.tabelaFreteCliente, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, ref componentesPorCarga, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, tabelaCargaPedidoCargas, cargaPedidosModalidades, configuracaoGeralCarga);

                if (freteCargaPedido.cargaPedidos.Any(o => o.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado || o.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado) || freteCargaPedido.cargaPedidos.Any(o => o.IndicadorCTeGlobalizadoDestinatario))
                    pedidosAgrupados.AddRange(freteCargaPedido.cargaPedidos);
            }

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValorPedagioIntegracao = repCargaConsultaValorPedagioIntegracao.ConsultaIntegracaoPorCarga(carga.Codigo, SituacaoIntegracao.Integrado);
            if (cargaConsultaValorPedagioIntegracao != null && cargaConsultaValorPedagioIntegracao.ValorValePedagio > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(carga.Codigo);
                bool removeuComponentesAnteriormente = false;
                if (cargaValePedagios != null && cargaValePedagios.Count > 0)
                    removeuComponentesAnteriormente = cargaValePedagios.Any(x => x.ValidaCompraRemoveuComponentes == true);//caso ja tenha removido componentes de vale pedagio, nao será adicionado novamente.

                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    serCargaPedido.ValidarValorPedagioPorCargaPedido(cargaPedido, calculoFreteFilialEmissora, _unitOfWork, _tipoServicoMultisoftware, componenteFretePedagio, cargaPedidosComponentesFreteCarga, cargaComponentesFretesDiretos, removeuComponentesAnteriormente, tabelaFrete);
                }
            }


            if (abriuTransacao)
                _unitOfWork.CommitChanges();

            if (pedidosAgrupados.Count > 0)
            {
                abriuTransacao = false;
                if (!_unitOfWork.IsActiveTransaction())
                {
                    _unitOfWork.Start();
                    abriuTransacao = true;
                }

                svcRateioFrete.CalcularImpostosAgrupados(ref carga, cargasOrigem, pedidosAgrupados, calculoFreteFilialEmissora, _tipoServicoMultisoftware, _unitOfWork, _configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosValoresNotas, configuracaoTabelaFrete, configuracaoGeralCarga);

                if (abriuTransacao)
                    _unitOfWork.CommitChanges();
            }

            svcCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, calculoFreteFilialEmissora, cargaPedidosComponentesFreteCarga, _unitOfWork);
            svcRateioFrete.AcrescentarValoresDaCargaAgrupada(carga, calculoFreteFilialEmissora, cargaPedidos, _unitOfWork);

            if (!apenasVerificar)
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    carga.PossuiPendencia = false;

                    svcRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, calculoFreteFilialEmissora, _tipoServicoMultisoftware, _unitOfWork, _configuracao);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(carga.Codigo, calculoFreteFilialEmissora);
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteISS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePisCONFIS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);

                    abriuTransacao = false;
                    if (!_unitOfWork.IsActiveTransaction())
                    {
                        _unitOfWork.Start();
                        abriuTransacao = true;
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        svcRateioFrete.GerarComponenteICMS(cargaPedido, calculoFreteFilialEmissora, componenteFreteICMS, cargaPedidosComponentesFreteCargaImpostos, _unitOfWork);
                        if (!calculoFreteFilialEmissora)
                            svcRateioFrete.GerarComponenteISS(cargaPedido, componenteFreteISS, cargaPedidosComponentesFreteCargaImpostos, false, _unitOfWork);

                        if (!calculoFreteFilialEmissora)
                            svcRateioFrete.GerarComponentePisCofins(cargaPedido, componenteFretePisCONFIS, cargaPedidosComponentesFreteCargaImpostos, _unitOfWork);
                    }

                    if (abriuTransacao)
                        _unitOfWork.CommitChanges();

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);

                    bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao() && (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false);

                    if (!carga.ExigeNotaFiscalParaCalcularFrete && !existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    else
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.MotivoPendencia = "";

                    if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 5 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                    svcFreteCliente.CriarTabelaFreteCliente(carga, cargaPedidos, apenasVerificar, calculoFreteFilialEmissora, _tipoServicoMultisoftware);

                    svcFrete.CriarCargaComponentes(carga, _tipoServicoMultisoftware, cargaPedidosComponentesFreteCarga, cargaPedidos.FirstOrDefault().ObterTomador(), calculoFreteFilialEmissora, _unitOfWork);

                    svcRateioFrete.GerarComponenteICMS(carga, cargaPedidos, false, calculoFreteFilialEmissora, _unitOfWork);
                    if (!calculoFreteFilialEmissora)
                        svcRateioFrete.GerarComponenteISS(carga, cargaPedidos, _unitOfWork);

                    if (!calculoFreteFilialEmissora)
                        svcRateioFrete.GerarComponentePisCofins(carga, cargaPedidos, false, _unitOfWork);

                    retorno = svcFreteCliente.ObterDadosTabelaFreteClientePorPedido(carga, calculoFreteFilialEmissora, _unitOfWork, _tipoServicoMultisoftware);

                    svcCargaAprovacaoFrete.CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.TabelaFrete, _tipoServicoMultisoftware);
                }
                else
                {
                    if (!calculoFreteFilialEmissora)
                        carga.ValorFreteTabelaFrete = cargaPedidos.Sum(o => o.ValorFreteTabelaFrete);
                    else
                        carga.ValorFreteTabelaFreteFilialEmissora = cargaPedidos.Sum(o => o.ValorFreteTabelaFreteFilialEmissora);

                    retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete()
                    {
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido
                    };
                }

                repCarga.Atualizar(carga);
            }

            return false;
        }

        #endregion

        #region Métodos Privados

        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, int distanciaPercurso, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesCarga, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNotas, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresCTesSubcontratacao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> cargaPedidoRotasFrete, bool calculoFilialEmissora, decimal pesoCalculo, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = null, int distanciaKmPedido = 0, int distanciaPorCarga = 0)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = cargaPedidos[0];

            if (carga.TipoOperacao?.UtilizarTipoCargaPedidoCalculoFrete ?? false)
                primeiroCargaPedido = cargaPedidos.OrderBy(obj => obj.Pedido.TipoDeCarga.PrioridadeCarga).FirstOrDefault();

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(_unitOfWork);

            int distancia = (int)cargaPedidos.Sum(o => o.Pedido.Distancia);

            if (distancia <= 0)
            {
                if (carga.Rota != null && carga.Rota.Quilometros > 0)
                    distancia = (int)carga.Rota.Quilometros;
                else
                    distancia = distanciaPercurso;

                if (carregamentoRoteirizacao != null && (_configuracao?.UtilizarDistanciaRoteirizacaoCarregamentoNaCarga ?? true) && carregamentoRoteirizacao.DistanciaKM > 0)
                    distancia = (int)carregamentoRoteirizacao.DistanciaKM;

                if (carga.DeslocamentoQuilometros > 0)
                    distancia += (int)carga.DeslocamentoQuilometros;

                if (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Concluido)
                {
                    if (tabelaFrete.UsarCalculoFretePorPedido)
                    {
                        if (primeiroCargaPedido.Recebedor == null && distanciaKmPedido > 0)
                            distancia = distanciaKmPedido;
                    }
                    else if (configuracaoCarga?.UtilizarDistanciaRoteirizacaoNaCarga ?? false)
                        distancia = distanciaPorCarga;
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = cargaPedidoQuantidadesCarga.Where(o => cargaPedidos.Any(cp => cp.Codigo == o.CargaPedido.Codigo)).ToList();

            decimal quantidadePallets = cargaPedidos.Sum(o => o.Pallet);
            if (quantidadePallets == 0)
                quantidadePallets = cargaPedidos.Sum(o => o.Pedido.NumeroPaletes + o.Pedido.NumeroPaletesFracionado);

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                decimal quantidadePalletsNotas = cargaPedidosValoresNotas.Where(o => cargaPedidos.Any(cp => cp.Codigo == o.Codigo)).Sum(o => o.QuantidadePallets);

                if (quantidadePalletsNotas > 0m)
                    quantidadePallets = quantidadePalletsNotas;
            }

            decimal peso = pesoCalculo;
            decimal pesoLiquido = cargaPedidos.Sum(X => X.PesoLiquido);

            if (peso <= 0)
            {
                if (tabelaFrete.UtilizarPesoLiquido)
                    peso = cargaPedidos.Sum(o => o.PesoLiquido);
                else
                {
                    peso = svcFreteCliente.ObterQuilosTotaisParaQuilos(cargaPedidoQuantidades);

                    if (peso <= 0m)
                        peso = cargaPedidos.Sum(o => o.Peso);
                }
            }

            peso -= cargaPedidos.Sum(o => o.PesoMercadoriaDescontar);

            int quantidadeNotasFiscais = cargaPedidosValoresNotas.Where(o => cargaPedidos.Any(cp => cp.Codigo == o.Codigo) && o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao).Sum(o => o.TotalNotasFiscais);
            decimal valorTotalNotasFiscais = 0m;

            if (!(carga.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false))
                valorTotalNotasFiscais = cargaPedidos.Sum(o => o.Pedido.ValorTotalNotasFiscais);
            else if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                primeiroCargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal ||
                primeiroCargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                valorTotalNotasFiscais = cargaPedidosValoresNotas.Where(o => cargaPedidos.Any(cp => cp.Codigo == o.Codigo)).Sum(o => o.ValorTotalNotaFiscal);
            else if (primeiroCargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                valorTotalNotasFiscais = cargaPedidosValoresCTesSubcontratacao.Where(o => cargaPedidos.Any(cp => cp.Codigo == o.Codigo)).Sum(o => o.ValorTotalNotaFiscal);

            valorTotalNotasFiscais -= cargaPedidos.Sum(o => o.ValorMercadoriaDescontar);

            bool possuiComponentePorQuantidadeDocumentos = tabelaFrete.Componentes.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado &&
                                                                                            o.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos &&
                                                                                            o.TipoDocumentoQuantidadeDocumentos.HasValue &&
                                                                                            o.TipoDocumentoQuantidadeDocumentos.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido);

            Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidadesDocumentosEmitir = null;

            if (possuiComponentePorQuantidadeDocumentos)
            {
                Servicos.Embarcador.Carga.CargaPedido.CriarPreviaDocumentoCarga(carga, _unitOfWork, _tipoServicoMultisoftware, _configuracao);

                quantidadesDocumentosEmitir = Servicos.Embarcador.Carga.CargaPedido.ObterQuantidadeDocumentosEmitir(null, cargaPedidos, null, null, _unitOfWork, _tipoServicoMultisoftware);
            }

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            if (carga.OrigemTrocaNota != null)
                origens.Add(carga.OrigemTrocaNota);

            if (tabelaFrete.UtilizarParticipantePedidoParaCalculo)
            {
                if (carga.OrigemTrocaNota == null && primeiroCargaPedido.Pedido.Origem != null)
                    origens.Add(primeiroCargaPedido.Pedido.Origem);

                if (primeiroCargaPedido.Pedido.Destino != null)
                    destinos.Add(primeiroCargaPedido.Pedido.Destino);
            }
            else
            {
                if (carga.OrigemTrocaNota == null && primeiroCargaPedido.Origem != null)
                    origens.Add(primeiroCargaPedido.Origem);

                if (primeiroCargaPedido.Destino != null)
                    destinos.Add(primeiroCargaPedido.Destino);
            }

            if (primeiroCargaPedido.Pedido.RegiaoDestino != null)
                regioesDestino.Add(primeiroCargaPedido.Pedido.RegiaoDestino);

            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();

            List<int> cepsRemetentes = new List<int>();
            List<int> cepsDestinatarios = new List<int>();

            if ((primeiroCargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || primeiroCargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) &&
                primeiroCargaPedido.Recebedor != null && !(carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false) && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
            {
                destinatarios.Add(primeiroCargaPedido.Recebedor);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroCargaPedido.Recebedor.CEP), out int cepDestinatario);
                cepsDestinatarios.Add(cepDestinatario);
            }
            else if (primeiroCargaPedido.Pedido.Destinatario != null)
            {
                destinatarios.Add(primeiroCargaPedido.Pedido.Destinatario);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroCargaPedido.Pedido.EnderecoDestino?.CEP ?? primeiroCargaPedido.Pedido.Destinatario.CEP), out int cepDestinatario);
                cepsDestinatarios.Add(cepDestinatario);
            }

            if (primeiroCargaPedido.Pedido.ClienteDeslocamento != null)
            {
                remetentes.Add(primeiroCargaPedido.Pedido.ClienteDeslocamento);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroCargaPedido.Pedido.ClienteDeslocamento.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }
            else if ((primeiroCargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || primeiroCargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) &&
                     primeiroCargaPedido.Expedidor != null && !tabelaFrete.NaoConsiderarExpedidorERecebedor)
            {
                remetentes.Add(primeiroCargaPedido.Expedidor);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroCargaPedido.Expedidor.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }
            else if (primeiroCargaPedido.Pedido.Remetente != null)
            {
                remetentes.Add(primeiroCargaPedido.Pedido.Remetente);

                int.TryParse(Utilidades.String.OnlyNumbers(primeiroCargaPedido.Pedido.EnderecoOrigem?.CEP ?? primeiroCargaPedido.Pedido.Remetente.CEP), out int cepRemetente);
                cepsRemetentes.Add(cepRemetente);
            }

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = carga.ModeloVeicularCarga;
            if (tabelaFrete.UtilizaModeloVeicularVeiculo)
            {
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                    modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
            }

            if (modeloVeicularCalculo != null)
            {
                if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeMinimaGarantidaModeloVeicular && modeloVeicularCalculo.ToleranciaPesoMenor > 0)
                {
                    if (peso < modeloVeicularCalculo.ToleranciaPesoMenor)
                        peso = modeloVeicularCalculo.ToleranciaPesoMenor;
                }
                else if (tabelaFrete.PesoParametroCalculoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete.CapacidadeModeloVeicular && modeloVeicularCalculo.CapacidadePesoTransporte > 0)
                    peso = modeloVeicularCalculo.CapacidadePesoTransporte;
            }

            decimal pesoCubado = 0m;
            decimal isencaoCubagem = 0m;
            bool calcularFretePorPesoCubado = tabelaFrete.CalcularFretePorPesoCubado;
            bool aplicarMaiorValorEntrePesoEPesoCubado = tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado;

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                pesoCubado = cargaPedidosValoresNotas.Where(o => cargaPedidos.Any(cp => cp.Codigo == o.Codigo)).Sum(o => o.Cubagem);

            if (calcularFretePorPesoCubado)
            {
                decimal cubagemPedidos = cargaPedidos.Sum(o => o.Pedido.CubagemTotal);
                if (tabelaFrete.FatorCubagem > 0 && cubagemPedidos > 0)
                {
                    pesoCubado = cubagemPedidos * tabelaFrete.FatorCubagem;
                    isencaoCubagem = tabelaFrete.IsencaoCubagem;
                }
            }
            else if (pesoCubado <= 0m)
                pesoCubado = cargaPedidos.Sum(o => o.Pedido.PesoCubado);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem> paramtrosTipoEmbalagem = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem>();
            if (tabelaFrete.TipoEmbalagens?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
                // todo: se ficar lento aqui cenário para poucos produtos, tratar para passar por parametro a lista de produtos da carga e filtrar aqui por cargapedido (dúvidas falar com o Rodrigo).
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargasPedidos((from obj in cargaPedidos select obj.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                {
                    if (cargaPedidoProduto.Produto?.TipoEmbalagem != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem parametroTipoEmbalagem = (from obj in paramtrosTipoEmbalagem where obj.TipoEmbalagem.Codigo == cargaPedidoProduto.Produto.TipoEmbalagem.Codigo select obj).FirstOrDefault();
                        if (parametroTipoEmbalagem == null)
                        {
                            parametroTipoEmbalagem = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem();
                            parametroTipoEmbalagem.TipoEmbalagem = cargaPedidoProduto.Produto.TipoEmbalagem;
                            parametroTipoEmbalagem.Quantidade = cargaPedidoProduto.Quantidade;
                            parametroTipoEmbalagem.Peso = cargaPedidoProduto.PesoTotal;
                            paramtrosTipoEmbalagem.Add(parametroTipoEmbalagem);
                        }
                        else
                        {
                            parametroTipoEmbalagem.Quantidade += cargaPedidoProduto.Quantidade;
                            parametroTipoEmbalagem.Peso += cargaPedidoProduto.PesoTotal;
                        }

                    }
                }
            }


            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete()
            {
                DataColeta = primeiroCargaPedido.Pedido.DataInicialColeta,
                DataFinalViagem = primeiroCargaPedido.Pedido.DataFinalViagemFaturada,
                DataInicialViagem = primeiroCargaPedido.Pedido.DataInicialViagemFaturada,
                DataVigencia = (tabelaFrete.ValidarPorDataCarregamento && carga.DataCarregamentoCarga.HasValue) ? carga.DataCarregamentoCarga.Value : carga.DataCriacaoCarga.Date,
                Desistencia = carga.Desistencia,
                DespachoTransitoAduaneiro = primeiroCargaPedido.Pedido.DespachoTransitoAduaneiro,
                NecessarioAjudante = primeiroCargaPedido.Pedido.Ajudante,
                Destinatarios = destinatarios,
                Destinos = destinos,
                PesoTotalCarga = carga?.DadosSumarizados?.PesoTotal ?? 0m,
                RegioesDestino = regioesDestino,
                Distancia = distancia,
                Empresa = !calculoFilialEmissora ? primeiroCargaPedido.CargaOrigem.Empresa : primeiroCargaPedido.CargaOrigem.EmpresaFilialEmissora,
                EscoltaArmada = primeiroCargaPedido.Pedido.EscoltaArmada,
                QuantidadeEscolta = primeiroCargaPedido.Pedido.QtdEscolta,
                Filial = carga.Filial,
                GerenciamentoRisco = primeiroCargaPedido.Pedido.GerenciamentoRisco,
                GrupoPessoas = carga.GrupoPessoaPrincipal,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>()
                {
                    primeiroCargaPedido.ModeloDocumentoFiscal
                },
                ModeloVeiculo = modeloVeicularCalculo,
                NecessarioReentrega = primeiroCargaPedido.Pedido.NecessarioReentrega,
                NumeroAjudantes = primeiroCargaPedido.Pedido.QtdAjudantes,
                NumeroDeslocamento = primeiroCargaPedido.Pedido.ValorDeslocamento ?? 0m,
                NumeroDiarias = primeiroCargaPedido.Pedido.ValorDiaria ?? 0m,
                NumeroEntregas = primeiroCargaPedido.Pedido.QtdEntregas,
                NumeroPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork).BuscarQuantidadePacoteCarga(carga.Codigo),
                NumeroPallets = quantidadePallets,
                Origens = origens,
                PercentualDesistencia = carga.PercentualDesistencia,
                Peso = peso,
                PesoLiquido = pesoLiquido,
                PesoCubado = pesoCubado,
                CalcularFretePorPesoCubado = calcularFretePorPesoCubado,
                AplicarMaiorValorEntrePesoEPesoCubado = aplicarMaiorValorEntrePesoEPesoCubado,
                IsencaoCubagem = isencaoCubagem,
                Cubagem = cargaPedidos.Sum(cp => cp.Pedido.CubagemTotal),
                MaiorAlturaProdutoEmCentimetros = cargaPedidos.Max(cp => cp.Pedido.MaiorAlturaProdutoEmCentimetros),
                MaiorLarguraProdutoEmCentimetros = cargaPedidos.Max(cp => cp.Pedido.MaiorLarguraProdutoEmCentimetros),
                MaiorComprimentoProdutoEmCentimetros = cargaPedidos.Max(cp => cp.Pedido.MaiorComprimentoProdutoEmCentimetros),
                MaiorVolumeProdutoEmCentimetros = cargaPedidos.Max(cp => cp.Pedido.MaiorVolumeProdutoEmCentimetros),
                PesoPaletizado = cargaPedidos.Sum(o => o.Pedido.PesoTotalPaletes),
                PossuiRestricaoTrafego = (primeiroCargaPedido.Pedido.Remetente != null && primeiroCargaPedido.Pedido.Remetente.PossuiRestricaoTrafego) ||
                                         (primeiroCargaPedido.Pedido.Destinatario != null && primeiroCargaPedido.Pedido.Destinatario.PossuiRestricaoTrafego),
                QuantidadeEmissoesPorModeloDocumento = quantidadesDocumentosEmitir,
                QuantidadeNotasFiscais = quantidadeNotasFiscais,
                Quantidades = cargaPedidoQuantidades.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade()
                {
                    Quantidade = o.Quantidade,
                    UnidadeMedida = o.Unidade
                }).ToList(),
                Rastreado = primeiroCargaPedido.Pedido.Rastreado,
                Remetentes = remetentes,
                Rota = (primeiroCargaPedido.Pedido.RotaFrete != null && _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ? primeiroCargaPedido.Pedido.RotaFrete : carga.Rota,
                RotasDinamicas = primeiroCargaPedido.Pedido.RotaFrete != null ? new List<Dominio.Entidades.RotaFrete>() { primeiroCargaPedido.Pedido.RotaFrete } : null,
                CodigosRotasFixas = (from obj in cargaPedidoRotasFrete select obj.Codigo).ToList(),
                TipoCarga = carga.TipoOperacao?.UtilizarTipoCargaPedidoCalculoFrete ?? false ? primeiroCargaPedido.Pedido.TipoDeCarga : carga.TipoDeCarga,
                TipoOperacao = carga.TipoOperacao,
                Tomador = primeiroCargaPedido.ObterTomador(),
                ValorNotasFiscais = valorTotalNotasFiscais,
                Veiculo = carga.Veiculo,
                Reboques = carga.VeiculosVinculados?.ToList(),
                Volumes = cargaPedidoQuantidades.Where(o => o.Unidade == Dominio.Enumeradores.UnidadeMedida.UN).Sum(o => o.Quantidade),
                DataBaseCRT = primeiroCargaPedido.Pedido.DataBaseCRT,
                CEPsRemetentes = cepsRemetentes.Distinct().ToList(),
                CEPsDestinatarios = cepsDestinatarios.Distinct().ToList(),
                ParametrosCarga = parametrosCalculoCarga,
                Fronteiras = carga.Rota?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
                FreteTerceiro = carga.FreteDeTerceiro,
                TiposEmbalagem = paramtrosTipoEmbalagem,
                CanalEntrega = primeiroCargaPedido.CanalEntrega != null ? primeiroCargaPedido.CanalEntrega : primeiroCargaPedido.Pedido.CanalEntrega,
                CanalVenda = primeiroCargaPedido.CanalVenda != null ? primeiroCargaPedido.CanalVenda : primeiroCargaPedido.Pedido.CanalVenda,
                DataPrevisaoEntrega = primeiroCargaPedido.Pedido?.PrevisaoEntrega ?? null,
                CargaPerigosa = carga?.CargaPerigosaIntegracaoLeilao ?? false,
            };

            return parametrosCalculoFrete;
        }

        private int ObterDistanciaPorPedidoAgrupadoComCache(Dictionary<double, int> cacheDistancias, int codigoCarga, double cpfCnpj, Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem)
        {
            if (!cacheDistancias.TryGetValue(cpfCnpj, out int distanciaKmPedido))
            {
                distanciaKmPedido = repositorioCargaRotaFretePontosPassagem.BuscarDistanciaPorCargaPedido(codigoCarga, cpfCnpj) / 1000;
                cacheDistancias[cpfCnpj] = distanciaKmPedido;
            }

            return distanciaKmPedido;
        }

        private void SetarTabelaFreteCargaPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool apenasVerificar, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, ref List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesPorCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> tabelaCargaPedidoCargas, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosModalidades, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            if (apenasVerificar)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosValoresNota = repPedidoXMLNotaFiscal.BuscarTotalSumarizadoPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula svcRateioFormula = new Servicos.Embarcador.Carga.RateioFormula();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = cargaPedidos.Last();
            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = cargaPedidos.First().FormulaRateio;
            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = carga.ContratoFreteTransportador;

            Dominio.Entidades.Cliente remetente = cargaPedidos.Select(o => o.Pedido.Remetente).FirstOrDefault();

            List<int> codigosCargaPedidos = cargaPedidos.Select(o => o.Codigo).ToList();

            decimal pesoLiquidoTotal = 0m, pesoTotal = 0m, valorTotalNF = 0m, metrosCubicosTotais = 0m;
            decimal valorContratoFrete = carga?.ValorFreteContratoFrete ?? 0m;

            int volumeTotal = 0;
            bool possuiComponenteFrete = (contratoFreteTransportador?.ComponenteFreteValorContrato ?? null) != null ? true : false;

            if (_configuracao.NaoAtualizarPesoPedidoPelaNFe)
                volumeTotal = cargaPedidos.Sum(obj => obj.Pedido.QtVolumes);
            else
                volumeTotal = cargaPedidos.Sum(obj => obj.QtVolumes);

            pesoLiquidoTotal = cargaPedidos.Sum(obj => obj.PesoLiquido);
            pesoTotal = cargaPedidos.Sum(obj => obj.Peso);

            if (carga.ExigeNotaFiscalParaCalcularFrete)
                valorTotalNF = repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedidos(codigosCargaPedidos);
            else
                valorTotalNF = cargaPedidos.Sum(o => o.Pedido.ValorTotalNotasFiscais);

            metrosCubicosTotais = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCargaPedidos(codigosCargaPedidos);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> dadosRateados = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete>();

            if (!calculoFreteFilialEmissora)
            {
                bool utilizaContaRazao = carga.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false;
                if (!utilizaContaRazao)
                {
                    repCargaPedidoContaContabilContabilizacao.DeletarPorCarga(carga.Codigo);
                    carga.PossuiPendenciaConfiguracaoContabil = false;
                }
            }
            decimal pesoTotalParaCalculoFatorCubagem = 0m;

            if (formulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                pesoTotalParaCalculoFatorCubagem = svcRateioFormula.ObterPesoTotalCubadoFatorCubagem(repPedidoXMLNotaFiscal.BuscarPorCargaPedidos(codigosCargaPedidos));

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                decimal pesoLiquido = 0m, peso = 0m, valorNF = 0m, metrosCubicos = 0m;
                int volume = 0;
                decimal valorDoContratoRateado = 0m;
                decimal pesoParaCalculoFatorCubagem = 0m;

                if (_configuracao.NaoAtualizarPesoPedidoPelaNFe)
                    volume = cargaPedido.Pedido.QtVolumes;
                else
                    volume = cargaPedido.QtVolumes;

                if (carga.ExigeNotaFiscalParaCalcularFrete)
                    valorNF = (from obj in cargaPedidosValoresNota where obj.Codigo == cargaPedido.Codigo select obj.ValorTotalNotaFiscal).Sum(); //repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedido(cargaPedido.Codigo);
                else
                    valorNF = cargaPedido.Pedido.ValorTotalNotasFiscais;

                pesoLiquido = cargaPedido.PesoLiquido;
                peso = cargaPedido.Peso;
                valorDoContratoRateado = pesoTotal > 0 ? (peso / pesoTotal) * valorContratoFrete : 0;

                metrosCubicos = (from obj in cargaPedidosValoresNota where obj.Codigo == cargaPedido.Codigo select obj.MetrosCubicos).Sum(); //repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCargaPedido(cargaPedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosPedido = dados.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(dadosPedido);

                dadosPedido.CodigoCargaPedido = cargaPedido.Codigo;
                dadosPedido.Componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();
                dadosPedido.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

                if (formulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo, true).FirstOrDefault();
                    pesoParaCalculoFatorCubagem = svcRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio?.ParametroRateioFormula, pedidoXMLNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXMLNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, volume, peso, metrosCubicos);
                }

                if (ultimaCargaPedido == cargaPedido)
                {
                    decimal valorFrete = possuiComponenteFrete
                                            ? dadosRateados.Sum(o => o.ValorFrete)
                                            : (dadosRateados.Sum(o => o.ValorFrete) + valorDoContratoRateado);

                    dadosPedido.ValorBase = dados.ValorBase - dadosRateados.Sum(o => o.ValorBase);
                    dadosPedido.ValorFixo = dados.ValorFixo - dadosRateados.Sum(o => o.ValorFixo);
                    dadosPedido.ValorFrete = dados.ValorFrete - valorFrete;
                    dadosPedido.ValorFreteMoeda = dados.ValorFreteMoeda - dadosRateados.Sum(o => o.ValorFreteMoeda);
                    dadosPedido.ValorFreteResidual = dados.ValorFreteResidual - dadosRateados.Sum(o => o.ValorFreteResidual);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dados.Componentes)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componentePedido = componente.Clonar();

                        componentePedido.ValorComponente = componente.ValorComponente - dadosRateados.Sum(o => o.Componentes.Where(c => c.ID == componente.ID).Sum(c => c.ValorComponente));
                        componentePedido.ValorComponenteMoeda = componente.ValorComponenteMoeda - dadosRateados.Sum(o => o.Componentes.Where(c => c.ID == componente.ID).Sum(c => c.ValorComponenteMoeda));
                        componentePedido.ValorComponenteParaCarga = componente.ValorComponenteParaCarga - dadosRateados.Sum(o => o.Componentes.Where(c => c.ID == componente.ID).Sum(c => c.ValorComponenteParaCarga));

                        dadosPedido.Componentes.Add(componentePedido);
                    }

                    if (possuiComponenteFrete)
                    {
                        dados.ComposicaoFrete.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete(TipoParametroBaseTabelaFrete.Peso)
                        {
                            CodigoComponente = contratoFreteTransportador?.ComponenteFreteValorContrato?.Codigo ?? 0,
                            DescricaoComponente = contratoFreteTransportador?.ComponenteFreteValorContrato.Descricao,
                            Formula = $"{(peso / pesoTotal).ToString("n2")} * {valorContratoFrete}",
                            ValorCalculado = valorDoContratoRateado,
                            TipoParametro = TipoParametroBaseTabelaFrete.Peso,
                            Valor = (peso / pesoTotal)
                        });
                    }
                }
                else
                {
                    decimal valorRateioOriginal = 0m;

                    dadosPedido.ValorBase = svcRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorBase, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    dadosPedido.ValorFixo = svcRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorFixo, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    dadosPedido.ValorFrete = svcRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorFrete, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    dadosPedido.ValorFreteMoeda = svcRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorFreteMoeda, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    dadosPedido.ValorFreteResidual = svcRateioFormula.AplicarFormulaRateio(formulaRateio, dados.ValorFreteResidual, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dados.Componentes)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componentePedido = componente.Clonar();

                        componentePedido.ValorComponente = svcRateioFormula.AplicarFormulaRateio(formulaRateio, componente.ValorComponente, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, componente.Percentual, componente.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        componentePedido.ValorComponenteMoeda = svcRateioFormula.AplicarFormulaRateio(formulaRateio, componente.ValorComponenteMoeda, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, componente.Percentual, componente.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        componentePedido.ValorComponenteParaCarga = svcRateioFormula.AplicarFormulaRateio(formulaRateio, componente.ValorComponenteParaCarga, cargaPedidos.Count, 1, pesoTotal, peso, valorNF, valorTotalNF, componente.Percentual, componente.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, metrosCubicos, metrosCubicosTotais, 0m, false, pesoLiquido, pesoLiquidoTotal, volume, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        dadosPedido.Componentes.Add(componentePedido);
                    }

                    if (possuiComponenteFrete)
                    {
                        dados.ComposicaoFrete.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete(TipoParametroBaseTabelaFrete.Peso)
                        {
                            CodigoComponente = contratoFreteTransportador?.ComponenteFreteValorContrato?.Codigo ?? 0,
                            DescricaoComponente = contratoFreteTransportador?.ComponenteFreteValorContrato.Descricao,
                            Formula = $"{(peso / pesoTotal).ToString("n2")} * {valorContratoFrete}",
                            ValorCalculado = valorDoContratoRateado,
                            TipoParametro = TipoParametroBaseTabelaFrete.Peso,
                            Valor = (peso / pesoTotal)
                        });
                    }
                }

                dadosRateados.Add(dadosPedido);

                svcFreteCliente.SetarTabelaFreteCargaPedido(carga, cargasOrigem, cargaPedido, parametros, dadosPedido, tabelaFreteCliente, apenasVerificar, _tipoServicoMultisoftware, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora, ref componentesPorCarga, false, _configuracao, cargaPedidoComponentesFretes, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, tabelaCargaPedidoCargas, cargaPedidosModalidades, configuracaoGeralCarga, false);
            }

            if (carga.ValorBaseFrete < cargaPedidos.Sum(obj => obj.ValorBaseFrete))
                carga.ValorBaseFrete = cargaPedidos.Sum(obj => obj.ValorBaseFrete);

            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, cargaPedidos, null, null, calculoFreteFilialEmissora, dados.ComposicaoFrete, _unitOfWork, null);
        }

        #endregion
    }
}
