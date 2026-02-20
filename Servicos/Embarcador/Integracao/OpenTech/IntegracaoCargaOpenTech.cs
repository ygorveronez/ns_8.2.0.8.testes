using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.OpenTech
{
    public class IntegracaoCargaOpenTech
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Servicos.ServicoOpenTech.sgrData _openTechSgrDataResult;
        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoIntegracaoOpenTech _configuracaoIntegracaoOpenTech;
        private Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa _configuracaoIntegracaoEmpresa;

        #endregion

        #region Construtores

        public IntegracaoCargaOpenTech(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech repGrupoProdutoOpenTech = new Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIsca repositorioCargaIsca = new Repositorio.Embarcador.Cargas.CargaIsca(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontos = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

            cargaIntegracao.NumeroTentativas++;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    throw new ServicoException("A configuração de integração para a OpenTech é inválida.");
                }

                if (cargaIntegracao.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaIntegracao.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    throw new ServicoException("Não é possível realizar a integração para carga já cancelada/anulada.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = repositorioPedidoProduto.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                IList<int> codigosVeiculosVinculados = repositorioVeiculo.BuscarVeiculosVinculadoACarga(cargaIntegracao.Carga.Codigo);
                List<Dominio.Entidades.Veiculo> veiculosVinculados = repositorioVeiculo.BuscarPorCodigos(codigosVeiculosVinculados, false);

                CadastrarRotaOpentech(cargaIntegracao);

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector);

                    if (cdTrans == 0)
                        throw new ServicoException("A configuração OpenTech de integração da empresa " + cargaIntegracao.Carga.Empresa.RazaoSocial + " é inválida.");

                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                        throw new ServicoException("Não foi possível realizar o login: " + retornoLogin.ReturnDescription);

                    decimal valorTotalPedidos = repPedidoXMLNotaFiscal.BuscarTotalPorCarga(cargaIntegracao.Carga.Codigo);
                    bool valorMaior = true;

                    if (valorTotalPedidos < _configuracaoIntegracaoOpenTech.ValorBaseOpenTech)
                        valorMaior = false;

                    Dominio.Entidades.Localidade origem = null;
                    Dominio.Entidades.Localidade destino = null;
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercursoOrigem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercursoDestino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem = repCargaRotaFretePontos.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                    List<Dominio.Entidades.Usuario> motoristas = repositorioCargaMotorista.BuscarMotoristasPorCarga(cargaIntegracao.Carga.Codigo);

                    if (cargaPercursoOrigem == null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaIntegracao.Carga.Codigo);
                        origem = primeiroCargaPedido?.Origem;
                    }

                    cargaPercursoOrigem ??= cargaPercursoDestino;
                    origem ??= cargaPercursoOrigem?.Origem;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    {
                        if (cargaRotaFretePontosPassagem?.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem primeiraCargaRotaFretePontoPassagem = cargaRotaFretePontosPassagem.FirstOrDefault();
                            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ultimaCargaRotaFretePontoPassagem = cargaRotaFretePontosPassagem.LastOrDefault();

                            if (primeiraCargaRotaFretePontoPassagem?.Cliente != null)
                                origem = primeiraCargaRotaFretePontoPassagem.Cliente.Localidade;
                            else if (primeiraCargaRotaFretePontoPassagem?.ClienteOutroEndereco?.Localidade != null)
                                origem = primeiraCargaRotaFretePontoPassagem.ClienteOutroEndereco.Localidade;

                            if (ultimaCargaRotaFretePontoPassagem?.Cliente != null)
                                destino = ultimaCargaRotaFretePontoPassagem.Cliente.Localidade;
                            else if (ultimaCargaRotaFretePontoPassagem?.ClienteOutroEndereco?.Localidade != null)
                                destino = ultimaCargaRotaFretePontoPassagem.ClienteOutroEndereco.Localidade;
                        }
                        else
                        {
                            destino = cargaPercursoOrigem?.Origem;
                            origem = cargaIntegracao.Carga.Filial?.Localidade ?? cargaPedidos?.FirstOrDefault()?.Origem;
                        }
                    }
                    else
                        destino = cargaPercursoOrigem?.Destino;

                    int codigoOrigemOpenTech = 0;
                    int codigoDestinoOpenTech = 0;

                    if (origem != null)
                        codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(origem.Codigo)?.CodigoIntegracao ?? 0;

                    if (destino != null)
                        codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(destino.Codigo)?.CodigoIntegracao ?? 0;

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, cargaPercursoDestino);

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        List<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade> configuracaoIntegracaoLocalidade = repConfiguracaoIntegracaoLocalidade.BuscarTodos();

                        if (configuracaoIntegracaoLocalidade.Count == 0)
                            throw new ServicoException("Nenhum registro de localidade encontrado! Favor buscar os códigos OpenTech através do cadastro de Integração OpenTech");

                        if (codigoOrigemOpenTech == 0)
                            throw new ServicoException("Cidade " + origem?.DescricaoCidadeEstado + " não possui Código de Integração OpenTech");

                        if (codigoDestinoOpenTech == 0)
                            throw new ServicoException("Cidade " + cargaPercursoOrigem?.Destino?.DescricaoCidadeEstado + " não possui Código de Integração OpenTech");
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaIsca> cargaIscas = repositorioCargaIsca.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                    List<ServicoOpenTech.sgrIsca> listaCargaIscas = new List<ServicoOpenTech.sgrIsca>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaIsca cargaIsca in cargaIscas)
                    {
                        listaCargaIscas.Add(new ServicoOpenTech.sgrIsca()
                        {
                            cdemprastrea = cargaIsca.Isca.CodigoEmpresaIsca.ToInt(),
                            nrisca = cargaIsca.Isca.Descricao,
                            dssiteisca = cargaIsca.Isca.Site,
                            dsususiteisca = cargaIsca.Isca.Login,
                            dssenhasiteisca = cargaIsca.Isca.Senha,
                        });
                    }

                    string nrplacacavalo = cargaIntegracao.Carga.Veiculo.Placa;
                    string nrplacacarreta1 = veiculosVinculados.Count > 0 ? veiculosVinculados.First().Placa : string.Empty;
                    string nrplacacarreta2 = veiculosVinculados.Count > 1 ? veiculosVinculados.Last().Placa : string.Empty;
                    string nrdocmotorista1 = motoristas.Count > 0 ? motoristas.First().CPF : string.Empty;
                    string nrdocmotorista2 = string.Empty;
                    string motorista1 = motoristas.Count > 0 ? motoristas.First().Nome : string.Empty;
                    string motorista2 = string.Empty;
                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = motoristas.Count > 0 ? motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrdddfonecelular = string.Empty;
                    string nrcontrolecarga = cargaIntegracao.Carga.CodigoCargaEmbarcador;
                    int cdtipooperacao = 1; //Transferência
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal distanciatotal = 0m;
                    decimal pesocarga = cargaPedidos.Sum(o => o.Peso);
                    string cdvincmot1 = "";
                    string cdvincmot2 = "";
                    string ratreadorCavalor = "";
                    int cdEmprastrCavalo = 0;
                    string rastradorCarreta1 = "";
                    int cdEmprastrCarreta = 0;
                    decimal valorCarga = 0;
                    decimal valorTotalNotas = 0;

                    ServicoOpenTech.sgrSensorTemperatura[] listaSensorTemperatura = new ServicoOpenTech.sgrSensorTemperatura[1];
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                    if (tipoDeCargaSensorOpentech != null)
                    {
                        listaSensorTemperatura[0] = new ServicoOpenTech.sgrSensorTemperatura();
                        listaSensorTemperatura[0].cdTpSensTemp = tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech;
                        listaSensorTemperatura[0].nrSensor = 1;
                        listaSensorTemperatura[0].vlToleraSup = tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior;
                        listaSensorTemperatura[0].vlToleraInf = tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior;
                        listaSensorTemperatura[0].vlIdealSup = tipoDeCargaSensorOpentech.TemperaturaIdealSuperior;
                        listaSensorTemperatura[0].vlIdealInf = tipoDeCargaSensorOpentech.TemperaturaIdealInferior;
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    string dtprevini = (_configuracaoIntegracaoOpenTech?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false) ? (cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")) : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string dtprevfim = (_configuracaoIntegracaoOpenTech?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false) ? (pedidos.FirstOrDefault().PrevisaoEntrega.HasValue ? pedidos.FirstOrDefault().PrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")) : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    bool integrarDocumentosPorPedido = cargaIntegracao.Carga.TipoOperacao?.IntegrarPedidosNaIntegracaoOpentech ?? false;

                    if (_configuracaoIntegracaoOpenTech?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.AddHours(1).ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddHours(24).ToString("yyyy-MM-dd HH:mm");
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repCargaCTe.BuscarPorCargaSemPreCte(cargaIntegracao.Carga.Codigo);
                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2;

                    if (integrarDocumentosPorPedido)
                        documentosV2 = ObterListaDocumentosIntegracaoPorPedido(cargaIntegracao, out valorCarga, out valorTotalNotas, valorMaior, ref dtprevini, ref dtprevfim, tipoServicoMultisoftware, cdTrans, codigoEmbarcadorOpenTech, cargaCtes, cargaPedidos);
                    else
                    {
                        if (cargaIntegracao.Carga.CargaEspelho != null)
                        {
                            cargaCtes = repCargaCTe.BuscarPorCargaSemPreCte(cargaIntegracao.Carga.CargaEspelho.Codigo);
                            documentosV2 = ObterListaDocumentosIntegracaoPorCTe(cargaIntegracao, out valorCarga, out valorTotalNotas, valorMaior, ref dtprevini, ref dtprevfim, tipoServicoMultisoftware, cdTrans, codigoEmbarcadorOpenTech, cargaCtes, cargaPedidos, true);
                        }
                        else
                            documentosV2 = ObterListaDocumentosIntegracaoPorCTe(cargaIntegracao, out valorCarga, out valorTotalNotas, valorMaior, ref dtprevini, ref dtprevfim, tipoServicoMultisoftware, cdTrans, codigoEmbarcadorOpenTech, cargaCtes, cargaPedidos);
                    }

                    if (_configuracaoIntegracaoOpenTech?.EnviarDataAtualNaDataPrevisaoOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddMinutes(1).ToString("yyyy-MM-dd HH:mm");
                    }

                    List<int> codigosProdutos = (from obj in pedidosProdutos select obj.Produto.Codigo).Distinct().ToList();

                    decimal valorTotalProdutos = pedidosProdutos.Sum(o => o.ValorProduto * o.Quantidade);
                    decimal diferencaNotas = valorTotalNotas - valorTotalProdutos;

                    if (diferencaNotas < 0m)
                        diferencaNotas = 0m;

                    List<ServicoOpenTech.sgrProduto> produtos = new List<ServicoOpenTech.sgrProduto>();

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech prodOpentech = ObterProdutoOpenTechIntegracaoCarga(cargaIntegracao.Carga, cargaPercursoDestino, cargaCtes, valorTotalNotas);
                        int codigoProdutoOpentech = prodOpentech?.CodigoProdutoOpentech > 0 ? prodOpentech.CodigoProdutoOpentech : _configuracaoIntegracaoOpenTech.CodigoProdutoPadraoOpentech;

                        produtos.Add(new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = codigoProdutoOpentech,
                            valor = valorTotalNotas
                        });
                    }
                    else if (codigosProdutos.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech prodOpentech = ObterProdutoOpenTechIntegracaoCarga(cargaIntegracao.Carga, cargaPercursoOrigem, cargaCtes);

                        for (var i = 0; i < codigosProdutos.Count; i++)
                        {
                            int codigoProduto = codigosProdutos[i];

                            Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech grupoProdutoOpenTech = repGrupoProdutoOpenTech.BuscarPorGrupoProduto((from obj in pedidosProdutos where obj.Produto.Codigo == codigoProduto select obj.Produto.GrupoProduto?.Codigo ?? 0).First());

                            decimal valorProduto = (from obj in pedidosProdutos where obj.Produto.Codigo == codigoProduto select (obj.ValorProduto * obj.Quantidade)).Sum();

                            if (i == 0)
                                valorProduto += diferencaNotas;

                            int codigoProdutoOpentech = prodOpentech?.CodigoProdutoOpentech > 0 ? prodOpentech.CodigoProdutoOpentech : (valorMaior ? grupoProdutoOpenTech?.CodigoProdutoValorMaior ?? _configuracaoIntegracaoOpenTech.CodigoProdutoPadraoOpentech : grupoProdutoOpenTech?.CodigoProdutoValorMenor ?? _configuracaoIntegracaoOpenTech.CodigoProdutoPadraoOpentech);

                            ServicoOpenTech.sgrProduto sgrProduto = (from obj in produtos where obj.cdprod == codigoProdutoOpentech select obj).FirstOrDefault();

                            if (sgrProduto == null)
                            {
                                produtos.Add(new ServicoOpenTech.sgrProduto()
                                {
                                    cdprod = codigoProdutoOpentech,
                                    valor = valorProduto
                                });
                            }
                            else
                                sgrProduto.valor += valorProduto;
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech prodOpentech = ObterProdutoOpenTechIntegracaoCarga(cargaIntegracao.Carga, cargaPercursoDestino, cargaCtes, valorTotalNotas);
                        int codigoProdutoOpentech = prodOpentech?.CodigoProdutoOpentech > 0 ? prodOpentech.CodigoProdutoOpentech : _configuracaoIntegracaoOpenTech.CodigoProdutoPadraoOpentech;
                        produtos.Add(new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = codigoProdutoOpentech,
                            valor = valorTotalNotas
                        });
                    }

                    int cdrotaopentech = 0;
                    Servicos.ServicoOpenTech.Rota[] rotas = null;
                    Dominio.Entidades.RotaFrete rotaFrete = cargaIntegracao.Carga.Rota;

                    if (_configuracaoIntegracaoOpenTech.IntegrarRotaCargaOpentech && rotaFrete != null && rotaFrete.CodigoIntegracao.ToInt() > 0)
                    {
                        ServicoOpenTech.Rota rota = new ServicoOpenTech.Rota()
                        {
                            cdRotaModelo = rotaFrete.CodigoIntegracao.ToInt()
                        };

                        cdrotaopentech = -1;
                        rotas = new ServicoOpenTech.Rota[] { rota };
                    }

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    PreencherCamposConfiguracaoIntegracao(cargaIntegracao, null, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorCarga, valorCarga);
                    ServicoOpenTech.sgrData retornoIntegracao = null;
                    Servicos.Log.TratarErro(" Antes do sgrGerarAEv9");
                    DateTime tempo = DateTime.Now;

                    if (_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                    {

                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv10(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                             pesocarga, "", "", "", "", "", "", cargaIntegracao.Carga.SetPointVeiculo, cargaIntegracao.Carga.IntegracaoTemperatura, cargaIntegracao.Carga.CategoriaCargaEmbarcador, "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                             tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                             nrdddfonecelular, "", "", "", listaCargaIscas.ToArray(), rotas, null, null, null, null);

                            Servicos.Log.TratarErro($" Depois do sgrGerarAEv10. Tempo em milisegundos => {(DateTime.Now - tempo).TotalMilliseconds}");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($" Depois do sgrGerarAEv10 com error log . Tempo em milisegundos => {(DateTime.Now - tempo).TotalMilliseconds}");
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv9(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                             pesocarga, "", "", "", "", "", "", cargaIntegracao.Carga.SetPointVeiculo, cargaIntegracao.Carga.IntegracaoTemperatura, cargaIntegracao.Carga.CategoriaCargaEmbarcador, "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                             tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                             nrdddfonecelular, "", "", "", listaCargaIscas.ToArray(), rotas);

                            Servicos.Log.TratarErro($" Depois do sgrGerarAEv9. Tempo em milisegundos => {(DateTime.Now - tempo).TotalMilliseconds}");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($" Depois do sgrGerarAEv9 com error log . Tempo em milisegundos => {(DateTime.Now - tempo).TotalMilliseconds}");
                            Servicos.Log.TratarErro(ex);
                        }
                    }

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração - " + retornoIntegracao?.ReturnDescription ?? string.Empty);

                    if (retornoIntegracao == null || retornoIntegracao?.ReturnDataset == null || retornoIntegracao.ReturnDescription != "OK")
                    {
                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar";

                        mensagemErro += ObterMensagemErro(retornoIntegracao);

                        throw new ServicoException(Utilidades.String.Left(mensagemErro, 300));
                    }

                    int cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);

                    cargaIntegracao.Protocolo = cdviag.ToString();
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cdviag + ".";
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração carga com a OpenTech";
            }

            repCargaCargaIntegracao.Atualizar(cargaIntegracao);

            if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
        }


        /// <summary>
        /// Método utilizado quando NFe é após o frete
        /// </summary>
        public void IntegrarCargaEtapaTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao.IntegracaoColeta)
                IntegrarCargaColeta(cargaIntegracao);
            else
            {
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoColeta = repCargaCargaIntegracao.BuscarPorCargaTipoIntegracaoColetaSituacao(cargaIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoSM = repCargaCargaIntegracao.BuscarPorCargaTipoIntegracaoColetaSituacao(cargaIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                if (cargaIntegracaoColeta == null && cargaIntegracaoSM == null)
                    IntegrarCargaTransporte(cargaIntegracao);
                else
                    IntegrarAtualizacaoCargaColeta(cargaIntegracao, cargaIntegracaoColeta?.Protocolo ?? cargaIntegracaoSM?.Protocolo);
            }
        }

        /// <summary>
        /// Método utilizado quando NFe é antes do frete
        /// </summary>
        public void IntegrarCargaEtapaTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            if (cargaDadosTransporteIntegracao.IntegracaoColeta)
                IntegrarCargaColeta(cargaDadosTransporteIntegracao);
            else
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracaoColeta = repCargaCargaIntegracao.BuscarPorCargaTipoIntegracaoColetaSituacao(cargaDadosTransporteIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracaoSM = repCargaCargaIntegracao.BuscarPorCargaTipoIntegracaoColetaSituacao(cargaDadosTransporteIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

                if (cargaIntegracaoColeta == null && cargaIntegracaoSM == null)
                    IntegrarCargaTransporte(cargaDadosTransporteIntegracao);
                else
                    IntegrarAtualizacaoCargaColeta(cargaDadosTransporteIntegracao, cargaIntegracaoColeta?.Protocolo ?? cargaIntegracaoSM.Protocolo);
            }
        }

        public void IntegrarCargaTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork, true);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga.Empresa);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                IList<int> codigosVeiculosVinculados = repVeiculo.BuscarVeiculosVinculadoACarga(cargaIntegracao.Carga.Codigo);
                List<Dominio.Entidades.Veiculo> veiculosVinculados = repVeiculo.BuscarPorCodigos(codigosVeiculosVinculados, false);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaIntegracao.ProblemaIntegracao = "A configuração de integração para a OpenTech é inválida.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<Servicos.ServicoOpenTech.sgrOpentechSoapClient, Servicos.ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector, retornoLogin);

                    if (cdTrans == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Transportador não possui cadastro na OpenTech.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (_configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista)
                    {
                        if (!IntegrarCadastros(cargaIntegracao, retornoLogin))
                            return;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso origem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);

                    if (destino == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um Destino para a carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (cargaIntegracao.Carga.Veiculo == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Carga sem veículo";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repCargaCTe.BuscarPorCargaSemPreCte(cargaIntegracao.Carga.Codigo);

                    Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = ObterProdutoOpenTechIntegracaoCarga(cargaIntegracao.Carga, destino, cargaCtes);

                    if (produtoOpentech == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um produto opentech configurado para esta carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repositorioCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                    }

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, destino);
                    int codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(origem != null ? origem.Origem.Codigo : (cargaIntegracao.Carga.Filial?.Localidade.Codigo ?? cargaPedidos.FirstOrDefault().Origem.Codigo))?.CodigoIntegracao ?? 0;
                    int codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(destino.Destino.Codigo)?.CodigoIntegracao ?? 0;

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        if (codigoOrigemOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + origem != null ? origem.Origem.DescricaoCidadeEstado : (cargaIntegracao.Carga.Filial?.Localidade.Descricao ?? "") + " não possui Código de Integração OpenTech";
                        else if (codigoDestinoOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + destino.Destino.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    string nrplacacavalo = cargaIntegracao.Carga.Veiculo.Placa;
                    string nrplacacarreta1 = veiculosVinculados.Count > 0 ? veiculosVinculados.First().Placa : string.Empty;
                    string nrplacacarreta2 = veiculosVinculados.Count > 1 ? veiculosVinculados.Last().Placa : string.Empty;
                    string nrdocmotorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().CPF : string.Empty;
                    string nrdocmotorista2 = string.Empty;
                    string motorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Nome : string.Empty;
                    string motorista2 = string.Empty;
                    string dtprevini = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string dtprevfim = (_configuracaoIntegracaoOpenTech?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false) ? (pedidos.FirstOrDefault().PrevisaoEntrega.HasValue ? pedidos.FirstOrDefault().PrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")) : (cargaIntegracao.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaIntegracao.Carga.DataPrevisaoTerminoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    int cdrotaopentech = 0;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrdddfonecelular = string.Empty;
                    string nrcontrolecarga = cargaIntegracao.Carga.CodigoCargaEmbarcador;
                    int cdtipooperacao = 1; //Transferência
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal distanciatotal = 0m;
                    decimal pesocarga = cargaPedidos.Sum(o => o.Peso);
                    string cdvincmot1 = "";
                    string cdvincmot2 = "";
                    string ratreadorCavalor = "";
                    int cdEmprastrCavalo = 0;
                    string rastradorCarreta1 = "";
                    int cdEmprastrCarreta = 0;
                    decimal valorCarga = 0;
                    var listaSensorTemperatura = new ServicoOpenTech.sgrSensorTemperatura[1];
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                    if (_configuracaoIntegracaoOpenTech?.EnviarDataAtualNaDataPrevisaoOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddMinutes(1).ToString("yyyy-MM-dd HH:mm");
                    }

                    if (_configuracaoIntegracaoOpenTech?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.AddHours(1).ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddHours(24).ToString("yyyy-MM-dd HH:mm");
                    }

                    if (tipoDeCargaSensorOpentech != null)
                    {
                        listaSensorTemperatura[0] = new ServicoOpenTech.sgrSensorTemperatura();
                        listaSensorTemperatura[0].cdTpSensTemp = tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech;
                        listaSensorTemperatura[0].nrSensor = tipoDeCargaSensorOpentech.QuantidadeSensores;
                        listaSensorTemperatura[0].vlToleraSup = tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior;
                        listaSensorTemperatura[0].vlToleraInf = tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior;
                        listaSensorTemperatura[0].vlIdealSup = tipoDeCargaSensorOpentech.TemperaturaIdealSuperior;
                        listaSensorTemperatura[0].vlIdealInf = tipoDeCargaSensorOpentech.TemperaturaIdealInferior;
                    }

                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutos = (from obj in cargaPedidos select obj.Produtos).SelectMany(o => o).ToList();

                    bool veiculoPermiteLocalizador = (cargaIntegracao.Carga.Veiculo?.ModeloVeicularCarga?.ModeloVeicularAceitaLocalizador ?? false) && (cargaIntegracao.Carga.Veiculo?.PossuiLocalizador ?? false);
                    int codProd = veiculoPermiteLocalizador ? _configuracaoIntegracaoOpenTech.CodigoProdutoVeiculoComLocalizadorOpenTech : 0;
                    if (codProd == 0 && produtoOpentech != null)
                        codProd = produtoOpentech?.CodigoProdutoOpentech ?? 0;

                    int codigoPedido = cargaPedidos.FirstOrDefault().Codigo;
                    bool enviarApenasPrimeiroPedidoNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false;
                    bool enviarInformacoesTotaisDaCargaNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarInformacoesTotaisDaCargaNaOpentech ?? false;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (enviarApenasPrimeiroPedidoNaOpentech && cargaPedido.Codigo != codigoPedido)
                            break;

                        if (origem == null && cargaPedido.Pedido.Expedidor != null)
                        {
                            codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Expedidor.Localidade.Codigo)?.CodigoIntegracao ?? 0;

                            if (codigoOrigemOpenTech == 0)
                            {
                                cargaIntegracao.ProblemaIntegracao = "Cidade " + cargaPedido.Pedido.Expedidor.Localidade.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                                return;
                            }

                            cdcidibgeorigem = codigoOrigemOpenTech;
                        }

                        if (enviarApenasPrimeiroPedidoNaOpentech && enviarInformacoesTotaisDaCargaNaOpentech)
                            valorCarga = pedidos.Select(x => x.ValorTotalCarga).Sum();
                        else
                            valorCarga += cargaPedido.Pedido.ValorTotalCarga;

                        decimal valorTotalProdutosPedido = cargaPedidosProdutos.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).Sum(o => o.ValorUnitarioProduto * o.Quantidade);

                        int cdCid = cargaPedido.Pedido.Recebedor != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0 : cargaPedido.Pedido.Destinatario != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Destinatario.Localidade.Codigo)?.CodigoIntegracao ?? 0 : 0;
                        int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                        int cdPaisOrigemEmitente = codigoBrasilOpenTech;
                        int cdProgramacao = 0;
                        int cdTrocaNota = 0;
                        int flRegiao = 0;

                        string dsNavio = "";
                        string dsSiglaDest = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Recebedor.Localidade.Estado.Sigla : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla : "";
                        string dsSiglaOrig = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Expedidor.Localidade.Estado.Sigla : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla : "";
                        string nrCnpjCpfEmitente = cargaIntegracao.Carga.Empresa.CNPJ;
                        string nrDDDFone1 = "";
                        string nrDDDFone2 = "";
                        string nrLacreArmador = "";
                        string nrLacreSIF = "";

                        List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();

                        List<ServicoOpenTech.sgrProduto> produtosPedido = new List<ServicoOpenTech.sgrProduto>
                    {
                        new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = codProd,
                            valor = valorTotalProdutosPedido
                        }
                    };

                        DateTime dtPrevista = cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.HasValue ? cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.Value : cargaPedido.Pedido.DataCriacao.Value.AddDays(1);
                        if (_configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false)
                            if (cargaPedido.Pedido?.NotasFiscais?.FirstOrDefault()?.DataEmissao != null)
                                dtPrevista = cargaPedido.Pedido.NotasFiscais.FirstOrDefault().DataEmissao;

                        PreencherCamposConfiguracaoIntegracao(cargaIntegracao, null, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorTotalProdutosPedido, valorCarga);

                        documentosV2.Add(new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
                        {
                            nrDoc = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            tpDoc = 3, //Pedido
                            valorDoc = valorTotalProdutosPedido,
                            tpOperacao = 3, //Entrega
                            dtPrevista = dtPrevista,
                            dtPrevistaSaida = "",
                            dsRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Endereco, 50) : string.Empty,
                            nrRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Numero, 6) : string.Empty,
                            complementoRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Complemento, 40) : string.Empty,
                            dsBairro = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Bairro, 50) : string.Empty,
                            cdCidIBGE = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.Localidade.CodigoIBGE : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE : 0,
                            nrCep = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Pedido.Recebedor.CEP : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CEP : string.Empty,
                            nrFone1 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone1) : string.Empty,
                            nrFone2 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone2) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone2) : string.Empty,
                            nrCnpjCPFDestinatario = ObterCnpjCPFDestinatario(cargaPedido),
                            nrCnpjCpfDestinatarioSequencia = "",
                            Latitude = 0f,
                            Longitude = 0f,
                            dsNome = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Nome, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Nome, 50) : string.Empty,
                            qtVolumes = cargaPedido.Pedido.QtVolumes,
                            qtPecas = cargaPedido.Pedido.QtVolumes,
                            nrControleCliente1 = "",
                            nrControleCliente2 = "",
                            nrControleCliente3 = "",
                            nrControleCliente7 = "",
                            nrControleCliente8 = "",
                            nrControleCliente9 = "",
                            nrControleCliente10 = "",
                            produtos = produtosPedido.ToArray(),
                            vlCubagem = 0,
                            vlPeso = (int)cargaPedido.Pedido.PesoTotal,
                            cdTransp = cdTrans,
                            flTrocaNota = 0,
                            cdProgramacaoDestinatario = 0,
                            cdCid = cdCid,
                            cdEmbarcador = codigoEmbarcadorOpenTech,
                            cdPaisOrigemDestinatario = cargaPedido.Pedido.Recebedor != null && cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Recebedor.Localidade?.Pais?.Descricao ?? "") : cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "") : cdPaisOrigemDestinatario,
                            cdPaisOrigemEmitente = cdPaisOrigemEmitente,
                            cdProgramacao = cdProgramacao,
                            cdTrocaNota = cdTrocaNota,
                            dsNavio = dsNavio,
                            dsSiglaDest = dsSiglaDest,
                            dsSiglaOrig = dsSiglaOrig,
                            flRegiao = flRegiao,
                            nfs = nfsOpen.ToArray(),
                            nrCnpjCpfEmitente = nrCnpjCpfEmitente,
                            nrDDDFone1 = nrDDDFone1,
                            nrDDDFone2 = nrDDDFone2,
                            nrLacreArmador = nrLacreArmador,
                            nrLacreSIF = nrLacreSIF,
                            flReentrega = 0,
                            vlInicioDiariaAtrasado = 0,
                            vlInicioDiariaNoPrazo = 0
                        });


                        //Cadastrar recebedor
                        if (cargaPedido.Pedido.Recebedor != null)
                        {
                            ServicoOpenTech.sgrData retornoIntegracaoRecebedor = null;
                            try
                            {
                                List<Servicos.ServicoOpenTech.sgrDestinatariosCliente> arrClientes = new List<ServicoOpenTech.sgrDestinatariosCliente>();

                                arrClientes.Add(new ServicoOpenTech.sgrDestinatariosCliente()
                                {
                                    strFlTipoPessoa = "F",
                                    strnrCGCCPF = cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? cargaPedido.Pedido.Recebedor.CodigoIntegracao : cargaPedido.Pedido.Recebedor.CPF_CNPJ_SemFormato,
                                    strRazaoSocial = cargaPedido.Pedido.Recebedor.Nome,
                                    strNomeDestinatario = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Recebedor.NomeFantasia) ? cargaPedido.Pedido.Recebedor.NomeFantasia : cargaPedido.Pedido.Recebedor.Nome,
                                    cdCidade = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0,
                                    strEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50),
                                    cdNrRua = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6),
                                    strComplementoEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40),
                                    strBairro = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50),
                                    strCEP = cargaPedido.Pedido.Recebedor.CEP,
                                    strFone = Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1),
                                    strEmail = cargaPedido.Pedido.Recebedor.Email,
                                    strObservacoes = string.Empty,
                                    vlLat = 0f,
                                    vlLong = 0f,
                                    cdPais = codigoBrasilOpenTech,
                                    cdTipoDestinatario = 4,
                                    strSigla = cargaPedido.Recebedor.CodigoIntegracao
                                });

                                retornoIntegracaoRecebedor = svcOpenTech.sgrCadastroDestinatarios(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, arrClientes.ToArray());

                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                            }

                            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> cargaCTeIntegracaoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                            cargaCTeIntegracaoArquivo.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração Recebedor - " + retornoIntegracaoRecebedor?.ReturnDescription ?? "");
                        }
                    }

                    decimal valorTotalNotas = (from obj in cargaPedidos select obj.Pedido).Sum(o => o.ValorTotalNotasFiscais);
                    decimal valorTotalProdutos = cargaPedidosProdutos.Sum(o => o.ValorUnitarioProduto * o.Quantidade);
                    decimal diferencaNotas = valorTotalNotas - valorTotalProdutos;

                    if (diferencaNotas < 0m)
                        diferencaNotas = 0m;

                    List<ServicoOpenTech.sgrProduto> produtos = new List<ServicoOpenTech.sgrProduto>();


                    produtos.Add(new ServicoOpenTech.sgrProduto()
                    {
                        cdprod = codProd,
                        valor = valorTotalProdutos
                    });

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    ServicoOpenTech.sgrData retornoIntegracao = null;
                    if (_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                    {

                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv10(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                             pesocarga, "", "", "", "", "", "", cargaIntegracao.Carga.SetPointVeiculo, cargaIntegracao.Carga.IntegracaoTemperatura, cargaIntegracao.Carga.CategoriaCargaEmbarcador, "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                             tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                             nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, null, null, null, null, null);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv9(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                            pesocarga, "", "", "", "", "", "", "", "", "", "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                            tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                            nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, null);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                    if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK")
                    {
                        int cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);

                        cargaIntegracao.Protocolo = cdviag.ToString();
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cdviag + ".";
                    }
                    else
                    {
                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar";

                        mensagemErro += ObterMensagemErro(retornoIntegracao);

                        //Tratativa para quando integração já tem número de protocolo e é o mesmo número de viagem retornado da Opentech
                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo) && mensagemErro.Contains("Existe uma viagem em andamento") && mensagemErro.Contains(cargaIntegracao.Protocolo))
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cargaIntegracao.Protocolo + ".";
                        }
                        else
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                        }
                    }

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = "1 - Não foi possível comunicar com os serviços da Opentech";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                return;
            }
        }

        public void IntegrarCargaTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(_unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                IList<int> codigosVeiculosVinculados = repVeiculo.BuscarVeiculosVinculadoACarga(cargaIntegracao.Carga.Codigo);
                List<Dominio.Entidades.Veiculo> veiculosVinculados = repVeiculo.BuscarPorCodigos(codigosVeiculosVinculados, false);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaIntegracao.ProblemaIntegracao = "A configuração de integração para a OpenTech é inválida.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector, retornoLogin);

                    if (cdTrans == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Transportador não possui cadastro na OpenTech.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (_configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista && !IntegrarCadastros(cargaIntegracao, retornoLogin))
                        return;

                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso origem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);

                    var localidadeOrigem = origem != null ? origem.Origem : (cargaIntegracao.Carga.Filial?.Localidade ?? cargaPedidos.LastOrDefault().Origem);
                    var localidadeDestino = destino != null ? destino.Destino : cargaPedidos.LastOrDefault().Destino;

                    if (cargaIntegracao.Carga.Veiculo == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Carga sem veículo";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> produtosOpenTech = new List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>();
                    Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = null;

                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroCarga = repositorioApoliceSeguroAverbacao.BuscarApolicesPorCarga(cargaIntegracao.Carga.Codigo);

                    decimal valorNotas = (from obj in cargaPedidos select obj.Pedido).Sum(o => o.ValorTotalNotasFiscais);

                    if (apolicesSeguroCarga.Count > 0)
                    {
                        if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0, localidadeDestino.Codigo);
                        else
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                        produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorNotas && x.ValorAte >= valorNotas).FirstOrDefault();

                        if (produtoOpentech == null)
                            produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
                    }

                    if (produtoOpentech == null)
                    {
                        if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeSemApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0, localidadeDestino.Codigo);
                        else
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoSemApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                        produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorNotas && x.ValorAte >= valorNotas).FirstOrDefault();

                        if (produtoOpentech == null)
                            produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
                    }

                    if (produtoOpentech == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um produto opentech configurado para esta carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, destino);
                    int codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(localidadeOrigem.Codigo)?.CodigoIntegracao ?? 0;
                    int codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(localidadeDestino.Codigo)?.CodigoIntegracao ?? 0;

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        if (codigoOrigemOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = $"Cidade {(localidadeOrigem != null ? localidadeOrigem.DescricaoCidadeEstado : cargaIntegracao.Carga.Filial?.Localidade.Descricao ?? string.Empty)} não possui Código de Integração OpenTech";
                        else if (codigoDestinoOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = $"Cidade {localidadeDestino.DescricaoCidadeEstado} não possui Código de Integração OpenTech";

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    string nrplacacavalo = cargaIntegracao.Carga.Veiculo.Placa;
                    string nrplacacarreta1 = veiculosVinculados.Count > 0 ? veiculosVinculados.First().Placa : string.Empty;
                    string nrplacacarreta2 = veiculosVinculados.Count > 1 ? veiculosVinculados.Last().Placa : string.Empty;
                    string nrdocmotorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().CPF : string.Empty;
                    string nrdocmotorista2 = string.Empty;
                    string motorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Nome : string.Empty;
                    string motorista2 = string.Empty;
                    string dtprevini = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string dtprevfim = (_configuracaoIntegracaoOpenTech?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false) ? (pedidos.FirstOrDefault().PrevisaoEntrega.HasValue ? pedidos.FirstOrDefault().PrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")) : (cargaIntegracao.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaIntegracao.Carga.DataPrevisaoTerminoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    int cdrotaopentech = 0;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrdddfonecelular = string.Empty;
                    string nrcontrolecarga = cargaIntegracao.Carga.CodigoCargaEmbarcador;
                    int cdtipooperacao = 1; //Transferência
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal distanciatotal = 0m;
                    decimal pesocarga = cargaPedidos.Sum(o => o.Peso);
                    string cdvincmot1 = "";
                    string cdvincmot2 = "";
                    string ratreadorCavalor = "";
                    int cdEmprastrCavalo = 0;
                    string rastradorCarreta1 = "";
                    int cdEmprastrCarreta = 0;
                    decimal valorCarga = 0;
                    var listaSensorTemperatura = new ServicoOpenTech.sgrSensorTemperatura[1];
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                    if (_configuracaoIntegracaoOpenTech?.EnviarDataAtualNaDataPrevisaoOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddMinutes(1).ToString("yyyy-MM-dd HH:mm");
                    }

                    if (_configuracaoIntegracaoOpenTech?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.AddHours(1).ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddHours(24).ToString("yyyy-MM-dd HH:mm");
                    }

                    if (tipoDeCargaSensorOpentech != null)
                    {
                        listaSensorTemperatura[0] = new ServicoOpenTech.sgrSensorTemperatura();
                        listaSensorTemperatura[0].cdTpSensTemp = tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech;
                        listaSensorTemperatura[0].nrSensor = tipoDeCargaSensorOpentech.QuantidadeSensores;
                        listaSensorTemperatura[0].vlToleraSup = tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior;
                        listaSensorTemperatura[0].vlToleraInf = tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior;
                        listaSensorTemperatura[0].vlIdealSup = tipoDeCargaSensorOpentech.TemperaturaIdealSuperior;
                        listaSensorTemperatura[0].vlIdealInf = tipoDeCargaSensorOpentech.TemperaturaIdealInferior;
                    }

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutos = (from obj in cargaPedidos select obj.Produtos).SelectMany(o => o).ToList();

                    int codigoPedido = cargaPedidos.FirstOrDefault().Codigo;
                    bool enviarApenasPrimeiroPedidoNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false;
                    bool enviarInformacoesTotaisDaCargaNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarInformacoesTotaisDaCargaNaOpentech ?? false;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (enviarApenasPrimeiroPedidoNaOpentech && cargaPedido.Codigo != codigoPedido)
                            break;

                        if (origem == null && cargaPedido.Pedido.Expedidor != null)
                        {
                            codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Expedidor.Localidade.Codigo)?.CodigoIntegracao ?? 0;

                            if (codigoOrigemOpenTech == 0)
                            {
                                cargaIntegracao.ProblemaIntegracao = "Cidade " + cargaPedido.Pedido.Expedidor.Localidade.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";

                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                                return;
                            }

                            cdcidibgeorigem = codigoOrigemOpenTech;
                        }

                        if (enviarApenasPrimeiroPedidoNaOpentech && enviarInformacoesTotaisDaCargaNaOpentech)
                            valorCarga = pedidos.Select(x => x.ValorTotalCarga).Sum();
                        else
                            valorCarga += cargaPedido.Pedido.ValorTotalCarga;

                        decimal valorTotalProdutosPedido = cargaPedidosProdutos.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).Sum(o => o.ValorUnitarioProduto * o.Quantidade);

                        int cdCid = cargaPedido.Pedido.Recebedor != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0 : cargaPedido.Pedido.Destinatario != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Destinatario.Localidade.Codigo)?.CodigoIntegracao ?? 0 : 0;
                        int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                        int cdPaisOrigemEmitente = codigoBrasilOpenTech;
                        int cdProgramacao = 0;
                        int cdTrocaNota = 0;
                        int flRegiao = 0;

                        string dsNavio = "";
                        string dsSiglaDest = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Recebedor.Localidade.Estado.Sigla : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla : "";
                        string dsSiglaOrig = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Expedidor.Localidade.Estado.Sigla : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla : "";

                        string nrCnpjCpfEmitente = cargaIntegracao.Carga.Empresa.CNPJ;
                        string nrDDDFone1 = "";
                        string nrDDDFone2 = "";
                        string nrLacreArmador = "";
                        string nrLacreSIF = "";

                        List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();
                        List<ServicoOpenTech.sgrProduto> produtosPedido = new List<ServicoOpenTech.sgrProduto>
                    {
                        new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = produtoOpentech.CodigoProdutoOpentech,
                            valor = valorTotalProdutosPedido
                        }
                    };

                        DateTime dtPrevista = cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.HasValue ? cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.Value : cargaPedido.Pedido.DataCriacao.Value.AddDays(1);

                        if ((_configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false) && cargaPedido.Pedido?.NotasFiscais?.FirstOrDefault()?.DataEmissao != null)
                            dtPrevista = cargaPedido.Pedido.NotasFiscais.FirstOrDefault().DataEmissao;

                        PreencherCamposConfiguracaoIntegracao(null, cargaIntegracao, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorTotalProdutosPedido, valorCarga);

                        documentosV2.Add(new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
                        {
                            nrDoc = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            tpDoc = 3, //Pedido
                            valorDoc = valorTotalProdutosPedido,
                            tpOperacao = 3, //Entrega
                            dtPrevista = dtPrevista,
                            dtPrevistaSaida = "",
                            dsRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Endereco, 50) : string.Empty,
                            nrRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Numero, 6) : string.Empty,
                            complementoRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Complemento, 40) : string.Empty,
                            dsBairro = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Bairro, 50) : string.Empty,
                            cdCidIBGE = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.Localidade.CodigoIBGE : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE : 0,
                            nrCep = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Pedido.Recebedor.CEP : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CEP : string.Empty,
                            nrFone1 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone1) : string.Empty,
                            nrFone2 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone2) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone2) : string.Empty,
                            nrCnpjCPFDestinatario = ObterCnpjCPFDestinatario(cargaPedido),
                            nrCnpjCpfDestinatarioSequencia = "",
                            Latitude = 0f,
                            Longitude = 0f,
                            dsNome = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Nome, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Nome, 50) : string.Empty,
                            qtVolumes = cargaPedido.Pedido.QtVolumes,
                            qtPecas = cargaPedido.Pedido.QtVolumes,
                            nrControleCliente1 = "",
                            nrControleCliente2 = "",
                            nrControleCliente3 = "",
                            nrControleCliente7 = "",
                            nrControleCliente8 = "",
                            nrControleCliente9 = "",
                            nrControleCliente10 = "",
                            produtos = produtosPedido.ToArray(),
                            vlCubagem = 0,
                            vlPeso = (int)cargaPedido.Pedido.PesoTotal,
                            cdTransp = cdTrans,
                            flTrocaNota = 0,
                            cdProgramacaoDestinatario = 0,
                            cdCid = cdCid,
                            cdEmbarcador = codigoEmbarcadorOpenTech,
                            cdPaisOrigemDestinatario = cargaPedido.Pedido.Recebedor != null && cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Recebedor.Localidade?.Pais?.Descricao ?? "") : cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "") : cdPaisOrigemDestinatario,
                            cdPaisOrigemEmitente = cdPaisOrigemEmitente,
                            cdProgramacao = cdProgramacao,
                            cdTrocaNota = cdTrocaNota,
                            dsNavio = dsNavio,
                            dsSiglaDest = dsSiglaDest,
                            dsSiglaOrig = dsSiglaOrig,
                            flRegiao = flRegiao,
                            nfs = nfsOpen.ToArray(),
                            nrCnpjCpfEmitente = nrCnpjCpfEmitente,
                            nrDDDFone1 = nrDDDFone1,
                            nrDDDFone2 = nrDDDFone2,
                            nrLacreArmador = nrLacreArmador,
                            nrLacreSIF = nrLacreSIF,
                            flReentrega = 0,
                            vlInicioDiariaAtrasado = 0,
                            vlInicioDiariaNoPrazo = 0
                        });

                        //Cadastrar recebedor
                        if (cargaPedido.Pedido.Recebedor != null)
                        {
                            ServicoOpenTech.sgrData retornoIntegracaoRecebedor = null;

                            try
                            {
                                List<Servicos.ServicoOpenTech.sgrDestinatariosCliente> arrClientes = new List<ServicoOpenTech.sgrDestinatariosCliente>
                            {
                                new ServicoOpenTech.sgrDestinatariosCliente()
                                {
                                    strFlTipoPessoa = "F",
                                    strnrCGCCPF = cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? cargaPedido.Pedido.Recebedor.CodigoIntegracao : cargaPedido.Pedido.Recebedor.CPF_CNPJ_SemFormato,
                                    strRazaoSocial = cargaPedido.Pedido.Recebedor.Nome,
                                    strNomeDestinatario = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Recebedor.NomeFantasia) ? cargaPedido.Pedido.Recebedor.NomeFantasia : cargaPedido.Pedido.Recebedor.Nome,
                                    cdCidade = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0,
                                    strEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50),
                                    cdNrRua = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6),
                                    strComplementoEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40),
                                    strBairro = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50),
                                    strCEP = cargaPedido.Pedido.Recebedor.CEP,
                                    strFone = Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1),
                                    strEmail = cargaPedido.Pedido.Recebedor.Email,
                                    strObservacoes = string.Empty,
                                    vlLat = 0f,
                                    vlLong = 0f,
                                    cdPais = codigoBrasilOpenTech,
                                    cdTipoDestinatario = 4,
                                    strSigla = cargaPedido.Recebedor.CodigoIntegracao
                                }
                            };

                                retornoIntegracaoRecebedor = svcOpenTech.sgrCadastroDestinatarios(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, arrClientes.ToArray());
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                            }

                            servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração Recebedor - " + retornoIntegracaoRecebedor?.ReturnDescription ?? "");
                        }
                    }

                    decimal valorTotalNotas = (from obj in cargaPedidos select obj.Pedido).Sum(o => o.ValorTotalNotasFiscais);
                    decimal valorTotalProdutos = cargaPedidosProdutos.Sum(o => o.ValorUnitarioProduto * o.Quantidade);

                    List<ServicoOpenTech.sgrProduto> produtos = new List<ServicoOpenTech.sgrProduto>
                {
                    new ServicoOpenTech.sgrProduto()
                    {
                        cdprod = produtoOpentech.CodigoProdutoOpentech,
                        valor = valorTotalProdutos
                    }
                };

                    Servicos.ServicoOpenTech.Rota[] rotas = null;
                    Dominio.Entidades.RotaFrete rotaFrete = cargaIntegracao.Carga.Rota;

                    if ((_configuracaoIntegracaoOpenTech.IntegrarRotaCargaOpentech || _configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10) && rotaFrete != null)
                    {
                        ServicoOpenTech.Rota rota = new ServicoOpenTech.Rota()
                        {
                            PolylineFull = rotaFrete.PolilinhaRota
                        };

                        if (!_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                            rota.cdRotaModelo = rotaFrete.CodigoIntegracao.ToInt();

                        cdrotaopentech = -1;
                        rotas = new ServicoOpenTech.Rota[] { rota };
                    }

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    ServicoOpenTech.sgrData retornoIntegracao = null;
                    if (_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv10(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                             pesocarga, "", "", "", "", "", "", cargaIntegracao.Carga.SetPointVeiculo, cargaIntegracao.Carga.IntegracaoTemperatura, cargaIntegracao.Carga.CategoriaCargaEmbarcador, "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                             tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                             nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, rotas, null, null, null, null);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv9(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                            pesocarga, "", "", "", "", "", "", "", "", "", "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                            tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                            nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, rotas);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                    if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK")
                    {
                        int cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);

                        cargaIntegracao.Protocolo = cdviag.ToString();
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cdviag + ".";
                    }
                    else
                    {
                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar";

                        mensagemErro += ObterMensagemErro(retornoIntegracao);

                        //Tratativa para quando integração já tem número de protocolo e é o mesmo número de viagem retornado da Opentech
                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo) && mensagemErro.Contains("Existe uma viagem em andamento") && mensagemErro.Contains(cargaIntegracao.Protocolo))
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cargaIntegracao.Protocolo + ".";
                        }
                        else
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                        }
                    }

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = "2 - Não foi possível comunicar com os serviços da Opentech.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                return;
            }
        }

        public void IntegrarCargaColeta(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(_unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                IList<int> codigosVeiculosVinculados = repVeiculo.BuscarVeiculosVinculadoACarga(cargaIntegracao.Carga.Codigo);
                List<Dominio.Entidades.Veiculo> veiculosVinculados = repVeiculo.BuscarPorCodigos(codigosVeiculosVinculados, false);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaIntegracao.ProblemaIntegracao = "A configuração de integração para a OpenTech é inválida.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector, retornoLogin);

                    if (cdTrans == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Transportador não possui cadastro na OpenTech.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (_configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista && !IntegrarCadastros(cargaIntegracao, retornoLogin))
                        return;

                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso origem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);

                    if (destino == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um Destino para a carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (cargaIntegracao.Carga.Veiculo == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Carga sem veículo";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int codProd = ObterCodigoProdutoOpenTech(cargaIntegracao.Carga);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> listaApoliceSeguraAverbacao = repApoliceSeguroAverbacao.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    if (listaApoliceSeguraAverbacao != null && listaApoliceSeguraAverbacao.Count > 0 && codProd == 0)
                    {
                        if (listaApoliceSeguraAverbacao.FirstOrDefault().ApoliceSeguro.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador)
                            codProd = _configuracaoIntegracaoOpenTech.CodigoProdutoColetaEmbarcadorOpentech;
                        else
                            codProd = _configuracaoIntegracaoOpenTech.CodigoProdutoColetaTransportadorOpentech;
                    }

                    if (codProd == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um produto Coleta opentech configurado";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = repProdutoOpentech.BuscarPorCodigoProdutoOpentTechETipoOperacao(codProd, cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                    if (produtoOpentech == null)
                        produtoOpentech = repProdutoOpentech.BuscarPorCodigoProdutoOpentTech(codProd);

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, destino);
                    int codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(origem != null ? origem.Origem.Codigo : (cargaIntegracao.Carga.Filial?.Localidade.Codigo ?? cargaPedidos.FirstOrDefault().Origem.Codigo))?.CodigoIntegracao ?? 0;
                    int codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(destino.Destino.Codigo)?.CodigoIntegracao ?? 0;

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        if (codigoOrigemOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + origem != null ? origem.Origem.DescricaoCidadeEstado : (cargaIntegracao.Carga.Filial?.Localidade.Descricao ?? "") + " não possui Código de Integração OpenTech";
                        else if (codigoDestinoOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + destino.Destino.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    string nrplacacavalo = cargaIntegracao.Carga.Veiculo.Placa;
                    string nrplacacarreta1 = veiculosVinculados.Count > 0 ? veiculosVinculados.First().Placa : string.Empty;
                    string nrplacacarreta2 = veiculosVinculados.Count > 1 ? veiculosVinculados.Last().Placa : string.Empty;
                    string nrdocmotorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().CPF : string.Empty;
                    string nrdocmotorista2 = string.Empty;
                    string motorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Nome : string.Empty;
                    string motorista2 = string.Empty;
                    string dtprevini = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string dtprevfim = (_configuracaoIntegracaoOpenTech?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false) ? (pedidos.FirstOrDefault().PrevisaoEntrega.HasValue ? pedidos.FirstOrDefault().PrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")) : (cargaIntegracao.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaIntegracao.Carga.DataPrevisaoTerminoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    int cdrotaopentech = 0;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrdddfonecelular = string.Empty;
                    string nrcontrolecarga = cargaIntegracao.Carga.CodigoCargaEmbarcador;
                    int cdtipooperacao = 2; //Coleta
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal distanciatotal = 0m;
                    decimal pesocarga = cargaPedidos.Sum(o => o.Peso);
                    string cdvincmot1 = "";
                    string cdvincmot2 = "";
                    string ratreadorCavalor = (cargaIntegracao.Carga.Veiculo?.PossuiRastreador ?? false) ? (cargaIntegracao.Carga.Veiculo?.NumeroEquipamentoRastreador ?? string.Empty) : string.Empty;
                    int cdEmprastrCavalo = 0;
                    string rastradorCarreta1 = veiculosVinculados.Count > 0 ? veiculosVinculados.First().NumeroEquipamentoRastreador : string.Empty;
                    int cdEmprastrCarreta = 0;
                    decimal valorCarga = 0;

                    if (_configuracaoIntegracaoOpenTech?.EnviarDataAtualNaDataPrevisaoOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddMinutes(1).ToString("yyyy-MM-dd HH:mm");
                    }

                    if (_configuracaoIntegracaoOpenTech?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.AddHours(1).ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddHours(24).ToString("yyyy-MM-dd HH:mm");
                    }

                    var listaSensorTemperatura = new ServicoOpenTech.sgrSensorTemperatura[1];
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);
                    if (tipoDeCargaSensorOpentech != null)
                    {
                        listaSensorTemperatura[0] = new ServicoOpenTech.sgrSensorTemperatura();
                        listaSensorTemperatura[0].cdTpSensTemp = tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech;
                        listaSensorTemperatura[0].nrSensor = tipoDeCargaSensorOpentech.QuantidadeSensores;
                        listaSensorTemperatura[0].vlToleraSup = tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior;
                        listaSensorTemperatura[0].vlToleraInf = tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior;
                        listaSensorTemperatura[0].vlIdealSup = tipoDeCargaSensorOpentech.TemperaturaIdealSuperior;
                        listaSensorTemperatura[0].vlIdealInf = tipoDeCargaSensorOpentech.TemperaturaIdealInferior;
                    }

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();

                    decimal valorTotalProdutosPedido = 0.01m;
                    valorCarga = 0.01m;

                    int cdCid = codigoDestinoOpenTech;
                    int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                    int cdPaisOrigemEmitente = codigoBrasilOpenTech;
                    int cdProgramacao = 0;
                    int cdTrocaNota = 0;
                    int flRegiao = 0;
                    string dsNavio = "";
                    string dsSiglaDest = origem?.Origem?.Estado.Sigla ?? "";
                    string dsSiglaOrig = cargaIntegracao.Carga.Empresa?.Localidade.Estado.Sigla ?? "";
                    string nrCnpjCpfEmitente = cargaIntegracao.Carga.Empresa.CNPJ;
                    string nrDDDFone1 = "";
                    string nrDDDFone2 = "";
                    string nrLacreArmador = "";
                    string nrLacreSIF = "";

                    List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();
                    List<ServicoOpenTech.sgrProduto> produtosPedido = new List<ServicoOpenTech.sgrProduto>
                {
                    new ServicoOpenTech.sgrProduto()
                    {
                        cdprod = codProd,
                        valor = valorTotalProdutosPedido
                    }
                };

                    int codigoPedido = cargaPedidos.FirstOrDefault().Codigo;

                    bool enviarApenasPrimeiroPedidoNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (enviarApenasPrimeiroPedidoNaOpentech && cargaPedido.Codigo != codigoPedido)
                            break;

                        if (origem == null && cargaPedido.Pedido.Expedidor != null)
                        {
                            codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Expedidor.Localidade.Codigo)?.CodigoIntegracao ?? 0;

                            if (codigoOrigemOpenTech == 0)
                            {
                                cargaIntegracao.ProblemaIntegracao = "Cidade " + cargaPedido.Pedido.Expedidor.Localidade.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                                return;
                            }

                            cdcidibgeorigem = codigoOrigemOpenTech;
                        }

                        PreencherCamposConfiguracaoIntegracao(cargaIntegracao, null, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorTotalProdutosPedido, valorCarga);

                        documentosV2.Add(new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
                        {
                            nrDoc = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            tpDoc = 3, //Pedido
                            valorDoc = valorTotalProdutosPedido,
                            tpOperacao = 2, //Coleta
                            dtPrevista = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value : cargaIntegracao.Carga.DataCriacaoCarga,
                            dtPrevistaSaida = "",
                            dsRua = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Endereco, 50) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Endereco, 50) : string.Empty,
                            nrRua = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Numero, 6) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Numero, 6) : string.Empty,
                            complementoRua = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Complemento, 40) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Complemento, 40) : string.Empty,
                            dsBairro = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Bairro, 50) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Bairro, 50) : string.Empty,
                            cdCidIBGE = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.Localidade.CodigoIBGE : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE : 0,
                            nrCep = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.CEP : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CEP : string.Empty,
                            nrFone1 = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Expedidor.Telefone1) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Remetente.Telefone1) : string.Empty,
                            nrFone2 = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Expedidor.Telefone2) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Remetente.Telefone2) : string.Empty,
                            nrCnpjCPFDestinatario = ObterCnpjCPFDestinatario(cargaPedido),
                            nrCnpjCpfDestinatarioSequencia = "",
                            Latitude = 0f,
                            Longitude = 0f,
                            dsNome = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Nome, 50) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Nome, 50) : string.Empty,
                            qtVolumes = 1,
                            qtPecas = 1,
                            nrControleCliente1 = "",
                            nrControleCliente2 = "",
                            nrControleCliente3 = "",
                            nrControleCliente7 = "",
                            nrControleCliente8 = "",
                            nrControleCliente9 = "",
                            nrControleCliente10 = "",
                            produtos = produtosPedido.ToArray(),
                            vlCubagem = 0,
                            vlPeso = 1,
                            cdTransp = cdTrans,
                            flTrocaNota = 0,
                            cdProgramacaoDestinatario = 0,
                            cdCid = cdCid,
                            cdEmbarcador = codigoEmbarcadorOpenTech,
                            cdPaisOrigemDestinatario = cargaPedido.Pedido.Recebedor != null && cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Recebedor.Localidade?.Pais?.Descricao ?? "") : cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "") : cdPaisOrigemDestinatario,
                            cdPaisOrigemEmitente = cdPaisOrigemEmitente,
                            cdProgramacao = cdProgramacao,
                            cdTrocaNota = cdTrocaNota,
                            dsNavio = dsNavio,
                            dsSiglaDest = dsSiglaDest,
                            dsSiglaOrig = dsSiglaOrig,
                            flRegiao = flRegiao,
                            nfs = nfsOpen.ToArray(),
                            nrCnpjCpfEmitente = nrCnpjCpfEmitente,
                            nrDDDFone1 = nrDDDFone1,
                            nrDDDFone2 = nrDDDFone2,
                            nrLacreArmador = nrLacreArmador,
                            nrLacreSIF = nrLacreSIF,
                            flReentrega = 0,
                            vlInicioDiariaAtrasado = 0,
                            vlInicioDiariaNoPrazo = 0
                        });

                        //Cadastrar recebedor
                        if (cargaPedido.Pedido.Recebedor != null)
                        {
                            ServicoOpenTech.sgrData retornoIntegracaoRecebedor = null;

                            try
                            {
                                List<Servicos.ServicoOpenTech.sgrDestinatariosCliente> arrClientes = new List<ServicoOpenTech.sgrDestinatariosCliente>
                            {
                                new ServicoOpenTech.sgrDestinatariosCliente()
                                {
                                    strFlTipoPessoa = "F",
                                    strnrCGCCPF = cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? cargaPedido.Pedido.Recebedor.CodigoIntegracao : cargaPedido.Pedido.Recebedor.CPF_CNPJ_SemFormato,
                                    strRazaoSocial = cargaPedido.Pedido.Recebedor.Nome,
                                    strNomeDestinatario = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Recebedor.NomeFantasia) ? cargaPedido.Pedido.Recebedor.NomeFantasia : cargaPedido.Pedido.Recebedor.Nome,
                                    cdCidade = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0,
                                    strEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50),
                                    cdNrRua = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6),
                                    strComplementoEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40),
                                    strBairro = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50),
                                    strCEP = cargaPedido.Pedido.Recebedor.CEP,
                                    strFone = Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1),
                                    strEmail = cargaPedido.Pedido.Recebedor.Email,
                                    strObservacoes = string.Empty,
                                    vlLat = 0f,
                                    vlLong = 0f,
                                    cdPais = codigoBrasilOpenTech,
                                    cdTipoDestinatario = 4,
                                    strSigla = cargaPedido.Recebedor.CodigoIntegracao
                                }
                            };

                                retornoIntegracaoRecebedor = svcOpenTech.sgrCadastroDestinatarios(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, arrClientes.ToArray());
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                            }

                            servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração Recebedor - " + retornoIntegracaoRecebedor?.ReturnDescription ?? "");
                        }
                    }

                    List<ServicoOpenTech.sgrProduto> produtos = new List<ServicoOpenTech.sgrProduto>
                {
                    new ServicoOpenTech.sgrProduto()
                    {
                        cdprod = codProd,
                        valor = 0.01m
                    }
                };

                    //Adicionando Polilinha na etapa 3
                    ServicoOpenTech.Rota[] rotas = null;
                    Dominio.Entidades.RotaFrete rotaFrete = cargaIntegracao.Carga.Rota;

                    if ((_configuracaoIntegracaoOpenTech.IntegrarRotaCargaOpentech || _configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10) && rotaFrete != null)
                    {
                        ServicoOpenTech.Rota rota = new ServicoOpenTech.Rota()
                        {
                            PolylineFull = rotaFrete.PolilinhaRota
                        };

                        if (!_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                            rota.cdRotaModelo = rotaFrete.CodigoIntegracao.ToInt();

                        cdrotaopentech = -1;
                        rotas = new ServicoOpenTech.Rota[] { rota };
                    }

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    ServicoOpenTech.sgrData retornoIntegracao = null;

                    if (_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv10(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                             pesocarga, "", "", "", "", "", "", cargaIntegracao.Carga.SetPointVeiculo, cargaIntegracao.Carga.IntegracaoTemperatura, cargaIntegracao.Carga.CategoriaCargaEmbarcador, "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                             tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                             nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, rotas, null, null, null, null);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv9(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                            pesocarga, "", "", "", "", "", "", "", "", "", "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                            tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                            nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, rotas);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                    if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK")
                    {
                        int cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);

                        cargaIntegracao.Protocolo = cdviag.ToString();
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cdviag + ".";
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar";

                        mensagemErro += ObterMensagemErro(retornoIntegracao);

                        cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                    }

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = "3 - Não foi possível comunicar com os serviços da Opentech.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                return;
            }
        }

        public void IntegrarCargaColeta(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(_unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaIntegracao.ProblemaIntegracao = "A configuração de integração para a OpenTech é inválida.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                IList<int> codigosVeiculosVinculados = repVeiculo.BuscarVeiculosVinculadoACarga(cargaIntegracao.Carga.Codigo);
                List<Dominio.Entidades.Veiculo> veiculosVinculados = repVeiculo.BuscarPorCodigos(codigosVeiculosVinculados, false);

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector, retornoLogin, cargaIntegracao);

                    if (cdTrans == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Transportador não possui cadastro na OpenTech.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (_configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista && !IntegrarCadastros(cargaIntegracao, retornoLogin))
                        return;

                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso origem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);

                    if (cargaIntegracao.Carga.Veiculo == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Carga sem veículo";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    Dominio.Entidades.Localidade localidadeOrigem = origem?.Origem ?? cargaIntegracao.Carga.Filial?.Localidade ?? cargaPedidos.FirstOrDefault().Origem;
                    Dominio.Entidades.Localidade localidadeDestino = destino?.Destino ?? cargaPedidos.LastOrDefault().Destino;

                    int codProd = ObterCodigoProdutoOpenTech(cargaIntegracao.Carga);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> listaApoliceSeguraAverbacao = repApoliceSeguroAverbacao.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    if (listaApoliceSeguraAverbacao != null && listaApoliceSeguraAverbacao.Count > 0 && codProd == 0)
                    {
                        if (listaApoliceSeguraAverbacao.FirstOrDefault().ApoliceSeguro.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador)
                            codProd = _configuracaoIntegracaoOpenTech.CodigoProdutoColetaEmbarcadorOpentech;
                        else
                            codProd = _configuracaoIntegracaoOpenTech.CodigoProdutoColetaTransportadorOpentech;
                    }

                    if (codProd == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um produto Coleta opentech configurado";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = repProdutoOpentech.BuscarPorCodigoProdutoOpentTechETipoOperacao(codProd, cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                    if (produtoOpentech == null)
                        produtoOpentech = repProdutoOpentech.BuscarPorCodigoProdutoOpentTech(codProd);

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, destino);
                    int codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(localidadeOrigem?.Codigo ?? 0)?.CodigoIntegracao ?? 0;
                    int codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(localidadeDestino?.Codigo ?? 0)?.CodigoIntegracao ?? 0;

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        if (codigoOrigemOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + (localidadeOrigem?.DescricaoCidadeEstado ?? "(Não possui localidade de origem)") + " não possui Código de Integração OpenTech";
                        else if (codigoDestinoOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + (localidadeDestino?.DescricaoCidadeEstado ?? "(Não possui localidade de destino)") + " não possui Código de Integração OpenTech";

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                    string nrplacacavalo = cargaIntegracao.Carga.Veiculo.Placa;
                    string nrplacacarreta1 = veiculosVinculados.Count > 0 ? veiculosVinculados.First().Placa : string.Empty;
                    string nrplacacarreta2 = veiculosVinculados.Count > 1 ? veiculosVinculados.Last().Placa : string.Empty;
                    string nrdocmotorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().CPF : string.Empty;
                    string nrdocmotorista2 = string.Empty;
                    string motorista1 = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Nome : string.Empty;
                    string motorista2 = string.Empty;
                    string dtprevini = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string dtprevfim = (_configuracaoIntegracaoOpenTech?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false) ? (pedidos.FirstOrDefault().PrevisaoEntrega.HasValue ? pedidos.FirstOrDefault().PrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")) : (cargaIntegracao.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaIntegracao.Carga.DataPrevisaoTerminoCarga.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    int cdrotaopentech = 0;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrdddfonecelular = string.Empty;
                    string nrcontrolecarga = cargaIntegracao.Carga.CodigoCargaEmbarcador;
                    int cdtipooperacao = 2; //Coleta
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal distanciatotal = 0m;
                    decimal pesocarga = cargaPedidos.Sum(o => o.Peso);
                    string cdvincmot1 = "";
                    string cdvincmot2 = "";
                    string ratreadorCavalor = "";
                    int cdEmprastrCavalo = 0;
                    string rastradorCarreta1 = "";
                    int cdEmprastrCarreta = 0;
                    decimal valorCarga = 0;

                    if (_configuracaoIntegracaoOpenTech?.EnviarDataAtualNaDataPrevisaoOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddMinutes(1).ToString("yyyy-MM-dd HH:mm");
                    }

                    if (_configuracaoIntegracaoOpenTech?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false)
                    {
                        dtprevini = cargaIntegracao.DataIntegracao.AddHours(1).ToString("yyyy-MM-dd HH:mm");
                        dtprevfim = cargaIntegracao.DataIntegracao.AddHours(24).ToString("yyyy-MM-dd HH:mm");
                    }

                    var listaSensorTemperatura = new ServicoOpenTech.sgrSensorTemperatura[1];
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);
                    if (tipoDeCargaSensorOpentech != null)
                    {
                        listaSensorTemperatura[0] = new ServicoOpenTech.sgrSensorTemperatura();
                        listaSensorTemperatura[0].cdTpSensTemp = tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech;
                        listaSensorTemperatura[0].nrSensor = tipoDeCargaSensorOpentech.QuantidadeSensores;
                        listaSensorTemperatura[0].vlToleraSup = tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior;
                        listaSensorTemperatura[0].vlToleraInf = tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior;
                        listaSensorTemperatura[0].vlIdealSup = tipoDeCargaSensorOpentech.TemperaturaIdealSuperior;
                        listaSensorTemperatura[0].vlIdealInf = tipoDeCargaSensorOpentech.TemperaturaIdealInferior;
                    }

                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();

                    decimal valorTotalProdutosPedido = 0.01m;
                    valorCarga = 0.01m;

                    int cdCid = codigoDestinoOpenTech;
                    int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                    int cdPaisOrigemEmitente = codigoBrasilOpenTech;
                    int cdProgramacao = 0;
                    int cdTrocaNota = 0;
                    int flRegiao = 0;

                    string dsNavio = "";
                    string dsSiglaDest = origem?.Origem?.Estado.Sigla ?? "";
                    string dsSiglaOrig = cargaIntegracao.Carga.Empresa?.Localidade.Estado.Sigla ?? "";
                    string nrCnpjCpfEmitente = cargaIntegracao.Carga.Empresa.CNPJ;
                    string nrDDDFone1 = "";
                    string nrDDDFone2 = "";
                    string nrLacreArmador = "";
                    string nrLacreSIF = "";

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

                    List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();
                    List<ServicoOpenTech.sgrProduto> produtosPedido = new List<ServicoOpenTech.sgrProduto>
                    {
                        new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = codProd,
                            valor = valorTotalProdutosPedido
                        }
                    };

                    int codigoPedido = cargaPedidos.FirstOrDefault().Codigo;

                    bool enviarApenasPrimeiroPedidoNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (enviarApenasPrimeiroPedidoNaOpentech && cargaPedido.Codigo != codigoPedido)
                            break;

                        if (origem == null && cargaPedido.Pedido.Expedidor != null)
                        {
                            codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Expedidor.Localidade.Codigo)?.CodigoIntegracao ?? 0;

                            if (codigoOrigemOpenTech == 0)
                            {
                                cargaIntegracao.ProblemaIntegracao = "Cidade " + cargaPedido.Pedido.Expedidor.Localidade.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                                return;
                            }

                            cdcidibgeorigem = codigoOrigemOpenTech;
                        }

                        PreencherCamposConfiguracaoIntegracao(null, cargaIntegracao, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorTotalProdutosPedido, valorCarga);

                        documentosV2.Add(new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
                        {
                            nrDoc = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            tpDoc = 3, //Pedido
                            valorDoc = valorTotalProdutosPedido,
                            tpOperacao = 2, //Coleta
                            dtPrevista = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value : cargaIntegracao.Carga.DataCriacaoCarga,
                            dtPrevistaSaida = "",
                            dsRua = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Endereco, 50) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Endereco, 50) : string.Empty,
                            nrRua = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Numero, 6) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Numero, 6) : string.Empty,
                            complementoRua = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Complemento, 40) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Complemento, 40) : string.Empty,
                            dsBairro = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Bairro, 50) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Bairro, 50) : string.Empty,
                            cdCidIBGE = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.Localidade.CodigoIBGE : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE : 0,
                            nrCep = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.CEP : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CEP : string.Empty,
                            nrFone1 = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Expedidor.Telefone1) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Remetente.Telefone1) : string.Empty,
                            nrFone2 = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Expedidor.Telefone2) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Remetente.Telefone2) : string.Empty,
                            //nrCnpjCPFDestinatario = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.CPF_CNPJ_SemFormato : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato : string.Empty,
                            nrCnpjCPFDestinatario = ObterCnpjCPFDestinatario(cargaPedido),
                            nrCnpjCpfDestinatarioSequencia = "",
                            Latitude = 0f,
                            Longitude = 0f,
                            dsNome = cargaPedido.Pedido.Expedidor != null ? Utilidades.String.Left(cargaPedido.Pedido.Expedidor.Nome, 50) : cargaPedido.Pedido.Remetente != null ? Utilidades.String.Left(cargaPedido.Pedido.Remetente.Nome, 50) : string.Empty,
                            qtVolumes = 1,
                            qtPecas = 1,
                            nrControleCliente1 = "",
                            nrControleCliente2 = "",
                            nrControleCliente3 = "",
                            nrControleCliente7 = "",
                            nrControleCliente8 = "",
                            nrControleCliente9 = "",
                            nrControleCliente10 = "",
                            produtos = produtosPedido.ToArray(),
                            vlCubagem = 0,
                            vlPeso = 1,
                            cdTransp = cdTrans,
                            flTrocaNota = 0,
                            cdProgramacaoDestinatario = 0,
                            cdCid = cdCid,
                            cdEmbarcador = codigoEmbarcadorOpenTech,
                            cdPaisOrigemDestinatario = cargaPedido.Pedido.Recebedor != null && cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Recebedor.Localidade?.Pais?.Descricao ?? "") : cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla == "EX" ? ObterCodigoPais(retornoLogin, cargaPedido.Pedido.Destinatario.Localidade?.Pais?.Descricao ?? "") : cdPaisOrigemDestinatario,
                            cdPaisOrigemEmitente = cdPaisOrigemEmitente,
                            cdProgramacao = cdProgramacao,
                            cdTrocaNota = cdTrocaNota,
                            dsNavio = dsNavio,
                            dsSiglaDest = dsSiglaDest,
                            dsSiglaOrig = dsSiglaOrig,
                            flRegiao = flRegiao,
                            nfs = nfsOpen.ToArray(),
                            nrCnpjCpfEmitente = nrCnpjCpfEmitente,
                            nrDDDFone1 = nrDDDFone1,
                            nrDDDFone2 = nrDDDFone2,
                            nrLacreArmador = nrLacreArmador,
                            nrLacreSIF = nrLacreSIF,
                            flReentrega = 0,
                            vlInicioDiariaAtrasado = 0,
                            vlInicioDiariaNoPrazo = 0
                        });

                        //Cadastrar recebedor
                        if (cargaPedido.Pedido.Recebedor != null)
                        {
                            ServicoOpenTech.sgrData retornoIntegracaoRecebedor = null;

                            try
                            {
                                List<Servicos.ServicoOpenTech.sgrDestinatariosCliente> arrClientes = new List<ServicoOpenTech.sgrDestinatariosCliente>
                            {
                                new ServicoOpenTech.sgrDestinatariosCliente()
                                {
                                    strFlTipoPessoa = "F",
                                    strnrCGCCPF = cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX" ? cargaPedido.Pedido.Recebedor.CodigoIntegracao : cargaPedido.Pedido.Recebedor.CPF_CNPJ_SemFormato,
                                    strRazaoSocial = cargaPedido.Pedido.Recebedor.Nome,
                                    strNomeDestinatario = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Recebedor.NomeFantasia) ? cargaPedido.Pedido.Recebedor.NomeFantasia : cargaPedido.Pedido.Recebedor.Nome,
                                    cdCidade = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0,
                                    strEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50),
                                    cdNrRua = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6),
                                    strComplementoEndereco = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40),
                                    strBairro = Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50),
                                    strCEP = cargaPedido.Pedido.Recebedor.CEP,
                                    strFone = Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1),
                                    strEmail = cargaPedido.Pedido.Recebedor.Email,
                                    strObservacoes = string.Empty,
                                    vlLat = 0f,
                                    vlLong = 0f,
                                    cdPais = codigoBrasilOpenTech,
                                    cdTipoDestinatario = 4,
                                    strSigla = cargaPedido.Recebedor.CodigoIntegracao
                                }
                            };

                                retornoIntegracaoRecebedor = svcOpenTech.sgrCadastroDestinatarios(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, arrClientes.ToArray());
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                            }

                            servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração Recebedor - " + retornoIntegracaoRecebedor?.ReturnDescription ?? "");
                        }
                    }

                    List<ServicoOpenTech.sgrProduto> produtos = new List<ServicoOpenTech.sgrProduto>
                    {
                        new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = codProd,
                            valor = 0.01m
                        }
                    };

                    ServicoOpenTech.Rota[] rotas = null;
                    Dominio.Entidades.RotaFrete rotaFrete = cargaIntegracao.Carga.Rota;

                    if ((_configuracaoIntegracaoOpenTech.IntegrarRotaCargaOpentech || _configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10) && rotaFrete != null)
                    {
                        ServicoOpenTech.Rota rota = new ServicoOpenTech.Rota()
                        {
                            PolylineFull = rotaFrete.PolilinhaRota
                        };

                        if (!_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                            rota.cdRotaModelo = rotaFrete.CodigoIntegracao.ToInt();

                        cdrotaopentech = -1;
                        rotas = new ServicoOpenTech.Rota[] { rota };
                    }

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    ServicoOpenTech.sgrData retornoIntegracao = null;
                    if (_configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10)
                    {

                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv10(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                             pesocarga, "", "", "", "", "", "", cargaIntegracao.Carga.SetPointVeiculo, cargaIntegracao.Carga.IntegracaoTemperatura, cargaIntegracao.Carga.CategoriaCargaEmbarcador, "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                             tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                             nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, rotas, null, null, null, null);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            retornoIntegracao = svcOpenTech.sgrGerarAEv9(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoBrasilOpenTech, nrplacacavalo,
                                                                                            codigoBrasilOpenTech, nrplacacarreta1, codigoBrasilOpenTech, nrplacacarreta2, codigoBrasilOpenTech, nrdocmotorista1, codigoBrasilOpenTech, nrdocmotorista2, motorista1, motorista2,
                                                                                            cdvincmot1, cdvincmot2, dtprevini, dtprevfim, ratreadorCavalor, cdEmprastrCavalo, rastradorCarreta1, cdEmprastrCarreta,
                                                                                            cdcidibgeorigem, cdcidibgedestino, cdrotaopentech, valorCarga, cdTrans, nrfonecelular, cdtipooperacao, codigoEmbarcadorOpenTech, nrcontrolecarga, nrfrota, distanciatotal,
                                                                                            pesocarga, "", "", "", "", "", "", "", "", "", "", produtos.ToArray(), documentosV2.ToArray(), new ServicoOpenTech.sgrPontoApoioViagem[] { },
                                                                                            tipoDeCargaSensorOpentech != null ? listaSensorTemperatura : new ServicoOpenTech.sgrSensorTemperatura[] { },
                                                                                            nrdddfonecelular, "", "", "", new ServicoOpenTech.sgrIsca[] { }, rotas);

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                    if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK")
                    {
                        int cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);

                        cargaIntegracao.Protocolo = cdviag.ToString();
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cdviag + ".";
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar";

                        mensagemErro += ObterMensagemErro(retornoIntegracao);

                        cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                    }

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = " 4 - Não foi possível comunicar com os serviços da Opentech.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                return;
            }
        }

        public void IntegrarAtualizacaoCargaColeta(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string protocoloIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(_unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaIntegracao.ProblemaIntegracao = "A configuração de integração para a OpenTech é inválida.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector, retornoLogin);

                    if (cdTrans == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Transportador não possui cadastro na OpenTech.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (_configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista && !IntegrarCadastros(cargaIntegracao, retornoLogin))
                        return;

                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso origem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);

                    if (destino == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um Destino para a carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (cargaIntegracao.Carga.Veiculo == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Carga sem veículo";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> produtosOpenTech = new List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>();
                    Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = null;

                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroCarga = repositorioApoliceSeguroAverbacao.BuscarApolicesPorCarga(cargaIntegracao.Carga.Codigo);

                    decimal valorPedido = (from obj in cargaPedidos select obj.Produtos).SelectMany(o => o).ToList().Sum(o => o.ValorUnitarioProduto * o.Quantidade);

                    if (apolicesSeguroCarga.Count > 0)
                    {
                        if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, destino.Destino.Estado.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0, destino.Destino.Codigo);
                        else
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, destino.Destino.Estado.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                        produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorPedido && x.ValorAte >= valorPedido).FirstOrDefault();

                        if (produtoOpentech == null)
                            produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
                    }

                    if (produtoOpentech == null)
                    {
                        if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeSemApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, destino.Destino.Estado.Sigla, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0, destino.Destino.Codigo);
                        else
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoSemApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, destino.Destino.Estado.Sigla, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);

                        produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorPedido && x.ValorAte >= valorPedido).FirstOrDefault();

                        if (produtoOpentech == null)
                            produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
                    }

                    if (produtoOpentech == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um produto opentech configurado para esta carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, destino);
                    int codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(origem != null ? origem.Origem.Codigo : (cargaIntegracao.Carga.Filial?.Localidade.Codigo ?? cargaPedidos.FirstOrDefault().Origem.Codigo))?.CodigoIntegracao ?? 0;
                    int codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(destino.Destino.Codigo)?.CodigoIntegracao ?? 0;

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        if (codigoOrigemOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + origem != null ? origem.Origem.DescricaoCidadeEstado : (cargaIntegracao.Carga.Filial?.Localidade.Descricao ?? "") + " não possui Código de Integração OpenTech";
                        else if (codigoDestinoOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + destino.Destino.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal valorCarga = 0;

                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutos = (from obj in cargaPedidos select obj.Produtos).SelectMany(o => o).ToList();

                    int codigoPedido = cargaPedidos.FirstOrDefault().Codigo;

                    bool veiculoPermiteLocalizador = (cargaIntegracao.Carga.Veiculo?.ModeloVeicularCarga?.ModeloVeicularAceitaLocalizador ?? false) && (cargaIntegracao.Carga.Veiculo?.PossuiLocalizador ?? false);
                    int codProd = veiculoPermiteLocalizador ? _configuracaoIntegracaoOpenTech.CodigoProdutoVeiculoComLocalizadorOpenTech : 0;
                    if (codProd == 0)
                        codProd = produtoOpentech.CodigoProdutoOpentech;

                    bool enviarApenasPrimeiroPedidoNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false;
                    bool enviarInformacoesTotaisDaCargaNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarInformacoesTotaisDaCargaNaOpentech ?? false;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (enviarApenasPrimeiroPedidoNaOpentech && cargaPedido.Codigo != codigoPedido)
                            break;

                        if (cargaIntegracao.Carga.CargaEmitidaParcialmente && !cargaIntegracao.Carga.ExigeNotaFiscalParaCalcularFrete && !cargaPedido.NotasFiscais.Any(o => o.CTes.Count == 0)) // Só envia para a Open as NFe que não tem CTe emitido devido as cargas agrupadas que retornam para emitir a segunda coleta
                            continue;

                        List<(decimal ValorNota, int NumeroNota)> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoSemFactura(cargaPedido.Codigo);
                        decimal valorTotalNotasPedido = 0m;
                        int pesoTotal = 0;

                        if (enviarApenasPrimeiroPedidoNaOpentech && enviarInformacoesTotaisDaCargaNaOpentech)
                        {
                            List<int> codigosCargaPedido = cargaPedidos.Select(x => x.Codigo).ToList();

                            pesoTotal = repPedidoXMLNotaFiscal.BuscarPesoPorCargaPedidos(codigosCargaPedido);
                            valorTotalNotasPedido = repPedidoXMLNotaFiscal.BuscarValorTotalPorCargaPedidos(codigosCargaPedido);
                        }
                        else
                        {
                            pesoTotal = (int)cargaPedido.Pedido.PesoTotal;
                            valorTotalNotasPedido = pedidoXMLNotasFiscais.Sum(x => x.ValorNota);
                        }

                        int cdCid = cargaPedido.Pedido.Recebedor != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0 : cargaPedido.Pedido.Destinatario != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Destinatario.Localidade.Codigo)?.CodigoIntegracao ?? 0 : 0;
                        int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                        int cdPaisOrigemEmitente = codigoBrasilOpenTech;
                        int cdProgramacao = 0;
                        int cdTrocaNota = 0;
                        int flRegiao = 0;

                        string dsNavio = "";
                        string numeroPrimeiraNota = pedidoXMLNotasFiscais.Select(x => x.NumeroNota).FirstOrDefault().ToString();
                        string dsSiglaDest = cargaPedido.Recebedor != null ? cargaPedido.Recebedor.Localidade.Estado.Sigla : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla : "";
                        string dsSiglaOrig = cargaPedido.Expedidor != null ? cargaPedido.Expedidor.Localidade.Estado.Sigla : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla : "";
                        string nrCnpjCpfEmitente = cargaIntegracao.Carga.Empresa.CNPJ;
                        string nrDDDFone1 = "";
                        string nrDDDFone2 = "";
                        string nrLacreArmador = "";
                        string nrLacreSIF = "";

                        List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();
                        List<ServicoOpenTech.sgrProduto> produtosPedido = new List<ServicoOpenTech.sgrProduto>
                    {
                        new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = codProd,
                            valor = valorTotalNotasPedido
                        }
                    };

                        DateTime dtPrevista = cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.HasValue ? cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.Value : cargaPedido.Pedido.DataCriacao.Value.AddDays(1);

                        if ((_configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false) && cargaPedido.Pedido?.NotasFiscais?.FirstOrDefault()?.DataEmissao != null)
                            dtPrevista = cargaPedido.Pedido.NotasFiscais.FirstOrDefault().DataEmissao;

                        string dtprevini = "";
                        string ratreadorCavalor = "";
                        int cdEmprastrCavalo = 0;

                        PreencherCamposConfiguracaoIntegracao(cargaIntegracao, null, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorTotalNotasPedido, valorCarga);

                        documentosV2.Add(new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
                        {
                            nrDoc = numeroPrimeiraNota, //cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            tpDoc = 3, //Pedido
                            valorDoc = valorTotalNotasPedido,
                            tpOperacao = 3, //Entrega
                            dtPrevista = dtPrevista,
                            dtPrevistaSaida = "",
                            dsRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Endereco, 50) : string.Empty,
                            nrRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Numero, 6) : string.Empty,
                            complementoRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Complemento, 40) : string.Empty,
                            dsBairro = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Bairro, 50) : string.Empty,
                            cdCidIBGE = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.Localidade.CodigoIBGE : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE : 0,
                            nrCep = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Pedido.Recebedor.CEP : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CEP : string.Empty,
                            nrFone1 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone1) : string.Empty,
                            nrFone2 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone2) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone2) : string.Empty,
                            nrCnpjCPFDestinatario = ObterCnpjCPFDestinatario(cargaPedido),
                            nrCnpjCpfDestinatarioSequencia = "",
                            Latitude = 0f,
                            Longitude = 0f,
                            dsNome = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Nome, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Nome, 50) : string.Empty,
                            qtVolumes = cargaPedido.Pedido.QtVolumes,
                            qtPecas = cargaPedido.Pedido.QtVolumes,
                            nrControleCliente1 = "",
                            nrControleCliente2 = "",
                            nrControleCliente3 = "",
                            nrControleCliente7 = "",
                            nrControleCliente8 = "",
                            nrControleCliente9 = "",
                            nrControleCliente10 = "",
                            produtos = produtosPedido.ToArray(),
                            vlCubagem = 0,
                            vlPeso = pesoTotal,
                            cdTransp = cdTrans,
                            flTrocaNota = 0,
                            cdProgramacaoDestinatario = 0,
                            cdCid = cdCid,
                            cdEmbarcador = codigoEmbarcadorOpenTech,
                            cdPaisOrigemDestinatario = cdPaisOrigemDestinatario,
                            cdPaisOrigemEmitente = cdPaisOrigemEmitente,
                            cdProgramacao = cdProgramacao,
                            cdTrocaNota = cdTrocaNota,
                            dsNavio = dsNavio,
                            dsSiglaDest = dsSiglaDest,
                            dsSiglaOrig = dsSiglaOrig,
                            flRegiao = flRegiao,
                            nfs = nfsOpen.ToArray(),
                            nrCnpjCpfEmitente = nrCnpjCpfEmitente,
                            nrDDDFone1 = nrDDDFone1,
                            nrDDDFone2 = nrDDDFone2,
                            nrLacreArmador = nrLacreArmador,
                            nrLacreSIF = nrLacreSIF,
                            flReentrega = 0,
                            vlInicioDiariaAtrasado = 0,
                            vlInicioDiariaNoPrazo = 0
                        });
                    }

                    ServicoOpenTech.sgrData retornoIntegracao = null;

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    try
                    {
                        int.TryParse(protocoloIntegracao, out int cdViagem);

                        retornoIntegracao = svcOpenTech.sgrNovaEntregaProdutoV3(retornoLogin.ReturnKey,
                                                                                codigoPASOpenTech,
                                                                                codigoClienteOpenTech,
                                                                                cdViagem,
                                                                                documentosV2.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", $"Integração NovaEntregaProdutoV3 - {retornoIntegracao?.ReturnDescription ?? ""} ");

                    if (!string.IsNullOrEmpty(cargaIntegracao.PreProtocolo) || (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK"))
                    {
                        int cdviag = 0;
                        if (string.IsNullOrEmpty(cargaIntegracao.PreProtocolo))
                            cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);
                        else
                        {
                            int PreProtocolo;
                            if (int.TryParse(cargaIntegracao.PreProtocolo, out PreProtocolo))
                                cdviag = PreProtocolo;
                        }

                        cargaIntegracao.PreProtocolo = cdviag.ToString();
                        ServicoOpenTech.sgrData retornoIntegracaoAtualizarRota = null;

                        try
                        {
                            retornoIntegracaoAtualizarRota = svcOpenTech.sgrAtualizaRotaViagem(retornoLogin.ReturnKey,
                                                                                               codigoPASOpenTech,
                                                                                               cdviag,
                                                                                               codigoClienteOpenTech,
                                                                                               new ServicoOpenTech.LatLng[] { },
                                                                                               null,
                                                                                               null,
                                                                                               1);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }

                        servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração AtualizarRotaViagem - " + retornoIntegracaoAtualizarRota?.ReturnDescription ?? "OK");

                        if (retornoIntegracaoAtualizarRota != null && retornoIntegracaoAtualizarRota.ReturnID == 0)
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Integração inclusão documentos e atualização de rota realizada com sucesso. CDVIAG: " + cdviag + ".";
                            cargaIntegracao.Protocolo = cdviag.ToString();
                        }
                        else
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            string mensagemErro = retornoIntegracaoAtualizarRota?.ReturnDescription ?? "Falha ao integrar atualizar Rota Viagem";

                            if (retornoIntegracaoAtualizarRota.ReturnDataset != null)
                                for (int i = 0; i < retornoIntegracaoAtualizarRota.ReturnDataset.Nodes.Count; i++)
                                    mensagemErro += !string.IsNullOrWhiteSpace(retornoIntegracaoAtualizarRota.ReturnDataset.Nodes[i].Value) ? " / " + retornoIntegracaoAtualizarRota.ReturnDataset.Nodes[i].Value : "";

                            cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                        }
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar Inclusão documentos ";

                        if (retornoIntegracao.ReturnDataset != null)
                            for (int i = 0; i < retornoIntegracao.ReturnDataset.Nodes.Count; i++)
                                mensagemErro += !string.IsNullOrWhiteSpace(retornoIntegracao.ReturnDataset.Nodes[i].Value) ? " / " + retornoIntegracao.ReturnDataset.Nodes[i].Value : "";

                        cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                    }

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = "5 - Não foi possível comunicar com os serviços da Opentech.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                return;
            }
        }

        public void IntegrarAtualizacaoCargaColeta(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, string protocoloIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Integracao.ProdutoOpentech repProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(_unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedidos = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaIntegracao.ProblemaIntegracao = "A configuração de integração para a OpenTech é inválida.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo))
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integração inclusão documentos e atualização de rota realizada com sucesso. CDVIAG:" + cargaIntegracao.Protocolo + ".";

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                    return;
                }

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedidos.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                    int cdTrans = ObterCodigoTransportadorOpenTech(cargaIntegracao.Carga, cargaPedidos, svcOpenTech, inspector, retornoLogin);

                    if (cdTrans == 0)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Transportador não possui cadastro na OpenTech.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    if (_configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista && !IntegrarCadastros(cargaIntegracao, retornoLogin))
                        return;

                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso origem = repCargaPercurso.BuscarOrigem(cargaIntegracao.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino = repCargaPercurso.BuscarUltimaEntrega(cargaIntegracao.Carga.Codigo);

                    if (cargaIntegracao.Carga.Veiculo == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Carga sem veículo";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    Dominio.Entidades.Localidade localidadeOrigem = origem != null ? origem.Origem : (cargaIntegracao.Carga.Filial?.Localidade ?? cargaPedidos.FirstOrDefault().Origem);
                    Dominio.Entidades.Localidade localidadeDestino = destino != null ? destino.Destino : cargaPedidos.LastOrDefault().Destino;

                    List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> produtosOpenTech = new List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>();
                    Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = null;

                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroCarga = repositorioApoliceSeguroAverbacao.BuscarApolicesPorCarga(cargaIntegracao.Carga.Codigo);

                    decimal valorPedido = (from obj in cargaPedidos select obj.Produtos).SelectMany(o => o).ToList().Sum(o => o.ValorUnitarioProduto * o.Quantidade);

                    if (apolicesSeguroCarga.Count > 0)
                    {
                        if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0, localidadeDestino.Codigo);
                        else
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);
                        produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorPedido && x.ValorAte >= valorPedido).FirstOrDefault();

                        if (produtoOpentech == null)
                            produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
                    }

                    if (produtoOpentech == null)
                    {
                        if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeSemApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0, localidadeDestino.Codigo);
                        else
                            produtosOpenTech = repProdutoOpentech.BuscarPorOperacaoEstadoSemApolice(cargaIntegracao.Carga.TipoOperacao?.Codigo ?? 0, localidadeDestino.Estado.Sigla, cargaIntegracao.Carga.TipoDeCarga?.Codigo ?? 0);
                        produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorPedido && x.ValorAte >= valorPedido).FirstOrDefault();

                        if (produtoOpentech == null)
                            produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
                    }

                    if (produtoOpentech == null)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Não existe um produto opentech configurado para esta carga";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int codigoBrasilOpenTech = 1;
                    int codigoEmbarcadorOpenTech = ObterCodigoEmbarcadorOpenTech(cargaIntegracao.Carga, destino);
                    int codigoOrigemOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(localidadeOrigem != null ? localidadeOrigem.Codigo : (cargaIntegracao.Carga.Filial?.Localidade.Codigo ?? cargaPedidos.FirstOrDefault().Origem.Codigo))?.CodigoIntegracao ?? 0;
                    int codigoDestinoOpenTech = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(localidadeDestino.Codigo)?.CodigoIntegracao ?? 0;

                    if (codigoOrigemOpenTech == 0 || codigoDestinoOpenTech == 0)
                    {
                        if (codigoOrigemOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + localidadeOrigem != null ? localidadeOrigem.DescricaoCidadeEstado : (cargaIntegracao.Carga.Filial?.Localidade.Descricao ?? "") + " não possui Código de Integração OpenTech";
                        else if (codigoDestinoOpenTech == 0)
                            cargaIntegracao.ProblemaIntegracao = "Cidade " + localidadeDestino.DescricaoCidadeEstado + " não possui Código de Integração OpenTech";

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                        return;
                    }

                    int cdcidibgeorigem = codigoOrigemOpenTech;
                    int cdcidibgedestino = codigoDestinoOpenTech;
                    string cdrotacliente = cdcidibgeorigem.ToString() + "-" + cdcidibgedestino.ToString();
                    string nrfonecelular = cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.First().Telefone ?? string.Empty : string.Empty;
                    string nrfrota = cargaIntegracao.Carga.Veiculo.NumeroFrota ?? string.Empty;
                    decimal valorCarga = 0;

                    List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();

                    bool veiculoPermiteLocalizador = (cargaIntegracao.Carga.Veiculo?.ModeloVeicularCarga?.ModeloVeicularAceitaLocalizador ?? false) && (cargaIntegracao.Carga.Veiculo?.PossuiLocalizador ?? false);
                    bool enviarApenasPrimeiroPedidoNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false;
                    bool enviarInformacoesTotaisDaCargaNaOpentech = cargaIntegracao.Carga.TipoOperacao?.ConfiguracaoIntegracao?.EnviarInformacoesTotaisDaCargaNaOpentech ?? false;

                    int codProd = veiculoPermiteLocalizador ? _configuracaoIntegracaoOpenTech.CodigoProdutoVeiculoComLocalizadorOpenTech : 0;
                    if (codProd == 0)
                        codProd = produtoOpentech.CodigoProdutoOpentech;

                    int codigoPedido = cargaPedidos.FirstOrDefault().Codigo;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (enviarApenasPrimeiroPedidoNaOpentech && cargaPedido.Codigo != codigoPedido)
                            break;

                        if (cargaIntegracao.Carga.CargaEmitidaParcialmente && !cargaIntegracao.Carga.ExigeNotaFiscalParaCalcularFrete && !cargaPedido.NotasFiscais.Any(o => o.CTes.Count == 0)) // Só envia para a Open as NFe que não tem CTe emitido devido as cargas agrupadas que retornam para emitir a segunda coleta
                            continue;

                        List<(decimal ValorNota, int NumeroNota)> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoSemFactura(cargaPedido.Codigo);
                        decimal valorTotalNotasPedido = 0m;
                        int pesoTotal = 0;

                        if (enviarApenasPrimeiroPedidoNaOpentech && enviarInformacoesTotaisDaCargaNaOpentech)
                        {
                            List<int> codigosCargaPedido = cargaPedidos.Select(x => x.Codigo).ToList();

                            pesoTotal = repPedidoXMLNotaFiscal.BuscarPesoPorCargaPedidos(codigosCargaPedido);
                            valorTotalNotasPedido = repPedidoXMLNotaFiscal.BuscarValorTotalPorCargaPedidos(codigosCargaPedido);
                        }
                        else
                        {
                            pesoTotal = (int)cargaPedido.Pedido.PesoTotal;
                            valorTotalNotasPedido = pedidoXMLNotasFiscais.Sum(x => x.ValorNota);
                        }

                        string numeroPrimeiraNota = pedidoXMLNotasFiscais.Select(x => x.NumeroNota).FirstOrDefault().ToString();

                        int cdCid = cargaPedido.Pedido.Recebedor != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Recebedor.Localidade.Codigo)?.CodigoIntegracao ?? 0 : cargaPedido.Pedido.Destinatario != null ? repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaPedido.Pedido.Destinatario.Localidade.Codigo)?.CodigoIntegracao ?? 0 : 0;
                        int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                        int cdPaisOrigemEmitente = codigoBrasilOpenTech;
                        int cdProgramacao = 0;
                        int cdTrocaNota = 0;
                        int flRegiao = 0;
                        string dsNavio = "";
                        string dsSiglaDest = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Recebedor.Localidade.Estado.Sigla : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla : "";
                        string dsSiglaOrig = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Expedidor.Localidade.Estado.Sigla : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla : "";
                        string nrCnpjCpfEmitente = cargaIntegracao.Carga.Empresa.CNPJ;
                        string nrDDDFone1 = "";
                        string nrDDDFone2 = "";
                        string nrLacreArmador = "";
                        string nrLacreSIF = "";

                        List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();
                        List<ServicoOpenTech.sgrProduto> produtosPedido = new List<ServicoOpenTech.sgrProduto>
                        {
                            new ServicoOpenTech.sgrProduto()
                            {
                                cdprod = codProd,
                                valor = valorTotalNotasPedido
                            }
                        };

                        DateTime dtPrevista = cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.HasValue ? cargaPedido.Pedido.DataPrevisaoChegadaDestinatario.Value : cargaPedido.Pedido.DataCriacao.Value.AddDays(1);

                        if ((_configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false) && cargaPedido.Pedido?.NotasFiscais?.FirstOrDefault()?.DataEmissao != null)
                            dtPrevista = cargaPedido.Pedido.NotasFiscais.FirstOrDefault().DataEmissao;

                        string dtprevini = "";
                        string ratreadorCavalor = "";
                        int cdEmprastrCavalo = 0;

                        PreencherCamposConfiguracaoIntegracao(null, cargaIntegracao, ref dtprevini, ref ratreadorCavalor, ref cdEmprastrCavalo, ref cdrotacliente, ref nrfonecelular, ref nrfrota, ref valorTotalNotasPedido, valorCarga);

                        documentosV2.Add(new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
                        {
                            nrDoc = numeroPrimeiraNota, //cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            tpDoc = 3, //Pedido
                            valorDoc = valorTotalNotasPedido,
                            tpOperacao = 3, //Entrega
                            dtPrevista = dtPrevista,
                            dtPrevistaSaida = "",
                            dsRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Endereco, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Endereco, 50) : string.Empty,
                            nrRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Numero, 6) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Numero, 6) : string.Empty,
                            complementoRua = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Complemento, 40) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Complemento, 40) : string.Empty,
                            dsBairro = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Bairro, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Bairro, 50) : string.Empty,
                            cdCidIBGE = cargaPedido.Pedido.Expedidor != null ? cargaPedido.Pedido.Expedidor.Localidade.CodigoIBGE : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE : 0,
                            nrCep = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Pedido.Recebedor.CEP : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CEP : string.Empty,
                            nrFone1 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone1) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone1) : string.Empty,
                            nrFone2 = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Recebedor.Telefone2) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.Telefone2) : string.Empty,
                            nrCnpjCPFDestinatario = ObterCnpjCPFDestinatario(cargaPedido),
                            nrCnpjCpfDestinatarioSequencia = "",
                            Latitude = 0f,
                            Longitude = 0f,
                            dsNome = cargaPedido.Pedido.Recebedor != null ? Utilidades.String.Left(cargaPedido.Pedido.Recebedor.Nome, 50) : cargaPedido.Pedido.Destinatario != null ? Utilidades.String.Left(cargaPedido.Pedido.Destinatario.Nome, 50) : string.Empty,
                            qtVolumes = cargaPedido.Pedido.QtVolumes,
                            qtPecas = cargaPedido.Pedido.QtVolumes,
                            nrControleCliente1 = "",
                            nrControleCliente2 = "",
                            nrControleCliente3 = "",
                            nrControleCliente7 = "",
                            nrControleCliente8 = "",
                            nrControleCliente9 = "",
                            nrControleCliente10 = "",
                            produtos = produtosPedido.ToArray(),
                            vlCubagem = 0,
                            vlPeso = pesoTotal,
                            cdTransp = cdTrans,
                            flTrocaNota = 0,
                            cdProgramacaoDestinatario = 0,
                            cdCid = cdCid,
                            cdEmbarcador = codigoEmbarcadorOpenTech,
                            cdPaisOrigemDestinatario = cdPaisOrigemDestinatario,
                            cdPaisOrigemEmitente = cdPaisOrigemEmitente,
                            cdProgramacao = cdProgramacao,
                            cdTrocaNota = cdTrocaNota,
                            dsNavio = dsNavio,
                            dsSiglaDest = dsSiglaDest,
                            dsSiglaOrig = dsSiglaOrig,
                            flRegiao = flRegiao,
                            nfs = nfsOpen.ToArray(),
                            nrCnpjCpfEmitente = nrCnpjCpfEmitente,
                            nrDDDFone1 = nrDDDFone1,
                            nrDDDFone2 = nrDDDFone2,
                            nrLacreArmador = nrLacreArmador,
                            nrLacreSIF = nrLacreSIF,
                            flReentrega = 0,
                            vlInicioDiariaAtrasado = 0,
                            vlInicioDiariaNoPrazo = 0
                        });
                    }

                    ServicoOpenTech.sgrData retornoIntegracao = null;

                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    try
                    {
                        int.TryParse(protocoloIntegracao, out int cdViagem);

                        retornoIntegracao = svcOpenTech.sgrNovaEntregaProdutoV3(retornoLogin.ReturnKey,
                                                                                codigoPASOpenTech,
                                                                                codigoClienteOpenTech,
                                                                                cdViagem,
                                                                                documentosV2.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }

                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", $"Integração NovaEntregaProdutoV3 - {retornoIntegracao?.ReturnDescription ?? ""} ");

                    if (!string.IsNullOrEmpty(cargaIntegracao.PreProtocolo) || (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK"))
                    {
                        int cdviag = 0;
                        if (string.IsNullOrEmpty(cargaIntegracao.PreProtocolo))
                            cdviag = int.Parse(retornoIntegracao.ReturnDataset.Nodes[1].Value);
                        else
                        {
                            int PreProtocolo;
                            if (int.TryParse(cargaIntegracao.PreProtocolo, out PreProtocolo))
                                cdviag = PreProtocolo;
                        }

                        cargaIntegracao.PreProtocolo = cdviag.ToString();
                        ServicoOpenTech.sgrData retornoIntegracaoAtualizarRota = null;

                        try
                        {
                            retornoIntegracaoAtualizarRota = svcOpenTech.sgrAtualizaRotaViagem(retornoLogin.ReturnKey,
                                                                                               _configuracaoIntegracaoOpenTech.CodigoPASOpenTech,
                                                                                               cdviag,
                                                                                               _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech,
                                                                                               new ServicoOpenTech.LatLng[] { },
                                                                                               null,
                                                                                               null,
                                                                                               1);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracaoAtualizarRota = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

                        arquivoIntegracaoAtualizarRota.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
                        arquivoIntegracaoAtualizarRota.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
                        arquivoIntegracaoAtualizarRota.Data = DateTime.Now;
                        arquivoIntegracaoAtualizarRota.Mensagem = "Integração AtualizarRotaViagem - " + retornoIntegracaoAtualizarRota?.ReturnDescription ?? "OK";
                        arquivoIntegracaoAtualizarRota.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracaoAtualizarRota);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracaoAtualizarRota);

                        if (retornoIntegracaoAtualizarRota != null && retornoIntegracaoAtualizarRota.ReturnID == 0)
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Integração inclusão documentos e atualização de rota realizada com sucesso. CDVIAG: " + cdviag + ".";
                            cargaIntegracao.Protocolo = cdviag.ToString();
                        }
                        else
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            string mensagemErro = retornoIntegracaoAtualizarRota?.ReturnDescription ?? "Falha ao integrar atualizar Rota Viagem";

                            mensagemErro += ObterMensagemErro(retornoIntegracao);

                            cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                        }
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        string mensagemErro = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar Inclusão documentos ";

                        mensagemErro += ObterMensagemErro(retornoIntegracao);

                        cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErro, 300);
                    }

                    repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                    if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = "6 - Não foi possível comunicar com os serviços da Opentech.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                NotificarPorEmail(cargaIntegracao.Carga, cargaIntegracao.ProblemaIntegracao);

                return;
            }
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaCancelamentoIntegracao.NumeroTentativas++;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                ObterConfiguracaoIntegracaoOpenTech(cargaCancelamentoIntegracao.CargaCancelamento.Carga);
                ObterConfiguracaoIntegracaoEmpresa(cargaCancelamentoIntegracao.CargaCancelamento.Carga?.Empresa);

                if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                    string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Opentech.";

                    repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                    return;
                }

                string protocolo = null;
                int codigoCarga = cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo;
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = cargaCancelamentoIntegracao.TipoIntegracao;

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork, tipoIntegracao.IntegracaoTransportador);

                if (tipoIntegracao.IntegracaoTransportador)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(codigoCarga, tipoIntegracao.Tipo);

                    if (cargaDadosTransporteIntegracao != null)
                        protocolo = cargaDadosTransporteIntegracao.Protocolo;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(codigoCarga, tipoIntegracao.Codigo);

                    if (cargaCargaIntegracao != null)
                        protocolo = cargaCargaIntegracao.Protocolo;
                    else
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(codigoCarga, tipoIntegracao.Tipo);

                        if (cargaDadosTransporteIntegracao != null)
                            protocolo = cargaDadosTransporteIntegracao.Protocolo;
                    }
                }

                if (string.IsNullOrWhiteSpace(protocolo))
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Integração Opentech não foi realizada na carga.";

                    repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                    return;
                }

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaCancelamentoIntegracao);

                    if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                    {
                        cargaCancelamentoIntegracao.ProblemaIntegracao = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                        cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                        return;
                    }

                    ServicoOpenTech.sgrData retornoIntegracao = null;

                    string mensagem = string.Empty;
                    int.TryParse(protocolo, out int cdViagem);

                    retornoIntegracao = svcOpenTech.sgrCancelarViagem(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech,
                                                                      cdViagem,
                                                                      1,
                                                                      "CARGA CANCELADA PELO EMBARCADOR");


                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                    if (retornoIntegracao != null && retornoIntegracao.ReturnID == 0)
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        mensagem = retornoIntegracao?.ReturnDescription ?? "Cancelamento realizado com sucesso";
                    }

                    mensagem = retornoIntegracao?.ReturnDescription ?? "Falha na integração com a Opentech";
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;

                    repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public void AtualizarVeiculoMotoristaColeta(Dominio.Entidades.Embarcador.Cargas.Carga carga, string cpfMotoristaNovo, string cpfMotoristaAnterior, string placaTracaoNova, string placaTracaoAnterior, string placaCarretaAnterior, string placaCarretaNova, string placaCarreta2Anterior, string placaCarreta2Nova)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ObterConfiguracaoIntegracaoOpenTech(carga);
            ObterConfiguracaoIntegracaoEmpresa(carga?.Empresa);

            if (!(_configuracaoIntegracaoOpenTech?.AtualizarVeiculoMotoristaOpentech ?? false))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoColeta = repCargaCargaIntegracao.BuscarPorCargaTipoIntegracaoColetaSituacao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

            if (cargaIntegracaoColeta == null || string.IsNullOrWhiteSpace(cargaIntegracaoColeta?.Protocolo))
                return;

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracaoColeta);

                if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                {
                    Servicos.Log.TratarErro("1 - AtualizarVeiculoMotoristaColeta: " + "Não foi possível realizar o login: " + retornoLogin.ReturnDescription, "Opentech");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(cpfMotoristaAnterior) && !string.IsNullOrWhiteSpace(cpfMotoristaNovo) && cpfMotoristaAnterior != cpfMotoristaNovo)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarMotoristaAe(svcOpenTech, retornoLogin, cargaIntegracaoColeta.Protocolo, cpfMotoristaNovo, cpfMotoristaAnterior);

                    servicoArquivoTransacao.Adicionar(cargaIntegracaoColeta, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarMotoristaAE - " + (retornoIntegracao?.ReturnDescription ?? ""));

                    if (retornoIntegracao == null || retornoIntegracao.ReturnID != 0)
                        Servicos.Log.TratarErro("AtualizarVeiculoMotoristaColeta: " + retornoIntegracao?.ReturnDescription ?? "Falha na integração com a Opentech" + retornoLogin.ReturnDescription, "Opentech");
                }

                if (!string.IsNullOrWhiteSpace(placaTracaoAnterior) && !string.IsNullOrWhiteSpace(placaTracaoNova) && placaTracaoAnterior != placaTracaoNova)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarCavaloAe(svcOpenTech, retornoLogin, cargaIntegracaoColeta.Protocolo, placaTracaoNova, placaTracaoAnterior);

                    servicoArquivoTransacao.Adicionar(cargaIntegracaoColeta, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarCavaloAE - " + (retornoIntegracao?.ReturnDescription ?? ""));

                    if (retornoIntegracao == null || retornoIntegracao.ReturnID != 0)
                        Servicos.Log.TratarErro("sgrTrocarCavaloAE: " + retornoIntegracao?.ReturnDescription ?? "Falha na integração com a Opentech" + retornoLogin.ReturnDescription, "Opentech");
                }

                if (!string.IsNullOrWhiteSpace(placaCarretaAnterior) && !string.IsNullOrWhiteSpace(placaCarretaNova) && placaCarretaAnterior != placaCarretaNova)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarCarretaAe(svcOpenTech, retornoLogin, cargaIntegracaoColeta.Protocolo, placaCarretaNova, placaCarretaAnterior);

                    servicoArquivoTransacao.Adicionar(cargaIntegracaoColeta, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarCarretaAE - " + (retornoIntegracao?.ReturnDescription ?? ""));

                    if (retornoIntegracao == null || retornoIntegracao.ReturnID != 0)
                        Servicos.Log.TratarErro("sgrTrocarCarretaAE: " + retornoIntegracao?.ReturnDescription ?? "Falha na integração com a Opentech" + retornoLogin.ReturnDescription, "Opentech");
                }

                if (!string.IsNullOrWhiteSpace(placaCarreta2Anterior) && !string.IsNullOrWhiteSpace(placaCarreta2Nova) && placaCarreta2Anterior != placaCarreta2Nova)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarCarretaAe(svcOpenTech, retornoLogin, cargaIntegracaoColeta.Protocolo, placaCarretaNova, placaCarretaAnterior);

                    servicoArquivoTransacao.Adicionar(cargaIntegracaoColeta, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarCarretaAE 2- " + (retornoIntegracao?.ReturnDescription ?? ""));

                    if (retornoIntegracao == null || retornoIntegracao.ReturnID != 0)
                        Servicos.Log.TratarErro("sgrTrocarCarretaAE 2: " + retornoIntegracao?.ReturnDescription ?? "Falha na integração com a Opentech" + retornoLogin.ReturnDescription, "Opentech");
                }

                repCargaCargaIntegracao.Atualizar(cargaIntegracaoColeta);
            }
        }

        public void AtualizarVeiculoMotoristaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string cpfMotoristaNovo, string cpfMotoristaAnterior, string placaTracaoNova, string placaTracaoAnterior, string placaCarretaAnterior, string placaCarretaNova, string placaCarreta2Anterior, string placaCarreta2Nova)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ObterConfiguracaoIntegracaoOpenTech(cargaDadosTransporteIntegracao.Carga);
            ObterConfiguracaoIntegracaoEmpresa(cargaDadosTransporteIntegracao.Carga?.Empresa);

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaDadosTransporteIntegracao);

                if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                {
                    Servicos.Log.TratarErro("2 - AtualizarVeiculoMotoristaColeta: " + "Não foi possível realizar o login: " + retornoLogin.ReturnDescription, "Opentech");
                    return;
                }

                StringBuilder mensagemErros = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(cpfMotoristaAnterior) && !string.IsNullOrWhiteSpace(cpfMotoristaNovo) && cpfMotoristaAnterior != cpfMotoristaNovo)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarMotoristaAe(svcOpenTech, retornoLogin, cargaDadosTransporteIntegracao.Protocolo, cpfMotoristaNovo, cpfMotoristaAnterior);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarMotoristaAE - " + (retornoIntegracao?.ReturnDescription ?? ""));

                    TratarRetornoIntegracao(ref mensagemErros, retornoIntegracao, cargaDadosTransporteIntegracao, "AtualizarVeiculoMotoristaColeta");
                }

                if (!string.IsNullOrWhiteSpace(placaTracaoAnterior) && !string.IsNullOrWhiteSpace(placaTracaoNova) && placaTracaoAnterior != placaTracaoNova)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarCavaloAe(svcOpenTech, retornoLogin, cargaDadosTransporteIntegracao.Protocolo, placaTracaoNova, placaTracaoAnterior);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarCavaloAE - " + (retornoIntegracao?.ReturnDescription ?? ""));

                    TratarRetornoIntegracao(ref mensagemErros, retornoIntegracao, cargaDadosTransporteIntegracao, "sgrTrocarCavaloAE");
                }

                if (!string.IsNullOrWhiteSpace(placaCarretaAnterior) && !string.IsNullOrWhiteSpace(placaCarretaNova) && placaCarretaAnterior != placaCarretaNova)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarCarretaAe(svcOpenTech, retornoLogin, cargaDadosTransporteIntegracao.Protocolo, placaCarretaNova, placaCarretaAnterior);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarCarretaAE - " + (retornoIntegracao?.ReturnDescription ?? ""));

                    TratarRetornoIntegracao(ref mensagemErros, retornoIntegracao, cargaDadosTransporteIntegracao, "sgrTrocarCarretaAE");
                }

                if (!string.IsNullOrWhiteSpace(placaCarreta2Anterior) && !string.IsNullOrWhiteSpace(placaCarreta2Nova) && placaCarreta2Anterior != placaCarreta2Nova)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = TrocarCarretaAe(svcOpenTech, retornoLogin, cargaDadosTransporteIntegracao.Protocolo, placaCarretaNova, placaCarretaAnterior);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "sgrTrocarCarretaAE 2- " + (retornoIntegracao?.ReturnDescription ?? ""));

                    TratarRetornoIntegracao(ref mensagemErros, retornoIntegracao, cargaDadosTransporteIntegracao, "sgrTrocarCarretaAE 2");
                }

                if (mensagemErros.Length > 0)
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.NumeroTentativas++;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemErros.ToString(), 300);
                    cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso. CDVIAG: " + cargaDadosTransporteIntegracao.Protocolo + ".";
                    cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
                }

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade> ObterCidadesParaIntegracoes(out string msgErro)
        {
            msgErro = "";
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade> cidadesIntegracoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade>();

            try
            {
                ObterConfiguracaoIntegracaoOpenTech();

                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    ServicoOpenTech.sgrData retornoLogin = svcOpenTech.sgrLogin(_configuracaoIntegracaoOpenTech.UsuarioOpenTech, _configuracaoIntegracaoOpenTech.SenhaOpenTech, _configuracaoIntegracaoOpenTech.DominioOpenTech);
                    ServicoOpenTech.sgrData retornoCidades = null;

                    retornoCidades = svcOpenTech.sgrListaCidades(retornoLogin.ReturnKey);

                    if (retornoCidades != null && retornoCidades.ReturnDataset != null)
                    {
                        cidadesIntegracoes = MontarDadosObterCidadesParaIntegracoes(retornoCidades);

                        return cidadesIntegracoes;
                    }

                    msgErro = retornoCidades?.ReturnDescription ?? "Falha ao integrar";

                    msgErro += ObterMensagemErro(retornoCidades);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao Obter cidades opentech - {ex}");
            }

            return cidadesIntegracoes;
        }

        public byte[] ObterAutorizacaoEmbarque(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out string mensagem)
        {
            ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
            ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

            if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
            {
                mensagem = "A configuração de integração para a OpenTech é inválida.";
                return null;
            }

            int codigoViagem;

            if (cargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ||
                string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo) ||
                !int.TryParse(cargaIntegracao.Protocolo, out codigoViagem))
            {
                mensagem = "A carga não foi integrada com sucesso ou não há um código de viagem registrado para esta integração.";
                return null;
            }

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {


                ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                {
                    mensagem = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                    return null;
                }

                int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                ServicoOpenTech.sgrData dados = svcOpenTech.sgrRetornaAE(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoViagem);

                if (dados.ReturnID != 0)
                {
                    mensagem = "Não foi possível obter os dados da autorização de embarque: " + dados.ReturnDescription;
                    return null;
                }

                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque> autorizacaoEmbarque = ObterListaAutorizacaoEmbarque(dados.ReturnDataset);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota> trechos = ObterListaTrechos(dados.ReturnDataset, autorizacaoEmbarque[0]);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento> documentos = ObterListaDocumentos(dados.ReturnDataset, autorizacaoEmbarque[0]);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido> locaisParada = ObterListaLocaisParada(dados.ReturnDataset, autorizacaoEmbarque[0]);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado> produtos = ObterListaProdutos(dados.ReturnDataset, autorizacaoEmbarque[0]);



                var report = ReportRequest.WithType(ReportType.AutorizacaoEmbarqueOpenTech)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigocargaIntegracao", cargaIntegracao.Codigo)
                    .AddExtraData("autorizacaoEmbarque", autorizacaoEmbarque.ToJson())
                    .AddExtraData("trechos", trechos.ToJson())
                    .AddExtraData("documentos", documentos.ToJson())
                    .AddExtraData("locaisParada", locaisParada.ToJson())
                    .AddExtraData("produtos", produtos.ToJson())
                    .CallReport();
                mensagem = report.ErrorMessage;

                return report.GetContentFile();
            }
        }

        public byte[] ObterAutorizacaoEmbarque(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, out string mensagem)
        {
            ObterConfiguracaoIntegracaoOpenTech(cargaIntegracao.Carga);
            ObterConfiguracaoIntegracaoEmpresa(cargaIntegracao.Carga?.Empresa);

            if (_configuracaoIntegracaoOpenTech == null || _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech <= 0 || _configuracaoIntegracaoOpenTech.CodigoPASOpenTech <= 0 ||
                string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.DominioOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.SenhaOpenTech) ||
                string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.URLOpenTech) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoOpenTech.UsuarioOpenTech))
            {
                mensagem = "A configuração de integração para a OpenTech é inválida.";
                return null;
            }

            int codigoViagem;

            if (cargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ||
                string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo) ||
                !int.TryParse(cargaIntegracao.Protocolo, out codigoViagem))
            {
                mensagem = "A carga não foi integrada com sucesso ou não há um código de viagem registrado para esta integração.";
                return null;
            }

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(svcOpenTech, ref inspector, ref cargaIntegracao);

                if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                {
                    mensagem = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;
                    return null;
                }

                int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                ServicoOpenTech.sgrData dados = svcOpenTech.sgrRetornaAE(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, codigoViagem);

                if (dados.ReturnID != 0)
                {
                    mensagem = "Não foi possível obter os dados da autorização de embarque: " + dados.ReturnDescription;
                    return null;
                }

                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque> autorizacaoEmbarque = ObterListaAutorizacaoEmbarque(dados.ReturnDataset);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota> trechos = ObterListaTrechos(dados.ReturnDataset, autorizacaoEmbarque[0]);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento> documentos = ObterListaDocumentos(dados.ReturnDataset, autorizacaoEmbarque[0]);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido> locaisParada = ObterListaLocaisParada(dados.ReturnDataset, autorizacaoEmbarque[0]);
                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado> produtos = ObterListaProdutos(dados.ReturnDataset, autorizacaoEmbarque[0]);

                var report = ReportRequest.WithType(ReportType.AutorizacaoEmbarqueOpenTech)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigocargaIntegracao", cargaIntegracao.Codigo)
                    .AddExtraData("autorizacaoEmbarque", autorizacaoEmbarque.ToJson())
                    .AddExtraData("trechos", trechos.ToJson())
                    .AddExtraData("documentos", documentos.ToJson())
                    .AddExtraData("locaisParada", locaisParada.ToJson())
                    .AddExtraData("produtos", produtos.ToJson())
                    .CallReport();
                mensagem = report.ErrorMessage;

                return report.GetContentFile();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool EnviarMotorista(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, ServicoOpenTech.sgrData retornoLogin)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            ServicoOpenTech.sgrData retornoIntegracao = IntegrarEnvioMotorista(cargaIntegracao.Carga, retornoLogin, out string request, out string response);

            if (retornoIntegracao == null || retornoIntegracao.ReturnDataset == null || retornoIntegracao.ReturnDescription != "OK")
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                servicoArquivoTransacao.Adicionar(cargaIntegracao, request, response, "xml", "Erro ao enviar motorista - Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

                cargaIntegracao.ProblemaIntegracao = "Erro ao enviar motorista - " + retornoIntegracao?.ReturnDescription ?? "";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                return false;
            }

            return true;
        }

        private bool EnviarMotorista(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, ServicoOpenTech.sgrData retornoLogin)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            ServicoOpenTech.sgrData retornoIntegracao = IntegrarEnvioMotorista(cargaIntegracao.Carga, retornoLogin, out string request, out string response);


            if (retornoIntegracao == null || retornoIntegracao.ReturnDataset == null || retornoIntegracao.ReturnDescription != "OK")
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                servicoArquivoTransacao.Adicionar(cargaIntegracao, request, response, "xml", "Erro ao enviar motorista - Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

                cargaIntegracao.ProblemaIntegracao = "Erro ao enviar motorista - " + retornoIntegracao?.ReturnDescription ?? "";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                return false;
            }

            return true;
        }

        private Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech ObterProdutoOpenTechIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPercurso destino, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes, decimal valorTotalProdutos = 0)
        {
            Repositorio.Embarcador.Integracao.ProdutoOpentech repositorioProdutoOpentech = new Repositorio.Embarcador.Integracao.ProdutoOpentech(_unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> produtosOpenTech = new List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>();
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroCarga = repositorioApoliceSeguroAverbacao.BuscarApolicesPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroCarregamento = carga.Carregamento != null ? repositorioCarregamentoApolice.BuscarApolicesPorCarregamento(carga.Carregamento.Codigo) : new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = null;

            decimal valorNotas = 0;
            if (valorTotalProdutos != 0)
                valorNotas = valorTotalProdutos;
            else
                valorNotas = (from obj in cargaPedidos select obj.Pedido).Sum(o => o.ValorTotalNotasFiscais);

            valorNotas = (valorNotas > 0 ? valorNotas : (from obj in cargaCtes select obj.NotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor)).Sum());

            if (apolicesSeguroCarga.Count > 0)
            {
                if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                    produtosOpenTech = repositorioProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeApolice(carga.TipoOperacao?.Codigo ?? 0, destino?.Destino?.Estado?.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), carga.TipoDeCarga?.Codigo ?? 0, destino.Destino.Codigo);
                else
                    produtosOpenTech = repositorioProdutoOpentech.BuscarPorOperacaoEstadoApolice(carga.TipoOperacao?.Codigo ?? 0, destino?.Destino?.Estado?.Sigla, apolicesSeguroCarga.Select(o => o.Codigo).ToList(), carga.TipoDeCarga?.Codigo ?? 0);

                produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorNotas && x.ValorAte >= valorNotas).FirstOrDefault();

                if (produtoOpentech == null)
                    produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
            }
            else if (apolicesSeguroCarregamento.Count > 0)
            {
                if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                    produtosOpenTech = repositorioProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeApolice(carga.TipoOperacao?.Codigo ?? 0, destino?.Destino?.Estado?.Sigla, apolicesSeguroCarregamento.Select(o => o.Codigo).ToList(), carga.TipoDeCarga?.Codigo ?? 0, destino.Destino.Codigo);
                else
                    produtosOpenTech = repositorioProdutoOpentech.BuscarPorOperacaoEstadoApolice(carga.TipoOperacao?.Codigo ?? 0, destino?.Destino?.Estado?.Sigla, apolicesSeguroCarregamento.Select(o => o.Codigo).ToList(), carga.TipoDeCarga?.Codigo ?? 0);
                produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorNotas && x.ValorAte >= valorNotas).FirstOrDefault();

                if (produtoOpentech == null)
                    produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
            }

            if (produtoOpentech == null)
            {
                if (_configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false)
                    produtosOpenTech = repositorioProdutoOpentech.BuscarPorOperacaoEstadoELocalidadeSemApolice(carga.TipoOperacao?.Codigo ?? 0, destino?.Destino?.Estado?.Sigla, carga.TipoDeCarga?.Codigo ?? 0, destino.Destino.Codigo);
                else
                    produtosOpenTech = repositorioProdutoOpentech.BuscarPorOperacaoEstadoSemApolice(carga.TipoOperacao?.Codigo ?? 0, destino?.Destino?.Estado?.Sigla, carga.TipoDeCarga?.Codigo ?? 0);
                produtoOpentech = produtosOpenTech.Where(x => x.ValorDe <= valorNotas && x.ValorAte >= valorNotas).FirstOrDefault();

                if (produtoOpentech == null)
                    produtoOpentech = produtosOpenTech.Where(x => x.ValorDe == 0 && x.ValorAte == 0).FirstOrDefault();
            }

            return produtoOpentech;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade> MontarDadosObterCidadesParaIntegracoes(ServicoOpenTech.sgrData retornoCidades)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade> cidadesIntegracoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade>();

            XElement xmlData = (XElement)retornoCidades.ReturnDataset.Nodes[1].FirstNode;
            IEnumerable<XElement> cidadesXml = xmlData.Elements("sgrTB");

            foreach (XElement cidadeXml in cidadesXml)
            {
                string cdibgeStr = cidadeXml.Element("CDIBGE")?.Value ?? "0";
                string cdcidStr = cidadeXml.Element("CDCID")?.Value ?? "0";
                string dscidade = cidadeXml.Element("DSCIDADE")?.Value ?? "";
                string dsuf = cidadeXml.Element("DSUF")?.Value ?? "";
                string dspais = cidadeXml.Element("DSPAIS")?.Value ?? "";

                int.TryParse(cdibgeStr, out int codigoIBGE);
                int.TryParse(cdcidStr, out int codigoIntegracao);

                Servicos.Log.TratarErro("CDCID: " + cdcidStr +
                                        " | DSCIDADE: " + dscidade +
                                        " | DSUF: " + dsuf +
                                        " | CDIBGE: " + cdibgeStr +
                                        " | DSPAIS: " + dspais, "Cidades Opentech");

                if (codigoIBGE == 0) continue;

                cidadesIntegracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade
                {
                    IBGE = codigoIBGE,
                    Integracao = codigoIntegracao,
                    Cidade = dscidade,
                    Estado = dsuf,
                    Pais = dspais.ToUpper()
                });
            }

            return cidadesIntegracoes;
        }

        private ServicoOpenTech.sgrData IntegrarEnvioMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, ServicoOpenTech.sgrData retornoLogin, out string request, out string response)
        {
            ObterConfiguracaoIntegracaoOpenTech(carga);

            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

            ServicoOpenTech.sgrData retornoIntegracao = new ServicoOpenTech.sgrData();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> listaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);
            request = string.Empty;
            response = string.Empty;

            if (listaMotoristas != null && listaMotoristas.Count > 0)
            {
                using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                {
                    foreach (var mot in listaMotoristas)
                    {
                        Dominio.Entidades.Usuario motoristaCarga = mot.Motorista;
                        try
                        {
                            string telefone = string.Empty;
                            string dddTelefone = string.Empty;
                            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(motoristaCarga.Telefone)))
                            {
                                string telefoneCompleto = Utilidades.String.OnlyNumbers(motoristaCarga.Telefone);
                                if (telefoneCompleto.Length >= 10)
                                {

                                    if (telefoneCompleto.StartsWith("0"))
                                    {
                                        dddTelefone = telefoneCompleto.Substring(1, 2);
                                        telefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
                                    }
                                    else
                                    {
                                        dddTelefone = telefoneCompleto.Substring(0, 2);
                                        telefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
                                    }
                                }
                            }

                            Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade cidadeIntegracao = repConfiguracaoIntegracaoLocalidade.BuscarPorIBGE(motoristaCarga?.Localidade?.CodigoIBGE ?? 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech);

                            retornoIntegracao = svcOpenTech.sgrAdicionarPessoaFisicaRapidoV3(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech,
                                                                                             1, //cdpaisorigem
                                                                                             ((int?)(motoristaCarga?.OrgaoEmissorRG)) ?? -1,
                                                                                             (motoristaCarga?.EstadoRG != null) ? ObterCodigoEstadoOpenTech(motoristaCarga.EstadoRG.Sigla) : -1, //sgrListaUF -- Código do estado de emissão da CNH
                                                                                             (motoristaCarga != null && motoristaCarga.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio) ? "F" : "T", //Lista os tipos de vínculo para o perfil profissional (agregado, funcionário, terceiro)
                                                                                             ((motoristaCarga?.DataEmissaoRG.HasValue ?? false) ? motoristaCarga.DataEmissaoRG.Value.ToString("yyyy-MM-dd") : string.Empty),
                                                                                             ((motoristaCarga?.DataHabilitacao.HasValue ?? false) ? motoristaCarga.DataHabilitacao.Value.ToString("yyyy-MM-dd") : string.Empty),
                                                                                             ((motoristaCarga?.DataVencimentoHabilitacao.HasValue ?? false) ? motoristaCarga.DataVencimentoHabilitacao.Value.ToString("yyyy-MM-dd") : string.Empty),
                                                                                             ((motoristaCarga?.DataVencimentoMoop.HasValue ?? false) ? motoristaCarga.DataVencimentoMoop.Value.ToString("yyyy-MM-dd") : string.Empty),
                                                                                             motoristaCarga?.CPF ?? string.Empty,
                                                                                             motoristaCarga?.RG ?? string.Empty,
                                                                                             motoristaCarga?.NumeroHabilitacao ?? string.Empty,
                                                                                             motoristaCarga?.Categoria ?? string.Empty,
                                                                                             string.Empty, //strDocNrRegistro
                                                                                             ((motoristaCarga?.DataNascimento.HasValue ?? false) ? motoristaCarga.DataNascimento.Value.ToString("yyyy-MM-dd") : string.Empty),
                                                                                             1, //numDpNacionalidade
                                                                                             motoristaCarga?.Nome ?? string.Empty,
                                                                                             cidadeIntegracao != null ? cidadeIntegracao.CodigoIntegracao : -1, //motoristaCarga.Localidade?.CodigoIBGE ?? 0,
                                                                                             telefone ?? string.Empty,
                                                                                             string.Empty, //strEndFone2
                                                                                             string.Empty, //strEndFax
                                                                                             -1, //cdEntidadeMOPP 
                                                                                             string.Empty, //nrMOPP
                                                                                             ((motoristaCarga?.DataAdmissao.HasValue ?? false) ? motoristaCarga.DataAdmissao.Value.ToString("yyyy-MM-dd") : string.Empty),
                                                                                             string.Empty, //nrMatricula
                                                                                             dddTelefone ?? string.Empty,
                                                                                             string.Empty, //DDD2
                                                                                             string.Empty, //DDDFax
                                                                                             string.Empty //strNrRadio
                                                                                             );

                            request = inspector?.LastRequestXML ?? string.Empty;
                            response = inspector?.LastResponseXML ?? string.Empty;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                }
            }

            return retornoIntegracao;
        }

        private bool EnviarVeiculo(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, ServicoOpenTech.sgrData retornoLogin)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                var listaVeiculos = new List<Dominio.Entidades.Veiculo>
                {
                    cargaIntegracao.Carga.Veiculo
                };

                foreach (var reboque in cargaIntegracao.Carga.VeiculosVinculados)
                    listaVeiculos.Add(reboque);

                foreach (var veiculo in listaVeiculos)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = null;

                    try
                    {
                        List<int> listaAcessorios = new List<int>();

                        ServicoOpenTech.CadastroVeiculo cadastroVeiculo = new ServicoOpenTech.CadastroVeiculo
                        {
                            cdpaisorigem = 1,
                            cdModVeic = 834, //NAO INFORMADO
                            cdTpVeic = veiculo.TipoVeiculoOpentech,
                            cdCor = 42, //NAO INFORMADA
                            nrAnoFab = veiculo.AnoFabricacao,
                            cdTipoCar = veiculo.TipoCarroceriaoOpentech,
                            cdCidLicencia = veiculo.Empresa == null ? 0 : repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(veiculo.Empresa.Localidade.Codigo)?.CodigoIntegracao ?? 0,
                            nrPlaca = veiculo.Placa,
                            nrFrota = veiculo.NumeroFrota,
                            cdEmpRastrea = -1,
                            cdEnderRastr = string.Empty,
                            cdEmpRastrea2 = -1,
                            cdEnderRastr2 = string.Empty,
                            flVinculo = veiculo.Tipo == "P" ? "F" : "T",
                            vlCapPallets = 0,
                            vlTara = 0,
                            vlCapVol = 0,
                            vlCapPeso = veiculo.CapacidadeKG,
                            acessorios = listaAcessorios.ToArray(),
                            dtInicialTolerancia = "1900-01-01",
                            dtFinalTolerancia = "1900-01-01",
                            cdOpTolerancia = -1,
                            flTpRastSatelital = -1,
                            flTpRastGPRS = -1
                        };

                        retornoIntegracao = svcOpenTech.sgrAdicionarVeiculoRapidoCompV5(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, cadastroVeiculo, false);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }

                    if (retornoIntegracao == null || retornoIntegracao.ReturnDataset == null || retornoIntegracao.ReturnDescription != "OK")
                    {
                        string msgErro = "Erro ao enviar veiculo";

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
                        arquivoIntegracao.Data = DateTime.Now;
                        arquivoIntegracao.Mensagem = "Integração - " + retornoIntegracao?.ReturnDescription ?? "";
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, msgErro, "", arquivoIntegracao);

                        return false;
                    }
                }

                if (EnviarVeiculoInterno(cargaIntegracao.Carga, retornoLogin, cargaIntegracao, out string msgRetornoIntegracao))
                    return true;

                cargaIntegracao.ProblemaIntegracao = "Erro ao enviar veículo - " + msgRetornoIntegracao;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                return false;
            }
        }

        private bool EnviarVeiculo(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, ServicoOpenTech.sgrData retornoLogin)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            if (EnviarVeiculoInterno(cargaIntegracao.Carga, retornoLogin, cargaIntegracao, out string msgRetornoIntegracao))
                return true;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaIntegracao.ProblemaIntegracao = "Erro ao enviar veículo - " + msgRetornoIntegracao;
            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            repCargaCargaIntegracao.Atualizar(cargaIntegracao);

            return false;
        }

        private bool EnviarVeiculoInterno<T>(Dominio.Entidades.Embarcador.Cargas.Carga carga, ServicoOpenTech.sgrData retornoLogin, T integracaoEntity, out string msgRetornoIntegracao)
            where T : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>
        {
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            IList<int> codigosVeiculosVinculados = repVeiculo.BuscarVeiculosVinculadoACarga(carga.Codigo);
            List<Dominio.Entidades.Veiculo> veiculosVinculados = repVeiculo.BuscarPorCodigos(codigosVeiculosVinculados, false);
            msgRetornoIntegracao = string.Empty;

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                var listaVeiculos = new List<Dominio.Entidades.Veiculo>
                {
                    carga.Veiculo
                };

                foreach (Dominio.Entidades.Veiculo reboque in veiculosVinculados)
                    listaVeiculos.Add(reboque);

                foreach (var veiculo in listaVeiculos)
                {
                    ServicoOpenTech.sgrData retornoIntegracao = null;

                    try
                    {
                        List<int> listaAcessorios = new List<int>();

                        ServicoOpenTech.CadastroVeiculo cadastroVeiculo = new ServicoOpenTech.CadastroVeiculo
                        {
                            cdpaisorigem = 1,
                            cdModVeic = 834, //NAO INFORMADO
                            cdTpVeic = veiculo.TipoVeiculoOpentech,
                            cdCor = 42, //NAO INFORMADA
                            nrAnoFab = veiculo.AnoFabricacao,
                            cdTipoCar = veiculo.TipoCarroceriaoOpentech,
                            cdCidLicencia = veiculo.Empresa == null ? 0 : repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(veiculo.Empresa.Localidade.Codigo)?.CodigoIntegracao ?? 0,
                            nrPlaca = veiculo.Placa,
                            nrFrota = veiculo.NumeroFrota,
                            cdEmpRastrea = -1,
                            cdEnderRastr = string.Empty,
                            cdEmpRastrea2 = -1,
                            cdEnderRastr2 = string.Empty,
                            flVinculo = veiculo.Tipo == "P" ? "F" : "T",
                            vlCapPallets = 0,
                            vlTara = 0,
                            vlCapVol = 0,
                            vlCapPeso = veiculo.CapacidadeKG,
                            acessorios = listaAcessorios.ToArray(),
                            dtInicialTolerancia = "1900-01-01",
                            dtFinalTolerancia = "1900-01-01",
                            cdOpTolerancia = -1,
                            flTpRastSatelital = -1,
                            flTpRastGPRS = -1
                        };

                        retornoIntegracao = svcOpenTech.sgrAdicionarVeiculoRapidoCompV5(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, cadastroVeiculo, false);

                        msgRetornoIntegracao = retornoIntegracao?.ReturnDescription ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }

                    if (retornoIntegracao == null || retornoIntegracao.ReturnDataset == null || retornoIntegracao.ReturnDescription != "OK")
                    {
                        ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                        servicoArquivoTransacao.Adicionar(integracaoEntity, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Erro ao enviar veiculo - Integração - " + retornoIntegracao?.ReturnDescription ?? "");

                        return false;
                    }
                }
            }

            return true;
        }

        private int ObterCodigoEmbarcadorOpenTech(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercursoDestino)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repCargaCTe.BuscarPorCargaSemPreCte(carga.Codigo);

            int codigoEmbarcadorOpenTech = 0;
            if (_configuracaoIntegracaoOpenTech.EnviarCodigoEmbarcadorProdutoOpentech)
            {
                Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech produtoOpentech = ObterProdutoOpenTechIntegracaoCarga(carga, cargaPercursoDestino, cargaCtes);
                codigoEmbarcadorOpenTech = (produtoOpentech?.CodigoEmbarcador != null ? produtoOpentech.CodigoEmbarcador : 0);
            }

            codigoEmbarcadorOpenTech = (codigoEmbarcadorOpenTech > 0 ? codigoEmbarcadorOpenTech : _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech);

            return codigoEmbarcadorOpenTech;
        }

        private List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> ObterListaDocumentosIntegracaoPorPedido(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out decimal valorCarga, out decimal valorTotalNotas, bool valorMaior, ref string dtprevini, ref string dtprevfim, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int cdTranspDocumentos, int codigoEmbarcadorOpenTech, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech repGrupoProdutoOpenTech = new Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech(_unitOfWork);
            List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();

            int codigoBrasilOpenTech = 1;

            DateTime? dataPrevisaoEntrega = cargaPedidos.Max(p => p.Pedido.DataFinalColeta);
            if (!dataPrevisaoEntrega.HasValue || dataPrevisaoEntrega == DateTime.MinValue)
                dataPrevisaoEntrega = DateTime.Now;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                DateTime? dataPrevisaoSaida = cargaPedidos.Where(o => o.Pedido.DataPrevisaoSaida.HasValue).Max(o => o.Pedido.DataPrevisaoSaida);
                dataPrevisaoEntrega = cargaPedidos.Where(o => o.Pedido.PrevisaoEntrega.HasValue).Max(o => o.Pedido.PrevisaoEntrega);

                if (dataPrevisaoSaida.HasValue)
                    dtprevini = dataPrevisaoSaida.Value.ToString("yyyy-MM-dd HH:mm");

                if (!dataPrevisaoEntrega.HasValue || dataPrevisaoEntrega == DateTime.MinValue)
                    dataPrevisaoEntrega = DateTime.Now;

                if (dataPrevisaoEntrega.HasValue)
                    dtprevfim = dataPrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm");
            }

            valorCarga = 0;
            valorTotalNotas = (from obj in cargaCtes select obj.NotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor)).Sum();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutosCTe = cargaPedido.Produtos.ToList();

                decimal valorTotalNotasCTe = pedido.NotasFiscais.Where(o => o.nfAtiva && !o.TipoFatura).Sum(o => o.Valor);
                if (valorTotalNotasCTe <= 0)
                    valorTotalNotasCTe = pedido.ValorTotalNotasFiscais;
                decimal valorTotalProdutosCTe = cargaPedidosProdutosCTe.Sum(o => o.ValorUnitarioProduto * o.Quantidade);
                decimal diferencaNotasCTe = valorTotalNotasCTe - valorTotalProdutosCTe;
                valorCarga += valorTotalNotasCTe;

                int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                int cdPaisOrigemEmitente = codigoBrasilOpenTech;

                string dsSiglaOrig = pedido.LocalidadeInicioPrestacao?.Estado?.Sigla ?? pedido.Origem?.Estado?.Sigla ?? string.Empty;
                string dsSiglaDest = pedido.LocalidadeTerminoPrestacao?.Estado?.Sigla ?? pedido.Destino?.Estado?.Sigla ?? string.Empty;
                string nrCnpjCpfEmitente = (pedido.Empresa ?? cargaIntegracao.Carga.Empresa)?.CNPJ;

                List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();

                if (diferencaNotasCTe < 0m)
                    diferencaNotasCTe = 0m;

                List<int> codigosProdutosCTe = (from obj in cargaPedidosProdutosCTe select obj.Produto.Codigo).Distinct().ToList();

                List<ServicoOpenTech.sgrProduto> produtosCTe = new List<ServicoOpenTech.sgrProduto>();
                for (var i = 0; i < codigosProdutosCTe.Count; i++)
                {
                    int codigoProduto = codigosProdutosCTe[i];
                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech grupoProdutoOpenTech = repGrupoProdutoOpenTech.BuscarPorGrupoProduto((from obj in cargaPedidosProdutosCTe where obj.Produto.Codigo == codigoProduto select obj.Produto.GrupoProduto?.Codigo ?? 0).First());

                    decimal valorProduto = (from obj in cargaPedidosProdutosCTe where obj.Produto.Codigo == codigoProduto select (obj.ValorUnitarioProduto * obj.Quantidade)).Sum();
                    if (i == 0) valorProduto += diferencaNotasCTe;

                    ServicoOpenTech.sgrProduto sgrProduto = (from obj in produtosCTe where obj.cdprod == (valorMaior ? grupoProdutoOpenTech?.CodigoProdutoValorMaior ?? 0 : grupoProdutoOpenTech?.CodigoProdutoValorMenor ?? 0) select obj).FirstOrDefault();

                    if (sgrProduto == null)
                    {
                        produtosCTe.Add(new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = valorMaior ? grupoProdutoOpenTech?.CodigoProdutoValorMaior ?? 0 : grupoProdutoOpenTech?.CodigoProdutoValorMenor ?? 0,
                            valor = valorProduto
                        });
                    }
                    else
                        sgrProduto.valor += valorProduto;
                }

                DateTime dtPrevista = pedido.PrevisaoEntrega ?? dataPrevisaoEntrega.Value;
                if (_configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false)
                    if (pedido?.NotasFiscais?.FirstOrDefault()?.DataEmissao != null)
                        dtPrevista = pedido.NotasFiscais.FirstOrDefault().DataEmissao;

                ServicoOpenTech.sgrDocumentoProdutosSeqV2 documentoSequencia = ObterDocumentoSequencia(cargaPedido);

                documentoSequencia.produtos = produtosCTe?.ToArray();
                documentoSequencia.dtPrevista = dtPrevista;
                documentoSequencia.cdEmbarcador = codigoEmbarcadorOpenTech;
                documentoSequencia.cdPaisOrigemDestinatario = cdPaisOrigemDestinatario;
                documentoSequencia.cdPaisOrigemEmitente = cdPaisOrigemEmitente;
                documentoSequencia.dsSiglaDest = dsSiglaDest;
                documentoSequencia.dsSiglaOrig = dsSiglaOrig;
                documentoSequencia.nfs = nfsOpen.ToArray();
                documentoSequencia.nrCnpjCpfEmitente = nrCnpjCpfEmitente;
                documentoSequencia.cdTransp = cdTranspDocumentos;

                ValidarCargaVinculadaIntegracaoOpentech(cargaIntegracao);

                documentosV2.Add(documentoSequencia);
            }

            return documentosV2;
        }

        private List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> ObterListaDocumentosIntegracaoPorCTe(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out decimal valorCarga, out decimal valorTotalNotas, bool valorMaior, ref string dtprevini, ref string dtprevfim, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int cdTranspDocumentos, int codigoEmbarcadorOpenTech, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool cargaEspelho = false)
        {
            Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech repGrupoProdutoOpenTech = new Repositorio.Embarcador.Produtos.GrupoProdutoOpenTech(_unitOfWork);
            List<Servicos.ServicoOpenTech.sgrDocumentoProdutosSeqV2> documentosV2 = new List<ServicoOpenTech.sgrDocumentoProdutosSeqV2>();

            int codigoBrasilOpenTech = 1;

            DateTime? dataPrevisaoEntrega = cargaPedidos.Max(p => p.Pedido.DataFinalColeta);
            if (!dataPrevisaoEntrega.HasValue || dataPrevisaoEntrega == DateTime.MinValue)
                dataPrevisaoEntrega = DateTime.Now;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                DateTime? dataPrevisaoSaida = cargaPedidos.Where(o => o.Pedido.DataPrevisaoSaida.HasValue).Max(o => o.Pedido.DataPrevisaoSaida);
                dataPrevisaoEntrega = cargaPedidos.Where(o => o.Pedido.PrevisaoEntrega.HasValue).Max(o => o.Pedido.PrevisaoEntrega);

                if (!dataPrevisaoEntrega.HasValue || dataPrevisaoEntrega == DateTime.MinValue)
                    dataPrevisaoEntrega = DateTime.Now;

                if (dataPrevisaoSaida.HasValue)
                    dtprevini = dataPrevisaoSaida.Value.ToString("yyyy-MM-dd HH:mm");

                if (dataPrevisaoEntrega.HasValue)
                    dtprevfim = dataPrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm");
            }

            valorCarga = 0;
            valorTotalNotas = (from obj in cargaCtes select obj.NotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor)).Sum();

            if (valorTotalNotas <= 0m)
                valorTotalNotas = cargaCtes.Sum(o => o.CTe.ValorTotalMercadoria);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCtes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = (from obj in cargaCTe.NotasFiscais select obj.PedidoXMLNotaFiscal.CargaPedido).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutosCTe = (from obj in cargaPedido select obj.Produtos).SelectMany(o => o).ToList();

                decimal valorTotalNotasCTe = cargaCTe.NotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor);
                if (valorTotalNotasCTe <= 0)
                    valorTotalNotasCTe = cargaCTe.CTe.ValorTotalMercadoria;

                decimal valorTotalProdutosCTe = cargaPedidosProdutosCTe.Sum(o => o.ValorUnitarioProduto * o.Quantidade);
                decimal diferencaNotasCTe = valorTotalNotasCTe - valorTotalProdutosCTe;
                valorCarga += valorTotalNotasCTe;

                int cdPaisOrigemDestinatario = codigoBrasilOpenTech;
                int cdPaisOrigemEmitente = codigoBrasilOpenTech;

                string dsSiglaDest = cargaCTe.CTe.LocalidadeTerminoPrestacao.Estado.Sigla;
                string dsSiglaOrig = cargaCTe.CTe.LocalidadeInicioPrestacao.Estado.Sigla;
                string nrCnpjCpfEmitente = cargaCTe.CTe.Empresa.CNPJ;

                List<ServicoOpenTech.sgrNF> nfsOpen = new List<ServicoOpenTech.sgrNF>();

                if (diferencaNotasCTe < 0m)
                    diferencaNotasCTe = 0m;

                List<int> codigosProdutosCTe = (from obj in cargaPedidosProdutosCTe select obj.Produto.Codigo).Distinct().ToList();

                List<ServicoOpenTech.sgrProduto> produtosCTe = new List<ServicoOpenTech.sgrProduto>();
                for (var i = 0; i < codigosProdutosCTe.Count; i++)
                {
                    int codigoProduto = codigosProdutosCTe[i];
                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoOpenTech grupoProdutoOpenTech = repGrupoProdutoOpenTech.BuscarPorGrupoProduto((from obj in cargaPedidosProdutosCTe where obj.Produto.Codigo == codigoProduto select obj.Produto.GrupoProduto?.Codigo ?? 0).First());
                    decimal valorProduto = 0;
                    if (cargaEspelho)
                        valorProduto = (from obj in cargaPedidosProdutosCTe where obj.Produto.Codigo == codigoProduto select (obj.ValorUnitarioProduto)).Sum();
                    else
                        valorProduto = (from obj in cargaPedidosProdutosCTe where obj.Produto.Codigo == codigoProduto select (obj.ValorUnitarioProduto * obj.Quantidade)).Sum();

                    if (i == 0) valorProduto += diferencaNotasCTe;

                    ServicoOpenTech.sgrProduto sgrProduto = (from obj in produtosCTe where obj.cdprod == (valorMaior ? grupoProdutoOpenTech?.CodigoProdutoValorMaior ?? 0 : grupoProdutoOpenTech?.CodigoProdutoValorMenor ?? 0) select obj).FirstOrDefault();

                    if (sgrProduto == null)
                    {
                        produtosCTe.Add(new ServicoOpenTech.sgrProduto()
                        {
                            cdprod = valorMaior ? grupoProdutoOpenTech?.CodigoProdutoValorMaior ?? 0 : grupoProdutoOpenTech?.CodigoProdutoValorMenor ?? 0,
                            valor = valorProduto
                        });
                    }
                    else
                        sgrProduto.valor += valorProduto;
                }
                decimal valorDoc = cargaCTe.CTe.ValorAReceber;
                if (_configuracaoIntegracaoOpenTech.EnviarValorDasNotasNoCampoValorDoc || cargaEspelho)
                    valorDoc = valorTotalNotasCTe;

                if (cargaEspelho && codigosProdutosCTe.Count <= 0)
                {
                    produtosCTe.Add(new ServicoOpenTech.sgrProduto()
                    {
                        cdprod = 0,
                        valor = valorDoc
                    });
                }

                ServicoOpenTech.sgrDocumentoProdutosSeqV2 documentoSequencia = ObterDocumentoSequencia(cargaCTe, valorDoc);

                documentoSequencia.produtos = produtosCTe?.ToArray();
                documentoSequencia.dtPrevista = cargaCTe.CTe.DataPrevistaEntrega ?? dataPrevisaoEntrega.Value;
                documentoSequencia.cdEmbarcador = codigoEmbarcadorOpenTech;
                documentoSequencia.cdPaisOrigemDestinatario = cdPaisOrigemDestinatario;
                documentoSequencia.cdPaisOrigemEmitente = cdPaisOrigemEmitente;
                documentoSequencia.dsSiglaDest = dsSiglaDest;
                documentoSequencia.dsSiglaOrig = dsSiglaOrig;
                documentoSequencia.nfs = nfsOpen?.ToArray();
                documentoSequencia.nrCnpjCpfEmitente = nrCnpjCpfEmitente;
                documentoSequencia.cdTransp = cdTranspDocumentos;

                ValidarCargaVinculadaIntegracaoOpentech(cargaIntegracao);

                documentosV2.Add(documentoSequencia);
            }

            return documentosV2;
        }

        private void ValidarCargaVinculadaIntegracaoOpentech(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            try
            {
                // Se CadastrarRotaCargaOpentech = true E Carga não possui uma "rota vinculada" com "código de integração".
                if (cargaIntegracao.Carga.Rota != null && string.IsNullOrWhiteSpace(cargaIntegracao.Carga.Rota?.CodigoIntegracao) && _configuracaoIntegracaoOpenTech.CadastrarRotaCargaOpentech)
                {
                    int codigoPASOpenTech = ObterCodigoPASOpenTech(cargaIntegracao.Carga);
                    int codigoClienteOpenTech = ObterCodigoClienteOpenTech(cargaIntegracao.Carga);

                    // Realizar o cadastro da rota na Opentech
                    using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
                    {
                        var retornoLogin = svcOpenTech.sgrLogin(_configuracaoIntegracaoOpenTech.UsuarioOpenTech, _configuracaoIntegracaoOpenTech.SenhaOpenTech, _configuracaoIntegracaoOpenTech.DominioOpenTech);

                        if (retornoLogin != null && retornoLogin.ReturnDescription == "OK")
                            _openTechSgrDataResult = svcOpenTech.sgrGerarRotaModelo(retornoLogin.ReturnKey, codigoPASOpenTech, codigoClienteOpenTech, cargaIntegracao.Carga.Rota.Descricao, cargaIntegracao.Carga.Rota.PolilinhaRota);

                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void CadastrarRotaOpentech(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {

            var mensagemErro = "";

            try
            {
                // Executa a validação em seguida faz a gravação do código da rota
                ValidarCargaVinculadaIntegracaoOpentech(cargaIntegracao);

                if (_openTechSgrDataResult == null)
                    return;

                string codigoRotaIntegracao = _openTechSgrDataResult.ReturnDataset?.Nodes[1]?.Value ?? "";

                if (string.IsNullOrEmpty(codigoRotaIntegracao))
                    return;

                cargaIntegracao.Carga.Rota.CodigoIntegracao = codigoRotaIntegracao;

                AtualizarSituacaoIntegracao(cargaIntegracao, mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
            }
            catch (Exception ex)
            {
                mensagemErro = $"Erro ao integrar Polilinha Opentech: {ex.Message}";
                AtualizarSituacaoIntegracao(cargaIntegracao, mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
            }
        }

        private void AtualizarSituacaoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoDominio, string problemaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var cargaIntegracaoRepositorio = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaIntegracaoDominio.ProblemaIntegracao = problemaIntegracao;
            cargaIntegracaoDominio.SituacaoIntegracao = situacao;
            cargaIntegracaoDominio.NumeroTentativas++;

            cargaIntegracaoRepositorio.Atualizar(cargaIntegracaoDominio);
        }

        private ServicoOpenTech.sgrDocumentoProdutosSeqV2 ObterDocumentoSequencia(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

            int cdCid = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(pedido.LocalidadeTerminoPrestacao?.Codigo ?? pedido.Destino?.Codigo ?? 0)?.CodigoIntegracao ?? 0;

            return new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
            {
                nrDoc = pedido.Numero.ToString(),
                tpDoc = 3, //Pedido
                valorDoc = cargaPedido.ValorFreteAPagar,
                tpOperacao = 4, //Coleta/Entrega
                dtPrevistaSaida = "",
                dsRua = Utilidades.String.Left(pedido.Destinatario.Endereco, 50),
                nrRua = Utilidades.String.Left(pedido.Destinatario.Numero, 6),
                complementoRua = Utilidades.String.Left(pedido.Destinatario.Complemento, 40),
                cdCidIBGE = pedido.Destinatario.Localidade?.CodigoIBGE ?? 0,
                dsBairro = Utilidades.String.Left(pedido.Destinatario.Bairro, 50),
                nrCep = pedido.Destinatario.CEP,
                nrFone1 = Utilidades.String.OnlyNumbers(pedido.Destinatario.Telefone1),
                nrFone2 = Utilidades.String.OnlyNumbers(pedido.Destinatario.Telefone2),
                nrCnpjCPFDestinatario = pedido.Destinatario.CPF_CNPJ_SemFormato,
                nrCnpjCpfDestinatarioSequencia = "",
                Latitude = (pedido.Destinatario.Latitude ?? "0").ToFloat(),
                Longitude = (pedido.Destinatario.Longitude ?? "0").ToFloat(),
                dsNome = Utilidades.String.Left(pedido.Destinatario.Nome, 50),
                qtVolumes = pedido.NotasFiscais.Where(o => !o.TipoFatura).Sum(o => o.Volumes),
                qtPecas = pedido.NotasFiscais.Where(o => !o.TipoFatura).Sum(o => o.Volumes),
                nrControleCliente1 = "",
                nrControleCliente2 = "",
                nrControleCliente3 = "",
                nrControleCliente7 = "",
                nrControleCliente8 = "",
                nrControleCliente9 = "",
                nrControleCliente10 = "",
                vlCubagem = 0,
                vlPeso = (int)pedido.NotasFiscais.Where(o => !o.TipoFatura).Sum(o => o.Peso),
                cdTransp = 0,
                flTrocaNota = 0,
                cdProgramacaoDestinatario = 0,
                cdCid = cdCid,
                cdProgramacao = 0,
                cdTrocaNota = 0,
                dsNavio = "",
                flRegiao = 0,
                nrDDDFone1 = "",
                nrDDDFone2 = "",
                nrLacreArmador = "",
                nrLacreSIF = "",
                flReentrega = 0,
                vlInicioDiariaAtrasado = 0,
                vlInicioDiariaNoPrazo = 0
            };
        }

        private ServicoOpenTech.sgrDocumentoProdutosSeqV2 ObterDocumentoSequencia(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, decimal _valorDoc)
        {
            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(_unitOfWork);

            int cdCid = repConfiguracaoIntegracaoLocalidade.BuscarPorLocalidade(cargaCTe.CTe.LocalidadeTerminoPrestacao.Codigo)?.CodigoIntegracao ?? 0;

            return new ServicoOpenTech.sgrDocumentoProdutosSeqV2()
            {
                nrDoc = cargaCTe.CTe.Numero.ToString(),
                tpDoc = 2, //Conhecimento de Frete
                valorDoc = _valorDoc,
                tpOperacao = 1, //Transferência
                dtPrevistaSaida = "",
                dsRua = Utilidades.String.Left(cargaCTe.CTe.Destinatario.Endereco, 50),
                nrRua = Utilidades.String.Left(cargaCTe.CTe.Destinatario.Numero, 6),
                complementoRua = Utilidades.String.Left(cargaCTe.CTe.Destinatario.Complemento, 40),
                dsBairro = Utilidades.String.Left(cargaCTe.CTe.Destinatario.Bairro, 50),
                cdCidIBGE = cargaCTe.CTe.Destinatario.Localidade?.CodigoIBGE ?? 0,
                nrCep = cargaCTe.CTe.Destinatario.CEP,
                nrFone1 = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Destinatario.Telefone1),
                nrFone2 = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Destinatario.Telefone2),
                nrCnpjCPFDestinatario = ObterCnpjCPFDestinatarioCTe(cargaCTe),
                nrCnpjCpfDestinatarioSequencia = "",
                Latitude = 0f,
                Longitude = 0f,
                dsNome = Utilidades.String.Left(cargaCTe.CTe.Destinatario.Nome, 50),
                qtVolumes = cargaCTe.NotasFiscais.Where(o => !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes),
                qtPecas = cargaCTe.NotasFiscais.Where(o => !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes),
                nrControleCliente1 = "",
                nrControleCliente2 = "",
                nrControleCliente3 = "",
                nrControleCliente7 = "",
                nrControleCliente8 = "",
                nrControleCliente9 = "",
                nrControleCliente10 = "",
                vlCubagem = 0,
                vlPeso = (int)cargaCTe.NotasFiscais.Where(o => !o.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura).Sum(o => o.PedidoXMLNotaFiscal.Peso),
                cdTransp = 0,
                flTrocaNota = 0,
                cdProgramacaoDestinatario = 0,
                cdCid = cdCid,
                cdProgramacao = 0,
                cdTrocaNota = 0,
                dsNavio = "",
                flRegiao = 0,
                nrDDDFone1 = "",
                nrDDDFone2 = "",
                nrLacreArmador = "",
                nrLacreSIF = "",
                flReentrega = 0,
                vlInicioDiariaAtrasado = 0,
                vlInicioDiariaNoPrazo = 0
            };
        }

        private int ObterCodigoPais(ServicoOpenTech.sgrData retornoLogin, string pais)
        {
            if (string.IsNullOrWhiteSpace(pais))
                return 1;

            using (ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, _configuracaoIntegracaoOpenTech.URLOpenTech, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                Servicos.Log.TratarErro("Consultando países", "Opentech_sgrListaPais");
                ServicoOpenTech.sgrData retornoIntegracao = svcOpenTech.sgrListaPais(retornoLogin.ReturnKey);
                if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null)
                    return MontarDadosObterCodigoPais(retornoIntegracao, pais);

                return 1;
            }
        }

        private int MontarDadosObterCodigoPais(ServicoOpenTech.sgrData retornoIntegracao, string pais)
        {
            try
            {
                XElement xmlData = (XElement)retornoIntegracao.ReturnDataset.Nodes[1].FirstNode;
                IEnumerable<XElement> paisesXml = xmlData.Elements("sgrTB");

                foreach (XElement paisXml in paisesXml)
                {
                    string cdPais = paisXml.Element("CDPAIS")?.Value ?? "1";
                    string dsPais = paisXml.Element("DSPAIS")?.Value ?? "";

                    if (dsPais.ToUpper() == pais.ToUpper())
                    {
                        if (int.TryParse(cdPais, out int codigoPais))
                            return codigoPais;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao processar lista de países: {ex.Message}", "Opentech_sgrListaPais");
            }

            return 1;
        }

        private void ObterConfiguracaoIntegracaoEmpresa(Dominio.Entidades.Empresa empresa)
        {
            if (empresa == null)
                return;

            if (_configuracaoIntegracaoEmpresa != null)
                return;

            Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa repositorioConfiguracaoIntegracaoEmpresa = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa(_unitOfWork);

            _configuracaoIntegracaoEmpresa = repositorioConfiguracaoIntegracaoEmpresa.BuscarPorEmpresa(empresa.Codigo);
        }

        private ServicoOpenTech.sgrData EfetuarLogin<TIntegracao>(ServicoOpenTech.sgrOpentechSoapClient svcOpenTech, ref Servicos.Models.Integracao.InspectorBehavior inspector, ref TIntegracao integracao)
            where TIntegracao : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>
        {
            Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao repToken = new Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao tipoIntegracaoAutenticacao = repToken.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech);

            ServicoOpenTech.sgrData retornoLogin = new ServicoOpenTech.sgrData();

            if (!string.IsNullOrWhiteSpace(tipoIntegracaoAutenticacao?.Token) && tipoIntegracaoAutenticacao.DataExpiracao > DateTime.Now)
            {
                retornoLogin.ReturnKey = tipoIntegracaoAutenticacao.Token;
                retornoLogin.ReturnDescription = "OK";

                return retornoLogin;
            }

            retornoLogin = svcOpenTech.sgrLogin(_configuracaoIntegracaoOpenTech.UsuarioOpenTech, _configuracaoIntegracaoOpenTech.SenhaOpenTech, _configuracaoIntegracaoOpenTech.DominioOpenTech);

            tipoIntegracaoAutenticacao ??= new Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao();

            tipoIntegracaoAutenticacao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech;
            tipoIntegracaoAutenticacao.Token = retornoLogin.ReturnKey;
            tipoIntegracaoAutenticacao.DataExpiracao = DateTime.Now.AddDays(1);

            if (tipoIntegracaoAutenticacao.Codigo > 0)
                repToken.Atualizar(tipoIntegracaoAutenticacao);
            else
                repToken.Inserir(tipoIntegracaoAutenticacao);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            servicoArquivoTransacao.Adicionar(integracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Login - " + retornoLogin.ReturnDescription, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);

            return retornoLogin;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque> ObterListaAutorizacaoEmbarque(Servicos.ServicoOpenTech.ArrayOfXElement dataSet)
        {
            try
            {
                XElement xmlData = (XElement)dataSet.Nodes[1].FirstNode;
                XElement viagemXml = xmlData.Elements("sgrViagem").FirstOrDefault();

                if (viagemXml == null)
                    return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque>();

                Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque autorizacaoEmbarque = new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque();

                autorizacaoEmbarque.Codigo = ObterValorCampoXML<int>(viagemXml, "CDVIAG");
                autorizacaoEmbarque.ControleCarga = ObterValorCampoXML<string>(viagemXml, "NRCONTROLECARGA");
                autorizacaoEmbarque.DataInicio = ObterValorCampoXML<DateTime>(viagemXml, "DTPREVINI");
                autorizacaoEmbarque.Destino = ObterValorCampoXML<string>(viagemXml, "DSCIDDESTINO") + "/" + ObterValorCampoXML<string>(viagemXml, "ESTADODES");
                autorizacaoEmbarque.Embarcador = ObterValorCampoXML<string>(viagemXml, "NMEMBARCADOR");
                autorizacaoEmbarque.Origem = ObterValorCampoXML<string>(viagemXml, "DSCIDORIGEM") + "/" + ObterValorCampoXML<string>(viagemXml, "ESTADOORI");
                autorizacaoEmbarque.RecomendacoesTransporte = ObterValorCampoXML<string>(viagemXml, "DSMSGAE");
                autorizacaoEmbarque.Transportadora = ObterValorCampoXML<string>(viagemXml, "DSNOMETRANSP");
                autorizacaoEmbarque.Validade = ObterValorCampoXML<DateTime>(viagemXml, "DTPREVFIM");
                autorizacaoEmbarque.ValorTotalCarga = ObterValorCampoXML<decimal>(viagemXml, "VLCARGA");

                autorizacaoEmbarque.CPFMotorista1 = ObterValorCampoXML<string>(viagemXml, "NRCPFMOT1");
                autorizacaoEmbarque.NomeMotorista1 = ObterValorCampoXML<string>(viagemXml, "DSMOTORISTA1");
                autorizacaoEmbarque.RGMotorista1 = ObterValorCampoXML<string>(viagemXml, "RGMOT1");

                autorizacaoEmbarque.CPFMotorista2 = ObterValorCampoXML<string>(viagemXml, "NRCPFMOT2");
                autorizacaoEmbarque.NomeMotorista2 = ObterValorCampoXML<string>(viagemXml, "DSMOTORISTA2");
                autorizacaoEmbarque.RGMotorista2 = ObterValorCampoXML<string>(viagemXml, "RGMOT2");

                autorizacaoEmbarque.AnoVeiculo1 = ObterValorCampoXML<int>(viagemXml, "ANOCAVALO");
                autorizacaoEmbarque.CarroceriaVeiculo1 = ObterValorCampoXML<string>(viagemXml, "TPCARCAVALO");
                autorizacaoEmbarque.ChassisVeiculo1 = ObterValorCampoXML<string>(viagemXml, "CHASSICAVALO");
                autorizacaoEmbarque.CorVeiculo1 = ObterValorCampoXML<string>(viagemXml, "CORCAVALO");
                autorizacaoEmbarque.EmpresaRastreamentoVeiculo1 = ObterValorCampoXML<string>(viagemXml, "DSNOMEEMPRASTREA");
                autorizacaoEmbarque.MarcaVeiculo1 = ObterValorCampoXML<string>(viagemXml, "MARCACAVALO") + "/" + ObterValorCampoXML<string>(viagemXml, "MODELOCAVALO");
                autorizacaoEmbarque.PlacaVeiculo1 = ObterValorCampoXML<string>(viagemXml, "NRPLACACAVALO");
                autorizacaoEmbarque.RastreadorVeiculo1 = ObterValorCampoXML<string>(viagemXml, "CDENDRASTR");

                autorizacaoEmbarque.AnoVeiculo2 = ObterValorCampoXML<int>(viagemXml, "ANOCARRETA");
                autorizacaoEmbarque.CarroceriaVeiculo2 = ObterValorCampoXML<string>(viagemXml, "TPCARCARRETA");
                autorizacaoEmbarque.ChassisVeiculo2 = ObterValorCampoXML<string>(viagemXml, "CHASSICARRETA");
                autorizacaoEmbarque.CorVeiculo2 = ObterValorCampoXML<string>(viagemXml, "CORCARRETA");
                autorizacaoEmbarque.EmpresaRastreamentoVeiculo2 = ObterValorCampoXML<string>(viagemXml, "DSNOMEEMPRASTREACARR");
                autorizacaoEmbarque.MarcaVeiculo2 = ObterValorCampoXML<string>(viagemXml, "MARCACARRETA") + "/" + ObterValorCampoXML<string>(viagemXml, "MODELOCARRETA");
                autorizacaoEmbarque.PlacaVeiculo2 = ObterValorCampoXML<string>(viagemXml, "NRPLACACARRETA");
                autorizacaoEmbarque.RastreadorVeiculo2 = ObterValorCampoXML<string>(viagemXml, "CDENDRASTRCARR");

                autorizacaoEmbarque.AnoVeiculo3 = ObterValorCampoXML<int>(viagemXml, "ANOCARRETA2");
                autorizacaoEmbarque.CarroceriaVeiculo3 = ObterValorCampoXML<string>(viagemXml, "TPCARCARRETA2");
                autorizacaoEmbarque.ChassisVeiculo3 = ObterValorCampoXML<string>(viagemXml, "CHASSICARRETA2");
                autorizacaoEmbarque.CorVeiculo3 = ObterValorCampoXML<string>(viagemXml, "CORCARRETA2");
                autorizacaoEmbarque.MarcaVeiculo3 = ObterValorCampoXML<string>(viagemXml, "MARCACARRETA2") + "/" + ObterValorCampoXML<string>(viagemXml, "MODELOCARRETA2");
                autorizacaoEmbarque.PlacaVeiculo3 = ObterValorCampoXML<string>(viagemXml, "NRPLACACARRETA2");

                return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque>() { autorizacaoEmbarque };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao processar autorização de embarque: {ex.Message}", "Opentech_AutorizacaoEmbarque");
                return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque>();
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota> ObterListaTrechos(Servicos.ServicoOpenTech.ArrayOfXElement dataSet, Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque ae)
        {
            try
            {
                XElement xmlData = (XElement)dataSet.Nodes[1].FirstNode;
                IEnumerable<XElement> trechosXml = xmlData.Elements("sgrTrechos");

                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota> trechos = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota>();

                foreach (XElement trechoXml in trechosXml)
                {
                    trechos.Add(new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota()
                    {
                        Codigo = ae.Codigo,
                        Estrada = ObterValorCampoXML<string>(trechoXml, "CDROD"),
                        NomeTrecho = ObterValorCampoXML<string>(trechoXml, "DSNOME"),
                        Sequencial = ObterValorCampoXML<int>(trechoXml, "SEQ")
                    });
                }

                return trechos;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao processar trechos: {ex.Message}", "Opentech_Trechos");
                return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota>();
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento> ObterListaDocumentos(Servicos.ServicoOpenTech.ArrayOfXElement dataSet, Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque ae)
        {
            try
            {
                XElement xmlData = (XElement)dataSet.Nodes[1].FirstNode;
                IEnumerable<XElement> documentosXml = xmlData.Elements("sgrDocumentos");

                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento> documentos = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento>();

                foreach (XElement documentoXml in documentosXml)
                {
                    documentos.Add(new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento()
                    {
                        Codigo = ae.Codigo,
                        Bairro = ObterValorCampoXML<string>(documentoXml, "DSBAIRRO"),
                        CEP = ObterValorCampoXML<string>(documentoXml, "NRCEP"),
                        Cidade = ObterValorCampoXML<string>(documentoXml, "DSNOMECID"),
                        DataPrevistaEntrega = ObterValorCampoXML<DateTime>(documentoXml, "DTPREVISTA"),
                        Endereco = ObterValorCampoXML<string>(documentoXml, "DSRUA"),
                        Nome = ObterValorCampoXML<string>(documentoXml, "DSNOME"),
                        Numero = ObterValorCampoXML<string>(documentoXml, "NRRUA"),
                        NumeroDocumento = ObterValorCampoXML<string>(documentoXml, "NRDOC"),
                        Situacao = ObterValorCampoXML<string>(documentoXml, "DSSITUACAO"),
                        Telefone = "(" + Utilidades.String.OnlyNumbers(ObterValorCampoXML<string>(documentoXml, "NRFONE2")) + ") " + Utilidades.String.OnlyNumbers(ObterValorCampoXML<string>(documentoXml, "NRFONE1")),
                        Valor = ObterValorCampoXML<decimal>(documentoXml, "VLVALOR")
                    });
                }

                return documentos;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao processar documentos: {ex.Message}", "Opentech_Documentos");
                return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento>();
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido> ObterListaLocaisParada(Servicos.ServicoOpenTech.ArrayOfXElement dataSet, Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque ae)
        {
            try
            {
                XElement xmlData = (XElement)dataSet.Nodes[1].FirstNode;
                IEnumerable<XElement> locaisParadaXml = xmlData.Elements("sgrPontosApoio");

                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido> locaisParada = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido>();

                foreach (XElement localParadaXml in locaisParadaXml)
                {
                    locaisParada.Add(new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido()
                    {
                        Codigo = ae.Codigo,
                        Cidade = ObterValorCampoXML<string>(localParadaXml, "DSCIDADE") + "/" + ObterValorCampoXML<string>(localParadaXml, "DSSIGLAUF"),
                        KM = ObterValorCampoXML<decimal>(localParadaXml, "VLDIST"),
                        Nome = ObterValorCampoXML<string>(localParadaXml, "DSFANTASIA"),
                        Sentido = ObterValorCampoXML<string>(localParadaXml, "SENTIDO"),
                        Telefone = "(" + Utilidades.String.OnlyNumbers(ObterValorCampoXML<string>(localParadaXml, "NRDDD")) + ") " + Utilidades.String.OnlyNumbers(ObterValorCampoXML<string>(localParadaXml, "NRFONE"))
                    });
                }

                return locaisParada;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao processar locais de parada: {ex.Message}", "Opentech_LocaisParada");
                return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido>();
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado> ObterListaProdutos(Servicos.ServicoOpenTech.ArrayOfXElement dataSet, Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque ae)
        {
            try
            {
                XElement xmlData = (XElement)dataSet.Nodes[1].FirstNode;
                IEnumerable<XElement> produtosXml = xmlData.Elements("sgrProdutos");

                List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado> produtos = new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado>();

                foreach (XElement produtoXml in produtosXml)
                {
                    produtos.Add(new Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado()
                    {
                        Codigo = ae.Codigo,
                        Identificacao = ObterValorCampoXML<string>(produtoXml, "DSPROD"),
                        Valor = ObterValorCampoXML<decimal>(produtoXml, "VLPROD")
                    });
                }

                return produtos;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao processar produtos: {ex.Message}", "Opentech_Produtos");
                return new List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado>();
            }
        }

        private T ObterValorCampoXML<T>(XElement elemento, string propriedade)
        {
            string valorStr = elemento.Element(propriedade)?.Value;
            Type tipo = typeof(T);

            if (string.IsNullOrEmpty(valorStr))
            {
                if (tipo == typeof(string))
                    return (T)Convert.ChangeType("", typeof(T));
                else if (tipo == typeof(int) || tipo == typeof(decimal) || tipo == typeof(float) || tipo == typeof(double))
                    return (T)Convert.ChangeType(0, typeof(T));
                else if (tipo == typeof(DateTime))
                    return (T)Convert.ChangeType(DateTime.MinValue, typeof(T));
            }

            try
            {
                if (tipo == typeof(DateTime))
                {
                    if (DateTime.TryParse(valorStr, out DateTime dataResultado))
                        return (T)Convert.ChangeType(dataResultado, typeof(T));
                    return (T)Convert.ChangeType(DateTime.MinValue, typeof(T));
                }

                return (T)Convert.ChangeType(valorStr, typeof(T));
            }
            catch
            {
                if (tipo == typeof(string))
                    return (T)Convert.ChangeType("", typeof(T));
                else if (tipo == typeof(int) || tipo == typeof(decimal) || tipo == typeof(float) || tipo == typeof(double))
                    return (T)Convert.ChangeType(0, typeof(T));
                else if (tipo == typeof(DateTime))
                    return (T)Convert.ChangeType(DateTime.MinValue, typeof(T));

                return default(T);
            }
        }

        private bool IntegrarCadastros(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, ServicoOpenTech.sgrData retornoLogin)
        {
            bool motorista = EnviarMotorista(cargaIntegracao, retornoLogin);
            if (!motorista)
                return false;

            bool veiculo = EnviarVeiculo(cargaIntegracao, retornoLogin);
            if (!veiculo)
                return false;

            return true;
        }

        private bool IntegrarCadastros(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, ServicoOpenTech.sgrData retornoLogin)
        {
            bool motorista = EnviarMotorista(cargaIntegracao, retornoLogin);
            if (!motorista)
                return false;

            bool veiculo = EnviarVeiculo(cargaIntegracao, retornoLogin);
            if (!veiculo)
                return false;

            return true;
        }

        private void AtualizarSituacaoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem, string protocolo, Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            cargaIntegracao.NumeroTentativas++;
            cargaIntegracao.SituacaoIntegracao = situacao;
            cargaIntegracao.ProblemaIntegracao = mensagem;
            cargaIntegracao.Protocolo = protocolo;
            if (arquivoIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private int ObterCodigoEstadoOpenTech(string siglaUF)
        {

            switch (siglaUF)
            {
                case "AC":
                    return 2;
                case "AL":
                    return 3;
                case "AM":
                    return 5;
                case "AP":
                    return 4;
                case "AR":
                    return 30;
                case "BA":
                    return 6;
                case "BO":
                    return 31;
                case "CB":
                    return 49;
                case "CE":
                    return 7;
                case "CH":
                    return 29;
                case "CL":
                    return 51;
                case "CO":
                    return 43;
                case "DF":
                    return 8;
                case "EP":
                    return 36;
                case "EQ":
                    return 50;
                case "ES":
                    return 9;
                case "GF":
                    return 55;
                case "GO":
                    return 10;
                case "GU":
                    return 54;
                case "HT":
                    return 44;
                case "IM":
                    return 42;
                case "IT":
                    return 39;
                case "JP":
                    return 38;
                case "MA":
                    return 11;
                case "MG":
                    return 14;
                case "MS":
                    return 13;
                case "MT":
                    return 12;
                case "MX":
                    return 46;
                case "PA":
                    return 15;
                case "PB":
                    return 16;
                case "PE":
                    return 18;
                case "PI":
                    return 19;
                case "PR":
                    return 17;
                case "PT":
                    return 41;
                case "PU":
                    return 32;
                case "PY":
                    return 34;
                case "RJ":
                    return 20;
                case "RN":
                    return 21;
                case "RO":
                    return 23;
                case "RR":
                    return 24;
                case "RS":
                    return 22;
                case "SC":
                    return 1;
                case "SE":
                    return 26;
                case "SP":
                    return 28;
                case "SU":
                    return 53;
                case "TO":
                    return 27;
                case "TT":
                    return 56;
                case "UR":
                    return 33;
                case "VE":
                    return 47;
                case "VZ":
                    return 52;
                case "XX":
                    return 35;
                default:
                    return 48;  //Vazio
            }
        }

        private void NotificarPorEmail(Dominio.Entidades.Embarcador.Cargas.Carga carga, string mensagem)
        {
            try
            {
                if (_configuracaoIntegracaoOpenTech.NotificarFalhaIntegracaoOpentech)
                {
                    List<string> listaEmails = new List<string>();

                    listaEmails.AddRange(_configuracaoIntegracaoOpenTech.EmailsNotificacaoFalhaIntegracaoOpentech.Split(';').ToList());
                    listaEmails = listaEmails.Distinct().ToList();

                    if (listaEmails.Count > 0)
                    {
                        Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
                        Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                        if (configEmail != null)
                        {
                            string mensagemErro = string.Empty;

                            string assunto = "Rejeição integração Opentech";
                            if (carga != null)
                                assunto += " carga " + carga.CodigoCargaEmbarcador;
                            string mensagemEmail = "Integração com a Opentech retornou: ";
                            mensagemEmail += "<br/>";
                            mensagemEmail += mensagem;

                            if (carga != null)
                            {
                                mensagemEmail += "Carga: " + carga.CodigoCargaEmbarcador;
                                mensagemEmail += "<br/>";
                                mensagemEmail += "Tipo de Operação: " + carga.TipoOperacao?.Descricao ?? string.Empty;
                                mensagemEmail += "<br/>";
                                mensagemEmail += "Transportador: " + carga.Empresa?.Descricao ?? string.Empty;
                                mensagemEmail += "<br/>";
                                mensagemEmail += "Veiculo: " + carga.Veiculo?.Descricao ?? string.Empty;
                                mensagemEmail += "<br/>";
                                mensagemEmail += "Motorista: " + (carga.Motoristas?.Count > 0 ? carga.Motoristas.FirstOrDefault().Descricao : string.Empty);
                            }

                            mensagemEmail += "<br/>";
                            mensagemEmail += "<br/>";
                            mensagemEmail += " *** E-MAIL ENVIADO AUTOMATICAMENTE PELO NOSSO SISTEMA. FAVOR NÃO RESPONDER ***";

                            bool sucesso = Servicos.Email.EnviarEmail(configEmail.Email, configEmail.Email, configEmail.Senha, null, listaEmails.ToArray(), null, assunto, mensagemEmail, configEmail.Smtp, out mensagemErro, configEmail.DisplayEmail, null, "", configEmail.RequerAutenticacaoSmtp, "", configEmail.PortaSmtp, _unitOfWork);

                            if (!sucesso)
                                throw new Exception("Email de notificação Opentech não enviado: " + mensagemErro);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void PreencherCamposConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, ref string dtprevini, ref string ratreadorcavalor, ref int cdemprastrcavalo, ref string cdrota, ref string nrfonecel, ref string nrfrota, ref decimal valorDoc, decimal valorCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedidos = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            int codigoCarga = cargaIntegracao != null ? cargaIntegracao.Carga.Codigo : cargaDadosTransporteIntegracao.Carga.Codigo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedidos.BuscarPorCarga(codigoCarga);

            if (cargaIntegracao != null)
            {
                if (_configuracaoIntegracaoOpenTech.EnviarDataPrevisaoSaidaPedidoOpentech)
                {
                    DateTime? dataPrevisaoSaida = cargaPedidos.Where(o => o.Pedido.DataPrevisaoSaida.HasValue).Max(o => o.Pedido.DataPrevisaoSaida);

                    if (dataPrevisaoSaida.HasValue)
                        dtprevini = dataPrevisaoSaida.Value.ToString("yyyy-MM-dd HH:mm");
                }

                if (_configuracaoIntegracaoOpenTech.EnviarInformacoesRastreadorCavaloOpentech)
                {
                    if (cargaIntegracao.Carga.Veiculo?.PossuiRastreador ?? false)
                    {
                        int codigoIntegracao = 0;
                        int.TryParse(cargaIntegracao.Carga.Veiculo?.TecnologiaRastreador?.CodigoIntegracao ?? string.Empty, out codigoIntegracao);

                        ratreadorcavalor = (cargaIntegracao.Carga.Veiculo?.NumeroEquipamentoRastreador ?? string.Empty);
                        cdemprastrcavalo = codigoIntegracao;
                    }
                }

                if (_configuracaoIntegracaoOpenTech.EnviarCodigoIntegracaoRotaCargaOpenTech)
                {
                    cdrota = cargaIntegracao.Carga.Rota?.CodigoIntegracao ?? string.Empty;
                }

                if (_configuracaoIntegracaoOpenTech.EnviarNrfonecelBrancoOpenTech)
                {
                    nrfonecel = "";
                }

                if (_configuracaoIntegracaoOpenTech.EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech)
                {
                    if (string.IsNullOrEmpty(nrfrota))
                    {
                        nrfrota = cargaIntegracao.Carga.Veiculo?.Placa ?? string.Empty;
                    }

                }

                if (_configuracaoIntegracaoOpenTech.EnviarValorNotasValorDocOpenTech)
                {
                    valorDoc = valorCarga;
                }

            }
            else
            {
                if (_configuracaoIntegracaoOpenTech.EnviarDataPrevisaoSaidaPedidoOpentech)
                {
                    DateTime? dataPrevisaoSaida = cargaPedidos.Where(o => o.Pedido.DataPrevisaoSaida.HasValue).Max(o => o.Pedido.DataPrevisaoSaida);

                    if (dataPrevisaoSaida.HasValue)
                        dtprevini = dataPrevisaoSaida.Value.ToString("yyyy-MM-dd HH:mm");
                }

                if (_configuracaoIntegracaoOpenTech.EnviarInformacoesRastreadorCavaloOpentech)
                {
                    if (cargaDadosTransporteIntegracao.Carga.Veiculo?.PossuiRastreador ?? false)
                    {
                        int codigoIntegracao = 0;
                        int.TryParse(cargaDadosTransporteIntegracao.Carga.Veiculo?.CodigoIntegracao ?? string.Empty, out codigoIntegracao);

                        ratreadorcavalor = (cargaDadosTransporteIntegracao.Carga.Veiculo?.NumeroEquipamentoRastreador ?? string.Empty);
                        cdemprastrcavalo = codigoIntegracao;
                    }
                }

                if (_configuracaoIntegracaoOpenTech.EnviarCodigoIntegracaoRotaCargaOpenTech)
                {
                    cdrota = cargaDadosTransporteIntegracao.Carga.Rota?.CodigoIntegracao ?? string.Empty;
                }

                if (_configuracaoIntegracaoOpenTech.EnviarNrfonecelBrancoOpenTech)
                {
                    nrfonecel = "";
                }

                if (_configuracaoIntegracaoOpenTech.EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech)
                {
                    if (string.IsNullOrEmpty(nrfrota.Trim()))
                    {
                        nrfrota = cargaDadosTransporteIntegracao.Carga.Veiculo?.Placa ?? string.Empty;
                    }

                }

                if (_configuracaoIntegracaoOpenTech.EnviarValorNotasValorDocOpenTech)
                {
                    valorDoc = valorCarga;
                }

            }

        }

        private string ObterCnpjCPFDestinatario(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            string nrCnpjCPFDestinatario = string.Empty;

            if (cargaPedido.Pedido.Recebedor != null)
            {
                nrCnpjCPFDestinatario = cargaPedido.Pedido.Recebedor.Localidade?.Estado?.Sigla == "EX"
                    ? cargaPedido.Pedido.Recebedor.CodigoIntegracao
                    : cargaPedido.Pedido.Recebedor.CPF_CNPJ_SemFormato;
            }
            else if (cargaPedido.Pedido.Destinatario != null)
            {
                nrCnpjCPFDestinatario = cargaPedido.Pedido.Destinatario.Localidade?.Estado?.Sigla == "EX"
                    ? cargaPedido.Pedido.Destinatario.CodigoIntegracao
                    : cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato;
            }

            return nrCnpjCPFDestinatario;
        }

        private string ObterCnpjCPFDestinatarioCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            string nrCnpjCPFDestinatario = string.Empty;

            if (cargaCTe.CTe.Recebedor != null)
            {
                nrCnpjCPFDestinatario = cargaCTe.CTe.Recebedor.Localidade?.Estado?.Sigla == "EX"
                    ? cargaCTe.CTe.Recebedor.CodigoIntegracao
                    : cargaCTe.CTe.Recebedor.CPF_CNPJ_SemFormato;
            }
            else if (cargaCTe.CTe.Destinatario != null)
            {
                nrCnpjCPFDestinatario = cargaCTe.CTe.Destinatario.Localidade?.Estado?.Sigla == "EX"
                    ? cargaCTe.CTe.Destinatario.CodigoIntegracao
                    : cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato;
            }

            return nrCnpjCPFDestinatario;
        }

        private int ObterCodigoTransportadorOpenTech(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, ServicoOpenTech.sgrOpentechSoapClient svcOpenTech, Servicos.Models.Integracao.InspectorBehavior inspector, ServicoOpenTech.sgrData retornoLogin = null, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao = null)
        {
            int.TryParse(cargaPedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoIntegracao ?? "0", out int cdTrans);

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.HabilitarOutraConfiguracaoOpenTech)
                cdTrans = carga?.TipoOperacao?.CodigoTransportadorOpenTech ?? 0;

            if (_configuracaoIntegracaoOpenTech.EnviarCodigoIntegracaoCentroCustoCargaOpenTech && !(carga?.TipoOperacao?.HabilitarOutraConfiguracaoOpenTech ?? false))
                cdTrans = cargaPedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoTransportadorOpenTech ?? 0;

            if (cdTrans == 0)
                int.TryParse(_configuracaoIntegracaoEmpresa?.CodigoIntegracao ?? "0", out cdTrans);

            if (cdTrans == 0 && retornoLogin != null)
            {
                ServicoOpenTech.sgrData retornoIntegracao = svcOpenTech.sgrListaTransportadoras(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech);

                if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null)
                {
                    int s = retornoIntegracao.ReturnDataset.Nodes.Count;
                    for (int i = 0; i < s; i++)
                    {
                        XElement root = (XElement)retornoIntegracao.ReturnDataset.Nodes[i].FirstNode;
                        XElement transportadoraElement = root.Descendants("sgrTB").FirstOrDefault(x => (string)x.Element("nrCGCCPF") == carga.Empresa.CNPJ);

                        if (transportadoraElement != null)
                        {
                            int.TryParse((string)transportadoraElement.Element("cdTransp"), out cdTrans);
                            break;
                        }
                    }
                }

                if (cargaIntegracao != null)
                {
                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", $"Integração - {retornoIntegracao?.ReturnDescription ?? ""} ");
                }
            }

            return cdTrans;
        }

        private void ObterConfiguracaoIntegracaoOpenTech(Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            if (_configuracaoIntegracaoOpenTech != null)
                return;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech configuracaoGeralIntegracaoOpenTech = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech(_unitOfWork).Buscar();

            _configuracaoIntegracaoOpenTech = new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoIntegracaoOpenTech();

            _configuracaoIntegracaoOpenTech.CadastrarRotaCargaOpentech = configuracaoIntegracao?.CadastrarRotaCargaOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarCodigoIntegracaoCentroCustoCargaOpenTech = configuracaoIntegracao?.EnviarCodigoIntegracaoCentroCustoCargaOpenTech ?? false;
            _configuracaoIntegracaoOpenTech.ValorBaseOpenTech = configuracaoIntegracao?.ValorBaseOpenTech ?? 0m;
            _configuracaoIntegracaoOpenTech.EnviarCodigoEmbarcadorProdutoOpentech = configuracaoIntegracao?.EnviarCodigoEmbarcadorProdutoOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarDataPrevisaoEntregaDataCarregamentoOpentech = configuracaoIntegracao?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarDataAtualNaDataPrevisaoOpentech = configuracaoIntegracao?.EnviarDataAtualNaDataPrevisaoOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarDataPrevisaoSaidaPedidoOpentech = configuracaoIntegracao?.EnviarDataPrevisaoSaidaPedidoOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarInformacoesRastreadorCavaloOpentech = configuracaoIntegracao?.EnviarInformacoesRastreadorCavaloOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarCodigoIntegracaoRotaCargaOpenTech = configuracaoIntegracao?.EnviarCodigoIntegracaoRotaCargaOpenTech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarNrfonecelBrancoOpenTech = configuracaoIntegracao?.EnviarNrfonecelBrancoOpenTech ?? false;
            _configuracaoIntegracaoOpenTech.EnviarValorNotasValorDocOpenTech = configuracaoIntegracao?.EnviarValorNotasValorDocOpenTech ?? false;
            _configuracaoIntegracaoOpenTech.IntegrarRotaCargaOpentech = configuracaoIntegracao?.IntegrarRotaCargaOpentech ?? false;
            _configuracaoIntegracaoOpenTech.CalcularPrevisaoEntregaComBaseDistanciaOpentech = configuracaoIntegracao?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false;
            _configuracaoIntegracaoOpenTech.NotificarFalhaIntegracaoOpentech = configuracaoIntegracao?.NotificarFalhaIntegracaoOpentech ?? false;
            _configuracaoIntegracaoOpenTech.EmailsNotificacaoFalhaIntegracaoOpentech = configuracaoIntegracao?.EmailsNotificacaoFalhaIntegracaoOpentech ?? "";
            _configuracaoIntegracaoOpenTech.IntegrarCargaOpenTechV10 = configuracaoIntegracao?.IntegrarCargaOpenTechV10 ?? false;
            _configuracaoIntegracaoOpenTech.IntegrarVeiculoMotorista = configuracaoIntegracao?.IntegrarVeiculoMotorista ?? false;
            _configuracaoIntegracaoOpenTech.EnviarValorDasNotasNoCampoValorDoc = configuracaoIntegracao?.EnviarValorDasNotasNoCampoValorDoc ?? false;
            _configuracaoIntegracaoOpenTech.CodigoProdutoColetaOpentech = configuracaoIntegracao?.CodigoProdutoColetaOpentech ?? 0;
            _configuracaoIntegracaoOpenTech.CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador = configuracaoIntegracao?.CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador ?? 0;
            _configuracaoIntegracaoOpenTech.CodigoProdutoVeiculoComLocalizadorOpenTech = configuracaoIntegracao?.CodigoProdutoVeiculoComLocalizadorOpenTech ?? 0;
            _configuracaoIntegracaoOpenTech.CodigoProdutoColetaEmbarcadorOpentech = configuracaoIntegracao?.CodigoProdutoColetaEmbarcadorOpentech ?? 0;
            _configuracaoIntegracaoOpenTech.CodigoProdutoColetaTransportadorOpentech = configuracaoIntegracao?.CodigoProdutoColetaTransportadorOpentech ?? 0;
            _configuracaoIntegracaoOpenTech.AtualizarVeiculoMotoristaOpentech = configuracaoIntegracao?.AtualizarVeiculoMotoristaOpentech ?? false;

            _configuracaoIntegracaoOpenTech.ConsiderarLocalidadeProdutoIntegracaoEntrega = configuracaoGeralIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false;
            _configuracaoIntegracaoOpenTech.EnviarDataNFeNaDataPrevistaOpentech = configuracaoGeralIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.HabilitarOutraConfiguracaoOpenTech)
            {
                _configuracaoIntegracaoOpenTech.UsuarioOpenTech = carga.TipoOperacao.UsuarioOpenTech;
                _configuracaoIntegracaoOpenTech.SenhaOpenTech = carga.TipoOperacao.SenhaOpenTech;
                _configuracaoIntegracaoOpenTech.DominioOpenTech = carga.TipoOperacao.DominioOpenTech;
                _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech = carga.TipoOperacao.CodigoClienteOpenTech;
                _configuracaoIntegracaoOpenTech.CodigoPASOpenTech = carga.TipoOperacao.CodigoPASOpenTech;
                _configuracaoIntegracaoOpenTech.URLOpenTech = carga.TipoOperacao.URLOpenTech;
                _configuracaoIntegracaoOpenTech.CodigoProdutoPadraoOpentech = carga.TipoOperacao.CodigoProdutoPadraoOpentech;

                return;
            }

            _configuracaoIntegracaoOpenTech.UsuarioOpenTech = configuracaoIntegracao?.UsuarioOpenTech ?? "";
            _configuracaoIntegracaoOpenTech.SenhaOpenTech = configuracaoIntegracao?.SenhaOpenTech ?? "";
            _configuracaoIntegracaoOpenTech.DominioOpenTech = configuracaoIntegracao?.DominioOpenTech ?? "";
            _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech = configuracaoIntegracao?.CodigoClienteOpenTech ?? 0;
            _configuracaoIntegracaoOpenTech.CodigoPASOpenTech = configuracaoIntegracao?.CodigoPASOpenTech ?? 0;
            _configuracaoIntegracaoOpenTech.URLOpenTech = configuracaoIntegracao?.URLOpenTech ?? "";
            _configuracaoIntegracaoOpenTech.CodigoProdutoPadraoOpentech = configuracaoIntegracao?.CodigoProdutoPadraoOpentech ?? 0;
        }

        private int ObterCodigoClienteOpenTech(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.HabilitarOutraConfiguracaoOpenTech)
                return carga.TipoOperacao.CodigoClienteOpenTech;

            if (_configuracaoIntegracaoEmpresa != null && _configuracaoIntegracaoEmpresa?.CodigoClienteOpenTech > 0)
                return _configuracaoIntegracaoEmpresa.CodigoClienteOpenTech;

            return _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech;
        }

        private int ObterCodigoPASOpenTech(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.HabilitarOutraConfiguracaoOpenTech)
                return carga.TipoOperacao.CodigoPASOpenTech;

            if (_configuracaoIntegracaoEmpresa != null && _configuracaoIntegracaoEmpresa?.CodigoPASOpenTech > 0)
                return _configuracaoIntegracaoEmpresa.CodigoPASOpenTech;

            return _configuracaoIntegracaoOpenTech.CodigoPASOpenTech;
        }

        private ServicoOpenTech.sgrData TrocarMotoristaAe(ServicoOpenTech.sgrOpentechSoapClient svcOpenTech, ServicoOpenTech.sgrData retornoLogin, string protocolo, string cpfMotoristaNovo, string cpfMotoristaAnterior)
        {
            ServicoOpenTech.sgrData retornoIntegracao = null;

            try
            {
                int.TryParse(protocolo, out int cdViagem);
                retornoIntegracao = svcOpenTech.sgrTrocarMotoristaAE(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, cdViagem, cpfMotoristaAnterior, cpfMotoristaNovo, 0);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return retornoIntegracao;
        }

        private ServicoOpenTech.sgrData TrocarCavaloAe(ServicoOpenTech.sgrOpentechSoapClient svcOpenTech, ServicoOpenTech.sgrData retornoLogin, string protocolo, string placaTracaoNova, string placaTracaoAnterior)
        {
            ServicoOpenTech.sgrData retornoIntegracao = null;

            try
            {
                int.TryParse(protocolo, out int cdViagem);
                retornoIntegracao = svcOpenTech.sgrTrocarCavaloAE(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, cdViagem, placaTracaoAnterior, placaTracaoNova, 0);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return retornoIntegracao;
        }

        private ServicoOpenTech.sgrData TrocarCarretaAe(ServicoOpenTech.sgrOpentechSoapClient svcOpenTech, ServicoOpenTech.sgrData retornoLogin, string protocolo, string placaCarretaNova, string placaCarretaAnterior)
        {
            ServicoOpenTech.sgrData retornoIntegracao = null;

            try
            {
                int.TryParse(protocolo, out int cdViagem);
                retornoIntegracao = svcOpenTech.sgrTrocarCarretaAE(retornoLogin.ReturnKey, _configuracaoIntegracaoOpenTech.CodigoPASOpenTech, _configuracaoIntegracaoOpenTech.CodigoClienteOpenTech, cdViagem, placaCarretaAnterior, placaCarretaNova, 0);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return retornoIntegracao;
        }

        private void TratarRetornoIntegracao(ref StringBuilder mensagemErros, ServicoOpenTech.sgrData retornoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string descricaoOperacao)
        {
            if (retornoIntegracao != null && retornoIntegracao.ReturnDataset != null && retornoIntegracao.ReturnDescription == "OK")
                return;

            string retorno = retornoIntegracao?.ReturnDescription ?? "Falha ao integrar";

            retorno += ObterMensagemErro(retornoIntegracao);

            if (!string.IsNullOrWhiteSpace(cargaDadosTransporteIntegracao.Protocolo) && retorno.Contains("Existe uma viagem em andamento") && retorno.Contains(cargaDadosTransporteIntegracao.Protocolo))
                return;

            mensagemErros.AppendLine($"{descricaoOperacao}: {Utilidades.String.Left(retorno, 200)}");
        }

        private int ObterCodigoProdutoOpenTech(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoIntegracao?.ValidarSomenteVeiculoMotoristaOpentech ?? false))
                return _configuracaoIntegracaoOpenTech.CodigoProdutoColetaOpentech;

            if ((carga.Veiculo?.ModeloVeicularCarga?.ModeloVeicularAceitaLocalizador ?? false) && (carga.Veiculo?.PossuiLocalizador ?? false))
                return _configuracaoIntegracaoOpenTech.CodigoProdutoVeiculoComLocalizadorOpenTech;

            if (carga.Veiculo?.PossuiRastreador ?? false)
                return _configuracaoIntegracaoOpenTech.CodigoProdutoColetaOpentech;

            return _configuracaoIntegracaoOpenTech.CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador;
        }

        private string ObterMensagemErro(ServicoOpenTech.sgrData retornoIntegracao)
        {
            string mensagemErro = "";

            if (retornoIntegracao?.ReturnDataset != null)
                for (int i = 0; i < retornoIntegracao.ReturnDataset.Nodes.Count; i++)
                    mensagemErro += !string.IsNullOrWhiteSpace(retornoIntegracao.ReturnDataset.Nodes[i].Value) ? " / " + retornoIntegracao.ReturnDataset.Nodes[i].Value : "";

            return mensagemErro;
        }

        #endregion Métodos Privados
    }
}