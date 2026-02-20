using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize(new string[] { "BuscarCTeCarga" }, "Cargas/Carga")]
    public class CargaComponenteFreteController : BaseController
    {
		#region Construtores

		public CargaComponenteFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarAsync(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AdicionarComponentes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoComponenteFrete = Request.GetIntParam("ComponenteFrete");
                int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");

                bool cobrarOutroDocumento = Request.GetBoolParam("CobrarOutroDocumento");

                decimal valorComponente = Request.GetDecimalParam("ValorComponente");
                decimal percentual = Request.GetDecimalParam("Percentual");
                decimal valorSugerido = Request.GetDecimalParam("ValorSugerido");
                decimal valorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda");

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork, cancellationToken);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork,cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork,cancellationToken);

                Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga.TabelaFrete != null &&
                    carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga &&
                    carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                    return new JsonpResult(false, true, "Não é permitido adicionar um componente quando o frete é calculado por pedido ou documento emitido.");

                if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoPermitirAlterarValorFreteNaCarga)
                    return new JsonpResult(false, true, "O tipo de operação desta carga não permite a alteração do valor da mesma.");

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                unitOfWork.Start();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || !carga.PossuiPendencia))
                {
                    if (await serCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, TipoServicoMultisoftware))
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = await repComponenteFrete.BuscarPorCodigoAsync(codigoComponenteFrete);

                        if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS || componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS)
                        {
                            await unitOfWork.RollbackAsync();
                            return new JsonpResult(false, true, "Não é possível adicionar um componente de imposto.");
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = await repCargaComponentesFrete.BuscarPorCargaPorCompomenteAsync(carga.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);

                        if (cargaComponentesFrete.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia))
                        {
                            await unitOfWork.RollbackAsync();
                            return new JsonpResult(false, true, "Não é possível adicionar o componente pois existe um componente do mesmo tipo adicionado à partir de uma ocorrência.");
                        }

                        if (cargaComponentesFrete.Count > 0)
                        {
                            await unitOfWork.RollbackAsync();
                            return new JsonpResult(false, true, "Já existe um componente (" + componenteFrete.Descricao + ") cadastrado para esta carga.");
                        }

                        decimal cotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

                        if (cobrarOutroDocumento)
                            modeloDocumentoFiscal = await repModeloDocumentoFiscal.BuscarPorIdAsync(codigoModeloDocumentoFiscal);

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                        if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM || componenteFrete.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
                        {
                            decimal valorNotas = 0;

                            var lstTipoContratacaoCargaDocAnterior = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga>() {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro
                                , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                                , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                                , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                                , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario
                            };

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                            {
                                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    valorNotas += await repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedidoAsync(cargaPedido.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                                else if (lstTipoContratacaoCargaDocAnterior.Contains(cargaPedido.TipoContratacaoCarga))
                                    valorNotas += await repPedidoCTeParaSubContratacao.BuscarTotalPorCargaPedidoAsync(cargaPedido.Codigo);
                            }

                            tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                            if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real && cotacaoMoeda > 0m)
                            {
                                valorTotalMoeda = Math.Round(valorNotas * percentual / 100 / cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                                valorComponente = Math.Round(valorTotalMoeda * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                valorComponente = valorNotas * (percentual / 100);
                            }
                        }
                        else
                        {
                            if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                                valorComponente = Math.Round(valorTotalMoeda * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                        }

                        if (componenteFrete.DescontarValorTotalAReceber)
                            valorComponente = -valorComponente;

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de " + componenteFrete.Descricao + " Informado Pelo Operador", " Valor Informado = " + valorComponente.ToString("n2"), valorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Valor de " + componenteFrete.Descricao + " informado pelo Operador", componenteFrete.Codigo, valorComponente);
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unitOfWork, null);

                        serComponetesFrete.AdicionarComponenteFreteCarga(carga, componenteFrete, valorComponente, percentual, false, tipoValor, componenteFrete.TipoComponenteFrete, null, true, false, modeloDocumentoFiscal, TipoServicoMultisoftware, this.Usuario, unitOfWork, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, false, false, null, false, moeda, cotacaoMoeda, valorTotalMoeda);
                        carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
                        await repCarga.AtualizarAsync(carga);

                        if (carga.FreteDeTerceiro && ConfiguracaoEmbarcador.TipoContratoFreteTerceiro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro.PorCarga)
                        {
                            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiro = await repContratoFreteTerceiro.BuscarPorCargaAsync(carga.Codigo);
                            serFreteSubcontratacaoTerceiro.AtualizarValorComponentesObrigatoriosParaTerceiro(carga, ref contratoFreteTerceiro, unitOfWork);
                        }

                        serCargaFrete.ProcessarRegraInclusaoICMSComponenteFrete(carga, carga.Pedidos.ToList(), ConfiguracaoEmbarcador, unitOfWork);
                        serCargaFrete.CalcularValorFreteComICMSIncluso(carga, unitOfWork);

                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Adicionou um componente de frete " + componenteFrete.Descricao + (valorSugerido > 0m ? " (Valor sugerido " + valorSugerido.ToString("n2") + ")" : "") + ".", unitOfWork);

                        await unitOfWork.CommitChangesAsync();

                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serCargaFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido);

                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        return new JsonpResult(false, true, "Não é possível adicionar um componente na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");
                    }
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, "Ação não permitida.");
                }
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AtualizarAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AdicionarComponentes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork,cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork,cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork, cancellationToken);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork,cancellationToken);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork,cancellationToken);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(int.Parse(Request.Params("Carga")));
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();

                if (carga.TabelaFrete != null &&
                    carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga &&
                    carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                    return new JsonpResult(false, true, "Não é permitido atualizar um componente quando o frete é calculado por pedido ou por documento emitido.");

                if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoPermitirAlterarValorFreteNaCarga)
                    return new JsonpResult(false, true, "O tipo de operação desta carga não permite a alteração do valor da mesma.");

                unitOfWork.Start();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || !carga.PossuiPendencia))
                {
                    if (await serCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, TipoServicoMultisoftware))
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = await repComponenteFrete.BuscarPorCodigoAsync(int.Parse(Request.Params("ComponenteFrete")));

                        if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = await repCargaComponenteFrete.BuscarPorCodigoAsync(codigo, carga.Codigo);

                            cargaComponenteFrete.ComponenteFrete = componenteFrete;
                            cargaComponenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;

                            await repCargaComponenteFrete.AtualizarAsync(cargaComponenteFrete);
                        }
                        else
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = await repCargaComponenteFrete.BuscarPorCargaPorCompomenteAsync(carga.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);

                            if (cargaComponentesFrete.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia))
                            {
                                await unitOfWork.RollbackAsync();
                                return new JsonpResult(false, true, "Não é possível atualizar o componente pois existe um componente do mesmo tipo adicionado à partir de uma ocorrência.");
                            }

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
                            if (bool.Parse(Request.Params("CobrarOutroDocumento")))
                                modeloDocumentoFiscal = await repModeloDocumentoFiscal.BuscarPorIdAsync(int.Parse(Request.Params("ModeloDocumentoFiscal")));

                            bool incluirBCICMS = cargaComponentesFrete.FirstOrDefault()?.IncluirBaseCalculoICMS ?? true;
                            bool incluirIntegralmenteContratoFreteTerceiro = cargaComponentesFrete.FirstOrDefault()?.IncluirIntegralmenteContratoFreteTerceiro ?? true;
                            decimal valorComponente = decimal.Parse(Request.Params("ValorComponente"));
                            decimal percentual = decimal.Parse(Request.Params("Percentual"));
                            decimal valorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda");

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
                            decimal cotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;

                            if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM || componenteFrete.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
                            {
                                decimal valorNotas = 0;

                                var lstTipoContratacaoCargaDocAnterior = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga>() {
                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro
                                    , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                                    , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                                    , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                                    , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario
                                };

                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                                {
                                    if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                        valorNotas += await repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedidoAsync(cargaPedido.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                                    if (lstTipoContratacaoCargaDocAnterior.Contains(cargaPedido.TipoContratacaoCarga))
                                        valorNotas += await repPedidoCTeParaSubContratacao.BuscarTotalPorCargaPedidoAsync(cargaPedido.Codigo);
                                }

                                tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real && cotacaoMoeda > 0m)
                                {
                                    valorTotalMoeda = Math.Round(valorNotas * percentual / 100 / cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                                    valorComponente = Math.Round(valorTotalMoeda * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    valorComponente = valorNotas * (percentual / 100);
                                }
                            }
                            else
                            {
                                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                                    valorComponente = Math.Round(valorTotalMoeda * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                            }

                            if (componenteFrete.DescontarValorTotalAReceber)
                                valorComponente = -valorComponente;

                            string erro = string.Empty;

                            if (!await ExcluirComponenteAsync(erro, carga, componenteFrete, unitOfWork, cancellationToken))
                            {
                                await unitOfWork.RollbackAsync();
                                return new JsonpResult(false, true, erro);
                            }

                            Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao = await repCargaComposicaoFrete.BuscarPorCodigoComponenteAsync(componenteFrete.Codigo, carga.Codigo);

                            if (cargaComposicao != null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de " + componenteFrete.Descricao + " informado pelo operador.", " Valor Informado = " + valorComponente.ToString("n2"), valorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "Valor de " + componenteFrete.Descricao + " informado pelo Operador", componenteFrete.Codigo, valorComponente);
                                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.AtualizarComposicaoFrete(carga, cargaComposicao, null, null, null, false, composicaoFrete, unitOfWork);
                            }

                            await serComponetesFrete.AdicionarComponenteFreteCargaAsync(carga, componenteFrete, valorComponente, percentual, false, tipoValor, componenteFrete.TipoComponenteFrete, null, incluirBCICMS, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, TipoServicoMultisoftware, this.Usuario, unitOfWork, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, false, false, null, false, moeda, cotacaoMoeda, valorTotalMoeda);
                            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
                            await repCarga.AtualizarAsync(carga);
                        }

                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Atualizou o componente de frete " + componenteFrete.Descricao + ".", unitOfWork);
                        await unitOfWork.CommitChangesAsync();
                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serCargaFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido);
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        return new JsonpResult(false, true, "Não é possível atualizar um componente na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");
                    }
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, "Ação não permitida.");
                }
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigoAsync(CancellationToken cancellationToken)
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AdicionarComponentes))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.RateioFrete serCargaFreteRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(int.Parse(Request.Params("Carga")));

                if (carga.TabelaFrete != null && carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                    return new JsonpResult(false, true, "Não é permitido remover um componente quando o frete é calculado por pedido.");

                await unitOfWork.StartAsync();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || !carga.PossuiPendencia))
                {
                    if (await serCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, TipoServicoMultisoftware))
                    {
                        string erro = string.Empty;
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(int.Parse(Request.Params("ComponenteFrete")));

                        if (! await ExcluirComponenteAsync(erro, carga, componenteFrete, unitOfWork, cancellationToken))
                        {
                            await unitOfWork.RollbackAsync();
                            return new JsonpResult(false, true, erro);
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                        serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, ConfiguracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);
                        carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;


                        Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao = await repCargaComposicaoFrete.BuscarPorCodigoComponenteAsync(componenteFrete.Codigo, carga.Codigo);

                        if (cargaComposicao != null)
                            await repCargaComposicaoFrete.DeletarAsync(cargaComposicao);

                        await repCarga.AtualizarAsync(carga);
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Excluiu o componente de frete " + componenteFrete.Descricao + ".", unitOfWork);
                        await unitOfWork.CommitChangesAsync();

                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serCargaFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido);
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        return new JsonpResult(false, true, "Não é possível adicionar um componente na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");
                    }
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, "Ação não permitida.");
                }
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterValorComponenteFreteCalculadoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.Frete svcFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unitOfWork, cancellationToken);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoComponenteFrete = Request.GetIntParam("ComponenteFrete");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = await repComponenteFrete.BuscarPorCodigoAsync(codigoComponenteFrete);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                if (componenteFrete == null)
                    return new JsonpResult(false, true, "Componente de frete não encontrado.");

                decimal valorComponente = 0m;

                Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                if (carga.TabelaFrete != null)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteFreteTabelaFrete = carga.TabelaFrete.Componentes.Where(o => o.ComponenteFrete.Codigo == codigoComponenteFrete).FirstOrDefault();

                    if (componenteFreteTabelaFrete != null && componenteFreteTabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.Percentual && componenteFreteTabelaFrete.TipoPercentual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreValorFreteEComponentes)
                    {
                        decimal valorComponentes = await repCargaComponentesFrete.BuscarValorTotalPorCargaSemImpostosAsync(carga.Codigo);
                        decimal valorFrete = carga.ValorFrete;

                        if (componenteFreteTabelaFrete.ValorInformadoNaTabela == true)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = await repCargaTabelaFreteCliente.BuscarPrimeiroPorCargaAsync(carga.Codigo);

                            if (cargaTabelaFreteCliente != null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros = svcFrete.ObterParametrosCalculoFretePorCarga(carga.TabelaFrete, carga, carga.Pedidos.ToList(), false, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);
                                if (parametros == null)
                                    return new JsonpResult(false, true, "Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis (exemplo, somente pedidos de pallet).");

                                List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> parametrosBaseCalculoTabelaFrete = svcFreteCliente.ObterItensParaCalculoPorTabelaFrete(cargaTabelaFreteCliente.TabelaFreteCliente, parametros);

                                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametroBaseCalculoTabelaFrete = (from obj in parametrosBaseCalculoTabelaFrete where obj.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete && obj.CodigoObjeto == componenteFreteTabelaFrete.Codigo select obj).FirstOrDefault();

                                if (itemParametroBaseCalculoTabelaFrete != null)
                                    valorComponente = svcFreteCliente.ObterValorAumentoPercentual(valorFrete, ref composicao, valorComponentes, itemParametroBaseCalculoTabelaFrete.ValorParaCalculo, 0m, componenteFreteTabelaFrete.TipoPercentual.Value, componenteFreteTabelaFrete.IncluirBaseCalculo ?? false);
                            }
                        }
                        else
                        {
                            valorComponente = svcFreteCliente.ObterValorAumentoPercentual(valorFrete, ref composicao, valorComponentes, componenteFreteTabelaFrete.Percentual ?? 0m, 0m, componenteFreteTabelaFrete.TipoPercentual.Value, componenteFreteTabelaFrete.IncluirBaseCalculo ?? false);
                        }
                    }
                }

                return new JsonpResult(new { Valor = valorComponente.ToString("n2") });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a sugestão de valor para o componente.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados
        

        private async Task<bool> ExcluirComponenteAsync(string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, 
            Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);

            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = await repCargaComponentesFrete.BuscarPorCargaPorCompomenteAsync(carga.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);

            if (cargaComponentesFrete.Any(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia))
            {
                erro = "Não foi possível remover/atualizar o componente pois já foi adicionado um componente do mesmo tipo à partir de uma ocorrência.";
                return false;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFrete)
                await repCargaComponentesFrete.DeletarAsync(componente);

            if (carga.CargaAgrupada)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteAgrupamento = await repCargaComponentesFrete.BuscarPorCargaPorCompomenteAsync(cargaOrigem.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFreteAgrupamento)
                        await repCargaComponentesFrete.DeletarAsync(componente);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoCompoenteFrete = await repComponentesFrete.BuscarPorCompomenteAsync(cargaPedido.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete, false);
                if (cargaPedidoCompoenteFrete != null)
                    await repComponentesFrete.DeletarAsync(cargaPedidoCompoenteFrete);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalComponentesDeFrete = await repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaPedidoAsync(cargaPedido.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete in pedidoXMLNotaFiscalComponentesDeFrete)
                    await repPedidoXMLNotaFiscalComponenteFrete.DeletarAsync(pedidoXMLNotaFiscalComponenteFrete);
            }

            erro = string.Empty;
            return await Task.FromResult(true);
        }

        #endregion
    }
}
