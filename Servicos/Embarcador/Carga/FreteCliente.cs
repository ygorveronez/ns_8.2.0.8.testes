using Dominio.Entidades.Embarcador.Frete;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class FreteCliente
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete _configuracaoTabelaFrete;
        private readonly CancellationToken _cancellationToken;

        #endregion Atributos

        #region Construtores

        public FreteCliente(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public FreteCliente(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;

        }

        #endregion Construtores

        #region Métodos Públicos

        /// <summary>
        /// Retorna o valor fixo se existir na tabela de frete do cliente, se ouver necessidade de retornar valores fixos separados para mais de um contratante na mesma carga rever, aqui retorna a soma de todos
        /// </summary>
        /// <param name="carga"></param>
        /// <param name="cargaPedidos"></param>
        /// <param name="_unitOfWork"></param>
        /// <returns></returns>
        public decimal ObterValorFixoSubContratacaoParcial(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao repTabelaFreteClienteSubContratacao = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao(_unitOfWork);

            decimal valorFixoSubContratacaoParcial = 0;

            List<Dominio.Entidades.Cliente> subContratantes = (from obj in cargaPedidos where obj.Pedido.SubContratante != null select obj.Pedido.SubContratante).Distinct().ToList();

            if (subContratantes.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, false);
                foreach (Dominio.Entidades.Cliente subcontratante in subContratantes)
                {
                    if (cargaTabelaFreteCliente != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao tabelaFreteClienteSubContratacao = repTabelaFreteClienteSubContratacao.BuscarPorTabelaETerceiro(cargaTabelaFreteCliente.TabelaFreteCliente.Codigo, subcontratante.CPF_CNPJ);
                        if (tabelaFreteClienteSubContratacao != null && tabelaFreteClienteSubContratacao.ValorFixoSubContratacaoParcial > 0)
                            valorFixoSubContratacaoParcial += tabelaFreteClienteSubContratacao.ValorFixoSubContratacaoParcial;
                    }
                }
            }
            return valorFixoSubContratacaoParcial;
        }

        public void ValidarSeUsaTabelaDestinatario(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.Frete svcFrete = new Frete(this._unitOfWork, tipoServicoMultisoftware);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            StringBuilder mensagem = new StringBuilder();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = svcFrete.ObterTabelasFrete(cargaPedido.Carga, _unitOfWork, tipoServicoMultisoftware, configuracao, ref mensagem, false, cargaPedido.Pedido.Destinatario);

            if (tabelasFrete != null && tabelasFrete.Count > 0)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = svcFrete.ObterParametrosCalculoFretePorCarga(tabelasFrete[0], cargaPedido.Carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, false, _unitOfWork, this._unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao);
                if (parametrosCalculo == null)
                    return;

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = ObterTabelasFrete(ref mensagem, parametrosCalculo, tabelasFrete[0], tipoServicoMultisoftware);

                if (tabelasCliente.Count == 1)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelasCliente[0];
                    if (!cargaPedido.Pedido.UsarTipoTomadorPedido && !cargaPedido.Carga.DadosPagamentoInformadosManualmente && tabelaFreteCliente.TipoPagamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao.UsarDaNotaFiscal && !cargaPedido.Carga.EmitirCTeComplementar)
                    {
                        cargaPedido.RegraTomador = null;
                        if (tabelaFreteCliente.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao.Pago)
                        {
                            cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                            cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                            cargaPedido.Tomador = null;
                        }
                        else if (tabelaFreteCliente.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao.A_Pagar)
                        {
                            cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                            cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                            cargaPedido.Tomador = null;
                        }
                        else
                        {
                            cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                            cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                            cargaPedido.Tomador = tabelaFreteCliente.Tomador;
                        }

                        repPedido.Atualizar(cargaPedido.Pedido);
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }
            }
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> ObterTabelasFrete(ref StringBuilder mensagemRetorno, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            bool possuiOrigem = (parametros.Remetentes?.Count > 0) || (parametros.RementesEDestinatariosOpcionaisQuandoExistirLocalidade && (parametros.Origens?.Count > 0));
            bool possuiDestino = (parametros.Destinatarios?.Count > 0) || (parametros.RementesEDestinatariosOpcionaisQuandoExistirLocalidade && (parametros.Destinos?.Count > 0));

            if (!possuiOrigem || !possuiDestino)
            {
                mensagemRetorno.Append($"Origens ou destinos inválidos para o cálculo.");
                return new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            }

            List<Dominio.Entidades.Cliente> remetentes = parametros.Remetentes ?? new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = parametros.Destinatarios ?? new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Localidade> origens = parametros.Origens ?? new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = parametros.Destinos ?? new List<Dominio.Entidades.Localidade>();

            List<double> cpfCnpjRemetentes = remetentes.Where(o => o != null).Select(o => o.CPF_CNPJ).Distinct().ToList();
            List<double> cpfCnpjDestinatarios = destinatarios.Where(o => o != null).Select(o => o.CPF_CNPJ).Distinct().ToList();

            List<int> cepsRemetentes = parametros.CEPsRemetentes;
            List<int> cepsDestinatarios = parametros.CEPsDestinatarios;

            List<int> localidadesOrigem = (from obj in origens select obj.Codigo).Distinct().ToList();
            List<int> localidadesDestino = (from obj in destinos select obj.Codigo).Distinct().ToList();

            List<string> estadosOrigem = (from obj in origens where obj.Estado != null select obj.Estado.Sigla).Distinct().ToList();
            List<string> estadosDestino = (from obj in destinos where obj.Estado != null select obj.Estado.Sigla).Distinct().ToList();

            List<int> regioesOrigem = (from obj in origens where obj.Regiao != null select obj.Regiao.Codigo).Distinct().ToList();
            List<int> regioesDestino = (from obj in destinos where obj.Regiao != null select obj.Regiao.Codigo).Distinct().ToList();

            if (parametros.RegioesDestino?.Count > 0)
                regioesDestino = (from obj in parametros.RegioesDestino select obj.Codigo).Distinct().ToList();

            if (!configuracaoTabelaFrete.NaoUtilizarRegiaoParaCalcularFrete)
                if (remetentes.Any(obj => obj.Regiao != null))
                    regioesOrigem = (from obj in remetentes where obj.Regiao != null select obj.Regiao.Codigo).Distinct().ToList();

            List<int> paisesOrigem = remetentes.Where(o => o.Tipo == "E" && o.Pais != null).Select(o => o.Pais.Codigo).ToList();
            paisesOrigem.AddRange(remetentes.Where(o => o.Tipo == "E" && o.Pais == null && o.Localidade.Pais != null).Select(o => o.Localidade.Pais.Codigo).ToList());
            paisesOrigem.AddRange(remetentes.Where(o => o.Tipo != "E" && o.Localidade?.Estado?.Pais != null).Select(o => o.Localidade.Estado.Pais.Codigo));
            paisesOrigem = paisesOrigem.Distinct().ToList();

            List<int> paisesDestino = destinatarios.Where(o => o.Tipo == "E" && o.Pais != null).Select(o => o.Pais.Codigo).ToList();
            paisesDestino.AddRange(destinatarios.Where(o => o.Tipo == "E" && o.Pais == null && o.Localidade.Pais != null).Select(o => o.Localidade.Pais.Codigo).ToList());
            paisesDestino.AddRange(destinatarios.Where(o => o.Tipo != "E" && o.Localidade?.Estado?.Pais != null).Select(o => o.Localidade.Estado.Pais.Codigo));
            paisesDestino = paisesDestino.Distinct().ToList();

            List<int> rotas = parametros.Rota != null ? new List<int>() { parametros.Rota.Codigo } : new List<int>();

            if (cpfCnpjDestinatarios.Count > 200)
            {
                cpfCnpjRemetentes = new List<double>();
                cpfCnpjDestinatarios = new List<double>();
            }

            if (cepsRemetentes == null || cepsRemetentes.Count <= 0)
                cepsRemetentes = (from obj in remetentes where !string.IsNullOrWhiteSpace(obj.CEP) select int.Parse(Utilidades.String.OnlyNumbers(obj.CEP))).Distinct().ToList();

            if (cepsDestinatarios == null || cepsDestinatarios.Count <= 0)
                cepsDestinatarios = (from obj in destinatarios where !string.IsNullOrWhiteSpace(obj.CEP) select int.Parse(Utilidades.String.OnlyNumbers(obj.CEP))).Distinct().ToList();

            bool possuiOrigemSemRegiao = origens.Any(o => o.Regiao == null);
            bool possuiDestinoSemRegiao = parametros.RegioesDestino?.Count == 0 ? destinos.Any(o => o.Regiao == null) : false;

            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteSelecionadas = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasDaCarga = null;

            try
            {
                tabelasDaCarga = repositorioTabelaFreteCliente.BuscarTabelasAptasParaCalculo(tabelaFrete?.Codigo ?? 0, cpfCnpjRemetentes, cepsRemetentes, localidadesOrigem, estadosOrigem, regioesOrigem, paisesOrigem, cpfCnpjDestinatarios, cepsDestinatarios, localidadesDestino, estadosDestino, regioesDestino, paisesDestino, rotas, parametros.DataVigencia, parametros.Empresa?.Codigo ?? 0, parametros.CanalEntrega?.Codigo ?? 0, parametros.CanalVenda?.Codigo ?? 0);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "CalcularFreteCargasPendentes");
            }

            if (tabelasDaCarga?.Count > 0)
            {
                if (SelecionarTabelaFrete(ref tabelasFreteSelecionadas, ref mensagemRetorno, parametros, tabelasDaCarga, cpfCnpjRemetentes, cpfCnpjDestinatarios, localidadesOrigem, localidadesDestino, cepsRemetentes, cepsDestinatarios, estadosOrigem, estadosDestino, regioesOrigem, possuiOrigemSemRegiao, regioesDestino, possuiDestinoSemRegiao, _unitOfWork, tipoServicoMultisoftware))
                    return tabelasFreteSelecionadas;
            }
            else
                mensagemRetorno.Append("Nenhum registro compatível com os parâmetros informados para a tabela " + tabelaFrete?.Descricao ?? "" + ".");

            // Para testar cargas em dev
            // tabelasFreteSelecionadas = tabelasDaCarga;

            return tabelasFreteSelecionadas;
        }

        public void SetarTabelaFreteNotasFiscais(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento, ref Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosRateioFrete parametrosGeraisRateioFrete, bool ultimoRegistro)
        {
            if (!apenasVerificar)
            {
                Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(this._unitOfWork);
                Servicos.Embarcador.Carga.RateioNotaFiscal svcRateioNotaFiscal = new RateioNotaFiscal(this._unitOfWork);
                Servicos.Embarcador.Carga.CargaPedido svcCargaPedido = new CargaPedido(this._unitOfWork);
                Servicos.Embarcador.Carga.ComponetesFrete svcComponentesFrete = new ComponetesFrete(this._unitOfWork);
                Servicos.Embarcador.Carga.CargaFronteira svcCargaFronteira = new CargaFronteira(_unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente repCargaPedidoXMLNotaFiscalTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

                bool possuiFronteira = svcCargaFronteira.TemFronteira(cargaPedido.Carga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido };

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                decimal valorTotalDasNotas = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Valor);
                decimal pesoNotasFiscais = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Peso);
                int volumeNotasFiscais = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Volumes);
                decimal pesoLiquidoNotasFiscais = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.PesoLiquido);
                decimal metrosCubicosNotasFiscais = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.MetrosCubicos);
                int quantidadeNotasFiscais = pedidoXMLNotasFiscais.Count();
                decimal pesoTotalParaCalculoFatorCubagem = svcRateio.ObterPesoTotalCubadoFatorCubagem(pedidoXMLNotasFiscais);

                bool incluirICMS = cargaPedido.IncluirICMSBaseCalculo;
                decimal percentualIncluirICMS = 100;

                if (tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete)
                {
                    if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                        incluirICMS = tabelaFreteCliente.TabelaFrete.IncluirICMSValorFrete;

                    percentualIncluirICMS = tabelaFreteCliente.TabelaFrete.PercentualICMSIncluir;
                }
                else
                {
                    if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                        incluirICMS = tabelaFreteCliente.IncluirICMSValorFrete;

                    percentualIncluirICMS = tabelaFreteCliente.PercentualICMSIncluir;
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                    SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                else
                    SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                if (parametrosGeraisRateioFrete != null && parametrosGeraisRateioFrete.Componentes != null)
                {
                    if (dados.Componentes == null)
                        dados.Componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();

                    for (int i = 0; i < parametrosGeraisRateioFrete.Componentes.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente = parametrosGeraisRateioFrete.Componentes[i];

                        decimal pesoParaCalculoFatorCubagem = 0;

                        if (cargaPedido.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                            pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(cargaPedido.FormulaRateio?.ParametroRateioFormula, cargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedido.FatorCubagemRateioFormula ?? 0, parametrosGeraisRateioFrete.Volumes, parametrosGeraisRateioFrete.Peso, metrosCubicosNotasFiscais);

                        decimal valorComponenteAplicar = 0m;
                        decimal valorRateioOriginal2 = 0;
                        if (ultimoRegistro)
                            valorComponenteAplicar = Math.Round(componente.ValorComponente - componente.ValorTotalRateado, 2, MidpointRounding.AwayFromZero);
                        else
                            valorComponenteAplicar = svcRateio.AplicarFormulaRateio(cargaPedido.FormulaRateio, componente.ValorComponente, parametrosGeraisRateioFrete.QuantidadeNotasFiscais, 0, parametrosGeraisRateioFrete.Peso, pesoNotasFiscais, valorTotalDasNotas, parametrosGeraisRateioFrete.ValorNotasFiscais, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal2, 0m, 0m, 0m, metrosCubicosNotasFiscais, parametrosGeraisRateioFrete.MetrosCubicos, 0, false, pesoNotasFiscais, parametrosGeraisRateioFrete.PesoLiquido, volumeNotasFiscais, parametrosGeraisRateioFrete.Volumes, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        parametrosGeraisRateioFrete.Componentes[i].ValorTotalRateado += valorComponenteAplicar;

                        dados.Componentes.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente()
                        {
                            AcrescentaValorTotalAReceber = componente.AcrescentaValorTotalAReceber,
                            CalculoPorQuantidadeDocumentos = componente.CalculoPorQuantidadeDocumentos,
                            ComponenteComparado = componente.ComponenteComparado,
                            ComponenteFrete = componente.ComponenteFrete,
                            ComponentePorCarga = componente.ComponentePorCarga,
                            DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber,
                            IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS,
                            ModeloDocumentoFiscal = componente.ModeloDocumentoFiscal,
                            ModeloDocumentoFiscalRateio = componente.ModeloDocumentoFiscalRateio,
                            NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber,
                            DescontarDoValorAReceberValorComponente = componente.DescontarDoValorAReceberValorComponente,
                            DescontarDoValorAReceberOICMSDoComponente = componente.DescontarDoValorAReceberOICMSDoComponente,
                            ValorICMSComponenteDestacado = componente.ValorICMSComponenteDestacado,
                            NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao,
                            OutraDescricaoCTe = componente.OutraDescricaoCTe,
                            Percentual = componente.Percentual,
                            QuantidadeTotalDocumentos = componente.QuantidadeTotalDocumentos,
                            RateioFormula = componente.RateioFormula,
                            SomarComponenteFreteLiquido = componente.SomarComponenteFreteLiquido,
                            DescontarComponenteFreteLiquido = componente.DescontarComponenteFreteLiquido,
                            TipoCalculoQuantidadeDocumentos = componente.TipoCalculoQuantidadeDocumentos,
                            TipoComponenteFrete = componente.TipoComponenteFrete,
                            TipoValor = componente.TipoValor,
                            ValorComponente = (componente.DescontarValorTotalAReceber && valorComponenteAplicar > 0) ? -valorComponenteAplicar : valorComponenteAplicar,
                            ValorComponenteMoeda = componente.ValorComponenteMoeda,
                            ValorComponenteParaCarga = componente.ValorComponenteParaCarga
                        });
                    }
                }

                int codigoUltimaNotaFiscal = pedidoXMLNotasFiscais.Last().Codigo;

                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(cargaPedido.Carga, null, pedidoXMLNotasFiscais, null, calculoFreteFilialEmissora, dados.ComposicaoFrete, _unitOfWork, null);

                cargaPedido.Carga.QuantidadeHoras = dados.QuantidadeHoras;
                cargaPedido.Carga.QuantidadeHorasExcedentes = dados.QuantidadeHorasExcedentes;

                repCarga.Atualizar(cargaPedido.Carga);

                decimal valorTotalRateado = 0m, valorTotal = 0m;
                decimal valorRateioOriginal = 0;
                Dictionary<int, decimal> componentesRateados = new Dictionary<int, decimal>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                {
                    pedidoXMLNotaFiscal.PercentualPagamentoAgregado = dados.PercentualPagamentoAgregado;

                    if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        repPedidoXMLNotaFiscalComponenteFrete.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo, calculoFreteFilialEmissora);

                    decimal valorRateado = 0m;
                    decimal densidadeProdutos = cargaPedido.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;

                    decimal pesoParaCalculoFatorCubagem = 0;

                    if (cargaPedido.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                        pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(cargaPedido.FormulaRateio.ParametroRateioFormula, cargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                    if (codigoUltimaNotaFiscal == pedidoXMLNotaFiscal.Codigo)
                        valorRateado = Math.Round(dados.ValorFrete - valorTotalRateado, 2, MidpointRounding.AwayFromZero);
                    else
                        valorRateado = svcRateio.AplicarFormulaRateio(cargaPedido.FormulaRateio, dados.ValorFrete, quantidadeNotasFiscais, 0, pesoNotasFiscais, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalDasNotas, dados.PercentualSobreNF, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNotasFiscais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoNotasFiscais, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeNotasFiscais, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                    valorTotalRateado += valorRateado;

                    if (!calculoFreteFilialEmissora)
                        pedidoXMLNotaFiscal.ValorFreteTabelaFrete = valorRateado;
                    else
                        pedidoXMLNotaFiscal.ValorFreteTabelaFreteFilialEmissora = valorRateado;

                    if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        if (!calculoFreteFilialEmissora)
                            pedidoXMLNotaFiscal.ValorFrete = valorRateado;
                        else
                            pedidoXMLNotaFiscal.ValorFreteFilialEmissora = valorRateado;

                        pedidoXMLNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                    }

                    repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente cargaPedidoXMLNotaFiscalTabelaFreteCliente = repCargaPedidoXMLNotaFiscalTabelaFreteCliente.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo, calculoFreteFilialEmissora);

                    if (cargaPedidoXMLNotaFiscalTabelaFreteCliente == null)
                    {
                        cargaPedidoXMLNotaFiscalTabelaFreteCliente = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente();
                        cargaPedidoXMLNotaFiscalTabelaFreteCliente.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                    }

                    cargaPedidoXMLNotaFiscalTabelaFreteCliente.TabelaFreteCliente = tabelaFreteCliente;
                    cargaPedidoXMLNotaFiscalTabelaFreteCliente.ValorFixo = valorRateado;
                    cargaPedidoXMLNotaFiscalTabelaFreteCliente.ValorFrete = valorRateado;
                    cargaPedidoXMLNotaFiscalTabelaFreteCliente.PercentualSobreNF = dados.PercentualSobreNF;
                    cargaPedidoXMLNotaFiscalTabelaFreteCliente.Observacao = dados.Observacao;
                    cargaPedidoXMLNotaFiscalTabelaFreteCliente.ObservacaoTerceiro = dados.ObservacaoTerceiro;

                    if (cargaPedidoXMLNotaFiscalTabelaFreteCliente.Codigo > 0)
                        repCargaPedidoXMLNotaFiscalTabelaFreteCliente.Atualizar(cargaPedidoXMLNotaFiscalTabelaFreteCliente);
                    else
                        repCargaPedidoXMLNotaFiscalTabelaFreteCliente.Inserir(cargaPedidoXMLNotaFiscalTabelaFreteCliente);

                    foreach (var componente in dados.Componentes)
                    {
                        decimal valorTotalRateadoComponente = 0m;
                        decimal valorRateadoComponente = 0m;

                        componentesRateados.TryGetValue(componente.ComponenteFrete.Codigo, out valorTotalRateadoComponente);

                        if (codigoUltimaNotaFiscal == pedidoXMLNotaFiscal.Codigo)
                            valorRateadoComponente = Math.Round(componente.ValorComponente - valorTotalRateadoComponente, 2, MidpointRounding.AwayFromZero);
                        else
                            valorRateadoComponente = svcRateio.AplicarFormulaRateio(cargaPedido.FormulaRateio, componente.ValorComponente, quantidadeNotasFiscais, 0, pesoNotasFiscais, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalDasNotas, dados.PercentualSobreNF, componente.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNotasFiscais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoNotasFiscais, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeNotasFiscais, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        componentesRateados[componente.ComponenteFrete.Codigo] = valorTotalRateadoComponente + valorRateadoComponente;

                        pedidoXMLNotaFiscal.ValorFreteTabelaFrete += valorRateadoComponente;

                        if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = new PedidoXMLNotaFiscalComponenteFrete()
                            {
                                AcrescentaValorTotalAReceber = componente.AcrescentaValorTotalAReceber,
                                NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber,
                                DescontarDoValorAReceberValorComponente = componente.DescontarDoValorAReceberValorComponente,
                                DescontarDoValorAReceberOICMSDoComponente = componente.DescontarDoValorAReceberOICMSDoComponente,
                                ValorICMSComponenteDestacado = componente.ValorICMSComponenteDestacado,
                                NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao,
                                ComponenteFrete = componente.ComponenteFrete,
                                ComponenteFilialEmissora = calculoFreteFilialEmissora,
                                DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber,
                                IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS,
                                ModeloDocumentoFiscal = componente.ModeloDocumentoFiscal,
                                OutraDescricaoCTe = componente.OutraDescricaoCTe,
                                PedidoXMLNotaFiscal = pedidoXMLNotaFiscal,
                                Percentual = componente.Percentual,
                                RateioFormula = cargaPedido.FormulaRateio,
                                TipoComponenteFrete = componente.TipoComponenteFrete,
                                TipoValor = componente.TipoValor,
                                ValorComponente = (componente.DescontarValorTotalAReceber && valorRateadoComponente > 0) ? -valorRateadoComponente : valorRateadoComponente,
                                IncluirIntegralmenteContratoFreteTerceiro = false
                            };

                            if (componente.CalculoPorQuantidadeDocumentos)
                            {
                                pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscalRateio = componente.ModeloDocumentoFiscalRateio;
                                pedidoXMLNotaFiscalComponenteFrete.PorQuantidadeDocumentos = componente.CalculoPorQuantidadeDocumentos;
                                pedidoXMLNotaFiscalComponenteFrete.TipoCalculoQuantidadeDocumentos = componente.TipoCalculoQuantidadeDocumentos;
                            }

                            repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);

                            svcComponentesFrete.AdicionarCargaPedidoComponente(cargaPedido, pedidoXMLNotaFiscalComponenteFrete.ValorComponente, pedidoXMLNotaFiscalComponenteFrete.Percentual, pedidoXMLNotaFiscalComponenteFrete.TipoValor, pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete, pedidoXMLNotaFiscalComponenteFrete.ComponenteFrete, pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS, pedidoXMLNotaFiscalComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro, pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscal, pedidoXMLNotaFiscalComponenteFrete.RateioFormula, pedidoXMLNotaFiscalComponenteFrete.OutraDescricaoCTe, pedidoXMLNotaFiscalComponenteFrete.DescontarValorTotalAReceber, pedidoXMLNotaFiscalComponenteFrete.AcrescentaValorTotalAReceber, calculoFreteFilialEmissora, _unitOfWork, pedidoXMLNotaFiscalComponenteFrete.PorQuantidadeDocumentos, pedidoXMLNotaFiscalComponenteFrete.TipoCalculoQuantidadeDocumentos, 1, pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscalRateio, pedidoXMLNotaFiscalComponenteFrete.NaoSomarValorTotalAReceber, pedidoXMLNotaFiscalComponenteFrete.NaoSomarValorTotalPrestacao);
                            svcComponentesFrete.AdicionarComponenteFreteCargaUnicoPorTipo(cargaPedido.Carga, pedidoXMLNotaFiscalComponenteFrete.ComponenteFrete, pedidoXMLNotaFiscalComponenteFrete.ValorComponente, pedidoXMLNotaFiscalComponenteFrete.Percentual, calculoFreteFilialEmissora, pedidoXMLNotaFiscalComponenteFrete.TipoValor, pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete, null, pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS, pedidoXMLNotaFiscalComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro, pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscal, tipoServicoMultisoftware, null, _unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, false, pedidoXMLNotaFiscalComponenteFrete.PorQuantidadeDocumentos, pedidoXMLNotaFiscalComponenteFrete.TipoCalculoQuantidadeDocumentos, 1, pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscalRateio);
                        }
                    }
                    repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                    valorTotal += cargaPedidoXMLNotaFiscalTabelaFreteCliente.ValorFrete;
                }

                if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                    cargaPedido.IncluirICMSBaseCalculo = incluirICMS;

                cargaPedido.PercentualIncluirBaseCalculo = percentualIncluirICMS;

                if (!calculoFreteFilialEmissora)
                    AtualizarPedido(tabelaFreteCliente, cargaPedido.Carga, cargaPedido, valorTotal, tabelaFreteCliente, atualizarInformacoesPagamentoPedido, dados.Observacao, dados.ObservacaoTerceiro, cargaPedidoModalidadesPagamento, _unitOfWork, possuiFronteira);
                else
                    AtualizarPedidoFilialEmissora(tabelaFreteCliente, cargaPedido.Carga, cargaPedido, valorTotal, tabelaFreteCliente, atualizarInformacoesPagamentoPedido, dados.Observacao, dados.ObservacaoTerceiro);

                repCargaPedido.Atualizar(cargaPedido);
                repPedido.Atualizar(cargaPedido.Pedido);

                if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    bool utilizarTransacao = false;
                    if (!apenasVerificar && !_unitOfWork.IsActiveTransaction())
                    {
                        utilizarTransacao = true;
                        _unitOfWork.Start();
                    }

                    svcRateioNotaFiscal.CalcularImpostos(cargaPedido.FormulaRateio, cargaPedido, cargaPedido.Carga, calculoFreteFilialEmissora, pedidoXMLNotasFiscais, pedidoXMLNotasFiscais.Sum(o => o.ValorFrete), incluirICMS, percentualIncluirICMS, tipoServicoMultisoftware, cargaPedido.Recebedor, _unitOfWork, configuracao, cargaPedidoProdutos, cargaPedido.Expedidor, pesoTotalParaCalculoFatorCubagem);

                    if (utilizarTransacao)
                        _unitOfWork.CommitChanges();
                }
            }
        }

        public void SetarTabelaFreteCTesTerceiro(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {
            if (!apenasVerificar)
            {
                Servicos.Embarcador.Carga.CTeSubContratacao svcCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(this._unitOfWork);
                Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(this._unitOfWork);
                Servicos.Embarcador.Carga.RateioNotaFiscal svcRateioNotaFiscal = new RateioNotaFiscal(this._unitOfWork);
                Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao svcRateioCTeSubcontratacao = new RateioCTeParaSubcontratacao(this._unitOfWork);
                Servicos.Embarcador.Carga.CargaPedido svcCargaPedido = new CargaPedido(this._unitOfWork);
                Servicos.Embarcador.Carga.ComponetesFrete svcComponentesFrete = new ComponetesFrete(this._unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCTeParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido };
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCteParaSubContratacaoComponentesFrete = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();

                string descricaoItemPeso = svcCTeSubContratacao.ObterDescricaoItemPeso(cargaPedido, _unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);

                decimal valorTotalDasNotas = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.ValorTotalMercadoria);
                decimal valorTotalCTes = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.ValorAReceber);
                decimal pesoNotasFiscais = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.Peso);
                int quantidadeNotasFiscais = pedidoCTesParaSubcontratacao.Sum(o => o.CTeTerceiro.NumeroTotalDocumentos);

                bool incluirICMS = cargaPedido.IncluirICMSBaseCalculo;
                decimal percentualIncluirICMS = 100;

                if (tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete)
                {
                    if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                        incluirICMS = tabelaFreteCliente.TabelaFrete.IncluirICMSValorFrete;

                    percentualIncluirICMS = tabelaFreteCliente.TabelaFrete.PercentualICMSIncluir;
                }
                else
                {
                    if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                        incluirICMS = tabelaFreteCliente.IncluirICMSValorFrete;

                    percentualIncluirICMS = tabelaFreteCliente.PercentualICMSIncluir;
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
                dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                    SetarValoresTabelaFreteComParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                else
                    SetarValoresTabelaFreteSemParametroBase(ref dados, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                int codigoUltimaNotaFiscal = pedidoCTesParaSubcontratacao.Last().Codigo;

                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(cargaPedido.Carga, null, null, pedidoCTesParaSubcontratacao, calculoFreteFilialEmissora, dados.ComposicaoFrete, _unitOfWork, null);

                cargaPedido.Carga.QuantidadeHoras = dados.QuantidadeHoras;
                cargaPedido.Carga.QuantidadeHorasExcedentes = dados.QuantidadeHorasExcedentes;

                repCarga.Atualizar(cargaPedido.Carga);

                decimal valorTotalRateado = 0m;
                Dictionary<int, decimal> componentesRateados = new Dictionary<int, decimal>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubcontratacao)
                {
                    pedidoCTeParaSubContratacao.PercentualPagamentoAgregado = dados.PercentualPagamentoAgregado;

                    if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        repPedidoCTeParaSubContratacaoComponenteFrete.DeletarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

                    decimal valorRateioOriginal = 0;
                    decimal valorRateado = 0m;
                    decimal peso = pedidoCTeParaSubContratacao.CTeTerceiro.Peso;
                    decimal densidadeProdutos = cargaPedido.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;

                    if (codigoUltimaNotaFiscal == pedidoCTeParaSubContratacao.Codigo)
                        valorRateado = Math.Round(dados.ValorFrete - valorTotalRateado, 2, MidpointRounding.AwayFromZero);
                    else
                        valorRateado = svcRateio.AplicarFormulaRateio(formulaRateio, dados.ValorFrete, quantidadeNotasFiscais, pedidoCTesParaSubcontratacao.Count, pesoNotasFiscais, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalDasNotas, dados.PercentualSobreNF, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTes);

                    valorTotalRateado += valorRateado;

                    if (!calculoFreteFilialEmissora)
                        pedidoCTeParaSubContratacao.ValorFreteTabelaFrete = valorRateado;

                    if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        if (!calculoFreteFilialEmissora)
                            pedidoCTeParaSubContratacao.ValorFrete = valorRateado;

                        pedidoCTeParaSubContratacao.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                    }

                    repPedidoCTeParaSubcontratacao.Atualizar(pedidoCTeParaSubContratacao);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente = repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

                    if (cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente == null)
                    {
                        cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente();
                        cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                    }

                    cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.TabelaFreteCliente = tabelaFreteCliente;
                    cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.ValorFixo = valorRateado;
                    cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.ValorFrete = valorRateado;
                    cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.PercentualSobreNF = dados.PercentualSobreNF;
                    cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.Observacao = dados.Observacao;
                    cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.ObservacaoTerceiro = dados.ObservacaoTerceiro;

                    if (cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.Codigo > 0)
                        repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.Atualizar(cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente);
                    else
                        repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.Inserir(cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente);

                    foreach (var componente in dados.Componentes)
                    {
                        decimal valorTotalRateadoComponente = 0m;
                        decimal valorRateadoComponente = 0m;
                        valorRateioOriginal = 0;

                        componentesRateados.TryGetValue(componente.ComponenteFrete.Codigo, out valorTotalRateadoComponente);

                        if (codigoUltimaNotaFiscal == pedidoCTeParaSubContratacao.Codigo)
                            valorRateadoComponente = Math.Round(componente.ValorComponente - valorTotalRateadoComponente, 2, MidpointRounding.AwayFromZero);
                        else
                            valorRateadoComponente = svcRateio.AplicarFormulaRateio(formulaRateio, componente.ValorComponente, quantidadeNotasFiscais, pedidoCTesParaSubcontratacao.Count, pesoNotasFiscais, peso, pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria, valorTotalDasNotas, dados.PercentualSobreNF, componente.TipoValor, 0, 0, ref valorRateioOriginal, pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber, 0, valorTotalCTes);

                        componentesRateados[componente.ComponenteFrete.Codigo] = valorTotalRateadoComponente + valorRateadoComponente;

                        pedidoCTeParaSubContratacao.ValorFreteTabelaFrete += valorRateadoComponente;

                        if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete = new PedidoCteParaSubContratacaoComponenteFrete()
                            {
                                AcrescentaValorTotalAReceber = componente.AcrescentaValorTotalAReceber,
                                NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber,
                                DescontarDoValorAReceberValorComponente = componente.DescontarDoValorAReceberValorComponente,
                                DescontarDoValorAReceberOICMSDoComponente = componente.DescontarDoValorAReceberOICMSDoComponente,
                                ValorICMSComponenteDestacado = componente.ValorICMSComponenteDestacado,
                                NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao,
                                ComponenteFrete = componente.ComponenteFrete,
                                DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber,
                                IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS,
                                ModeloDocumentoFiscal = componente.ModeloDocumentoFiscal,
                                OutraDescricaoCTe = componente.OutraDescricaoCTe,
                                PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao,
                                Percentual = componente.Percentual,
                                RateioFormula = formulaRateio,
                                TipoComponenteFrete = componente.TipoComponenteFrete,
                                TipoValor = componente.TipoValor,
                                ValorComponente = (componente.DescontarValorTotalAReceber && valorRateadoComponente > 0) ? -valorRateadoComponente : valorRateadoComponente,
                                IncluirIntegralmenteContratoFreteTerceiro = false
                            };

                            if (componente.CalculoPorQuantidadeDocumentos)
                            {
                                pedidoCteParaSubContratacaoComponenteFrete.ModeloDocumentoFiscalRateio = componente.ModeloDocumentoFiscalRateio;
                                pedidoCteParaSubContratacaoComponenteFrete.PorQuantidadeDocumentos = componente.CalculoPorQuantidadeDocumentos;
                                pedidoCteParaSubContratacaoComponenteFrete.TipoCalculoQuantidadeDocumentos = componente.TipoCalculoQuantidadeDocumentos;
                            }

                            repPedidoCTeParaSubContratacaoComponenteFrete.Inserir(pedidoCteParaSubContratacaoComponenteFrete);

                            svcComponentesFrete.AdicionarCargaPedidoComponente(cargaPedido, pedidoCteParaSubContratacaoComponenteFrete.ValorComponente, pedidoCteParaSubContratacaoComponenteFrete.Percentual, pedidoCteParaSubContratacaoComponenteFrete.TipoValor, pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete, pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete, pedidoCteParaSubContratacaoComponenteFrete.IncluirBaseCalculoICMS, pedidoCteParaSubContratacaoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro, pedidoCteParaSubContratacaoComponenteFrete.ModeloDocumentoFiscal, pedidoCteParaSubContratacaoComponenteFrete.RateioFormula, pedidoCteParaSubContratacaoComponenteFrete.OutraDescricaoCTe, pedidoCteParaSubContratacaoComponenteFrete.DescontarValorTotalAReceber, pedidoCteParaSubContratacaoComponenteFrete.AcrescentaValorTotalAReceber, calculoFreteFilialEmissora, _unitOfWork, pedidoCteParaSubContratacaoComponenteFrete.PorQuantidadeDocumentos, pedidoCteParaSubContratacaoComponenteFrete.TipoCalculoQuantidadeDocumentos, 1, pedidoCteParaSubContratacaoComponenteFrete.ModeloDocumentoFiscalRateio, pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalAReceber, pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalPrestacao);
                            svcComponentesFrete.AdicionarComponenteFreteCargaUnicoPorTipo(cargaPedido.Carga, pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete, pedidoCteParaSubContratacaoComponenteFrete.ValorComponente, pedidoCteParaSubContratacaoComponenteFrete.Percentual, calculoFreteFilialEmissora, pedidoCteParaSubContratacaoComponenteFrete.TipoValor, pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete, null, pedidoCteParaSubContratacaoComponenteFrete.IncluirBaseCalculoICMS, pedidoCteParaSubContratacaoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro, pedidoCteParaSubContratacaoComponenteFrete.ModeloDocumentoFiscal, tipoServicoMultisoftware, null, _unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, false, pedidoCteParaSubContratacaoComponenteFrete.PorQuantidadeDocumentos, pedidoCteParaSubContratacaoComponenteFrete.TipoCalculoQuantidadeDocumentos, 1, pedidoCteParaSubContratacaoComponenteFrete.ModeloDocumentoFiscalRateio);

                            pedidoCteParaSubContratacaoComponentesFrete.Add(pedidoCteParaSubContratacaoComponenteFrete);
                        }
                    }

                    repPedidoCTeParaSubcontratacao.Atualizar(pedidoCTeParaSubContratacao);
                }

                if (!cargaPedido.Carga.DadosPagamentoInformadosManualmente)
                    cargaPedido.IncluirICMSBaseCalculo = incluirICMS;

                cargaPedido.PercentualIncluirBaseCalculo = percentualIncluirICMS;

                if (!string.IsNullOrWhiteSpace(dados.Observacao))
                {
                    if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                        cargaPedido.Pedido.ObservacaoCTe = dados.Observacao;
                    else if (!cargaPedido.Pedido.ObservacaoCTe.ToLower().Contains(dados.Observacao.ToLower()))
                        cargaPedido.Pedido.ObservacaoCTe += " / " + dados.Observacao;
                }

                if (!string.IsNullOrWhiteSpace(dados.ObservacaoTerceiro))
                {
                    if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTeTerceiro))
                        cargaPedido.Pedido.ObservacaoCTeTerceiro = dados.ObservacaoTerceiro;
                    else if (!cargaPedido.Pedido.ObservacaoCTeTerceiro.ToLower().Contains(dados.ObservacaoTerceiro.ToLower()))
                        cargaPedido.Pedido.ObservacaoCTeTerceiro += " / " + dados.ObservacaoTerceiro;
                }

                repCargaPedido.Atualizar(cargaPedido);
                repPedido.Atualizar(cargaPedido.Pedido);

                if (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    bool utilizarTransacao = false;
                    if (!apenasVerificar && !_unitOfWork.IsActiveTransaction())
                    {
                        utilizarTransacao = true;
                        _unitOfWork.Start();
                    }

                    svcRateioCTeSubcontratacao.CalcularImpostos(cargaPedido.Carga, cargaPedido, formulaRateio, pedidoCTesParaSubcontratacao, pedidoCteParaSubContratacaoComponentesFrete, pedidoCTesParaSubcontratacao.Sum(o => o.ValorFrete), incluirICMS, percentualIncluirICMS, descricaoItemPeso, cargaPedidoContaContabilContabilizacaos, _unitOfWork, tipoServicoMultisoftware, configuracao, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);

                    if (utilizarTransacao)
                        _unitOfWork.CommitChanges();
                }
            }
        }

        public void SetarTabelaFreteCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, ref List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesPorCarga, bool ultimoPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> tabelaCargaPedidoCargas, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosModalidades, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, bool setarComposicaoFrete = true)
        {
            if (!apenasVerificar)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Servicos.Embarcador.Carga.CargaFronteira svcCargaFronteira = new CargaFronteira(_unitOfWork);
                bool possuiFronteira = svcCargaFronteira.TemFronteira(carga);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente tabelaCargaPedido = (from obj in tabelaCargaPedidoCargas where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                cargaPedidos.Add(cargaPedido);

                if (tabelaCargaPedido == null)
                {
                    tabelaCargaPedido = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente();
                    tabelaCargaPedido.CargaPedido = cargaPedido;
                }

                tabelaCargaPedido.TabelaFreteCliente = tabelaFreteCliente;

                if (setarComposicaoFrete)
                    Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, null, null, calculoFreteFilialEmissora, dados.ComposicaoFrete, _unitOfWork, null);

                tabelaCargaPedido.ValorFixo = Math.Round(dados.ValorFixo, 2, MidpointRounding.AwayFromZero);
                tabelaCargaPedido.ValorFrete = Math.Round(dados.ValorFrete, 2, MidpointRounding.AwayFromZero);
                tabelaCargaPedido.PercentualSobreNF = dados.PercentualSobreNF;
                tabelaCargaPedido.Observacao = dados.Observacao;
                tabelaCargaPedido.ObservacaoTerceiro = dados.ObservacaoTerceiro;
                tabelaCargaPedido.TabelaFreteFilialEmissora = calculoFreteFilialEmissora;

                cargaPedido.PercentualPagamentoAgregado = dados.PercentualPagamentoAgregado;
                cargaPedido.ValorBaseFrete = dados.ValorBase;
                cargaPedido.ValorFreteResidual = dados.ValorFreteResidual;

                carga.QuantidadeHoras = dados.QuantidadeHoras;
                carga.QuantidadeHorasExcedentes = dados.QuantidadeHorasExcedentes;

                carga.ValorFreteResidual += dados.ValorFreteResidual;

                if (tabelaCargaPedido.Codigo > 0)
                    repCargaPedidoTabelaFreteCliente.Atualizar(tabelaCargaPedido);
                else
                    repCargaPedidoTabelaFreteCliente.Inserir(tabelaCargaPedido);

                if (!calculoFreteFilialEmissora)
                    AtualizarPedido(tabelaFreteCliente, carga, cargaPedido, tabelaCargaPedido.ValorFrete, tabelaCargaPedido.TabelaFreteCliente, atualizarInformacoesPagamentoPedido, tabelaCargaPedido.Observacao, tabelaCargaPedido.ObservacaoTerceiro, cargaPedidosModalidades, _unitOfWork, possuiFronteira);
                else
                    AtualizarPedidoFilialEmissora(tabelaFreteCliente, carga, cargaPedido, tabelaCargaPedido.ValorFrete, tabelaCargaPedido.TabelaFreteCliente, atualizarInformacoesPagamentoPedido, tabelaCargaPedido.Observacao, tabelaCargaPedido.ObservacaoTerceiro);

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    if (adicionarComponentesCarga)
                        SalvarComponentesCargaPedido(dados, cargaPedido, calculoFreteFilialEmissora, _unitOfWork, ref componentesPorCarga, ultimoPedido, cargaPedidoComponentesFretes);

                    AdicionarComponentesPedido(carga, cargaPedidos, cargaPedidoComponentesFretes, _unitOfWork);
                }

                SalvarFreteCargaPedido(carga, cargasOrigem, cargaPedido, tabelaCargaPedido, _unitOfWork, apenasVerificar, parametros.ValorNotasFiscais, calculoFreteFilialEmissora, tipoServicoMultisoftware, configuracao, cargaPedidoComponentesFretes, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);

                if (adicionarComponentesCarga && carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    if (!calculoFreteFilialEmissora)
                        cargaPedido.ValorFreteTabelaFrete += dados.Componentes.Sum(o => o.ValorComponente);
                    else
                        cargaPedido.ValorFreteTabelaFreteFilialEmissora += dados.Componentes.Sum(o => o.ValorComponente);
                }

                SetarPrevisaoEntregaTeoricaPedido(tabelaCargaPedido, configuracao, _unitOfWork);

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete SetarTabelaFreteCarga(ref Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            decimal valorTotalDasNotas = preCarga.Pedidos.Sum(obj => obj.ValorTotalNotasFiscais); //ObterValorTotalNotasFiscais(preCarga, unidadeDeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
            dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

            if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                SetarValoresTabelaFreteComParametroBase(ref dados, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
            else
                SetarValoresTabelaFreteSemParametroBase(ref dados, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

            preCarga.TabelaFrete = tabelaFreteCliente.TabelaFrete;
            preCarga.ValorFrete = dados.ValorFrete;
            preCarga.PendenciaCalculoFrete = false;
            repPreCarga.Atualizar(preCarga);

            SalvarComponentesCarga(dados, preCarga, false, _unitOfWork);

            //AtualizarPedidos(tabelaFreteCliente, preCarga, cargaPedidos, tabelaCarga, unidadeDeTrabalho, atualizarInformacoesPagamentoPedido, calculoFreteFilialEmissora);

            preCarga.PendenciaCalculoFrete = false;
            preCarga.MotivoPendencia = "";

            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(null, null, null, null, false, dados.ComposicaoFrete, _unitOfWork, preCarga);

            Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(this._unitOfWork);
            serFreteRateio.RatearFreteEntrePedidos(preCarga, preCarga.ValorFrete, false, tipoServicoMultisoftware, _unitOfWork, configuracao);
            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido };

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete SetarTabelaFreteCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora)
        {
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFrete = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente tabelaCarga = repCargaTabelaFrete.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

            decimal valorTotalDasNotas = ObterValorTotalNotasFiscais(carga, _unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();
            dados.ComposicaoFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

            if (!apenasVerificar)
            {
                if (!calculoFreteFilialEmissora)
                {
                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        carga.TabelaFrete = tabelaFreteCliente.TabelaFrete;
                }
                else
                    carga.TabelaFreteFilialEmissora = tabelaFreteCliente.TabelaFrete;

                if (tabelaFreteCliente.TabelaFrete != null && tabelaFreteCliente.TabelaFrete.UtilizaModeloVeicularVeiculo)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = null;
                    if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                        modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                    else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                        modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
                    if (modeloVeicularCalculo != null)
                        carga.ModeloVeicularCarga = modeloVeicularCalculo;
                }

                //repCarga.Atualizar(carga);

                if (tabelaCarga == null)
                {
                    tabelaCarga = new Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente();
                    tabelaCarga.Carga = carga;
                }

                tabelaCarga.TabelaFreteFilialEmissora = calculoFreteFilialEmissora;

                tabelaCarga.TabelaFreteCliente = tabelaFreteCliente;

                dados.TabelaFrete = carga?.TabelaFrete ?? null;

                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                    SetarValoresTabelaFreteComParametroBase(ref dados, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                else
                    SetarValoresTabelaFreteSemParametroBase(ref dados, parametros, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                AdicionarComponenteFretePedidoEmCarga(ref dados, carga, cargaPedidos, _unitOfWork);

                tabelaCarga.ValorFixo = dados.ValorFixo;
                tabelaCarga.ValorFrete = dados.ValorFrete;
                tabelaCarga.PercentualSobreNF = dados.PercentualSobreNF;
                tabelaCarga.Observacao = dados.Observacao?.Left(2000);
                tabelaCarga.ObservacaoTerceiro = dados.ObservacaoTerceiro?.Left(2000);
                tabelaCarga.Moeda = dados.Moeda;
                tabelaCarga.ValorCotacaoMoeda = dados.ValorCotacaoMoeda;
                tabelaCarga.ValorTotalMoeda = dados.ValorFreteMoeda;

                if (tabelaCarga.Codigo > 0)
                    repCargaTabelaFrete.Atualizar(tabelaCarga);
                else
                    repCargaTabelaFrete.Inserir(tabelaCarga);


                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    if (adicionarComponentesCarga)
                        SalvarComponentesCarga(dados, carga, calculoFreteFilialEmissora, _unitOfWork);

                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Servicos.Embarcador.Carga.ComponetesFrete serCargaComponentesFrete = new ComponetesFrete(_unitOfWork);

                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = carga.Veiculo?.ModeloCarroceria?.ComponenteFrete;

                        if (componente == null && carga.VeiculosVinculados != null)
                            componente = (from obj in carga.VeiculosVinculados where obj.ModeloCarroceria != null && obj.ModeloCarroceria.ComponenteFrete != null select obj.ModeloCarroceria.ComponenteFrete).FirstOrDefault();

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFreteAdicionarCarroceria = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                        decimal valorAdicionalCarroceria = Frete.CalcularValorFreteAdicionalPorModeloCarroceriaVeiculo(carga.Veiculo, carga.VeiculosVinculados, ref composicaoFreteAdicionarCarroceria, componente, carga.ValorFrete);

                        if (componente != null && valorAdicionalCarroceria > 0)
                        {
                            serCargaComponentesFrete.AdicionarComponenteFreteCarga(carga, componente, valorAdicionalCarroceria, 0, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, componente.TipoComponenteFrete, null, true, false, null, tipoServicoMultisoftware, null, _unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true);
                            dados.ComposicaoFrete.Add(composicaoFreteAdicionarCarroceria);
                        }
                    }

                    AtualizarPedidos(tabelaFreteCliente, carga, cargaPedidos, tabelaCarga, _unitOfWork, atualizarInformacoesPagamentoPedido, calculoFreteFilialEmissora);

                    if (!calculoFreteFilialEmissora)
                    {
                        carga.PossuiPendencia = false;

                        if (!configuracao.CalcularFreteInicioCarga || (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe))
                        {
                            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);

                            bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao() && (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false);

                            if (!carga.ExigeNotaFiscalParaCalcularFrete && !existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                            else
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                Servicos.Log.GravarInfo("Atualizou a situação para calculo frete 8 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                        carga.MotivoPendencia = "";
                    }
                }

                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, dados.ComposicaoFrete, _unitOfWork, null);

                carga.QuantidadeHoras = dados.QuantidadeHoras;
                carga.QuantidadeHorasExcedentes = dados.QuantidadeHorasExcedentes;
                carga.PercentualPagamentoAgregado = dados.PercentualPagamentoAgregado;

                repCarga.Atualizar(carga);
            }

            var retorno = CalcularFrete(ref carga, cargaPedidos, configuracao, ref tabelaCarga, _unitOfWork, apenasVerificar, calculoFreteFilialEmissora, valorTotalDasNotas, tipoServicoMultisoftware, dados);

            return retorno;
        }

        public void SimularCalculoFrete(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem itemSimulacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            //TODO: implementar corretamente a simulação do frete com os cálculos novos (por nota, etc)

            //Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            //Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            //Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem repAjusteTabelaFreteSimulacaoItem = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem(_unitOfWork);
            //Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente repAjusteTabelaFreteSimulacaoItemComponente = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente(_unitOfWork);

            //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(itemSimulacao.TabelaCarga.Carga.Codigo);

            //decimal valorTotalDasNotas = ObterValorTotalNotasFiscais(itemSimulacao.TabelaCarga.Carga, _unitOfWork);

            //Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            //if (itemSimulacao.TabelaAjuste.TabelaFrete.ParametroBase.HasValue)
            //    SetarValoresTabelaFreteComParametroBase(ref dados, parametros, itemSimulacao.TabelaAjuste, _unitOfWork, tipoServicoMultisoftware);
            //else
            //    SetarValoresTabelaFreteSemParametroBase(ref dados, parametros, itemSimulacao.TabelaAjuste, _unitOfWork, tipoServicoMultisoftware);

            //decimal baseCalculoICMSOriginal = itemSimulacao.TabelaCarga.Carga.ValorFrete + itemSimulacao.TabelaCarga.Carga.Componentes.Where(o => o.IncluirBaseCalculoICMS).Sum(o => o.ValorComponente);
            //decimal baseCalculoICMSAjuste = dados.ValorFrete + dados.Componentes.Where(o => o.IncluirBaseCalculoICMS).Sum(o => o.ValorComponente);
            //decimal valorICMS = (baseCalculoICMSAjuste * itemSimulacao.TabelaCarga.Carga.ValorICMS) / baseCalculoICMSOriginal;

            //itemSimulacao.ValorFreteTotal = dados.ValorFrete + dados.Componentes.Sum(o => o.ValorComponente) + valorICMS;
            //itemSimulacao.ValorFrete = dados.ValorFrete;
            //itemSimulacao.ValorICMS = valorICMS;

            //repAjusteTabelaFreteSimulacaoItem.Atualizar(itemSimulacao);

            //for (var i = 0; i < dados.Componentes.Count; i++)
            //{
            //    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente = dados.Componentes[i];

            //    Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente componenteSimulacao = new AjusteTabelaFreteSimulacaoItemComponente();
            //    componenteSimulacao.ComponenteFrete = componente.ComponenteFrete;
            //    componenteSimulacao.IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS;
            //    componenteSimulacao.ItemSimulacao = itemSimulacao;
            //    componenteSimulacao.ValorComponente = componente.ValorComponente;

            //    repAjusteTabelaFreteSimulacaoItemComponente.Inserir(componenteSimulacao);
            //}
        }

        public bool PermiteCalcularFrete(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaCliente)
        {
            if (tabelaCliente == null)
                return false;

            return PermiteCalcularFrete(new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>() { tabelaCliente });
        }

        public bool PermiteCalcularFrete(List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente)
        {
            if ((tabelasCliente.Count <= 0) || (tabelasCliente.Count > 1))
                return false;

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaCliente = tabelasCliente[0];

            return ((tabelaCliente.Tipo != TipoTabelaFreteCliente.Alteracao) || tabelaCliente.PermitirCalcularFreteEmAlteracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados 

        private void AdicionarComponentesPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFrete, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarComponentesManuaisPedidoComTabelaFrete ?? false))
                return;

            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> componentesFretePedidos = repPedidoComponenteFrete.BuscarPorPedidos(cargaPedidos.Select(o => o.Pedido.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> componentesFretePedido = componentesFretePedidos.Where(o => o.Pedido.Codigo == cargaPedido.Pedido.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componenteFretePedido in componentesFretePedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = (from obj in cargaPedidoComponentesFrete where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoComponenteFrete == componenteFretePedido.ComponenteFrete.TipoComponenteFrete && ((componenteFretePedido.ComponenteFrete == null && obj.ComponenteFrete == null) || (componenteFretePedido.ComponenteFrete != null && obj.ComponenteFrete.Codigo == componenteFretePedido.ComponenteFrete.Codigo)) select obj).FirstOrDefault();

                    bool inserir = false;
                    if (cargaPedidoComponenteFrete == null)
                    {
                        cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                        inserir = true;
                    }

                    cargaPedidoComponenteFrete.ComponenteFilialEmissora = false;
                    cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                    cargaPedidoComponenteFrete.ComponenteFrete = componenteFretePedido.ComponenteFrete;
                    cargaPedidoComponenteFrete.TipoComponenteFrete = componenteFretePedido.TipoComponenteFrete;
                    cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = componenteFretePedido.IncluirBaseCalculoICMS;
                    cargaPedidoComponenteFrete.DescontarValorTotalAReceber = componenteFretePedido.DescontarValorTotalAReceber;
                    cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = componenteFretePedido.AcrescentaValorTotalAReceber;
                    cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = componenteFretePedido.NaoSomarValorTotalAReceber;
                    cargaPedidoComponenteFrete.DescontarDoValorAReceberValorComponente = componenteFretePedido.DescontarDoValorAReceberValorComponente;
                    cargaPedidoComponenteFrete.DescontarDoValorAReceberOICMSDoComponente = componenteFretePedido.DescontarDoValorAReceberOICMSDoComponente;
                    cargaPedidoComponenteFrete.ValorICMSComponenteDestacado = componenteFretePedido.ValorICMSComponenteDestacado;
                    cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = componenteFretePedido.NaoSomarValorTotalPrestacao;
                    cargaPedidoComponenteFrete.ModeloDocumentoFiscal = componenteFretePedido.ModeloDocumentoFiscal;
                    cargaPedidoComponenteFrete.RateioFormula = componenteFretePedido.RateioFormula;
                    cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                    cargaPedidoComponenteFrete.DescontarComponenteFreteLiquido = componenteFretePedido.DescontarComponenteFreteLiquido;


                    if (!string.IsNullOrWhiteSpace(componenteFretePedido.OutraDescricaoCTe))
                        cargaPedidoComponenteFrete.OutraDescricaoCTe = componenteFretePedido.OutraDescricaoCTe;
                    else
                        cargaPedidoComponenteFrete.OutraDescricaoCTe = "";

                    cargaPedidoComponenteFrete.Percentual = componenteFretePedido.Percentual;
                    cargaPedidoComponenteFrete.TipoValor = componenteFretePedido.TipoValor;
                    cargaPedidoComponenteFrete.ValorComponente = (componenteFretePedido.DescontarValorTotalAReceber && componenteFretePedido.ValorComponente > 0) ? -componenteFretePedido.ValorComponente : componenteFretePedido.ValorComponente;

                    if (inserir)
                    {
                        repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);

                        cargaPedidoComponentesFrete.Add(cargaPedidoComponenteFrete);
                    }
                    else
                        repCargaPedidoComponentesFrete.Atualizar(cargaPedidoComponenteFrete);
                }
            }
        }

        private void AdicionarComponenteFretePedidoEmCarga(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarComponentesManuaisPedidoComTabelaFrete ?? false))
                return;

            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> componentesFretePedidos = repPedidoComponenteFrete.BuscarPorPedidos(cargaPedidos.Select(o => o.Pedido.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componenteFretePedido in componentesFretePedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente();

                componente.ValorComponente = (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0) ? -componenteFretePedido.ValorComponente : componenteFretePedido.ValorComponente;
                componente.ComponenteFrete = componenteFretePedido.ComponenteFrete;
                componente.TipoComponenteFrete = componenteFretePedido.TipoComponenteFrete;
                componente.Percentual = componenteFretePedido.Percentual;
                componente.DescontarValorTotalAReceber = componenteFretePedido.DescontarValorTotalAReceber;
                componente.IncluirBaseCalculoICMS = componenteFretePedido.IncluirBaseCalculoICMS;
                componente.NaoSomarValorTotalAReceber = componenteFretePedido.NaoSomarValorTotalAReceber;
                componente.DescontarDoValorAReceberValorComponente = componenteFretePedido.DescontarDoValorAReceberValorComponente;
                componente.DescontarDoValorAReceberOICMSDoComponente = componenteFretePedido.DescontarDoValorAReceberOICMSDoComponente;
                componente.ValorICMSComponenteDestacado = componenteFretePedido.ValorICMSComponenteDestacado;
                componente.NaoSomarValorTotalPrestacao = componenteFretePedido.NaoSomarValorTotalPrestacao;
                componente.DescontarComponenteFreteLiquido = componenteFretePedido.DescontarComponenteFreteLiquido;
                componente.TipoValor = componenteFretePedido.TipoValor;

                dados.Componentes.Add(componente);
            }
        }

        private decimal ObterValorTotalNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);

            decimal valorTotalDasNotas = repPedidoXMLNotaFiscal.BuscarTotalPorCarga(carga.Codigo);

            return valorTotalDasNotas;
        }

        private void AtualizarPedido(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteAplicada, bool atualizarInformacoesPagamento, string observacaoCTe, string observacaoCTeTerceiro, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento, Repositorio.UnitOfWork unitOfWork, bool possuiFronteira)
        {

            string obsTabelaFrete = string.Empty;
            string obsTerceiroTabelaFrete = string.Empty;

            if (tabelaFreteCliente.ImprimirObservacaoCTe)
            {
                obsTabelaFrete = tabelaFreteCliente.Observacao;
                obsTerceiroTabelaFrete = tabelaFreteCliente.ObservacaoTerceiro;
            }
            else if (tabelaFreteCliente.TabelaFrete.ImprimirObservacaoCTe)
            {
                obsTabelaFrete = tabelaFreteCliente.TabelaFrete.Observacao;
                obsTerceiroTabelaFrete = tabelaFreteCliente.TabelaFrete.ObservacaoTerceiro;
            }

            if (string.IsNullOrWhiteSpace(obsTabelaFrete) && string.IsNullOrWhiteSpace(obsTerceiroTabelaFrete))
            {
                if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
                {
                    obsTabelaFrete = carga.TipoOperacao.ObservacaoCTe;
                    obsTerceiroTabelaFrete = carga.TipoOperacao.ObservacaoCTeTerceiro;
                }
                else
                {
                    Dominio.Entidades.Cliente tomador = cargaPedido?.ObterTomador();

                    if (tomador != null)
                    {
                        if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        {
                            obsTabelaFrete = tomador.ObservacaoCTe;
                            obsTerceiroTabelaFrete = tomador.ObservacaoCTeTerceiro;
                        }
                        else if (tomador.GrupoPessoas != null)
                        {
                            obsTabelaFrete = tomador.GrupoPessoas.ObservacaoCTe;
                            obsTerceiroTabelaFrete = tomador.GrupoPessoas.ObservacaoCTeTerceiro;
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(obsTabelaFrete))
            {
                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                    cargaPedido.Pedido.ObservacaoCTe = obsTabelaFrete;
                else if (!cargaPedido.Pedido.ObservacaoCTe.ToLower().Contains(obsTabelaFrete.ToLower()))
                    cargaPedido.Pedido.ObservacaoCTe += " / " + obsTabelaFrete;
            }

            if (!string.IsNullOrWhiteSpace(obsTerceiroTabelaFrete))
            {
                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTeTerceiro))
                    cargaPedido.Pedido.ObservacaoCTeTerceiro = obsTerceiroTabelaFrete;
                else if (!cargaPedido.Pedido.ObservacaoCTeTerceiro.ToLower().Contains(obsTerceiroTabelaFrete.ToLower()))
                    cargaPedido.Pedido.ObservacaoCTeTerceiro += " / " + obsTerceiroTabelaFrete;
            }

            if (!carga.DadosPagamentoInformadosManualmente && atualizarInformacoesPagamento && !cargaPedido.Pedido.UsarTipoTomadorPedido && (cargaPedido.RegraTomador == null || tabelaFreteCliente.TipoPagamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao.UsarDaNotaFiscal) && !carga.EmitirCTeComplementar)
            {
                Dominio.Enumeradores.TipoPagamento tipoPagamentoPedidoPadrao = cargaPedido.Pedido.TipoPagamento;

                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                    && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                    && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                    && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                {

                    if (tabelaFreteCliente.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao.UsarDaNotaFiscal)
                    {
                        cargaPedido.Pedido.UsarTipoPagamentoNF = true;
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = (from obj in cargaPedidoModalidadesPagamento where obj.Codigo == cargaPedido.Codigo select obj.modalidadePagamentoFrete).FirstOrDefault();

                        if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
                            cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;
                        else
                        {
                            cargaPedido.Pedido.UsarTipoPagamentoNF = false;

                            if (!cargaPedido.Pedido.AdicionadaManualmente)
                                cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                            else
                                cargaPedido.Pedido.TipoPagamento = tipoPagamentoPedidoPadrao;
                        }
                    }
                    else
                    {
                        cargaPedido.RegraTomador = null;
                        cargaPedido.Pedido.UsarTipoPagamentoNF = false;
                        cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)tabelaFreteCliente.TipoPagamento;
                    }
                }
                else
                {
                    cargaPedido.Pedido.UsarTipoPagamentoNF = false;
                    cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                }

                if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros && cargaPedido.Tomador == null)
                {
                    cargaPedido.Tomador = tabelaFreteCliente.Tomador;

                    if (cargaPedido.Tomador == null)
                        cargaPedido.Pedido.TipoPagamento = (tipoPagamentoPedidoPadrao != 0 && tipoPagamentoPedidoPadrao != Dominio.Enumeradores.TipoPagamento.Outros) ? tipoPagamentoPedidoPadrao : Dominio.Enumeradores.TipoPagamento.Pago; //não encontrou o tomador no pedido e nem na tebela de frete, para não lançar exceção posteriormente seta para o que veio no pedido como default
                }

                if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
            }

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                cargaPedido.ValorFrete = Math.Round(valorFrete, 2, MidpointRounding.AwayFromZero);

            if (!tabelaFreteClienteAplicada.HerdarInclusaoICMSTabelaFrete)
            {
                if (!carga.DadosPagamentoInformadosManualmente && atualizarInformacoesPagamento)
                    cargaPedido.IncluirICMSBaseCalculo = tabelaFreteClienteAplicada.IncluirICMSValorFrete;
                cargaPedido.PercentualIncluirBaseCalculo = tabelaFreteClienteAplicada.PercentualICMSIncluir;
            }
            else
            {
                if (!carga.DadosPagamentoInformadosManualmente && atualizarInformacoesPagamento)
                    cargaPedido.IncluirICMSBaseCalculo = tabelaFreteClienteAplicada.TabelaFrete.IncluirICMSValorFrete;
                cargaPedido.PercentualIncluirBaseCalculo = tabelaFreteClienteAplicada.TabelaFrete.PercentualICMSIncluir;
            }

            if (tabelaFreteClienteAplicada.FormulaRateio != null)
            {
                cargaPedido.FormulaRateio = tabelaFreteClienteAplicada.FormulaRateio;

                if (cargaPedido.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                {
                    cargaPedido.TipoUsoFatorCubagemRateioFormula = carga.TipoOperacao?.ConfiguracaoEmissao?.TipoUsoFatorCubagemRateioFormula ?? null;
                    cargaPedido.FatorCubagemRateioFormula = carga.TipoOperacao?.ConfiguracaoEmissao?.FatorCubagemRateioFormula ?? null;
                }
            }

            if (tabelaFreteClienteAplicada.TipoRateioDocumentos != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                cargaPedido.TipoRateio = tabelaFreteClienteAplicada.TipoRateioDocumentos;

            if (!string.IsNullOrWhiteSpace(observacaoCTe))
            {
                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                    cargaPedido.Pedido.ObservacaoCTe = observacaoCTe;
                else if (!cargaPedido.Pedido.ObservacaoCTe.ToLower().Contains(observacaoCTe.ToLower()))
                    cargaPedido.Pedido.ObservacaoCTe += " / " + observacaoCTe;
            }

            if (!string.IsNullOrWhiteSpace(observacaoCTeTerceiro))
            {
                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTeTerceiro))
                    cargaPedido.Pedido.ObservacaoCTeTerceiro = observacaoCTeTerceiro;
                else if (!cargaPedido.Pedido.ObservacaoCTeTerceiro.ToLower().Contains(observacaoCTeTerceiro.ToLower()))
                    cargaPedido.Pedido.ObservacaoCTeTerceiro += " / " + observacaoCTeTerceiro;
            }

            if (!string.IsNullOrEmpty(cargaPedido.Pedido.ObservacaoCTe) && cargaPedido.Pedido.ObservacaoCTe.Length > 2000)
                cargaPedido.Pedido.ObservacaoCTe = cargaPedido.Pedido.ObservacaoCTe.Left(2000);

            if (!string.IsNullOrEmpty(cargaPedido.Pedido.ObservacaoCTeTerceiro) && cargaPedido.Pedido.ObservacaoCTeTerceiro.Length > 2000)
                cargaPedido.Pedido.ObservacaoCTeTerceiro = cargaPedido.Pedido.ObservacaoCTeTerceiro.Left(2000);

            if (!possuiFronteira && tabelaFreteCliente.Fronteira != null)
            {
                // Adicionar fronteira na carga
                Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);
                repCargaFronteira.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaFronteira
                {
                    Carga = carga,
                    Fronteira = tabelaFreteCliente.Fronteira
                });
            }

        }

        private void AtualizarPedidoFilialEmissora(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteAplicada, bool atualizarInformacoesPagamento, string observacaoCTe, string observacaoCTeTerceiro)
        {
            string obsTabelaFrete = string.Empty;
            string obsTerceiroTabelaFrete = string.Empty;

            if (tabelaFreteCliente.ImprimirObservacaoCTe)
            {
                obsTabelaFrete = tabelaFreteCliente.Observacao;
                obsTerceiroTabelaFrete = tabelaFreteCliente.ObservacaoTerceiro;
            }
            else if (tabelaFreteCliente.TabelaFrete.ImprimirObservacaoCTe)
            {
                obsTabelaFrete = tabelaFreteCliente.TabelaFrete.Observacao;
                obsTerceiroTabelaFrete = tabelaFreteCliente.TabelaFrete.ObservacaoTerceiro;
            }

            if (string.IsNullOrWhiteSpace(obsTabelaFrete) && string.IsNullOrWhiteSpace(obsTerceiroTabelaFrete))
            {
                if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
                {
                    obsTabelaFrete = carga.TipoOperacao.ObservacaoCTe;
                    obsTerceiroTabelaFrete = carga.TipoOperacao.ObservacaoCTeTerceiro;
                }
                else
                {
                    Dominio.Entidades.Cliente tomador = cargaPedido?.ObterTomador();

                    if (tomador != null)
                    {
                        if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        {
                            obsTabelaFrete = tomador.ObservacaoCTe;
                            obsTerceiroTabelaFrete = tomador.ObservacaoCTeTerceiro;
                        }
                        else if (tomador.GrupoPessoas != null)
                        {
                            obsTabelaFrete = tomador.GrupoPessoas.ObservacaoCTe;
                            obsTerceiroTabelaFrete = tomador.GrupoPessoas.ObservacaoCTeTerceiro;
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(obsTabelaFrete))
            {
                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                    cargaPedido.Pedido.ObservacaoCTe = obsTabelaFrete;
                else if (!cargaPedido.Pedido.ObservacaoCTe.ToLower().Contains(obsTabelaFrete.ToLower()))
                    cargaPedido.Pedido.ObservacaoCTe += " / " + obsTabelaFrete;
            }

            if (!string.IsNullOrWhiteSpace(obsTerceiroTabelaFrete))
            {
                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTeTerceiro))
                    cargaPedido.Pedido.ObservacaoCTeTerceiro = obsTerceiroTabelaFrete;
                else if (!cargaPedido.Pedido.ObservacaoCTeTerceiro.ToLower().Contains(obsTerceiroTabelaFrete.ToLower()))
                    cargaPedido.Pedido.ObservacaoCTeTerceiro += " / " + obsTerceiroTabelaFrete;
            }

            cargaPedido.Pedido.ObservacaoCTeTerceiro = Utilidades.String.Left(cargaPedido.Pedido.ObservacaoCTeTerceiro, 2000);
            cargaPedido.Pedido.ObservacaoCTe = Utilidades.String.Left(cargaPedido.Pedido.ObservacaoCTe, 2000);

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                cargaPedido.ValorFreteFilialEmissora = valorFrete;

            if (!tabelaFreteClienteAplicada.HerdarInclusaoICMSTabelaFrete)
            {
                if (!carga.DadosPagamentoInformadosManualmente && atualizarInformacoesPagamento)
                    cargaPedido.IncluirICMSBaseCalculoFilialEmissora = tabelaFreteClienteAplicada.IncluirICMSValorFrete;
                cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = tabelaFreteClienteAplicada.PercentualICMSIncluir;
            }
            else
            {
                if (!carga.DadosPagamentoInformadosManualmente && atualizarInformacoesPagamento)
                    cargaPedido.IncluirICMSBaseCalculoFilialEmissora = tabelaFreteClienteAplicada.TabelaFrete.IncluirICMSValorFrete;
                cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = tabelaFreteClienteAplicada.TabelaFrete.PercentualICMSIncluir;
            }
        }

        private void AtualizarPedidos(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente tabelaCarga, Repositorio.UnitOfWork unidadeTrabalho, bool atualizarInformacoesPagamento, bool calculoFreteFilialEmissora)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidoModalidadesPagamento = repPedidoXMLNotaFiscal.BuscarModalidadesDeFretePadraoCargaPedidoPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unidadeTrabalho);
            bool possuiFronteira = serCargaFronteira.TemFronteira(carga);

            bool abriuTransacao = false;
            if (!unidadeTrabalho.IsActiveTransaction())
            {
                unidadeTrabalho.Start();
                abriuTransacao = true;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!calculoFreteFilialEmissora)
                    AtualizarPedido(tabelaFreteCliente, carga, cargaPedido, cargaPedido.ValorFrete, tabelaCarga.TabelaFreteCliente, atualizarInformacoesPagamento, tabelaCarga.Observacao, tabelaCarga.ObservacaoTerceiro, cargaPedidoModalidadesPagamento, unidadeTrabalho, possuiFronteira);
                else
                    AtualizarPedidoFilialEmissora(tabelaFreteCliente, carga, cargaPedido, cargaPedido.ValorFrete, tabelaCarga.TabelaFreteCliente, atualizarInformacoesPagamento, tabelaCarga.Observacao, tabelaCarga.ObservacaoTerceiro);

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);
            }

            if (abriuTransacao)
                unidadeTrabalho.CommitChanges();
        }

        private void SalvarComponentesCargaPedido(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool componenteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho, ref List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesPorCarga, bool ultimoPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentes = CompararComponentesComparaveis(dados);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in componentes)
            {
                //Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = repCargaPedidoComponentesFrete.BuscarPorCompomente(cargaPedido.Codigo, componente.ComponenteFrete.TipoComponenteFrete, componente.ComponenteFrete, componenteFilialEmissora);
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoComponenteFrete == componente.ComponenteFrete.TipoComponenteFrete && obj.ComponenteFilialEmissora == componenteFilialEmissora && ((componente.ComponenteFrete == null && obj.ComponenteFrete == null) || (componente.ComponenteFrete != null && obj.ComponenteFrete.Codigo == componente.ComponenteFrete.Codigo)) select obj).FirstOrDefault();
                bool inserir = false;
                if (cargaPedidoComponenteFrete == null)
                {
                    cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                    inserir = true;
                }

                cargaPedidoComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                cargaPedidoComponenteFrete.ComponenteFrete = componente.ComponenteFrete;
                cargaPedidoComponenteFrete.TipoComponenteFrete = componente.TipoComponenteFrete;
                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS;
                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber;
                cargaPedidoComponenteFrete.AcrescentaValorTotalAReceber = componente.AcrescentaValorTotalAReceber;
                cargaPedidoComponenteFrete.NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber;
                cargaPedidoComponenteFrete.DescontarDoValorAReceberValorComponente = componente.DescontarDoValorAReceberValorComponente;
                cargaPedidoComponenteFrete.NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao;
                cargaPedidoComponenteFrete.ModeloDocumentoFiscal = componente.ModeloDocumentoFiscal;
                cargaPedidoComponenteFrete.RateioFormula = componente.RateioFormula;
                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaPedidoComponenteFrete.DescontarComponenteFreteLiquido = componente.DescontarComponenteFreteLiquido;
                cargaPedidoComponenteFrete.DescontarDoValorAReceberOICMSDoComponente = componente.DescontarDoValorAReceberOICMSDoComponente;
                cargaPedidoComponenteFrete.ValorICMSComponenteDestacado = componente.ValorICMSComponenteDestacado;

                if (!string.IsNullOrWhiteSpace(componente.OutraDescricaoCTe))
                    cargaPedidoComponenteFrete.OutraDescricaoCTe = componente.OutraDescricaoCTe;
                else
                    cargaPedidoComponenteFrete.OutraDescricaoCTe = "";

                cargaPedidoComponenteFrete.Percentual = componente.Percentual;
                cargaPedidoComponenteFrete.TipoValor = componente.TipoValor;

                if (componente.ComponentePorCarga)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componenteCarga = (from obj in componentesPorCarga where obj.ComponenteFrete?.Codigo == componente.ComponenteFrete?.Codigo && obj.ComponentePorCarga select obj).FirstOrDefault();

                    if (componenteCarga == null)
                    {
                        componenteCarga = componente;
                        componentesPorCarga.Add(componenteCarga);
                    }
                    else
                        componenteCarga.ValorComponente += componente.ValorComponente;

                    if (ultimoPedido)
                        componente.ValorComponente += componenteCarga.ValorComponenteParaCarga - componenteCarga.ValorComponente;
                }

                cargaPedidoComponenteFrete.ValorComponente = (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0) ? -componente.ValorComponente : componente.ValorComponente;

                if (componente.CalculoPorQuantidadeDocumentos)
                {
                    cargaPedidoComponenteFrete.PorQuantidadeDocumentos = componente.CalculoPorQuantidadeDocumentos;
                    cargaPedidoComponenteFrete.ModeloDocumentoFiscalRateio = componente.ModeloDocumentoFiscalRateio;
                    cargaPedidoComponenteFrete.TipoCalculoQuantidadeDocumentos = componente.TipoCalculoQuantidadeDocumentos;
                    cargaPedidoComponenteFrete.QuantidadeTotalDocumentos = componente.QuantidadeTotalDocumentos;
                }

                if (inserir)
                {
                    repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);
                    cargaPedidoComponentesFretes.Add(cargaPedidoComponenteFrete);
                }
                else
                    repCargaPedidoComponentesFrete.Atualizar(cargaPedidoComponenteFrete);
            }
        }

        private void SalvarComponentesCarga(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.PreCargas.PreCargaCompomenteFrete repPreCargaCompomenteFrete = new Repositorio.Embarcador.PreCargas.PreCargaCompomenteFrete(unidadeTrabalho);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dados.Componentes)
            {
                Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete preCargaComponenteFrete = new Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete
                {
                    PreCarga = preCarga,
                    ComponenteFrete = componente.ComponenteFrete,
                    TipoComponenteFrete = componente.TipoComponenteFrete,
                    IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS,
                    ComponenteFilialEmissora = calculoFreteFilialEmissora,
                    DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber,
                    AcrescentaValorTotalAReceber = componente.AcrescentaValorTotalAReceber,
                    NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber,
                    NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao,
                    SomarComponenteFreteLiquido = componente.SomarComponenteFreteLiquido,
                    DescontarComponenteFreteLiquido = componente.DescontarComponenteFreteLiquido,
                    ModeloDocumentoFiscal = componente.ModeloDocumentoFiscal,
                    RateioFormula = componente.RateioFormula,
                    Percentual = componente.Percentual,
                    TipoValor = componente.TipoValor,
                    ValorComponente = (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0) ? -componente.ValorComponente : componente.ValorComponente
                };

                if (!string.IsNullOrWhiteSpace(componente.OutraDescricaoCTe))
                    preCargaComponenteFrete.OutraDescricaoCTe = componente.OutraDescricaoCTe;
                else
                    preCargaComponenteFrete.OutraDescricaoCTe = "";

                repPreCargaCompomenteFrete.Inserir(preCargaComponenteFrete);
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> CompararComponentesComparaveis(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesComparados = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dados.Componentes)
            {
                if (componente.ComponenteComparado)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componenteComparado = (from obj in componentesComparados where obj.ComponenteComparado && obj.ComponenteFrete.Codigo == componente.ComponenteFrete.Codigo select obj).FirstOrDefault();
                    if (componenteComparado != null)
                    {
                        if (componenteComparado.ValorComponente < componente.ValorComponente)
                        {
                            componentesComparados.Remove(componenteComparado);
                            componentesComparados.Add(componente);
                        }
                    }
                    else
                        componentesComparados.Add(componente);
                }
                else
                    componentesComparados.Add(componente);
            }

            return componentesComparados;
        }

        private void SalvarComponentesCarga(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repComponenteFreteCarga = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete repCargaTabelaFreteComponenteFrete = new Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentes = CompararComponentesComparaveis(dados);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in componentes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete
                {
                    Carga = carga,
                    ComponenteFrete = componente.ComponenteFrete,
                    TipoComponenteFrete = componente.TipoComponenteFrete,
                    IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS,
                    ComponenteFilialEmissora = calculoFreteFilialEmissora,
                    DescontarValorTotalAReceber = componente.DescontarValorTotalAReceber,
                    AcrescentaValorTotalAReceber = componente.AcrescentaValorTotalAReceber,
                    NaoSomarValorTotalAReceber = componente.NaoSomarValorTotalAReceber,
                    NaoSomarValorTotalPrestacao = componente.NaoSomarValorTotalPrestacao,
                    SomarComponenteFreteLiquido = componente.SomarComponenteFreteLiquido,
                    DescontarComponenteFreteLiquido = componente.DescontarComponenteFreteLiquido,
                    ModeloDocumentoFiscal = componente.ModeloDocumentoFiscal,
                    RateioFormula = componente.RateioFormula,
                    ValorTotalMoeda = componente.ValorComponenteMoeda,
                    ValorCotacaoMoeda = dados.ValorCotacaoMoeda,
                    Moeda = dados.Moeda,
                    Percentual = componente.Percentual,
                    TipoValor = componente.TipoValor,
                    ValorComponente = (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0) ? -componente.ValorComponente : componente.ValorComponente,
                    SempreExtornar = true,
                    IncluirIntegralmenteContratoFreteTerceiro = false,
                    UtilizarFormulaRateioCarga = componente.UtilizarFormulaRateioCarga

                };

                if (!string.IsNullOrWhiteSpace(componente.OutraDescricaoCTe))
                    cargaComponenteFrete.OutraDescricaoCTe = componente.OutraDescricaoCTe;
                else
                    cargaComponenteFrete.OutraDescricaoCTe = "";

                if (componente.CalculoPorQuantidadeDocumentos)
                {
                    cargaComponenteFrete.PorQuantidadeDocumentos = componente.CalculoPorQuantidadeDocumentos;
                    cargaComponenteFrete.ModeloDocumentoFiscalRateio = componente.ModeloDocumentoFiscalRateio;
                    cargaComponenteFrete.TipoCalculoQuantidadeDocumentos = componente.TipoCalculoQuantidadeDocumentos;
                    cargaComponenteFrete.QuantidadeTotalDocumentos = componente.QuantidadeTotalDocumentos;
                }

                repComponenteFreteCarga.Inserir(cargaComponenteFrete);

                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete cargaTabelaFreteCompoenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete
                {
                    Carga = carga,
                    ComponenteFrete = componente.ComponenteFrete,
                    TipoComponenteFrete = componente.TipoComponenteFrete,
                    PercentualSobreNF = componente.Percentual
                };

                repCargaTabelaFreteComponenteFrete.Inserir(cargaTabelaFreteCompoenteFrete);
            }
        }

        private bool SelecionarTabelaFrete(ref List<TabelaFreteCliente> tabelasFreteSelecionadas, ref StringBuilder mensagem, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<TabelaFreteCliente> tabelasFrete, List<double> remetentes, List<double> destinatarios, List<int> origens, List<int> destinos, List<int> cepsOrigem, List<int> cepsDestinos, List<string> estadosOrigem, List<string> estadosDestino, List<int> regioesOrigem, bool possuiOrigemSemRegiao, List<int> regioesDestino, bool possuiDestinoSemRegiao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            tabelasFrete = tabelasFrete
                                   .OrderBy(o => o.PrioridadeUso ?? int.MaxValue)
                                   .ThenByDescending(o => o.CanalEntrega != null)
                                   .ThenByDescending(o => o.CanalVenda != null)
                                   .ThenByDescending(o => o.TiposOperacao.Count) //ordena para trazer as tabelas com tipo de operação primeiro, senão vai pegar as tabelas sem o tipo de operação e as que tem tipo de operação não vão ser escolhidas nunca
                                   .ThenByDescending(o => o.TiposCarga.Count)
                                   .ThenByDescending(o => o.Fronteiras.Count)
                                   .ThenByDescending(o => o.TransportadoresTerceiros.Count)
                                   .ThenByDescending(o => o.Empresa != null)
                                   .ThenByDescending(o => o.ClientesOrigem.Count)
                                   .ThenBy(o => o.FreteValidoParaQualquerOrigem)
                                   .ThenByDescending(o => o.CEPsOrigem.Count)
                                   .ThenByDescending(o => o.Origens.Count)
                                   .ThenByDescending(o => o.RegioesOrigem.Count)
                                   .ThenByDescending(o => o.RotasOrigem.Count)
                                   .ThenByDescending(o => o.EstadosOrigem.Count)
                                   .ThenByDescending(o => o.PaisesOrigem.Count)
                                   .ThenByDescending(o => o.ClientesDestino.Count)
                                   .ThenBy(o => o.FreteValidoParaQualquerDestino)
                                   .ThenByDescending(o => o.CEPsDestino.Count)
                                   .ThenByDescending(o => o.Destinos.Count)
                                   .ThenByDescending(o => o.RegioesDestino.Count)
                                   .ThenByDescending(o => o.RotasDestino.Count)
                                   .ThenByDescending(o => o.EstadosDestino.Count)
                                   .ThenByDescending(o => o.PaisesDestino.Count)
                                   .ThenBy(o => o.Vigencia.DataFinal - o.Vigencia.DataInicial)
                                   .ToList();


            if (parametros.Rota != null)
                tabelasFrete = tabelasFrete.OrderByDescending(obj => obj.TabelaFrete != null && obj.TabelaFrete.RotasFreteEmbarcador != null && obj.TabelaFrete.RotasFreteEmbarcador.Count > 0 && obj.TabelaFrete.RotasFreteEmbarcador.Any(rota => rota.RotaFrete.Codigo == parametros.Rota.Codigo)).ToList();

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete in tabelasFrete)
            {
                if (ValidarTabelaFrete(tabelaFrete, ref mensagem, parametros, tabelasFrete, remetentes, destinatarios, origens, destinos, cepsOrigem, cepsDestinos, estadosOrigem, estadosDestino, regioesOrigem, possuiOrigemSemRegiao, regioesDestino, possuiDestinoSemRegiao, unitOfWork, tipoServicoMultisoftware))
                {
                    tabelasFreteSelecionadas.Add(tabelaFrete);
                    if (!parametros.NaoValidarTransportador)
                        return true;
                }
            }

            if (tabelasFreteSelecionadas.Count > 0 && parametros.NaoValidarTransportador)
                return true;


            return false;
        }

        private bool ValidarTabelaFrete(TabelaFreteCliente tabelaFrete, ref StringBuilder mensagem, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<TabelaFreteCliente> tabelasFrete, List<double> cpfCnpjRemetentes, List<double> cpfCnpjDestinatarios, List<int> origens, List<int> destinos, List<int> cepsOrigem, List<int> cepsDestino, List<string> estadosOrigem, List<string> estadosDestino, List<int> regioesOrigem, bool possuiOrigemSemRegiao, List<int> regioesDestino, bool possuiDestinoSemRegiao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool valido = true;
            string msg = string.Empty;

            if (!(tabelaFrete.Vigencia.DataInicial <= parametros.DataVigencia.Date && (!tabelaFrete.Vigencia.DataFinal.HasValue || tabelaFrete.Vigencia.DataFinal >= parametros.DataVigencia.Date)))
            {
                valido = false;
                msg = "a vigência da tabela de frete (" + tabelaFrete.DescricaoVigencia + ") não coincide com a data de criação da carga (" + parametros.DataVigencia.ToString("dd/MM/yyyy") + ".";
            }
            else if (tabelaFrete.ClientesOrigem != null && tabelaFrete.ClientesOrigem.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerOrigem && (cpfCnpjRemetentes.Count != tabelaFrete.ClientesOrigem.Count || !cpfCnpjRemetentes.All(cpfCnpj => tabelaFrete.ClientesOrigem.Any(cliente => cliente.CPF_CNPJ == cpfCnpj)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerOrigem && !cpfCnpjRemetentes.All(cpfCnpj => tabelaFrete.ClientesOrigem.Any(cliente => cliente.CPF_CNPJ == cpfCnpj))))))
            {
                valido = false;
                msg = "o(s) remetente(s) da tabela de frete difere(m) do(s) remetente(s) da carga.";
            }
            else if (tabelaFrete.CEPsOrigem != null && tabelaFrete.CEPsOrigem.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerOrigem && (cepsOrigem.Count != tabelaFrete.CEPsOrigem.Count || !cepsOrigem.All(cep => tabelaFrete.CEPsOrigem.Any(faixaCEP => faixaCEP.CEPInicial <= cep && faixaCEP.CEPFinal >= cep)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerOrigem && !cepsOrigem.All(cep => tabelaFrete.CEPsOrigem.Any(faixaCEP => faixaCEP.CEPInicial <= cep && faixaCEP.CEPFinal >= cep))))))
            {
                valido = false;
                msg = "o(s) CEP(s) de origem da tabela de frete difere(m) do(s) CEP(s) do(s) remetente(s) da carga.";
            }
            else if (tabelaFrete.Origens != null && tabelaFrete.Origens.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerOrigem && (origens.Count != tabelaFrete.Origens.Count || !origens.All(origem => tabelaFrete.Origens.Any(locOrigem => locOrigem.Codigo == origem)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerOrigem && !origens.All(origem => tabelaFrete.Origens.Any(locOrigem => locOrigem.Codigo == origem))))))
            {
                valido = false;
                msg = "a(s) localidade(s) de origem da tabela de frete difere(m) da(s) localidade(s) de origem da carga.";
            }
            else if (tabelaFrete.EstadosOrigem != null && tabelaFrete.EstadosOrigem.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerOrigem && (estadosOrigem.Count != tabelaFrete.EstadosOrigem.Count || !estadosOrigem.All(estado => tabelaFrete.EstadosOrigem.Any(est => est.Sigla == estado)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerOrigem && !estadosOrigem.All(estado => tabelaFrete.EstadosOrigem.Any(est => est.Sigla == estado))))))
            {
                valido = false;
                msg = "o(s) estado(s) de origem da tabela de frete difere(m) do(s) estado(s) de origem da carga.";
            }
            else if (tabelaFrete.RegioesOrigem != null && tabelaFrete.RegioesOrigem.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerOrigem && (possuiOrigemSemRegiao || regioesOrigem.Count != tabelaFrete.RegioesOrigem.Count || !regioesOrigem.All(regiao => tabelaFrete.RegioesOrigem.Any(reg => reg.Codigo == regiao)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerOrigem && (possuiDestinoSemRegiao || !regioesOrigem.All(regiao => tabelaFrete.RegioesOrigem.Any(reg => reg.Codigo == regiao)))))))
            {
                valido = false;
                msg = "a(s) região(ões) de origem da tabela de frete difere(m) da(s) região(ões) de origem da carga.";
            }
            else if (tabelaFrete.ClientesDestino != null && tabelaFrete.ClientesDestino.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerDestino && (cpfCnpjDestinatarios.Count != tabelaFrete.ClientesDestino.Count || !cpfCnpjDestinatarios.All(cpfCnpj => tabelaFrete.ClientesDestino.Any(cliente => cliente.CPF_CNPJ == cpfCnpj)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerDestino && !cpfCnpjDestinatarios.All(cpfCnpj => tabelaFrete.ClientesDestino.Any(cliente => cliente.CPF_CNPJ == cpfCnpj))))))
            {
                valido = false;
                msg = "o(s) destinatário(s) da tabela de frete difere(m) do(s) destinatário(s) da carga.";
            }
            else if (tabelaFrete.CEPsDestino != null && tabelaFrete.CEPsDestino.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerDestino && (cepsDestino.Count != tabelaFrete.CEPsDestino.Count || !cepsDestino.All(cep => tabelaFrete.CEPsDestino.Any(faixaCEP => faixaCEP.CEPInicial <= cep && faixaCEP.CEPFinal >= cep)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerDestino && !cepsDestino.All(cep => tabelaFrete.CEPsDestino.Any(faixaCEP => faixaCEP.CEPInicial <= cep && faixaCEP.CEPFinal >= cep))))))
            {
                valido = false;
                msg = "o(s) CEP(s) de destino da tabela de frete difere(m) do(s) CEP(s) do(s) destinatário(s) da carga.";
            }
            else if (tabelaFrete.Destinos != null && tabelaFrete.Destinos.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerDestino && (destinos.Count != tabelaFrete.Destinos.Count || !destinos.All(destino => tabelaFrete.Destinos.Any(locDestino => locDestino.Codigo == destino)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerDestino && !destinos.All(destino => tabelaFrete.Destinos.Any(locDestino => locDestino.Codigo == destino))))))
            {
                valido = false;
                msg = "a(s) localidade(s) de destino da tabela de frete difere(m) da(s) localidade(s) de destino da carga.";
            }
            else if (tabelaFrete.EstadosDestino != null && tabelaFrete.EstadosDestino.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerDestino && (estadosDestino.Count != tabelaFrete.EstadosDestino.Count || !estadosDestino.All(estado => tabelaFrete.EstadosDestino.Any(est => est.Sigla == estado)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerDestino && !estadosDestino.All(estado => tabelaFrete.EstadosDestino.Any(est => est.Sigla == estado))))))
            {
                valido = false;
                msg = "o(s) estado(s) de destino da tabela de frete difere(m) do(s) estado(s) de destino da carga.";
            }
            else if (tabelaFrete.RegioesDestino != null && tabelaFrete.RegioesDestino.Count > 0 &&
                    ((!tabelaFrete.FreteValidoParaQualquerDestino && (possuiDestinoSemRegiao || regioesDestino.Count != tabelaFrete.RegioesDestino.Count || !regioesDestino.All(regiao => tabelaFrete.RegioesDestino.Any(reg => reg.Codigo == regiao)))) ||
                    ((tabelaFrete.FreteValidoParaQualquerDestino && (possuiDestinoSemRegiao || !regioesDestino.All(regiao => tabelaFrete.RegioesDestino.Any(reg => reg.Codigo == regiao)))))))
            {
                valido = false;
                msg = "a(s) região(ões) de destino da tabela de frete difere(m) da(s) região(ões) de destino da carga.";
            }
            else if (!parametros.NaoValidarTransportador && (tabelaFrete.Empresa != null && ((parametros.Empresa != null && tabelaFrete.Empresa.Codigo != parametros.Empresa.Codigo) || (tabelaFrete.Empresa != null && parametros.Empresa == null))))
            {
                valido = false;
                msg = "a transportadora da carga não possui uma tabela de frete configurada.";
            }
            else if (tabelaFrete.TiposOperacao != null && tabelaFrete.TiposOperacao.Count > 0 && (parametros.TipoOperacao == null || !tabelaFrete.TiposOperacao.Any(o => o.Codigo == parametros.TipoOperacao.Codigo)))
            {
                msg = $"o tipo de operação da tabela de frete ({string.Join(", ", tabelaFrete.TiposOperacao.Select(o => o.Descricao))}) difere do tipo de operação da carga ({(parametros.TipoOperacao?.Descricao ?? "nenhum valor encontrado")}).";
                valido = false;
            }
            else if (tabelaFrete.TiposCarga != null && tabelaFrete.TiposCarga.Count > 0 && (parametros.TipoCarga == null || !tabelaFrete.TiposCarga.Any(o => o.Codigo == parametros.TipoCarga.Codigo)))
            {
                msg = $"o tipo de carga da tabela de frete ({string.Join(", ", tabelaFrete.TiposCarga.Select(o => o.Descricao))}) difere do tipo de carga da carga ({(parametros.TipoCarga?.Descricao ?? "nenhum valor encontrado")}).";
                valido = false;
            }
            else if (tabelaFrete.TransportadoresTerceiros != null && tabelaFrete.TransportadoresTerceiros.Count > 0 && (parametros.TransportadorTerceiro == null || !tabelaFrete.TransportadoresTerceiros.Any(o => o.CPF_CNPJ == parametros.TransportadorTerceiro.CPF_CNPJ)))
            {
                msg = $"os transportadores terceiros da tabela de frete diferem do transportador terceiro da carga ({(parametros.TransportadorTerceiro?.Descricao ?? "nenhum valor encontrado")}).";
                valido = false;
            }
            else if (tabelaFrete.Fronteiras != null && tabelaFrete.Fronteiras.Count > 0 && (parametros.Fronteiras == null || parametros.Fronteiras.Count == 0 || !parametros.Fronteiras.All(o => tabelaFrete.Fronteiras.Any(f => f.Codigo == o.Codigo))))
            {
                msg = $"as fronteiras da tabela de frete ({string.Join(",", tabelaFrete.Fronteiras.Select(o => o.Descricao))}) diferem das fronteiras da carga ({(parametros.Fronteiras != null ? (string.Join(",", parametros.Fronteiras.Select(o => o.Descricao))) : "")}).";
                valido = false;
            }
            else if (!ValidarParametrosTabelaFrete(tabelaFrete, parametros, ref msg, unitOfWork, tipoServicoMultisoftware))
            {
                valido = false;
            }
            else if (!tabelaFrete.TabelaFrete.NaoUsarCanalEntregaComoFiltroParaCotacao && tabelaFrete.CanalEntrega != null && (parametros.CanalEntrega == null || parametros.CanalEntrega.Codigo != tabelaFrete.CanalEntrega.Codigo))
            {
                msg = $"O Canal de Entrega da Carga não foi localizado em uma tabela de frete.";
                valido = false;
            }
            else if (tabelaFrete.CanalVenda != null && (parametros.CanalVenda == null || parametros.CanalVenda.Codigo != tabelaFrete.CanalVenda.Codigo))
            {
                msg = $"O Canal de venda da Carga não foi localizado em uma tabela de frete.";
                valido = false;
            }

            if (!valido)
            {
                string mensagemNova = "A tabela de frete '" + tabelaFrete.TabelaFrete.Descricao + "' com origem de '" + tabelaFrete.DescricaoOrigem + "' e destino de '" + tabelaFrete.DescricaoDestino + "' não foi selecionada pois " + msg;

                if (!mensagem.ToString().Contains(mensagemNova))
                    mensagem.Append(mensagemNova).AppendLine();
            }

            return valido;
        }

        private bool ValidarParametrosTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (parametros.CalcularVariacoes)
                return true;

            if (!ValidarTracoesTabelaFrete(parametros.Veiculo, tabelaFrete, ref mensagem))
                return false;

            if (!ValidarReboquesTabelaFrete(parametros.ModeloVeiculo, tabelaFrete, ref mensagem))
                return false;

            if (!ValidarTiposCargaTabelaFrete(parametros.TipoCarga, tabelaFrete, ref mensagem))
                return false;

            if (!parametros.NaoValidarTransportador)
            {
                if (!ValidarValorFreteTabelaFrete(tabelaFrete, parametros, unitOfWork, tipoServicoMultisoftware, ref mensagem))
                    return false;
            }

            return true;
        }

        public void CriarTabelaFreteCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool apenasVerificar, bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!apenasVerificar)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);

                decimal valorComponentesOcorrencia = repCargaComponenteFrete.BuscarValorCompomentePorTipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia, calculoFreteFilialEmissora);

                if (!calculoFreteFilialEmissora)
                {
                    carga.ValorICMS = Math.Round(carga.ValorICMS, 2, MidpointRounding.AwayFromZero);
                    carga.ValorISS = Math.Round(carga.ValorISS, 2, MidpointRounding.AwayFromZero);
                    carga.ValorRetencaoISS = Math.Round(carga.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFreteAPagar = Math.Round(carga.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFrete = Math.Round(carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFreteTabelaFrete = Math.Round(carga.ValorFreteAPagar - valorComponentesOcorrencia, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFreteLiquido = Math.Round(carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    carga.ValorICMSFilialEmissora = Math.Round(carga.ValorICMSFilialEmissora, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFreteAPagarFilialEmissora = Math.Round(carga.ValorFreteAPagarFilialEmissora, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFreteFilialEmissora = Math.Round(carga.ValorFreteFilialEmissora, 2, MidpointRounding.AwayFromZero);
                    carga.ValorFreteTabelaFreteFilialEmissora = Math.Round(carga.ValorFreteAPagarFilialEmissora - valorComponentesOcorrencia, 2, MidpointRounding.AwayFromZero);
                }


                repCarga.Atualizar(carga);
            }

            carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
            carga.MotivoPendencia = "";
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete ObterDadosTabelaFreteCliente(Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente freteCliente = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente();
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = cargaTabelaFreteCliente?.TabelaFreteCliente;

            if (carga.ConfiguracaoTabelaFretePorPedido && (carga.TabelaFrete == null || carga.TabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorDocumentoEmitido))
            {
                freteCliente.Tabela = "Configuração utilizada por pedido";
            }
            else if (tabelaFreteCliente != null)
            {
                if (!string.IsNullOrWhiteSpace(tabelaFreteCliente?.CodigoIntegracao))
                    freteCliente.Tabela = tabelaFreteCliente.CodigoIntegracao + " - ";

                freteCliente.Tabela += tabelaFreteCliente.TabelaFrete?.Descricao ?? string.Empty;

                if ((tabelaFreteCliente.TabelaFrete?.ContratoFreteTransportador ?? null) != null)
                {
                    freteCliente.ContratoFrete = "nº " + tabelaFreteCliente.TabelaFrete.ContratoFreteTransportador.Numero;
                    freteCliente.ContratoFrete += " - " + tabelaFreteCliente.TabelaFrete.ContratoFreteTransportador.Descricao;
                }
                else
                    freteCliente.ContratoFrete = "";

                freteCliente.PermiteAlterarValorFrete = tabelaFreteCliente.TabelaFrete.PermiteAlterarValor;
                freteCliente.Origem = tabelaFreteCliente.DescricaoOrigem;
                freteCliente.Destino = tabelaFreteCliente.DescricaoDestino;
            }

            if (contratoFreteTransportador != null)
            {
                freteCliente.ContratoFrete = "nº " + contratoFreteTransportador.Numero;
                freteCliente.ContratoFrete += " - " + contratoFreteTransportador.Descricao;
            }

            freteCliente.TipoCalculo = carga.TabelaFrete?.TipoCalculo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga;

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
            retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
            retorno.valorFrete = carga.ValorFrete;

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete;
            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, carga.ContratoFreteTransportador?.ComponenteFreteValorContrato);
            bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.DescontarComponenteFreteLiquido) ?? false;

            if (carga.TabelaFrete == null && carga.ContratoFreteTransportador != null)
                retorno.ValorFreteLiquido = carga.ValorFreteContratoFreteTotal;
            else
                retorno.ValorFreteLiquido = carga.ValorFreteLiquido + (((carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);

            retorno.valorFreteAPagar = carga.ValorFreteAPagar;
            retorno.ValorFreteNegociado = carga.ValorFreteNegociado;
            retorno.valorFreteTabelaFrete = carga.ValorFreteTabelaFrete;
            retorno.valorFreteOperador = carga.ValorFreteOperador;
            retorno.valorFreteLeilao = carga.ValorFreteLeilao;
            retorno.valorFreteEmbarcador = carga.ValorFreteEmbarcador;
            retorno.valorICMS = carga.ValorICMS;
            retorno.aliquotaICMS = repCargaPedido.BuscarMediaAliquotaICMSdaCarga(carga.Codigo);
            retorno.csts = repCargaPedido.BuscarCSTICMSdaCarga(carga.Codigo);
            retorno.taxaDocumentacao = Servicos.Embarcador.Carga.Frete.RetornarTaxaDocumental(carga);
            retorno.aliquotaISS = repCargaPedido.BuscarMediaAliquotaISSdaCarga(carga.Codigo);
            retorno.valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
            retorno.peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
            retorno.Moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
            retorno.ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
            retorno.ValorTotalMoeda = carga.ValorTotalMoeda ?? 0m;
            retorno.ValorTotalMoedaPagar = carga.ValorTotalMoedaPagar ?? 0m;
            retorno.PercentualBonificacaoTransportador = carga.PercentualBonificacaoTransportador;
            retorno.CustoFrete = carga.DadosSumarizados?.CustoFrete ?? string.Empty;
            retorno.DescricaoBonificacaoTransportador = (carga.PercentualBonificacaoTransportador != 0m) ? carga.BonificacaoTransportador?.ComponenteFrete?.Descricao ?? string.Empty : string.Empty;

            if (tabelaFreteCliente != null)
            {
                bool usarVigenciaTabelaCliente = false;

                if (tabelaFreteCliente.TabelaFrete != null && tabelaFreteCliente.TabelaFrete.UsarComoDataBaseVigenciaDataAtual && tabelaFreteCliente.Vigencia != null)
                    usarVigenciaTabelaCliente = true;

                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigenciasTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(_unitOfWork);
                VigenciaTabelaFrete vigenciaTabelaFrete = repositorioVigenciasTabelaFrete.BuscarVigenciaPorData(usarVigenciaTabelaCliente ? tabelaFreteCliente.Vigencia.DataInicial : (carga.DataInicioCalculoFrete.HasValue ? carga.DataInicioCalculoFrete.Value : carga.DataCriacaoCarga), tabelaFreteCliente.TabelaFrete.Codigo);

                if (vigenciaTabelaFrete != null)
                {
                    StringBuilder mensagemVigencia = new StringBuilder();
                    mensagemVigencia.Append("De ", vigenciaTabelaFrete.DataFinal.HasValue);
                    mensagemVigencia.Append(vigenciaTabelaFrete.DataInicial.ToString("dd/MM/yyyy"));
                    mensagemVigencia.Append($" Até {vigenciaTabelaFrete.DataFinal?.ToString("dd/MM/yyyy")}", vigenciaTabelaFrete.DataFinal.HasValue);

                    retorno.DataVigenciaTabelaFrete = mensagemVigencia.ToString();
                }
            }

            if (tabelaFreteCliente?.TabelaFrete?.PermiteInformarDiasUteisPorFaixaCEP ?? false)
                retorno.CEPDestinoDiasUteis = tabelaFreteCliente.CEPsDestino?.FirstOrDefault()?.DiasUteis ?? 0;

            ComponetesFrete serComponentesFrete = new ComponetesFrete(_unitOfWork);
            serComponentesFrete.BuscarComponentesDeFreteDaCarga(ref retorno, carga, calculoFreteFilialEmissora, _unitOfWork, tipoServicoMultisoftware);

            retorno.dadosRetornoTipoFrete = freteCliente;

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete ObterDadosTabelaFreteClientePorPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool apenasVerificar = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repositorioCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente freteCliente = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete;
            if (calculoFreteFilialEmissora)
                tabelaFrete = carga.TabelaFreteFilialEmissora;

            freteCliente.Tabela = tabelaFrete?.Descricao ?? string.Empty;
            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = repositorioCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

            if ((tabelaFrete?.ContratoFreteTransportador ?? null) != null)
            {
                freteCliente.ContratoFrete = "nº " + tabelaFrete.ContratoFreteTransportador.Numero;
                freteCliente.ContratoFrete += " - " + tabelaFrete.ContratoFreteTransportador.Descricao;
            }
            else
                freteCliente.ContratoFrete = "";

            freteCliente.PermiteAlterarValorFrete = tabelaFrete?.PermiteAlterarValor ?? false;
            freteCliente.Origem = "";
            freteCliente.Destino = "";
            freteCliente.TipoCalculo = tabelaFrete?.TipoCalculo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido;

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
            retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;

            if (apenasVerificar && carga.PossuiPendencia && carga.MotivoPendenciaFrete == MotivoPendenciaFrete.ProblemaCalculoFrete && !string.IsNullOrWhiteSpace(carga.MotivoPendencia))
            {
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retorno.mensagem = carga.MotivoPendencia;
            }
            else
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

            if (!calculoFreteFilialEmissora)
            {

                bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, carga.ContratoFreteTransportador?.ComponenteFreteValorContrato);
                bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.DescontarComponenteFreteLiquido) ?? false;

                retorno.valorFrete = carga.ValorFrete;
                retorno.ValorFreteLiquido = carga.ValorFreteLiquido + (((carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);
                retorno.valorFreteAPagar = carga.ValorFreteAPagar;
                retorno.ValorFreteNegociado = carga.ValorFreteNegociado;
                retorno.valorFreteTabelaFrete = carga.ValorFreteTabelaFrete;
                retorno.valorFreteOperador = carga.ValorFreteOperador;
                retorno.valorFreteLeilao = carga.ValorFreteLeilao;
                retorno.valorFreteEmbarcador = carga.ValorFreteEmbarcador;
                retorno.valorICMS = carga.ValorICMS;
                retorno.aliquotaICMS = repCargaPedido.BuscarMediaAliquotaICMSdaCarga(carga.Codigo);
                retorno.csts = repCargaPedido.BuscarCSTICMSdaCarga(carga.Codigo);
                retorno.taxaDocumentacao = Servicos.Embarcador.Carga.Frete.RetornarTaxaDocumental(carga);
                retorno.aliquotaISS = repCargaPedido.BuscarMediaAliquotaISSdaCarga(carga.Codigo);
                retorno.valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
                retorno.peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
                retorno.Moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
                retorno.ValorTotalMoeda = carga.ValorTotalMoeda ?? 0m;
                retorno.ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
                retorno.ValorTotalMoedaPagar = carga.ValorTotalMoedaPagar ?? 0m;
                retorno.CustoFrete = carga.DadosSumarizados?.CustoFrete ?? string.Empty;
                retorno.PercentualBonificacaoTransportador = carga.PercentualBonificacaoTransportador;
                retorno.DescricaoBonificacaoTransportador = (carga.PercentualBonificacaoTransportador != 0m) ? carga.BonificacaoTransportador?.ComponenteFrete?.Descricao ?? string.Empty : string.Empty;
            }
            else
            {
                retorno.valorFrete = carga.ValorFreteFilialEmissora;
                retorno.valorFreteAPagar = carga.ValorFreteAPagarFilialEmissora;
                retorno.valorFreteTabelaFrete = carga.ValorFreteTabelaFreteFilialEmissora;
                retorno.valorICMS = carga.ValorICMSFilialEmissora;
            }

            if (cargaTabelaFreteCliente?.TabelaFreteCliente != null)
            {
                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigenciasTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
                VigenciaTabelaFrete vigenciaTabelaFrete = repositorioVigenciasTabelaFrete.BuscarPorTabelaFreteCliente(cargaTabelaFreteCliente.TabelaFreteCliente.Codigo);

                if (vigenciaTabelaFrete != null)
                {
                    StringBuilder mensagemVigencia = new StringBuilder();
                    mensagemVigencia.Append("De ", vigenciaTabelaFrete.DataFinal.HasValue);
                    mensagemVigencia.Append(vigenciaTabelaFrete.DataInicial.ToString("dd/MM/yyyy"));
                    mensagemVigencia.Append($" Até {vigenciaTabelaFrete.DataFinal?.ToString("dd/MM/yyyy")}", vigenciaTabelaFrete.DataFinal.HasValue);

                    retorno.DataVigenciaTabelaFrete = mensagemVigencia.ToString();
                }
            }
            else if (tabelaFrete != null)
            {
                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigenciasTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
                VigenciaTabelaFrete vigenciaTabelaFrete = repositorioVigenciasTabelaFrete.BuscarVigenciaPorData(carga.DataInicioCalculoFrete.HasValue ? carga.DataInicioCalculoFrete.Value : carga.DataCriacaoCarga, tabelaFrete.Codigo);

                if (vigenciaTabelaFrete != null)
                {
                    StringBuilder mensagemVigencia = new StringBuilder();
                    mensagemVigencia.Append("De ", vigenciaTabelaFrete.DataFinal.HasValue);
                    mensagemVigencia.Append(vigenciaTabelaFrete.DataInicial.ToString("dd/MM/yyyy"));
                    mensagemVigencia.Append($" Até {vigenciaTabelaFrete.DataFinal?.ToString("dd/MM/yyyy")}", vigenciaTabelaFrete.DataFinal.HasValue);

                    retorno.DataVigenciaTabelaFrete = mensagemVigencia.ToString();
                }
            }

            ComponetesFrete serComponentesFrete = new ComponetesFrete(unitOfWork);
            serComponentesFrete.BuscarComponentesDeFreteDaCarga(ref retorno, carga, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);

            retorno.dadosRetornoTipoFrete = freteCliente;


            return retorno;
        }

        public void RecalculaImpostoValorFreteManualPedidoNaoAgrupados(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaFreteCliente,
             bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
             Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
             List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes,
             List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo,
             List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos,
             List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas,
             List<Dominio.Entidades.Cliente> tomadoresFilialEmissora,
             Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga
             )
        {
            Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(this._unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                if (cargaPedido.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                    && cargaPedido.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado
                    && !cargaPedido.IndicadorCTeGlobalizadoDestinatario)
                {
                    serFreteRateio.CalcularImpostos(ref carga, cargaOrigem, cargaPedido, !calculoFreteFilialEmissora ? cargaPedido.ValorFrete : cargaPedido.ValorFreteFilialEmissora, calculoFreteFilialEmissora, tipoServicoMultisoftware, _unitOfWork, configuracao, cargaPedidoComponentesFretes, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);

                    if (!calculoFreteFilialEmissora)
                        cargaPedido.ValorFreteTabelaFrete = cargaPedido.ValorFreteAPagar;
                    else
                        cargaPedido.ValorFreteTabelaFreteFilialEmissora = cargaPedido.ValorFreteAPagar;
                }

            }
            if (!calculoFreteFilialEmissora)
                serFreteRateio.InformarDadosContabeisCargaPedido(cargaPedido, cargaOrigem, configuracao, tipoServicoMultisoftware, _unitOfWork);
        }

        private void SalvarFreteCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cargaPedidoTabelaFreteCliente, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, decimal valorTotalDasNotas, bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(_unitOfWork);
            if (!apenasVerificar)
            {
                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    if (!calculoFreteFilialEmissora)
                    {
                        cargaPedido.ValorFrete = cargaPedidoTabelaFreteCliente.ValorFrete;
                        cargaPedido.ValorFreteAPagar = cargaPedidoTabelaFreteCliente.ValorFrete;
                    }
                    else
                    {
                        cargaPedido.ValorFreteFilialEmissora = cargaPedidoTabelaFreteCliente.ValorFrete;
                        cargaPedido.ValorFreteAPagarFilialEmissora = cargaPedidoTabelaFreteCliente.ValorFrete;
                    }
                }
                if (!calculoFreteFilialEmissora)
                    cargaPedido.ValorFreteTabelaFrete = cargaPedidoTabelaFreteCliente.ValorFrete;
                else
                    cargaPedido.ValorFreteTabelaFreteFilialEmissora = cargaPedidoTabelaFreteCliente.ValorFrete;

                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    if (cargaPedido.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                        && cargaPedido.TipoRateio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado
                        && !cargaPedido.IndicadorCTeGlobalizadoDestinatario)
                    {
                        serFreteRateio.CalcularImpostos(ref carga, cargaOrigem, cargaPedido, !calculoFreteFilialEmissora ? cargaPedido.ValorFrete : cargaPedido.ValorFreteFilialEmissora, calculoFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidoComponentesFretes, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);

                        if (!calculoFreteFilialEmissora)
                            cargaPedido.ValorFreteTabelaFrete = cargaPedido.ValorFreteAPagar;
                        else
                            cargaPedido.ValorFreteTabelaFreteFilialEmissora = cargaPedido.ValorFreteAPagar;
                    }

                }

                if (!calculoFreteFilialEmissora)
                    serFreteRateio.InformarDadosContabeisCargaPedido(cargaPedido, cargaOrigem, configuracao, tipoServicoMultisoftware, unitOfWork);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFrete(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, ref Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool calculoFreteFilialEmissora, decimal valorTotalDasNotas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            try
            {
                if (!apenasVerificar)
                {
                    if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    {
                        if (!calculoFreteFilialEmissora)
                        {
                            carga.ValorFrete = cargaTabelaFreteCliente.ValorFrete;
                            carga.ValorFreteAPagar = cargaTabelaFreteCliente.ValorFrete;
                            carga.ValorFreteTabelaFrete = cargaTabelaFreteCliente.ValorFrete;
                            carga.Moeda = cargaTabelaFreteCliente.Moeda;
                            carga.ValorCotacaoMoeda = cargaTabelaFreteCliente.ValorCotacaoMoeda;
                            carga.ValorTotalMoeda = cargaTabelaFreteCliente.ValorTotalMoeda;
                            carga.ValorTotalMoedaPagar = cargaTabelaFreteCliente.ValorTotalMoeda;
                        }
                        else
                        {
                            carga.ValorFreteFilialEmissora = cargaTabelaFreteCliente.ValorFrete;
                            carga.ValorFreteAPagarFilialEmissora = cargaTabelaFreteCliente.ValorFrete;
                            carga.ValorFreteTabelaFreteFilialEmissora = cargaTabelaFreteCliente.ValorFrete;
                        }
                    }
                    else
                    {
                        if (!calculoFreteFilialEmissora)
                            carga.ValorFreteTabelaFrete = cargaTabelaFreteCliente.ValorFrete + dados.Componentes.Sum(o => o.ValorComponente);
                        else
                            carga.ValorFreteTabelaFreteFilialEmissora = cargaTabelaFreteCliente.ValorFrete + dados.Componentes.Sum(o => o.ValorComponente);
                    }

                    repCarga.Atualizar(carga);
                }

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                {
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.MotivoPendencia = "";

                    if (!apenasVerificar)
                    {
                        decimal valorComponentesOcorrencia = repCargaComponenteFrete.BuscarValorCompomentePorTipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia, calculoFreteFilialEmissora);

                        Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(_unitOfWork);

                        serFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);

                        if (!calculoFreteFilialEmissora)
                            carga.ValorFreteTabelaFrete = carga.ValorFreteAPagar - valorComponentesOcorrencia;
                        else
                            carga.ValorFreteTabelaFreteFilialEmissora = carga.ValorFreteAPagarFilialEmissora - valorComponentesOcorrencia;
                    }
                }
                else
                {
                    if (!repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo) && carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

                        RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(_unitOfWork);
                        serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, calculoFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, configuracao);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = null;

                if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                    retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido };
                else
                    retorno = ObterDadosTabelaFreteCliente(cargaTabelaFreteCliente, null, carga, calculoFreteFilialEmissora, tipoServicoMultisoftware);

                return retorno;
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException ex)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { };
                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retorno.mensagem = ex.Message.ToString();

                carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                carga.PossuiPendencia = true;
                carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

                return retorno;
            }
        }

        public void SetarValoresTabelaFreteSemParametroBase(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            SetarObservacaoTabelaFreteCarga(ref dados, tabelaFreteCliente, null);

            SetarValoresTabelaFretePorItensParametroBase(ref dados, parametros, tabelaFreteCliente.ItensBaseCalculo, tabelaFreteCliente, _unitOfWork, tipoServicoMultisoftware, tabelaFreteCliente.ValorMinimoGarantido, tabelaFreteCliente.ValorMaximo, tabelaFreteCliente.ValorBase, configuracaoTMS);

            dados.PercentualPagamentoAgregado = tabelaFreteCliente.PercentualPagamentoAgregado;
            dados.LeadTime = tabelaFreteCliente.LeadTime;

            Servicos.Embarcador.Carga.Frete.AjustarValorDoFreteDescontandoComponenteFreteLiquido(dados);
        }

        public void SetarValoresTabelaFreteComParametroBase(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametrosParaCalculo = ObterParametroBaseTabelaFrete(tabelaFreteCliente, parametros);

            dados.ComposicoesVariacao = new Dictionary<ParametroBaseCalculoTabelaFrete, List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>>();
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicoesAdicionadas = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

            foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in parametrosParaCalculo)
            {
                SetarObservacaoTabelaFreteCarga(ref dados, tabelaFreteCliente, parametro);

                SetarValoresTabelaFretePorItensParametroBase(ref dados, parametros, parametro.ItensBaseCalculo, tabelaFreteCliente, _unitOfWork, tipoServicoMultisoftware, parametro.ValorMinimoGarantido, parametro.ValorMaximo, parametro.ValorBase, configuracaoTMS);

                dados.PercentualPagamentoAgregado = parametro.PercentualPagamentoAgregado;
                dados.LeadTime = tabelaFreteCliente.LeadTime;

                dados.ComposicoesVariacao.Add(parametro, dados.ComposicaoFrete.Except(composicoesAdicionadas).ToList());

                if (dados.ComposicaoFrete != null)
                    composicoesAdicionadas.AddRange(dados.ComposicaoFrete);
            }
            Servicos.Embarcador.Carga.Frete.AjustarValorDoFreteDescontandoComponenteFreteLiquido(dados);
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> ObterItensParaCalculoPorTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros)
        {
            if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
            {
                List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametrosParaCalculo = ObterParametroBaseTabelaFrete(tabelaFreteCliente, parametros);

                return parametrosParaCalculo.FirstOrDefault()?.ItensBaseCalculo.ToList() ?? new List<ItemParametroBaseCalculoTabelaFrete>();
            }
            else
            {
                return tabelaFreteCliente.ItensBaseCalculo.ToList();
            }
        }

        private void ConverterValoresCotacaoMoeda(Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            dados.Moeda = tabelaFreteCliente.Moeda;
            dados.UtilizaMoedaEstrangeira = configuracaoTMS.UtilizaMoedaEstrangeira;

            if (configuracaoTMS.UtilizaMoedaEstrangeira)
            {
                if (tabelaFreteCliente.Moeda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    dados.UtilizaMoedaEstrangeira = false; //se é Real não precisa de conversão
                else if (parametros.DataBaseCRT.HasValue)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional tipoCotacao = parametros.TipoOperacao?.ConfiguracaoCalculoFrete?.TipoCotacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional.CotacaoCorrente;
                    decimal valorCotacao = parametros.TipoOperacao?.ConfiguracaoCalculoFrete?.ValorMoedaCotacao ?? 0m;

                    if (tipoCotacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional.CotacaoFixa)
                        dados.ValorCotacaoMoeda = valorCotacao;
                    else
                    {
                        Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork);

                        Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = repCotacao.BuscarCotacao(tabelaFreteCliente.Moeda, parametros.DataBaseCRT.Value);
                        if (cotacao.CotacaoAutomaticaViaWS && cotacao.UtilizarCotacaoRetroativa)
                        {
                            Servicos.Embarcador.Moedas.Cotacao servicoCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);
                            cotacao.ValorMoeda = servicoCotacao.ObterValoMoedaDiaria(cotacao.MoedaCotacaoBancoCentral, parametros.DataBaseCRT.Value, unitOfWork);
                        }

                        if (cotacao != null)
                            dados.ValorCotacaoMoeda = cotacao.ValorMoeda;

                        if (tipoCotacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional.CotacaoMinima && dados.ValorCotacaoMoeda < valorCotacao)
                            dados.ValorCotacaoMoeda = valorCotacao;
                    }
                }
            }

            if (dados.UtilizaMoedaEstrangeira)
            {
                dados.ValorFreteMoeda = dados.ValorFrete;
                dados.ValorFrete = Math.Round(dados.ValorFrete * dados.ValorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);

                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dados.Componentes)
                {
                    if (componente.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal && !parametros.CargaInternacional)
                    {
                        //quando calcula por percentual sobre o valor da nota fiscal sempre pega o valor em reais da nota fiscal, então, deve calcular o valor em moeda, ao contrário dos demais
                        if (dados.ValorCotacaoMoeda > 0m)
                            componente.ValorComponenteMoeda = Math.Round(componente.ValorComponente / dados.ValorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        componente.ValorComponenteMoeda = componente.ValorComponente;
                        componente.ValorComponente = Math.Round(componente.ValorComponente * dados.ValorCotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }
        }

        private void SetarValoresTabelaFretePorItensParametroBase(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, IList<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, decimal valorMinimoGarantido, decimal valorMaximo, decimal valorBase, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensGerais = itens.Where(o => o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque ||
                                                                                                                        o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao ||
                                                                                                                        o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem ||
                                                                                                                        (o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga && !tabelaFreteCliente.TabelaFrete.NaoPermitirLancarValorPorTipoDeCarga) ||
                                                                                                                        o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Rota).OrderBy(o => o.TipoValor).ToList();

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itensGerais)
            {
                if (item.ValorParaCalculo > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                    composicao.TipoParametro = item.TipoObjeto;

                    switch (item.TipoObjeto)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque:
                            if (parametros.ModeloVeiculo?.Codigo == item.CodigoObjeto || parametros.CalcularVariacoes)
                            {
                                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(item.CodigoObjeto);
                                Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "Valor por Modelo de Reboque", (modeloVeicularCarga?.Descricao ?? "") + " = " + item.ValorParaCalculoFormatado, item.ValorParaCalculo);
                                SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, item.ValorParaCalculo, 0m, false, null, tabelaFreteCliente, unitOfWork);
                                dados.ComposicaoFrete.Add(composicao);
                            }
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao:
                            if (Servicos.Veiculo.ObterTracao(parametros.Veiculo)?.ModeloVeicularCarga?.Codigo == item.CodigoObjeto || parametros.CalcularVariacoes)
                            {
                                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(item.CodigoObjeto);
                                Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "Valor por Modelo de Tração", (modeloVeicularCarga?.Descricao ?? "") + " = " + item.ValorParaCalculoFormatado, item.ValorParaCalculo);
                                SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, item.ValorParaCalculo, 0m, false, null, tabelaFreteCliente, unitOfWork);
                                dados.ComposicaoFrete.Add(composicao);
                            }
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga:
                            if (parametros.TipoCarga?.Codigo == item.CodigoObjeto)
                            {
                                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPorCodigo(item.CodigoObjeto);
                                Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "Valor por Tipo de Carga", (tipoDeCarga?.Descricao ?? "") + " = " + item.ValorParaCalculoFormatado, item.ValorParaCalculo);
                                SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, item.ValorParaCalculo, 0m, false, null, tabelaFreteCliente, unitOfWork);
                                dados.ComposicaoFrete.Add(composicao);
                            }
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Rota:
                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                SetarValorPorRotaDinamica(ref dados, tabelaFreteCliente, parametros, item, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem:
                            if ((parametros.TiposEmbalagem?.Count ?? 0) > 0)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem parametroTipoEmbalagem = (from obj in parametros.TiposEmbalagem where obj.TipoEmbalagem.Codigo == item.CodigoObjeto select obj).FirstOrDefault();
                                if (parametroTipoEmbalagem != null)
                                {
                                    decimal valor = item.ValorParaCalculo * parametroTipoEmbalagem.Quantidade;
                                    Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, "Valor por Tipo de Embalagem", (parametroTipoEmbalagem.TipoEmbalagem.Descricao) + " * Quantidade (" + parametroTipoEmbalagem.Quantidade + ") = " + valor.ToString("n2"), item.ValorParaCalculo);
                                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, valor, 0m, false, null, tabelaFreteCliente, unitOfWork);
                                    dados.ComposicaoFrete.Add(composicao);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            Log.TratarErro($"Setar valores tabela frete parametros", "GATILHO");

            BuscarValorPorDistancia(parametros, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia && obj.TipoValor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.Desabilitado select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);
            BuscarValorPorPeso(parametros, false, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso && obj.TipoValor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.Desabilitado select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente, tipoServicoMultisoftware);
            BuscarValorPorPallets(parametros, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);
            if (!tabelaFreteCliente.TabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                BuscarValorPorEntrega(parametros, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega select obj).ToList(), unitOfWork, ref dados, tipoServicoMultisoftware, tabelaFreteCliente);
            if (tabelaFreteCliente.TabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos && dados.ValorFrete <= 0)
                BuscarValorPorPeso(parametros, true, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso && obj.TipoValor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.Desabilitado select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente, tipoServicoMultisoftware);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametrosCalculoTempo = ObterParametrosCalculoFreteTempo(tabelaFreteCliente, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo select obj).ToList(), parametros, unitOfWork);
            BuscarValorPorTempo(tabelaFreteCliente, parametros, parametrosCalculoTempo, ref dados, unitOfWork);

            BuscarValorPorAjudante(parametros, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);
            BuscarValorPorHora(parametros, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente, out bool valorCalculadoPorHoraCorrida);
            BuscarValorPorPacote(parametros, (from obj in itens where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pacote select obj).ToList(), unitOfWork, ref dados, tipoServicoMultisoftware, tabelaFreteCliente);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                SetarValorPorRotaFixa(ref dados, tabelaFreteCliente, parametros, unitOfWork);

            if (tabelaFreteCliente.TabelaFrete.PossuiValorBase && valorBase > 0)
            {
                if (!tabelaFreteCliente.TabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Base (" + valorBase.ToString("n2") + ")", "Valor Frete = " + valorBase.ToString("n2"), valorBase, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor Base", 0, valorBase);
                    dados.ComposicaoFrete.Add(composicao);
                    dados.ValorFrete += parametros.MultiplicarPeloResultadoDaDistancia || parametros.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga ? 0 : valorBase;
                }
                else
                {
                    dados.ValorBase = valorBase;
                }
            }

            if (tabelaFreteCliente.TabelaFrete.PossuiMinimoGarantido && valorMinimoGarantido > 0m && dados.ValorFrete < valorMinimoGarantido)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Minimo Garantido (" + valorMinimoGarantido.ToString("n2") + ") é maior que valor Frete " + dados.ValorFrete.ToString("n2"), "Valor Frete = " + valorMinimoGarantido.ToString("n2"), valorMinimoGarantido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Mínimo Garantido", 0, valorMinimoGarantido);
                dados.ComposicaoFrete.Add(composicao);
                dados.ValorFrete = valorMinimoGarantido;
            }
            else if (tabelaFreteCliente.TabelaFrete.PossuiValorMaximo && valorMaximo > 0m && dados.ValorFrete > valorMaximo)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Máximo do Frete (" + valorMaximo.ToString("n2") + ") é menor que valor Frete " + dados.ValorFrete.ToString("n2"), "Valor Frete = " + valorMaximo.ToString("n2"), valorMaximo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor Máximo do Frete", 0, valorMaximo);
                dados.ComposicaoFrete.Add(composicao);
                dados.ValorFrete = valorMaximo;
            }

            if (parametros.Desistencia)
            {
                dados.ValorFrete = Math.Round((dados.ValorFrete * (parametros.PercentualDesistencia / 100)), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                bool veiculoPossuiTagValePedagio = parametros.Veiculo?.PossuiTagValePedagio ?? false;
                List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesFreteTabelaFrete = repComponenteFreteTabelaFrete.BuscarPorTabelaFreteParaCalculo(tabelaFreteCliente.TabelaFrete.Codigo, veiculoPossuiTagValePedagio);
                List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesFreteTabelaFreteCargaPerigosa = (from obj in componentesFreteTabelaFrete where obj.ComponenteFrete.SomenteParaCargaPerigosa select obj).ToList();
                componentesFreteTabelaFrete = (from obj in componentesFreteTabelaFrete where !obj.ComponenteFrete.SomenteParaCargaPerigosa select obj).ToList();

                if (parametros.CargaPerigosa)
                    componentesFreteTabelaFrete.AddRange(componentesFreteTabelaFreteCargaPerigosa);

                List<int> codigosProcessarPosteriormente = (from obj in componentesFreteTabelaFrete where obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && obj.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Percentual && obj.TipoPercentual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreValorFreteEComponentes select obj.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensComponentes = itens.Where(o => o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete && !codigosProcessarPosteriormente.Contains(o.CodigoObjeto)).ToList();

                itensComponentes.AddRange(itens.Where(o => o.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete && codigosProcessarPosteriormente.Contains(o.CodigoObjeto)));
                itensComponentes = itensComponentes.GroupBy(item => item.CodigoObjeto).Select(obj => obj.FirstOrDefault()).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itensComponentes)
                {
                    if (item.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteFreteTabelaFrete = (from obj in componentesFreteTabelaFrete where obj.Codigo == item.CodigoObjeto select obj).FirstOrDefault();

                        if (componenteFreteTabelaFrete != null)
                            SetarComponente(tabelaFreteCliente, ref dados, parametros, componenteFreteTabelaFrete, item, unitOfWork);
                    }
                }

                if (tabelaFreteCliente.TabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                    BuscarValorPorEntrega(parametros, (from obj in itens where obj.TipoObjeto == TipoParametroBaseTabelaFrete.NumeroEntrega select obj).ToList(), unitOfWork, ref dados, tipoServicoMultisoftware, tabelaFreteCliente);
            }

            ConverterValoresCotacaoMoeda(dados, tabelaFreteCliente, parametros, configuracaoTMS, unitOfWork);
        }

        private void SetarValorPorRotaFixa(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork)
        {
            if (parametros.CodigosRotasFixas == null)
                return;

            Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete repRotaEmbarcadorTabelaFrete = new Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesRota = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

            foreach (int codigoCargaPedidoRotaFrete in parametros.CodigosRotasFixas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete cargaPedidoRotaFrete = repCargaPedidoRotaFrete.BuscarPorCodigo(codigoCargaPedidoRotaFrete, false);

                if (cargaPedidoRotaFrete != null)
                {
                    cargaPedidoRotaFrete.ValorTabelaFrete = 0m;

                    Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete rotaEmbarcadorTabelaFrete = repRotaEmbarcadorTabelaFrete.BuscarPorRotaFixa(cargaPedidoRotaFrete.RotaFrete.Codigo, tabelaFreteCliente.TabelaFrete.Codigo);

                    if (rotaEmbarcadorTabelaFrete != null && rotaEmbarcadorTabelaFrete.ValorAdicionalFixoPorRota)
                    {
                        if (rotaEmbarcadorTabelaFrete.ComponenteFrete != null)
                        {
                            if (componentesRota.Exists(obj => obj.ComponenteFrete.Codigo == rotaEmbarcadorTabelaFrete.ComponenteFrete.Codigo))
                            {
                                componentesRota.Find(obj => rotaEmbarcadorTabelaFrete.ComponenteFrete.Codigo == obj.ComponenteFrete.Codigo).ValorComponente += rotaEmbarcadorTabelaFrete.ValorFixoRota;
                            }
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteRota = new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico();
                                componenteRota.ComponenteFrete = rotaEmbarcadorTabelaFrete.ComponenteFrete;
                                componenteRota.ValorComponente = rotaEmbarcadorTabelaFrete.ValorFixoRota;
                                componenteRota.OutraDescricaoCTe = rotaEmbarcadorTabelaFrete.RotaFrete.Descricao;
                                componentesRota.Add(componenteRota);
                            }
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Rota", rotaEmbarcadorTabelaFrete.RotaFrete.Descricao + " = " + rotaEmbarcadorTabelaFrete.ValorFixoRota.ToString("n2"), rotaEmbarcadorTabelaFrete.ValorFixoRota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Rota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0);
                            SetarValorPorTipoEValor(ref dados, ref composicao, parametros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, rotaEmbarcadorTabelaFrete.ValorFixoRota, 0m, false, null, false, unitOfWork);
                            dados.ComposicaoFrete.Add(composicao);
                        }

                        cargaPedidoRotaFrete.ValorTabelaFrete = rotaEmbarcadorTabelaFrete.ValorFixoRota;
                    }

                    repCargaPedidoRotaFrete.Atualizar(cargaPedidoRotaFrete);
                }
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteRota in componentesRota)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Rota", componenteRota.OutraDescricaoCTe + " = " + componenteRota.ValorComponente.ToString("n2"), componenteRota.ValorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Rota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, componenteRota.ComponenteFrete?.Codigo ?? 0);
                componenteRota.OutraDescricaoCTe = "";
                SetarComponente(ref dados, ref composicao, parametros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, componenteRota.ValorComponente, componenteRota.ComponenteFrete, tabelaFreteCliente.IncluirICMSValorFrete, unitOfWork, 0m);
                dados.ComposicaoFrete.Add(composicao);
            }
        }

        private void SetarComponente(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor, decimal valor, Dominio.Entidades.Embarcador.Frete.ComponenteFrete compomenteFrete, bool incluirICMSValorFrete, Repositorio.UnitOfWork unitOfWork, decimal valorExcedente)
        {
            Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);

            if (valor > 0m || valorExcedente > 0m)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente();

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = dados.TabelaFrete;
                bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, compomenteFrete);

                componente.ComponenteFrete = compomenteFrete;
                componente.TipoComponenteFrete = componente.ComponenteFrete.TipoComponenteFrete;
                componente.IncluirBaseCalculoICMS = incluirICMSValorFrete;
                componente.DescontarValorTotalAReceber = compomenteFrete?.DescontarValorTotalAReceber ?? false;
                componente.AcrescentaValorTotalAReceber = compomenteFrete?.AcrescentaValorTotalAReceber ?? false;
                componente.NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : compomenteFrete?.NaoSomarValorTotalAReceber) ?? false;
                componente.DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : compomenteFrete?.DescontarValorTotalAReceber) ?? false;
                componente.DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false;
                componente.ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0;
                componente.NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : compomenteFrete?.NaoSomarValorTotalPrestacao) ?? false;
                componente.SomarComponenteFreteLiquido = compomenteFrete?.SomarComponenteFreteLiquido ?? false;
                componente.DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : compomenteFrete?.DescontarComponenteFreteLiquido) ?? false;

                decimal valorNotas = (compomenteFrete?.NaoDeveIncidirSobreNotasFiscaisPateles ?? false) && parametros.ValorNotasFiscaisSemPallets > 0 ? parametros.ValorNotasFiscaisSemPallets : parametros.ValorNotasFiscais;

                switch (tipoValor)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual:
                        componente.Percentual = valor;
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual;
                        decimal valorCalculadoAumentoPercentual = (valorNotas * (valor / 100)) + valorExcedente;
                        componente.ValorComponente = valorCalculadoAumentoPercentual;
                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, " (Valor Total Mercadoria * Percentual)", " (" + valorNotas.ToString("n5") + " * " + dados.PercentualSobreNF.ToString("n5") + "%)", valor);
                        composicao.ValorCalculado = valorCalculadoAumentoPercentual;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;
                        componente.ValorComponente = valor + valorExcedente;
                        composicao.ValorCalculado = valor + valorExcedente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                        componente.Percentual = valor;
                        decimal valorCalculado = (valorNotas * (valor / 100)) + valorExcedente;
                        dados.ValorFrete += valorCalculado;
                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, " (Valor Total Mercadoria * Percentual)", " (" + valorNotas.ToString("n5") + " * " + dados.PercentualSobreNF.ToString("n5") + "%)", valor);
                        composicao.ValorCalculado = valorCalculado;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                        componente.ValorComponente = valor + valorExcedente;
                        composicao.ValorCalculado = valor + valorExcedente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                        componente.ValorComponente = valor + valorExcedente;
                        composicao.ValorCalculado = valor + valorExcedente;
                        break;
                    default:
                        break;
                }

                if (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0)
                {
                    componente.ValorComponente = -componente.ValorComponente;
                    composicao.ValorCalculado = -composicao.ValorCalculado;
                }

                serComponetesFrete.SetarConfiguracoesComponente(ref componente, parametros, unitOfWork);

                dados.Componentes.Add(componente);
            }
        }

        private bool VerificarSeComponenteEstaNoDiaEspecifico(DateTime dia, Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabelaFrete)
        {
            switch (dia.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    if (componenteTabelaFrete.Domingo.Value)
                        return true;
                    break;
                case DayOfWeek.Monday:
                    if (componenteTabelaFrete.SegundaFeira.Value)
                        return true;
                    break;
                case DayOfWeek.Tuesday:
                    if (componenteTabelaFrete.TercaFeira.Value)
                        return true;
                    break;
                case DayOfWeek.Wednesday:
                    if (componenteTabelaFrete.QuartaFeira.Value)
                        return true;
                    break;
                case DayOfWeek.Thursday:
                    if (componenteTabelaFrete.QuintaFeira.Value)
                        return true;
                    break;
                case DayOfWeek.Friday:
                    if (componenteTabelaFrete.SextaFeira.Value)
                        return true;
                    break;
                case DayOfWeek.Saturday:
                    if (componenteTabelaFrete.Sabado.Value)
                        return true;
                    break;
                default:
                    break;
            }

            return false;
        }

        private bool ValidarUtilizacaoComponenteCalculado(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (!ValidarUtilizacaoComponenteCalculadoPorValorMercadoria(parametros, componenteTabelaFrete))
                return false;

            if ((componenteTabelaFrete.EntregaMinima > 0m) && (parametros.NumeroEntregas < componenteTabelaFrete.EntregaMinima))
                return false;

            bool validacaoPorPesoMercadoriaConfigurada = componenteTabelaFrete.ValidarPesoMercadoria ?? false;
            bool validacaoPorDimensoesMercadoriaConfigurada = componenteTabelaFrete.ValidarDimensoesMercadoria ?? false;
            bool pesoMercadoriaValidado = ValidarUtilizacaoComponenteCalculadoPorPesoMercadoria(parametros, componenteTabelaFrete);
            bool dimensoesMercadoriaValidadas = ValidarUtilizacaoComponenteCalculadoPorDimensoesMercadoria(parametros, componenteTabelaFrete);

            if (validacaoPorPesoMercadoriaConfigurada && validacaoPorDimensoesMercadoriaConfigurada)
            {
                if (!pesoMercadoriaValidado && !dimensoesMercadoriaValidadas)
                    return false;
            }
            else if (!pesoMercadoriaValidado || !dimensoesMercadoriaValidadas)
                return false;

            if (componenteTabelaFrete.EscoltaArmada.HasValue && componenteTabelaFrete.EscoltaArmada.Value && !parametros.EscoltaArmada)
                return false;

            if (componenteTabelaFrete.GerenciamentoRisco.HasValue && componenteTabelaFrete.GerenciamentoRisco.Value && !parametros.GerenciamentoRisco)
                return false;

            if (componenteTabelaFrete.Reentrega.HasValue && componenteTabelaFrete.Reentrega.Value && !parametros.NecessarioReentrega)
                return false;

            if (componenteTabelaFrete.Rastreado.HasValue && componenteTabelaFrete.Rastreado.Value && !parametros.Rastreado)
                return false;

            if (componenteTabelaFrete.RestricaoTrafego.HasValue && componenteTabelaFrete.RestricaoTrafego.Value && !parametros.PossuiRestricaoTrafego)
                return false;

            if (componenteTabelaFrete.DespachoTransitoAduaneiro.HasValue && componenteTabelaFrete.DespachoTransitoAduaneiro.Value && !parametros.DespachoTransitoAduaneiro)
                return false;

            if (componenteTabelaFrete.MultiplicarPorAjudante && parametros.NumeroAjudantes <= 0)
                return false;

            if (componenteTabelaFrete.MultiplicarPorDeslocamento && parametros.NumeroDeslocamento <= 0)
                return false;

            if (componenteTabelaFrete.MultiplicarPorDiaria && parametros.NumeroDiarias <= 0)
                return false;

            if (componenteTabelaFrete.MultiplicarPorEntrega && parametros.NumeroEntregas <= 0)
                return false;

            if (componenteTabelaFrete.TipoViagem.HasValue)
            {
                if (componenteTabelaFrete.TipoViagem == TipoViagemComponenteTabelaFrete.Propria && parametros.FreteTerceiro)
                    return false;
                else if (componenteTabelaFrete.TipoViagem == TipoViagemComponenteTabelaFrete.Terceiros && !parametros.FreteTerceiro)
                    return false;
            }

            if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos && componenteTabelaFrete.TipoDocumentoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
            {
                if (parametros.QuantidadeEmissoesPorModeloDocumento == null || parametros.QuantidadeEmissoesPorModeloDocumento.Count == 0)
                    return false;

                if (componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade != null)
                {
                    if (componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade.Numero == "57" && parametros.QuantidadeEmissoesPorModeloDocumento.All(o => o.Key != null && o.Key.Numero != "57")) //cargaPedidos.Any(o => o.ModeloDocumentoFiscal != null && o.ModeloDocumentoFiscal.Numero != "57"))
                        return false;
                    else if (parametros.QuantidadeEmissoesPorModeloDocumento.All(o => o.Key == null || o.Key.Codigo != componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade.Codigo))  //cargaPedidos.Any(o => o.ModeloDocumentoFiscal == null || o.ModeloDocumentoFiscal.Codigo != componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade.Codigo))
                        return false;
                }
            }

            if (componenteTabelaFrete.SomenteComDataPrevisaoEntrega.HasValue && componenteTabelaFrete.SomenteComDataPrevisaoEntrega.Value && !parametros.DataPrevisaoEntrega.HasValue)
                return false;

            if ((componenteTabelaFrete.UtilizarDiasEspecificos.HasValue && componenteTabelaFrete.UtilizarDiasEspecificos.Value) || (componenteTabelaFrete.UtilizarPeriodoColeta.HasValue && componenteTabelaFrete.UtilizarPeriodoColeta.Value))
            {
                bool utilizarComponente = false;

                if (!parametros.DataColeta.HasValue)
                    return false;

                DateTime dataColeta = parametros.DataColeta.Value;

                if (componenteTabelaFrete.UtilizarPeriodoColeta.HasValue && componenteTabelaFrete.UtilizarPeriodoColeta.Value)
                {
                    if (VerificarSeEstaEntreFaixaDeHorarios(dataColeta.TimeOfDay, componenteTabelaFrete.HoraColetaInicial.Value, componenteTabelaFrete.HoraColetaFinal.Value))
                        return true;
                }

                if (componenteTabelaFrete.UtilizarDiasEspecificos.HasValue && componenteTabelaFrete.UtilizarDiasEspecificos.Value)
                {
                    if (VerificarSeComponenteEstaNoDiaEspecifico(dataColeta, componenteTabelaFrete))
                        return true;

                    if (componenteTabelaFrete.Feriados.HasValue && componenteTabelaFrete.Feriados.Value && new Servicos.Embarcador.Configuracoes.Feriado(unidadeTrabalho).VerificarSePossuiFeriado(dataColeta, parametros.Empresa?.Localidade))
                        return true;
                }

                return utilizarComponente;
            }

            return true;
        }

        private bool ValidarUtilizacaoComponenteCalculadoPorDimensoesMercadoria(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabelaFrete)
        {
            if (!(componenteTabelaFrete.ValidarDimensoesMercadoria ?? false))
                return true;

            bool validarAlturaMinima = (componenteTabelaFrete.AlturaMercadoriaMinima > 0m);
            bool validarLarguraMinima = (componenteTabelaFrete.LarguraMercadoriaMinima > 0m);
            bool validarComprimentoMinimo = (componenteTabelaFrete.ComprimentoMercadoriaMinimo > 0m);
            bool validarVolumeMinimo = (componenteTabelaFrete.VolumeMercadoriaMinimo > 0m);

            if (validarAlturaMinima || validarLarguraMinima || validarComprimentoMinimo || validarVolumeMinimo)
            {
                bool alturaMinimaValida = validarAlturaMinima && (parametros.MaiorAlturaProdutoEmCentimetros >= componenteTabelaFrete.AlturaMercadoriaMinima);
                bool larguraMinimaValida = validarLarguraMinima && (parametros.MaiorLarguraProdutoEmCentimetros >= componenteTabelaFrete.LarguraMercadoriaMinima);
                bool comprimentoMinimoValido = validarComprimentoMinimo && (parametros.MaiorComprimentoProdutoEmCentimetros >= componenteTabelaFrete.ComprimentoMercadoriaMinimo);
                bool volumeMinimoValido = validarVolumeMinimo && (parametros.MaiorVolumeProdutoEmCentimetros >= componenteTabelaFrete.VolumeMercadoriaMinimo);
                bool dimensoesMinimasValidas = alturaMinimaValida || larguraMinimaValida || comprimentoMinimoValido || volumeMinimoValido;

                if (!dimensoesMinimasValidas)
                    return false;
            }

            bool validarAlturaMaxima = (componenteTabelaFrete.AlturaMercadoriaMaxima > 0m);
            bool validarLarguraMaxima = (componenteTabelaFrete.LarguraMercadoriaMaxima > 0m);
            bool validarComprimentoMaximo = (componenteTabelaFrete.ComprimentoMercadoriaMaximo > 0m);
            bool validarVolumeMaximo = (componenteTabelaFrete.VolumeMercadoriaMaximo > 0m);

            if (validarAlturaMaxima || validarLarguraMaxima || validarComprimentoMaximo || validarVolumeMaximo)
            {
                bool alturaMaximaValida = validarAlturaMaxima && (parametros.MaiorAlturaProdutoEmCentimetros <= componenteTabelaFrete.AlturaMercadoriaMaxima);
                bool larguraMaximaValida = validarLarguraMaxima && (parametros.MaiorLarguraProdutoEmCentimetros <= componenteTabelaFrete.LarguraMercadoriaMaxima);
                bool comprimentoMaximoValido = validarComprimentoMaximo && (parametros.MaiorComprimentoProdutoEmCentimetros <= componenteTabelaFrete.ComprimentoMercadoriaMaximo);
                bool volumeMaximoValido = validarVolumeMaximo && (parametros.MaiorVolumeProdutoEmCentimetros <= componenteTabelaFrete.VolumeMercadoriaMaximo);
                bool dimensoesMaximasValidadas = alturaMaximaValida || larguraMaximaValida || comprimentoMaximoValido || volumeMaximoValido;

                if (!dimensoesMaximasValidadas)
                    return false;
            }

            return true;
        }

        private bool ValidarUtilizacaoComponenteCalculadoPorPesoMercadoria(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabelaFrete)
        {
            if (!(componenteTabelaFrete.ValidarPesoMercadoria ?? false))
                return true;

            decimal peso = ObterPesoPorUnidadeMedida(parametros, Dominio.Enumeradores.UnidadeMedida.KG);

            if ((componenteTabelaFrete.PesoMercadoriaMinimo > 0m) && (peso < componenteTabelaFrete.PesoMercadoriaMinimo))
                return false;

            if ((componenteTabelaFrete.PesoMercadoriaMaximo > 0m) && (peso > componenteTabelaFrete.PesoMercadoriaMaximo))
                return false;

            return true;
        }

        private bool ValidarUtilizacaoComponenteCalculadoPorValorMercadoria(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabelaFrete)
        {
            if (!(componenteTabelaFrete.ValidarValorMercadoria ?? false))
                return true;

            if ((componenteTabelaFrete.ValorMercadoriaMinimo > 0m) && (parametros.ValorNotasFiscais < componenteTabelaFrete.ValorMercadoriaMinimo))
                return false;

            if ((componenteTabelaFrete.ValorMercadoriaMaximo > 0m) && (parametros.ValorNotasFiscais > componenteTabelaFrete.ValorMercadoriaMaximo))
                return false;

            return true;
        }

        private void SetarComponente(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteTabelaFrete, Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repComponenteFreteCarga = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete repCargaTabelaFreteComponenteFrete = new Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);

            decimal pesoReferenciaCalculo = 0m, valor = 0m, valorMinimo = 0m, valorMaximo = 0m, valorExcedenteKG = 0m, valorAdicionalComponente = 0m, volumeReferenciaCalculo = 0m, valorExcedenteVolume = 0m;
            decimal cubagemReferenciaCalculo = 0m, pesoMercadoriaMinimo = 0m, valorMercadoriaMinimo = 0m, volumeMercadoriaMinimo = 0m;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = item.TipoValor;

            bool calculoPorPeso = false, calculoPorQuantidadeDocumentos = false, incluirBaseCalculo = false, calculoPorEixo = false, calculoPorCubagem = false, calculoPorVolume = false;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete tipoCalculoPeso = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorFracao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete tipoCalculoVolume = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorFracao;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete tipoCalculoQuantidadeDocumentos = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorNotaFiscal;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete tipoCalculoPercentual = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreNotaFiscal;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete tipoCalculoCubagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete.PorUnidadeIncompleta;

            int quantidadeDocumentos = 0;

            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
            composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete;

            if (componenteTabelaFrete.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado)
            {
                if (!ValidarUtilizacaoComponenteCalculado(parametros, componenteTabelaFrete, unitOfWork))
                    return;

                if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Tempo)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametroCalculoFreteTempo = ObterParametrosCalculoFreteTempo(componenteTabelaFrete, parametros, unitOfWork);

                    BuscarValorPorTempo(tabelaFreteCliente, parametros, parametroCalculoFreteTempo, ref dados, unitOfWork);

                    return;
                }

                if (!componenteTabelaFrete.ValorInformadoNaTabela.Value)
                {
                    if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Percentual)
                    {
                        if (componenteTabelaFrete.Percentual.HasValue)
                            valor = componenteTabelaFrete.Percentual.Value;

                        if (componenteTabelaFrete.TipoPercentual.HasValue)
                        {
                            tipoCalculoPercentual = componenteTabelaFrete.TipoPercentual.Value;
                            if (componenteTabelaFrete.ValorMercadoriaMinimo.HasValue)
                                valorMercadoriaMinimo = componenteTabelaFrete.ValorMercadoriaMinimo.Value;
                        }


                        if (componenteTabelaFrete.TipoPercentual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreNotaFiscal || componenteTabelaFrete.TipoPercentual == TipoPercentualComponenteTabelaFrete.SobreValorNotasExcedenteValorMinimo)
                            tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                        else
                            tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual;

                        if (componenteTabelaFrete.IncluirBaseCalculo.HasValue)
                            incluirBaseCalculo = componenteTabelaFrete.IncluirBaseCalculo.Value;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Peso)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.ValorFormula.HasValue)
                            valor = componenteTabelaFrete.ValorFormula.Value;

                        if (componenteTabelaFrete.TipoCalculoPeso.HasValue)
                        {
                            tipoCalculoPeso = componenteTabelaFrete.TipoCalculoPeso.Value;
                            if (componenteTabelaFrete.PesoMercadoriaMinimo.HasValue)
                                pesoMercadoriaMinimo = componenteTabelaFrete.PesoMercadoriaMinimo.Value;
                        }


                        if (componenteTabelaFrete.Peso.HasValue)
                            pesoReferenciaCalculo = componenteTabelaFrete.Peso.Value;

                        if (componenteTabelaFrete.ValorExcedentePorKG.HasValue)
                            valorExcedenteKG = componenteTabelaFrete.ValorExcedentePorKG.Value;

                        calculoPorPeso = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Volume)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.ValorFormula.HasValue)
                            valor = componenteTabelaFrete.ValorFormula.Value;

                        if (componenteTabelaFrete.TipoCalculoVolume.HasValue)
                        {
                            tipoCalculoVolume = componenteTabelaFrete.TipoCalculoVolume.Value;
                            if (componenteTabelaFrete.VolumeMercadoriaMinimo.HasValue)
                                volumeMercadoriaMinimo = componenteTabelaFrete.VolumeMercadoriaMinimo.Value;
                        }


                        if (componenteTabelaFrete.Volume.HasValue)
                            volumeReferenciaCalculo = componenteTabelaFrete.Volume.Value;

                        if (componenteTabelaFrete.ValorExcedentePorVolume.HasValue)
                            valorExcedenteVolume = componenteTabelaFrete.ValorExcedentePorVolume.Value;

                        calculoPorVolume = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Cubagem)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.ValorFormula.HasValue)
                            valor = componenteTabelaFrete.ValorFormula.Value;

                        if (componenteTabelaFrete.TipoCalculoCubagem.HasValue)
                            tipoCalculoCubagem = componenteTabelaFrete.TipoCalculoCubagem.Value;

                        if (componenteTabelaFrete.Cubagem.HasValue)
                            cubagemReferenciaCalculo = componenteTabelaFrete.Cubagem.Value;

                        calculoPorCubagem = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.TipoDocumentoQuantidadeDocumentos.HasValue)
                            tipoCalculoQuantidadeDocumentos = componenteTabelaFrete.TipoDocumentoQuantidadeDocumentos.Value;

                        calculoPorQuantidadeDocumentos = true;

                        if (componenteTabelaFrete.ValorFormula.HasValue)
                            valor = componenteTabelaFrete.ValorFormula.Value;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.ValorFixo)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.ValorFormula.HasValue)
                            valor = componenteTabelaFrete.ValorFormula.Value;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Eixo)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.ValorFormula.HasValue)
                            valor = componenteTabelaFrete.ValorFormula.Value;

                        calculoPorEixo = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.ParametroFixo)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                        valor = parametros.PercentualFixoAdValorem;
                    }
                }
                else
                {
                    valor = item.ValorParaCalculo;

                    if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Percentual)
                    {
                        if (componenteTabelaFrete.TipoPercentual.HasValue)
                        {
                            tipoCalculoPercentual = componenteTabelaFrete.TipoPercentual.Value;
                            if (componenteTabelaFrete.ValorMercadoriaMinimo.HasValue)
                                valorMercadoriaMinimo = componenteTabelaFrete.ValorMercadoriaMinimo.Value;
                        }


                        if (componenteTabelaFrete.TipoPercentual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreNotaFiscal || componenteTabelaFrete.TipoPercentual == TipoPercentualComponenteTabelaFrete.SobreValorNotasExcedenteValorMinimo)
                            tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                        else
                            tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual;

                        if (componenteTabelaFrete.IncluirBaseCalculo.HasValue)
                            incluirBaseCalculo = componenteTabelaFrete.IncluirBaseCalculo.Value;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Peso)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.TipoCalculoPeso.HasValue)
                        {
                            tipoCalculoPeso = componenteTabelaFrete.TipoCalculoPeso.Value;

                            if (componenteTabelaFrete.PesoMercadoriaMinimo.HasValue)
                                pesoMercadoriaMinimo = componenteTabelaFrete.PesoMercadoriaMinimo.Value;
                        }

                        if (componenteTabelaFrete.Peso.HasValue)
                            pesoReferenciaCalculo = componenteTabelaFrete.Peso.Value;

                        if (componenteTabelaFrete.ValorExcedentePorKG.HasValue)
                            valorExcedenteKG = componenteTabelaFrete.ValorExcedentePorKG.Value;

                        calculoPorPeso = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Volume)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.TipoCalculoVolume.HasValue)
                        {
                            tipoCalculoVolume = componenteTabelaFrete.TipoCalculoVolume.Value;

                            if (componenteTabelaFrete.VolumeMercadoriaMinimo.HasValue)
                                volumeMercadoriaMinimo = componenteTabelaFrete.VolumeMercadoriaMinimo.Value;
                        }

                        if (componenteTabelaFrete.Volume.HasValue)
                            volumeReferenciaCalculo = componenteTabelaFrete.Volume.Value;

                        if (componenteTabelaFrete.ValorExcedentePorKG.HasValue)
                            valorExcedenteKG = componenteTabelaFrete.ValorExcedentePorKG.Value;

                        calculoPorVolume = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Cubagem)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.TipoCalculoCubagem.HasValue)
                            tipoCalculoCubagem = componenteTabelaFrete.TipoCalculoCubagem.Value;

                        if (componenteTabelaFrete.Cubagem.HasValue)
                            cubagemReferenciaCalculo = componenteTabelaFrete.Cubagem.Value;

                        calculoPorCubagem = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteTabelaFrete.TipoDocumentoQuantidadeDocumentos.HasValue)
                            tipoCalculoQuantidadeDocumentos = componenteTabelaFrete.TipoDocumentoQuantidadeDocumentos.Value;

                        calculoPorQuantidadeDocumentos = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.ValorFixo)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Eixo)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;
                        calculoPorEixo = true;
                    }
                    else if (componenteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.ParametroFixo)
                    {
                        tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual;
                        valor = parametros.PercentualFixoAdValorem;
                    }
                }

                if (componenteTabelaFrete.ValorMinimo.HasValue)
                    valorMinimo = componenteTabelaFrete.ValorMinimo.Value;

                if (componenteTabelaFrete.ValorMaximo.HasValue)
                    valorMaximo = componenteTabelaFrete.ValorMaximo.Value;
            }
            else
            {
                if (componenteTabelaFrete.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO
                && ObterConfiguracaoTabelaFrete().ArredondarValorDoComponenteDePedagioParaProximoInteiro)
                    valor = item.ValorParaCalculo % 1 != 0 ? (int)Math.Floor(item.ValorParaCalculo) + 1 : item.ValorParaCalculo;
                else
                    valor = item.ValorParaCalculo;

                tipoValor = item.TipoValor;
                composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de " + componenteTabelaFrete.ComponenteFrete.Descricao, componenteTabelaFrete.ComponenteFrete.Descricao + " = " + item.ValorParaCalculoFormatado, item.ValorParaCalculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, item.TipoValor, componenteTabelaFrete.ComponenteFrete.Codigo);
            }

            if (calculoPorPeso)
            {
                decimal peso = ObterPesoPorUnidadeMedida(parametros, Dominio.Enumeradores.UnidadeMedida.KG);

                decimal fatorMultiplicacao = 0m;

                string formula = "Cálculo de quantidade ";
                string valoresFormula = "";
                string formulaExcedente = "";
                string valoresFormulaExcedente = "";

                decimal taxaDoElemento = 0m;

                switch (tipoCalculoPeso)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorUnidadeIncompleta:
                        fatorMultiplicacao = pesoReferenciaCalculo > 0m ? Math.Ceiling(peso / pesoReferenciaCalculo) : 0m;
                        formula += "por unidade incompleta (Arredondamento para cima) (Quantidade / Quantidade Refêrencia para Cálculo = Fator de Multiplicação)";
                        valoresFormula = "(" + peso.ToString("n5") + " / " + pesoReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n5") + ")";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorUnidadeCompleta:
                        fatorMultiplicacao = pesoReferenciaCalculo > 0m ? Math.Floor(peso / pesoReferenciaCalculo) : 0m;
                        formula += "por unidade completa (Arredondamento para baixo) (Quantidade / Quantidade Referência para Cálculo = Fator de Multiplicação)";
                        valoresFormula = "(" + peso.ToString("n5") + " / " + pesoReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n5") + ")";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorValorFixoComExcedente:
                        fatorMultiplicacao = 1;
                        formula = "Fator de Multiplicação Fixo";
                        valoresFormula = "Fator de Multiplicação Fixo = 1";

                        if (pesoReferenciaCalculo < peso && valorExcedenteKG > 0m)
                        {
                            formulaExcedente = " + (Quantidade - Quantidade Referência) * Valor por Unidade Excedente ";
                            valoresFormulaExcedente = " + (" + peso.ToString("n5") + " - " + pesoReferenciaCalculo.ToString("n5") + ") * " + valorExcedenteKG.ToString("n6") + " ";
                            valorAdicionalComponente = Math.Round((peso - pesoReferenciaCalculo) * valorExcedenteKG, 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete.PorFracao:
                        fatorMultiplicacao = pesoReferenciaCalculo > 0m ? peso / pesoReferenciaCalculo : 0m;
                        formula += "Fator de Multiplicação = Quantidade / Quantidade Referência";
                        valoresFormula = peso.ToString("n5") + " / " + pesoReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n2") + " ";
                        break;
                    case TipoCalculoPesoTabelaFreteComponenteFrete.PorFracaoExcedentePesoMinimo:
                        decimal pesoUsar = (peso - pesoMercadoriaMinimo);
                        fatorMultiplicacao = pesoReferenciaCalculo > 0m ? pesoUsar / pesoReferenciaCalculo : 0m;
                        formula += "Fator de Multiplicação = Quantidade Excedente / Quantidade Referência";
                        valoresFormula = pesoUsar.ToString("n5") + " / " + pesoReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n2") + " ";
                        break;
                }

                formula += " (Fator de Multiplicação * Valor do Componente) ";
                valoresFormula += " (" + fatorMultiplicacao.ToString("n2") + " *  " + valor.ToString("n2") + ") ";
                taxaDoElemento = valor;

                valor = fatorMultiplicacao * valor;

                if (valor <= 0m)
                {
                    if (valorMinimo > 0m && valorMinimo > valor)
                    {
                        formula = " Valor Mínimo ";
                        valoresFormula = " Valor Mínimo = " + valorMinimo.ToString("n2");
                        valor = valorMinimo;
                    }
                    else if (valorMaximo > 0m && valorMaximo < valor)
                    {
                        valor = valorMaximo;
                    }
                }

                composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete(formula + formulaExcedente, valoresFormula + valoresFormulaExcedente, taxaDoElemento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, item.TipoValor, 0);
            }
            else if (calculoPorVolume)
            {
                decimal volume = parametros.Volumes;

                decimal fatorMultiplicacao = 0m;

                string formula = "Cálculo de quantidade ";
                string valoresFormula = "";
                string formulaExcedente = "";
                string valoresFormulaExcedente = "";

                decimal taxaDoElemento = 0m;

                switch (tipoCalculoVolume)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorUnidadeIncompleta:
                        fatorMultiplicacao = volumeReferenciaCalculo > 0m ? Math.Ceiling(volume / volumeReferenciaCalculo) : 0m;
                        formula += "por unidade incompleta (Arredondamento para cima) (Quantidade / Quantidade Refêrencia para Cálculo = Fator de Multiplicação)";
                        valoresFormula = "(" + volume.ToString("n5") + " / " + volumeReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n5") + ")";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorUnidadeCompleta:
                        fatorMultiplicacao = volumeReferenciaCalculo > 0m ? Math.Floor(volume / volumeReferenciaCalculo) : 0m;
                        formula += "por unidade completa (Arredondamento para baixo) (Quantidade / Quantidade Referência para Cálculo = Fator de Multiplicação)";
                        valoresFormula = "(" + volume.ToString("n5") + " / " + volumeReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n5") + ")";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorValorFixoComExcedente:
                        fatorMultiplicacao = 1;
                        formula = "Fator de Multiplicação Fixo";
                        valoresFormula = "Fator de Multiplicação Fixo = 1";

                        if (volumeReferenciaCalculo < volume && valorExcedenteVolume > 0m)
                        {
                            formulaExcedente = " + (Quantidade - Quantidade Referência) * Valor por Unidade Excedente ";
                            valoresFormulaExcedente = " + (" + volume.ToString("n5") + " - " + volumeReferenciaCalculo.ToString("n5") + ") * " + valorExcedenteVolume.ToString("n6") + " ";
                            valorAdicionalComponente = Math.Round((volume - volumeReferenciaCalculo) * valorExcedenteVolume, 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete.PorFracao:
                        fatorMultiplicacao = volumeReferenciaCalculo > 0m ? volume / volumeReferenciaCalculo : 0m;
                        formula += "Fator de Multiplicação = Quantidade / Quantidade Referência";
                        valoresFormula = volume.ToString("n5") + " / " + volumeReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n2") + " ";
                        break;
                    case TipoCalculoVolumeTabelaFreteComponenteFrete.PorFracaoExcedentePesoMinimo:
                        decimal volumeUsar = (volume - volumeMercadoriaMinimo);
                        fatorMultiplicacao = volumeReferenciaCalculo > 0m ? volumeUsar / volumeReferenciaCalculo : 0m;
                        formula += "Fator de Multiplicação = Quantidade Excedente / Quantidade Referência";
                        valoresFormula = volumeUsar.ToString("n5") + " / " + volumeReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n2") + " ";
                        break;
                }

                formula += " (Fator de Multiplicação * Valor do Componente) ";
                valoresFormula += " (" + fatorMultiplicacao.ToString("n2") + " *  " + valor.ToString("n2") + ") ";
                taxaDoElemento = valor;

                valor = fatorMultiplicacao * valor;

                if (valor <= 0m)
                {
                    if (valorMinimo > 0m && valorMinimo > valor)
                    {
                        formula = " Valor Mínimo ";
                        valoresFormula = " Valor Mínimo = " + valorMinimo.ToString("n2");
                        valor = valorMinimo;
                    }
                    else if (valorMaximo > 0m && valorMaximo < valor)
                    {
                        valor = valorMaximo;
                    }
                }

                composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete(formula + formulaExcedente, valoresFormula + valoresFormulaExcedente, taxaDoElemento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, item.TipoValor, 0);
            }
            else if (calculoPorCubagem)
            {
                decimal cubagem = ObterCubagem(parametros);

                decimal fatorMultiplicacao = 0m;

                string formula = "Cálculo por cubagem ";
                string valoresFormula = "";
                decimal taxaDoElemento = 0m;

                switch (tipoCalculoCubagem)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete.PorUnidadeIncompleta:
                        fatorMultiplicacao = cubagemReferenciaCalculo > 0m ? Math.Ceiling(cubagem / cubagemReferenciaCalculo) : 0m;
                        formula += "por unidade incompleta (Arredondamento para cima) (Quantidade / Quantidade Refêrencia para Cálculo = Fator de Multiplicação)";
                        valoresFormula = "(" + cubagem.ToString("n5") + " / " + cubagemReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n5") + ")";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete.PorUnidadeCompleta:
                        fatorMultiplicacao = cubagemReferenciaCalculo > 0m ? Math.Floor(cubagem / cubagemReferenciaCalculo) : 0m;
                        formula += "por unidade completa (Arredondamento para baixo) (Quantidade / Quantidade Referência para Cálculo = Fator de Multiplicação)";
                        valoresFormula = "(" + cubagem.ToString("n5") + " / " + cubagemReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n5") + ")";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete.PorFracao:
                        fatorMultiplicacao = cubagemReferenciaCalculo > 0m ? cubagem / cubagemReferenciaCalculo : 0m;
                        formula += "Fator de Multiplicação = Quantidade / Quantidade Referência";
                        valoresFormula = cubagem.ToString("n5") + " / " + cubagemReferenciaCalculo.ToString("n5") + " = " + fatorMultiplicacao.ToString("n2") + " ";
                        break;
                }

                formula += " (Fator de Multiplicação * Valor do Componente) ";
                valoresFormula += " (" + fatorMultiplicacao.ToString("n2") + " *  " + valor.ToString("n2") + ") ";
                taxaDoElemento = valor;

                valor = fatorMultiplicacao * valor;

                if (valor <= 0m)
                {
                    if (valorMinimo > 0m && valorMinimo > valor)
                    {
                        formula = " Valor Mínimo ";
                        valoresFormula = " Valor Mínimo = " + valorMinimo.ToString("n2");
                        valor = valorMinimo;
                    }
                    else if (valorMaximo > 0m && valorMaximo < valor)
                    {
                        valor = valorMaximo;
                    }
                }

                composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete(formula, valoresFormula, taxaDoElemento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, item.TipoValor, 0);
            }
            else if (calculoPorQuantidadeDocumentos)
            {
                string formula = "Cálculo por Quantidade";
                string valoresFormula = "";

                if (tipoCalculoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
                {
                    formula += " de ";

                    if (componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade != null)
                    {
                        formula += componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade.Abreviacao + "(s) Emitidos";
                        quantidadeDocumentos = parametros.QuantidadeEmissoesPorModeloDocumento.Where(o => o.Key.Codigo == componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade.Codigo).Sum(o => o.Value);
                    }
                    else
                    {
                        formula += " Documentos Emitidos";
                        quantidadeDocumentos = parametros.QuantidadeEmissoesPorModeloDocumento.Sum(o => o.Value);
                    }
                }
                else
                {
                    formula += " de NF-e";
                    quantidadeDocumentos = parametros.QuantidadeNotasFiscais;
                }

                decimal valorPorItem = valor;

                formula += " (Valor * Quantidade Documentos)";
                valoresFormula = valor.ToString("n2") + " * " + quantidadeDocumentos.ToString();
                valor = valor * quantidadeDocumentos;

                composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete(formula, valoresFormula, valorPorItem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, item.TipoValor, "", 0, valor);
            }
            else if (calculoPorEixo)
            {
                string formula = "Cálculo por Eixo";
                string valoresFormula = "";

                int quantidadeEixos = (parametros.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0) + (parametros.Reboques?.Sum(o => o.ModeloVeicularCarga?.NumeroEixos ?? 0) ?? 0);

                decimal valorPorItem = valor;

                formula += " (Valor * Quantidade de Eixos)";
                valoresFormula = valor.ToString("n2") + " * " + quantidadeEixos.ToString();
                valor = valor * quantidadeEixos;

                composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete(formula, valoresFormula, valorPorItem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, item.TipoValor, "", 0, valor);
            }

            if (valor > 0m)
            {
                bool multiplicarPorAjudante = false, multiplicarPorEscolta = false, multiplicarPorDeslocamento = false, multiplicarPorDiaria = false, multiplicarPorEntrega = false;
                int quantidadeAjudantes = 0, quantidadeEscoltas = 0, quantidadeDeslocamento = 0, quantidadeDiarias = 0, quantidadeEntregas = 0;

                if (componenteTabelaFrete.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado)
                {
                    if (componenteTabelaFrete.MultiplicarPorAjudante)
                    {
                        multiplicarPorAjudante = true;
                        quantidadeAjudantes = (int)parametros.NumeroAjudantes;
                    }

                    if (componenteTabelaFrete.EscoltaArmada.HasValue && componenteTabelaFrete.EscoltaArmada.Value)
                    {
                        multiplicarPorEscolta = true;
                        quantidadeEscoltas = parametros.QuantidadeEscolta;
                    }

                    if (componenteTabelaFrete.MultiplicarPorDeslocamento)
                    {
                        multiplicarPorDeslocamento = true;
                        quantidadeDeslocamento = (int)parametros.NumeroDeslocamento;
                    }

                    if (componenteTabelaFrete.MultiplicarPorDiaria)
                    {
                        multiplicarPorDiaria = true;
                        quantidadeDiarias = (int)parametros.NumeroDiarias;
                    }

                    if (componenteTabelaFrete.MultiplicarPorEntrega)
                    {
                        multiplicarPorEntrega = true;
                        quantidadeEntregas = (int)parametros.NumeroEntregas;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente();

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = dados.TabelaFrete;
                bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, componenteTabelaFrete.ComponenteFrete);

                componente.ComponenteFrete = componenteTabelaFrete.ComponenteFrete;
                componente.TipoComponenteFrete = componente.ComponenteFrete.TipoComponenteFrete;
                componente.IncluirBaseCalculoICMS = componenteTabelaFrete.IncluirBaseCalculoICMS;
                componente.DescontarValorTotalAReceber = componenteTabelaFrete.ComponenteFrete?.DescontarValorTotalAReceber ?? false;
                componente.AcrescentaValorTotalAReceber = componenteTabelaFrete.ComponenteFrete?.AcrescentaValorTotalAReceber ?? false;
                componente.DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : componenteTabelaFrete.ComponenteFrete?.DescontarValorTotalAReceber) ?? false;
                componente.DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false;
                componente.ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0;
                componente.SomarComponenteFreteLiquido = componenteTabelaFrete.ComponenteFrete?.SomarComponenteFreteLiquido ?? false;
                componente.NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : componenteTabelaFrete.ComponenteFrete.NaoSomarValorTotalAReceber) ?? false;
                componente.NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : componenteTabelaFrete.ComponenteFrete.NaoSomarValorTotalPrestacao) ?? false;
                componente.ComponenteComparado = componenteTabelaFrete.ComponenteComparado;
                componente.DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : componenteTabelaFrete.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false;
                componente.UtilizarFormulaRateioCarga = componenteTabelaFrete.UtilizarFormulaRateioCarga ?? false;

                //regra fixa na tabela por pedido, rateia o valor do frete por peso entre os pedidos.
                if ((componenteTabelaFrete?.ValorUnicoParaCarga ?? false) && parametros.ParametrosCarga != null)
                {
                    RateioFormula serRateioFormula = new RateioFormula(_unitOfWork);
                    componente.ValorComponenteParaCarga = valor;
                    componente.ComponentePorCarga = true;
                    decimal valorRateioOriginal = 0;
                    valor = serRateioFormula.AplicarFormulaRateio(parametros.ParametrosCarga.FormulaRateio, valor, parametros.ParametrosCarga.NumeroPedidos, 0, parametros.ParametrosCarga.Peso, parametros.Peso, parametros.ValorNotasFiscais, parametros.ParametrosCarga.ValorNotasFiscais, componente.Percentual, componente.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, parametros.PesoLiquido, parametros.ParametrosCarga.PesoLiquido, (int)parametros.Volumes, (int)parametros.ParametrosCarga.Volumes);
                }

                if (calculoPorQuantidadeDocumentos)
                {
                    componente.CalculoPorQuantidadeDocumentos = true;
                    componente.TipoCalculoQuantidadeDocumentos = tipoCalculoQuantidadeDocumentos;
                    componente.QuantidadeTotalDocumentos = quantidadeDocumentos;

                    if (tipoCalculoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
                    {
                        componente.ModeloDocumentoFiscalRateio = componenteTabelaFrete.ModeloDocumentoFiscalRestringirQuantidade;
                        componente.RateioFormula = repRateioFormula.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe);
                    }
                    else if (tipoCalculoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorNotaFiscal)
                    {
                        componente.RateioFormula = repRateioFormula.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porNotaFiscal);
                    }
                }

                switch (tipoValor)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual:

                        componente.Percentual = valor;
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual;

                        if (componenteTabelaFrete.UtilizarCalculoDesseComponenteNaOcorrencia ?? false)
                            componente.ValorComponente = ObterValorPercentualOcorrencia(dados.ValorFrete, ref composicao, dados.Componentes.Sum(o => o.ValorComponente), valor, valorAdicionalComponente, tipoCalculoPercentual, incluirBaseCalculo);
                        else
                            componente.ValorComponente = ObterValorAumentoPercentual(dados.ValorFrete, ref composicao, dados.Componentes.Sum(o => o.ValorComponente), valor, valorAdicionalComponente, tipoCalculoPercentual, incluirBaseCalculo);

                        composicao.ValorCalculado = componente.ValorComponente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;
                        componente.ValorComponente = Math.Round(valor + valorAdicionalComponente, 2, MidpointRounding.AwayFromZero);
                        composicao.ValorCalculado = valor + valorAdicionalComponente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:
                        decimal valorNotasParametro = componenteTabelaFrete.ComponenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles && parametros.ValorNotasFiscaisSemPallets > 0 ? parametros.ValorNotasFiscaisSemPallets : parametros.ValorNotasFiscais;
                        decimal valorNotas = valorNotasParametro;
                        string descricaoExcente = "";
                        if (tipoCalculoPercentual == TipoPercentualComponenteTabelaFrete.SobreValorNotasExcedenteValorMinimo)
                        {
                            valorNotas = valorNotasParametro - valorMercadoriaMinimo;
                            descricaoExcente = "Excedente";
                        }

                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                        componente.Percentual = valor;
                        componente.ValorComponente = Math.Round(((valorNotas * (valor / 100)) + valorAdicionalComponente), 2, MidpointRounding.AwayFromZero);
                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, " (Valor Total " + descricaoExcente + " da Mercadoria * Percentual)", " (" + valorNotas.ToString("n5") + " * " + componente.Percentual.ToString("n5") + "%)", valor);
                        composicao.ValorCalculado = componente.ValorComponente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                        componente.ValorComponente = Math.Round(valor + valorAdicionalComponente, 2, MidpointRounding.AwayFromZero);
                        composicao.ValorCalculado = valor + valorAdicionalComponente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima:
                        componente.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                        componente.ValorComponente = Math.Round(valor + valorAdicionalComponente, 2, MidpointRounding.AwayFromZero);
                        composicao.ValorCalculado = valor + valorAdicionalComponente;
                        break;
                    default:
                        break;
                }

                if (componente.DescontarValorTotalAReceber && componente.ValorComponente > 0)
                {
                    componente.ValorComponente = -componente.ValorComponente;
                    composicao.ValorCalculado = -composicao.ValorCalculado;
                }

                if (multiplicarPorAjudante)
                {
                    if (composicao != null)
                    {
                        composicao.Formula = (composicao.Formula?.Trim() ?? "Valor") + " * Quantidade de Ajudantes";
                        composicao.ValoresFormula = (composicao.ValoresFormula?.Trim() ?? componente.ValorComponente.ToString("n5")) + $" * {quantidadeAjudantes}";
                        composicao.ValorCalculado = componente.ValorComponente * quantidadeAjudantes;
                    }

                    componente.ValorComponente *= quantidadeAjudantes;
                }

                if (multiplicarPorEscolta)
                {
                    if (composicao != null)
                    {
                        composicao.Formula = (composicao.Formula?.Trim() ?? "Valor") + " * Quantidade de Escoltas";
                        composicao.ValoresFormula = (composicao.ValoresFormula?.Trim() ?? componente.ValorComponente.ToString("n5")) + $" * {quantidadeEscoltas}";
                        composicao.ValorCalculado = componente.ValorComponente * quantidadeEscoltas;
                    }

                    componente.ValorComponente *= quantidadeEscoltas;
                }

                if (multiplicarPorDeslocamento)
                {
                    if (composicao != null)
                    {
                        composicao.Formula = (composicao.Formula?.Trim() ?? "Valor") + " * Quantidade por Deslocamento";
                        composicao.ValoresFormula = (composicao.ValoresFormula?.Trim() ?? componente.ValorComponente.ToString("n5")) + $" * {quantidadeDeslocamento}";
                        composicao.ValorCalculado = componente.ValorComponente * quantidadeDeslocamento;
                    }

                    componente.ValorComponente *= quantidadeDeslocamento;
                }

                if (multiplicarPorDiaria)
                {
                    if (composicao != null)
                    {
                        composicao.Formula = (composicao.Formula?.Trim() ?? "Valor") + " * Quantidade de Diárias";
                        composicao.ValoresFormula = (composicao.ValoresFormula?.Trim() ?? componente.ValorComponente.ToString("n5")) + $" * {quantidadeDiarias}";
                        composicao.ValorCalculado = componente.ValorComponente * quantidadeDiarias;
                    }

                    componente.ValorComponente *= quantidadeDiarias;
                }

                if (multiplicarPorEntrega)
                {
                    if (composicao != null)
                    {
                        composicao.Formula = (composicao.Formula?.Trim() ?? "Valor") + " * Quantidade de Entregas";
                        composicao.ValoresFormula = (composicao.ValoresFormula?.Trim() ?? componente.ValorComponente.ToString("n5")) + $" * {quantidadeEntregas}";
                        composicao.ValorCalculado = componente.ValorComponente * quantidadeEntregas;
                    }

                    componente.ValorComponente *= quantidadeEntregas;
                }

                if (composicao != null)
                {
                    composicao.DescricaoComponente = componente.ComponenteFrete.Descricao;
                    composicao.CodigoComponente = componente.ComponenteFrete.Codigo;
                    dados.ComposicaoFrete.Add(composicao);
                }

                if (valorMinimo > 0m && valorMinimo > componente.ValorComponente)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoMinimoGarantido = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete($"Valor mínimo garantido de {componente.ComponenteFrete.Descricao} ({valorMinimo.ToString("n2")}) é maior que o valor calculado {componente.ValorComponente.ToString("n2")}.", $"Valor de {componente.ComponenteFrete.Descricao} = {valorMinimo.ToString("n2")}.", valorMinimo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Mínimo Garantido de " + componente.ComponenteFrete.Descricao, componente.ComponenteFrete.Codigo, valorMinimo);
                    dados.ComposicaoFrete.Add(composicaoMinimoGarantido);
                    componente.ValorComponente = valorMinimo;
                }
                else if (valorMaximo > 0m && valorMaximo < componente.ValorComponente)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoMaximo = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete($"Valor máximo de {componente.ComponenteFrete.Descricao} ({valorMaximo.ToString("n2")}) é menor que o valor calculado ({componente.ValorComponente.ToString("n2")}).", $"Valor de {componente.ComponenteFrete.Descricao} = {valorMaximo.ToString("n2")}.", valorMaximo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Valor Máximo de " + componente.ComponenteFrete.Descricao, componente.ComponenteFrete.Codigo, valorMaximo);
                    dados.ComposicaoFrete.Add(composicaoMaximo);
                    componente.ValorComponente = valorMaximo;
                }

                if (parametros.PagamentoTerceiro)
                    componente.Justificativa = componenteTabelaFrete.Justificativa;

                serComponetesFrete.SetarConfiguracoesComponente(ref componente, parametros, unitOfWork);

                dados.Componentes.Add(componente);
            }
        }

        public decimal ObterValorAumentoPercentual(decimal valorFrete, ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, decimal valorComponentes, decimal percentual, decimal valorAdicionalComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete tipoPercentualComponenteTabelaFrete, bool incluirBaseCalculo)
        {
            decimal valorUtilizadoParaCalculo = valorFrete;
            decimal valorAumentoPercentual;

            if (tipoPercentualComponenteTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreValorFreteEComponentes)
                valorUtilizadoParaCalculo += valorComponentes;

            if (incluirBaseCalculo && percentual < 100m)
            {
                valorAumentoPercentual = (valorUtilizadoParaCalculo / ((100 - percentual) / 100)) - valorUtilizadoParaCalculo;
                composicao.Formula += " (Valor para Cálculo / ((100 - Percentual) / 100)) - Valor para Cálculo ";
                composicao.ValoresFormula += " (" + valorUtilizadoParaCalculo.ToString("n2") + " / ((100 - " + percentual.ToString("n2") + ") / 100) - " + valorUtilizadoParaCalculo.ToString("n2") + ") ";
                composicao.Valor = percentual;
            }
            else
            {
                valorAumentoPercentual = valorUtilizadoParaCalculo * (percentual / 100);
                composicao.Formula += " (Valor para Cálculo * Percentual) ";
                composicao.ValoresFormula += valorUtilizadoParaCalculo.ToString("n2") + " * " + percentual.ToString("n2") + "% ";
                composicao.Valor = percentual;
            }

            valorAumentoPercentual += valorAdicionalComponente;

            valorAumentoPercentual = Math.Round(valorAumentoPercentual, 2, MidpointRounding.AwayFromZero);

            return valorAumentoPercentual;
        }

        public decimal ObterValorPercentualOcorrencia(decimal valorFrete, ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, decimal valorComponentes, decimal percentual, decimal valorAdicionalComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete tipoPercentualComponenteTabelaFrete, bool incluirBaseCalculo)
        {
            decimal valorUtilizadoParaCalculo = valorFrete;

            valorUtilizadoParaCalculo = valorUtilizadoParaCalculo * (percentual / 100);
            composicao.Formula += " (Valor para Cálculo * Percentual) ";
            composicao.ValoresFormula += valorUtilizadoParaCalculo.ToString("n2") + " * " + percentual.ToString("n2") + "% ";
            composicao.Valor = percentual;

            valorUtilizadoParaCalculo += valorAdicionalComponente;

            valorUtilizadoParaCalculo = Math.Round(valorUtilizadoParaCalculo, 2, MidpointRounding.AwayFromZero);

            return valorUtilizadoParaCalculo;
        }

        private void SetarValorPorRotaDinamica(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, Repositorio.UnitOfWork unitOfWork)
        {
            if (parametros.RotasDinamicas == null)
                return;

            Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete repRotaEmbarcadorTabelaFrete = new Repositorio.Embarcador.Frete.RotaEmbarcadorTabelaFrete(unitOfWork);

            foreach (Dominio.Entidades.RotaFrete rotaDinamica in parametros.RotasDinamicas)
            {

                Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete rotaEmbarcadorTabelaFrete = repRotaEmbarcadorTabelaFrete.BuscarPorCodigo(item.Codigo);

                if (rotaEmbarcadorTabelaFrete != null && rotaEmbarcadorTabelaFrete.RotaFrete.Codigo == rotaDinamica.Codigo && !rotaEmbarcadorTabelaFrete.ValorAdicionalFixoPorRota)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Rota", rotaEmbarcadorTabelaFrete.RotaFrete.Descricao + " = " + item.ValorParaCalculoFormatado, item.ValorParaCalculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Rota, item.TipoValor, 0);

                    if (rotaEmbarcadorTabelaFrete.ComponenteFrete != null)
                        SetarComponente(ref dados, ref composicao, parametros, item.TipoValor, item.ValorParaCalculo, rotaEmbarcadorTabelaFrete.ComponenteFrete, tabelaFreteCliente.IncluirICMSValorFrete, unitOfWork, 0m);
                    else
                        SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, item, item.ValorParaCalculo, 0m, false, null, tabelaFreteCliente, unitOfWork);

                    dados.ComposicaoFrete.Add(composicao);
                }
            }
        }

        private void SetarPrevisaoEntregaTeoricaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente tabelaCargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);

            int leadTimeEmDias = tabelaCargaPedido.TabelaFreteCliente.LeadTime;

            if (leadTimeEmDias == 0)
                return;

            Servicos.Embarcador.Configuracoes.Feriado svcFeriado = new Configuracoes.Feriado(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

            if (!configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            DateTime previsaoEntrega = DateTime.Now;
            for (int i = 0; i < leadTimeEmDias; i++)
            {
                previsaoEntrega = previsaoEntrega.AddDays(1);

                while ((previsaoEntrega.DayOfWeek == DayOfWeek.Saturday) || (previsaoEntrega.DayOfWeek == DayOfWeek.Sunday) || svcFeriado.VerificarSePossuiFeriado(previsaoEntrega))
                    previsaoEntrega = previsaoEntrega.AddDays(1);
            }

            if (!tabelaCargaPedido.TabelaFreteFilialEmissora)
            {
                tabelaCargaPedido.CargaPedido.Pedido.LeadTime = leadTimeEmDias;
                tabelaCargaPedido.CargaPedido.Pedido.PrevisaoEntrega = previsaoEntrega;
            }
            else
            {
                tabelaCargaPedido.CargaPedido.Pedido.LeadTimeFilialEmissora = leadTimeEmDias;
                tabelaCargaPedido.CargaPedido.Pedido.PrevisaoEntregaFilialEmissora = previsaoEntrega;
            }

            if ((configuracaoControleEntrega?.UtilizarPrevisaoEntregaPedidoComoDataPrevista) ?? false && (configuracao?.PermitirAtualizarPrevisaoEntregaPedidoControleEntrega ?? false))
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorPedido(tabelaCargaPedido.CargaPedido.Pedido.Codigo);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarDataPrevisaoEntregaPorEntregaPedido(tabelaCargaPedido.CargaPedido.Pedido, cargaEntrega, unitOfWork);
            }

            repositorioPedido.Atualizar(tabelaCargaPedido.CargaPedido.Pedido);
        }

        #region Frete por Peso

        private decimal RetornarValorBaseMultiplicavel(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados)
        {
            decimal valor = itemParaCalculo.ValorParaCalculo;
            if (tabelaFrete.ParametroBase.HasValue)
            {
                if (tabelaFrete.MultiplicarValorDaFaixa)
                {
                    if (tabelaFrete.ParametroBase.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora)
                    {
                        int fatorMultiplicacaoMinuto = 60;
                        //Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = ObterParametroBaseTabelaFrete(tabelaFreteCliente, parametros);
                        valor = itemParaCalculo.ValorParaCalculo * (parametros.TotalMinutos / fatorMultiplicacaoMinuto);
                        if (dados != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Horas * Valor base por Hora  ", (parametros.TotalMinutos / fatorMultiplicacaoMinuto) + " horas * " + itemParaCalculo.ValorParaCalculo.ToString("n5"), itemParaCalculo.ValorParaCalculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora, itemParaCalculo.TipoValor, 0);
                            dados.ComposicaoFrete.Add(composicao);
                        }
                    }
                }
            }
            return valor;
        }

        private decimal BuscarValorPorPeso(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, bool paraCalcularValorBase, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.PesoTabelaFrete repPesoTabelaFrete = new Repositorio.Embarcador.Frete.PesoTabelaFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> pesoTabelaFrete = repPesoTabelaFrete.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());

            Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete calculoPorValorFixo = pesoTabelaFrete.Where(o => (o.ModeloVeicularCarga == null || o.ModeloVeicularCarga?.Codigo == parametros.ModeloVeiculo?.Codigo) && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.ValorFixoPorPesoTransportado && o.ParaCalcularValorBase == paraCalcularValorBase).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> calculoPorFaixa = pesoTabelaFrete.Where(o => (o.ModeloVeicularCarga == null || o.ModeloVeicularCarga?.Codigo == parametros.ModeloVeiculo?.Codigo) && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.PorFaixaPesoTransportado && o.ParaCalcularValorBase == paraCalcularValorBase).ToList();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa?.FirstOrDefault()?.TabelaFrete;
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = tabelaFrete?.ComponenteFretePeso;
            if (calculoPorValorFixo?.ComponenteFrete != null)
                componente = calculoPorValorFixo.ComponenteFrete;
            else if (calculoPorFaixa?.FirstOrDefault()?.TabelaFrete?.ComponenteFretePeso != null)
                componente = calculoPorFaixa?.FirstOrDefault()?.TabelaFrete?.ComponenteFretePeso;

            decimal pesoTipoEmbalagem = 0;
            if (tabelaFreteCliente.TabelaFrete.ParametroBase == TipoParametroBaseTabelaFrete.TipoEmbalagem)
            {
                pesoTipoEmbalagem = (from obj in parametros.TiposEmbalagem where obj.TipoEmbalagem.Codigo == itens.FirstOrDefault().ParametroBaseCalculo.CodigoObjeto select obj.Peso).Sum();
            }

            decimal valorPorPeso = 0;
            string messagem = string.Empty;

            if (calculoPorValorFixo != null)
            {
                decimal peso = ObterPesoPorUnidadeMedida(parametros, calculoPorValorFixo.UnidadeMedida.UnidadeMedida, pesoTipoEmbalagem);
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == calculoPorValorFixo.Codigo select obj).FirstOrDefault();

                string mensagemPeso = peso.ToString("n5");

                if (tabelaFrete.PesoParametroCalculoFrete == PesoParametroCalculoFrete.ProporcionalCapacidadeModeloVeicular)
                {
                    peso = peso * ObterCalculoCoeficiente(parametros.ModeloVeiculo.CapacidadePesoTransporte, parametros.PesoTotalCarga);
                    messagem = $" {peso} * {ObterCalculoCoeficiente(parametros.ModeloVeiculo.CapacidadePesoTransporte, parametros.PesoTotalCarga).ToString("n4")} ";
                }


                if (tabelaFrete.CalcularFatorPesoPelaKM && parametros.Distancia > 0)
                {
                    valorPorPeso = (itemParaCalculo.ValorParaCalculo * parametros.Distancia) * peso;
                    messagem = parametros.Distancia + " km * " + itemParaCalculo.ValorParaCalculoFormatado + " * " + peso.ToString("n5");
                }
                else
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        valorPorPeso = RetornarValorBaseMultiplicavel(tabelaFrete, tabelaFreteCliente, itemParaCalculo, parametros, ref dados) * peso;
                    }
                    else
                    {
                        valorPorPeso = Math.Round(RetornarValorBaseMultiplicavel(tabelaFrete, tabelaFreteCliente, itemParaCalculo, parametros, ref dados) * peso, 2, MidpointRounding.AwayFromZero);
                    }
                    messagem = itemParaCalculo.ValorParaCalculoFormatado + " * " + peso.ToString("n5");
                }

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por " + calculoPorValorFixo.UnidadeMedida.Descricao + " * " + calculoPorValorFixo.UnidadeMedida.Descricao, messagem, itemParaCalculo.ValorParaCalculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorPeso, 0m, false, componente, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }
                else if (valorPorPeso > 0m)
                    return valorPorPeso;
            }

            if (calculoPorFaixa.Count > 0)
            {
                Dominio.Enumeradores.UnidadeMedida unidadeMedida = calculoPorFaixa[0].UnidadeMedida.UnidadeMedida;

                decimal peso = ObterPesoPorUnidadeMedida(parametros, calculoPorFaixa[0].UnidadeMedida.UnidadeMedida, pesoTipoEmbalagem);

                decimal pesoParaComparacao = ObterPesoParaComparacao(unidadeMedida, peso);

                decimal valorDescontoCubagem = 0;
                decimal pesoAposDesconto = 0;
                if (unidadeMedida == Dominio.Enumeradores.UnidadeMedida.M3 && tabelaFreteCliente.TabelaFrete.DescontoCubagemCalculoFrete > 0)
                {
                    valorDescontoCubagem = pesoParaComparacao * (tabelaFreteCliente.TabelaFrete.DescontoCubagemCalculoFrete / 100);
                    pesoParaComparacao -= valorDescontoCubagem;

                    pesoAposDesconto = pesoParaComparacao;
                }

                Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete faixaPesoSelecionado = null;

                bool necessarioAjudante = parametros.NecessarioAjudante;
                if (paraCalcularValorBase)
                    necessarioAjudante = parametros.ParametrosCarga?.NecessarioAjudante ?? parametros.NecessarioAjudante;

                if (tabelaFrete.PermiteValorAdicionalPorPesoExcedente)
                {
                    if (necessarioAjudante)
                        faixaPesoSelecionado = calculoPorFaixa.Where(o => o.PesoInicial <= pesoParaComparacao && o.PesoFinal >= pesoParaComparacao && o.ComAjudante).FirstOrDefault();
                    else
                        faixaPesoSelecionado = calculoPorFaixa.Where(o => o.PesoInicial <= pesoParaComparacao && o.PesoFinal >= pesoParaComparacao && !o.ComAjudante).FirstOrDefault();

                    if (faixaPesoSelecionado == null)
                        faixaPesoSelecionado = calculoPorFaixa.Where(o => o.PesoInicial <= pesoParaComparacao && o.PesoFinal >= pesoParaComparacao).FirstOrDefault();

                    if (faixaPesoSelecionado == null) //excedeu o peso 
                        faixaPesoSelecionado = calculoPorFaixa.OrderByDescending(o => o.PesoFinal).FirstOrDefault();
                }
                else
                {
                    if (necessarioAjudante)
                        faixaPesoSelecionado = calculoPorFaixa.Where(o => (o.PesoInicial <= pesoParaComparacao || o.PesoInicial == 0m) && (o.PesoFinal >= pesoParaComparacao || o.PesoFinal == 0m) && o.ComAjudante).FirstOrDefault();
                    else
                        faixaPesoSelecionado = calculoPorFaixa.Where(o => (o.PesoInicial <= pesoParaComparacao || o.PesoInicial == 0m) && (o.PesoFinal >= pesoParaComparacao || o.PesoFinal == 0m) && !o.ComAjudante).FirstOrDefault();

                    if (faixaPesoSelecionado == null)
                        faixaPesoSelecionado = calculoPorFaixa.Where(o => (o.PesoInicial <= pesoParaComparacao || o.PesoInicial == 0m) && (o.PesoFinal >= pesoParaComparacao || o.PesoFinal == 0m)).FirstOrDefault();
                }

                if (faixaPesoSelecionado != null)
                {
                    if (faixaPesoSelecionado.ComponenteFrete != null)
                        componente = faixaPesoSelecionado.ComponenteFrete;

                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaPesoSelecionado.Codigo select obj).FirstOrDefault();

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();

                    composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso;

                    string formula = "Valor por ";

                    if (tabelaFrete.MultiplicarValorDaFaixa || faixaPesoSelecionado.CalculoPeso == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorPesoTransportado.Multiplicacao)
                        formula += faixaPesoSelecionado.UnidadeMedida.Descricao;
                    else
                        formula += "Faixa de " + faixaPesoSelecionado.UnidadeMedida.Descricao;

                    formula += " (" + faixaPesoSelecionado.Descricao + ")";

                    string valoresFormula = string.Empty;

                    decimal valorPeso = 0;
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        valorPeso = RetornarValorBaseMultiplicavel(tabelaFrete, tabelaFreteCliente, itemParaCalculo, parametros, ref dados);
                    }
                    else
                    {
                        valorPeso = Math.Round(RetornarValorBaseMultiplicavel(tabelaFrete, tabelaFreteCliente, itemParaCalculo, parametros, ref dados), 2, MidpointRounding.AwayFromZero);
                    }

                    decimal pesoParaCalculo = peso;
                    if (tabelaFrete.PermiteValorAdicionalPorPesoExcedente && faixaPesoSelecionado.PesoFinal.Value > 0m && pesoParaCalculo > faixaPesoSelecionado.PesoFinal.Value)
                        pesoParaCalculo = faixaPesoSelecionado.PesoFinal.Value;

                    if (pesoAposDesconto > 0)
                    {
                        pesoParaCalculo = pesoAposDesconto;
                        peso = pesoAposDesconto;
                    }

                    if (tabelaFrete.MultiplicarValorDaFaixa || faixaPesoSelecionado.CalculoPeso == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorPesoTransportado.Multiplicacao)
                    {
                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            valorPorPeso = valorPeso * pesoParaCalculo;
                        }
                        else
                        {
                            valorPorPeso = Math.Round(valorPeso * pesoParaCalculo, 2, MidpointRounding.AwayFromZero);
                        }

                        valoresFormula = pesoParaCalculo.ToString("n5") + " " + faixaPesoSelecionado.UnidadeMedida.Descricao + "(s) * " + valorPeso.ToString("n2");
                    }
                    else
                    {
                        valorPorPeso = valorPeso;
                        valoresFormula = peso.ToString("n5") + " " + faixaPesoSelecionado.UnidadeMedida.Descricao + "(s) = " + valorPeso.ToString("n2");
                    }

                    if (valorDescontoCubagem > 0)
                        valoresFormula += $" (com desconto na fórmula {tabelaFreteCliente.TabelaFrete.DescontoCubagemCalculoFrete} %)";

                    if (dados != null)
                    {
                        decimal valorExcedenteAdicionar = 0m;
                        decimal valorTotal = valorPorPeso;
                        if (tabelaFrete.PermiteValorAdicionalPorPesoExcedente && faixaPesoSelecionado.PesoFinal.Value > 0m && pesoParaComparacao > faixaPesoSelecionado.PesoFinal.Value)
                        {
                            decimal pesoExcedente = peso - faixaPesoSelecionado.PesoFinal.Value;

                            decimal valorPorPesoExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorPesoExcedente ?? itemParaCalculo.TabelaFrete.ValorPesoExcedente;

                            decimal valorExcedente = 0m;

                            if (tabelaFrete.PesoExcecente > 0m)
                                valorExcedente = Math.Round((pesoExcedente / tabelaFrete.PesoExcecente) * valorPorPesoExcedente, 2, MidpointRounding.AwayFromZero);

                            if (valorExcedente > 0m)
                            {
                                formula += " + " + valorPorPesoExcedente.ToString("n2") + " a cada " + tabelaFrete.PesoExcecente.ToString("n2") + " " + faixaPesoSelecionado.UnidadeMedida.Descricao + " Excedente";
                                valoresFormula += " + (" + pesoExcedente.ToString("n5") + " / " + tabelaFrete.PesoExcecente + ") * " + valorPorPesoExcedente.ToString("n2");
                                valorExcedenteAdicionar = valorExcedente;
                                valorTotal += valorExcedente;
                            }
                        }

                        composicao.ValoresFormula = valoresFormula;
                        composicao.Formula = formula;
                        composicao.Valor = valorPorPeso;

                        SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorPeso, valorExcedenteAdicionar, paraCalcularValorBase, componente, tabelaFreteCliente, unitOfWork);

                        if (!paraCalcularValorBase)
                            dados.ComposicaoFrete.Add(composicao);
                        else
                            dados.ComposicaoValorBaseFrete.Add(composicao);

                        valorPorPeso += valorExcedenteAdicionar;
                    }
                }

            }
            return valorPorPeso;
        }

        private decimal ObterCubagem(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros)
        {
            return parametros.Cubagem;
        }

        private decimal ObterPesoPorUnidadeMedida(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Enumeradores.UnidadeMedida unidadeMedida, decimal pesoTipoEmbalagem = 0)
        {
            bool utilizarPesoCubado = false, utilizarPesoPaletizado = false;
            decimal pesoCubado = 0m, pesoPaletizado = 0m;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem? tipoUsoFatorCubagem = null;

            decimal parametroPeso = parametros.Peso;
            if (pesoTipoEmbalagem > 0)
                parametroPeso = pesoTipoEmbalagem;

            if (parametros.TipoOperacao != null)
            {
                if (parametros.TipoOperacao.UtilizarFatorCubagem)
                {
                    pesoCubado = parametros.PesoCubado;

                    tipoUsoFatorCubagem = parametros.TipoOperacao.TipoUsoFatorCubagem;

                    if (!tipoUsoFatorCubagem.HasValue)
                        tipoUsoFatorCubagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria;

                    utilizarPesoCubado = true;
                }

                if (parametros.TipoOperacao.UtilizarPaletizacao)
                {
                    pesoPaletizado = parametros.PesoPaletizado;
                    utilizarPesoPaletizado = true;
                }
            }

            if ((unidadeMedida == Dominio.Enumeradores.UnidadeMedida.KG) || (unidadeMedida == Dominio.Enumeradores.UnidadeMedida.TON))
            {
                decimal peso = parametroPeso;

                //if (pedidoXMLNotasFiscais != null && pedidoXMLNotasFiscais.Count > 0)
                //    peso = pedidoXMLNotasFiscais.Sum(o => o.XMLNotaFiscal.Peso);
                //else
                //{
                //    peso = ObterQuilosTotaisParaQuilos(cargaPedidoQuantidades);

                //    if (peso <= 0m)
                //        peso = cargaPedidos.Sum(o => o.Peso);
                //}

                if (parametros.CalcularFretePorPesoCubado)
                {
                    if (parametros.IsencaoCubagem > 0)
                    {
                        if (parametros.PesoCubado > parametros.IsencaoCubagem)
                        {
                            if (parametros.AplicarMaiorValorEntrePesoEPesoCubado && parametros.PesoCubado > parametroPeso)
                                peso = parametros.PesoCubado;
                            else if (!parametros.AplicarMaiorValorEntrePesoEPesoCubado)
                                peso = parametros.PesoCubado;
                        }
                    }
                    else if (parametros.AplicarMaiorValorEntrePesoEPesoCubado && parametros.PesoCubado > parametroPeso)
                        peso = parametros.PesoCubado;
                    else if (!parametros.AplicarMaiorValorEntrePesoEPesoCubado)
                        peso = parametros.PesoCubado;
                }
                else if (utilizarPesoPaletizado && pesoPaletizado > 0m)
                    peso = pesoPaletizado;
                else if (utilizarPesoCubado && (tipoUsoFatorCubagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem.SempreUtilizarPesoConvertido || (tipoUsoFatorCubagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria && pesoCubado > peso)))
                    peso = pesoCubado;

                if (unidadeMedida == Dominio.Enumeradores.UnidadeMedida.TON)
                    return (peso > parametros.PesoCubado ? peso : parametros.PesoCubado) / 1000;

                return (peso > parametros.PesoCubado ? peso : parametros.PesoCubado);
            }
            else if (unidadeMedida == Dominio.Enumeradores.UnidadeMedida.LT || unidadeMedida == Dominio.Enumeradores.UnidadeMedida.M3 || unidadeMedida == Dominio.Enumeradores.UnidadeMedida.MMBTU)
            {
                if (parametros.Quantidades != null)
                    return (from obj in parametros.Quantidades where obj.UnidadeMedida == unidadeMedida select obj.Quantidade).Sum();
            }
            else if (unidadeMedida == Dominio.Enumeradores.UnidadeMedida.UN)
            {
                if (parametros.Volumes > 0)
                    return parametros.Volumes;

                if (parametros.Quantidades != null)
                    return (from obj in parametros.Quantidades where obj.UnidadeMedida == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum();
            }

            return 0m;
        }

        public decimal ObterQuilosTotaisParaQuilos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades)
        {
            decimal pesoEmQuilos = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.KG select obj.Quantidade).Sum();
            decimal pesoEmToneladas = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.TON select obj.Quantidade).Sum();
            pesoEmQuilos += pesoEmToneladas * 1000;

            if (pesoEmQuilos == 0)
                pesoEmQuilos = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum();

            return pesoEmQuilos;
        }

        public decimal ObterQuilosTotaisParaQuilos(List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> cargaPedidoQuantidades)
        {
            decimal pesoEmQuilos = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.KG select obj.Quantidade).Sum();
            decimal pesoEmToneladas = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.TON select obj.Quantidade).Sum();
            pesoEmQuilos += pesoEmToneladas * 1000;

            if (pesoEmQuilos == 0)
                pesoEmQuilos = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum();

            return pesoEmQuilos;
        }

        private decimal ObterPesoParaComparacao(Dominio.Enumeradores.UnidadeMedida unidadeMedida, decimal pesoReal)
        {
            //arredonda sempre para duas casas decimais para baixo quando for tonelada apenas para comparação.
            //if (unidadeMedida == Dominio.Enumeradores.UnidadeMedida.TON)
            //    return (Math.Floor(pesoReal * 100) / 100);
            //else
            return pesoReal;
        }

        #endregion Frete por Peso

        #region Frete por Distância

        private decimal BuscarValorPorDistancia(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.DistanciaTabelaFrete repDistanciaTabelaFrete = new Repositorio.Embarcador.Frete.DistanciaTabelaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete> distanciasTabelaFrete = repDistanciaTabelaFrete.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());

            Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete calculoPorValorFixo = distanciasTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete.ValorFixoPorDistanciaPercorrida).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete> calculoPorFaixa = distanciasTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete.PorFaixaDistanciaPercorrida).ToList();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa.FirstOrDefault()?.TabelaFrete;

            decimal valorPorKM = 0m;

            decimal parametroDistancia = parametros.Distancia;
            string sigla = "KM";
            string unidade = "Distancia";

            if (tabelaFrete?.UsarCubagemComoParametroDeDistancia ?? false)
            {
                parametroDistancia = parametros.Cubagem;
                sigla = "m²";
                unidade = "Cubagem";
            }


            if (calculoPorValorFixo != null)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == calculoPorValorFixo.Codigo select obj).FirstOrDefault();

                if (tabelaFrete.MultiplicarValorPorPallet && parametros.NumeroPallets > 0)
                    valorPorKM = (itemParaCalculo.ValorParaCalculo * parametroDistancia) * parametros.NumeroPallets;
                else
                    valorPorKM = itemParaCalculo.ValorParaCalculo * parametroDistancia;

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por " + sigla + " * " + unidade, (tabelaFrete.MultiplicarValorPorPallet && parametros.NumeroPallets > 0) ? parametros.NumeroPallets.ToString("n2") + " palles * " + itemParaCalculo.ValorParaCalculoFormatado + " * " + parametroDistancia.ToString("n2") : itemParaCalculo.ValorParaCalculoFormatado + " * " + parametroDistancia.ToString("n2"), valorPorKM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorKM, 0m, false, tabelaFrete.ComponenteFreteQuilometragem, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }
                else if (valorPorKM > 0m)
                    return valorPorKM;
            }

            if (calculoPorFaixa.Count > 0)
            {
                Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete faixaDistanciaSelecionada = null;

                if (tabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente)
                {
                    faixaDistanciaSelecionada = calculoPorFaixa.Where(o => o.QuilometragemInicial <= parametroDistancia && o.QuilometragemFinal >= parametroDistancia).FirstOrDefault();

                    if (faixaDistanciaSelecionada == null) //excedeu a distância 
                        faixaDistanciaSelecionada = calculoPorFaixa.OrderByDescending(o => o.QuilometragemFinal).FirstOrDefault();
                }
                else
                {
                    faixaDistanciaSelecionada = calculoPorFaixa.Where(o => (o.QuilometragemInicial <= parametroDistancia || o.QuilometragemInicial == 0m) && (o.QuilometragemFinal >= parametroDistancia || o.QuilometragemFinal == 0m)).FirstOrDefault();
                }

                if (faixaDistanciaSelecionada != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                    composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia;

                    string formula = "Valor por ";

                    if (tabelaFrete.MultiplicarValorPorPallet && parametros.NumeroPallets > 0)
                        formula += "Número de Pallets (" + parametros.NumeroPallets.ToString("n2") + ") * ";

                    if (faixaDistanciaSelecionada.MultiplicarValorDaFaixa)
                        formula += (unidade + " * ");

                    if (distanciasTabelaFrete.Any(o => o.TabelaFrete.Codigo == tabelaFrete.Codigo && (o.MultiplicarPeloResultadoDaDistancia || (o.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga.HasValue && o.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga.Value))))
                        formula += " (Valor faixa de " + sigla + " (" + faixaDistanciaSelecionada.Descricao + ")";
                    else
                        formula += " Valor faixa de " + sigla + " (" + faixaDistanciaSelecionada.Descricao + ")";

                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaDistanciaSelecionada.Codigo select obj).FirstOrDefault();
                    string valoresFormula = parametroDistancia.ToString("n2") + " " + sigla + " = " + itemParaCalculo.ValorParaCalculoFormatado;

                    if (tabelaFrete.MultiplicarValorDaFaixa || faixaDistanciaSelecionada.MultiplicarValorDaFaixa)
                    {
                        valorPorKM = parametroDistancia * itemParaCalculo.ValorParaCalculo;
                        valoresFormula = parametroDistancia.ToString("n2") + " * " + itemParaCalculo.ValorParaCalculoFormatado;
                    }
                    else
                        valorPorKM = itemParaCalculo.ValorParaCalculo;

                    if (tabelaFrete.MultiplicarValorPorPallet && parametros.NumeroPallets > 0)
                    {
                        valoresFormula += " {Valor por " + sigla + " (" + valorPorKM.ToString("n2") + ") * Número de pallets (" + parametros.NumeroPallets.ToString("n2") + ")} = " + (valorPorKM * parametros.NumeroPallets).ToString("n2");
                        valorPorKM = valorPorKM * parametros.NumeroPallets;
                    }

                    if (distanciasTabelaFrete.Any(o => o.TabelaFrete.Codigo == tabelaFrete.Codigo) && parametros.PesoTotalCarga > 0 && itemParaCalculo.ValorParaCalculo > 0 && itemParaCalculo.ParametroBaseCalculo != null)
                    {
                        if (distanciasTabelaFrete.Any(o => o.MultiplicarPeloResultadoDaDistancia))
                        {
                            decimal valorDistancia = parametroDistancia * itemParaCalculo.ValorParaCalculo;

                            formula += " + Valor Base) * Peso Total da Carga";
                            valoresFormula = " (Valor Distância" + " (" + valorDistancia.ToString("n2") + ") + Valor Base (" + itemParaCalculo.ParametroBaseCalculo.ValorBase.ToString("n2") + ")) * Peso Total da Carga (" + parametros.PesoTotalCarga.ToString("n2") + ") = " + ((valorDistancia + itemParaCalculo.ParametroBaseCalculo.ValorBase) * (parametros.PesoTotalCarga / 1000)).ToString("n2");
                            valorPorKM = (valorDistancia + itemParaCalculo.ParametroBaseCalculo.ValorBase) * (parametros.PesoTotalCarga / 1000);

                            parametros.MultiplicarPeloResultadoDaDistancia = true;
                        }

                        if (distanciasTabelaFrete.Any(o => o.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga.HasValue && o.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga.Value))
                        {
                            decimal valorDistancia = itemParaCalculo.ValorParaCalculo;

                            formula += " * Peso Total da Carga";
                            valorPorKM = valorDistancia * parametros.PesoTotalCarga;
                            valoresFormula = $"Valor Distância ({valorDistancia:n2}) * Peso Total da Carga ({parametros.PesoTotalCarga:n2}) = {valorPorKM:n2}";

                            parametros.MultiplicarValorFixoFaixaDistanciaPeloPesoCarga = true;
                        }
                    }

                    if (dados != null)
                    {
                        decimal valorExcedenteAdicionar = 0m;
                        decimal valorTotal = valorPorKM;

                        if (faixaDistanciaSelecionada.QuilometragemFinal.Value > 0m && parametroDistancia > faixaDistanciaSelecionada.QuilometragemFinal.Value)
                        {
                            decimal distanciaExcedente = parametroDistancia - faixaDistanciaSelecionada.QuilometragemFinal.Value;

                            decimal valorQuilometragemExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorQuilometragemExcedente ?? itemParaCalculo.TabelaFrete?.ValorQuilometragemExcedente ?? 0m;

                            decimal valorExcedente = 0m;

                            if (tabelaFrete.QuilometragemExcedente > 0m)
                                valorExcedente = (distanciaExcedente / tabelaFrete.QuilometragemExcedente) * valorQuilometragemExcedente;

                            if (valorExcedente > 0m)
                            {
                                formula += " + " + valorQuilometragemExcedente.ToString("n2") + " a cada " + tabelaFrete.QuilometragemExcedente.ToString("n2") + " " + sigla + " Excedente * " + sigla + " Excedente";
                                valoresFormula += " + (" + distanciaExcedente.ToString("n2") + " / " + tabelaFrete.QuilometragemExcedente + ") * " + valorQuilometragemExcedente.ToString("n2");
                                valorExcedenteAdicionar = valorExcedente;
                                valorTotal += valorExcedente;
                            }
                        }

                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, formula, valoresFormula, valorTotal);
                        if (tabelaFrete.ComponenteFreteQuilometragemExcedente != null)
                        {
                            SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorKM, 0, false, tabelaFrete.ComponenteFreteQuilometragem, tabelaFreteCliente, unitOfWork);
                            SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorExcedenteAdicionar, 0, false, tabelaFrete.ComponenteFreteQuilometragemExcedente, tabelaFreteCliente, unitOfWork);
                        }
                        else
                            SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorKM, valorExcedenteAdicionar, false, tabelaFrete.ComponenteFreteQuilometragem, tabelaFreteCliente, unitOfWork);
                        dados.ComposicaoFrete.Add(composicao);
                    }
                }
            }

            return valorPorKM;
        }

        #endregion Frete por Distância

        #region Frete por Número de Entregas

        private decimal BuscarValorPorEntrega(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.NumeroEntregaTabelaFrete repNumeroEntregaTabelaFrete = new Repositorio.Embarcador.Frete.NumeroEntregaTabelaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete> numeroEntregaTabelaFrete = repNumeroEntregaTabelaFrete.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());

            Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete calculoPorValorFixo = numeroEntregaTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete.ValorFixoPorEntrega).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete> calculoPorFaixa = numeroEntregaTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroEntregaTabelaFrete.PorFaixaEntrega).ToList();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa.FirstOrDefault()?.TabelaFrete;

            decimal valorPorEntrega = 0m;
            decimal numeroEntregas = parametros.NumeroEntregas;

            if (tabelaFrete != null && tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos &&
                (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido ||
                tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                && parametros.ParametrosCarga != null)
                numeroEntregas = parametros.ParametrosCarga.NumeroEntregas;

            if (calculoPorValorFixo != null)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == calculoPorValorFixo.Codigo select obj).FirstOrDefault();

                valorPorEntrega = itemParaCalculo.ValorParaCalculo * numeroEntregas;

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Entrega * Entregas", itemParaCalculo.ValorParaCalculoFormatado + " * " + numeroEntregas.ToString("n2"), valorPorEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorEntrega, 0m, tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos, tabelaFrete.ComponenteFreteNumeroEntregas, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }

                else if (valorPorEntrega > 0m)
                    return valorPorEntrega;
            }

            if (calculoPorFaixa.Count > 0)
            {
                Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete faixaEntregaSelecionada = null;

                bool necessarioAjudante = parametros.NecessarioAjudante;
                if (tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
                    necessarioAjudante = parametros.ParametrosCarga?.NecessarioAjudante ?? parametros.NecessarioAjudante;

                if (tabelaFrete.PermiteValorAdicionalPorEntregaExcedente)
                {
                    if (necessarioAjudante)
                        faixaEntregaSelecionada = calculoPorFaixa.Where(o => o.NumeroInicialEntrega <= numeroEntregas && o.NumeroFinalEntrega >= numeroEntregas && o.ComAjudante).FirstOrDefault();
                    else
                        faixaEntregaSelecionada = calculoPorFaixa.Where(o => o.NumeroInicialEntrega <= numeroEntregas && o.NumeroFinalEntrega >= numeroEntregas && !o.ComAjudante).FirstOrDefault();

                    if (faixaEntregaSelecionada == null)
                        faixaEntregaSelecionada = calculoPorFaixa.Where(o => o.NumeroInicialEntrega <= numeroEntregas && o.NumeroFinalEntrega >= numeroEntregas).FirstOrDefault();

                    if (faixaEntregaSelecionada == null) //excedeu o número de entregas 
                        faixaEntregaSelecionada = calculoPorFaixa.OrderByDescending(o => o.NumeroFinalEntrega).FirstOrDefault();
                }
                else
                {
                    if (necessarioAjudante)
                        faixaEntregaSelecionada = calculoPorFaixa.Where(o => (o.NumeroInicialEntrega <= numeroEntregas || o.NumeroInicialEntrega == 0m) && (o.NumeroFinalEntrega >= numeroEntregas || o.NumeroFinalEntrega == 0m) && o.ComAjudante).FirstOrDefault();
                    else
                        faixaEntregaSelecionada = calculoPorFaixa.Where(o => (o.NumeroInicialEntrega <= numeroEntregas || o.NumeroInicialEntrega == 0m) && (o.NumeroFinalEntrega >= numeroEntregas || o.NumeroFinalEntrega == 0m) && !o.ComAjudante).FirstOrDefault();

                    if (faixaEntregaSelecionada == null)
                        faixaEntregaSelecionada = calculoPorFaixa.Where(o => (o.NumeroInicialEntrega <= numeroEntregas || o.NumeroInicialEntrega == 0m) && (o.NumeroFinalEntrega >= numeroEntregas || o.NumeroFinalEntrega == 0m)).FirstOrDefault();
                }

                if (faixaEntregaSelecionada != null)
                {

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                    composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega;

                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaEntregaSelecionada.Codigo select obj).FirstOrDefault();

                    string valorMultiplicacao = "";
                    string formulaMultiplicacao = "";
                    if (!tabelaFrete.MultiplicarValorDaFaixa)
                        valorPorEntrega = itemParaCalculo.ValorParaCalculo;
                    else
                    {
                        valorPorEntrega = itemParaCalculo.ValorParaCalculo * numeroEntregas;
                        valorMultiplicacao = " * " + numeroEntregas.ToString();
                        formulaMultiplicacao = " * Número de Entregas";
                    }

                    string formula = string.Empty;
                    string valoresFormula = string.Empty;
                    if (tabelaFrete.CalcularValorEntregaPorPercentualFrete)
                        formula = "Percentual do frete por entrega (" + faixaEntregaSelecionada.Descricao + ")";
                    else if (tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                        formula = "Percentual do frete + componentes por entrega (" + faixaEntregaSelecionada.Descricao + ")";
                    else
                    {
                        formula = "Valor por Faixa de entrega (" + faixaEntregaSelecionada.Descricao + ")" + formulaMultiplicacao;
                        valoresFormula = numeroEntregas + " Entrega(s) = " + itemParaCalculo.ValorParaCalculo.ToString("n2") + valorMultiplicacao;
                    }

                    if (dados != null)
                    {
                        decimal valorTotal = valorPorEntrega;
                        decimal valorExcedenteAdicionar = 0m;

                        if (tabelaFrete.CalcularValorEntregaPorPercentualFrete || tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                        {
                            decimal numeroEntregasExcedente = 0;
                            if (faixaEntregaSelecionada.NumeroInicialEntrega.Value > 0 && faixaEntregaSelecionada.NumeroFinalEntrega.Value >= 0)
                                numeroEntregasExcedente = numeroEntregas - faixaEntregaSelecionada.NumeroInicialEntrega.Value + 1;

                            if (dados.ValorFrete > 0 && numeroEntregasExcedente > 0 && tabelaFrete.CalcularValorEntregaPorPercentualFrete)
                            {
                                valorPorEntrega = dados.ValorFrete * ((numeroEntregasExcedente * valorPorEntrega) / 100);

                                valoresFormula = numeroEntregas + " Entrega(s) = " + (numeroEntregasExcedente * valorTotal).ToString("n2") + "%";
                            }
                            else if (dados.ValorTotal > 0 && numeroEntregasExcedente > 0 && tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                            {
                                valorPorEntrega = dados.ValorTotal * ((numeroEntregasExcedente * valorPorEntrega) / 100);

                                valoresFormula = numeroEntregas + " Entrega(s) = " + (numeroEntregasExcedente * valorTotal).ToString("n2") + "%";
                            }
                            else
                            {
                                valorTotal = 0;
                                valorPorEntrega = 0;
                            }
                        }
                        else if (faixaEntregaSelecionada.NumeroFinalEntrega.Value > 0m && numeroEntregas > faixaEntregaSelecionada.NumeroFinalEntrega.Value)
                        {
                            decimal numeroEntregasExcedente = numeroEntregas - faixaEntregaSelecionada.NumeroFinalEntrega.Value;

                            decimal valorPorEntregaExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorEntregaExcedente ?? itemParaCalculo.TabelaFrete?.ValorEntregaExcedente ?? 0m;

                            decimal valorExcedente = numeroEntregasExcedente * valorPorEntregaExcedente;

                            if (valorExcedente > 0m)
                            {
                                formula += " + Valor Por Entrega Excedente * Entregas Excedentes";
                                valoresFormula += " + " + valorPorEntregaExcedente.ToString("n2") + " * " + numeroEntregasExcedente.ToString("n2");
                                valorExcedenteAdicionar = valorExcedente;
                                valorTotal += valorExcedente;
                            }
                        }

                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, formula, valoresFormula, valorTotal);
                        SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorEntrega, valorExcedenteAdicionar, tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos, tabelaFrete.ComponenteFreteNumeroEntregas, tabelaFreteCliente, unitOfWork);
                        if (!tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
                            dados.ComposicaoFrete.Add(composicao);
                        else
                            dados.ComposicaoValorBaseFrete.Add(composicao);
                    }
                }
            }

            return valorPorEntrega;
        }

        #endregion Frete por Número de Entregas

        #region Frete por Pallet

        private decimal BuscarValorPorPallets(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.PalletTabelaFrete repPalletTabelaFrete = new Repositorio.Embarcador.Frete.PalletTabelaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete> palletTabelaFrete = repPalletTabelaFrete.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());

            Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete calculoPorValorFixo = palletTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete.ValorFixoPorPallet).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete> calculoPorFaixa = palletTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNumeroPalletsTabelaFrete.PorFaixaPallets).ToList();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa.FirstOrDefault()?.TabelaFrete;

            decimal valorPorPallet = 0m;
            decimal numeroPallet = parametros.NumeroPallets;


            if (itens.Any(o => o.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima))
            {
                decimal valorMath = Math.Ceiling(numeroPallet);
                numeroPallet = valorMath;
            }

            if (calculoPorValorFixo != null)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == calculoPorValorFixo.Codigo select obj).FirstOrDefault();

                valorPorPallet = itemParaCalculo.ValorParaCalculo * numeroPallet;

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Pallet * Número de Pallets", itemParaCalculo.ValorParaCalculoFormatado + " * " + parametros.NumeroPallets.ToString("n5"), valorPorPallet, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorPallet, 0m, false, tabelaFrete.ComponenteFretePallet, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }

                else if (valorPorPallet > 0m)
                    return valorPorPallet;
            }

            if (calculoPorFaixa.Count > 0)
            {
                Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete faixaPalletSelecionado = null;

                if (tabelaFrete.PermiteValorAdicionalPorPalletExcedente)
                {
                    faixaPalletSelecionado = calculoPorFaixa.Where(o => o.NumeroInicialPallet <= numeroPallet && o.NumeroFinalPallet >= numeroPallet).FirstOrDefault();

                    if (faixaPalletSelecionado == null) //excedeu o número de entregas 
                        faixaPalletSelecionado = calculoPorFaixa.OrderByDescending(o => o.NumeroFinalPallet).FirstOrDefault();
                }
                else
                {
                    faixaPalletSelecionado = calculoPorFaixa.Where(o => (o.NumeroInicialPallet <= numeroPallet || o.NumeroInicialPallet == 0m) && (o.NumeroFinalPallet >= numeroPallet || o.NumeroFinalPallet == 0m)).FirstOrDefault();
                }

                if (faixaPalletSelecionado != null)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaPalletSelecionado.Codigo select obj).FirstOrDefault();

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                    composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets;

                    string formula = "Valor por Faixa de Pallets (" + faixaPalletSelecionado.Descricao + ")";

                    valorPorPallet = itemParaCalculo.ValorParaCalculo;

                    string valoresFormula = numeroPallet.ToString("n5") + " Pallet(s) = " + valorPorPallet.ToString("n2");

                    if (dados != null)
                    {
                        decimal valorExcedenteAdicionar = 0m;
                        decimal valorTotal = valorPorPallet;
                        if (faixaPalletSelecionado.NumeroFinalPallet.Value > 0m && numeroPallet > faixaPalletSelecionado.NumeroFinalPallet.Value)
                        {
                            decimal numeroPalletsExcedente = numeroPallet - faixaPalletSelecionado.NumeroFinalPallet.Value;

                            decimal valorPorPalletExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorPalletExcedente ?? itemParaCalculo.TabelaFrete?.ValorPalletExcedente ?? 0m;

                            decimal valorExcedente = numeroPalletsExcedente * valorPorPalletExcedente;

                            if (valorExcedente > 0m)
                            {
                                formula += " + Valor Por Pallet Excedente * Pallets Excedentes";
                                valoresFormula += " + " + valorPorPalletExcedente.ToString("n2") + " * " + numeroPalletsExcedente.ToString("n5");
                                valorExcedenteAdicionar = valorExcedente;
                                valorTotal += valorExcedente;
                            }
                        }

                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, formula, valoresFormula, valorTotal);
                        SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorPallet, valorExcedenteAdicionar, false, tabelaFrete.ComponenteFretePallet, tabelaFreteCliente, unitOfWork);
                        dados.ComposicaoFrete.Add(composicao);
                    }
                }
            }
            return valorPorPallet;
        }

        #endregion Frete por Pallet

        #region Frete por Tempo 

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo ObterParametrosCalculoFreteTempo(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork)
        {
            if (!tabelaFreteCliente.TabelaFrete.Tempos.Any())
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametroCalculoFreteTempo = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo()
            {
                ComponenteFrete = tabelaFreteCliente.TabelaFrete.ComponenteFreteTempo,
                DataInicial = parametros.DataInicialViagem,
                DataFinal = parametros.DataFinalViagem,
                HorasMinimasCobranca = tabelaFreteCliente.TabelaFrete.HorasMinimasCobranca,
                MinutosArredondamentoHoras = tabelaFreteCliente.TabelaFrete.MinutosArredondamentoHoras,
                MultiplicarPorHora = tabelaFreteCliente.TabelaFrete.MultiplicarValorTempoPorHora,
                PossuiHorasMinimasCobranca = tabelaFreteCliente.TabelaFrete.PossuiHorasMinimasCobranca,
                UtilizarArredondamentoHoras = tabelaFreteCliente.TabelaFrete.UtilizarArredondamentoHoras,
                UtilizarMinutosInformadosComoCorteArredondamentoHoraExata = tabelaFreteCliente.TabelaFrete.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata,
                SumarizarTempoDadosFrete = true,
                Faixas = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa>()
            };

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo faixaTempo in tabelaFreteCliente.TabelaFrete.Tempos)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemTempo = (from obj in itens where obj.CodigoObjeto == faixaTempo.Codigo && obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo select obj).FirstOrDefault();

                if (itemTempo == null)
                    continue;

                parametroCalculoFreteTempo.Faixas.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa()
                {
                    HoraFinal = faixaTempo.HoraFinal,
                    HoraFinalCobrancaMinima = faixaTempo.HoraFinalCobrancaMinima,
                    HoraInicial = faixaTempo.HoraInicial,
                    HoraInicialCobrancaMinima = faixaTempo.HoraInicialCobrancaMinima,
                    PeriodoInicial = faixaTempo.PeriodoInicial,
                    TipoCampoValorTabelaFrete = itemTempo.TipoValor,
                    Valor = itemTempo.ValorParaCalculo
                });
            }

            return parametroCalculoFreteTempo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo ObterParametrosCalculoFreteTempo(Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteFrete, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork)
        {
            if (componenteFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Tempo)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametroCalculoFreteTempo = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo()
            {
                ComponenteFrete = componenteFrete.ComponenteFrete,
                DataInicial = parametros.DataInicialViagem,
                DataFinal = parametros.DataFinalViagem,
                HorasMinimasCobranca = componenteFrete.HorasMinimasCobrancaTempo,
                MinutosArredondamentoHoras = componenteFrete.MinutosArredondamentoHorasTempo,
                MultiplicarPorHora = componenteFrete.MultiplicarPorHoraTempo,
                MultiplicarPorAjudante = componenteFrete.MultiplicarPorAjudante,
                MultiplicarPorDeslocamento = componenteFrete.MultiplicarPorDeslocamento,
                MultiplicarPorDiaria = componenteFrete.MultiplicarPorDiaria,
                MultiplicarPorEntrega = componenteFrete.MultiplicarPorEntrega,
                PossuiHorasMinimasCobranca = componenteFrete.PossuiHorasMinimasCobrancaTempo,
                UtilizarArredondamentoHoras = componenteFrete.UtilizarArredondamentoHorasTempo,
                SumarizarTempoDadosFrete = false,
                Faixas = componenteFrete.Tempos.Select(faixaTempo => new Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa()
                {
                    HoraFinal = faixaTempo.HoraFinal,
                    HoraFinalCobrancaMinima = faixaTempo.HoraFinalCobrancaMinima,
                    HoraInicial = faixaTempo.HoraInicial,
                    HoraInicialCobrancaMinima = faixaTempo.HoraInicialCobrancaMinima,
                    PeriodoInicial = faixaTempo.PeriodoInicial,
                    TipoCampoValorTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor,
                    Valor = faixaTempo.Valor
                }).ToList()
            };

            return parametroCalculoFreteTempo;
        }

        private decimal BuscarValorPorTempo(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametroCalculoTempo, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Repositorio.UnitOfWork unitOfWork)
        {
            if (parametroCalculoTempo == null || !parametroCalculoTempo.DataInicial.HasValue || !parametroCalculoTempo.DataFinal.HasValue || parametroCalculoTempo.DataInicial >= parametroCalculoTempo.DataFinal)
                return 0m;

            decimal valorPorTempo = 0;

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa periodoInicial = parametroCalculoTempo.Faixas.Where(o => o.PeriodoInicial).FirstOrDefault();

            if (periodoInicial == null)
                periodoInicial = parametroCalculoTempo.Faixas.OrderBy(o => o.HoraInicial).FirstOrDefault();

            List<DateTime> datasJaCobradasExcedente = new List<DateTime>();

            for (var i = 0; i < parametroCalculoTempo.Faixas.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa faixaTempo = parametroCalculoTempo.Faixas[i];

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo;

                decimal fatorMultiplicacao = ObterFatorMultiplicacaoPorFaixaTempo(ref dados, parametros, parametroCalculoTempo, composicao, periodoInicial, faixaTempo, ref datasJaCobradasExcedente, out bool cobrarHoraMinimaEmOutroPeriodo);

                if (fatorMultiplicacao <= 0m)
                    continue;

                valorPorTempo = faixaTempo.Valor * fatorMultiplicacao;

                composicao.Valor = faixaTempo.Valor;
                composicao.ValoresFormula = faixaTempo.Valor.ToString("n2") + " * " + fatorMultiplicacao.ToString("n2");

                if (parametroCalculoTempo.MultiplicarPorAjudante)
                {
                    composicao.ValoresFormula += " * " + parametros.NumeroAjudantes.ToString();
                    valorPorTempo *= parametros.NumeroAjudantes;
                }

                if (parametroCalculoTempo.MultiplicarPorDeslocamento)
                {
                    composicao.ValoresFormula += " * " + parametros.NumeroDeslocamento.ToString();
                    valorPorTempo *= parametros.NumeroDeslocamento;
                }

                if (parametroCalculoTempo.MultiplicarPorDiaria)
                {
                    composicao.ValoresFormula += " * " + parametros.NumeroDiarias.ToString();
                    valorPorTempo *= parametros.NumeroDiarias;
                }

                if (parametroCalculoTempo.MultiplicarPorEntrega)
                {
                    composicao.ValoresFormula += " * " + parametros.NumeroEntregas.ToString();
                    valorPorTempo *= parametros.NumeroEntregas;
                }

                if (dados != null)
                {
                    SetarValorPorTipoEValor(ref dados, ref composicao, parametros, faixaTempo.TipoCampoValorTabelaFrete, valorPorTempo, 0m, false, parametroCalculoTempo.ComponenteFrete, tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete ? tabelaFreteCliente.TabelaFrete.IncluirICMSValorFrete : tabelaFreteCliente.IncluirICMSValorFrete, unitOfWork);

                    dados.ComposicaoFrete.Add(composicao);
                }
                else if (valorPorTempo > 0m)
                    return valorPorTempo;

                if (cobrarHoraMinimaEmOutroPeriodo)
                {
                    if (!datasJaCobradasExcedente.Contains(parametroCalculoTempo.DataInicial.Value))
                    {
                        for (var l = 0; l < parametroCalculoTempo.Faixas.Count; l++)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa faixaHoraMinima = parametroCalculoTempo.Faixas[l];

                            if (faixaHoraMinima.HoraInicialCobrancaMinima.HasValue && faixaHoraMinima.HoraFinalCobrancaMinima.HasValue)
                            {
                                if (VerificarSeEstaEntreFaixaDeHorarios(parametroCalculoTempo.DataInicial.Value.TimeOfDay, faixaHoraMinima.HoraInicialCobrancaMinima.Value, faixaHoraMinima.HoraFinalCobrancaMinima.Value))
                                {
                                    DateTime dataInicialHoraMinima = new DateTime(1990, 9, 8, faixaHoraMinima.HoraInicialCobrancaMinima.Value.Hours, faixaHoraMinima.HoraInicialCobrancaMinima.Value.Minutes, faixaHoraMinima.HoraInicialCobrancaMinima.Value.Seconds);
                                    DateTime dataFinalHoraMinima = new DateTime(1990, 9, 8, faixaHoraMinima.HoraFinalCobrancaMinima.Value.Hours, faixaHoraMinima.HoraFinalCobrancaMinima.Value.Minutes, faixaHoraMinima.HoraFinalCobrancaMinima.Value.Seconds);

                                    TimeSpan horasDiferenca = (dataFinalHoraMinima - dataInicialHoraMinima);

                                    decimal fatorMultiplicacaoHoraMinima = ObterFatorMultiplicacaoTempo(parametroCalculoTempo.MultiplicarPorHora, (decimal)parametroCalculoTempo.HorasMinimasCobranca.Value.TotalHours, (decimal)horasDiferenca.TotalHours);

                                    valorPorTempo = faixaHoraMinima.Valor * fatorMultiplicacaoHoraMinima;

                                    if (dados != null)
                                    {
                                        composicao.DescricaoComponente += "De " + parametroCalculoTempo.DataInicial.Value.ToString("dd/MM/yyyy HH:mm") + " até " + parametroCalculoTempo.DataFinal.Value.ToString("dd/MM/yyyy HH:mm") + " ref. à faixa de " + faixaHoraMinima.HoraInicial.ToString(@"hh\:mm") + " às " + faixaHoraMinima.HoraFinal.ToString(@"hh\:mm") + " foram calculadas " + horasDiferenca.ToString(@"hh\:mm") + ".";
                                        composicao.ValoresFormula += faixaHoraMinima.Valor.ToString("n2") + " * " + fatorMultiplicacaoHoraMinima.ToString("n2");
                                        composicao.Valor = faixaHoraMinima.Valor;

                                        SetarValorPorTipoEValor(ref dados, ref composicao, parametros, faixaTempo.TipoCampoValorTabelaFrete, valorPorTempo, 0m, false, parametroCalculoTempo.ComponenteFrete, tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete ? tabelaFreteCliente.TabelaFrete.IncluirICMSValorFrete : tabelaFreteCliente.IncluirICMSValorFrete, unitOfWork);

                                        dados.ComposicaoFrete.Add(composicao);

                                        if (parametroCalculoTempo.SumarizarTempoDadosFrete)
                                            dados.QuantidadeHorasExcedentes += (decimal)horasDiferenca.TotalHours;
                                    }
                                    else if (valorPorTempo > 0m)
                                        return valorPorTempo;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return valorPorTempo;
        }

        private decimal ObterFatorMultiplicacaoTempo(bool multiplicarValorPorHora, decimal totalHorasTrabalhadas, decimal totalHorasFaixa)
        {
            if (multiplicarValorPorHora)
                return totalHorasTrabalhadas;
            else
            {
                if (totalHorasFaixa > 0m)
                    return totalHorasTrabalhadas / totalHorasFaixa;
                else
                    return 0m;
            }
        }

        private void ArredondarHorasTrabalhadas(bool utilizarArredondamentoHoras, int? minutosArredondamentoHoras, bool utilizarMinutosInformadosComoCorteArredondamentoHoraExata, ref decimal totalHorasTrabalhadas)
        {
            if (utilizarArredondamentoHoras && minutosArredondamentoHoras.HasValue && minutosArredondamentoHoras > 0)
            {

                decimal minutosTrabalhados = totalHorasTrabalhadas - Math.Truncate(totalHorasTrabalhadas);

                decimal minutosArredondamento = minutosArredondamentoHoras.Value / 60m;
                decimal quantidadeArredondamento = 0;
                if (minutosTrabalhados > 0m)
                {
                    if (!utilizarMinutosInformadosComoCorteArredondamentoHoraExata)
                    {
                        quantidadeArredondamento = minutosTrabalhados / minutosArredondamento;

                        decimal sobraArredondamento = quantidadeArredondamento - Math.Truncate(quantidadeArredondamento);
                        quantidadeArredondamento -= sobraArredondamento;

                        if (sobraArredondamento >= 0.5m)
                            quantidadeArredondamento += 1;

                        totalHorasTrabalhadas -= minutosTrabalhados;
                        totalHorasTrabalhadas += minutosArredondamento * quantidadeArredondamento;
                    }
                    else
                    {
                        totalHorasTrabalhadas -= minutosTrabalhados;

                        if (minutosTrabalhados >= minutosArredondamento)
                            totalHorasTrabalhadas += 1;
                    }


                }

            }
        }

        private decimal ObterFatorMultiplicacaoPorFaixaTempo(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametroCalculoTempo, Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa periodoInicial, Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempoFaixa faixaTempo, ref List<DateTime> datasJaCobradasExcedente, out bool cobrarHoraMinimaEmOutroPeriodo)
        {
            cobrarHoraMinimaEmOutroPeriodo = false;

            DateTime dataInicial = parametroCalculoTempo.DataInicial.Value;
            DateTime dataFinal = parametroCalculoTempo.DataFinal.Value;

            decimal totalHorasGeral = 0m;
            decimal fatorMultiplicacao = 0m;

            TimeSpan horaInicialFaixa = faixaTempo.HoraInicial;
            TimeSpan horaFinalFaixa = faixaTempo.HoraFinal.Add(new TimeSpan(0, 0, 1));

            DateTime dataInicialFaixaAux = new DateTime(1990, 8, 9, horaInicialFaixa.Hours, horaInicialFaixa.Minutes, horaInicialFaixa.Seconds);
            DateTime dataFinalFaixaAux = new DateTime(1990, 8, 9, horaFinalFaixa.Hours, horaFinalFaixa.Minutes, horaFinalFaixa.Seconds);

            if (dataInicialFaixaAux > dataFinalFaixaAux)
                dataFinalFaixaAux = dataFinalFaixaAux.AddDays(1);

            DateTime data = dataInicial;
            decimal totalHorasFaixa = (decimal)(dataFinalFaixaAux - dataInicialFaixaAux).TotalHours;

            DateTime dataInicialDia = data;
            DateTime dataFinalDia = new DateTime(data.Year, data.Month, data.Day, periodoInicial.HoraInicial.Hours, periodoInicial.HoraInicial.Minutes, periodoInicial.HoraInicial.Seconds);

            dataFinalDia = dataFinalDia.AddDays(1);

            bool percorrerNovamenteAFaixaParaOPrimeiroDia = false;

            do
            {
                DateTime dataInicialFaixa = new DateTime(data.Year, data.Month, data.Day, horaInicialFaixa.Hours, horaInicialFaixa.Minutes, horaInicialFaixa.Seconds);
                DateTime dataFinalFaixa = new DateTime(data.Year, data.Month, data.Day, horaFinalFaixa.Hours, horaFinalFaixa.Minutes, horaFinalFaixa.Seconds);

                if (dataInicialFaixa > dataFinalFaixa)
                {
                    if (dataInicialDia == dataInicial && !percorrerNovamenteAFaixaParaOPrimeiroDia)
                    {
                        dataInicialFaixa = dataInicialFaixa.AddDays(-1);
                        percorrerNovamenteAFaixaParaOPrimeiroDia = true;
                    }
                    else
                    {
                        dataFinalFaixa = dataFinalFaixa.AddDays(1);
                        percorrerNovamenteAFaixaParaOPrimeiroDia = false;
                    }
                }

                DateTime dataInicialCalcular = dataInicialFaixa;
                DateTime dataFinalCalcular = dataFinalFaixa;

                if (dataFinalDia < dataInicialDia)
                    dataFinalDia = dataFinalDia.AddDays(1);

                if (dataFinalDia > dataFinal)
                    dataFinalDia = dataFinal;

                if ((dataInicialDia >= dataInicialFaixa && dataInicialDia <= dataFinalFaixa) || (dataFinalDia >= dataInicialFaixa && dataFinalDia <= dataFinalFaixa) ||
                    (dataInicialFaixa >= dataInicialDia && dataInicialFaixa <= dataFinalDia) || (dataFinalFaixa >= dataInicialDia && dataFinalFaixa <= dataFinalDia))
                {
                    if (dataInicialFaixa < dataInicialDia)
                        dataInicialCalcular = dataInicialDia;

                    if (dataFinalFaixa > dataFinalDia)
                        dataFinalCalcular = dataFinalDia;

                    decimal totalHorasPeriodo = (decimal)(dataFinalCalcular - dataInicialCalcular).TotalHours;

                    ArredondarHorasTrabalhadas(parametroCalculoTempo.UtilizarArredondamentoHoras, parametroCalculoTempo.MinutosArredondamentoHoras, parametroCalculoTempo.UtilizarMinutosInformadosComoCorteArredondamentoHoraExata, ref totalHorasPeriodo);

                    if (!datasJaCobradasExcedente.Contains(dataInicialDia) && parametroCalculoTempo.PossuiHorasMinimasCobranca && totalHorasPeriodo < (decimal)parametroCalculoTempo.HorasMinimasCobranca.Value.TotalHours)
                    {
                        if (faixaTempo.HoraInicialCobrancaMinima.HasValue && faixaTempo.HoraFinalCobrancaMinima.HasValue)
                        {
                            TimeSpan faixaTempoHoraMinimaCobrancaInicial = faixaTempo.HoraInicialCobrancaMinima ?? faixaTempo.HoraInicial;
                            TimeSpan faixaTempoHoraMinimaCobrancaFinal = faixaTempo.HoraFinalCobrancaMinima ?? faixaTempo.HoraFinal;
                            TimeSpan horaInicialCalculo = dataInicialDia.TimeOfDay;

                            if (VerificarSeEstaEntreFaixaDeHorarios(horaInicialCalculo, faixaTempoHoraMinimaCobrancaInicial, faixaTempoHoraMinimaCobrancaFinal))
                                totalHorasPeriodo = (decimal)parametroCalculoTempo.HorasMinimasCobranca.Value.TotalHours;
                            else
                                cobrarHoraMinimaEmOutroPeriodo = true;

                            datasJaCobradasExcedente.Add(dataInicialDia);
                        }
                        else
                        {
                            totalHorasPeriodo = (decimal)parametroCalculoTempo.HorasMinimasCobranca.Value.TotalHours;
                        }
                    }

                    totalHorasGeral += totalHorasPeriodo;
                    fatorMultiplicacao += ObterFatorMultiplicacaoTempo(parametroCalculoTempo.MultiplicarPorHora, totalHorasPeriodo, totalHorasFaixa);
                }

                if (!percorrerNovamenteAFaixaParaOPrimeiroDia)
                {
                    DateTime novaData = data.AddDays(1);

                    data = new DateTime(novaData.Year, novaData.Month, novaData.Day, periodoInicial.HoraInicial.Hours, periodoInicial.HoraInicial.Minutes, periodoInicial.HoraInicial.Seconds);

                    dataInicialDia = data;
                    dataFinalDia = data.AddSeconds(-1);
                }

            } while (data < dataFinal);

            if (totalHorasGeral > 0m)
            {
                composicao.Formula = "De " + dataInicial.ToString("dd/MM/yyyy HH:mm") + " até " + dataFinal.ToString("dd/MM/yyyy HH:mm") + " ref. a faixa de " + faixaTempo.HoraInicial.ToString(@"hh\:mm") + " às " + faixaTempo.HoraFinal.ToString(@"hh\:mm") + " foram calculadas " + TimeSpan.FromHours((double)totalHorasGeral).ToString(@"hh\:mm");

                if (parametroCalculoTempo.MultiplicarPorAjudante)
                    composicao.Formula += " multiplicadas por " + parametros.NumeroAjudantes.ToString() + " ajudante(s)";

                if (parametroCalculoTempo.MultiplicarPorDeslocamento)
                    composicao.Formula += " multiplicadas por " + parametros.NumeroDeslocamento.ToString() + " deslocamento(s)";

                if (parametroCalculoTempo.MultiplicarPorDiaria)
                    composicao.Formula += " multiplicadas por " + parametros.NumeroDiarias.ToString() + " diária(s)";

                if (parametroCalculoTempo.MultiplicarPorEntrega)
                    composicao.Formula += " multiplicadas por " + parametros.NumeroEntregas.ToString() + " entrega(s)";

                composicao.Formula += ".";

                if (parametroCalculoTempo.SumarizarTempoDadosFrete && dados != null)
                    dados.QuantidadeHoras += totalHorasGeral;
            }

            return fatorMultiplicacao;
        }

        private bool VerificarSeEstaEntreFaixaDeHorarios(TimeSpan hora, TimeSpan horaInicial, TimeSpan horaFinal)
        {
            if (horaInicial > horaFinal)
            {
                if ((hora >= horaInicial && hora >= horaFinal) || hora <= horaInicial && hora <= horaFinal)
                    return true;
            }
            else
            {
                if (horaInicial <= hora && hora <= horaFinal)
                    return true;
            }

            return false;
        }

        #endregion Frete por Tempo

        #region Frete por Ajudante

        private decimal BuscarValorPorAjudante(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteAjudante repAjudante = new Repositorio.Embarcador.Frete.TabelaFreteAjudante(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante> ajudanteTabelaFrete = repAjudante.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());

            Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante calculoPorValorFixo = ajudanteTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete.ValorFixoPorAjudante).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante> calculoPorFaixa = ajudanteTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes).ToList();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa.FirstOrDefault()?.TabelaFrete;

            decimal valorPorAjudante = 0m;

            if (calculoPorValorFixo != null)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == calculoPorValorFixo.Codigo select obj).FirstOrDefault();

                valorPorAjudante = itemParaCalculo.ValorParaCalculo * parametros.NumeroAjudantes;

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Ajudante * Número de Ajudante", itemParaCalculo.ValorParaCalculoFormatado + " * " + parametros.NumeroAjudantes.ToString("n2"), valorPorAjudante, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorAjudante, 0m, false, tabelaFrete.ComponenteFreteAjudante, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }

                else if (valorPorAjudante > 0m)
                    return valorPorAjudante;
            }

            if (calculoPorFaixa.Count > 0)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante faixaAjudanteSelecionado = null;

                if (tabelaFrete.PermiteValorAdicionalPorAjudanteExcedente)
                {
                    faixaAjudanteSelecionado = calculoPorFaixa.Where(o => o.NumeroInicial <= parametros.NumeroAjudantes && o.NumeroFinal >= parametros.NumeroAjudantes).FirstOrDefault();

                    if (faixaAjudanteSelecionado == null) //excedeu o número de entregas 
                        faixaAjudanteSelecionado = calculoPorFaixa.OrderByDescending(o => o.NumeroFinal).FirstOrDefault();
                }
                else
                {
                    faixaAjudanteSelecionado = calculoPorFaixa.Where(o => (o.NumeroInicial <= parametros.NumeroAjudantes || o.NumeroInicial == 0m) && (o.NumeroFinal >= parametros.NumeroAjudantes || o.NumeroFinal == 0m)).FirstOrDefault();
                }

                if (faixaAjudanteSelecionado != null)
                {
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaAjudanteSelecionado.Codigo select obj).FirstOrDefault();

                    valorPorAjudante = itemParaCalculo.ValorParaCalculo;

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                    composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante;
                    string formula = "Valor por Faixa de Ajudantes (" + faixaAjudanteSelecionado.Descricao + ")";
                    string valoresFormula = parametros.NumeroAjudantes + " Ajudante(s) = " + valorPorAjudante.ToString("n2");

                    if (valorPorAjudante > 0 && dados != null)
                    {
                        decimal valorTotal = valorPorAjudante;
                        decimal valorExcedenteAdicionar = 0m;

                        if (faixaAjudanteSelecionado.NumeroFinal.Value > 0m && parametros.NumeroAjudantes > faixaAjudanteSelecionado.NumeroFinal.Value)
                        {
                            decimal quantidadeAjudantesExcedente = parametros.NumeroAjudantes - faixaAjudanteSelecionado.NumeroFinal.Value;

                            decimal valorPorAjudanteExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorAjudanteExcedente ?? itemParaCalculo.TabelaFrete?.ValorAjudanteExcedente ?? 0m;

                            decimal valorExcedente = quantidadeAjudantesExcedente * valorPorAjudanteExcedente;

                            if (valorExcedente > 0m)
                            {
                                formula += " + Valor Por Ajudante Excedente * Ajudante Excedentes";
                                valoresFormula += " + " + valorPorAjudanteExcedente.ToString("n2") + " * " + quantidadeAjudantesExcedente.ToString("n2");
                                valorExcedenteAdicionar = valorExcedente;
                                valorTotal += valorExcedente;
                            }
                        }

                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, formula, valoresFormula, valorTotal);
                        SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorAjudante, valorExcedenteAdicionar, false, tabelaFrete.ComponenteFreteAjudante, tabelaFreteCliente, unitOfWork);
                        dados.ComposicaoFrete.Add(composicao);
                    }
                }
            }

            return valorPorAjudante;
        }

        #endregion Frete por Ajudante

        #region Frete por Hora

        private decimal BuscarValorPorHora(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, out bool valorCalculadoPorHoraCorrida)
        {

            Log.TratarErro($"Setar valores tabela frete parametros POR HORA", "GATILHO");

            Repositorio.Embarcador.Frete.TabelaFreteHora repositorioHora = new Repositorio.Embarcador.Frete.TabelaFreteHora(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteHora> horasTabelaFrete = repositorioHora.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());
            Dominio.Entidades.Embarcador.Frete.TabelaFreteHora calculoPorValorFixo = horasTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete.ValorFixoPorHora).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteHora> calculoPorFaixa = horasTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete.PorFaixaHora).ToList();
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa.FirstOrDefault()?.TabelaFrete;
            valorCalculadoPorHoraCorrida = false;

            if (tabelaFrete == null)
                return 0m;

            int multiplicadorHoras = 60;
            int minutosCorridos = parametros.TotalMinutos;
            decimal fracaoHoras = minutosCorridos / (decimal)multiplicadorHoras;
            decimal horasCorridas = tabelaFrete.TipoArredondamentoHoras == TipoArredondamentoTabelaFrete.ParaCima ? Math.Ceiling(fracaoHoras) :
                tabelaFrete.TipoArredondamentoHoras == TipoArredondamentoTabelaFrete.ParaBaixo ? Math.Floor(fracaoHoras) :
                Math.Round(fracaoHoras, 2, MidpointRounding.AwayFromZero);
            decimal valorPorHora = 0m;
            decimal valorTotal = 0m;

            if (tabelaFrete.TipoArredondamentoHoras != TipoArredondamentoTabelaFrete.NaoArredondar)
                minutosCorridos = multiplicadorHoras * (int)horasCorridas;

            Log.TratarErro($"Setar valores tabela frete parametros POR HORA MinutosCorridos {minutosCorridos}", "GATILHO");

            if (calculoPorValorFixo != null)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from o in itens where o.CodigoObjeto == calculoPorValorFixo.Codigo select o).FirstOrDefault();

                if (tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida)
                {
                    valorCalculadoPorHoraCorrida = true;
                    valorPorHora = ((itemParaCalculo.ValorParaCalculo / multiplicadorHoras) * minutosCorridos) * horasCorridas;
                }
                else
                    valorPorHora = (itemParaCalculo.ValorParaCalculo / multiplicadorHoras) * minutosCorridos;

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Hora * Número de Hora", $"{(tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida ? $"{horasCorridas.ToString("n2")} Horas Corridas * " : "")}{itemParaCalculo.ValorParaCalculoFormatado} * {(minutosCorridos * multiplicadorHoras).ToString("n2")}", valorPorHora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorHora, 0m, false, tabelaFrete.ComponenteFreteHora, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }

                if (valorPorHora > 0m)
                    return valorPorHora;
            }

            if (calculoPorFaixa.Count > 0)
            {
                if (tabelaFrete.CalcularComTodasFaixasHora)
                {
                    List<TabelaFreteHora> faixasCompativeis = BuscarTodasFaixasHoraCompativel(calculoPorFaixa, minutosCorridos);

                    int minutosCorridosFaixas = minutosCorridos;

                    foreach (TabelaFreteHora faixa in faixasCompativeis)
                    {
                        int minutosFaixa = minutosCorridosFaixas;
                        if (faixa.MinutoFinal.HasValue)
                        {
                            minutosFaixa = (faixa.MinutoFinal ?? 0) - (faixa.MinutoInicial ?? 0);
                            if (minutosFaixa > minutosCorridosFaixas)
                                minutosFaixa = minutosCorridosFaixas;

                            minutosCorridosFaixas -= minutosFaixa;
                        }

                        int horasCorridasFaixa = minutosFaixa / multiplicadorHoras;
                        valorTotal += CalcularValorPorFixaHora(parametros, faixa, itens, tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida, horasCorridasFaixa, minutosFaixa, unitOfWork, ref dados, tabelaFreteCliente);
                        valorCalculadoPorHoraCorrida = tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida;

                        if (minutosCorridosFaixas <= 0)
                            break;
                    }
                }
                else
                {
                    TabelaFreteHora faixaHoraSelecionado = BuscarFaixaHoraCompativel(tabelaFrete, calculoPorFaixa, minutosCorridos);

                    if (faixaHoraSelecionado != null)
                    {
                        Log.TratarErro($"Setar valores tabela frete parametros POR HORA faixaHoraSelecionado {faixaHoraSelecionado.Descricao}", "GATILHO");

                        valorTotal = CalcularValorPorFixaHora(parametros, faixaHoraSelecionado, itens, tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida, horasCorridas, minutosCorridos, unitOfWork, ref dados, tabelaFreteCliente);
                        valorCalculadoPorHoraCorrida = tabelaFrete.MultiplicarValorFaixaHoraPelaHoraCorrida;


                    }
                }
            }

            Log.TratarErro($"Setar valores tabela frete parametros POR HORA ValorTotal {valorTotal}", "GATILHO");

            return valorTotal;
        }

        #endregion Frete por Hora

        #region Frete por Pacote

        private decimal BuscarValorPorPacote(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.PacoteTabelaFrete repPacoteTabelaFrete = new Repositorio.Embarcador.Frete.PacoteTabelaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete> pacoteTabelaFrete = repPacoteTabelaFrete.BuscarPorCodigos(itens.Select(o => o.CodigoObjeto).ToArray());

            Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete calculoPorValorFixo = pacoteTabelaFrete.Find(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPacoteTabelaFrete.ValorFixoPorPacote);
            List<Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete> calculoPorFaixa = pacoteTabelaFrete.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPacoteTabelaFrete.PorFaixaPacote).ToList();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = calculoPorValorFixo?.TabelaFrete ?? calculoPorFaixa.FirstOrDefault()?.TabelaFrete;

            decimal valorPorPacote = 0m;
            decimal numeroPacotes = parametros.NumeroPacotes;

            if (tabelaFrete != null && tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos &&
                (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido ||
                tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                && parametros.ParametrosCarga != null)
                numeroPacotes = parametros.ParametrosCarga.NumeroPacotes;

            if (calculoPorValorFixo != null)
            {
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == calculoPorValorFixo.Codigo select obj).FirstOrDefault();

                valorPorPacote = itemParaCalculo.ValorParaCalculo * numeroPacotes;

                if (dados != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Pacote * Pacotes", itemParaCalculo.ValorParaCalculoFormatado + " * " + numeroPacotes.ToString("n2"), valorPorPacote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pacote, itemParaCalculo.TipoValor, 0);
                    SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorPacote, 0m, tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos, tabelaFrete.ComponenteFretePacotes, tabelaFreteCliente, unitOfWork);
                    dados.ComposicaoFrete.Add(composicao);
                }

                else if (valorPorPacote > 0m)
                    return valorPorPacote;
            }

            if (calculoPorFaixa.Count > 0)
            {
                Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete faixaPacoteSelecionada = null;

                bool necessarioAjudante = parametros.NecessarioAjudante;
                if (tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
                    necessarioAjudante = parametros.ParametrosCarga?.NecessarioAjudante ?? parametros.NecessarioAjudante;

                if (tabelaFrete.PermiteValorAdicionalPorPacoteExcedente)
                {
                    if (necessarioAjudante)
                        faixaPacoteSelecionada = calculoPorFaixa.Find(o => o.NumeroInicialPacote <= numeroPacotes && o.NumeroFinalPacote >= numeroPacotes && o.ComAjudante);
                    else
                        faixaPacoteSelecionada = calculoPorFaixa.Find(o => o.NumeroInicialPacote <= numeroPacotes && o.NumeroFinalPacote >= numeroPacotes && !o.ComAjudante);

                    if (faixaPacoteSelecionada == null)
                        faixaPacoteSelecionada = calculoPorFaixa.Find(o => o.NumeroInicialPacote <= numeroPacotes && o.NumeroFinalPacote >= numeroPacotes);

                    if (faixaPacoteSelecionada == null) //excedeu o número de pacotes 
                        faixaPacoteSelecionada = calculoPorFaixa.OrderByDescending(o => o.NumeroFinalPacote).FirstOrDefault();
                }
                else
                {
                    if (necessarioAjudante)
                        faixaPacoteSelecionada = calculoPorFaixa.Find(o => (o.NumeroInicialPacote <= numeroPacotes || o.NumeroInicialPacote == 0m) && (o.NumeroFinalPacote >= numeroPacotes || o.NumeroFinalPacote == 0m) && o.ComAjudante);
                    else
                        faixaPacoteSelecionada = calculoPorFaixa.Find(o => (o.NumeroInicialPacote <= numeroPacotes || o.NumeroInicialPacote == 0m) && (o.NumeroFinalPacote >= numeroPacotes || o.NumeroFinalPacote == 0m) && !o.ComAjudante);

                    if (faixaPacoteSelecionada == null)
                        faixaPacoteSelecionada = calculoPorFaixa.Find(o => (o.NumeroInicialPacote <= numeroPacotes || o.NumeroInicialPacote == 0m) && (o.NumeroFinalPacote >= numeroPacotes || o.NumeroFinalPacote == 0m));
                }

                if (faixaPacoteSelecionada != null)
                {

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                    composicao.TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pacote;

                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaPacoteSelecionada.Codigo select obj).FirstOrDefault();

                    string valorMultiplicacao = "";
                    string formulaMultiplicacao = "";
                    if (!tabelaFrete.MultiplicarValorDaFaixa)
                        valorPorPacote = itemParaCalculo.ValorParaCalculo;
                    else
                    {
                        valorPorPacote = itemParaCalculo.ValorParaCalculo * numeroPacotes;
                        valorMultiplicacao = " * " + numeroPacotes.ToString();
                        formulaMultiplicacao = " * Número de Pacotes";
                    }

                    string formula = string.Empty;
                    string valoresFormula = string.Empty;
                    //if (tabelaFrete.CalcularValorEntregaPorPercentualFrete)
                    //    formula = "Percentual do frete por pacote (" + faixaPacoteSelecionada.Descricao + ")";
                    //else if (tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                    //    formula = "Percentual do frete + componentes por pacote (" + faixaPacoteSelecionada.Descricao + ")";
                    //else
                    //{
                    formula = "Valor por Faixa de pacote (" + faixaPacoteSelecionada.Descricao + ")" + formulaMultiplicacao;
                    valoresFormula = numeroPacotes + " Pacote(s) = " + itemParaCalculo.ValorParaCalculo.ToString("n2") + valorMultiplicacao;
                    //}

                    if (dados != null)
                    {
                        decimal valorTotal = valorPorPacote;
                        decimal valorExcedenteAdicionar = 0m;

                        //if (tabelaFrete.CalcularValorEntregaPorPercentualFrete || tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                        //{
                        //    decimal numeroEntregasExcedente = 0;
                        //    if (faixaPacoteSelecionada.NumeroInicialPacote.Value > 0 && faixaPacoteSelecionada.NumeroFinalPacote.Value >= 0)
                        //        numeroEntregasExcedente = numeroPacotes - faixaPacoteSelecionada.NumeroInicialPacote.Value + 1;

                        //    if (dados.ValorFrete > 0 && numeroEntregasExcedente > 0 && tabelaFrete.CalcularValorEntregaPorPercentualFrete)
                        //    {
                        //        valorPorPacote = dados.ValorFrete * ((numeroEntregasExcedente * valorPorPacote) / 100);

                        //        valoresFormula = numeroPacotes + " Pacote(s) = " + (numeroEntregasExcedente * valorTotal).ToString("n2") + "%";
                        //    }
                        //    else if (dados.ValorTotal > 0 && numeroEntregasExcedente > 0 && tabelaFrete.CalcularValorEntregaPorPercentualFreteComComponentes)
                        //    {
                        //        valorPorPacote = dados.ValorTotal * ((numeroEntregasExcedente * valorPorPacote) / 100);

                        //        valoresFormula = numeroPacotes + " Pacote(s) = " + (numeroEntregasExcedente * valorTotal).ToString("n2") + "%";
                        //    }
                        //    else
                        //    {
                        //        valorTotal = 0;
                        //        valorPorPacote = 0;
                        //    }
                        //}
                        if (faixaPacoteSelecionada.NumeroFinalPacote.Value > 0m && numeroPacotes > faixaPacoteSelecionada.NumeroFinalPacote.Value)
                        {
                            decimal numeroPacotesExcedente = numeroPacotes - faixaPacoteSelecionada.NumeroFinalPacote.Value;

                            decimal valorPorPacoteExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorPacoteExcedente ?? itemParaCalculo.TabelaFrete?.ValorPacoteExcedente ?? 0m;

                            decimal valorExcedente = numeroPacotesExcedente * valorPorPacoteExcedente;

                            if (valorExcedente > 0m)
                            {
                                formula += " + Valor Por Pacote Excedente * Pacotes Excedentes";
                                valoresFormula += " + " + valorPorPacoteExcedente.ToString("n2") + " * " + numeroPacotesExcedente.ToString("n2");
                                valorExcedenteAdicionar = valorExcedente;
                                valorTotal += valorExcedente;
                            }
                        }

                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, formula, valoresFormula, valorTotal);
                        SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorPacote, valorExcedenteAdicionar, tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos, tabelaFrete.ComponenteFretePacotes, tabelaFreteCliente, unitOfWork);
                        if (!tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos)
                            dados.ComposicaoFrete.Add(composicao);
                        else
                            dados.ComposicaoValorBaseFrete.Add(composicao);
                    }
                }
            }

            return valorPorPacote;
        }

        #endregion Frete por Pacote

        private TabelaFreteHora BuscarFaixaHoraCompativel(TabelaFrete tabelaFrete, List<TabelaFreteHora> calculoPorFaixa, int minutosCorridos)
        {
            TabelaFreteHora faixaHoraSelecionado = null;

            if (tabelaFrete.PermiteValorAdicionalPorHoraExcedente)
            {
                faixaHoraSelecionado = calculoPorFaixa.Where(o => o.MinutoInicial <= minutosCorridos && o.MinutoFinal >= minutosCorridos).FirstOrDefault();

                if (faixaHoraSelecionado == null) //excedeu o número de entregas 
                    faixaHoraSelecionado = calculoPorFaixa.OrderByDescending(o => o.MinutoFinal).FirstOrDefault();
            }
            else
                faixaHoraSelecionado = calculoPorFaixa.Where(o => (o.MinutoInicial <= minutosCorridos || o.MinutoInicial == 0m) && (o.MinutoFinal >= minutosCorridos || o.MinutoFinal == 0m)).FirstOrDefault();

            return faixaHoraSelecionado;
        }

        private List<TabelaFreteHora> BuscarTodasFaixasHoraCompativel(List<TabelaFreteHora> calculoPorFaixa, int minutosCorridos)
        {
            List<TabelaFreteHora> faixasOrdenadas = calculoPorFaixa.OrderBy(o => o.MinutoInicial).ToList();
            List<TabelaFreteHora> faixasCompativeis = new List<TabelaFreteHora>();

            foreach (TabelaFreteHora faixa in faixasOrdenadas)
            {
                if (!faixa.MinutoFinal.HasValue)
                {
                    faixasCompativeis.Add(faixa);
                    break;
                }

                int minutosFaixa = (faixa.MinutoFinal ?? 0) - (faixa.MinutoInicial ?? 0);
                minutosCorridos -= minutosFaixa;
                faixasCompativeis.Add(faixa);

                if (minutosCorridos <= 0)
                    break;
            }

            return faixasCompativeis;
        }

        private decimal CalcularValorPorFixaHora(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, TabelaFreteHora faixaHoraSelecionado, List<ItemParametroBaseCalculoTabelaFrete> itens, bool multiplicarValorFaixaHoraPelaHoraCorrida, decimal horasCorridas, int minutosCorridos, Repositorio.UnitOfWork unitOfWork, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            decimal valorTotal = 0m;
            decimal valorPorHora = 0m;
            int multiplicadorHoras = 60;

            ItemParametroBaseCalculoTabelaFrete itemParaCalculo = (from obj in itens where obj.CodigoObjeto == faixaHoraSelecionado.Codigo select obj).FirstOrDefault();
            StringBuilder formula = new StringBuilder();
            StringBuilder valoresFormula = new StringBuilder();

            formula.Append("Valor por ");

            if (multiplicarValorFaixaHoraPelaHoraCorrida)
                formula.Append($"Horas Corridas ({horasCorridas:n2}) * ");

            if (tabelaFreteCliente.TabelaFrete.MultiplicarValorDaFaixa)
                formula.Append("Horas * ");

            formula.Append($" Faixa de Horas ({faixaHoraSelecionado.Descricao})");

            if (tabelaFreteCliente.TabelaFrete.MultiplicarValorDaFaixa)
            {
                valorPorHora = parametros.Peso * itemParaCalculo.ValorParaCalculo;
                valoresFormula.Append($"{parametros.Peso:n2} * {itemParaCalculo.ValorParaCalculoFormatado}");
            }
            else
            {
                valorPorHora = itemParaCalculo.ValorParaCalculo;
                valoresFormula.Append($"{minutosCorridos / multiplicadorHoras:n2} Hora = {itemParaCalculo.ValorParaCalculoFormatado}");
            }

            if (multiplicarValorFaixaHoraPelaHoraCorrida)
            {
                valoresFormula.Append($" {{Valor por Hora ({valorPorHora:n2}) * Horas Corridas ({horasCorridas:n2})}} = {valorPorHora * horasCorridas:n2}");
                valorPorHora *= horasCorridas;
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete
            {
                TipoParametro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora
            };
            decimal valorExcedenteAdicionar = 0m;

            valorTotal = valorPorHora;

            if ((faixaHoraSelecionado.MinutoFinal.Value > 0m) && (minutosCorridos > faixaHoraSelecionado.MinutoFinal.Value))
            {
                decimal quantidadeMinutosHorasExcedente = minutosCorridos - faixaHoraSelecionado.MinutoFinal.Value;
                decimal valorPorHoraExcedente = itemParaCalculo.ParametroBaseCalculo?.ValorHoraExcedente ?? itemParaCalculo.TabelaFrete?.ValorHoraExcedente ?? 0m;

                if (tabelaFreteCliente.TabelaFrete.MultiplicarValorDaFaixa)
                    valorPorHoraExcedente = parametros.Peso * valorPorHoraExcedente;

                decimal valorExcedente = (quantidadeMinutosHorasExcedente / multiplicadorHoras) * valorPorHoraExcedente;

                if (valorExcedente > 0m)
                {
                    formula.Append(" + Valor Por Hora Excedente * Horas Excedentes");
                    valoresFormula.Append($" + {valorPorHoraExcedente.ToString("n2")} * {(quantidadeMinutosHorasExcedente * multiplicadorHoras).ToString("n2")}");
                    valorExcedenteAdicionar = valorExcedente;
                    valorTotal += valorExcedente;
                }
            }

            if (valorTotal > 0 && dados != null)
            {
                Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, formula.ToString(), valoresFormula.ToString(), valorTotal);
                SetarValorTabelaFreteCarga(ref dados, ref composicao, parametros, itemParaCalculo, valorPorHora, valorExcedenteAdicionar, false, tabelaFreteCliente.TabelaFrete.ComponenteFreteHora, tabelaFreteCliente, unitOfWork);
                dados.ComposicaoFrete.Add(composicao);
            }

            return valorTotal;
        }

        private void SetarObservacaoTabelaFreteCarga(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro)
        {
            if (parametro != null && parametro.ImprimirObservacaoCTe)
            {
                dados.Observacao = parametro.Observacao;
                dados.ObservacaoTerceiro = parametro.ObservacaoTerceiro;
            }
            else if (tabelaFreteCliente.ImprimirObservacaoCTe)
            {
                dados.Observacao = tabelaFreteCliente.Observacao;
                dados.ObservacaoTerceiro = tabelaFreteCliente.ObservacaoTerceiro;
            }
            else if (tabelaFreteCliente.TabelaFrete.ImprimirObservacaoCTe)
            {
                dados.Observacao = tabelaFreteCliente.TabelaFrete.Observacao;
                dados.ObservacaoTerceiro = tabelaFreteCliente.TabelaFrete.ObservacaoTerceiro;
            }
            else
            {
                dados.Observacao = null;
                dados.ObservacaoTerceiro = null;
            }
        }

        private void SetarValorPorTipoEValor(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoCampoValorTabelaFrete, decimal valor, decimal valorExcedente, bool InformarValorComoBase, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, bool incluirICMSValorFrete, Repositorio.UnitOfWork unitOfWork)
        {
            composicao.TipoValor = tipoCampoValorTabelaFrete;

            if (componenteFrete != null)
            {
                SetarComponente(ref dados, ref composicao, parametros, tipoCampoValorTabelaFrete, valor, componenteFrete, incluirICMSValorFrete, unitOfWork, valorExcedente);
                composicao.CodigoComponente = componenteFrete.Codigo;
            }
            else
            {
                switch (tipoCampoValorTabelaFrete)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual:
                        if (InformarValorComoBase && valor > 0)
                        {
                            dados.ValorBase += dados.ValorBase * (valor / 100);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor:
                        if (!InformarValorComoBase)
                        {
                            dados.ValorFrete += valor + valorExcedente;
                            composicao.ValorCalculado = valor + valorExcedente;
                        }
                        else
                            dados.ValorBase += valor + valorExcedente;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:
                        dados.PercentualSobreNF += valor;
                        decimal valorCalculado = (parametros.ValorNotasFiscais * (valor / 100)) + valorExcedente;
                        dados.ValorFrete += valorCalculado;
                        Servicos.Embarcador.Frete.ComposicaoFrete.InformarComposicaoFrete(ref composicao, " (Valor Total Mercadoria * Percentual)", " (" + parametros.ValorNotasFiscais.ToString("n5") + " * " + dados.PercentualSobreNF.ToString("n5") + "%)", valor);
                        composicao.ValorCalculado = valorCalculado;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima:
                        if ((valor + valorExcedente) > 0)
                        {
                            if (!InformarValorComoBase)
                            {
                                dados.ValorFixo = valor + valorExcedente;
                                dados.ValorFrete = valor + valorExcedente;
                                composicao.ValorCalculado = valor + valorExcedente;
                            }
                            else
                                dados.ValorBase = valor + valorExcedente;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetarValorTabelaFreteCarga(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados, ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, decimal valor, decimal valorExcedente, bool InformarValorComoBase, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            SetarValorPorTipoEValor(ref dados, ref composicao, parametros, item.TipoValor, valor, valorExcedente, InformarValorComoBase, componenteFrete, tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete ? tabelaFreteCliente.TabelaFrete.IncluirICMSValorFrete : tabelaFreteCliente.IncluirICMSValorFrete, unitOfWork);
        }

        private bool ValidarTracoesTabelaFrete(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, ref string mensagem)
        {
            if (tabelaFrete.TabelaFrete.ModelosTracao.Count <= 0)
                return true;

            Dominio.Entidades.Veiculo tracao = Servicos.Veiculo.ObterTracao(veiculo);

            if (tracao == null)
            {
                mensagem = "não foi encontrado um veículo de tração na carga para comparar com os modelos de tração da tabela de frete.";
                return false;
            }

            if (tracao.ModeloVeicularCarga == null || !tabelaFrete.TabelaFrete.ModelosTracao.Any(o => o.Codigo == tracao.ModeloVeicularCarga.Codigo))
            {
                mensagem = "o modelo do veículo de tração da carga (" + tracao.Placa + " - " + (tracao.ModeloVeicularCarga?.Descricao ?? string.Empty) + ") difere dos modelos veículares de tração configurados na tabela de frete.";
                return false;
            }

            return true;
        }

        private bool ValidarReboquesTabelaFrete(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaReboque, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, ref string mensagem)
        {
            if (tabelaFrete.TabelaFrete.ModelosReboque.Count <= 0)
                return true;

            if (modeloVeicularCargaReboque == null || !tabelaFrete.TabelaFrete.ModelosReboque.Any(o => o.Codigo == modeloVeicularCargaReboque.Codigo))
            {
                mensagem = "o modelo do veículo de reboque da carga ( " + (modeloVeicularCargaReboque?.Descricao ?? string.Empty) + ") difere dos modelos veiculares de reboque configurados na tabela de frete.";
                return false;
            }
            return true;
        }

        private bool ValidarTiposCargaTabelaFrete(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, ref string mensagem)
        {
            if (tabelaFrete.TabelaFrete.TiposCarga.Count <= 0)
                return true;

            if (tipoCarga == null || !tabelaFrete.TabelaFrete.TiposCarga.Any(o => o.Codigo == tipoCarga.Codigo))
            {
                mensagem = "o tipo da carga (" + tipoCarga?.Descricao + ") difere dos tipos de carga configurados na tabela de frete.";
                return false;
            }

            return true;
        }

        private bool ValidarValorFreteTabelaFrete(TabelaFreteCliente tabelaFrete, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagem)
        {
            if (tabelaFrete.TabelaFrete.ParametroBase.HasValue)
                return ValidarValorFreteTabelaFreteComParametroBase(tabelaFrete, parametros, unitOfWork, tipoServicoMultisoftware, ref mensagem);
            else
                return ValidarValorFreteTabelaFreteSemParametroBase(tabelaFrete, parametros, unitOfWork, tipoServicoMultisoftware, ref mensagem);
        }

        private bool ValidarValorFreteTabelaFreteComParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagem)
        {
            List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametrosParaCalculo = ObterParametroBaseTabelaFrete(tabelaFrete, parametros);

            if (parametrosParaCalculo.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in parametrosParaCalculo)
                {
                    if (!ValidarValorFreteTabelaFretePorItensParametroBase(tabelaFrete, parametro.ItensBaseCalculo, parametros, tipoServicoMultisoftware, parametro.ValorBase, unitOfWork, out mensagem))
                    {
                        if (string.IsNullOrWhiteSpace(mensagem))
                            mensagem = "O valor dos itens configurados no parâmetro para cálculo estão zerados.";

                        return false;
                    }
                }
            }
            else
            {
                mensagem = "os valores do parâmetro para cálculo não foram configurados.";
                return false;
            }
            return true;
        }

        private bool ValidarValorFreteTabelaFreteSemParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagem)
        {
            if (!ValidarValorFreteTabelaFretePorItensParametroBase(tabelaFrete, tabelaFrete.ItensBaseCalculo, parametros, tipoServicoMultisoftware, tabelaFrete.ValorBase, unitOfWork, out mensagem))
            {
                if (string.IsNullOrWhiteSpace(mensagem))
                    mensagem = "o valor dos itens configurados para cálculo estão zerados.";

                return false;
            }

            return true;
        }

        private bool ValidarValorFreteTabelaFretePorItensParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, IList<ItemParametroBaseCalculoTabelaFrete> itensBaseCalculo, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, decimal valorBase, Repositorio.UnitOfWork unitOfWork, out string mensagem)
        {
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesFreteTabelaFrete = new List<ComponenteFreteTabelaFrete>();
            if (!tabelaFreteCliente.TabelaFrete.NaoCalculaSemFreteValor)
                componentesFreteTabelaFrete = repComponenteFreteTabelaFrete.BuscarPorTabelaFreteParaCalculo(tabelaFreteCliente.TabelaFrete.Codigo, false);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentesFreteTabelaFreteCargaPerigosa = (from obj in componentesFreteTabelaFrete where obj.ComponenteFrete.SomenteParaCargaPerigosa select obj).ToList();
            componentesFreteTabelaFrete = (from obj in componentesFreteTabelaFrete where !obj.ComponenteFrete.SomenteParaCargaPerigosa select obj).ToList();

            if (parametros.CargaPerigosa)
                componentesFreteTabelaFrete.AddRange(componentesFreteTabelaFreteCargaPerigosa);

            bool possuiParametroObrigatorio = tabelaFreteCliente.TabelaFrete.ObrigatorioValorFretePeso;
            bool freteValido = false;
            mensagem = string.Empty;

            itensBaseCalculo = itensBaseCalculo.OrderBy(obj => obj.TipoValor).ToList();

            foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itensBaseCalculo)
            {
                if (item.ValorParaCalculo > 0)
                {
                    switch (item.TipoObjeto)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque:

                            if (item.CodigoObjeto == parametros.ModeloVeiculo?.Codigo && item.ValorParaCalculo > 0m)
                                freteValido = true;

                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao:

                            Dominio.Entidades.Veiculo tracao = Servicos.Veiculo.ObterTracao(parametros.Veiculo);

                            if (tracao?.ModeloVeicularCarga?.Codigo == item.CodigoObjeto && item.ValorParaCalculo > 0m)
                                freteValido = true;

                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga:

                            if ((parametros.TipoCarga?.Codigo == item.CodigoObjeto && item.ValorParaCalculo > 0m) || tabelaFreteCliente.TabelaFrete.NaoPermitirLancarValorPorTipoDeCarga)
                                freteValido = true;

                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete:
                            if ((from obj in componentesFreteTabelaFrete where obj.Codigo == item.CodigoObjeto select obj).FirstOrDefault() != null && item.ValorParaCalculo > 0m)
                                freteValido = true;
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem:
                            if ((parametros.TiposEmbalagem?.Count ?? 0) > 0)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem parametroTipoEmbalagem = (from obj in parametros.TiposEmbalagem where obj.TipoEmbalagem.Codigo == item.CodigoObjeto select obj).FirstOrDefault();
                                if (parametroTipoEmbalagem != null && item.ValorParaCalculo > 0)
                                {
                                    freteValido = true;
                                    break;
                                }
                            }

                            break;
                        default:
                            break;
                    }

                    if (!possuiParametroObrigatorio && freteValido)
                        return true;
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados = null;

            decimal valor = BuscarValorPorPeso(parametros, false, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso && obj.TipoValor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.Desabilitado select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente, tipoServicoMultisoftware);

            if (valor <= 0m && tabelaFreteCliente.TabelaFrete.ObrigatorioValorFretePeso)
            {
                mensagem = "não foi encontrado um valor de frete por peso. De acordo com as configurações da tabela de frete este valor é obrigatório.";
                return false;
            }
            else if (valor > 0m)
                freteValido = true;

            if (!freteValido)
            {
                valor = BuscarValorPorDistancia(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia && obj.TipoValor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.Desabilitado select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);

                if (valor > 0m)
                    freteValido = true;
            }

            if (!freteValido)
            {
                valor = BuscarValorPorEntrega(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega select obj).ToList(), unitOfWork, ref dados, tipoServicoMultisoftware, tabelaFreteCliente);

                if (valor > 0m)
                    freteValido = true;
            }

            if (!freteValido)
            {
                valor = BuscarValorPorPallets(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);

                if (valor > 0m)
                    freteValido = true;
            }

            if (!freteValido)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ParametroCalculoFreteTempo parametrosCalculoTempo = ObterParametrosCalculoFreteTempo(tabelaFreteCliente, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo select obj).ToList(), parametros, unitOfWork);

                valor = BuscarValorPorTempo(tabelaFreteCliente, parametros, parametrosCalculoTempo, ref dados, unitOfWork);

                //valor = BuscarValorPorTempo(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);

                if (valor > 0m)
                    freteValido = true;
            }

            if (!freteValido)
            {
                valor = BuscarValorPorAjudante(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente);

                if (valor > 0m)
                    freteValido = true;
            }

            if (!freteValido)
            {
                valor = BuscarValorPorHora(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora select obj).ToList(), unitOfWork, ref dados, tabelaFreteCliente, out bool valorCalculadoPorHoraCorrida);

                if ((valor > 0m) || valorCalculadoPorHoraCorrida)
                    freteValido = true;
            }

            if (!freteValido)
            {
                valor = BuscarValorPorPacote(parametros, (from obj in itensBaseCalculo where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pacote select obj).ToList(), unitOfWork, ref dados, tipoServicoMultisoftware, tabelaFreteCliente);

                if (valor > 0m)
                    freteValido = true;
            }

            if (!freteValido)
            {
                if (tabelaFreteCliente.TabelaFrete?.TiposTerceiros != null && tabelaFreteCliente.TabelaFrete?.TiposTerceiros?.Count > 0)
                    freteValido = true;
            }

            if (!freteValido)
            {
                if (valorBase > 0 || (tabelaFreteCliente.TabelaFrete?.ContratoFreteTransportador != null && (tabelaFreteCliente.TabelaFrete?.PossuiValorBase ?? false)))
                    freteValido = true;
            }

            return freteValido;
        }

        private List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> ObterParametroBaseTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros)
        {

            List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> listaParametrosBaseCalculo = new List<ParametroBaseCalculoTabelaFrete>();

            switch (tabelaFrete.TabelaFrete.ParametroBase.Value)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao:

                    Dominio.Entidades.Veiculo tracao = Servicos.Veiculo.ObterTracao(parametros.Veiculo);

                    if (tracao != null && tracao.ModeloVeicularCarga != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = (from obj in tabelaFrete.ParametrosBaseCalculo where obj.CodigoObjeto == tracao.ModeloVeicularCarga.Codigo select obj).FirstOrDefault();
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }
                    else if (parametros.CalcularVariacoes)
                        listaParametrosBaseCalculo.AddRange(from obj in tabelaFrete.ParametrosBaseCalculo select obj);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque:

                    if (parametros.ModeloVeiculo != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = (from obj in tabelaFrete.ParametrosBaseCalculo where obj.CodigoObjeto == parametros.ModeloVeiculo.Codigo select obj).FirstOrDefault();
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }
                    else if (parametros.CalcularVariacoes)
                        listaParametrosBaseCalculo.AddRange(from obj in tabelaFrete.ParametrosBaseCalculo select obj);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga:

                    if (parametros.TipoCarga != null && !tabelaFrete.TabelaFrete.NaoPermitirLancarValorPorTipoDeCarga)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = (from obj in tabelaFrete.ParametrosBaseCalculo where obj.CodigoObjeto == parametros.TipoCarga.Codigo select obj).FirstOrDefault();
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso:

                    Dominio.Enumeradores.UnidadeMedida unidadeMedida = tabelaFrete.TabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.UnidadeMedida).FirstOrDefault();

                    decimal peso = ObterPesoPorUnidadeMedida(parametros, unidadeMedida);

                    peso = ObterPesoParaComparacao(unidadeMedida, peso);

                    Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete faixaPesoSelecionado = tabelaFrete.TabelaFrete.PesosTransportados.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.PorFaixaPesoTransportado && (o.PesoInicial <= peso || o.PesoInicial == 0m) && (o.PesoFinal >= peso || o.PesoFinal == 0m) && (o.ModeloVeicularCarga == null || o.ModeloVeicularCarga?.Codigo == parametros.ModeloVeiculo?.Codigo)).FirstOrDefault();

                    if (faixaPesoSelecionado != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroPeso = tabelaFrete.ParametrosBaseCalculo.Where(o => o.CodigoObjeto == faixaPesoSelecionado.Codigo).FirstOrDefault();
                        if (parametroPeso != null)
                            listaParametrosBaseCalculo.Add(parametroPeso);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete pesoFixoSelecionado = tabelaFrete.TabelaFrete.PesosTransportados.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPesoTabelaFrete.ValorFixoPorPesoTransportado).FirstOrDefault();
                        if (pesoFixoSelecionado != null)
                        {
                            Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = tabelaFrete.ParametrosBaseCalculo.Where(o => o.CodigoObjeto == pesoFixoSelecionado.Codigo).FirstOrDefault();
                            if (parametro != null)
                                listaParametrosBaseCalculo.Add(parametro);
                        }
                    }

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia:

                    decimal distancia = parametros.Distancia;
                    if (tabelaFrete.TabelaFrete.UsarCubagemComoParametroDeDistancia)
                        distancia = parametros.Cubagem;

                    Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete faixaDistanciaTabelaFrete = tabelaFrete.TabelaFrete.Distancias.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDistanciaTabelaFrete.PorFaixaDistanciaPercorrida && (o.QuilometragemInicial <= distancia || o.QuilometragemInicial == 0m) && (o.QuilometragemFinal >= distancia || o.QuilometragemFinal == 0m)).FirstOrDefault();

                    if (faixaDistanciaTabelaFrete != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = tabelaFrete.ParametrosBaseCalculo.Where(o => o.CodigoObjeto == faixaDistanciaTabelaFrete.Codigo).FirstOrDefault();
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega:

                    int numeroEntrega = (int)parametros.NumeroEntregas;

                    Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete faixaNumeroEntregas = tabelaFrete.TabelaFrete.NumeroEntregas.Where(o => o.Tipo == TipoNumeroEntregaTabelaFrete.PorFaixaEntrega && (o.NumeroInicialEntrega <= numeroEntrega || o.NumeroInicialEntrega == 0) && (o.NumeroFinalEntrega >= numeroEntrega || o.NumeroFinalEntrega == 0)).FirstOrDefault();

                    if (faixaNumeroEntregas != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = tabelaFrete.ParametrosBaseCalculo.Where(o => o.CodigoObjeto == faixaNumeroEntregas.Codigo).FirstOrDefault();
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Hora:

                    int minutos = parametros.TotalMinutos;

                    Dominio.Entidades.Embarcador.Frete.TabelaFreteHora faixaTabelaFreteHora = tabelaFrete.TabelaFrete.Horas.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaHoraTabelaFrete.PorFaixaHora && (o.MinutoInicial <= minutos || o.MinutoInicial == 0m) && (o.MinutoFinal >= minutos || o.MinutoFinal == 0m)).FirstOrDefault();

                    if (faixaTabelaFreteHora != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = tabelaFrete.ParametrosBaseCalculo.Where(o => o.CodigoObjeto == faixaTabelaFreteHora.Codigo).FirstOrDefault();
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem:

                    if ((parametros.TiposEmbalagem?.Count ?? 0) > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ParametroTipoEmbalagem tipoEmbalagem in parametros.TiposEmbalagem)
                        {
                            Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = (from obj in tabelaFrete.ParametrosBaseCalculo where obj.CodigoObjeto == tipoEmbalagem.TipoEmbalagem.Codigo select obj).FirstOrDefault();
                            if (parametro != null)
                                listaParametrosBaseCalculo.Add(parametro);
                        }
                    }
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pacote:

                    int numeroPacotes = (int)parametros.NumeroPacotes;

                    Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete faixaPacotes = tabelaFrete.TabelaFrete.Pacotes.FirstOrDefault(o => o.Tipo == TipoPacoteTabelaFrete.PorFaixaPacote && (o.NumeroInicialPacote <= numeroPacotes || o.NumeroInicialPacote == 0) && (o.NumeroFinalPacote >= numeroPacotes || o.NumeroFinalPacote == 0));

                    if (faixaPacotes != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = tabelaFrete.ParametrosBaseCalculo.FirstOrDefault(o => o.CodigoObjeto == faixaPacotes.Codigo);
                        if (parametro != null)
                            listaParametrosBaseCalculo.Add(parametro);
                    }

                    break;
            }

            return listaParametrosBaseCalculo;

        }

        private decimal ObterCalculoCoeficiente(decimal capacidadeModeloVeicular, decimal pesoTotalCarga)
        {
            if (pesoTotalCarga == 0) return 0;
            return capacidadeModeloVeicular / pesoTotalCarga;
        }

        private decimal ObterPesoCalculodoPorCoeficiente(decimal peso, decimal capacidadeModeloVeicular, decimal pesoTotalCarga)
        {
            return peso * ObterCalculoCoeficiente(capacidadeModeloVeicular, pesoTotalCarga);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete ObterConfiguracaoTabelaFrete()
        {
            if (_configuracaoTabelaFrete == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(_unitOfWork);
                _configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
            }

            return _configuracaoTabelaFrete;
        }

        #endregion
    }
}
