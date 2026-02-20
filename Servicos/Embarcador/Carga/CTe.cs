using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Interfaces.Repositorios;
using Dominio.ObjetosDeValor.CTe;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class CTe : ServicoBase
    {
        public CTe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Metodos Públicos

        public string EmitirCTE(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, int tipoEnvio)
        {
            string mensagem = "";

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIrrelevante = (from obj in cargaPedidos where obj.StageRelevanteCusto == null select obj).ToList();
            string chave = "";

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<int> codigosEmpresaTransportadoraCarga = repEmpresa.BuscarCodigoMatrizEFiliais(carga.Empresa.CNPJ);

            Servicos.Log.GravarInfo("3 - Emitindo CTE " + carga.Codigo, "SolicitarEmissaoDocumentosAutorizados");

            if (cargaPedidosIrrelevante.Count > 0)
            {
                int i;
                for (i = 0; i < cargaPedidosIrrelevante.Count; i++)
                {
                    if (codigosEmpresaTransportadoraCarga.Contains(repEmpresa.BuscarCodigoPorCNPJ(cargaPedidosIrrelevante[i].Pedido?.Destinatario?.Codigo.ToString() ?? string.Empty)))
                    {
                        chave = cargaPedidosIrrelevante[i].NotasFiscais.Where(o => (o.XMLNotaFiscal?.TipoNotaFiscalIntegrada ?? 0) == TipoNotaFiscalIntegrada.RemessaPallet).FirstOrDefault()?.XMLNotaFiscal.Chave ?? string.Empty;
                        break;
                    }
                }
            }

            if (carga.TipoDocumentoTransporte != null)
                cargaPedidos = cargaPedidos.Where(x => x.StageRelevanteCusto != null).ToList();

            if (cargaPedidos?.Count <= 0)
                return mensagem;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repCargaPedido.BuscarPrimeiroPedidoSemNotasDePallet(cargaPedidos);

            if (pedido != null && !string.IsNullOrEmpty(chave))
            {
                pedido.Initialize();
                pedido.ObservacaoCTe += " Chave nota de pallet: " + chave;
                new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).Atualizar(pedido);
            }

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            List<Dominio.Entidades.NFSe> NFSesParaEmissao = new List<Dominio.Entidades.NFSe>();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Carga(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            CTePorNota serCTePorNota = new CTePorNota(unitOfWork);
            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotas repCargaPedidoRotas = new Repositorio.Embarcador.Cargas.CargaPedidoRotas(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao repPedidoXMLNotaFiscalContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao repPedidoCTeParaSubContratacaoContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTeParaSubContratacaos = repPedidoCTeParaSubContratacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTeEmitidos = repCargaPedidoXMLNotaFiscalCTe.BuscarCTEsPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> cargaPedidoRotasCarga = repCargaPedidoRotas.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidosContaContabilContabilizacao = repCargaPedidoContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao = repPedidoCTeParaSubContratacaoContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> pedidosXMLNotaFiscalContaContabilContabilizacao = repPedidoXMLNotaFiscalContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = null;
            int countMotorista = carga.Motoristas?.Count() ?? 0;

            // Sefaz (Ajuste Sinief Nº 8, de 11 Abril de 2025) - Simplificado terá abrangência por município e não mais por UF, como era inicialmente.
            bool ajusteSiniefNro8 = configuracaoCargaEmissaoDocumento.DataAjusteSiniefNro8 != null ? System.DateTime.Now.Date >= (DateTime)configuracaoCargaEmissaoDocumento.DataAjusteSiniefNro8 : false;

            Servicos.Log.GravarInfo($"1 - Carga: '{carga.Codigo}' -> Emissao Contingrncia: {carga.ContingenciaEmissao}", "EmissaoContingencia");
            if (carga.FreteDeTerceiro && !((carga.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || (carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) || (carga.ContingenciaEmissao)))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                    (cargaCIOT == null ||
                     (cargaCIOT.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto &&
                      cargaCIOT.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao &&
                      cargaCIOT.CIOT.Situacao != SituacaoCIOT.AgLiberarViagem)))
                {
                    Dominio.Entidades.Cliente terceiro = null;

                    if (configuracaoEmbarcador == null || configuracaoEmbarcador.TipoContratoFreteTerceiro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro.PorCarga)
                    {
                        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

                        if (contratoFrete != null)
                            terceiro = contratoFrete.TransportadorTerceiro;
                    }
                    else if (configuracaoEmbarcador.TipoContratoFreteTerceiro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro.PorPagamentoAgregado)
                    {
                        terceiro = carga.Terceiro;
                    }

                    if (terceiro != null)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(terceiro, unitOfWork);
                        if (modalidadeTerceiro != null && modalidadeTerceiro.GerarCIOT)
                        {
                            if (carga.Motoristas.Count > 0 && carga.Veiculo != null)
                                ciot = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork).Buscar(terceiro.CPF_CNPJ, carga.Motoristas.Select(o => o.Codigo).FirstOrDefault(), carga.Veiculo.Codigo, new List<SituacaoCIOT>() { SituacaoCIOT.Aberto }, carga.Codigo);
                        }
                    }
                }
                else
                    ciot = cargaCIOT?.CIOT;
            }

            int totalDocumentos = 0, totalDocumentosGerados = 0;

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaPedido = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            if (configuracaoEmbarcador.EmitirCTesSeparandoPorTipoCarga)
                tiposCargaPedido = (from obj in cargaPedidos select obj.Pedido.TipoDeCarga).Distinct().ToList();
            else
                tiposCargaPedido.Add(null);

            for (int tc = 0; tc < tiposCargaPedido.Count; tc++)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoCarga = cargaPedidos;

                if (configuracaoEmbarcador.EmitirCTesSeparandoPorTipoCarga)
                    cargaPedidosTipoCarga = (from obj in cargaPedidos where obj.Pedido.TipoDeCarga == tiposCargaPedido[tc] select obj).ToList();

                List<bool> indicadoresNFSGlobalizado = (from obj in cargaPedidosTipoCarga select obj.IndicadorNFSGlobalizado).Distinct().ToList();
                for (int inf = 0; inf < indicadoresNFSGlobalizado.Count; inf++)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorNFS = (from obj in cargaPedidosTipoCarga where obj.IndicadorNFSGlobalizado == indicadoresNFSGlobalizado[inf] select obj).ToList();

                    List<bool> indicadoresGlobalizadoDestinatario = (from obj in cargaPedidosIndicadorNFS select obj.IndicadorCTeGlobalizadoDestinatario).Distinct().ToList();

                    for (int idg = 0; idg < indicadoresGlobalizadoDestinatario.Count; idg++)
                    {
                        bool indicadorGlobalizadoDestinatario = indicadoresGlobalizadoDestinatario[idg];
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorGlobalizadoDetinatario = (from obj in cargaPedidosIndicadorNFS where obj.IndicadorCTeGlobalizadoDestinatario == indicadorGlobalizadoDestinatario select obj).ToList();

                        #region Agrupamento Indicador Remessa Venda

                        List<bool> indicadoresRemessaVenda = (from obj in cargaPedidosIndicadorGlobalizadoDetinatario select obj.IndicadorRemessaVenda).Distinct().ToList();
                        for (int irv = 0; irv < indicadoresRemessaVenda.Count; irv++)
                        {
                            bool indicadorRemessaVenda = indicadoresRemessaVenda[irv];
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorRemessaVenda = (from obj in cargaPedidosIndicadorGlobalizadoDetinatario where obj.IndicadorRemessaVenda == indicadorRemessaVenda select obj).ToList();

                            #region Agrupamento Redespacho

                            List<bool> redespachos = (from obj in cargaPedidosIndicadorRemessaVenda select obj.Redespacho).Distinct().ToList();
                            for (int a = 0; a < redespachos.Count; a++)
                            {
                                bool redespacho = redespachos[a];
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEmissaoRedespacho = (from obj in cargaPedidosIndicadorGlobalizadoDetinatario where obj.Redespacho == redespacho select obj).ToList();

                                #region Agrupamento Ct-e Simplificado

                                List<bool> indicadoresCteSimplificado = (from obj in cargaPedidosEmissaoRedespacho select obj.IndicadorCTeSimplificado).Distinct().ToList();
                                for (int ics = 0; ics < indicadoresCteSimplificado.Count; ics++)
                                {
                                    bool indicadorCteSimplificado = indicadoresCteSimplificado[ics];
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorCteSimplificado = (from obj in cargaPedidosEmissaoRedespacho where obj.IndicadorCTeSimplificado == indicadorCteSimplificado select obj).ToList();

                                    #region Agrupamento por Cidade/Estado Ct-e Simplificado

                                    List<string> ufSCteSimplificadoEstado = new List<string>();
                                    if (indicadorCteSimplificado)
                                    {
                                        if (ajusteSiniefNro8)
                                            ufSCteSimplificadoEstado = (from obj in cargaPedidosIndicadorCteSimplificado select obj.Destino.Descricao).Distinct().ToList();
                                        else
                                            ufSCteSimplificadoEstado = (from obj in cargaPedidosIndicadorCteSimplificado select obj.Destino.Estado.Sigla).Distinct().ToList();
                                    }
                                    else
                                        ufSCteSimplificadoEstado.Add("Todos");

                                    for (int uf = 0; uf < ufSCteSimplificadoEstado.Count; uf++)
                                    {
                                        string ufCteSimplificadoEstado = ufSCteSimplificadoEstado[uf];
                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIndicadorCteSimplificadoEstado = null;
                                        if (indicadorCteSimplificado)
                                        {
                                            if (ajusteSiniefNro8)
                                                cargaPedidosIndicadorCteSimplificadoEstado = (from obj in cargaPedidosIndicadorCteSimplificado where obj.Destino.Descricao == ufCteSimplificadoEstado select obj).ToList();
                                            else
                                                cargaPedidosIndicadorCteSimplificadoEstado = (from obj in cargaPedidosIndicadorCteSimplificado where obj.Destino.Estado.Sigla == ufCteSimplificadoEstado select obj).ToList();
                                        }
                                        else
                                            cargaPedidosIndicadorCteSimplificadoEstado = (from obj in cargaPedidosIndicadorCteSimplificado select obj).ToList();

                                        #region Carregar informações e gerar Ct-e de acordo com a origem

                                        List<int> codigoCargasOrigem = (from obj in cargaPedidosIndicadorCteSimplificadoEstado select obj.CargaOrigem.Codigo).Distinct().ToList();
                                        for (int co = 0; co < codigoCargasOrigem.Count; co++)
                                        {
                                            int codigoCargaOrigem = codigoCargasOrigem[co];
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaOrigem = (from obj in cargaPedidosIndicadorCteSimplificadoEstado where obj.CargaOrigem.Codigo == codigoCargaOrigem select obj).ToList();

                                            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tipoEmissaoDocumentos = (from obj in cargaPedidosCargaOrigem select obj.TipoRateio).Distinct().ToList();
                                            for (int b = 0; b < tipoEmissaoDocumentos.Count; b++)
                                            {
                                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoDocumento = tipoEmissaoDocumentos[b];
                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEmissaoDocumento = (from obj in cargaPedidosCargaOrigem where obj.TipoRateio == tipoEmissaoDocumento select obj).ToList();
                                                bool contemNotaFiscalSemInscricao = false;
                                                if (configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                                    contemNotaFiscalSemInscricao = repPedidoXMLNotaFiscal.ContemNotaFiscalSemInscricao(cargaPedidosEmissaoDocumento.Select(c => c.Codigo).ToList());

                                                switch (tipoEmissaoDocumento)
                                                {
                                                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos:
                                                        {
                                                            if (!contemNotaFiscalSemInscricao && configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                                                totalDocumentos += repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(cargaPedidosEmissaoDocumento.Select(c => c.Codigo).ToList(), configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario);
                                                            else
                                                                totalDocumentos += repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(cargaPedidosEmissaoDocumento.Select(c => c.Codigo).ToList(), false);
                                                            break;
                                                        }
                                                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada:
                                                        {
                                                            if (!contemNotaFiscalSemInscricao && configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                                                totalDocumentos += (from obj in cargaPedidosEmissaoDocumento select repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(obj.Codigo, configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario, 0)).Sum();
                                                            else
                                                                totalDocumentos += (from obj in cargaPedidosEmissaoDocumento select repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(obj.Codigo, false, 0)).Sum();
                                                            break;
                                                        }
                                                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual:
                                                        totalDocumentos += (from obj in cargaPedidosEmissaoDocumento select repPedidoXMLNotaFiscal.ContarPorCargaPedido(obj.Codigo)).Sum();
                                                        break;
                                                    default:
                                                        break;
                                                }
                                                //totalDocumentos += (from obj in cargaPedidosEmissaoDocumento select repPedidoCTeParaSubContratacao.ContarPorCargaPedido(obj.Codigo)).Sum();

                                                for (int i = 0; i < cargaPedidosEmissaoDocumento.Count; i++)
                                                    totalDocumentos += (from obj in pedidoCTeParaSubContratacaos where obj.CargaPedido.Codigo == cargaPedidosEmissaoDocumento[i].Codigo select obj).Count();

                                                List<bool> tiposCargaFilialEmissora = (from obj in cargaPedidosEmissaoDocumento select obj.CargaPedidoFilialEmissora).Distinct().ToList();
                                                for (int u = 0; u < tiposCargaFilialEmissora.Count; u++)
                                                {
                                                    bool emitirCteFilialEmissora = false;
                                                    if (carga.EmpresaFilialEmissora != null && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                                        emitirCteFilialEmissora = true;

                                                    bool emitirEmitirCTeSub = false;
                                                    if (carga.EmpresaFilialEmissora != null && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                                        emitirEmitirCTeSub = true;

                                                    bool tipoCargaPedidoFilialEmissora = tiposCargaFilialEmissora[u];

                                                    if (emitirCteFilialEmissora && !tipoCargaPedidoFilialEmissora)
                                                        continue;

                                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTiposCargaFilialEmissora = (from obj in cargaPedidosEmissaoDocumento where obj.CargaPedidoFilialEmissora == tipoCargaPedidoFilialEmissora select obj).ToList();

                                                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tipoEmissaoCTeParticipantes = (from obj in cargaPedidosTiposCargaFilialEmissora select obj.TipoEmissaoCTeParticipantes).Distinct().ToList();
                                                    int quantidadeDocumentosAtualizarCarga = ObterQuantidadeDocumentosAtualizarCarga(totalDocumentos);

                                                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga> tiposContratacoesCarga = null;
                                                    if (!emitirEmitirCTeSub || !tipoCargaPedidoFilialEmissora)
                                                        tiposContratacoesCarga = (from obj in cargaPedidosTiposCargaFilialEmissora select obj.TipoContratacaoCarga).Distinct().ToList();
                                                    else
                                                        tiposContratacoesCarga = (from obj in cargaPedidosTiposCargaFilialEmissora select obj.TipoContratacaoCargaSubContratacaoFilialEmissora).Distinct().ToList();

                                                    for (int t = 0; t < tiposContratacoesCarga.Count; t++)
                                                    {

                                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = tiposContratacoesCarga[t];
                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoContratacaoCarga = null;

                                                        if (!emitirEmitirCTeSub || !tipoCargaPedidoFilialEmissora)
                                                            cargaPedidosTipoContratacaoCarga = (from obj in cargaPedidosTiposCargaFilialEmissora where obj.TipoContratacaoCarga == tipoContratacaoCarga select obj).ToList();
                                                        else
                                                            cargaPedidosTipoContratacaoCarga = (from obj in cargaPedidosTiposCargaFilialEmissora where obj.TipoContratacaoCargaSubContratacaoFilialEmissora == tipoContratacaoCarga select obj).ToList();


                                                        if ((tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro
                                                            || tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                                                            || tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada
                                                            || tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                                                            || tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                                                            || tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario))
                                                        {
                                                            Servicos.Embarcador.Carga.CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
                                                            mensagem = serCTeSubContratacaoEmissao.EmitirCTE(cargaPedidosTipoContratacaoCarga, carga, cargasOrigem, tipoEmissaoDocumento, tipoContratacaoCarga, configuracaoEmbarcador, configuracaoGeralCarga, configuracaoCargaEmissaoDocumento, unitOfWork, tipoServicoMultisoftware, pedidosCTeParaSubContratacaoContaContabilContabilizacao, webServiceConsultaCTe, totalDocumentos, quantidadeDocumentosAtualizarCarga, indicadorCteSimplificado, ref totalDocumentosGerados);
                                                            if (emitirCteFilialEmissora)
                                                                emitirCteFilialEmissora = false;
                                                        }

                                                        if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada || emitirCteFilialEmissora)
                                                        {
                                                            if (tipoEmissaoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual && !indicadorGlobalizadoDestinatario)
                                                            {
                                                                for (int i = 0; i < cargaPedidosTipoContratacaoCarga.Count; i++)
                                                                {
                                                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidosTipoContratacaoCarga[i];
                                                                    Servicos.Log.GravarInfo("1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                                                                    serCTePorPedido.GerarCTePorPedidoIndividual(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedido, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.Tomador, cargaPedido.Expedidor, cargaPedido.Recebedor, cargaPedido.Origem, cargaPedido.Destino, cargaPedido.Pedido.EnderecoOrigem, cargaPedido.Pedido.EnderecoDestino, cargaPedido.Pedido.TipoPagamento, cargaPedido.TipoTomador, ref NFSesParaEmissao, tipoServicoMultisoftware, unitOfWork, totalDocumentos, ref totalDocumentosGerados, cargaPedido.ModeloDocumentoFiscal, ciot, redespacho, tipoEnvio, pedidoXMLNotaFiscals, cargaPedidoXMLNotaFiscalCTeEmitidos, cargaPedidoRotasCarga, cargaPedido.Pedido.EnderecoRecebedor, cargaPedidosContaContabilContabilizacao, cargaPedido.Pedido.EnderecoExpedidor);
                                                                    Servicos.Log.GravarInfo("FIM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                List<Dominio.Entidades.Cliente> recebedores = (from obj in cargaPedidosTipoContratacaoCarga select obj.ObterRecebedorParaEmissao()).Distinct().ToList();
                                                                List<Dominio.Entidades.Cliente> expedidores = (from obj in cargaPedidosTipoContratacaoCarga select obj.Expedidor).Distinct().ToList();

                                                                if (recebedores.Count <= 0)
                                                                    recebedores.Add(null);

                                                                if (expedidores.Count <= 0)
                                                                    expedidores.Add(null);

                                                                for (int c = 0; c < expedidores.Count; c++)
                                                                {
                                                                    Dominio.Entidades.Cliente expedidor = expedidores[c];
                                                                    for (int d = 0; d < recebedores.Count; d++)
                                                                    {
                                                                        Dominio.Entidades.Cliente recebedor = recebedores[d];

                                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosIntermediadores = (from obj in cargaPedidosTipoContratacaoCarga where obj.ObterRecebedorParaEmissao() == recebedor && obj.Expedidor == expedidor select obj).ToList();
                                                                        List<int> codigosCargaPedido = cargaPedidosIntermediadores.Select(o => o.Codigo).ToList();

                                                                        List<Dominio.Entidades.Cliente> remetentes = (from obj in cargaPedidosIntermediadores select obj.Pedido.Remetente).Distinct().ToList();

                                                                        if (tipoEmissaoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                                                                            remetentes = repPedidoXMLNotaFiscal.BuscarCadastroRemetentesPorCargaPedido(codigosCargaPedido);

                                                                        List<string> numeroControle = (from obj in cargaPedidosIntermediadores select obj.Pedido.ObterNumeroControle()).Distinct().ToList();

                                                                        for (int nc = 0; nc < numeroControle.Count; nc++)
                                                                        {
                                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoNotaCobertura = (from obj in cargaPedidosIntermediadores where obj.Pedido.ObterNumeroControle() == numeroControle[nc] select obj).ToList();
                                                                            //bool pedidoComNotaCobertura = cargaPedidosIntermediadores.Any(o => !string.IsNullOrWhiteSpace(o.Pedido.NumeroControle)) && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
                                                                            bool pedidoComNotaCobertura = !string.IsNullOrWhiteSpace(numeroControle[nc]);

                                                                            for (int e = 0; e < remetentes.Count; e++)
                                                                            {
                                                                                Dominio.Entidades.Cliente remetente = remetentes[e];
                                                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRemetentes = null;
                                                                                if (tipoEmissaoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                                                                                    cargaPedidosRemetentes = cargaPedidosIntermediadores;
                                                                                else
                                                                                    cargaPedidosRemetentes = (from obj in cargaPedidosIntermediadores where obj.Pedido.Remetente == remetente select obj).ToList();

                                                                                List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>() { };
                                                                                if (tipoEmissaoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                                                                                {
                                                                                    //destinatarios = repPedidoXMLNotaFiscal.BuscarCadastroDestinatariosPorCargaPedido(codigosCargaPedido);//alterado devido a extrema lentidão quando tinha varias notas com varios destinatarios e remetentes
                                                                                    destinatarios = (from obj in cargaPedidosRemetentes select obj.Pedido.Destinatario).Distinct().ToList();
                                                                                }
                                                                                else if ((indicadorGlobalizadoDestinatario || indicadorCteSimplificado) && pedidoComNotaCobertura)
                                                                                {
                                                                                    Dominio.Entidades.Cliente destinatarioNotaCobertura = cargaPedidosRemetentes.Where(o => o.Pedido.Destinatario.Localidade != remetente.Localidade).Select(obj => obj.Pedido.Destinatario).FirstOrDefault();
                                                                                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                                                                    {
                                                                                        if (destinatarioNotaCobertura != null)
                                                                                            destinatarios.Add(destinatarioNotaCobertura);
                                                                                    }
                                                                                    else
                                                                                        destinatarios.Add(remetente);
                                                                                }
                                                                                else if (indicadorGlobalizadoDestinatario || indicadorCteSimplificado)
                                                                                {
                                                                                    carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                                                                    Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork);
                                                                                    Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                                                                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(serPessoa.ConverterObjetoEmpresa(carga.Empresa), "Transportador Destinatário Globalizado", unitOfWork, 0, false);

                                                                                    if (retorno.Status == true)
                                                                                        destinatarios.Add(retorno.cliente);
                                                                                    else
                                                                                        Servicos.Log.GravarInfo("Não foi possivel converter o transportador para cliente para emissão do CT-e Globalizado. Motivo: " + retorno.Mensagem);
                                                                                }
                                                                                else if (indicadorRemessaVenda)
                                                                                {
                                                                                    ClassificacaoNFe? classificacaoNotaDesconsiderar = ClassificacaoNFe.Venda;
                                                                                    if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false)
                                                                                        classificacaoNotaDesconsiderar = ClassificacaoNFe.Remessa;
                                                                                    else if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false)
                                                                                        classificacaoNotaDesconsiderar = ClassificacaoNFe.Venda;

                                                                                    destinatarios = (from obj in cargaPedidosRemetentes
                                                                                                     where obj.NotasFiscais.Any(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNotaDesconsiderar)
                                                                                                     select obj.Pedido.Destinatario).Distinct().ToList();
                                                                                }
                                                                                else
                                                                                    destinatarios = (from obj in cargaPedidosRemetentes select obj.Pedido.Destinatario).Distinct().ToList();

                                                                                for (int f = 0; f < destinatarios.Count; f++)
                                                                                {
                                                                                    Dominio.Entidades.Cliente destinatario = destinatarios[f];

                                                                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCliente = null;
                                                                                    if (tipoEmissaoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                                                                                        cargaPedidosCliente = cargaPedidosRemetentes;// repPedidoXMLNotaFiscal.BuscarCargaPedidoPorDestinatario(codigosCargaPedido, destinatario.CPF_CNPJ);
                                                                                    else if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado && !indicadorRemessaVenda)
                                                                                        cargaPedidosCliente = (from obj in cargaPedidosRemetentes where obj.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ select obj).ToList();
                                                                                    else
                                                                                        cargaPedidosCliente = cargaPedidosRemetentes;

                                                                                    List<Dominio.Enumeradores.TipoPagamento> tiposPagamento = (from obj in cargaPedidosCliente select obj.Pedido.TipoPagamento).Distinct().ToList();

                                                                                    for (int g = 0; g < tiposPagamento.Count; g++)
                                                                                    {
                                                                                        Dominio.Enumeradores.TipoPagamento tipoPagamento = tiposPagamento[g];

                                                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoPagamento = (from obj in cargaPedidosCliente where obj.Pedido.TipoPagamento == tipoPagamento select obj).ToList();

                                                                                        List<Dominio.Enumeradores.TipoTomador> tipoTomadores = (from obj in cargaPedidosTipoPagamento select obj.TipoTomador).Distinct().ToList();

                                                                                        for (int h = 0; h < tipoTomadores.Count; h++)
                                                                                        {
                                                                                            Dominio.Enumeradores.TipoTomador tipoTomador = tipoTomadores[h];

                                                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRequisitantes = (from obj in cargaPedidosTipoPagamento where obj.TipoTomador == tipoTomador select obj).ToList();

                                                                                            List<Dominio.Entidades.Cliente> tomadores = (from obj in cargaPedidosRequisitantes select obj.Tomador).Distinct().ToList();

                                                                                            if (tomadores.Count <= 0)
                                                                                                tomadores.Add(null);

                                                                                            for (int i = 0; i < tomadores.Count; i++)
                                                                                            {
                                                                                                Dominio.Entidades.Cliente tomador = tomadores[i];
                                                                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTomadores = (from obj in cargaPedidosRequisitantes where obj.Tomador == tomador select obj).ToList();

                                                                                                List<Dominio.Entidades.Localidade> origensDaPrestacao = (from obj in cargaPedidosTomadores select obj.Origem).Distinct().ToList();

                                                                                                for (int j = 0; j < origensDaPrestacao.Count; j++)
                                                                                                {
                                                                                                    Dominio.Entidades.Localidade origem = origensDaPrestacao[j];

                                                                                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOrigem = (from obj in cargaPedidosTomadores where obj.Origem.Codigo == origem.Codigo select obj).ToList();

                                                                                                    List<Dominio.Entidades.Localidade> destinosDaPrestacao = new List<Dominio.Entidades.Localidade>();

                                                                                                    if ((indicadorGlobalizadoDestinatario || indicadorCteSimplificado) && pedidoComNotaCobertura)
                                                                                                    {
                                                                                                        var destinoPrestacaoNotaCobertura = (from obj in cargaPedidosOrigem
                                                                                                                                             where obj.Destino != remetente.Localidade
                                                                                                                                             orderby obj.OrdemEntrega
                                                                                                                                             select obj.Destino).LastOrDefault();

                                                                                                        if (destinoPrestacaoNotaCobertura != null)
                                                                                                            destinosDaPrestacao.Add(destinoPrestacaoNotaCobertura);
                                                                                                        else
                                                                                                        {
                                                                                                            Dominio.Entidades.Localidade destPrest = (from obj in cargaPedidosOrigem
                                                                                                                                                      where obj.Destino == remetente.Localidade
                                                                                                                                                      && obj.PossuiNFSManual
                                                                                                                                                      orderby obj.OrdemEntrega
                                                                                                                                                      select obj.Destino).LastOrDefault();
                                                                                                            if (destPrest != null)
                                                                                                                destinosDaPrestacao.Add(destPrest);
                                                                                                            else
                                                                                                                destinosDaPrestacao.Add((from obj in cargaPedidosOrigem orderby obj.OrdemEntrega select obj.Destino).LastOrDefault());

                                                                                                        }
                                                                                                    }
                                                                                                    else if (indicadorRemessaVenda)
                                                                                                        destinosDaPrestacao = (from obj in cargaPedidosOrigem where obj.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ select obj.Destino).Distinct().ToList();
                                                                                                    else if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado)
                                                                                                        destinosDaPrestacao = (from obj in cargaPedidosOrigem select obj.ObterDestinoParaEmissao()).Distinct().ToList();
                                                                                                    else
                                                                                                        destinosDaPrestacao.Add((from obj in cargaPedidosOrigem orderby obj.OrdemEntrega select obj.Destino).LastOrDefault());


                                                                                                    for (int l = 0; l < destinosDaPrestacao.Count; l++)
                                                                                                    {
                                                                                                        List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = new List<Dominio.Entidades.Embarcador.Pedidos.Stage>();
                                                                                                        if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado && !indicadorRemessaVenda)
                                                                                                            stages = (from obj in cargaPedidosOrigem select obj.StageRelevanteCusto).Distinct().ToList();
                                                                                                        else
                                                                                                            stages.Add(null);

                                                                                                        for (int st = 0; st < stages.Count; st++)
                                                                                                        {
                                                                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosStages = null;
                                                                                                            if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado && !indicadorRemessaVenda)
                                                                                                                cargaPedidosStages = (from obj in cargaPedidosOrigem where obj.StageRelevanteCusto == stages[st] select obj).ToList();
                                                                                                            else
                                                                                                                cargaPedidosStages = cargaPedidosOrigem;

                                                                                                            Dominio.Entidades.Localidade destino = destinosDaPrestacao[l];
                                                                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDestinos = null;

                                                                                                            if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado && !indicadorRemessaVenda/* || (indicadorGlobalizadoDestinatario && pedidoComNotaCobertura)*/)
                                                                                                                cargaPedidosDestinos = (from obj in cargaPedidosStages where obj.ObterDestinoParaEmissao().Codigo == destino.Codigo select obj).ToList();
                                                                                                            else
                                                                                                                cargaPedidosDestinos = cargaPedidosStages;

                                                                                                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutrosEnderecoOrigem = (from obj in cargaPedidosDestinos select obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco).Distinct().ToList();

                                                                                                            for (int eo = 0; eo < clienteOutrosEnderecoOrigem.Count; eo++)
                                                                                                            {
                                                                                                                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEnderecoOrigem = clienteOutrosEnderecoOrigem[eo];

                                                                                                                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutrosEnderecoDestino = null;
                                                                                                                if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado && !indicadorRemessaVenda)
                                                                                                                    clienteOutrosEnderecoDestino = (from obj in cargaPedidosDestinos where obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == clienteOutroEnderecoOrigem select obj.Pedido.EnderecoDestino?.ClienteOutroEndereco).Distinct().ToList();
                                                                                                                else
                                                                                                                {
                                                                                                                    clienteOutrosEnderecoDestino = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
                                                                                                                    clienteOutrosEnderecoDestino.Add(null);
                                                                                                                }

                                                                                                                for (int ed = 0; ed < clienteOutrosEnderecoDestino.Count; ed++)
                                                                                                                {
                                                                                                                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEnderecoDestino = clienteOutrosEnderecoDestino[ed];

                                                                                                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEnderecos = null;
                                                                                                                    if (!indicadorGlobalizadoDestinatario && !indicadorCteSimplificado && !indicadorRemessaVenda)
                                                                                                                        cargaPedidosEnderecos = (from obj in cargaPedidosDestinos where obj.Pedido.EnderecoDestino?.ClienteOutroEndereco == clienteOutroEnderecoDestino && obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == clienteOutroEnderecoOrigem select obj).ToList();
                                                                                                                    else
                                                                                                                        cargaPedidosEnderecos = (from obj in cargaPedidosDestinos where obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == clienteOutroEnderecoOrigem select obj).ToList();

                                                                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRementente = (from obj in cargaPedidosEnderecos where obj.Pedido.EnderecoOrigem != null select obj.Pedido.EnderecoOrigem).FirstOrDefault();
                                                                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario = (from obj in cargaPedidosEnderecos where obj.Pedido.EnderecoDestino != null select obj.Pedido.EnderecoDestino).FirstOrDefault();
                                                                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor = (from obj in cargaPedidosEnderecos where obj.Pedido.EnderecoRecebedor != null select obj.Pedido.EnderecoRecebedor).FirstOrDefault();
                                                                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor = (from obj in cargaPedidosEnderecos where obj.Pedido.EnderecoExpedidor != null select obj.Pedido.EnderecoExpedidor).FirstOrDefault();


                                                                                                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = cargaPedidosEnderecos.OrderBy(obj => obj.PedidoSemNFe).FirstOrDefault().ModeloDocumentoFiscal;

                                                                                                                    bool usaClienteOutroEnderecoDestino = clienteOutroEnderecoDestino != null;
                                                                                                                    bool usaClienteOutroEnderecoOrigem = clienteOutroEnderecoOrigem != null;

                                                                                                                    if (modeloDocumentoFiscalCarga == null)
                                                                                                                        modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorModelo("57");

                                                                                                                    if (carga.EmitindoCRT && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                                                                                                                    {
                                                                                                                        modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorModeloCRT();

                                                                                                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                                                                                                            serCTePorPedido.GerarOutrosDocumentosPorPedido(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosDestinos, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, tipoServicoMultisoftware, unitOfWork, modeloDocumentoFiscalCarga, enderecoRecebedor, cargaPedidosContaContabilContabilizacao, enderecoExpedidor);
                                                                                                                    }
                                                                                                                    else if (carga.RealizandoOperacaoContainer)
                                                                                                                    {
                                                                                                                        modeloDocumentoFiscalCarga = carga.TipoOperacao?.ConfiguracaoContainer?.ModeloDocumentoContainer ?? null;

                                                                                                                        serCTePorPedido.GerarOutrosDocumentosPorPedido(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosDestinos, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, tipoServicoMultisoftware, unitOfWork, modeloDocumentoFiscalCarga, enderecoRecebedor, cargaPedidosContaContabilContabilizacao, enderecoExpedidor, true);
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        //o tipo de emissão é definido no pedido e pode variar entre os métodos abaixo, normalmente o mesmo tipo de emissão deve ocorrer para uma carga.
                                                                                                                        switch (tipoEmissaoDocumento)
                                                                                                                        {
                                                                                                                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado: //quando é embarcador por padrão inicialmente emite desta forma
                                                                                                                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado:
                                                                                                                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual:
                                                                                                                                if (totalDocumentos == 0)
                                                                                                                                {
                                                                                                                                    totalDocumentos = expedidores.Count * recebedores.Count * remetentes.Count * destinatarios.Count * tiposPagamento.Count * tipoTomadores.Count * tomadores.Count * origensDaPrestacao.Count * destinosDaPrestacao.Count;

                                                                                                                                    quantidadeDocumentosAtualizarCarga = ObterQuantidadeDocumentosAtualizarCarga(totalDocumentos);
                                                                                                                                }

                                                                                                                                Servicos.Log.GravarInfo("4 - Emitindo GerarCTePorPedidoAgrupado " + carga.Codigo, "SolicitarEmissaoDocumentosAutorizados");
                                                                                                                                serCTePorPedido.GerarCTePorPedidoAgrupado(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosEnderecos, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, ref NFSesParaEmissao, tipoServicoMultisoftware, unitOfWork, totalDocumentos, ref totalDocumentosGerados, modeloDocumentoFiscalCarga, ciot, redespacho, tipoEnvio, indicadorGlobalizadoDestinatario, indicadorCteSimplificado, pedidoXMLNotaFiscals, cargaPedidoXMLNotaFiscalCTeEmitidos, cargaPedidoRotasCarga, enderecoRecebedor, cargaPedidosContaContabilContabilizacao, enderecoExpedidor, usaClienteOutroEnderecoDestino, usaClienteOutroEnderecoOrigem, ajusteSiniefNro8);
                                                                                                                                break;
                                                                                                                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos:
                                                                                                                                serCTePorNota.GerarCTePorNotaAgrupadaEntrePedidos(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosEnderecos, tomador, expedidor, recebedor, tipoPagamento, tipoTomador, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, modeloDocumentoFiscalCarga, ciot, tipoEnvio, enderecoRecebedor, pedidosXMLNotaFiscalContaContabilContabilizacao, enderecoExpedidor, enderecoRementente, enderecoDestinatario);
                                                                                                                                break;
                                                                                                                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada:
                                                                                                                                serCTePorNota.GerarCTePorNotaAgrupada(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosEnderecos, tomador, expedidor, recebedor, tipoPagamento, tipoTomador, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, modeloDocumentoFiscalCarga, ciot, tipoEnvio, enderecoRecebedor, pedidosXMLNotaFiscalContaContabilContabilizacao, enderecoExpedidor);
                                                                                                                                break;
                                                                                                                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual:
                                                                                                                                serCTePorNota.GerarCTePorNotaIndividual(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosEnderecos, tomador, expedidor, recebedor, tipoPagamento, tipoTomador, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, modeloDocumentoFiscalCarga, ciot, tipoEnvio, enderecoRecebedor, pedidosXMLNotaFiscalContaContabilContabilizacao, enderecoExpedidor);
                                                                                                                                break;
                                                                                                                            default:
                                                                                                                                break;
                                                                                                                        }
                                                                                                                        //}

                                                                                                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                                                                                                            serCTePorPedido.GerarOutrosDocumentosPorPedido(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidosDestinos, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, tipoServicoMultisoftware, unitOfWork, modeloDocumentoFiscalCarga, enderecoRecebedor, cargaPedidosContaContabilContabilizacao, enderecoExpedidor);
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    } // limite ultimo for
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (carga.AgImportacaoCTe || carga.AgNFSManual)
                                                                                {
                                                                                    carga = repCarga.BuscarPorCodigo(carga.Codigo);
                                                                                    repCarga.Atualizar(carga);
                                                                                    Log.GravarInfo("Atualizou " + " Carga " + carga.CodigoCargaEmbarcador + " ag = " + carga.AgNFSManual.ToString(), "AgNFSManual");
                                                                                }

                                                                                carga = servicoCarga.AtualizarStatusCustoExtra(carga, svcHubCarga, repCarga);
                                                                            }
                                                                        }


                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        #endregion Carregar informações e gerar Ct-e de acordo com a origem
                                    }

                                    #endregion Agrupamento por Cidade/Estado Ct-e Simplificado
                                }

                                #endregion Agrupamento Ct-e Simplificado
                            }

                            #endregion Agrupamento Redespacho
                        }

                        #endregion Agrupamento Indicador Remessa Venda
                    }
                }
            }

            return mensagem;
        }

        public bool VerificarSePodeEmitirAutomaticamente(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool ativarAutorizacaoAutomaticaDeTodasCargas)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ||
                (carga.TabelaFrete != null && carga.TabelaFrete.EmissaoAutomaticaCTe) ||
                (carga.TabelaFrete == null && carga.TipoOperacao != null && carga.TipoOperacao.EmissaoAutomaticaCTe) ||
                carga.CargaSVM || carga.CargaTakeOrPay ||
                ativarAutorizacaoAutomaticaDeTodasCargas || carga.EmitindoCRT || carga.RealizandoOperacaoContainer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AverbaCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE, List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Enumeradores.FormaAverbacaoCTE forma)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosAverbacao repCargaDadosAverbacao = new Repositorio.Embarcador.Cargas.CargaDadosAverbacao(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (repCargaDadosAverbacao.ConterDadosAverbacaoPorCarga(cargaCTE.Carga.Codigo))
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao> dadosAverbacaos = repCargaDadosAverbacao.BuscarPorCarga(cargaCTE.Carga.Codigo);
                foreach (var dado in dadosAverbacaos)
                {
                    Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceSeguroAverbacao = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                    {
                        ApoliceSeguro = dado.ApoliceSeguro,
                        CargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCTE.Carga.Codigo),
                        Desconto = 0,
                        SeguroFilialEmissora = false
                    };
                    repApoliceSeguroAverbacao.Inserir(apoliceSeguroAverbacao);

                    Dominio.Entidades.AverbacaoCTe averbaCTe = new Dominio.Entidades.AverbacaoCTe
                    {
                        Carga = cargaCTE.Carga,
                        CTe = cargaCTE.CTe,
                        ApoliceSeguroAverbacao = apoliceSeguroAverbacao,
                        Tipo = dado.Tipo,
                        SeguradoraAverbacao = dado.SeguradoraAverbacao,
                        Desconto = dado.Desconto,
                        Percentual = dado.Percentual,
                        Forma = dado.Forma,
                        Adicional = dado.Adicional,
                        Averbacao = dado.Averbacao,
                        CodigoIntegracao = dado.CodigoIntegracao,
                        CodigoRetorno = dado.CodigoRetorno,
                        DataRetorno = dado.DataRetorno,
                        IOF = dado.IOF,
                        MensagemRetorno = dado.MensagemRetorno,
                        Protocolo = dado.Protocolo,
                        SituacaoFechamento = dado.SituacaoFechamento,
                        Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso,
                        tentativasIntegracao = dado.tentativasIntegracao,
                        AverbacaoImportada = true
                    };

                    repAverbacaoCTe.Inserir(averbaCTe);
                }

                return true;
            }

            if (configuracaoTMS.UtilizaEmissaoMultimodal && !repCargaPedido.GerarAverbacaoCarga(cargaCTE.Carga.Codigo))
                return true;

            if (cargaCTE.CTe != null && cargaCTE.CTe.ModeloDocumentoFiscal.AverbarDocumento)
            {
                foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro in apolicesSeguro)
                {
                    if (seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido)
                        continue;

                    if ((cargaCTE.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ||
                        seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM ||
                        seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Senig ||
                        seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro) &&
                        cargaCTE.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                    {
                        Dominio.Entidades.AverbacaoCTe averbaCTe = new Dominio.Entidades.AverbacaoCTe
                        {
                            Carga = cargaCTE.Carga,
                            CTe = cargaCTE.CTe,
                            ApoliceSeguroAverbacao = seguro,
                            Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao,
                            SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido,
                            Desconto = seguro.Desconto.HasValue && seguro.Desconto.Value > 0 ? cargaCTE.CTe.ValorAReceber * (seguro.Desconto.Value / 100) : 0,
                            Percentual = seguro.Desconto,
                            Forma = forma
                        };

                        if (cargaCTE.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros && cargaCTE.CTe.Status == "A")
                            averbaCTe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao;
                        else
                            averbaCTe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;

                        repAverbacaoCTe.Inserir(averbaCTe);

                        if (seguro.ApoliceSeguro?.ValorFixoAverbacao > 0)
                        {
                            cargaCTE.CTe.ValorCarbaAverbacao = seguro.ApoliceSeguro.ValorFixoAverbacao;

                            repositorioCte.Atualizar(cargaCTE.CTe);
                        }
                    }
                }
            }

            return false;
        }

        public void ObterDescricoesComponentesPadrao(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes repTipoOperacaoConfiguracaoComponentes = new Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes(unitOfWork);

            descricaoComponenteValorFrete = null;
            descricaoComponenteValorICMS = null;

            bool setouDescricao = false;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                descricaoComponenteValorFrete = carga.TipoOperacao.DescricaoComponenteFreteEmbarcador;
                IList<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes> tipoOperacaoConfiguracaoComponentes = repTipoOperacaoConfiguracaoComponentes.BuscarPorTipoOperacao(carga.TipoOperacao.Codigo);
                descricaoComponenteValorICMS = tipoOperacaoConfiguracaoComponentes.Where(o => o.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS).Select(o => o.OutraDescricaoCTe).FirstOrDefault();
                setouDescricao = true;
            }

            if (!setouDescricao && tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    descricaoComponenteValorFrete = tomador.DescricaoComponenteFreteEmbarcador;
                    descricaoComponenteValorICMS = tomador.ClienteConfiguracoesComponentes.Where(o => o.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS).Select(o => o.OutraDescricaoCTe).FirstOrDefault();
                }
                else if (tomador.GrupoPessoas != null)
                {
                    descricaoComponenteValorFrete = tomador.GrupoPessoas.DescricaoComponenteFreteEmbarcador;
                    descricaoComponenteValorICMS = tomador.GrupoPessoas.GrupoPessoasConfiguracaoComponentesFretes.Where(o => o.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS).Select(o => o.OutraDescricaoCTe).FirstOrDefault();
                }
            }


            if (string.IsNullOrWhiteSpace(descricaoComponenteValorFrete))
                descricaoComponenteValorFrete = configuracaoTMS.DescricaoComponentePadraoCTe;

            if (string.IsNullOrWhiteSpace(descricaoComponenteValorICMS))
                descricaoComponenteValorICMS = configuracaoTMS.DescricaoComponenteImpostoCTe;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarCTe(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals, Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRementente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades, List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteParaEmissaoCTe, string observacaoCTe, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, List<string> rotas, List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro, Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS, Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Enumeradores.TipoServico tipoServico, Dominio.Enumeradores.TipoCTE tipoCTe, List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores, int tipoEnvio, bool sempreEmitirAutomaticamente, int numeroDoc, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais, string descricaoComponenteValorFrete, string descricaoComponenteValorICMS, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoDestinatario, string itemServico, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoEscrituracao, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoICMS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoPIS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoCOFINS, decimal valorMaximoCentroContabilizacao, List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoesContabeisContabilizacao, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuals)
        {
            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual repConfiguracaoNFSeManual = new Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual(unitOfWork);

            Dominio.Entidades.Cliente cliTomador = ObterClienteTomador(tipoTomador, remetente, destinatario, expedidor, recebedor, tomador);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual configuracaoNFSeManual = repConfiguracaoNFSeManual.BuscarConfiguracaoPadrao();

            bool utilizarRecebedorApenasComoParticipante = cargas != null && cargas.Count > 0 ? cargas.FirstOrDefault().TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false : false;

            string observacaoRegraICMS = "";
            SetarInformacoesCTe(ref cte, empresa, null, remetente, destinatario, tomador, expedidor, recebedor, ref origem, ref destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, quantidades, observacaoCTe, pedidoParaObservacaoCTe, rotas, apolicesSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, cliTomador, tipoEnvio, sempreEmitirAutomaticamente, utilizarRecebedorApenasComoParticipante, unitOfWork, null, ref observacaoRegraICMS, 0, null, null, null, null, null, lancamentoNFSManual.DadosNFS?.ServicoNFSe);

            int numeroDocumento = 0;

            string statusEmissaoAuto = "E";
            if (modeloDocumentoFiscal != null &&
                modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                statusEmissaoAuto = "A";
                if (configuracaoNFSeManual?.UtilizarNumeroSerieInformadoTelaQuandoEmitidoModeloDocumentoNaoFiscal ?? false)
                {
                    cte.Numero = lancamentoNFSManual.DadosNFS?.Numero ?? 0;
                    cte.Serie = lancamentoNFSManual.DadosNFS?.Serie.Numero ?? 0;
                }
                else
                {
                    if (modeloDocumentoFiscal.UtilizarNumeracaoCTe)
                    {
                        if (tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ||
                            tipoServico == Dominio.Enumeradores.TipoServico.Redespacho ||
                            tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                            numeroDocumento = ctesAnteriores?.FirstOrDefault()?.Numero ?? 0;
                    }
                    else if (modeloDocumentoFiscal.UtilizarNumeracaoNFe)
                    {
                        if (int.TryParse(cte.Documentos?.Select(o => o.Numero).FirstOrDefault(), out int numero) && numero > 0)
                            numeroDocumento = numero;
                    }
                }
            }

            if (numeroDoc > 0)
                numeroDocumento = numeroDoc;

            // VerificarSePodeEmitirAutomaticamente Espera uma CARGA como parametro
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, 0, unitOfWork, "1", tipoEnvio, statusEmissaoAuto, modeloDocumentoFiscal, numeroDocumento, tipoServicoMultisoftware, null, cargas.FirstOrDefault()?.Codigo ?? 0, descricaoComponenteValorFrete, descricaoComponenteValorICMS);

            cteIntegrado.CentroResultado = centroResultado;
            cteIntegrado.CentroResultadoEscrituracao = centroResultadoEscrituracao;
            cteIntegrado.CentroResultadoICMS = centroResultadoICMS;
            cteIntegrado.CentroResultadoPIS = centroResultadoPIS;
            cteIntegrado.CentroResultadoCOFINS = centroResultadoCOFINS;
            cteIntegrado.ValorMaximoCentroContabilizacao = valorMaximoCentroContabilizacao;
            cteIntegrado.CentroResultadoDestinatario = centroResultadoDestinatario;
            cteIntegrado.ItemServico = itemServico;

            foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil in configuracoesContabeisContabilizacao)
            {
                Dominio.Entidades.CTeContaContabilContabilizacao cTeContaContabilContabilizacao = new Dominio.Entidades.CTeContaContabilContabilizacao
                {
                    Cte = cteIntegrado,
                    PlanoConta = configuracaoContabil.PlanoConta,
                    TipoContabilizacao = configuracaoContabil.TipoContabilizacao,
                    TipoContaContabil = configuracaoContabil.TipoContaContabil
                };

                repCTeContaContabilContabilizacao.Inserir(cTeContaContabilContabilizacao);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                if (!carga.CargaAgrupada)
                    VincularCTeACarga(carga, cliTomador, cteIntegrado, lancamentoNFSManual, cargaPedidos, pedidoXMLNotaFiscals, observacaoCTe, pedidoParaObservacaoCTe, tipoServicoMultisoftware, modeloDocumentoFiscal, xmlNotasFiscais, unitOfWork, configuracaoTMS, cargaDocumentoParaEmissaoNFSManuals);
                else
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOrigens = repCarga.BuscarCargasOriginais(carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargaOrigens)
                    {
                        VincularCTeACarga(cargaOrigem, cliTomador, cteIntegrado, lancamentoNFSManual, cargaPedidos, pedidoXMLNotaFiscals, observacaoCTe, pedidoParaObservacaoCTe, tipoServicoMultisoftware, modeloDocumentoFiscal, xmlNotasFiscais, unitOfWork, configuracaoTMS, cargaDocumentoParaEmissaoNFSManuals);
                    }
                }
            }

            return cteIntegrado;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarCTe(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.ObjetosDeValor.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);

            Dominio.Entidades.Cliente cliTomador = ObterClienteTomador(preCte.TipoTomador, preCte.Remetente?.Cliente, preCte.Destinatario?.Cliente, preCte.Expedidor?.Cliente, preCte.Recebedor?.Cliente, preCte.Tomador?.Cliente);

            bool utilizarRecebedorApenasComoParticipante = cargaCTe.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false;

            string observacaoRegraICMS = "";
            Dominio.Entidades.Localidade origem = preCte.LocalidadeInicioPrestacao;
            Dominio.Entidades.Localidade destino = preCte.LocalidadeTerminoPrestacao;

            List<string> rotas = new List<string>();
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            if (preCte.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                regraISS.AliquotaISS = cte.ISS.Aliquota;
                regraISS.ValorBaseCalculoISS = cte.ISS.BaseCalculo;
                regraISS.PercentualRetencaoISS = cte.ISS.PercentualRetencao;
                regraISS.ValorRetencaoISS = cte.ISS.ValorRetencao;
                regraISS.ValorISS = cte.ISS.Valor;
            }

            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;
            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            SetarInformacoesCTe(ref cte, preCte.Empresa, null, preCte.Remetente?.Cliente, preCte.Destinatario?.Cliente, preCte.Tomador?.Cliente, preCte.Expedidor?.Cliente, preCte.Recebedor?.Cliente,
                ref origem, ref destino, null, null, Dominio.Enumeradores.TipoPagamento.Pago, Dominio.Enumeradores.TipoTomador.Outros, quantidades, "", null, rotas, apolicesSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware,
                preCte.ModeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, cliTomador, 0, false, utilizarRecebedorApenasComoParticipante, unitOfWork, null, ref observacaoRegraICMS);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, 0, unitOfWork, "1", 0, "A", preCte.ModeloDocumentoFiscal, 0, tipoServicoMultisoftware);

            return cteIntegrado;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> GerarCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRementente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades, List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteParaEmissaoCTe, string observacaoCTe, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, List<string> rotas, List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro, Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS, Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Enumeradores.TipoServico tipoServico, Dominio.Enumeradores.TipoCTE tipoCTe, List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores, int tipoEnvio, bool sempreEmitirAutomaticamente, int numeroCTe, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais, string descricaoComponenteValorFrete, string descricaoComponenteValorICMS, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoDestinatario, string itemServico, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoEscrituracao, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoICMS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoPIS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoCOFINS, decimal valorMaximoCentroContabilizacao, List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoesContabeisContabilizacao, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = null)
        {
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.CTe.CTe servicoCteEmbarcador = new Servicos.Embarcador.CTe.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.PreCTeContaContabilContabilizacao repPreCTeContaContabilContabilizacao = new Repositorio.PreCTeContaContabilContabilizacao(unitOfWork);
            Repositorio.PreConhecimentoDeTransporteEletronico repPreCte = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            Dominio.Entidades.Cliente cliTomador = ObterClienteTomador(tipoTomador, remetente, destinatario, expedidor, recebedor, tomador);

            bool utilizarRecebedorApenasComoParticipante = cargasCTes != null && cargasCTes.Count > 0 ? cargasCTes.FirstOrDefault().Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false : false;

            cte.TipoModal = TipoModal.Rodoviario;
            if (cargaCteComplementarInfo.IndicadorCTeGlobalizado)
                cte.indicadorGlobalizado = Dominio.Enumeradores.OpcaoSimNao.Sim;

            string observacaoRegraICMS = "";

            SetarInformacoesModais(ref cte, null, unitOfWork, cte.ValorAReceber, ref origem, ref destino, cteComplementado, (regraICMS?.Aliquota ?? 0), out decimal valorTaxaEmissao, ref tipoServico, null, 0, null, true);
            SetarInformacoesCTe(ref cte, empresa, null, remetente, destinatario, tomador, expedidor, recebedor, ref origem, ref destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, quantidades, observacaoCTe, pedidoParaObservacaoCTe, rotas, apolicesSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, cliTomador, tipoEnvio, sempreEmitirAutomaticamente, utilizarRecebedorApenasComoParticipante, unitOfWork, null, ref observacaoRegraICMS, 0m, cargaCteComplementarInfo?.CargaOcorrencia?.Carga, null);

            int numeroDocumento = 0;

            string statusEmissaoAuto = "E";

            if (modeloDocumentoFiscal != null &&
                modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                statusEmissaoAuto = "A";
                if (cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.PermiteAlterarNumeroDocumentoOcorrencia)
                    statusEmissaoAuto = "S";

                if (modeloDocumentoFiscal.UtilizarNumeracaoCTe)
                {
                    if (tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ||
                        tipoServico == Dominio.Enumeradores.TipoServico.Redespacho ||
                        tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                        numeroDocumento = ctesAnteriores?.FirstOrDefault()?.Numero ?? 0;
                }
                else if (modeloDocumentoFiscal.UtilizarNumeracaoNFe)
                {
                    if (int.TryParse(cte.Documentos?.Select(o => o.Numero).FirstOrDefault(), out int numero) && numero > 0)
                        numeroDocumento = numero;
                }
            }
            else
            {
                numeroDocumento = numeroCTe;
            }

            if ((cargaCteComplementarInfo.CargaOcorrencia.Carga?.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false))
            {
                if (cte.ObservacoesContribuinte == null)
                    cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

                //o xCampo está fixo para riachuelo se outro cliente exigir sugiro criar um parametro no própria conta contabil ou centro de custo com a descrição xcampo.
                if (configuracoesContabeisContabilizacao.Count > 0 && !string.IsNullOrWhiteSpace(configuracoesContabeisContabilizacao.FirstOrDefault().PlanoConta.PlanoContabilidade))
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "CtContabil";
                    observacao.Descricao = configuracoesContabeisContabilizacao.FirstOrDefault().PlanoConta.PlanoContabilidade;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
                if (centroResultado != null && !string.IsNullOrWhiteSpace(centroResultado.PlanoContabilidade))
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "CentroCD";
                    observacao.Descricao = centroResultado.PlanoContabilidade;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
                if (centroResultadoDestinatario != null && !string.IsNullOrWhiteSpace(centroResultadoDestinatario.PlanoContabilidade))
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "CentroDest";
                    observacao.Descricao = centroResultadoDestinatario.PlanoContabilidade;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
                if (!string.IsNullOrWhiteSpace(cargaCteComplementarInfo.CargaOcorrencia?.Carga?.NumeroOrdem))
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "Ordem";
                    observacao.Descricao = cargaCteComplementarInfo.CargaOcorrencia.Carga.NumeroOrdem;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
            }

            if (
                (empresa.EmissaoDocumentosForaDoSistema && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) || //SE CTe e Emite Documento Fora 
                ((empresa.EmissaoCRTForaDoSistema ?? false) && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros) || //SE CRT e Emite Documento Fora 
                (empresa.EmissaoNFSeForaDoSistema && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) || //Se NFSe e Emite NFSe fora do embarcador
                (empresa.EmiteNFSeOcorrenciaForaEmbarcador && (modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))) //Se NFSe e Emite NFSe de ocorrência fora do embarcador
            {
                Servicos.PreCTe serPreCTE = new Servicos.PreCTe(unitOfWork);
                Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTE = serPreCTE.GerarPreCTePorObjeto(cte, modeloDocumentoFiscal, codigoPedidoCliente: string.Empty, preCTePesoCarga: 0, preCTeValoresFormula: string.Empty, codigoCanalEntrega: 0);

                preCTE.CentroResultado = centroResultado;
                preCTE.CentroResultadoEscrituracao = centroResultadoEscrituracao;
                preCTE.CentroResultadoICMS = centroResultadoICMS;
                preCTE.CentroResultadoPIS = centroResultadoPIS;
                preCTE.CentroResultadoCOFINS = centroResultadoCOFINS;
                preCTE.ValorMaximoCentroContabilizacao = valorMaximoCentroContabilizacao;
                preCTE.CentroResultadoDestinatario = centroResultadoDestinatario;
                preCTE.ItemServico = itemServico;

                foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil in configuracoesContabeisContabilizacao)
                {
                    Dominio.Entidades.PreCTeContaContabilContabilizacao preCTeContaContabilContabilizacao = new Dominio.Entidades.PreCTeContaContabilContabilizacao();
                    preCTeContaContabilContabilizacao.PreCTe = preCTE;
                    preCTeContaContabilContabilizacao.PlanoConta = configuracaoContabil.PlanoConta;
                    preCTeContaContabilContabilizacao.TipoContabilizacao = configuracaoContabil.TipoContabilizacao;
                    preCTeContaContabilContabilizacao.TipoContaContabil = configuracaoContabil.TipoContaContabil;
                    repPreCTeContaContabilContabilizacao.Inserir(preCTeContaContabilContabilizacao);
                }
                repPreCte.Atualizar(preCTE);

                cargaCteComplementarInfo.PreCTe = preCTE;

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();

                    carga.AgImportacaoCTe = true;

                    cargaCTE.Carga = carga;
                    cargaCTE.CargaOrigem = carga;
                    cargaCTE.PreCTe = preCTE;
                    cargaCTE.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores;

                    repCarga.Atualizar(carga);
                    repCargaCte.Inserir(cargaCTE);

                    cargasCTes.Add(cargaCTE);
                }
            }
            else
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, 0, unitOfWork, "1", tipoEnvio, statusEmissaoAuto, modeloDocumentoFiscal, numeroDocumento, tipoServicoMultisoftware, null, cargas?.FirstOrDefault()?.Codigo ?? 0, descricaoComponenteValorFrete, descricaoComponenteValorICMS);

                if (cargaCteComplementarInfo?.CargaOcorrencia?.Carga != null)
                {
                    cteIntegrado.CentroResultadoFaturamento = repCargaPedido.BuscarCentroResultadoPorCarga(cargaCteComplementarInfo.CargaOcorrencia.Carga.Codigo);
                    cteIntegrado.PossuiPedidoSubstituicao = repCargaPedido.PossuiPedidoSubstituicao(cargaCteComplementarInfo.CargaOcorrencia.Carga.Codigo);
                }

                cteIntegrado.CentroResultado = centroResultado;
                cteIntegrado.CentroResultadoEscrituracao = centroResultadoEscrituracao;
                cteIntegrado.CentroResultadoICMS = centroResultadoICMS;
                cteIntegrado.CentroResultadoPIS = centroResultadoPIS;
                cteIntegrado.CentroResultadoCOFINS = centroResultadoCOFINS;
                cteIntegrado.ValorMaximoCentroContabilizacao = valorMaximoCentroContabilizacao;
                cteIntegrado.CentroResultadoDestinatario = centroResultadoDestinatario;
                cteIntegrado.ItemServico = itemServico;

                if (cargaCteComplementarInfo?.CargaCTeComplementado?.CTe != null && cteIntegrado != null)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                    {
                        CTeGerado = cteIntegrado,
                        CTeOriginal = cargaCteComplementarInfo.CargaCTeComplementado.CTe,
                        TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.Complemento
                    };
                    repCTeRelacaoDocumento.Inserir(cTeRelacao);
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil in configuracoesContabeisContabilizacao)
                {
                    Dominio.Entidades.CTeContaContabilContabilizacao cTeContaContabilContabilizacao = new Dominio.Entidades.CTeContaContabilContabilizacao
                    {
                        Cte = cteIntegrado,
                        PlanoConta = configuracaoContabil.PlanoConta,
                        TipoContabilizacao = configuracaoContabil.TipoContabilizacao,
                        TipoContaContabil = configuracaoContabil.TipoContaContabil
                    };

                    repCTeContaContabilContabilizacao.Inserir(cTeContaContabilContabilizacao);
                }

                cargaCteComplementarInfo.CTe = cteIntegrado;

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
                    cargaCTE.Carga = carga;
                    cargaCTE.CargaOrigem = carga;
                    cargaCTE.CTe = cteIntegrado;
                    cargaCTE.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;

                    if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.Numero == "39" && cliTomador.Localidade.Codigo != empresa.Localidade.Codigo)
                    {
                        cteIntegrado.TomadorPagador.InscricaoMunicipal = "";
                        repParticipanteCTe.Atualizar(cteIntegrado.TomadorPagador);
                    }

                    if (xmlNotasFiscais != null && xmlNotasFiscais.Count > 0)
                        cteIntegrado.XMLNotaFiscais = xmlNotasFiscais;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (!configuracaoTMS.NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe && (string.IsNullOrWhiteSpace(observacaoCTe) || !observacaoCTe.Contains("#NumeroPedido")) && pedidoParaObservacaoCTe != null && !string.IsNullOrWhiteSpace(pedidoParaObservacaoCTe.NumeroPedidoEmbarcador))
                            observacaoCTe += " Número do DT: #NumeroPedido.";

                        if (carga.Lacres.Count > 0)
                            observacaoCTe += " Lacres: " + string.Join(", ", carga.Lacres.Select(o => o.Numero)) + ".";

                        if (regraISS.ReterIR && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            observacaoCTe += $" IR Retido: R$ {regraISS.ValorIR} ";

                    }

                    repCargaCte.Inserir(cargaCTE);
                    cargasCTes.Add(cargaCTE);

                    if (cteIntegrado.CST == "90")
                        servicoCteEmbarcador.GerarGuiasTributacaoEstadual(unitOfWork, carga, cteIntegrado);
                }

                SetarObservacaoCTe(null, cteIntegrado, observacaoCTe, null, observacaoRegraICMS, rotas, pedidoParaObservacaoCTe, false, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);
            }

            repCargaCTeComplementoInfo.Atualizar(cargaCteComplementarInfo);

            return cargasCTes;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe GerarCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRementente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades, List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteParaEmissaoCTe, string observacaoCTe, string observacaoCTeTerceiro, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, List<string> rotas, List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro, Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS, Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Enumeradores.TipoServico tipoServico, Dominio.Enumeradores.TipoCTE tipoCTe, List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores, int tipoEnvio, bool sempreEmitirAutomaticamente, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora, bool cteComplementar, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoDestinatario, string itemServico, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoEscrituracao, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoICMS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoPIS, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoCOFINS, decimal valorMaximoCentroContabilizacao, List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoesContabeisContabilizacao, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, string descricaoComponenteICMS = "IMPOSTOS", string descricaoComponenteValorFrete = "FRETE VALOR", Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor = null, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = null, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = null, double cnpjDestinatarioContainer = 0, List<string> chavesCTes = null, Dominio.Entidades.ParticipanteCTe remetenteCTe = null, Dominio.Entidades.ParticipanteCTe destinatarioCTe = null, Dominio.Entidades.ParticipanteCTe expedidorCTe = null, Dominio.Entidades.ParticipanteCTe recebedorCTe = null, bool naoGerarDocumentoAnterior = false, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = null)
        {
            //aqui geracao de cte pela carga
            Servicos.Log.GravarInfo("1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);
            Servicos.Embarcador.CTe.CTe servicoCteEmbarcador = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            var configuracaoAmbiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente();

            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.PreCTeContaContabilContabilizacao repPreCTeContaContabilContabilizacao = new Repositorio.PreCTeContaContabilContabilizacao(unitOfWork);
            Repositorio.PreConhecimentoDeTransporteEletronico repPreCte = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(carga.TipoOperacao?.Codigo ?? 0);

            Dominio.Entidades.Cliente cliTomador = ObterClienteTomador(tipoTomador, remetente, destinatario, expedidor, recebedor, tomador);
            Servicos.Log.GravarInfo("2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());

            decimal tempoViagemHoras = ((carga?.Rota?.TempoDeViagemEmMinutos ?? 0) / 60);

            if (tipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false)
            {
                if (tipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false)
                    DefinirNotasRemessaEntregaParaTransporteObservacao(ref observacaoCTe, cte, ClassificacaoNFe.Remessa);
                else if (tipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false)
                    DefinirNotasRemessaEntregaParaTransporteObservacao(ref observacaoCTe, cte, ClassificacaoNFe.Venda);
            }

            if (tipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
            {
                cte.Documentos = (from obj in cte.Documentos where obj.ClassificacaoNFe != ClassificacaoNFe.Venda select obj).ToList();
                xmlNotasFiscais = xmlNotasFiscais.Where(x => x.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.OrdemVenda).ToList();
            }

            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
            Servicos.Log.GravarInfo("3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            SetarInformacoesModais(ref cte, cargaPedido, unitOfWork, cte.ValorAReceber, ref origem, ref destino, cteComplementado, (regraICMS?.Aliquota ?? 0), out decimal valorTaxaEmissao, ref tipoServico, cargasPedidos, cnpjDestinatarioContainer, chavesCTes);

            if (valorTaxaEmissao > 0 && regraICMS != null && regraICMS.ValorBaseCalculoICMS > 0)// alteração necessária e temporária pois a Aliança não consegue enviar o valor de taxa em componente de frete
            {
                if (regraICMS.Aliquota > 0)
                    valorTaxaEmissao = Math.Round((valorTaxaEmissao / ((100 - regraICMS.Aliquota) / 100)), 2);

                decimal valorParaBaseDeCalculo = regraICMS.ValorBaseCalculoICMS + valorTaxaEmissao;
                decimal percentualICMSIncluirNoFrete = 0;
                if (regraICMS.ValorICMS > 0 && regraICMS.Aliquota > 0)
                {
                    regraICMS.ValorICMSIncluso = serICMS.CalcularICMSInclusoNoFrete(regraICMS.CST, ref valorParaBaseDeCalculo, regraICMS.Aliquota, percentualICMSIncluirNoFrete, regraICMS.PercentualReducaoBC, false, regraICMS.AliquotaInternaDifal);
                    regraICMS.ValorICMS = serICMS.CalcularInclusaoICMSNoFrete(regraICMS.CST, ref valorParaBaseDeCalculo, regraICMS.Aliquota, percentualICMSIncluirNoFrete, regraICMS.PercentualReducaoBC, false);

                    if (regraICMS.PercentualCreditoPresumido > 0m && regraICMS.ValorICMS > 0)
                        regraICMS.ValorCreditoPresumido = decimal.Round(regraICMS.ValorICMS * (regraICMS.PercentualCreditoPresumido / 100), 2, MidpointRounding.AwayFromZero);

                    regraICMS.ValorBaseCalculoICMS = decimal.Round(valorParaBaseDeCalculo, 2, MidpointRounding.AwayFromZero);
                }
            }

            Servicos.Log.GravarInfo("4 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            string observacaoRegraICMS = "";
            SetarInformacoesCTe(ref cte, empresa, tipoOperacao, remetente, destinatario, tomador, expedidor, recebedor, ref origem, ref destino, enderecoRementente, enderecoDestinatario, tipoPagamento, tipoTomador, quantidades, observacaoCTe, pedidoParaObservacaoCTe, rotas, apolicesSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, cliTomador, tipoEnvio, sempreEmitirAutomaticamente, tipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false, unitOfWork, enderecoRecebedor, ref observacaoRegraICMS, tempoViagemHoras, carga, cargaPedido, enderecoExpedidor, ocorrencia);
            Servicos.Log.GravarInfo("5 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            if (carga.Redespacho != null && configuracaoEmbarcador != null && !string.IsNullOrWhiteSpace(configuracaoEmbarcador.CampoObsContribuinteCTeCargaRedespacho) && !string.IsNullOrWhiteSpace(configuracaoEmbarcador.TextoObsContribuinteCTeCargaRedespacho))
            {
                if (cte.ObservacoesContribuinte == null)
                    cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();
                Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                observacao.Identificador = configuracaoEmbarcador.CampoObsContribuinteCTeCargaRedespacho;
                observacao.Descricao = configuracaoEmbarcador.TextoObsContribuinteCTeCargaRedespacho;
                cte.ObservacoesContribuinte.Add(observacao);
            }
            Servicos.Log.GravarInfo("6 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            cte.CaracteristicaTransporte = BuscarDescricaoCaracteristicaTransporteCTe(cargaPedido, ocorrencia, unitOfWork);
            Servicos.Log.GravarInfo("7 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);
            Servicos.Log.GravarInfo("8 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            int numeroDocumento = 0;

            Servicos.Log.GravarInfo("9 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());

            if (carga.Veiculo != null && !string.IsNullOrWhiteSpace(carga.Veiculo.Renavam))
            {
                bool observacaoVeiculo = false;
                cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;
                cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>();
                cte.Veiculos.Add(serVeiculo.ObterVeiculoCTE(carga.Veiculo, carga.Filial != null && carga.Filial.EmiteMDFeFilialEmissora ? carga.EmpresaFilialEmissora : null));
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                {
                    cte.Veiculos.Add(serVeiculo.ObterVeiculoCTE(reboque, carga.Filial != null && carga.Filial.EmiteMDFeFilialEmissora ? carga.EmpresaFilialEmissora : null));
                    if (!observacaoVeiculo)
                    {
                        if (!string.IsNullOrWhiteSpace(reboque.XCampo) || !string.IsNullOrWhiteSpace(reboque.XTexto))
                        {
                            if (cte.ObservacoesContribuinte == null)
                                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();
                            Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                            observacao.Identificador = !string.IsNullOrWhiteSpace(reboque.XCampo) ? reboque.XCampo : "";
                            observacao.Descricao = !string.IsNullOrWhiteSpace(reboque.XTexto) ? reboque.XTexto : "";
                            cte.ObservacoesContribuinte.Add(observacao);
                            observacaoVeiculo = true;
                        }
                    }
                }

                if (!observacaoVeiculo)
                {
                    if (!string.IsNullOrWhiteSpace(carga.Veiculo.XCampo) || !string.IsNullOrWhiteSpace(carga.Veiculo.XTexto))
                    {
                        if (cte.ObservacoesContribuinte == null)
                            cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();
                        Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                        observacao.Identificador = !string.IsNullOrWhiteSpace(carga.Veiculo.XCampo) ? carga.Veiculo.XCampo : "";
                        observacao.Descricao = !string.IsNullOrWhiteSpace(carga.Veiculo.XTexto) ? carga.Veiculo.XTexto : "";
                        cte.ObservacoesContribuinte.Add(observacao);
                    }
                }
            }

            if ((tipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false))
            {
                if (cte.ObservacoesContribuinte == null)
                    cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

                //o xCampo está fixo para riachuelo se outro cliente exigir sugiro criar um parametro no própria conta contabil ou centro de custo com a descrição xcampo.
                if (configuracoesContabeisContabilizacao.Count > 0)
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "CtContabil";
                    observacao.Descricao = configuracoesContabeisContabilizacao.FirstOrDefault().PlanoConta.PlanoContabilidade;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
                if (centroResultado != null)
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "CentroCD";
                    observacao.Descricao = centroResultado.PlanoContabilidade;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
                if (centroResultadoDestinatario != null)
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "CentroDest";
                    observacao.Descricao = centroResultadoDestinatario.PlanoContabilidade;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
                if (!string.IsNullOrWhiteSpace(carga?.NumeroOrdem))
                {
                    Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao();
                    observacao.Identificador = "Ordem";
                    observacao.Descricao = carga.NumeroOrdem;
                    cte.ObservacoesContribuinte.Add(observacao);
                }
            }

            bool isUtilizarXCampoSomenteNoRedespacho = tipoOperacao?.UtilizarXCampoSomenteNoRedespacho ?? false;
            bool isFlagXCampoValido = !isUtilizarXCampoSomenteNoRedespacho || (isUtilizarXCampoSomenteNoRedespacho && carga.Redespacho != null);
            if (!string.IsNullOrWhiteSpace(tipoOperacao?.DocumentoXCampo) && !string.IsNullOrWhiteSpace(tipoOperacao?.DocumentoXTexto) && isFlagXCampoValido)
            {
                if (cte.ObservacoesContribuinte == null)
                    cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

                Dominio.ObjetosDeValor.CTe.Observacao observacao = new Dominio.ObjetosDeValor.CTe.Observacao()
                {
                    Identificador = tipoOperacao.DocumentoXCampo,
                    Descricao = tipoOperacao.DocumentoXTexto,
                };

                cte.ObservacoesContribuinte.Add(observacao);
            }

            Servicos.Log.GravarInfo("10 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            if (cargaMotoristas.Count > 0)
            {
                cte.Motoristas = new List<Dominio.ObjetosDeValor.CTe.Motorista>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
                {
                    Dominio.ObjetosDeValor.CTe.Motorista motoristaCTE = new Dominio.ObjetosDeValor.CTe.Motorista();
                    motoristaCTE.CPF = cargaMotorista.Motorista.CPF;
                    motoristaCTE.Nome = cargaMotorista.Motorista.Nome;
                    cte.Motoristas.Add(motoristaCTE);
                }
            }
            Servicos.Log.GravarInfo("11 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTeComponentesFrete = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();

            Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componenteICMSCTe = GerarComponenteICMS(cte, unitOfWork);
            Servicos.Log.GravarInfo("12 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componenteISSCTe = GerarComponenteISS(cte, unitOfWork);
            Servicos.Log.GravarInfo("13 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            if (componenteICMSCTe != null)
                cargaCTeComponentesFrete.Add(componenteICMSCTe);

            if (componenteISSCTe != null)
                cargaCTeComponentesFrete.Add(componenteISSCTe);

            Servicos.Log.GravarInfo("14 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            if (componentesFreteParaEmissaoCTe.Count > 0)
            {
                cte.ComponentesDaPrestacao = new List<Dominio.ObjetosDeValor.CTe.ComponentePrestacao>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete in componentesFreteParaEmissaoCTe)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete
                    {
                        TipoComponenteFrete = componenteFrete.TipoComponenteFrete,
                        ComponenteFrete = componenteFrete.ComponenteFrete,
                        ValorComponente = carga.PossuiComponenteFreteComImpostoIncluso ? componenteFrete.ValorComponenteComICMSIncluso : componenteFrete.ValorComponente,
                        IncluirBaseCalculoICMS = componenteFrete.IncluirBaseCalculoImposto,
                        TipoValor = componenteFrete.TipoValor,
                        DescontarValorTotalAReceber = componenteFrete.DescontarValorTotalAReceber,
                        AcrescentaValorTotalAReceber = componenteFrete.AcrescentaValorTotalAReceber,
                        NaoSomarValorTotalAReceber = componenteFrete.NaoSomarValorTotalAReceber,
                        DescontarDoValorAReceberValorComponente = componenteFrete.DescontarDoValorAReceberValorComponente,
                        DescontarDoValorAReceberOICMSDoComponente = componenteFrete.DescontarDoValorAReceberOICMSDoComponente,
                        ValorICMSComponenteDestacado = componenteFrete.ValorICMSComponenteDestacado,
                        NaoSomarValorTotalPrestacao = componenteFrete.NaoSomarValorTotalPrestacao,
                        Percentual = componenteFrete.Percentual,
                        Moeda = componenteFrete.Moeda,
                        ValorTotalMoeda = componenteFrete.ValorTotalMoeda,
                        ValorCotacaoMoeda = componenteFrete.ValorCotacaoMoeda,
                        IncluirIntegralmenteContratoFreteTerceiro = componenteFrete.IncluirIntegralmenteContratoFreteTerceiro
                    };

                    cargaCTeComponentesFrete.Add(cargaCTeComponenteFrete);

                    //quando o valor do Componente deve ser descontado ou acrescido no total a receber não é gerado um componente para o ducumento fiscal.

                    if (componenteFrete.ComponenteFrete == null || (!componenteFrete.ComponenteFrete.ComponentePertenceComposicaoFreteValor && !componenteFrete.ComponenteFrete.ComponenteApenasInformativoDocumentoEmitido))
                    {
                        if (!cargaCTeComponenteFrete.DescontarValorTotalAReceber && !cargaCTeComponenteFrete.AcrescentaValorTotalAReceber)
                        {
                            Dominio.ObjetosDeValor.CTe.ComponentePrestacao componente = new Dominio.ObjetosDeValor.CTe.ComponentePrestacao();
                            if (string.IsNullOrWhiteSpace(componenteFrete.OutraDescricaoCTe))
                            {
                                if (componenteFrete.ComponenteFrete == null)
                                    componente.Descricao = componenteFrete.DescricaoComponente;
                                else
                                    componente.Descricao = componenteFrete.ComponenteFrete.Descricao;
                            }
                            else
                            {
                                componente.Descricao = componenteFrete.OutraDescricaoCTe;
                            }

                            if (componente.Descricao.Length > 15)
                                componente.Descricao = componente.Descricao.Substring(0, 15);

                            if (carga.PossuiComponenteFreteComImpostoIncluso)
                                componente.Valor = componenteFrete.ValorComponenteComICMSIncluso;
                            else
                                componente.Valor = componenteFrete.ValorComponente;

                            componente.IncluiBaseCalculoICMS = componenteFrete.IncluirBaseCalculoImposto;

                            if (cargaPedido != null && cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao)
                            {
                                if (cargaCTeComponenteFrete.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                                {
                                    if (!cargaCTeComponenteFrete.NaoSomarValorTotalAReceber)
                                        cte.ValorAReceber += componente.Valor;

                                    if (cargaCTeComponenteFrete?.DescontarDoValorAReceberValorComponente ?? false)
                                        cte.ValorAReceber -= componente.Valor;

                                    if (cargaCTeComponenteFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false)
                                        cte.ValorAReceber -= cargaCTeComponenteFrete.ValorICMSComponenteDestacado;

                                    if (!cargaCTeComponenteFrete.NaoSomarValorTotalPrestacao)
                                        cte.ValorTotalPrestacaoServico += componente.Valor;
                                }
                            }
                            else
                            {
                                if (!cargaCTeComponenteFrete.NaoSomarValorTotalAReceber)
                                    cte.ValorAReceber += componente.Valor;

                                if (cargaCTeComponenteFrete?.DescontarDoValorAReceberValorComponente ?? false)
                                    cte.ValorAReceber -= componente.Valor;

                                if (cargaCTeComponenteFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false)
                                    cte.ValorAReceber -= cargaCTeComponenteFrete.ValorICMSComponenteDestacado;

                                if (!cargaCTeComponenteFrete.NaoSomarValorTotalPrestacao)
                                    cte.ValorTotalPrestacaoServico += componente.Valor;
                            }
                            componente.IncluiValorAReceber = true;
                            componente.CodigoComponenteFrete = componenteFrete?.ComponenteFrete?.Codigo ?? 0;

                            cte.ComponentesDaPrestacao.Add(componente);

                            //if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            //{
                            //    if (cte.ItensNFSe != null && cte.ItensNFSe.Count > 0)
                            //    {
                            //        cte.ItensNFSe.FirstOrDefault().ValorServico += componente.Valor;
                            //        cte.ItensNFSe.FirstOrDefault().ValorTotal += componente.Valor;
                            //    }
                            //d}
                        }
                        else
                        {
                            cte.ValorAReceber += componenteFrete.ValorComponente;

                            //quando acrescenta deve somar no total da prestação, quando desconta não.
                            if (cargaCTeComponenteFrete.AcrescentaValorTotalAReceber)
                            {
                                if (cargaPedido != null && cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao)
                                {
                                    if (cargaCTeComponenteFrete.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                                    {
                                        cte.ValorTotalPrestacaoServico += componenteFrete.ValorComponente;
                                        cte.ValorFrete += componenteFrete.ValorComponente;
                                    }
                                }
                                else
                                {
                                    cte.ValorTotalPrestacaoServico += componenteFrete.ValorComponente;
                                    cte.ValorFrete += componenteFrete.ValorComponente;
                                }
                            }
                            else
                            {
                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                                {
                                    //regra fixa embarcador para Minerva
                                    cte.ValorTotalPrestacaoServico += componenteFrete.ValorComponente;
                                    cte.ValorFrete += componenteFrete.ValorComponente;
                                }
                            }
                        }
                    }
                }
            }
            Servicos.Log.GravarInfo("15 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());

            if (cargaPedido != null && cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao && cte.ValorTotalPrestacaoServico > 0)
            {
                cte.ValorTotalPrestacaoServico = (cte.ValorTotalPrestacaoServico / ((100 - cte.ICMS.Aliquota) / 100));
                if (cte.TipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
                {
                    cte.ValorAReceber = cte.ValorTotalPrestacaoServico;
                    cte.ValorTotalPrestacaoServico = cte.ValorTotalPrestacaoServico + cte.ICMS.Valor;
                }
                else
                    cte.ValorAReceber = cte.ValorTotalPrestacaoServico - cte.ICMS.Valor;
            }

            if ((configuracaoEmbarcador?.UtilizaEmissaoMultimodal ?? false) && (cargaPedido?.FormulaRateio?.RatearEmBlocoDeEmissao ?? false))
            {
                cte.ValorTotalPrestacaoServico = Math.Round(cte.ValorTotalPrestacaoServico, 2, MidpointRounding.ToEven);
                cte.ValorAReceber = Math.Round(cte.ValorAReceber, 2, MidpointRounding.ToEven);
            }

            if (regraICMS != null && !regraICMS.DescontarICMSDoValorAReceber && cte.ValorTotalPrestacaoServico == cte.ValorAReceber && cte.ICMS != null && !string.IsNullOrWhiteSpace(cte.ICMS.CST) && cte.ICMS.CST != "60" && cte.ICMS.CST != "060" && cte.ICMS.BaseCalculo > 0 && cte.ICMS.BaseCalculo != cte.ValorTotalPrestacaoServico && configuracaoEmbarcador.UtilizaEmissaoMultimodal && configuracaoAmbiente.RecalcularICMSNaEmissaoCTe.Value)
            {
                if (regraICMS.PercentualReducaoBC <= 0 && regraICMS.PercentualInclusaoBC <= 0)
                    cte.ICMS.BaseCalculo = cte.ValorTotalPrestacaoServico;
            }
            else if (regraICMS != null && regraICMS.DescontarICMSDoValorAReceber && cte.ValorTotalPrestacaoServico != cte.ICMS.BaseCalculo && cte.ICMS != null && !string.IsNullOrWhiteSpace(cte.ICMS.CST) && (cte.ICMS.CST == "60" || cte.ICMS.CST == "060") && cte.ICMS.BaseCalculo > 0 && configuracaoEmbarcador.UtilizaEmissaoMultimodal && configuracaoAmbiente.RecalcularICMSNaEmissaoCTe.Value)
            {
                if (regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao)
                {
                    if (regraICMS.PercentualReducaoBC <= 0 && regraICMS.PercentualInclusaoBC <= 0)
                        cte.ICMS.BaseCalculo = cte.ValorTotalPrestacaoServico;
                }
            }
            if ((configuracaoEmbarcador.UtilizaEmissaoMultimodal || configuracaoAmbiente.AplicarValorICMSNoComplemento.Value) && configuracaoAmbiente.RecalcularICMSNaEmissaoCTe.Value && regraICMS != null && cte.ValorTotalPrestacaoServico == cte.ValorAReceber && cte.ICMS.BaseCalculo > 0 && cte.ICMS.BaseCalculo != cte.ValorTotalPrestacaoServico)
            {
                if (regraICMS.PercentualReducaoBC <= 0 && cte.ValorTotalPrestacaoServico > 0)
                {
                    decimal diferenca = Math.Round(cte.ICMS.BaseCalculo - cte.ValorTotalPrestacaoServico, 2, MidpointRounding.ToEven);
                    Servicos.Log.GravarInfo("16.1 - Diferenca de " + diferenca.ToString("n2") + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
                    if (diferenca >= -0.02m || diferenca <= 0.02m)
                        cte.ICMS.BaseCalculo = cte.ValorTotalPrestacaoServico;
                }
                else if (configuracaoAmbiente.AplicarValorICMSNoComplemento.Value && cte.ValorTotalPrestacaoServico == 0 && cte.TipoCTe == Dominio.Enumeradores.TipoCTE.Complemento && cte.ICMS?.Valor > 0)
                {
                    cte.ValorTotalPrestacaoServico = cte.ICMS?.Valor ?? 0m;
                    cte.ValorAReceber = cte.ICMS?.Valor ?? 0m;
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
            cargaCTE.Carga = carga;
            cargaCTE.CargaOrigem = cargaPedido?.CargaOrigem ?? carga;

            string statusEmissaoAuto = "E";
            Servicos.Log.GravarInfo("16 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            if (modeloDocumentoFiscal != null &&
                modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                statusEmissaoAuto = "A";
                if (ocorrencia?.TipoOcorrencia?.PermiteAlterarNumeroDocumentoOcorrencia ?? false)
                    statusEmissaoAuto = "S";

                if (modeloDocumentoFiscal.UtilizarNumeracaoCTe)
                {
                    if (tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoServico == Dominio.Enumeradores.TipoServico.Redespacho || tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                        numeroDocumento = ctesAnteriores?.FirstOrDefault()?.Numero ?? 0;
                    else
                        numeroDocumento = repCargaCte.BuscarNumeroPrimeiroCTeCarga(carga.Codigo);
                }
                else if (modeloDocumentoFiscal.UtilizarNumeracaoNFe)
                {
                    if (int.TryParse(cte.Documentos?.Select(o => o.Numero).FirstOrDefault(), out int numero) && numero > 0)
                        numeroDocumento = numero;
                }
            }
            Servicos.Log.GravarInfo("17 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            Servicos.Log.GravarInfo($"2 - Carga: '{carga.Codigo}' -> Emissao Contingencia: {carga.ContingenciaEmissao}", "EmissaoContingencia");
            if (((empresa.EmissaoDocumentosForaDoSistema || (tipoOperacao != null && (tipoOperacao.EmissaoDocumentosForaDoSistema && (carga.EmpresaFilialEmissora == null || carga.EmpresaFilialEmissora.Codigo != empresa.Codigo))) || (carga.ContingenciaEmissao)) && modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) || //SE CTe e Emite Documento Fora 
                (empresa.EmissaoNFSeForaDoSistema && modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) || //Se NFSe e Emite NFSe fora do embarcador
                ((empresa.EmissaoCRTForaDoSistema ?? false) && modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros) ||
                (empresa.EmiteNFSeOcorrenciaForaEmbarcador && (cteComplementar || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento) && modeloDocumentoFiscal != null && (modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))) //Se NFSe e Emite NFSe de ocorrência fora do embarcador
            {
                string codigoPedidoCliente = cargaPedido?.Pedido?.CodigoPedidoCliente ?? string.Empty;
                int codigoCanalEntrega = cargaPedido?.Pedido?.CanalEntrega?.Codigo ?? 0;

                decimal preCTePesoCarga = carga.DadosSumarizados?.PesoTotal > 0 ? carga.DadosSumarizados.PesoTotal : 0;
                Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete composicaoFrete = repCargaComposicaoFrete.BuscarPorCargaETipoParametro(carga.Codigo, false, TipoParametroBaseTabelaFrete.Peso);
                string preCTeValoresFormula = composicaoFrete?.ValoresFormula ?? string.Empty;

                Servicos.Log.GravarInfo("18 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
                Servicos.PreCTe serPreCTE = new Servicos.PreCTe(unitOfWork);
                Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTE = serPreCTE.GerarPreCTePorObjeto(cte, modeloDocumentoFiscal, codigoPedidoCliente, preCTePesoCarga, preCTeValoresFormula, codigoCanalEntrega);
                cargaCTE.PreCTe = preCTE;

                preCTE.CentroResultado = centroResultado;
                preCTE.CentroResultadoDestinatario = centroResultadoDestinatario;
                preCTE.CentroResultadoEscrituracao = centroResultadoEscrituracao;
                preCTE.CentroResultadoICMS = centroResultadoICMS;
                preCTE.CentroResultadoPIS = centroResultadoPIS;
                preCTE.CentroResultadoCOFINS = centroResultadoCOFINS;
                preCTE.ValorMaximoCentroContabilizacao = valorMaximoCentroContabilizacao;
                preCTE.ItemServico = itemServico;

                foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil in configuracoesContabeisContabilizacao)
                {
                    Dominio.Entidades.PreCTeContaContabilContabilizacao preCTeContaContabilContabilizacao = new Dominio.Entidades.PreCTeContaContabilContabilizacao();
                    preCTeContaContabilContabilizacao.PreCTe = preCTE;
                    preCTeContaContabilContabilizacao.PlanoConta = configuracaoContabil.PlanoConta;
                    preCTeContaContabilContabilizacao.TipoContabilizacao = configuracaoContabil.TipoContabilizacao;
                    preCTeContaContabilContabilizacao.TipoContaContabil = configuracaoContabil.TipoContaContabil;
                    repPreCTeContaContabilContabilizacao.Inserir(preCTeContaContabilContabilizacao);
                }
                repPreCte.Atualizar(preCTE);

                carga.AgImportacaoCTe = true;
                //new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, configuracaoEmbarcador).DefinirCargaCTePorPreCTe(cargaCTE, tipoServicoMultisoftware);                
                cargaCTE.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores;
                preCTE.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);

                Servicos.Log.GravarInfo("19 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            }
            else
            {
                Servicos.Log.GravarInfo("18 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
                string statusCTe = !sempreEmitirAutomaticamente ? (VerificarSePodeEmitirAutomaticamente(tipoServicoMultisoftware, carga, configuracaoEmbarcador.AtivarAutorizacaoAutomaticaDeTodasCargas) ? statusEmissaoAuto : "S") : statusEmissaoAuto;
                bool objCacheEmissaoDocumentos = CacheProvider.Instance.Get<bool>("TransmitirDocumentoPorThread");
                if (objCacheEmissaoDocumentos && statusCTe == "E")
                {
                    statusCTe = "S";
                    cargaCTE.SituacaoProcessamentoThread = SituacaoProcessamentoThread.Pendente;
                }
                else
                    cargaCTE.SituacaoProcessamentoThread = SituacaoProcessamentoThread.NaoSeAplica;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, 0, unitOfWork, "1", tipoEnvio, statusCTe, modeloDocumentoFiscal, numeroDocumento, tipoServicoMultisoftware, null, carga.Codigo, descricaoComponenteValorFrete, descricaoComponenteICMS, naoGerarDocumentoAnterior, tipoOperacao);
                Servicos.Log.GravarInfo("19 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
                if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && cliTomador.Localidade.Codigo != empresa.Localidade.Codigo)
                {
                    cteIntegrado.TomadorPagador.InscricaoMunicipal = "";
                    repParticipanteCTe.Atualizar(cteIntegrado.TomadorPagador);
                }

                if (carga != null)
                {
                    cteIntegrado.CentroResultadoFaturamento = repCargaPedido.BuscarCentroResultadoPorCarga(carga.Codigo);
                    cteIntegrado.PossuiPedidoSubstituicao = repCargaPedido.PossuiPedidoSubstituicao(carga.Codigo);
                }

                if (cteComplementado != null)
                {
                    if (cteComplementado != null && cteIntegrado != null)
                    {
                        Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                        {
                            CTeGerado = cteIntegrado,
                            CTeOriginal = cteComplementado,
                            TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.Complemento
                        };

                        repCTeRelacaoDocumento.Inserir(cTeRelacao);
                    }
                }

                if (regraICMS != null && regraICMS.CodigoRegra > 0)
                    cteIntegrado.RegraICMS = new Dominio.Entidades.Embarcador.ICMS.RegraICMS() { Codigo = regraICMS.CodigoRegra };
                cteIntegrado.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);

                cteIntegrado.CentroResultado = centroResultado;
                cteIntegrado.CentroResultadoEscrituracao = centroResultadoEscrituracao;
                cteIntegrado.CentroResultadoICMS = centroResultadoICMS;
                cteIntegrado.CentroResultadoPIS = centroResultadoPIS;
                cteIntegrado.CentroResultadoCOFINS = centroResultadoCOFINS;
                cteIntegrado.ValorMaximoCentroContabilizacao = valorMaximoCentroContabilizacao;
                cteIntegrado.CentroResultadoDestinatario = centroResultadoDestinatario;
                cteIntegrado.ItemServico = itemServico;

                foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil configuracaoContabil in configuracoesContabeisContabilizacao)
                {
                    Dominio.Entidades.CTeContaContabilContabilizacao cTeContaContabilContabilizacao = new Dominio.Entidades.CTeContaContabilContabilizacao
                    {
                        Cte = cteIntegrado,
                        PlanoConta = configuracaoContabil.PlanoConta,
                        TipoContabilizacao = configuracaoContabil.TipoContabilizacao,
                        TipoContaContabil = configuracaoContabil.TipoContaContabil
                    };

                    repCTeContaContabilContabilizacao.Inserir(cTeContaContabilContabilizacao);
                }

                if (xmlNotasFiscais != null && xmlNotasFiscais.Count > 0)
                {
                    cteIntegrado.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xmlNotasFiscais)
                    {
                        int codigo = xmlNotaFiscal.Codigo;

                        if (!cteIntegrado.XMLNotaFiscais.Any(o => o.Codigo == xmlNotaFiscal.Codigo))
                            cteIntegrado.XMLNotaFiscais.Add(new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal() { Codigo = codigo });
                    }

                }
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                if (cteIntegrado.ModeloDocumentoFiscal?.DocumentoTipoCRT ?? false)
                {
                    cteIntegrado.SiglaPaisOrigemCRT = cteIntegrado.Remetente?.Pais?.Abreviacao ?? "BR";
                    cteIntegrado.NumeroLicencaTNTICRT = cteIntegrado.Remetente?.Pais?.LicencaTNTI ?? "5525";

                    // caso for exportacao, então deve-se usar a abreviacao da origem e o numeroTNTI do destino 
                    if (cteIntegrado.SiglaPaisOrigemCRT == "BR" && (cteIntegrado.Destinatario?.Pais?.Abreviacao ?? "BR") != "BR")
                    {
                        cteIntegrado.NumeroLicencaTNTICRT = cteIntegrado.Destinatario?.Pais?.LicencaTNTI ?? "5525";
                    }

                    if (!string.IsNullOrWhiteSpace(cteIntegrado.NumeroLicencaTNTICRT) && cteIntegrado.NumeroLicencaTNTICRT.Contains("/"))
                        cteIntegrado.NumeroLicencaTNTICRT = cteIntegrado.NumeroLicencaTNTICRT.Split('/').FirstOrDefault();

                    cteIntegrado.NumeroSequencialCRT = repCte.ProximoNumeroSequencialCRT(cteIntegrado.SiglaPaisOrigemCRT, cteIntegrado.NumeroLicencaTNTICRT);
                    cteIntegrado.NumeroCRT = cteIntegrado.SiglaPaisOrigemCRT + cteIntegrado.NumeroLicencaTNTICRT + cteIntegrado.NumeroSequencialCRT.ToString().PadLeft(5, '0');

                    repCte.Atualizar(cteIntegrado);
                }

                if (cargaPedido != null && cargaPedido.IncluirICMSBCFreteProprio != null && !cargaPedido.IncluirICMSBCFreteProprio.Value)
                    AtualizarValorComponenteICMSFreteProprio(cteIntegrado, unitOfWork);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (!configuracaoEmbarcador.NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe && (string.IsNullOrWhiteSpace(observacaoCTe) || !observacaoCTe.Contains("#NumeroPedido")) && pedidoParaObservacaoCTe != null && !string.IsNullOrWhiteSpace(pedidoParaObservacaoCTe.NumeroPedidoEmbarcador))
                        observacaoCTe += " Número do DT: #NumeroPedido.";

                    List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacres = repCargaLacre.BuscarPorCarga(carga.Codigo);
                    if (lacres?.Count > 0)
                        observacaoCTe += " Lacres: " + string.Join(", ", lacres.Select(o => o.Numero)) + ".";

                    if (regraISS.ReterIR && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        observacaoCTe += $" IR Retido: R$ {regraISS.ValorIR} ";
                }

                Servicos.Log.GravarInfo("20 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
                SetarObservacaoCTe(cargaPedido, cteIntegrado, observacaoCTe, observacaoCTeTerceiro, observacaoRegraICMS, rotas, pedidoParaObservacaoCTe, false, unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, configuracaoGeralCarga, cargasPedidos, xmlNotasFiscais);

                cargaCTE.CTe = cteIntegrado;
                cargaCTE.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;
                Servicos.Log.GravarInfo("21 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());
            }

            cargaCTE.CargaCTeFilialEmissora = cargaCTeFilialEmissora;

            repCargaCte.Inserir(cargaCTE);

            if (carga.AgImportacaoCTe == true)
                new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, configuracaoEmbarcador).DefinirCargaCTePorPreCTe(cargaCTE, tipoServicoMultisoftware);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete in cargaCTeComponentesFrete)
            {
                cargaCTeComponenteFrete.CargaCTe = cargaCTE;
                repCargaCTeComponentesFrete.Inserir(cargaCTeComponenteFrete);
            }
            Servicos.Log.GravarInfo("FIM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTe_" + tipoEnvio.ToString());

            if (cargaCTE.CTe != null && cargaCTE.CTe.CST == "90")
                servicoCteEmbarcador.GerarGuiasTributacaoEstadual(unitOfWork, carga, cargaCTE.CTe);

            if (cargaCTE.CTe != null)
            {
                new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).GeracaoControleDocumento(cargaCTE.CTe);
            }

            return cargaCTE;
        }

        #endregion

        #region Métodos Privados

        private void SetarInformacoesModais(ref Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, decimal valorPrestacaoServico, ref Dominio.Entidades.Localidade origem, ref Dominio.Entidades.Localidade destino, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado, decimal aliquotaICMS, out decimal valorTaxaEmissao, ref Dominio.Enumeradores.TipoServico tipoServico, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = null, double cnpjDestinatarioContainer = 0, List<string> chavesCTesAnterior = null, bool naoCalcularTaxaEmissao = false)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            List<int> codigosContainer = new List<int>();
            valorTaxaEmissao = 0;
            if (cteComplementado != null)
            {
                naoCalcularTaxaEmissao = true;
                if (cteComplementado.PortoOrigem != null)
                    cte.PortoOrigem = new Dominio.ObjetosDeValor.CTe.Porto()
                    {
                        Codigo = cteComplementado.PortoOrigem.Codigo
                    };
                if (cteComplementado.PortoDestino != null)
                    cte.PortoDestino = new Dominio.ObjetosDeValor.CTe.Porto()
                    {
                        Codigo = cteComplementado.PortoDestino.Codigo
                    };
                if (cteComplementado.TerminalOrigem != null)
                    cte.TerminalOrigem = new Dominio.ObjetosDeValor.CTe.Terminal()
                    {
                        CodigoTerminal = cteComplementado.TerminalOrigem.Codigo
                    };
                if (cteComplementado.TerminalDestino != null)
                    cte.TerminalDestino = new Dominio.ObjetosDeValor.CTe.Terminal()
                    {
                        CodigoTerminal = cteComplementado.TerminalDestino.Codigo
                    };
                if (cteComplementado.Viagem != null)
                    cte.Viagem = new Dominio.ObjetosDeValor.CTe.Viagem()
                    {
                        CodigoViagem = cteComplementado.Viagem.Codigo
                    };

                cte.TipoModal = cteComplementado.TipoModal;
                if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal)
                {
                    cte.NumeroCOTM = cteComplementado.NumeroCOTM;
                    if (cteComplementado.Navio != null)
                        cte.Navio = new Dominio.ObjetosDeValor.CTe.Navio()
                        {
                            CodigoNavio = cteComplementado.Navio.Codigo
                        };
                    if (cteComplementado.Viagem != null)
                        cte.NumeroViagem = cteComplementado.Viagem.NumeroViagem.ToString("D");

                    cte.Balsas = new List<Dominio.ObjetosDeValor.CTe.Balsa>();
                    cte.Containeres = new List<Dominio.ObjetosDeValor.CTe.Container>();
                }
                else if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                {
                    cte.ValorPrestacaoAFRMM = valorPrestacaoServico;
                    cte.ValorAdicionalAFRMM = 0;
                    if (cteComplementado.Navio != null)
                        cte.Navio = new Dominio.ObjetosDeValor.CTe.Navio()
                        {
                            CodigoNavio = cteComplementado.Navio.Codigo
                        };
                    if (cteComplementado.Viagem != null)
                        cte.NumeroViagem = cteComplementado.Viagem.NumeroViagem.ToString("D");
                    if (cteComplementado.Viagem != null)
                        cte.Direcao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(cteComplementado.Viagem.DirecaoViagemMultimodal);

                    cte.Balsas = new List<Dominio.ObjetosDeValor.CTe.Balsa>();
                    cte.Containeres = new List<Dominio.ObjetosDeValor.CTe.Container>();
                }
                cte.NumeroBooking = cteComplementado.NumeroBooking;
                cte.NumeroOS = cteComplementado.NumeroOS;
                cte.Embarque = cteComplementado.Embarque;
                cte.MasterBL = cteComplementado.MasterBL;
                cte.NumeroDI = cteComplementado.NumeroDI;
                cte.BookingReference = cteComplementado.BookingReference;
                cte.SVMTerceiro = cteComplementado.SVMTerceiro;
                cte.SVMProprio = cteComplementado.SVMProprio;
                cte.TipoOSConvertido = cteComplementado.TipoOSConvertido;
                cte.TipoOS = cteComplementado.TipoOS;
                if (cteComplementado.ClienteProvedorOS != null)
                {
                    cte.ClienteProvedorOS = new Dominio.ObjetosDeValor.CTe.Cliente()
                    {
                        Bairro = cteComplementado.ClienteProvedorOS.Bairro,
                        CEP = cteComplementado.ClienteProvedorOS.CEP,
                        CodigoAtividade = cteComplementado.ClienteProvedorOS.Atividade?.Codigo ?? 0,
                        CodigoIBGECidade = cteComplementado.ClienteProvedorOS.Localidade.CodigoIBGE,
                        Complemento = cteComplementado.ClienteProvedorOS.Complemento,
                        CPFCNPJ = cteComplementado.ClienteProvedorOS.CPF_CNPJ_SemFormato,
                        Endereco = cteComplementado.ClienteProvedorOS.Endereco,
                        NomeFantasia = cteComplementado.ClienteProvedorOS.NomeFantasia,
                        Numero = cteComplementado.ClienteProvedorOS.Numero,
                        Exportacao = false,
                        RazaoSocial = cteComplementado.ClienteProvedorOS.Nome,
                        RGIE = cteComplementado.ClienteProvedorOS.IE_RG,
                        Telefone1 = cteComplementado.ClienteProvedorOS.Telefone1,
                        Emails = cteComplementado.ClienteProvedorOS.Email,
                        StatusEmails = true
                    };
                }
                cte.DescricaoCarrier = cteComplementado.DescricaoCarrier;
                cte.TipoPropostaFeeder = cteComplementado.TipoPropostaFeeder;
                cte.NumeroControle = "";

                origem = cteComplementado.LocalidadeInicioPrestacao;
                destino = cteComplementado.LocalidadeTerminoPrestacao;
            }
            if (cargaPedido != null && cargaPedido.Pedido != null)
            {
                if (cargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder)
                {
                    if (cargaPedido.Pedido?.TerminalOrigem?.Terminal?.Localidade != null)
                        origem = cargaPedido.Pedido?.TerminalOrigem?.Terminal?.Localidade;
                    else if (cargaPedido.Pedido?.TerminalOrigem?.Porto?.Localidade != null)
                        origem = cargaPedido.Pedido?.TerminalOrigem?.Porto?.Localidade;
                }
                else if (cargaPedido.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorta || cargaPedido.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto)
                {
                    if (cargaPedido.Pedido?.TerminalOrigem?.Porto?.Localidade != null)
                        origem = cargaPedido.Pedido?.TerminalOrigem?.Porto?.Localidade;
                }
                if (cargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder)
                {
                    if (cargaPedido.Pedido?.TerminalDestino?.Terminal?.Localidade != null)
                        destino = cargaPedido.Pedido?.TerminalDestino?.Terminal?.Localidade;
                    else if (cargaPedido.Pedido?.TerminalDestino?.Porto?.Localidade != null)
                        destino = cargaPedido.Pedido?.TerminalDestino?.Porto?.Localidade;
                }
                else if (cargaPedido.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortaPorto || cargaPedido.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto)
                {
                    if (cargaPedido.Pedido?.TerminalDestino?.Porto?.Localidade != null)
                        destino = cargaPedido.Pedido?.TerminalDestino?.Porto?.Localidade;
                }

                if (cargaPedido.Pedido?.Porto != null)
                    cte.PortoOrigem = new Dominio.ObjetosDeValor.CTe.Porto()
                    {
                        Codigo = cargaPedido.Pedido.Porto.Codigo
                    };
                if (cargaPedido.Pedido?.PortoDestino != null)
                    cte.PortoDestino = new Dominio.ObjetosDeValor.CTe.Porto()
                    {
                        Codigo = cargaPedido.Pedido.PortoDestino.Codigo
                    };
                if (cargaPedido.Pedido?.TerminalOrigem != null)
                    cte.TerminalOrigem = new Dominio.ObjetosDeValor.CTe.Terminal()
                    {
                        CodigoTerminal = cargaPedido.Pedido.TerminalOrigem.Codigo
                    };
                if (cargaPedido.Pedido?.TerminalDestino != null)
                    cte.TerminalDestino = new Dominio.ObjetosDeValor.CTe.Terminal()
                    {
                        CodigoTerminal = cargaPedido.Pedido.TerminalDestino.Codigo
                    };
                if (cargaPedido.Carga?.PedidoViagemNavio != null)
                    cte.Viagem = new Dominio.ObjetosDeValor.CTe.Viagem()
                    {
                        CodigoViagem = cargaPedido.Carga.PedidoViagemNavio.Codigo
                    };


                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> pedidoTransbordos = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);

                if (cargaPedido.Pedido != null && pedidoTransbordos.Count > 0)
                {
                    var listaTransbordo = (from obj in pedidoTransbordos orderby obj.Sequencia select obj).ToList();
                    int sequencia = 1;
                    foreach (var transbordo in listaTransbordo)
                    {
                        if (sequencia == 1)
                        {
                            cte.PortoPassagemUm = new Dominio.ObjetosDeValor.CTe.Porto()
                            {
                                Codigo = transbordo.Porto.Codigo
                            };
                            cte.ViagemPassagemUm = new Dominio.ObjetosDeValor.CTe.Viagem()
                            {
                                CodigoViagem = transbordo.PedidoViagemNavio.Codigo
                            };
                        }
                        else if (sequencia == 2)
                        {
                            cte.PortoPassagemDois = new Dominio.ObjetosDeValor.CTe.Porto()
                            {
                                Codigo = transbordo.Porto.Codigo
                            };
                            cte.ViagemPassagemDois = new Dominio.ObjetosDeValor.CTe.Viagem()
                            {
                                CodigoViagem = transbordo.PedidoViagemNavio.Codigo
                            };
                        }
                        else if (sequencia == 3)
                        {
                            cte.PortoPassagemTres = new Dominio.ObjetosDeValor.CTe.Porto()
                            {
                                Codigo = transbordo.Porto.Codigo
                            };
                            cte.ViagemPassagemTres = new Dominio.ObjetosDeValor.CTe.Viagem()
                            {
                                CodigoViagem = transbordo.PedidoViagemNavio.Codigo
                            };
                        }
                        else if (sequencia == 4)
                        {
                            cte.PortoPassagemQuatro = new Dominio.ObjetosDeValor.CTe.Porto()
                            {
                                Codigo = transbordo.Porto.Codigo
                            };
                            cte.ViagemPassagemQuatro = new Dominio.ObjetosDeValor.CTe.Viagem()
                            {
                                CodigoViagem = transbordo.PedidoViagemNavio.Codigo
                            };
                        }
                        else if (sequencia == 5)
                        {
                            cte.PortoPassagemCinco = new Dominio.ObjetosDeValor.CTe.Porto()
                            {
                                Codigo = transbordo.Porto.Codigo
                            };
                            cte.ViagemPassagemCinco = new Dominio.ObjetosDeValor.CTe.Viagem()
                            {
                                CodigoViagem = transbordo.PedidoViagemNavio.Codigo
                            };
                        }
                        sequencia += 1;
                    }
                }

                if (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalProprio || cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                {
                    cte.TipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                    tipoServico = cte.TipoServico;
                }
                else if (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.RedespachoIntermediario)
                {
                    if (cargaPedido?.Expedidor != null)
                        cte.TipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                    else
                        cte.TipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                    tipoServico = cte.TipoServico;
                }
                else if (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao)
                {
                    cte.TipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
                    tipoServico = cte.TipoServico;
                }

                if (cargaPedido.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTeRodoviario)
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
                else if (cargaPedido.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTeMultimodal)
                {
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal;
                    cte.NumeroCOTM = cargaPedido.Carga?.Empresa?.COTM ?? "";
                    if (cargaPedido.Pedido?.Navio != null)
                        cte.Navio = new Dominio.ObjetosDeValor.CTe.Navio()
                        {
                            CodigoNavio = cargaPedido.Pedido.Navio.Codigo
                        };
                    if (cargaPedido.Carga?.PedidoViagemNavio != null)
                        cte.NumeroViagem = cargaPedido.Carga.PedidoViagemNavio.NumeroViagem.ToString("D");
                }
                else if (cargaPedido.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTEAquaviario)
                {
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario;
                    cte.ValorPrestacaoAFRMM = valorPrestacaoServico;
                    cte.ValorAdicionalAFRMM = 0;
                    if (cargaPedido.Pedido?.Navio != null)
                        cte.Navio = new Dominio.ObjetosDeValor.CTe.Navio()
                        {
                            CodigoNavio = cargaPedido.Pedido.Navio.Codigo
                        };
                    if (cargaPedido.Pedido?.Navio == null && cargaPedido.Carga?.Navio != null)
                    {
                        cte.Navio = new Dominio.ObjetosDeValor.CTe.Navio()
                        {
                            CodigoNavio = cargaPedido.Carga.Navio.Codigo
                        };
                    }
                    if (cargaPedido.Carga?.PedidoViagemNavio != null)
                        cte.NumeroViagem = cargaPedido.Carga.PedidoViagemNavio.NumeroViagem.ToString("D");
                    if (cargaPedido.Pedido != null)
                        cte.Direcao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodalHelper.ObterAbreviacao(cargaPedido.Pedido.DirecaoViagemMultimodal);
                    cte.Balsas = new List<Dominio.ObjetosDeValor.CTe.Balsa>();
                    if (cargaPedido.Carga?.Balsa != null)
                    {
                        cte.Balsas.Add(new Dominio.ObjetosDeValor.CTe.Balsa
                        {
                            Codigo = 0,
                            Descricao = cargaPedido.Carga.Balsa.Descricao
                        });
                    }
                }

                cte.Containeres = new List<Dominio.ObjetosDeValor.CTe.Container>();
                if (cargasPedidos != null && cargasPedidos.Count > 0)
                {
                    foreach (var cargaPed in cargasPedidos)
                    {
                        if (cargaPed.Pedido != null && cargaPed.Pedido.Container != null && cargaPed.Pedido?.Container?.Codigo > 0)
                        {
                            if (codigosContainer.Contains(cargaPed.Pedido.Container.Codigo))
                                continue;
                            else
                                codigosContainer.Add(cargaPed.Pedido.Container.Codigo);
                        }
                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = null;
                        if (chavesCTesAnterior != null && chavesCTesAnterior.Count > 0)
                            notas = repCTe.BuscarXMLNotaFiscalPorChaves(chavesCTesAnterior);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> documentos = null;
                        if (notas == null || notas.Count == 0)
                        {
                            if (documentos == null || documentos.Count == 0)
                                documentos = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPed.Codigo, cnpjDestinatarioContainer, "", "");
                            if (documentos == null || documentos.Count == 0)
                                documentos = repPedidoXMLNotaFiscal.BuscarPorContainer(cargaPed.Pedido?.Container?.Codigo ?? 0, 0);
                            if (documentos == null || documentos.Count == 0)
                                documentos = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPed.Codigo);
                        }

                        if ((documentos != null && documentos.Count > 0) || (notas != null && notas.Count > 0))
                        {
                            if (cargaPed.Pedido.Container != null && !codigosContainer.Contains(cargaPed.Pedido.Container.Codigo))
                                codigosContainer.Add(cargaPed.Pedido?.Container?.Codigo ?? 0);
                            Dominio.ObjetosDeValor.CTe.Container container = new Dominio.ObjetosDeValor.CTe.Container()
                            {
                                CodigoContainer = cargaPed.Pedido?.Container?.Codigo ?? 0,
                                DataPrevista = cargaPed.Pedido?.DataPrevisaoChegadaDestinatario,
                                Lacre1 = Utilidades.String.SanitizeString((cargaPed.Pedido?.LacreContainerUm ?? "").Trim()),
                                Lacre2 = Utilidades.String.SanitizeString((cargaPed.Pedido?.LacreContainerDois ?? "").Trim()),
                                Lacre3 = Utilidades.String.SanitizeString((cargaPed.Pedido?.LacreContainerTres ?? "").Trim()),
                                Numero = Utilidades.String.SanitizeString(cargaPed.Pedido?.Container?.Numero ?? ""),
                                Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
                            };
                            if (notas != null && notas.Count > 0)
                            {
                                bool contemDocumentosNoCTe = false;
                                if (cte.Documentos != null && cte.Documentos.Count > 0)
                                    contemDocumentosNoCTe = true;

                                foreach (var docm in notas)
                                {
                                    if (docm != null)
                                    {
                                        if (contemDocumentosNoCTe)
                                        {
                                            if (cte.Documentos.Any(d => d.ChaveNFE == docm.Chave))
                                            {
                                                if (container.Documentos != null && container.Documentos.Count > 0)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(docm.Chave) && container.Documentos.Any(d => d.ChaveNFE == docm.Chave))
                                                        continue;
                                                    else if (string.IsNullOrWhiteSpace(docm.Chave) && docm.Numero > 0 && container.Documentos.Any(d => d.Numero == docm.Numero.ToString("D")))
                                                        continue;
                                                }

                                                container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                                {
                                                    ChaveNFE = docm.Chave,
                                                    Numero = docm.Numero.ToString("D"),
                                                    Serie = docm.Serie,
                                                    Tipo = docm.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                                    Peso = docm.Peso,
                                                    Volume = docm.Volumes,
                                                    NumeroReferenciaEDI = docm.NumeroReferenciaEDI,
                                                    NumeroControleCliente = docm.NumeroControleCliente,
                                                    NCMPredominante = docm.NCM
                                                });
                                            }
                                        }
                                        else
                                        {
                                            if (container.Documentos != null && container.Documentos.Count > 0)
                                            {
                                                if (!string.IsNullOrWhiteSpace(docm.Chave) && container.Documentos.Any(d => d.ChaveNFE == docm.Chave))
                                                    continue;
                                                else if (string.IsNullOrWhiteSpace(docm.Chave) && docm.Numero > 0 && container.Documentos.Any(d => d.Numero == docm.Numero.ToString("D")))
                                                    continue;
                                            }

                                            container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                            {
                                                ChaveNFE = docm.Chave,
                                                Numero = docm.Numero.ToString("D"),
                                                Serie = docm.Serie,
                                                Tipo = docm.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                                Peso = docm.Peso,
                                                Volume = docm.Volumes,
                                                NumeroReferenciaEDI = docm.NumeroReferenciaEDI,
                                                NumeroControleCliente = docm.NumeroControleCliente,
                                                NCMPredominante = docm.NCM
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var docm in documentos)
                                {
                                    if (docm.XMLNotaFiscal != null)
                                    {
                                        if (cte.Documentos != null && cte.Documentos.Count > 0 && !string.IsNullOrWhiteSpace(docm.XMLNotaFiscal.Chave) && !cte.Documentos.Any(d => d.ChaveNFE == docm.XMLNotaFiscal.Chave))
                                            continue;
                                        else if (cte.Documentos != null && cte.Documentos.Count > 0 && string.IsNullOrWhiteSpace(docm.XMLNotaFiscal.Chave) && docm.XMLNotaFiscal.Numero > 0 && !cte.Documentos.Any(d => d.Numero == docm.XMLNotaFiscal.Numero.ToString("D")))
                                            continue;

                                        if (container.Documentos != null && container.Documentos.Count > 0)
                                        {
                                            if (!string.IsNullOrWhiteSpace(docm.XMLNotaFiscal.Chave) && container.Documentos.Any(d => d.ChaveNFE == docm.XMLNotaFiscal.Chave))
                                                continue;
                                            else if (string.IsNullOrWhiteSpace(docm.XMLNotaFiscal.Chave) && docm.XMLNotaFiscal.Numero > 0 && container.Documentos.Any(d => d.Numero == docm.XMLNotaFiscal.Numero.ToString("D")))
                                                continue;
                                        }
                                        container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                        {
                                            ChaveNFE = docm.XMLNotaFiscal.Chave,
                                            Numero = docm.XMLNotaFiscal.Numero.ToString("D"),
                                            Serie = docm.XMLNotaFiscal.Serie,
                                            Tipo = docm.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                            Peso = docm.XMLNotaFiscal.Peso,
                                            Volume = docm.XMLNotaFiscal.Volumes,
                                            NumeroReferenciaEDI = docm.XMLNotaFiscal.NumeroReferenciaEDI,
                                            NumeroControleCliente = docm.XMLNotaFiscal.NumeroControleCliente,
                                            NCMPredominante = docm.XMLNotaFiscal.NCM
                                        });
                                    }
                                }
                            }
                            if (container != null && container.Documentos != null && container.Documentos.Count > 0)
                                cte.Containeres.Add(container);
                        }
                    }
                }
                else if (cargaPedido.Pedido?.Container != null)
                {
                    codigosContainer.Add(cargaPedido.Pedido?.Container?.Codigo ?? 0);
                    Dominio.ObjetosDeValor.CTe.Container container = new Dominio.ObjetosDeValor.CTe.Container()
                    {
                        CodigoContainer = cargaPedido.Pedido.Container.Codigo,
                        DataPrevista = cargaPedido.Pedido?.DataPrevisaoChegadaDestinatario,
                        Lacre1 = Utilidades.String.SanitizeString((cargaPedido.Pedido?.LacreContainerUm ?? "").Trim()),
                        Lacre2 = Utilidades.String.SanitizeString((cargaPedido.Pedido?.LacreContainerDois ?? "").Trim()),
                        Lacre3 = Utilidades.String.SanitizeString((cargaPedido.Pedido?.LacreContainerTres ?? "").Trim()),
                        Numero = Utilidades.String.SanitizeString((cargaPedido.Pedido?.Container.Numero ?? "").Trim()),
                        Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
                    };

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = null;
                    if (chavesCTesAnterior != null && chavesCTesAnterior.Count > 0)
                        notas = repCTe.BuscarXMLNotaFiscalPorChaves(chavesCTesAnterior);

                    bool contemNFe = false;
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> documentos = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                    if (documentos == null || documentos.Count == 0)
                        documentos = repPedidoXMLNotaFiscal.BuscarPorContainer(cargaPedido.Pedido.Container.Codigo, cnpjDestinatarioContainer);
                    if (documentos != null && documentos.Count > 0)
                        contemNFe = documentos.Any(o => o.XMLNotaFiscal.Modelo == "55");
                    if (notas != null && notas.Count > 0)
                        contemNFe = notas.Any(o => o.Modelo == "55");

                    if (notas != null && notas.Count > 0)
                    {
                        bool contemDocumentosNoCTe = false;
                        if (cte.Documentos != null && cte.Documentos.Count > 0)
                            contemDocumentosNoCTe = true;

                        foreach (var docm in notas)
                        {
                            if (docm != null)
                            {
                                if (contemDocumentosNoCTe)
                                {
                                    if (cte.Documentos.Any(d => d.ChaveNFE == docm.Chave))
                                    {
                                        if (contemNFe && docm.Modelo == "55")
                                            container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                            {
                                                ChaveNFE = docm.Chave,
                                                Numero = docm.Numero.ToString("D"),
                                                Serie = docm.Serie,
                                                Tipo = docm.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                                Peso = docm.Peso,
                                                Volume = docm.Volumes,
                                                NumeroReferenciaEDI = docm.NumeroReferenciaEDI,
                                                NumeroControleCliente = docm.NumeroControleCliente,
                                                NCMPredominante = docm.NCM
                                            });
                                        else if (!contemNFe)
                                            container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                            {
                                                ChaveNFE = docm.Chave,
                                                Numero = docm.Numero.ToString("D"),
                                                Serie = docm.Serie,
                                                Tipo = docm.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                                Peso = docm.Peso,
                                                Volume = docm.Volumes,
                                                NumeroReferenciaEDI = docm.NumeroReferenciaEDI,
                                                NumeroControleCliente = docm.NumeroControleCliente,
                                            });
                                    }
                                }
                                else
                                {
                                    if (contemNFe && docm.Modelo == "55")
                                        container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                        {
                                            ChaveNFE = docm.Chave,
                                            Numero = docm.Numero.ToString("D"),
                                            Serie = docm.Serie,
                                            Tipo = docm.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                            Peso = docm.Peso,
                                            Volume = docm.Volumes,
                                            NumeroReferenciaEDI = docm.NumeroReferenciaEDI,
                                            NumeroControleCliente = docm.NumeroControleCliente,
                                            NCMPredominante = docm.NCM
                                        });
                                    else if (!contemNFe)
                                        container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                        {
                                            ChaveNFE = docm.Chave,
                                            Numero = docm.Numero.ToString("D"),
                                            Serie = docm.Serie,
                                            Tipo = docm.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                            Peso = docm.Peso,
                                            Volume = docm.Volumes,
                                            NumeroReferenciaEDI = docm.NumeroReferenciaEDI,
                                            NumeroControleCliente = docm.NumeroControleCliente,
                                            NCMPredominante = docm.NCM
                                        });
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var docm in documentos)
                        {
                            if (docm.XMLNotaFiscal != null)
                            {
                                if (contemNFe && docm.XMLNotaFiscal.Modelo == "55")
                                    container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                    {
                                        ChaveNFE = docm.XMLNotaFiscal.Chave,
                                        Numero = docm.XMLNotaFiscal.Numero.ToString("D"),
                                        Serie = docm.XMLNotaFiscal.Serie,
                                        Tipo = docm.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                        Peso = docm.XMLNotaFiscal.Peso,
                                        Volume = docm.XMLNotaFiscal.Volumes,
                                        NumeroReferenciaEDI = docm.XMLNotaFiscal.NumeroReferenciaEDI,
                                        NumeroControleCliente = docm.XMLNotaFiscal.NumeroControleCliente,
                                        NCMPredominante = docm.XMLNotaFiscal.NCM
                                    });
                                else if (!contemNFe)
                                    container.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento()
                                    {
                                        ChaveNFE = docm.XMLNotaFiscal.Chave,
                                        Numero = docm.XMLNotaFiscal.Numero.ToString("D"),
                                        Serie = docm.XMLNotaFiscal.Serie,
                                        Tipo = docm.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                        Peso = docm.XMLNotaFiscal.Peso,
                                        Volume = docm.XMLNotaFiscal.Volumes,
                                        NumeroReferenciaEDI = docm.XMLNotaFiscal.NumeroReferenciaEDI,
                                        NumeroControleCliente = docm.XMLNotaFiscal.NumeroControleCliente,
                                        NCMPredominante = docm.XMLNotaFiscal.NCM
                                    });
                            }
                        }
                    }
                    cte.Containeres.Add(container);
                }

                cte.NumeroOS = cargaPedido.Pedido.NumeroOS;
                cte.Embarque = cargaPedido.Pedido.Embarque;
                cte.MasterBL = cargaPedido.Pedido.MasterBL;
                cte.NumeroDI = cargaPedido.Pedido.NumeroDI;
                cte.BookingReference = cargaPedido.Pedido.BookingReference;

                if (cargaPedido.Pedido.CentroDeCustoViagem != null)
                    cte.CentroDeCustoViagem = new Dominio.ObjetosDeValor.CTe.CentroCustoViagem()
                    {
                        Codigo = cargaPedido.Pedido.CentroDeCustoViagem.Codigo
                    };

                cte.SVMTerceiro = cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalTerceiro;
                cte.SVMProprio = cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalProprio;
                if (cargaPedido.Pedido.ProvedorOS != null)
                {
                    cte.ClienteProvedorOS = new Dominio.ObjetosDeValor.CTe.Cliente()
                    {
                        Bairro = cargaPedido.Pedido.ProvedorOS.Bairro,
                        CEP = cargaPedido.Pedido.ProvedorOS.CEP,
                        CodigoAtividade = cargaPedido.Pedido.ProvedorOS.Atividade?.Codigo ?? 0,
                        CodigoIBGECidade = cargaPedido.Pedido.ProvedorOS.Localidade.CodigoIBGE,
                        Complemento = cargaPedido.Pedido.ProvedorOS.Complemento,
                        CPFCNPJ = cargaPedido.Pedido.ProvedorOS.CPF_CNPJ_SemFormato,
                        Endereco = cargaPedido.Pedido.ProvedorOS.Endereco,
                        NomeFantasia = cargaPedido.Pedido.ProvedorOS.NomeFantasia,
                        Numero = cargaPedido.Pedido.ProvedorOS.Numero,
                        Exportacao = false,
                        RazaoSocial = cargaPedido.Pedido.ProvedorOS.Nome,
                        RGIE = cargaPedido.Pedido.ProvedorOS.IE_RG,
                        Telefone1 = cargaPedido.Pedido.ProvedorOS.Telefone1,
                        Emails = cargaPedido.Pedido.ProvedorOS.Email,
                        StatusEmails = true
                    };
                }
                cte.NumeroBooking = cargaPedido.Pedido.NumeroBooking;
                cte.DescricaoCarrier = cargaPedido.Pedido.DescricaoCarrierNavioViagem;
                cte.TipoPropostaFeeder = cargaPedido.Pedido.TipoPropostaFeeder;
                if (!naoCalcularTaxaEmissao && !string.IsNullOrWhiteSpace(cte.NumeroBooking) && cargaPedido.Pedido.ValorTaxaDocumento > 0 && cargaPedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao >= 0)
                {

                    decimal valorTaxaComImposto = cargaPedido.Pedido.ValorTaxaDocumento;

                    Servicos.Log.GravarInfo("Lancando Taxa de Documento vl " + valorTaxaComImposto.ToString("n2"), "SetarInformacoesModal");

                    if (cargaPedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao == 0)
                    {
                        valorTaxaEmissao += valorTaxaComImposto;
                        cte.ValorAReceber += valorTaxaComImposto;
                        cte.ValorTotalPrestacaoServico += valorTaxaComImposto;
                        AdicionarTaxaEmissaoCarga(cte, valorTaxaComImposto, cargaPedido, unitOfWork);
                    }
                    else
                    {
                        //int sequencia = repCTe.BuscarSequenciaCTeBooking(0, cte.NumeroBooking, cte.TipoServico);
                        if (codigosContainer != null && codigosContainer.Count > 0)
                        {
                            codigosContainer = codigosContainer.Distinct().ToList();
                            foreach (var codigoContainer in codigosContainer)
                            {
                                int qtdConhecimentosNoContainer = repContainerCTE.BuscarQuantidadeCTeContainer(codigoContainer, cte.NumeroBooking);
                                qtdConhecimentosNoContainer++;
                                Servicos.Log.GravarInfo("Lancando Taxa de Documento vl " + valorTaxaComImposto.ToString("n2") + " qtd " + qtdConhecimentosNoContainer.ToString() + " ct " + codigoContainer.ToString() + " bk " + cte.NumeroBooking, "SetarInformacoesModal");
                                if (qtdConhecimentosNoContainer > cargaPedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao)
                                {
                                    Servicos.Log.GravarInfo("Valor Lancado Taxa de Documento vl " + valorTaxaComImposto.ToString("n2") + " qtd " + qtdConhecimentosNoContainer.ToString() + " ct " + codigoContainer.ToString() + " bk " + cte.NumeroBooking, "SetarInformacoesModal");
                                    valorTaxaEmissao += valorTaxaComImposto;
                                    cte.ValorAReceber += valorTaxaComImposto;
                                    cte.ValorTotalPrestacaoServico += valorTaxaComImposto;
                                    AdicionarTaxaEmissaoCarga(cte, valorTaxaComImposto, cargaPedido, unitOfWork);
                                }
                            }
                        }
                    }
                }
                cte.NumeroControle = "";
            }
        }

        private void SetarInformacoesCTe(ref Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, ref Dominio.Entidades.Localidade origem, ref Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRementente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades, string observacaoCTe, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, List<string> rotas, List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro, Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS, Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Enumeradores.TipoServico tipoServico, Dominio.Enumeradores.TipoCTE tipoCTe, List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores, Dominio.Entidades.Cliente cliTomador, int tipoEnvio, bool sempreEmitirAutomaticamente, bool UtilizarRecebedorApenasComoParticipante, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, ref string observacaoRegraICMS, decimal horasPrevistasParaEntrega = 0m, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor = null, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = null, Dominio.Entidades.ServicoNFSe servicoNFSe = null)
        {
            Servicos.Log.GravarInfo("1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            Servicos.Cliente serCliente = new Servicos.Cliente(StringConexao);
            Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Transportadores.ConfiguracaoTipoOperacao repConfiguracaoTipoOperacao = new Repositorio.Embarcador.Transportadores.ConfiguracaoTipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Servicos.Log.GravarInfo("2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");

            if (configuracaoEmbarcador == null)
                configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            if (enderecoRecebedor != null)
                enderecoRecebedor = repPedidoEndereco.BuscarPorCodigo(enderecoRecebedor.Codigo);
            if (enderecoExpedidor != null)
                enderecoExpedidor = repPedidoEndereco.BuscarPorCodigo(enderecoExpedidor.Codigo);

            Servicos.Log.GravarInfo("3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            if (tipoOperacao == null || !tipoOperacao.IndicadorGlobalizadoRemetente)
                cte.Remetente = serCliente.ObterClienteCTE(remetente, enderecoRementente != null && enderecoRementente.ClienteOutroEndereco != null ? enderecoRementente : null);
            else
            {
                Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(serPessoa.ConverterObjetoEmpresa(empresa), "Transportador Remetente Globalizado", unitOfWork, 0, false);
                if (retorno.Status == true)
                {
                    cte.Remetente = serCliente.ObterClienteCTE(retorno.cliente, null);
                    if (cte.indicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        cte.Remetente.RazaoSocial = "DIVERSOS";
                    cte.Remetente.NaoAtualizarEndereco = true;
                }
                else
                    cte.Remetente = serCliente.ObterClienteCTE(remetente, enderecoRementente);
            }
            Servicos.Log.GravarInfo("4 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            if (cte.indicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Nao)
            {
                cte.Destinatario = serCliente.ObterClienteCTE(destinatario, enderecoDestinatario != null && enderecoDestinatario.ClienteOutroEndereco != null ? enderecoDestinatario : null);

                if (tipoOperacao?.UtilizarNomeDestinatarioNotaFiscalParaEmitirCTe ?? false)
                {
                    if (cte.Documentos != null && cte.Documentos.Count > 0)
                    {
                        string nomeDestinatario = cte.Documentos.Select(o => o.NomeDestinatario).FirstOrDefault();
                        string ieDestinatario = cte.Documentos.Select(o => o.IEDestinatario).FirstOrDefault();
                        string ieRemetente = cte.Documentos.Select(o => o.IERemetente).FirstOrDefault();

                        if (cte.Destinatario != null && !string.IsNullOrWhiteSpace(nomeDestinatario))
                        {
                            cte.Destinatario.RazaoSocial = nomeDestinatario;
                            cte.Destinatario.NomeFantasia = nomeDestinatario;
                            cte.Destinatario.NaoAtualizarDadosCadastrais = true;
                        }
                        if (cte.Destinatario != null && !string.IsNullOrWhiteSpace(ieDestinatario) && (!string.IsNullOrWhiteSpace(cte.Destinatario.RGIE) || cte.Destinatario.RGIE != ieDestinatario))
                        {
                            cte.Destinatario.RGIE = ieDestinatario;
                            cte.Destinatario.NaoAtualizarDadosCadastrais = true;
                        }
                        if (cte.Remetente != null && !string.IsNullOrWhiteSpace(ieRemetente) && (string.IsNullOrWhiteSpace(cte.Remetente.RGIE) || cte.Remetente.RGIE != ieRemetente))
                        {
                            cte.Remetente.RGIE = ieRemetente;
                            cte.Remetente.NaoAtualizarDadosCadastrais = true;
                        }
                    }
                }
            }
            else
            {
                cte.Destinatario = serCliente.ObterClienteCTE(destinatario, null);
                cte.Destinatario.RazaoSocial = "DIVERSOS";
                cte.Destinatario.NaoAtualizarEndereco = true;
            }
            Servicos.Log.GravarInfo("5 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            cte.Emitente = Servicos.Empresa.ObterEmpresaCTE(empresa);
            Servicos.Log.GravarInfo("6 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            int IBGECidadeInicioPrestacao = origem.CodigoIBGE;
            int IBGECidadeTerminoPrestacao = destino.CodigoIBGE;

            Servicos.Log.GravarInfo("7 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            //if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS
            //    && modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
            //{
            if (expedidor != null && recebedor != null)
            {
                if ((carga?.TipoOperacao?.PermitirExpedidorRecebedorIgualRemetenteDestinatario ?? false) || (configuracaoEmbarcador.PermitirRecebedorIgualDestinatario && ((tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoServico == Dominio.Enumeradores.TipoServico.Redespacho || tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento) || (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal || configuracaoEmbarcador.UtilizaEmissaoMultimodal))))
                    cte.Recebedor = serCliente.ObterClienteCTE(recebedor, enderecoRecebedor);
                else if ((tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoServico == Dominio.Enumeradores.TipoServico.Redespacho || tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || recebedor.CPF_CNPJ != destinatario.CPF_CNPJ || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento || tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor) || (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal || configuracaoEmbarcador.UtilizaEmissaoMultimodal))
                    cte.Recebedor = serCliente.ObterClienteCTE(recebedor, enderecoRecebedor);
                if (tipoCTe != Dominio.Enumeradores.TipoCTE.Complemento && !UtilizarRecebedorApenasComoParticipante && cte.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                    IBGECidadeTerminoPrestacao = recebedor.Localidade.CodigoIBGE;

                if ((carga?.TipoOperacao?.PermitirExpedidorRecebedorIgualRemetenteDestinatario ?? false) || (configuracaoEmbarcador.PermitirExpedidorIgualRemetente && ((tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoServico == Dominio.Enumeradores.TipoServico.Redespacho || tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento) || (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal || configuracaoEmbarcador.UtilizaEmissaoMultimodal))))
                    cte.Expedidor = serCliente.ObterClienteCTE(expedidor, enderecoExpedidor);
                else if ((tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoServico == Dominio.Enumeradores.TipoServico.Redespacho || tipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || expedidor.CPF_CNPJ != remetente.CPF_CNPJ || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento || tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor) || (tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal || configuracaoEmbarcador.UtilizaEmissaoMultimodal))
                    cte.Expedidor = serCliente.ObterClienteCTE(expedidor, enderecoExpedidor);
                if (tipoCTe != Dominio.Enumeradores.TipoCTE.Complemento && cte.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                    IBGECidadeInicioPrestacao = expedidor.Localidade.CodigoIBGE;

                if (cte.Recebedor != null && recebedor.Localidade != null && enderecoRecebedor != null && enderecoRecebedor.Localidade != null && !UtilizarRecebedorApenasComoParticipante)
                {
                    IBGECidadeTerminoPrestacao = enderecoRecebedor.Localidade.CodigoIBGE;
                    destino = enderecoRecebedor.Localidade;
                }
                if (cte.Expedidor != null && expedidor.Localidade != null && enderecoExpedidor != null && enderecoExpedidor.Localidade != null)
                {
                    IBGECidadeInicioPrestacao = enderecoExpedidor.Localidade.CodigoIBGE;
                    origem = enderecoExpedidor.Localidade;
                }
                else if (cte.Expedidor != null && expedidor.Localidade != null && modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) //nota fiscal de serviço eletronica.)
                {
                    IBGECidadeInicioPrestacao = expedidor.Localidade.CodigoIBGE;
                    origem = expedidor.Localidade;
                }
            }
            else
            {
                if (recebedor != null)
                {
                    if ((carga?.TipoOperacao?.PermitirExpedidorRecebedorIgualRemetenteDestinatario ?? false) || (configuracaoEmbarcador.PermitirRecebedorIgualDestinatario && ((tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento) || tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)))
                        cte.Recebedor = serCliente.ObterClienteCTE(recebedor, enderecoRecebedor);
                    else if ((tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || recebedor.CPF_CNPJ != destinatario.CPF_CNPJ || tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento) || tipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                        cte.Recebedor = serCliente.ObterClienteCTE(recebedor, enderecoRecebedor);
                    if (tipoCTe != Dominio.Enumeradores.TipoCTE.Complemento && !UtilizarRecebedorApenasComoParticipante && cte.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                        IBGECidadeTerminoPrestacao = recebedor.Localidade.CodigoIBGE;

                    if (cte.Recebedor != null && recebedor.Localidade != null && enderecoRecebedor != null && enderecoRecebedor.Localidade != null && !UtilizarRecebedorApenasComoParticipante)
                    {
                        IBGECidadeTerminoPrestacao = enderecoRecebedor.Localidade.CodigoIBGE;
                        destino = enderecoRecebedor.Localidade;
                    }
                }

                if (expedidor != null)
                {
                    cte.Expedidor = serCliente.ObterClienteCTE(expedidor, enderecoExpedidor);

                    if (tipoCTe != Dominio.Enumeradores.TipoCTE.Complemento && cte.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                        IBGECidadeInicioPrestacao = expedidor.Localidade.CodigoIBGE;

                    if (cte.Expedidor != null && expedidor.Localidade != null && enderecoExpedidor != null && enderecoExpedidor.Localidade != null)
                    {
                        IBGECidadeInicioPrestacao = enderecoExpedidor.Localidade.CodigoIBGE;
                        origem = enderecoExpedidor.Localidade;
                    }
                    else if (cte.Expedidor != null && expedidor.Localidade != null && modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) //nota fiscal de serviço eletronica.)
                    {
                        IBGECidadeInicioPrestacao = expedidor.Localidade.CodigoIBGE;
                        origem = expedidor.Localidade;
                    }
                }
            }

            if (configuracaoEmbarcador.ExpedidorIgualRemetente && cte.Expedidor == null && remetente != null)
                cte.Expedidor = serCliente.ObterClienteCTE(remetente, enderecoRementente);

            if (configuracaoEmbarcador.RecebedorIgualDestinatario && cte.Recebedor == null && destinatario != null)
                cte.Recebedor = serCliente.ObterClienteCTE(destinatario, enderecoDestinatario);

            Servicos.Log.GravarInfo("8 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            cte.CFOP = 0;
            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorarioPadrao);
            DateTime dataEmissao = DateTime.Now;
            dataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

            if (modeloDocumentoFiscal != null && ocorrencia != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros && ocorrencia.TipoOcorrencia.DataComplementoIgualDataOcorrencia)
                cte.DataEmissao = ocorrencia.DataOcorrencia.ToDateTimeString();
            else if (tipoOperacao != null && tipoOperacao.UtilizarDataNFeEmissaoDocumentos && tipoCTe == Dominio.Enumeradores.TipoCTE.Normal && cte.Documentos != null && cte.Documentos.Count > 0 && !string.IsNullOrWhiteSpace(cte.Documentos.FirstOrDefault().DataEmissao))
                cte.DataEmissao = cte.Documentos.FirstOrDefault().DataEmissao;
            //se estiver marcado essa opçao na operação sempre considera o ultimo dia do mês anterior, se necessário que exista outras datas criar uma configuração
            else if (tipoOperacao == null || !tipoOperacao.EmitirDocumentosRetroativamente)
                cte.DataEmissao = dataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
            else
            {
                var ultimoDiaMesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ultimoDiaMesAtual = ultimoDiaMesAtual.AddMonths(1).AddDays(-1);
                if (ultimoDiaMesAtual.Date != DateTime.Today)
                {
                    var primeiroDiaMesCorrente = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime ultimoDiaDoMesAnterior = primeiroDiaMesCorrente.AddDays(-1);
                    cte.DataEmissao = ultimoDiaDoMesAnterior.ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                    cte.DataEmissao = dataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (destino != null)
                {
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                    destino = repLocalidade.BuscarPorCodigo(destino.Codigo);

                    if (destino.Regiao != null && destino.Regiao.DiasPrazoEntrega > 0)
                        cte.DataPrevistaEntrega = dataEmissao.AddDays(destino.Regiao.DiasPrazoEntrega).ToString("dd/MM/yyyy HH:mm:ss");
                }

                if (horasPrevistasParaEntrega > 0m)
                    cte.DataPrevistaEntrega = dataEmissao.AddHours((double)horasPrevistasParaEntrega).ToString("dd/MM/yyyy HH:mm:ss");
            }
            Servicos.Log.GravarInfo("9 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");

            if (tipoOperacao != null && !string.IsNullOrWhiteSpace(tipoOperacao.ConfiguracaoEmissaoDocumento?.TipoConhecimentoProceda ?? ""))
                cte.TipoConhecimentoProceda = tipoOperacao.ConfiguracaoEmissaoDocumento?.TipoConhecimentoProceda ?? "";

            cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>();
            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade in quantidades)
            {
                Dominio.ObjetosDeValor.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga();
                quantidadeCarga.UnidadeMedida = string.Format("{0:00}", (int)quantidade.Unidade);
                quantidadeCarga.Quantidade = quantidade.Quantidade;
                quantidadeCarga.Descricao = quantidade.Medida;
                cte.QuantidadesCarga.Add(quantidadeCarga);
            }
            Servicos.Log.GravarInfo("10 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            if (empresa != null && tipoOperacao != null)
            {
                List<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao> configuracoesTipoOperacao = repConfiguracaoTipoOperacao.BuscarPorEmpresa(empresa.Codigo);

                Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao configuracao = (from obj in configuracoesTipoOperacao where obj.TipoOperacao.Codigo == tipoOperacao.Codigo select obj).FirstOrDefault();
                if (configuracao != null)
                {
                    if (origem.Estado.Sigla == destino.Estado.Sigla)
                    {
                        if (configuracao.SerieIntraestadual != null)
                            cte.Serie = configuracao.SerieIntraestadual.Numero;
                    }
                    else
                    {
                        if (configuracao.SerieInterestadual != null)
                            cte.Serie = configuracao.SerieInterestadual.Numero;
                    }
                }
            }
            Servicos.Log.GravarInfo("11 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            cte.Retira = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.Seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro>();

            if (empresa.EmpresaPai != null)
            {
                //todo: remover essa inserção após atualizar na tirol e criar o cadastro da apolice deles.
                Repositorio.ApoliceDeSeguro repApoliceDeSeguro = new Repositorio.ApoliceDeSeguro(unitOfWork);
                List<Dominio.Entidades.ApoliceDeSeguro> apolicesDeSeguro = repApoliceDeSeguro.BuscarPorCliente(empresa.Codigo, empresa.EmpresaPai.Codigo, 0);
                if (apolicesDeSeguro.Count > 0)
                {
                    foreach (Dominio.Entidades.ApoliceDeSeguro apolice in apolicesDeSeguro)
                    {
                        Dominio.ObjetosDeValor.CTe.Seguro seguro = new Dominio.ObjetosDeValor.CTe.Seguro() { CNPJSeguradora = apolice.CNPJSeguradora, NomeSeguradora = apolice.NomeSeguradora, NumeroAverbacao = "", Tipo = Dominio.Enumeradores.TipoSeguro.Remetente, NumeroApolice = apolice.NumeroApolice, Valor = cte.ValorTotalMercadoria };
                        cte.Seguros.Add(seguro);
                    }
                }
            }
            Servicos.Log.GravarInfo("12 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Seguro apolice in apolicesSeguro)
            {
                Dominio.ObjetosDeValor.CTe.Seguro seguro = new Dominio.ObjetosDeValor.CTe.Seguro() { CNPJSeguradora = apolice.CNPJSeguradora, NomeSeguradora = apolice.Seguradora, NumeroAverbacao = apolice.Averbacao, Tipo = apolice.ResponsavelSeguro, NumeroApolice = apolice.Apolice, Valor = cte.ValorTotalMercadoria };
                cte.Seguros.Add(seguro);
            }
            Servicos.Log.GravarInfo("13 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            if (cte.Seguros.Count == 0)
            {
                Dominio.ObjetosDeValor.CTe.Seguro seguro = new Dominio.ObjetosDeValor.CTe.Seguro() { CNPJSeguradora = "", NomeSeguradora = "", NumeroAverbacao = "", Tipo = empresa.EmpresaPai.Configuracao?.ResponsavelSeguro ?? Dominio.Enumeradores.TipoSeguro.Remetente, NumeroApolice = "", Valor = 0 };
                cte.Seguros.Add(seguro);
            }
            Servicos.Log.GravarInfo("14 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            cte.TipoCTe = tipoCTe;
            cte.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato;
            cte.TipoPagamento = tipoPagamento;

            if (configuracaoEmbarcador.UtilizarLocalidadePrestacaoPedido && cargaPedido?.Pedido?.LocalidadeInicioPrestacao != null)
                cte.CodigoIBGECidadeInicioPrestacao = cargaPedido.Pedido.LocalidadeInicioPrestacao.CodigoIBGE;
            else
                cte.CodigoIBGECidadeInicioPrestacao = IBGECidadeInicioPrestacao;

            if (configuracaoEmbarcador.UtilizarLocalidadePrestacaoPedido && cargaPedido?.Pedido?.LocalidadeTerminoPrestacao != null)
                cte.CodigoIBGECidadeTerminoPrestacao = cargaPedido.Pedido.LocalidadeTerminoPrestacao.CodigoIBGE;
            else
                cte.CodigoIBGECidadeTerminoPrestacao = IBGECidadeTerminoPrestacao;
            //cte.CodigoIBGECidadeTerminoPrestacao = cte.indicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim ? IBGECidadeInicioPrestacao : IBGECidadeTerminoPrestacao;

            cte.TipoServico = tipoServico;
            Servicos.Log.GravarInfo("15 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            //se o serviço não for normal terá documentos anteriores, porém em ambiente de homologação e for TMS o sistema vai deixar como normal para autorizar, exceto se estiver marcado que está usando documentos em homologação como documentos anteriores.
            if (tipoServico != Dominio.Enumeradores.TipoServico.Normal && (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao || (configuracaoEmbarcador.UtilizarNFeEmHomologacao || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)))
            {
                if (ctesAnteriores.Count > 0)
                {
                    cte.DocumentosTransporteAnteriores = new List<Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior>();
                    cte.DocumentosAnterioresDePapel = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnt in ctesAnteriores)
                    {
                        if (!string.IsNullOrWhiteSpace(cteAnt.Chave) && cteAnt.Chave.Length == 44)
                        {
                            Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior documentoAnterior = new Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior();
                            documentoAnterior.TipoDocumento = Dominio.Enumeradores.TipoDocumentoAnteriorCTe.Eletronico;
                            documentoAnterior.Chave = cteAnt.Chave;
                            documentoAnterior.Emissor = new Dominio.ObjetosDeValor.CTe.Cliente()
                            {
                                Bairro = cteAnt.Emitente.Endereco.Bairro,
                                CEP = cteAnt.Emitente.Endereco.CEP,
                                CodigoAtividade = cteAnt.Emitente.Atividade,
                                CodigoIBGECidade = cteAnt.Emitente.Endereco.Cidade.IBGE,
                                Complemento = cteAnt.Emitente.Endereco.Complemento,
                                CPFCNPJ = cteAnt.Emitente.CNPJ,
                                Endereco = cteAnt.Emitente.Endereco.Logradouro,
                                NomeFantasia = cteAnt.Emitente.NomeFantasia,
                                Numero = cteAnt.Emitente.Endereco.Numero,
                                Exportacao = false,
                                RazaoSocial = cteAnt.Emitente.RazaoSocial,
                                RGIE = cteAnt.Emitente.IE,
                                Telefone1 = cteAnt.Emitente.Endereco.Telefone,
                                Emails = cteAnt.Emitente.Emails,
                                StatusEmails = true,
                                NaoAtualizarDadosCadastrais = true
                            };
                            cte.DocumentosTransporteAnteriores.Add(documentoAnterior);
                        }
                        else if (!string.IsNullOrWhiteSpace(cteAnt.Chave))
                        {
                            Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel documentoAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel();
                            documentoAnterior.TipoDocumentoTransportaAnteriorPapel = "13";
                            documentoAnterior.Serie = "1";
                            documentoAnterior.Numero = cteAnt.Chave.Length > 20 ? cteAnt.Chave.Substring(0, 20) : cteAnt.Chave;
                            documentoAnterior.DataEmissao = DateTime.Now.Date;
                            documentoAnterior.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
                            {
                                CPFCNPJ = cteAnt.Emitente.CNPJ,
                                RGIE = cteAnt.Emitente.IE
                            };
                            cte.DocumentosAnterioresDePapel.Add(documentoAnterior);
                        }
                    }
                }
                else
                    cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
            }
            else
                cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;

            cte.TipoTomador = tipoTomador;
            Servicos.Log.GravarInfo("16 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                if (tomador != null)
                {
                    if (cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario || cte.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                    {
                        if (expedidor != null && tomador.CPF_CNPJ == expedidor.CPF_CNPJ)
                        {
                            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                        }
                        else if (recebedor != null && tomador.CPF_CNPJ == recebedor.CPF_CNPJ)
                        {
                            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                        }
                        else
                        {
                            cte.Tomador = serCliente.ObterClienteCTE(tomador, null);
                        }
                    }
                    else if (tomador.CPF_CNPJ == remetente.CPF_CNPJ)
                    {
                        cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                        cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    }
                    else if (tomador.CPF_CNPJ == destinatario.CPF_CNPJ)
                    {
                        cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                        cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    }
                    else if (expedidor != null && tomador.CPF_CNPJ == expedidor.CPF_CNPJ)
                    {
                        cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                        cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    }
                    else if (recebedor != null && tomador.CPF_CNPJ == recebedor.CPF_CNPJ)
                    {
                        cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                        cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    }
                    else
                    {
                        cte.Tomador = serCliente.ObterClienteCTE(tomador, null);
                    }
                }
            }
            Servicos.Log.GravarInfo("17 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            switch (cte.TipoTomador)
            {
                case Dominio.Enumeradores.TipoTomador.Remetente:
                    cte.indicadorIETomador = remetente?.IndicadorIE;
                    SetarObservacoesTomador(cte, remetente, carga, cargaPedido, unitOfWork);
                    break;
                case Dominio.Enumeradores.TipoTomador.Expedidor:
                    cte.indicadorIETomador = expedidor?.IndicadorIE;
                    SetarObservacoesTomador(cte, expedidor, carga, cargaPedido, unitOfWork);
                    break;
                case Dominio.Enumeradores.TipoTomador.Recebedor:
                    cte.indicadorIETomador = recebedor?.IndicadorIE;
                    SetarObservacoesTomador(cte, recebedor, carga, cargaPedido, unitOfWork);
                    break;
                case Dominio.Enumeradores.TipoTomador.Destinatario:
                    cte.indicadorIETomador = destinatario?.IndicadorIE;
                    SetarObservacoesTomador(cte, destinatario, carga, cargaPedido, unitOfWork);
                    break;
                case Dominio.Enumeradores.TipoTomador.Outros:
                    cte.indicadorIETomador = tomador?.IndicadorIE;
                    SetarObservacoesTomador(cte, tomador, carga, cargaPedido, unitOfWork);
                    break;
                default:
                    cte.indicadorIETomador = null;
                    break;
            }
            SetarObservacaoContribuinte(cte, carga, ocorrencia, unitOfWork, tipoServicoMultisoftware);
            Servicos.Log.GravarInfo("18 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.IncluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.PercentualICMSIncluirNoFrete = 100;

            if (tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
            {
                if (cte.Remetente != null)
                    cte.Remetente.NaoAtualizarEndereco = true;
                if (cte.Destinatario != null)
                    cte.Destinatario.NaoAtualizarEndereco = true;
                if (cte.Expedidor != null)
                    cte.Expedidor.NaoAtualizarEndereco = true;
                if (cte.Recebedor != null)
                    cte.Recebedor.NaoAtualizarEndereco = true;
                if (cte.Tomador != null)
                    cte.Tomador.NaoAtualizarEndereco = true;
            }

            if (regraICMS.NaoImprimirImpostosDACTE)
                cte.ExibeICMSNaDACTE = false;
            else
                cte.ExibeICMSNaDACTE = true;

            cte.CFOP = regraICMS.CFOP;


            if (regraICMS.NaoEnviarImpostoICMSNaEmissaoCte)
            {
                cte.ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                {
                    Aliquota = 0,
                    BaseCalculo = 0,
                    CST = regraICMS.CST,
                    Valor = 0,
                    ValorIncluso = 0,
                    ValorCreditoPresumido = regraICMS.ValorCreditoPresumido,
                    PercentualReducaoBaseCalculo = 0,
                };
            }
            else
                cte.ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                {
                    Aliquota = regraICMS.Aliquota,
                    BaseCalculo = regraICMS.ValorBaseCalculoICMS,
                    CST = regraICMS.CST,
                    Valor = regraICMS.ValorICMS,
                    ValorIncluso = regraICMS.ValorICMSIncluso,
                    ValorCreditoPresumido = regraICMS.ValorCreditoPresumido,
                    PercentualReducaoBaseCalculo = regraICMS.PercentualReducaoBC,
                };

            if (regraICMS.ValorPis > 0)
            {
                cte.PIS = new Dominio.ObjetosDeValor.CTe.ImpostoPIS()
                {
                    Aliquota = regraICMS.AliquotaPis,
                    Valor = regraICMS.ValorPis,
                    BaseCalculo = regraICMS.ValorBaseCalculoPISCOFINS,
                    CST = regraICMS.CST
                };
            }

            if (regraICMS.ValorCofins > 0)
            {
                cte.COFINS = new Dominio.ObjetosDeValor.CTe.ImpostoCOFINS()
                {
                    Aliquota = regraICMS.AliquotaCofins,
                    Valor = regraICMS.ValorCofins,
                    BaseCalculo = regraICMS.ValorBaseCalculoPISCOFINS,
                    CST = regraICMS.CST
                };
            }

            if (regraICMS.IncluirICMSBC)
            {
                decimal valorICMSIncluir = regraICMS.ValorICMSIncluso > 0m ? regraICMS.ValorICMSIncluso : regraICMS.ValorICMS;
                decimal valorICMSRecolhido = Math.Round(valorICMSIncluir * (regraICMS.PercentualInclusaoBC / 100), 2, MidpointRounding.AwayFromZero);
                cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;

                if (configuracaoEmbarcador.UtilizarRegraICMSParaDescontarValorICMS)
                {
                    if (!regraICMS.DescontarICMSDoValorAReceber) // if (cte.ICMS.CST != "60") // Considerar para todas CST (Tarefa #3586 Marfrig)
                        cte.ValorAReceber += valorICMSRecolhido;
                }
                else
                {
                    if (cte.ICMS.CST != "60")
                        cte.ValorAReceber += valorICMSRecolhido;
                }

                if (!regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao)
                    cte.ValorTotalPrestacaoServico += valorICMSRecolhido;

                cte.PercentualICMSIncluirNoFrete = regraICMS.PercentualInclusaoBC;
            }
            else
            {
                if (configuracaoEmbarcador.UtilizarRegraICMSParaDescontarValorICMS)
                {
                    if (cte.ICMS.Valor > 0 && regraICMS.DescontarICMSDoValorAReceber && cte.ValorAReceber > 0) //cte.ICMS.CST == "60" // Considerar para todas CST (Tarefa #3586 Marfrig)
                        cte.ValorAReceber = cte.ValorAReceber - cte.ICMS.Valor;
                }
                else
                {
                    if (cte.ICMS.Valor > 0 && cte.ICMS.CST == "60" && cte.ValorAReceber > 0)
                        cte.ValorAReceber = cte.ValorAReceber - cte.ICMS.Valor;
                }
            }

            cte.IBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterImpostoIBSCBSDoCTe(impostoIBSCBS);

            if (regraISS != null)
            {
                cte.ISS = new Dominio.ObjetosDeValor.CTe.ImpostoISS()
                {
                    Aliquota = regraISS.AliquotaISS,
                    BaseCalculo = regraISS.ValorBaseCalculoISS,
                    PercentualRetencao = regraISS.PercentualRetencaoISS,
                    Valor = regraISS.ValorISS,
                    ValorRetencao = regraISS.ValorRetencaoISS
                };



                if (regraISS.IncluirISSBaseCalculo)
                {
                    cte.ValorAReceber += regraISS.ValorISS - regraISS.ValorRetencaoISS;
                    cte.ValorTotalPrestacaoServico += regraISS.ValorISS;
                    cte.IncluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                }
                else
                {
                    if (configuracaoEmbarcador.ReduzirRetencaoISSValorAReceberNFSManual)
                        cte.ValorAReceber -= regraISS.ValorRetencaoISS;
                }

                if (regraISS.ReterIR) // iniciar
                {
                    if (cte.IR == null)
                        cte.IR = new Dominio.ObjetosDeValor.CTe.ImpostoIR();

                    cte.IR.Valor = regraISS.ValorIR;
                }
            }

            Servicos.Log.GravarInfo("19 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            observacaoRegraICMS = serICMS.ObterObservacaoRegraICMS(regraICMS.ObservacaoCTe, regraICMS.Aliquota, regraICMS.AliquotaSimples, cte.ValorAReceber, cte.ICMS.Valor, cte.ICMS.BaseCalculo, empresa, remetente, destinatario, cliTomador, origem, destino, cte.ICMS.PercentualReducaoBaseCalculo, cte.ProdutoPredominante, cte.ICMS.ValorCreditoPresumido);
            Servicos.Log.GravarInfo("20 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
            if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)//nota fiscal de serviço eletronica.
            {

                NFSe.NFSe serNFSe = new NFSe.NFSe(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(empresa.Codigo, origem.Codigo, cliTomador?.Localidade?.Estado?.Sigla ?? "", cliTomador?.GrupoPessoas?.Codigo ?? 0, cliTomador?.Localidade?.Codigo ?? 0, carga?.TipoOperacao?.Codigo ?? 0, cliTomador?.CPF_CNPJ ?? 0, 0, unitOfWork);

                cte.Serie = transportadorConfiguracaoNFSe?.SerieNFSe != null ? transportadorConfiguracaoNFSe.SerieNFSe.Numero : 0;
                cte.SerieRPS = transportadorConfiguracaoNFSe?.SerieRPS ?? "0";
                cte.NaturezaNFSe = new Dominio.ObjetosDeValor.CTe.NaturezaNFSe();
                cte.NaturezaNFSe.CodigoInterno = transportadorConfiguracaoNFSe?.NaturezaNFSe.Codigo ?? 0;
                cte.NaoEnviarAliquotaEValorISS = transportadorConfiguracaoNFSe?.NaoEnviarAliquotaEValorISS ?? false;

                int exigibilidadeISS = 1;
                if (transportadorConfiguracaoNFSe?.ExigibilidadeISS != null && transportadorConfiguracaoNFSe?.ExigibilidadeISS != Dominio.Enumeradores.ExigibilidadeISS.NaoInformado)
                {
                    if (!transportadorConfiguracaoNFSe.IncidenciaISSLocalidadePrestador || (transportadorConfiguracaoNFSe.IncidenciaISSLocalidadePrestador && cliTomador?.Localidade.Codigo == transportadorConfiguracaoNFSe.LocalidadePrestacao?.Codigo))
                        exigibilidadeISS = (int)transportadorConfiguracaoNFSe.ExigibilidadeISS;
                }

                decimal valorBasePisCofins = cte.ISS == null ? 0 : cte.ISS.BaseCalculo + (regraISS.IncluirISSBaseCalculo ? cte.ISS.Valor : 0);

                Dominio.ObjetosDeValor.CTe.ItemNFSe itemNFSe = new Dominio.ObjetosDeValor.CTe.ItemNFSe()
                {
                    AliquotaISS = regraISS.AliquotaISS,
                    BaseCalculoISS = regraISS.ValorBaseCalculoISS,
                    CodigoIBGECidade = origem.CodigoIBGE,
                    CodigoIBGECidadeIncidencia = origem.CodigoIBGE,
                    CodigoPaisPrestacaoServico = origem.Pais?.Codigo ?? 1058,
                    ExigibilidadeISS = exigibilidadeISS,
                    ISSInclusoValorTotal = regraISS.IncluirISSBaseCalculo,
                    Quantidade = 1,
                    ServicoPrestadoNoPais = true,
                    ValorServico = regraISS.ValorBaseCalculoISS,
                    ValorISS = regraISS.ValorISS,
                    ValorTotal = regraISS.ValorBaseCalculoISS,
                    Servico = new Dominio.ObjetosDeValor.CTe.ServicoNFSe()
                    {
                        CodigoInterno = servicoNFSe?.Codigo ?? transportadorConfiguracaoNFSe?.ServicoNFSe?.Codigo ?? 0
                    },
                    PIS = new Dominio.ObjetosDeValor.CTe.ImpostoPIS
                    {
                        CST = empresa?.Configuracao?.CSTPISCOFINS ?? "",
                        BaseCalculo = valorBasePisCofins,
                        Aliquota = empresa?.Configuracao?.AliquotaPIS ?? 0,
                        Valor = ObterValorPisCofins(valorBasePisCofins, empresa?.Configuracao?.AliquotaPIS),
                    },
                    COFINS = new Dominio.ObjetosDeValor.CTe.ImpostoCOFINS
                    {
                        CST = empresa?.Configuracao?.CSTPISCOFINS ?? "",
                        BaseCalculo = valorBasePisCofins,
                        Aliquota = empresa?.Configuracao?.AliquotaCOFINS ?? 0,
                        Valor = ObterValorPisCofins(valorBasePisCofins, empresa?.Configuracao?.AliquotaCOFINS)
                    }
                };

                if (itemNFSe.PIS.Valor > 0)
                {
                    cte.PIS ??= new Dominio.ObjetosDeValor.CTe.ImpostoPIS();
                    cte.PIS.BaseCalculo += itemNFSe.PIS.BaseCalculo;
                    cte.PIS.Aliquota = itemNFSe.PIS.Aliquota;
                    cte.PIS.Valor += itemNFSe.PIS.Valor;
                }

                if (itemNFSe.COFINS.Valor > 0)
                {
                    cte.COFINS ??= new Dominio.ObjetosDeValor.CTe.ImpostoCOFINS();
                    cte.COFINS.BaseCalculo += itemNFSe.COFINS.BaseCalculo;
                    cte.COFINS.Aliquota = itemNFSe.COFINS.Aliquota;
                    cte.COFINS.Valor += itemNFSe.COFINS.Valor;
                }

                if (impostoIBSCBS != null)
                    itemNFSe.IBSCBS = new Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS
                    {
                        NBS = !string.IsNullOrWhiteSpace(transportadorConfiguracaoNFSe?.NBS) ? transportadorConfiguracaoNFSe.NBS :
                                  !string.IsNullOrWhiteSpace(servicoNFSe?.NBS) ? servicoNFSe.NBS : impostoIBSCBS.NBS,
                        CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao ?? string.Empty,
                        CST = impostoIBSCBS.CST ?? string.Empty,
                        ClassificacaoTributaria = impostoIBSCBS.ClassificacaoTributaria ?? string.Empty,
                        BaseCalculo = impostoIBSCBS.BaseCalculo,
                        AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual,
                        PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual,
                        ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual,
                        AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal,
                        PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal,
                        ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal,
                        AliquotaCBS = impostoIBSCBS.AliquotaCBS,
                        PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS,
                        ValorCBS = impostoIBSCBS.ValorCBS
                    };

                if (transportadorConfiguracaoNFSe != null && string.IsNullOrWhiteSpace(transportadorConfiguracaoNFSe.DiscriminacaoNFSe))
                    itemNFSe.Discriminacao = servicoNFSe?.Descricao ?? transportadorConfiguracaoNFSe.ServicoNFSe.Descricao;
                else
                {
                    string discriminacao = transportadorConfiguracaoNFSe?.DiscriminacaoNFSe ?? string.Empty;
                    discriminacao = discriminacao
                        .Replace("#CodigoDestinatario", destinatario.CodigoIntegracao)
                        .Replace("#CodigoRemetente", remetente.CodigoIntegracao)
                        .Replace("#DataCarga", cte.DataEmissao)
                        .Replace("#Destino", destino.Descricao)
                        .Replace("#Origem", origem.Descricao)
                        .Replace("#NotasFiscais", cte.Documentos != null ? string.Join(", ", cte.Documentos.Select(o => o.Numero)) : string.Empty)
                        .Replace("#CnpjRemetente", remetente.CPF_CNPJ_Formatado)
                        .Replace("#NomeRemetente", remetente.Nome)
                        .Replace("#CnpjDestinatario", destinatario.CPF_CNPJ_Formatado)
                        .Replace("#NomeDestinatario", destinatario.Nome);

                    if (carga != null)
                    {
                        discriminacao = discriminacao
                            .Replace("#TipoCarga", discriminacao.Contains("#TipoCarga") ? (carga.TipoDeCarga?.Descricao ?? string.Empty) : string.Empty)
                            .Replace("#PesoCarga", discriminacao.Contains("#PesoCarga") ? (carga.DadosSumarizados?.PesoTotal.ToString("n2") ?? string.Empty) : string.Empty)
                            .Replace("#ValorMercadoria", discriminacao.Contains("#ValorMercadoria") ? (carga.DadosSumarizados?.ValorTotalProdutos.ToString("n2") ?? string.Empty) : string.Empty)
                            .Replace("#PlacaVeiculo", discriminacao.Contains("#PlacaVeiculo") ? (carga.PlacasVeiculos ?? string.Empty) : string.Empty)
                            .Replace("#NomeMotorista", discriminacao.Contains("#NomeMotorista") ? string.Join(", ", carga.Motoristas.Select(o => o.Nome)) : string.Empty)
                            .Replace("#NumeroCarga", discriminacao.Contains("#NumeroCarga") ? carga.CodigoCargaEmbarcador : string.Empty)
                            .Replace("#NumeroPedidoEmbarcador", discriminacao.Contains("#NumeroPedidoEmbarcador") ? carga.DadosSumarizados?.NumeroPedidoEmbarcador : string.Empty)
                            .Replace("#CPFMotorista", discriminacao.Contains("#CPFMotorista") ? string.Join(", ", carga.Motoristas.Select(o => o.CPF_CNPJ_Formatado)) : string.Empty);
                    }

                    itemNFSe.Discriminacao = discriminacao;
                }

                cte.ItensNFSe = new List<Dominio.ObjetosDeValor.CTe.ItemNFSe>() { itemNFSe };

                if ((cliTomador?.Localidade?.Codigo ?? 0) != (empresa?.Localidade?.Codigo ?? 0))
                {
                    if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        cte.Remetente.IM = "";
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        cte.Destinatario.IM = "";
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        cte.Tomador.IM = "";
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        cte.Expedidor.IM = "";
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        cte.Recebedor.IM = "";
                }
            }
            Servicos.Log.GravarInfo("FIM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarInformacoesCTe");
        }

        private decimal ObterValorPisCofins(decimal valorBasePisCofins, decimal? aliquota)
            => valorBasePisCofins > 0 && (aliquota ?? 0) > 0 ? (valorBasePisCofins * (aliquota.Value / 100)) : 0;

        private void SetarObservacaoContribuinte(Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            //Servicos.Log.GravarInfo("1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarObservacaoContribuinte");

            if (cte == null)
                return;

            Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte repCargaOcorrenciaObservacaoContribuinte = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte(unitOfWork);

            List<Dominio.ObjetosDeValor.CTe.Observacao> observacoesFisco = new List<Dominio.ObjetosDeValor.CTe.Observacao>();
            List<Dominio.ObjetosDeValor.CTe.Observacao> observacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> observacoes = repObservacaoContribuinte.BuscarAtivos();

            if (carga != null)
            {
                observacoes.AddRange(repObservacaoContribuinte.BuscarPorCarga(carga.Codigo));

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento != null && carga.TipoOperacao.ConfiguracaoEmissaoDocumento.AverbarContainerComAverbacaoCarga && carga.TipoOperacao.ConfiguracaoEmissaoDocumento.ValorContainerAverbacao > 0)
                    {
                        Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte obs = new Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte();
                        obs.Tipo = TipoObservacaoCTe.Contribuinte;
                        obs.Identificador = "ValorContainer";
                        obs.Texto = carga.TipoOperacao.ConfiguracaoEmissaoDocumento.ValorContainerAverbacao.ToString("n2");
                        observacoes.Add(obs);
                    }
                    else if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento != null && carga.TipoOperacao.ConfiguracaoEmissaoDocumento.AverbarContainerComAverbacaoCarga && carga.Veiculo.ValorContainerAverbacao > 0)
                    {
                        Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte obs = new Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte();
                        obs.Tipo = TipoObservacaoCTe.Contribuinte;
                        obs.Identificador = "ValorContainer";
                        obs.Texto = carga.Veiculo.ValorContainerAverbacao.ToString("n2");
                        observacoes.Add(obs);
                    }
                }
            }

            if (observacoes.Count > 0)
            {
                observacoesFisco = observacoes.Where(o => o.Tipo == TipoObservacaoCTe.Fisco).Select(o => o.ObterObservacaoCTe()).ToList();
                observacoesContribuinte = observacoes.Where(o => o.Tipo == TipoObservacaoCTe.Contribuinte).Select(o => o.ObterObservacaoCTe()).ToList();
            }

            if (ocorrencia != null)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte> observacoesFiscoContribuinteOcorrencia = repCargaOcorrenciaObservacaoContribuinte.BuscarPorOcorrencia(ocorrencia.Codigo);
                if (observacoesFiscoContribuinteOcorrencia.Count > 0)
                {
                    observacoesFisco.AddRange(observacoesFiscoContribuinteOcorrencia.Where(o => o.Tipo == TipoObservacaoCTe.Fisco).Select(o => o.ObterObservacaoCTe()).ToList());
                    observacoesContribuinte.AddRange(observacoesFiscoContribuinteOcorrencia.Where(o => o.Tipo == TipoObservacaoCTe.Contribuinte).Select(o => o.ObterObservacaoCTe()).ToList());
                }
            }

            if (observacoesFisco.Count <= 0 && observacoesContribuinte.Count <= 0)
                return;

            //Servicos.Log.GravarInfo("2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarObservacaoContribuinte");

            if (cte.ObservacoesContribuinte == null)
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            if (cte.ObservacoesFisco == null)
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            cte.ObservacoesContribuinte.AddRange(observacoesContribuinte.Select(o => new Dominio.ObjetosDeValor.CTe.Observacao()
            {
                Identificador = Utilidades.String.Left(o.Identificador, 20),
                Descricao = Utilidades.String.Left(o.Descricao, 160)
            }).ToList());

            cte.ObservacoesFisco.AddRange(observacoesFisco.Select(o => new Dominio.ObjetosDeValor.CTe.Observacao()
            {
                Identificador = Utilidades.String.Left(o.Identificador, 20),
                Descricao = Utilidades.String.Left(o.Descricao, 160)
            }).ToList());

            //Servicos.Log.GravarInfo("3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarObservacaoContribuinte");
        }

        private void SetarObservacoesTomador(Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (tomador == null || cte == null)
                return;

            Repositorio.Embarcador.Pessoas.ClienteObservacaoCTe repClienteObservacaoCTe = new Repositorio.Embarcador.Pessoas.ClienteObservacaoCTe(unitOfWork);

            List<Dominio.ObjetosDeValor.CTe.Observacao> observacoesFisco = new List<Dominio.ObjetosDeValor.CTe.Observacao>();
            List<Dominio.ObjetosDeValor.CTe.Observacao> observacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe> observacoesPessoa = repClienteObservacaoCTe.BuscarPorPessoa(tomador.CPF_CNPJ);

            if (observacoesPessoa.Count > 0)
            {
                observacoesFisco = observacoesPessoa.Where(o => o.Tipo == TipoObservacaoCTe.Fisco).Select(o => o.ObterObservacaoCTe()).ToList();
                observacoesContribuinte = observacoesPessoa.Where(o => o.Tipo == TipoObservacaoCTe.Contribuinte).Select(o => o.ObterObservacaoCTe()).ToList();
            }
            else if (tomador.GrupoPessoas != null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoCTe repGrupoPessoasObservacaoCTe = new Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe> observacoesGrupoPessoas = repGrupoPessoasObservacaoCTe.BuscarPorGrupoPessoas(tomador.GrupoPessoas.Codigo);

                observacoesFisco = observacoesGrupoPessoas.Where(o => o.Tipo == TipoObservacaoCTe.Fisco).Select(o => o.ObterObservacaoCTe()).ToList();
                observacoesContribuinte = observacoesGrupoPessoas.Where(o => o.Tipo == TipoObservacaoCTe.Contribuinte).Select(o => o.ObterObservacaoCTe()).ToList();
            }

            if (observacoesFisco.Count <= 0 && observacoesContribuinte.Count <= 0)
                return;

            if (cte.ObservacoesContribuinte == null)
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            if (cte.ObservacoesFisco == null)
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            cte.ObservacoesContribuinte.AddRange(observacoesContribuinte.Select(o => new Dominio.ObjetosDeValor.CTe.Observacao()
            {
                Identificador = Utilidades.String.Left(o.Identificador, 20),
                Descricao = Utilidades.String.Left(SubstituirTagsObservacoesTomadorCTe(o.Descricao, cte, carga, cargaPedido), 160)
            }).Where(c => c.Descricao != "").ToList());

            cte.ObservacoesFisco.AddRange(observacoesFisco.Select(o => new Dominio.ObjetosDeValor.CTe.Observacao()
            {
                Identificador = Utilidades.String.Left(o.Identificador, 20),
                Descricao = Utilidades.String.Left(SubstituirTagsObservacoesTomadorCTe(o.Descricao, cte, carga, cargaPedido), 60)
            }).Where(c => c.Descricao != "").ToList());
        }

        private string SubstituirTagsObservacoesTomadorCTe(string observacao, Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (string.IsNullOrWhiteSpace(observacao))
                return observacao;

            string numeroRerenciaEDI = cte != null && cte.Documentos != null ? string.Join(", ", cte?.Documentos?.Select(n => n.NumeroReferenciaEDI).ToList()) : string.Empty;
            string codigoOBSTerminalDestino = cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TerminalDestino != null && !string.IsNullOrWhiteSpace(cargaPedido.Pedido.TerminalDestino.CodigoObservacaoContribuinte) ? cargaPedido.Pedido.TerminalDestino.CodigoObservacaoContribuinte : string.Empty;
            string codigoOBSTerminalOrigem = cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TerminalOrigem != null && !string.IsNullOrWhiteSpace(cargaPedido.Pedido.TerminalOrigem.CodigoObservacaoContribuinte) ? cargaPedido.Pedido.TerminalOrigem.CodigoObservacaoContribuinte : string.Empty;
            string placasReboques = carga != null && carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 ? string.Join(", ", carga.VeiculosVinculados.Select(n => n.Placa).ToList()) : string.Empty;
            string cpfMotorista = carga != null && carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault().CPF : string.Empty;
            string nomeMotorista = carga != null && carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault().Nome : string.Empty;
            string codigoIntegracaoTipoOperacao = carga?.TipoOperacao?.CodigoIntegracao ?? string.Empty;

            return observacao.Replace("#NumeroCarga", carga?.CodigoCargaEmbarcador ?? string.Empty)
                             .Replace("#NumeroPedidoEmbarcador", cargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty)
                             .Replace("#PlacaTracao", carga?.Veiculo?.Placa ?? string.Empty)
                             .Replace("#NumeroNotaFiscal", cte?.Documentos?.FirstOrDefault()?.Numero ?? string.Empty)
                             .Replace("#NumeroReferenciaEDI", numeroRerenciaEDI)
                             .Replace("#CodigoOBSTerminalDestino", codigoOBSTerminalDestino)
                             .Replace("#CodigoOBSTerminalOrigem", codigoOBSTerminalOrigem)
                             .Replace("#PlacasReboque", placasReboques)
                             .Replace("#CPFMotorista", cpfMotorista)
                             .Replace("#NomeMotorista", nomeMotorista)
                             .Replace("#CodigoIntegracaoTipoOperacao", codigoIntegracaoTipoOperacao);
        }

        private Dominio.Entidades.Cliente ObterClienteTomador(Dominio.Enumeradores.TipoTomador tipoTomador, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Cliente tomador)
        {
            Dominio.Entidades.Cliente cliTomador = null;
            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                cliTomador = remetente;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                cliTomador = destinatario;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                cliTomador = tomador;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                cliTomador = expedidor;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                cliTomador = recebedor;

            return cliTomador;
        }

        private void SetarObservacoesCTePedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ref string observacaoCTe, string observacaoCTeTerceiro, List<string> rotas, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, bool permiteObservacaoVazia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = null)
        {
            Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (cargaPedido != null)
            {
                List<int> codigosContainer = new List<int>();
                int qtdObservacaoCont = 0;

                if (cargasPedidos != null && cargasPedidos.Count > 0)
                {
                    foreach (var cargaPed in cargasPedidos)
                        BuscarObservacaoPedido(ref qtdObservacaoCont, cargaPed, cte, ref observacaoCTe, ref codigosContainer, unitOfWork);

                    if (cargasPedidos.Any(o => o.Pedido != null && o.Pedido.Container != null && o.Pedido.Container.ContainerTipo != null) && codigosContainer != null && codigosContainer.Count > 0)
                        observacaoCTe += " / Qtde: " + codigosContainer.Count().ToString("D") + " container de " + string.Join(", ", (from obj in cargasPedidos select obj.Pedido?.Container?.ContainerTipo?.Descricao).Distinct()) + " pés ";
                }
                else
                {
                    BuscarObservacaoPedido(ref qtdObservacaoCont, cargaPedido, cte, ref observacaoCTe, ref codigosContainer, unitOfWork);
                    if (cargaPedido.Pedido != null && cargaPedido.Pedido.Container != null && cargaPedido.Pedido.Container.ContainerTipo != null)
                        observacaoCTe += " / Qtde: 1 container de " + cargaPedido.Pedido.Container.ContainerTipo.Descricao + " pés ";

                }
            }
        }

        private void BuscarObservacaoPedido(ref int qtdObservacaoCont, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ref string observacaoCTe, ref List<int> codigosContainer, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            if (cargaPedido.Pedido != null && cargaPedido.Pedido.Container != null && !codigosContainer.Contains(cargaPedido.Pedido.Container.Codigo))
            {
                List<int> codigosCntCTe = repContainerCTE.BuscarCodigoContainerPorCTe(cte.Codigo);
                if (codigosCntCTe == null || codigosCntCTe.Count == 0 || codigosCntCTe.Contains(cargaPedido.Pedido.Container.Codigo))
                {
                    codigosContainer.Add(cargaPedido.Pedido.Container.Codigo);
                    observacaoCTe += " Container - Lacre: " + (cargaPedido.Pedido.Container.Numero + " - " + (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerUm) ? cargaPedido.Pedido.LacreContainerUm : "") + " " + (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerDois) ? cargaPedido.Pedido.LacreContainerDois : "") + " " + (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerTres) ? cargaPedido.Pedido.LacreContainerTres : ""));
                }
            }
            if (cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0 && cte.XMLNotaFiscais.Count <= 1000)
            {
                List<int> codigosNotas = cte.XMLNotaFiscais.Select(c => c.Codigo).ToList();
                IList<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosObservacaoNotaFiscal> dadosObservacaoNotas = repXMLNotaFiscal.BuscarDadosObservacaoNotaFiscal(codigosNotas);
                if (dadosObservacaoNotas != null && dadosObservacaoNotas.Count > 0)
                {
                    foreach (var notaFiscal in dadosObservacaoNotas)
                    {
                        if (!string.IsNullOrWhiteSpace(notaFiscal.NumeroControleCliente))
                            observacaoCTe += " / N.C: " + notaFiscal.NumeroControleCliente;

                        if (!string.IsNullOrWhiteSpace(notaFiscal.ObservacaoNotaFiscalParaCTe))
                            observacaoCTe += " / OBS NF " + notaFiscal.ObservacaoNotaFiscalParaCTe;
                    }
                }
            }
            if (cargaPedido.ModalPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.Nenhum)
            {
                if (cargaPedido.TipoCobrancaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.NotaFiscalServico)
                {
                    string msgModal = " / Modal: " + Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodalHelper.ObterDescricaoCompleta(cargaPedido.ModalPropostaMultimodal);
                    string msgValorComercial = " / Valor Comercial: " + cte.ValorTotalMercadoria.ToString("n2");
                    if (string.IsNullOrWhiteSpace(observacaoCTe) || !observacaoCTe.Contains(msgModal))
                        observacaoCTe += msgModal;
                    if (string.IsNullOrWhiteSpace(observacaoCTe) || !observacaoCTe.Contains(msgValorComercial))
                        observacaoCTe += msgValorComercial;
                }
                if (cargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS)
                {
                    if (cargaPedido.Pedido != null && !string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroOS))
                        observacaoCTe += " / OS: " + cargaPedido.Pedido.NumeroOS;
                }
            }
        }

        private void SetarObservacaoCTe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string observacaoCTe, string observacaoCTeTerceiro, string observacaoRegraICMS, List<string> rotas, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, bool permiteObservacaoVazia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = null, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = null)
        {
            Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);
  
            if (!string.IsNullOrWhiteSpace(cte.NumeroBooking))
            {
                Dominio.Entidades.ObservacaoContribuinteCTE observacaoContribuinte = new Dominio.Entidades.ObservacaoContribuinteCTE()
                {
                    CTE = cte,
                    Descricao = " Booking: " + cte.NumeroBooking,
                    Identificador = "Booking"
                };
                repObservacaoContribuinteCTE.Inserir(observacaoContribuinte);
            }
            if (!string.IsNullOrWhiteSpace(cte.NumeroControle))
            {
                Dominio.Entidades.ObservacaoContribuinteCTE observacaoContribuinte = new Dominio.Entidades.ObservacaoContribuinteCTE()
                {
                    CTE = cte,
                    Descricao = " Num Ctrl: " + cte.NumeroControle,
                    Identificador = "Ctrl"
                };
                repObservacaoContribuinteCTE.Inserir(observacaoContribuinte);
            }
            if (cte.Viagem != null)
            {
                Dominio.Entidades.ObservacaoContribuinteCTE observacaoContribuinte = new Dominio.Entidades.ObservacaoContribuinteCTE()
                {
                    CTE = cte,
                    Descricao = " Navio/Viagem: " + cte.Viagem.Descricao,
                    Identificador = "Viagem"
                };
                repObservacaoContribuinteCTE.Inserir(observacaoContribuinte);
            }
            if (cte.TipoModal == TipoModal.Aquaviario && cte.PortoOrigem != null && cte.PortoOrigem != null && cte.TomadorPagador != null && cte.TomadorPagador.GrupoPessoas != null)
            {
                if (cte.PortoOrigem.AtivarDespachanteComoConsignatario && cte.PortoDestino.AtivarDespachanteComoConsignatario && cte.TomadorPagador != null && cte.TomadorPagador.GrupoPessoas != null && cte.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cte.TomadorPagador.GrupoPessoas.Despachante != null)
                {
                    cte.ObservacoesGerais += " A " + cte.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cte.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                    cte.ObservacoesGerais = cte.ObservacoesGerais.Trim();
                }
            }
            SetarObservacoesCTePedido(cargaPedido, cte, ref observacaoCTe, observacaoCTeTerceiro, rotas, pedidoParaObservacaoCTe, permiteObservacaoVazia, unitOfWork, tipoServicoMultisoftware, cargasPedidos);

            if (!configuracaoEmbarcador.GerarObservacaoRegraICMSAposObservacaoCTe)
                cte.ObservacoesGerais += observacaoRegraICMS.Trim();

            if (!string.IsNullOrWhiteSpace(observacaoCTe) || !string.IsNullOrWhiteSpace(observacaoCTeTerceiro))
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoTransporteAnterior = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                string rota = rotas != null ? string.Join("/", rotas) : string.Empty;
                string rotaCargaPedido = string.Empty;
                string rotaCargaPedidoComValor = string.Empty;
                string rotaFreteCarga = string.Empty;
                string numeroCarga = string.Empty;
                string placas = string.Empty;
                string estadoVeiculo = string.Empty;
                string nomeMotorista = string.Empty;
                string cpfMotorista = string.Empty;
                string cnhMotorista = "";
                string rgMotorista = "";
                string codigoTabelaFrete = string.Empty;
                string codigoConfiguracaoTabelaFrete = string.Empty;
                string cpfOperador = string.Empty;
                string nomeOperador = string.Empty;
                string modeloVeicularCarga = string.Empty;
                string apoliceSeguro = string.Empty;
                string seguradora = string.Empty;
                string nomeProprietarioVeiculo = string.Empty;
                string cpfCnpjProprietarioVeiculo = string.Empty;
                string IEProprietarioVeiculo = string.Empty;
                string rntrcProprietarioVeiculo = string.Empty;
                string enderecoProprietarioVeiculo = string.Empty;
                string marcaVeiculoTerceiro = string.Empty;
                string placaVeiculoTerceiro = string.Empty;
                string estadoVeiculoTerceiro = string.Empty;
                string renavamVeiculoTerceiro = string.Empty;
                string chaveDocumentoTransporteAnterior = string.Empty;
                string marcaVeiculo = string.Empty;
                string numeroNotaFiscal = string.Empty;
                string numeroReferenciaEDI = string.Empty;
                string observacaoNotaFiscal = string.Empty;
                string codigoIntegracaoTipoOperacao = string.Empty;
                string codigoIntegracaoFilial = string.Empty;
                string codigoModeloVeicularCarga = string.Empty;
                string tipoEmbarque = string.Empty;
                string numeroContainer = string.Empty;
                string ordemPedido = string.Empty;
                string tipoOperacao = string.Empty;
                string dataAgendamento = string.Empty;
                string faixaTemperatura = string.Empty;
                string renavamVeiculo = string.Empty;
                string reserva = string.Empty;
                string lacres = string.Empty;
                int quantidadeVolumes = 0;
                decimal valorAdvalorem = 0m;
                decimal percentualBonificacaoTransportador = 0m;
                decimal kmTotalCarga = 0m;
                string masterBL = string.Empty;
                string numeroDI = string.Empty;
                string embarque = string.Empty;
                decimal valorPedagio = 0m;


                if (cargaPedido != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
                    Dominio.Entidades.Veiculo veiculo = cargaPedido.Carga.Veiculo;
                    List<Dominio.Entidades.Veiculo> reboques = cargaPedido.Carga.VeiculosVinculados?.ToList();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> rotasFrete = repCargaPedidoRotaFrete.BuscarPorCargaPedido(cargaPedido.Codigo);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguroAverbacao = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo);

                    List<string> renavamVeiculos = new List<string>() { veiculo?.Renavam };
                    renavamVeiculos.AddRange(reboques?.Select(o => o.Renavam));

                    List<string> estadoVeiculos = new List<string>() { veiculo?.Estado?.Sigla };
                    estadoVeiculos.AddRange(reboques?.Select(o => o.Estado?.Sigla));

                    List<string> cnhMotoristas = repCargaMotorista.BuscarCNHMotoristasPorCarga(cargaPedido.Carga.Codigo);
                    List<string> rgMotoristas = repCargaMotorista.BuscarRGMotoristasPorCarga(cargaPedido.Carga.Codigo);
                    List<string> nomeMotoristas = repCargaMotorista.BuscarNomeMotoristasPorCarga(cargaPedido.Carga.Codigo);
                    List<string> cpfMotoristas = repCargaMotorista.BuscarCPFMotoristasPorCarga(cargaPedido.Carga.Codigo);

                    cnhMotorista = string.Join(", ", cnhMotoristas);
                    rgMotorista = string.Join(", ", rgMotoristas);
                    codigoTabelaFrete = string.Join(",", repCargaTabelaFreteCliente.BuscarCodigosIntegracaoPorCarga(cargaPedido.Carga.Codigo));
                    codigoConfiguracaoTabelaFrete = string.Join(", ", repCargaTabelaFreteCliente.BuscarCodigosIntegracaoConfiguracaoTabelaFretePorCarga(cargaPedido.Carga.Codigo));

                    rotaFreteCarga = cargaPedido.Carga.Rota?.Descricao ?? string.Empty;
                    rotaCargaPedido = rotasFrete != null && rotasFrete.Count > 0 ? string.Join("/", rotasFrete.Select(o => o.RotaFrete.Descricao)) : string.Empty;
                    rotaCargaPedidoComValor = rotasFrete != null && rotasFrete.Count > 0 ? string.Join("/", rotasFrete.Select(o => o.RotaFrete.Descricao + " (" + o.ValorTabelaFrete.ToString("n2") + ")")) : string.Empty;
                    numeroCarga = configuracaoGeralCarga.NaoUtilizarCodigoCargaOrigemNaObservacaoCTe ? cargaPedido?.Carga?.CodigoCargaEmbarcador ?? cargaPedido.CargaOrigem?.CodigoCargaEmbarcador ?? "" : cargaPedido.CargaOrigem?.CodigoCargaEmbarcador ?? "";
                    placas = cargaPedido.Carga.PlacasVeiculos;
                    renavamVeiculo = string.Join(", ", renavamVeiculos?.Where(o => !string.IsNullOrWhiteSpace(o)));
                    estadoVeiculo = string.Join(", ", estadoVeiculos?.Where(o => !string.IsNullOrWhiteSpace(o)).Distinct());
                    nomeMotorista = string.Join(", ", nomeMotoristas);
                    cpfMotorista = string.Join(", ", cpfMotoristas);
                    cpfOperador = cargaPedido.Carga.Operador?.CPF_Formatado ?? string.Empty;
                    nomeOperador = cargaPedido.Carga.Operador?.Nome ?? string.Empty;
                    modeloVeicularCarga = cargaPedido.Carga.ModeloVeicularCarga?.Descricao ?? string.Empty;
                    seguradora = apolicesSeguroAverbacao != null && apolicesSeguroAverbacao.Count > 0 ? string.Join(", ", (from obj in apolicesSeguroAverbacao select obj.ApoliceSeguro?.Seguradora?.Nome).Distinct()) : string.Empty;
                    apoliceSeguro = apolicesSeguroAverbacao != null && apolicesSeguroAverbacao.Count > 0 ? string.Join(", ", (from obj in apolicesSeguroAverbacao select obj.ApoliceSeguro?.NumeroApolice).Distinct()) : string.Empty;
                    chaveDocumentoTransporteAnterior = string.Join(", ", repDocumentoTransporteAnterior.BuscarChaveDocumentosEletronicosPorCTe(cte.Codigo));
                    marcaVeiculo = veiculo?.Marca?.Descricao ?? string.Empty;
                    numeroNotaFiscal = string.Join(", ", repDocumentoCTe.BuscarNumeroNotasFiscais(cte.Codigo));
                    numeroReferenciaEDI = string.Join(", ", repDocumentoCTe.BuscarNumeroReferenciaEDI(cte.Codigo));
                    masterBL = xmlNotasFiscais != null && xmlNotasFiscais.Count > 0 ? string.Join(", ", xmlNotasFiscais.Where(o => !string.IsNullOrWhiteSpace(o.MasterBL)).Select(o => o.MasterBL)) : "";
                    embarque = xmlNotasFiscais != null && xmlNotasFiscais.Count > 0 ? string.Join(", ", xmlNotasFiscais.Where(o => !string.IsNullOrWhiteSpace(o.Embarque)).Select(o => o.Embarque)) : "";
                    numeroDI = xmlNotasFiscais != null && xmlNotasFiscais.Count > 0 ? string.Join(", ", xmlNotasFiscais.Where(o => !string.IsNullOrWhiteSpace(o.NumeroDI)).Select(o => o.NumeroDI)) : "";
                    observacaoNotaFiscal = xmlNotasFiscais != null && xmlNotasFiscais.Count > 0 ? string.Join(", ", xmlNotasFiscais.Where(o => !string.IsNullOrWhiteSpace(o.Observacao)).Select(o => o.Observacao)) : "";
                    codigoIntegracaoTipoOperacao = cargaPedido.Carga.TipoOperacao?.CodigoIntegracao ?? string.Empty;
                    codigoIntegracaoFilial = cargaPedido.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty;
                    codigoModeloVeicularCarga = cargaPedido.Carga.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty;
                    tipoEmbarque = cargaPedido.Pedido.TipoEmbarque ?? string.Empty;
                    numeroContainer = cargaPedido.Pedido != null && cargaPedido.Pedido.Container != null ? cargaPedido.Pedido.Container.Numero : cargaPedido.Pedido?.NumeroContainer ?? string.Empty;
                    percentualBonificacaoTransportador = cargaPedido.Carga.PercentualBonificacaoTransportador;
                    kmTotalCarga = cargaPedido.Carga?.DadosSumarizados?.Distancia ?? 0m;
                    valorPedagio = cargaPedido.ValorPedagio;


                    if (string.IsNullOrWhiteSpace(numeroContainer))
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> listaCargaVeiculoContainers = repCargaVeiculoContainer.BuscarPorCarga(cargaPedido.Carga.Codigo);
                        numeroContainer = listaCargaVeiculoContainers != null && listaCargaVeiculoContainers.Count > 0 ? string.Join(", ", listaCargaVeiculoContainers.Select(o => o.NumeroContainer)) : string.Empty;
                    }

                    ordemPedido = cargaPedido.Pedido?.Ordem ?? string.Empty;
                    tipoOperacao = cargaPedido.Carga.TipoOperacao?.Descricao ?? string.Empty;
                    dataAgendamento = cargaPedido.Pedido?.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
                    reserva = cargaPedido.Pedido?.Reserva ?? string.Empty;
                    quantidadeVolumes = cargaPedido.Pedido?.QtVolumes ?? 0;
                    lacres = carga != null && carga.Lacres?.Count > 0 ? string.Join(", ", carga.Lacres.Select(o => o.Numero)) : string.Empty;

                    if ((observacaoCTe?.Contains("#FaixaTemperatura") ?? false) && (cargaPedido.Carga.TipoDeCarga?.ControlaTemperatura ?? false))
                    {
                        Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemp = cargaPedido.Carga.TipoDeCarga?.FaixaDeTemperatura;

                        if (faixaTemp != null)
                            faixaTemperatura = $"{faixaTemp.FaixaInicial.ToString("n2")} até {faixaTemp.FaixaFinal.ToString("n2")}";
                    }
                }

                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = observacaoCTe.Replace("#", " #");

                if (cte.ComponentesPrestacao != null && cte.ComponentesPrestacao.Count > 0 && !string.IsNullOrWhiteSpace(observacaoCTe) && observacaoCTe.Contains("#ValorADValorem"))
                {
                    if (cte.ComponentesPrestacao.Any(o => o.ComponenteFrete != null && o.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM))
                        valorAdvalorem = cte.ComponentesPrestacao.Where(o => o.ComponenteFrete != null && o.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM).Sum(o => o.Valor);
                }

                string numeroPedido = pedidoParaObservacaoCTe?.NumeroPedidoEmbarcador ?? "";
                if (observacaoCTe != null && observacaoCTe.Contains("#NumeroPedido") && cargasPedidos != null && cargasPedidos.Count > 1)
                    numeroPedido = string.Join(",", (from obj in cargasPedidos select obj.Pedido.NumeroPedidoEmbarcador).Distinct().ToList());

                string observacao = observacaoCTe?
                    .Replace("#CNPJTomador", cte.Tomador?.CPF_CNPJ_Formatado ?? string.Empty)
                    .Replace("#NomeTomador", cte.Tomador?.Nome ?? string.Empty)
                    .Replace("#CNPJRemetente", cte.Remetente?.CPF_CNPJ_Formatado)
                    .Replace("#NomeRemetente", cte.Remetente?.Nome)
                    .Replace("#LocalPrestacaoServico", cte.Remetente?.Localidade?.DescricaoCidadeEstado ?? "")
                    .Replace("#CNPJDestinatario", cte.Destinatario?.CPF_CNPJ_Formatado ?? "")
                    .Replace("#NomeDestinatario", cte.Destinatario?.Nome ?? "")
                    .Replace("#NumeroPedido", numeroPedido)
                    .Replace("#NumeroBooking", pedidoParaObservacaoCTe?.NumeroBooking ?? "")
                    .Replace("#NumeroOS", pedidoParaObservacaoCTe?.NumeroOS ?? "")
                    .Replace("#ValorNota", cte.ValorTotalMercadoria.ToString("n2"))
                    .Replace("#ValorADValorem", valorAdvalorem.ToString("n2"))
                    .Replace("#NumeroPedidoCliente", pedidoParaObservacaoCTe?.CodigoPedidoCliente ?? "")
                    .Replace("#NavioViagemDirecao", pedidoParaObservacaoCTe?.PedidoViagemNavio?.Descricao ?? "")
                    .Replace("#QuantidadeETipoContainer", (pedidoParaObservacaoCTe != null && pedidoParaObservacaoCTe.Container != null && pedidoParaObservacaoCTe.Container.ContainerTipo != null ? "Qtde: 1 container de " + pedidoParaObservacaoCTe.Container.ContainerTipo.Descricao + " pés" : ""))
                    .Replace("#PortoOrigem", pedidoParaObservacaoCTe?.Porto?.Descricao ?? "")
                    .Replace("#PortoDestino", pedidoParaObservacaoCTe?.PortoDestino?.Descricao ?? "")
                    .Replace("#NumeroCTe", cte.Numero.ToString())
                    .Replace("#NumeroCarga", numeroCarga)
                    .Replace("#SerieCTe", cte.Serie.Numero.ToString())
                    .Replace("#RotaPedidoComValor", rotaCargaPedidoComValor)
                    .Replace("#RotaPedido", rotaCargaPedido)
                    .Replace("#RotaFreteCarga", rotaFreteCarga)
                    .Replace("#Rota", rota)
                    .Replace("#ValorTotalPrestacao", cte.ValorPrestacaoServico.ToString("n2"))
                    .Replace("#Placas", placas)
                    .Replace("#RENAVAM", renavamVeiculo)
                    .Replace("#CPFMotorista", cpfMotorista)
                    .Replace("#NomeMotorista", nomeMotorista)
                    .Replace("#CNHMotorista", cnhMotorista)
                    .Replace("#RGMotorista", rgMotorista)
                    .Replace("#CodigoTabelaFrete", codigoTabelaFrete)
                    .Replace("#CodigoConfiguracaoTabelaFrete", codigoConfiguracaoTabelaFrete)
                    .Replace("#CPFOperador", cpfOperador)
                    .Replace("#NomeOperador", nomeOperador)
                    .Replace("#ModeloVeicularCarga", modeloVeicularCarga)
                    .Replace("#Seguradora", seguradora)
                    .Replace("#ApoliceSeguro", apoliceSeguro)
                    .Replace("#ChaveDocumentoTransporteAnterior", chaveDocumentoTransporteAnterior)
                    .Replace("#CNPJEmitente", cte.Empresa?.CNPJ_Formatado ?? string.Empty)
                    .Replace("#NomeEmitente", cte.Empresa?.RazaoSocial ?? string.Empty)
                    .Replace("#MarcaVeiculo", marcaVeiculo)
                    .Replace("#NumeroNotaFiscal", numeroNotaFiscal)
                    .Replace("#NumeroReferenciaEDI", numeroReferenciaEDI)
                    .Replace("#ObservacaoNotaFiscal", observacaoNotaFiscal)
                    .Replace("#TipoEmbarque", tipoEmbarque)
                    .Replace("#CodigoIntegracaoTipoOperacao", codigoIntegracaoTipoOperacao)
                    .Replace("#CodigoModeloVeicularCarga", codigoModeloVeicularCarga)
                    .Replace("#CodigoIntegracaoFilial", codigoIntegracaoFilial)
                    .Replace("#NumeroContainer", numeroContainer)
                    .Replace("#OrdemPedido", ordemPedido)
                    .Replace("#TipoOperacao", tipoOperacao)
                    .Replace("#ValorDoFreteMenosUmPorcento", (cte.ValorFrete > 0 ? (cte.ValorFrete * 0.99m) : 0).ToString("n2"))
                    .Replace("#ValorFrete", cte.ValorFrete.ToString("n2"))
                    .Replace("#DataAgendamento", dataAgendamento)
                    .Replace("#FaixaTemperatura", faixaTemperatura)
                    .Replace("#Moeda", cte.Moeda?.ObterDescricao() ?? string.Empty)
                    .Replace("#ValorCotacaoMoeda", cte.ValorCotacaoMoeda?.ToString("n10") ?? 0.ToString("n10"))
                    .Replace("#ValorTotalMoeda", cte.ValorTotalMoeda?.ToString("n2") ?? 0.ToString("n2"))
                    .Replace("#NumeroDI", numeroDI)
                    .Replace("#MasterBL", masterBL)
                    .Replace("#Embarque", embarque)
                    .Replace("#Adicional1Pedido", pedidoParaObservacaoCTe?.Adicional1 ?? string.Empty)
                    .Replace("#Adicional2Pedido", pedidoParaObservacaoCTe?.Adicional2 ?? string.Empty)
                    .Replace("#Adicional3Pedido", pedidoParaObservacaoCTe?.Adicional3 ?? string.Empty)
                    .Replace("#Adicional4Pedido", pedidoParaObservacaoCTe?.Adicional4 ?? string.Empty)
                    .Replace("#Adicional5Pedido", pedidoParaObservacaoCTe?.Adicional5 ?? string.Empty)
                    .Replace("#Adicional6Pedido", pedidoParaObservacaoCTe?.Adicional6 ?? string.Empty)
                    .Replace("#Adicional7Pedido", pedidoParaObservacaoCTe?.Adicional7 ?? string.Empty)
                    .Replace("#Reserva", reserva)
                    .Replace("#QuantidadeVolumes", quantidadeVolumes.ToString("n0"))
                    .Replace("#LacresCarga", lacres)
                    .Replace("#PesoCarga", cargaPedido?.Carga?.DadosSumarizados?.PesoTotal > 0m ? cargaPedido?.Carga?.DadosSumarizados?.PesoTotal.ToString("n2") ?? string.Empty : string.Empty)
                    .Replace("#PesoCTe", cte.Peso > 0m ? cte.Peso.ToString("n2") : string.Empty)
                    .Replace("#DataPrevisaoEntrega", (cargaPedido?.Pedido?.PrevisaoEntrega.HasValue ?? false) ? cargaPedido.Pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty)
                    .Replace("#LocRemetente", cte.Remetente != null ? cte.Remetente?.Localidade?.Descricao ?? "" + " - " + cte.Remetente?.Localidade?.Estado?.Sigla ?? "" : "")
                    .Replace("#LocDestinatario", cte.Destinatario != null ? cte.Destinatario?.Localidade?.Descricao ?? "" + " - " + cte.Destinatario?.Localidade?.Estado?.Sigla ?? "" : "")
                    .Replace("#LocExpedidor", cte.Expedidor != null ? cte.Expedidor?.Localidade?.Descricao ?? "" + " - " + cte.Expedidor?.Localidade?.Estado?.Sigla ?? "" : "")
                    .Replace("#LocRecebedor", cte.Recebedor != null ? cte.Recebedor?.Localidade?.Descricao ?? "" + " - " + cte.Recebedor?.Localidade?.Estado?.Sigla ?? "" : "")
                    .Replace("#PercentualBonificacaoTransportador", percentualBonificacaoTransportador != 0m ? percentualBonificacaoTransportador.ToString("n2") : string.Empty)
                    .Replace("#UFVeiculo", estadoVeiculo)
                    .Replace("#BancoContaTomador", cte.TomadorPagador?.Cliente?.Banco?.Descricao ?? string.Empty)
                    .Replace("#AgenciaContaTomador", cte.TomadorPagador?.Cliente?.Agencia ?? string.Empty)
                    .Replace("#DigitoContaTomador", cte.TomadorPagador?.Cliente?.DigitoAgencia ?? string.Empty)
                    .Replace("#NumeroContaTomador", cte.TomadorPagador?.Cliente?.NumeroConta ?? string.Empty)
                    .Replace("#TipoContaTomador", cte.TomadorPagador?.Cliente?.TipoContaBanco.ObterDescricaoAbreviada() ?? string.Empty)
                    .Replace("#NumeroCarregamento", cargaPedido?.Carga?.Carregamento?.NumeroCarregamento ?? string.Empty)
                    .Replace("#KMTotalCarga", kmTotalCarga != 0m ? kmTotalCarga.ToString("n2") : string.Empty)
                    .Replace("#TipoCargaPedido", repCargaPedido.BuscarDescricaoTipoDeCargaPorPrioridade(cargasPedidos?.Select(x => x.Codigo).ToList()) ?? string.Empty)
                    .Replace("#TipoCarga", cargaPedido?.Carga?.TipoDeCarga?.Descricao)
                    .Replace("#OutrosNumerosCarga", repCarga.OutrosNumerosObsCte(cargaPedido?.Carga ?? null))
                    .Replace("#ValorValePedagio", valorPedagio.ToString("n2"));





                if (!string.IsNullOrEmpty(cte.Empresa?.ObservacaoCTe))
                {
                    observacao = (observacao ?? "") + (cte.Empresa?.ObservacaoCTe)
                    .Replace("#CNPJEmpresa", cte.Empresa?.CNPJ_Formatado ?? string.Empty)
                    .Replace("#RazaoSocialEmpresa", cte.Empresa?.RazaoSocial ?? string.Empty)
                    .Replace("#LocalidadeEmpresa", cte.Empresa?.Localidade?.DescricaoCidadeEstado ?? string.Empty);
                }


                if (!string.IsNullOrWhiteSpace(observacao))
                    cte.ObservacoesGerais += observacao.Trim();

                if (!string.IsNullOrWhiteSpace(observacaoCTeTerceiro) && cargaPedido != null && cargaPedido.Carga.Veiculo != null && (cargaPedido.Carga.FreteDeTerceiro || (cargaPedido.Carga.Veiculo.Proprietario != null || (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora))))
                {
                    Dominio.Entidades.Veiculo veiculo = cargaPedido.Carga.Veiculo;
                    Dominio.Entidades.Empresa empresaVeiculo = tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? veiculo.Empresa : null;

                    List<string> placasTerceiros = new List<string>() { veiculo.Placa };
                    if (cargaPedido.Carga.VeiculosVinculados != null)
                        placasTerceiros.AddRange((from o in cargaPedido.Carga.VeiculosVinculados select o.Placa).ToList());

                    List<string> renavansTerceiros = new List<string>() { veiculo.Renavam };
                    if (cargaPedido.Carga.VeiculosVinculados != null)
                        renavansTerceiros.AddRange((from o in cargaPedido.Carga.VeiculosVinculados select o.Renavam).ToList());

                    List<string> estadosTerceiros = new List<string>() { veiculo.Estado?.Sigla };
                    if (cargaPedido.Carga.VeiculosVinculados != null)
                        estadosTerceiros.AddRange(cargaPedido.Carga.VeiculosVinculados.Select(o => o.Estado?.Sigla));

                    nomeProprietarioVeiculo = veiculo.Proprietario?.Nome ?? empresaVeiculo?.RazaoSocial ?? string.Empty;
                    cpfCnpjProprietarioVeiculo = veiculo.Proprietario?.CPF_CNPJ_Formatado ?? empresaVeiculo?.CNPJ_Formatado ?? string.Empty;
                    IEProprietarioVeiculo = veiculo.Proprietario?.IE_RG ?? empresaVeiculo?.InscricaoEstadual ?? string.Empty;

                    if (veiculo.RNTRC > 0)
                        rntrcProprietarioVeiculo = string.Format("{0:00000000}", veiculo.RNTRC);

                    enderecoProprietarioVeiculo = (veiculo.Proprietario?.EnderecoCompleto ?? empresaVeiculo?.Endereco ?? string.Empty) + ", " + (veiculo.Proprietario?.Localidade?.DescricaoCidadeEstado ?? empresaVeiculo?.Localidade?.DescricaoCidadeEstado ?? string.Empty);
                    marcaVeiculoTerceiro = veiculo.Marca?.Descricao ?? string.Empty;
                    placaVeiculoTerceiro = string.Join(", ", placasTerceiros.Distinct());
                    renavamVeiculoTerceiro = string.Join(", ", renavansTerceiros.Distinct());
                    estadoVeiculoTerceiro = string.Join(", ", estadosTerceiros?.Where(o => !string.IsNullOrWhiteSpace(o)).Distinct());

                    string observacaoTerceiro = observacaoCTeTerceiro.Replace("#CPFCNPJProprietarioVeiculo", cpfCnpjProprietarioVeiculo).
                                                                      Replace("#NomeProprietarioVeiculo", nomeProprietarioVeiculo).
                                                                      Replace("#IEProprietarioVeiculo", IEProprietarioVeiculo).
                                                                      Replace("#RNTRCProprietarioVeiculo", rntrcProprietarioVeiculo).
                                                                      Replace("#EnderecoProprietarioVeiculo", enderecoProprietarioVeiculo).
                                                                      Replace("#MarcaVeiculo", marcaVeiculoTerceiro).
                                                                      Replace("#Placa", placaVeiculoTerceiro).
                                                                      Replace("#RENAVAM", renavamVeiculoTerceiro).
                                                                      Replace("#UFVeiculo", estadoVeiculoTerceiro);

                    cte.ObservacoesGerais += "\n" + observacaoTerceiro;
                }

                cte.ObservacoesGerais = Utilidades.String.Left(cte.ObservacoesGerais, 2000);

                repCTe.Atualizar(cte);
            }
            else
            {
                if (permiteObservacaoVazia)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    cte.ObservacoesGerais = "";
                    repCTe.Atualizar(cte);
                }
            }

            if (configuracaoEmbarcador.GerarObservacaoRegraICMSAposObservacaoCTe)
                cte.ObservacoesGerais += observacaoRegraICMS.Trim();
        }

        public decimal ObterPesoModeloVeicularRateado(decimal pesoDocumento, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false) ||
                (carga?.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0m) <= 0m)
                return 0m;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            decimal pesoTotalCarga = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
            decimal pesoModeloVeicular = carga.ModeloVeicularCarga.CapacidadePesoTransporte;

            decimal pesoRateado = pesoModeloVeicular / pesoTotalCarga * pesoDocumento;

            return pesoRateado;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> RetornaQuantidades(decimal pesoKG, int volumes, decimal fatorCubagem, decimal metrosCubicos, decimal pesoCubado, bool adicionarPesoModeloVeicularRateado = false, string descricaoPesoModeloVeicularRateado = null, decimal pesoModeloVeicularRateado = 0m)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            if (pesoKG > 0m || volumes <= 0)
            {
                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Quilograma",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                    Quantidade = pesoKG
                });
            }

            if (fatorCubagem > 0m && metrosCubicos > 0m)
            {
                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Peso Base Cálc. (KG)",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                    Quantidade = pesoCubado > pesoKG ? pesoCubado : pesoKG
                });

                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Cubagem",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.M3,
                    Quantidade = metrosCubicos
                });

                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Peso Cubado (KG)",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                    Quantidade = pesoCubado
                });
            }
            else if (metrosCubicos > 0m)
            {
                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Metros Cúbicos",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.M3,
                    Quantidade = metrosCubicos
                });
            }
            else if (pesoCubado > 0m)
            {
                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Peso Cubado (KG)",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                    Quantidade = pesoCubado
                });
            }

            if (volumes > 0)
            {
                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = "Unidade",
                    Unidade = Dominio.Enumeradores.UnidadeMedida.UN,
                    Quantidade = volumes
                });
            }

            if (adicionarPesoModeloVeicularRateado && !string.IsNullOrWhiteSpace(descricaoPesoModeloVeicularRateado))
            {
                quantidades.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                {
                    Medida = descricaoPesoModeloVeicularRateado,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                    Quantidade = pesoModeloVeicularRateado
                });
            }

            return quantidades;
        }

        public void AdicionarAtualizarEntregaCTe(ref Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.ObjetosDeValor.CTe.Documento docNF, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool possuiComponenteFreteComImpostoIncluso, bool emitirCteFilialEmissora, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino, bool ajusteSiniefNro8, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnterior = null, bool somarFrete = true)
        {
            int codigoLocalidadeOrigem = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.Localidade.Codigo;
            int codigoLocalidadeDestino = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Localidade.Codigo;

            if (ajusteSiniefNro8 && enderecoOrigem != null & enderecoDestino != null)
            {
                codigoLocalidadeOrigem = enderecoOrigem.Localidade.Codigo;
                codigoLocalidadeDestino = enderecoDestino.Localidade.Codigo;
            }

            int index = cte.Entregas.FindIndex(o => o.codigoLocalidadeOrigem == codigoLocalidadeOrigem &&
                            o.codigoLocalidadeDestino == codigoLocalidadeDestino);

            if (index != -1)
            {
                Dominio.ObjetosDeValor.CTe.Entrega entrega = cte.Entregas[index];

                if (somarFrete)
                {
                    if (!emitirCteFilialEmissora)
                    {
                        entrega.ValorFrete += possuiComponenteFreteComImpostoIncluso ? pedidoXMLNotaFiscal.ValorFreteComICMSIncluso : pedidoXMLNotaFiscal.ValorFrete;
                        entrega.ValorPrestacaoServico += possuiComponenteFreteComImpostoIncluso ? pedidoXMLNotaFiscal.ValorFreteComICMSIncluso : pedidoXMLNotaFiscal.ValorFrete;
                        entrega.ValorAReceber += possuiComponenteFreteComImpostoIncluso ? pedidoXMLNotaFiscal.ValorFreteComICMSIncluso : pedidoXMLNotaFiscal.ValorFrete;

                    }
                    else
                    {
                        entrega.ValorFrete += pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                        entrega.ValorPrestacaoServico += pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                        entrega.ValorAReceber += pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                    }
                }

                if (docNF != null)
                {
                    if (entrega.Documentos == null)
                        entrega.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                    entrega.Documentos.Add(docNF);
                }

                if (cteAnterior != null)
                {
                    if (entrega.DocumentosTransporteAnteriores == null)
                        entrega.DocumentosTransporteAnteriores = new List<Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior>();

                    entrega.DocumentosTransporteAnteriores.Add(this.ConverterClasseCTeParaDocumentoTransporteAnterior(cteAnterior));
                }

                AdicionarAtualizarEntregaCTeComponentePrestacao(ref entrega, pedidoXMLNotaFiscal, possuiComponenteFreteComImpostoIncluso, emitirCteFilialEmissora, unitOfWork);

                cte.Entregas[index] = entrega;
            }
            else
            {
                Dominio.ObjetosDeValor.CTe.Entrega entrega = new Dominio.ObjetosDeValor.CTe.Entrega();
                entrega.codigoLocalidadeOrigem = codigoLocalidadeOrigem;
                entrega.codigoLocalidadeDestino = codigoLocalidadeDestino;

                if (somarFrete)
                {
                    if (!emitirCteFilialEmissora)
                    {
                        entrega.ValorFrete = possuiComponenteFreteComImpostoIncluso ? pedidoXMLNotaFiscal.ValorFreteComICMSIncluso : pedidoXMLNotaFiscal.ValorFrete;
                        entrega.ValorPrestacaoServico = possuiComponenteFreteComImpostoIncluso ? pedidoXMLNotaFiscal.ValorFreteComICMSIncluso : pedidoXMLNotaFiscal.ValorFrete;
                        entrega.ValorAReceber = possuiComponenteFreteComImpostoIncluso ? pedidoXMLNotaFiscal.ValorFreteComICMSIncluso : pedidoXMLNotaFiscal.ValorFrete;
                    }
                    else
                    {
                        entrega.ValorFrete = pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                        entrega.ValorPrestacaoServico = pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                        entrega.ValorAReceber = pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                    }
                }

                if (docNF != null)
                {
                    entrega.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();
                    entrega.Documentos.Add(docNF);
                }

                if (docNF != null)
                {
                    entrega.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();
                    entrega.Documentos.Add(docNF);
                }

                if (cteAnterior != null)
                {
                    entrega.DocumentosTransporteAnteriores = new List<Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior>();
                    entrega.DocumentosTransporteAnteriores.Add(this.ConverterClasseCTeParaDocumentoTransporteAnterior(cteAnterior));
                }

                AdicionarAtualizarEntregaCTeComponentePrestacao(ref entrega, pedidoXMLNotaFiscal, possuiComponenteFreteComImpostoIncluso, emitirCteFilialEmissora, unitOfWork);

                cte.Entregas.Add(entrega);
            }
        }

        public void AdicionarAtualizarEntregaCTeComponentePrestacao(ref Dominio.ObjetosDeValor.CTe.Entrega Entrega, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool possuiComponenteFreteComImpostoIncluso, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXmlNFCompontentesFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscalComPisCofins(pedidoXMLNotaFiscal.Codigo, pedidoXMLNotaFiscal.ModeloDocumentoFiscal, componenteFilialEmissora);

            if (Entrega.ComponentesPrestacao == null)
                Entrega.ComponentesPrestacao = new List<Dominio.ObjetosDeValor.CTe.EntregaComponentePrestacao>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXmlNFComponenteFrete in pedidoXmlNFCompontentesFrete)
            {
                if (pedidoXmlNFComponenteFrete.DescontarValorTotalAReceber)
                    continue;

                int index = Entrega.ComponentesPrestacao
                   .FindIndex(obj => obj.TipoComponenteFrete == pedidoXmlNFComponenteFrete.TipoComponenteFrete &&
                                     obj.CodigoComponenteFrete == pedidoXmlNFComponenteFrete.ComponenteFrete.Codigo);

                if (index != -1)
                {
                    Dominio.ObjetosDeValor.CTe.EntregaComponentePrestacao entregaComponenteFrete = Entrega.ComponentesPrestacao[index];
                    entregaComponenteFrete.Valor += pedidoXmlNFComponenteFrete.ValorComponente;
                    Entrega.ComponentesPrestacao[index] = entregaComponenteFrete;
                }
                else
                {
                    Dominio.ObjetosDeValor.CTe.EntregaComponentePrestacao entregaComponenteFrete = new Dominio.ObjetosDeValor.CTe.EntregaComponentePrestacao();
                    entregaComponenteFrete.TipoComponenteFrete = pedidoXmlNFComponenteFrete.TipoComponenteFrete;

                    if (string.IsNullOrWhiteSpace(pedidoXmlNFComponenteFrete.ComponenteFrete.DescricaoCTe))
                        entregaComponenteFrete.Descricao = pedidoXmlNFComponenteFrete.ComponenteFrete.DescricaoComponente.Left(15).Trim();
                    else
                        entregaComponenteFrete.Descricao = pedidoXmlNFComponenteFrete.ComponenteFrete.DescricaoCTe.Left(15).Trim();

                    if (possuiComponenteFreteComImpostoIncluso)
                        entregaComponenteFrete.Valor = pedidoXmlNFComponenteFrete.ValorComponenteComICMSIncluso;
                    else
                        entregaComponenteFrete.Valor = pedidoXmlNFComponenteFrete.ValorComponente;

                    entregaComponenteFrete.IncluiBaseCalculoICMS = pedidoXmlNFComponenteFrete.IncluirBaseCalculoICMS;
                    entregaComponenteFrete.IncluiValorAReceber = true;
                    entregaComponenteFrete.CodigoComponenteFrete = pedidoXmlNFComponenteFrete.ComponenteFrete.Codigo;

                    Entrega.ComponentesPrestacao.Add(entregaComponenteFrete);
                }
            }
        }

        public Dominio.ObjetosDeValor.CTe.Documento BuscarDocumentoCTe(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Enumeradores.TipoAmbiente tipoAmbienteEmpresa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Dominio.ObjetosDeValor.CTe.Documento docNF = new Dominio.ObjetosDeValor.CTe.Documento();
            docNF.Valor = xmlNotaFiscal.Valor;
            docNF.Numero = !string.IsNullOrEmpty(xmlNotaFiscal.NumeroOutroDocumento) ? xmlNotaFiscal.NumeroOutroDocumento : xmlNotaFiscal.Numero.ToString();
            docNF.CFOP = string.IsNullOrEmpty(xmlNotaFiscal.Chave) && !string.IsNullOrEmpty(xmlNotaFiscal.CFOP) ? xmlNotaFiscal.CFOP : "0";
            docNF.ChaveNFE = xmlNotaFiscal.Chave;
            docNF.ProtocoloAutorizacao = xmlNotaFiscal.Protocolo;
            docNF.DataEmissao = xmlNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
            docNF.ModeloDocumentoFiscal = xmlNotaFiscal.Modelo;
            docNF.Peso = configuracaoEmbarcador.UtilizarPesoLiquidoNFeParaCTeMDFe && xmlNotaFiscal.PesoLiquido > 0 ? xmlNotaFiscal.PesoLiquido : xmlNotaFiscal.Peso;
            docNF.Volume = xmlNotaFiscal.Volumes;
            docNF.Serie = xmlNotaFiscal.Serie;
            docNF.ValorICMS = xmlNotaFiscal.ValorICMS;
            docNF.ValorICMSST = xmlNotaFiscal.ValorST;
            docNF.ValorProdutos = xmlNotaFiscal.ValorTotalProdutos;
            docNF.BaseCalculoICMS = xmlNotaFiscal.BaseCalculoICMS;
            docNF.BaseCalculoICMSST = xmlNotaFiscal.BaseCalculoST;
            docNF.protocoloNFe = xmlNotaFiscal.Codigo;
            docNF.NumeroReferenciaEDI = xmlNotaFiscal.NumeroReferenciaEDI;
            docNF.NumeroControleCliente = xmlNotaFiscal.NumeroControleCliente;
            docNF.NumeroRomaneio = xmlNotaFiscal.NumeroRomaneio;
            docNF.NumeroPedido = xmlNotaFiscal.NumeroPedidoEmbarcador;
            docNF.NomeDestinatario = xmlNotaFiscal.NomeDestinatario;
            docNF.IEDestinatario = xmlNotaFiscal.IEDestinatario;
            docNF.IERemetente = xmlNotaFiscal.IERemetente;
            docNF.NCMPredominante = xmlNotaFiscal.NCM;
            docNF.ClassificacaoNFe = xmlNotaFiscal.ClassificacaoNFe;

            if (xmlNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe || xmlNotaFiscal.TipoDocumento == 0)
            {
                if (tipoAmbienteEmpresa == Dominio.Enumeradores.TipoAmbiente.Producao || configuracaoEmbarcador.UtilizarNFeEmHomologacao)
                {
                    docNF.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe;
                    docNF.ModeloDocumentoFiscal = "55";
                }
                else
                {
                    //utilizado apenas para teste em ambiente de Homologação
                    docNF.ModeloDocumentoFiscal = "99";
                    docNF.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.Outros;
                    docNF.Descricao = "Teste Homologacao";
                }
            }
            else if (xmlNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NotaFiscal)
            {
                docNF.ModeloDocumentoFiscal = xmlNotaFiscal.Modelo ?? "01";
                docNF.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NF;
                docNF.Descricao = xmlNotaFiscal.Descricao;
            }
            else
            {
                docNF.ModeloDocumentoFiscal = "99";
                docNF.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.Outros;
                docNF.Descricao = xmlNotaFiscal.Descricao;
            }
            return docNF;
        }

        public Dominio.ObjetosDeValor.CTe.Documento BuscarDocumentoCTe(Dominio.Entidades.DocumentosCTE documentoCTe, Dominio.Enumeradores.TipoAmbiente tipoAmbienteEmpresa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Dominio.ObjetosDeValor.CTe.Documento docNF = new Dominio.ObjetosDeValor.CTe.Documento();
            docNF.Valor = documentoCTe.Valor;
            docNF.Numero = documentoCTe.Numero;
            docNF.CFOP = string.IsNullOrEmpty(documentoCTe.ChaveNFE) && !string.IsNullOrEmpty(documentoCTe.CFOP) ? documentoCTe.CFOP : "0";
            docNF.ChaveNFE = documentoCTe.ChaveNFE;
            docNF.DataEmissao = documentoCTe.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
            docNF.ModeloDocumentoFiscal = documentoCTe.ModeloDocumentoFiscal?.Numero ?? "99";
            docNF.Peso = documentoCTe.Peso;
            docNF.Volume = documentoCTe.Volume;
            docNF.Serie = documentoCTe.Serie;
            docNF.ValorICMS = documentoCTe.ValorICMS;
            docNF.ValorICMSST = documentoCTe.ValorICMSST;
            docNF.ValorProdutos = documentoCTe.ValorProdutos;
            docNF.BaseCalculoICMS = documentoCTe.BaseCalculoICMS;
            docNF.BaseCalculoICMSST = documentoCTe.BaseCalculoICMSST;
            docNF.ProtocoloAutorizacao = documentoCTe.ProtocoloNFe;
            docNF.NumeroReferenciaEDI = documentoCTe.NumeroReferenciaEDI;
            docNF.NumeroControleCliente = documentoCTe.NumeroControleCliente;
            //docNF.protocoloNFe = documentoCTe.Codigo;
            docNF.NumeroRomaneio = documentoCTe.NumeroRomaneio;
            docNF.NumeroPedido = documentoCTe.NumeroPedido;
            docNF.NCMPredominante = documentoCTe.NCMPredominante;

            return docNF;
        }

        public string BuscarProdutoPredominante(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();
            Servicos.Embarcador.Pedido.Produto serPedidoProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);

            string produtoPredominante;

            if (!string.IsNullOrWhiteSpace(cargaPedido.Carga.TipoDeCarga?.ProdutoPredominante))
                produtoPredominante = cargaPedido.Carga.TipoDeCarga.ProdutoPredominante;
            else if (!string.IsNullOrWhiteSpace(cargaPedido.Carga.TipoOperacao?.ProdutoPredominanteOperacao))
                produtoPredominante = cargaPedido.Carga.TipoOperacao.ProdutoPredominanteOperacao;
            else
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (!string.IsNullOrWhiteSpace(tomador?.GrupoPessoas?.ProdutoPredominante))
                    produtoPredominante = tomador.GrupoPessoas.ProdutoPredominante;
                else
                {
                    if (configuracaoEmbarcador.BuscarProdutoPredominanteNoPedido)
                        produtoPredominante = serPedidoProduto.BuscarProdutoPredominanteEntreOsPedidos(cargaPedidos, unitOfWork);
                    else
                        produtoPredominante = configuracaoEmbarcador.DescricaoProdutoPredominatePadrao;

                    if (string.IsNullOrWhiteSpace(produtoPredominante))
                    {
                        produtoPredominante = cargaPedido.Pedido.ProdutoPredominante;
                        if (string.IsNullOrWhiteSpace(produtoPredominante) && !configuracaoEmbarcador.BuscarProdutoPredominanteNoPedido)
                            produtoPredominante = serPedidoProduto.BuscarProdutoPredominanteEntreOsPedidos(cargaPedidos, unitOfWork);
                    }

                }
            }

            return produtoPredominante;
        }

        public string BuscarProdutoPredominante(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            string produtoPredominante;
            Servicos.Embarcador.Pedido.Produto serPedidoProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);

            if (!string.IsNullOrWhiteSpace(cargaPedido.Carga.TipoDeCarga?.ProdutoPredominante))
                produtoPredominante = cargaPedido.Carga.TipoDeCarga.ProdutoPredominante;
            else if (!string.IsNullOrWhiteSpace(cargaPedido.Carga.TipoOperacao?.ProdutoPredominanteOperacao))
                produtoPredominante = cargaPedido.Carga.TipoOperacao.ProdutoPredominanteOperacao;
            else
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (!string.IsNullOrWhiteSpace(tomador?.GrupoPessoas?.ProdutoPredominante))
                    produtoPredominante = tomador.GrupoPessoas.ProdutoPredominante;
                else
                {
                    if (configuracaoEmbarcador.BuscarProdutoPredominanteNoPedido)
                        produtoPredominante = serPedidoProduto.BuscarProdutoPredominanteEntreOsPedidos(cargaPedido, unitOfWork);
                    else
                        produtoPredominante = configuracaoEmbarcador.DescricaoProdutoPredominatePadrao;

                    if (string.IsNullOrWhiteSpace(produtoPredominante))
                    {
                        produtoPredominante = cargaPedido.Pedido.ProdutoPredominante;
                        if (string.IsNullOrWhiteSpace(produtoPredominante) && !configuracaoEmbarcador.BuscarProdutoPredominanteNoPedido)
                            produtoPredominante = serPedidoProduto.BuscarProdutoPredominanteEntreOsPedidos(cargaPedido, unitOfWork);
                    }
                }
            }

            return produtoPredominante;
        }

        private string BuscarDescricaoCaracteristicaTransporteCTe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(cargaOcorrencia?.TipoOcorrencia?.CaracteristicaAdicionalCTe))
                return cargaOcorrencia.TipoOcorrencia.CaracteristicaAdicionalCTe;

            if (cargaPedido != null)
            {
                if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao && !string.IsNullOrWhiteSpace(cargaPedido.Carga.TipoOperacao.CaracteristicaTransporteCTe))
                    return cargaPedido.Carga.TipoOperacao.CaracteristicaTransporteCTe;
                else if (cargaPedido.ModalPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.Nenhum)
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodalHelper.ObterAbreviacao(cargaPedido.ModalPropostaMultimodal);
                else
                {
                    Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                    if (tomador != null)
                    {
                        if (tomador.NaoUsarConfiguracaoEmissaoGrupo && !string.IsNullOrWhiteSpace(tomador.CaracteristicaTransporteCTe))
                            return tomador.CaracteristicaTransporteCTe;
                        else if (tomador.GrupoPessoas != null && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.CaracteristicaTransporteCTe))
                            return tomador.GrupoPessoas.CaracteristicaTransporteCTe;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retorna como deve ser feita a emissão dos Documentos, por exemplo, vai emitir um CT-e para cada nota fiscal. Outro Exemplo: vai emitir um único CT-e para todo o pedido.
        /// </summary>
        /// <param name="cargaPedidos"></param>
        /// <param name="tipoServicoMultisoftware"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos BuscarTipoEmissaoDocumentosCTe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Cliente tomadorEmMemoria = null)
        {
            Dominio.Entidades.Cliente tomador = null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

            if (tomadorEmMemoria != null)
                tomador = tomadorEmMemoria;
            else
                tomador = cargaPedido.ObterTomador();

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentosExclusivo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado;

            if (tomador != null)
            {
                FormaEmissaoSVM formaEmissaoSVM = cargaPedido.Pedido.PortoDestino?.FormaEmissaoSVM ?? FormaEmissaoSVM.Nenhum;
                bool ignorarRateioConfiguradoPorto = !(cargaPedido.Pedido.TipoOperacao?.ConfiguracaoCarga?.IgnorarRateioConfiguradoPorto ?? false);
                bool contemConfiguracaoExclusica = false;

                if (formaEmissaoSVM == FormaEmissaoSVM.PortoDestino && ignorarRateioConfiguradoPorto)
                {
                    if (repCargaPedido.QuantidadeCargaPedido(cargaPedido.Carga.Codigo) >= 2)
                    {
                        contemConfiguracaoExclusica = true;
                        tipoEmissaoCTeDocumentosExclusivo = TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual;
                    }
                    else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo != null && tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo.HasValue && tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                    {
                        contemConfiguracaoExclusica = true;
                        tipoEmissaoCTeDocumentosExclusivo = tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo.Value;
                    }
                    else if (tomador.TipoEmissaoCTeDocumentosExclusivo != null && tomador.TipoEmissaoCTeDocumentosExclusivo.HasValue && tomador.TipoEmissaoCTeDocumentosExclusivo.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                    {
                        contemConfiguracaoExclusica = true;
                        tipoEmissaoCTeDocumentosExclusivo = tomador.TipoEmissaoCTeDocumentosExclusivo.Value;
                    }
                }
                else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo != null && tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo.HasValue && tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                {
                    contemConfiguracaoExclusica = true;
                    tipoEmissaoCTeDocumentosExclusivo = tomador.GrupoPessoas.TipoEmissaoCTeDocumentosExclusivo.Value;
                }
                else if (tomador.TipoEmissaoCTeDocumentosExclusivo != null && tomador.TipoEmissaoCTeDocumentosExclusivo.HasValue && tomador.TipoEmissaoCTeDocumentosExclusivo.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                {
                    contemConfiguracaoExclusica = true;
                    tipoEmissaoCTeDocumentosExclusivo = tomador.TipoEmissaoCTeDocumentosExclusivo.Value;
                }


                if (!contemConfiguracaoExclusica || tipoEmissaoCTeDocumentosExclusivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                {
                    tipoEmissaoCTeDocumentos = tomador.TipoEmissaoCTeDocumentos;

                    if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                        tipoEmissaoCTeDocumentos = cargaPedido.Carga.TipoOperacao.TipoEmissaoCTeDocumentos;
                    else
                    {
                        if (!tomador.NaoUsarConfiguracaoEmissaoGrupo && tomador.GrupoPessoas != null)
                            tipoEmissaoCTeDocumentos = tomador.GrupoPessoas.TipoEmissaoCTeDocumentos;
                    }

                    if (tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado)
                    {
                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            tipoEmissaoCTeDocumentos = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada;
                        else
                        {
                            tipoEmissaoCTeDocumentos = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado;
                        }
                    }
                }
                else
                    tipoEmissaoCTeDocumentos = tipoEmissaoCTeDocumentosExclusivo;
            }
            else
            {
                if (cargaPedido != null && cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                    tipoEmissaoCTeDocumentos = cargaPedido.Carga.TipoOperacao.TipoEmissaoCTeDocumentos;
            }

            if (cargaPedido.Carga.CargaEspelho != null && tipoEmissaoCTeDocumentos != TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual)
                Log.GravarInfo($"Carga ({cargaPedido.Carga.Codigo}) Tipo de rateio do Divergente: {tipoEmissaoCTeDocumentos}", "TipoRateioCargaPedido");

            return tipoEmissaoCTeDocumentos;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes BuscarTipoEmissaoCTeParticipantes(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool utilizarParticipantesDaCargaPeloPedido)
        {
            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
            //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = cargaPedido.TipoEmissaoCTeParticipantes;
            if (utilizarParticipantesDaCargaPeloPedido && cargaPedido.Pedido != null)
            {
                if (cargaPedido.Pedido.Recebedor != null && cargaPedido.Pedido.Expedidor != null)
                    tipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                else if (cargaPedido.Pedido.Recebedor == null && cargaPedido.Pedido.Expedidor != null)
                    tipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComExpedidor;
                else if (cargaPedido.Pedido.Recebedor != null && cargaPedido.Pedido.Expedidor == null)
                    tipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComRecebedor;
                else if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                    tipoEmissaoCTeParticipantes = cargaPedido.Carga.TipoOperacao.TipoEmissaoCTeParticipantes;
                else if (tomador != null && tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    tipoEmissaoCTeParticipantes = tomador.TipoEmissaoCTeParticipantes;
                else if (tomador != null && !tomador.NaoUsarConfiguracaoEmissaoGrupo && tomador.GrupoPessoas != null)
                    tipoEmissaoCTeParticipantes = tomador.GrupoPessoas.TipoEmissaoCTeParticipantes;
            }
            else if (tomador != null)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                        tipoEmissaoCTeParticipantes = cargaPedido.Carga.TipoOperacao.TipoEmissaoCTeParticipantes;
                    else if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        tipoEmissaoCTeParticipantes = tomador.TipoEmissaoCTeParticipantes;
                    else if (!tomador.NaoUsarConfiguracaoEmissaoGrupo && tomador.GrupoPessoas != null)
                        tipoEmissaoCTeParticipantes = tomador.GrupoPessoas.TipoEmissaoCTeParticipantes;
                }
                /*#4414 - Estava alterando o tipo de participante recebedor ja definido na carga pedido
                else
                {
                    tipoEmissaoCTeParticipantes = tomador.TipoEmissaoCTeParticipantes;
                }
                */
            }

            return tipoEmissaoCTeParticipantes;
        }

        public bool VerificarSeEmissaoSeraPorNota(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos)
        {
            if (tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerificarSeEmissaoSeraPorComTransbordo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComTransbordo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool VerificarSeEmissaoSeraPorNotaComRecebedor(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerificarSePercursoDestinoSeraPorNota(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool validar = false)
        {
            if ((tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                && (tipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComTransbordo
                && tipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor
                && (validar ? tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? tipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor : true : tipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
                && tipoEmissaoCTeParticipantes != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerificarSePercursoDestinoSeraPorPedido(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            if ((tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerificarSePercursoOrigemSeraPorNota(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            if ((tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                && (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ConverterCTesTerceirosParaAnteriores(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
        {
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
                cteAnterior.Emitente = serEmpresa.ConverterObjetoEmpresa(cte.Empresa);
                cteAnterior.Chave = cte.Chave;
                ctesAnteriores.Add(cteAnterior);
            }

            return ctesAnteriores;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterSegurosDeTerceirosEmSeguroCTe(List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> seguros, Dominio.Enumeradores.TipoTomador tipoTomador, decimal valorMercadoria)
        {
            if (seguros == null || seguros.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apoliceSeguros = seguros.Select(o => o.ApoliceSeguro).ToList();

            return ConverterApolicesSeguroEmSeguroCTe(apoliceSeguros, tipoTomador, valorMercadoria);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterApolicesSeguroEmSeguroCTe(List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apoliceSeguros, Dominio.Enumeradores.TipoTomador tipoTomador, decimal valorMercadoria)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro seguro in apoliceSeguros)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.Seguro apoliceSeguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                apoliceSeguro.Apolice = seguro.NumeroApolice;
                apoliceSeguro.Averbacao = seguro.NumeroAverbacao;
                apoliceSeguro.Seguradora = seguro.Seguradora.Nome;
                apoliceSeguro.CNPJSeguradora = seguro.Seguradora.ClienteSeguradora != null ? seguro.Seguradora.ClienteSeguradora.CPF_CNPJ_SemFormato : "";
                apoliceSeguro.Valor = valorMercadoria;

                Dominio.Enumeradores.TipoSeguro responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Remetente;

                if (seguro.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Transportador)
                    responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Emitente_CTE;
                else
                {
                    if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Remetente;
                    else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Destinatario;
                    else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Expedidor;
                    else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Recebedor;
                    else
                        responsavelSeguro = Dominio.Enumeradores.TipoSeguro.Tomador_Servico;
                }
                apoliceSeguro.ResponsavelSeguro = responsavelSeguro;

                apolicesSeguro.Add(apoliceSeguro);
            }

            return apolicesSeguro;
        }

        private void AdicionarTaxaEmissaoCarga(Dominio.ObjetosDeValor.CTe.CTe cte, decimal valorTaxa, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedido == null || cargaPedido.Carga == null || valorTaxa <= 0)
                return;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicoesFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
            composicoesFrete.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Taxa adicionado pela integração", " Valor da Taxa = " + valorTaxa.ToString("n2"), valorTaxa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "TAXA DE EMISSÃO", 0, valorTaxa));

            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(cargaPedido.Carga, null, null, null, false, composicoesFrete, unitOfWork, null);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
            carga.ValorFrete += valorTaxa;
            repCarga.Atualizar(carga);
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete GerarComponenteISS(Dominio.ObjetosDeValor.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);

            if (cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componenteCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete();

                componenteCTe.NaoSomarValorTotalPrestacao = false;
                componenteCTe.NaoSomarValorTotalAReceber = false;
                componenteCTe.AcrescentaValorTotalAReceber = false;
                componenteCTe.ComponenteFrete = componenteFrete;
                componenteCTe.DescontarValorTotalAReceber = false;
                componenteCTe.IncluirBaseCalculoICMS = false;
                componenteCTe.IncluirIntegralmenteContratoFreteTerceiro = false;
                componenteCTe.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                componenteCTe.ValorComponente = cte.ISS.Valor;

                return componenteCTe;
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete GerarComponenteICMS(Dominio.ObjetosDeValor.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);

            if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.ICMS != null && cte.ICMS.CST != "60" && (cte.ICMS.Valor > 0m || cte.ICMS.ValorIncluso > 0m))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componenteCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete();

                componenteCTe.NaoSomarValorTotalPrestacao = false;
                componenteCTe.NaoSomarValorTotalAReceber = false;
                componenteCTe.AcrescentaValorTotalAReceber = false;
                componenteCTe.ComponenteFrete = componenteFrete;
                componenteCTe.DescontarValorTotalAReceber = false;
                componenteCTe.IncluirBaseCalculoICMS = false;
                componenteCTe.IncluirIntegralmenteContratoFreteTerceiro = false;
                componenteCTe.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                componenteCTe.ValorComponente = cte.ICMS.ValorIncluso > 0m ? cte.ICMS.ValorIncluso : cte.ICMS.Valor;

                return componenteCTe;
            }

            return null;
        }

        private int ObterQuantidadeDocumentosAtualizarCarga(int totalDocumentos)
        {
            int quantidadeDocumentosAtualizarCarga = (int)Math.Round((totalDocumentos * 0.1m), 0, MidpointRounding.ToEven);

            if (quantidadeDocumentosAtualizarCarga > 10)
                quantidadeDocumentosAtualizarCarga = 10;
            else if (quantidadeDocumentosAtualizarCarga < 2)
                quantidadeDocumentosAtualizarCarga = 2;

            return quantidadeDocumentosAtualizarCarga;
        }

        private void VincularCTeACarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente cliTomador, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals, string observacaoCTe, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoParaObservacaoCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuals)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDaCarga = (from obj in cargaPedidos where obj.CargaOrigem.Codigo == carga.Codigo select obj).Distinct().ToList();

            if (cargaPedidosDaCarga.Count <= 0)
                return;

            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Servicos.Embarcador.NFSe.NFSe serNFSe = new NFSe.NFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
            cargaCTE.Carga = carga.CargaAgrupamento != null ? carga.CargaAgrupamento : carga;
            cargaCTE.CargaOrigem = carga;

            if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.Numero == "39" && cliTomador.Localidade.Codigo != carga.Empresa.Localidade.Codigo)
            {
                cteIntegrado.TomadorPagador.InscricaoMunicipal = "";
                repParticipanteCTe.Atualizar(cteIntegrado.TomadorPagador);
            }

            if (xmlNotasFiscais != null && xmlNotasFiscais.Count > 0)
                cteIntegrado.XMLNotaFiscais = xmlNotasFiscais;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!configuracaoTMS.NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe && (string.IsNullOrWhiteSpace(observacaoCTe) || !observacaoCTe.Contains("#NumeroPedido")) && pedidoParaObservacaoCTe != null && !string.IsNullOrWhiteSpace(pedidoParaObservacaoCTe.NumeroPedidoEmbarcador))
                    observacaoCTe += " Número do DT: #NumeroPedido.";

                if (carga.Lacres.Count > 0)
                    observacaoCTe += " Lacres: " + string.Join(", ", carga.Lacres.Select(o => o.Numero)) + ".";
            }

            //SetarObservacaoCTe(cargaPedido, cteIntegrado, observacaoCTe, rotas, pedidoParaObservacaoCTe, false, unitOfWork);
            cargaCTE.CTe = cteIntegrado;
            cargaCTE.LancamentoNFSManual = lancamentoNFSManual;
            cargaCTE.LancamentoManualPossuiNFSOcorrencia = cargaDocumentoParaEmissaoNFSManuals.Any(obj => obj.CargaOcorrencia != null && obj.Carga.Codigo == cargaCTE.Carga.Codigo);
            cargaCTE.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;

            repCargaCte.Inserir(cargaCTE);


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosDaCarga)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = (from obj in pedidoXMLNotaFiscals where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).Distinct().ToList();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                }
            }
        }

        private void DefinirNotasRemessaEntregaParaTransporteObservacao(ref string observacaoCTe, Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe classificacaoNFeObservacao)
        {
            if (cte.Documentos.Any(x => x.ClassificacaoNFe == classificacaoNFeObservacao))
            {
                observacaoCTe += $" NFe {classificacaoNFeObservacao.ObterDescricao()}: {string.Join(", ", (from obj in cte.Documentos where obj.ClassificacaoNFe == classificacaoNFeObservacao select obj.ChaveNFE).Distinct().ToList())}";
                cte.Documentos = (from obj in cte.Documentos where obj.ClassificacaoNFe != classificacaoNFeObservacao select obj).ToList();
            }
        }

        private static void AtualizarValorComponenteICMSFreteProprio(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            decimal valorICMS = cte.ValorICMS;

            if (valorICMS == 0)
                return;

            Repositorio.ComponentePrestacaoCTE repositorioComponentePrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTe = repositorioComponentePrestacaoCTE.BuscarPorCTe(cte.Codigo);
            if (componentesPrestacaoCTe == null || !componentesPrestacaoCTe.Any())
                return;

            Dominio.Entidades.ComponentePrestacaoCTE componenteImposto = componentesPrestacaoCTe.FirstOrDefault(componente => componente.ValorImpostos > 0);
            if (componenteImposto == null)
                return;

            componenteImposto.Valor = Math.Max(0, componenteImposto.Valor - valorICMS);
            if (componenteImposto.Valor > 0)
                repositorioComponentePrestacaoCTE.Atualizar(componenteImposto);
            else
                repositorioComponentePrestacaoCTE.Deletar(componenteImposto);

            Dominio.Entidades.ComponentePrestacaoCTE componenteValorFrete = componentesPrestacaoCTe.FirstOrDefault(componente => componente.ValorFrete > 0);
            if (componenteValorFrete == null)
                return;

            componenteValorFrete.Valor += valorICMS;
            repositorioComponentePrestacaoCTE.Atualizar(componenteValorFrete);
        }

        private Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior ConverterClasseCTeParaDocumentoTransporteAnterior(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnt)
        {
            Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior documentoAnterior = new Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior();
            documentoAnterior.TipoDocumento = Dominio.Enumeradores.TipoDocumentoAnteriorCTe.Eletronico;
            documentoAnterior.Chave = cteAnt.Chave;
            documentoAnterior.Emissor = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = cteAnt.Emitente.Endereco.Bairro,
                CEP = cteAnt.Emitente.Endereco.CEP,
                CodigoAtividade = cteAnt.Emitente.Atividade,
                CodigoIBGECidade = cteAnt.Emitente.Endereco.Cidade.IBGE,
                Complemento = cteAnt.Emitente.Endereco.Complemento,
                CPFCNPJ = cteAnt.Emitente.CNPJ,
                Endereco = cteAnt.Emitente.Endereco.Logradouro,
                NomeFantasia = cteAnt.Emitente.NomeFantasia,
                Numero = cteAnt.Emitente.Endereco.Numero,
                Exportacao = false,
                RazaoSocial = cteAnt.Emitente.RazaoSocial,
                RGIE = cteAnt.Emitente.IE,
                Telefone1 = cteAnt.Emitente.Endereco.Telefone,
                Emails = cteAnt.Emitente.Emails,
                StatusEmails = true,
                NaoAtualizarDadosCadastrais = true
            };

            return documentoAnterior;
        }

        #endregion
    }
}
